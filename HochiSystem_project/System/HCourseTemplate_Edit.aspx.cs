using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



public partial class HCourseTemplate_Edit : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

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

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        for (int i = 0; i < RPT_Tag.Items.Count; i++)
        {
            string currentHID = ((HiddenField)RPT_Tag.Items[i].FindControl("HF_HID")).Value;

            if (LB_NavTab.Text == currentHID)
            {
                ((LinkButton)RPT_Tag.Items[i].FindControl("LBtn_Tag")).CssClass = "nav-link active show";
            }
            else
            {
                ((LinkButton)RPT_Tag.Items[i].FindControl("LBtn_Tag")).CssClass = "nav-link";
            }

        }

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RegScript();//註冊js

        #region 分頁copy-2
        if (!IsPostBack)
        {
            ViewState["Search"] = "";
        }

        if (!IsPostBack)
        {
            SDS_HC_T.SelectCommand = "SELECT a.HID, a.HTemplateName, a.HOSystem, b.HSystemName, a.HRSystem, a.HPMethod FROM HCourse_T as a left join HSystem as b on a.HOSystem=b.HID Where a.HStatus='1' ORDER BY a.HTemplateNum DESC";

            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HC_T.SelectCommand, PageMax, LastPage, false, Rpt_HC_T);
            ViewState["Search"] = SDS_HC_T.SelectCommand;

        }
        else
        {
            SDS_HC_T.SelectCommand = ViewState["Search"].ToString();
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", ViewState["Search"].ToString(), PageMax, LastPage, false, Rpt_HC_T);
        }
        #endregion

        if (!IsPostBack)
        {


            SqlConnection dbConn = default(SqlConnection);
            SqlCommand dbCmd = default(SqlCommand);
            string strDBConn = null;
            strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
            dbConn = new SqlConnection(strDBConn);
            dbConn.Open();

            //刪除1天前未存檔的資料
            string strDelHC_T = "delete from HCourse_T where HSave='0' and HCreateDT < '" + DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss") + "'";
            dbCmd = new SqlCommand(strDelHC_T, dbConn);
            dbCmd.ExecuteNonQuery();

            //string strDelHCM_T = "delete from HCourseMaterial_T where HCTemplateID not in (select HID from HCourse_T)";
            string strDelHCM_T = "delete from HCourseMaterial_T where HSave='0' and HCreateDT < '" + DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss") + "'";
            dbCmd = new SqlCommand(strDelHCM_T, dbConn);
            dbCmd.ExecuteNonQuery();

            //string strDelHLC_T = "delete from HLeadingCourse_T where HCTemplateID not in (select HID from HCourse_T)";
            string strDelHLC_T = "delete from HLeadingCourse_T where HSave='0' and HCreateDT < '" + DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss") + "'";
            dbCmd = new SqlCommand(strDelHLC_T, dbConn);
            dbCmd.ExecuteNonQuery();

            //string strDelHTL_T = "delete from HTodoList_T where HCTemplateID not in (select HID from HCourse_T)";
            string strDelHTL_T = "delete from HTodoList_T where HSave='0' and HCreateDT < '" + DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss") + "'";
            dbCmd = new SqlCommand(strDelHTL_T, dbConn);
            dbCmd.ExecuteNonQuery();

            //刪除教材temp檔
            string gHCMFileTemp = "~/uploads/CourseMaterialTemp/";
            DeleteSrcFolder(Server.MapPath(gHCMFileTemp));

            //MA20250408_講師教材
            string strDelHCTM_T = "delete from  HCourseTMaterial_T where HSave='0' and HCreateDT < '" + DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss") + "'";
            dbCmd = new SqlCommand(strDelHCTM_T, dbConn);
            dbCmd.ExecuteNonQuery();


            string strSelHQ = "select HID, HTitle, HStatus from HQuestion WHERE HSave=1 order by HTitle";
            //Response.Write(strSQL);
            dbCmd = new SqlCommand(strSelHQ, dbConn);
            SqlDataReader MyQueryHQ = dbCmd.ExecuteReader();
            while (MyQueryHQ.Read())
            {
                if (MyQueryHQ["HStatus"].ToString() == "0")
                {
                    LBox_HQuestionID.Items.Add(new ListItem("(停用)" + MyQueryHQ["HTitle"].ToString(), MyQueryHQ["HID"].ToString()));
                }
                else
                {
                    LBox_HQuestionID.Items.Add(new ListItem(MyQueryHQ["HTitle"].ToString(), MyQueryHQ["HID"].ToString()));
                }
            }
            MyQueryHQ.Close();

      


            dbCmd.Cancel();
            dbConn.Close();






            SqlDataReader QueryHCourse_Class = SQLdatabase.ExecuteReader("SELECT HID, HCourseName FROM HCourse_Class");
            while (QueryHCourse_Class.Read())
            {
                DDL_HType.Items.Add(new ListItem(QueryHCourse_Class["HCourseName"].ToString(), QueryHCourse_Class["HID"].ToString()));
            }
            QueryHCourse_Class.Close();






            //DDL_TDept.Items.Add(new ListItem("-請選擇-", "0"));
            SqlDataReader QueryHGroup = SQLdatabase.ExecuteReader("SELECT HID, HGroupName FROM HGroup WHERE HStatus='1'");
            while (QueryHGroup.Read())
            {
                DDL_HGroupName.Items.Add(new ListItem(QueryHGroup["HGroupName"].ToString(), (QueryHGroup["HID"].ToString())));
            }
            QueryHGroup.Close();




        }
        //SDS_HC_T.SelectCommand = "SELECT HID, HTemplateName, HOSystem, HRSystem  FROM HCourse_T";
    }

    #region 搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        SDS_HC_T.SelectCommand = "SELECT a.HID, a.HTemplateName, a.HOSystem, b.HSystemName, a.HRSystem, a.HPMethod FROM HCourse_T as a left join HSystem as b on a.HOSystem=b.HID Where a.HStatus='1' and HTemplateName like'%" + TB_Search.Text + "%' ORDER BY a.HTemplateNum DESC";
        #region 分頁copy-3搜尋用
        ViewState["Search"] = SDS_HC_T.SelectCommand;
        //呼叫分頁 (連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 搜尋開啟, DataList控件)
        Pg_Paging.PagingLoad("HochiSystemConnection", SDS_HC_T.SelectCommand, PageMax, LastPage, true, Rpt_HC_T);
        #endregion


        //Rpt_HC_T.DataBind();
    }
    #endregion

    #region 取消搜尋功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_Search.Text = "";

        SDS_HC_T.SelectCommand = "SELECT a.HID, a.HTemplateName, a.HOSystem, b.HSystemName, a.HRSystem, a.HPMethod FROM HCourse_T as a left join HSystem as b on a.HOSystem=b.HID Where a.HStatus='1' ORDER BY a.HTemplateNum DESC";
        ViewState["Search"] = SDS_HC_T.SelectCommand;

        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HC_T.SelectCommand, PageMax, LastPage, true, Rpt_HC_T);
    }
    #endregion

    #region 列表繫結
    protected void Rpt_HC_T_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //取得DataRowView
        DataRowView gDRV = (DataRowView)e.Item.DataItem;



        string[] gHRSystem = gDRV["HRSystem"].ToString().Split(',');
        string gLB_HRSystem = "";
        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        for (int i = 0; i < gHRSystem.Length - 1; i++)
        {

            string strSelHS = "select HSystemName from HSystem where HID='" + gHRSystem[i].ToString() + "'";
            dbCmd = new SqlCommand(strSelHS, dbConn);
            SqlDataReader MyQueryHS = dbCmd.ExecuteReader();
            if (MyQueryHS.Read())
            {
                gLB_HRSystem = gLB_HRSystem + MyQueryHS["HSystemName"].ToString() + ",";
            }
            MyQueryHS.Close();

        }
        ((Label)e.Item.FindControl("LB_HRSystem")).Text = gLB_HRSystem.Length > 0 ? gLB_HRSystem.Substring(0, gLB_HRSystem.Length - 1) : gLB_HRSystem;

        dbConn.Close();


        //繳費帳戶 1:基金會、2:文化事業
        ((Label)e.Item.FindControl("LB_HPMethod")).Text = ((Label)e.Item.FindControl("LB_HPMethod")).Text == "1" ? "基金會" : ((Label)e.Item.FindControl("LB_HPMethod")).Text == "2" ? "文化事業" : "";


    }
    #endregion

    #region 編輯功能
    protected void LBtn_Edit_Click(object sender, EventArgs e)
    {
        Panel_List.Visible = false;
        Panel_Edit.Visible = true;

        #region Tab顯示

        RPT_Tag.DataSource = HCourse_NavTab.HCourseTemp_NavTab.NavTabs
.Where(tab => tab.Visible)
.Select(tab => new
{
    HID = tab.ID,
    HName_TW = tab.Name
}).ToList();

        RPT_Tag.DataBind();
        LB_NavTab.Text = "1";

        Panel_Template.Attributes.Add("class", "tab-pane fade active show");

        #endregion

        SDS_HSystem.SelectCommand = "SELECT HID, HSystemName, HStatus FROM HSystem WHERE HStatus=1";
        SDS_HDharma.SelectCommand = "SELECT HID, HDTypeID, HDharmaName FROM HDharma WHERE HStatus='1'";
        SDS_HMType.SelectCommand = "SELECT HID, HMType FROM HMType ORDER BY HID ASC";
        SDS_HBudgetType.SelectCommand = "SELECT HID, HContent, HStatus FROM HBudgetType WHERE HStatus=1";
        SDS_HExamContentName.SelectCommand = "SELECT HID, (HExamContentName+'-'+(CASE HExamType WHEN 1 THEN '筆試' WHEN 2 THEN '實作' WHEN '3' THEN '試教' ELSE '' END)) AS HExamContentName  FROM HExamContent WHERE HStatus=1";
        SDS_HExamSubject.SelectCommand = "SELECT HID, HExamSubjectName FROM HExamSubject WHERE HStatus=1";
        SDS_HTMaterial.SelectCommand = "SELECT a.HID, (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) AS HTMName, a.HTMaterial, a.HStatus FROM   HTeacherMaterial AS a LEFT JOIN HDocCTypeClass AS b ON a.HDocCTClassID=b.HID LEFT JOIN HCourseLevel AS c ON a.HCourseLevelID=c.HID LEFT JOIN HFileType AS d ON a.HFileTypeID=d.HID WHERE a.HStatus='1' AND (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) <>'' AND (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) IS NOT NULL ORDER BY a.HStatus DESC";

        LinkButton LBtn_Edit = sender as LinkButton;
        string Edit_CA = LBtn_Edit.CommandArgument;

        LB_HID.Text = Edit_CA;

        LBox_HNRequirement.DataBind();


        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        //WE20250611_新增HBookByDateYN(是否開放單天報名)欄位
        string strSelHC_T = "SELECT HID, HTemplateNum, HTemplateName, HType, HOSystem, HRSystem, HNLCourse, HNGuide, HNFull, HNRequirement, HTeam,HQuestionID, HPMethod, HBCPoint, HSGList, HIRestriction, HRemark, HContentTitle, HContent, HBudgetType, HSave, HVRoleID, HARoleID, HSerial, HBudget,  HLodging, HTMaterialID, HCDeadline, HCDeadlineDay, HAxisYN, HAxisClass, HExamSubject, HExamContentID, HParticipantLimit, HBookByDateYN, HCCPeriodYN  FROM HCourse_T WHERE HID='" + Edit_CA + "' AND HStatus='1'";
        dbCmd = new SqlCommand(strSelHC_T, dbConn);
        SqlDataReader MyQueryHC_T = dbCmd.ExecuteReader();

        //DDL_HOSystem.DataBind();
        LBox_HRSystem.DataBind();
        DDL_HBudgetType.DataBind();
        //LBox_HTMaterialID.DataBind();
        DDL_HExamSubject.DataBind();
        DDL_HExamContentID.DataBind();


        if (MyQueryHC_T.Read())
        {
            TB_HTemplateNum.Text = MyQueryHC_T["HTemplateNum"].ToString();
            TB_HTemplateName.Text = MyQueryHC_T["HTemplateName"].ToString();

            DDL_HType.SelectedValue = MyQueryHC_T["HType"].ToString();
            DDL_HOSystem.SelectedValue = MyQueryHC_T["HOSystem"].ToString();
            //LBox_HRSystem.SelectedValue = MyQueryHC_T["HRSystem"].ToString();
            string[] gHRSystem = MyQueryHC_T["HRSystem"].ToString().Split(',');
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

            TB_HCDeadline.Text = MyQueryHC_T["HCDeadline"].ToString();

            //AA20250611_新增HBookByDateYN欄位
            RBL_HBookByDateYN.SelectedValue = !string.IsNullOrEmpty(MyQueryHC_T["HBookByDateYN"].ToString()) ? MyQueryHC_T["HBookByDateYN"].ToString() : "0";

            //AA20250715_新增HCCPeriodYN欄位
            RBL_HCCPeriodYN.SelectedValue = !string.IsNullOrEmpty(MyQueryHC_T["HCCPeriodYN"].ToString()) ? MyQueryHC_T["HCCPeriodYN"].ToString() : "0";

            SqlDataReader QueryHCourse_Class = SQLdatabase.ExecuteReader("SELECT HID, HType FROM HCourse_Class where HID='" + MyQueryHC_T["HType"].ToString() + "'");
            if (QueryHCourse_Class.Read())
            {
                RBL_TestCourse.SelectedValue = QueryHCourse_Class["HType"].ToString() == "2" ? "1" : "0";
            }
            QueryHCourse_Class.Close();


            if (RBL_TestCourse.SelectedValue == "1")
            {
                //MA20240424_加入檢覈科目名稱
                if (DDL_HExamSubject.Items.FindByValue(MyQueryHC_T["HExamSubject"].ToString()) != null)
                {
                    DDL_HExamSubject.SelectedValue = MyQueryHC_T["HExamSubject"].ToString();
                    DDL_HExamSubject.Enabled = true;
                    Span_HExamSubject.Visible = true;
                }

                //MA20250503_加入檢覈內容名稱
                if (DDL_HExamContentID.Items.FindByValue(MyQueryHC_T["HExamContentID"].ToString()) != null)
                {
                    DDL_HExamContentID.SelectedValue = MyQueryHC_T["HExamContentID"].ToString();
                    DDL_HExamContentID.Enabled = true;
                }

            }
            else
            {
                DDL_HExamSubject.SelectedValue = "0";
                DDL_HExamSubject.Enabled = false;
                Span_HExamSubject.Visible = false;

                DDL_HExamContentID.SelectedValue = "0";
                DDL_HExamContentID.Enabled = false;
            }

            //RBL_TestCourse.SelectedValue = MyQueryHC_T["HNLCourse"].ToString();

            RBL_HNLCourse.SelectedValue = MyQueryHC_T["HNLCourse"].ToString();
            RBL_HNGuide.SelectedValue = MyQueryHC_T["HNGuide"].ToString();
            RBL_HNFull.SelectedValue = MyQueryHC_T["HNFull"].ToString();
            //RBL_HSerial.SelectedValue = MyQueryHC_T["HSerial"].ToString();
            ListItem HSerial = RBL_HSerial.Items.FindByValue(MyQueryHC_T["HSerial"].ToString());
            if (HSerial != null)
            {
                RBL_HSerial.SelectedValue = MyQueryHC_T["HSerial"].ToString();
            }
            //RBL_HBudget .SelectedValue= MyQueryHC_T["HBudget"].ToString();
            ListItem HBudget = RBL_HBudget.Items.FindByValue(MyQueryHC_T["HBudget"].ToString());
            if (HBudget != null)
            {
                RBL_HBudget.SelectedValue = MyQueryHC_T["HBudget"].ToString();
            }
            //LBox_HNRequirement.SelectedValue = MyQueryHC_T["HNRequirement"].ToString();

            ListItem HLodging = RBL_HLodging.Items.FindByValue(MyQueryHC_T["HLodging"].ToString());
            if (HLodging != null)
            {
                RBL_HLodging.SelectedValue = MyQueryHC_T["HLodging"].ToString();
            }

            string[] gHNRequirement = MyQueryHC_T["HNRequirement"].ToString().Split(',');
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

            if (DDL_HBudgetType.Items.FindByValue(MyQueryHC_T["HBudgetType"].ToString()) != null)
            {
                DDL_HBudgetType.SelectedValue = MyQueryHC_T["HBudgetType"].ToString();
            }
            //DDL_HBudgetType.SelectedValue = MyQueryHC_T["HBudgetType"].ToString();

            //MA20231030_課程報名截止日
            if (RBL_HSerial.SelectedValue == "0")
            {
                LB_HCDeadlineDayTitle.Text = "課程開始日前";
            }
            else
            {
                LB_HCDeadlineDayTitle.Text = "課程結束日後";
            }

            if (!string.IsNullOrEmpty(MyQueryHC_T["HCDeadlineDay"].ToString()))
            {
                TB_HCDeadlineDay.Text = MyQueryHC_T["HCDeadlineDay"].ToString();
            }
            else
            {
                TB_HCDeadlineDay.Text = "0";
            }

            //MA20240424_加入軸線類別
            ListItem HAxisYN = RBL_HAxisYN.Items.FindByValue(MyQueryHC_T["HAxisYN"].ToString());
            if (HAxisYN != null)
            {
                RBL_HAxisYN.SelectedValue = MyQueryHC_T["HAxisYN"].ToString();

                if (MyQueryHC_T["HAxisYN"].ToString() == "1")
                {
                    if (!string.IsNullOrEmpty(MyQueryHC_T["HAxisClass"].ToString()))
                    {
                        DDL_HAxisClass.SelectedValue = MyQueryHC_T["HAxisClass"].ToString();
                    }

                    Span_HAxisClass.Visible = true;
                    DDL_HAxisClass.Enabled = true;
                }
                else
                {
                    DDL_HAxisClass.SelectedValue = "0";
                    Span_HAxisClass.Visible = false;
                    DDL_HAxisClass.Enabled = false;
                }
            }


            //TB_HNCWSheet.Text = HttpUtility.HtmlDecode(MyQueryHC_T["HNCWSheet"].ToString());
            //TB_HNCWDay.Text = HttpUtility.HtmlDecode(MyQueryHC_T["HNCWDay"].ToString());

            //LBox_HQuestionID.SelectedValue = MyQueryHC_T["HQuestionID"].ToString();
            string[] gHQuestionID = MyQueryHC_T["HQuestionID"].ToString().Split(',');
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

            //string[] gHTMaterialID = MyQueryHC_T["HTMaterialID"].ToString().Split(',');
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

            DDL_HPMethod.SelectedValue = MyQueryHC_T["HPMethod"].ToString();
            //220819-顯示轉為金額
            if (!string.IsNullOrEmpty(MyQueryHC_T["HBCPoint"].ToString()))
            {
                TB_HBCPoint.Text = (Convert.ToInt32(MyQueryHC_T["HBCPoint"].ToString()) * 10).ToString();
            }

            RBL_HSGList.SelectedValue = MyQueryHC_T["HSGList"].ToString();
            //LBox_HIRestriction.SelectedValue = MyQueryHC_T["HIRestriction"].ToString();
            LBox_HIRestriction.DataBind();
            string[] gHIRestriction = MyQueryHC_T["HIRestriction"].ToString().Split(',');
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

            TB_HParticipantLimit.Text = MyQueryHC_T["HParticipantLimit"].ToString();

            TB_HRemark.Text = HttpUtility.HtmlDecode(MyQueryHC_T["HRemark"].ToString());
            TB_HContentTitle.Text = HttpUtility.HtmlDecode(MyQueryHC_T["HContentTitle"].ToString());
            CKE_HContent.Text = HttpUtility.HtmlDecode(MyQueryHC_T["HContent"].ToString());


        }
        MyQueryHC_T.Close();
        dbConn.Close();
        dbCmd.Cancel();



        //EE20241113_加入排序欄位；資料依排序顯示
        SDS_HCourseMaterial_T.SelectCommand = "SELECT HID, HCMName, HCMaterial, HCMLink, HSort FROM HCourseMaterial_T where HCTemplateID='" + LB_HID.Text + "' AND HStatus=1 ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
        //SDS_HCourseMaterial_T.SelectCommand = "SELECT HID, HCMName, HCMaterial, HCMLink FROM HCourseMaterial_T where HCTemplateID='" + LB_HID.Text + "' and HStatus='1'  ORDER BY case when HCMName <> '' then 0 else 1 end ASC";
        Rpt_HCourseMaterial_T.DataBind();

        SDS_HTodoList_T.SelectCommand = "SELECT HID, HGroupName, HTask, HTaskContent, HTaskNum FROM HTodoList_T where HCTemplateID='" + LB_HID.Text + "' and HStatus='1'";
        Rpt_HTodoList_T.DataBind();

        //MA20250408_講師教材
        SDS_HTMaterial_T.SelectCommand = "SELECT HID, HCTemplateID, HTMaterialID, HSort FROM HCourseTMaterial_T WHERE HCTemplateID='" + LB_HID.Text + "' AND HStatus='1' ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
        Rpt_HTMaterial_T.DataBind();

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


        ////刪除1天前未存檔的資料
        //string strDelHC_T = "delete from HCourse_T where HID='" + Del_CA + "'";
        //dbCmd = new SqlCommand(strDelHC_T, dbConn);
        //dbCmd.ExecuteNonQuery();

        //string strDelHCM_T = "delete from HCourseMaterial_T where HCTemplateID ='" + Del_CA + "'";
        //dbCmd = new SqlCommand(strDelHCM_T, dbConn);
        //dbCmd.ExecuteNonQuery();

        //string strDelHLC_T = "delete from HLeadingCourse_T where HCTemplateID ='" + Del_CA + "'";
        //dbCmd = new SqlCommand(strDelHLC_T, dbConn);
        //dbCmd.ExecuteNonQuery();

        //string strDelHTL_T = "delete from HTodoList_T where HCTemplateID ='" + Del_CA + "'";
        //dbCmd = new SqlCommand(strDelHTL_T, dbConn);
        //dbCmd.ExecuteNonQuery();


        string strUpdHC_T = "update HCourse_T set HStatus='0' where HID='" + Del_CA + "'";
        dbCmd = new SqlCommand(strUpdHC_T, dbConn);
        dbCmd.ExecuteNonQuery();

        string strUpdHCM_T = "update HCourseMaterial_T set HStatus='0' where HCTemplateID='" + Del_CA + "'";
        dbCmd = new SqlCommand(strUpdHCM_T, dbConn);
        dbCmd.ExecuteNonQuery();

        string strUpdHLC_T = "update HLeadingCourse_T set HStatus='0' where HCTemplateID='" + Del_CA + "'";
        dbCmd = new SqlCommand(strUpdHLC_T, dbConn);
        dbCmd.ExecuteNonQuery();

        string strUpdHTL_T = "update HTodoList_T set HStatus='0' where HCTemplateID='" + Del_CA + "'";
        dbCmd = new SqlCommand(strUpdHTL_T, dbConn);
        dbCmd.ExecuteNonQuery();








        dbCmd.Cancel();
        dbConn.Close();


        Response.Write("<script>alert('刪除成功!');window.location.href='HCourseTemplate_Edit.aspx';</script>");





    }
    #endregion

    #region 課程類別選單
    protected void DDL_HType_SelectedIndexChanged(object sender, EventArgs e)
    {
        SqlDataReader QueryHCourse_Class = SQLdatabase.ExecuteReader("SELECT HID, HType FROM HCourse_Class where HID='" + DDL_HType.SelectedValue + "'");
        if (QueryHCourse_Class.Read())
        {
            //RBL_TestCourse.SelectedValue = QueryHCourse_Class["HType"].ToString() == "2" ? "1" : "0";

            //MA20240421_加入檢覈科目名稱顯示判斷；檢覈課程即顯示
            if (QueryHCourse_Class["HType"].ToString() == "2")
            {
                RBL_TestCourse.SelectedValue = "1";
                Span_HExamSubject.Visible = true;
                DDL_HExamSubject.Enabled = true;
            }
            else
            {
                RBL_TestCourse.SelectedValue = "0";
                Span_HExamSubject.Visible = false;
                DDL_HExamSubject.Enabled = false;
                DDL_HExamSubject.Items.Clear();
                DDL_HExamSubject.Items.Add(new ListItem("-請選擇-", "0"));
                DDL_HExamSubject.DataBind();
            }
        }
        QueryHCourse_Class.Close();

        //活動-繳費帳戶限定基金會
        if (DDL_HType.SelectedValue == "13")
        {
            DDL_HPMethod.SelectedValue = "1";
            DDL_HPMethod.Enabled = false;
            TB_HBCPoint.Enabled = false;
            //TB_HBCPoint.Text = "50";  //50點
            TB_HBCPoint.Text = "500";  //220819-前台顯示改成金額，故改為500元
        }
        else
        {
            DDL_HPMethod.SelectedValue = "0";
            DDL_HBudgetType.SelectedValue = "0";
            DDL_HPMethod.Enabled = true;
            DDL_HBudgetType.Enabled = false;
            TB_HBCPoint.Enabled = true;
            TB_HBCPoint.Text = null;
        }
    }
    #endregion

    #region 學員課程教材
    protected void Rpt_HCourseMaterial_T_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ((FileUpload)e.Item.FindControl("FU_HCMaterial")).Visible = false;
        ((HyperLink)e.Item.FindControl("HL_HCMaterial")).Visible = true;
        ((HyperLink)e.Item.FindControl("HL_HCMaterial")).NavigateUrl = "";
        ((HyperLink)e.Item.FindControl("HL_HCMaterial")).Target = "_blank";
        ((TextBox)e.Item.FindControl("TB_HCMName")).Text = HttpUtility.HtmlDecode(((TextBox)e.Item.FindControl("TB_HCMName")).Text);
        ((TextBox)e.Item.FindControl("TB_HCMLink")).Text = HttpUtility.HtmlDecode(((TextBox)e.Item.FindControl("TB_HCMLink")).Text);



    }

    #region 學員課程教材-新增功能
    protected void LBtn_HCourseMaterial_T_add_Click(object sender, EventArgs e)
    {

        if (TB_HCMName.Text.Trim() == "")
        {
            //string RS_PeGN = (string)this.GetGlobalResourceObject("LangSource", "RS_PeGN");
            Response.Write("<script>alert('請輸入教材名稱');</script>");
            return;
        }

        //建立日期;系統時間
        //DateTime gDate = DateTime.Now;
        string gDay = DateTime.Now.Day.ToString();
        string gMonth = DateTime.Now.Month.ToString();
        string gYear = DateTime.Now.Year.ToString();
        string gDate = System.DateTime.Now.ToString("yyyy/MM/dd");

        string gHCMFileTemp = "~/uploads/CourseMaterialTemp/";
        //string gHCMFileRoot = "~/uploads/CourseMaterial/";
        string gHCMFileName = "";//主檔名
        string gHCMFileExtension = "";//副檔名

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

                bool fileIsValid = false;

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
                    //gHCMFileName = TB_HCMName.Text.Trim() + "_" + DateTime.Now.ToString("yyMMddssff") + gHCMFileExtension;
                    //FU_HCMaterial.SaveAs(Server.MapPath(gHCMFileTemp) + gHCMFileName);
                    gHCMFileName = Path.GetFileNameWithoutExtension(FU_HCMaterial.FileName) + "_" + DateTime.Now.ToString("yyMMddssff") + gHCMFileExtension;
                    //Response.Write(FU_HCMaterial.FileName);
                    //Response.End();
                    FU_HCMaterial.SaveAs(Server.MapPath(gHCMFileTemp) + gHCMFileName);
                }
                else
                {
                    Response.Write("<script>alert('只能上傳PDF或mp3檔哦~~!');</script>");
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

        //EE20241113_改為參數寫法&加入排序欄位
        dbCmd = new SqlCommand("INSERT INTO HCourseMaterial_T (HCTemplateID, HCMName, HCMaterial, HCMLink, HStatus, HCreate, HCreateDT, HSave, HSort) values (@HCTemplateID, @HCMName, @HCMaterial, @HCMLink, @HStatus, @HCreate, @HCreateDT, @HSave, @HSort)", dbConn);
        //dbConn.Open();
        dbCmd.Parameters.AddWithValue("@HCTemplateID", LB_HID.Text);
        dbCmd.Parameters.AddWithValue("@HCMName", HttpUtility.HtmlEncode(TB_HCMName.Text.Trim()));
        dbCmd.Parameters.AddWithValue("@HCMaterial", gHCMFileName);
        dbCmd.Parameters.AddWithValue("@HCMLink", HttpUtility.HtmlEncode(TB_HCMLink.Text.Trim()));
        dbCmd.Parameters.AddWithValue("@HStatus", "1");
        dbCmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        dbCmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        dbCmd.Parameters.AddWithValue("@HSave", "1");
        dbCmd.Parameters.AddWithValue("@HSort", TB_HSort.Text.Trim());

        dbCmd.ExecuteNonQuery();
        dbConn.Close();
        dbCmd.Cancel();
        //string strSelHCM_T = "Insert Into HCourseMaterial_T (HCTemplateID, HCMName, HCMaterial, HCMLink, HStatus, HCreate, HCreateDT, HSave) values ('" + LB_HID.Text + "', '" + HttpUtility.HtmlEncode(TB_HCMName.Text.Trim()) + "', '" + gHCMFileName + "', '" + HttpUtility.HtmlEncode(TB_HCMLink.Text.Trim()) + "', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','0')";
        //dbCmd = new SqlCommand(strSelHCM_T, dbConn);
        //dbCmd.ExecuteReader();

        //dbConn.Close();
        //dbCmd.Cancel();


        //EE20241113_加入排序欄位；資料依排序顯示
        SDS_HCourseMaterial_T.SelectCommand = "SELECT HID, HCMName, HCMaterial, HCMLink, HSort FROM HCourseMaterial_T where HCTemplateID='" + LB_HID.Text + "' AND HStatus=1  ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
        //SDS_HCourseMaterial_T.SelectCommand = "SELECT HID, HCMName, HCMaterial, HCMLink FROM HCourseMaterial_T where HCTemplateID='" + LB_HID.Text + "' and HStatus='1' ORDER BY case when HCMName <> '' then 0 else 1 end ASC";
        Rpt_HCourseMaterial_T.DataBind();

        //清除textbox
        TB_HCMName.Text = "";
        TB_HCMLink.Text = "";
        TB_HSort.Text = "";
    }
    #endregion

    #region 學員課程教材-刪除功能
    protected void LBtn_HCourseMaterial_T_Del_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Del = sender as LinkButton;
        string Del_CA = LBtn_Del.CommandArgument;

        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        //刪除1天前未存檔的資料
        string strDelHC_T = "delete from HCourseMaterial_T where HID='" + Del_CA + "'";
        dbCmd = new SqlCommand(strDelHC_T, dbConn);
        dbCmd.ExecuteNonQuery();

        dbConn.Close();
        dbCmd.Cancel();

        //EE20241113_加入排序欄位；資料依排序顯示
        SDS_HCourseMaterial_T.SelectCommand = "SELECT HID, HCMName, HCMaterial, HCMLink, HSort FROM HCourseMaterial_T where HCTemplateID='" + LB_HID.Text + "' AND HStatus=1 ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
        //SDS_HCourseMaterial_T.SelectCommand = "SELECT HID, HCMName, HCMaterial, HCMLink FROM HCourseMaterial_T where HCTemplateID='" + LB_HID.Text + "' and HStatus='1'  ORDER BY case when HCMName <> '' then 0 else 1 end ASC";
        Rpt_HCourseMaterial_T.DataBind();
    }
    #endregion

    //protected void LBtn_HCourseMaterial_T_Click(object sender, EventArgs e)
    //{
    //    Panel_HCourseMaterial_T.Visible = (Panel_HCourseMaterial_T.Visible == false ? true : false);
    //    Panel_HLeadingCourse_T.Visible = false;
    //    Panel_HTodoList_T.Visible = false;
    //}
    //protected void LBtn_HLeadingCourse_T_Click(object sender, EventArgs e)
    //{
    //    Panel_HCourseMaterial_T.Visible = false;
    //    Panel_HLeadingCourse_T.Visible = (Panel_HLeadingCourse_T.Visible == false ? true : false);
    //    Panel_HTodoList_T.Visible = false;
    //}
    //protected void LBtn_HTodoList_T_Click(object sender, EventArgs e)
    //{
    //    Panel_HCourseMaterial_T.Visible = false;
    //    Panel_HLeadingCourse_T.Visible = false;
    //    Panel_HTodoList_T.Visible = (Panel_HTodoList_T.Visible == false ? true : false);
    //}
    //protected void LBtn_HCourseContent_T_Click(object sender, EventArgs e)
    //{

    //}

    #endregion

    #region 體系護持工作項目

    protected void Rpt_HTodoList_T_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //取得DataRowView
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

        ((TextBox)e.Item.FindControl("TB_HTaskContent")).Text = HttpUtility.HtmlDecode(((TextBox)e.Item.FindControl("TB_HTaskContent")).Text);

        ((TextBox)e.Item.FindControl("TB_HTaskNum")).Text = gDRV["HTaskNum"].ToString();
    }


    #region 體系護持工作項目-新增功能
    protected void LBtn_HTodoList_T_add_Click(object sender, EventArgs e)
    {

        if (DDL_HGroupName.SelectedValue == "0")
        {
            Response.Write("<script>alert('請選擇所屬組別!');</script>");
            return;
        }


        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        //EE20240503_加入人數
        string strSelHTL_T = "insert into HTodoList_T (HCTemplateID, HGroupName, HTask, HTaskNum, HTaskContent, HGroupLeaderID, HStatus, HCreate, HCreateDT, HSave) values ('" + LB_HID.Text + "', '" + DDL_HGroupName.SelectedValue + "', '" + DDL_HTask.SelectedValue + "', '" + TB_HTaskNum.Text.Trim() + "', '" + HttpUtility.HtmlEncode(TB_HTaskContent.Text.Trim()) + "','0', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '1')";
        dbCmd = new SqlCommand(strSelHTL_T, dbConn);
        dbCmd.ExecuteReader();

        dbConn.Close();
        dbCmd.Cancel();

        //清空資料
        DDL_HGroupName.SelectedValue = "0";
        DDL_HTask.SelectedValue = "0";
        TB_HTaskNum.Text = null;
        TB_HTaskContent.Text = null;

        SDS_HTodoList_T.SelectCommand = "SELECT HID, HGroupName, HTask, HTaskContent, HTaskNum FROM HTodoList_T where HCTemplateID='" + LB_HID.Text + "' and HStatus='1'";
        Rpt_HTodoList_T.DataBind();
    }
    #endregion

    #region 體系護持工作項目-刪除功能
    protected void LBtn_HTodoList_T_Del_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Del = sender as LinkButton;
        string Del_CA = LBtn_Del.CommandArgument;

        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        //刪除1天前未存檔的資料
        string strDelHC_T = "delete from HTodoList_T where HID='" + Del_CA + "'";
        dbCmd = new SqlCommand(strDelHC_T, dbConn);
        dbCmd.ExecuteNonQuery();

        dbConn.Close();
        dbCmd.Cancel();

        SDS_HTodoList_T.SelectCommand = "SELECT HID, HGroupName, HTask, HTaskContent , HTaskNum FROM HTodoList_T where HCTemplateID='" + LB_HID.Text + "' and HStatus='1'";
        Rpt_HTodoList_T.DataBind();
    }
    #endregion

    #region 所屬組別
    protected void DDL_HGroupName_SelectedIndexChanged(object sender, EventArgs e)
    {
        DDL_HTask.Items.Clear();
        DDL_HTask.Enabled = true;
        DDL_HTask.Items.Add(new ListItem("-請選擇-", "0"));
        SqlDataReader QueryHCourseTask = SQLdatabase.ExecuteReader("SELECT HID, HTask, HTaskContent FROM HCourseTask where HGroupID='" + DDL_HGroupName.SelectedValue + "'");
        while (QueryHCourseTask.Read())
        {
            DDL_HTask.Items.Add(new ListItem(QueryHCourseTask["HTask"].ToString(), (QueryHCourseTask["HID"].ToString())));
        }
        QueryHCourseTask.Close();
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
    }
    #endregion

    #endregion

    #region 講師教材設定

    #region 講師教材-新增功能
    protected void LBtn_HTMaterial_add_Click(object sender, EventArgs e)
    {

        if (DDL_HTMaterial.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('請選擇講師教材名稱!');", true);
            //Response.Write("<script>alert('請選擇講師教材名稱!');</script>");
            return;
        }

        #region 判斷是否重複

        SqlCommand read = new SqlCommand("SELECT HID FROM  HCourseTMaterial_T WHERE HCTemplateID=@HCTemplateID AND HTMaterialID=@HTMaterialID AND HStatus='1'", con);
        con.Open();
        read.Parameters.AddWithValue("@HCTemplateID", LB_HID.Text);
        read.Parameters.AddWithValue("@HTMaterialID", DDL_HTMaterial.SelectedValue);

        SqlDataReader MyEBF = read.ExecuteReader();

        if (MyEBF.Read())
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('資料已重複囉!');", true);
            return;
        }
        MyEBF.Close();
        con.Close();

        #endregion

        SqlCommand cmd = new SqlCommand("INSERT INTO [HCourseTMaterial_T] (HCTemplateID, HTMaterialID, HSort, HTMaterial, HSave, HStatus, HCreate, HCreateDT)VALUES(@HCTemplateID, @HTMaterialID, @HSort, @HTMaterial, @HSave, @HStatus, @HCreate, @HCreateDT)", con);
        con.Open();
        cmd.Parameters.AddWithValue("@HCTemplateID", LB_HID.Text);
        cmd.Parameters.AddWithValue("@HTMaterialID", DDL_HTMaterial.SelectedValue);
        cmd.Parameters.AddWithValue("@HSort", TB_HTMaterialSort.Text.Trim());
        cmd.Parameters.AddWithValue("@HTMaterial", DDL_HTMaterial.SelectedItem.Text);
        cmd.Parameters.AddWithValue("@HStatus", "1");
        cmd.Parameters.AddWithValue("@HSave", "1");
        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        con.Close();
        cmd.Cancel();


        DDL_HTMaterial.SelectedValue = "0";
        TB_HTMaterialSort.Text = null;

        SDS_HTMaterial_T.SelectCommand = "SELECT HID, HCTemplateID, HTMaterialID, HSort FROM HCourseTMaterial_T WHERE HCTemplateID='" + LB_HID.Text + "'  AND HStatus='1' ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
        Rpt_HTMaterial_T.DataBind();


    }
    #endregion

    protected void Rpt_HTMaterial_T_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            DataRowView gDRV = (DataRowView)e.Item.DataItem;
            DropDownList ddl = (DropDownList)e.Item.FindControl("DDL_HTMaterial");
            ddl.Items.Clear();
            using (SqlDataReader QueryHTMaterial = SQLdatabase.ExecuteReader(
            "SELECT a.HID, (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) AS HTMName " +
            "FROM HTeacherMaterial AS a " +
            "LEFT JOIN HDocCTypeClass AS b ON a.HDocCTClassID=b.HID " +
            "LEFT JOIN HCourseLevel AS c ON a.HCourseLevelID=c.HID " +
            "LEFT JOIN HFileType AS d ON a.HFileTypeID=d.HID " +
            "WHERE a.HStatus='1' AND (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) <>'' " +
            "AND (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) IS NOT NULL " +
            "ORDER BY a.HStatus DESC"))
            {
                while (QueryHTMaterial.Read())
                {
                    ddl.Items.Add(new ListItem(
                        QueryHTMaterial["HTMName"].ToString(),
                        QueryHTMaterial["HID"].ToString()
                    ));
                }
            }

            // 設定預設選取值
            string selectedID = gDRV["HTMaterialID"] != DBNull.Value ? gDRV["HTMaterialID"].ToString() : null;
            if (!string.IsNullOrEmpty(selectedID) && ddl.Items.FindByValue(selectedID) != null)
            {
                ddl.SelectedValue = selectedID;
            }
            else
            {
                // 避免找不到值時出錯，可選擇加個預設項目：
                ddl.Items.Insert(0, new ListItem("-請選擇-", "0"));
                ddl.SelectedValue = "0";
            }
        }


    }

    #region 講師教材-刪除功能
    protected void LBtn_HTMaterial_Del_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Del = sender as LinkButton;
        string Del_CA = LBtn_Del.CommandArgument;

        SqlCommand cmd = new SqlCommand("DELETE FROM HCourseTMaterial_T WHERE HID =@HID", con);
        con.Open();
        cmd.Parameters.AddWithValue("@HID", Del_CA);
        cmd.ExecuteNonQuery();
        con.Close();
        cmd.Cancel();

        SDS_HTMaterial_T.SelectCommand = "SELECT HID, HCTemplateID, HTMaterialID, HSort FROM HCourseTMaterial_T WHERE HCTemplateID='" + LB_HID.Text + "'  AND HStatus='1' ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
        Rpt_HTMaterial_T.DataBind();
    }
    #endregion

    #endregion

    #region 儲存功能
    protected void Btn_Submit_Click(object sender, EventArgs e)
    {
        //220819 加入必填判斷
        if (DDL_HPMethod.SelectedValue == "0")
        {
            Response.Write("<script>alert('請選擇繳費帳戶~');</script>");
            return;
        }
        else
        {
            if (DDL_HPMethod.SelectedValue == "1")
            {
                if (DDL_HBudgetType.SelectedValue == "0")
                {
                    Response.Write("<script>alert('請選擇預算類別~');</script>");
                    return;
                }
            }
        }

        //220819 加入必填判斷
        if (string.IsNullOrEmpty(TB_HBCPoint.Text.Trim()))
        {
            Response.Write("<script>alert('請輸入參與課程基本金額');</script>");
            return;
        }


        if (DDL_HType.SelectedValue == "0")
        {
            Response.Write("<script>alert('請選擇課程類別!');</script>");
            return;
        }

        if (string.IsNullOrEmpty(TB_HTemplateName.Text.Trim()))
        {
            Response.Write("<script>alert('請輸入課程範本名稱~');</script>");
            return;
        }

      

        int gCT = 1;
        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        #region EE20241113_註解：教材檔案
        //將上傳之教材暫存檔名改為正式檔名
        string gHCMFileTemp = "~/uploads/CourseMaterialTemp/";
        string gHCMFileRoot = "~/uploads/CourseMaterial/";
        string gHCMFileName = "";//主檔名
        //string gHCMFileExtension = "";//副檔名

        string strSelHCM_T = "select HCMaterial from HCourseMaterial_T where HCTemplateID = '" + LB_HID.Text + "' and HSave='1'";
        dbCmd = new SqlCommand(strSelHCM_T, dbConn);
        SqlDataReader MyQueryHCM_T = dbCmd.ExecuteReader();

        while (MyQueryHCM_T.Read())
        {
            if (!string.IsNullOrEmpty(MyQueryHCM_T["HCMaterial"].ToString()))
            {
                //主檔名+副檔名
                gHCMFileName = MyQueryHCM_T["HCMaterial"].ToString();

                //取得檔名
                string[] Filename = gHCMFileName.Split('.');
                //string New_name = Filename[0].ToString() + "_" + DateTime.Now.ToString("yyMMdd") + "." + Filename[1].ToString();  //避免名稱重複 資料夾已有檔案會error
                //將上傳之教材從暫存區移動到正式區
                //System.IO.File.Move(Server.MapPath(gHCMFileTemp) + gHCMFileName, Server.MapPath(gHCMFileRoot) + New_name);
                if (System.IO.File.Exists(Server.MapPath(gHCMFileTemp) + gHCMFileName))
                {
                    System.IO.File.Move(Server.MapPath(gHCMFileTemp) + gHCMFileName, Server.MapPath(gHCMFileRoot) + gHCMFileName);
                }
            }
        }
        MyQueryHCM_T.Close();
        #endregion

  
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


        string gLBox_HIRestriction = "";
        foreach (ListItem LBoxHIRestriction in LBox_HIRestriction.Items)
        {
            if (LBoxHIRestriction.Selected == true)
            {
                gLBox_HIRestriction += LBoxHIRestriction.Value + ",";
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
  

        //220819-基本費用(元)存入DB要先轉換成點數概念(1元=10點)
        int BasicPrice = Convert.ToInt32(TB_HBCPoint.Text.Trim()) / 10;

    

        string strUpdHC_T = "EXECUTE [sp_HCourseTemplate] @StatementType, @HID, @HTemplateName  ,@HType  ,@HOSystem  ,@HRSystem  ,@HNLCourse  ,@HNGuide  ,@HNFull  ,@HNRequirement  ,@HTeam  ,@HNCWSheet  ,@HNCWDay  ,@HQuestionID  ,@HPMethod  ,@HBCPoint  ,@HSGList  ,@HIRestriction  ,@HRemark  ,@HContentTitle  ,@HContent  ,@HStatus  ,@HCMID  ,@HCMDT  ,@HSave  ,@HSerial  ,@HBudget  ,@HBudgetType  ,@HLodging  ,@HTMaterialID  ,@HCDeadlineDay  ,@HAxisYN  ,@HAxisClass ,@HExamSubject, @HExamContentID, @HParticipantLimit, @HBookByDateYN, @HCCPeriodYN";
        SqlCommand cmd = new SqlCommand(strUpdHC_T, con);
        con.Open();
        cmd.Parameters.AddWithValue("@StatementType", "Update");
        cmd.Parameters.AddWithValue("@HID", LB_HID.Text);
        cmd.Parameters.AddWithValue("@HTemplateName", TB_HTemplateName.Text.Trim());
        cmd.Parameters.AddWithValue("@HType", DDL_HType.SelectedValue);
        cmd.Parameters.AddWithValue("@HOSystem", DDL_HOSystem.SelectedValue);
        cmd.Parameters.AddWithValue("@HRSystem", gLBox_HRSystem);
        cmd.Parameters.AddWithValue("@HNLCourse", RBL_HNLCourse.SelectedValue);
        cmd.Parameters.AddWithValue("@HNGuide", RBL_HNGuide.SelectedValue);
        cmd.Parameters.AddWithValue("@HNFull", RBL_HNFull.SelectedValue);
        cmd.Parameters.AddWithValue("@HNRequirement", gLBox_HNRequirement);
        cmd.Parameters.AddWithValue("@HTeam", "");
        cmd.Parameters.AddWithValue("@HNCWSheet", HttpUtility.HtmlEncode(TB_HNCWSheet.Text.Trim()));
        cmd.Parameters.AddWithValue("@HNCWDay", HttpUtility.HtmlEncode(TB_HNCWDay.Text.Trim()));
        cmd.Parameters.AddWithValue("@HQuestionID", gLBox_HQuestionID);
        cmd.Parameters.AddWithValue("@HPMethod", DDL_HPMethod.SelectedValue);
        cmd.Parameters.AddWithValue("@HBCPoint", BasicPrice.ToString());
        cmd.Parameters.AddWithValue("@HSGList", RBL_HSGList.SelectedValue);
        cmd.Parameters.AddWithValue("@HIRestriction", gLBox_HIRestriction);
        cmd.Parameters.AddWithValue("@HRemark", HttpUtility.HtmlEncode(TB_HRemark.Text.Trim()));
        cmd.Parameters.AddWithValue("@HContentTitle", HttpUtility.HtmlEncode(TB_HContentTitle.Text.Trim()));
        cmd.Parameters.AddWithValue("@HContent", HttpUtility.HtmlEncode(CKE_HContent.Text));
        cmd.Parameters.AddWithValue("@HStatus", "1");
        cmd.Parameters.AddWithValue("@HCMID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCMDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("@HSave", "1");
        cmd.Parameters.AddWithValue("@HSerial", RBL_HSerial.SelectedValue);
        cmd.Parameters.AddWithValue("@HBudget", RBL_HBudget.SelectedValue);
        cmd.Parameters.AddWithValue("@HBudgetType", DDL_HBudgetType.SelectedValue);
        cmd.Parameters.AddWithValue("@HLodging", RBL_HLodging.SelectedValue);
        cmd.Parameters.AddWithValue("@HTMaterialID", gLBox_HTMaterialID);
        cmd.Parameters.AddWithValue("@HCDeadlineDay", TB_HCDeadlineDay.Text.Trim());
        cmd.Parameters.AddWithValue("@HAxisYN", RBL_HAxisYN.SelectedValue);
        cmd.Parameters.AddWithValue("@HAxisClass", DDL_HAxisClass.SelectedValue);
        cmd.Parameters.AddWithValue("@HExamSubject", DDL_HExamSubject.SelectedValue);
        cmd.Parameters.AddWithValue("@HExamContentID", DDL_HExamContentID.SelectedValue);
        cmd.Parameters.AddWithValue("@HParticipantLimit", TB_HParticipantLimit.Text.Trim());
        cmd.Parameters.AddWithValue("@HBookByDateYN", RBL_HBookByDateYN.SelectedValue);
        cmd.Parameters.AddWithValue("@HCCPeriodYN", RBL_HCCPeriodYN.SelectedValue);

        cmd.ExecuteReader();
        con.Close();
        cmd.Cancel();


        for (int i = 0; i < Rpt_HCourseMaterial_T.Items.Count; i++)
        {
            dbCmd = new SqlCommand("UPDATE HCourseMaterial_T SET HCTemplateID=@HCTemplateID, HCMName=@HCMName, HCMLink=@HCMLink, HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT, HSave=@HSave, HSort=@HSort WHERE HCTemplateID =@HCTemplateID AND HID=@HID", dbConn);

            dbCmd.Parameters.AddWithValue("@HCTemplateID", LB_HID.Text);
            dbCmd.Parameters.AddWithValue("@HID", ((Label)Rpt_HCourseMaterial_T.Items[i].FindControl("LB_HID")).Text);
            dbCmd.Parameters.AddWithValue("@HCMName", ((TextBox)Rpt_HCourseMaterial_T.Items[i].FindControl("TB_HCMName")).Text.Trim());
            dbCmd.Parameters.AddWithValue("@HCMLink", ((TextBox)Rpt_HCourseMaterial_T.Items[i].FindControl("TB_HCMLink")).Text.Trim());
            dbCmd.Parameters.AddWithValue("@HStatus", "1");
            dbCmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            dbCmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            dbCmd.Parameters.AddWithValue("@HSave", "1");
            //EE20241113_改為參數寫法&加入排序欄位
            if (((TextBox)Rpt_HCourseMaterial_T.Items[i].FindControl("TB_HSort")).Text.Trim() != ((Label)Rpt_HCourseMaterial_T.Items[i].FindControl("LB_HSort")).Text.Trim())
            {
                //dbConn.Open();

                dbCmd.Parameters.AddWithValue("@HSort", ((TextBox)Rpt_HCourseMaterial_T.Items[i].FindControl("TB_HSort")).Text.Trim());
            }
            else
            {
                dbCmd.Parameters.AddWithValue("@HSort", ((Label)Rpt_HCourseMaterial_T.Items[i].FindControl("LB_HSort")).Text.Trim());
            }
            dbCmd.ExecuteNonQuery();

          
        }



        string strUpdHLC_T = "update HLeadingCourse_T set HStatus='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1' where HCTemplateID = '" + LB_HID.Text + "'";
        dbCmd = new SqlCommand(strUpdHLC_T, dbConn);
        dbCmd.ExecuteReader();

        string strUpdHTL_T = "update HTodoList_T set HStatus='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1' where HCTemplateID = '" + LB_HID.Text + "'";
        dbCmd = new SqlCommand(strUpdHTL_T, dbConn);
        dbCmd.ExecuteReader();

        //MA20250517_講師教材設定
        for (int i = 0; i < Rpt_HTMaterial_T.Items.Count; i++)
        {
            if (((TextBox)Rpt_HTMaterial_T.Items[i].FindControl("TB_HTMaterialSort")).Text.Trim() != ((Label)Rpt_HTMaterial_T.Items[i].FindControl("LB_HTMaterialSort")).Text.Trim())
            {
                //SQL開始
                dbCmd = new SqlCommand("UPDATE HCourseTMaterial_T SET HSort=@HSort, HModify=@HModify,HModifyDT=@HModifyDT WHERE  HID =@HID ", dbConn);
                dbCmd.Parameters.AddWithValue("@HID", ((Label)Rpt_HTMaterial_T.Items[i].FindControl("LB_HID")).Text.Trim());
                dbCmd.Parameters.AddWithValue("@HSort", ((TextBox)Rpt_HTMaterial_T.Items[i].FindControl("TB_HTMaterialSort")).Text.Trim());
                dbCmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                dbCmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                dbCmd.ExecuteNonQuery();
                dbCmd.Cancel();
            }
        }



        dbConn.Close();
        dbCmd.Cancel();

        Response.Write("<script>alert('存檔成功!');window.location.href='HCourseTemplate_Edit.aspx';</script>");
    }
    #endregion

    #region 取消儲存功能
    protected void Btn_Cancel_Click(object sender, EventArgs e)
    {
        Response.Write("<script>window.location.href='HCourseTemplate_Edit.aspx';</script>");
    }
    #endregion

    #region Tab切換
    protected void LBtn_NavTab_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        LB_NavTab.Text = btn.TabIndex.ToString();

        Panel[] Tabstr = { Panel_Template, Panel_Content, Panel_Material, Panel_Jobs, Panel_Homework, Panel_HTMaterial };

        for (int k = 0; k < Tabstr.Length; k++)
        {
            Tabstr[k].Attributes.Add("class", "tab-pane fade");

            if (k == btn.TabIndex - 1)
            {
                Tabstr[k].Attributes.Add("class", "tab-pane fade active show");
            }

        }

    }

    #endregion

    #region 繳費帳戶選單
    protected void DDL_HPMethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DDL_HPMethod.SelectedValue == "1")
        {
            DDL_HBudgetType.Enabled = true;
            Required.Visible = true;

        }
        else
        {
            DDL_HBudgetType.Enabled = false;
            Required.Visible = false;
            DDL_HBudgetType.SelectedValue = "0";
        }
    }
    #endregion

    #region 執行js
    public void RegScript()
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$('.js-example-basic-single').select2({closeOnSelect: true,});"), true);//執行js


        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS3", ("$('.ListB_Multi').SumoSelect({ search: true, placeholder: '-請選擇-', csvDispCount: 5, });"), true);//執行js

    }
    #endregion

    #region 帶狀課程選單
    //MA20231030_課程報名截止日天數顯示判斷：帶狀1-課程結束日後N天；非帶狀0-課程開始日前N天
    protected void RBL_HSerial_SelectedIndexChanged(object sender, EventArgs e)
    {
        //0：非帶狀；1：帶狀
        if (RBL_HSerial.SelectedValue == "0")
        {
            LB_HCDeadlineDayTitle.Text = "課程開始日前";
        }
        else
        {
            LB_HCDeadlineDayTitle.Text = "課程結束日後";
        }
    }
    #endregion

    #region 是否為軸線課程
    protected void RBL_HAxisYN_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (RBL_HAxisYN.SelectedValue == "0")
        {
            DDL_HAxisClass.Enabled = false;
            Span_HAxisClass.Visible = false;
            DDL_HAxisClass.SelectedValue = "0";
        }
        else if (RBL_HAxisYN.SelectedValue == "1")
        {
            DDL_HAxisClass.Enabled = true;
            Span_HAxisClass.Visible = true;
        }
    }
    #endregion

    #region 儲存Function
    private void SubmitFun(string From, string StatementType)
    {
        #region 必填判斷

        if (DDL_HType.SelectedValue == "0")
        {
            Response.Write("<script>alert('請選擇課程類別!');</script>");
            return;
        }

        if (string.IsNullOrEmpty(TB_HTemplateName.Text.Trim()))
        {
            Response.Write("<script>alert('請輸入課程範本名稱!');</script>");
            return;
        }

        //220819 加入必填判斷
        if (DDL_HPMethod.SelectedValue == "0")
        {
            Response.Write("<script>alert('請選擇繳費帳戶!');</script>");
            return;
        }
        else
        {
            if (DDL_HPMethod.SelectedValue == "1")
            {
                if (DDL_HBudgetType.SelectedValue == "0")
                {
                    Response.Write("<script>alert('請選擇預算類別!');</script>");
                    return;
                }
            }
        }

        //220819 加入必填判斷
        if (string.IsNullOrEmpty(TB_HBCPoint.Text.Trim()))
        {
            Response.Write("<script>alert('請輸入參與課程基本金額!');</script>");
            return;
        }

        //MA20240421_加入檢覈科目名稱；如為檢覈課程即為必填
        //WE250210_先註解
        //if (RBL_TestCourse.SelectedValue == "1" && DDL_HExamSubject.SelectedValue == "0")
        //{
        //    Response.Write("<script>alert('請選擇檢覈科目名稱!');</script>");
        //    return;
        //}

        ////MA20240421_加入是否為軸線；如為軸線課程時軸線類別即為必填
        /// //WE250210_先註解
        //if (RBL_HAxisYN.SelectedValue == "1" && DDL_HAxisClass.SelectedValue == "0")
        //{
        //    Response.Write("<script>alert('請選擇軸線類別!');</script>");
        //    return;
        //}

        #endregion

        #region 教材檔案
        int gCT = 1;

        //將上傳之教材暫存檔名改為正式檔名
        string gHCMFileTemp = "~/uploads/CourseMaterialTemp/";
        string gHCMFileRoot = "~/uploads/CourseMaterial/";
        string gHCMFileName = "";//主檔名
        //string gHCMFileExtension = "";//副檔名

        string strSelHCM_T = "select HCMaterial from HCourseMaterial_T where HCTemplateID = '" + LB_HID.Text + "' and HSave='1'";
        SqlDataReader MyQueryHCM_T = SQLdatabase.ExecuteReader(strSelHCM_T);

        while (MyQueryHCM_T.Read())
        {
            if (!string.IsNullOrEmpty(MyQueryHCM_T["HCMaterial"].ToString()))
                if (!string.IsNullOrEmpty(MyQueryHCM_T["HCMaterial"].ToString()))
                {
                    //主檔名+副檔名
                    gHCMFileName = MyQueryHCM_T["HCMaterial"].ToString();

                    //取得檔名
                    string[] Filename = gHCMFileName.Split('.');
                    //string New_name = Filename[0].ToString() + "_" + DateTime.Now.ToString("yyMMdd") + "." + Filename[1].ToString();  //避免名稱重複 資料夾已有檔案會error
                    //將上傳之教材從暫存區移動到正式區
                    //System.IO.File.Move(Server.MapPath(gHCMFileTemp) + gHCMFileName, Server.MapPath(gHCMFileRoot) + New_name);
                    if (System.IO.File.Exists(Server.MapPath(gHCMFileTemp) + gHCMFileName))
                    {
                        System.IO.File.Move(Server.MapPath(gHCMFileTemp) + gHCMFileName, Server.MapPath(gHCMFileRoot) + gHCMFileName);
                    }
                }
        }
        MyQueryHCM_T.Close();

        #endregion

        #region 取ListBox的值，使用ForEach方式
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



        string gLBox_HIRestriction = "";
        foreach (ListItem LBoxHIRestriction in LBox_HIRestriction.Items)
        {
            if (LBoxHIRestriction.Selected == true)
            {
                gLBox_HIRestriction += LBoxHIRestriction.Value + ",";
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

        #endregion

        string HID;
        //220819-基本費用(元)存入DB要先轉換成點數概念(1元=10點)
        int BasicPrice = 0;
        //int BasicPrice = Convert.ToInt32(TB_HBCPoint.Text.Trim()) / 10;
        if (!string.IsNullOrEmpty(TB_HBCPoint.Text.Trim()))
        {
            BasicPrice = Convert.ToInt32(TB_HBCPoint.Text.Trim()) / 10;
        }
        string strUpdHC_T = "EXECUTE [sp_HCourseTemplate] @StatementType, @HID, @HTemplateName  ,@HType  ,@HOSystem  ,@HRSystem  ,@HNLCourse  ,@HNGuide  ,@HNFull  ,@HNRequirement  ,@HTeam  ,@HNCWSheet  ,@HNCWDay  ,@HQuestionID  ,@HPMethod  ,@HBCPoint  ,@HSGList  ,@HIRestriction  ,@HRemark  ,@HContentTitle  ,@HContent  ,@HStatus  ,@HCMID  ,@HCMDT  ,@HSave  ,@HSerial  ,@HBudget  ,@HBudgetType  ,@HLodging  ,@HTMaterialID  ,@HCDeadlineDay  ,@HAxisYN  ,@HAxisClass ,@HExamSubject";
        SqlCommand cmd = new SqlCommand(strUpdHC_T, con);
        con.Open();
        cmd.Parameters.AddWithValue("@StatementType", StatementType);
        //cmd.Parameters.AddWithValue("@StatementType", "Update");
        cmd.Parameters.AddWithValue("@HID", LB_HID.Text);
        cmd.Parameters.AddWithValue("@HTemplateName", TB_HTemplateName.Text.Trim());
        cmd.Parameters.AddWithValue("@HType", DDL_HType.SelectedValue);
        cmd.Parameters.AddWithValue("@HOSystem", DDL_HOSystem.SelectedValue);
        cmd.Parameters.AddWithValue("@HRSystem", gLBox_HRSystem);
        cmd.Parameters.AddWithValue("@HNLCourse", RBL_HNLCourse.SelectedValue);
        cmd.Parameters.AddWithValue("@HNGuide", RBL_HNGuide.SelectedValue);
        cmd.Parameters.AddWithValue("@HNFull", RBL_HNFull.SelectedValue);
        cmd.Parameters.AddWithValue("@HNRequirement", gLBox_HNRequirement);
        cmd.Parameters.AddWithValue("@HTeam", "");
        cmd.Parameters.AddWithValue("@HNCWSheet", HttpUtility.HtmlEncode(TB_HNCWSheet.Text.Trim()));
        cmd.Parameters.AddWithValue("@HNCWDay", HttpUtility.HtmlEncode(TB_HNCWDay.Text.Trim()));
        cmd.Parameters.AddWithValue("@HQuestionID", gLBox_HQuestionID);
        cmd.Parameters.AddWithValue("@HPMethod", DDL_HPMethod.SelectedValue);
        cmd.Parameters.AddWithValue("@HBCPoint", BasicPrice.ToString());
        cmd.Parameters.AddWithValue("@HSGList", RBL_HSGList.SelectedValue);
        cmd.Parameters.AddWithValue("@HIRestriction", gLBox_HIRestriction);
        cmd.Parameters.AddWithValue("@HRemark", HttpUtility.HtmlEncode(TB_HRemark.Text.Trim()));
        cmd.Parameters.AddWithValue("@HContentTitle", HttpUtility.HtmlEncode(TB_HContentTitle.Text.Trim()));
        cmd.Parameters.AddWithValue("@HContent", HttpUtility.HtmlEncode(CKE_HContent.Text));
        cmd.Parameters.AddWithValue("@HStatus", "1");
        cmd.Parameters.AddWithValue("@HCMID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCMDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("@HSave", "1");
        cmd.Parameters.AddWithValue("@HSerial", RBL_HSerial.SelectedValue);
        cmd.Parameters.AddWithValue("@HBudget", RBL_HBudget.SelectedValue);
        cmd.Parameters.AddWithValue("@HBudgetType", DDL_HBudgetType.SelectedValue);
        cmd.Parameters.AddWithValue("@HLodging", RBL_HLodging.SelectedValue);
        cmd.Parameters.AddWithValue("@HTMaterialID", gLBox_HTMaterialID);
        cmd.Parameters.AddWithValue("@HCDeadlineDay", TB_HCDeadlineDay.Text.Trim());
        cmd.Parameters.AddWithValue("@HAxisYN", RBL_HAxisYN.SelectedValue);
        cmd.Parameters.AddWithValue("@HAxisClass", DDL_HAxisClass.SelectedValue);
        cmd.Parameters.AddWithValue("@HExamSubject", DDL_HExamSubject.SelectedValue);

        if (StatementType == "Insert")
        {
            HID = cmd.ExecuteScalar().ToString();

            LB_HID.Text = HID;
        }
        else
        {
            cmd.ExecuteReader();
        }
        con.Close();
        cmd.Cancel();


        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        for (int i = 0; i < Rpt_HCourseMaterial_T.Items.Count; i++)
        {
            string strUpdHCM_T = "update HCourseMaterial_T set  HCMName='" + ((TextBox)Rpt_HCourseMaterial_T.Items[i].FindControl("TB_HCMName")).Text + "', HCMLink='" + ((TextBox)Rpt_HCourseMaterial_T.Items[i].FindControl("TB_HCMLink")).Text + "', HStatus='1', HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HCreateDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1' where HCTemplateID = '" + LB_HID.Text + "' AND HID='" + ((Label)Rpt_HCourseMaterial_T.Items[i].FindControl("LB_HID")).Text + "'";
            dbCmd = new SqlCommand(strUpdHCM_T, dbConn);
            dbCmd.ExecuteReader();

        }

        string strUpdHLC_T = "update HLeadingCourse_T set HStatus='1', HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HCreateDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1' where HCTemplateID = '" + LB_HID.Text + "'";
        dbCmd = new SqlCommand(strUpdHLC_T, dbConn);
        dbCmd.ExecuteReader();

        string strUpdHTL_T = "update HTodoList_T set HStatus='1', HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HCreateDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1' where HCTemplateID = '" + LB_HID.Text + "'";
        dbCmd = new SqlCommand(strUpdHTL_T, dbConn);
        dbCmd.ExecuteReader();


        dbConn.Close();
        dbCmd.Cancel();

        if (From == "submit")
        {
            Response.Write("<script>alert('存檔成功!');window.location.href='HCourseTemplate_Edit.aspx';</script>");
        }

    }

    #endregion

}