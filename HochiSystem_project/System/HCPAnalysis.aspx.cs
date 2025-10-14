using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Serialization;

public partial class System_HCPAnalysis : System.Web.UI.Page
{
    // web.config 需有 connectionStrings["HochiSystemConnection"]
    private readonly string _connStr = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindHMType();
            BindCourses();
            BindAll();
        }
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        BindAll();
    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        ddlCourse.SelectedIndex = 0;
        foreach (ListItem it in lbHMType.Items) it.Selected = false;
        BindAll();
    }

    private void BindHMType()
    {
        using (var cn = new SqlConnection(_connStr))
        using (var cmd = new SqlCommand(@"
            SELECT HID, HMType
            FROM [HochiSystem].[dbo].[HMType]
            WHERE HStatus = 1
            ORDER BY HID", cn))
        {
            cn.Open();
            using (var rd = cmd.ExecuteReader())
            {
                lbHMType.DataSource = rd;
                lbHMType.DataTextField = "HMType";
                lbHMType.DataValueField = "HID";
                lbHMType.DataBind();
            }
        }
    }

    private void BindCourses()
    {
        ddlCourse.Items.Clear();
        ddlCourse.Items.Add(new ListItem("（不指定課程）", ""));
        using (var cn = new SqlConnection(_connStr))
        using (var cmd = new SqlCommand(@"
            SELECT DISTINCT TOP 20 HCourseName, MAX(HModifyDT) AS HModifyDT
            FROM [HochiSystem].[dbo].[HCourse]
            WHERE HStatus = 1 AND CHARINDEX('_', HCourseName) = 0 AND NOT HOCPlace = 1
            GROUP BY HCourseName
            ORDER BY MAX(HModifyDT) DESC;", cn))
        {
            cn.Open();
            using (var rd = cmd.ExecuteReader())
            {
                ddlCourse.DataSource = rd;
                ddlCourse.DataTextField = "HCourseName";
                ddlCourse.DataValueField = "HCourseName";
                ddlCourse.DataBind();
            }
        }
    }

    // 放在 class System_HCPAnalysis 裡（欄位區）
    private HashSet<int> _courseNoteMembers = new HashSet<int>(); // 依目前所選課程，有「課程備註」的 memberId
    private HashSet<int> _personNoteMembers = new HashSet<int>(); // 有「常態備註」的 memberId

    private class MemberDisplay
    {
        public int HID { get; set; }
        public string HUserName { get; set; }
        public string HPeriod { get; set; }
        public int? HAreaID { get; set; }
        public bool IsBooked { get; set; }
        public string TeamRaw { get; set; }

        public string MemberTypeName { get; set; } // ← 新增：學員類型中文
    }

    private class ChildTeam
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<MemberDisplay> Members { get; set; }
        public int Total { get; set; }
        public int Booked { get; set; }
    }

    private class MotherTeamGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<MemberDisplay> MotherMembers { get; set; } // 直屬母光團
        public List<ChildTeam> Children { get; set; }
        public int Total { get; set; }
        public int Booked { get; set; }
    }

    private class LargeAreaGroup
    {
        public int Id { get; set; }           // HLAreaID
        public string Name { get; set; }      // HLArea 名稱
        public List<MotherTeamGroup> Mothers { get; set; }
        public int Total { get; set; }
        public int Booked { get; set; }
    }


    private void BindAll()
    {

        var members = GetMembers();
        var areas = GetAreas();
        var lareas = GetLargeAreas();
        // 讀取 HMType 對照（ID→名稱）
        var hmTypeMap = GetHMTypeMap();

        var selectedTypes = lbHMType.Items.Cast<ListItem>()
            .Where(x => x.Selected)
            .Select(x => SafeInt(x.Value))
            .ToHashSet();

        if (selectedTypes.Count > 0)
        {
            members = members.Where(m => m.HType.HasValue && selectedTypes.Contains(m.HType.Value)).ToList();
        }

        HashSet<int> bookedMemberIds = new HashSet<int>();
        string chosenCourse = ddlCourse.SelectedValue;
        if (!string.IsNullOrWhiteSpace(chosenCourse))
        {
            bookedMemberIds = GetBookedMemberIds(chosenCourse);
        }

        // 已有：string chosenCourse = ddlCourse.SelectedValue; ...

        if (!string.IsNullOrWhiteSpace(chosenCourse))
        {
            _courseNoteMembers = GetMembersWithCourseNotes(chosenCourse);
        }
        else
        {
            _courseNoteMembers = new HashSet<int>(); // 未選課程則不顯示課程備註圖示
        }
        _personNoteMembers = GetMembersWithPersonNotes();


        var queryArea =
            from m in members
            join a in areas on m.HAreaID equals (a == null ? (int?)null : a.HID) into areaJoin
            from a in areaJoin.DefaultIfEmpty()
            join la in lareas on (a != null ? a.HLAreaID : 0) equals la.HID into laJoin
            from la in laJoin.DefaultIfEmpty()
            select new
            {
                LAreaID = la != null ? la.HID : 0,
                LAreaName = la != null ? la.HLArea : "(未設定大區)",
                AreaID = a != null ? a.HID : 0,
                AreaName = a != null ? a.HArea : "(未設定區屬)",
                Member = m,
                IsBooked = bookedMemberIds.Contains(m.HID)
            };

        var lareaGroups = queryArea
            .GroupBy(x => new { x.LAreaID, x.LAreaName })
            .OrderBy(g => g.Key.LAreaName)
            .Select(g => new
            {
                LAreaID = g.Key.LAreaID,
                LAreaName = g.Key.LAreaName,
                Total = g.Count(),
                Booked = g.Count(x => x.IsBooked),
                Areas = g.GroupBy(x => new { x.AreaID, x.AreaName })
                         .OrderBy(ag => ag.Key.AreaName)
                         .Select(ag => new
                         {
                             AreaID = ag.Key.AreaID,
                             AreaName = ag.Key.AreaName,
                             Total = ag.Count(),
                             Booked = ag.Count(x => x.IsBooked),
                             Members = ag.Select(x => new
                             {
                                 x.Member.HID,
                                 x.Member.HUserName,
                                 x.Member.HPeriod,
                                 MemberTypeName = (x.Member.HType.HasValue && hmTypeMap.ContainsKey(x.Member.HType.Value))
                        ? hmTypeMap[x.Member.HType.Value] : "",
                                 IsBooked = x.IsBooked
                             })
                             .OrderBy(m => m.HUserName)
                             .ToList()
                         })
                         .ToList()
            })
            .ToList();

        var memberDisplays = members.Select(m => new MemberDisplay
        {
            HID = m.HID,
            HUserName = m.HUserName,
            HPeriod = m.HPeriod,
            HAreaID = m.HAreaID,
            IsBooked = bookedMemberIds.Contains(m.HID),
            TeamRaw = m.HTeamIDRaw,
            MemberTypeName = (m.HType.HasValue && hmTypeMap.ContainsKey(m.HType.Value)) ? hmTypeMap[m.HType.Value] : ""
        }).ToList();


        // 取光團資料
        var mTeams = GetHMTeams();
        var cTeams = GetHCTeams();
        var mMap = mTeams.ToDictionary(x => x.HID, x => x);
        var cMap = cTeams.ToDictionary(x => x.HID, x => x);
        var laMap = lareas.ToDictionary(x => x.HID, x => x.HLArea);

        // 先依大區建樹
        var laGroups = new Dictionary<int, LargeAreaGroup>(); // key = HLAreaID（0 = 未設定）
        Func<int, LargeAreaGroup> ensureLA = (lid) =>
        {
            LargeAreaGroup g;
            if (!laGroups.TryGetValue(lid, out g))
            {
                string name = "(未設定大區)";
                if (lid != 0 && laMap.ContainsKey(lid)) name = laMap[lid];
                g = new LargeAreaGroup { Id = lid, Name = name, Mothers = new List<MotherTeamGroup>() };
                laGroups[lid] = g;
            }
            return g;
        };

        // 在某大區底下確保一個母光團節點
        Func<LargeAreaGroup, int, MotherTeamGroup> ensureMother = (lag, mid) =>
        {
            for (int i = 0; i < lag.Mothers.Count; i++)
                if (lag.Mothers[i].Id == mid) return lag.Mothers[i];

            string name = "(未設定)";
            if (mid != 0 && mMap.ContainsKey(mid)) name = mMap[mid].HMTeam;

            var mg = new MotherTeamGroup
            {
                Id = mid,
                Name = name,
                MotherMembers = new List<MemberDisplay>(),
                Children = new List<ChildTeam>()
            };
            lag.Mothers.Add(mg);
            return mg;
        };

        // 在某母光團底下確保一個子光團節點
        Func<MotherTeamGroup, int, ChildTeam> ensureChild = (mg, cid) =>
        {
            for (int i = 0; i < mg.Children.Count; i++)
                if (mg.Children[i].Id == cid) return mg.Children[i];

            string name = "(未設定子光團)";
            if (cid != 0 && cMap.ContainsKey(cid)) name = cMap[cid].HCTeam;

            var ct = new ChildTeam { Id = cid, Name = name, Members = new List<MemberDisplay>() };
            mg.Children.Add(ct);
            return ct;
        };

        // 分配成員到「大區→母→子」
        foreach (var m in memberDisplays)
        {
            var tr = ParseTeamRef(m.TeamRaw);
            if (tr.Kind == 1)
            {
                // 直屬母光團：大區從「母光團」取得
                int mid = tr.Id;
                int lid = 0;
                if (mMap.ContainsKey(mid)) lid = mMap[mid].HLAreaID;

                var lag = ensureLA(lid);
                var mg = ensureMother(lag, mid);
                mg.MotherMembers.Add(m);
            }
            else if (tr.Kind == 2)
            {
                // 子光團：母光團與大區由子光團反查
                int cid = tr.Id;
                int mid = 0, lid = 0;
                if (cMap.ContainsKey(cid))
                {
                    mid = cMap[cid].HMTeamID;
                    lid = cMap[cid].HLAreaID;
                }
                var lag = ensureLA(lid);
                var mg = ensureMother(lag, mid);
                var cg = ensureChild(mg, cid);
                cg.Members.Add(m);
            }
            else
            {
                // 無光團：歸到「大區=0、母光團=0」之下
                var lag = ensureLA(0);
                var mg = ensureMother(lag, 0);
                mg.MotherMembers.Add(m);
            }
        }

        // 統計每層
        foreach (var lag in laGroups.Values)
        {
            lag.Total = 0; lag.Booked = 0;
            foreach (var mg in lag.Mothers)
            {
                foreach (var cg in mg.Children)
                {
                    cg.Total = cg.Members.Count;
                    cg.Booked = cg.Members.Count(x => x.IsBooked);
                }
                var motherOnlyTotal = mg.MotherMembers.Count;
                var motherOnlyBooked = mg.MotherMembers.Count(x => x.IsBooked);
                mg.Total = motherOnlyTotal + mg.Children.Sum(x => x.Total);
                mg.Booked = motherOnlyBooked + mg.Children.Sum(x => x.Booked);

                lag.Total += mg.Total;
                lag.Booked += mg.Booked;
            }
        }

        // 排序：大區（未設定置底）→ 母光團名 → 子光團名
        var orderedLAs = laGroups.Values
            .OrderBy(g => g.Id == 0 ? 1 : 0)
            .ThenBy(g => g.Name)
            .Select(g =>
            {
                g.Mothers = g.Mothers
                    .OrderBy(mg => mg.Id == 0 ? 1 : 0)
                    .ThenBy(mg => mg.Name)
                    .ToList();
                foreach (var mg in g.Mothers)
                {
                    mg.Children = mg.Children
                        .OrderBy(cg => cg.Id == 0 ? 1 : 0)
                        .ThenBy(cg => cg.Name)
                        .ToList();
                }
                return g;
            })
            .ToList();

        // 輸出新的四層 HTML
        litTeamAccordion.Text = BuildTeamAccordionHtml_LAreaMotherChild(orderedLAs);




        litAreaAccordion.Text = BuildAreaAccordionHtml(lareaGroups);

    }

    #region Data Fetch

    private string BuildTeamAccordionHtml_LAreaMotherChild(IEnumerable<LargeAreaGroup> las)
    {
        var sb = new StringBuilder();
        sb.Append("<div id='acc-team' class='accordion'>");
        int laIdx = 0;

        foreach (var la in las)
        {
            string laId = "la-" + (laIdx++);
            sb.Append("<div class='card'>");
            sb.Append("<div class='card-header' id='head-" + laId + "'>");
            sb.Append("<h5 class='mb-0 d-flex align-items-center justify-content-between'>");
            sb.Append("<button type='button' class='btn btn-link' data-toggle='collapse' data-target='#col-" + laId + "' aria-expanded='false' aria-controls='col-" + laId + "'>");
            sb.Append("大區：" + HtmlEncode(la.Name));
            sb.Append("</button>");
            sb.Append("<span class='badge badge-primary stat-badge'>已報名 " + la.Booked + " / 共 " + la.Total + "</span>");
            sb.Append("</h5></div>");

            sb.Append("<div id='col-" + laId + "' class='collapse' data-parent='#acc-team'>");
            sb.Append("<div class='card-body'>");

            // 第二層：母光團
            sb.Append("<div id='acc-m-" + laId + "' class='accordion'>");
            int mIdx = 0;
            foreach (var mg in la.Mothers)
            {
                string mId = laId + "-m-" + (mIdx++);
                sb.Append("<div class='card'>");
                sb.Append("<div class='card-header' id='head-" + mId + "'>");
                sb.Append("<h6 class='mb-0 d-flex align-items-center justify-content-between'>");
                sb.Append("<button type='button' class='btn btn-link' data-toggle='collapse' data-target='#col-" + mId + "' aria-expanded='false' aria-controls='col-" + mId + "'>");
                sb.Append("母光團：" + HtmlEncode(mg.Name));
                sb.Append("</button>");
                sb.Append("<span class='badge badge-info stat-badge'>已報名 " + mg.Booked + " / 共 " + mg.Total + "</span>");
                sb.Append("</h6></div>");

                sb.Append("<div id='col-" + mId + "' class='collapse' data-parent='#acc-m-" + laId + "'>");
                sb.Append("<div class='card-body'>");

                // 若母光團直屬有人，先列出
                if (mg.MotherMembers != null && mg.MotherMembers.Count > 0)
                {
                    sb.Append("<div class='mb-2 small text-muted'>母光團直屬成員</div>");
                    foreach (var mem in mg.MotherMembers.OrderBy(x => x.HUserName))
                    {
                        //sb.Append("<div class='member-row'>");
                        //sb.Append("<div>");
                        //sb.Append("<strong>" + HtmlEncode(mem.HUserName) + "</strong>");
                        //sb.Append("<span class='small-muted ml-2'>期別：" + HtmlEncode(mem.HPeriod) + "</span>");
                        //sb.Append("<span class='small-muted ml-2'>學員類型：" + HtmlEncode(mem.MemberTypeName) + "</span>");
                        //sb.Append("</div>");
                        //sb.Append(mem.IsBooked ? "<span class='badge badge-success'>已報名</span>" : "<span class='badge badge-light'>未報名</span>");
                        //sb.Append("</div>");
                        sb.Append("<div class='member-row' data-member-id='" + mem.HID + "' data-member-name='" + HtmlEncode(mem.HUserName) + "' data-member-period='" + HtmlEncode(mem.HPeriod) + "'>");
                        sb.Append("<div>");
                        sb.Append("<strong><a href='javascript:void(0);' class='js-open-member'>" + HtmlEncode(mem.HUserName) + "</a></strong>");
                        sb.Append("<span class='small-muted ml-2'>期別：" + HtmlEncode(mem.HPeriod) + "</span>");
                        sb.Append("<span class='small-muted ml-2'>學員類型：" + HtmlEncode(mem.MemberTypeName) + "</span>");
                        sb.Append("</div>");
                        sb.Append("<div class='d-flex align-items-center'>");
                        var hasC = (_courseNoteMembers != null && _courseNoteMembers.Contains(mem.HID)) ? " on" : "";
                        var hasP = (_personNoteMembers != null && _personNoteMembers.Contains(mem.HID)) ? " on" : "";

                        sb.Append("<div class='note-icons'>");
                        sb.Append("<span class='note-icon ico-course" + hasC + "' title='有課程備註'>📝</span>");
                        sb.Append("<span class='note-icon ico-person" + hasP + "' title='有常態備註'>📌</span>");
                        sb.Append("</div>");

                        sb.Append(mem.IsBooked ? "<span class='badge badge-success ml-2'>已報名</span>" : "<span class='badge badge-light ml-2'>未報名</span>");
                        sb.Append("</div>");
                        sb.Append("</div>");

                    }
                    sb.Append("<hr />");
                }

                // 第三層：子光團
                sb.Append("<div id='acc-c-" + mId + "' class='accordion'>");
                int cIdx = 0;
                foreach (var cg in mg.Children)
                {
                    string cId = mId + "-c-" + (cIdx++);
                    sb.Append("<div class='card'>");
                    sb.Append("<div class='card-header' id='head-" + cId + "'>");
                    sb.Append("<h6 class='mb-0 d-flex align-items-center justify-content-between'>");
                    sb.Append("<button type='button' class='btn btn-link' data-toggle='collapse' data-target='#col-" + cId + "' aria-expanded='false' aria-controls='col-" + cId + "'>");
                    sb.Append("子光團：" + HtmlEncode(cg.Name));
                    sb.Append("</button>");
                    sb.Append("<span class='badge badge-secondary stat-badge'>已報名 " + cg.Booked + " / 共 " + cg.Total + "</span>");
                    sb.Append("</h6></div>");

                    sb.Append("<div id='col-" + cId + "' class='collapse' data-parent='#acc-c-" + mId + "'>");
                    sb.Append("<div class='card-body scroll-y'>");

                    // 第四層：成員
                    foreach (var mem in cg.Members.OrderBy(x => x.HUserName))
                    {
                        //sb.Append("<div class='member-row'>");
                        //sb.Append("<div>");
                        //sb.Append("<strong>" + HtmlEncode(mem.HUserName) + "</strong>");
                        //sb.Append("<span class='small-muted ml-2'>期別：" + HtmlEncode(mem.HPeriod) + "</span>");
                        //sb.Append("<span class='small-muted ml-2'>學員類型：" + HtmlEncode(mem.MemberTypeName) + "</span>");
                        //sb.Append("</div>");
                        //sb.Append(mem.IsBooked ? "<span class='badge badge-success'>已報名</span>" : "<span class='badge badge-light'>未報名</span>");
                        //sb.Append("</div>");
                        sb.Append("<div class='member-row' data-member-id='" + mem.HID + "' data-member-name='" + HtmlEncode(mem.HUserName) + "' data-member-period='" + HtmlEncode(mem.HPeriod) + "'>");
                        sb.Append("<div>");
                        sb.Append("<strong><a href='javascript:void(0);' class='js-open-member'>" + HtmlEncode(mem.HUserName) + "</a></strong>");
                        sb.Append("<span class='small-muted ml-2'>期別：" + HtmlEncode(mem.HPeriod) + "</span>");
                        sb.Append("<span class='small-muted ml-2'>學員類型：" + HtmlEncode(mem.MemberTypeName) + "</span>");
                        sb.Append("</div>");
                        sb.Append("<div class='d-flex align-items-center'>");
                        var hasC = (_courseNoteMembers != null && _courseNoteMembers.Contains(mem.HID)) ? " on" : "";
                        var hasP = (_personNoteMembers != null && _personNoteMembers.Contains(mem.HID)) ? " on" : "";

                        sb.Append("<div class='note-icons'>");
                        sb.Append("<span class='note-icon ico-course" + hasC + "' title='有課程備註'>📝</span>");
                        sb.Append("<span class='note-icon ico-person" + hasP + "' title='有常態備註'>📌</span>");
                        sb.Append("</div>");

                        sb.Append(mem.IsBooked ? "<span class='badge badge-success ml-2'>已報名</span>" : "<span class='badge badge-light ml-2'>未報名</span>");
                        sb.Append("</div>");
                        sb.Append("</div>");

                    }

                    sb.Append("</div></div></div>");
                }
                sb.Append("</div>"); // acc-c-*

                sb.Append("</div></div></div>");
            }
            sb.Append("</div>"); // acc-m-*

            sb.Append("</div></div></div>");
        }

        sb.Append("</div>");
        return sb.ToString();
    }


    private List<MemberRow> GetMembers()
    {
        var list = new List<MemberRow>();
        using (var cn = new SqlConnection(_connStr))
        using (var cmd = new SqlCommand(@"
        SELECT HID, HUserName, HPeriod, HAreaID, HTeamID, HType
        FROM [HochiSystem].[dbo].[HMember]
        WHERE HStatus = 1;", cn))
        {
            cn.Open();
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    list.Add(new MemberRow
                    {
                        HID = ReadInt(rd, 0),
                        HUserName = ReadString(rd, 1),
                        HPeriod = ReadString(rd, 2),
                        HAreaID = ReadNullableInt(rd, 3),
                        HTeamIDRaw = ReadString(rd, 4),   // ← 用字串讀取
                        HType = ReadNullableInt(rd, 5)
                    });
                }
            }
        }
        return list;
    }



    private List<AreaRow> GetAreas()
    {
        var list = new List<AreaRow>();
        using (var cn = new SqlConnection(_connStr))
        using (var cmd = new SqlCommand(@"
        SELECT HID, HLAreaID, HArea
        FROM [HochiSystem].[dbo].[HArea]
        WHERE HStatus = 1;", cn))
        {
            cn.Open();
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    list.Add(new AreaRow
                    {
                        HID = ReadInt(rd, 0),
                        HLAreaID = ReadInt(rd, 1),
                        HArea = ReadString(rd, 2)
                    });
                }
            }
        }
        return list;
    }


    private List<LAreaRow> GetLargeAreas()
    {
        var list = new List<LAreaRow>();
        using (var cn = new SqlConnection(_connStr))
        using (var cmd = new SqlCommand(@"
        SELECT HID, HLArea
        FROM [HochiSystem].[dbo].[HLArea]
        WHERE HStatus = 1;", cn))
        {
            cn.Open();
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    list.Add(new LAreaRow
                    {
                        HID = ReadInt(rd, 0),
                        HLArea = ReadString(rd, 1)
                    });
                }
            }
        }
        return list;
    }


    private HashSet<int> GetBookedMemberIds(string courseName)
    {
        var set = new HashSet<int>();
        using (var cn = new SqlConnection(_connStr))
        using (var cmd = new SqlCommand(@"
            SELECT HMemberID
            FROM [HochiSystem].[dbo].[HCourseBooking]
            WHERE HStatus = 1 AND HCourseName = @name;", cn))
        {
            cmd.Parameters.Add("@name", SqlDbType.NVarChar, 200).Value = courseName;
            cn.Open();
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    if (!rd.IsDBNull(0)) set.Add(rd.GetInt32(0));
                }
            }
        }
        return set;
    }

    #endregion

    private string BuildTeamAccordionHtml_MotherChild(IEnumerable<MotherTeamGroup> mothers)
    {
        var sb = new StringBuilder();
        sb.Append("<div id='acc-team' class='accordion'>");
        int mIdx = 0;

        foreach (var mg in mothers)
        {
            string mId = "mteam-" + (mIdx++);
            sb.Append("<div class='card'>");
            sb.Append("<div class='card-header' id='head-" + mId + "'>");
            sb.Append("<h5 class='mb-0 d-flex align-items-center justify-content-between'>");
            sb.Append("<button type='button' class='btn btn-link' data-toggle='collapse' data-target='#col-" + mId + "' aria-expanded='false' aria-controls='col-" + mId + "'>");
            sb.Append("母光團：" + HtmlEncode(mg.Name));
            sb.Append("</button>");
            sb.Append("<span class='badge badge-primary stat-badge'>已報名 " + mg.Booked + " / 共 " + mg.Total + "</span>");
            sb.Append("</h5></div>");
            sb.Append("<div id='col-" + mId + "' class='collapse' data-parent='#acc-team'>");
            sb.Append("<div class='card-body'>");

            // 若母光團直屬有人，先列一段
            if (mg.MotherMembers != null && mg.MotherMembers.Count > 0)
            {
                sb.Append("<div class='mb-2 small text-muted'>母光團直屬成員</div>");
                foreach (var mem in mg.MotherMembers.OrderBy(x => x.HUserName))
                {
                    sb.Append("<div class='member-row'>");
                    sb.Append("<div>");
                    sb.Append("<strong>" + HtmlEncode(mem.HUserName) + "</strong>");
                    sb.Append("<span class='small-muted ml-2'>期別：" + HtmlEncode(mem.HPeriod) + "</span>");
                    sb.Append("</div>");
                    sb.Append(mem.IsBooked ? "<span class='badge badge-success'>已報名</span>" : "<span class='badge badge-light'>未報名</span>");
                    sb.Append("</div>");
                }
                sb.Append("<hr />");
            }

            // 子光團 Accordion
            sb.Append("<div id='acc-cteam-" + mId + "' class='accordion'>");
            int cIdx = 0;
            foreach (var cg in mg.Children.OrderBy(x => x.Name))
            {
                string cId = mId + "-c-" + (cIdx++);
                sb.Append("<div class='card'>");
                sb.Append("<div class='card-header' id='head-" + cId + "'>");
                sb.Append("<h6 class='mb-0 d-flex align-items-center justify-content-between'>");
                sb.Append("<button type='button' class='btn btn-link' data-toggle='collapse' data-target='#col-" + cId + "' aria-expanded='false' aria-controls='col-" + cId + "'>");
                sb.Append("子光團：" + HtmlEncode(cg.Name));
                sb.Append("</button>");
                sb.Append("<span class='badge badge-info stat-badge'>已報名 " + cg.Booked + " / 共 " + cg.Total + "</span>");
                sb.Append("</h6></div>");
                sb.Append("<div id='col-" + cId + "' class='collapse' data-parent='#acc-cteam-" + mId + "'>");
                sb.Append("<div class='card-body scroll-y'>");

                foreach (var mem in cg.Members.OrderBy(x => x.HUserName))
                {
                    sb.Append("<div class='member-row'>");
                    sb.Append("<div>");
                    sb.Append("<strong>" + HtmlEncode(mem.HUserName) + "</strong>");
                    sb.Append("<span class='small-muted ml-2'>期別：" + HtmlEncode(mem.HPeriod) + "</span>");
                    sb.Append("</div>");
                    sb.Append(mem.IsBooked ? "<span class='badge badge-success'>已報名</span>" : "<span class='badge badge-light'>未報名</span>");
                    sb.Append("</div>");
                }

                sb.Append("</div></div></div>");
            }
            sb.Append("</div>"); // acc-cteam

            sb.Append("</div></div></div>");
        }

        sb.Append("</div>");
        return sb.ToString();
    }


    #region HTML Builders (C# 5 safe)

    private string BuildAreaAccordionHtml(IEnumerable<dynamic> lareaGroups)
    {
        var sb = new StringBuilder();
        sb.Append("<div id='acc-larea' class='accordion'>");
        int laIdx = 0;

        foreach (var la in lareaGroups)
        {
            string laId = "larea-" + (laIdx++);
            sb.Append("<div class='card'>");
            sb.Append("<div class='card-header' id='head-" + laId + "'>");
            sb.Append("<h5 class='mb-0 d-flex align-items-center justify-content-between'>");
            sb.Append("<button type='button' class='btn btn-link' data-toggle='collapse' data-target='#col-" + laId + "' aria-expanded='true' aria-controls='col-" + laId + "'>");
            sb.Append("大區：" + HtmlEncode(la.LAreaName));
            sb.Append("</button>");
            sb.Append("<span class='badge badge-primary stat-badge'>已報名 " + la.Booked + " / 共 " + la.Total + "</span>");
            sb.Append("</h5></div>");
            sb.Append("<div id='col-" + laId + "' class='collapse' data-parent='#acc-larea'><div class='card-body'>");
            sb.Append("<div id='acc-area-" + laId + "' class='accordion'>");

            int aIdx = 0;
            foreach (var a in la.Areas)
            {
                string aId = laId + "-a-" + (aIdx++);
                sb.Append("<div class='card'>");
                sb.Append("<div class='card-header' id='head-" + aId + "'>");
                sb.Append("<h6 class='mb-0 d-flex align-items-center justify-content-between'>");
                sb.Append("<button type='button' class='btn btn-link' data-toggle='collapse' data-target='#col-" + aId + "' aria-expanded='false' aria-controls='col-" + aId + "'>");
                sb.Append("區屬：" + HtmlEncode(a.AreaName));
                sb.Append("</button>");
                sb.Append("<span class='badge badge-info stat-badge'>已報名 " + a.Booked + " / 共 " + a.Total + "</span>");
                sb.Append("</h6></div>");
                sb.Append("<div id='col-" + aId + "' class='collapse' data-parent='#acc-area-" + laId + "'>");
                sb.Append("<div class='card-body scroll-y'>");

                foreach (var m in a.Members)
                {
                    sb.Append("<div class='member-row' data-member-id='" + m.HID + "' data-member-name='" + HtmlEncode(m.HUserName) + "' data-member-period='" + HtmlEncode(m.HPeriod) + "'>");
                    sb.Append("<div>");
                    sb.Append("<strong><a href='javascript:void(0);' class='js-open-member'>" + HtmlEncode(m.HUserName) + "</a></strong>");
                    sb.Append("<span class='small-muted ml-2'>期別：" + HtmlEncode(m.HPeriod) + "</span>");
                    sb.Append("<span class='small-muted ml-2'>學員類型：" + HtmlEncode(m.MemberTypeName) + "</span>");
                    sb.Append("</div>");
                    sb.Append("<div class='d-flex align-items-center'>");
                    var hasC = (_courseNoteMembers != null && _courseNoteMembers.Contains(m.HID)) ? " on" : "";
                    var hasP = (_personNoteMembers != null && _personNoteMembers.Contains(m.HID)) ? " on" : "";

                    sb.Append("<div class='note-icons'>");
                    sb.Append("<span class='note-icon ico-course" + hasC + "' title='有課程備註'>📝</span>");
                    sb.Append("<span class='note-icon ico-person" + hasP + "' title='有常態備註'>📌</span>");
                    sb.Append("</div>");

                    sb.Append(m.IsBooked ? "<span class='badge badge-success ml-2'>已報名</span>" : "<span class='badge badge-light ml-2'>未報名</span>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                }

                sb.Append("</div></div></div>");
            }

            sb.Append("</div></div></div></div>");
        }

        sb.Append("</div>");
        return sb.ToString();
    }

    private string BuildTeamAccordionHtml(IEnumerable<dynamic> teamGroups)
    {
        var sb = new StringBuilder();
        sb.Append("<div id='acc-team' class='accordion'>");
        int tIdx = 0;

        foreach (var t in teamGroups)
        {
            string tId = "team-" + (tIdx++);
            sb.Append("<div class='card'>");
            sb.Append("<div class='card-header' id='head-" + tId + "'>");
            sb.Append("<h5 class='mb-0 d-flex align-items-center justify-content-between'>");
            sb.Append("<button type='button' class='btn btn-link' data-toggle='collapse' data-target='#col-" + tId + "' aria-expanded='false' aria-controls='col-" + tId + "'>");
            sb.Append("光團：" + (t.TeamID == 0 ? "(未設定)" : t.TeamID.ToString()));
            sb.Append("</button>");
            sb.Append("<span class='badge badge-primary stat-badge'>已報名 " + t.Booked + " / 共 " + t.Total + "</span>");
            sb.Append("</h5></div>");
            sb.Append("<div id='col-" + tId + "' class='collapse' data-parent='#acc-team'>");
            sb.Append("<div class='card-body scroll-y'>");

            foreach (var m in t.Members)
            {
                sb.Append("<div class='member-row'>");
                sb.Append("<div>");
                sb.Append("<strong>" + HtmlEncode(m.HUserName) + "</strong>");
                sb.Append("<span class='small-muted ml-2'>期別：" + HtmlEncode(m.HPeriod) + "</span>");
                sb.Append("<span class='small-muted ml-2'>區屬ID：" + IntObjToText(m.HAreaID) + "</span>"); // ← 這行改了
                sb.Append("</div>");
                sb.Append(m.IsBooked ? "<span class='badge badge-success'>已報名</span>" : "<span class='badge badge-light'>未報名</span>");
                sb.Append("</div>");
            }

            sb.Append("</div></div></div>");
        }

        sb.Append("</div>");
        return sb.ToString();
    }

    private static string HtmlEncode(object s)
    {
        return System.Web.HttpUtility.HtmlEncode(s == null ? "" : s.ToString());
    }

    private static int SafeInt(string s)
    {
        int v;
        return int.TryParse(s, out v) ? v : 0;
    }

    #endregion

    #region POCOs
    private class MemberRow
    {
        public int HID { get; set; }
        public string HUserName { get; set; }
        public string HPeriod { get; set; }
        public int? HAreaID { get; set; }
        public string HTeamIDRaw { get; set; }  // ← 改成字串存原始值，如 "23,2"、"60,1"、"0"
        public int? HType { get; set; }
    }

    private class AreaRow
    {
        public int HID { get; set; }
        public int HLAreaID { get; set; }
        public string HArea { get; set; }
    }

    private class LAreaRow
    {
        public int HID { get; set; }
        public string HLArea { get; set; }
    }
    #endregion

    // ---- 放在類別內任何區塊都可（例如 #region Utils）----
    private static int ReadInt(IDataRecord rd, int ordinal)
    {
        if (rd.IsDBNull(ordinal)) return 0;
        object v = rd.GetValue(ordinal);

        if (v is int) return (int)v;
        if (v is long) return (int)(long)v;               // bigint
        if (v is short) return (int)(short)v;             // smallint
        if (v is byte) return (int)(byte)v;               // tinyint
        if (v is decimal) return (int)(decimal)v;         // numeric/decimal
        if (v is double) return (int)(double)v;
        if (v is float) return (int)(float)v;
        if (v is string)
        {
            int n; return int.TryParse((string)v, out n) ? n : 0;
        }
        // 其他型別：嘗試 Convert
        try { return Convert.ToInt32(v); } catch { return 0; }
    }

    private static int? ReadNullableInt(IDataRecord rd, int ordinal)
    {
        if (rd.IsDBNull(ordinal)) return null;
        return ReadInt(rd, ordinal);
    }

    private static string ReadString(IDataRecord rd, int ordinal)
    {
        return rd.IsDBNull(ordinal) ? "" : rd.GetValue(ordinal).ToString();
    }

    private static string IntObjToText(object val)
    {
        if (val == null || val is DBNull) return "-";
        try { return Convert.ToInt32(val).ToString(); }
        catch { return val.ToString(); }
    }

    private HashSet<int> GetMembersWithCourseNotes(string courseName)
    {
        var set = new HashSet<int>();
        using (var cn = new SqlConnection(_connStr))
        using (var cmd = new SqlCommand(@"
        SELECT DISTINCT HUserId
        FROM [HochiSystem].[dbo].[HCNotes]
        WHERE HCourseName = @c AND LEN(ISNULL(HDirections, '')) > 0;", cn))
        {
            cmd.Parameters.Add("@c", SqlDbType.NVarChar, 200).Value = courseName;
            cn.Open();
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    if (!rd.IsDBNull(0)) set.Add(Convert.ToInt32(rd.GetValue(0)));
                }
            }
        }
        return set;
    }

    private HashSet<int> GetMembersWithPersonNotes()
    {
        var set = new HashSet<int>();
        using (var cn = new SqlConnection(_connStr))
        using (var cmd = new SqlCommand(@"
        SELECT DISTINCT HUserId
        FROM [HochiSystem].[dbo].[HPNotes]
        WHERE LEN(ISNULL(HDirections, '')) > 0;", cn))
        {
            cn.Open();
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    if (!rd.IsDBNull(0)) set.Add(Convert.ToInt32(rd.GetValue(0)));
                }
            }
        }
        return set;
    }


    // HMTeam（母光團）
    private class HMTeamRow
    {
        public int HID { get; set; }
        public int HLAreaID { get; set; }
        public int HAreaID { get; set; }
        public string HMTeam { get; set; } // 母光團名稱
    }

    // HCTeam（子光團）
    private class HCTeamRow
    {
        public int HID { get; set; }
        public int HLAreaID { get; set; }
        public int HAreaID { get; set; }
        public int HMTeamID { get; set; }  // 對應母光團
        public string HCTeam { get; set; } // 子光團名稱
    }

    private List<HMTeamRow> GetHMTeams()
    {
        var list = new List<HMTeamRow>();
        using (var cn = new SqlConnection(_connStr))
        using (var cmd = new SqlCommand(@"
        SELECT HID, HLAreaID, HAreaID, HMTeam
        FROM [HochiSystem].[dbo].[HMTeam]
        WHERE HStatus = 1;", cn))
        {
            cn.Open();
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    list.Add(new HMTeamRow
                    {
                        HID = ReadInt(rd, 0),
                        HLAreaID = ReadInt(rd, 1),
                        HAreaID = ReadInt(rd, 2),
                        HMTeam = ReadString(rd, 3)
                    });
                }
            }
        }
        return list;
    }

    private List<HCTeamRow> GetHCTeams()
    {
        var list = new List<HCTeamRow>();
        using (var cn = new SqlConnection(_connStr))
        using (var cmd = new SqlCommand(@"
        SELECT HID, HLAreaID, HAreaID, HMTeamID, HCTeam
        FROM [HochiSystem].[dbo].[HCTeam]
        WHERE HStatus = 1;", cn))
        {
            cn.Open();
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    list.Add(new HCTeamRow
                    {
                        HID = ReadInt(rd, 0),
                        HLAreaID = ReadInt(rd, 1),
                        HAreaID = ReadInt(rd, 2),
                        HMTeamID = ReadInt(rd, 3),
                        HCTeam = ReadString(rd, 4)
                    });
                }
            }
        }
        return list;
    }

    // kind: 0=無光團或格式無效；1=母光團(HMTeam)；2=子光團(HCTeam)
    private class TeamRef { public int Kind; public int Id; }

    private static TeamRef ParseTeamRef(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw) || raw == "0")
            return new TeamRef { Kind = 0, Id = 0 };

        var parts = raw.Split(',');
        int id = 0, kind = 0;
        if (parts.Length > 0) int.TryParse(parts[0], out id);
        if (parts.Length > 1) int.TryParse(parts[1], out kind);
        if (kind != 1 && kind != 2) kind = 0;
        return new TeamRef { Kind = kind, Id = id };
    }

    private Dictionary<int, string> GetHMTypeMap()
    {
        var dict = new Dictionary<int, string>();
        using (var cn = new SqlConnection(_connStr))
        using (var cmd = new SqlCommand(@"
        SELECT HID, HMType
        FROM [HochiSystem].[dbo].[HMType]
        WHERE HStatus = 1;", cn))
        {
            cn.Open();
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    var id = ReadInt(rd, 0);
                    var name = ReadString(rd, 1);
                    if (!dict.ContainsKey(id)) dict.Add(id, name);
                }
            }
        }
        return dict;
    }

    public class MemberDetailDto
    {
        public int MemberId;
        public string MemberName;
        public string Period;

        public bool HasCourseNote;
        public bool HasPersonNote;

        public string[] CourseNotes; // 已有課程備註（簡要）
        public string[] PersonNotes; // 已有常態備註（簡要）

        public int WdM, WdA, WdE, HdM, HdA, HdE; // 0/1
    }

    // 取得成員詳細（含是否有備註 & 既有備註列表 & 時段）
    [WebMethod]
    public static MemberDetailDto GetMemberDetail(int memberId, string courseName)
    {
        var dto = new MemberDetailDto
        {
            MemberId = memberId,
            MemberName = "",
            Period = "",
            HasCourseNote = false,
            HasPersonNote = false,
            CourseNotes = new string[0],
            PersonNotes = new string[0],
            WdM = 0,
            WdA = 0,
            WdE = 0,
            HdM = 0,
            HdA = 0,
            HdE = 0
        };

        var connStr = System.Configuration.ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;

        // 讀基本姓名/期別（可選）
        using (var cn = new SqlConnection(connStr))
        using (var cmd = new SqlCommand(@"SELECT HUserName,HPeriod FROM [HochiSystem].[dbo].[HMember] WHERE HID=@id", cn))
        {
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = memberId;
            cn.Open();
            using (var rd = cmd.ExecuteReader())
            {
                if (rd.Read())
                {
                    dto.MemberName = rd.IsDBNull(0) ? "" : rd.GetString(0);
                    dto.Period = rd.IsDBNull(1) ? "" : rd.GetString(1);
                }
            }
        }

        // 課程備註（HCNotes）
        if (!string.IsNullOrWhiteSpace(courseName))
        {
            var notes = new List<string>();
            using (var cn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(@"
            SELECT TOP 20 HDirections, HCreateTime
            FROM [HochiSystem].[dbo].[HCNotes]
            WHERE HUserId=@uid AND HCourseName=@c
            ORDER BY HCreateTime DESC", cn))
            {
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = memberId;
                cmd.Parameters.Add("@c", SqlDbType.NVarChar, 200).Value = courseName;
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var dir = rd.IsDBNull(0) ? "" : rd.GetString(0);
                        var ts = rd.IsDBNull(1) ? "" : rd.GetDateTime(1).ToString("yyyy-MM-dd HH:mm");
                        if (!string.IsNullOrWhiteSpace(dir)) notes.Add(ts + "：" + dir);
                    }
                }
            }
            dto.CourseNotes = notes.ToArray();
            dto.HasCourseNote = dto.CourseNotes.Length > 0;
        }

        // 常態備註（HPNotes）
        {
            var notes = new List<string>();
            using (var cn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(@"
            SELECT TOP 20 HDirections, HCreateTime
            FROM [HochiSystem].[dbo].[HPNotes]
            WHERE HUserId=@uid
            ORDER BY HCreateTime DESC", cn))
            {
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = memberId;
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var dir = rd.IsDBNull(0) ? "" : rd.GetString(0);
                        var ts = rd.IsDBNull(1) ? "" : rd.GetDateTime(1).ToString("yyyy-MM-dd HH:mm");
                        if (!string.IsNullOrWhiteSpace(dir)) notes.Add(ts + "：" + dir);
                    }
                }
            }
            dto.PersonNotes = notes.ToArray();
            dto.HasPersonNote = dto.PersonNotes.Length > 0;
        }

        // 時段偏好（HDSchedule）
        {
            using (var cn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(@"
            SELECT HWeekdayMornings,HWeekdayAfternoons,HWeekdayEvenings,
                   HHolidayMornings,HHolidayAfternoons,HHolidayEvenings
            FROM [HochiSystem].[dbo].[HDSchedule]
            WHERE HUserId=@uid", cn))
            {
                cmd.Parameters.Add("@uid", SqlDbType.Int).Value = memberId;
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        dto.WdM = rd.IsDBNull(0) ? 0 : Convert.ToInt32(rd.GetValue(0));
                        dto.WdA = rd.IsDBNull(1) ? 0 : Convert.ToInt32(rd.GetValue(1));
                        dto.WdE = rd.IsDBNull(2) ? 0 : Convert.ToInt32(rd.GetValue(2));
                        dto.HdM = rd.IsDBNull(3) ? 0 : Convert.ToInt32(rd.GetValue(3));
                        dto.HdA = rd.IsDBNull(4) ? 0 : Convert.ToInt32(rd.GetValue(4));
                        dto.HdE = rd.IsDBNull(5) ? 0 : Convert.ToInt32(rd.GetValue(5));
                    }
                }
            }
        }

        return dto;
    }

    // 新增一筆課程備註
    [WebMethod(EnableSession = true)]
    public static bool SaveCourseNote(int memberId, string courseName, string directions)
    {
        if (string.IsNullOrWhiteSpace(courseName) || string.IsNullOrWhiteSpace(directions)) return false;
        var connStr = System.Configuration.ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        int creator = 0;
        var ctx = System.Web.HttpContext.Current;
        if (ctx != null && ctx.Session != null && ctx.Session["HUserHID"] != null)
        {
            int.TryParse(ctx.Session["HUserHID"].ToString(), out creator);
        }

        using (var cn = new SqlConnection(connStr))
        using (var cmd = new SqlCommand(@"
        INSERT INTO [HochiSystem].[dbo].[HCNotes]
            (HUserId,HCourseName,HDirections,HCreateUser,HCreateTime)
        VALUES
            (@uid,@c,@d,@u,GETDATE())", cn))
        {
            cmd.Parameters.Add("@uid", SqlDbType.Int).Value = memberId;
            cmd.Parameters.Add("@c", SqlDbType.NVarChar, 200).Value = courseName;
            cmd.Parameters.Add("@d", SqlDbType.NVarChar).Value = directions;
            cmd.Parameters.Add("@u", SqlDbType.Int).Value = creator;
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    // 新增一筆常態備註
    [WebMethod(EnableSession = true)]
    public static bool SavePersonNote(int memberId, string directions)
    {
        if (string.IsNullOrWhiteSpace(directions)) return false;
        var connStr = System.Configuration.ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        int creator = 0;
        var ctx = System.Web.HttpContext.Current;
        if (ctx != null && ctx.Session != null && ctx.Session["HUserHID"] != null)
        {
            int.TryParse(ctx.Session["HUserHID"].ToString(), out creator);
        }

        using (var cn = new SqlConnection(connStr))
        using (var cmd = new SqlCommand(@"
        INSERT INTO [HochiSystem].[dbo].[HPNotes]
            (HUserId,HDirections,HCreateUser,HCreateTime)
        VALUES
            (@uid,@d,@u,GETDATE())", cn))
        {
            cmd.Parameters.Add("@uid", SqlDbType.Int).Value = memberId;
            cmd.Parameters.Add("@d", SqlDbType.NVarChar).Value = directions;
            cmd.Parameters.Add("@u", SqlDbType.Int).Value = creator;
            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    // 儲存/更新時段偏好（Upsert）
    [WebMethod]
    public static bool SaveSchedule(int memberId, int wdM, int wdA, int wdE, int hdM, int hdA, int hdE)
    {
        var connStr = System.Configuration.ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        using (var cn = new SqlConnection(connStr))
        {
            cn.Open();
            // 先檢查有沒有
            bool exists = false;
            using (var chk = new SqlCommand(@"SELECT 1 FROM [HochiSystem].[dbo].[HDSchedule] WHERE HUserId=@uid", cn))
            {
                chk.Parameters.Add("@uid", SqlDbType.Int).Value = memberId;
                var o = chk.ExecuteScalar();
                exists = (o != null);
            }
            if (exists)
            {
                using (var up = new SqlCommand(@"
                UPDATE [HochiSystem].[dbo].[HDSchedule]
                SET HWeekdayMornings=@a, HWeekdayAfternoons=@b, HWeekdayEvenings=@c,
                    HHolidayMornings=@d, HHolidayAfternoons=@e, HHolidayEvenings=@f,
                    HModifytime=GETDATE()
                WHERE HUserId=@uid", cn))
                {
                    up.Parameters.Add("@a", SqlDbType.Int).Value = wdM;
                    up.Parameters.Add("@b", SqlDbType.Int).Value = wdA;
                    up.Parameters.Add("@c", SqlDbType.Int).Value = wdE;
                    up.Parameters.Add("@d", SqlDbType.Int).Value = hdM;
                    up.Parameters.Add("@e", SqlDbType.Int).Value = hdA;
                    up.Parameters.Add("@f", SqlDbType.Int).Value = hdE;
                    up.Parameters.Add("@uid", SqlDbType.Int).Value = memberId;
                    return up.ExecuteNonQuery() > 0;
                }
            }
            else
            {
                using (var ins = new SqlCommand(@"
                INSERT INTO [HochiSystem].[dbo].[HDSchedule]
                    (HUserId,HWeekdayMornings,HWeekdayAfternoons,HWeekdayEvenings,
                     HHolidayMornings,HHolidayAfternoons,HHolidayEvenings,HModifytime)
                VALUES (@uid,@a,@b,@c,@d,@e,@f,GETDATE())", cn))
                {
                    ins.Parameters.Add("@uid", SqlDbType.Int).Value = memberId;
                    ins.Parameters.Add("@a", SqlDbType.Int).Value = wdM;
                    ins.Parameters.Add("@b", SqlDbType.Int).Value = wdA;
                    ins.Parameters.Add("@c", SqlDbType.Int).Value = wdE;
                    ins.Parameters.Add("@d", SqlDbType.Int).Value = hdM;
                    ins.Parameters.Add("@e", SqlDbType.Int).Value = hdA;
                    ins.Parameters.Add("@f", SqlDbType.Int).Value = hdE;
                    return ins.ExecuteNonQuery() > 0;
                }
            }
        }
    }

}
