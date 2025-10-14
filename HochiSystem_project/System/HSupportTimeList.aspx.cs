
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using HtmlAgilityPack;


//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;
//using System.Text.RegularExpressions;
//using System.Web.UI.HtmlControls;
//using System.Linq;

public partial class System_HSupportTimeList : System.Web.UI.Page
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


    #region 動態table
    private void ShowITable(string gCourseID)
    {
        //Response.Write(DDL_OGNum.SelectedValue);
        //*****************************使用動態 table開始*******************************//

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS3", ("$('.date').datepicker({format: 'dd/mm/yyyy', todayHighlight: true,orientation: 'bottom auto',autoclose: true,startDate:new Date(),});"), true);     //執行日曆js


        //DataTable取值與連結與新增刪除修改
        DataTable GHTUN_DT = new DataTable("QueryGHTUN_DT");
        GHTUN_DT.Columns.Add("HGroupID", typeof(String));
        GHTUN_DT.Columns.Add("HTaskID", typeof(String));

        //DataTable取值與連結與新增刪除修改
        DataTable HRM_DT = new DataTable("QueryHRM_DT");
        HRM_DT.Columns.Add("HGDay", typeof(String));
        HRM_DT.Columns.Add("HArea", typeof(String));
        HRM_DT.Columns.Add("HSystemName", typeof(String));
        HRM_DT.Columns.Add("HPeriod", typeof(String));
        HRM_DT.Columns.Add("HUserName", typeof(String));
        HRM_DT.Columns.Add("HGSTime", typeof(String));
        HRM_DT.Columns.Add("HGETime", typeof(String));
        HRM_DT.Columns.Add("HGroupID", typeof(String));
        HRM_DT.Columns.Add("HTaskID", typeof(String));


        //int TRow = gTRow;
        int gCT = 0;
        int gACT = 0;
        int gACTAll = 0;
        int gLCTAll = 0;

        //int gRCT = 0;
        int gAllCT = 0; ;//總筆數

        //string[] arr2 = new string[] { };
        string gSDate = "";
        string gEDate = "";

        //判斷天數相差幾天
        int gDateRange = 0;//連續天數
        //int gHDateRangeCT = 0;

        //Response.Write("select Count(a.HGroupName) as CT, b.HDateRange from HTodoList as a join HCourse as b on a.HCourseID = b.HID where a.HCourseID = '" + gCourseID + "' group by HDateRange");
        //Response.End();

        string[] gDateRangeA = { };
        //不含HTodoList_T
        //SqlDataReader QueryHMCBCAct = SQLdatabase.ExecuteReader("select Count(a.HGroupName) as CT, b.HDateRange from HTodoList as a join HCourse as b on a.HCourseID = b.HID where a.HCourseID = '" + gCourseID + "' group by HDateRange");
        SqlDataReader QueryHMCBCAct = SQLdatabase.ExecuteReader("select Count(a.HGroupName) as CT, b.HDateRange from (select HCourseID, HGroupName from HTodoList union select cc.HID, HGroupName from HTodoList_T as aa join HCourse_T as bb on aa.HCTemplateID = bb.HID join HCourse as cc on cc.HCTemplateID = bb.HID) as a join HCourse as b on a.HCourseID = b.HID where a.HCourseID = '" + gCourseID + "' group by HDateRange");
        //Response.Write("<br/>select Count(a.HGroupName) as CT, b.HDateRange from (select HCourseID, HGroupName from HTodoList union select cc.HID, HGroupName from HTodoList_T as aa join HCourse_T as bb on aa.HCTemplateID = bb.HID join HCourse as cc on cc.HCTemplateID = bb.HID) as a join HCourse as b on a.HCourseID = b.HID where a.HCourseID = '" + gCourseID + "' group by HDateRange<br/><br/><br/><br/>");
        //Response.End();
        while (QueryHMCBCAct.Read())
        {
            if (QueryHMCBCAct["HDateRange"].ToString().IndexOf("-") >= 0)
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                string[] gDateArray = QueryHMCBCAct["HDateRange"].ToString().Split('-');
                string gDate = "";
                gSDate = (gDateArray[0]);
                gEDate = (gDateArray[1]);
                gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                TableCell[] HCell = new TableCell[gDateRangeA.Length];
                for (int i = 0; i < gDateRange; i++)
                {
                    gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                }

                gDateRangeA = gDate.Trim(',').Split(',');
            }
            else if (QueryHMCBCAct["HDateRange"].ToString().IndexOf(",") >= 0)
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                gDateRangeA = QueryHMCBCAct["HDateRange"].ToString().Trim(',').Split(',');
                gSDate = (gDateRangeA[0]);
                gEDate = (gDateRangeA[gDateRangeA.Length - 1]);

                gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
            }
            //AE20230908_加入單一日期
            else if (QueryHMCBCAct["HDateRange"].ToString().IndexOf("-") < 0 && QueryHMCBCAct["HDateRange"].ToString().IndexOf(",") < 0)//單一日期
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                string[] gDateArray = (QueryHMCBCAct["HDateRange"].ToString() + " - " + QueryHMCBCAct["HDateRange"].ToString()).Split('-');
                string gDate = "";
                gSDate = (Convert.ToDateTime(gDateArray[0]).AddDays(-1)).ToString();
                gEDate = (gDateArray[1]);
                gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                gDateRangeA = QueryHMCBCAct["HDateRange"].ToString().Trim(',').Split(',');
            }
            else
            {
                // Response.Write("<script>alert('日期錯誤!');window.location.href='HStayList.aspx';</script>");

                //AE20230908_修改導回的頁面
                Response.Write("<script>alert('日期錯誤!');window.location.href='HSupportTimeList.aspx';</script>");
                return;
            }
        }
        QueryHMCBCAct.Close();









        //for (int gRow = 4; gRow <= 23; gRow++)
        //{

        string gGroupID = "";
        string gTaskID = "";

        //********表格標題開始********
        TableRow HRow1 = new TableRow();
        TableRow HRow2 = new TableRow();
        TableCell HCell1 = new TableCell();
        TableCell HCell2 = new TableCell();



        HCell1.Text = "日期";
        HCell1.BorderWidth = 1;
        HCell1.Width = Unit.Percentage(5);
        HCell1.HorizontalAlign = HorizontalAlign.Center;
        HCell1.CssClass = "font-weight-bold";
        HCell1.RowSpan = 2;
        HRow1.Cells.Add(HCell1);

        HCell2.Text = "時間區間";
        HCell2.BorderWidth = 1;
        HCell2.Width = Unit.Percentage(10);
        HCell2.HorizontalAlign = HorizontalAlign.Center;
        HCell2.CssClass = "font-weight-bold";
        HCell2.RowSpan = 2;
        HRow1.Cells.Add(HCell2);

        int gAllDateCT = 0;

        if (gCT == 0)
        {
            int gQueryGHGNct = 0;

            //HRow1.Cells.Add(HCell2);

            //標題用
            SqlDataReader QueryGHGN = SQLdatabase.ExecuteReader("select Count(a.HGroupName) as CT, Max(a.HGroupName) as HGroupID, max(b.HGroupName) as HGroupName from (select HTask, HCourseID, HGroupName from HTodoList union select HTask, cc.HID, HGroupName from HTodoList_T as aa join HCourse_T as bb on aa.HCTemplateID = bb.HID join HCourse as cc on cc.HCTemplateID = bb.HID) as a left join HGroup as b on a.HGroupName = b.HID where a.HCourseID  = '" + gCourseID + "' group by a.HGroupName order by a.HGroupName");

            //Response.Write("select Count(a.HGroupName) as CT, Max(a.HGroupName) as HGroupID, max(b.HGroupName) as HGroupName from (select HTask, HCourseID, HGroupName from HTodoList union select HTask, cc.HID, HGroupName from HTodoList_T as aa join HCourse_T as bb on aa.HCTemplateID = bb.HID join HCourse as cc on cc.HCTemplateID = bb.HID) as a left join HGroup as b on a.HGroupName = b.HID where a.HCourseID  = '" + gCourseID + "' group by a.HGroupName order by a.HGroupName<br/>");

            while (QueryGHGN.Read())
            {
                //int gQueryGHTUNct = 0;

                TableCell[] HCell = new TableCell[gDateRange];
                //string gDate = Convert.ToDateTime(gSDate).AddDays(i).ToString("MM/dd");
                HCell[0] = new TableCell();
                HCell[0].ColumnSpan = Convert.ToInt32(QueryGHGN["CT"].ToString());
                HCell[0].Text = QueryGHGN["HGroupName"].ToString();
                HCell[0].BorderWidth = 1;
                HCell[0].HorizontalAlign = HorizontalAlign.Center;
                HCell[0].CssClass = "font-weight-bold";
                HRow1.Cells.Add(HCell[0]);


                //gQueryGHGNct++;





                //標題用
                SqlDataReader QueryGHTUN = SQLdatabase.ExecuteReader("select a.HGroupName as HGroupID, b.HTask, b.HID as HTaskID from (select HTask, HCourseID, HGroupName from HTodoList union select HTask, cc.HID, HGroupName from HTodoList_T as aa join HCourse_T as bb on aa.HCTemplateID = bb.HID join HCourse as cc on cc.HCTemplateID = bb.HID) as a left join HCourseTask as b on a.HTask = b.HID where a.HCourseID = '" + gCourseID + "' and a.HGroupName = '" + QueryGHGN["HGroupID"].ToString() + "'");

                //Response.Write("select a.HGroupName as HGroupID, b.HTask, b.HID as HTaskID from (select HTask, HCourseID, HGroupName from HTodoList union select HTask, cc.HID, HGroupName from HTodoList_T as aa join HCourse_T as bb on aa.HCTemplateID = bb.HID join HCourse as cc on cc.HCTemplateID = bb.HID) as a left join HCourseTask as b on a.HTask = b.HID where a.HCourseID = '" + gCourseID + "' and a.HGroupName = '" + QueryGHGN["HGroupID"].ToString() + "'<br/>");




                while (QueryGHTUN.Read())
                {

                    //新增DT資料
                    DataRow GHTUN_DT_row;
                    GHTUN_DT_row = GHTUN_DT.NewRow();
                    GHTUN_DT_row["HGroupID"] = QueryGHTUN["HGroupID"].ToString();
                    GHTUN_DT_row["HTaskID"] = QueryGHTUN["HTaskID"].ToString();
                    GHTUN_DT.Rows.Add(GHTUN_DT_row);


                    HCell[0] = new TableCell();
                    HCell[0].BorderWidth = 1;
                    HCell[0].HorizontalAlign = HorizontalAlign.Center;
                    HCell[0].Text = QueryGHTUN["HTask"].ToString();
                    HCell[0].CssClass = "font-weight-bold";
                    HRow2.Cells.Add(HCell[0]);



                    TBL_SupportTimeList.Rows.Add(HRow1);
                    TBL_SupportTimeList.Rows.Add(HRow2);
                    TBL_SupportTimeList.CssClass = "table table-hover table-responsive";
                    //gQueryGHTUNct++;








                    //********表格標題結束********











                    //AE20240223_加入體系資訊
                    //SqlDataReader QueryHRM = SQLdatabase.ExecuteReader("select b.HCourseID, a.HGDay, a.HGSTime, a.HGETime, d.HTask, d.HID as HTaskID, a.HGroupID, c.HGroupName, f.HUSerName, f.HPeriod, f.HArea, f.HSystemName from HCourseBooking_Group as a left join OrderList_Merge as b on a.HBookingID = b.HID left join HGroup as c on a.HGroupID = c.HID left join HCourseTask as d on a.HTaskID = d.HID  left join MemberList as f on b.HMemberID = f.HID where b.HCourseID  = '" + gCourseID + "' and a.HGroupID='" + QueryGHGN["HGroupID"].ToString() + "' and d.HID='" + gTaskID + "' and SUBSTRING(a.HGSTime,1,2)<='" + gRow.ToString().PadLeft(2, '0') + "' and SUBSTRING(HGETime,1,2)>='" + Convert.ToInt32(gRow + 1).ToString().PadLeft(2, '0') + "' and HGDay='" + gYearDate + "' and b.HStatus='1' and b.HItemStatus='1'");
                    //SqlDataReader QueryHRM = SQLdatabase.ExecuteReader("select b.HCourseID, a.HGDay, a.HGSTime, a.HGETime, d.HTask, d.HID as HTaskID, a.HGroupID, c.HGroupName, f.HUSerName, f.HPeriod, f.HArea, f.HSystemName from HCourseBooking_Group as a left join OrderList_Merge as b on a.HBookingID = b.HID left join HGroup as c on a.HGroupID = c.HID left join HCourseTask as d on a.HTaskID = d.HID  left join MemberList as f on b.HMemberID = f.HID where b.HCourseID  = '" + gCourseID + "' and a.HGroupID='" + QueryGHGN["HGroupID"].ToString() + "' and d.HID='" + QueryGHTUN["HTaskID"].ToString() + "' and SUBSTRING(a.HGSTime,1,2)<='04' and SUBSTRING(HGETime,1,2)>='24' and HGDay='" + gYearDate + "' and b.HStatus='1' and b.HItemStatus='1'");
                    SqlDataReader QueryHRM = SQLdatabase.ExecuteReader("select b.HCourseID, a.HGDay, a.HGSTime, a.HGETime, d.HTask, d.HID as HTaskID, a.HGroupID, c.HGroupName, f.HUSerName, f.HPeriod, f.HArea, f.HSystemName from HCourseBooking_Group as a left join OrderList_Merge as b on a.HBookingID = b.HID left join HGroup as c on a.HGroupID = c.HID left join HCourseTask as d on a.HTaskID = d.HID  left join MemberList as f on b.HMemberID = f.HID where b.HCourseID  = '" + gCourseID + "' and a.HGroupID='" + QueryGHGN["HGroupID"].ToString() + "' and d.HID='" + QueryGHTUN["HTaskID"].ToString() + "' and b.HStatus='1' and b.HItemStatus='1'");





                    //Response.Write("select b.HCourseID, a.HGDay, a.HGSTime, a.HGETime, d.HTask, d.HID as HTaskID, a.HGroupID, c.HGroupName, f.HUSerName, f.HPeriod, f.HArea, f.HSystemName from HCourseBooking_Group as a left join OrderList_Merge as b on a.HBookingID = b.HID left join HGroup as c on a.HGroupID = c.HID left join HCourseTask as d on a.HTaskID = d.HID  left join MemberList as f on b.HMemberID = f.HID where b.HCourseID  = '" + gCourseID + "' and a.HGroupID='" + QueryGHGN["HGroupID"].ToString() + "' and d.HID='" + QueryGHTUN["HTaskID"].ToString() + "' and b.HStatus='1' and b.HItemStatus='1'<br/>");


                    while (QueryHRM.Read())
                    {
                        //Response.Write("<br/>QueryHRM.HGDay:" + QueryHRM["HGDay"].ToString() + "<br/>");
                        //Response.Write("<br/>QueryHRM.HGSTime:" + QueryHRM["HGSTime"].ToString() + "<br/>");
                        //Response.Write("<br/>QueryHRM.HGETime:" + QueryHRM["HGETime"].ToString() + "<br/>");

                        //新增DT資料
                        DataRow HRM_DT_row;
                        HRM_DT_row = HRM_DT.NewRow();
                        HRM_DT_row["HGDay"] = QueryHRM["HGDay"].ToString();
                        HRM_DT_row["HArea"] = QueryHRM["HArea"].ToString();
                        HRM_DT_row["HSystemName"] = QueryHRM["HSystemName"].ToString();
                        HRM_DT_row["HPeriod"] = QueryHRM["HPeriod"].ToString();
                        HRM_DT_row["HUserName"] = QueryHRM["HUserName"].ToString();
                        HRM_DT_row["HGSTime"] = QueryHRM["HGSTime"].ToString();
                        HRM_DT_row["HGETime"] = QueryHRM["HGETime"].ToString();
                        HRM_DT_row["HGroupID"] = QueryHRM["HGroupID"].ToString();
                        HRM_DT_row["HTaskID"] = QueryHRM["HTaskID"].ToString();
                        HRM_DT.Rows.Add(HRM_DT_row);


                    }
                    QueryHRM.Close();

















                }
                QueryGHTUN.Close();
            }
            QueryGHGN.Close();



        }//if (gCT == 0) 結束


        gCT++;




        //********表格內容開始********












        //天開始
        for (int i = 0; i < gDateRangeA.Length; i++)
        {
            string gDate = Convert.ToDateTime(gDateRangeA[i]).ToString("MM/dd");
            string gYMD = Convert.ToDateTime(gDateRangeA[i]).ToString("yyyy/MM/dd");



            //小時開始
            for (int gRow = 4; gRow <= 23; gRow++)
            {

                TableRow BRow = new TableRow();
                TableCell BCell1 = new TableCell();//編號
                TableCell BCell2 = new TableCell();//編號

                if (gRow == 4)
                {
                    Label LB_HDateRange = new Label();
                    LB_HDateRange.ID = "LB_HDateRange" + gCT.ToString();
                    LB_HDateRange.Text = gDate.ToString();
                    LB_HDateRange.Style["text-align"] = "center";
                    LB_HDateRange.BorderWidth = 0;
                    LB_HDateRange.CssClass = "form-control staylist_cell";
                    BCell1.BorderWidth = 1;
                    BCell1.RowSpan = 20;
                    BCell1.Controls.Add(LB_HDateRange);

                    BRow.Cells.Add(BCell1);
                }


                Label LB_AllTime = new Label();
                LB_AllTime.ID = "LB_AllTime" + gCT.ToString();
                LB_AllTime.Text = gRow + ":00~" + Convert.ToInt32(gRow + 1) + ":00";
                LB_AllTime.Style["text-align"] = "center";
                LB_AllTime.BorderWidth = 0;
                LB_AllTime.CssClass = "form-control staylist_cell";
                BCell2.BorderWidth = 1;
                BCell2.Controls.Add(LB_AllTime);
                BRow.Cells.Add(BCell2);


                TBL_SupportTimeList.Rows.Add(BRow);


                int gTPeople = 0;
                string gAPeople = "0";
                string gHLodging = "0";
                string gHAStatus = "";


                //找全部內容-驗證用
                //select e.HID as HCourseID, a.HGDay, a.HGSTime, a.HGETime, d.HTask, a.HGroupID, c.HGroupName, f.HUSerName from HCourseBooking_Group as a left join HCourseBooking as b on a.HBookingID = b.HID left join HGroup as c on a.HGroupID = c.HID left join HCourseTask as d on d.HGroupID = c.HID left join HCourse as e on b.HCourseID = e.HID left join HMember as f on b.HMemberID = f.HID where b.HCourseID = '7' order by HGroupName




               TableCell[] BCell = new TableCell[Convert.ToInt32(gAllCT)];




                foreach (DataRow GHTUNrow in GHTUN_DT.Rows)
                {

                    int gQueryGHTUNbCT = 0;//暫用不到?

                    BCell[gQueryGHTUNbCT] = new TableCell();
                    BCell[gQueryGHTUNbCT].BorderWidth = 1;
                    BCell[gQueryGHTUNbCT].HorizontalAlign = HorizontalAlign.Center;




                    foreach (DataRow HRMrow in HRM_DT.Rows)
                    {

                        //Response.Write("<br/>HGDay:" + HRMrow["HGDay"].ToString() + "<br/>");
                        //Response.Write("<br/>gYMD:" + gYMD + "<br/>");
                        //Response.Write("<br/>HGSTime:" + HRMrow["HGSTime"].ToString() + "<br/>");
                        //Response.Write("<br/>HGETime:" + HRMrow["HGETime"].ToString() + "<br/>");
                        //Response.Write("<br/>gRow:" + gRow + "<br/>");

                        if ((HRMrow["HGDay"].ToString() == gYMD.ToString()) && (Convert.ToInt32(HRMrow["HGSTime"].ToString().Substring(0, 2)) <= gRow && Convert.ToInt32(HRMrow["HGETime"].ToString().Substring(0, 2)) >= gRow) && GHTUNrow["HGroupID"].ToString()== HRMrow["HGroupID"].ToString() && GHTUNrow["HTaskID"].ToString() == HRMrow["HTaskID"].ToString())
                        {
                            //Response.Write("<br/>");
                            //Response.Write("<br/>HGDay:" + HRMrow["HGDay"].ToString() + "<br/>");
                            //Response.Write("<br/>gYMD:" + gYMD + "<br/>");
                            //Response.Write("<br/>HGSTime:" + HRMrow["HGSTime"].ToString() + "<br/>");
                            //Response.Write("<br/>HGETime:" + HRMrow["HGETime"].ToString() + "<br/>");
                            //Response.Write("<br/>gRow:" + gRow + "<br/>");

                            //Response.Write("<br/>HArea:" + HRMrow["HArea"].ToString() + "<br/>");
                            //Response.Write("<br/>HSystemName:" + HRMrow["HSystemName"].ToString() + "<br/>");
                            //Response.Write("<br/>HPeriod:" + HRMrow["HPeriod"].ToString() + "<br/>");
                            //Response.Write("<br/>HUserName:" + HRMrow["HUserName"].ToString() + "<br/>");
                            //Response.Write("<br/>------------------------------------------------------------------------------------------------------<br/>");
                            //AE20240223_加入體系資訊
                            BCell[gQueryGHTUNbCT].Text = HRMrow["HArea"].ToString() + "/" + HRMrow["HSystemName"].ToString() + "/" + HRMrow["HPeriod"].ToString() + " " + HRMrow["HUserName"].ToString();
                        }
                    }


                    BRow.Cells.Add(BCell[gQueryGHTUNbCT]);
                    TBL_SupportTimeList.Rows.Add(BRow);

                    gQueryGHTUNbCT++;


                }





            }//小時結束








        }//天結束













        //}// for (int gRow = 4; gRow <= 23; gRow++)結束



        //********表格內容結束********









        // }





        //}
        //QueryHMCBCA.Close();



        //****************************************使用動態 table結束 * *******************************************
    }

    #endregion

    #region 分頁copy-1
    private int PageMax = 15;   //分頁最大顯示數量
    private int LastPage = 10;  //分頁數量
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {

        #region 分頁copy-2
        if (!IsPostBack)
        {
            ViewState["Search"] = "";
        }

        if (!IsPostBack)
        {
            SDS_HC.SelectCommand = "SELECT a.HID, a.HCourseName, a.HTeacherName, a.HDateRange, b.HPlaceName FROM HCourse as a join HPlace as b on a.HOCPlace=b.HID where a.HStatus='1' ORDER BY a.HDateRange DESC";
            //SDS_HC.SelectCommand = "SELECT * FROM Course_BackendList where HStatus='1'";
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HC.SelectCommand, PageMax, LastPage, false, Rpt_HC);
            ViewState["Search"] = SDS_HC.SelectCommand;

        }
        else
        {
            SDS_HC.SelectCommand = ViewState["Search"].ToString();
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", ViewState["Search"].ToString(), PageMax, LastPage, false, Rpt_HC);




        }
        #endregion

        if (!IsPostBack)
        {
            SqlConnection dbConn = default(SqlConnection);
            SqlCommand dbCmd = default(SqlCommand);
            string strDBConn = null;
            strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
            dbConn = new SqlConnection(strDBConn);
            dbConn.Open();
            string strSelHPN = "select HID, HPlaceName, HStatus from HPlace order by HPlaceName";
            //Response.Write(strSQL);
            dbCmd = new SqlCommand(strSelHPN, dbConn);
            SqlDataReader MyQueryHPN = dbCmd.ExecuteReader();
            DDL_HOCPlace.Items.Add(new ListItem("請選擇上課地點", "0"));
            while (MyQueryHPN.Read())
            {
                if (MyQueryHPN["HStatus"].ToString() == "0")
                {
                    DDL_HOCPlace.Items.Add(new ListItem(MyQueryHPN["HPlaceName"].ToString() + "(停用)", (MyQueryHPN["HID"].ToString())));
                }
                else
                {
                    DDL_HOCPlace.Items.Add(new ListItem(MyQueryHPN["HPlaceName"].ToString(), (MyQueryHPN["HID"].ToString())));
                }
            }
            MyQueryHPN.Close();
            dbConn.Close();

            SqlDataReader QueryHSSel = SQLdatabase.ExecuteReader("SELECT HID, HSystemName FROM HSystem");
            while (QueryHSSel.Read())
            {
                DDL_HSystem.Items.Add(new ListItem(QueryHSSel["HSystemName"].ToString(), (QueryHSSel["HID"].ToString())));
            }
            QueryHSSel.Close();


            SqlDataReader QuerySelHG = SQLdatabase.ExecuteReader("SELECT HID, HGroupName FROM HGroup WHERE HStatus='1'");
            while (QuerySelHG.Read())
            {
                DDL_HMemberGroup.Items.Add(new ListItem(QuerySelHG["HGroupName"].ToString(), (QuerySelHG["HID"].ToString())));
            }
            QuerySelHG.Close();
        }

    }

    #region 搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        string[] gHDateRangeArray = TB_SearchDate.Text == "" ? "2000/01/01-3000/12/31".Split('-') : TB_SearchDate.Text.Split('-');
        string gHOCPlace = DDL_HOCPlace.SelectedValue == "0" ? "like '%'" : "='" + DDL_HOCPlace.SelectedValue + "'";//地點沒輸入則全找
        //string gHOCPlace = DDL_HOCPlace.SelectedValue == "0" ? "='0'" : "='" + DDL_HOCPlace.SelectedValue + "'";//地點沒輸入則全不找
        string gFrom = "";
        string gHSystem = "";
        if (DDL_HSystem.SelectedValue == "0" && DDL_HMemberGroup.SelectedValue == "0") //體系與體系護持組別沒輸入則全找
        {
            gFrom = "HCourse as a join HPlace as b on a.HOCPlace=b.HID";
            gHSystem = "";
        }
        else if (DDL_HSystem.SelectedValue != "0" && DDL_HMemberGroup.SelectedValue == "0")//體系有輸入，體系護持組別沒輸入
        {
            gFrom = "HCourse as a join HPlace as b on a.HOCPlace = b.HID join (select HCourseID, HGroupName from HTodoList union select cc.HID, HGroupName from HTodoList_T as aa join HCourse_T as bb on aa.HCTemplateID = bb.HID join HCourse as cc on cc.HCTemplateID = bb.HID) as c on a.HID = c.HCourseID join HCourseTask as d on c.HGroupName = d.HID join HSystem as e on d.HSystemID = e.HID";
            gHSystem = "and e.HID='" + DDL_HSystem.SelectedValue + "'";
        }
        else if (DDL_HSystem.SelectedValue == "0" && DDL_HMemberGroup.SelectedValue != "0")//體系沒輸入，體系護持組別有輸入
        {
            gFrom = "HCourse as a join HPlace as b on a.HOCPlace = b.HID join (select HCourseID, HGroupName from HTodoList union select cc.HID, HGroupName from HTodoList_T as aa join HCourse_T as bb on aa.HCTemplateID = bb.HID join HCourse as cc on cc.HCTemplateID = bb.HID) as c on a.HID = c.HCourseID";
            gHSystem = "and c.HGroupName='" + DDL_HMemberGroup.SelectedValue + "'";
        }
        else if (DDL_HSystem.SelectedValue != "0" && DDL_HMemberGroup.SelectedValue != "0")//體系與體系護持組別都有輸入
        {
            gFrom = "HCourse as a join HPlace as b on a.HOCPlace = b.HID join (select HCourseID, HGroupName from HTodoList union select cc.HID, HGroupName from HTodoList_T as aa join HCourse_T as bb on aa.HCTemplateID = bb.HID join HCourse as cc on cc.HCTemplateID = bb.HID) as c on a.HID = c.HCourseID join HCourseTask as d on c.HGroupName = d.HID join HSystem as e on d.HSystemID = e.HID";
            gHSystem = "and e.HID='" + DDL_HSystem.SelectedValue + "' and c.HGroupName='" + DDL_HMemberGroup.SelectedValue + "'";
        }

        //未加入體系搜尋
        //SDS_HC.SelectCommand = "SELECT a.HID, a.HCourseName, a.HTeacherName, a.HDateRange, b.HPlaceName FROM HCourse as a join HPlace as b on a.HOCPlace=b.HID WHERE HCourseName like'%" + TB_Search.Text + "%' and b.HID " + gHOCPlace + " and ((left(HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and left(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "'and right(HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "')) and a.HStatus='1' ORDER BY a.HDateRange DESC";
        //加入體系搜尋
        SDS_HC.SelectCommand = "SELECT a.HID, a.HCourseName, a.HTeacherName, a.HDateRange, b.HPlaceName FROM " + gFrom + " WHERE HCourseName like'%" + TB_Search.Text + "%' and b.HID " + gHOCPlace + " and ((left(HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and left(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "'and right(HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "')) and a.HStatus='1'" + gHSystem + " ORDER BY a.HDateRange DESC";

        //Response.Write("<br/><br/><br/><br/><br/>SELECT a.HID, a.HCourseName, a.HTeacherName, a.HDateRange, b.HPlaceName FROM "+ gFrom + " WHERE HCourseName like'%" + TB_Search.Text + "%' and b.HID " + gHOCPlace + " and ((left(HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and left(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "'and right(HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "')) and a.HStatus='1'" + gHSystem + " ORDER BY a.HDateRange DESC");


        #region 分頁copy-3搜尋用
        ViewState["Search"] = SDS_HC.SelectCommand;
        //呼叫分頁 (連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 搜尋開啟, DataList控件)
        Pg_Paging.PagingLoad("HochiSystemConnection", SDS_HC.SelectCommand, PageMax, LastPage, true, Rpt_HC);
        #endregion

    }
    #endregion

    #region 取消搜尋功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_Search.Text = "";
        TB_SearchDate.Text = "";
        DDL_HOCPlace.SelectedValue = "0";

        SDS_HC.SelectCommand = "SELECT a.HID, a.HCourseName, a.HTeacherName, a.HDateRange, b.HPlaceName FROM HCourse as a join HPlace as b on a.HOCPlace=b.HID where a.HStatus='1' ORDER BY a.HDateRange DESC";
        //SDS_HC.SelectCommand = "SELECT * FROM Course_BackendList where HStatus='1'";
        ViewState["Search"] = SDS_HC.SelectCommand;

        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HC.SelectCommand, PageMax, LastPage, true, Rpt_HC);
    }
    #endregion

    protected void Rpt_HC_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;
        ((Label)e.Item.FindControl("LB_HTeacherName")).Text = "";

        if (!string.IsNullOrEmpty(gDRV["HTeacherName"].ToString()))
        {
            string[] gHTeacherName = gDRV["HTeacherName"].ToString().Split(',');
            for (int i = 0; i < gHTeacherName.Length - 1; i++)
            {
                //Response.Write("SELECT HUserName FROM HMember where HID = '" + gHTeacherName[i].ToString() + "'");
                SqlDataReader QueryHTeacher = SQLdatabase.ExecuteReader("SELECT B.HUserName FROM HTeacher AS A JOIN HMember AS B ON A.HMemberID =B.HID where A.HID = '" + gHTeacherName[i].ToString() + "'");
                if (QueryHTeacher.Read())
                {
                    ((Label)e.Item.FindControl("LB_HTeacherName")).Text = QueryHTeacher["HUserName"].ToString() + "," + ((Label)e.Item.FindControl("LB_HTeacherName")).Text;
                }
                QueryHTeacher.Close();
            }

            if (((Label)e.Item.FindControl("LB_HTeacherName")).Text != "")
            {
                ((Label)e.Item.FindControl("LB_HTeacherName")).Text = ((Label)e.Item.FindControl("LB_HTeacherName")).Text.Substring(0, ((Label)e.Item.FindControl("LB_HTeacherName")).Text.Length - 1);
            }


        }

    }

    #region 下載分工時間表
    protected void LBtn_SupportTimeList_Click(object sender, EventArgs e)
    {




        Panel_CourseList.Visible = false;
        Panel_SupportTimeList.Visible = true;
        Panel_Search.Visible = false;


        LinkButton LBtn_SupportTimeList = sender as LinkButton;
        string SupportTimeList_CA = LBtn_SupportTimeList.CommandArgument;


        SqlDataReader QueryHCourse = SQLdatabase.ExecuteReader("select a.HCourseName, a.HTeacherName, a.HDateRange from HCourse as a where a.HID='" + SupportTimeList_CA + "'");
        //Response.Write("select a.HCourseName, a.HTeacherName, a.HDateRange from HCourse as a where a.HID='" + SupportTimeList_CA + "'");

        while (QueryHCourse.Read())
        {
            LB_HCourseName.Text = QueryHCourse["HCourseName"].ToString() + " 體系護持分工時間表";


            string[] gHTeacherName = QueryHCourse["HTeacherName"].ToString().Split(',');
            for (int i = 0; i < gHTeacherName.Length - 1; i++)
            {
                //Response.Write("SELECT HUserName FROM HMember where HID = '" + gHTeacherName[i].ToString() + "'");
                SqlDataReader QueryHTeacher = SQLdatabase.ExecuteReader("SELECT B.HUserName FROM HTeacher AS A JOIN HMember AS B ON A.HMemberID =B.HID where A.HID = '" + gHTeacherName[i].ToString() + "'");
                if (QueryHTeacher.Read())
                {
                    LB_HTeacherName.Text = QueryHTeacher["HUserName"].ToString() + "," + LB_HTeacherName.Text;
                }
                QueryHTeacher.Close();
            }

            LB_HTeacherName.Text = LB_HTeacherName.Text.Length > 0 ? "講師：" + LB_HTeacherName.Text.Substring(0, LB_HTeacherName.Text.Length - 1) : "講師：";

            LB_HDateRange.Text = "課程日期：" + QueryHCourse["HDateRange"].ToString();


        }

        QueryHCourse.Close();




        ShowITable(SupportTimeList_CA);








       // Response.End();












        ////Response.Clear();
        ////通知瀏覽器下載檔案
        //Response.AddHeader("content-disposition", "attachment;filename=體系護持分工時間表.xls");
        //Response.ContentType = "application/application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //System.IO.StringWriter sw = new System.IO.StringWriter();
        //System.Web.UI.HtmlTextWriter hw = new HtmlTextWriter(sw);
        ////TBL_SupportTimeList.RenderControl(hw);
        //Panel_SupportTimeList.RenderControl(hw);
        //Response.Write(sw.ToString());
        //Response.End();

    }
    #endregion

    #region 回上一頁
    protected void LBtn_Back_Click(object sender, EventArgs e)
    {
        //Response.Write("<script>window.location.href='HEnrollment.aspx';</script>");

        Panel_Search.Visible = true;
        Panel_CourseList.Visible = true;
        Panel_SupportTimeList.Visible = false;
    }
    #endregion
}