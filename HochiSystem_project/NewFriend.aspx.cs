using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Services;
using System.Web.Services;

public partial class CRM_NewFriend : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // 先吃 QueryString ?myHid=xxxx（活動 QR 推薦帶這個）
        var hid = Request["myHid"];

        // 後援：如果沒帶，就嘗試 Cookie（未來併回 Master 可用）
        if (string.IsNullOrEmpty(hid))
            hid = GetCookie("person_id");

        // 仍取不到就置 0，前端會擋住儲存按鈕
        hidMyHID.Value = string.IsNullOrEmpty(hid) ? "0" : hid;

        // 同時把 channel 也承接（預設道場）
        var ch = Request["channel"];
        hidChannel.Value = string.IsNullOrEmpty(ch) ? "道場" : ch;
    }


    private string ConnStr { get { return ConfigurationManager.ConnectionStrings["HochiReports"].ConnectionString; } }

    private static string GetCookie(string key)
    {
        var c = System.Web.HttpContext.Current.Request.Cookies[key];
        return c == null ? null : c.Value;
    }

    #region DTO
    [Serializable]
    public class NFDto
    {
        public string FullName { get; set; }
        public string MobilePhone { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
    }
    public class SearchResult { public bool Found; public object Data; }
    public class ApiResult { public bool Ok; public string Msg; public int NewFriendId; }
    [Serializable]
    public class InteractionDto
    {
        public int NewFriendId { get; set; }
        public int ContactHID { get; set; }
        public string Method { get; set; }
        public string IntentLevel { get; set; }
        public string NextAction { get; set; }
        public string NextActionDate { get; set; }
        public string Memo { get; set; }

        // 同步主檔
        public string FullName { get; set; }
        public string MobilePhone { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
    }

    #endregion

    #region Utils
    private static string NormalizeMobile(string mobile)
    {
        if (string.IsNullOrEmpty(mobile)) return null;
        var digits = System.Text.RegularExpressions.Regex.Replace(mobile, @"\D", "");
        if (string.IsNullOrEmpty(digits)) return null; // ★ 關鍵：完全沒有數字就回傳 null

        if (digits.StartsWith("09")) return "+886" + digits.Substring(1);
        if (digits.StartsWith("9") && digits.Length == 9) return "+886" + digits;
        if (digits.StartsWith("886")) return "+" + digits;
        if (digits.StartsWith("0")) return "+886" + digits.Substring(1);
        return "+" + digits;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "";
        return name.Replace(" ", "").Replace("　", "").Trim();
    }
    #endregion

    [WebMethod, ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static SearchPayload SearchPerson(string mobile, string name)
    {
        var payload = new SearchPayload { Found = false, Multiple = false, Data = null, Candidates = new List<PersonCandidate>() };

        var connStr = ConfigurationManager.ConnectionStrings["HochiReports"].ConnectionString;
        string m = NormalizeMobile(mobile);
        string n = NormalizeName(name);

        using (var cn = new SqlConnection(connStr))
        using (var cmd = cn.CreateCommand())
        {
            cn.Open();

            if (!string.IsNullOrEmpty(m))
            {
                // 先用手機精準
                cmd.CommandText = @"
SELECT TOP 20 source_type, newfriend_id, edu_hid, full_name, mobile_norm, City, District, Address
FROM dbo.v_crm_people_search
WHERE mobile_norm = @m
ORDER BY CASE WHEN source_type='NF' THEN 0 ELSE 1 END, full_name";
                cmd.Parameters.AddWithValue("@m", m);
            }
            else if (!string.IsNullOrEmpty(n))
            {
                // 再用姓名（模糊 -> 前後含，視需求可改為等於或前綴）
                cmd.CommandText = @"
SELECT TOP 20 source_type, newfriend_id, edu_hid, full_name, mobile_norm, City, District, Address
FROM dbo.v_crm_people_search
WHERE full_name LIKE @n
ORDER BY CASE WHEN source_type='NF' THEN 0 ELSE 1 END, full_name";
                cmd.Parameters.AddWithValue("@n", "%" + n + "%");
            }
            else
            {
                return payload; // 沒條件就不查
            }

            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    payload.Candidates.Add(new PersonCandidate
                    {
                        SourceType = r["source_type"] as string,
                        NewFriendId = r["newfriend_id"] as int?,
                        EduHID = r["edu_hid"] as int?,
                        FullName = r["full_name"] as string,
                        MobilePhone = r["mobile_norm"] as string,
                        City = r["City"] as string,
                        District = r["District"] as string,
                        Address = r["Address"] as string
                    });
                }
            }
        }

        payload.Found = payload.Candidates.Count > 0;
        if (!payload.Found) return payload;

        if (payload.Candidates.Count == 1)
        {
            payload.Multiple = false;
            var c = payload.Candidates[0];
            // 舊版 Data 結構（表單可直接塞）
            payload.Data = new
            {
                NewFriendId = c.NewFriendId ?? 0,
                FullName = c.FullName,
                MobilePhone = c.MobilePhone,
                City = c.City,
                District = c.District,
                Address = c.Address
            };
        }
        else
        {
            payload.Multiple = true;
        }
        return payload;
    }


    [WebMethod, ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static ApiResult CreateNewFriend(NFDto dto, int createdByHID, string channel)
    {
        if (dto == null)
            return new ApiResult { Ok = false, Msg = "無法解析參數：請重新整理頁面後再試（AddInteraction）" };

        if (createdByHID <= 0)
            return new ApiResult { Ok = false, Msg = "缺少建立者 ContactHID（myHid）。請用 ?myHid= 開啟本頁" };

        var connStr = ConfigurationManager.ConnectionStrings["HochiReports"].ConnectionString;
        var name = NormalizeName(dto.FullName);
        var mobile = NormalizeMobile(dto.MobilePhone);
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(mobile))
            return new ApiResult { Ok = false, Msg = "姓名與手機為必填" };

        using (var cn = new SqlConnection(connStr))
        {
            cn.Open(); // 先開連線
            using (var tx = cn.BeginTransaction("nf_create")) // 再開始交易
            using (var cmd = cn.CreateCommand())
            {
                cmd.Transaction = tx;
                try
                {
                    int nid = 0;

                    // 先用手機判重（決定性）
                    cmd.CommandText = @"SELECT TOP 1 NewFriendId 
                                    FROM dbo.NewFriend 
                                    WHERE MobilePhone=@m AND IsMergedIntoId IS NULL";
                    cmd.Parameters.AddWithValue("@m", mobile);
                    var obj = cmd.ExecuteScalar();
                    if (obj != null) nid = Convert.ToInt32(obj);
                    cmd.Parameters.Clear();

                    if (nid == 0)
                    {
                        // 新增 NewFriend
                        cmd.CommandText = @"
INSERT INTO dbo.NewFriend(FullName,FullNameNorm,MobilePhone,City,District,Address,CreatedByHID)
VALUES(@name,@namen,@m,@city,@dist,@addr,@hid);
SELECT CAST(SCOPE_IDENTITY() AS int);";
                        cmd.Parameters.AddWithValue("@name", dto.FullName ?? "");
                        cmd.Parameters.AddWithValue("@namen", name);
                        cmd.Parameters.AddWithValue("@m", mobile);
                        cmd.Parameters.AddWithValue("@city", (object)(dto.City ?? (object)DBNull.Value));
                        cmd.Parameters.AddWithValue("@dist", (object)(dto.District ?? (object)DBNull.Value));
                        cmd.Parameters.AddWithValue("@addr", (object)(dto.Address ?? (object)DBNull.Value));
                        cmd.Parameters.AddWithValue("@hid", createdByHID);
                        nid = (int)cmd.ExecuteScalar();
                        cmd.Parameters.Clear();

                        // 新增 Assignment（來源）
                        cmd.CommandText = @"INSERT INTO dbo.NewFriendAssignment(NewFriendId,ContactHID,Channel,Note) 
                                        VALUES(@nid,@hid,@ch,NULL)";
                        cmd.Parameters.AddWithValue("@nid", nid);
                        cmd.Parameters.AddWithValue("@hid", createdByHID);
                        cmd.Parameters.AddWithValue("@ch", channel ?? "道場");
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // 加：若沒有同一同修/來源的關係，就補一筆
                        cmd.CommandText = @"
IF NOT EXISTS(
  SELECT 1 FROM dbo.NewFriendAssignment 
  WHERE NewFriendId=@nid AND ContactHID=@hid AND Channel=@ch
)
INSERT INTO dbo.NewFriendAssignment(NewFriendId,ContactHID,Channel,Note)
VALUES(@nid,@hid,@ch,NULL)";
                        cmd.Parameters.AddWithValue("@nid", nid);
                        cmd.Parameters.AddWithValue("@hid", createdByHID);
                        cmd.Parameters.AddWithValue("@ch", channel ?? "道場");
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return new ApiResult { Ok = true, NewFriendId = nid };
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    return new ApiResult { Ok = false, Msg = ex.Message };
                }
            }
        }
    }


    [WebMethod, ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static ApiResult UpdateNewFriend(int id, NFDto dto, int updatedByHID)
    {
        if (dto == null)
            return new ApiResult { Ok = false, Msg = "無法解析參數：請重新整理頁面後再試（AddInteraction）" };

        var connStr = ConfigurationManager.ConnectionStrings["HochiReports"].ConnectionString;
        var name = NormalizeName(dto.FullName);
        var mobile = NormalizeMobile(dto.MobilePhone);
        if (id <= 0) return new ApiResult { Ok = false, Msg = "ID無效" };

        using (var cn = new SqlConnection(connStr))
        using (var cmd = cn.CreateCommand())
        {
            cn.Open();
            cmd.CommandText = @"
UPDATE dbo.NewFriend
SET FullName     = CASE WHEN @name  <> '' AND (FullName IS NULL OR LTRIM(RTRIM(FullName)) = '') THEN @name  ELSE FullName END,
    FullNameNorm = CASE WHEN @namen <> '' AND (FullNameNorm IS NULL OR LTRIM(RTRIM(FullNameNorm))='') THEN @namen ELSE FullNameNorm END,
    MobilePhone  = CASE WHEN @m     <> '' AND (MobilePhone IS NULL OR LTRIM(RTRIM(MobilePhone))='') THEN @m     ELSE MobilePhone END,
    City         = CASE WHEN @city  <> '' AND (City IS NULL OR LTRIM(RTRIM(City))='') THEN @city  ELSE City END,
    District     = CASE WHEN @dist  <> '' AND (District IS NULL OR LTRIM(RTRIM(District))='') THEN @dist  ELSE District END,
    Address      = CASE WHEN @addr  <> '' AND (Address IS NULL OR LTRIM(RTRIM(Address))='') THEN @addr  ELSE Address END,
    UpdatedAt    = GETDATE(), UpdatedByHID=@hid
WHERE NewFriendId=@id AND IsMergedIntoId IS NULL;
";
            cmd.Parameters.AddWithValue("@name", dto.FullName ?? "");
            cmd.Parameters.AddWithValue("@namen", name);
            cmd.Parameters.AddWithValue("@m", (object)(mobile ?? (object)DBNull.Value));
            cmd.Parameters.AddWithValue("@city", (object)(dto.City ?? (object)DBNull.Value));
            cmd.Parameters.AddWithValue("@dist", (object)(dto.District ?? (object)DBNull.Value));
            cmd.Parameters.AddWithValue("@addr", (object)(dto.Address ?? (object)DBNull.Value));
            cmd.Parameters.AddWithValue("@hid", updatedByHID);
            cmd.Parameters.AddWithValue("@id", id);
            var n = cmd.ExecuteNonQuery();
            return new ApiResult { Ok = n > 0, Msg = n > 0 ? null : "未更新任何資料" };
        }
    }

    [WebMethod, ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static ApiResult AddInteraction(InteractionDto dto)
    {
        if (dto == null)
            return new ApiResult { Ok = false, Msg = "無法解析參數：請重新整理頁面後再試（AddInteraction）" };

        var connStr = ConfigurationManager.ConnectionStrings["HochiReports"].ConnectionString;
        if (dto.NewFriendId <= 0)
            return new ApiResult { Ok = false, Msg = "缺少 NewFriendId：請先建立或選擇新朋友" };
        if (dto.ContactHID <= 0)
            return new ApiResult { Ok = false, Msg = "缺少 ContactHID（myHid）。請用 ?myHid= 開啟或以專屬 QR 進入此頁" };


        using (var cn = new SqlConnection(connStr))
        {
            cn.Open(); // 先開連線
            using (var tx = cn.BeginTransaction("itx")) // 再開始交易
            using (var cmd = cn.CreateCommand())
            {
                cmd.Transaction = tx;
                try
                {
                    // 互動 INSERT 前加這段
                    // 1) 正規化
                    var normName = NormalizeName(dto.FullName ?? "");
                    var normMobile = NormalizeMobile(dto.MobilePhone);

                    // 2) 覆蓋更新主檔（有送就覆蓋）
                    cmd.CommandText = @"
UPDATE dbo.NewFriend
SET FullName = CASE WHEN @name <> '' THEN @name ELSE FullName END,
    FullNameNorm = CASE WHEN @namen <> '' THEN @namen ELSE FullNameNorm END,
    MobilePhone = CASE WHEN @m <> '' THEN @m ELSE MobilePhone END,
    City = CASE WHEN @city <> '' THEN @city ELSE City END,
    District = CASE WHEN @dist <> '' THEN @dist ELSE District END,
    Address = CASE WHEN @addr <> '' THEN @addr ELSE Address END,
    UpdatedAt = GETDATE(), UpdatedByHID = @hid
WHERE NewFriendId = @nid AND IsMergedIntoId IS NULL";
                    cmd.Parameters.AddWithValue("@name", (object)(dto.FullName ?? ""));
                    cmd.Parameters.AddWithValue("@namen", (object)(normName ?? ""));
                    cmd.Parameters.AddWithValue("@m", (object)(normMobile ?? ""));
                    cmd.Parameters.AddWithValue("@city", (object)(dto.City ?? ""));
                    cmd.Parameters.AddWithValue("@dist", (object)(dto.District ?? ""));
                    cmd.Parameters.AddWithValue("@addr", (object)(dto.Address ?? ""));
                    cmd.Parameters.AddWithValue("@hid", dto.ContactHID);
                    cmd.Parameters.AddWithValue("@nid", dto.NewFriendId);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = @"
INSERT INTO dbo.Interactions(NewFriendId,ContactHID,Method,IntentLevel,NextAction,NextActionDate,Memo)
VALUES(@nid,@hid,@m,@i,@na,@nad,@memo)";
                    cmd.Parameters.AddWithValue("@nid", dto.NewFriendId);
                    cmd.Parameters.AddWithValue("@hid", dto.ContactHID);
                    cmd.Parameters.AddWithValue("@m", dto.Method ?? "電話");
                    cmd.Parameters.AddWithValue("@i", (object)(dto.IntentLevel ?? (object)DBNull.Value));
                    cmd.Parameters.AddWithValue("@na", (object)(dto.NextAction ?? (object)DBNull.Value));

                    DateTime d;
                    if (!DateTime.TryParse(dto.NextActionDate, out d))
                    {
                        if (!DateTime.TryParseExact(dto.NextActionDate,
                            new[] { "yyyy-MM-dd", "yyyy/MM/dd" },
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out d))
                        {
                            d = DateTime.MinValue;
                        }
                    }
                    cmd.Parameters.AddWithValue("@nad", d == DateTime.MinValue ? (object)DBNull.Value : d);


                    cmd.Parameters.AddWithValue("@memo", (object)(dto.Memo ?? (object)DBNull.Value));
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    // 若有意向，順便更新階段（可選）
                    if (!string.IsNullOrEmpty(dto.IntentLevel))
                    {
                        cmd.CommandText = @"UPDATE dbo.NewFriend 
                                        SET Stage=@s, UpdatedAt=GETDATE(), UpdatedByHID=@hid 
                                        WHERE NewFriendId=@nid";
                        cmd.Parameters.AddWithValue("@s", dto.IntentLevel);
                        cmd.Parameters.AddWithValue("@hid", dto.ContactHID);
                        cmd.Parameters.AddWithValue("@nid", dto.NewFriendId);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return new ApiResult { Ok = true };
                }
                catch (Exception ex)
                {
                    try { tx.Rollback(); } catch { }
                    return new ApiResult { Ok = false, Msg = ex.Message };
                }
            }
        }
    }

    public class PersonCandidate
    {
        public string SourceType; // "NF" or "EDU"
        public int? NewFriendId;
        public int? EduHID;
        public string FullName;
        public string MobilePhone;
        public string City;
        public string District;
        public string Address;
    }

    public class SearchPayload
    {
        public bool Found;          // 是否至少一筆
        public bool Multiple;       // 是否多筆
        public object Data;         // 若唯一，沿用你舊結構（表單可直接塞）
        public List<PersonCandidate> Candidates; // 多筆時用這個清單
    }

}
