using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HContains; //讓Contains不判斷大小寫
using System.Net;
using System.Text;
using System.Collections.Specialized;
using System.Activities.Expressions;
using System.Globalization;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.Security;



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


public partial class HochiMaster : System.Web.UI.MasterPage
{
    string ConStr = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

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

        #region EA20240415_程式碼優化<待測試>
        //if (Session["HAccount"] != null && Session["Login"] != null)
        //{
        //    if (!string.IsNullOrEmpty(Session["HAccount"].ToString()) && Session["Login"].ToString() == "OK")
        //    {
        //        HAccount = Session["HAccount"].ToString();

        //        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HUserName, HAccount, HEIPUid FROM HMember WHERE HAccount ='" + Session["HAccount"].ToString() + "' AND HStatus=1");
        //        if (dr.Read())
        //        {
        //            HUserName = dr["HUserName"].ToString();
        //            HUserHID = dr["HID"].ToString();
        //            EIPUID = dr["HEIPUid"].ToString();
        //            AccountHID = dr["HID"].ToString();
        //        }
        //        dr.Close();
        //        LonginStatus = 1;
        //    }

        //}
        //else if (Request.Cookies["HochiInfo"] != null)
        //{

        //    if (!string.IsNullOrEmpty(Request.Cookies["HochiInfo"]["HAccount"]) && !string.IsNullOrEmpty(Request.Cookies["HochiInfo"]["Login"]) && !string.IsNullOrEmpty(Request.Cookies["HochiInfo"]["Remember"]))
        //    {
        //        if (Request.Cookies["HochiInfo"]["Login"].ToString() == "OK")
        //        {
        //            HAccount = Request.Cookies["HochiInfo"]["HAccount"].ToString();
        //            SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HUserName, HAccount, HEIPUid FROM HMember WHERE HAccount ='" + Request.Cookies["HochiInfo"]["HAccount"].ToString() + "' AND HStatus=1");
        //            if (dr.Read())
        //            {
        //                HUserName = dr["HUserName"].ToString();
        //                HUserHID = dr["HID"].ToString();
        //                EIPUID = dr["HEIPUid"].ToString();
        //                AccountHID = dr["HID"].ToString();
        //            }
        //            dr.Close();

        //            Response.Cookies["HochiInfoHAccount"].Value = HAccount;
        //            Response.Cookies["HochiInfoHUserName"].Value = HUserName;
        //            Response.Cookies["HochiInfoHUserHID"].Value = HUserHID;
        //            Response.Cookies["HochiInfoEIPUID"].Value = EIPUID;

        //            Session["HAccount"] = Request.Cookies["HochiInfo"]["HAccount"].ToString();
        //            Session["HUserName"] = Request.Cookies["HochiInfo"]["HUserName"].ToString();
        //            Session["HUserHID"] = Request.Cookies["HochiInfo"]["HUserHID"].ToString();
        //            Session["EIPUID"] = Request.Cookies["HochiInfo"]["EIPUID"].ToString();

        //            LonginStatus = 1;


        //            //LoginStatus(1);  //登入中

        //            AccountHID = Request.Cookies["HochiInfo"]["HUserHID"].ToString();
        //        }

        //    }

