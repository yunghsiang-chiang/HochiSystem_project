using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.Expressions;

public partial class HochiSCMaster : System.Web.UI.MasterPage
{
    string ConStr = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;


    string searchstr = "SELECT a.HID, a.HCourseID, c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  a.HTopicName, a.HPinTop, a.HContent, a.HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID  LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE  a.HStatus=1 ";

    //GA20250706_新增EIP搜尋語法
    string EIPsearchstr = "SELECT l_id AS HID, '' AS HCourseID,  '大愛光老師專欄' AS HSCForumClassB, 'EIP' AS HSCForumClassC, l_title AS HTopicName, '0' AS HPinTop, l_Content AS HContent, '' AS HFile1, l_cname AS UserName, '' AS HImg,  l_recorddate AS HCreateDT,  l_udate AS HModifyDT, 'EIP' AS source, l_id AS EIPID, '' AS HIRestriction FROM heip.laoshi WHERE l_status = 2 AND l_category = 0  AND l_recorddate > '20250913000000' AND  l_recorddate < DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 15 HOUR), '%Y%m%d%H%i%s') ";
     


    protected void Page_Load(object sender, EventArgs e)
    {
       

        if (!IsPostBack)
        {
            SDS_HSCFClassFavorite.SelectCommand = "SELECT LEFT(m.SCFCName, len(m.SCFCName) - 1) AS HSCFCName, LEFT(m.HSCForumClassID, len(m.HSCForumClassID) - 1)  AS HSCForumClassID FROM ( SELECT DISTINCT (cast(HSCForumClass.HSCFCName AS NVARCHAR) + ',') AS SCFCName,   (cast(HSCForumClass.HID AS NVARCHAR) + ',') AS HSCForumClassID    FROM HMember CROSS APPLY SPLIT(HSCFClassFavorite, ',') INNER JOIN HSCForumClass ON value = HSCForumClass.HID WHERE HMember.HID='" + LB_HUserHID.Text + "') AS m";
            Rpt_HSCFClassFavorite.DataBind();
        }



    }

    protected void Page_Init(object sender, EventArgs e)
    {
        string AccountHID = null;//紀錄有後台權限HID
        string HAccount = null;
        string HUserName = null;
        string HUserHID = null;
        string EIPUID = null;
        int LonginStatus = 0; //0=未登入；1=已登入

        #region 登入判斷
        if (Session["HAccount"] != null)
        {
            if (Request.Cookies["HochiInfo"] != null)
            {
                HAccount = (Session["HAccount"].ToString() == "" || Session["HAccount"].ToString() == null) ? (Request.Cookies["HochiInfo"]["HAccount"].ToString() == "" || Request.Cookies["HochiInfo"]["HAccount"].ToString() == null) ? "" : Request.Cookies["HochiInfo"]["HAccount"].ToString() : Session["HAccount"].ToString();
            }
            else
            {
                HAccount = Session["HAccount"].ToString();
            }
        }
        else
        {

            if (Request.Cookies["HochiInfo"] != null)
            {

                if (!string.IsNullOrEmpty(Request.Cookies["HochiInfo"]["HAccount"]))
                {
                    HAccount = Request.Cookies["HochiInfo"]["HAccount"].ToString();
                    Response.Cookies["HochiInfoHAccount"].Value = Request.Cookies["HochiInfo"]["HAccount"].ToString();
                    Session["HAccount"] = Request.Cookies["HochiInfo"]["HAccount"].ToString();
                    LonginStatus = 1;
                    //判斷是否有後臺權限
                    AccountHID = LB_HUserHID.Text;
                }

                #region 多一層cookie確保cookie失效問題
                else if (Request.Cookies["HochiInfoHAccount"] != null)
                {
                    HAccount = Request.Cookies["HochiInfoHAccount"].Value;
                    Response.Cookies["HochiInfo"]["HAccount"] = Request.Cookies["HochiInfoHAccount"].Value;
                    Session["HAccount"] = Request.Cookies["HochiInfoHAccount"].Value;
                    LonginStatus = 1;
                    //判斷是否有後臺權限
                    AccountHID = LB_HUserHID.Text;
                }
                #endregion

                else
                {

                    Response.Write("<script>alert('Cookie因瀏覽器問題已經消失了，可能要先清除瀏覽器Cookie後再登入喔~!');window.location.href='Hlogin.aspx';</script>");
                }
            }

            #region 多一層cookie確保cookie失效問題
            else if (Request.Cookies["HochiInfoHAccount"] != null)
            {

                HAccount = Request.Cookies["HochiInfoHAccount"].Value;

                Response.Cookies["HochiInfo"]["HAccount"] = Request.Cookies["HochiInfoHAccount"].Value;
                Session["HAccount"] = Request.Cookies["HochiInfoHAccount"].Value;
                LonginStatus = 1;
                //判斷是否有後臺權限
                AccountHID = LB_HUserHID.Text;
            }
            #endregion
            else
            {
                LonginStatus = 0;
            }

        }
        #endregion

        if (HAccount != null)
        {

            SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HUserName, HAccount, HEIPUid FROM HMember WHERE HAccount ='" + HAccount + "' AND HStatus=1");
            if (dr.Read())
            {
                Session["HUserName"] = dr["HUserName"].ToString() == "周瑞宏" ? "大愛光老師" : dr["HUserName"].ToString();//使用者的名稱
                Session["HUserHID"] = dr["HID"].ToString();//使用者的HID代號
                Session["EIPUID"] = dr["HEIPUid"].ToString();//使用者的EIP UID代號
                Session["HUserHID"] = dr["HID"].ToString();//使用者的HID代號

                if (!string.IsNullOrEmpty(dr["HUserName"].ToString()))
                {
                    HUserName = (Session["HUserName"].ToString() == "" || Session["HUserName"].ToString() == null) ? (Request.Cookies["HochiInfo"]["HUserName"].ToString() == "" || Request.Cookies["HochiInfo"]["HUserName"].ToString() == null) ? "" : Request.Cookies["HochiInfo"]["HUserName"].ToString() : Session["HUserName"].ToString();
                }

                HUserHID = (Session["HUserHID"].ToString() == "" || Session["HUserHID"].ToString() == null) ? (Request.Cookies["HochiInfo"]["HUserHID"].ToString() == "" || Request.Cookies["HochiInfo"]["HUserHID"].ToString() == null) ? "" : Request.Cookies["HochiInfo"]["HUserHID"].ToString() : Session["HUserHID"].ToString();

                if (!string.IsNullOrEmpty(dr["HEIPUid"].ToString()))
                {
                    EIPUID = (Session["EIPUID"].ToString() == "" || Session["EIPUID"].ToString() == null) ? (Request.Cookies["HochiInfo"]["EIPUID"].ToString() == "" || Request.Cookies["HochiInfo"]["EIPUID"].ToString() == null) ? "" : Request.Cookies["HochiInfo"]["EIPUID"].ToString() : Session["EIPUID"].ToString();
                }
                //GA20240604_加入判斷，若資料庫沒有EIPuid則先帶0
                else
                {
                    EIPUID = "0";
                }

            }
            dr.Close();

            LonginStatus = 1;
            AccountHID = Session["HUserHID"].ToString();
        }

        if (!string.IsNullOrEmpty(HAccount) && !string.IsNullOrEmpty(HUserName) && !string.IsNullOrEmpty(HUserHID) && !string.IsNullOrEmpty(EIPUID))
        {
            LB_HAccount.Text = HAccount;
            LB_HUserName.Text = HUserName;
            LB_HUserHID.Text = HUserHID;
            TB_EIPUid.Text = EIPUID;
        }

        if (LonginStatus == 1)
        {

            LoginStatus(1, HUserHID);  //登入中


          
            ////判斷是否有後臺權限
            //if (BackPermissionCheck(AccountHID) == 1)
            //{
            //    EnterBack.Visible = true;
            //}
            //else
            //{
            //    EnterBack.Visible = false;
            //}
        }
        else
        {
            if (Session["HAccount"] == null && Session["HUserName"] == null && Session["HUserHID"] == null)
            {

                // 取得包含檔名 + QueryString 的部分
                string pathAndQuery = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
                string fileWithQuery = pathAndQuery.StartsWith("/")? pathAndQuery.Substring(1) : pathAndQuery;  // 去掉開頭的 "/"
                Response.Write("<script>alert('您尚未登入哦~系統將引導您先登入，謝謝~!');window.location.href='HLogin.aspx?Url=" + fileWithQuery + "';</script>");

            }
           //Response.Write("<script>alert('您尚未登入哦~系統將引導您先登入，謝謝~!');window.location.href='HLogin.aspx?F=1'</script>");

        }


    }




