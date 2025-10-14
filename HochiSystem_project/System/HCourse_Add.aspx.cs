using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class HCourse_Add : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    #region --根目錄--
    string CourseImgRoot = "D:\\Website\\System\\HochiSystem\\uploads\\Course\\";
    string CourseImg = "~/uploads/Course/";
    string CourseFileRoot = "D:\\Website\\System\\HochiSystem\\uploads\\CourseFile\\";
    string CourseFile = "~/uploads/CourseFile/";
    #endregion

    #region 寄件資訊<正式>
    //public string Sender = "mail.edu@hochi.org.tw";  //寄信人地址
    //public string EAcount = "mail.edu@hochi.org.tw";  //寄信人帳號
    //public string EPasword = "Hochi@2024";
    //string EHost = "smtp.gmail.com";  //寄信伺服器
    //int EPort = 587;
    //bool EEnabledSSL = true;

    public string Sender = MailConfig.Sender;
    public string EAcount = MailConfig.Account;
    public string EPasword = MailConfig.Password;
    public string EHost = MailConfig.Host;
    public int EPort = MailConfig.Port;
    public bool EEnabledSSL = MailConfig.EnableSSL;
    #endregion

    #region 測試
    //public string Sender = "testmail@tecgenic.com";  //寄信人地址
    //public string EAcount = "testmail@tecgenic.com";  //寄信人帳號
    //public string EPasword = "1qaz@test79";
    //string EHost = "mail.tecgenic.com";    //寄信伺服器
    //int EPort = 25;
    //bool EEnabledSSL = false;
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

    //刪除資料夾內所有檔案
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

        if (!IsPostBack)
        {
            if (string.IsNullOrEmpty(LB_NavTab.Text))
            {
                Status(1);
                LBtn_Template.CssClass = "nav-link active show";

                #region 課程範本基本資訊
                SDS_HCourseTemplate.SelectCommand = "SELECT HID, HTemplateNum, HTemplateName, (HTemplateNum+'-'+HTemplateName) AS HTNumName  FROM HCourse_T WHERE HStatus=1 order by HTemplateNum";
                //DDL_HCourseTemplate.DataBind();

                SDS_HType.SelectCommand = "SELECT HID, HCourseName FROM HCourse_Class";
                //DDL_HType.DataBind();

                SDS_HSystem.SelectCommand = "SELECT HID, HSystemName, HStatus FROM HSystem WHERE HStatus=1";
                //DDL_HOSystem.DataBind();

                SDS_HDharma.SelectCommand = "SELECT HID, HDTypeID, HDharmaName FROM HDharma WHERE HStatus='1'";
                //LBox_HNRequirement.DataBind();

                SDS_HMType.SelectCommand = "SELECT HID, HMType FROM HMType ORDER BY HID ASC";
                //LBox_HIRestriction.DataBind();

                SDS_HBudgetType.SelectCommand = "SELECT HID, HContent, HStatus FROM HBudgetType WHERE HStatus=1";
                //DDL_HBudgetType.DataBind();

                SDS_HQuestionID.SelectCommand = "SELECT HID, HTitle FROM HQuestion  WHERE HStatus=1 ORDER BY HTitle";
                //LBox_HQuestionID.DataBind();

                //SDS_HTMaterialID.SelectCommand = "SELECT a.HID, (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) AS HTMName, a.HTMaterial, a.HStatus FROM   HTeacherMaterial AS a LEFT JOIN HDocCTypeClass AS b ON a.HDocCTClassID=b.HID LEFT JOIN HCourseLevel AS c ON a.HCourseLevelID=c.HID LEFT JOIN HFileType AS d ON a.HFileTypeID=d.HID WHERE a.HStatus='1' AND (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) <>'' AND (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) IS NOT NULL ORDER BY a.HStatus DESC";
                //LBox_HTMaterialID.DataBind();

                SDS_HExamContentName.SelectCommand = "SELECT HID, (HExamContentName+'-'+(CASE HExamType WHEN 1 THEN '筆試' WHEN 2 THEN '實作' WHEN '3' THEN '試教' ELSE '' END)) AS HExamContentName  FROM HExamContent WHERE HStatus=1";
                ////DDL_HExamContentID.DataBind();

                //SDS_HExamSubject.SelectCommand = "SELECT HID, HExamSubject FROM HExamSubject WHERE HStatus=1";
                ////DDL_HExamSubject.DataBind();


               


                #endregion

            }

            TB_HDateRange.Attributes.Add("readonly", "true");

            #region 課程開課資訊
            SDS_HTeam.SelectCommand = "SELECT HMember.HID, HUserName,HAreaID, HPeriod, (HArea+'/'+HPeriod+' '+HUserName) as UserName, HMember.HStatus from HMember Left Join HArea On HMember.HAreaID =HArea.HID WHERE HMember.HStatus=1 AND HType in(7,8,9,10,11,12,13)  order by HUserName";
            //LBox_HTeam.DataBind();

            //SDS_HSupervise.SelectCommand = "SELECT HMember.HID, HUserName,HAreaID, HPeriod, (HArea+'/'+HPeriod+' '+HUserName) as UserName, HMember.HStatus from HMember Left Join HArea On HMember.HAreaID =HArea.HID WHERE HMember.HStatus=1 AND HType in(7,8,9,10,11,12,13)  order by HUserName";
            ////LBox_HSupervise.DataBind();

            SDS_HOCPlace.SelectCommand = "SELECT HID, HPlaceName FROM HPlaceList  WHERE HStatus=1 ORDER BY HStatus DESC, strHAreaID ASC";
            //LBox_HOCPlace.DataBind();

            SDS_HExamPaperID.SelectCommand = "SELECT HID, HTitle FROM HExamPaper  WHERE HStatus=1 ORDER BY HTitle";
            //LBox_HExamPaperID.DataBind();

            #endregion


            #region 下拉選單項目
            DDL_HTeacherClass.DataSource = DDL_ListItem.HTeacher.HTeacherClass;
            DDL_HTeacherClass.DataTextField = "Value";
            DDL_HTeacherClass.DataValueField = "Key";
            DDL_HTeacherClass.DataBind();

            DDL_HTearcherLV.DataSource = DDL_ListItem.HTeacher.HTearcherLV;
            DDL_HTearcherLV.DataTextField = "Value";
            DDL_HTearcherLV.DataValueField = "Key";
            DDL_HTearcherLV.DataBind();
            #endregion

            #region 刪除1天前未存檔的資料


            //string gHCMFileTemp = Server.MapPath("~/uploads/CourseMaterialTemp/");
            //string strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;

            //// 延遲背景清除
            //Task.Run(() =>
            //{
            //    try
            //    {
            //        using (SqlConnection dbConn = new SqlConnection(strDBConn))
            //        {
            //            dbConn.Open();
            //            DeleteUnsavedCourseData(dbConn); // 刪除1天前未儲存課程資料
            //        }

            //        DeleteSrcFolder(gHCMFileTemp);// 刪除教材暫存資料夾
            //    }
            //    catch (Exception ex)
            //    {
            //        // log ex.ToString() 或寫入例外追蹤系統
            //    }
            //});

            #endregion
        }


        #region 體系護持工作項目
        SDS_HGroupName.SelectCommand = "SELECT HID, HGroupName FROM HGroup WHERE HStatus='1'";
        //DDL_HGroupName.DataBind();

        SDS_HGroupLeader.SelectCommand = "select HMember.HID, HUserName,HAreaID, HPeriod, (HArea+'/'+HPeriod+' '+HUserName) as UserName from HMember Left Join HArea On HMember.HAreaID =HArea.HID WHERE HMember.HStatus='1' order by HUserName";
        //DDL_HGroupLeader.DataBind();

        //SDS_HExamStaff.SelectCommand = "SELECT HMember.HID, HUserName,HAreaID, HPeriod, (HArea+'/'+HPeriod+' '+HUserName) AS UserName FROM HMember LEFT JOIN HArea ON HMember.HAreaID =HArea.HID ORDER BY HUserName";
        //LBox_HExamStaff.DataBind();

        #endregion

        #region 講師教材
        SDS_HTMaterial.SelectCommand = "SELECT a.HID, (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) AS HTMName, a.HTMaterial, a.HStatus FROM   HTeacherMaterial AS a LEFT JOIN HDocCTypeClass AS b ON a.HDocCTClassID=b.HID LEFT JOIN HCourseLevel AS c ON a.HCourseLevelID=c.HID LEFT JOIN HFileType AS d ON a.HFileTypeID=d.HID WHERE a.HStatus='1' AND (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) <>'' AND (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) IS NOT NULL ORDER BY a.HStatus DESC";
        //Rpt_HTMaterialDetail.DataBind();
        #endregion


    }

    #region 課程範本資訊

    #region 課程範本名稱
    protected void DDL_CourseTemplate_SelectedIndexChanged(object sender, EventArgs e)
    {
        RegScript();//註冊js

        LBox_HRSystem.ClearSelection();
        LBox_HNRequirement.ClearSelection();
        LBox_HIRestriction.ClearSelection();
        LBox_HQuestionID.ClearSelection();
        //LBox_HTMaterialID.ClearSelection();

        SqlDataReader MyQueryHC_T = SQLdatabase.ExecuteReader("SELECT HID, HTemplateNum, HTemplateName, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam,HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HBudgetType, HSave, HVRoleID, HARoleID, HSerial, HBudget,  HLodging, HTMaterialID, HCDeadline, HCDeadlineDay, HAxisYN, HAxisClass, HExamSubject, HExamContentID, HParticipantLimit, HBookByDateYN, HCCPeriodYN FROM HCourse_T WHERE HID='" + DDL_HCourseTemplate.SelectedValue + "'");
        if (MyQueryHC_T.Read())
        {
            DDL_HType.SelectedValue = MyQueryHC_T["HType"].ToString();
            RBL_HSerial.SelectedValue = string.IsNullOrEmpty(MyQueryHC_T["HSerial"].ToString()) ? "0" : MyQueryHC_T["HSerial"].ToString();
            RBL_HNLCourse.SelectedValue = string.IsNullOrEmpty(MyQueryHC_T["HNLCourse"].ToString()) ? "0" : MyQueryHC_T["HNLCourse"].ToString();
            RBL_HNGuide.SelectedValue = string.IsNullOrEmpty(MyQueryHC_T["HNGuide"].ToString()) ? "0" : MyQueryHC_T["HNGuide"].ToString();
            RBL_HNFull.SelectedValue = string.IsNullOrEmpty(MyQueryHC_T["HNFull"].ToString()) ? "0" : MyQueryHC_T["HNFull"].ToString();
            RBL_HBudget.SelectedValue = string.IsNullOrEmpty(MyQueryHC_T["HBudget"].ToString()) ? "0" : MyQueryHC_T["HBudget"].ToString();
            RBL_HAxisYN.SelectedValue = string.IsNullOrEmpty(MyQueryHC_T["HAxisYN"].ToString()) ? "0" : MyQueryHC_T["HAxisYN"].ToString();
            //EA20240516_新增是否為軸線課程
            RBL_HLodging.SelectedValue = string.IsNullOrEmpty(MyQueryHC_T["HLodging"].ToString()) ? "0" : MyQueryHC_T["HLodging"].ToString();

            //AA20250611_新增HBookByDateYN (是否開放單天報名)
            RBL_HBookByDateYN.SelectedValue = string.IsNullOrEmpty(MyQueryHC_T["HBookByDateYN"].ToString()) ? "0" : MyQueryHC_T["HBookByDateYN"].ToString();

            //AA20250715_新增HCCPeriodYN
            RBL_HCCPeriodYN.SelectedValue = string.IsNullOrEmpty(MyQueryHC_T["HCCPeriodYN"].ToString()) ? "0" : MyQueryHC_T["HCCPeriodYN"].ToString();

            if (RBL_HCCPeriodYN.SelectedValue == "0")
            {
                DDL_HCCPeriodDItem.Enabled = false;
            }
            else
            {
                DDL_HCCPeriodDItem.Enabled = true;
            }

            HBudgetTable.Visible = !string.IsNullOrEmpty(MyQueryHC_T["HBudget"].ToString()) && MyQueryHC_T["HBudget"].ToString() == "1" ? true : false;

            DDL_HBudgetType.SelectedValue = MyQueryHC_T["HBudgetType"].ToString();

            TB_HCDeadlineDay.Text = !string.IsNullOrEmpty(MyQueryHC_T["HCDeadlineDay"].ToString()) ? MyQueryHC_T["HCDeadlineDay"].ToString() : "0";

            DDL_HOSystem.SelectedValue = MyQueryHC_T["HOSystem"].ToString();

            // HRSystem
            SetSelectedItems(LBox_HRSystem, MyQueryHC_T["HRSystem"].ToString());
            LB_HRSystem.Text = MyQueryHC_T["HRSystem"].ToString();

            // HNRequirement
            SetSelectedItems(LBox_HNRequirement, MyQueryHC_T["HNRequirement"].ToString());
            LB_HNRequirement.Text = MyQueryHC_T["HNRequirement"].ToString();

            // HIRestriction
            SetSelectedItems(LBox_HIRestriction, MyQueryHC_T["HIRestriction"].ToString());
            LB_HIRestriction.Text = MyQueryHC_T["HIRestriction"].ToString();

            // HQuestionID
            SetSelectedItems(LBox_HQuestionID, MyQueryHC_T["HQuestionID"].ToString());

            // HTMaterialID
            //SetSelectedItems(LBox_HTMaterialID, MyQueryHC_T["HTMaterialID"].ToString());


            DDL_HPMethod.SelectedValue = MyQueryHC_T["HPMethod"].ToString();
            //220819-顯示轉為金額
            if (!string.IsNullOrEmpty(MyQueryHC_T["HBCPoint"].ToString()))
            {
                TB_HBCPoint.Text = (Convert.ToInt32(MyQueryHC_T["HBCPoint"].ToString()) * 10).ToString();
            }


            //EA20240516_新增軸線類別
            if (MyQueryHC_T["HAxisYN"].ToString() == "1")
            {
                if (!string.IsNullOrEmpty(MyQueryHC_T["HAxisClass"].ToString()))
                {

                    DDL_HAxisClass.SelectedValue = DDL_HAxisClass.Items.FindByValue(MyQueryHC_T["HExamSubject"].ToString()) != null ? MyQueryHC_T["HAxisClass"].ToString() : "0";

                }
            }
            else
            {
                DDL_HAxisClass.SelectedValue = "0";
            }


            RBL_HSGList.SelectedValue = MyQueryHC_T["HSGList"].ToString();

            //EA20240517_加入檢覈科目名稱
            if (RBL_TestCourse.SelectedValue == "1")
            {
                if (DDL_HExamSubject.Items.FindByValue(MyQueryHC_T["HExamSubject"].ToString()) != null)
                {
                    DDL_HExamSubject.SelectedValue = MyQueryHC_T["HExamSubject"].ToString();
                }
            }
            else
            {
                DDL_HExamSubject.SelectedValue = "0";
            }

            //EA20250509_檢覈內容名稱、報名人數上限
            DDL_HExamContentID.SelectedValue = !string.IsNullOrEmpty(MyQueryHC_T["HExamContentID"].ToString()) ? MyQueryHC_T["HExamContentID"].ToString() : "0";
            TB_HParticipantLimit.Text = MyQueryHC_T["HParticipantLimit"].ToString();


            TB_HRemark.Text = HttpUtility.HtmlDecode(MyQueryHC_T["HRemark"].ToString());
            TB_HContentTitle.Text = HttpUtility.HtmlDecode(MyQueryHC_T["HContentTitle"].ToString());
            CKE_HContent.Text = HttpUtility.HtmlDecode(MyQueryHC_T["HContent"].ToString());

        }
        MyQueryHC_T.Close();

        #region 欄位判斷/顯示
        SqlDataReader QueryHCourse_Class = SQLdatabase.ExecuteReader("SELECT HID, HType FROM HCourse_Class WHERE HID='" + DDL_HType.SelectedValue + "'");
        if (QueryHCourse_Class.Read())
        {
            RBL_TestCourse.SelectedValue = QueryHCourse_Class["HType"].ToString() == "2" ? "1" : "0";
        }
        QueryHCourse_Class.Close();


        //EA20231030_課程報名截止日
        if (RBL_HSerial.SelectedValue == "0")
        {
            LB_HCDeadlineDayTitle.Text = "課程開始日前";
        }
        else
        {
            LB_HCDeadlineDayTitle.Text = "課程結束日後";
        }


        SqlDataReader QueryHCDeadline = SQLdatabase.ExecuteReader("SELECT HID, HCDeadline FROM HCDeadline WHERE HID=1");
        if (QueryHCDeadline.Read())
        {
            TB_HCDeadline.Text = QueryHCDeadline["HCDeadline"].ToString();
        }
        QueryHCDeadline.Close();


        #endregion

        #region 各Tab SDS.DataBind()
        if (DDL_HCourseTemplate.SelectedValue != "0")
        {
            //EE20241120_加入排序欄位；依排序欄位顯示
            SDS_HCourseMaterial.SelectCommand = " SELECT * FROM  (( SELECT HCMName, HCMaterial, HCMLink, HSave, IIF(HSort IS NULL,'0',HSort) AS HSort FROM HCourseMaterial_T WHERE HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave, HSort ) UNION   (SELECT HCMName, HCMaterial, HCMLink, HSave, HSort FROM HCourseMaterial  WHERE HCBatchNum='" + LB_HCBatchNum.Text + "'  GROUP BY HCMName, HCMaterial, HCMLink, HSave, HSort)) AS A ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
            //SDS_HCourseMaterial.SelectCommand = "SELECT HCMName, HCMaterial, HCMLink, HSave, HSort FROM HCourseMaterial_T where HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' UNION ALL SELECT HCMName, HCMaterial, HCMLink, HSave, HSort FROM HCourseMaterial where HCBatchNum='" + LB_HCBatchNum.Text + "'  ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
            Rpt_HCourseMaterial.DataBind();

            //SDS_HCourseMaterial.SelectCommand = "SELECT  HID, HCourseID, HCMName, HCMaterial, HCMLink, HCBatchNum, HSort FROM HCourseMaterial WHERE HCBatchNum='" + LB_HCBatchNum.Text + "'  ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
            //Rpt_HCourseMaterial.DataBind();


            SDS_HTodoList.SelectCommand = "SELECT HGroupName, HTask, HTaskContent, HGroupLeaderID, HSave, HTaskNum, HExamStaff FROM HTodoList_T where HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' GROUP BY HGroupName, HTask, HTaskContent,HGroupLeaderID, HSave, HTaskNum, HExamStaff UNION  SELECT HGroupName, HTask, HTaskContent,HGroupLeaderID, HSave, HTaskNum, HExamStaff FROM HTodoList where HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY HGroupName, HTask, HTaskContent, HGroupLeaderID,HSave, HTaskNum, HExamStaff ORDER BY HGroupName ASC";
            Rpt_HTodoList.DataBind();


            //EA20250408_講師教材設定
            SDS_HTMaterialDetail.SelectCommand = "SELECT HID, HCTemplateID, HTMaterialID, HSort, HSave FROM HCourseTMaterial_T WHERE HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' AND HStatus='1' ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
            Rpt_HTMaterialDetail.DataBind();
        }

        #endregion


    }
    #endregion

    #endregion

    #region 開課資訊

    #region 課程日期連續與否
    protected void RBL_Continuous_SelectedIndexChanged(object sender, EventArgs e)
    {
        TB_HDateRange.Text = null;
        RegScript();//註冊js
    }
    #endregion

    #endregion

    #region 學員課程教材
    protected void Rpt_HCourseMaterial_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            var fileUpload = (FileUpload)e.Item.FindControl("FU_HCMaterial");
            var link = (HyperLink)e.Item.FindControl("HL_HCMaterial");
            var tbName = (TextBox)e.Item.FindControl("TB_HCMName");
            var tbLink = (TextBox)e.Item.FindControl("TB_HCMLink");
            var tbSort = (TextBox)e.Item.FindControl("TB_HSort");

            fileUpload.Visible = false;

            link.Visible = true;
            link.NavigateUrl = "";
            link.Target = "_blank";

            tbName.Text = HttpUtility.HtmlDecode(tbName.Text);
            tbLink.Text = HttpUtility.HtmlDecode(tbLink.Text);


        }

    }

    #region 新增功能
    protected void LBtn_HCourseMaterial_add_Click(object sender, EventArgs e)
    {
        //RegScript();//註冊js

        if (TB_HCMName.Text.Trim() == "")
        {
            Response.Write("<script>alert('請輸入教材名稱');</script>");
            return;
        }

        string gDay = DateTime.Now.Day.ToString();
        string gMonth = DateTime.Now.Month.ToString();
        string gYear = DateTime.Now.Year.ToString();
        string gDate = System.DateTime.Now.ToString("yyyy/MM/dd");

        string gHCMFileTemp = "~/uploads/CourseMaterialTemp/";
        string gHCMFileRoot = "~/uploads/CourseMaterial/";
        string gHCMFileName = "";//主檔名
        string gHCMFileExtension = "";//副檔名

        bool fileIsValid = false;

        // Directory.CreateDirectory(gHCMFileRoot);
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

        if (LB_HCBatchNum.Text.Trim() == "")
        {
            InsertCourserFun("HCourseMaterial_add");
        }



        if (LB_HID.Text.Trim(',') != "")
        {

            string[] strHID = LB_HID.Text.Split(',');
            for (int i = 0; i < strHID.Length - 1; i++)
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


            }


        }

        //EE20241120_加入排序欄位；依排序欄位顯示
        SDS_HCourseMaterial.SelectCommand = "SELECT * FROM  (( SELECT HCMName, HCMaterial, HCMLink, IIF(HSort IS NULL,'0',HSort) AS HSort FROM HCourseMaterial_T WHERE HCTemplateID='" + DDL_HCourseTemplate.SelectedValue + "' GROUP BY HCMName, HCMaterial, HCMLink, HSort ) UNION   (SELECT HCMName, HCMaterial, HCMLink, HSort FROM HCourseMaterial  WHERE HCBatchNum='" + LB_HCBatchNum.Text + "'  GROUP BY HCMName, HCMaterial, HCMLink, HSort))AS A ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC ";
        Rpt_HCourseMaterial.DataBind();

        //清除textbox
        TB_HCMName.Text = "";
        TB_HCMLink.Text = "";
        TB_HSort.Text = "";


    }
    #endregion

    #endregion

    #region 體系護持工作項目

    protected void Rpt_HTodoList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        var DDL_Group = (DropDownList)e.Item.FindControl("DDL_HGroupName");
        var DDL_Task = (DropDownList)e.Item.FindControl("DDL_HTask");
        var TB_Content = (TextBox)e.Item.FindControl("TB_HTaskContent");
        var DDL_Leader = (DropDownList)e.Item.FindControl("DDL_HGroupLeader");
        var LB_ExamStaff = (Label)e.Item.FindControl("LB_HExamStaff");
        var LBox_ExamStaff = (ListBox)e.Item.FindControl("LBox_HExamStaff");

        using (SqlDataReader drGroup = SQLdatabase.ExecuteReader("SELECT HID, HGroupName FROM HGroup WHERE HStatus='1'"))
        {
            while (drGroup.Read())
            {
                DDL_Group.Items.Add(new ListItem(drGroup["HGroupName"].ToString(), drGroup["HID"].ToString()));
            }
        }

        DDL_Group.SelectedValue = gDRV["HGroupName"].ToString();

        //根據所選群組查對應任務
        string groupID = DDL_Group.SelectedValue;
        using (SqlDataReader drTask = SQLdatabase.ExecuteReader("SELECT HID, HTask FROM HCourseTask WHERE HGroupID = '" + groupID + "'"))
        {
            while (drTask.Read())
            {
                DDL_Task.Items.Add(new ListItem(drTask["HTask"].ToString(), drTask["HID"].ToString()));
            }
        }

        DDL_Task.SelectedValue = gDRV["HTask"].ToString();

        //體系護持工作項目
        TB_Content.Text = HttpUtility.HtmlDecode(TB_Content.Text);

        //群組負責人
        DDL_Leader.SelectedValue = DDL_Leader.Items.FindByValue(gDRV["HGroupLeaderID"].ToString()) != null ? gDRV["HGroupLeaderID"].ToString() : "0";
        //DDL_Leader.SelectedValue = gDRV["HGroupLeaderID"].ToString();

        //試務人員名單(多選)
        string[] selectedValues = LB_ExamStaff.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        HashSet<string> selectedSet = new HashSet<string>(selectedValues);

        foreach (ListItem item in LBox_ExamStaff.Items)
        {
            item.Selected = selectedSet.Contains(item.Value);
        }

    }

    #region 組別選單
    protected void DDL_HGroupName_SelectedIndexChanged(object sender, EventArgs e)
    {
        DDL_HTask.Items.Clear();
        DDL_HTask.Enabled = true;
        DDL_HTask.Items.Add(new ListItem("-請選擇-", "0"));

        using (SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HTask, HTaskContent FROM HCourseTask where HGroupID='" + DDL_HGroupName.SelectedValue + "'"))
        {
            while (dr.Read())
            {
                DDL_HTask.Items.Add(new ListItem(dr["HTask"].ToString(), dr["HID"].ToString()));
            }
        }

        RegScript();//執行js
    }
    #endregion

    #region 任務職稱
    protected void DDL_HTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        using (SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HTask, HTaskContent FROM HCourseTask where HID='" + DDL_HTask.SelectedValue + "' AND HStatus='1'"))
        {
            if (dr.Read())
            {
                TB_HTaskContent.Text = dr["HTaskContent"].ToString();
            }
        }

        RegScript();//執行js
    }
    #endregion

    #region 體系護持工作項目_新增功能
    protected void LBtn_HTodoList_add_Click(object sender, EventArgs e)
    {
        //RegScript();//註冊js

        if (LB_HCBatchNum.Text == "")
        {
            InsertCourserFun("HTodoList_add");
        }
        else
        {
            if (LB_HID.Text.Trim(',') != "")
            {
                UpdateCourseFun("HTodoList_add");
            }
        }


        //EA20240623_試務人員選單(多選)
        string gHExamStaff = GetSelectedValues(LBox_HExamStaff);

        if (LB_HID.Text.Trim(',') != "")
        {
            //string[] strHID = LB_HID.Text.Split(',');
            //EE20250417_改為參數化方式
            string[] strHID = LB_HID.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            using (con)
            {
                con.Open();

                foreach (string HCourseID in strHID)
                {
                    using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO HTodoList 
                (HCourseID, HGroupName, HTask, HTaskNum, HTaskContent, HGroupLeaderID, HStatus, HCreate, HCreateDT, HSave, HCBatchNum, HExamStaff) 
                VALUES 
                (@HCourseID, @HGroupName, @HTask, @HTaskNum, @HTaskContent, @HGroupLeaderID, @HStatus, @HCreate, @HCreateDT, @HSave, @HCBatchNum, @HExamStaff)", con))
                    {
                        cmd.Parameters.AddWithValue("@HCourseID", HCourseID);
                        cmd.Parameters.AddWithValue("@HGroupName", DDL_HGroupName.SelectedValue);
                        cmd.Parameters.AddWithValue("@HTask", DDL_HTask.SelectedValue);
                        cmd.Parameters.AddWithValue("@HTaskNum", TB_HTaskNum.Text.Trim());
                        cmd.Parameters.AddWithValue("@HTaskContent", HttpUtility.HtmlEncode(TB_HTaskContent.Text.Trim()));
                        cmd.Parameters.AddWithValue("@HGroupLeaderID", DDL_HGroupLeader.SelectedValue);
                        cmd.Parameters.AddWithValue("@HStatus", "1");
                        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@HSave", "1");
                        cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);
                        cmd.Parameters.AddWithValue("@HExamStaff", gHExamStaff);
                        cmd.ExecuteNonQuery();

                    }
                }
            }
        }

        SDS_HTodoList.SelectCommand = "SELECT HGroupName, HTask, HTaskContent, HGroupLeaderID,HSave, HTaskNum, HExamStaff FROM HTodoList where HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY HGroupName, HTask, HTaskContent, HGroupLeaderID,HSave, HTaskNum, HExamStaff ORDER BY HGroupName ASC";

        Rpt_HTodoList.DataBind();

        //清空
        DDL_HGroupName.SelectedValue = "0";
        DDL_HTask.SelectedValue = "0";
        TB_HTaskContent.Text = null;
        TB_HTaskNum.Text = null;
        DDL_HGroupLeader.SelectedValue = "0";
        LBox_HExamStaff.ClearSelection();
        LBox_HExamStaff.DataBind();
    }
    #endregion

    #endregion

    #region 作業

    #region 新增功能
    protected void LBtn_HW_Add_Click(object sender, EventArgs e)
    {

        if (LB_HCBatchNum.Text == "")
        {
            InsertCourserFun("HW_Add");
        }
        else
        {
            if (LB_HID.Text.Trim(',') != "")
            {
                UpdateCourseFun("HW_Add");
            }
        }


        #region 必填判斷

        if (string.IsNullOrEmpty(TB_HNumbers.Text.Trim()) && string.IsNullOrEmpty(TB_HDeadLine.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('要記得輸入數量與期限哦~');", true);
            return;
        }

        #endregion


        if (LB_HID.Text.Trim(',') != "")
        {

            //EE20250417_改為參數化方式
            string[] strHID = LB_HID.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            using (con)
            {
                con.Open();

                foreach (string HCourseID in strHID)
                {
                    using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO HCourse_HWSetting 
                (HCourseID, HNumbers, HDeadLine, HStatus, HCreate, HCreateDT, HSave, HCBatchNum, HHWType, HQuestionHID, HDescription)
                VALUES
                (@HCourseID, @HNumbers, @HDeadLine, @HStatus, @HCreate, @HCreateDT, @HSave, @HCBatchNum, @HHWType, @HQuestionHID, @HDescription)", con))
                    {
                        cmd.Parameters.AddWithValue("@HCourseID", HCourseID);
                        cmd.Parameters.AddWithValue("@HNumbers", TB_HNumbers.Text.Trim());
                        cmd.Parameters.AddWithValue("@HDeadLine", TB_HDeadLine.Text.Trim());
                        cmd.Parameters.AddWithValue("@HStatus", "1");
                        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@HSave", "0");
                        cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);
                        cmd.Parameters.AddWithValue("@HHWType", DDL_HHWType.SelectedValue);
                        cmd.Parameters.AddWithValue("@HQuestionHID", DDL_HQuestion.SelectedValue);
                        cmd.Parameters.AddWithValue("@HDescription", TB_HDescription.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }


        //清空資料
        TB_HNumbers.Text = null;
        TB_HDeadLine.Text = null;
        TB_HDescription.Text = null;
        DDL_HQuestion.SelectedValue = "0";

        //AE20240329_加入作業類型、問卷、內容描述
        SDS_HCourseHWSetting.SelectCommand = "SELECT a.HCBatchNum, a.HNumbers, a.HDeadLine, a.HSave, a.HHWType, a.HQuestionHID, a.HDescription, b.HTitle FROM HCourse_HWSetting AS a  LEFT JOIN HQuestion AS b ON a.HQuestionHID= b.HID WHERE a.HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY a.HCBatchNum, a.HNumbers, a.HDeadLine, a.HSave,  a.HHWType, a.HQuestionHID, a.HDescription, b.HTitle";
        RPT_HCourseHWSetting.DataBind();


    }
    #endregion

    #region 作業刪除功能
    protected void LBtn_HW_Del_Click(object sender, EventArgs e)
    {
        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        //EE20250417_改為參數化方式
        using (con)
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand(@"
        DELETE FROM HCourse_HWSetting 
        WHERE HCBatchNum = @HCBatchNum 
        AND HNumbers = @HNumbers 
        AND HDeadLine = @HDeadLine", con))
            {
                cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);
                cmd.Parameters.AddWithValue("@HNumbers", ((Label)RI.FindControl("LB_HNumbers")).Text);
                cmd.Parameters.AddWithValue("@HDeadLine", ((Label)RI.FindControl("LB_HDeadLine")).Text);

                cmd.ExecuteNonQuery();
            }
        }

        //AE20240329_加入作業類型、問卷、內容描述
        SDS_HCourseHWSetting.SelectCommand = "SELECT a.HCBatchNum, a.HNumbers, a.HDeadLine, a.HSave, a.HHWType, a.HQuestionHID, a.HDescription, b.HTitle FROM HCourse_HWSetting AS a  LEFT JOIN HQuestion AS b ON a.HQuestionHID= b.HID WHERE a.HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY a.HCBatchNum, a.HNumbers, a.HDeadLine, a.HSave,  a.HHWType, a.HQuestionHID, a.HDescription, b.HTitle";
        //RPT_HCourseHWSetting.DataBind();
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
            ((Label)e.Item.FindControl("LB_HHWType")).Text = "回應(可上傳圖片/PDF/Excel)";
        }
        else
        {
            ((Label)e.Item.FindControl("LB_HHWType")).Text = "問卷";
        }
    }
    #endregion

    #endregion

    #region 儲存功能
    protected void Btn_Submit_Click(object sender, EventArgs e)
    {


        if (LB_HCBatchNum.Text == "")
        {
            InsertCourserFun("Submit");
        }
        else
        {
            if (LB_HID.Text.Trim(',') != "")
            {
                string[] strHID = LB_HID.Text.Trim(',').Split(',');

                UpdateCourseFun("Submit");

                foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
                {
                    if (LBoxHOCPlace.Selected == true)
                    {
                        for (int i = 0; i < strHID.Length; i++)
                        {
                            SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HCourseNum FROM HCourse WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'");
                            if (!dr.Read())
                            {
                                InsertCourserFun("Submit");
                            }
                            dr.Close();
                        }
                    }
                }
            }
        }


    }
    #endregion

    #region 送審功能
    protected void Btn_Verify_Click(object sender, EventArgs e)
    {
        if (LB_HCBatchNum.Text == "")
        {
            InsertCourserFun("Verify");
        }
        else
        {
            if (LB_HID.Text.Trim(',') != "")
            {
                UpdateCourseFun("Verify");
            }
        }
    }
    #endregion

    #region 取消功能
    protected void Btn_Cancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("HCourse_Edit.aspx");
    }
    #endregion

    #region 上傳照片 (.gif, .jpg, .bmp, .png)
    protected void Upload_Pic()
    {
        //建立資料夾
        Directory.CreateDirectory(CourseImgRoot);

        if (FU_HImg.HasFile)
        {
            //上傳檔是否大於10M
            if (FU_HImg.PostedFile.ContentLength > (10 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的圖片不能超過10MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_HImg.FileName);
                //檔名
                LB_PicName.Text = "CourseImg_" + DateTime.Now.ToString("yyMMddmmss") + FileExtension;

                this.FU_HImg.SaveAs(Server.MapPath("~/uploads/Course/" + LB_PicName.Text));


            }
        }
    }
    #endregion

    #region 上傳檔案 (.xls、.xlsx、pdf、.doc、.docx)
    protected void Upload_File()
    {
        //建立資料夾
        Directory.CreateDirectory(CourseFileRoot);

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
                LB_BCSchedule.Text = "CourseFile_" + DateTime.Now.ToString("yyMMddmmss") + FileExtension;

                this.FU_BCSchedule.SaveAs(Server.MapPath("~/uploads/CourseFile/" + LB_BCSchedule.Text));
            }
        }

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
                LB_BECSchedule.Text = "CourseFile_" + DateTime.Now.ToString("yyMMddmmss") + FileExtension;

                this.FU_BECSchedule.SaveAs(Server.MapPath("~/uploads/CourseFile/" + LB_BECSchedule.Text));
            }
        }

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
                LB_HBudgetTable.Text = "CourseFile_" + DateTime.Now.ToString("yyMMddmmss") + FileExtension;

                this.FU_HBudgetTable.SaveAs(Server.MapPath("~/uploads/CourseFile/" + LB_HBudgetTable.Text));
            }
        }

    }
    #endregion

    #region 檢覈評比項目
    //EA20240624_新增檢覈評比項目：新增功能、刪除功能
    #region 檢覈評比項目-新增功能
    protected void LBtn_HCE_Add_Click(object sender, EventArgs e)
    {
        if (LB_HCBatchNum.Text == "")
        {
            InsertCourserFun("HCE_Add");
        }
        else
        {
            if (LB_HID.Text.Trim(',') != "")
            {
                UpdateCourseFun("HCE_Add");
            }
        }

        #region 必填判斷

        if (string.IsNullOrEmpty(TB_HCENum.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('請輸入數量');", true);
            return;
        }

        #endregion

        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //string[] strHID = LB_HID.Text.Split(',');

            using (con)
            {
                con.Open();

                foreach (string HCourseID in strHID)
                {
                    using (SqlCommand cmd = new SqlCommand(@"
            INSERT INTO HCourseEvaluation 
            (HCourseID, HCBatchNum, HCEType, HCEContent, HCENum, HStatus, HCreate, HCreateDT, HModify, HModifyDT) 
            VALUES 
            (@HCourseID, @HCBatchNum, @HCEType, @HCEContent, @HCENum, @HStatus, @HCreate, @HCreateDT, @HModify, @HModifyDT)", con))
                    {
                        cmd.Parameters.AddWithValue("@HCourseID", HCourseID);
                        cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text.Trim());
                        cmd.Parameters.AddWithValue("@HCEType", DDL_HCEType.SelectedValue);
                        cmd.Parameters.AddWithValue("@HCEContent", TB_HCEContent.Text.Trim());
                        cmd.Parameters.AddWithValue("@HCENum", TB_HCENum.Text.Trim());
                        cmd.Parameters.AddWithValue("@HStatus", "1");
                        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                        cmd.ExecuteNonQuery();
                    }
                }

            }
        }

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

        using (con)
        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand(@"
        DELETE FROM HCourseEvaluation 
        WHERE HCBatchNum = @HCBatchNum 
        AND HCEType = @HCEType 
        AND HCEContent = @HCEContent 
        AND HCENum = @HCENum", con))
            {
                cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);
                cmd.Parameters.AddWithValue("@HCEType", ((Label)RI.FindControl("LB_HCEType")).Text);
                cmd.Parameters.AddWithValue("@HCEContent", ((Label)RI.FindControl("LB_HCEContent")).Text);
                cmd.Parameters.AddWithValue("@HCENum", ((Label)RI.FindControl("LB_HCENum")).Text);

                cmd.ExecuteNonQuery();
            }
        }

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

    #region 講師教材設定
    protected void Rpt_HTMaterialDetail_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;
        ((DropDownList)e.Item.FindControl("DDL_HTMaterial")).SelectedValue = ((DropDownList)e.Item.FindControl("DDL_HTMaterial")).Items.FindByValue(gDRV["HTMaterialID"].ToString()) != null ? gDRV["HTMaterialID"].ToString() : "0";
    }
    #endregion

    #region Tab切換
    protected void LBtn_NavTab_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        Status(btn.TabIndex);
        btn.CssClass = "nav-link active show";
        RegScript();//註冊js


    }
    #endregion

    #region Tab切換Function
    public void Status(int TabIndex)
    {
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

                //AE20231217_改成另開顯示前導課程設定頁面
                if (TabIndex == 5)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "newpage", "window.open('HLCourseRelated_Add.aspx','_parent,toolbar=yes,location=no,directories=yes,status=yes,menubar=yes,resizable=yes,scrollbars=yes');", true);
                }
                else if (TabIndex == 2)  //開課資訊
                {
                    LoadTeacherList();//EE20250415_課程講師選單

                    SDS_HDonationItem.SelectCommand = "SELECT HID, HDItem FROM HDonationItem WHERE HStatus=1";
                    DDL_HCCPeriodDItem.DataSourceID = "SDS_HDonationItem";
                    DDL_HCCPeriodDItem.DataTextField = "HDItem";
                    DDL_HCCPeriodDItem.DataValueField = "HID";
                    DDL_HCCPeriodDItem.DataBind();

                    //EA20250531_檢覈課程=>上課地點為檢覈用地點；無法變更
                    if (RBL_TestCourse.SelectedValue == "1")
                    {
                        SetSelectedItems(LBox_HOCPlace, "31");
                        LBox_HOCPlace.Enabled = false;
                    }
                    else
                    {
                        SetSelectedItems(LBox_HOCPlace, "0");
                        LBox_HOCPlace.Enabled = true;
                    }
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "ListBoxInfo", " initSelect2('LBox_HSupervise', 'HCourse_Add.aspx?GetSuperviseList', '請搜尋督導'); initSelect2('LBox_HTeacherName', 'HCourse_Add.aspx/GetTeacherList', '請搜尋講師');initSelect2('LBox_HOCPlace', 'HCourse_Add.aspx/GetPlaceList', '搜尋上課地點'); initSelect2('LBox_HTeam', 'HCourse_Add.aspx/GetTeamList', '請搜尋主班團隊');", true);


                }

            }

        }

    }
    #endregion

    #region 執行js
    public void RegScript()
    {

        TB_HDateRange.CssClass = RBL_Continuous.SelectedValue == "0" ? "form-control datemultipicker" : "form-control daterange";

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
                orientation: 'bottom auto'
            });

            $('.datesinglepicker').datepicker({
                language: 'en',
                dateFormat: 'yyyy/mm/dd',
                multipleDates: false,
                minDate: new Date(),
                todayHighlight: true,
                orientation: 'bottom auto'
            });
        });
    ";

        ScriptManager.RegisterStartupScript(Page, GetType(), "AllInitScripts", script, true);

    }
    #endregion

    #region 新增課程Function
    private void InsertCourserFun(string From)
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
            return;
        }
        #endregion

        #region 將上傳之教材暫存檔名改為正式檔名
        SqlConnection dbConn = default(SqlConnection);
        SqlCommand Cmd = default(SqlCommand);
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
        Cmd = new SqlCommand(strSelHCM, dbConn);
        SqlDataReader MyQueryHCM = Cmd.ExecuteReader();

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
        Cmd.Cancel();
        dbConn.Close();
        #endregion

        #region 上傳圖片
        bool fileIsValid = false;
        //上傳圖片
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
                //上傳圖片
                Upload_Pic();
            }
            else
            {
                FU_HImg.Controls.Clear();
                Response.Write("<script>alert('只能上傳.gif.jpg.bmp.png的圖檔喔！');</script>");
                return;
            }
        }
        #endregion

        #region 上傳檔案
        //上傳檔案
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
                //上傳檔案
                Upload_File();
            }
            else
            {
                FU_BCSchedule.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        //上傳檔案
        if (FU_HBudgetTable.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_HBudgetTable.FileName).ToLower();
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
                //上傳檔案
                Upload_File();
            }
            else
            {
                FU_HBudgetTable.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }
        #endregion


        #region 取ListBox的值

        string gLBox_HRSystem = GetSelectedValues(LBox_HRSystem);
        string gLBox_HNRequirement = GetSelectedValues(LBox_HNRequirement);
        string gLBox_HTeam = GetSelectedValues(LBox_HTeam);
        string gLBox_HIRestriction = GetSelectedValues(LBox_HIRestriction);
        string gLBox_HTeacherName = GetSelectedValues(LBox_HTeacherName);
        string gLBox_HQuestionID = GetSelectedValues(LBox_HQuestionID);
        string gLBox_HTMaterialID = "";
        //string gLBox_HTMaterialID = GetSelectedValues(LBox_HTMaterialID);
        string gLBox_HExamPaperID = GetSelectedValues(LBox_HExamPaperID);
        string gLBox_HSupervise = GetSelectedValues(LBox_HSupervise);

        #endregion

        #region 單號自動產生流水號
        string StrHCBatchNum = null;
        string day = DateTime.Now.ToString("yyyyMMdd");

        SqlDataReader AutoReg = SQLdatabase.ExecuteReader("SELECT TOP (1) HCBatchNum FROM HCourse WHERE HCBatchNum LIKE '%" + day + "%' ORDER BY HCBatchNum DESC");
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
        AutoReg.Close();


        if (string.IsNullOrEmpty(LB_HCBatchNum.Text))
        {
            LB_HCBatchNum.Text = StrHCBatchNum;
        }

        #endregion




        string HID = "";
        string strUniteTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        //220819-基本費用(元)存入DB要先轉換成點數概念(1元=10點)
        int BasicPrice = 0;
        //int BasicPrice = Convert.ToInt32(TB_HBCPoint.Text.Trim()) / 10;
        if (!string.IsNullOrEmpty(TB_HBCPoint.Text.Trim()))
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
                        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HCourseNum FROM HCourse WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HOCPlace='" + LBoxHOCPlace.Value + "'");
                        if (!dr.Read())
                        {
                            string strHC = "EXECUTE [sp_HCourse_Insert] @StatementType, @HCTemplateID, @HCBatchNum, @HCourseName, @HTeacherName, @HOCPlace, @HDateRange, @HSTime, @HETime, @HType, @HOSystem, @HRSystem, @HNLCourse, @HNGuide, @HNFull, @HNRequirement, @HTeam,  @HQuestionID, @HPMethod, @HBCPoint, @HSGList, @HIRestriction, @HRemark, @HContentTitle, @HContent, @HRNContent, @HVerifyNum, @HApplicant, @HApplyTime, @HVerifyTime, @HVerifyStatus, @HStatus, @HCMID, @HCMDT,  @HSave, @HImg, @BCSchedule, @BECSchedule, @TPosition, @HBudget, @HBudgetTable, @HSCourseID, @HLodging, @HTMaterialID, @HCDeadLine, @HCDeadlineDay, @HSerial, @HDContinous, @HBudgetType, @HCourseType, @HAxisYN, @HAxisClass, @HExamSubject, @HGradeCalculation, @HExamPaperID,  @HSupervise, @HAttRateStandard, @HExamPassStandard, @HExamContentID, @HParticipantLimit, @HTeacherClass, @HTearcherLV, @HBookByDateYN, @HCCPeriodYN, @HCCPeriodDItem";

                            SqlCommand cmd = new SqlCommand(strHC, con);
                            con.Open();

                            cmd.Parameters.AddWithValue("@StatementType", "Insert");
                            cmd.Parameters.AddWithValue("@HCTemplateID", DDL_HCourseTemplate.SelectedValue);
                            cmd.Parameters.AddWithValue("@HCourseName", HttpUtility.HtmlEncode(TB_HCourseName.Text.Trim()));
                            cmd.Parameters.AddWithValue("@HTeacherName", gLBox_HTeacherName);
                            cmd.Parameters.AddWithValue("@HOCPlace", LBoxHOCPlace.Value);
                            cmd.Parameters.AddWithValue("@HDateRange", TB_HDateRange.Text);
                            cmd.Parameters.AddWithValue("@HSTime", TB_HSTime.Text);
                            cmd.Parameters.AddWithValue("@HETime", TB_HETime.Text);
                            cmd.Parameters.AddWithValue("@HType", DDL_HType.SelectedValue);
                            cmd.Parameters.AddWithValue("@HOSystem", DDL_HOSystem.SelectedValue);
                            cmd.Parameters.AddWithValue("@HRSystem", LB_HRSystem.Text);
                            cmd.Parameters.AddWithValue("@HNLCourse", RBL_HNLCourse.SelectedValue);
                            cmd.Parameters.AddWithValue("@HNGuide", RBL_HNGuide.SelectedValue);
                            cmd.Parameters.AddWithValue("@HNFull", RBL_HNFull.SelectedValue);
                            cmd.Parameters.AddWithValue("@HNRequirement", !string.IsNullOrEmpty(LB_HNRequirement.Text.Trim()) ? LB_HNRequirement.Text.Trim() : "");
                            cmd.Parameters.AddWithValue("@HTeam", gLBox_HTeam);
                            cmd.Parameters.AddWithValue("@HQuestionID", gLBox_HQuestionID);
                            cmd.Parameters.AddWithValue("@HPMethod", DDL_HPMethod.SelectedValue);
                            cmd.Parameters.AddWithValue("@HBCPoint", BasicPrice.ToString());
                            cmd.Parameters.AddWithValue("@HSGList", RBL_HSGList.SelectedValue);
                            cmd.Parameters.AddWithValue("@HIRestriction", LB_HIRestriction.Text.Trim());
                            cmd.Parameters.AddWithValue("@HRemark", HttpUtility.HtmlEncode(TB_HRemark.Text.Trim()));
                            cmd.Parameters.AddWithValue("@HContentTitle", HttpUtility.HtmlEncode(TB_HContentTitle.Text.Trim()));
                            cmd.Parameters.AddWithValue("@HContent", HttpUtility.HtmlEncode(CKE_HContent.Text.Trim()));
                            cmd.Parameters.AddWithValue("@HRNContent", HttpUtility.HtmlEncode(CKE_HRNContent.Text.Trim()));
                            cmd.Parameters.AddWithValue("@HVerifyNum", "");
                            cmd.Parameters.AddWithValue("@HApplicant", ((Label)Master.FindControl("LB_HUserHID")).Text);
                            cmd.Parameters.AddWithValue("@HApplyTime", strUniteTime);
                            cmd.Parameters.AddWithValue("@HVerifyTime", "");
                            cmd.Parameters.AddWithValue("@HCMID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                            cmd.Parameters.AddWithValue("@HCMDT", strUniteTime);
                            cmd.Parameters.AddWithValue("@HSave", "1");
                            cmd.Parameters.AddWithValue("@HImg", LB_PicName.Text);
                            cmd.Parameters.AddWithValue("@BCSchedule", !string.IsNullOrEmpty(LB_BCSchedule.Text.Trim()) ? LB_BCSchedule.Text.Trim() : "");
                            cmd.Parameters.AddWithValue("@BECSchedule", !string.IsNullOrEmpty(LB_BECSchedule.Text.Trim()) ? LB_BECSchedule.Text.Trim() : "");
                            cmd.Parameters.AddWithValue("@TPosition", "");
                            cmd.Parameters.AddWithValue("@HBudget", RBL_HBudget.SelectedValue);
                            cmd.Parameters.AddWithValue("@HBudgetTable", !string.IsNullOrEmpty(LB_HBudgetTable.Text.Trim()) ? LB_HBudgetTable.Text.Trim() : "");
                            cmd.Parameters.AddWithValue("@HSCourseID", "0");
                            cmd.Parameters.AddWithValue("@HLodging", RBL_HLodging.SelectedValue);
                            cmd.Parameters.AddWithValue("@HTMaterialID", gLBox_HTMaterialID);
                            cmd.Parameters.AddWithValue("@HCDeadLine", TB_HCDeadline.Text);
                            cmd.Parameters.AddWithValue("@HCDeadlineDay", TB_HCDeadlineDay.Text.Trim());
                            cmd.Parameters.AddWithValue("@HSerial", RBL_HSerial.SelectedValue);
                            cmd.Parameters.AddWithValue("@HDContinous", RBL_Continuous.SelectedValue);
                            cmd.Parameters.AddWithValue("@HBudgetType", DDL_HBudgetType.SelectedValue != "0" ? DDL_HBudgetType.SelectedItem.Text : "");
                            cmd.Parameters.AddWithValue("@HCourseType", DDL_HCourseType.SelectedValue);

                            //EA20240618_新增欄位
                            cmd.Parameters.AddWithValue("@HAxisYN", RBL_HAxisYN.SelectedValue);
                            cmd.Parameters.AddWithValue("@HAxisClass", DDL_HAxisClass.SelectedValue);
                            cmd.Parameters.AddWithValue("@HExamSubject", DDL_HExamSubject.SelectedValue);
                            cmd.Parameters.AddWithValue("@HGradeCalculation", DDL_HGradeCalculation.SelectedValue);
                            cmd.Parameters.AddWithValue("@HExamPaperID", gLBox_HExamPaperID);
                            cmd.Parameters.AddWithValue("@HSupervise", gLBox_HSupervise);
                            cmd.Parameters.AddWithValue("@HAttRateStandard", TB_HAttRateStandard.Text);
                            cmd.Parameters.AddWithValue("@HExamPassStandard", TB_HExamPassStandard.Text);

                            //EA20250509_新增欄位(檢覈內容名稱、報名人數上限、通過後成為的講師類別、通過後成為的講師層級分類)
                            cmd.Parameters.AddWithValue("@HExamContentID", DDL_HExamContentID.SelectedValue);
                            cmd.Parameters.AddWithValue("@HParticipantLimit", TB_HParticipantLimit.Text);
                            cmd.Parameters.AddWithValue("@HTeacherClass", DDL_HTeacherClass.SelectedValue);
                            cmd.Parameters.AddWithValue("@HTearcherLV", DDL_HTearcherLV.SelectedValue);

                            //AA20250611_新增HBookByDate(是否開放單天報名)
                            cmd.Parameters.AddWithValue("@HBookByDateYN", RBL_HBookByDateYN.SelectedValue);

                            //AA20250611_新增@HCCPeriodYN
                            cmd.Parameters.AddWithValue("@HCCPeriodYN", RBL_HCCPeriodYN.SelectedValue);
                            cmd.Parameters.AddWithValue("@HCCPeriodDItem", DDL_HCCPeriodDItem.SelectedValue);

                            //cmd.Parameters.AddWithValue("@HID", LB_HID.Text);
                            cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);
                            cmd.Parameters.AddWithValue("@HStatus", "1");

                            if (From == "Submit" || From == "HCourseMaterial_add" || From == "HTodoList_add" || From == "HW_Add" || From == "HCE_Add")
                            {
                                cmd.Parameters.AddWithValue("@HVerifyStatus", "99");
                            }
                            else if (From == "Verify")
                            {
                                cmd.Parameters.AddWithValue("@HVerifyStatus", "0");
                            }

                            HID += cmd.ExecuteScalar().ToString() + ",";
                            con.Close();
                            cmd.Cancel();
                        }
                        dr.Close();
                    }

                }
            }
        }
        else
        {
            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    string strHC = "EXECUTE [sp_HCourse_Insert] @StatementType, @HCTemplateID, @HCBatchNum, @HCourseName, @HTeacherName, @HOCPlace, @HDateRange, @HSTime, @HETime, @HType, @HOSystem, @HRSystem, @HNLCourse, @HNGuide, @HNFull, @HNRequirement, @HTeam,  @HQuestionID, @HPMethod, @HBCPoint, @HSGList, @HIRestriction, @HRemark, @HContentTitle, @HContent, @HRNContent, @HVerifyNum, @HApplicant, @HApplyTime, @HVerifyTime, @HVerifyStatus, @HStatus, @HCMID, @HCMDT,  @HSave, @HImg, @BCSchedule, @BECSchedule, @TPosition, @HBudget, @HBudgetTable, @HSCourseID, @HLodging, @HTMaterialID, @HCDeadLine, @HCDeadlineDay, @HSerial, @HDContinous, @HBudgetType, @HCourseType, @HAxisYN, @HAxisClass, @HExamSubject, @HGradeCalculation, @HExamPaperID, @HSupervise, @HAttRateStandard, @HExamPassStandard, @HExamContentID, @HParticipantLimit, @HTeacherClass, @HTearcherLV, @HBookByDateYN,@HCCPeriodYN,@HCCPeriodDItem";

                    SqlCommand cmd = new SqlCommand(strHC, con);
                    con.Open();

                    cmd.Parameters.AddWithValue("@StatementType", "Insert");
                    cmd.Parameters.AddWithValue("@HCTemplateID", DDL_HCourseTemplate.SelectedValue);
                    cmd.Parameters.AddWithValue("@HCourseName", HttpUtility.HtmlEncode(TB_HCourseName.Text.Trim()));
                    cmd.Parameters.AddWithValue("@HTeacherName", gLBox_HTeacherName);
                    cmd.Parameters.AddWithValue("@HOCPlace", LBoxHOCPlace.Value);
                    cmd.Parameters.AddWithValue("@HDateRange", TB_HDateRange.Text);
                    cmd.Parameters.AddWithValue("@HSTime", TB_HSTime.Text);
                    cmd.Parameters.AddWithValue("@HETime", TB_HETime.Text);
                    cmd.Parameters.AddWithValue("@HType", DDL_HType.SelectedValue);
                    cmd.Parameters.AddWithValue("@HOSystem", DDL_HOSystem.SelectedValue);
                    cmd.Parameters.AddWithValue("@HRSystem", LB_HRSystem.Text);
                    cmd.Parameters.AddWithValue("@HNLCourse", RBL_HNLCourse.SelectedValue);
                    cmd.Parameters.AddWithValue("@HNGuide", RBL_HNGuide.SelectedValue);
                    cmd.Parameters.AddWithValue("@HNFull", RBL_HNFull.SelectedValue);
                    cmd.Parameters.AddWithValue("@HNRequirement", !string.IsNullOrEmpty(LB_HNRequirement.Text.Trim()) ? LB_HNRequirement.Text.Trim() : "");
                    cmd.Parameters.AddWithValue("@HTeam", gLBox_HTeam);
                    cmd.Parameters.AddWithValue("@HQuestionID", gLBox_HQuestionID);
                    cmd.Parameters.AddWithValue("@HPMethod", DDL_HPMethod.SelectedValue);
                    cmd.Parameters.AddWithValue("@HBCPoint", BasicPrice.ToString());
                    cmd.Parameters.AddWithValue("@HSGList", RBL_HSGList.SelectedValue);
                    cmd.Parameters.AddWithValue("@HIRestriction", LB_HIRestriction.Text.Trim());
                    cmd.Parameters.AddWithValue("@HRemark", HttpUtility.HtmlEncode(TB_HRemark.Text.Trim()));
                    cmd.Parameters.AddWithValue("@HContentTitle", HttpUtility.HtmlEncode(TB_HContentTitle.Text.Trim()));
                    cmd.Parameters.AddWithValue("@HContent", HttpUtility.HtmlEncode(CKE_HContent.Text.Trim()));
                    cmd.Parameters.AddWithValue("@HRNContent", HttpUtility.HtmlEncode(CKE_HRNContent.Text.Trim()));
                    cmd.Parameters.AddWithValue("@HVerifyNum", "");
                    cmd.Parameters.AddWithValue("@HApplicant", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    cmd.Parameters.AddWithValue("@HApplyTime", strUniteTime);
                    cmd.Parameters.AddWithValue("@HVerifyTime", "");
                    cmd.Parameters.AddWithValue("@HCMID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    cmd.Parameters.AddWithValue("@HCMDT", strUniteTime);
                    cmd.Parameters.AddWithValue("@HSave", "1");
                    cmd.Parameters.AddWithValue("@HImg", LB_PicName.Text);
                    cmd.Parameters.AddWithValue("@BCSchedule", !string.IsNullOrEmpty(LB_BCSchedule.Text.Trim()) ? LB_BCSchedule.Text.Trim() : "");
                    cmd.Parameters.AddWithValue("@BECSchedule", !string.IsNullOrEmpty(LB_BECSchedule.Text.Trim()) ? LB_BECSchedule.Text.Trim() : "");
                    cmd.Parameters.AddWithValue("@TPosition", "");
                    cmd.Parameters.AddWithValue("@HBudget", RBL_HBudget.SelectedValue);
                    cmd.Parameters.AddWithValue("@HBudgetTable", !string.IsNullOrEmpty(LB_HBudgetTable.Text.Trim()) ? LB_HBudgetTable.Text.Trim() : "");
                    cmd.Parameters.AddWithValue("@HSCourseID", "0");
                    cmd.Parameters.AddWithValue("@HLodging", RBL_HLodging.SelectedValue);
                    cmd.Parameters.AddWithValue("@HTMaterialID", gLBox_HTMaterialID);
                    cmd.Parameters.AddWithValue("@HCDeadLine", TB_HCDeadline.Text);
                    cmd.Parameters.AddWithValue("@HCDeadlineDay", TB_HCDeadlineDay.Text.Trim());
                    cmd.Parameters.AddWithValue("@HSerial", RBL_HSerial.SelectedValue);
                    cmd.Parameters.AddWithValue("@HDContinous", RBL_Continuous.SelectedValue);
                    cmd.Parameters.AddWithValue("@HBudgetType", DDL_HBudgetType.SelectedValue != "0" ? DDL_HBudgetType.SelectedItem.Text : "");
                    cmd.Parameters.AddWithValue("@HCourseType", DDL_HCourseType.SelectedValue);

                    //EA20240618_新增欄位
                    cmd.Parameters.AddWithValue("@HAxisYN", RBL_HAxisYN.SelectedValue);
                    cmd.Parameters.AddWithValue("@HAxisClass", DDL_HAxisClass.SelectedValue);
                    cmd.Parameters.AddWithValue("@HExamSubject", DDL_HExamSubject.SelectedValue);
                    cmd.Parameters.AddWithValue("@HGradeCalculation", DDL_HGradeCalculation.SelectedValue);
                    cmd.Parameters.AddWithValue("@HExamPaperID", gLBox_HExamPaperID);
                    cmd.Parameters.AddWithValue("@HSupervise", gLBox_HSupervise);
                    cmd.Parameters.AddWithValue("@HAttRateStandard", TB_HAttRateStandard.Text);
                    cmd.Parameters.AddWithValue("@HExamPassStandard", TB_HExamPassStandard.Text);

                    //EA20250509_新增欄位(檢覈內容名稱、報名人數上限、通過後成為的講師類別、通過後成為的講師層級分類)
                    cmd.Parameters.AddWithValue("@HExamContentID", DDL_HExamContentID.SelectedValue);
                    cmd.Parameters.AddWithValue("@HParticipantLimit", TB_HParticipantLimit.Text);
                    cmd.Parameters.AddWithValue("@HTeacherClass", DDL_HTeacherClass.SelectedValue);
                    cmd.Parameters.AddWithValue("@HTearcherLV", DDL_HTearcherLV.SelectedValue);

                    //AA20250611_新增HBookByDate(是否開放單天報名)
                    cmd.Parameters.AddWithValue("@HBookByDateYN", RBL_HBookByDateYN.SelectedValue);

                    //AA20250611_新增HCCPeriodYN
                    cmd.Parameters.AddWithValue("@HCCPeriodYN", RBL_HCCPeriodYN.SelectedValue);
                    cmd.Parameters.AddWithValue("@HCCPeriodDItem", DDL_HCCPeriodDItem.SelectedValue);

                    //cmd.Parameters.AddWithValue("@HID", LB_HID.Text);
                    cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);
                    cmd.Parameters.AddWithValue("@HStatus", "1");

                    if (From == "Submit" || From == "HCourseMaterial_add" || From == "HTodoList_add" || From == "HW_Add" || From == "HCE_Add")
                    {
                        cmd.Parameters.AddWithValue("@HVerifyStatus", "99");
                    }
                    else if (From == "Verify")
                    {
                        cmd.Parameters.AddWithValue("@HVerifyStatus", "0");
                    }

                    HID += cmd.ExecuteScalar().ToString() + ",";
                    con.Close();
                    cmd.Cancel();
                }
            }
        }
        LB_HID.Text = HID;

        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Split(',');
            for (int i = 0; i < strHID.Length - 1; i++)
            {
                //學員課程教材
                for (int x = 0; x < Rpt_HCourseMaterial.Items.Count; x++)
                {
                    SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HCMName, HCMaterial, HCMLink, HSave, HSort FROM HCourseMaterial  WHERE HCBatchNum='" + LB_HCBatchNum.Text + "' AND HCMName='" + ((TextBox)Rpt_HCourseMaterial.Items[x].FindControl("TB_HCMName")).Text.Trim() + "' AND  HCMaterial='" + ((HyperLink)Rpt_HCourseMaterial.Items[x].FindControl("HL_HCMaterial")).Text + "' AND HCMLink='" + ((TextBox)Rpt_HCourseMaterial.Items[x].FindControl("TB_HCMLink")).Text.Trim() + "'");

                    if (dr.Read())
                    {
                        if (dr["HSave"].ToString() == "0")
                        //if (((Label)Rpt_HCourseMaterial.Items[x].FindControl("LB_HSave")).Text == "0")
                        {
                            string strUpdHTL = "update HCourseMaterial set HStatus='1', HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HCreateDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1' where HCBatchNum = '" + LB_HCBatchNum.Text + "'";
                            SqlCommand dbCmd = new SqlCommand(strUpdHTL, con);
                            con.Open();
                            dbCmd.ExecuteNonQuery();
                            con.Close();
                            dbCmd.Cancel();

                        }
                        else
                        {

                            SqlCommand cmd = new SqlCommand("INSERT INTO HCourseMaterial (HCourseID, HCMName, HCMaterial, HCMLink, HStatus, HCreate, HCreateDT, HSave, HCBatchNum, HSort) values (@HCourseID, @HCMName, @HCMaterial, @HCMLink, @HStatus, @HCreate, @HCreateDT, @HSave, @HCBatchNum, @HSort)", con);
                            con.Open();
                            cmd.Parameters.AddWithValue("@HCourseID", strHID[i]);
                            cmd.Parameters.AddWithValue("@HCMName", HttpUtility.HtmlEncode(((TextBox)Rpt_HCourseMaterial.Items[x].FindControl("TB_HCMName")).Text.Trim()));
                            cmd.Parameters.AddWithValue("@HCMaterial", ((HyperLink)Rpt_HCourseMaterial.Items[x].FindControl("HL_HCMaterial")).Text);
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

                    }
                    else
                    {

                        SqlCommand cmd = new SqlCommand("INSERT INTO HCourseMaterial (HCourseID, HCMName, HCMaterial, HCMLink, HStatus, HCreate, HCreateDT, HSave, HCBatchNum, HSort) values (@HCourseID, @HCMName, @HCMaterial, @HCMLink, @HStatus, @HCreate, @HCreateDT, @HSave, @HCBatchNum, @HSort)", con);
                        con.Open();
                        cmd.Parameters.AddWithValue("@HCourseID", strHID[i]);
                        cmd.Parameters.AddWithValue("@HCMName", HttpUtility.HtmlEncode(((TextBox)Rpt_HCourseMaterial.Items[x].FindControl("TB_HCMName")).Text.Trim()));
                        cmd.Parameters.AddWithValue("@HCMaterial", ((HyperLink)Rpt_HCourseMaterial.Items[x].FindControl("HL_HCMaterial")).Text);
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
                    dr.Close();

                }

                //體系護持工作項目
                for (int x = 0; x < Rpt_HTodoList.Items.Count; x++)
                {
                    if (((Label)Rpt_HTodoList.Items[x].FindControl("LB_HSave")).Text == "0")
                    {
                        string strUpdHTL = "update HTodoList set HStatus='1', HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HCreateDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1' where HCBatchNum = '" + LB_HCBatchNum.Text + "'";
                        SqlCommand dbCmd = new SqlCommand(strUpdHTL, con);
                        con.Open();
                        dbCmd.ExecuteNonQuery();
                        con.Close();
                        dbCmd.Cancel();

                    }
                    else
                    {

                        string strInsHC = "insert into HTodoList (HCourseID, HGroupName, HTask, HTaskNum, HTaskContent, HGroupLeaderID, HStatus, HCreate, HCreateDT, HSave, HCBatchNum) values ('" + strHID[i] + "', '" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupName")).SelectedValue + "', '" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HTask")).SelectedValue + "', '" + ((TextBox)Rpt_HTodoList.Items[x].FindControl("TB_HTaskNum")).Text.Trim() + "', '" + HttpUtility.HtmlEncode(((TextBox)Rpt_HTodoList.Items[x].FindControl("TB_HTaskContent")).Text.Trim()) + "','" + ((DropDownList)Rpt_HTodoList.Items[x].FindControl("DDL_HGroupLeader")).SelectedValue + "', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteTime + "', '1','" + LB_HCBatchNum.Text + "')";

                        SqlCommand dbCmd = new SqlCommand(strInsHC, con);
                        con.Open();
                        dbCmd.ExecuteNonQuery();
                        con.Close();
                        dbCmd.Cancel();

                    }



                }

                //講師教材
                for (int x = 0; x < Rpt_HTMaterialDetail.Items.Count; x++)
                {
                    if (((Label)Rpt_HTMaterialDetail.Items[x].FindControl("LB_HSave")).Text == "0")
                    {
                        string strUpdHTL = "update HCourseTMaterial set HStatus='1', HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HCreateDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1' where HCBatchNum = '" + LB_HCBatchNum.Text + "'";
                        SqlCommand dbCmd = new SqlCommand(strUpdHTL, con);
                        con.Open();
                        dbCmd.ExecuteNonQuery();
                        con.Close();
                        dbCmd.Cancel();

                    }
                    else
                    {

                        SqlCommand cmd = new SqlCommand("INSERT INTO HCourseTMaterial (HCBatchNum, HCourseID, HTMaterialID, HSort, HTMaterial, HStatus, HCreate, HCreateDT, HSave) values (@HCBatchNum, @HCourseID, @HTMaterialID, @HSort, @HTMaterial, @HStatus, @HCreate, @HCreateDT, @HSave)", con);
                        con.Open();

                        cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);
                        cmd.Parameters.AddWithValue("@HCourseID", strHID[i]);
                        cmd.Parameters.AddWithValue("@HTMaterialID", ((DropDownList)Rpt_HTMaterialDetail.Items[x].FindControl("DDL_HTMaterial")).SelectedValue);
                        cmd.Parameters.AddWithValue("@HSort", ((TextBox)Rpt_HTMaterialDetail.Items[x].FindControl("TB_HTMaterialSort")).Text.Trim());
                        cmd.Parameters.AddWithValue("@HTMaterial", ((DropDownList)Rpt_HTMaterialDetail.Items[x].FindControl("DDL_HTMaterial")).SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@HStatus", "1");
                        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@HSave", "1");

                        cmd.ExecuteNonQuery();
                        con.Close();
                        cmd.Cancel();

                    }

                }
            }

        }



        #region 日期判斷
        //AA20250611_新增判斷:若勾選開放單天報名，則日期會拆成一筆一筆的存法
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

                //寫入資料表
                DataTable dtCourseDate = (DataTable)ViewState["dtCourseDate"];


                for (int i = 0; i <= gHDateRange.Length - 1; i++)
                {
                    DateTime gHDate = Convert.ToDateTime(gHDateRange[i].ToString());//上課日
                    DataRow rowRC;
                    rowRC = dtCourseDate.NewRow();
                    rowRC["HCBatchNum"] = LB_HCBatchNum.Text;
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









        if (From == "Submit")
        {
            Response.Write("<script>alert('存檔成功!');window.location.href='HCourse_Edit.aspx';</script>");
        }
        else if (From == "Verify")
        {
            #region 寄信通知具審核權限者做開班審核 --先撈取審核者名單
            string Reciever = "";
            SqlDataReader QueryMail = SQLdatabase.ExecuteReader("SELECT C.HID,(SELECT DISTINCT(cast(B.HAccount AS NVARCHAR) + ',') FROM HRole CROSS APPLY SPLIT(HMemberID, ',') INNER JOIN HMember AS B ON value = B.HID WHERE  HRole.HID = C.HID FOR XML PATH('')) AS Email FROM HRole AS C WHERE C.HID=(SELECT TOP(1) HVRoleHID FROM HCVerifyUnit)");
            if (QueryMail.Read())
            {
                if (string.IsNullOrEmpty(QueryMail["Email"].ToString()))
                {
                    Response.Write("<script>alert('審核單位尚未選擇管理人員哦，請先建立常態任務中的權限名單~!');</script>");
                    return;
                }
                else
                {
                    Reciever = QueryMail["Email"].ToString().TrimEnd(',');
                }
            }
            QueryMail.Close();

            #endregion

            #region 寄信通知具審核權限者做開班審核 
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
            mail.Body = "<p>課程名稱：【" + TB_HCourseName.Text + "】需要審核哦~</p><p>請盡快至玉成系統後台 > 簽核管理 > 開班審核中做審核~謝謝~</p><br/><hr/><p style='font-weight:bold;'> 此郵件為和氣大愛玉成系統自動寄出，請勿回信。</p> ";

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
                Response.Write("<script>alert('寄驗證信失敗！請確認信箱是否正確');</script>");

            }
            #endregion

        }


    }
    #endregion

    #region 更新課程Function
    private void UpdateCourseFun(string From)
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
            return;
        }
        #endregion

        #region 上傳圖片
        bool fileIsValid = false;
        //上傳圖片
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
                //上傳圖片
                Upload_Pic();
            }
            else
            {
                FU_HImg.Controls.Clear();
                Response.Write("<script>alert('只能上傳.gif.jpg.bmp.png的圖檔喔！');</script>");
                return;
            }
        }
        #endregion

        #region 上傳檔案
        //上傳檔案
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
                //上傳檔案
                Upload_File();
            }
            else
            {
                FU_BCSchedule.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }

        //上傳檔案
        if (FU_HBudgetTable.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_HBudgetTable.FileName).ToLower();
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
                //上傳檔案
                Upload_File();
            }
            else
            {
                FU_HBudgetTable.Controls.Clear();
                Response.Write("<script>alert('只能上傳.xls, .xlsx, .pdf, .doc, .docx的檔案喔！');</script>");
                return;
            }
        }
        #endregion


        #region 取ListBox的值

        string gLBox_HRSystem = GetSelectedValues(LBox_HRSystem);
        string gLBox_HNRequirement = GetSelectedValues(LBox_HNRequirement);
        string gLBox_HTeam = GetSelectedValues(LBox_HTeam);
        string gLBox_HIRestriction = GetSelectedValues(LBox_HIRestriction);
        string gLBox_HTeacherName = GetSelectedValues(LBox_HTeacherName);
        string gLBox_HQuestionID = GetSelectedValues(LBox_HQuestionID);
        string gLBox_HTMaterialID = "";
        //string gLBox_HTMaterialID = GetSelectedValues(LBox_HTMaterialID);
        string gLBox_HExamPaperID = GetSelectedValues(LBox_HExamPaperID);
        string gLBox_HSupervise = GetSelectedValues(LBox_HSupervise);

        #endregion

        string strUniteTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        //220819-基本費用(元)存入DB要先轉換成點數概念(1元=10點)
        int BasicPrice = 0;
        //int BasicPrice = Convert.ToInt32(TB_HBCPoint.Text.Trim()) / 10;
        if (!string.IsNullOrEmpty(TB_HBCPoint.Text.Trim()))
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
                        string strHC = "EXECUTE [sp_HCourse_Update] @StatementType, @HID, @HCTemplateID, @HCBatchNum, @HCourseName, @HTeacherName, @HOCPlace, @HDateRange, @HSTime, @HETime, @HType, @HOSystem, @HRSystem, @HNLCourse, @HNGuide, @HNFull, @HNRequirement, @HTeam,  @HQuestionID, @HPMethod, @HBCPoint, @HSGList, @HIRestriction, @HRemark, @HContentTitle, @HContent, @HRNContent, @HVerifyNum, @HApplicant, @HApplyTime, @HVerifyTime, @HVerifyStatus, @HStatus, @HCMID, @HCMDT,  @HSave, @HImg, @BCSchedule, @BECSchedule, @TPosition, @HBudget, @HBudgetTable, @HSCourseID, @HLodging, @HTMaterialID, @HCDeadLine, @HCDeadlineDay, @HSerial, @HDContinous, @HBudgetType, @HCourseType, @HAxisYN, @HAxisClass, @HExamSubject, @HGradeCalculation, @HExamPaperID, @HSupervise, @HAttRateStandard, @HExamPassStandard, @HExamContentID, @HParticipantLimit, @HTeacherClass, @HTearcherLV, @HBookByDateYN";

                        SqlCommand cmd = new SqlCommand(strHC, con);
                        con.Open();

                        cmd.Parameters.AddWithValue("@StatementType", "Update");
                        cmd.Parameters.AddWithValue("@HCTemplateID", DDL_HCourseTemplate.SelectedValue);
                        cmd.Parameters.AddWithValue("@HCourseName", HttpUtility.HtmlEncode(TB_HCourseName.Text.Trim()));
                        cmd.Parameters.AddWithValue("@HTeacherName", gLBox_HTeacherName);
                        cmd.Parameters.AddWithValue("@HOCPlace", LBoxHOCPlace.Value);
                        cmd.Parameters.AddWithValue("@HDateRange", TB_HDateRange.Text);
                        cmd.Parameters.AddWithValue("@HSTime", TB_HSTime.Text);
                        cmd.Parameters.AddWithValue("@HETime", TB_HETime.Text);
                        cmd.Parameters.AddWithValue("@HType", DDL_HType.SelectedValue);
                        cmd.Parameters.AddWithValue("@HOSystem", DDL_HOSystem.SelectedValue);
                        cmd.Parameters.AddWithValue("@HRSystem", LB_HRSystem.Text);
                        cmd.Parameters.AddWithValue("@HNLCourse", RBL_HNLCourse.SelectedValue);
                        cmd.Parameters.AddWithValue("@HNGuide", RBL_HNGuide.SelectedValue);
                        cmd.Parameters.AddWithValue("@HNFull", RBL_HNFull.SelectedValue);
                        cmd.Parameters.AddWithValue("@HNRequirement", !string.IsNullOrEmpty(LB_HNRequirement.Text.Trim()) ? LB_HNRequirement.Text.Trim() : "");
                        cmd.Parameters.AddWithValue("@HTeam", gLBox_HTeam);
                        cmd.Parameters.AddWithValue("@HQuestionID", gLBox_HQuestionID);
                        cmd.Parameters.AddWithValue("@HPMethod", DDL_HPMethod.SelectedValue);
                        cmd.Parameters.AddWithValue("@HBCPoint", BasicPrice.ToString());
                        cmd.Parameters.AddWithValue("@HSGList", RBL_HSGList.SelectedValue);
                        cmd.Parameters.AddWithValue("@HIRestriction", LB_HIRestriction.Text.Trim());
                        cmd.Parameters.AddWithValue("@HRemark", HttpUtility.HtmlEncode(TB_HRemark.Text.Trim()));
                        cmd.Parameters.AddWithValue("@HContentTitle", HttpUtility.HtmlEncode(TB_HContentTitle.Text.Trim()));
                        cmd.Parameters.AddWithValue("@HContent", HttpUtility.HtmlEncode(CKE_HContent.Text.Trim()));
                        cmd.Parameters.AddWithValue("@HRNContent", HttpUtility.HtmlEncode(CKE_HRNContent.Text.Trim()));
                        cmd.Parameters.AddWithValue("@HVerifyNum", "");
                        cmd.Parameters.AddWithValue("@HApplicant", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        cmd.Parameters.AddWithValue("@HApplyTime", strUniteTime);
                        cmd.Parameters.AddWithValue("@HVerifyTime", "");
                        cmd.Parameters.AddWithValue("@HCMID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        cmd.Parameters.AddWithValue("@HCMDT", strUniteTime);
                        cmd.Parameters.AddWithValue("@HSave", "1");
                        cmd.Parameters.AddWithValue("@HImg", LB_PicName.Text);
                        cmd.Parameters.AddWithValue("@BCSchedule", !string.IsNullOrEmpty(LB_BCSchedule.Text.Trim()) ? LB_BCSchedule.Text.Trim() : "");
                        cmd.Parameters.AddWithValue("@BECSchedule", !string.IsNullOrEmpty(LB_BECSchedule.Text.Trim()) ? LB_BECSchedule.Text.Trim() : "");
                        cmd.Parameters.AddWithValue("@TPosition", "");
                        cmd.Parameters.AddWithValue("@HBudget", RBL_HBudget.SelectedValue);
                        cmd.Parameters.AddWithValue("@HBudgetTable", !string.IsNullOrEmpty(LB_HBudgetTable.Text.Trim()) ? LB_HBudgetTable.Text.Trim() : "");
                        cmd.Parameters.AddWithValue("@HSCourseID", "0");
                        cmd.Parameters.AddWithValue("@HLodging", RBL_HLodging.SelectedValue);
                        cmd.Parameters.AddWithValue("@HTMaterialID", gLBox_HTMaterialID);
                        cmd.Parameters.AddWithValue("@HCDeadLine", TB_HCDeadline.Text);
                        cmd.Parameters.AddWithValue("@HCDeadlineDay", TB_HCDeadlineDay.Text.Trim());
                        cmd.Parameters.AddWithValue("@HSerial", RBL_HSerial.SelectedValue);
                        cmd.Parameters.AddWithValue("@HDContinous", RBL_Continuous.SelectedValue);
                        cmd.Parameters.AddWithValue("@HBudgetType", DDL_HBudgetType.SelectedValue != "0" ? DDL_HBudgetType.SelectedItem.Text : "");
                        cmd.Parameters.AddWithValue("@HCourseType", DDL_HCourseType.SelectedValue);

                        //EA20240618_新增欄位
                        cmd.Parameters.AddWithValue("@HAxisYN", RBL_HAxisYN.SelectedValue);
                        cmd.Parameters.AddWithValue("@HAxisClass", DDL_HAxisClass.SelectedValue);
                        cmd.Parameters.AddWithValue("@HExamSubject", DDL_HExamSubject.SelectedValue);
                        cmd.Parameters.AddWithValue("@HGradeCalculation", DDL_HGradeCalculation.SelectedValue);
                        cmd.Parameters.AddWithValue("@HExamPaperID", gLBox_HExamPaperID);
                        cmd.Parameters.AddWithValue("@HSupervise", gLBox_HSupervise);
                        cmd.Parameters.AddWithValue("@HAttRateStandard", TB_HAttRateStandard.Text);
                        cmd.Parameters.AddWithValue("@HExamPassStandard", TB_HExamPassStandard.Text);

                        //EA20250509_新增欄位(檢覈內容名稱、報名人數上限、通過後成為的講師類別、通過後成為的講師層級分類)
                        cmd.Parameters.AddWithValue("@HExamContentID", DDL_HExamContentID.SelectedValue);
                        cmd.Parameters.AddWithValue("@HParticipantLimit", TB_HParticipantLimit.Text);
                        cmd.Parameters.AddWithValue("@HTeacherClass", DDL_HTeacherClass.SelectedValue);
                        cmd.Parameters.AddWithValue("@HTearcherLV", DDL_HTearcherLV.SelectedValue);

                        //AA20250611_新增HBookByDate(是否開放單天報名)
                        cmd.Parameters.AddWithValue("@HBookByDateYN", RBL_HBookByDateYN.SelectedValue);


                        cmd.Parameters.AddWithValue("@HID", strHID[i]);
                        cmd.Parameters.AddWithValue("@HCBatchNum", LB_HCBatchNum.Text);
                        cmd.Parameters.AddWithValue("@HVerifyStatus", "99");
                        cmd.Parameters.AddWithValue("@HStatus", "1");
                        cmd.ExecuteNonQuery();

                        con.Close();
                        cmd.Cancel();
                    }

                }
            }
        }


        #region 日期判斷
        //AA20250611_新增判斷:若勾選開放單天報名，則日期會拆成一筆一筆的存法
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

                //寫入資料表
                DataTable dtCourseDate = (DataTable)ViewState["dtCourseDate"];


                for (int i = 0; i <= gHDateRange.Length - 1; i++)
                {
                    DateTime gHDate = Convert.ToDateTime(gHDateRange[i].ToString());//上課日
                    DataRow rowRC;
                    rowRC = dtCourseDate.NewRow();
                    rowRC["HCBatchNum"] = LB_HCBatchNum.Text;
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





        if (From == "Submit")
        {
            Response.Write("<script>alert('存檔成功!');window.location.href='HCourse_Edit.aspx';</script>");
        }
        else if (From == "Verify")
        {
            #region 寄信通知具審核權限者做開班審核 --先撈取審核者名單
            string Reciever = "";
            SqlDataReader QueryMail = SQLdatabase.ExecuteReader("SELECT C.HID,(SELECT DISTINCT(cast(B.HAccount AS NVARCHAR) + ',') FROM HRole CROSS APPLY SPLIT(HMemberID, ',') INNER JOIN HMember AS B ON value = B.HID WHERE  HRole.HID = C.HID FOR XML PATH('')) AS Email FROM HRole AS C WHERE C.HID=(SELECT TOP(1) HVRoleHID FROM HCVerifyUnit)");
            if (QueryMail.Read())
            {
                if (string.IsNullOrEmpty(QueryMail["Email"].ToString()))
                {
                    Response.Write("<script>alert('審核單位尚未選擇管理人員哦，請先建立常態任務中的權限名單~!');</script>");
                    return;
                }
                else
                {
                    Reciever = QueryMail["Email"].ToString().TrimEnd(',');
                }
            }
            QueryMail.Close();

            #endregion

            #region 寄信通知具審核權限者做開班審核 
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
            mail.Body = "<p>課程名稱：【" + TB_HCourseName.Text + "】需要審核哦~</p><p>請盡快至玉成系統後台 > 簽核管理 > 開班審核中做審核~謝謝~</p><br/><hr/><p style='font-weight:bold;'> 此郵件為和氣大愛玉成系統自動寄出，請勿回信。</p> ";

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
                Response.Write("<script>alert('寄驗證信失敗！請確認信箱是否正確');</script>");

            }
            #endregion

        }


    }
    #endregion

    #region 刪除1天前未存檔的資料Function
    private void DeleteUnsavedCourseData(SqlConnection dbConn)
    {
        string[] tables = new string[]
     {
        "HCourse",
        "HCourseMaterial",
        "HLeadingCourse",
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

    #region 必填判斷Function
    private bool ValidateRequiredFields(out string message)
    {
        string alert = "";

        if (DDL_HCourseTemplate.SelectedValue == "0")
        {
            alert += "請選擇課程範本名稱！\\n";
        }

        if (string.IsNullOrEmpty(TB_HCourseName.Text))
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

        if (DDL_HCourseType.SelectedValue == "0")
        {
            alert += "請選擇課程類型（必修/選修）！\\n";
        }

        //加入必填判斷
        if (RBL_HCCPeriodYN.SelectedValue=="1" && DDL_HCCPeriodDItem.SelectedValue=="0")
        {
            alert += "有勾選要導向信用卡授權書，要填寫捐款項目名稱哦~！\\n";
        }

        message = alert;
        return string.IsNullOrEmpty(alert);
    }
    #endregion

    #region 課程講師選單Function
    private void LoadTeacherList()
    {
        using (SqlDataReader reader = SQLdatabase.ExecuteReader(@"SELECT HTeacher.HID, HMemberID, HUserName, HTeacher.HContent, HTeacher.HStatus, HPeriod, HArea, (HArea+'/'+HPeriod+' '+HUserName) AS UserName, HCourseName FROM HTeacher INNER JOIN HMember ON HMemberID = HMember.HID LEFT JOIN HArea ON HMember.HAreaID = HArea.HID LEFT JOIN HCourse ON HCourseID=HCourse.HID"))
        {
            while (reader.Read())
            {
                string label = reader["UserName"] + "-" + reader["HCourseName"];
                if (reader["HStatus"].ToString() == "0")
                    label = "(停用)" + label;

                LBox_HTeacherName.Items.Add(new ListItem(label, reader["HID"].ToString()));
            }
            reader.Close();
        }
    }
    #endregion

    #region  暫時註解
    ////督導資料
    //[System.Web.Services.WebMethod]
    //public static List<ListItem> GetSuperviseList(string keyword)
    //{
    //    List<ListItem> result = new List<ListItem>();

    //    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString))
    //    {
    //        using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 20 HID, (HUserName + '-' + HAreaID + '-' + HPeriod) AS DisplayName 
    //                                              FROM HMember 
    //                                              WHERE HStatus=1 AND HType IN (7,8,9,10,11,12,13) 
    //                                                AND HUserName LIKE @kw + '%'", con))
    //        {
    //            cmd.Parameters.AddWithValue("@kw", keyword);
    //            con.Open();
    //            SqlDataReader reader = cmd.ExecuteReader();
    //            while (reader.Read())
    //            {
    //                result.Add(new ListItem(reader["DisplayName"].ToString(), reader["HID"].ToString()));
    //            }
    //        }
    //    }

    //    return result;
    //}

    //[System.Web.Services.WebMethod]
    //public static List<ListItem> GetPlaceList(string keyword)
    //{
    //    List<ListItem> result = new List<ListItem>();
    //    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString))
    //    {
    //        using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 20 HID, HPlaceName FROM HPlaceList WHERE HStatus = 1 AND HPlaceName LIKE @kw + '%'ORDER BY HStatus DESC", con))
    //        {
    //            cmd.Parameters.AddWithValue("@kw", keyword);
    //            con.Open();
    //            SqlDataReader reader = cmd.ExecuteReader();
    //            while (reader.Read())
    //            {
    //                result.Add(new ListItem(reader["HPlaceName"].ToString(), reader["HID"].ToString()));
    //            }
    //        }
    //    }
    //    return result;
    //}

    //[System.Web.Services.WebMethod]
    //public static List<ListItem> GetTeamList(string keyword)
    //{
    //    List<ListItem> result = new List<ListItem>();
    //    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString))
    //    {
    //        using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 20 HID, TeamName  FROM HCTeam WHERE HStatus=1 AND TeamName LIKE @kw + '%'", con))
    //        {
    //            cmd.Parameters.AddWithValue("@kw", keyword);
    //            con.Open();
    //            SqlDataReader reader = cmd.ExecuteReader();
    //            while (reader.Read())
    //            {
    //                result.Add(new ListItem(reader["TeamName"].ToString(), reader["HID"].ToString()));
    //            }
    //        }
    //    }
    //    return result;
    //}
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
    //public string ListBox(ListBox ListB)
    //{
    //    string ClassVal = "";
    //    string ClassSelectecd = string.Empty;
    //    foreach (ListItem li in ListB.Items)
    //    {
    //        if (li.Selected == true)
    //        {
    //            ClassVal += li.Value + ",";
    //        }
    //    }
    //    string ClassID = ClassVal.ToString();
    //    ClassID = ClassID.Trim(new Char[] { ',' });//去除結尾逗號

    //    return ClassID;
    //}
    #endregion

    #endregion

}