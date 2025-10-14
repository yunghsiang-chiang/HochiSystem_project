using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;

public partial class HSCGRecordData : System.Web.UI.Page
{
    // SQL Server 連線（依你專案的 Web.config 調整名稱）
    private readonly SqlConnection ConStr = new SqlConnection(
        ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    // 檔案虛擬根目錄（依你現況）
    private const string UploadVirtualRoot = "uploads/HSCGRecord/";

    protected void Page_Load(object sender, EventArgs e)
    {
        string memberId = Request["memberid"]; // 你的前端通常會帶 current user 的 HID
        string date = Request["date"];         // 期望格式：yyyy/MM/dd（與頁面 header 顯示一致）

        if (string.IsNullOrEmpty(memberId) || string.IsNullOrEmpty(date))
        {
            WriteAndComplete("<div class='text-danger'>缺少 memberid 或 date 參數</div>");
            return;
        }

        // 1) 先抓該會員在指定日期的主文
        DataTable mainDt = FetchMainRecords(memberId, date);
        if (mainDt.Rows.Count == 0)
        {
            WriteAndComplete("<div class='text-muted'>當日查無任何紀錄。</div>");
            return;
        }

        // 2) 依主文建立區塊容器與顯示順序
        List<string> order = new List<string>();                               // 主文 HID 順序
        Dictionary<string, StringBuilder> blocks = new Dictionary<string, StringBuilder>(); // HID -> HTML
        BuildMainHtml(mainDt, order, blocks);

        // 3) 一次抓所有屬於這些主文的回應，並掛回各自主文下
        DataTable replyDt = FetchReplies(order);
        if (replyDt.Rows.Count > 0)
        {
            AppendRepliesHtml(replyDt, blocks);
        }

        // 4) 組裝輸出
        string html = AssembleFinal(order, blocks);


//WriteAndComplete(html);

        ////重新讀取資料
        Response.Clear();
        Response.ContentType = "text/plain";
        Response.Write(html);
        Response.End();


    }

    /// <summary>
    /// 抓當天該會員發的主文（HSCGRMsg）。
    /// 以 CONVERT(VARCHAR(10), a.HCreateDT, 111) = yyyy/mm/dd 來比對日期。
    /// </summary>
    private DataTable FetchMainRecords(string memberId, string dateYmdSlash)
    {
        const string sql = @"
SELECT a.HID, a.HTopicName, a.HCourseID, c.HCourseName, a.HSCClassID, b.HSCClassName, d.HSCRTName,
       a.HSCJiugonggeTypeID, a.HGProgress, a.HOGProgress, a.HContent,
       a.HFile1, a.HFile2, a.HFile3, a.HVideoLink, a.HHashTag, a.HOpenObject, a.HCreateDT
FROM HSCGRMsg AS a
LEFT JOIN HSCClass AS b ON a.HSCClassID = b.HID
LEFT JOIN HCourse_Merge AS c ON a.HCourseID = c.HID
LEFT JOIN HSCRecordType AS d ON a.HSCRecordTypeID = d.HID
WHERE a.HMemberID = @mid
  AND CONVERT(VARCHAR(10), a.HCreateDT, 111) = @d
  AND a.HStatus = 1
ORDER BY a.HCreateDT DESC;";

        DataTable dt = new DataTable();
        using (SqlDataAdapter da = new SqlDataAdapter(sql, ConStr))
        {
            da.SelectCommand.Parameters.Add("@mid", SqlDbType.VarChar).Value = memberId;
            da.SelectCommand.Parameters.Add("@d", SqlDbType.VarChar, 10).Value = dateYmdSlash;
            da.Fill(dt);
        }
        return dt;
    }

    /// <summary>
    /// 建立主文 HTML（先保留收尾，等回應掛好再補閉合標籤）。
    /// </summary>
    private void BuildMainHtml(DataTable mainDt, List<string> order, Dictionary<string, StringBuilder> blocks)
    {
        for (int i = 0; i < mainDt.Rows.Count; i++)
        {
            DataRow row = mainDt.Rows[i];
            string hid = Convert.ToString(row["HID"]);
            order.Add(hid);

            StringBuilder sb = new StringBuilder();
            blocks.Add(hid, sb);

            sb.Append("<div class='media' id='").Append(hid).Append("'>");
            sb.Append("<div class='media-body'><p>");

            // 標題（課程分類 + 主題）
            sb.Append("<h4 class='text-blue font-weight-bold media-heading'>#")
              .Append(HttpUtility.HtmlEncode(Convert.ToString(row["HSCClassName"])))
              .Append("〈<strong>")
              .Append(HttpUtility.HtmlEncode(Convert.ToString(row["HTopicName"])))
              .Append("</strong>〉</h4>");

            // 課程名稱（有則顯示）
            if (!IsNullOrEmpty(row, "HCourseName"))
            {
                sb.Append("<p class='font-weight-bold mb-1'>◎<b>")
                  .Append(HttpUtility.HtmlEncode(Convert.ToString(row["HCourseName"])))
                  .Append("</b></p>");
            }

            // 九宮格名稱
            string gridName = MapJiugongge(Convert.ToString(row["HSCJiugonggeTypeID"]));

            // 類別/九宮格
            sb.Append("<p class='mb-0'>專欄分類：<span class='font-weight-bold'>")
              .Append(HttpUtility.HtmlEncode(Convert.ToString(row["HSCClassName"])))
              .Append("</span> / 紀錄類型：<span class='font-weight-bold'>")
              .Append(HttpUtility.HtmlEncode(Convert.ToString(row["HSCRTName"])))
              .Append("</span> / 九宮格：<span class='font-weight-bold'>")
              .Append(HttpUtility.HtmlEncode(gridName))
              .Append("</span></p>");

            // HashTag
            if (!IsNullOrEmpty(row, "HHashTag"))
            {
                sb.Append("<span class='fa fa-tag mr-1'></span>")
                  .Append(HttpUtility.HtmlEncode(Convert.ToString(row["HHashTag"])))
                  .Append("<br/>");
            }

            // 內容（保留換行）
            if (!IsNullOrEmpty(row, "HContent"))
            {
                string content = Convert.ToString(row["HContent"]).Replace(Environment.NewLine, "<br />");
                sb.Append(content).Append("<br/>");
            }

            // 附檔（1~3）
            string ymd = SafeYmdFolder(row["HCreateDT"]);
            AppendFileHtml(sb, ymd, Convert.ToString(row["HFile1"]));
            AppendFileHtml(sb, ymd, Convert.ToString(row["HFile2"]));
            AppendFileHtml(sb, ymd, Convert.ToString(row["HFile3"]));

            // 影片連結
            if (!IsNullOrEmpty(row, "HVideoLink"))
            {
                sb.Append("<div class='embed-responsive embed-responsive-16by9' style='width:50%;'><iframe src='")
                  .Append(HttpUtility.HtmlAttributeEncode(Convert.ToString(row["HVideoLink"])))
                  .Append("'></iframe></div><br/>");
            }

            // 回應按鈕（沿用你原 JS）
            sb.Append(" <a href=\"javascript:func_EDUReply('growth','")
              .Append(hid)
              .Append("')\" class='btn btn-outline btn-primary btn-xs mt-2 mb-2' data-toggle='tooltip' data-placement='top' title='回應本文'><i class='fa fa-reply'></i> 回應</a>");

            // 回應插入點
            sb.Append("<!-- replies appended below -->");
        }
    }

    /// <summary>
    /// 一次撈出所有主文的回應，依主文 HID 分配。
    /// </summary>
    private DataTable FetchReplies(List<string> mainIdsInOrder)
    {
        DataTable dt = new DataTable();
        if (mainIdsInOrder == null || mainIdsInOrder.Count == 0) return dt;

        string sqlBase = @"
SELECT r.HID AS ReplyID,
       r.HSCGRMsgID AS MainID,
       r.HGRMRContent AS Content,
       CONVERT(VARCHAR(16), r.HCreateDT, 120) AS ResponseTime,
       m.HUserName AS ReplierName
FROM HSCGRMsgResponse AS r
LEFT JOIN HMember AS m ON r.HMemberID = m.HID
WHERE r.HStatus = 1
  AND r.HSCGRMsgID IN ({0})
ORDER BY r.HCreateDT ASC;";

        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = ConStr;
            StringBuilder inList = new StringBuilder();
            for (int i = 0; i < mainIdsInOrder.Count; i++)
            {
                if (i > 0) inList.Append(",");
                string p = "@id" + i;
                inList.Append(p);
                cmd.Parameters.Add(p, SqlDbType.Int).Value = Convert.ToInt32(mainIdsInOrder[i]);
            }
            cmd.CommandText = string.Format(sqlBase, inList.ToString());

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
        }
        return dt;
    }

