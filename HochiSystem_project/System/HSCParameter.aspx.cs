using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class System_HSCParameter : System.Web.UI.Page
{
    /// <summary>
    ///1：專欄分類；2：紀錄類型；3：討論區主類別；4：討論區次類別；5：討論區名稱；6：熱門標籤
    /// </summary>

    SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        for (int i = 0; i < RPT_Tag.Items.Count; i++)
        {

            //AE20250408_先找這筆點的HID來做比對
            string currentHID = ((HiddenField)RPT_Tag.Items[i].FindControl("HF_HID")).Value;

            //if ((Convert.ToInt32(LB_NavTab.Text)) == (i + 1))
            if (LB_NavTab.Text == currentHID)
            {
                ((LinkButton)RPT_Tag.Items[i].FindControl("LBtn_Tag")).CssClass = "nav-link active show";
            }
            else
            {
                ((LinkButton)RPT_Tag.Items[i].FindControl("LBtn_Tag")).CssClass = "nav-link";
            }

        }

    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            SDS_Tag.SelectCommand = "SELECT HID, HName_TW FROM HSCParmTab WHERE HStatus = 1";
            SDS_Tag.DataBind();
            RPT_Tag.DataBind();


            if (Request.QueryString["ID"] != null)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ID"].ToString()))
                {
                    LB_NavTab.Text = Request.QueryString["ID"].ToString();


                    HtmlContainerControl[] Tabstr = { Div_HSCClass, Div_HSCRecordType, Div_HSCForumClassA, Div_HSCForumClassB, Div_HSCForumClassC, Div_HSCHotHashTag };

                    Repeater[] Rptstr = { Rpt_HSCClass, Rpt_HSCRecordType, Rpt_HSCForumClassA, Rpt_HSCForumClassB, Rpt_HSCForumClassC, Rpt_HSCHotHashTag };

                    for (int k = 0; k < Tabstr.Length; k++)
                    {
                        Tabstr[k].Attributes.Add("class", "tab-pane fade");
                        if (k == Convert.ToInt32(LB_NavTab.Text) - 1)
                        {
                            Tabstr[k].Attributes.Add("class", "tab-pane fade active show");

                            //Response.Write("Rptstr=" + Rptstr[3].ID);

                            //Response.End();

                            Rptstr[k].DataBind();
                        }

                    }
                }
            }
            else
            {
                Div_HSCClass.Attributes.Add("class", "tab-pane fade active show");

                //SDS_Tag.SelectCommand = "SELECT HID, HName_TW FROM HSCParmTab WHERE HStatus = 1";
                //SDS_Tag.DataBind();
                //RPT_Tag.DataBind();
            }



        }

        RegScript();//執行js

    }


    #region Tab切換
    protected void LBtn_Tag_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;

        LB_NavTab.Text = btn.TabIndex.ToString();

        //EE20240625_修改寫法
        HtmlContainerControl[] Tabstr = { Div_HSCClass, Div_HSCRecordType, Div_HSCForumClassA, Div_HSCForumClassB, Div_HSCForumClassC, Div_HSCHotHashTag };

        Repeater[] Rptstr = { Rpt_HSCClass, Rpt_HSCRecordType, Rpt_HSCForumClassA, Rpt_HSCForumClassB, Rpt_HSCForumClassC, Rpt_HSCHotHashTag };

        for (int k = 0; k < Tabstr.Length; k++)
        {
            Tabstr[k].Attributes.Add("class", "tab-pane fade");
            if (k == btn.TabIndex - 1)
            {
                Tabstr[k].Attributes.Add("class", "tab-pane fade active show");
                Rptstr[k].DataBind();
            }

        }

    }
    #endregion

    #region 列表資料繫結
    protected void Rpt_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        #region 狀態-按鈕顯示
        if (((Label)e.Item.FindControl("LB_Status")).Text == "0") //停用
        {
            ((LinkButton)e.Item.FindControl("LBtn_Del")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_Upload")).Visible = true;
        }
        else if (((Label)e.Item.FindControl("LB_Status")).Text == "1")
        {
            ((LinkButton)e.Item.FindControl("LBtn_Del")).Visible = true;
            ((LinkButton)e.Item.FindControl("LBtn_Upload")).Visible = false;
        }
        #endregion

        #region 狀態顯示
        string Status = ((Label)e.Item.FindControl("LB_Status")).Text;

        HtmlGenericControl StatusType = e.Item.FindControl("Status") as HtmlGenericControl;
        if (Status == "0")  //停用
        {
            ((Label)e.Item.FindControl("LB_Status")).Text = "停用";
            StatusType.Style.Add("border-color", "#de4848");
            StatusType.Style.Add("color", "#de4848");
        }
        else if (Status == "1")   //啟用
        {
            ((Label)e.Item.FindControl("LB_Status")).Text = "啟用";
            StatusType.Style.Add("border-color", "#09af2d");
            StatusType.Style.Add("color", "#09af2d");
        }
        #endregion

        #region 選單
        if (LB_NavTab.Text == "4")
        {
            //AE20241231_暫時註解
            //if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HSCFCMasterA")).Text))
            //{
            //    ((DropDownList)e.Item.FindControl("DDL_HSCFCMasterA")).SelectedValue = ((Label)e.Item.FindControl("LB_HSCFCMasterA")).Text;
            //}
        }
        else if (LB_NavTab.Text == "5")
        {
            if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HSCFCMasterB")).Text))
            {
                ((DropDownList)e.Item.FindControl("DDL_HSCFCMasterB")).SelectedValue = ((Label)e.Item.FindControl("LB_HSCFCMasterB")).Text;
            }
        }
        #endregion

    }

    #endregion

    #region 新增功能
    protected void LBtn_Add_Click(object sender, EventArgs e)
    {
        if (LB_NavTab.Text == "1")
        {
            if (string.IsNullOrEmpty(TB_HSCClassName.Text.Trim()))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('請輸入專欄分類!');", true);
                return;
            }

            #region 判斷是否重複

            SqlCommand read = new SqlCommand("SELECT HID, HSCClassName FROM HSCClass WHERE HSCClassName=@HSCClassName", ConStr);
            ConStr.Open();
            read.Parameters.AddWithValue("@HSCClassName", TB_HSCClassName.Text.Trim());

            SqlDataReader MyEBF = read.ExecuteReader();

            if (MyEBF.Read())
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('已經有這個專欄分類囉!');", true);
                return;
            }
            MyEBF.Close();
            ConStr.Close();

            #endregion

            SqlCommand cmd = new SqlCommand("INSERT INTO [HSCClass] ( HSCClassName, HStatus, HCreate, HCreateDT)VALUES( @HSCClassName, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HSCClassName", TB_HSCClassName.Text.Trim());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            TB_HSCClassName.Text = null;
            Rpt_HSCClass.DataBind();

        }
        else if (LB_NavTab.Text == "2")
        {
            if (string.IsNullOrEmpty(TB_HSCRTName.Text.Trim()))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('請輸入紀錄類型!');", true);
                return;
            }

            #region 判斷是否重複

            SqlCommand read = new SqlCommand("SELECT HID, HSCRTName FROM HSCRecordType WHERE HSCRTName=@HSCRTName", ConStr);
            ConStr.Open();
            read.Parameters.AddWithValue("@HSCRTName", TB_HSCRTName.Text.Trim());

            SqlDataReader MyEBF = read.ExecuteReader();

            if (MyEBF.Read())
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('已經有這個紀錄類型囉!');", true);
                return;
            }
            MyEBF.Close();
            ConStr.Close();

            #endregion

            SqlCommand cmd = new SqlCommand("INSERT INTO [HSCRecordType] ( HSCRTName, HStatus, HCreate, HCreateDT)VALUES( @HSCRTName, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HSCRTName", TB_HSCRTName.Text.Trim());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            TB_HSCRTName.Text = null;
            Rpt_HSCRecordType.DataBind();

        }
        else if (LB_NavTab.Text == "3")
        {
            if (string.IsNullOrEmpty(TB_HSCFCNameA.Text.Trim()))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('請輸入討論區主類別!');", true);
                return;
            }

            #region 判斷是否重複

            SqlCommand read = new SqlCommand("SELECT HID, HSCFCName FROM HSCForumClass WHERE HSCFCName=@HSCFCName AND HSCFCLevel='10'", ConStr);
            ConStr.Open();
            read.Parameters.AddWithValue("@HSCFCName", TB_HSCFCNameA.Text.Trim());

            SqlDataReader MyEBF = read.ExecuteReader();

            if (MyEBF.Read())
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('已經有這個討論區主類別囉!');", true);
                return;
            }
            MyEBF.Close();
            ConStr.Close();

            #endregion

            SqlCommand cmd = new SqlCommand("INSERT INTO [HSCForumClass] ( HSCFCMaster, HSCFCName, HSCFCLevel, HStatus, HCreate, HCreateDT)VALUES( @HSCFCMaster, @HSCFCName, @HSCFCLevel, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HSCFCMaster", "0");
            cmd.Parameters.AddWithValue("@HSCFCName", TB_HSCFCNameA.Text.Trim());
            cmd.Parameters.AddWithValue("@HSCFCLevel", "10");
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            TB_HSCFCNameA.Text = null;
            Rpt_HSCForumClassA.DataBind();

            DDL_HSCFCMasterA.Items.Clear();
            DDL_HSCFCMasterA.Items.Add(new ListItem("請選擇", "0"));
            DDL_HSCFCMasterA.DataBind();

        }
        else if (LB_NavTab.Text == "4")
        {
            if (DDL_HSCFCMasterA.SelectedValue == "0" || string.IsNullOrEmpty(TB_HSCFCNameB.Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('資料填寫不完整!');", true);
                return;
            }

            #region 判斷是否重複

            SqlCommand read = new SqlCommand("SELECT HID FROM HSCForumClass WHERE HSCFCMaster=@HSCFCMaster AND HSCFCName=@HSCFCName AND HSCFCLevel='20' ", ConStr);
            ConStr.Open();
            read.Parameters.AddWithValue("@HSCFCMaster", DDL_HSCFCMasterA.SelectedValue);
            read.Parameters.AddWithValue("@HSCFCName", TB_HSCFCNameB.Text.Trim());

            SqlDataReader MyEBF = read.ExecuteReader();

            if (MyEBF.Read())
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('已經有這個討論區次類別囉!');", true);
                return;
            }
            MyEBF.Close();
            ConStr.Close();

            #endregion

            SqlCommand cmd = new SqlCommand("INSERT INTO [HSCForumClass] ( HSCFCMaster, HSCFCName, HSCFCLevel, HStatus, HCreate, HCreateDT)VALUES( @HSCFCMaster, @HSCFCName, @HSCFCLevel, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HSCFCMaster", DDL_HSCFCMasterA.SelectedValue);
            cmd.Parameters.AddWithValue("@HSCFCName", TB_HSCFCNameB.Text.Trim());
            cmd.Parameters.AddWithValue("@HSCFCLevel", "20");
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();


            DDL_HSCFCMasterA.SelectedValue = "0";

            DDL_HSCFCMasterB.Items.Clear();
            DDL_HSCFCMasterB.Items.Add(new ListItem("請選擇", "0"));
            DDL_HSCFCMasterB.DataBind();

            TB_HSCFCNameB.Text = null;
            Rpt_HSCForumClassB.DataBind();

        }
        else if (LB_NavTab.Text == "5")
        {
            if (DDL_HSCFCMasterB.SelectedValue == "0" || string.IsNullOrEmpty(TB_HSCFCNameC.Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('資料填寫不完整!');", true);
                return;
            }

            #region 判斷是否重複

            SqlCommand read = new SqlCommand("SELECT HID FROM HSCForumClass WHERE HSCFCMaster=@HSCFCMaster AND HSCFCName=@HSCFCName AND HSCFCLevel='30' ", ConStr);
            ConStr.Open();
            read.Parameters.AddWithValue("@HSCFCMaster", DDL_HSCFCMasterB.SelectedValue);
            read.Parameters.AddWithValue("@HSCFCName", TB_HSCFCNameC.Text.Trim());

            SqlDataReader MyEBF = read.ExecuteReader();

            if (MyEBF.Read())
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('已經有這個討論區名稱囉!');", true);
                return;
            }
            MyEBF.Close();
            ConStr.Close();

            #endregion

            SqlCommand cmd = new SqlCommand("INSERT INTO [HSCForumClass] ( HSCFCMaster, HSCFCName, HSCFCLevel, HStatus, HCreate, HCreateDT)VALUES( @HSCFCMaster, @HSCFCName, @HSCFCLevel, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HSCFCMaster", DDL_HSCFCMasterB.SelectedValue);
            cmd.Parameters.AddWithValue("@HSCFCName", TB_HSCFCNameC.Text.Trim());
            cmd.Parameters.AddWithValue("@HSCFCLevel", "30");
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            //DDL_HSCFCMasterB.Items.Clear();
            //DDL_HSCFCMasterB.Items.Add(new ListItem("請選擇", "0"));
            //DDL_HSCFCMasterB.DataBind();
            DDL_HSCFCMasterB.SelectedValue = "0";
            TB_HSCFCNameC.Text = null;
            Rpt_HSCForumClassC.DataBind();

        }
        else if (LB_NavTab.Text == "6")
        {
            if (string.IsNullOrEmpty(TB_HSCHHashTag.Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('請輸入熱門標籤!');", true);
                return;
            }

            #region 判斷是否重複

            SqlCommand read = new SqlCommand("SELECT HID FROM HSCHotHashTag WHERE HSCHHashTag=@HSCHHashTag ", ConStr);
            ConStr.Open();
            read.Parameters.AddWithValue("@HSCHHashTag", TB_HSCHHashTag.Text.Trim());

            SqlDataReader MyEBF = read.ExecuteReader();

            if (MyEBF.Read())
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('已經有這個熱門標籤囉!');", true);
                return;
            }
            MyEBF.Close();
            ConStr.Close();

            #endregion

            SqlCommand cmd = new SqlCommand("INSERT INTO [HSCHotHashTag] ( HSCHHashTag, HStatus, HCreate, HCreateDT)VALUES( @HSCHHashTag, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HSCHHashTag", TB_HSCHHashTag.Text.Trim());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();


            TB_HSCHHashTag.Text = null;
            Rpt_HSCHotHashTag.DataBind();

        }
        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('新增成功!');", true);
        return;
    }
    #endregion

    #region 編輯功能
    protected void LBtn_Edit_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Edit = sender as LinkButton;
        int index = Convert.ToInt32(LBtn_Edit.CommandArgument);


        if (LB_NavTab.Text == "1")
        {
            ((Label)Rpt_HSCClass.Items[index].FindControl("LB_HSCClassName")).Visible = false;

            ((TextBox)Rpt_HSCClass.Items[index].FindControl("TB_HSCClassName")).Visible = true;

            ((LinkButton)Rpt_HSCClass.Items[index].FindControl("LBtn_Edit")).Visible = false;
            ((LinkButton)Rpt_HSCClass.Items[index].FindControl("LBtn_Save")).Visible = true;

            for (int i = 0; i < Rpt_HSCClass.Items.Count; i++)
            {
                if (i != index)
                {
                    ((Label)Rpt_HSCClass.Items[i].FindControl("LB_HSCClassName")).Visible = true;
                    ((TextBox)Rpt_HSCClass.Items[i].FindControl("TB_HSCClassName")).Visible = false;
                    ((LinkButton)Rpt_HSCClass.Items[i].FindControl("LBtn_Save")).Visible = false;

                    ((LinkButton)Rpt_HSCClass.Items[i].FindControl("LBtn_Edit")).Visible = true;
                }

            }

        }
        else if (LB_NavTab.Text == "2")
        {
            ((Label)Rpt_HSCRecordType.Items[index].FindControl("LB_HSCRTName")).Visible = false;

            ((TextBox)Rpt_HSCRecordType.Items[index].FindControl("TB_HSCRTName")).Visible = true;

            ((LinkButton)Rpt_HSCRecordType.Items[index].FindControl("LBtn_Edit")).Visible = false;
            ((LinkButton)Rpt_HSCRecordType.Items[index].FindControl("LBtn_Save")).Visible = true;

            for (int i = 0; i < Rpt_HSCRecordType.Items.Count; i++)
            {
                if (i != index)
                {
                    ((Label)Rpt_HSCRecordType.Items[i].FindControl("LB_HSCRTName")).Visible = true;
                    ((TextBox)Rpt_HSCRecordType.Items[i].FindControl("TB_HSCRTName")).Visible = false;
                    ((LinkButton)Rpt_HSCRecordType.Items[i].FindControl("LBtn_Save")).Visible = false;

                    ((LinkButton)Rpt_HSCRecordType.Items[i].FindControl("LBtn_Edit")).Visible = true;
                }

            }
        }
        else if (LB_NavTab.Text == "3")
        {
            ((Label)Rpt_HSCForumClassA.Items[index].FindControl("LB_HSCFCNameA")).Visible = false;

            ((TextBox)Rpt_HSCForumClassA.Items[index].FindControl("TB_HSCFCNameA")).Visible = true;

            ((LinkButton)Rpt_HSCForumClassA.Items[index].FindControl("LBtn_Edit")).Visible = false;
            ((LinkButton)Rpt_HSCForumClassA.Items[index].FindControl("LBtn_Save")).Visible = true;

            for (int i = 0; i < Rpt_HSCForumClassA.Items.Count; i++)
            {
                if (i != index)
                {
                    ((Label)Rpt_HSCForumClassA.Items[i].FindControl("LB_HSCFCNameA")).Visible = true;
                    ((TextBox)Rpt_HSCForumClassA.Items[i].FindControl("TB_HSCFCNameA")).Visible = false;
                    ((LinkButton)Rpt_HSCForumClassA.Items[i].FindControl("LBtn_Save")).Visible = false;

                    ((LinkButton)Rpt_HSCForumClassA.Items[i].FindControl("LBtn_Edit")).Visible = true;
                }

            }
        }
        else if (LB_NavTab.Text == "4")
        {
            ((Label)Rpt_HSCForumClassB.Items[index].FindControl("LB_HSCFCNameA")).Visible = false;
            ((Label)Rpt_HSCForumClassB.Items[index].FindControl("LB_HSCFCNameB")).Visible = false;

            ((TextBox)Rpt_HSCForumClassB.Items[index].FindControl("TB_HSCFCNameB")).Visible = true;
            ((DropDownList)Rpt_HSCForumClassB.Items[index].FindControl("DDL_HSCFCMasterA")).Visible = true;


            ((LinkButton)Rpt_HSCForumClassB.Items[index].FindControl("LBtn_Edit")).Visible = false;
            ((LinkButton)Rpt_HSCForumClassB.Items[index].FindControl("LBtn_Save")).Visible = true;

            for (int i = 0; i < Rpt_HSCForumClassB.Items.Count; i++)
            {
                if (i != index)
                {
                    ((Label)Rpt_HSCForumClassB.Items[i].FindControl("LB_HSCFCNameA")).Visible = true;
                    ((Label)Rpt_HSCForumClassB.Items[i].FindControl("LB_HSCFCNameB")).Visible = true;

                    ((TextBox)Rpt_HSCForumClassB.Items[i].FindControl("TB_HSCFCNameB")).Visible = false;
                    ((DropDownList)Rpt_HSCForumClassB.Items[i].FindControl("DDL_HSCFCMasterA")).Visible = false;

                    ((LinkButton)Rpt_HSCForumClassB.Items[i].FindControl("LBtn_Save")).Visible = false;

                    ((LinkButton)Rpt_HSCForumClassB.Items[i].FindControl("LBtn_Edit")).Visible = true;
                }

            }
        }
        else if (LB_NavTab.Text == "5")
        {
            ((Label)Rpt_HSCForumClassC.Items[index].FindControl("LB_HSCFCNameB")).Visible = false;
            ((Label)Rpt_HSCForumClassC.Items[index].FindControl("LB_HSCFCNameC")).Visible = false;

            ((TextBox)Rpt_HSCForumClassC.Items[index].FindControl("TB_HSCFCNameC")).Visible = true;
            ((DropDownList)Rpt_HSCForumClassC.Items[index].FindControl("DDL_HSCFCMasterB")).Visible = true;


            ((LinkButton)Rpt_HSCForumClassC.Items[index].FindControl("LBtn_Edit")).Visible = false;
            ((LinkButton)Rpt_HSCForumClassC.Items[index].FindControl("LBtn_Save")).Visible = true;

            for (int i = 0; i < Rpt_HSCForumClassC.Items.Count; i++)
            {
                if (i != index)
                {
                    ((Label)Rpt_HSCForumClassC.Items[i].FindControl("LB_HSCFCNameB")).Visible = true;
                    ((Label)Rpt_HSCForumClassC.Items[i].FindControl("LB_HSCFCNameC")).Visible = true;

                    ((TextBox)Rpt_HSCForumClassC.Items[i].FindControl("TB_HSCFCNameC")).Visible = false;
                    ((DropDownList)Rpt_HSCForumClassC.Items[i].FindControl("DDL_HSCFCMasterB")).Visible = false;

                    ((LinkButton)Rpt_HSCForumClassC.Items[i].FindControl("LBtn_Save")).Visible = false;

                    ((LinkButton)Rpt_HSCForumClassC.Items[i].FindControl("LBtn_Edit")).Visible = true;
                }

            }
        }
        else if (LB_NavTab.Text == "6")
        {
            ((Label)Rpt_HSCHotHashTag.Items[index].FindControl("LB_HSCHHashTag")).Visible = false;
            ((TextBox)Rpt_HSCHotHashTag.Items[index].FindControl("TB_HSCHHashTag")).Visible = true;

            ((LinkButton)Rpt_HSCHotHashTag.Items[index].FindControl("LBtn_Edit")).Visible = false;
            ((LinkButton)Rpt_HSCHotHashTag.Items[index].FindControl("LBtn_Save")).Visible = true;

            for (int i = 0; i < Rpt_HSCHotHashTag.Items.Count; i++)
            {
                if (i != index)
                {
                    ((Label)Rpt_HSCHotHashTag.Items[i].FindControl("LB_HSCHHashTag")).Visible = true;
                    ((TextBox)Rpt_HSCHotHashTag.Items[i].FindControl("TB_HSCHHashTag")).Visible = false;

                    ((LinkButton)Rpt_HSCHotHashTag.Items[i].FindControl("LBtn_Save")).Visible = false;

                    ((LinkButton)Rpt_HSCHotHashTag.Items[i].FindControl("LBtn_Edit")).Visible = true;
                }

            }
        }


    }
    #endregion

    #region 編輯儲存
    protected void LBtn_Save_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Save = sender as LinkButton;
        int index = Convert.ToInt32(LBtn_Save.CommandArgument);

        if (LB_NavTab.Text == "1")
        {
            if (string.IsNullOrEmpty(((TextBox)Rpt_HSCClass.Items[index].FindControl("TB_HSCClassName")).Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('請輸入專欄分類!');", true);
                return;
            }

            //資料庫
            SqlCommand cmd = new SqlCommand("UPDATE [HSCClass] SET HSCClassName=@HSCClassName, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HID", ((Label)Rpt_HSCClass.Items[index].FindControl("LB_HID")).Text);
            cmd.Parameters.AddWithValue("@HSCClassName", ((TextBox)Rpt_HSCClass.Items[index].FindControl("TB_HSCClassName")).Text.Trim());
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            Rpt_HSCClass.DataBind();
        }
        else if (LB_NavTab.Text == "2")
        {
            if (string.IsNullOrEmpty(((TextBox)Rpt_HSCRecordType.Items[index].FindControl("TB_HSCRTName")).Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('請輸入紀錄類型!');", true);
                return;
            }

            //資料庫
            SqlCommand cmd = new SqlCommand("UPDATE [HSCRecordType] SET HSCRTName=@HSCRTName, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HID", ((Label)Rpt_HSCRecordType.Items[index].FindControl("LB_HID")).Text);
            cmd.Parameters.AddWithValue("@HSCRTName", ((TextBox)Rpt_HSCRecordType.Items[index].FindControl("TB_HSCRTName")).Text.Trim());
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            Rpt_HSCRecordType.DataBind();
        }
        else if (LB_NavTab.Text == "3")
        {
            if (string.IsNullOrEmpty(((TextBox)Rpt_HSCForumClassA.Items[index].FindControl("TB_HSCFCNameA")).Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('請輸入討論區主類別!');", true);
                return;
            }

            //資料庫
            SqlCommand cmd = new SqlCommand("UPDATE [HSCForumClass] SET HSCFCName=@HSCFCName, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID AND HSCFCLevel='10'", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HID", ((Label)Rpt_HSCForumClassA.Items[index].FindControl("LB_HID")).Text);
            cmd.Parameters.AddWithValue("@HSCFCName", ((TextBox)Rpt_HSCForumClassA.Items[index].FindControl("TB_HSCFCNameA")).Text.Trim());
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            Rpt_HSCForumClassA.DataBind();

            DDL_HSCFCMasterA.Items.Clear();
            DDL_HSCFCMasterA.Items.Add(new ListItem("請選擇", "0"));
            DDL_HSCFCMasterA.DataBind();
        }
        else if (LB_NavTab.Text == "4")
        {
            if (((DropDownList)Rpt_HSCForumClassB.Items[index].FindControl("DDL_HSCFCMasterA")).SelectedValue == "0" || string.IsNullOrEmpty(((TextBox)Rpt_HSCForumClassB.Items[index].FindControl("TB_HSCFCNameB")).Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('資料填寫不完整!');", true);
                return;
            }

            //資料庫
            SqlCommand cmd = new SqlCommand("UPDATE [HSCForumClass] SET HSCFCMaster=@HSCFCMaster, HSCFCName=@HSCFCName, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID AND HSCFCLevel='20'", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HID", ((Label)Rpt_HSCForumClassB.Items[index].FindControl("LB_HID")).Text);
            cmd.Parameters.AddWithValue("@HSCFCMaster", ((DropDownList)Rpt_HSCForumClassB.Items[index].FindControl("DDL_HSCFCMasterA")).SelectedValue);
            cmd.Parameters.AddWithValue("@HSCFCName", ((TextBox)Rpt_HSCForumClassB.Items[index].FindControl("TB_HSCFCNameB")).Text.Trim());
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            Rpt_HSCForumClassB.DataBind();

            DDL_HSCFCMasterB.Items.Clear();
            DDL_HSCFCMasterB.Items.Add(new ListItem("請選擇", "0"));
            DDL_HSCFCMasterB.DataBind();
        }
        else if (LB_NavTab.Text == "5")
        {
            if (((DropDownList)Rpt_HSCForumClassC.Items[index].FindControl("DDL_HSCFCMasterB")).SelectedValue == "0" || string.IsNullOrEmpty(((TextBox)Rpt_HSCForumClassC.Items[index].FindControl("TB_HSCFCNameC")).Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('資料填寫不完整!');", true);
                return;
            }

            //資料庫
            SqlCommand cmd = new SqlCommand("UPDATE [HSCForumClass] SET HSCFCMaster=@HSCFCMaster, HSCFCName=@HSCFCName, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HID", ((Label)Rpt_HSCForumClassC.Items[index].FindControl("LB_HID")).Text);
            cmd.Parameters.AddWithValue("@HSCFCMaster", ((DropDownList)Rpt_HSCForumClassC.Items[index].FindControl("DDL_HSCFCMasterB")).SelectedValue);
            cmd.Parameters.AddWithValue("@HSCFCName", ((TextBox)Rpt_HSCForumClassC.Items[index].FindControl("TB_HSCFCNameC")).Text.Trim());
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            Rpt_HSCForumClassC.DataBind();
        }
        else if (LB_NavTab.Text == "6")
        {
            if (string.IsNullOrEmpty(((TextBox)Rpt_HSCHotHashTag.Items[index].FindControl("TB_HSCHHashTag")).Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('請輸入熱門標籤!');", true);
                return;
            }

            //資料庫
            SqlCommand cmd = new SqlCommand("UPDATE [HSCHotHashTag] SET HSCHHashTag=@HSCHHashTag, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HID", ((Label)Rpt_HSCHotHashTag.Items[index].FindControl("LB_HID")).Text);
            cmd.Parameters.AddWithValue("@HSCHHashTag", ((TextBox)Rpt_HSCHotHashTag.Items[index].FindControl("TB_HSCHHashTag")).Text.Trim());
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            Rpt_HSCHotHashTag.DataBind();
        }

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('更新成功!');", true);
        return;
    }
    #endregion

    #region 動態啟用
    protected void LBtn_Upload_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Upload = sender as LinkButton;
        int Upload_CA = Convert.ToInt32(LBtn_Upload.CommandArgument);

        if (LB_NavTab.Text == "1")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HSCClass SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HID", Upload_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
            Rpt_HSCClass.DataBind();

        }
        else if (LB_NavTab.Text == "2")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HSCRecordType SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HID", Upload_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
            Rpt_HSCRecordType.DataBind();

        }
        else if (LB_NavTab.Text == "3")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HSCForumClass SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HID", Upload_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
            Rpt_HSCForumClassA.DataBind();

            DDL_HSCFCMasterA.Items.Clear();
            DDL_HSCFCMasterA.Items.Add(new ListItem("請選擇", "0"));
            DDL_HSCFCMasterA.DataBind();
        }
        else if (LB_NavTab.Text == "4")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HSCForumClass SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HID", Upload_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
            Rpt_HSCForumClassB.DataBind();

            DDL_HSCFCMasterB.Items.Clear();
            DDL_HSCFCMasterB.Items.Add(new ListItem("請選擇", "0"));
            DDL_HSCFCMasterB.DataBind();

        }
        else if (LB_NavTab.Text == "5")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HSCForumClass SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HID", Upload_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
            Rpt_HSCForumClassC.DataBind();

        }
        else if (LB_NavTab.Text == "6")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HSCHotHashTag SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HID", Upload_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
            Rpt_HSCHotHashTag.DataBind();

        }

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('啟用成功!');", true);
        return;
    }
    #endregion

    #region 動態停用
    protected void LBtn_Del_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Del = sender as LinkButton;
        int Del_CA = Convert.ToInt32(LBtn_Del.CommandArgument);

        if (LB_NavTab.Text == "1")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HSCClass SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HStatus", 0);
            cmd.Parameters.AddWithValue("@HID", Del_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
            Rpt_HSCClass.DataBind();

        }
        else if (LB_NavTab.Text == "2")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HSCRecordType SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HStatus", 0);
            cmd.Parameters.AddWithValue("@HID", Del_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
            Rpt_HSCRecordType.DataBind();

        }
        else if (LB_NavTab.Text == "3")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HSCForumClass SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HStatus", 0);
            cmd.Parameters.AddWithValue("@HID", Del_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
            Rpt_HSCForumClassA.DataBind();

            DDL_HSCFCMasterA.Items.Clear();
            DDL_HSCFCMasterA.Items.Add(new ListItem("請選擇", "0"));
            DDL_HSCFCMasterA.DataBind();
        }
        else if (LB_NavTab.Text == "4")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HSCForumClass SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HStatus", 0);
            cmd.Parameters.AddWithValue("@HID", Del_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            Rpt_HSCForumClassB.DataBind();

            DDL_HSCFCMasterB.Items.Clear();
            DDL_HSCFCMasterB.Items.Add(new ListItem("請選擇", "0"));
            DDL_HSCFCMasterB.DataBind();

        }
        else if (LB_NavTab.Text == "5")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HSCForumClass SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HStatus", 0);
            cmd.Parameters.AddWithValue("@HID", Del_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            Rpt_HSCForumClassC.DataBind();

        }
        else if (LB_NavTab.Text == "6")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HSCHotHashTag SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HStatus", 0);
            cmd.Parameters.AddWithValue("@HID", Del_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

            Rpt_HSCHotHashTag.DataBind();

        }
        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('停用成功!');", true);
        return;
    }
    #endregion

    #region 執行js
    public void RegScript()
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "js", ("$('.js-example-basic-single').select2({ closeOnSelect: true, })"), true);
    }
    #endregion


}