    /// <summary>
    /// 執行登入狀態的按鈕顯示
    /// </summary>
    /// <param name="Status">1：登入狀態，0：登出狀態</param>
    protected void LoginStatus(int Status, string HUserHID)
    {
        if (Status == 1)
        {
            member.Visible = true;//學員專區
                                  //member.Attributes.Add("style", "margin-left:35%");

           



        }
        else
        {
            member.Visible = false;
        }
    }


    #region 所有討論區資料繫結
    protected void Rpt_HSCForumClassSearch_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        //次類別資料
        Repeater Rpt_HSCForumClassBSearch = e.Item.FindControl("Rpt_HSCForumClassBSearch") as Repeater;  //找到repeater物件
        SqlDataSource SDS_HSCForumClassBSearch = e.Item.FindControl("SDS_HSCForumClassBSearch") as SqlDataSource;  //找到SqlDataSource物件        
        SDS_HSCForumClassBSearch.SelectCommand = "SELECT a.HID, a.HSCFCName, a.HSCFCLevel, a.HStatus, b.HSCFCName AS HSCForumMaster FROM HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON b.HID=a.HSCFCMaster WHERE a.HStatus = 1 AND a.HSCFCLevel = '20' AND a.HSCFCMaster ='" + ((Label)e.Item.FindControl("LB_HID")).Text + "';";
        Rpt_HSCForumClassBSearch.DataBind();

