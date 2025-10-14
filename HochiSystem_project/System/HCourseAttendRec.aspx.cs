using NPOI.HPSF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class HCourseAttendRec : System.Web.UI.Page
{



    #region 動態table
    private void ShowITable(string gHCourseName, string gHDateRange, string gHOCPlace)
    {
        //Response.Write(DDL_OGNum.SelectedValue);
        //*****************************使用動態 table開始*******************************//

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS3", ("$('.date').datepicker({format: 'dd/mm/yyyy', todayHighlight: true,orientation: 'bottom auto',autoclose: true,startDate:new Date(),});"), true);     //執行日曆js


        //int TRow = gTRow;
        int gCT = 0;
        int gACT = 0;
        int gACTAll = 0;
        int gLCTAll = 0;

        //int gRCT = 0;
        int gAllCT = 0;//總筆數

        //string[] arr2 = new string[] { };
        string gSDate = "";
        string gEDate = "";

        //判斷天數相差幾天
        int gDateDiff = 0;
        //int gHDateRangeCT = 0;


        string[] gHDateRangeA = { };
        string gHDateRangeList = "";



        //AE20231002_將HCourseBooking改成OrderList_Merge
        //EE20231110-條件式加入參班身分為參班(HAttend='1')
        //AE20240924-條件式加入參班身分為參班兼護持(HAttend='6')
        SqlDataReader QueryHMCBCAct = SQLdatabase.ExecuteReader("select Count(a.HID) as CT, b.HDateRange from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID left join HArea as d on a.HAreaID=d.HID where b.HCourseName='" + gHCourseName + "' and b.HDateRange='" + gHDateRange + "'" + gHOCPlace + " and b.HStatus=1 and b.HItemStatus=1 and (b.HAttend='1' OR b.HAttend='6') group by b.HDateRange");


        //Response.Write("<br/><br/><br/><br/><br/>select Count(a.HID) as CT, b.HDateRange from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID left join HArea as d on a.HAreaID=d.HID where b.HCourseName='" + gHCourseName + "' and b.HDateRange='" + gHDateRange + "'" + gHOCPlace + " and b.HStatus=1 and b.HItemStatus=1 and (b.HAttend='1' OR b.HAttend='6') group by b.HDateRange");



        while (QueryHMCBCAct.Read())
        {

            if (QueryHMCBCAct["HDateRange"].ToString().IndexOf("-") >= 0)
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                string[] gDateArray = QueryHMCBCAct["HDateRange"].ToString().Split('-');
                string gDate = "";
                string gDate1 = "";
                gSDate = (gDateArray[0]);
                gEDate = (gDateArray[1]);
                gDateDiff = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                TableCell[] HCell = new TableCell[gHDateRangeA.Length];
                for (int i = 0; i < gDateDiff; i++)
                {
                    gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                    gDate1 += "'" + Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + "'" + ",";
                }

                gHDateRangeA = gDate.Trim(',').Split(',');
                gHDateRangeList = gDate1.Trim(',');
            }
            else if (QueryHMCBCAct["HDateRange"].ToString().IndexOf(",") >= 0)
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                gHDateRangeA = QueryHMCBCAct["HDateRange"].ToString().Trim(',').Split(',');
                string gDate1 = "";
                for (int i = 0; i <= gHDateRangeA.Length - 1; i++)
                {
                    gDate1 += "'" + gHDateRangeA[i] + "'" + ",";
                }
                gHDateRangeList = gDate1.Trim(',');
                gSDate = (gHDateRangeA[0]);
                gEDate = (gHDateRangeA[gHDateRangeA.Length - 1]);

                gDateDiff = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
            }
            else if (QueryHMCBCAct["HDateRange"].ToString().IndexOf("-") < 0 && QueryHMCBCAct["HDateRange"].ToString().IndexOf(",") < 0)//單一日期
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                string[] gDateArray = (QueryHMCBCAct["HDateRange"].ToString() + " - " + QueryHMCBCAct["HDateRange"].ToString()).Split('-');
                string gDate = "";
                string gDate1 = "";
                gSDate = (gDateArray[0]);
                gEDate = (gDateArray[1]);
                gDateDiff = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                TableCell[] HCell = new TableCell[gHDateRangeA.Length];
                for (int i = 0; i < gDateDiff; i++)
                {
                    gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                    gDate1 += "'" + Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + "'" + ",";

                }

                gHDateRangeA = gDate.Trim(',').Split(',');
                gHDateRangeList = gDate1.Trim(',');
            }
            else
            {

                Response.Write("<script>alert('日期錯誤!');window.location.href='HCourseAttendRec.aspx';</script>");
                return;
            }

        }


        QueryHMCBCAct.Close();

        string[] gAll = new string[gDateDiff * Convert.ToInt32(gAllCT) * 3];



        //AE20231002-將HCourseBooking改成OrderList_Merge
        //EE20231110-條件式加入參班身分為參班(HAttend='1')；加入體系
        //AE20240924_條件式加入參班身分為參班兼護持(HAttend='6')
        SqlDataReader QueryHMCBCA = SQLdatabase.ExecuteReader("select a.HAreaID, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, b.HRemark, b.HPlaceName, f.HSystemName from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID left join HArea as d on a.HAreaID=d.HID LEFT JOIN HLArea AS e ON d.HLAreaID =e.HID LEFT JOIN HSystem AS f ON a.HSystemID =f.HID LEFT JOIN HPlaceList AS g ON b.HPlaceName =g.HPlaceName  where b.HCourseName='" + gHCourseName + "' and b.HDateRange='" + gHDateRange + "'" + gHOCPlace + " and b.HStatus=1 and b.HItemStatus=1 and (b.HAttend='1' OR b.HAttend='6') order by g.strHAreaID ASC, e.HSort ASC, d.HSort ASC, f.HSystemName DESC");


        //Response.Write("<br/><br/>TTT<br/>select a.HAreaID, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, b.HRemark, b.HPlaceName, f.HSystemName from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID left join HArea as d on a.HAreaID=d.HID LEFT JOIN HLArea AS e ON d.HLAreaID =e.HID LEFT JOIN HSystem AS f ON a.HSystemID =f.HID LEFT JOIN HPlaceList AS g ON b.HPlaceName =g.HPlaceName  where b.HCourseName='" + gHCourseName + "' and b.HDateRange='" + gHDateRange + "'" + gHOCPlace + " and b.HStatus=1 and b.HItemStatus=1 and (b.HAttend='1' OR b.HAttend='6') order by g.strHAreaID ASC, e.HSort ASC, d.HSort ASC, f.HSystemName DESC");





        //********表格標題開始********
        TableRow HRow1 = new TableRow();
        TableCell HCell1 = new TableCell();
        TableCell HCell2 = new TableCell();
        TableCell HCell3 = new TableCell();
        TableCell HCell4 = new TableCell();
        TableCell HCell5 = new TableCell();
        TableCell HCell6 = new TableCell();


        //HCell1.Text = "編號";
        //HCell2.Text = "區屬";
        //HCell3.Text = "光團";
        //HCell4.Text = "期別";
        //HCell5.Text = "姓名";
        //HCell6.Text = "備註";

        HCell1.Text = "上課地點";
        HCell2.Text = "區屬";
        HCell3.Text = "光團";
        HCell4.Text = "期別";
        HCell5.Text = "姓名";
        HCell6.Text = "體系";

        HCell1.BorderWidth = 1;
        HCell2.BorderWidth = 1;
        HCell3.BorderWidth = 1;
        HCell4.BorderWidth = 1;
        HCell5.BorderWidth = 1;
        HCell6.BorderWidth = 1;


        HCell1.Width = Unit.Percentage(7);
        HCell2.Width = Unit.Percentage(5);
        HCell3.Width = Unit.Percentage(7);
        HCell4.Width = Unit.Percentage(4);
        HCell5.Width = Unit.Percentage(5);
        HCell6.Width = Unit.Percentage(4);

        HCell1.HorizontalAlign = HorizontalAlign.Center;
        HCell2.HorizontalAlign = HorizontalAlign.Center;
        HCell3.HorizontalAlign = HorizontalAlign.Center;
        HCell4.HorizontalAlign = HorizontalAlign.Center;
        HCell5.HorizontalAlign = HorizontalAlign.Center;
        HCell6.HorizontalAlign = HorizontalAlign.Center;

        HCell1.Font.Bold = true;
        HCell2.Font.Bold = true;
        HCell3.Font.Bold = true;
        HCell4.Font.Bold = true;
        HCell5.Font.Bold = true;
        HCell6.Font.Bold = true;



        int gAllDateCT = 0;

        if (gCT == 0)
        {
            HRow1.Cells.Add(HCell1);
            HRow1.Cells.Add(HCell2);
            HRow1.Cells.Add(HCell3);
            HRow1.Cells.Add(HCell4);
            HRow1.Cells.Add(HCell5);
            HRow1.Cells.Add(HCell6);

            TableCell[] HCell = new TableCell[gDateDiff];
            for (int i = 0; i < gHDateRangeA.Length; i++)
            {
                //string gDate = Convert.ToDateTime(gSDate).AddDays(i).ToString("MM/dd");
                HCell[i] = new TableCell();

                HCell[i].Text = Convert.ToDateTime(gHDateRangeA[i]).ToString("MM/dd");
                HCell[i].BorderWidth = 1;
                HCell[i].HorizontalAlign = HorizontalAlign.Center;
                HCell[i].Font.Bold = true;

                HRow1.Cells.Add(HCell[i]);


            }
            TBL_HCAttendRec.Rows.Add(HRow1);

        }
        //********表格標題結束********












        string gHPlaceNameList = "";
        string gHAreaList = "";
        string gHTeamIDList = "";
        string gHPeriodList = "";
        string gHUserNameList = "";
        string gHSystemNameList = "";
        string gHCourseIDList = "";
        string gHMemberIDList = "";

        string[] gHPlaceNameArray = { };
        string[] gHAreaArray = { };
        string[] gHTeamIDArray = { };
        string[] gHPeriodArray = { };
        string[] gHUserNameArray = { };
        string[] gHSystemNameArray = { };
        string[] gHCourseIDArray = { };
        string[] gHMemberIDArray = { };

        while (QueryHMCBCA.Read())
        {
            gHPlaceNameList += "'" + QueryHMCBCA["HPlaceName"].ToString() + "'" + "|";
            gHAreaList += "'" + QueryHMCBCA["HArea"].ToString() + "'" + "|";
            gHTeamIDList += "'" + QueryHMCBCA["HTeamID"].ToString() + "'" + "|";
            gHPeriodList += "'" + QueryHMCBCA["HPeriod"].ToString() + "'" + "|";
            gHUserNameList += "'" + QueryHMCBCA["HUserName"].ToString() + "'" + "|";
            gHSystemNameList += "'" + QueryHMCBCA["HSystemName"].ToString() + "'" + "|";
            gHCourseIDList += "'" + QueryHMCBCA["HCourseID"].ToString() + "'" + ",";
            gHMemberIDList += "'" + QueryHMCBCA["HID"].ToString() + "'" + ",";

            gHPlaceNameArray = gHPlaceNameList.Trim('|').Split('|');
            gHAreaArray = gHAreaList.Trim('|').Split('|');
            gHTeamIDArray = gHTeamIDList.Trim('|').Split('|');
            gHPeriodArray = gHPeriodList.Trim('|').Split('|');
            gHUserNameArray = gHUserNameList.Trim('|').Split('|');
            gHSystemNameArray = gHSystemNameList.Trim('|').Split('|');
            gHCourseIDArray = gHCourseIDList.Trim(',').Split(',');
            gHMemberIDArray = gHMemberIDList.Trim(',').Split(',');

        }
        QueryHMCBCA.Close();






        #region 移除多餘陣列
        List<string> resultList = new List<string>();
        foreach (string Data in gHCourseIDArray)
        {
            if (!resultList.Contains(Data))
            {
                resultList.Add(Data);
            }
        }
        string[] gHCourseIDArrayNew = resultList.ToArray();
        string gHCourseIDListNew = String.Join(",", gHCourseIDArrayNew);//還原為字串

        #endregion


        //AE20231002-將HCourseBooking改成OrderList_Merge
        //EE20231110-條件式加入參班身分為參班(HAttend='1')
        //AE20240924-條件式加入參班身分為參班兼護持(HAttend='6')
        SqlDataReader QueryHRM = SQLdatabase.ExecuteReader("SELECT FORMAT(a.HRCDate,'yyyy/MM/dd') AS HRCDate, a.HAStatus, c.HLodging, c.HLDate, a.HMemberID FROM HRollCall AS a LEFT JOIN HMember AS b ON a.HMemberID = b.HID LEFT JOIN OrderList_Merge AS c ON a.HMemberID=c.HMemberID AND a.HCourseID=c.HCourseID WHERE a.HCourseID IN (" + gHCourseIDListNew.Trim(',') + ") AND a.HRCDate IN (" + gHDateRangeList + ") AND a.HmemberID IN (" + gHMemberIDList.Trim(',') + ") AND c.HStatus=1  AND c.HItemStatus=1 AND (c.HAttend='1' OR c.HAttend='6')");

        //Response.Write("<br/>SELECT FORMAT(a.HRCDate,'yyyy/MM/dd') AS HRCDate, a.HAStatus, c.HLodging, c.HLDate, a.HMemberID FROM HRollCall AS a LEFT JOIN HMember AS b ON a.HMemberID = b.HID LEFT JOIN OrderList_Merge AS c ON a.HMemberID=c.HMemberID AND a.HCourseID=c.HCourseID WHERE a.HCourseID IN (" + gHCourseIDListNew.Trim(',') + ") AND a.HRCDate IN (" + gHDateRangeList + ") AND a.HmemberID IN (" + gHMemberIDList.Trim(',') + ") AND c.HStatus=1  AND c.HItemStatus=1 AND (c.HAttend='1' OR c.HAttend='6')");

        string gRollCallHRCDateList = "";
        string gRollCallHAStatusList = "";
        string gRollCallHMemberIDList = "";
        if (QueryHRM.HasRows)
        {
            while (QueryHRM.Read())
            {
                gRollCallHRCDateList += "'" + QueryHRM["HRCDate"].ToString() + "'" + ",";
                gRollCallHAStatusList += "'" + QueryHRM["HAStatus"].ToString() + "'" + ",";
                gRollCallHMemberIDList += "'" + QueryHRM["HMemberID"].ToString() + "'" + ",";
            }
        }
        QueryHRM.Close();

        //Response.End();




        for (int k = 0; k < gHUserNameArray.Length; k++)
        {
            TableRow BRow = new TableRow();

            //********表格前段內容開始********

            gCT++;

            TableCell BCell1 = new TableCell();//上課地點
            TableCell BCell2 = new TableCell();//區屬
            TableCell BCell3 = new TableCell();//光團
            TableCell BCell4 = new TableCell();//期別
            TableCell BCell5 = new TableCell();//姓名
            TableCell BCell6 = new TableCell();//體系

            Label LB_QName1 = new Label();
            Label LB_QName2 = new Label();
            Label LB_QName3 = new Label();
            Label LB_QName4 = new Label();
            Label LB_QName5 = new Label();
            Label LB_QName6 = new Label();


            LB_QName1.ID = "HPlaceName" + gCT.ToString();
            LB_QName2.ID = "HArea" + gCT.ToString();
            LB_QName3.ID = "HTeamID" + gCT.ToString();
            LB_QName4.ID = "HPeriod" + gCT.ToString();
            LB_QName5.ID = "HUserName" + gCT.ToString();
            LB_QName6.ID = "HSystemName" + gCT.ToString();




            LB_QName1.Text = gHPlaceNameArray[k].ToString().Trim('\'');
            LB_QName2.Text = gHAreaArray[k].ToString().Trim('\'');

            string gHTeamType = "";
            string gHTeamName = "";
            string[] gHTeamID = gHTeamIDArray[k].ToString().Split(',');
            if (gHTeamID.Length > 1)
            {
                gHTeamType = gHTeamID[1].Trim('\'') == "1" ? "HMTeam" : "HCTeam";
                gHTeamName = gHTeamID[1].Trim('\'') == "1" ? "HMTeam" : "HCTeam";
                SqlDataReader QueryHTeam = SQLdatabase.ExecuteReader("select " + gHTeamName + " from " + gHTeamType + " where HID='" + gHTeamID[0].Trim('\'') + "'");
                while (QueryHTeam.Read())
                {
                    LB_QName3.Text = QueryHTeam[gHTeamName].ToString();
                }
                QueryHTeam.Close();
            }
            else
            {
                LB_QName3.Text = "";
            }


            LB_QName4.Text = gHPeriodArray[k].ToString().Trim('\'');
            LB_QName5.Text = gHUserNameArray[k].ToString().Trim('\'');
            LB_QName6.Text = gHSystemNameArray[k].ToString().Trim('\'');

            BCell1.BorderWidth = 1;
            BCell2.BorderWidth = 1;
            BCell3.BorderWidth = 1;
            BCell4.BorderWidth = 1;
            BCell5.BorderWidth = 1;
            BCell6.BorderWidth = 1;

            BCell1.HorizontalAlign = HorizontalAlign.Center;
            BCell2.HorizontalAlign = HorizontalAlign.Center;
            BCell4.HorizontalAlign = HorizontalAlign.Center;
            BCell5.HorizontalAlign = HorizontalAlign.Center;
            BCell6.HorizontalAlign = HorizontalAlign.Center;

            BCell1.Controls.Add(LB_QName1);
            BCell2.Controls.Add(LB_QName2);
            BCell3.Controls.Add(LB_QName3);
            BCell4.Controls.Add(LB_QName4);
            BCell5.Controls.Add(LB_QName5);
            BCell6.Controls.Add(LB_QName6);


            BRow.Cells.Add(BCell1);
            BRow.Cells.Add(BCell2);
            BRow.Cells.Add(BCell3);
            BRow.Cells.Add(BCell4);
            BRow.Cells.Add(BCell5);
            BRow.Cells.Add(BCell6);

            //********表格前段內容結束********





            //********表格後段內容開始********

            TableCell BCell7 = new TableCell();//日期序列


            #region 待處理

            string[] gHDateRangeArray = gHDateRangeList.Trim(',').Split(',');
            string[] gRollCallHRCDateArray = gRollCallHRCDateList.Trim(',').Split(',');
            string[] gRollCallHAStatusArray = gRollCallHAStatusList.Trim(',').Split(',');
            string[] gRollCallHMemberIDArray = gRollCallHMemberIDList.Trim(',').Split(',');

            int gAPeopleCT = 0;//出席次數

            string gHRCDateList = "";
            string gHAStatusList = "";
            string[] gHRCDateArray = { };
            string[] gHAStatusArray = { };

            for (int n = 0; n < gRollCallHMemberIDArray.Length; n++)
            {
                if (gRollCallHMemberIDArray[n] == gHMemberIDArray[k])
                {
                    gHRCDateList += "" + gRollCallHRCDateArray[n] + "" + "|";
                    gHAStatusList += "" + gRollCallHAStatusArray[n] + "" + "|";
                }
            }




            gHRCDateArray = gHRCDateList.Trim('|').Split('|');
            gHAStatusArray = gHAStatusList.Trim('|').Split('|');

            for (int x = 0; x < gHDateRangeArray.Length; x++)//列表日期(完整月份)
            {
                if (gHRCDateArray[0].ToString().Trim() != "")
                {
                    for (int y = 0; y < gHRCDateArray.Length; y++)//課程日期
                    {
                        //Response.Write("gHDateRangeArray=" + gHDateRangeArray[x] + "<br/>");
                        //Response.Write("gHRCDateArray=" + gHRCDateArray[x] + "<br/>");
                        string gHAStatus = "";//原因
                        string gAttend = "";//是否有出席(如果沒出席則顯示-)
                        if (gHDateRangeArray[x].ToString() == gHRCDateArray[y].ToString())
                        {
                            //原因S
                            //HAStatus，0=請選擇、1=實體、2=連線、3=遲到、4=請假
                            gHAStatus = gHAStatusArray[y].Trim('\'').ToString() == "1" ? "" : gHAStatusArray[y].Trim('\'').ToString() == "2" ? "" : gHAStatusArray[y].Trim('\'').ToString() == "3" ? "" : gHAStatusArray[y].Trim('\'').ToString() == "4" ? "請假" : "";
                            //原因E

                            //出席次數S
                            if (gHAStatusArray[y].Trim('\'').ToString() == "1" || gHAStatusArray[y].Trim('\'').ToString() == "2" || gHAStatusArray[y].Trim('\'').ToString() == "3")
                            {
                                gAPeopleCT = gAPeopleCT + 1;
                            }
                            else
                            {
                                gAttend = "-";
                            }
                            //出席次數E


                            BCell7 = new TableCell();
                            BCell7.BorderWidth = 1;
                            BCell7.HorizontalAlign = HorizontalAlign.Center;
                            BRow.Cells.Add(BCell7);
                            gAll[gACT] = gAPeopleCT == 1 ? Convert.ToString(Convert.ToInt32(gAll[gACT]) + 1) : Convert.ToString(Convert.ToInt32(gAll[gACT]) + 0);

                            if (gHAStatus == "" && gAttend == "")
                            {
                                BCell7.Text = gAPeopleCT.ToString();
                            }
                            else if (gHAStatus == "" && gAttend != "")
                            {
                                BCell7.Text = gAttend;
                            }
                            else if (gHAStatus != "" && gAttend == "")
                            {
                                BCell7.Text = gHAStatus;
                            }
                            else if (gHAStatus != "" && gAttend != "")
                            {
                                BCell7.Text = gHAStatus;
                            }
                            else
                            {
                                BCell7.Text = "";
                            }



                            gACT++;

                        }

                    }

                }
                else
                {
                    BCell7 = new TableCell();
                    BCell7.BorderWidth = 1;
                    BCell7.HorizontalAlign = HorizontalAlign.Center;
                    BRow.Cells.Add(BCell7);
                    BCell7.Text = "";
                }
            }




            //********表格後段內容結束********
            #endregion



            TBL_HCAttendRec.Rows.Add(BRow);






        }















        //****************************************使用動態 table結束 * *******************************************
    }

    #endregion


    #region 動態table
    //AA20250619_可單天報名(JOIN HCourseBooking_DateAttend)
    private void ShowITableBBDate(string gHCourseName, string gHDateRange, string gHOCPlace)
    {
        //Response.Write(DDL_OGNum.SelectedValue);
        //*****************************使用動態 table開始*******************************//

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS3", ("$('.date').datepicker({format: 'dd/mm/yyyy', todayHighlight: true,orientation: 'bottom auto',autoclose: true,startDate:new Date(),});"), true);     //執行日曆js



        //int TRow = gTRow;
        int gCT = 0;
        int gACT = 0;
        int gACTAll = 0;
        int gLCTAll = 0;

        //int gRCT = 0;
        int gAllCT = 0;//總筆數

        //string[] arr2 = new string[] { };
        string gSDate = "";
        string gEDate = "";

        //判斷天數相差幾天
        int gDateDiff = 0;
        //int gHDateRangeCT = 0;


        string[] gHDateRangeA = { };
        string gHDateRangeList = "";



        //AE20231002_將HCourseBooking改成OrderList_Merge
        //EE20231110-條件式加入參班身分為參班(HAttend='1')
        //AE20240924-條件式加入參班身分為參班兼護持(HAttend='6')
        SqlDataReader QueryHMCBCAct = SQLdatabase.ExecuteReader("SELECT Count(a.HID) as CT, b.HDateRange from HMember as a INNER JOIN HCourseBooking AS b ON a.HID = b.HMemberID INNER JOIN HCourseBooking_DateAttend AS e ON b.HID = e.HCourseBookingID LEFT JOIN HArea as d on a.HAreaID = d.HID LEFT JOIN Course_BackendList AS g ON b.HCourseID = g.HID WHERE b.HCourseName='" + gHCourseName + "' AND b.HDateRange='" + gHDateRange + "'" + gHOCPlace + " and b.HStatus=1 AND b.HItemStatus=1 AND (e.HAttend='1' OR e.HAttend='6') GROUP BY b.HDateRange");


        //Response.Write("<br/><br/><br/><br/><br/>SELECT Count(a.HID) as CT, b.HDateRange from HMember as a INNER JOIN HCourseBooking AS b ON a.HID = b.HMemberID INNER JOIN HCourseBooking_DateAttend AS e ON b.HID = e.HCourseBookingID LEFT JOIN HArea as d on a.HAreaID = d.HID LEFT JOIN Course_BackendList AS g ON b.HCourseID = g.HID WHERE b.HCourseName='" + gHCourseName + "' AND b.HDateRange='" + gHDateRange + "'" + gHOCPlace + " and b.HStatus=1 AND b.HItemStatus=1 AND (e.HAttend='1' OR e.HAttend='6') GROUP BY b.HDateRange");

        //Response.End();

        while (QueryHMCBCAct.Read())
        {

            if (QueryHMCBCAct["HDateRange"].ToString().IndexOf("-") >= 0)
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                string[] gDateArray = QueryHMCBCAct["HDateRange"].ToString().Split('-');
                string gDate = "";
                string gDate1 = "";
                gSDate = (gDateArray[0]);
                gEDate = (gDateArray[1]);
                gDateDiff = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                TableCell[] HCell = new TableCell[gHDateRangeA.Length];
                for (int i = 0; i < gDateDiff; i++)
                {
                    gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                    gDate1 += "'" + Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + "'" + ",";
                }

                gHDateRangeA = gDate.Trim(',').Split(',');
                gHDateRangeList = gDate1.Trim(',');
            }
            else if (QueryHMCBCAct["HDateRange"].ToString().IndexOf(",") >= 0)
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                gHDateRangeA = QueryHMCBCAct["HDateRange"].ToString().Trim(',').Split(',');
                string gDate1 = "";
                for (int i = 0; i <= gHDateRangeA.Length - 1; i++)
                {
                    gDate1 += "'" + gHDateRangeA[i] + "'" + ",";
                }
                gHDateRangeList = gDate1.Trim(',');
                gSDate = (gHDateRangeA[0]);
                gEDate = (gHDateRangeA[gHDateRangeA.Length - 1]);

                gDateDiff = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
            }
            else if (QueryHMCBCAct["HDateRange"].ToString().IndexOf("-") < 0 && QueryHMCBCAct["HDateRange"].ToString().IndexOf(",") < 0)//單一日期
            {
                gAllCT = Convert.ToInt32(QueryHMCBCAct["CT"].ToString());
                string[] gDateArray = (QueryHMCBCAct["HDateRange"].ToString() + " - " + QueryHMCBCAct["HDateRange"].ToString()).Split('-');
                string gDate = "";
                string gDate1 = "";
                gSDate = (gDateArray[0]);
                gEDate = (gDateArray[1]);
                gDateDiff = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                TableCell[] HCell = new TableCell[gHDateRangeA.Length];
                for (int i = 0; i < gDateDiff; i++)
                {
                    gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                    gDate1 += "'" + Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + "'" + ",";

                }

                gHDateRangeA = gDate.Trim(',').Split(',');
                gHDateRangeList = gDate1.Trim(',');
            }
            else
            {

                Response.Write("<script>alert('日期錯誤!');window.location.href='HCourseAttendRec.aspx';</script>");
                return;
            }

        }


        QueryHMCBCAct.Close();

        string[] gAll = new string[gDateDiff * Convert.ToInt32(gAllCT) * 3];

        //Response.Write("<br/><br/>select a.HAreaID, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, STRING_AGG(FORMAT(c.HDate,'yyyy/MM/dd'), ',') AS HDate, b.HRemark, g.HPlaceName, f.HSystemName from HMember as a LEFT JOIN HCourseBooking AS b ON a.HID = b.HMemberID INNER JOIN HCourseBooking_DateAttend AS c ON b.HID = c.HCourseBookingID left join HArea as d on a.HAreaID=d.HID LEFT JOIN HLArea AS e ON d.HLAreaID =e.HID LEFT JOIN HSystem AS f ON a.HSystemID =f.HID LEFT JOIN Course_BackendList AS g ON b.HCourseID = g.HID where b.HCourseName='" + gHCourseName + "' and b.HDateRange='" + gHDateRange + "'" + gHOCPlace + " and b.HStatus=1 and b.HItemStatus=1 and (c.HAttend='1' OR c.HAttend='6') GROUP BY a.HAreaID, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, b.HRemark, g.HPlaceName, f.HSystemName,e.HSort, d.HSort,f.HSystemName order by  e.HSort ASC, d.HSort ASC, f.HSystemName DESC" + "<br/><br/>");


        //Response.End();

        //AE20231002-將HCourseBooking改成OrderList_Merge
        //EE20231110-條件式加入參班身分為參班(HAttend='1')；加入體系
        //AE20240924_條件式加入參班身分為參班兼護持(HAttend='6')
        //AE20250623_改JOIN HCourseBooking & HCourseBooking_DateAttend
        //SqlDataReader QueryHMCBCA = SQLdatabase.ExecuteReader("select a.HAreaID, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, b.HRemark, b.HPlaceName, f.HSystemName from HMember as a  LEFT JOIN HCourseBooking AS b ON a.HID = b.HMemberID INNER JOIN HCourseBooking_DateAttend AS e ON b.HID = e.HCourseBookingID left join HArea as d on a.HAreaID=d.HID LEFT JOIN HLArea AS e ON d.HLAreaID =e.HID LEFT JOIN HSystem AS f ON a.HSystemID =f.HID LEFT JOIN HPlaceList AS g ON b.HPlaceName =g.HPlaceName  where b.HCourseName='" + gHCourseName + "' and b.HDateRange='" + gHDateRange + "'" + gHOCPlace + " and b.HStatus=1 and b.HItemStatus=1 and (e.HAttend='1' OR e.HAttend='6') order by g.strHAreaID ASC, e.HSort ASC, d.HSort ASC, f.HSystemName DESC");


        SqlDataReader QueryHMCBCA = SQLdatabase.ExecuteReader("select a.HAreaID, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, STRING_AGG(FORMAT(c.HDate,'yyyy/MM/dd'), ',') AS HDate, b.HRemark, g.HPlaceName, f.HSystemName from HMember as a LEFT JOIN HCourseBooking AS b ON a.HID = b.HMemberID INNER JOIN HCourseBooking_DateAttend AS c ON b.HID = c.HCourseBookingID left join HArea as d on a.HAreaID=d.HID LEFT JOIN HLArea AS e ON d.HLAreaID =e.HID LEFT JOIN HSystem AS f ON a.HSystemID =f.HID LEFT JOIN Course_BackendList AS g ON b.HCourseID = g.HID where b.HCourseName='" + gHCourseName + "' and b.HDateRange='" + gHDateRange + "'" + gHOCPlace + " and b.HStatus=1 and b.HItemStatus=1 and (c.HAttend='1' OR c.HAttend='6') GROUP BY a.HAreaID, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, b.HRemark, g.HPlaceName, f.HSystemName,e.HSort, d.HSort,f.HSystemName order by  e.HSort ASC, d.HSort ASC, f.HSystemName DESC");





        //********表格標題開始********
        TableRow HRow1 = new TableRow();
        TableCell HCell1 = new TableCell();
        TableCell HCell2 = new TableCell();
        TableCell HCell3 = new TableCell();
        TableCell HCell4 = new TableCell();
        TableCell HCell5 = new TableCell();
        TableCell HCell6 = new TableCell();


        //HCell1.Text = "編號";
        //HCell2.Text = "區屬";
        //HCell3.Text = "光團";
        //HCell4.Text = "期別";
        //HCell5.Text = "姓名";
        //HCell6.Text = "備註";

        HCell1.Text = "上課地點";
        HCell2.Text = "區屬";
        HCell3.Text = "光團";
        HCell4.Text = "期別";
        HCell5.Text = "姓名";
        HCell6.Text = "體系";

        HCell1.BorderWidth = 1;
        HCell2.BorderWidth = 1;
        HCell3.BorderWidth = 1;
        HCell4.BorderWidth = 1;
        HCell5.BorderWidth = 1;
        HCell6.BorderWidth = 1;


        HCell1.Width = Unit.Percentage(7);
        HCell2.Width = Unit.Percentage(5);
        HCell3.Width = Unit.Percentage(7);
        HCell4.Width = Unit.Percentage(4);
        HCell5.Width = Unit.Percentage(5);
        HCell6.Width = Unit.Percentage(4);

        HCell1.HorizontalAlign = HorizontalAlign.Center;
        HCell2.HorizontalAlign = HorizontalAlign.Center;
        HCell3.HorizontalAlign = HorizontalAlign.Center;
        HCell4.HorizontalAlign = HorizontalAlign.Center;
        HCell5.HorizontalAlign = HorizontalAlign.Center;
        HCell6.HorizontalAlign = HorizontalAlign.Center;

        HCell1.Font.Bold = true;
        HCell2.Font.Bold = true;
        HCell3.Font.Bold = true;
        HCell4.Font.Bold = true;
        HCell5.Font.Bold = true;
        HCell6.Font.Bold = true;



        int gAllDateCT = 0;

        if (gCT == 0)
        {
            HRow1.Cells.Add(HCell1);
            HRow1.Cells.Add(HCell2);
            HRow1.Cells.Add(HCell3);
            HRow1.Cells.Add(HCell4);
            HRow1.Cells.Add(HCell5);
            HRow1.Cells.Add(HCell6);

            TableCell[] HCell = new TableCell[gDateDiff];
            for (int i = 0; i < gHDateRangeA.Length; i++)
            {
                //string gDate = Convert.ToDateTime(gSDate).AddDays(i).ToString("MM/dd");
                HCell[i] = new TableCell();

                HCell[i].Text = Convert.ToDateTime(gHDateRangeA[i]).ToString("MM/dd");
                HCell[i].BorderWidth = 1;
                HCell[i].HorizontalAlign = HorizontalAlign.Center;
                HCell[i].Font.Bold = true;

                HRow1.Cells.Add(HCell[i]);


            }
            TBL_HCAttendRec.Rows.Add(HRow1);

        }
        //********表格標題結束********










        string gHPlaceNameList = "";
        string gHAreaList = "";
        string gHTeamIDList = "";
        string gHPeriodList = "";
        string gHUserNameList = "";
        string gHSystemNameList = "";
        string gHCourseIDList = "";
        string gHMemberIDList = "";



        string[] gHPlaceNameArray = { };
        string[] gHAreaArray = { };
        string[] gHTeamIDArray = { };
        string[] gHPeriodArray = { };
        string[] gHUserNameArray = { };
        string[] gHSystemNameArray = { };
        string[] gHCourseIDArray = { };
        string[] gHMemberIDArray = { };

        while (QueryHMCBCA.Read())
        {
            gHPlaceNameList += "'" + QueryHMCBCA["HPlaceName"].ToString() + "'" + "|";
            gHAreaList += "'" + QueryHMCBCA["HArea"].ToString() + "'" + "|";
            gHTeamIDList += "'" + QueryHMCBCA["HTeamID"].ToString() + "'" + "|";
            gHPeriodList += "'" + QueryHMCBCA["HPeriod"].ToString() + "'" + "|";
            gHUserNameList += "'" + QueryHMCBCA["HUserName"].ToString() + "'" + "|";
            gHSystemNameList += "'" + QueryHMCBCA["HSystemName"].ToString() + "'" + "|";
            gHCourseIDList += "'" + QueryHMCBCA["HCourseID"].ToString() + "'" + ",";
            gHMemberIDList += "'" + QueryHMCBCA["HID"].ToString() + "'" + ",";

        }
        QueryHMCBCA.Close();



        gHPlaceNameArray = gHPlaceNameList.Trim('|').Split('|');
        gHAreaArray = gHAreaList.Trim('|').Split('|');
        gHTeamIDArray = gHTeamIDList.Trim('|').Split('|');
        gHPeriodArray = gHPeriodList.Trim('|').Split('|');
        gHUserNameArray = gHUserNameList.Trim('|').Split('|');
        gHSystemNameArray = gHSystemNameList.Trim('|').Split('|');
        gHCourseIDArray = gHCourseIDList.Trim(',').Split(',');
        gHMemberIDArray = gHMemberIDList.Trim(',').Split(',');


        #region 移除多餘陣列
        List<string> resultList = new List<string>();
        foreach (string Data in gHCourseIDArray)
        {
            if (!resultList.Contains(Data))
            {
                resultList.Add(Data);
            }
        }
        string[] gHCourseIDArrayNew = resultList.ToArray();
        string gHCourseIDListNew = String.Join(",", gHCourseIDArrayNew);//還原為字串

        #endregion


        //AE20231002-將HCourseBooking改成OrderList_Merge
        //EE20231110-條件式加入參班身分為參班(HAttend='1')
        //AE20240924-條件式加入參班身分為參班兼護持(HAttend='6')

        //Response.Write("<br/>SELECT FORMAT(a.HRCDate,'yyyy/MM/dd') AS HRCDate, a.HAStatus, a.HMemberID FROM HRollCall AS a LEFT JOIN HMember AS b ON a.HMemberID = b.HID LEFT JOIN HCourseBooking AS c ON a.HMemberID=c.HMemberID AND a.HCourseID=c.HCourseID LEFT JOIN HCourseBooking_DateAttend AS d ON c.HID=d.HCourseBookingID WHERE a.HCourseID IN (" + gHCourseIDListNew.Trim(',') + ") AND a.HRCDate IN (" + gHDateRangeList + ") AND a.HmemberID IN (" + gHMemberIDList.Trim(',') + ") AND c.HStatus=1  AND c.HItemStatus=1 AND (d.HAttend='1' OR d.HAttend='6') GROUP BY c.HCourseID, a.HRCDate, a.HAStatus, a.HMemberID");



        SqlDataReader QueryHRM = SQLdatabase.ExecuteReader("SELECT FORMAT(a.HRCDate,'yyyy/MM/dd') AS HRCDate, a.HAStatus, a.HMemberID FROM HRollCall AS a LEFT JOIN HMember AS b ON a.HMemberID = b.HID LEFT JOIN HCourseBooking AS c ON a.HMemberID=c.HMemberID AND a.HCourseID=c.HCourseID LEFT JOIN HCourseBooking_DateAttend AS d ON c.HID=d.HCourseBookingID WHERE a.HCourseID IN (" + gHCourseIDListNew.Trim(',') + ") AND a.HRCDate IN (" + gHDateRangeList + ") AND a.HmemberID IN (" + gHMemberIDList.Trim(',') + ") AND c.HStatus=1  AND c.HItemStatus=1 AND (d.HAttend='1' OR d.HAttend='6') GROUP BY c.HCourseID, a.HRCDate, a.HAStatus, a.HMemberID ");


        string gRollCallHRCDateList = "";
        string gRollCallHAStatusList = "";
        string gRollCallHMemberIDList = "";



        if (QueryHRM.HasRows)
        {
            while (QueryHRM.Read())
            {
                gRollCallHRCDateList += "'" + QueryHRM["HRCDate"].ToString() + "'" + ",";
                gRollCallHAStatusList += "'" + QueryHRM["HAStatus"].ToString() + "'" + ",";
                gRollCallHMemberIDList += "'" + QueryHRM["HMemberID"].ToString() + "'" + ",";
            }
        }
        QueryHRM.Close();


        //Response.Write("<br/>gRollCallHRCDateList="+ gRollCallHRCDateList+ "<br/><br/>");


        //Response.End();




        for (int k = 0; k < gHUserNameArray.Length; k++)
        {
            TableRow BRow = new TableRow();

            //********表格前段內容開始********

            gCT++;

            TableCell BCell1 = new TableCell();//上課地點
            TableCell BCell2 = new TableCell();//區屬
            TableCell BCell3 = new TableCell();//光團
            TableCell BCell4 = new TableCell();//期別
            TableCell BCell5 = new TableCell();//姓名
            TableCell BCell6 = new TableCell();//體系

            Label LB_QName1 = new Label();
            Label LB_QName2 = new Label();
            Label LB_QName3 = new Label();
            Label LB_QName4 = new Label();
            Label LB_QName5 = new Label();
            Label LB_QName6 = new Label();


            LB_QName1.ID = "HPlaceName" + gCT.ToString();
            LB_QName2.ID = "HArea" + gCT.ToString();
            LB_QName3.ID = "HTeamID" + gCT.ToString();
            LB_QName4.ID = "HPeriod" + gCT.ToString();
            LB_QName5.ID = "HUserName" + gCT.ToString();
            LB_QName6.ID = "HSystemName" + gCT.ToString();




            LB_QName1.Text = gHPlaceNameArray[k].ToString().Trim('\'');
            LB_QName2.Text = gHAreaArray[k].ToString().Trim('\'');

            string gHTeamType = "";
            string gHTeamName = "";
            string[] gHTeamID = gHTeamIDArray[k].ToString().Split(',');
            if (gHTeamID.Length > 1)
            {
                gHTeamType = gHTeamID[1].Trim('\'') == "1" ? "HMTeam" : "HCTeam";
                gHTeamName = gHTeamID[1].Trim('\'') == "1" ? "HMTeam" : "HCTeam";
                SqlDataReader QueryHTeam = SQLdatabase.ExecuteReader("select " + gHTeamName + " from " + gHTeamType + " where HID='" + gHTeamID[0].Trim('\'') + "'");
                while (QueryHTeam.Read())
                {
                    LB_QName3.Text = QueryHTeam[gHTeamName].ToString();
                }
                QueryHTeam.Close();
            }
            else
            {
                LB_QName3.Text = "";
            }


            LB_QName4.Text = gHPeriodArray[k].ToString().Trim('\'');
            LB_QName5.Text = gHUserNameArray[k].ToString().Trim('\'');
            LB_QName6.Text = gHSystemNameArray[k].ToString().Trim('\'');

            BCell1.BorderWidth = 1;
            BCell2.BorderWidth = 1;
            BCell3.BorderWidth = 1;
            BCell4.BorderWidth = 1;
            BCell5.BorderWidth = 1;
            BCell6.BorderWidth = 1;

            BCell1.HorizontalAlign = HorizontalAlign.Center;
            BCell2.HorizontalAlign = HorizontalAlign.Center;
            BCell4.HorizontalAlign = HorizontalAlign.Center;
            BCell5.HorizontalAlign = HorizontalAlign.Center;
            BCell6.HorizontalAlign = HorizontalAlign.Center;

            BCell1.Controls.Add(LB_QName1);
            BCell2.Controls.Add(LB_QName2);
            BCell3.Controls.Add(LB_QName3);
            BCell4.Controls.Add(LB_QName4);
            BCell5.Controls.Add(LB_QName5);
            BCell6.Controls.Add(LB_QName6);


            BRow.Cells.Add(BCell1);
            BRow.Cells.Add(BCell2);
            BRow.Cells.Add(BCell3);
            BRow.Cells.Add(BCell4);
            BRow.Cells.Add(BCell5);
            BRow.Cells.Add(BCell6);

            //********表格前段內容結束********





            //********表格後段內容開始********

            TableCell BCell7 = new TableCell();//日期序列


            #region 待處理

            string[] gHDateRangeArray = gHDateRangeList.Trim(',').Split(',');
            string[] gRollCallHRCDateArray = gRollCallHRCDateList.Trim(',').Split(',');
            string[] gRollCallHAStatusArray = gRollCallHAStatusList.Trim(',').Split(',');
            string[] gRollCallHMemberIDArray = gRollCallHMemberIDList.Trim(',').Split(',');

            int gAPeopleCT = 0;//出席次數

            string gHRCDateList = "";
            string gHAStatusList = "";
            string[] gHRCDateArray = { };
            string[] gHAStatusArray = { };

            for (int n = 0; n < gRollCallHMemberIDArray.Length; n++)
            {
                if (gRollCallHMemberIDArray[n] == gHMemberIDArray[k])
                {
                    gHRCDateList += "" + gRollCallHRCDateArray[n] + "" + "|";
                    gHAStatusList += "" + gRollCallHAStatusArray[n] + "" + "|";
                }
            }




            gHRCDateArray = gHRCDateList.Trim('|').Split('|');
            gHAStatusArray = gHAStatusList.Trim('|').Split('|');

            for (int x = 0; x < gHDateRangeArray.Length; x++)//列表日期(完整月份)
            {
                //AE20250710_變更位置
                BCell7 = new TableCell();
                BCell7.BorderWidth = 1;
                BCell7.HorizontalAlign = HorizontalAlign.Center;
                BRow.Cells.Add(BCell7);


                if (gHRCDateArray[0].ToString().Trim() != "") //點名日期
                {
                    for (int y = 0; y < gHRCDateArray.Length; y++)//點名日期
                    {
                        string gHAStatus = "";//原因
                        string gAttend = "";//是否有出席(如果沒出席則顯示-)

                        if (gHDateRangeArray[x].ToString() == gHRCDateArray[y].ToString())
                        {
                            //原因S
                            //HAStatus，0=請選擇、1=實體、2=連線、3=遲到、4=請假
                            gHAStatus = gHAStatusArray[y].Trim('\'').ToString() == "1" ? "" : gHAStatusArray[y].Trim('\'').ToString() == "2" ? "" : gHAStatusArray[y].Trim('\'').ToString() == "3" ? "" : gHAStatusArray[y].Trim('\'').ToString() == "4" ? "請假" : "";
                            //原因E

                            //出席次數S
                            if (gHAStatusArray[y].Trim('\'').ToString() == "1" || gHAStatusArray[y].Trim('\'').ToString() == "2" || gHAStatusArray[y].Trim('\'').ToString() == "3")
                            {
                                gAPeopleCT = gAPeopleCT + 1;
                            }
                            else
                            {
                                gAttend = "未點名";
                            }
                            //出席次數E

                            //原本位置
                            //BCell7 = new TableCell();
                            //BCell7.BorderWidth = 1;
                            //BCell7.HorizontalAlign = HorizontalAlign.Center;
                            //BRow.Cells.Add(BCell7);

                            gAll[gACT] = gAPeopleCT == 1 ? Convert.ToString(Convert.ToInt32(gAll[gACT]) + 1) : Convert.ToString(Convert.ToInt32(gAll[gACT]) + 0);

                            if (gHAStatus == "" && gAttend == "")
                            {
                                BCell7.Text = gAPeopleCT.ToString();
                            }
                            else if (gHAStatus == "" && gAttend != "")
                            {
                                BCell7.Text = gAttend;
                            }
                            else if (gHAStatus != "" && gAttend == "")
                            {
                                BCell7.Text = gHAStatus;
                            }
                            else if (gHAStatus != "" && gAttend != "")
                            {
                                BCell7.Text = gHAStatus;
                            }
                            else
                            {
                                BCell7.Text = "";
                            }

                            gACT++;

                        }
                
                    }

                }
                else
                {
                    BCell7 = new TableCell();
                    BCell7.BorderWidth = 1;
                    BCell7.HorizontalAlign = HorizontalAlign.Center;
                    BCell7.Text = "";
                    BRow.Cells.Add(BCell7);

                }
            }




            //********表格後段內容結束********
            #endregion


            TBL_HCAttendRec.Rows.Add(BRow);






        }















    }

    #endregion




    protected void Page_Load(object sender, EventArgs e)
    {






        if (!IsPostBack)
        {
            RegScript();

            //課程名稱
            //如需修改ORDER BY，務必連showtable裡的QueryHCourseName的ORDER BY一併變更
            //AE20250710_改抓近2個月的課程就好
            SqlDataReader QueryHCN = SQLdatabase.ExecuteReader("SELECT a.HCourseName, a.HBookByDateYN,  MIN(b.Value) AS StartDate, MAX(b.value) AS EndDate FROM HCourseBooking AS a JOIN HCourse AS c ON a.HCourseID = c.HID Cross Apply SPLIT(Replace(a.HDateRange,' - ',','), ',') b WHERE c.HStatus=1 AND c.HVerifyStatus=2 AND a.HStatus=1 AND a.HItemStatus=1 AND (TRY_CAST(b.Value AS DATE) >= DATEADD(MONTH, -2, CAST(GETDATE() AS DATE)) AND TRY_CAST(b.Value AS DATE) <= DATEADD(MONTH, 1, CAST(GETDATE() AS DATE))) GROUP BY a.HCourseName, a.HDateRange, a.HBookByDateYN ORDER BY a.HDateRange DESC");
            //DDL_HCourseName.Items.Add(new ListItem("請選擇課程名稱", "0"));
            while (QueryHCN.Read())
            {
                DDL_HCourseName.Items.Add(new ListItem(QueryHCN["HCourseName"].ToString(), QueryHCN["HCourseName"].ToString()));
            }
            QueryHCN.Close();


            //上課地點
            //AE20250710_改抓placelist&增加排序條件
            SqlDataReader QueryHPN = SQLdatabase.ExecuteReader("SELECT c.HPlaceName FROM HCourseBooking AS a INNER JOIN HCourse AS b ON a.HCourseID = b.HID INNER JOIN HPlaceList AS c ON b.HOCPlace = c.HID WHERE a.HStatus=1 AND a.HItemStatus=1  GROUP BY c.HPlaceName ,c.strHAreaID ORDER BY c.strHAreaID ASC");
            while (QueryHPN.Read())
            {
                LBox_HOCPlace.Items.Add(new ListItem(QueryHPN["HPlaceName"].ToString(), QueryHPN["HPlaceName"].ToString()));
            }
            QueryHPN.Close();




        }


    }





    protected void DDL_HCourseName_SelectedIndexChanged(object sender, EventArgs e)
    {
        RegScript();

        DDL_HDateRange.Items.Clear();

        Panel_Report.Visible = false;


        if (DDL_HCourseName.SelectedValue != "0")
        {
            //課程日期
            SqlDataReader QueryHDR = SQLdatabase.ExecuteReader("SELECT HDateRange FROM OrderList_Merge WHERE HStatus=1 AND HItemStatus=1 AND HCourseName='" + DDL_HCourseName.SelectedValue + "' GROUP BY HCourseName, HDateRange ORDER BY HDateRange DESC");
            //DDL_HDateRange.Items.Add(new ListItem("請選擇日期", "0"));
            while (QueryHDR.Read())
            {
                DDL_HDateRange.Items.Add(new ListItem(QueryHDR["HDateRange"].ToString(), QueryHDR["HDateRange"].ToString()));
            }
            QueryHDR.Close();
            //日期預設帶第一筆
            if (DDL_HDateRange.Items.Count > 0)
            {
                DDL_HDateRange.Items[0].Selected = true;
            }

            //取ListBox的值，使用ForEach方式
            string gLBox_HOCPlace = "";
            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    gLBox_HOCPlace += LBoxHOCPlace.Value + "&";
                }
            }

            //上課地點
            LBox_HOCPlace.Items.Clear();
            SqlDataReader QueryHPN = SQLdatabase.ExecuteReader("SELECT HPlaceName FROM OrderList_Merge WHERE HStatus=1 AND HItemStatus=1 AND HCourseName='" + DDL_HCourseName.SelectedValue + "' GROUP BY HCourseName, HPlaceName ORDER BY HPlaceName");
            while (QueryHPN.Read())
            {
                LBox_HOCPlace.Items.Add(new ListItem(QueryHPN["HPlaceName"].ToString(), (QueryHPN["HPlaceName"].ToString())));
            }
            QueryHPN.Close();

            string[] gHOCPlace = gLBox_HOCPlace.Split('&');
            for (int i = 0; i < gHOCPlace.Length - 1; i++)
            {
                for (int j = 0; j < LBox_HOCPlace.Items.Count; j++)
                {
                    if (gHOCPlace[i].ToString() == LBox_HOCPlace.Items[j].Value)
                    {
                        LBox_HOCPlace.Items[j].Selected = true;
                    }
                }
            }
        }
        else if (DDL_HCourseName.SelectedValue == "0" && LBox_HOCPlace.SelectedValue == "")
        {
            LBox_HOCPlace.Items.Clear();
            //上課地點
            SqlDataReader QueryHPN = SQLdatabase.ExecuteReader("SELECT HPlaceName FROM OrderList_Merge WHERE HStatus=1 AND HItemStatus=1 GROUP BY HPlaceName ORDER BY HPlaceName DESC");
            while (QueryHPN.Read())
            {
                LBox_HOCPlace.Items.Add(new ListItem(QueryHPN["HPlaceName"].ToString(), QueryHPN["HPlaceName"].ToString()));
            }
            QueryHPN.Close();

        }
        else
        {

        }


    }






    protected void LBox_HOCPlace_SelectedIndexChanged(object sender, EventArgs e)
    {
        RegScript();

        if (LBox_HOCPlace.SelectedValue != "")
        {
            //取ListBox的值，使用ForEach方式
            string gLBox_HOCPlace = "";
            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    gLBox_HOCPlace += LBoxHOCPlace.Value + "&";
                }
            }

            string gHCourseName = DDL_HCourseName.SelectedValue;
            string gHDateRange = DDL_HDateRange.SelectedValue;


            //課程名稱
            DDL_HCourseName.Items.Clear();
            SqlDataReader QueryHCN = SQLdatabase.ExecuteReader("SELECT a.HCourseName FROM OrderList_Merge AS a CROSS APPLY SPLIT('" + gLBox_HOCPlace.Trim('&') + "', '&') AS b WHERE a.HStatus=1 AND a.HItemStatus=1 AND a.HPlaceName=b.value GROUP BY HCourseName, HDateRange ORDER BY HDateRange DESC");
            //Response.Write("SELECT a.HCourseName FROM OrderList_Merge AS a CROSS APPLY SPLIT('" + gLBox_HOCPlace.Trim('&') + "', '&') AS b WHERE a.HStatus=1 AND a.HItemStatus=1 AND a.HPlaceName=b.value GROUP BY HCourseName, HDateRange ORDER BY HDateRange DESC");
            //Response.End();
            DDL_HCourseName.Items.Add(new ListItem("請選擇課程名稱", "0"));
            while (QueryHCN.Read())
            {
                DDL_HCourseName.Items.Add(new ListItem(QueryHCN["HCourseName"].ToString(), QueryHCN["HCourseName"].ToString()));
            }
            QueryHCN.Close();

            //課程日期
            DDL_HDateRange.Items.Clear();
            SqlDataReader QueryHDR = SQLdatabase.ExecuteReader("SELECT HDateRange FROM OrderList_Merge WHERE HStatus=1 AND HItemStatus=1 AND HCourseName='" + gHCourseName + "' GROUP BY HCourseName, HDateRange ORDER BY HDateRange DESC");
            while (QueryHDR.Read())
            {
                DDL_HDateRange.Items.Add(new ListItem(QueryHDR["HDateRange"].ToString(), QueryHDR["HDateRange"].ToString()));
            }
            QueryHDR.Close();


            DDL_HCourseName.SelectedValue = gHCourseName;
            DDL_HDateRange.SelectedValue = gHDateRange;




        }
        else if (LBox_HOCPlace.SelectedValue == "" && DDL_HCourseName.SelectedValue == "0")
        {
            //課程名稱
            DDL_HDateRange.Items.Clear();
            DDL_HCourseName.Items.Clear();
            SqlDataReader QueryHCN = SQLdatabase.ExecuteReader("SELECT HCourseName FROM OrderList_Merge WHERE HStatus=1 AND HItemStatus=1 GROUP BY HCourseName, HDateRange ORDER BY HDateRange DESC");
            DDL_HCourseName.Items.Add(new ListItem("請選擇課程名稱", "0"));
            while (QueryHCN.Read())
            {
                DDL_HCourseName.Items.Add(new ListItem(QueryHCN["HCourseName"].ToString(), QueryHCN["HCourseName"].ToString()));
            }
            QueryHCN.Close();

        }
        else
        {

        }







    }












    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        RegScript();


        string gHOCPlace = "";
        string gHOCPlaceBBDate = "";
        string gLBoxHOCPlace = "";
        string gLBoxHOCPlaceBBDate = "";

        if (DDL_HCourseName.SelectedValue == "" || DDL_HCourseName.SelectedValue == "0")
        {
            //Response.Write("<script>alert('請至少選一個地點喔!');window.location.href='HStayList.aspx';</script>");
            Response.Write("<script>alert('請至少選一個課程名稱喔!');</script>");
            return;
        }

        if (DDL_HDateRange.SelectedValue == "" || DDL_HDateRange.SelectedValue == "0")
        {
            //Response.Write("<script>alert('請至少選一個地點喔!');window.location.href='HStayList.aspx';</script>");
            Response.Write("<script>alert('請至少選一個上課日期喔!');</script>");
            return;
        }


        //判斷上課地點有無輸入
        if (LBox_HOCPlace.SelectedValue == "")//上課地點沒輸入
        {
            gHOCPlace = "";
            gHOCPlaceBBDate = "";
        }
        else //上課地點有輸入
        {
            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    //gLBoxHOCPlace += " HPlaceName='" + LBoxHOCPlace.Value + "' or";
                    gLBoxHOCPlace += " b.HPlaceName='" + LBoxHOCPlace.Value + "' or";
                    gLBoxHOCPlaceBBDate += " g.HPlaceName='" + LBoxHOCPlace.Value + "' or";
                }
            }
            gHOCPlace = " AND (" + gLBoxHOCPlace.Substring(0, gLBoxHOCPlace.Length - 2) + ")";
            gHOCPlaceBBDate = " AND (" + gLBoxHOCPlaceBBDate.Substring(0, gLBoxHOCPlaceBBDate.Length - 2) + ")";
        }



        //Response.Write("<br/><br/><br/><br/><br/>gHOCPlace=" + gHOCPlaceBBDate + "<br/>");
        //Response.End();


        //AA20250623_要先判斷是否為七天班會，用不同的方式產生
        SqlDataReader QueryHBookByDate = SQLdatabase.ExecuteReader("SELECT HBookByDateYN FROM HCourse WHERE HCourseName='" + DDL_HCourseName.SelectedItem.Text + "' AND HDateRange ='" + DDL_HDateRange.SelectedItem.Text + "' AND HVerifyStatus=2 AND HStatus=1 GROUP BY HCourseName, HDateRange, HBookByDateYN");

        //Response.Write("<br/><br/><br/><br/><br/>gHOCPlace=" + gHOCPlace + "<br/>");

        //Response.Write("<br/><br/><br/><br/><br/>SELECT HBookByDateYN FROM HCourse WHERE HCourseName='" + DDL_HCourseName.SelectedItem.Text + "' AND HDateRange ='" + DDL_HDateRange.SelectedItem.Text + "' AND HVerifyStatus=2 AND HStatus=1 GROUP BY HCourseName, HDateRange, HBookByDateYN<br/>");


        if (QueryHBookByDate.Read())
        {
            if (QueryHBookByDate["HBookByDateYN"].ToString() == "1")
            {
                ShowITableBBDate(DDL_HCourseName.SelectedItem.Text, DDL_HDateRange.SelectedItem.Text, gHOCPlaceBBDate);
            }
            else
            {
                ShowITable(DDL_HCourseName.SelectedItem.Text, DDL_HDateRange.SelectedItem.Text, gHOCPlace);
            }
        }
        QueryHBookByDate.Close();


        Panel_Report.Visible = true;

        //課程名稱
        LB_HCourseName.Text = DDL_HCourseName.SelectedItem.Text;


    }




    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        RegScript();

        DDL_HCourseName.Text = "0";
        DDL_HDateRange.Text = null;
        LBox_HOCPlace.Text = null;

        DDL_HCourseName.ClearSelection();
        DDL_HDateRange.ClearSelection();
        LBox_HOCPlace.ClearSelection();

        DDL_HCourseName.Items.Clear();
        DDL_HDateRange.Items.Clear();
        LBox_HOCPlace.Items.Clear();

        TBL_HCAttendRec.Dispose();
        LB_HCourseName.Text = "";



        //課程名稱
        SqlDataReader QueryHCN = SQLdatabase.ExecuteReader("SELECT HCourseName FROM OrderList_Merge WHERE HStatus=1 AND HItemStatus=1 GROUP BY HCourseName, HDateRange ORDER BY HDateRange DESC");
        DDL_HCourseName.Items.Add(new ListItem("請選擇課程名稱", "0"));
        while (QueryHCN.Read())
        {
            DDL_HCourseName.Items.Add(new ListItem(QueryHCN["HCourseName"].ToString(), QueryHCN["HCourseName"].ToString()));
        }
        QueryHCN.Close();

        //上課地點
        SqlDataReader QueryHPN = SQLdatabase.ExecuteReader("SELECT HPlaceName FROM OrderList_Merge WHERE HStatus=1 AND HItemStatus=1 GROUP BY HPlaceName ORDER BY HPlaceName DESC");
        while (QueryHPN.Read())
        {
            LBox_HOCPlace.Items.Add(new ListItem(QueryHPN["HPlaceName"].ToString(), QueryHPN["HPlaceName"].ToString()));
        }
        QueryHPN.Close();






    }






    protected void IBtn_ToExcel_Click(object sender, ImageClickEventArgs e)
    {
        RegScript();
        string gHOCPlace = "";
        string gHOCPlaceBBDate = "";
        string gLBoxHOCPlace = "";
        string gLBoxHOCPlaceBBDate = "";

        if (DDL_HCourseName.SelectedValue == "")
        {
            //Response.Write("<script>alert('請至少選一個地點喔!');window.location.href='HStayList.aspx';</script>");
            Response.Write("<script>alert('請至少選一個課程名稱喔!');</script>");
            return;
        }

        if (DDL_HCourseName.SelectedValue == "")
        {
            //Response.Write("<script>alert('請至少選一個地點喔!');window.location.href='HStayList.aspx';</script>");
            Response.Write("<script>alert('請至少選一個上課日期喔!');</script>");
            return;
        }


        //判斷上課地點有無輸入
        if (LBox_HOCPlace.SelectedValue == "")//上課地點沒輸入
        {
            gHOCPlace = "";
            gHOCPlaceBBDate = "";
        }
        else //上課地點有輸入
        {
            foreach (ListItem LBoxHOCPlace in LBox_HOCPlace.Items)
            {
                if (LBoxHOCPlace.Selected == true)
                {
                    //gLBoxHOCPlace += " HPlaceName='" + LBoxHOCPlace.Value + "' or";
                    gLBoxHOCPlace += " b.HPlaceName='" + LBoxHOCPlace.Value + "' or";
                    gLBoxHOCPlaceBBDate += " g.HPlaceName='" + LBoxHOCPlace.Value + "' or";
                }
            }
            gHOCPlace = " AND (" + gLBoxHOCPlace.Substring(0, gLBoxHOCPlace.Length - 2) + ")";
            gHOCPlaceBBDate = " AND (" + gLBoxHOCPlaceBBDate.Substring(0, gLBoxHOCPlaceBBDate.Length - 2) + ")";
        }

        //AA20250623_要先判斷是否為七天班會，用不同的方式產生
        SqlDataReader QueryHBookByDate = SQLdatabase.ExecuteReader("SELECT HBookByDateYN FROM HCourse WHERE HCourseName='" + DDL_HCourseName.SelectedItem.Text + "' AND HDateRange ='" + DDL_HDateRange.SelectedItem.Text + "' AND HVerifyStatus=2 AND HStatus=1 GROUP BY HCourseName, HDateRange, HBookByDateYN");


        if (QueryHBookByDate.Read())
        {
            if (QueryHBookByDate["HBookByDateYN"].ToString() == "1")
            {
                ShowITableBBDate(DDL_HCourseName.SelectedItem.Text, DDL_HDateRange.SelectedItem.Text, gHOCPlaceBBDate);
            }
            else
            {
                ShowITable(DDL_HCourseName.SelectedItem.Text, DDL_HDateRange.SelectedItem.Text, gHOCPlace);
            }
        }
        QueryHBookByDate.Close();


        //ShowITable(DDL_HCourseName.SelectedItem.Text, DDL_HDateRange.SelectedItem.Text, gHOCPlace);


        //課程名稱
        LB_HCourseName.Text = DDL_HCourseName.SelectedItem.Text;



        Response.Clear();
        //通知瀏覽器下載檔案
        Response.AddHeader("content-disposition", "attachment;filename=課程出席紀錄表.xls");
        Response.ContentType = "application/application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        System.IO.StringWriter sw = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter hw = new HtmlTextWriter(sw);
        TBL_HCAttendRec.RenderControl(hw);
        //Panel1.RenderControl(hw);
        System.IO.StreamWriter swUTF8 = new System.IO.StreamWriter(Response.OutputStream, System.Text.Encoding.UTF8);//這樣一筆的時侯,就不會亂碼了
                                                                                                                     //Response.Write(sw.ToString());
        swUTF8.Write(sw.ToString());
        swUTF8.Close();
        Response.End();




    }







    protected void LBtn_Check_Click(object sender, EventArgs e)
    {
        //Panel_CourseList.Visible = false;
        Panel_Report.Visible = true;
    }

    protected void LBtn_Back_Click(object sender, EventArgs e)
    {
        Response.Write("<script>window.location.href='HCourseAttendRec.aspx';</script>");
    }


    protected void Btn_Cancel_Click(object sender, EventArgs e)
    {

        Response.Write("<script>window.location.href='HCourseAttendRec.aspx';</script>");
    }


    #region 執行js
    public void RegScript()
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$('.js-example-basic-single').select2({closeOnSelect: true,});$('.select2-multiple').select2();"), true);//執行js



    }
    #endregion
}