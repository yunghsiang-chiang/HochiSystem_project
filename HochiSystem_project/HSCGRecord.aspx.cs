using MySql.Data.MySqlClient;
using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Activities.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Color = System.Drawing.Color;


public partial class HSCGRecord : System.Web.UI.Page
{
    string EIPconnStr = ConfigurationManager.ConnectionStrings["HochiEIPConnection"].ConnectionString;
    SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    #region --根目錄--
    string FileRoot = "D:\\Website\\System\\HochiSystem\\uploads\\HSCGRecord\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
    private object dtRecords;
    #endregion

    #region 速度優化後
    private void ShowITable(int gWhichWeek)
    {
        string loginHID = ((Label)Master.FindControl("LB_HUserHID")).Text;
        string loginHEIPUid = ((TextBox)Master.FindControl("TB_EIPUid")).Text;

        int gDateDiff = 7;
        //DateTime dt = DateTime.Today.AddDays(toMonday(DateTime.Today.DayOfWeek.ToString())).AddDays(gWhichWeek * 7);
        DateTime dt = DateTime.Today.AddDays(toMonday(DateTime.Today.DayOfWeek)).AddDays(gWhichWeek * 7);
        string gSDate = dt.ToString("yyyy/MM/dd");
        string gEDate = dt.AddDays(gDateDiff - 1).ToString("yyyy/MM/dd");

        string[] gHDateRangeA = new string[gDateDiff];
        for (int i = 0; i < gDateDiff; i++)
        {
            gHDateRangeA[i] = dt.AddDays(i).ToString("yyyy/MM/dd");
        }

        //改用預存程序&設定HMember資料表的索引值
        //var dtMembers = SQLdatabase.ExecuteDataTable("EXEC MemberInfo @HID ='" + loginHID + "'");

        //AA20250408_ 判斷是否有抓過成員資料，避免重複執行耗效能
        System.Data.DataTable dtMembers = new System.Data.DataTable();
        System.Data.DataTable dtSessionMembers = Session["dtMembers"] as System.Data.DataTable;
        if (Session["dtMembers"] != null)
        {
            dtMembers = dtSessionMembers;
        }
        else
        {
            //dtMembers = SQLdatabase.ExecuteDataTable("EXEC MemberInfo @HID ='" + loginHID + "'");
            dtMembers = GetCachedMemberData(loginHID);
            Session["dtMembers"] = dtMembers; // 順手存進 Session
        }


        // 整合4表，僅查有資料的成員
        // 取得 MSSQL 資料
        var dtRecords = SQLdatabase.ExecuteDataTable(
            "SELECT a.HMemberID, LEFT(a.HCreateDT,10) AS HCreateDate, " +
            "  CASE WHEN c.HSCClassName='應用' THEN '應' WHEN c.HSCClassName='提問' THEN '問' ELSE LEFT(c.HSCClassName,1) END AS HSCClassName, '1' AS gType " +
            "FROM HSCGRMsg AS a LEFT JOIN HSCClass AS c ON a.HSCClassID=c.HID " +
            "WHERE LEFT(a.HCreateDT,10) BETWEEN '" + gSDate + "' AND '" + gEDate + "' AND a.HMemberID IN (SELECT HID FROM HMember WHERE HTeamID = (SELECT HTeamID FROM HMember WHERE HID = '" + loginHID + "' AND HStatus=1)) " +

            "UNION ALL " +

            "SELECT a.HMemberID, LEFT(a.HCreateDT,10), '回', '1' FROM HSCGRMsgResponse AS a " +
            "WHERE LEFT(a.HCreateDT,10) BETWEEN '" + gSDate + "' AND '" + gEDate + "' AND a.HMemberID IN (SELECT HID FROM HMember WHERE HTeamID = (SELECT HTeamID FROM HMember WHERE HID = '" + loginHID + "' AND HStatus=1)) " +

            "UNION ALL " +

            "SELECT a.HMemberID, LEFT(a.HCreateDT,10), '回', '2' FROM HSCTMsg AS a " +
            "WHERE LEFT(a.HCreateDT,10) BETWEEN '" + gSDate + "' AND '" + gEDate + "' AND a.HMemberID IN (SELECT HID FROM HMember WHERE HTeamID = (SELECT HTeamID FROM HMember WHERE HID = '" + loginHID + "' AND HStatus=1)) " +

            "UNION ALL " +

            "SELECT a.HMemberID, LEFT(a.HCreateDT,10), '回', '2' FROM HSCTMsgResponse AS a " +
            "WHERE LEFT(a.HCreateDT,10) BETWEEN '" + gSDate + "' AND '" + gEDate + "' AND a.HMemberID IN (SELECT HID FROM HMember WHERE HTeamID = (SELECT HTeamID FROM HMember WHERE HID = '" + loginHID + "' AND HStatus=1))"
        );



        //轉換EIP的Uid成十碼
        string gEIPUid = loginHEIPUid.PadLeft(10, '0');

        DateTime today = DateTime.Today;
        int diff = DayOfWeek.Monday - today.DayOfWeek;

        //DateTime startOfWeek = today.AddDays(diff);
        //DateTime endOfWeek = startOfWeek.AddDays(6);

        //改抓顯示的該週的星期一
        DateTime startOfWeek = dt;                 // dt 已是該週的星期一
        DateTime endOfWeek = dt.AddDays(6);      // 該週星期日

        string gEIPSDate = startOfWeek.ToString("yyyyMMdd") + "000000";
        string gEIPEDate = endOfWeek.ToString("yyyyMMdd") + "235959";

        // 撈 MySQL 資料
        var mysqlConn = new MySqlConnection(EIPconnStr);

        //只先抓自己的
        //var mysqlCmd = new MySqlCommand("SELECT gr_uid, gr_recorddate, gr_content1, gr_type FROM growthrecord WHERE gr_status < 9 AND gr_uid = '" + gEIPUid + "' AND gr_recorddate BETWEEN '" + gEIPSDate + "' AND '" + gEIPEDate + "' AND gr_uid='" + gEIPUid + "'", mysqlConn);



        var mysqlCmd = new MySqlCommand(@"
SELECT gr_uid, gr_recorddate, gr_content1, gr_type
FROM growthrecord
WHERE gr_status < 9
  AND gr_uid = @uid
  AND gr_recorddate >= @beg
  AND gr_recorddate <= @end;", mysqlConn);

        mysqlCmd.Parameters.Add("@uid", MySqlDbType.VarChar).Value = gEIPUid;     // 已經 PadLeft(10,'0')
        mysqlCmd.Parameters.Add("@beg", MySqlDbType.VarChar).Value = gEIPSDate;   // yyyyMMdd000000
        mysqlCmd.Parameters.Add("@end", MySqlDbType.VarChar).Value = gEIPEDate;   // yyyyMMdd235959

        var mysqlAdapter = new MySqlDataAdapter(mysqlCmd);
        System.Data.DataTable mysqlRecords = new System.Data.DataTable();
        mysqlAdapter.Fill(mysqlRecords);


        // 建立統一結構 DataTable（用 dtRecords 為藍本）
        System.Data.DataTable unifiedTable = dtRecords.Clone();  // 包含欄位型別設定，避免衝突
        unifiedTable.Columns["HSCClassName"].MaxLength = -1;

        // 匯入 MSSQL 資料
        foreach (DataRow row in dtRecords.Rows)
        {
            unifiedTable.ImportRow(row);
        }

        // 匯入 MySQL 資料（轉為統一結構）
        foreach (DataRow row in mysqlRecords.Rows)
        {
            DataRow newRow = unifiedTable.NewRow();


            // 轉型與錯誤容錯
            int memberId;
            if (!int.TryParse(row["gr_uid"].ToString(), out memberId)) continue;

            string rawDate = row["gr_recorddate"].ToString();
            string convertedDate = DateTime.ParseExact(rawDate.Substring(0, 8), "yyyyMMdd", null).ToString("yyyy/MM/dd");

            string gTypeName = row["gr_type"].ToString() == "1" ? "日 (EIP)" : row["gr_type"].ToString() == "2" ? "用 (EIP)" : row["gr_type"].ToString() == "3" ? "問 (EIP)" : row["gr_type"].ToString() == "4" ? "回 (EIP)" : row["gr_type"].ToString() == "5" ? "進 (EIP)" : row["gr_type"].ToString() == "6" ? "法 (EIP)" : "日 (EIP)";

            //newRow["HMemberID"] = memberId;
            newRow["HMemberID"] = gEIPUid;

            newRow["HCreateDate"] = convertedDate;
            newRow["HSCClassName"] = gTypeName;   // 自訂類型
            newRow["gType"] = row["gr_type"].ToString();            // 區分來源用
            //gr_type: 1=日、2=用、3=問、4=回、5=進、6=進、其他:日
            unifiedTable.Rows.Add(newRow);
        }





        //AA20250408_新增同修報名課程的出缺席紀錄
        #region 報名課程的出缺席紀錄
        var courseDict = new Dictionary<string, List<DataRow>>();

        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString))
        {
            SqlCommand cmd = new SqlCommand("GetWeekCourses", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@HID", SqlDbType.NVarChar).Value = loginHID;
            cmd.Parameters.Add("@WeekStart", SqlDbType.DateTime).Value = dt;
            cmd.Parameters.Add("@WeekEnd", SqlDbType.DateTime).Value = dt.AddDays(gDateDiff - 1);

            System.Data.DataTable dtCourse = new System.Data.DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dtCourse);

            foreach (DataRow row in dtCourse.Rows)
            {
                string memberID = row["HMemberID"].ToString();
                if (!courseDict.ContainsKey(memberID))
                    courseDict[memberID] = new List<DataRow>();
                courseDict[memberID].Add(row);
            }
        }

        #endregion



        var recordDict = new Dictionary<string, List<DataRow>>();
        //OLD
        //foreach (DataRow row in dtRecords.Rows)
        //{
        //    string key = row["HMemberID"].ToString();
        //    if (!recordDict.ContainsKey(key))
        //        recordDict[key] = new List<DataRow>();
        //    recordDict[key].Add(row);
        //}

        //改成抓新的datatable
        foreach (DataRow row in unifiedTable.Rows)
        {
            string key = "";
            if (row["HSCClassName"].ToString().Contains("EIP"))
            {
                key = row["HMemberID"].ToString();
                if (key == loginHEIPUid)
                {
                    if (!recordDict.ContainsKey(key))
                        recordDict[key] = new List<DataRow>();

                    recordDict[key].Add(row);
                }

            }
            else
            {
                key = row["HMemberID"].ToString();
                if (!recordDict.ContainsKey(key))
                    recordDict[key] = new List<DataRow>();

                recordDict[key].Add(row);
            }

        }

        TBL_HSCGRecord.Rows.Clear();




