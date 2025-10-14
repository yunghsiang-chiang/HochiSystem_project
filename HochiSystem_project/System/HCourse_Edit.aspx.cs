using DocumentFormat.OpenXml.Math;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Color = System.Drawing.Color;
using ListItem = System.Web.UI.WebControls.ListItem;



public partial class HCourse_Edit : System.Web.UI.Page
{

    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    #region --根目錄--
    string CourseImgRoot = "D:\\Website\\System\\HochiSystem\\uploads\\Course\\";
    string CourseImg = "~/uploads/Course/";
    string CourseFileRoot = "D:\\Website\\System\\HochiSystem\\uploads\\CourseFile\\";
    string CourseFile = "~/uploads/CourseFile/";
    #endregion

    #region 寄件資訊<正式>
    public string Sender = MailConfig.Sender;
    public string EAcount = MailConfig.Account;
    public string EPasword = MailConfig.Password;
    public string EHost = MailConfig.Host;
    public int EPort = MailConfig.Port;
    public bool EEnabledSSL = MailConfig.EnableSSL;
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

    public void DeleteSrcFolder(string Path)
    {
        //去除資料夾和子檔案的只讀屬性
        //去除資料夾的只讀屬性
        //System.IO.DirectoryInfo fileInfo = new DirectoryInfo(Path);
        //fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
        //去除檔案的只讀屬性
        System.IO.File.SetAttributes(Path, System.IO.FileAttributes.Normal);
        //判斷資料夾是否還存在
        if (Directory.Exists(Path))
        {
            foreach (string f in Directory.GetFileSystemEntries(Path))
            {
                //刪除1天前的檔案
                if (File.Exists(f) && (File.GetLastWriteTime(f) < DateTime.Now.AddDays(-1)))
                {
                    //Response.Write("檔案寫入時間1："+File.GetLastWriteTime(f) +"<br/>");
                    //如果有子檔案刪除檔案
                    File.Delete(f);
                }
                else
                {
                    //迴圈遞迴刪除子資料夾 
                    DeleteSrcFolder1(f);
                }
            }
            //刪除空資料夾
            //Directory.Delete(Path);
        }
    }
    //迴圈遞迴刪除子資料夾
    public void DeleteSrcFolder1(string Path)
    {
        //去除資料夾和子檔案的只讀屬性
        //去除資料夾的只讀屬性
        //System.IO.DirectoryInfo fileInfo = new DirectoryInfo(Path);
        //fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
        //去除檔案的只讀屬性
        System.IO.File.SetAttributes(Path, System.IO.FileAttributes.Normal);
        //判斷資料夾是否還存在
        if (Directory.Exists(Path))
        {
            foreach (string f in Directory.GetFileSystemEntries(Path))
            {
                //刪除1天前的檔案
                if (File.Exists(f) && (File.GetLastWriteTime(f) < DateTime.Now.AddDays(-1)))
                {
                    //Response.Write("檔案寫入時間2：" + File.GetLastWriteTime(f) + "<br/>");
                    //如果有子檔案刪除檔案
                    File.Delete(f);
                }
                else
                {
                    //迴圈遞迴刪除子資料夾 
                    DeleteSrcFolder1(f);
                }
            }
            //刪除空資料夾
            //Directory.Delete(Path);
        }
    }

    #region 分頁copy-1
    private int PageMax = 15;   //分頁最大顯示數量
    private int LastPage = 10;  //分頁數量
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        DataTable dtCourseDate = new DataTable("dtCourseDate");
        dtCourseDate.Columns.Add("HCBatchNum", typeof(String));
        dtCourseDate.Columns.Add("HCourseName", typeof(String));
        dtCourseDate.Columns.Add("HDateRange", typeof(String));
        dtCourseDate.Columns.Add("HDate", typeof(String));
        dtCourseDate.Columns.Add("HStatus", typeof(String));
        dtCourseDate.Columns.Add("HCreate", typeof(String));
        dtCourseDate.Columns.Add("HCreateDT", typeof(String));

        ViewState["dtCourseDate"] = dtCourseDate;
        //清除DataTable以釋放記憶體(如果不需要或有異常可先註解)
        dtCourseDate.Clear();



        

        #region DISABLE範本設定的資料
        LBox_HRSystem.Attributes.Add("disabled", "true");
        LBox_HNRequirement.Attributes.Add("disabled", "true");
        LBox_HIRestriction.Attributes.Add("disabled", "true");
        #endregion

        #region 分頁copy-2
        if (!IsPostBack)
        {
            ViewState["Search"] = "";
            RegScript();//註冊js
            RegisterAsync();
        }

        if (!IsPostBack)
        {
            //JA20240506-使用HCourse_Merge預抓3個月、HCourseCombine預抓6個月
            string gHCBatchNum = "CB" + DateTime.Now.AddMonths(-3).ToString("yyyyMMdd") + "0001";

            #region 判斷是否為最高權限者
            SqlDataReader MyQuery = SQLdatabase.ExecuteReader("SELECT HMemberID FROM HRole WHERE HID=1 AND (HMemberID LIKE '%" + "," + Session["HUserHID"] + "," + "%')");

            string Select = "SELECT MAX(HID) AS HID,HCourseName , HCBatchNum , MAX(HDateRange) AS HDateRange, MAX(HPlaceName) AS HPlaceName, MAX(TeacherName) AS TeacherName, MIN(b.Value) AS StartDate, MAX(b.value) AS EndDate , HVerifyStatus FROM HCourse_Merge Cross Apply SPLIT(Replace(HDateRange,' - ',','), ',') b ";

            StringBuilder sql = new StringBuilder(Select);
            List<string> WHERE = new List<string>();


            if (!MyQuery.Read())
            {
                WHERE.Add(" ((','+HTMemberID+',') LIKE '%," + Session["HUserHID"] + ",%' OR (','+HTeam) LIKE '%," + Session["HUserHID"] + ",%'  OR HCreate LIKE '%" + Session["HUserHID"] + "%') ");
            }

            WHERE.Add(" HCBatchNum >= '" + gHCBatchNum + "' ");

            if (WHERE.Count > 0)
            {
                string wh = string.Join(" AND ", WHERE.ToArray());
                sql.Append(" WHERE HStatus='1' AND " + wh);
            }
            else
            {
                sql.Append(" WHERE HStatus='1' ");
            }

            //WE20240323_加入依上課日期排序判斷(與前台顯示相同)//HCourse_Merge//HCourseCombine
            //WE20240507_MAX(HTeacherName)用HCourse_Merge要改成MAX(TeacherName)
            //EE20240702_ORDERBY 順序調換 yyyy->MM
            SDS_HC.SelectCommand = sql.ToString() + " GROUP BY HCourseName, HCBatchNum, HVerifyStatus ORDER BY  IIF(DATEPART(YYYY,LEFT(TRY_CONVERT(nvarchar, getdate(), 111),10))=DATEPART(YYYY,TRY_CONVERT(nvarchar, MIN(b.Value),111)), '5', '6') ASC, IIF(DATEPART(Month,LEFT(TRY_CONVERT(nvarchar, getdate(), 111),10))=DATEPART(Month,TRY_CONVERT(nvarchar, MIN(b.Value),111)),'1' ,'2')  ASC,StartDate DESC ";

            Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HC.SelectCommand, PageMax, LastPage, false, Rpt_HC);
            ViewState["Search"] = SDS_HC.SelectCommand;

