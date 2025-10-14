 using MySql.Data.MySqlClient;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


public partial class HSCForum : System.Web.UI.Page
{
    string EIPconnStr = ConfigurationManager.ConnectionStrings["HochiEIPConnection"].ConnectionString;
    SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    string Con = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
    int gPermission = 1;

    string gSelHSCTopic = "SELECT TOP(15)  a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  HTopicName, HPinTop,HContent, HFile1 ,  e.HID AS HMemberID,(ISNULL(e.HSystemName,'無體系')+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, e.HImg,  a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID";

    string gQuerySCTopic = "SELECT TOP(15) a.HID, a.HCourseID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , (ISNULL(e.HSystemName,'無體系')+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, IIF(e.HID IS NULL,'0',e.HID) AS HMemberID,e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName ,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY IIF(a.HPinTop IS NULL,0,a.HPinTop)  DESC, IIF(a.HModifyDT IS NOT NULL,a.HModifyDT, a.HCreateDT) DESC";





    #region --根目錄--
    string SCFileRoot = "D:\\Website\\System\\HochiSystem\\uploads\\SpecialColumn\\";
    string SCFile = "~/uploads/SpecialColumn/";
    #endregion


    // 結構定義
    class TitleInfo
    {
        public string Date = "";
        public string Core = "";
        public string Suffix = "";
    }

