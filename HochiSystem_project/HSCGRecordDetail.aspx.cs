using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Presentation;
using NPOI.SS.Formula.Functions;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Image = System.Web.UI.WebControls.Image;
using Path = System.IO.Path;

public partial class HSCGRecordDetail : System.Web.UI.Page
{
    SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    #region --根目錄--
    string SCFile = "~/uploads/SpecialColumn/";

    string File1Root = "D:\\Website\\System\\HochiSystem\\uploads\\HSCTMsg\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";


    #endregion

    #region 回應成長紀錄&回應留言SQL SelectCommand String
    string gHSCGRMsgCon = "SELECT a.HID, a.HMemberID, a.HContent, a.HFile1, a.HVideoLink, a.HStatus, a.HCreate, a.HCreateDT, (ISNull(b.HSystemName,'')+' '+ISNull(b.HArea,'')+' '+b.HPeriod+' '+b.HUserName) AS HUserName, b.HIMG FROM HSCGRMsg AS a LEFT JOIN MemberList AS b ON a.HMemberID=b.HID";

    string gHSCGRMsgResponseCon = "SELECT a.HID, a.HSCGRMsgID, a.HMemberID, a.HGRMRContent, a.HStatus, a.HCreate,  a.HCreateDT, (ISNull(b.HSystemName,'')+' '+ISNull(b.HArea,'')+' '+b.HPeriod+' '+b.HUserName) AS HUserName, b.HIMG, c.HCreateDT AS SCGRDate FROM HSCGRMsgResponse AS a  LEFT JOIN MemberList AS b ON a.HMemberID=b.HID  LEFT JOIN HSCGRMsg AS c ON a.HSCGRMsgID=c.HID";
    #endregion


    string gCreateDate = null;

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        #region 數量計算

        int MsgSum = 0;
        MsgSum += Rpt_HSCGRMsg.Items.Count; //主題-留言數量計算(回應主題)

        for (int x = 0; x < Rpt_HSCGRMsg.Items.Count; x++)
        {
            //主題-留言數量計算(回應留言)
            Repeater Rpt_HSCGRMsgResponse = Rpt_HSCGRMsg.Items[x].FindControl("Rpt_HSCGRMsgResponse") as Repeater;  //找到Repeater物件
            MsgSum += Rpt_HSCGRMsgResponse.Items.Count;

            //主題-心情數量計算
            int TopicThumbUpSum = 0, TopicHeartSum = 0, TopicSmile = 0;
            SqlDataReader QuerySCGRecordMood = SQLdatabase.ExecuteReader("SELECT HID, HSCGRecordID, HMemberID, HType FROM HSCGRecord_Mood WHERE HSCGRecordID='" + LB_TID.Text + "'");

            while (QuerySCGRecordMood.Read())
            {
                if (QuerySCGRecordMood["HType"].ToString() == "1")
                {
                    TopicThumbUpSum++;
                }
                else if (QuerySCGRecordMood["HType"].ToString() == "2")
                {
                    TopicHeartSum++;
                }
                else if (QuerySCGRecordMood["HType"].ToString() == "3")
                {
                    TopicSmile++;
                }
            }
            QuerySCGRecordMood.Close();

            LB_TopicThumbUpSum.Text = TopicThumbUpSum.ToString("N0");
            LB_TopicHeartSum.Text = TopicHeartSum.ToString("N0");
            LB_TopicSmileSum.Text = TopicSmile.ToString("N0");

            //回應成長紀錄-心情數量計算
            int MsgThumbUpSum = 0, MsgHeartSum = 0, MsgSmile = 0;
            SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT a.HID, a.HSCGRMsgID, a.HMemberID, a.HType FROM HSCGRMsg_Mood AS a WHERE  a.HSCGRMsgID='" + ((Label)Rpt_HSCGRMsg.Items[x].FindControl("LB_HSCGRMsgID")).Text + "'");

            while (dr.Read())
            {
                if (dr["HType"].ToString() == "1")
                {
                    MsgThumbUpSum++;
                }
                else if (dr["HType"].ToString() == "2")
                {
                    MsgHeartSum++;
                }
                else if (dr["HType"].ToString() == "3")
                {
                    MsgSmile++;
                }
            }
            dr.Close();

            ((Label)Rpt_HSCGRMsg.Items[x].FindControl("LB_MsgThumbUpSum")).Text = MsgThumbUpSum.ToString("N0");
            ((Label)Rpt_HSCGRMsg.Items[x].FindControl("LB_MsgHeartSum")).Text = MsgHeartSum.ToString("N0");
            ((Label)Rpt_HSCGRMsg.Items[x].FindControl("LB_MsgSmileSum")).Text = MsgSmile.ToString("N0");
        }

        LB_MsgSum.Text = MsgSum.ToString("N0"); //主題-留言數量計算(回應主題)

