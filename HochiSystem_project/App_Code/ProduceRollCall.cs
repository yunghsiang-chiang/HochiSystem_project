using System;
using System.Data;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI;
using RestSharp;
using System.Collections.Generic;
using System.Linq;

public class ProduceRollCall
{
    //static public string GetRollCall(string gGERPTable)//需回傳值
    //gHID=LBtn_Produce_CA，VS_dtRC=ViewState["dtRC"]，gHUserHID=((Label)Master.FindControl("LB_HUserHID")).Text
    //gHID=HCourse.HID，VS_dtRC=建立dataTable架架構，gUserHID=登入者HID
    static public void GetRollCall(string gHID, DataTable VS_dtRC, string gHUserHID)//不需回傳值
    {
        SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

        string gSDate = "";
        string gEDate = "";

        //判斷天數相差幾天
        int gDateRange = 0;//連續天數



        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        string strSelHRC = "SELECT TOP(1) HID FROM HRollCall WHERE HCourseID='" + gHID + "'";
        dbCmd = new SqlCommand(strSelHRC, dbConn);
        SqlDataReader MyQueryHRC = dbCmd.ExecuteReader();
        if (MyQueryHRC.Read())
        {
            //HttpContext.Current.Response.Write("<br/>第二次產生點名單<br/>");

            //已產生過點名單，則重新產生
            string[] gHDateRange = { };
            string gHDateRange1 = "";

            //GE20240409_將OrderList_Merge改成直接寫
            string strSelHCBMC = "SELECT a.HCourseID, a.HMemberID, a.HDateRange, a.HCGuide FROM (SELECT A.HID, A.HMemberID, A.HCourseID, A.HDateRange,A.HCGuide FROM HCourseBooking AS A INNER JOIN HCourse AS B On A.HCourseID = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND (A.HChangeStatus IS NULL OR A.HChangeStatus = '2') UNION SELECT D.HID, A.HMemberID, D.HCourseIDNew AS HCourseID,  D.HDateRange, D.HCGuide FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON A.HOrderGroup = D.HOrderGroupNew INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND A.HCourseName = '換課' AND(D.HChangeStatus IS NULL OR D.HChangeStatus = '2') UNION SELECT D.HID, A.HMemberID, D.HCourseIDNew AS HCourseID, D.HDateRange,  D.HCGuide FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON(A.HOrderGroup = D.HOrderGroupSrc AND A.HOrderNum = D.HOrderNumSrc) OR(A.HOrderGroup = D.HOrderGroupSrc AND D.HOrderNumSrc = '') INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND(D.HOrderGroupNew = '' OR D.HOrderGroupNew IS NULL) UNION SELECT D.HID, A.HMemberID, D.HCourseIDNew AS HCourseID, D.HDateRange, D.HCGuide FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON(A.HOrderGroup = D.HOrderGroupSrc) LEFT JOIN HCBChangeRecord AS E ON(D.HOrderGroupSrc = E.HOrderGroupNew) INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND A.HCourseName = '換課' AND(D.HOrderGroupNew = '' OR D.HOrderGroupNew IS NULL) AND(E.HChangeStatus = '1') AND(A.HChangeStatus IS NULL OR A.HChangeStatus = '2')) as a join HMember as b on a.HMemberID=b.HID where a.HCourseID='" + gHID + "'";


            dbCmd = new SqlCommand(strSelHCBMC, dbConn);
            SqlDataReader MyQueryHCBMC = dbCmd.ExecuteReader();
            while (MyQueryHCBMC.Read())
            {

                if (MyQueryHCBMC["HDateRange"].ToString().IndexOf("-") >= 0)
                {
                    gHDateRange = MyQueryHCBMC["HDateRange"].ToString().Trim(',').Split('-');
                    string gDate = "";
                    string gDate1 = "";
                    gSDate = (gHDateRange[0]);
                    gEDate = (gHDateRange[1]);
                    gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;

                    //GA20250621_防止產生過多點名單
                    if (gDateRange < 60)
                    {
                        for (int i = 0; i < gDateRange; i++)
                        {
                            gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                            gDate1 += "'" + Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + "'" + ",";
                        }

                        gHDateRange = gDate.Trim(',').Split(',');
                        gHDateRange1 = gDate1.Trim(',');
                    }

                }
                else if (MyQueryHCBMC["HDateRange"].ToString().IndexOf(",") >= 0)
                {
                    gHDateRange = MyQueryHCBMC["HDateRange"].ToString().Trim(',').Split(',');
                    string gDate1 = "";
                    //GA20250621_防止產生過多點名單
                    if (gHDateRange.Length < 60)
                    {
                        for (int i = 0; i <= gHDateRange.Length - 1; i++)
                        {
                            gDate1 += "'" + gHDateRange[i] + "'" + ",";
                        }
                    }
                    gHDateRange1 = gDate1.Trim(',');
                }
                else if (MyQueryHCBMC["HDateRange"].ToString().IndexOf("-") < 0 && MyQueryHCBMC["HDateRange"].ToString().IndexOf(",") < 0)//單一日期
                {
                    gHDateRange = (MyQueryHCBMC["HDateRange"].ToString() + " - " + MyQueryHCBMC["HDateRange"].ToString()).Split('-');
                    string gDate = "";
                    string gDate1 = "";
                    gSDate = (gHDateRange[0]);
                    gEDate = (gHDateRange[1]);
                    gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;
                    MyQueryHCBMC["HDateRange"].ToString().Trim(',');
                    for (int i = 0; i < gDateRange; i++)
                    {
                        gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                        gDate1 += "'" + Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + "'" + ",";
                    }

                    gHDateRange = gDate.Trim(',').Split(',');
                    gHDateRange1 = gDate1.Trim(',');
                }
                else
                {

                }

                //新增參班點名人數
                string strSelHNewRC = "SELECT HCourseID, HMemberID, HRCDate FROM HRollCall where HCourseID='" + MyQueryHCBMC["HCourseID"].ToString() + "' AND HMemberID='" + MyQueryHCBMC["HMemberID"].ToString() + "' AND HRCDate IN (" + gHDateRange1 + ")";


                dbCmd = new SqlCommand(strSelHNewRC, dbConn);
                SqlDataReader MyQueryHNewRC = dbCmd.ExecuteReader();
                string DiffDate = "";
                string gHRCDateList = "";
                if (MyQueryHNewRC.HasRows)
                {
                    string[] gHDateRangeArray = { };
                    string[] gHRCDateArray = { };

                    while (MyQueryHNewRC.Read())
                    {
                        gHRCDateList += "'" + Convert.ToDateTime(MyQueryHNewRC["HRCDate"].ToString()).ToString("yyyy/MM/dd") + "'" + ",";
                    }
                    gHDateRangeArray = gHDateRange1.Trim(',').Split(',');
                    gHRCDateArray = gHRCDateList.Trim(',').Split(',');

                    //找出二陣列差異
                    IEnumerable<string> gCompare = gHDateRangeArray.Except(gHRCDateArray);
                    foreach (string Diff in gCompare)
                    {
                        DiffDate = DiffDate + Diff + ",";
                    }

                }
                else
                {
                    DiffDate = gHDateRange1.Trim(',');
                }
                MyQueryHNewRC.Close();

                //HttpContext.Current.Response.Write("<br/><br/>DiffDate=" + DiffDate + "< br/><br/>");

                string[] gDiffDate = DiffDate.Trim(',').Split(',');


                //GA20250621_避免產生過多點名單，因最長連續課程最多一年、每周一次，所以抓小於60天才產生
                if (gDiffDate.Length < 60)
                {

                    for (int x = 0; x < gDiffDate.Length; x++)
                    {



                        if (gDiffDate[x].Trim('\'').Trim().ToString() != "")
                        {

                            string strInsHRC = "Insert Into HRollCall (HCourseID, HMemberID, HRCDate, HAStatus, HStatus, HCreate, HCreateDT) values ('" + MyQueryHCBMC["HCourseID"].ToString() + "', '" + MyQueryHCBMC["HMemberID"].ToString() + "', '" + gDiffDate[x].Trim('\'').ToString() + "', '0','1','" + gHUserHID + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "')";

                            dbCmd = new SqlCommand(strInsHRC, dbConn);
                            dbCmd.ExecuteNonQuery();

                            //新增護持點名人數
                            if (MyQueryHCBMC["HCGuide"].ToString() != "" && MyQueryHCBMC["HCGuide"].ToString() != null)
                            {
                                string[] gHCGuide = MyQueryHCBMC["HCGuide"].ToString().Split(',');
                                for (int j = 0; j < gHCGuide.Length - 1; j++)
                                {
                                    string strInsHRCa = "Insert Into HRollCall (HCourseID, HMemberID, HRCDate, HAStatus, HStatus, HCreate, HCreateDT) values ('" + MyQueryHCBMC["HCourseID"].ToString() + "', '" + gHCGuide[j].ToString() + "', '" + gDiffDate[x].Trim('\'').ToString() + "', '0','1','" + gHUserHID + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "')";

                                    dbCmd = new SqlCommand(strInsHRCa, dbConn);
                                    dbCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

            }
            MyQueryHCBMC.Close();




        }
        else
        {
            //HttpContext.Current.Response.Write("<br/>第一次產生點名單<br/>");

            string[] gHDateRange = { };

            //GE20240409_將OrderList_Merge改成直接寫
            string strSelHCBMC = "SELECT a.HCourseID, a.HMemberID, a.HDateRange, a.HCGuide FROM  (SELECT A.HID, A.HMemberID, A.HCourseID, A.HDateRange,A.HCGuide FROM HCourseBooking AS A INNER JOIN HCourse AS B On A.HCourseID = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND (A.HChangeStatus IS NULL OR A.HChangeStatus = '2') UNION SELECT D.HID, A.HMemberID, D.HCourseIDNew AS HCourseID,  D.HDateRange, D.HCGuide FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON A.HOrderGroup = D.HOrderGroupNew INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND A.HCourseName = '換課' AND(D.HChangeStatus IS NULL OR D.HChangeStatus = '2') UNION SELECT D.HID, A.HMemberID, D.HCourseIDNew AS HCourseID, D.HDateRange,  D.HCGuide FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON(A.HOrderGroup = D.HOrderGroupSrc AND A.HOrderNum = D.HOrderNumSrc) OR(A.HOrderGroup = D.HOrderGroupSrc AND D.HOrderNumSrc = '') INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND(D.HOrderGroupNew = '' OR D.HOrderGroupNew IS NULL) UNION SELECT D.HID, A.HMemberID, D.HCourseIDNew AS HCourseID, D.HDateRange,  D.HCGuide FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON(A.HOrderGroup = D.HOrderGroupSrc) LEFT JOIN HCBChangeRecord AS E ON(D.HOrderGroupSrc = E.HOrderGroupNew) INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND A.HCourseName = '換課' AND(D.HOrderGroupNew = '' OR D.HOrderGroupNew IS NULL) AND(E.HChangeStatus = '1') AND(A.HChangeStatus IS NULL OR A.HChangeStatus = '2')) as a join HMember as b on a.HMemberID=b.HID WHERE a.HCourseID='" + gHID + "' ";

            dbCmd = new SqlCommand(strSelHCBMC, dbConn);
            SqlDataReader MyQueryHCBMC = dbCmd.ExecuteReader();
            while (MyQueryHCBMC.Read())
            {
                if (MyQueryHCBMC["HDateRange"].ToString().IndexOf("-") >= 0)
                {
                    gHDateRange = MyQueryHCBMC["HDateRange"].ToString().Trim(',').Split('-');
                    string gDate = "";
                    string gDate1 = "";
                    gSDate = (gHDateRange[0]);
                    gEDate = (gHDateRange[1]);
                    gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;

                    if (gDateRange < 60)
                    {
                        for (int i = 0; i < gDateRange; i++)
                        {
                            gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                            gDate1 += "'" + Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + "'" + ",";
                        }

                        gHDateRange = gDate.Trim(',').Split(',');
                    }

                }
                else if (MyQueryHCBMC["HDateRange"].ToString().IndexOf(",") >= 0)
                {
                    gHDateRange = MyQueryHCBMC["HDateRange"].ToString().Trim(',').Split(',');
                }
                else if (MyQueryHCBMC["HDateRange"].ToString().IndexOf("-") < 0 && MyQueryHCBMC["HDateRange"].ToString().IndexOf(",") < 0)//單一日期
                {
                    gHDateRange = (MyQueryHCBMC["HDateRange"].ToString() + " - " + MyQueryHCBMC["HDateRange"].ToString()).Split('-');
                    string gDate = "";
                    string gDate1 = "";
                    gSDate = (gHDateRange[0]);
                    gEDate = (gHDateRange[1]);
                    gDateRange = (Convert.ToDateTime(gEDate) - Convert.ToDateTime(gSDate)).Days + 1;

                    for (int i = 0; i < gDateRange; i++)
                    {
                        gDate += Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + ",";
                        gDate1 += "'" + Convert.ToDateTime(gSDate).AddDays(i).ToString("yyyy/MM/dd").ToString() + "'" + ",";
                    }

                    gHDateRange = gDate.Trim(',').Split(',');
                    //gHDateRange1 = gDate.Trim(',');

                }
                else
                {
                    //GE20231004_改成保留搜尋條件
                    //HttpContext.Current.Response.Write("<script>alert('日期錯誤!');</script>");
                    //ScriptManager.RegisterStartupScript(this, Page.GetType(), "alert", "alert('日期錯誤!');", true);
                    return;
                }


                //JE20241111改用sqlulkcopy寫法
                DataTable dtRC = (DataTable)VS_dtRC;
                //GA20250621_避免產生過多點名單，因最長連續課程為1個月，所以抓小於40天才產生
                if (gHDateRange.Length < 60)
                {
                    for (int i = 0; i <= gHDateRange.Length - 1; i++)
                    {
                        DateTime gHDate = Convert.ToDateTime(gHDateRange[i].ToString());//上課日

                        //新增資料
                        DataRow rowRC;
                        rowRC = dtRC.NewRow();
                        rowRC["HCourseID"] = MyQueryHCBMC["HCourseID"].ToString();
                        rowRC["HMemberID"] = MyQueryHCBMC["HMemberID"].ToString();
                        rowRC["HRCDate"] = gHDate.ToString("yyyy/MM/dd");
                        rowRC["HAStatus"] = "0";
                        rowRC["HStatus"] = "1";
                        rowRC["HCreate"] = gHUserHID;
                        rowRC["HCreateDT"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                        dtRC.Rows.Add(rowRC);
                        VS_dtRC = dtRC;

                        //HttpContext.Current.Response.Write("<br/>VS_dtRC:" + VS_dtRC.Rows.Count + "<br/>");


                        //新增護持點名人數
                        if (MyQueryHCBMC["HCGuide"].ToString() != "" && MyQueryHCBMC["HCGuide"].ToString() != null)
                        {
                            string[] gHCGuide = MyQueryHCBMC["HCGuide"].ToString().Split(',');
                            //JE20241111改用sqlulkcopy寫法
                            DataTable dtRCa = (DataTable)VS_dtRC;
                            for (int j = 0; j < gHCGuide.Length - 1; j++)
                            {
                                //新增資料
                                DataRow rowRCa;
                                rowRCa = dtRCa.NewRow();
                                rowRCa["HCourseID"] = MyQueryHCBMC["HCourseID"].ToString();
                                rowRCa["HMemberID"] = gHCGuide[j].ToString();
                                rowRCa["HRCDate"] = gHDate.ToString("yyyy/MM/dd");
                                rowRCa["HAStatus"] = "0";
                                rowRCa["HStatus"] = "1";
                                rowRCa["HCreate"] = gHUserHID;
                                rowRCa["HCreateDT"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                                dtRCa.Rows.Add(rowRCa);

                                VS_dtRC = dtRCa;



                            }
                            //清除DataTable以釋放記憶體(如果不需要或有異常可先註解)
                            //dtRCa.Clear();
                        }
                    }
                }
                //清除DataTable以釋放記憶體(如果不需要或有異常可先註解)
                //dtRC.Clear();

            }
            MyQueryHCBMC.Close();





            //寫入資料庫(使用SqlBulkCopy)
            DataTable dtRCAdd = (DataTable)VS_dtRC;


            SqlBulkCopy bulkCopy = new SqlBulkCopy(ConStr);
            //設定一個批次量寫入多少筆資料
            bulkCopy.BatchSize = 100;
            //設定逾時的秒數
            bulkCopy.BulkCopyTimeout = 60;
            //設定要寫入的資料庫
            bulkCopy.DestinationTableName = "HRollCall";
            //HCourseID, HMemberID, HRCDate, HAStatus, HStatus, HCreate, HCreateDT
            //對應資料行
            bulkCopy.ColumnMappings.Add("HCourseID", "HCourseID");
            bulkCopy.ColumnMappings.Add("HMemberID", "HMemberID");
            bulkCopy.ColumnMappings.Add("HRCDate", "HRCDate");
            bulkCopy.ColumnMappings.Add("HAStatus", "HAStatus");
            bulkCopy.ColumnMappings.Add("HStatus", "HStatus");
            bulkCopy.ColumnMappings.Add("HCreate", "HCreate");
            bulkCopy.ColumnMappings.Add("HCreateDT", "HCreateDT");

            ConStr.Open();
            //開始寫入
            bulkCopy.WriteToServer(dtRCAdd);
            ConStr.Close();
            //清除DataTable以釋放記憶體(如果不需要或有異常可先註解)
            dtRCAdd.Clear();








        }

        MyQueryHRC.Close();










    }










}
