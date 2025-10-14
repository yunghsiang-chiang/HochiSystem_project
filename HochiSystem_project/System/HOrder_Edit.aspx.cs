using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;



public partial class HOrder_Edit : System.Web.UI.Page
{

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
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {


        #region 分頁copy-2
        if (!IsPostBack)
        {
            ViewState["Search"] = "";
            ViewState["ReceiptReport"] = "";
        }

        if (!IsPostBack)
        {

            SDS_HCMCB.SelectCommand = "SELECT TOP(100) ROW, HOrderGroup, CourseName, HOrderNum, HCXLApplyDate, HMerchantTradeNo, PMethod, HMemberID,   PayMethod, HPaymentDate, HPayAmt, HECPAmount, HStatus, HUse, UserName, HCreateDT, HOrderGroupSrc FROM OrderList_BackEnd ORDER BY HOrderNum DESC";

            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCMCB.SelectCommand, PageMax, LastPage, false, Rpt_HCMCB);
            ViewState["Search"] = SDS_HCMCB.SelectCommand;


        }
        else
        {
            SDS_HCMCB.SelectCommand = ViewState["Search"].ToString();
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", ViewState["Search"].ToString(), PageMax, LastPage, false, Rpt_HCMCB);
        }
        #endregion


    }







    #region 搜尋
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {


        string gSearch = TB_Search.Text.Trim();
        string[] gSearchDate;
        string SearchDateS = "1000/01/01";
        string SearchDateE = "9999/12/31";
        if (TB_SearchDate.Text != "")
        {
            gSearchDate = TB_SearchDate.Text.Split('-');
            SearchDateS = gSearchDate[0].Trim();
            SearchDateE = gSearchDate[1].Trim();
        }

        string gPMethod = DDL_HPMethod.SelectedItem.Text;
        if (DDL_HPMethod.SelectedValue == "0")
        {
            gPMethod = "";
        }

        string gHStatus_Search = DDL_HStatus_Search.SelectedValue;

        string QuerySql = "SELECT ROW, HOrderGroup, CourseName, HOrderNum, HCXLApplyDate, HMerchantTradeNo, PMethod, HMemberID, PayMethod, HPaymentDate, HPayAmt, HECPAmount, HStatus, HUse, UserName, HCreateDT, HOrderGroupSrc FROM OrderList_Backend";


        //搜尋條件
        StringBuilder sql = new StringBuilder(QuerySql);
        List<string> WHERE = new List<string>();

        //排序
        string Order = "  ORDER BY HOrderNum DESC";

        //關鍵字
        if (!string.IsNullOrEmpty(gSearch))
        {
            WHERE.Add("  (HOrderGroup LIKE '%" + gSearch + "%' OR UserName  LIKE N'%" + gSearch + "%' OR HMerchantTradeNo= '" + gSearch + "' ) ");
        }

        //付款日期區間
        if (!string.IsNullOrEmpty(TB_SearchDate.Text.Trim()))
        {
            WHERE.Add("  (LEFT(HPaymentDate,10) >='" + SearchDateS + "' AND LEFT(HPaymentDate,10) <='" + SearchDateE + "')");
        }

        //繳費帳戶
        if (DDL_HPMethod.SelectedValue != "0")
        {
            WHERE.Add("  (PMethod = '" + gPMethod + "') ");
        }

        //訂單狀態
        if (DDL_HStatus_Search.SelectedValue != "0")
        {
            WHERE.Add("  (HStatus = '" + DDL_HStatus_Search.SelectedValue + "') ");
        }

        //判斷使用者是否選擇了條件
        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" WHERE " + wh);
        }

        SDS_HCMCB.SelectCommand = sql.ToString() + Order;



        #region 分頁copy-3搜尋用
        ViewState["Search"] = SDS_HCMCB.SelectCommand;
        //呼叫分頁 (連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 搜尋開啟, DataList控件)
        Pg_Paging.PagingLoad("HochiSystemConnection", SDS_HCMCB.SelectCommand, PageMax, LastPage, true, Rpt_HCMCB);
        #endregion





    }
    #endregion


    #region 取消搜尋
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_Search.Text = "";
        TB_SearchDate.Text = "";
        DDL_HPMethod.SelectedValue = "0";
        DDL_HStatus_Search.SelectedValue = "0";


        SDS_HCMCB.SelectCommand = "SELECT ROW, HOrderGroup, CourseName, HOrderNum, HCXLApplyDate, HMerchantTradeNo, PMethod, HMemberID, PayMethod, HPaymentDate, HPayAmt, HECPAmount, HStatus, HUse, UserName, HCreateDT, HOrderGroupSrc FROM OrderList_BackEnd ORDER BY HOrderNum DESC";


        ViewState["Search"] = SDS_HCMCB.SelectCommand;

        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCMCB.SelectCommand, PageMax, LastPage, true, Rpt_HCMCB);



    }
    #endregion








    protected void Rpt_HCBM_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        //ListItemType.Item→奇數行、ListItemType.AlternatingItem→偶數行
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            Parameter gDI = (Parameter)e.Item.DataItem;
            ((Label)e.Item.FindControl("LB_HSex")).Text = gDI.HSex.ToString() == "1" ? "男" : gDI.HSex.ToString() == "2" ? "女" : "其他";
        }

    }







    public static class Content
    {
        public static List<Parameter> mParameter { get; set; }
    }

    public class Parameter
    {
        public string HCGuide { get; set; }
        public string HPersonID { get; set; }
        public string HSex { get; set; }


        public Parameter(string HCGuide, string HPersonID, string HSex)
        {
            this.HCGuide = HCGuide;
            this.HPersonID = HPersonID;
            this.HSex = HSex;
        }

    }

    #region 內頁
    protected void LBtn_Edit_Click(object sender, EventArgs e)
    {
        Panel_List.Visible = false;
        Panel_Edit.Visible = true;

        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        LinkButton LBtn_Edit = sender as LinkButton;
        string[] gCheck_CA = LBtn_Edit.CommandArgument.Split(',');

        string gOrder = null;


        if (!string.IsNullOrEmpty(gCheck_CA[0]))
        {
            gOrder = gCheck_CA[0];  //訂單代碼
        }
        else
        {
            gOrder = gCheck_CA[1];   //項目編號
        }




        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        string strSelHCMCB = null;
        string strSelHCB = null;  //護持者


        string OrderType = gOrder.Substring(0, 1);

        if (OrderType == "B")
        {
            LB_HOrderNum.Visible = false;
            LB_HOrderNum.Text = gOrder;
            strSelHCMCB = "SELECT HID, HStatus, HOrderGroup, HOrderNum, HCBCourseName, HCBHDateRange, HPMethod, HBDate, HPayMethod,    HAttend, HMerchantTradeNo, HTradeNo, HCreateDT, HTeam, HType, HMType, HUserName, HMemberID, HPersonID,    HPhone, HEmail, HCGuide, HCourseName, HDateRange, HTeacherName, HBCPoint, HPoint, HLDiscount, TeacherName,    HRoom, HRoomName, HRoomTime, HDCode, HDSummary, HDPoint, HPlaceName, HPAmount, HSubTotal, HECPAmount,    HPaymentDate, HPayAmt, HPaymentNo, HATMBankCode, HATMVAccount, HExpireDate, HFinanceRemark, HInvoiceNo,    HInvoiceDate, HInvoiceStatus, HUse, HItemStatus, HChangeStatus, HCXLOrderGroup, HCXLApplyDate, HCXLFinishDate,    HCXLAmount, HCXLReason, HCXLDetailReason, HCXLSubTotalSum, HCXLHandlingFee, HCXLTotal, HCXLHandleStatus,    HCXLBankCode, HCXLBankName, HCXLBAccount, HCXLBAName, HCDeadline, HCPkgYN, HCPkgHID, HCPkgName,    HUploadIRS, HBookByDateYN FROM OrderList WHERE  HOrderNum='" + gOrder + "'";
            strSelHCB = "SELECT HCGuide FROM HCourseBooking WHERE HOrderNum='" + gOrder + "'";
        }
        else
        {
            LB_HOrderGroup.Visible = true;
            LB_HOrderGroup.Text = gOrder;
            strSelHCMCB = "SELECT HID, HStatus, HOrderGroup, HOrderNum, HCBCourseName, HCBHDateRange, HPMethod, HBDate, HPayMethod,    HAttend, HMerchantTradeNo, HTradeNo, HCreateDT, HTeam, HType, HMType, HUserName, HMemberID, HPersonID,    HPhone, HEmail, HCGuide, HCourseName, HDateRange, HTeacherName, HBCPoint, HPoint, HLDiscount, TeacherName,    HRoom, HRoomName, HRoomTime, HDCode, HDSummary, HDPoint, HPlaceName, HPAmount, HSubTotal, HECPAmount,    HPaymentDate, HPayAmt, HPaymentNo, HATMBankCode, HATMVAccount, HExpireDate, HFinanceRemark, HInvoiceNo,    HInvoiceDate, HInvoiceStatus, HUse, HItemStatus, HChangeStatus, HCXLOrderGroup, HCXLApplyDate, HCXLFinishDate,    HCXLAmount, HCXLReason, HCXLDetailReason, HCXLSubTotalSum, HCXLHandlingFee, HCXLTotal, HCXLHandleStatus,    HCXLBankCode, HCXLBankName, HCXLBAccount, HCXLBAName, HCDeadline, HCPkgYN, HCPkgHID, HCPkgName,    HUploadIRS, HBookByDateYN FROM OrderList WHERE  HOrderGroup='" + gOrder + "'";
            strSelHCB = "SELECT HCGuide FROM HCourseBooking WHERE HOrderGroup='" + gOrder + "'";
        }

        Boolean NewOrder = false;


        //WA20230929_加入換課後資訊
        if (LB_HOrderGroup.Text != ((Label)RI.FindControl("LB_HOrderGroupSrc")).Text)
        {
            LB_OriOrderGroup.Text = ((Label)RI.FindControl("LB_HOrderGroupSrc")).Text;
            LB_OrderGroupSrc.Text = ((Label)RI.FindControl("LB_HOrderGroupSrc")).Text;
            //LB_Memo.Visible = true;
            //Supplement.Visible = true;
        }
        else
        {
            LB_OriOrderGroup.Text = "-";
        }

        #region  訂單變更紀錄顯示
        //WE20230910_修正語法
        SDS_ChangeRecord.SelectCommand = "SELECT DISTINCT a.HOrderGroupSrc,  a.HOrderGroupNew, c.HPayAmt AS BalancePaid,a.HCreateDT AS ChangeDT FROM HCBChangeRecord AS a LEFT JOIN HCourseBooking AS b ON (a.HOrderGroupSrc = b.HOrderGroup ) LEFT JOIN HCourseBooking AS c ON a.HOrderGroupNew = c.HOrderGroup WHERE b.HOrderGroup='" + LB_HOrderGroup.Text + "' AND a.HItemStatus='1' GROUP BY a.HOrderGroupSrc, a.HOrderGroupNew, c.HPayAmt, a.HCreateDT ORDER BY a.HCreateDT DESC";
        Rpt_ChangeRecord.DataBind();


        if (Rpt_ChangeRecord.Items.Count != 0)
        {
            Panel_HChangeRecord.Visible = true;
        }
        else
        {
            Panel_HChangeRecord.Visible = false;
        }

        #endregion

        //WA20240205_判斷是否為套裝課程訂單
        string gStrCPkgYN = null;

        dbCmd = new SqlCommand(strSelHCMCB, dbConn);
        SqlDataReader MyQueryHCMCB = dbCmd.ExecuteReader();
        if (MyQueryHCMCB.Read())
        {
            //WA20250620_加入是否開放單日報名的資訊
            if (MyQueryHCMCB["HBookByDateYN"].ToString() == "1")
            {
                HF_HBookByDateYN.Value = MyQueryHCMCB["HBookByDateYN"].ToString();
            }

            //WA20230929_加入換課資訊
            if (MyQueryHCMCB["HCBCourseName"].ToString() == "換課")
            {
                NewOrder = true;
            }

            //WA20240205_判斷此訂單是否為套裝課程訂單(HCPkgYN=1:表示為套裝課程訂單)
            if (MyQueryHCMCB["HCPkgYN"].ToString() == "1")
            {
                gStrCPkgYN = "1";
            }



            ////訂單狀態HStatus-250516更新
            ///1=訂單成立、2=訂單取消

            LB_HStatus.Text = MyQueryHCMCB["HStatus"].ToString();
            DDL_HStatus.SelectedValue = MyQueryHCMCB["HStatus"].ToString();

            LB_HOrderGroup.Text = MyQueryHCMCB["HOrderGroup"].ToString();
            LB_HBDate.Text = Convert.ToDateTime(MyQueryHCMCB["HCreateDT"].ToString()).ToString("yyyy/MM/dd");
            LB_HMerchantTradeNo.Text = MyQueryHCMCB["HMerchantTradeNo"].ToString();
            LB_HTradeNo.Text = MyQueryHCMCB["HTradeNo"].ToString();


            if (!string.IsNullOrEmpty(MyQueryHCMCB["HPayAmt"].ToString()) && !string.IsNullOrEmpty(MyQueryHCMCB["HPaymentDate"].ToString()))
            {
                LB_HPayAmt.Text = Convert.ToInt32(MyQueryHCMCB["HPayAmt"].ToString()).ToString("N0");
            }
            else if (!string.IsNullOrEmpty(MyQueryHCMCB["HPayAmt"].ToString()))
            {
                LB_HPayAmt.Text = Convert.ToInt32(MyQueryHCMCB["HPayAmt"].ToString()).ToString("N0");
            }
            else
            {
                LB_HPayAmt.Text = "0";
            }

            LB_HPaymentDate.Text = MyQueryHCMCB["HPaymentDate"].ToString();
            LB_HECPAmount.Text = MyQueryHCMCB["HECPAmount"].ToString();

            //0=請選擇、1=線上刷卡、2=線上ATM、3=超商繳費、9=其它
            LB_HPayMethod.Text = MyQueryHCMCB["HPayMethod"].ToString() == "1" ? "線上刷卡" : MyQueryHCMCB["HPayMethod"].ToString() == "2" ? "線上ATM" : MyQueryHCMCB["HPayMethod"].ToString() == "3" ? "超商繳費" : MyQueryHCMCB["HPayMethod"].ToString() == "4" ? "ATM櫃員機" : MyQueryHCMCB["HPayMethod"].ToString() == "9" ? "其它" : "其它";

            if (LB_HPayMethod.Text != "超商繳費")
            {
                LB_HPaymentNo.Text = "-";
            }
            else
            {
                LB_PayType.Text = "超商";
                LB_HPaymentNo.Text = MyQueryHCMCB["HPaymentNo"].ToString();
                LB_HExpireDate.Text = MyQueryHCMCB["HExpireDate"].ToString();

            }

            //WE230822_加入ATM櫃員機繳費資訊
            if (LB_HPayMethod.Text != "ATM櫃員機")
            {
                LB_HATMBankCode.Text = "-";
                LB_HATMVAccount.Text = "-";
            }
            else
            {
                LB_PayType.Text = "ATM櫃員機";
                LB_HATMBankCode.Text = MyQueryHCMCB["HATMBankCode"].ToString();
                LB_HATMVAccount.Text = MyQueryHCMCB["HATMVAccount"].ToString();
                LB_HExpireDate.Text = MyQueryHCMCB["HExpireDate"].ToString();
            }



            //0=請選擇、1=基金會、2=文化事業
            //LB_HPMethod.Text = MyQueryHCMCB["HPMethod"].ToString() == "1" ? "基金會" : MyQueryHCMCB["HPMethod"].ToString() == "2" ? "文化事業" : "無";

            //WE20230919_修改判斷條件
            if (!string.IsNullOrEmpty(LB_HOrderGroup.Text))
            {
                if (LB_HOrderGroup.Text.Substring(0, 1).Trim() == "F")
                {
                    LB_HPMethod.Text = "基金會";
                }
                else if (LB_HOrderGroup.Text.Substring(0, 1).Trim() == "C")
                {
                    LB_HPMethod.Text = "文化事業";
                }
                else if (LB_HOrderGroup.Text.Substring(0, 1).Trim() == "O") //混合基金會和文化事業
                {

                    SqlDataReader QueryDonate = SQLdatabase.ExecuteReader("SELECT HPMethod, HCourseDonate FROM HCourseBooking WHERE HOrderGroup='" + LB_HOrderGroup.Text + "'");
                    while (QueryDonate.Read())
                    {
                        if (QueryDonate["HPMethod"].ToString() == "2" && QueryDonate["HCourseDonate"].ToString() == "1")  //文化+捐款
                        {
                            LB_HPMethod.Text = "基金會";
                        }
                        else if (QueryDonate["HPMethod"].ToString() == "1")  //基金會
                        {
                            LB_HPMethod.Text = "基金會";
                        }

                    }
                    QueryDonate.Close();


                }
            }
            else  //B開頭 (最原始的資料)
            {
                //WE20231117_改判斷寫法
                if (MyQueryHCMCB["HPMethod"].ToString() == "1")
                {
                    LB_HPMethod.Text = "基金會";
                }
                else if (MyQueryHCMCB["HPMethod"].ToString() == "2")
                {
                    LB_HPMethod.Text = "文化事業";
                }

            }


    

            //學員類別改抓HMType對應的的資料--220705
            LB_HType.Text = MyQueryHCMCB["HMType"].ToString();

            LB_HMemberID.Text = MyQueryHCMCB["HMemberID"].ToString();
            LB_HUserName.Text = MyQueryHCMCB["HUserName"].ToString();
            LB_HPersonID_N.Text = MyQueryHCMCB["HPersonID"].ToString();
            LB_HPhone.Text = MyQueryHCMCB["HPhone"].ToString();
            LB_HEmail.Text = MyQueryHCMCB["HEmail"].ToString();


            //220829-加入財務備註欄位
            TB_HFinanceRemark.Text = MyQueryHCMCB["HFinanceRemark"].ToString();

            LB_HPaymentDate.Text = MyQueryHCMCB["HPaymentDate"].ToString();


            //加入顯示發票資訊
            LB_HInvoiceNo.Text = MyQueryHCMCB["HInvoiceNo"].ToString();
            LB_HInvoiceDate.Text = MyQueryHCMCB["HInvoiceDate"].ToString();
            LB_HInvoiceStatus.Text = MyQueryHCMCB["HInvoiceStatus"].ToString() == "1" ? "已開立" : MyQueryHCMCB["HInvoiceStatus"].ToString() == "0" ? "尚未開立" : MyQueryHCMCB["HInvoiceStatus"].ToString() == "2" ? "已作廢" : MyQueryHCMCB["HInvoiceStatus"].ToString() == "3" ? "已折讓" : "-";

            LB_HInvoiceStatus.CssClass = MyQueryHCMCB["HInvoiceStatus"].ToString() == "1" ? "text-success" : MyQueryHCMCB["HInvoiceStatus"].ToString() == "0" ? "text-danger" : MyQueryHCMCB["HInvoiceStatus"].ToString() == "2" ? "text-info" : MyQueryHCMCB["HInvoiceStatus"].ToString() == "3" ? "text-primary" : "text-info";

            //EA20240221_加入上傳國稅局
            //ME20240318_加入身分證欄位判斷
            if (MyQueryHCMCB["HPMethod"].ToString() == "1")
            {
                LB_HUploadIRS.Text = MyQueryHCMCB["HUploadIRS"].ToString() == "1" && !string.IsNullOrEmpty(MyQueryHCMCB["HPersonID"].ToString()) ? "是" : MyQueryHCMCB["HUploadIRS"].ToString() == "0" ? "否" : "-";
            }
            else
            {
                LB_HUploadIRS.Text = "-";
            }


            LB_HUse.Text = string.IsNullOrEmpty(MyQueryHCMCB["HUse"].ToString()) ? "0" : MyQueryHCMCB["HUse"].ToString();


            //WA20250113-加入判斷:若是中心財務(綉琪&楊姐)才可以修改訂單狀態
            if (((Label)Master.FindControl("LB_HUserHID")).Text == "7676" || ((Label)Master.FindControl("LB_HUserHID")).Text == "9377" || ((Label)Master.FindControl("LB_HUserHID")).Text == "9381")
            {
                //WA20250516_如果訂單已退款則不會再顯示取消報名按鈕
                if (DDL_HStatus.SelectedValue == "2")
                {
                    LBtn_Refund.Enabled = false;
                    LBtn_Refund.CssClass = "btn btn-gray";

                    Div_Refund.Visible = true;
                    LB_HCXLTotal.Text = Convert.ToInt32(MyQueryHCMCB["HCXLTotal"].ToString()).ToString("N0");
                }
                else
                {

                    LBtn_Refund.Enabled = true;
                    LBtn_Refund.Visible = true;
                }
            }
            else
            {
                LBtn_Refund.Visible = false;
            }

        }

        MyQueryHCMCB.Close();



        dbCmd = new SqlCommand(strSelHCB, dbConn);
        SqlDataReader MyQueryHCB = dbCmd.ExecuteReader();
        if (MyQueryHCB.Read())
        {
            if (!string.IsNullOrEmpty(MyQueryHCB["HCGuide"].ToString()))
            {
                string gRpt_HCGuide = MyQueryHCB["HCGuide"].ToString();
                List<Parameter> mParameter = new List<Parameter>();
                //for (int i = 0; i < gRpt_HCGuide.Length - 1; i++)
                for (int i = 0; i < 1; i++)
                {
                    string strSelHM = "select HUserName, HPersonID, HSex from HMember where HID='" + gRpt_HCGuide[i].ToString() + "'";
                    dbCmd = new SqlCommand(strSelHM, dbConn);
                    SqlDataReader MyQueryHM = dbCmd.ExecuteReader();
                    if (MyQueryHM.Read())
                    {
                        mParameter.Add(new Parameter(MyQueryHM["HUserName"].ToString(), MyQueryHM["HPersonID"].ToString(), MyQueryHM["HSex"].ToString()));
                    }
                    MyQueryHM.Close();
                }
                Content.mParameter = mParameter;

                //Repeater綁定
                Rpt_HCBM.DataSource = Content.mParameter;
                Rpt_HCBM.DataBind();
            }





        }

        MyQueryHCB.Close();





        #region 訂單明細
        //WA20230929_加入顯示換課資訊


        if (NewOrder == true)
        {
            LB_Memo.Visible = true;
            //WE20230916_為了與訂單明細的顯示資訊一致，所以隱藏不顯示(補繳金額)的字樣
            Supplement.Visible = true;
            LB_OriPaid.Visible = true;

            //WA20230916_若為變更後訂單則需顯示原訂單已付總金額
            Tr_Original.Visible = true;

            SDS_BookingList.SelectCommand = "SELECT ROW_NUMBER() OVER (ORDER BY a.HOrderNumNew ASC) AS ROW, a.HOrderNumNew AS HOrderNum, a.HPMethod, a.HCourseDonate, a.HPoint, a.HDCode,  a.HDPoint,a.HCourseDonate, a.HCourseName AS DCName, a.HDateRange AS DCDateRange, a.HItemStatus,b.HCDeadline,  a.HStatus, a.HChangeStatus, a.HCourseIDNew AS HCourseID, a.HDateRange, b.HCourseNum, b.HCourseName, b.HDateRange,b.Location AS Location, b.HBCPoint, a.HAttendNew  AS HAttend, a.HPAmount, a.HSubTotal, '' AS HBookedDate FROM HCBChangeRecord AS a LEFT JOIN HCourseBooking AS c ON a.HOrderGroupNew = c.HOrderGroup LEFT JOIN CourseList AS b ON a.HCourseIDNew = b.HID WHERE c.HOrderGroup = '" + LB_HOrderGroup.Text + "'";

        }
        else
        {
            LB_Memo.Visible = false;
            Supplement.Visible = false;
            LB_OriPaid.Visible = false;

            //WA20240204_新增套裝課程判斷
            if (gStrCPkgYN == null)
            {
                //WA20250620_新增判斷是否開放單日報名
                if (HF_HBookByDateYN.Value == "1")
                {
                    SDS_BookingList.SelectCommand = "SELECT ROW_NUMBER() OVER(ORDER BY a.HDateRange ASC) AS ROW, '-' AS HOrderNum,STRING_AGG(a.HID, ',') AS HID, a.HPMethod, a.HCourseDonate, a.HCourseName AS DCName, a.HPayAmt, a.HDCode, a.HDPoint,a.HCourseDonate, STRING_AGG(a.HCourseID, ',') AS HCourseID, a.HCPkgHID, a.HCPkgName,  a.HPkgPrice, a.HPkgPAmount, a.HPoint,a.HPkgSubTotal, STRING_AGG(CASE WHEN c.HAttend = '1' THEN '參班' WHEN c.HAttend = '5' THEN '純護持' WHEN c.HAttend = '6' THEN '參班兼護持' ELSE '-' END, ',') AS HAttend, STRING_AGG(FORMAT(c.HDate, CASE WHEN YEAR(c.HDate) = YEAR(GETDATE()) THEN 'MM/dd' ELSE 'yyyy/MM/dd' END), ', ') WITHIN GROUP(ORDER BY c.HDate) AS HBookedDate, a.HPAmount, a.HSubTotal, a.HDateRange AS HDCDateRange, a.HItemStatus, a.HStatus,a.HChangeStatus, a.HCourseName, a.HDateRange AS DCDateRange,b.HCourseName, b.HDateRange,STRING_AGG(b.Location, ',') AS Location, b.HBCPoint FROM HCourseBooking AS a LEFT JOIN CourseList AS b ON a.HCourseID = b.HID INNER JOIN HCourseBooking_DateAttend AS c ON a.HID = c.HCourseBookingID WHERE a.HOrderGroup = '" + LB_HOrderGroup.Text + "' GROUP BY a.HPMethod, a.HCourseDonate, a.HPayAmt, a.HDCode, a.HDPoint,a.HCourseDonate, a.HCourseName, a.HDateRange, a.HPAmount, a.HSubTotal, a.HItemStatus, a.HStatus, a.HChangeStatus, a.HCPkgHID, a.HCPkgName, a.HPkgPrice, a.HPkgPAmount, a.HPoint, a.HPkgSubTotal,b.HCourseName,  b.HDateRange, b.HBCPoint";

                }
                else
                {
                    if (OrderType == "B")
                    {
                        SDS_BookingList.SelectCommand = "SELECT ROW_NUMBER() OVER (ORDER BY a.HOrderNum ASC) AS ROW, a.HID, a.HOrderNum, a.HAttend, a.HPMethod, a.HCourseDonate, a.HPoint, a.HSubTotal,  a.HDCode, a.HDPoint,a.HCourseDonate, a.HCourseName AS DCName, a.HDateRange AS DCDateRange, a.HStatus, b.HCourseNum, b.HCourseName, b.HDateRange, b.Location, b.HBCPoint, a.HItemStatus, '' AS HBookedDate FROM HCourseBooking AS a LEFT JOIN CourseList AS b ON a.HCourseID = b.HID WHERE a.HOrderNum = '" + LB_HOrderNum.Text + "'";
                    }
                    else
                    {
                        SDS_BookingList.SelectCommand = "SELECT ROW_NUMBER() OVER (ORDER BY a.HOrderNum ASC) AS ROW, a.HID, a.HOrderNum, a.HAttend, a.HPMethod, a.HCourseDonate, a.HPoint, a.HSubTotal,   a.HDCode, a.HDPoint,a.HCourseDonate, a.HCourseName AS DCName, a.HDateRange AS DCDateRange, a.HStatus, b.HCourseNum, b.HCourseName, b.HDateRange, b.Location, b.HBCPoint, a.HItemStatus, '' AS HBookedDate FROM HCourseBooking AS a LEFT JOIN CourseList AS b ON a.HCourseID = b.HID WHERE a.HOrderGroup = '" + LB_HOrderGroup.Text + "'";
                    }
                }


            }
            else
            {
                //WA20240204_套裝訂單的顯示

                SDS_HCPackageList.SelectCommand = "SELECT  a.HCPkgHID, a.HPMethod, a.HCourseDonate, a.HDCode, a.HDPoint,a.HCourseDonate, a.HItemStatus, b.HCDeadline, a.HStatus, a.HChangeStatus,a.HCPkgName, b.HCStartDate,  a.HAttend, a.HPkgPrice, a.HPkgPAmount, a.HPkgSubTotal, '' AS HBookedDate FROM HCourseBooking AS a LEFT JOIN HCoursePackage AS b ON a.HCPkgHID= b.HID WHERE a.HOrderGroup = '" + LB_HOrderGroup.Text + "' AND a.HCPkgYN=1 GROUP BY a.HCPkgHID,a.HPMethod, a.HCourseDonate, a.HDCode, a.HDPoint,a.HCourseDonate, a.HItemStatus, b.HCDeadline, a.HStatus, a.HChangeStatus,a.HCPkgName, b.HCStartDate, a.HAttend, a.HPkgPrice, a.HPkgPAmount, a.HPkgSubTotal";
                Rpt_HCPackageList.DataBind();

                LB_HCPkgHead.Visible = true;
                LB_HCStart.Visible = true;
            }
        }




        Rpt_BookingList.DataBind();




        #region 計算金額
        decimal cTotal = 0;
        decimal cDTotal = 0;

        for (int i = 0; i < Rpt_BookingList.Items.Count; i++)
        {
            cTotal += Convert.ToDecimal(((Label)Rpt_BookingList.Items[i].FindControl("LB_HPoint")).Text.Replace(",", ""));

            if (!string.IsNullOrEmpty(((Label)Rpt_BookingList.Items[i].FindControl("LB_HDPoint")).Text))
            {
                cDTotal += Convert.ToDecimal(((Label)Rpt_BookingList.Items[i].FindControl("LB_HDPoint")).Text.Replace(",", ""));
            }
        }

        //WA20240204_新增套裝課程金額加總
        if (gStrCPkgYN == "1")
        {
            //套裝課程
            for (int i = 0; i < Rpt_HCPackageList.Items.Count; i++)
            {
                cTotal += Convert.ToDecimal(((Label)Rpt_HCPackageList.Items[i].FindControl("LB_HPkgSubTotal")).Text.Replace(",", ""));

                if (!string.IsNullOrEmpty(((Label)Rpt_HCPackageList.Items[i].FindControl("LB_HDPoint")).Text))
                {
                    cDTotal += Convert.ToDecimal(((Label)Rpt_HCPackageList.Items[i].FindControl("LB_HDPoint")).Text.Replace(",", ""));
                }
            }
        }

        LB_SubTotal.Text = cTotal.ToString("N0");
        LB_DTotal.Text = cDTotal.ToString("N0");
        if (!string.IsNullOrEmpty(LB_PTotal.Text.Replace(",", "")))
        {
            LB_Total.Text = (cTotal - Convert.ToInt32(LB_PTotal.Text.Replace(",", ""))).ToString("N0");
        }

        if (NewOrder == true)
        {
            //WA20230916_原訂單已付總金額
            LB_Total.Visible = false;
            LB_AfterTotal.Visible = true;
            LB_Paid.Visible = false;
            LB_Supplementary.Visible = true;


            LB_OriPaidAmount.Text = (Convert.ToInt32(LB_Total.Text.Replace(",", "")) - Convert.ToInt32(LB_HPayAmt.Text.Replace(",", ""))).ToString("N0");
            //LB_AfterTotal.Text = Convert.ToInt32(LB_APayAmt.Text.Replace(",", "")).ToString("N0");

            //WE20230916_修改訂單總金額資訊
            LB_AfterTotal.Text = Convert.ToInt32(LB_HPayAmt.Text.Replace(",", "")).ToString("N0");

            Supplement.Visible = true;
            LB_Supplementary.Text = LB_HECPAmount.Text;

            //WE20230910_修改計算方式
            LB_Paid.Text = (Convert.ToInt32(LB_Total.Text.Replace(",", "")) - Convert.ToInt32(LB_HPayAmt.Text.Replace(",", ""))).ToString("N0");


        }
        else
        {
            //WA20230916_原訂單已付總金額
            LB_Total.Visible = true;
            LB_AfterTotal.Visible = false;
            LB_Paid.Visible = true;
            LB_Supplementary.Visible = false;


            Supplement.Visible = false;
            LB_Paid.Text = LB_HECPAmount.Text;
        }



        #endregion

        #endregion

        #region 發票明細
        SDS_Invoice.SelectCommand = "SELECT ROW_NUMBER() OVER (ORDER BY a.HOrderNum ASC) AS ROW, a.HOrderNum, a.HInvoiceNo, a.HInvoiceDate, a.HInvoiceStatus FROM HCourseBooking AS a LEFT JOIN CourseList AS b ON a.HCourseID = b.HID WHERE a.HOrderGroup = '" + LB_HOrderGroup.Text + "'";
        Rpt_Invoice.DataBind();

        #endregion


        decimal Total = 0;
        decimal CXLTotal = 0;

        for (int i = 0; i < Rpt_BookingList.Items.Count; i++)
        {
            Total += Convert.ToDecimal(((Label)Rpt_BookingList.Items[i].FindControl("LB_HPoint")).Text.Replace(",", ""));
            if (((Label)Rpt_BookingList.Items[i].FindControl("LB_HItemStatus")).Text == "已退款")
            {
                CXLTotal += Convert.ToDecimal(((Label)Rpt_BookingList.Items[i].FindControl("LB_HPoint")).Text.Replace(",", ""));
            }

        }
        LB_Total.Text = (Convert.ToDouble(Total)).ToString("N0");

        if (DDL_HStatus.SelectedValue == "1" && CXLTotal != 0)
        {
            Div_Refund.Visible = true;
            LB_HCXLTotal.Text = Convert.ToInt32(CXLTotal).ToString("N0");
        }



        dbConn.Close();
        dbCmd.Cancel();


















    }
    #endregion




    #region 變更後紀錄顯示
    protected void Rpt_ChangeRecordDetail_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ((Label)e.Item.FindControl("LB_HAttend")).Text = ((Label)e.Item.FindControl("LB_HAttend")).Text == "1" ? "參班" : ((Label)e.Item.FindControl("LB_HAttend")).Text == "5" ? "純護持(非班員)" : ((Label)e.Item.FindControl("LB_HAttend")).Text == "6" ? "參班兼護持" : "-";

        ///HItemStatus 1=已付款、2=未付款、3=已退款
        ((Label)e.Item.FindControl("LB_HItemStatus")).CssClass = ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "1" ? "text-success" : ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "2" ? "text-danger" : ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "3" ? "text-dark" : "text-default";

        ((Label)e.Item.FindControl("LB_HItemStatus")).Text = ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "1" ? "已付款" : ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "2" ? "未付款" : ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "3" ? "已退款" : "-";


        if (((Label)e.Item.FindControl("LB_HPMethod")).Text == "2" && ((Label)e.Item.FindControl("LB_HCorseDonate")).Text == "1")   //文化課程、捐款
        {
            //HPoint存的即是金額
            ((Label)e.Item.FindControl("LB_HPoint")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HPoint")).Text)).ToString("N0");
        }
        else if (((Label)e.Item.FindControl("LB_HPMethod")).Text == "2" && ((Label)e.Item.FindControl("LB_HCorseDonate")).Text == "0")    //文化課程、報名課程
        {
            //HPoint存的是點數，需轉成金額
            ((Label)e.Item.FindControl("LB_HPoint")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HPoint")).Text) * 10).ToString("N0");
        }
        else if (((Label)e.Item.FindControl("LB_HPMethod")).Text == "1")
        {
            //HPoint存的即是金額
            ((Label)e.Item.FindControl("LB_HPoint")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HPoint")).Text)).ToString("N0");
        }
        else if (((Label)e.Item.FindControl("LB_HPMethod")).Text == "2" && string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HCorseDonate")).Text))  //文化課程、報名課程(開放捐款以前的訂單)
        {
            //HPoint存的是點數，需轉成金額
            ((Label)e.Item.FindControl("LB_HPoint")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HPoint")).Text) * 10).ToString("N0");
        }

        //基本費用(點數*10轉成金額-不分繳費帳戶)
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HBCPoint")).Text))
        {
            ((Label)e.Item.FindControl("LB_HBCPoint")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HBCPoint")).Text) * 10).ToString("N0");
        }

        //WA20230916_折扣金額(HDPoint*10轉成金額)
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HDPoint")).Text))
        {
            ((Label)e.Item.FindControl("LB_HDPoint")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HDPoint")).Text) * 10).ToString("N0");
        }


        ((Label)e.Item.FindControl("LB_HPAmount")).Text = !string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HPAmount")).Text) ? (Convert.ToInt32(((Label)e.Item.FindControl("LB_HPAmount")).Text)).ToString("N0") : ((Label)e.Item.FindControl("LB_HPAmount")).Text;
    }



    protected void Rpt_ChangeRecord_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        if (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HOrderGroupNew")).Text) || ((Label)e.Item.FindControl("LB_HOrderGroupNew")).Text == "")
        {
            ((SqlDataSource)e.Item.FindControl("SDS_ChangeRecordDetail")).SelectCommand = "SELECT ROW_NUMBER() OVER (ORDER BY a.HOrderNumSrc ASC) AS ROW, a.HOrderNumSrc AS HOrderNum, a.HPMethod, a.HCourseDonate, a.HPoint, a.HDCode, a.HDPoint,a.HCourseDonate, a.HCourseName AS HDCName, a.HDateRange AS HDCDateRange, a.HItemStatus,b.HCDeadline,  a.HStatus, c.HChangeStatus, a.HCourseIDNew AS HCourseID, a.HDateRange, b.HCourseNum, b.HCourseName, b.HDateRange,b.Location AS HOCPlace, b.HBCPoint, a.HAttendNew  AS HAttend, a.HPAmount, a.HSubTotal FROM HCBChangeRecord AS a LEFT JOIN HCourseBooking AS c ON  (a.HOrderGroupSrc = c.HOrderGroup AND a.HOrderNumSrc= c.HOrderNum) LEFT JOIN CourseList AS b ON a.HCourseIDNew = b.HID WHERE a.HOrderGroupSrc = '" + ((Label)e.Item.FindControl("LB_HOrderGroupSrc")).Text + "' AND a.HCreateDT='" + gDRV["ChangeDT"].ToString() + "' AND a.HOrderGroupNew =''";


            ((Repeater)e.Item.FindControl("Rpt_ChangeRecordDetail")).DataBind();
        }
        else
        {
            ((SqlDataSource)e.Item.FindControl("SDS_ChangeRecordDetail")).SelectCommand = "SELECT ROW_NUMBER() OVER (ORDER BY a.HOrderNumNew ASC) AS ROW, a.HOrderNumNew AS HOrderNum, a.HPMethod, a.HCourseDonate, a.HPoint, a.HDCode, a.HDPoint,a.HCourseDonate, a.HCourseName AS HDCName, a.HDateRange AS HDCDateRange, a.HItemStatus,b.HCDeadline,  a.HStatus, c.HChangeStatus, a.HCourseIDNew AS HCourseID, a.HDateRange, b.HCourseNum, b.HCourseName, b.HDateRange,b.Location AS HOCPlace, b.HBCPoint, a.HAttendNew  AS HAttend, a.HPAmount, a.HSubTotal FROM HCBChangeRecord AS a LEFT JOIN HCourseBooking AS c ON a.HOrderGroupNew = c.HOrderGroup LEFT JOIN CourseList AS b ON a.HCourseIDNew = b.HID WHERE c.HOrderGroup = '" + ((Label)e.Item.FindControl("LB_HOrderGroupNew")).Text + "'";
            ((Repeater)e.Item.FindControl("Rpt_ChangeRecordDetail")).DataBind();
        }


        ((Label)e.Item.FindControl("LB_BalancePaid")).Text = (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HOrderGroupNew")).Text) || ((Label)e.Item.FindControl("LB_HOrderGroupNew")).Text == "") ? "0" : ((Label)e.Item.FindControl("LB_BalancePaid")).Text;


        ((Label)e.Item.FindControl("LB_HOrderGroupNew")).Text = (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HOrderGroupNew")).Text) || ((Label)e.Item.FindControl("LB_HOrderGroupNew")).Text == "") ? ((Label)e.Item.FindControl("LB_HOrderGroupSrc")).Text : ((Label)e.Item.FindControl("LB_HOrderGroupNew")).Text;






    }

    #endregion

    protected void Rpt_BookingList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (((Label)e.Item.FindControl("LB_HPMethod")).Text == "2" && ((Label)e.Item.FindControl("LB_HCorseDonate")).Text == "1")   //文化課程、捐款
        {
            //HPoint存的即是金額
            ((Label)e.Item.FindControl("LB_HPoint")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HPoint")).Text)).ToString("N0");
        }
        else if (((Label)e.Item.FindControl("LB_HPMethod")).Text == "2" && ((Label)e.Item.FindControl("LB_HCorseDonate")).Text == "0")    //文化課程、報名課程
        {
            //HPoint存的是點數，需轉成金額
            ((Label)e.Item.FindControl("LB_HPoint")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HPoint")).Text) * 10).ToString("N0");
        }
        else if (((Label)e.Item.FindControl("LB_HPMethod")).Text == "1")
        {
            //HPoint存的即是金額
            ((Label)e.Item.FindControl("LB_HPoint")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HPoint")).Text)).ToString("N0");
        }
        else if (((Label)e.Item.FindControl("LB_HPMethod")).Text == "2" && string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HCorseDonate")).Text))  //文化課程、報名課程(開放捐款以前的訂單)
        {
            //HPoint存的是點數，需轉成金額
            ((Label)e.Item.FindControl("LB_HPoint")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HPoint")).Text) * 10).ToString("N0");
        }

        //折扣金額
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HDPoint")).Text))
        {
            ((Label)e.Item.FindControl("LB_HDPoint")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HDPoint")).Text) * 10).ToString("N0");
        }

        //基本費用
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HBCPoint")).Text))
        {
            ((Label)e.Item.FindControl("LB_HBCPoint")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HBCPoint")).Text) * 10).ToString("N0");
        }

        //捐款
        if (((Label)e.Item.FindControl("LB_HCourseDonate")).Text == "1")  //捐款
        {
            ((Label)e.Item.FindControl("LB_HCourseName")).Text = ((Label)e.Item.FindControl("LB_DCName")).Text + "<span style='color:#30a3fe;'> ( 捐款 ) </span>";
            ((Label)e.Item.FindControl("LB_HDateRange")).Text = ((Label)e.Item.FindControl("LB_DCDateRange")).Text;
        }


        //參班與否
        if (HF_HBookByDateYN.Value != "1")
        {
            if (((Label)e.Item.FindControl("LB_HAttend")).Text == "1")
            {
                ((Label)e.Item.FindControl("LB_HAttend")).Text = "參班";
            }
            else if (((Label)e.Item.FindControl("LB_HAttend")).Text == "5")
            {
                //WE20231017_護持體系專業改成純護持(非班員)
                // ((Label)e.Item.FindControl("LB_HAttend")).Text = "護持體系專業";
                ((Label)e.Item.FindControl("LB_HAttend")).Text = "純護持(非班員)";
            }
            else if (((Label)e.Item.FindControl("LB_HAttend")).Text == "6")
            {
                ((Label)e.Item.FindControl("LB_HAttend")).Text = "參班兼護持";
            }
            else
            {
                ((Label)e.Item.FindControl("LB_HAttend")).Text = "-";
            }

        }
        else
        {
            //顯示報名的上課日期
            LB_Booked.Visible = true;
            ((Label)e.Item.FindControl("LB_HDateRange")).Visible = false;
            ((Label)e.Item.FindControl("LB_HBookedDate")).Visible = true;
        }




        ////訂單狀態HStatus  
        /////HStatus 1=訂單成立、2=訂單取消
        // 付款狀態 HItemStatus
        //1=已付款；2=未付款；3=已退款
        ((Label)e.Item.FindControl("LB_HItemStatus")).CssClass = ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "1" ? "text-success" : ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "2" ? "text-danger" : ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "3" ? "text-gray" : ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "4" ? "text-info" : "text-danger";

        ((Label)e.Item.FindControl("LB_HItemStatus")).Text = ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "1" ? "已付款" : ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "2" ? "未付款" : ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "3" ? "已退款" : "";


    }

    protected void Rpt_HCMCB_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        #region //訂單狀態
        ///230707- 更改: HStatus 1=訂單成立、2=訂單取消
        ((Label)e.Item.FindControl("LB_HStatus")).CssClass = ((Label)e.Item.FindControl("LB_HStatus")).Text == "1" ? "label label-success" : ((Label)e.Item.FindControl("LB_HStatus")).Text == "2" ? "label label-default bg-gray" : "label label-default bg-gray";

        ((Label)e.Item.FindControl("LB_HStatus")).Text = ((Label)e.Item.FindControl("LB_HStatus")).Text == "1" ? "訂單成立" : ((Label)e.Item.FindControl("LB_HStatus")).Text == "2" ? "訂單取消" : "";


        #endregion

        #region //付款狀態判斷

        string ItemStatus = null;
        string CXLHandleStatus = null;
        int Nums = 0;

        SqlDataReader QueryItemStatus = SQLdatabase.ExecuteReader("SELECT HItemStatus, HCXLHandleStatus FROM HCourseBooking WHERE HOrdergroup='" + ((Label)e.Item.FindControl("LB_HOrderGroup")).Text + "'");

        while (QueryItemStatus.Read())
        {
            ItemStatus += QueryItemStatus["HItemStatus"].ToString();
            //CXLHandleStatus += QueryItemStatus["HCXLHandleStatus"].ToString();
            Nums += 1;
        }
        QueryItemStatus.Close();


        SqlDataReader QueryCXLHandleStatus = SQLdatabase.ExecuteReader("SELECT HCXLOrderGroup,HCXLHandleStatus FROM HCourseBooking WHERE HOrdergroup = '" + ((Label)e.Item.FindControl("LB_HOrderGroup")).Text + "'AND(HCXLHandleStatus <> null OR HCXLHandleStatus != '') GROUP BY HCXLOrderGroup, HCXLHandleStatus");

        if (QueryCXLHandleStatus.Read())
        {
            CXLHandleStatus = QueryCXLHandleStatus["HCXLHandleStatus"].ToString();
        }
        QueryCXLHandleStatus.Close();

        char ch = '3';  //退款
        char success = '1';  //已付款
        char none = '2';  //未付款
        char handle = '3';  //已處理	///HCXLHandleStatus : 1 = 待處理、2 = 處理中、3 = 已處理、4 = 取消

        int freq = ItemStatus.Count(f => (f == ch));  //計算字符串中給定字符的出現次數
        if (freq == Nums && CXLHandleStatus == "3")
        {
            ((Label)e.Item.FindControl("LB_HItemStatus")).Text = "已全部退款";
            ((Label)e.Item.FindControl("LB_HItemStatus")).CssClass = "label label-default bg-gray";
        }
        else if (freq < Nums && CXLHandleStatus == "3")
        {
            ((Label)e.Item.FindControl("LB_HItemStatus")).Text = "已部分退款";
            ((Label)e.Item.FindControl("LB_HItemStatus")).CssClass = "label label-default bg-gray";
        }

        int successfreq = ItemStatus.Count(f => (f == success));  //計算字符串中給定字符的出現次數
        if (successfreq == Nums)
        {
            ((Label)e.Item.FindControl("LB_HItemStatus")).Text = "已付款";
            ((Label)e.Item.FindControl("LB_HItemStatus")).CssClass = "label label-success";
        }


        if (CXLHandleStatus == "1" || CXLHandleStatus == "2")
        {
            ((Label)e.Item.FindControl("LB_HItemStatus")).Text = "退款處理中";
            ((Label)e.Item.FindControl("LB_HItemStatus")).CssClass = "label label-danger";
        }

        int nonefreq = ItemStatus.Count(f => (f == none));  //計算字符串中給定字符的出現次數
        if (nonefreq == Nums)
        {
            ((Label)e.Item.FindControl("LB_HItemStatus")).Text = "未付款";
            ((Label)e.Item.FindControl("LB_HItemStatus")).CssClass = "label label-info";
        }


        if (((Label)e.Item.FindControl("LB_HItemStatus")).Text == "已部分退款")
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "訂單成立";
            ((Label)e.Item.FindControl("LB_HStatus")).CssClass = "label label-success";
        }

        #endregion


        //金額轉成千分位
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HPayAmt")).Text))
        {
            ((Label)e.Item.FindControl("LB_HPayAmt")).Text = Convert.ToInt32(((Label)e.Item.FindControl("LB_HPayAmt")).Text).ToString("N0");
        }

        if (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_PaymentDate")).Text) || ((Label)e.Item.FindControl("LB_PaymentDate")).Text == "")
        {
            if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HPayAmt")).Text) && !string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HUse")).Text))
            {
                if (Convert.ToInt32(((Label)e.Item.FindControl("LB_HUse")).Text) * 10 == Convert.ToInt32(((Label)e.Item.FindControl("LB_HPayAmt")).Text.Replace(",", "")))
                {
                    ((Label)e.Item.FindControl("LB_HPayAmt")).Text = "0";
                }
            }
        }

        if (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_PaymentDate")).Text))
        {
            ((Label)e.Item.FindControl("LB_HPayAmt")).Text = "0";
        }


        //應繳總金額
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_Total")).Text) && ((Label)e.Item.FindControl("LB_Total")).Text != "0")
        {
            ((Label)e.Item.FindControl("LB_Total")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_Total")).Text)).ToString("N0");
        }

        if (((Label)e.Item.FindControl("LB_Total")).Text == "0")
        {
            ((Label)e.Item.FindControl("LB_PaymentDate")).Text = ((Label)e.Item.FindControl("LB_HCreateDT")).Text;
        }


        //WA20230914_判斷訂單的繳費帳戶
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HOrderGroup")).Text))
        {
            if (((Label)e.Item.FindControl("LB_HOrderGroup")).Text.Substring(0, 1).Trim() == "F")
            {
                ((Label)e.Item.FindControl("LB_HPMethod")).Text = "基金會";
            }
            else if (((Label)e.Item.FindControl("LB_HOrderGroup")).Text.Substring(0, 1).Trim() == "C")
            {
                ((Label)e.Item.FindControl("LB_HPMethod")).Text = "文化事業";
            }
            else if (((Label)e.Item.FindControl("LB_HOrderGroup")).Text.Substring(0, 1).Trim() == "O") //混合基金會和文化事業
            {
                //
                SqlDataReader QueryDonate = SQLdatabase.ExecuteReader("SELECT HPMethod, HCourseDonate FROM HCourseBooking WHERE HOrderGroup='" + ((Label)e.Item.FindControl("LB_HOrderGroup")).Text + "'");
                while (QueryDonate.Read())
                {
                    if (QueryDonate["HPMethod"].ToString() == "2" && QueryDonate["HCourseDonate"].ToString() == "1")  //文化+捐款
                    {
                        ((Label)e.Item.FindControl("LB_HPMethod")).Text = "基金會";
                    }
                    else if (QueryDonate["HPMethod"].ToString() == "1")  //基金會
                    {
                        ((Label)e.Item.FindControl("LB_HPMethod")).Text = "基金會";
                    }

                }
                QueryDonate.Close();


            }

        }
        else  //B開頭 (最原始的資料)
        {
            if (((Label)e.Item.FindControl("LB_HPMethod")).Text.Substring(0, 1).Trim() == "1")
            {
                ((Label)e.Item.FindControl("LB_HPMethod")).Text = "基金會";
            }
            else
            {
                ((Label)e.Item.FindControl("LB_HPMethod")).Text = "文化事業";
            }
        }

    }


    protected void Rpt_TWE_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ((Label)e.Item.FindControl("LB_HPMethod")).Text = ((Label)e.Item.FindControl("LB_HPMethod")).Text == "1" ? "基金會" : ((Label)e.Item.FindControl("LB_HPMethod")).Text == "2" ? "文化事業" : "無";

        ((Label)e.Item.FindControl("LB_HCourseDonate")).Text = ((Label)e.Item.FindControl("LB_HCourseDonate")).Text == "0" ? "否" : "是";
        ((Label)e.Item.FindControl("LB_HInvoiceStatus")).Text = ((Label)e.Item.FindControl("LB_HInvoiceStatus")).Text == "1" ? "已開立" : ((Label)e.Item.FindControl("LB_HInvoiceStatus")).Text == "2" ? "已作廢" : ((Label)e.Item.FindControl("LB_HInvoiceStatus")).Text == "3" ? "已折讓" : "-";


        ////訂單狀態HStatus-220917更新
        ///0=訂單取消(已付款)、1=訂單成立(已付款)、2=未付款訂單、3=訂單取消(未付款)、4=訂單取消(已退款/點)
        //((Label)e.Item.FindControl("LB_HStatus")).Text = ((Label)e.Item.FindControl("LB_HStatus")).Text == "0" ? "訂單取消" : ((Label)e.Item.FindControl("LB_HStatus")).Text == "1" ? "訂單成立" : ((Label)e.Item.FindControl("LB_HStatus")).Text == "2" ? "未付款訂單" : "";

        HtmlGenericControl Status = e.Item.FindControl("Status") as HtmlGenericControl;
        if (((Label)e.Item.FindControl("LB_HStatus")).Text == "0")
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "訂單取消(已付款)";
            Status.Style.Add("background-color", "#888 !important");
            Status.Attributes.Add("class", "label  label-default");
        }
        else if (((Label)e.Item.FindControl("LB_HStatus")).Text == "1")
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "訂單成立(已付款)";
            Status.Attributes.Add("class", "label  label-success");
        }
        else if (((Label)e.Item.FindControl("LB_HStatus")).Text == "2")
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "未付款訂單";
            Status.Attributes.Add("class", "label  label-info");
        }
        else if (((Label)e.Item.FindControl("LB_HStatus")).Text == "3")
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "訂單取消(未付款)";
            Status.Style.Add("background-color", "#888 !important");
            Status.Attributes.Add("class", "label  label-info");
        }
        else if (((Label)e.Item.FindControl("LB_HStatus")).Text == "4")
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "訂單取消(已退款/點)";
            Status.Style.Add("background-color", "#9e9e9e !important");
            Status.Attributes.Add("class", "label  label-info");
        }


        //220707-參班與否
        ((Label)e.Item.FindControl("LB_HAttend")).Text = ((Label)e.Item.FindControl("LB_HAttend")).Text == "1" ? "參班" : ((Label)e.Item.FindControl("LB_HAttend")).Text == "5" ? "純護持(非班員)" : ((Label)e.Item.FindControl("LB_HAttend")).Text == "6" ? "參班兼護持" : "-";

        //220707-折扣碼使用
        ((Label)e.Item.FindControl("LB_HDCode")).Text = ((Label)e.Item.FindControl("LB_HDCode")).Text == "" ? "無" : ((Label)e.Item.FindControl("LB_HDCode")).Text;

        //220707-判斷基金會或文化事業:若為基金會，Price=HPoint

        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_Price")).Text))
        {
            ((Label)e.Item.FindControl("LB_Price")).Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)e.Item.FindControl("LB_Price")).Text));
        }

        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HPayAmt")).Text))
        {
            ((Label)e.Item.FindControl("LB_HPayAmt")).Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)e.Item.FindControl("LB_HPayAmt")).Text));
        }

       

    }







    #region 儲存功能
    protected void Btn_Submit_Click(object sender, EventArgs e)
    {


        //220701-暫時註解，未來要再打開
        int gCT = 1;
        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        //HStatus→0=訂單取消(已付款)、1=訂單成立(已付款)、2=未付款訂單、3=訂單取消(未付款)、4=訂單取消(已退款/點)
        //string strUpdHC = "update HCourseBooking set HStatus='" + DDL_HStatus.SelectedValue + "', HFinanceRemark='" + TB_HFinanceRemark.Text.Trim()+ "', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' where HID = '" + LB_HID.Text + "'";

        string strUpdHC = null;
        if (string.IsNullOrEmpty(LB_HOrderGroup.Text))
        {
            strUpdHC = "UPDATE HCourseBooking SET HStatus='" + DDL_HStatus.SelectedValue + "', HFinanceRemark='" + TB_HFinanceRemark.Text.Trim() + "', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HOrderNum = '" + LB_HOrderNum.Text + "'";
        }
        else
        {
            strUpdHC = "UPDATE HCourseBooking SET HStatus='" + DDL_HStatus.SelectedValue + "', HFinanceRemark='" + TB_HFinanceRemark.Text.Trim() + "', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HOrderGroup = '" + LB_HOrderGroup.Text + "'";



        }




        dbCmd = new SqlCommand(strUpdHC, dbConn);
        dbCmd.ExecuteReader();

        Response.Write("<script>alert('訂單存檔成功!');window.location.href='HOrder_Edit.aspx';</script>");


    }
    #endregion




    protected void LBtn_Del_Click(object sender, EventArgs e)
    {

    }



    protected void Btn_Cancel_Click(object sender, EventArgs e)
    {
        Response.Write("<script>window.location.href='HOrder_Edit.aspx';</script>");
    }



    protected void Rpt_Invoice_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ((Label)e.Item.FindControl("LB_HInvoiceStatus")).Text = ((Label)e.Item.FindControl("LB_HInvoiceStatus")).Text == "1" ? "已開立" : ((Label)e.Item.FindControl("LB_HInvoiceStatus")).Text == "0" ? "尚未開立" : ((Label)e.Item.FindControl("LB_HInvoiceStatus")).Text == "2" ? "已作廢" : ((Label)e.Item.FindControl("LB_HInvoiceStatus")).Text == "3" ? "已折讓" : "-";
    }


    #region 訂單明細-顯示套裝課程資訊
    //WA20240204_新增
    protected void Rpt_HCPackageList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ((Label)e.Item.FindControl("LB_HPkgSubTotal")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HPkgSubTotal")).Text)).ToString("N0");

        //折扣金額
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HDPoint")).Text))
        {
            ((Label)e.Item.FindControl("LB_HDPoint")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HDPoint")).Text) * 10).ToString("N0");
        }

        //套裝基本費用
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HPkgPrice")).Text))
        {
            ((Label)e.Item.FindControl("LB_HPkgPrice")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HPkgPrice")).Text)).ToString("N0");
        }


        //捐款
        if (((Label)e.Item.FindControl("LB_HCourseDonate")).Text == "1")  //捐款
        {
            ((Label)e.Item.FindControl("LB_HCPkgName")).Text = ((Label)e.Item.FindControl("LB_HCPkgName")).Text + "<span style='color:#611673;'> ( 捐款 ) </span>";
            ((Label)e.Item.FindControl("LB_HDateRange")).Text = ((Label)e.Item.FindControl("LB_HDateRange")).Text;
        }


        //參班身分
        ((Label)e.Item.FindControl("LB_HAttend")).Text = ((Label)e.Item.FindControl("LB_HAttend")).Text == "1" ? "參班" : ((Label)e.Item.FindControl("LB_HAttend")).Text == "5" ? "純護持(非班員)" : ((Label)e.Item.FindControl("LB_HAttend")).Text == "6" ? "參班兼護持" : "-";

        //	//付款狀態HItemStatus
        ///1=已付款、2=未付款、3=已退款
        ((Label)e.Item.FindControl("LB_HItemStatus")).CssClass = ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "1" ? "text-success" : ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "2" ? "text-danger" : ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "3" ? "text-dark" : "text-default";

        ((Label)e.Item.FindControl("LB_HItemStatus")).Text = ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "1" ? "已付款" : ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "2" ? "未付款" : ((Label)e.Item.FindControl("LB_HItemStatus")).Text == "3" ? "已退款" : "-";


    }
    #endregion

    #region 取消報名
    //WA20250516_Add
    protected void LBtn_Refund_Click(object sender, EventArgs e)
    {
        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;



        //判斷是否為0元訂單
        if (HF_HBookByDateYN.Value == "1")
        {

            SDS_RHCBookingList.SelectCommand = "SELECT ROW_NUMBER() OVER(ORDER BY a.HDateRange ASC) AS ROW, '-' AS HOrderNum,STRING_AGG(a.HID, ',') AS HID,  a.HCourseName AS DCName, a.HPayAmt,  a.HPoint,a.HPkgSubTotal, a.HPAmount, a.HSubTotal, a.HDateRange AS HDCDateRange, a.HItemStatus, a.HStatus,a.HChangeStatus, a.HCourseName, a.HDateRange AS DCDateRange,b.HCourseName, b.HDateRange,STRING_AGG(b.Location, ',') AS Location, b.HBCPoint, a.HCXLAmount, a.HCourseDonate FROM HCourseBooking AS a LEFT JOIN CourseList AS b ON a.HCourseID = b.HID INNER JOIN HCourseBooking_DateAttend AS c ON a.HID = c.HCourseBookingID WHERE a.HOrderGroup = '" + LB_HOrderGroup.Text + "' AND a.HBookByDateYN=1 GROUP BY  a.HPayAmt, a.HCourseName, a.HDateRange, a.HPAmount, a.HSubTotal, a.HItemStatus, a.HStatus, a.HChangeStatus, a.HPoint, b.HCourseName, b.HDateRange,b.HBCPoint, a.HPkgSubTotal, a.HBookByDateYN, a.HCXLAmount, a.HCourseDonate";
            Rpt_RHCBookingList.DataBind();

            for (int i = 0; i < Rpt_RHCBookingList.Items.Count; i++)
            {
                //只能整筆取消
                ((CheckBox)Rpt_RHCBookingList.Items[i].FindControl("CB_CancelSelect")).Checked = true;
                ((CheckBox)Rpt_RHCBookingList.Items[i].FindControl("CB_CancelSelect")).Enabled = false;
                ((Label)Rpt_RHCBookingList.Items[i].FindControl("LB_HPoint")).Visible = true;
                ((Label)Rpt_RHCBookingList.Items[i].FindControl("LB_HSubTotal")).Visible = false;
            }
          
        }
        else
        {

            SDS_RHCBookingList.SelectCommand = "SELECT ROW_NUMBER() OVER (ORDER BY a.HOrderNum ASC) AS ROW, a.HID, a.HOrderNum, a.HAttend, a.HPMethod, a.HCourseDonate, a.HPoint, a.HSubTotal, a.HDCode, a.HDPoint,a.HCourseDonate, a.HCourseName AS DCName, a.HDateRange AS DCDateRange, a.HStatus, b.HCourseNum, b.HCourseName, b.HDateRange, b.Location, b.HBCPoint, a.HItemStatus, '' AS HBookedDate, a.HCXLAmount FROM HCourseBooking AS a LEFT JOIN CourseList AS b ON a.HCourseID = b.HID WHERE a.HOrderGroup = '" + LB_HOrderGroup.Text + "'";
            Rpt_RHCBookingList.DataBind();

            for (int i = 0; i < Rpt_RHCBookingList.Items.Count; i++)
            {
                if (((Label)Rpt_RHCBookingList.Items[i].FindControl("LB_HItemStatus")).Text == "3")  //已退款
                {
                    ((CheckBox)Rpt_RHCBookingList.Items[i].FindControl("CB_CancelSelect")).Enabled = false;
                    ((TextBox)Rpt_RHCBookingList.Items[i].FindControl("TB_HCXLRefund")).Enabled = false;
                    ((TextBox)Rpt_RHCBookingList.Items[i].FindControl("TB_HCXLRefund")).Text = ((Label)Rpt_RHCBookingList.Items[i].FindControl("LB_HCXLAmount")).Text;
                }
                else
                {
                    ((CheckBox)Rpt_RHCBookingList.Items[i].FindControl("CB_CancelSelect")).Enabled = true; 
                }

            }



        }

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_CancelList').modal('show');</script>", false);//執行js

    }
    #endregion

    #region 確認取消報名
    protected void LBtn_RSubmit_Click(object sender, EventArgs e)
    {

        if (HF_HBookByDateYN.Value == "1")
        {
            for (int i = 0; i < Rpt_RHCBookingList.Items.Count; i++)
            {
                if (((CheckBox)Rpt_RHCBookingList.Items[i].FindControl("CB_CancelSelect")).Checked == true)
                {
                    SqlCommand QueryUpdHCB1 = new SqlCommand("UPDATE HCourseBooking SET HCXLTotal=@HCXLTotal, HItemStatus=@HItemStatus, HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT, HCXLFinishDate=@HCXLFinishDate, HCXLHandleStatus=@HCXLHandleStatus WHERE HOrderGroup=@HOrderGroup AND HBookByDateYN=1;", SQLdatabase.OpenConnection());
                    QueryUpdHCB1.Parameters.AddWithValue("@HOrderGroup", LB_HOrderGroup.Text);
                    //HItemStatus：1=已付款、2=未付款、3=已退款
                    QueryUpdHCB1.Parameters.AddWithValue("@HItemStatus", "3");
                    //HStatus：1=訂單成立、2=訂單取消
                    QueryUpdHCB1.Parameters.AddWithValue("@HStatus", "2");
                    //HCXLHandleStatus：1=待處理、2=處理中、3=已處理、4=取消
                    QueryUpdHCB1.Parameters.AddWithValue("@HCXLHandleStatus", "3");
                    QueryUpdHCB1.Parameters.AddWithValue("@HCXLTotal", Convert.ToInt32(((TextBox)Rpt_RHCBookingList.Items[i].FindControl("TB_HCXLRefund")).Text.Trim()));
                    QueryUpdHCB1.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    QueryUpdHCB1.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    QueryUpdHCB1.Parameters.AddWithValue("@HCXLFinishDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    QueryUpdHCB1.ExecuteNonQuery();
                    QueryUpdHCB1.Cancel();
                    SQLdatabase.OpenConnection().Close();
                }
            }

            Response.Write("<script>alert('您已完成取消訂單!');window.location.href='HOrder_Edit.aspx'</script>");
        }
        else
        {
            //若為訂單成立未付款或訂單金額為0元則可直接取消訂單
            if (DDL_HStatus.SelectedValue == "1" && LB_HPayAmt.Text == "0")
            {

                for (int i = 0; i < Rpt_RHCBookingList.Items.Count; i++)
                {
                    if (((CheckBox)Rpt_RHCBookingList.Items[i].FindControl("CB_CancelSelect")).Checked == true)
                    {
                        SqlCommand QueryUpdHCB1 = new SqlCommand("UPDATE HCourseBooking SET HItemStatus=@HItemStatus, HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT,HCXLFinishDate=@HCXLFinishDate, HCXLHandleStatus=@HCXLHandleStatus WHERE HOrderGroup=@HOrderGroup AND HID=@HID;", SQLdatabase.OpenConnection());
                        QueryUpdHCB1.Parameters.AddWithValue("@HID", ((Label)Rpt_RHCBookingList.Items[i].FindControl("LB_HID")).Text);
                        QueryUpdHCB1.Parameters.AddWithValue("@HOrderGroup", LB_HOrderGroup.Text);
                        //HItemStatus：1=已付款、2=未付款、3=已退款
                        QueryUpdHCB1.Parameters.AddWithValue("@HItemStatus", "3");
                        //HStatus：1=訂單成立、2=訂單取消
                        QueryUpdHCB1.Parameters.AddWithValue("@HStatus", "2");
                        //HCXLHandleStatus：1=待處理、2=處理中、3=已處理、4=取消
                        QueryUpdHCB1.Parameters.AddWithValue("@HCXLHandleStatus", "3");
                        QueryUpdHCB1.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        QueryUpdHCB1.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        QueryUpdHCB1.Parameters.AddWithValue("@HCXLFinishDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        QueryUpdHCB1.ExecuteNonQuery();
                        QueryUpdHCB1.Cancel();
                        SQLdatabase.OpenConnection().Close();
                    }


                }

                Response.Write("<script>alert('您已完成取消訂單!');window.location.href='HOrder_Edit.aspx'</script>");
                //return;
            }
            else
            {
                //必填判斷

                int gHCXLTotal = 0;
                int gCancelSelect = 0;

                for (int i = 0; i < Rpt_RHCBookingList.Items.Count; i++)
                {
                    if (((CheckBox)Rpt_RHCBookingList.Items[i].FindControl("CB_CancelSelect")).Checked)
                    {
                        gCancelSelect += 1;


                        if (string.IsNullOrEmpty(((TextBox)Rpt_RHCBookingList.Items[i].FindControl("TB_HCXLRefund")).Text.Trim()))
                        {
                            Response.Write(@"<script>alert('有勾選要取消的課程要先填寫退款金額哦~!');</script>");
                            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_CancelList').modal('show');</script>", false);//執行js
                            return;
                        }
                        else
                        {


                            if (Convert.ToInt32(((TextBox)Rpt_RHCBookingList.Items[i].FindControl("TB_HCXLRefund")).Text.Trim()) > Convert.ToInt32(LB_HECPAmount.Text))
                            {
                                Response.Write(@"<script>alert('退款金額不能大於付款金額哦~!');</script>");
                                ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_CancelList').modal('show');</script>", false);//執行js
                                return;
                            }
                            else
                            {
                                gHCXLTotal += Convert.ToInt32(((TextBox)Rpt_RHCBookingList.Items[i].FindControl("TB_HCXLRefund")).Text.Trim());
                            }
                        }
                    }
                }

                //HStatus：1=訂單成立、2=訂單取消
                string gHStatus = "";//如果退款金額>=訂單總金額，則訂單狀態(HStatus)改為：2 (訂單取消)
                if (gHCXLTotal >= Convert.ToInt32(LB_HECPAmount.Text) || gCancelSelect == Rpt_RHCBookingList.Items.Count)
                {
                    gHStatus = "2";
                }
                else
                {
                    gHStatus = "1";
                }

                //判斷是否勾選要取消的課程
                string gCB_CancelSelect = "0";

                //WA20240204_新增是否為套裝課程的判斷
                if (LB_HCPkgYN.Text != "1")
                {
                    for (int j = 0; j <= Rpt_RHCBookingList.Items.Count - 1; j++)
                    {
                        //是否至少勾選1個項目
                        if (((CheckBox)Rpt_RHCBookingList.Items[j].FindControl("CB_CancelSelect")).Checked == true)
                        {
                            gCB_CancelSelect = "1";
                        }
                    }


                    if (gCB_CancelSelect != "1")
                    {
                        ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('請勾選需退款項目!');", true);
                        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_CancelList').modal('show');</script>", false);//執行
                        return;
                    }
                }
                else  //如果是套裝課程
                {
                    for (int j = 0; j <= Rpt_RHCPackageList.Items.Count - 1; j++)
                    {
                        //是否至少勾選1個項目
                        if (((CheckBox)Rpt_RHCPackageList.Items[j].FindControl("CB_CancelSelect")).Checked == true)
                        {
                            gCB_CancelSelect = "1";
                        }
                    }


                    if (gCB_CancelSelect != "1")
                    {
                        ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('請勾選需退款項目!');", true);
                        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_CancelList').modal('show');</script>", false);//執行
                        return;
                    }
                }

                string gHOrderStatus = "";
                string gHItemStatus = "";
                SqlDataReader QuerySelHCBP = SQLdatabase.ExecuteReader("SELECT ROW_NUMBER() OVER (ORDER BY a.HOrderGroup DESC) AS ROW, a.HOrderGroup, a.HPayMethod,  a.HPaymentDate, a.HPayAmt, a.HStatus, b.HUse, a.HMerchantTradeNo, a.HTradeNo, a.HPayAmt, a.HGwsr, a.HItemStatus FROM HCourseBooking AS a LEFT JOIN HPoints AS b ON a.HOrderGroup = b.HOrderGroup WHERE  a.HOrderGroup != '' AND a.HOrderGroup = '" + LB_HOrderGroup.Text + "' GROUP BY a.HOrderGroup, a.HPayMethod, a.HPaymentDate, a.HPayAmt, a.HStatus, b.HUse, a.HMerchantTradeNo, a.HTradeNo, a.HPayAmt, a.HGwsr, a.HItemStatus ORDER BY a.HOrderGroup DESC");

                if (QuerySelHCBP.Read())
                {
                    gHOrderStatus = QuerySelHCBP["HStatus"].ToString();//訂單狀態
                    gHItemStatus = QuerySelHCBP["HItemStatus"].ToString();//付款狀態
                }
                QuerySelHCBP.Close();

                //申請退款代碼第一個字母→ HPMethod：1=基金會(F)、2=文化事業(C)
                string gHCXLOrderGroup = LB_HPMethod.Text == "基金會" ? "F" + DateTime.Now.ToString("yyMMddHHmmssffff") : LB_HPMethod.Text == "文化事業" ? "C" + DateTime.Now.ToString("yyMMddHHmmssffff") : "";

                string gHID = "";

                if (LB_HCPkgYN.Text != "1")
                {
                    for (int i = 0; i < Rpt_RHCBookingList.Items.Count; i++)
                    {
                        if (((CheckBox)Rpt_RHCBookingList.Items[i].FindControl("CB_CancelSelect")).Checked)
                        {
                            int gHCXLSubTotalSum = 0;
                            gHCXLSubTotalSum += Convert.ToInt32(((TextBox)Rpt_RHCBookingList.Items[i].FindControl("TB_HCXLRefund")).Text.Trim());

                            int gHCXLHandlingFee = gHCXLSubTotalSum - Convert.ToInt32(LB_HECPAmount.Text.Replace(",", ""));

                            SqlCommand QueryUpdHCB = new SqlCommand("UPDATE HCourseBooking SET HCXLOrderGroup=@HCXLOrderGroup, HCXLApplyDate=@HCXLApplyDate, HCXLFinishDate=@HCXLFinishDate, HCXLAmount=@HCXLAmount, HCXLSubTotalSum=@HCXLSubTotalSum, HCXLHandlingFee=@HCXLHandlingFee, HCXLTotal=@HCXLTotal, HCXLHandleStatus=@HCXLHandleStatus, HStatus=@HStatus, HItemStatus=@HItemStatus WHERE HID=@HID", SQLdatabase.OpenConnection());

                            QueryUpdHCB.Parameters.AddWithValue("@HID", ((Label)Rpt_RHCBookingList.Items[i].FindControl("LB_HID")).Text);
                            //HItemStatus：1=已付款、2=未付款、3=已退款
                            QueryUpdHCB.Parameters.AddWithValue("@HCXLOrderGroup", gHCXLOrderGroup);
                            QueryUpdHCB.Parameters.AddWithValue("@HCXLApplyDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                            QueryUpdHCB.Parameters.AddWithValue("@HCXLFinishDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                            QueryUpdHCB.Parameters.AddWithValue("@HCXLAmount", ((TextBox)Rpt_RHCBookingList.Items[i].FindControl("TB_HCXLRefund")).Text.Trim());
                            QueryUpdHCB.Parameters.AddWithValue("@HCXLSubTotalSum", gHCXLSubTotalSum.ToString());
                            QueryUpdHCB.Parameters.AddWithValue("@HCXLHandlingFee", gHCXLHandlingFee.ToString());
                            QueryUpdHCB.Parameters.AddWithValue("@HCXLTotal", gHCXLSubTotalSum.ToString());
                            //HCXLHandleStatus：1=待處理、2=處理中、3=已處理、4=取消
                            QueryUpdHCB.Parameters.AddWithValue("@HCXLHandleStatus", 3);
                            QueryUpdHCB.Parameters.AddWithValue("@HStatus", gHStatus);
                            QueryUpdHCB.Parameters.AddWithValue("@HItemStatus", 3);

                            QueryUpdHCB.ExecuteNonQuery();
                            SQLdatabase.OpenConnection().Close();

                            gHID = gHID + ((Label)Rpt_RHCBookingList.Items[i].FindControl("LB_HID")).Text + ",";

                        }
                    }

                    SDS_BookingList.SelectCommand = "SELECT ROW_NUMBER() OVER (ORDER BY a.HOrderNum ASC) AS ROW, a.HID, a.HOrderNum, a.HAttend, a.HPMethod, a.HCourseDonate, a.HPoint, a.HSubTotal, a.HDCode, a.HDPoint,a.HCourseDonate, a.HCourseName AS DCName, a.HDateRange AS DCDateRange, a.HStatus, b.HCourseNum, b.HCourseName, b.HDateRange, b.Location, b.HBCPoint, a.HItemStatus, '' AS HBookedDate FROM HCourseBooking AS a LEFT JOIN CourseList AS b ON a.HCourseID = b.HID WHERE a.HOrderGroup = '" + LB_HOrderGroup.Text + "'";
                    //Rpt_BookingList.DataBind();
                }
                else  //如果是套裝課程
                {
                    for (int i = 0; i < Rpt_RHCPackageList.Items.Count; i++)
                    {

                        if (((CheckBox)Rpt_RHCPackageList.Items[i].FindControl("CB_CancelSelect")).Checked)
                        {
                            //WE230823_加入分行名稱欄位
                            SqlCommand QueryUpdHCB = new SqlCommand("UPDATE HCourseBooking SET  HCXLOrderGroup=@HCXLOrderGroup, HCXLApplyDate=@HCXLApplyDate, HCXLFinishDate=@HCXLFinishDate, HCXLAmount=@HCXLAmount,  HCXLHandleStatus=@HCXLHandleStatus,HStatus=@HStatus  WHERE HOrderGroup=@HOrderGroup", SQLdatabase.OpenConnection());
                            //HCXLSubTotalSum = @HCXLSubTotalSum, HCXLHandlingFee = @HCXLHandlingFee, HCXLTotal = @HCXLTotal, 
                            QueryUpdHCB.Parameters.AddWithValue("@HOrderGroup", ((Label)Rpt_RHCPackageList.Items[i].FindControl("LB_HOrderGroup")).Text);
                            //HItemStatus：1=已付款、2=未付款、3=已退款
                            QueryUpdHCB.Parameters.AddWithValue("@HItemStatus", "3");
                            QueryUpdHCB.Parameters.AddWithValue("@HCXLOrderGroup", gHCXLOrderGroup);
                            QueryUpdHCB.Parameters.AddWithValue("@HCXLApplyDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                            QueryUpdHCB.Parameters.AddWithValue("@HCXLFinishDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                            QueryUpdHCB.Parameters.AddWithValue("@HCXLAmount", ((Label)Rpt_RHCPackageList.Items[i].FindControl("LB_HCXLAmount")).Text);
                            //HCXLHandleStatus：1=待處理、2=處理中、3=已處理、4=取消
                            QueryUpdHCB.Parameters.AddWithValue("@HCXLHandleStatus", "3");
                            //HStatus：1=訂單成立、2=訂單取消
                            QueryUpdHCB.Parameters.AddWithValue("@HStatus", gHStatus);

                            QueryUpdHCB.ExecuteNonQuery();
                            QueryUpdHCB.Cancel();
                            SQLdatabase.OpenConnection().Close();


                        }

                    }
                }


                Response.Write("<script>alert('您已完成取消訂單~!');window.location.href='HOrder_Edit.aspx'</script>");

            }
        }



    }
    #endregion


}