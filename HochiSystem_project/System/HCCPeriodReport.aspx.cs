using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class HCCPeriodReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region 搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        //顯示最後結果的方式
        //string QuerySql = "SELECT * FROM (SELECT a.HCCPeriodCode, a.HCourseName, (b.HArea + '/' + b.HPeriod + ' ' + b.HUserName) AS UserName, a.HDTotal,      a.HDCCPTimes, a.HDCCPAmount, ISNULL(z.AttendCount, 0) AS num, (a.HDCCPTimes - ISNULL(z.AttendCount, 0)) AS RemainTimes,   f.HCCPOrderCode,      e.HMerchantTradeNo, FORMAT(e.HAmount, 'N0') AS HAmount, e.HRtnMsg, LEFT(e.HProcessDate, 10) AS HProcessDate, '-' AS HPaymentDate, '-' AS HPayAmount FROM HCCPeriod AS a  LEFT JOIN MemberList AS b ON a.HMemberID = b.HID  LEFT JOIN HCCPOrderDetail AS d ON a.HCCPeriodCode = d.HCCPeriodCode  INNER JOIN HCCPOTRecord AS e ON d.HID = e.HCCPODetailID  JOIN HCCPOrder AS f ON d.HCCPOrderID = f.HID  LEFT JOIN HCCPOPaidRecord AS g ON d.HID = g.HCCPODetailID  LEFT JOIN ( SELECT d.HCCPeriodCode, a.HMemberID, COUNT(*) AS AttendCount FROM HCCPOrderDetail AS d JOIN HCCPOTRecord AS o ON d.HID = o.HCCPODetailID JOIN HCCPeriod AS a ON d.HCCPeriodCode = a.HCCPeriodCode WHERE a.HPaperYN = '1' AND a.HVerifyStatus = '3' AND a.HOrderStatus = '2' AND a.HStatus = '1' GROUP BY d.HCCPeriodCode, a.HMemberID  ) AS z ON a.HCCPeriodCode = z.HCCPeriodCode AND a.HMemberID = z.HMemberID  WHERE a.HPaperYN = '1' AND a.HVerifyStatus = '3' AND a.HOrderStatus = '2' AND a.HStatus = '1'  GROUP BY a.HCCPeriodCode, a.HCourseName, b.HArea, b.HPeriod, b.HUserName, a.HDTotal, a.HDCCPTimes, a.HDCCPAmount,f.HCCPOrderCode, e.HMerchantTradeNo, e.HAmount, e.HRtnMsg, e.HProcessDate, z.AttendCount UNION SELECT a.HCCPeriodCode, a.HCourseName, (b.HArea + '/' + b.HPeriod + ' ' + b.HUserName) AS UserName,  a.HDTotal,  a.HDCCPTimes, a.HDCCPAmount, ISNULL(z.AttendCount, 0) AS num, (a.HDCCPTimes - ISNULL(z.AttendCount, 0)) AS RemainTimes, '-' AS HCCPOrderCode, '-' AS HMerchantTradeNo,  '-' AS HAmount, '-' AS HRtnMsg, '-' AS HProcessDate, LEFT(g.HPaymentDate, 10) AS HPaymentDate,FORMAT(g.HPayAmount, 'N0') AS HPayAmount  FROM HCCPeriod AS a  LEFT JOIN MemberList AS b ON a.HMemberID = b.HID  LEFT JOIN HCCPOrderDetail AS d ON a.HCCPeriodCode = d.HCCPeriodCode  INNER JOIN HCCPOTRecord AS e ON d.HID = e.HCCPODetailID  JOIN HCCPOrder AS f ON d.HCCPOrderID = f.HID  JOIN HCCPOPaidRecord AS g ON d.HID = g.HCCPODetailID  LEFT JOIN (SELECT d.HCCPeriodCode, a.HMemberID, COUNT(*) AS AttendCount      FROM HCCPOrderDetail AS d JOIN HCCPOTRecord AS o ON d.HID = o.HCCPODetailID JOIN HCCPeriod AS a ON d.HCCPeriodCode = a.HCCPeriodCode      WHERE a.HPaperYN = '1' AND a.HVerifyStatus = '3' AND a.HOrderStatus = '2' AND a.HStatus = '1' GROUP BY d.HCCPeriodCode, a.HMemberID  ) AS z ON a.HCCPeriodCode = z.HCCPeriodCode AND a.HMemberID = z.HMemberID  WHERE a.HPaperYN = '1' AND a.HVerifyStatus = '3' AND a.HOrderStatus = '2' AND a.HStatus = '1'  GROUP BY a.HCCPeriodCode, a.HCourseName, b.HArea, b.HPeriod, b.HUserName, a.HDTotal, a.HDCCPTimes, a.HDCCPAmount, g.HPaymentDate, g.HPayAmount, z.AttendCount) AS m";

        string QuerySql = "SELECT * FROM (   SELECT     a.HCCPeriodCode,    a.HCourseName,    (b.HArea + '/' + b.HPeriod + ' ' + b.HUserName) AS UserName,    a.HDTotal,    a.HDCCPTimes,    a.HDCCPAmount,    1 AS num,    ROW_NUMBER() OVER (PARTITION BY a.HCCPeriodCode, a.HMemberID ORDER BY o.HProcessDate) AS DeductRound,    (a.HDCCPTimes - ROW_NUMBER() OVER (PARTITION BY a.HCCPeriodCode, a.HMemberID ORDER BY o.HProcessDate)) AS RemainTimes,    ROW_NUMBER() OVER (PARTITION BY a.HCCPeriodCode, a.HMemberID ORDER BY o.HProcessDate) AS PeriodIndex,    f.HCCPOrderCode,    o.HMerchantTradeNo,    FORMAT(o.HAmount, 'N0') AS HAmount,    o.HRtnMsg,    LEFT(o.HProcessDate, 10) AS HProcessDate,    '-' AS HPaymentDate,    '-' AS HPayAmount   FROM HCCPeriod AS a   LEFT JOIN MemberList AS b ON a.HMemberID = b.HID   LEFT JOIN HCCPOrderDetail AS d ON a.HCCPeriodCode = d.HCCPeriodCode   JOIN HCCPOTRecord AS o ON d.HID = o.HCCPODetailID   JOIN HCCPOrder AS f ON d.HCCPOrderID = f.HID   WHERE a.HPaperYN = '1'   AND a.HVerifyStatus = '3'   AND a.HOrderStatus = '2'   AND a.HStatus = '1'   UNION ALL   SELECT     a.HCCPeriodCode,    a.HCourseName,    (b.HArea + '/' + b.HPeriod + ' ' + b.HUserName) AS UserName,    a.HDTotal,    a.HDCCPTimes,    a.HDCCPAmount,    0 AS num,    NULL AS DeductRound,    a.HDCCPTimes AS RemainTimes,    NULL AS PeriodIndex,    '-' AS HCCPOrderCode,    '-' AS HMerchantTradeNo,    '-' AS HAmount,    '-' AS HRtnMsg,  '-' AS HProcessDate,    LEFT(g.HPaymentDate, 10) AS HPaymentDate,    FORMAT(g.HPayAmount, 'N0') AS HPayAmount   FROM HCCPeriod AS a   LEFT JOIN MemberList AS b ON a.HMemberID = b.HID   LEFT JOIN HCCPOrderDetail AS d ON a.HCCPeriodCode = d.HCCPeriodCode   JOIN HCCPOPaidRecord AS g ON d.HID = g.HCCPODetailID   WHERE a.HPaperYN = '1'   AND a.HVerifyStatus = '3'   AND a.HOrderStatus = '2'   AND a.HStatus = '1') AS m";


        string gOrder = " ORDER BY  CASE WHEN m.HProcessDate = '-' AND m.HPaymentDate = '-' THEN NULL WHEN m.HProcessDate = '-' THEN m.HPaymentDate WHEN m.HPaymentDate = '-' THEN m.HProcessDate ELSE CASE WHEN m.HProcessDate >= m.HPaymentDate THEN m.HProcessDate ELSE m.HPaymentDate END  END DESC";

        //搜尋條件
        StringBuilder sql = new StringBuilder(QuerySql);
        List<string> WHERE = new List<string>();

        //授權訂單代碼、綠界廠商訂單編號、學員姓名、授權書單號
        if (!string.IsNullOrEmpty(TB_Search.Text))
        {
            WHERE.Add(" (m.HCCPOrderCode LIKE '%" + TB_Search.Text.Trim() + "%' OR m.HCCPeriodCode LIKE '%" + TB_Search.Text.Trim() + "%' OR m.UserName LIKE N'%" + TB_Search.Text.Trim() + "%' OR m.HMerchantTradeNo LIKE '%" + TB_Search.Text.Trim() + "%') ");
        }

        //捐款項目
        if (!string.IsNullOrEmpty(TB_SHCourseName.Text))
        {
            WHERE.Add(" ( m.HCourseName LIKE N'%" + TB_SHCourseName.Text.Trim() + "%') ");
        }

        //執行日期
        string[] gHPDateRangeArray = TB_SHPaymentDate.Text.Trim() == "" ? "2023/01/01-3000/12/31".Split('-') : TB_SHPaymentDate.Text.Split('-');
        if (!string.IsNullOrEmpty(TB_SHPaymentDate.Text))
        {
            WHERE.Add(" ((CONVERT(varchar(10), m.HProcessDate, 111) >='" + gHPDateRangeArray[0].Trim() + "' and CONVERT(varchar(10), m.HProcessDate, 111)<='" + gHPDateRangeArray[1].Trim() + "') OR   (CONVERT(varchar(10), m.HPaymentDate, 111) >='" + gHPDateRangeArray[0].Trim() + "' and CONVERT(varchar(10), m.HPaymentDate, 111)<='" + gHPDateRangeArray[1].Trim() + "'))");
        }

        //授權結果：1=已授權，其他皆失敗
        if (DDL_SHRtnCode.SelectedValue != "0")
        {
            WHERE.Add(" ( m.HRtnMsg LIKE '%" + DDL_SHRtnCode.SelectedItem.Text + "%')");
        }

        //判斷使用者是否選擇了條件
        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" WHERE " + wh);
        }
        else
        {
            if (WHERE.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('至少輸入一個條件式!');", true);
                return;
            }
        }

        //Response.Write(sql.ToString() + gOrder);



        SDS_HCCPOTRecord.SelectCommand = sql.ToString() + gOrder;
        SDS_HCCPOTRecord.DataBind();
        Rpt_HCCPOTRecord.DataSourceID = "SDS_HCCPOTRecord";
        Rpt_HCCPOTRecord.DataBind();


        Panel_OrderList.Visible = true;


    }
    #endregion

    #region 搜尋取消功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_SHCourseName.Text = "";
        TB_Search.Text = "";
        TB_SHPaymentDate.Text = "";
        DDL_SHRtnCode.SelectedValue = "0";
        Panel_OrderList.Visible = false;
        SDS_HCCPOTRecord.SelectCommand = "";

        Rpt_HCCPOTRecord.DataSourceID = "SDS_HCCPOTRecord";
        Rpt_HCCPOTRecord.DataBind();


    }
    #endregion

    protected void Rpt_HCCPOTRecord_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        #region 金額轉成千分位
        //if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HAmount")).Text))
        //{
        //    ((Label)e.Item.FindControl("LB_HAmount")).Text = Convert.ToInt32(((Label)e.Item.FindControl("LB_HAmount")).Text).ToString("N0");
        //}

        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HDCCPAmount")).Text))
        {
            ((Label)e.Item.FindControl("LB_HDCCPAmount")).Text = Convert.ToInt32(((Label)e.Item.FindControl("LB_HDCCPAmount")).Text).ToString("N0");
        }

        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HDTotal")).Text))
        {
            ((Label)e.Item.FindControl("LB_HDTotal")).Text = Convert.ToInt32(((Label)e.Item.FindControl("LB_HDTotal")).Text).ToString("N0");
        }

        //if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HPayAmount")).Text))
        //{
        //    ((Label)e.Item.FindControl("LB_HPayAmount")).Text = Convert.ToInt32(((Label)e.Item.FindControl("LB_HPayAmount")).Text).ToString("N0");
        //}


        #endregion


        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HRtnMsg")).Text) && ((Label)e.Item.FindControl("LB_HRtnMsg")).Text.Contains("失敗") == true)
        {
            ((Label)e.Item.FindControl("LB_HRtnMsg")).Text = "付款失敗";
            ((Label)e.Item.FindControl("LB_HRtnMsg")).CssClass = "text-danger";
        }



    }
}