        #endregion
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["TID"] != null)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["TID"].ToString()))
            {
                LB_TID.Text = Request.QueryString["TID"].ToString();

                SqlDataReader QueryHSCGRecord = SQLdatabase.ExecuteReader("SELECT HID, HSCForumClassID, HTopicName, HCreate, HCreateDT FROM HSCGRecord WHERE HID='" + LB_TID.Text + "'");

                if (QueryHSCGRecord.Read())
                {
                    LTR_HTopicName.Text = QueryHSCGRecord["HTopicName"].ToString();
                    LB_HDate.Text = QueryHSCGRecord["HCreateDT"].ToString();
                    gCreateDate = (Convert.ToDateTime(QueryHSCGRecord["HCreateDT"].ToString())).ToString("yyyy/MM/dd");


                    IMG_Creator.ImageUrl = "~/images/icon.png";
                    LB_HMember.Text = "系統自動產生";
                }
                QueryHSCGRecord.Close();

            }
            else
            {
                Response.Redirect("HSCIndex.aspx");
            }
        }
        else
        {
            Response.Redirect("HSCIndex.aspx");
        }

        if (!IsPostBack)
        {
            SDS_HSCGRMsg.SelectCommand = gHSCGRMsgCon + " WHERE a.HCreateDT LIKE '%" + gCreateDate + "%'  AND a.HStatus=1 ORDER BY a.HCreateDT DESC";
            Rpt_HSCGRMsg.DataBind();

        }
    }

    #region 主題-心情功能
    //HType：1=讚 /  2=愛心 / 3=微笑
    protected void LBtn_HTopicType_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;

        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HSCTopicID, HMemberID, HType FROM HSCTopic_Mood WHERE HSCTopicID='" + LB_TID.Text + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        if (dr.Read())
        {
            if (btn.TabIndex.ToString() != dr["HType"].ToString())
            {
                SqlCommand cmd = new SqlCommand("UPDATE [HSCTopic_Mood] SET HType=@HType, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCTopicID=@HSCTopicID AND HMemberID=@HMemberID", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HSCTopicID", LB_TID.Text);
                cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HType", btn.TabIndex.ToString());
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                cmd.ExecuteNonQuery();
                ConStr.Close();
                cmd.Cancel();
            }

        }
        else
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO [HSCTopic_Mood] (HSCTopicID, HMemberID, HType, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HType, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCTopicID", LB_TID.Text);
            cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HType", btn.TabIndex.ToString());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
        }
        dr.Close();
    }

    #endregion

    #region 回應主題
    //EA20240718_新增回應主題-送出回應、資料繫結、上傳檔案功能
    //EA20240719_新增回應主題-心情功能
    //EA20240720_新增刪除已上傳檔案功能、編輯留言功能、刪除留言功能
    #region 回應主題-送出回應
    protected void LBtn_HSubmitMsg_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(CKE_HContent.Text) && string.IsNullOrEmpty(TB_HVideoLink.Text) && string.IsNullOrEmpty(LB_HFile1.Text))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入想要回應的內容');", true);
            return;
        }

        SqlCommand cmd = new SqlCommand("INSERT INTO [HSCTMsg] (HSCTopicID, HMemberID, HContent, HFile1, HVideoLink, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HContent, @HFile1, @HVideoLink, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HSCTopicID", LB_TID.Text.Trim());
        cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HContent", CKE_HContent.Text.Trim());
        cmd.Parameters.AddWithValue("@HFile1", LB_HFile1.Text.Trim());
        cmd.Parameters.AddWithValue("@HVideoLink", TB_HVideoLink.Text.Trim());
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('新增成功!');", true);

        CKE_HContent.Text = null;
        TB_HVideoLink.Text = null;
        LB_HFile1.Text = null;
        LB_HFileMsg1.Text = null;
        LBtn_HFile1_Del.Visible = false;

        SDS_HSCGRMsg.SelectCommand = gHSCGRMsgCon + " WHERE a.HCreateDT LIKE '%" + gCreateDate + "%'  AND a.HStatus=1 ORDER BY a.HCreateDT DESC";
        Rpt_HSCGRMsg.DataBind();

    }
    #endregion

    #region 回應主題-資料繫結
    protected void Rpt_HSCGRMsg_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //大頭照顯示
        UserImgShow(((Label)e.Item.FindControl("LB_HImg")).Text, ((Image)e.Item.FindControl("IMG_HImg")));

        #region MP3檔案顯示判斷
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_Audio")).Text))
        {
            if (((Label)e.Item.FindControl("LB_Audio")).Text.Substring(((Label)e.Item.FindControl("LB_Audio")).Text.Length - 4, 4).ToLower() == ".mp3")
            {
                ((HtmlControl)e.Item.FindControl("Audio1")).Visible = true;
            }
        }
        #endregion

        #region 編輯留言顯示判斷
        SqlDataReader userdr = SQLdatabase.ExecuteReader("SELECT HID, HMemberID FROM HSCGRMsg WHERE HID = '" +  ((Label)e.Item.FindControl("LB_HSCGRMsgID")).Text  + "' ");
        if (userdr.Read())
        {
            if (((Label)Master.FindControl("LB_HUserHID")).Text == userdr["HMemberID"].ToString())
            {
                ((HtmlControl)e.Item.FindControl("Li_MsgEdit")).Visible = true;
                ((HtmlControl)e.Item.FindControl("Li_MsgDel")).Visible = true;
                ((HtmlControl)e.Item.FindControl("Li_MsgHide")).Visible = true;
            }
            else
            {
                ((HtmlControl)e.Item.FindControl("Li_MsgEdit")).Visible = false;
                ((HtmlControl)e.Item.FindControl("Li_MsgDel")).Visible = false;
                ((HtmlControl)e.Item.FindControl("Li_MsgHide")).Visible = false;
            }
        }

        userdr.Close();

        #endregion

        #region 回應留言
        Repeater Rpt_HSCGRMsgResponse = e.Item.FindControl("Rpt_HSCGRMsgResponse") as Repeater;  //找到Repeater物件
        SqlDataSource SDS_HSCGRMsgResponse = e.Item.FindControl("SDS_HSCGRMsgResponse") as SqlDataSource;  //找到SqlDataSource物件        

        SDS_HSCGRMsgResponse.SelectCommand = gHSCGRMsgResponseCon + " WHERE a.HSCGRMsgID='" + ((Label)e.Item.FindControl("LB_HSCGRMsgID")).Text + "' AND c.HCreateDT LIKE '%" + gCreateDate + "%' AND a.HStatus=1";
        SDS_HSCGRMsgResponse.DataBind();
        Rpt_HSCGRMsgResponse.DataBind();
        #endregion



    }
    #endregion

    #region 回應主題-上傳檔案功能
    protected void LBtn_HFile1_Click(object sender, EventArgs e)
    {

        //判斷檔案路徑是否存在，不存在則建立資料夾 
        if (!Directory.Exists(Server.MapPath("~/uploads/HSCTMsg/" + DateTime.Now.ToString("yyyyMMdd"))))
        {
            Directory.CreateDirectory(Server.MapPath("~/uploads/HSCTMsg/" + DateTime.Now.ToString("yyyyMMdd")));//不存在就建立目錄 
        }

        string gUPFile = "~/uploads/HSCTMsg/" + DateTime.Now.ToString("yyyyMMdd") + "/";
        bool FileIsValidUP1 = false;

        if (FU_HFile1.HasFile)
        {
            //取得上傳檔案大小
            int fileSize = FU_HFile1.PostedFile.ContentLength;
            if (fileSize > 200 * 1024 * 1024) //200MB
            {
                LB_HFileMsg1.Text = "檔案上限為200MB";
                LB_HFileMsg1.ForeColor = Color.Red;
            }
            else
            {

                string gUPFileMain = Path.GetFileNameWithoutExtension(FU_HFile1.FileName);//取得上傳檔案的主檔名(沒有副檔名)
                string gUPFileExtension = System.IO.Path.GetExtension(FU_HFile1.FileName).ToLower(); //取得上傳檔案的副檔名

                String[] restrictExtension = { ".jpg", ".png", ".gif", ".mp3", ".pdf", ".docx", ".doc", ".xlsx", ".xls" };
                foreach (string i in restrictExtension)
                {
                    if (gUPFileExtension == i)
                    {
                        FileIsValidUP1 = true;
                        break;
                    }
                }

                if (FileIsValidUP1)
                {
                    //全檔名
                    LB_HFile1.Text = DateTime.Now.ToString("yyyyMMddssff") + "_" + gUPFileMain + gUPFileExtension;
                    FU_HFile1.SaveAs(Server.MapPath(gUPFile) + LB_HFile1.Text);

                    LB_HFileMsg1.Text = "上傳成功";
                    LB_HFileMsg1.ForeColor = Color.Red;
                    LBtn_HFile1_Del.Visible = true;

                }
                else
                {
                    LB_HFileMsg1.Text = "上傳檔案格式錯誤";
                    LB_HFileMsg1.ForeColor = Color.Red;

                }


            }
        }
        else
        {
            LB_HFileMsg1.Text = "請選擇上傳檔案";
            LB_HFileMsg1.ForeColor = Color.Red;

        }

        return;


    }
    #endregion

    #region 回應主題-刪除已上傳檔案
    protected void LBtn_HFile1_Del_Click(object sender, EventArgs e)
    {
        string sourceFile = File1Root + LB_HFile1.Text;

        if (File.Exists(sourceFile)) File.Delete(sourceFile);
        LB_HFile1.Text = null;
        LB_HFileMsg1.Text = null;
        LBtn_HFile1_Del.Visible = false;

    }
    #endregion

    #region 回應主題-心情功能
    //HType：1=讚 /  2=愛心 / 3=微笑
    protected void LBtn_HMsgType_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        int index = Convert.ToInt32(btn.CommandArgument);

        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HSCGRMsgID, HMemberID, HType FROM HSCGRMsg_Mood WHERE HSCGRMsgID='" + ((Label)Rpt_HSCGRMsg.Items[index].FindControl("LB_HSCGRMsgID")).Text + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        if (dr.Read())
        {
            if (btn.TabIndex.ToString() != dr["HType"].ToString())
            {
                SqlCommand cmd = new SqlCommand("UPDATE HSCGRMsg_Mood SET HType=@HType, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCGRMsgID=@HSCGRMsgID AND HMemberID=@HMemberID", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HSCGRMsgID", ((Label)Rpt_HSCGRMsg.Items[index].FindControl("LB_HSCGRMsgID")).Text);
                cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HType", btn.TabIndex.ToString());
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                cmd.ExecuteNonQuery();
                ConStr.Close();
                cmd.Cancel();
            }

        }
        else
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO HSCGRMsg_Mood (HSCGRMsgID, HMemberID, HType, HStatus, HCreate, HCreateDT ) VALUES ( @HSCGRMsgID, @HMemberID, @HType, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCGRMsgID", ((Label)Rpt_HSCGRMsg.Items[index].FindControl("LB_HSCGRMsgID")).Text);
            cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HType", btn.TabIndex.ToString());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
        }
        dr.Close();

        if (((HtmlGenericControl)Rpt_HSCGRMsg.Items[index].FindControl("Div_FeelingArea")).Visible == true)
        {
            ((HtmlGenericControl)Rpt_HSCGRMsg.Items[index].FindControl("Div_FeelingArea")).Visible = false;
        }


    }
    #endregion

    #region 回應主題-編輯留言

    #region 編輯留言Modal
    //因成長紀錄的留言是從自己發的成長紀錄來，故不直接在這修改
    protected void LBtn_MsgEdit_Click(object sender, EventArgs e)
    {
        LinkButton gButton1 = sender as LinkButton;
        string gButton1_CA = gButton1.CommandArgument;
        LB_HSCTMsgID.Text = gButton1_CA;

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_MsgEdit').modal('show');</script>", false);//執行js

        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HContent, HFile1, HVideoLink  FROM HSCTMsg WHERE HID ='" + gButton1_CA + "'");

        if (dr.Read())
        {
            CKE_HContent_Edit.Text = dr["HContent"].ToString();
            TB_HVideoLink_Edit.Text = dr["HVideoLink"].ToString();
            LB_HFile1_Edit.Text = dr["HFile1"].ToString();

            if (!string.IsNullOrEmpty(dr["HFile1"].ToString()))
            {
                LBtn_HFile1_EDel.Visible = true;

                string[] filename = dr["HFile1"].ToString().Split('_');
                //HL_HFile1_Edit.Text = dr["HFile1"].ToString();
                HL_HFile1_Edit.Text = "瀏覽檔案";
                HL_HFile1_Edit.NavigateUrl = "~/uploads/HSCTMsg/" + filename[0].Substring(0, 8) + "/" + dr["HFile1"].ToString();
            }

        }
        dr.Close();

    }
    #endregion

    #region 儲存功能
    protected void Btn_MsgEditSubmit_Click(object sender, EventArgs e)
    {
        SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsg] SET HContent=@HContent, HFile1=@HFile1, HVideoLink=@HVideoLink, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgID.Text);
        cmd.Parameters.AddWithValue("@HContent", CKE_HContent_Edit.Text.Trim());
        cmd.Parameters.AddWithValue("@HFile1", LB_HFile1_Edit.Text.Trim());
        cmd.Parameters.AddWithValue("@HVideoLink", TB_HVideoLink_Edit.Text.Trim());
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();


        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('儲存成功!');", true);

        SDS_HSCGRMsg.SelectCommand = gHSCGRMsgCon + " WHERE a.HCreateDT LIKE '%" + gCreateDate + "%'  AND a.HStatus=1 ORDER BY a.HCreateDT DESC";
        SDS_HSCGRMsg.DataBind();
        Rpt_HSCGRMsg.DataBind();
    }
    #endregion

    #region 上傳檔案功能
    protected void LBtn_HFile1_Edit_Click(object sender, EventArgs e)
    {
        //判斷檔案路徑是否存在，不存在則建立資料夾 
        if (!Directory.Exists(Server.MapPath("~/uploads/HSCTMsg/" + DateTime.Now.ToString("yyyyMMdd"))))
        {
            Directory.CreateDirectory(Server.MapPath("~/uploads/HSCTMsg/" + DateTime.Now.ToString("yyyyMMdd")));//不存在就建立目錄 
        }

        string gUPFile = "~/uploads/HSCTMsg/" + DateTime.Now.ToString("yyyyMMdd") + "/";
        bool FileIsValidUP1 = false;

        if (FU_HFile1_Edit.HasFile)
        {
            //取得上傳檔案大小
            int fileSize = FU_HFile1_Edit.PostedFile.ContentLength;
            if (fileSize > 200 * 1024 * 1024) //200MB
            {
                LB_HFileMsg1_Edit.Text = "檔案上限為200MB";
                LB_HFileMsg1_Edit.ForeColor = Color.Red;
            }
            else
            {

                string gUPFileMain = Path.GetFileNameWithoutExtension(FU_HFile1_Edit.FileName);//取得上傳檔案的主檔名(沒有副檔名)
                string gUPFileExtension = System.IO.Path.GetExtension(FU_HFile1_Edit.FileName).ToLower(); //取得上傳檔案的副檔名

                String[] restrictExtension = { ".jpg", ".png", ".gif", ".mp3", ".pdf", ".docx", ".doc", ".xlsx", ".xls" };
                foreach (string i in restrictExtension)
                {
                    if (gUPFileExtension == i)
                    {
                        FileIsValidUP1 = true;
                        break;
                    }
                }

                if (FileIsValidUP1)
                {
                    //全檔名
                    LB_HFile1_Edit.Text = DateTime.Now.ToString("yyyyMMddssff") + "_" + gUPFileMain + gUPFileExtension;
                    FU_HFile1_Edit.SaveAs(Server.MapPath(gUPFile) + LB_HFile1_Edit.Text);

                    LB_HFileMsg1_Edit.Text = "上傳成功";
                    LB_HFileMsg1_Edit.ForeColor = Color.Red;
                    LBtn_HFile1_EDel.Visible = true;

                }
                else
                {
                    LB_HFileMsg1_Edit.Text = "上傳檔案格式錯誤";
                    LB_HFileMsg1_Edit.ForeColor = Color.Red;

                }


            }
        }
        else
        {
            LB_HFileMsg1_Edit.Text = "請選擇上傳檔案";
            LB_HFileMsg1_Edit.ForeColor = Color.Red;

        }

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_MsgEdit').modal('show');</script>", false);//執行js

    }
    #endregion

    #region 刪除已上傳檔案
    protected void LBtn_HFile1_EDel_Click(object sender, EventArgs e)
    {
        string sourceFile = File1Root + LB_HFile1_Edit.Text;

        if (File.Exists(sourceFile)) File.Delete(sourceFile);
        LB_HFile1_Edit.Text = null;
        LB_HFileMsg1_Edit.Text = null;
        LBtn_HFile1_EDel.Visible = false;
        HL_HFile1_Edit.Text = null;
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_MsgEdit').modal('show');</script>", false);//執行js

    }
    #endregion

    #endregion

    #region 回應主題-刪除留言
    //留言狀態：dbo.HSCTMsg.HStatus (0:刪除/1:正常/2:隱藏)
    //留言Log類型：dbo.HSCTMsg_Log.HLogType (0:刪除/1:編輯/2:隱藏)
    #region 刪除留言Modal
    protected void LBtn_MsgDel_Click(object sender, EventArgs e)
    {
        LinkButton gButton1 = sender as LinkButton;
        string gButton1_CA = gButton1.CommandArgument;
        LB_HSCTMsgID.Text = gButton1_CA;

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgDelModal').modal();"), true);

        Div_MsgDel.Visible = true;
        Div_MsgResponseDel.Visible = false;


    }
    #endregion

    #region 刪除留言
    protected void Btn_MsgDel_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_HMsgReason_Del.Text))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入刪除留言原因');", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgDelModal').modal();"), true);
            return;
        }

        //變更留言狀態
        SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsg] SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgID.Text);
        cmd.Parameters.AddWithValue("@HStatus", 0);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCTMsg_Log] (HSCTMsgID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCTMsgID", LB_HSCTMsgID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 0);
        logcmd.Parameters.AddWithValue("@HReason", TB_HMsgReason_Del.Text.Trim());
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('刪除成功!');", true);

        SDS_HSCGRMsg.SelectCommand = gHSCGRMsgCon + " WHERE a.HCreateDT LIKE '%" + gCreateDate + "%'  AND a.HStatus=1 ORDER BY a.HCreateDT DESC";
        SDS_HSCGRMsg.DataBind();
        Rpt_HSCGRMsg.DataBind();

    }
    #endregion

    #endregion

    #region 回應主題-隱藏留言
    //留言狀態：dbo.HSCTMsg.HStatus (0:刪除/1:正常/2:隱藏)
    //留言Log類型：dbo.HSCTMsg_Log.HLogType (0:刪除/1:編輯/2:隱藏)
    #region 隱藏留言Modal
    protected void LBtn_MsgHide_Click(object sender, EventArgs e)
    {
        LinkButton gButton1 = sender as LinkButton;
        string gButton1_CA = gButton1.CommandArgument;
        LB_HSCTMsgID.Text = gButton1_CA;

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgHideModal').modal();"), true);

        Div_MsgHide.Visible = true;
        Div_MsgResponseHide.Visible = false;
    }
    #endregion

    #region 隱藏留言
    protected void Btn_MsgHide_Click(object sender, EventArgs e)
    {
          if (string.IsNullOrEmpty(TB_HMsgReason_Hide.Text))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入隱藏留言原因');", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgHideModal').modal();"), true);
            return;
        }

        //變更留言狀態
        SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsg] SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgID.Text);
        cmd.Parameters.AddWithValue("@HStatus", 2);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCTMsg_Log] (HSCTMsgID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCTMsgID", LB_HSCTMsgID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 2);
        logcmd.Parameters.AddWithValue("@HReason", TB_HMsgReason_Hide.Text.Trim());
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('隱藏成功!');", true);

        SDS_HSCGRMsg.SelectCommand = gHSCGRMsgCon + " WHERE a.HCreateDT LIKE '%" + gCreateDate + "%'  AND a.HStatus=1 ORDER BY a.HCreateDT DESC";
        SDS_HSCGRMsg.DataBind();
        Rpt_HSCGRMsg.DataBind();

    }
    #endregion

    #endregion

    #endregion

    #region 回應留言
    //EA20240718_新增回應留言-送出回應功能
    //EA20240720_編輯回應功能、刪除回應功能
    #region 回應留言-送出回應
    protected void LBtn_HSubmitMsgResponse_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Submit = sender as LinkButton;
        int index = Convert.ToInt32(LBtn_Submit.CommandArgument);

        if (string.IsNullOrEmpty(((HtmlTextArea)Rpt_HSCGRMsg.Items[index].FindControl("TA_HMsgResponse")).InnerText))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入想要回應留言的內容');", true);
            return;
        }

        SqlCommand cmd = new SqlCommand("INSERT INTO HSCGRMsgResponse (HSCGRMsgID, HMemberID, HGRMRContent, HStatus, HCreate, HCreateDT ) VALUES (@HSCGRMsgID, @HMemberID, @HGRMRContent, @HStatus, @HCreate, @HCreateDT);SELECT SCOPE_IDENTITY() AS HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HSCGRMsgID", ((Label)Rpt_HSCGRMsg.Items[index].FindControl("LB_HSCGRMsgID")).Text);
        cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HGRMRContent", ((HtmlTextArea)Rpt_HSCGRMsg.Items[index].FindControl("TA_HMsgResponse")).InnerText);
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        //cmd.ExecuteNonQuery();
        int gHSCGRMsgResponseID = Convert.ToInt32(cmd.ExecuteScalar().ToString());
        ConStr.Close();
        cmd.Cancel();

        ((HtmlTextArea)Rpt_HSCGRMsg.Items[index].FindControl("TA_HMsgResponse")).InnerText = null;


        //回應主題通知
        /// <summary>
        /// 新增通知紀錄
        /// </summary>
        /// <param name="gHMemberID">接收通知者</param>
        /// <param name="gHActorMemberID">觸發通知者</param>
        /// <param name="gHNotifyType">通知類型(1:回應、2:心情(讚)、3:心情(愛心)、4:心情(微笑)、5:分享、6:提問、7:被刪除留言/回應、8:被隱藏留言/回應)</param>
        /// <param name="gHTargetID">對應資料表的流水號</param>
        /// <param name="gTableName">對應資料表名稱</param>
        Notification.AddNotification(((Label)Rpt_HSCGRMsg.Items[index].FindControl("LB_HMemberID")).Text, ((Label)Master.FindControl("LB_HUserHID")).Text, "1", gHSCGRMsgResponseID.ToString(), "HSCGRMsgResponse");


        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('新增成功!');", true);

        Repeater Rpt_HSCGRMsgResponse = ((Repeater)Rpt_HSCGRMsg.Items[index].FindControl("Rpt_HSCGRMsgResponse")) as Repeater;  //找到repeater物件
        SqlDataSource SDS_HSCGRMsgResponse = ((SqlDataSource)Rpt_HSCGRMsg.Items[index].FindControl("SDS_HSCGRMsgResponse")) as SqlDataSource;   //找到SqlDataSource物件        

        SDS_HSCGRMsgResponse.SelectCommand = gHSCGRMsgResponseCon + " WHERE a.HSCGRMsgID='" + ((Label)Rpt_HSCGRMsg.Items[index].FindControl("LB_HSCGRMsgID")).Text + "' AND c.HCreateDT LIKE '%" + gCreateDate + "%' AND a.HStatus=1";
        Rpt_HSCGRMsgResponse.DataBind();

    }
    #endregion

    #region 回應留言-資料繫結
    protected void Rpt_HSCGRMsgResponse_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //大頭照顯示
        UserImgShow(((Label)e.Item.FindControl("LB_HImg")).Text, ((Image)e.Item.FindControl("IMG_HImg")));

        #region 編輯回應顯示判斷
        SqlDataReader userdr = SQLdatabase.ExecuteReader("SELECT HID, HMemberID FROM HSCGRMsgResponse WHERE HID = '" + ((Label)e.Item.FindControl("LB_HID")).Text + "'");

        if (userdr.Read())
        {
            if (((Label)Master.FindControl("LB_HUserHID")).Text == userdr["HMemberID"].ToString())
            {
                ((HtmlControl)e.Item.FindControl("Li_MsgResponseEdit")).Visible = true;
                ((HtmlControl)e.Item.FindControl("Li_MsgResponseDel")).Visible = true;
                ((HtmlControl)e.Item.FindControl("Li_MsgResponseHide")).Visible = true;
            }
            else
            {
                ((HtmlControl)e.Item.FindControl("Li_MsgResponseEdit")).Visible = false;
                ((HtmlControl)e.Item.FindControl("Li_MsgResponseDel")).Visible = false;
                ((HtmlControl)e.Item.FindControl("Li_MsgResponseHide")).Visible = false;
            }
        }

        userdr.Close();

        #endregion


        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HCreateDT")).Text))
        {
            ((Label)e.Item.FindControl("LB_HCreateDT")).Text = ((Label)e.Item.FindControl("LB_HCreateDT")).Text.Substring(0, 4) + "/" + ((Label)e.Item.FindControl("LB_HCreateDT")).Text.Substring(4, 2) + "/" + ((Label)e.Item.FindControl("LB_HCreateDT")).Text.Substring(6, 2)+ " " + ((Label)e.Item.FindControl("LB_HCreateDT")).Text.Substring(8, 2)+":" + ((Label)e.Item.FindControl("LB_HCreateDT")).Text.Substring(10, 2)+":" + ((Label)e.Item.FindControl("LB_HCreateDT")).Text.Substring(12, 2);
        }
    }
    #endregion

    #region 回應留言-編輯回應

    #region 編輯回應Modal
    protected void LBtn_MsgResponseEdit_Click(object sender, EventArgs e)
    {
        LinkButton gButton1 = sender as LinkButton;
        LB_HSCGRMsgResponseID.Text = gButton1.CommandArgument;
        //AA20240909_加入回應第幾筆&判斷
        int gMsgResponse_CN = Convert.ToInt32(gButton1.CommandName);

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_MsgResponseEdit').modal('show');</script>", false);//執行js

        //AA20240909_抓父層(上一層)的repeater寫法(用NamingContainer)
        var gChildRepeaterItem = (RepeaterItem)gButton1.NamingContainer;
        var gChildRepeater = (Repeater)gChildRepeaterItem.NamingContainer;
        var gParentRepeaterItem = (RepeaterItem)gChildRepeater.NamingContainer;
        var gParentRepeaterItemIndex = gParentRepeaterItem.ItemIndex;

        Repeater Rpt_HSCGRMsgResponse = ((Repeater)Rpt_HSCGRMsg.Items[gParentRepeaterItemIndex].FindControl("Rpt_HSCGRMsgResponse")) as Repeater;
        ((HtmlControl)Rpt_HSCGRMsgResponse.Items[gMsgResponse_CN].FindControl("Div_ReplyArea")).Visible = false;




        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HGRMRContent FROM HSCGRMsgResponse WHERE HID ='" + LB_HSCGRMsgResponseID.Text + "'");

        if (dr.Read())
        {
            TA_HGRMRContent_Edit.InnerText = dr["HGRMRContent"].ToString();
        }
        dr.Close();

    }
    #endregion

    #region 儲存功能
    protected void Btn_MsgREitdSubmit_Click(object sender, EventArgs e)
    {
        SqlCommand cmd = new SqlCommand("UPDATE HSCGRMsgResponse SET HGRMRContent=@HGRMRContent, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_HSCGRMsgResponseID.Text);
        cmd.Parameters.AddWithValue("@HGRMRContent", TA_HGRMRContent_Edit.InnerText);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();


        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('儲存成功!');", true);


        SDS_HSCGRMsg.SelectCommand = gHSCGRMsgCon + " WHERE a.HCreateDT LIKE '%" + gCreateDate + "%'  AND a.HStatus=1 ORDER BY a.HCreateDT DESC";
        Rpt_HSCGRMsg.DataBind();

    }
    #endregion


    #endregion

    #region 回應留言-刪除回應
    //回應留言狀態：dbo.HSCGRMsgResponse.HStatus (0:刪除/1:正常/2:隱藏)
    //回應留言Log類型：dbo.HSCGRMsgResponse_Log.HLogType (0:刪除/1:編輯/2:隱藏)
    #region 刪除回應Modal
    protected void LBtn_MsgResponseDel_Click(object sender, EventArgs e)
    {

        LinkButton gButton1 = sender as LinkButton;
        LB_HSCGRMsgResponseID.Text = gButton1.CommandArgument;

        //AA20240909_加入回應第幾筆&判斷
        int gMsgResponse_CN = Convert.ToInt32(gButton1.CommandName);

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgDelModal').modal();"), true);

        //AA20240909_抓父層(上一層)的repeater寫法(用NamingContainer)
        var gChildRepeaterItem = (RepeaterItem)gButton1.NamingContainer;
        var gChildRepeater = (Repeater)gChildRepeaterItem.NamingContainer;
        var gParentRepeaterItem = (RepeaterItem)gChildRepeater.NamingContainer;
        var gParentRepeaterItemIndex = gParentRepeaterItem.ItemIndex;

        Repeater Rpt_HSCGRMsgResponse = ((Repeater)Rpt_HSCGRMsg.Items[gParentRepeaterItemIndex].FindControl("Rpt_HSCGRMsgResponse")) as Repeater;
        ((HtmlControl)Rpt_HSCGRMsgResponse.Items[gMsgResponse_CN].FindControl("Div_ReplyArea")).Visible = false;


        Div_MsgDel.Visible = false;
        Div_MsgResponseDel.Visible = true;
    }
    #endregion

    #region 刪除回應
    protected void Btn_MsgResponseDel_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_MsgResponse_Del.Text))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入刪除回應原因');", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgDelModal').modal();"), true);
            return;
        }

        //變更留言狀態
        SqlCommand cmd = new SqlCommand("UPDATE [HSCGRMsgResponse] SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_HSCGRMsgResponseID.Text);
        cmd.Parameters.AddWithValue("@HStatus", 0);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCGRMsgResponse_Log] (HSCGRMsgResponseID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCGRMsgResponseID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCGRMsgResponseID", LB_HSCGRMsgResponseID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 0);
        logcmd.Parameters.AddWithValue("@HReason", TB_MsgResponse_Del.Text.Trim());
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('刪除成功!');", true);

        SDS_HSCGRMsg.SelectCommand = gHSCGRMsgCon + " WHERE a.HCreateDT LIKE '%" + gCreateDate + "%'  AND a.HStatus=1 ORDER BY a.HCreateDT DESC";
        Rpt_HSCGRMsg.DataBind();

    }
    #endregion

    #endregion

    #region 回應留言-隱藏回應
    //回應留言狀態：dbo.HSCGRMsgResponse.HStatus (0:刪除/1:正常/2:隱藏)
    //回應留言Log類型：dbo.HSCGRMsgResponse_Log.HLogType (0:刪除/1:編輯/2:隱藏)
    #region 隱藏回應Modal
    protected void LBtn_MsgResponseHide_Click(object sender, EventArgs e)
    {
        LinkButton gButton1 = sender as LinkButton;
        LB_HSCGRMsgResponseID.Text = gButton1.CommandArgument;
        //AA20240909_加入回應第幾筆&判斷
        int gMsgResponse_CN = Convert.ToInt32(gButton1.CommandName);

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgHideModal').modal();"), true);

        //AA20240909_抓父層(上一層)的repeater寫法(用NamingContainer)
        var gChildRepeaterItem = (RepeaterItem)gButton1.NamingContainer;
        var gChildRepeater = (Repeater)gChildRepeaterItem.NamingContainer;
        var gParentRepeaterItem = (RepeaterItem)gChildRepeater.NamingContainer;
        var gParentRepeaterItemIndex = gParentRepeaterItem.ItemIndex;

        Repeater Rpt_HSCGRMsgResponse = ((Repeater)Rpt_HSCGRMsg.Items[gParentRepeaterItemIndex].FindControl("Rpt_HSCGRMsgResponse")) as Repeater;
        ((HtmlControl)Rpt_HSCGRMsgResponse.Items[gMsgResponse_CN].FindControl("Div_ReplyArea")).Visible = false;

        Div_MsgHide.Visible = false;
        Div_MsgResponseHide.Visible = true;
    }
    #endregion

    #region 隱藏回應
    protected void Btn_MsgResponseHide_Click(object sender, EventArgs e)
    {
             if (string.IsNullOrEmpty(TB_MsgResponse_Hide.Text))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入隱藏回應原因');", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgHideModal').modal();"), true);
            return;
        }

        //變更留言狀態
        SqlCommand cmd = new SqlCommand("UPDATE [HSCGRMsgResponse] SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_HSCGRMsgResponseID.Text);
        cmd.Parameters.AddWithValue("@HStatus", 2);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCGRMsgResponse_Log] (HSCGRMsgResponseID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCGRMsgResponseID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCGRMsgResponseID", LB_HSCGRMsgResponseID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 2);
        logcmd.Parameters.AddWithValue("@HReason", TB_MsgResponse_Hide.Text.Trim());
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('隱藏成功!');", true);

        SDS_HSCGRMsg.SelectCommand = gHSCGRMsgCon + " WHERE a.HCreateDT LIKE '%" + gCreateDate + "%'  AND a.HStatus=1 ORDER BY a.HCreateDT DESC";
        SDS_HSCGRMsg.DataBind();
        Rpt_HSCGRMsg.DataBind();
    }
    #endregion

    #endregion

    #endregion

    #region 回應留言的更多選項(編輯、刪除、隱藏)
    //AA20240724_新增回應留言的編輯、刪除、隱藏
    protected void LBtn_ReplyMore_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_ReplyMore = sender as LinkButton;
        string gMore_CA = LBtn_ReplyMore.CommandArgument;
        int gMore_CN = Convert.ToInt32(LBtn_ReplyMore.CommandName);

        //AA20240828_抓父層(上一層)的repeater寫法(用NamingContainer)
        var gChildRepeaterItem = (RepeaterItem)LBtn_ReplyMore.NamingContainer;
        var gChildRepeater = (Repeater)gChildRepeaterItem.NamingContainer;
        var gParentRepeaterItem = (RepeaterItem)gChildRepeater.NamingContainer;
        var gParentRepeaterItemIndex = gParentRepeaterItem.ItemIndex;

        Repeater Rpt_HSCGRMsgResponse = ((Repeater)Rpt_HSCGRMsg.Items[gParentRepeaterItemIndex].FindControl("Rpt_HSCGRMsgResponse")) as Repeater;
        ((HtmlControl)Rpt_HSCGRMsgResponse.Items[gMore_CN].FindControl("Div_ReplyArea")).Visible = false;

        //判斷外層repeater的
        for (int y = 0; y < Rpt_HSCGRMsg.Items.Count; y++)
        {
            if (y != gParentRepeaterItemIndex)
            {
                Repeater OtherRepeater = ((Repeater)Rpt_HSCGRMsg.Items[y].FindControl("Rpt_HSCGRMsgResponse")) as Repeater;

                for (int i = 0; i < OtherRepeater.Items.Count; i++)
                {
                    ((HtmlControl)OtherRepeater.Items[i].FindControl("Div_ReplyArea")).Visible = false;
                }
            }
            else
            {
                for (int x = 0; x < Rpt_HSCGRMsgResponse.Items.Count; x++)
                {
                    if (x != gMore_CN)
                    {
                        ((HtmlControl)Rpt_HSCGRMsgResponse.Items[x].FindControl("Div_ReplyArea")).Visible = false;
                    }

                }
            }

        }

        //顯示資料
        if (((HtmlControl)Rpt_HSCGRMsgResponse.Items[gMore_CN].FindControl("Div_ReplyArea")).Visible == false)
        {
            ((HtmlControl)Rpt_HSCGRMsgResponse.Items[gMore_CN].FindControl("Div_ReplyArea")).Visible = true;
        }
        else
        {
            ((HtmlControl)Rpt_HSCGRMsgResponse.Items[gMore_CN].FindControl("Div_ReplyArea")).Visible = false;
        }




    }
    #endregion


    #region 主題心情項目(按讚、愛心、笑臉)
    protected void LBtn_HSCTopicMood_Click(object sender, EventArgs e)
    {

        if (Div_TFeelingArea.Visible == false)
        {
            Div_TFeelingArea.Visible = true;
        }
        else
        {
            Div_TFeelingArea.Visible = false;
        }

    }
    #endregion

    #region 留言心情項目(按讚、愛心、笑臉)
    protected void LBtn_HSCTMsgMood_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_HSCTMsgMood = sender as LinkButton;

        string gFeeling_CA = LBtn_HSCTMsgMood.CommandArgument;
        int gFeeling_CN = 0;

        if (!string.IsNullOrEmpty(LBtn_HSCTMsgMood.CommandName))
        {
            gFeeling_CN = Convert.ToInt32(LBtn_HSCTMsgMood.CommandName);
        }
        //判斷若已經有點開某一個，則要隱藏
        for (int i = 0; i < Rpt_HSCGRMsg.Items.Count; i++)
        {
            if (i != gFeeling_CN)
            {
                ((HtmlControl)Rpt_HSCGRMsg.Items[i].FindControl("Div_FeelingArea")).Visible = false;
            }
        }

        if (((HtmlControl)Rpt_HSCGRMsg.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible == false)
        {
            ((HtmlControl)Rpt_HSCGRMsg.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible = true;
        }
        else
        {
            ((HtmlControl)Rpt_HSCGRMsg.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible = false;
        }



    }
    #endregion



    #region Function

    #region 大頭照顯示
    private void UserImgShow(string LB_HImg, Image Img)
    {
        if (string.IsNullOrEmpty(LB_HImg))
        {
            Img.ImageUrl = "images/icons/profile_small.jpg";
        }
        else
        {
            Img.ImageUrl = "uploads/Account/" + LB_HImg;
        }
    }
    #endregion

    #endregion

    protected void LinkButton35_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgHideModal').modal();"), true);
    }


    #region 關鍵字連結
    protected void LBtn_HHashTag_Click(object sender, EventArgs e)
    {

    }
    #endregion

    #region 瀏覽數量計算
    private int ViewCounts(string SCTopicID)
    {
        //資料讀取
        SqlDataReader QueryView = SQLdatabase.ExecuteReader("SELECT HTimes FROM HSCTopic_View WHERE HSCTopicID='" + SCTopicID + "' AND HStatus=1 ");

        int gView = 0;

        while (QueryView.Read())
        {
            gView += Convert.ToInt32(QueryView["HTimes"].ToString());
        }
        QueryView.Close();

        //LB_ViewNum.Text = gView.ToString("N0");

        return gView;
    }
    #endregion



}