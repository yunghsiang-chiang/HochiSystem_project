using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class System_HCCRegular : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    #region 分頁
    private int PageMax = 30;   //分頁最大顯示數量
    private int LastPage = 10;  //分頁數量
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


    string gHDTypeCode = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        RegScript();//執行js

        if (!IsPostBack)
        {
            //AA20241030_新增捐款項目HID
            if (Request.QueryString["HID"] != null)
            {
                DDL_HDPurpose.DataBind();

                SqlDataReader QueryHDItem = SQLdatabase.ExecuteReader("SELECT a.HID, a.HDPurposeID, a.HDItem, b.HDTypeCode FROM HDonationItem AS a LEFT JOIN HDPurpose AS b ON a.HDPurposeID= b.HID WHERE a.HID='" + Request.QueryString["HID"].ToString() + "'");

                if (QueryHDItem.Read())
                {
                    gHDTypeCode = QueryHDItem["HDTypeCode"].ToString();
                    DDL_HDPurpose.SelectedValue = QueryHDItem["HDPurposeID"].ToString();
                }
                QueryHDItem.Close();


                SDS_HDonationItem.SelectCommand = "SELECT HID, HDPurposeID, HDItem FROM HDonationItem WHERE HStatus='1' AND HDPurposeID='" + DDL_HDPurpose.SelectedValue + "'";
                DDL_HDItem.DataBind();
                DDL_HDItem.SelectedValue = Request.QueryString["HID"].ToString();

                LB_HCCPCodeHead.Text = "F" + gHDTypeCode + "EXXX";//預設編碼
                TB_HCCPCodeHeadYear.Text = "XX"; //預設編碼
                TB_HCCPCodeHeadSerial.Text = "X"; //預設編碼
                TB_HCCPCodeHeadYear.Enabled = false;
                TB_HCCPCodeHeadSerial.Enabled = false;
            }


            SDS_HCCRegular.SelectCommand = "SELECT HID, HCCPCodeHead, HDPurposeID, HDPurpose, HDItemID, HDItem, HLimitTotal, HMinTotal, HMinTimes, HMaxTimes, HCCPSDate, HDOpenDate, HDExpDate, HOpenBlessing, HOpenPaper, HStatus FROM HCCRegular ORDER BY HStatus DESC";
            //ORDER BY HStatus DESC, HModifyDT DESC

            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCRegular.SelectCommand, PageMax, LastPage, false, Rpt_HCCRegular);
            ViewState["Search"] = SDS_HCCRegular.SelectCommand;
        }
        else
        {
            SDS_HCCRegular.SelectCommand = ViewState["Search"].ToString();
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", ViewState["Search"].ToString(), PageMax, LastPage, false, Rpt_HCCRegular);
        }
    }

    #region 新增功能
    protected void LBtn_Add_Click(object sender, EventArgs e)
    {

        if (DDL_HDPurpose.SelectedValue == "0" || DDL_HDItem.SelectedValue == "0" || string.IsNullOrEmpty(TB_HMinTotal.Text.Trim()) || string.IsNullOrEmpty(TB_HMinTotal.Text.Trim()) || string.IsNullOrEmpty(TB_HMinTimes.Text.Trim()) /*|| string.IsNullOrEmpty(TB_HDCCPAmount.Text.Trim())*/ || string.IsNullOrEmpty(TB_HMaxTimes.Text.Trim()) || string.IsNullOrEmpty(TB_HDOpenDate.Text.Trim()) || string.IsNullOrEmpty(TB_HDExpDate.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請輸入必填欄位');", true);
            return;
        }


        //AA20241018_新增開放紙本，則紙本扣款起始日為必填
        if (DDL_HOpenPaper.SelectedValue == "1")
        {
            if (string.IsNullOrEmpty(TB_HCCPSDate.Text.Trim()))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('開放紙本授權，要記得填寫紙本起始扣款日哦~');", true);
                return;
            }
        }


        //加入資料庫
        con.Open();

        //AE20241111_新增單號開頭欄位
        SqlCommand cmd = new SqlCommand("INSERT INTO [HCCRegular] (HCCPCodeHead,HDPurposeID, HDPurpose, HDItemID, HDItem, HLimitTotal, HMinTotal, HMinTimes, HMaxTimes, HCCPSDate, HDOpenDate, HDExpDate, HOpenBlessing, HOpenPaper, HStatus, HCreate, HCreateDT)VALUES(@HCCPCodeHead,@HDPurposeID, @HDPurpose, @HDItemID, @HDItem, @HLimitTotal, @HMinTotal, @HMinTimes, @HMaxTimes, @HCCPSDate, @HDOpenDate, @HDExpDate, @HOpenBlessing, @HOpenPaper, @HStatus, @HCreate, @HCreateDT)", con);

        cmd.Parameters.AddWithValue("@HCCPCodeHead", LB_HCCPCodeHead.Text);
        cmd.Parameters.AddWithValue("@HDPurposeID", DDL_HDPurpose.SelectedValue);
        cmd.Parameters.AddWithValue("@HDPurpose", DDL_HDPurpose.SelectedItem.Text);
        cmd.Parameters.AddWithValue("@HDItemID", DDL_HDItem.SelectedValue);
        cmd.Parameters.AddWithValue("@HDItem", DDL_HDItem.SelectedItem.Text);
        //cmd.Parameters.AddWithValue("@HDCCPAmount", TB_HDCCPAmount.Text.Trim());
        cmd.Parameters.AddWithValue("@HLimitTotal", DDL_HLimitTotal.SelectedValue);
        cmd.Parameters.AddWithValue("@HMinTotal", TB_HMinTotal.Text.Trim());
        cmd.Parameters.AddWithValue("@HMinTimes", TB_HMinTimes.Text.Trim());
        cmd.Parameters.AddWithValue("@HMaxTimes", TB_HMaxTimes.Text.Trim());
        cmd.Parameters.AddWithValue("@HCCPSDate", TB_HCCPSDate.Text.Trim());
        cmd.Parameters.AddWithValue("@HDOpenDate", TB_HDOpenDate.Text.Trim());
        cmd.Parameters.AddWithValue("@HDExpDate", TB_HDExpDate.Text.Trim());
        cmd.Parameters.AddWithValue("@HOpenBlessing", DDL_HOpenBlessing.SelectedValue);
        cmd.Parameters.AddWithValue("@HOpenPaper", DDL_HOpenPaper.SelectedValue);
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        con.Close();
        cmd.Cancel();


        //AA20241018_清掉資料
        DDL_HDPurpose.SelectedValue = "0";
        DDL_HDItem.SelectedValue = "0";
        TB_HMinTotal.Text = null;
        DDL_HLimitTotal.SelectedValue = "0";
        TB_HMinTimes.Text = null;
        TB_HMaxTimes.Text = null;
        DDL_HOpenBlessing.SelectedValue = "0";
        DDL_HOpenPaper.SelectedValue = "0";
        TB_HCCPSDate.Text = null;
        TB_HDOpenDate.Text = null;
        TB_HDExpDate.Text = null;
        LB_HCCPCodeHead.Text = null;

        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCRegular.SelectCommand, PageMax, LastPage, true, Rpt_HCCRegular);
        ViewState["Search"] = SDS_HCCRegular.SelectCommand;

    }
    #endregion

    #region 列表資料繫結
    protected void Rpt_HCCRegular_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        #region 狀態-按鈕顯示
        //AE20241030_註解:停用/啟用改成開啟/關閉，控制條件會依開放日期&截止日期
        //if (((Label)e.Item.FindControl("LB_Status")).Text == "0") //停用
        //{
        //    ((LinkButton)e.Item.FindControl("LBtn_Stop")).Visible = false;
        //    ((LinkButton)e.Item.FindControl("LBtn_Upload")).Visible = true;
        //}
        //else if (((Label)e.Item.FindControl("LB_Status")).Text == "1")
        //{
        //    ((LinkButton)e.Item.FindControl("LBtn_Stop")).Visible = true;
        //    ((LinkButton)e.Item.FindControl("LBtn_Upload")).Visible = false;
        //}
        #endregion

        #region 狀態顯示
        //AE20241030_判斷條件改成:依開放日期&截止日期顯示狀態為開啟或關閉。
        //string Status = ((Label)e.Item.FindControl("LB_Status")).Text;

        HtmlGenericControl StatusType = e.Item.FindControl("Status") as HtmlGenericControl;

        DateTime Today = DateTime.Now;
        DateTime HDOpenDate = Convert.ToDateTime(((Label)e.Item.FindControl("LB_HDOpenDate")).Text);
        DateTime HDExpDate = Convert.ToDateTime(((Label)e.Item.FindControl("LB_HDExpDate")).Text);

        //判斷今天落在此日期區間
        if (Today.CompareTo(HDOpenDate) >= 0 && Today.CompareTo(HDExpDate) <= 0)
        {
            ((Label)e.Item.FindControl("LB_Status")).Text = "開放中";
            StatusType.Style.Add("color", "#09af2d");
        }
        else   //關閉
        {
            ((Label)e.Item.FindControl("LB_Status")).Text = "關閉中";
            StatusType.Style.Add("color", "#de4848");

        }
        #endregion


        #region 
        if (((Label)e.Item.FindControl("LB_HOpenBlessing")).Text == "0")  //否
        {
            ((Label)e.Item.FindControl("LB_HOpenBlessing")).Text = "否";
        }
        else
        {
            ((Label)e.Item.FindControl("LB_HOpenBlessing")).Text = "是";
        }

        if (((Label)e.Item.FindControl("LB_HOpenPaper")).Text == "0")  //否
        {
            ((Label)e.Item.FindControl("LB_HOpenPaper")).Text = "否";
        }
        else
        {
            ((Label)e.Item.FindControl("LB_HOpenPaper")).Text = "是";
        }
        #endregion


        #region 選單
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HDPurposeID")).Text))
        {
            ((DropDownList)e.Item.FindControl("DDL_HDPurpose1")).SelectedValue = ((Label)e.Item.FindControl("LB_HDPurposeID")).Text;
        }

        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HDItemID")).Text))
        {
            ((DropDownList)e.Item.FindControl("DDL_HDItem")).SelectedValue = ((Label)e.Item.FindControl("LB_HDItemID")).Text;
        }

        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HLimitTotal")).Text))
        {
            if (((Label)e.Item.FindControl("LB_HLimitTotal")).Text == "0")
            {
                ((DropDownList)e.Item.FindControl("DDL_HLimitTotal")).SelectedValue = ((Label)e.Item.FindControl("LB_HLimitTotal")).Text;
                ((Label)e.Item.FindControl("LB_HLimitTotal")).Text = "不限制";
            }
            else if (((Label)e.Item.FindControl("LB_HLimitTotal")).Text == "1")
            {
                ((DropDownList)e.Item.FindControl("DDL_HLimitTotal")).SelectedValue = ((Label)e.Item.FindControl("LB_HLimitTotal")).Text;
                ((Label)e.Item.FindControl("LB_HLimitTotal")).Text = "限制";
            }
        }
        #endregion

    }
    #endregion

    #region 編輯功能
    protected void LBtn_Edit_Click(object sender, EventArgs e)
    {
        //非同步
        RegisterAsync();

        LinkButton LBtn_Edit = sender as LinkButton;
        int index = Convert.ToInt32(LBtn_Edit.CommandArgument);

        ((Label)Rpt_HCCRegular.Items[index].FindControl("LB_HDPurpose")).Visible = false;
        ((Label)Rpt_HCCRegular.Items[index].FindControl("LB_HDItem")).Visible = false;
        //((Label)Rpt_HCCRegular.Items[index].FindControl("LB_HDCCPAmount")).Visible = false;
        ((Label)Rpt_HCCRegular.Items[index].FindControl("LB_HLimitTotal")).Visible = false;
        ((Label)Rpt_HCCRegular.Items[index].FindControl("LB_HMinTotal")).Visible = false;
        ((Label)Rpt_HCCRegular.Items[index].FindControl("LB_HMinTimes")).Visible = false;
        ((Label)Rpt_HCCRegular.Items[index].FindControl("LB_HMaxTimes")).Visible = false;
        ((Label)Rpt_HCCRegular.Items[index].FindControl("LB_HCCPSDate")).Visible = false;
        ((Label)Rpt_HCCRegular.Items[index].FindControl("LB_HDOpenDate")).Visible = false;
        ((Label)Rpt_HCCRegular.Items[index].FindControl("LB_HDExpDate")).Visible = false;
        ((Label)Rpt_HCCRegular.Items[index].FindControl("LB_HOpenBlessing")).Visible = false;
        ((Label)Rpt_HCCRegular.Items[index].FindControl("LB_HOpenPaper")).Visible = false;

        ((DropDownList)Rpt_HCCRegular.Items[index].FindControl("DDL_HDPurpose1")).Visible = true;
        ((DropDownList)Rpt_HCCRegular.Items[index].FindControl("DDL_HDItem")).Visible = true;
        //((TextBox)Rpt_HCCRegular.Items[index].FindControl("TB_HDCCPAmount")).Visible = true;
        ((DropDownList)Rpt_HCCRegular.Items[index].FindControl("DDL_HLimitTotal")).Visible = true;
        ((TextBox)Rpt_HCCRegular.Items[index].FindControl("TB_HMinTotal")).Visible = true;
        ((TextBox)Rpt_HCCRegular.Items[index].FindControl("TB_HMinTimes")).Visible = true;
        ((TextBox)Rpt_HCCRegular.Items[index].FindControl("TB_HMaxTimes")).Visible = true;
        ((TextBox)Rpt_HCCRegular.Items[index].FindControl("TB_HCCPSDate")).Visible = true;
        ((TextBox)Rpt_HCCRegular.Items[index].FindControl("TB_HDOpenDate")).Visible = true;
        ((TextBox)Rpt_HCCRegular.Items[index].FindControl("TB_HDExpDate")).Visible = true;
        ((DropDownList)Rpt_HCCRegular.Items[index].FindControl("DDL_HOpenBlessing")).Visible = true;
        ((DropDownList)Rpt_HCCRegular.Items[index].FindControl("DDL_HOpenPaper")).Visible = true;

        ((LinkButton)Rpt_HCCRegular.Items[index].FindControl("LBtn_Edit")).Visible = false;
        ((LinkButton)Rpt_HCCRegular.Items[index].FindControl("LBtn_Save")).Visible = true;
        ((LinkButton)Rpt_HCCRegular.Items[index].FindControl("LBtn_EditCode")).Visible = true;
        
        for (int i = 0; i < Rpt_HCCRegular.Items.Count; i++)
        {
            if (i != index)
            {
                ((Label)Rpt_HCCRegular.Items[i].FindControl("LB_HDPurpose")).Visible = true;
                ((Label)Rpt_HCCRegular.Items[i].FindControl("LB_HDItem")).Visible = true;
                //((Label)Rpt_HCCRegular.Items[i].FindControl("LB_HDCCPAmount")).Visible = true;
                ((Label)Rpt_HCCRegular.Items[i].FindControl("LB_HLimitTotal")).Visible = true;
                ((Label)Rpt_HCCRegular.Items[i].FindControl("LB_HMinTotal")).Visible = true;
                ((Label)Rpt_HCCRegular.Items[i].FindControl("LB_HMinTimes")).Visible = true;
                ((Label)Rpt_HCCRegular.Items[i].FindControl("LB_HMaxTimes")).Visible = true;
                ((Label)Rpt_HCCRegular.Items[i].FindControl("LB_HDOpenDate")).Visible = true;
                ((Label)Rpt_HCCRegular.Items[i].FindControl("LB_HDExpDate")).Visible = true;
                ((Label)Rpt_HCCRegular.Items[i].FindControl("LB_HOpenBlessing")).Visible = true;
                ((Label)Rpt_HCCRegular.Items[i].FindControl("LB_HOpenPaper")).Visible = true;



                ((DropDownList)Rpt_HCCRegular.Items[i].FindControl("DDL_HDPurpose1")).Visible = false;
                ((DropDownList)Rpt_HCCRegular.Items[i].FindControl("DDL_HDItem")).Visible = false;
                //((TextBox)Rpt_HCCRegular.Items[i].FindControl("TB_HDCCPAmount")).Visible = false;
                ((DropDownList)Rpt_HCCRegular.Items[i].FindControl("DDL_HLimitTotal")).Visible = false;
                ((TextBox)Rpt_HCCRegular.Items[i].FindControl("TB_HMinTotal")).Visible = false;
                ((TextBox)Rpt_HCCRegular.Items[i].FindControl("TB_HMinTimes")).Visible = false;
                ((TextBox)Rpt_HCCRegular.Items[i].FindControl("TB_HMaxTimes")).Visible = false;
                ((TextBox)Rpt_HCCRegular.Items[i].FindControl("TB_HDOpenDate")).Visible = false;
                ((TextBox)Rpt_HCCRegular.Items[i].FindControl("TB_HDExpDate")).Visible = false;
                ((DropDownList)Rpt_HCCRegular.Items[i].FindControl("DDL_HOpenBlessing")).Visible = false;
                ((DropDownList)Rpt_HCCRegular.Items[i].FindControl("DDL_HOpenPaper")).Visible = false;

                ((LinkButton)Rpt_HCCRegular.Items[i].FindControl("LBtn_Save")).Visible = false;
                ((LinkButton)Rpt_HCCRegular.Items[i].FindControl("LBtn_Edit")).Visible = true;
                ((LinkButton)Rpt_HCCRegular.Items[i].FindControl("LBtn_EditCode")).Visible = false;
            }

        }

        if (((DropDownList)Rpt_HCCRegular.Items[index].FindControl("DDL_HDPurpose1")).SelectedValue != "0")
        {
            ((DropDownList)Rpt_HCCRegular.Items[index].FindControl("DDL_HDItem")).Items.Clear();
            SDS_HDonationItem1.SelectCommand = "SELECT HID, HDPurposeID, HDItem FROM HDonationItem WHERE HStatus='1' AND HDPurposeID='" + ((DropDownList)Rpt_HCCRegular.Items[index].FindControl("DDL_HDPurpose1")).SelectedValue + "'";
            SDS_HDonationItem1.DataBind();
            ((DropDownList)Rpt_HCCRegular.Items[index].FindControl("DDL_HDItem")).DataBind();
            ((DropDownList)Rpt_HCCRegular.Items[index].FindControl("DDL_HDItem")).SelectedValue = ((Label)Rpt_HCCRegular.Items[index].FindControl("LB_HDItemID")).Text;
        }
        else
        {
            ((DropDownList)Rpt_HCCRegular.Items[index].FindControl("DDL_HDItem")).Items.Add(new ListItem("請選擇", "0"));
            ((DropDownList)Rpt_HCCRegular.Items[index].FindControl("DDL_HDItem")).DataBind();
        }


        //AA20241018_新增判斷:當開放紙本授權為否，則紙本扣款起始日不能改
        if (((DropDownList)Rpt_HCCRegular.Items[index].FindControl("DDL_HOpenPaper")).SelectedValue == "0")
        {
            ((TextBox)Rpt_HCCRegular.Items[index].FindControl("TB_HCCPSDate")).Enabled = false;
        }
        else if (((DropDownList)Rpt_HCCRegular.Items[index].FindControl("DDL_HOpenPaper")).SelectedValue == "1")
        {
            ((TextBox)Rpt_HCCRegular.Items[index].FindControl("TB_HCCPSDate")).Enabled = true;
        }


        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCRegular.SelectCommand, PageMax, LastPage, false, Rpt_HCCRegular);
        ViewState["Search"] = SDS_HCCRegular.SelectCommand;
    }
    #endregion

    #region 編輯儲存功能
    protected void LBtn_Save_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Save = sender as LinkButton;
        int Save_CA = Convert.ToInt32(LBtn_Save.CommandArgument);

        if (((DropDownList)Rpt_HCCRegular.Items[Save_CA].FindControl("DDL_HDPurpose1")).SelectedValue == "0" || ((DropDownList)Rpt_HCCRegular.Items[Save_CA].FindControl("DDL_HDItem")).SelectedValue == "0" || string.IsNullOrEmpty(((DropDownList)Rpt_HCCRegular.Items[Save_CA].FindControl("DDL_HDItem")).Text.Trim()) || string.IsNullOrEmpty(((TextBox)Rpt_HCCRegular.Items[Save_CA].FindControl("TB_HMinTotal")).Text.Trim()) || string.IsNullOrEmpty(((TextBox)Rpt_HCCRegular.Items[Save_CA].FindControl("TB_HMinTimes")).Text.Trim()) || string.IsNullOrEmpty(((TextBox)/*Rpt_HCCRegular.Items[Save_CA].FindControl("TB_HDCCPAmount")).Text.Trim()) || string.IsNullOrEmpty(((TextBox)*/Rpt_HCCRegular.Items[Save_CA].FindControl("TB_HMaxTimes")).Text.Trim()) || string.IsNullOrEmpty(((TextBox)Rpt_HCCRegular.Items[Save_CA].FindControl("TB_HDOpenDate")).Text.Trim()) || string.IsNullOrEmpty(((TextBox)Rpt_HCCRegular.Items[Save_CA].FindControl("TB_HDExpDate")).Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請輸入必填欄位');", true);
            return;
        }


        //AA20241018_新增開放紙本，則紙本扣款起始日為必填
        if (((DropDownList)Rpt_HCCRegular.Items[Save_CA].FindControl("DDL_HOpenPaper")).SelectedValue == "1")
        {
            if (string.IsNullOrEmpty(((TextBox)Rpt_HCCRegular.Items[Save_CA].FindControl("TB_HCCPSDate")).Text.Trim()))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('開放紙本授權，要記得填寫紙本起始扣款日哦~');", true);
                return;
            }
        }

        //資料庫
        SqlCommand cmd = new SqlCommand("UPDATE [HCCRegular] SET HCCPCodeHead=@HCCPCodeHead, HDPurposeID=@HDPurposeID, HDPurpose=@HDPurpose, HDItemID=@HDItemID, HDItem=@HDItem, HLimitTotal=@HLimitTotal, HMinTotal=@HMinTotal, HMinTimes=@HMinTimes, HMaxTimes=@HMaxTimes, HCCPSDate=@HCCPSDate, HDOpenDate=@HDOpenDate, HDExpDate=@HDExpDate, HOpenBlessing=@HOpenBlessing, HOpenPaper=@HOpenPaper, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", con);
        //, HStatus=@HStatus
        con.Open();
        cmd.Parameters.AddWithValue("@HCCPCodeHead", ((Label)Rpt_HCCRegular.Items[Save_CA].FindControl("LB_HCCPCodeHead")).Text);
        cmd.Parameters.AddWithValue("@HID", ((Label)Rpt_HCCRegular.Items[Save_CA].FindControl("LB_HID")).Text);
        cmd.Parameters.AddWithValue("@HDPurposeID", ((DropDownList)Rpt_HCCRegular.Items[Save_CA].FindControl("DDL_HDPurpose1")).SelectedValue);
        cmd.Parameters.AddWithValue("@HDPurpose", ((DropDownList)Rpt_HCCRegular.Items[Save_CA].FindControl("DDL_HDPurpose1")).SelectedItem.Text);
        cmd.Parameters.AddWithValue("@HDItemID", ((DropDownList)Rpt_HCCRegular.Items[Save_CA].FindControl("DDL_HDItem")).SelectedValue);
        cmd.Parameters.AddWithValue("@HDItem", ((DropDownList)Rpt_HCCRegular.Items[Save_CA].FindControl("DDL_HDItem")).SelectedItem.Text);
        //cmd.Parameters.AddWithValue("@HDCCPAmount", ((TextBox)Rpt_HCCRegular.Items[Save_CA].FindControl("TB_HDCCPAmount")).Text.Trim());
        cmd.Parameters.AddWithValue("@HLimitTotal", ((DropDownList)Rpt_HCCRegular.Items[Save_CA].FindControl("DDL_HLimitTotal")).SelectedValue);
        cmd.Parameters.AddWithValue("@HMinTotal", ((TextBox)Rpt_HCCRegular.Items[Save_CA].FindControl("TB_HMinTotal")).Text.Trim());
        cmd.Parameters.AddWithValue("@HMinTimes", ((TextBox)Rpt_HCCRegular.Items[Save_CA].FindControl("TB_HMinTimes")).Text.Trim());
        cmd.Parameters.AddWithValue("@HMaxTimes", ((TextBox)Rpt_HCCRegular.Items[Save_CA].FindControl("TB_HMaxTimes")).Text.Trim());
        cmd.Parameters.AddWithValue("@HCCPSDate", ((TextBox)Rpt_HCCRegular.Items[Save_CA].FindControl("TB_HCCPSDate")).Text.Trim());
        cmd.Parameters.AddWithValue("@HDOpenDate", ((TextBox)Rpt_HCCRegular.Items[Save_CA].FindControl("TB_HDOpenDate")).Text.Trim());
        cmd.Parameters.AddWithValue("@HDExpDate", ((TextBox)Rpt_HCCRegular.Items[Save_CA].FindControl("TB_HDExpDate")).Text.Trim());
        cmd.Parameters.AddWithValue("@HOpenBlessing", ((DropDownList)Rpt_HCCRegular.Items[Save_CA].FindControl("DDL_HOpenBlessing")).SelectedValue);
        cmd.Parameters.AddWithValue("@HOpenPaper", ((DropDownList)Rpt_HCCRegular.Items[Save_CA].FindControl("DDL_HOpenPaper")).SelectedValue);
        //cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        con.Close();
        cmd.Cancel();

        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCRegular.SelectCommand, PageMax, LastPage, true, Rpt_HCCRegular);
        ViewState["Search"] = SDS_HCCRegular.SelectCommand;

    }
    #endregion

    #region 搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        string QuerySql = "SELECT HID, HCCPCodeHead, HDPurposeID, HDPurpose, HDItemID, HDItem, HLimitTotal, HMinTotal, HMinTimes, HMaxTimes, HCCPSDate, HDOpenDate, HDExpDate, HOpenBlessing, HOpenPaper, HStatus FROM HCCRegular ";

        //搜尋條件
        StringBuilder sql = new StringBuilder(QuerySql);
        List<string> WHERE = new List<string>();

        //捐款用途
        if (DDL_SHDPurpose.SelectedValue != "0")
        {
            WHERE.Add("  (HDPurposeID ='" + DDL_SHDPurpose.SelectedValue + "') ");
        }

        //捐款項目
        if (DDL_SHDItem.SelectedValue != "0")
        {
            WHERE.Add("  (HDPurposeID ='" + DDL_SHDItem.SelectedValue + "') ");
        }


        //判斷使用者是否選擇了條件
        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" WHERE " + wh);
        }

        SDS_HCCRegular.SelectCommand = sql.ToString();
        //+ " ORDER BY HStatus DESC, HModifyDT DESC"
        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCRegular.SelectCommand, PageMax, LastPage, true, Rpt_HCCRegular);
        ViewState["Search"] = SDS_HCCRegular.SelectCommand;

    }
    #endregion

    #region 取消搜尋功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        SDS_HDonationItem.DataBind();
        DDL_SHDItem.Items.Clear();
        DDL_SHDItem.Items.Add(new ListItem("請選擇", "0"));
        DDL_SHDItem.DataBind();

        DDL_SHDPurpose.SelectedValue = "0";
        DDL_SHDItem.SelectedValue = "0";

        SDS_HCCRegular.SelectCommand = "SELECT HID, HCCPCodeHead, HDPurposeID, HDPurpose, HDItemID, HDItem, HLimitTotal, HMinTotal, HMinTimes, HMaxTimes, HCCPSDate,HDOpenDate, HDExpDate, HOpenBlessing, HOpenPaper, HStatus FROM HCCRegular";
        //ORDER BY HStatus DESC, HModifyDT DESC

        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCRegular.SelectCommand, PageMax, LastPage, true, Rpt_HCCRegular);
        ViewState["Search"] = SDS_HCCRegular.SelectCommand;
    }
    #endregion

    #region 啟用功能
    protected void LBtn_Upload_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Upload = sender as LinkButton;
        int Upload_CA = Convert.ToInt32(LBtn_Upload.CommandArgument);

        //更新資料庫
        SqlCommand cmd = new SqlCommand("UPDATE [HCCRegular] SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", con);
        con.Open();
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HID", Upload_CA);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        con.Close();
        cmd.Cancel();

        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCRegular.SelectCommand, PageMax, LastPage, true, Rpt_HCCRegular);
        ViewState["Search"] = SDS_HCCRegular.SelectCommand;
    }
    #endregion

    #region 停用功能
    protected void LBtn_Stop_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Stop = sender as LinkButton;
        int Stop_CA = Convert.ToInt32(LBtn_Stop.CommandArgument);

        //更新資料庫
        SqlCommand cmd = new SqlCommand("UPDATE [HCCRegular] SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", con);
        con.Open();
        cmd.Parameters.AddWithValue("@HStatus", 0);
        cmd.Parameters.AddWithValue("@HID", Stop_CA);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        con.Close();
        cmd.Cancel();

        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCRegular.SelectCommand, PageMax, LastPage, true, Rpt_HCCRegular);
        ViewState["Search"] = SDS_HCCRegular.SelectCommand;

    }
    #endregion

    #region 執行js
    public void RegScript()
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$('.js-example-basic-single').select2({closeOnSelect: true,});"), true);//執行js

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "Datepicker", ("$('.datepicker').datepicker({format: 'yyyy/mm/dd',todayHighlight: true,orientation: 'bottom auto',autoclose: true,});"), true);
    }
    #endregion

    #region 捐款用途(下拉選單)
    protected void DDL_HDPurpose_SelectedIndexChanged(object sender, EventArgs e)
    {
        DDL_HDItem.Items.Clear();
        DDL_HDItem.Items.Add(new ListItem("請選擇", "0"));
        //DDL_HDItem.DataBind();

        if (DDL_HDPurpose.SelectedValue != "0")
        {
            SDS_HDonationItem.SelectCommand = "SELECT HID, HDPurposeID, HDItem FROM HDonationItem WHERE HStatus='1' AND HDPurposeID='" + DDL_HDPurpose.SelectedValue + "'";
            SDS_HDonationItem.DataBind();
            DDL_HDItem.DataBind();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇捐款用途');", true);
            return;
        }
    }

    protected void DDL_HDPurpose1_SelectedIndexChanged(object sender, EventArgs e)
    {
        RepeaterItem HDPurpose1item = (sender as DropDownList).NamingContainer as RepeaterItem;

        if (((DropDownList)HDPurpose1item.FindControl("DDL_HDPurpose1")).SelectedValue != "0")
        {
            ((DropDownList)HDPurpose1item.FindControl("DDL_HDItem")).Items.Clear();
            SDS_HDonationItem1.SelectCommand = "SELECT HID, HDPurposeID, HDItem FROM HDonationItem WHERE HStatus='1' AND HDPurposeID='" + ((DropDownList)HDPurpose1item.FindControl("DDL_HDPurpose1")).SelectedValue + "'";
            SDS_HDonationItem1.DataBind();
            ((DropDownList)HDPurpose1item.FindControl("DDL_HDItem")).DataBind();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇捐款用途');", true);
            return;
        }
    }

    #endregion


    #region 開放紙本授權切換
    //AA20241018_開放紙本授權判斷
    protected void DDL_HOpenPaper_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (DDL_HDItem.SelectedValue != "0")
        //{
        //    SqlDataReader QueryHDItem = SQLdatabase.ExecuteReader("SELECT a.HID, a.HDPurposeID, a.HDItem, b.HDTypeCode FROM HDonationItem AS a JOIN HDPurpose AS b ON a.HDPurposeID=b.HID WHERE a.HID='" + DDL_HDItem.SelectedValue + "'");

        //    if (QueryHDItem.Read())
        //    {
        //        gHDTypeCode = QueryHDItem["HDTypeCode"].ToString();
        //    }
        //    QueryHDItem.Close();
        //}

        //if (DDL_HOpenPaper.SelectedValue == "1")
        //{
        //    TB_HCCPSDate.Enabled = true;
        //    Star.Visible = true;

        //    //AA20241030_新增單號開頭的編碼規則判斷
        //    LB_HCCPCodeHead.Text = "F" + gHDTypeCode + "P";
        //    TB_HCCPCodeHeadYear.Text = null;
        //    TB_HCCPCodeHeadSerial.Text = null;
        //    TB_HCCPCodeHeadYear.Enabled = true;
        //    TB_HCCPCodeHeadSerial.Enabled = true;


        //}
        //else
        //{
        //    TB_HCCPSDate.Enabled = false;
        //    TB_HCCPSDate.Text = null;
        //    Star.Visible = false;

        //    //AA20241030_新增單號開頭的編碼規則判斷
        //    LB_HCCPCodeHead.Text = "F" + gHDTypeCode + "EXXX";//預設編碼
        //    TB_HCCPCodeHeadYear.Text = "XX"; //預設編碼
        //    TB_HCCPCodeHeadSerial.Text = "X"; //預設編碼
        //    TB_HCCPCodeHeadYear.Enabled = false;
        //    TB_HCCPCodeHeadSerial.Enabled = false;

        //}

        if (DDL_HOpenPaper.SelectedValue == "1")
        {
            TB_HCCPSDate.Enabled = true;
        }
        
    }
    #endregion


    #region   開放紙本授權切換
    //AA20241018_新增開放紙本判斷
    protected void DDL_HOpenPaper_SelectedIndexChanged1(object sender, EventArgs e)
    {
        // 取得觸發事件的 DropDownList
        DropDownList DDL_HOpenPaper = (DropDownList)sender;

        // 取得該 DropDownList 所屬的 RepeaterItem
        RepeaterItem item = (RepeaterItem)DDL_HOpenPaper.NamingContainer;

        // 取得選擇的值
        string selectedValue = DDL_HOpenPaper.SelectedValue;

        // 假設還有其他控制項，比如 Label，可以這樣取得
        TextBox TB_HCCPSDate = (TextBox)item.FindControl("TB_HCCPSDate");

        if (selectedValue == "1")
        {
            TB_HCCPSDate.Enabled = true;
        }
        else
        {
            TB_HCCPSDate.Enabled = false;
            TB_HCCPSDate.Text = null;
        }
    }
    #endregion


    protected void DDL_HDItem_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (DDL_HDItem.SelectedValue != "0")
        //{
        //    SqlDataReader QueryHDItem = SQLdatabase.ExecuteReader("SELECT a.HID, a.HDPurposeID, a.HDItem, b.HDTypeCode FROM HDonationItem AS a JOIN HDPurpose AS b ON a.HDPurposeID=b.HID WHERE a.HID='" + DDL_HDItem.SelectedValue + "'");

        //    if (QueryHDItem.Read())
        //    {
        //        gHDTypeCode = QueryHDItem["HDTypeCode"].ToString();
        //    }
        //    QueryHDItem.Close();
        //}
        //string gPE = "";

        //if (DDL_HOpenPaper.SelectedValue == "1")
        //{
        //    gPE = "P";
        //    TB_HCCPCodeHeadYear.Text = null; //預設編碼
        //    TB_HCCPCodeHeadSerial.Text = null; //預設編碼
        //    TB_HCCPCodeHeadYear.Enabled = true;
        //    TB_HCCPCodeHeadSerial.Enabled = true;
        //}
        //else
        //{
        //    gPE = "EXXX";
        //    TB_HCCPCodeHeadYear.Text = "XX"; //預設編碼
        //    TB_HCCPCodeHeadSerial.Text = "X"; //預設編碼
        //    TB_HCCPCodeHeadYear.Enabled = false;
        //    TB_HCCPCodeHeadSerial.Enabled = false;
        //}

        //LB_HCCPCodeHead.Text = "F" + gHDTypeCode + gPE;//預設編碼



    }

    #region 刪除功能
    protected void LBtn_Del_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Del = sender as LinkButton;
        int Del_CA = Convert.ToInt32(LBtn_Del.CommandArgument);

        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;


        SqlDataReader QueryCCPeriod = SQLdatabase.ExecuteReader("SELECT HID, HCCRegularID, HDPurpose, HDItem FROM HCCPeriod WHERE HCCRegularID='" + Del_CA + "' AND HStatus=1");
        if (QueryCCPeriod.HasRows)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('已有人申請捐款，所以不能做刪除囉!');", true);
            return;
        }
        QueryCCPeriod.Close();

        //AA20241111_先判斷是否已有人捐款或已有填寫信用卡授權書，若有就不能刪除
        SqlDataReader QueryDonate = SQLdatabase.ExecuteReader("SELECT HID FROM HCCPOrderDetail WHERE HDPurpose=N'" + ((Label)RI.FindControl("LB_HDPurpose")).Text + "' AND HDItem=N'" + ((Label)RI.FindControl("LB_HDItem")).Text + "' AND HStatus=1");

        if (QueryDonate.HasRows)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('已有人申請捐款，所以不能做刪除囉!');", true);
            return;
        }
        else
        {
            //更新資料庫
            SqlCommand cmd = new SqlCommand("DELETE FROM [HCCRegular] WHERE HID =@HID", con);
            con.Open();
            cmd.Parameters.AddWithValue("@HID", Del_CA);
            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Cancel();

        }
        QueryDonate.Close();


        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCRegular.SelectCommand, PageMax, LastPage, true, Rpt_HCCRegular);
        ViewState["Search"] = SDS_HCCRegular.SelectCommand;
    }
    #endregion


    #region 儲存單號開頭編碼
    //AA20241108_新增
    protected void LBtn_ECodeSubmit_Click(object sender, EventArgs e)
    {

        string gHCCPCodeHead = "";

        if (LB_SubmitType.Text == "1")  //新增單號開頭編號
        {
            gHCCPCodeHead = TB_HPayAccount.Text.Trim() + TB_HDTypeCode.Text.Trim() + TB_Type.Text.Trim() + TB_HCCPCodeHeadYear.Text.Trim() + TB_HCCPCodeHeadSerial.Text.Trim();
            //將編碼帶入列表
            LB_HCCPCodeHead.Text = gHCCPCodeHead;
        }
        else if(LB_SubmitType.Text == "2")   //編輯單號開頭編號
        {
            gHCCPCodeHead = TB_HPayAccount.Text.Trim() + TB_HDTypeCode.Text.Trim() + TB_Type.Text.Trim() + TB_HCCPCodeHeadYear.Text.Trim() + TB_HCCPCodeHeadSerial.Text.Trim();


            ((Label)Rpt_HCCRegular.Items[Convert.ToInt32(LB_RptNum.Text)].FindControl("LB_HCCPCodeHead")).Text = gHCCPCodeHead;

        }

        //關閉modal
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#AddCode').modal('hide');</script>", false);//執行js

    }
    #endregion

    #region 建立單號開頭編碼
    //AA20241108_新增
    protected void LBtn_AddCode_Click(object sender, EventArgs e)
    {
        //1=新增單號開頭編號；2=修改單號開頭編號
        LB_SubmitType.Text = "1";

        if (DDL_HDPurpose.SelectedValue != "0" && DDL_HDItem.SelectedValue != "0")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#AddCode').modal('show');</script>", false);//執行js

            //顯示Modal裡的資料
            //取得捐款用途代碼
            SqlDataReader QueryHDTypeCode = SQLdatabase.ExecuteReader("SELECT a.HID, a.HDTypeCode FROM HDPurpose AS a WHERE a.HID='" + DDL_HDPurpose.SelectedValue + "'");

            if (QueryHDTypeCode.Read())
            {
                TB_HDTypeCode.Text = QueryHDTypeCode["HDTypeCode"].ToString();
            }
            QueryHDTypeCode.Close();

            if (DDL_HOpenPaper.SelectedValue == "1")
            {
                TB_Type.Text = "P";

                TB_HCCPCodeHeadYear.Enabled = true;
                TB_HCCPCodeHeadSerial.Enabled = true;

                //判斷是否已輸入年度與續報
                if (!string.IsNullOrEmpty(LB_HCCPCodeHead.Text) && LB_HCCPCodeHead.Text.Length == 7)
                {
                    TB_HCCPCodeHeadYear.Text = LB_HCCPCodeHead.Text.Substring(4, 2);
                    TB_HCCPCodeHeadSerial.Text = LB_HCCPCodeHead.Text.Substring(6);
                }
                else
                {
                    TB_HCCPCodeHeadYear.Text = null;
                    TB_HCCPCodeHeadSerial.Text = null;

                }


            }
            else
            {
                TB_Type.Text = "E";
                TB_HCCPCodeHeadYear.Text = "XX";
                TB_HCCPCodeHeadSerial.Text = "X";
                TB_HCCPCodeHeadYear.Enabled = false;
                TB_HCCPCodeHeadSerial.Enabled = false;
            }



        }
        else
        {

            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('捐款用途、捐款項目未填寫完整哦~!');", true);
            return;
        }
    }
    #endregion



    #region 修改單號開頭編碼
    protected void LBtn_EditCode_Click(object sender, EventArgs e)
    {
        //1=新增單號開頭編號；2=修改單號開頭編號
        LB_SubmitType.Text = "2";

        LinkButton LBtn_EditCode = sender as LinkButton;
        string gEditCode_CA = LBtn_EditCode.CommandArgument;
        string gEditCode_CN = LBtn_EditCode.CommandName;  //Repeater第幾筆

        LB_RptNum.Text = gEditCode_CN;

        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        if (((DropDownList)RI.FindControl("DDL_HDPurpose1")).SelectedValue != "0" && ((DropDownList)RI.FindControl("DDL_HDItem")).SelectedValue != "0")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#AddCode').modal('show');</script>", false);//執行js

            //顯示Modal裡的資料
            //取得捐款用途代碼
            SqlDataReader QueryHDTypeCode = SQLdatabase.ExecuteReader("SELECT a.HID, a.HDTypeCode FROM HDPurpose AS a WHERE a.HID='" + ((DropDownList)RI.FindControl("DDL_HDPurpose1")).SelectedValue + "'");

            if (QueryHDTypeCode.Read())
            {
                TB_HDTypeCode.Text = QueryHDTypeCode["HDTypeCode"].ToString();
            }
            QueryHDTypeCode.Close();

            if (((DropDownList)RI.FindControl("DDL_HOpenPaper")).SelectedValue == "1")
            {
                TB_Type.Text = "P";

                TB_HCCPCodeHeadYear.Enabled = true;
                TB_HCCPCodeHeadSerial.Enabled = true;

                //判斷是否已輸入年度與續報
                if (!string.IsNullOrEmpty(((Label)RI.FindControl("LB_HCCPCodeHead")).Text) && ((Label)RI.FindControl("LB_HCCPCodeHead")).Text.Length == 7)
                {
                    TB_HCCPCodeHeadYear.Text = ((Label)RI.FindControl("LB_HCCPCodeHead")).Text.Substring(4, 2);
                    TB_HCCPCodeHeadSerial.Text = ((Label)RI.FindControl("LB_HCCPCodeHead")).Text.Substring(6);
                }
                else
                {
                    TB_HCCPCodeHeadYear.Text = null;
                    TB_HCCPCodeHeadSerial.Text = null;

                }


            }
            else
            {
                TB_Type.Text = "E";
                TB_HCCPCodeHeadYear.Text = "XX";
                TB_HCCPCodeHeadSerial.Text = "X";
                TB_HCCPCodeHeadYear.Enabled = false;
                TB_HCCPCodeHeadSerial.Enabled = false;
            }



        }
        else
        {

            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('捐款用途、捐款項目未填寫完整哦~!');", true);
            return;
        }
    }
    #endregion

    #region 捐款用途搜尋關聯
    protected void DDL_SHDPurpose_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DDL_SHDPurpose.SelectedValue != "0")
        {
            DDL_SHDItem.Items.Clear();
            DDL_SHDItem.Items.Add(new ListItem("請選擇", "0"));
            SDS_HDonationItem.SelectCommand = "SELECT HID, HDPurposeID, HDItem FROM HDonationItem WHERE HStatus='1' AND HDPurposeID='" + DDL_SHDPurpose.SelectedValue + "'";
            SDS_HDonationItem.DataBind();
            DDL_SHDItem.DataBind();
        }
    }
    #endregion


    #region 註冊非同步事件
    protected void RegisterAsync()
    {
        //for (int x = 0; x < Rpt_HCCRegular.Items.Count; x++)
        //{
        //    ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Rpt_HCCRegular.Items[x].FindControl("LBtn_EditCode"));
        //}

    }
    #endregion

}