    static TitleInfo ParseTitle(string input)
    {
        var result = new TitleInfo();

        // 抓日期 (8碼數字)
        var dateMatch = Regex.Match(input, @"\d{8}");
        if (dateMatch.Success)
            result.Date = dateMatch.Value;

        // 抓主題核心（如 晨光上_幸福印記）
        var coreMatch = Regex.Match(input, @"([\u4e00-\u9fa5A-Za-z0-9]+_[\u4e00-\u9fa5A-Za-z0-9]+)");
        if (coreMatch.Success)
            result.Core = coreMatch.Value;

        return result;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UnifiedTable"] == null)
        {
            // 建一個空資料結構，避免第一次 AJAX 就炸掉
            var dt = new DataTable();
            dt.Columns.Add("HID");
            dt.Columns.Add("HTopicName");
            dt.Columns.Add("HContent");
            dt.Columns.Add("UserName");
            dt.Columns.Add("HCreateDT");
            Session["UnifiedTable"] = dt;
        }



        if (!IsPostBack)
        {
            //GA20240724_選單引用：App_Code/SelectListItem/RBL_ListItem.cs
            RBL_HSCJiugonggeTypeID.DataSource = RBL_ListItem.HSCJiugonggeType.ListItem;
            RBL_HSCJiugonggeTypeID.DataTextField = "Value";
            RBL_HSCJiugonggeTypeID.DataValueField = "Key";
            RBL_HSCJiugonggeTypeID.DataBind();

            RBL_HSCJiugonggeTypeM.DataSource = RBL_ListItem.HSCJiugonggeType.ListItem;
            RBL_HSCJiugonggeTypeM.DataTextField = "Value";
            RBL_HSCJiugonggeTypeM.DataValueField = "Key";
            RBL_HSCJiugonggeTypeM.DataBind();


            #region 報名哪些課程資訊
            DataTable dTBookedCourse = SQLdatabase.ExecuteDataTable(
    "SELECT TOP(15) HCoursename FROM HCourseBooking " +
    "WHERE HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'" +
    "AND HStatus = 1 AND HItemStatus = 1 AND HCourseDonate = '0' ORDER BY HID DESC"
);

            HashSet<string> userCourseNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // 不區分大小寫
            foreach (DataRow row in dTBookedCourse.Rows)
            {
                userCourseNames.Add(row["HCoursename"].ToString().Trim());
            }

            // 放到 Session 或 ViewState（視情況）供 ItemDataBound 使用
            Session["UserBookedCourseNames"] = userCourseNames;
            #endregion



            if (Request.QueryString["FClassBID"] != null)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["FClassBID"].ToString()))   //HSCForumClass.HID(次類別)
                {
                    LB_HSCForumClassBID.Text = Request.QueryString["FClassBID"].ToString();

                    SqlDataReader QueryHSCForumClass = SQLdatabase.ExecuteReader("SELECT a.HID, b.HID AS HSCForumClassAID, b.HSCFCName AS HSCForumClassA, a.HSCFCName AS HSCForumClassB FROM  HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON a.HSCFCMaster=b.HID WHERE a.HID='" + Request.QueryString["FClassBID"].ToString() + "'");

                    while (QueryHSCForumClass.Read())
                    {
                        LB_HSCForumClassAID.Text = QueryHSCForumClass["HSCForumClassAID"].ToString();
                        LB_HSCForumClassA.Text = QueryHSCForumClass["HSCForumClassA"].ToString();
                        LTR_HSCForumClassB.Text = QueryHSCForumClass["HSCForumClassB"].ToString();
                    }
                    QueryHSCForumClass.Close();

                    SDS_HSCForumClassCKeyword.SelectCommand = "SELECT a.HID, a.HSCFCName AS HSCForumClassC, a.HPublic FROM  HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON a.HSCFCMaster=b.HID WHERE a.HSCFCMaster='" + Request.QueryString["FClassBID"].ToString() + "' AND a.HStatus=1";
                    Rpt_HSCForumClassCKeyword.DataBind();

    

                    //GE20250706_綜合顯示EIP資料
                    #region 整合EIP資料
                    //1.EDU的資料
                    //var dtRecords = SQLdatabase.ExecuteDataTable(gSelHSCTopic + " WHERE a.HStatus=1 AND c.HID='" + Request.QueryString["FClassBID"].ToString() + "' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName ,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY IIF(a.HPinTop IS NULL,0,a.HPinTop)  DESC, IIF(a.HModifyDT IS NOT NULL,a.HModifyDT, a.HCreateDT) DESC");

                    var sql = "SELECT TOP(15)  a.HID,    a.HCourseID,    c.HSCFCName AS HSCForumClassB,    b.HSCFCName AS HSCForumClassC,    a.HTopicName,    a.HPinTop,    a.HContent,    a.HFile1,    (ISNULL(e.HSystemName, N'無體系') + N' ' + ISNULL(e.HArea, N'') + N'/' + ISNULL(e.HPeriod, N'') + N' ' + ISNULL(e.HUserName, N'')) AS UserName,    CASE WHEN e.HID IS NULL THEN 0 ELSE e.HID END AS HMemberID,    e.HImg,    a.HCreateDT,    a.HModifyDT, h.HIRestriction FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID = b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster = c.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID = d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID OUTER APPLY (     SELECT TOP (1) LTRIM(RTRIM(x.n.value('.', 'varchar(50)'))) AS CourseIDStr    FROM (SELECT CAST('<i>' + REPLACE(ISNULL(a.HCourseID,''), ',', '</i><i>') + '</i>' AS xml) AS xdoc) AS t    CROSS APPLY t.xdoc.nodes('/i') AS x(n)    WHERE LTRIM(RTRIM(x.n.value('.', 'varchar(50)'))) <> '') ss LEFT JOIN HCourse AS h     ON h.HID = CASE WHEN ISNUMERIC(ss.CourseIDStr) = 1 THEN CAST(ss.CourseIDStr AS int) ELSE NULL END WHERE     a.HStatus = 1    AND c.HID ='"+ Request.QueryString["FClassBID"].ToString() + "' GROUP BY a.HID,    a.HCourseID,    c.HSCFCName,    b.HSCFCName ,    a.HTopicName,    a.HPinTop,    a.HContent,    a.HFile1, e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID, e.HImg,    a.HCreateDT,    a.HModifyDT, h.HIRestriction ORDER BY     ISNULL(a.HPinTop, 0) DESC,    ISNULL(a.HModifyDT, a.HCreateDT) DESC;";
                    var dtRecords = SQLdatabase.ExecuteDataTable(sql);

                    // 2. 撈 MySQL 資料

                    var sw = new System.Diagnostics.Stopwatch();

                   



                    var mysqlConn = new MySqlConnection(EIPconnStr);
                    var mysqlCmd = new MySqlCommand("SELECT  l_id AS HID, 'EIP' AS HSCForumClassB, 'EIP' AS HSCForumClassC,  l_title AS Title, l_Content AS Content, l_recorddate AS HCreateDT,  l_cname AS UserName FROM heip.laoshi WHERE l_status=2 AND l_category=0 AND l_title NOT LIKE '%成長紀錄%'  AND l_recorddate >= '20250913000000' AND l_recorddate < DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 15 HOUR), '%Y%m%d%H%i%S') ORDER BY l_no DESC LIMIT 15;", mysqlConn);/*  l_recorddate LIKE CONCAT('%', DATE_FORMAT(CURRENT_DATE(), '%y%m%d'), '%') AND l_recorddate < DATE_FORMAT(DATE_ADD(NOW(), INTERVAL 15 HOUR), '%y%m%d%H%i%s') */
                    //AND l_recorddate LIKE CONCAT('%', DATE_FORMAT(CURRENT_DATE(), '%y%m%d'), '%')
                    var mysqlAdapter = new MySqlDataAdapter(mysqlCmd);
                    System.Data.DataTable mysqlRecords = new System.Data.DataTable();
                    mysqlAdapter.Fill(mysqlRecords);

                    // 1. 建立 MSSQL 主題索引（以 "主題核心+日期" 為 Key）
                    Dictionary<string, DataRow> mssqlTopicMap = new Dictionary<string, DataRow>();
                    foreach (DataRow row in dtRecords.Rows)
                    {
                        string title = row["HTopicName"].ToString();
                        var parsed = ParseTitle(title);
                        if (string.IsNullOrEmpty(parsed.Core) || string.IsNullOrEmpty(parsed.Date)) continue;

                        string key = parsed.Core + "_" + parsed.Date;

                        if (!mssqlTopicMap.ContainsKey(parsed.Core))
                        {
                            if (!mssqlTopicMap.ContainsKey(parsed.Date))
                            {
                                mssqlTopicMap[key] = row;
                            }
                        }

                    }

                    // 2. 建立最終合併結果表 unifiedTable
                    System.Data.DataTable unifiedTable = dtRecords.Clone();
                    //新增欄位
                    unifiedTable.Columns.Add("source", typeof(string));   // 來源
                    unifiedTable.Columns.Add("EIPID", typeof(string));  // 合併進來的 EIP l_id
                    HashSet<string> mergedKeys = new HashSet<string>();

                    // 3. 先將 MSSQL 資料加入 unifiedTable
                    foreach (var row in dtRecords.Rows)
                    {
                        DataRow newRow = unifiedTable.NewRow();
                        newRow.ItemArray = ((DataRow)row).ItemArray.Clone() as object[];
                        //新增欄位
                        newRow["source"] = "EDU";
                        newRow["EIPID"] = ""; // 預設無
                        unifiedTable.Rows.Add(newRow);
                    }

                    // 4. 處理 MySQL 資料
                    foreach (DataRow row in mysqlRecords.Rows)
                    {
                        string lid = row["HID"].ToString();

                        //string title = row["Title"].ToString();

                        //縮減標題長度
                        //string title = row["Title"].ToString().Length > 50 ? row["Title"].ToString().Substring(0, 50)+"..." : row["Title"].ToString();

                        //GE20250706_改第一個br前才作為標題
                        int brIndex = row["Title"].ToString().IndexOf("br", StringComparison.OrdinalIgnoreCase);
                        string title = row["Title"].ToString();
                        string extra = "";

                        if (brIndex >= 0)
                        {
                            title = row["Title"].ToString().Substring(0, brIndex); // 取 br 前作為新標題
                            extra = row["Title"].ToString().Substring(brIndex + 2); // 剩下的移到內容（跳過 br 共4字元）
                        }
                        string content = extra.Replace("br", "<br/>") + row["Content"].ToString();
                        var parsed = ParseTitle(title);
                        if (string.IsNullOrEmpty(parsed.Core) || string.IsNullOrEmpty(parsed.Date)) continue;

                        string key = parsed.Core + "_" + parsed.Date;
                        string datePart = "";
                        string titlePart = "";

                        if (!string.IsNullOrEmpty(parsed.Core) && parsed.Core.Length >= 8 && parsed.Core.Substring(0, 8).All(char.IsDigit))
                        {
                            datePart = parsed.Core.Substring(0, 8);
                            titlePart = parsed.Core.Substring(8).Split('_')[0];
                        }
                        else
                        {
                            datePart = parsed.Date;
                            titlePart = parsed.Core;
                        }


                        bool matched = false;

                        foreach (var kvp in mssqlTopicMap)
                        {
                            string mssqlKey = kvp.Key; // 格式是 core + "_" + date，如 晨光上_幸福印記_20250323

                            if (mssqlKey.Contains(titlePart) && mssqlKey.Contains(datePart))
                            {
                                // 找到相符主題，合併內容
                                DataRow targetRow = kvp.Value;
                                targetRow["HContent"] = targetRow["HContent"].ToString() + "\n---\n" + content;

                                //// 記錄 EIP l_id
                                //string oldEip = targetRow.Table.Columns.Contains("EIP_HID") ? targetRow["EIP_HID"].ToString() : "";
                                //if (!string.IsNullOrEmpty(oldEip))
                                //{

                                //    targetRow["EIP_HID"] = oldEip + "," + lid;

                                //    //Response.Write("oldEip=" + oldEip + "lid=" + lid + "<br/>");
                                //    //Response.End();
                                //}
                                //else
                                //{
                                //    targetRow["EIP_HID"] = lid;

                                //}


                                matched = true;
                                break;
                            }

                        }

                        if (!matched)
                        {
                            // 沒有找到相同主題，新增 MySQL 資料為獨立主題
                            DataRow newRow = unifiedTable.NewRow();
                            newRow["HID"] = lid;
                            newRow["HTopicName"] = title;
                            newRow["HContent"] = content;
                            newRow["UserName"] = "EIP";
                            newRow["HCreateDT"] = row["HCreateDT"];
                            //newRow["EIPlrID"] = "";
                            unifiedTable.Rows.Add(newRow);
                        }

                    }

                    //Session["UnifiedTable"] = unifiedTable;

                    //加入排序條件
                    DataView sortedView = unifiedTable.DefaultView;
                    sortedView.Sort = "HCreateDT DESC";
                    DataTable sortedTable = sortedView.ToTable();


                    //初次只載入前10筆
                    //DataTable first10 = sortedTable.Clone();
                    //for (int i = 0; i < Math.Min(10, sortedTable.Rows.Count); i++)
                    //{
                    //    first10.ImportRow(sortedTable.Rows[i]);
                    //}
                    //Rpt_HSCTopic.DataSource = first10;
                    Rpt_HSCTopic.DataSource = sortedTable;
                    Rpt_HSCTopic.DataBind();

                    // 記住目前已載入筆數 (可存在 hidden field 或 ViewState)
                   // HF_CurrentIndex.Value = first10.Rows.Count.ToString();

                    // Session 仍保留完整資料供 LoadMore 使用
                    Session["UnifiedTable"] = sortedTable;   // 必須保留完整筆數供 ashx 分頁使用


                    #endregion

                    SDS_HSCForumClassA.SelectCommand = "SELECT HID, HSCFCMaster, HSCFCName, HSCFCLevel, HStatus FROM HSCForumClass WHERE HStatus=1 AND HSCFCMaster='0' AND HSCFCLevel='10' AND HID='" + LB_HSCForumClassAID.Text + "'";
                    //DDL_HSCForumClassA.DataBind();
                    DDL_HSCForumClassA.SelectedValue = LB_HSCForumClassAID.Text;
                    DDL_HSCForumClassA.Enabled = false;

                    SDS_HSCForumClassB.SelectCommand = "SELECT a.HID, a.HSCFCName, a.HSCFCLevel, a.HStatus, b.HSCFCName AS HSCForumMaster FROM HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON b.HID=a.HSCFCMaster WHERE a.HStatus = 1 AND a.HSCFCLevel = '20' AND a.HID='" + LB_HSCForumClassBID.Text + "'";
                    DDL_HSCForumClassB.SelectedValue = LB_HSCForumClassBID.Text;
                    DDL_HSCForumClassB.Enabled = false;

                    SDS_HSCForumClassC.SelectCommand = "SELECT a.HID, a.HSCFCName, a.HSCFCLevel, a.HStatus, b.HSCFCName AS HSCForumMaster FROM HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON b.HID=a.HSCFCMaster WHERE a.HStatus = 1 AND a.HSCFCLevel = '30' AND a.HSCFCMaster ='" + LB_HSCForumClassBID.Text + "'";

                }

            }
            else
            {
                Response.Redirect("HSCIndex.aspx");
            }


        }
       
    }


    #region 發表主題
    protected void Btn_Launch_Click(object sender, EventArgs e)
    {
        #region 必填判斷
        if (string.IsNullOrEmpty(TB_HTopicName.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請輸入主題名稱~');$('#ContentModal').modal('show');", true);
            return;
        }

        if (DDL_HSCForumClassA.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區主類別~');$('#ContentModal').modal('show');", true);
            return;
        }

        if (DDL_HSCForumClassB.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區次類別~');$('#ContentModal').modal('show');", true);
            return;
        }
        #endregion

        //GA20240827_新增HashTag標籤(多選)
        //取ListBox的值，使用ForEach方式
        string gLBox_HSCHotHashTag = "";
        foreach (ListItem LBoxHSCHotHashTag in LBox_HSCHotHashTag.Items)
        {
            if (LBoxHSCHotHashTag.Selected == true)
            {
                gLBox_HSCHotHashTag += LBoxHSCHotHashTag.Text + ",";
            }
        }

        if (!string.IsNullOrEmpty(TB_HHashTag.Text.Trim()))
        {
            gLBox_HSCHotHashTag += TB_HHashTag.Text.Trim();
        }
        else
        {
            gLBox_HSCHotHashTag = gLBox_HSCHotHashTag.TrimEnd(',');
        }


        SqlCommand cmd = new SqlCommand("INSERT INTO HSCTopic ( HSCForumClassID, HTopicName, HPinTop, HSCClassID, HSCRecordTypeID, HSCJiugonggeTypeID, HGProgress, HOGProgress, HContent, HFile1, HFile2, HFile3, HVideoLink, HHashTag, HStatus, HCreate, HCreateDT)VALUES(@HSCForumClassID, @HTopicName, @HPinTop, @HSCClassID, @HSCRecordTypeID, @HSCJiugonggeTypeID, @HGProgress, @HOGProgress, @HContent, @HFile1, @HFile2, @HFile3, @HVideoLink, @HHashTag, @HStatus, @HCreate, @HCreateDT)", ConStr);

        ConStr.Open();
        cmd.Parameters.AddWithValue("@HSCForumClassID", DDL_HSCForumClassC.SelectedValue);
        cmd.Parameters.AddWithValue("@HTopicName", TB_HTopicName.Text.Trim());
        cmd.Parameters.AddWithValue("@HPinTop", RBtn_HPinTop.SelectedValue == "1" ? "TRUE" : "FALSE");
        cmd.Parameters.AddWithValue("@HSCClassID", DDL_HSCClass.SelectedValue);
        cmd.Parameters.AddWithValue("@HSCRecordTypeID", DDL_HSCRecordType.SelectedValue);
        cmd.Parameters.AddWithValue("@HSCJiugonggeTypeID", RBL_HSCJiugonggeTypeM.SelectedValue);
        cmd.Parameters.AddWithValue("@HGProgress", DDL_HGProgress.SelectedValue);
        cmd.Parameters.AddWithValue("@HOGProgress", TB_HOGProgress.Text.Trim());
        cmd.Parameters.AddWithValue("@HContent", CKE_HContentM.Text.Trim());
        cmd.Parameters.AddWithValue("@HFile1", LB_File1.Text);
        cmd.Parameters.AddWithValue("@HFile2", LB_File2.Text);
        cmd.Parameters.AddWithValue("@HFile3", LB_File3.Text);
        cmd.Parameters.AddWithValue("@HVideoLink", TB_HVideoLink.Text.Trim());
        cmd.Parameters.AddWithValue("@HHashTag", gLBox_HSCHotHashTag);
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();


        #region 清空資料
        TB_HTopicName.Text = null;
        DDL_HSCForumClassA.SelectedValue = "0";
        DDL_HSCForumClassB.SelectedValue = "0";
        DDL_HSCForumClassC.SelectedValue = "0";
        RBtn_HPinTop.SelectedValue = null;
        DDL_HSCClass.SelectedValue = null;
        DDL_HSCRecordType.SelectedValue = null;
        RBL_HSCJiugonggeTypeM.SelectedValue = null;
        DDL_HGProgress.SelectedValue = "0";
        TB_HOGProgress.Text = null;
        CKE_HContentM.Text = null;
        LB_File1.Text = null;
        LB_File2.Text = null;
        LB_File3.Text = null;
        TB_HVideoLink.Text = null;
        TB_HHashTag.Text = null;
        #endregion

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('已發表主題~');", true);

        SDS_HSCTopic.SelectCommand = gSelHSCTopic + " WHERE a.HStatus=1 GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName ,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY IIF(a.HPinTop IS NULL,0,a.HPinTop)  DESC, IIF(a.HModifyDT IS NOT NULL,a.HModifyDT, a.HCreateDT) DESC";

    }
    #endregion

    #region 關閉主題
    protected void Btn_Close_Click(object sender, EventArgs e)
    {

    }
    #endregion



    #region 發表主題 上傳檔案
    protected void LBtn_HFile1Upload_Click(object sender, EventArgs e)
    {
        //建立資料夾
        if (!Directory.Exists(SCFileRoot))
        {
            Directory.CreateDirectory(SCFileRoot);
        }

        bool FileIsValidUP1 = false;

        if (FU_HFile1.HasFile)
        {
            //上傳檔是否大於10M
            if (FU_HFile1.PostedFile.ContentLength > (10 * 1024 * 1024))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('上傳的檔案不能超過10MB喔！');", true);
                //Response.Write("<script>alert('上傳的檔案不能超過5MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_HFile1.FileName).ToLower(); //取得上傳檔案的副檔名

                String[] restrictExtension = { ".jpg", ".png", ".heic", ".gif", ".mp3", ".pdf", ".docx", ".doc", ".xlsx", ".xls" };
                foreach (string i in restrictExtension)
                {
                    if (FileExtension == i)
                    {
                        FileIsValidUP1 = true;
                        break;
                    }
                }

                if (FileIsValidUP1)
                {
                    //檔名
                    LB_File1.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;
                    this.FU_HFile1.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_File1.Text));
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('上傳檔案格式錯誤~');", true);
                    return;

                }


            }
        }

        //重開Modal
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
    }


    protected void LBtn_HFile2Upload_Click(object sender, EventArgs e)
    {
        //建立資料夾
        if (!Directory.Exists(SCFileRoot))
        {
            Directory.CreateDirectory(SCFileRoot);
        }

        bool FileIsValidUP2 = false;

        if (FU_HFile2.HasFile)
        {
            //上傳檔是否大於10M
            if (FU_HFile2.PostedFile.ContentLength > (10 * 1024 * 1024))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('上傳的檔案不能超過10MB喔！');", true);
                //Response.Write("<script>alert('上傳的檔案不能超過5MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_HFile2.FileName).ToLower(); //取得上傳檔案的副檔名

                String[] restrictExtension = { ".jpg", ".png", ".heic", ".gif", ".mp3", ".pdf", ".docx", ".doc", ".xlsx", ".xls" };
                foreach (string i in restrictExtension)
                {
                    if (FileExtension == i)
                    {
                        FileIsValidUP2 = true;
                        break;
                    }
                }

                if (FileIsValidUP2)
                {
                    //檔名
                    LB_File2.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;

                    this.FU_HFile2.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_File2.Text));
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('上傳檔案格式錯誤~');", true);
                    return;

                }

            }
        }

        //重開Modal
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
    }

    protected void LBtn_HFile3Upload_Click(object sender, EventArgs e)
    {
        //建立資料夾
        if (!Directory.Exists(SCFileRoot))
        {
            Directory.CreateDirectory(SCFileRoot);
        }

        bool FileIsValidUP3 = false;

        if (FU_HFile3.HasFile)
        {
            //上傳檔是否大於10M
            if (FU_HFile3.PostedFile.ContentLength > (10 * 1024 * 1024))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('上傳的檔案不能超過10MB喔！');", true);
                //Response.Write("<script>alert('上傳的檔案不能超過5MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_HFile3.FileName).ToLower(); //取得上傳檔案的副檔名

                String[] restrictExtension = { ".jpg", ".png", ".heic", ".gif", ".mp3", ".pdf", ".docx", ".doc", ".xlsx", ".xls" };
                foreach (string i in restrictExtension)
                {
                    if (FileExtension == i)
                    {
                        FileIsValidUP3 = true;
                        break;
                    }
                }

                if (FileIsValidUP3)
                {
                    //檔名
                    LB_File3.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;

                    this.FU_HFile3.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_File3.Text));
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('上傳檔案格式錯誤~');", true);
                    return;

                }

            }
        }

        //重開Modal
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
    }
    #endregion


    #region 編輯主題 上傳檔案
    protected void LBtn_HFileM1Upload_Click(object sender, EventArgs e)
    {
        //建立資料夾
        if (!Directory.Exists(SCFileRoot))
        {
            Directory.CreateDirectory(SCFileRoot);
        }

        bool FileIsValidM1 = false;
        if (FU_HFileM1.HasFile)
        {
            //上傳檔是否大於10M
            if (FU_HFileM1.PostedFile.ContentLength > (10 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_HFileM1.FileName).ToLower(); //取得上傳檔案的副檔名

                String[] restrictExtension = { ".jpg", ".png", ".heic", ".gif", ".mp3", ".pdf", ".docx", ".doc", ".xlsx", ".xls" };
                foreach (string i in restrictExtension)
                {
                    if (FileExtension == i)
                    {
                        FileIsValidM1 = true;
                        break;
                    }
                }

                if (FileIsValidM1)
                {
                    //檔名
                    LB_FileM1.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;
                    LB_FileM1.Visible = true;
                    this.FU_HFileM1.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_FileM1.Text));
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('上傳檔案格式錯誤~');", true);
                    return;
                }

            }
        }

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Edit').modal('show');</script>", false);//執行js
    }

    protected void LBtn_HFileM2Upload_Click(object sender, EventArgs e)
    {
        //建立資料夾
        if (!Directory.Exists(SCFileRoot))
        {
            Directory.CreateDirectory(SCFileRoot);
        }

        bool FileIsValidM2 = false;

        if (FU_HFileM2.HasFile)
        {
            //上傳檔是否大於10M
            if (FU_HFileM2.PostedFile.ContentLength > (10 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_HFileM2.FileName).ToLower(); //取得上傳檔案的副檔名

                String[] restrictExtension = { ".jpg", ".png", ".heic", ".gif", ".mp3", ".pdf", ".docx", ".doc", ".xlsx", ".xls" };
                foreach (string i in restrictExtension)
                {
                    if (FileExtension == i)
                    {
                        FileIsValidM2 = true;
                        break;
                    }
                }

                if (FileIsValidM2)
                {
                    //檔名
                    LB_FileM2.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;
                    LB_FileM2.Visible = true;
                    this.FU_HFileM2.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_FileM2.Text));
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('上傳檔案格式錯誤~');", true);
                    return;
                }

            }
        }

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Edit').modal('show');</script>", false);//執行js
    }

    protected void LBtn_HFileM3Upload_Click(object sender, EventArgs e)
    {
        //建立資料夾
        if (!Directory.Exists(SCFileRoot))
        {
            Directory.CreateDirectory(SCFileRoot);
        }
        bool FileIsValidM3 = false;

        if (FU_HFileM3.HasFile)
        {
            //上傳檔是否大於10M
            if (FU_HFileM3.PostedFile.ContentLength > (10 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_HFileM3.FileName).ToLower(); //取得上傳檔案的副檔名

                String[] restrictExtension = { ".jpg", ".png", ".heic", ".gif", ".mp3", ".pdf", ".docx", ".doc", ".xlsx", ".xls" };
                foreach (string i in restrictExtension)
                {
                    if (FileExtension == i)
                    {
                        FileIsValidM3 = true;
                        break;
                    }
                }

                if (FileIsValidM3)
                {
                    //檔名
                    LB_FileM3.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;
                    LB_FileM3.Visible = true;
                    this.FU_HFileM3.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_FileM3.Text));
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('上傳檔案格式錯誤~');", true);
                    return;
                }

            }
        }

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Edit').modal('show');</script>", false);//執行js
    }
    #endregion

    private string NormalizeTopicName(string gTopicName)
    {
        string result = gTopicName;

        // 1. 去掉【回應窗口】
        result = result.Replace("【回應窗口】", "").Trim();

        // 2. 去掉前面的 yyyyMMdd
        if (result.Length >= 8 && System.Text.RegularExpressions.Regex.IsMatch(result.Substring(0, 8), @"^\d{8}$"))
        {
            result = result.Substring(8);
        }

        // 3. 找 "_Day" 位置，只取之前的部分
        int idxDay = result.IndexOf("_Day", StringComparison.OrdinalIgnoreCase);
        if (idxDay > 0)
        {
            result = result.Substring(0, idxDay);
        }

        // 4. 去掉首尾空白
        return result.Trim();
    }


    #region 資料繫結
    protected void Rpt_HSCTopic_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView DRV_HSCTopic = (DataRowView)e.Item.DataItem;


        #region 判斷是否有報名該課程
        #region 判斷是否有報名該課程（標題 vs 已報名課名，子字串 & 不分大小寫）
        var divTopic = (HtmlGenericControl)e.Item.FindControl("Div_Topicarea");
        var divNoPerm = (HtmlGenericControl)e.Item.FindControl("Div_NoPermission");
        var divShow = (HtmlGenericControl)e.Item.FindControl("Div_Show");

        // 1) 取得這筆主題的標題
        string gTopicName = (DRV_HSCTopic["HTopicName"] == DBNull.Value) ? "" : DRV_HSCTopic["HTopicName"].ToString().Trim();

        // 2) 取得使用者已報名的課名集合（主流程已放進 Session，大小寫不敏感）
        var bookedNames = Session["UserBookedCourseNames"] as HashSet<string>;

        // 3) 判斷：只要主題標題包含任一已報名課名（不分大小寫），就視為「有報名」
        bool hasBooked = false;
        string compareName = NormalizeTopicName(gTopicName);
        //if (!string.IsNullOrEmpty(gTopicName) && bookedNames != null && bookedNames.Count > 0)
        //{
        //    foreach (var cn in bookedNames)
        //    {
        //        if (string.IsNullOrWhiteSpace(cn)) continue;

        //        // 不分大小寫的子字串比對
        //        if (gTopicName.IndexOf(cn, StringComparison.OrdinalIgnoreCase) >= 0 || cn.ToString().Contains(compareName) )
        //        {
        //            hasBooked = true;
        //            break;
        //        }
        //    }
        //}
        //else
        //{
        //    // 沒標題或沒有報名清單 → 預設視為未報名
        //    hasBooked = false;
        //}
        // 修正判斷（兩點：1) 先確保 compareName 不為空 2) Contains 方向改為 compareName.IndexOf(cn, ...)）
        if (!string.IsNullOrEmpty(gTopicName) &&
            bookedNames != null && bookedNames.Count > 0)
        {
            foreach (var cn in bookedNames)
            {
                if (string.IsNullOrWhiteSpace(cn)) continue;

                if (gTopicName.IndexOf(cn, StringComparison.OrdinalIgnoreCase) >= 0
                    || (!string.IsNullOrEmpty(compareName)
                        && compareName.IndexOf(cn, StringComparison.OrdinalIgnoreCase) >= 0) || cn.ToString().Contains(compareName))
                {
                    hasBooked = true;
                    break;
                }
            }
        }

        // 4) 前端標記 + 顯示控制
        divTopic.Attributes["data-booked"] = hasBooked ? "1" : "0";
        divNoPerm.Visible = !hasBooked;           // 未報名 → 顯示無權限
        divShow.Visible = hasBooked;            // 已報名 → 顯示可看內容

        // 5) 連動按鈕可否點擊（例如：詳情按鈕）
        var btnDetail = (LinkButton)e.Item.FindControl("LBtn_MsgDetail");
        if (btnDetail != null) btnDetail.Enabled = hasBooked;
        #endregion

        #endregion


        string gSource = "1"; //1:新版專欄、2:EIP

        if (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HSCForumClassB")).Text))
        {
            ((Label)e.Item.FindControl("LB_HSCForumClassB")).Text = "大愛光老師專欄";
            ((Label)e.Item.FindControl("LB_HSCForumClassC")).Text = "EIP";
            gSource = "2";
        }


        //判斷若有討論區是沒有權限的才抓資料
        if (gPermission == 0)
        {
            SqlDataReader QueryIsMember = SQLdatabase.ExecuteReader("SELECT HID FROM HSCTopic_Role WHERE HSCTopicID <> '" + DRV_HSCTopic["HID"].ToString() + "' AND HMemberID ='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");

            if (!QueryIsMember.HasRows)
            {
                ((HtmlGenericControl)e.Item.FindControl("Div_HSCTopic")).Visible = false;
            }
            QueryIsMember.Close();
        }



        ((Label)e.Item.FindControl("LB_HLink")).Text = "http://" + Request.Url.Authority + "/HSCTopicDetail.aspx?TID=" + DRV_HSCTopic["HID"].ToString();

        if (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HMember")).Text))
        {
            ((Label)e.Item.FindControl("LB_HMember")).Text = "系統自動產生";
        }

        if (((Label)e.Item.FindControl("LB_HMember")).Text == "系統自動產生")
        {
            ((Image)e.Item.FindControl("IMG_Creator")).ImageUrl = "~/images/icon.png";
        }
        else
        {
            UserImgShow(((Label)e.Item.FindControl("LB_HImg")).Text, ((Image)e.Item.FindControl("IMG_Creator")));
        }


        #region 數量計算
        //留言與回應
        //ResponseCounts(((Label)e.Item.FindControl("LB_HID")).Text);
             ((Label)e.Item.FindControl("LB_MsgNum")).Text = (ResponseCounts(((Label)e.Item.FindControl("LB_HID")).Text, gSource)).ToString("N0");

        //心情 
        ((Label)e.Item.FindControl("LB_FeelingNum")).Text = (FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString().Split(',')[0];

        //按讚
        ((LinkButton)e.Item.FindControl("LBtn_ThumbsUp")).Text = "<span class='ti-thumb-up mr-2'></span>";
        ((LinkButton)e.Item.FindControl("LBtn_ThumbsUpNum")).Text = (FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString().Split(',')[0];

        //愛心
        ((LinkButton)e.Item.FindControl("LBtn_Love")).Text = "<span class='ti-heart mr-2'></span>";
        ((LinkButton)e.Item.FindControl("LBtn_LoveNum")).Text = (FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString().Split(',')[1];

        //笑臉
        ((LinkButton)e.Item.FindControl("LBtn_Smile")).Text = "<span class='ti-face-smile mr-2'></span>";
        ((LinkButton)e.Item.FindControl("LBtn_SmileNum")).Text = (FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString().Split(',')[2];


        #region 判斷登入者點擊哪一個心情符號
        if ((FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString().Split(',')[3] != "0")
        {
            if ((FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString().Split(',')[3] == "1")
            {
                ((LinkButton)e.Item.FindControl("LBtn_ThumbsUp")).Text = "<span class='ti-thumb-up mr-2 text-info'></span>";
                ((LinkButton)e.Item.FindControl("LBtn_ThumbsUp")).Style.Add("font-size", "20px");

            }
            else if ((FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString().Split(',')[3] == "2")
            {
                ((LinkButton)e.Item.FindControl("LBtn_Love")).Text = "<span class='fa fa-heart  mr-2 text-danger'></span>";
                ((LinkButton)e.Item.FindControl("LBtn_Love")).Style.Add("font-size", "20px");
            }
            else if ((FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString().Split(',')[3] == "3")
            {
                ((LinkButton)e.Item.FindControl("LBtn_Smile")).Text = "<span class='ti-face-smile mr-2 text-success'></span>";
                ((LinkButton)e.Item.FindControl("LBtn_Smile")).Style.Add("font-size", "20px");
            }
        }


        #endregion

        //瀏覽次數
        ((Label)e.Item.FindControl("LB_SeenNum")).Text = (ViewCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString("N0");

        //分享次數
        ((Label)e.Item.FindControl("LB_ShareNum")).Text = (ShareCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString("N0");
        #endregion



    }
    #endregion


    #region 所有討論區資料繫結
    protected void Rpt_HSCForumClassSearch_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        //次類別資料
        Repeater Rpt_HSCForumClassBSearch = e.Item.FindControl("Rpt_HSCForumClassBSearch") as Repeater;  //找到repeater物件
        SqlDataSource SDS_HSCForumClassBSearch = e.Item.FindControl("SDS_HSCForumClassBSearch") as SqlDataSource;  //找到SqlDataSource物件        
        SDS_HSCForumClassBSearch.SelectCommand = "SELECT a.HID, a.HSCFCName, a.HSCFCLevel, a.HStatus, b.HSCFCName AS HSCForumMaster FROM HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON b.HID=a.HSCFCMaster WHERE a.HStatus = 1 AND a.HSCFCLevel = '20' AND a.HSCFCMaster ='" + ((Label)e.Item.FindControl("LB_HID")).Text + "';";
        Rpt_HSCForumClassBSearch.DataBind();

        //若沒有子項目則不用顯示箭頭
        if (Rpt_HSCForumClassBSearch.Items.Count == 0)
        {
            ((HtmlControl)e.Item.FindControl("arrowicon")).Visible = false;

        }

    }
    #endregion


    #region 查看更多
    protected void LBtn_View_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_View = sender as LinkButton;
        string gView_CA = LBtn_View.CommandArgument;
        string gView_CN = LBtn_View.CommandName;

        //紀錄瀏覽次數
        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HSCTopicID, HMemberID, HTimes FROM HSCTopic_View WHERE HSCTopicID='" + gView_CA + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        if (dr.Read())
        {
            SqlCommand cmd = new SqlCommand("UPDATE HSCTopic_View SET HTimes=@HTimes, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCTopicID=@HSCTopicID AND HMemberID=@HMemberID", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCTopicID", gView_CA);
            cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HTimes", Convert.ToInt32(dr["HTimes"].ToString()) + 1);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();

        }
        else
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO HSCTopic_View (HSCTopicID, HMemberID, HTimes, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HTimes, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCTopicID", gView_CA);
            cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HTimes", 1);
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
        }
        dr.Close();


        string gFrom = "0"; //紀錄資料來源 : 0=EDU、1=EIP
        if (gView_CN == "EIP")
        {
            gFrom = "1";
        }


        //導頁
        Response.Redirect("HSCTopicDetail.aspx?TID=" + gView_CA + "&F=" + gFrom);

    }
    #endregion

    #region 分享連結
    protected void Btn_Share_Click(object sender, EventArgs e)
    {

        if (Request.Cookies["TopicID"] != null)
        {
            if (Request.Cookies["TopicID"].Value != "undefined")
            {

                //分享主題通知
                /// <summary>
                /// 新增通知紀錄
                /// </summary>
                /// <param name="gHMemberID">接收通知者</param>
                /// <param name="gHActorMemberID">觸發通知者</param>
                /// <param name="gHNotifyType">通知類型(1:回應、2:心情(讚)、3:心情(愛心)、4:心情(微笑)、5:分享、6:提問)</param>
                /// <param name="gHTargetID">對應資料表的流水號</param>
                /// <param name="gTableName">對應資料表名稱</param>
                Notification.AddNotification(LB_MemberHID.Text, ((Label)Master.FindControl("LB_HUserHID")).Text, "5", Request.Cookies["TopicID"].Value, "HSCTopic");

                //紀錄分享次數
                SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HSCTopicID, HMemberID, HTimes FROM HSCTopic_Share WHERE HSCTopicID='" + Request.Cookies["TopicID"].Value + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
                if (dr.Read())
                {
                    SqlCommand cmd = new SqlCommand("UPDATE HSCTopic_Share SET HTimes=@HTimes, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCTopicID=@HSCTopicID AND HMemberID=@HMemberID", ConStr);
                    ConStr.Open();

                    cmd.Parameters.AddWithValue("@HSCTopicID", Request.Cookies["TopicID"].Value);
                    cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    cmd.Parameters.AddWithValue("@HTimes", Convert.ToInt32(dr["HTimes"].ToString()) + 1);
                    cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                    cmd.ExecuteNonQuery();
                    ConStr.Close();
                    cmd.Cancel();

                }
                else
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO HSCTopic_Share (HSCTopicID, HMemberID, HTimes, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HTimes, @HStatus, @HCreate, @HCreateDT)", ConStr);
                    ConStr.Open();

                    cmd.Parameters.AddWithValue("@HSCTopicID", Request.Cookies["TopicID"].Value);
                    cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    cmd.Parameters.AddWithValue("@HTimes", 1);
                    cmd.Parameters.AddWithValue("@HStatus", 1);
                    cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                    cmd.ExecuteNonQuery();
                    ConStr.Close();
                    cmd.Cancel();
                }
                dr.Close();

            }

            //清除cookie
            Response.Cookies["TopicID"].Expires = DateTime.Now.AddDays(-1);
        }



    }
    #endregion



    #region 主類別選擇後變化
    protected void DDL_HSCForumClassA_SelectedIndexChanged(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#ContentModal').modal();"), true);
        SDS_HSCForumClassB.SelectCommand = "SELECT a.HID, a.HSCFCName, a.HSCFCLevel, a.HStatus, b.HSCFCName AS HSCForumMaster FROM HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON b.HID=a.HSCFCMaster WHERE a.HStatus = 1 AND a.HSCFCLevel = '20' AND a.HSCFCMaster ='" + DDL_HSCForumClassA.SelectedValue + "'";

        DDL_HSCForumClassBM.DataBind();

    }
    #endregion

    #region 所屬討論區次類別選擇後變化
    protected void DDL_HSCForumClassB_SelectedIndexChanged(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#ContentModal').modal();"), true);
        SDS_HSCForumClassC.SelectCommand = "SELECT a.HID, a.HSCFCName, a.HSCFCLevel, a.HStatus, b.HSCFCName AS HSCForumMaster FROM HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON b.HID=a.HSCFCMaster WHERE a.HStatus = 1 AND a.HSCFCLevel = '30' AND a.HSCFCMaster ='" + DDL_HSCForumClassB.SelectedValue + "'";

        DDL_HSCForumClassC.DataBind();
    }
    #endregion

    #region 更多選項(編輯、刪除、隱藏)
    protected void LBtn_More_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_More = sender as LinkButton;
        string gMore_CA = LBtn_More.CommandArgument;
        int gMore_CN = Convert.ToInt32(LBtn_More.CommandName);

        //判斷若已經有點開某一個，則要隱藏
        for (int i = 0; i < Rpt_HSCTopic.Items.Count; i++)
        {
            if (i != gMore_CN)
            {
                ((HtmlControl)Rpt_HSCTopic.Items[i].FindControl("Div_Editarea")).Visible = false;
            }
        }

        if (((HtmlControl)Rpt_HSCTopic.Items[gMore_CN].FindControl("Div_Editarea")).Visible == false)
        {
            ((HtmlControl)Rpt_HSCTopic.Items[gMore_CN].FindControl("Div_Editarea")).Visible = true;
        }
        else
        {
            ((HtmlControl)Rpt_HSCTopic.Items[gMore_CN].FindControl("Div_Editarea")).Visible = false;
        }


    }

    #endregion

    #region 編輯主題(打開Modal)
    protected void LBtn_Edit_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Edit = sender as LinkButton;
        string gEdit_CA = LBtn_Edit.CommandArgument;
        int gEdit_CN = Convert.ToInt32(LBtn_Edit.CommandName);

        ((HtmlControl)Rpt_HSCTopic.Items[gEdit_CN].FindControl("Div_Editarea")).Visible = false;


        //先清空資料
        LB_FileM1.Text = null;
        LB_FileM2.Text = null;
        LB_FileM3.Text = null;
        IMG_File1.ImageUrl = null;
        IMG_File2.ImageUrl = null;
        IMG_File3.ImageUrl = null;
        Source1.Src = null;
        Source2.Src = null;
        Source3.Src = null;
        IMG_File1.Visible = false;
        IMG_File2.Visible = false;
        IMG_File3.Visible = false;
        Audio1.Visible = false;
        Audio2.Visible = false;
        Audio3.Visible = false;


        #region 判斷權限<待改成function>
        //本人或討論區的管理者才可以做隱藏
        SqlDataReader QueryEditor = SQLdatabase.ExecuteReader("SELECT a.HID, a.HSCForumClassID FROM HSCTopic AS a LEFT JOIN HSCModeratorSetting AS b ON a.HSCForumClassID=b.HSCForumClassID WHERE a.HID='" + gEdit_CA + "' AND (a.HCreate = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' OR b.HMemberID LIKE '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%')");

        if (QueryEditor.Read())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Edit').modal('show');</script>", false);//執行js
            LB_EditHID.Text = gEdit_CA;

            //顯示資料
            SqlDataReader QuerySCTopic = SQLdatabase.ExecuteReader("SELECT a.HID, a.HSCForumClassID AS HSCForumClassCID, c.HID AS HSCForumClassBID, d.HID AS HSCForumClassAID, a.HTopicName, a.HPinTop, a.HSCClassID, a.HSCRecordTypeID, a.HSCJiugonggeTypeID, a.HGProgress, a.HOGProgress, a.HContent, a.HFile1, a.HFile2, a.HFile3, a.HVideoLink, a.HHashTag, a.HStatus FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID  LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCForumClass AS d ON c.HSCFCMaster=d.HID  WHERE a.HID='" + gEdit_CA + "'");

            if (QuerySCTopic.Read())
            {
                SDS_HSCForumClassB.SelectCommand = "SELECT a.HID, a.HSCFCName, a.HSCFCLevel, a.HStatus, b.HSCFCName AS HSCForumMaster FROM HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON b.HID=a.HSCFCMaster WHERE a.HStatus = 1 AND a.HSCFCLevel = '20' AND a.HSCFCMaster ='" + QuerySCTopic["HSCForumClassAID"].ToString() + "'";
                DDL_HSCForumClassBM.DataBind();

                SDS_HSCForumClassC.SelectCommand = "SELECT a.HID, a.HSCFCName, a.HSCFCLevel, a.HStatus, b.HSCFCName AS HSCForumMaster FROM HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON b.HID=a.HSCFCMaster WHERE a.HStatus = 1 AND a.HSCFCLevel = '30' AND a.HSCFCMaster ='" + QuerySCTopic["HSCForumClassBID"].ToString() + "'";
                DDL_HSCForumClassCM.DataBind();

                RBL_HSCJiugonggeTypeM.DataSource = RBL_ListItem.HSCJiugonggeType.ListItem;
                RBL_HSCJiugonggeTypeM.DataTextField = "Value";
                RBL_HSCJiugonggeTypeM.DataValueField = "Key";
                RBL_HSCJiugonggeTypeM.DataBind();

                TB_HSCTopicNameM.Text = QuerySCTopic["HTopicName"].ToString();
                DDL_HSCForumClassAM.SelectedValue = QuerySCTopic["HSCForumClassAID"].ToString();
                DDL_HSCForumClassBM.SelectedValue = QuerySCTopic["HSCForumClassBID"].ToString();
                DDL_HSCForumClassCM.SelectedValue = QuerySCTopic["HSCForumClassCID"].ToString();
                RBtn_HPinTopM.SelectedValue = !string.IsNullOrEmpty(QuerySCTopic["HPinTop"].ToString()) ? "True" : "FALSE";
                DDL_HSCClassM.SelectedValue = QuerySCTopic["HSCClassID"].ToString();
                DDL_HSCRecordTypeM.SelectedValue = QuerySCTopic["HSCRecordTypeID"].ToString();

                if (!string.IsNullOrEmpty(QuerySCTopic["HSCJiugonggeTypeID"].ToString()) && QuerySCTopic["HSCJiugonggeTypeID"].ToString() != "0")
                {
                    RBL_HSCJiugonggeTypeM.SelectedValue = QuerySCTopic["HSCJiugonggeTypeID"].ToString();
                }

                DDL_HGProgressM.SelectedValue = QuerySCTopic["HGProgress"].ToString();
                TB_HOGProgressM.Text = QuerySCTopic["HOGProgress"].ToString();
                CKE_HContentM.Text = QuerySCTopic["HContent"].ToString();

                if (!string.IsNullOrEmpty(QuerySCTopic["HFile1"].ToString()))
                {
                    LB_FileM1.Visible = false;
                    LB_FileM1.Text = QuerySCTopic["HFile1"].ToString();
                    string[] gExtension = LB_FileM1.Text.Split('.');

                    if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "heic" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                    {
                        IMG_File1.Visible = true;
                        IMG_File1.ImageUrl = SCFile + LB_FileM1.Text;
                    }
                    else if (gExtension[1] == "mp3")
                    {
                        Audio1.Visible = true;
                        Source1.Src = SCFile + LB_FileM1.Text;
                    }
                    else if (gExtension[1] == "pdf")
                    {
                        LB_FileM1.Visible = true;
                    }
                }

                if (!string.IsNullOrEmpty(QuerySCTopic["HFile2"].ToString()))
                {
                    LB_FileM2.Visible = false;
                    LB_FileM2.Text = QuerySCTopic["HFile2"].ToString();
                    string[] gExtension = LB_FileM2.Text.Split('.');
                    if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "heic" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                    {
                        IMG_File2.Visible = true;
                        IMG_File2.ImageUrl = SCFile + LB_FileM2.Text;
                    }
                    else if (gExtension[1] == "mp3")
                    {
                        Audio2.Visible = true;
                        Source2.Src = SCFile + LB_FileM2.Text;
                    }
                    else if (gExtension[1] == "pdf")
                    {
                        LB_FileM2.Visible = true;
                    }
                }

                if (!string.IsNullOrEmpty(QuerySCTopic["HFile3"].ToString()))
                {
                    LB_FileM3.Visible = false;
                    LB_FileM3.Text = QuerySCTopic["HFile3"].ToString();
                    string[] gExtension = LB_FileM3.Text.Split('.');
                    if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "heic" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                    {
                        IMG_File3.Visible = true;
                        IMG_File3.ImageUrl = SCFile + LB_FileM3.Text;
                    }
                    else if (gExtension[1] == "mp3")
                    {
                        Audio3.Visible = true;
                        Source3.Src = SCFile + LB_FileM3.Text;
                    }
                    else if (gExtension[1] == "pdf")
                    {
                        LB_FileM3.Visible = true;
                    }
                }

                TB_HVideoLinkM.Text = QuerySCTopic["HVideoLink"].ToString();
                TB_HHashTagM.Text = QuerySCTopic["HHashTag"].ToString();
            }
            QuerySCTopic.Close();

        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "Notify", "<script>alert('你沒有權限編輯主題哦~');</script>", false);//執行js
            return;
        }
        QueryEditor.Close();
        #endregion

    }
    #endregion


    #region 確認儲存編輯主題功能
    //主題狀態：dbo.HSCTopic.HStatus (0:刪除/1:正常/2:隱藏)
    //主題Log類型：dbo.HSCTopic_Log.HLogType (0:刪除/1:編輯/2:隱藏)
    protected void Btn_UPDSubmit_Click(object sender, EventArgs e)
    {
        #region 必填判斷
        if (string.IsNullOrEmpty(TB_HSCTopicNameM.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請輸入主題名稱~');$('#Div_Edit').modal('show');", true);
            return;
        }

        //if (DDL_HSCForumClassAM.SelectedValue == "0")
        //{
        //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區主類別~');$('#Div_Edit').modal('show');", true);
        //    return;
        //}

        //if (DDL_HSCForumClassBM.SelectedValue == "0")
        //{
        //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區次類別~');$('#Div_Edit').modal('show');", true);
        //    return;
        //}

        if (DDL_HSCForumClassCM.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區名稱~');$('#Div_Edit').modal('show');", true);
            return;
        }
        #endregion


        //GA20240827_新增HashTag標籤(多選)
        //取ListBox的值，使用ForEach方式
        string gLBox_HSCHotHashTag = "";
        foreach (ListItem LBoxHSCHotHashTagM in LBox_HSCHotHashTagM.Items)
        {
            if (LBoxHSCHotHashTagM.Selected == true)
            {
                gLBox_HSCHotHashTag += LBoxHSCHotHashTagM.Text + ",";
            }
        }

        if (!string.IsNullOrEmpty(TB_HHashTagM.Text.Trim()))
        {
            gLBox_HSCHotHashTag += TB_HHashTagM.Text.Trim();
        }
        else
        {
            gLBox_HSCHotHashTag = gLBox_HSCHotHashTag.TrimEnd(',');
        }



        //更新資料庫

        SqlCommand cmd = new SqlCommand("UPDATE HSCTopic SET HSCForumClassID=@HSCForumClassID, HTopicName=@HTopicName, HPinTop=@HPinTop, HSCClassID=@HSCClassID, HSCRecordTypeID=@HSCRecordTypeID, HSCJiugonggeTypeID=@HSCJiugonggeTypeID, HGProgress=@HGProgress, HOGProgress=@HOGProgress, HContent=@HContent, HFile1=@HFile1, HFile2=@HFile2, HFile3=@HFile3, HVideoLink=@HVideoLink, HHashTag=@HHashTag, HStatus=@HStatus, HModify=@HModify,HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();
        cmd.Parameters.AddWithValue("@HID", Convert.ToInt32(LB_EditHID.Text));
        cmd.Parameters.AddWithValue("@HSCForumClassID", DDL_HSCForumClassCM.SelectedValue);
        cmd.Parameters.AddWithValue("@HTopicName", TB_HSCTopicNameM.Text.Trim());
        cmd.Parameters.AddWithValue("@HPinTop", RBtn_HPinTopM.SelectedValue == "1" ? "TRUE" : "FALSE");
        cmd.Parameters.AddWithValue("@HSCClassID", DDL_HSCClassM.SelectedValue);
        cmd.Parameters.AddWithValue("@HSCRecordTypeID", DDL_HSCRecordTypeM.SelectedValue);
        cmd.Parameters.AddWithValue("@HSCJiugonggeTypeID", RBL_HSCJiugonggeTypeM.SelectedValue);
        cmd.Parameters.AddWithValue("@HGProgress", DDL_HGProgressM.SelectedValue);
        cmd.Parameters.AddWithValue("@HOGProgress", TB_HOGProgressM.Text.Trim());
        cmd.Parameters.AddWithValue("@HContent", CKE_HContentM.Text.Trim());
        cmd.Parameters.AddWithValue("@HFile1", LB_FileM1.Text);
        cmd.Parameters.AddWithValue("@HFile2", LB_FileM2.Text);
        cmd.Parameters.AddWithValue("@HFile3", LB_FileM3.Text);
        cmd.Parameters.AddWithValue("@HVideoLink", TB_HVideoLinkM.Text.Trim());
        cmd.Parameters.AddWithValue("@HHashTag", gLBox_HSCHotHashTag);
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        cmd.Cancel();



        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO HSCTopic_Log (HSCTopicID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);


        logcmd.Parameters.AddWithValue("@HSCTopicID", LB_EditHID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 1);
        logcmd.Parameters.AddWithValue("@HReason", "");
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('主題內容修改完成~');", true);

        SDS_HSCTopic.SelectCommand = gSelHSCTopic + " WHERE a.HStatus=1 AND c.HID='" + Request.QueryString["FClassBID"].ToString() + "' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName ,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY IIF(a.HPinTop IS NULL,0,a.HPinTop)  DESC, IIF(a.HModifyDT IS NOT NULL,a.HModifyDT, a.HCreateDT) DESC";
        Rpt_HSCTopic.DataBind();



    }
    #endregion

    #region 刪除主題(打開Modal)
    protected void LBtn_Del_Click(object sender, EventArgs e)
    {




        LinkButton LBtn_Del = sender as LinkButton;
        string gDel_CA = LBtn_Del.CommandArgument;
        int gDel_CN = Convert.ToInt32(LBtn_Del.CommandName);

        ((HtmlControl)Rpt_HSCTopic.Items[gDel_CN].FindControl("Div_Editarea")).Visible = false;

        #region 判斷權限<待改成function>
        //本人或討論區的管理者才可以做刪除
        SqlDataReader QueryEditor = SQLdatabase.ExecuteReader("SELECT a.HID, a.HSCForumClassID FROM HSCTopic AS a LEFT JOIN HSCModeratorSetting AS b ON a.HSCForumClassID=b.HSCForumClassID WHERE a.HID='" + gDel_CA + "' AND (a.HCreate = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' OR b.HMemberID LIKE '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%')");

        if (QueryEditor.Read())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_TopicReviseModal').modal('show');</script>", false);//執行js
            Div_SCTopicDel.Visible = true;
            Div_SCTopicHide.Visible = false;

            LB_ReviseHID.Text = gDel_CA;
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "Notify", "<script>alert('你沒有權限刪除主題哦~');</script>", false);//執行js
            return;
        }
        QueryEditor.Close();
        #endregion

    }
    #endregion

    #region 確認刪除主題功能
    //主題狀態：dbo.HSCTopic.HStatus (0:刪除/1:正常/2:隱藏)
    //主題Log類型：dbo.HSCTopic_Log.HLogType (0:刪除/1:編輯/2:隱藏)
    protected void Btn_HSCTopicDel_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_HSCTopicDelReason.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入刪除主題原因');", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_TopicReviseModal').modal();"), true);
            return;
        }

        //變更主題狀態
        SqlCommand cmd = new SqlCommand("UPDATE HSCTopic SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_ReviseHID.Text);
        cmd.Parameters.AddWithValue("@HStatus", 0);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO HSCTopic_Log (HSCTopicID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCTopicID", LB_ReviseHID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 0);
        logcmd.Parameters.AddWithValue("@HReason", TB_HSCTopicDelReason.Text.Trim());
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('刪除成功!');", true);


        SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  HTopicName, HPinTop,HContent, HFile1, e.HID AS HMemberID, (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName ,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY a.HPinTop DESC, IIF(a.HModifyDT IS NULL, a.HCreateDT,  a.HModifyDT) DESC";


        Rpt_HSCTopic.DataBind();

    }
    #endregion

    #region 隱藏主題(打開Modal)
    protected void LBtn_Hide_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Hide = sender as LinkButton;
        string gHide_CA = LBtn_Hide.CommandArgument;
        int gHide_CN = Convert.ToInt32(LBtn_Hide.CommandName);

        ((HtmlControl)Rpt_HSCTopic.Items[gHide_CN].FindControl("Div_Editarea")).Visible = false;

        #region 判斷權限<待改成function>
        //本人或討論區的管理者才可以做隱藏
        SqlDataReader QueryEditor = SQLdatabase.ExecuteReader("SELECT a.HID, a.HSCForumClassID FROM HSCTopic AS a LEFT JOIN HSCModeratorSetting AS b ON a.HSCForumClassID=b.HSCForumClassID WHERE a.HID='" + gHide_CA + "' AND (a.HCreate = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' OR b.HMemberID LIKE '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%')");

        if (QueryEditor.Read())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_TopicReviseModal').modal('show');</script>", false);//執行js
            Div_SCTopicDel.Visible = false;
            Div_SCTopicHide.Visible = true;

            LB_ReviseHID.Text = gHide_CA;
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "Notify", "<script>alert('你沒有權限隱藏主題哦~');</script>", false);//執行js
            return;
        }
        QueryEditor.Close();
        #endregion



    }
    #endregion

    #region 確認隱藏主題功能
    //主題狀態：dbo.HSCTopic.HStatus (0:刪除/1:正常/2:隱藏)
    //主題Log類型：dbo.HSCTopic_Log.HLogType (0:刪除/1:編輯/2:隱藏)
    protected void Btn_HSCTopicHide_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_HSCTopicHideReason.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入隱藏主題原因');", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_TopicReviseModal').modal();"), true);
            return;
        }

        //變更主題狀態
        SqlCommand cmd = new SqlCommand("UPDATE HSCTopic SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_ReviseHID.Text);
        cmd.Parameters.AddWithValue("@HStatus", 2);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO HSCTopic_Log (HSCTopicID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCTopicID", LB_ReviseHID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 2);
        logcmd.Parameters.AddWithValue("@HReason", TB_HSCTopicHideReason.Text.Trim());
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('隱藏成功!');", true);

        SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  HTopicName, HPinTop,HContent, HFile1 , e.HID AS HMemberID, (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, e.HImg,a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName ,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY IIF(a.HPinTop IS NULL,0,a.HPinTop)  DESC, IIF(a.HModifyDT IS NOT NULL,a.HModifyDT, a.HCreateDT) DESC";
        Rpt_HSCTopic.DataBind();
    }
    #endregion

    #region 更新主題取消功能
    protected void Btn_UPDCancel_Click(object sender, EventArgs e)
    {

    }
    #endregion


    #region 心情項目(按讚、愛心、笑臉)
    protected void LBtn_Feeling_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Feeling = sender as LinkButton;
        string gFeeling_CA = LBtn_Feeling.CommandArgument;
        int gFeeling_CN = Convert.ToInt32(LBtn_Feeling.CommandName);

        //判斷若已經有點開某一個，則要隱藏
        for (int i = 0; i < Rpt_HSCTopic.Items.Count; i++)
        {
            if (i != gFeeling_CN)
            {
                ((HtmlControl)Rpt_HSCTopic.Items[i].FindControl("Div_FeelingArea")).Visible = false;
            }
        }

        if (((HtmlControl)Rpt_HSCTopic.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible == false)
        {
            ((HtmlControl)Rpt_HSCTopic.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible = true;
        }
        else
        {
            ((HtmlControl)Rpt_HSCTopic.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible = false;
        }

    }
    #endregion

    #region 主題-心情功能
    //HType：1=讚 /  2=愛心 / 3=微笑
    protected void LBtn_FeelingType_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        string gFeelingType_CA = btn.CommandArgument;
        //int gFeelingType_CN = Convert.ToInt32(btn.CommandName);

        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        //心情通知
        /// <summary>
        /// 新增通知紀錄
        /// </summary>
        /// <param name="gHMemberID">接收通知者</param>
        /// <param name="gHActorMemberID">觸發通知者</param>
        /// <param name="gHNotifyType">通知類型(1:回應、2:心情(讚)、3:心情(愛心)、4:心情(微笑)、5:分享、6:提問)</param>
        /// <param name="gHTargetID">對應資料表的流水號</param>
        /// <param name="gTableName">對應資料表名稱</param>
        if (((Label)RI.FindControl("LB_HSCForumClassC")).Text != "EIP")
        {
            Notification.AddNotification(((Label)RI.FindControl("LB_HMemberID")).Text, ((Label)Master.FindControl("LB_HUserHID")).Text, (Convert.ToInt32(btn.TabIndex.ToString()) + 1).ToString(), ((Label)RI.FindControl("LB_HID")).Text, "HSCTopic");
        }
       



        SqlDataReader QueryFeeling = SQLdatabase.ExecuteReader("SELECT HID, HSCTopicID, HMemberID, HType FROM HSCTopic_Mood WHERE HSCTopicID='" + gFeelingType_CA + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        if (QueryFeeling.Read())
        {
            if (btn.TabIndex.ToString() != QueryFeeling["HType"].ToString())
            {
                SqlCommand cmd = new SqlCommand("UPDATE HSCTopic_Mood SET HType=@HType, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCTopicID=@HSCTopicID AND HMemberID=@HMemberID", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HSCTopicID", gFeelingType_CA);
                cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HType", btn.TabIndex.ToString());
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                cmd.ExecuteNonQuery();
                ConStr.Close();
                cmd.Cancel();
            }

        }
        else
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO HSCTopic_Mood (HSCTopicID, HMemberID, HType, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HType, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCTopicID", gFeelingType_CA);
            cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HType", btn.TabIndex.ToString());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
        }
        QueryFeeling.Close();

        //隱藏顯示的區域
        //((HtmlControl)Rpt_HSCTopic.Items[gFeelingType_CN].FindControl("Div_FeelingArea")).Visible = false;


        DataTable dtHSCTopic = Session["UnifiedTable"] as DataTable;
        if (dtHSCTopic != null)
        {
            Rpt_HSCTopic.DataSource = dtHSCTopic;
            Rpt_HSCTopic.DataBind();
        }


    }
    #endregion

    #region 主題-心情功能
    //HType：1=讚 /  2=愛心 / 3=微笑
    protected void LBtn_FeelingTypeM_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;

        if (btn.TabIndex.ToString() == "1")
        {
            LBtn_ThumbsUpM.CssClass = "nav-link  active show font-weight-bold";
            LBtn_HeartM.CssClass = "nav-link font-weight-bold";
            LBtn_SmileM.CssClass = "nav-link font-weight-bold";

            SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_SCTopicID.Text + "' AND a.HType='1' AND a.HStatus=1";

        }
        else if (btn.TabIndex.ToString() == "2")
        {
            LBtn_ThumbsUpM.CssClass = "nav-link font-weight-bold";
            LBtn_HeartM.CssClass = "nav-link  active show font-weight-bold";
            LBtn_SmileM.CssClass = "nav-link font-weight-bold";

            SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_SCTopicID.Text + "' AND a.HType='2' AND a.HStatus=1";

        }
        else if (btn.TabIndex.ToString() == "3")
        {
            LBtn_ThumbsUpM.CssClass = "nav-link font-weight-bold";
            LBtn_HeartM.CssClass = "nav-link font-weight-bold";
            LBtn_SmileM.CssClass = "nav-link  active show font-weight-bold";

            SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_SCTopicID.Text + "' AND a.HType='3' AND a.HStatus=1";

        }

        Rpt_HSCTopicMood.DataBind();
        Rpt_HSCTopic.DataBind();

        //計算數量
        FeelingCounts(LB_SCTopicID.Text);

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

    }
    #endregion


    #region 查看心情紀錄
    protected void LBtn_MoodModal_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_MoodModal = sender as LinkButton;
        string gMood_CA = LBtn_MoodModal.CommandArgument;

        LB_SCTopicID.Text = gMood_CA;

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

        //預設顯示按讚的資訊
        LBtn_ThumbsUpM.CssClass = "nav-link  active show font-weight-bold";
        LBtn_HeartM.CssClass = "nav-link  font-weight-bold";
        LBtn_SmileM.CssClass = "nav-link  font-weight-bold";

        SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + gMood_CA + "' AND a.HType='1' AND a.HStatus=1";
        Rpt_HSCTopicMood.DataBind();

        //計算數量
        FeelingCounts(gMood_CA);
    }
    #endregion

    #region 心情紀錄(Modal資料繫結)
    protected void Rpt_HSCTopicMood_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        UserImgShow(gDRV["HImg"].ToString(), ((Image)e.Item.FindControl("Img_HImg")));
    }
    #endregion

    #region 討論區名稱關鍵字資料繫結
    protected void Rpt_HSCForumClassCKeyword_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //取得Repeater的DataRowView
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        //判斷討論區公開與否
        if (!string.IsNullOrEmpty(gDRV["HPublic"].ToString()))
        {
            //不公開
            if (gDRV["HPublic"].ToString() == "0")
            {
                //判斷登入者是否此討論區裡
                SqlDataReader QueryIsMember = SQLdatabase.ExecuteReader("SELECT HID FROM HSCTopic_Role WHERE HSCForumClassID='" + gDRV["HID"].ToString() + "' AND HMemberID ='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' UNION SELECT HID FROM HSCModeratorSetting WHERE HSCForumClassID='" + gDRV["HID"].ToString() + "' AND HMemberID LIKE '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%'");

                if (!QueryIsMember.HasRows)
                {
                    gPermission = 0;
                    ((HtmlGenericControl)e.Item.FindControl("Li_HSCForumClassC")).Visible = false;
                }
                QueryIsMember.Close();


                ////判斷是不是管理者
                //SqlDataReader QueryModerator = SQLdatabase.ExecuteReader("SELECT HMemberID FROM HSCModeratorSetting WHERE HSCForumClassID='" + gDRV["HID"].ToString() + "' AND HMemberID LIKE '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%'");

                //if (!QueryModerator.HasRows)
                //{
                //    ((HtmlGenericControl)e.Item.FindControl("Li_HSCForumClassC")).Visible = false;
                //}
                //QueryModerator.Close();
            }
        }
    }
    #endregion

    #region 點討論區名稱關鍵字
    protected void LBtn_HSCForumClassC_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_HSCForumClassC = sender as LinkButton;
        string gHSCForumClassC_CA = LBtn_HSCForumClassC.CommandArgument;

        Response.Redirect("HSCForumDetail.aspx?FClassCID=" + gHSCForumClassC_CA);
    }
    #endregion

    #region 所有討論區
    protected void LBtn_HSCForumClassCAll_Click(object sender, EventArgs e)
    {
        Response.Redirect("HSCForum.aspx?FClassBID=" + LB_HSCForumClassBID.Text);
    }
    #endregion

    #region 大頭照顯示
    private void UserImgShow(string LB_HImg, Image Img)
    {
        if (string.IsNullOrEmpty(LB_HImg))
        {
            Img.ImageUrl = "images/icons/profile_small.jpg";
        }
        else
        {
            Img.ImageUrl = "uploads/Account/" + LB_HImg;
        }
    }
    #endregion

    #region 心情數量計算
    private string FeelingCounts(string SCTopicID)
    {

        System.Data.DataTable dtFeeling = SQLdatabase.ExecuteDataTable("SELECT HMemberID, HType FROM HSCTopic_Mood WHERE HSCTopicID='" + SCTopicID + "'");
        int gThumbsUp = 0;
        int gLove = 0;
        int gSmile = 0;

        string gUserMood = null;
        foreach (DataRow datarow in dtFeeling.Rows)
        {
            if (datarow["HType"].ToString() == "1")
            {
                gThumbsUp++;
            }
            else if (datarow["HType"].ToString() == "2")
            {
                gLove++;
            }
            else if (datarow["HType"].ToString() == "3")
            {
                gSmile++;
            }

            //GA20241225_判斷登入者是否有按心情符號
            if (datarow["HMemberID"].ToString() == ((Label)Master.FindControl("LB_HUserHID")).Text)
            {
                gUserMood = datarow["HType"].ToString();
            }

        }


        LBtn_ThumbsUpM.Text = "<span class='ti-thumb-up mr-2'></span>" + gThumbsUp.ToString("N0");
        LBtn_HeartM.Text = "<span class='ti-heart mr-2'></span>" + gLove.ToString("N0");
        LBtn_SmileM.Text = "<span class='ti-face-smile mr-2'></span>" + gSmile.ToString("N0");

        if (!string.IsNullOrEmpty(gUserMood))
        {
            if (gUserMood == "1")
            {
                LBtn_ThumbsUpM.Text = "<span class='ti-thumb-up mr-2 text-info font-weight-bold'></span>" + gThumbsUp.ToString("N0");
            }
            else if (gUserMood == "2")
            {
                LBtn_HeartM.Text = "<span class='fa fa-heart mr-2 text-danger font-weight-bold'></span>" + gLove.ToString("N0");
            }
            else if (gUserMood == "3")
            {
                LBtn_SmileM.Text = "<span class='ti-face-smile  mr-2 text-success font-weight-bold'></span>" + gSmile.ToString("N0");
            }
        }


        //string gSum = Convert.ToString(gThumbsUp + gLove + gSmile);

        //return gSum + "," + gUserMood;
        return gThumbsUp + "," + gLove + "," + gSmile + "," + gUserMood;


    }
    #endregion

    #region 回應數量計算
    private int ResponseCounts(string SCTopicID, string Source)
    {
        int gResponse = 0;

        if (Source == "1")
        {
            //資料讀取(新版專欄數量)
            SqlDataReader QueryResponse = SQLdatabase.ExecuteReader("SELECT Count(HID) AS Num FROM HSCTMsg  WHERE HSCTopicID='" + SCTopicID + "' AND HStatus=1 UNION ALL SELECT Count(a.HID) AS Num FROM HSCTMsg AS a  JOIN HSCTMsgResponse AS b ON a.HID= b.HSCTMsgID WHERE a.HSCTopicID='" + SCTopicID + "' AND a.HStatus=1 ");

            while (QueryResponse.Read())
            {
                gResponse += Convert.ToInt32(QueryResponse["Num"].ToString());
            }
            QueryResponse.Close();
        }
        else if (Source == "2")
        {
            //資料讀取(EIP回應數量)
            using (var mysqlConn = new MySqlConnection(EIPconnStr))
            {
                mysqlConn.Open();

                using (var mysqlCmd = new MySqlCommand(@"
        SELECT COUNT(*) AS Num 
        FROM heip.laoshireply AS b 
        WHERE lr_lid = LPAD(@SCTopicID, 10, '0')  
          AND lr_Status <9 AND lr_parent =''", mysqlConn))
                {
                    // 加參數
                    mysqlCmd.Parameters.AddWithValue("@SCTopicID", SCTopicID);

                    using (var reader = mysqlCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            gResponse += Convert.ToInt32(reader["Num"]);
                        }
                    }
                }
            }

        }


        return gResponse;
    }
    #endregion

    #region 瀏覽數量計算
    private int ViewCounts(string SCTopicID)
    {
        //資料讀取
        SqlDataReader QueryView = SQLdatabase.ExecuteReader("SELECT HTimes FROM HSCTopic_View WHERE HSCTopicID='" + SCTopicID + "' AND HStatus=1 ");

        int gView = 0;

        while (QueryView.Read())
        {
            gView += Convert.ToInt32(QueryView["HTimes"].ToString());
        }
        QueryView.Close();

        //LB_ViewNum.Text = gView.ToString("N0");

        return gView;
    }
    #endregion

    #region 分享數量計算
    private int ShareCounts(string SCTopicID)
    {
        //資料讀取
        SqlDataReader QueryShare = SQLdatabase.ExecuteReader("SELECT HTimes FROM HSCTopic_Share WHERE HSCTopicID='" + SCTopicID + "' AND HStatus=1 ");

        int gView = 0;

        while (QueryShare.Read())
        {
            gView += Convert.ToInt32(QueryShare["HTimes"].ToString());
        }
        QueryShare.Close();

        //LB_ViewNum.Text = gView.ToString("N0");

        return gView;
    }
    #endregion


    #region 瀏覽紀錄
    protected void LBtn_Seen_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Seen = sender as LinkButton;
        string gSeen_CA = LBtn_Seen.CommandArgument;

        LB_SCTopicID.Text = gSeen_CA;
        LB_Head.Text = LBtn_Seen.CommandName + "<br/>讀訊人數";

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_SeenRecord').modal('show');</script>", false);//執行js


        System.Data.DataTable dtHSCTopic_View = SQLdatabase.ExecuteDataTable("SELECT (b.HArea +' '+ISNULL(b.HSystemName,'無體系')  +' / '+HPeriod+' '+HUserName)  AS HUserName, a.HTimes, a.HCreateDT AS FirstTime, a.HModifyDT AS LatestTime FROM HSCTopic_View AS a JOIN MemberList AS b ON a.HMemberID= b.HID WHERE a.HStatus=1 AND a.HSCTopicID='" + gSeen_CA + "'");

        Rpt_HSCTopic_View.DataSource = dtHSCTopic_View;
        Rpt_HSCTopic_View.DataBind();


        SDS_HSCTopic.SelectCommand = gQuerySCTopic;
        Rpt_HSCTopic.DataBind();

    }
    #endregion

    protected void Rpt_HSCTopic_View_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        if (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_LatestTime")).Text))
        {
            ((Label)e.Item.FindControl("LB_LatestTime")).Text = "-";
        }


    }


    #region 按讚數量名單查看
    protected void LBtn_ThumbsUpNum_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_MoodModal = sender as LinkButton;
        string gMood_CA = LBtn_MoodModal.CommandArgument;

        LB_SCTopicID.Text = gMood_CA;

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

        //預設顯示按讚的資訊
        LBtn_ThumbsUpM.CssClass = "nav-link active show font-weight-bold";
        LBtn_HeartM.CssClass = "nav-link font-weight-bold";
        LBtn_SmileM.CssClass = "nav-link font-weight-bold";

        SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + gMood_CA + "' AND a.HType='1' AND a.HStatus=1";


        //資料繫結
        //SDS_HSCTopic.SelectCommand = gQuerySCTopic;
        Rpt_HSCTopic.DataBind();

        //計算數量
        FeelingCounts(gMood_CA);
    }
    #endregion


    #region 按愛心數量名單查看
    protected void LBtn_LoveNum_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_MoodModal = sender as LinkButton;
        string gMood_CA = LBtn_MoodModal.CommandArgument;

        LB_SCTopicID.Text = gMood_CA;

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

        //預設顯示按讚的資訊
        LBtn_ThumbsUpM.CssClass = "nav-link font-weight-bold";
        LBtn_HeartM.CssClass = "nav-link active show  font-weight-bold";
        LBtn_SmileM.CssClass = "nav-link font-weight-bold";

        SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + gMood_CA + "' AND a.HType='2' AND a.HStatus=1";


        //資料繫結
        //SDS_HSCTopic.SelectCommand = gQuerySCTopic;
        Rpt_HSCTopic.DataBind();

        //計算數量
        FeelingCounts(gMood_CA);
    }
    #endregion


    #region 按笑臉數量名單查看
    protected void LBtn_SmileNum_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_MoodModal = sender as LinkButton;
        string gMood_CA = LBtn_MoodModal.CommandArgument;

        LB_SCTopicID.Text = gMood_CA;

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

        //預設顯示按讚的資訊
        LBtn_ThumbsUpM.CssClass = "nav-link font-weight-bold";
        LBtn_HeartM.CssClass = "nav-link  font-weight-bold";
        LBtn_SmileM.CssClass = "nav-link active show font-weight-bold";

        SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + gMood_CA + "' AND a.HType='3' AND a.HStatus=1";


        //資料繫結
        //SDS_HSCTopic.SelectCommand = gQuerySCTopic;
        Rpt_HSCTopic.DataBind();

        //計算數量
        FeelingCounts(gMood_CA);
    }
    #endregion


    #region 只顯示可參與
    protected void Btn_Participate_Click(object sender, EventArgs e)
    {
        //Response.Write("A");

        if (CB_Participate.Checked == true)
        {
            //Response.Write("B");

            if (!string.IsNullOrEmpty(((Label)Master.FindControl("LB_HUserHID")).Text))
            {
                string StrHType = null;

                SqlDataReader QueryHIRestriction = SQLdatabase.ExecuteReader("SELECT HType FROM HMember WHERE HID=" + ((Label)Master.FindControl("LB_HUserHID")).Text);

                if (QueryHIRestriction.Read())
                {
                    if (QueryHIRestriction["HType"].ToString() != "0")
                    {
                        StrHType = QueryHIRestriction["HType"].ToString();
                    }
                }
                QueryHIRestriction.Close();

                //SDS_HSCTopic.SelectCommand = "SELECT a.HID,   a.HCourseID, c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , (ISNULL(e.HSystemName,'無體系')+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName,  IIF(e.HID IS NULL,'0',e.HID) AS HMemberID,e.HImg, a.HCreateDT, a.HModifyDT, d.HCTemplateID FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID LEFT JOIN HCourse_T AS f ON d.HCTemplateID=f.HID WHERE a.HStatus=1 AND f.HIRestriction LIKE '%" + StrHType + "%' ORDER BY IIF(a.HPinTop IS NULL,0,a.HPinTop)  DESC, IIF(a.HModifyDT IS NOT NULL,a.HModifyDT, a.HCreateDT) DESC";

                SDS_HSCTopic.SelectCommand = gQuerySCTopic + " WHERE a.HStatus=1 AND f.HIRestriction LIKE '%" + StrHType + "%' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName , a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY IIF(a.HPinTop IS NULL,0,a.HPinTop)  DESC, IIF(a.HModifyDT IS NOT NULL,a.HModifyDT, a.HCreateDT) DESC";

            }

            Rpt_HSCTopic.DataBind();

        }
    }
    #endregion

    #region 只顯示已報名(OLD)
    //protected void Btn_Booking_Click(object sender, EventArgs e)
    //{
    //    if (CB_Booking.Checked)
    //    {
    //        string userID = ((Label)Master.FindControl("LB_HUserHID")).Text.Trim();
    //        if (!string.IsNullOrEmpty(userID))
    //        {
    //            SqlDataReader dr = SQLdatabase.ExecuteReader(
    //                "SELECT HCoursename FROM HCourseBooking " +
    //                "WHERE HMemberID = '" + userID + "' " +
    //                "AND HStatus = 1 AND HItemStatus = 1 AND HCourseDonate = '0'"
    //            );

    //            List<string> courseNames = new List<string>();
    //            while (dr.Read())
    //            {
    //                courseNames.Add(dr["HCourseName"].ToString().Trim().Replace("'", "''")); // 加上 SQL 安全處理
    //            }
    //            dr.Close();

    //            DataTable dtHSCTopic = Session["UnifiedTable"] as DataTable;

    //            if (dtHSCTopic != null && courseNames.Count > 0)
    //            {
    //                // 建立過濾條件
    //                List<string> filters = new List<string>();
    //                foreach (string name in courseNames)
    //                {
    //                    filters.Add("HTopicName LIKE '%" + name + "%'");
    //                }
    //                string filter = string.Join(" OR ", filters);

    //                DataRow[] matchedRows = dtHSCTopic.Select(filter);

    //                if (matchedRows.Length > 0)
    //                {
    //                    DataTable resultTable = matchedRows.CopyToDataTable();
    //                    Rpt_HSCTopic.DataSource = resultTable;
    //                    Rpt_HSCTopic.DataBind();

    //                    Panel_NoResult.Visible = false;
    //                    Rpt_HSCTopic.Visible = true;

    //                }
    //                else
    //                {
    //                    // 沒找到資料
    //                    Rpt_HSCTopic.DataSource = null;
    //                    Rpt_HSCTopic.DataBind();

    //                    Panel_NoResult.Visible = true;
    //                    LB_Nodata.Text = "您目前沒有已報名課程的相關討論區哦~";
    //                }
    //            }
    //            else
    //            {
    //                // 沒有任何課程名稱
    //                Panel_NoResult.Visible = true;
    //                LB_Nodata.Text = "您目前沒有已報名課程的相關討論區哦~";

    //            }
    //        }
    //    }
    //    else
    //    {
    //        DataTable dtHSCTopic = Session["UnifiedTable"] as DataTable;
    //        Rpt_HSCTopic.DataSource = dtHSCTopic;
    //        Rpt_HSCTopic.DataBind();
    //        Rpt_HSCTopic.Visible = true;
    //    }

    //    //    if (CB_Booking.Checked == true)
    //    //    {
    //    //        if (!string.IsNullOrEmpty(((Label)Master.FindControl("LB_HUserHID")).Text))
    //    //        {
    //    //            SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HCourseName FROM HCourseBooking " +
    //    //"WHERE HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'" +
    //    //"AND HStatus = 1 AND HItemStatus = 1 AND HCourseDonate = '0'");
    //    //            if (dr.Read())
    //    //            {
    //    //                // 定義要比對的關鍵字
    //    //                string keyword = dr["HCourseName"].ToString();
    //    //                DataTable dtHSCTopic = Session["UnifiedTable"] as DataTable;

    //    //                //GridView1.DataSource = dtHSCTopic;
    //    //                //GridView1.DataBind();
    //    //                // 在 Topic 欄位中模糊搜尋關鍵字
    //    //                DataRow[] matchedRows = dtHSCTopic.Select(" HTopicName LIKE '%" + keyword + "%'");


    //    //                if (matchedRows.Length > 0)
    //    //                {
    //    //                    // 有找到 → 只顯示比對到的資料
    //    //                    DataTable resultTable = matchedRows.CopyToDataTable();
    //    //                    Rpt_HSCTopic.DataSource = resultTable;
    //    //                    Rpt_HSCTopic.DataBind();
    //    //                }
    //    //                else
    //    //                {
    //    //                    Rpt_HSCTopic.DataSource = "";
    //    //                    Rpt_HSCTopic.DataBind();
    //    //                }
    //    //            }
    //    //            dr.Close();


    //    //        }
    //    //    }
    //}
    #endregion




    #region 顯示可參與&已報名內容切換
    // ================= 後端：主流程 =================
    protected void Btn_ShowContent_Click(object sender, EventArgs e)
    {
        ShowContentLinqBoolFirst();
    }

    private void ShowContentLinqBoolFirst()
    {
        // 共同來源表（整併後的 UnifiedTable）
        DataTable dtHSCTopic = Session["UnifiedTable"] as DataTable;
        if (dtHSCTopic == null)
        {
            BindEmpty("目前沒有可顯示的討論區資料。");
            return;
        }

        bool showBooking = CB_Booking.Checked;          // 已報名
        bool showParticipate = CB_Participate.Checked;  // 可參與

        // 取登入者 HType（供 HIRestriction 規則用）
        int loginHType = GetLoginMemberHType();

        // 已報名課程名稱清單（參數化）
        List<string> bookedCourseNames = showBooking ? GetBookedCourseNames() : new List<string>();

        // DataTable -> IEnumerable<DataRow>
        IEnumerable<DataRow> queryAll = dtHSCTopic.AsEnumerable();

        // 先做「是否有符合（true/false）」的布林預判（僅針對已報名）
        bool hasBookedTopic = false;
        if (showBooking && bookedCourseNames.Count > 0)
        {
            // hasBookedTopic：是否「至少有一筆」討論主題，其標題(HTopicName)含有使用者已報名的任一課程名稱
            // 使用 Enumerable.Any(…) 可在找到第一筆符合即停止（效率比先取清單後 Count>0 更好）
            hasBookedTopic = queryAll.Any(r =>
            {
                // 取出當前列的主題標題文字；SafeString 會處理 null/空白與 Trim
                string name = SafeString(r, "HTopicName");

                // 若主題沒有標題，當列必定不符合
                if (string.IsNullOrEmpty(name)) return false;

                // 逐一檢查「使用者已報名的課程名稱」清單
                // 只要主題標題包含任一課程名稱（不分大小寫）就算吻合
                for (int i = 0; i < bookedCourseNames.Count; i++)
                {

                    //Response.Write("A="+ bookedCourseNames[i]+"<br/>");

                    string cn = bookedCourseNames[i];

                    // 跳過空白的課程名稱；使用 IndexOf + StringComparison.OrdinalIgnoreCase 避免大小寫影響
                    if (!string.IsNullOrEmpty(cn) &&
                        name.IndexOf(cn, StringComparison.OrdinalIgnoreCase) >= 0)

                        // Response.Write("B=" + name.IndexOf(cn, StringComparison.OrdinalIgnoreCase) + "<br/>");

                        return true;
                }
                return false;
            });

            // 只勾已報名且完全沒符合 → 直接提示並返回
            if (showBooking && !showParticipate && !hasBookedTopic)
            {
                BindEmpty("您目前沒有已報名課程的相關討論區哦~");
                return;
            }
        }

        // 建立實際查詢
        IEnumerable<DataRow> qBooking = Enumerable.Empty<DataRow>();
        IEnumerable<DataRow> qParticipate = Enumerable.Empty<DataRow>();

        if (showBooking && bookedCourseNames.Count > 0)
        {
            qBooking =
                from r in queryAll
                let name = SafeString(r, "HTopicName")
                where !string.IsNullOrEmpty(name) &&
                      bookedCourseNames.Any(cn =>
                          !string.IsNullOrEmpty(cn) &&
                          name.IndexOf(cn, StringComparison.OrdinalIgnoreCase) >= 0)
                select r;
        }

        if (showParticipate)
        {
            // 【可參與】：只檢查 HIRestriction + 登入者 HType
            qParticipate =
                from r in queryAll
                where CanParticipateByRestriction(r, loginHType)
                select r;
        }

        //兩個都有勾，排除重複
        const string PK = "HID";  //HTopicHID
        IEnumerable<DataRow> resultRows;
        if (showBooking && showParticipate)
        {
            resultRows = qBooking.Concat(qParticipate)
                                 .GroupBy(r => r.Field<object>(PK))
                                 .Select(g => g.First());
        }
        else if (showBooking)  //已報名
        {
            resultRows = qBooking;
        }
        else if (showParticipate)  //可參與
        {
            resultRows = qParticipate;
        }
        else
        {
            resultRows = queryAll; // 兩個都沒勾 → 顯示全部
        }

        // 將已報名課程名稱清單存到 Session，採不分大小寫的 HashSet，加速後續查核
        if (bookedCourseNames != null && bookedCourseNames.Count > 0)
        {
            Session["UserBookedCourseNames"] =
                new HashSet<string>(
                    bookedCourseNames.Where(s => !string.IsNullOrWhiteSpace(s)),
                    StringComparer.OrdinalIgnoreCase // 大小寫不敏感
                );
        }
        else
        {
            Session["UserBookedCourseNames"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }



        // 轉 DataTable
        DataTable result = RowsToTable(dtHSCTopic, resultRows);

        //勾「可參與」時：列表可見、內容遮蔽
        if (showParticipate)
        {
            MaskContentForParticipate(result, "HContent"); // 清空或改提示文字
            EnsureColumn<int>(result, "PreviewOnly");
            for (int i = 0; i < result.Rows.Count; i++)
                result.Rows[i]["PreviewOnly"] = 1;
        }

        // 綁定
        if (result.Rows.Count > 0)
        {
            Panel_NoResult.Visible = false;
            LB_Nodata.Text = "";
            Rpt_HSCTopic.DataSource = result;
            Rpt_HSCTopic.DataBind();
            Rpt_HSCTopic.Visible = true;

        }
        else
        {
            Rpt_HSCTopic.DataSource = null;
            Rpt_HSCTopic.DataBind();
            Panel_NoResult.Visible = true;
            LB_Nodata.Text = showBooking
                ? "您目前沒有已報名課程的相關討論區哦~"
                : "目前沒有可參與的討論區哦~";
        }
    }

    // ================= 規則：HIRestriction + HType =================
    // 範例：HIRestriction <= 0 → 無限制；>0 → 僅允許白名單 HType
    private static readonly HashSet<int> AllowedHTypeForRestricted = new HashSet<int>(new[] { 1, 2 }); // TODO: 換成你制度允許的 HType

    private bool CanParticipateByRestriction(DataRow r, int userHType)
    {
        int hi = ToInt(r, "HIRestriction");
        if (hi <= 0) return true; // 無限制
        return AllowedHTypeForRestricted.Contains(userHType);
    }

    // ================= 登入者 HType =================
    private int GetLoginMemberHType()
    {
        string userID = ((Label)Master.FindControl("LB_HUserHID")).Text.Trim();
        if (string.IsNullOrEmpty(userID)) return 0;

        using (SqlConnection conn = new SqlConnection(SQLdatabase.ConnectionString))
        using (SqlCommand cmd = new SqlCommand(@"
        SELECT TOP 1 HType
        FROM MemberList
        WHERE HID = @uid
    ", conn))
        {
            cmd.Parameters.AddWithValue("@uid", userID);
            conn.Open();
            object o = cmd.ExecuteScalar();
            int v; return (o != null && int.TryParse(o.ToString(), out v)) ? v : 0;
        }
    }

    // ================= 已報名課程名稱（參數化） =================
    private List<string> GetBookedCourseNames()
    {
        var list = new List<string>();
        string userID = ((Label)Master.FindControl("LB_HUserHID")).Text.Trim();
        if (string.IsNullOrEmpty(userID)) return list;

        using (SqlConnection conn = new SqlConnection(SQLdatabase.ConnectionString))
        using (SqlCommand cmd = new SqlCommand(@"
        SELECT TOP(15) HCourseName
        FROM HCourseBooking
        WHERE HMemberID = @uid
          AND HStatus = 1
          AND HItemStatus = 1
          AND HCourseDonate = '0' ORDER BY HID DESC
    ", conn))
        {
            cmd.Parameters.AddWithValue("@uid", userID);
            conn.Open();
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    string name = (dr["HCourseName"] == DBNull.Value) ? "" : dr["HCourseName"].ToString().Trim();
                    if (!string.IsNullOrEmpty(name)) list.Add(name);
                }
            }
        }
        return list;
    }

    // ================= 工具方法 =================
    private string SafeString(DataRow r, string col)
    {
        return r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? r[col].ToString().Trim() : "";
    }

    private int ToInt(DataRow r, string col)
    {
        if (!r.Table.Columns.Contains(col) || r[col] == DBNull.Value) return 0;
        int v; return int.TryParse(r[col].ToString(), out v) ? v : 0;
    }

    private DataTable RowsToTable(DataTable schema, IEnumerable<DataRow> rows)
    {
        DataTable dt = schema.Clone();
        foreach (var rr in rows) dt.ImportRow(rr);
        return dt;
    }

    private void EnsureColumn<T>(DataTable t, string col)
    {
        if (!t.Columns.Contains(col)) t.Columns.Add(col, typeof(T));
    }

    private void MaskContentForParticipate(DataTable t, string contentColumn)
    {
        if (!t.Columns.Contains(contentColumn)) return;
        for (int i = 0; i < t.Rows.Count; i++)
        {
            // 可改為固定提示：t.Rows[i][contentColumn] = "請完成報名或符合資格後查看內容";
            t.Rows[i][contentColumn] = "";
        }
    }

    private void BindEmpty(string msg)
    {
        Rpt_HSCTopic.DataSource = null;
        Rpt_HSCTopic.DataBind();
        Panel_NoResult.Visible = true;
        LB_Nodata.Text = msg;
    }
    #endregion


    #region 立即報名課程導頁
    protected string ToCourseListUrl(object title)
    {
        string gTitle = "";

        if (!title.ToString().Contains("【回應窗口】")) 
        {
            string[] parts = title.ToString().Split('_');
          
            if (parts.Length > 1 && parts.Length <= 3)
            {
                // 超過 2 個底線，取前三段
                gTitle = parts[0] + "_" + parts[1];
            }
            else if (parts.Length > 3)
            {
                // 剛好有 1 或 2 個底線，取前兩段
                gTitle = parts[0] + "_" + parts[1] + "_" + parts[2];
            }
            else
            {
                // 沒有底線，就直接用原始字串
                gTitle = parts[0];
            }
        }
        else
        {
            // 1. 去掉前綴 "【回應窗口】" + 8 碼日期
            string result = title.ToString().Substring("【回應窗口】".Length + 8);

            // 2. 以 "Day" 分割，只取前半段
            string[] daySplit = result.Split(new string[] { "Day" }, StringSplitOptions.None);
            result = daySplit[0];

            // 3. 去掉最後一個 "_"
            if (result.EndsWith("_"))
            {
                result = result.Substring(0, result.Length - 1);
            }

            gTitle = result;
        }



            //string gTitle = title.ToString().Split('_')[0]+ "_"+title.ToString().Split('_')[1];
            var t = Convert.ToString(gTitle ?? "");
        return ResolveUrl("~/HCourseList.aspx?q=" + System.Web.HttpUtility.UrlEncode(t));
    }
    #endregion
}




