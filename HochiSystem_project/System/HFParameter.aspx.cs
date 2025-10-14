using NPOI.OpenXmlFormats.Vml;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class System_HFParameter : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

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
        for (int i = 0; i < RPT_Tag.Items.Count; i++)
        {
            if ((Convert.ToInt32(LB_NavTab.Text)) == (i + 1))
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
            Div_HDPurpose.Attributes.Add("class", "tab-pane fade active show");
            SDS_Tag.SelectCommand = "SELECT HID, HName_TW FROM HFParmTab WHERE HStatus = 1";
            SDS_Tag.DataBind();
            RPT_Tag.DataBind();
        }

        RegScript();//執行js

    }

    #region Tab切換
    protected void LBtn_Tag_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;

        LB_NavTab.Text = btn.TabIndex.ToString();

        if (btn.TabIndex == 1)
        {
            Div_HDPurpose.Attributes.Add("class", "tab-pane fade active show");
            Div_HDItem.Attributes.Add("class", "tab-pane fade");
            Div_HCCPeriodYN.Attributes.Add("class", "tab-pane fade");

            SDS_HDPurpose.SelectCommand = "SELECT HID, HDTypeCode, HDPurpose, HStatus FROM HDPurpose ORDER BY HStatus DESC";
            SDS_HDPurpose.DataBind();
            Rpt_HDPurpose.DataBind();
        }
        else if (btn.TabIndex == 2)
        {
            Div_HDPurpose.Attributes.Add("class", "tab-pane fade");
            Div_HDItem.Attributes.Add("class", "tab-pane fade active show");
            Div_HCCPeriodYN.Attributes.Add("class", "tab-pane fade");

            SDS_HDonationItem.SelectCommand = "SELECT A.HID, A.HDPurposeID, B.HDPurpose, A.HDItem, A.HRemark, A.HStatus FROM HDonationItem A INNER JOIN HDPurpose B ON A.HDPurposeID=B.HID ORDER BY A.HStatus DESC";
            SDS_HDonationItem.DataBind();
            Rpt_HDonationItem.DataBind();


        }
        else if (btn.TabIndex == 3)
        {
            Div_HDPurpose.Attributes.Add("class", "tab-pane fade");
            Div_HDItem.Attributes.Add("class", "tab-pane fade");
            Div_HCCPeriodYN.Attributes.Add("class", "tab-pane fade active show");

            SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT  HID, HCCPControl FROM HCCPControl WHERE HID='1'");
            if (dr.Read())
            {
                DDL_HCCPeriodYN.SelectedValue = dr["HCCPControl"].ToString();
            }
            dr.Close();

        }

    }
    #endregion

    #region--列表資料繫結--
    protected void RPT_ItemDataBound(object sender, RepeaterItemEventArgs e)
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
        if (LB_NavTab.Text == "2")
        {
            if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HDPurposeID")).Text))
            {
                ((DropDownList)e.Item.FindControl("DDL_HDPurpose")).SelectedValue = ((Label)e.Item.FindControl("LB_HDPurposeID")).Text;
            }
        }
        #endregion

        //連結
        //GA20241030_捐款項目新增連結至定期定額專區
        if (LB_NavTab.Text == "2")
        {
            ((HyperLink)e.Item.FindControl("HL_HCCRegular")).NavigateUrl = "HCCRegular.aspx?HID=" + ((Label)e.Item.FindControl("LB_HID")).Text;
        }


    }
    #endregion

    #region 新增功能
    protected void LBtn_Add_Click(object sender, EventArgs e)
    {
        //1：捐款用途；2：捐款項目；3：前台信用卡授權設定
        if (LB_NavTab.Text == "1")
        {
            if (string.IsNullOrEmpty(TB_HDPurpose.Text) || string.IsNullOrEmpty(TB_HDTypeCode.Text.Trim()))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('資料填寫不完整!');", true);
                return;
            }

            #region 判斷是否重複

            SqlCommand read = new SqlCommand("SELECT HID, HDPurpose FROM HDPurpose WHERE HDPurpose=@HDPurpose", con);
            con.Open();
            read.Parameters.AddWithValue("@HDPurpose", TB_HDPurpose.Text.Trim());

            SqlDataReader MyEBF = read.ExecuteReader();

            if (MyEBF.Read())
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('已經有這個捐款用途囉!');", true);
                return;
            }
            MyEBF.Close();
            con.Close();

            #endregion

            //加入資料庫
            con.Open();


            SqlCommand cmd = new SqlCommand("INSERT INTO [HDPurpose] (HDTypeCode, HDPurpose, HStatus,HCreate, HCreateDT, HModifyDT)VALUES(@HDTypeCode,@HDPurpose, @HStatus, @HCreate, @HCreateDT, @HModifyDT)", con);

            cmd.Parameters.AddWithValue("@HDTypeCode", TB_HDTypeCode.Text.Trim());
            cmd.Parameters.AddWithValue("@HDPurpose", TB_HDPurpose.Text.Trim());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Cancel();

            TB_HDPurpose.Text = null;
            TB_HDTypeCode.Text = null;

            Rpt_HDPurpose.DataBind();
            DDL_HDPurpose.Items.Clear();
            DDL_HDPurpose.Items.Add(new ListItem("請選擇", "0"));
            DDL_HDPurpose.DataBind();

        }
        else if (LB_NavTab.Text == "2")
        {
            if (DDL_HDPurpose.SelectedValue == "0" || string.IsNullOrEmpty(TB_HDItem.Text.Trim()) )
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('資料填寫不完整!');", true);
                return;
            }

            #region 判斷是否重複

            SqlCommand read = new SqlCommand("SELECT HID, HDItem FROM HDonationItem WHERE HDItem=@HDItem", con);

            con.Open();
            read.Parameters.AddWithValue("@HDItem", TB_HDItem.Text.Trim());

            SqlDataReader MyEBF = read.ExecuteReader();

            if (MyEBF.Read())
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('已經有這個捐款項目囉!');", true);
                return;
            }
            MyEBF.Close();
            con.Close();

            #endregion



            //加入資料庫
            con.Open();


            SqlCommand cmd = new SqlCommand("INSERT INTO [HDonationItem] (HDPurposeID, HDItem, HStatus, HCreate, HCreateDT, HModifyDT)VALUES(@HDPurposeID,  @HDItem,  @HStatus, @HCreate, @HCreateDT, @HModifyDT)", con);

            cmd.Parameters.AddWithValue("@HDPurposeID", DDL_HDPurpose.SelectedValue);
            cmd.Parameters.AddWithValue("@HDItem", TB_HDItem.Text.Trim());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Cancel();

            DDL_HDPurpose.SelectedValue = "0";
            TB_HDItem.Text = null;

            Rpt_HDonationItem.DataBind();

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

        //1：捐款用途；2：捐款項目；3：前台信用卡授權設定
        if (LB_NavTab.Text == "1")
        {
            ((Label)Rpt_HDPurpose.Items[index].FindControl("LB_HDPurpose")).Visible = false;
            ((Label)Rpt_HDPurpose.Items[index].FindControl("LB_HDTypeCode")).Visible = false;
            ((TextBox)Rpt_HDPurpose.Items[index].FindControl("TB_HDTypeCode")).Visible = true;
            ((TextBox)Rpt_HDPurpose.Items[index].FindControl("TB_HDPurpose")).Visible = true;

            ((LinkButton)Rpt_HDPurpose.Items[index].FindControl("LBtn_Edit")).Visible = false;
            ((LinkButton)Rpt_HDPurpose.Items[index].FindControl("LBtn_Save")).Visible = true;

            for (int i = 0; i < Rpt_HDPurpose.Items.Count; i++)
            {
                if (i != index)
                {
                    ((Label)Rpt_HDPurpose.Items[i].FindControl("LB_HDPurpose")).Visible = true;
                    ((Label)Rpt_HDPurpose.Items[i].FindControl("LB_HDTypeCode")).Visible = true;
                    ((TextBox)Rpt_HDPurpose.Items[i].FindControl("TB_HDTypeCode")).Visible = false;
                    ((TextBox)Rpt_HDPurpose.Items[i].FindControl("TB_HDPurpose")).Visible = false;
                    ((LinkButton)Rpt_HDPurpose.Items[i].FindControl("LBtn_Save")).Visible = false;

                    ((LinkButton)Rpt_HDPurpose.Items[i].FindControl("LBtn_Edit")).Visible = true;
                }

            }

        }
        else if (LB_NavTab.Text == "2")
        {
            ((Label)Rpt_HDonationItem.Items[index].FindControl("LB_HDItem")).Visible = false;
           
            ((TextBox)Rpt_HDonationItem.Items[index].FindControl("TB_HDItem")).Visible = true;
          
            ((Label)Rpt_HDonationItem.Items[index].FindControl("LB_HDPurpose")).Visible = false;
            ((DropDownList)Rpt_HDonationItem.Items[index].FindControl("DDL_HDPurpose")).Visible = true;

            ((LinkButton)Rpt_HDonationItem.Items[index].FindControl("LBtn_Edit")).Visible = false;
            ((LinkButton)Rpt_HDonationItem.Items[index].FindControl("LBtn_Save")).Visible = true;

            for (int i = 0; i < Rpt_HDonationItem.Items.Count; i++)
            {
                if (i != index)
                {
                    ((Label)Rpt_HDonationItem.Items[i].FindControl("LB_HDPurpose")).Visible = true;
                    ((DropDownList)Rpt_HDonationItem.Items[i].FindControl("DDL_HDPurpose")).Visible = false;
                    ((Label)Rpt_HDonationItem.Items[i].FindControl("LB_HDItem")).Visible = true;
                    ((TextBox)Rpt_HDonationItem.Items[i].FindControl("TB_HDItem")).Visible = false;

                    ((LinkButton)Rpt_HDonationItem.Items[i].FindControl("LBtn_Save")).Visible = false;
                    ((LinkButton)Rpt_HDonationItem.Items[i].FindControl("LBtn_Edit")).Visible = true;
                }

            }




        }


    }
    #endregion

    #region 編輯儲存
    protected void LBtn_Save_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Save = sender as LinkButton;
        int Save_CA = Convert.ToInt32(LBtn_Save.CommandArgument);

        //1：捐款用途；2：捐款項目；3：前台信用卡授權設定
        if (LB_NavTab.Text == "1")
        {
            if (string.IsNullOrEmpty(((TextBox)Rpt_HDPurpose.Items[Save_CA].FindControl("TB_HDPurpose")).Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('資料填寫不完整!');", true);
                return;
            }

            //資料庫
            SqlCommand cmd = new SqlCommand("UPDATE [HDPurpose] SET HDPurpose=@HDPurpose, HDTypeCode=@HDTypeCode, HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", con);
            con.Open();
            cmd.Parameters.AddWithValue("@HID", ((Label)Rpt_HDPurpose.Items[Save_CA].FindControl("LB_HID")).Text);
            cmd.Parameters.AddWithValue("@HDTypeCode", ((TextBox)Rpt_HDPurpose.Items[Save_CA].FindControl("TB_HDTypeCode")).Text.Trim());
            cmd.Parameters.AddWithValue("@HDPurpose", ((TextBox)Rpt_HDPurpose.Items[Save_CA].FindControl("TB_HDPurpose")).Text.Trim());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Cancel();

            Rpt_HDPurpose.DataBind();

            DDL_HDPurpose.Items.Clear();
            DDL_HDPurpose.Items.Add(new ListItem("請選擇", "0"));
            DDL_HDPurpose.DataBind();
        }
        else if (LB_NavTab.Text == "2")
        {
            if (string.IsNullOrEmpty(((TextBox)Rpt_HDonationItem.Items[Save_CA].FindControl("TB_HDItem")).Text))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('資料填寫不完整!');", true);
                return;
            }

            #region 判斷是否重複

            SqlCommand read = new SqlCommand("SELECT a.HID, a.HDItem FROM HDonationItem AS a  WHERE a.HDItem=@HDItem AND NOT EXISTS ( SELECT HDItem FROM HDonationItem  tb WHERE  a.HID =@HID )", con);

            con.Open();
            read.Parameters.AddWithValue("@HID", ((Label)Rpt_HDonationItem.Items[Save_CA].FindControl("LB_HID")).Text);
            read.Parameters.AddWithValue("@HDItem", ((TextBox)Rpt_HDonationItem.Items[Save_CA].FindControl("TB_HDItem")).Text.Trim());
            SqlDataReader MyEBF = read.ExecuteReader();

            if (MyEBF.Read())
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('已經有這個捐款項目囉!');", true);
                return;
            }
            MyEBF.Close();
            con.Close();
            #endregion


            //資料庫
            SqlCommand cmd = new SqlCommand("UPDATE [HDonationItem] SET HDPurposeID=@HDPurposeID,  HDItem=@HDItem, HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", con);
            con.Open();
            cmd.Parameters.AddWithValue("@HID", ((Label)Rpt_HDonationItem.Items[Save_CA].FindControl("LB_HID")).Text);
            cmd.Parameters.AddWithValue("@HDPurposeID", ((DropDownList)Rpt_HDonationItem.Items[Save_CA].FindControl("DDL_HDPurpose")).Text.Trim());
           
            cmd.Parameters.AddWithValue("@HDItem", ((TextBox)Rpt_HDonationItem.Items[Save_CA].FindControl("TB_HDItem")).Text.Trim());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Cancel();

            Rpt_HDonationItem.DataBind();

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

        //1：捐款用途；2：捐款項目；3：前台信用卡授權設定
        if (LB_NavTab.Text == "1")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HDPurpose SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", con);
            con.Open();
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HID", Upload_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Cancel();
            Rpt_HDPurpose.DataBind();
            DDL_HDPurpose.Items.Clear();
            DDL_HDPurpose.Items.Add(new ListItem("請選擇", "0"));
            DDL_HDPurpose.DataBind();
        }
        else if (LB_NavTab.Text == "2")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HDonationItem SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID =@HID", con);
            con.Open();
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HID", Upload_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Cancel();
            Rpt_HDonationItem.DataBind();
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

        //1：捐款用途；2：捐款項目；3：前台信用卡授權設定
        if (LB_NavTab.Text == "1")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HDPurpose SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", con);
            con.Open();
            cmd.Parameters.AddWithValue("@HStatus", 0);
            cmd.Parameters.AddWithValue("@HID", Del_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Cancel();
            Rpt_HDPurpose.DataBind();
            DDL_HDPurpose.Items.Clear();
            DDL_HDPurpose.Items.Add(new ListItem("請選擇", "0"));
            DDL_HDPurpose.DataBind();
        }
        else if (LB_NavTab.Text == "2")
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HDonationItem SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", con);
            con.Open();
            cmd.Parameters.AddWithValue("@HStatus", 0);
            cmd.Parameters.AddWithValue("@HID", Del_CA);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Cancel();
            Rpt_HDonationItem.DataBind();
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

    #region 前台信用卡授權設定-儲存功能
    protected void Btn_HCCPYNSubmit_Click(object sender, EventArgs e)
    {
        if (LB_NavTab.Text == "3")
        {
            //資料庫
            SqlCommand cmd = new SqlCommand("UPDATE [HCCPControl] SET HCCPControl=@HCCPControl, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID='1'", con);
            con.Open();
            cmd.Parameters.AddWithValue("@HCCPControl", DDL_HCCPeriodYN.SelectedValue);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Cancel();
        }


        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT  HID, HCCPControl FROM HCCPControl WHERE HID='1'");
        if (dr.Read())
        {
            DDL_HCCPeriodYN.SelectedValue = dr["HCCPControl"].ToString();
        }
        dr.Close();

        //DDL_HCCPeriodYN.Enabled = false;
        //Btn_HCCPYNModify.Visible = true;
        //Btn_HCCPYNSubmit.Visible = false;

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('儲存成功!');", true);
        return;

    }
    #endregion

    #region 前台信用卡授權設定-修改功能
    protected void Btn_HCCPYNModify_Click(object sender, EventArgs e)
    {
        DDL_HCCPeriodYN.Enabled = true;
        Btn_HCCPYNModify.Visible = false;
        Btn_HCCPYNSubmit.Visible = true;
    }
    #endregion

}