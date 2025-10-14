using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



public partial class HStayList : System.Web.UI.Page
{



    #region 動態table
    private void ShowITable(string gCourseID)
    {
        //*****************************使用動態 table開始*******************************//

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS3", ("$('.date').datepicker({format: 'dd/mm/yyyy', todayHighlight: true,orientation: 'bottom auto',autoclose: true,startDate:new Date(),});"), true);     //執行日曆js

        //int TRow = gTRow;
        int gCT = 0;//第幾筆資料
        //int gACT = 0;
        //int gACTAll = 0;
        //int gLCTAll = 0;

        //int gRCT = 0;
        int gAllCT = 0;//資料總筆數

        //string[] arr2 = new string[] { };
        string gSDate = "";
        string gEDate = "";

        //判斷天數相差幾天
        int gDateRange = 0;//連續天數
        //int gDateRangeCT = 0;

        string[] gDateRangeA = { };
        string[] gDateRangeB = { };//開課前一天起算住宿日
        //找住宿人員筆數與課程日期

        //AE20230930_將HCourseBooking改成OrderList_Merge&不用再join HCourse
        SqlDataReader QueryHMCBCAct = SQLdatabase.ExecuteReader("select Count(a.HID) as CT, b.HDateRange from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID  left join HArea as d on a.HAreaID=d.HID where b.HCourseID='" + gCourseID + "' and b.HStatus='1' AND b.HItemStatus='1' group by b.HDateRange");

        while (QueryHMCBCAct.Read())
        {




            if (QueryHMCBCAct["HDateRange"].ToString().IndexOf("-") >= 0)
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                string[] gDateArray = QueryHMCBCAct["HDateRange"].ToString().Split('-');
                string gDate = "";
                gSDate = (Convert.ToDateTime(gDateArray[0]).AddDays(-1)).ToString();
                gEDate = (gDateArray[1]);
                gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                //TableCell[] HCell = new TableCell[gDateRangeA.Length];
                for (int i = 0; i < gDateRange; i++)
                {
                    gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                }

                gDateRangeA = gDate.Trim(',').Split(',');
                gDateRangeB = gDate.Trim(',').Split(',');
            }
            else if (QueryHMCBCAct["HDateRange"].ToString().IndexOf(",") >= 0)
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                string[] gDateArray = QueryHMCBCAct["HDateRange"].ToString().Trim(',').Split(',');
                string gDate = "";
                gSDate = (Convert.ToDateTime(gDateArray[0]).AddDays(-1)).ToString();
                gEDate = (gDateArray[gDateArray.Length - 1]);
                gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                //TableCell[] HCell = new TableCell[gDateRangeA.Length];
                for (int i = 0; i < gDateRange; i++)
                {
                    gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                }

                gDateRangeA = gDate.Trim(',').Split(',');
                gDateRangeB = gDate.Trim(',').Split(',');
            }
            else if (QueryHMCBCAct["HDateRange"].ToString().IndexOf("-") < 0 && QueryHMCBCAct["HDateRange"].ToString().IndexOf(",") < 0)//單一日期
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                string[] gDateArray = (QueryHMCBCAct["HDateRange"].ToString() + " - " + QueryHMCBCAct["HDateRange"].ToString()).Split('-');
                string gDate = "";
                gSDate = (Convert.ToDateTime(gDateArray[0]).AddDays(-1)).ToString();
                gEDate = (gDateArray[1]);
                gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                //TableCell[] HCell = new TableCell[gDateRangeA.Length];
                for (int i = 0; i < gDateRange; i++)
                {
                    gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                }

