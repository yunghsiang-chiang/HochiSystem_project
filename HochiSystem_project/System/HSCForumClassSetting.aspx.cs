using DocumentFormat.OpenXml.Drawing.Charts;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Web.UI.HtmlControls;
using ECPay.Payment.Integration.Extensions; // 需安裝 Newtonsoft.Json 套件 (NuGet)

public partial class System_HSCForumClassSetting : System.Web.UI.Page
{
    SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);



    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Rpt_HSCForumClassFirst.DataBind();
        }

    }





    #region 第一層
    protected void Rpt_HSCForumClassFirst_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        #region 狀態顯示
        string Status = ((Label)e.Item.FindControl("LB_HStatus")).Text;

        if (Status == "0")  //停用
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "停用";
            ((Label)e.Item.FindControl("LB_HStatus")).Style.Add("color", "#de4848");

            ((Button)e.Item.FindControl("Btn_SCForumClassA_Disabled")).Visible = false;
            ((Button)e.Item.FindControl("Btn_SCForumClassA_Enabled")).Visible = true;
        }
        else if (Status == "1")   //啟用
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "啟用";
            ((Label)e.Item.FindControl("LB_HStatus")).Style.Add("color", "#09af2d");

            ((Button)e.Item.FindControl("Btn_SCForumClassA_Disabled")).Visible = true;
            ((Button)e.Item.FindControl("Btn_SCForumClassA_Enabled")).Visible = false;
        }
        #endregion


        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        //次類別資料
        Repeater Rpt_HSCForumClassSecond = e.Item.FindControl("Rpt_HSCForumClassSecond") as Repeater;  //找到repeater物件
        SqlDataSource SDS_HSCForumClassSecond = e.Item.FindControl("SDS_HSCForumClassSecond") as SqlDataSource;  //找到SqlDataSource物件        
        SDS_HSCForumClassSecond.SelectCommand = "SELECT a.HID, a.HSCFCName AS HSCForumClassB, a.HSCFCLevel, a.HStatus, b.HSCFCName AS HSCForumMaster, a.HSCFCMaster, a.HSort FROM HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON b.HID=a.HSCFCMaster WHERE a.HSCFCLevel = '20' AND a.HSCFCMaster ='" + ((Label)e.Item.FindControl("LB_HID")).Text + "' ORDER BY b.HStatus DESC, a.HSort  ASC;";
        Rpt_HSCForumClassSecond.DataBind();

        if (Rpt_HSCForumClassSecond.Items.Count == 0)
        {
            ((HtmlGenericControl)e.Item.FindControl("Btn_Toggle")).Visible = false;
        }

    }
    #endregion

    #region
    protected void Rpt_HSCForumClassSecond_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        #region 狀態顯示
        string Status = ((Label)e.Item.FindControl("LB_HStatus")).Text;

        if (Status == "0")  //停用
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "停用";
            ((Label)e.Item.FindControl("LB_HStatus")).Style.Add("color", "#de4848");

            ((Button)e.Item.FindControl("Btn_SCForumClassB_Disabled")).Visible = false;
            ((Button)e.Item.FindControl("Btn_SCForumClassB_Enabled")).Visible = true;
        }
        else if (Status == "1")   //啟用
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "啟用";
            ((Label)e.Item.FindControl("LB_HStatus")).Style.Add("color", "#09af2d");

            ((Button)e.Item.FindControl("Btn_SCForumClassB_Disabled")).Visible = true;
            ((Button)e.Item.FindControl("Btn_SCForumClassB_Enabled")).Visible = false;
        }
        #endregion


        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        //次類別資料
        Repeater Rpt_HSCForumClassThird = e.Item.FindControl("Rpt_HSCForumClassThird") as Repeater;  //找到repeater物件
        SqlDataSource SDS_HSCForumClassThird = e.Item.FindControl("SDS_HSCForumClassThird") as SqlDataSource;  //找到SqlDataSource物件        
        SDS_HSCForumClassThird.SelectCommand = "SELECT a.HID, a.HSCFCName AS HSCForumClassC, a.HSCFCLevel, a.HStatus, a.HPublic, b.HSCFCName AS HSCForumMaster,a.HSCFCMaster, a.HSort FROM HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON b.HID=a.HSCFCMaster WHERE a.HSCFCLevel = '30' AND a.HSCFCMaster ='" + ((Label)e.Item.FindControl("LB_HID")).Text + "' ORDER BY b.HStatus DESC, a.HSort  ASC;";
        Rpt_HSCForumClassThird.DataBind();

        if (Rpt_HSCForumClassThird.Items.Count == 0)
        {
            ((HtmlGenericControl)e.Item.FindControl("Btn_Toggle")).Visible = false;
        }

    }
    #endregion

    protected void Rpt_HSCForumClassThird_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        #region 狀態顯示
        string Status = ((Label)e.Item.FindControl("LB_HStatus")).Text;

        if (Status == "0")  //停用
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "停用";
            ((Label)e.Item.FindControl("LB_HStatus")).Style.Add("color", "#de4848");

            ((Button)e.Item.FindControl("Btn_SCForumClassC_Disabled")).Visible = false;
            ((Button)e.Item.FindControl("Btn_SCForumClassC_Enabled")).Visible = true;
        }
        else if (Status == "1")   //啟用
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "啟用";
            ((Label)e.Item.FindControl("LB_HStatus")).Style.Add("color", "#09af2d");

            ((Button)e.Item.FindControl("Btn_SCForumClassC_Disabled")).Visible = true;
            ((Button)e.Item.FindControl("Btn_SCForumClassC_Enabled")).Visible = false;
        }
        #endregion



        #region 公開顯示
        string gPublicStatus = ((Label)e.Item.FindControl("LB_HPublic")).Text;

        if (Status == "0")  //不公開
        {
            ((Label)e.Item.FindControl("LB_HPublic")).Text = "不公開";
            ((Label)e.Item.FindControl("LB_HPublic")).Style.Add("color", "#d9183c");

        }
        else if (Status == "1")   //公開
        {
            ((Label)e.Item.FindControl("LB_HPublic")).Text = "公開";
            ((Label)e.Item.FindControl("LB_HPublic")).Style.Add("color", "#08c");
        }
        #endregion
    }

    #region 討論區次類別_停用功能
    protected void Btn_SCForumClassB_Disabled_Click(object sender, EventArgs e)
    {
        Button Btn_Disabled = sender as Button;
        int Disabled_CA = Convert.ToInt32(Btn_Disabled.CommandArgument);


        //更新資料庫
        SqlCommand cmd = new SqlCommand("UPDATE HSCForumClass SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
        ConStr.Open();
        cmd.Parameters.AddWithValue("@HStatus", 0);
        cmd.Parameters.AddWithValue("@HID", Disabled_CA);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();


        Rpt_HSCForumClassFirst.DataBind();

    }
    #endregion

    protected void Btn_SCForumClassB_Enabled_Click(object sender, EventArgs e)
    {
        Button Btn_Enabled = sender as Button;
        int Enabled_CA = Convert.ToInt32(Btn_Enabled.CommandArgument);


        //更新資料庫
        SqlCommand cmd = new SqlCommand("UPDATE HSCForumClass SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
        ConStr.Open();
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HID", Enabled_CA);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        Rpt_HSCForumClassFirst.DataBind();
    }


    protected void Btn_SCForumClassC_Disabled_Click(object sender, EventArgs e)
    {
        Button Btn_Disabled = sender as Button;
        int Disabled_CA = Convert.ToInt32(Btn_Disabled.CommandArgument);


        //更新資料庫
        SqlCommand cmd = new SqlCommand("UPDATE HSCForumClass SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
        ConStr.Open();
        cmd.Parameters.AddWithValue("@HStatus", 0);
        cmd.Parameters.AddWithValue("@HID", Disabled_CA);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //抓父層(上一層)的repeater寫法(用NamingContainer)
        var gChildRepeaterItem = (RepeaterItem)Btn_Disabled.NamingContainer;
        var gChildRepeater = (Repeater)gChildRepeaterItem.NamingContainer;
        var gParentRepeaterItem = (RepeaterItem)gChildRepeater.NamingContainer;
        var gParentRepeaterItemIndex = gParentRepeaterItem.ItemIndex;

        Repeater Rpt_HSCForumClassThird = ((Repeater)gChildRepeater.Items[gParentRepeaterItemIndex].FindControl("Rpt_HSCForumClassThird")) as Repeater;

        Rpt_HSCForumClassThird.DataBind();


    }

    protected void Btn_SCForumClassC_Enabled_Click(object sender, EventArgs e)
    {
        Button Btn_Enabled = sender as Button;
        int Enabled_CA = Convert.ToInt32(Btn_Enabled.CommandArgument);


        //更新資料庫
        SqlCommand cmd = new SqlCommand("UPDATE HSCForumClass SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
        ConStr.Open();
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HID", Enabled_CA);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //抓父層(上一層)的repeater寫法(用NamingContainer)
        var gChildRepeaterItem = (RepeaterItem)Btn_Enabled.NamingContainer;
        var gChildRepeater = (Repeater)gChildRepeaterItem.NamingContainer;
        var gParentRepeaterItem = (RepeaterItem)gChildRepeater.NamingContainer;
        var gParentRepeaterItemIndex = gParentRepeaterItem.ItemIndex;

        Repeater Rpt_HSCForumClassThird = ((Repeater)gChildRepeater.Items[gParentRepeaterItemIndex].FindControl("Rpt_HSCForumClassThird")) as Repeater;

        Rpt_HSCForumClassThird.DataBind();

    }

    #region 討論區主類別_停用功能
    protected void Btn_SCForumClassA_Disabled_Click(object sender, EventArgs e)
    {
        Button Btn_Disabled = sender as Button;
        int Disabled_CA = Convert.ToInt32(Btn_Disabled.CommandArgument);


        //更新資料庫
        SqlCommand cmd = new SqlCommand("UPDATE HSCForumClass SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
        ConStr.Open();
        cmd.Parameters.AddWithValue("@HStatus", 0);
        cmd.Parameters.AddWithValue("@HID", Disabled_CA);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();



        Rpt_HSCForumClassFirst.DataBind();



    }
    #endregion


    #region 討論區主類別_啟用功能
    protected void Btn_SCForumClassA_Enabled_Click(object sender, EventArgs e)
    {
        Button Btn_Enabled = sender as Button;
        int Enabled_CA = Convert.ToInt32(Btn_Enabled.CommandArgument);


        //更新資料庫
        SqlCommand cmd = new SqlCommand("UPDATE HSCForumClass SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
        ConStr.Open();
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HID", Enabled_CA);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();


        Rpt_HSCForumClassFirst.DataBind();

    }
    #endregion

    #region 新增討論區
    protected void LBtn_SCForumClass_Add_Click(object sender, EventArgs e)
    {

    }
    #endregion

    protected void DDL_HSCForumClassA_SelectedIndexChanged(object sender, EventArgs e)
    {
        RegScript();
        DDL_HSCForumClassB.Items.Clear();
        DDL_HSCForumClassB.Items.Add(new ListItem("請選擇", "0"));

        if (DDL_HSCForumClassA.SelectedValue != "0")
        {
            //Response.Write("SELECT HID, HSCFCName AS HSCForumClassB, HStatus FROM HSCForumClass WHERE HSCFCLevel='20' AND HSCFCMaster = '" + DDL_HSCForumClassA.SelectedValue + "'  AND HStatus=1 ORDER BY HSort ASC");

            SDS_HSCForumClassB.SelectCommand = "SELECT HID, HSCFCName AS HSCForumClassB, HStatus, HSort FROM HSCForumClass WHERE HSCFCLevel='20' AND HSCFCMaster = '" + DDL_HSCForumClassA.SelectedValue + "'  AND HStatus=1 ORDER BY HSort ASC";
            DDL_HSCForumClassB.DataBind();
        }



    }



    #region 執行js
    public void RegScript()
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$('.js-example-basic-single').select2({closeOnSelect: true,});"), true);//執行js

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS3", ("$('.ListB_Multi').SumoSelect({ search: true, placeholder: '-請選擇-', csvDispCount: 5, });"), true);//執行js

    }
    #endregion

    #region 新增討論區-取消功能
    protected void Btn_CloseModal_Click(object sender, EventArgs e)
    {

    }
    #endregion

    #region  新增討論區-儲存功能
    protected void Btn_Submit_Click(object sender, EventArgs e)
    {
        if (DDL_HSCForumClassA.SelectedValue == "0" || DDL_HSCForumClassB.SelectedValue == "0" || string.IsNullOrEmpty(TB_HSCForumClassC.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('資料填寫不完整!');", true);
            return;
        }

        #region 判斷是否重複

        SqlCommand read = new SqlCommand("SELECT HID FROM HSCForumClass WHERE HSCFCMaster=@HSCFCMaster AND HSCFCName=@HSCFCName AND HSCFCLevel='30' ", ConStr);
        ConStr.Open();
        read.Parameters.AddWithValue("@HSCFCMaster", DDL_HSCForumClassB.SelectedValue);
        read.Parameters.AddWithValue("@HSCFCName", TB_HSCForumClassC.Text.Trim());

        SqlDataReader MyEBF = read.ExecuteReader();

        if (MyEBF.Read())
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('已經有這個討論區名稱囉!');", true);
            return;
        }
        MyEBF.Close();
        ConStr.Close();

        #endregion

        SqlCommand cmd = new SqlCommand("INSERT INTO [HSCForumClass] ( HSCFCMaster, HSCFCName, HSCFCLevel, HPublic, HStatus, HCreate, HCreateDT)VALUES( @HSCFCMaster, @HSCFCName, @HSCFCLevel, @HPublic, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();
        cmd.Parameters.AddWithValue("@HSCFCMaster", DDL_HSCForumClassB.SelectedValue);
        cmd.Parameters.AddWithValue("@HSCFCName", TB_HSCForumClassC.Text.Trim());
        cmd.Parameters.AddWithValue("@HSCFCLevel", "30");
        cmd.Parameters.AddWithValue("@HPublic", RBL_HPublic.SelectedValue);
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //清空
        DDL_HSCForumClassA.SelectedValue = "0";
        DDL_HSCForumClassB.SelectedValue = "0";
        TB_HSCForumClassC.Text = null;

        //外層nestable重新繫結
        Rpt_HSCForumClassFirst.DataBind();

    }
    #endregion

    #region 修改主類別名稱
    protected void LBtn_EditSCForumClassA_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Edit = sender as LinkButton;
        int index = Convert.ToInt32(LBtn_Edit.CommandArgument);
        string gSCForumClassName = LBtn_Edit.CommandName;

        TB_HSCFourmClassName.Text = gSCForumClassName;
        LB_HID.Text = index.ToString();


        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$('#Div_SCForumClassName').modal();"), true);//執行js

    }
    #endregion


    #region 修改次類別名稱
    protected void LBtn_EditSCForumClassB_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Edit = sender as LinkButton;
        int index = Convert.ToInt32(LBtn_Edit.CommandArgument);
        string gSCForumClassName = LBtn_Edit.CommandName;

        TB_HSCFourmClassName.Text = gSCForumClassName;
        LB_HID.Text = index.ToString();


        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$('#Div_SCForumClassName').modal();"), true);//執行js

    }
    #endregion


    #region 修改討論區名稱
    protected void LBtn_EditSCForumClassC_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Edit = sender as LinkButton;
        int index = Convert.ToInt32(LBtn_Edit.CommandArgument);
        string gSCForumClassName = LBtn_Edit.CommandName;

        TB_HSCFourmClassName.Text = gSCForumClassName;
        LB_HID.Text = index.ToString();


        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$('#Div_SCForumClassName').modal();"), true);//執行js

    }
    #endregion

    #region 修改名稱-儲存功能
    protected void Btn_SCForumClassName_Save_Click(object sender, EventArgs e)
    {
        //更新DB
        if (!string.IsNullOrEmpty(TB_HSCFourmClassName.Text.Trim()))
        {
            //資料庫
            SqlCommand cmd = new SqlCommand("UPDATE HSCForumClass SET HSCFCName=@HSCFCName, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
            ConStr.Open();
            cmd.Parameters.AddWithValue("@HID", LB_HID.Text);
            cmd.Parameters.AddWithValue("@HSCFCName", TB_HSCFourmClassName.Text.Trim());
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
        }

        //重新繫結
        Rpt_HSCForumClassFirst.DataBind();

    }
    #endregion


    //sortable更新排序
    #region 更新排序
    protected void Btn_UpdateOrder_Click(object sender, EventArgs e)
    {
        // 從 HiddenField 取得前端序列化後的 JSON
        string json = hfOrder.Value;
        if (!string.IsNullOrEmpty(json))
        {
            // 反序列化成樹狀物件
            List<NodeModel> list = JsonConvert.DeserializeObject<List<NodeModel>>(json);
            // 逐一更新資料庫
            UpdateNodes(list);
        }


        Response.Redirect("HSCForumClassSetting.aspx");
    }
    #endregion

    #region 取消更新
    protected void Btn_Cancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("HSCForumClassSetting.aspx");
    }
    #endregion

    /// <summary>
    ///  遞迴更新每個節點的 Master, Level, Sort
    /// </summary>
    private void UpdateNodes(List<NodeModel> nodes)
    {
        foreach (var node in nodes)
        {
            UpdateSingleNode(node.Id, node.Master, node.Level, node.Sort);
            // 若還有子項，繼續遞迴
            if (node.Children != null && node.Children.Count > 0)
            {
                UpdateNodes(node.Children);
            }
        }
    }

    /// <summary>
    ///  單筆更新
    /// </summary>
    private void UpdateSingleNode(int id, int master, int level, int sort)
    {
        string connString = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        string sql = @"
            UPDATE HSCForumClass 
               SET HSCFCMaster = @HSCFCMaster, 
                   HSCFCLevel  = @HSCFCLevel, 
                   HSort   = @HSort
             WHERE HID = @HID";

        using (SqlConnection conn = new SqlConnection(connString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@HSCFCMaster", master);
            cmd.Parameters.AddWithValue("@HSCFCLevel", level);
            cmd.Parameters.AddWithValue("@HSort", sort);
            cmd.Parameters.AddWithValue("@HID", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }


    }



    protected void Btn_SCFClassASubmit_Click(object sender, EventArgs e)
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

        DDL_HSCForumClassA.Items.Clear();
        DDL_HSCForumClassA.Items.Add(new ListItem("請選擇", "0"));
        DDL_HSCForumClassA.DataBind();


        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$('#Div_SCFourmClassAdd').modal();"), true);//執行js
    }

    protected void Btn_SCFClassBSubmit_Click(object sender, EventArgs e)
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
        TB_HSCFCNameB.Text = null;


        SDS_HSCForumClassB.SelectCommand = "SELECT HID, HSCFCName AS HSCForumClassB, HStatus, HSort FROM HSCForumClass WHERE HSCFCLevel='20' AND HSCFCMaster = '" + DDL_HSCForumClassA.SelectedValue + "'  AND HStatus=1 ORDER BY HSort ASC";
        DDL_HSCForumClassB.DataBind();

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$('#Div_SCFourmClassAdd').modal();"), true);//執行js
    }

    protected void LBtn_AddSCForumClassB_Click(object sender, EventArgs e)
    {
        DDL_HSCFCMasterA.DataBind();
        DDL_HSCForumClassA.DataBind();
        DDL_HSCForumClassB.DataBind();
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$('#Div_SCFourmClassAdd').hide();$('#Div_AddSCForumClassB').modal();"), true);//執行js
    }

    protected void LBtn_AddSCForumClassA_Click(object sender, EventArgs e)
    {

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$('#Div_SCFourmClassAdd').hide();$('#Div_AddSCForumClassA').modal();"), true);//執行js
    }
}



// NodeModel 與前端 JSON 結構相對應
public class NodeModel
{
    public int Id { get; set; }
    public int Master { get; set; }
    public int Level { get; set; }
    public int Sort { get; set; }
    public List<NodeModel> Children { get; set; }
}