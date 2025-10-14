using DocumentFormat.OpenXml.Drawing.Charts;
using NPOI.SS.Formula.Functions;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class System_HSCModeratorSetting : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    #region 分頁copy-1
    private int PageMax = 15;   //分頁最大顯示數量
    private int LastPage = 10;  //分頁數量
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["Search"] = "";
            ViewState["JoinSearch"] = "";

            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, Repeater控件)
            Pg_Paging.PagingLoad("HochiSystemConnection", SDS_HSCModeratorSetting.SelectCommand, PageMax, LastPage, false, Rpt_HSCModeratorSetting);
            ViewState["Search"] = SDS_HSCModeratorSetting.SelectCommand;
        }
        else
        {
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, Repeater控件)
            Pg_Paging.PagingLoad("HochiSystemConnection", ViewState["Search"].ToString(), PageMax, LastPage, false, Rpt_HSCModeratorSetting);

        }
    }

    #region 搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        string Select = "SELECT  b.HID, b.HSCFCName, Count(c.HUserName) AS HTotalPeople, b.HStatus FROM HSCModeratorSetting AS a Cross Apply SPLIT(a.HMemberID, ',') AS d RIGHT JOIN HSCForumClass as b on b.HID=a.HSCForumClassID LEFT JOIN MemberList as c on d.value=c.HID WHERE  b.HSCFCLevel='30' ";
        //b.HStatus=1 AND

        StringBuilder sql = new StringBuilder(Select);
        List<string> WHERE = new List<string>();

        if (!string.IsNullOrEmpty(TB_Search.Text))
        {
            WHERE.Add(" (b.HSCFCName LIKE N'%" + TB_Search.Text.Trim() + "%')");
        }

        //判斷使用者是否選擇了條件
        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" AND " + wh);
        }

        SDS_HSCModeratorSetting.SelectCommand = sql.ToString() + " GROUP BY  b.HID, b.HSCFCName, b.HStatus ORDER BY b.HID ASC";

        #region 分頁copy-3搜尋用
        ViewState["Search"] = SDS_HSCModeratorSetting.SelectCommand;
        Pg_Paging.PagingLoad("HochiSystemConnection", SDS_HSCModeratorSetting.SelectCommand, PageMax, LastPage, true, Rpt_HSCModeratorSetting);
        #endregion
    }
    #endregion

    #region 取消搜尋功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_Search.Text = null;

        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, Repeater控件)
        Pg_Paging.PagingLoad("HochiSystemConnection", SDS_HSCModeratorSetting.SelectCommand, PageMax, LastPage, true, Rpt_HSCModeratorSetting);
        ViewState["Search"] = SDS_HSCModeratorSetting.SelectCommand;
    }
    #endregion

    #region 編輯功能
    protected void LBtn_Edit_Click(object sender, EventArgs e)
    {
        Panel_List.Visible = false;
        Panel_Edit.Visible = true;
        Panel_MList.Visible = true;

        LinkButton LBtn_Edit = sender as LinkButton;
        string Edit_CA = LBtn_Edit.CommandArgument;

        LB_HID.Text = Edit_CA;

        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT  b.HID, b.HSCFCName, a.HMemberID FROM HSCModeratorSetting AS a RIGHT JOIN HSCForumClass as b on b.HID=a.HSCForumClassID WHERE b.HSCFCLevel='30' AND b.HID='" + Edit_CA + "' GROUP BY  b.HID, b.HSCFCName, a.HMemberID ORDER BY b.HID ASC");
        //b.HStatus=1 AND

        if (dr.Read())
        {
            TB_HSCFCName.Text = dr["HSCFCName"].ToString();
            if (!string.IsNullOrEmpty(dr["HMemberID"].ToString()))
            {
                LB_AllHMemberID.Text = dr["HMemberID"].ToString().Substring(1, dr["HMemberID"].ToString().Length - 1); ;
            }
        }
        dr.Close();


        //顯示資料來源由HSCForumClass來；判斷如果HSCModeratorSetting沒有資料就新增
        SqlDataReader Insertdr = SQLdatabase.ExecuteReader("SELECT HID, HSCForumClassID, HMemberID, HStatus FROM HSCModeratorSetting WHERE HSCForumClassID ='" + Edit_CA + "' AND HStatus='1'");

        string HSCMSHID = null;

        if (!Insertdr.Read())
        {
            SqlCommand insertcmd = new SqlCommand("INSERT INTO HSCModeratorSetting (HSCForumClassID, HStatus, HCreate, HCreateDT) values (@HSCForumClassID, @HStatus, @HCreate, @HCreateDT); SELECT SCOPE_IDENTITY() AS TNo ", con);

            con.Open();
            insertcmd.Parameters.AddWithValue("@HSCForumClassID", Edit_CA);
            insertcmd.Parameters.AddWithValue("@HStatus", "1");
            insertcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            insertcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            HSCMSHID = insertcmd.ExecuteScalar().ToString();
            con.Close();
            insertcmd.Cancel();
        }
        Insertdr.Close();

        SDS_HMemberList.SelectCommand = "SELECT c.HID AS HMemberID,(c.HArea+'/'+c.HPeriod+' '+c.HUserName)AS HUserName FROM HSCModeratorSetting as a  CROSS APPLY Split(HMemberID, ',') as b LEFT JOIN MemberList as c on b.value=c.HID WHERE a.HSCForumClassID ='" + Edit_CA + "' AND value<>''";
        SDS_HMemberList.DataBind();

    }
    #endregion

    #region 管理人員名單

    #region 新增功能
    protected void LBtn_HMemberAdd_Click(object sender, EventArgs e)
    {
        if (DDL_HMemberID.SelectedValue=="0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('請選擇姓名!');", true);
            return;
        }

        StringBuilder gHMemberID = new StringBuilder();

        string[] gLB_AllHMemberID = LB_AllHMemberID.Text.Split(',');
        string gCK = "0";
        for (int i = 0; i < gLB_AllHMemberID.Length - 1; i++)
        {
            if (gLB_AllHMemberID[i].ToString() == DDL_HMemberID.SelectedValue)
            {
                gCK = "1";
            }
        }

        if (gCK == "1")
        {
            Response.Write("<script>alert('此管理者已存在!');</script>");
            return;
        }
        else
        {
            gHMemberID.Append(LB_AllHMemberID.Text + DDL_HMemberID.SelectedValue + ",");
        }

        SqlCommand upcmd = new SqlCommand("UPDATE HSCModeratorSetting SET HMemberID=@HMemberID, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCForumClassID=@HSCForumClassID", con);

        con.Open();
        upcmd.Parameters.AddWithValue("@HMemberID", "," + gHMemberID);
        upcmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        upcmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        upcmd.Parameters.AddWithValue("@HSCForumClassID", LB_HID.Text);

        upcmd.ExecuteNonQuery();
        con.Close();
        upcmd.Cancel();

        LB_AllHMemberID.Text = gHMemberID.ToString();

        SDS_HMemberList.SelectCommand = "SELECT c.HID AS HMemberID,(c.HArea+'/'+c.HPeriod+' '+c.HUserName)AS HUserName FROM HSCModeratorSetting as a  CROSS APPLY Split(HMemberID, ',') as b LEFT JOIN MemberList as c on b.value=c.HID WHERE a.HSCForumClassID ='" + LB_HID.Text + "' AND value<>''";
        SDS_HMemberList.DataBind();
        Rpt_HMemberList.DataBind();

        DDL_HMemberID.SelectedValue = "0";
    }
    #endregion

    #region 刪除功能
    protected void LBtn_HMemberDel_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_UserDel = sender as LinkButton;
        string UserDel_CA = LBtn_UserDel.CommandArgument;

        String[] gHMemberIDArray = LB_AllHMemberID.Text.Split(',');
        //Lambda運算式→匿名委派
        gHMemberIDArray = gHMemberIDArray.Where(val => val != UserDel_CA).ToArray();
        string gHMemberID = string.Join(",", gHMemberIDArray);

        SqlCommand upcmd = new SqlCommand("UPDATE HSCModeratorSetting SET HMemberID=@HMemberID, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCForumClassID=@HSCForumClassID", con);

        con.Open();
        upcmd.Parameters.AddWithValue("@HMemberID", "," + gHMemberID);
        upcmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        upcmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        upcmd.Parameters.AddWithValue("@HSCForumClassID", LB_HID.Text);

        upcmd.ExecuteNonQuery();
        con.Close();
        upcmd.Cancel();

        Response.Write("<script>alert('刪除成功!');</script>");

        LB_AllHMemberID.Text = gHMemberID.ToString();

        SDS_HMemberList.SelectCommand = "SELECT c.HID AS HMemberID,(c.HArea+'/'+c.HPeriod+' '+c.HUserName)AS HUserName FROM HSCModeratorSetting as a  CROSS APPLY Split(HMemberID, ',') as b LEFT JOIN MemberList as c on b.value=c.HID WHERE a.HSCForumClassID ='" + LB_HID.Text + "' AND value<>''";
        SDS_HMemberList.DataBind();
        Rpt_HMemberList.DataBind();

    }
    #endregion

    #endregion

    #region 回上一頁
    protected void LBtn_Return_Click(object sender, EventArgs e)
    {
        Panel_List.Visible = true;
        Panel_Edit.Visible = false;
        Panel_MList.Visible = false;

        string Select = "SELECT  b.HID, b.HSCFCName, Count(c.HUserName) AS HTotalPeople, b.HStatus FROM HSCModeratorSetting AS a Cross Apply SPLIT(a.HMemberID, ',') AS d RIGHT JOIN HSCForumClass as b on b.HID=a.HSCForumClassID LEFT JOIN MemberList as c on d.value=c.HID WHERE b.HSCFCLevel='30' ";
        //b.HStatus=1 AND

        StringBuilder sql = new StringBuilder(Select);
        List<string> WHERE = new List<string>();

        if (!string.IsNullOrEmpty(TB_Search.Text))
        {
            WHERE.Add(" (b.HSCFCName LIKE N'%" + TB_Search.Text.Trim() + "%')");
        }

        //判斷使用者是否選擇了條件
        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" AND " + wh);
        }

        SDS_HSCModeratorSetting.SelectCommand = sql.ToString() + " GROUP BY  b.HID, b.HSCFCName, b.HStatus ORDER BY b.HID ASC";

        #region 分頁copy-3搜尋用
        ViewState["Search"] = SDS_HSCModeratorSetting.SelectCommand;
        Pg_Paging.PagingLoad("HochiSystemConnection", SDS_HSCModeratorSetting.SelectCommand, PageMax, LastPage, true, Rpt_HSCModeratorSetting);
        #endregion
    }
    #endregion

    #region 列表資料繫結
    protected void Rpt_HSCModeratorSetting_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
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

    }
    #endregion

}