                gDateRangeA = gDate.Trim(',').Split(',');
                gDateRangeB = gDate.Trim(',').Split(',');
            }
            else
            {

                Response.Write("<script>alert('日期錯誤!');window.location.href='HStayList.aspx';</script>");
                return;
            }

        }
        QueryHMCBCAct.Close();


        //AE20231122_加入體系資訊&不用再join HCourse&HPlace
        SqlDataReader QueryHMCBCA = SQLdatabase.ExecuteReader("SELECT a.HAreaID, a.HArea, a.HSystemName, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, a.HSex, a.HAge, b.HAttend, b.HOCPlace,b.HPlaceName, b.HLDate, b.HBDate, b.HRemark, b.HRoomRemark, f.HRoomName, b.HRoomTime FROM MemberList AS a JOIN OrderList_Merge AS b ON a.HID = b.HMemberID  LEFT JOIN HArea AS d ON a.HAreaID=d.HID LEFT JOIN HRoom AS f ON b.HRoom=f.HID  WHERE b.HCourseID='" + gCourseID + "' AND b.HStatus='1' AND b.HItemStatus='1' AND (b.HLodging = '1')  ORDER BY b.HCreateDT ASC");

        while (QueryHMCBCA.Read())
        {


            //********表格標題開始********
            TableRow HRow1 = new TableRow();
            TableRow HRow2 = new TableRow();
            TableCell HCell1 = new TableCell();
            TableCell HCell2 = new TableCell();
            TableCell HCell3 = new TableCell();
            TableCell HCell15 = new TableCell();
            TableCell HCell4 = new TableCell();
            TableCell HCell5 = new TableCell();
            TableCell HCell6 = new TableCell();
            TableCell HCell7 = new TableCell();
            TableCell HCell14 = new TableCell();
            TableCell HCell8 = new TableCell();
            TableCell HCell9 = new TableCell();
            TableCell HCell10 = new TableCell();
            TableCell HCell11 = new TableCell();
            TableCell HCell12 = new TableCell();
            TableCell HCell13 = new TableCell();
            //TableCell HCell14 = new TableCell();



            HCell1.Text = "編號";
            HCell2.Text = "區屬";
            HCell3.Text = "光團";
            HCell15.Text = "體系";
            HCell4.Text = "期別";
            HCell5.Text = "姓名";
            HCell6.Text = "性別";
            HCell7.Text = "年齡";
            HCell14.Text = "報名日期";
            HCell8.Text = "參班/護持任務";
            HCell9.Text = "上課地點";
            HCell10.Text = "住宿區";
            HCell11.Text = "住宿日期";
            HCell12.Text = "入住時間";
            HCell13.Text = "備註";
            //HCell14.Text = "開班前一天是否需要用餐";

            HCell1.BorderWidth = 1;
            HCell2.BorderWidth = 1;
            HCell3.BorderWidth = 1;
            HCell15.BorderWidth = 1;
            HCell4.BorderWidth = 1;
            HCell5.BorderWidth = 1;
            HCell6.BorderWidth = 1;
            HCell7.BorderWidth = 1;
            HCell14.BorderWidth = 1;
            HCell8.BorderWidth = 1;
            HCell9.BorderWidth = 1;
            HCell10.BorderWidth = 1;
            HCell11.BorderWidth = 1;
            HCell12.BorderWidth = 1;
            HCell13.BorderWidth = 1;
            //HCell14.BorderWidth = 1;

            HCell1.Width = Unit.Percentage(3);
            HCell2.Width = Unit.Percentage(10);
            HCell3.Width = Unit.Percentage(8);
            HCell15.Width = Unit.Percentage(6);
            HCell4.Width = Unit.Percentage(5);
            HCell5.Width = Unit.Percentage(10);
            HCell6.Width = Unit.Percentage(5);
            HCell7.Width = Unit.Percentage(5);
            HCell14.Width = Unit.Percentage(5);
            HCell8.Width = Unit.Percentage(9);
            HCell9.Width = Unit.Percentage(10);
            HCell10.Width = Unit.Percentage(10);
            HCell11.Width = Unit.Percentage(10);
            HCell12.Width = Unit.Percentage(10);
            HCell13.Width = Unit.Percentage(5);
            //HCell14.Width = Unit.Percentage(10);

            HCell1.HorizontalAlign = HorizontalAlign.Center;
            HCell2.HorizontalAlign = HorizontalAlign.Left;
            HCell3.HorizontalAlign = HorizontalAlign.Left;
            HCell15.HorizontalAlign = HorizontalAlign.Left;
            HCell4.HorizontalAlign = HorizontalAlign.Center;
            HCell5.HorizontalAlign = HorizontalAlign.Left;
            HCell6.HorizontalAlign = HorizontalAlign.Center;
            HCell7.HorizontalAlign = HorizontalAlign.Center;
            HCell14.HorizontalAlign = HorizontalAlign.Center;
            HCell8.HorizontalAlign = HorizontalAlign.Center;
            HCell9.HorizontalAlign = HorizontalAlign.Center;
            HCell10.HorizontalAlign = HorizontalAlign.Center;
            HCell11.HorizontalAlign = HorizontalAlign.Center;
            HCell12.HorizontalAlign = HorizontalAlign.Center;
            HCell13.HorizontalAlign = HorizontalAlign.Center;
            // HCell14.HorizontalAlign = HorizontalAlign.Center;

            HCell1.CssClass = "font-weight-bold";
            HCell2.CssClass = "font-weight-bold";
            HCell3.CssClass = "font-weight-bold";
            HCell15.CssClass = "font-weight-bold";
            HCell4.CssClass = "font-weight-bold";
            HCell5.CssClass = "font-weight-bold";
            HCell6.CssClass = "font-weight-bold";
            HCell7.CssClass = "font-weight-bold";
            HCell14.CssClass = "font-weight-bold";
            HCell8.CssClass = "font-weight-bold";
            HCell9.CssClass = "font-weight-bold";
            HCell10.CssClass = "font-weight-bold";
            HCell11.CssClass = "font-weight-bold";
            HCell12.CssClass = "font-weight-bold";
            HCell13.CssClass = "font-weight-bold";
            //HCell14.CssClass = "font-weight-bold";



            int gAllDateCT = 0;

            if (gCT == 0)
            {


                HRow1.Cells.Add(HCell1);
                HRow1.Cells.Add(HCell2);
                HRow1.Cells.Add(HCell3);
                HRow1.Cells.Add(HCell15);
                HRow1.Cells.Add(HCell4);
                HRow1.Cells.Add(HCell5);
                HRow1.Cells.Add(HCell6);
                HRow1.Cells.Add(HCell7);
                HRow1.Cells.Add(HCell14);
                HRow1.Cells.Add(HCell8);
                HRow1.Cells.Add(HCell9);
                HRow1.Cells.Add(HCell10);
                HRow1.Cells.Add(HCell11);
                HRow1.Cells.Add(HCell12);
                //HRow1.Cells.Add(HCell14);






                TableCell[] HCell = new TableCell[gDateRangeA.Length];
                for (int i = 0; i < gDateRangeA.Length; i++)
                {
                    HCell[i] = new TableCell();
                    //HCell[i].ColumnSpan = 3;
                    //HCell[i].Text = gDate;
                    HCell[i].Text = Convert.ToDateTime(gDateRangeA[i]).ToString("MM/dd");
                    HCell[i].BorderWidth = 1;
                    HCell[i].HorizontalAlign = HorizontalAlign.Center;
                    HCell[i].CssClass = "font-weight-bold";

                    HRow1.Cells.Add(HCell[i]);



                }

                HRow1.Cells.Add(HCell13);


                TBL_StayList.Rows.Add(HRow1);
                TBL_StayList.CssClass = "table table-hover table-responsive";
                //TBL_StayList.Rows.Add(HRow2);

            }
            //********表格標題結束********








            //********表格內容開始********

            gCT++;
            TableRow BRow = new TableRow();

            TableCell BCell1 = new TableCell();//編號
            TableCell BCell2 = new TableCell();//區屬
            TableCell BCell15 = new TableCell();//體系
            TableCell BCell3 = new TableCell();//光團
            TableCell BCell4 = new TableCell();//期別
            TableCell BCell5 = new TableCell();//姓名
            TableCell BCell6 = new TableCell();//工作項目
            TableCell BCell7 = new TableCell();//工作項目
            TableCell BCell14 = new TableCell();//工作項目
            TableCell BCell8 = new TableCell();//工作項目
            TableCell BCell9 = new TableCell();//工作項目
            TableCell BCell10 = new TableCell();//工作項目
            TableCell BCell11 = new TableCell();//工作項目
            TableCell BCell12 = new TableCell();//入住時間
            TableCell BCell13 = new TableCell();//工作項目
            //TableCell BCell14 = new TableCell();//工作項目

            BCell1.HorizontalAlign = HorizontalAlign.Center;
            BCell6.HorizontalAlign = HorizontalAlign.Center;
            BCell7.HorizontalAlign = HorizontalAlign.Center;
            BCell11.HorizontalAlign = HorizontalAlign.Left;

            Label LB_BName1 = new Label();
            Label LB_BName2 = new Label();
            Label LB_BName3 = new Label();
            Label LB_BName15 = new Label();
            Label LB_BName4 = new Label();
            Label LB_BName5 = new Label();
            Label LB_BName6 = new Label();
            Label LB_BName7 = new Label();
            Label LB_BName14 = new Label();
            Label LB_BName8 = new Label();
            Label LB_BName9 = new Label();
            Label LB_BName10 = new Label();
            Label LB_BName11 = new Label();
            Label LB_BName12 = new Label();
            Label LB_BName13 = new Label();

            LB_BName1.ID = "gCT" + gCT.ToString();
            LB_BName2.ID = "HAreaID" + gCT.ToString();
            LB_BName15.ID = "HSystemName" + gCT.ToString();
            LB_BName3.ID = "HTeamID" + gCT.ToString();
            LB_BName4.ID = "HPeriod" + gCT.ToString();
            LB_BName5.ID = "HUserName" + gCT.ToString();
            LB_BName6.ID = "HSex" + gCT.ToString();
            LB_BName7.ID = "HAge" + gCT.ToString();
            LB_BName14.ID = "HBDate" + gCT.ToString();
            LB_BName8.ID = "HAttend" + gCT.ToString();
            LB_BName9.ID = "HOCPlace" + gCT.ToString();
            LB_BName10.ID = "xxx" + gCT.ToString();
            LB_BName11.ID = "HLDate" + gCT.ToString();
            LB_BName12.ID = "HRoomTime" + gCT.ToString();
            LB_BName13.ID = "HRemark" + gCT.ToString();
            //LB_BName14.ID = "xxxx" + gCT.ToString();

            LB_BName1.Text = gCT.ToString();
            LB_BName2.Text = QueryHMCBCA["HArea"].ToString();



            string gHTeamType = "";
            string gHTeamName = "";
            string[] gHTeamID = QueryHMCBCA["HTeamID"].ToString().Split(',');
            if (gHTeamID.Length > 1)
            {
                gHTeamType = gHTeamID[1] == "1" ? "HMTeam" : "HCTeam";
                gHTeamName = gHTeamID[1] == "1" ? "HMTeam" : "HCTeam";
                SqlDataReader QueryHTeam = SQLdatabase.ExecuteReader("select " + gHTeamName + " from " + gHTeamType + " where HID='" + gHTeamID[0] + "'");
                while (QueryHTeam.Read())
                {
                    LB_BName3.Text = QueryHTeam[gHTeamName].ToString();
                }
                QueryHTeam.Close();
            }
            else
            {
                LB_BName3.Text = "";
            }



            LB_BName4.Text = "&nbsp;" + QueryHMCBCA["HPeriod"].ToString();
            LB_BName5.Text = QueryHMCBCA["HUserName"].ToString();
            //1=男、2=女
            LB_BName6.Text = QueryHMCBCA["HSex"].ToString() == "1" ? "男" : "女"; ;
            LB_BName7.Text = "";
            LB_BName14.Text = QueryHMCBCA["HBDate"].ToString();
            //1 = 參班【一般】、2 = 參班【學青(25歲以下在學青年)】、3 = 參班【經濟困難(需經光團1號導師審核通過)】、4 = 參班【經濟困難(護持出家師父及全心奉獻各修行組織團體之清修人士(請光團1號導師協助確認)】、5 = 不參班(專業護持)、6 = 不參班(純護持)
            LB_BName8.Text = QueryHMCBCA["HAttend"].ToString() == "5" ? "專業護持者" : QueryHMCBCA["HAttend"].ToString() == "6" ? "純護持" : QueryHMCBCA["HAttend"].ToString() == "" || QueryHMCBCA["HAttend"].ToString() == null ? "未選擇" : QueryHMCBCA["HAttend"].ToString() == "0" ? "" : "參班";
            LB_BName9.Text = QueryHMCBCA["HPlaceName"].ToString();
            LB_BName10.Text = QueryHMCBCA["HRoomName"].ToString();
            LB_BName11.Text = QueryHMCBCA["HLDate"].ToString();
            LB_BName12.Text = QueryHMCBCA["HRoomTime"].ToString();
            //LB_BName13.Text = QueryHMCBCA["HRemark"].ToString();
            LB_BName13.Text = QueryHMCBCA["HRemark"].ToString() + "<br/>" + QueryHMCBCA["HRoomRemark"].ToString();
            //LB_BName14.Text = "";
            LB_BName15.Text = QueryHMCBCA["HSystemName"].ToString();


            LB_BName1.CssClass = "form-control staylist_cell text-center";
            LB_BName2.CssClass = "form-control staylist_cell text-left";
            LB_BName3.CssClass = "form-control staylist_cell text-left";
            LB_BName4.CssClass = "form-control staylist_cell text-center";
            LB_BName5.CssClass = "form-control staylist_cell text-left";
            LB_BName6.CssClass = "form-control staylist_cell text-center";
            LB_BName7.CssClass = "form-control staylist_cell text-center";
            LB_BName14.CssClass = "form-control staylist_cell text-center";
            LB_BName8.CssClass = "form-control staylist_cell text-left";
            LB_BName9.CssClass = "form-control staylist_cell text-left";
            LB_BName10.CssClass = "form-control staylist_cell text-left";
            LB_BName11.CssClass = "form-control staylist_cell text-left";
            LB_BName12.CssClass = "form-control staylist_cell text-left";
            LB_BName13.CssClass = "form-control staylist_cell text-left";
            LB_BName15.CssClass = "form-control staylist_cell text-center";
            //LB_BName14.CssClass = "form-control staylist_cell text-left";


            BCell1.BorderWidth = 1;
            BCell2.BorderWidth = 1;
            BCell3.BorderWidth = 1;
            BCell15.BorderWidth = 1;
            BCell4.BorderWidth = 1;
            BCell5.BorderWidth = 1;
            BCell6.BorderWidth = 1;
            BCell7.BorderWidth = 1;
            BCell14.BorderWidth = 1;
            BCell8.BorderWidth = 1;
            BCell9.BorderWidth = 1;
            BCell10.BorderWidth = 1;
            BCell11.BorderWidth = 1;
            BCell12.BorderWidth = 1;
            BCell13.BorderWidth = 1;
            //BCell14.BorderWidth = 1;

            BCell1.Controls.Add(LB_BName1);
            BCell2.Controls.Add(LB_BName2);
            BCell3.Controls.Add(LB_BName3);
            BCell15.Controls.Add(LB_BName15);
            BCell4.Controls.Add(LB_BName4);
            BCell5.Controls.Add(LB_BName5);
            BCell6.Controls.Add(LB_BName6);
            BCell7.Controls.Add(LB_BName7);
            BCell14.Controls.Add(LB_BName14);
            BCell8.Controls.Add(LB_BName8);
            BCell9.Controls.Add(LB_BName9);
            BCell10.Controls.Add(LB_BName10);
            BCell11.Controls.Add(LB_BName11);
            BCell12.Controls.Add(LB_BName12);
            BCell13.Controls.Add(LB_BName13);
            //BCell14.Controls.Add(LB_BName14);

            BRow.Cells.Add(BCell1);
            BRow.Cells.Add(BCell2);
            BRow.Cells.Add(BCell3);
            BRow.Cells.Add(BCell15);
            BRow.Cells.Add(BCell4);
            BRow.Cells.Add(BCell5);
            BRow.Cells.Add(BCell6);
            BRow.Cells.Add(BCell7);
            BRow.Cells.Add(BCell14);
            BRow.Cells.Add(BCell8);
            BRow.Cells.Add(BCell9);
            BRow.Cells.Add(BCell10);
            BRow.Cells.Add(BCell11);
            BRow.Cells.Add(BCell12);

            string gHLodging = "0";//顯示住宿與列表日是否同天，是=1、否=0
            string gHAStatus = "";


            TableCell[] BCell = new TableCell[gDateRangeA.Length];















            //找住宿日期

            //AE20230930_將HCourseBooking改成OrderList_Merge
            SqlDataReader QueryHRM = SQLdatabase.ExecuteReader("SELECT a.HAStatus, c.HLodging, c.HLDate, b.HUserName FROM HRollCall as a LEFT JOIN HMember as b on a.HMemberID = b.HID left join OrderList_Merge as c on a.HMemberID=c.HMemberID and a.HCourseID=c.HCourseID WHERE a.HCourseID = '" + QueryHMCBCA["HCourseID"].ToString() + "' and a.HmemberID='" + QueryHMCBCA["HID"].ToString() + "' and c.HStatus='1' AND c.HItemStatus='1'");
            if (QueryHRM.Read())
            {

                for (int i = 0; i < gDateRangeA.Length; i++)
                {

                    //HAStatus，0=請選擇、1=實體、2=連線、3=遲到、4=請假
                    gHAStatus = QueryHRM["HAStatus"].ToString() == "1" ? "" : QueryHRM["HAStatus"].ToString() == "2" ? "" : QueryHRM["HAStatus"].ToString() == "3" ? "" : QueryHRM["HAStatus"].ToString() == "4" ? "請假" : "";

                    if (QueryHRM["HLodging"].ToString() == "1")
                    {
                        if (QueryHRM["HLDate"].ToString().IndexOf("-") >= 0)
                        {
                            string[] gHLDate1 = QueryHRM["HLDate"].ToString().Split('-');//人員連續住宿日期

                            DateTime gHLSDate = Convert.ToDateTime(gHLDate1[0]);//人員住宿開始日
                            DateTime gHLEDate = Convert.ToDateTime(gHLDate1[1]);//人員住宿結束日
                            int gHLDate2 = (gHLEDate - gHLSDate).Days + 1;//人員連續住宿天數
                            string gHLDate = "";//人員住宿日期
                            for (int i1 = 0; i1 < gHLDate2; i1++)
                            {
                                //gHLDate：住宿日期
                                gHLDate = gHLSDate.AddDays(i1).ToString("yyyy/MM/dd");

                                if (gHLDate == gDateRangeA[i])//住宿日期=列表日期
                                {
                                    gHLodging = "1";
                                }

                            }

                        }
                        else if (QueryHRM["HLDate"].ToString().IndexOf(",") >= 0)
                        {
                            string[] gHLDate1 = QueryHRM["HLDate"].ToString().Split(',');//人員連續住宿日期
                            DateTime gHLSDate = Convert.ToDateTime(gHLDate1[0]);//人員住宿開始日
                            DateTime gHLEDate = Convert.ToDateTime(gHLDate1[gHLDate1.Length - 1]);//人員住宿結束日
                            int gHLDate2 = (gHLEDate - gHLSDate).Days + 1;//人員連續住宿天數
                            string gHLDate = "";//人員住宿日期
                            for (int i1 = 0; i1 < gHLDate2; i1++)
                            {
                                //gHLDate：住宿日期
                                gHLDate = gHLSDate.AddDays(i1).ToString("yyyy/MM/dd");

                                if (gHLDate == gDateRangeA[i])//住宿日期=列表日期
                                {
                                    gHLodging = "1";
                                }

                            }

                        }
                        else
                        {

                            if (QueryHRM["HLDate"].ToString() == gDateRangeA[i])//住宿日期=列表日期
                            {
                                gHLodging = "1";
                            }
                        }
                    }





                    for (int j = 0; j <= 0; j++)//j必定等於0
                    {
                        BCell[j] = new TableCell();

                        if (j == 0)
                        {
                            BCell[j].BorderWidth = 1;
                            BCell[j].HorizontalAlign = HorizontalAlign.Center;
                            BRow.Cells.Add(BCell[j]);//印出內容cell的格子
                                                     //gAll[gACT] = gHLodging == "1" ? Convert.ToString(Convert.ToInt32(gAll[gACT]) + 1) : Convert.ToString(Convert.ToInt32(gAll[gACT]) + 0);//暫用不到，待日後刪除
                            BCell[j].Text = gHLodging;//有住宿的話gHLodging=1，沒住宿gHLodging=0
                                                      //gACT++;
                        }

                    }
                    //gAPeople = "0";
                    gHLodging = "0";

                }

            }

            QueryHRM.Close();

























            BRow.Cells.Add(BCell13);

            TBL_StayList.Rows.Add(BRow);






            //********表格內容結束********



        }
        QueryHMCBCA.Close();










        //****************************************使用動態 table結束 * *******************************************
    }

    #endregion



    #region 動態table(開放單日報名)
    private void ShowIBBDateTable(string gCourseID)
    {
        //Response.Write(DDL_OGNum.SelectedValue);
        //*****************************使用動態 table開始*******************************//

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS3", ("$('.date').datepicker({format: 'dd/mm/yyyy', todayHighlight: true,orientation: 'bottom auto',autoclose: true,startDate:new Date(),});"), true);     //執行日曆js

        //int TRow = gTRow;
        int gCT = 0;//第幾筆資料
        //int gACT = 0;
        //int gACTAll = 0;
        //int gLCTAll = 0;

        //int gRCT = 0;
        int gAllCT = 0;//資料總筆數

        //string[] arr2 = new string[] { };
        string gSDate = "";
        string gEDate = "";

        //判斷天數相差幾天
        int gDateRange = 0;//連續天數
        //int gDateRangeCT = 0;

        string[] gDateRangeA = { };
        string[] gDateRangeB = { };//開課前一天起算住宿日
        //找住宿人員筆數與課程日期

        //AE20230930_將HCourseBooking改成OrderList_Merge&不用再join HCourse
        SqlDataReader QueryHMCBCAct = SQLdatabase.ExecuteReader("select Count(a.HID) as CT, b.HDateRange from HMember as a join HCourseBooking as b on a.HID = b.HMemberID  left join HArea as d on a.HAreaID=d.HID where b.HCourseID='" + gCourseID + "' and b.HStatus='1' AND b.HItemStatus='1' group by b.HDateRange");

        while (QueryHMCBCAct.Read())
        {




            if (QueryHMCBCAct["HDateRange"].ToString().IndexOf("-") >= 0)
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                string[] gDateArray = QueryHMCBCAct["HDateRange"].ToString().Split('-');
                string gDate = "";
                gSDate = (Convert.ToDateTime(gDateArray[0]).AddDays(-1)).ToString();
                gEDate = (gDateArray[1]);
                gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                //TableCell[] HCell = new TableCell[gDateRangeA.Length];
                for (int i = 0; i < gDateRange; i++)
                {
                    gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                }

                gDateRangeA = gDate.Trim(',').Split(',');
                gDateRangeB = gDate.Trim(',').Split(',');
            }
            else if (QueryHMCBCAct["HDateRange"].ToString().IndexOf(",") >= 0)
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                string[] gDateArray = QueryHMCBCAct["HDateRange"].ToString().Trim(',').Split(',');
                string gDate = "";
                gSDate = (Convert.ToDateTime(gDateArray[0]).AddDays(-1)).ToString();
                gEDate = (gDateArray[gDateArray.Length - 1]);
                gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                //TableCell[] HCell = new TableCell[gDateRangeA.Length];
                for (int i = 0; i < gDateRange; i++)
                {
                    gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                }

                gDateRangeA = gDate.Trim(',').Split(',');
                gDateRangeB = gDate.Trim(',').Split(',');
            }
            else if (QueryHMCBCAct["HDateRange"].ToString().IndexOf("-") < 0 && QueryHMCBCAct["HDateRange"].ToString().IndexOf(",") < 0)//單一日期
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                string[] gDateArray = (QueryHMCBCAct["HDateRange"].ToString() + " - " + QueryHMCBCAct["HDateRange"].ToString()).Split('-');
                string gDate = "";
                gSDate = (Convert.ToDateTime(gDateArray[0]).AddDays(-1)).ToString();
                gEDate = (gDateArray[1]);
                gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                //TableCell[] HCell = new TableCell[gDateRangeA.Length];
                for (int i = 0; i < gDateRange; i++)
                {
                    gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                }

                gDateRangeA = gDate.Trim(',').Split(',');
                gDateRangeB = gDate.Trim(',').Split(',');
            }
            else
            {

                Response.Write("<script>alert('日期錯誤!');window.location.href='HStayList.aspx';</script>");
                return;
            }

        }
        QueryHMCBCAct.Close();

        //string[] gAll = new string[gDateRange * Convert.ToInt32(gAllCT)];//暫用不到，待日後刪除




        //AE20231122_加入體系資訊&不用再join HCourse&HPlace
        SqlDataReader QueryHMCBCA = SQLdatabase.ExecuteReader("SELECT a.HAreaID, a.HArea, a.HSystemName, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, a.HSex, a.HAge, b.HAttend, b.HOCPlace,b.HPlaceName, b.HLDate, b.HBDate, b.HRemark, b.HRoomRemark, f.HRoomName, b.HRoomTime FROM MemberList AS a JOIN HCourseBooking AS b ON a.HID = b.HMemberID  LEFT JOIN HArea AS d ON a.HAreaID=d.HID LEFT JOIN HRoom AS f ON b.HRoom=f.HID  WHERE b.HCourseID='" + gCourseID + "' AND b.HStatus='1' AND b.HItemStatus='1' AND (b.HLodging = '1') GROP BY a.HAreaID, a.HArea, a.HSystemName, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, a.HSex, a.HAge, b.HAttend, b.HOCPlace,b.HPlaceName, b.HLDate, b.HBDate, b.HRemark, b.HRoomRemark, f.HRoomName, b.HRoomTime  ORDER BY b.HCreateDT ASC");


        while (QueryHMCBCA.Read())
        {


            //********表格標題開始********
            TableRow HRow1 = new TableRow();
            TableRow HRow2 = new TableRow();
            TableCell HCell1 = new TableCell();
            TableCell HCell2 = new TableCell();
            TableCell HCell3 = new TableCell();
            TableCell HCell15 = new TableCell();
            TableCell HCell4 = new TableCell();
            TableCell HCell5 = new TableCell();
            TableCell HCell6 = new TableCell();
            TableCell HCell7 = new TableCell();
            TableCell HCell14 = new TableCell();
            TableCell HCell8 = new TableCell();
            TableCell HCell9 = new TableCell();
            TableCell HCell10 = new TableCell();
            TableCell HCell11 = new TableCell();
            TableCell HCell12 = new TableCell();
            TableCell HCell13 = new TableCell();
            //TableCell HCell14 = new TableCell();



            HCell1.Text = "編號";
            HCell2.Text = "區屬";
            HCell3.Text = "光團";
            HCell15.Text = "體系";
            HCell4.Text = "期別";
            HCell5.Text = "姓名";
            HCell6.Text = "性別";
            HCell7.Text = "年齡";
            HCell14.Text = "報名日期";
            HCell8.Text = "參班/護持任務";
            HCell9.Text = "上課地點";
            HCell10.Text = "住宿區";
            HCell11.Text = "住宿日期";
            HCell12.Text = "入住時間";
            HCell13.Text = "備註";
            //HCell14.Text = "開班前一天是否需要用餐";

            HCell1.BorderWidth = 1;
            HCell2.BorderWidth = 1;
            HCell3.BorderWidth = 1;
            HCell15.BorderWidth = 1;
            HCell4.BorderWidth = 1;
            HCell5.BorderWidth = 1;
            HCell6.BorderWidth = 1;
            HCell7.BorderWidth = 1;
            HCell14.BorderWidth = 1;
            HCell8.BorderWidth = 1;
            HCell9.BorderWidth = 1;
            HCell10.BorderWidth = 1;
            HCell11.BorderWidth = 1;
            HCell12.BorderWidth = 1;
            HCell13.BorderWidth = 1;
            //HCell14.BorderWidth = 1;

            HCell1.Width = Unit.Percentage(3);
            HCell2.Width = Unit.Percentage(10);
            HCell3.Width = Unit.Percentage(8);
            HCell15.Width = Unit.Percentage(6);
            HCell4.Width = Unit.Percentage(5);
            HCell5.Width = Unit.Percentage(10);
            HCell6.Width = Unit.Percentage(5);
            HCell7.Width = Unit.Percentage(5);
            HCell14.Width = Unit.Percentage(5);
            HCell8.Width = Unit.Percentage(9);
            HCell9.Width = Unit.Percentage(10);
            HCell10.Width = Unit.Percentage(10);
            HCell11.Width = Unit.Percentage(10);
            HCell12.Width = Unit.Percentage(10);
            HCell13.Width = Unit.Percentage(5);
            //HCell14.Width = Unit.Percentage(10);

            HCell1.HorizontalAlign = HorizontalAlign.Center;
            HCell2.HorizontalAlign = HorizontalAlign.Left;
            HCell3.HorizontalAlign = HorizontalAlign.Left;
            HCell15.HorizontalAlign = HorizontalAlign.Left;
            HCell4.HorizontalAlign = HorizontalAlign.Center;
            HCell5.HorizontalAlign = HorizontalAlign.Left;
            HCell6.HorizontalAlign = HorizontalAlign.Center;
            HCell7.HorizontalAlign = HorizontalAlign.Center;
            HCell14.HorizontalAlign = HorizontalAlign.Center;
            HCell8.HorizontalAlign = HorizontalAlign.Center;
            HCell9.HorizontalAlign = HorizontalAlign.Center;
            HCell10.HorizontalAlign = HorizontalAlign.Center;
            HCell11.HorizontalAlign = HorizontalAlign.Center;
            HCell12.HorizontalAlign = HorizontalAlign.Center;
            HCell13.HorizontalAlign = HorizontalAlign.Center;
            // HCell14.HorizontalAlign = HorizontalAlign.Center;

            HCell1.CssClass = "font-weight-bold";
            HCell2.CssClass = "font-weight-bold";
            HCell3.CssClass = "font-weight-bold";
            HCell15.CssClass = "font-weight-bold";
            HCell4.CssClass = "font-weight-bold";
            HCell5.CssClass = "font-weight-bold";
            HCell6.CssClass = "font-weight-bold";
            HCell7.CssClass = "font-weight-bold";
            HCell14.CssClass = "font-weight-bold";
            HCell8.CssClass = "font-weight-bold";
            HCell9.CssClass = "font-weight-bold";
            HCell10.CssClass = "font-weight-bold";
            HCell11.CssClass = "font-weight-bold";
            HCell12.CssClass = "font-weight-bold";
            HCell13.CssClass = "font-weight-bold";
            //HCell14.CssClass = "font-weight-bold";



            int gAllDateCT = 0;

            if (gCT == 0)
            {


                HRow1.Cells.Add(HCell1);
                HRow1.Cells.Add(HCell2);
                HRow1.Cells.Add(HCell3);
                HRow1.Cells.Add(HCell15);
                HRow1.Cells.Add(HCell4);
                HRow1.Cells.Add(HCell5);
                HRow1.Cells.Add(HCell6);
                HRow1.Cells.Add(HCell7);
                HRow1.Cells.Add(HCell14);
                HRow1.Cells.Add(HCell8);
                HRow1.Cells.Add(HCell9);
                HRow1.Cells.Add(HCell10);
                HRow1.Cells.Add(HCell11);
                HRow1.Cells.Add(HCell12);
                //HRow1.Cells.Add(HCell14);






                TableCell[] HCell = new TableCell[gDateRangeA.Length];
                for (int i = 0; i < gDateRangeA.Length; i++)
                {
                    HCell[i] = new TableCell();
                    //HCell[i].ColumnSpan = 3;
                    //HCell[i].Text = gDate;
                    HCell[i].Text = Convert.ToDateTime(gDateRangeA[i]).ToString("MM/dd");
                    HCell[i].BorderWidth = 1;
                    HCell[i].HorizontalAlign = HorizontalAlign.Center;
                    HCell[i].CssClass = "font-weight-bold";

                    HRow1.Cells.Add(HCell[i]);



                }

                HRow1.Cells.Add(HCell13);


                TBL_StayList.Rows.Add(HRow1);
                TBL_StayList.CssClass = "table table-hover table-responsive";
                //TBL_StayList.Rows.Add(HRow2);

            }
            //********表格標題結束********








            //********表格內容開始********

            gCT++;
            TableRow BRow = new TableRow();

            TableCell BCell1 = new TableCell();//編號
            TableCell BCell2 = new TableCell();//區屬
            TableCell BCell15 = new TableCell();//體系
            TableCell BCell3 = new TableCell();//光團
            TableCell BCell4 = new TableCell();//期別
            TableCell BCell5 = new TableCell();//姓名
            TableCell BCell6 = new TableCell();//工作項目
            TableCell BCell7 = new TableCell();//工作項目
            TableCell BCell14 = new TableCell();//工作項目
            TableCell BCell8 = new TableCell();//工作項目
            TableCell BCell9 = new TableCell();//工作項目
            TableCell BCell10 = new TableCell();//工作項目
            TableCell BCell11 = new TableCell();//工作項目
            TableCell BCell12 = new TableCell();//入住時間
            TableCell BCell13 = new TableCell();//工作項目
            //TableCell BCell14 = new TableCell();//工作項目

            BCell1.HorizontalAlign = HorizontalAlign.Center;
            BCell6.HorizontalAlign = HorizontalAlign.Center;
            BCell7.HorizontalAlign = HorizontalAlign.Center;
            BCell11.HorizontalAlign = HorizontalAlign.Left;

            Label LB_BName1 = new Label();
            Label LB_BName2 = new Label();
            Label LB_BName3 = new Label();
            Label LB_BName15 = new Label();
            Label LB_BName4 = new Label();
            Label LB_BName5 = new Label();
            Label LB_BName6 = new Label();
            Label LB_BName7 = new Label();
            Label LB_BName14 = new Label();
            Label LB_BName8 = new Label();
            Label LB_BName9 = new Label();
            Label LB_BName10 = new Label();
            Label LB_BName11 = new Label();
            Label LB_BName12 = new Label();
            Label LB_BName13 = new Label();
            //Label LB_BName14 = new Label();


            LB_BName1.ID = "gCT" + gCT.ToString();
            LB_BName2.ID = "HAreaID" + gCT.ToString();
            LB_BName15.ID = "HSystemName" + gCT.ToString();
            LB_BName3.ID = "HTeamID" + gCT.ToString();
            LB_BName4.ID = "HPeriod" + gCT.ToString();
            LB_BName5.ID = "HUserName" + gCT.ToString();
            LB_BName6.ID = "HSex" + gCT.ToString();
            LB_BName7.ID = "HAge" + gCT.ToString();
            LB_BName14.ID = "HBDate" + gCT.ToString();
            LB_BName8.ID = "HAttend" + gCT.ToString();
            LB_BName9.ID = "HOCPlace" + gCT.ToString();
            LB_BName10.ID = "xxx" + gCT.ToString();
            LB_BName11.ID = "HLDate" + gCT.ToString();
            LB_BName12.ID = "HRoomTime" + gCT.ToString();
            LB_BName13.ID = "HRemark" + gCT.ToString();
            //LB_BName14.ID = "xxxx" + gCT.ToString();

            LB_BName1.Text = gCT.ToString();
            LB_BName2.Text = QueryHMCBCA["HArea"].ToString();



            string gHTeamType = "";
            string gHTeamName = "";
            string[] gHTeamID = QueryHMCBCA["HTeamID"].ToString().Split(',');
            if (gHTeamID.Length > 1)
            {
                gHTeamType = gHTeamID[1] == "1" ? "HMTeam" : "HCTeam";
                gHTeamName = gHTeamID[1] == "1" ? "HMTeam" : "HCTeam";
                SqlDataReader QueryHTeam = SQLdatabase.ExecuteReader("select " + gHTeamName + " from " + gHTeamType + " where HID='" + gHTeamID[0] + "'");
                while (QueryHTeam.Read())
                {
                    LB_BName3.Text = QueryHTeam[gHTeamName].ToString();
                }
                QueryHTeam.Close();
            }
            else
            {
                LB_BName3.Text = "";
            }



            LB_BName4.Text = "&nbsp;" + QueryHMCBCA["HPeriod"].ToString();
            LB_BName5.Text = QueryHMCBCA["HUserName"].ToString();
            //1=男、2=女
            LB_BName6.Text = QueryHMCBCA["HSex"].ToString() == "1" ? "男" : "女"; ;
            LB_BName7.Text = "";
            LB_BName14.Text = QueryHMCBCA["HBDate"].ToString();
            //1 = 參班【一般】、2 = 參班【學青(25歲以下在學青年)】、3 = 參班【經濟困難(需經光團1號導師審核通過)】、4 = 參班【經濟困難(護持出家師父及全心奉獻各修行組織團體之清修人士(請光團1號導師協助確認)】、5 = 不參班(專業護持)、6 = 不參班(純護持)
            LB_BName8.Text = QueryHMCBCA["HAttend"].ToString() == "5" ? "專業護持者" : QueryHMCBCA["HAttend"].ToString() == "6" ? "純護持" : QueryHMCBCA["HAttend"].ToString() == "" || QueryHMCBCA["HAttend"].ToString() == null ? "未選擇" : QueryHMCBCA["HAttend"].ToString() == "0" ? "" : "參班";
            LB_BName9.Text = QueryHMCBCA["HPlaceName"].ToString();
            LB_BName10.Text = QueryHMCBCA["HRoomName"].ToString();
            LB_BName11.Text = QueryHMCBCA["HLDate"].ToString();
            LB_BName12.Text = QueryHMCBCA["HRoomTime"].ToString();
            //LB_BName13.Text = QueryHMCBCA["HRemark"].ToString();
            LB_BName13.Text = QueryHMCBCA["HRemark"].ToString() + "<br/>" + QueryHMCBCA["HRoomRemark"].ToString();
            //LB_BName14.Text = "";
            LB_BName15.Text = QueryHMCBCA["HSystemName"].ToString();


            LB_BName1.CssClass = "form-control staylist_cell text-center";
            LB_BName2.CssClass = "form-control staylist_cell text-left";
            LB_BName3.CssClass = "form-control staylist_cell text-left";
            LB_BName4.CssClass = "form-control staylist_cell text-center";
            LB_BName5.CssClass = "form-control staylist_cell text-left";
            LB_BName6.CssClass = "form-control staylist_cell text-center";
            LB_BName7.CssClass = "form-control staylist_cell text-center";
            LB_BName14.CssClass = "form-control staylist_cell text-center";
            LB_BName8.CssClass = "form-control staylist_cell text-left";
            LB_BName9.CssClass = "form-control staylist_cell text-left";
            LB_BName10.CssClass = "form-control staylist_cell text-left";
            LB_BName11.CssClass = "form-control staylist_cell text-left";
            LB_BName12.CssClass = "form-control staylist_cell text-left";
            LB_BName13.CssClass = "form-control staylist_cell text-left";
            LB_BName15.CssClass = "form-control staylist_cell text-center";
            //LB_BName14.CssClass = "form-control staylist_cell text-left";


            BCell1.BorderWidth = 1;
            BCell2.BorderWidth = 1;
            BCell3.BorderWidth = 1;
            BCell15.BorderWidth = 1;
            BCell4.BorderWidth = 1;
            BCell5.BorderWidth = 1;
            BCell6.BorderWidth = 1;
            BCell7.BorderWidth = 1;
            BCell14.BorderWidth = 1;
            BCell8.BorderWidth = 1;
            BCell9.BorderWidth = 1;
            BCell10.BorderWidth = 1;
            BCell11.BorderWidth = 1;
            BCell12.BorderWidth = 1;
            BCell13.BorderWidth = 1;
            //BCell14.BorderWidth = 1;

            BCell1.Controls.Add(LB_BName1);
            BCell2.Controls.Add(LB_BName2);
            BCell3.Controls.Add(LB_BName3);
            BCell15.Controls.Add(LB_BName15);
            BCell4.Controls.Add(LB_BName4);
            BCell5.Controls.Add(LB_BName5);
            BCell6.Controls.Add(LB_BName6);
            BCell7.Controls.Add(LB_BName7);
            BCell14.Controls.Add(LB_BName14);
            BCell8.Controls.Add(LB_BName8);
            BCell9.Controls.Add(LB_BName9);
            BCell10.Controls.Add(LB_BName10);
            BCell11.Controls.Add(LB_BName11);
            BCell12.Controls.Add(LB_BName12);
            BCell13.Controls.Add(LB_BName13);
            //BCell14.Controls.Add(LB_BName14);

            BRow.Cells.Add(BCell1);
            BRow.Cells.Add(BCell2);
            BRow.Cells.Add(BCell3);
            BRow.Cells.Add(BCell15);
            BRow.Cells.Add(BCell4);
            BRow.Cells.Add(BCell5);
            BRow.Cells.Add(BCell6);
            BRow.Cells.Add(BCell7);
            BRow.Cells.Add(BCell14);
            BRow.Cells.Add(BCell8);
            BRow.Cells.Add(BCell9);
            BRow.Cells.Add(BCell10);
            BRow.Cells.Add(BCell11);
            BRow.Cells.Add(BCell12);
            //BRow.Cells.Add(BCell14);






            //int gTPeople = 0;//
            //string gAPeople = "0";
            string gHLodging = "0";//顯示住宿與列表日是否同天，是=1、否=0
            string gHAStatus = "";


            TableCell[] BCell = new TableCell[gDateRangeA.Length];












            //找住宿日期
            //AE20230930_將HCourseBooking改成OrderList_Merge
            SqlDataReader QueryHRM = SQLdatabase.ExecuteReader("SELECT a.HAStatus, c.HLodging, c.HLDate, b.HUserName FROM HRollCall as a LEFT JOIN HMember as b on a.HMemberID = b.HID left join OrderList_Merge as c on a.HMemberID=c.HMemberID and a.HCourseID=c.HCourseID WHERE a.HCourseID = '" + QueryHMCBCA["HCourseID"].ToString() + "' and a.HmemberID='" + QueryHMCBCA["HID"].ToString() + "' and c.HStatus='1' AND c.HItemStatus='1'");

            if (QueryHRM.Read())
            {

                for (int i = 0; i < gDateRangeA.Length; i++)
                {
                    //Response.Write("gDateRangeA[i]01=" + gDateRangeA[i] + "<br/>");
                    //gDate：列表日期
                    //string gDate = Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd");


                    //gTPeople++;
                    //HAStatus，0=請選擇、1=實體、2=連線、3=遲到、4=請假
                    gHAStatus = QueryHRM["HAStatus"].ToString() == "1" ? "" : QueryHRM["HAStatus"].ToString() == "2" ? "" : QueryHRM["HAStatus"].ToString() == "3" ? "" : QueryHRM["HAStatus"].ToString() == "4" ? "請假" : "";

                    //住宿人數統計→暫用不到(預留功能)，可日後取消或刪除
                    if (QueryHRM["HLodging"].ToString() == "1")
                    {
                        if (QueryHRM["HLDate"].ToString().IndexOf("-") >= 0)
                        {
                            string[] gHLDate1 = QueryHRM["HLDate"].ToString().Split('-');//人員連續住宿日期

                            DateTime gHLSDate = Convert.ToDateTime(gHLDate1[0]);//人員住宿開始日
                            DateTime gHLEDate = Convert.ToDateTime(gHLDate1[1]);//人員住宿結束日
                            int gHLDate2 = (gHLEDate - gHLSDate).Days + 1;//人員連續住宿天數
                            string gHLDate = "";//人員住宿日期
                            for (int i1 = 0; i1 < gHLDate2; i1++)
                            {
                                //gHLDate：住宿日期
                                gHLDate = gHLSDate.AddDays(i1).ToString("yyyy/MM/dd");

                                if (gHLDate == gDateRangeA[i])//住宿日期=列表日期
                                {
                                    gHLodging = "1";
                                }

                            }

                        }
                        else if (QueryHRM["HLDate"].ToString().IndexOf(",") >= 0)
                        {
                            string[] gHLDate1 = QueryHRM["HLDate"].ToString().Split(',');//人員連續住宿日期
                            DateTime gHLSDate = Convert.ToDateTime(gHLDate1[0]);//人員住宿開始日
                            DateTime gHLEDate = Convert.ToDateTime(gHLDate1[gHLDate1.Length - 1]);//人員住宿結束日
                            int gHLDate2 = (gHLEDate - gHLSDate).Days + 1;//人員連續住宿天數
                            string gHLDate = "";//人員住宿日期
                            for (int i1 = 0; i1 < gHLDate2; i1++)
                            {
                                //gHLDate：住宿日期
                                gHLDate = gHLSDate.AddDays(i1).ToString("yyyy/MM/dd");

                                if (gHLDate == gDateRangeA[i])//住宿日期=列表日期
                                {
                                    gHLodging = "1";
                                }

                            }

                        }
                        else
                        {

                            if (QueryHRM["HLDate"].ToString() == gDateRangeA[i])//住宿日期=列表日期
                            {
                                gHLodging = "1";
                            }
                        }
                    }





                    for (int j = 0; j <= 0; j++)//j必定等於0
                    {
                        BCell[j] = new TableCell();

                        if (j == 0)
                        {
                            BCell[j].BorderWidth = 1;
                            BCell[j].HorizontalAlign = HorizontalAlign.Center;
                            BRow.Cells.Add(BCell[j]);//印出內容cell的格子
                                                     //gAll[gACT] = gHLodging == "1" ? Convert.ToString(Convert.ToInt32(gAll[gACT]) + 1) : Convert.ToString(Convert.ToInt32(gAll[gACT]) + 0);//暫用不到，待日後刪除
                            BCell[j].Text = gHLodging;//有住宿的話gHLodging=1，沒住宿gHLodging=0
                                                      //gACT++;
                        }

                    }
                    //gAPeople = "0";
                    gHLodging = "0";

                }

            }

            QueryHRM.Close();

























            BRow.Cells.Add(BCell13);

            TBL_StayList.Rows.Add(BRow);






            //********表格內容結束********



        }
        QueryHMCBCA.Close();










        //****************************************使用動態 table結束 * *******************************************
    }

    #endregion





    #region 分頁copy-1
    private int PageMax = 15;   //分頁最大顯示數量
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
        }
    }

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
        DDL_HOCPlace.Items.Add(new ListItem("-請選擇上課地點-", "0"));
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
    }

    protected void LBtn_Search_Click(object sender, EventArgs e)
    {

        string[] gDateRangeArray = TB_SearchDate.Text == "" ? "2000/01/01-3000/12/31".Split('-') : TB_SearchDate.Text.Split('-');
        string gHOCPlace = DDL_HOCPlace.SelectedValue == "0" ? "like '%'" : "='" + DDL_HOCPlace.SelectedValue + "'";//地點沒輸入則全找
                                                                                                                    //string gHOCPlace = DDL_HOCPlace.SelectedValue == "0" ? "='0'" : "='" + DDL_HOCPlace.SelectedValue + "'";//地點沒輸入則全不找


        SDS_HC.SelectCommand = "SELECT a.HID, a.HCourseName, a.HTeacherName, a.HDateRange, b.HPlaceName FROM HCourse as a join HPlace as b on a.HOCPlace=b.HID WHERE HCourseName like'%" + TB_Search.Text.Trim() + "%' and b.HID " + gHOCPlace + " and ((left(HDateRange,10)<='" + gDateRangeArray[0].Trim() + "' and right(HDateRange,10)>='" + gDateRangeArray[1].Trim() + "') or (left(HDateRange,10)<='" + gDateRangeArray[0].Trim() + "' and right(HDateRange,10)>='" + gDateRangeArray[0].Trim() + "' and right(HDateRange,10)<='" + gDateRangeArray[1].Trim() + "') or (left(HDateRange,10)>='" + gDateRangeArray[0].Trim() + "' and left(HDateRange,10)<='" + gDateRangeArray[1].Trim() + "'and right(HDateRange,10)>='" + gDateRangeArray[1].Trim() + "') or (left(HDateRange,10)>='" + gDateRangeArray[0].Trim() + "' and right(HDateRange,10)<='" + gDateRangeArray[1].Trim() + "')) and a.HStatus='1' ORDER BY a.HDateRange DESC";
        //SDS_HC.SelectCommand = "SELECT * FROM Course_BackendList WHERE HCourseName like'%" + TB_Search.Text + "%' and HStatus='1' ";

        #region 分頁copy-3搜尋用
        ViewState["Search"] = SDS_HC.SelectCommand;
        //呼叫分頁 (連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 搜尋開啟, DataList控件)
        Pg_Paging.PagingLoad("HochiSystemConnection", SDS_HC.SelectCommand, PageMax, LastPage, true, Rpt_HC);
        #endregion

    }

    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_Search.Text = "";
        TB_SearchDate.Text = "";
        DDL_HOCPlace.SelectedValue = "0";

        SDS_HC.SelectCommand = "SELECT a.HID, a.HCourseName, a.HTeacherName, a.HDateRange, b.HPlaceName FROM HCourse as a join HPlace as b on a.HOCPlace=b.HID where a.HStatus='1' ORDER BY a.HDateRange DESC";
        ViewState["Search"] = SDS_HC.SelectCommand;

        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HC.SelectCommand, PageMax, LastPage, true, Rpt_HC);
    }












    protected void Rpt_HC_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;
        ((Label)e.Item.FindControl("LB_HTeacherName")).Text = "";

        if (!string.IsNullOrEmpty(gDRV["HTeacherName"].ToString()))
        {
            string[] gHTeacherName = gDRV["HTeacherName"].ToString().Split(',');
            for (int i = 0; i < gHTeacherName.Length - 1; i++)
            {
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



    protected void Rpt_StayList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {


    }


    protected void LBtn_StayList_Click(object sender, EventArgs e)
    {
        Panel_CourseList.Visible = false;
        Panel_StayList.Visible = true;
        Panel_Search.Visible = false;


        LinkButton LBtn_StayList = sender as LinkButton;
        string StayList_CA = LBtn_StayList.CommandArgument;

        LB_HID.Text = StayList_CA;


        //AE20230908_先清空
        LB_HTeacherName.Text = null;


        SqlDataReader QueryHCourse = SQLdatabase.ExecuteReader("select a.HCourseName, a.HTeacherName, a.HDateRange from HCourse as a where a.HID='" + StayList_CA + "'");

        while (QueryHCourse.Read())
        {
            LB_HCourseName.Text = QueryHCourse["HCourseName"].ToString() + " 住宿登記表";


            string[] gHTeacherName = QueryHCourse["HTeacherName"].ToString().Split(',');
            for (int i = 0; i < gHTeacherName.Length - 1; i++)
            {
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


        ShowITable(StayList_CA);



    }




    protected void IBtn_ToExcel_Click(object sender, ImageClickEventArgs e)
    {
        Panel_CourseList.Visible = false;
        Panel_StayList.Visible = true;
        Panel_Search.Visible = false;



        SqlDataReader QueryHCourse = SQLdatabase.ExecuteReader("select a.HCourseName, a.HTeacherName, a.HDateRange from HCourse as a where a.HID='" + LB_HID.Text + "'");

        while (QueryHCourse.Read())
        {
            LB_HCourseName.Text = QueryHCourse["HCourseName"].ToString() + " 住宿登記表";


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

        ShowITable(LB_HID.Text);



        Response.Clear();
        //通知瀏覽器下載檔案
        Response.AddHeader("content-disposition", "attachment;filename=住宿登記表.xls");
        Response.ContentType = "application/application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        System.IO.StringWriter sw = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter hw = new HtmlTextWriter(sw);
        //TBL_StayList.RenderControl(hw);

        Panel1.RenderControl(hw);

        //Response.Write(sw.ToString());

        System.IO.StreamWriter swUTF8 = new System.IO.StreamWriter(Response.OutputStream, System.Text.Encoding.UTF8);//這樣一筆的時侯,就不會亂碼了
        swUTF8.Write(sw.ToString());
        swUTF8.Close();

        Response.End();
    }



    #region 回上一頁
    protected void LBtn_Back_Click(object sender, EventArgs e)
    {
        Response.Write("<script>window.location.href='HStayList.aspx';</script>");

    }
    #endregion
}