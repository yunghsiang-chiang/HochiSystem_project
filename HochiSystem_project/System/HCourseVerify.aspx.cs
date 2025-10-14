using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class HCourseVerify : System.Web.UI.Page
{

    #region 分頁copy-1
    private int PageMax = 15;   //分頁最大顯示數量
    private int LastPage = 10;  //分頁數量
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
            ViewState["Search"] = "";

            //KE20220930-排除已刪除的課程
            //KE20240508_先預設抓3個月
            SDS_HCourse.SelectCommand = "SELECT HCourseName, HPlaceName, HStatus, HVerifyStatus, Applicant, HID, HApplyTime, HVerifyTime FROM HCourseVerifyList WHERE HStatus='1' AND HVerifyStatus <> '99'  AND HSave = '1'  AND HApplyTime BETWEEN DATEADD(month, -3, GETDATE()) AND  GETDATE() ORDER BY CASE WHEN HVerifyStatus=2 THEN 1 WHEN HVerifyStatus=1 THEN 2 ELSE 0 END ASC, HApplyTime DESC";

            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCourse.SelectCommand, PageMax, LastPage, false, Rpt_HCourse);
            ViewState["Search"] = SDS_HCourse.SelectCommand;
        }
        else
        {
            SDS_HCourse.SelectCommand = ViewState["Search"].ToString();
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", ViewState["Search"].ToString(), PageMax, LastPage, false, Rpt_HCourse);
        }

        #endregion

        #region 初始化(下拉選單)
        if (!IsPostBack)
        {
            //KE20220713-加入上課地點
            string strSelHC_Place = "SELECT HID, HPlaceName FROM HPlace ORDER BY HID ASC";
            SqlDataReader MyQueryHC_Place = SQLdatabase.ExecuteReader(strSelHC_Place);
            DDL_HOCPlace.Items.Add(new ListItem("請選擇上課地點", "0"));
            while (MyQueryHC_Place.Read())
            {
                DDL_HOCPlace.Items.Add(new ListItem(MyQueryHC_Place["HPlaceName"].ToString(), (MyQueryHC_Place["HID"].ToString())));
            }
            MyQueryHC_Place.Close();
        }
        #endregion
    }


    #region 搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(TB_Search.Text.Trim()))
        {
            if (DDL_HOCPlace.SelectedValue != "0")
            {
                SDS_HCourse.SelectCommand = "SELECT HCourseName, HPlaceName, HStatus, HVerifyStatus, Applicant, HID, HApplyTime, HVerifyTime FROM HCourseVerifyList where HStatus='1' AND HVerifyStatus <> '99'  AND HSave = '1' AND (HPlaceName LIKE '%," + DDL_HOCPlace.SelectedItem.Text + "' OR HPlaceName LIKE '" + DDL_HOCPlace.SelectedItem.Text + ",%' OR HPlaceName LIKE '%," + DDL_HOCPlace.SelectedItem.Text + ",%'   OR HPlaceName='" + DDL_HOCPlace.SelectedItem.Text + "') AND HCourseName LIKE '%" + TB_Search.Text.Trim() + "%' ORDER BY CASE WHEN HVerifyStatus=2 THEN 1 WHEN HVerifyStatus=1 THEN 2 ELSE 0 END ASC, HApplyTime DESC";
            }
            else
            {
                SDS_HCourse.SelectCommand = "SELECT HCourseName, HPlaceName, HStatus, HVerifyStatus, Applicant, HID, HApplyTime, HVerifyTime FROM HCourseVerifyList where HStatus='1' AND HVerifyStatus <> '99'  AND HSave = '1' AND HCourseName LIKE '%" + TB_Search.Text.Trim() + "%' ORDER BY CASE WHEN HVerifyStatus=2 THEN 1 WHEN HVerifyStatus=1 THEN 2 ELSE 0 END ASC, HApplyTime DESC";
            }

        }
        else
        {
            if (DDL_HOCPlace.SelectedValue != "0")
            {
                SDS_HCourse.SelectCommand = "SELECT HCourseName, HPlaceName, HStatus, HVerifyStatus, Applicant, HID, HApplyTime, HVerifyTime FROM HCourseVerifyList where HStatus='1' AND HVerifyStatus <> '99'  AND HSave = '1'  AND (HPlaceName LIKE '%," + DDL_HOCPlace.SelectedItem.Text + "' OR HPlaceName LIKE '" + DDL_HOCPlace.SelectedItem.Text + ",%' OR HPlaceName LIKE '%," + DDL_HOCPlace.SelectedItem.Text + ",%'   OR HPlaceName='" + DDL_HOCPlace.SelectedItem.Text + "')  ORDER BY CASE WHEN HVerifyStatus=2 THEN 1 WHEN HVerifyStatus=1 THEN 2 ELSE 0 END ASC, HApplyTime DESC";
            }
            else
            {
                SDS_HCourse.SelectCommand = "SELECT HCourseName, HPlaceName, HStatus, HVerifyStatus, Applicant, HID, HApplyTime, HVerifyTime FROM HCourseVerifyList where HStatus='1' AND HVerifyStatus <> '99'  AND HSave = '1' ORDER BY CASE WHEN HVerifyStatus=2 THEN 1 WHEN HVerifyStatus=1 THEN 2 ELSE 0 END ASC, HApplyTime DESC";
            }


        }

        #region 分頁copy-3搜尋用
        ViewState["Search"] = SDS_HCourse.SelectCommand;
        //呼叫分頁 (連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 搜尋開啟, DataList控件)
        Pg_Paging.PagingLoad("HochiSystemConnection", SDS_HCourse.SelectCommand, PageMax, LastPage, true, Rpt_HCourse);
        #endregion

    }
    #endregion

    #region 取消搜尋功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_Search.Text = "";
        DDL_HOCPlace.SelectedValue = "0";

        //KE20240508_先預設抓3個月
        SDS_HCourse.SelectCommand = "SELECT HCourseName, HPlaceName, HStatus, HVerifyStatus, Applicant, HID, HApplyTime, HVerifyTime FROM HCourseVerifyList WHERE HStatus='1' AND HVerifyStatus <> '99'  AND HSave = '1'  AND HApplyTime BETWEEN DATEADD(month, -3, GETDATE()) AND  GETDATE() ORDER BY CASE WHEN HVerifyStatus=2 THEN 1 WHEN HVerifyStatus=1 THEN 2 ELSE 0 END ASC, HApplyTime DESC";

        ViewState["Search"] = SDS_HCourse.SelectCommand;

        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCourse.SelectCommand, PageMax, LastPage, true, Rpt_HCourse);
    }
    #endregion

    #region 編輯功能
    protected void LBtn_Edit_Click(object sender, EventArgs e)
    {
        Panel_List.Visible = false;
        Panel_Edit.Visible = true;



        LinkButton LBtn_Edit = sender as LinkButton;
        string Edit_CA = LBtn_Edit.CommandArgument;


        LB_HID.Text = Edit_CA;




        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        //KA20210915
        //ME20221023
        //KE20250725_改*變欄位名稱(要再核對看不需要的欄位)
        string strSelHC = "SELECT a.HCourseName, a.HTeacherName, a.HTeam, a.HDateRange, a.HDharmaID, a.HCTemplateID, a.HTMemberID, a.TeacherName,  a.HPlaceName, a.HStatus, a.HVerifyStatus, a.HCreate, a.HSave, a.HCBatchNum, a.HSCourseID, a.HSTime, a.HETime, a.HRemark,a.HContentTitle, a.HContent, a.HRNContent, a.HImg, BCSchedule, a.BECSchedule, a.ICRecord, a.DPosition, a.CPosition, a.TPosition, a.HUserName, a.HApplicant, a.HApplyTime, a.HVerifyTime, a.HType, a.HEnableSystem, a.HApplySystem, a.HOSystem, a.HNLCourse, a.HRSystem, a.HNGuide, a.HNFull, a.HNRequirement, a.HQuestionID, a.HPMethod, a.HBCPoint, a.HSGList, a.HIRestriction, a.HVerifyNum, a.HBudget, b.HNCWSheet, b.HNCWDay, a.HBudgetTable,a.HID, a.HOCPlace, a.HTMaterialID, a.HCDeadline, a.HSerial, a.HCDeadlineDay, a.HDContinous, a.HBudgetType, a.HCourseType, a.HAxisYN, a.HAxisClass, a.HSupervise, a.HAttRateStandard, a.HExamPassStandard, a.HExamContentID, c.HCourseName as ClassName FROM HCourse_Merge AS a left join HCourse_T AS b ON a.HCTemplateID=b.HID LEFT JOIN HCourse_Class AS c ON a.HType=c.HID WHERE a.HID='" + Edit_CA + "' AND a.HStatus='1'";

        dbCmd = new SqlCommand(strSelHC, dbConn);
        SqlDataReader MyQueryHC = dbCmd.ExecuteReader();
        if (MyQueryHC.Read())
        {
            LB_HCourseName.Text = MyQueryHC["HCourseName"].ToString();
            LB_HCTemplateID.Text = MyQueryHC["HCTemplateID"].ToString();

            //KE20210915_講師轉成為多個人
            string[] gHMember = MyQueryHC["HTeacherName"].ToString().Split(',');

            //KA20231002_新增講師HID
            LB_HTeacherHID.Text = MyQueryHC["HTeacherName"].ToString();
            //string gHR = "";
            for (int i = 0; i < gHMember.Length - 1; i++)
            {
                //講師帶入區屬期別姓名
                //講師改抓為HTeacher.HID
                string strSelHM = "select B.HID, B.HUserName, B.HAreaID, B.HPeriod, (C.HArea+'/'+B.HPeriod+' '+B.HUserName) as UserName, B.HStatus from HTeacher AS A LEFT JOIN  HMember AS B ON A.HMemberID=B.HID Left Join HArea AS C On B.HAreaID =C.HID where A.HID = '" + gHMember[i].ToString() + "'";
                //Response.Write(strSelHM+"<br/>");
                dbCmd = new SqlCommand(strSelHM, dbConn);
                SqlDataReader MyQueryHM = dbCmd.ExecuteReader();
                if (MyQueryHM.Read())
                {
                    LB_HTeacherName.Text += MyQueryHM["UserName"].ToString() + ",";
                    //KA20231011_新增MemberHID&區屬
                    LB_HMemberHID.Text += MyQueryHM["HID"].ToString() + ",";
                    LB_HAreaID.Text += MyQueryHM["HAreaID"].ToString() + ",";
                }
                MyQueryHM.Close();
            }

            LB_HTeacherName.Text = LB_HTeacherName.Text == null || LB_HTeacherName.Text == "" ? "" : LB_HTeacherName.Text.Substring(0, (LB_HTeacherName.Text.Length) - 1);
            //KA20231011_新增MemberHID
            LB_HMemberHID.Text = LB_HMemberHID.Text == null || LB_HMemberHID.Text == "" ? "" : LB_HMemberHID.Text.Substring(0, (LB_HMemberHID.Text.Length) - 1);

            //HOSystem，1=導師、2=導護、3=營運、4=服務、5=玉成、6=守護、7=法系
            LB_HOSystem.Text = MyQueryHC["HOSystem"].ToString() == "1" ? "導師" : MyQueryHC["HOSystem"].ToString() == "2" ? "導護" : MyQueryHC["HOSystem"].ToString() == "3" ? "營運" : MyQueryHC["HOSystem"].ToString() == "4" ? "服務" : MyQueryHC["HOSystem"].ToString() == "5" ? "玉成" : MyQueryHC["HOSystem"].ToString() == "6" ? "守護" : MyQueryHC["HOSystem"].ToString() == "7" ? "法系" : "";

            //HRSystem，1=導師、2=導護、3=營運、4=服務、5=玉成、6=守護、7=法系
            string[] gHRSystem = MyQueryHC["HRSystem"].ToString().Split(',');
            for (int i = 0; i < gHRSystem.Length - 1; i++)
            {
                string strSelHR = "SELECT HID, HSystemName, HStatus FROM HSystem WHERE HStatus = 1 AND HID='" + gHRSystem[i].ToString() + "'";
                dbCmd = new SqlCommand(strSelHR, dbConn);
                SqlDataReader MyQueryHR = dbCmd.ExecuteReader();
                if (MyQueryHR.Read())
                {
                    LB_HRSystem.Text += MyQueryHR["HSystemName"].ToString() + ",";
                }
                MyQueryHR.Close();

            }
            LB_HRSystem.Text = LB_HRSystem.Text == null || LB_HRSystem.Text == "" ? "" : LB_HRSystem.Text.Substring(0, (LB_HRSystem.Text.Length) - 1);


            //HNLCourse，0=否、1=是
            LB_HNLCourse.Text = MyQueryHC["HNLCourse"].ToString() == "0" ? "否" : MyQueryHC["HNGuide"].ToString() == "1" ? "是" : "";
            //HNGuide，0=否、1=是
            LB_HNGuide.Text = MyQueryHC["HNGuide"].ToString() == "0" ? "否" : MyQueryHC["HNGuide"].ToString() == "1" ? "是" : "";
            //HNFull，0=否、1=是
            LB_HNFull.Text = MyQueryHC["HNFull"].ToString() == "0" ? "否" : MyQueryHC["HNFull"].ToString() == "1" ? "是" : "";

            //HTeam 主班團隊姓名
            LB_HTeam_temp.Text = MyQueryHC["HTeam"].ToString();
            string[] gHTeam = MyQueryHC["HTeam"].ToString().Split(',');

            //string gHR = "";
            for (int i = 0; i < gHTeam.Length - 1; i++)
            {
                //主班團隊帶入區屬期別姓名
                // string strSelHM = "select HMember.HID, HUserName,HAreaID, HPeriod, (HArea+'/'+HPeriod+' '+HUserName) as UserName, HStatus from HMember Left Join HArea On HMember.HAreaID =HArea.HID where HMember.HID like '%" + gHTeam[i].ToString() + "%' order by HUserName";
                string strSelHM = "select HMember.HID, HUserName,HAreaID, HPeriod, (HArea+'/'+HPeriod+' '+HUserName) as UserName, HMember.HStatus from HMember Left Join HArea On HMember.HAreaID =HArea.HID where HMember.HID= '" + gHTeam[i].ToString() + "'";
                dbCmd = new SqlCommand(strSelHM, dbConn);
                SqlDataReader MyQueryHM = dbCmd.ExecuteReader();
                if (MyQueryHM.Read())
                {
                    LB_HTeam.Text += MyQueryHM["UserName"].ToString() + ",";
                }
                MyQueryHM.Close();
            }
            LB_HTeam.Text = LB_HTeam.Text == null || LB_HTeam.Text == "" ? "" : LB_HTeam.Text.Substring(0, (LB_HTeam.Text.Length) - 1);



            LB_HNCW.Text = MyQueryHC["HNCWSheet"].ToString() + "篇," + MyQueryHC["HNCWDay"].ToString() + "天";
            //KA20210713_加入欄位
            //LB_HCourseNum.Text = MyQueryHC["HCourseNum"].ToString();
            LB_HCBatchNum.Text = MyQueryHC["HCBatchNum"].ToString();
            LB_HOCPlace.Text = MyQueryHC["HPlaceName"].ToString();
            LB_HDateRange.Text = MyQueryHC["HDateRange"].ToString();
            LBHTime.Text = MyQueryHC["HSTime"].ToString() + "~" + MyQueryHC["HETime"].ToString();
            //HType，1=入門課程(新生)、2=玉成課程、3=傳光課程、4=族群課程、5=體系課程、6=公法課程

            LB_HType.Text = MyQueryHC["ClassName"].ToString();
            //HPMethod，1=基金會、2=文化事業
            LB_HPMethod.Text = MyQueryHC["HPMethod"].ToString() == "1" ? "基金會" : MyQueryHC["HPMethod"].ToString() == "2" ? "文化事業" : "";
            LB_HPMethod_temp.Text = MyQueryHC["HPMethod"].ToString();

            string[] gHNRequirement = MyQueryHC["HNRequirement"].ToString().Split(',');
            for (int i = 0; i < gHNRequirement.Length - 1; i++)
            {
                //受傳法改抓資料表HDharma
                //gHR = gHNRequirement[i].ToString() == "1" ? "大愛手證照" : gHNRequirement[i].ToString() == "2" ? "尋光階呼吸講師" : "";
                string strSelDharma = "SELECT HID, HDTypeID, HDharmaName FROM HDharma WHERE HStatus='1' AND HID= '" + gHNRequirement[i].ToString() + "'";
                dbCmd = new SqlCommand(strSelDharma, dbConn);
                SqlDataReader MyQueryDharma = dbCmd.ExecuteReader();
                if (MyQueryDharma.Read())
                {
                    LB_HNRequirement.Text += MyQueryDharma["HDharmaName"].ToString() + ",";
                }
                MyQueryDharma.Close();

            }
            //LB_HNRequirement.Text = LB_HNRequirement.Text.Substring(0, (LB_HNRequirement.Text.Length) - 1);
            LB_HNRequirement.Text = LB_HNRequirement.Text == null || LB_HNRequirement.Text == "" ? "" : LB_HNRequirement.Text.Substring(0, (LB_HNRequirement.Text.Length) - 1);

            //HIRestriction，學員類別限制改抓HMType
            string[] gHIRestriction = MyQueryHC["HIRestriction"].ToString().Split(',');
            for (int i = 0; i < gHIRestriction.Length - 1; i++)
            {
                string strSelHIR = "SELECT HID, HMType FROM HMType WHERE HStatus='1' AND HID='" + gHIRestriction[i].ToString() + "'";
                dbCmd = new SqlCommand(strSelHIR, dbConn);
                SqlDataReader MyQueryHIR = dbCmd.ExecuteReader();
                if (MyQueryHIR.Read())
                {
                    LB_HIRestriction.Text += MyQueryHIR["HMType"].ToString() + ",";
                }
                MyQueryHIR.Close();

            }
            LB_HIRestriction.Text = LB_HIRestriction.Text == null || LB_HIRestriction.Text == "" ? "" : LB_HIRestriction.Text.Substring(0, (LB_HIRestriction.Text.Length) - 1);
            //LB_HIRestriction.Text = MyQueryHC["HIRestriction"].ToString() == "1" ? "新生" : MyQueryHC["HIRestriction"].ToString() == "2" ? "舊生" : "";


            //20220819
            LB_HBCPoint.Text = (Convert.ToInt32(MyQueryHC["HBCPoint"].ToString()) * 10).ToString();

            LB_HRemark.Text = HttpUtility.HtmlDecode(MyQueryHC["HRemark"].ToString());
            LB_HContentTitle.Text = HttpUtility.HtmlDecode(MyQueryHC["HContentTitle"].ToString());
            CKE_HContent.Text = HttpUtility.HtmlDecode(MyQueryHC["HContent"].ToString());


            SDS_HCourseMaterial.SelectCommand = "SELECT HCMName, HCMaterial, HCMLink, HSave FROM HCourseMaterial_T where HCTemplateID='" + MyQueryHC["HCTemplateID"].ToString() + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave  UNION ALL SELECT HCMName, HCMaterial, HCMLink, HSave FROM HCourseMaterial where HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY HCMName, HCMaterial, HCMLink, HSave";

            SDS_HLeadingCourse.SelectCommand = "SELECT HLCourseID, HDiscount, a.HSave, b.HTemplateName FROM HLeadingCourse_T AS a JOIN HCourse_T AS b ON a.HCTemplateID=b.HID  WHERE HCTemplateID='" + MyQueryHC["HCTemplateID"].ToString() + "' UNION ALL SELECT HLCourseID, HDiscount, a.HSave, b.HCourseName FROM HLeadingCourse AS a JOIN HCourse AS b ON a.HCBatchNum=b.HCBatchNum WHERE a.HCBatchNum='" + LB_HCBatchNum.Text + "'";

            SDS_HTodoList.SelectCommand = "SELECT HGroupName, HTask, HTaskContent, HGroupLeaderID,HSave, HTaskNum, HExamStaff FROM HTodoList where HCBatchNum='" + LB_HCBatchNum.Text + "' AND HStatus=1 AND HSave=1 GROUP BY HCBatchNum, HGroupName, HTask, HTaskContent, HGroupLeaderID,HSave, HTaskNum, HExamStaff";

            SDS_HCVerifyLog.SelectCommand = "SELECT a.HSignName, a.HVDate, a.HVResult, a.HVOpinion, b.HUserName FROM HCVerifyLog as a Join HMember as b on a.HSignName= b.HID where a.HCBatchNum='" + LB_HCBatchNum.Text + "' GROUP BY a.HSignName, a.HVDate, a.HVResult, a.HVOpinion, b.HUserName";
        }

        MyQueryHC.Close();
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






        string strUpdHC_T = "update HCourse set HStatus='0' where HID='" + Del_CA + "'";
        dbCmd = new SqlCommand(strUpdHC_T, dbConn);
        dbCmd.ExecuteNonQuery();

        string strUpdHCM_T = "update HCourseMaterial set HStatus='0' where HID='" + Del_CA + "'";
        dbCmd = new SqlCommand(strUpdHCM_T, dbConn);
        dbCmd.ExecuteNonQuery();

        string strUpdHLC_T = "update HLeadingCourse set HStatus='0' where HID='" + Del_CA + "'";
        dbCmd = new SqlCommand(strUpdHLC_T, dbConn);
        dbCmd.ExecuteNonQuery();

        string strUpdHTL_T = "update HTodoList set HStatus='0' where HID='" + Del_CA + "'";
        dbCmd = new SqlCommand(strUpdHTL_T, dbConn);
        dbCmd.ExecuteNonQuery();






        dbConn.Close();
        dbCmd.Cancel();

        Response.Write("<script>alert('刪除成功!');window.location.href='HCourseVerify.aspx';</script>");




    }
    #endregion

    protected void Rpt_HCourse_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //HVerifyStatus→0審核中、1退審、2審核已通過、3核准不通過、4.核准已通過
        //  ((Label)e.Item.FindControl("LB_HVStatus")).Text = ((Label)e.Item.FindControl("LB_HVStatus")).Text == "0" ? "審核中" : ((Label)e.Item.FindControl("LB_HVStatus")).Text == "1"?"待補件": "已通過";

        #region 用途
        HtmlGenericControl Status = e.Item.FindControl("Status") as HtmlGenericControl;
        switch (((Label)e.Item.FindControl("LB_HVStatus")).Text)
        {
            case "0":
                Status.Attributes.Add("class", "label label-info");
                ((Label)e.Item.FindControl("LB_HVStatus")).Text = "審核中";
                break;
            case "1":
                Status.Attributes.Add("class", "label label-danger");
                ((Label)e.Item.FindControl("LB_HVStatus")).Text = "退審";
                break;
            case "2":
                Status.Attributes.Add("class", "label label-success");
                ((Label)e.Item.FindControl("LB_HVStatus")).Text = "審核通過";
                break;
            case "3":
                Status.Attributes.Add("class", "label label-success");
                ((Label)e.Item.FindControl("LB_HVStatus")).Text = "核准不通過";
                break;
            case "4":
                Status.Attributes.Add("class", "label label-success");
                ((Label)e.Item.FindControl("LB_HVStatus")).Text = "核准通過";
                break;

        }
        #endregion
    }

    protected void Rpt_HCourseMaterial_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ((HyperLink)e.Item.FindControl("HL_HCMaterial")).Visible = true;
        ((HyperLink)e.Item.FindControl("HL_HCMaterial")).NavigateUrl = "";
        ((HyperLink)e.Item.FindControl("HL_HCMaterial")).Target = "_blank";
    }

    protected void Rpt_HLeadingCourse_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //取得Repeater的DataRowView
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HDiscount")).Text))
        {
            //20220819-轉成金額(1元=10點)
            ((Label)e.Item.FindControl("LB_HDiscount")).Text = (Convert.ToInt32(Convert.ToDouble(((Label)e.Item.FindControl("LB_HDiscount")).Text)) * 10).ToString();
        }

    }

    protected void Rpt_HTodoList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

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

        ((TextBox)e.Item.FindControl("TB_HTaskContent")).Text = HttpUtility.HtmlDecode(((TextBox)e.Item.FindControl("TB_HTaskContent")).Text);

        ((DropDownList)e.Item.FindControl("DDL_HGroupName")).SelectedValue = gDRV["HGroupName"].ToString();

        //MA20240623_試務人員名單(多選)
        string[] gHExamStaff = ((Label)e.Item.FindControl("LB_HExamStaff")).Text.Split(',');
        for (int i = 0; i < gHExamStaff.Length - 1; i++)
        {
            for (int j = 0; j < ((ListBox)e.Item.FindControl("LBox_HExamStaff")).Items.Count; j++)
            {
                if (gHExamStaff[i].ToString() == ((ListBox)e.Item.FindControl("LBox_HExamStaff")).Items[j].Value)
                {
                    ((ListBox)e.Item.FindControl("LBox_HExamStaff")).Items[j].Selected = true;
                    ((Label)e.Item.FindControl("LB_HExamStaff")).Text += ((ListBox)e.Item.FindControl("LBox_HExamStaff")).Items[j].Text + ",";
                }
            }
        }

    }

    protected void Rpt_HCVerifyLog_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        //HVResult→0審核中、1退審、2審核通過、3核准不通過、4核准通過
        ((Label)e.Item.FindControl("LB_HVResult")).Text = ((Label)e.Item.FindControl("LB_HVResult")).Text == "0" ? "審核中" : ((Label)e.Item.FindControl("LB_HVResult")).Text == "1" ? "退審" : ((Label)e.Item.FindControl("LB_HVResult")).Text == "2" ? "審核通過" : ((Label)e.Item.FindControl("LB_HVResult")).Text == "3" ? "核准不通過" : ((Label)e.Item.FindControl("LB_HVResult")).Text == "4" ? "核准通過" : "";


    }

    #region 取消功能
    protected void Btn_Cancel_Click(object sender, EventArgs e)
    {
        Response.Write("<script>window.location.href='HCourseVerify.aspx';</script>");

    }
    #endregion

    #region 通過功能
    protected void Btn_Allow_Click(object sender, EventArgs e)
    {
        int gCT = 1;
        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();


        string strUniteTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        string strUniteDate = DateTime.Now.ToString("yyyy/MM/dd");


        //if (核准權限)
        //{
        //HVerifyStatus→0審核中、1退審、2審核已通過、3核准不通過、4.核准已通過
        //目前只有一層審核，所以HVerifyStatus有使用的只有→0審核中、1退審、2審核已通過
        string strUpdHC = "update HCourse set HVerifyTime='" + strUniteTime + "', HVerifyStatus='2' where HCBatchNum = '" + LB_HCBatchNum.Text + "'";
        dbCmd = new SqlCommand(strUpdHC, dbConn);
        dbCmd.ExecuteReader();

        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Trim(',').Split(',');
            for (int i = 0; i < strHID.Length; i++)
            {
                //HVResult→0審核中、1退審、2審核通過、3核准不通過、4核准通過
                //目前只有一層審核，所以HVResult有使用的只有→0審核中、1退審、2審核已通過
                string strInsHC = "INSERT INTO HCVerifyLog (HCourseID, HSignName, HVDate, HVResult, HVOpinion, HStatus, HCreate, HCreateDT, HSave, HCBatchNum) VALUES ('" + strHID[i] + "', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteDate + "', '2', '" + TB_HVOpinion.Text.Trim() + "', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteTime + "', '1', '" + LB_HCBatchNum.Text + "')";
                dbCmd = new SqlCommand(strInsHC, dbConn);
                dbCmd.ExecuteReader();



                #region 判斷若有變更日期或課程名稱，且有人報名的話，會變更HCoursebooking & HCBChangeRecord&HLCourse&HLCourse_Detail


                //KE20240205_新增套裝課程判斷

                SqlDataReader QueryHCBPackage = SQLdatabase.ExecuteReader("SELECT HCourseName, HCPkgName, HCPkgHID FROM HCourseBooking  WHERE HCourseID='" + strHID[i] + "' GROUP BY HCourseName, HCPkgName, HCPkgHID");
                if (QueryHCBPackage.Read())
                {

                    string strUpdHCBPackage = "UPDATE HCoursePackage_Detail SET HCourseName=N'" + LB_HCourseName.Text + "', HDateRange= N'" + LB_HDateRange.Text + "' WHERE HCPkgHID='" + QueryHCBPackage["HCPkgHID"].ToString() + "' AND HCourseName='" + QueryHCBPackage["HCourseName"].ToString() + "'";
                    dbCmd = new SqlCommand(strUpdHCBPackage, dbConn);
                    dbCmd.ExecuteReader();

                }
                QueryHCBPackage.Close();

                //KA20240111_新增判斷

                SqlDataReader QueryHCBooking = SQLdatabase.ExecuteReader("SELECT HID, HCourseName, HDateRange FROM HCourseBooking  WHERE HCourseID='" + strHID[i] + "'");
                while (QueryHCBooking.Read())
                {
                    string strUpdHCBooking = "UPDATE HCourseBooking SET HCourseName=N'" + LB_HCourseName.Text + "', HDateRange= N'" + LB_HDateRange.Text + "' WHERE HID='" + QueryHCBooking["HID"].ToString() + "' AND HCourseID='" + strHID[i] + "'";
                    dbCmd = new SqlCommand(strUpdHCBooking, dbConn);
                    dbCmd.ExecuteReader();


                }
                QueryHCBooking.Close();

                SqlDataReader QueryHCBChangeRecord = SQLdatabase.ExecuteReader("SELECT HID, HCourseName, HDateRange FROM HCBChangeRecord  WHERE HCourseIDNew='" + strHID[i] + "'");
                while (QueryHCBChangeRecord.Read())
                {
                    string strUpdHCBChangeRecord = "UPDATE HCBChangeRecord SET HCourseName=N'" + LB_HCourseName.Text + "', HDateRange= N'" + LB_HDateRange.Text + "' WHERE HID='" + QueryHCBChangeRecord["HID"].ToString() + "' AND HCourseIDNew='" + strHID[i] + "'";
                    dbCmd = new SqlCommand(strUpdHCBChangeRecord, dbConn);
                    dbCmd.ExecuteReader();
                }
                QueryHCBChangeRecord.Close();


                #endregion




            }
        }


        dbConn.Close();
        dbCmd.Cancel();







        #region 幫主班團隊報名
        //KA20231020_新增
        //繳費帳戶：1 - 基金會(金額) / 2 - 文化事業(點數)

        string[] gHTeam = LB_HTeam_temp.Text.TrimEnd(',').Split(',');



        foreach (var item in gHTeam)
        {

            //先判斷是否已報名此課程
            SqlDataReader QueryBooking = SQLdatabase.ExecuteReader("SELECT HID FROM OrderList_Merge WHERE HCTemplateID='" + LB_HCTemplateID.Text + "' AND HCourseName= N'" + LB_HCourseName.Text + "' AND HDateRange='" + LB_HDateRange.Text + "' AND HMemberID='" + item + "' AND HCourseDonate='0' AND HStatus=1 AND HItemStatus=1");

            if (!QueryBooking.Read())
            {
                //KA20221109-新增寫入他人的購物車
                #region 寫入他人的購物車 
                SqlDataReader QuerySelSC = SQLdatabase.ExecuteReader("SELECT a.HID, a.HCTemplateID, a.HCourseName, a.HDateRange, b.HPMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCourseID=b.HID WHERE a.HCTemplateID='" + LB_HCTemplateID.Text + "' AND a.HCourseName= N'" + LB_HCourseName.Text + "' AND a.HDateRange='" + LB_HDateRange.Text + "' AND a.HMemberID='" + item + "' and a.HCourseDonate='0'");
                if (QuerySelSC.Read())
                {
                    //更新資料庫
                    SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HCourseID=@HCourseID, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID =@HID and HCourseDonate = '0' and HMemberID=@HMemberID", SQLdatabase.OpenConnection());
                    QueryUpdSC.Parameters.AddWithValue("@HID", QuerySelSC["HID"].ToString());
                    QueryUpdSC.Parameters.AddWithValue("@HCourseID", '0');
                    QueryUpdSC.Parameters.AddWithValue("@HMemberID", item);
                    QueryUpdSC.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    QueryUpdSC.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    QueryUpdSC.ExecuteNonQuery();
                    QueryUpdSC.Cancel();
                    SQLdatabase.OpenConnection().Close();

                    //ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('此課程已經報名了喔!');", true);

                }
                else
                {
                    string gHPoint = null;
                    //若是文化事業課程要換成點數
                    gHPoint = LB_HPMethod_temp.Text == "1" ? LB_HBCPoint.Text : (Convert.ToInt32(LB_HBCPoint.Text) / 10).ToString();


                    //寫入資料庫

                    //KE20231012_新增需要的欄位，避免後續產生報表group by有問題
                    SqlCommand QueryInsSC = new SqlCommand("INSERT INTO HShoppingCart (HOrderNum, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HSelect, HCourseDonate, HStatus, HCreate, HCreateDT) VALUES (@HOrderNum, @HTradeNo, @HMerchantTradeNo, @HMemberID, @HCourseID, @HCTemplateID, @HCourseName, @HDateRange, @HPMethod, @HAttend, @HLodging, @HBDate, @HLDate, @HLCourse, @HLCourseName, @HLDiscount, @HCGuide, @HPayMethod, @HPoint, @HMemberGroup, @HDharmaPass, @HRoom, @HRoomTime, @HSubscribe, @HDCode, @HDPoint, @HPaymentNo, @HExpireDate, @HFailReason, @HPaymentDate, @HPayAmt, @HFinanceRemark, @HInvoiceNo, @HInvoiceDate, @HInvoiceStatus, @HRemark, @HSelect, @HCourseDonate, @HStatus, @HCreate, @HCreateDT)", SQLdatabase.OpenConnection());

                    QueryInsSC.Parameters.AddWithValue("@HOrderNum", "");
                    QueryInsSC.Parameters.AddWithValue("@HTradeNo", "");
                    QueryInsSC.Parameters.AddWithValue("@HMerchantTradeNo", "");
                    QueryInsSC.Parameters.AddWithValue("@HMemberID", item);
                    QueryInsSC.Parameters.AddWithValue("@HCourseID", '0');
                    QueryInsSC.Parameters.AddWithValue("@HCTemplateID", LB_HCTemplateID.Text);
                    QueryInsSC.Parameters.AddWithValue("@HCourseName", LB_HCourseName.Text);
                    QueryInsSC.Parameters.AddWithValue("@HDateRange", LB_HDateRange.Text);
                    QueryInsSC.Parameters.AddWithValue("@HPMethod", LB_HPMethod_temp.Text);
                    QueryInsSC.Parameters.AddWithValue("@HAttend", "5"); //幫講師或主班報名身分為純護持(非班員)
                    QueryInsSC.Parameters.AddWithValue("@HLodging", "");
                    QueryInsSC.Parameters.AddWithValue("@HBDate", "");
                    QueryInsSC.Parameters.AddWithValue("@HLDate", "");
                    QueryInsSC.Parameters.AddWithValue("@HLCourse", "");
                    QueryInsSC.Parameters.AddWithValue("@HLCourseName", "");
                    QueryInsSC.Parameters.AddWithValue("@HLDiscount", "");
                    QueryInsSC.Parameters.AddWithValue("@HCGuide", "");
                    QueryInsSC.Parameters.AddWithValue("@HPayMethod", "");
                    QueryInsSC.Parameters.AddWithValue("@HPoint", gHPoint);
                    QueryInsSC.Parameters.AddWithValue("@HMemberGroup", "");
                    QueryInsSC.Parameters.AddWithValue("@HDharmaPass", "");
                    QueryInsSC.Parameters.AddWithValue("@HRoom", "");
                    QueryInsSC.Parameters.AddWithValue("@HRoomTime", "");
                    QueryInsSC.Parameters.AddWithValue("@HSubscribe", "");
                    QueryInsSC.Parameters.AddWithValue("@HDCode", "");
                    QueryInsSC.Parameters.AddWithValue("@HDPoint", "");
                    QueryInsSC.Parameters.AddWithValue("@HPaymentNo", "");
                    QueryInsSC.Parameters.AddWithValue("@HExpireDate", "");
                    QueryInsSC.Parameters.AddWithValue("@HFailReason", "");
                    QueryInsSC.Parameters.AddWithValue("@HPaymentDate", "");
                    QueryInsSC.Parameters.AddWithValue("@HPayAmt", "");
                    QueryInsSC.Parameters.AddWithValue("@HFinanceRemark", "");
                    QueryInsSC.Parameters.AddWithValue("@HInvoiceNo", "");
                    QueryInsSC.Parameters.AddWithValue("@HInvoiceDate", "");
                    QueryInsSC.Parameters.AddWithValue("@HInvoiceStatus", "");
                    QueryInsSC.Parameters.AddWithValue("@HRemark", "");
                    QueryInsSC.Parameters.AddWithValue("@HSelect", false);
                    QueryInsSC.Parameters.AddWithValue("@HCourseDonate", "0");
                    QueryInsSC.Parameters.AddWithValue("@HStatus", "1");
                    QueryInsSC.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    QueryInsSC.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    QueryInsSC.ExecuteNonQuery();
                    QueryInsSC.Cancel();
                    SQLdatabase.OpenConnection().Close();

                }
                QuerySelSC.Close();
                #endregion
            }
            QueryBooking.Close();




        }

        #endregion


        #region //20231020_幫講師報名(加入購物車)
        //KA20231020_新增
        //繳費帳戶：1 - 基金會(金額) / 2 - 文化事業(點數)

        string[] gHTeacher_MemberID = LB_HMemberHID.Text.TrimEnd(',').Split(',');



        foreach (var item in gHTeacher_MemberID)
        {

            //先判斷是否已報名此課程
            SqlDataReader QueryBooking = SQLdatabase.ExecuteReader("SELECT HID FROM OrderList_Merge WHERE HCTemplateID='" + LB_HCTemplateID.Text + "' AND HCourseName= N'" + LB_HCourseName.Text + "' AND HDateRange='" + LB_HDateRange.Text + "' AND HMemberID='" + item + "' AND HCourseDonate='0' AND HStatus=1 AND HItemStatus=1");

            if (!QueryBooking.Read())
            {
                //KA20221109_新增寫入他人購物車
                #region 寫入他人的購物車 
                SqlDataReader QuerySelSC = SQLdatabase.ExecuteReader("SELECT a.HID, a.HCTemplateID, a.HCourseName, a.HDateRange, b.HPMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCourseID=b.HID WHERE a.HCTemplateID='" + LB_HCTemplateID.Text + "' AND a.HCourseName= N'" + LB_HCourseName.Text + "' AND a.HDateRange='" + LB_HDateRange.Text + "' AND a.HMemberID='" + item + "' and a.HCourseDonate='0'");
                //Response.Write("SELECT a.HID, b.HPMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCourseID=b.HID WHERE a.HCourseID='" + gCourseBooking_CA + "' and a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' and a.HCourseDonate='0'");
                //Response.End();
                if (QuerySelSC.Read())
                {
                    //更新資料庫
                    SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HCourseID=@HCourseID, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID =@HID and HCourseDonate = '0' and HMemberID=@HMemberID", SQLdatabase.OpenConnection());
                    QueryUpdSC.Parameters.AddWithValue("@HID", QuerySelSC["HID"].ToString());
                    QueryUpdSC.Parameters.AddWithValue("@HCourseID", '0');
                    QueryUpdSC.Parameters.AddWithValue("@HMemberID", item);
                    QueryUpdSC.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    QueryUpdSC.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    QueryUpdSC.ExecuteNonQuery();
                    QueryUpdSC.Cancel();
                    SQLdatabase.OpenConnection().Close();

                    //ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('此課程已經報名了喔!');", true);

                }
                else
                {
                    string gHPoint = null;
                    //若是文化事業課程要換成點數
                    gHPoint = LB_HPMethod_temp.Text == "1" ? LB_HBCPoint.Text : (Convert.ToInt32(LB_HBCPoint.Text) / 10).ToString();


                    //寫入資料庫
                    //  SqlCommand QueryInsSC = new SqlCommand("INSERT INTO HShoppingCart (HOrderNum, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend,HPoint, HSelect, HCourseDonate, HStatus, HCreate, HCreateDT) VALUES (@HOrderNum, @HTradeNo, @HMerchantTradeNo, @HMemberID, @HCourseID, @HCTemplateID, @HCourseName, @HDateRange, @HPMethod, @HAttend, @HPoint, @HSelect, @HCourseDonate, @HStatus, @HCreate, @HCreateDT)", SQLdatabase.OpenConnection())

                    //KE20231012_新增需要的欄位，避免後續產生報表group by有問題
                    SqlCommand QueryInsSC = new SqlCommand("INSERT INTO HShoppingCart (HOrderNum, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HSelect, HCourseDonate, HStatus, HCreate, HCreateDT) VALUES (@HOrderNum, @HTradeNo, @HMerchantTradeNo, @HMemberID, @HCourseID, @HCTemplateID, @HCourseName, @HDateRange, @HPMethod, @HAttend, @HLodging, @HBDate, @HLDate, @HLCourse, @HLCourseName, @HLDiscount, @HCGuide, @HPayMethod, @HPoint, @HMemberGroup, @HDharmaPass, @HRoom, @HRoomTime, @HSubscribe, @HDCode, @HDPoint, @HPaymentNo, @HExpireDate, @HFailReason, @HPaymentDate, @HPayAmt, @HFinanceRemark, @HInvoiceNo, @HInvoiceDate, @HInvoiceStatus, @HRemark, @HSelect, @HCourseDonate, @HStatus, @HCreate, @HCreateDT)", SQLdatabase.OpenConnection());

                    QueryInsSC.Parameters.AddWithValue("@HOrderNum", "");
                    QueryInsSC.Parameters.AddWithValue("@HTradeNo", "");
                    QueryInsSC.Parameters.AddWithValue("@HMerchantTradeNo", "");
                    QueryInsSC.Parameters.AddWithValue("@HMemberID", item);
                    QueryInsSC.Parameters.AddWithValue("@HCourseID", '0');
                    QueryInsSC.Parameters.AddWithValue("@HCTemplateID", LB_HCTemplateID.Text);
                    QueryInsSC.Parameters.AddWithValue("@HCourseName", LB_HCourseName.Text);
                    QueryInsSC.Parameters.AddWithValue("@HDateRange", LB_HDateRange.Text);
                    QueryInsSC.Parameters.AddWithValue("@HPMethod", LB_HPMethod_temp.Text);
                    QueryInsSC.Parameters.AddWithValue("@HAttend", "5"); //幫講師或主班報名身分為純護持(非班員)
                    QueryInsSC.Parameters.AddWithValue("@HLodging", "");
                    QueryInsSC.Parameters.AddWithValue("@HBDate", "");
                    QueryInsSC.Parameters.AddWithValue("@HLDate", "");
                    QueryInsSC.Parameters.AddWithValue("@HLCourse", "");
                    QueryInsSC.Parameters.AddWithValue("@HLCourseName", "");
                    QueryInsSC.Parameters.AddWithValue("@HLDiscount", "");
                    QueryInsSC.Parameters.AddWithValue("@HCGuide", "");
                    QueryInsSC.Parameters.AddWithValue("@HPayMethod", "");
                    QueryInsSC.Parameters.AddWithValue("@HPoint", gHPoint);
                    QueryInsSC.Parameters.AddWithValue("@HMemberGroup", "");
                    QueryInsSC.Parameters.AddWithValue("@HDharmaPass", "");
                    QueryInsSC.Parameters.AddWithValue("@HRoom", "");
                    QueryInsSC.Parameters.AddWithValue("@HRoomTime", "");
                    QueryInsSC.Parameters.AddWithValue("@HSubscribe", "");
                    QueryInsSC.Parameters.AddWithValue("@HDCode", "");
                    QueryInsSC.Parameters.AddWithValue("@HDPoint", "");
                    QueryInsSC.Parameters.AddWithValue("@HPaymentNo", "");
                    QueryInsSC.Parameters.AddWithValue("@HExpireDate", "");
                    QueryInsSC.Parameters.AddWithValue("@HFailReason", "");
                    QueryInsSC.Parameters.AddWithValue("@HPaymentDate", "");
                    QueryInsSC.Parameters.AddWithValue("@HPayAmt", "");
                    QueryInsSC.Parameters.AddWithValue("@HFinanceRemark", "");
                    QueryInsSC.Parameters.AddWithValue("@HInvoiceNo", "");
                    QueryInsSC.Parameters.AddWithValue("@HInvoiceDate", "");
                    QueryInsSC.Parameters.AddWithValue("@HInvoiceStatus", "");
                    QueryInsSC.Parameters.AddWithValue("@HRemark", "");
                    QueryInsSC.Parameters.AddWithValue("@HSelect", false);
                    QueryInsSC.Parameters.AddWithValue("@HCourseDonate", "0");
                    QueryInsSC.Parameters.AddWithValue("@HStatus", "1");
                    QueryInsSC.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    QueryInsSC.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    QueryInsSC.ExecuteNonQuery();
                    QueryInsSC.Cancel();
                    SQLdatabase.OpenConnection().Close();

                }
                QuerySelSC.Close();
                #endregion
            }
            QueryBooking.Close();




        }

        #endregion
        Response.Write("<script>alert('審核通過存檔成功!');window.location.href='HCourseVerify.aspx';</script>");

    }
    #endregion

    #region 退審功能
    protected void Btn_Deny_Click(object sender, EventArgs e)
    {



        int gCT = 1;
        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        string strUniteTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        string strUniteDate = DateTime.Now.ToString("yyyy/MM/dd");

        //HVerifyStatus→0審核中、1退審、2審核已通過、3核准不通過、4.核准已通過
        //目前只有一層審核，所以HVerifyStatus有使用的只有→0審核中、1退審、2審核已通過
        string strUpdHC = "update HCourse set HVerifyTime='" + strUniteTime + "', HVerifyStatus='1' where HCBatchNum = '" + LB_HCBatchNum.Text + "'";
        //string strUpdHC = "update HCourse set HVerifyTime='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HVerifyStatus='1' where HID = '" + LB_HID.Text + "'";
        dbCmd = new SqlCommand(strUpdHC, dbConn);
        dbCmd.ExecuteReader();

        if (LB_HID.Text.Trim(',') != "")
        {
            string[] strHID = LB_HID.Text.Trim(',').Split(',');
            for (int i = 0; i < strHID.Length; i++)
            {
                //HVResult→0審核中、1退審、2審核通過、3核准不通過、4核准通過
                //目前只有一層審核，所以HVResult有使用的只有→0審核中、1退審、2審核已通過
                string strInsHC = "INSERT INTO HCVerifyLog (HCourseID, HSignName, HVDate, HVResult, HVOpinion, HStatus, HCreate, HCreateDT, HSave, HCBatchNum) VALUES ('" + strHID[i] + "', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + strUniteDate + "', '1', '" + TB_HVOpinion.Text + "', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' , '" + strUniteTime + "', '1', '" + LB_HCBatchNum.Text + "')";
                dbCmd = new SqlCommand(strInsHC, dbConn);
                dbCmd.ExecuteReader();

                //ME20230308_刪除購物車中已退審或已刪除的課程
                string strDelHC = "DELETE FROM HShoppingCart WHERE HCourseID IN ( SELECT HID FROM HCourse WHERE HID='" + strHID[i] + "' AND (HVerifyStatus ='1' OR HStatus='0'))";
                dbCmd = new SqlCommand(strDelHC, dbConn);
                dbCmd.ExecuteNonQuery();

            }
        }

     

        dbConn.Close();
        dbCmd.Cancel();


        #region  寄信通知主班團隊
        //KE20220929_排除大愛光老師
        //KE20250725_把收件人信箱HAccount改成HEmail欄位
        SqlDataReader QueryMail = SQLdatabase.ExecuteReader("SELECT c.HID,(SELECT DISTINCT(cast(b.HEmail AS NVARCHAR) + ',') FROM HCourse AS a CROSS APPLY SPLIT(a.HTeam, ',')  INNER JOIN HMember AS b ON value = b.HID WHERE  a.HID = c.HID AND b.HID NOT IN('9390') FOR XML PATH('')) AS Account FROM HCourse AS c WHERE c.HCBatchNum = '" + LB_HCBatchNum.Text + "'");


        if (QueryMail.Read())
        {
            if (!string.IsNullOrEmpty(QueryMail["Account"].ToString().TrimEnd(',')))
            {
                //信件本體宣告
                MailMessage mail = new MailMessage();
                // 寄件者, 收件者和副本郵件地址        
                mail.From = new MailAddress(Sender, "和氣大愛玉成系統自動寄信機器人");
                // 設定收件者
                mail.To.Add(QueryMail["HEmail"].ToString().TrimEnd(','));
                //mail.To.Add(QueryMail["Email"].ToString().TrimEnd(','));
                // 優先等級
                mail.Priority = MailPriority.Normal;
                // 主旨
                mail.Subject = "和氣大愛玉成系統【" + LB_HCourseName.Text + "】 - 退審通知";
                //信件內容         
                mail.Body = "<p style='display:block;'>親愛的主班團隊您好~</p><p style='display:block;'>以下課程：【" + LB_HCourseName.Text + "】被退審哦~</p><p style='display:block'>審核者意見如下：</p>" +
                    "<p style='display:block'>" + TB_HVOpinion.Text.Trim().Replace("\r\n", "<br/>") + "</p><br/><hr/><p style='font-weight:bold;'> 此郵件由和氣大愛玉成系統自動寄出，請勿回信。</p>";

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
                    Response.Write("<script>alert('課程審核存檔成功~並通知主班團隊課程退審~~');window.location.href='HCourseVerify.aspx';</script>");

                }
                catch (Exception ex)
                {
                    //寄信失敗
                    Response.Write("<script>alert('退審通知信寄送失敗！請確認主班團隊的信箱是否正確~');</script>");

                }
            }

        }
        QueryMail.Close();


        #endregion


        #region  寄信通知申請者
        //KE20250725_將HAccount改成HEmail欄位
        SqlDataReader QueryNotify = SQLdatabase.ExecuteReader("SELECT b.HEmail, b.HUsername FROM HCourse AS a  INNER JOIN HMember AS b ON a.HApplicant = b.HID WHERE a.HCBatchNum = '" + LB_HCBatchNum.Text + "'");


        if (QueryNotify.Read())
        {
            if (!string.IsNullOrEmpty(QueryNotify["HEmail"].ToString().TrimEnd(',')))
            {
                //信件本體宣告
                MailMessage mail = new MailMessage();
                // 寄件者, 收件者和副本郵件地址        
                mail.From = new MailAddress(Sender, "和氣大愛玉成系統自動寄信機器人");
                // 設定收件者
                mail.To.Add(QueryNotify["HEmail"].ToString().TrimEnd(','));
                //mail.To.Add(QueryMail["Email"].ToString().TrimEnd(','));
                // 優先等級
                mail.Priority = MailPriority.Normal;
                // 主旨
                mail.Subject = "和氣大愛玉成系統【" + LB_HCourseName.Text + "】 - 退審通知";
                //信件內容         
                mail.Body = "<p style='display:block;'>"+ QueryNotify["HUsername"].ToString() + " 您好~</p><p style='display:block;'>您申請的課程：【" + LB_HCourseName.Text + "】被退審哦~</p><p style='display:block'>退審原因如下：</p>" +
                    "<p style='display:block'>" + TB_HVOpinion.Text.Trim().Replace("\r\n", "<br/>") + "</p><br/><hr/><p style='font-weight:bold;'> 此郵件由和氣大愛玉成系統自動寄出，請勿回信。</p>";

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
                    Response.Write("<script>alert('已通知申請人課程退審~~');window.location.href='HCourseVerify.aspx';</script>");
                }
                catch (Exception ex)
                {
                    //寄信失敗
                    Response.Write("<script>alert('退審通知信寄送失敗！請確認申請人的信箱是否正確~');</script>");
                }
            }

        }
        QueryNotify.Close();


        #endregion



        Response.Write("<script>alert('退審存檔成功!');window.location.href='HCourseVerify.aspx';</script>");



    }
    #endregion

}