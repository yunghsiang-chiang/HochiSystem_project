using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class HSCModerator : System.Web.UI.Page
{
    SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    /**備註**/
    //公開設定：0=不公開 / 1=公開

    string HMemberStr = "SELECT a.HID, a.HArea, a.HAreaID, a.HTeamID, a.HSystemName, a.HSystemID, a.HPeriod, a.HUserName FROM MemberList as a join (SELECT  A.HMemberID, A.HCourseID FROM HCourseBooking AS A INNER JOIN HCourse AS B On A.HCourseID = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND (A.HChangeStatus IS NULL OR A.HChangeStatus = '2') UNION SELECT A.HMemberID, D.HCourseIDNew AS HCourseID FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON A.HOrderGroup = D.HOrderGroupNew INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND A.HCourseName = '換課' AND(D.HChangeStatus IS NULL OR D.HChangeStatus = '2') UNION SELECT A.HMemberID, D.HCourseIDNew AS HCourseID FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON(A.HOrderGroup = D.HOrderGroupSrc AND A.HOrderNum = D.HOrderNumSrc) OR(A.HOrderGroup = D.HOrderGroupSrc AND D.HOrderNumSrc = '') INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND(D.HOrderGroupNew = '' OR D.HOrderGroupNew IS NULL) UNION SELECT  A.HMemberID, D.HCourseIDNew AS HCourseID FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON(A.HOrderGroup = D.HOrderGroupSrc) LEFT JOIN HCBChangeRecord AS E ON(D.HOrderGroupSrc = E.HOrderGroupNew) INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND A.HCourseName = '換課' AND(D.HOrderGroupNew = '' OR D.HOrderGroupNew IS NULL) AND(E.HChangeStatus = '1') AND(A.HChangeStatus IS NULL OR A.HChangeStatus = '2')) as b on a.HID = b.HMemberID ";

    string HSCTopicMemberStr = "SELECT a.HID, a.HSCForumClassID, a.HSCTopicID, a.HMemberID, a.HStatus, b.HArea, b.HAreaID, b.HTeamID, b.HSystemName, b.HSystemID, b.HPeriod, b.HUserName FROM HSCTopic_Role AS a INNER JOIN MemberList AS b ON a.HMemberID=b.HID ";

    string HSCForumClassID = "0";//討論區名稱HID

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        #region 總人數計算

        for (int i = 0; i < Rpt_HSCTopic.Items.Count; i++)
        {
            int MemberNum = 0;

            SqlDataReader HMemberdr = SQLdatabase.ExecuteReader("SELECT HID FROM HSCTopic_Role WHERE HSCForumClassID='" + HSCForumClassID + "' AND HSCTopicID='" + ((Label)Rpt_HSCTopic.Items[i].FindControl("LB_HID")).Text + "'");

            while (HMemberdr.Read())
            {
                MemberNum += 1;
            }
            HMemberdr.Close();

            ((Label)Rpt_HSCTopic.Items[i].FindControl("LB_MemberNum")).Text = MemberNum.ToString("N0");

        }

        #endregion
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        JS_Register();
        //AA20240720_隱藏master左側功能
        ((HtmlControl)Master.FindControl("Div_Left")).Visible = false;

        if (Request.QueryString["FClassCID"] != null)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["FClassCID"].ToString()))
            {
                HSCForumClassID = Request.QueryString["FClassCID"].ToString();

                #region 討論區權限判斷

                SqlDataReader strSQLdr = SQLdatabase.ExecuteReader("SELECT HID, HSCForumClassID, HMemberID FROM HSCModeratorSetting WHERE HSCForumClassID='" + HSCForumClassID + "' AND (HMemberID LIKE '%" + "," + ((Label)Master.FindControl("LB_HUserHID")).Text + "," + "%')");

                if (!strSQLdr.Read())
                {
                    Response.Write("<script>alert('您沒有權限喔!');window.location.href='HSCDetail.aspx';</script>");
                }
                strSQLdr.Close();

                #endregion

                SqlDataReader HSCFCNamedr = SQLdatabase.ExecuteReader("SELECT HID, HSCFCMaster, HSCFCName, HSCFCLevel FROM HSCForumClass WHERE HID='" + HSCForumClassID + "' AND HSCFCLevel=30 AND HStatus=1");

                if (HSCFCNamedr.Read())
                {
                    LB_HSCFCName.Text = HSCFCNamedr["HSCFCName"].ToString();
                }
                else
                {
                    Response.Redirect("HSCDetail.aspx");
                }
                HSCFCNamedr.Close();

                SDS_HSCTopic.SelectCommand = "SELECT HID, HCourseID, HSCForumClassID, HTopicName, HSCClassID, HSCRecordTypeID, HStatus FROM HSCTopic WHERE HSCForumClassID='" + HSCForumClassID + "' AND HStatus=1 ORDER BY HCreateDT DESC";
                SDS_HSCTopic.DataBind();
                Rpt_HSCTopic.DataBind();
            }
        }
        else
        {
            Response.Redirect("HSCDetail.aspx");
        }

        if (!IsPostBack)
        {
            if (Panel_Rule.Visible == true)
            {
                SqlDataReader MyEBF = SQLdatabase.ExecuteReader("SELECT HID, HSCMRule FROM HSCMRule WHERE HSCForumClassID='" + HSCForumClassID + "' AND HStatus=1");

                if (MyEBF.Read())
                {
                    CKE_HSCMRule.Text = MyEBF["HSCMRule"].ToString();
                }
                else
                {
                    SqlDataReader read_T = SQLdatabase.ExecuteReader("SELECT TOP (1) HID, HSCRule, HStatus FROM HSCRule_T WHERE HStatus='1' ORDER BY HStatus");

                    if (read_T.Read())
                    {
                        CKE_HSCMRule.Text = read_T["HSCRule"].ToString();
                    }
                    read_T.Close();

                }

                MyEBF.Close();
            }
        }


    }

    #region 編輯版規
    protected void LBtn_EditHSCMRule_Click(object sender, EventArgs e)
    {
        Panel_Rule.Visible = true;
        Panel_Member.Visible = false;
        Panel_Setting.Visible = false;

        LBtn_EditHSCMRule.Style.Add("color", "#896ee2");
        LBtn_MemManage.Style.Add("color", "#000");
        LBtn_PublicSetting.Style.Add("color", "#000");

        SqlDataReader MyEBF = SQLdatabase.ExecuteReader("SELECT HID, HSCMRule FROM HSCMRule WHERE HSCForumClassID='" + HSCForumClassID + "' AND HStatus=1");

        if (MyEBF.Read())
        {
            CKE_HSCMRule.Text = MyEBF["HSCMRule"].ToString();
        }
        else
        {
            SqlDataReader read_T = SQLdatabase.ExecuteReader("SELECT TOP (1) HID, HSCRule, HStatus FROM HSCRule_T WHERE HStatus='1' ORDER BY HStatus");

            if (read_T.Read())
            {
                CKE_HSCMRule.Text = read_T["HSCRule"].ToString();
            }
            read_T.Close();

        }

        MyEBF.Close();


    }

    #region 編輯版規-儲存功能
    protected void LBtn_HSCMRuleSubmit_Click(object sender, EventArgs e)
    {
        SqlDataReader MyEBF = SQLdatabase.ExecuteReader("SELECT HID FROM HSCMRule WHERE HSCForumClassID='" + HSCForumClassID + "' AND HStatus=1");

        if (MyEBF.Read())
        {
            SqlCommand cmd = new SqlCommand("UPDATE HSCMRule SET HSCMRule=@HSCMRule,HModify=@HModify,HModifyDT=@HModifyDT WHERE HSCForumClassID =@HSCForumClassID", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCForumClassID", HSCForumClassID);
            cmd.Parameters.AddWithValue("@HSCMRule", CKE_HSCMRule.Text.Trim());
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            #region 版規Log

            SqlCommand cmdlog = new SqlCommand("INSERT INTO [HSCMRule_Log] ( HSCForumClassID, HSCMRuleID, HSCMRule, HStatus, HCreate, HCreateDT)VALUES( @HSCForumClassID, @HSCMRuleID, @HSCMRule, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();

            cmdlog.Parameters.AddWithValue("@HSCForumClassID", HSCForumClassID);
            cmdlog.Parameters.AddWithValue("@HSCMRuleID", MyEBF["HID"].ToString());
            cmdlog.Parameters.AddWithValue("@HSCMRule", CKE_HSCMRule.Text.Trim());
            cmdlog.Parameters.AddWithValue("@HStatus", 1);
            cmdlog.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmdlog.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmdlog.ExecuteNonQuery();
            ConStr.Close();
            cmdlog.Cancel();

            #endregion

        }
        else
        {

            string HID = null;

            SqlCommand cmd = new SqlCommand("INSERT INTO [HSCMRule] ( HSCForumClassID, HSCMRule, HStatus, HCreate, HCreateDT)VALUES( @HSCForumClassID, @HSCMRule, @HStatus, @HCreate, @HCreateDT); SELECT SCOPE_IDENTITY() AS HID", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCForumClassID", HSCForumClassID);
            cmd.Parameters.AddWithValue("@HSCMRule", CKE_HSCMRule.Text.Trim());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            //cmd.ExecuteNonQuery();
            HID = cmd.ExecuteScalar().ToString();
            ConStr.Close();
            cmd.Cancel();

            #region 版規Log

            SqlCommand cmdlog = new SqlCommand("INSERT INTO [HSCMRule_Log] ( HSCForumClassID, HSCMRuleID, HSCMRule, HStatus, HCreate, HCreateDT)VALUES( @HSCForumClassID, @HSCMRuleID, @HSCMRule, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();

            cmdlog.Parameters.AddWithValue("@HSCForumClassID", HSCForumClassID);
            cmdlog.Parameters.AddWithValue("@HSCMRuleID", HID);
            cmdlog.Parameters.AddWithValue("@HSCMRule", CKE_HSCMRule.Text.Trim());
            cmdlog.Parameters.AddWithValue("@HStatus", 1);
            cmdlog.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmdlog.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmdlog.ExecuteNonQuery();
            ConStr.Close();
            cmdlog.Cancel();

            #endregion

        }
        MyEBF.Close();



        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('儲存成功!');", true);

    }
    #endregion

    #region 編輯版規-取消儲存功能
    protected void LBtn_HSCMRuleCancel_Click(object sender, EventArgs e)
    {
        SqlDataReader MyEBF = SQLdatabase.ExecuteReader("SELECT HID, HSCMRule FROM HSCMRule WHERE HSCForumClassID='" + HSCForumClassID + "' AND HStatus=1");

        if (MyEBF.Read())
        {
            CKE_HSCMRule.Text = MyEBF["HSCMRule"].ToString();
        }
        else
        {
            SqlDataReader read_T = SQLdatabase.ExecuteReader("SELECT TOP (1) HID, HSCRule, HStatus FROM HSCRule_T WHERE HStatus='1' ORDER BY HStatus");

            if (read_T.Read())
            {
                CKE_HSCMRule.Text = read_T["HSCRule"].ToString();
            }
            read_T.Close();

        }

        MyEBF.Close();
    }
    #endregion

    #endregion

    #region 成員管理
    protected void LBtn_MemManage_Click(object sender, EventArgs e)
    {
        Panel_Rule.Visible = false;
        //AA20240718_加入討論區裡的主題列表
        Panel_SCTopic.Visible = true;
        Panel_Member.Visible = false;
        Panel_Setting.Visible = false;

        LBtn_EditHSCMRule.Style.Add("color", "#000");
        LBtn_MemManage.Style.Add("color", "#896ee2");
        LBtn_PublicSetting.Style.Add("color", "#000");

        //AA20240804_加入JS註冊
        JS_Register();
    }

    #region 主題列表-查看成員名單
    protected void LBtn_View_Click(object sender, EventArgs e)
    {
        Panel_Rule.Visible = false;
        //AA20240718_加入討論區裡的主題列表
        Panel_SCTopic.Visible = false;
        Panel_Member.Visible = true;
        Panel_Setting.Visible = false;

        LinkButton LBtn_View = sender as LinkButton;
        string View_CA = LBtn_View.CommandArgument;

        LB_HTopicName.Text = LBtn_View.CommandName;

        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT a.HID, a.HCourseID, a.HSCForumClassID, a.HTopicName, a.HSCClassID, a.HSCRecordTypeID, a.HStatus, b.HPublic FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID WHERE a.HStatus=1 AND a.HID ='" + View_CA + "'");

        

        if (dr.Read())
        {
            LB_HSCTopicID.Text = dr["HID"].ToString();
            LB_HCourseID.Text = dr["HCourseID"].ToString();

            if (!string.IsNullOrEmpty(dr["HCourseID"].ToString()))
            {
                Tr_MemberAdd.Visible = false;

                //EA20240722_如果成員表中沒有資料就Insert
                SqlDataReader Memberdr = SQLdatabase.ExecuteReader(HMemberStr + " WHERE b.HCourseID IN (" + dr["HCourseID"].ToString().TrimStart(',').TrimEnd(',') + ")");
                while (Memberdr.Read())
                {
                    SqlDataReader HSCTMemberdr = SQLdatabase.ExecuteReader(HSCTopicMemberStr + " WHERE HSCForumClassID='" + HSCForumClassID + "' AND HSCTopicID='" + LB_HSCTopicID.Text + "' AND HMemberID ='" + Memberdr["HID"].ToString() + "'");
                    if (!HSCTMemberdr.Read())
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO [HSCTopic_Role] ( HSCForumClassID, HSCTopicID, HMemberID, HStatus, HCreate, HCreateDT)VALUES( @HSCForumClassID, @HSCTopicID, @HMemberID, @HStatus, @HCreate, @HCreateDT); SELECT SCOPE_IDENTITY() AS HID", ConStr);
                        ConStr.Open();

                        cmd.Parameters.AddWithValue("@HSCForumClassID", HSCForumClassID);
                        cmd.Parameters.AddWithValue("@HSCTopicID", LB_HSCTopicID.Text.Trim());
                        cmd.Parameters.AddWithValue("@HMemberID", Memberdr["HID"].ToString());
                        cmd.Parameters.AddWithValue("@HStatus", 1);
                        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                        cmd.ExecuteNonQuery();
                        ConStr.Close();
                        cmd.Cancel();
                    }
                    HSCTMemberdr.Close();
                }
                Memberdr.Close();
            }
            else
            {
                Tr_MemberAdd.Visible = true;
            }

            //AA20250106_若為不公開的討論區，可以自己新增
            if (dr["HPublic"].ToString() == "0")
            {
                Tr_MemberAdd.Visible = true;
            }

        }
        dr.Close();

        SDS_HMember.SelectCommand = HSCTopicMemberStr + " WHERE a.HSCForumClassID='" + HSCForumClassID + "' AND a.HSCTopicID='" + LB_HSCTopicID.Text + "'";
        SDS_HMember.DataBind();
        Rpt_HMember.DataBind();
        LB_HMemberNum.Text = Rpt_HMember.Items.Count.ToString();

    }
    #endregion

    #region 返回成員管理的主題列表
    protected void LBtn_Back_Click(object sender, EventArgs e)
    {
        Panel_Rule.Visible = false;
        //AA20240718_加入討論區裡的主題列表
        Panel_SCTopic.Visible = true;
        Panel_Member.Visible = false;
        Panel_Setting.Visible = false;

        SDS_HMember.SelectCommand = "";
        SDS_HMember.DataBind();
        Rpt_HMember.DataBind();
    }
    #endregion


    #region 主題-搜尋功能
    protected void LBtn_HSCTopicSearch_Click(object sender, EventArgs e)
    {
        StringBuilder sql = new StringBuilder("SELECT HID, HCourseID, HSCForumClassID, HTopicName, HSCClassID, HSCRecordTypeID, HStatus  FROM HSCTopic WHERE HStatus='1' AND HSCForumClassID='"+HSCForumClassID+"' ");

        List<string> WHERE = new List<string>();

        if (!string.IsNullOrEmpty(TB_HSCTopicSearch.Text.Trim()))
        {
            WHERE.Add(" (HTopicName LIKE N'%" + TB_HSCTopicSearch.Text.Trim() + "%')");
        }

        //判斷使用者是否選擇了條件
        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" AND " + wh);
        }

        SDS_HSCTopic.SelectCommand = sql.ToString()+ "  ORDER BY HCreateDT DESC";
        Rpt_HSCTopic.DataBind();
    }
    #endregion

    #region 主題-取消搜尋功能
    protected void LBtn_HSCTopicCancel_Click(object sender, EventArgs e)
    {
        TB_HSCTopicSearch.Text = null;
        SDS_HSCTopic.SelectCommand = "SELECT HID, HCourseID, HSCForumClassID, HTopicName, HSCClassID, HSCRecordTypeID, HStatus FROM HSCTopic WHERE HSCForumClassID='"+HSCForumClassID+"' HStatus=1 ORDER BY HCreateDT DESC";
        SDS_HSCTopic.DataBind();
        Rpt_HSCTopic.DataBind();

    }
    #endregion

    #region 成員-搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {

        StringBuilder sql = new StringBuilder(HSCTopicMemberStr + " WHERE a.HSCForumClassID='" + HSCForumClassID + "' AND a.HSCTopicID='" + LB_HSCTopicID.Text + "'");

        List<string> WHERE = new List<string>();


        if (!string.IsNullOrEmpty(TB_Search.Text.Trim()))
        {
            WHERE.Add(" (b.HUserName LIKE N'%" + TB_Search.Text.Trim() + "%')");
        }

        if (DDL_HArea.SelectedValue != "0")
        {
            WHERE.Add(" (b.HAreaID =" + DDL_HArea.SelectedValue + ")");
        }

        if (DDL_HSystem.SelectedValue != "0")
        {
            WHERE.Add(" (b.HSystemID=" + DDL_HSystem.SelectedValue + ")");
        }

        if (DDL_HStatus.SelectedValue != "0")
        {
            WHERE.Add(" (a.HStatus=" + DDL_HStatus.SelectedValue + ")");
        }

        //判斷使用者是否選擇了條件
        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" AND " + wh);
        }

        SDS_HMember.SelectCommand = sql.ToString();
        Rpt_HMember.DataBind();

    }
    #endregion

    #region 成員-取消搜尋功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_Search.Text = null;
        DDL_HArea.SelectedValue = "0";
        DDL_HSystem.SelectedValue = "0";
        DDL_HStatus.SelectedValue = "0";
        SDS_HMember.SelectCommand = HSCTopicMemberStr + " WHERE a.HSCForumClassID='" + HSCForumClassID + "' AND a.HSCTopicID='" + LB_HSCTopicID.Text + "'";
        Rpt_HMember.DataBind();

    }
    #endregion

    #region 成員管理列表-資料繫結
    protected void Rpt_HMember_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        string gHTeamType = "";
        string gHTeamName = "";
        string[] gHTeamID = ((Label)e.Item.FindControl("LB_HTeamID")).Text.Split(',');
        if (gHTeamID.Length > 1)
        {
            gHTeamType = gHTeamID[1] == "1" ? "HMTeam" : "HCTeam";
            gHTeamName = gHTeamID[1] == "1" ? "HMTeam" : "HCTeam";
            SqlDataReader QueryHTeam = SQLdatabase.ExecuteReader("SELECT " + gHTeamName + " FROM " + gHTeamType + " WHERE HID='" + gHTeamID[0] + "'");
            while (QueryHTeam.Read())
            {
                ((Label)e.Item.FindControl("LB_HTeamID")).Text = QueryHTeam[gHTeamName].ToString();
            }
            QueryHTeam.Close();

        }
        else
        {
            ((Label)e.Item.FindControl("LB_HTeamID")).Text = "";
        }

        #region 狀態-按鈕顯示&狀態顯示
        if (((Label)e.Item.FindControl("LB_HStatus")).Text == "0") //停用
        {
            ((LinkButton)e.Item.FindControl("LBtn_Stop")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_Upload")).Visible = true;
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "停用";
            ((Label)e.Item.FindControl("LB_HStatus")).CssClass = "text-danger";
        }
        else if (((Label)e.Item.FindControl("LB_HStatus")).Text == "1")
        {
            ((LinkButton)e.Item.FindControl("LBtn_Stop")).Visible = true;
            ((LinkButton)e.Item.FindControl("LBtn_Upload")).Visible = false;
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "啟用";
            ((Label)e.Item.FindControl("LB_HStatus")).CssClass = "text-success";
        }
        #endregion


    }
    #endregion

    #region 新增學員
    protected void LBtn_MemberAdd_Click(object sender, EventArgs e)
    {
        if (DDL_HMemberID.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('請選擇學員!');", true);
            return;
        }

        SqlDataReader HSCTMemberdr = SQLdatabase.ExecuteReader(HSCTopicMemberStr + " WHERE HSCForumClassID='" + HSCForumClassID + "' AND HSCTopicID='" + LB_HSCTopicID.Text + "' AND HMemberID ='" + DDL_HMemberID.SelectedValue + "'");
        if (!HSCTMemberdr.Read())
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO [HSCTopic_Role] ( HSCForumClassID, HSCTopicID, HMemberID, HStatus, HCreate, HCreateDT)VALUES( @HSCForumClassID, @HSCTopicID, @HMemberID, @HStatus, @HCreate, @HCreateDT); SELECT SCOPE_IDENTITY() AS HID", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCForumClassID", HSCForumClassID);
            cmd.Parameters.AddWithValue("@HSCTopicID", LB_HSCTopicID.Text.Trim());
            cmd.Parameters.AddWithValue("@HMemberID", DDL_HMemberID.SelectedValue);
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('此學員已是成員囉!');", true);
            return;
        }
        DDL_HMemberID.SelectedValue = "0";

        SDS_HMember.SelectCommand = HSCTopicMemberStr + " WHERE a.HSCForumClassID='" + HSCForumClassID + "' AND a.HSCTopicID='" + LB_HSCTopicID.Text + "'";
        SDS_HMember.DataBind();
        Rpt_HMember.DataBind();
        LB_HMemberNum.Text = Rpt_HMember.Items.Count.ToString();
    }
    #endregion

    #region 成員管理-啟用功能
    protected void LBtn_Upload_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Upload = sender as LinkButton;
        int Upload_CA = Convert.ToInt32(LBtn_Upload.CommandArgument);

        SqlCommand cmd = new SqlCommand("UPDATE HSCTopic_Role SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HID", Upload_CA);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        SDS_HMember.SelectCommand = HSCTopicMemberStr + " WHERE a.HSCForumClassID='" + HSCForumClassID + "' AND a.HSCTopicID='" + LB_HSCTopicID.Text + "'";
        SDS_HMember.DataBind();
        Rpt_HMember.DataBind();
    }
    #endregion

    #region 成員管理-停用功能
    protected void LBtn_Stop_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Stop = sender as LinkButton;
        int Stop_CA = Convert.ToInt32(LBtn_Stop.CommandArgument);

        SqlCommand cmd = new SqlCommand("UPDATE HSCTopic_Role SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HStatus", 0);
        cmd.Parameters.AddWithValue("@HID", Stop_CA);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        SDS_HMember.SelectCommand = HSCTopicMemberStr + " WHERE a.HSCForumClassID='" + HSCForumClassID + "' AND a.HSCTopicID='" + LB_HSCTopicID.Text + "'";
        SDS_HMember.DataBind();
        Rpt_HMember.DataBind();
    }
    #endregion

    #endregion

    #region 公開設定
    protected void LBtn_PublicSetting_Click(object sender, EventArgs e)
    {
        Panel_Rule.Visible = false;
        //AA20240718_加入討論區裡的主題列表
        Panel_SCTopic.Visible = false;
        Panel_Member.Visible = false;
        Panel_Setting.Visible = true;

        LBtn_EditHSCMRule.Style.Add("color", "#000");
        LBtn_MemManage.Style.Add("color", "#000");
        LBtn_PublicSetting.Style.Add("color", "#896ee2");

        //AE20250106_取同一張表
        SqlDataReader MyEBF = SQLdatabase.ExecuteReader("SELECT HID, HPublic FROM HSCForumClass WHERE HID='" + HSCForumClassID + "' AND HStatus=1");

        if (MyEBF.Read())
        {
            RBL_HPublic.SelectedValue = MyEBF["HPublic"].ToString();
        }
        MyEBF.Close();
    }

    #region 公開設定-儲存功能
    protected void LBtn_HPSSubmit_Click(object sender, EventArgs e)
    {


        //AE250105_改抓HSCForumClass資料表，不用分開join
        SqlCommand cmd = new SqlCommand("UPDATE HSCForumClass SET HPublic=@HPublic,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HID", HSCForumClassID);
            cmd.Parameters.AddWithValue("@HPublic", RBL_HPublic.SelectedValue);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            #region 公開設定Log
            SqlCommand cmdlog = new SqlCommand("INSERT INTO [HSCMPublicYN_Log] ( HSCForumClassID, HSCFCName, HSCMPublicYN, HStatus, HCreate, HCreateDT)VALUES( @HSCForumClassID, @HSCFCName, @HSCMPublicYN, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();

            cmdlog.Parameters.AddWithValue("@HSCForumClassID", HSCForumClassID);
            cmdlog.Parameters.AddWithValue("@HSCFCName", LB_HSCFCName.Text);
            cmdlog.Parameters.AddWithValue("@HSCMPublicYN", RBL_HPublic.SelectedValue);
            cmdlog.Parameters.AddWithValue("@HStatus", 1);
            cmdlog.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmdlog.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmdlog.ExecuteNonQuery();
            ConStr.Close();
            cmdlog.Cancel();

            #endregion

     


        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('儲存成功!');", true);
        return;
    }
    #endregion

    #region 公開設定-取消儲存功能

    protected void LBtn_HPSCancel_Click(object sender, EventArgs e)
    {
        SqlDataReader MyEBF = SQLdatabase.ExecuteReader("SELECT HID, HPublic FROM HSCForumClass WHERE HID='" + HSCForumClassID + "' AND HStatus=1");

        if (MyEBF.Read())
        {
            if (!string.IsNullOrEmpty(MyEBF["HPublic"].ToString()))
            {
                RBL_HPublic.SelectedValue = MyEBF["HPublic"].ToString();
            }
          
        }
        else
        {
            RBL_HPublic.SelectedValue = "0";//預設為不公開
        }

        MyEBF.Close();

    }
    #endregion

    #endregion

    #region 回討論區
    protected void LBtn_ReturnForum_Click(object sender, EventArgs e)
    {
        Response.Redirect("HSCForumDetail.aspx?FClassCID=" + HSCForumClassID);
    }
    #endregion


    #region 註冊JS 
    protected void JS_Register()
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", (" $('.js-example-basic-single').select2();"), true);
       
    }
    #endregion
}