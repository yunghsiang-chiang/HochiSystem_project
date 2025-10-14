using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class System_HApplyRecord : System.Web.UI.Page
{







    #region 動態table
    private void ShowITable(string gCourseDescribe, string gHDateRange, string gHOCPlace, string gHArea)
    {
        //*****************************使用動態 table開始*******************************//

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS3", ("$('.date').datepicker({format: 'dd/mm/yyyy', todayHighlight: true,orientation: 'bottom auto',autoclose: true,startDate:new Date(),});"), true);     //執行日曆js
        string[] gHDateRangeArray = gHDateRange.ToString().Split('-');
        int gCT = 0;
        int gAllCT = 0;//總筆數

        int gCourseCT = 0;//課程筆數
        int gBookingCT = 0;//Booking筆數


        //HCourseBooking改成OrderList_Merge
        SqlDataReader QueryCourseCT = SQLdatabase.ExecuteReader("select Count(distinct c.HID) as CT from OrderList_Merge as a join HCourse as c on a.HCourseID = c.HID where " + gCourseDescribe + " and ((left(c.HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and left(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "'and right(c.HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "')) " + gHOCPlace + " and a.HStatus='1'  AND a.HItemStatus='1'");

        while (QueryCourseCT.Read())
        {
            gCourseCT = Convert.ToInt32(QueryCourseCT["CT"].ToString());
        }
        QueryCourseCT.Close();

        //HCourseBooking改成OrderList_Merge
        SqlDataReader QuerygTotalCT = SQLdatabase.ExecuteReader("select count(a.HID) as CT from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID join HCourse as c on b.HCourseID = c.HID left join HArea as d on a.HAreaID = d.HID where " + gCourseDescribe + " and ((left(c.HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and left(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "'and right(c.HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "')) " + gHOCPlace + gHArea + " and b.HStatus='1' AND b.HItemStatus='1'");

        while (QuerygTotalCT.Read())
        {
            gBookingCT = Convert.ToInt32(QuerygTotalCT["CT"].ToString());
        }
        QuerygTotalCT.Close();


        int gQueryArea = 0;

        //將HCourseBooking改成OrderList_Merge
        SqlDataReader QueryArea = SQLdatabase.ExecuteReader("select max(a.HAreaID) as HAreaID, max(d.HArea) as HArea from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID join HCourse as c on b.HCourseID = c.HID left join HArea as d on a.HAreaID=d.HID where " + gCourseDescribe + " and ((left(c.HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and left(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "'and right(c.HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "')) " + gHOCPlace + gHArea + " and b.HStatus='1'  and b.HItemStatus='1' group by a.HAreaID");

        while (QueryArea.Read())
        {
            //********表格標題開始********

            int gQueryHMCBCA = 0;
  
            //AE20230930_將HCourseBooking改成OrderList_Merge
            //EE20231122-條件式加入體系(HSystemName)；排序改為區屬->體系
            SqlDataReader QueryHMCBCA = SQLdatabase.ExecuteReader("select d.HArea, a.HPeriod, a.HID, a.HUserName, e.HSystemName from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID join HCourse as c on b.HCourseID = c.HID left join HArea as d on a.HAreaID = d.HID  LEFT JOIN HSystem AS e ON a.HSystemID =e.HID  where " + gCourseDescribe + " and ((left(c.HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and left(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "'and right(c.HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "')) " + gHOCPlace + gHArea + " and a.HAreaID = '" + QueryArea["HAreaID"].ToString() + "' and b.HStatus='1' and b.HItemStatus='1' group by d.HArea, a.HPeriod, a.HID, a.HUserName, e.HSystemName order by d.HArea ASC,e.HSystemName DESC");


            while (QueryHMCBCA.Read())
            {





                int gAllDateCT = 0;

                if (gCT == 0)
                {
                    TableRow HRow1 = new TableRow();
                    //TableRow HRow2 = new TableRow();

                    TableCell HCell1 = new TableCell();
                    HCell1.Text = "區屬";
                    HCell1.BorderWidth = 0;
                    HCell1.Width = Unit.Percentage(10);
                    HCell1.HorizontalAlign = HorizontalAlign.Center;
                    HCell1.CssClass = "font-weight-bold";
                    HRow1.Cells.Add(HCell1);

                    TableCell HCell2 = new TableCell();
                    HCell2.Text = "期別";
                    HCell2.BorderWidth = 0;
                    HCell2.Width = Unit.Percentage(5);
                    HCell2.HorizontalAlign = HorizontalAlign.Center;
                    HCell2.CssClass = "font-weight-bold";
                    HRow1.Cells.Add(HCell2);

                    TableCell HCell3 = new TableCell();
                    HCell3.Text = "姓名";
                    HCell3.BorderWidth = 0;
                    HCell3.Width = Unit.Percentage(10);
                    HCell3.HorizontalAlign = HorizontalAlign.Center;
                    HCell3.CssClass = "font-weight-bold";
                    HRow1.Cells.Add(HCell3);

                    //EA20231110-體系
                    TableCell HCell4 = new TableCell();
                    HCell4.Text = "體系";
                    HCell4.BorderWidth = 0;
                    HCell4.Width = Unit.Percentage(10);
                    HCell4.HorizontalAlign = HorizontalAlign.Center;
                    HCell4.CssClass = "font-weight-bold";
                    HRow1.Cells.Add(HCell4);


                    //往右延伸之標題
                    int gQueryHCourseCT = 0;
                    TableCell[] HCell = new TableCell[gCourseCT];

                    //GE20230930_將HCourseBooking改成OrderList_Merge
                    SqlDataReader QueryHCourseName = SQLdatabase.ExecuteReader("select Max(c.HCourseName) as HCourseName, c.HID as HCourseID, e.HPlaceName from OrderList_Merge as a join HMember as b on b.HID = a.HMemberID join HCourse as c on a.HCourseID = c.HID left join HArea as d on b.HAreaID = d.HID JOIN HPlace AS e ON c.HOCPlace=e.HID where " + gCourseDescribe + " and ((left(c.HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and left(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "'and right(c.HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "')) " + gHOCPlace + gHArea + " and a.HStatus='1' and a.HItemStatus='1' group by c.HID, e.HPlaceName order by c.HID");

                    while (QueryHCourseName.Read())
                    {
                        HCell[gQueryHCourseCT] = new TableCell();
                        //HCell[i].ColumnSpan = 3;
                        HCell[gQueryHCourseCT].Text = QueryHCourseName["HCourseName"].ToString() + "【" + QueryHCourseName["HPlaceName"].ToString() + "】";
                        HCell[gQueryHCourseCT].BorderWidth = 0;
                        HCell[gQueryHCourseCT].HorizontalAlign = HorizontalAlign.Center;
                        HCell[gQueryHCourseCT].CssClass = "font-weight-bold";

                        HRow1.Cells.Add(HCell[gQueryHCourseCT]);


                        gQueryHCourseCT++;
                    }
                    QueryHCourseName.Close();

                    TBL_ApplyRecord.Rows.Add(HRow1);

                }//if (gCT == 0)結束


                if (gQueryHMCBCA == 0)
                {
                    TableRow HRowArea = new TableRow();
                    TableCell HCellArea = new TableCell();
                    HCellArea.Text = QueryArea["HArea"].ToString();
                    HCellArea.ColumnSpan = Convert.ToInt32(gCourseCT + 4);
                    HCellArea.BorderWidth = 0;
                    HCellArea.BackColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
                    //HCellArea.Width = Unit.Percentage(10);
                    HCellArea.HorizontalAlign = HorizontalAlign.Center;
                    HRowArea.Cells.Add(HCellArea);
                    TBL_ApplyRecord.Rows.Add(HRowArea);

                }// if (gQueryArea==0) 結束



                //********表格標題結束********








                //********表格內容開始********

                gCT++;
                TableRow BRow = new TableRow();

                TableCell BCell1 = new TableCell();//區屬
                Label LB_BName1 = new Label();
                LB_BName1.ID = "HAreaID" + gCT.ToString();
                LB_BName1.Text = QueryHMCBCA["HArea"].ToString();
                LB_BName1.Style["text-align"] = "center";
                LB_BName1.BorderWidth = 0;
                LB_BName1.CssClass = "form-control data_cell";
                BCell1.BorderWidth = 0;
                //BCell1.HorizontalAlign = HorizontalAlign.Center;
                BCell1.Controls.Add(LB_BName1);
                BRow.Cells.Add(BCell1);

                TableCell BCell2 = new TableCell();//期別
                Label LB_BName2 = new Label();
                LB_BName2.ID = "HPeriod" + gCT.ToString();
                LB_BName2.Text = "&nbsp;" + QueryHMCBCA["HPeriod"].ToString();
                LB_BName2.Style["text-align"] = "center";
                LB_BName2.BorderWidth = 0;
                LB_BName2.CssClass = "form-control data_cell";
                BCell2.BorderWidth = 0;
                //BCell2.HorizontalAlign = HorizontalAlign.Center;
                BCell2.Controls.Add(LB_BName2);
                BRow.Cells.Add(BCell2);

                TableCell BCell3 = new TableCell();//姓名
                Label LB_BName3 = new Label();
                LB_BName3.ID = "HUserName" + gCT.ToString();
                LB_BName3.Text = QueryHMCBCA["HUserName"].ToString();
                LB_BName3.Style["text-align"] = "center";
                LB_BName3.BorderWidth = 0;
                LB_BName3.CssClass = "form-control data_cell";
                BCell3.BorderWidth = 0;
                //BCell3.HorizontalAlign = HorizontalAlign.Center;
                BCell3.Controls.Add(LB_BName3);
                BRow.Cells.Add(BCell3);

                //EA20231110-體系
                TableCell BCell4 = new TableCell();//體系
                Label LB_BName4 = new Label();
                LB_BName4.ID = "HSystemName" + gCT.ToString();
                LB_BName4.Text = QueryHMCBCA["HSystemName"].ToString();
                LB_BName4.Style["text-align"] = "center";
                LB_BName4.BorderWidth = 0;
                LB_BName4.CssClass = "form-control data_cell";
                BCell4.BorderWidth = 0;
                //BCell4.HorizontalAlign = HorizontalAlign.Center;
                BCell4.Controls.Add(LB_BName4);
                BRow.Cells.Add(BCell4);


                //往右延伸之內容
                int gQueryHCourseCK = 0;
                TableCell[] BCell = new TableCell[gCourseCT];

                //AE20230930_將HCourseBooking改成OrderList_Merge
                SqlDataReader QueryHCourseCK = SQLdatabase.ExecuteReader("select Max(c.HCourseName) as HCourseName, c.HID as HCourseID from OrderList_Merge as a join HMember as b on b.HID = a.HMemberID join HCourse as c on a.HCourseID = c.HID left join HArea as d on b.HAreaID = d.HID where " + gCourseDescribe + " and ((left(c.HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and left(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "'and right(c.HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(c.HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(c.HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "')) " + gHOCPlace + gHArea + " and a.HStatus='1'  and a.HItemStatus='1' group by c.HID order by c.HID");
                while (QueryHCourseCK.Read())
                {
                    BCell[gQueryHCourseCK] = new TableCell();
                    BCell[gQueryHCourseCK].BorderWidth = 0;
                    BCell[gQueryHCourseCK].HorizontalAlign = HorizontalAlign.Center;
                    BRow.Cells.Add(BCell[gQueryHCourseCK]);

                    //AE20230930_將HCourseBooking改成OrderList_Merge
                    SqlDataReader QueryHMemberCK = SQLdatabase.ExecuteReader("select Max(c.HCourseName) as HCourseName, c.HID as HCourseID from OrderList_Merge as a join HMember as b on b.HID = a.HMemberID join HCourse as c on a.HCourseID = c.HID left join HArea as d on b.HAreaID = d.HID where HCourseID ='" + QueryHCourseCK["HCourseID"].ToString() + "' and b.HID='" + QueryHMCBCA["HID"].ToString() + "' and a.HStatus='1' and a.HItemStatus='1' group by c.HID order by c.HID");

                    if (QueryHMemberCK.Read())
                    {
                        BCell[gQueryHCourseCK].Text = "1";
                        gQueryHCourseCK++;
                    }
                    else
                    {
                        BCell[gQueryHCourseCK].Text = "";
                    }
                    QueryHMemberCK.Close();
                }
                QueryHCourseCK.Close();
                gAllCT++;

                //}

                TBL_ApplyRecord.Rows.Add(BRow);







                //********表格內容結束********


                gQueryHMCBCA++;
            }
            QueryHMCBCA.Close();
            gQueryArea++;
        }
        QueryArea.Close();










        //****************************************使用動態 table結束 * *******************************************
    }

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









    protected void Page_Load(object sender, EventArgs e)
    {
        TB_Search.Attributes.Add("onkeypress", "if( event.keyCode == 13 ) { return false; }");
        TB_SearchDate.Attributes.Add("onkeypress", "if( event.keyCode == 13 ) { return false; }");



        if (!IsPostBack)
        {

            string strSelHPN = "select HID, HPlaceName, HStatus from HPlace order by HPlaceName";
            SqlDataReader MyQueryHPN = SQLdatabase.ExecuteReader(strSelHPN);
            while (MyQueryHPN.Read())
            {
                if (MyQueryHPN["HStatus"].ToString() == "0")
                {
                    LBox_HOCPlace.Items.Add(new ListItem(MyQueryHPN["HPlaceName"].ToString() + "(停用)", (MyQueryHPN["HID"].ToString())));
                }
                else
                {
                    LBox_HOCPlace.Items.Add(new ListItem(MyQueryHPN["HPlaceName"].ToString(), (MyQueryHPN["HID"].ToString())));
                }
            }
            MyQueryHPN.Close();

            string strSelHA = "select HID, HArea, HStatus from HArea order by HID ASC ";
            SqlDataReader MyQueryHA = SQLdatabase.ExecuteReader(strSelHA);
            DDL_HArea.Items.Add(new ListItem("-請選擇區屬-", "0"));
            while (MyQueryHA.Read())
            {
                if (MyQueryHA["HStatus"].ToString() == "0")
                {
                    DDL_HArea.Items.Add(new ListItem(MyQueryHA["HArea"].ToString() + "(停用)", (MyQueryHA["HID"].ToString())));
                }
                else
                {
                    DDL_HArea.Items.Add(new ListItem(MyQueryHA["HArea"].ToString(), (MyQueryHA["HID"].ToString())));
                }
            }
            MyQueryHA.Close();

        }


    }




    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        Panel_CourseList.Visible = true;//上課地點與區屬where條件

        string gHOCPlace = "";
        string gLBoxHOCPlace = "";
        string gHArea = "";

        if (TB_Search.Text.Trim() == "")  //故限制至少輸入一個條件式。
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('請輸入課程名稱!');", true);
            return;
        }

        //判斷上課地點與區屬有無輸入
        if (LBox_HOCPlace.SelectedValue == "" && DDL_HArea.SelectedValue == "0")//上課地點與區屬都沒輸入
        {
            gHOCPlace = "";
            gHArea = "";
        }
        else if (LBox_HOCPlace.SelectedValue != "" && DDL_HArea.SelectedValue == "0") //上課地點有輸入，區屬沒輸入
        {
            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    gLBoxHOCPlace += " c.HOCPlace='" + LBoxHOCPlace.Value + "' or";
                }
            }
            gHOCPlace = "AND (" + gLBoxHOCPlace.Substring(0, gLBoxHOCPlace.Length - 2) + ")";
            gHArea = "";
        }
        else if (LBox_HOCPlace.SelectedValue == "" && DDL_HArea.SelectedValue != "0") //上課地點沒輸入，區屬有輸入
        {
            gHOCPlace = "";
            gHArea = "AND d.HID='" + DDL_HArea.SelectedValue + "'";
        }
        else if (LBox_HOCPlace.SelectedValue != "" && DDL_HArea.SelectedValue != "0") //上課地點與區屬都有輸入
        {
            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    gLBoxHOCPlace += " c.HOCPlace='" + LBoxHOCPlace.Value + "' or";
                }
            }
            gHOCPlace = "AND (" + gLBoxHOCPlace.Substring(0, gLBoxHOCPlace.Length - 2) + ")";
            gHArea = "AND d.HID='" + DDL_HArea.SelectedValue + "'";
        }


        //判斷課程名稱與日期有無輸入
        if (TB_Search.Text.Trim() != "" && TB_SearchDate.Text.Trim() == "")
        {
            ShowITable("c.HCourseName like '%" + TB_Search.Text.Trim() + "%'", "2000/01/01 - 3000/12/31", gHOCPlace, gHArea);
        }
        else if (TB_Search.Text.Trim() != "" && TB_SearchDate.Text.Trim() != "")
        {
            ShowITable("c.HCourseName like '%" + TB_Search.Text.Trim() + "%'", TB_SearchDate.Text.Trim(), gHOCPlace, gHArea);
        }

    }



    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_Search.Text = "";
        TB_SearchDate.Text = "";
        LBox_HOCPlace.Text = null;
        DDL_HArea.SelectedValue = "0";
        string gHOCPlace = "";
        string gHArea = "";
        TBL_ApplyRecord.Dispose();
        //ShowITable("HCourseID like '%'", "2000/01/01 - 3000/12/31", gHOCPlace, gHArea);
    }



    protected void IBtn_ToExcel_Click(object sender, ImageClickEventArgs e)
    {
        Panel_CourseList.Visible = true;//上課地點與區屬where條件

        string gHOCPlace = "";
        string gLBoxHOCPlace = "";
        string gHArea = "";

        //判斷上課地點與區屬有無輸入
        if (LBox_HOCPlace.SelectedValue == "" && DDL_HArea.SelectedValue == "0")//上課地點與區屬都沒輸入
        {
            gHOCPlace = "";
            gHArea = "";
        }
        else if (LBox_HOCPlace.SelectedValue != "" && DDL_HArea.SelectedValue == "0") //上課地點有輸入，區屬沒輸入
        {
            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    gLBoxHOCPlace += " c.HOCPlace='" + LBoxHOCPlace.Value + "' or";
                }
            }
            gHOCPlace = "AND (" + gLBoxHOCPlace.Substring(0, gLBoxHOCPlace.Length - 2) + ")";
            gHArea = "";
        }
        else if (LBox_HOCPlace.SelectedValue == "" && DDL_HArea.SelectedValue != "0") //上課地點沒輸入，區屬有輸入
        {
            gHOCPlace = "";
            gHArea = "AND d.HID='" + DDL_HArea.SelectedValue + "'";
        }
        else if (LBox_HOCPlace.SelectedValue != "" && DDL_HArea.SelectedValue != "0") //上課地點與區屬都有輸入
        {
            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    gLBoxHOCPlace += " c.HOCPlace='" + LBoxHOCPlace.Value + "' or";
                }
            }
            gHOCPlace = "AND (" + gLBoxHOCPlace.Substring(0, gLBoxHOCPlace.Length - 2) + ")";
            gHArea = "AND d.HID='" + DDL_HArea.SelectedValue + "'";
        }


        //判斷課程名稱與日期有無輸入
        if (TB_Search.Text.Trim() != "" && TB_SearchDate.Text.Trim() == "")
        {
            ShowITable("c.HCourseName like '%" + TB_Search.Text.Trim() + "%'", "2000/01/01 - 3000/12/31", gHOCPlace, gHArea);
        }
        else if (TB_Search.Text.Trim() != "" && TB_SearchDate.Text.Trim() != "")
        {
            ShowITable("c.HCourseName like '%" + TB_Search.Text.Trim() + "%'", TB_SearchDate.Text.Trim(), gHOCPlace, gHArea);
        }



        Response.Clear();
        //通知瀏覽器下載檔案
        Response.AddHeader("content-disposition", "attachment;filename=單一課程參班記錄分析表.xls");
        Response.ContentType = "application/application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        System.IO.StringWriter sw = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter hw = new HtmlTextWriter(sw);
        TBL_ApplyRecord.RenderControl(hw);
        //Panel1.RenderControl(hw);
        //Response.Write(sw.ToString());

        System.IO.StreamWriter swUTF8 = new System.IO.StreamWriter(Response.OutputStream, System.Text.Encoding.UTF8);//這樣一筆的時侯,就不會亂碼了
        swUTF8.Write(sw.ToString());
        swUTF8.Close();


        Response.End();
    }

}