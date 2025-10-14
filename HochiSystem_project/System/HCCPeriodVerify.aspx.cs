using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Text;
using iTextSharp.text.pdf;
using System.IO;
using DocumentFormat.OpenXml.Drawing.Charts;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.HSSF.Util;
using NPOI.XSSF.Streaming;
using System.Data.OleDb;
using System.Diagnostics;
using System.Web;
using ECPay.Payment.Integration.SPCheckOut;
using System.Net.Mail;



//MA20230807_新增信用卡定期定額授權審核管理頁面
public partial class System_HCCPeriodVerify : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    #region 寄件資訊<正式>
    public string Sender = MailConfig.Sender;
    public string EAcount = MailConfig.Account;
    public string EPasword = MailConfig.Password;
    public string EHost = MailConfig.Host;
    public int EPort = MailConfig.Port;
    public bool EEnabledSSL = MailConfig.EnableSSL;
    #endregion

    protected void Page_PreRenderComplete(object sender, EventArgs e)
    {


        //if ((LB_MHVerifyStatus.Text == "審核通過" || LB_MHVerifyStatus.Text == "審核不通過") && LB_MHStatus.Text == "有效")
        //ME20230927_修改判斷
        //GA20240119_加入停用狀態判斷
        //ME20241010_新增授權書狀態: 0=停用/中止、1=有效、2=作廢、3=新增申請中、4=變更申請中、5=提前付清、6=付訖終止 (已付清)、7=換單
        if ((LB_MHVerifyStatus.Text == "審核通過" && LB_MHStatus.Text == "有效" && LB_MHOrderStatus.Text == "已成立") || LB_MHStatus.Text == "付訖終止 " || LB_MHStatus.Text == "提前付清")
        {
            Btn_Allow.Visible = false;
            Btn_Deny.Visible = false;
            Btn_Invalid.Visible = false;
            Btn_Valid.Visible = false;
            Btn_Save.Visible = true;
            //LBtn_Allow.Enabled = false;
            //LBtn_Deny.Enabled = false;
            //LBtn_Invalid.Enabled = false;

        }
        else if (LB_MHStatus.Text == "作廢")
        {
            //若申請書已作廢則不能再進行審核
            Btn_Allow.Visible = false;
            Btn_Deny.Visible = false;
            Btn_Invalid.Visible = false;
            Btn_Valid.Visible = true;
            Btn_Save.Visible = true;
            Btn_Allow.CssClass = "btn btn-success";
            Btn_Deny.CssClass = "btn btn-info";
            Btn_Valid.CssClass = "btn btn-success";
        }
        else if (LB_MHStatus.Text == "有效" && LB_MHOrderStatus.Text == "未成立" || LB_MHStatus.Text == "新增申請中" && LB_MHOrderStatus.Text == "未成立" || LB_MHStatus.Text == "變更申請中" && LB_MHOrderStatus.Text == "未成立" || LB_MHStatus.Text == "提前付清" && LB_MHOrderStatus.Text == "未成立")
        {
            Btn_Allow.Visible = true;
            Btn_Deny.Visible = true;
            Btn_Invalid.Visible = true;
            Btn_Valid.Visible = false;
            Btn_Save.Visible = false;
            Btn_Allow.CssClass = "btn btn-success";
            Btn_Deny.CssClass = "btn btn-info";
            Btn_Valid.CssClass = "btn btn-success";
        }
        else if (LB_MHStatus.Text == "有效")
        {
            Btn_Allow.Visible = true;
            Btn_Deny.Visible = true;
            Btn_Invalid.Visible = true;
            Btn_Valid.Visible = false;
            Btn_Save.Visible = false;
            Btn_Allow.CssClass = "btn btn-success";
            Btn_Deny.CssClass = "btn btn-info";
            Btn_Valid.CssClass = "btn btn-success";
        }
        else if (LB_MHStatus.Text == "停用" || LB_MHStatus.Text == "停用/中止")
        {
            Btn_Allow.Visible = false;
            Btn_Deny.Visible = false;
            Btn_Invalid.Visible = false;
            Btn_Valid.Visible = true;
            Btn_Save.Visible = true;
            Btn_Allow.CssClass = "btn btn-success";
            Btn_Deny.CssClass = "btn btn-info";
            Btn_Valid.CssClass = "btn btn-success";
        }

    }

    #region 分頁copy-1
    private int PageMax = 15;   //分頁最大顯示數量
    private int LastPage = 10;  //分頁數量
    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        #region 分頁copy-2

        if (!IsPostBack)
        {
            ViewState["Search"] = "";

            //ME20241011_新增欄位HOriCCPOrderCode(原始授權書單號)、HPartialPayTimes(部分付款/提前付清期數)、HPartialPayAmount(部分付款/提前付清金額) & 調整排序
            SDS_HCCPeriod.SelectCommand = "SELECT  a.HID, a.HCCPeriodCode, a.HMemberID, a.HCourseName, a.HDUserName, a.HDPhone, a.HDonor, a.HCardNum, a.HDTotal, a.HDCCPTimes,  a.HDCCPAmount, (a.HDCCPSDate+'-'+a.HDCCPEDate) AS HDCCPDateRange, a.HVerifyStatus,a.HOrderStatus, a.HStatus, (b.HArea + '/' + b.HPeriod + ' ' + b.HUserName) AS UserName, HOriCCPOrderCode, HPartialPayTimes, HPartialPayAmount FROM HCCPeriod  AS a LEFT JOIN MemberList AS b ON a.HMemberID = b.HID WHERE  HPaperYN='1' ORDER BY a.HCreateDT DESC, a.HModifyDT DESC";

            Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCPeriod.SelectCommand, PageMax, LastPage, false, Rpt_HCCPeriod);
            ViewState["Search"] = SDS_HCCPeriod.SelectCommand;
        }
        else
        {
            SDS_HCCPeriod.SelectCommand = ViewState["Search"].ToString();
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", ViewState["Search"].ToString(), PageMax, LastPage, false, Rpt_HCCPeriod);
        }
        #endregion
    }

    #region 轉成信用卡授權訂單功能
    protected void LBtn_TransferCCPOrder_Click(object sender, EventArgs e)
    {
        //判斷有選
        int SelectNum = 0;

        //MA20230927_加入判斷:轉成授權訂單不能點選&審核狀態
        int SuccessNum = 0;
        int VerifyNum = 0;


        for (int x = 0; x < Rpt_HCCPeriod.Items.Count; x++)
        {
            if (((CheckBox)Rpt_HCCPeriod.Items[x].FindControl("CB_Select")).Checked == true)
            {
                SelectNum++;

                if (((Label)Rpt_HCCPeriod.Items[x].FindControl("LB_HOrderStatus")).Text == "已成立")
                {
                    SuccessNum++;
                }
                else if (((Label)Rpt_HCCPeriod.Items[x].FindControl("LB_HVerifyStatus")).Text != "審核通過")
                {
                    VerifyNum++;
                }
            }
        }




        //for (int x = 0; x < Rpt_HCCPeriod.Items.Count; x++)
        //{
        //	if (((CheckBox)Rpt_HCCPeriod.Items[x].FindControl("CB_Select")).Checked == true)
        //	{
        //		SelectNum++;
        //	}
        //}

        if (SelectNum != 0 && VerifyNum == 0 && SuccessNum == 0)
        {
            string HID = "";
            //先寫入HCCPOrder，取得授權訂單代碼
            string strInsHC = "IF EXISTS(SELECT HCCPOrderCode FROM HCCPOrder WHERE SUBSTRING(HCCPOrderCode,2,8) = CONVERT(nvarchar, getdate(), 112)) " +
                     "BEGIN " +
                         "insert into HCCPOrder (HCCPOrderCode, HStatus, HCreate, HCreateDT, HPaperYN) values ( 'P' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HCCPOrderCode), 12)) FROM HCCPOrder) + 1),'1','" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','1');SELECT SCOPE_IDENTITY() AS HID " +
                     "END " +
                     "ELSE " +
                     "BEGIN " +
                         "insert into HCCPOrder (HCCPOrderCode, HStatus, HCreate, HCreateDT, HPaperYN) values ('P' + CONVERT(nvarchar, getdate(), 112) + '0001','1','" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','1');SELECT SCOPE_IDENTITY() AS HID " +
                     "END";


            SqlConnection dbConn = default(SqlConnection);
            SqlCommand dbCmd = default(SqlCommand);
            string strDBConn = null;
            strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
            dbConn = new SqlConnection(strDBConn);
            dbConn.Open();
            dbCmd = new SqlCommand(strInsHC, dbConn);
            HID = dbCmd.ExecuteScalar().ToString();


            //判斷哪些有勾，再寫入HCCPOrderDetail
            for (int x = 0; x < Rpt_HCCPeriod.Items.Count; x++)
            {
                if (((CheckBox)Rpt_HCCPeriod.Items[x].FindControl("CB_Select")).Checked == true)
                {
                    string HCCPeriodCode = ((Label)Rpt_HCCPeriod.Items[x].FindControl("LB_HCCPeriodCode")).Text;

                    //加入資料庫
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);
                    SqlCommand cmd = new SqlCommand("INSERT INTO HCCPOrderDetail (HCCPOrderID, HCCPeriodCode,HMemberID, HCTemplateID, HCourseID, HCourseName, HDateRange, HDUserName, HDEmail, HDPostal, HDAddress, HDTel, HDPhone, HDBirth, HDPersonID, HDReceiptSType, HDonor, HCardHolder, HCHPersonID, HCardNum, HCardBank, HCardType, HCVCCode, HCardValidDate, HDTotal, HDCCPTimes, HDCCPAmount, HDCCPSDate, HDCCPEDate, HCHPhone, HDAPublic, HRemark, HVerifyStatus, HStatus, HCreate, HCreateDT, HPaperYN) SELECT '" + HID + "','" + HCCPeriodCode + "',HMemberID, HCTemplateID, HCourseID, HCourseName, HDateRange, HDUserName,HDEmail, HDPostal, HDAddress, HDTel, HDPhone, HDBirth, HDPersonID, HDReceiptSType, HDonor, HCardHolder,HCHPersonID, HCardNum, HCardBank, HCardType, HCVCCode, HCardValidDate, HDTotal, HDCCPTimes, HDCCPAmount,HDCCPSDate, HDCCPEDate, HCHPhone, HDAPublic, HRemark, HVerifyStatus, HStatus,HCreate,  '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '1' FROM HCCPeriod WHERE HCCPeriodCode='" + HCCPeriodCode + "'", con);

                    //更新授權書狀態(HOrderStatus)：1=未成立、2=已成立
                    SqlCommand cmd2 = new SqlCommand("UPDATE HCCPeriod SET HOrderStatus ='2' WHERE HCCPeriodCode ='" + HCCPeriodCode + "'", con);


                    con.Open();
                    cmd.ExecuteNonQuery();
                    cmd2.ExecuteNonQuery();
                    con.Close();
                    cmd.Cancel();
                }
            }
            dbConn.Close();
            dbCmd.Cancel();

            Response.Write("<script>alert('轉成信用卡授權訂單~');window.location.href='HCCPeriodVerify.aspx';</script>");
        }
        else
        {
            if (VerifyNum > 0)
            {
                Response.Write("<script>alert('訂單尚未審核通過，請先點選批次審核通過~!');</script>");
                return;
            }
            else
            {
                Response.Write("<script>alert('請先勾選要轉成授權訂單的項目哦~!');</script>");
                return;
            }

        }





    }
    #endregion

    #region 批次審核通過
    protected void LBtn_BatchAllow_Click(object sender, EventArgs e)
    {
        //判斷有選
        int SelectNum = 0;
        //MA20230927_加入判斷:轉成授權訂單不能點選&審核狀態
        int SuccessNum = 0;
        int VerifyNum = 0;


        for (int x = 0; x < Rpt_HCCPeriod.Items.Count; x++)
        {
            if (((CheckBox)Rpt_HCCPeriod.Items[x].FindControl("CB_Select")).Checked == true)
            {
                SelectNum++;

                if (((Label)Rpt_HCCPeriod.Items[x].FindControl("LB_HOrderStatus")).Text == "已成立")
                {
                    SuccessNum++;
                }
                else if (((Label)Rpt_HCCPeriod.Items[x].FindControl("LB_HVerifyStatus")).Text != "審核通過")
                {
                    VerifyNum++;
                }
            }
        }

        if (SelectNum > 0)
        {
            //判斷哪些有勾，UPDATE審核狀態&日期
            for (int x = 0; x < Rpt_HCCPeriod.Items.Count; x++)
            {
                if (((CheckBox)Rpt_HCCPeriod.Items[x].FindControl("CB_Select")).Checked == true)
                {
                    string HCCPeriodCode = ((Label)Rpt_HCCPeriod.Items[x].FindControl("LB_HCCPeriodCode")).Text;

                    //更新資料庫
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

                    //更新授權書狀態(HVerifyStatus)：1=未送審、2=審核中、3=審核通過、4=審核不通過
                    SqlCommand cmd = new SqlCommand("UPDATE HCCPeriod SET HVerifyStatus ='3',HVerifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPeriodCode ='" + HCCPeriodCode + "'", con);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    cmd.Cancel();
                }
            }

            Response.Write("<script>alert('批次審核通過完成~');window.location.href='HCCPeriodVerify.aspx';</script>");
        }
        else
        {
            Response.Write("<script>alert('請先勾選要批次審核通過的項目哦~!');</script>");
            return;
        }


    }
    #endregion

    #region 搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        //string gName = null;
        //ME20241011_新增欄位HOriCCPOrderCode(原始授權書單號)、HPartialPayTimes(部分付款/提前付清期數)、HPartialPayAmount(部分付款/提前付清金額)&調整排序
        string Select = "SELECT  a.HID, a.HCCPeriodCode, a.HMemberID, a.HCourseName, a.HDUserName, a.HDPhone, a.HDonor, a.HCardNum, a.HDTotal, a.HDCCPTimes,  a.HDCCPAmount, (a.HDCCPSDate+'-'+a.HDCCPEDate) AS HDCCPDateRange, a.HVerifyStatus,a.HOrderStatus, a.HStatus, (b.HArea + '/' + b.HPeriod + ' ' + b.HUserName) AS UserName, HOriCCPOrderCode, HPartialPayTimes, HPartialPayAmount FROM HCCPeriod AS a LEFT JOIN MemberList AS b ON a.HMemberID = b.HID WHERE HPaperYN='1' ";

        //搜尋條件
        StringBuilder sql = new StringBuilder(Select);
        List<string> WHERE = new List<string>();
        if (!string.IsNullOrEmpty(TB_SKeyword.Text.Trim()))
        {
            //ME20241007_新增條件:查詢捐款項目
            WHERE.Add(" (a.HDUserName LIKE N'%" + TB_SKeyword.Text.Trim() + "%' OR b.HUserName LIKE N'%" + TB_SKeyword.Text.Trim() + "%' OR a.HCCPeriodCode LIKE N'%" + TB_SKeyword.Text.Trim() + "%' OR a.HCourseName LIKE N'%" + TB_SKeyword.Text.Trim() + "%'  )");
        }

        if (DDL_SHVerifyStatus.SelectedValue != "0")
        {
            WHERE.Add(" a.HVerifyStatus='" + DDL_SHVerifyStatus.SelectedValue + "'");
        }

        if (DDL_SHOrderStatus.SelectedValue != "0")
        {
            WHERE.Add(" a.HOrderStatus='" + DDL_SHOrderStatus.SelectedValue + "'");
        }

        if (DDL_SHStatus.SelectedValue != "99")
        {
            WHERE.Add(" a.HStatus='" + DDL_SHStatus.SelectedValue + "'");
        }

        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" AND " + wh + " ORDER BY a.HCreateDT DESC, a.HModifyDT DESC");
        }
        else
        {
            sql.Append("  ORDER BY a.HCreateDT DESC, a.HModifyDT DESC");

        }
        

        SDS_HCCPeriod.SelectCommand = sql.ToString();
        ViewState["Search"] = SDS_HCCPeriod.SelectCommand;


        //呼叫分頁 (連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 搜尋開啟, DataList控件)
        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCPeriod.SelectCommand, PageMax, LastPage, true, Rpt_HCCPeriod);
    }
    #endregion

    #region 取消搜尋功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_SKeyword.Text = null;
        DDL_SHVerifyStatus.SelectedValue = "0";
        DDL_SHOrderStatus.SelectedValue = "0";
        DDL_SHStatus.SelectedValue = "99";

        //ME20241011_新增欄位HOriCCPOrderCode(原始授權書單號)、HPartialPayTimes(部分付款/提前付清期數)、HPartialPayAmount(部分付款/提前付清金額)&調整排序
        SDS_HCCPeriod.SelectCommand = "SELECT  a.HID, a.HCCPeriodCode, a.HMemberID, a.HCourseName, a.HDUserName, a.HDPhone, a.HDonor, a.HCardNum, a.HDTotal, a.HDCCPTimes,  a.HDCCPAmount, (a.HDCCPSDate+'-'+a.HDCCPEDate) AS HDCCPDateRange, a.HVerifyStatus, a.HOrderStatus,a.HStatus, (b.HArea + '/' + b.HPeriod + ' ' + b.HUserName) AS UserName, HOriCCPOrderCode, HPartialPayTimes, HPartialPayAmount FROM HCCPeriod AS a LEFT JOIN MemberList AS b ON a.HMemberID = b.HID  WHERE  HPaperYN='1'   ORDER BY a.HCreateDT DESC, a.HModifyDT DESC"; 


        ViewState["Search"] = SDS_HCCPeriod.SelectCommand;
        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCPeriod.SelectCommand, PageMax, LastPage, true, Rpt_HCCPeriod);
    }
    #endregion

    #region 審核通過功能
    protected void Btn_Allow_Click(object sender, EventArgs e)
    {

        //ME20241010_新增授權書狀態: 0=停用/中止、1=有效、2=作廢、3=新增申請中、4=變更申請中、5=提前付清、6=付訖終止 (已付清)、7=換單
        string gCCPeriodHStatus = "1";
        string gHOrderGroup = "";  //部分付款或結清的訂單代碼

        //MA20241011_新增判斷，若是變更申請的資料，並有勾選部分付款或提前付清，則會幫同修建立新的訂單，並mail通知付款
        if (LB_HOriCCPeriodCode.Text != "0")
        {
            if (LB_HPartialPayAmount.Text != "0")
            {
                //變更申請的單當審核通過一樣紀錄為有效
                gCCPeriodHStatus = "1";

                #region 寫入dbo.訂單管理
                string strInsHQ = "IF EXISTS(SELECT HOrderGroup FROM HCourseBooking WHERE SUBSTRING(HOrderGroup,2,8) = CONVERT(nvarchar, getdate(), 112)) " +
                                  "BEGIN " +
                                  "INSERT INTO HCourseBooking (HCourseID,HCTemplateID, HCourseName, HDateRange,HMemberID, HOrderGroup, HOrderNum, HPMethod, HAttend, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HPAmount, HSubTotal, HPayAmt, HInvoiceType, HInvoiceStatus, HItemStatus, HStatus,HChangeStatus, HCourseDonate,HCreate, HCreateDT, HRemark, HRoom, HRoomTime, HDCode, HDPoint) VALUES (@HCourseID, @HCTemplateID, @HCourseName, @HDateRange,@HMemberID, 'F' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HOrderGroup), 12)) FROM HCourseBooking WHERE HOrderGroup LIKE '%F%' ) + 1),'B' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HOrderNum), 12)) FROM HCourseBooking) + 1), @HPMethod, @HAttend, @HLodging, @HBDate, @HLDate, @HLCourse, @HLCourseName, @HLDiscount, @HCGuide, @HPayMethod, @HPoint, @HMemberGroup, @HPAmount, @HSubTotal, @HPayAmt , @HInvoiceType, @HInvoiceStatus, @HItemStatus, @HStatus, @HChangeStatus, @HCourseDonate, @HCreate, @HCreateDT, @HRemark,  @HRoom, @HRoomTime, @HDCode, @HDPoint); SELECT SCOPE_IDENTITY() AS HID " +
                                  "END " +
                                  "ELSE BEGIN " +
                                  "INSERT INTO HCourseBooking (HCourseID, HCTemplateID, HCourseName, HDateRange,HMemberID, HOrderGroup,HOrderNum, HPMethod, HAttend, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HPAmount, HSubTotal, HPayAmt,HInvoiceType, HInvoiceStatus, HItemStatus, HStatus,HChangeStatus,HCourseDonate, HCreate, HCreateDT, HRemark, HRoom, HRoomTime, HDCode, HDPoint) VALUES (@HCourseID, @HCTemplateID, @HCourseName, @HDateRange, @HMemberID,'F' + CONVERT(nvarchar, getdate(), 112) + '0001', 'B' + CONVERT(nvarchar, getdate(), 112) + '0001', @HPMethod, @HAttend, @HLodging, @HBDate, @HLDate, @HLCourse, @HLCourseName, @HLDiscount, @HCGuide, @HPayMethod, @HPoint, @HMemberGroup, @HPAmount, @HSubTotal, @HPayAmt, @HInvoiceType, @HInvoiceStatus, @HItemStatus, @HStatus, @HChangeStatus, @HCourseDonate, @HCreate, @HCreateDT, @HRemark,  @HRoom, @HRoomTime, @HDCode, @HDPoint); SELECT SCOPE_IDENTITY() AS HID " +
                                  "END";


                SqlCommand cmd = new SqlCommand(strInsHQ, con);

                con.Open();
                //產生訂單編號（編號規則：英文字"B"+西元年月日+四碼流水號，例如：B202107150001)
                cmd.Parameters.AddWithValue("@HCourseID", "0"); //0=捐款
                cmd.Parameters.AddWithValue("@HCTemplateID", LB_HCTemplateID.Text);//99=血脈轉輪
                cmd.Parameters.AddWithValue("@HCourseName", LB_HTitle.Text);
                cmd.Parameters.AddWithValue("@HDateRange", LB_HDateRange.Text);
                cmd.Parameters.AddWithValue("@HMemberID", LB_HMemberID.Text);
                cmd.Parameters.AddWithValue("@HPMethod", "1");  //1=基金會
                cmd.Parameters.AddWithValue("@HAttend", "0");
                cmd.Parameters.AddWithValue("@HLodging", "0");
                cmd.Parameters.AddWithValue("@HBDate", "");
                cmd.Parameters.AddWithValue("@HLDate", "");
                cmd.Parameters.AddWithValue("@HLCourse", "");
                cmd.Parameters.AddWithValue("@HLCourseName", "");
                cmd.Parameters.AddWithValue("@HLDiscount", 0);
                cmd.Parameters.AddWithValue("@HCGuide", "");
                cmd.Parameters.AddWithValue("@HPayMethod", "1");  //先預設為信用卡
                cmd.Parameters.AddWithValue("@HPoint", 112000);
                cmd.Parameters.AddWithValue("@HMemberGroup", "");
                cmd.Parameters.AddWithValue("@HPAmount", 112000);
                cmd.Parameters.AddWithValue("@HSubTotal", 112000);
                cmd.Parameters.AddWithValue("@HPayAmt", 112000);
                cmd.Parameters.AddWithValue("@HInvoiceType", "1");
                cmd.Parameters.AddWithValue("@HInvoiceStatus", "0");
                cmd.Parameters.AddWithValue("@HItemStatus", "2");  //2=未付款
                cmd.Parameters.AddWithValue("@HStatus", "1");
                cmd.Parameters.AddWithValue("@HChangeStatus", "2");
                cmd.Parameters.AddWithValue("@HCourseDonate", "1");  //1=捐款
                cmd.Parameters.AddWithValue("@HCreate", "系統自動建立");
                cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@HRemark", "定期定額部份付款或結清");
                cmd.Parameters.AddWithValue("@HRoom", "0");
                cmd.Parameters.AddWithValue("@HRoomTime", "");
                cmd.Parameters.AddWithValue("@HDCode", "");
                cmd.Parameters.AddWithValue("@HDPoint", "");


                string HID = "";
                HID = cmd.ExecuteScalar().ToString();
                con.Close();
                cmd.Cancel();


                //取得訂單代碼
                SqlDataReader QueryOrderGroup = SQLdatabase.ExecuteReader("SELECT HOrderGroup FROM HCourseBooking WHERE HID =' " + HID + "'");
                if (QueryOrderGroup.Read())
                {
                    gHOrderGroup = QueryOrderGroup["HOrderGroup"].ToString();
                }
                QueryOrderGroup.Close();
                #endregion

                #region 通知同修
                #region 寄信
                //信件本體宣告
                MailMessage mail = new MailMessage();
                // 寄件者, 收件者和副本郵件地址        
                mail.From = new MailAddress(Sender, "和氣大愛玉成系統自動寄信機器人");
                // 設定收件者
                mail.To.Add("testmail@gwange.com");
                // 優先等級
                mail.Priority = MailPriority.Normal;
                // 主旨
                mail.Subject = "和氣大愛玉成系統 - 信用卡定期定額部分付款/提前結清繳費通知";
                //信件內容         
                mail.Body = "<p>" + TB_HDUserName.Text.Trim() + "，您好~ <br/>您申請信用卡定期定額部份付款或提前付清已審核通過囉~! <br/>以下為待付款之訂單代碼：" + gHOrderGroup + "， <br/>請於7天內盡速登入EDU系統或點擊以下連結：【<a href='http://127.0.0.1:1001/HMember_Order.aspx?U=1'>尚未付款訂單</a>】完成繳費哦~!</p>" +
                    "<p style='color:#dc3545'>請留意7天內若未完成付款，系統將復原授權書狀態，謝謝~!</p>" +
             "<hr/>" +
             "<p style='font-weight:bold;'>此郵件為和氣大愛玉成系統自動寄出，請勿回信。</p>";

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
                    ScriptManager.RegisterStartupScript(Page, GetType(), "alert", "alert('報名完成囉');windows.location.href='HOrder_Edit.aspx", true);

                    //Response.Write("<script>alert('課程送審成功~並通知審核單位盡快做開班審核囉~');window.location.href='HCourse_Edit.aspx';</script>");

                }
                catch (Exception ex)
                {
                    //寄驗證信失敗
                    Response.Write("<script>alert('寄驗證信失敗！請確認信箱是否正確');</script>");

                }
                #endregion
                #endregion
            }
            else
            {
                gCCPeriodHStatus = "1";
            }

        }

        //審核狀態(HVerifyStatus)：0=請選擇、1=未送審、2=送審中、3=審核通過、4=審核不通過
        //ME20241016_新增欄位:HRemark(備註)
        SQLdatabase.ExecuteNonQuery("UPDATE HCCPeriod SET HRemark='"+ TB_HRemark.Text.Trim() + "', HStatus='" + gCCPeriodHStatus + "', HVerifyStatus='3', HOrderGroup='" + gHOrderGroup + "', HVerifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',  HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPeriodCode='" + LB_MHCCPeriodCode.Text + "'");



        Response.Write("<script>alert('授權書審核通過完成~');window.location.href='HCCPeriodVerify.aspx';</script>");

    }
    #endregion

    #region 審核不通過功能
    protected void Btn_Deny_Click(object sender, EventArgs e)
    {
        //審核狀態(HVerifyStatus)：0=請選擇、1=未送審、2=送審中、3=審核通過、4=審核不通過
        //ME20241016_新增欄位:HRemark(備註)
        SQLdatabase.ExecuteNonQuery("UPDATE HCCPeriod SET  HRemark='"+ TB_HRemark.Text.Trim() + "',HVerifyStatus='4', HVerifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "',  HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPeriodCode='" + LB_MHCCPeriodCode.Text + "'");

        Response.Write("<script>alert('授權書審核不通過完成~');window.location.href='HCCPeriodVerify.aspx';</script>");
    }
    #endregion

    #region 作廢功能
    protected void Btn_Invalid_Click(object sender, EventArgs e)
    {
        //狀態(HStatus)：0=停用、1=啟用、2=作廢
        SQLdatabase.ExecuteNonQuery("UPDATE HCCPeriod SET HStatus='2', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPeriodCode='" + LB_MHCCPeriodCode.Text + "'");

        Response.Write("<script>alert('授權書作廢完成~');window.location.href='HCCPeriodVerify.aspx';</script>");
    }
    #endregion

    #region 啟用功能
    protected void Btn_Valid_Click(object sender, EventArgs e)
    {
        //狀態(HStatus)：0=停用、1=啟用、2=作廢
        SQLdatabase.ExecuteNonQuery("UPDATE HCCPeriod SET HStatus='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPeriodCode='" + LB_MHCCPeriodCode.Text + "'");

        Response.Write("<script>alert('授權書啟用完成~');window.location.href='HCCPeriodVerify.aspx';</script>");
    }
    #endregion

    #region 取消審核功能
    //protected void Btn_Cancel_Click(object sender, EventArgs e)
    //{
    //    Response.Redirect("HCCPeriodVerify.aspx");
    //}
    #endregion

    #region 查看授權書功能
    protected void LBtn_View_Click(object sender, EventArgs e)
    {
        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        //開啟Modal
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_HCCPeriod').modal('show');</script>", false);

        //ME20241010_新增欄位:HOriCCPOrderCode(原授權書單號), HPartialPayTimes(部分付款/提前結清期數), HPartialPayAmount(部分付款/提前結清付款金額)
        SqlDataReader QueryCCPeriod = SQLdatabase.ExecuteReader("SELECT a.HCCPeriodCode, a.HMemberID, a.HCTemplateID, a.HCourseID, a.HCourseName, a.HDateRange, a.HDUserName, a.HDEmail, a.HDPostal, a.HDAddress, a.HDTel, a.HDPhone, a.HDBirth, a.HDPersonID, a.HDReceiptSType, a.HDonor, a.HCardHolder, a.HCHPersonID, a.HRemark, a.HCardNum, a.HCardBank, a.HCardType, a.HCVCCode, a.HCardValidDate, a.HDTotal, a.HDCCPTimes, a.HDCCPAmount,a.HDCCPSDate, a.HDCCPEDate, a.HCHPhone, a.HDAPublic, a.HVerifyStatus, a.HOrderStatus,a.HStatus,  a.HOriCCPOrderCode, a.HPartialPayTimes, a.HPartialPayAmount, a.HOrderGroup, b.HItemStatus, b.HStatus AS BookingStatus  FROM HCCPeriod AS a LEFT JOIN HCourseBooking AS b ON a.HOrderGroup=b.HOrderGroup WHERE HCCPeriodCode='" + ((Label)RI.FindControl("LB_HCCPeriodCode")).Text + "'");

        if (QueryCCPeriod.Read())
        {
            //MA20241011_新增需要儲存的資料
            LB_HMemberID.Text = QueryCCPeriod["HMemberID"].ToString();
            LB_HCourseID.Text = QueryCCPeriod["HCourseID"].ToString();
            LB_HCTemplateID.Text = QueryCCPeriod["HCTemplateID"].ToString();
            LB_HDateRange.Text = QueryCCPeriod["HDateRange"].ToString();



            LB_HTitle.Text = QueryCCPeriod["HCourseName"].ToString();
            LB_MHCCPeriodCode.Text = QueryCCPeriod["HCCPeriodCode"].ToString();
            TB_HDUserName.Text = QueryCCPeriod["HDUserName"].ToString();
            TB_HDEmail.Text = QueryCCPeriod["HDEmail"].ToString();
            TB_HDTel.Text = QueryCCPeriod["HDTel"].ToString();
            TB_HDPhone.Text = QueryCCPeriod["HDPhone"].ToString();
            TB_HDBirth.Text = QueryCCPeriod["HDBirth"].ToString();
            TB_HDPersonID.Text = QueryCCPeriod["HDPersonID"].ToString();
            TB_HDPostal.Text = QueryCCPeriod["HDPostal"].ToString();
            TB_HDAddress.Text = QueryCCPeriod["HDAddress"].ToString();
            TB_HRemark.Text = QueryCCPeriod["HRemark"].ToString();
            RBL_HDReceiptSType.SelectedValue = !string.IsNullOrEmpty(QueryCCPeriod["HDReceiptSType"].ToString()) ? QueryCCPeriod["HDReceiptSType"].ToString() : "1";

            //GA20230815_必填顯示
            if (RBL_HDReceiptSType.SelectedValue == "2")
            {
                Span_HDPersonID.Visible = true;
            }
            else
            {
                Span_HDPersonID.Visible = false;
            }

            TB_HDonor.Text = QueryCCPeriod["HDonor"].ToString();
            TB_HCardHolder.Text = QueryCCPeriod["HCardHolder"].ToString();
            TB_HCHPersonID.Text = QueryCCPeriod["HCHPersonID"].ToString();
            TB_HCHPhone.Text = QueryCCPeriod["HCHPhone"].ToString();
            TB_HCardBank.Text = QueryCCPeriod["HCardBank"].ToString();
            TB_HCardNum1.Text = QueryCCPeriod["HCardNum"].ToString().Split('-')[0].ToString();
            TB_HCardNum2.Text = QueryCCPeriod["HCardNum"].ToString().Split('-')[1].ToString();
            TB_HCardNum3.Text = QueryCCPeriod["HCardNum"].ToString().Split('-')[2].ToString();
            TB_HCardNum4.Text = QueryCCPeriod["HCardNum"].ToString().Split('-')[3].ToString();
            TB_HCVCCode.Text = QueryCCPeriod["HCVCCode"].ToString();
            TB_HCardValidDateM.Text = QueryCCPeriod["HCardValidDate"].ToString().Split('/')[0].ToString();
            TB_HCardValidDateY.Text = QueryCCPeriod["HCardValidDate"].ToString().Split('/')[1].ToString();
            TB_HDTotal.Text = QueryCCPeriod["HDTotal"].ToString();
            TB_HDCCPTimes.Text = QueryCCPeriod["HDCCPTimes"].ToString();
            TB_HDCCPAmount.Text = QueryCCPeriod["HDCCPAmount"].ToString();
            TB_HDCCPSDate.Text = QueryCCPeriod["HDCCPSDate"].ToString();
            TB_HDCCPEDate.Text = QueryCCPeriod["HDCCPEDate"].ToString();
            CB_HDAPublic.Checked = !string.IsNullOrEmpty(QueryCCPeriod["HDAPublic"].ToString()) && QueryCCPeriod["HDAPublic"].ToString() == "1" ? true : false;
            RBL_HCardType.SelectedValue = QueryCCPeriod["HCardType"].ToString();


            //授權訂單狀態(HOrderStatus)：1=未成立、2=已成立
            LB_MHOrderStatus.CssClass = QueryCCPeriod["HOrderStatus"].ToString() == "1" ? "text-gray" : QueryCCPeriod["HOrderStatus"].ToString() == "2" ? "text-success border-0" : "text-gray border-0";
            LB_MHOrderStatus.Text = QueryCCPeriod["HOrderStatus"].ToString() == "1" ? "未成立" : QueryCCPeriod["HOrderStatus"].ToString() == "2" ? "已成立" : "未成立";

            //授權書狀態(HStatus)：0=停用、1=有效、2=作廢
            //ME20241010_新增授權書狀態: 0=停用/中止、1=有效、2=作廢、3=新增申請中、4=變更申請中、5=提前付清、6=付訖終止 (已付清)、7=換單
            string gHStatus = QueryCCPeriod["HStatus"].ToString();
            LB_OriHStatus.Text = gHStatus;
            switch (gHStatus)
            {
                case "0":
                    LB_MHStatus.Text = "停用/中止";
                    LB_MHStatus.CssClass = "text-gray";
                    break;
                case "1":
                    LB_MHStatus.Text = "有效";
                    LB_MHStatus.CssClass = "text-success";
                    break;
                case "2":
                    LB_MHStatus.Text = "作廢";
                    LB_MHStatus.CssClass = "text-danger";
                    break;
                case "3":
                    LB_MHStatus.Text = "新增申請中";
                    LB_MHStatus.CssClass = "text-info";
                    break;
                case "4":
                    LB_MHStatus.Text = "變更申請中";
                    LB_MHStatus.CssClass = "text-info";
                    break;
                case "5":
                    LB_MHStatus.Text = "提前付清";
                    LB_MHStatus.CssClass = "text-gray";
                    break;
                case "6":
                    LB_MHStatus.Text = "付訖終止 ";
                    LB_MHStatus.CssClass = "text-gray";
                    break;
                case "7":
                    LB_MHStatus.Text = "換單";
                    LB_MHStatus.CssClass = "text-primary";
                    break;

            }

            //MA20241011_顯示變更申請內容
            if (!string.IsNullOrEmpty(QueryCCPeriod["HOriCCPOrderCode"].ToString()) && QueryCCPeriod["HOriCCPOrderCode"].ToString() != "0")
            {
                //ME250702_先隱藏
                Panel_Change.Visible = false;
                //LB_Changed.Visible = true;
                Div_Original.Visible = false;
                LB_HOriCCPeriodCode.Text = QueryCCPeriod["HOriCCPOrderCode"].ToString();
                LB_HPartialPayTimes.Text = QueryCCPeriod["HPartialPayTimes"].ToString();
                LB_HPartialPayAmount.Text =QueryCCPeriod["HPartialPayAmount"].ToString() + "元";
                LB_HOrderGroup.Text = QueryCCPeriod["HOrderGroup"].ToString();

                if (TB_HDTotal.Text == "0" && LB_HPartialPayAmount.Text != "0")
                {
                    RBL_PayOption.SelectedValue = "2";
                    //LB_HModifyInfo.Text = "申請提前付清，金額為"+ Convert.ToInt32(LB_HPartialPayAmount.Text).ToString("N0")+"元";
                }
                else if (TB_HDTotal.Text != "0" && LB_HPartialPayAmount.Text != "0")
                {
                    RBL_PayOption.SelectedValue = "1";
                    //LB_HModifyInfo.Text = "申請部份付款，金額為" + Convert.ToInt32(LB_HPartialPayAmount.Text).ToString("N0") + "元";
                }



                if (QueryCCPeriod["BookingStatus"].ToString() == "1")
                {
                    if (QueryCCPeriod["HItemStatus"].ToString() == "1")
                    {
                        LB_HItemStatus.Text = "已付款";
                        LB_HItemStatus.CssClass = "text-success";
                    }
                    else if (QueryCCPeriod["HItemStatus"].ToString() == "2")
                    {
                        LB_HItemStatus.Text = "未付款";
                        LB_HItemStatus.CssClass = "text-danger";
                    }
                }


            }
            else
            {
                LB_HPartialPayTimes.Text = "0";
                LB_HPartialPayAmount.Text = "0";
            }


            //審核狀態(HVerifyStatus)：0=請選擇、1=未送審、2=送審中、3=審核通過、4=審核不通過
            LB_MHVerifyStatus.Text = QueryCCPeriod["HVerifyStatus"].ToString() == "1" ? "未送審" : QueryCCPeriod["HVerifyStatus"].ToString() == "2" ? "送審中" : QueryCCPeriod["HVerifyStatus"].ToString() == "3" ? "審核通過" : QueryCCPeriod["HVerifyStatus"].ToString() == "4" ? "審核不通過" : "";

            LB_MHVerifyStatus.CssClass = QueryCCPeriod["HVerifyStatus"].ToString() == "1" ? "text-info " : QueryCCPeriod["HVerifyStatus"].ToString() == "2" ? "text-info" : QueryCCPeriod["HVerifyStatus"].ToString() == "3" ? "text-success" : QueryCCPeriod["HVerifyStatus"].ToString() == "4" ? "text-danger" : "";

            //MA230822_加入若授權書狀態為作廢，審核狀態為空白
            if (QueryCCPeriod["HStatus"].ToString() == "2")
            {
                LB_MHVerifyStatus.Text = "";
            }


            //ME20230927_調整判斷:只有在訂單成立狀態(HOrderStatus：1=未成立 / 2=已成立)下，才不能修改
            if (QueryCCPeriod["HOrderStatus"].ToString() == "2")
            {
                Btn_Allow.Visible = false;
                Btn_Deny.Visible = false;
                Btn_Invalid.Visible = false;
                Btn_Valid.Visible = false;
            }
            else
            {
                Btn_Allow.Visible = true;
                Btn_Deny.Visible = true;
                Btn_Invalid.Visible = true;
                Btn_Allow.CssClass = "btn btn-success";
                Btn_Deny.CssClass = "btn btn-info";
                Btn_Invalid.CssClass = "btn btn-danger";
            }

            //若申請書已作廢則不能再進行審核
            if (QueryCCPeriod["HStatus"].ToString() == "2")
            {
                Btn_Allow.Visible = false;
                Btn_Deny.Visible = false;
                Btn_Invalid.Visible = false;
                Btn_Valid.Visible = true;
            }
            else
            {
                Btn_Valid.Visible = true;
                Btn_Invalid.Visible = false;
            }

        }
        QueryCCPeriod.Close();

        //MA20241013_判斷是否有變更
        SqlDataReader QueryCCPeriodNew = SQLdatabase.ExecuteReader("SELECT a.HCCPeriodCode, a.HMemberID, a.HCTemplateID, a.HCourseID, a.HCourseName, a.HDateRange, a.HDUserName, a.HDEmail, a.HDPostal, a.HDAddress, a.HDTel, a.HDPhone, a.HDBirth, a.HDPersonID, a.HDReceiptSType, a.HDonor, a.HCardHolder,a.HCHPersonID, a.HCardNum, a.HCardBank, a.HCardType, a.HCVCCode, a.HCardValidDate, a.HDTotal, a.HDCCPTimes, a.HDCCPAmount,a.HDCCPSDate, a.HDCCPEDate, a.HCHPhone, a.HDAPublic, a.HVerifyStatus, a.HOrderStatus,a.HStatus,  a.HOriCCPOrderCode, a.HPartialPayTimes, a.HPartialPayAmount, a.HOrderGroup, b.HItemStatus, b.HStatus AS BookingStatus  FROM HCCPeriod AS a LEFT JOIN HCourseBooking AS b ON a.HOrderGroup=b.HOrderGroup WHERE HOriCCPOrderCode='" + ((Label)RI.FindControl("LB_HCCPeriodCode")).Text + "'");

        if (QueryCCPeriodNew.Read())
        {
            Panel_Change.Visible = true;
            LB_HNewCCPeriodCode.Text = QueryCCPeriodNew["HCCPeriodCode"].ToString();

        }
        else
        {
            LB_HNewCCPeriodCode.Visible = false;
            Div_ChangedCode.Visible = false;
        }
        QueryCCPeriodNew.Close();

    }
    #endregion

    #region 列表資料繫結
    protected void Rpt_HCCPeriod_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        //授權訂單狀態(HOrderStatus)：1=未成立、2=已成立
        ((CheckBox)e.Item.FindControl("CB_Select")).Enabled = gDRV["HOrderStatus"].ToString() == "1" ? true : gDRV["HOrderStatus"].ToString() == "2" ? false : true;


        //審核狀態(HVerifyStatus)：0=請選擇、1=未送審、2=送審中、3=審核通過、4=審核不通過
        //ME20230927_註解
        //((CheckBox)e.Item.FindControl("CB_Select")).Enabled = gDRV["HVerifyStatus"].ToString() == "1" ? false : gDRV["HVerifyStatus"].ToString() == "2" ? false : gDRV["HVerifyStatus"].ToString() == "3" ? true : gDRV["HVerifyStatus"].ToString() == "4" ? false : false;

        ((Label)e.Item.FindControl("LB_HVerifyStatus")).CssClass = gDRV["HVerifyStatus"].ToString() == "1" ? "badge badge-info border-0" : gDRV["HVerifyStatus"].ToString() == "2" ? "badge badge-info border-0" : gDRV["HVerifyStatus"].ToString() == "3" ? "badge badge-success border-0" : gDRV["HVerifyStatus"].ToString() == "4" ? "badge badge-danger border-0" : "-";

        ((Label)e.Item.FindControl("LB_HVerifyStatus")).Text = gDRV["HVerifyStatus"].ToString() == "1" ? "未送審" : gDRV["HVerifyStatus"].ToString() == "2" ? "送審中" : gDRV["HVerifyStatus"].ToString() == "3" ? "審核通過" : gDRV["HVerifyStatus"].ToString() == "4" ? "審核不通過" : "未選擇";

        //勾選Enabled判斷:訂單成立&審核通過&有效
        if (gDRV["HOrderStatus"].ToString() == "2" && ((Label)e.Item.FindControl("LB_HVerifyStatus")).Text == "審核通過")
        {
            ((CheckBox)e.Item.FindControl("CB_Select")).Enabled = false;
        }





        //授權訂單狀態(HOrderStatus)：1=未成立、2=已成立
        ((Label)e.Item.FindControl("LB_HOrderStatus")).CssClass = gDRV["HOrderStatus"].ToString() == "1" ? "badge badge-gray border-0" : gDRV["HOrderStatus"].ToString() == "2" ? "badge badge-success border-0" : "badge badge-gray border-0";

        ((Label)e.Item.FindControl("LB_HOrderStatus")).Text = gDRV["HOrderStatus"].ToString() == "1" ? "未成立" : gDRV["HOrderStatus"].ToString() == "2" ? "已成立" : "未成立";

        #region 授權書狀態(HStatus)：0=停用、1=有效、2=作廢
        //GA20230821_判斷寫法
        //GA20240119_加入啟用/停用按鈕判斷
        //ME20241010_狀態調整：0=停用/中止、1=有效、2=作廢、3=新增申請中、4=變更申請中、5=提前付清、6=付訖終止 (已付清)、7=換單
        if (gDRV["HStatus"].ToString() == "1")
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "有效";
            ((Label)e.Item.FindControl("LB_HStatus")).CssClass = "text text-success border-0";
            ((LinkButton)e.Item.FindControl("LBtn_StartUsing")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_Deactivate")).Visible = true;
        }
        else if (gDRV["HStatus"].ToString() == "2")
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "作廢";
            ((Label)e.Item.FindControl("LB_HStatus")).CssClass = "text text-dark border-0";
            ((Label)e.Item.FindControl("LB_HVerifyStatus")).Text = "";
            //MA20230927_若為作廢則不能被勾選
            ((CheckBox)e.Item.FindControl("CB_Select")).Enabled = false;

            ((LinkButton)e.Item.FindControl("LBtn_StartUsing")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_Deactivate")).Visible = false;
        }
        else if (gDRV["HStatus"].ToString() == "3")
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "新增申請中";
            ((Label)e.Item.FindControl("LB_HStatus")).CssClass = "text text-info border-0";
            ((LinkButton)e.Item.FindControl("LBtn_StartUsing")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_Deactivate")).Visible = true;
            ((CheckBox)e.Item.FindControl("CB_Select")).Enabled = false;
        }
        else if (gDRV["HStatus"].ToString() == "4")
        {
            if (gDRV["HDCCPTimes"].ToString() != "0")
            {
                ((Label)e.Item.FindControl("LB_HStatus")).Text = "變更申請中";
                ((Label)e.Item.FindControl("LB_HStatus")).CssClass = "text text-info border-0";
                ((LinkButton)e.Item.FindControl("LBtn_StartUsing")).Visible = false;
                ((LinkButton)e.Item.FindControl("LBtn_Deactivate")).Visible = true;
                ((CheckBox)e.Item.FindControl("CB_Select")).Enabled = false;
            }
            else
            {
                ((Label)e.Item.FindControl("LB_HStatus")).Text = "變更申請中";
                ((Label)e.Item.FindControl("LB_HStatus")).CssClass = "text text-info border-0";
                ((LinkButton)e.Item.FindControl("LBtn_StartUsing")).Visible = false;
                ((LinkButton)e.Item.FindControl("LBtn_Deactivate")).Visible = true;
                ((CheckBox)e.Item.FindControl("CB_Select")).Enabled = false;

            }

        }
        else if (gDRV["HStatus"].ToString() == "5")
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "提前付清";
            ((Label)e.Item.FindControl("LB_HStatus")).CssClass = "text text-dark border-0";
            ((LinkButton)e.Item.FindControl("LBtn_StartUsing")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_Deactivate")).Visible = false;
            ((CheckBox)e.Item.FindControl("CB_Select")).Enabled = false;
        }
        else if (gDRV["HStatus"].ToString() == "6")
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "付訖終止 ";
            ((Label)e.Item.FindControl("LB_HStatus")).CssClass = "text text-dark border-0";
            ((LinkButton)e.Item.FindControl("LBtn_StartUsing")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_Deactivate")).Visible = false;
            ((CheckBox)e.Item.FindControl("CB_Select")).Enabled = false;
        }
        else if (gDRV["HStatus"].ToString() == "7")
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "換單";
            ((Label)e.Item.FindControl("LB_HStatus")).CssClass = "text text-dark border-0";
            ((LinkButton)e.Item.FindControl("LBtn_StartUsing")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_Deactivate")).Visible = false;
            ((CheckBox)e.Item.FindControl("CB_Select")).Enabled = false;
        }
        else
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "停用/中止";
            ((Label)e.Item.FindControl("LB_HStatus")).CssClass = "text text-danger border-0";
            ((LinkButton)e.Item.FindControl("LBtn_StartUsing")).Visible = true;
            ((LinkButton)e.Item.FindControl("LBtn_Deactivate")).Visible = false;
            ((CheckBox)e.Item.FindControl("CB_Select")).Enabled = false;
        }
        //((Label)e.Item.FindControl("LB_HStatus")).CssClass = gDRV["HStatus"].ToString() == "1" ? "badge badge-success border-0" : gDRV["HStatus"].ToString() == "2" ? "badge badge-danger border-0" : "badge badge-secondary border-0"; //GE20230821_改寫語法；註解
        //((Label)e.Item.FindControl("LB_HStatus")).Text = gDRV["HStatus"].ToString() == "1" ? "有效" : gDRV["HStatus"].ToString() == "2" ? "作廢" : ""; //GE20230821_改寫語法；註解
        #endregion

        ((Label)e.Item.FindControl("LB_HCardNum")).Text = !string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HCardNum")).Text) ? ((Label)e.Item.FindControl("LB_HCardNum")).Text.Split('-')[3].ToString() : ((Label)e.Item.FindControl("LB_HCardNum")).Text;

        //千分位
        ((Label)e.Item.FindControl("LB_HDTotal")).Text = Convert.ToInt32(((Label)e.Item.FindControl("LB_HDTotal")).Text).ToString("N0");
        ((Label)e.Item.FindControl("LB_HDCCPAmount")).Text = Convert.ToInt32(((Label)e.Item.FindControl("LB_HDCCPAmount")).Text).ToString("N0");


        #region 計算扣款期數
        //ME20241016_計算扣款期數
        SqlDataReader QueryPaidTimes = SQLdatabase.ExecuteReader("SELECT Count(*) AS num,  a.HDCCPTimes, (a.HDCCPTimes - Count(*)) AS RemainTimes FROM HCCPOrderDetail AS a INNER JOIN HCCPOTRecord AS b ON a.HID = b.HCCPODetailID  INNER JOIN HCourseBooking AS c ON b.HTradeNo = c.HTradeNo WHERE b.HRtnCode = 1 AND c.HItemStatus = 1 AND a.HCCPeriodCode='" + ((Label)e.Item.FindControl("LB_HCCPeriodCode")).Text + "' GROUP BY HCCPeriodCode, HDCCPTimes, HDCCPAmount");

        if (QueryPaidTimes.Read())
        {
            ((Label)e.Item.FindControl("LB_PaidPeriod")).Text = QueryPaidTimes["num"].ToString();
           
        }
        QueryPaidTimes.Close();

        if (((Label)e.Item.FindControl("LB_PaidPeriod")).Text != ((Label)e.Item.FindControl("LB_TotalPeriod")).Text)
        {
            ((Label)e.Item.FindControl("LB_PaidPeriod")).CssClass = "text-danger";
        }
        else{
            ((Label)e.Item.FindControl("LB_PaidPeriod")).CssClass = "text-success";
        }

        #endregion

        #region
        //MA20250715_新增判斷:若已扣期數=總期數，表示已完成，授權書狀態會顯示為付訖終止 
        if (((Label)e.Item.FindControl("LB_PaidPeriod")).Text == ((Label)e.Item.FindControl("LB_TotalPeriod")).Text)
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "付訖終止 ";
            ((Label)e.Item.FindControl("LB_HStatus")).CssClass = "label label-black border-0";
            ((LinkButton)e.Item.FindControl("LBtn_StartUsing")).Enabled = false;
            ((LinkButton)e.Item.FindControl("LBtn_Deactivate")).Enabled = false;
        }

        #endregion


    }
    #endregion

    #region 下載PDF
    //MA20230816_新增下載PDF功能
    //GE20230923_PDF檔名與標題取值調整
    protected void Btn_Download_Click(object sender, EventArgs e)
    {
        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HCCPeriodCode, HMemberID, HCourseName, HDUserName, HDEmail, HDPostal, HDAddress, HDTel, HDPhone, HDBirth, HDPersonID, HDReceiptSType, HDonor, HCardHolder, HCHPersonID, HCardNum, HCardBank, HCardType, HCVCCode, HCardValidDate, IIF(HDTotal IS NOT NULL AND HDTotal <>'0', FORMAT(HDTotal, 'N0'),'0') AS HDTotal, HDCCPTimes, IIF(HDCCPAmount IS NOT NULL AND HDCCPAmount <>'0', FORMAT(HDCCPAmount, 'N0'),'0') AS HDCCPAmount, HDCCPSDate, HDCCPEDate, HCHPhone, HDAPublic, HCreateDT FROM HCCPeriod WHERE HCCPeriodCode ='" + LB_MHCCPeriodCode.Text + "'");


        if (dr.Read())
        {

            // 讀取標楷體字體
            BaseFont chBaseFont = BaseFont.CreateFont(@"C:\\WINDOWS\\Fonts\\KAIU.TTF", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            iTextSharp.text.Font chFont = new iTextSharp.text.Font(chBaseFont, 12);

            string templetePath = Path.Combine(Request.PhysicalApplicationPath, "App_Data/pdf", "HCCPeriodForm.pdf");


            using (var reader = new PdfReader(templetePath))
            {
                using (var ms = new MemoryStream())
                {
                    using (var stamper = new PdfStamper(reader, ms))
                    {
                        //取得表單
                        var form = stamper.AcroFields;
                        //form.GenerateAppearances = true;
                        #region 加入標楷體字體，這個過程是為了避免中文亂碼或是無法顯示的問題。
                        foreach (string field in form.Fields.Keys)
                        {
                            // 更新字體
                            form.SetFieldProperty(field, "textfont", chBaseFont, null);
                        }
                        #endregion


                        string HCreateDTStr = "";
                        if (!string.IsNullOrEmpty(dr["HCreateDT"].ToString()))
                        {
                            HCreateDTStr = Convert.ToDateTime(dr["HCreateDT"].ToString()).ToString("yyyy/MM/dd");
                        }
                        //MA20230811_新增授權申請單號資訊
                        //GA20230923_新增授權單標題
                        //GA20240411_新增授權單副標題
                        form.SetField("PTB_Title", dr["HCourseName"].ToString());//授權單標題
                        form.SetField("PTB_SubTitle", "專用信用卡捐款授權書");//授權單副標題
                        form.SetField("PTB_HCCPeriodCode", dr["HCCPeriodCode"].ToString());//授權申請單號；AA20230811_新增授權申請單號資訊
                        form.SetField("PTB_HCreateDT", HCreateDTStr);//填表日期
                        form.SetField("PTB_HDUserName", dr["HDUserName"].ToString());//姓名
                        form.SetField("PTB_HDEmail", dr["HDEmail"].ToString());//E-mail
                        form.SetField("PTB_HDAddress", dr["HDPostal"].ToString() + " " + dr["HDAddress"].ToString());//通訊地址
                        form.SetField("PTB_HDTel", dr["HDTel"].ToString());//聯絡電話
                        form.SetField("PTB_HDPhone", dr["HDPhone"].ToString());//行動電話
                        form.SetField("PTB_HDBirth", dr["HDBirth"].ToString());//出生年月日
                        form.SetField("PTB_HDPersonID", dr["HDPersonID"].ToString());//身分證字號

                        //GE20240813_和氣財務：透過學員資料中的上傳國稅局欄位設定；故註解
                        //if (dr["HDReceiptSType"].ToString() == "1")
                        //{
                        //    form.SetField("PCB_HDReceiptSType_1", "Yes", true);//收據寄送
                        //}
                        //else if (dr["HDReceiptSType"].ToString() == "2")
                        //{
                        //    form.SetField("PCB_HDReceiptSType__2", "Yes", true);//收據寄送
                        //}

                        form.SetField("PTB_HDonor", dr["HDonor"].ToString());//祝福對象
                        form.SetField("PTB_HCardHolder", dr["HCardHolder"].ToString());//持卡人姓名
                        form.SetField("PTB_HCHPersonID", dr["HCHPersonID"].ToString());//持卡人身分證字號

                        string[] HCardNumStr = null;
                        if (!string.IsNullOrEmpty(dr["HCardNum"].ToString()) && dr["HCardNum"].ToString().IndexOf('-') > -1)
                        {
                            HCardNumStr = dr["HCardNum"].ToString().Split('-');
                            form.SetField("PTB_CardNum1", HCardNumStr[0]);//信用卡卡號
                            form.SetField("PTB_CardNum2", HCardNumStr[1]);//信用卡卡號
                            form.SetField("PTB_CardNum3", HCardNumStr[2]);//信用卡卡號
                            form.SetField("PTB_CardNum4", HCardNumStr[3]);//信用卡卡號
                        }

                        form.SetField("PTB_HCardBank", dr["HCardBank"].ToString());//發卡銀行


                        if (dr["HCardType"].ToString() == "1")
                        {
                            form.SetField("PCB_HCardType_1", "Yes", true);//信用卡卡別
                        }
                        else if (dr["HCardType"].ToString() == "2")
                        {
                            form.SetField("PCB_HCardType_2", "Yes", true);//信用卡卡別
                        }
                        else if (dr["HCardType"].ToString() == "3")
                        {
                            form.SetField("PCB_HCardType_3", "Yes", true);//信用卡卡別
                        }

                        form.SetField("PTB_HCVCCode", dr["HCVCCode"].ToString());//信用卡背面末三碼

                        string[] HCardValidDateStr = null;
                        if (!string.IsNullOrEmpty(dr["HCardValidDate"].ToString()) && dr["HCardValidDate"].ToString().IndexOf('/') > -1)
                        {
                            HCardValidDateStr = dr["HCardValidDate"].ToString().Split('/');
                            form.SetField("PTB_CardValidDateM", HCardValidDateStr[0]);//信用卡有效期限(月)
                            form.SetField("PTB_CardValidDateY", HCardValidDateStr[1]);//信用卡有效期限(年)
                        }

                        form.SetField("PTB_HDTotal", dr["HDTotal"].ToString());//捐款總金額
                        form.SetField("PTB_HDCCPTimes", dr["HDCCPTimes"].ToString());//定期定額扣款期數
                        form.SetField("PTB_HDCCPAmount", dr["HDCCPAmount"].ToString());//每期扣款金額

                        if (!string.IsNullOrEmpty(dr["HDCCPSDate"].ToString()))//扣款期間(起)
                        {
                            DateTime dateTime = DateTime.Parse(dr["HDCCPSDate"].ToString());
                            System.Globalization.GregorianCalendar tc = new System.Globalization.GregorianCalendar();//西元年
                            //System.Globalization.TaiwanCalendar tc = new System.Globalization.TaiwanCalendar();//民國
                            string HDCCPSDateStr = String.Format(" {0}年 {1}月 {2}日", tc.GetYear(dateTime), tc.GetMonth(dateTime), tc.GetDayOfMonth(dateTime));
                            form.SetField("PTB_HDCCPSDate", HDCCPSDateStr);//日期
                        }
                        else
                        {
                            form.SetField("PTB_HDCCPSDate", "");//日期
                        }

                        if (!string.IsNullOrEmpty(dr["HDCCPEDate"].ToString()))//扣款期間(訖)
                        {
                            DateTime dateTime = DateTime.Parse(dr["HDCCPEDate"].ToString());
                            System.Globalization.GregorianCalendar tc = new System.Globalization.GregorianCalendar();//西元年
                            //System.Globalization.TaiwanCalendar tc = new System.Globalization.TaiwanCalendar();//民國
                            string HDCCPEDateStr = String.Format(" {0}年 {1}月 {2}日", tc.GetYear(dateTime), tc.GetMonth(dateTime), tc.GetDayOfMonth(dateTime));
                            form.SetField("PTB_HDCCPEDate", HDCCPEDateStr);//日期
                        }
                        else
                        {
                            form.SetField("PTB_HDCCPEDate", "");//日期
                        }

                        //GE20240813_和氣財務：不需提供此欄位；故註解
                        //if (dr["HDAPublic"].ToString() == "1")
                        //{
                        //    form.SetField("CB_HDAPublic", "Yes", true);//捐贈不公開
                        //}

                        form.SetField("PTB_HCHPhone", dr["HCHPhone"].ToString());//持卡人手機號碼

                        stamper.FormFlattening = true;  //表單扁平化，防止表單編輯
                        stamper.Close();
                    }

                    #region Response File

                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = "和氣大愛文教基金會" + dr["HCourseName"].ToString() + "專用信用卡捐款授權書.pdf",
                        //FileName = "和氣大愛文教基金會84輪專用信用卡捐款授權書.pdf",
                        Inline = true,
                    };
                    Response.BinaryWrite(ms.ToArray());
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + dr["HCourseName"].ToString() + "專用信用卡捐款授權書" + DateTime.Now.ToString("yyyy/MM/dd") + ".pdf");
                    //Response.AddHeader("Content-Disposition", "attachment; filename=" + "和氣大愛文教基金會84輪專用信用卡捐款授權書" + DateTime.Now.ToString("yyyy/MM/dd") + ".pdf");
                    //Response.AddHeader("Content-Disposition", "attachment; filename=" + "三聯式發票" + DateTime.Now.ToString("yyyy/MM/dd") + ".pdf");
                    //Response.AddHeader("Content-Disposition", cd.ToString());

                    Response.End();



                    // return File(ms.ToArray(), "application/pdf", "result.pdf");
                    #endregion

                } //dispose memory stream
            }// dispose pdf reader



        }
        dr.Close();

    }
    #endregion

    #region 匯出Excel功能
    protected void LBtn_ToExcel_Click(object sender, EventArgs e)
    {
        System.Data.DataTable dataTable = new System.Data.DataTable();

        SqlCommand dbCmd = default(SqlCommand);

        con.Open();
        dbCmd = new SqlCommand("SELECT a.HCCPeriodCode AS '授權申請單號',  a.HDUserName AS '捐款人姓名', a.HDPhone AS '捐款人手機',a.HDonor  AS '祝福對象',SUBSTRING(a.HCardNum, 16, 20) AS '卡號末四碼', a.HCourseName AS '捐款項目', a.HDTotal AS '捐款總金額', a.HDCCPTimes AS '扣款期數', a.HDCCPAmount AS '每期扣金額', (a.HDCCPSDate + '-' + a.HDCCPEDate) AS '扣款期間', Case(a.HStatus)  When('2') Then('作廢') When('1') Then('有效')  When('0') Then('停用') Else '-'  End AS '授權書狀態', Case(a.HVerifyStatus)  When('4') Then('審核不通過') When('3') Then('審核通過') When('2') Then('送審中') When('1') Then('未送審')  Else '請選擇'  End AS '審核狀態', Case(a.HOrderStatus)  When('2') Then('已成立') When('1') Then('未成立')  Else '-'  End  AS '授權訂單狀態' FROM HCCPeriod AS a LEFT JOIN MemberList AS b ON a.HMemberID = b.HID ORDER BY a.HOrderStatus ASC, a.HCreateDT DESC, a.HVerifyStatus DESC, a.HStatus DESC", con);
        // create data adapter
        SqlDataAdapter da = new SqlDataAdapter(dbCmd);
        // this will query your database and return the result to your datatable
        da.Fill(dataTable);

        string gXlsFile = "~/uploads/App_Data/";
        TableToExcel(dataTable, Server.MapPath(gXlsFile) + "CCPeriod.xlsx", "1");

        con.Close();
        dbCmd.Cancel();
    }

    #region Datable匯出成Excel
    /// <summary>
    /// Datable匯出成Excel
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="file"></param>
    public static void TableToExcel(System.Data.DataTable dt, string file, string pmethod)
    {
        IWorkbook workbook;
        string fileExt = Path.GetExtension(file).ToLower();
        if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); } else { workbook = null; }
        if (workbook == null) { return; }
        string filename = "CCPeriod";

        //pmethod：1=匯出列表Excel；2=匯出文化事業(發票)；3=匯出基金會&文化事業-捐款(收據)
        if (pmethod == "1")
        {
            ISheet sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet("訂單明細總表") : workbook.CreateSheet(dt.TableName);
            filename = "訂單明細總表";

            //表頭  
            IRow row = sheet.CreateRow(0);
            //利用sheet建立列
            //sheet.GetRow(0).CreateCell(0).SetCellValue("序號");
            //sheet.GetRow(0).CreateCell(1).SetCellValue("發票/捐款收據號碼");
            //sheet.GetRow(0).CreateCell(2).SetCellValue("發票/捐款收據日期");
            //sheet.GetRow(0).CreateCell(3).SetCellValue("訂單代碼");
            //sheet.GetRow(0).CreateCell(4).SetCellValue("項目編號");
            //sheet.GetRow(0).CreateCell(5).SetCellValue("綠界廠商訂單編號");
            //sheet.GetRow(0).CreateCell(6).SetCellValue("綠界訂單編號");
            //sheet.GetRow(0).CreateCell(7).SetCellValue("課程名稱");
            //sheet.GetRow(0).CreateCell(8).SetCellValue("繳費帳戶");//2023/06/07 :原"繳費方式"改"繳費帳戶"
            //sheet.GetRow(0).CreateCell(9).SetCellValue("學員姓名");
            //sheet.GetRow(0).CreateCell(10).SetCellValue("是否參班");
            //sheet.GetRow(0).CreateCell(11).SetCellValue("使用折扣碼");
            //sheet.GetRow(0).CreateCell(12).SetCellValue("課程/捐款");
            ////sheet.GetRow(0).CreateCell(10).SetCellValue("是否捐款");
            ////sheet.GetRow(0).CreateCell(11).SetCellValue("報名金額");
            //sheet.GetRow(0).CreateCell(13).SetCellValue("報名/護持金額");//2023/03/01 :原"報名金額"改"報名/護持金額"
            //sheet.GetRow(0).CreateCell(14).SetCellValue("使用點數");
            //sheet.GetRow(0).CreateCell(15).SetCellValue("付款方式");
            //sheet.GetRow(0).CreateCell(16).SetCellValue("付款時間");
            //sheet.GetRow(0).CreateCell(17).SetCellValue("付款狀態");
            //sheet.GetRow(0).CreateCell(18).SetCellValue("付款總金額");

            //資料  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row1 = sheet.CreateRow(i + 1);

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    //sheet.AutoSizeColumn((short)j);//自動根據長度調整儲存格寬度

                    ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());

                }


            }


        }

        //下載Excel檔
        //HttpContext.Current.Response.BinaryWrite(stream.GetBuffer());
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", filename + DateTime.Now.ToString("yyyyMMdd") + ".xlsx"));
        HttpContext.Current.Response.AddHeader("Transfer-Encoding", "identity");
        workbook.Write(HttpContext.Current.Response.OutputStream);
        HttpContext.Current.Response.Flush();
        HttpContext.Current.Response.Close();

    }
    #endregion

    #endregion

    #region 啟用/停用功能
    //GA20240118_加入啟用/停用功能
    protected void LBtn_StartUsing_Click(object sender, EventArgs e)
    {
        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        SqlCommand cmd = new SqlCommand("UPDATE HCCPeriod SET HStatus ='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPeriodCode ='" + ((Label)RI.FindControl("LB_HCCPeriodCode")).Text + "';UPDATE HCCPOrderDetail SET HStatus ='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPeriodCode ='" + ((Label)RI.FindControl("LB_HCCPeriodCode")).Text + "'", con);

        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();
        cmd.Cancel();

        Response.Write("<script>alert('啟用成功');window.location.href='HCCPeriodVerify.aspx';</script>");
    }

    protected void LBtn_Deactivate_Click(object sender, EventArgs e)
    {
        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        SqlCommand cmd = new SqlCommand("UPDATE HCCPeriod SET HStatus ='0', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPeriodCode ='" + ((Label)RI.FindControl("LB_HCCPeriodCode")).Text + "';UPDATE HCCPOrderDetail SET HStatus ='0', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPeriodCode ='" + ((Label)RI.FindControl("LB_HCCPeriodCode")).Text + "'", con);

        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();
        cmd.Cancel();

        Response.Write("<script>alert('停用成功');window.location.href='HCCPeriodVerify.aspx';</script>");
    }

    #endregion


    #region 儲存功能
    protected void Btn_Save_Click(object sender, EventArgs e)
    {
        //ME20250708_新增開啟前台變更申請按鈕&新的扣款起始日欄位

        //判斷若勾選要開啟變更，則必須填寫新的扣款起始日&變更授權書狀態為中止
        if (RBL_HOpenEdit.SelectedValue == "1")
        {
            if (string.IsNullOrEmpty(TB_HNewDCCPSDate.Text.Trim()))
            {
                Response.Write("<script>alert('有勾選開放變更申請按鈕，請填寫新的扣款起始日哦~!');</script>");
                ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_HCCPeriod').modal('show');</script>", false);
                return;
            }
        }


        //判斷授權書狀態的變更
        string gStatus = ""; 
        if (RBL_HOpenEdit.SelectedValue == "1" && !string.IsNullOrEmpty(TB_HNewDCCPSDate.Text.Trim()))
        {
            gStatus = "0";
        }
        else
        {
            gStatus = LB_OriHStatus.Text;
        }



        SqlCommand cmd = new SqlCommand("UPDATE HCCPeriod SET HRemark ='"+ TB_HRemark.Text.Trim() + "', HOpenEdit='"+ RBL_HOpenEdit.SelectedValue + "', HNewDCCPSDate ='"+ TB_HNewDCCPSDate.Text.Trim() + "', HStatus='"+ gStatus + "', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPeriodCode ='" + LB_MHCCPeriodCode.Text + "'", con);

        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();
        cmd.Cancel();

        Response.Write("<script>alert('儲存成功');</script>");

        //開啟Modal
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_HCCPeriod').modal('show');</script>", false);

        if (gStatus=="0")
        {
            LB_MHStatus.Text = "停用/中止";
            LB_MHStatus.CssClass = "text-gray";
        }

        SDS_HCCPeriod.SelectCommand = "SELECT  a.HID, a.HCCPeriodCode, a.HMemberID, a.HCourseName, a.HDUserName, a.HDPhone, a.HDonor, a.HCardNum, a.HDTotal, a.HDCCPTimes,  a.HDCCPAmount, (a.HDCCPSDate+'-'+a.HDCCPEDate) AS HDCCPDateRange, a.HVerifyStatus,a.HOrderStatus, a.HStatus, (b.HArea + '/' + b.HPeriod + ' ' + b.HUserName) AS UserName, HOriCCPOrderCode, HPartialPayTimes, HPartialPayAmount FROM HCCPeriod  AS a LEFT JOIN MemberList AS b ON a.HMemberID = b.HID WHERE  HPaperYN='1' ORDER BY a.HCreateDT DESC, a.HModifyDT DESC";/* ORDER BY a.HStatus ASC, a.HOrderStatus ASC, a.HVerifyStatus ASC*/

        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCPeriod.SelectCommand, PageMax, LastPage, true, Rpt_HCCPeriod);

    }
    #endregion
}