    /// <summary>
    /// 把回應 HTML 附加到各自主文區塊
    /// </summary>
    private void AppendRepliesHtml(DataTable replyDt, Dictionary<string, StringBuilder> blocks)
    {
        for (int i = 0; i < replyDt.Rows.Count; i++)
        {
            DataRow r = replyDt.Rows[i];
            string mainId = Convert.ToString(r["MainID"]);
            StringBuilder sb;
            if (!blocks.TryGetValue(mainId, out sb)) continue;

            sb.Append("<div class='media response-media' id='")
              .Append(HttpUtility.HtmlEncode(Convert.ToString(r["ReplyID"])))
              .Append("'>");

            sb.Append("<div class='media-body response-body'>");
            sb.Append("<div class='bg-muted p-xs pl-0 pb-0 b-r-sm'><h5 class='response-heading'><i class='fa fa-reply mr-2'></i>")
              .Append(HttpUtility.HtmlEncode(Convert.ToString(r["ReplierName"])))
              .Append(" 於 ")
              .Append(HttpUtility.HtmlEncode(Convert.ToString(r["ResponseTime"])))
              .Append(" 回應</h5></div>");

            sb.Append("<p class='mt-2 mb-1 pl-3'>")
              .Append(Convert.ToString(r["Content"]))
              .Append("</p>");

            sb.Append("</div></div>");
        }
    }