            MyQuery.Close();
            #endregion



        }
        else
        {
            SDS_HC.SelectCommand = ViewState["Search"].ToString();
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", ViewState["Search"].ToString(), PageMax, LastPage, false, Rpt_HC);
        }
        #endregion

        

        if (!IsPostBack)
        {
            if (string.IsNullOrEmpty(LB_NavTab.Text))
            {
                Status(1);
                LBtn_Template.CssClass = "nav-link active show";
            }
            RegScript();//註冊js
            TB_HDateRange.Attributes.Add("readonly", "true");

            #region 刪除1天前未存檔的資料

            string gHCMFileTemp = Server.MapPath("~/uploads/CourseMaterialTemp/");
            string strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;

            Task.Run(() =>
            {
                try
                {
                    using (SqlConnection dbConn = new SqlConnection(strDBConn))
                    {
                        dbConn.Open();
                        DeleteUnsavedCourseData(dbConn); // 刪除1天前未儲存課程資料
                    }

                    DeleteSrcFolder(gHCMFileTemp); // 刪除教材暫存資料夾
                }
                catch (Exception ex)
                {
                    // log ex.ToString() 或寫入例外追蹤系統
                }
            });

            #endregion

            #region 選單選項

            #region 檢覈相關欄位
            //WE20250513_暫時註解

            //檢覈內容名稱
            BindListControls("SELECT HID, (HExamContentName+'-'+(CASE HExamType WHEN 1 THEN '筆試' WHEN 2 THEN '實作' WHEN '3' THEN '試教' ELSE '' END)) AS HExamContentName, HStatus  FROM HExamContent WHERE HStatus=1", "HExamContentName", "HID", "-請選擇-", true, DDL_HExamContentID);

            //檢覈科目名稱
            //BindListControls("SELECT HID, HExamSubject FROM HExamSubject WHERE HStatus=1", "HExamContentName", "HID", "-請選擇-", true, DDL_HExamSubject);

            //督導
            //BindListControls("SELECT HMember.HID, (HArea+'/'+HPeriod+' '+HUserName) as UserName, HMember.HStatus from HMember Left Join HArea On HMember.HAreaID =HArea.HID WHERE HMember.HStatus=1 AND HType in(7,8,9,10,11,12,13)  order by HUserName", "UserName", "HID", "-請選擇-", true, LBox_HSupervise);

            //試務人員
            //BindListControls("SELECT HMember.HID, (HArea+'/'+HPeriod+' '+HUserName) AS UserName FROM HMember AS a LEFT JOIN HArea AS b ON a.HAreaID =b.HID WHERE a.HStatus=1 ORDER BY HArea ASC ", "UserName", "HID", "-請選擇-", true, LBox_HExamStaff);

            //考卷
            BindListControls("SELECT HID, HTitle, HStatus FROM HExamPaper  WHERE HStatus=1 ORDER BY HTitle", "HTitle", "HID", "-請選擇-", true, LBox_HExamPaperID);
            #endregion

            //捐款項目
            BindListControls("SELECT HID, HDItem, HStatus FROM HDonationItem", "HDItem", "HID", "-請選擇-", true, DDL_HCCPeriodDItem);



            #endregion


            SDS_HGroupLeader.SelectCommand = "select HMember.HID, (HArea+'/'+HPeriod+' '+HUserName) as UserName from HMember Left Join HArea On HMember.HAreaID =HArea.HID WHERE HMember.HStatus='1' AND HType <>'1' order by HUserName";
        }

     
    }






    #region 搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        #region 判斷是否為最高權限者
        SqlDataReader MyQuery = SQLdatabase.ExecuteReader("SELECT HMemberID FROM HRole WHERE HID=1 AND (HMemberID LIKE '%" + "," + Session["HUserHID"] + "," + "%')");

        string Select = "SELECT MAX(HID) AS HID,HCourseName ,HCBatchNum , MAX(HDateRange) AS HDateRange, MAX(HPlaceName) AS HPlaceName, MAX(TeacherName) AS TeacherName, MIN(b.Value) AS StartDate, MAX(b.value) AS EndDate , HVerifyStatus FROM HCourse_Merge Cross Apply SPLIT(Replace(HDateRange,' - ',','), ',') b   ";

        StringBuilder sql = new StringBuilder(Select);
        List<string> WHERE = new List<string>();

        if (MyQuery.Read())
        {
            WHERE.Add(" HCourseName LIKE '%" + TB_Search.Text.Trim() + "%' ");
        }
        else
        {
            WHERE.Add(" HCourseName LIKE '%" + TB_Search.Text.Trim() + "%' AND ((','+HTMemberID+',') LIKE '%," + Session["HUserHID"] + ",%' or (','+HTeam) LIKE '%," + Session["HUserHID"] + ",%' OR HCreate LIKE '%" + Session["HUserHID"] + "%') ");
        }
        MyQuery.Close();
        #endregion

        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" WHERE HStatus='1' AND " + wh);

        }
        else
        {
            sql.Append(" WHERE HStatus='1' ");
        }


        //WE20240323_加入依上課日期排序判斷(與前台顯示相同) //HCourse_Merge //HCourseCombine
        //WE20240507_MAX(HTeacherName)用HCourse_Merge要改成MAX(TeacherName)
        //EE20240702_ORDERBY 順序調換 yyyy->MM
        SDS_HC.SelectCommand = sql.ToString() + " GROUP BY HCourseName, HCBatchNum, HVerifyStatus ORDER BY  IIF(DATEPART(YYYY,LEFT(TRY_CONVERT(nvarchar, getdate(), 111),10))=DATEPART(YYYY,TRY_CONVERT(nvarchar, MIN(b.Value),111)), '5', '6') ASC, IIF(DATEPART(Month,LEFT(TRY_CONVERT(nvarchar, getdate(), 111),10))=DATEPART(Month,TRY_CONVERT(nvarchar, MIN(b.Value),111)),'1' ,'2')  ASC,StartDate DESC ";

        #region 分頁copy-3搜尋用
        ViewState["Search"] = SDS_HC.SelectCommand;
        //呼叫分頁 (連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 搜尋開啟, DataList控件)
        Pg_Paging.PagingLoad("HochiSystemConnection", SDS_HC.SelectCommand, PageMax, LastPage, true, Rpt_HC);
        #endregion

    }
    #endregion

    #region 取消搜尋功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_Search.Text = "";

        #region 判斷是否為最高權限者
        SqlDataReader MyQuery = SQLdatabase.ExecuteReader("SELECT HMemberID FROM HRole WHERE HID=1 AND (HMemberID LIKE '%" + "," + Session["HUserHID"] + "," + "%')");
        //JA20240506-使用HCourse_Merge預抓3個月、HCourseCombine預抓6個月
        string gHCBatchNum = "CB" + DateTime.Now.AddMonths(-3).ToString("yyyyMMdd") + "0001";

        string Select = "SELECT MAX(HID) AS HID,HCourseName , HCBatchNum , MAX(HDateRange) AS HDateRange, MAX(HPlaceName) AS HPlaceName, MAX(TeacherName) AS TeacherName, MIN(b.Value) AS StartDate, MAX(b.value) AS EndDate , HVerifyStatus FROM HCourse_Merge Cross Apply SPLIT(Replace(HDateRange,' - ',','), ',') b ";

        StringBuilder sql = new StringBuilder(Select);
        List<string> WHERE = new List<string>();

        if (!MyQuery.Read())
        {
            WHERE.Add(" ((','+HTMemberID+',') LIKE '%," + Session["HUserHID"] + ",%' OR (','+HTeam) LIKE '%," + Session["HUserHID"] + ",%'  OR HCreate LIKE '%" + Session["HUserHID"] + "%') ");
        }

        MyQuery.Close();

        WHERE.Add(" HCBatchNum >= '" + gHCBatchNum + "' ");

        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" WHERE HStatus='1' AND " + wh);
        }
        else
        {
            sql.Append(" WHERE HStatus='1' ");
        }

        //WE20240323_加入依上課日期排序判斷(與前台顯示相同)
        //EE20240702_ORDERBY 順序調換 yyyy->MM
        SDS_HC.SelectCommand = sql.ToString() + " GROUP BY HCourseName, HCBatchNum, HVerifyStatus ORDER BY  IIF(DATEPART(YYYY,LEFT(TRY_CONVERT(nvarchar, getdate(), 111),10))=DATEPART(YYYY,TRY_CONVERT(nvarchar, MIN(b.Value),111)), '5', '6') ASC, IIF(DATEPART(Month,LEFT(TRY_CONVERT(nvarchar, getdate(), 111),10))=DATEPART(Month,TRY_CONVERT(nvarchar, MIN(b.Value),111)),'1' ,'2')  ASC,StartDate DESC ";
        #endregion

        ViewState["Search"] = SDS_HC.SelectCommand;

        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HC.SelectCommand, PageMax, LastPage, true, Rpt_HC);
    }
    #endregion

    #region 編輯功能
    protected void LBtn_Edit_Click(object sender, EventArgs e)
    {
        Panel_List.Visible = false;
        Panel_Edit.Visible = true;
        RegisterAsync();


        //直接顯示開課資訊，不用再自己切換
        Status(2);
        LBtn_Course.CssClass = "nav-link active show";

        LinkButton LBtn_Edit = sender as LinkButton;
        string Edit_CA = LBtn_Edit.CommandArgument;


        LB_HID.Text = Edit_CA;

        DDL_HOSystem.DataBind();
        LBox_HRSystem.DataBind();
        LBox_HNRequirement.DataBind();
        DDL_HSCourse.DataBind();
        DDL_HQuestion.DataBind();
        //DDL_HExamSubject.DataBind();
        //LBox_HSupervise.DataBind();
        LBox_HExamPaperID.DataBind();
        LBox_HIRestriction.DataBind();

        DDL_HExamContentID.DataBind();
        //DDL_HTeacherClass.DataBind();
        //DDL_HTearcherLV.DataBind();


        string VerifyStatus = "";
        using (con)
        {

            string sql = "SELECT a.HCourseName, a.HTeacherName, a.HTeam, a.HDateRange, a.HCTemplateID, a.HTMemberID, a.TeacherName, a.HPlaceName,a.HCBatchNum, a.HSCourseID, a.HSTime, a.HETime, a.HRemark,a.HContentTitle, a.HContent, a.HRNContent, a.HImg, a.BCSchedule, a.BECSchedule, a.ICRecord, a.DPosition, a.CPosition, a.TPosition,  a.HType, a.HOSystem, a.HNLCourse,a.HRSystem, a.HNGuide, a.HNFull, a.HNRequirement, a.HQuestionID, a.HPMethod, a.HBCPoint, a.HSGList, a.HIRestriction,a.HBudget, a.HBudgetTable,a.HVerifyStatus, a.HID, a.HOCPlace, a.HTMaterialID, a.HCDeadline, a.HCDeadlineDay, a.HBudgetType, a.HSerial, b.HLodging, a.HDContinous, a.HCourseType, a.HAxisYN, a.HAxisClass, a.HExamSubject, a.HGradeCalculation, a.HExamPaperID, a.HSupervise, a.HAttRateStandard, a.HExamPassStandard, a.HExamContentID, a.HParticipantLimit, a.HTeacherClass, a.HTearcherLV, a.HBookByDateYN,a.HCCPeriodYN,a.HCCPeriodDItem FROM HCourse_Merge AS a LEFT JOIN HCourse_T AS b ON a.HCTemplateID=b.HID WHERE a.HID='" + Edit_CA + "' AND a.HStatus='1'";
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                con.Open();

                using (SqlDataReader MyQueryHC = cmd.ExecuteReader())
                {

                    if (MyQueryHC.Read())
                    {
                        #region 1221修-判斷審核狀態顯示按鈕/開放功能

                        LB_HVerifyStatus.Text = MyQueryHC["HVerifyStatus"].ToString();
                        VerifyStatus = MyQueryHC["HVerifyStatus"].ToString();

                        //HVerifyStatus→99:尚未審核、0審核中、1審核不通過、2審核已通過、3核准不通過、4.核准已通過
                        Btn_Verify.Visible = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        Btn_Submit.Visible = VerifyStatus == "0" ? false : true;
                        Panel_Template.Enabled = VerifyStatus == "0" ? false : true;//--221021 審核通過後仍可修改；審核中不能改
                        //Panel_Course.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        TB_HCourseName.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        LBox_HTeacherName.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        LBox_HTeam.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        LBox_HSupervise.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        LBox_HOCPlace.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        LBtn_ClearAllPlace.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        RBL_Continuous.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        TB_HDateRange.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        TB_HSTime.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        TB_HETime.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        DDL_HCourseType.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        DDL_HGradeCalculation.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        LBox_HExamPaperID.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        TB_HAttRateStandard.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        DDL_HTeacherClass.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        DDL_HTearcherLV.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        FU_HImg.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;

                        TB_HExamPassStandard.Enabled = VerifyStatus == "0" ? false : true;//審核通過後仍可修改；審核中不能改

                        Panel_Content.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        Panel_Material.Enabled = VerifyStatus == "0" ? false : true;//--220713 審核通過後仍可修改；審核中不能改
                        Tr_Material_Add.Visible = VerifyStatus == "0" ? false : true;//--221021 審核通過後仍可修改；審核中不能改
                        Tr_Jobs_Add.Visible = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        Panel_Notice.Enabled = VerifyStatus == "0" || VerifyStatus == "2" || VerifyStatus == "4" ? false : true;
                        Tr_HW_Add.Visible = VerifyStatus == "0" ? false : true;//--221021 審核通過後仍可修改；審核中不能改
                        Panel_Homework.Enabled = VerifyStatus == "0" ? false : true;//--221021 審核通過後仍可修改；審核中不能改
                        Panel_HCourseEvaluation.Enabled = VerifyStatus == "0" ? false : true;
                        Tr_HCourseEvaluation_Add.Visible = VerifyStatus == "0" ? false : true;

                        #endregion

                        #region 課程範本資訊
                        DDL_HCourseTemplate.SelectedValue = MyQueryHC["HCTemplateID"].ToString();
                        DDL_HType.SelectedValue = MyQueryHC["HType"].ToString();

                        RBL_HNLCourse.SelectedValue = MyQueryHC["HNLCourse"].ToString();
                        RBL_HNGuide.SelectedValue = MyQueryHC["HNGuide"].ToString();
                        RBL_HNFull.SelectedValue = MyQueryHC["HNFull"].ToString();
                        RBL_HSerial.SelectedValue = string.IsNullOrEmpty(MyQueryHC["HSerial"].ToString()) ? "0" : MyQueryHC["HSerial"].ToString();
                        //RBL_HBudget.SelectedValue = string.IsNullOrEmpty(MyQueryHC["HBudget"].ToString()) ? "0" : MyQueryHC["HBudget"].ToString();

                        TB_HCDeadline.Text = MyQueryHC["HCDeadline"].ToString();
                        DDL_HSCourse.SelectedValue = !string.IsNullOrEmpty(MyQueryHC["HSCourseID"].ToString()) ? MyQueryHC["HSCourseID"].ToString() : "0";

                        if (RBL_TestCourse.SelectedValue == "1")
                        {

                            if (DDL_HExamSubject.Items.FindByValue(MyQueryHC["HExamSubject"].ToString()) != null)
                            {
                                DDL_HExamSubject.SelectedValue = MyQueryHC["HExamSubject"].ToString();
                            }
                        }
                        else
                        {
                            DDL_HExamSubject.SelectedValue = "0";
                        }
                        DDL_HOSystem.SelectedValue = MyQueryHC["HOSystem"].ToString();

                        SetSelectedItems(LBox_HRSystem, MyQueryHC["HRSystem"].ToString());
                        LB_HRSystem.Text = MyQueryHC["HRSystem"].ToString();

                        TB_HCDeadlineDay.Text = !string.IsNullOrEmpty(MyQueryHC["HCDeadlineDay"].ToString()) ? MyQueryHC["HCDeadlineDay"].ToString() : "0";

                        //EA20240424_加入軸線類別
                        ListItem HAxisYN = RBL_HAxisYN.Items.FindByValue(MyQueryHC["HAxisYN"].ToString());
                        if (HAxisYN != null)
                        {
                            RBL_HAxisYN.SelectedValue = MyQueryHC["HAxisYN"].ToString();

                            if (MyQueryHC["HAxisYN"].ToString() == "1")
                            {
                                if (!string.IsNullOrEmpty(MyQueryHC["HAxisClass"].ToString()))
                                {
                                    DDL_HAxisClass.SelectedValue = MyQueryHC["HAxisClass"].ToString();
                                }

                            }
                            else
                            {
                                DDL_HAxisClass.SelectedValue = "0";
                            }
                        }

                        ListItem HLodging = RBL_HLodging.Items.FindByValue(MyQueryHC["HLodging"].ToString());
                        if (HLodging != null)
                        {
                            RBL_HLodging.SelectedValue = MyQueryHC["HLodging"].ToString();
                        }

                        ListItem HBudget = RBL_HBudget.Items.FindByValue(MyQueryHC["HBudget"].ToString());
                        if (HBudget != null)
                        {
                            RBL_HBudget.SelectedValue = MyQueryHC["HBudget"].ToString();
                        }

                        //WE20231214_加入註解_HPMethod(繳費帳戶)：1=基金會；2=文化事業(HBCPoint*10=報名金額)
                        DDL_HPMethod.SelectedValue = MyQueryHC["HPMethod"].ToString();
                        //220819-顯示轉為金額
                        if (!string.IsNullOrEmpty(MyQueryHC["HBCPoint"].ToString()))
                        {
                            TB_HBCPoint.Text = (Convert.ToInt32(MyQueryHC["HBCPoint"].ToString()) * 10).ToString();
                        }

                        //WA20250611_新增HBookByDateYN
                        RBL_HBookByDateYN.SelectedValue = !string.IsNullOrEmpty(MyQueryHC["HBookByDateYN"].ToString()) ? MyQueryHC["HBookByDateYN"].ToString() : "0";

                        //WA20250715_新增HCCPeriodYN
                        RBL_HCCPeriodYN.SelectedValue = string.IsNullOrEmpty(MyQueryHC["HCCPeriodYN"].ToString()) ? "0" : MyQueryHC["HCCPeriodYN"].ToString();

                        DDL_HCCPeriodDItem.SelectedValue = string.IsNullOrEmpty(MyQueryHC["HCCPeriodDItem"].ToString()) ? "0" : MyQueryHC["HCCPeriodDItem"].ToString();

                        SetSelectedItems(LBox_HNRequirement, MyQueryHC["HNRequirement"].ToString());
                        LB_HNRequirement.Text = MyQueryHC["HNRequirement"].ToString();
                        SetSelectedItems(LBox_HQuestionID, MyQueryHC["HQuestionID"].ToString());
                        //SetSelectedItems(LBox_HTMaterialID, MyQueryHC["HTMaterialID"].ToString());
                        SetSelectedItems(LBox_HIRestriction, MyQueryHC["HIRestriction"].ToString());
                        LB_HIRestriction.Text = MyQueryHC["HIRestriction"].ToString();

                        //TB_HBudgetType.Visible = true;
                        TB_HBudgetType.Text = MyQueryHC["HBudgetType"].ToString();

                        //EA20250509_ 檢覈內容名稱、報名人數上限
                        DDL_HExamContentID.SelectedValue = !string.IsNullOrEmpty(MyQueryHC["HExamContentID"].ToString()) ? MyQueryHC["HExamContentID"].ToString() : "0";
                        TB_HParticipantLimit.Text = MyQueryHC["HParticipantLimit"].ToString();


                        TB_HRemark.Text = HttpUtility.HtmlDecode(MyQueryHC["HRemark"].ToString());

                        #endregion

                        #region 開課資訊

                        TB_HCBatchNum.Text = MyQueryHC["HCBatchNum"].ToString();
                        LB_HCBatchNum.Text = MyQueryHC["HCBatchNum"].ToString();
                        TB_HCourseName.Text = MyQueryHC["HCourseName"].ToString();

                        ////上課地點
                        //BindListControls("SELECT HID, HPlaceName, HStatus FROM HPlaceList ORDER BY HStatus DESC, strHAreaID ASC", "HPlaceName", "HID", "-請選擇-", true, LBox_HOCPlace, DDL_HOCPlace, DDL_SHOCPlace);

                        ////主班團隊--二階以上可成為主班
                        //BindListControls("select HMember.HID, (HArea+'/'+HPeriod+' '+HUserName) as UserName, HMember.HStatus from HMember Left Join HArea On HMember.HAreaID =HArea.HID WHERE  HMember.HStatus =1 AND HType in(7,8,9,10,11,12,13)   order by HUserName", "UserName", "HID", "-請選擇-", true, LBox_HTeam);

                        ////課程講師
                        //BindListControls("SELECT HTeacher.HID, HTeacher.HStatus, HPeriod, HArea, (HArea+'/'+HPeriod+' '+HUserName+'-'+ISNULL(HCourseName, '')) AS HTeacherName, HCourseName FROM HTeacher INNER JOIN HMember ON HMemberID = HMember.HID LEFT JOIN HArea ON HMember.HAreaID = HArea.HID LEFT JOIN HCourse ON HCourseID=HCourse.HID", "HTeacherName", "HID", "-請選擇-", true, LBox_HTeacherName);


                        //DDL_HTeacherClass.DataSource = DDL_ListItem.HTeacher.HTeacherClass;
                        //DDL_HTeacherClass.DataTextField = "Value";
                        //DDL_HTeacherClass.DataValueField = "Key";
                        //DDL_HTeacherClass.DataBind();

                        //DDL_HTearcherLV.DataSource = DDL_ListItem.HTeacher.HTearcherLV;
                        //DDL_HTearcherLV.DataTextField = "Value";
                        //DDL_HTearcherLV.DataValueField = "Key";
                        //DDL_HTearcherLV.DataBind();

                        SetSelectedItems(LBox_HTeam, MyQueryHC["HTeam"].ToString());
                        SetSelectedItems(LBox_HSupervise, MyQueryHC["HSupervise"].ToString());
                        SetSelectedItems(LBox_HTeacherName, MyQueryHC["HTeacherName"].ToString());
                        SetSelectedItems(LBox_HOCPlace, MyQueryHC["HOCPlace"].ToString());






                        ////WA20231207_加入判斷課程日期要用哪一個多選還是區間
                        //if (RBL_Continuous.SelectedValue == "0")  //非連續
                        //{
                        //    TB_HDateRange.CssClass = "form-control datemultipicker";
                        //}
                        //else
                        //{
                        //    TB_HDateRange.CssClass = "form-control daterange";
                        //}

                        //WA20231129_加入連續日期判斷
                        TB_HDateRange.Text = MyQueryHC["HDateRange"].ToString();
                        if (MyQueryHC["HDContinous"].ToString() == "True")
                        {
                            RBL_Continuous.SelectedValue = "1";
                            TB_HDateRange.CssClass = "form-control daterange";
                        }
                        else
                        {
                            RBL_Continuous.SelectedValue = "0"; //非連續
                            TB_HDateRange.CssClass = "form-control datemultipicker";
                            //將資料帶入
                            ScriptManager.RegisterStartupScript(Page, GetType(), "multipledateScripts", "$('.datemultipicker').datepicker({language: 'en',dateFormat: 'yyyy/mm/dd',multipleDates: true, todayHighlight: true,orientation: 'bottom auto',startDate: $('#ctl00_ContentPlaceHolder1_TB_HDateRange').val(),onShow: function(dp, animationCompleted) {var val = $(this.$el).val();if (val) {var arr = val.split(',');this.selectDate([]);arr.forEach(function(dateStr) {    dateStr = dateStr.trim();    if (dateStr) {        var d = new Date(dateStr.replace(/\\//g, '-'));        if (!isNaN(d)) this.selectDate(d);    }}, this);} }});", true);
                        }






                        TB_HSTime.Text = MyQueryHC["HSTime"].ToString();
                        TB_HETime.Text = MyQueryHC["HETime"].ToString();
                        DDL_HCourseType.SelectedValue = string.IsNullOrEmpty(MyQueryHC["HCourseType"].ToString()) ? "0" : MyQueryHC["HCourseType"].ToString();

                        RBL_HSGList.SelectedValue = MyQueryHC["HSGList"].ToString();

                        //EA20240618_新增成績計算方式、考卷、督導、出席率標準、檢覈成績通過標準
                        DDL_HGradeCalculation.SelectedValue = !string.IsNullOrEmpty(MyQueryHC["HGradeCalculation"].ToString()) ? MyQueryHC["HGradeCalculation"].ToString() : "0";

                        SetSelectedItems(LBox_HExamPaperID, MyQueryHC["HExamPaperID"].ToString());


                        TB_HAttRateStandard.Text = !string.IsNullOrEmpty(MyQueryHC["HAttRateStandard"].ToString()) ? MyQueryHC["HAttRateStandard"].ToString() : "";
                        TB_HExamPassStandard.Text = !string.IsNullOrEmpty(MyQueryHC["HExamPassStandard"].ToString()) ? MyQueryHC["HExamPassStandard"].ToString() : "";

                        //EA20250510_加入通過後成為的講師類別、通過後成為的講師層級分類
                        DDL_HTeacherClass.SelectedValue = !string.IsNullOrEmpty(MyQueryHC["HTeacherClass"].ToString()) ? MyQueryHC["HTeacherClass"].ToString() : "0";
                        DDL_HTearcherLV.SelectedValue = !string.IsNullOrEmpty(MyQueryHC["HTearcherLV"].ToString()) ? MyQueryHC["HTearcherLV"].ToString() : "0";

                        //課程圖片
                        if (!string.IsNullOrEmpty(MyQueryHC["HImg"].ToString()))
                        {
                            IMG_Pic.ImageUrl = CourseImg + MyQueryHC["HImg"].ToString();
                            LB_OldPic.Text = MyQueryHC["HImg"].ToString();
                        }
                        else
                        {
                            Btn_Del.Visible = false;
                            IMG_Pic.Visible = false;
                            IMG_Pic.Width = 0;
                        }
                        #endregion

                        #region 內文
                        TB_HContentTitle.Text = HttpUtility.HtmlDecode(MyQueryHC["HContentTitle"].ToString());
                        CKE_HContent.Text = HttpUtility.HtmlDecode(MyQueryHC["HContent"].ToString());
                        #endregion

                        #region 報名須知/條款內容
                        CKE_HRNContent.Text = HttpUtility.HtmlDecode(MyQueryHC["HRNContent"].ToString());
                        #endregion

                        #region  相關文件
                        if (!string.IsNullOrEmpty(MyQueryHC["HBudget"].ToString()))
                        {
                            if (MyQueryHC["HBudget"].ToString() == "1")
                            {
                                HBudgetTable.Visible = true;
                                if (!string.IsNullOrEmpty(MyQueryHC["HBudgetTable"].ToString()))
                                {
                                    LBtn_HBudgetTableDel.Visible = true;
                                    HL_HBudgetTable.Text = MyQueryHC["HBudgetTable"].ToString();
                                    HL_HBudgetTable.NavigateUrl = CourseFile + MyQueryHC["HBudgetTable"].ToString();
                                    LB_HBudgetTable.Text = MyQueryHC["HBudgetTable"].ToString();
                                }
                            }
                            else
                            {
                                HBudgetTable.Visible = false;
                            }
                        }

                        if (!string.IsNullOrEmpty(MyQueryHC["BCSchedule"].ToString()))
                        {
                            LBtn_BCScheduleDel.Visible = true;
                            HL_BCSchedule.Text = MyQueryHC["BCSchedule"].ToString();
                            HL_BCSchedule.NavigateUrl = CourseFile + MyQueryHC["BCSchedule"].ToString();
                            LB_BCSchedule.Text = MyQueryHC["BCSchedule"].ToString();
                        }

                        if (!string.IsNullOrEmpty(MyQueryHC["BECSchedule"].ToString()))
                        {
                            LBtn_BECScheduleDel.Visible = true;
                            HL_BECSchedule.Text = MyQueryHC["BECSchedule"].ToString();
                            HL_BECSchedule.NavigateUrl = CourseFile + MyQueryHC["BECSchedule"].ToString();
                            LB_BECSchedule.Text = MyQueryHC["BECSchedule"].ToString();
                        }

                        if (!string.IsNullOrEmpty(MyQueryHC["ICRecord"].ToString()))
                        {
                            LBtn_ICRecordDel.Visible = true;
                            HL_ICRecord.Text = MyQueryHC["ICRecord"].ToString();
                            HL_ICRecord.NavigateUrl = CourseFile + MyQueryHC["ICRecord"].ToString();
                            LB_ICRecord.Text = MyQueryHC["ICRecord"].ToString();
                        }

                        if (!string.IsNullOrEmpty(MyQueryHC["DPosition"].ToString()))
                        {
                            LBtn_DPositionDel.Visible = true;
                            HL_DPosition.Text = MyQueryHC["DPosition"].ToString();
                            HL_DPosition.NavigateUrl = CourseFile + MyQueryHC["DPosition"].ToString();
                            LB_DPosition.Text = MyQueryHC["DPosition"].ToString();
                        }

                        if (!string.IsNullOrEmpty(MyQueryHC["CPosition"].ToString()))
                        {
                            LBtn_CPositionDel.Visible = true;
                            HL_CPosition.Text = MyQueryHC["CPosition"].ToString();
                            HL_CPosition.NavigateUrl = CourseFile + MyQueryHC["CPosition"].ToString();
                            LB_CPosition.Text = MyQueryHC["CPosition"].ToString();
                        }


                        if (!string.IsNullOrEmpty(MyQueryHC["TPosition"].ToString()))
                        {
                            LBtn_TPositionDel.Visible = true;
                            HL_TPosition.Text = MyQueryHC["TPosition"].ToString();
                            HL_TPosition.NavigateUrl = CourseFile + MyQueryHC["TPosition"].ToString();
                            LB_TPosition.Text = MyQueryHC["TPosition"].ToString();
                        }


                        #endregion

                    }
                }
            }

        }

        #region 各Tab SDS.DataBind()


        ////20220821體系護持工作項目改為可修改組長
        SDS_HTodoList.SelectCommand = "SELECT HGroupName, HTask, HTaskContent, HGroupLeaderID,HSave, HTaskNum, HExamStaff FROM HTodoList where HCBatchNum='" + LB_HCBatchNum.Text + "' AND HStatus=1 GROUP BY HGroupName, HTask, HTaskContent, HGroupLeaderID,HSave, HTaskNum, HExamStaff ORDER BY HGroupName ASC";
        //Rpt_HTodoList.DataBind();

        //WE20240329_加入作業類型、問卷、內容描述
        SDS_HCourseHWSetting.SelectCommand = "SELECT a.HCBatchNum, a.HNumbers, a.HDeadLine, a.HSave, a.HHWType, a.HQuestionHID, a.HDescription, b.HTitle FROM HCourse_HWSetting AS a  LEFT JOIN HQuestion AS b ON a.HQuestionHID= b.HID WHERE a.HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY a.HCBatchNum, a.HNumbers, a.HDeadLine, a.HSave,  a.HHWType, a.HQuestionHID, a.HDescription, b.HTitle  ORDER BY HDeadLine ASC";
        //RPT_HCourseHWSetting.DataBind();


        //EA20250409_講師教材設定
        SDS_HTMaterialDetail.SelectCommand = "SELECT HTMaterialID, HSort, HTMaterial, HSave FROM HCourseTMaterial WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HStatus='1' GROUP BY HTMaterialID, HSort, HTMaterial, HSave ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
        //Rpt_HTMaterialDetail.DataBind();
        #endregion

        #region 欄位判斷/顯示

        SqlDataReader QueryHCourse_Class = SQLdatabase.ExecuteReader("SELECT HID, HType FROM HCourse_Class where HID='" + DDL_HType.SelectedValue + "'");
        if (QueryHCourse_Class.Read())
        {
            RBL_TestCourse.SelectedValue = QueryHCourse_Class["HType"].ToString() == "2" ? "1" : "0";
        }
        QueryHCourse_Class.Close();



        DDL_HSCourse.Enabled = RBL_HSerial.SelectedValue == "1" ? true : false;

        //EA20231030_課程報名截止日
        if (RBL_HSerial.SelectedValue == "0")
        {
            LB_HCDeadlineDayTitle.Text = "課程開始日前";
        }
        else
        {
            LB_HCDeadlineDayTitle.Text = "課程結束日後";
        }

        //230713-審核中作業應不能刪除
        for (int i = 0; i < RPT_HCourseHWSetting.Items.Count; i++)
        {
            if (VerifyStatus == "0")
            {
                ((LinkButton)RPT_HCourseHWSetting.Items[i].FindControl("LBtn_HW_Del")).Visible = false;
            }
        }

        for (int i = 0; i < Rpt_HCourseEvaluation.Items.Count; i++)
        {
            if (VerifyStatus == "0")
            {
                ((LinkButton)Rpt_HCourseEvaluation.Items[i].FindControl("LBtn_HCE_Del")).Visible = false;
            }
        }


        //WA20250517_審核中體系護持工作項目不能刪除
        for (int i = 0; i < Rpt_HTodoList.Items.Count; i++)
        {
            if (VerifyStatus == "0")
            {
                ((LinkButton)Rpt_HTodoList.Items[i].FindControl("LBtn_HTodoList_Del")).Visible = false;
            }
        }

        #endregion

    }
    #endregion

    #region 複製功能
    protected void LBtn_Copy_Click(object sender, EventArgs e)
    {
        Panel_List.Visible = false;
        Panel_Edit.Visible = true;

        //ScriptManager.RegisterStartupScript(Page, GetType(), "Scripts", "showTab('tab-course');", true);

        //讓它固定在開課資訊
        Status(2);
        LBtn_Course.CssClass = "nav-link active show";



        LinkButton LBtn_Edit = sender as LinkButton;
        string Edit_CA = LBtn_Edit.CommandArgument;


        #region 單號自動產生流水號

        string connStr = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;

        string StrHCBatchNum = null;
        string day = DateTime.Now.ToString("yyyyMMdd");

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string sql = "SELECT TOP (1) HCBatchNum FROM HCourse WHERE HCBatchNum LIKE @DayPattern ORDER BY HCBatchNum DESC";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@DayPattern", "%" + day + "%");
                conn.Open();

                using (SqlDataReader AutoReg = cmd.ExecuteReader())
                {
                    if (AutoReg.Read())
                    {
                        //先取當日最後一筆報名單號
                        string RNo = AutoReg["HCBatchNum"].ToString();
                        //將單號分割只取後面四碼，並判斷避免原字串長度不夠
                        string SerialNum = RNo.Substring((RNo.Length - 4) > 0 ? RNo.Length - 4 : 0);
                        //去除後四碼前面的0
                        int num = Convert.ToInt32(SerialNum.TrimStart('0'));
                        int w = 4;
                        StrHCBatchNum = "CB" + day + String.Format("{0:D" + w + "}", num + 1);
                    }
                    else
                    {
                        int num = 1;
                        int w = 4;
                        StrHCBatchNum = "CB" + day + String.Format("{0:D" + w + "}", num);
                    }
                }

            }



        }

        #endregion

        LB_HCBatchNum.Text = StrHCBatchNum;

        string gHCTemplateID = "";//複製HTodoList用
        string gHID = "";//copy後取得HQuestion的HID

        //WA20241206_新增紀錄原上課地點的變數
        string gOriHOCPlace = null;//複製時先紀錄原本的上課地點

        string strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        SqlConnection dbConn = new SqlConnection(strDBConn);
        SqlCommand dbCmd = default(SqlCommand);
        dbConn.Open();

        string strSelHC1 = "SELECT HCourseName, HTeacherName, HTeam, HDateRange, HDharmaID, HCTemplateID, HTMemberID,   TeacherName, HPlaceName, HStatus, HVerifyStatus, HCreate, HSave, HCBatchNum, HSCourseID, HSTime, HETime,  HRemark, HContentTitle, HContent, HRNContent, HImg, BCSchedule, BECSchedule, ICRecord, DPosition, CPosition,TPosition, HUserName, HApplicant, HApplyTime, HVerifyTime, HType, HEnableSystem, HApplySystem, HOSystem,HNLCourse, HRSystem, HNGuide, HNFull, HNRequirement, HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HVerifyNum, HBudget, HBudgetTable, HID, HOCPlace, HTMaterialID, HCDeadline, HSerial, HCDeadlineDay, HDContinous, HBudgetType, HCourseType, HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID, HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV, HBookByDateYN, HCCPeriodYN, HCCPeriodDItem FROM HCourse_Merge WHERE HID='" + Edit_CA + "'";

        dbCmd = new SqlCommand(strSelHC1, dbConn);
        SqlDataReader MyQueryHC1 = dbCmd.ExecuteReader();
        if (MyQueryHC1.Read())
        {
            gHCTemplateID = MyQueryHC1["HCTemplateID"].ToString();
            //HCourse(課程)-BCSchedule, BECSchedule, ICRecord, DPosition, CPosition, TPosition不用複製
            //EE20231121_拿掉 a.HDharmaID (受傳)
            //EE20240622_改寫取值寫法
            //WE20241206_HDateRange(課程日期)不用複製、HOCPlace上課地點要複製，但因一個地點是一筆資料，故用變數(gOriHOCPlace)暫存的方式帶入選單中，先不寫入DB
            string strInsHC1 = " IF EXISTS(SELECT HCourseNum FROM HCourse WHERE SUBSTRING(HCourseNum,2,8) = CONVERT(nvarchar, getdate(), 112)) BEGIN INSERT INTO HCourse (HCourseNum, HEnableSystem, HApplySystem, HCTemplateID, HCBatchNum, HCourseName, HTeacherName, HOCPlace, HDateRange, HSTime, HETime, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam, HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HRNContent, HVerifyNum, HApplicant, HApplyTime, HVerifyTime, HVerifyStatus, HImg, BCSchedule, BECSchedule, ICRecord, DPosition, CPosition, TPosition, HBudget, HBudgetTable, HSCourseID, HTMaterialID, HCDeadlineDay, HSerial, HDContinous, HBudgetType, HCourseType, HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID, HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV, HBookByDateYN,HStatus, HCreate, HCreateDT, HSave) VALUES ('C' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HCourseNum), 12)) FROM HCourse) + 1),	@HEnableSystem,	@HApplySystem,	@HCTemplateID,	@HCBatchNum,	@HCourseName,	@HTeacherName,	@HOCPlace,	@HDateRange,	@HSTime,	@HETime,	@HType,	@HOSystem,	@HRSystem,	@HNLCourse,	@HNGuide,	@HNFull,	@HNRequirement,	@HTeam,	@HQuestionID,	@HPMethod,	@HBCPoint,	@HSGList,	@HIRestriction,	@HRemark,	@HContentTitle,	@HContent,	@HRNContent,	@HVerifyNum,	@HApplicant,	@HApplyTime,	@HVerifyTime,	@HVerifyStatus,	@HImg,	@BCSchedule,	@BECSchedule,	@ICRecord,	@DPosition,	@CPosition,	@TPosition,	@HBudget,	@HBudgetTable,	@HSCourseID,	@HTMaterialID,	@HCDeadlineDay,	@HSerial,	@HDContinous,	@HBudgetType,	@HCourseType,	@HAxisYN, @HAxisClass,	@HExamSubject,	@HGradeCalculation,	@HExamPaperID,	@HSupervise,	@HAttRateStandard,	@HExamPassStandard, @HExamContentID, @HParticipantLimit, @HTeacherClass, @HTearcherLV, @HBookByDateYN,	@HStatus,	@HCreate,	@HCreateDT,	@HSave); SELECT SCOPE_IDENTITY() AS HID END ELSE BEGIN INSERT INTO HCourse (HCourseNum, HEnableSystem, HApplySystem, HCTemplateID, HCBatchNum, HCourseName, HTeacherName, HOCPlace, HDateRange, HSTime, HETime, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam, HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HRNContent, HVerifyNum, HApplicant, HApplyTime, HVerifyTime, HVerifyStatus, HImg, BCSchedule, BECSchedule, ICRecord, DPosition, CPosition, TPosition, HBudget, HBudgetTable, HSCourseID, HTMaterialID, HCDeadlineDay, HSerial, HDContinous,HBudgetType, HCourseType, HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID, HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV, HBookByDateYN,HStatus, HCreate, HCreateDT, HSave) VALUES ('C' + CONVERT(nvarchar, getdate(), 112) + '0001',	@HEnableSystem,	@HApplySystem,	@HCTemplateID,	@HCBatchNum,	@HCourseName,	@HTeacherName,	@HOCPlace,	@HDateRange,	@HSTime,	@HETime,	@HType,	@HOSystem,	@HRSystem,	@HNLCourse,	@HNGuide,	@HNFull,	@HNRequirement,	@HTeam,	@HQuestionID,	@HPMethod,	@HBCPoint,	@HSGList,	@HIRestriction,	@HRemark,	@HContentTitle,	@HContent,	@HRNContent,	@HVerifyNum,	@HApplicant,	@HApplyTime,	@HVerifyTime,	@HVerifyStatus,	@HImg,	@BCSchedule,	@BECSchedule,	@ICRecord,	@DPosition,	@CPosition,	@TPosition,	@HBudget,	@HBudgetTable,	@HSCourseID,	@HTMaterialID,	@HCDeadlineDay,	@HSerial,	@HDContinous,	@HBudgetType,	@HCourseType,	@HAxisYN,	@HAxisClass,	@HExamSubject,	@HGradeCalculation,	@HExamPaperID,	@HSupervise,	@HAttRateStandard,	@HExamPassStandard, @HExamContentID, @HParticipantLimit, @HTeacherClass, @HTearcherLV,	 @HBookByDateYN,@HStatus,	@HCreate,	@HCreateDT,	@HSave); SELECT SCOPE_IDENTITY() AS HID END";

            dbCmd = new SqlCommand(strInsHC1, dbConn);
            dbCmd.Parameters.AddWithValue("@HEnableSystem", MyQueryHC1["HEnableSystem"].ToString());
            dbCmd.Parameters.AddWithValue("@HApplySystem", MyQueryHC1["HApplySystem"].ToString());
            dbCmd.Parameters.AddWithValue("@HCTemplateID", MyQueryHC1["HCTemplateID"].ToString());
            dbCmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);
            dbCmd.Parameters.AddWithValue("@HCourseName", MyQueryHC1["HCourseName"].ToString());
            dbCmd.Parameters.AddWithValue("@HTeacherName", MyQueryHC1["HTeacherName"].ToString());
            //dbCmd.Parameters.AddWithValue("@HOCPlace", "0");
            dbCmd.Parameters.AddWithValue("@HOCPlace", MyQueryHC1["HOCPlace"].ToString());
            dbCmd.Parameters.AddWithValue("@HDateRange", MyQueryHC1["HDateRange"].ToString());
            dbCmd.Parameters.AddWithValue("@HSTime", MyQueryHC1["HSTime"].ToString());
            dbCmd.Parameters.AddWithValue("@HETime", MyQueryHC1["HETime"].ToString());
            dbCmd.Parameters.AddWithValue("@HType", MyQueryHC1["HType"].ToString());
            dbCmd.Parameters.AddWithValue("@HOSystem", MyQueryHC1["HOSystem"].ToString());
            dbCmd.Parameters.AddWithValue("@HRSystem", MyQueryHC1["HRSystem"].ToString());
            dbCmd.Parameters.AddWithValue("@HNLCourse", MyQueryHC1["HNLCourse"].ToString());
            dbCmd.Parameters.AddWithValue("@HNGuide", MyQueryHC1["HNGuide"].ToString());
            dbCmd.Parameters.AddWithValue("@HNFull", MyQueryHC1["HNFull"].ToString());
            dbCmd.Parameters.AddWithValue("@HNRequirement", MyQueryHC1["HNRequirement"].ToString());
            dbCmd.Parameters.AddWithValue("@HTeam", MyQueryHC1["HTeam"].ToString());
            dbCmd.Parameters.AddWithValue("@HQuestionID", MyQueryHC1["HQuestionID"].ToString());
            dbCmd.Parameters.AddWithValue("@HPMethod", MyQueryHC1["HPMethod"].ToString());
            dbCmd.Parameters.AddWithValue("@HBCPoint", MyQueryHC1["HBCPoint"].ToString());
            dbCmd.Parameters.AddWithValue("@HSGList", MyQueryHC1["HSGList"].ToString());
            dbCmd.Parameters.AddWithValue("@HIRestriction", MyQueryHC1["HIRestriction"].ToString());
            dbCmd.Parameters.AddWithValue("@HRemark", MyQueryHC1["HRemark"].ToString());
            dbCmd.Parameters.AddWithValue("@HContentTitle", MyQueryHC1["HContentTitle"].ToString());
            dbCmd.Parameters.AddWithValue("@HContent", MyQueryHC1["HContent"].ToString());
            dbCmd.Parameters.AddWithValue("@HRNContent", MyQueryHC1["HRNContent"].ToString());
            dbCmd.Parameters.AddWithValue("@HVerifyNum", MyQueryHC1["HVerifyNum"].ToString());
            dbCmd.Parameters.AddWithValue("@HApplicant", MyQueryHC1["HApplicant"].ToString());
            dbCmd.Parameters.AddWithValue("@HApplyTime", MyQueryHC1["HApplyTime"].ToString());
            dbCmd.Parameters.AddWithValue("@HVerifyTime", MyQueryHC1["HVerifyTime"].ToString());
            dbCmd.Parameters.AddWithValue("@HVerifyStatus", "99");
            dbCmd.Parameters.AddWithValue("@HImg", MyQueryHC1["HImg"].ToString());
            dbCmd.Parameters.AddWithValue("@BCSchedule", MyQueryHC1["BCSchedule"].ToString());
            dbCmd.Parameters.AddWithValue("@BECSchedule", MyQueryHC1["BECSchedule"].ToString());
            dbCmd.Parameters.AddWithValue("@ICRecord", MyQueryHC1["ICRecord"].ToString());
            dbCmd.Parameters.AddWithValue("@DPosition", MyQueryHC1["DPosition"].ToString());
            dbCmd.Parameters.AddWithValue("@CPosition", MyQueryHC1["CPosition"].ToString());
            dbCmd.Parameters.AddWithValue("@TPosition", MyQueryHC1["TPosition"].ToString());
            dbCmd.Parameters.AddWithValue("@HBudget", MyQueryHC1["HBudget"].ToString());
            dbCmd.Parameters.AddWithValue("@HBudgetTable", MyQueryHC1["HBudgetTable"].ToString());
            dbCmd.Parameters.AddWithValue("@HSCourseID", MyQueryHC1["HSCourseID"].ToString());
            dbCmd.Parameters.AddWithValue("@HTMaterialID", MyQueryHC1["HTMaterialID"].ToString());
            dbCmd.Parameters.AddWithValue("@HCDeadlineDay", MyQueryHC1["HCDeadlineDay"].ToString());
            dbCmd.Parameters.AddWithValue("@HSerial", MyQueryHC1["HSerial"].ToString());
            dbCmd.Parameters.AddWithValue("@HDContinous", MyQueryHC1["HDContinous"].ToString());
            dbCmd.Parameters.AddWithValue("@HBudgetType", MyQueryHC1["HBudgetType"].ToString());
            dbCmd.Parameters.AddWithValue("@HCourseType", MyQueryHC1["HCourseType"].ToString());
            dbCmd.Parameters.AddWithValue("@HAxisYN", MyQueryHC1["HAxisYN"].ToString());
            dbCmd.Parameters.AddWithValue("@HAxisClass", MyQueryHC1["HAxisClass"].ToString());
            dbCmd.Parameters.AddWithValue("@HExamSubject", MyQueryHC1["HExamSubject"].ToString());
            dbCmd.Parameters.AddWithValue("@HGradeCalculation", MyQueryHC1["HGradeCalculation"].ToString());
            dbCmd.Parameters.AddWithValue("@HExamPaperID", MyQueryHC1["HExamPaperID"].ToString());
            dbCmd.Parameters.AddWithValue("@HSupervise", MyQueryHC1["HSupervise"].ToString());
            dbCmd.Parameters.AddWithValue("@HAttRateStandard", MyQueryHC1["HAttRateStandard"].ToString());
            dbCmd.Parameters.AddWithValue("@HExamPassStandard", MyQueryHC1["HExamPassStandard"].ToString());
            dbCmd.Parameters.AddWithValue("@HExamContentID", MyQueryHC1["HExamContentID"].ToString());
            dbCmd.Parameters.AddWithValue("@HParticipantLimit", MyQueryHC1["HParticipantLimit"].ToString());
            dbCmd.Parameters.AddWithValue("@HTeacherClass", MyQueryHC1["HTeacherClass"].ToString());
            dbCmd.Parameters.AddWithValue("@HTearcherLV", MyQueryHC1["HTearcherLV"].ToString());
            dbCmd.Parameters.AddWithValue("@HBookByDateYN", MyQueryHC1["HBookByDateYN"].ToString());
            dbCmd.Parameters.AddWithValue("@HStatus", "1");
            dbCmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            dbCmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            dbCmd.Parameters.AddWithValue("@HSave", "0");

            //要取得新的HID資料(複製後的HID)
            gHID = dbCmd.ExecuteScalar().ToString();

            //gHID += dbCmd.ExecuteScalar().ToString() + ",";

            //WA20241206_新增紀錄原本的上課地點
            gOriHOCPlace = MyQueryHC1["HOCPlace"].ToString();


        }
        MyQueryHC1.Close();

        //複製功能HID要改為複製後的HID
        LB_HID.Text = gHID;
        if (gHID != "")
        {
            LB_HID.Text = gHID;
        }

        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Trim(',').Split(',');

            #region HTodoList(體系護持工作項目)-組長不用複製，帶範本的就好

            string strSelHTL = "SELECT HID, HCTemplateID, HGroupName, HTask, HTaskNum, HTaskContent, HGroupLeaderID, HStatus, HSave FROM HTodoList_T WHERE HCTemplateID = @TemplateID";
            dbCmd = new SqlCommand(strSelHTL, dbConn);
            dbCmd.Parameters.AddWithValue("@TemplateID", gHCTemplateID);
            SqlDataReader MyQueryHTL = dbCmd.ExecuteReader();
            List<Dictionary<string, object>> todoListData = new List<Dictionary<string, object>>();
            while (MyQueryHTL.Read())
            {
                var row = new Dictionary<string, object>();
                row["HGroupName"] = MyQueryHTL["HGroupName"].ToString();
                row["HTask"] = MyQueryHTL["HTask"].ToString();
                row["HTaskNum"] = MyQueryHTL["HTaskNum"].ToString();
                row["HTaskContent"] = MyQueryHTL["HTaskContent"].ToString();
                row["HGroupLeaderID"] = MyQueryHTL["HGroupLeaderID"].ToString();
                todoListData.Add(row);
            }
            MyQueryHTL.Close();

            #endregion

            // 逐筆插入
            string insertHTLSQL = @"INSERT INTO HTodoList 