        TableRow headerRow = new TableRow();
        TableCell th1 = new TableCell();
        th1.Text = "大區/區屬/光團";
        th1.HorizontalAlign = HorizontalAlign.Center;
        th1.Font.Bold = true;
        th1.Font.Size = 10;
        th1.BorderWidth = 1;
        th1.Width = Unit.Percentage(8);
        headerRow.Cells.Add(th1);

        TableCell th2 = new TableCell();
        th2.Text = "期別/姓名/天命法位";
        th2.HorizontalAlign = HorizontalAlign.Center;
        th2.Font.Bold = true;
        th2.Font.Size = 10;
        th2.BorderWidth = 1;
        th2.Width = Unit.Percentage(12);
        headerRow.Cells.Add(th2);

        for (int i = 0; i < gHDateRangeA.Length; i++)
        {
            string date = gHDateRangeA[i];
            TableCell td = new TableCell();
            td.Text = Convert.ToDateTime(date).ToString("MM/dd") + " (" + ToChtWeek(Convert.ToDateTime(date).DayOfWeek.ToString()) + ")";
            td.HorizontalAlign = HorizontalAlign.Center;
            td.Font.Bold = true;
            td.BorderWidth = 1;

            if (ToChtWeek(Convert.ToDateTime(date).DayOfWeek.ToString()) == "六" || ToChtWeek(Convert.ToDateTime(date).DayOfWeek.ToString()) == "日")
            {
                td.Style.Add("color", "#d11");
            }
            if (date == DateTime.Today.ToString("yyyy/MM/dd"))
            {
                td.Style.Add("background-color", "#ffc8c8");
            }

            headerRow.Cells.Add(td);
        }
        TBL_HSCGRecord.Rows.Add(headerRow);

