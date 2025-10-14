using HContains; //讓Contains不判斷大小寫
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


//讓Contains不判斷大小寫
namespace HContains
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}

public partial class Hochisystem : System.Web.UI.MasterPage
{

    protected void Page_Init(object sender, EventArgs e)
    {
        Session["CKUrl"] = "HWelcome.aspx";

        if (Session["HAccount"] != null && Session["HUserName"] != null && Session["HUserHID"] != null && Session["EIPUID"] != null)
        {
            if (Request.Cookies["HochiInfo"] != null)
            {
                LB_HAccount.Text = (Session["HAccount"].ToString() == "" || Session["HAccount"].ToString() == null) ? (Request.Cookies["HochiInfo"]["HAccount"].ToString() == "" || Request.Cookies["HochiInfo"]["HAccount"].ToString() == null) ? "" : Request.Cookies["HochiInfo"]["HAccount"].ToString() : Session["HAccount"].ToString();
                LB_HUserName.Text = (Session["HUserName"].ToString() == "" || Session["HUserName"].ToString() == null) ? (Request.Cookies["HochiInfo"]["HUserName"].ToString() == "" || Request.Cookies["HochiInfo"]["HUserName"].ToString() == null) ? "" : Request.Cookies["HochiInfo"]["HUserName"].ToString() : Session["HUserName"].ToString();
                LB_HUserHID.Text = (Session["HUserHID"].ToString() == "" || Session["HUserHID"].ToString() == null) ? (Request.Cookies["HochiInfo"]["HUserHID"].ToString() == "" || Request.Cookies["HochiInfo"]["HUserHID"].ToString() == null) ? "" : Request.Cookies["HochiInfo"]["HUserHID"].ToString() : Session["HUserHID"].ToString();
                //LB_EIPUID.Text = (Session["EIPUID"].ToString() == "" || Session["EIPUID"].ToString() == null) ? (Request.Cookies["HochiInfo"]["EIPUID"].ToString() == "" || Request.Cookies["HochiInfo"]["EIPUID"].ToString() == null) ? "" : Request.Cookies["HochiInfo"]["EIPUID"].ToString() : Session["EIPUID"].ToString();
            }
            else
            {
                LB_HAccount.Text = Session["HAccount"].ToString();
                LB_HUserName.Text = Session["HUserName"].ToString();
                LB_HUserHID.Text = Session["HUserHID"].ToString();
                //LB_EIPUID.Text = Session["EIPUID"].ToString();
            }
        }
        else
        {
            if (Request.Cookies["HochiInfo"] == null)
            {
                Response.Write("<script>alert('連線逾時，請重新登入!');window.location.href='../../HLogin.aspx';</script>");
            }
            else
            {
                LB_HAccount.Text = Request.Cookies["HochiInfo"]["HAccount"].ToString();
                Response.Cookies["HochiInfoHAccount"].Value = Request.Cookies["HochiInfo"]["HAccount"].ToString();
                Session["HAccount"] = Request.Cookies["HochiInfo"]["HAccount"].ToString();

                SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HUserName, HAccount, HEIPUid FROM HMember WHERE HAccount ='" + LB_HAccount.Text + "' AND HStatus=1");
                if (dr.Read())
                {
                    LB_HAccount.Text = dr["HAccount"].ToString();
                    LB_HUserName.Text = dr["HUserName"].ToString();
                    LB_HUserHID.Text = dr["HID"].ToString();

                    Request.Cookies["HochiInfo"]["HAccount"] = dr["HAccount"].ToString();
                    Request.Cookies["HochiInfo"]["HUserName"] = dr["HUserName"].ToString();
                    Request.Cookies["HochiInfo"]["HUserHID"] = dr["HID"].ToString();
                }
                dr.Close();

                //LB_HAccount.Text = Request.Cookies["HochiInfo"]["HAccount"].ToString();
                //LB_HUserName.Text = Request.Cookies["HochiInfo"]["HUserName"].ToString();
                //LB_HUserHID.Text = Request.Cookies["HochiInfo"]["HUserHID"].ToString();
                //LB_EIPUID.Text = Request.Cookies["HochiInfo"]["EIPUID"].ToString();

                Session["HAccount"] = Request.Cookies["HochiInfo"]["HAccount"].ToString();
                Session["HUserName"] = Request.Cookies["HochiInfo"]["HUserName"].ToString();
                Session["HUserHID"] = Request.Cookies["HochiInfo"]["HUserHID"].ToString();
                //Session["EIPUID"] = Request.Cookies["HochiInfo"]["EIPUID"].ToString();
            }
        }

    }




    protected void Page_Load(object sender, EventArgs e)
    {
        //顯示登入者
        LB_HName.Text = LB_HUserName.Text;

        if (!IsPostBack)
        {
            //1.開班審核
            SqlDataReader QueryCourseVerify = SQLdatabase.ExecuteReader("SELECT a.HID, b.HMemberID  FROM HCVerifyUnit AS a LEFT JOIN HRole AS b ON a.HVRoleHID = b.HID WHERE b.HMemberID LIKE '%," + LB_HUserHID.Text + ",%'");

            if (QueryCourseVerify.Read())
            {
                SDS_HCourseVerify.SelectCommand = "SELECT HCourseName, HApplyTime FROM HCourse WHERE HVerifyStatus=0 AND HStatus=1 GROUP BY HCourseName, HApplyTime  ORDER BY HApplyTime DESC";
            }
            QueryCourseVerify.Close();

            //2.請假審核(通知主班團隊)
            SqlDataReader QueryDayOffVerify = SQLdatabase.ExecuteReader("SELECT HID FROM HCourse WHERE ',' + HTeam LIKE '%," + LB_HUserHID.Text + ",%' AND LEFT(HDateRange,10) BETWEEN DATEADD(month, -1, GETDATE()) AND GETDATE() ");

            while (QueryDayOffVerify.Read())
            {

                SDS_HDayOffVerify.SelectCommand = "SELECT (b.HArea+'/'+b.HPeriod+' '+HUserName) AS HUsername, c.HCourseName , a.HAStatusDT FROM HRollCall AS a JOIN MemberList AS b ON a.HMemberID= b.HID LEFT JOIN HCourse AS c ON a.HCourseID = c.HID WHERE HCourseID='" + QueryDayOffVerify["HID"].ToString() + "'  AND LEFT(c.HDateRange,10) BETWEEN DATEADD(month, -1, GETDATE()) AND GETDATE()  AND HAStatus ='5' ORDER BY HAStatusDT DESC";
            }
            QueryDayOffVerify.Close();


            //3.人工退款(只有中心財務才會看到)
            if (LB_HUserHID.Text == "7676" || LB_HUserHID.Text == "9377")
            {
                SDS_HRefund.SelectCommand = "SELECT a.HOrderGroup, (b.HArea + '/' + b.HPeriod + ' ' + b.HUserName) AS HUserName,  a.HCXLTotal, a.HCXLApplyDate FROM HCourseBooking AS a LEFT JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HCXLHandleStatus IN (1) AND a.HPayMethod IN(2,3,4) AND (a.HCXLOrderGroup <>''AND a.HCXLOrderGroup IS NOT NULL) AND  (a.HCXLApplyDate <>'' AND a.HCXLApplyDate IS NOT NULL) AND HItemStatus<>'2' AND a.HCXLTotal <>0 GROUP BY a.HOrderGroup, b.HArea, b.HPeriod, b.HUserName , a.HPAmount, a.HPayMethod, a.HPayAmt, a.HStatus, a.HItemStatus, a.HCXLOrderGroup, a.HCXLApplyDate, a.HCXLTotal, a.HCXLHandleStatus ORDER BY a.HCXLApplyDate DESC";
            }

            //計算通知的數量
            int gNotifyNum = 0;
            Rpt_HCourseVerify.DataBind();
            Rpt_HDayOffVerify.DataBind();
            Rpt_HRefund.DataBind();

            gNotifyNum = Rpt_HCourseVerify.Items.Count + Rpt_HDayOffVerify.Items.Count + Rpt_HRefund.Items.Count;
            LB_NotifyNum.Text = gNotifyNum.ToString();
        }

    }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        //計算通知的數量
        int gNotifyNum = 0;

