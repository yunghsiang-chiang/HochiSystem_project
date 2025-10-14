using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class System_HSCCourseSetting : System.Web.UI.Page
{

    SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        RegScript();//執行js

    }


    #region 搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        string QuerySql = "SELECT a.HID, a.HCTemplateID, b.HTemplateName, a.HSCForumClassID, c.HSCFCName, a.HTopicNameType, a.HStatus FROM HSCCTopicSetting AS a INNER JOIN HCourse_T AS b ON a.HCTemplateID=b.HID INNER JOIN HSCForumClass AS c ON a.HSCForumClassID=c.HID WHERE a.HStatus=1 ";

        //搜尋條件
        StringBuilder sql = new StringBuilder(QuerySql);
        List<string> WHERE = new List<string>();


        if (DDL_SHCTemplateID.SelectedValue != "0")
        {
            WHERE.Add(" ( a.HCTemplateID =" + DDL_SHCTemplateID.SelectedValue + " )");
        }

        if (DDL_SHSCForumClassID.SelectedValue != "0")
        {
            WHERE.Add(" ( a.HSCForumClassID =" + DDL_SHSCForumClassID.SelectedValue + " )");
        }

        //判斷使用者是否選擇了條件
        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" AND " + wh);
        }
        else
        {
            if (WHERE.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('至少輸入一個條件式!');", true);
                return;
            }
        }

        SDS_HSCCourseSetting.SelectCommand = sql.ToString();

    }
    #endregion

    #region 搜尋取消功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        DDL_SHCTemplateID.SelectedValue = "0";
        DDL_SHSCForumClassID.SelectedValue = "0";
        Rpt_HSCCourseSetting.DataBind();

    }
    #endregion

    #region 列表資料繫結
    protected void Rpt_HSCCourseSetting_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        #region 選單
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HSCForumClassID")).Text))
        {
            ((DropDownList)e.Item.FindControl("DDL_HSCForumClassID")).SelectedValue = ((Label)e.Item.FindControl("LB_HSCForumClassID")).Text;
        }

        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HCTemplateID")).Text))
        {
            ((DropDownList)e.Item.FindControl("DDL_HCTemplateID")).SelectedValue = ((Label)e.Item.FindControl("LB_HCTemplateID")).Text;
        }

        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HTopicNameType")).Text))
        {
            ((DropDownList)e.Item.FindControl("DDL_HTopicNameType")).SelectedValue = ((Label)e.Item.FindControl("LB_HTopicNameType")).Text;
        }


        #endregion

    }
    #endregion

    #region 新增功能
    protected void LBtn_Add_Click(object sender, EventArgs e)
    {
        if (DDL_HSCForumClassID.SelectedValue == "0" || DDL_HCTemplateID.SelectedValue == "0" || DDL_HTopicNameType.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('資料填寫不完整!');", true);
            return;
        }

        #region 判斷是否重複

        SqlCommand read = new SqlCommand("SELECT HID FROM HSCCTopicSetting WHERE HSCForumClassID=@HSCForumClassID AND HCTemplateID=@HCTemplateID AND HTopicNameType=@HTopicNameType AND HStatus=1", ConStr);
        ConStr.Open();
        read.Parameters.AddWithValue("@HSCForumClassID", DDL_HSCForumClassID.SelectedValue);
        read.Parameters.AddWithValue("@HCTemplateID", DDL_HCTemplateID.SelectedValue);
        read.Parameters.AddWithValue("@HTopicNameType", DDL_HTopicNameType.SelectedValue);

        SqlDataReader MyEBF = read.ExecuteReader();

        if (MyEBF.Read())
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('已經有這個主題設定囉!');", true);
            return;
        }
        MyEBF.Close();
        ConStr.Close();

        #endregion

        SqlCommand cmd = new SqlCommand("INSERT INTO [HSCCTopicSetting] ( HCTemplateID, HSCForumClassID, HTopicNameType, HStatus, HCreate, HCreateDT)VALUES( @HCTemplateID, @HSCForumClassID, @HTopicNameType, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HSCForumClassID", DDL_HSCForumClassID.SelectedValue);
        cmd.Parameters.AddWithValue("@HCTemplateID", DDL_HCTemplateID.SelectedValue);
        cmd.Parameters.AddWithValue("@HTopicNameType", DDL_HTopicNameType.SelectedValue);
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        DDL_HSCForumClassID.SelectedValue = "0";
        DDL_HCTemplateID.SelectedValue = "0";
        DDL_HTopicNameType.SelectedValue = "0";
        Rpt_HSCCourseSetting.DataBind();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('新增成功!');", true);

    }
    #endregion

    #region 刪除功能
    protected void LBtn_Del_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Del = sender as LinkButton;
        int Del_CA = Convert.ToInt32(LBtn_Del.CommandArgument);

        //更新資料庫
        SqlCommand cmd = new SqlCommand("UPDATE HSCCTopicSetting SET HStatus=@HStatus,HModify=@HModify,HModifyDT=@HModifyDT WHERE HID =@HID", ConStr);
        ConStr.Open();
        cmd.Parameters.AddWithValue("@HStatus", 0);
        cmd.Parameters.AddWithValue("@HID", Del_CA);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        Rpt_HSCCourseSetting.DataBind();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('刪除成功!');", true);

    }
    #endregion

    #region 執行js
    public void RegScript()
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "js", ("$('.js-example-basic-single').select2({ closeOnSelect: true, })"), true);
    }
    #endregion

}