    /// <summary>
    /// 依主文順序收尾（補上分隔線與關閉標籤）。
    /// </summary>
    private string AssembleFinal(List<string> order, Dictionary<string, StringBuilder> blocks)
    {
        StringBuilder final = new StringBuilder();
        for (int i = 0; i < order.Count; i++)
        {
            StringBuilder sb = blocks[order[i]];
            sb.Append("<hr/></p></div></div>");
            final.Append(sb.ToString());
        }
        return final.ToString();
    }

    // ===== 小工具 =====

    private static string MapJiugongge(string id)
    {
        switch (id)
        {
            case "1": return "身";
            case "2": return "心";
            case "3": return "靈";
            case "4": return "人";
            case "5": return "事";
            case "6": return "境";
            case "7": return "金錢";
            case "8": return "關係";
            case "9": return "時間";
            default: return "";
        }
    }

    private static bool IsNullOrEmpty(DataRow row, string col)
    {
        return row == null || row.IsNull(col) || string.IsNullOrEmpty(Convert.ToString(row[col]));
    }

    private static string SafeYmdFolder(object createDtObj)
    {
        // a.HCreateDT 目前為 datetime；若未來改成字串 yyyyMMddHHmmss，可改用 ParseExact
        try
        {
            DateTime dt = Convert.ToDateTime(createDtObj);
            return dt.ToString("yyyyMMdd");
        }
        catch
        {
            return string.Empty;
        }
    }

    private static void AppendFileHtml(StringBuilder sb, string ymd, string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return;

        string lower = fileName.Trim().ToLowerInvariant();
        string prefix = UploadVirtualRoot + ymd + "/";

        bool isImg = lower.EndsWith(".jpg") || lower.EndsWith(".jpeg") ||
                     lower.EndsWith(".png") || lower.EndsWith(".gif") ||
                     lower.EndsWith(".heic") || lower.EndsWith(".heif");

        if (isImg)
        {
            sb.Append("<img src='").Append(prefix).Append(HttpUtility.UrlPathEncode(fileName))
              .Append("' alt='' class='screcord_img mb-2'><br/>");
            return;
        }

        if (lower.EndsWith(".mp3"))
        {
            sb.Append("語音：<br/><audio src='").Append(prefix).Append(HttpUtility.UrlPathEncode(fileName))
              .Append("' preload='auto' controls loop></audio><br/>");
            return;
        }

        sb.Append("檔案：<a href='").Append(prefix).Append(HttpUtility.UrlPathEncode(fileName))
          .Append("' target='_blank'>下載/閱覽</a><br/>");
    }

    /// <summary>
    /// 統一輸出並避免 ThreadAbortException。
    /// </summary>
    private void WriteAndComplete(string html)
    {
        Response.Clear();
        Response.ContentType = "text/html; charset=utf-8";
        Response.Write(html);
        Response.Flush();
        HttpContext.Current.ApplicationInstance.CompleteRequest();
    }
}







