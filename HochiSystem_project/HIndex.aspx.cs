
using NPOI.SS.Formula.Functions;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IdentityModel.Claims;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient; // MySQL

public partial class HIndex : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);


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


    #region --根目錄--
    string CourseImg = "../uploads/Course/";
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


    protected void Page_LoadComplete(object sender, EventArgs e)
    {
    }


    protected void Page_Load(object sender, EventArgs e)
    {

        #region 宣告dataTable框架
        System.Data.DataTable dtRC = new System.Data.DataTable("dtRC");
        dtRC.Columns.Add("HCourseID", typeof(String));
        dtRC.Columns.Add("HMemberID", typeof(String));
        dtRC.Columns.Add("HRCDate", typeof(String));
        dtRC.Columns.Add("HAStatus", typeof(String));
        dtRC.Columns.Add("HStatus", typeof(String));
        dtRC.Columns.Add("HCreate", typeof(String));
        dtRC.Columns.Add("HCreateDT", typeof(String));

        ViewState["dtRC"] = dtRC;
        //清除DataTable以釋放記憶體(如果不需要或有異常可先註解)
        dtRC.Clear();
        #endregion


        #region 課程連結註解
        if (!IsPostBack)
        {

            if (string.IsNullOrEmpty(((Label)Master.FindControl("LB_HUserHID")).Text))
            {
                CourseLink.Style.Add("display", "none");
            }
            else
            {
                string Today = DateTime.Now.ToString("yyyy/MM/dd");

                //AE20231205_調整寫法
                //AE20250911_新增師娘
                if (((Label)Master.FindControl("LB_HUserHID")).Text == "9390" || ((Label)Master.FindControl("LB_HUserHID")).Text == "9391") //大愛光老師 & 師娘
                {
                    //AA20240325_老師不用報名或有報名的課程連結顯示
                    SDS_HCourseLink.SelectCommand = "SELECT MAX(A.HID) AS HCourseID, A.HCourseName AS HCName , A.HCourseLink, A.HDateRange, A.HSATCourseLink, A.HSUNCourseLink, '1' AS HAttend FROM HCourse AS A WHERE(A.HCourseLink<> null OR A.HCourseLink<> '' OR A.HSATCourseLink<> '' OR A.HSUNCourseLink<> '')  AND A.HTeacherName LIKE '%81,%' AND A.HStatus = 1 AND A.HVerifyStatus = 2 AND DateDiff(Day, left(A.HDateRange, 10),getdate()) >= 0 AND DateDiff(Day, RIGHT(A.HDateRange, 10),getdate()) <= 0 AND(A.HOCPlace = '1' OR A.HShowZoom = '1') GROUP BY A.HCourseName, A.HDateRange, A.HCourseLink, A.HSATCourseLink, A.HSUNCourseLink UNION SELECT A.HCourseID, A.HCourseName AS HCName,  A.HDateRange, A.HCourseLink, A.HSATCourseLink, A.HSUNCourseLink,  '1' AS HAttend FROM OrderList_Detail AS A WHERE (A.HCourseLink <> null OR A.HCourseLink <> '' OR A.HSATCourseLink <> '' OR A.HSUNCourseLink <> '')  AND A.HStatus = 1 AND A.HItemStatus = 1  AND A.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'  AND A.HCourseID <> '2045'  AND A.HCourseID <> '332' AND DateDiff(Day,left(A.HDateRange, 10),getdate()) >= 0 AND DateDiff(Day,RIGHT(A.HDateRange, 10),getdate()) <= 0 AND (A.HOCPlace ='1' OR A.HShowZoom ='1') GROUP BY A.HMemberID, A.HAttend,A.HCourseID, A.HCourseName, A.HPlaceName, A.HCourseName, A.HDateRange, A.HCourseLink, A.HSATCourseLink, A.HDate, A.HSUNCourseLink";

  


                    //AE20231129_改掉不用再join HCourse&HOCPlace
                    //AE20240104_OrderList_Merge改用OrderList_Detail
                    //AA20240325_老師不用報名或有報名的課程連結顯示
                    SDS_Replay.SelectCommand = "SELECT  MAX(A.HID)  AS HCourseID,A.HCourseName AS HCName ,A.HDateRange, A.HCourseLinkRelay, '5' AS HAttend FROM HCourse AS A WHERE (A.HCourseLinkRelay <> null OR A.HCourseLinkRelay <> '') AND A.HTeacherName LIKE '%81,%'   AND A.HStatus = 1 AND A.HVerifyStatus = 2 AND DateDiff(Day, left(A.HDateRange, 10),getdate()) >= 0 AND DateDiff(Day, RIGHT(A.HDateRange, 10),getdate()) <= 0 AND(A.HOCPlace = '1' OR A.HShowZoom = '1') GROUP BY  A.HCourseName, A.HDateRange, A.HCourseLinkRelay UNION SELECT  A.HCourseID, A.HCourseName AS HCName, A.HDateRange, A.HCourseLinkRelay,  HAttend FROM OrderList_Detail AS A LEFT JOIN HCourseBooking_Group AS D ON A.HID=D.HBookingID WHERE (A.HCourseLinkRelay <> null OR A.HCourseLinkRelay <> '') AND A.HStatus=1  AND A.HItemStatus = 1 AND A.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'   AND (A.HAttend='5' OR A.HAttend = '6') AND A.HCourseID <> '332'   AND A.HCourseID <> '2045' AND DateDiff(Day,left(A.HDateRange, 10),getdate()) >= 0 AND DateDiff(Day,RIGHT(A.HDateRange, 10),getdate()) <= 0 GROUP BY A.HMemberID, A.HAttend,A.HCourseID, A.HPlaceName, A.HCourseName, A.HDateRange,A.HDate,A.HCourseLinkRelay";



                }
                else
                {
                    //AE20231129_改掉不用join HCourse&HOCPlace
                    //AE20240103_課程連結不用join 體系護持工作
                    //AE20240104_OrderList_Merge改用OrderList_Detail
                    SDS_HCourseLink.SelectCommand = "SELECT A.HMemberID, A.HAttend,A.HCourseID, (A.HCourseName + '_' + A.HPlaceName) AS HCName, A.HCourseName,  A.HDate AS HDateRange, A.HCourseLink, A.HSATCourseLink, A.HSUNCourseLink FROM OrderList_Detail AS A  WHERE (A.HCourseLink <> null OR A.HCourseLink <> '' OR A.HSATCourseLink <> '' OR A.HSUNCourseLink <> '')  AND A.HStatus = 1 AND A.HItemStatus = 1  AND (A.HAttend = '1' OR A.HAttend = '6' OR A.HAttend LIKE '%,1,%' OR A.HAttend LIKE '%,6,%')  AND A.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'  AND A.HCourseID <> '2045'  AND A.HCourseID <> '332' AND DateDiff(Day,left(A.HDateRange, 10),getdate()) >= 0 AND DateDiff(Day,RIGHT(A.HDateRange, 10),getdate()) <= 0 AND (A.HOCPlace ='1' OR A.HShowZoom ='1') GROUP BY A.HMemberID, A.HAttend,A.HCourseID, A.HCourseName, A.HPlaceName, A.HCourseName, A.HDateRange, A.HDate,A.HCourseLink, A.HSATCourseLink, A.HSUNCourseLink ";


                    //AE20231129_改掉不用join HCourse&HOCPlace

                    //AE20240104_OrderList_Merge改用OrderList_Detail，因同修會有沒填護持日期的情況，所以暫不加入護持日期的條件判斷
                    SDS_Task.SelectCommand = "SELECT A.HMemberID, A.HAttend,A.HCourseID, (A.HCourseName+'_'+A.HPlaceName) AS HCName, A.HCourseName, A.HDate AS HDateRange,A.HCourseLinkTask FROM OrderList_Detail AS A LEFT JOIN HCourseBooking_Group AS D ON A.HID=D.HBookingID WHERE  (A.HCourseLinkTask <> null OR A.HCourseLinkTask <> '') AND  A.HStatus=1  AND A.HItemStatus = 1 AND A.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND  (((A.HAttend='5'  OR A.HAttend = '6'  OR A.HAttend LIKE '%,5,%' OR A.HAttend LIKE '%,6,%') AND (D.HGroupID NOT IN(45,15,42) OR D.HGroupID IS NULL )) OR ((A.HAttend = '1'  OR A.HAttend = '6' OR A.HAttend LIKE '%,1,%' OR A.HAttend LIKE '%,6,%')  AND D.HGroupID NOT IN(45,15,42) AND D.HGroupID IS NOT NULL))  AND A.HCourseID <> '332'   AND A.HCourseID <> '2045' AND DateDiff(Day,left(A.HDateRange, 10),getdate()) >= 0　AND DateDiff(Day,RIGHT(A.HDateRange, 10),getdate()) <= 0 GROUP BY A.HMemberID, A.HAttend,A.HCourseID, A.HPlaceName, A.HCourseName, A.HDateRange,A.HDate, A.HCourseLinkTask  ";



                    //AE20231129_改掉不用再join HCourse&HOCPlace
                    //AE20240104_OrderList_Merge改用OrderList_Detail&加入護持日期為當天才會顯示
                    SDS_Replay.SelectCommand = "SELECT A.HMemberID, A.HAttend,A.HCourseID, (A.HCourseName+'_'+A.HPlaceName) AS HCName, A.HCourseName, A.HDate AS HDateRange, A.HCourseLinkRelay FROM OrderList_Detail AS A LEFT JOIN HCourseBooking_Group AS D ON A.HID=D.HBookingID WHERE (A.HCourseLinkRelay <> null OR A.HCourseLinkRelay <> '') AND A.HStatus=1  AND A.HItemStatus = 1 AND A.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'   AND (D.HGroupID ='45' OR D.HGroupID ='15' OR D.HGroupID ='42' )    AND A.HCourseID <> '332'   AND A.HCourseID <> '2045' AND DateDiff(Day,left(A.HDateRange, 10),getdate()) >= 0　AND DateDiff(Day,RIGHT(A.HDateRange, 10),getdate()) <= 0 AND D.HGDay='" + Today + "'  GROUP BY A.HMemberID, A.HAttend,A.HCourseID, A.HPlaceName, A.HCourseName, A.HDateRange,  A.HDate, A.HCourseLinkRelay";







                }



                Rpt_HCourseLink.DataSourceID = "SDS_HCourseLink";
                Rpt_HCourseLink.DataBind();

                Rpt_Task.DataSourceID = "SDS_Task";
                Rpt_Task.DataBind();

                Rpt_Replay.DataSourceID = "SDS_Replay";
                Rpt_Replay.DataBind();

                CourseLink.Style.Add("display", "block");


                if (Rpt_HCourseLink.Items.Count != 0)
                {
                    Ul_Courselink.Visible = true;
                }
                else
                {
                    Ul_Courselink.Visible = false;
                }


                if (Rpt_Task.Items.Count != 0)
                {
                    Ul_TaskArea.Visible = true;
                }
                else
                {
                    Ul_TaskArea.Visible = false;
                }

                if (Rpt_Replay.Items.Count != 0)
                {
                    Ul_RelayArea.Visible = true;
                }
                else
                {
                    Ul_RelayArea.Visible = false;
                }

                if (Rpt_HCourseLink.Items.Count == 0 && Rpt_Task.Items.Count == 0 && Rpt_Replay.Items.Count == 0)
                {
                    CourseLink.Style.Add("display", "none");
                }


            }

            // 取登入者 HID（有登入時 Master 已設定好）
            var hidLabel = (Label)Master.FindControl("LB_HUserHID");
            var myHid = hidLabel != null ? hidLabel.Text : null;
            // 瀏覽器 Console
            ScriptManager.RegisterStartupScript(
                this, GetType(), "logMyHid",
                "console.log('myHid(client)=', " +
                (myHid == null ? "null" : "'" + HttpUtility.JavaScriptStringEncode(myHid) + "'")
                + ");",
                true
            );
            if (!string.IsNullOrEmpty(myHid))
            {
                pnQuickLead.Visible = true;
                var url = ResolveUrl("~/NewFriend.aspx?myHid=" + Server.UrlEncode(myHid) + "&channel=" + Server.UrlEncode("活動"));
                // 因為是 <a runat="server">，用 Attributes 設定 href
                lnkQuickLead.Attributes["href"] = url;
            }
            else
            {
                pnQuickLead.Visible = false; // 沒登入就不顯示
            }
        }

        #endregion


        #region 快速報到_抓當天有報名的課程資訊
        //HCourseID=2045(身強體壯功十八式真傳)、HCourseID=2045(有聲書_識透生命真相)
        //AA20240917

        if (!IsPostBack)
        {

            //AA250308_新增串接EIP的訊息公告資訊
            SDS_HEIPBroadcast.SelectCommand = "SELECT b_id, b_subunitname, b_title, b_content, b_cdate, b_udate FROM broadcast WHERE (REPLACE(b_subunitname, ' ', '') LIKE '%課程%' OR REPLACE(b_subunitname, ' ', '') LIKE '%活動%') ORDER BY b_udate DESC LIMIT 6 OFFSET 0;";
            Rpt_HEIPBroadcast.DataBind();

        }


        #endregion


        //AA20240920_新增快速報到回傳的部分
        if (Request.QueryString["R"] != null)
        {
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_Rollcall').modal();"), true);
        }
    }

    #region 輪播圖
    protected void Rpt_HIndexSlide_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        HtmlGenericControl slide = e.Item.FindControl("slide") as HtmlGenericControl;
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_SlideName")).Text))
        {
            slide.Attributes.Add("style", " background-image:url('uploads/IndexSlide/" + ((Label)e.Item.FindControl("LB_SlideName")).Text + "'); ");
        }
    }
    #endregion

    #region 送出留言
    protected void LBtn_Submit_Click(object sender, EventArgs e)
    {
        if (TB_Email.Text.Trim() != "")        //如果不是空的，才驗證格式--mail
        {
            string a = TB_Email.Text.Trim();
            string pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";         //正規運算式，可到http://rubular.com/驗證
            if (!Regex.IsMatch(a, pattern))
            {
                Response.Write("<script>alert('請輸入正確的信箱格式(ex: test@mail.com)');</script>");
                return;
            }
        }
        string alert = chkValidation();   //驗證格式

        if (string.IsNullOrEmpty(alert))
        {
            //*********************寄信開始***********************/
            //信件本體宣告
            MailMessage mail = new MailMessage();

            // 寄件者, 收件者和副本郵件地址
            mail.From = new MailAddress(Sender, "HochiSystem Mail");

            // 設定收件者
            //mail.To.Add(new MailAddress("mail.edu@hochi.org.tw"));

            //AE20230802_更改新的收件者
            mail.To.Add(new MailAddress("spiritual_life@hochi.org.tw"));


            // 優先等級
            mail.Priority = MailPriority.High;

            //主旨
            mail.Subject = "來自和氣大愛玉成系統的訊息";
            mail.SubjectEncoding = Encoding.UTF8;

            //信件內容
            mail.Body = "來自和氣大愛玉成系統的諮詢內容：<br/><br/>" +
            "姓名：" + TB_Name.Text.Trim() + "<br/>" +
            "電子信箱：" + TB_Email.Text.Trim() + "<br/>" +
            "諮詢內容：" + TA_Content.InnerText.Trim() + "<br/><br/><hr/>" +
            "此信件為系統自動發送，請勿回信!";
            mail.BodyEncoding = Encoding.GetEncoding("utf-8");
            mail.IsBodyHtml = true;
            SmtpClient smtpServer = new SmtpClient(EHost, EPort);
            smtpServer.Credentials = new System.Net.NetworkCredential(EAcount, EPasword);
            smtpServer.EnableSsl = EEnabledSSL;
            try
            {
                // 寄出郵件
                smtpServer.Send(mail);
                Response.Write("<script>alert('您的留言成功送出囉~!會有專人盡快與您聯絡~!');window.location.href='HIndex.aspx';</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('訊息寄出失敗~請重新送出~');</script>");
            }
        }
        else
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert", "<script>alert('" + alert + "');</script>");
        }

        //**************寄信結束*****************//

    }
    protected string chkValidation()
    {
        string alert = "";

        //必填欄位判斷
        if (Session["ValidateCode"] != null)
        {
            if (TB_Captcha.Text.ToLower() != Session["ValidateCode"].ToString().ToLower()) alert += "請輸入正確的驗證碼";
        }
        else
        {
            Response.Write("<script>alert('畫面停留太久囉，請重新整理頁面');</script>");
        }
        bool value = CB_Subscribe.Checked;
        if (value == false) alert += "請先閱讀並同意個資保護說明";
        return alert;
    }
    #endregion




    #region 最新課程
    protected void Rpt_Course_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //取得Repeater的DataRowView
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        if (!string.IsNullOrEmpty(((Label)Master.FindControl("LB_HUserHID")).Text))
        {
            //判斷是否已存在購物車中(課程)LBtn_CourseBooking
            SqlDataReader QueryHSC = SQLdatabase.ExecuteReader("SELECT HID FROM HShoppingCart WHERE HCTemplateID='" + gDRV["HCTemplateID"].ToString() + "' AND HCourseName='" + gDRV["HCourseName"].ToString() + "' AND HDateRange='" + gDRV["HDateRange"].ToString() + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND HCourseDonate=0");
            if (QueryHSC.Read())
            {
                ((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Text = "<span class='fas fa-book-open mr-2'></span>已加入報名清單";
                ((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).CssClass = "button button-red text-white ml-0";
                ((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Enabled = false;
            }
            QueryHSC.Close();

            //判斷是否已存在購物車中(捐款)
            SqlDataReader QueryHDC = SQLdatabase.ExecuteReader("SELECT HID FROM HShoppingCart WHERE HCTemplateID='" + gDRV["HCTemplateID"].ToString() + "' AND HCourseName='" + gDRV["HCourseName"].ToString() + "' AND HDateRange='" + gDRV["HDateRange"].ToString() + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'  AND HCourseDonate=1");
            if (QueryHDC.Read())
            {
                ((LinkButton)e.Item.FindControl("LBtn_CourseDonate")).Text = "<span class='fa fa-hand-holding-heart mr-2'></span>已加入捐款清單";
                ((LinkButton)e.Item.FindControl("LBtn_CourseDonate")).CssClass = "button button-red text-white ml-0";
                ((LinkButton)e.Item.FindControl("LBtn_CourseDonate")).Enabled = false;
            }
            QueryHDC.Close();


            //AA20221012-判斷是否已報名過課程
            //SqlDataReader QueryHCB = SQLdatabase.ExecuteReader("SELECT HID FROM HCourse WHERE HCTemplateID='" + gDRV["HCTemplateID"].ToString() + "' AND HCourseName='" + gDRV["HCourseName"].ToString() + "' AND HDateRange='" + gDRV["HDateRange"].ToString() + "' AND HStatus='1' AND HVerifyStatus='2'");

            SqlDataReader QueryHCB = SQLdatabase.ExecuteReader("SELECT a.HID FROM HCourseBooking AS a LEFT JOIN HCourse AS b ON a.HCourseID = b.HID WHERE b.HCTemplateID ='" + gDRV["HCTemplateID"].ToString() + "' AND b.HCourseName = '" + gDRV["HCourseName"].ToString() + "'  AND b.HDateRange = '" + gDRV["HDateRange"].ToString() + "'  AND a.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'  AND a.HStatus = '1' AND a.HItemStatus = '1' AND b.HStatus='1' AND b.HVerifyStatus='2'");


            if (QueryHCB.Read())
            {
                ((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Text = "<span class='fas fa-book-open mr-2'></span>已完成報名";
                ((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).CssClass = "button button-gray text-white ml-0";
                ((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Enabled = false;
            }
            QueryHCB.Close();
        }


        #region 判斷學員的體系是否符合資格  --20220629暫時註解，未來要再打開
        //if (!string.IsNullOrEmpty(((Label)Master.FindControl("LB_HUserHID")).Text))
        //{
        //	SqlDataReader system = SQLdatabase.ExecuteReader("SELECT HSystemID FROM HMember WHERE HID=" + ((Label)Master.FindControl("LB_HUserHID")).Text);
        //	string systemID = ((Label)e.Item.FindControl("LB_HRSystem")).Text.TrimEnd(','); //可報名的體系
        //	if (system.Read())
        //	{
        //		if (system["HSystemID"].ToString() != "0")
        //		{
        //			string[] words = systemID.Split(',');

        //			int id = Array.IndexOf(words, system["HSystemID"].ToString());

        //			if (id == -1)
        //			{
        //				((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Text = "資格不符";
        //				((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).CssClass = "text-danger";
        //				((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Enabled = false;
        //				//ScriptManager.RegisterStartupScript(Page, GetType(), "alert", "alert('資格不符，尚無法報名');", true);
        //				//return;
        //			}
        //			else
        //			{
        //				#region 護持報名截止 --220629暫時註解，未來要再打開
        //				//DateTime gSDate = Convert.ToDateTime(((Label)e.Item.FindControl("LB_HDateRange")).Text.Substring(0, 10));//開課日
        //				//if (DateTime.Now.AddDays(3) > gSDate)//今天+3天<開課日
        //				//{
        //				//	((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Text = "報名截止";
        //				//	//((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).CssClass = "text-danger";
        //				//	((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Style.Add("background-color", "#808080");
        //				//	((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Style.Add("color", "#fff");
        //				//	((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Enabled = false;
        //				//	//Response.Write("報名截止");
        //				//}
        //				//else
        //				//{
        //				//	((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Text = "報名課程";
        //				//}
        //				#endregion
        //			}
        //		}
        //		else
        //		{
        //			((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Text = "資格不符";
        //			((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).CssClass = "text-danger";
        //			((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Enabled = false;
        //		}

        //	}
        //	system.Close();

        //}
        //else
        //{
        //	#region 護持報名截止 --220629暫時註解，未來要再打開
        //	//DateTime gSDate = Convert.ToDateTime(((Label)e.Item.FindControl("LB_HDateRange")).Text.Substring(0, 10));//開課日
        //	//if (DateTime.Now.AddDays(3) > gSDate)//今天+3天<開課日
        //	//{
        //	//	((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Text = "報名截止";
        //	//	//((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).CssClass = "text-danger";
        //	//	((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Style.Add("background-color", "#808080");
        //	//	((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Style.Add("color", "#fff");
        //	//	((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Enabled = false;
        //	//	//Response.Write("報名截止");
        //	//}
        //	//else
        //	//{
        //	//	((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Text = "報名課程";
        //	//}
        //	#endregion
        //}

        #endregion


        #region 判斷學員類別是否符合課程類別限制(暫時註解)
        //EA20231217_新增判斷學員類別是否符合課程類別限制
        //if (!string.IsNullOrEmpty(((Label)Master.FindControl("LB_HUserHID")).Text))
        //{
        //    string StrHType = null;

        //    SqlDataReader QueryHIRestriction = SQLdatabase.ExecuteReader("SELECT HType FROM HMember WHERE HID=" + ((Label)Master.FindControl("LB_HUserHID")).Text);

        //    if (QueryHIRestriction.Read())
        //    {
        //        if (QueryHIRestriction["HType"].ToString() != "0")
        //        {
        //            StrHType = QueryHIRestriction["HType"].ToString();
        //        }
        //    }
        //    QueryHIRestriction.Close();

        //    if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HIRestriction")).Text))
        //    {
        //        string[] StrLBHIRestriction = ((Label)e.Item.FindControl("LB_HIRestriction")).Text.TrimEnd(',').Split(','); //學員類別限制
        //        int ConformYN = 0;

        //        for (int i = 0; i < StrLBHIRestriction.Length; i++)
        //        {
        //            if (StrLBHIRestriction[i].ToString() == StrHType)
        //            {
        //                ConformYN = 1;
        //            }
        //        }

        //        if (ConformYN != 1)
        //        {
        //            ((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Text = "資格不符";
        //            ((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).CssClass = "text-danger";
        //            ((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Enabled = false;
        //        }

        //    }

        //}


        #endregion

        #region  是否為七天班會課程
        if (gDRV["HBookByDateYN"].ToString() =="1")
        {
            ((LinkButton)e.Item.FindControl("LBtn_QuickPay")).Visible = true;
            ((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_CourseDonate")).Visible = false;
            


            //判斷是否已有報名，若已全部日期都報完按鈕會反綠顯示已完成報名
            string memberID = ((Label)Master.FindControl("LB_HUserHID")).Text;
            string courseName = gDRV["HCourseName"].ToString();
            string dateRange = gDRV["HDateRange"].ToString();

            string[] gHDateRange = { };

            // 解析日期範圍
            if (dateRange.Contains("-")) // 連續日期
            {
                var parts = dateRange.Trim(',').Split('-');
                DateTime start = DateTime.Parse(parts[0]);
                DateTime end = DateTime.Parse(parts[1]);
                int days = (end - start).Days + 1;
                gHDateRange = Enumerable.Range(0, days)
                    .Select(i => start.AddDays(i).ToString("yyyy/MM/dd"))
                    .ToArray();
            }
            else if (dateRange.Contains(",")) // 多選日期
            {
                gHDateRange = dateRange.Trim(',').Split(',');
            }
            else // 單一日期
            {
                gHDateRange = new string[] { dateRange };
            }

            // 查出所有已報名的日期
            string gOrderedDates = null;
            List<string> bookedDates = new List<string>();
            string sql = "SELECT b.HDate, a.HItemStatus " +
        "FROM HCourseBooking AS a " +
        "INNER JOIN HCourseBooking_DateAttend AS b ON a.HID = b.HCourseBookingID " +
        "WHERE a.HCourseName = @CourseName AND a.HDateRange = @DateRange " +
        "AND a.HMemberID = @MemberID " +
        "AND a.HCourseDonate = 0 AND a.HStatus = 1 ";/*AND a.HItemStatus = 1*/
            con.Open();
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@CourseName", courseName);
                cmd.Parameters.AddWithValue("@DateRange", dateRange);
                cmd.Parameters.AddWithValue("@MemberID", memberID);

                using (SqlDataReader QueryDateBooked = cmd.ExecuteReader())
                {
                    while (QueryDateBooked.Read())
                    {
                        if (QueryDateBooked["HItemStatus"].ToString() == "1")
                        {
                            gOrderedDates += Convert.ToDateTime(QueryDateBooked["HDate"]).ToString("yyyy/MM/dd") + ",";
                            bookedDates.Add(Convert.ToDateTime(QueryDateBooked["HDate"]).ToString("yyyy/MM/dd"));
                        }
                        else
                        {
                            gOrderedDates = "0";
                        }
                    }
                }
            }
            con.Close();


            // 檢查是否所有日期都有報名
            bool allDatesBooked = gHDateRange.All(date => bookedDates.Contains(date));
            ((Label)e.Item.FindControl("LB_BookedDates")).Text = gOrderedDates;

            // 設定按鈕啟用狀態
            ((LinkButton)e.Item.FindControl("LBtn_QuickPay")).Enabled = !allDatesBooked;
            if (((LinkButton)e.Item.FindControl("LBtn_QuickPay")).Enabled == false)
            {
                ((LinkButton)e.Item.FindControl("LBtn_QuickPay")).Text = "已完成報名";
            }


            //判斷是否報名但尚未完成繳費(未付款訂單)
            //HStatus=1 (訂單成立)、HItemStatus=2 (未付款)
            SqlDataReader QueryHCBUnpaid = SQLdatabase.ExecuteReader("SELECT a.HID, a.HOrderGroup FROM HCourseBooking AS a LEFT JOIN HCourse AS b ON a.HCourseID = b.HID WHERE  b.HCTemplateID ='" + gDRV["HCTemplateID"].ToString() + "' AND b.HCourseName = N'" + gDRV["HCourseName"].ToString() + "'  AND b.HDateRange = '" + gDRV["HDateRange"].ToString() + "'  AND a.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HStatus = '1' AND a.HItemStatus = '2' AND a.HChangeStatus = '2'  AND b.HStatus='1' AND b.HVerifyStatus='2'");

            if (QueryHCBUnpaid.Read())
            {
                ((LinkButton)e.Item.FindControl("LBtn_QuickPay")).Visible = false;
                ((LinkButton)e.Item.FindControl("LBtn_Pay")).Visible = true;
                ((LinkButton)e.Item.FindControl("LBtn_Pay")).Style.Add("width","100%");
                ((LinkButton)e.Item.FindControl("LBtn_Pay")).Style.Add("color","#fff");
              ((LinkButton)e.Item.FindControl("LBtn_Pay")).CommandArgument = QueryHCBUnpaid["HOrderGroup"].ToString();
            }
            QueryHCBUnpaid.Close();


        }
        else
        {
            ((LinkButton)e.Item.FindControl("LBtn_QuickPay")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Visible = true;


            //判斷是否報名但尚未完成繳費(未付款訂單)
            //HStatus=1 (訂單成立)、HItemStatus=2 (未付款)
            SqlDataReader QueryHCBUnpaid = SQLdatabase.ExecuteReader("SELECT a.HID, a.HOrderGroup FROM HCourseBooking AS a LEFT JOIN HCourse AS b ON a.HCourseID = b.HID WHERE  b.HCTemplateID ='" + gDRV["HCTemplateID"].ToString() + "' AND b.HCourseName = N'" + gDRV["HCourseName"].ToString() + "'  AND b.HDateRange = '" + gDRV["HDateRange"].ToString() + "'  AND a.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HStatus = '1' AND a.HItemStatus = '2' AND a.HChangeStatus = '2'  AND b.HStatus='1' AND b.HVerifyStatus='2'");

            if (QueryHCBUnpaid.Read())
            {
                ((LinkButton)e.Item.FindControl("LBtn_CourseBooking")).Visible = false;
                ((LinkButton)e.Item.FindControl("LBtn_Pay")).Visible = true;
                ((LinkButton)e.Item.FindControl("LBtn_Pay")).Style.Add("color", "#fff");
                ((LinkButton)e.Item.FindControl("LBtn_Pay")).CommandArgument = QueryHCBUnpaid["HOrderGroup"].ToString();
            }
            QueryHCBUnpaid.Close();

        }
        #endregion

        //AA20230816_新增判斷:若為七彩光橋課程，加入捐款清單按鈕顯示為申請信用卡授權
        #region  
        if (gDRV["HCCPeriodYN"].ToString() == "1")
        {
            ((LinkButton)e.Item.FindControl("LBtn_CourseDonate")).Text = "<i class='fas fa-credit-card mr-1'></i>申請信用卡授權";
        }
        #endregion

    }
    #endregion

    #region 課程內頁
    protected void LBtn_Detail_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Edit = (LinkButton)sender;
        string[] Edit_CA = LBtn_Edit.CommandArgument.Split('&');//HCTemplateID, HCourseName, HDateRange

        //session傳值
        Session["CName"] = Edit_CA[1];
        Session["CDateRange"] = Edit_CA[2];
        Session["TypeName"] = Edit_CA[3];
        Session["PMethod"] = Edit_CA[4];

        Response.Redirect("HCourseDetail.aspx?CID=" + Edit_CA[0]);
    }
    #endregion

    #region 報名課程
    protected void LBtn_CourseBooking_Click(object sender, EventArgs e)
    {


        LinkButton gCourseBooking = sender as LinkButton;
        string[] gCourseBooking_CA = gCourseBooking.CommandArgument.Split('&');


        if (string.IsNullOrEmpty(((Label)Master.FindControl("LB_HUserHID")).Text))
        {
            //Response.Write("<script>alert('請登入系統!');window.location.href='HLogin.aspx';</script>");
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "AlertReturn", "alert('請登入系統~!');window.location.href='HLogin.aspx?Url=HIndex.aspx';", true);
            return;
        }


        #region 寫入購物車
        SqlDataReader QuerySelSC = SQLdatabase.ExecuteReader("SELECT a.HID, a.HCTemplateID, a.HCourseName, a.HDateRange, b.HPMethod FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCourseID=b.HID WHERE a.HCTemplateID='" + gCourseBooking_CA[0] + "' AND a.HCourseName='" + gCourseBooking_CA[1] + "' AND a.HDateRange='" + gCourseBooking_CA[2] + "' AND a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' and a.HCourseDonate='0'");
        if (QuerySelSC.Read())
        {
            ////更新資料庫
            //            SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HMerchantTradeNo=@HMerchantTradeNo, HCourseID=@HCourseID, HCourseName=@HCourseName, HAttend=@HAttend, HLodging=@HLodging, HLDate=@HLDate, HCGuide=@HCGuide, HRemark=@HRemark, HRoom=@HRoom, HRoomTime=@HRoomTime, HDCode=@HDCode, HCourseDonate=@HCourseDonate, HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID =@HID and HCourseDonate = '0' and HMemberID=@HMemberID", SQLdatabase.OpenConnection());
            //            QueryUpdSC.Parameters.AddWithValue("@HMerchantTradeNo", "");
            //            QueryUpdSC.Parameters.AddWithValue("@HID", gCourseBooking_CA);
            //            QueryUpdSC.Parameters.AddWithValue("@HCourseDonate", "0");
            //            QueryUpdSC.Parameters.AddWithValue("@HMemberID", Session["UserID"].ToString());
            //            QueryUpdSC.Parameters.AddWithValue("@Status", "1");
            //            QueryUpdSC.Parameters.AddWithValue("@Modify", Session["UserID"].ToString());
            //            QueryUpdSC.Parameters.AddWithValue("@ModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            //            QueryUpdSC.ExecuteNonQuery();
            //            QueryUpdSC.Cancel();
            //            SQLdatabase.OpenConnection().Close();
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('此課程已經報名了喔!');", true);
        }
        else
        {


            //寫入資料庫
            SqlCommand QueryInsSC = new SqlCommand("INSERT INTO HShoppingCart (HOrderNum, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HSelect, HCourseDonate, HStatus, HCreate, HCreateDT) VALUES (@HOrderNum, @HTradeNo, @HMerchantTradeNo, @HMemberID, @HCourseID, @HCTemplateID, @HCourseName, @HDateRange, @HPMethod, @HAttend, @HLodging, @HBDate, @HLDate, @HLCourse, @HLCourseName, @HLDiscount, @HCGuide, @HPayMethod, @HPoint, @HMemberGroup, @HDharmaPass, @HRoom, @HRoomTime, @HSubscribe, @HDCode, @HDPoint, @HPaymentNo, @HExpireDate, @HFailReason, @HPaymentDate, @HPayAmt, @HFinanceRemark, @HInvoiceNo, @HInvoiceDate, @HInvoiceStatus, @HRemark, @HSelect, @HCourseDonate, @HStatus, @HCreate, @HCreateDT)", SQLdatabase.OpenConnection());
            QueryInsSC.Parameters.AddWithValue("@HOrderNum", "");
            QueryInsSC.Parameters.AddWithValue("@HTradeNo", "");
            QueryInsSC.Parameters.AddWithValue("@HMerchantTradeNo", "");
            QueryInsSC.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            QueryInsSC.Parameters.AddWithValue("@HCourseID", "");
            QueryInsSC.Parameters.AddWithValue("@HCTemplateID", gCourseBooking_CA[0]);
            QueryInsSC.Parameters.AddWithValue("@HCourseName", gCourseBooking_CA[1]);
            QueryInsSC.Parameters.AddWithValue("@HDateRange", gCourseBooking_CA[2]);
            QueryInsSC.Parameters.AddWithValue("@HPMethod", gCourseBooking_CA[3]); ;
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



        ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('加入報名清單成功~!');", true);
        Rpt_Course.DataBind();
        //SqlDataReader QuerySelSCCT = SQLdatabase.ExecuteReader("SELECT Count(a.CT) as CT FROM (SELECT Count(PID) as CT, PID FROM ShoppingCart WHERE MemberID='" + Session["UserID"].ToString() + "' group by PID) as a");
        //if (QuerySelSCCT.Read())
        //{
        //    ((Label)Master.FindControl("LB_CartNum")).Text = QuerySelSCCT["CT"].ToString();
        //    //讓master的數量可以一起同步更新
        //    ((UpdatePanel)Master.FindControl("UpdateNumbers")).Update();
        //}
        //QuerySelSCCT.Close();
        #endregion


    }
    #endregion


    #region   OLD上課連結
    #region 學員報名的課程zoom連結顯示
    protected void Rpt_HCourseLink_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        //((HyperLink)e.Item.FindControl("HL_CourseLink")).NavigateUrl = ((Label)e.Item.FindControl("LB_CourseLink")).Text;

        //--另開視窗用，但跑不到Onclick事件
        //  	((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Attributes.Add("href", ((Label)e.Item.FindControl("LB_CourseLink")).Text);
        //((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Attributes.Add("target", "_blank");


        #region 判斷為課程當天才顯示上課連結
        DateTime Today = DateTime.Now;
        string weekday = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(Today.DayOfWeek);



        if (((Label)e.Item.FindControl("LB_DateRange")).Text.IndexOf("-") >= 0)
        {
            #region 判斷當天為平日還是周六或周日
            if (weekday == "星期六")
            {
                if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HSATCourseLink")).Text))
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = true;
                    ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                    ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;
                }
                else
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = true;
                    ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                    ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;

                }
            }
            else if (weekday == "星期日")
            {
                if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HSUNCourseLink")).Text))
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = true;
                    ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;

                }
                else
                {

                    ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = true;
                    ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                    ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;

                }
            }
            else  //平日
            {
                if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_CourseLink")).Text))
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = true;
                    ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                    ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;
                    //Ul_Courselink.Visible = true;
                }
                else
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                    ((Image)e.Item.FindControl("IMG_Zoom")).Visible = false;
                    //Ul_Courselink.Visible = false;
                }

            }
            #endregion

        }
        else if (((Label)e.Item.FindControl("LB_DateRange")).Text.IndexOf(",") >= 0)//非連續課程日期
        {
            //string[] gCourseDate = ((Label)e.Item.FindControl("LB_DateRange")).Text.Split(',');//課程日期
            //DateTime gHCSDate = Convert.ToDateTime(gCourseDate[0]);//課程開始日
            //DateTime gHCEDate = Convert.ToDateTime(gCourseDate[gCourseDate.Length - 1]);//課程結束日

            var check = Array.Exists(((Label)e.Item.FindControl("LB_DateRange")).Text.Split(','), x => x == DateTime.Now.ToString("yyyy/MM/dd"));

            if (check == true)
            {
                #region 判斷當天為平日還是周六或周日


                if (weekday == "星期六")
                {
                    if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HSATCourseLink")).Text))
                    {
                        ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                        ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = true;
                        ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                        ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;

                    }
                    else
                    {
                        ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = true;
                        ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                        ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                        ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;

                    }
                }
                else if (weekday == "星期日")
                {
                    if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HSUNCourseLink")).Text))
                    {
                        ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                        ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                        ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = true;
                        ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;

                    }
                    else
                    {
                        ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = true;
                        ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                        ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                        ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;

                    }
                }
                else  //平日
                {
                    if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_CourseLink")).Text))
                    {
                        ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = true;
                        ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                        ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                        ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;
                    }
                    else
                    {
                        ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                        ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                        ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                        ((Image)e.Item.FindControl("IMG_Zoom")).Visible = false;

                    }

                }
                #endregion
            }
            else
            {
                ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                ((Image)e.Item.FindControl("IMG_Zoom")).Visible = false;
                Ul_Courselink.Visible = false;
            }
        }
        else if (((Label)e.Item.FindControl("LB_DateRange")).Text == DateTime.Now.ToString("yyyy/MM/dd"))//課程日期=今天日期
        {
            #region 判斷當天為平日還是周六或周日


            if (weekday == "星期六")
            {
                if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HSATCourseLink")).Text))
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = true;
                    ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                    ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;

                }
                else
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = true;
                    ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                    ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;

                }
            }
            else if (weekday == "星期日")
            {
                if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HSUNCourseLink")).Text))
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = true;
                    ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;

                }
                else
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = true;
                    ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                    ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;

                }
            }
            else  //平日
            {
                if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_CourseLink")).Text))
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = true;
                    ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                    ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;
                }
                else
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                    ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                    ((Image)e.Item.FindControl("IMG_Zoom")).Visible = false;

                }

            }
            #endregion
        }
        else
        {
            ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
            ((Image)e.Item.FindControl("IMG_Zoom")).Visible = false;
            Ul_Courselink.Visible = false;


        }

        #endregion





    }
    #endregion

    //AE20230824_註解:報名體系專業護持者才顯示護持者連結
    protected void Rpt_Task_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {


        #region 判斷為課程當天才顯示上課連結
        DateTime Today = DateTime.Now;
        string weekday = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(Today.DayOfWeek);

        if (((Label)e.Item.FindControl("LB_DateRange")).Text.IndexOf("-") >= 0)
        {
            ((LinkButton)e.Item.FindControl("LBtn_CourseLinkTask")).Visible = true;
            ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;

        }
        else if (((Label)e.Item.FindControl("LB_DateRange")).Text.IndexOf(",") >= 0)//非連續課程日期
        {

            var check = Array.Exists(((Label)e.Item.FindControl("LB_DateRange")).Text.Split(','), x => x == DateTime.Now.ToString("yyyy/MM/dd"));

            if (check == true)
            {
                ((LinkButton)e.Item.FindControl("LBtn_CourseLinkTask")).Visible = true;
                ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;
            }
            else
            {
                ((LinkButton)e.Item.FindControl("LBtn_CourseLinkTask")).Visible = false;
                ((Image)e.Item.FindControl("IMG_Zoom")).Visible = false;
                Ul_TaskArea.Visible = false;
            }
        }
        else if (((Label)e.Item.FindControl("LB_DateRange")).Text == DateTime.Now.ToString("yyyy/MM/dd"))//課程日期=今天日期
        {

            ((LinkButton)e.Item.FindControl("LBtn_CourseLinkTask")).Visible = true;
            ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;
        }
        else
        {
            ((LinkButton)e.Item.FindControl("LBtn_CourseLinkTask")).Visible = false;
            ((Image)e.Item.FindControl("IMG_Zoom")).Visible = false;
            Ul_TaskArea.Visible = false;

        }

        #endregion


    }

    //AE20230824_註解:報名體系專業護持者(僅音控組)才顯示護持者連結
    protected void Rpt_Replay_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        #region 判斷為課程當天才顯示上課連結
        DateTime Today = DateTime.Now;
        string weekday = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(Today.DayOfWeek);

        if (((Label)e.Item.FindControl("LB_DateRange")).Text.IndexOf("-") >= 0)
        {
            ((LinkButton)e.Item.FindControl("LBtn_CourseLinkRelay")).Visible = true;
            ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;

        }
        else if (((Label)e.Item.FindControl("LB_DateRange")).Text.IndexOf(",") >= 0)//非連續課程日期
        {

            var check = Array.Exists(((Label)e.Item.FindControl("LB_DateRange")).Text.Split(','), x => x == DateTime.Now.ToString("yyyy/MM/dd"));

            if (check == true)
            {
                ((LinkButton)e.Item.FindControl("LBtn_CourseLinkRelay")).Visible = true;
                ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;
            }
            else
            {
                ((LinkButton)e.Item.FindControl("LBtn_CourseLinkRelay")).Visible = false;
                ((Image)e.Item.FindControl("IMG_Zoom")).Visible = false;
                Ul_TaskArea.Visible = false;
            }
        }
        else if (((Label)e.Item.FindControl("LB_DateRange")).Text == DateTime.Now.ToString("yyyy/MM/dd"))//課程日期=今天日期
        {

            ((LinkButton)e.Item.FindControl("LBtn_CourseLinkRelay")).Visible = true;
            ((Image)e.Item.FindControl("IMG_Zoom")).Visible = true;
        }
        else
        {
            ((LinkButton)e.Item.FindControl("LBtn_CourseLinkRelay")).Visible = false;
            ((Image)e.Item.FindControl("IMG_Zoom")).Visible = false;
            Ul_TaskArea.Visible = false;

        }

        #endregion
    }

    protected void LBtn_CourseLink_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_CourseLink = sender as LinkButton;
        //string CourseHID = LBtn_CourseLink.CommandArgument;
        string[] Course_CA = LBtn_CourseLink.CommandArgument.Split(',');
        //string RollcallHID = LBtn_CourseLink.CommandName;


        if (!string.IsNullOrEmpty(Course_CA[1]))
        {
            SqlDataReader RollCallAStatus = SQLdatabase.ExecuteReader("SELECT a.HID AS RollcallHID, a.HCourseID, a.HMemberID, a.HRCDate, a.HAStatus, b.HCourseName, b.HCourseLink, b.HCourseLinkRelay, b.HCourseLinkTask FROM HRollCall as a left join HCourse as b on a.HCourseID = b.HID WHERE a.HAStatus = '0' AND a.HRCDate = CONVERT(nvarchar(20), GETDATE(), 111) AND a.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseID = '" + Course_CA[0] + "'");

            if (RollCallAStatus.Read())
            {
                //進行報到
                SqlCommand cmd = new SqlCommand("UPDATE HRollCall SET HAStatus=@HAStatus, HAStatusDT=@HAStatusDT, HModify=@HModify,HModifyDT=@HModifyDT WHERE HID='" + RollCallAStatus["RollcallHID"].ToString() + "'", con);

                con.Open();
                cmd.Parameters.AddWithValue("@HAStatus", 2);    //0=請選擇、1=實體、2=線上、3=遲到、4=請假
                cmd.Parameters.AddWithValue("@HAStatusDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo));
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo));

                cmd.ExecuteNonQuery();
                cmd.Cancel();
                con.Close();
                con.Dispose();

            }
            RollCallAStatus.Close();

            //開啟Zoom連結
            //Response.Write("<script>window.location.href('" + Course_CA[1] + "');</script>");
            Response.Redirect(Course_CA[1]);
        }
        else
        {
            Response.Write("<script>alert('上課連結尚未設定哦~請通知音控組夥伴進行設定，謝謝~');</script>");
            return;
        }




    }

    #region 慈場線ZOOM連結
    protected void LBtn_CourseLinkRelay_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_CourseLinkRelay = sender as LinkButton;
        string CourseLinkRelay = LBtn_CourseLinkRelay.CommandArgument;

        Response.Redirect(CourseLinkRelay);
    }
    #endregion


    #region 護持者連結
    protected void LBtn_CourseLinkTask_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_CourseLinkTask = sender as LinkButton;
        string CourseLinkTask = LBtn_CourseLinkTask.CommandArgument;

        Response.Redirect(CourseLinkTask);
    }
    #endregion


    #endregion



    #region 快速報到
    //AA20240917_新增快速報到功能(顯示當天課程，可做實體或線上報到)
    #region 實體報到
    protected void Btn_Rollcall_Click(object sender, EventArgs e)
    {
        Button Btn_Rollcall = sender as Button;
        string CourseHID = Btn_Rollcall.CommandArgument;
        string Today = DateTime.Now.ToString("yyyy/MM/dd");

        SqlDataReader RollCallAStatus = SQLdatabase.ExecuteReader("SELECT HAStatus FROM HRollCall WHERE HCourseID = '" + CourseHID + "' AND HRCDate='" + Today + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");


        if (RollCallAStatus.Read())
        {
            if (RollCallAStatus["HAStatus"].ToString() == "0")
            {
                SqlCommand cmd = new SqlCommand("UPDATE HRollCall SET HAStatus=@HAStatus, HAStatusDT=@HAStatusDT, HModify=@HModify,HModifyDT=@HModifyDT WHERE HCourseID = @HCourseID AND HRCDate=@HRCDate AND HMemberID=@HMemberID", con);

                con.Open();
                cmd.Parameters.AddWithValue("@HAStatus", 1);    //0=請選擇、1=實體、2=線上、3=遲到、4=請假
                cmd.Parameters.AddWithValue("@HAStatusDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo));
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo));
                cmd.Parameters.AddWithValue("@HCourseID", CourseHID);
                cmd.Parameters.AddWithValue("@HRCDate", Today);
                cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.ExecuteNonQuery();
                con.Close();
                cmd.Cancel();

                Response.Write("<script>alert('完成實體報到囉~!');window.location.href='Hindex.aspx'</script>");

            }
            else if (RollCallAStatus["HAStatus"].ToString() == "1" || RollCallAStatus["HAStatus"].ToString() == "2" || RollCallAStatus["HAStatus"].ToString() == "3")
            {
                Response.Write("<script>alert('您之前已完成報到囉~');</script>");
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_Rollcall').modal();"), true);
                //return;
            }
            else
            {
                Response.Write("<script>alert('您之前已請假~');</script>");
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_Rollcall').modal();"), true);
                //return;
            }
        }
        else
        {
            Response.Write("<script>alert('點名單可能尚未產生或查無您報名的資訊哦~');</script>");
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_Rollcall').modal();"), true);
            //return;
        }
        RollCallAStatus.Close();
    }
    #endregion

    protected void Rpt_HRollcall_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        HtmlTableRow Tr_Course = ((HtmlTableRow)e.Item.FindControl("Tr_Course"));


        #region 判斷為課程當天才顯示課程資訊
        DateTime Today = DateTime.Now;
        string weekday = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(Today.DayOfWeek);


        #region 上課連結
        //如果參班身分是護持者，則不會顯示任何上課連結
        //if (((Label)e.Item.FindControl("LB_HAttend")).Text == "1" || ((Label)e.Item.FindControl("LB_HAttend")).Text == "6")
        if (!string.IsNullOrEmpty(gDRV["HCourseLInk"].ToString()) && !string.IsNullOrEmpty(gDRV["HSATCourseLink"].ToString()) && !string.IsNullOrEmpty(gDRV["HSUNCourseLink"].ToString()))
        {
            if (gDRV["HAttend"].ToString() == "1" || gDRV["HAttend"].ToString() == "6")
            {
                if (gDRV["HDateRange"].ToString().IndexOf("-") >= 0)
                {
                    #region 判斷當天為平日還是周六或周日
                    if (weekday == "星期六")
                    {
                        if (!string.IsNullOrEmpty(gDRV["HSATCourseLink"].ToString()))
                        {
                            ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                            ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = true;
                            ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                        }

                    }
                    else if (weekday == "星期日")
                    {
                        if (!string.IsNullOrEmpty(gDRV["HSUNCourseLink"].ToString()))
                        {
                            ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                            ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                            ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = true;
                        }

                    }
                    else  //平日
                    {
                        if (!string.IsNullOrEmpty(gDRV["HCourseLink"].ToString()))
                        {
                            ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = true;
                            ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                            ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                        }
                        else
                        {
                            ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                            ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                            ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                        }

                    }
                    #endregion
                }
                else if (gDRV["HDateRange"].ToString().IndexOf(",") >= 0)//非連續課程日期
                {
                    var check = Array.Exists(gDRV["HDateRange"].ToString().Split(','), x => x == DateTime.Now.ToString("yyyy/MM/dd"));

                    if (check == true)
                    {
                        #region 判斷當天為平日還是周六或周日
                        if (weekday == "星期六")
                        {
                            if (!string.IsNullOrEmpty(gDRV["HSATCourseLink"].ToString()))
                            {
                                ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                                ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = true;
                                ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                            }

                        }
                        else if (weekday == "星期日")
                        {
                            if (!string.IsNullOrEmpty(gDRV["HSUNCourseLink"].ToString()))
                            {
                                ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                                ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                                ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = true;
                            }

                        }
                        else  //平日
                        {
                            if (!string.IsNullOrEmpty(gDRV["HCourseLink"].ToString()))
                            {
                                ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = true;
                                ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                                ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                            }
                            else
                            {
                                ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                                ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                                ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                            }

                        }
                        #endregion
                    }

                }
                else if (gDRV["HDateRange"].ToString() == DateTime.Now.ToString("yyyy/MM/dd"))//課程日期=今天日期
                {
                    #region 判斷當天為平日還是周六或周日
                    if (weekday == "星期六")
                    {
                        if (!string.IsNullOrEmpty(gDRV["HSATCourseLink"].ToString()))
                        {
                            ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                            ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = true;
                            ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                        }

                    }
                    else if (weekday == "星期日")
                    {
                        if (!string.IsNullOrEmpty(gDRV["HSUNCourseLink"].ToString()))
                        {
                            ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                            ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                            ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = true;
                        }

                    }
                    else  //平日
                    {
                        if (!string.IsNullOrEmpty(gDRV["HCourseLink"].ToString()))
                        {
                            ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = true;
                            ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                            ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                        }
                        else
                        {
                            ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Visible = false;
                            ((LinkButton)e.Item.FindControl("LBtn_SatCourseLink")).Visible = false;
                            ((LinkButton)e.Item.FindControl("LBtn_SunCourseLink")).Visible = false;
                        }
                    }
                    #endregion
                }


            }
        }
        else
        {
            ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Enabled = false;
            ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).Text = "無";
            ((LinkButton)e.Item.FindControl("LBtn_CourseLink")).CssClass = "";
        }

        #endregion


        #region 護持者連結
        //if ((gDRV["HAttend"].ToString() == "5" || gDRV["HAttend"].ToString() == "6") && (gDRV["HGroupID"].ToString() == null || gDRV["HGroupID"].ToString() != "45" || gDRV["HGroupID"].ToString() != "15" || gDRV["HGroupID"].ToString() != "42"))

        //if ((((Label)e.Item.FindControl("LB_HAttend")).Text == "5" || ((Label)e.Item.FindControl("LB_HAttend")).Text == "6"))
        if (gDRV["HAttend"].ToString() == "5" || gDRV["HAttend"].ToString() == "6")
        {
            SqlDataReader QueryHTaskLink = SQLdatabase.ExecuteReader("SELECT A.HDateRange, A.HCourseLinkTask FROM OrderList_Detail AS A LEFT JOIN HCourseBooking_Group AS D ON A.HID=D.HBookingID WHERE  (A.HCourseLinkTask <> null OR A.HCourseLinkTask <> '') AND  A.HStatus=1  AND A.HItemStatus = 1 AND A.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND  (((A.HAttend='5'  OR A.HAttend = '6') AND (D.HGroupID NOT IN(45,15,42) OR D.HGroupID IS NULL )) OR ((A.HAttend = '1'  OR A.HAttend = '6')  AND D.HGroupID NOT IN(45,15,42) AND D.HGroupID IS NOT NULL))  AND HCourseID='" + ((Label)e.Item.FindControl("LB_HCourseID")).Text + "' GROUP BY  A.HDateRange, A.HCourseLinkTask ");
            if (QueryHTaskLink.Read())
            {
                if (QueryHTaskLink["HDateRange"].ToString().IndexOf("-") >= 0)
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLinkTask")).Visible = true;
                }
                else if (QueryHTaskLink["HDateRange"].ToString().IndexOf(",") >= 0)//非連續課程日期
                {

                    var check = Array.Exists(QueryHTaskLink["HDateRange"].ToString().Split(','), x => x == DateTime.Now.ToString("yyyy/MM/dd"));

                    if (check == true)
                    {
                        ((LinkButton)e.Item.FindControl("LBtn_CourseLinkTask")).Visible = true;
                    }
                    else
                    {
                        ((LinkButton)e.Item.FindControl("LBtn_CourseLinkTask")).Visible = false;
                    }
                }
                else if (QueryHTaskLink["HDateRange"].ToString() == DateTime.Now.ToString("yyyy/MM/dd"))//課程日期=今天日期
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLinkTask")).Visible = true;
                }
                else
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLinkTask")).Visible = false;
                }


            }
            QueryHTaskLink.Close();




        }


        #endregion


        #region 慈場線連結
        //if ((gDRV["HAttend"].ToString() == "5" || gDRV["HAttend"].ToString() == "6") && (gDRV["HGroupID"].ToString() != null || gDRV["HGroupID"].ToString() == "45" || gDRV["HGroupID"].ToString() == "15" || gDRV["HGroupID"].ToString() == "42") && gDRV["HGDay"].ToString() == Today.ToString("yyyy/MM/dd"))

        //if ((((Label)e.Item.FindControl("LB_HAttend")).Text == "5" || ((Label)e.Item.FindControl("LB_HAttend")).Text == "6"))
        if (gDRV["HAttend"].ToString() == "5" || gDRV["HAttend"].ToString() == "6")
        {
            SqlDataReader QueryHRelayLink = SQLdatabase.ExecuteReader("SELECT A.HDateRange, A.HCourseLinkRelay FROM OrderList_Merge AS A LEFT JOIN HCourseBooking_Group AS D ON A.HID=D.HBookingID WHERE (A.HCourseLinkRelay <> null OR A.HCourseLinkRelay <> '') AND A.HStatus=1  AND A.HItemStatus = 1 AND A.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'   AND (D.HGroupID ='45' OR D.HGroupID ='15' OR D.HGroupID ='42' )   AND D.HGDay='" + Today.ToString("yyyy/MM/dd") + "' AND HCourseID='" + gDRV["HCourseID"].ToString() + "'  GROUP BY A.HDateRange, A.HCourseLinkRelay");
            if (QueryHRelayLink.Read())
            {
                if (QueryHRelayLink["HDateRange"].ToString().IndexOf("-") >= 0)
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLinkRelay")).Visible = true;
                }
                else if (QueryHRelayLink["HDateRange"].ToString().IndexOf(",") >= 0)//非連續課程日期
                {

                    var check = Array.Exists(QueryHRelayLink["HDateRange"].ToString().Split(','), x => x == DateTime.Now.ToString("yyyy/MM/dd"));

                    if (check == true)
                    {
                        ((LinkButton)e.Item.FindControl("LBtn_CourseLinkRelay")).Visible = true;
                    }
                    else
                    {
                        ((LinkButton)e.Item.FindControl("LBtn_CourseLinkRelay")).Visible = false;
                    }
                }
                else if (QueryHRelayLink["HDateRange"].ToString() == DateTime.Now.ToString("yyyy/MM/dd"))//課程日期=今天日期
                {

                    ((LinkButton)e.Item.FindControl("LBtn_CourseLinkRelay")).Visible = true;
                }
                else
                {
                    ((LinkButton)e.Item.FindControl("LBtn_CourseLinkRelay")).Visible = false;

                }
            }
            QueryHRelayLink.Close();
        }
        #endregion


        #region 隱藏非當天的課程
        if (gDRV["HDateRange"].ToString().IndexOf(",") >= 0)//非連續課程日期
        {
            var check = Array.Exists(gDRV["HDateRange"].ToString().Split(','), x => x == DateTime.Now.ToString("yyyy/MM/dd"));
            if (check == false)
            {
                Tr_Course.Visible = false;
            }
        }
        else if (gDRV["HDateRange"].ToString() != DateTime.Now.ToString("yyyy/MM/dd") && gDRV["HDateRange"].ToString().IndexOf("-") < 0)
        {
            Tr_Course.Visible = false;
        }
        #endregion

        #endregion


        #region  判斷是否已有產生點名單
        //AA20250212_新增判斷 用非同步的方式執行
        //AE20250621_若無法排除非課程類的則先不產生
        #endregion

        #region 判斷是否已報到
        SqlDataReader QueryRollcall = SQLdatabase.ExecuteReader("SELECT HAStatus FROM OrderList_Detail AS A LEFT JOIN HRollcall AS b ON A.HCourseID=b.HCourseID AND B.HMemberID=A.HMemberID  WHERE   A.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND A.HCourseID='" + ((Label)e.Item.FindControl("LB_HCourseID")).Text + "' AND b.HRCDate = '" + Today.ToString("yyyy/MM/dd") + "'");
        if (QueryRollcall.Read())
        {
            if (QueryRollcall["HAStatus"].ToString() == "0")
            {
                //((Label)e.Item.FindControl("LB_HAStatus")).Text = "尚未點名";
                //((Label)e.Item.FindControl("LB_HAStatus")).CssClass = "text-danger";

                ((Button)e.Item.FindControl("Btn_Rollcall")).Enabled = true;
                ((Button)e.Item.FindControl("Btn_Rollcall")).CssClass = "btn btn-success p-2";
            }
            else if (QueryRollcall["HAStatus"].ToString() == "1")
            {
                //((Label)e.Item.FindControl("LB_HAStatus")).Text = "實體已報到";
                //((Label)e.Item.FindControl("LB_HAStatus")).CssClass = "text-success";

                ((Button)e.Item.FindControl("Btn_Rollcall")).Enabled = false;
                ((Button)e.Item.FindControl("Btn_Rollcall")).CssClass = "btn btn-gray p-2";
                //((Button)e.Item.FindControl("Btn_Rollcall")).Text = "已報到";
            }
            else if (QueryRollcall["HAStatus"].ToString() == "2")
            {
                //((Label)e.Item.FindControl("LB_HAStatus")).Text = "線上已報到";
                //((Label)e.Item.FindControl("LB_HAStatus")).CssClass = "text-success";

                ((Button)e.Item.FindControl("Btn_Rollcall")).Enabled = false;
                ((Button)e.Item.FindControl("Btn_Rollcall")).CssClass = "btn btn-gray p-2";
            }
            else if (QueryRollcall["HAStatus"].ToString() == "3")
            {
                //((Label)e.Item.FindControl("LB_HAStatus")).Text = "遲到";

                ((Button)e.Item.FindControl("Btn_Rollcall")).Enabled = false;
                ((Button)e.Item.FindControl("Btn_Rollcall")).CssClass = "text-gray p-2";
                ((Button)e.Item.FindControl("Btn_Rollcall")).Text = "遲到";
            }
            else if (QueryRollcall["HAStatus"].ToString() == "4")
            {
                //((Label)e.Item.FindControl("LB_HAStatus")).Text = "請假";

                ((Button)e.Item.FindControl("Btn_Rollcall")).Enabled = false;
                ((Button)e.Item.FindControl("Btn_Rollcall")).CssClass = "text-gray p-2";
                ((Button)e.Item.FindControl("Btn_Rollcall")).Text = "已請假";
            }
            else if (QueryRollcall["HAStatus"].ToString() == "5")
            {
                ((Label)e.Item.FindControl("LB_HAStatus")).Text = "請假審核中";

                ((Button)e.Item.FindControl("Btn_Rollcall")).Enabled = false;
                ((Button)e.Item.FindControl("Btn_Rollcall")).CssClass = "text-gray p-2";
                ((Button)e.Item.FindControl("Btn_Rollcall")).Text = "請假審核中";
            }

        }
        else
        {
            ((Label)e.Item.FindControl("LB_HAStatus")).Text = "尚未產生點名單";
            ////((Button)e.Item.FindControl("Btn_Rollcall")).Enabled = false;
        }
        QueryRollcall.Close();
        #endregion

    }

    #region 線上課程連結
    protected void LBtn_MCourseLink_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_CourseLink = sender as LinkButton;
        //string CourseHID = LBtn_CourseLink.CommandArgument;
        string[] Course_CA = LBtn_CourseLink.CommandArgument.Split(',');
        //string RollcallHID = LBtn_CourseLink.CommandName;


        if (!string.IsNullOrEmpty(Course_CA[1]))
        {
            SqlDataReader RollCallAStatus = SQLdatabase.ExecuteReader("SELECT a.HID AS RollcallHID, a.HCourseID, a.HMemberID, a.HRCDate, a.HAStatus, b.HCourseName, b.HCourseLink, b.HCourseLinkRelay, b.HCourseLinkTask FROM HRollCall as a left join HCourse as b on a.HCourseID = b.HID WHERE a.HAStatus = '0' AND a.HRCDate = CONVERT(nvarchar(20), GETDATE(), 111) AND a.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HCourseID = '" + Course_CA[0] + "'");

            if (RollCallAStatus.Read())
            {
                //進行報到
                SqlCommand cmd = new SqlCommand("UPDATE HRollCall SET HAStatus=@HAStatus, HAStatusDT=@HAStatusDT, HModify=@HModify,HModifyDT=@HModifyDT WHERE HID='" + RollCallAStatus["RollcallHID"].ToString() + "'", con);

                con.Open();
                cmd.Parameters.AddWithValue("@HAStatus", 2);    //0=請選擇、1=實體、2=線上、3=遲到、4=請假
                cmd.Parameters.AddWithValue("@HAStatusDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo));
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo));

                cmd.ExecuteNonQuery();
                cmd.Cancel();
                con.Close();
                con.Dispose();

            }
            RollCallAStatus.Close();

            //開啟Zoom連結
            //Response.Write("<script>window.location.href('" + Course_CA[1] + "');</script>");
            Response.Redirect(Course_CA[1]);
        }
        else
        {
            Response.Write("<script>alert('上課連結尚未設定哦~請通知音控組夥伴進行設定，謝謝~');</script>");
            return;
        }

    }
    #endregion


    #region 慈場線ZOOM連結
    protected void LBtn_MCourseLinkRelay_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_CourseLinkRelay = sender as LinkButton;
        string CourseLinkRelay = LBtn_CourseLinkRelay.CommandArgument;

        Response.Redirect(CourseLinkRelay);
    }
    #endregion


    #region 護持者連結
    protected void LBtn_MCourseLinkTask_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_CourseLinkTask = sender as LinkButton;
        string CourseLinkTask = LBtn_CourseLinkTask.CommandArgument;

        Response.Redirect(CourseLinkTask);
    }
    #endregion

    #endregion



    #region 課程捐款
    protected void LBtn_CourseDonate_Click(object sender, EventArgs e)
    {
        LinkButton gCourseDonate = sender as LinkButton;
        string[] gCourseDonate_CA = gCourseDonate.CommandArgument.Split('&');
        string gCourseDonate_CN = gCourseDonate.CommandName;

        if (string.IsNullOrEmpty(((Label)Master.FindControl("LB_HUserHID")).Text))
        {
            Response.Write("<script>alert('請登入系統!');window.location.href='HLogin.aspx';</script>");
            return;
        }

        #region 七彩光橋捐款改導向信用卡授權申請
        if (gCourseDonate_CA[1] == "2023.08.19～08.25七彩光橋法系列_孝親報恩光橋血脈轉輪班會" || gCourseDonate_CA[1] == "2024七彩光橋法系列_孝親報恩光橋血脈轉輪班會" || (!string.IsNullOrEmpty(gCourseDonate_CN) && gCourseDonate_CN !="0"))
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "confirm", ("NoticeCCPeriod(" + gCourseDonate_CN + ");"), true);
            //ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('此課程僅開放使用信用卡授權付款方式，系統將為您導向信用卡授權申請頁面~謝謝!');window.location.href='HMember_CCPeriod.aspx';", true);
        }
        else
        {
            #region 寫入捐款
            SqlDataReader QuerySelSC = SQLdatabase.ExecuteReader("SELECT a.HID, b.HPMethod, a.HCTemplateID, a.HCourseName, a.HDateRange FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCourseID=b.HID WHERE a.HCTemplateID='" + gCourseDonate_CA[0] + "' AND a.HCourseName='" + gCourseDonate_CA[1] + "' AND a.HDateRange='" + gCourseDonate_CA[2] + "' AND a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' and a.HCourseDonate='1'");
        if (QuerySelSC.Read())
        {
            ////更新資料庫
            //            SqlCommand QueryUpdSC = new SqlCommand("UPDATE HShoppingCart SET HMerchantTradeNo=@HMerchantTradeNo, HCourseID=@HCourseID, HCourseName=@HCourseName, HAttend=@HAttend, HLodging=@HLodging, HLDate=@HLDate, HCGuide=@HCGuide, HRemark=@HRemark, HRoom=@HRoom, HRoomTime=@HRoomTime, HDCode=@HDCode, HCourseDonate=@HCourseDonate, HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID =@HID and HCourseDonate = '0' and HMemberID=@HMemberID", SQLdatabase.OpenConnection());
            //            QueryUpdSC.Parameters.AddWithValue("@HMerchantTradeNo", "");
            //            QueryUpdSC.Parameters.AddWithValue("@HID", gCourseDonate);
            //            QueryUpdSC.Parameters.AddWithValue("@HCourseDonate", "0");
            //            QueryUpdSC.Parameters.AddWithValue("@HMemberID", Session["UserID"].ToString());
            //            QueryUpdSC.Parameters.AddWithValue("@Status", "1");
            //            QueryUpdSC.Parameters.AddWithValue("@Modify", Session["UserID"].ToString());
            //            QueryUpdSC.Parameters.AddWithValue("@ModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            //            QueryUpdSC.ExecuteNonQuery();
            //            QueryUpdSC.Cancel();
            //            SQLdatabase.OpenConnection().Close();
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('此課程已經有捐款了喔!');", true);
        }
        else
        {


            //寫入資料庫
            SqlCommand QueryInsSC = new SqlCommand("INSERT INTO HShoppingCart (HOrderNum, HTradeNo, HMerchantTradeNo, HMemberID, HCourseID, HCTemplateID, HCourseName, HDateRange, HPMethod, HAttend, HLodging, HBDate, HLDate, HLCourse, HLCourseName, HLDiscount, HCGuide, HPayMethod, HPoint, HMemberGroup, HDharmaPass, HRoom, HRoomTime, HSubscribe, HDCode, HDPoint, HPaymentNo, HExpireDate, HFailReason, HPaymentDate, HPayAmt, HFinanceRemark, HInvoiceNo, HInvoiceDate, HInvoiceStatus, HRemark, HSelect, HCourseDonate, HStatus, HCreate, HCreateDT) VALUES (@HOrderNum, @HTradeNo, @HMerchantTradeNo, @HMemberID, @HCourseID, @HCTemplateID, @HCourseName, @HDateRange, @HPMethod, @HAttend, @HLodging, @HBDate, @HLDate, @HLCourse, @HLCourseName, @HLDiscount, @HCGuide, @HPayMethod, @HPoint, @HMemberGroup, @HDharmaPass, @HRoom, @HRoomTime, @HSubscribe, @HDCode, @HDPoint, @HPaymentNo, @HExpireDate, @HFailReason, @HPaymentDate, @HPayAmt, @HFinanceRemark, @HInvoiceNo, @HInvoiceDate, @HInvoiceStatus, @HRemark, @HSelect, @HCourseDonate, @HStatus, @HCreate, @HCreateDT)", SQLdatabase.OpenConnection());
            QueryInsSC.Parameters.AddWithValue("@HOrderNum", "");
            QueryInsSC.Parameters.AddWithValue("@HTradeNo", "");
            QueryInsSC.Parameters.AddWithValue("@HMerchantTradeNo", "");
            QueryInsSC.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            QueryInsSC.Parameters.AddWithValue("@HCourseID", "");
            QueryInsSC.Parameters.AddWithValue("@HCTemplateID", gCourseDonate_CA[0]);
            QueryInsSC.Parameters.AddWithValue("@HCourseName", gCourseDonate_CA[1]);
            QueryInsSC.Parameters.AddWithValue("@HDateRange", gCourseDonate_CA[2]);
            QueryInsSC.Parameters.AddWithValue("@HPMethod", gCourseDonate_CA[3]); ;
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
            QueryInsSC.Parameters.AddWithValue("@HCourseDonate", "1");
            QueryInsSC.Parameters.AddWithValue("@HStatus", "1");
            QueryInsSC.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            QueryInsSC.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            QueryInsSC.ExecuteNonQuery();
            QueryInsSC.Cancel();
            SQLdatabase.OpenConnection().Close();

        }
        QuerySelSC.Close();

        #region 報名/捐款清單重整
        ((SqlDataSource)Master.FindControl("SDS_ShoppingCart")).SelectCommand = "SELECT a.HID AS ShoppingCartID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, IIF(ISNUMERIC(b.HBCPoint )=1,FORMAT(IIF(b.HBCPoint  IS NULL,0,b.HBCPoint ),'N0'),b.HBCPoint ) AS HBCPoint,a.HCourseDonate  FROM HShoppingCart AS a LEFT JOIN HCourse AS b ON a.HCTemplateID=b.HCTemplateID AND a.HCourseName=b.HCourseName AND a.HDateRange=b.HDateRange WHERE a.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND b.HStatus='1' AND b.HVerifyStatus='2'  GROUP BY a.HID, a.HCourseID, a.HCTemplateID, a.HCourseName, a.HDateRange, a.HSelect, b.HCourseName, b.HBCPoint,a.HCourseDonate";
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


        ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('加入捐款清單成功~!');", true);
        Rpt_Course.DataBind();

            //SqlDataReader QuerySelSCCT = SQLdatabase.ExecuteReader("SELECT Count(a.CT) as CT FROM (SELECT Count(PID) as CT, PID FROM ShoppingCart WHERE MemberID='" + Session["UserID"].ToString() + "' group by PID) as a");
            //if (QuerySelSCCT.Read())
            //{
            //    ((Label)Master.FindControl("LB_CartNum")).Text = QuerySelSCCT["CT"].ToString();
            //    //讓master的數量可以一起同步更新
            //    ((UpdatePanel)Master.FindControl("UpdateNumbers")).Update();
            //}
            //QuerySelSCCT.Close();
            #endregion
        }
        #endregion
    }
    #endregion





    #region 請假
    protected void Btn_DayOff_Click(object sender, EventArgs e)
    {
        Button Btn_DayOff = (Button)sender;
        string HDetail_CA = Btn_DayOff.CommandArgument;  //HCourseID

        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as Button).NamingContainer as RepeaterItem;

        SqlDataReader QueryHSC = SQLdatabase.ExecuteReader("SELECT a.HID AS BookingID, a.HOrderGroup, a.HCourseID, a.HMemberID, b.HType, a.HFrom FROM OrderList_Merge AS a INNER JOIN HCourse AS b ON a.HCourseID = b.HID WHERE HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'AND a.HCourseID = '" + HDetail_CA + "' AND B.HType != '13' AND A.HStatus = '1' AND A.HItemStatus = '1'  GROUP BY a.HID, a.HOrderGroup, a.HCourseID, a.HMemberID, b.HType, a.HFrom");
        if (QueryHSC.Read())
        {
            Context.Items["HFrom"] = QueryHSC["HFrom"].ToString();
            Context.Items["BookingID"] = QueryHSC["BookingID"].ToString();
            Context.Items["HType"] = QueryHSC["HType"].ToString();
            //Session["rollcalldate"] = DateTime.Now.ToString("yyyyMMdd");
            Server.Transfer("HMember_CourseDetail.aspx");
            //Server.Transfer("HMember_CourseDetail.aspx?DT=" + Session["rollcalldate"].ToString());

        }
        else
        {
            Response.Write("<script>alert('您尚未報名此課程哦~!');</script>");
            return;
        }
        QueryHSC.Close();





    }
    #endregion


    #region 快速報到
    protected void LBtn_FastRollcall_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(((Label)Master.FindControl("LB_HUserHID")).Text))
        {
            Response.Write("<script>alert('您尚未完成登入哦~!請先登入，謝謝~');window.location.href='HLogin.aspx?R=1';</script>");
            return;
        }
        else
        {
            //SDS_HRollcall.SelectCommand = "SELECT A.HMemberID, A.HAttend,A.HCourseID, A.HCourseName , A.HPlaceName , A.HDateRange, A.HCourseLink, A.HSATCourseLink, A.HSUNCourseLink,A.HCourseLinkTask, A.HCourseLinkRelay , A.HShowZoom FROM OrderList_Detail AS A WHERE  A.HStatus = 1  AND A.HItemStatus = 1  AND A.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'  AND A.HCourseID <> '2045'  AND A.HCourseID <> '332'  AND DateDiff(Day,left(A.HDateRange, 10),getdate()) >= 0 AND DateDiff(Day,RIGHT(A.HDateRange, 10),getdate()) <= 0 GROUP BY A.HMemberID, A.HAttend,A.HCourseID, A.HCourseName , A.HPlaceName , A.HDateRange, A.HCourseLink, A.HSATCourseLink, A.HSUNCourseLink,A.HCourseLinkTask, A.HCourseLinkRelay , A.HShowZoom ";

            //AE20250314_改寫法
            SDS_HRollcall.SelectCommand = "SELECT DISTINCT A.HMemberID, A.HAttend, A.HCourseID, A.HCourseName, A.HPlaceName, A.HDate AS HDateRange, A.HCourseLink, A.HSATCourseLink, A.HSUNCourseLink, A.HCourseLinkTask, A.HCourseLinkRelay, A.HShowZoom FROM OrderList_Detail AS A WHERE A.HStatus = 1  AND A.HItemStatus = 1   AND A.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'  AND A.HCourseID NOT IN('2045', '332')  AND (GETDATE() BETWEEN CAST(LEFT(A.HDateRange, 10) AS DATE) AND CAST(RIGHT(A.HDateRange, 10) AS DATE) OR CHARINDEX(CONVERT(VARCHAR(10), GETDATE(), 111), A.HDateRange) > 0 ) ";

            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_Rollcall').modal();window.location.replace='HIndex.aspx'"), true);
        }

    }
    #endregion

    #region EIP訊息公告
    //AA20250308_新增串接EIP的訊息公告資訊
    protected void Rpt_HEIPBroadcast_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_MDate")).Text))
        {
            DateTime date = DateTime.ParseExact(((Label)e.Item.FindControl("LB_MDate")).Text, "yyyyMMddHHmmss", null);
            string formattedDate = date.ToString("yyyy/MM/dd");
            ((Label)e.Item.FindControl("LB_MDate")).Text = formattedDate;
        }

              //了解更多
              ((LinkButton)e.Item.FindControl("LBtn_Detail")).PostBackUrl = "~/HNews_Detail.aspx?HID=" + ((Label)e.Item.FindControl("LB_HID")).Text;
    }
    #endregion

    #region 七天班會快速報名
    protected void LBtn_QuickPay_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(((Label)Master.FindControl("LB_HUserHID")).Text))
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", ("alert('請先登入系統~!');window.location.href='HLogin.aspx?Url=HBBDateCourseList.aspx';"), true);
            //Response.Write("<script>alert('請登入系統!');window.location.href='HLogin.aspx';</script>");
            return;
        }

        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;


        Session.Add("HCourseName", ((Label)RI.FindControl("LB_HCourseName")).Text);
        Session.Add("HDateRange", ((Label)RI.FindControl("LB_HDateRange")).Text);
        Session.Add("HCBatchNum", ((Label)RI.FindControl("LB_HCBatchNum")).Text);
        Session.Add("HBCPoint", ((Label)RI.FindControl("LB_HBCPoint")).Text.Replace(",", ""));
        //AA20231108_加入繳費帳戶的傳值(1:基金會，2:文化事業)
        Session.Add("HPMethod", ((Label)RI.FindControl("LB_HPMethod")).Text);
        Session.Add("BookedDates", ((Label)RI.FindControl("LB_BookedDates")).Text);
        Response.Redirect("HBBDateCQuickPay.aspx");
    }
    #endregion


    #region 尚未完成付款按鈕
    protected void LBtn_Pay_Click(object sender, EventArgs e)
    {
        LinkButton gLBtn_Pay = sender as LinkButton;

        Session["OrderGroup"] = gLBtn_Pay.CommandArgument;

        ScriptManager.RegisterStartupScript(this, Page.GetType(), "confirm", ("Notice();"), true);
        //Response.Write("<script>alert('此課程已存在未付款訂單中，系統將為您導向訂單紀錄頁面，再請您進行結帳流程，謝謝~');window.location.href='HMember_Order.aspx';</script>");
    }
    #endregion
}