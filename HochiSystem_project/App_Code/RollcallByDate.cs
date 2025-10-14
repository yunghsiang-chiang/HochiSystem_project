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

/// <summary>
/// RollcallByDate 的摘要描述
/// </summary>
public class RollcallByDate
{
    //static public string GetRollCall(string gGERPTable)//需回傳值
    //gHID=LBtn_Produce_CA，VS_dtRC=ViewState["dtRC"]，gHUserHID=((Label)Master.FindControl("LB_HUserHID")).Text
    //gHID=HCourse.HID，VS_dtRC=建立dataTable架架構(點名單)，gUserHID=登入者HID
    static public void GetRollCall(string gHID, DataTable VS_dtRC, string gHUserHID)
    {
        SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

        //string gSDate = "";
        //string gEDate = "";

        ////判斷天數相差幾天
        //int gDateRange = 0;//連續天數

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
            //string gHDateRange1 = ""; //字串，用來撈現在已產生點名單的內容
            string gHBookedDate = ""; //字串，用來存報名的日期

            //撈報名的資料
            string strSelHCBMC = "SELECT a.HCourseID, a.HMemberID, a.HDateRange, b.HDate, b.HAttend FROM HCourseBooking AS a INNER JOIN HCourseBooking_DateAttend AS b ON a.HID=b.HCourseBookingID WHERE a.HItemStatus=1 AND a.HStatus=1 AND a.HChangeStatus=2 AND a.HCourseID='" + gHID + "'";


            //HttpContext.Current.Response.Write("<br/>strSelHCBMC=" + strSelHCBMC + "<br/>");

            dbCmd = new SqlCommand(strSelHCBMC, dbConn);
            SqlDataReader MyQueryHCBMC = dbCmd.ExecuteReader();
            while (MyQueryHCBMC.Read())
            {
                //報名的日期
                gHBookedDate = "'" + Convert.ToDateTime(MyQueryHCBMC["HDate"].ToString()).ToString("yyyy/MM/dd")+"'";


                //新增參班點名人數
                //STEP1、先撈出目前相同報名日期已產生的點名單紀錄
                string strSelHNewRC = "SELECT HCourseID, HMemberID, HRCDate FROM HRollCall where HCourseID='" + MyQueryHCBMC["HCourseID"].ToString() + "' AND HMemberID='" + MyQueryHCBMC["HMemberID"].ToString() + "' AND HRCDate IN (" + gHBookedDate + ")";


                //HttpContext.Current.Response.Write("<br/>strSelHNewRC=" + strSelHNewRC + "<br/>");

                dbCmd = new SqlCommand(strSelHNewRC, dbConn);
                SqlDataReader MyQueryHNewRC = dbCmd.ExecuteReader();
                string DiffDate = "";
                string gHRCDateList = "";
                if (MyQueryHNewRC.HasRows)
                {
                    //string[] gHDateRangeArray = { };
                    //string[] gHRCDateArray = { };

                    //while (MyQueryHNewRC.Read())
                    //{
                    //    gHRCDateList += "'" + MyQueryHNewRC["HRCDate"].ToString() + "'" + ",";  //目前已有的點名資料
                    //}
                    //gHDateRangeArray = gHBookedDate.Trim(',').Split(',');  //課程完整的上課日期
                    //gHRCDateArray = gHRCDateList.Trim(',').Split(',');  //點名單的日期(報名的上課日期)

                    ////HttpContext.Current.Response.Write("<br/>gHRCDateList=" + gHRCDateList + "<br/>");

                    ////HttpContext.Current.Response.Write("<br/>gHDateRangeArray=" + string.Join(", ", gHDateRangeArray) + "<br/>");

                    ////HttpContext.Current.Response.Write("<br/>gHRCDateArray=" + string.Join(", ", gHRCDateArray)  + "<br/>");


                    ////找出二陣列差異
                    //IEnumerable<string> gCompare = gHDateRangeArray.Except(gHRCDateArray);
                    //foreach (string Diff in gCompare)
                    //{
                    //    DiffDate = DiffDate + Diff + ",";
                    //}


                    ////HttpContext.Current.Response.Write("<br/>WOWDiffDate=" + DiffDate + "<br/>");

                    #region GPT優化
                    string courseId = MyQueryHCBMC["HCourseID"].ToString();
                    string memberId = MyQueryHCBMC["HMemberID"].ToString();
                    string hDate = Convert.ToDateTime(MyQueryHCBMC["HDate"]).ToString("yyyy/MM/dd");

                    // 檢查是否已存在點名紀錄
                    string checkSQL = @"SELECT COUNT(*) FROM HRollCall 
                        WHERE HCourseID = @CourseID 
                          AND HMemberID = @MemberID 
                          AND HRCDate = @HRCDate";

                    using (SqlCommand checkCmd = new SqlCommand(checkSQL, dbConn))
                    {
                        checkCmd.Parameters.AddWithValue("@CourseID", courseId);
                        checkCmd.Parameters.AddWithValue("@MemberID", memberId);
                        checkCmd.Parameters.AddWithValue("@HRCDate", hDate);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count == 0)
                        {
                            // 未存在，才執行 INSERT
                            string insertSQL = @"INSERT INTO HRollCall 
                (HCourseID, HMemberID, HRCDate, HAStatus, HStatus, HCreate, HCreateDT)
                VALUES (@CourseID, @MemberID, @HRCDate, '0', '1', @Create, @CreateDT)";

                            using (SqlCommand insertCmd = new SqlCommand(insertSQL, dbConn))
                            {
                                insertCmd.Parameters.AddWithValue("@CourseID", courseId);
                                insertCmd.Parameters.AddWithValue("@MemberID", memberId);
                                insertCmd.Parameters.AddWithValue("@HRCDate", hDate);
                                insertCmd.Parameters.AddWithValue("@Create", gHUserHID);
                                insertCmd.Parameters.AddWithValue("@CreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    DiffDate = gHBookedDate.Trim(',');
                }
                MyQueryHNewRC.Close();

                //HttpContext.Current.Response.Write("<br/><br/>YYDiffDate=" + DiffDate + "< br/><br/>");

                string[] gDiffDate = DiffDate.Trim(',').Split(',');




                for (int x = 0; x < gDiffDate.Length; x++)
                {
                    if (gDiffDate[x].Trim('\'').Trim().ToString() != "")
                    {

                        string strInsHRC = "Insert Into HRollCall (HCourseID, HMemberID, HRCDate, HAStatus, HStatus, HCreate, HCreateDT) values ('" + MyQueryHCBMC["HCourseID"].ToString() + "', '" + MyQueryHCBMC["HMemberID"].ToString() + "', '" + gDiffDate[x].Trim('\'').ToString() + "', '0','1','" + gHUserHID + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "')";



                        dbCmd = new SqlCommand(strInsHRC, dbConn);
                        dbCmd.ExecuteNonQuery();
                    }
                }

            }
            MyQueryHCBMC.Close();




        }
        else
        {
            //HttpContext.Current.Response.Write("<br/>第一次產生點名單<br/>");
            //第一次產生點名單: 直接依報名的日期產生就可以了，因為現在是BY人
            string[] gHDateRange = { };

            string strSelHCBMC = "SELECT a.HCourseID, a.HDateRange, a.HMemberID, b.HDate, b.HAttend FROM HCourseBooking AS a INNER JOIN HCourseBooking_DateAttend AS b ON a.HID=b.HCourseBookingID WHERE a.HItemStatus=1 AND a.HStatus=1 AND a.HChangeStatus=2 AND a.HBookByDateYN=1 AND a.HCourseID='" + gHID + "' ";

            //HttpContext.Current.Response.Write("<br/>SELECT a.HCourseID, a.HDateRange, a.HMemberID, b.HDate, b.HAttend FROM HCourseBooking AS a INNER JOIN HCourseBooking_DateAttend AS b ON a.HID=b.HCourseBookingID WHERE a.HItemStatus=1 AND a.HStatus=1 AND a.HChangeStatus=2 AND a.HCourseID='" + gHID + "'<br/>");


            dbCmd = new SqlCommand(strSelHCBMC, dbConn);
            SqlDataReader MyQueryHCBMC = dbCmd.ExecuteReader();
            while (MyQueryHCBMC.Read())
            {

                //QE20241111改用sqlulkcopy寫法
                DataTable dtRC = (DataTable)VS_dtRC;

                DateTime gHDate = Convert.ToDateTime(MyQueryHCBMC["HDate"].ToString());//報名的上課日

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