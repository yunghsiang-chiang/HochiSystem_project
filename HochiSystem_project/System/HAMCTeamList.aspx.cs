using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class System_HAMCTeamList : System.Web.UI.Page
{
    #region 分頁copy-1
    private int PageMax = 15;   //分頁最大顯示數量
    private int LastPage = 10;  //分頁數量
    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {


        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();
        string strSelHPN = "select HID, HArea, HStatus from HArea order by HID ASC ";
        //Response.Write(strSQL);
        dbCmd = new SqlCommand(strSelHPN, dbConn);
        SqlDataReader MyQueryHPN = dbCmd.ExecuteReader();
        DDL_HArea.Items.Add(new ListItem("-請選擇區屬-", "0"));
        while (MyQueryHPN.Read())
        {
            if (MyQueryHPN["HStatus"].ToString() == "0")
            {
                DDL_HArea.Items.Add(new ListItem(MyQueryHPN["HArea"].ToString() + "(停用)", (MyQueryHPN["HID"].ToString())));
            }
            else
            {
                DDL_HArea.Items.Add(new ListItem(MyQueryHPN["HArea"].ToString(), (MyQueryHPN["HID"].ToString())));
            }
        }
        MyQueryHPN.Close();
        dbConn.Close();


        if (!IsPostBack)
        {
            SDS_TotalListArea.SelectCommand = "select a.HAreaID, B.HArea, b.HSort, c.HSort as HSortL, C.HLArea from HMember AS A left join HArea AS B on A.HAreaID = B.HID left join HLArea AS C on B.HLAreaID = C.HID  where a.HStatus=1 group by a.HAreaID, B.HArea, b.HSort, c.HSort, C.HLArea ORDER BY c.HSort, b.HSort";
            Rpt_TotalListArea.DataBind();
        }

    }

    protected void LBtn_Search_Click(object sender, EventArgs e)
    {

        if (DDL_HArea.SelectedValue != "0")
        {
            LB_HTitle.Text = DDL_HArea.SelectedItem.Text + "區-光團連線名單";
            SDS_TotalListArea.SelectCommand = "select a.HAreaID, B.HArea , C.HLArea from HMember AS A left join HArea AS B on A.HAreaID = B.HID left join HLArea AS C on B.HLAreaID = C.HID  where a.HAreaID ='" + DDL_HArea.SelectedValue + "' AND  a.HStatus=1 group by  a.HAreaID, B.HArea , C.HLArea  order by B.HArea ASC";
        }
        else
        {
            LB_HTitle.Text = "";
            SDS_TotalListArea.SelectCommand = "select a.HAreaID, B.HArea, b.HSort, c.HSort as HSortL, C.HLArea from HMember AS A left join HArea AS B on A.HAreaID = B.HID left join HLArea AS C on B.HLAreaID = C.HID  where a.HStatus=1 group by a.HAreaID, B.HArea, b.HSort, c.HSort, C.HLArea ORDER BY c.HSort, b.HSort";
        }


        Rpt_TotalListArea.DataBind();
    }

    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        DDL_HArea.SelectedValue = "0";
        LB_HTitle.Text = "";
        SDS_TotalListArea.SelectCommand = "select a.HAreaID, B.HArea, b.HSort, c.HSort as HSortL, C.HLArea from HMember AS A left join HArea AS B on A.HAreaID = B.HID left join HLArea AS C on B.HLAreaID = C.HID  where a.HStatus = '1' group by a.HAreaID, B.HArea, b.HSort, c.HSort, C.HLArea ORDER BY c.HSort, b.HSort";
        Rpt_TotalListArea.DataBind();
    }


    protected void Rpt_TotalListAreaChild_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;
        string gHTeamType = "";
        string gHTeamName = "";
        string gHTeamLeader = "";
        string[] gHTeamID = ((Label)e.Item.FindControl("LB_HTeamID")).Text.Split(',');
        if (gHTeamID.Length > 1)
        {
            gHTeamType = gHTeamID[1] == "1" ? "HMTeam" : "HCTeam";
            gHTeamName = gHTeamID[1] == "1" ? "HMTeam" : "HCTeam";
            gHTeamLeader = gHTeamID[1] == "1" ? "HMLeaderID" : "HCLeaderID";

            //顯示光團名稱
            SqlDataReader QueryHTeam = SQLdatabase.ExecuteReader("select " + gHTeamName + ",  B.HUserName as LeaderName, C.HUserName as ALeaderName  from " + gHTeamType + " LEFT JOIN HMember AS B ON " + gHTeamType + "." + gHTeamLeader + " =B.HID LEFT JOIN HMember AS C ON " + gHTeamType + ".HALeaderID =C.HID  where " + gHTeamType + ".HID='" + gHTeamID[0] + "'");

            while (QueryHTeam.Read())
            {
                ((Label)e.Item.FindControl("LB_HTeamID")).Text = QueryHTeam[gHTeamName].ToString();
                ((Label)e.Item.FindControl("LB_CTLeader")).Text = QueryHTeam["LeaderName"].ToString();  //帶領導師
                ((Label)e.Item.FindControl("LB_ATLeader")).Text = QueryHTeam["ALeaderName"].ToString();  //大區傳導師
            }
            QueryHTeam.Close();



            if (gHTeamType == "HCTeam")
            {
                SqlDataReader QueryMTeam = SQLdatabase.ExecuteReader("select HMTeam, B.HUserName as MTeamLeader , C.HUserName as ALeaderName from HCTeam LEFT JOIN HMTeam ON HCTeam.HMTeamID =HMTeam.HID LEFT JOIN  HMember AS B ON HMTeam.HMLeaderID = B.HID LEFT JOIN HMember AS C ON HCTeam.HALeaderID =C.HID where HCTeam.HID='" + gHTeamID[0] + "'");
                while (QueryMTeam.Read())
                {
                    ((Label)e.Item.FindControl("LB_MTeam")).Text = QueryMTeam["HMTeam"].ToString();
                    ((Label)e.Item.FindControl("LB_CTLeader")).Text = QueryMTeam["MTeamLeader"].ToString();  //帶領導師
                    ((Label)e.Item.FindControl("LB_ATLeader")).Text = QueryMTeam["ALeaderName"].ToString();  //大區傳導師
                }
                QueryMTeam.Close();
            }
            else
            {
                SqlDataReader QueryTeamInfo = SQLdatabase.ExecuteReader("select B.HUserName as MTeamLeader , C.HUserName as ALeaderName from HMTeam LEFT JOIN  HMember AS B ON HMTeam.HMLeaderID = B.HID LEFT JOIN HMember AS C ON HMTeam.HALeaderID =C.HID where HMTeam.HID='" + gHTeamID[0] + "'");
                while (QueryTeamInfo.Read())
                {
                    ((Label)e.Item.FindControl("LB_MTeam")).Text = "-";
                    ((Label)e.Item.FindControl("LB_CTLeader")).Text = QueryTeamInfo["MTeamLeader"].ToString();  //帶領導師
                    ((Label)e.Item.FindControl("LB_ATLeader")).Text = QueryTeamInfo["ALeaderName"].ToString();  //大區傳導師
                }
                QueryTeamInfo.Close();

            }

        }
        else
        {
            ((Label)e.Item.FindControl("LB_HTeamID")).Text = " - ";
        }


    }

    protected void Rpt_TotalListArea_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;


        //EE20231124_加入排序：光團->體系->天命法位
        ((SqlDataSource)e.Item.FindControl("SDS_TotalListAreaChild")).SelectCommand = "select A.HAreaID,A.HTeamID, E.HLArea, B.HArea, C.HSystemName, D.HRName, A.HPeriod, A.HID, A.HUserName, F.HMType AS HTypeName FROM HMember AS A LEFT JOIN HArea AS B on A.HAreaID = B.HID LEFT JOIN HSystem AS C ON A.HSystemID = C.HID LEFT JOIN HRole AS D ON A.HPositionID = D.HID LEFT JOIN HLArea AS E ON B.HLAreaID=E.HID LEFT JOIN HMType AS F ON A.HType=F.HID where  A.HAreaID='" + gDRV["HAreaID"].ToString() + "' and A.HStatus='1' ORDER BY A.HTeamID DESC,  C.HSystemName DESC, D.HRName DESC";

        //AE20230908_加入人數統計
        ((SqlDataSource)e.Item.FindControl("SDS_TotalListAreaChild")).DataBind();
        ((Repeater)e.Item.FindControl("Rpt_TotalListAreaChild")).DataBind();
        ((Label)e.Item.FindControl("LB_Count")).Text = "(" + (((Repeater)e.Item.FindControl("Rpt_TotalListAreaChild")).Items.Count).ToString() + ")";
    }







}