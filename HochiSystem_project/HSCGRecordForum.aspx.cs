using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using NPOI.SS.Formula.Functions;
using System;
using System.Activities.Expressions;
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

public partial class HSCGRecordForum : System.Web.UI.Page
{
    SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    string Con = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;

    #region --根目錄--
    string SCFileRoot = "D:\\Website\\System\\HochiSystem\\uploads\\SpecialColumn\\";
    string SCFile = "~/uploads/SpecialColumn/";
    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SDS_HSCGRecord.SelectCommand = "SELECT a.HID,  HTopicName, (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, e.HImg,a.HCreateDT, a.HModifyDT FROM HSCGRecord AS a LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 ORDER BY a.HModifyDT DESC, a.HCreateDT DESC";
            Rpt_HSCGRecord.DataBind();
        }
        else
        {
            SDS_HSCGRecord.SelectCommand = "SELECT a.HID,  HTopicName, (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, e.HImg,a.HCreateDT, a.HModifyDT FROM HSCGRecord AS a LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 ORDER BY a.HModifyDT DESC, a.HCreateDT DESC";
            Rpt_HSCGRecord.DataBind();

        }

    }

    #region 進入管理員功能
    protected void LBtn_HManager_Click(object sender, EventArgs e)
    {
        Response.Redirect("HSCModerator.aspx?FClassCID=" + LB_HSCForumClassCID.Text);
    }
    #endregion

    protected void LinkButton32_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_DeleteReply').modal();"), true);
    }

    protected void LinkButton35_Click(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_HideReply').modal();"), true);
    }




    #region 資料繫結
    protected void Rpt_HSCGRecord_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //DataRowView DRV_HSCTopic = (DataRowView)e.Item.DataItem;

        ((Label)e.Item.FindControl("LB_HLink")).Text = "http://" + Request.Url.Authority + "/HSCGRecordDetail.aspx?TID=" + ((Label)e.Item.FindControl("LB_HID")).Text;

        if (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HMember")).Text))
        {
            ((Label)e.Item.FindControl("LB_HMember")).Text = "系統自動產生";
        }

        if (((Label)e.Item.FindControl("LB_HMember")).Text == "系統自動產生")
        {
            ((Image)e.Item.FindControl("IMG_Creator")).ImageUrl = "~/images/icon.png";
        }
        else
        {
            UserImgShow(((Label)e.Item.FindControl("LB_HImg")).Text, ((Image)e.Item.FindControl("IMG_Creator")));
        }


        #region 數量計算
        //留言與回應
        //ResponseCounts(((Label)e.Item.FindControl("LB_HID")).Text);
        //((Label)e.Item.FindControl("LB_MsgNum")).Text = (ResponseCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString("N0");

        string gDate=(Convert.ToDateTime(((Label)e.Item.FindControl("LB_HDate")).Text)).ToString("yyyy/MM/dd");

        ((Label)e.Item.FindControl("LB_MsgNum")).Text = (ResponseCounts(gDate)).ToString("N0");

        //心情
        FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text);
        ((Label)e.Item.FindControl("LB_FeelingNum")).Text = (FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString("N0");

        //瀏覽次數
        ((Label)e.Item.FindControl("LB_ViewNum")).Text = (ViewCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString("N0");

        //分享次數
        ((Label)e.Item.FindControl("LB_ShareNum")).Text = (ShareCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString("N0");
        #endregion


    }
    #endregion


    #region 查看更多
    protected void LBtn_View_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_View = sender as LinkButton;
        string gView_CA = LBtn_View.CommandArgument;

        //紀錄瀏覽次數
        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HSCGRecordID, HMemberID, HTimes FROM HSCGRecord_View WHERE HSCGRecordID='" + gView_CA + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        if (dr.Read())
        {
            SqlCommand cmd = new SqlCommand("UPDATE HSCGRecord_View SET HTimes=@HTimes, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCGRecordID=@HSCGRecordID AND HMemberID=@HMemberID", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCGRecordID", gView_CA);
            cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HTimes", Convert.ToInt32(dr["HTimes"].ToString()) + 1);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

        }
        else
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO HSCGRecord_View (HSCGRecordID, HMemberID, HTimes, HStatus, HCreate, HCreateDT ) VALUES ( @HSCGRecordID, @HMemberID, @HTimes, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCGRecordID", gView_CA);
            cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HTimes", 1);
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
        }
        dr.Close();

        //導頁
        Response.Redirect("HSCGRecordDetail.aspx?TID=" + gView_CA);

    }
    #endregion

    #region 分享連結
    protected void Btn_Share_Click(object sender, EventArgs e)
    {
        if (Request.Cookies["TopicID"] != null)
        {

            //紀錄分享次數
            SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HSCGRecordID, HMemberID, HTimes FROM HSCGRecord_Share WHERE HSCGRecordID='" + Request.Cookies["TopicID"].Value + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
            if (dr.Read())
            {
                SqlCommand cmd = new SqlCommand("UPDATE HSCGRecord_Share SET HTimes=@HTimes, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCGRecordID=@HSCGRecordID AND HMemberID=@HMemberID", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HSCGRecordID", Request.Cookies["TopicID"].Value);
                cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HTimes", Convert.ToInt32(dr["HTimes"].ToString()) + 1);
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                cmd.ExecuteNonQuery();
                ConStr.Close();
                cmd.Cancel();

            }
            else
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO HSCGRecord_Share (HSCGRecordID, HMemberID, HTimes, HStatus, HCreate, HCreateDT ) VALUES ( @HSCGRecordID, @HMemberID, @HTimes, @HStatus, @HCreate, @HCreateDT)", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HSCGRecordID", Request.Cookies["TopicID"].Value);
                cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HTimes", 1);
                cmd.Parameters.AddWithValue("@HStatus", 1);
                cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                cmd.ExecuteNonQuery();
                ConStr.Close();
                cmd.Cancel();
            }
            dr.Close();

            //清除cookie
            Response.Cookies["TopicID"].Expires = DateTime.Now.AddDays(-1);
        }



    }
    #endregion



    #region 心情項目(按讚、愛心、笑臉)
    protected void LBtn_Feeling_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Feeling = sender as LinkButton;
        string gFeeling_CA = LBtn_Feeling.CommandArgument;
        int gFeeling_CN = Convert.ToInt32(LBtn_Feeling.CommandName);

        //判斷若已經有點開某一個，則要隱藏
        for (int i = 0; i < Rpt_HSCGRecord.Items.Count; i++)
        {
            if (i != gFeeling_CN)
            {
                ((HtmlControl)Rpt_HSCGRecord.Items[i].FindControl("Div_FeelingArea")).Visible = false;
            }
        }

        if (((HtmlControl)Rpt_HSCGRecord.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible == false)
        {
            ((HtmlControl)Rpt_HSCGRecord.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible = true;
        }
        else
        {
            ((HtmlControl)Rpt_HSCGRecord.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible = false;
        }

    }
    #endregion

    #region 主題-心情功能
    //HType：1=讚 /  2=愛心 / 3=微笑
    protected void LBtn_FeelingType_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        string gFeelingType_CA = btn.CommandArgument;
        int gFeelingType_CN = Convert.ToInt32(btn.CommandName);

        SqlDataReader QueryFeeling = SQLdatabase.ExecuteReader("SELECT HID, HSCGRecordID, HMemberID, HType FROM HSCGRecord_Mood WHERE HSCGRecordID='" + gFeelingType_CA + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        if (QueryFeeling.Read())
        {
            if (btn.TabIndex.ToString() != QueryFeeling["HType"].ToString())
            {
                SqlCommand cmd = new SqlCommand("UPDATE HSCGRecord_Mood SET HType=@HType, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCGRecordID=@HSCGRecordID AND HMemberID=@HMemberID", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HSCGRecordID", gFeelingType_CA);
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
            SqlCommand cmd = new SqlCommand("INSERT INTO HSCGRecord_Mood (HSCGRecordID, HMemberID, HType, HStatus, HCreate, HCreateDT ) VALUES ( @HSCGRecordID, @HMemberID, @HType, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCGRecordID", gFeelingType_CA);
            cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HType", btn.TabIndex.ToString());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
        }
        QueryFeeling.Close();

        //隱藏顯示的區域
        ((HtmlControl)Rpt_HSCGRecord.Items[gFeelingType_CN].FindControl("Div_FeelingArea")).Visible = false;

        //資料繫結
        SDS_HSCGRecord.SelectCommand = "SELECT a.HID,  HTopicName, (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, e.HImg,a.HCreateDT, a.HModifyDT FROM HSCGRecord AS a LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 ORDER BY a.HModifyDT DESC, a.HCreateDT DESC";
        Rpt_HSCGRecord.DataBind();


    }
    #endregion

    #region 主題-心情功能
    //HType：1=讚 /  2=愛心 / 3=微笑
    protected void LBtn_FeelingTypeM_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;

        if (btn.TabIndex.ToString() == "1")
        {
            LBtn_ThumbsUp.CssClass = "nav-link  active show font-weight-bold";
            LBtn_Heart.CssClass = "nav-link font-weight-bold";
            LBtn_Smile.CssClass = "nav-link font-weight-bold";

            SDS_HSCGRecordMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCGRecord_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCGRecordID='" + LB_SCTopicID.Text + "' AND a.HType='1' AND a.HStatus=1";

        }
        else if (btn.TabIndex.ToString() == "2")
        {
            LBtn_ThumbsUp.CssClass = "nav-link font-weight-bold";
            LBtn_Heart.CssClass = "nav-link  active show font-weight-bold";
            LBtn_Smile.CssClass = "nav-link font-weight-bold";

            SDS_HSCGRecordMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCGRecord_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCGRecordID='" + LB_SCTopicID.Text + "' AND a.HType='2' AND a.HStatus=1";

        }
        else if (btn.TabIndex.ToString() == "3")
        {
            LBtn_ThumbsUp.CssClass = "nav-link font-weight-bold";
            LBtn_Heart.CssClass = "nav-link font-weight-bold";
            LBtn_Smile.CssClass = "nav-link  active show font-weight-bold";

            SDS_HSCGRecordMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCGRecord_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCGRecordID='" + LB_SCTopicID.Text + "' AND a.HType='3' AND a.HStatus=1";

        }

        Rpt_HSCGRecordMood.DataBind();
        //計算數量
        FeelingCounts(LB_SCTopicID.Text);

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

    }
    #endregion


    #region 查看心情紀錄
    protected void LBtn_MoodModal_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_MoodModal = sender as LinkButton;
        string gMood_CA = LBtn_MoodModal.CommandArgument;

        LB_SCTopicID.Text = gMood_CA;

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js



        //預設顯示按讚的資訊
        LBtn_ThumbsUp.CssClass = "nav-link  active show font-weight-bold";
        LBtn_Heart.CssClass = "nav-link  font-weight-bold";
        LBtn_Smile.CssClass = "nav-link  font-weight-bold";

        SDS_HSCGRecordMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCGRecord_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCGRecordID='" + gMood_CA + "' AND a.HType='1' AND a.HStatus=1";
        Rpt_HSCGRecordMood.DataBind();

        //計算數量
        FeelingCounts(gMood_CA);
    }
    #endregion

    #region 心情紀錄(Modal資料繫結)
    protected void Rpt_HSCGRecordMood_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        UserImgShow(gDRV["HImg"].ToString(), ((Image)e.Item.FindControl("Img_HImg")));
    }
    #endregion


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

    #region 心情數量計算
    private int FeelingCounts(string SCTopicID)
    {
        //資料讀取
        SqlDataReader QueryFeeling = SQLdatabase.ExecuteReader("SELECT HMemberID, HType FROM HSCGRecord_Mood WHERE HSCGRecordID='" + SCTopicID + "'");

        int gThumbsUp = 0;
        int gLove = 0;
        int gSmile = 0;

        while (QueryFeeling.Read())
        {
            if (QueryFeeling["HType"].ToString() == "1")
            {
                gThumbsUp++;
            }
            else if (QueryFeeling["HType"].ToString() == "2")
            {
                gLove++;
            }
            else if (QueryFeeling["HType"].ToString() == "3")
            {
                gSmile++;
            }
        }
        QueryFeeling.Close();

        LBtn_ThumbsUp.Text = "<span class='ti-thumb-up mr-2'></span>" + gThumbsUp.ToString("N0");
        LBtn_Heart.Text = "<span class='ti-heart mr-2'></span>" + gLove.ToString("N0");
        LBtn_Smile.Text = "<span class='ti-face-smile mr-2'></span>" + gSmile.ToString("N0");

        return gThumbsUp + gLove + gSmile;
    }
    #endregion

    #region 回應數量計算
    private int ResponseCounts(string CreateDate)
    {
        //資料讀取
        SqlDataReader QueryResponse = SQLdatabase.ExecuteReader("SELECT Count(HID) AS Num FROM HSCGRMsg WHERE HCreateDT LIKE '%"+ CreateDate + "%' AND HStatus = 1 UNION ALL SELECT Count(a.HID) AS Num FROM HSCGRMsg AS a JOIN HSCGRMsgResponse AS b ON a.HID = b.HSCGRMsgID WHERE a.HCreateDT LIKE '%"+ CreateDate + "%' AND a.HStatus = 1 AND b.HStatus = 1");

        int gResponse = 0;

        while (QueryResponse.Read())
        {
            gResponse += Convert.ToInt32(QueryResponse["Num"].ToString());
        }
        QueryResponse.Close();

        //LB_MsgNum.Text = gResponse.ToString("N0");

        return gResponse;
    }
    #endregion

    #region 瀏覽數量計算
    private int ViewCounts(string SCTopicID)
    {
        //資料讀取
        SqlDataReader QueryView = SQLdatabase.ExecuteReader("SELECT HTimes FROM HSCGRecord_View WHERE HSCGRecordID='" + SCTopicID + "' AND HStatus=1 ");

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

    #region 分享數量計算
    private int ShareCounts(string SCTopicID)
    {
        //資料讀取
        SqlDataReader QueryShare = SQLdatabase.ExecuteReader("SELECT HTimes FROM HSCGRecord_Share WHERE HSCGRecordID='" + SCTopicID + "' AND HStatus=1 ");

        int gView = 0;

        while (QueryShare.Read())
        {
            gView += Convert.ToInt32(QueryShare["HTimes"].ToString());
        }
        QueryShare.Close();

        //LB_ViewNum.Text = gView.ToString("N0");

        return gView;
    }
    #endregion

}