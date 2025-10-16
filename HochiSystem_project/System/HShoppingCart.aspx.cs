using DocumentFormat.OpenXml.Office.Word;
using ECPay.Payment.Integration;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class HShoppingCart : System.Web.UI.Page
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

    #region 模型
    //護持項目
    public class Team
    {
        public class list_item
        {
            public string HGroupName { get; set; }
            public string HGroupID { get; set; }
            public string HTaskID { get; set; }
            public string HTask { get; set; }
            public string HGDay { get; set; }
            public bool HSameAsCourse { get; set; }
            public string HGSTime { get; set; }
            public string HGETime { get; set; }
            public string HID { get; set; }
            public string Add { get; set; }
            public string Update { get; set; }
        }
        public List<list_item> NameList { get; set; }
    }

    //幫他人報名
    public class Other
    {
        public class list_item
        {
            public string HOtherMember { get; set; }
            public string HID { get; set; }
            public string Add { get; set; }
            public string Update { get; set; }
        }
        public List<list_item> NameList { get; set; }
    }

    //活動-親朋好友報名
    public class Activity
    {
        public class list_item
        {
            public string HAName { get; set; }
            public string HASex { get; set; }
            public string HASexID { get; set; }
            public string HAAge { get; set; }
            public string HARelation { get; set; }
            public string HARelationID { get; set; }
            public string HID { get; set; }
            public string Add { get; set; }
            public string Update { get; set; }
        }
        public List<list_item> NameList { get; set; }
    }
    #endregion

    int Subtotal, ExemptionTotal, HDisPoint;

    decimal CulturalTotal = 0; //文化課程總金額
    decimal FoundationTotal = 0;  //傳光(基金會)課程總金額
    decimal DonateTotal = 0;   //Donate總金額
    decimal Remain = 0;   //剩餘點數轉換金額
    int CNum = 0;
    int FNum = 0;  //傳光(基金會)數量
    int DNum = 0;  //Donate數量
    int gTotalC = 0;//文化(玉成(二~七階))總計
    int gTotalFC = 0;//傳光(基金會)(傳光(尋光階~一階)總計
    int gTotalFD = 0;//傳光(基金會)(損款)總計

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        #region  判斷目前有幾筆有做勾選
        for (int i = 0; i < Rpt_Cultural.Items.Count; i++)
        {
            if (((CheckBox)Rpt_Cultural.Items[i].FindControl("CB_SelectC")).Checked == true)
            {
                CNum += 1;
                if (!string.IsNullOrEmpty(((Label)Rpt_Cultural.Items[i].FindControl("LB_HSubTotalC")).Text))
                {
                    CulturalTotal += Convert.ToDecimal(((Label)Rpt_Cultural.Items[i].FindControl("LB_HSubTotalC")).Text);
                }

            }
        }


        ////GA20240129_新增套裝課程報名金額加總
        //for (int i = 0; i < Rpt_HCoursePackageC.Items.Count; i++)
        //{
        //    if (((CheckBox)Rpt_HCoursePackageC.Items[i].FindControl("CB_SelectPkgC")).Checked == true)
        //    {
        //        CNum += 1;
        //        if (!string.IsNullOrEmpty(((Label)Rpt_HCoursePackageC.Items[i].FindControl("LB_HSubTotalPkgC")).Text))
        //        {
        //            CulturalTotal += Convert.ToDecimal(((Label)Rpt_HCoursePackageC.Items[i].FindControl("LB_HSubTotalPkgC")).Text);
        //        }

        //    }
        //}


        ////GA20240129_新增套裝課程報名金額加總
        //for (int i = 0; i < Rpt_HCoursePackageFC.Items.Count; i++)
        //{
        //    if (((CheckBox)Rpt_HCoursePackageFC.Items[i].FindControl("CB_SelectPkgFC")).Checked == true)
        //    {
        //        FNum += 1;
        //        if (!string.IsNullOrEmpty(((Label)Rpt_HCoursePackageFC.Items[i].FindControl("LB_HSubTotalPkgFC")).Text))
        //        {
        //            FoundationTotal += Convert.ToDecimal(((Label)Rpt_HCoursePackageFC.Items[i].FindControl("LB_HSubTotalPkgFC")).Text);
        //        }

        //    }
        //}


        for (int i = 0; i < Rpt_FoundationC.Items.Count; i++)
        {
            if (((CheckBox)Rpt_FoundationC.Items[i].FindControl("CB_SelectFC")).Checked == true)
            {
                FNum += 1;
                if (!string.IsNullOrEmpty(((Label)Rpt_FoundationC.Items[i].FindControl("LB_HSubTotalFC")).Text))
                {
                    FoundationTotal += Convert.ToDecimal(((Label)Rpt_FoundationC.Items[i].FindControl("LB_HSubTotalFC")).Text);
                }
            }
        }

        for (int i = 0; i < Rpt_FoundationD.Items.Count; i++)
        {
            if (((CheckBox)Rpt_FoundationD.Items[i].FindControl("CB_SelectFD")).Checked == true)
            {
                DNum += 1;
                if (!string.IsNullOrEmpty(((TextBox)Rpt_FoundationD.Items[i].FindControl("TB_PAmountFD")).Text.Trim()))
                {
                    DonateTotal += Convert.ToDecimal(((TextBox)Rpt_FoundationD.Items[i].FindControl("TB_PAmountFD")).Text.Trim());
                }
            }
        }


        if (FNum == 0 && DNum == 0 && CNum != 0)
        {
            LB_Navtab.Text = "2"; //文化事業
        }
        else if ((FNum != 0 || DNum != 0) && CNum != 0)
        {
            LB_Navtab.Text = "3"; //綜合
        }
        else if ((FNum != 0 || DNum != 0) && CNum == 0)
        {
            LB_Navtab.Text = "1"; //基金會
        }

        if (Rpt_Cultural.Items.Count == 0)
        {
            Div_Cultural.Visible = false;
            LB_NoCourseC.Visible = true;
        }
        else
        {
            Div_Cultural.Visible = true;
            LB_NoCourseC.Visible = false;
        }

        if (Rpt_FoundationC.Items.Count == 0)
        {
            Div_Foundation.Visible = false;
            LB_NoCourseF.Visible = true;
        }
        else
        {
            Div_Foundation.Visible = true;
            LB_NoCourseF.Visible = false;
        }



        if (Rpt_FoundationD.Items.Count == 0)
        {
            Div_Donation.Visible = false;
            LB_NoCourseD.Visible = true;
        }
        else
        {
            Div_Donation.Visible = true;
            LB_NoCourseD.Visible = false;
        }


        #endregion

        ////計算總金額
        Count();


    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            #region 清除Cache
            //# 設定此頁面不留Cache，讓上一頁無法回來。
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoServerCaching();
            HttpContext.Current.Response.Cache.SetNoStore();
            //HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            //HttpContext.Current.Response.AddHeader("Pragma", "no-cache");
            //HttpContext.Current.Response.AddHeader("Expires", "0");
            #endregion
        }


        if (Session["HAccount"] == null || Session["HUserName"] == null || Session["HUserHID"] == null || Session["EIPUID"] == null)
        {
            if (Request.Cookies["HochiInfo"] == null)
            {
                Response.Write("<script>alert('畫面停留太久，系統已自動登出囉~請重新登入，謝謝~!');window.location.href='Hlogin.aspx';</script>");
            }
            else
            {
                Session["HAccount"] = Request.Cookies["HochiInfo"]["HAccount"].ToString();
                Session["HUserName"] = Request.Cookies["HochiInfo"]["HUserName"].ToString();
                Session["HUserHID"] = Request.Cookies["HochiInfo"]["HUserHID"].ToString();
                Session["EIPUID"] = Request.Cookies["HochiInfo"]["EIPUID"].ToString();
            }
        }


        if (!IsPostBack)
        {
            ViewState["Key"] = "";
            //護持項目
            ViewState["Item_temp"] = "[]";
            ViewState["Item_Del"] = "";
            ViewState["PMethod"] = "";
            //幫他人報名
            ViewState["Other_temp"] = "[]";
            ViewState["Other_Del"] = "";
            //活動-親朋好友報名
            ViewState["Activity_temp"] = "[]";
            ViewState["Activity_Del"] = "";

            DIV_Cart.Attributes.Add("class", "process-item active");

            DIV_RemainPoint.Visible = false;
            DIV_Difference.Visible = false;


            //221003-HBCPoint要轉成金額(*10)
            SDS_Cultural.SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCTemplateID, a.HCourseName, a.HCourseID, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, (b.HBCPoint*10) AS CPrice, MAX(a.HPoint*10) AS SCPrice, a.HLDiscount*10 AS LDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange LEFT JOIN HExamContent AS c ON b.HExamContentID=c.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseDonate='0' AND b.HPMethod='2' AND b.HVerifyStatus='2' AND b.HStatus='1'  GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, a.HLDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod";
            Rpt_Cultural.DataBind();

            SDS_FoundationC.SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HCourseID, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, (b.HBCPoint*10) AS FCPrice, MAX(a.HPoint) AS SFCPrice, a.HLDiscount*10 AS LDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange LEFT JOIN HExamContent AS c ON b.HExamContentID=c.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseDonate='0' AND b.HPMethod='1' AND b.HVerifyStatus='2' AND b.HStatus='1'  GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, a.HLDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod";
            Rpt_FoundationC.DataBind();


            SDS_FoundationD.SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HCourseID, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, (b.HBCPoint*10) AS FDPrice FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseDonate='1' AND b.HVerifyStatus='2' AND b.HStatus='1'  GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint";
            Rpt_FoundationD.DataBind();


            if (Rpt_Cultural.Items.Count == 0 && Rpt_FoundationC.Items.Count == 0 && Rpt_FoundationD.Items.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('您的報名/捐款清單是空的哦!');window.location.href='HCourseList.aspx';", true);
            }

        }


        //221015-文化的課程才會顯示剩餘點數(金額)
        #region 尚餘點數
        SqlDataReader MyQueryHP = SQLdatabase.ExecuteReader("SELECT SUM(ISNULL(a.HBuy, 0)) AS HBuy, SUM(ISNULL(a.HUse, 0)) AS HUse, b.HUserName, c.HArea, b.HPeriod FROM HPoints AS a JOIN HMember AS b ON a.HMemberID = b.HID JOIN HArea AS c ON b.HareaID = c.HID WHERE a.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HStatus=1 GROUP BY b.HUserName, c.HArea, b.HPeriod");

        if (MyQueryHP.Read())
        {

            LB_RemainPoints.Text = Convert.ToString(Convert.ToInt32(MyQueryHP["HBuy"].ToString()) - Convert.ToInt32(MyQueryHP["HUse"].ToString()));

            if (LB_RemainPoints.Text != "0")
            {
                LB_Remain.Text = ConvertFare.ToThousands((Convert.ToInt32(MyQueryHP["HBuy"].ToString()) - Convert.ToInt32(MyQueryHP["HUse"].ToString())) * 10);
                Remain = (Convert.ToInt32(MyQueryHP["HBuy"].ToString()) - Convert.ToInt32(MyQueryHP["HUse"].ToString())) * 10;
            }
            else
            {
                Remain = 0;
            }

        }
        MyQueryHP.Close();
        #endregion


        //HA20250516_檢覈課程判斷
        if (!IsPostBack)
        {

            //玉成-改寫在Rpt_Cultural_ItemDataBound
            for (int x = 0; x < Rpt_Cultural.Items.Count; x++)
            {
                //檢覈課程判斷
                SqlDataReader QueryHCourse = SQLdatabase.ExecuteReader("SELECT a.HExamContentID FROM HCourse AS a WHERE a.HStatus='1' AND a.HVerifyStatus='2'  AND a.HID = '" + ((Label)Rpt_Cultural.Items[x].FindControl("LB_CourseIDC")).Text + "' AND a.HCourseName = N'" + ((Label)Rpt_Cultural.Items[x].FindControl("LB_CourseNameC")).Text + "'");

                while (QueryHCourse.Read())
                {
                    if (!string.IsNullOrEmpty(QueryHCourse["HExamContentID"].ToString()) && QueryHCourse["HExamContentID"].ToString() != "0")
                    {
                        ((Label)Rpt_Cultural.Items[x].FindControl("LBtn_HCourseNameC")).ForeColor = Color.Blue;
                        //((Label)Rpt_Cultural.Items[x].FindControl("LB_HOCPlaceC")).Text = "LB_HOCPlaceC";
                        //Label開啟modal
                        ((Label)Rpt_Cultural.Items[x].FindControl("LBtn_HCourseNameC")).CssClass = "title text-info";
                        ((Label)Rpt_Cultural.Items[x].FindControl("LBtn_HCourseNameC")).Style["cursor"] = "pointer";
                        ((Label)Rpt_Cultural.Items[x].FindControl("LBtn_HCourseNameC")).Attributes["data-toggle"] = "modal";
                        ((Label)Rpt_Cultural.Items[x].FindControl("LBtn_HCourseNameC")).Attributes["data-target"] = "#subjectModal";
                        //顯示檢覈費用，不顯示基本費用與課程費用
                        ((Label)Rpt_Cultural.Items[x].FindControl("LB_HBCPointC")).Visible = false;
                        ((TextBox)Rpt_Cultural.Items[x].FindControl("TB_HPAmountC")).Visible = true;
                        ((TextBox)Rpt_Cultural.Items[x].FindControl("TB_HPAmountC")).Text = "0";
                    }
                }
                QueryHCourse.Close();
            }

            //傳光-改寫在Rpt_FoundationC_ItemDataBound
            var data = SDS_FoundationC.Select(DataSourceSelectArguments.Empty);
            //轉成 List<DataRowView> 才能與 Repeater.Items 對應
            var list = ((System.Data.DataView)data).Cast<System.Data.DataRowView>().ToList();
            for (int y = 0; y < Rpt_FoundationC.Items.Count; y++)
            {
                SqlDataReader QueryHCourse = SQLdatabase.ExecuteReader("SELECT a.HExamContentID FROM HCourse AS a WHERE a.HStatus='1' AND a.HVerifyStatus='2'  AND a.HID = '" + ((Label)Rpt_FoundationC.Items[y].FindControl("LB_CourseIDFC")).Text + "' AND a.HCourseName = N'" + ((Label)Rpt_FoundationC.Items[y].FindControl("LB_CourseNameFC")).Text + "'");
                //Response.Write("<br/><br/><br/><br/><br/>SELECT a.HExamContentID FROM HCourse AS a WHERE a.HStatus='1' AND a.HVerifyStatus='2'  AND a.HID = '" + ((Label)Rpt_FoundationC.Items[y].FindControl("LB_CourseIDFC")).Text + "' AND a.HCourseName = N'" + ((Label)Rpt_FoundationC.Items[y].FindControl("LB_CourseNameFC")).Text + "'<br/>");
                while (QueryHCourse.Read())
                {
                    if (!string.IsNullOrEmpty(QueryHCourse["HExamContentID"].ToString()) && QueryHCourse["HExamContentID"].ToString() != "0")
                    {
                        ((Label)Rpt_FoundationC.Items[y].FindControl("LBtn_HCourseNameFC")).ForeColor = Color.Blue;
                        //((Label)Rpt_FoundationC.Items[y].FindControl("LB_HOCPlaceFC")).Text = "LB_HOCPlaceFC";
                        //Label開啟modal
                        ((Label)Rpt_FoundationC.Items[y].FindControl("LBtn_HCourseNameFC")).CssClass = "title text-info";
                        ((Label)Rpt_FoundationC.Items[y].FindControl("LBtn_HCourseNameFC")).Style["cursor"] = "pointer";
                        ((Label)Rpt_FoundationC.Items[y].FindControl("LBtn_HCourseNameFC")).Attributes["data-toggle"] = "modal";
                        ((Label)Rpt_FoundationC.Items[y].FindControl("LBtn_HCourseNameFC")).Attributes["data-target"] = "#subjectModal";
                        //課程費用改為檢覈費用，不顯示基本費用
                        ((TextBox)Rpt_FoundationC.Items[y].FindControl("TB_HPAmountFC")).Text = list[y]["HExamFee"].ToString();
                        ((Label)Rpt_FoundationC.Items[y].FindControl("LB_HSubTotalFC")).Text = list[y]["HExamFee"].ToString();
                        ((Label)Rpt_FoundationC.Items[y].FindControl("LB_HBCPointFC")).Visible = false;
                    }
                    else
                    {

                    }
                }
                QueryHCourse.Close();
            }



        }

    }

    #region 玉成(文化事業)課程
    protected void LBtn_Cultural_Click(object sender, EventArgs e)
    {
        #region  判斷基傳光課程/捐款是否有作勾選
        for (int i = 0; i < Rpt_FoundationC.Items.Count; i++)
        {
            if (((CheckBox)Rpt_FoundationC.Items[i].FindControl("CB_SelectFC")).Checked == true)
            {
                //Response.Write("<script>alert('傳光課程與玉成課程不能同時結帳哦~ 如要先結玉成課程，請先取消傳光課程的勾選~謝謝~!');</script>");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice4", "alert('傳光課程與玉成課程不能同時結帳哦~ 如要先結玉成課程，請先取消傳光課程的勾選~謝謝~!')", true);
                return;
            }
        }

        for (int i = 0; i < Rpt_FoundationD.Items.Count; i++)
        {
            if (((CheckBox)Rpt_FoundationD.Items[i].FindControl("CB_SelectFD")).Checked == true)
            {
                //Response.Write("<script>alert('傳光課程與玉成課程不能同時結帳哦~如要先結玉成課程，請先取消捐款的勾選~謝謝~!');</script>");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice4", "alert('傳光課程與玉成課程不能同時結帳哦~ 如要先結玉成課程，請先取消傳光課程的勾選~謝謝~!')", true);
                return;
            }
        }
        #endregion


        //221015-文化的課程才會顯示剩餘點數(金額)
        #region 文化的課程才會顯示剩餘點數(金額)→不確定這個region是不是指這個
        if (Remain != 0)
        {
            DIV_RemainPoint.Visible = true;
            DIV_Difference.Visible = true;
        }
        else
        {
            DIV_RemainPoint.Visible = false;
            DIV_Difference.Visible = false;
        }
        #endregion



        //221003-HBCPoint要轉成金額(*10)
        SDS_Cultural.SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCTemplateID, a.HCourseName, a.HCourseID, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, (b.HBCPoint*10) AS CPrice, MAX(a.HPoint*10) AS SCPrice, a.HLDiscount*10 AS LDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod  FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange LEFT JOIN HExamContent AS c ON b.HExamContentID=c.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseDonate='0' AND b.HPMethod='2' AND b.HVerifyStatus='2' AND b.HStatus='1' GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, a.HLDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod";

    }
    #endregion

    #region 傳光(基金會)課程
    protected void LBtn_Foundation_Click(object sender, EventArgs e)
    {
        #region  判斷玉成的課程是否有作勾選
        for (int i = 0; i < Rpt_Cultural.Items.Count; i++)
        {
            if (((CheckBox)Rpt_Cultural.Items[i].FindControl("CB_SelectC")).Checked == true)
            {
                // Response.Write("<script>alert('傳光課程與玉成課程不能同時結帳哦~ 如要先結傳光課程，請先取消玉成課程的勾選~謝謝~!');</script>");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice4", "alert('傳光課程與玉成課程不能同時結帳哦~ 如要先結玉成課程，請先取消傳光課程的勾選~謝謝~!')", true);
                return;
            }
        }
        #endregion


        //221015-隱藏剩餘點數(金額)的區塊
        DIV_RemainPoint.Visible = false;
        DIV_Difference.Visible = false;
        LB_RemainPoints.Text = "0";
        LB_Remain.Text = "0";
        Remain = 0;


        //221003-HBCPoint要轉成金額(*10)
        SDS_FoundationC.SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HCourseID, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, (b.HBCPoint*10) AS FCPrice, MAX(a.HPoint) AS SFCPrice, a.HLDiscount*10 AS LDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange LEFT JOIN HExamContent AS c ON b.HExamContentID=c.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseDonate='0' AND b.HPMethod='1' AND b.HVerifyStatus='2' AND b.HStatus='1' GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, a.HLDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod";
        SDS_FoundationD.SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HCourseID, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, (b.HBCPoint*10) AS FDPrice FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseDonate='1' AND b.HVerifyStatus='2' AND b.HStatus='1'  GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint";

        //計算總金額
        //Count();
    }
    #endregion

    #region 玉成已報名列表顯示
    protected void Rpt_Cultural_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //取得Repeater的DataRowView
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        //DDL_HOCPlaceC

        //上課地點
        SqlDataReader QueryHCourse_Place = SQLdatabase.ExecuteReader("SELECT a.HID, a.HOCPlace, b.HPlaceName FROM HCourse AS a LEFT JOIN HPlace AS b ON a.HOCPlace = b.HID WHERE a.HStatus='1' AND a.HVerifyStatus='2'  AND a.HCTemplateID = '" + gDRV["HCTemplateID"].ToString() + "' AND a.HCourseName = N'" + gDRV["HCourseName"].ToString() + "' AND a.HDateRange = '" + gDRV["HDateRange"].ToString() + "' GROUP BY a.HOCPlace, b.HPlaceName, a.HID ORDER BY convert(int, a.HOCPlace) ASC ");
        //  int placecount = 0;

        while (QueryHCourse_Place.Read())
        {
            ((DropDownList)e.Item.FindControl("DDL_HOCPlaceC")).Items.Add(new ListItem(QueryHCourse_Place["HPlaceName"].ToString(), QueryHCourse_Place["HOCPlace"].ToString()));
            //placecount++;
        }
        QueryHCourse_Place.Close();


        //GA20231219_加入參班身分帶資料庫資料
        if (gDRV["HAttend"].ToString() != null || gDRV["HAttend"].ToString() != "")
        {
            ((DropDownList)e.Item.FindControl("DDL_HAttendC")).SelectedValue = gDRV["HAttend"].ToString();
        }

        //帶出已選上課地點
        SqlDataReader QuerySelSC = SQLdatabase.ExecuteReader("SELECT HCourseID, HAttend, HPAmount, HDCode, HDPoint, HSubTotal, HLDiscount FROM HShoppingCart WHERE HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND HCTemplateID = '" + gDRV["HCTemplateID"].ToString() + "' AND HCourseName = '" + gDRV["HCourseName"].ToString() + "' AND HDateRange = '" + gDRV["HDateRange"].ToString() + "' AND HCourseDonate='0' AND HPMethod='2'");
        if (QuerySelSC.Read())
        {
            if (QuerySelSC["HCourseID"].ToString() != "0")
            {
                SqlDataReader QuerySelHC = SQLdatabase.ExecuteReader("SELECT HOCPlace FROM HCourse WHERE HID = '" + QuerySelSC["HCourseID"].ToString() + "'");
                if (QuerySelHC.Read())
                {
                    ((DropDownList)e.Item.FindControl("DDL_HOCPlaceC")).SelectedValue = QuerySelHC["HOCPlace"].ToString();
                }
                QuerySelHC.Close();


                ((DropDownList)e.Item.FindControl("DDL_HAttendC")).SelectedValue = QuerySelSC["HAttend"].ToString();


                //GE20231218_修改:加入QuerySelSC["HPAmount"].ToString() != ""得條件
                if (QuerySelSC["HPAmount"].ToString() != "" && QuerySelSC["HPAmount"].ToString() != null)
                {
                    ((TextBox)e.Item.FindControl("TB_HPAmountC")).Text = QuerySelSC["HPAmount"].ToString();
                }
                //HE20231217_空白也需要帶出
                //if (QuerySelSC["HPAmount"].ToString() != null)
                //{
                //	((TextBox)e.Item.FindControl("TB_HPAmountC")).Text = QuerySelSC["HPAmount"].ToString();
                //}



                //HA20250520_檢覈課程金額變更
                if (!string.IsNullOrEmpty(gDRV["HExamContentID"].ToString()) && gDRV["HExamContentID"].ToString() != "0")
                {
                    ((LinkButton)e.Item.FindControl("LBtn_HCourseNameC")).Text = gDRV["HCourseName"].ToString();
                    ((LinkButton)e.Item.FindControl("LBtn_HCourseNameC")).ForeColor = Color.Blue;
                    ((LinkButton)e.Item.FindControl("LBtn_HCourseNameC")).Enabled = true;
                    ((TextBox)e.Item.FindControl("TB_HPAmountC")).Text = "0";//課程費用改為檢覈費用(未確認科目前暫為0)



                    //((Label)e.Item.FindControl("LBtn_HCourseNameC")).Text = gDRV["HExamFee"].ToString();
                    //1:課程金額=檢覈金額(不管科目數量)、2:課程金額=檢覈金額*科目數量、3:課程金額依科目數量定義(HSubjectMinNum與c.SubjectMaxNum)檢覈金額。
                    if (gDRV["HChargeMethod"].ToString() == "1")
                    {
                        SqlDataReader QueryHEF = SQLdatabase.ExecuteReader("SELECT a.HExamFee FROM HExamFee AS a WHERE a.HExamContentID='" + gDRV["HExamContentID"].ToString() + "'");
                        while (QueryHEF.Read())
                        {
                            ((TextBox)e.Item.FindControl("TB_HPAmountC")).Text = QueryHEF["HExamFee"].ToString();
                        }
                    }
                    else if (gDRV["HChargeMethod"].ToString() == "2")
                    {
                        SqlDataReader QueryHEFEL = SQLdatabase.ExecuteReader("SELECT a.HExamFee, count(b.HID) AS ExamListCT FROM  HExamFee AS a JOIN HShoppingCart_Exam AS b ON a.HExamContentID=b.HExamContentID WHERE a.HExamContentID='" + gDRV["HExamContentID"].ToString() + "' AND b.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' GROUP BY a.HExamFee");
                        while (QueryHEFEL.Read())
                        {
                            ((TextBox)e.Item.FindControl("TB_HPAmountC")).Text = (Convert.ToInt32(QueryHEFEL["HExamFee"].ToString()) * Convert.ToInt32(QueryHEFEL["ExamListCT"].ToString())).ToString();
                        }
                    }
                    else if (gDRV["HChargeMethod"].ToString() == "3")
                    {
                        SqlDataReader QueryHEFEL = SQLdatabase.ExecuteReader("SELECT a.HSubjectMinNum, a.HSubjectMaxNum, a.HExamFee, count(b.HID) AS ExamListCT FROM HExamFee AS a JOIN HShoppingCart_Exam AS b ON a.HExamContentID=b.HExamContentID WHERE a.HExamContentID='" + gDRV["HExamContentID"].ToString() + "' AND b.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' GROUP BY a.HSubjectMinNum, a.HSubjectMaxNum, a.HExamFee");
                        while (QueryHEFEL.Read())
                        {
                            if (Convert.ToInt32(QueryHEFEL["ExamListCT"].ToString()) >= Convert.ToInt32(QueryHEFEL["HSubjectMinNum"].ToString()) && (Convert.ToInt32(QueryHEFEL["ExamListCT"].ToString()) <= Convert.ToInt32(QueryHEFEL["HSubjectMaxNum"].ToString())))
                            {
                                //((LinkButton)e.Item.FindControl("LBtn_HCourseNameC")).Text = QueryHEFEL["HSubjectMinNum"].ToString() + "/" + QueryHEFEL["HSubjectMaxNum"].ToString() + "/" + QueryHEFEL["HExamFee"].ToString()+"/"+ QueryHEFEL["ExamListCT"].ToString();
                                ((TextBox)e.Item.FindControl("TB_HPAmountC")).Text = QueryHEFEL["HExamFee"].ToString();
                            }
                        }
                    }



                    if (((TextBox)e.Item.FindControl("TB_HPAmountC")).Text == "0")
                    {
                        ((TextBox)e.Item.FindControl("TB_HPAmountC")).Visible = false;//課程費用未確認時暫不顯示
                    }

                    ((Label)e.Item.FindControl("LB_HBCPointC")).Visible = false;//不顯示基本費用
                    ((DropDownList)e.Item.FindControl("DDL_HOCPlaceC")).Visible = false; //不顯示上課地點
                }
                else
                {
                }




                ((TextBox)e.Item.FindControl("TB_HDCodeC")).Text = QuerySelSC["HDCode"].ToString();
                ((Label)e.Item.FindControl("LB_HDPointC")).Text = !string.IsNullOrEmpty(QuerySelSC["HDPoint"].ToString()) ? (Convert.ToInt32(QuerySelSC["HDPoint"].ToString()) * 10).ToString() : QuerySelSC["HDPoint"].ToString();

                //GE20231219_折扣金額計算加入前導課程折扣金額
                if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HLDiscountC")).Text) && !string.IsNullOrEmpty(QuerySelSC["HDPoint"].ToString()))
                {
                    ((Label)e.Item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(QuerySelSC["HDPoint"].ToString()) * 10 + Convert.ToInt32(((Label)e.Item.FindControl("LB_HLDiscountC")).Text)).ToString();
                }
                else if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HLDiscountC")).Text) && string.IsNullOrEmpty(QuerySelSC["HDPoint"].ToString()))
                {
                    ((Label)e.Item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HLDiscountC")).Text)).ToString();
                }
                else if (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HLDiscountC")).Text) && !string.IsNullOrEmpty(QuerySelSC["HDPoint"].ToString()))
                {
                    ((Label)e.Item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(QuerySelSC["HDPoint"].ToString()) * 10).ToString();
                }
                else
                {
                    ((Label)e.Item.FindControl("LB_HDiscountC")).Text = null;
                }


                //GA20231219_判斷若折扣金額不為空或0，小計會重新計算
                if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HDiscountC")).Text))
                {
                    ((Label)e.Item.FindControl("LB_HSubTotalC")).Text = (Convert.ToInt32(((TextBox)e.Item.FindControl("TB_HPAmountC")).Text.Trim()) - Convert.ToInt32(((Label)e.Item.FindControl("LB_HDiscountC")).Text)).ToString();

                    //GA20240307_加入負值判斷
                    if (Convert.ToInt32(((Label)e.Item.FindControl("LB_HSubTotalC")).Text) < 0)
                    {
                        ((Label)e.Item.FindControl("LB_HSubTotalC")).Text = "0";
                    }
                }
                else
                {
                    ((Label)e.Item.FindControl("LB_HSubTotalC")).Text = ((TextBox)e.Item.FindControl("TB_HPAmountC")).Text.Trim();
                }



                //HA20231217_當基本費用是0元時，付款金額不預設帶值，並顯示請自行輸入(純護持除外)-S
                if ((((TextBox)e.Item.FindControl("TB_HPAmountC")).Text.Trim() == "0" || ((TextBox)e.Item.FindControl("TB_HPAmountC")).Text.Trim() == "") && (((Label)e.Item.FindControl("LB_HSubTotalC")).Text.Trim() == "0" || ((Label)e.Item.FindControl("LB_HSubTotalC")).Text.Trim() == "") && ((DropDownList)e.Item.FindControl("DDL_HAttendC")).SelectedValue != "5")
                {
                    ((TextBox)e.Item.FindControl("TB_HPAmountC")).Text = "";
                    ((TextBox)e.Item.FindControl("TB_HPAmountC")).Attributes.Add("PlaceHolder", "請自行輸入");
                    ((Label)e.Item.FindControl("LB_HSubTotalC")).Text = "";


                    //GA20231218_顯示提示文字
                    LB_HNotice.Visible = true;
                }
                //HA20231217_當基本費用是0元時，付款金額不預設帶值，並顯示請自行輸入(純護持除外)-E


            }
        }
        QuerySelSC.Close();

       

      

    }
    #endregion

    #region 玉成開啟檢覈科目選擇madel按鈕
    protected void LBtn_HCourseNameC_Click(object sender, EventArgs e)
    {
        var IBtn = sender as IButtonControl;
        RepeaterItem RILBtn = (sender as LinkButton).NamingContainer as RepeaterItem;

        ViewState["ClickShoppingCartID"] = ((Label)RILBtn.FindControl("LB_ShoppingCartIDC")).Text;
        //ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice4", "alert('gClickCourseID=" + gClickCourseID + "')", true);

        SDS_ExamSubject.SelectCommand = "SELECT a.HChargeMethod, b.HExamContentID, b.HExamSubjectID, c.HExamSubjectName FROM HExamContent AS a JOIN HExamContentDetail AS b ON a.HID=b.HExamContentID LEFT JOIN HExamSubject AS c ON b.HExamSubjectID=c.HID WHERE c.HStatus='1' AND a.HID = '" + ((Label)RILBtn.FindControl("LB_HExamContentIDC")).Text + "' GROUP BY a.HChargeMethod, b.HExamContentID, b.HExamSubjectID, c.HExamSubjectName";
        //更新包在UP_SubjectModal裡面的內容
        UP_SubjectModal.Update();
        //要註冊才能打開modal
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#subjectModal').modal('show');</script>", false);//執行js

    }
    #endregion

    #region 傳光(基金會)課程已報名列表顯示
    protected void Rpt_FoundationC_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //取得Repeater的DataRowView
        DataRowView gDRV = (DataRowView)e.Item.DataItem;


        //上課地點
        SqlDataReader QueryHCourse_Place = SQLdatabase.ExecuteReader("SELECT a.HID, a.HOCPlace, b.HPlaceName FROM HCourse AS a LEFT JOIN HPlace AS b ON a.HOCPlace = b.HID WHERE a.HStatus='1' AND a.HVerifyStatus='2' AND a.HCTemplateID = '" + gDRV["HCTemplateID"].ToString() + "' AND a.HCourseName = N'" + gDRV["HCourseName"].ToString() + "' AND a.HDateRange = '" + gDRV["HDateRange"].ToString() + "' GROUP BY a.HOCPlace, b.HPlaceName, a.HID ORDER BY convert(int, a.HOCPlace) ASC ");
        // int placecount = 0;

        while (QueryHCourse_Place.Read())
        {
            ((DropDownList)e.Item.FindControl("DDL_HOCPlaceFC")).Items.Add(new ListItem(QueryHCourse_Place["HPlaceName"].ToString(), QueryHCourse_Place["HOCPlace"].ToString()));
            // placecount++;
        }
        QueryHCourse_Place.Close();

        //GA20231219_加入參班身分帶資料庫資料
        if (gDRV["HAttend"].ToString() != null || gDRV["HAttend"].ToString() != "")
        {
            ((DropDownList)e.Item.FindControl("DDL_HAttendFC")).SelectedValue = gDRV["HAttend"].ToString();
        }

        //帶出已選上課地點
        SqlDataReader QuerySelSC = SQLdatabase.ExecuteReader("SELECT HCourseID, HAttend, HPAmount, HDCode, HDPoint, HSubTotal FROM HShoppingCart WHERE HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND HCTemplateID = '" + gDRV["HCTemplateID"].ToString() + "' AND HCourseName = N'" + gDRV["HCourseName"].ToString() + "' AND HDateRange = '" + gDRV["HDateRange"].ToString() + "' AND HCourseDonate='0' AND HPMethod='1'");
        if (QuerySelSC.Read())
        {
            if (QuerySelSC["HCourseID"].ToString() != "0")
            {
                SqlDataReader QuerySelHC = SQLdatabase.ExecuteReader("SELECT HOCPlace FROM HCourse WHERE HID = '" + QuerySelSC["HCourseID"].ToString() + "'");
                if (QuerySelHC.Read())
                {
                    ((DropDownList)e.Item.FindControl("DDL_HOCPlaceFC")).SelectedValue = QuerySelHC["HOCPlace"].ToString();
                }
                QuerySelHC.Close();


                ((DropDownList)e.Item.FindControl("DDL_HAttendFC")).SelectedValue = QuerySelSC["HAttend"].ToString();

                //GE20231218_修改:加入QuerySelSC["HPAmount"].ToString() != ""得條件
                if (QuerySelSC["HPAmount"].ToString() != "" && QuerySelSC["HPAmount"].ToString() != null)
                {
                    ((TextBox)e.Item.FindControl("TB_HPAmountFC")).Text = QuerySelSC["HPAmount"].ToString();
                }


                //HA20250520_檢覈課程金額變更
                if (!string.IsNullOrEmpty(gDRV["HExamContentID"].ToString()) && gDRV["HExamContentID"].ToString() != "0")
                {
                    ((LinkButton)e.Item.FindControl("LBtn_HCourseNameFC")).Text = gDRV["HCourseName"].ToString();
                    ((LinkButton)e.Item.FindControl("LBtn_HCourseNameFC")).ForeColor = Color.Blue;
                    ((LinkButton)e.Item.FindControl("LBtn_HCourseNameFC")).Enabled = true;
                    ((TextBox)e.Item.FindControl("TB_HPAmountFC")).Text = "0";//課程費用改為檢覈費用(未確認科目前暫為0)

                    //((Label)e.Item.FindControl("LBtn_HCourseNameFC")).Text = gDRV["HExamFee"].ToString();
                    //1:課程金額=檢覈金額(不管科目數量)、2:課程金額=檢覈金額*科目數量、3:課程金額依科目數量定義(HSubjectMinNum與c.SubjectMaxNum)檢覈金額。
                    if (gDRV["HChargeMethod"].ToString() == "1")
                    {
                        SqlDataReader QueryHEF = SQLdatabase.ExecuteReader("SELECT a.HExamFee FROM HExamFee AS a WHERE a.HExamContentID='" + gDRV["HExamContentID"].ToString() + "'");
                        while (QueryHEF.Read())
                        {
                            ((TextBox)e.Item.FindControl("TB_HPAmountFC")).Text = QueryHEF["HExamFee"].ToString();
                        }
                    }
                    else if (gDRV["HChargeMethod"].ToString() == "2")
                    {
                        SqlDataReader QueryHEFEL = SQLdatabase.ExecuteReader("SELECT a.HExamFee, count(b.HID) AS ExamListCT FROM  HExamFee AS a JOIN HShoppingCart_Exam AS b ON a.HExamContentID=b.HExamContentID WHERE a.HExamContentID='" + gDRV["HExamContentID"].ToString() + "' AND b.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' GROUP BY a.HExamFee");
                        while (QueryHEFEL.Read())
                        {
                            ((TextBox)e.Item.FindControl("TB_HPAmountFC")).Text = (Convert.ToInt32(QueryHEFEL["HExamFee"].ToString()) * Convert.ToInt32(QueryHEFEL["ExamListCT"].ToString())).ToString();
                        }
                    }
                    else if (gDRV["HChargeMethod"].ToString() == "3")
                    {
                        SqlDataReader QueryHEFEL = SQLdatabase.ExecuteReader("SELECT a.HSubjectMinNum, a.HSubjectMaxNum, a.HExamFee, count(b.HID) AS ExamListCT FROM HExamFee AS a JOIN HShoppingCart_Exam AS b ON a.HExamContentID=b.HExamContentID WHERE a.HExamContentID='" + gDRV["HExamContentID"].ToString() + "' AND b.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' GROUP BY a.HSubjectMinNum, a.HSubjectMaxNum, a.HExamFee");
                        while (QueryHEFEL.Read())
                        {
                            if (Convert.ToInt32(QueryHEFEL["ExamListCT"].ToString()) >= Convert.ToInt32(QueryHEFEL["HSubjectMinNum"].ToString()) && (Convert.ToInt32(QueryHEFEL["ExamListCT"].ToString()) <= Convert.ToInt32(QueryHEFEL["HSubjectMaxNum"].ToString())))
                            {
                                //((LinkButton)e.Item.FindControl("LBtn_HCourseNameFC")).Text = QueryHEFEL["HSubjectMinNum"].ToString() + "/" + QueryHEFEL["HSubjectMaxNum"].ToString() + "/" + QueryHEFEL["HExamFee"].ToString()+"/"+ QueryHEFEL["ExamListCT"].ToString();
                                ((TextBox)e.Item.FindControl("TB_HPAmountFC")).Text = QueryHEFEL["HExamFee"].ToString();
                            }
                        }
                    }





                    if (((TextBox)e.Item.FindControl("TB_HPAmountFC")).Text == "0")
                    {
                        ((TextBox)e.Item.FindControl("TB_HPAmountFC")).Visible = false;//課程費用未確認時暫不顯示
                    }

                    ((Label)e.Item.FindControl("LB_HBCPointFC")).Visible = false;//不顯示基本費用
                    ((DropDownList)e.Item.FindControl("DDL_HOCPlaceFC")).Visible = false; //不顯示上課地點
                }
                else
                {
                }


                ((TextBox)e.Item.FindControl("TB_HDCodeFC")).Text = QuerySelSC["HDCode"].ToString();
                ((Label)e.Item.FindControl("LB_HDPointFC")).Text = !string.IsNullOrEmpty(QuerySelSC["HDPoint"].ToString()) ? (Convert.ToInt32(QuerySelSC["HDPoint"].ToString()) * 10).ToString() : QuerySelSC["HDPoint"].ToString();

                //GE20231219_折扣金額計算加入前導課程折扣金額
                if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HLDiscountFC")).Text) && !string.IsNullOrEmpty(QuerySelSC["HDPoint"].ToString()))
                {
                    ((Label)e.Item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(QuerySelSC["HDPoint"].ToString()) * 10 + Convert.ToInt32(((Label)e.Item.FindControl("LB_HLDiscountFC")).Text)).ToString();
                }
                else if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HLDiscountFC")).Text) && string.IsNullOrEmpty(QuerySelSC["HDPoint"].ToString()))
                {
                    ((Label)e.Item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HLDiscountFC")).Text)).ToString();
                }
                else if (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HLDiscountFC")).Text) && !string.IsNullOrEmpty(QuerySelSC["HDPoint"].ToString()))
                {
                    ((Label)e.Item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(QuerySelSC["HDPoint"].ToString()) * 10).ToString();
                }
                else
                {
                    ((Label)e.Item.FindControl("LB_HDiscountFC")).Text = null;
                }

                //GA20231219_判斷若折扣金額不為空或0，小計會重新計算
                if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HDiscountFC")).Text))
                {
                    ((Label)e.Item.FindControl("LB_HSubTotalFC")).Text = (Convert.ToInt32(((TextBox)e.Item.FindControl("TB_HPAmountFC")).Text.Trim()) - Convert.ToInt32(((Label)e.Item.FindControl("LB_HDiscountFC")).Text)).ToString();

                    //GA20240307_加入負值判斷
                    if (Convert.ToInt32(((Label)e.Item.FindControl("LB_HSubTotalFC")).Text) < 0)
                    {
                        ((Label)e.Item.FindControl("LB_HSubTotalFC")).Text = "0";
                    }
                }
                else
                {
                    ((Label)e.Item.FindControl("LB_HSubTotalFC")).Text = ((TextBox)e.Item.FindControl("TB_HPAmountFC")).Text.Trim();
                }

                //HA20231217_當基本費用是0元時，付款金額不預設帶值，並顯示請自行輸入(純護持除外)-S
                if ((((TextBox)e.Item.FindControl("TB_HPAmountFC")).Text.Trim() == "0" || ((TextBox)e.Item.FindControl("TB_HPAmountFC")).Text.Trim() == "") && (((Label)e.Item.FindControl("LB_HSubTotalFC")).Text.Trim() == "0" || ((Label)e.Item.FindControl("LB_HSubTotalFC")).Text.Trim() == "") && ((DropDownList)e.Item.FindControl("DDL_HAttendFC")).SelectedValue != "5")
                {
                    ((TextBox)e.Item.FindControl("TB_HPAmountFC")).Text = "";
                    ((TextBox)e.Item.FindControl("TB_HPAmountFC")).Attributes.Add("PlaceHolder", "請自行輸入");
                    ((Label)e.Item.FindControl("LB_HSubTotalFC")).Text = "";
                    //GA20231218_顯示提示文字
                    LB_HNotice.Visible = true;
                }
                //HA20231217_當基本費用是0元時，付款金額不預設帶值，並顯示請自行輸入(純護持除外)-E
            }
        }
        QuerySelSC.Close();

      

        #region 計算勾選的課程總金額
        if (((CheckBox)e.Item.FindControl("CB_SelectFC")).Checked == true)
        {
            if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HSubTotalFC")).Text.Trim()))
            {
                gTotalFC += Convert.ToInt32(((Label)e.Item.FindControl("LB_HSubTotalFC")).Text.Trim());
            }

            LB_Total.Text = (gTotalFC + gTotalFD).ToString();
        }
        #endregion
    }
    #endregion

    #region 傳光(基金會)開啟檢覈科目選擇madel按鈕
    protected void LBtn_HCourseNameFC_Click(object sender, EventArgs e)
    {
        var IBtn = sender as IButtonControl;
        RepeaterItem RILBtn = (sender as LinkButton).NamingContainer as RepeaterItem;

        ViewState["ClickShoppingCartID"] = ((Label)RILBtn.FindControl("LB_ShoppingCartIDFC")).Text;
        //ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice4", "alert('gClickCourseID=" + gClickCourseID + "')", true);

        SDS_ExamSubject.SelectCommand = "SELECT a.HChargeMethod, b.HExamContentID, b.HExamSubjectID, c.HExamSubjectName FROM HExamContent AS a JOIN HExamContentDetail AS b ON a.HID=b.HExamContentID LEFT JOIN HExamSubject AS c ON b.HExamSubjectID=c.HID WHERE c.HStatus='1' AND a.HID = '" + ((Label)RILBtn.FindControl("LB_HExamContentIDFC")).Text + "' GROUP BY a.HChargeMethod, b.HExamContentID, b.HExamSubjectID, c.HExamSubjectName";
        //更新包在UP_SubjectModal裡面的內容
        UP_SubjectModal.Update();

        //要註冊才能打開modal
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#subjectModal').modal('show');</script>", false);//執行js

    }
    #endregion

    #region 檢覈課程科目列表
    protected void Rpt_ExamSubject_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //取得Repeater的DataRowView
        DataRowView gDRV = (DataRowView)e.Item.DataItem;


        SqlDataReader QueryHEL = SQLdatabase.ExecuteReader("SELECT HID FROM HShoppingCart_Exam WHERE HExamContentID='" + gDRV["HExamContentID"].ToString() + "' AND HExamSubjectID='"+ gDRV["HExamSubjectID"].ToString() + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        while (QueryHEL.Read())
        {
            if (!string.IsNullOrEmpty(QueryHEL["HID"].ToString()))
            {
                ((CheckBox)e.Item.FindControl("CB_SelectSubject")).Checked = true;
            }
        }


    }
    #endregion

    #region 檢覈課程科目勾選
    protected void CB_SelectSubject_CheckedChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();
        RepeaterItem RICB = (sender as CheckBox).NamingContainer as RepeaterItem;

        //ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice4", "alert('gClickCourseID=" + ViewState["ClickCourseID"].ToString() +"')", true);

        if (((CheckBox)RICB.FindControl("CB_SelectSubject")).Checked)
        {
            //新增
            SqlCommand cmdInsHEL = new SqlCommand("INSERT INTO HShoppingCart_Exam (HShoppingCartID, HExamContentID, HExamSubjectID, HMemberID, HStatus, HCreate, HCreateDT, HModify, HModifyDT) values (@HShoppingCartID, @HExamContentID, @HExamSubjectID, @HMemberID, @HStatus, @HCreate, @HCreateDT, @HModify, @HModifyDT)", con);
            con.Open();
            cmdInsHEL.Parameters.AddWithValue("@HShoppingCartID", ViewState["ClickShoppingCartID"].ToString());
            cmdInsHEL.Parameters.AddWithValue("@HExamContentID", ((Label)RICB.FindControl("LB_HExamContentID")).Text);
            cmdInsHEL.Parameters.AddWithValue("@HExamSubjectID", ((Label)RICB.FindControl("LB_HExamSubjectID")).Text);
            cmdInsHEL.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmdInsHEL.Parameters.AddWithValue("@HStatus", "1");
            cmdInsHEL.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmdInsHEL.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmdInsHEL.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmdInsHEL.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmdInsHEL.ExecuteNonQuery();
            con.Close();
            cmdInsHEL.Cancel();
        }
        else
        {
            //刪除
            SqlCommand cmdDelHEL = new SqlCommand("DELETE FROM HShoppingCart_Exam WHERE HShoppingCartID=@HShoppingCartID AND HExamContentID=@HExamContentID AND HExamSubjectID=@HExamSubjectID", con);
            con.Open();
            cmdDelHEL.Parameters.AddWithValue("@HShoppingCartID", ViewState["ClickShoppingCartID"].ToString());
            cmdDelHEL.Parameters.AddWithValue("@HExamContentID", ((Label)RICB.FindControl("LB_HExamContentID")).Text);
            cmdDelHEL.Parameters.AddWithValue("@HExamSubjectID", ((Label)RICB.FindControl("LB_HExamSubjectID")).Text);
            cmdDelHEL.ExecuteNonQuery();
            con.Close();
            cmdDelHEL.Cancel();
        }




        SDS_Cultural.SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCTemplateID, a.HCourseName, a.HCourseID, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, (b.HBCPoint*10) AS CPrice, MAX(a.HPoint*10) AS SCPrice, a.HLDiscount*10 AS LDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange LEFT JOIN HExamContent AS c ON b.HExamContentID=c.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseDonate='0' AND b.HPMethod='2' AND b.HVerifyStatus='2' AND b.HStatus='1' GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, a.HLDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod";
        SDS_Cultural.DataBind();
        Rpt_Cultural.DataBind();


        SDS_FoundationC.SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HCourseID, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, (b.HBCPoint*10) AS FCPrice, MAX(a.HPoint) AS SFCPrice, a.HLDiscount*10 AS LDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange LEFT JOIN HExamContent AS c ON b.HExamContentID=c.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseDonate='0' AND b.HPMethod='1' AND b.HVerifyStatus='2' AND b.HStatus='1'  GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, a.HLDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod";
        Rpt_FoundationC.DataBind();
        Rpt_FoundationC.DataBind();



        //頁面重新整理
        //ScriptManager.RegisterStartupScript(this, this.GetType(), "reload", "location.reload();", true);








    }
    #endregion

    #region 檢覈課程科目儲存
    protected void Btn_SubmitSubject_Click(object sender, EventArgs e)
    {
        ////執行js
        //RegScript();


        //for (int i = 0; i < Rpt_Cultural.Items.Count; i++)  //傳光課程(傳光(基金會)課程)
        //{
        //    //刪除
        //    SqlCommand cmdDelHEL = new SqlCommand("DELETE FROM HShoppingCart_Exam WHERE HExamContentID=@HExamContentID", con);
        //    con.Open();
        //    cmdDelHEL.Parameters.AddWithValue("@HExamContentID", ((Label)RICB.FindControl("LB_HExamContentID")).Text);
        //    cmdDelHEL.ExecuteNonQuery();
        //    con.Close();
        //    cmdDelHEL.Cancel();

        //    if (((CheckBox)Rpt_Cultural.Items[i].FindControl("CB_SelectSubject")).Checked)
        //    {
        //        //新增
        //        SqlCommand cmdInsHEL = new SqlCommand("INSERT INTO HShoppingCart_Exam (HExamContentID, HExamSubjectID, HMemberID, HStatus, HCreate, HCreateDT, HModify, HModifyDT) values (@HExamContentID, @HExamSubjectID, @HMemberID, @HStatus, @HCreate, @HCreateDT, @HModify, @HModifyDT)", con);
        //        con.Open();
        //        cmdInsHEL.Parameters.AddWithValue("@HExamContentID", ((Label)RICB.FindControl("LB_HExamContentID")).Text);
        //        cmdInsHEL.Parameters.AddWithValue("@HExamSubjectID", ((Label)RICB.FindControl("LB_HExamSubjectID")).Text);
        //        cmdInsHEL.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        //        cmdInsHEL.Parameters.AddWithValue("@HStatus", "1");
        //        cmdInsHEL.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        //        cmdInsHEL.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        //        cmdInsHEL.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        //        cmdInsHEL.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        //        cmdInsHEL.ExecuteNonQuery();
        //        con.Close();
        //        cmdInsHEL.Cancel();
        //    }
        //}


        //for (int i = 0; i < Rpt_FoundationC.Items.Count; i++)  //傳光課程(傳光(基金會)課程)
        //{
        //    if (((CheckBox)RICB.FindControl("CB_SelectSubject")).Checked)
        //    {
        //        //新增
        //        SqlCommand cmdInsHEL = new SqlCommand("INSERT INTO HShoppingCart_Exam (HExamContentID, HExamSubjectID, HMemberID, HStatus, HCreate, HCreateDT, HModify, HModifyDT) values (@HExamContentID, @HExamSubjectID, @HMemberID, @HStatus, @HCreate, @HCreateDT, @HModify, @HModifyDT)", con);
        //        con.Open();
        //        cmdInsHEL.Parameters.AddWithValue("@HExamContentID", ((Label)RICB.FindControl("LB_HExamContentID")).Text);
        //        cmdInsHEL.Parameters.AddWithValue("@HExamSubjectID", ((Label)RICB.FindControl("LB_HExamSubjectID")).Text);
        //        cmdInsHEL.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        //        cmdInsHEL.Parameters.AddWithValue("@HStatus", "1");
        //        cmdInsHEL.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        //        cmdInsHEL.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        //        cmdInsHEL.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        //        cmdInsHEL.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        //        cmdInsHEL.ExecuteNonQuery();
        //        con.Close();
        //        cmdInsHEL.Cancel();
        //    }
        //    else
        //    {
        //        //刪除
        //        SqlCommand cmdDelHEL = new SqlCommand("DELETE FROM HShoppingCart_Exam WHERE HExamSubjectID=@HExamSubjectID", con);
        //        con.Open();
        //        cmdDelHEL.Parameters.AddWithValue("@HExamSubjectID", ((Label)RICB.FindControl("LB_HExamSubjectID")).Text);
        //        cmdDelHEL.ExecuteNonQuery();
        //        con.Close();
        //        cmdDelHEL.Cancel();
        //    }
        //}

        //ScriptManager.RegisterStartupScript(this, this.GetType(), "reload", "location.reload();", true);
    }
    #endregion

    #region 捐款
    protected void Rpt_FoundationD_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //取得Repeater的DataRowView
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        #region 計算勾選的課程總金額
        if (((CheckBox)e.Item.FindControl("CB_SelectFD")).Checked == true)
        {
            gTotalFD += Convert.ToInt32(((Label)e.Item.FindControl("LB_SubTotalFD")).Text.Trim());
            LB_Total.Text = (gTotalFC + gTotalFD).ToString();
        }
        #endregion


    }
    #endregion

    #region   報名/捐款清單 Page

    #region 文化課程-勾選

    protected void CB_SelectC_CheckedChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        RepeaterItem item = (sender as CheckBox).NamingContainer as RepeaterItem;


        #region  先判斷是否已有勾選傳光的項目
        if (((CheckBox)item.FindControl("CB_SelectC")).Checked == true)
        {

            string ckSelectFC = "0";
            string ckSelectFD = "0";
            for (int i = 0; i < Rpt_FoundationC.Items.Count; i++)  //傳光課程(傳光(基金會)課程)
            {
                if (((CheckBox)Rpt_FoundationC.Items[i].FindControl("CB_SelectFC")).Checked == true)
                {
                    ckSelectFC = "1";
                }
            }
            for (int i = 0; i < Rpt_FoundationD.Items.Count; i++)    //捐款
            {
                if (((CheckBox)Rpt_FoundationD.Items[i].FindControl("CB_SelectFD")).Checked == true)
                {
                    ckSelectFD = "1";
                }
            }

            if (ckSelectFC == "1" || ckSelectFD == "1")
            {
                LB_Navtab.Text = "1";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice4", "alert('傳光與玉成不能同時結帳哦~ 如要先結玉成課程，請先取消傳光勾選的課程~謝謝~!')", true);
                ((CheckBox)item.FindControl("CB_SelectC")).Checked = false;
                //return;
            }
            else
            {

                LB_Navtab.Text = "2";
                //221013-  勾選的時候也要UPDATE付款金額
                //221230-  加入UPDATE HPAmount & HSubTotal
                //GE20231218_加入判斷:若小計不為空才update
                if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HSubTotalC")).Text))
                {
                    decimal gDPoint = 0;
                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDPointC")).Text))
                    {
                        gDPoint = Convert.ToInt32(((Label)item.FindControl("LB_HDPointC")).Text);
                    }
                    else
                    {
                        gDPoint = 0;
                    }

                    SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect, HAttend=@HAttend, HPAmount=@HPAmount, HSubTotal=@HSubTotal, HPoint = @HPoint WHERE HID =@HID", SQLdatabase.OpenConnection());
                    QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDC")).Text);
                    QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectC")).Checked);
                    QueryUpdSC.Parameters.AddWithValue("@HAttend", ((DropDownList)item.FindControl("DDL_HAttendC")).SelectedValue);
                    QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim());  //存使用者輸入的金額
                    QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalC")).Text);   //存金額
                    QueryUpdSC.Parameters.AddWithValue("@HPoint", Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text) / 10);  //文化課程要轉成點數存入
                    QueryUpdSC.Parameters.AddWithValue("@HDPoint", gDPoint / 10);    //文化課程要轉成點數存入
                    QueryUpdSC.ExecuteNonQuery();
                    QueryUpdSC.Cancel();
                    SQLdatabase.OpenConnection().Close();
                }
                //GA20240316_加入金額為0
                else if (string.IsNullOrEmpty(((Label)item.FindControl("LB_HSubTotalC")).Text))
                {
                    SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect WHERE HID =@HID", SQLdatabase.OpenConnection());
                    QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDC")).Text);
                    QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectC")).Checked);
                    QueryUpdSC.ExecuteNonQuery();
                    QueryUpdSC.Cancel();
                    SQLdatabase.OpenConnection().Close();
                }

            }
        }
        else
        {
            SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect WHERE HID =@HID", SQLdatabase.OpenConnection());
            QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDC")).Text);
            QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectC")).Checked);
            QueryUpdSC.ExecuteNonQuery();
            QueryUpdSC.Cancel();
            SQLdatabase.OpenConnection().Close();
        }
        #endregion



    }
    #endregion



    #region 傳光(基金會)課程-勾選
    protected void CB_SelectFC_CheckedChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        RepeaterItem item = (sender as CheckBox).NamingContainer as RepeaterItem;


        #region  先判斷是否已有勾選玉成的項目
        if (((CheckBox)item.FindControl("CB_SelectFC")).Checked == true)
        {
            string ckSelectC = "0";
            for (int i = 0; i < Rpt_Cultural.Items.Count; i++)  //傳光課程(傳光(基金會)課程)
            {
                if (((CheckBox)Rpt_Cultural.Items[i].FindControl("CB_SelectC")).Checked == true)
                {
                    ckSelectC = "1";
                }
            }

            if (ckSelectC == "1")
            {
                LB_Navtab.Text = "2";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice4", "alert('傳光與玉成不能同時結帳哦~ 如要先結傳光課程，請先取消玉成勾選的課程~謝謝~!')", true);
                ((CheckBox)item.FindControl("CB_SelectFC")).Checked = false;
                //return;
            }
            else
            {
                LB_Navtab.Text = "1";

                //221013-  勾選的時候也要UPDATE付款金額
                SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect, HAttend=@HAttend, HPAmount=@HPAmount, HSubTotal=@HSubTotal, HPoint = @HPoint WHERE HID =@HID", SQLdatabase.OpenConnection());
                QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDFC")).Text);
                QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectFC")).Checked);
                QueryUpdSC.Parameters.AddWithValue("@HAttend", ((DropDownList)item.FindControl("DDL_HAttendFC")).SelectedValue);
                QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim());  //存使用者輸入的金額
                QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalFC")).Text);   //存金額
                QueryUpdSC.Parameters.AddWithValue("@HPoint", ((Label)item.FindControl("LB_HSubTotalFC")).Text.Trim());  //傳光(基金會)的課程存入是金額
                QueryUpdSC.ExecuteNonQuery();
                QueryUpdSC.Cancel();
                SQLdatabase.OpenConnection().Close();

            }
        }
        else
        {
            SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect WHERE HID =@HID", SQLdatabase.OpenConnection());
            QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDFC")).Text);
            QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectFC")).Checked);
            QueryUpdSC.ExecuteNonQuery();
            QueryUpdSC.Cancel();
            SQLdatabase.OpenConnection().Close();
        }
        #endregion



    }
    #endregion



    #region 課程捐款-勾選
    protected void CB_SelectFD_CheckedChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        RepeaterItem item = (sender as CheckBox).NamingContainer as RepeaterItem;

        #region  先判斷是否已有勾選玉成的項目
        if (((CheckBox)item.FindControl("CB_SelectFD")).Checked == true)
        {
            string ckSelectFD = "0";
            for (int i = 0; i < Rpt_Cultural.Items.Count; i++)  //傳光課程(傳光(基金會)課程)
            {
                if (((CheckBox)Rpt_Cultural.Items[i].FindControl("CB_SelectC")).Checked == true)
                {
                    ckSelectFD = "1";
                }
            }

            if (ckSelectFD == "1")
            {
                LB_Navtab.Text = "2";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice4", "alert('傳光與玉成不能同時結帳哦~ 如要先結傳光課程，請先取消玉成勾選的課程~謝謝~!')", true);
                ((CheckBox)item.FindControl("CB_SelectFD")).Checked = false;
                //return;
            }
            else
            {
                LB_Navtab.Text = "1";

                //221013-  勾選的時候也要UPDATE付款金額
                SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect, HPoint = @HPoint,HPAmount=@HPAmount, HSubTotal=@HSubTotal  WHERE HID =@HID", SQLdatabase.OpenConnection());
                QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDFD")).Text);
                QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectFD")).Checked);
                QueryUpdSC.Parameters.AddWithValue("@HPoint", ((TextBox)item.FindControl("TB_PAmountFD")).Text.Trim());  //不管文化還是傳光(基金會)的存入直接是金額
                QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_PAmountFD")).Text.Trim());
                QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((TextBox)item.FindControl("TB_PAmountFD")).Text.Trim());
                QueryUpdSC.ExecuteNonQuery();
                QueryUpdSC.Cancel();
                SQLdatabase.OpenConnection().Close();
            }
        }
        else
        {
            //221013-  勾選的時候也要UPDATE付款金額
            SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect WHERE HID =@HID", SQLdatabase.OpenConnection());
            QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDFD")).Text);
            QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectFD")).Checked);
            QueryUpdSC.ExecuteNonQuery();
            QueryUpdSC.Cancel();
            SQLdatabase.OpenConnection().Close();
        }
        #endregion


    }
    #endregion



    #region 文化課程-上課地點選擇後帶出課程費用
    protected void DDL_HOCPlaceC_SelectedIndexChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();


        RepeaterItem item = (sender as DropDownList).NamingContainer as RepeaterItem;

        string gHCourseID = "";

        //221031-加入條件:課程為已審核通過且未下架
        SqlDataReader QuerySelSC = SQLdatabase.ExecuteReader("SELECT HID AS HCourseID FROM HCourse WHERE HOCPlace='" + ((DropDownList)item.FindControl("DDL_HOCPlaceC")).SelectedValue + "' AND HCTemplateID = '" + ((Label)item.FindControl("LB_CTemplateIDC")).Text + "' AND HCourseName = N'" + ((Label)item.FindControl("LB_CourseNameC")).Text + "' AND HDateRange = '" + ((Label)item.FindControl("LB_DateRangeC")).Text + "' AND HVerifyStatus=2 AND HStatus=1");
        gHCourseID = QuerySelSC.Read() == true ? QuerySelSC["HCourseID"].ToString() : "0";
        QuerySelSC.Close();


        //221204-將所有相關金額的資料變回預設值(付款金額、折扣碼、折扣金額及小計、是否參班改回參班(一般)都要清掉)
        SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HCourseID=@HCourseID, HPoint =@HPoint, HAttend=@HAttend, HPAmount=@HPAmount, HSubTotal=@HSubTotal, HDCode=@HDCode, HDPoint=@HDPoint WHERE HID =@HID", SQLdatabase.OpenConnection());
        QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDC")).Text);
        QueryUpdSC.Parameters.AddWithValue("@HCourseID", gHCourseID);
        QueryUpdSC.Parameters.AddWithValue("@HAttend", "1");
        QueryUpdSC.Parameters.AddWithValue("@HPoint", Convert.ToInt32(((Label)item.FindControl("LB_HBCPointC")).Text) / 10);  //文化課程要除以10(點數)
        QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((Label)item.FindControl("LB_HBCPointC")).Text);
        QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HBCPointC")).Text);
        QueryUpdSC.Parameters.AddWithValue("@HDCode", "");
        QueryUpdSC.Parameters.AddWithValue("@HDPoint", "");
        QueryUpdSC.ExecuteNonQuery();
        QueryUpdSC.Cancel();
        SQLdatabase.OpenConnection().Close();

        //地點不同基本費用會相同，所以不需要重新讀取-暫時註解-(註解會影響折扣碼輸入)
        //GE20231219_加入前導課程折扣金額
        SDS_Cultural.SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCTemplateID, a.HCourseName, a.HCourseID, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, (b.HBCPoint*10) AS CPrice, MAX(a.HPoint*10) AS SCPrice, a.HLDiscount*10 AS LDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange LEFT JOIN HExamContent AS c ON b.HExamContentID=c.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseDonate='0' AND b.HPMethod='2' AND b.HVerifyStatus='2' AND b.HStatus='1' GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, a.HLDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod";
        SDS_Cultural.DataBind();
        Rpt_Cultural.DataBind();


    }
    #endregion


    #region 傳光(基金會)課程-上課地點選擇後帶出課程費用
    protected void DDL_HOCPlaceFC_SelectedIndexChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        RepeaterItem item = (sender as DropDownList).NamingContainer as RepeaterItem;

        string gHCourseID = "";

        //221031-加入條件:課程為已審核通過且未下架
        SqlDataReader QuerySelSC = SQLdatabase.ExecuteReader("SELECT HID AS HCourseID FROM HCourse WHERE HOCPlace='" + ((DropDownList)item.FindControl("DDL_HOCPlaceFC")).SelectedValue + "' AND HCTemplateID = '" + ((Label)item.FindControl("LB_CTemplateIDFC")).Text + "' AND HCourseName = N'" + ((Label)item.FindControl("LB_CourseNameFC")).Text + "' AND HDateRange = '" + ((Label)item.FindControl("LB_DateRangeFC")).Text + "' AND HVerifyStatus=2 AND HStatus=1");

        gHCourseID = QuerySelSC.Read() == true ? QuerySelSC["HCourseID"].ToString() : "0";
        QuerySelSC.Close();


        //221204-將所有相關金額的資料變回預設值(付款金額、折扣碼、折扣金額及小計、是否參班改回參班(一般)都要清掉)
        SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HCourseID=@HCourseID, HPoint =@HPoint, HAttend=@HAttend, HPAmount=@HPAmount, HSubTotal=@HSubTotal, HDCode=@HDCode, HDPoint=@HDPoint WHERE HID =@HID", SQLdatabase.OpenConnection());
        QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDFC")).Text);
        QueryUpdSC.Parameters.AddWithValue("@HCourseID", gHCourseID);
        QueryUpdSC.Parameters.AddWithValue("@HAttend", "1");
        QueryUpdSC.Parameters.AddWithValue("@HPoint", ((Label)item.FindControl("LB_HBCPointFC")).Text);
        QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((Label)item.FindControl("LB_HBCPointFC")).Text);
        QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HBCPointFC")).Text);
        QueryUpdSC.Parameters.AddWithValue("@HDCode", "");
        QueryUpdSC.Parameters.AddWithValue("@HDPoint", "");
        QueryUpdSC.ExecuteNonQuery();
        QueryUpdSC.Cancel();
        SQLdatabase.OpenConnection().Close();


        //地點不同基本費用會相同，所以不需要重新讀取-暫時註解-(註解會影響折扣碼輸入)
        SDS_FoundationC.SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HCourseID, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, (b.HBCPoint*10) AS FCPrice, MAX(a.HPoint) AS SFCPrice, a.HLDiscount*10 AS LDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange LEFT JOIN HExamContent AS c ON b.HExamContentID=c.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseDonate='0' AND b.HPMethod='1' AND b.HVerifyStatus='2' AND b.HStatus='1'  GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, a.HLDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod";
        Rpt_FoundationC.DataBind();
        Rpt_FoundationC.DataBind();



    }
    #endregion


    #region 課程捐款-上課地點選擇後帶出課程費用
    protected void DDL_HOCPlaceFD_SelectedIndexChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        RepeaterItem item = (sender as DropDownList).NamingContainer as RepeaterItem;

        string gHCourseID = "";

        //221031-加入條件:課程為已審核通過且未下架
        SqlDataReader QuerySelSC = SQLdatabase.ExecuteReader("SELECT HID AS HCourseID FROM HCourse WHERE HOCPlace='" + ((DropDownList)item.FindControl("DDL_HOCPlaceFD")).SelectedValue + "' AND HCTemplateID = '" + ((Label)item.FindControl("LB_CTemplateIDFD")).Text + "' AND HCourseName = N'" + ((Label)item.FindControl("LB_CourseNameFD")).Text + "' AND HDateRange = '" + ((Label)item.FindControl("LB_DateRangeFD")).Text + "' AND HVerifyStatus=2 AND HStatus=1");
        gHCourseID = QuerySelSC.Read() == true ? QuerySelSC["HCourseID"].ToString() : "0";
        QuerySelSC.Close();

        SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HCourseID=@HCourseID WHERE HID =@HID", SQLdatabase.OpenConnection());
        QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDFD")).Text);
        QueryUpdSC.Parameters.AddWithValue("@HCourseID", gHCourseID);
        QueryUpdSC.ExecuteNonQuery();
        QueryUpdSC.Cancel();
        SQLdatabase.OpenConnection().Close();



    }
    #endregion

    #region 文化課程-是否參班選擇
    protected void DDL_HAttendC_SelectedIndexChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        RepeaterItem item = (sender as DropDownList).NamingContainer as RepeaterItem;


        ((TextBox)item.FindControl("TB_HDCodeC")).Text = null;
        ((Label)item.FindControl("LB_HDPointC")).Text = "0";

        //GA20231219_加入前導課程折扣金額計算
        if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text))
        {
            ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountC")).Text)).ToString();
        }
        else
        {
            ((Label)item.FindControl("LB_HDiscountC")).Text = "";
        }


        //1=參班，5=純護持，6=參班兼護持
        if (((DropDownList)item.FindControl("DDL_HAttendC")).SelectedValue == "1")
        {
            ((TextBox)item.FindControl("TB_HPAmountC")).Text = ((Label)item.FindControl("LB_HBCPointC")).Text;
            //((Label)item.FindControl("LB_HSubTotalC")).Text = ((Label)item.FindControl("LB_HBCPointC")).Text;
        }
        else if (((DropDownList)item.FindControl("DDL_HAttendC")).SelectedValue == "5")
        {
            ((TextBox)item.FindControl("TB_HPAmountC")).Text = "0";
            //((Label)item.FindControl("LB_HSubTotalC")).Text = "0";
        }
        //GA20231209_加入參班兼護持附款金額:與參班相同
        else if (((DropDownList)item.FindControl("DDL_HAttendC")).SelectedValue == "6")  //參班兼護持
        {
            ((TextBox)item.FindControl("TB_HPAmountC")).Text = ((Label)item.FindControl("LB_HBCPointC")).Text;
            //((Label)item.FindControl("LB_HSubTotalC")).Text = ((Label)item.FindControl("LB_HBCPointC")).Text;
        }


        //GE20231219_計算小計
        if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountC")).Text))
        {
            ((Label)item.FindControl("LB_HSubTotalC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text.Replace(",", string.Empty))).ToString();
        }
        else
        {
            ((Label)item.FindControl("LB_HSubTotalC")).Text = ((Label)item.FindControl("LB_HBCPointC")).Text;
        }

        if (Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text) < 0)
        {
            ((Label)item.FindControl("LB_HSubTotalC")).Text = "0";
        }

        //HA20231217_當基本費用是0元時，付款金額不預設帶值，並顯示請自行輸入(純護持除外)-S
        if ((((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim() == "0" || ((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim() == "") && (((Label)item.FindControl("LB_HSubTotalC")).Text.Trim() == "0" || ((Label)item.FindControl("LB_HSubTotalC")).Text.Trim() == "") && ((DropDownList)item.FindControl("DDL_HAttendC")).SelectedValue != "5")
        {
            ((TextBox)item.FindControl("TB_HPAmountC")).Text = "";
            ((TextBox)item.FindControl("TB_HPAmountC")).Attributes.Add("PlaceHolder", "請自行輸入");
            ((Label)item.FindControl("LB_HSubTotalC")).Text = "";
            //GA20231218_顯示提示文字
            LB_HNotice.Visible = true;
        }
        //HA20231217_當基本費用是0元時，付款金額不預設帶值，並顯示請自行輸入(純護持除外)-E





        #region 更新金額&參班狀態
        //更新資料庫
        SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HAttend=@HAttend, HPoint=@HPoint, HPAmount=@HPAmount, HDPoint=@HDPoint, HSubTotal=@HSubTotal, HDCode=@HDCode WHERE HID =@HID", SQLdatabase.OpenConnection());
        QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDC")).Text);
        QueryUpdSC.Parameters.AddWithValue("@HAttend", ((DropDownList)item.FindControl("DDL_HAttendC")).SelectedValue);
        if (((Label)item.FindControl("LB_HSubTotalC")).Text == "")
        {
            QueryUpdSC.Parameters.AddWithValue("@HPoint", "");  //文化事業課程直接存點數
        }
        else
        {
            QueryUpdSC.Parameters.AddWithValue("@HPoint", Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text) / 10);  //文化事業課程直接存點數
        }
        QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim());
        QueryUpdSC.Parameters.AddWithValue("@HDPoint", ((Label)item.FindControl("LB_HDPointC")).Text); //折扣碼折扣金額
        QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalC")).Text);
        QueryUpdSC.Parameters.AddWithValue("@HDCode", ((TextBox)item.FindControl("TB_HDCodeC")).Text.Trim());
        QueryUpdSC.ExecuteNonQuery();
        QueryUpdSC.Cancel();
        SQLdatabase.OpenConnection().Close();

        #endregion
    }
    #endregion



    #region 傳光(基金會)課程-是否參班選擇
    protected void DDL_HAttendFC_SelectedIndexChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        RepeaterItem item = (sender as DropDownList).NamingContainer as RepeaterItem;

        ((TextBox)item.FindControl("TB_HDCodeFC")).Text = null;
        ((Label)item.FindControl("LB_HDPointFC")).Text = "0";

        //GA20231219_加入前導課程折扣金額計算
        if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text))
        {
            ((Label)item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountFC")).Text)).ToString();
        }
        else
        {
            ((Label)item.FindControl("LB_HDiscountFC")).Text = "";
        }

        //1：參班【一般】 2：參班【學青】3：參班【經濟困難】4：參班【護持出家師父】5：不參班(純護持)->改為:護持體系專業-->改成純護持(不參班)
        //2、3、4已停用
        if (((DropDownList)item.FindControl("DDL_HAttendFC")).SelectedValue == "1")
        {
            ((TextBox)item.FindControl("TB_HPAmountFC")).Text = ((Label)item.FindControl("LB_HBCPointFC")).Text;
            //((Label)item.FindControl("LB_HSubTotalFC")).Text = ((Label)item.FindControl("LB_HBCPointFC")).Text;

        }
        else if (((DropDownList)item.FindControl("DDL_HAttendFC")).SelectedValue == "5")
        {
            ((TextBox)item.FindControl("TB_HPAmountFC")).Text = "0";
            //((Label)item.FindControl("LB_HSubTotalFC")).Text = "0";
        }
        //GA20231209_加入參班兼護持附款金額:與參班相同
        else if (((DropDownList)item.FindControl("DDL_HAttendFC")).SelectedValue == "6")  //參班兼護持
        {
            ((TextBox)item.FindControl("TB_HPAmountFC")).Text = ((Label)item.FindControl("LB_HBCPointFC")).Text;
            //((Label)item.FindControl("LB_HSubTotalFC")).Text = ((Label)item.FindControl("LB_HBCPointFC")).Text;
        }

        //GE20231219_計算小計
        if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountFC")).Text))
        {
            ((Label)item.FindControl("LB_HSubTotalFC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountFC")).Text.Replace(",", string.Empty))).ToString();
        }
        else
        {
            ((Label)item.FindControl("LB_HSubTotalFC")).Text = ((Label)item.FindControl("LB_HBCPointFC")).Text;
        }

        if (Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalFC")).Text) < 0)
        {
            ((Label)item.FindControl("LB_HSubTotalFC")).Text = "0";
        }


        //HA20231217_當基本費用是0元時，付款金額不預設帶值，並顯示請自行輸入(純護持除外)-S
        if ((((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim() == "0" || ((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim() == "") && (((Label)item.FindControl("LB_HSubTotalFC")).Text.Trim() == "0" || ((Label)item.FindControl("LB_HSubTotalFC")).Text.Trim() == "") && ((DropDownList)item.FindControl("DDL_HAttendFC")).SelectedValue != "5")
        {
            ((TextBox)item.FindControl("TB_HPAmountFC")).Text = "";
            ((TextBox)item.FindControl("TB_HPAmountFC")).Attributes.Add("PlaceHolder", "請自行輸入");
            ((Label)item.FindControl("LB_HSubTotalFC")).Text = "";
            //GA20231218_顯示提示文字
            LB_HNotice.Visible = true;
        }
        //HA20231217_當基本費用是0元時，付款金額不預設帶值，並顯示請自行輸入(純護持除外)-E




        #region 更新金額&參班狀態
        //更新資料庫
        SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HAttend=@HAttend, HPoint=@HPoint, HPAmount=@HPAmount, HDPoint=@HDPoint, HSubTotal=@HSubTotal, HDCode=@HDCode WHERE HID =@HID", SQLdatabase.OpenConnection());
        QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDFC")).Text);
        QueryUpdSC.Parameters.AddWithValue("@HAttend", ((DropDownList)item.FindControl("DDL_HAttendFC")).SelectedValue);
        QueryUpdSC.Parameters.AddWithValue("@HPoint", (((Label)item.FindControl("LB_HSubTotalFC")).Text));  //基金會課程直接存金額(已扣完折扣金額)
        QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim());
        QueryUpdSC.Parameters.AddWithValue("@HDPoint", ((Label)item.FindControl("LB_HDPointFC")).Text); //折扣碼折扣金額
        QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalFC")).Text);
        QueryUpdSC.Parameters.AddWithValue("@HDCode", ((TextBox)item.FindControl("TB_HDCodeFC")).Text.Trim());
        QueryUpdSC.ExecuteNonQuery();
        QueryUpdSC.Cancel();
        SQLdatabase.OpenConnection().Close();

        #endregion



    }
    #endregion




    #region 課程捐款-是否參班選擇
    protected void DDL_HAttendFD_SelectedIndexChanged(object sender, EventArgs e)
    {
        
    }
    #endregion


    #region 文化課程-付款金額切換
    protected void TB_HPAmountC_TextChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();


        RepeaterItem item = (sender as TextBox).NamingContainer as RepeaterItem;
        //221204-加入判斷要選完上課地點
        if (((Label)item.FindControl("LB_CourseIDC")).Text == "0")
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('請先選擇完上課地點再輸入付款金額喔~');", true);
            ((TextBox)item.FindControl("TB_HPAmountC")).Text = ((Label)item.FindControl("LB_HBCPointC")).Text;
            return;
        }

        if (!string.IsNullOrEmpty(((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim()))
        {

            //20240221_新增參班兼護持的判斷
            if ((((DropDownList)item.FindControl("DDL_HAttendC")).SelectedValue == "1" || ((DropDownList)item.FindControl("DDL_HAttendC")).SelectedValue == "6") && (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text) < Convert.ToInt32(((Label)item.FindControl("LB_HBCPointC")).Text)))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice", "alert('輸入金額不能小於基本費用喔~!')", true);
                ((TextBox)item.FindControl("TB_HPAmountC")).Text = ((Label)item.FindControl("LB_HBCPointC")).Text;


                if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountC")).Text))
                {
                    ((Label)item.FindControl("LB_HSubTotalC")).Text = Convert.ToDouble(Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text)).ToString();
                }
                else
                {
                    ((Label)item.FindControl("LB_HSubTotalC")).Text = ((Label)item.FindControl("LB_HBCPointC")).Text;
                }

                return;
            }
            else
            {

                //沒有輸入折扣碼時
                //GE20231219_加入小計金額計算
                if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text))
                {
                    ((Label)item.FindControl("LB_HSubTotalC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text.Replace(",", string.Empty))).ToString();
                }
                else
                {
                    ((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text;

                }

                #region 折扣金額
                if (((Label)item.FindControl("LB_CourseIDC")).Text != "0" && !string.IsNullOrEmpty(((TextBox)item.FindControl("TB_HDCodeC")).Text.Trim()))
                {
                    SqlDataReader QuerySelHDC = SQLdatabase.ExecuteReader("SELECT HID, HDCode, HDSummary, HDPoint, HCourseID, HEmailGroup, HEmailPerson, HDType, HStatus FROM HDiscountCode WHERE HDCode='" + ((TextBox)item.FindControl("TB_HDCodeC")).Text.Trim() + "' and HStatus='1'");

                    if (QuerySelHDC.Read())
                    {
                        string gHCourseIDCK = "0";  //判斷購買人員輸入的折扣碼有無在HCourseID裡
                        string gHEmailGroupCK = "0";  //判斷購買人員輸入的折扣碼有無在HEmailGroup裡
                        string gHEmailPersonCK = "0";  //判斷購買人員輸入的折扣碼有無在HEmailPerson裡

                        //判斷購買人員輸入的折扣碼有無在HCourseID裡
                        string[] gHCourseID = QuerySelHDC["HCourseID"].ToString().Split(',');
                        for (int i = 0; i < gHCourseID.Length; i++)
                        {
                            if (((Label)item.FindControl("LB_CourseIDC")).Text == gHCourseID[i])
                            {
                                //LB_HDPoint.Text = QuerySelHDC["HDPoint"].ToString();
                                gHCourseIDCK = "1";
                            }
                        }

                        //判斷購買人員的折扣碼有無在HEmailGroup裡
                        string[] gHEmailGroup = QuerySelHDC["HEmailGroup"].ToString().Split(',');
                        for (int i = 0; i < gHEmailGroup.Length; i++)
                        {
                            SqlDataReader QuerySelHDCG = SQLdatabase.ExecuteReader("SELECT HID, HGNum, HGName, HMemberID, HGType, HStatus FROM HDCGroup WHERE HID='" + gHEmailGroup[i].ToString() + "' and HStatus='1'");
                            if (QuerySelHDCG.Read())
                            {
                                string[] gHMemberID = QuerySelHDCG["HMemberID"].ToString().Split(',');
                                for (int j = 0; j < gHMemberID.Length; j++)
                                {
                                    if (((Label)Master.FindControl("LB_HUserHID")).Text == gHMemberID[j])
                                    {
                                        //LB_HDPoint.Text = QuerySelHDCG["HDPoint"].ToString();
                                        gHEmailGroupCK = "1";
                                    }
                                }
                            }
                            QuerySelHDCG.Close();
                        }

                        //判斷購買人員輸入的折扣碼有無在HEmailPerson裡
                        string[] gHEmailPerson = QuerySelHDC["HEmailPerson"].ToString().Split(',');
                        for (int j = 0; j < gHEmailPerson.Length; j++)
                        {
                            if (((Label)Master.FindControl("LB_HUserHID")).Text == gHEmailPerson[j])
                            {
                                //LB_HDPoint.Text = QuerySelHDC["HDPoint"].ToString();
                                gHEmailPersonCK = "1";
                            }

                        }




                        if (gHCourseIDCK == "1" && (gHEmailGroupCK == "1" || gHEmailPersonCK == "1"))
                        {
                            //折扣金額
                            ((Label)item.FindControl("LB_HDPointC")).Text = (Convert.ToInt32(QuerySelHDC["HDPoint"].ToString()) * 10).ToString();

                            //GE20231219_總折扣金額計算加入前導課程折扣金額
                            if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text) && !string.IsNullOrEmpty(QuerySelHDC["HDPoint"].ToString()))
                            {
                                ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(QuerySelHDC["HDPoint"].ToString()) * 10 + Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountC")).Text)).ToString();
                            }
                            else if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text) && string.IsNullOrEmpty(QuerySelHDC["HDPoint"].ToString()))
                            {
                                ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountC")).Text)).ToString();
                            }
                            else if (string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text) && !string.IsNullOrEmpty(QuerySelHDC["HDPoint"].ToString()))
                            {
                                ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(QuerySelHDC["HDPoint"].ToString()) * 10).ToString();
                            }
                            else
                            {
                                ((Label)item.FindControl("LB_HDiscountC")).Text = "";
                            }

                            //((Label)item.FindControl("LB_HDPointC")).Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HDPointC")).Text));
                            //折扣總金額(轉千分位)
                            ((Label)item.FindControl("LB_HDiscountC")).Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text));

                            //((Label)item.FindControl("LB_HSubTotalC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDPointC")).Text.Replace(",", string.Empty))).ToString();

                            //GE20231219_改成扣折扣總金額
                            ((Label)item.FindControl("LB_HSubTotalC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text.Replace(",", string.Empty))).ToString();

                            if (Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text) < 0)
                            {
                                ((Label)item.FindControl("LB_HSubTotalC")).Text = "0";
                            }

                            HDisPoint = Convert.ToInt32(((Label)item.FindControl("LB_HDPointC")).Text.Replace(",", string.Empty));
                            //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)) - HDisPoint);//總計=小計-折扣碼金額
                            //GE20231219
                            LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)));
                            //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text) - HDisPoint);//總計=小計-前導課程減免總金額-折扣碼金額

                            if (Convert.ToInt32(LB_Total.Text.Replace(",", string.Empty)) < 0)
                            {
                                LB_Total.Text = "0";
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('折扣碼錯誤，請重新輸入。');", true);
                            ((TextBox)item.FindControl("TB_HDCodeC")).Text = "";
                            ((TextBox)item.FindControl("TB_HDCodeC")).Focus();
                            ((Label)item.FindControl("LB_HDPointC")).Text = "0";
                            HDisPoint = 0;

                            //GE20231219_加入前導課程折扣金額計算
                            if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text))
                            {
                                ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountC")).Text)).ToString();
                            }
                            else
                            {
                                ((Label)item.FindControl("LB_HDiscountC")).Text = "";
                            }

                            //221013-小計=輸入的付款金額
                            //((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text;

                            //GE202312219_修改小計計算
                            if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountC")).Text))
                            {
                                ((Label)item.FindControl("LB_HSubTotalC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text.Replace(",", string.Empty))).ToString();
                            }
                            else
                            {
                                ((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text;
                            }

                            LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)));//總計=小計
                                                                                                                                                                 //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text));//總計=小計-前導課程減免總金額

                            if (Convert.ToInt32(LB_Total.Text.Replace(",", string.Empty)) < 0)
                            {
                                LB_Total.Text = "0";
                            }

                        }


                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('折扣碼錯誤，請重新輸入。');", true);
                        ((TextBox)item.FindControl("TB_HDCodeC")).Text = "";
                        ((TextBox)item.FindControl("TB_HDCodeC")).Focus();
                        ((Label)item.FindControl("LB_HDPointC")).Text = "0";
                        HDisPoint = 0;

                        //GE20231219_加入前導課程折扣金額計算
                        if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text))
                        {
                            ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountC")).Text)).ToString();
                        }
                        else
                        {
                            ((Label)item.FindControl("LB_HDiscountC")).Text = "";
                        }

                        //221013-小計=輸入的付款金額
                        //((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text;

                        //GE202312219_修改小計計算
                        if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountC")).Text))
                        {
                            ((Label)item.FindControl("LB_HSubTotalC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text.Replace(",", string.Empty))).ToString();
                        }
                        else
                        {
                            ((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text;
                        }

                        LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)));//總計=小計
                                                                                                                                                             //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text));//總計=小計-前導課程減免總金額
                        if (Convert.ToInt32(LB_Total.Text.Replace(",", string.Empty)) < 0)
                        {
                            LB_Total.Text = "0";
                        }
                    }
                    QuerySelHDC.Close();

                }
                else
                {
                    //沒有輸入折扣碼時
                    //GE20231219_加入前導課程折扣金額計算
                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text))
                    {
                        ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountC")).Text)).ToString();
                    }
                    else
                    {
                        ((Label)item.FindControl("LB_HDiscountC")).Text = "";
                    }

                    //221013-小計=輸入的付款金額
                    //((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text;

                    //GE202312219_修改小計計算
                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountC")).Text))
                    {
                        ((Label)item.FindControl("LB_HSubTotalC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text.Replace(",", string.Empty))).ToString();
                    }
                    else
                    {
                        ((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text;
                    }

                    LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)));//總計=小計
                }
                #endregion

                //更新資料庫
                SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HAttend=@HAttend, HPoint=@HPoint, HPAmount=@HPAmount, HDPoint=@HDPoint, HSubTotal=@HSubTotal WHERE HID =@HID", SQLdatabase.OpenConnection());
                QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDC")).Text);
                QueryUpdSC.Parameters.AddWithValue("@HAttend", ((DropDownList)item.FindControl("DDL_HAttendC")).SelectedValue);
                QueryUpdSC.Parameters.AddWithValue("@HPoint", Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text) / 10);  //文化課程要轉換回點數
                QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountC")).Text);
                QueryUpdSC.Parameters.AddWithValue("@HDPoint", ((Label)item.FindControl("LB_HDPointC")).Text);
                QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalC")).Text);
                QueryUpdSC.ExecuteNonQuery();
                QueryUpdSC.Cancel();
                SQLdatabase.OpenConnection().Close();

            }
        }
        else
        {
            //GA20231218_加入判斷
            if (((Label)item.FindControl("LB_HBCPointC")).Text == "0" && ((DropDownList)item.FindControl("DDL_HAttendC")).SelectedValue != "5")
            {
                ((TextBox)item.FindControl("TB_HPAmountC")).Text = "";
                ((TextBox)item.FindControl("TB_HPAmountC")).Attributes.Add("PlaceHolder", "請自行輸入");
                ((Label)item.FindControl("LB_HSubTotalC")).Text = "";
                //GA20231218_顯示提示文字
                LB_HNotice.Visible = true;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('金額不能小於基本費用~');", true);
                ((TextBox)item.FindControl("TB_HPAmountC")).Text = ((Label)item.FindControl("LB_HBCPointC")).Text;
                ((TextBox)item.FindControl("TB_HDCodeC")).Text = null;
                ((Label)item.FindControl("LB_HDPointC")).Text = "0";
                //((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text;
                //GE20231219_修改小計金額計算
                if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text))
                {
                    ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountC")).Text)).ToString();
                }
                else
                {
                    ((Label)item.FindControl("LB_HDiscountC")).Text = "";
                }

                if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountC")).Text))
                {
                    ((Label)item.FindControl("LB_HSubTotalC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text)).ToString();
                }
                else
                {
                    ((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim();
                }

                return;
            }


        }
    }
    #endregion

    #region 文化檢覈課程-課程費用
    protected void TB_HExamFeeC_TextChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        RepeaterItem item = (sender as TextBox).NamingContainer as RepeaterItem;




    }
    #endregion


    #region 傳光(基金會)課程-付款金額切換
    protected void TB_HPAmountFC_TextChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        RepeaterItem item = (sender as TextBox).NamingContainer as RepeaterItem;

        //221204-加入判斷要選完上課地點
        if (((Label)item.FindControl("LB_CourseIDFC")).Text == "0")
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('選擇完地點後才能輸入付款金額喔~');", true);
            ((TextBox)item.FindControl("TB_HPAmountFC")).Text = ((Label)item.FindControl("LB_HBCPointFC")).Text;
            return;
        }


        if (!string.IsNullOrEmpty(((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim()))
        {
            //if (((DropDownList)item.FindControl("DDL_HAttendFC")).SelectedValue == "1" && (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text) < Convert.ToInt32(((Label)item.FindControl("LB_HBCPointFC")).Text)))

            //20240221_新增參班兼護持的判斷
            if ((((DropDownList)item.FindControl("DDL_HAttendFC")).SelectedValue == "1" || ((DropDownList)item.FindControl("DDL_HAttendFC")).SelectedValue == "6") && (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text) < Convert.ToInt32(((Label)item.FindControl("LB_HBCPointFC")).Text)))
            {
                //Response.Write("<script>alert('輸入金額不能小於基本費用喔');</script>");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice", "alert('輸入金額不能小於基本費用喔~!')", true);
                ((TextBox)item.FindControl("TB_HPAmountFC")).Text = ((Label)item.FindControl("LB_HBCPointFC")).Text;
                //((Label)item.FindControl("LB_HSubTotalFC")).Text = ((Label)item.FindControl("LB_HBCPointFC")).Text;

                if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountFC")).Text))
                {
                    ((Label)item.FindControl("LB_HSubTotalFC")).Text = Convert.ToDouble(Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountFC")).Text)).ToString();
                }
                else
                {
                    ((Label)item.FindControl("LB_HSubTotalFC")).Text = ((Label)item.FindControl("LB_HBCPointFC")).Text;
                }
                return;
            }
            else
            {
                //沒有輸入折扣碼時
                //GE20231219_加入小計金額計算
                if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text))
                {
                    ((Label)item.FindControl("LB_HSubTotalFC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountFC")).Text.Replace(",", string.Empty))).ToString();
                }
                else
                {
                    ((Label)item.FindControl("LB_HSubTotalFC")).Text = ((TextBox)item.FindControl("TB_HPAmountFC")).Text;

                }


                #region 折扣金額
                if (((Label)item.FindControl("LB_CourseIDFC")).Text != "0" && !string.IsNullOrEmpty(((TextBox)item.FindControl("TB_HDCodeFC")).Text.Trim()))
                {
                    SqlDataReader QuerySelHDC = SQLdatabase.ExecuteReader("SELECT HID, HDCode, HDSummary, HDPoint, HCourseID, HEmailGroup, HEmailPerson, HDType, HStatus FROM HDiscountCode WHERE HDCode='" + ((TextBox)item.FindControl("TB_HDCodeFC")).Text.Trim() + "' and HStatus='1'");
                    if (QuerySelHDC.Read())
                    {
                        string gHCourseIDCK = "0";  //判斷購買人員輸入的折扣碼有無在HCourseID裡
                        string gHEmailGroupCK = "0";  //判斷購買人員輸入的折扣碼有無在HEmailGroup裡
                        string gHEmailPersonCK = "0";  //判斷購買人員輸入的折扣碼有無在HEmailPerson裡

                        //判斷購買人員輸入的折扣碼有無在HCourseID裡
                        string[] gHCourseID = QuerySelHDC["HCourseID"].ToString().Split(',');
                        for (int i = 0; i < gHCourseID.Length; i++)
                        {
                            if (((Label)item.FindControl("LB_CourseIDFC")).Text == gHCourseID[i])
                            {
                                //LB_HDPoint.Text = QuerySelHDC["HDPoint"].ToString();
                                gHCourseIDCK = "1";
                            }
                        }

                        //判斷購買人員的折扣碼有無在HEmailGroup裡
                        string[] gHEmailGroup = QuerySelHDC["HEmailGroup"].ToString().Split(',');
                        for (int i = 0; i < gHEmailGroup.Length; i++)
                        {
                            SqlDataReader QuerySelHDCG = SQLdatabase.ExecuteReader("SELECT HID, HGNum, HGName, HMemberID, HGType, HStatus FROM HDCGroup WHERE HID='" + gHEmailGroup[i].ToString() + "' and HStatus='1'");
                            if (QuerySelHDCG.Read())
                            {
                                string[] gHMemberID = QuerySelHDCG["HMemberID"].ToString().Split(',');
                                for (int j = 0; j < gHMemberID.Length; j++)
                                {
                                    if (((Label)Master.FindControl("LB_HUserHID")).Text == gHMemberID[j])
                                    {
                                        //LB_HDPoint.Text = QuerySelHDCG["HDPoint"].ToString();
                                        gHEmailGroupCK = "1";
                                    }
                                }
                            }
                            QuerySelHDCG.Close();
                        }

                        //判斷購買人員輸入的折扣碼有無在HEmailPerson裡
                        string[] gHEmailPerson = QuerySelHDC["HEmailPerson"].ToString().Split(',');
                        for (int j = 0; j < gHEmailPerson.Length; j++)
                        {
                            if (((Label)Master.FindControl("LB_HUserHID")).Text == gHEmailPerson[j])
                            {
                                //LB_HDPoint.Text = QuerySelHDC["HDPoint"].ToString();
                                gHEmailPersonCK = "1";
                            }

                        }




                        if (gHCourseIDCK == "1" && (gHEmailGroupCK == "1" || gHEmailPersonCK == "1"))
                        {
                            //折扣碼金額
                            ((Label)item.FindControl("LB_HDPointFC")).Text = (Convert.ToInt32(QuerySelHDC["HDPoint"].ToString()) * 10).ToString();

                            //GE20231219_總折扣金額計算加入前導課程折扣金額
                            if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text) && !string.IsNullOrEmpty(QuerySelHDC["HDPoint"].ToString()))
                            {
                                ((Label)item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(QuerySelHDC["HDPoint"].ToString()) * 10 + Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountFC")).Text)).ToString();
                            }
                            else if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text) && string.IsNullOrEmpty(QuerySelHDC["HDPoint"].ToString()))
                            {
                                ((Label)item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountFC")).Text)).ToString();
                            }
                            else if (string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text) && !string.IsNullOrEmpty(QuerySelHDC["HDPoint"].ToString()))
                            {
                                ((Label)item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(QuerySelHDC["HDPoint"].ToString()) * 10).ToString();
                            }
                            else
                            {
                                ((Label)item.FindControl("LB_HDiscountFC")).Text = "";
                            }

                            //((Label)item.FindControl("LB_HDPointFC")).Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HDPointFC")).Text));
                            //折扣總金額
                            ((Label)item.FindControl("LB_HDiscountFC")).Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HDiscountFC")).Text));

                            //((Label)item.FindControl("LB_HSubTotalFC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDPointFC")).Text.Replace(",", string.Empty))).ToString();
                            //GE20231219_改成扣折扣總金額
                            ((Label)item.FindControl("LB_HSubTotalFC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountFC")).Text.Replace(",", string.Empty))).ToString();

                            if (Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalFC")).Text) < 0)
                            {
                                ((Label)item.FindControl("LB_HSubTotalFC")).Text = "0";
                            }

                            HDisPoint = Convert.ToInt32(((Label)item.FindControl("LB_HDPointFC")).Text.Replace(",", string.Empty));//折扣碼的折扣金額
                                                                                                                                   //<待確認>231219
                                                                                                                                   //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty)) - HDisPoint);//總計=小計-折扣碼金額

                            //GE20231219
                            LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty)));
                            //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text) - HDisPoint);//總計=小計-前導課程減免總金額-折扣碼金額

                            if (Convert.ToInt32(LB_Total.Text.Replace(",", string.Empty)) < 0)
                            {
                                LB_Total.Text = "0";
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('折扣碼錯誤，請重新輸入。');", true);
                            ((TextBox)item.FindControl("TB_HDCodeFC")).Text = "";
                            ((TextBox)item.FindControl("TB_HDCodeFC")).Focus();
                            ((Label)item.FindControl("LB_HDPointFC")).Text = "0";
                            HDisPoint = 0;

                            //GE20231219_加入前導課程折扣金額計算
                            if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text))
                            {
                                ((Label)item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountFC")).Text)).ToString();
                            }
                            else
                            {
                                ((Label)item.FindControl("LB_HDiscountFC")).Text = "";
                            }




                            //221013-小計=輸入的付款金額
                            //((Label)item.FindControl("LB_HSubTotalFC")).Text = ((TextBox)item.FindControl("TB_HPAmountFC")).Text;
                            //GE202312219_修改小計計算
                            if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountFC")).Text))
                            {
                                ((Label)item.FindControl("LB_HSubTotalFC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountFC")).Text.Replace(",", string.Empty))).ToString();
                            }
                            else
                            {
                                ((Label)item.FindControl("LB_HSubTotalFC")).Text = ((TextBox)item.FindControl("TB_HPAmountFC")).Text;
                            }


                            LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty)));//總計=小計
                                                                                                                                                                  //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text));//總計=小計-前導課程減免總金額

                            if (Convert.ToInt32(LB_Total.Text.Replace(",", string.Empty)) < 0)
                            {
                                LB_Total.Text = "0";
                            }
                        }


                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('折扣碼錯誤，請重新輸入。');", true);
                        ((TextBox)item.FindControl("TB_HDCodeFC")).Text = "";
                        ((TextBox)item.FindControl("TB_HDCodeFC")).Focus();
                        ((Label)item.FindControl("LB_HDPointFC")).Text = "0";
                        HDisPoint = 0;

                        //GE20231219_加入前導課程折扣金額計算
                        if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text))
                        {
                            ((Label)item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountFC")).Text)).ToString();
                        }
                        else
                        {
                            ((Label)item.FindControl("LB_HDiscountFC")).Text = "";
                        }

                        //221013-小計=輸入的付款金額
                        //GE20231219_小計=輸入的付款金額-折扣碼折扣金額-前導課程折扣金額
                        if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountFC")).Text))
                        {
                            ((Label)item.FindControl("LB_HSubTotalFC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim()) - Convert.ToInt32(((TextBox)item.FindControl("LB_HDiscountFC")).Text.Trim())).ToString();
                        }
                        else
                        {
                            ((Label)item.FindControl("LB_HSubTotalFC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim())).ToString();
                        }



                        LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty)));//總計=小計
                                                                                                                                                              //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text));//總計=小計-前導課程減免總金額
                        if (Convert.ToInt32(LB_Total.Text.Replace(",", string.Empty)) < 0)
                        {
                            LB_Total.Text = "0";
                        }
                    }
                    QuerySelHDC.Close();
                }
                #endregion


                //更新資料庫
                SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HAttend=@HAttend, HPoint=@HPoint, HPAmount=@HPAmount, HDPoint=@HDPoint, HSubTotal=@HSubTotal WHERE HID =@HID", SQLdatabase.OpenConnection());
                QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDFC")).Text);
                QueryUpdSC.Parameters.AddWithValue("@HAttend", ((DropDownList)item.FindControl("DDL_HAttendFC")).SelectedValue);
                QueryUpdSC.Parameters.AddWithValue("@HPoint", ((Label)item.FindControl("LB_HSubTotalFC")).Text);  //傳光(基金會)課程直接存金額
                QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountFC")).Text);
                QueryUpdSC.Parameters.AddWithValue("@HDPoint", ((Label)item.FindControl("LB_HDPointFC")).Text);//折扣碼折扣金額
                QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalFC")).Text);
                QueryUpdSC.ExecuteNonQuery();
                QueryUpdSC.Cancel();
                SQLdatabase.OpenConnection().Close();

            }
        }
        else
        {
            //GA20231218_加入判斷
            if (((Label)item.FindControl("LB_HBCPointFC")).Text == "0" && ((DropDownList)item.FindControl("DDL_HAttendFC")).SelectedValue != "5")
            {
                ((TextBox)item.FindControl("TB_HPAmountFC")).Text = "";
                ((TextBox)item.FindControl("TB_HPAmountFC")).Attributes.Add("PlaceHolder", "請自行輸入");
                ((Label)item.FindControl("LB_HSubTotalFC")).Text = "";
                //GA20231218_顯示提示文字
                LB_HNotice.Visible = true;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('金額不能小於基本費用~');", true);
                ((TextBox)item.FindControl("TB_HPAmountFC")).Text = ((Label)item.FindControl("LB_HBCPointFC")).Text;
                ((TextBox)item.FindControl("TB_HDCodeFC")).Text = null;
                ((Label)item.FindControl("LB_HDPointFC")).Text = "0";

                //GA20231219_加入折扣總金額計算
                if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text))
                {
                    ((Label)item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountFC")).Text)).ToString();
                }
                else
                {
                    ((Label)item.FindControl("LB_HDiscountFC")).Text = "";
                }

                //GE20231219_修改小計金額計算
                if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountFC")).Text))
                {
                    ((Label)item.FindControl("LB_HSubTotalFC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountFC")).Text)).ToString();
                }
                else
                {
                    ((Label)item.FindControl("LB_HSubTotalFC")).Text = ((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim();
                }

                return;
            }

        }



    }
    #endregion

    #region 傳光檢覈課程-課程費用
    protected void TB_HExamFeeFC_TextChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        RepeaterItem item = (sender as TextBox).NamingContainer as RepeaterItem;




    }
    #endregion


    #region 課程捐款-課程費用切換
    protected void TB_HCourseDonateFD_TextChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        RepeaterItem item = (sender as TextBox).NamingContainer as RepeaterItem;

        ((Label)item.FindControl("LB_SubTotalFD")).Text = ((TextBox)item.FindControl("TB_PAmountFD")).Text;
        //221015- -改為捐款金額最低為100元

        //if (Convert.ToInt32(((TextBox)item.FindControl("TB_PAmountFD")).Text) <= 0)
        if (Convert.ToInt32(((TextBox)item.FindControl("TB_PAmountFD")).Text) < 100)
        {
            //Response.Write("<script>alert('捐款金額最低不能小於100哦~!');</script>");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice2", "alert('捐款金額最低不能小於100哦~!')", true);
            ((TextBox)item.FindControl("TB_PAmountFD")).Text = ((Label)item.FindControl("LB_HBCPointFD")).Text;
            ((Label)item.FindControl("LB_SubTotalFD")).Text = ((TextBox)item.FindControl("TB_PAmountFD")).Text;
            return;
        }
        else
        {
            //更新資料庫
            SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HPoint=@HPoint, HPAmount=@HPAmount, HSubTotal=@HSubTotal  WHERE HID =@HID", SQLdatabase.OpenConnection());
            QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDFD")).Text);
            QueryUpdSC.Parameters.AddWithValue("@HPoint", ((TextBox)item.FindControl("TB_PAmountFD")).Text.Trim());  //捐款直接存金額
            QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_PAmountFD")).Text.Trim());
            QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((TextBox)item.FindControl("TB_PAmountFD")).Text.Trim());
            QueryUpdSC.ExecuteNonQuery();
            QueryUpdSC.Cancel();
            SQLdatabase.OpenConnection().Close();

        }


    }
    #endregion

    #region 文化課程-折扣碼金額切換
    protected void TB_HDCodeC_TextChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        RepeaterItem item = (sender as TextBox).NamingContainer as RepeaterItem;

        if (((Label)item.FindControl("LB_CourseIDC")).Text == "0")
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('請先選擇完上課地點再輸入付款金額喔~');", true);
            ((TextBox)item.FindControl("TB_HDCodeC")).Text = "";
            return;
        }



        if (!string.IsNullOrEmpty(((TextBox)item.FindControl("TB_HDCodeC")).Text.Trim()))
        {
            //GE20241202_新增欄位HCTemplateID,HUseOnce, HSDate, HEDate,
            SqlDataReader QuerySelHDC = SQLdatabase.ExecuteReader("SELECT HID, HDCode, HDSummary, HDPoint, HCourseID, HCTemplateID,HEmailGroup, HEmailPerson, HDType,HUseOnce, HSDate, HEDate, HStatus FROM HDiscountCode WHERE HDCode='" + ((TextBox)item.FindControl("TB_HDCodeC")).Text.Trim() + "' and HStatus='1'");

            if (QuerySelHDC.Read())
            {
                string gHCourseIDCK = "0";  //判斷購買人員輸入的折扣碼有無在HCourseID裡
                string gHEmailGroupCK = "0";  //判斷購買人員輸入的折扣碼有無在HEmailGroup裡
                string gHEmailPersonCK = "0";  //判斷購買人員輸入的折扣碼有無在HEmailPerson裡
                //GA20241202_新增判斷
                string gHCTemplateIDCK = "0";  //判斷購買人員輸入的折扣碼有無在HCTemplateID裡
                string gHDCodeUse = "0";  //判斷購買人是否已使用過折扣碼

                //判斷購買人員輸入的折扣碼有無在HCourseID裡
                string[] gHCourseID = QuerySelHDC["HCourseID"].ToString().Split(',');
                for (int i = 0; i < gHCourseID.Length; i++)
                {
                    if (((Label)item.FindControl("LB_CourseIDC")).Text == gHCourseID[i])
                    {
                        //LB_HDPoint.Text = QuerySelHDC["HDPoint"].ToString();
                        gHCourseIDCK = "1";
                    }
                }

                //GA20241202
                //判斷購買人員輸入的折扣碼有無在HCTemplateID裡
                string[] gHCTemplateID = QuerySelHDC["HCTemplateID"].ToString().Split(',');
                for (int i = 0; i < gHCTemplateID.Length; i++)
                {
                    if (((Label)item.FindControl("LB_CTemplateIDC")).Text == gHCTemplateID[i])
                    {
                        gHCTemplateIDCK = "1";
                    }
                }

                //判斷購買人員的折扣碼有無在HEmailGroup裡
                string[] gHEmailGroup = QuerySelHDC["HEmailGroup"].ToString().Split(',');
                for (int i = 0; i < gHEmailGroup.Length; i++)
                {
                    SqlDataReader QuerySelHDCG = SQLdatabase.ExecuteReader("SELECT HID, HGNum, HGName, HMemberID, HGType, HStatus FROM HDCGroup WHERE HID='" + gHEmailGroup[i].ToString() + "' and HStatus='1'");
                    if (QuerySelHDCG.Read())
                    {
                        string[] gHMemberID = QuerySelHDCG["HMemberID"].ToString().Split(',');
                        for (int j = 0; j < gHMemberID.Length; j++)
                        {
                            if (((Label)Master.FindControl("LB_HUserHID")).Text == gHMemberID[j])
                            {
                                //LB_HDPoint.Text = QuerySelHDCG["HDPoint"].ToString();
                                gHEmailGroupCK = "1";
                            }
                        }
                    }
                    QuerySelHDCG.Close();
                }

                //判斷購買人員輸入的折扣碼有無在HEmailPerson裡
                string[] gHEmailPerson = QuerySelHDC["HEmailPerson"].ToString().Split(',');
                for (int j = 0; j < gHEmailPerson.Length; j++)
                {
                    if (((Label)Master.FindControl("LB_HUserHID")).Text == gHEmailPerson[j])
                    {
                        //LB_HDPoint.Text = QuerySelHDC["HDPoint"].ToString();
                        gHEmailPersonCK = "1";
                    }

                }


                //GA20241202_判斷若折扣碼只能使用一次，則當購買人有在名單中，則要再判斷是否已使用折扣碼
                if (QuerySelHDC["HUseOnce"].ToString() == "True")
                {
                    if (gHEmailGroupCK == "1" || gHEmailPersonCK == "1")
                    {
                        //訂單
                        SqlDataReader QueryHDCodeUse = SQLdatabase.ExecuteReader("SELECT HID FROM HCourseBooking WHERE HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND HDCode='" + QuerySelHDC["HDCode"].ToString() + "' AND HStatus=1 AND HItemStatus=1");

                        if (QueryHDCodeUse.HasRows)
                        {
                            gHDCodeUse = "1";
                        }
                        QueryHDCodeUse.Close();

                        //購物車
                        SqlDataReader QueryHDCodeUseShopping = SQLdatabase.ExecuteReader("SELECT HID FROM HShoppingCart WHERE HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND HDCode='" + QuerySelHDC["HDCode"].ToString() + "'");

                        if (QueryHDCodeUseShopping.HasRows)
                        {
                            gHDCodeUse = "1";
                        }
                        QueryHDCodeUseShopping.Close();
                    }
                }


                //GE20241202_新增課程範本判斷&沒使用過折扣碼
                if ((gHCourseIDCK == "1" || gHCTemplateIDCK == "1") && (gHEmailGroupCK == "1" || gHEmailPersonCK == "1") && gHDCodeUse == "0")
                {

                    ((Label)item.FindControl("LB_HDPointC")).Text = (Convert.ToInt32(QuerySelHDC["HDPoint"].ToString()) * 10).ToString(); //折扣點數取出需轉成金額

                    //GE20231219_折扣金額計算加入前導課程折扣金額
                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text) && !string.IsNullOrEmpty(QuerySelHDC["HDPoint"].ToString()))
                    {
                        ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(QuerySelHDC["HDPoint"].ToString()) * 10 + Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountC")).Text)).ToString();
                    }
                    else if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text) && string.IsNullOrEmpty(QuerySelHDC["HDPoint"].ToString()))
                    {
                        ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountC")).Text)).ToString();
                    }
                    else if (string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text) && !string.IsNullOrEmpty(QuerySelHDC["HDPoint"].ToString()))
                    {
                        ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(QuerySelHDC["HDPoint"].ToString()) * 10).ToString();
                    }
                    else
                    {
                        ((Label)item.FindControl("LB_HDiscountC")).Text = "";
                    }




                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountC")).Text))
                    {
                        //折扣總金額(轉千分位)
                        ((Label)item.FindControl("LB_HDiscountC")).Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text));

                        if (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text.Replace(",", string.Empty)) < 0)
                        {
                            ((Label)item.FindControl("LB_HSubTotalC")).Text = "0";
                        }
                        else
                        {
                            ((Label)item.FindControl("LB_HSubTotalC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text.Replace(",", string.Empty))).ToString();
                        }
                    }
                    else
                    {
                        ((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim();
                    }

                    HDisPoint = Convert.ToInt32(((Label)item.FindControl("LB_HDPointC")).Text.Replace(",", string.Empty));
                    //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)) - HDisPoint);//總計=小計-折扣碼金額
                    //GE20231219
                    LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)));//總計=小計-折扣碼金額

                    //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text) - HDisPoint);//總計=小計-前導課程減免總金額-折扣碼金額

                    if (Convert.ToInt32(LB_Total.Text.Replace(",", string.Empty)) < 0)
                    {
                        LB_Total.Text = "0";
                    }

                    #region 更新折扣碼&折扣金額&折扣後金額
                    SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect, HPoint = @HPoint, HDCode=@HDCode, HDPoint=@HDPoint, HPAmount=@HPAmount, HSubTotal=@HSubTotal WHERE HID =@HID", SQLdatabase.OpenConnection());
                    QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDC")).Text);
                    QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectC")).Checked);
                    QueryUpdSC.Parameters.AddWithValue("@HPoint", Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)) / 10);   //文化課程要轉成點數存入(折扣完的金額)

                    QueryUpdSC.Parameters.AddWithValue("@HDCode", ((TextBox)item.FindControl("TB_HDCodeC")).Text.Trim());
                    QueryUpdSC.Parameters.AddWithValue("@HDPoint", HDisPoint / 10);  //折扣金額存入DB需轉成點數
                    QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim());
                    QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalC")).Text);
                    QueryUpdSC.ExecuteNonQuery();
                    QueryUpdSC.Cancel();
                    SQLdatabase.OpenConnection().Close();
                    #endregion
                }
                else if (gHDCodeUse == "1")
                {
                    ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('您已使用過此折扣碼，此折扣碼僅能使用一次哦~');", true);
                    ((TextBox)item.FindControl("TB_HDCodeC")).Text = "";
                    ((TextBox)item.FindControl("TB_HDCodeC")).Focus();
                    ((Label)item.FindControl("LB_HDPointC")).Text = "0";
                    HDisPoint = 0;

                    //GE20231219_折扣金額計算加入前導課程折扣金額
                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text))
                    {
                        ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountC")).Text)).ToString();
                    }
                    else
                    {
                        ((Label)item.FindControl("LB_HDiscountC")).Text = "";
                    }

                    //GA20231219-小計=輸入的付款金額-折扣總金額
                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountC")).Text))
                    {
                        ((Label)item.FindControl("LB_HSubTotalC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text)).ToString();
                    }
                    else
                    {
                        ((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text;
                    }

                    LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)));//總計=小計
                                                                                                                                                         //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text));//總計=小計-前導課程減免總金額

                    if (Convert.ToInt32(LB_Total.Text.Replace(",", string.Empty)) < 0)
                    {
                        LB_Total.Text = "0";
                    }


                    #region 230107-ADD 更新折扣碼&折扣金額&折扣後金額
                    SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect, HPoint = @HPoint, HDCode=@HDCode, HDPoint=@HDPoint, HPAmount=@HPAmount, HSubTotal=@HSubTotal WHERE HID =@HID", SQLdatabase.OpenConnection());
                    QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDC")).Text);
                    QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectC")).Checked);
                    QueryUpdSC.Parameters.AddWithValue("@HPoint", Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)) / 10);   //文化課程要轉成點數存入(折扣完的金額)

                    QueryUpdSC.Parameters.AddWithValue("@HDCode", "");  //更新為空值
                    QueryUpdSC.Parameters.AddWithValue("@HDPoint", "");  //更新為空值
                    QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim());
                    QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty));
                    QueryUpdSC.ExecuteNonQuery();
                    QueryUpdSC.Cancel();
                    SQLdatabase.OpenConnection().Close();
                    #endregion



                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('折扣碼錯誤，請重新輸入~');", true);
                    ((TextBox)item.FindControl("TB_HDCodeC")).Text = "";
                    ((TextBox)item.FindControl("TB_HDCodeC")).Focus();
                    ((Label)item.FindControl("LB_HDPointC")).Text = "0";
                    HDisPoint = 0;

                    //GE20231219_折扣金額計算加入前導課程折扣金額
                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text))
                    {
                        ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountC")).Text)).ToString();
                    }
                    else
                    {
                        ((Label)item.FindControl("LB_HDiscountC")).Text = "";
                    }

                    //GA20231219-小計=輸入的付款金額-折扣總金額
                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountC")).Text))
                    {
                        ((Label)item.FindControl("LB_HSubTotalC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text)).ToString();
                    }
                    else
                    {
                        ((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text;
                    }

                    LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)));//總計=小計
                                                                                                                                                         //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text));//總計=小計-前導課程減免總金額

                    if (Convert.ToInt32(LB_Total.Text.Replace(",", string.Empty)) < 0)
                    {
                        LB_Total.Text = "0";
                    }


                    #region 230107-ADD 更新折扣碼&折扣金額&折扣後金額
                    SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect, HPoint = @HPoint, HDCode=@HDCode, HDPoint=@HDPoint, HPAmount=@HPAmount, HSubTotal=@HSubTotal WHERE HID =@HID", SQLdatabase.OpenConnection());
                    QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDC")).Text);
                    QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectC")).Checked);
                    QueryUpdSC.Parameters.AddWithValue("@HPoint", Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)) / 10);   //文化課程要轉成點數存入(折扣完的金額)

                    QueryUpdSC.Parameters.AddWithValue("@HDCode", "");  //更新為空值
                    QueryUpdSC.Parameters.AddWithValue("@HDPoint", "");  //更新為空值
                    QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim());
                    QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty));
                    QueryUpdSC.ExecuteNonQuery();
                    QueryUpdSC.Cancel();
                    SQLdatabase.OpenConnection().Close();
                    #endregion
                }

            }
            else
            {
                ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('折扣碼錯誤，請重新輸入。');", true);
                ((TextBox)item.FindControl("TB_HDCodeC")).Text = "";
                ((TextBox)item.FindControl("TB_HDCodeC")).Focus();
                ((Label)item.FindControl("LB_HDPointC")).Text = "0";
                HDisPoint = 0;

                //GE20231219_折扣金額計算加入前導課程折扣金額
                if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text))
                {
                    ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountC")).Text)).ToString();
                }
                else
                {
                    ((Label)item.FindControl("LB_HDiscountC")).Text = "";
                }

                //GE20231219_計算小計 =輸入的付款金額-折扣總金額
                if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountC")).Text))
                {
                    ((Label)item.FindControl("LB_HSubTotalC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text)).ToString();
                }
                else
                {
                    ((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text;
                }


                LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)));//總計=小計
                                                                                                                                                     //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text));//總計=小計-前導課程減免總金額
                if (Convert.ToInt32(LB_Total.Text.Replace(",", string.Empty)) < 0)
                {
                    LB_Total.Text = "0";
                }



                #region 230107-ADD 更新折扣碼&折扣金額&折扣後金額
                SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect, HPoint = @HPoint, HDCode=@HDCode, HDPoint=@HDPoint, HPAmount=@HPAmount, HSubTotal=@HSubTotal WHERE HID =@HID", SQLdatabase.OpenConnection());
                QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDC")).Text);
                QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectC")).Checked);
                QueryUpdSC.Parameters.AddWithValue("@HPoint", Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)) / 10);   //文化課程要轉成點數存入(折扣完的金額)

                QueryUpdSC.Parameters.AddWithValue("@HDCode", "");  //更新為空值
                QueryUpdSC.Parameters.AddWithValue("@HDPoint", "");  //更新為空值
                QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim());
                QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty));
                QueryUpdSC.ExecuteNonQuery();
                QueryUpdSC.Cancel();
                SQLdatabase.OpenConnection().Close();
                #endregion


            }
            QuerySelHDC.Close();
        }
        else
        {
            //沒有輸入折扣碼時
            ((Label)item.FindControl("LB_HDPointC")).Text = "0";

            //GE20231219_折扣金額計算加入前導課程折扣金額
            if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountC")).Text))
            {
                ((Label)item.FindControl("LB_HDiscountC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountC")).Text)).ToString();
            }
            else
            {
                ((Label)item.FindControl("LB_HDiscountC")).Text = "";
            }

            //	((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text;

            //GE20231219_修改小計=輸入總金額-折扣總金額
            if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountC")).Text))
            {
                ((Label)item.FindControl("LB_HSubTotalC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountC")).Text) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountC")).Text)).ToString();
            }
            else
            {
                ((Label)item.FindControl("LB_HSubTotalC")).Text = ((TextBox)item.FindControl("TB_HPAmountC")).Text;
            }


            LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)));//總計=小計


            #region 230107-ADD 更新折扣碼&折扣金額&折扣後金額
            SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect, HPoint = @HPoint, HDCode=@HDCode, HDPoint=@HDPoint, HPAmount=@HPAmount, HSubTotal=@HSubTotal WHERE HID =@HID", SQLdatabase.OpenConnection());
            QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDC")).Text);
            QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectC")).Checked);
            QueryUpdSC.Parameters.AddWithValue("@HPoint", Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty)) / 10);   //文化課程要轉成點數存入(折扣完的金額)

            QueryUpdSC.Parameters.AddWithValue("@HDCode", "");  //更新為空值
            QueryUpdSC.Parameters.AddWithValue("@HDPoint", "");  //更新為空值
            QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountC")).Text.Trim());
            QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalC")).Text.Replace(",", string.Empty));
            QueryUpdSC.ExecuteNonQuery();
            QueryUpdSC.Cancel();
            SQLdatabase.OpenConnection().Close();
            #endregion

        }

    }
    #endregion


    #region 傳光(基金會)課程-折扣碼金額切換
    protected void TB_HDCodeFC_TextChanged(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        RepeaterItem item = (sender as TextBox).NamingContainer as RepeaterItem;

        if (((Label)item.FindControl("LB_CourseIDFC")).Text == "0")
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('選擇完地點後才能輸入折扣碼喔。');", true);
            ((TextBox)item.FindControl("TB_HDCodeFC")).Text = "";

            return;
        }

        if (!string.IsNullOrEmpty(((TextBox)item.FindControl("TB_HDCodeFC")).Text.Trim()))
        {

            //GE20241202_新增欄位HCTemplateID,HUseOnce, HSDate, HEDate,
            SqlDataReader QuerySelHDC = SQLdatabase.ExecuteReader("SELECT HID, HDCode, HDSummary, HDPoint, HCourseID, HCTemplateID,HEmailGroup, HEmailPerson, HDType,HUseOnce, HSDate, HEDate, HStatus FROM HDiscountCode WHERE HDCode='" + ((TextBox)item.FindControl("TB_HDCodeFC")).Text + "' and HStatus='1'");
            if (QuerySelHDC.Read())
            {
                string gHCourseIDCK = "0";  //判斷購買人員輸入的折扣碼有無在HCourseID裡
                string gHEmailGroupCK = "0";  //判斷購買人員輸入的折扣碼有無在HEmailGroup裡
                string gHEmailPersonCK = "0";  //判斷購買人員輸入的折扣碼有無在HEmailPerson裡
                //GA20241202_新增判斷
                string gHCTemplateIDCK = "0";  //判斷購買人員輸入的折扣碼有無在HCTemplateID裡
                string gHDCodeUse = "0";  //判斷購買人是否已使用過折扣碼

                //判斷購買人員輸入的折扣碼有無在HCourseID裡
                string[] gHCourseID = QuerySelHDC["HCourseID"].ToString().Split(',');
                for (int i = 0; i < gHCourseID.Length; i++)
                {
                    if (((Label)item.FindControl("LB_CourseIDFC")).Text == gHCourseID[i])
                    {
                        //LB_HDPoint.Text = QuerySelHDC["HDPoint"].ToString();
                        gHCourseIDCK = "1";
                    }
                }

                //GA20241202
                //判斷購買人員輸入的折扣碼有無在HCTemplateID裡
                string[] gHCTemplateID = QuerySelHDC["HCTemplateID"].ToString().Split(',');
                for (int i = 0; i < gHCTemplateID.Length; i++)
                {
                    if (((Label)item.FindControl("LB_CTemplateIDFC")).Text == gHCTemplateID[i])
                    {
                        gHCTemplateIDCK = "1";
                    }
                }

                //判斷購買人員的折扣碼有無在HEmailGroup裡
                string[] gHEmailGroup = QuerySelHDC["HEmailGroup"].ToString().Split(',');
                for (int i = 0; i < gHEmailGroup.Length; i++)
                {
                    SqlDataReader QuerySelHDCG = SQLdatabase.ExecuteReader("SELECT HID, HGNum, HGName, HMemberID, HGType, HStatus FROM HDCGroup WHERE HID='" + gHEmailGroup[i].ToString() + "' and HStatus='1'");
                    if (QuerySelHDCG.Read())
                    {
                        string[] gHMemberID = QuerySelHDCG["HMemberID"].ToString().Split(',');
                        for (int j = 0; j < gHMemberID.Length; j++)
                        {
                            if (((Label)Master.FindControl("LB_HUserHID")).Text == gHMemberID[j])
                            {
                                //LB_HDPoint.Text = QuerySelHDCG["HDPoint"].ToString();
                                gHEmailGroupCK = "1";
                            }
                        }
                    }
                    QuerySelHDCG.Close();
                }

                //判斷購買人員輸入的折扣碼有無在HEmailPerson裡
                string[] gHEmailPerson = QuerySelHDC["HEmailPerson"].ToString().Split(',');
                for (int j = 0; j < gHEmailPerson.Length; j++)
                {
                    if (((Label)Master.FindControl("LB_HUserHID")).Text == gHEmailPerson[j])
                    {
                        //LB_HDPoint.Text = QuerySelHDC["HDPoint"].ToString();
                        gHEmailPersonCK = "1";
                    }

                }


                //GA20241202_判斷若折扣碼只能使用一次，則當購買人有在名單中，則要再判斷是否已使用折扣碼
                if (QuerySelHDC["HUseOnce"].ToString() == "True")
                {
                    if (gHEmailGroupCK == "1" || gHEmailPersonCK == "1")
                    {
                        //訂單
                        SqlDataReader QueryHDCodeUseBooking = SQLdatabase.ExecuteReader("SELECT HID FROM HCourseBooking WHERE HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND HDCode='" + QuerySelHDC["HDCode"].ToString() + "' AND HStatus=1 AND HItemStatus=1");

                        if (QueryHDCodeUseBooking.HasRows)
                        {
                            gHDCodeUse = "1";
                        }
                        QueryHDCodeUseBooking.Close();

                        //購物車
                        SqlDataReader QueryHDCodeUseShopping = SQLdatabase.ExecuteReader("SELECT HID FROM HShoppingCart WHERE HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND HDCode='" + QuerySelHDC["HDCode"].ToString() + "'");

                        if (QueryHDCodeUseShopping.HasRows)
                        {
                            gHDCodeUse = "1";
                        }
                        QueryHDCodeUseShopping.Close();
                    }
                }







                //GE20241202_新增課程範本判斷&沒使用過折扣碼
                if ((gHCourseIDCK == "1" || gHCTemplateIDCK == "1") && (gHEmailGroupCK == "1" || gHEmailPersonCK == "1") && gHDCodeUse == "0")
                {
                    //折扣碼折扣金額
                    ((Label)item.FindControl("LB_HDPointFC")).Text = (Convert.ToInt32(QuerySelHDC["HDPoint"].ToString()) * 10).ToString();  //折扣點數取出需轉成金額   

                    //GE20231219_折扣金額計算加入前導課程折扣金額
                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text) && !string.IsNullOrEmpty(QuerySelHDC["HDPoint"].ToString()))
                    {
                        ((Label)item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(QuerySelHDC["HDPoint"].ToString()) * 10 + Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountFC")).Text)).ToString();
                    }
                    else if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text) && string.IsNullOrEmpty(QuerySelHDC["HDPoint"].ToString()))
                    {
                        ((Label)item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountFC")).Text)).ToString();
                    }
                    else if (string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text) && !string.IsNullOrEmpty(QuerySelHDC["HDPoint"].ToString()))
                    {
                        ((Label)item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(QuerySelHDC["HDPoint"].ToString()) * 10).ToString();
                    }
                    else
                    {
                        ((Label)item.FindControl("LB_HDiscountFC")).Text = "";
                    }


                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountFC")).Text))
                    {
                        if (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountFC")).Text) < 0)
                        {
                            ((Label)item.FindControl("LB_HSubTotalFC")).Text = "0";
                        }
                        else
                        {
                            ((Label)item.FindControl("LB_HSubTotalFC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim()) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountFC")).Text.Replace(",", string.Empty))).ToString();
                        }
                    }
                    else
                    {
                        ((Label)item.FindControl("LB_HSubTotalFC")).Text = ((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim();
                    }


                    HDisPoint = Convert.ToInt32(((Label)item.FindControl("LB_HDPointFC")).Text.Replace(",", string.Empty));
                    //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty)) - HDisPoint);//總計=小計-折扣碼金額
                    //GE20231219
                    LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty)));//總計=小計-折扣碼金額
                                                                                                                                                          //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text) - HDisPoint);//總計=小計-前導課程減免總金額-折扣碼金額

                    if (Convert.ToInt32(LB_Total.Text.Replace(",", string.Empty)) < 0)
                    {
                        LB_Total.Text = "0";
                    }

                    #region 更新折扣碼&折扣金額&折扣後金額
                    SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect, HPoint = @HPoint, HDCode=@HDCode, HDPoint=@HDPoint, HPAmount=@HPAmount, HSubTotal=@HSubTotal WHERE HID =@HID", SQLdatabase.OpenConnection());
                    QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDFC")).Text);
                    QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectFC")).Checked);
                    QueryUpdSC.Parameters.AddWithValue("@HPoint", ((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty));  //基金會課程直接存金額
                    QueryUpdSC.Parameters.AddWithValue("@HDCode", ((TextBox)item.FindControl("TB_HDCodeFC")).Text.Trim());
                    QueryUpdSC.Parameters.AddWithValue("@HDPoint", HDisPoint / 10);  //折扣金額存入DB需轉成點數
                    QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim());
                    QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty));
                    QueryUpdSC.ExecuteNonQuery();
                    QueryUpdSC.Cancel();
                    SQLdatabase.OpenConnection().Close();
                    #endregion
                }
                else if (gHDCodeUse == "1")
                {
                    ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('您已使用過此折扣碼，此折扣碼僅能使用一次哦~');", true);
                    ((TextBox)item.FindControl("TB_HDCodeFC")).Text = "";
                    ((TextBox)item.FindControl("TB_HDCodeFC")).Focus();
                    ((Label)item.FindControl("LB_HDPointFC")).Text = "0";
                    HDisPoint = 0;

                    //GE20231219_加入前導課程折扣金額計算
                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text))
                    {
                        ((Label)item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountFC")).Text)).ToString();
                    }
                    else
                    {
                        ((Label)item.FindControl("LB_HDiscountFC")).Text = "";
                    }




                    //GA20231219-小計=輸入的付款金額-折扣總金額
                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountFC")).Text))
                    {
                        ((Label)item.FindControl("LB_HSubTotalFC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountFC")).Text)).ToString();
                    }
                    else
                    {
                        ((Label)item.FindControl("LB_HSubTotalFC")).Text = ((TextBox)item.FindControl("TB_HPAmountFC")).Text;
                    }


                    LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty)));//總計=小計
                                                                                                                                                          //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text));//總計=小計-前導課程減免總金額

                    if (Convert.ToInt32(LB_Total.Text.Replace(",", string.Empty)) < 0)
                    {
                        LB_Total.Text = "0";
                    }


                    #region 
                    //GA241202-更新折扣碼&折扣金額&折扣後金額
                    SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect, HPoint = @HPoint, HDCode=@HDCode, HDPoint=@HDPoint, HPAmount=@HPAmount, HSubTotal=@HSubTotal WHERE HID =@HID", SQLdatabase.OpenConnection());
                    QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDFC")).Text);
                    QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectFC")).Checked);
                    QueryUpdSC.Parameters.AddWithValue("@HPoint", ((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty));  //基金會課程直接存金額
                    QueryUpdSC.Parameters.AddWithValue("@HDCode", ""); //更新為空值
                    QueryUpdSC.Parameters.AddWithValue("@HDPoint", "");  //更新為空值
                    QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim());
                    QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalFC")).Text);
                    QueryUpdSC.ExecuteNonQuery();
                    QueryUpdSC.Cancel();
                    SQLdatabase.OpenConnection().Close();
                    #endregion


                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('折扣碼錯誤，請重新輸入。');", true);
                    ((TextBox)item.FindControl("TB_HDCodeFC")).Text = "";
                    ((TextBox)item.FindControl("TB_HDCodeFC")).Focus();
                    ((Label)item.FindControl("LB_HDPointFC")).Text = "0";
                    HDisPoint = 0;

                    //GE20231219_加入前導課程折扣金額計算
                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text))
                    {
                        ((Label)item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountFC")).Text)).ToString();
                    }
                    else
                    {
                        ((Label)item.FindControl("LB_HDiscountFC")).Text = "";
                    }




                    //GA20231219-小計=輸入的付款金額-折扣總金額
                    if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountFC")).Text))
                    {
                        ((Label)item.FindControl("LB_HSubTotalFC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountFC")).Text)).ToString();
                    }
                    else
                    {
                        ((Label)item.FindControl("LB_HSubTotalFC")).Text = ((TextBox)item.FindControl("TB_HPAmountFC")).Text;
                    }


                    LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty)));//總計=小計
                                                                                                                                                          //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text));//總計=小計-前導課程減免總金額

                    if (Convert.ToInt32(LB_Total.Text.Replace(",", string.Empty)) < 0)
                    {
                        LB_Total.Text = "0";
                    }


                    #region 
                    //GA241202-更新折扣碼&折扣金額&折扣後金額
                    SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect, HPoint = @HPoint, HDCode=@HDCode, HDPoint=@HDPoint, HPAmount=@HPAmount, HSubTotal=@HSubTotal WHERE HID =@HID", SQLdatabase.OpenConnection());
                    QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDFC")).Text);
                    QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectFC")).Checked);
                    QueryUpdSC.Parameters.AddWithValue("@HPoint", ((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty));  //基金會課程直接存金額
                    QueryUpdSC.Parameters.AddWithValue("@HDCode", ""); //更新為空值
                    QueryUpdSC.Parameters.AddWithValue("@HDPoint", "");  //更新為空值
                    QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim());
                    QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalFC")).Text);
                    QueryUpdSC.ExecuteNonQuery();
                    QueryUpdSC.Cancel();
                    SQLdatabase.OpenConnection().Close();
                    #endregion


                }


            }
            else
            {
                ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('折扣碼錯誤，請重新輸入。');", true);
                ((TextBox)item.FindControl("TB_HDCodeFC")).Text = "";
                ((TextBox)item.FindControl("TB_HDCodeFC")).Focus();
                ((Label)item.FindControl("LB_HDPointFC")).Text = "0";
                HDisPoint = 0;


                //GE20231219_加入前導課程折扣金額計算
                if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text))
                {
                    ((Label)item.FindControl("LB_HDiscountFC")).Text = Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountFC")).Text).ToString();
                }
                else
                {
                    ((Label)item.FindControl("LB_HDiscountFC")).Text = "";
                }



                //221013-小計=輸入的付款金額
                //((Label)item.FindControl("LB_HSubTotalFC")).Text = ((TextBox)item.FindControl("TB_HPAmountFC")).Text;

                //GE20231219_計算小計 =輸入的付款金額-折扣總金額
                if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountFC")).Text))
                {
                    ((Label)item.FindControl("LB_HSubTotalFC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountFC")).Text)).ToString();
                }
                else
                {
                    ((Label)item.FindControl("LB_HSubTotalFC")).Text = ((TextBox)item.FindControl("TB_HPAmountFC")).Text;
                }


                LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty)));//總計=小計
                                                                                                                                                      //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text));//總計=小計-前導課程減免總金額
                if (Convert.ToInt32(LB_Total.Text.Replace(",", string.Empty)) < 0)
                {
                    LB_Total.Text = "0";
                }

                #region 
                //GA241202-更新折扣碼&折扣金額&折扣後金額
                SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect, HPoint = @HPoint, HDCode=@HDCode, HDPoint=@HDPoint, HPAmount=@HPAmount, HSubTotal=@HSubTotal WHERE HID =@HID", SQLdatabase.OpenConnection());
                QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDFC")).Text);
                QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectFC")).Checked);
                QueryUpdSC.Parameters.AddWithValue("@HPoint", ((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty));  //基金會課程直接存金額
                QueryUpdSC.Parameters.AddWithValue("@HDCode", ""); //更新為空值
                QueryUpdSC.Parameters.AddWithValue("@HDPoint", "");  //更新為空值
                QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim());
                QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalFC")).Text);
                QueryUpdSC.ExecuteNonQuery();
                QueryUpdSC.Cancel();
                SQLdatabase.OpenConnection().Close();
                #endregion


            }
            QuerySelHDC.Close();
        }
        else
        {
            ((Label)item.FindControl("LB_HDPointFC")).Text = "0";
            //GE20231219_折扣金額計算加入前導課程折扣金額
            if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HLDiscountFC")).Text))
            {
                ((Label)item.FindControl("LB_HDiscountFC")).Text = (Convert.ToInt32(((Label)item.FindControl("LB_HLDiscountFC")).Text)).ToString();
            }
            else
            {
                ((Label)item.FindControl("LB_HDiscountFC")).Text = "";
            }


            //((Label)item.FindControl("LB_HSubTotalFC")).Text = ((TextBox)item.FindControl("TB_HPAmountFC")).Text;
            //GE20231219_修改小計=輸入總金額-折扣總金額
            if (!string.IsNullOrEmpty(((Label)item.FindControl("LB_HDiscountFC")).Text))
            {
                ((Label)item.FindControl("LB_HSubTotalFC")).Text = (Convert.ToInt32(((TextBox)item.FindControl("TB_HPAmountFC")).Text) - Convert.ToInt32(((Label)item.FindControl("LB_HDiscountFC")).Text)).ToString();
            }
            else
            {
                ((Label)item.FindControl("LB_HSubTotalFC")).Text = ((TextBox)item.FindControl("TB_HPAmountFC")).Text;
            }



            LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty)));//總計=小計

            #region 230107-更新折扣碼&折扣金額&折扣後金額
            SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HSelect=@HSelect, HPoint = @HPoint, HDCode=@HDCode, HDPoint=@HDPoint, HPAmount=@HPAmount, HSubTotal=@HSubTotal WHERE HID =@HID", SQLdatabase.OpenConnection());
            QueryUpdSC.Parameters.AddWithValue("@HID", ((Label)item.FindControl("LB_ShoppingCartIDFC")).Text);
            QueryUpdSC.Parameters.AddWithValue("@HSelect", ((CheckBox)item.FindControl("CB_SelectFC")).Checked);
            QueryUpdSC.Parameters.AddWithValue("@HPoint", ((Label)item.FindControl("LB_HSubTotalFC")).Text.Replace(",", string.Empty));  //基金會課程直接存金額
            QueryUpdSC.Parameters.AddWithValue("@HDCode", ""); //更新為空值
            QueryUpdSC.Parameters.AddWithValue("@HDPoint", "");  //更新為空值
            QueryUpdSC.Parameters.AddWithValue("@HPAmount", ((TextBox)item.FindControl("TB_HPAmountFC")).Text.Trim());
            QueryUpdSC.Parameters.AddWithValue("@HSubTotal", ((Label)item.FindControl("LB_HSubTotalFC")).Text);
            QueryUpdSC.ExecuteNonQuery();
            QueryUpdSC.Cancel();
            SQLdatabase.OpenConnection().Close();
            #endregion
        }


    }
    #endregion

    #region 刪除文化課程
    protected void LBtn_DelC_Click(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        LinkButton gDelC = sender as LinkButton;
        string gDelC_A = gDelC.CommandArgument;

        SqlCommand cmd = new SqlCommand("DELETE FROM HShoppingCart WHERE HID=@HID", SQLdatabase.OpenConnection());
        cmd.Parameters.AddWithValue("@HID", gDelC_A);
        cmd.ExecuteNonQuery();
        cmd.Cancel();
        SQLdatabase.OpenConnection().Close();
        SDS_Cultural.SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCTemplateID, a.HCourseName, a.HCourseID, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, (b.HBCPoint*10) AS CPrice, MAX(a.HPoint*10) AS SCPrice, a.HLDiscount*10 AS LDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange LEFT JOIN HExamContent AS c ON b.HExamContentID=c.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseDonate='0' AND b.HPMethod='2' AND b.HVerifyStatus='2' AND b.HStatus='1' GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, a.HLDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod";
        SDS_Cultural.DataBind();
        Rpt_Cultural.DataBind();


        #region 報名/捐款清單重整
        ((SqlDataSource)Master.FindControl("SDS_ShoppingCart")).SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, IIF(ISNUMERIC(b.HBCPoint )=1,FORMAT(IIF(b.HBCPoint  IS NULL,0,b.HBCPoint ),'N0'),b.HBCPoint ) AS HBCPoint,a.HCourseDonate   FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND b.HStatus='1' AND b.HVerifyStatus='2' GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint,a.HCourseDonate";
        ((SqlDataSource)Master.FindControl("SDS_ShoppingCart")).DataBind();
        ((Repeater)Master.FindControl("RPT_ShoppingCart")).DataBind();

        SqlDataReader cartnum = SQLdatabase.ExecuteReader("SELECT SUM(CAST(m.HBCPoint AS INT)*10) AS Total, COUNT(m.CartNum) AS HCartNum FROM (SELECT b.HBCPoint, a.HCourseName , COUNT(a.HCourseName) AS CartNum, a.HCourseDonate FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange AND a.HPMethod=b.HPMethod WHERE HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND b.HStatus='1' AND b.HVerifyStatus='2'  GROUP BY  a.HCourseName , a.HCTemplateID, a.HDateRange, b.HBCPoint, b.HPMethod, a.HCourseDonate ) AS m ");
        //SqlDataReader cartnum = SQLdatabase.ExecuteReader("SELECT COUNT(m.CartNum) AS HCartNum FROM (SELECT COUNT(a.HCourseName) AS CartNum, a.HCourseName FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange WHERE HMemberID='"+((Label)Master.FindControl("LB_HUserHID")).Text +"' GROUP BY a.HCourseName, b.HBCPoint) AS m");
        if (cartnum.Read())
        {
            ((Label)Master.FindControl("LB_HCartNum")).Text = cartnum["HCartNum"].ToString();
            ((Label)Master.FindControl("LB_HCartNum1")).Text = cartnum["HCartNum"].ToString();
            ((Label)Master.FindControl("LB_Total")).Text = !string.IsNullOrEmpty(cartnum["Total"].ToString()) ? (Convert.ToInt32(cartnum["Total"].ToString())).ToString("N0") : "0";
            //讓master的數量可以一起同步更新
            ((UpdatePanel)Master.FindControl("UpdateNumbers")).Update();

        }
        cartnum.Close();
        #endregion

    }
    #endregion

    #region 刪除傳光(基金會)課程
    protected void LBtn_DelFC_Click(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        LinkButton gDelFC = sender as LinkButton;
        string gDelFC_CA = gDelFC.CommandArgument;

        SqlCommand cmd = new SqlCommand("DELETE FROM HShoppingCart WHERE HID=@HID", SQLdatabase.OpenConnection());
        cmd.Parameters.AddWithValue("@HID", gDelFC_CA);
        cmd.ExecuteNonQuery();
        cmd.Cancel();
        SQLdatabase.OpenConnection().Close();
        SDS_FoundationC.SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HCourseID, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, (b.HBCPoint*10) AS FCPrice, MAX(a.HPoint) AS SFCPrice, a.HLDiscount*10 AS LDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange LEFT JOIN HExamContent AS c ON b.HExamContentID=c.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseDonate='0' AND b.HPMethod='1' AND b.HVerifyStatus='2' AND b.HStatus='1' GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, a.HLDiscount, a.HAttend, b.HExamContentID, c.HChargeMethod";
        SDS_FoundationC.DataBind();
        Rpt_FoundationC.DataBind();

        #region 報名/捐款清單重整
        ((SqlDataSource)Master.FindControl("SDS_ShoppingCart")).SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, IIF(ISNUMERIC(b.HBCPoint )=1,FORMAT(IIF(b.HBCPoint  IS NULL,0,b.HBCPoint ),'N0'),b.HBCPoint ) AS HBCPoint,a.HCourseDonate   FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND b.HStatus='1' AND b.HVerifyStatus='2' GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint,a.HCourseDonate";
        ((SqlDataSource)Master.FindControl("SDS_ShoppingCart")).DataBind();
        ((Repeater)Master.FindControl("RPT_ShoppingCart")).DataBind();

        SqlDataReader cartnum = SQLdatabase.ExecuteReader("SELECT SUM(CAST(m.HBCPoint AS INT)*10) AS Total, COUNT(m.CartNum) AS HCartNum FROM (SELECT b.HBCPoint, a.HCourseName , COUNT(a.HCourseName) AS CartNum, a.HCourseDonate FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange AND a.HPMethod=b.HPMethod WHERE HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND b.HStatus='1' AND b.HVerifyStatus='2' GROUP BY  a.HCourseName , a.HCTemplateID, a.HDateRange, b.HBCPoint, b.HPMethod, a.HCourseDonate ) AS m ");
        //SqlDataReader cartnum = SQLdatabase.ExecuteReader("SELECT COUNT(m.CartNum) AS HCartNum FROM (SELECT COUNT(a.HCourseName) AS CartNum, a.HCourseName FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange WHERE HMemberID='"+((Label)Master.FindControl("LB_HUserHID")).Text +"' GROUP BY a.HCourseName, b.HBCPoint) AS m");
        if (cartnum.Read())
        {
            ((Label)Master.FindControl("LB_HCartNum")).Text = cartnum["HCartNum"].ToString();
            ((Label)Master.FindControl("LB_HCartNum1")).Text = cartnum["HCartNum"].ToString();
            ((Label)Master.FindControl("LB_Total")).Text = !string.IsNullOrEmpty(cartnum["Total"].ToString()) ? (Convert.ToInt32(cartnum["Total"].ToString())).ToString("N0") : "0";
            //讓master的數量可以一起同步更新
            ((UpdatePanel)Master.FindControl("UpdateNumbers")).Update();

        }
        cartnum.Close();
        #endregion
    }
    #endregion

    #region 刪除課程捐款
    protected void LBtn_DelFD_Click(object sender, EventArgs e)
    {
        //執行js
        RegScript();

        LinkButton gDelFD = sender as LinkButton;
        string gDelFD_A = gDelFD.CommandArgument;

        SqlCommand cmd = new SqlCommand("DELETE FROM HShoppingCart WHERE HID=@HID", SQLdatabase.OpenConnection());
        cmd.Parameters.AddWithValue("@HID", gDelFD_A);
        cmd.ExecuteNonQuery();
        cmd.Cancel();
        SQLdatabase.OpenConnection().Close();
        SDS_FoundationD.SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HCourseID, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint, (b.HBCPoint*10) AS FDPrice FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseDonate='1' AND b.HVerifyStatus='2' AND b.HStatus='1'  GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint";
        SDS_FoundationD.DataBind();
        Rpt_FoundationD.DataBind();

        #region 報名/捐款清單重整
        ((SqlDataSource)Master.FindControl("SDS_ShoppingCart")).SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, IIF(ISNUMERIC(b.HBCPoint )=1,FORMAT(IIF(b.HBCPoint  IS NULL,0,b.HBCPoint ),'N0'),b.HBCPoint ) AS HBCPoint,a.HCourseDonate   FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND b.HStatus='1' AND b.HVerifyStatus='2' GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint,a.HCourseDonate";
        ((SqlDataSource)Master.FindControl("SDS_ShoppingCart")).DataBind();
        ((Repeater)Master.FindControl("RPT_ShoppingCart")).DataBind();

        SqlDataReader cartnum = SQLdatabase.ExecuteReader("SELECT SUM(CAST(m.HBCPoint AS INT)*10) AS Total, COUNT(m.CartNum) AS HCartNum FROM (SELECT b.HBCPoint, a.HCourseName , COUNT(a.HCourseName) AS CartNum, a.HCourseDonate FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange AND a.HPMethod=b.HPMethod WHERE HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND b.HStatus='1' AND b.HVerifyStatus='2' GROUP BY  a.HCourseName , a.HCTemplateID, a.HDateRange, b.HBCPoint, b.HPMethod, a.HCourseDonate ) AS m ");
        //SqlDataReader cartnum = SQLdatabase.ExecuteReader("SELECT COUNT(m.CartNum) AS HCartNum FROM (SELECT COUNT(a.HCourseName) AS CartNum, a.HCourseName FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange WHERE HMemberID='"+((Label)Master.FindControl("LB_HUserHID")).Text +"' GROUP BY a.HCourseName, b.HBCPoint) AS m");
        if (cartnum.Read())
        {
            ((Label)Master.FindControl("LB_HCartNum")).Text = cartnum["HCartNum"].ToString();
            ((Label)Master.FindControl("LB_HCartNum1")).Text = cartnum["HCartNum"].ToString();
            ((Label)Master.FindControl("LB_Total")).Text = !string.IsNullOrEmpty(cartnum["Total"].ToString()) ? (Convert.ToInt32(cartnum["Total"].ToString())).ToString("N0") : "0";
            //讓master的數量可以一起同步更新
            ((UpdatePanel)Master.FindControl("UpdateNumbers")).Update();

        }
        cartnum.Close();
        #endregion
    }
    #endregion

    #region 刪除體系
    protected void LBtn_DelG_Click(object sender, EventArgs e)
    {

    }
    #endregion

    #region 計算金額加總 --221003-  ADD
    public void Count()
    {
        //先全部歸零
        CulturalTotal = 0;
        FoundationTotal = 0;
        DonateTotal = 0;
        //Remain = 0;
        CNum = 0;
        FNum = 0;
        DNum = 0;




        //判斷目前是屬於LB_Navtab.Text=2 :玉成(二~七階)-玉成(文化事業)，1:傳光(尋光階~一階)-傳光(基金會)
        if (LB_Navtab.Text == "2")  //玉成(二~七階)-玉成(文化事業)
        {

            //單一課程
            for (int i = 0; i < Rpt_Cultural.Items.Count; i++)
            {
                if (((CheckBox)Rpt_Cultural.Items[i].FindControl("CB_SelectC")).Checked == true)
                {
                    CNum += 1;
                    if (!string.IsNullOrEmpty(((Label)Rpt_Cultural.Items[i].FindControl("LB_HSubTotalC")).Text))
                    {
                        CulturalTotal += Convert.ToDecimal(((Label)Rpt_Cultural.Items[i].FindControl("LB_HSubTotalC")).Text);
                    }
                }
            }




            LB_Total.Text = (Convert.ToDouble(CulturalTotal)).ToString("N0");
            LB_Num.Text = CNum.ToString();

            //221015-扣除剩餘點數
            if (Remain != 0)
            {
                DIV_RemainPoint.Visible = true;
                DIV_Difference.Visible = true;

                if ((CulturalTotal - Remain) > 0)
                {
                    LB_Difference.Text = Convert.ToDouble((CulturalTotal - Remain)).ToString("N0");
                }
                else
                {
                    LB_Difference.Text = "0";
                }

            }
            else
            {
                DIV_RemainPoint.Visible = false;
                DIV_Difference.Visible = false;
                LB_Difference.Text = "0";
            }

        }
        else if (LB_Navtab.Text == "1")  //傳光(尋光階~一階)-傳光(基金會)
        {
            DIV_RemainPoint.Visible = false;
            DIV_Difference.Visible = false;
            LB_RemainPoints.Text = "0";
            LB_Remain.Text = "0";
            Remain = 0;


            for (int i = 0; i < Rpt_FoundationC.Items.Count; i++)
            {
                if (((CheckBox)Rpt_FoundationC.Items[i].FindControl("CB_SelectFC")).Checked == true)
                {
                    FNum += 1;
                    if (!string.IsNullOrEmpty(((Label)Rpt_FoundationC.Items[i].FindControl("LB_HSubTotalFC")).Text))
                    {
                        FoundationTotal += Convert.ToDecimal(((Label)Rpt_FoundationC.Items[i].FindControl("LB_HSubTotalFC")).Text);
                    }

                }
            }





            for (int i = 0; i < Rpt_FoundationD.Items.Count; i++)
            {
                if (((CheckBox)Rpt_FoundationD.Items[i].FindControl("CB_SelectFD")).Checked == true)
                {
                    DNum += 1;
                    if (!string.IsNullOrEmpty(((TextBox)Rpt_FoundationD.Items[i].FindControl("TB_PAmountFD")).Text.Trim()))
                    {
                        DonateTotal += Convert.ToDecimal(((TextBox)Rpt_FoundationD.Items[i].FindControl("TB_PAmountFD")).Text.Trim());
                    }
                }
            }
            LB_Total.Text = (Convert.ToDouble(FoundationTotal + DonateTotal)).ToString("N0");
            LB_Num.Text = (FNum + DNum).ToString();
        }
        else if (LB_Navtab.Text == "3")  //綜合
        {
            for (int i = 0; i < Rpt_Cultural.Items.Count; i++)
            {
                if (((CheckBox)Rpt_Cultural.Items[i].FindControl("CB_SelectC")).Checked == true)
                {
                    CNum += 1;
                    if (!string.IsNullOrEmpty(((Label)Rpt_Cultural.Items[i].FindControl("LB_HSubTotalC")).Text))
                    {
                        CulturalTotal += Convert.ToDecimal(((Label)Rpt_Cultural.Items[i].FindControl("LB_HSubTotalC")).Text);
                    }
                }
            }

            for (int i = 0; i < Rpt_FoundationC.Items.Count; i++)
            {
                if (((CheckBox)Rpt_FoundationC.Items[i].FindControl("CB_SelectFC")).Checked == true)
                {
                    FNum += 1;
                    if (!string.IsNullOrEmpty(((Label)Rpt_FoundationC.Items[i].FindControl("LB_HSubTotalFC")).Text))
                    {
                        FoundationTotal += Convert.ToDecimal(((Label)Rpt_FoundationC.Items[i].FindControl("LB_HSubTotalFC")).Text);
                    }
                }
            }

            for (int i = 0; i < Rpt_FoundationD.Items.Count; i++)
            {
                if (((CheckBox)Rpt_FoundationD.Items[i].FindControl("CB_SelectFD")).Checked == true)
                {
                    DNum += 1;
                    if (!string.IsNullOrEmpty(((TextBox)Rpt_FoundationD.Items[i].FindControl("TB_PAmountFD")).Text.Trim()))
                    {
                        DonateTotal += Convert.ToDecimal(((TextBox)Rpt_FoundationD.Items[i].FindControl("TB_PAmountFD")).Text.Trim());
                    }
                }
            }
            LB_Total.Text = (Convert.ToDouble(FoundationTotal + DonateTotal)).ToString("N0");
            LB_Num.Text = (FNum + DNum).ToString();

            //Response.Write("AA=" + CNum);

            LB_Total.Text = (Convert.ToDouble(CulturalTotal + FoundationTotal + DonateTotal)).ToString("N0");
            LB_Num.Text = (CNum + FNum + DNum).ToString();


        }
    }
    #endregion

    #region 填寫資料
    int alerttime = 0;
    protected void Btn_FillData_Click(object sender, EventArgs e)
    {

        //GA20240409_重新計算金額，避免textchanged比按按鈕來的慢
        Count();


        //ScriptManager.RegisterStartupScript(this, this.GetType(), "aaa", "alert('aaa=" + Session["Step"].ToString() + "');", true);
        if (Session["Step"] != null)
        {
            if (Convert.ToInt32(Session["Step"].ToString()) <= 2)
            {
                Session["Step"] = "2";
            }
            else
            {
            }
        }




        #region 判斷如果總金額為0，則隱藏付款方式
        if (LB_Total.Text == "0")
        {
            Panel_PayMethod.Visible = false;
            Panel_Invoice.Visible = false;
        }
        else if (LB_Total.Text != "0" && LB_Remain.Text != "0" && LB_Difference.Text == "0" && LB_Navtab.Text == "2")  //文化的結帳，剩餘點數!=0且尚補差額==0
        {
            Panel_PayMethod.Visible = false;
            Panel_Invoice.Visible = false;
        }
        else
        {
            Panel_PayMethod.Visible = true;
            Panel_Invoice.Visible = true;
        }
        #endregion




        #region 判斷有無勾選
        string ckSelectFC = "0";
        string ckSelectC = "0";
        string ckSelectFD = "0";

        #region 230214- OLD寫法註解
        //if (LB_Navtab.Text == "1")  //傳光(基金會)
        //      {
        //          for (int i = 0; i < Rpt_FoundationC.Items.Count; i++)  //傳光課程(傳光(基金會)課程)
        //          {
        //              if (((CheckBox)Rpt_FoundationC.Items[i].FindControl("CB_SelectFC")).Checked == true)
        //              {
        //                  ckSelectFC = "1";
        //              }
        //          }
        //          for (int i = 0; i < Rpt_FoundationD.Items.Count; i++)    //捐款
        //          {
        //              if (((CheckBox)Rpt_FoundationD.Items[i].FindControl("CB_SelectFD")).Checked == true)
        //              {
        //                  ckSelectFD = "1";
        //              }
        //          }
        //      }
        //      else
        //      {
        //          for (int j = 0; j < Rpt_Cultural.Items.Count; j++)   //玉成課程(玉成(文化事業)課程)
        //          {
        //              if (((CheckBox)Rpt_Cultural.Items[j].FindControl("CB_SelectC")).Checked == true)
        //              {
        //                  ckSelectC = "1";
        //              }
        //          }
        //      }
        #endregion


        for (int i = 0; i < Rpt_FoundationC.Items.Count; i++)  //傳光課程(傳光(基金會)課程)
        {
            if (((CheckBox)Rpt_FoundationC.Items[i].FindControl("CB_SelectFC")).Checked == true)
            {
                ckSelectFC = "1";
            }
        }
        for (int i = 0; i < Rpt_FoundationD.Items.Count; i++)    //捐款
        {
            if (((CheckBox)Rpt_FoundationD.Items[i].FindControl("CB_SelectFD")).Checked == true)
            {
                ckSelectFD = "1";
            }
        }
        for (int j = 0; j < Rpt_Cultural.Items.Count; j++)   //玉成課程(玉成(文化事業)課程)
        {
            if (((CheckBox)Rpt_Cultural.Items[j].FindControl("CB_SelectC")).Checked == true)
            {
                ckSelectC = "1";
            }
        }

        if (ckSelectFC == "1" && ckSelectC == "1" || ckSelectFD == "1" && ckSelectC == "1")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('一次只能選擇結帳一個類別哦~!請先選擇要報名玉成還是傳光~'); window.location.href='HShoppingCart.aspx';", true);
            return;
        }

        if (ckSelectFC == "0" && ckSelectC == "0" && ckSelectFD == "0")
        {
            //Response.Write("<script>alert('請至少勾選一個項目才能結帳喔~'); window.location.href='HShoppingCart.aspx';</script>");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice3", "alert('請至少勾選一個項目才能結帳喔~!'); window.location.href='HShoppingCart.aspx';", true);
            return;
        }
        #endregion

        #region 取得學員區屬
        if (string.IsNullOrEmpty(LB_HAreaID.Text))
        {
            SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HAreaID FROM HMember WHERE HID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");

            if (dr.Read())
            {
                LB_HAreaID.Text = dr["HAreaID"].ToString();
            }
            dr.Close();

        }

        #endregion


        #region 判斷勾選課程必需有選上課地點&上課地點區屬提示



        bool boolAreaYN = true;

        if (LB_Navtab.Text == "1")  //傳光(基金會)
        {
            for (int i = 0; i < Rpt_FoundationC.Items.Count; i++)
            {
                if (((DropDownList)Rpt_FoundationC.Items[i].FindControl("DDL_HOCPlaceFC")).SelectedValue == "0" && ((CheckBox)Rpt_FoundationC.Items[i].FindControl("CB_SelectFC")).Checked == true)
                {
                    //Response.Write("<script>alert('有勾選的課程請選擇上課地點哦~'); window.location.href='HShoppingCart.aspx';</script>");
                    // Response.Write("<script>alert('有勾選的課程請選擇上課地點哦~'); javascript:history.back(-1);</script>");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice3", "alert('有勾選的課程請選擇上課地點哦~');", true);
                    return;
                }
                //GA20231209_判斷付款金額是否有填寫
                else if (string.IsNullOrEmpty(((TextBox)Rpt_FoundationC.Items[i].FindControl("TB_HPAmountFC")).Text.Trim()) && ((CheckBox)Rpt_FoundationC.Items[i].FindControl("CB_SelectFC")).Checked == true)
                {
                    //Response.Write("<script>alert('有勾選的課程請選擇上課地點哦~'); window.location.href='HShoppingCart.aspx';</script>");
                    // Response.Write("<script>alert('有勾選的課程請選擇上課地點哦~'); javascript:history.back(-1);</script>");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice3", "alert('" +
                        "還有課程還沒填寫課程費用哦~');", true);
                    return;
                }
                else if (((DropDownList)Rpt_FoundationC.Items[i].FindControl("DDL_HOCPlaceFC")).SelectedValue != "0" && ((CheckBox)Rpt_FoundationC.Items[i].FindControl("CB_SelectFC")).Checked == true)
                {
                    SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HAreaID FROM HPlaceList AS a WHERE HID='" + ((DropDownList)Rpt_FoundationC.Items[i].FindControl("DDL_HOCPlaceFC")).SelectedValue + "' AND  (HAreaID LIKE '%," + LB_HAreaID.Text + "' OR HAreaID LIKE '" + LB_HAreaID.Text + ",%' OR HAreaID LIKE '%," + LB_HAreaID.Text + ",%' OR HAreaID='" + LB_HAreaID.Text + "' OR HAreaID='0') ");
                    if (!dr.Read())
                    {
                        boolAreaYN = false;
                    }


                }

            }
        }
        else
        {
            for (int j = 0; j < Rpt_Cultural.Items.Count; j++)
            {
                if (((DropDownList)Rpt_Cultural.Items[j].FindControl("DDL_HOCPlaceC")).SelectedValue == "0" && ((CheckBox)Rpt_Cultural.Items[j].FindControl("CB_SelectC")).Checked == true)
                {
                    //Response.Write("<script>alert('有勾選的課程請選擇上課地點哦~'); window.location.href='HShoppingCart.aspx';</script>");
                    //Response.Write("<script>alert('有勾選的課程請選擇上課地點哦~'); javascript:history.back(-1);</script>");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice3", "alert('有勾選的課程請選擇上課地點哦~');", true);
                    return;
                }
                //GA20231209_判斷付款金額是否有填寫
                else if (string.IsNullOrEmpty(((TextBox)Rpt_Cultural.Items[j].FindControl("TB_HPAmountC")).Text.Trim()) && ((CheckBox)Rpt_Cultural.Items[j].FindControl("CB_SelectC")).Checked == true)
                {
                    //Response.Write("<script>alert('有勾選的課程請選擇上課地點哦~'); window.location.href='HShoppingCart.aspx';</script>");
                    // Response.Write("<script>alert('有勾選的課程請選擇上課地點哦~'); javascript:history.back(-1);</script>");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice3", "alert('" +
                        "還有課程還沒填寫課程費用哦~');", true);
                    return;
                }
                else if (((DropDownList)Rpt_Cultural.Items[j].FindControl("DDL_HOCPlaceC")).SelectedValue != "0" && ((CheckBox)Rpt_Cultural.Items[j].FindControl("CB_SelectC")).Checked == true)
                {
                    SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HAreaID FROM HPlaceList AS a WHERE HID='" + ((DropDownList)Rpt_Cultural.Items[j].FindControl("DDL_HOCPlaceC")).SelectedValue + "' AND  (HAreaID LIKE '%," + LB_HAreaID.Text + "' OR HAreaID LIKE '" + LB_HAreaID.Text + ",%' OR HAreaID LIKE '%," + LB_HAreaID.Text + ",%' OR HAreaID='" + LB_HAreaID.Text + "'  OR HAreaID='0') ");
                    if (!dr.Read())
                    {
                        boolAreaYN = false;
                    }


                }
            }
        }


        if (boolAreaYN == false)
        {
            // ScriptManager.RegisterStartupScript(this, Page.GetType(), "confirm", ("myFunction();"), true);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "confirm", ("myFunction();"), true);
            //Response.Write("<script>alert('上課地點與您的區屬不同哦~再請確認地點是否正確~');</script>");
            //return;
        }


        #endregion


        Panel_Cart.Visible = false;
        Panel_FillData.Visible = true;
        LB_Note.Visible = false;
        LB_HNotice.Visible = false;
        //Panel_Check.Visible = false;

        Btn_Back.Visible = true;
        Btn_FillData.Visible = false;
        Btn_CheckOut.Visible = true;
        //Btn_Next.Visible = false;

        DIV_Cart.Attributes.Add("class", "process-item");
        DIV_Data.Attributes.Add("class", "process-item active");


        TB_HGDay_Add.Attributes.Add("readonly", "true");

        //221003- : 一次只能一種做結帳
        if (LB_Navtab.Text == "1")   //傳光(基金會)
        {
            DIV_RemainPoint.Visible = false;


            //GA20231011_新增參班身分
            //GA250302_新增是否產生點名單欄位
            SDS_FillData.SelectCommand = "SELECT a.HID as ShoppingCartID, a.HCourseID, a.HSelect, a.HCourseName, a.HCourseDonate, b.HBCPoint, c.HPlaceName, a.HAttend, b.HRollcallYN FROM HShoppingCart as a LEFT JOIN HCourse as b ON a.HCourseID=b.HID LEFT JOIN HPlace AS c ON b.HOCPlace=c.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HSelect='true' AND ((a.HPMethod='1' AND HCourseID <>'0') OR a.HCourseDonate='1') ORDER BY a.HCourseDonate ASC";

            #region 是否上傳國稅局&身分證
            //WA20240125_加入是否上傳國稅局顯示判斷
            Div_HUploadIRS.Visible = DDL_InvoiceType.SelectedValue == "1" ? true : false;

            //WA20240125_系統自動取值(是否上傳國稅局&身分證字號)
            SqlDataReader MemberReader = SQLdatabase.ExecuteReader("SELECT HUploadIRS, HPersonID FROM HMember WHERE HID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
            if (MemberReader.Read())
            {
                //WA20240221_CheckBox改為DropDownList
                DDL_HUploadIRS.SelectedValue = !string.IsNullOrEmpty(MemberReader["HUploadIRS"].ToString()) && MemberReader["HUploadIRS"].ToString() == "1" ? "1" : "0";
                Div_HPersonID.Visible = DDL_HUploadIRS.SelectedValue == "1" && DDL_InvoiceType.SelectedValue == "1" ? true : false;


                TB_HPersonID.Text = MemberReader["HPersonID"].ToString();

                if (!string.IsNullOrEmpty(MemberReader["HPersonID"].ToString()))
                {
                    TB_HPersonID.Enabled = false;
                }
                else
                {
                    TB_HPersonID.Enabled = true;
                }
            }
            MemberReader.Close();

            #endregion
        }
        else if (LB_Navtab.Text == "2")  //玉成(文化事業)
        {
            //GA20231011_新增參班身分
            //GA250302_新增是否產生點名單欄位
            SDS_FillData.SelectCommand = "SELECT a.HID as ShoppingCartID, a.HCourseID, a.HSelect, a.HCourseName, a.HCourseDonate, b.HBCPoint, c.HPlaceName, a.HAttend, b.HRollcallYN FROM HShoppingCart as a LEFT JOIN HCourse as b ON a.HCourseID=b.HID LEFT JOIN HPlace AS c ON b.HOCPlace=c.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HSelect='true' AND b.HPMethod='2' AND a.HCourseDonate='0' AND HCourseID <>'0'";

            //WA20240125_文化課程-是否上傳國稅局欄位&身分證隱藏
            Div_HUploadIRS.Visible = false;
            Div_HPersonID.Visible = false;
        }


        Session["SubmitCheck"] = "1";
    }
    #endregion

    #endregion

    #region 填寫資料 Page

    protected void Rpt_FillData_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {


        //取得Repeater的DataRowView
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        if (gDRV["HCourseDonate"].ToString() == "1" && gDRV["HCourseID"].ToString() == "0")
        {
            ((LinkButton)e.Item.FindControl("LBtn_FillIn")).Visible = false;
            ((Label)e.Item.FindControl("LB_HCourseName")).Text += " <span style='color:#9850ee;'>(捐款)</span>";
        }

        //GA20231011_加入參班身分顯示
        if (gDRV["HAttend"].ToString() == "1")
        {
            ((Label)e.Item.FindControl("LB_HAttend")).Text = "參班";
        }
        else if (gDRV["HAttend"].ToString() == "5")
        {
            ((Label)e.Item.FindControl("LB_HAttend")).Text = "純護持(非班員)";
        }
        else if (gDRV["HAttend"].ToString() == "6")   //參班兼護持
        {
            ((Label)e.Item.FindControl("LB_HAttend")).Text = "參班兼護持";
        }
        //GA20240126_加入0的判斷
        else if (gDRV["HAttend"].ToString() == "0")
        {
            ((Label)e.Item.FindControl("LB_HAttend")).Text = "";
        }

    }

    #region 填寫資料modal按鈕
    protected void LBtn_FillIn_Click(object sender, EventArgs e)
    {

        if (Session["SubmitCheck"] != null)
        {
            if (Session["SubmitCheck"].ToString() == "0")
            {
                Response.Write("<script>alert('資料錯誤，請重新選擇'); window.location.href='HShoppingCart.aspx'; </script>");
                return;
            }
        }
        else
        {
            Response.Write("<script>alert('畫面停留太久，請重新操作報名流程哦~謝謝~!');window.location.href='HShoppingCart.aspx';</script>");
        }



        LinkButton gFillIn = sender as LinkButton;
        string gFillIn_A = gFillIn.CommandArgument;

        CB_HSameAsCourse_Add.Checked = false;
        TA_Remark.InnerText = "";
        DDL_HCGuide.Items.Clear();
        //GA20230922_加入一對一護持(請選擇)
        DDL_HCGuide.Items.Add(new ListItem("請選擇", "0"));
        LBox_HOtherMember.Items.Clear();
        DDL_HGroupName_Add.Items.Clear();
        DDL_HGroupName_Add.Items.Add(new ListItem("請選擇", "0"));
        DDL_HTask_Add.Items.Clear();
        DDL_HTask_Add.Items.Add(new ListItem("請選擇", "0"));


        TB_HGDay_Add.Text = "";
        TB_GSTime_Add.Text = "";
        TB_GETime_Add.Text = "";


        //SqlDataReader MyQueryHSCP = SQLdatabase.ExecuteReader("SELECT a.HID as ShoppingCartID, a.HCourseID, a.HSelect, b.HNGuide, b.HType, b.HCourseName, b.HBCPoint, b.HRSystem, c.HPlaceName FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCourseID=b.HID LEFT JOIN HPlace AS c ON b.HOCPlace=c.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HID='"+ gFillIn_A +"'");

        //改抓CourseList檢視表
        //SqlDataReader MyQueryHSCP = SQLdatabase.ExecuteReader("SELECT a.HID as ShoppingCartID, a.HCTemplateID, a.HCourseID, a.HSelect, a.HDateRange, a.HPMethod, a.HRemark, b.HNGuide, b.HType, b.HCourseName, b.HBCPoint, b.HRSystem, b.Location FROM HShoppingCart AS a LEFT JOIN CourseList AS b ON a.HCourseID=b.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HID='" + gFillIn_A + "'");

        //GE20230922_加入HCGuide資料讀取
        SqlDataReader MyQueryHSCP = SQLdatabase.ExecuteReader("SELECT a.HID as ShoppingCartID, a.HCTemplateID, a.HCourseID, a.HSelect, a.HCGuide, a.HDateRange, a.HPMethod, a.HRemark, b.HNGuide, b.HType, b.HCourseName, b.HBCPoint, b.HRSystem, b.Location FROM HShoppingCart AS a LEFT JOIN CourseList AS b ON a.HCourseID=b.HID WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HID='" + gFillIn_A + "'");

        if (MyQueryHSCP.Read())
        {

            LB_CourseID.Text = MyQueryHSCP["HCourseID"].ToString();
            LB_ShoppingCartID.Text = gFillIn_A;
            LB_SystemID.Text = MyQueryHSCP["HRSystem"].ToString().TrimEnd(',');
            LB_HDateRange_Add.Text = MyQueryHSCP["HDateRange"].ToString();

            LB_CTemplateID.Text = MyQueryHSCP["HCTemplateID"].ToString();
            LB_MCourseName.Text = MyQueryHSCP["HCourseName"].ToString();
            LB_HPlaceName.Text = " 【" + MyQueryHSCP["Location"].ToString() + "】";
            LB_MDateRange.Text = MyQueryHSCP["HDateRange"].ToString();
            LB_MPMethod.Text = MyQueryHSCP["HPMethod"].ToString();

            TA_Remark.InnerText = MyQueryHSCP["HRemark"].ToString();
            // 判斷是否需要護持者，才顯示課程護持區塊
            DIV_Support.Visible = MyQueryHSCP["HNGuide"].ToString() == "0" ? false : true;

            // 判斷若課程類別屬於活動類型 HCourse_Class.HType="3"
            DIV_ActivityDetail.Visible = MyQueryHSCP["HType"].ToString() == "3" ? true : false;


            #region 課程護持

            SDS_HCGuide.SelectCommand = "SELECT a.HCourseID, a.HMemberID, (HArea+'/'+HPeriod+' '+ HUserName) AS HCGuideItem FROM HCourseBooking AS a JOIN HMember AS b ON a.HMemberID = b.HID JOIN HArea AS c ON b.HAreaID=c.HID WHERE HCourseID = '" + LB_CourseID.Text + "' AND HMemberID NOT IN('" + ((Label)Master.FindControl("LB_HUserHID")).Text + "')  GROUP BY a.HCourseID, a.HMemberID, c.HArea, b.HPeriod, b.HUserName"; /*AND b.HType IN(2)*/
            //Response.Write("SELECT a.HCourseID, a.HMemberID, (HArea+'/'+HPeriod+' '+ HUserName) AS HCGuideItem FROM HShoppingCart AS a JOIN HMember AS b ON a.HMemberID = b.HID JOIN HArea AS c ON b.HAreaID=c.HID WHERE HCourseID = '" + LB_CourseID.Text + "' AND HMemberID NOT IN('" + ((Label)Master.FindControl("LB_HUserHID")).Text + "') AND b.HType IN (2) GROUP BY a.HCourseID, a.HMemberID,HArea,HPeriod,HUserName");
            //Response.End();

            SDS_HCGuide.DataBind();
            DDL_HCGuide.DataBind();
            #endregion


            //GA20230922_如果有選擇護持者
            if (!string.IsNullOrEmpty(MyQueryHSCP["HCGuide"].ToString()))
            {
                DDL_HCGuide.SelectedValue = MyQueryHSCP["HCGuide"].ToString();
            }
            else
            {
                DDL_HCGuide.SelectedValue = "0";
            }
        }
        MyQueryHSCP.Close();






        //220823-體系專業護持組別改只抓HTodoList即可

        SDS_HGroupName.SelectCommand = "SELECT A.HGroupName AS HGroupID, B.HGroupName FROM HTodoList A LEFT JOIN HGroup B ON A.HGroupName = B.HID WHERE HCourseID = '" + LB_CourseID.Text + "' AND HSave = 1 GROUP BY A.HGroupName, B.HGroupName";
        //Response.Write("SELECT A.HGroupName AS HGroupID, B.HGroupName FROM HTodoList A LEFT JOIN HGroup B ON A.HGroupName = B.HID WHERE HCourseID = '" + LB_CourseID.Text + "' AND HSave = 1 GROUP BY A.HGroupName, B.HGroupName");
        //Response.End();
        DDL_HGroupName_Add.DataBind();




        if (!string.IsNullOrEmpty(LB_SystemID.Text))
        {
            //220818- 加入判斷，若可報名的體系全勾，表示尚未有體系的學員也可以報名
            //221109- 調整: 幫他人報名改為加入購物車，故需判斷購物車是否已存在
            if (LB_SystemID.Text == "1,2,3,4,5,6,7")
            {

                SDS_HOtherMember.SelectCommand = "SELECT HMember.HID, HMemberID, HUserName, (HArea+'/'+HPeriod+' '+ HUserName) AS HOther FROM HMember LEFT JOIN HCourseBooking  ON HMemberID=HMember.HID INNER JOIN HArea ON HAreaID=HArea.HID WHERE HMember.HID NOT IN ('" + ((Label)Master.FindControl("LB_HUserHID")).Text + "') AND ( HCourseID  NOT IN (" + LB_CourseID.Text + ") OR  HCourseID IS NULL) AND (HSystemID IN (" + LB_SystemID.Text + ") OR HSystemID is null OR HSystemID ='') AND HMember.HStatus='1'  AND HMember.HID NOT IN (SELECT HMemberID FROM HCourseBooking WHERE HCTemplateID = '" + LB_CTemplateID.Text + "' AND HCourseName = N'" + LB_MCourseName.Text + "' AND HDateRange = '" + LB_MDateRange.Text + "' )  GROUP BY  HMemberID, HUserName, HArea, HPeriod, HMember.HID";
                //SDS_HOtherMember.SelectCommand = "SELECT HMember.HID, HMemberID, HUserName, (HArea+'/'+HPeriod+' '+ HUserName) AS HOther FROM HMember LEFT JOIN HCourseBooking  ON HMemberID=HMember.HID INNER JOIN HArea ON HAreaID=HArea.HID WHERE HMember.HID NOT IN ('" + ((Label)Master.FindControl("LB_HUserHID")).Text + "') AND ( HCourseID  NOT IN (" + LB_CourseID.Text + ") OR  HCourseID IS NULL) AND (HSystemID IN (" + LB_SystemID.Text + ") OR HSystemID is null OR HSystemID ='') AND HMember.HStatus='1'  AND HMember.HID NOT IN (SELECT HMemberID FROM HCourseBooking WHERE HCourseID='" + LB_CourseID.Text + "')  GROUP BY  HMemberID, HUserName, HArea, HPeriod, HMember.HID";
            }
            else
            {
                SDS_HOtherMember.SelectCommand = "SELECT HMember.HID, HMemberID, HUserName, (HArea+'/'+HPeriod+' '+ HUserName) AS HOther FROM HMember LEFT JOIN HCourseBooking  ON HMemberID=HMember.HID INNER JOIN HArea ON HAreaID=HArea.HID WHERE HMember.HID NOT IN ('" + ((Label)Master.FindControl("LB_HUserHID")).Text + "') AND ( HCourseID  NOT IN (" + LB_CourseID.Text + ") OR  HCourseID IS NULL) AND HSystemID IN (" + LB_SystemID.Text + ") AND HMember.HStatus='1'  AND HMember.HID NOT IN (SELECT HMemberID FROM HCourseBooking WHERE  HCTemplateID = '" + LB_CTemplateID.Text + "' AND HCourseName = N'" + LB_MCourseName.Text + "' AND HDateRange = '" + LB_MDateRange.Text + "' )  GROUP BY  HMemberID, HUserName, HArea, HPeriod, HMember.HID";
                //SDS_HOtherMember.SelectCommand = "SELECT HMember.HID, HMemberID, HUserName, (HArea+'/'+HPeriod+' '+ HUserName) AS HOther FROM HMember LEFT JOIN HCourseBooking  ON HMemberID=HMember.HID INNER JOIN HArea ON HAreaID=HArea.HID WHERE HMember.HID NOT IN ('" + ((Label)Master.FindControl("LB_HUserHID")).Text + "') AND ( HCourseID  NOT IN (" + LB_CourseID.Text + ") OR  HCourseID IS NULL) AND HSystemID IN (" + LB_SystemID.Text + ") AND HMember.HStatus='1'  AND HMember.HID NOT IN (SELECT HMemberID FROM HCourseBooking WHERE HCourseID='" + LB_CourseID.Text + "')  GROUP BY  HMemberID, HUserName, HArea, HPeriod, HMember.HID";
            }

        }
        else
        {

            SDS_HOtherMember.SelectCommand = "SELECT HMember.HID, HMemberID, HUserName, (HArea+'/'+HPeriod+' '+ HUserName) AS HOther FROM HMember LEFT JOIN HCourseBooking  ON HMemberID=HMember.HID INNER JOIN HArea ON HAreaID=HArea.HID WHERE HMember.HID NOT IN ('" + ((Label)Master.FindControl("LB_HUserHID")).Text + "') AND ( HCourseID  NOT IN (" + LB_CourseID.Text + ") OR  HCourseID IS NULL) AND HMember.HStatus='1'  AND HMember.HID NOT IN (SELECT HMemberID FROM HCourseBooking WHERE  HCTemplateID = '" + LB_CTemplateID.Text + "' AND HCourseName = N'" + LB_MCourseName.Text + "' AND HDateRange = '" + LB_MDateRange.Text + "' )  GROUP BY  HMemberID, HUserName, HArea, HPeriod, HMember.HID";
            //SDS_HOtherMember.SelectCommand = "SELECT HMember.HID, HMemberID, HUserName, (HArea+'/'+HPeriod+' '+ HUserName) AS HOther FROM HMember LEFT JOIN HCourseBooking  ON HMemberID=HMember.HID INNER JOIN HArea ON HAreaID=HArea.HID WHERE HMember.HID NOT IN ('" + ((Label)Master.FindControl("LB_HUserHID")).Text + "') AND ( HCourseID  NOT IN (" + LB_CourseID.Text + ") OR  HCourseID IS NULL) AND HMember.HStatus='1'  AND HMember.HID NOT IN (SELECT HMemberID FROM HCourseBooking WHERE HCourseID='" + LB_CourseID.Text + "')  GROUP BY  HMemberID, HUserName, HArea, HPeriod, HMember.HID";
        }


        SDS_HOtherMember.DataBind();
        LBox_HOtherMember.DataBind();

        #region HShoppingCart_Group資料表-體系專業護持資訊
        ViewState["Item_temp"] = "[]";
        UpdateTable();
        List<Team.list_item> Table = JsonConvert.DeserializeObject<List<Team.list_item>>(ViewState["Item_temp"].ToString());

        string gHGDay = "";
        //SqlDataReader MyQueryHSCG = SQLdatabase.ExecuteReader("SELECT a.HID, a.HGroupID, a.HGDay, a.HGSTime, a.HGETime, a.HTaskID, a.HTask, b.HGroupName FROM HShoppingCart_Group AS a LEFT JOIN HGroup AS b ON a.HGroupID = b.HID WHERE HShoppingCartID='" + LB_ShoppingCartID.Text + "'");
        SqlDataReader MyQueryGHGDay1 = SQLdatabase.ExecuteReader("SELECT a.HGroupID FROM HShoppingCart_Group AS a LEFT JOIN HGroup AS b ON a.HGroupID = b.HID JOIN HShoppingCart AS c ON a.HShoppingCartID=c.HID WHERE HShoppingCartID='" + LB_ShoppingCartID.Text + "' GROUP BY a.HGroupID");
        //組合日期
        while (MyQueryGHGDay1.Read())
        {
            SqlDataReader MyQueryGHGDay2 = SQLdatabase.ExecuteReader("SELECT a.HGroupID, STUFF((SELECT ',' + a.HGDay FROM HShoppingCart_Group AS a LEFT JOIN HGroup AS b ON a.HGroupID = b.HID WHERE a.HShoppingCartID='" + LB_ShoppingCartID.Text + "' AND HGroupID='" + MyQueryGHGDay1["HGroupID"].ToString() + "' FOR XML PATH('')),1,1,'') AS HGDay, a.HSameAsCourse, a.HGSTime, a.HGETime, a.HTaskID, a.HTask, b.HGroupName, c.HDateRange FROM HShoppingCart_Group AS a LEFT JOIN HGroup AS b ON a.HGroupID = b.HID JOIN HShoppingCart AS c ON a.HShoppingCartID=c.HID WHERE HShoppingCartID='" + LB_ShoppingCartID.Text + "' AND HGroupID='" + MyQueryGHGDay1["HGroupID"].ToString() + "' GROUP BY a.HGroupID, a.HSameAsCourse, a.HGSTime, a.HGETime, a.HTaskID, a.HTask, b.HGroupName, c.HDateRange");

            //組合日期
            while (MyQueryGHGDay2.Read())
            {
                if (MyQueryGHGDay2["HDateRange"].ToString().IndexOf("-") >= 0 && Convert.ToBoolean(MyQueryGHGDay2["HSameAsCourse"].ToString()) == true) //同課程日期
                {
                    //gHGDay = MyQueryGHGDay2["HGDay"].ToString().Replace(",", " - ");
                    //221013_ 
                    gHGDay = MyQueryGHGDay2["HDateRange"].ToString();
                }
                else if ((MyQueryGHGDay2["HDateRange"].ToString().IndexOf("-") < 0 && MyQueryGHGDay2["HDateRange"].ToString().IndexOf(",") < 0) && (Convert.ToBoolean(MyQueryGHGDay2["HSameAsCourse"].ToString()) == true))//單一日期
                {
                    gHGDay = MyQueryGHGDay2["HGDay"].ToString().Replace(",", "");
                }
                else
                {
                    gHGDay = MyQueryGHGDay2["HGDay"].ToString();
                }
                //Response.Write("HGDay="+MyQueryHSCG["HGDay"].ToString()+"<br/>");
                Table.Add(new Team.list_item()
                {
                    HGroupName = MyQueryGHGDay2["HGroupName"].ToString(),
                    HGroupID = MyQueryGHGDay2["HGroupID"].ToString(),
                    HTaskID = MyQueryGHGDay2["HTaskID"].ToString(),
                    HTask = MyQueryGHGDay2["HTask"].ToString(),
                    HGDay = gHGDay,
                    HSameAsCourse = Convert.ToBoolean(MyQueryGHGDay2["HSameAsCourse"].ToString()),
                    HGSTime = MyQueryGHGDay2["HGSTime"].ToString(),
                    HGETime = MyQueryGHGDay2["HGETime"].ToString(),
                    Add = "1",
                    Update = "",
                });
            }
            MyQueryGHGDay2.Close();
        }
        MyQueryGHGDay1.Close();
        ViewState["Item_temp"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
        //Label1.Text = ViewState["Item_temp"].ToString();
        CreateTable();


        #endregion






        #region HShoppingCart_Other資料表-幫他人報名
        string gHMemberID = "";
        SqlDataReader MyQueryHSCO = SQLdatabase.ExecuteReader("SELECT HMemberID FROM HShoppingCart_Other WHERE HShoppingCartID='" + LB_ShoppingCartID.Text + "'");
        while (MyQueryHSCO.Read())
        {
            gHMemberID += MyQueryHSCO["HMemberID"].ToString() + ",";
        }
        string[] gHOtherMember = gHMemberID.Split(',');
        for (int i = 0; i < gHOtherMember.Length - 1; i++)
        {
            //Response.Write("LBox_HOtherMember="+LBox_HOtherMember.Items.Count+"<br/>");
            for (int j = 0; j < LBox_HOtherMember.Items.Count; j++)
            {
                if (gHOtherMember[i].ToString() == LBox_HOtherMember.Items[j].Value)
                {
                    LBox_HOtherMember.Items[j].Selected = true;
                }
            }
        }
        //Response.End();

        #endregion



        #region HShoppingCart_Activity資料表-親朋好友報名
        ViewState["Activity_temp"] = "[]";
        ActivityUpdateTable();
        List<Activity.list_item> TableActivity = JsonConvert.DeserializeObject<List<Activity.list_item>>(ViewState["Activity_temp"].ToString());

        SqlDataReader MyQueryHSCA = SQLdatabase.ExecuteReader("SELECT a.HAName, a.HASex, a.HAAge, a.HARelation, b.HRelation FROM HShoppingCart_Activity AS a LEFT JOIN HRelation AS b ON a.HARelation=b.HID WHERE HShoppingCartID='" + LB_ShoppingCartID.Text + "'");
        while (MyQueryHSCA.Read())
        {
            TableActivity.Add(new Activity.list_item()
            {
                HAName = MyQueryHSCA["HAName"].ToString(),
                HASex = MyQueryHSCA["HASex"].ToString() == "1" ? "男" : "女",
                HASexID = MyQueryHSCA["HASex"].ToString(),
                HAAge = MyQueryHSCA["HAAge"].ToString(),
                HARelation = MyQueryHSCA["HRelation"].ToString(),
                HARelationID = MyQueryHSCA["HARelation"].ToString(),
                Add = "1",
                Update = "",
            });
        }
        MyQueryHSCA.Close();


        ViewState["Activity_temp"] = JsonConvert.SerializeObject(TableActivity, Formatting.Indented);
        ActivityCreateTable();

        TB_HAName.Text = null;
        TB_HAAge.Text = null;
        DDL_HARelation.SelectedValue = "0";

        RegisterAsync();//註冊非同步事件

        #endregion


        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Edit').modal('show');</script>", false);//執行js

    }
    #endregion

    #region 同上課日期勾選
    protected void CB_HSameAsCourse_Add_CheckedChanged(object sender, EventArgs e)
    {
        //RepeaterItem item = (sender as CheckBox).NamingContainer as RepeaterItem;
        //((TextBox)item.FindControl("TB_HGDay")).Text=((Label)item.FindControl("LB_HDateRange")).Text;

        //if (CB_HSameAsCourse_Add.Checked == true && TB_HGDay_Add.Text == "")
        if (CB_HSameAsCourse_Add.Checked == true)
        {
            TB_HGDay_Add.Text = LB_HDateRange_Add.Text;
            //221013- 
            TB_HGDay_Add.Enabled = false;
            TB_HGDay_Add.CssClass = "datemultipicker form-control";
        }
        else
        {
            TB_HGDay_Add.Text = "";
            //221013- 
            TB_HGDay_Add.Enabled = true;
            TB_HGDay_Add.CssClass = "datemultipicker form-control disabledate";
        }



        RegisterAsync();//註冊非同步事件
    }
    #endregion

    #region 護持項目

    #region 護持項目-新增功能
    protected void LBtn_HTeam_Add_Click(object sender, EventArgs e)
    {


        //Label1.Text="ViewState[Item_temp]="+ViewState["Item_temp"];
        //Response.End();
        //220825-加入必填判斷
        if (DDL_HGroupName_Add.SelectedValue == "0" || DDL_HTask_Add.SelectedValue == "0" || string.IsNullOrEmpty(TB_HGDay_Add.Text.Trim()) || string.IsNullOrEmpty(TB_GSTime_Add.Text.Trim()) || string.IsNullOrEmpty(TB_GETime_Add.Text.Trim()))
        {
            //Response.Write("<script>alert('請選擇體系護持組別並填寫完整護持日期與時間才能新增哦~');</script>");
            //return;

            //221003-updatepanel alert寫法
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "message", "alert('請選擇體系護持組別並填寫完整護持日期與時間才能新增哦~');", true);

            RegisterAsync();//註冊非同步事件
        }
        else
        {

            UpdateTable();

            List<Team.list_item> Table = JsonConvert.DeserializeObject<List<Team.list_item>>(ViewState["Item_temp"].ToString());

            for (int i = 0; i < Table.Count; i++)
            {
                if (Table[i].HGroupID == DDL_HGroupName_Add.SelectedValue && Table[i].HTaskID == DDL_HTask_Add.SelectedValue)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "message", "alert('相同體系專業護持組別已經存在了喔~');", true);
                    return;
                }
            }








            Table.Add(new Team.list_item()
            {
                HGroupName = DDL_HGroupName_Add.SelectedItem.Text,
                HGroupID = DDL_HGroupName_Add.SelectedValue,
                HTaskID = DDL_HTask_Add.SelectedValue,
                HTask = DDL_HTask_Add.SelectedItem.Text,
                HGDay = TB_HGDay_Add.Text.Trim(),
                //HGDay = DDL_HGDay_Add.SelectedValue,  //210710-註解
                HSameAsCourse = CB_HSameAsCourse_Add.Checked,
                HGSTime = TB_GSTime_Add.Text,
                HGETime = TB_GETime_Add.Text,
                Add = "1",
                Update = "",
            });

            ViewState["Item_temp"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
            CreateTable();

            DDL_HGroupName_Add.SelectedValue = "0";
            DDL_HTask_Add.SelectedValue = "0";
            //DDL_HGDay_Add.SelectedValue = "0"; //210710-註解
            TB_HGDay_Add.Text = null;
            TB_HGDay_Add.CssClass = "datemultipicker form-control disabledate";
            TB_HGDay_Add.Enabled = true;
            TB_GSTime_Add.Text = null;
            TB_GETime_Add.Text = null;
            CB_HSameAsCourse_Add.Checked = false;

            RegisterAsync();//註冊非同步事件
        }


    }
    #endregion

    #region  護持項目更新
    public void UpdateTable()
    {
        //RegScript();//執行js

        List<Team.list_item> Table = JsonConvert.DeserializeObject<List<Team.list_item>>(ViewState["Item_temp"].ToString());
        int x = 0;
        foreach (Team.list_item Element in Table)
        {

            if (((Label)Rpt_Support.Items[x].FindControl("LB_HGroupName")).Text != Element.HGroupName)
            {
                Element.HGroupName = ((Label)Rpt_Support.Items[x].FindControl("LB_HGroupName")).Text;
                Element.Update = "1";
            }

            if (((Label)Rpt_Support.Items[x].FindControl("LB_HTask")).Text != Element.HTask)
            {
                Element.HTask = ((Label)Rpt_Support.Items[x].FindControl("LB_HTask")).Text;
                Element.Update = "1";
            }
            if (((TextBox)Rpt_Support.Items[x].FindControl("TB_HGDay")).Text != Element.HGDay)
            {
                Element.HGDay = ((TextBox)Rpt_Support.Items[x].FindControl("TB_HGDay")).Text;
                Element.Update = "1";
            }
            if (((CheckBox)Rpt_Support.Items[x].FindControl("CB_HSameAsCourse")).Checked != Element.HSameAsCourse)
            {
                Element.HSameAsCourse = ((CheckBox)Rpt_Support.Items[x].FindControl("CB_HSameAsCourse")).Checked;
                Element.Update = "1";
            }
            if (((TextBox)Rpt_Support.Items[x].FindControl("TB_GSTime")).Text != Element.HGSTime)
            {
                Element.HGSTime = ((TextBox)Rpt_Support.Items[x].FindControl("TB_GSTime")).Text;
                Element.Update = "1";
            }
            if (((TextBox)Rpt_Support.Items[x].FindControl("TB_GETime")).Text != Element.HGETime)
            {
                Element.HGETime = ((TextBox)Rpt_Support.Items[x].FindControl("TB_GETime")).Text;
                Element.Update = "1";
            }

            x++;
        }
        ViewState["Item_temp"] = JsonConvert.SerializeObject(Table, Formatting.Indented);


    }
    #endregion

    #region 建立
    public void CreateTable()
    {
        if (ViewState["Item_temp"] != null && !string.IsNullOrEmpty(ViewState["Item_temp"].ToString()))
        {
            List<Team.list_item> Table = JsonConvert.DeserializeObject<List<Team.list_item>>(ViewState["Item_temp"].ToString());
            Rpt_Support.DataSource = Table;
            Rpt_Support.DataBind();

            int x = 0;
            foreach (Team.list_item Element in Table)
            {
                //((DropDownList)Rpt_Support.Items[x].FindControl("DDL_HGroupName")).SelectedValue = Element.HGroupName;
                //((DropDownList)Rpt_Support.Items[x].FindControl("DDL_HTask")).SelectedValue = Element.HTask;
                //((DropDownList)Rpt_Support.Items[x].FindControl("DDL_HGDay")).SelectedValue = Element.HGDay;  //--210710- 註解
                //((TextBox)Rpt_Support.Items[x].FindControl("TB_HGDay")).Text = Element.HGDay;
                //x++;
            }

        }

    }
    #endregion

    #region 護持項目-刪除功能
    protected void LBtn_Del_Click(object sender, EventArgs e)
    {
        UpdateTable();

        LinkButton Del = sender as LinkButton;
        string Del_CA = Del.CommandArgument;

        List<Team.list_item> Table = JsonConvert.DeserializeObject<List<Team.list_item>>(ViewState["Item_temp"].ToString());
        if (!string.IsNullOrEmpty(((Label)Rpt_Support.Items[Convert.ToInt32(Del_CA)].FindControl("LB_HID")).Text))
        {
            ViewState["Item_Del"] += ((Label)Rpt_Support.Items[Convert.ToInt32(Del_CA)].FindControl("LB_HID")).Text + ",";
        }
        Table.RemoveAt(Convert.ToInt32(Del_CA));
        ViewState["Item_temp"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
        CreateTable();

        RegisterAsync();//註冊非同步事件
    }
    #endregion


    #region 體系專業護持組別選項
    protected void DDL_HGroupName_Add_SelectedIndexChanged(object sender, EventArgs e)
    {
        DDL_HTask_Add.Items.Clear();
        DDL_HTask_Add.Enabled = true;
        DDL_HTask_Add.Items.Add(new ListItem("請選擇", "0"));

        //220823--體系專業護持組別改只抓HTodoList即可
        SqlDataReader QueryHCourseTask = SQLdatabase.ExecuteReader("SELECT B.HID,HGroupName, B.HTask, B.HTaskContent, HGroupLeaderID, HSave FROM  HTodoList A INNER JOIN HCourseTask B ON A.HTask = B.HID where HCourseID = '" + LB_CourseID.Text + "' AND HGroupID='" + DDL_HGroupName_Add.SelectedValue + "' AND A.HSave ='1'");
        while (QueryHCourseTask.Read())
        {
            DDL_HTask_Add.Items.Add(new ListItem(QueryHCourseTask["HTask"].ToString(), (QueryHCourseTask["HID"].ToString())));
        }
        QueryHCourseTask.Close();

        RegScript(); //執行js

        //ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Edit').modal('show');</script>", false);//執行js
        //return;
    }


    #endregion

    #endregion

    #region 幫他人報名(非必填)

    //#region 幫他人報名-新增功能
    //protected void LBtn_OtherBooking_Click(object sender, EventArgs e)
    //{

    //	//220825-加入必填判斷
    //	if (LBox_HOtherMember.SelectedValue == "0")
    //	{
    //		Response.Write("<script>alert('請先選擇學員姓名哦~');</script>");
    //		return;
    //	}
    //	else
    //	{

    //		OtherUpdateTable();

    //		List<Other.list_item> Table = JsonConvert.DeserializeObject<List<Other.list_item>>(ViewState["Other_temp"].ToString());

    //		Table.Add(new Other.list_item()
    //		{
    //			HOtherMember = LBox_HOtherMember.SelectedValue,
    //			Add = "1",
    //			Update = "",
    //		});

    //		ViewState["Other_temp"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
    //		OtherCreateTable();



    //		LBox_HOtherMember.Items.Clear();
    //		LBox_HOtherMember.Items.Add(new ListItem("請選擇幫報名學員", "0"));
    //		SDS_HOtherMember.SelectCommand = "SELECT HMember.HID, HMemberID, HUserName, (HArea+'/'+HPeriod+' '+ HUserName) AS HOther FROM HMember LEFT JOIN HCourseBooking  ON HMemberID=HMember.HID INNER JOIN HArea ON HAreaID=HArea.HID WHERE HMember.HID NOT IN (" + ((Label)Master.FindControl("LB_HUserHID")).Text + ") AND ( HCourseID  NOT IN (" + LB_CourseID.Text + ") OR  HCourseID IS NULL) AND HSystemID IN (" + LB_SystemID.Text + ") AND HMember.HStatus='1'  AND HMember.HID NOT IN (SELECT HMemberID FROM HCourseBooking WHERE HCourseID='" + LB_CourseID.Text + "')  GROUP BY  HMemberID, HUserName, HArea, HPeriod, HMember.HID";
    //           //Response.Write();
    //		SDS_HOtherMember.DataBind();

    //		RegisterAsync();//註冊非同步事件
    //	}
    //}

    //#endregion

    //#region  幫他人報名更新
    //public void OtherUpdateTable()
    //{
    //	//RegScript();//執行js

    //	List<Other.list_item> Table = JsonConvert.DeserializeObject<List<Other.list_item>>(ViewState["Other_temp"].ToString());
    //	int x = 0;
    //	foreach (Other.list_item Element in Table)
    //	{
    //		if (((DropDownList)Rpt_Other.Items[x].FindControl("LBox_HOtherMember")).SelectedValue != Element.HOtherMember)
    //		{
    //			Element.HOtherMember = ((DropDownList)Rpt_Other.Items[x].FindControl("LBox_HOtherMember")).SelectedValue;
    //			Element.Update = "1";
    //		}
    //		if (((Label)Rpt_Other.Items[x].FindControl("LB_HOtherMemberID")).Text != Element.HOtherMember)
    //		{
    //			Element.HOtherMember = ((Label)Rpt_Other.Items[x].FindControl("LB_HOtherMemberID")).Text;
    //			Element.Update = "1";
    //		}
    //		x++;
    //	}
    //	ViewState["Other_temp"] = JsonConvert.SerializeObject(Table, Formatting.Indented);

    //}
    //#endregion

    //#region 建立
    //public void OtherCreateTable()
    //{
    //	if (ViewState["Other_temp"] != null && !string.IsNullOrEmpty(ViewState["Other_temp"].ToString()))
    //	{
    //		List<Other.list_item> Table = JsonConvert.DeserializeObject<List<Other.list_item>>(ViewState["Other_temp"].ToString());
    //		Rpt_Other.DataSource = Table;
    //		Rpt_Other.DataBind();

    //		int x = 0;
    //		foreach (Other.list_item Element in Table)
    //		{
    //			((DropDownList)Rpt_Other.Items[x].FindControl("LBox_HOtherMember")).SelectedValue = Element.HOtherMember;
    //			x++;
    //		}
    //	}

    //}
    //#endregion

    //#region 幫他人報名-刪除功能
    //protected void LBtn_OtherDel_Click(object sender, EventArgs e)
    //{
    //	OtherUpdateTable();

    //	LinkButton Del = sender as LinkButton;
    //	string Del_CA = Del.CommandArgument;

    //	List<Other.list_item> Table = JsonConvert.DeserializeObject<List<Other.list_item>>(ViewState["Other_temp"].ToString());
    //	if (!string.IsNullOrEmpty(((Label)Rpt_Other.Items[Convert.ToInt32(Del_CA)].FindControl("LB_HOtherMemberID")).Text))
    //	{
    //		ViewState["Other_Del"] += ((Label)Rpt_Other.Items[Convert.ToInt32(Del_CA)].FindControl("LB_HOtherMemberID")).Text + ",";
    //	}
    //	Table.RemoveAt(Convert.ToInt32(Del_CA));
    //	ViewState["Other_temp"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
    //	OtherCreateTable();

    //	LBox_HOtherMember.Items.Clear();
    //	LBox_HOtherMember.Items.Add(new ListItem("請選擇幫報名學員", "0"));
    //	SDS_HOtherMember.SelectCommand = "SELECT HMember.HID, HMemberID, HUserName, (HArea+'/'+HPeriod+' '+ HUserName) AS HOther FROM HMember LEFT JOIN HCourseBooking  ON HMemberID=HMember.HID INNER JOIN HArea ON HAreaID=HArea.HID WHERE HMember.HID NOT IN (" + ((Label)Master.FindControl("LB_HUserHID")).Text + ") AND ( HCourseID  NOT IN (" + LB_CourseID.Text + ") OR  HCourseID IS NULL) AND HSystemID IN (" + LB_SystemID.Text + ") AND HMember.HStatus='1'  AND HMember.HID NOT IN (SELECT HMemberID FROM HCourseBooking WHERE HCourseID='" + LB_CourseID.Text + "')  GROUP BY  HMemberID, HUserName, HArea, HPeriod, HMember.HID";
    //	SDS_HOtherMember.DataBind();

    //	RegisterAsync();//註冊非同步事件
    //}

    //#endregion

    #endregion

    #region 活動-親朋好友報名

    #region 親朋好友報名-新增功能
    protected void LBtn_Activity_Add_Click(object sender, EventArgs e)
    {
        ActivityUpdateTable();
        List<Activity.list_item> Table = JsonConvert.DeserializeObject<List<Activity.list_item>>(ViewState["Activity_temp"].ToString());

        Table.Add(new Activity.list_item()
        {
            HAName = TB_HAName.Text,
            HASex = DDL_HASex.SelectedItem.Text,
            HASexID = DDL_HASex.SelectedValue,
            HAAge = TB_HAAge.Text,
            HARelation = DDL_HARelation.SelectedItem.Text,
            HARelationID = DDL_HARelation.SelectedValue,
            Add = "1",
            Update = "",
        });

        ViewState["Activity_temp"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
        ActivityCreateTable();

        TB_HAName.Text = null;
        TB_HAAge.Text = null;
        DDL_HARelation.SelectedValue = "0";

        RegisterAsync();//註冊非同步事件
    }
    #endregion

    #region 親朋好友報名更新
    public void ActivityUpdateTable()
    {
        List<Activity.list_item> Table = JsonConvert.DeserializeObject<List<Activity.list_item>>(ViewState["Activity_temp"].ToString());
        int x = 0;
        foreach (Activity.list_item Element in Table)
        {
            if (((Label)Rpt_Activity.Items[x].FindControl("LB_HAName")).Text != Element.HAName)
            {
                Element.HAName = ((Label)Rpt_Activity.Items[x].FindControl("LB_HAName")).Text;
                Element.Update = "1";
            }
            if (((Label)Rpt_Activity.Items[x].FindControl("LB_HASex")).Text != Element.HASex)
            {
                Element.HASex = ((Label)Rpt_Activity.Items[x].FindControl("LB_HASex")).Text;
                Element.Update = "1";
            }
            if (((Label)Rpt_Activity.Items[x].FindControl("LB_HASexID")).Text != Element.HASexID)
            {
                Element.HASexID = ((Label)Rpt_Activity.Items[x].FindControl("LB_HASexID")).Text;
                Element.Update = "1";
            }
            if (((Label)Rpt_Activity.Items[x].FindControl("LB_HAAge")).Text != Element.HAAge)
            {
                Element.HAAge = ((Label)Rpt_Activity.Items[x].FindControl("LB_HAAge")).Text;
                Element.Update = "1";
            }
            if (((Label)Rpt_Activity.Items[x].FindControl("LB_HARelation")).Text != Element.HARelation)
            {
                Element.HARelation = ((Label)Rpt_Activity.Items[x].FindControl("LB_HARelation")).Text;
                Element.Update = "1";
            }
            if (((Label)Rpt_Activity.Items[x].FindControl("LB_HARelationID")).Text != Element.HARelationID)
            {
                Element.HARelationID = ((Label)Rpt_Activity.Items[x].FindControl("LB_HARelationID")).Text;
                Element.Update = "1";
            }
            x++;
        }
        ViewState["Activity_temp"] = JsonConvert.SerializeObject(Table, Formatting.Indented);

    }

    #endregion



    #region 建立
    public void ActivityCreateTable()
    {
        if (ViewState["Activity_temp"] != null && !string.IsNullOrEmpty(ViewState["Activity_temp"].ToString()))
        {
            List<Activity.list_item> Table = JsonConvert.DeserializeObject<List<Activity.list_item>>(ViewState["Activity_temp"].ToString());
            Rpt_Activity.DataSource = Table;
            Rpt_Activity.DataBind();

            if (Rpt_Activity.Items.Count != 0)
            {
                int activityfee = 0;
                activityfee = 500 * (Rpt_Activity.Items.Count + 1);
                //LB_HBCPoint.Text = activityfee.ToString();
                //TB_HDonate.Text = LB_HBCPoint.Text;
                //LB_Fee.Text = LB_HBCPoint.Text + "元以上";
                //LB_Subtotal.Text = ConvertFare.ToThousands(Convert.ToInt32(TB_HDonate.Text));
                //LB_Sub.Text = (Convert.ToInt32(TB_HDonate.Text)).ToString();
                //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - HDisPoint);//總計=小計-折扣碼金額
                //																				  //LB_Total.Text = ConvertFare.ToThousands(Convert.ToInt32(LB_Sub.Text) - Convert.ToInt32(LB_ExemptionTotal.Text) - HDisPoint);//總計=小計-前導課程減免總金額-折扣碼金額
            }

        }

    }
    #endregion



    #region 親朋好友報名-刪除功能
    protected void LBtn_ActivityDel_Click(object sender, EventArgs e)
    {
        ActivityUpdateTable();

        LinkButton Del = sender as LinkButton;
        string Del_CA = Del.CommandArgument;

        List<Activity.list_item> Table = JsonConvert.DeserializeObject<List<Activity.list_item>>(ViewState["Activity_temp"].ToString());
        if (!string.IsNullOrEmpty(((Label)Rpt_Activity.Items[Convert.ToInt32(Del_CA)].FindControl("LB_HID")).Text))
        {
            ViewState["Activity_Del"] += ((Label)Rpt_Activity.Items[Convert.ToInt32(Del_CA)].FindControl("LB_HID")).Text + ",";
        }
        Table.RemoveAt(Convert.ToInt32(Del_CA));
        ViewState["Activity_temp"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
        ActivityCreateTable();

        RegisterAsync();//註冊非同步事件
    }
    #endregion

    #region 資料繫結
    protected void Rpt_Activity2_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        #region 性別
        if (((Label)e.Item.FindControl("LB_HASex")).Text == "1")
        {
            ((Label)e.Item.FindControl("LB_HASex")).Text = "男";
        }
        else
        {
            ((Label)e.Item.FindControl("LB_HASex")).Text = "女";
        }
        #endregion
    }
    #endregion
    #endregion

    #region 填寫資料-確認資料功能
    protected void LBtn_Submit_Click(object sender, EventArgs e)
    {
       


        string HID = null;    //訂單HID

        #region 更新購物車(一對一護持)
        string strInsHQ = "UPDATE HShoppingCart SET HCGuide=@HCGuide ,HRemark=@HRemark WHERE HID=@HID";
        SqlCommand cmd = new SqlCommand(strInsHQ, con);
        con.Open();
        cmd.Parameters.AddWithValue("@HID", LB_ShoppingCartID.Text);
        cmd.Parameters.AddWithValue("@HCGuide", DDL_HCGuide.SelectedValue);
        cmd.Parameters.AddWithValue("@HRemark", TA_Remark.InnerText);
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        //HID = cmd.ExecuteScalar().ToString();
        con.Close();
        cmd.Cancel();

        #endregion



        #region 體系專業護持資訊
        //體系專業護持資訊-寫入dbo.HShoppingCart_Group
        SqlCommand cmdDelHSCG = new SqlCommand("Delete HShoppingCart_Group WHERE HShoppingCartID='" + LB_ShoppingCartID.Text + "'", con);
        con.Open();
        cmdDelHSCG.ExecuteNonQuery();
        cmdDelHSCG.Cancel();
        con.Close();
        for (int x = 0; x < Rpt_Support.Items.Count; x++)
        {

            //一個日期一筆資料寫入資料表
            string gSDate = "";
            string gEDate = "";
            int gHGDay = 0;//判斷天數相差幾天、連續天數
            string[] gHGDayA = { };

            if (((TextBox)Rpt_Support.Items[x].FindControl("TB_HGDay")).Text.IndexOf("-") >= 0)
            {
                string[] gDateArray = ((TextBox)Rpt_Support.Items[x].FindControl("TB_HGDay")).Text.Split('-');
                string gDate = "";
                gSDate = (gDateArray[0]);
                gEDate = (gDateArray[1]);
                gHGDay = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                TableCell[] HCell = new TableCell[gHGDayA.Length];
                for (int i = 0; i < gHGDay; i++)
                {
                    gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                }

                gHGDayA = gDate.Trim(',').Split(',');
            }
            else if (((TextBox)Rpt_Support.Items[x].FindControl("TB_HGDay")).Text.IndexOf(",") >= 0)
            {
                gHGDayA = ((TextBox)Rpt_Support.Items[x].FindControl("TB_HGDay")).Text.Trim(',').Split(',');
                gSDate = (gHGDayA[0]);
                gEDate = (gHGDayA[gHGDayA.Length - 1]);

                gHGDay = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
            }
            else if (((TextBox)Rpt_Support.Items[x].FindControl("TB_HGDay")).Text.IndexOf("-") < 0 && ((TextBox)Rpt_Support.Items[x].FindControl("TB_HGDay")).Text.IndexOf(",") < 0)//單一日期
            {
                string[] gDateArray = (((TextBox)Rpt_Support.Items[x].FindControl("TB_HGDay")).Text + " - " + ((TextBox)Rpt_Support.Items[x].FindControl("TB_HGDay")).Text).Split('-');
                string gDate = "";
                gSDate = (gDateArray[0]);
                gEDate = (gDateArray[1]);
                gHGDay = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                TableCell[] HCell = new TableCell[gHGDayA.Length];
                for (int i = 0; i < gHGDay; i++)
                {
                    gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                }

                gHGDayA = gDate.Trim(',').Split(',');
            }
            else
            {

                //Response.Write("<script>alert('日期錯誤!');window.location.href='HShoppingCart.aspx';</script>");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Notice3", "alert('日期錯誤~'); window.location.href='HShoppingCart.aspx';", true);
                return;
            }

            for (int i = 0; i < gHGDayA.Length; i++)
            {
                SqlCommand cmdDetail = new SqlCommand("INSERT INTO HShoppingCart_Group (HShoppingCartID, HGroupID, HTaskID, HTask, HGDay, HSameAsCourse, HGSTime, HGETime, HStatus, HCreate, HCreateDT) values (@HShoppingCartID, @HGroupID, @HTaskID, @HTask, @HGDay, @HSameAsCourse, @HGSTime, @HGETime, @HStatus, @HCreate, @HCreateDT)", con);
                con.Open();
                cmdDetail.Parameters.AddWithValue("@HShoppingCartID", LB_ShoppingCartID.Text);
                cmdDetail.Parameters.AddWithValue("@HGroupID", ((Label)Rpt_Support.Items[x].FindControl("LB_HGroupID")).Text);
                cmdDetail.Parameters.AddWithValue("@HTaskID", ((Label)Rpt_Support.Items[x].FindControl("LB_HTaskID")).Text);
                cmdDetail.Parameters.AddWithValue("@HTask", ((Label)Rpt_Support.Items[x].FindControl("LB_HTask")).Text);
                cmdDetail.Parameters.AddWithValue("@HGDay", gHGDayA[i]);
                cmdDetail.Parameters.AddWithValue("@HSameAsCourse", ((CheckBox)Rpt_Support.Items[x].FindControl("CB_HSameAsCourse")).Checked);
                cmdDetail.Parameters.AddWithValue("@HGSTime", ((TextBox)Rpt_Support.Items[x].FindControl("TB_GSTime")).Text);
                cmdDetail.Parameters.AddWithValue("@HGETime", ((TextBox)Rpt_Support.Items[x].FindControl("TB_GETime")).Text);
                cmdDetail.Parameters.AddWithValue("@HStatus", "1");
                cmdDetail.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmdDetail.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                cmdDetail.ExecuteNonQuery();
                con.Close();
                cmdDetail.Cancel();

            }

        }
        //體系專業護持資訊--刪除
        string[] Del = ViewState["Item_Del"].ToString().TrimEnd(',').Split(',');
        for (int i = 0; i < Del.Length; i++)
        {
            cmd = new SqlCommand("DELETE FROM HShoppingCart_Group WHERE HID=@HID", con);
            con.Open();
            cmd.Parameters.AddWithValue("@HID", Del[i]);
            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Cancel();
        }
        #endregion



        #region 幫他人報名

        //幫他人報名-寫入HShoppingCart_Other
        SqlCommand cmdDelHSCO = new SqlCommand("Delete HShoppingCart_Other WHERE HShoppingCartID='" + LB_ShoppingCartID.Text + "'", con);
        con.Open();
        cmdDelHSCO.ExecuteNonQuery();
        cmdDelHSCO.Cancel();
        con.Close();
        //取ListBox的值，使用ForEach方式
        string gLBox_HOtherMember = "";
        foreach (ListItem LBoxHOtherMember in LBox_HOtherMember.Items)
        {
            if (LBoxHOtherMember.Selected == true)
            {
                //gLBox_HOtherMember += LBoxHOtherMember.Value + ",";
                SqlCommand cmdOther = new SqlCommand("INSERT INTO HShoppingCart_Other (HShoppingCartID, HCourseID, HMemberID, HStatus, HCreate, HCreateDT, HMailStatus) values (@HShoppingCartID, @HCourseID, @HMemberID, @HStatus, @HCreate, @HCreateDT, @HMailStatus)", con);
                con.Open();
                cmdOther.Parameters.AddWithValue("@HShoppingCartID", LB_ShoppingCartID.Text);
                cmdOther.Parameters.AddWithValue("@HCourseID", LB_CourseID.Text);
                cmdOther.Parameters.AddWithValue("@HMemberID", LBoxHOtherMember.Value); //((DropDownList)Rpt_Other.Items[x].FindControl("DDL_HOtherMember")).SelectedValue);
                cmdOther.Parameters.AddWithValue("@HStatus", 1);
                cmdOther.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmdOther.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                cmdOther.Parameters.AddWithValue("@HMailStatus", 0);
                cmdOther.ExecuteNonQuery();
                con.Close();
                cmdOther.Cancel();


                //221109-  新增
                #region 寫入他人的購物車 
                SqlDataReader QuerySelSC = SQLdatabase.ExecuteReader("SELECT a.HID, a.HCTemplateID, a.HCourseName, a.HDateRange, b.HPMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCourseID=b.HID WHERE a.HCTemplateID='" + LB_CTemplateID.Text + "' AND a.HCourseName= N'" + LB_MCourseName.Text + "' AND a.HDateRange='" + LB_MDateRange.Text + "' AND a.HMemberID='" + LBoxHOtherMember.Value + "' and a.HCourseDonate='0'");
                //Response.Write("SELECT a.HID, b.HPMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCourseID=b.HID WHERE a.HCourseID='" + gCourseBooking_CA + "' and a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' and a.HCourseDonate='0'");
                //Response.End();
                if (QuerySelSC.Read())
                {
                    //更新資料庫
                    SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HCourseID=@HCourseID, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID =@HID and HCourseDonate = '0' and HMemberID=@HMemberID", SQLdatabase.OpenConnection());
                    QueryUpdSC.Parameters.AddWithValue("@HID", QuerySelSC["HID"].ToString());
                    QueryUpdSC.Parameters.AddWithValue("@HCourseID", LB_CourseID.Text);
                    QueryUpdSC.Parameters.AddWithValue("@HMemberID", LBoxHOtherMember.Value);
                    QueryUpdSC.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    QueryUpdSC.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    QueryUpdSC.ExecuteNonQuery();
                    QueryUpdSC.Cancel();
                    SQLdatabase.OpenConnection().Close();
                    //ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('此課程已經報名了喔!');", true);
                }
                else
                {
                    //寫入資料庫
                    SqlCommand QueryInsSC = new SqlCommand("INSERT INTO HShoppingCart (HOrderNum, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HSelect, HCourseDonate, HStatus, HCreate, HCreateDT) VALUES (@HOrderNum, @HTradeNo, @HMerchantTradeNo, @HMemberID, @HCourseID, @HCTemplateID, @HCourseName, @HDateRange, @HPMethod, @HAttend, @HLodging, @HBDate, @HLDate, @HLCourse, @HLCourseName, @HLDiscount, @HCGuide, @HPayMethod, @HPoint, @HMemberGroup, @HDharmaPass, @HRoom, @HRoomTime, @HSubscribe, @HDCode, @HDPoint, @HPaymentNo, @HExpireDate, @HFailReason, @HPaymentDate, @HPayAmt, @HFinanceRemark, @HInvoiceNo, @HInvoiceDate, @HInvoiceStatus, @HRemark, @HSelect, @HCourseDonate, @HStatus, @HCreate, @HCreateDT)", SQLdatabase.OpenConnection());
                    QueryInsSC.Parameters.AddWithValue("@HOrderNum", "");
                    QueryInsSC.Parameters.AddWithValue("@HTradeNo", "");
                    QueryInsSC.Parameters.AddWithValue("@HMerchantTradeNo", "");
                    QueryInsSC.Parameters.AddWithValue("@HMemberID", LBoxHOtherMember.Value);
                    QueryInsSC.Parameters.AddWithValue("@HCourseID", LB_CourseID.Text);
                    QueryInsSC.Parameters.AddWithValue("@HCTemplateID", LB_CTemplateID.Text);
                    QueryInsSC.Parameters.AddWithValue("@HCourseName", LB_MCourseName.Text);
                    QueryInsSC.Parameters.AddWithValue("@HDateRange", LB_MDateRange.Text);
                    QueryInsSC.Parameters.AddWithValue("@HPMethod", LB_MPMethod.Text); ;
                    QueryInsSC.Parameters.AddWithValue("@HAttend", "");
                    QueryInsSC.Parameters.AddWithValue("@HLodging", "");
                    QueryInsSC.Parameters.AddWithValue("@HBDate", "");
                    QueryInsSC.Parameters.AddWithValue("@HLDate", "");
                    QueryInsSC.Parameters.AddWithValue("@HLCourse", "");
                    QueryInsSC.Parameters.AddWithValue("@HLCourseName", "");
                    QueryInsSC.Parameters.AddWithValue("@HLDiscount", "");
                    QueryInsSC.Parameters.AddWithValue("@HCGuide", "");
                    QueryInsSC.Parameters.AddWithValue("@HPayMethod", "");
                    QueryInsSC.Parameters.AddWithValue("@HPoint", "");
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
        }
        #endregion





        #region 活動-親朋好友報名
        //活動-親朋好友報名-寫入HShoppingCart_Activity
        SqlCommand cmdDelHSCA = new SqlCommand("Delete HShoppingCart_Activity WHERE HShoppingCartID='" + LB_ShoppingCartID.Text + "'", con);
        con.Open();
        cmdDelHSCA.ExecuteNonQuery();
        cmdDelHSCA.Cancel();
        con.Close();
        for (int x = 0; x < Rpt_Activity.Items.Count; x++)
        {
            SqlCommand cmdDetail = new SqlCommand("INSERT INTO HShoppingCart_Activity (HShoppingCartID, HMemberID, HAName, HASex, HAAge, HARelation, HAmount, HStatus, HCreate, HCreateDT) values (@HShoppingCartID, @HMemberID, @HAName, @HASex, @HAAge, @HARelation, @HAmount, @HStatus, @HCreate, @HCreateDT)", con);
            con.Open();
            cmdDetail.Parameters.AddWithValue("@HShoppingCartID", LB_ShoppingCartID.Text);
            cmdDetail.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmdDetail.Parameters.AddWithValue("@HAName", ((Label)Rpt_Activity.Items[x].FindControl("LB_HAName")).Text);
            cmdDetail.Parameters.AddWithValue("@HASex", ((Label)Rpt_Activity.Items[x].FindControl("LB_HASexID")).Text);
            cmdDetail.Parameters.AddWithValue("@HAAge", ((Label)Rpt_Activity.Items[x].FindControl("LB_HAAge")).Text);
            cmdDetail.Parameters.AddWithValue("@HARelation", ((Label)Rpt_Activity.Items[x].FindControl("LB_HARelationID")).Text);
            cmdDetail.Parameters.AddWithValue("@HAmount", "500");
            cmdDetail.Parameters.AddWithValue("@HStatus", "1");
            cmdDetail.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmdDetail.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmdDetail.ExecuteNonQuery();
            con.Close();
            cmdDetail.Cancel();
        }
        //活動-親朋好友報名--刪除
        string[] ActivityDel = ViewState["Activity_Del"].ToString().TrimEnd(',').Split(',');
        for (int i = 0; i < ActivityDel.Length; i++)
        {
            cmd = new SqlCommand("DELETE FROM HShoppingCart_Activity WHERE HID=@HID", con);
            con.Open();
            cmd.Parameters.AddWithValue("@HID", ActivityDel[i]);
            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Cancel();
        }
        #endregion



    }
    #endregion
    #endregion

    #region 確認結帳
    protected void Btn_CheckOut_Click(object sender, EventArgs e)
    {

        if (Session["Step"] != null)
        {
            Session["Step"] = "3";
        }

        if (Session["SubmitCheck"] != null)
        {
            if (Session["SubmitCheck"].ToString() == "0")
            {
                Response.Write("<script>alert('請勿重新送出訂單，將為您導到未結訂單頁面。'); window.location.href='HMember_Order.aspx'; </script>");
                return;
            }
        }
        else
        {
            Response.Write("<script>alert('畫面停留太久，請重新操作報名流程哦~謝謝~!');window.location.href='HShoppingCart.aspx';</script>");
        }

        #region 必填判斷
        if (LB_Total.Text == "0")
        {
            DDL_HPayMethod.SelectedValue = "0";
        }
        else if (LB_Total.Text != "0" && LB_Remain.Text != "0" && LB_Difference.Text == "0" && LB_Navtab.Text == "2")  //文化的結帳，剩餘點數!=0且尚補差額==0
        {
            DDL_HPayMethod.SelectedValue = "0";
        }
        else
        {
            if (DDL_HPayMethod.SelectedValue == "0")
            {
                Response.Write("<script>alert('請選擇付款方式哦~');</script>");
                return;
            }
        }

        #region 提示總金額上限訊息
        //WA20230817_提示總金額上限訊息
        if (DDL_HPayMethod.SelectedValue == "1")
        {
            if (!string.IsNullOrEmpty(LB_Total.Text) && LB_Total.Text != "0")
            {
                if (Convert.ToInt32(LB_Total.Text.Replace(",", "")) > 199999)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "PayMethod", "alert('綠界金流代收單一次刷卡金額上限為199,999元，大額超限金額請分次刷卡，謝謝!')", true);
                    return;
                }
            }

        }
        else if (DDL_HPayMethod.SelectedValue == "2")
        {
            if (!string.IsNullOrEmpty(LB_Total.Text) && LB_Total.Text != "0")
            {
                if (Convert.ToInt32(LB_Total.Text.Replace(",", "")) > 49999)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "PayMethod", "alert('綠界金流代收單一次線上ATM金額上限為49,999元，請選擇其他付款方式，謝謝!')", true);
                    return;
                }
            }
        }
        else if (DDL_HPayMethod.SelectedValue == "3")
        {
            if (!string.IsNullOrEmpty(LB_Total.Text) && LB_Total.Text != "0")
            {
                if (Convert.ToInt32(LB_Total.Text.Replace(",", "")) > 20000)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "PayMethod", "alert('綠界金流代收單一次超商繳費金額上限為20,000元，請選擇其他付款方式，謝謝!')", true);
                    return;
                }
            }
        }
        else if (DDL_HPayMethod.SelectedValue == "4")
        {
            if (!string.IsNullOrEmpty(LB_Total.Text) && LB_Total.Text != "0")
            {
                if (Convert.ToInt32(LB_Total.Text.Replace(",", "")) > 49999)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "PayMethod", "alert('綠界金流代收單一次ATM櫃員機金額上限為49,999元，請選擇其他付款方式，謝謝!')", true);
                    return;
                }
            }
        }
        #endregion


        if (CB_Agree.Checked == false)
        {
            Response.Write("<script>alert('請先詳細閱讀繳費須知並勾選同意哦~謝謝~');</script>");
            return;
        }


        //判斷收據勾選狀況
        if (DDL_InvoiceType.SelectedValue == "2")
        {
            if (string.IsNullOrEmpty(TB_InvoiceTitle.Text.Trim()))
            {
                Response.Write("<script>alert('請填寫收據抬頭~');</script>");
                return;
            }
            else if (string.IsNullOrEmpty(TB_TaxID.Text.Trim()))
            {
                Response.Write("<script>alert('請填寫統一編號~');</script>");
                return;
            }
        }
        else if (DDL_InvoiceType.SelectedValue == "1" && LB_Navtab.Text == "1")
        {
            if (DDL_HUploadIRS.SelectedValue == "99")
            {
                Response.Write("<script>alert('請選擇是否上傳國稅局');</script>");
                return;
            }

            #region 願意將捐款資料上傳國稅局時身分證須為必填判斷
            //WA20240125_願意將捐款資料上傳國稅局時身分證須為必填判斷
            //WE20240221_CheckBox改為DropDownList
            if (DDL_HUploadIRS.SelectedValue == "1")
            {
                if (string.IsNullOrEmpty(TB_HPersonID.Text.Trim()))
                {
                    Response.Write("<script>alert('選擇願意將捐款資料上傳國稅局時，身分證不能為空白哦!');</script>");
                    return;
                }
                else if (!string.IsNullOrEmpty(TB_HPersonID.Text.ToUpper().Trim()))
                {
                    //重複判斷
                    //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);
                    SqlCommand cmd = new SqlCommand("SELECT HPersonID FROM HMember WHERE HPersonID=@HPersonID AND HAccount <> @HAccount", con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@HPersonID", TB_HPersonID.Text.ToUpper().Trim());
                    cmd.Parameters.AddWithValue("@HAccount", ((Label)Master.FindControl("LB_HAccount")).Text);
                    SqlDataReader MyEBF = cmd.ExecuteReader();
                    if (MyEBF.Read())
                    {
                        LB_NoticePersonID.Visible = true;
                        LB_NoticePersonID.Text = "此身分證已存在";
                        Response.Write("<script>alert('此身分證已存在，請重新填寫~謝謝~');</script>");
                        return;
                    }
                    else
                    {
                        //格式判斷
                        if (CheckId(TB_HPersonID.Text.ToUpper().Trim()) == false)
                        {
                            LB_NoticePersonID.Visible = true;
                            LB_NoticePersonID.Text = "身分證格式有誤";
                        }
                        else
                        {
                            LB_NoticePersonID.Visible = false;
                        }
                    }
                    MyEBF.Close();
                    cmd.Cancel();
                    con.Close();

                }
            }
    
            #endregion

        }

        #endregion


        #region 判斷參班身分若為純護持，則護持項目為必填
        //GA20231011_加入必填判斷
        int ANum = 0;  //純護持項目
        for (int i = 0; i < Rpt_FillData.Items.Count; i++)
        {
            if (((Label)Rpt_FillData.Items[i].FindControl("LB_HAttend")).Text == "純護持(非班員)")
            {
                SqlDataReader QueryGroup = SQLdatabase.ExecuteReader("SELECT HID FROM HShoppingCart_Group WHERE HShoppingCartID='" + ((Label)Rpt_FillData.Items[i].FindControl("LB_ShoppingCartHID")).Text + "'");
                if (!QueryGroup.Read())
                {
                    ANum += 1;
                }
                QueryGroup.Close();

            }
        }

        if (ANum != 0)
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "alert", "alert('參班身分為純護持(非班員)要記得填寫體系護持工作項目哦~!');", true);
            //Response.Write("<script>alert('參班身分為純護持(非班員)要記得填寫體系護持工作項目哦~');</script>");
            return;
        }


        #endregion




        string HID = null;    //訂單HID
        string StrSQL = null;
        string gOGroupMark = null;
        string day = DateTime.Now.ToString("yyyyMMdd");

        //系統編的訂單代碼（編號規則：英文字+西元年月日+四碼流水號，例如：O202107150001；文化事業的訂單為C開頭；基金會的訂單為F開頭) 221217改
        // string StrOrderGroup = null;
        //LB_HOrderGroup.Text = StrOrderGroup;


        //判斷目前結哪一個單
        if (LB_Navtab.Text == "1")  //傳光(尋光階~一階) - 傳光(基金會)
        {
            gOGroupMark = "F";

            //傳光(基金會)需包含donate的部分
            StrSQL = "SELECT HID AS ShoppingCartID FROM HShoppingCart WHERE HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND HSelect='True' AND ((HPMethod='" + LB_Navtab.Text + "' AND HCourseID <>'0') OR HCourseDonate='1') ";
        }
        else if (LB_Navtab.Text == "2") //玉成(二~七階)-玉成(文化事業)
        {
            gOGroupMark = "C";

            StrSQL = "SELECT HID AS ShoppingCartID FROM HShoppingCart WHERE HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND HSelect='True' AND HPMethod ='" + LB_Navtab.Text + "' AND HCourseDonate='0' AND HCourseID <>'0'";
        }


        //避免重整頁面重複寫入資料
        SqlDataReader Orderdr = SQLdatabase.ExecuteReader("SELECT HOrderGroup  FROM HCourseBooking WHERE SUBSTRING(HOrderGroup, 2, 8) = CONVERT(nvarchar, getdate(), 112) AND SUBSTRING(HOrderGroup, 1, 1) ='" + gOGroupMark + "'  GROUP BY HOrderGroup ORDER BY HOrderGroup DESC");

        if (Orderdr.Read())
        {
            //正式機
            LB_HMerchantTradeNo.Text = "EDU" + DateTime.Now.ToString("yyMMddHHmmssffff");
            //測試機
            //LB_HMerchantTradeNo.Text = "TEST" + DateTime.Now.ToString("yyMMddHHmmssffff");

            SqlDataReader QuerySelHSC = SQLdatabase.ExecuteReader(StrSQL);

            while (QuerySelHSC.Read())
            {
                #region 寫入訂單紀錄(dbo.HCourseBooking)
                SqlDataReader QuerySelHCB = SQLdatabase.ExecuteReader("SELECT HOrderNum FROM HCourseBooking WHERE SUBSTRING(HOrderNum,2,8) = CONVERT(nvarchar, getdate(), 112)  ORDER BY HID DESC");


                if (QuerySelHCB.Read())
                {
                    //SqlCommand cmdInsHCB = new SqlCommand("INSERT INTO HCourseBooking (HOrderNum, HOrderGroup, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal,  HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, HItemStatus, HStatus, HChangeStatus,HCreate, HCreateDT) SELECT 'B' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HOrderNum), 12)) FROM HCourseBooking) + 1), '" + Orderdr["HOrderGroup"].ToString().Substring(0, 9) + String.Format("{0:D4}", Convert.ToInt32(Orderdr["HOrderGroup"].ToString().Substring(9)) + 1) + "', HTradeNo, '" + LB_HMerchantTradeNo.Text + "',  HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, '" + DDL_HPayMethod.SelectedValue + "', HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate,HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, '2','1', '2',HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart WHERE HID='" + QuerySelHSC["ShoppingCartID"].ToString() + "';SELECT SCOPE_IDENTITY() AS HID", con);

                    //GE20231210_加入報名日期存入
                    SqlCommand cmdInsHCB = new SqlCommand("INSERT INTO HCourseBooking (HOrderNum, HOrderGroup, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal,  HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, HItemStatus, HStatus, HChangeStatus,HCreate, HCreateDT) SELECT 'B' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HOrderNum), 12)) FROM HCourseBooking) + 1), '" + Orderdr["HOrderGroup"].ToString().Substring(0, 9) + String.Format("{0:D4}", Convert.ToInt32(Orderdr["HOrderGroup"].ToString().Substring(9)) + 1) + "', HTradeNo, '" + LB_HMerchantTradeNo.Text + "',  HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal, HLodging, '" + DateTime.Now.ToString("yyyy/MM/dd") + "', HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, '" + DDL_HPayMethod.SelectedValue + "', HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate,HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, '2','1', '2',HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart WHERE HID='" + QuerySelHSC["ShoppingCartID"].ToString() + "';SELECT SCOPE_IDENTITY() AS HID", con);

                    //Response.Write("INSERT INTO HCourseBooking (HOrderNum, HOrderGroup, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal,  HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, HItemStatus, HStatus, HChangeStatus,HCreate, HCreateDT) SELECT 'B' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HOrderNum), 12)) FROM HCourseBooking) + 1), '" + Orderdr["HOrderGroup"].ToString().Substring(0, 9) + String.Format("{0:D4}", Convert.ToInt32(Orderdr["HOrderGroup"].ToString().Substring(9)) + 1) + "', HTradeNo, '" + LB_HMerchantTradeNo.Text + "',  HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal, HLodging, '" + DateTime.Now.ToString("yyyy/MM/dd") + "'', HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide,  '" + DDL_HPayMethod.SelectedValue + "', HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate,HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, '2','1', '2',HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart WHERE HID='" + QuerySelHSC["ShoppingCartID"].ToString() + "';SELECT SCOPE_IDENTITY() AS HID");
                    //Response.End();

                    con.Open();
                    HID = cmdInsHCB.ExecuteScalar().ToString();
                    con.Close();
                    cmdInsHCB.Cancel();

                }
                else  //編號會從1開始，但訂單代碼會續編
                {
                    //寫入HCourseBooking
                    //產生訂單編號（編號規則：英文字"B"+西元年月日+四碼流水號，例如：B202107150001)
                    //產生系統編的訂單代碼（編號規則：英文字+西元年月日+四碼流水號，例如：O202107150001；文化事業的訂單為C開頭；基金會的訂單為F開頭) 

                    ////訂單狀態HStatus
                    ///此狀態已停用--0=訂單取消(已付款)、1=訂單成立(已付款)、2=未付款訂單、3=訂單取消(未付款)、4=訂單取消(已退款/點)
                    //GE230801_狀態變更：HStatus=1:訂單成立、2:訂單取消；HItemStatus=1:已付款、2:未付款、3:已退款；
                    //SqlCommand cmdInsHCB = new SqlCommand("INSERT INTO HCourseBooking (HOrderNum, HOrderGroup, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, HItemStatus, HStatus, HChangeStatus, HCreate, HCreateDT) SELECT 'B' + CONVERT(nvarchar, getdate(), 112) + '0001', '" + Orderdr["HOrderGroup"].ToString().Substring(0, 9) + String.Format("{0:D4}", Convert.ToInt32(Orderdr["HOrderGroup"].ToString().Substring(9)) + 1) + "' , HTradeNo, '" + LB_HMerchantTradeNo.Text + "', HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, '" + DDL_HPayMethod.SelectedValue + "', HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, '2', '1', '2',HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart WHERE HID='" + QuerySelHSC["ShoppingCartID"].ToString() + "';SELECT SCOPE_IDENTITY() AS HID", con);

                    //GE20231210_加入報名日期存入
                    SqlCommand cmdInsHCB = new SqlCommand("INSERT INTO HCourseBooking (HOrderNum, HOrderGroup, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, HItemStatus, HStatus, HChangeStatus, HCreate, HCreateDT) SELECT 'B' + CONVERT(nvarchar, getdate(), 112) + '0001', '" + Orderdr["HOrderGroup"].ToString().Substring(0, 9) + String.Format("{0:D4}", Convert.ToInt32(Orderdr["HOrderGroup"].ToString().Substring(9)) + 1) + "' , HTradeNo, '" + LB_HMerchantTradeNo.Text + "', HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal, HLodging, '" + DateTime.Now.ToString("yyyy/MM/dd") + "', HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, '" + DDL_HPayMethod.SelectedValue + "', HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, '2', '1', '2',HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart WHERE HID='" + QuerySelHSC["ShoppingCartID"].ToString() + "';SELECT SCOPE_IDENTITY() AS HID", con);

                    con.Open();
                    HID = cmdInsHCB.ExecuteScalar().ToString();
                    con.Close();
                    cmdInsHCB.Cancel();

                }
                #endregion


                #region 寫入檢覈訂單明細(dbo.HCourseBooking_Exam)
                    SqlCommand cmdInsHCBHCBExam = new SqlCommand("INSERT INTO HCourseBooking_Exam (HCourseBookingID, HExamContentID, HExamSubjectID, HMemberID, HStatus, HCreate, HCreateDT,  HModify, HModifyDT)  SELECT '" + HID + "',HExamContentID, HExamSubjectID, HMemberID, HStatus, HCreate,  '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HModify,  '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart_Exam WHERE HShoppingCartID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);

                    con.Open();
                    cmdInsHCBHCBExam.ExecuteNonQuery();
                    con.Close();
                    cmdInsHCBHCBExam.Cancel();

                #endregion



                #region 寫入體系專業護持報名紀錄(dbo.HCourseBooking_Group)
                //寫入HCourseBooking_Group
                SqlCommand cmdInsHCBG = new SqlCommand("INSERT INTO HCourseBooking_Group (HBookingID, HGroupID, HSameAsCourse, HGDay, HGSTime, HGETime, HTaskID, HTask, HStatus, HCreate, HCreateDT) SELECT '" + HID + "', HGroupID, HSameAsCourse, HGDay, HGSTime, HGETime, HTaskID, HTask, HStatus, HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart_Group WHERE HShoppingCartID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdInsHCBG.ExecuteNonQuery();
                con.Close();
                cmdInsHCBG.Cancel();
                #endregion

                #region 寫入幫他人報名紀錄(dbo.HCourseBooking_Other)
                //寫入HCourseBooking_Other
                SqlCommand cmdInsHCBO = new SqlCommand("INSERT INTO HCourseBooking_Other (HBookingID, HCourseID, HMemberID, HMailStatus, HStatus, HCreate, HCreateDT) SELECT '" + HID + "', HCourseID, HMemberID, HMailStatus, HStatus, HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart_Other WHERE HShoppingCartID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdInsHCBO.ExecuteNonQuery();
                con.Close();
                cmdInsHCBO.Cancel();
                #endregion

                #region 寫入活動報名紀錄(dbo.HCourseBooking_Activity)
                //寫入HCourseBooking_Activity
                SqlCommand cmdInsHCBA = new SqlCommand("INSERT INTO HCourseBooking_Activity (HBookingID, HMemberID, HAName, HASex, HAAge, HARelation, HAmount, HStatus, HCreate, HCreateDT) SELECT '" + HID + "', HMemberID, HAName, HASex, HAAge, HARelation, HAmount, HStatus, HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart_Activity WHERE HShoppingCartID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdInsHCBA.ExecuteNonQuery();
                con.Close();
                cmdInsHCBA.Cancel();
                #endregion

                #region 清空購物車紀錄(因已移轉至訂單)
                //刪除HShoppingCart
                SqlCommand cmdDelHSC = new SqlCommand("Delete HShoppingCart WHERE HID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdDelHSC.ExecuteNonQuery();
                cmdDelHSC.Cancel();
                con.Close();



                //刪除HShoppingCart_Group
                SqlCommand cmdDelHSCG = new SqlCommand("Delete HShoppingCart_Group WHERE HShoppingCartID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdDelHSCG.ExecuteNonQuery();
                cmdDelHSCG.Cancel();
                con.Close();


                //刪除HShoppingCart_Other
                SqlCommand cmdDelHSCO = new SqlCommand("Delete HShoppingCart_Other WHERE HShoppingCartID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdDelHSCO.ExecuteNonQuery();
                cmdDelHSCO.Cancel();
                con.Close();



                //刪除HShoppingCart_Activity
                SqlCommand cmdDelHSCA = new SqlCommand("Delete HShoppingCart_Activity WHERE HShoppingCartID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdDelHSCA.ExecuteNonQuery();
                cmdDelHSCA.Cancel();
                con.Close();


                //刪除HShoppingCart_Exam
                SqlCommand cmdDelHSCExam = new SqlCommand("Delete HShoppingCart_Exam WHERE HShoppingCartID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdDelHSCExam.ExecuteNonQuery();
                cmdDelHSCExam.Cancel();
                con.Close();
                #endregion


            }
            QuerySelHSC.Close();

        }
        else
        {
            //正式機
            LB_HMerchantTradeNo.Text = "EDU" + DateTime.Now.ToString("yyMMddHHmmssffff");
            //測試機
            //LB_HMerchantTradeNo.Text = "TEST" + DateTime.Now.ToString("yyMMddHHmmssffff");

            SqlDataReader QuerySelHSC = SQLdatabase.ExecuteReader(StrSQL);

            while (QuerySelHSC.Read())
            {
                #region 寫入訂單紀錄(dbo.HCourseBooking)
                SqlDataReader QuerySelHCB = SQLdatabase.ExecuteReader("SELECT HOrderNum FROM HCourseBooking WHERE SUBSTRING(HOrderNum,2,8) = CONVERT(nvarchar, getdate(), 112) ORDER BY HID DESC");

                //SqlDataReader QuerySelHCB = SQLdatabase.ExecuteReader("SELECT HOrderGroup, HOrderNum FROM HCourseBooking WHERE SUBSTRING(HOrderNum,2,8) = CONVERT(nvarchar, getdate(), 112) AND HOrderGroup LIKE '%" + gOGroupMark + day + "%' ORDER BY HID DESC");

                //Response.Write("SELECT HOrderGroup, HOrderNum FROM HCourseBooking WHERE SUBSTRING(HOrderNum,2,8) = CONVERT(nvarchar, getdate(), 112) AND HOrderGroup LIKE '%" + gOGroupMark + day + "%' ORDER BY HID DESC" + "<br/><br/>");


                if (QuerySelHCB.Read())
                {

                    //SqlCommand cmdInsHCB = new SqlCommand("INSERT INTO HCourseBooking (HOrderNum, HOrderGroup, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal,  HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, HItemStatus, HStatus, HChangeStatus, HCreate, HCreateDT) SELECT 'B' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HOrderNum), 12)) FROM HCourseBooking) + 1),'" + gOGroupMark + "' + CONVERT(nvarchar, getdate(), 112) + '0001', HTradeNo, '" + LB_HMerchantTradeNo.Text + "',  HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide,'" + DDL_HPayMethod.SelectedValue + "', HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate,HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, '2', '1', '2',HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart WHERE HID='" + QuerySelHSC["ShoppingCartID"].ToString() + "';SELECT SCOPE_IDENTITY() AS HID", con);

                    //GE20231210_加入報名日期存入
                    SqlCommand cmdInsHCB = new SqlCommand("INSERT INTO HCourseBooking (HOrderNum, HOrderGroup, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal,  HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, HItemStatus, HStatus, HChangeStatus, HCreate, HCreateDT) SELECT 'B' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HOrderNum), 12)) FROM HCourseBooking) + 1),'" + gOGroupMark + "' + CONVERT(nvarchar, getdate(), 112) + '0001', HTradeNo, '" + LB_HMerchantTradeNo.Text + "',  HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal, HLodging, '" + DateTime.Now.ToString("yyyy/MM/dd") + "', HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide,'" + DDL_HPayMethod.SelectedValue + "', HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate,HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, '2', '1', '2',HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart WHERE HID='" + QuerySelHSC["ShoppingCartID"].ToString() + "';SELECT SCOPE_IDENTITY() AS HID", con);

                    con.Open();
                    HID = cmdInsHCB.ExecuteScalar().ToString();
                    con.Close();
                    cmdInsHCB.Cancel();

                }
                else  //編號會從1開始，但訂單代碼從1開始
                {

                    //SqlCommand cmdInsHCB = new SqlCommand("INSERT INTO HCourseBooking (HOrderNum, HOrderGroup, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark,  HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate,HItemStatus, HStatus, HChangeStatus, HCreate, HCreateDT) SELECT 'B' + CONVERT(nvarchar, getdate(), 112) + '0001', '" + gOGroupMark + "' + CONVERT(nvarchar, getdate(), 112) + '0001', HTradeNo, '" + LB_HMerchantTradeNo.Text + "', HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, '" + DDL_HPayMethod.SelectedValue + "', HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark,HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, '2', '1', '2', HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart WHERE HID='" + QuerySelHSC["ShoppingCartID"].ToString() + "';SELECT SCOPE_IDENTITY() AS HID", con);


                    //GE20231210_加入報名日期存入
                    SqlCommand cmdInsHCB = new SqlCommand("INSERT INTO HCourseBooking (HOrderNum, HOrderGroup, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark,  HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate,HItemStatus, HStatus, HChangeStatus, HCreate, HCreateDT) SELECT 'B' + CONVERT(nvarchar, getdate(), 112) + '0001', '" + gOGroupMark + "' + CONVERT(nvarchar, getdate(), 112) + '0001', HTradeNo, '" + LB_HMerchantTradeNo.Text + "', HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HPAmount, HSubTotal, HLodging,  '" + DateTime.Now.ToString("yyyy/MM/dd") + "', HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, '" + DDL_HPayMethod.SelectedValue + "', HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark,HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HCourseDonate, '2', '1', '2', HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart WHERE HID='" + QuerySelHSC["ShoppingCartID"].ToString() + "';SELECT SCOPE_IDENTITY() AS HID", con);


                    con.Open();
                    HID = cmdInsHCB.ExecuteScalar().ToString();
                    con.Close();
                    cmdInsHCB.Cancel();

                }
                #endregion

                #region 寫入體系專業護持報名紀錄(dbo.HCourseBooking_Group)
                //寫入HCourseBooking_Group
                SqlCommand cmdInsHCBG = new SqlCommand("INSERT INTO HCourseBooking_Group (HBookingID, HGroupID, HSameAsCourse,HGDay, HGSTime, HGETime, HTaskID, HTask, HStatus, HCreate, HCreateDT) SELECT '" + HID + "', HGroupID, HSameAsCourse, HGDay, HGSTime, HGETime, HTaskID, HTask, HStatus, HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart_Group WHERE HShoppingCartID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdInsHCBG.ExecuteNonQuery();
                con.Close();
                cmdInsHCBG.Cancel();
                #endregion

                #region 寫入幫他人報名紀錄(dbo.HCourseBooking_Other)
                //寫入HCourseBooking_Other
                SqlCommand cmdInsHCBO = new SqlCommand("INSERT INTO HCourseBooking_Other (HBookingID, HCourseID, HMemberID, HMailStatus, HStatus, HCreate, HCreateDT) SELECT '" + HID + "', HCourseID, HMemberID, HMailStatus, HStatus, HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart_Other WHERE HShoppingCartID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdInsHCBO.ExecuteNonQuery();
                con.Close();
                cmdInsHCBO.Cancel();
                #endregion

                #region 寫入活動報名紀錄(dbo.HCourseBooking_Activity)
                //寫入HCourseBooking_Activity
                SqlCommand cmdInsHCBA = new SqlCommand("INSERT INTO HCourseBooking_Activity (HBookingID, HMemberID, HAName, HASex, HAAge, HARelation, HAmount, HStatus, HCreate, HCreateDT) SELECT '" + HID + "', HMemberID, HAName, HASex, HAAge, HARelation, HAmount, HStatus, HCreate, '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' FROM HShoppingCart_Activity WHERE HShoppingCartID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdInsHCBA.ExecuteNonQuery();
                con.Close();
                cmdInsHCBA.Cancel();
                #endregion

                #region 清空購物車紀錄(因已移轉至訂單)
                //刪除HShoppingCart
                SqlCommand cmdDelHSC = new SqlCommand("Delete HShoppingCart WHERE HID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdDelHSC.ExecuteNonQuery();
                cmdDelHSC.Cancel();
                con.Close();



                //刪除HShoppingCart_Group
                SqlCommand cmdDelHSCG = new SqlCommand("Delete HShoppingCart_Group WHERE HShoppingCartID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdDelHSCG.ExecuteNonQuery();
                cmdDelHSCG.Cancel();
                con.Close();


                //刪除HShoppingCart_Other
                SqlCommand cmdDelHSCO = new SqlCommand("Delete HShoppingCart_Other WHERE HShoppingCartID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdDelHSCO.ExecuteNonQuery();
                cmdDelHSCO.Cancel();
                con.Close();



                //刪除HShoppingCart_Activity
                SqlCommand cmdDelHSCA = new SqlCommand("Delete HShoppingCart_Activity WHERE HShoppingCartID='" + QuerySelHSC["ShoppingCartID"].ToString() + "'", con);
                con.Open();
                cmdDelHSCA.ExecuteNonQuery();
                cmdDelHSCA.Cancel();
                con.Close();
                #endregion


            }
            QuerySelHSC.Close();

        }
        Orderdr.Close();



        #region 230616-取得HOrderGroup & HMerchantTradeNo
        SqlDataReader QueryOrderGroup = SQLdatabase.ExecuteReader("SELECT HOrderGroup, HMerchantTradeNo FROM HCourseBooking  WHERE HID = '" + HID + "'");

        if (QueryOrderGroup.Read())
        {
            LB_HOrderGroup.Text = QueryOrderGroup["HOrderGroup"].ToString();
            LB_HMerchantTradeNo.Text = QueryOrderGroup["HMerchantTradeNo"].ToString();
        }
        QueryOrderGroup.Close();
        #endregion



        #region 更新付款資訊&收據資料&是否願意上傳國稅局
        //WE20240125_加入是否願意上傳國稅局、改為參數方式
        //WE20240221_CheckBox改為DropDownList
        if (DDL_InvoiceType.SelectedValue == "2")
        {
            DDL_HUploadIRS.SelectedValue = "0";
            //CB_HUploadIRS.Checked = false;
        }

        SqlCommand UPcmd = new SqlCommand("UPDATE HCourseBooking SET HInvoiceType=@HInvoiceType, HInvoiveTitle=@HInvoiveTitle, HTaxID=@HTaxID, HUploadIRS=@HUploadIRS WHERE HOrderGroup = @HOrderGroup AND HMerchantTradeNo = @HMerchantTradeNo", con);
        con.Open();
        UPcmd.Parameters.AddWithValue("@HInvoiceType", DDL_InvoiceType.SelectedValue);
        UPcmd.Parameters.AddWithValue("@HInvoiveTitle", TB_InvoiceTitle.Text.Trim());
        UPcmd.Parameters.AddWithValue("@HTaxID", TB_TaxID.Text.Trim());
        UPcmd.Parameters.AddWithValue("@HUploadIRS", DDL_HUploadIRS.SelectedValue);
        //UPcmd.Parameters.AddWithValue("@HUploadIRS", CB_HUploadIRS.Checked);
        UPcmd.Parameters.AddWithValue("@HOrderGroup", LB_HOrderGroup.Text.Trim());
        UPcmd.Parameters.AddWithValue("@HMerchantTradeNo", LB_HMerchantTradeNo.Text.Trim());
        UPcmd.ExecuteNonQuery();
        UPcmd.Cancel();
        con.Close();

        //SQLdatabase.ExecuteNonQuery("UPDATE HCourseBooking SET HInvoiceType='" + DDL_InvoiceType.SelectedValue + "', HInvoiveTitle='" + TB_InvoiceTitle.Text.Trim() + "', HTaxID='" + TB_TaxID.Text.Trim()  + "', HUploadIRS=" + CB_HUploadIRS.Checked + " WHERE HOrderGroup = '" + LB_HOrderGroup.Text + "' AND HMerchantTradeNo = '" + LB_HMerchantTradeNo.Text + "'");
        #endregion


        //串接綠界-
        #region Session-金流資訊

        string HOrderGroup = null;
        string MerchantTradeNo = null;  //綠界廠商交易編號
        string MemberID = null;
        string PMethod = null;  //繳費帳戶(1:傳光(基金會)/2:玉成(文化事業))
        string CTotal = "";  //玉成(文化事業)總金額 (HPoint為點數需轉成金額*10)
        string FTotal = "";  //傳光(基金會)總金額 (HPoint即為金額)

        string UserName = null;
        string Email = null;
        string Phone = null;


        string PayMethod = "";  //付款方式(0:無、1:線上刷卡、2:線上ATM、3:超商繳費、4: ATM櫃員機、9:其他/使用點數)

        string SelectSQL = null;



        if (LB_Navtab.Text == "1") //傳光(基金會)
        {
            SelectSQL = "SELECT HOrderGroup, HMerchantTradeNo, HMemberID, SUM(convert(int,HPoint))AS FTotal FROM HCourseBooking WHERE HOrderGroup = '" + LB_HOrderGroup.Text + "' AND HMerchantTradeNo = '" + LB_HMerchantTradeNo.Text + "' AND  (HPMethod='1' OR HCourseDonate = '1') Group by HOrderGroup, HMerchantTradeNo, HMemberID";
        }
        else if (LB_Navtab.Text == "2") //玉成(文化事業)(點數要轉換成金額*10)
        {
            SelectSQL = "SELECT HOrderGroup, HMerchantTradeNo, HMemberID, HPMethod, SUM(convert(int,HPoint))*10 AS CTotal FROM HCourseBooking WHERE HOrderGroup = '" + LB_HOrderGroup.Text + "' AND HMerchantTradeNo = '" + LB_HMerchantTradeNo.Text + "' AND HPMethod='2' AND HCourseDonate = '0' Group by HOrderGroup, HMerchantTradeNo, HMemberID, HPMethod";
        }

        SqlDataReader dr = SQLdatabase.ExecuteReader(SelectSQL);


        PMethod = LB_Navtab.Text;
        MemberID = ((Label)Master.FindControl("LB_HUserHID")).Text;


        if (dr.Read())
        {
            HOrderGroup = dr["HOrderGroup"].ToString();
            MerchantTradeNo = dr["HMerchantTradeNo"].ToString();


            if (LB_Navtab.Text == "1")
            {
                FTotal = dr["FTotal"].ToString();

                //221205-ADD-更新HPayAmt (應繳訂單總金額)
                SQLdatabase.ExecuteNonQuery("UPDATE HCourseBooking SET HPayAmt='" + FTotal + "' WHERE HOrderGroup = '" + HOrderGroup + "' AND HMerchantTradeNo = '" + MerchantTradeNo + "'");

            }
            else if (LB_Navtab.Text == "2")
            {
                CTotal = dr["CTotal"].ToString();

                //221205-ADD-更新HPayAmt (應繳訂單總金額)
                SQLdatabase.ExecuteNonQuery("UPDATE HCourseBooking SET HPayAmt='" + CTotal + "' WHERE HOrderGroup = '" + HOrderGroup + "' AND HMerchantTradeNo = '" + MerchantTradeNo + "'");
            }

        }
        dr.Close();

        SqlDataReader Memberdr = SQLdatabase.ExecuteReader("SELECT HID, HUserName, HAccount, HPhone FROM HMember WHERE HID=" + MemberID);

        //221004-信箱以登入帳號為主
        if (Memberdr.Read())
        {
            UserName = Memberdr["HUserName"].ToString();
            Email = Memberdr["HAccount"].ToString();
            Phone = Memberdr["HPhone"].ToString();
        }
        Memberdr.Close();


        //綠界金流(判斷為文化還是傳光(基金會))
        //付款方式(0:無、1:線上刷卡、2:線上ATM、3:超商繳費、4: ATM櫃員機、9:其他/使用點數)
        PayMethod = DDL_HPayMethod.SelectedValue == "1" ? "Credit" : DDL_HPayMethod.SelectedValue == "2" ? "WebATM" : DDL_HPayMethod.SelectedValue == "3" ? "CVS" : DDL_HPayMethod.SelectedValue == "4" ? "ATM" : "9";

        //訂單明細
        Session.Add("OrderGroup", HOrderGroup);
        Session.Add("MerchantTradeNo", MerchantTradeNo);
        Session.Add("PMethod", PMethod);
        Session.Add("MemberID", MemberID);
        Session.Add("UserName", UserName);
        Session.Add("Email", Email);
        Session.Add("Phone", Phone);
        Session.Add("PayMethod", PayMethod);
        Session.Add("ChangeStatus", 2); //紀錄變更訂單用: 1=已變更、2=未變更

        #region 狀態統整(20230616更新)
        ///訂單單頭狀態(HStatus)
        ///1:訂單成立、2:訂單取消
        ///訂單明細付款狀態(HItemStatus)
        ///1:已付款、2:未付款、3:已退款
        #endregion

        if (PMethod == "1")   //傳光(基金會)
        {
            if (FTotal != "0")  //金額不等於0才導綠界
            {
                Session.Add("Total", FTotal);
                Session["SubmitCheck"] = "0";
                Response.Redirect("~/AioCheckOut.aspx");
            }
            else  //不需結帳
            {
                //完成報名
                SQLdatabase.ExecuteNonQuery("UPDATE HCourseBooking SET HPayMethod = '0', HItemStatus='1', HStatus='1' WHERE HOrderGroup = '" + LB_HOrderGroup.Text + "'");

                #region Mail通知-報名成功
                //主報名者
                int CheckEmail = SentEmail(HOrderGroup, UserName, Email, Phone, FTotal, "無");

                if (CheckEmail == 0)
                {
                    Response.Write("<script>alert('寄驗證信失敗！請確認信箱是否正確');</script>");
                }
                #endregion

                #region 課程類別 Type=2 - 寫入dbo.資格檢覈
                SqlDataReader QueryBooking = SQLdatabase.ExecuteReader("SELECT a.HOrderGroup, a.HOrderNum, a.HID, b.HType, b.HCourseName FROM HCourseBooking AS a LEFT JOIN CourseList AS b ON a.HCourseID = b.HID WHERE a.HOrderGroup = '" + LB_HOrderGroup.Text + "' AND a.HStatus='1' ");
                con.Open();
                while (QueryBooking.Read())
                {
                    if (QueryBooking["HType"].ToString() == "2")
                    {
                        //避免重整頁面重複寫入資料
                        SqlDataReader Qualifydr = SQLdatabase.ExecuteReader("SELECT a.HStatus, a.HCBookingID FROM HQualify AS a INNER JOIN HCourseBooking AS b ON a.HCBookingID = b.HID WHERE a.HCBookingID = '" + QueryBooking["HID"].ToString() + "'");

                        if (!Qualifydr.Read())
                        {
                            SqlCommand Insertcmd = new SqlCommand("INSERT INTO HQualify (HCBookingID, HTStatus1, HTStatus2, HTStatus3, HStatus, HCreate, HCreateDT) VALUES (@HCBookingID, @HTStatus1, @HTStatus2, @HTStatus3, @HStatus, @HCreate, @HCreateDT);", con);


                            Insertcmd.Parameters.AddWithValue("@HCBookingID", QueryBooking["HID"].ToString());
                            Insertcmd.Parameters.AddWithValue("@HTStatus1", "0");
                            Insertcmd.Parameters.AddWithValue("@HTStatus2", "0");
                            Insertcmd.Parameters.AddWithValue("@HTStatus3", "0");
                            Insertcmd.Parameters.AddWithValue("@HStatus", "0");
                            Insertcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                            Insertcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                            Insertcmd.ExecuteNonQuery();
                            Insertcmd.Cancel();
                        }
                        Qualifydr.Close();
                    }
                }
                QueryBooking.Close();
                con.Close();
                #endregion


                #region 產生/更新點名單

                #region 宣告dataTable框架
                DataTable dtRC = new DataTable("dtRC");
                dtRC.Columns.Add("HCourseID", typeof(String));
                dtRC.Columns.Add("HMemberID", typeof(String));
                dtRC.Columns.Add("HRCDate", typeof(String));
                dtRC.Columns.Add("HAStatus", typeof(String));
                dtRC.Columns.Add("HStatus", typeof(String));
                dtRC.Columns.Add("HCreate", typeof(String));
                dtRC.Columns.Add("HCreateDT", typeof(String));
                Session["dtRC"] = dtRC;
                //清除DataTable以釋放記憶體(如果不需要或有異常可先註解)
                dtRC.Clear();
                #endregion


                //GE250302_改成用課程內容的地方跑迴圈
                for (int x = 0; x < Rpt_FillData.Items.Count; x++)
                {

                    if (((Label)Rpt_FillData.Items[x].FindControl("LB_HRollcallYN")).Text == "1")
                    {
                        ProduceRollCall.GetRollCall(((Label)Rpt_FillData.Items[x].FindControl("LB_HCourseID")).Text, (System.Data.DataTable)Session["dtRC"], ((Label)Master.FindControl("LB_HUserHID")).Text);
                    }
                }

                #endregion
                ScriptManager.RegisterStartupScript(Page, GetType(), "alert", "alert('因不用付款，已完成報名囉~!');window.location.href='HMember_Order.aspx';", true);
            }
        }
        else if (PMethod == "2")  //玉成(文化事業)
        {
            //221015--先判斷是否要補差額
            if (LB_Difference.Text != "0")  //要補差額
            {
                //221205-ADD-更新HPayAmt (應繳訂單總金額)
                SQLdatabase.ExecuteNonQuery("UPDATE HCourseBooking SET HPayAmt='" + Convert.ToInt32(LB_Difference.Text.Replace(",", "")) + "' WHERE HOrderGroup = '" + HOrderGroup + "' AND HMerchantTradeNo = '" + MerchantTradeNo + "'");

                //221216-  Add
                #region 扣使用的剩餘點數

                SqlDataReader OrderPoints = SQLdatabase.ExecuteReader("SELECT a.HMemberID, a.HOrderGroup FROM HPoints AS a INNER JOIN  HCourseBooking AS b ON a.HOrderGroup = b.HOrderGroup WHERE a.HOrderGroup = '" + LB_HOrderGroup.Text + "' AND a.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HStatus='1'");

                if (!OrderPoints.Read())
                {
                    //使用的點數
                    int UsePoint = Convert.ToInt32(LB_Remain.Text.Replace(",", "")) / 10;

                    SqlCommand Pointcmd = new SqlCommand("INSERT INTO HPoints (HMemberID, HRecrodDate, HPayMethod, HUse, HRecord, HAmount, HUseFor, HStatus, HCreate, HCreateDT, HOrderGroup) VALUES (@HMemberID, @HRecrodDate, @HPayMethod, @HUse, @HRecord, @HAmount, @HUseFor, @HStatus, @HCreate, @HCreateDT, @HOrderGroup);", con);
                    con.Open();

                    Pointcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    Pointcmd.Parameters.AddWithValue("@HRecrodDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    Pointcmd.Parameters.AddWithValue("@HPayMethod", "9");  //用點數購買，付款方式列為"其他"  //1=線上刷卡、2=線上ATM、3=超商繳費、9=其它　
                    Pointcmd.Parameters.AddWithValue("@HUse", UsePoint);
                    //Pointcmd.Parameters.AddWithValue("@HUse", LB_Total2.Text);
                    Pointcmd.Parameters.AddWithValue("@HRecord", "報名玉成課程");
                    Pointcmd.Parameters.AddWithValue("@HAmount", "0");//未完成-待確認
                    Pointcmd.Parameters.AddWithValue("@HUseFor", "購買課程");
                    Pointcmd.Parameters.AddWithValue("@HStatus", "1");
                    Pointcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    Pointcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    Pointcmd.Parameters.AddWithValue("@HOrderGroup", LB_HOrderGroup.Text);


                    Pointcmd.ExecuteNonQuery();
                    con.Close();
                    Pointcmd.Cancel();
                }
                OrderPoints.Close();
                #endregion


                //傳給綠界為差額
                Session.Add("Total", LB_Difference.Text.Replace(",", ""));
                Session["SubmitCheck"] = "0";
                Response.Redirect("~/CAioCheckOut.aspx");
            }
            else
            {
                //判斷剩餘金額是否不為0且大於等於總金額，直接扣點報名
                if (Convert.ToInt32(LB_Remain.Text.Replace(",", "")) >= Convert.ToInt32(LB_Total.Text.Replace(",", "")) && LB_Remain.Text != "0" && LB_Difference.Text == "0")
                {
                    //完成報名
                    SQLdatabase.ExecuteNonQuery("UPDATE HCourseBooking SET HPayMethod = '9', HItemStatus='1', HStatus='1' WHERE HOrderGroup = '" + LB_HOrderGroup.Text + "'");
                    //HPoint = '" + (Convert.ToInt32(LB_Total.Text.Replace(",", ""))) / 10 + "',

                    #region 寫入dbo.點數管理

                    //if (dr["HPayMethod"].ToString() == "1" || dr["HPayMethod"].ToString() == "2")
                    //{
                    //避免重整頁面重複寫入資料
                    SqlDataReader Order = SQLdatabase.ExecuteReader("SELECT a.HMemberID, a.HOrderGroup FROM HPoints AS a INNER JOIN  HCourseBooking AS b ON a.HOrderGroup = b.HOrderGroup WHERE a.HOrderGroup = '" + LB_HOrderGroup.Text + "' AND a.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HStatus='1'");

                    if (!Order.Read())
                    {

                        //使用的點數(全額扣點數)
                        int UsePoint = (Convert.ToInt32(LB_Total.Text.Replace(",", ""))) / 10;

                        SqlCommand Pointcmd = new SqlCommand("INSERT INTO HPoints (HMemberID, HRecrodDate, HPayMethod, HUse, HRecord, HAmount, HUseFor, HStatus, HCreate, HCreateDT, HOrderGroup) VALUES (@HMemberID, @HRecrodDate, @HPayMethod, @HUse, @HRecord, @HAmount, @HUseFor, @HStatus, @HCreate, @HCreateDT, @HOrderGroup);", con);
                        con.Open();

                        Pointcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        Pointcmd.Parameters.AddWithValue("@HRecrodDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        Pointcmd.Parameters.AddWithValue("@HPayMethod", "9");  //用點數購買，付款方式列為"其他"  //1=線上刷卡、2=線上ATM、3=超商繳費、9=其它　
                        Pointcmd.Parameters.AddWithValue("@HUse", UsePoint);
                        //Pointcmd.Parameters.AddWithValue("@HUse", LB_Total2.Text);
                        Pointcmd.Parameters.AddWithValue("@HRecord", "報名玉成課程");
                        Pointcmd.Parameters.AddWithValue("@HAmount", "0");//未完成-待確認
                        Pointcmd.Parameters.AddWithValue("@HUseFor", "購買課程");
                        Pointcmd.Parameters.AddWithValue("@HStatus", "1");
                        Pointcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        Pointcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        Pointcmd.Parameters.AddWithValue("@HOrderGroup", LB_HOrderGroup.Text);

                        Pointcmd.ExecuteNonQuery();
                        con.Close();
                        Pointcmd.Cancel();
                    }
                    Order.Close();

                    //}

                    #endregion

                    #region Mail通知-報名成功
                    //主報名者
                    int CheckEmail = SentEmail(HOrderGroup, UserName, Email, Phone, CTotal, "使用點數");
                    if (CheckEmail == 0)
                    {
                        Response.Write("<script>alert('寄驗證信失敗！請確認信箱是否正確');</script>");
                    }
                    #endregion

                    #region 課程類別 Type=2 - 寫入dbo.資格檢覈
                    SqlDataReader QueryBooking = SQLdatabase.ExecuteReader("SELECT a.HOrderGroup, a.HOrderNum, a.HID, b.HType, b.HCourseName FROM HCourseBooking AS a LEFT JOIN CourseList AS b ON a.HCourseID = b.HID WHERE a.HOrderGroup = '" + LB_HOrderGroup.Text + "' AND a.HStatus='1' ");
                    con.Open();
                    while (QueryBooking.Read())
                    {
                        if (QueryBooking["HType"].ToString() == "2")
                        {
                            //避免重整頁面重複寫入資料
                            SqlDataReader Qualifydr = SQLdatabase.ExecuteReader("SELECT a.HStatus, a.HCBookingID FROM HQualify AS a INNER JOIN HCourseBooking AS b ON a.HCBookingID = b.HID WHERE a.HCBookingID = '" + QueryBooking["HID"].ToString() + "'");

                            if (!Qualifydr.Read())
                            {
                                SqlCommand Insertcmd = new SqlCommand("INSERT INTO HQualify (HCBookingID, HTStatus1, HTStatus2, HTStatus3, HStatus, HCreate, HCreateDT) VALUES (@HCBookingID, @HTStatus1, @HTStatus2, @HTStatus3, @HStatus, @HCreate, @HCreateDT);", con);


                                Insertcmd.Parameters.AddWithValue("@HCBookingID", QueryBooking["HID"].ToString());
                                Insertcmd.Parameters.AddWithValue("@HTStatus1", "0");
                                Insertcmd.Parameters.AddWithValue("@HTStatus2", "0");
                                Insertcmd.Parameters.AddWithValue("@HTStatus3", "0");
                                Insertcmd.Parameters.AddWithValue("@HStatus", "0");
                                Insertcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                                Insertcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                                Insertcmd.ExecuteNonQuery();
                                Insertcmd.Cancel();
                            }
                            Qualifydr.Close();
                        }
                    }
                    QueryBooking.Close();
                    con.Close();
                    #endregion


                    #region 產生/更新點名單
                    //GA250215_新增
                    #region 宣告dataTable框架
                    DataTable dtRC = new DataTable("dtRC");
                    dtRC.Columns.Add("HCourseID", typeof(String));
                    dtRC.Columns.Add("HMemberID", typeof(String));
                    dtRC.Columns.Add("HRCDate", typeof(String));
                    dtRC.Columns.Add("HAStatus", typeof(String));
                    dtRC.Columns.Add("HStatus", typeof(String));
                    dtRC.Columns.Add("HCreate", typeof(String));
                    dtRC.Columns.Add("HCreateDT", typeof(String));

                    Session["dtRC"] = dtRC;
                    //清除DataTable以釋放記憶體(如果不需要或有異常可先註解)
                    dtRC.Clear();
                    #endregion

                    //GE250302_改成用課程內容的地方跑迴圈
                    for (int x = 0; x < Rpt_FillData.Items.Count; x++)
                    {
                        if (((Label)Rpt_FillData.Items[x].FindControl("LB_HRollcallYN")).Text == "1")
                        {
                         ProduceRollCall.GetRollCall(((Label)Rpt_FillData.Items[x].FindControl("LB_HCourseID")).Text, (System.Data.DataTable)Session["dtRC"], ((Label)Master.FindControl("LB_HUserHID")).Text);
                        }
                    }


                    #endregion

                    ScriptManager.RegisterStartupScript(Page, GetType(), "alert", "alert('系統已自動扣除點數，並完成報名囉~!');window.location.href='HMember_Order.aspx';", true);
                }
                else
                {
                    if (CTotal != "0")  //金額不等於0才導綠界
                    {
                        Session.Add("Total", CTotal);
                        Session["SubmitCheck"] = "0";
                        Response.Redirect("~/CAioCheckOut.aspx");
                    }
                    else
                    {
                        //完成報名
                        SQLdatabase.ExecuteNonQuery("UPDATE HCourseBooking SET HPayMethod = '0',HItemStatus='1', HStatus='1' WHERE HOrderGroup = '" + LB_HOrderGroup.Text + "'");

                        #region Mail通知-報名成功
                        //主報名者
                        int CheckEmail = SentEmail(HOrderGroup, UserName, Email, Phone, CTotal, "無");
                        if (CheckEmail == 0)
                        {
                            Response.Write("<script>alert('寄驗證信失敗！請確認信箱是否正確');</script>");
                        }
                        #endregion

                        #region 課程類別 Type=2 - 寫入dbo.資格檢覈
                        SqlDataReader QueryBooking = SQLdatabase.ExecuteReader("SELECT a.HOrderGroup, a.HOrderNum, a.HID, b.HType, b.HCourseName FROM HCourseBooking AS a LEFT JOIN CourseList AS b ON a.HCourseID = b.HID WHERE a.HOrderGroup = '" + LB_HOrderGroup.Text + "' AND a.HStatus='1' ");
                        con.Open();
                        while (QueryBooking.Read())
                        {
                            if (QueryBooking["HType"].ToString() == "2")
                            {
                                //避免重整頁面重複寫入資料
                                SqlDataReader Qualifydr = SQLdatabase.ExecuteReader("SELECT a.HStatus, a.HCBookingID FROM HQualify AS a INNER JOIN HCourseBooking AS b ON a.HCBookingID = b.HID WHERE a.HCBookingID = '" + QueryBooking["HID"].ToString() + "'");

                                if (!Qualifydr.Read())
                                {
                                    SqlCommand Insertcmd = new SqlCommand("INSERT INTO HQualify (HCBookingID, HTStatus1, HTStatus2, HTStatus3, HStatus, HCreate, HCreateDT) VALUES (@HCBookingID, @HTStatus1, @HTStatus2, @HTStatus3, @HStatus, @HCreate, @HCreateDT);", con);


                                    Insertcmd.Parameters.AddWithValue("@HCBookingID", QueryBooking["HID"].ToString());
                                    Insertcmd.Parameters.AddWithValue("@HTStatus1", "0");
                                    Insertcmd.Parameters.AddWithValue("@HTStatus2", "0");
                                    Insertcmd.Parameters.AddWithValue("@HTStatus3", "0");
                                    Insertcmd.Parameters.AddWithValue("@HStatus", "0");
                                    Insertcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                                    Insertcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                                    Insertcmd.ExecuteNonQuery();
                                    Insertcmd.Cancel();
                                }
                                Qualifydr.Close();
                            }
                        }
                        QueryBooking.Close();
                        con.Close();
                        #endregion


                        #region 產生/更新點名單
                        //GA250215_新增

                        string gHCourseID = "";


                        #region 宣告dataTable框架
                        DataTable dtRC = new DataTable("dtRC");
                        dtRC.Columns.Add("HCourseID", typeof(String));
                        dtRC.Columns.Add("HMemberID", typeof(String));
                        dtRC.Columns.Add("HRCDate", typeof(String));
                        dtRC.Columns.Add("HAStatus", typeof(String));
                        dtRC.Columns.Add("HStatus", typeof(String));
                        dtRC.Columns.Add("HCreate", typeof(String));
                        dtRC.Columns.Add("HCreateDT", typeof(String));

                        Session["dtRC"] = dtRC; // 使用 Session 儲存，避免 ViewState 限制
                        //清除DataTable以釋放記憶體(如果不需要或有異常可先註解)
                        dtRC.Clear();
                        #endregion


                        //GE250302_改成用課程內容的地方跑迴圈
                        for (int x = 0; x < Rpt_FillData.Items.Count; x++)
                        {
                            if (((Label)Rpt_FillData.Items[x].FindControl("LB_HRollcallYN")).Text == "1")
                            {
                               ProduceRollCall.GetRollCall(((Label)Rpt_FillData.Items[x].FindControl("LB_HCourseID")).Text, (System.Data.DataTable)Session["dtRC"], ((Label)Master.FindControl("LB_HUserHID")).Text);
                            }
                        }

                        #endregion

                        ScriptManager.RegisterStartupScript(Page, GetType(), "alert", "alert('因不用付款，已完成報名囉~!');window.location.href='HMember_Order.aspx';", true);
                    }
                }



            }
        }


        #endregion

    }
    #endregion

    #region 檢查欄位空值&格式判斷
    //protected string chkValidation()
    //{
    //	string alert = "";

    //	//是否住宿慈場
    //	if (DDL_HLoading.SelectedValue == "1")
    //	{
    //		if (string.IsNullOrEmpty(TB_HLDate.Text))
    //		{
    //			alert += @"請選擇住宿日期哦!。\n";
    //		}
    //		if (DDL_HRoom.SelectedValue == "0")
    //		{
    //			alert += @"請選擇住宿區意願哦!。\n";
    //		}
    //		if (string.IsNullOrEmpty(TB_HRoomTime.Text))
    //		{
    //			alert += @"請選擇住宿時間哦!。\n";
    //		}
    //	}

    //	//玉成(文化事業)-點數不足
    //	//if (ViewState["PMethod"].ToString() == "2")
    //	//{
    //	//    if (Convert.ToInt32(LB_RemainPoints.Text) < Convert.ToInt32(TB_HPoint.Text))
    //	//    {
    //	//        alert += @"目前餘額不足哦。\n";
    //	//        //alert += @"目前點數不足哦~請先儲值點數。\n";
    //	//    }
    //	//}

    //	if (Convert.ToInt32(LB_Total.Text.Replace(",", "")) < 0)
    //	{
    //		alert += @"金額不得小於0哦。\n";
    //	}

    //	//繳費須知確認
    //	if (CB_Agree.Checked == false)
    //	{
    //		alert += @"請勾選是否同意須知。\n";
    //	}

    //	return alert;
    //}
    #endregion

    #region 註冊非同步事件
    protected void RegisterAsync()
    {
        RegScript(); //執行js

        //註冊事件
        for (int x = 0; x < Rpt_Support.Items.Count; x++)
        {
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Rpt_Support.Items[x].FindControl("LBtn_Del"));
        }

        for (int x = 0; x < Rpt_Cultural.Items.Count; x++)
        {
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Rpt_Cultural.Items[x].FindControl("CB_SelectC"));
        }

        for (int x = 0; x < Rpt_FoundationC.Items.Count; x++)
        {
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Rpt_FoundationC.Items[x].FindControl("CB_SelectFC"));
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Rpt_FoundationC.Items[x].FindControl("LBtn_HCourseNameFC"));
        }

        for (int x = 0; x < Rpt_FoundationD.Items.Count; x++)
        {
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Rpt_FoundationD.Items[x].FindControl("CB_SelectFD"));
        }

        for (int x = 0; x < Rpt_Activity.Items.Count; x++)
        {
            ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Rpt_Activity.Items[x].FindControl("LBtn_ActivityDel"));
        }


    }
    #endregion

    #region 執行js
    public void RegScript()
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$('.js-example-basic-single').select2({closeOnSelect: true,});"), true);//執行js

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS2", ("$('.timepicker').timepicker({ timeFormat: 'HH:mm',interval: 30,minTime: '4',maxTime: '11:00 pm', startTime: '04:00',dynamic: false, dropdown: true, scrollbar: true});"), true);     //執行時間js

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "Datepicker", ("$('.datepicker').datepicker({format: 'yyyy/mm/dd',todayHighlight: true,orientation: 'bottom auto',toggleActive: true,autoclose: true, });"), true);

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "datemultipicker", (" $('.datemultipicker').datepicker({language: 'en',dateFormat: 'yyyy/mm/dd',multipleDates: true,minDate: new Date(),todayHighlight: true,orientation: 'bottom auto',});"), true);

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "icheck", (" $('.check').on('ifChanged', function (event) {$(this).trigger('click');});"), true);

        // ScriptManager.RegisterStartupScript(Page, this.GetType(), "Datepicker", ("$('.datepicker').datepicker({format: 'yyyy/mm/dd',todayHighlight: true,orientation: 'bottom auto',toggleActive: true,autoclose: true,});"), true);
        //ScriptManager.RegisterStartupScript(Page, this.GetType(), "icheck", ("for (int x = 0; x < Rpt_Cultural.Items.Count; x++) { $(document.getElementBId(Rpt_Cultural.Items[x].FindControl('CB_SelectC')).on('ifChanged', function(event) {$(this).trigger('click'); })"),true);

    }
    #endregion

    #region 收據類型
    protected void DDL_InvoiceType_SelectedIndexChanged(object sender, EventArgs e)
    {
        Div_TaxID.Visible = DDL_InvoiceType.SelectedValue == "2" ? true : false;
        Div_InvoiceTitle.Visible = DDL_InvoiceType.SelectedValue == "2" ? true : false;

        //WA20240125_加入上傳國稅局&身分證顯示判斷
        if (LB_Navtab.Text == "1")
        {
            Div_HUploadIRS.Visible = DDL_InvoiceType.SelectedValue == "1" ? true : false;

            //WE20240221_CheckBox改為DropDownList
            if (DDL_HUploadIRS.SelectedValue == "1" && DDL_InvoiceType.SelectedValue == "1")
            {
                Div_HPersonID.Visible = true;
            }
            else
            {
                Div_HPersonID.Visible = false;
            }
     
        }
        else
        {
            Div_HUploadIRS.Visible = false;
            Div_HPersonID.Visible = false;
        }
    }
    #endregion

    #region 寄信
    /// <summary>
    /// 寄信
    /// </summary>
    /// <param name="RandomCode">亂數</param>
    /// <param name="Name">信件顯示姓名</param>
    /// <returns>寄信是否成功，1：成功，2：失敗</returns>
    public int SentEmail(string OrderGroup, string Name, string Email, string Phone, string Total, string PayMethod)
    {
        int Ok = 0;
        //信件本體宣告
        MailMessage mail = new MailMessage();
        // 寄件者, 收件者和副本郵件地址        
        mail.From = new MailAddress(Sender, "和氣大愛玉成系統自動寄信機器人");
        // 設定收件者
        mail.To.Add(new MailAddress(((Label)Master.FindControl("LB_HAccount")).Text.Trim(), ""));
        // 優先等級
        mail.Priority = MailPriority.Normal;
        // 主旨
        mail.Subject = "和氣大愛玉成系統 - 完成報名" + "，訂單代碼#" + OrderGroup;

        mail.Body = "<p>" + Name + "，您好~ <br/>感謝您的報名~</p><p>您的報名資訊如下：</p>" +
            "<p><span style='font-weight:bold;'>訂單代碼： </span>" + OrderGroup + " </p>" +
            "<p><span style='font-weight:bold;'>學員姓名： </span>" + Name + "  </p>" +
   "<p><span style='font-weight:bold;'>電子信箱： </span>" + Email + "  </p>" +
   "<p><span style='font-weight:bold;'>手機號碼：</span> " + Phone + "  </p>" +
   "<p><span style='font-weight:bold;'>總付款金額：</span> " + Total + " 元 </p>" +
   "<p><span style='font-weight:bold;'>付款方式：</span> " + PayMethod + "  </p>" +
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
            Ok = 1;
            return Ok;
        }
        catch (Exception ex)
        {
            //寄信失敗            
            Ok = 0;
            return Ok;
        }
    }
    #endregion

    #region 上一步
    protected void Btn_Back_Click(object sender, EventArgs e)
    {

        //GE20230821_修改回上一步的寫法，避免金額被清掉
        Panel_Cart.Visible = true;
        Btn_FillData.Visible = true;
        Panel_FillData.Visible = false;
        Btn_Back.Visible = false;
        Btn_CheckOut.Visible = false;

        //GA20231017_加入顏色固定位置
        DIV_Cart.Attributes.Add("class", "process-item active");
        DIV_Data.Attributes.Add("class", "process-item");
    }
    #endregion

    #region 付款方式選擇
    //WA20230817_提示總金額上限訊息
    protected void DDL_HPayMethod_TextChanged(object sender, EventArgs e)
    {
        if (DDL_HPayMethod.SelectedValue == "1")
        {
            if (!string.IsNullOrEmpty(LB_Total.Text) && LB_Total.Text != "0")
            {
                if (Convert.ToInt32(LB_Total.Text.Replace(",", "")) > 199999)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "PayMethod", "alert('綠界金流代收單一次刷卡金額上限為199,999元，大額超限金額請分次刷卡，謝謝!')", true);
                    return;
                }
            }

        }
        else if (DDL_HPayMethod.SelectedValue == "2")
        {
            if (!string.IsNullOrEmpty(LB_Total.Text) && LB_Total.Text != "0")
            {
                if (Convert.ToInt32(LB_Total.Text.Replace(",", "")) > 49999)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "PayMethod", "alert('綠界金流代收單一次線上ATM金額上限為49,999元，請選擇其他付款方式，謝謝!')", true);
                    return;
                }
            }
        }
        else if (DDL_HPayMethod.SelectedValue == "3")
        {
            if (!string.IsNullOrEmpty(LB_Total.Text) && LB_Total.Text != "0")
            {
                if (Convert.ToInt32(LB_Total.Text.Replace(",", "")) > 20000)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "PayMethod", "alert('綠界金流代收單一次超商繳費金額上限為20,000元，請選擇其他付款方式，謝謝!')", true);
                    return;
                }
            }
        }
        //GE20230820_新增ATM櫃員機付款上限
        else if (DDL_HPayMethod.SelectedValue == "4")
        {
            if (!string.IsNullOrEmpty(LB_Total.Text) && LB_Total.Text != "0")
            {
                if (Convert.ToInt32(LB_Total.Text.Replace(",", "")) > 49999)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "PayMethod", "alert('綠界金流代收單一次ATM櫃員機金額上限為49,999元，請選擇其他付款方式，謝謝!')", true);
                    return;
                }
            }
        }
    }
    #endregion

    #region 身分證
    protected void TB_HPersonID_TextChanged(object sender, EventArgs e)
    {
        #region 重複&格式判斷
        if (!string.IsNullOrEmpty(TB_HPersonID.Text.ToUpper().Trim()))
        {

            //重複判斷
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT HPersonID FROM HMember WHERE HPersonID=@HPersonID AND HAccount <> @HAccount", con);
            con.Open();
            cmd.Parameters.AddWithValue("@HPersonID", TB_HPersonID.Text.ToUpper().Trim());
            cmd.Parameters.AddWithValue("@HAccount", ((Label)Master.FindControl("LB_HAccount")).Text);
            SqlDataReader MyEBF = cmd.ExecuteReader();
            if (MyEBF.Read())
            {
                LB_NoticePersonID.Visible = true;
                LB_NoticePersonID.Text = "此身分證已存在";
            }
            else  //沒有重複再判斷格式
            {
                //格式判斷
                if (CheckId(TB_HPersonID.Text.ToUpper().Trim()) == false)
                {
                    LB_NoticePersonID.Visible = true;
                    LB_NoticePersonID.Text = "身分證格式有誤";
                }
                else
                {
                    LB_NoticePersonID.Visible = false;
                }

            }
            MyEBF.Close();
            con.Close();
            cmd.Cancel();
        }
        else
        {
            LB_NoticePersonID.Visible = false;
        }



        #endregion

        RegScript();//執行js
    }

    #region //檢查身分證字號格式
    public bool CheckId(string user_id)
    {
        int[] uid = new int[10]; //數字陣列存放身分證字號用
        int chkTotal; //計算總和用

        //初始值
        int Result = 1;

        if (user_id.Length == 10) //檢查長度
        {
            user_id = user_id.ToUpper(); //將身分證字號英文改為大寫

            //將輸入的值存入陣列中
            for (int i = 1; i < user_id.Length; i++)
            {
                uid[i] = Convert.ToInt32(user_id.Substring(i, 1));
            }
            //將開頭字母轉換為對應的數值
            switch (user_id.Substring(0, 1).ToUpper())
            {
                case "A": uid[0] = 10; break;
                case "B": uid[0] = 11; break;
                case "C": uid[0] = 12; break;
                case "D": uid[0] = 13; break;
                case "E": uid[0] = 14; break;
                case "F": uid[0] = 15; break;
                case "G": uid[0] = 16; break;
                case "H": uid[0] = 17; break;
                case "I": uid[0] = 34; break;
                case "J": uid[0] = 18; break;
                case "K": uid[0] = 19; break;
                case "L": uid[0] = 20; break;
                case "M": uid[0] = 21; break;
                case "N": uid[0] = 22; break;
                case "O": uid[0] = 35; break;
                case "P": uid[0] = 23; break;
                case "Q": uid[0] = 24; break;
                case "R": uid[0] = 25; break;
                case "S": uid[0] = 26; break;
                case "T": uid[0] = 27; break;
                case "U": uid[0] = 28; break;
                case "V": uid[0] = 29; break;
                case "W": uid[0] = 32; break;
                case "X": uid[0] = 30; break;
                case "Y": uid[0] = 31; break;
                case "Z": uid[0] = 33; break;
            }
            //檢查第一個數值是否為1.2(判斷性別)
            if (uid[1] == 1 || uid[1] == 2)
            {
                chkTotal = (uid[0] / 10 * 1) + (uid[0] % 10 * 9);

                int k = 8;
                for (int j = 1; j < 9; j++)
                {
                    chkTotal += uid[j] * k;
                    k--;
                }

                chkTotal += uid[9];

                if (chkTotal % 10 != 0)
                {
                    return false;
                    //Response.Write("身分證字號錯誤");
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
                //Response.Write("身分證字號錯誤");
            }
        }
        else
        {
            return false;
            //Response.Write("身分證字號長度錯誤");
        }

    }
    #endregion

    #endregion

    #region 上傳國稅局DropDownList
    //WA20240221_Checkbox改為DropDownList
    protected void DDL_HUploadIRS_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DDL_HUploadIRS.SelectedValue == "1" && DDL_InvoiceType.SelectedValue == "1")
        {
            Div_HPersonID.Visible = true;
        }
        else
        {
            Div_HPersonID.Visible = false;
        }
    }
    #endregion

    #region 上傳國稅局Checkbox<尚未使用>
    protected void CB_HUploadIRS_CheckedChanged(object sender, EventArgs e)
    {
        if (CB_HUploadIRS.Checked && DDL_InvoiceType.SelectedValue == "1")
        {
            Div_HPersonID.Visible = true;
        }
        else
        {
            Div_HPersonID.Visible = false;
        }
    }
    #endregion

}