(HCourseID, HGroupName, HTask, HTaskNum, HTaskContent, HGroupLeaderID, HStatus, HCreate, HCreateDT, HSave, HCBatchNum) 
VALUES 
(@HCourseID, @HGroupName, @HTask, @HTaskNum, @HTaskContent, @HGroupLeaderID, '1', @HCreate, @HCreateDT, '0', @HCBatchNum)";

            for (int i = 0; i < strHID.Length; i++)
            {
                #region HTodoList(體系護持工作項目)
                foreach (var item in todoListData)
                {
                    using (SqlCommand insertCmd = new SqlCommand(insertHTLSQL, dbConn))
                    {
                        insertCmd.Parameters.AddWithValue("@HCourseID", strHID[i]);
                        insertCmd.Parameters.AddWithValue("@HGroupName", item["HGroupName"]);
                        insertCmd.Parameters.AddWithValue("@HTask", item["HTask"]);
                        insertCmd.Parameters.AddWithValue("@HTaskNum", item["HTaskNum"]);
                        insertCmd.Parameters.AddWithValue("@HTaskContent", item["HTaskContent"]);
                        insertCmd.Parameters.AddWithValue("@HGroupLeaderID", item["HGroupLeaderID"]);
                        insertCmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        insertCmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        insertCmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);

                        insertCmd.ExecuteNonQuery();
                    }
                }
                #endregion


                #region 講師教材
                string strSelHCTM = "SELECT HID, HCTemplateID, HTMaterialID, HSort, HTMaterial FROM HCourseTMaterial_T WHERE HCTemplateID='" + gHCTemplateID + "'";

                dbCmd = new SqlCommand(strSelHCTM, dbConn);
                SqlDataReader MyQueryHCTM = dbCmd.ExecuteReader();
                while (MyQueryHCTM.Read())
                {

                    SqlCommand cmd = new SqlCommand("INSERT INTO HCourseTMaterial (HCBatchNum, HCourseID, HTMaterialID, HSort, HTMaterial, HStatus, HCreate, HCreateDT, HSave) values (@HCBatchNum, @HCourseID, @HTMaterialID, @HSort, @HTMaterial, @HStatus, @HCreate, @HCreateDT, @HSave)", dbConn);


                    cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);
                    cmd.Parameters.AddWithValue("@HCourseID", strHID[i]);
                    cmd.Parameters.AddWithValue("@HTMaterialID", MyQueryHCTM["HTMaterialID"].ToString());
                    cmd.Parameters.AddWithValue("@HSort", MyQueryHCTM["HSort"].ToString());
                    cmd.Parameters.AddWithValue("@HTMaterial", MyQueryHCTM["HTMaterial"].ToString());
                    cmd.Parameters.AddWithValue("@HStatus", "1");
                    cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@HSave", "0");

                    cmd.ExecuteNonQuery();
                }
                MyQueryHCTM.Close();
                #endregion
            }

        }

        string strSelHC = "SELECT * FROM HCourse AS a left join HCourse_T AS b ON a.HCTemplateID=b.HID WHERE a.HCBatchNum='" + LB_HCBatchNum.Text + "' AND a.HStatus='1'";

        dbCmd = new SqlCommand(strSelHC, dbConn);

        SqlDataReader MyQueryHC = dbCmd.ExecuteReader();


        //檢覈內容名稱
        BindListControls("SELECT HID, (HExamContentName+'-'+(CASE HExamType WHEN 1 THEN '筆試' WHEN 2 THEN '實作' WHEN '3' THEN '試教' ELSE '' END)) AS HExamContentName, HStatus  FROM HExamContent WHERE HStatus=1", "HExamContentName", "HID", "-請選擇-", true, DDL_HExamContentID);

        DDL_HOSystem.DataBind();
        LBox_HRSystem.DataBind();
        LBox_HNRequirement.DataBind();
        DDL_HSCourse.DataBind();
        LBox_HIRestriction.DataBind();
        DDL_HExamContentID.DataBind();
        DDL_HTeacherClass.DataBind();
        DDL_HTearcherLV.DataBind();

        if (MyQueryHC.Read())
        {
            LB_CopyPage.Text = "True";

            #region 1221修-判斷審核狀態顯示按鈕/開放功能

            LB_HVerifyStatus.Text = MyQueryHC["HVerifyStatus"].ToString();
            string VerifyStatus = MyQueryHC["HVerifyStatus"].ToString();

           
            #endregion


            DDL_HCourseTemplate.SelectedValue = MyQueryHC["HCTemplateID"].ToString();
            TB_HCourseName.Text = MyQueryHC["HCourseName"].ToString();

            string[] gHTeacherName = MyQueryHC["HTeacherName"].ToString().Split(',');
            for (int i = 0; i < gHTeacherName.Length - 1; i++)
            {
                for (int j = 0; j < LBox_HTeacherName.Items.Count; j++)
                {
                    if (gHTeacherName[i].ToString() == LBox_HTeacherName.Items[j].Value)
                    {
                        LBox_HTeacherName.Items[j].Selected = true;
                    }
                }
            }

            DDL_HSCourse.SelectedValue = !string.IsNullOrEmpty(MyQueryHC["HSCourseID"].ToString()) ? MyQueryHC["HSCourseID"].ToString() : "0";

            DDL_HType.SelectedValue = MyQueryHC["HType"].ToString();

            SqlDataReader QueryHCourse_Class = SQLdatabase.ExecuteReader("SELECT HID, HType FROM HCourse_Class where HID='" + MyQueryHC["HType"].ToString() + "'");
            if (QueryHCourse_Class.Read())
            {
                RBL_TestCourse.SelectedValue = QueryHCourse_Class["HType"].ToString() == "2" ? "1" : "0";
            }
            QueryHCourse_Class.Close();


            //WE20241206_上課地點會先帶出原上課地點的資訊
            string[] gHOCPlace = gOriHOCPlace.Split(',');
            for (int i = 0; i < gHOCPlace.Length; i++)
            {
                for (int j = 0; j < LBox_HOCPlace.Items.Count; j++)
                {
                    if (gHOCPlace[i].ToString() == LBox_HOCPlace.Items[j].Value)
                    {
                        LBox_HOCPlace.Items[j].Selected = true;
                    }
                }
            }

            //WA20250611_新增HBookByDateYN欄位
            RBL_HBookByDateYN.SelectedValue = !string.IsNullOrEmpty(MyQueryHC["HBookByDateYN"].ToString()) ? MyQueryHC["HBookByDateYN"].ToString() : "0";


            DDL_HOCPlace.SelectedValue = "0";//已無使用

            //WE20241206_上課日期不複製
            //TB_HDateRange.Text = MyQueryHC["HDateRange"].ToString();
            TB_HSTime.Text = MyQueryHC["HSTime"].ToString();
            TB_HETime.Text = MyQueryHC["HETime"].ToString();

            DDL_HCourseType.SelectedValue = string.IsNullOrEmpty(MyQueryHC["HCourseType"].ToString()) ? "0" : MyQueryHC["HCourseType"].ToString();

            CKE_HRNContent.Text = HttpUtility.HtmlDecode(MyQueryHC["HRNContent"].ToString());

            //課程圖片
            if (!string.IsNullOrEmpty(MyQueryHC["HImg"].ToString()))
            {
                IMG_Pic.ImageUrl = CourseImg + MyQueryHC["HImg"].ToString();
                LB_OldPic.Text = MyQueryHC["HImg"].ToString();
            }
            else
            {
                Btn_Del.Visible = false;
                IMG_Pic.Visible = false;
                IMG_Pic.Width = 0;
            }

            #region  課程檔案
            if (!string.IsNullOrEmpty(MyQueryHC["HBudget"].ToString()))
            {
                if (MyQueryHC["HBudget"].ToString() == "1")
                {
                    HBudgetTable.Visible = true;
                    if (!string.IsNullOrEmpty(MyQueryHC["HBudgetTable"].ToString()))
                    {
                        LBtn_HBudgetTableDel.Visible = true;
                        HL_HBudgetTable.Text = MyQueryHC["HBudgetTable"].ToString();
                        HL_HBudgetTable.NavigateUrl = CourseFile + MyQueryHC["HBudgetTable"].ToString();
                        LB_HBudgetTable.Text = MyQueryHC["HBudgetTable"].ToString();
                    }
                }
                else
                {
                    HBudgetTable.Visible = false;
                }
            }

            if (!string.IsNullOrEmpty(MyQueryHC["BCSchedule"].ToString()))
            {
                LBtn_BCScheduleDel.Visible = true;
                HL_BCSchedule.Text = MyQueryHC["BCSchedule"].ToString();
                HL_BCSchedule.NavigateUrl = CourseFile + MyQueryHC["BCSchedule"].ToString();
                LB_BCSchedule.Text = MyQueryHC["BCSchedule"].ToString();
            }

            if (!string.IsNullOrEmpty(MyQueryHC["BECSchedule"].ToString()))
            {
                LBtn_BECScheduleDel.Visible = true;
                HL_BECSchedule.Text = MyQueryHC["BECSchedule"].ToString();
                HL_BECSchedule.NavigateUrl = CourseFile + MyQueryHC["BECSchedule"].ToString();
                LB_BECSchedule.Text = MyQueryHC["BECSchedule"].ToString();
            }

            if (!string.IsNullOrEmpty(MyQueryHC["ICRecord"].ToString()))
            {
                LBtn_ICRecordDel.Visible = true;
                HL_ICRecord.Text = MyQueryHC["ICRecord"].ToString();
                HL_ICRecord.NavigateUrl = CourseFile + MyQueryHC["ICRecord"].ToString();
                LB_ICRecord.Text = MyQueryHC["ICRecord"].ToString();
            }

            if (!string.IsNullOrEmpty(MyQueryHC["DPosition"].ToString()))
            {
                LBtn_DPositionDel.Visible = true;
                HL_DPosition.Text = MyQueryHC["DPosition"].ToString();
                HL_DPosition.NavigateUrl = CourseFile + MyQueryHC["DPosition"].ToString();
                LB_DPosition.Text = MyQueryHC["DPosition"].ToString();
            }

            if (!string.IsNullOrEmpty(MyQueryHC["CPosition"].ToString()))
            {
                LBtn_CPositionDel.Visible = true;
                HL_CPosition.Text = MyQueryHC["CPosition"].ToString();
                HL_CPosition.NavigateUrl = CourseFile + MyQueryHC["CPosition"].ToString();
                LB_CPosition.Text = MyQueryHC["CPosition"].ToString();
            }

            if (!string.IsNullOrEmpty(MyQueryHC["TPosition"].ToString()))
            {
                LBtn_TPositionDel.Visible = true;
                HL_TPosition.Text = MyQueryHC["TPosition"].ToString();
                HL_TPosition.NavigateUrl = CourseFile + MyQueryHC["TPosition"].ToString();
                LB_TPosition.Text = MyQueryHC["TPosition"].ToString();
            }

            #endregion

            TB_HBudgetType.Text = MyQueryHC["HBudgetType"].ToString();


            DDL_HOSystem.SelectedValue = MyQueryHC["HOSystem"].ToString();
            //LBox_HRSystem.SelectedValue = MyQueryHC["HRSystem"].ToString();
            string[] gHRSystem = MyQueryHC["HRSystem"].ToString().Split(',');
            for (int i = 0; i < gHRSystem.Length - 1; i++)
            {
                for (int j = 0; j < LBox_HRSystem.Items.Count; j++)
                {
                    if (gHRSystem[i].ToString() == LBox_HRSystem.Items[j].Value)
                    {
                        LBox_HRSystem.Items[j].Selected = true;
                    }
                }
            }
            LB_HRSystem.Text = MyQueryHC["HRSystem"].ToString();

            RBL_HNLCourse.SelectedValue = MyQueryHC["HNLCourse"].ToString();
            RBL_HNGuide.SelectedValue = MyQueryHC["HNGuide"].ToString();
            RBL_HNFull.SelectedValue = MyQueryHC["HNFull"].ToString();
            RBL_HSerial.SelectedValue = string.IsNullOrEmpty(MyQueryHC["HSerial"].ToString()) ? "0" : MyQueryHC["HSerial"].ToString();

            //WA20231129_加入
            if (MyQueryHC["HDContinous"].ToString() == "True")
            {
                RBL_Continuous.SelectedValue = "1";
            }
            else
            {
                RBL_Continuous.SelectedValue = "0";
            }

            //WA20231207_加入判斷課程日期要用哪一個多選還是區間
            if (RBL_Continuous.SelectedValue == "0")  //非連續
            {
                TB_HDateRange.CssClass = "form-control datemultipicker";
            }
            else
            {
                TB_HDateRange.CssClass = "form-control daterange";
            }


            SerialCourse.Visible = RBL_HSerial.SelectedValue == "1" ? true : false;
            DDL_HSCourse.Enabled = RBL_HSerial.SelectedValue == "1" ? true : false;

            //WA20231102_複製開課會抓新的換課/地點/身分的期限
            SqlDataReader QueryHCDeadline = SQLdatabase.ExecuteReader("SELECT HID, HCDeadline FROM HCDeadline WHERE HID=1");
            while (QueryHCDeadline.Read())
            {
                TB_HCDeadline.Text = QueryHCDeadline["HCDeadline"].ToString();
            }
            QueryHCDeadline.Close();

            //EA20231030_課程報名截止日
            if (RBL_HSerial.SelectedValue == "0")
            {
                LB_HCDeadlineDayTitle.Text = "課程開始日前";
            }
            else
            {
                LB_HCDeadlineDayTitle.Text = "課程結束日後";
            }

            if (!string.IsNullOrEmpty(MyQueryHC["HCDeadlineDay"].ToString()))
            {
                TB_HCDeadlineDay.Text = MyQueryHC["HCDeadlineDay"].ToString();
            }
            else
            {
                TB_HCDeadlineDay.Text = "0";
            }

            DDL_HSCourse.Enabled = RBL_HSerial.SelectedValue == "1" ? true : false;
            ListItem HBudget = RBL_HBudget.Items.FindByValue(MyQueryHC["HBudget"].ToString());
            if (HBudget != null)
            {
                RBL_HBudget.SelectedValue = MyQueryHC["HBudget"].ToString();
            }

            ListItem HLodging = RBL_HLodging.Items.FindByValue(MyQueryHC["HLodging"].ToString());
            if (HLodging != null)
            {
                RBL_HLodging.SelectedValue = MyQueryHC["HLodging"].ToString();
            }

            string[] gHNRequirement = MyQueryHC["HNRequirement"].ToString().Split(',');
            for (int i = 0; i < gHNRequirement.Length - 1; i++)
            {
                for (int j = 0; j < LBox_HNRequirement.Items.Count; j++)
                {
                    if (gHNRequirement[i].ToString() == LBox_HNRequirement.Items[j].Value)
                    {
                        LBox_HNRequirement.Items[j].Selected = true;
                    }
                }
            }
            LB_HNRequirement.Text = MyQueryHC["HNRequirement"].ToString();

            string[] gHTeam = MyQueryHC["HTeam"].ToString().Split(',');
            for (int i = 0; i < gHTeam.Length - 1; i++)
            {
                for (int j = 0; j < LBox_HTeam.Items.Count; j++)
                {
                    if (gHTeam[i].ToString() == LBox_HTeam.Items[j].Value)
                    {
                        LBox_HTeam.Items[j].Selected = true;
                    }
                }
            }


            string[] gHQuestionID = MyQueryHC["HQuestionID"].ToString().Split(',');
            for (int i = 0; i < gHQuestionID.Length - 1; i++)
            {
                for (int j = 0; j < LBox_HQuestionID.Items.Count; j++)
                {
                    if (gHQuestionID[i].ToString() == LBox_HQuestionID.Items[j].Value)
                    {
                        LBox_HQuestionID.Items[j].Selected = true;
                    }
                }
            }

            //string[] gHTMaterialID = MyQueryHC["HTMaterialID"].ToString().Split(',');
            //for (int i = 0; i < gHTMaterialID.Length - 1; i++)
            //{
            //    for (int j = 0; j < LBox_HTMaterialID.Items.Count; j++)
            //    {
            //        if (gHTMaterialID[i].ToString() == LBox_HTMaterialID.Items[j].Value)
            //        {
            //            LBox_HTMaterialID.Items[j].Selected = true;
            //        }
            //    }
            //}

            DDL_HPMethod.SelectedValue = MyQueryHC["HPMethod"].ToString();

            //220819-顯示轉為金額
            if (!string.IsNullOrEmpty(MyQueryHC["HBCPoint"].ToString()))
            {
                TB_HBCPoint.Text = (Convert.ToInt32(MyQueryHC["HBCPoint"].ToString()) * 10).ToString();
            }
            RBL_HSGList.SelectedValue = MyQueryHC["HSGList"].ToString();

            string[] gHIRestriction = MyQueryHC["HIRestriction"].ToString().Split(',');
            for (int i = 0; i < gHIRestriction.Length - 1; i++)
            {
                for (int j = 0; j < LBox_HIRestriction.Items.Count; j++)
                {
                    if (gHIRestriction[i].ToString() == LBox_HIRestriction.Items[j].Value)
                    {
                        LBox_HIRestriction.Items[j].Selected = true;
                    }
                }
            }
            LB_HIRestriction.Text = MyQueryHC["HIRestriction"].ToString();
            TB_HRemark.Text = HttpUtility.HtmlDecode(MyQueryHC["HRemark"].ToString());
            TB_HContentTitle.Text = HttpUtility.HtmlDecode(MyQueryHC["HContentTitle"].ToString());
            CKE_HContent.Text = HttpUtility.HtmlDecode(MyQueryHC["HContent"].ToString());

            //EA20240618_新增成績計算方式、考卷、督導、出席率標準、檢覈成績通過標準
            DDL_HGradeCalculation.SelectedValue = !string.IsNullOrEmpty(MyQueryHC["HGradeCalculation"].ToString()) ? MyQueryHC["HGradeCalculation"].ToString() : "0";
            string[] gHExamPaperID = MyQueryHC["HExamPaperID"].ToString().Split(',');
            for (int i = 0; i < gHExamPaperID.Length - 1; i++)
            {
                for (int j = 0; j < LBox_HExamPaperID.Items.Count; j++)
                {
                    if (gHExamPaperID[i].ToString() == LBox_HExamPaperID.Items[j].Value)
                    {
                        LBox_HExamPaperID.Items[j].Selected = true;
                    }
                }
            }

            string[] gHSupervise = MyQueryHC["HSupervise"].ToString().Split(',');
            for (int i = 0; i < gHSupervise.Length - 1; i++)
            {
                for (int j = 0; j < LBox_HSupervise.Items.Count; j++)
                {
                    if (gHSupervise[i].ToString() == LBox_HSupervise.Items[j].Value)
                    {
                        LBox_HSupervise.Items[j].Selected = true;
                    }
                }
            }
            TB_HAttRateStandard.Text = !string.IsNullOrEmpty(MyQueryHC["HAttRateStandard"].ToString()) ? MyQueryHC["HAttRateStandard"].ToString() : "";
            TB_HExamPassStandard.Text = !string.IsNullOrEmpty(MyQueryHC["HExamPassStandard"].ToString()) ? MyQueryHC["HExamPassStandard"].ToString() : "";

            //EA20250510_加入檢覈內容名稱、報名人數上限、通過後成為的講師類別、通過後成為的講師層級分類
            DDL_HExamContentID.SelectedValue = string.IsNullOrEmpty(MyQueryHC["HExamContentID"].ToString()) ? "0" : MyQueryHC["HExamContentID"].ToString();
            TB_HParticipantLimit.Text = !string.IsNullOrEmpty(MyQueryHC["HParticipantLimit"].ToString()) ? MyQueryHC["HParticipantLimit"].ToString() : "";
            DDL_HTeacherClass.SelectedValue = string.IsNullOrEmpty(MyQueryHC["HTeacherClass"].ToString()) ? "0" : MyQueryHC["HTeacherClass"].ToString();
            DDL_HTearcherLV.SelectedValue = string.IsNullOrEmpty(MyQueryHC["HTearcherLV"].ToString()) ? "0" : MyQueryHC["HTearcherLV"].ToString();




        }

        MyQueryHC.Close();

        #region 各Tab SDS.DataBind()
        //EE20241120_加入排序欄位；依排序欄位顯示
        SDS_HCourseMaterial.SelectCommand = "SELECT * FROM  (( SELECT HCMName, HCMaterial, HCMLink, HSave, HSort  FROM HCourseMaterial where HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave, HSort) EXCEPT ( SELECT HCMName, HCMaterial, HCMLink, HSave, HSort FROM HCourseMaterial_T where HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave, HSort))AS A ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC  ";
        //Rpt_HCourseMaterial.DataBind();


        //20220821體系護持工作項目改為可修改組長
        SDS_HTodoList.SelectCommand = "SELECT HGroupName, HTask, HTaskContent, HGroupLeaderID, HSave, HTaskNum, HExamStaff FROM HTodoList where HCBatchNum='" + LB_HCBatchNum.Text + "' AND HStatus=1 GROUP BY HCBatchNum, HGroupName, HTask, HTaskContent, HGroupLeaderID,HSave, HTaskNum, HExamStaff ORDER BY HGroupName ASC";
        //Rpt_HTodoList.DataBind();

        //EA20250409_講師教材設定
        SDS_HTMaterialDetail.SelectCommand = "SELECT HTMaterialID, HSort, HTMaterial, HSave FROM HCourseTMaterial WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HStatus='1' GROUP BY HTMaterialID, HSort, HTMaterial, HSave ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
        //Rpt_HTMaterialDetail.DataBind();

        #endregion


        string strDelHC = "DELETE FROM HCourse WHERE HSave='0' AND HCBatchNum ='" + LB_HCBatchNum.Text + "'";
        dbCmd = new SqlCommand(strDelHC, dbConn);
        dbCmd.ExecuteNonQuery();

        dbConn.Close();
        dbCmd.Cancel();
    }
    #endregion

    #region 刪除功能
    protected void LBtn_Del_Click(object sender, EventArgs e)
    {

        LinkButton LBtn_Del = sender as LinkButton;
        string Del_CA = LBtn_Del.CommandArgument;

        LB_HID.Text = Del_CA;

        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();


        string[] strHID = LB_HID.Text.Trim(',').Split(',');
        for (int i = 0; i < strHID.Length; i++)
        {
            string strUpdHC_T = "update HCourse set HStatus='0' where HID='" + strHID[i] + "'";
            dbCmd = new SqlCommand(strUpdHC_T, dbConn);
            dbCmd.ExecuteNonQuery();

            string strUpdHCM_T = "update HCourseMaterial set HStatus='0' where HID='" + strHID[i] + "'";
            dbCmd = new SqlCommand(strUpdHCM_T, dbConn);
            dbCmd.ExecuteNonQuery();

            string strUpdHTL_T = "update HTodoList set HStatus='0' where HID='" + strHID[i] + "'";
            dbCmd = new SqlCommand(strUpdHTL_T, dbConn);
            dbCmd.ExecuteNonQuery();

            string strUpdHCE_T = "update HCourseEvaluation set HStatus='0' where HID='" + strHID[i] + "'";
            dbCmd = new SqlCommand(strUpdHCE_T, dbConn);
            dbCmd.ExecuteNonQuery();

            string strUpdHCTM_T = "update HCourseTMaterial set HStatus='0' where HID='" + strHID[i] + "'";
            dbCmd = new SqlCommand(strUpdHCTM_T, dbConn);
            dbCmd.ExecuteNonQuery();

            //2023/03/08刪除購物車中已退審或已刪除的課程
            string strDelHC = "DELETE FROM HShoppingCart WHERE HCourseID IN ( SELECT HID FROM HCourse WHERE HID='" + strHID[i] + "' AND (HVerifyStatus ='1' OR HStatus='0'))";
            dbCmd = new SqlCommand(strDelHC, dbConn);
            dbCmd.ExecuteNonQuery();

        }


        dbConn.Close();
        dbCmd.Cancel();

        Response.Write("<script>alert('刪除成功!');window.location.href='HCourse_Edit.aspx';</script>");


    }
    #endregion

    #region 資料繫結
    protected void Rpt_HC_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //HVerifyStatus→99:尚未審核、0審核中、1退審、2審核已通過、3核准不通過、4.核准已通過
        string VerifyStatus = ((Label)e.Item.FindControl("LB_HVerifyStatus")).Text;
        ((Label)e.Item.FindControl("LB_HVerifyStatus")).Text = VerifyStatus == "1" ? "退審" : VerifyStatus == "2" ? "審核已通過" : VerifyStatus == "3" ? "核准不通過" : VerifyStatus == "4" ? "核准已通過" : VerifyStatus == "99" ? "尚未送審" : "審核中";
        ((Label)e.Item.FindControl("LB_HVerifyStatus")).CssClass = VerifyStatus == "1" ? "badge badge-warning" : VerifyStatus == "2" ? "badge badge-info" : VerifyStatus == "3" ? "badge badge-danger" : VerifyStatus == "4" ? "badge badge-success" : VerifyStatus == "99" ? "badge badge-default" : "badge badge-outline-dark";

        ((Label)e.Item.FindControl("LB_HTeacherName")).Text = ((Label)e.Item.FindControl("LB_HTeacherName")).Text.TrimEnd(',');



    }
    #endregion

    #region 學員課程教材
    protected void Rpt_HCourseMaterial_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ((FileUpload)e.Item.FindControl("FU_HCMaterial")).Visible = false;
        ((HyperLink)e.Item.FindControl("HL_HCMaterial")).Visible = true;
        ((HyperLink)e.Item.FindControl("HL_HCMaterial")).NavigateUrl = "";
        ((HyperLink)e.Item.FindControl("HL_HCMaterial")).Target = "_blank";
    }

    #region 新增功能
    protected void LBtn_HCourseMaterial_add_Click(object sender, EventArgs e)
    {
        #region 必填判斷
        string alert;
        if (!ValidateRequiredFields(out alert))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('" + alert + "');", true);
            return;
        }

        if (TB_HCMName.Text.Trim() == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('請輸入教材名稱');", true);
            return;
        }
        #endregion


        //建立日期;系統時間
        //DateTime gDate = DateTime.Now;
        string gDay = DateTime.Now.Day.ToString();
        string gMonth = DateTime.Now.Month.ToString();
        string gYear = DateTime.Now.Year.ToString();
        string gDate = System.DateTime.Now.ToString("yyyy/MM/dd");

        string gHCMFileTemp = "~/uploads/CourseMaterialTemp/";
        string gHCMFileRoot = "~/uploads/CourseMaterial/";
        string gHCMFileName = "";//主檔名
        string gHCMFileExtension = "";//副檔名

        // Directory.CreateDirectory(gHCMFileRoot);
        bool fileIsValid = false;

        if (FU_HCMaterial.HasFile)
        {
            //取得上傳檔案大小
            int fileSize = FU_HCMaterial.PostedFile.ContentLength;
            if (fileSize > 200 * 1024 * 1024) //200MB
            {
                //Response.Write("<script>alert('只能上傳mp4、wmv或avi檔哦~~!');</script>");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('單一教材上限為200MB哦!');", true);
                return;
            }
            else
            {
                //取得副檔名
                gHCMFileExtension = Path.GetExtension(FU_HCMaterial.FileName);

                String[] restrictExtension = { ".pdf", ".PDF", ".mp3", ".MP3" };
                foreach (string i in restrictExtension)
                {
                    if (gHCMFileExtension == i)
                    {
                        fileIsValid = true;
                        break;
                    }
                }

                if (fileIsValid)
                {
                    //檔名
                    gHCMFileName = Path.GetFileNameWithoutExtension(FU_HCMaterial.FileName) + "_" + DateTime.Now.ToString("yyMMddssff") + gHCMFileExtension;
                    //gHCMFileName = TB_HCMName.Text.Trim() + "_" + DateTime.Now.ToString("yyMMddssff") + gHCMFileExtension;

                    FU_HCMaterial.SaveAs(Server.MapPath(gHCMFileTemp) + gHCMFileName);
                }
                else
                {
                    Response.Write("<script>alert('只能上傳PDF、mp3檔哦~~!');</script>");
                    return;
                }
            }
        }
        else if (FU_HCMaterial.HasFile == false && string.IsNullOrEmpty(TB_HCMLink.Text))
        {
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert", "<script>alert('請先上傳教材喔!');</script>");
            Response.Write("<script>alert('請上傳教材或影片連結哦~!');</script>");
            return;
        }

        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        //if (LB_HID.Text.Trim() == "")
        //{
        string HID = "";
        string gLBox_HRSystem = "", gLBox_HNRequirement = "", gLBox_HTeam = "", gLBox_HIRestriction = "", gLBox_HTeacherName = "", gLBox_HQuestionID = "", gLBox_HTMaterialID = "", gLBox_HExamPaperID = "", gLBox_HSupervise = "";

        #region 課程名稱符號轉換
        string[] transf = { "＿", "－", "-" };

        for (int i = 0; i < transf.Length; i++)
        {
            if (TB_HCourseName.Text.IndexOf(transf[i]) >= 0)
            {
                TB_HCourseName.Text = TB_HCourseName.Text.Replace(transf[i], "_");
            }
        }

        if (TB_HCourseName.Text.IndexOf("(") >= 0)
        {
            TB_HCourseName.Text = TB_HCourseName.Text.Replace("(", "（");
        }
        if (TB_HCourseName.Text.IndexOf(")") >= 0)
        {
            TB_HCourseName.Text = TB_HCourseName.Text.Replace(")", "）");
        }

        #endregion

        #region  重複判斷
        int reHOCPlace = 0;
        string stHOCPlace = null;
        foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
        {
            if (LBoxHOCPlace.Selected == true)
            {
                SqlDataReader MyEBF = SQLdatabase.ExecuteReader("SELECT a.HOCPlace, b.HPlaceName FROM HCourse AS a LEFT JOIN HPlace AS b ON a.HOCPlace=b.HID WHERE a.HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' AND a.HCourseName='" + TB_HCourseName.Text + "' AND a.HOCPlace='" + LBoxHOCPlace.Value + "' AND a.HDateRange='" + TB_HDateRange.Text + "' AND a.HStatus='1' AND a.HSave='1' AND NOT EXISTS ( SELECT HCourseName FROM HCourse  tb WHERE  a.HCBatchNum ='" + LB_HCBatchNum.Text + "' )");

                if (MyEBF.Read())
                {
                    reHOCPlace += 1;
                    stHOCPlace += MyEBF["HPlaceName"].ToString() + ",";
                }
                MyEBF.Close();


            }
        }

        if (reHOCPlace != 0)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('上課地點：" + stHOCPlace.TrimEnd(',') + "已重複開課，再請協助確認，謝謝!');", true);
            //Response.Write("<script>alert('上課地點：" + stHOCPlace.TrimEnd(',') + "已重複開課，再請協助確認，謝謝!');</script>");
            return;
        }
        #endregion


        #region--圖片上傳
        string picname = "";
        if (LB_OldPic.Text != "" && FU_HImg.HasFile == true)
        {
            //上傳圖片
            Upload_Pic();

            picname = LB_Pic.Text;
        }
        else if (LB_OldPic.Text != "" && FU_HImg.HasFile == false)
        {
            picname = LB_OldPic.Text;
        }
        else if (LB_OldPic.Text == "" && FU_HImg.HasFile == true)
        {
            //上傳圖片
            Upload_Pic();
            picname = LB_Pic.Text;
        }
        //else if (LB_OldPic.Text == "" && FU_HImg.HasFile == false)
        //{
        //	Response.Write("<script>alert('請選擇圖片上傳');</script>");
        //	return;
        //}
        #endregion


        #region 上傳檔案
        if (FU_BCSchedule.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_BCSchedule.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_BCSchedule.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_BCSchedule.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }


        if (FU_BECSchedule.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_BECSchedule.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_BECSchedule.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_BECSchedule.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        if (FU_ICRecord.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_ICRecord.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_ICRecord.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_ICRecord.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        if (FU_DPosition.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_DPosition.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_DPosition.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_DPosition.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        if (FU_CPosition.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_CPosition.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_CPosition.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_CPosition.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        if (FU_TPosition.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_TPosition.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_TPosition.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_TPosition.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        #endregion


        //220819-基本費用(元)存入DB要先轉換成點數概念(1元=10點)
        int BasicPrice = 0;

        if (string.IsNullOrEmpty(TB_HBCPoint.Text.Trim()))
        {
            Response.Write("<script>alert('請先確認已有選擇課程範本哦~!');</script>");
            return;
        }
        else
        {
            BasicPrice = Convert.ToInt32(TB_HBCPoint.Text.Trim()) / 10;
        }



        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Trim(',').Split(',');

            //221018  -上課地點多選
            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    for (int i = 0; i < strHID.Length; i++)
                    {
                        //220823  --課程編輯不會update到審核紀錄
                        //WE20231129_加入課程連續與否
                        string strUpdHC = "update HCourse set HEnableSystem='', HApplySystem='', HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "', HCourseName='" + TB_HCourseName.Text + "', HTeacherName='" + gLBox_HTeacherName + "', HOCPlace='" + LBoxHOCPlace.Value + "', HDateRange='" + TB_HDateRange.Text + "', HSTime='" + TB_HSTime.Text + "', HETime='" + TB_HETime.Text + "', HType='" + DDL_HType.SelectedValue + "', HOSystem='" + DDL_HOSystem.SelectedValue + "', HRSystem='" + LB_HRSystem.Text + "', HNLCourse='" + RBL_HNLCourse.SelectedValue + "', HNGuide='" + RBL_HNGuide.SelectedValue + "', HNFull='" + RBL_HNFull.SelectedValue + "', HNRequirement='" + LB_HNRequirement.Text + "', HTeam='" + gLBox_HTeam + "',  HQuestionID='" + gLBox_HQuestionID + "', HPMethod='" + DDL_HPMethod.SelectedValue + "', HBCPoint='" + BasicPrice + "', HSGList='" + RBL_HSGList.SelectedValue + "', HIRestriction='" + LB_HIRestriction.Text + "', HRemark=N'" + HttpUtility.HtmlEncode(TB_HRemark.Text.Trim()) + "', HContentTitle=N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', HContent=N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', HRNContent=N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', HStatus='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1',HImg='" + picname + "', BCSchedule='" + LB_BCSchedule.Text + "' , BECSchedule='" + LB_BECSchedule.Text + "', ICRecord='" + LB_ICRecord.Text + "', DPosition='" + LB_DPosition.Text + "', CPosition='" + LB_CPosition.Text + "', TPosition='" + LB_TPosition.Text + "',HBudget='" + RBL_HBudget.SelectedValue + "', HBudgetTable='" + LB_HBudgetTable.Text + "', HLodging='" + RBL_HLodging.SelectedValue + "', HTMaterialID='" + gLBox_HTMaterialID + "', HCDeadline ='" + TB_HCDeadline.Text.Trim() + "', HCDeadlineDay ='" + TB_HCDeadlineDay.Text.Trim() + "',  HSerial= '" + RBL_HSerial.SelectedValue + "', HDContinous='" + RBL_Continuous.SelectedValue + "', HBudgetType='" + TB_HBudgetType.Text + "', HCourseType='" + DDL_HCourseType.SelectedValue + "', HAxisYN='" + RBL_HAxisYN.SelectedValue + "', HAxisClass='" + DDL_HAxisClass.SelectedValue + "', HExamSubject='" + DDL_HExamSubject.SelectedValue + "', HGradeCalculation='" + DDL_HGradeCalculation.SelectedValue + "', HExamPaperID='" + gLBox_HExamPaperID + "',  HSupervise='" + gLBox_HSupervise + "', HAttRateStandard='" + TB_HAttRateStandard.Text + "', HExamPassStandard='" + TB_HExamPassStandard.Text + "', HExamContentID='" + DDL_HExamContentID.SelectedValue + "', HParticipantLimit='" + TB_HParticipantLimit.Text + "', HTeacherClass='" + DDL_HTeacherClass.SelectedValue + "', HTearcherLV='" + DDL_HTearcherLV.SelectedValue + "' where  HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'";
                        dbCmd = new SqlCommand(strUpdHC, dbConn);
                        dbCmd.ExecuteNonQuery();
                        //Response.Write(strUpdHC + "<br/>");

                    }

                    SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HCourseNum FROM HCourse WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'");
                    if (!dr.Read())
                    {
                        string strInsHC = "IF EXISTS(SELECT HCourseNum FROM HCourse WHERE SUBSTRING(HCourseNum,2,8) = CONVERT(nvarchar, getdate(), 112)) " +
   "BEGIN " +
       "insert into HCourse (HEnableSystem, HApplySystem, HCTemplateID, HCBatchNum, HCourseNum, HCourseName, HTeacherName, HOCPlace, HDateRange, HSTime, HETime, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam,  HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HRNContent, HVerifyNum, HApplicant, HApplyTime, HVerifyTime, HVerifyStatus, HStatus, HCreate, HCreateDT,HModify, HModifyDT,  HSave,HImg,BCSchedule, BECSchedule,  ICRecord, DPosition, CPosition, TPosition, HBudget, HBudgetTable, HSCourseID, HLodging, HTMaterialID, HCDeadlineDay, HSerial, HDContinous, HBudgetType, HCourseType, HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID,  HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV) values ('', '', '" + DDL_HCourseTemplate.SelectedValue + "', '" + LB_HCBatchNum.Text + "', 'C' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HCourseNum), 12)) FROM HCourse) + 1), '" + TB_HCourseName.Text + "', '" + gLBox_HTeacherName + "', '" + LBoxHOCPlace.Value + "', '" + TB_HDateRange.Text + "', '" + TB_HSTime.Text + "', '" + TB_HETime.Text + "', '" + DDL_HType.SelectedValue + "','" + DDL_HOSystem.SelectedValue + "', '" + LB_HRSystem.Text + "', '" + RBL_HNLCourse.SelectedValue + "', '" + RBL_HNGuide.SelectedValue + "', '" + RBL_HNFull.SelectedValue + "','" + LB_HNRequirement.Text + "', '" + gLBox_HTeam + "',  '" + gLBox_HQuestionID + "', '" + DDL_HPMethod.SelectedValue + "', '" + BasicPrice + "', '" + RBL_HSGList.SelectedValue + "', '" + LB_HIRestriction.Text + "', N'" + HttpUtility.HtmlEncode(TB_HRemark.Text) + "', N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', '', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '', '99', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',  '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','1','" + picname + "','" + LB_BCSchedule.Text + "','" + LB_BECSchedule.Text + "','" + LB_ICRecord.Text + "','" + LB_DPosition.Text + "','" + LB_CPosition.Text + "','" + LB_TPosition.Text + "','" + RBL_HBudget.SelectedValue + "','" + LB_HBudgetTable.Text + "','" + DDL_HSCourse.SelectedValue + "','" + RBL_HLodging.SelectedValue + "', '" + gLBox_HTMaterialID + "', '" + TB_HCDeadlineDay.Text.Trim() + "','" + RBL_HSerial.SelectedValue + "','" + RBL_Continuous.SelectedValue + "','" + TB_HBudgetType.Text + "','" + DDL_HCourseType.SelectedValue + "', '" + RBL_HAxisYN.SelectedValue + "', '" + DDL_HAxisClass.SelectedValue + "', '" + DDL_HExamSubject.SelectedValue + "', '" + DDL_HGradeCalculation.SelectedValue + "', '" + gLBox_HExamPaperID + "',  '" + gLBox_HSupervise + "', '" + TB_HAttRateStandard.Text + "', '" + TB_HExamPassStandard.Text + "', '" + DDL_HExamContentID.SelectedValue + "', '" + TB_HParticipantLimit.Text + "', '" + DDL_HTeacherClass.SelectedValue + "', '" + DDL_HTearcherLV.SelectedValue + "');SELECT SCOPE_IDENTITY() AS HID " +
   "END " +
   "ELSE " +
   "BEGIN " +
       "insert into HCourse (HEnableSystem, HApplySystem, HCTemplateID, HCBatchNum, HCourseNum, HCourseName, HTeacherName, HOCPlace, HDateRange, HSTime, HETime, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam,  HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HRNContent, HVerifyNum, HApplicant, HApplyTime, HVerifyTime, HVerifyStatus, HStatus, HCreate, HCreateDT,HModify, HModifyDT,  HSave,HImg, BCSchedule, BECSchedule, ICRecord, DPosition, CPosition, TPosition, HBudget, HBudgetTable, HSCourseID, HLodging, HTMaterialID, HCDeadlineDay, HSerial,HDContinous, HBudgetType,HCourseType, HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID,  HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV) values ('', '', '" + DDL_HCourseTemplate.SelectedValue + "', '" + LB_HCBatchNum.Text + "', 'C' + CONVERT(nvarchar, getdate(), 112) + '0001', '" + TB_HCourseName.Text + "', '" + gLBox_HTeacherName + "', '" + LBoxHOCPlace.Value + "', '" + TB_HDateRange.Text + "', '" + TB_HSTime.Text + "', '" + TB_HETime.Text + "', '" + DDL_HType.SelectedValue + "','" + DDL_HOSystem.SelectedValue + "', '" + LB_HRSystem.Text + "', '" + RBL_HNLCourse.SelectedValue + "', '" + RBL_HNGuide.SelectedValue + "', '" + RBL_HNFull.SelectedValue + "','" + LB_HNRequirement.Text + "', '" + gLBox_HTeam + "','" + gLBox_HQuestionID + "', '" + DDL_HPMethod.SelectedValue + "', '" + BasicPrice + "', '" + RBL_HSGList.SelectedValue + "', '" + LB_HIRestriction.Text + "', N'" + HttpUtility.HtmlEncode(TB_HRemark.Text) + "', N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', '', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '', '99', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '1','" + picname + "','" + LB_BCSchedule.Text + "','" + LB_BECSchedule.Text + "','" + LB_ICRecord.Text + "','" + LB_DPosition.Text + "','" + LB_CPosition.Text + "','" + LB_TPosition.Text + "','" + RBL_HBudget.SelectedValue + "','" + LB_HBudgetTable.Text + "','" + DDL_HSCourse.SelectedValue + "','" + RBL_HLodging.SelectedValue + "', '" + gLBox_HTMaterialID + "', '" + TB_HCDeadlineDay.Text.Trim() + "','" + RBL_HSerial.SelectedValue + "','" + RBL_Continuous.SelectedValue + "','" + TB_HBudgetType.Text + "','" + DDL_HCourseType.SelectedValue + "', '" + RBL_HAxisYN.SelectedValue + "', '" + DDL_HAxisClass.SelectedValue + "', '" + DDL_HExamSubject.SelectedValue + "', '" + DDL_HGradeCalculation.SelectedValue + "', '" + gLBox_HExamPaperID + "',  '" + gLBox_HSupervise + "', '" + TB_HAttRateStandard.Text + "', '" + TB_HExamPassStandard.Text + "', '" + DDL_HExamContentID.SelectedValue + "', '" + TB_HParticipantLimit.Text + "', '" + DDL_HTeacherClass.SelectedValue + "', '" + DDL_HTearcherLV.SelectedValue + "');SELECT SCOPE_IDENTITY() AS HID " +
   "END";
                        dbCmd = new SqlCommand(strInsHC, dbConn);
                        //dbCmd.ExecuteNonQuery();
                        HID += dbCmd.ExecuteScalar().ToString() + ",";
                        //dbCmd.Cancel();
                        //Response.Write(strInsHC+"<br/>");
                    }
                    dr.Close();
                }
                else
                {
                    SqlDataReader drDel = SQLdatabase.ExecuteReader("SELECT HCourseNum FROM HCourse WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'");

                    if (drDel.Read())
                    {
                        for (int i = 0; i < strHID.Length; i++)
                        {
                            string strUpdHC_T = "update HCourse set HStatus='0' where HID='" + strHID[i] + "' AND HOCPlace='" + LBoxHOCPlace.Value + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "'";
                            dbCmd = new SqlCommand(strUpdHC_T, dbConn);
                            dbCmd.ExecuteNonQuery();


                            string strUpdHCM_T = "update HCourseMaterial set HStatus='0' where HCourseID='" + strHID[i] + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "'";
                            dbCmd = new SqlCommand(strUpdHCM_T, dbConn);
                            dbCmd.ExecuteNonQuery();

                            string strUpdHTL_T = "update HTodoList set HStatus='0' where HCourseID='" + strHID[i] + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "'";
                            dbCmd = new SqlCommand(strUpdHTL_T, dbConn);
                            dbCmd.ExecuteNonQuery();

                            string strUpdHCE_T = "update HCourseEvaluation set HStatus='0' where HCourseID='" + strHID[i] + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "'";
                            dbCmd = new SqlCommand(strUpdHCE_T, dbConn);
                            dbCmd.ExecuteNonQuery();

                            string strUpdHCTM_T = "update HCourseTMaterial set HStatus='0' where HCourseID='" + strHID[i] + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "'";
                            dbCmd = new SqlCommand(strUpdHCTM_T, dbConn);
                            dbCmd.ExecuteNonQuery();
                        }

                    }
                    drDel.Close();
                }

            }


        }
        if (HID != "")
        {
            LB_HID.Text = HID;
        }

        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Trim(',').Split(',');
            for (int i = 0; i < strHID.Length; i++)
            {
                //EE20241120_SQL語法改為參數方式
                SqlCommand cmd = new SqlCommand("INSERT INTO HCourseMaterial (HCourseID, HCMName, HCMaterial, HCMLink, HStatus, HCreate, HCreateDT, HSave, HCBatchNum, HSort) values (@HCourseID, @HCMName, @HCMaterial, @HCMLink, @HStatus, @HCreate, @HCreateDT, @HSave, @HCBatchNum, @HSort)", con);
                con.Open();
                cmd.Parameters.AddWithValue("@HCourseID", strHID[i]);
                cmd.Parameters.AddWithValue("@HCMName", HttpUtility.HtmlEncode(TB_HCMName.Text.Trim()));
                cmd.Parameters.AddWithValue("@HCMaterial", gHCMFileName);
                cmd.Parameters.AddWithValue("@HCMLink", HttpUtility.HtmlEncode(TB_HCMLink.Text.Trim()));
                cmd.Parameters.AddWithValue("@HStatus", "1");
                cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@HSave", "1");
                cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);
                cmd.Parameters.AddWithValue("@HSort", TB_HSort.Text.Trim());

                cmd.ExecuteNonQuery();
                con.Close();
                cmd.Cancel();
                //string strSelHCM = "Insert Into HCourseMaterial (HCourseID, HCMName, HCMaterial, HCMLink, HStatus, HCreate, HCreateDT, HSave, HCBatchNum) values ('" + strHID[i] + "', '" + TB_HCMName.Text + "', '" + gHCMFileName + "', '" + TB_HCMLink.Text + "', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','0','" + LB_HCBatchNum.Text + "')";
                //dbCmd = new SqlCommand(strSelHCM, dbConn);
                //dbCmd.ExecuteReader();
            }
        }
        dbConn.Close();
        dbCmd.Cancel();

        //EE20241120_加入排序欄位；依排序欄位顯示
        SDS_HCourseMaterial.SelectCommand = "SELECT * FROM  (( SELECT HCMName, HCMaterial, HCMLink, HSave, HSort  FROM HCourseMaterial where HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave, HSort) EXCEPT ( SELECT HCMName, HCMaterial, HCMLink, HSave, HSort FROM HCourseMaterial_T where HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave, HSort))AS A ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC  ";
        //SDS_HCourseMaterial.SelectCommand = "SELECT * FROM  (( SELECT HCMName, HCMaterial, HCMLink, HSave, HSort FROM HCourseMaterial_T where HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave, HSort  ) UNION   ( SELECT HCMName, HCMaterial, HCMLink, HSave, HSort FROM HCourseMaterial where HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave, HSort))AS A ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC ";
        //SDS_HCourseMaterial.SelectCommand = "SELECT  HCMName, HCMaterial, HCMLink, HCBatchNum, HStatus, HSave, HSort FROM HCourseMaterial WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY HCMName, HCMaterial, HCMLink, HCBatchNum, HStatus, HSave, HSort ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC ";
        //SDS_HCourseMaterial.SelectCommand = "SELECT HCMName, HCMaterial, HCMLink, HSave FROM HCourseMaterial where HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave UNION ALL SELECT HCMName, HCMaterial, HCMLink, HSave FROM HCourseMaterial_T where HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave ";
        //SDS_HCourseMaterial.SelectCommand = "SELECT HCMName, HCMaterial, HCMLink, HSave FROM HCourseMaterial_T where HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' UNION ALL SELECT HCMName, HCMaterial, HCMLink, HSave FROM HCourseMaterial where HCourseID='" + LB_HID.Text + "'";

        Rpt_HCourseMaterial.DataBind();


        //清除textbox
        TB_HCMName.Text = "";
        TB_HCMLink.Text = "";
        TB_HSort.Text = "";
    }
    #endregion

    #endregion

    #region 體系護持工作項目

    private DataTable HGroupTable
    {
        get
        {
            if (ViewState["HGroupTable"] == null)
            {
                ViewState["HGroupTable"] = SQLdatabase.ExecuteDataTable("SELECT HID, HGroupName FROM HGroup WHERE HStatus='1'");
            }
            return (DataTable)ViewState["HGroupTable"];
        }
    }


    public static DataTable GetTasksByGroupId(string groupId)
    {
        var dt = new DataTable();
        using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString))
        using (var cmd = new SqlCommand("SELECT HID, HTask FROM HCourseTask WHERE HGroupID = @gid", con))
        {
            cmd.Parameters.AddWithValue("@gid", groupId);
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
        }
        return dt;
    }


    protected void Rpt_HTodoList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            return;

      


        //取得Repeater的DataRowView
        DataRowView gDRV = (DataRowView)e.Item.DataItem;
        SqlDataReader QueryHGroup = SQLdatabase.ExecuteReader("SELECT HID, HGroupName FROM HGroup WHERE HStatus='1'");
        while (QueryHGroup.Read())
        {
            ((DropDownList)e.Item.FindControl("DDL_HGroupName")).Items.Add(new ListItem(QueryHGroup["HGroupName"].ToString(), (QueryHGroup["HID"].ToString())));
        }
        QueryHGroup.Close();
        ((DropDownList)e.Item.FindControl("DDL_HGroupName")).SelectedValue = gDRV["HGroupName"].ToString();

        SqlDataReader QueryHCourseTask = SQLdatabase.ExecuteReader("SELECT HID, HTask, HTaskContent FROM HCourseTask where HGroupID='" + ((DropDownList)e.Item.FindControl("DDL_HGroupName")).SelectedValue + "'");
        while (QueryHCourseTask.Read())
        {
            ((DropDownList)e.Item.FindControl("DDL_HTask")).Items.Add(new ListItem(QueryHCourseTask["HTask"].ToString(), (QueryHCourseTask["HID"].ToString())));
        }
        QueryHCourseTask.Close();
        ((DropDownList)e.Item.FindControl("DDL_HTask")).SelectedValue = gDRV["HTask"].ToString();



        ((DropDownList)e.Item.FindControl("DDL_HGroupName")).SelectedValue = gDRV["HGroupName"].ToString();

        //EA20240623_試務人員名單(多選)
        //WE20250513_暫時註解
        //string[] gHExamStaff = ((Label)e.Item.FindControl("LB_HExamStaff")).Text.Split(',');
        //for (int i = 0; i < gHExamStaff.Length - 1; i++)
        //{
        //    for (int j = 0; j < ((ListBox)e.Item.FindControl("LBox_HExamStaff")).Items.Count; j++)
        //    {
        //        if (gHExamStaff[i].ToString() == ((ListBox)e.Item.FindControl("LBox_HExamStaff")).Items[j].Value)
        //        {
        //            ((ListBox)e.Item.FindControl("LBox_HExamStaff")).Items[j].Selected = true;
        //        }
        //    }
        //}

    }

    #region 組別選單
    protected void DDL_HGroupName_SelectedIndexChanged(object sender, EventArgs e)
    {
        DDL_HTask.Items.Clear();
        DDL_HTask.Enabled = true;
        DDL_HTask.Items.Add(new ListItem("-請選擇-", "0"));
       

        using (SqlConnection localCon = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString))
        using (SqlCommand cmd = new SqlCommand("SELECT HID, HTask FROM HCourseTask WHERE HGroupID = @gid", localCon))
        {
            cmd.Parameters.AddWithValue("@gid", DDL_HGroupName.SelectedValue);
            localCon.Open();
            using (SqlDataReader QueryHCourseTask = cmd.ExecuteReader())
            {
                while (QueryHCourseTask.Read())
                {
                    DDL_HTask.Items.Add(new ListItem(QueryHCourseTask["HTask"].ToString(), (QueryHCourseTask["HID"].ToString())));
                }
            }
        }




        RegScript();//執行js
    }
    #endregion

    #region 任務職稱
    protected void DDL_HTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        SqlDataReader QueryHCourseTask = SQLdatabase.ExecuteReader("SELECT HID, HTask, HTaskContent FROM HCourseTask where HID='" + DDL_HTask.SelectedValue + "'");
        while (QueryHCourseTask.Read())
        {
            TB_HTaskContent.Text = QueryHCourseTask["HTaskContent"].ToString();
        }
        QueryHCourseTask.Close();
        RegScript();//執行js
    }
    #endregion

    #region 體系護持工作項目_新增功能
    protected void LBtn_HTodoList_add_Click(object sender, EventArgs e)
    {
        RegScript();//執行js

        #region 必填判斷

        string alert;
        if (!ValidateRequiredFields(out alert))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('" + alert + "');", true);
            return;
        }

        #endregion

        bool fileIsValid = false;

        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

  
        string HID = "";
        string gLBox_HRSystem = "", gLBox_HNRequirement = "", gLBox_HTeam = "", gLBox_HIRestriction = "", gLBox_HTeacherName = "", gLBox_HQuestionID = "", gLBox_HTMaterialID = "", gLBox_HExamPaperID = "", gLBox_HSupervise = "";

        #region 課程名稱符號轉換
        string[] transf = { "＿", "－", "-" };

        for (int i = 0; i < transf.Length; i++)
        {
            if (TB_HCourseName.Text.IndexOf(transf[i]) >= 0)
            {
                TB_HCourseName.Text = TB_HCourseName.Text.Replace(transf[i], "_");
            }
        }

        if (TB_HCourseName.Text.IndexOf("(") >= 0)
        {
            TB_HCourseName.Text = TB_HCourseName.Text.Replace("(", "（");
        }
        if (TB_HCourseName.Text.IndexOf(")") >= 0)
        {
            TB_HCourseName.Text = TB_HCourseName.Text.Replace(")", "）");
        }

        #endregion

        #region  重複判斷
        int reHOCPlace = 0;
        string stHOCPlace = null;
        foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
        {
            if (LBoxHOCPlace.Selected == true)
            {
                SqlDataReader MyEBF = SQLdatabase.ExecuteReader("SELECT a.HOCPlace, b.HPlaceName FROM HCourse AS a LEFT JOIN HPlace AS b ON a.HOCPlace=b.HID WHERE a.HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' AND a.HCourseName='" + TB_HCourseName.Text + "' AND a.HOCPlace='" + LBoxHOCPlace.Value + "' AND a.HDateRange='" + TB_HDateRange.Text + "' AND a.HStatus='1' AND a.HSave='1' AND NOT EXISTS ( SELECT HCourseName FROM HCourse  tb WHERE  a.HCBatchNum ='" + LB_HCBatchNum.Text + "' )");

                if (MyEBF.Read())
                {
                    reHOCPlace += 1;
                    stHOCPlace += MyEBF["HPlaceName"].ToString() + ",";
                }
                MyEBF.Close();


            }
        }

        if (reHOCPlace != 0)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('上課地點：" + stHOCPlace.TrimEnd(',') + "已重複開課，再請協助確認，謝謝!');", true);
            //Response.Write("<script>alert('上課地點：" + stHOCPlace.TrimEnd(',') + "已重複開課，再請協助確認，謝謝!');</script>");
            return;
        }
        #endregion


        #region--圖片上傳
        string picname = "";
        if (LB_OldPic.Text != "" && FU_HImg.HasFile == true)
        {
            //上傳圖片
            Upload_Pic();

            picname = LB_Pic.Text;
        }
        else if (LB_OldPic.Text != "" && FU_HImg.HasFile == false)
        {
            picname = LB_OldPic.Text;
        }
        else if (LB_OldPic.Text == "" && FU_HImg.HasFile == true)
        {
            //上傳圖片
            Upload_Pic();
            picname = LB_Pic.Text;
        }
        #endregion

        //220819-基本費用(元)存入DB要先轉換成點數概念(1元=10點)
        int BasicPrice = 0;

        if (string.IsNullOrEmpty(TB_HBCPoint.Text.Trim()))
        {
            Response.Write("<script>alert('請先確認已有選擇課程範本哦~!');</script>");
            return;
        }
        else
        {
            BasicPrice = Convert.ToInt32(TB_HBCPoint.Text.Trim()) / 10;
        }

        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Trim(',').Split(',');

            //221018  -上課地點多選
            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    for (int i = 0; i < strHID.Length; i++)
                    {
                        //220823  --課程編輯不會update到審核紀錄
                        //WE20231129_加入課程連續欄位資訊
                        string strUpdHC = "update HCourse set HEnableSystem='', HApplySystem='', HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "', HCourseName='" + TB_HCourseName.Text + "', HTeacherName='" + gLBox_HTeacherName + "', HOCPlace='" + LBoxHOCPlace.Value + "', HDateRange='" + TB_HDateRange.Text + "', HSTime='" + TB_HSTime.Text + "', HETime='" + TB_HETime.Text + "', HType='" + DDL_HType.SelectedValue + "', HOSystem='" + DDL_HOSystem.SelectedValue + "', HRSystem='" + LB_HRSystem.Text + "', HNLCourse='" + RBL_HNLCourse.SelectedValue + "', HNGuide='" + RBL_HNGuide.SelectedValue + "', HNFull='" + RBL_HNFull.SelectedValue + "', HNRequirement='" + LB_HNRequirement.Text + "', HTeam='" + gLBox_HTeam + "',  HQuestionID='" + gLBox_HQuestionID + "', HPMethod='" + DDL_HPMethod.SelectedValue + "', HBCPoint='" + BasicPrice + "', HSGList='" + RBL_HSGList.SelectedValue + "', HIRestriction='" + LB_HIRestriction.Text + "', HRemark=N'" + HttpUtility.HtmlEncode(TB_HRemark.Text.Trim()) + "', HContentTitle=N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', HContent=N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', HRNContent=N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', HStatus='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1',HImg='" + picname + "', BCSchedule='" + LB_BCSchedule.Text + "' , BECSchedule='" + LB_BECSchedule.Text + "', ICRecord='" + LB_ICRecord.Text + "', DPosition='" + LB_DPosition.Text + "', CPosition='" + LB_CPosition.Text + "', TPosition='" + LB_TPosition.Text + "',HBudget='" + RBL_HBudget.SelectedValue + "', HBudgetTable='" + LB_HBudgetTable.Text + "', HLodging='" + RBL_HLodging.SelectedValue + "', HTMaterialID='" + gLBox_HTMaterialID + "', HCDeadline ='" + TB_HCDeadline.Text.Trim() + "', HCDeadlineDay ='" + TB_HCDeadlineDay.Text.Trim() + "', HDContinous='" + RBL_Continuous.SelectedValue + "', HBudgetType='" + TB_HBudgetType.Text + "', HCourseType='" + DDL_HCourseType.SelectedValue + "', HAxisYN='" + RBL_HAxisYN.SelectedValue + "', HAxisClass='" + DDL_HAxisClass.SelectedValue + "', HExamSubject='" + DDL_HExamSubject.SelectedValue + "', HGradeCalculation='" + DDL_HGradeCalculation.SelectedValue + "', HExamPaperID='" + gLBox_HExamPaperID + "',  HSupervise='" + gLBox_HSupervise + "', HAttRateStandard='" + TB_HAttRateStandard.Text + "', HExamPassStandard='" + TB_HExamPassStandard.Text + "', HExamContentID='" + DDL_HExamContentID.SelectedValue + "', HParticipantLimit='" + TB_HParticipantLimit.Text + "', HTeacherClass='" + DDL_HTeacherClass.SelectedValue + "', HTearcherLV='" + DDL_HTearcherLV.SelectedValue + "' where  HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'";
                        dbCmd = new SqlCommand(strUpdHC, dbConn);
                        dbCmd.ExecuteNonQuery();
                    }

                    SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HCourseNum FROM HCourse WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'");
                    if (!dr.Read())
                    {
                        string strInsHC = "IF EXISTS(SELECT HCourseNum FROM HCourse WHERE SUBSTRING(HCourseNum,2,8) = CONVERT(nvarchar, getdate(), 112)) " +
   "BEGIN " +
       "insert into HCourse (HEnableSystem, HApplySystem, HCTemplateID, HCBatchNum, HCourseNum, HCourseName, HTeacherName, HOCPlace, HDateRange, HSTime, HETime, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam,  HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HRNContent, HVerifyNum, HApplicant, HApplyTime, HVerifyTime, HVerifyStatus, HStatus, HCreate, HCreateDT,HModify, HModifyDT,  HSave,HImg,BCSchedule, BECSchedule,  ICRecord, DPosition, CPosition, TPosition, HBudget, HBudgetTable, HSCourseID, HLodging, HTMaterialID, HCDeadlineDay,HSerial,  HDContinous, HBudgetType,HCourseType, HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID,  HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV) values ('', '', '" + DDL_HCourseTemplate.SelectedValue + "', '" + LB_HCBatchNum.Text + "', 'C' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HCourseNum), 12)) FROM HCourse) + 1), '" + TB_HCourseName.Text + "', '" + gLBox_HTeacherName + "', '" + LBoxHOCPlace.Value + "', '" + TB_HDateRange.Text + "', '" + TB_HSTime.Text + "', '" + TB_HETime.Text + "', '" + DDL_HType.SelectedValue + "','" + DDL_HOSystem.SelectedValue + "', '" + LB_HRSystem.Text + "', '" + RBL_HNLCourse.SelectedValue + "', '" + RBL_HNGuide.SelectedValue + "', '" + RBL_HNFull.SelectedValue + "','" + LB_HNRequirement.Text + "', '" + gLBox_HTeam + "',  '" + gLBox_HQuestionID + "', '" + DDL_HPMethod.SelectedValue + "', '" + BasicPrice + "', '" + RBL_HSGList.SelectedValue + "', '" + LB_HIRestriction.Text + "', N'" + HttpUtility.HtmlEncode(TB_HRemark.Text) + "', N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', '', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '', '99', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',  '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','1','" + picname + "','" + LB_BCSchedule.Text + "','" + LB_BECSchedule.Text + "','" + LB_ICRecord.Text + "','" + LB_DPosition.Text + "','" + LB_CPosition.Text + "','" + LB_TPosition.Text + "','" + RBL_HBudget.SelectedValue + "','" + LB_HBudgetTable.Text + "','" + DDL_HSCourse.SelectedValue + "','" + RBL_HLodging.SelectedValue + "', '" + gLBox_HTMaterialID + "', '" + TB_HCDeadlineDay.Text.Trim() + "', '" + RBL_HSerial.SelectedValue + "','" + RBL_Continuous.SelectedValue + "', '" + TB_HBudgetType.Text + "','" + DDL_HCourseType.SelectedValue + "', '" + RBL_HAxisYN.SelectedValue + "', '" + DDL_HAxisClass.SelectedValue + "', '" + DDL_HExamSubject.SelectedValue + "', '" + DDL_HGradeCalculation.SelectedValue + "', '" + gLBox_HExamPaperID + "',  '" + gLBox_HSupervise + "', '" + TB_HAttRateStandard.Text + "', '" + TB_HExamPassStandard.Text + "', '" + DDL_HExamContentID.SelectedValue + "', '" + TB_HParticipantLimit.Text + "', '" + DDL_HTeacherClass.SelectedValue + "', '" + DDL_HTearcherLV.SelectedValue + "');SELECT SCOPE_IDENTITY() AS HID " +
   "END " +
   "ELSE " +
   "BEGIN " +
       "insert into HCourse (HEnableSystem, HApplySystem, HCTemplateID, HCBatchNum, HCourseNum, HCourseName, HTeacherName, HOCPlace, HDateRange, HSTime, HETime, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam,  HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HRNContent, HVerifyNum, HApplicant, HApplyTime, HVerifyTime, HVerifyStatus, HStatus, HCreate, HCreateDT,HModify, HModifyDT,  HSave,HImg, BCSchedule, BECSchedule, ICRecord, DPosition, CPosition, TPosition, HBudget, HBudgetTable, HSCourseID, HLodging, HTMaterialID, HCDeadlineDay,  HSerial,HDContinous, HBudgetType,HCourseType, HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID,  HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV) values ('', '', '" + DDL_HCourseTemplate.SelectedValue + "', '" + LB_HCBatchNum.Text + "', 'C' + CONVERT(nvarchar, getdate(), 112) + '0001', '" + TB_HCourseName.Text + "', '" + gLBox_HTeacherName + "', '" + LBoxHOCPlace.Value + "', '" + TB_HDateRange.Text + "', '" + TB_HSTime.Text + "', '" + TB_HETime.Text + "', '" + DDL_HType.SelectedValue + "','" + DDL_HOSystem.SelectedValue + "', '" + LB_HRSystem.Text + "', '" + RBL_HNLCourse.SelectedValue + "', '" + RBL_HNGuide.SelectedValue + "', '" + RBL_HNFull.SelectedValue + "','" + LB_HNRequirement.Text + "', '" + gLBox_HTeam + "','" + gLBox_HQuestionID + "', '" + DDL_HPMethod.SelectedValue + "', '" + BasicPrice + "', '" + RBL_HSGList.SelectedValue + "', '" + LB_HIRestriction.Text + "', N'" + HttpUtility.HtmlEncode(TB_HRemark.Text) + "', N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', '', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '', '99', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '1','" + picname + "','" + LB_BCSchedule.Text + "','" + LB_BECSchedule.Text + "','" + LB_ICRecord.Text + "','" + LB_DPosition.Text + "','" + LB_CPosition.Text + "','" + LB_TPosition.Text + "','" + RBL_HBudget.SelectedValue + "','" + LB_HBudgetTable.Text + "','" + DDL_HSCourse.SelectedValue + "','" + RBL_HLodging.SelectedValue + "', '" + gLBox_HTMaterialID + "', '" + TB_HCDeadlineDay.Text.Trim() + "', '" + RBL_HSerial.SelectedValue + "','" + RBL_Continuous.SelectedValue + "','" + TB_HBudgetType.Text + "', '" + DDL_HCourseType.SelectedValue + "', '" + RBL_HAxisYN.SelectedValue + "', '" + DDL_HAxisClass.SelectedValue + "', '" + DDL_HExamSubject.SelectedValue + "', '" + DDL_HGradeCalculation.SelectedValue + "', '" + gLBox_HExamPaperID + "',  '" + gLBox_HSupervise + "', '" + TB_HAttRateStandard.Text + "', '" + TB_HExamPassStandard.Text + "', '" + DDL_HExamContentID.SelectedValue + "', '" + TB_HParticipantLimit.Text + "', '" + DDL_HTeacherClass.SelectedValue + "', '" + DDL_HTearcherLV.SelectedValue + "');SELECT SCOPE_IDENTITY() AS HID " +
   "END";
                        dbCmd = new SqlCommand(strInsHC, dbConn);
                        HID += dbCmd.ExecuteScalar().ToString() + ",";
                    }
                    dr.Close();
                }
 
            }


        }
        if (HID != "")
        {
            LB_HID.Text = HID;
        }


        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Trim(',').Split(',');
            for (int i = 0; i < strHID.Length; i++)
            {

                //WE20250513_先拿掉insert gHExamStaff
                string strSelHTL = "insert into HTodoList (HCourseID, HGroupName, HTask, HTaskContent,HGroupLeaderID, HStatus, HCreate, HCreateDT, HSave, HCBatchNum, HTaskNum) values ('" + strHID[i] + "', '" + DDL_HGroupName.SelectedValue + "', '" + DDL_HTask.SelectedValue + "', '" + TB_HTaskContent.Text + "', '" + DDL_HGroupLeader.SelectedValue + "', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '0','" + LB_HCBatchNum.Text + "', '" + TB_HTaskNum.Text + "')";

                dbCmd = new SqlCommand(strSelHTL, dbConn);
                dbCmd.ExecuteNonQuery();


            }
        }
        dbConn.Close();
        dbCmd.Cancel();

        //20220821體系護持工作項目改為可修改組長
        SDS_HTodoList.SelectCommand = "SELECT HGroupName, HTask, HTaskContent, HGroupLeaderID,HSave, HTaskNum, HExamStaff FROM HTodoList where HCBatchNum='" + LB_HCBatchNum.Text + "' AND HStatus=1 GROUP BY HCBatchNum, HGroupName, HTask, HTaskContent, HGroupLeaderID,HSave, HTaskNum, HExamStaff  ORDER BY HGroupName ASC";

        //清空
        DDL_HGroupName.SelectedValue = "0";
        DDL_HTask.SelectedValue = "0";
        TB_HTaskNum.Text = null;
        TB_HTaskContent.Text = null;
        DDL_HGroupLeader.SelectedValue = "0";
    }
    #endregion

    #region 體系護持工作刪除功能
    protected void LBtn_HTodoList_Del_Click(object sender, EventArgs e)
    {
        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        //刪除
        string[] gHID = LB_HID.Text.Split(',');
        for (int i = 0; i < gHID.Length; i++)
        {

            string strDelHC_T = "DELETE FROM HTodoList WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HCourseID='" + gHID[i] + "' AND HTask='" + ((DropDownList)RI.FindControl("DDL_HTask")).SelectedValue + "' AND HGroupName ='" + ((DropDownList)RI.FindControl("DDL_HGroupName")).SelectedValue + "'";
            dbCmd = new SqlCommand(strDelHC_T, dbConn);
            dbCmd.ExecuteNonQuery();
        }



        dbConn.Close();
        dbCmd.Cancel();

        SDS_HTodoList.SelectCommand = "SELECT HGroupName, HTask, HTaskContent, HGroupLeaderID,HSave, HTaskNum, HExamStaff FROM HTodoList where HCBatchNum='" + LB_HCBatchNum.Text + "' AND HStatus=1  GROUP BY HCBatchNum,  HGroupName, HTask, HTaskContent, HGroupLeaderID,HSave, HTaskNum, HExamStaff ORDER BY HGroupName ASC";
        Rpt_HTodoList.DataBind();
    }
    #endregion

    #endregion

    #region 作業

    #region 新增功能
    protected void LBtn_HW_Add_Click(object sender, EventArgs e)
    {
        #region 必填判斷

        string alert;

        if (!ValidateRequiredFields(out alert))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('" + alert + "');", true);
            return;
        }


        //WA20240329_加入作業類型(回應/問卷)
        if (DDL_HHWType.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('請選擇作業類型(回應/問卷)。');", true);
            return;
        }


        //寫入DB
        if (string.IsNullOrEmpty(TB_HNumbers.Text.Trim()) && string.IsNullOrEmpty(TB_HDeadLine.Text.Trim()))
        {
            Response.Write("<script>alert('要記得輸入數量與期限哦~~');</script>");
            return;
        }


        #endregion

        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

   
        string HID = "";
        string gLBox_HRSystem = "", gLBox_HNRequirement = "", gLBox_HTeam = "", gLBox_HIRestriction = "", gLBox_HTeacherName = "", gLBox_HQuestionID = "", gLBox_HTMaterialID = "", gLBox_HExamPaperID = "", gLBox_HSupervise = "";

        #region 課程名稱符號轉換
        string[] transf = { "＿", "－", "-" };

        for (int i = 0; i < transf.Length; i++)
        {
            if (TB_HCourseName.Text.IndexOf(transf[i]) >= 0)
            {
                TB_HCourseName.Text = TB_HCourseName.Text.Replace(transf[i], "_");
            }
        }

        if (TB_HCourseName.Text.IndexOf("(") >= 0)
        {
            TB_HCourseName.Text = TB_HCourseName.Text.Replace("(", "（");
        }
        if (TB_HCourseName.Text.IndexOf(")") >= 0)
        {
            TB_HCourseName.Text = TB_HCourseName.Text.Replace(")", "）");
        }

        #endregion

        #region  重複判斷
        int reHOCPlace = 0;
        string stHOCPlace = null;
        foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
        {
            if (LBoxHOCPlace.Selected == true)
            {
                SqlDataReader MyEBF = SQLdatabase.ExecuteReader("SELECT a.HOCPlace, b.HPlaceName FROM HCourse AS a LEFT JOIN HPlace AS b ON a.HOCPlace=b.HID WHERE a.HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' AND a.HCourseName='" + TB_HCourseName.Text + "' AND a.HOCPlace='" + LBoxHOCPlace.Value + "' AND a.HDateRange='" + TB_HDateRange.Text + "' AND a.HStatus='1' AND a.HSave='1' AND NOT EXISTS ( SELECT HCourseName FROM HCourse  tb WHERE  a.HCBatchNum ='" + LB_HCBatchNum.Text + "' )");

                if (MyEBF.Read())
                {
                    reHOCPlace += 1;
                    stHOCPlace += MyEBF["HPlaceName"].ToString() + ",";
                }
                MyEBF.Close();


            }
        }

        if (reHOCPlace != 0)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('上課地點：" + stHOCPlace.TrimEnd(',') + "已重複開課，再請協助確認，謝謝!');", true);
            //Response.Write("<script>alert('上課地點：" + stHOCPlace.TrimEnd(',') + "已重複開課，再請協助確認，謝謝!');</script>");
            return;
        }
        #endregion


        #region--圖片上傳
        string picname = "";
        if (LB_OldPic.Text != "" && FU_HImg.HasFile == true)
        {
            //上傳圖片
            Upload_Pic();

            picname = LB_Pic.Text;
        }
        else if (LB_OldPic.Text != "" && FU_HImg.HasFile == false)
        {
            picname = LB_OldPic.Text;
        }
        else if (LB_OldPic.Text == "" && FU_HImg.HasFile == true)
        {
            //上傳圖片
            Upload_Pic();
            picname = LB_Pic.Text;
        }
        #endregion

        //220819-基本費用(元)存入DB要先轉換成點數概念(1元=10點)
        int BasicPrice = 0;
        if (string.IsNullOrEmpty(TB_HBCPoint.Text.Trim()))
        {
            Response.Write("<script>alert('請先確認已有選擇課程範本哦~!');</script>");
            return;
        }
        else
        {
            BasicPrice = Convert.ToInt32(TB_HBCPoint.Text.Trim()) / 10;
        }

        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Trim(',').Split(',');

            //221018  -上課地點多選
            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    for (int i = 0; i < strHID.Length; i++)
                    {
                        //220823  --課程編輯不會update到審核紀錄
                        //WE20231129_加入是否為連續日期
                        string strUpdHC = "update HCourse set HEnableSystem='', HApplySystem='', HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "', HCourseName='" + TB_HCourseName.Text + "', HTeacherName='" + gLBox_HTeacherName + "', HOCPlace='" + LBoxHOCPlace.Value + "', HDateRange='" + TB_HDateRange.Text + "', HSTime='" + TB_HSTime.Text + "', HETime='" + TB_HETime.Text + "', HType='" + DDL_HType.SelectedValue + "', HOSystem='" + DDL_HOSystem.SelectedValue + "', HRSystem='" + LB_HRSystem.Text + "', HNLCourse='" + RBL_HNLCourse.SelectedValue + "', HNGuide='" + RBL_HNGuide.SelectedValue + "', HNFull='" + RBL_HNFull.SelectedValue + "', HNRequirement='" + LB_HNRequirement.Text + "', HTeam='" + gLBox_HTeam + "',  HQuestionID='" + gLBox_HQuestionID + "', HPMethod='" + DDL_HPMethod.SelectedValue + "', HBCPoint='" + BasicPrice + "', HSGList='" + RBL_HSGList.SelectedValue + "', HIRestriction='" + LB_HIRestriction.Text + "', HRemark=N'" + HttpUtility.HtmlEncode(TB_HRemark.Text.Trim()) + "', HContentTitle=N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', HContent=N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', HRNContent=N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', HStatus='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1',HImg='" + picname + "', BCSchedule='" + LB_BCSchedule.Text + "' , BECSchedule='" + LB_BECSchedule.Text + "', ICRecord='" + LB_ICRecord.Text + "', DPosition='" + LB_DPosition.Text + "', CPosition='" + LB_CPosition.Text + "', TPosition='" + LB_TPosition.Text + "',HBudget='" + RBL_HBudget.SelectedValue + "', HBudgetTable='" + LB_HBudgetTable.Text + "', HLodging='" + RBL_HLodging.SelectedValue + "', HTMaterialID='" + gLBox_HTMaterialID + "', HCDeadline ='" + TB_HCDeadline.Text.Trim() + "', HCDeadlineDay ='" + TB_HCDeadlineDay.Text.Trim() + "', HDContinous='" + RBL_Continuous.SelectedValue + "', HBudgetType='" + TB_HBudgetType.Text + "', HCourseType='" + DDL_HCourseType.SelectedValue + "', HAxisYN='" + RBL_HAxisYN.SelectedValue + "', HAxisClass='" + DDL_HAxisClass.SelectedValue + "', HExamSubject='" + DDL_HExamSubject.SelectedValue + "', HGradeCalculation='" + DDL_HGradeCalculation.SelectedValue + "', HExamPaperID='" + gLBox_HExamPaperID + "',  HSupervise='" + gLBox_HSupervise + "', HAttRateStandard='" + TB_HAttRateStandard.Text + "', HExamPassStandard='" + TB_HExamPassStandard.Text + "', HExamContentID='" + DDL_HExamContentID.SelectedValue + "', HParticipantLimit='" + TB_HParticipantLimit.Text + "', HTeacherClass='" + DDL_HTeacherClass.SelectedValue + "', HTearcherLV='" + DDL_HTearcherLV.SelectedValue + "' where  HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'";
                        dbCmd = new SqlCommand(strUpdHC, dbConn);
                        dbCmd.ExecuteNonQuery();
                        //Response.Write(strUpdHC + "<br/>");

                    }

                    SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HCourseNum FROM HCourse WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'");
                    if (!dr.Read())
                    {
                        string strInsHC = "IF EXISTS(SELECT HCourseNum FROM HCourse WHERE SUBSTRING(HCourseNum,2,8) = CONVERT(nvarchar, getdate(), 112)) " +
   "BEGIN " +
       "insert into HCourse (HEnableSystem, HApplySystem, HCTemplateID, HCBatchNum, HCourseNum, HCourseName, HTeacherName, HOCPlace, HDateRange, HSTime, HETime, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam,  HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HRNContent, HVerifyNum, HApplicant, HApplyTime, HVerifyTime, HVerifyStatus, HStatus, HCreate, HCreateDT,HModify, HModifyDT,  HSave,HImg,BCSchedule, BECSchedule,  ICRecord, DPosition, CPosition, TPosition, HBudget, HBudgetTable, HSCourseID, HLodging, HTMaterialID, HCDeadlineDay,  HSerial, HDContinous, HBudgetType,HCourseType, HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID,  HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV) values ('', '', '" + DDL_HCourseTemplate.SelectedValue + "', '" + LB_HCBatchNum.Text + "', 'C' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HCourseNum), 12)) FROM HCourse) + 1), '" + TB_HCourseName.Text + "', '" + gLBox_HTeacherName + "', '" + LBoxHOCPlace.Value + "', '" + TB_HDateRange.Text + "', '" + TB_HSTime.Text + "', '" + TB_HETime.Text + "', '" + DDL_HType.SelectedValue + "','" + DDL_HOSystem.SelectedValue + "', '" + LB_HRSystem.Text + "', '" + RBL_HNLCourse.SelectedValue + "', '" + RBL_HNGuide.SelectedValue + "', '" + RBL_HNFull.SelectedValue + "','" + LB_HNRequirement.Text + "', '" + gLBox_HTeam + "',  '" + gLBox_HQuestionID + "', '" + DDL_HPMethod.SelectedValue + "', '" + BasicPrice + "', '" + RBL_HSGList.SelectedValue + "', '" + LB_HIRestriction.Text + "', N'" + HttpUtility.HtmlEncode(TB_HRemark.Text) + "', N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', '', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '', '99', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',  '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','1','" + picname + "','" + LB_BCSchedule.Text + "','" + LB_BECSchedule.Text + "','" + LB_ICRecord.Text + "','" + LB_DPosition.Text + "','" + LB_CPosition.Text + "','" + LB_TPosition.Text + "','" + RBL_HBudget.SelectedValue + "','" + LB_HBudgetTable.Text + "','" + DDL_HSCourse.SelectedValue + "','" + RBL_HLodging.SelectedValue + "', '" + gLBox_HTMaterialID + "', '" + TB_HCDeadlineDay.Text.Trim() + "','" + RBL_HSerial.SelectedValue + "','" + RBL_Continuous.SelectedValue + "','" + TB_HBudgetType.Text + "','" + DDL_HCourseType.SelectedValue + "', '" + RBL_HAxisYN.SelectedValue + "', '" + DDL_HAxisClass.SelectedValue + "', '" + DDL_HExamSubject.SelectedValue + "', '" + DDL_HGradeCalculation.SelectedValue + "', '" + gLBox_HExamPaperID + "',  '" + gLBox_HSupervise + "', '" + TB_HAttRateStandard.Text + "', '" + TB_HExamPassStandard.Text + "', '" + DDL_HExamContentID.SelectedValue + "', '" + TB_HParticipantLimit.Text + "', '" + DDL_HTeacherClass.SelectedValue + "', '" + DDL_HTearcherLV.SelectedValue + "' );SELECT SCOPE_IDENTITY() AS HID " +
   "END " +
   "ELSE " +
   "BEGIN " +
       "insert into HCourse (HEnableSystem, HApplySystem, HCTemplateID, HCBatchNum, HCourseNum, HCourseName, HTeacherName, HOCPlace, HDateRange, HSTime, HETime, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam,  HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HRNContent, HVerifyNum, HApplicant, HApplyTime, HVerifyTime, HVerifyStatus, HStatus, HCreate, HCreateDT,HModify, HModifyDT,  HSave,HImg, BCSchedule, BECSchedule, ICRecord, DPosition, CPosition, TPosition, HBudget, HBudgetTable, HSCourseID, HLodging, HTMaterialID, HCDeadlineDay, HSerial,HDContinous,HBudgetType, HCourseType, HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID,  HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV) values ('', '', '" + DDL_HCourseTemplate.SelectedValue + "', '" + LB_HCBatchNum.Text + "', 'C' + CONVERT(nvarchar, getdate(), 112) + '0001', '" + TB_HCourseName.Text + "', '" + gLBox_HTeacherName + "', '" + LBoxHOCPlace.Value + "', '" + TB_HDateRange.Text + "', '" + TB_HSTime.Text + "', '" + TB_HETime.Text + "', '" + DDL_HType.SelectedValue + "','" + DDL_HOSystem.SelectedValue + "', '" + LB_HRSystem.Text + "', '" + RBL_HNLCourse.SelectedValue + "', '" + RBL_HNGuide.SelectedValue + "', '" + RBL_HNFull.SelectedValue + "','" + LB_HNRequirement.Text + "', '" + gLBox_HTeam + "','" + gLBox_HQuestionID + "', '" + DDL_HPMethod.SelectedValue + "', '" + BasicPrice + "', '" + RBL_HSGList.SelectedValue + "', '" + LB_HIRestriction.Text + "', N'" + HttpUtility.HtmlEncode(TB_HRemark.Text) + "', N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', '', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '', '99', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '1','" + picname + "','" + LB_BCSchedule.Text + "','" + LB_BECSchedule.Text + "','" + LB_ICRecord.Text + "','" + LB_DPosition.Text + "','" + LB_CPosition.Text + "','" + LB_TPosition.Text + "','" + RBL_HBudget.SelectedValue + "','" + LB_HBudgetTable.Text + "','" + DDL_HSCourse.SelectedValue + "','" + RBL_HLodging.SelectedValue + "', '" + gLBox_HTMaterialID + "', '" + TB_HCDeadlineDay.Text.Trim() + "','" + RBL_HSerial.SelectedValue + "','" + RBL_Continuous.SelectedValue + "','" + TB_HBudgetType.Text + "','" + DDL_HCourseType.SelectedValue + "', '" + RBL_HAxisYN.SelectedValue + "', '" + DDL_HAxisClass.SelectedValue + "', '" + DDL_HExamSubject.SelectedValue + "', '" + DDL_HGradeCalculation.SelectedValue + "', '" + gLBox_HExamPaperID + "',  '" + gLBox_HSupervise + "', '" + TB_HAttRateStandard.Text + "', '" + TB_HExamPassStandard.Text + "', '" + DDL_HExamContentID.SelectedValue + "', '" + TB_HParticipantLimit.Text + "', '" + DDL_HTeacherClass.SelectedValue + "', '" + DDL_HTearcherLV.SelectedValue + "' );SELECT SCOPE_IDENTITY() AS HID " +
   "END";
                        dbCmd = new SqlCommand(strInsHC, dbConn);
                        //dbCmd.ExecuteNonQuery();
                        HID += dbCmd.ExecuteScalar().ToString() + ",";
                        //dbCmd.Cancel();
                        //Response.Write(strInsHC+"<br/>");
                    }
                    dr.Close();
                }
                else
                {
                    SqlDataReader drDel = SQLdatabase.ExecuteReader("SELECT HCourseNum FROM HCourse WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'");

                    if (drDel.Read())
                    {
                        for (int i = 0; i < strHID.Length; i++)
                        {
                            string strUpdHC_T = "update HCourse set HStatus='0' where HID='" + strHID[i] + "' AND HOCPlace='" + LBoxHOCPlace.Value + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "'";
                            dbCmd = new SqlCommand(strUpdHC_T, dbConn);
                            dbCmd.ExecuteNonQuery();


                            string strUpdHCM_T = "update HCourseMaterial set HStatus='0' where HCourseID='" + strHID[i] + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "'";
                            dbCmd = new SqlCommand(strUpdHCM_T, dbConn);
                            dbCmd.ExecuteNonQuery();

                            string strUpdHTL_T = "update HTodoList set HStatus='0' where HCourseID='" + strHID[i] + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "'";
                            dbCmd = new SqlCommand(strUpdHTL_T, dbConn);
                            dbCmd.ExecuteNonQuery();

                            string strUpdHCE_T = "update HCourseEvaluation set HStatus='0' where HCourseID='" + strHID[i] + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "'";
                            dbCmd = new SqlCommand(strUpdHCE_T, dbConn);
                            dbCmd.ExecuteNonQuery();

                            string strUpdHCTM_T = "update HCourseTMaterials set HStatus='0' where HCourseID='" + strHID[i] + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "'";
                            dbCmd = new SqlCommand(strUpdHCTM_T, dbConn);
                            dbCmd.ExecuteNonQuery();
                        }

                    }
                    drDel.Close();
                }
            }


        }


        if (HID != "")
        {
            LB_HID.Text = HID;
        }

        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Trim(',').Split(',');
            for (int i = 0; i < strHID.Length; i++)
            {

                //WE20240329_加入作業類型、問卷、作業內容描述
                string strHWSetting = "insert into HCourse_HWSetting (HCourseID, HNumbers, HDeadLine, HStatus, HCreate, HCreateDT, HSave, HCBatchNum, HHWType, HQuestionHID, HDescription) values ('" + strHID[i] + "', '" + TB_HNumbers.Text.Trim() + "', '" + TB_HDeadLine.Text.Trim() + "', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '0', '" + LB_HCBatchNum.Text + "','" + DDL_HHWType.SelectedValue + "' ,'" + DDL_HQuestion.SelectedValue + "', '" + TB_HDescription.Text.Trim() + "' )";


                dbCmd = new SqlCommand(strHWSetting, dbConn);
                dbCmd.ExecuteNonQuery();
            }
        }


        dbCmd.Cancel();
        dbConn.Close();

        //清空資料
        TB_HNumbers.Text = null;
        TB_HDeadLine.Text = null;
        TB_HDescription.Text = null;
        DDL_HQuestion.SelectedValue = "0";


        //WE20240329_加入作業類型、問卷、內容描述
        SDS_HCourseHWSetting.SelectCommand = "SELECT a.HCBatchNum, a.HNumbers, a.HDeadLine, a.HSave, a.HHWType, a.HQuestionHID, a.HDescription, b.HTitle FROM HCourse_HWSetting AS a  LEFT JOIN HQuestion AS b ON a.HQuestionHID= b.HID WHERE a.HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY a.HCBatchNum, a.HNumbers, a.HDeadLine, a.HSave,  a.HHWType, a.HQuestionHID, a.HDescription, b.HTitle  ORDER BY HDeadLine ASC";






    }
    #endregion

    #region 作業刪除功能
    protected void LBtn_HW_Del_Click(object sender, EventArgs e)
    {

        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        //刪除
        string strDelHC_T = "DELETE FROM HCourse_HWSetting WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HNumbers='" + ((Label)RI.FindControl("LB_HNumbers")).Text + "' AND HDeadLine='" + ((Label)RI.FindControl("LB_HDeadLine")).Text + "'";
        dbCmd = new SqlCommand(strDelHC_T, dbConn);
        dbCmd.ExecuteNonQuery();

        dbConn.Close();
        dbCmd.Cancel();

        //WE20240329_加入作業類型、問卷、內容描述
        SDS_HCourseHWSetting.SelectCommand = "SELECT a.HCBatchNum, a.HNumbers, a.HDeadLine, a.HSave, a.HHWType, a.HQuestionHID, a.HDescription, b.HTitle FROM HCourse_HWSetting AS a  LEFT JOIN HQuestion AS b ON a.HQuestionHID= b.HID WHERE a.HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY a.HCBatchNum, a.HNumbers, a.HDeadLine, a.HSave,  a.HHWType, a.HQuestionHID, a.HDescription, b.HTitle  ORDER BY HDeadLine ASC";
    }
    #endregion

    #region 作業類型
    protected void DDL_HHWType_SelectedIndexChanged(object sender, EventArgs e)
    {
        RegScript();//執行js

        if (DDL_HHWType.SelectedValue == "2")
        {
            DDL_HQuestion.Visible = true;
        }
        else
        {
            DDL_HQuestion.Visible = false;
        }
    }
    #endregion

    #region 作業繫結
    protected void RPT_HCourseHWSetting_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if ((((Label)e.Item.FindControl("LB_HHWType")).Text) == "1")
        {
            (((Label)e.Item.FindControl("LB_HHWType"))).Text = "回應(可上傳圖片/PDF/Excel)";
        }
        else
        {
            (((Label)e.Item.FindControl("LB_HHWType"))).Text = "問卷";
        }
    }
    #endregion

    #endregion

    #region 儲存功能
    protected void Btn_Submit_Click(object sender, EventArgs e)
    {

        string HVerifyStatus = LB_HVerifyStatus.Text;

        #region 必填判斷

        string alert;
        if (!ValidateRequiredFields(out alert))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('" + alert + "');", true);
            return;
        }

        #endregion

        #region 課程名稱符號轉換
        string[] transf = { "＿", "－", "-" };

        for (int i = 0; i < transf.Length; i++)
        {
            if (TB_HCourseName.Text.IndexOf(transf[i]) >= 0)
            {
                TB_HCourseName.Text = TB_HCourseName.Text.Replace(transf[i], "_");
            }
        }

        if (TB_HCourseName.Text.IndexOf("(") >= 0)
        {
            TB_HCourseName.Text = TB_HCourseName.Text.Replace("(", "（");
        }
        if (TB_HCourseName.Text.IndexOf(")") >= 0)
        {
            TB_HCourseName.Text = TB_HCourseName.Text.Replace(")", "）");
        }

        #endregion

        #region  重複判斷
        int reHOCPlace = 0;
        string stHOCPlace = null;
        foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
        {
            if (LBoxHOCPlace.Selected == true)
            {
                SqlDataReader MyEBF = SQLdatabase.ExecuteReader("SELECT a.HOCPlace, b.HPlaceName FROM HCourse AS a LEFT JOIN HPlace AS b ON a.HOCPlace=b.HID WHERE a.HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' AND a.HCourseName='" + TB_HCourseName.Text + "' AND a.HOCPlace='" + LBoxHOCPlace.Value + "' AND a.HDateRange='" + TB_HDateRange.Text + "' AND a.HStatus='1' AND a.HSave='1' AND NOT EXISTS ( SELECT HCourseName FROM HCourse  tb WHERE  a.HCBatchNum ='" + LB_HCBatchNum.Text + "' )");

                if (MyEBF.Read())
                {
                    reHOCPlace += 1;
                    stHOCPlace += MyEBF["HPlaceName"].ToString() + ",";
                }
                MyEBF.Close();


            }
        }


        if (reHOCPlace != 0)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('上課地點：" + stHOCPlace.TrimEnd(',') + "已重複開課，再請協助確認，謝謝!');", true);
            //Response.Write("<script>alert('上課地點：" + stHOCPlace.TrimEnd(',') + "已重複開課，再請協助確認，謝謝!');</script>");
            return;
        }
        #endregion


        #region--圖片上傳
        string picname = "";
        if (LB_OldPic.Text != "" && FU_HImg.HasFile == true)
        {
            //上傳圖片
            Upload_Pic();

            picname = LB_Pic.Text;
        }
        else if (LB_OldPic.Text != "" && FU_HImg.HasFile == false)
        {
            picname = LB_OldPic.Text;
        }
        else if (LB_OldPic.Text == "" && FU_HImg.HasFile == true)
        {
            //上傳圖片
            Upload_Pic();
            picname = LB_Pic.Text;
        }
        #endregion

        bool fileIsValid = false;

        #region 上傳檔案
        if (FU_BCSchedule.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_BCSchedule.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_BCSchedule.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_BCSchedule.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }


        if (FU_BECSchedule.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_BECSchedule.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_BECSchedule.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_BECSchedule.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        if (FU_ICRecord.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_ICRecord.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_ICRecord.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_ICRecord.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        if (FU_DPosition.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_DPosition.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_DPosition.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_DPosition.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        if (FU_CPosition.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_CPosition.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_CPosition.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_CPosition.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        if (FU_TPosition.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_TPosition.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_TPosition.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_TPosition.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        #endregion


        int gCT = 1;
        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        #region 將上傳之教材暫存檔名改為正式檔名
        string gHCMFileTemp = "~/uploads/CourseMaterialTemp/";
        string gHCMFileRoot = "~/uploads/CourseMaterial/";
        string gHCMFileName = "";//主檔名
                                 //string gHCMFileExtension = "";//副檔名

        string strSelHCM = "select HCMaterial from HCourseMaterial where HCBatchNum = '" + LB_HCBatchNum.Text + "' GROUP BY HCMaterial";
        //string strSelHCM = "select HCMaterial from HCourseMaterial where HCourseID = '" + LB_HID.Text + "' and HSave='0'";
        dbCmd = new SqlCommand(strSelHCM, dbConn);
        SqlDataReader MyQueryHCM = dbCmd.ExecuteReader();

        while (MyQueryHCM.Read())
        {
            if (!string.IsNullOrEmpty(MyQueryHCM["HCMaterial"].ToString()))
            {
                //主檔名+副檔名
                gHCMFileName = MyQueryHCM["HCMaterial"].ToString();

                //取得檔名
                string[] Filename = gHCMFileName.Split('.');
                // string New_name = Filename[0].ToString() + "_" + DateTime.Now.ToString("yyMMdd") + "." + Filename[1].ToString(); //避免名稱重複 資料夾已有檔案會error
                //將上傳之教材從暫存區移動到正式區
                System.IO.File.Move(Server.MapPath(gHCMFileTemp) + gHCMFileName, Server.MapPath(gHCMFileRoot) + gHCMFileName);
            }
        }
        MyQueryHCM.Close();
        #endregion


        #region    取ListBox的值，使用ForEach方式
        string gLBox_HRSystem = "";
        foreach (ListItem LBoxHRSystem in LBox_HRSystem.Items)
        {
            if (LBoxHRSystem.Selected == true)
            {
                gLBox_HRSystem += LBoxHRSystem.Value + ",";
            }
        }

        string gLBox_HNRequirement = "";
        foreach (ListItem LBoxHNRequirement in LBox_HNRequirement.Items)
        {
            if (LBoxHNRequirement.Selected == true)
            {
                gLBox_HNRequirement += LBoxHNRequirement.Value + ",";
            }
        }

        string gLBox_HTeam = "";
        foreach (ListItem LBoxHTeam in LBox_HTeam.Items)
        {
            if (LBoxHTeam.Selected == true)
            {
                gLBox_HTeam += LBoxHTeam.Value + ",";
            }
        }

        string gLBox_HIRestriction = "";
        foreach (ListItem LBoxHIRestriction in LBox_HIRestriction.Items)
        {
            if (LBoxHIRestriction.Selected == true)
            {
                gLBox_HIRestriction += LBoxHIRestriction.Value + ",";
            }
        }

        string gLBox_HTeacherName = "";
        foreach (ListItem LBoxHTeacherName in LBox_HTeacherName.Items)
        {
            if (LBoxHTeacherName.Selected == true)
            {
                gLBox_HTeacherName += LBoxHTeacherName.Value + ",";
            }
        }

        string gLBox_HQuestionID = "";
        foreach (ListItem LBoxHQuestionID in LBox_HQuestionID.Items)
        {
            if (LBoxHQuestionID.Selected == true)
            {
                gLBox_HQuestionID += LBoxHQuestionID.Value + ",";
            }
        }

        string gLBox_HTMaterialID = "";

        //EA20240618_新增
        string gLBox_HExamPaperID = "";
        foreach (ListItem LBoxHExamPaperID in LBox_HExamPaperID.Items)
        {
            if (LBoxHExamPaperID.Selected == true)
            {
                gLBox_HExamPaperID += LBoxHExamPaperID.Value + ",";
            }
        }


        //EA20240618_新增
        string gLBox_HSupervise = "";
        foreach (ListItem LBoxHSupervise in LBox_HSupervise.Items)
        {
            if (LBoxHSupervise.Selected == true)
            {
                gLBox_HSupervise += LBoxHSupervise.Value + ",";
            }
        }

        #endregion


        //220819-基本費用(元)存入DB要先轉換成點數概念(1元=10點)
        int BasicPrice = Convert.ToInt32(TB_HBCPoint.Text.Trim()) / 10;
        string strUniteTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");


        string HID = "";
        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Trim(',').Split(',');

            //221018  -上課地點多選
            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    for (int i = 0; i < strHID.Length; i++)
                    {
                        //220823  --課程編輯不會update到審核紀錄

                        string strUpdHC = "update HCourse set HEnableSystem='', HApplySystem='', HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "', HCourseName='" + TB_HCourseName.Text + "', HTeacherName='" + gLBox_HTeacherName + "', HOCPlace='" + LBoxHOCPlace.Value + "', HDateRange='" + TB_HDateRange.Text + "', HSTime='" + TB_HSTime.Text + "', HETime='" + TB_HETime.Text + "', HType='" + DDL_HType.SelectedValue + "', HOSystem='" + DDL_HOSystem.SelectedValue + "', HRSystem='" + LB_HRSystem.Text + "', HNLCourse='" + RBL_HNLCourse.SelectedValue + "', HNGuide='" + RBL_HNGuide.SelectedValue + "', HNFull='" + RBL_HNFull.SelectedValue + "', HNRequirement='" + LB_HNRequirement.Text + "', HTeam='" + gLBox_HTeam + "',  HQuestionID='" + gLBox_HQuestionID + "', HPMethod='" + DDL_HPMethod.SelectedValue + "', HBCPoint='" + BasicPrice + "', HSGList='" + RBL_HSGList.SelectedValue + "', HIRestriction='" + LB_HIRestriction.Text + "', HRemark=N'" + HttpUtility.HtmlEncode(TB_HRemark.Text.Trim()) + "', HContentTitle=N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', HContent=N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', HRNContent=N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', HStatus='1', HApplicant='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + strUniteTime + "', HSave='1',HImg='" + picname + "', BCSchedule='" + LB_BCSchedule.Text + "' , BECSchedule='" + LB_BECSchedule.Text + "', ICRecord='" + LB_ICRecord.Text + "', DPosition='" + LB_DPosition.Text + "', CPosition='" + LB_CPosition.Text + "', TPosition='" + LB_TPosition.Text + "',HBudget='" + RBL_HBudget.SelectedValue + "', HBudgetTable='" + LB_HBudgetTable.Text + "', HLodging='" + RBL_HLodging.SelectedValue + "', HTMaterialID='" + gLBox_HTMaterialID + "', HCDeadline ='" + TB_HCDeadline.Text.Trim() + "', HCDeadlineDay ='" + TB_HCDeadlineDay.Text.Trim() + "', HDContinous='" + RBL_Continuous.SelectedValue + "', HBudgetType='" + TB_HBudgetType.Text + "', HCourseType='" + DDL_HCourseType.SelectedValue + "', HAxisYN='" + RBL_HAxisYN.SelectedValue + "', HAxisClass='" + DDL_HAxisClass.SelectedValue + "', HExamSubject='" + DDL_HExamSubject.SelectedValue + "', HGradeCalculation='" + DDL_HGradeCalculation.SelectedValue + "', HExamPaperID='" + gLBox_HExamPaperID + "',  HSupervise='" + gLBox_HSupervise + "', HAttRateStandard='" + TB_HAttRateStandard.Text + "', HExamPassStandard='" + TB_HExamPassStandard.Text + "', HExamContentID='" + DDL_HExamContentID.SelectedValue + "', HParticipantLimit='" + TB_HParticipantLimit.Text + "', HTeacherClass='" + DDL_HTeacherClass.SelectedValue + "', HTearcherLV='" + DDL_HTearcherLV.SelectedValue + "' where  HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'";
                        //, HApplyTime='" + strUniteTime + "', HVerifyTime='', HVerifyStatus='99' 
                        dbCmd = new SqlCommand(strUpdHC, dbConn);
                        dbCmd.ExecuteNonQuery();
                    }

                    SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HCourseNum FROM HCourse WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'");
                    if (!dr.Read())
                    {
                        string strInsHC = "IF EXISTS(SELECT HCourseNum FROM HCourse WHERE SUBSTRING(HCourseNum,2,8) = CONVERT(nvarchar, getdate(), 112)) " +
   "BEGIN " +
       "insert into HCourse (HEnableSystem, HApplySystem, HCTemplateID, HCBatchNum, HCourseNum, HCourseName, HTeacherName, HOCPlace, HDateRange, HSTime, HETime, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam,  HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HRNContent, HVerifyNum, HApplicant, HApplyTime, HVerifyTime, HVerifyStatus, HStatus, HCreate, HCreateDT,HModify, HModifyDT,  HSave,HImg,BCSchedule, BECSchedule,  ICRecord, DPosition, CPosition, TPosition, HBudget, HBudgetTable, HSCourseID, HLodging, HTMaterialID, HCDeadline, HCDeadlineDay, HSerial, HDContinous, HBudgetType, HCourseType, HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID,  HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV) values ('', '', '" + DDL_HCourseTemplate.SelectedValue + "', '" + LB_HCBatchNum.Text + "', 'C' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HCourseNum), 12)) FROM HCourse) + 1), '" + TB_HCourseName.Text + "', '" + gLBox_HTeacherName + "', '" + LBoxHOCPlace.Value + "', '" + TB_HDateRange.Text + "', '" + TB_HSTime.Text + "', '" + TB_HETime.Text + "', '" + DDL_HType.SelectedValue + "','" + DDL_HOSystem.SelectedValue + "', '" + LB_HRSystem.Text + "', '" + RBL_HNLCourse.SelectedValue + "', '" + RBL_HNGuide.SelectedValue + "', '" + RBL_HNFull.SelectedValue + "','" + LB_HNRequirement.Text + "', '" + gLBox_HTeam + "',  '" + gLBox_HQuestionID + "', '" + DDL_HPMethod.SelectedValue + "', '" + BasicPrice + "', '" + RBL_HSGList.SelectedValue + "', '" + LB_HIRestriction.Text + "', N'" + HttpUtility.HtmlEncode(TB_HRemark.Text) + "', N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', '', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteTime + "', '', '99', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteTime + "',  '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteTime + "','1','" + picname + "','" + LB_BCSchedule.Text + "','" + LB_BECSchedule.Text + "','" + LB_ICRecord.Text + "','" + LB_DPosition.Text + "','" + LB_CPosition.Text + "','" + LB_TPosition.Text + "','" + RBL_HBudget.SelectedValue + "','" + LB_HBudgetTable.Text + "','" + DDL_HSCourse.SelectedValue + "','" + RBL_HLodging.SelectedValue + "', '" + gLBox_HTMaterialID + "','" + TB_HCDeadline.Text.Trim() + "', '" + TB_HCDeadlineDay.Text.Trim() + "', '" + RBL_HSerial.SelectedValue + "','" + RBL_Continuous.SelectedValue + "','" + TB_HBudgetType.Text + "', '" + DDL_HCourseType.SelectedValue + "', '" + RBL_HAxisYN.SelectedValue + "', '" + DDL_HAxisClass.SelectedValue + "', '" + DDL_HExamSubject.SelectedValue + "', '" + DDL_HGradeCalculation.SelectedValue + "', '" + gLBox_HExamPaperID + "',  '" + gLBox_HSupervise + "', '" + TB_HAttRateStandard.Text + "', '" + TB_HExamPassStandard.Text + "', '" + DDL_HExamContentID.SelectedValue + "', '" + TB_HParticipantLimit.Text + "', '" + DDL_HTeacherClass.SelectedValue + "', '" + DDL_HTearcherLV.SelectedValue + "' );SELECT SCOPE_IDENTITY() AS HID " +
   "END " +
   "ELSE " +
   "BEGIN " +
       "insert into HCourse (HEnableSystem, HApplySystem, HCTemplateID, HCBatchNum, HCourseNum, HCourseName, HTeacherName, HOCPlace, HDateRange, HSTime, HETime, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam,  HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HRNContent, HVerifyNum, HApplicant, HApplyTime, HVerifyTime, HVerifyStatus, HStatus, HCreate, HCreateDT,HModify, HModifyDT,  HSave,HImg, BCSchedule, BECSchedule, ICRecord, DPosition, CPosition, TPosition, HBudget, HBudgetTable, HSCourseID, HLodging, HTMaterialID, HCDeadline, HCDeadlineDay, HSerial, HDContinous,  HBudgetType, HCourseType, HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID,  HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV) values ('', '', '" + DDL_HCourseTemplate.SelectedValue + "', '" + LB_HCBatchNum.Text + "', 'C' + CONVERT(nvarchar, getdate(), 112) + '0001', '" + TB_HCourseName.Text + "', '" + gLBox_HTeacherName + "', '" + LBoxHOCPlace.Value + "', '" + TB_HDateRange.Text + "', '" + TB_HSTime.Text + "', '" + TB_HETime.Text + "', '" + DDL_HType.SelectedValue + "','" + DDL_HOSystem.SelectedValue + "', '" + LB_HRSystem.Text + "', '" + RBL_HNLCourse.SelectedValue + "', '" + RBL_HNGuide.SelectedValue + "', '" + RBL_HNFull.SelectedValue + "','" + LB_HNRequirement.Text + "', '" + gLBox_HTeam + "','" + gLBox_HQuestionID + "', '" + DDL_HPMethod.SelectedValue + "', '" + BasicPrice + "', '" + RBL_HSGList.SelectedValue + "', '" + LB_HIRestriction.Text + "', N'" + HttpUtility.HtmlEncode(TB_HRemark.Text) + "', N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', '', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteTime + "', '', '99', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteTime + "', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteTime + "', '1','" + picname + "','" + LB_BCSchedule.Text + "','" + LB_BECSchedule.Text + "','" + LB_ICRecord.Text + "','" + LB_DPosition.Text + "','" + LB_CPosition.Text + "','" + LB_TPosition.Text + "','" + RBL_HBudget.SelectedValue + "','" + LB_HBudgetTable.Text + "','" + DDL_HSCourse.SelectedValue + "','" + RBL_HLodging.SelectedValue + "', '" + gLBox_HTMaterialID + "','" + TB_HCDeadline.Text.Trim() + "', '" + TB_HCDeadlineDay.Text.Trim() + "', '" + RBL_HSerial.SelectedValue + "','" + RBL_Continuous.SelectedValue + "','" + TB_HBudgetType.Text + "',  '" + DDL_HCourseType.SelectedValue + "', '" + RBL_HAxisYN.SelectedValue + "', '" + DDL_HAxisClass.SelectedValue + "', '" + DDL_HExamSubject.SelectedValue + "', '" + DDL_HGradeCalculation.SelectedValue + "', '" + gLBox_HExamPaperID + "',  '" + gLBox_HSupervise + "', '" + TB_HAttRateStandard.Text + "', '" + TB_HExamPassStandard.Text + "', '" + DDL_HExamContentID.SelectedValue + "', '" + TB_HParticipantLimit.Text + "', '" + DDL_HTeacherClass.SelectedValue + "', '" + DDL_HTearcherLV.SelectedValue + "' );SELECT SCOPE_IDENTITY() AS HID " +
   "END";
                        dbCmd = new SqlCommand(strInsHC, dbConn);
                        HID += dbCmd.ExecuteScalar().ToString() + ",";
                    }
                    dr.Close();

                }

            }

            for (int i = 0; i < strHID.Length; i++)
            {
                for (int x = 0; x < Rpt_HTodoList.Items.Count; x++)
                {
                    if (((Label)Rpt_HTodoList.Items[x].FindControl("LB_HSave")).Text == "0")
                    {

                        string strUpdHTL = "UPDATE HTodoList SET HStatus='1', HGroupLeaderID='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupLeader")).SelectedValue + "', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1' WHERE HCBatchNum = '" + LB_HCBatchNum.Text + "' AND HCourseID='" + strHID[i] + "' AND HTask='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HTask")).SelectedValue + "' AND HGroupName ='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupName")).SelectedValue + "'";
                        dbCmd = new SqlCommand(strUpdHTL, dbConn);
                        dbCmd.ExecuteNonQuery();
                    }

                }

                for (int x = 0; x < Rpt_HTMaterialDetail.Items.Count; x++)
                {
                    if (((Label)Rpt_HTMaterialDetail.Items[x].FindControl("LB_HSave")).Text == "0")
                    {
                        string strUpdHCTM = "update HCourseTMaterial set HStatus='1', HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HCreateDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1' where HCBatchNum = '" + LB_HCBatchNum.Text + "'";
                        dbCmd = new SqlCommand(strUpdHCTM, dbConn);
                        dbCmd.ExecuteNonQuery();
                    }

                }

            }


            #region 日期判斷
            //WA20250611_新增判斷:若勾選開放單天報名，則日期會拆成一筆一筆的存法
            if (RBL_HBookByDateYN.SelectedValue == "1")
            {
                if (!string.IsNullOrEmpty(TB_HDateRange.Text.Trim()))
                {
                    string[] gHDateRange = { };
                    string gSDate = "";
                    string gEDate = "";

                    //判斷天數相差幾天
                    int gDateRange = 0;//連續天數


                    if (TB_HDateRange.Text.Trim().IndexOf("-") >= 0)  //連續日期
                    {
                        gHDateRange = TB_HDateRange.Text.Trim().Trim(',').Split('-');
                        string gDate = "";
                        string gDate1 = "";
                        gSDate = (gHDateRange[0]);
                        gEDate = (gHDateRange[1]);
                        gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;

                        for (int i = 0; i < gDateRange; i++)
                        {
                            gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                            gDate1 += "'" + Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + "'" + ",";
                        }

                        gHDateRange = gDate.Trim(',').Split(',');
                    }
                    else if (TB_HDateRange.Text.Trim().IndexOf(",") >= 0)   //多選日期
                    {
                        Response.Write("AA");
                        gHDateRange = TB_HDateRange.Text.Trim().Trim(',').Split(',');
                    }
                    else if (TB_HDateRange.Text.Trim().IndexOf("-") < 0 && TB_HDateRange.Text.Trim().IndexOf(",") < 0)//單一日期
                    {
                        gHDateRange = (TB_HDateRange.Text.Trim() + " - " + TB_HDateRange.Text.Trim()).Split('-');
                        string gDate = "";
                        string gDate1 = "";
                        gSDate = (gHDateRange[0]);
                        gEDate = (gHDateRange[1]);
                        gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;

                        for (int i = 0; i < gDateRange; i++)
                        {
                            gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                            gDate1 += "'" + Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + "'" + ",";
                        }

                        gHDateRange = gDate.Trim(',').Split(',');
                    }


                    //先刪掉原本的內容，再重新寫入資料表
                    string sql = "DELETE FROM HCourseDate WHERE HCBatchNum=@HCBatchNum AND HCourseName=@HCourseName AND HDateRange=@HDateRange";
                    SqlCommand cmd = new SqlCommand(sql, dbConn);
                    cmd.Parameters.AddWithValue("@HCBatchNum", TB_HCBatchNum.Text.Trim());
                    cmd.Parameters.AddWithValue("@HCourseName", TB_HCourseName.Text.Trim());
                    cmd.Parameters.AddWithValue("@HDateRange", TB_HDateRange.Text.Trim());
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();



                    //寫入資料表
                    DataTable dtCourseDate = (DataTable)ViewState["dtCourseDate"];


                    for (int i = 0; i <= gHDateRange.Length - 1; i++)
                    {
                        DateTime gHDate = Convert.ToDateTime(gHDateRange[i].ToString());//上課日
                        DataRow rowRC;
                        rowRC = dtCourseDate.NewRow();
                        rowRC["HCBatchNum"] = TB_HCBatchNum.Text;
                        rowRC["HCourseName"] = TB_HCourseName.Text.Trim();
                        rowRC["HDateRange"] = TB_HDateRange.Text.Trim();
                        rowRC["HDate"] = gHDate.ToString("yyyy/MM/dd");
                        rowRC["HStatus"] = "1";
                        rowRC["HCreate"] = ((Label)Master.FindControl("LB_HUserHID")).Text;
                        rowRC["HCreateDT"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                        dtCourseDate.Rows.Add(rowRC);
                    }
                    ViewState["dtCourseDate"] = dtCourseDate;


                    //寫入資料庫(使用SqlBulkCopy)
                    DataTable dtCourseDateAdd = (DataTable)ViewState["dtCourseDate"];
                    SqlBulkCopy bulkCopy = new SqlBulkCopy(con);
                    //設定一個批次量寫入多少筆資料
                    bulkCopy.BatchSize = 1000;
                    //設定逾時的秒數
                    bulkCopy.BulkCopyTimeout = 60;
                    //設定要寫入的資料庫
                    bulkCopy.DestinationTableName = "HCourseDate";
                    //對應資料行
                    bulkCopy.ColumnMappings.Add("HCBatchNum", "HCBatchNum");
                    bulkCopy.ColumnMappings.Add("HCourseName", "HCourseName");
                    bulkCopy.ColumnMappings.Add("HDateRange", "HDateRange");
                    bulkCopy.ColumnMappings.Add("HDate", "HDate");
                    bulkCopy.ColumnMappings.Add("HStatus", "HStatus");
                    bulkCopy.ColumnMappings.Add("HCreate", "HCreate");
                    bulkCopy.ColumnMappings.Add("HCreateDT", "HCreateDT");

                    con.Open();
                    //開始寫入
                    bulkCopy.WriteToServer(dtCourseDateAdd);
                    con.Close();
                    //清除DataTable以釋放記憶體(如果不需要或有異常可先註解)
                    dtCourseDateAdd.Clear();
                }
            }

            #endregion





        }

        if (HID != "")
        {
            LB_HID.Text = HID;
        }
        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Trim(',').Split(',');
            for (int i = 0; i < strHID.Length; i++)
            {
                //EA20241120_學員課程教材內容更新
                for (int x = 0; x < Rpt_HCourseMaterial.Items.Count; x++)
                {

                    SqlDataReader drCM = SQLdatabase.ExecuteReader("SELECT HID, HCourseID FROM HCourseMaterial WHERE HCMName='" + ((TextBox)Rpt_HCourseMaterial.Items[x].FindControl("TB_HCMName")).Text.Trim() + "' AND HCMaterial='" + ((HyperLink)Rpt_HCourseMaterial.Items[x].FindControl("HL_HCMaterial")).Text.Trim() + "' AND HCMLink='" + ((TextBox)Rpt_HCourseMaterial.Items[x].FindControl("TB_HCMLink")).Text.Trim() + "' AND HCourseID='" + strHID[i] + "' ");

                    if (drCM.Read())
                    {

                        //SQL開始
                        SqlCommand cmd = new SqlCommand("UPDATE HCourseMaterial SET HSort=@HSort, HModify=@HModify,HModifyDT=@HModifyDT WHERE HCBatchNum=@HCBatchNum AND HCMName=@HCMName AND HCMaterial=@HCMaterial AND HCMLink=@HCMLink AND HID=@HID", con);
                        //AND HCMName=@HCMName AND HCMaterial=@HCMaterial AND HCMLink=@HCMLink
                        con.Open();
                        cmd.Parameters.AddWithValue("@HID", drCM["HID"].ToString());
                        cmd.Parameters.AddWithValue("@HCourseID", strHID[i]);
                        cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);
                        cmd.Parameters.AddWithValue("@HCMName", ((TextBox)Rpt_HCourseMaterial.Items[x].FindControl("TB_HCMName")).Text.Trim());
                        cmd.Parameters.AddWithValue("@HCMaterial", ((HyperLink)Rpt_HCourseMaterial.Items[x].FindControl("HL_HCMaterial")).Text.Trim());
                        cmd.Parameters.AddWithValue("@HCMLink", ((TextBox)Rpt_HCourseMaterial.Items[x].FindControl("TB_HCMLink")).Text.Trim());
                        cmd.Parameters.AddWithValue("@HSort", ((TextBox)Rpt_HCourseMaterial.Items[x].FindControl("TB_HSort")).Text.Trim());
                        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        cmd.ExecuteNonQuery();
                        con.Close();
                        cmd.Cancel();

                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO HCourseMaterial (HCourseID, HCMName, HCMaterial, HCMLink, HStatus, HCreate, HCreateDT, HSave, HCBatchNum, HSort) values (@HCourseID, @HCMName, @HCMaterial, @HCMLink, @HStatus, @HCreate, @HCreateDT, @HSave, @HCBatchNum, @HSort)", con);
                        con.Open();
                        cmd.Parameters.AddWithValue("@HCourseID", strHID[i]);
                        cmd.Parameters.AddWithValue("@HCMName", HttpUtility.HtmlEncode(((TextBox)Rpt_HCourseMaterial.Items[x].FindControl("TB_HCMName")).Text.Trim()));
                        cmd.Parameters.AddWithValue("@HCMaterial", ((HyperLink)Rpt_HCourseMaterial.Items[x].FindControl("HL_HCMaterial")).Text.Trim());
                        cmd.Parameters.AddWithValue("@HCMLink", HttpUtility.HtmlEncode(((TextBox)Rpt_HCourseMaterial.Items[x].FindControl("TB_HCMLink")).Text.Trim()));
                        cmd.Parameters.AddWithValue("@HStatus", "1");
                        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@HSave", "1");
                        cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);
                        cmd.Parameters.AddWithValue("@HSort", ((TextBox)Rpt_HCourseMaterial.Items[x].FindControl("TB_HSort")).Text.Trim());

                        cmd.ExecuteNonQuery();
                        con.Close();
                        cmd.Cancel();
                    }
                    drCM.Close();


                }

                //體系專業護持工作
                for (int x = 0; x < Rpt_HTodoList.Items.Count; x++)
                {
                    SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID FROM HTodoList WHERE HCourseID='" + strHID[i] + "' AND HGroupName='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupName")).SelectedValue + "' AND HTask='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HTask")).SelectedValue + "' AND HTaskContent='" + ((TextBox)Rpt_HTodoList.Items[x].FindControl("TB_HTaskContent")).Text + "' AND HGroupLeaderID='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupLeader")).SelectedValue + "' AND HStatus='1' AND HCBatchNum='" + LB_HCBatchNum.Text + "'");

                    if (dr.Read())
                    {
                        string strUpdHTL = "update HTodoList set HGroupName='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupName")).SelectedValue + "', HTask='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HTask")).SelectedValue + "', HTaskContent='" + ((TextBox)Rpt_HTodoList.Items[x].FindControl("TB_HTaskContent")).Text + "', HTaskNum='" + ((TextBox)Rpt_HTodoList.Items[x].FindControl("TB_HTaskNum")).Text + "', HGroupLeaderID='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupLeader")).SelectedValue + "', HStatus='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + strUniteTime + "', HSave='1' where HCourseID='" + strHID[i] + "' AND HCBatchNum = '" + LB_HCBatchNum.Text + "' AND HID='" + dr["HID"].ToString() + "'";
                        dbCmd = new SqlCommand(strUpdHTL, dbConn);
                        dbCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string strSelHTL_T = "insert into HTodoList (HCourseID, HGroupName, HTask, HTaskNum, HTaskContent, HGroupLeaderID, HStatus, HCreate, HCreateDT, HSave) values ('" + strHID[i] + "', '" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupName")).SelectedValue + "', '" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HTask")).SelectedValue + "','" + ((TextBox)Rpt_HTodoList.Items[x].FindControl("TB_HTaskNum")).Text + "', '" + HttpUtility.HtmlEncode(((TextBox)Rpt_HTodoList.Items[x].FindControl("TB_HTaskContent")).Text.Trim()) + "','" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupLeader")).SelectedValue + "', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '1')";

                        dbCmd = new SqlCommand(strSelHTL_T, dbConn);
                        dbCmd.ExecuteNonQuery();
                    }
                    dr.Close();


                }

                //講師教材
                for (int x = 0; x < Rpt_HTMaterialDetail.Items.Count; x++)
                {

                    SqlDataReader drCM = SQLdatabase.ExecuteReader("SELECT HID FROM HCourseTMaterial WHERE HTMaterialID='" + ((Label)Rpt_HTMaterialDetail.Items[x].FindControl("LB_HTMaterialID")).Text.Trim() + "' AND HSort='" + ((TextBox)Rpt_HTMaterialDetail.Items[x].FindControl("TB_HTMaterialSort")).Text.Trim() + "' AND HTMaterial='" + ((TextBox)Rpt_HTMaterialDetail.Items[x].FindControl("TB_HTMaterial")).Text.Trim() + "' AND HCourseID='" + strHID[i] + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "' AND HStatus='1'");

                    if (drCM.Read())
                    {
                        //SQL開始
                        SqlCommand cmd = new SqlCommand("UPDATE HCourseTMaterial SET HSort=@HSort, HSave=@HSave, HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HCBatchNum=@HCBatchNum AND HTMaterialID=@HTMaterialID AND HTMaterial=@HTMaterial AND HID=@HID", con);
                        //AND HCMName=@HCMName AND HCMaterial=@HCMaterial AND HCMLink=@HCMLink
                        con.Open();
                        cmd.Parameters.AddWithValue("@HID", drCM["HID"].ToString());
                        cmd.Parameters.AddWithValue("@HCourseID", strHID[i]);
                        cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);
                        cmd.Parameters.AddWithValue("@HTMaterialID", ((Label)Rpt_HTMaterialDetail.Items[x].FindControl("LB_HTMaterialID")).Text);
                        cmd.Parameters.AddWithValue("@HSort", ((TextBox)Rpt_HTMaterialDetail.Items[x].FindControl("TB_HTMaterialSort")).Text.Trim());
                        cmd.Parameters.AddWithValue("@HTMaterial", ((TextBox)Rpt_HTMaterialDetail.Items[x].FindControl("TB_HTMaterial")).Text);
                        cmd.Parameters.AddWithValue("@HSave", 1);
                        cmd.Parameters.AddWithValue("@HStatus", 1);
                        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        cmd.ExecuteNonQuery();
                        con.Close();
                        cmd.Cancel();



                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO HCourseTMaterial (HCBatchNum, HCourseID, HTMaterialID, HSort, HTMaterial, HStatus, HCreate, HCreateDT, HSave) values (@HCBatchNum, @HCourseID, @HTMaterialID, @HSort, @HTMaterial, @HStatus, @HCreate, @HCreateDT, @HSave)", con);
                        con.Open();


                        cmd.Parameters.AddWithValue("@HCourseID", strHID[i]);
                        cmd.Parameters.AddWithValue("@HTMaterialID", ((Label)Rpt_HTMaterialDetail.Items[x].FindControl("LB_HTMaterialID")).Text);
                        cmd.Parameters.AddWithValue("@HSort", ((TextBox)Rpt_HTMaterialDetail.Items[x].FindControl("TB_HTMaterialSort")).Text.Trim());
                        cmd.Parameters.AddWithValue("@HTMaterial", ((TextBox)Rpt_HTMaterialDetail.Items[x].FindControl("TB_HTMaterial")).Text);
                        cmd.Parameters.AddWithValue("@HStatus", "1");
                        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@HSave", "1");
                        cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);

                        cmd.ExecuteNonQuery();
                        con.Close();
                        cmd.Cancel();
                    }

                    drCM.Close();


                }


            }
        }


        //EE20241120_加入排序欄位；依排序欄位顯示
        SDS_HCourseMaterial.SelectCommand = "SELECT * FROM  (( SELECT HCMName, HCMaterial, HCMLink, HSave, HSort  FROM HCourseMaterial where HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave, HSort) EXCEPT ( SELECT HCMName, HCMaterial, HCMLink, HSave, HSort FROM HCourseMaterial_T where HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave, HSort))AS A ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC  ";
        SDS_HCourseMaterial.DataBind();

        //20220821體系護持工作項目改為可修改組長
        SDS_HTodoList.SelectCommand = "SELECT HGroupName, HTask, HTaskContent,HGroupLeaderID, HSave, HTaskNum, HExamStaff FROM HTodoList where HCBatchNum='" + LB_HCBatchNum.Text + "' AND HStatus=1 GROUP BY  HCBatchNum, HGroupName, HTask, HTaskContent,HGroupLeaderID, HSave, HTaskNum, HExamStaff  ORDER BY HGroupName ASC ";
        //SDS_HTodoList.DataBind();

        //WE20240329_加入作業類型、問卷、內容描述
        SDS_HCourseHWSetting.SelectCommand = "SELECT a.HCBatchNum, a.HNumbers, a.HDeadLine, a.HSave, a.HHWType, a.HQuestionHID, a.HDescription, b.HTitle FROM HCourse_HWSetting AS a  LEFT JOIN HQuestion AS b ON a.HQuestionHID= b.HID WHERE a.HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY a.HCBatchNum, a.HNumbers, a.HDeadLine, a.HSave,  a.HHWType, a.HQuestionHID, a.HDescription, b.HTitle  ORDER BY HDeadLine ASC";

        //EA20250409_講師教材設定
        SDS_HTMaterialDetail.SelectCommand = "SELECT HTMaterialID, HSort, HTMaterial, HSave FROM HCourseTMaterial WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HStatus='1' GROUP BY HTMaterialID, HSort, HTMaterial, HSave ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
        Rpt_HTMaterialDetail.DataBind();

        DataView dv_HCM = (DataView)SDS_HCourseMaterial.Select(DataSourceSelectArguments.Empty);
        for (int i = 0; i < dv_HCM.Count; i++)
        {
            if (dv_HCM[i]["HSave"].ToString() == "0")
            {

                string strUpdHCM = "update HCourseMaterial set HStatus='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + strUniteTime + "', HSave='1' where HCBatchNum = '" + LB_HCBatchNum.Text + "'";
                dbCmd = new SqlCommand(strUpdHCM, dbConn);
                dbCmd.ExecuteNonQuery();
            }

        }


        DataView dv_HHWSetting = (DataView)SDS_HCourseHWSetting.Select(DataSourceSelectArguments.Empty);
        for (int i = 0; i < dv_HHWSetting.Count; i++)
        {
            if (dv_HHWSetting[i]["HSave"].ToString() == "0")
            {
                string strUpdHWSetting = "update  HCourse_HWSetting set HStatus='1', HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HCreateDT='" + strUniteTime + "', HSave='1' where HCBatchNum = '" + LB_HCBatchNum.Text + "'";
                dbCmd = new SqlCommand(strUpdHWSetting, dbConn);
                dbCmd.ExecuteNonQuery();
            }

        }

        dbConn.Close();
        dbCmd.Cancel();

        Response.Write("<script>alert('存檔成功!');window.location.href='HCourse_Edit.aspx';</script>");

    }
    #endregion

    #region 送審功能
    protected void Btn_Verify_Click(object sender, EventArgs e)
    {
        #region 必填判斷

        string alert;
        if (!ValidateRequiredFields(out alert))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('" + alert + "');", true);
            return;
        }

        #endregion

        #region 課程名稱符號轉換
        string[] transf = { "＿", "－", "-" };

        for (int i = 0; i < transf.Length; i++)
        {
            if (TB_HCourseName.Text.IndexOf(transf[i]) >= 0)
            {
                TB_HCourseName.Text = TB_HCourseName.Text.Replace(transf[i], "_");
            }
        }

        if (TB_HCourseName.Text.IndexOf("(") >= 0)
        {
            TB_HCourseName.Text = TB_HCourseName.Text.Replace("(", "（");
        }
        if (TB_HCourseName.Text.IndexOf(")") >= 0)
        {
            TB_HCourseName.Text = TB_HCourseName.Text.Replace(")", "）");
        }

        #endregion


        #region  重複判斷
        int reHOCPlace = 0;
        string stHOCPlace = null;
        foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
        {
            if (LBoxHOCPlace.Selected == true)
            {
                SqlDataReader MyEBF = SQLdatabase.ExecuteReader("SELECT a.HOCPlace, b.HPlaceName FROM HCourse AS a LEFT JOIN HPlace AS b ON a.HOCPlace=b.HID WHERE a.HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' AND a.HCourseName='" + TB_HCourseName.Text + "' AND a.HOCPlace='" + LBoxHOCPlace.Value + "' AND a.HDateRange='" + TB_HDateRange.Text + "' AND a.HStatus='1' AND a.HSave='1' AND NOT EXISTS ( SELECT HCourseName FROM HCourse  tb WHERE  a.HCBatchNum ='" + LB_HCBatchNum.Text + "' )");

                if (MyEBF.Read())
                {
                    reHOCPlace += 1;
                    stHOCPlace += MyEBF["HPlaceName"].ToString() + ",";
                }
                MyEBF.Close();


            }
        }


        if (reHOCPlace != 0)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('上課地點：" + stHOCPlace.TrimEnd(',') + "已重複開課，再請協助確認，謝謝!');", true);
            //Response.Write("<script>alert('上課地點：" + stHOCPlace.TrimEnd(',') + "已重複開課，再請協助確認，謝謝!');</script>");
            return;
        }

        #endregion


        string Reciever = "";

        #region 寄信通知具審核權限者做開班審核 
        SqlDataReader QueryMail = SQLdatabase.ExecuteReader("SELECT C.HID,(SELECT DISTINCT(cast(B.HAccount AS NVARCHAR) + ',') FROM HRole CROSS APPLY SPLIT(HMemberID, ',') INNER JOIN HMember AS B ON value = B.HID WHERE  HRole.HID = C.HID FOR XML PATH('')) AS Email FROM HRole AS C WHERE C.HID=(SELECT TOP(1) HVRoleHID FROM HCVerifyUnit)");
        if (QueryMail.Read())
        {
            if (string.IsNullOrEmpty(QueryMail["Email"].ToString()))
            {
                Response.Write("<script>alert('審核單位尚未選擇管理人哦，請先建立常態任務中的權限名單~!');</script>");
                return;
            }
            else
            {
                Reciever = QueryMail["Email"].ToString().TrimEnd(',');
            }
        }
        QueryMail.Close();

        #endregion

        #region--圖片上傳
        string picname = "";
        if (LB_OldPic.Text != "" && FU_HImg.HasFile == true)
        {
            //上傳圖片
            Upload_Pic();

            picname = LB_Pic.Text;
        }
        else if (LB_OldPic.Text != "" && FU_HImg.HasFile == false)
        {
            picname = LB_OldPic.Text;
        }
        else if (LB_OldPic.Text == "" && FU_HImg.HasFile == true)
        {
            //上傳圖片
            Upload_Pic();
            picname = LB_Pic.Text;
        }
        #endregion

        bool fileIsValid = false;

        #region 上傳檔案
        if (FU_BCSchedule.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_BCSchedule.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_BCSchedule.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_BCSchedule.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }


        if (FU_BECSchedule.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_BECSchedule.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_BECSchedule.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_BECSchedule.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        if (FU_ICRecord.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_ICRecord.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_ICRecord.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_ICRecord.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        if (FU_DPosition.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_DPosition.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_DPosition.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_DPosition.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        if (FU_CPosition.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_CPosition.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_CPosition.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_CPosition.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        if (FU_TPosition.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_TPosition.FileName).ToLower();
            String[] restrictExtension = { ".xls", ".xlsx", ".pdf", ".doc", ".docx" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }

            if (fileIsValid)
            {

                //上傳檔是否大於10M
                if (FU_TPosition.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                    return;
                }
                else
                {
                    //上傳檔案
                    Upload_File();
                }
            }
            else
            {
                FU_TPosition.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        #endregion


        int gCT = 1;
        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        //將上傳之教材暫存檔名改為正式檔名
        string gHCMFileTemp = "~/uploads/CourseMaterialTemp/";
        string gHCMFileRoot = "~/uploads/CourseMaterial/";
        string gHCMFileName = "";//主檔名
                                 //string gHCMFileExtension = "";//副檔名

        string strSelHCM = "select HCMaterial from HCourseMaterial where HCBatchNum = '" + LB_HCBatchNum.Text + "' and HSave='0' GROUP BY HCMaterial";
        //string strSelHCM = "select HCMaterial from HCourseMaterial where HCourseID = '" + LB_HID.Text + "' and HSave='0'";
        dbCmd = new SqlCommand(strSelHCM, dbConn);
        SqlDataReader MyQueryHCM = dbCmd.ExecuteReader();

        while (MyQueryHCM.Read())
        {
            if (!string.IsNullOrEmpty(MyQueryHCM["HCMaterial"].ToString()))
            {
                //主檔名+副檔名
                gHCMFileName = MyQueryHCM["HCMaterial"].ToString();

                //取得檔名
                string[] Filename = gHCMFileName.Split('.');
                // string New_name = Filename[0].ToString() + "_" + DateTime.Now.ToString("yyMMdd") + "." + Filename[1].ToString(); //避免名稱重複 資料夾已有檔案會error
                //將上傳之教材從暫存區移動到正式區
                System.IO.File.Move(Server.MapPath(gHCMFileTemp) + gHCMFileName, Server.MapPath(gHCMFileRoot) + gHCMFileName);
            }
        }
        MyQueryHCM.Close();


        //取ListBox的值，使用ForEach方式
        string gLBox_HRSystem = "";
        foreach (ListItem LBoxHRSystem in LBox_HRSystem.Items)
        {
            if (LBoxHRSystem.Selected == true)
            {
                gLBox_HRSystem += LBoxHRSystem.Value + ",";
            }
        }

        string gLBox_HNRequirement = "";
        foreach (ListItem LBoxHNRequirement in LBox_HNRequirement.Items)
        {
            if (LBoxHNRequirement.Selected == true)
            {
                gLBox_HNRequirement += LBoxHNRequirement.Value + ",";
            }
        }

        string gLBox_HTeam = "";
        foreach (ListItem LBoxHTeam in LBox_HTeam.Items)
        {
            if (LBoxHTeam.Selected == true)
            {
                gLBox_HTeam += LBoxHTeam.Value + ",";
            }
        }

        string gLBox_HIRestriction = "";
        foreach (ListItem LBoxHIRestriction in LBox_HIRestriction.Items)
        {
            if (LBoxHIRestriction.Selected == true)
            {
                gLBox_HIRestriction += LBoxHIRestriction.Value + ",";
            }
        }

        string gLBox_HTeacherName = "";
        foreach (ListItem LBoxHTeacherName in LBox_HTeacherName.Items)
        {
            if (LBoxHTeacherName.Selected == true)
            {
                gLBox_HTeacherName += LBoxHTeacherName.Value + ",";
            }
        }

        string gLBox_HQuestionID = "";
        foreach (ListItem LBoxHQuestionID in LBox_HQuestionID.Items)
        {
            if (LBoxHQuestionID.Selected == true)
            {
                gLBox_HQuestionID += LBoxHQuestionID.Value + ",";
            }
        }

        string gLBox_HTMaterialID = "";

        //EA20240618_新增
        string gLBox_HExamPaperID = "";
        foreach (ListItem LBoxHExamPaperID in LBox_HExamPaperID.Items)
        {
            if (LBoxHExamPaperID.Selected == true)
            {
                gLBox_HExamPaperID += LBoxHExamPaperID.Value + ",";
            }
        }

        //EA20240618_新增
        string gLBox_HSupervise = "";
        foreach (ListItem LBoxHSupervise in LBox_HSupervise.Items)
        {
            if (LBoxHSupervise.Selected == true)
            {
                gLBox_HSupervise += LBoxHSupervise.Value + ",";
            }
        }


        //220819-基本費用(元)存入DB要先轉換成點數概念(1元=10點)
        int BasicPrice = Convert.ToInt32(TB_HBCPoint.Text.Trim()) / 10;
        string strUniteTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        string HID = "";

        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Trim(',').Split(',');

            //221018  -上課地點多選
            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    for (int i = 0; i < strHID.Length; i++)
                    {
                        //220823  --課程編輯不會update到審核紀錄
                        string strUpdHC = "update HCourse set HEnableSystem='', HApplySystem='', HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "', HCourseName='" + TB_HCourseName.Text + "', HTeacherName='" + gLBox_HTeacherName + "', HOCPlace='" + LBoxHOCPlace.Value + "', HDateRange='" + TB_HDateRange.Text + "', HSTime='" + TB_HSTime.Text + "', HETime='" + TB_HETime.Text + "', HType='" + DDL_HType.SelectedValue + "', HOSystem='" + DDL_HOSystem.SelectedValue + "', HRSystem='" + LB_HRSystem.Text + "', HNLCourse='" + RBL_HNLCourse.SelectedValue + "', HNGuide='" + RBL_HNGuide.SelectedValue + "', HNFull='" + RBL_HNFull.SelectedValue + "', HNRequirement='" + LB_HNRequirement.Text + "', HTeam='" + gLBox_HTeam + "',  HQuestionID='" + gLBox_HQuestionID + "', HPMethod='" + DDL_HPMethod.SelectedValue + "', HBCPoint='" + BasicPrice + "', HSGList='" + RBL_HSGList.SelectedValue + "', HIRestriction='" + LB_HIRestriction.Text + "', HRemark=N'" + HttpUtility.HtmlEncode(TB_HRemark.Text.Trim()) + "', HContentTitle=N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', HContent=N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', HRNContent=N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', HStatus='1', HVerifyNum='', HApplicant='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HApplyTime='" + strUniteTime + "', HVerifyTime='', HVerifyStatus='0', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + strUniteTime + "', HSave='1',HImg='" + picname + "', BCSchedule='" + LB_BCSchedule.Text + "' , BECSchedule='" + LB_BECSchedule.Text + "', ICRecord='" + LB_ICRecord.Text + "', DPosition='" + LB_DPosition.Text + "', CPosition='" + LB_CPosition.Text + "', TPosition='" + LB_TPosition.Text + "',HBudget='" + RBL_HBudget.SelectedValue + "', HBudgetTable='" + LB_HBudgetTable.Text + "', HLodging='" + RBL_HLodging.SelectedValue + "', HTMaterialID='" + gLBox_HTMaterialID + "', HCDeadline ='" + TB_HCDeadline.Text.Trim() + "', HCDeadlineDay ='" + TB_HCDeadlineDay.Text.Trim() + "', HDContinous='" + RBL_Continuous.SelectedValue + "', HBudgetType='" + TB_HBudgetType.Text + "', HCourseType='" + DDL_HCourseType.SelectedValue + "', HAxisYN='" + RBL_HAxisYN.SelectedValue + "', HAxisClass='" + DDL_HAxisClass.SelectedValue + "', HExamSubject='" + DDL_HExamSubject.SelectedValue + "', HGradeCalculation='" + DDL_HGradeCalculation.SelectedValue + "', HExamPaperID='" + gLBox_HExamPaperID + "',  HSupervise='" + gLBox_HSupervise + "', HAttRateStandard='" + TB_HAttRateStandard.Text + "', HExamPassStandard='" + TB_HExamPassStandard.Text + "', HExamContentID='" + DDL_HExamContentID.SelectedValue + "', HParticipantLimit='" + TB_HParticipantLimit.Text + "', HTeacherClass='" + DDL_HTeacherClass.SelectedValue + "', HTearcherLV='" + DDL_HTearcherLV.SelectedValue + "' where  HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'";
                        dbCmd = new SqlCommand(strUpdHC, dbConn);
                        dbCmd.ExecuteNonQuery();
                        //Response.Write(strUpdHC + "<br/>");
                    }

                    SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HCourseNum FROM HCourse WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'");
                    if (!dr.Read())
                    {
                        string strInsHC = "IF EXISTS(SELECT HCourseNum FROM HCourse WHERE SUBSTRING(HCourseNum,2,8) = CONVERT(nvarchar, getdate(), 112)) " +
   "BEGIN " +
       "insert into HCourse (HEnableSystem, HApplySystem, HCTemplateID, HCBatchNum, HCourseNum, HCourseName, HTeacherName, HOCPlace, HDateRange, HSTime, HETime, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam,  HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HRNContent, HVerifyNum, HApplicant, HApplyTime, HVerifyTime, HVerifyStatus, HStatus, HCreate, HCreateDT,HModify, HModifyDT,  HSave,HImg,BCSchedule, BECSchedule,  ICRecord, DPosition, CPosition, TPosition, HBudget, HBudgetTable, HSCourseID, HLodging, HTMaterialID,HCDeadline, HCDeadlineDay,HSerial,HDContinous, HBudgetType, HCourseType, HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID,  HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV) values ('', '', '" + DDL_HCourseTemplate.SelectedValue + "', '" + LB_HCBatchNum.Text + "', 'C' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HCourseNum), 12)) FROM HCourse) + 1), '" + TB_HCourseName.Text + "', '" + gLBox_HTeacherName + "', '" + LBoxHOCPlace.Value + "', '" + TB_HDateRange.Text + "', '" + TB_HSTime.Text + "', '" + TB_HETime.Text + "', '" + DDL_HType.SelectedValue + "','" + DDL_HOSystem.SelectedValue + "', '" + LB_HRSystem.Text + "', '" + RBL_HNLCourse.SelectedValue + "', '" + RBL_HNGuide.SelectedValue + "', '" + RBL_HNFull.SelectedValue + "','" + LB_HNRequirement.Text + "', '" + gLBox_HTeam + "',  '" + gLBox_HQuestionID + "', '" + DDL_HPMethod.SelectedValue + "', '" + BasicPrice + "', '" + RBL_HSGList.SelectedValue + "', '" + LB_HIRestriction.Text + "', N'" + HttpUtility.HtmlEncode(TB_HRemark.Text) + "', N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', '', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteTime + "', '', '0', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteTime + "',  '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteTime + "','1','" + picname + "','" + LB_BCSchedule.Text + "','" + LB_BECSchedule.Text + "','" + LB_ICRecord.Text + "','" + LB_DPosition.Text + "','" + LB_CPosition.Text + "','" + LB_TPosition.Text + "','" + RBL_HBudget.SelectedValue + "','" + LB_HBudgetTable.Text + "','" + DDL_HSCourse.SelectedValue + "','" + RBL_HLodging.SelectedValue + "', '" + gLBox_HTMaterialID + "','" + TB_HCDeadline.Text.Trim() + "', '" + TB_HCDeadlineDay.Text.Trim() + "', '" + RBL_HSerial.SelectedValue + "','" + RBL_Continuous.SelectedValue + "','" + TB_HBudgetType.Text + "', '" + DDL_HCourseType.SelectedValue + "', '" + RBL_HAxisYN.SelectedValue + "', '" + DDL_HAxisClass.SelectedValue + "', '" + DDL_HExamSubject.SelectedValue + "', '" + DDL_HGradeCalculation.SelectedValue + "', '" + gLBox_HExamPaperID + "',  '" + gLBox_HSupervise + "', '" + TB_HAttRateStandard.Text + "', '" + TB_HExamPassStandard.Text + "', '" + DDL_HExamContentID.SelectedValue + "', '" + TB_HParticipantLimit.Text + "', '" + DDL_HTeacherClass.SelectedValue + "', '" + DDL_HTearcherLV.SelectedValue + "');SELECT SCOPE_IDENTITY() AS HID " +
   "END " +
   "ELSE " +
   "BEGIN " +
       "insert into HCourse (HEnableSystem, HApplySystem, HCTemplateID, HCBatchNum, HCourseNum, HCourseName, HTeacherName, HOCPlace, HDateRange, HSTime, HETime, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam,  HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HRNContent, HVerifyNum, HApplicant, HApplyTime, HVerifyTime, HVerifyStatus, HStatus, HCreate, HCreateDT,HModify, HModifyDT,  HSave,HImg, BCSchedule, BECSchedule, ICRecord, DPosition, CPosition, TPosition, HBudget, HBudgetTable, HSCourseID, HLodging, HTMaterialID,HCDeadline, HCDeadlineDay,HSerial,HDContinous, HBudgetType,HCourseType , HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID,  HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV) values ('', '', '" + DDL_HCourseTemplate.SelectedValue + "', '" + LB_HCBatchNum.Text + "', 'C' + CONVERT(nvarchar, getdate(), 112) + '0001', '" + TB_HCourseName.Text + "', '" + gLBox_HTeacherName + "', '" + LBoxHOCPlace.Value + "', '" + TB_HDateRange.Text + "', '" + TB_HSTime.Text + "', '" + TB_HETime.Text + "', '" + DDL_HType.SelectedValue + "','" + DDL_HOSystem.SelectedValue + "', '" + LB_HRSystem.Text + "', '" + RBL_HNLCourse.SelectedValue + "', '" + RBL_HNGuide.SelectedValue + "', '" + RBL_HNFull.SelectedValue + "','" + LB_HNRequirement.Text + "', '" + gLBox_HTeam + "','" + gLBox_HQuestionID + "', '" + DDL_HPMethod.SelectedValue + "', '" + BasicPrice + "', '" + RBL_HSGList.SelectedValue + "', '" + LB_HIRestriction.Text + "', N'" + HttpUtility.HtmlEncode(TB_HRemark.Text) + "', N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', '', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteTime + "', '', '0', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteTime + "', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteTime + "', '1','" + picname + "','" + LB_BCSchedule.Text + "','" + LB_BECSchedule.Text + "','" + LB_ICRecord.Text + "','" + LB_DPosition.Text + "','" + LB_CPosition.Text + "','" + LB_TPosition.Text + "','" + RBL_HBudget.SelectedValue + "','" + LB_HBudgetTable.Text + "','" + DDL_HSCourse.SelectedValue + "','" + RBL_HLodging.SelectedValue + "', '" + gLBox_HTMaterialID + "','" + TB_HCDeadline.Text.Trim() + "', '" + TB_HCDeadlineDay.Text.Trim() + "', '" + RBL_HSerial.SelectedValue + "', '" + RBL_Continuous.SelectedValue + "','" + TB_HBudgetType.Text + "',  '" + DDL_HCourseType.SelectedValue + "', '" + RBL_HAxisYN.SelectedValue + "', '" + DDL_HAxisClass.SelectedValue + "', '" + DDL_HExamSubject.SelectedValue + "', '" + DDL_HGradeCalculation.SelectedValue + "', '" + gLBox_HExamPaperID + "',  '" + gLBox_HSupervise + "', '" + TB_HAttRateStandard.Text + "', '" + TB_HExamPassStandard.Text + "', '" + DDL_HExamContentID.SelectedValue + "', '" + TB_HParticipantLimit.Text + "', '" + DDL_HTeacherClass.SelectedValue + "', '" + DDL_HTearcherLV.SelectedValue + "');SELECT SCOPE_IDENTITY() AS HID " +
   "END";
                        dbCmd = new SqlCommand(strInsHC, dbConn);
                        //dbCmd.ExecuteNonQuery();
                        HID += dbCmd.ExecuteScalar().ToString() + ",";
                        //dbCmd.Cancel();

                    }
                    dr.Close();

                }


            }



        }


        if (HID != "")
        {
            LB_HID.Text = HID;
        }

        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Trim(',').Split(',');
            for (int i = 0; i < strHID.Length; i++)
            {
                for (int x = 0; x < Rpt_HTodoList.Items.Count; x++)
                {
                    SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID FROM HTodoList WHERE HCourseID='" + strHID[i] + "' AND HGroupName='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupName")).SelectedValue + "' AND HTask='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HTask")).SelectedValue + "' AND HTaskContent='" + ((TextBox)Rpt_HTodoList.Items[x].FindControl("TB_HTaskContent")).Text + "' AND HGroupLeaderID='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupLeader")).SelectedValue + "' AND HStatus='1' AND HCBatchNum='" + LB_HCBatchNum.Text + "'");

                    if (dr.Read())
                    {
                        string strUpdHTL = "update HTodoList set HGroupName='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupName")).SelectedValue + "', HTask='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HTask")).SelectedValue + "', HTaskContent='" + ((TextBox)Rpt_HTodoList.Items[x].FindControl("TB_HTaskContent")).Text + "', HGroupLeaderID='" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupLeader")).SelectedValue + "', HStatus='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + strUniteTime + "', HSave='1' where HCourseID='" + strHID[i] + "' AND HCBatchNum = '" + LB_HCBatchNum.Text + "' AND HID='" + dr["HID"].ToString() + "'";
                        dbCmd = new SqlCommand(strUpdHTL, dbConn);
                        dbCmd.ExecuteNonQuery();


                    }
                    else
                    {
                        string strInsHTL = "insert into HTodoList (HCourseID, HGroupName, HTask, HTaskContent, HGroupLeaderID, HStatus, HCreate, HCreateDT, HSave, HCBatchNum) values ('" + strHID[i] + "', '" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupName")).SelectedValue + "', '" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HTask")).SelectedValue + "', '" + ((TextBox)Rpt_HTodoList.Items[x].FindControl("TB_HTaskContent")).Text + "', '" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupLeader")).SelectedValue + "', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '1','" + LB_HCBatchNum.Text + "')";

                        dbCmd = new SqlCommand(strInsHTL, dbConn);
                        dbCmd.ExecuteNonQuery();
                    }
                    dr.Close();

                }


            }
        }


        //EE20241120_加入排序欄位；依排序欄位顯示
        SDS_HCourseMaterial.SelectCommand = "SELECT * FROM  (( SELECT HCMName, HCMaterial, HCMLink, HSave, HSort  FROM HCourseMaterial where HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave, HSort) EXCEPT ( SELECT HCMName, HCMaterial, HCMLink, HSave, HSort FROM HCourseMaterial_T where HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave, HSort))AS A ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC  ";


        //20220821體系護持工作項目改為可修改組長
        SDS_HTodoList.SelectCommand = "SELECT HGroupName, HTask, HTaskContent,HGroupLeaderID, HSave, HTaskNum, HExamStaff FROM HTodoList where HCBatchNum='" + LB_HCBatchNum.Text + "' AND HStatus=1 GROUP BY  HCBatchNum, HGroupName, HTask, HTaskContent,HGroupLeaderID, HSave, HTaskNum, HExamStaff  ORDER BY HGroupName ASC";


        //WE20240329_加入作業類型、問卷、內容描述
        SDS_HCourseHWSetting.SelectCommand = "SELECT a.HCBatchNum, a.HNumbers, a.HDeadLine, a.HSave, a.HHWType, a.HQuestionHID, a.HDescription, b.HTitle FROM HCourse_HWSetting AS a  LEFT JOIN HQuestion AS b ON a.HQuestionHID= b.HID WHERE a.HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY a.HCBatchNum, a.HNumbers, a.HDeadLine, a.HSave,  a.HHWType, a.HQuestionHID, a.HDescription, b.HTitle  ORDER BY HDeadLine ASC";

        SDS_HCourseEvaluation.SelectCommand = "SELECT HCBatchNum, HCEType, HCEContent, HCENum FROM HCourseEvaluation WHERE HStatus=1 AND HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY  HCBatchNum, HCEType, HCEContent, HCENum";
        Rpt_HCourseEvaluation.DataBind();

        //EA20250409_講師教材設定
        SDS_HTMaterialDetail.SelectCommand = "SELECT HTMaterialID, HSort, HTMaterial, HSave FROM HCourseTMaterial WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HStatus='1' GROUP BY HTMaterialID, HSort, HTMaterial, HSave ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
        Rpt_HTMaterialDetail.DataBind();

        DataView dv_HCM = (DataView)SDS_HCourseMaterial.Select(DataSourceSelectArguments.Empty);
        for (int i = 0; i < dv_HCM.Count; i++)
        {
            if (dv_HCM[i]["HSave"].ToString() == "0")
            {

                string strUpdHCM = "update HCourseMaterial set HStatus='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + strUniteTime + "', HSave='1' where HCBatchNum = '" + LB_HCBatchNum.Text + "'";
                dbCmd = new SqlCommand(strUpdHCM, dbConn);
                dbCmd.ExecuteNonQuery();
            }
        }







        DataView dv_HHWSetting = (DataView)SDS_HCourseHWSetting.Select(DataSourceSelectArguments.Empty);
        for (int i = 0; i < dv_HHWSetting.Count; i++)
        {
            if (dv_HHWSetting[i]["HSave"].ToString() == "0")
            {
                string strUpdHWSetting = "update  HCourse_HWSetting set HStatus='1', HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HCreateDT='" + strUniteTime + "', HSave='1' where HCBatchNum = '" + LB_HCBatchNum.Text + "'";
                dbCmd = new SqlCommand(strUpdHWSetting, dbConn);
                dbCmd.ExecuteNonQuery();
            }
        }






        #region 寄信
        //信件本體宣告
        MailMessage mail = new MailMessage();
        // 寄件者, 收件者和副本郵件地址        
        mail.From = new MailAddress(Sender, "和氣大愛玉成系統自動寄信機器人");
        // 設定收件者
        mail.To.Add(Reciever);
        // 優先等級
        mail.Priority = MailPriority.Normal;
        // 主旨
        mail.Subject = "和氣大愛玉成系統 - 開課審核通知";
        //信件內容         
        mail.Body = "<p>課程名稱：【" + TB_HCourseName.Text + "】需要審核哦~</p><p>請盡快至玉成系統後台 > 簽核管理 > 開班審核中做審核~謝謝~</p><br/><hr/><p style='font-weight:bold;'> 此郵件為和氣大愛玉成系統自動寄出，請勿回信。</p>";

        mail.IsBodyHtml = true;
        mail.SubjectEncoding = Encoding.UTF8;
        mail.BodyEncoding = Encoding.GetEncoding("utf-8");
        SmtpClient smtpServer = new SmtpClient();
        smtpServer.Credentials = new System.Net.NetworkCredential(EAcount, EPasword);
        smtpServer.Port = EPort;
        smtpServer.Host = EHost;
        smtpServer.EnableSsl = EEnabledSSL;
        try
        {
            // 寄出郵件
            smtpServer.Send(mail);
            Response.Write("<script>alert('課程送審成功~並通知審核單位盡快做開班審核囉~');window.location.href='HCourse_Edit.aspx';</script>");

        }
        catch (Exception ex)
        {
            //寄驗證信失敗
            Response.Write("<script>alert('寄信失敗！請確認信箱是否正確');</script>");

        }
        #endregion


        dbConn.Close();
        dbCmd.Cancel();

        //Response.Write("<script>alert('存檔成功!');window.location.href='HCourse_Edit.aspx';</script>");
    }
    #endregion

    #region 取消功能
    protected void Btn_Cancel_Click(object sender, EventArgs e)
    {
        Response.Write("<script>window.location.href='HCourse_Edit.aspx';</script>");

    }
    #endregion

    #region 上傳照片 (.gif, .jpg, .bmp, .png)
    protected void Upload_Pic()
    {
        bool fileIsValid = false;
        if (FU_HImg.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_HImg.FileName).ToLower();
            String[] restrictExtension = { ".gif", ".jpg", ".bmp", ".png", ".jpeg" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }
            if (fileIsValid)
            {
                //上傳檔是否大於10M
                if (FU_HImg.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的圖片不能超過10MB喔！');</script>");
                    return;
                }

                //建立資料夾
                Directory.CreateDirectory(CourseImgRoot);

                //檔名
                LB_Pic.Text = "CourseImg_" + DateTime.Now.ToString("yyMMddmmss") + fileExtension;

                this.FU_HImg.SaveAs(Server.MapPath(CourseImg + LB_Pic.Text));
            }
            else
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert", "<script>alert('只能上傳.gif.jpg.bmp.png的圖檔喔！');</script>");
            }
        }
    }
    #endregion

    #region--移除圖片-
    protected void Btn_Del_Click(object sender, EventArgs e)
    {
        Btn_Del.Visible = false;
        IMG_Pic.ImageUrl = "";
        IMG_Pic.Width = 0;
    }
    #endregion

    #region 上傳檔案 (.xls、.xlsx、pdf)
    protected void Upload_File()
    {
        //建立資料夾
        Directory.CreateDirectory(CourseFileRoot);


        #region 經費預算表
        if (FU_HBudgetTable.HasFile)
        {

            //上傳檔是否大於10M
            if (FU_HBudgetTable.PostedFile.ContentLength > (10 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_HBudgetTable.FileName);
                //檔名
                LB_HBudgetTable.Text = "01" + DateTime.Now.ToString("yyMMddmmss") + FileExtension;

                this.FU_HBudgetTable.SaveAs(Server.MapPath("~/uploads/CourseFile/" + LB_HBudgetTable.Text));
            }
        }
        #endregion


        #region 課前課程表
        if (FU_BCSchedule.HasFile)
        {

            //上傳檔是否大於10M
            if (FU_BCSchedule.PostedFile.ContentLength > (10 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_BCSchedule.FileName);
                //檔名
                LB_BCSchedule.Text = "01" + DateTime.Now.ToString("yyMMddmmss") + FileExtension;

                this.FU_BCSchedule.SaveAs(Server.MapPath("~/uploads/CourseFile/" + LB_BCSchedule.Text));
            }
        }
        #endregion

        #region 課前簡易課程表
        if (FU_BECSchedule.HasFile)
        {

            //上傳檔是否大於10M
            if (FU_BECSchedule.PostedFile.ContentLength > (10 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_BECSchedule.FileName);
                //檔名
                LB_BECSchedule.Text = "02" + DateTime.Now.ToString("yyMMddmmss") + FileExtension;

                this.FU_BECSchedule.SaveAs(Server.MapPath("~/uploads/CourseFile/" + LB_BECSchedule.Text));
            }
        }
        #endregion

        #region 課中記錄
        if (FU_ICRecord.HasFile)
        {

            //上傳檔是否大於10M
            if (FU_ICRecord.PostedFile.ContentLength > (10 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_ICRecord.FileName);
                //檔名
                LB_ICRecord.Text = "03" + DateTime.Now.ToString("yyMMddmmss") + FileExtension;

                this.FU_ICRecord.SaveAs(Server.MapPath("~/uploads/CourseFile/" + LB_ICRecord.Text));
            }
        }
        #endregion

        #region 請法站位圖
        if (FU_DPosition.HasFile)
        {

            //上傳檔是否大於10M
            if (FU_DPosition.PostedFile.ContentLength > (10 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_DPosition.FileName);
                //檔名
                LB_DPosition.Text = "04" + DateTime.Now.ToString("yyMMddmmss") + FileExtension;

                this.FU_DPosition.SaveAs(Server.MapPath("~/uploads/CourseFile/" + LB_DPosition.Text));
            }
        }
        #endregion

        #region 課中記錄
        if (FU_CPosition.HasFile)
        {

            //上傳檔是否大於10M
            if (FU_CPosition.PostedFile.ContentLength > (10 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_CPosition.FileName);
                //檔名
                LB_CPosition.Text = "05" + DateTime.Now.ToString("yyMMddmmss") + FileExtension;

                this.FU_CPosition.SaveAs(Server.MapPath("~/uploads/CourseFile/" + LB_CPosition.Text));
            }
        }
        #endregion

        #region 課中記錄
        if (FU_TPosition.HasFile)
        {

            //上傳檔是否大於10M
            if (FU_TPosition.PostedFile.ContentLength > (10 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_TPosition.FileName);
                //檔名
                LB_TPosition.Text = "06" + DateTime.Now.ToString("yyMMddmmss") + FileExtension;

                this.FU_TPosition.SaveAs(Server.MapPath("~/uploads/CourseFile/" + LB_TPosition.Text));
            }
        }
        #endregion

    }
    #endregion

    #region 刪除已上傳的檔案
    protected void LBtn_BCScheduleDel_Click(object sender, EventArgs e)
    {
        LBtn_BCScheduleDel.Visible = false;
        HL_BCSchedule.Font.Strikeout = true;
        HL_BCSchedule.ForeColor = Color.Gray;
        LB_BCSchedule.Text = "";

    }

    protected void LBtn_BECScheduleDel_Click(object sender, EventArgs e)
    {
        LBtn_BECScheduleDel.Visible = false;
        HL_BECSchedule.Font.Strikeout = true;
        HL_BECSchedule.ForeColor = Color.Gray;
        LB_BECSchedule.Text = "";

    }

    protected void LBtn_ICRecordDel_Click(object sender, EventArgs e)
    {
        LBtn_ICRecordDel.Visible = false;
        HL_ICRecord.Font.Strikeout = true;
        HL_ICRecord.ForeColor = Color.Gray;
        LB_ICRecord.Text = "";


    }

    protected void LBtn_DPositionDel_Click(object sender, EventArgs e)
    {
        LBtn_DPositionDel.Visible = false;
        HL_DPosition.Font.Strikeout = true;
        HL_DPosition.ForeColor = Color.Gray;
        LB_DPosition.Text = "";

    }

    protected void LBtn_CPositionDel_Click(object sender, EventArgs e)
    {
        LBtn_CPositionDel.Visible = false;
        HL_CPosition.Font.Strikeout = true;
        HL_CPosition.ForeColor = Color.Gray;
        LB_CPosition.Text = "";

    }


    protected void LBtn_TPositionDel_Click(object sender, EventArgs e)
    {
        LBtn_TPositionDel.Visible = false;
        HL_TPosition.Font.Strikeout = true;
        HL_TPosition.ForeColor = Color.Gray;
        LB_TPosition.Text = "";
    }
    #endregion

    #region 課程日期連續與否
    protected void RBL_Continuous_SelectedIndexChanged(object sender, EventArgs e)
    {
        TB_HDateRange.Text = null;
        TB_HDateRange.CssClass = RBL_Continuous.SelectedValue == "0" ? "form-control datemultipicker" : "form-control daterange";
        RegScript();//註冊js
    }
    #endregion

    #region Tab切換
    protected void LBtn_NavTab_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        Status(btn.TabIndex);
        btn.CssClass = "nav-link active show";

        RegisterAsync();
        RegScript();//註冊js
    }
    #endregion

    #region Tab切換Function
    public void Status(int TabIndex)
    {
        //RegisterAsync();
        LB_NavTab.Text = TabIndex.ToString();

        LBtn_Template.CssClass = "nav-link";
        LBtn_Course.CssClass = "nav-link";
        LBtn_Introduction.CssClass = "nav-link";
        LBtn_Material.CssClass = "nav-link";
        LBtn_LeadCourse.CssClass = "nav-link";
        LBtn_SupportJob.CssClass = "nav-link";
        LBtn_Notice.CssClass = "nav-link";
        LBtn_Homework.CssClass = "nav-link";
        LBtn_Related.CssClass = "nav-link";
        LBtn_Evaluation.CssClass = "nav-link";
        LBtn_HTMaterial.CssClass = "nav-link";

        //EE20240624_修改寫法
        Panel[] Tabstr = { Panel_Template, Panel_Course, Panel_Content, Panel_Material, Panel_LeadingCourse, Panel_Jobs, Panel_Notice, Panel_Homework, Panel_Related, Panel_HCourseEvaluation, Panel_HTMaterial };

        for (int k = 0; k < Tabstr.Length; k++)
        {
            Tabstr[k].Attributes.Add("class", "tab-pane fade");

            if (k == TabIndex - 1)
            {
                Tabstr[k].Attributes.Add("class", "tab-pane fade active show");


                //課程範本
                if (TabIndex == 1)
                {
                    //課程範本
                    BindListControls("SELECT HID, (HTemplateNum + '-' + HTemplateName) AS HCourse, HStatus FROM HCourse_T ORDER BY HTemplateNum", "HCourse", "HID", "-請選擇-", true, DDL_HCourseTemplate);

                    //課程類別
                    BindListControls("SELECT HID, HCourseName, HStatus FROM HCourse_Class WHERE HStatus=1 ORDER BY HStatus DESC", "HCourseName", "HID", "-請選擇-", true, DDL_HType);

                    //預算類別
                    BindListControls("SELECT HID, HContent, HStatus FROM HBudgetType WHERE HStatus=1", "HContent", "HID", "-請選擇-", true, DDL_HBudgetType);

                    //可開課之體系、可報名之體系
                    BindListControls("SELECT HID, HSystemName, HStatus FROM HSystem WHERE HStatus=1", "HSystemName", "HID", "-請選擇-", true, DDL_HOSystem, LBox_HRSystem);

                    //學員類別限制
                    BindListControls("SELECT HID, HMType, HStatus FROM HMType WHERE HStatus='1' ORDER BY HID ASC", "HMType", "HID", "-請選擇-", true, LBox_HIRestriction);

                    //受傳過之法條件
                    BindListControls("SELECT HID, HDTypeID, HDharmaName, HStatus FROM HDharma WHERE HStatus='1'", "HDharmaName", "HID", "-請選擇-", true, LBox_HNRequirement);

                    //問卷
                    BindListControls("SELECT HID, HTitle, HStatus FROM HQuestion ORDER BY HTitle", "HTitle", "HID", "-請選擇-", true, LBox_HQuestionID, DDL_HQuestion);

                    ////講師教材
                    BindListControls("SELECT a.HID, (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) AS HTMName, a.HTMaterial, a.HStatus FROM   HTeacherMaterial AS a LEFT JOIN HDocCTypeClass AS b ON a.HDocCTClassID=b.HID LEFT JOIN HCourseLevel AS c ON a.HCourseLevelID=c.HID LEFT JOIN HFileType AS d ON a.HFileTypeID=d.HID WHERE (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) <>'' AND (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) IS NOT NULL ORDER BY a.HStatus DESC", "HTMName", "HID", "-請選擇-", true, LBox_HTMaterialID);

                    //帶狀課程
                    BindListControls("SELECT HID, (HCourseName + '_' + HPlaceName) AS HSCourse, HStatus FROM Course_BackendList WHERE HVerifyStatus = '2' AND HStatus = '1' ORDER BY HID DESC", "HSCourse", "HID", "-請選擇-", true, DDL_HSCourse);

                    //檢覈內容名稱
                    //BindListControls("SELECT HID, (HExamContentName+'-'+(CASE HExamType WHEN 1 THEN '筆試' WHEN 2 THEN '實作' WHEN '3' THEN '試教' ELSE '' END)) AS HExamContentName, HStatus  FROM HExamContent WHERE HStatus=1", "HExamContentName", "HID", "-請選擇-", true, DDL_HExamContentID);

                    //檢覈科目名稱
                    //BindListControls("SELECT HID, HExamSubject FROM HExamSubject WHERE HStatus=1", "HExamContentName", "HID", "-請選擇-", true, DDL_HExamSubject);

                    //考卷
                    //BindListControls("SELECT HID, HTitle FROM HQuestion  WHERE HStatus=1 ORDER BY HTitle", "HTitle", "HID", "-請選擇-", true, LBox_HExamPaperID);
                }
                //開課資訊
                else if (TabIndex == 2)
                {

                    //上課地點
                    BindListControls("SELECT HID, HPlaceName, HStatus FROM HPlaceList WHERE HStatus=1 ORDER BY HStatus DESC, strHAreaID ASC", "HPlaceName", "HID", "-請選擇-", true, LBox_HOCPlace, DDL_HOCPlace, DDL_SHOCPlace);

                    //主班團隊--二階以上可成為主班
                    BindListControls("select HMember.HID, (HArea+'/'+HPeriod+' '+HUserName) as UserName, HMember.HStatus from HMember Left Join HArea On HMember.HAreaID =HArea.HID WHERE  HMember.HStatus =1 AND HType in(7,8,9,10,11,12,13)   order by HUserName", "UserName", "HID", "-請選擇-", true, LBox_HTeam);

                    //課程講師
                    BindListControls("SELECT HTeacher.HID, HTeacher.HStatus,  (HArea+'/'+HPeriod+' '+HUserName+'-'+ISNULL(HCourseName, '')) AS HTeacherName FROM HTeacher INNER JOIN HMember ON HMemberID = HMember.HID LEFT JOIN HArea ON HMember.HAreaID = HArea.HID LEFT JOIN HCourse ON HCourseID=HCourse.HID", "HTeacherName", "HID", "-請選擇-", true, LBox_HTeacherName);


                    //EA20250531_檢覈課程=>上課地點為檢覈用地點；無法變更
                    //WE20250605_更新至正式區，暫時註解
                    //if (RBL_TestCourse.SelectedValue == "1")
                    //{
                    //    if (LB_CopyPage.Text == "True")
                    //    {
                    //        SetSelectedItems(LBox_HOCPlace, "31");
                    //    }

                    //    LBox_HOCPlace.Enabled = false;
                    //}


                    //督導
                    //BindListControls("SELECT HMember.HID, (HArea+'/'+HPeriod+' '+HUserName) as UserName, HMember.HStatus from HMember Left Join HArea On HMember.HAreaID =HArea.HID WHERE HMember.HStatus=1 AND HType in(7,8,9,10,11,12,13)  order by HUserName", "UserName", "HID", "-請選擇-", true, LBox_HSupervise);



                    //DDL_HTeacherClass.DataSource = DDL_ListItem.HTeacher.HTeacherClass;
                    //DDL_HTeacherClass.DataTextField = "Value";
                    //DDL_HTeacherClass.DataValueField = "Key";
                    //DDL_HTeacherClass.DataBind();

                    //DDL_HTearcherLV.DataSource = DDL_ListItem.HTeacher.HTearcherLV;
                    //DDL_HTearcherLV.DataTextField = "Value";
                    //DDL_HTearcherLV.DataValueField = "Key";
                    //DDL_HTearcherLV.DataBind();


                }
                //學員課程教材
                else if (TabIndex == 4)
                {
                    //EE20241120_加入排序欄位；依排序欄位顯示
                    SDS_HCourseMaterial.SelectCommand = "SELECT * FROM  (( SELECT HCMName, HCMaterial, HCMLink, HSave, HSort  FROM HCourseMaterial where HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave, HSort) EXCEPT ( SELECT HCMName, HCMaterial, HCMLink, HSave, HSort FROM HCourseMaterial_T where HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave, HSort))AS A ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC  ";
                    Rpt_HCourseMaterial.DataSourceID = "SDS_HCourseMaterial";
                    Rpt_HCourseMaterial.DataBind();
                }
                //前導課程
                else if (TabIndex == 5) //WE20231217_改成另開顯示前導課程設定頁面
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "newpage", "window.open('HLCourseRelated_Add.aspx','_parent,toolbar=yes,location=no,directories=yes,status=yes,menubar=yes,resizable=yes,scrollbars=yes');", true);
                }
                //體系護持工作
                else if (TabIndex == 6)
                {
                    ////體系護持工作項目-所屬組別
                    BindListControls("SELECT HID, HGroupName, HStatus FROM HGroup WHERE HStatus='1'", "HGroupName", "HID", "-請選擇-", true, DDL_HGroupName);

                    ////體系護持工作組長
                    BindListControls("SELECT a.HID, (b.HArea+'/'+a.HPeriod+' '+a.HUserName) as UserName, a.HStatus from HMember AS a Left Join HArea AS b On a.HAreaID =b.HID WHERE a.HStatus=1 AND HType <>'1' order by a.HUserName", "UserName", "HID", "-請選擇-", true, DDL_HGroupLeader);


                    //20220821體系護持工作項目改為可修改組長
                    SDS_HTodoList.SelectCommand = "SELECT HGroupName, HTask, HTaskContent, HGroupLeaderID,HSave, HTaskNum, HExamStaff FROM HTodoList WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HStatus=1 GROUP BY HCBatchNum, HGroupName, HTask, HTaskContent, HGroupLeaderID,HSave, HTaskNum, HExamStaff ORDER BY HGroupName ASC";
                    //Rpt_HTodoList.DataBind();
                }
            }
        }

    }
    #endregion

    #region 刪除已上傳的檔案
    protected void LBtn_HBudgetTableDel_Click(object sender, EventArgs e)
    {
        LBtn_HBudgetTableDel.Visible = false;
        HL_HBudgetTable.Font.Strikeout = true;
        HL_HBudgetTable.ForeColor = Color.Gray;
        LB_HBudgetTable.Text = "";
    }
    #endregion

    #region 執行js
    public void RegScript()
    {
        //TB_HDateRange.CssClass = RBL_Continuous.SelectedValue == "0" ? "form-control datemultipicker" : "form-control daterange";

        string script = @"
        $(function () {
            $('.js-example-basic-single').select2({ closeOnSelect: true });

            $('.ListB_Multi').SumoSelect({
                placeholder: '-請選擇-',
                search: true,
                csvDispCount: 5
            });

            $('.timepicker').timepicker({
                timeFormat: 'HH:mm',
                interval: 30,
                minTime: '4',
                maxTime: '23:00',
                dynamic: false,
                dropdown: true,
                scrollbar: true
            });

            $('.daterange').daterangepicker({
                opens: 'right',
                autoUpdateInput: false,
                locale: {
                    cancelLabel: 'Clear',
                    format: 'YYYY/MM/DD'
                }
            });

            $('.daterange').on('apply.daterangepicker', function(ev, picker) {
                $(this).val(picker.startDate.format('YYYY/MM/DD') + ' - ' + picker.endDate.format('YYYY/MM/DD'));
            });

            $('.daterange').on('cancel.daterangepicker', function(ev, picker) {
                $(this).val('');
            });

            $('.datemultipicker').datepicker({
                language: 'en',
                dateFormat: 'yyyy/mm/dd',
                multipleDates: true,
                todayHighlight: true,
                orientation: 'bottom auto',
onShow: function(dp, animationCompleted) {
        var val = $(this.$el).val();
        if (val) {
            var arr = val.split(',');
            this.selectDate([]);
            arr.forEach(function(dateStr) {
                dateStr = dateStr.trim();
                if (dateStr) {
                    var d = new Date(dateStr.replace(/\//g, '-'));
                    if (!isNaN(d)) this.selectDate(d);
                }
            }, this);
        }
    }
            });

            $('.datesinglepicker').datepicker({
                language: 'en',
                dateFormat: 'yyyy/mm/dd',
                multipleDates: false,
                minDate: new Date(),
                todayHighlight: true,
                orientation: 'bottom auto'
            });

            $('.dropify').dropify();

        });
    ";

        ScriptManager.RegisterStartupScript(Page, GetType(), "AllInitScripts", script, true);


    }
    #endregion

    #region 註冊非同步事件
    protected void RegisterAsync()
    {
        for (int x = 0; x < Rpt_HTodoList.Items.Count; x++)
        {
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Rpt_HTodoList.Items[x].FindControl("LBtn_HTodoList_Del"));
        }

    }
    #endregion

    #region 檢覈評比項目
    //EA20240624_新增檢覈評比項目：新增功能、刪除功能
    #region 檢覈評比項目-新增功能
    protected void LBtn_HCE_Add_Click(object sender, EventArgs e)
    {
        #region 必填判斷
        string alert;
        if (!ValidateRequiredFields(out alert))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('" + alert + "');", true);
            return;
        }

        if (string.IsNullOrEmpty(TB_HCENum.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('請輸入數量');", true);
            return;
        }

        #endregion


        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        string HID = "";
        string gLBox_HRSystem = "", gLBox_HNRequirement = "", gLBox_HTeam = "", gLBox_HIRestriction = "", gLBox_HTeacherName = "", gLBox_HQuestionID = "", gLBox_HTMaterialID = "", gLBox_HExamPaperID = "", gLBox_HSupervise = "";

        #region 課程名稱符號轉換
        string[] transf = { "＿", "－", "-" };

        for (int i = 0; i < transf.Length; i++)
        {
            if (TB_HCourseName.Text.IndexOf(transf[i]) >= 0)
            {
                TB_HCourseName.Text = TB_HCourseName.Text.Replace(transf[i], "_");
            }
        }

        if (TB_HCourseName.Text.IndexOf("(") >= 0)
        {
            TB_HCourseName.Text = TB_HCourseName.Text.Replace("(", "（");
        }
        if (TB_HCourseName.Text.IndexOf(")") >= 0)
        {
            TB_HCourseName.Text = TB_HCourseName.Text.Replace(")", "）");
        }

        #endregion

        #region  重複判斷
        int reHOCPlace = 0;
        string stHOCPlace = null;
        foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
        {
            if (LBoxHOCPlace.Selected == true)
            {
                SqlDataReader MyEBF = SQLdatabase.ExecuteReader("SELECT a.HOCPlace, b.HPlaceName FROM HCourse AS a LEFT JOIN HPlace AS b ON a.HOCPlace=b.HID WHERE a.HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' AND a.HCourseName='" + TB_HCourseName.Text + "' AND a.HOCPlace='" + LBoxHOCPlace.Value + "' AND a.HDateRange='" + TB_HDateRange.Text + "' AND a.HStatus='1' AND a.HSave='1' AND NOT EXISTS ( SELECT HCourseName FROM HCourse  tb WHERE  a.HCBatchNum ='" + LB_HCBatchNum.Text + "' )");

                if (MyEBF.Read())
                {
                    reHOCPlace += 1;
                    stHOCPlace += MyEBF["HPlaceName"].ToString() + ",";
                }
                MyEBF.Close();


            }
        }

        if (reHOCPlace != 0)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('上課地點：" + stHOCPlace.TrimEnd(',') + "已重複開課，再請協助確認，謝謝!');", true);
            //Response.Write("<script>alert('上課地點：" + stHOCPlace.TrimEnd(',') + "已重複開課，再請協助確認，謝謝!');</script>");
            return;
        }
        #endregion

        #region--圖片上傳
        string picname = "";
        if (LB_OldPic.Text != "" && FU_HImg.HasFile == true)
        {
            //上傳圖片
            Upload_Pic();

            picname = LB_Pic.Text;
        }
        else if (LB_OldPic.Text != "" && FU_HImg.HasFile == false)
        {
            picname = LB_OldPic.Text;
        }
        else if (LB_OldPic.Text == "" && FU_HImg.HasFile == true)
        {
            //上傳圖片
            Upload_Pic();
            picname = LB_Pic.Text;
        }
        #endregion

        //基本費用(元)存入DB要先轉換成點數概念(1元=10點)
        int BasicPrice = 0;
        if (string.IsNullOrEmpty(TB_HBCPoint.Text.Trim()))
        {
            Response.Write("<script>alert('請先確認已有選擇課程範本哦~!');</script>");
            return;
        }
        else
        {
            BasicPrice = Convert.ToInt32(TB_HBCPoint.Text.Trim()) / 10;
        }

        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Trim(',').Split(',');

            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    for (int i = 0; i < strHID.Length; i++)
                    {
                        string strUpdHC = "update HCourse set HEnableSystem='', HApplySystem='', HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "', HCourseName='" + TB_HCourseName.Text + "', HTeacherName='" + gLBox_HTeacherName + "', HOCPlace='" + LBoxHOCPlace.Value + "', HDateRange='" + TB_HDateRange.Text + "', HSTime='" + TB_HSTime.Text + "', HETime='" + TB_HETime.Text + "', HType='" + DDL_HType.SelectedValue + "', HOSystem='" + DDL_HOSystem.SelectedValue + "', HRSystem='" + LB_HRSystem.Text + "', HNLCourse='" + RBL_HNLCourse.SelectedValue + "', HNGuide='" + RBL_HNGuide.SelectedValue + "', HNFull='" + RBL_HNFull.SelectedValue + "', HNRequirement='" + LB_HNRequirement.Text + "', HTeam='" + gLBox_HTeam + "',  HQuestionID='" + gLBox_HQuestionID + "', HPMethod='" + DDL_HPMethod.SelectedValue + "', HBCPoint='" + BasicPrice + "', HSGList='" + RBL_HSGList.SelectedValue + "', HIRestriction='" + LB_HIRestriction.Text + "', HRemark=N'" + HttpUtility.HtmlEncode(TB_HRemark.Text.Trim()) + "', HContentTitle=N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', HContent=N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', HRNContent=N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', HStatus='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1',HImg='" + picname + "', BCSchedule='" + LB_BCSchedule.Text + "' , BECSchedule='" + LB_BECSchedule.Text + "', ICRecord='" + LB_ICRecord.Text + "', DPosition='" + LB_DPosition.Text + "', CPosition='" + LB_CPosition.Text + "', TPosition='" + LB_TPosition.Text + "',HBudget='" + RBL_HBudget.SelectedValue + "', HBudgetTable='" + LB_HBudgetTable.Text + "', HLodging='" + RBL_HLodging.SelectedValue + "', HTMaterialID='" + gLBox_HTMaterialID + "', HCDeadline ='" + TB_HCDeadline.Text.Trim() + "', HCDeadlineDay ='" + TB_HCDeadlineDay.Text.Trim() + "', HDContinous='" + RBL_Continuous.SelectedValue + "', HBudgetType='" + TB_HBudgetType.Text + "', HCourseType='" + DDL_HCourseType.SelectedValue + "', HAxisYN='" + RBL_HAxisYN.SelectedValue + "', HAxisClass='" + DDL_HAxisClass.SelectedValue + "', HExamSubject='" + DDL_HExamSubject.SelectedValue + "', HGradeCalculation='" + DDL_HGradeCalculation.SelectedValue + "', HExamPaperID='" + gLBox_HExamPaperID + "',  HSupervise='" + gLBox_HSupervise + "', HAttRateStandard='" + TB_HAttRateStandard.Text + "', HExamPassStandard='" + TB_HExamPassStandard.Text + "', HExamContentID='" + DDL_HExamContentID.SelectedValue + "', HParticipantLimit='" + TB_HParticipantLimit.Text + "', HTeacherClass='" + DDL_HTeacherClass.SelectedValue + "', HTearcherLV='" + DDL_HTearcherLV.SelectedValue + "' where  HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'";
                        dbCmd = new SqlCommand(strUpdHC, dbConn);
                        dbCmd.ExecuteNonQuery();

                    }

                    SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HCourseNum FROM HCourse WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'");
                    if (!dr.Read())
                    {
                        string strInsHC = "IF EXISTS(SELECT HCourseNum FROM HCourse WHERE SUBSTRING(HCourseNum,2,8) = CONVERT(nvarchar, getdate(), 112)) " +
    "BEGIN " +
       "insert into HCourse (HEnableSystem, HApplySystem, HCTemplateID, HCBatchNum, HCourseNum, HCourseName, HTeacherName, HOCPlace, HDateRange, HSTime, HETime, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam,  HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HRNContent, HVerifyNum, HApplicant, HApplyTime, HVerifyTime, HVerifyStatus, HStatus, HCreate, HCreateDT,HModify, HModifyDT,  HSave,HImg,BCSchedule, BECSchedule,  ICRecord, DPosition, CPosition, TPosition, HBudget, HBudgetTable, HSCourseID, HLodging, HTMaterialID, HCDeadlineDay,  HSerial, HDContinous, HBudgetType,HCourseType, HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID,  HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV) values ('', '', '" + DDL_HCourseTemplate.SelectedValue + "', '" + LB_HCBatchNum.Text + "', 'C' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HCourseNum), 12)) FROM HCourse) + 1), '" + TB_HCourseName.Text + "', '" + gLBox_HTeacherName + "', '" + LBoxHOCPlace.Value + "', '" + TB_HDateRange.Text + "', '" + TB_HSTime.Text + "', '" + TB_HETime.Text + "', '" + DDL_HType.SelectedValue + "','" + DDL_HOSystem.SelectedValue + "', '" + LB_HRSystem.Text + "', '" + RBL_HNLCourse.SelectedValue + "', '" + RBL_HNGuide.SelectedValue + "', '" + RBL_HNFull.SelectedValue + "','" + LB_HNRequirement.Text + "', '" + gLBox_HTeam + "',  '" + gLBox_HQuestionID + "', '" + DDL_HPMethod.SelectedValue + "', '" + BasicPrice + "', '" + RBL_HSGList.SelectedValue + "', '" + LB_HIRestriction.Text + "', N'" + HttpUtility.HtmlEncode(TB_HRemark.Text) + "', N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', '', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '', '99', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',  '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','1','" + picname + "','" + LB_BCSchedule.Text + "','" + LB_BECSchedule.Text + "','" + LB_ICRecord.Text + "','" + LB_DPosition.Text + "','" + LB_CPosition.Text + "','" + LB_TPosition.Text + "','" + RBL_HBudget.SelectedValue + "','" + LB_HBudgetTable.Text + "','" + DDL_HSCourse.SelectedValue + "','" + RBL_HLodging.SelectedValue + "', '" + gLBox_HTMaterialID + "', '" + TB_HCDeadlineDay.Text.Trim() + "','" + RBL_HSerial.SelectedValue + "','" + RBL_Continuous.SelectedValue + "','" + TB_HBudgetType.Text + "','" + DDL_HCourseType.SelectedValue + "', '" + RBL_HAxisYN.SelectedValue + "', '" + DDL_HAxisClass.SelectedValue + "', '" + DDL_HExamSubject.SelectedValue + "', '" + DDL_HGradeCalculation.SelectedValue + "', '" + gLBox_HExamPaperID + "',  '" + gLBox_HSupervise + "', '" + TB_HAttRateStandard.Text + "', '" + TB_HExamPassStandard.Text + "', '" + DDL_HExamContentID.SelectedValue + "', '" + TB_HParticipantLimit.Text + "', '" + DDL_HTeacherClass.SelectedValue + "', '" + DDL_HTearcherLV.SelectedValue + "' );SELECT SCOPE_IDENTITY() AS HID " +
    "END " +
    "ELSE " +
    "BEGIN " +
       "insert into HCourse (HEnableSystem, HApplySystem, HCTemplateID, HCBatchNum, HCourseNum, HCourseName, HTeacherName, HOCPlace, HDateRange, HSTime, HETime, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam,  HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HRNContent, HVerifyNum, HApplicant, HApplyTime, HVerifyTime, HVerifyStatus, HStatus, HCreate, HCreateDT,HModify, HModifyDT,  HSave,HImg, BCSchedule, BECSchedule, ICRecord, DPosition, CPosition, TPosition, HBudget, HBudgetTable, HSCourseID, HLodging, HTMaterialID, HCDeadlineDay, HSerial,HDContinous,HBudgetType, HCourseType, HAxisYN, HAxisClass, HExamSubject, HGradeCalculation, HExamPaperID,  HSupervise, HAttRateStandard, HExamPassStandard, HExamContentID, HParticipantLimit, HTeacherClass, HTearcherLV) values ('', '', '" + DDL_HCourseTemplate.SelectedValue + "', '" + LB_HCBatchNum.Text + "', 'C' + CONVERT(nvarchar, getdate(), 112) + '0001', '" + TB_HCourseName.Text + "', '" + gLBox_HTeacherName + "', '" + LBoxHOCPlace.Value + "', '" + TB_HDateRange.Text + "', '" + TB_HSTime.Text + "', '" + TB_HETime.Text + "', '" + DDL_HType.SelectedValue + "','" + DDL_HOSystem.SelectedValue + "', '" + LB_HRSystem.Text + "', '" + RBL_HNLCourse.SelectedValue + "', '" + RBL_HNGuide.SelectedValue + "', '" + RBL_HNFull.SelectedValue + "','" + LB_HNRequirement.Text + "', '" + gLBox_HTeam + "','" + gLBox_HQuestionID + "', '" + DDL_HPMethod.SelectedValue + "', '" + BasicPrice + "', '" + RBL_HSGList.SelectedValue + "', '" + LB_HIRestriction.Text + "', N'" + HttpUtility.HtmlEncode(TB_HRemark.Text) + "', N'" + HttpUtility.HtmlEncode(TB_HContentTitle.Text) + "', N'" + HttpUtility.HtmlEncode(CKE_HContent.Text.TrimEnd()) + "', N'" + HttpUtility.HtmlEncode(CKE_HRNContent.Text) + "', '', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '', '99', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '1','" + picname + "','" + LB_BCSchedule.Text + "','" + LB_BECSchedule.Text + "','" + LB_ICRecord.Text + "','" + LB_DPosition.Text + "','" + LB_CPosition.Text + "','" + LB_TPosition.Text + "','" + RBL_HBudget.SelectedValue + "','" + LB_HBudgetTable.Text + "','" + DDL_HSCourse.SelectedValue + "','" + RBL_HLodging.SelectedValue + "', '" + gLBox_HTMaterialID + "', '" + TB_HCDeadlineDay.Text.Trim() + "','" + RBL_HSerial.SelectedValue + "','" + RBL_Continuous.SelectedValue + "','" + TB_HBudgetType.Text + "','" + DDL_HCourseType.SelectedValue + "', '" + RBL_HAxisYN.SelectedValue + "', '" + DDL_HAxisClass.SelectedValue + "', '" + DDL_HExamSubject.SelectedValue + "', '" + DDL_HGradeCalculation.SelectedValue + "', '" + gLBox_HExamPaperID + "',  '" + gLBox_HSupervise + "', '" + TB_HAttRateStandard.Text + "', '" + TB_HExamPassStandard.Text + "', '" + DDL_HExamContentID.SelectedValue + "', '" + TB_HParticipantLimit.Text + "', '" + DDL_HTeacherClass.SelectedValue + "', '" + DDL_HTearcherLV.SelectedValue + "' );SELECT SCOPE_IDENTITY() AS HID " +
    "END";
                        dbCmd = new SqlCommand(strInsHC, dbConn);
                        HID += dbCmd.ExecuteScalar().ToString() + ",";

                    }
                    dr.Close();
                }
                else
                {
                    SqlDataReader drDel = SQLdatabase.ExecuteReader("SELECT HCourseNum FROM HCourse WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'");

                    if (drDel.Read())
                    {
                        for (int i = 0; i < strHID.Length; i++)
                        {
                            string strUpdHC_T = "update HCourse set HStatus='0' where HID='" + strHID[i] + "' AND HOCPlace='" + LBoxHOCPlace.Value + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "'";
                            dbCmd = new SqlCommand(strUpdHC_T, dbConn);
                            dbCmd.ExecuteNonQuery();


                            string strUpdHCM_T = "update HCourseMaterial set HStatus='0' where HCourseID='" + strHID[i] + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "'";
                            dbCmd = new SqlCommand(strUpdHCM_T, dbConn);
                            dbCmd.ExecuteNonQuery();

                            string strUpdHTL_T = "update HTodoList set HStatus='0' where HCourseID='" + strHID[i] + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "'";
                            dbCmd = new SqlCommand(strUpdHTL_T, dbConn);
                            dbCmd.ExecuteNonQuery();

                            string strUpdHCE_T = "update HCourseEvaluation set HStatus='0' where HCourseID='" + strHID[i] + "' AND HCBatchNum='" + LB_HCBatchNum.Text + "'";
                            dbCmd = new SqlCommand(strUpdHCE_T, dbConn);
                            dbCmd.ExecuteNonQuery();
                        }

                    }
                    drDel.Close();
                }
            }


        }



        if (HID != "")
        {
            LB_HID.Text = HID;
        }

        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Split(',');
            for (int i = 0; i < strHID.Length - 1; i++)
            {
                string strHCE = "INSERT INTO HCourseEvaluation (HCourseID, HCBatchNum, HCEType, HCEContent, HCENum, HStatus, HCreate, HCreateDT, HModify,HModifyDT) VALUES (@HCourseID, @HCBatchNum, @HCEType, @HCEContent, @HCENum, @HStatus, @HCreate, @HCreateDT, @HModify,@HModifyDT ) ";

                dbCmd = new SqlCommand(strHCE, dbConn);
                dbCmd.Parameters.AddWithValue("@HCourseID", strHID[i]);
                dbCmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text.Trim());
                dbCmd.Parameters.AddWithValue("@HCEType", DDL_HCEType.SelectedValue);
                dbCmd.Parameters.AddWithValue("@HCEContent", TB_HCEContent.Text.Trim());
                dbCmd.Parameters.AddWithValue("@HCENum", TB_HCENum.Text.Trim());
                dbCmd.Parameters.AddWithValue("@HStatus", "1");
                dbCmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                dbCmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                dbCmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                dbCmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                dbCmd.ExecuteNonQuery();

            }

        }


        dbConn.Close();
        dbCmd.Cancel();

        //清空資料
        TB_HCEContent.Text = null;
        TB_HCENum.Text = null;


        SDS_HCourseEvaluation.SelectCommand = "SELECT HCBatchNum, HCEType, HCEContent, HCENum FROM HCourseEvaluation WHERE HStatus=1 AND HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY  HCBatchNum, HCEType, HCEContent, HCENum";
        Rpt_HCourseEvaluation.DataBind();

    }
    #endregion

    #region 檢覈評比項目-刪除功能
    protected void LBtn_HCE_Del_Click(object sender, EventArgs e)
    {
        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        string strDelHCE = "DELETE FROM HCourseEvaluation WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HCEType='" + ((Label)RI.FindControl("LB_HCEType")).Text + "' AND HCEContent='" + ((Label)RI.FindControl("LB_HCEContent")).Text + "' AND HCENum='" + ((Label)RI.FindControl("LB_HCENum")).Text + "' ";
        SqlCommand dbCmd = new SqlCommand(strDelHCE, con);
        con.Open();
        dbCmd.ExecuteNonQuery();
        con.Close();
        dbCmd.Cancel();

        SDS_HCourseEvaluation.SelectCommand = "SELECT HCBatchNum, HCEType, HCEContent, HCENum FROM HCourseEvaluation WHERE HStatus=1 AND HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY  HCBatchNum, HCEType, HCEContent, HCENum";
        Rpt_HCourseEvaluation.DataBind();


    }
    #endregion

    #region 檢覈評比項目-資料繫結
    protected void Rpt_HCourseEvaluation_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if ((((Label)e.Item.FindControl("LB_HCEType")).Text) == "1")
        {
            (((Label)e.Item.FindControl("LB_HCETypeName"))).Text = "影片";
        }
        else
        {
            (((Label)e.Item.FindControl("LB_HCETypeName"))).Text = "檔案(圖片/PDF/Excel)";
        }
    }
    #endregion

    #endregion

    #region 必填判斷Function
    private bool ValidateRequiredFields(out string message)
    {
        string alert = "";

        if (string.IsNullOrEmpty(TB_HCourseName.Text.Trim()))
        {
            alert += "請輸入課程名稱！\\n";
        }



        //if (RBL_TestCourse.SelectedValue == "1")
        //{

        //}
        //else
        //{
        if (string.IsNullOrEmpty(GetSelectedValues(LBox_HOCPlace)))
        {
            alert += "請選擇上課地點！\\n";
        }
        //}

        if (string.IsNullOrEmpty(TB_HDateRange.Text.Trim()))
        {
            alert += "請輸入課程日期！\\n";
        }

        if (string.IsNullOrEmpty(TB_HSTime.Text.Trim()))
        {
            alert += "請輸入上課開始時間！\\n";
        }

        if (string.IsNullOrEmpty(TB_HETime.Text.Trim()))
        {
            alert += "請輸入上課結束時間！\\n";
        }

        message = alert;
        return string.IsNullOrEmpty(alert);
    }
    #endregion

    #region 刪除1天前未存檔的資料Function
    private void DeleteUnsavedCourseData(SqlConnection dbConn)
    {
        string[] tables = new string[]
     {
        "HCourse",
        "HCourseMaterial",
        "HTodoList",
        "HCourseTMaterial"
     };

        string timestamp = DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss");

        foreach (string table in tables)
        {
            string sql = "DELETE FROM " + table + " WHERE HSave = '0' AND HCreateDT < @CutoffDate";
            SqlCommand cmd = new SqlCommand(sql, dbConn);
            cmd.Parameters.AddWithValue("@CutoffDate", timestamp);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
    }
    #endregion

    #region 選單Function
    private void BindListControls(string sql, string textField, string valueField, string defaultText, bool includeStopTag, params ListControl[] controls)
    {
        SqlDataReader reader = SQLdatabase.ExecuteReader(sql);

        if (!string.IsNullOrEmpty(defaultText))
        {
            foreach (ListControl ctl in controls)
            {
                ctl.Items.Add(new ListItem(defaultText, "0"));
            }
        }

        while (reader.Read())
        {
            string text = reader[textField].ToString();
            string value = reader[valueField].ToString();

            string textWithTag = includeStopTag && reader["HStatus"].ToString() == "0"
                ? "(停用)" + text
                : text;

            foreach (ListControl ctl in controls)
            {
                ctl.Items.Add(new ListItem(textWithTag, value));
            }
        }

        reader.Close();
    }
    #endregion



    #region ListBox選單Function

    //字串還原成選取項目
    private void SetSelectedItems(ListBox listBox, string data)
    {
        var selectedValues = new HashSet<string>(data.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        foreach (ListItem item in listBox.Items)
        {
            item.Selected = selectedValues.Contains(item.Value);
        }
    }

    //取得選取項目轉為字串
    private string GetSelectedValues(ListBox listBox)
    {
        if (listBox.Items.Count == 0) return "";
        return string.Join(",", listBox.Items
            .Cast<ListItem>()
            .Where(i => i.Selected)
            .Select(i => i.Value));
    }

    #region 多選 Function<註解>
    public string ListBox(ListBox ListB)
    {
        string ClassVal = "";
        string ClassSelectecd = string.Empty;
        foreach (ListItem li in ListB.Items)
        {
            if (li.Selected == true)
            {
                ClassVal += li.Value + ",";
            }
        }
        string ClassID = ClassVal.ToString();
        ClassID = ClassID.Trim(new Char[] { ',' });//去除結尾逗號

        return ClassID;
    }
    #endregion

    #endregion


    public int InsertMergedCourse(string sourceHIDs, string batchNum, string creator)
    {
        int newHID = 0;
        string connStr = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            using (SqlCommand cmd = new SqlCommand("sp_InsertHCourseFromMultipleMerge", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // 傳入參數
                cmd.Parameters.AddWithValue("@SourceHIDs", sourceHIDs);              // 例："1011,1012"
                cmd.Parameters.AddWithValue("@HCBatchNum", batchNum);                // 批號，例如 CB202504220001
                cmd.Parameters.AddWithValue("@HCreate", creator);                    // 建立者 ID
                cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now);             // 建立時間

                // OUTPUT 參數
                SqlParameter outputId = new SqlParameter("@NewHID", SqlDbType.Int);
                outputId.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputId);

                conn.Open();
                cmd.ExecuteNonQuery();

                // 取得新課程 HID
                newHID = (int)outputId.Value;
            }
        }

        return newHID;
    }







}


