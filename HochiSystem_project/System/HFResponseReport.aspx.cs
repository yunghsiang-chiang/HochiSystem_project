using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class HFResponseReport : System.Web.UI.Page
{

    string gSql= @"DECLARE @yy_now  int = YEAR(GETDATE()) % 100;       -- 今年後兩碼
DECLARE @yy_prev int = (YEAR(GETDATE()) - 1) % 100; -- 去年後兩碼

;WITH TargetMembers AS (  -- (可選) 只統計新朋友(期末=00) 或 今年/去年新同修；且啟用
    SELECT m.HID, m.HUserName, m.HPeriod, m.HAreaID
    FROM dbo.HMember AS m
    WHERE m.HStatus = 1
      AND (
             RIGHT(m.HPeriod, 2) = '00'
          OR TRY_CONVERT(int, LEFT(m.HPeriod, 2)) IN (@yy_now, @yy_prev)
      )
),
MemberList AS (            -- 學員 MemberList + 區屬排序
    SELECT  m.HID                      AS MemberID,
            m.HUserName               AS MemberName,
            a.HArea                   AS MemberArea,
            a.HSort                   AS MemberAreaSort,   -- 區屬排序值
            ISNULL(a.HArea, N'') + N'/' + ISNULL(m.HPeriod, N'') + N' ' + ISNULL(m.HUserName, N'') AS MemberMemberList
    FROM TargetMembers m
    LEFT JOIN dbo.HArea a ON a.HID = m.HAreaID
),
MentorMap AS (             -- 學員 → 護持者（含護持者 MemberList + 區屬排序）
    SELECT
        r.HMemberID              AS MemberID,
        r.HMentorMemberID        AS MentorID,
        mm.HUserName             AS MentorName,
        ma.HArea                 AS MentorArea,
        ma.HSort                 AS MentorAreaSort,   -- 護持者的區屬排序值（如需使用可保留）
        ISNULL(ma.HArea, N'') + N'/' + ISNULL(mm.HPeriod, N'') + N' ' + ISNULL(mm.HUserName, N'') AS MentorMemberList
    FROM dbo.HMMentorRelationship r
    JOIN dbo.HMember mm  ON mm.HID = r.HMentorMemberID AND mm.HStatus = 1
    LEFT JOIN dbo.HArea ma ON ma.HID = mm.HAreaID
),
Posts AS (                 -- 學員的所有貼文
    SELECT j.HID AS JournalID,
           j.HMemberID AS MemberID
    FROM dbo.HFeelingsJournals j
    JOIN MemberList ml ON ml.MemberID = j.HMemberID
),
MemberTotals AS (          -- 每位學員的總發文數
    SELECT MemberID, COUNT(*) AS TotalPosts
    FROM Posts
    GROUP BY MemberID
),
MentorReplies AS (         -- 每位(學員×護持者)針對哪些貼文有回覆（同一篇不重複計）
    SELECT DISTINCT
           p.MemberID,
           mm.MentorID,
           p.JournalID
    FROM Posts p
    JOIN dbo.HFeelingsJournalsMsg msg
      ON msg.HFeelingsJournalsID = p.JournalID
    JOIN MentorMap mm
      ON mm.MemberID = p.MemberID
     AND mm.MentorID = msg.HMemberID   -- 只計護持者的回覆
)
SELECT
    ml.MemberID,
    ml.MemberName,
    ml.MemberArea,
    ml.MemberMemberList,
    mt.TotalPosts,                                     -- 該學員總發文數
    mm.MentorID,
    mm.MentorName,
    mm.MentorArea,
    mm.MentorMemberList,
    COUNT(r.JournalID)            AS MentorRepliedPosts,      -- 該護持者回覆篇數
    CAST(100.0 * COUNT(r.JournalID) / NULLIF(mt.TotalPosts,0) AS decimal(5,2)) AS MentorReplyRatePct  -- 回覆占比(%)
FROM MentorMap mm
JOIN MemberList ml     ON ml.MemberID = mm.MemberID
JOIN MemberTotals mt   ON mt.MemberID = mm.MemberID
LEFT JOIN MentorReplies r
       ON r.MemberID = mm.MemberID
      AND r.MentorID = mm.MentorID
GROUP BY
    ml.MemberID, ml.MemberName, ml.MemberArea, ml.MemberAreaSort, ml.MemberMemberList,
    mt.TotalPosts,
    mm.MentorID, mm.MentorName, mm.MentorArea, mm.MentorMemberList
ORDER BY
    CASE WHEN ml.MemberAreaSort IS NULL THEN 1 ELSE 0 END,  -- ✅ 非 NULL 在前，NULL 在後
    ml.MemberAreaSort ASC,                                  -- ✅ 依 HArea.HSort 排
    ml.MemberName,
    MentorRepliedPosts DESC,
    mm.MentorName;

";



    protected void Page_Load(object sender, EventArgs e)
    {
        //初始化
        if (!IsPostBack)
        {
            //區屬選單
            string strSelHA = "SELECT HID, HArea, HStatus FROM HArea WHERE HStatus=1 ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC ";
            SqlDataReader MyQueryHA = SQLdatabase.ExecuteReader(strSelHA);
            DDL_HArea.Items.Add(new ListItem("請選擇學員區屬", "0"));
            while (MyQueryHA.Read())
            {
                    DDL_HArea.Items.Add(new ListItem(MyQueryHA["HArea"].ToString(), (MyQueryHA["HID"].ToString())));
            }
            MyQueryHA.Close();

            SDS_HFeelingsPost.SelectCommand = gSql;
            Rpt_HFeelingsPost.DataBind();
        }

 
    }


    #region 搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        // 讀取前端控制項（若某些控制項不存在，請保留預設空字串 / all）
        var areaText = (DDL_HArea != null && DDL_HArea.SelectedIndex > 0) ? DDL_HArea.SelectedItem.Text.Trim() : ""; // 區屬
        var memberName = !string.IsNullOrEmpty(TB_HMemberUserName.Text.Trim()) ? TB_HMemberUserName.Text.Trim() : "";  // 學員姓名關鍵字
        var mentorName = !string.IsNullOrEmpty(TB_MentorUsername.Text.Trim()) ? TB_MentorUsername.Text.Trim() : "";   // 護持者姓名關鍵字


        // 以原本 gSql 為基礎，加外層 WHERE 條件（不改 gSql 內容）
        string filteredSql =
            "DECLARE @yy_now  int = YEAR(GETDATE()) % 100; " +
            "DECLARE @yy_prev int = (YEAR(GETDATE()) - 1) % 100; " +

        @";
WITH TargetMembers AS (
    SELECT m.HID, m.HUserName, m.HPeriod, m.HAreaID
    FROM dbo.HMember AS m
    WHERE m.HStatus = 1
      AND (
             RIGHT(m.HPeriod, 2) = '00'
          OR TRY_CONVERT(int, LEFT(m.HPeriod, 2)) IN (@yy_now, @yy_prev)
      )
),
MemberList AS (
    SELECT  m.HID AS MemberID,
            m.HUserName AS MemberName,
            a.HArea AS MemberArea,
            a.HSort AS MemberAreaSort,
            ISNULL(a.HArea, N'') + N'/' + ISNULL(m.HPeriod, N'') + N' ' + ISNULL(m.HUserName, N'') AS MemberMemberList
    FROM TargetMembers m
    LEFT JOIN dbo.HArea a ON a.HID = m.HAreaID
),
MentorMap AS (
    SELECT
        r.HMemberID AS MemberID,
        r.HMentorMemberID AS MentorID,
        mm.HUserName AS MentorName,
        ma.HArea AS MentorArea,
        ma.HSort AS MentorAreaSort,
        ISNULL(ma.HArea, N'') + N'/' + ISNULL(mm.HPeriod, N'') + N' ' + ISNULL(mm.HUserName, N'') AS MentorMemberList
    FROM dbo.HMMentorRelationship r
    JOIN dbo.HMember mm  ON mm.HID = r.HMentorMemberID AND mm.HStatus = 1
    LEFT JOIN dbo.HArea ma ON ma.HID = mm.HAreaID
),
Posts AS (
    SELECT j.HID AS JournalID,
           j.HMemberID AS MemberID
    FROM dbo.HFeelingsJournals j
    JOIN MemberList ml ON ml.MemberID = j.HMemberID
),
MemberTotals AS (
    SELECT MemberID, COUNT(*) AS TotalPosts
    FROM Posts
    GROUP BY MemberID
),
MentorReplies AS (
    SELECT DISTINCT
           p.MemberID,
           mm.MentorID,
           p.JournalID
    FROM Posts p
    JOIN dbo.HFeelingsJournalsMsg msg
      ON msg.HFeelingsJournalsID = p.JournalID
    JOIN MentorMap mm
      ON mm.MemberID = p.MemberID
     AND mm.MentorID = msg.HMemberID
),
Base AS (
    SELECT
        ml.MemberID,
        ml.MemberName,
        ml.MemberArea,
        ml.MemberAreaSort,
        ml.MemberMemberList,
        mt.TotalPosts,
        mm.MentorID,
        mm.MentorName,
        mm.MentorArea,
        mm.MentorMemberList,
        COUNT(r.JournalID) AS MentorRepliedPosts,
        CAST(100.0 * COUNT(r.JournalID) / NULLIF(mt.TotalPosts,0) AS decimal(5,2)) AS MentorReplyRatePct
    FROM MentorMap mm
    JOIN MemberList ml   ON ml.MemberID = mm.MemberID
    JOIN MemberTotals mt ON mt.MemberID = mm.MemberID
    LEFT JOIN MentorReplies r
           ON r.MemberID = mm.MemberID
          AND r.MentorID = mm.MentorID
    GROUP BY
        ml.MemberID, ml.MemberName, ml.MemberArea, ml.MemberAreaSort, ml.MemberMemberList,
        mt.TotalPosts,
        mm.MentorID, mm.MentorName, mm.MentorArea, mm.MentorMemberList
)
SELECT *
FROM Base
WHERE
    (N'" + areaText + @"' = N'' OR MemberArea = N'" + areaText + @"')
AND (N'" + memberName + @"' = N'' OR MemberName LIKE N'%" + memberName + @"%')
AND (N'" + mentorName + @"' = N'' OR MentorName LIKE N'%" + mentorName + @"%')
ORDER BY
    CASE WHEN MemberAreaSort IS NULL THEN 1 ELSE 0 END,
    MemberAreaSort ASC,
    MemberName,
    MentorRepliedPosts DESC,
    MentorName;";

        // 建議 DDL_ReplyStatus 的值用：all / none / partial / full

        SDS_HFeelingsPost.SelectCommand = filteredSql;

        Rpt_HFeelingsPost.DataBind();
    }
    #endregion


    #region 取消搜尋功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        // 還原 UI 狀態
        if (DDL_HArea != null) DDL_HArea.SelectedIndex = 0;
        var tbMentee = (FindControl("TB_MemberName") as TextBox);
        if (tbMentee != null) tbMentee.Text = "";
        var tbMentor = (FindControl("TB_MentorName") as TextBox);
        if (tbMentor != null) tbMentor.Text = "";
        var ddlReply = (FindControl("DDL_ReplyStatus") as DropDownList);
        if (ddlReply != null) ddlReply.SelectedValue = "all";

        // 還原查詢
        SDS_HFeelingsPost.SelectCommand = gSql;
        SDS_HFeelingsPost.SelectParameters.Clear();
        Rpt_HFeelingsPost.DataBind();
    }
    #endregion

    protected void Rpt_HFeelingsPost_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var lblRate = (Label)e.Item.FindControl("LB_MentorReplyRatePct");
        decimal rate;

        if (lblRate != null && decimal.TryParse(lblRate.Text, out rate))
        {
            if (rate < 100 && rate > 0)
            {
                ((HtmlTableRow)e.Item.FindControl("Tr_Row")).Style.Add("background", "#ffdd87");
            }
            else if (rate ==0)
            {
                ((HtmlTableRow)e.Item.FindControl("Tr_Row")).Style.Add("background", "#ffc9c9");
            }
            else if (rate == 100)
            {
                ((HtmlTableRow)e.Item.FindControl("Tr_Row")).Style.Add("background", "#dbfbce");
            }
        }

    }

 
}