        //若沒有子項目則不用顯示箭頭
        if (Rpt_HSCForumClassBSearch.Items.Count == 0)
        {
            ((HtmlControl)e.Item.FindControl("arrowicon")).Visible = false;

        }

    }
    #endregion

    #region 所有討論區資料繫結
    protected void Rpt_HSCForumClassBSearch_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

    }
    #endregion

    #region 次類別進入HSCForum頁面
    protected void LBtn_HSCForumClassB_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_HSCForumClassB = sender as LinkButton;
        string gHSCForumClassB_CA = LBtn_HSCForumClassB.CommandArgument;
        
        Response.Redirect("HSCForum.aspx?FClassBID=" + gHSCForumClassB_CA);
    }
    #endregion

    #region 我的最愛繫結
    protected void Rpt_HSCFClassFavorite_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
    }
    #endregion

    #region 我的最愛連結至討論區
    protected void LBtn_HSCFClassFavoriteName_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_HSCFClassFavoriteName = sender as LinkButton;
        string gHSCForumClassC_CA = LBtn_HSCFClassFavoriteName.CommandArgument;

        Response.Redirect("HSCForumDetail.aspx?FClassCID=" + gHSCForumClassC_CA);
    }
    #endregion

    #region 關鍵字搜尋
    //MA20240723_新增關鍵字搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        StringBuilder sql = new StringBuilder(searchstr);
        //EIP
        StringBuilder EIPSql = new StringBuilder(EIPsearchstr);


        List<string> WHERE = new List<string>();
        List<string> EIPWHERE = new List<string>();//EIP搜尋範圍

        if (!string.IsNullOrEmpty(TB_SKeyWord.Text.Trim()))
        {
            WHERE.Add(" ((c.HSCFCName LIKE N'%" + TB_SKeyWord.Text.Trim() + "%') OR  (b.HSCFCName LIKE N'%" + TB_SKeyWord.Text.Trim() + "%') OR  (HTopicName LIKE N'%" + TB_SKeyWord.Text.Trim() + "%') OR  (HContent LIKE N'%" + TB_SKeyWord.Text.Trim() + "%') OR  (e.HUserName LIKE N'%" + TB_SKeyWord.Text.Trim() + "%'))");

            EIPWHERE.Add(" ((l_title LIKE N'%" + TB_SKeyWord.Text.Trim() + "%') OR  (l_Content LIKE N'%" + TB_SKeyWord.Text.Trim() + "%')) ");
        }

        //判斷使用者是否選擇了條件
        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" AND " + wh);
        }


        if (EIPWHERE.Count > 0)
        {
            string wh = string.Join(" AND ", EIPWHERE.ToArray());
            EIPSql.Append(" AND " + wh);
        }


        Session["SearchIndex"] = sql.ToString() + " ORDER BY IIF(a.HPinTop IS NULL,0,a.HPinTop)  DESC, IIF(a.HModifyDT IS NOT NULL,a.HModifyDT, a.HCreateDT) DESC";

        Session["SearchIndexEIP"] = EIPSql.ToString() + " ORDER BY l_no DESC";


        Response.Redirect("HSCIndex.aspx");
    }
    #endregion

    #region 進階搜尋功能
    //MA20240723_新增進階搜尋功能
    protected void Btn_Search_Click(object sender, EventArgs e)
    {
        StringBuilder sql = new StringBuilder(searchstr);
        //EIP
        StringBuilder EIPSql = new StringBuilder(EIPsearchstr);


        List<string> WHERE = new List<string>();
        List<string> AllWHERE = new List<string>();//全站搜尋範圍

        List<string> EIPWHERE = new List<string>();//EIP搜尋範圍

        #region 關鍵字&全站搜尋範圍

        string KeyWordStr = " ((c.HSCFCName LIKE N'%" + TB_SAKeyWord.Text.Trim() + "%') OR  (b.HSCFCName LIKE N'%" + TB_SAKeyWord.Text.Trim() + "%') OR  (HTopicName LIKE N'%" + TB_SAKeyWord.Text.Trim() + "%') OR  (HContent LIKE N'%" + TB_SAKeyWord.Text.Trim() + "%') OR  (e.HUserName LIKE N'%" + TB_SAKeyWord.Text.Trim() + "%'))";
        string SAllRangeStr1 = " (b.HSCFCName LIKE N'%" + TB_SAKeyWord.Text.Trim() + "%') ";
        string SAllRangeStr2 = " (((SELECT DISTINCT (cast(b.HCourseName AS NVARCHAR) + ',') FROM HSCTopic CROSS APPLY SPLIT(HSCTopic.HCourseID, ',') INNER JOIN HCourse as b ON value = b.HID WHERE HSCTopic.HCourseID=a.HCourseID FOR XML PATH('')) LIKE N'%" + TB_SAKeyWord.Text.Trim() + "%')) ";
        string SAllRangeStr3 = " (HTopicName LIKE N'%" + TB_SAKeyWord.Text.Trim() + "%')  ";
        string SAllRangeStr4 = " (HContent LIKE N'%" + TB_SAKeyWord.Text.Trim() + "%')  ";

        string[] SAllRangeArray = { KeyWordStr, SAllRangeStr1, SAllRangeStr2, SAllRangeStr3, SAllRangeStr4 };

        //string AllRangeValues = "";
        foreach (ListItem SAllRange in CBL_SAllRange.Items)
        {
            if (SAllRange.Selected == true)
            {
                if (SAllRange.Value == "1")
                {
                    AllWHERE.Add(SAllRangeArray[1]);
                }
                if (SAllRange.Value == "2")
                {
                    AllWHERE.Add(SAllRangeArray[2]);
                }
                if (SAllRange.Value == "3")
                {
                    AllWHERE.Add(SAllRangeArray[3]);
                }
                if (SAllRange.Value == "4")
                {
                    AllWHERE.Add(SAllRangeArray[4]);
                }
                //AllRangeValues += SAllRange.Value + ",";
            }
            else
            {
                WHERE.Add(SAllRangeArray[0]);
            }
        }


        EIPWHERE.Add(" ((l_title LIKE N'%" + TB_SAKeyWord.Text.Trim() + "%') OR  (l_Content LIKE N'%" + TB_SAKeyWord.Text.Trim() + "%')) ");
        #endregion

        #region HashTag標籤
        foreach (ListItem HashTag in LBox_SHHashTag.Items)
        {
            if (HashTag.Selected == true)
            {
                AllWHERE.Add(" ((a.HHashTag LIKE '%" + HashTag.Value + "%'))");
            }

        }
        #endregion

        #region 發表人
        if (!string.IsNullOrEmpty(TB_SHCreate.Text.Trim()))
        {
            WHERE.Add(" ((e.HUserName LIKE N'%" + TB_SHCreate.Text + "%'))");

            EIPWHERE.Add(" ((l_cname LIKE N'%" + TB_SHCreate.Text + "%'))");
        }
        #endregion

        #region 發表日期區間
        string[] gSHCreateDTArray = TB_SHCreateDT.Text.Trim() == "" ? "2000/01/01-3000/12/31".Split('-') : TB_SHCreateDT.Text.Split('-');
        if (!string.IsNullOrEmpty(TB_SHCreateDT.Text))
        {
            WHERE.Add(" ((CONVERT(varchar(10), a.HCreateDT, 111)>='" + gSHCreateDTArray[0].Trim() + "' AND CONVERT(varchar(10), a.HCreateDT, 111)<='" + gSHCreateDTArray[1].Trim() + "')) ");

            EIPWHERE.Add(" ((l_recorddate>='" + Convert.ToDateTime(gSHCreateDTArray[0].Trim()).ToString("yyyyMMdd") + "' AND l_recorddate <='" + Convert.ToDateTime(gSHCreateDTArray[1].Trim()).ToString("yyyyMMdd") + "')) ");
        }
        #endregion

        string wh = null;
        string whEIP = null;

        if (AllWHERE.Count > 0)
        {
            wh = string.Join(" OR ", AllWHERE.ToArray());
            sql.Append(" AND " + wh);
        }

        //判斷使用者是否選擇了條件
        if (WHERE.Count > 0)
        {
            wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" AND " + wh);
        }


        if (EIPWHERE.Count > 0)
        {
            whEIP = string.Join(" AND ", EIPWHERE.ToArray());
            EIPSql.Append(" AND " + whEIP);
        }


        Session["SearchIndex"] = sql.ToString() + " GROUP BY  a.HID, a.HCourseID, c.HSCFCName, b.HSCFCName, a.HTopicName, a.HPinTop, a.HContent, a.HFile1 ,e.HSystemName, e.HArea,e.HPeriod,e.HUserName, e.HImg, a.HCreateDT, a.HModifyDT ORDER BY a.HPinTop DESC, a.HCreateDT DESC";


        Session["SearchIndexEIP"] = EIPSql.ToString() + " ORDER BY l_no DESC";


        Response.Redirect("HSCIndex.aspx");
    }
    #endregion

    #region 取消進階搜尋功能
    //MA20240724_取消進階搜尋功能
    protected void Btn_SearchCancel_Click(object sender, EventArgs e)
    {
        SqlDataSource SDS_HSCTopic = ((SqlDataSource)ContentPlaceHolder1.FindControl("SDS_HSCTopic")) == null ? new SqlDataSource() : ((SqlDataSource)ContentPlaceHolder1.FindControl("SDS_HSCTopic"));
        Repeater Rpt_HSCTopic = ((Repeater)ContentPlaceHolder1.FindControl("Rpt_HSCTopic")) == null ? new Repeater() : ((Repeater)ContentPlaceHolder1.FindControl("Rpt_HSCTopic"));
        SDS_HSCTopic.DataBind();
        Rpt_HSCTopic.DataBind();

        TB_SAKeyWord.Text = null;
        CBL_SAllRange.SelectedValue = null;
        LBox_SHHashTag.SelectedValue = null;
        TB_SHCreate.Text = null;
        TB_SHCreateDT.Text = null;
        Session["SearchType"] = null;

    }
    #endregion

    #region 最新主題
    //MA20240724_最新主題：顯示當天日期的主題內容
    protected void LBtn_NewSearch_Click(object sender, EventArgs e)
    {
        Session["SearchType"] = "1"; //0:進階搜尋 / 1:最新主題 / 2:熱門主題

        Session["SearchIndex"] = @"SELECT 
    a.HID,
    a.HCourseID,
    c.HSCFCName AS HSCForumClassB,
    b.HSCFCName AS HSCForumClassC,
    a.HTopicName,
    a.HPinTop,
    a.HContent,
    a.HFile1,
    (ISNULL(e.HSystemName, '無體系') + ' ' + e.HArea + '/' + e.HPeriod + ' ' + e.HUserName) AS UserName,
    IIF(e.HID IS NULL, '0', e.HID) AS HMemberID,
    e.HImg,
    a.HCreateDT,
    a.HModifyDT,
    h.HIRestriction
FROM HSCTopic AS a
LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID = b.HID
LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster = c.HID
LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID = d.HSCForumClassID
LEFT JOIN MemberList AS e ON a.HCreate = e.HID
OUTER APPLY (
    SELECT TOP 1 LTRIM(RTRIM(x.n.value('.', 'varchar(50)'))) AS CourseIDStr
    FROM (SELECT CAST('<i>' + REPLACE(ISNULL(a.HCourseID,''), ',', '</i><i>') + '</i>' AS xml) AS xdoc) AS t
    CROSS APPLY t.xdoc.nodes('/i') AS x(n)
    WHERE LTRIM(RTRIM(x.n.value('.', 'varchar(50)'))) <> ''
) ss
LEFT JOIN HCourse AS h 
    ON h.HID = CASE WHEN ISNUMERIC(ss.CourseIDStr) = 1 THEN CAST(ss.CourseIDStr AS int) ELSE NULL END
WHERE 
    a.HStatus = 1
    AND a.HTopicName LIKE '%' + CONVERT(varchar(6), GETDATE(), 12) + '%'
	GROUP BY a.HID,
    a.HCourseID,
    c.HSCFCName,
    b.HSCFCName,
    a.HTopicName,
    a.HPinTop,
    a.HContent,
    a.HFile1,
   e.HSystemName,e.HArea, e.HPeriod, e.HUserName,
   e.HID,
    e.HImg,
    a.HCreateDT,
    a.HModifyDT,
    h.HIRestriction
ORDER BY 
    IIF(a.HPinTop IS NULL, 0, a.HPinTop) DESC,
    IIF(a.HModifyDT IS NOT NULL, a.HModifyDT, a.HCreateDT) DESC;";


        Session["SearchIndexEIP"] = "SELECT  l_id AS HID, '' AS HCourseID, 'EIP' AS HSCForumClassB, 'EIP' AS HSCForumClassC,  l_title AS HTopicName, l_Content AS HContent, l_recorddate AS HCreateDT,  l_cname AS UserName, '' AS HIRestriction FROM heip.laoshi WHERE l_status=2 AND l_category=0 AND l_title NOT LIKE '%成長紀錄%' AND l_recorddate > DATE_FORMAT(CURDATE(), '%Y%m%d%H%i%S') AND l_recorddate < DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 15 HOUR), '%Y%m%d%H%i%s')";

        Response.Redirect("HSCIndex.aspx");
    }
    #endregion

    #region 熱門主題
    //MA20240725_熱門主題：回應數最多的主題
    //AE20241224_加入時間、數量條件:近7天內，回應數最多的前15筆
    protected void LBtn_HotSearch_Click(object sender, EventArgs e)
    {
        Session["SearchType"] = "2"; //0:進階搜尋 / 1:最新主題 / 2:熱門主題



        Session["SearchIndex"] = "SELECT TOP(15) a.HID, a.HCourseID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  a.HTopicName, a.HPinTop, a.HContent, a.HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID  LEFT JOIN MemberList AS e ON a.HCreate = e.HID LEFT JOIN HSCTMsg AS f ON (a.HID=f.HSCTopicID AND f.HStatus=1) LEFT JOIN HSCTMsgResponse AS g ON (f.HID=g.HSCTMsgID AND g.HStatus=1)WHERE  a.HStatus=1 AND ((f.HCreateDT >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE)) AND f.HCreateDT < CAST(GETDATE() AS DATE)) OR (g.HCreateDT >= DATEADD(DAY, -7, CAST(GETDATE() AS DATE)) AND g.HCreateDT < CAST(GETDATE() AS DATE))) GROUP BY a.HID, a.HCourseID,  c.HSCFCName,  b.HSCFCName, a.HTopicName, a.HPinTop, a.HContent, a.HFile1, e.HSystemName, e.HArea, e.HPeriod, e.HUserName, e.HImg, a.HCreateDT, a.HModifyDT ORDER BY (COUNT(f.HID)+COUNT(g.HID)) DESC ";


        Session["SearchIndexEIP"] = "SELECT   l_id AS HID,  '' AS HCourseID,  '大愛光老師專欄' AS HSCForumClassB,  'EIP' AS HSCForumClassC,  a.l_title AS HTopicName,  '0' AS HPinTop,  a.l_Content AS HContent,  '' AS HFile1,  l_cname AS UserName,  '' AS HImg,  l_recorddate AS HCreateDT,  a.l_udate AS HModifyDT,  'EIP' AS source,  a.l_id AS EIPID,  '' AS HIRestriction,  COALESCE(rc.Num, 0) AS Num  FROM heip.laoshi AS a  LEFT JOIN (  SELECT lr_lid, COUNT(*) AS Num  FROM heip.laoshireply  WHERE lr_Status < 9    AND (lr_parent = '' OR lr_parent IS NULL)   GROUP BY lr_lid  ) AS rc ON rc.lr_lid = a.l_id  WHERE a.l_status = 2  AND a.l_category = 0  AND a.l_recorddate > '20250913000000'  AND a.l_recorddate < DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 15 HOUR), '%Y%m%d%H%i%s') AND COALESCE(rc.Num, 0) > 0  ORDER BY COALESCE(rc.Num, 0) DESC, a.l_recorddate DESC LIMIT 15;";

        Response.Redirect("HSCIndex.aspx");

    }
    #endregion

    #region 登出功能
    protected void LBtn_LogOut_Click(object sender, EventArgs e)
    {
        Response.Write("<script>window.location.href='HLogout.aspx';</script>");
    }
    #endregion

    #region 熱門標籤搜尋
    protected void LBtn_HSCHotHashTag_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_HSCHotHashTag = sender as LinkButton;
        string gHSCHotHashTag_CA = LBtn_HSCHotHashTag.CommandArgument;

        string Sql = "SELECT a.HID, a.HCourseID, c.HSCFCName AS HSCForumClassB, b.HSCFCName AS HSCForumClassC, a.HTopicName, a.HPinTop, a.HContent, a.HFile1 , (e.HSystemName + ' ' + e.HArea + '/' + e.HPeriod + ' ' + e.HUserName) AS UserName, e.HImg, a.HCreateDT, a.HModifyDT, d.HCTemplateID FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID = b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster = c.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID = d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus = 1 AND ((a.HHashTag LIKE '%" + gHSCHotHashTag_CA + "%')) ORDER BY a.HPinTop DESC, a.HCreateDT DESC";

        Session["SearchIndex"] = Sql.ToString();

        Response.Redirect("HSCIndex.aspx");

    }
    #endregion

    #region 全部已讀
    protected void LBtn_ReadAll_Click(object sender, EventArgs e)
    {
        SQLdatabase.ExecuteNonQuery("UPDATE HNotification SET HReadStatus = '1' WHERE HMemberID='" + LB_HUserHID.Text + "'");

        SDS_HNotification.SelectCommand = "SELECT TOP(10) a.HID, a.HMemberID, a.HActorMemberID,  (b.HArea+'/'+b.HPeriod +' '+ b.HUserName) AS HActorMemberName,a.HNotifyType, a.HTargetID, a.HTableName, a.HReadStatus, a.HStatus, a.HCreate, a.HCreateDT FROM HNotification AS a JOIN MemberList AS b ON a.HActorMemberID = b.HID WHERE a.HStatus = 1 AND a.HMemberID ='" + LB_HUserHID.Text + "' AND a.HNotifyType = '1' ORDER BY a.HCreateDT DESC ";
        Rpt_HNotification.DataBind();


        //重新計算數量
        NotifyCounts();

    }
    #endregion

    #region 通知資料繫結
    protected void Rpt_HNotification_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

       


        //判斷已讀未讀
        if (gDRV["HReadStatus"].ToString() =="0")
        {
            ((Label)e.Item.FindControl("LB_Alert")).Visible = true;
        }



        //通知類型(1:回應、2:心情(讚)、3:心情(愛心)、4:心情(微笑)、5:分享、6:提問、7:其他(刪除/隱藏))
        if (gDRV["HNotifyType"].ToString() == "1")
        {
            ((Label)e.Item.FindControl("LB_HAction")).Text = "回應您的主題/留言：";
        }
        else if (gDRV["HNotifyType"].ToString() == "2")
        {
            ((Label)e.Item.FindControl("LB_HAction")).Text = "說你的主題/留言讚：";
        }
        else if (gDRV["HNotifyType"].ToString() == "3")
        {
            ((Label)e.Item.FindControl("LB_HAction")).Text = "給你的主題/留言愛心：";

        }
        else if (gDRV["HNotifyType"].ToString() == "4")
        {
            ((Label)e.Item.FindControl("LB_HAction")).Text = "給你的留言微笑：";
        }
        else if (gDRV["HNotifyType"].ToString() == "5")
        {
            ((Label)e.Item.FindControl("LB_HAction")).Text = "分享您的留言：";
        }
        else if (gDRV["HNotifyType"].ToString() == "6")
        {
            ((Label)e.Item.FindControl("LB_HAction")).Text = "對以下主題提出提問：";
        }


        string strSql = "";
        string strType = ""; //1=HSCTopic、2=HSCGRecord
        if (gDRV["HTableName"].ToString() == "HSCTopic")
        {
            strSql = "SELECT HID, HTopicName FROM " + gDRV["HTableName"].ToString() + " WHERE HStatus=1 AND HID='" + gDRV["HTargetID"].ToString() + "'";
            strType = "1";
        }
        else if (gDRV["HTableName"].ToString() == "HSCTMsg")
        {
            strSql = "SELECT b.HID, b.HTopicName FROM " + gDRV["HTableName"].ToString() + " AS a JOIN HSCTopic AS b ON a.HSCTopicID=b.HID WHERE  a.HStatus=1 AND  a.HID='" + gDRV["HTargetID"].ToString() + "'";
            strType = "1";
        }
        else if (gDRV["HTableName"].ToString() == "HSCTMsgResponse")
        {
            strSql = "SELECT b.HSCTopicID AS HID,b.HContent FROM " + gDRV["HTableName"].ToString() + " AS a JOIN HSCTMsg AS b ON a.HSCTMsgID=b.HID WHERE a.HStatus=1 AND a.HID='" + gDRV["HTargetID"].ToString() + "'";
            strType = "1";
        }
        else if (gDRV["HTableName"].ToString() == "HSCTMsg_Mood")
        {
            strSql = "SELECT b.HSCTopicID AS HID, b.HContent FROM " + gDRV["HTableName"].ToString() + " AS a JOIN HSCTMsg AS b ON a.HSCTMsgID=b.HID WHERE  a.HStatus=1 AND a.HID='" + gDRV["HTargetID"].ToString() + "'";
            strType = "1";
        }
        else if (gDRV["HTableName"].ToString() == "HSCGRMsg_Mood")
        {
            strSql = "SELECT LEFT(b.HCreateDT,10) AS Date, b.HTopicName FROM " + gDRV["HTableName"].ToString() + " AS a JOIN HSCGRMsg AS b ON a.HSCGRMsgID=b.HID WHERE a.HStatus=1 AND a.HID='" + gDRV["HTargetID"].ToString() + "'";
            strType = "2";
        }
        else if (gDRV["HTableName"].ToString() == "HSCGRMsgResponse")
        {
            strSql = "SELECT LEFT(b.HCreateDT,10) AS Date, b.HTopicName FROM " + gDRV["HTableName"].ToString() + " AS a JOIN HSCGRMsg AS b ON a.HSCGRMsgID=b.HID WHERE a.HStatus=1 AND a.HID='" + gDRV["HTargetID"].ToString() + "'";
            strType = "2";
        }

        SqlDataReader dr = SQLdatabase.ExecuteReader(strSql);
        if (dr.Read())
        {
            ((Label)e.Item.FindControl("LB_HContent")).Text = gDRV["HTableName"].ToString() == "HSCTMsg_Mood" || gDRV["HTableName"].ToString() == "HSCTMsgResponse" ? dr["HContent"].ToString() : dr["HTopicName"].ToString();

            if (strType == "1")
            {
                ((HyperLink)e.Item.FindControl("HL_HLink")).NavigateUrl = "HSCTopicDetail.aspx?TID=" + dr["HID"].ToString();
            }
            else if (strType == "2")
            {
                if (!string.IsNullOrEmpty(dr["Date"].ToString()))
                {
                    SqlDataReader QueryHSCGRecordID = SQLdatabase.ExecuteReader("SELECT HID FROM HSCGRecord WHERE HCreateDT LIKE N'%"+ dr["Date"].ToString() + "%'");
                    if (QueryHSCGRecordID.Read())
                    {
                       ((HyperLink)e.Item.FindControl("HL_HLink")).NavigateUrl = "HSCGRecordDetail.aspx?TID=" + QueryHSCGRecordID["HID"].ToString();
                    }
                    QueryHSCGRecordID.Close();
                }
            }
            
        }
        dr.Close();


    }
    #endregion

    #region 通知的menu切換
    protected void LBtn_Tab_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;

        LB_NavTab.Text = btn.TabIndex.ToString();
        //LB_NavTab.Text = "1";
        Cartlist.Attributes.Add("class", "dropdown-menu cart-dropdown-menu show");
        ScriptManager.RegisterStartupScript(this, GetType(), "js", "<script>event.stopPropagation();</script>", false);//執行js

        if (btn.TabIndex == 1)
        {
            LBtn_Reply.CssClass = "nav-link active show";
            LBtn_Mood.CssClass = "nav-link";
            LBtn_QA.CssClass = "nav-link";
            LBtn_Share.CssClass = "nav-link";
            LBtn_Others.CssClass = "nav-link";

            SDS_HNotification.SelectCommand = "SELECT TOP(10) a.HID, a.HMemberID, a.HActorMemberID,  (b.HArea+'/'+b.HPeriod +' '+ b.HUserName) AS HActorMemberName,a.HNotifyType, a.HTargetID, a.HTableName, a.HReadStatus, a.HStatus, a.HCreate, a.HCreateDT FROM HNotification AS a JOIN MemberList AS b ON a.HActorMemberID = b.HID WHERE a.HStatus = 1 AND a.HMemberID ='" + LB_HUserHID.Text + "' AND a.HNotifyType = '1'  ORDER BY a.HCreateDT DESC";
            //Rpt_HNotification.DataBind();

            LB_NavTab.Text = "1";

        }
        else if (btn.TabIndex == 2)
        {
            LBtn_Reply.CssClass = "nav-link";
            LBtn_Mood.CssClass = "nav-link active show";
            LBtn_QA.CssClass = "nav-link";
            LBtn_Share.CssClass = "nav-link";
            LBtn_Others.CssClass = "nav-link";

            SDS_HNotification.SelectCommand = "SELECT TOP(10) a.HID, a.HMemberID, a.HActorMemberID,  (b.HArea+'/'+b.HPeriod +' '+ b.HUserName) AS HActorMemberName,a.HNotifyType, a.HTargetID, a.HTableName, a.HReadStatus, a.HStatus, a.HCreate, a.HCreateDT FROM HNotification AS a JOIN MemberList AS b ON a.HActorMemberID = b.HID WHERE a.HStatus = 1 AND a.HMemberID ='" + LB_HUserHID.Text + "' AND (a.HNotifyType = '2' OR a.HNotifyType = '3' OR a.HNotifyType = '4')  ORDER BY a.HCreateDT DESC";
            //Rpt_HNotification.DataBind();

            LB_NavTab.Text = "2";
        }
        else if (btn.TabIndex == 3)  //提問
        {
            LBtn_Reply.CssClass = "nav-link";
            LBtn_Mood.CssClass = "nav-link";
            LBtn_QA.CssClass = "nav-link active show";
            LBtn_Share.CssClass = "nav-link";
            LBtn_Others.CssClass = "nav-link";

            SDS_HNotification.SelectCommand = "SELECT TOP(10) a.HID, a.HMemberID, a.HActorMemberID,  (b.HArea+'/'+b.HPeriod +' '+ b.HUserName) AS HActorMemberName,a.HNotifyType, a.HTargetID, a.HTableName, a.HReadStatus, a.HStatus, a.HCreate, a.HCreateDT FROM HNotification AS a JOIN MemberList AS b ON a.HActorMemberID = b.HID WHERE a.HStatus = 1 AND a.HMemberID ='" + LB_HUserHID.Text + "' AND a.HNotifyType = '6'  ORDER BY a.HCreateDT DESC";
            //Rpt_HNotification.DataBind();

            LB_NavTab.Text = "3";
        }
        else if (btn.TabIndex == 4) //分享
        {
            LBtn_Reply.CssClass = "nav-link";
            LBtn_Mood.CssClass = "nav-link";
            LBtn_QA.CssClass = "nav-link";
            LBtn_Share.CssClass = "nav-link active show";
            LBtn_Others.CssClass = "nav-link";

            SDS_HNotification.SelectCommand = "SELECT TOP(10) a.HID, a.HMemberID, a.HActorMemberID,  (b.HArea+'/'+b.HPeriod +' '+ b.HUserName) AS HActorMemberName,a.HNotifyType, a.HTargetID, a.HTableName, a.HReadStatus, a.HStatus, a.HCreate, a.HCreateDT FROM HNotification AS a JOIN MemberList AS b ON a.HActorMemberID = b.HID WHERE a.HStatus = 1 AND a.HMemberID ='" + LB_HUserHID.Text + "' AND a.HNotifyType = '5'  ORDER BY a.HCreateDT DESC";
            //Rpt_HNotification.DataBind();

            LB_NavTab.Text = "4";
        }
        else if (btn.TabIndex == 5) //其他
        {
            LBtn_Reply.CssClass = "nav-link";
            LBtn_Mood.CssClass = "nav-link";
            LBtn_QA.CssClass = "nav-link";
            LBtn_Share.CssClass = "nav-link";
            LBtn_Others.CssClass = "nav-link active show";

            SDS_HNotification.SelectCommand = "SELECT TOP(10) a.HID, a.HMemberID, a.HActorMemberID,  (b.HArea+'/'+b.HPeriod +' '+ b.HUserName) AS HActorMemberName,a.HNotifyType, a.HTargetID, a.HTableName, a.HReadStatus, a.HStatus, a.HCreate, a.HCreateDT FROM HNotification AS a JOIN MemberList AS b ON a.HActorMemberID = b.HID WHERE a.HStatus = 1 AND a.HMemberID ='" + LB_HUserHID.Text + "' AND a.HNotifyType = '7'  ORDER BY a.HCreateDT DESC";
            //Rpt_HNotification.DataBind();

            LB_NavTab.Text = "5";
        }

        //統計數量
        NotifyCounts();


        Session["computer"] = "1";


    }
    #endregion


    #region 當各頁Click觸發後，masterpage最後執行的事件
    protected void Page_PreRender(object sender, EventArgs e)
    {
        NotifyCounts();

        if (Session["Computer"] !=null)
        {
            if (Session["Computer"].ToString() == "1")
            {
                Li1.Attributes.Add("class", "nav-item dropdown show");
            }
           
        }



        if (Session["HAccount"] == null && Session["HUserName"] == null && Session["HUserHID"] == null)
        {

            //抓程式檔名→routing的狀況下
            System.Web.UI.Page oPage = (System.Web.UI.Page)System.Web.HttpContext.Current.CurrentHandler;
            string gPage = oPage.Request.CurrentExecutionFilePath;
            Response.Write("<script>alert('您尚未登入哦~系統將引導您先登入，謝謝~!');window.location.href='HLogin.aspx?Url=" + gPage.Trim('/') + "';</script>");

        }


    }
    #endregion



    #region 計算通知數量
    public void NotifyCounts()
    {
        //每則通知數量計算
        //通知類型(1:回應、2:心情(讚)、3:心情(愛心)、4:心情(微笑)、5:分享、6:提問、7:其他(刪除/隱藏))
        SqlDataReader QueryNotifyNum = SQLdatabase.ExecuteReader("SELECT a.HMemberID, COUNT(CASE WHEN a.HNotifyType = 1 THEN 0 END) AS ReplyNum,COUNT(CASE WHEN a.HNotifyType =2 THEN 0 END) AS ThumbsNum,COUNT(CASE WHEN a.HNotifyType =3 THEN 0 END) AS HeartNum,COUNT(CASE WHEN a.HNotifyType =4 THEN 0 END) AS SmileNum,COUNT(CASE WHEN a.HNotifyType = 5 THEN 0 END) AS ShareNum,COUNT(CASE WHEN a.HNotifyType = 6 THEN 0 END) AS QANum,COUNT(CASE WHEN a.HNotifyType = 7 THEN 0 END) AS OtherNum, a.HNotifyType FROM HNotification AS a JOIN MemberList AS b ON a.HActorMemberID = b.HID WHERE a.HStatus = 1 AND a.HMemberID ='" + LB_HUserHID.Text + "' AND  a.HReadStatus ='0' GROUP BY a.HMemberID, a.HNotifyType, a.HTargetID");

        int ReplyNum = 0;
        int MoodNum = 0;
        int ShareNum = 0;
        int QANum = 0;
        int OtherNum = 0;

        while (QueryNotifyNum.Read())
        {
            ReplyNum += Convert.ToInt32(QueryNotifyNum["ReplyNum"].ToString());
            MoodNum += Convert.ToInt32(QueryNotifyNum["ThumbsNum"].ToString()) + Convert.ToInt32(QueryNotifyNum["HeartNum"].ToString()) + Convert.ToInt32(QueryNotifyNum["SmileNum"].ToString());

            QANum += Convert.ToInt32(QueryNotifyNum["QANum"].ToString());
            ShareNum += Convert.ToInt32(QueryNotifyNum["ShareNum"].ToString());
            OtherNum += Convert.ToInt32(QueryNotifyNum["OtherNum"].ToString());
        }
        QueryNotifyNum.Close();

        LB_ReplyNum.Text = ReplyNum.ToString();
        LB_MoodNum.Text = MoodNum.ToString();
        LB_QANum.Text = QANum.ToString();
        LB_ShareNum.Text = ShareNum.ToString();
        LB_OthersNum.Text = OtherNum.ToString();

        LBtn_Reply.Text = "回應(" + LB_ReplyNum.Text + ")";
        LBtn_Mood.Text = "心情(" + LB_MoodNum.Text + ")";
        LBtn_QA.Text = "提問(" + LB_QANum.Text + ")";
        LBtn_Share.Text = "分享(" + LB_ShareNum.Text + ")";
        LBtn_Others.Text = "其他(" + LB_OthersNum.Text + ")";

        LB_NotifyNum.Text = (ReplyNum+ MoodNum+ QANum+ ShareNum+ OtherNum).ToString();
        LB_NotifySum.Text = LB_NotifyNum.Text;


        if (LB_NotifyNum.Text == "0")
        {
            LBtn_ReadAll.Enabled = false;
            LBtn_ReadAll.Style.Add("color", "#ccc");
        }
    }
    #endregion



    protected void LBtn_BackIndex_Click(object sender, EventArgs e)
    {
        Session["SearchType"] = null;
        Response.Redirect("HSCIndex.aspx");
    }

    #region 關閉手機版menu
    protected void LBtn_CloseMobilemenu_Click(object sender, EventArgs e)
    {
        Div_MobileMenu.Attributes.Add("class", "mobilemenu navbar-collapse collapse");

    }
    #endregion

    #region 關閉手機版通知
    protected void LBtn_CloseNotify_Click(object sender, EventArgs e)
    {
        Cartlist.Attributes.Add("class","dropdown-menu cart-dropdown-menu");

    }
    #endregion
}