        //}
        ////else
        ////{
        ////    LoginStatus(0);  //登出
        ////}
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
                //AA20240604_加入判斷，若資料庫沒有EIPuid則先帶0
                else
                {
                    EIPUID = "0";
                }

            }
            dr.Close();

            LonginStatus = 1;

            if (Session["HUserHID"] != null)
            {
                AccountHID = Session["HUserHID"].ToString();
            }
         
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
            LoginStatus(0, "");  //登出
        }





    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //#region 帶狀課程自動續報判斷
        //WebRequest request = WebRequest.Create("http://" + Request.Url.Authority + "/HSubscribe.ashx");
        //request.Method = "GET";
        //using (WebResponse wr = request.GetResponse())
        //{
        //	//在这里对接收到的页面内容进行处理 
        //}
        //#endregion

        #region 作業缺繳通知
        //WebRequest HWNotice = WebRequest.Create("http://" + Request.Url.Authority + "/HWNotice.ashx");
        ////WebRequest HWNotice = WebRequest.Create("https://" + Request.Url.Authority + "/HWNotice.ashx");
        //HWNotice.Method = "GET";
        //using (WebResponse wr = HWNotice.GetResponse())
        //{
        //	//在这里对接收到的页面内容进行处理 
        //}
        #endregion





        #region 230315- 判斷登入者身分證格式正確是否並做導頁與跳出提醒
        if (!IsPostBack)
        {
            SqlDataReader QueryPersonID = SQLdatabase.ExecuteReader("SELECT HPersonID, HPIDEmpty, HUploadIRS FROM HMember WHERE HID='" + LB_HUserHID.Text + "'");

            if (QueryPersonID.Read())
            {
                //EA20240127_上傳國稅局欄位如果是空值；系統需顯示提示
                if (string.IsNullOrEmpty(QueryPersonID["HUploadIRS"].ToString()))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "confirm", ("ConfirmUploadIRS();"), true);
                }
                else
                {
                    //判斷身分證格式
                    if (!string.IsNullOrEmpty(QueryPersonID["HPersonID"].ToString()) && Session["PersonIDNotice"] == null)
                    {
                        if (CheckId(QueryPersonID["HPersonID"].ToString().ToUpper().Trim()) == false)
                        {
                            Session["PersonIDNotice"] = 1;
                            //Response.Write("<script>alert('因身分證欄位格式錯誤，系統將引導您至學員資料修改，若不提供身分證字號請空白，未來報名課程或捐款護持將不上傳國稅局。');window.location.href='HMember_Setting.aspx?W=12'</script>");
                            Response.Redirect("HMember_Setting.aspx?W=1");
                        }
                        //else   //重複資料
                        //{
                        //                SqlDataReader QuerayRptPersonID = SQLdatabase.ExecuteReader("SELECT HPersonID FROM HMember WHERE HPersonID = '"+ QueryPersonID["HPersonID"].ToString() + "' AND HAccount <> '"+ LB_HAccount.Text + "'");
                        //                if (QuerayRptPersonID.Read())
                        //                {
                        //                    Session["PersonIDNotice"] = 1;
                        //                    Response.Redirect("HMember_Setting.aspx?W=1");
                        //                }
                        //                QuerayRptPersonID.Close();

                        //            }
                    }
                    else if (string.IsNullOrEmpty(QueryPersonID["HPersonID"].ToString()) || QueryPersonID["HPersonID"].ToString() == "") //若身分證是空白也會跳提醒導頁
                    {

                        if (!string.IsNullOrEmpty(QueryPersonID["HPIDEmpty"].ToString()) || QueryPersonID["HPIDEmpty"].ToString() == "")
                        {
                            if ((QueryPersonID["HPIDEmpty"].ToString() != "1" || QueryPersonID["HPIDEmpty"].ToString() == null))
                            {
                                //更新已跳出提醒
                                SQLdatabase.ExecuteNonQuery("UPDATE HMember SET HPIDEmpty='1' WHERE HID='" + LB_HUserHID.Text + "'");

                                //Session["PersonIDEmptyNotice"] = 1;
                                Response.Redirect("HMember_Setting.aspx?W=2");
                            }
                        }


                    }

                }


            }
            QueryPersonID.Close();
        }
        #endregion


       

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
            sign_item.Visible = false;
            LB_LoginPerson.Visible = false;
            LB_LoginPerson.Text = Session["HAccount"].ToString();
            //LBtn_Login.Visible = false;
            //LBtn_LogOut.Visible = true;

            //判斷是否有後臺權限
            if (BackPermissionCheck(HUserHID) == 1)
            {
                //AE240530_進入後台改判斷
                Li_Backend.Visible = true;
                //EnterBack.Visible = true;


                //AE20240425_加入各項權限判斷
                if (!string.IsNullOrEmpty(BackPermissionItem(HUserHID)))
                {
                    string[] gPermissionItem = BackPermissionItem(HUserHID).Split(',');

                    //AA20240425_加入判斷是否有常用後台功能的權限(CB=開課管理、GZ=點名管理、HB=同課程同日期報表、EA=幫他人報名)
                    if (gPermissionItem[0] == "1")
                    {
                        OpenCourse.Visible = true;
                    }
                    if (gPermissionItem[1] == "1")
                    {
                        Rollcall.Visible = true;
                    }
                    if (gPermissionItem[2] == "1")
                    {
                        Report.Visible = true;
                    }
                    if (gPermissionItem[3] == "1")
                    {
                        ApplyBooking.Visible = true;
                    }
                }



            }
            else
            {
                //AE240530_進入後台改判斷
                Li_Backend.Visible = false;
                //EnterBack.Visible = false;
            }
            //EE20240415_LB_HUserHID改成參數HUserHID
            SDS_ShoppingCart.SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, IIF(ISNUMERIC(b.HBCPoint )=1,FORMAT(IIF(b.HBCPoint  IS NULL,0,b.HBCPoint ),'N0'),b.HBCPoint ) AS HBCPoint,a.HCourseDonate  FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange WHERE a.HMemberID='" + HUserHID + "' AND b.HStatus='1' AND b.HVerifyStatus='2'  GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint,a.HCourseDonate";
            SDS_ShoppingCart.DataBind();
            RPT_ShoppingCart.DataBind();

            //EE20240415_LB_HUserHID改成參數HUserHID
            SqlDataReader cartnum = SQLdatabase.ExecuteReader("SELECT SUM(CAST(m.HBCPoint AS INT)*10) AS Total, COUNT(m.CartNum) AS HCartNum FROM (SELECT b.HBCPoint, a.HCourseName , COUNT(a.HCourseName) AS CartNum, a.HCourseDonate FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange AND a.HPMethod=b.HPMethod WHERE HMemberID='" + HUserHID + "' AND b.HStatus='1' AND b.HVerifyStatus='2' GROUP BY  a.HCourseName , a.HCTemplateID, a.HDateRange, b.HBCPoint, b.HPMethod, a.HCourseDonate ) AS m ");
            //SqlDataReader cartnum = SQLdatabase.ExecuteReader("SELECT COUNT(m.CartNum) AS HCartNum FROM (SELECT COUNT(a.HCourseName) AS CartNum, a.HCourseName FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange WHERE HMemberID='"+LB_HUserHID.Text+"' GROUP BY a.HCourseName, b.HBCPoint) AS m");

            if (cartnum.Read())
            {
                LB_HCartNum.Text = cartnum["HCartNum"].ToString();
                LB_HCartNum1.Text = cartnum["HCartNum"].ToString();
                LB_Total.Text = !string.IsNullOrEmpty(cartnum["Total"].ToString()) ? (Convert.ToInt32(cartnum["Total"].ToString())).ToString("N0") : "0";
            }
            cartnum.Close();


            //AA20231203_加入尚未付款訂單數量
            //SqlDataReader QueryUnpaidNum = SQLdatabase.ExecuteReader("SELECT COUNT(HOrderGroup) AS Num FROM OrderList_Front WHERE HMemberID = '" + LB_HUserHID.Text + "' AND HStatus=',1,' AND ItemStatus=',2,'");

            //AE20240229_加入判斷只抓今天日期往前推半年未付款的訂單
            //EE20240415_LB_HUserHID改成參數HUserHID；改為不抓View：OrderList_Front
            //AE20240704_修改語法不須JOIN，數量以訂單數量顯示
            //AE20240923_加入TRY_CONVERT()
            //SqlDataReader QueryUnpaidNum = SQLdatabase.ExecuteReader("SELECT DISTINCT ROW_NUMBER() OVER (ORDER BY a.HOrderGroup DESC) AS Num FROM HCourseBooking AS a  WHERE HMemberID = '" + HUserHID + "' AND (a.HStatus='1' AND a.HItemStatus='2')  AND a.HCreateDT > DATEADD(MM, -6, GETDATE())");
            SqlDataReader QueryUnpaidNum = SQLdatabase.ExecuteReader("SELECT Count(m.HOrderGroup) AS Num FROM (SELECT a.HOrderGroup, a.HMemberID , (SELECT  Convert(nvarchar, f.HItemStatus) + ',' FROM HCourseBooking AS f WHERE f.HOrderGroup = a.HOrderGroup  FOR XML PATH('')) AS HItemStatus ,(SELECT  Convert(nvarchar, g.HStatus) + ',' FROM HCourseBooking AS g WHERE g.HOrderGroup = a.HOrderGroup  FOR XML PATH('')) AS HStatus FROM HCourseBooking AS a WHERE a.HMemberID = '" + HUserHID + "' AND((HStatus = '1' AND  HItemStatus = '2') OR(HStatus LIKE '%,1,%' AND  HItemStatus LIKE '%,2,%')) AND TRY_Convert(DateTime,HCreateDT) > DATEADD(MM, -6, GETDATE()) GROUP BY a.HOrderGroup, a.HMemberID) AS m");


            if (QueryUnpaidNum.Read())
            {
                LB_Unpaid.Text = !string.IsNullOrEmpty(QueryUnpaidNum["Num"].ToString()) ? (Convert.ToInt32(QueryUnpaidNum["Num"].ToString())).ToString("N0") : "0";
            }
            QueryUnpaidNum.Close();


        }
        else
        {
            member.Visible = false;
            sign_item.Visible = true;
            LB_LoginPerson.Text = "";
            LB_LoginPerson.Visible = false;
            //LBtn_Login.Visible = true;
            //LBtn_LogOut.Visible = false;
            //EnterBack.Visible = false;
            //AE20240523_改變後台連結
            Li_Backend.Visible = false;
        }
    }

    /// <summary>
    /// 判斷是否有後臺的權限(天命法位不為空值或常態任務不為空)
    /// </summary>
    /// <param name="HID">帳號的HID</param>
    /// <returns>1：有權限，0：無權限</returns>
    protected int BackPermissionCheck(string HID)
    {
        int Result = 0;

        SqlDataReader QueryUsualTask = SQLdatabase.ExecuteReader("SELECT HID, HMemberID, HRAccess FROM HRole WHERE HMemberID LIKE '%," + HID + ",%'");


        string Access = null;

        while (QueryUsualTask.Read())
        {
            Access += QueryUsualTask["HRAccess"].ToString();
        }
        QueryUsualTask.Close();


        if (string.IsNullOrEmpty(Access))
        {
            Result = 0;
        }
        else
        {
            Result = 1;
        }

        return Result;
    }



    /// <summary>
    /// 判斷後台有哪些權限
    /// </summary>
    /// <param name="HID">帳號的HID</param>
    protected string BackPermissionItem(string HID)
    {
        string Access = "";

        SqlDataReader QueryUsualTask = SQLdatabase.ExecuteReader("SELECT HID, HMemberID, HRAccess FROM HRole WHERE HMemberID LIKE '%," + HID + ",%'");

        //AA20240425_加入判斷是否有常用後台功能的權限(CB=開課管理、GZ=點名管理、HB=同課程同日期報表、EA=幫他人報名)
        string gIntCB = "";
        string gIntGZ = "";
        string gIntHB = "";
        string gIntEA = "";

        while (QueryUsualTask.Read())
        {
            //AA20240425_加入判斷是否有常用後台功能的權限(CB=開課管理、GZ=點名管理、HB=同課程同日期報表、EA=幫他人報名)
            if (QueryUsualTask["HRAccess"].ToString().Contains("CB"))
            {
                gIntCB = "1";
            }
            if (QueryUsualTask["HRAccess"].ToString().Contains("GZ"))
            {
                gIntGZ = "1";
            }
            if (QueryUsualTask["HRAccess"].ToString().Contains("HB"))
            {
                gIntHB = "1";
            }
            if (QueryUsualTask["HRAccess"].ToString().Contains("EA"))
            {
                gIntEA = "1";
            }

        }
        QueryUsualTask.Close();


        Access = gIntCB + "," + gIntGZ + "," + gIntHB + "," + gIntEA;

        return Access;
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //抓程式檔名→routing的狀況下
        System.Web.UI.Page oPage = (System.Web.UI.Page)System.Web.HttpContext.Current.CurrentHandler;
        string gPage1 = Path.GetFileName(oPage.AppRelativeVirtualPath);
        string gPage2 = Path.GetFileName(oPage.Request.PhysicalPath);
        string gPage = oPage.Request.CurrentExecutionFilePath;

        //前台需要判斷session的畫面
        //bool A = gPage.Contains("/HCourseBooking.aspx", StringComparison.OrdinalIgnoreCase);//購買課程
        bool B = gPage.Contains("/HMember_Setting.aspx", StringComparison.OrdinalIgnoreCase);//會員管理
        bool C = gPage.Contains("/HMember_Course.aspx", StringComparison.OrdinalIgnoreCase);//課程資訊
        bool D = gPage.Contains("/HMember_Points.aspx", StringComparison.OrdinalIgnoreCase);//點數紀錄
        bool E = gPage.Contains("/HPointsBooking.aspx", StringComparison.OrdinalIgnoreCase);//點數加值
        bool F = gPage.Contains("/HMember_CourseDetail.aspx", StringComparison.OrdinalIgnoreCase); //課程詳細內容
        bool G = gPage.Contains("/HMember_CourseMaterial.aspx", StringComparison.OrdinalIgnoreCase);//課程教材
        bool H = gPage.Contains("/HQuestionnaire.aspx", StringComparison.OrdinalIgnoreCase);//課程問卷
        bool I = gPage.Contains("/HMember_CourseWork.aspx", StringComparison.OrdinalIgnoreCase);//繳交作業
        bool J = gPage.Contains("/HCourseDetail.aspx", StringComparison.OrdinalIgnoreCase);//課程內頁
        bool k = gPage.Contains("/HMember_CCPeriod.aspx", StringComparison.OrdinalIgnoreCase);//信用卡定期定額授權
        bool l = gPage.Contains("/HSystemFeedback.aspx", StringComparison.OrdinalIgnoreCase);//系統建議回饋專區


        #region 判斷是否是這些頁面，要加入session是否還存在
        if ( B == true || C == true || D == true || E == true || F == true || G == true || H == true || I == true || k == true || l==true)/*230720註解 || J == true A == true ||*/
        {
            if (Session["HAccount"] == null && Session["HUserName"] == null && Session["HUserHID"] == null)
            {
                Response.Write("<script>alert('畫面閒置太久囉~，請重新登入!');window.location.href='HLogin.aspx?Url="+ gPage.Trim('/') + "';</script>");
       
            }
        }


        #endregion
    }


    #region 登出功能
    protected void LBtn_LogOut_Click(object sender, EventArgs e)
    {
        Response.Write("<script>window.location.href='HLogout.aspx';</script>");
    }
    #endregion



    #region 進入EIP
    //protected void LBtn_EIP_Click(object sender, EventArgs e)
    //{
    //	//因master不能用form post，故導去中繼頁面做處理
    //	Response.Redirect("Post.aspx?uid="+ TB_EIPUid.Text);
    //}
    #endregion

    #region 進入EIP
    protected void LBtn_EIP_Click(object sender, EventArgs e)
    {
        //先判斷是否已登入
        //AE20240604_新增EIPUid的判斷條件
        if (Session["HUserHID"] == null || Session["EIPUID"] == null || TB_EIPUid.Text == "0")
        {
            Response.Write("<script>alert('請先登入玉成系統');window.location.href='Hlogin.aspx';</script>");
        }
        else
        {
            //Response.Redirect("Post.aspx?uid=" + TB_EIPUid.Text);
            //Response.Redirect("Post.aspx");
            Response.Write("<script>window.location.href='Post.aspx';</script>");
        }

    }
    #endregion


    #region 進入大愛光老師專欄
    //AA20240804加入
    protected void LBtn_SpecialColumn_Click(object sender, EventArgs e)
    {

        //先判斷是否已登入
        //AE20240604_新增EIPUid的判斷條件
        if (Session["HUserHID"] == null || string.IsNullOrEmpty(LB_HUserHID.Text))
        {
            Response.Write("<script>alert('請先登入玉成系統');window.location.href='Hlogin.aspx';</script>");
        }
        else
        {
            Response.Write("<script>window.location.href='HSCIndex.aspx';</script>");
        }

    }
    #endregion

    #region 前往報名/捐款清單
    protected void LBtn_ShoppingCart_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(LB_HCartNum.Text) || LB_HCartNum.Text == "0")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('您的報名/捐款清單是空的哦!');window.location.href='HCourseList.aspx';", true);
        }
        else
        {
            //Response.Redirect("HShoppingCart.aspx");
            Session["Step"] = "1";
            Response.Write("<script>window.location.href='HShoppingCart.aspx';</script>");
        }
    }
    #endregion


    protected void RPT_ShoppingCart_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HBCPoint")).Text) && ((Label)e.Item.FindControl("LB_HBCPoint")).Text != "0")
        {
            ((Label)e.Item.FindControl("LB_HBCPoint")).Text = (Convert.ToInt32(((Label)e.Item.FindControl("LB_HBCPoint")).Text.Replace(",", "")) * 10).ToString("N0");
        }

        //221013-AMY-加入捐款字樣
        if (((Label)e.Item.FindControl("LB_HCourseDonate")).Text == "1")
        {
            ((Label)e.Item.FindControl("LB_HCourseName")).Text = ((Label)e.Item.FindControl("LB_HCourseName")).Text + "<span style='color:#8548d6'>(捐款)</span>";
        }
    }

    #region 尚未付款訂單
    protected void LBtn_Unpaid_Click(object sender, EventArgs e)
    {
        if (LB_Unpaid.Text != "0")
        {
            Response.Redirect("HMember_Order.aspx?U=1");
        }
        else
        {
            Response.Write("<script>alert('您沒有尚未完成繳費的訂單哦~!');</script>");
            return;
        }
       
    }
    #endregion

    #region 上傳國稅局存檔功能
    protected void Btn_HUploadIRSSubmit_Click(object sender, EventArgs e)
    {
        SQLdatabase.ExecuteNonQuery("UPDATE HMember SET HUploadIRS = '1' WHERE HID='" + LB_HUserHID.Text + "'");

        SqlDataReader QueryPersonID = SQLdatabase.ExecuteReader("SELECT HPersonID, HPIDEmpty FROM HMember WHERE HID='" + LB_HUserHID.Text + "'");
        if (QueryPersonID.Read())
        {
            //判斷身分證格式
            if (!string.IsNullOrEmpty(QueryPersonID["HPersonID"].ToString()) && Session["PersonIDNotice"] == null)
            {
                if (CheckId(QueryPersonID["HPersonID"].ToString().ToUpper().Trim()) == false)
                {
                    Session["PersonIDNotice"] = 1;
                    //Response.Write("<script>alert('因身分證欄位格式錯誤，系統將引導您至學員資料修改，若不提供身分證字號請空白，未來報名課程或捐款護持將不上傳國稅局。');window.location.href='HMember_Setting.aspx?W=12'</script>");
                    Response.Redirect("HMember_Setting.aspx?W=1");
                }
                else
                {
                    Response.Redirect("HMember_Setting.aspx?W=3");
                }
            }
            else if (string.IsNullOrEmpty(QueryPersonID["HPersonID"].ToString()) || QueryPersonID["HPersonID"].ToString() == "") //若身分證是空白也會跳提醒導頁
            {

                if (!string.IsNullOrEmpty(QueryPersonID["HPIDEmpty"].ToString()) || QueryPersonID["HPIDEmpty"].ToString() == "")
                {
                    if ((QueryPersonID["HPIDEmpty"].ToString() != "1" || QueryPersonID["HPIDEmpty"].ToString() == null))
                    {
                        //更新已跳出提醒
                        SQLdatabase.ExecuteNonQuery("UPDATE HMember SET HPIDEmpty='1' WHERE HID='" + LB_HUserHID.Text + "'");

                        //Session["PersonIDEmptyNotice"] = 1;
                        //Response.Redirect("HMember_Setting.aspx?W=3");
                    }
                    Response.Redirect("HMember_Setting.aspx?W=3");
                }
                else
                {
                    Response.Redirect("HMember_Setting.aspx?W=3");
                }


            }
        }
        QueryPersonID.Close();

        //Response.Write("<script>window.location.href='HMember_Setting.aspx?W=3';</script>");
        //Response.Write("<script>alert('設定成功! 系統將引導您至學員資料確認');window.location.href='HMember_Setting.aspx?W=3';</script>");
    }

    protected void Btn_HUploadIRSNSubmit_Click(object sender, EventArgs e)
    {
        SQLdatabase.ExecuteNonQuery("UPDATE HMember SET HUploadIRS = '0' WHERE HID='" + LB_HUserHID.Text + "'");

        SqlDataReader QueryPersonID = SQLdatabase.ExecuteReader("SELECT HPersonID, HPIDEmpty FROM HMember WHERE HID='" + LB_HUserHID.Text + "'");
        if (QueryPersonID.Read())
        {
            //判斷身分證格式
            if (!string.IsNullOrEmpty(QueryPersonID["HPersonID"].ToString()) && Session["PersonIDNotice"] == null)
            {
                if (CheckId(QueryPersonID["HPersonID"].ToString().ToUpper().Trim()) == false)
                {
                    Session["PersonIDNotice"] = 1;
                    //Response.Write("<script>alert('因身分證欄位格式錯誤，系統將引導您至學員資料修改，若不提供身分證字號請空白，未來報名課程或捐款護持將不上傳國稅局。');window.location.href='HMember_Setting.aspx?W=12'</script>");
                    Response.Redirect("HMember_Setting.aspx?W=1");
                }
            }
            else if (string.IsNullOrEmpty(QueryPersonID["HPersonID"].ToString()) || QueryPersonID["HPersonID"].ToString() == "") //若身分證是空白也會跳提醒導頁
            {

                if (!string.IsNullOrEmpty(QueryPersonID["HPIDEmpty"].ToString()) || QueryPersonID["HPIDEmpty"].ToString() == "")
                {
                    if ((QueryPersonID["HPIDEmpty"].ToString() != "1" || QueryPersonID["HPIDEmpty"].ToString() == null))
                    {
                        //更新已跳出提醒
                        SQLdatabase.ExecuteNonQuery("UPDATE HMember SET HPIDEmpty='1' WHERE HID='" + LB_HUserHID.Text + "'");

                        //Session["PersonIDEmptyNotice"] = 1;
                        //Response.Redirect("HMember_Setting.aspx?W=2");
                    }

                }


            }
        }
        QueryPersonID.Close();

        //Response.Write("<script>window.location.href='HMember_Setting.aspx?W=3';</script>");
        //Response.Write("<script>alert('設定成功! 系統將引導您至學員資料確認');window.location.href='HMember_Setting.aspx?W=3';</script>");
    }

    #endregion

   

}
