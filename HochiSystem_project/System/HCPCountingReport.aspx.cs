using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class System_HCPCountingReport : System.Web.UI.Page
{
    //EA20240408_新增開課明細與人數統計表

    #region 分頁copy-1
    private int PageMax = 15;   //分頁最大顯示數量
    private int LastPage = 10;  //分頁數量
    #endregion

    protected void Page_Error(object sender, EventArgs e)
    {
        Exception ex = Server.GetLastError();
        if (ex is HttpRequestValidationException)
        {
            string js = "<script type='text/javascript'>" +
                        "alert('請勿嘗試輸入具有危險性的字元!');" +
                        "history.go(-1);" +
                        "</script>";
            Server.ClearError(); // 如果不ClearError()這個異常會繼續傳到Application_Error()
            Response.Write(js);
            //Response.Write("<script language=javascript>alert('字符串含有非法字符！')</script>");
            //Response.Write("<script language=javascript>window.location.href='HCourse_Edit.aspx';</script>");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        #region 分頁copy-2
        if (!IsPostBack)
        {
            ViewState["Search"] = "";
        }

        if (!IsPostBack)
        {
            //SDS_HC.SelectCommand = "SELECT a.HCourseName, a.HDateRange, a.HTeam, a.HTeacherName FROM HCourse as a LEFT JOIN HPlace as b on a.HOCPlace=b.HID where a.HStatus='1' AND a.HVerifyStatus='2' AND a.HSave='1' group by a.HCourseName, a.HDateRange, a.HTeam, a.HTeacherName ORDER BY a.HDateRange DESC";


            //有按搜尋才撈
            //SDS_HC.SelectCommand = "SELECT ROW_NUMBER() OVER(ORDER BY m.HDateRange DESC) AS Row#,  m.HBudgetType, IIF(HPMethod='1','基金會','文化事業') AS PMethod, HTemplateName, e.HCourseName AS CourseType, M.HCourseName ,HDateRange, LEFT(HDateRange,10) AS StartDate,IIF(LEFT(m.TeacherName, len(m.TeacherName) - 1) = '周瑞宏', '大愛光老師', LEFT(m.TeacherName, len(m.TeacherName) - 1)) AS TeacherName FROM (SELECT b.HID, HCourseName, HTeacherName, b.HStatus, HDateRange, HOCPlace, HCTemplateID, b.HType,   (SELECT DISTINCT(cast(HMember.HUserName AS NVARCHAR) + ',')  FROM HCourse CROSS APPLY SPLIT(HTeacherName, ',')   INNER JOIN HTeacher ON value = HTeacher.HID INNER JOIN HMember ON HTeacher.HMemberID = HMember.HID WHERE HCourse.HID = b.HID FOR XML PATH('')) AS TeacherName,  b.HBudgetType FROM HCourse AS b WHERE b.HSave = 1 ) M JOIN HPlace AS c ON M.HOCPlace = c.HID JOIN HCourse_T AS d ON M.HCTemplateID = d.HID JOIN HCourse_Class AS e ON m.HType = e.HID GROUP BY HPMethod,HTemplateName,e.HCourseName,M.HCourseName ,HDateRange, m.TeacherName, m.HBudgetType";

       

            //// 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            ////Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HC.SelectCommand, PageMax, LastPage, false, Rpt_HC);
            //ViewState["Search"] = SDS_HC.SelectCommand;
            //Rpt_HC.DataBind();

        }
        else
        {
            SDS_HC.SelectCommand = ViewState["Search"].ToString();
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Rpt_HC.DataBind();
            //Pg_Paging.FrontPagingLoad("HochiSystemConnection", ViewState["Search"].ToString(), PageMax, LastPage, false, Rpt_HC);

        }
        #endregion



        //SqlConnection dbConn = default(SqlConnection);
        //SqlCommand dbCmd = default(SqlCommand);
        //string strDBConn = null;
        //strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        //dbConn = new SqlConnection(strDBConn);
        //dbConn.Open();
        //string strSelHPN = "select HID, HPlaceName, HStatus from HPlace order by HPlaceName";
        ////Response.Write(strSQL);
        //dbCmd = new SqlCommand(strSelHPN, dbConn);
        //SqlDataReader MyQueryHPN = dbCmd.ExecuteReader();
        //DDL_HOCPlace.Items.Add(new ListItem("-請選擇上課地點-", "0"));
        //while (MyQueryHPN.Read())
        //{
        //    if (MyQueryHPN["HStatus"].ToString() == "0")
        //    {
        //        DDL_HOCPlace.Items.Add(new ListItem(MyQueryHPN["HPlaceName"].ToString() + "(停用)", (MyQueryHPN["HID"].ToString())));
        //    }
        //    else
        //    {
        //        DDL_HOCPlace.Items.Add(new ListItem(MyQueryHPN["HPlaceName"].ToString(), (MyQueryHPN["HID"].ToString())));
        //    }
        //}
        //MyQueryHPN.Close();
        //dbConn.Close();
    }

    #region 搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        string[] gHDateRangeArray = TB_SearchDate.Text == "" ? "2020/01/01-3000/12/31".Split('-') : TB_SearchDate.Text.Split('-');
        //string gHOCPlace = DDL_HOCPlace.SelectedValue=="0"?"like '%'":"='"+ DDL_HOCPlace.SelectedValue + "'";//地點沒輸入則全找

        string gHPMethod = DDL_HPMethod.SelectedValue == "0" ? "!='0'" : "='" + DDL_HPMethod.SelectedValue + "'";//繳費帳戶沒輸入則全不找


        SDS_HC.SelectCommand = "SELECT ROW_NUMBER() OVER(ORDER BY m.HDateRange DESC) AS Row#,  m.HBudgetType, IIF(HPMethod='1','基金會','文化事業') AS PMethod, HTemplateName, e.HCourseName AS CourseType, M.HCourseName ,HDateRange, LEFT(HDateRange,10) AS StartDate,IIF(LEFT(m.TeacherName, len(m.TeacherName) - 1) = '周瑞宏', '大愛光老師', LEFT(m.TeacherName, len(m.TeacherName) - 1)) AS TeacherName FROM (SELECT b.HID, HCourseName, HTeacherName, b.HStatus, HDateRange, HOCPlace, HCTemplateID, b.HType,   (SELECT DISTINCT(cast(HMember.HUserName AS NVARCHAR) + ',')  FROM HCourse CROSS APPLY SPLIT(HTeacherName, ',')   INNER JOIN HTeacher ON value = HTeacher.HID INNER JOIN HMember ON HTeacher.HMemberID = HMember.HID WHERE HCourse.HID = b.HID FOR XML PATH('')) AS TeacherName,  b.HBudgetType, b.HVerifyStatus FROM HCourse AS b WHERE b.HSave = 1 ) M JOIN HPlace AS c ON M.HOCPlace = c.HID JOIN HCourse_T AS d ON M.HCTemplateID = d.HID JOIN HCourse_Class AS e ON m.HType = e.HID WHERE m.HCourseName like N'%" + TB_Search.Text.Trim() + "%'  and ((left(HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and left(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "'and right(HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "')) and HPMethod "+ gHPMethod + " and m.HStatus='1' AND m.HVerifyStatus=2 GROUP BY HPMethod,HTemplateName,e.HCourseName,M.HCourseName ,HDateRange, m.TeacherName, m.HBudgetType, m.HVerifyStatus, m.HStatus ORDER BY HDateRange DESC";

       

        #region 分頁copy-3搜尋用
        ViewState["Search"] = SDS_HC.SelectCommand;
        //呼叫分頁 (連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 搜尋開啟, DataList控件)
        Rpt_HC.DataBind();
        //Pg_Paging.PagingLoad("HochiSystemConnection", SDS_HC.SelectCommand, PageMax, LastPage, true, Rpt_HC);
        #endregion

    }
    #endregion

    #region 取消搜尋功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_Search.Text = "";
        TB_SearchDate.Text = "";
        DDL_HPMethod.SelectedValue = "0";

        SDS_HC.SelectCommand = "SELECT ROW_NUMBER() OVER(ORDER BY m.HDateRange DESC) AS Row#,  m.HBudgetType, IIF(HPMethod='1','基金會','文化事業') AS PMethod, HTemplateName, e.HCourseName AS CourseType, M.HCourseName ,HDateRange, LEFT(HDateRange,10) AS StartDate,IIF(LEFT(m.TeacherName, len(m.TeacherName) - 1) = '周瑞宏', '大愛光老師', LEFT(m.TeacherName, len(m.TeacherName) - 1)) AS TeacherName FROM (SELECT b.HID, HCourseName, HTeacherName, b.HStatus, HDateRange, HOCPlace, HCTemplateID, b.HType,   (SELECT DISTINCT(cast(HMember.HUserName AS NVARCHAR) + ',')  FROM HCourse CROSS APPLY SPLIT(HTeacherName, ',')   INNER JOIN HTeacher ON value = HTeacher.HID INNER JOIN HMember ON HTeacher.HMemberID = HMember.HID WHERE HCourse.HID = b.HID FOR XML PATH('')) AS TeacherName,  b.HBudgetType, b.HVerifyStatus FROM HCourse AS b WHERE b.HSave = 1 ) M JOIN HPlace AS c ON M.HOCPlace = c.HID JOIN HCourse_T AS d ON M.HCTemplateID = d.HID JOIN HCourse_Class AS e ON m.HType = e.HID WHERE m.HVerifyStatus='2' AND m.HStatus=1 GROUP BY HPMethod,HTemplateName,e.HCourseName,M.HCourseName ,HDateRange, m.TeacherName, m.HBudgetType, , m.HVerifyStatus, m.HStatus ORDER BY HDateRange DESC";
        ViewState["Search"] = SDS_HC.SelectCommand;

        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
        Rpt_HC.DataBind();
        //Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HC.SelectCommand, PageMax, LastPage, true, Rpt_HC);
    }
    #endregion

    #region 課程列表繫結
    protected void Rpt_HC_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;


        #region 計算上課天數
        //AA250116_新增計算上課天數
        if (gDRV["HDateRange"].ToString().IndexOf("-") >= 0)  //連續課程
        {
            string[] gCourseDate = gDRV["HDateRange"].ToString().Split('-');
            DateTime gLastDate = Convert.ToDateTime(gCourseDate[1]);
            DateTime gFirstDate = Convert.ToDateTime(gCourseDate[0]);
            TimeSpan gdaysdiff = new TimeSpan(gLastDate.Ticks - gFirstDate.Ticks);

            ((Label)e.Item.FindControl("LB_Days")).Text = gdaysdiff.Days.ToString();
        }
        else if (gDRV["HDateRange"].ToString().IndexOf(",") >= 0)  //非連續課程日期
        {
            string[] gCourseDate = gDRV["HDateRange"].ToString().Split(',');
            ((Label)e.Item.FindControl("LB_Days")).Text = gCourseDate.Length.ToString();
        }
        else  //單一日期
        {
            ((Label)e.Item.FindControl("LB_Days")).Text = "1";
        }
        #endregion

        #region 報名人數&男生人數&女生人數
        //AE250116_新增男女生人數計算
        //SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT COUNT(a.HUserName) AS  HApplyNum, SUM(CASE WHEN a.HSex = '1' THEN 1 ELSE 0 END) AS MaleNum, SUM(CASE WHEN a.HSex = '2' THEN 1 ELSE 0 END) AS FemaleNum FROM MemberList as a join OrderList_Merge as b on a.HID = b.HMemberID where b.HCourseName = '" + gDRV["HCourseName"] + "' AND b.HDateRange = '" + gDRV["HDateRange"] + "' and b.HStatus = '1' and b.HItemStatus = '1'");
        //if (dr.Read())
        //{
        //    ((Label)e.Item.FindControl("LB_HApplyNum")).Text = dr["HApplyNum"].ToString();
        //    ((Label)e.Item.FindControl("LB_MaleNum")).Text = !string.IsNullOrEmpty(dr["MaleNum"].ToString()) ? dr["MaleNum"].ToString() : "0";
        //    ((Label)e.Item.FindControl("LB_FemaleNum")).Text = !string.IsNullOrEmpty(dr["FemaleNum"].ToString()) ? dr["FemaleNum"].ToString() : "0";
        //}
        //dr.Close();


        System.Data.DataTable dtApplyNum = SQLdatabase.ExecuteDataTable("SELECT COUNT(a.HUserName) AS  HApplyNum, SUM(CASE WHEN a.HSex = '1' THEN 1 ELSE 0 END) AS MaleNum, SUM(CASE WHEN a.HSex = '2' THEN 1 ELSE 0 END) AS FemaleNum FROM MemberList as a join OrderList_Merge as b on a.HID = b.HMemberID where b.HCourseName = '" + gDRV["HCourseName"] + "' AND b.HDateRange = '" + gDRV["HDateRange"] + "' and b.HStatus = '1' and b.HItemStatus = '1'");
        foreach (DataRow ApplyNumrow in dtApplyNum.Rows)
        {
            ((Label)e.Item.FindControl("LB_HApplyNum")).Text = ApplyNumrow["HApplyNum"].ToString();
            ((Label)e.Item.FindControl("LB_MaleNum")).Text = !string.IsNullOrEmpty(ApplyNumrow["MaleNum"].ToString()) ? ApplyNumrow["MaleNum"].ToString() : "0";
            ((Label)e.Item.FindControl("LB_FemaleNum")).Text = !string.IsNullOrEmpty(ApplyNumrow["FemaleNum"].ToString()) ? ApplyNumrow["FemaleNum"].ToString() : "0";
        }

        dtApplyNum.Clear();
        dtApplyNum.Dispose();
        #endregion

        #region 參班人數、參班兼護持人數、純護持人數
        //參班人數：HAttend=1、2、3、4
        //參班兼護持人數：HAttend=6
        //純護持人數：HAttend=5
        //SqlDataReader QueryHMCBCA = SQLdatabase.ExecuteReader("select a.HAreaID, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, a.HSex, isnull(b.HAttend,0) as HAttend, b.HOCPlace, b.HLDate, b.HRemark from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID  left join HArea as d on a.HAreaID=d.HID where b.HCourseName='" + gDRV["HCourseName"] + "' and b.HDateRange='" + gDRV["HDateRange"] + "' and b.HStatus='1' AND b.HItemStatus='1' order by b.HCourseID");

        //while (QueryHMCBCA.Read())
        //{
        //    if (QueryHMCBCA["HAttend"].ToString() == "1" || QueryHMCBCA["HAttend"].ToString() == "2" || QueryHMCBCA["HAttend"].ToString() == "3" || QueryHMCBCA["HAttend"].ToString() == "4")
        //    {
        //        ((Label)e.Item.FindControl("LB_AttendNum")).Text = Convert.ToString(Convert.ToInt32(((Label)e.Item.FindControl("LB_AttendNum")).Text) + 1);
        //    }
        //    else if (QueryHMCBCA["HAttend"].ToString() == "5")
        //    {
        //        ((Label)e.Item.FindControl("LB_GuideNum")).Text = Convert.ToString(Convert.ToInt32(((Label)e.Item.FindControl("LB_GuideNum")).Text) + 1);
        //    }
        //    else if (QueryHMCBCA["HAttend"].ToString() == "6")
        //    {
        //        ((Label)e.Item.FindControl("LB_ProGuideNum")).Text = Convert.ToString(Convert.ToInt32(((Label)e.Item.FindControl("LB_ProGuideNum")).Text) + 1);
        //    }

        //}
        //QueryHMCBCA.Close();


        System.Data.DataTable dtHMCBCA = SQLdatabase.ExecuteDataTable("select a.HAreaID, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, a.HSex, isnull(b.HAttend,0) as HAttend, b.HOCPlace, b.HLDate, b.HRemark from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID  left join HArea as d on a.HAreaID=d.HID where b.HCourseName='" + gDRV["HCourseName"] + "' and b.HDateRange='" + gDRV["HDateRange"] + "' and b.HStatus='1' AND b.HItemStatus='1' order by b.HCourseID");
        foreach (DataRow HMCBCArow in dtHMCBCA.Rows)
        {
            if (HMCBCArow["HAttend"].ToString() == "1" || HMCBCArow["HAttend"].ToString() == "2" || HMCBCArow["HAttend"].ToString() == "3" || HMCBCArow["HAttend"].ToString() == "4")
            {
                ((Label)e.Item.FindControl("LB_AttendNum")).Text = Convert.ToString(Convert.ToInt32(((Label)e.Item.FindControl("LB_AttendNum")).Text) + 1);
            }
            else if (HMCBCArow["HAttend"].ToString() == "5")
            {
                ((Label)e.Item.FindControl("LB_GuideNum")).Text = Convert.ToString(Convert.ToInt32(((Label)e.Item.FindControl("LB_GuideNum")).Text) + 1);
            }
            else if (HMCBCArow["HAttend"].ToString() == "6")
            {
                ((Label)e.Item.FindControl("LB_ProGuideNum")).Text = Convert.ToString(Convert.ToInt32(((Label)e.Item.FindControl("LB_ProGuideNum")).Text) + 1);
            }

        }

        dtHMCBCA.Clear();
        dtHMCBCA.Dispose();


        #endregion

        #region 主班團隊
        //AE250115_註解
        //if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HTeam")).Text))
        //{
        //    string[] gHTeamID = ((Label)e.Item.FindControl("LB_HTeam")).Text.Split(',');
        //    for (int i = 0; i < gHTeamID.Length - 1; i++)
        //    {
        //        SqlDataReader MyQueryHM = SQLdatabase.ExecuteReader("select HMember.HID, HUserName,HAreaID, HPeriod, (HArea+'/'+HPeriod+' '+HUserName) as UserName, HMember.HStatus from HMember Left Join HArea On HMember.HAreaID =HArea.HID where HMember.HID= '" + gHTeamID[i].ToString() + "'");
        //        if (MyQueryHM.Read())
        //        {
        //            ((Label)e.Item.FindControl("LB_HTeamName")).Text += MyQueryHM["HUserName"].ToString() + ",";
        //            //((Label)e.Item.FindControl("LB_HTeamName")).Text += MyQueryHM["UserName"].ToString() + ",";
        //        }
        //        MyQueryHM.Close();
        //    }
        // ((Label)e.Item.FindControl("LB_HTeamName")).Text = ((Label)e.Item.FindControl("LB_HTeamName")).Text == null || ((Label)e.Item.FindControl("LB_HTeamName")).Text == "" ? "" : ((Label)e.Item.FindControl("LB_HTeamName")).Text.Substring(0, (((Label)e.Item.FindControl("LB_HTeamName")).Text.Length) - 1);
        //}

        #endregion

        #region 課程講師
        //AE250115_註解，改用SQL取
        //if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HTeacher")).Text))
        //{
        //    string[] gHTeacherNameID = ((Label)e.Item.FindControl("LB_HTeacher")).Text.Split(',');
        //    for (int i = 0; i < gHTeacherNameID.Length - 1; i++)
        //    {
        //        SqlDataReader MyQueryHM = SQLdatabase.ExecuteReader("SELECT HUserName FROM HTeacher a LEFT JOIN HMember b ON a.HMemberID=b.HID WHERE a.HID= '" + gHTeacherNameID[i].ToString() + "'");
        //        if (MyQueryHM.Read())
        //        {
        //            ((Label)e.Item.FindControl("LB_HTeacherName")).Text += MyQueryHM["HUserName"].ToString() + ",";
        //        }
        //        MyQueryHM.Close();
        //    }
        // ((Label)e.Item.FindControl("LB_HTeacherName")).Text = ((Label)e.Item.FindControl("LB_HTeacherName")).Text == null || ((Label)e.Item.FindControl("LB_HTeacherName")).Text == "" ? "" : ((Label)e.Item.FindControl("LB_HTeacherName")).Text.Substring(0, (((Label)e.Item.FindControl("LB_HTeacherName")).Text.Length) - 1);
        //}

        #endregion

        #region 上課方式的顯示判斷: 線上(只有ZOOM) / 實體 (當上課地點超過3個以上就視為實體)
        //AA250116_新增
        //SqlDataReader QueryPlaceNum = SQLdatabase.ExecuteReader("SELECT SUM(CASE WHEN HOCplace!=1 THEN 1 ELSE 0 END) AS PlaceNum FROM HCourse WHERE HCourseName='" + gDRV["HCourseName"].ToString() + "' AND HDateRange = '"+ gDRV["HDateRange"].ToString() + "' AND HStatus=1 AND HVerifyStatus=2");
        //if (QueryPlaceNum.Read())
        //{
        //    if (QueryPlaceNum["PlaceNum"].ToString() == "0")
        //    {
        //        ((Label)e.Item.FindControl("LB_HCPlace")).Text = "線上";
        //    }
        //    else
        //    {
        //        ((Label)e.Item.FindControl("LB_HCPlace")).Text = "實體";
        //    }
        //}
        //QueryPlaceNum.Close();



        System.Data.DataTable dtPlaceNum = SQLdatabase.ExecuteDataTable("SELECT SUM(CASE WHEN HOCplace!=1 THEN 1 ELSE 0 END) AS PlaceNum FROM HCourse WHERE HCourseName='" + gDRV["HCourseName"].ToString() + "' AND HDateRange = '" + gDRV["HDateRange"].ToString() + "' AND HStatus=1 AND HVerifyStatus=2");
        foreach (DataRow PlaceNumrow in dtPlaceNum.Rows)
        {
            if (PlaceNumrow["PlaceNum"].ToString() == "0")
            {
                ((Label)e.Item.FindControl("LB_HCPlace")).Text = "線上";
            }
            else
            {
                ((Label)e.Item.FindControl("LB_HCPlace")).Text = "實體";
            }

        }

        dtPlaceNum.Clear();
        dtPlaceNum.Dispose();

        #endregion

    }
    #endregion

}