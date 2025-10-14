using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class HLogin : System.Web.UI.Page
{
    SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

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
        if (!IsPostBack)
        {
            if (Request.Cookies["HochiInfo"] != null)
            {
                if (!string.IsNullOrEmpty(Request.Cookies["HochiInfo"]["HAccount"]))
                //&& !string.IsNullOrEmpty(Request.Cookies["HochiInfo"]["HPassword"])
                {
                    CB_Remember.Checked = true;
                    //是否記住帳號的顯示
                    TB_Account.Attributes.Add("value", Request.Cookies["HochiInfo"]["HAccount"]);
                    //TB_Password.Attributes.Add("value", Request.Cookies["HochiInfo"]["HPassword"]);
                }
            }

      
        }

    }

    #region 登入功能
    protected void Btn_Login_Click(object sender, EventArgs e)
    {
        string alert = FormCheck();   //驗證格式

        //驗證帳號密碼
        if (string.IsNullOrEmpty(alert))
        {

            #region HASH雜湊
            //GA240926_將密碼hash存入DB
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] gHashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(TB_Password.Text.Trim()));
            #endregion


            //連結資料庫
            Conn.Open();

            string PostedSQL = "SELECT HID,HUserName,HAccount,HPassword,HStatus, HEIPUid, HLoginType, HOTPStatus FROM HMember WHERE HAccount = @HAccount And HPassword =@password;";

            SqlCommand cmd = new SqlCommand(PostedSQL, Conn);
            cmd.Parameters.AddWithValue("@HAccount", TB_Account.Text.Trim());
            cmd.Parameters.AddWithValue("@password", Convert.ToBase64String(gHashValue));
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                if (dr["HStatus"].ToString() == "1" && dr["HOTPStatus"].ToString() == "1")
                {
                    Session["HAccount"] = dr["HAccount"].ToString();//使用者的帳號(信箱)
                    Session["Login"] = "OK";

               
                    // --帳號密碼驗證成功，才能獲得Session("login") = "OK"的鑰匙
                    Session.Timeout = 2880;  //分鐘                    

                    //記住帳號
                    if (CB_Remember.Checked == true)
                    {
                        Response.Cookies["HochiInfo"]["HAccount"] = dr["HAccount"].ToString();
                        Response.Cookies["HochiInfo"]["Login"] = "OK";
                        Response.Cookies["HochiInfo"]["Remember"] = "Y";
                        Response.Cookies["HochiInfo"].Expires = DateTime.Now.AddYears(1);//記住帳號到期日 

                        Response.Cookies["HochiInfoHAccount"].Value = dr["HAccount"].ToString();

                        Response.Cookies["HochiInfoHAccount"].Expires = DateTime.Now.AddYears(1);//記住帳號到期日
                    }
                    else
                    {
                        Response.Cookies["HochiInfo"]["HAccount"] = "";
                        Response.Cookies["HochiInfo"].Expires = DateTime.Now.AddDays(-1);//直接過期
                        Response.Cookies["HochiInfoHAccount"].Value = "";
                        Response.Cookies["HochiInfoHAccount"].Expires = DateTime.Now.AddDays(-1);//直接過期
                    }


                    //判斷重覆登入方式(將登入時加入的session與Application變數的GLOBAL_USER_LIST進行比較，如果Application變數有找到該賬號，代表該賬號已經登入，所以deny)
                    if (Session["HAccount"] != null)
                    {
                        string strUserId = Session["HAccount"].ToString();
                        ArrayList list = Application.Get("GLOBAL_USER_LIST") as ArrayList;
                        if (list == null)
                        {
                            list = new ArrayList();
                        }

                        //登入成功，帳號加入Application變數
                        list.Add(strUserId);
                        Application.Add("GLOBAL_USER_LIST", list);
                    }

                    //進入
                    if (Session["T"] != null)
                    {
                        if (Session["T"].ToString() == "Rollcall" && Session["CourseHID"] != null && Session["CourseDate"] != null)
                        {
                            Response.Cookies["HochiInfo"]["HAccount"] = dr["HAccount"].ToString();
                            Response.Cookies["HochiInfo"].Expires = DateTime.Now.AddDays(1);

                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Rollcall", "<script>window.location.href='HRollcall.aspx?CID=" + Session["CourseHID"].ToString() + "&D=" + Session["CourseDate"].ToString() + "';</script>");

                            //清除session
                            Session.Contents.Remove("T");
                            Session.Contents.Remove("CourseHID");
                            Session.Contents.Remove("CourseDate");

                            return;
                        }


                    }
                    //GA20240907_從專欄頁面尚未登入，登入後要導回專欄首頁
                    else if (Request.QueryString["F"] != null)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "SCIndex", "<script>window.location.href='HSCIndex.aspx';</script>");
                    }
                    //GA20240920_加入從首頁點快速報到，尚未登入會導回首頁跳出Modal
                    else if (Request.QueryString["R"] != null)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "QuickRollcall", "<script>window.location.href='HIndex.aspx?R=1';</script>");
                    }
                    //GA20250823_從體悟分享紀錄區尚未登入，登入後要導回體悟分享紀錄區首頁
                    else if (Request.QueryString["G"] != null)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "SCIndex", "<script>window.location.href='HFeelingsJournals.aspx';</script>");
                    }
                    //GA250309_加入需要登入的頁面導回判斷
                    else if (Request.QueryString["Url"] != null)
                    {
                        if (Request.QueryString["Batch"] != null)
                        {
                            string gUrl = Request.QueryString["Url"].ToString() + "?Batch=" + Request.QueryString["Batch"].ToString();
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "PostBack", "<script>window.location.href='" + gUrl + "';</script>");
                        }
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "PostBack", "<script>window.location.href='" + Request.QueryString["Url"].ToString() + "';</script>");
                        }

                    }
                    else
                    {
                        Session["CKUrl"] = "HIndex.aspx";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Index", "<script>window.location.href='HIndex.aspx';</script>");
                    }


                }
                else
                {
                    if (dr["HOTPStatus"].ToString() == "0")
                    {
                        Session["LoginAccount"] = TB_Account.Text.Trim();

                        if (dr["HLoginType"].ToString() == "1")  //信箱
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "AlertEmail", @"<script>alert('您的信箱尚未通過驗證~!\n系統將引導您重新驗證哦，謝謝~!');window.location.href='HValidation.aspx?Type=1';</script>");
                        }
                        else if(dr["HLoginType"].ToString() == "2")  //手機
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "AlertEmail", @"<script>alert('您的手機尚未通過驗證~!\n系統將引導您重新驗證哦，謝謝~!');window.location.href='HValidation.aspx?Type=2';</script>");
                        }
                        else if (dr["HLoginType"].ToString() == "3")  //自訂帳號+手機
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "AlertEmail", @"<script>alert('您的手機尚未通過驗證~!\n系統將引導您重新驗證哦，謝謝~!');window.location.href='HValidation.aspx?Type=3';</script>");
                        }

                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "<script>alert('帳號目前停用中~請洽玉成體夥伴協助開通哦~謝謝~!');</script>");
                        return;
                    }

                }

               
            }
            else
            {
                SqlDataReader UnValidDr = SQLdatabase.ExecuteReader("SELECT HID,HUserName,HAccount,HPassword,HStatus, HEIPUid FROM HMember WHERE HAccount ='" + TB_Account.Text.Trim() + "'");

                if (UnValidDr.Read())
                {
                    if (UnValidDr["HStatus"].ToString() == "0")
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", @"<script>alert('帳號目前停用中~請洽玉成體夥伴協助開通哦~謝謝~!');</script>");
                        return;
                    }
                    else
                    {
                        if (Convert.ToBase64String(gHashValue) != UnValidDr["HPassword"].ToString())
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", @"<script>alert('密碼輸入錯誤~\n若忘記密碼可以點選畫面中右下角忘記密碼，重新設定哦~!');</script>");
                            return;
                        }
                    }
                   
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "<script>alert('查無此帳號哦~歡迎註冊EDU系統~謝謝~!');</script>");
                    return;
                }
                UnValidDr.Close();

                

            }
            cmd.Cancel();
            dr.Close();
            Conn.Close();
            Conn.Dispose();
            //SqlConnection.ClearAllPools(); //強制清除連線池中未使用的連線
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Alert", "<script>alert('" + alert + "');</script>");
            return;
        }

    }
    #endregion



   

    //檢查欄位空值&格式判斷
    protected string FormCheck()
    {
        string alert = "";
        if (string.IsNullOrEmpty(TB_Account.Text.Trim())) alert += @"請輸入帳號名稱。\n";
        if (string.IsNullOrEmpty(TB_Password.Text.Trim())) alert += @"請您輸入密碼。\n";
        //  if (ValidateCode() != 1) alert += @"驗證碼輸入錯誤(不分大小寫)。\n";
        return alert;
    }

    /// <summary>
    /// 判斷驗證碼是否輸入正確
    /// </summary>
    /// <returns>1；正確，0：錯誤</returns>
    protected int ValidateCode()
    {
        int Result = 0;
        string UserEnter = TB_ValidNo.Text.ToLower();
        string Code = Server.UrlDecode(Session["ValidateCode"].ToString()).ToString().ToLower();
        if (UserEnter == Code) Result = 1;
        return Result;
    }
}