        foreach (DataRow member in dtMembers.Rows)
        {
            //加入uid
            string eipuid = member["HEIPUid"].ToString(); /*HEIPUid*/
            string hid = member["HID"].ToString();
            string area = member["LAreaName"].ToString() + "/" + member["AreaName"].ToString() + "<br/>" + member["TeamName"].ToString();
            string pname = member["HPeriod"] + " " + member["HUserName"];
            string rollcallhead = member["AreaName"].ToString() + "/" + member["HPeriod"] + " " + member["HUserName"];
            string hrName = member["HRName"].ToString();

            //Response.Write("Uid="+ eipuid);

            TableRow row = new TableRow();

            TableCell cell1 = new TableCell();
            cell1.Text = area;
            cell1.HorizontalAlign = HorizontalAlign.Center;
            cell1.Font.Size = 9;
            cell1.BorderWidth = 1;
            row.Cells.Add(cell1);

            TableCell cell2 = new TableCell();
            cell2.HorizontalAlign = HorizontalAlign.Center;
            cell2.Font.Size = 9;
            cell2.BorderWidth = 1;

            //LinkButton lnk = new LinkButton();
            //lnk.ID = "HQName" + hid;
            //lnk.Text = pname + "<br/>" + hrName;


            #region 統計
            int gTotalRecord = 0;  //總發表成長記錄次數
            //統計-歷年成長記錄篇數 (新版專欄)
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand("SELECT count(*) AS Num FROM HSCGRMsg WHERE HStatus=1 AND HMemberID=@HMemberID", con))
                {
                    cmd.Parameters.Add("@HMemberID", SqlDbType.NVarChar);

                    cmd.Parameters["@HMemberID"].Value = hid;

                    gTotalRecord = Convert.ToInt32(cmd.ExecuteScalar());
                }
                con.Close();
            }

            //EIP次數統計
            int gThisWeekNum = 0;
            int gGrowthReply = 0;
            int gLaoshireply = 0;

            using (var con = new MySqlConnection(EIPconnStr))
            {
                con.Open();
                string sql = @"
SELECT
    (SELECT COUNT(*) FROM growthrecord WHERE gr_status < 9 AND gr_uid = @gr_uid) AS growth_count,
    (SELECT COUNT(*) FROM laoshireply WHERE lr_status < 9 AND lr_uid = @gr_uid AND lr_parent = '') AS reply_count,
    (SELECT COUNT(*) FROM growthrecord 
     WHERE gr_status < 9 
       AND gr_uid = @gr_uid 
       AND gr_recorddate > @beg_date 
       AND gr_recorddate < @end_date) AS thisweek_count,
  (SELECT COUNT(*) FROM growthrecordreply WHERE grr_status<9 and grr_uid = @gr_uid) AS growthreply_count,
  (SELECT COUNT(*) FROM laoshireply WHERE lr_status < 9 AND lr_uid = @gr_uid AND  lr_parent<>'') AS laoshireply_count;
";

                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@gr_uid", MySqlDbType.VarChar).Value = eipuid.PadLeft(10, '0');
                    cmd.Parameters.Add("@beg_date", MySqlDbType.VarChar).Value = gEIPSDate;
                    cmd.Parameters.Add("@end_date", MySqlDbType.VarChar).Value = gEIPEDate;

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            gTotalRecord += Convert.ToInt32(reader["growth_count"]);
                            gTotalRecord += Convert.ToInt32(reader["reply_count"]);
                            gThisWeekNum = Convert.ToInt32(reader["thisweek_count"]);
                            gGrowthReply = Convert.ToInt32(reader["growthreply_count"]);
                            gLaoshireply = Convert.ToInt32(reader["laoshireply_count"]);
                        }
                    }
                }

                con.Close();
            }



            #endregion


            var lnk = new LinkButton { ID = "HQName" + hid, Text = pname + hrName + "<br/>" + gTotalRecord + "-" + gThisWeekNum + "<br/>" + (gGrowthReply + gLaoshireply) + "-0" };

            lnk.Attributes.Add("data-toggle", "modal");
            lnk.Attributes.Add("data-target", "#OrderInfo");
            lnk.Attributes.Add("onclick", "ShowOrderModalByID('" + hid + "','" + rollcallhead + "'); return false;");
            lnk.Attributes.Add("style", "line-height:normal");
            //lnk.Attributes.Add("data-studentinfo", rollcallhead);
            cell2.Controls.Add(lnk);
            row.Cells.Add(cell2);


            // 加入出缺席資料
            string courseInfo = "<table class='table table-bordered table-striped booking-table'><thead><tr><th>課程名稱</th><th class='text-center'>應出席次數</th><th class='text-center'>實際出席次數</th><th class='text-center'>請假次數</th><th class='text-center'>遲到次數</th><th class='text-center'>缺席次數</th></tr></thead><tbody>";

            if (courseDict.ContainsKey(hid))
            {
                foreach (var course in courseDict[hid])
                {
                    string cName = course["HCourseName"].ToString();

                    //string attend = course["HAttend"].ToString() == "1" ? "出席" : "缺席";
                    courseInfo += "<tr><td>" + cName + "</td><td></td><td></td><td></td><td></td><td></td></tr>";
                }
            }

            //Response.Write("A="+ courseInfo);

            string modalHtml = courseInfo + "</tbody></table>";
            string safeModalHtml = modalHtml.Replace("'", "\\'").Replace(Environment.NewLine, "").Replace("\r", "").Replace("\n", "");

            //lnk.Attributes.Add("onclick", "ShowOrderModal('" + hid + "','" + safeModalHtml + "'); return false;");
            //根據 hid 呼叫 $.ajax() 從後端載入 modal 內容
            lnk.Attributes.Add("onclick", "ShowOrderModalByID('" + hid + "','" + rollcallhead + "'); return false;");


            for (int i = 0; i < gHDateRangeA.Length; i++)
            {
                string date = gHDateRangeA[i];
                TableCell cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.BorderWidth = 1;

                //EDU大愛光老師專欄紀錄

                //Add(確認資料正確)
                //foreach (var kvp in recordDict)
                //{
                //    string key = kvp.Key;

                //    foreach (DataRow kvprow in kvp.Value)
                //    {
                //        foreach (DataColumn col in kvprow.Table.Columns)
                //        {
                //            string colName = col.ColumnName;
                //            string value = kvprow[colName].ToString();
                //            Response.Write(colName + ": " + value + "<br/>");
                //        }
                //        Response.Write("<hr/>");
                //    }
                //}




                //Response.Write("P="+ recordDict.ContainsKey(hid)+"；Q="+ recordDict.ContainsKey(eipuid));


                if (recordDict.ContainsKey(hid))
                {

                    foreach (DataRow rec in recordDict[hid])
                    {
                        if (rec["HCreateDate"].ToString() == date)
                        {
                            string gType = rec["gType"].ToString();
                            string title = rec["HSCClassName"].ToString();
                            //string color = gType == "1" ? "#e0a1db" : "#75c8d6";
                            string color = gType == "1" ? "#e0a1db" : gType == "2" ? "#e0a1db" : gType == "3" ? "#e0a1db" : gType == "4" ? "#75c8d6" : gType == "5" ? "#8f4898" : "#f5b247";
                            string html = "<a style='display:block;width:100%;background: " + color + ";border-radius: 5px; color:#fff; padding-left: 5px;text-align:left;margin-bottom:3px;' href=\"javascript:ShowModalContent('" + gType + "','" + date + "','" + hid + "','<h6>" + area + "</h6><h4 class=font-weight-bold>" + pname + "</h4>');\">" + title + "</a>";

                            cell.Controls.Add(new Literal { Text = html });
                            row.Cells.Add(cell);

                        }
                    }
                }


                //Response.Write("A="+ eipuid.PadLeft(10, '0')+"<br/>");

                //加入EIP的紀錄
                if (recordDict.ContainsKey(eipuid))
                {

                    foreach (DataRow rec in recordDict[eipuid])
                    {
                        //Response.Write("CDate=" + rec["HCreateDate"].ToString() + "<br/>");

                        if (rec["HCreateDate"].ToString() == date)
                        {
                            //Response.Write("HSCClassName=" + rec["HSCClassName"].ToString()+"<br/>");

                            string gType = rec["gType"].ToString();
                            string title = rec["HSCClassName"].ToString();
                            string color = gType == "1" ? "#e0a1db" : "#75c8d6";
                            //string html = "<a style='display:block;width:100%;background: " + color + ";border-radius: 5px; color:#fff; padding-left: 5px;text-align:left;margin-bottom:3px;' href=\"javascript:ShowModalContent('" + gType + "','" + date + "','" + hid + "','<h6>" + area + "</h6><h4 class=font-weight-bold>" + pname + "</h4>');\">" + title + "</a>";

                            string html = "<a style='display:block;width:100%;background: " + color + ";border-radius: 5px; color:#fff; padding-left: 5px;text-align:left;margin-bottom:3px;'  href=\"javascript:ShowModalEIPContent('" + gType + "','" + date + "','" + eipuid.PadLeft(10, '0') + "','<h6>" + area + "</h6><h4 class=font-weight-bold>" + pname + "</h4>');\">" + title + "</a>";

                            cell.Controls.Add(new Literal { Text = html });
                            row.Cells.Add(cell);
                        }
                    }
                }


                //補剩餘的cell
                row.Cells.Add(cell);
            }

            TBL_HSCGRecord.Rows.Add(row);
        }




    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        //隱藏master的熱門標籤
        ((HtmlControl)Master.FindControl("Div_Right")).Visible = false;




        #region 選單初始化
        if (!IsPostBack)
        {
            RegScript();//執行js

            //AA20250408_先將 dtMembers 暫存在 Session
            string loginHID = ((Label)Master.FindControl("LB_HUserHID")).Text;
            HF_EIPUid.Value = !string.IsNullOrEmpty(((TextBox)Master.FindControl("TB_EIPUid")).Text) ? ((TextBox)Master.FindControl("TB_EIPUid")).Text : "0";

            Session["dtMembers"] = SQLdatabase.ExecuteDataTable("EXEC MemberInfo @HID ='" + loginHID + "'");

            ShowITable(Convert.ToInt32(LB_WhichWeek.Text));

            //EA20240709_選單引用：App_Code/SelectListItem/RBL_ListItem.cs
            RBL_HSCJiugonggeTypeID.DataSource = RBL_ListItem.HSCJiugonggeType.ListItem;
            RBL_HSCJiugonggeTypeID.DataTextField = "Value";
            RBL_HSCJiugonggeTypeID.DataValueField = "Key";
            RBL_HSCJiugonggeTypeID.DataBind();





        }
        #endregion



        if (IsPostBack)
        {

            //預先判斷由那一個按鈕所按下
            if (this.Request.Form["__EVENTTARGET"] != null)
            {
                string gEVENTTARGET = this.Request.Form["__EVENTTARGET"];
                int gIndex = 0;
                //找最後一個$出現的位置(（)實際控制項 ID)
                gIndex = gEVENTTARGET.LastIndexOf("$");
                //string gBtnID = gEVENTTARGET.Substring(gIndex + 1, gEVENTTARGET.Length - (gIndex + 1));

                string btnID = gEVENTTARGET.Substring(gEVENTTARGET.LastIndexOf("$") + 1);

                switch (btnID)
                {
                    case "LBtn_LastWeek":
                        // 處理上一週邏輯
                        LB_WhichWeek.Text = (Convert.ToInt32(LB_WhichWeek.Text) - 1).ToString();

                        //如果有搜尋條件，則須保留
                        if (Session["Search_Active"] != null)
                        {
                            DateTime dt = DateTime.Today.AddDays(toMonday(DateTime.Today.DayOfWeek)).AddDays(Convert.ToInt32(LB_WhichWeek.Text) * 7);
                            TB_DateSearch.Text = dt.ToString("yyyy/MM/dd");

                            //執行搜尋顯示（不需觸發 button event）
                            LBtn_Search_Click(null, null);
                        }
                        else
                        {
                            ShowITable(Convert.ToInt32(LB_WhichWeek.Text));
                        }

                        break;

                    case "LBtn_NextWeek":
                        // 處理下一週邏輯
                        LB_WhichWeek.Text = (Convert.ToInt32(LB_WhichWeek.Text) + 1).ToString();

                        //如果有搜尋條件，則須保留
                        if (Session["Search_Active"] != null)
                        {
                            DateTime dt = DateTime.Today.AddDays(toMonday(DateTime.Today.DayOfWeek)).AddDays(Convert.ToInt32(LB_WhichWeek.Text) * 7);
                            TB_DateSearch.Text = dt.ToString("yyyy/MM/dd");


                            //執行搜尋顯示（不需觸發 button event）
                            LBtn_Search_Click(null, null);
                        }
                        else
                        {
                            ShowITable(Convert.ToInt32(LB_WhichWeek.Text));
                        }

                        break;

                    //新增成長紀錄儲存按鈕
                    case "Btn_Submit":
                        if (string.IsNullOrEmpty(TB_HTopicName.Text.Trim()) || DDL_HSCClass.SelectedValue == "0")
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入*必填資料');", true);
                            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
                            return;
                        }



                        SqlCommand cmd = new SqlCommand("INSERT INTO [HSCGRMsg] (HMemberID, HTopicName, HCourseID, HSCClassID, HSCRecordTypeID, HSCJiugonggeTypeID, HGProgress, HOGProgress, HContent, HFile1, HFile2, HFile3, HVideoLink, HHashTag, HOpenObject, HNotifyMentor, HStatus, HCreate, HCreateDT ) VALUES (@HMemberID, @HTopicName, @HCourseID, @HSCClassID, @HSCRecordTypeID, @HSCJiugonggeTypeID, @HGProgress, @HOGProgress, @HContent, @HFile1, @HFile2, @HFile3, @HVideoLink, @HHashTag, @HOpenObject, @HNotifyMentor, @HStatus, @HCreate, @HCreateDT)", ConStr);
                        ConStr.Open();
                        cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        cmd.Parameters.AddWithValue("@HTopicName", TB_HTopicName.Text.Trim());
                        cmd.Parameters.AddWithValue("@HCourseID", DDL_HCourseName.SelectedValue);
                        cmd.Parameters.AddWithValue("@HSCClassID", DDL_HSCClass.SelectedValue);
                        cmd.Parameters.AddWithValue("@HSCRecordTypeID", DDL_HSCRecordType.SelectedValue);
                        cmd.Parameters.AddWithValue("@HSCJiugonggeTypeID", RBL_HSCJiugonggeTypeID.SelectedValue);
                        cmd.Parameters.AddWithValue("@HGProgress", DDL_HGProgress.SelectedValue);
                        cmd.Parameters.AddWithValue("@HOGProgress", TB_HOGProgress.Text.Trim());
                        cmd.Parameters.AddWithValue("@HContent", CKE_HContent.Text.Trim());
                        cmd.Parameters.AddWithValue("@HFile1", LB_HFile1.Text.Trim());
                        cmd.Parameters.AddWithValue("@HFile2", LB_HFile2.Text.Trim());
                        cmd.Parameters.AddWithValue("@HFile3", LB_HFile3.Text.Trim());
                        cmd.Parameters.AddWithValue("@HVideoLink", TB_HVideoLink.Text.Trim());
                        cmd.Parameters.AddWithValue("@HHashTag", TB_HHashTag.Text.Trim());
                        cmd.Parameters.AddWithValue("@HOpenObject", RBL_HOpenObject.SelectedValue);
                        cmd.Parameters.AddWithValue("@HNotifyMentor", CB_HNotifyMentor.Checked);
                        cmd.Parameters.AddWithValue("@HStatus", 1);
                        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                        cmd.ExecuteNonQuery();
                        ConStr.Close();
                        cmd.Cancel();


                        //判斷是否要同步到大愛光老師專欄
                        if (DDL_HSCTopic.SelectedValue != "0")
                        {
                            cmd = new SqlCommand("INSERT INTO HSCTMsg (HMemberID, HSCTopicID, HContent, HFile1, HVideoLink, HHashTag,  HStatus, HCreate, HCreateDT ) VALUES ( @HMemberID, @HSCTopicID, @HContent,  @HFile1, @HVideoLink, @HHashTag, @HStatus, @HCreate, @HCreateDT)", ConStr);
                            cmd.CommandTimeout = 30;  // 增加查詢超時秒數

                            cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                            cmd.Parameters.AddWithValue("@HSCTopicID", DDL_HSCTopic.SelectedValue);
                            cmd.Parameters.AddWithValue("@HContent", CKE_HContent.Text.Trim());
                            cmd.Parameters.AddWithValue("@HFile1", LB_HFile1.Text.Trim());
                            cmd.Parameters.AddWithValue("@HVideoLink", TB_HVideoLink.Text.Trim());
                            cmd.Parameters.AddWithValue("@HHashTag", TB_HHashTag.Text.Trim());
                            cmd.Parameters.AddWithValue("@HStatus", 1);
                            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                            cmd.ExecuteNonQuery();
                            ConStr.Close();
                            cmd.Cancel();
                        }

                        #region 清除資料
                        TB_HTopicName.Text = null;
                        DDL_HCourseName.SelectedValue = "0";
                        DDL_HSCClass.SelectedValue = "0";
                        DDL_HSCRecordType.SelectedValue = "0";
                        DDL_HGProgress.SelectedValue = "0";
                        TB_HOGProgress.Text = null;
                        CKE_HContent.Text = null;
                        LB_HFile1.Text = null;
                        LB_HFile2.Text = null;
                        LB_HFile3.Text = null;
                        TB_HVideoLink.Text = null;
                        TB_HHashTag.Text = null;
                        #endregion

                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('新增成功!');", true);


                        ShowITable(Convert.ToInt32(LB_WhichWeek.Text));


                        break;

                }




            }


        }



    }


    #region 上傳檔案功能
    protected void LBtn_HFile1_Click(object sender, EventArgs e)
    {

        if (string.IsNullOrEmpty(TB_HTopicName.Text))
        {
            LB_HFileMsg1.Text = "請輸入主題名稱";
            LB_HFileMsg1.ForeColor = Color.Red;
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
            return;
        }


        //判斷檔案路徑是否存在，不存在則建立資料夾 
        if (!Directory.Exists(Server.MapPath("~/uploads/HSCGRecord/" + DateTime.Now.ToString("yyyyMMdd"))))
        {
            Directory.CreateDirectory(Server.MapPath("~/uploads/HSCGRecord/" + DateTime.Now.ToString("yyyyMMdd")));//不存在就建立目錄 
        }

        string gUPFile = "~/uploads/HSCGRecord/" + DateTime.Now.ToString("yyyyMMdd") + "/";
        bool FileIsValidUP1 = false;

        if (FU_HFile1.HasFile)
        {
            //取得上傳檔案大小
            int fileSize = FU_HFile1.PostedFile.ContentLength;
            if (fileSize > 200 * 1024 * 1024) //200MB
            {
                LB_HFileMsg1.Text = "檔案上限為200MB";
                LB_HFileMsg1.ForeColor = Color.Red;
            }
            else
            {

                string gUPFileMain = Path.GetFileNameWithoutExtension(FU_HFile1.FileName);//取得上傳檔案的主檔名(沒有副檔名)
                string gUPFileExtension = System.IO.Path.GetExtension(FU_HFile1.FileName).ToLower(); //取得上傳檔案的副檔名

                String[] restrictExtension = { ".jpg", ".jpeg", ".png", ".gif", ".heic", ".heif", ".mp3", ".pdf", ".docx", ".doc", ".xlsx", ".xls" };
                foreach (string i in restrictExtension)
                {
                    if (gUPFileExtension == i)
                    {
                        FileIsValidUP1 = true;
                        break;
                    }
                }

                if (FileIsValidUP1)
                {
                    //全檔名
                    LB_HFile1.Text = TB_HTopicName.Text.Trim() + "_1_" + gUPFileMain + "_" + DateTime.Now.ToString("yyMMddssff") + gUPFileExtension;


                    string saveFileName = TB_HTopicName.Text.Trim() + "_1_" + gUPFileMain + "_" + DateTime.Now.ToString("yyMMddssff");



                    string savePath = Server.MapPath(gUPFile) + saveFileName + gUPFileExtension;
                    FU_HFile1.SaveAs(savePath);
                    LB_HFile1.Text = saveFileName + gUPFileExtension;



                    LB_HFileMsg1.Text = "上傳成功";
                    LB_HFileMsg1.ForeColor = Color.Green;
                    LBtn_HFile1_Del.Visible = true;

                }
                else
                {
                    LB_HFileMsg1.Text = "上傳檔案格式錯誤";
                    LB_HFileMsg1.ForeColor = Color.Red;

                }


            }
        }
        else
        {
            LB_HFileMsg1.Text = "請選擇上傳檔案";
            LB_HFileMsg1.ForeColor = Color.Red;

        }

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
        return;

    }

    protected void LBtn_HFile2_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_HTopicName.Text))
        {
            LB_HFileMsg2.Text = "請輸入主題名稱";
            LB_HFileMsg2.ForeColor = Color.Red;
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
            return;
        }


        //判斷檔案路徑是否存在，不存在則建立資料夾 
        if (!Directory.Exists(Server.MapPath("~/uploads/HSCGRecord/" + DateTime.Now.ToString("yyyyMMdd"))))
        {
            Directory.CreateDirectory(Server.MapPath("~/uploads/HSCGRecord/" + DateTime.Now.ToString("yyyyMMdd")));//不存在就建立目錄 
        }

        string gUPFile = "~/uploads/HSCGRecord/" + DateTime.Now.ToString("yyyyMMdd") + "/";
        bool FileIsValidUP2 = false;

        if (FU_HFile2.HasFile)
        {
            //取得上傳檔案大小
            int fileSize = FU_HFile2.PostedFile.ContentLength;
            if (fileSize > 200 * 1024 * 1024) //200MB
            {
                LB_HFileMsg2.Text = "檔案上限為200MB";
                LB_HFileMsg2.ForeColor = Color.Red;
            }
            else
            {

                string gUPFileMain = Path.GetFileNameWithoutExtension(FU_HFile2.FileName);//取得上傳檔案的主檔名(沒有副檔名)
                string gUPFileExtension = System.IO.Path.GetExtension(FU_HFile2.FileName).ToLower(); //取得上傳檔案的副檔名

                String[] restrictExtension = { ".jpg", ".jpeg", ".png", ".gif", ".heic", ".heif", ".mp3", ".pdf", ".docx", ".doc", ".xlsx", ".xls" };
                foreach (string i in restrictExtension)
                {
                    if (gUPFileExtension == i)
                    {
                        FileIsValidUP2 = true;
                        break;
                    }
                }

                if (FileIsValidUP2)
                {
                    //全檔名
                    LB_HFile2.Text = TB_HTopicName.Text.Trim() + "_2_" + gUPFileMain + "_" + DateTime.Now.ToString("yyMMddssff") + gUPFileExtension;
                    FU_HFile2.SaveAs(Server.MapPath(gUPFile) + LB_HFile2.Text);

                    LB_HFileMsg2.Text = "上傳成功";
                    LB_HFileMsg2.ForeColor = Color.Green;
                    LBtn_HFile2_Del.Visible = true;

                }
                else
                {
                    LB_HFileMsg2.Text = "上傳檔案格式錯誤";
                    LB_HFileMsg2.ForeColor = Color.Red;

                }


            }
        }
        else
        {
            LB_HFileMsg2.Text = "請選擇上傳檔案";
            LB_HFileMsg2.ForeColor = Color.Red;

        }

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
        return;
    }

    protected void LBtn_HFile3_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_HTopicName.Text))
        {
            LB_HFileMsg3.Text = "請輸入主題名稱";
            LB_HFileMsg3.ForeColor = Color.Red;
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
            return;
        }


        //判斷檔案路徑是否存在，不存在則建立資料夾 
        if (!Directory.Exists(Server.MapPath("~/uploads/HSCGRecord/" + DateTime.Now.ToString("yyyyMMdd"))))
        {
            Directory.CreateDirectory(Server.MapPath("~/uploads/HSCGRecord/" + DateTime.Now.ToString("yyyyMMdd")));//不存在就建立目錄 
        }

        string gUPFile = "~/uploads/HSCGRecord/" + DateTime.Now.ToString("yyyyMMdd") + "/";
        bool FileIsValidUP3 = false;

        if (FU_HFile3.HasFile)
        {
            //取得上傳檔案大小
            int fileSize = FU_HFile3.PostedFile.ContentLength;
            if (fileSize > 200 * 1024 * 1024) //200MB
            {
                LB_HFileMsg3.Text = "檔案上限為200MB";
                LB_HFileMsg3.ForeColor = Color.Red;
            }
            else
            {

                string gUPFileMain = Path.GetFileNameWithoutExtension(FU_HFile3.FileName);//取得上傳檔案的主檔名(沒有副檔名)
                string gUPFileExtension = System.IO.Path.GetExtension(FU_HFile3.FileName).ToLower(); //取得上傳檔案的副檔名

                String[] restrictExtension = { ".jpg", ".jpeg", ".png", ".gif", ".heic", ".heif", ".mp3", ".pdf", ".docx", ".doc", ".xlsx", ".xls" };
                foreach (string i in restrictExtension)
                {
                    if (gUPFileExtension == i)
                    {
                        FileIsValidUP3 = true;
                        break;
                    }
                }

                if (FileIsValidUP3)
                {
                    //全檔名
                    LB_HFile3.Text = TB_HTopicName.Text.Trim() + "_3_" + gUPFileMain + "_" + DateTime.Now.ToString("yyMMddssff") + gUPFileExtension;
                    FU_HFile3.SaveAs(Server.MapPath(gUPFile) + LB_HFile3.Text);

                    LB_HFileMsg3.Text = "上傳成功";
                    LB_HFileMsg3.ForeColor = Color.Green;
                    LBtn_HFile3_Del.Visible = true;

                }
                else
                {
                    LB_HFileMsg3.Text = "上傳檔案格式錯誤";
                    LB_HFileMsg3.ForeColor = Color.Red;

                }


            }
        }
        else
        {
            LB_HFileMsg3.Text = "請選擇上傳檔案";
            LB_HFileMsg3.ForeColor = Color.Red;

        }

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
        return;
    }

    #endregion

    #region 刪除已上傳檔案
    protected void LBtn_HFile1_Del_Click(object sender, EventArgs e)
    {
        string sourceFile = FileRoot + LB_HFile1.Text;

        if (File.Exists(sourceFile)) File.Delete(sourceFile);
        LB_HFile1.Text = null;
        LB_HFileMsg1.Text = null;
        LBtn_HFile1_Del.Visible = false;

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
        return;
    }

    protected void LBtn_HFile2_Del_Click(object sender, EventArgs e)
    {
        string sourceFile = FileRoot + LB_HFile2.Text;

        if (File.Exists(sourceFile)) File.Delete(sourceFile);
        LB_HFile2.Text = null;
        LB_HFileMsg2.Text = null;
        LBtn_HFile2_Del.Visible = false;

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
        return;
    }

    protected void LBtn_HFile3_Del_Click(object sender, EventArgs e)
    {
        string sourceFile = FileRoot + LB_HFile3.Text;

        if (File.Exists(sourceFile)) File.Delete(sourceFile);
        LB_HFile3.Text = null;
        LB_HFileMsg3.Text = null;
        LBtn_HFile3_Del.Visible = false;

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
        return;
    }
    #endregion

    #region 儲存功能
    //沒有用到
    protected void Btn_Submit_Click(object sender, EventArgs e)
    {
        //if (string.IsNullOrEmpty(TB_HTopicName.Text.Trim()))
        //{
        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入*必填資料');", true);
        //    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
        //    return;
        //}

        //SqlCommand cmd = new SqlCommand("INSERT INTO [HSCGRMsg] (HMemberID, HTopicName, HCourseID, HSCClassID, HSCRecordTypeID, HSCJiugonggeTypeID, HGProgress, HOGProgress, HContent, HFile1, HFile2, HFile3, HVideoLink, HHashTag, HOpenObject, HStatus, HCreate, HCreateDT ) VALUES ( @HMemberID, @HTopicName, @HCourseID, @HSCClassID, @HSCRecordTypeID, @HSCJiugonggeTypeID, @HGProgress, @HOGProgress, @HContent, @HFile1, @HFile2, @HFile3, @HVideoLink, @HHashTag, @HOpenObject, @HStatus, @HCreate, @HCreateDT)", ConStr);
        //cmd.CommandTimeout = 30;  // 設定查詢超時秒數
        //ConStr.Open();
        //cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        //cmd.Parameters.AddWithValue("@HTopicName", TB_HTopicName.Text.Trim());
        //cmd.Parameters.AddWithValue("@HCourseID", DDL_HCourseName.SelectedValue);
        //cmd.Parameters.AddWithValue("@HSCClassID", DDL_HSCClass.SelectedValue);
        //cmd.Parameters.AddWithValue("@HSCRecordTypeID", DDL_HSCRecordType.SelectedValue);
        //cmd.Parameters.AddWithValue("@HSCJiugonggeTypeID", RBL_HSCJiugonggeTypeID.SelectedValue);
        //cmd.Parameters.AddWithValue("@HGProgress", DDL_HGProgress.SelectedValue);
        //cmd.Parameters.AddWithValue("@HOGProgress", TB_HOGProgress.Text.Trim());
        //cmd.Parameters.AddWithValue("@HContent", CKE_HContent.Text.Trim());
        //cmd.Parameters.AddWithValue("@HFile1", LB_HFile1.Text.Trim());
        //cmd.Parameters.AddWithValue("@HFile2", LB_HFile2.Text.Trim());
        //cmd.Parameters.AddWithValue("@HFile3", LB_HFile3.Text.Trim());
        //cmd.Parameters.AddWithValue("@HVideoLink", TB_HVideoLink.Text.Trim());
        //cmd.Parameters.AddWithValue("@HHashTag", TB_HHashTag.Text.Trim());
        //cmd.Parameters.AddWithValue("@HOpenObject", RBL_HOpenObject.SelectedValue);
        //cmd.Parameters.AddWithValue("@HStatus", 1);
        //cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        //cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        //cmd.ExecuteNonQuery();
        //cmd.Cancel();

        //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('新增成功!');", true);/*window.location.href='HSCGRecord.aspx';*/


        ////判斷是否要同步到大愛光老師專欄
        //if (DDL_HSCTopic.SelectedValue !="0")
        //{
        //     cmd = new SqlCommand("INSERT INTO HSCTMsg (HMemberID, HSCTopicID, HContent,  HFile1, HVideoLink, HHashTag,  HStatus, HCreate, HCreateDT ) VALUES ( @HMemberID, @HSCTopicID, @HContent,  @HFile1, @HVideoLink, @HHashTag, @HStatus, @HCreate, @HCreateDT)", ConStr);
        //    cmd.CommandTimeout = 30;  // 增加查詢超時秒數

        //    cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        //    cmd.Parameters.AddWithValue("@HSCTopicID", DDL_HSCTopic.SelectedValue);
        //    cmd.Parameters.AddWithValue("@HContent", CKE_HContent.Text.Trim());
        //    cmd.Parameters.AddWithValue("@HFile1", LB_HFile1.Text.Trim());
        //    cmd.Parameters.AddWithValue("@HVideoLink", TB_HVideoLink.Text.Trim());
        //    cmd.Parameters.AddWithValue("@HHashTag", TB_HHashTag.Text.Trim());
        //    cmd.Parameters.AddWithValue("@HStatus", 1);
        //    cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        //    cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        //    cmd.ExecuteNonQuery();
        //    ConStr.Close();
        //    cmd.Cancel();
        //}


        //執行table
        //ShowITable(Convert.ToInt32(LB_WhichWeek.Text));

    }
    #endregion

    #region 關閉Modal功能
    protected void Btn_CloseModal_Click(object sender, EventArgs e)
    {
        //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "window.location.href='HSCGRecord.aspx';", true);
    }
    #endregion

    #region 課程下拉選單
    protected void DDL_HCourseName_SelectedIndexChanged(object sender, EventArgs e)
    {
        //有選擇課程=>開放對象要顯示"僅限班會成員"的選項
        if (DDL_HCourseName.SelectedValue != "0")
        {
            RBL_HOpenObject.Items[1].Enabled = true;
        }
        else
        {
            RBL_HOpenObject.Items[1].Enabled = false;
        }

    }
    #endregion

    #region 執行js
    public void RegScript()
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$('.js-example-basic-single').select2({closeOnSelect: true,});"), true);//執行js

    }
    #endregion


    private string ToChtWeek(string strWeek)
    {
        string strReturn = "";
        if (strWeek == "Monday")
            strReturn = "一";
        if (strWeek == "Tuesday")
            strReturn = "二";
        if (strWeek == "Wednesday")
            strReturn = "三";
        if (strWeek == "Thursday")
            strReturn = "四";
        if (strWeek == "Friday")
            strReturn = "五";
        else if (strWeek == "Saturday")
            strReturn = "六";
        else if (strWeek == "Sunday")
            strReturn = "日";
        return strReturn;
    }


    private static int toMonday(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday: return 0;
            case DayOfWeek.Tuesday: return -1;
            case DayOfWeek.Wednesday: return -2;
            case DayOfWeek.Thursday: return -3;
            case DayOfWeek.Friday: return -4;
            case DayOfWeek.Saturday: return -5;
            case DayOfWeek.Sunday: return -6;
            default: return 0;
        }
    }


    //AA20250408_封裝抓取邏輯
    private System.Data.DataTable GetCachedMemberData(string loginHID)
    {
        if (Session["dtMembers"] == null)
        {
            Session["dtMembers"] = SQLdatabase.ExecuteDataTable("EXEC MemberInfo @HID ='" + loginHID + "'");
        }
        return (System.Data.DataTable)Session["dtMembers"];
    }

    protected void Rpt_HSCCLassIcon_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (((Label)e.Item.FindControl("LB_HSCClassName")).Text == "提問")
        {
            ((Label)e.Item.FindControl("LB_SCClassFirstWord")).Text = "問";
        }

        if (((Label)e.Item.FindControl("LB_HSCClassName")).Text == "進度")
        {
            ((HtmlGenericControl)e.Item.FindControl("Div_Icon")).Attributes.Add("style", "background-color:#75c8d6");
        }

        if (((Label)e.Item.FindControl("LB_HSCClassName")).Text == "法脈結構進度")
        {
            ((HtmlGenericControl)e.Item.FindControl("Div_Icon")).Attributes.Add("style", "background-color:#8f4898");
        }
    }

    #region 搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {

        #region 搜尋多人
        string Select = @"
            SELECT  
                a.HID,HAreaID, a.HEIPUid,
                (d.HLArea + '/' + c.HArea) AS Area,
                HTeamID,
                (HPeriod + ' ' + HUserName) AS PName,
                e.HSystemName,
                b.HRName,
                a.HAxisType
            FROM HMember AS a
            LEFT JOIN HRole AS b ON a.HPositionID = b.HID
            LEFT JOIN HArea AS c ON a.HAreaID = c.HID
            LEFT JOIN HLArea AS d ON c.HLAreaID = d.HID
            LEFT JOIN HSystem AS e ON a.HSystemID = e.HID
        ";

        var whereClauses = new List<string>();
        var gHMemberIDs = new List<string>();
        var gMemberEIPUidMap = new Dictionary<string, string>();


        if (string.IsNullOrEmpty(TB_HUsernameSearch.Text.Trim()) && DDL_HAreaSearch.SelectedValue == "0" && DDL_HSystemSearch.SelectedValue == "0" && DDL_HAxisTypeSearch.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMsg", "alert('請至少選擇兩項搜尋條件，不能只搜尋日期~謝謝~');", true);
            return;
        }

        using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString))
        {
            conn.Open();

            if (!string.IsNullOrEmpty(TB_HUsernameSearch.Text.Trim()))
                whereClauses.Add("a.HUserName LIKE '%" + TB_HUsernameSearch.Text.Trim() + "%'");
            if (DDL_HAreaSearch.SelectedValue != "0")
                whereClauses.Add("c.HID = '" + DDL_HAreaSearch.SelectedValue + "'");
            if (DDL_HSystemSearch.SelectedValue != "0")
                whereClauses.Add("e.HID = '" + DDL_HSystemSearch.SelectedValue + "'");
            if (DDL_HAxisTypeSearch.SelectedValue != "0")
                whereClauses.Add("a.HAxisType = '" + DDL_HAxisTypeSearch.SelectedValue + "'");

            if (whereClauses.Count > 0)
                Select += " WHERE a.HStatus=1 AND " + string.Join(" AND ", whereClauses.ToArray());

            using (var cmd = new SqlCommand(Select, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var hid = reader["HID"].ToString();
                    var eip = reader["HEIPUid"].ToString().PadLeft(10, '0');
                    if (!gHMemberIDs.Contains(hid))
                    {
                        gHMemberIDs.Add(hid);
                        gMemberEIPUidMap[hid] = eip;
                    }
                }
            }
            conn.Close();
        }

        var joinedIDs = "'" + string.Join("','", gHMemberIDs.ToArray()) + "'";


        string gSDate = null;
        string gEDate = null;


        var dt = string.IsNullOrEmpty(TB_DateSearch.Text.Trim()) ? DateTime.Today : DateTime.Parse(TB_DateSearch.Text.Trim());
        var monday = dt.AddDays(-(int)dt.DayOfWeek + (dt.DayOfWeek == DayOfWeek.Sunday ? -6 : 1));
        var sunday = monday.AddDays(6);
        gSDate = monday.ToString("yyyy/MM/dd");
        gEDate = sunday.ToString("yyyy/MM/dd");


        //統整成長紀錄、專欄回應
        var dtRecords = SQLdatabase.ExecuteDataTable(
            string.Format(@"
        SELECT a.HMemberID, LEFT(a.HCreateDT,10) AS HCreateDate,
        CASE WHEN c.HSCClassName='應用' THEN '應' WHEN c.HSCClassName='提問' THEN '問' ELSE LEFT(c.HSCClassName,1) END AS HSCClassName, '1' AS gType
        FROM HSCGRMsg AS a LEFT JOIN HSCClass AS c ON a.HSCClassID=c.HID
        WHERE LEFT(a.HCreateDT,10) BETWEEN '{0}' AND '{1}' AND a.HMemberID IN ({2})
        UNION ALL
        SELECT a.HMemberID, LEFT(a.HCreateDT,10), '回', '1' FROM HSCGRMsgResponse AS a
        WHERE LEFT(a.HCreateDT,10) BETWEEN '{0}' AND '{1}' AND a.HMemberID IN ({2})
        UNION ALL
        SELECT a.HMemberID, LEFT(a.HCreateDT,10), '回', '2' FROM HSCTMsg AS a
        WHERE LEFT(a.HCreateDT,10) BETWEEN '{0}' AND '{1}' AND a.HMemberID IN ({2})
        UNION ALL
        SELECT a.HMemberID, LEFT(a.HCreateDT,10), '回', '2' FROM HSCTMsgResponse AS a
        WHERE LEFT(a.HCreateDT,10) BETWEEN '{0}' AND '{1}' AND a.HMemberID IN ({2})",
                gSDate, gEDate, joinedIDs)
        );

        var gEIPSDate = Convert.ToDateTime(gSDate).ToString("yyyyMMdd") + "000000";
        var gEIPEDate = Convert.ToDateTime(gEDate).ToString("yyyyMMdd") + "235959";
        var eipInClause = "'" + string.Join("','", gMemberEIPUidMap.Values.ToArray()) + "'";

        var mysqlConn = new MySqlConnection(EIPconnStr);
        var mysqlCmd = new MySqlCommand(
            string.Format("SELECT gr_uid, gr_recorddate, gr_content1, gr_type FROM growthrecord WHERE gr_status < 9 AND gr_uid IN ({0}) AND gr_recorddate BETWEEN '{1}' AND '{2}'", eipInClause, gEIPSDate, gEIPEDate),
            mysqlConn
        );
        var mysqlAdapter = new MySqlDataAdapter(mysqlCmd);
        var mysqlRecords = new System.Data.DataTable();
        mysqlAdapter.Fill(mysqlRecords);

        var unifiedTable = dtRecords.Clone();
        unifiedTable.Columns["HSCClassName"].MaxLength = -1;

        //EDU資料
        foreach (DataRow row in dtRecords.Rows)
            unifiedTable.ImportRow(row);

        //EIP資料
        foreach (DataRow row in mysqlRecords.Rows)
        {
            var newRow = unifiedTable.NewRow();
            newRow["HMemberID"] = row["gr_uid"].ToString();
            newRow["HCreateDate"] = DateTime.ParseExact(row["gr_recorddate"].ToString().Substring(0, 8), "yyyyMMdd", null).ToString("yyyy/MM/dd");
            var gType = row["gr_type"].ToString();
            var gTypeName = "日 (EIP)";
            if (gType == "2") gTypeName = "用 (EIP)";
            else if (gType == "3") gTypeName = "問 (EIP)";
            else if (gType == "4") gTypeName = "回 (EIP)";
            else if (gType == "5" || gType == "6") gTypeName = "進 (EIP)";
            newRow["HSCClassName"] = gTypeName;
            newRow["gType"] = gType;
            unifiedTable.Rows.Add(newRow);
        }

        //成長紀錄
        var recordDict = new Dictionary<string, List<DataRow>>();
        foreach (DataRow row in unifiedTable.Rows)
        {
            var memberID = row["HMemberID"].ToString();
            var className = row["HSCClassName"].ToString();

            string key = "";

            if (className.Contains("EIP"))
            {
                //將前面0去除
                string gEIPUid = Convert.ToInt32(memberID).ToString();
                var match = gMemberEIPUidMap.FirstOrDefault(kvp => kvp.Key == gEIPUid);
                key = match.Key ?? gEIPUid;
            }
            else
            {
                key = memberID;
            }

            if (!recordDict.ContainsKey(key))
                recordDict[key] = new List<DataRow>();

            recordDict[key].Add(row);
        }


        // 報名課程的出缺席紀錄
        #region
        var courseDict = new Dictionary<string, List<DataRow>>();
        using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString))
        {
            con.Open();
            using (var cmd = new SqlCommand("GetWeekCourses", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@HID", SqlDbType.NVarChar);
                cmd.Parameters.Add("@WeekStart", SqlDbType.DateTime);
                cmd.Parameters.Add("@WeekEnd", SqlDbType.DateTime);

                foreach (var hid in gHMemberIDs)
                {
                    cmd.Parameters["@HID"].Value = hid;
                    cmd.Parameters["@WeekStart"].Value = Convert.ToDateTime(gSDate);
                    cmd.Parameters["@WeekEnd"].Value = Convert.ToDateTime(gEDate);

                    var dtCourse = new System.Data.DataTable();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dtCourse);
                    }

                    foreach (DataRow row in dtCourse.Rows)
                    {
                        var memberID = row["HMemberID"].ToString();
                        if (!courseDict.ContainsKey(memberID))
                            courseDict[memberID] = new List<DataRow>();
                        courseDict[memberID].Add(row);
                    }
                }
            }
            con.Close();
        }
        #endregion


        TBL_HSCGRecord.Rows.Clear();

        // 建立表頭
        var headerRow = new TableRow();
        var th1 = new TableCell { Text = "大區/區屬/光團", HorizontalAlign = HorizontalAlign.Center, Font = { Bold = true, Size = 10 }, BorderWidth = 1, Width = Unit.Percentage(8) };
        var th2 = new TableCell { Text = "期別/姓名/天命法位", HorizontalAlign = HorizontalAlign.Center, Font = { Bold = true, Size = 10 }, BorderWidth = 1, Width = Unit.Percentage(12) };
        headerRow.Cells.Add(th1);
        headerRow.Cells.Add(th2);

        var gHDateRangeA = new string[7];
        for (int i = 0; i < 7; i++)
        {
            var date = Convert.ToDateTime(gSDate).AddDays(i);
            gHDateRangeA[i] = date.ToString("yyyy/MM/dd");
            var td = new TableCell
            {
                Text = date.ToString("MM/dd") + " (" + ToChtWeek(date.DayOfWeek.ToString()) + ")",
                HorizontalAlign = HorizontalAlign.Center,
                Font = { Bold = true },
                BorderWidth = 1
            };
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                td.Style.Add("color", "#d11");
            if (date.Date == DateTime.Today)
                td.Style.Add("background-color", "#ffc8c8");

            headerRow.Cells.Add(td);
        }
        TBL_HSCGRecord.Rows.Add(headerRow);

        foreach (var hid in gHMemberIDs)
        {
            var eipuid = Convert.ToInt32(gMemberEIPUidMap[hid]).ToString();

            //Response.Write("uid="+ eipuid+"<br/>");
            //Response.End();
            var dtMemberInfo = SQLdatabase.ExecuteDataTable("EXEC SearchMemberInfo_SCGRecord @HID = '" + hid + "'");
            if (dtMemberInfo.Rows.Count == 0) continue;
            var member = dtMemberInfo.Rows[0];

            var area = member["LAreaName"] + "/" + member["AreaName"] + "<br/>" + member["TeamName"];
            var pname = member["HPeriod"] + " " + member["HUserName"];
            var rollcallhead = member["AreaName"] + "/" + pname;
            var hrName = member["HRName"].ToString();
            int gTotalRecord = 0;  //總發表成長記錄次數

            var row = new TableRow();
            row.Cells.Add(new TableCell { Text = area, HorizontalAlign = HorizontalAlign.Center, Font = { Size = 9 }, BorderWidth = 1 });

            #region 統計
            //統計-歷年成長記錄篇數 (新版專欄)
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand("SELECT count(*) AS Num FROM HSCGRMsg WHERE HStatus=1 AND HMemberID=@HMemberID", con))
                {
                    cmd.Parameters.Add("@HMemberID", SqlDbType.NVarChar);

                    cmd.Parameters["@HMemberID"].Value = hid;

                    gTotalRecord = Convert.ToInt32(cmd.ExecuteScalar());
                }
                con.Close();
            }

            //EIP次數統計
            int gThisWeekNum = 0;
            int gGrowthReply = 0;
            int gLaoshireply = 0;

            using (var con = new MySqlConnection(EIPconnStr))
            {
                con.Open();
                string sql = @"
SELECT
    (SELECT COUNT(*) FROM growthrecord WHERE gr_status < 9 AND gr_uid = @gr_uid) AS growth_count,
    (SELECT COUNT(*) FROM laoshireply WHERE lr_status < 9 AND lr_uid = @gr_uid AND lr_parent = '') AS reply_count,
    (SELECT COUNT(*) FROM growthrecord 
     WHERE gr_status < 9 
       AND gr_uid = @gr_uid 
       AND gr_recorddate > @beg_date 
       AND gr_recorddate < @end_date) AS thisweek_count,
  (SELECT COUNT(*) FROM growthrecordreply WHERE grr_status<9 and grr_uid = @gr_uid) AS growthreply_count,
  (SELECT COUNT(*) FROM laoshireply WHERE lr_status < 9 AND lr_uid = @gr_uid AND  lr_parent<>'') AS laoshireply_count;
";

                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@gr_uid", MySqlDbType.VarChar).Value = eipuid.PadLeft(10, '0');
                    cmd.Parameters.Add("@beg_date", MySqlDbType.VarChar).Value = gEIPSDate;
                    cmd.Parameters.Add("@end_date", MySqlDbType.VarChar).Value = gEIPEDate;

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            gTotalRecord += Convert.ToInt32(reader["growth_count"]);
                            gTotalRecord += Convert.ToInt32(reader["reply_count"]);
                            gThisWeekNum = Convert.ToInt32(reader["thisweek_count"]);
                            gGrowthReply = Convert.ToInt32(reader["growthreply_count"]);
                            gLaoshireply = Convert.ToInt32(reader["laoshireply_count"]);
                        }
                    }
                }

                con.Close();
            }



            #endregion
            var cell2 = new TableCell { HorizontalAlign = HorizontalAlign.Center, Font = { Size = 9 }, BorderWidth = 1 };
            //AE20250728_新增統計次數
            var lnk = new LinkButton { ID = "HQName" + hid, Text = pname + hrName + "<br/>" + gTotalRecord + "-" + gThisWeekNum + "<br/>" + (gGrowthReply + gLaoshireply) + "-0" };
            lnk.Attributes.Add("data-toggle", "modal");
            lnk.Attributes.Add("data-target", "#OrderInfo");
            lnk.Attributes.Add("onclick", "ShowOrderModalByID('" + hid + "','" + rollcallhead + "'); return false;");
            lnk.Attributes.Add("style", "line-height:normal");
            cell2.Controls.Add(lnk);
            row.Cells.Add(cell2);


            #region 加入出缺席資料
            // 加入出缺席資料
            string courseInfo = "<table class='table table-bordered table-striped booking-table'><thead><tr><th>課程名稱</th><th class='text-center'>應出席次數</th><th class='text-center'>實際出席次數</th><th class='text-center'>請假次數</th><th class='text-center'>遲到次數</th><th class='text-center'>缺席次數</th></tr></thead><tbody>";

            if (courseDict.ContainsKey(hid))
            {
                foreach (var course in courseDict[hid])
                {
                    string cName = course["HCourseName"].ToString();

                    //string attend = course["HAttend"].ToString() == "1" ? "出席" : "缺席";
                    courseInfo += "<tr><td>" + cName + "</td><td></td><td></td><td></td><td></td><td></td></tr>";
                }
            }

            //Response.Write("A="+ courseInfo);

            string modalHtml = courseInfo + "</tbody></table>";
            string safeModalHtml = modalHtml.Replace("'", "\\'").Replace(Environment.NewLine, "").Replace("\r", "").Replace("\n", "");

            //lnk.Attributes.Add("onclick", "ShowOrderModal('" + hid + "','" + safeModalHtml + "'); return false;");
            //根據 hid 呼叫 $.ajax() 從後端載入 modal 內容
            lnk.Attributes.Add("onclick", "ShowOrderModalByID('" + hid + "','" + rollcallhead + "'); return false;");

            #endregion





            for (int i = 0; i < gHDateRangeA.Length; i++)
            {
                var date = gHDateRangeA[i];
                var cell = new TableCell { HorizontalAlign = HorizontalAlign.Center, BorderWidth = 1 };

                if (recordDict.ContainsKey(hid))
                {
                    foreach (var rec in recordDict[hid])
                    {
                        if (rec["HCreateDate"].ToString() != date) continue;
                        var gType = rec["gType"].ToString();
                        var title = rec["HSCClassName"].ToString();
                        var color = gType == "1" || gType == "2" || gType == "3" ? "#e0a1db" :
                                    gType == "4" ? "#75c8d6" :
                                    gType == "5" ? "#75c8d6" : "#f5b247";
                        var html = "<a style='display:block;width:100%;background: " + color + ";border-radius: 5px; color:#fff; padding-left: 5px;text-align:left;margin-bottom:3px;' href=\"javascript:ShowModalContent('" + gType + "','" + date + "','" + hid + "','<h6>" + area + "</h6><h4 class=font-weight-bold>" + pname + "</h4>');\">" + title + "</a>";
                        cell.Controls.Add(new Literal { Text = html });
                    }
                }


                //Response.Write("eipuid="+ eipuid + "<br/>");

                if (!string.IsNullOrEmpty(eipuid) && recordDict.ContainsKey(eipuid))
                {
                    foreach (var rec in recordDict[eipuid])
                    {
                        if (rec["HCreateDate"].ToString() != date) continue;
                        var gType = rec["gType"].ToString();
                        var title = rec["HSCClassName"].ToString();
                        var color = gType == "1" ? "#e0a1db" : "#80bfff";
                        var html = "<a style='display:block;width:100%;background: " + color + ";border-radius: 5px; color:#fff; padding-left: 5px;text-align:left;margin-bottom:3px;' href=\"javascript:ShowModalEIPContent('" + gType + "','" + date + "','" + eipuid.PadLeft(10, '0') + "','<h6>" + area + "</h6><h4 class=font-weight-bold>" + pname + "</h4>');\">" + title + "</a>";
                        cell.Controls.Add(new Literal { Text = html });
                    }
                }

                row.Cells.Add(cell);
            }

            TBL_HSCGRecord.Rows.Add(row);
        }


        #endregion



        #region 記住搜尋條件
        //AA20250726
        Session["Search_Username"] = TB_HUsernameSearch.Text.Trim();
        Session["Search_Date"] = TB_DateSearch.Text.Trim();
        Session["Search_HArea"] = DDL_HAreaSearch.SelectedValue;
        Session["Search_HSystem"] = DDL_HSystemSearch.SelectedValue;
        Session["Search_AxisType"] = DDL_HAxisTypeSearch.SelectedValue;
        Session["Search_Active"] = true;

        #endregion




    }
    #endregion

    #region 取得該週數的第一天
    private DateTime GetMondayDate(int weekOffset)
    {
        DateTime today = DateTime.Today;
        int daysToMonday = -(int)(today.DayOfWeek - DayOfWeek.Monday);
        DateTime thisMonday = today.AddDays(daysToMonday);
        return thisMonday.AddDays(weekOffset * 7);
    }
    #endregion

    #region 取消搜尋功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_HUsernameSearch.Text = null;
        TB_DateSearch.Text = null;
        DDL_HAreaSearch.SelectedValue = "0";
        DDL_HSystemSearch.SelectedValue = "0";
        DDL_HAxisTypeSearch.SelectedValue = "0";


        //清除Session
        Session.Remove("Search_Username");
        Session.Remove("Search_Date");
        Session.Remove("Search_HArea");
        Session.Remove("Search_HSystem");
        Session.Remove("Search_AxisType");
        Session.Remove("Search_Active");

        Response.Redirect("HSCGRecord.aspx");

    }
    #endregion

    #region 匯出成長紀錄Excel
    /// <summary>
    /// 需匯出EDU和EIP的紀錄
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Btn_Export_Click(object sender, EventArgs e)
    {
        System.Data.DataTable gDtRecord = new System.Data.DataTable();

        SqlCommand dbCmd = default(SqlCommand);
        string QuerySql = null;


        if (!string.IsNullOrEmpty(TB_DateRange.Text.Trim()))
        {
            string Days = TB_DateRange.Text;//取得日期區間
            string[] Date = Days.Split('-');//區間分割    

            var startDate = DateTime.Parse(Date[0].Trim()).Date;           // 起日 00:00:00
            var endExclusive = DateTime.Parse(Date[1].Trim()).Date.AddDays(1); // 迄日翌日 00:00:00（半開區間）
            var memberId = ((Label)Master.FindControl("LB_HUserHID")).Text; //EDU HID
            var eipUidRaw = ((TextBox)Master.FindControl("TB_EIPUid")).Text;     // 取登入者 EIP UID（若你有別的來源，調整這行）
            var eipUid10 = (eipUidRaw ?? "").Trim().PadLeft(10, '0');           // EIP UID 統一補滿 10 碼

            //EDU SQL語法
            QuerySql = @"
SELECT 
    a.HCreateDT      AS N'紀錄日期',
    a.HTopicName     AS N'主題名稱',
    e.HCourseName    AS N'課程名稱',
    c.HSCClassName   AS N'專欄分類',
    b.HSCRTName      AS N'紀錄類型',
    a.HContent       AS N'內容',
    CASE a.HSCJiugonggeTypeID
        WHEN '1' THEN N'身' WHEN '2' THEN N'心' WHEN '3' THEN N'靈'
        WHEN '4' THEN N'人' WHEN '5' THEN N'事' WHEN '6' THEN N'境'
        WHEN '7' THEN N'金錢' WHEN '8' THEN N'關係' WHEN '9' THEN N'時間'
        ELSE N'' END      AS N'九宮格類型',
    d.HSCGProgressName AS N'成長進度',
    a.HOGProgress       AS N'其他成長進度',
    a.HHashTag          AS N'HashTag標籤'
FROM HSCGRMsg AS a
LEFT JOIN HSCRecordType AS b ON a.HSCRecordTypeID = b.HID
LEFT JOIN HSCClass      AS c ON a.HSCClassID      = c.HID
LEFT JOIN HSCGProgress  AS d ON a.HGProgress      = d.HID
LEFT JOIN HCourse       AS e ON a.HCourseID       = e.HID
WHERE 
    a.HStatus = 1
    AND a.HCreateDT >= @StartDate
    AND a.HCreateDT <  @EndExclusive
    AND a.HMemberID = @MemberID
ORDER BY a.HCreateDT DESC;";

            using (var con = new SqlConnection(
          ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString))
            using (var cmd = new SqlCommand(QuerySql, con))
            {
                cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                cmd.Parameters.Add("@EndExclusive", SqlDbType.DateTime).Value = endExclusive;
                cmd.Parameters.Add("@MemberID", SqlDbType.NVarChar, 50).Value = memberId ?? (object)DBNull.Value;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                //DataTable dt = new System.Data.DataTable();
                // this will query your database and return the result to your datatable
                da.Fill(gDtRecord);

            }

            //EIP SQL語法
            // 2) ⬇️ 新增：撈 EIP 並合併（MySQL，參數化）
            //    以 gDtRecord 的欄位為準，EIP 沒有的欄位補空字串。
            using (var mycon = new MySqlConnection(EIPconnStr))
            {
                mycon.Open();

                // 以半開區間查 EIP；EIP 的日期是字串 yyyyMMddHHmmss，這裡也用字串區間
                string eipBeg = startDate.ToString("yyyyMMdd") + "000000";
                string eipEnd = endExclusive.AddSeconds(-1).ToString("yyyyMMdd") + "235959"; // 或者直接用 endExclusive 的 000000 當 < 上限

                string mysqlSql = @"
SELECT gr_uid, gr_title, gr_category,gr_grid, gr_dharma,gr_dharmaname, gr_recorddate, gr_type, gr_content1
FROM growthrecord
WHERE gr_status < 9
  AND gr_uid = @uid
  AND gr_recorddate >= @beg
  AND gr_recorddate <  @end ORDER BY gr_recorddate DESC;";

                using (var mycmd = new MySqlCommand(mysqlSql, mycon))
                {
                    mycmd.Parameters.Add("@uid", MySqlDbType.VarChar).Value = eipUid10;
                    mycmd.Parameters.Add("@beg", MySqlDbType.VarChar).Value = startDate.ToString("yyyyMMdd") + "000000";
                    mycmd.Parameters.Add("@end", MySqlDbType.VarChar).Value = endExclusive.ToString("yyyyMMdd") + "000000";

                    using (var rdr = mycmd.ExecuteReader())
                    {
                        // 確保 gDtRecord 有需要的欄位（若前面 MSSQL 有選了中文別名，就照那些欄位名）
                        // 欄位：紀錄日期, 主題名稱, 課程名稱, 專欄分類, 紀錄類型, 內容, 九宮格類型, 成長進度, 其他成長進度, HashTag標籤
                        while (rdr.Read())
                        {
                            var dr = gDtRecord.NewRow();

                            // 轉「紀錄日期」
                            // gr_recorddate: yyyyMMddHHmmss → 取日期部分
                            var s = rdr["gr_recorddate"].ToString();
                            DateTime dt;
                            if (DateTime.TryParseExact(s, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out dt))
                                dr["紀錄日期"] = dt.ToString("yyyy/MM/dd HH:mm:ss"); // 若你要字串：dt.ToString("yyyy/MM/dd")

                            // 專欄分類（把 EIP 類型映射成你現有顏色/類別的一致值）
                            // 1=日、2=用、3=問、4=回、5=進、6=法（你之前 5/6 一律視為「進 (EIP)」）
                            var t = rdr["gr_type"].ToString();
                            string typeName = "日誌 (EIP)";
                            if (t == "2") typeName = "應用 (EIP)";
                            else if (t == "3") typeName = "提問 (EIP)";
                            else if (t == "4") typeName = "回應 (EIP)";
                            else if (t == "5") typeName = "進度 (EIP)";
                            else if (t == "6") typeName = "法脈結構進度 (EIP)";
                            dr["專欄分類"] = typeName;

                            // 內容
                            dr["內容"] = rdr["gr_content1"] == DBNull.Value ? "" : rdr["gr_content1"].ToString();

                            // EIP 沒有的欄位補空
                            dr["主題名稱"] = rdr["gr_title"].ToString();
                            dr["課程名稱"] = "";
                            dr["紀錄類型"] = rdr["gr_category"].ToString() == "0" ? "煉" : rdr["gr_category"].ToString() == "1" ? "行" : rdr["gr_category"].ToString() == "2" ? "愿" : rdr["gr_category"].ToString() == "3" ? "修" : "";
                            dr["九宮格類型"] = rdr["gr_grid"].ToString() == "1" ? "身" : rdr["gr_grid"].ToString() == "2" ? "心" : rdr["gr_grid"].ToString() == "3" ? "靈" : rdr["gr_grid"].ToString() == "4" ? "人" : rdr["gr_grid"].ToString() == "5" ? "事" : rdr["gr_grid"].ToString() == "6" ? "境" : rdr["gr_grid"].ToString() == "7" ? "金錢" : rdr["gr_grid"].ToString() == "8" ? "關係" : rdr["gr_grid"].ToString() == "9" ? "時間" : "";
                            dr["成長進度"] = rdr["gr_dharma"].ToString();
                            dr["其他成長進度"] = rdr["gr_dharmaname"].ToString();
                            dr["HashTag標籤"] = "";

                            gDtRecord.Rows.Add(dr);
                        }
                    }
                }
            }

            // TODO: 綁定 dt 到前端
            // 清理 HTML 標籤
            foreach (DataRow row in gDtRecord.Rows)
            {
                if (row["內容"] != DBNull.Value)
                {
                    row["內容"] = StripHtml(row["內容"].ToString());
                }
            }

            //匯出Excel
            TableToExcel(gDtRecord, ".xlsx");

            //清空
            TB_DateRange.Text = null;

        }
        else
        {
            Response.Write("<script>alert('請先選擇要搜尋的日期區間~');</script>");
            return;
        }

        //dbCmd = new SqlCommand(QuerySql, ConStr);





    }
    #endregion

    #region 去除html標籤
    protected static string StripHtml(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // 使用正則移除所有 HTML 標籤
        return Regex.Replace(input, "<.*?>", string.Empty);
    }
    #endregion

    /// <summary>
    /// Datable匯出成Excel
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="file"></param>
    public static void TableToExcel(System.Data.DataTable dt, string file)
    {
        IWorkbook workbook;
        string fileExt = Path.GetExtension(file).ToLower();
        if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); } else { workbook = null; }
        if (workbook == null) { return; }
        ISheet sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(dt.TableName);

        //表頭  
        IRow row = sheet.CreateRow(0);
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            ICell cell = row.CreateCell(i);
            cell.SetCellValue(dt.Columns[i].ColumnName);
        }

        //資料  
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            IRow row1 = sheet.CreateRow(i + 1);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                ICell cell = row1.CreateCell(j);
                cell.SetCellValue(dt.Rows[i][j].ToString());
            }
        }

        //下載Excel檔
        //HttpContext.Current.Response.BinaryWrite(stream.GetBuffer());
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", "SCGRecord.xlsx"));
        workbook.Write(HttpContext.Current.Response.OutputStream);
        HttpContext.Current.Response.Flush();
        HttpContext.Current.Response.Close();


        ////轉為位元組陣列  
        //MemoryStream stream = new MemoryStream();
        //workbook.Write(stream);
        //var buf = stream.ToArray();

        ////儲存為Excel檔案  
        //using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
        //{
        //    fs.Write(buf, 0, buf.Length);
        //    fs.Flush();
        //}
    }



    #region  查詢舊資料 
    protected void Btn_SearchOldRecord_Click(object sender, EventArgs e)
    {
        string gid = int.Parse(DDL_HUsername.SelectedValue).ToString("D10");
        string guid = int.Parse(((TextBox)Master.FindControl("TB_EIPUid")).Text).ToString("D10");
        string date = Convert.ToDateTime(TB_RDate.Text.Trim()).ToString("yyyyMMdd");

        string url = "https://eip.hochi.org.tw/GrowthRecordHistory.aspx?date=" + date + "&id=" + gid + "&uid=" + guid;

        //Response.Write("url="+ url);
        //Response.End();

        //string script = "window.open('" + url + "', '_blank');";
        // 將 window.open 包進 window.onload（配合 __doPostBack 流程）
        string script = "<script>window.onload = function() { window.open('" + url + "', '_blank'); };</script>";
        ClientScript.RegisterStartupScript(this.GetType(), "openWindow", script);

        ShowITable(Convert.ToInt32(LB_WhichWeek.Text));

        //清空資料
        DDL_HUsername.SelectedValue = "0";
        TB_RDate.Text = null;
    }
    #endregion

    #region 查詢過往成長紀錄
    protected void LBtn_SearchOldData_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(((TextBox)Master.FindControl("TB_EIPUid")).Text))
        {
            DDL_HUsername.SelectedValue = ((TextBox)Master.FindControl("TB_EIPUid")).Text;
        }

        ShowITable(Convert.ToInt32(LB_WhichWeek.Text));

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#SearchModal').modal('show');</script>", false);//執行js
    }
    #endregion

    #region 新增成長紀錄
    protected void LBtn_AddSCGRecord_Click(object sender, EventArgs e)
    {
        SDS_HCourseName.SelectCommand = "SELECT MAX(HID) AS HID, HCourseName , MAX(HDateRange) AS HDateRange FROM HCourse_Merge Cross Apply SPLIT(Replace(HDateRange,' - ',','), ',') b WHERE HStatus=1 AND HVerifyStatus=2 GROUP BY HCourseName , HCBatchNum ORDER BY IIF(DATEPART(YYYY,LEFT(TRY_CONVERT(nvarchar, getdate(), 111),10))=DATEPART(YYYY,TRY_CONVERT(nvarchar, MIN(b.Value),111)), '5', '6') ASC, IIF(DATEPART(Month,LEFT(TRY_CONVERT(nvarchar, getdate(), 111),10))=DATEPART(Month,TRY_CONVERT(nvarchar, MIN(b.Value),111)),'1' ,'2')  ASC";
    }
    #endregion


    // C#5 相容：不要用 tuple/解構/字串插值
    private sealed class Edge
    {
        public int Child;  // HMemberID
        public int Mentor; // HMentorMemberID
        public Edge(int child, int mentor) { Child = child; Mentor = mentor; }
    }

    /// <summary>
    /// 依「護持樹」邏輯取得連線名單：
    ///   - Up: 往上找所有護持者（祖先）
    ///   - Down: 往下展開到指定層數（maxDownDepth，建議 1）
    ///   - 兩邊合併後去重
    /// </summary>
    private List<int> GetConnectedIds_MentorTreeLike(int startHid, DateTime asOfDate, int maxDownDepth)
    {
        // 1) 撈出有效期內的有向邊（child -> mentor）
        const string sql = @"
SELECT HMemberID, HMentorMemberID
FROM dbo.HMMentorRelationship
WHERE (HEndDate IS NULL OR @asOfDate < HEndDate)
  AND HMemberID IS NOT NULL
  AND HMentorMemberID IS NOT NULL
  AND HMemberID <> HMentorMemberID;";

        var edges = new List<Edge>(4096);
        using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString))
        using (var cmd = new SqlCommand(sql, con))
        {
            cmd.Parameters.Add("@asOfDate", SqlDbType.Date).Value = asOfDate.Date;
            con.Open();
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    int child = r.GetInt32(0);
                    int mentor = r.GetInt32(1);
                    edges.Add(new Edge(child, mentor));
                }
            }
        }

        // 2) 建兩個鄰接表：往上 (child -> mentor)，往下 (mentor -> children)
        var up = new Dictionary<int, List<int>>(edges.Count * 2);
        var down = new Dictionary<int, List<int>>(edges.Count * 2);

        foreach (Edge e in edges)
        {
            List<int> tmp;
            if (!up.TryGetValue(e.Child, out tmp)) { tmp = new List<int>(); up[e.Child] = tmp; }
            tmp.Add(e.Mentor);

            if (!down.TryGetValue(e.Mentor, out tmp)) { tmp = new List<int>(); down[e.Mentor] = tmp; }
            tmp.Add(e.Child);
        }

        var result = new HashSet<int>();
        result.Add(startHid);

        // 3) 往上：所有祖先（無限層）
        var stack = new Stack<int>();
        stack.Push(startHid);
        while (stack.Count > 0)
        {
            int cur = stack.Pop();
            List<int> mentors;
            if (!up.TryGetValue(cur, out mentors)) continue;
            for (int i = 0; i < mentors.Count; i++)
            {
                int m = mentors[i];
                if (result.Add(m)) stack.Push(m);
            }
        }

        // 4) 往下：限制層數（建議 1）
        if (maxDownDepth > 0)
        {
            var frontier = new Queue<int>();
            frontier.Enqueue(startHid);
            int depth = 0;
            while (frontier.Count > 0 && depth < maxDownDepth)
            {
                int countThisLevel = frontier.Count;
                for (int k = 0; k < countThisLevel; k++)
                {
                    int cur = frontier.Dequeue();
                    List<int> kids;
                    if (!down.TryGetValue(cur, out kids)) continue;
                    for (int i = 0; i < kids.Count; i++)
                    {
                        int ch = kids[i];
                        if (result.Add(ch)) frontier.Enqueue(ch);
                    }
                }
                depth++;
            }
        }

        var resultIds = new List<int>(result);
        resultIds.Sort();
        return resultIds;
    }


    // 批次取出「表格需要用到」的成員資料欄位，組成和原本 dtMembers 相同 schema
    private System.Data.DataTable GetMembersTableForIds(IEnumerable<int> hids)
    {
        // Schema 依你 ShowITable() 會用到的欄位
        var dt = new System.Data.DataTable();
        dt.Columns.Add("HID", typeof(string));
        dt.Columns.Add("HEIPUid", typeof(string));
        dt.Columns.Add("LAreaName", typeof(string));
        dt.Columns.Add("AreaName", typeof(string));
        dt.Columns.Add("TeamName", typeof(string));
        dt.Columns.Add("HPeriod", typeof(string));
        dt.Columns.Add("HUserName", typeof(string));
        dt.Columns.Add("HRName", typeof(string));

        // C#5：不能用 ?. 和 ?? new List<int>() 的合併；改成傳統寫法
        var idList = new List<int>();
        if (hids != null)
        {
            // 去重；若不想依賴 LINQ，可改用 HashSet 再 AddRange
            idList = hids.Distinct().ToList();
        }
        if (idList.Count == 0) return dt;

        // 拆批避免 IN 太長（每批 200）
        const int BATCH = 200;
        for (int i = 0; i < idList.Count; i += BATCH)
        {
            var chunk = idList.Skip(i).Take(BATCH).ToList();
            string inClause = string.Join(",", chunk);

            // C#5 沒有字串插值，改用字串連接
            string sql =
                "SELECT a.HID, a.HEIPUid, d.HLArea AS LAreaName, c.HArea AS AreaName, "+
                "       CAST(NULL AS NVARCHAR(100)) AS TeamName, " +   // ← 這行取代原本的 t.HTeamName
                "       a.HPeriod, a.HUserName, r.HRName " +
                "FROM dbo.HMember a " +
                "LEFT JOIN dbo.HRole r   ON a.HPositionID = r.HID " +
                "LEFT JOIN dbo.HArea c   ON a.HAreaID = c.HID " +
                "LEFT JOIN dbo.HLArea d  ON c.HLAreaID = d.HID " +
                "WHERE a.HStatus = 1 AND a.HID IN (" + inClause + ");";

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                var tmp = new System.Data.DataTable();
                con.Open();
                da.Fill(tmp);
                foreach (DataRow r in tmp.Rows) dt.ImportRow(r);
            }
        }
        return dt;
    }


    protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        string loginHID = ((Label)Master.FindControl("LB_HUserHID")).Text;

        if ("team".Equals(RadioButtonList1.SelectedValue, StringComparison.OrdinalIgnoreCase))
        {
            Session["dtMembers"] = SQLdatabase.ExecuteDataTable(
                "EXEC MemberInfo @HID ='" + loginHID + "'"
            );
        }
        else
        {
            int startHid;
            if (!int.TryParse(loginHID, out startHid)) startHid = 0;

            // ↑ 往上無限層；↓ 往下 1 層（可視需要調整 0/1/2）
            var ids = GetConnectedIds_MentorTreeLike(startHid, DateTime.Today, 1);

            var dtNet = GetMembersTableForIds(ids);   // 你已有的查人檔方法
            Session["dtMembers"] = dtNet;

            // 你在 ShowITable 裡請務必用 Session["dtMembers"] 的 HID 組 IN(...)，不要再用光團子查詢；
            // 你的檔案目前仍有『以光團 IN(...)』的 SQL，記得替換成用 dtMembers 的 HID 串出 in-clause。:contentReference[oaicite:0]{index=0}
        }

        ShowITable(Convert.ToInt32(LB_WhichWeek.Text));
    }
}