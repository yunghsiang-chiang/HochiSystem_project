using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Util;

public partial class HSCDetail : System.Web.UI.Page
{

    SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);
    string Con = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;

    #region --根目錄--
    string SCFileRoot = "D:\\Website\\System\\HochiSystem\\uploads\\SpecialColumn\\";
    string SCFile = "~/uploads/SpecialColumn/";
    #endregion


    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        //當沒有管理的討論則整個區塊不顯示
        if (Rpt_HSCForumCLass.Items.Count == 0)
        {
            Div_SCForumCLass.Visible = false;
        }
        else
        {
            Div_SCForumCLass.Visible = true;
        }
    }




    protected void Page_Load(object sender, EventArgs e)
    {
        //隱藏master的熱門標籤
        ((HtmlControl)Master.FindControl("Div_Right")).Visible = false;



        LBtn_Topic.Attributes.Add("class", "nav-link active show");
        LBtn_Response.Attributes.Add("class", "nav-link");


        string HUserHID = null;
        if (!IsPostBack)
        {
            SqlDataReader QueryML = SQLdatabase.ExecuteReader("SELECT HID, (HSystemName+' '+ HArea+'/'+HPeriod) AS MDescribe, HUserName FROM MemberList WHERE HID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");

            if (QueryML.Read())
            {
                LB_MDescribe.Text = QueryML["MDescribe"].ToString();
                LB_HUserName.Text = QueryML["HUserName"].ToString();
                HUserHID = QueryML["HID"].ToString();
            }
            QueryML.Close();

            LoginStatus(1, HUserHID);  //登入中

            SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  HTopicName, HPinTop,HContent, HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, a.HCreateDT, a.HModifyDT, d.HCTemplateID FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID  JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 AND a.HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' ORDER BY a.HPinTop DESC, a.HModifyDT DESC, a.HCreateDT DESC";
            Rpt_HSCTopic.DataBind();


            SDS_HSCForumClass.SelectCommand = "SELECT a.HSCForumClassID, a.HMemberID, b.HSCFCName FROM HSCModeratorSetting AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID = b.HID WHERE a.HMemberID LIKE '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%' AND a.HStatus=1 AND b.HStatus=1";
            Rpt_HSCForumCLass.DataBind();
        }







    }

    protected void LBtn_Topic_Click(object sender, EventArgs e)
    {
        LBtn_Topic.Attributes.Add("class", "nav-link active show");
        LBtn_Response.Attributes.Add("class", "nav-link");

        //Panel_Topic.Attributes.Add("class", "tab-pane  fade show active");
        Panel_Topic.Visible = true;



        SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  HTopicName, HPinTop,HContent, HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, a.HCreateDT, a.HModifyDT, d.HCTemplateID FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID  JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 AND a.HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'  ORDER BY IIF(a.HModifyDT is null,a.HCreateDT, a.HModifyDT ) DESC";
        Rpt_HSCTopic.DataBind();

    }

    protected void LBtn_Response_Click(object sender, EventArgs e)
    {
        LBtn_Response.Attributes.Add("class", "nav-link active show");
        LBtn_Topic.Attributes.Add("class", "nav-link");

        //Panel_Topic.Attributes.Add("class", "tab-pane fade");
        Panel_Topic.Visible = true;


        SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  a.HTopicName, a.HContent, a.HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, a.HCreateDT, a.HModifyDT, d.HCTemplateID FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID  JOIN MemberList AS e ON a.HCreate = e.HID LEFT JOIN HSCTMsg AS f ON f.HSCTopicID=a.HID LEFT JOIN HSCTMsgResponse AS g ON f.HID=g.HSCTMsgID WHERE a.HStatus=1 AND (f.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' OR g.HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "') GROUP BY a.HID,  c.HSCFCName, b.HSCFCName, a.HTopicName, a.HContent, a.HFile1, a.HCreateDT, a.HModifyDT, d.HCTemplateID, e.HSystemName,e.HArea,e.HPeriod,e.HUserName ORDER BY IIF(a.HModifyDT is null,a.HCreateDT, a.HModifyDT ) DESC";
        Rpt_HSCTopic.DataBind();

    }

    #region 查看更多
    protected void LBtn_View_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_View = sender as LinkButton;
        string gView_CA = LBtn_View.CommandArgument;

        //紀錄瀏覽次數
        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HSCTopicID, HMemberID, HTimes FROM HSCTopic_View WHERE HSCTopicID='" + gView_CA + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        if (dr.Read())
        {
            SqlCommand cmd = new SqlCommand("UPDATE HSCTopic_View SET HTimes=@HTimes, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCTopicID=@HSCTopicID AND HMemberID=@HMemberID", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCTopicID", gView_CA);
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
            SqlCommand cmd = new SqlCommand("INSERT INTO HSCTopic_View (HSCTopicID, HMemberID, HTimes, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HTimes, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCTopicID", gView_CA);
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
        Response.Redirect("HSCTopicDetail.aspx?TID=" + gView_CA);

    }
    #endregion


    #region 分享連結
    protected void LBtn_Share_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Share = sender as LinkButton;
        string gShare_CA = LBtn_Share.CommandArgument;

        string gShareLink = "HSCTopicDetail.aspx?TID=" + gShare_CA;

        //紀錄分享次數
        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HSCTopicID, HMemberID, HTimes FROM HSCTopic_Share WHERE HSCTopicID='" + gShare_CA + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        if (dr.Read())
        {
            SqlCommand cmd = new SqlCommand("UPDATE HSCTopic_Share SET HTimes=@HTimes, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCTopicID=@HSCTopicID AND HMemberID=@HMemberID", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCTopicID", gShare_CA);
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
            SqlCommand cmd = new SqlCommand("INSERT INTO HSCTopic_Share (HSCTopicID, HMemberID, HTimes, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HTimes, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCTopicID", gShare_CA);
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

        ////導頁
        //Response.Redirect("HSCTopicDetail.aspx?TID=" + gShare_CA);

    }
    #endregion






    #region 心情項目(按讚、愛心、笑臉)
    protected void LBtn_Feeling_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Feeling = sender as LinkButton;
        string gFeeling_CA = LBtn_Feeling.CommandArgument;
        int gFeeling_CN = Convert.ToInt32(LBtn_Feeling.CommandName);

        //判斷若已經有點開某一個，則要隱藏
        for (int i = 0; i < Rpt_HSCTopic.Items.Count; i++)
        {
            if (i != gFeeling_CN)
            {
                ((HtmlControl)Rpt_HSCTopic.Items[i].FindControl("Div_FeelingArea")).Visible = false;
            }
        }

        if (((HtmlControl)Rpt_HSCTopic.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible == false)
        {
            ((HtmlControl)Rpt_HSCTopic.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible = true;
        }
        else
        {
            ((HtmlControl)Rpt_HSCTopic.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible = false;
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

        SqlDataReader QueryFeeling = SQLdatabase.ExecuteReader("SELECT HID, HSCTopicID, HMemberID, HType FROM HSCTopic_Mood WHERE HSCTopicID='" + gFeelingType_CA + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        if (QueryFeeling.Read())
        {
            if (btn.TabIndex.ToString() != QueryFeeling["HType"].ToString())
            {
                SqlCommand cmd = new SqlCommand("UPDATE [HSCTopic_Mood] SET HType=@HType, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCTopicID=@HSCTopicID AND HMemberID=@HMemberID", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HSCTopicID", gFeelingType_CA);
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

            cmd.Parameters.AddWithValue("@HSCTopicID", gFeelingType_CA);
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
        ((HtmlControl)Rpt_HSCTopic.Items[gFeelingType_CN].FindControl("Div_FeelingArea")).Visible = false;




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

            SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_SCTopicID.Text + "' AND a.HType='1' AND a.HStatus=1";
            Rpt_HSCTopicMood.DataBind();
            //計算數量
            //FeelingCounts(LB_SCTopicID.Text);
        }
        else if (btn.TabIndex.ToString() == "2")
        {
            LBtn_ThumbsUp.CssClass = "nav-link font-weight-bold";
            LBtn_Heart.CssClass = "nav-link  active show font-weight-bold";
            LBtn_Smile.CssClass = "nav-link font-weight-bold";

            SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_SCTopicID.Text + "' AND a.HType='2' AND a.HStatus=1";

            Rpt_HSCTopicMood.DataBind();
            //計算數量
            //FeelingCounts(LB_SCTopicID.Text);
        }
        else if (btn.TabIndex.ToString() == "3")
        {
            LBtn_ThumbsUp.CssClass = "nav-link font-weight-bold";
            LBtn_Heart.CssClass = "nav-link font-weight-bold";
            LBtn_Smile.CssClass = "nav-link  active show font-weight-bold";

            SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_SCTopicID.Text + "' AND a.HType='3' AND a.HStatus=1";

            Rpt_HSCTopicMood.DataBind();
            //計算數量
            //FeelingCounts(LB_SCTopicID.Text);

        }


        //資料讀取
        SqlDataReader QueryFeeling = SQLdatabase.ExecuteReader("SELECT HMemberID, HType FROM HSCTopic_Mood WHERE HSCTopicID='" + LB_SCTopicID.Text + "'");

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

        //<未完成>
        LBtn_ThumbsUp.Text = "<span class='ti-thumb-up mr-2'></span>" + gThumbsUp.ToString();
        LB_LoveNum.Text = "<span class='ti-heart-up mr-2'></span>" + gLove.ToString();
        LB_SmileNum.Text = "<span class='ti-face-smile mr-2'></span>" + gSmile.ToString();




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

        SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + gMood_CA + "' AND a.HType='1' AND a.HStatus=1";
        Rpt_HSCTopic.DataBind();

        //計算數量
        FeelingCounts(gMood_CA);
    }
    #endregion

    #region 心情紀錄(Modal資料繫結)
    protected void Rpt_HSCTopicMood_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        UserImgShow(gDRV["HImg"].ToString(), ((Image)e.Item.FindControl("Img_HImg")));
    }
    #endregion











    #region 發表主題
    protected void Btn_Launch_Click(object sender, EventArgs e)
    {
        #region 必填判斷
        if (string.IsNullOrEmpty(TB_HSCTopicName.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請輸入主題名稱~');", true);
            return;
        }

        if (DDL_HSCForumClassA.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區主類別~');", true);
            return;
        }

        if (DDL_HSCForumClassB.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區次類別~');", true);
            return;
        }

        if (DDL_HSCForumClassC.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區名稱~');", true);
            return;
        }
        #endregion

        SqlCommand cmd = new SqlCommand("INSERT INTO HSCTopic ( HSCForumClassID, HTopicName, HPinTop, HSCClassID, HSCRecordTypeID, HSCJiugonggeTypeID, HGProgress, HOGProgress, HContent, HFile1, HFile2, HFile3, HVideoLink, HHashTag, HStatus, HCreate, HCreateDT)VALUES(@HSCForumClassID, @HTopicName, @HPinTop, @HSCClassID, @HSCRecordTypeID, @HSCJiugonggeTypeID, @HGProgress, @HOGProgress, @HContent, @HFile1, @HFile2, @HFile3, @HVideoLink, @HHashTag, @HStatus, @HCreate, @HCreateDT)", ConStr);

        ConStr.Open();
        cmd.Parameters.AddWithValue("@HSCForumClassID", DDL_HSCForumClassC.SelectedValue);
        cmd.Parameters.AddWithValue("@HTopicName", TB_HSCTopicName.Text.Trim());
        cmd.Parameters.AddWithValue("@HPinTop", RBtn_HPinTop.SelectedValue == "1" ? "TRUE" : "FALSE");
        cmd.Parameters.AddWithValue("@HSCClassID", DDL_HSCClass.SelectedValue);
        cmd.Parameters.AddWithValue("@HSCRecordTypeID", DDL_HSCRecordType.SelectedValue);
        cmd.Parameters.AddWithValue("@HSCJiugonggeTypeID", RBL_HSCJiugonggeTypeM.SelectedValue);
        cmd.Parameters.AddWithValue("@HGProgress", DDL_HGProgress.SelectedValue);
        cmd.Parameters.AddWithValue("@HOGProgress", TB_HOGProgress.Text.Trim());
        cmd.Parameters.AddWithValue("@HContent", CKE_HContent.Text.Trim());
        cmd.Parameters.AddWithValue("@HFile1", LB_File1.Text);
        cmd.Parameters.AddWithValue("@HFile2", LB_File2.Text);
        cmd.Parameters.AddWithValue("@HFile3", LB_File3.Text);
        cmd.Parameters.AddWithValue("@HVideoLink", TB_HVideoLink.Text.Trim());
        cmd.Parameters.AddWithValue("@HHashTag", TB_HHashTag.Text.Trim());
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();


        #region 清空資料
        TB_HSCTopicName.Text = null;
        DDL_HSCForumClassA.SelectedValue = "0";
        DDL_HSCForumClassB.SelectedValue = "0";
        DDL_HSCForumClassC.SelectedValue = "0";
        RBtn_HPinTop.SelectedValue = null;
        DDL_HSCClass.SelectedValue = null;
        DDL_HSCRecordType.SelectedValue = null;
        RBL_HSCJiugonggeTypeM.SelectedValue = null;
        DDL_HGProgress.SelectedValue = "0";
        TB_HOGProgress.Text = null;
        CKE_HContent.Text = null;
        LB_File1.Text = null;
        LB_File2.Text = null;
        LB_File3.Text = null;
        TB_HVideoLink.Text = null;
        TB_HHashTag.Text = null;
        #endregion

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('已發表主題~');", true);

        SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  HTopicName, HPinTop,HContent, HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, a.HCreateDT, a.HModifyDT, d.HCTemplateID FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1  ORDER BY IIF(a.HModifyDT is null,a.HCreateDT, a.HModifyDT ) DESC";




    }
    #endregion

    #region 關閉主題
    protected void Btn_Close_Click(object sender, EventArgs e)
    {

    }
    #endregion

    #region 上傳照片 (.jpg、.png、.gif、.mp3)
    protected void UploadFile()
    {








    }
    #endregion

    #region 發表主題 上傳檔案
    protected void LBtn_HFile1Upload_Click(object sender, EventArgs e)
    {
        //建立資料夾
        if (!Directory.Exists(SCFileRoot))
        {
            Directory.CreateDirectory(SCFileRoot);
        }
        if (FU_HFile1.HasFile)
        {
            //上傳檔是否大於10M
            if (FU_HFile1.PostedFile.ContentLength > (5 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過5MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_HFile1.FileName);
                //檔名
                LB_File1.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;

                this.FU_HFile1.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_File1.Text));


            }
        }


    }


    protected void LBtn_HFile2Upload_Click(object sender, EventArgs e)
    {
        //建立資料夾
        if (!Directory.Exists(SCFileRoot))
        {
            Directory.CreateDirectory(SCFileRoot);
        }
        if (FU_HFile2.HasFile)
        {
            //上傳檔是否大於10M
            if (FU_HFile2.PostedFile.ContentLength > (5 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過5MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_HFile2.FileName);
                //檔名
                LB_File2.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;

                this.FU_HFile2.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_File2.Text));


            }
        }
    }

    protected void LBtn_HFile3Upload_Click(object sender, EventArgs e)
    {
        //建立資料夾
        if (!Directory.Exists(SCFileRoot))
        {
            Directory.CreateDirectory(SCFileRoot);
        }
        if (FU_HFile3.HasFile)
        {
            //上傳檔是否大於10M
            if (FU_HFile3.PostedFile.ContentLength > (5 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過5MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_HFile3.FileName);
                //檔名
                LB_File3.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;

                this.FU_HFile3.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_File3.Text));


            }
        }
    }



    protected void LBtn_HFileM1Upload_Click(object sender, EventArgs e)
    {

    }

    protected void LBtn_HFileM2Upload_Click(object sender, EventArgs e)
    {

    }

    protected void LBtn_HFileM3Upload_Click(object sender, EventArgs e)
    {

    }
    #endregion

    #region 更多選項(編輯、刪除、隱藏)
    protected void LBtn_More_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_More = sender as LinkButton;
        string gMore_CA = LBtn_More.CommandArgument;
        int gMore_CN = Convert.ToInt32(LBtn_More.CommandName);

        if (((HtmlControl)Rpt_HSCTopic.Items[gMore_CN].FindControl("Div_Editarea")).Visible == false)
        {
            ((HtmlControl)Rpt_HSCTopic.Items[gMore_CN].FindControl("Div_Editarea")).Visible = true;
        }
        else
        {
            ((HtmlControl)Rpt_HSCTopic.Items[gMore_CN].FindControl("Div_Editarea")).Visible = false;
        }






    }
    #endregion

    #region 編輯主題(打開Modal)
    protected void LBtn_Edit_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Edit = sender as LinkButton;
        string gEdit_CA = LBtn_Edit.CommandArgument;
        int gEdit_CN = Convert.ToInt32(LBtn_Edit.CommandName);

        ((HtmlControl)Rpt_HSCTopic.Items[gEdit_CN].FindControl("Div_Editarea")).Visible = false;

        #region 判斷權限<待改成function>
        //本人或討論區的管理者才可以做隱藏
        SqlDataReader QueryEditor = SQLdatabase.ExecuteReader("SELECT a.HID, a.HSCForumClassID FROM HSCTopic AS a LEFT JOIN HSCModeratorSetting AS b ON a.HSCForumClassID=b.HSCForumClassID WHERE a.HID='" + gEdit_CA + "' AND (a.HCreate = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' OR b.HMemberID LIKE '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%')");

        if (QueryEditor.Read())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Edit').modal('show');</script>", false);//執行js
            LB_EditHID.Text = gEdit_CA;
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "Notify", "<script>alert('你沒有權限編輯主題哦~');</script>", false);//執行js
            return;
        }
        QueryEditor.Close();
        #endregion

    }
    #endregion

    #region 確認儲存編輯主題功能
    //主題狀態：dbo.HSCTopic.HStatus (0:刪除/1:正常/2:隱藏)
    //主題Log類型：dbo.HSCTopic_Log.HLogType (0:刪除/1:編輯/2:隱藏)
    protected void Btn_UPDSubmit_Click(object sender, EventArgs e)
    {
        #region 必填判斷
        if (string.IsNullOrEmpty(TB_HSCTopicNameM.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請輸入主題名稱~');", true);
            return;
        }

        if (DDL_HSCForumClassAM.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區主類別~');", true);
            return;
        }

        if (DDL_HSCForumClassBM.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區次類別~');", true);
            return;
        }

        if (DDL_HSCForumClassCM.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區名稱~');", true);
            return;
        }
        #endregion


        //更新資料庫
        using (SqlConnection con = new SqlConnection(Con))
        {
            SqlCommand cmd = new SqlCommand("UPDATE HSCTopic SET HSCForumClassID=@HSCForumClassID, HTopicName=@HTopicName, HPinTop=@HPinTop, HSCClassID=@HSCClassID, HSCRecordTypeID=@HSCRecordTypeID, HSCJiugonggeTypeID=@HSCJiugonggeTypeID, HGProgress=@HGProgress, HOGProgress=@HOGProgress, HContent=@HContent, HFile1=@HFile1, HFile2=@HFile2, HFile3=@HFile3, HVideoLink=@HVideoLink, HHashTag=@HHashTag, HStatus=@HStatus, HModify=@HModify,HModifyDT=@HModifyDT WHERE HID=@HID", con);
            con.Open();
            cmd.Parameters.AddWithValue("@HID", Convert.ToInt32(LB_EditHID.Text));
            cmd.Parameters.AddWithValue("@HSCForumClassID", DDL_HSCForumClassCM.SelectedValue);
            cmd.Parameters.AddWithValue("@HTopicName", TB_HSCTopicNameM.Text.Trim());
            cmd.Parameters.AddWithValue("@HPinTop", RBtn_HPinTopM.SelectedValue == "1" ? "TRUE" : "FALSE");
            cmd.Parameters.AddWithValue("@HSCClassID", DDL_HSCClassM.SelectedValue);
            cmd.Parameters.AddWithValue("@HSCRecordTypeID", DDL_HSCRecordTypeM.SelectedValue);
            cmd.Parameters.AddWithValue("@HSCJiugonggeTypeID", RBL_HSCJiugonggeTypeM.SelectedValue);
            cmd.Parameters.AddWithValue("@HGProgress", DDL_HGProgressM.SelectedValue);
            cmd.Parameters.AddWithValue("@HOGProgress", TB_HOGProgressM.Text.Trim());
            cmd.Parameters.AddWithValue("@HContent", CKE_HContentM.Text.Trim());
            cmd.Parameters.AddWithValue("@HFile1", LB_FileM1.Text);
            cmd.Parameters.AddWithValue("@HFile2", LB_FileM2.Text);
            cmd.Parameters.AddWithValue("@HFile3", LB_FileM3.Text);
            cmd.Parameters.AddWithValue("@HVideoLink", TB_HVideoLinkM.Text.Trim());
            cmd.Parameters.AddWithValue("@HHashTag", TB_HHashTagM.Text.Trim());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            cmd.Cancel();
        }

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO HSCTopic_Log (HSCTopicID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCTopicID", LB_EditHID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 1);
        logcmd.Parameters.AddWithValue("@HReason", "");
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('主題內容修改完成~');", true);

        Rpt_HSCTopic.DataBind();


    }
    #endregion

    #region 刪除主題(打開Modal)
    protected void LBtn_Del_Click(object sender, EventArgs e)
    {




        LinkButton LBtn_Del = sender as LinkButton;
        string gDel_CA = LBtn_Del.CommandArgument;
        int gDel_CN = Convert.ToInt32(LBtn_Del.CommandName);

        ((HtmlControl)Rpt_HSCTopic.Items[gDel_CN].FindControl("Div_Editarea")).Visible = false;

        #region 判斷權限<待改成function>
        //本人或討論區的管理者才可以做刪除
        SqlDataReader QueryEditor = SQLdatabase.ExecuteReader("SELECT a.HID, a.HSCForumClassID FROM HSCTopic AS a LEFT JOIN HSCModeratorSetting AS b ON a.HSCForumClassID=b.HSCForumClassID WHERE a.HID='" + gDel_CA + "' AND (a.HCreate = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' OR b.HMemberID LIKE '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%')");

        if (QueryEditor.Read())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_TopicReviseModal').modal('show');</script>", false);//執行js
            Div_SCTopicDel.Visible = true;
            Div_SCTopicHide.Visible = false;

            LB_ReviseHID.Text = gDel_CA;
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "Notify", "<script>alert('你沒有權限刪除主題哦~');</script>", false);//執行js
            return;
        }
        QueryEditor.Close();
        #endregion

    }
    #endregion

    #region 確認刪除主題功能
    //主題狀態：dbo.HSCTopic.HStatus (0:刪除/1:正常/2:隱藏)
    //主題Log類型：dbo.HSCTopic_Log.HLogType (0:刪除/1:編輯/2:隱藏)
    protected void Btn_HSCTopicDel_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_HSCTopicDelReason.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入刪除主題原因');", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_TopicReviseModal').modal();"), true);
            return;
        }

        //變更主題狀態
        SqlCommand cmd = new SqlCommand("UPDATE HSCTopic SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_ReviseHID.Text);
        cmd.Parameters.AddWithValue("@HStatus", 0);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO HSCTopic_Log (HSCTopicID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCTopicID", LB_ReviseHID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 0);
        logcmd.Parameters.AddWithValue("@HReason", TB_HSCTopicDelReason.Text.Trim());
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('刪除成功!');", true);

        SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  HTopicName, HPinTop,HContent, HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, a.HCreateDT, a.HModifyDT, d.HCTemplateID FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID  JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 AND a.HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' ORDER BY a.HModifyDT DESC, a.HCreateDT DESC";
        Rpt_HSCTopic.DataBind();
    }
    #endregion

    #region 隱藏主題(打開Modal)
    protected void LBtn_Hide_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Hide = sender as LinkButton;
        string gHide_CA = LBtn_Hide.CommandArgument;
        int gHide_CN = Convert.ToInt32(LBtn_Hide.CommandName);

        ((HtmlControl)Rpt_HSCTopic.Items[gHide_CN].FindControl("Div_Editarea")).Visible = false;

        #region 判斷權限<待改成function>
        //本人或討論區的管理者才可以做隱藏
        SqlDataReader QueryEditor = SQLdatabase.ExecuteReader("SELECT a.HID, a.HSCForumClassID FROM HSCTopic AS a LEFT JOIN HSCModeratorSetting AS b ON a.HSCForumClassID=b.HSCForumClassID WHERE a.HID='" + gHide_CA + "' AND (a.HCreate = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' OR b.HMemberID LIKE '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%')");

        if (QueryEditor.Read())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_TopicReviseModal').modal('show');</script>", false);//執行js
            Div_SCTopicDel.Visible = false;
            Div_SCTopicHide.Visible = true;

            LB_ReviseHID.Text = gHide_CA;
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "Notify", "<script>alert('你沒有權限隱藏主題哦~');</script>", false);//執行js
            return;
        }
        QueryEditor.Close();
        #endregion



    }
    #endregion

    #region 確認隱藏主題功能
    //主題狀態：dbo.HSCTopic.HStatus (0:刪除/1:正常/2:隱藏)
    //主題Log類型：dbo.HSCTopic_Log.HLogType (0:刪除/1:編輯/2:隱藏)
    protected void Btn_HSCTopicHide_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_HSCTopicHideReason.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入隱藏主題原因');", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_TopicReviseModal').modal();"), true);
            return;
        }

        //變更主題狀態
        SqlCommand cmd = new SqlCommand("UPDATE HSCTopic SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_ReviseHID.Text);
        cmd.Parameters.AddWithValue("@HStatus", 2);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO HSCTopic_Log (HSCTopicID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCTopicID", LB_ReviseHID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 2);
        logcmd.Parameters.AddWithValue("@HReason", TB_HSCTopicHideReason.Text.Trim());
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('隱藏成功!');", true);

        SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  HTopicName, HPinTop,HContent, HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, a.HCreateDT, a.HModifyDT, d.HCTemplateID FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID  JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 AND a.HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' ORDER BY  a.HModifyDT DESC, a.HCreateDT DESC";
        Rpt_HSCTopic.DataBind();
    }
    #endregion


    #region 更新主題取消功能
    protected void Btn_UPDCancel_Click(object sender, EventArgs e)
    {

    }
    #endregion

    #region 資料繫結
    protected void Rpt_HSCTopic_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        

        #region 數量計算
        //留言與回應
        //ResponseCounts(((Label)e.Item.FindControl("LB_HID")).Text);
        ((Label)e.Item.FindControl("LB_MsgNum")).Text = (ResponseCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString("N0");

        //心情
        FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text);
        ((Label)e.Item.FindControl("LB_FeelingNum")).Text = (FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString("N0");

        //瀏覽次數
        ((Label)e.Item.FindControl("LB_ViewNum")).Text = (ViewCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString("N0");
        #endregion
    }


    #endregion




    #region 主類別選擇後變化
    protected void DDL_HSCForumClassA_SelectedIndexChanged(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#ContentModal').modal();"), true);
        SDS_HSCForumClassB.SelectCommand = "SELECT a.HID, a.HSCFCName, a.HSCFCLevel, a.HStatus, b.HSCFCName AS HSCForumMaster FROM HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON b.HID=a.HSCFCMaster WHERE a.HStatus = 1 AND a.HSCFCLevel = '20' AND a.HSCFCMaster ='" + DDL_HSCForumClassA.SelectedValue + "'";

        DDL_HSCForumClassBM.DataBind();

    }
    #endregion

    #region 所屬討論區次類別選擇後變化
    protected void DDL_HSCForumClassB_SelectedIndexChanged(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#ContentModal').modal();"), true);
        SDS_HSCForumClassC.SelectCommand = "SELECT a.HID, a.HSCFCName, a.HSCFCLevel, a.HStatus, b.HSCFCName AS HSCForumMaster FROM HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON b.HID=a.HSCFCMaster WHERE a.HStatus = 1 AND a.HSCFCLevel = '30' AND a.HSCFCMaster ='" + DDL_HSCForumClassB.SelectedValue + "'";

        DDL_HSCForumClassC.DataBind();
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
        System.Data.DataTable dtFeeling = SQLdatabase.ExecuteDataTable("SELECT HMemberID, HType FROM HSCTopic_Mood WHERE HSCTopicID='" + SCTopicID + "'");
        int gThumbsUp = 0;
        int gLove = 0;
        int gSmile = 0;
        foreach (DataRow datarow in dtFeeling.Rows)
        {
            if (datarow["HType"].ToString() == "1")
            {
                gThumbsUp++;
            }
            else if (datarow["HType"].ToString() == "2")
            {
                gLove++;
            }
            else if (datarow["HType"].ToString() == "3")
            {
                gSmile++;
            }
        }


        LBtn_ThumbsUp.Text = "<span class='ti-thumb-up mr-2'></span>" + gThumbsUp.ToString("N0");
        LBtn_Heart.Text = "<span class='ti-heart mr-2'></span>" + gLove.ToString("N0");
        LBtn_Smile.Text = "<span class='ti-face-smile mr-2'></span>" + gSmile.ToString("N0");


        return gThumbsUp + gLove + gSmile;
    }
    #endregion


    #region 回應數量計算
    private int ResponseCounts(string SCTopicID)
    {
      



        System.Data.DataTable dtResponse = SQLdatabase.ExecuteDataTable("SELECT Count(HID) AS Num FROM HSCTMsg  WHERE HSCTopicID='" + SCTopicID + "' AND HStatus=1 UNION ALL SELECT Count(a.HID) AS Num FROM HSCTMsg AS a  JOIN HSCTMsgResponse AS b ON a.HID= b.HSCTMsgID WHERE a.HSCTopicID='" + SCTopicID + "' AND a.HStatus=1 ");

        int gResponse = 0;
        foreach (DataRow datarow in dtResponse.Rows)
        {
            gResponse += Convert.ToInt32(datarow["Num"].ToString());
        }

        return gResponse;


    }
    #endregion


    #region 瀏覽數量計算
    private int ViewCounts(string SCTopicID)
    {
       


        System.Data.DataTable dtView = SQLdatabase.ExecuteDataTable("SELECT HTimes, IIF(HModifyDT IS NUll, HCreateDT,HModifyDT) AS Date FROM HSCTopic_View WHERE HSCTopicID='" + SCTopicID + "' AND HStatus=1 ORDER BY IIF(HModifyDT IS NUll, HCreateDT,HModifyDT ) DESC");

        int gView = 0;
        foreach (DataRow datarow in dtView.Rows)
        {
            gView += Convert.ToInt32(datarow["HTimes"].ToString());
        }

        return gView;
    }
    #endregion


    protected void Rpt_HSCForumCLass_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        ((HyperLink)e.Item.FindControl("HL_HSCFCLassName")).NavigateUrl = "HSCModerator.aspx?FClassCID=" + gDRV["HSCForumClassID"].ToString();


    }


    /// <summary>
    /// 執行登入狀態的按鈕顯示
    /// </summary>
    /// <param name="Status">1：登入狀態，0：登出狀態</param>
    protected void LoginStatus(int Status, string HUserHID)
    {
        if (Status == 1)
        {
            //後台專欄設定功能的權限(NA=課程討論區與主題設定、NB=版規範本、NC=討論區管理員設定、ND=專欄相關參數設定、專欄報表分析(尚未完成功能暫沒加入權限))
            if (!string.IsNullOrEmpty(BackPermissionItem(HUserHID)) && BackPermissionItem(HUserHID).ToString() != ",,,")
            {
                Div_EDUBackEnd.Visible = true;

                string[] gPermissionItem = BackPermissionItem(HUserHID).Split(',');

                if (gPermissionItem[0] == "1")
                {
                    HL_HSCCourseSetting.Visible = true;
                }
                if (gPermissionItem[1] == "1")
                {
                    HL_HSCRuleTemplate.Visible = true;
                }
                if (gPermissionItem[2] == "1")
                {
                    HL_HSCModeratorSetting.Visible = true;
                }
                if (gPermissionItem[3] == "1")
                {
                    HL_HSCParameter.Visible = true;
                }
            }
            else
            {
                Div_EDUBackEnd.Visible = false;
            }

        }
        else
        {
            LoginStatus(0, "");  //登出
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

        string gIntNA = "";
        string gIntNB = "";
        string gIntNC = "";
        string gIntND = "";

        while (QueryUsualTask.Read())
        {
            //後台專欄設定功能的權限(NA=課程討論區與主題設定、NB=版規範本、NC=討論區管理員設定、ND=專欄相關參數設定、專欄報表分析(尚未完成功能暫沒加入權限))
            if (QueryUsualTask["HRAccess"].ToString().Contains("NA"))
            {
                gIntNA = "1";
            }
            if (QueryUsualTask["HRAccess"].ToString().Contains("NB"))
            {
                gIntNB = "1";
            }
            if (QueryUsualTask["HRAccess"].ToString().Contains("NC"))
            {
                gIntNC = "1";
            }
            if (QueryUsualTask["HRAccess"].ToString().Contains("ND"))
            {
                gIntND = "1";
            }

        }
        QueryUsualTask.Close();


        Access = gIntNA + "," + gIntNB + "," + gIntNC + "," + gIntND;

        return Access;
    }
}