        gNotifyNum = Rpt_HCourseVerify.Items.Count + Rpt_HDayOffVerify.Items.Count + Rpt_HRefund.Items.Count;
        LB_NotifyNum.Text = gNotifyNum.ToString();
    }



    protected void Page_PreRender(object sender, EventArgs e)
    {

        if (Session["HAccount"] == null && Session["HUserName"] == null && Session["HUserHID"] == null && Session["EIPUID"] == null)
        {
            if (Request.Cookies["HochiInfo"] == null)
            {
                Response.Write("<script>alert('連線逾時，請重新登入!');window.location.href='../../HLogin.aspx';</script>");
            }
        }


        //Response.End();
        //string RS_UnAvailibility = (string)this.GetGlobalResourceObject("LangSource", "RS_UnAvailibility"); /*您沒有權限喔!*/


        //抓程式檔名→routing的狀況下
        System.Web.UI.Page oPage = (System.Web.UI.Page)System.Web.HttpContext.Current.CurrentHandler;
        string gPage1 = Path.GetFileName(oPage.AppRelativeVirtualPath);
        string gPage2 = Path.GetFileName(oPage.Request.PhysicalPath);
        string gPage = oPage.Request.CurrentExecutionFilePath;

        HtmlButton gBtn_Add = ((HtmlButton)ContentPlaceHolder1.FindControl("Btn_Add")) == null ? new HtmlButton() : ((HtmlButton)ContentPlaceHolder1.FindControl("Btn_Add")); //判斷submit按鈕是否要顯示
        Button gBtn_Submit = ((Button)ContentPlaceHolder1.FindControl("Btn_Submit")) == null ? new Button() : ((Button)ContentPlaceHolder1.FindControl("Btn_Submit")); //判斷submit按鈕是否要顯示
        Button gBtn_Allow = ((Button)ContentPlaceHolder1.FindControl("Btn_Allow")) == null ? new Button() : ((Button)ContentPlaceHolder1.FindControl("Btn_Allow")); //判斷Btn_Allow按鈕是否要顯示
        Button gBtn_Deny = ((Button)ContentPlaceHolder1.FindControl("Btn_Deny")) == null ? new Button() : ((Button)ContentPlaceHolder1.FindControl("Btn_Deny")); //判斷Btn_Deny按鈕是否要顯示

        Button gBtn_Cancel = ((Button)ContentPlaceHolder1.FindControl("Btn_Cancel")) == null ? new Button() : ((Button)ContentPlaceHolder1.FindControl("Btn_Cancel")); //判斷Btn_Cancel按鈕是否要顯示
        Panel gPanel_List = ((Panel)ContentPlaceHolder1.FindControl("Panel_List")) == null ? new Panel() : ((Panel)ContentPlaceHolder1.FindControl("Panel_List")); //判斷Btn_Cancel按鈕是否要顯示
        Panel gPanel_Edit = ((Panel)ContentPlaceHolder1.FindControl("Panel_Edit")) == null ? new Panel() : ((Panel)ContentPlaceHolder1.FindControl("Panel_Edit")); //判斷Btn_Cancel按鈕是否要顯示
        LinkButton gLBtn_HTMaterial = ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_HTMaterial")) == null ? new LinkButton() : ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_HTMaterial")); //判斷上傳按鈕是否要顯示

        LinkButton gLBtn_TransferCCPOrder = ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_TransferCCPOrder")) == null ? new LinkButton() : ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_TransferCCPOrder")); //判斷轉成信用卡授權訂單是否要顯示


        string gRpt_ID = "";
        //取得該頁面的所有控制項
        foreach (Control ctl in gPanel_List.Controls)
        {
            //如果是repeater，則取得ID
            if (ctl.GetType().ToString() == "System.Web.UI.WebControls.Repeater")
            {
                gRpt_ID = ctl.ID;
            }
        }

        Repeater gRpt = ((Repeater)ContentPlaceHolder1.FindControl(gRpt_ID)) == null ? new Repeater() : ((Repeater)ContentPlaceHolder1.FindControl(gRpt_ID));
        int gLBtn_Count = ((Repeater)ContentPlaceHolder1.FindControl(gRpt_ID)) == null ? 0 : ((Repeater)ContentPlaceHolder1.FindControl(gRpt_ID)).Items.Count;




        #region 檔案名稱
        //學員及權限管理-A
        bool AA_Add = gPage.Contains("/system/HMember_Add.aspx", StringComparison.OrdinalIgnoreCase);//會員管理
        bool AB_Add = gPage.Contains("/system/HPosition_Add.aspx", StringComparison.OrdinalIgnoreCase);//體系設定
        bool AC_Add = gPage.Contains("/system/HUsualTask_Add.aspx", StringComparison.OrdinalIgnoreCase);//常態任務管理
        bool AD_Add = gPage.Contains("/system/HCourseTask_Add.aspx", StringComparison.OrdinalIgnoreCase);//體系護持工作項目管理

        bool AA_Edit = gPage.Contains("/system/HMember_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool AB_Edit = gPage.Contains("/system/HPosition_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool AC_Edit = gPage.Contains("/system/HUsualTask_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool AD_Edit = gPage.Contains("/system/HCourseTask_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool AE_Edit = gPage.Contains("/system/HMentorRegister.aspx", StringComparison.OrdinalIgnoreCase); //護持者登記

        //檢覈模組-B
        bool BA_Add = gPage.Contains("/system/HTest_Add.aspx", StringComparison.OrdinalIgnoreCase);//檢覈項目管理
        bool BB_Add = gPage.Contains("/system/HQualify_Add.aspx", StringComparison.OrdinalIgnoreCase);//資格檢覈記錄
        //bool BC_Add = gPage.Contains("/system/HExamBase_Add.aspx", StringComparison.OrdinalIgnoreCase);//題庫管理
        //bool BD_Add = gPage.Contains("/system/HExamPaper_Add.aspx", StringComparison.OrdinalIgnoreCase);//考卷管理

        bool BA_Edit = gPage.Contains("/system/HTest_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool BB_Edit = gPage.Contains("/system/HQualify_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        //bool BC_Edit = gPage.Contains("/system/HExamBase_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        //bool BD_Edit = gPage.Contains("/system/HExamPaper_Edit.aspx", StringComparison.OrdinalIgnoreCase);

        //課程模組-C
        bool CA_Add = gPage.Contains("/system/HCourseTemplate_Add.aspx", StringComparison.OrdinalIgnoreCase);//課程範本管理
        bool CB_Add = gPage.Contains("/system/HCourse_Add.aspx", StringComparison.OrdinalIgnoreCase);//開課管理
        bool CC_Add = gPage.Contains("/system/HTeacher_Add.aspx", StringComparison.OrdinalIgnoreCase); //講師管理
                                                                                                       //bool CD_Add = gPage.Contains("/system/HCourseApply_Add.aspx", StringComparison.OrdinalIgnoreCase);//課程報名清單
                                                                                                       //bool CE_Add = gPage.Contains("/system/HCourseApply_Add.aspx", StringComparison.OrdinalIgnoreCase);//體系報名管理

        bool CA_Edit = gPage.Contains("/system/HCourseTemplate_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool CB_Edit = gPage.Contains("/system/HCourse_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool CC_Edit = gPage.Contains("/system/HTeacher_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool CD_Edit = gPage.Contains("/system/HCourseApply_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool CE_Edit = gPage.Contains("/system/HSystemApply_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool CF_Edit = gPage.Contains("/system/HACMaterial.aspx", StringComparison.OrdinalIgnoreCase);
        //GA20231227_加入前導課程管理
        bool CG_Edit = gPage.Contains("/system/HLCourseRelated_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        //GA20241218_加入套裝課程管理
        bool CH_Edit = gPage.Contains("/system/HCoursePackage_Edit.aspx", StringComparison.OrdinalIgnoreCase);



        //簽核管理-D
        bool DA_Add = gPage.Contains("/system/HCourseVerify.aspx", StringComparison.OrdinalIgnoreCase);//開班審核
        bool DB_Add = gPage.Contains("/system/HTransferVerify.aspx", StringComparison.OrdinalIgnoreCase);//轉區審核
        bool DC_Add = gPage.Contains("/system/HDayoffVerify.aspx", StringComparison.OrdinalIgnoreCase);//請假審核
        bool DD_Add = gPage.Contains("/system/HTeamTransferVerify.aspx", StringComparison.OrdinalIgnoreCase);//轉團審核

        bool DA_Edit = gPage.Contains("/system/HCourseVerify.aspx", StringComparison.OrdinalIgnoreCase);
        bool DB_Edit = gPage.Contains("/system/HTransferVerify.aspx", StringComparison.OrdinalIgnoreCase);
        bool DC_Edit = gPage.Contains("/system/HDayoffVerify.aspx", StringComparison.OrdinalIgnoreCase);
        bool DD_Edit = gPage.Contains("/system/HTeamTransferVerify.aspx", StringComparison.OrdinalIgnoreCase);//轉團審核

        //訂單管理-E
        //bool EZ_Add = gPage.Contains("/system/HOrder_Add.aspx", StringComparison.OrdinalIgnoreCase);//訂單管理
        bool EZ_Edit = gPage.Contains("/system/HOrder_Edit.aspx", StringComparison.OrdinalIgnoreCase);//訂單管理
        bool EA_Add = gPage.Contains("/system/HCourseBookingB.aspx", StringComparison.OrdinalIgnoreCase); //幫他人報名
        bool EB_Edit = gPage.Contains("/system/HRefund_Edit.aspx", StringComparison.OrdinalIgnoreCase); //幫他人報名
        bool EC_Add = gPage.Contains("/System/HCCPeriod_Add.aspx", StringComparison.OrdinalIgnoreCase); //信用卡授權審核管理/信用卡定期定額授權審核管理
        bool EC_Edit = gPage.Contains("/System/HCCPeriodVerify.aspx", StringComparison.OrdinalIgnoreCase); //信用卡授權審核管理/信用卡定期定額授權審核管理
        bool ED_Edit = gPage.Contains("/System/HCCPeriodOrder_Edit.aspx", StringComparison.OrdinalIgnoreCase); //信用卡授權訂單

        //點數管理-F
        bool FZ_Add = gPage.Contains("/system/HPoints.aspx", StringComparison.OrdinalIgnoreCase);//點數管理
        bool FZ_Edit = gPage.Contains("/system/HPoints.aspx", StringComparison.OrdinalIgnoreCase);

        //點名管理-G
        //bool GZ_Add = gPage.Contains("/system/HRollCall_Add.aspx", StringComparison.OrdinalIgnoreCase);//點名管理
        bool GZ_Edit = gPage.Contains("/system/HRollCall_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool GZ_Edit1 = gPage.Contains("/system/HCourseAttend.aspx", StringComparison.OrdinalIgnoreCase);
        bool GZ_Edit2 = gPage.Contains("/system/HSupportAttend.aspx", StringComparison.OrdinalIgnoreCase);


        //玉成報表-H
        bool HA_Edit = gPage.Contains("/system/HEnrollment.aspx", StringComparison.OrdinalIgnoreCase);  //單一課程報名總名單
        bool HB_Edit = gPage.Contains("/system/HEnrollmentAll.aspx", StringComparison.OrdinalIgnoreCase);  //同課程同日期報名總名單
        bool HC_Edit = gPage.Contains("/system/HCSupportList.aspx", StringComparison.OrdinalIgnoreCase);  //班會護持分工表
        bool HD_Edit = gPage.Contains("/system/HStayList.aspx", StringComparison.OrdinalIgnoreCase);  //住宿登記表
        bool HE_Edit = gPage.Contains("/system/HSupportTimeList.aspx", StringComparison.OrdinalIgnoreCase);  //體系護持分工時間表
        bool HF_Edit = gPage.Contains("/system/HEnrollAnalysis.aspx", StringComparison.OrdinalIgnoreCase);  //一對一護持者總名單
        bool HG_Edit = gPage.Contains("/system/HApplyRecord.aspx", StringComparison.OrdinalIgnoreCase);  //單一課程參班紀錄分析
        bool HH_Edit = gPage.Contains("/system/HMCTeamList.aspx", StringComparison.OrdinalIgnoreCase);  //班會光團連線名單
        bool HJ_Edit = gPage.Contains("/system/HAMCTeamList.aspx", StringComparison.OrdinalIgnoreCase);  //區光團連線名單
        bool HI_Edit = gPage.Contains("/system/HQAnalysis.aspx", StringComparison.OrdinalIgnoreCase);  //問卷統計分析表
        bool HL_Edit = gPage.Contains("/system/HApplyRecordSame.aspx", StringComparison.OrdinalIgnoreCase);  //同課程同日期參班紀錄分析
        bool HM_Edit = gPage.Contains("/system/HQAnalysisSame.aspx", StringComparison.OrdinalIgnoreCase);  //同課程同日期問卷回饋總表
        bool HO_List = gPage.Contains("/system/HCourseAttendRec.aspx", StringComparison.OrdinalIgnoreCase);  //課程出席紀錄表
      
        //QA20231107_新增表
        bool HP_List = gPage.Contains("/system/HApplyGList.aspx", StringComparison.OrdinalIgnoreCase);  //參班分析總表
        bool HQ_List = gPage.Contains("/system/HPaymentStatus.aspx", StringComparison.OrdinalIgnoreCase);  //各班繳費狀況表
       
        //GA20241218_新增課程護持出席紀錄表
        bool HR_List = gPage.Contains("/system/HCourseSupportRec.aspx", StringComparison.OrdinalIgnoreCase);  //課程護持出席紀錄表

        //GA20250402_新增各區與光團及導師階層總覽&無區屬或無光團階層總覽
        bool HS_ListA = gPage.Contains("/system/HOrganization.aspx", StringComparison.OrdinalIgnoreCase);  //各區與光團及導師階層總覽
        bool HS_ListB = gPage.Contains("/system/HNoTeamArea.aspx", StringComparison.OrdinalIgnoreCase);  //無區屬或無光團階層總覽

        //GA20250830_新增開課明細與人數統計表&學員報名參班狀況統計表
        bool HT_List = gPage.Contains("/system/HCPCountingReport.aspx", StringComparison.OrdinalIgnoreCase);  //開課明細與人數統計表
        bool HW_List = gPage.Contains("/system/HMCBRecord.aspx", StringComparison.OrdinalIgnoreCase);  //學員報名參班狀況統計表

        //GA20250625_匯出單一課程報名單
        bool HZZ_Edit = gPage.Contains("/system/HBBDateEnrollment.aspx", StringComparison.OrdinalIgnoreCase);  //匯出單一課程報名總名單(跟著HA)


        //財務報表-H
        bool HK_Edit = gPage.Contains("/system/HOrderReport.aspx", StringComparison.OrdinalIgnoreCase);  //訂單明細總表
        bool HN_Edit = gPage.Contains("/system/HAnnualReport.aspx", StringComparison.OrdinalIgnoreCase);  //年度各月各課程大表
        bool HX_Edit = gPage.Contains("/system/HCCPeriodReport.aspx", StringComparison.OrdinalIgnoreCase);  //信用卡授權交易總表


        //消息管理-I
        bool IA_Add = gPage.Contains("/system/HNews_Add.aspx", StringComparison.OrdinalIgnoreCase);//最新消息
        bool IB_Add = gPage.Contains("/system/HActivity_Add.aspx", StringComparison.OrdinalIgnoreCase);//課程影音

        bool IA_Edit = gPage.Contains("/system/HNews_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool IB_Edit = gPage.Contains("/system/HActivity_Edit.aspx", StringComparison.OrdinalIgnoreCase);


        //問卷管理-J
        bool JZ_Add = gPage.Contains("/system/HQuestion_Add.aspx", StringComparison.OrdinalIgnoreCase);//問卷管理
        bool JZ_Edit = gPage.Contains("/system/HQuestion_Edit.aspx", StringComparison.OrdinalIgnoreCase);

        //系統管理-K
        bool KA_Add = gPage.Contains("/system/HSlider_Add.aspx", StringComparison.OrdinalIgnoreCase);//首頁輪播圖
        bool KB_Add = gPage.Contains("/system/HParameter.aspx", StringComparison.OrdinalIgnoreCase);//參數設定
        bool KC_Add = gPage.Contains("/system/HLArea_Add.aspx", StringComparison.OrdinalIgnoreCase);//大區管理
        bool KD_Add = gPage.Contains("/system/HArea_Add.aspx", StringComparison.OrdinalIgnoreCase);//區管理
        bool KE_Add = gPage.Contains("/system/HMTeam_Add.aspx", StringComparison.OrdinalIgnoreCase);//母光團管理
        bool KF_Add = gPage.Contains("/system/HCTeam_Add.aspx", StringComparison.OrdinalIgnoreCase);//光團(小組)管理
        bool KG_Add = gPage.Contains("/system/HDiscountCode_Add.aspx", StringComparison.OrdinalIgnoreCase);//折扣碼管理
        bool KH_Add = gPage.Contains("/system/HDCGroup_Add.aspx", StringComparison.OrdinalIgnoreCase);//折扣碼群組管理
        bool KJ_Add = gPage.Contains("/system/HFParameter.aspx", StringComparison.OrdinalIgnoreCase);//捐款參數設定
        bool KK_Add = gPage.Contains("/system/HCCRegular.aspx", StringComparison.OrdinalIgnoreCase);//定期定額專區

        bool KA_Edit = gPage.Contains("/system/HSlider_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool KB_Edit = gPage.Contains("/system/HParameter.aspx", StringComparison.OrdinalIgnoreCase);
        bool KC_Edit = gPage.Contains("/system/HLArea_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool KD_Edit = gPage.Contains("/system/HArea_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool KE_Edit = gPage.Contains("/system/HMTeam_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool KF_Edit = gPage.Contains("/system/HCTeam_Edit.aspx", StringComparison.OrdinalIgnoreCase);
        bool KG_Edit = gPage.Contains("/system/HDiscountCode_Edit.aspx", StringComparison.OrdinalIgnoreCase);//折扣碼管理
        bool KH_Edit = gPage.Contains("/system/HDCGroup_Edit.aspx", StringComparison.OrdinalIgnoreCase);//折扣碼群組管理
        bool KI_Edit = gPage.Contains("/system/HCourseLink_Edit.aspx", StringComparison.OrdinalIgnoreCase);//課程ZOOM管理
        bool KL_Edit = gPage.Contains("/system/HBudgetType_Edit.aspx", StringComparison.OrdinalIgnoreCase);//折扣碼管理
        bool KJ_Edit = gPage.Contains("/system/HFParameter.aspx", StringComparison.OrdinalIgnoreCase);//捐款參數設定
        bool KK_Edit = gPage.Contains("/system/HCCRegular.aspx", StringComparison.OrdinalIgnoreCase);//定期定額專區


        //講師教材-L
        bool LA_Add = gPage.Contains("/system/HTMaterial_Add.aspx", StringComparison.OrdinalIgnoreCase);//講師教材管理

        bool LA_Edit = gPage.Contains("/system/HTMaterial_Edit.aspx", StringComparison.OrdinalIgnoreCase);//講師教材管理
        bool LB_Edit = gPage.Contains("/system/HTMaterial_View.aspx", StringComparison.OrdinalIgnoreCase);//講師教材瀏覽專區

        //QE20240814_財務報表-M(原為預算執行報表-M獨立功能，現已併入財務報表-H)
        bool MA_List = gPage.Contains("/system/HFPaymentStatus.aspx", StringComparison.OrdinalIgnoreCase);  //預算執行報表


        //專欄設定-N
        bool NA_Edit = gPage.Contains("/system/HSCParameter.aspx", StringComparison.OrdinalIgnoreCase);
        bool NB_Edit = gPage.Contains("/system/HSCCourseSetting.aspx", StringComparison.OrdinalIgnoreCase);
        bool NC_Edit = gPage.Contains("/system/HSCRuleTemplate.aspx", StringComparison.OrdinalIgnoreCase);
        bool ND_Edit = gPage.Contains("/system/HSCModeratorSetting.aspx", StringComparison.OrdinalIgnoreCase);
        //GA20250830_新增專欄MENU設定
        bool NE_Edit = gPage.Contains("/system/HSCForumClassSetting.aspx", StringComparison.OrdinalIgnoreCase);








        bool WelcomePage = gPage.Contains("/System/HWelcome.aspx", StringComparison.OrdinalIgnoreCase);//歡迎頁面



        #endregion

        SqlConnection dbConn;
        SqlCommand dbCmd;
        string strDBConn, strSQL;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();
        //當有天命法位和常態任務兩種的時候會合併權限
        strSQL = "SELECT HRAccess FROM (SELECT HMemberID, (SELECT DISTINCT(',' + HRAccess) FROM HRole WHERE HMemberID like '%," + LB_HUserHID.Text + ",%' FOR XML PATH('')) AS HRAccess FROM HRole AS b) M WHERE M.HMemberID like '%," + LB_HUserHID.Text + ",%' GROUP BY HRAccess";

        dbCmd = new SqlCommand(strSQL, dbConn);
        SqlDataReader MyQuery;
        MyQuery = dbCmd.ExecuteReader();


        #region 權限
        if (MyQuery.Read())
        {

            //學員及權限管理-A
            if ((AA_Add == true || AA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("AA") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("AA2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("AA2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("AA2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示

                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("AA2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((AA_Add == true || AA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("AA") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            else if ((AB_Add == true || AB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("AB") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("AB2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("AB2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("AB2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("AB2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((AB_Add == true || AB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("AB") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((AC_Add == true || AC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("AC") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("AC2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("AC2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("AC2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("AC2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                    gRpt.Items[i].FindControl("LBtn_Join").Visible = MyQuery["HRAccess"].ToString().IndexOf("AC2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Join為不顯示
                }
            }
            else if ((AC_Add == true || AC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("AC") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((AD_Add == true || AD_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("AD") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("AD2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("AD2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("AD2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("AD2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((AD_Add == true || AD_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("AD") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((AE_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("AE") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("AE2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("AE2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("AE2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("AE2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((AE_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("AE") < 0)
            {
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            //檢覈模組-B
            else if ((BA_Add == true || BA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("BA") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("BA2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("BA2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("BA2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("BA2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((BA_Add == true || BA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("BA") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            else if ((BB_Add == true || BB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("BB") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("BB2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("BB2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("BB2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("BB2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((BB_Add == true || BB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("BB") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            //else if ((BC_Add == true || BC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("BC") >= 0)
            //{
            //    gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("BC2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
            //    gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("BC2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
            //    gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("BC2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
            //    for (int i = 0; i < gLBtn_Count; i++)
            //    {
            //        gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("BC2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
            //    }
            //}
            //else if ((BC_Add == true || BC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("BC") < 0)
            //{
            //    //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
            //    Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
            //    return;
            //}

            //else if ((BD_Add == true || BD_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("BD") >= 0)
            //{
            //    gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("BD2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
            //    gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("BD2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
            //    gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("BD2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
            //    for (int i = 0; i < gLBtn_Count; i++)
            //    {
            //        gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("BD2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
            //    }
            //}
            //else if ((BD_Add == true || BD_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("BD") < 0)
            //{
            //    //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
            //    Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
            //    return;
            //}

            //課程模組-C
            else if ((CA_Add == true || CA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CA") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("CA2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("CA2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Allow.Visible = MyQuery["HRAccess"].ToString().IndexOf("CA2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Allow為不顯示
                gBtn_Deny.Visible = MyQuery["HRAccess"].ToString().IndexOf("CA2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Deny為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("CA2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("CA2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }

            }
            else if ((CA_Add == true || CA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CA") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((CB_Add == true || CB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CB") >= 0)
            {
                //gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("CB2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                if (gBtn_Submit.Visible == true)
                {
                    gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("CB2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                }
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("CB2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Allow.Visible = MyQuery["HRAccess"].ToString().IndexOf("CB2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Allow為不顯示
                gBtn_Deny.Visible = MyQuery["HRAccess"].ToString().IndexOf("CB2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Deny為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("CB2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("CB2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((CB_Add == true || CB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CB") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((CC_Add == true || CC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CC") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("CC2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("CC2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Allow.Visible = MyQuery["HRAccess"].ToString().IndexOf("CC2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Allow為不顯示
                gBtn_Deny.Visible = MyQuery["HRAccess"].ToString().IndexOf("CC2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Deny為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("CC2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("CC2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((CC_Add == true || CC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CC") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((CD_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CD") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("CD2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("CD2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Allow.Visible = MyQuery["HRAccess"].ToString().IndexOf("CD2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Allow為不顯示
                gBtn_Deny.Visible = MyQuery["HRAccess"].ToString().IndexOf("CD2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Deny為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("CD2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("CD2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((CD_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CD") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((CE_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CE") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("CE2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("CE2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Allow.Visible = MyQuery["HRAccess"].ToString().IndexOf("CE2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Allow為不顯示
                gBtn_Deny.Visible = MyQuery["HRAccess"].ToString().IndexOf("CE2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Deny為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("CE2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("CE2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((CE_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CE") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((CF_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CF") >= 0)
            {
                LinkButton gLBtn_UP_Add = ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_UP_Add")) == null ? new LinkButton() : ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_UP_Add")); //判斷動態新增的按鈕是否要顯示

                gLBtn_UP_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("CF2") >= 0 ? true : false; //如果沒有寫入權限，則LBtn_UP_Add為不顯示

            }
            else if ((CF_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CF") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            //GA20231227_加入前導課程管理權限
            else if ((CG_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CG") >= 0)
            {
                LinkButton gLBtn_HLeadingCourse_Add = ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_HLeadingCourse_Add")) == null ? new LinkButton() : ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_HLeadingCourse_add")); //判斷動態新增的按鈕是否要顯示
                LinkButton gLBtn_HLeadingCourse_Del = ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_HLeadingCourse_Del")) == null ? new LinkButton() : ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_HLeadingCourse_Del")); //判斷動態新增的按鈕是否要顯示

                gLBtn_HLeadingCourse_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("CG2") >= 0 ? true : false; //如果沒有寫入權限，則不顯示
                gLBtn_HLeadingCourse_Del.Visible = MyQuery["HRAccess"].ToString().IndexOf("CG2") >= 0 ? true : false; //如果沒有寫入權限，則不顯示

                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("CG2") >= 0 ? true : false; //如果沒有寫入權限，則不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Disabled").Visible = MyQuery["HRAccess"].ToString().IndexOf("CG2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }



                //string gRptDetail_ID = "";
                ////取得該頁面的所有控制項
                //foreach (Control ctl in gPanel_Edit.Controls)
                //{
                //	//如果是repeater，則取得ID
                //	if (ctl.GetType().ToString() == "System.Web.UI.WebControls.Repeater")
                //	{
                //		gRptDetail_ID = ctl.ID;
                //	}
                //}

                //Repeater gRptDetail = ((Repeater)ContentPlaceHolder1.FindControl(gRptDetail_ID)) == null ? new Repeater() : ((Repeater)ContentPlaceHolder1.FindControl(gRpt_ID));
                //int gRptDetailLBtn_Count = ((Repeater)ContentPlaceHolder1.FindControl(gRptDetail_ID)) == null ? 0 : ((Repeater)ContentPlaceHolder1.FindControl(gRptDetail_ID)).Items.Count;

                //Response.Write("AA="+ gRptDetail_ID);
                //Response.End();
                //for (int i = 0; i < gRptDetailLBtn_Count; i++)
                //{
                //	gRptDetail.Items[i].FindControl("LBtn_HLeadingCourse_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("CG2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                //}

                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("CG2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
            }
            else if ((CG_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CG") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            //GA20241218_加入套裝課程管理權限
            else if ((CH_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CH") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("CH2") >= 0 ? true : false; //如果沒有寫入權限，則不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("CH2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
            }
            else if ((CH_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("CH") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }


            //簽核管理-D
            else if ((DA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("DA") >= 0)
            {
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("DA2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Allow.Visible = MyQuery["HRAccess"].ToString().IndexOf("DA2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Allow為不顯示
                gBtn_Deny.Visible = MyQuery["HRAccess"].ToString().IndexOf("DA2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Deny為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("DA2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("DA2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((DA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("DA") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((DB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("DB") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("DB2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("DB2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Allow.Visible = MyQuery["HRAccess"].ToString().IndexOf("DB2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Allow為不顯示
                gBtn_Deny.Visible = MyQuery["HRAccess"].ToString().IndexOf("DB2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Deny為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("DB2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("DB2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((DB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("DB") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((DC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("DC") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("DC2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("DC2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Allow.Visible = MyQuery["HRAccess"].ToString().IndexOf("DC2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Allow為不顯示
                gBtn_Deny.Visible = MyQuery["HRAccess"].ToString().IndexOf("DC2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Deny為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("DC2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("DC2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((DC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("DC") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((DD_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("DD") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("DD2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("DD2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Allow.Visible = MyQuery["HRAccess"].ToString().IndexOf("DD2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Allow為不顯示
                gBtn_Deny.Visible = MyQuery["HRAccess"].ToString().IndexOf("DD2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Deny為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("DD2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("DD2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((DD_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("DD") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            //訂單模組-E
            //訂單管理-EZ
            else if ((EZ_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("EZ") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("EZ2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("EZ2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("EZ2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
            }
            else if ((EZ_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("EZ") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            //人工退款訂單管理
            else if ((EB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("EB") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("EB2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("EB2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("EB2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
            }
            else if ((EB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("EB") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            //幫他人報名-EA
            else if ((EA_Add == true) && MyQuery["HRAccess"].ToString().IndexOf("EA") >= 0)
            {
                LinkButton gLBtn_Checkout = ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_Checkout")) == null ? new LinkButton() : ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_Checkout")); //判斷submit按鈕是否要顯示
                gLBtn_Checkout.Visible = MyQuery["HRAccess"].ToString().IndexOf("EA2") >= 0 ? true : false; //如果沒有寫入權限，則LBtn_Checkout按鈕為不顯示
            }
            else if ((EA_Add == true) && MyQuery["HRAccess"].ToString().IndexOf("EA") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            //GA20230811_新增信用卡定期定額權限
            //信用卡授權審核管理/信用卡定期定額審核管理-EC
            else if ((EC_Add == true || EC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("EC") >= 0)
            {
                gBtn_Allow.Visible = MyQuery["HRAccess"].ToString().IndexOf("EC2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Deny.Visible = MyQuery["HRAccess"].ToString().IndexOf("EC2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gLBtn_TransferCCPOrder.Visible = MyQuery["HRAccess"].ToString().IndexOf("EC2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示

                Button gBtn_Invalid = ((Button)ContentPlaceHolder1.FindControl("Btn_Invalid")) == null ? new Button() : ((Button)ContentPlaceHolder1.FindControl("Btn_Invalid")); //判斷Btn_Deny按鈕是否要顯示
                gBtn_Invalid.Visible = MyQuery["HRAccess"].ToString().IndexOf("EC2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
            }
            else if ((EC_Add == true || EC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("EC") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            //信用卡授權訂單-ED
            else if ((ED_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("ED") >= 0)
            {
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("ED2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示

                LinkButton gLBtn_Export = ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_Export")) == null ? new LinkButton() : ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_Export")); //判斷轉成信用卡授權訂單是否要顯示
                LinkButton gLBtn_Import = ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_Import")) == null ? new LinkButton() : ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_Import")); //判斷轉成信用卡授權訂單是否要顯示


                gLBtn_Export.Visible = MyQuery["HRAccess"].ToString().IndexOf("ED2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gLBtn_Import.Visible = MyQuery["HRAccess"].ToString().IndexOf("ED2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
            }
            else if ((ED_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("ED") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            //點數管理-F
            else if ((FZ_Add == true || FZ_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("FZ") >= 0)
            {
            }
            else if ((FZ_Add == true || FZ_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("FZ") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            //點名管理-G
            else if ((GZ_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("GZ") >= 0)
            {
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("GZ2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("GZ2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                                                                                                            //for (int i = 0; i < gLBtn_Count; i++)
                                                                                                            //{
                                                                                                            //	gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("GZ2") >= 0 ? false : false;//不顯示刪除按鈕
                                                                                                            //}
            }
            else if ((GZ_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("GZ") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((GZ_Edit1 == true) && MyQuery["HRAccess"].ToString().IndexOf("GZ") >= 0)
            {

            }
            else if ((GZ_Edit1 == true) && MyQuery["HRAccess"].ToString().IndexOf("GZ") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((GZ_Edit2 == true) && MyQuery["HRAccess"].ToString().IndexOf("GZ") >= 0)
            {

            }
            else if ((GZ_Edit2 == true) && MyQuery["HRAccess"].ToString().IndexOf("GZ") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }


            //報表分析-H  --尚未完成
            else if ((HA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HA") >= 0)
            {
            }
            else if ((HA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HA") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((HB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HB") >= 0)
            {

            }
            else if ((HB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HB") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((HC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HC") >= 0)
            {

            }
            else if ((HC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HC") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((HD_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HD") >= 0)
            {

            }
            else if ((HD_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HD") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((HE_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HE") >= 0)
            {

            }
            else if ((HE_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HE") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((HF_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HF") >= 0)
            {

            }
            else if ((HF_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HF") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((HG_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HG") >= 0)
            {

            }
            else if ((HG_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HG") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((HH_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HH") >= 0)
            {

            }
            else if ((HH_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HH") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((HJ_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HJ") >= 0)
            {

            }
            else if ((HJ_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HJ") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((HI_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HI") >= 0)
            {

            }
            else if ((HI_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HI") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((HK_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HK") >= 0)
            {

            }
            else if ((HK_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HK") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((HN_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HN") >= 0)
            {

            }
            else if ((HN_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HN") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((HL_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HL") >= 0)
            {

            }
            else if ((HL_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HL") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((HM_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HM") >= 0)
            {

            }
            else if ((HM_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HM") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            //QA20231107_新增權限設定(未完成-待確認)
            //ME20231107_課程出席紀錄表權限
            else if ((HO_List == true) && MyQuery["HRAccess"].ToString().IndexOf("HO") >= 0)
            {

            }
            else if ((HO_List == true) && MyQuery["HRAccess"].ToString().IndexOf("HO") < 0)
            {
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            //ME20231219_參班分析總表：第二階段新增功能所以暫時註解。
            //else if ((HP_List == true) && MyQuery["HRAccess"].ToString().IndexOf("HP") >= 0)
            //{

            //}
            //else if ((HP_List == true) && MyQuery["HRAccess"].ToString().IndexOf("HP") < 0)
            //{
            //    //Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
            //    //return;
            //}
            //匯出單一課程報名單(跟著單一課程報名總名單)
            else if ((HZZ_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HA") >= 0)
            {
            }
            else if ((HZZ_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HA") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            //ME20231219_各班繳費狀況表
            else if ((HQ_List == true) && MyQuery["HRAccess"].ToString().IndexOf("HQ") >= 0)
            {

            }
            else if ((HQ_List == true) && MyQuery["HRAccess"].ToString().IndexOf("HQ") < 0)
            {
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            //GA20241218_課程護持出席紀錄表
            else if ((HQ_List == true) && MyQuery["HRAccess"].ToString().IndexOf("HQ") >= 0)
            {

            }
            else if ((HQ_List == true) && MyQuery["HRAccess"].ToString().IndexOf("HQ") < 0)
            {
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            //GA20250402_各區與光團及導師階層總覽&無區屬與無光團階層總覽
            else if ((HS_ListA == true) && MyQuery["HRAccess"].ToString().IndexOf("HS") >= 0)
            {
            }
            else if ((HS_ListA == true) && MyQuery["HRAccess"].ToString().IndexOf("HS") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((HS_ListB == true) && MyQuery["HRAccess"].ToString().IndexOf("HS") >= 0)
            {
            }
            else if ((HS_ListB == true) && MyQuery["HRAccess"].ToString().IndexOf("HS") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }


            //GA20250830
            else if ((HT_List == true) && MyQuery["HRAccess"].ToString().IndexOf("HT") >= 0)
            {

            }
            else if ((HT_List == true) && MyQuery["HRAccess"].ToString().IndexOf("HT") < 0)
            {
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            else if ((HX_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HX") >= 0)
            {

            }
            else if ((HX_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("HX") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            else if ((HW_List == true) && MyQuery["HRAccess"].ToString().IndexOf("HW") >= 0)
            {

            }
            else if ((HW_List == true) && MyQuery["HRAccess"].ToString().IndexOf("HW") < 0)
            {
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }



            //消息管理-I  
            else if ((IA_Add == true || IA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("IA") >= 0)    //最新消息
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("IA2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("IA2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("IA2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("IA2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((IA_Add == true || IA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("IA") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((IB_Add == true || IB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("IB") >= 0)  //課程影音
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("IB2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("IB2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("IB2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("IB2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((IB_Add == true || IB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("IB") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            //問卷管理-J
            else if ((JZ_Add == true || JZ_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("JZ") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("JZ2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("JZ2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("JZ2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("JZ2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((JZ_Add == true || JZ_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("JZ") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            //系統管理-K
            else if ((KA_Add == true || KA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KA") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("KA2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("KA2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("KA2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("KA2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((KA_Add == true || KA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KA") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((KB_Add == true || KB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KB") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("KB2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("KB2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("KB2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("KB2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((KB_Add == true || KB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KB") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((KC_Add == true || KC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KC") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("KC2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("KC2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Allow.Visible = MyQuery["HRAccess"].ToString().IndexOf("KC2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Allow為不顯示
                gBtn_Deny.Visible = MyQuery["HRAccess"].ToString().IndexOf("KC2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Deny為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("KC2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("KC2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((KC_Add == true || KC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KC") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((KD_Add == true || KD_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KD") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("KD2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("KD2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("KD2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("KD2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((KD_Add == true || KD_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KD") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((KE_Add == true || KE_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KE") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("KE2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("KE2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("KE2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("KE2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((KE_Add == true || KE_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KE") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((KF_Add == true || KF_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KF") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("KF2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("KF2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("KF2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("KF2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((KF_Add == true || KF_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KF") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((KG_Add == true || KG_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KG") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("KG2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("KG2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("KG2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("KG2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((KG_Add == true || KG_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KG") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((KH_Add == true || KH_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KH") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("KH2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("KH2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("KH2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("KH2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((KH_Add == true || KH_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KH") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((KI_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KI") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("KI2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("KI2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                                                                                                            //gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("KI2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示

            }
            else if ((KI_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KI") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((KL_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KL") >= 0)
            {
                ((LinkButton)ContentPlaceHolder1.FindControl("LBtn_HBudgetType_Add")).Visible = MyQuery["HRAccess"].ToString().IndexOf("KL2") >= 0 ? true : false;
                //for (int i = 0; i < gLBtn_Count; i++)
                //{
                //	gRpt.Items[i].FindControl("LBtn_Disabled").Visible = MyQuery["HRAccess"].ToString().IndexOf("KL2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                //	gRpt.Items[i].FindControl("LBtn_Enabled").Visible = MyQuery["HRAccess"].ToString().IndexOf("KL2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                //}
            }
            else if ((KL_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KL") < 0)
            {
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }


            //MA20240105_捐款參數設定&定期定額專區
            else if ((KJ_Add == true || KB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KJ") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("KJ2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("KJ2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("KJ2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("KJ2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((KJ_Add == true || KJ_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KJ") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((KK_Add == true || KK_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KK") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("KK2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("KK2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("KK2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                //for (int i = 0; i < gLBtn_Count; i++)
                //{
                //    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("KK2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                //}
            }
            else if ((KK_Add == true || KK_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("KK") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }




            //講師教材-L
            else if ((LA_Add == true || LA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("LA") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("LA2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("LA2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("LA2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                gLBtn_HTMaterial.Visible = MyQuery["HRAccess"].ToString().IndexOf("LA2") >= 0 ? true : false;    //如果沒有寫入權限，則LBtn_HTMaterial為不顯示

                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("LA2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }

            }
            else if ((LA_Add == true || LA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("LA") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((LB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("LB") >= 0)
            {

            }
            else if ((LB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("LB") < 0)
            {
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }



            //財務報表-M(原為預算執行報表-M獨立功能，現已併入財務報表-H)
            // EE20231219_預算執行報表
            else if ((MA_List == true) && MyQuery["HRAccess"].ToString().IndexOf("MA") >= 0)
            {

            }
            else if ((MA_List == true) && MyQuery["HRAccess"].ToString().IndexOf("MA") < 0)
            {
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }















            //專欄設定-N
            if ((NA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("NA") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("NA2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("NA2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("NA2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示

                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("NA2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((NA_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("NA") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            else if ((NB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("NB") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("NB2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("NB2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("NB2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("NB2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                }
            }
            else if ((NB_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("NB") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((NC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("NC") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("NC2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("NC2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("NC2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                for (int i = 0; i < gLBtn_Count; i++)
                {
                    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("NC2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                    gRpt.Items[i].FindControl("LBtn_Join").Visible = MyQuery["HRAccess"].ToString().IndexOf("NC2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Join為不顯示
                }
            }
            else if ((NC_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("NC") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }
            else if ((ND_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("ND") >= 0)
            {
                gBtn_Add.Visible = MyQuery["HRAccess"].ToString().IndexOf("ND2") >= 0 ? true : false; //如果沒有寫入權限，則Btn_Add按鈕為不顯示
                gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("ND2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("ND2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                //for (int i = 0; i < gLBtn_Count; i++)
                //{
                //    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("ND2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                //}
            }
            else if ((ND_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("ND") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }

            else if ((NE_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("NE") >= 0)
            {
                //gBtn_Submit.Visible = MyQuery["HRAccess"].ToString().IndexOf("ND2") >= 0 ? true : false;    //如果沒有寫入權限，則Submit為不顯示
                //gBtn_Cancel.Visible = MyQuery["HRAccess"].ToString().IndexOf("ND2") >= 0 ? true : false;    //如果沒有寫入權限，則Btn_Cancel為不顯示
                ////for (int i = 0; i < gLBtn_Count; i++)
                ////{
                ////    gRpt.Items[i].FindControl("LBtn_Del").Visible = MyQuery["HRAccess"].ToString().IndexOf("ND2") >= 0 ? true : false;//如果沒有寫入權限，則Repeater裡面的LBtn_Del為不顯示
                ////}
            }
            else if ((NE_Edit == true) && MyQuery["HRAccess"].ToString().IndexOf("NE") < 0)
            {
                //Response.Write("<script>alert('您沒有權限喔!');window.history.go(-1);</script>");
                Response.Write("<script>alert('您沒有權限喔!');window.location.href='HWelcome.aspx'</script>");
                return;
            }


























            else if (WelcomePage == true)
            {

            }
            else
            {
                //Response.Write("<script>alert('頁面權限末設定!');window.location.href='HWelcome.aspx'</script>");
            }
        }
        else
        {
            Response.Write("<script>alert('您沒有權限喔!');window.location.href='../HIndex.aspx'</script>");
        }
        MyQuery.Close();
        dbCmd.Cancel();
        dbConn.Close();
        dbConn.Dispose();
        #endregion

























    }



    #region 登出功能
    protected void LBtn_LogOut_Click(object sender, EventArgs e)
    {
        Response.Write("<script>window.location.href='../../HLogout.aspx';</script>");
    }
    #endregion

    #region 開班審核通知資料繫結
    protected void Rpt_HCourseVerify_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;
        ((Label)e.Item.FindControl("LB_HNotifyContent")).Text = gDRV["HCourseName"].ToString();

    }
    #endregion

    #region 請假審核通知資料繫結
    protected void Rpt_HDayOffVerify_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;
        ((Label)e.Item.FindControl("LB_HNotifyContent")).Text = gDRV["HUsername"].ToString() + gDRV["HCourseName"].ToString();
    }
    #endregion

    #region 人工退款通知資料繫結
    protected void Rpt_HRefund_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;
        ((Label)e.Item.FindControl("LB_HNotifyContent")).Text = gDRV["HUsername"].ToString() + gDRV["HOrderGroup"].ToString() + "_退款總金額：" + Convert.ToDouble(gDRV["HCXLTotal"].ToString()).ToString("N0") + "元";
    }
    #endregion
}
