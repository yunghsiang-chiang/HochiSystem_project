using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class System_HSCRuleTemplate : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT TOP (1) HID, HSCRule, HStatus FROM HSCRule_T WHERE HStatus='1' ORDER BY HStatus");
            if (dr.Read())
            {
                CKE_HSCRule.Text = dr["HSCRule"].ToString();
            }
            dr.Close();
        }

    }

    #region 儲存功能
    protected void Btn_Submit_Click(object sender, EventArgs e)
    {
        string strHSCRule = "IF EXISTS(SELECT HSCRule FROM HSCRule_T WHERE HStatus='1')  BEGIN UPDATE HSCRule_T SET HSCRule=@HSCRule, HModify=@HModify, HModifyDT=@HModifyDT  END ELSE  BEGIN INSERT INTO HSCRule_T (HSCRule, HStatus, HCreate, HCreateDT, HModify, HModifyDT) VALUES (@HSCRule, @HStatus, @HCreate, @HCreateDT, @HModify, @HModifyDT ) END";

        SqlCommand cmd = new SqlCommand(strHSCRule, con);
        con.Open();
        cmd.Parameters.AddWithValue("@HSCRule", CKE_HSCRule.Text.Trim());
        cmd.Parameters.AddWithValue("@HStatus", "1");
        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteReader();
        con.Close();
        cmd.Cancel();

        Response.Write("<script>alert('存檔成功!');window.location.href='HSCRuleTemplate.aspx';</script>");

    }
    #endregion

    #region 取消儲存功能
    protected void Btn_Cancel_Click(object sender, EventArgs e)
    {
        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT TOP (1) HID, HSCRule, HStatus FROM HSCRule_T WHERE HStatus='1' ORDER BY HStatus");
        if (dr.Read())
        {
            CKE_HSCRule.Text = HttpUtility.HtmlDecode(dr["HSCRule"].ToString());
        }
        dr.Close();

    }
    #endregion
}