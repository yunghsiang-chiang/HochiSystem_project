using DocumentFormat.OpenXml.Spreadsheet;
using MySql.Data.MySqlClient;
using NPOI.SS.Formula.Functions;
using System;
using System.Activities.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Util;

public partial class HSCIndex : System.Web.UI.Page
{
    string EIPconnStr = ConfigurationManager.ConnectionStrings["HochiEIPConnection"].ConnectionString;
    SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);
    string Con = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;


    string gQuerySCTopic = "SELECT TOP(30) a.HID, a.HCourseID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , (ISNULL(e.HSystemName,'無體系')+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, IIF(e.HID IS NULL,'0',e.HID) AS HMemberID,e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName ,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY IIF(a.HPinTop IS NULL,0,a.HPinTop)  DESC, IIF(a.HModifyDT IS NOT NULL,a.HModifyDT, a.HCreateDT) DESC";

    #region --根目錄--
    string SCFileRoot = "D:\\Website\\System\\HochiSystem\\uploads\\SpecialColumn\\";
    string SCFile = "~/uploads/SpecialColumn/";
    #endregion

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        string Select = "SELECT a.HID,  a.HCourseID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , (ISNULL(e.HSystemName,'無體系')+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, IIF(e.HID IS NULL,'0',e.HID) AS HMemberID, e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID  LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID LEFT JOIN HCourse_T AS f ON d.HCTemplateID=f.HID";

        StringBuilder sql = new StringBuilder(Select);
        List<string> WHERE = new List<string>();


        if (Rpt_HSCTopic.Items.Count == 0)
        {
            if (Session["SearchType"] != null)
            {
                if (Session["SearchType"].ToString() == "1")
                {
                    Panel_NoResult.Visible = true;
                    LB_Nodata.Text = "今日尚未有新的主題哦~請重新搜尋~謝謝!";
                }

            }
        }

    }

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
        //var coreMatch = Regex.Match(input, @"([\u4e00-\u9fa5A-Za-z0-9]+_[\u4e00-\u9fa5A-Za-z0-9]+)");
        var coreMatch = Regex.Match(input, @"([\u4e00-\u9fa5A-Za-z0-9]{1,30})_Day\d+");
        if (coreMatch.Success)
            result.Core = coreMatch.Value;

        return result;
    }

    protected void Page_Load(object sender, EventArgs e)
    {


        if (!IsPostBack)
        {

            


            //Response.Write("SearchIndex=" + Session["SearchIndex"].ToString()+"<br/>");
            //Response.Write("SearchIndexEIP="+ Session["SearchIndexEIP"].ToString());
            //Response.End();


            Session["SCTopicID"] = LB_SCTopicID.Text; // 存到 Session


            //判斷全站的類型 Session["SearchType"]=0:進階搜尋 / 1:最新主題 / 2:熱門主題
            if (Session["SearchType"] != null)
            {
                if (Session["SearchType"].ToString() == "0")
                {
                    ((LinkButton)Master.FindControl("LBtn_AdvancedSearch")).CssClass = "mr-2 btn btn-outline-purple btn-rounded btn-special";
                    ((LinkButton)Master.FindControl("LBtn_NewSearch")).CssClass = "mr-2 btn btn-outline-purple btn-rounded";
                    ((LinkButton)Master.FindControl("LBtn_HotSearch")).CssClass = "mr-2 btn btn-outline-purple btn-rounded";
                }
                else if (Session["SearchType"].ToString() == "1")
                {
                    ((LinkButton)Master.FindControl("LBtn_AdvancedSearch")).CssClass = "mr-2 btn btn-outline-purple btn-rounded";
                    ((LinkButton)Master.FindControl("LBtn_NewSearch")).CssClass = "mr-2 btn btn-outline-purple btn-rounded btn-special";
                    ((LinkButton)Master.FindControl("LBtn_HotSearch")).CssClass = "mr-2 btn btn-outline-purple btn-rounded";
                }
                else if (Session["SearchType"].ToString() == "2")
                {
                    ((LinkButton)Master.FindControl("LBtn_AdvancedSearch")).CssClass = "mr-2 btn btn-outline-purple btn-rounded";
                    ((LinkButton)Master.FindControl("LBtn_HotSearch")).CssClass = "mr-2 btn btn-outline-purple btn-rounded";
                    ((LinkButton)Master.FindControl("LBtn_HotSearch")).CssClass = "mr-2 btn btn-outline-purple btn-rounded btn-special";
                }
            }
            else
            {

                //Response.Write("AAA<br/>");

                ((LinkButton)Master.FindControl("LBtn_AdvancedSearch")).CssClass = "mr-2 btn btn-outline-purple btn-rounded";
                ((LinkButton)Master.FindControl("LBtn_HotSearch")).CssClass = "mr-2 btn btn-outline-purple btn-rounded";
                ((LinkButton)Master.FindControl("LBtn_HotSearch")).CssClass = "mr-2 btn btn-outline-purple btn-rounded";

                //SDS_HSCTopic.SelectCommand = gQuerySCTopic;
                //Rpt_HSCTopic.DataBind();


                #region 整合EIP資料顯示
                //新版專欄資料
                // 1. 撈 SQL Server 資料
                //var dtRecords = SQLdatabase.ExecuteDataTable(@"SELECT a.HID, a.HCourseID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , (ISNULL(e.HSystemName, '無體系') + ' ' + e.HArea + '/' + e.HPeriod + ' ' + e.HUserName) AS UserName, IIF(e.HID IS NULL, '0', e.HID) AS HMemberID, e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID = b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster = c.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID = d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus = 1 AND HTopicName LIKE '%' + CONVERT(varchar(6), GETDATE(), 12) + '%' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName ,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY IIF(a.HPinTop IS NULL,0,a.HPinTop)  DESC, IIF(a.HModifyDT IS NOT NULL, a.HModifyDT, a.HCreateDT) DESC");


                var dtRecords = SQLdatabase.ExecuteDataTable(@"SELECT 
    a.HID,
    a.HCourseID,
    c.HSCFCName AS HSCForumClassB,
    b.HSCFCName AS HSCForumClassC,
    a.HTopicName,
    a.HPinTop,
    a.HContent,
    a.HFile1,
    (ISNULL(e.HSystemName, '無體系') + ' ' + e.HArea + '/' + e.HPeriod + ' ' + e.HUserName) AS UserName,
    IIF(e.HID IS NULL, '0', e.HID) AS HMemberID,
    e.HImg,
    a.HCreateDT,
    a.HModifyDT,
    h.HIRestriction
FROM HSCTopic AS a
LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID = b.HID
LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster = c.HID
LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID = d.HSCForumClassID
LEFT JOIN MemberList AS e ON a.HCreate = e.HID
OUTER APPLY (
    SELECT TOP 1 LTRIM(RTRIM(x.n.value('.', 'varchar(50)'))) AS CourseIDStr
    FROM (SELECT CAST('<i>' + REPLACE(ISNULL(a.HCourseID,''), ',', '</i><i>') + '</i>' AS xml) AS xdoc) AS t
    CROSS APPLY t.xdoc.nodes('/i') AS x(n)
    WHERE LTRIM(RTRIM(x.n.value('.', 'varchar(50)'))) <> ''
) ss
LEFT JOIN HCourse AS h 
    ON h.HID = CASE WHEN ISNUMERIC(ss.CourseIDStr) = 1 THEN CAST(ss.CourseIDStr AS int) ELSE NULL END
WHERE 
    a.HStatus = 1
    AND a.HTopicName LIKE '%' + CONVERT(varchar(6), GETDATE(), 12) + '%'
	GROUP BY a.HID,
    a.HCourseID,
    c.HSCFCName,
    b.HSCFCName,
    a.HTopicName,
    a.HPinTop,
    a.HContent,
    a.HFile1,
   e.HSystemName,e.HArea, e.HPeriod, e.HUserName,
   e.HID,
    e.HImg,
    a.HCreateDT,
    a.HModifyDT,
    h.HIRestriction
ORDER BY 
    IIF(a.HPinTop IS NULL, 0, a.HPinTop) DESC,
    IIF(a.HModifyDT IS NOT NULL, a.HModifyDT, a.HCreateDT) DESC;");


                //Response.Write("SELECT a.HID, a.HCourseID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , (ISNULL(e.HSystemName, '無體系') + ' ' + e.HArea + '/' + e.HPeriod + ' ' + e.HUserName) AS UserName, IIF(e.HID IS NULL, '0', e.HID) AS HMemberID, e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID = b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster = c.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID = d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus = 1 AND HTopicName LIKE '%' + CONVERT(varchar(6), GETDATE(), 12) + '%' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName ,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY IIF(a.HPinTop IS NULL,0,a.HPinTop)  DESC, IIF(a.HModifyDT IS NOT NULL, a.HModifyDT, a.HCreateDT) DESC <br/>");

                // 2. 撈 MySQL 資料
                //var mysqlConn = new MySqlConnection("Server =61.220.221.250;Database=heip;Uid=GwUser;Pwd=!@#Gw2025;Charset=utf8mb4;SslMode=none;");
                var mysqlConn = new MySqlConnection(EIPconnStr);
                //var mysqlCmd = new MySqlCommand("SELECT  l_id AS HID, 'EIP' AS HSCForumClassB, 'EIP' AS HSCForumClassC,  l_title AS Title, l_Content AS Content, l_recorddate AS HCreateDT,  l_cname AS UserName FROM heip.laoshi WHERE l_status=2 AND l_category=0 AND l_recorddate > DATE_FORMAT(CURDATE(), '%Y%m%d%H%i%S')  AND l_recorddate < DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 15 HOUR), '%Y%m%d%H%i%S')  ORDER BY l_no DESC", mysqlConn);

                var mysqlCmd = new MySqlCommand("SELECT  l_id AS HID, '' AS HCourseID, 'EIP' AS HSCForumClassB, 'EIP' AS HSCForumClassC,  l_title AS HTopicName, l_Content AS HContent, l_recorddate AS HCreateDT,  l_cname AS UserName, '' AS HIRestriction FROM heip.laoshi WHERE l_status=2 AND l_category=0 AND l_title NOT LIKE '%成長紀錄%' AND l_recorddate > DATE_FORMAT(CURDATE(), '%Y%m%d%H%i%S') AND l_recorddate < DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 15 HOUR), '%Y%m%d%H%i%s')", mysqlConn);


                // Response.Write("SELECT  l_id AS HID, '' AS HCourseID, 'EIP' AS HSCForumClassB, 'EIP' AS HSCForumClassC,  l_title AS Title, l_Content AS Content, l_recorddate AS HCreateDT,  l_cname AS UserName FROM heip.laoshi WHERE l_status=2 AND l_category=0 AND l_title NOT LIKE '%成長紀錄%' AND l_recorddate > DATE_FORMAT(CURDATE(), '%Y%m%d%H%i%S') AND l_recorddate < DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 15 HOUR), '%Y%m%d%H%i%s') < br/>");


                /*  l_recorddate LIKE CONCAT('%', DATE_FORMAT(CURRENT_DATE(), '%y%m%d'), '%') AND l_recorddate < DATE_FORMAT(DATE_ADD(NOW(), INTERVAL 15 HOUR), '%y%m%d%H%i%s') */
                var mysqlAdapter = new MySqlDataAdapter(mysqlCmd);
                System.Data.DataTable mysqlRecords = new System.Data.DataTable();
                mysqlAdapter.Fill(mysqlRecords);


                //GridView1.DataSource = mysqlRecords;
                //GridView1.DataBind();

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
                    int brIndex = row["HTopicName"].ToString().IndexOf("br", StringComparison.OrdinalIgnoreCase);
                    string title = row["HTopicName"].ToString();
                    string extra = "";

                    if (brIndex >= 0)
                    {
                        title = row["HTopicName"].ToString().Substring(0, brIndex); // 取 br 前作為新標題
                        extra = row["HTopicName"].ToString().Substring(brIndex + 2); // 剩下的移到內容（跳過 br 共4字元）
                    }
                    string content = extra.Replace("br", "<br/>") + row["HContent"].ToString();
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

                            // 記錄 EIP l_id
                            string oldEip = targetRow.Table.Columns.Contains("EIPID") ? targetRow["EIPID"].ToString() : "";
                            if (!string.IsNullOrEmpty(oldEip))
                            {
                                targetRow["EIPID"] = oldEip + "," + lid;
                            }
                            else
                            {
                                targetRow["EIPID"] = lid;

                            }

                            matched = true;
                            break;
                        }

                    }

                    if (!matched)
                    {
                        try
                        {
                            // 沒有找到相同主題，新增 MySQL 資料為獨立主題
                            DataRow newRow = unifiedTable.NewRow();
                            newRow["HID"] = lid;
                            newRow["HCourseID"] = "";
                            newRow["HSCForumClassB"] = "大愛光老師專欄";
                            newRow["HSCForumClassC"] = "EIP";
                            newRow["HTopicName"] = title;
                            newRow["HPinTop"] = "false";
                            newRow["HContent"] = content;
                            newRow["HFile1"] = "";
                            newRow["UserName"] = "EIP";
                            newRow["HMemberID"] = "0";
                            newRow["HImg"] = "";
                            newRow["HIRestriction"] = ""; //身分限制
                            newRow["HCreateDT"] = row["HCreateDT"];
                            newRow["HModifyDT"] = "";
                            unifiedTable.Rows.Add(newRow);
                        }
                        catch (Exception ex)
                        {
                            Response.Write("加新資料時錯誤: " + ex.Message + "<br/>");
                        }
                    }


                }





                //加入排序條件
                DataView sortedView = unifiedTable.DefaultView;
                sortedView.Sort = "HCreateDT DESC";
                DataTable sortedTable = sortedView.ToTable();



                Session["UnifiedTable"] = sortedTable;

                Rpt_HSCTopic.DataSource = sortedTable;
                Rpt_HSCTopic.DataBind();
                #endregion



            }


            //全站搜尋條件用
            if (Session["SearchIndex"] != null)
            {
                if (Session["SearchIndexEIP"] != null)
                {
                    #region 整合EIP資料顯示_改寫版
                    ///* 前置：你原本的 dtRecords 與 mysqlRecords 已經取好 */
                    //var dtRecords = SQLdatabase.ExecuteDataTable(Session["SearchIndex"].ToString());

                    //var mysqlConn = new MySqlConnection(EIPconnStr);
                    //var mysqlCmd = new MySqlCommand(Session["SearchIndexEIP"].ToString(), mysqlConn);
                    //var mysqlAdapter = new MySqlDataAdapter(mysqlCmd);
                    //System.Data.DataTable mysqlRecords = new System.Data.DataTable();
                    //mysqlAdapter.Fill(mysqlRecords);

                    ///* =============== 共用小工具 =============== */
                    //Func<string, string> Safe = s => s == null ? "" : s;
                    //Func<object, string> AsStr = o => o == null ? "" : o.ToString();

                    //Action<DataTable, string, Type> EnsureCol = (dt, col, tp) =>
                    //{
                    //    if (!dt.Columns.Contains(col)) dt.Columns.Add(col, tp);
                    //};

                    //Func<string, string> NormalizeBrToHtml = s =>
                    //{
                    //    if (string.IsNullOrEmpty(s)) return "";
                    //    // 將各式 br / BR / <br> / <br/> / <BR> / <BR /> 都正規化成 <br/>
                    //    // 先去掉空白再處理常見寫法
                    //    string t = s.Replace("<BR />", "<br/>").Replace("<BR/>", "<br/>").Replace("<BR>", "<br/>")
                    //                .Replace("<br />", "<br/>").Replace("<br>", "<br/>");
                    //    // 若文字中還有裸 "br"（你原碼遇到的情況），再保守替換
                    //    t = t.Replace("BR", "<br/>").Replace("Br", "<br/>").Replace("bR", "<br/>").Replace("br", "<br/>");
                    //    return t;
                    //};

                    //// 解析標題：回傳 Core（主題核心，不含日期）與 Date（yyyyMMdd）
                    //Func<string, Tuple<string, string>> ParseTitle = title =>
                    //{
                    //    string src = Safe(title).Trim();
                    //    string core = src;
                    //    string date8 = "";

                    //    // 規則：若字串前 8 碼皆為數字，當成日期 yyyyMMdd
                    //    if (core.Length >= 8)
                    //    {
                    //        bool allNum = true;
                    //        for (int i = 0; i < 8; i++)
                    //        {
                    //            if (!char.IsDigit(core[i])) { allNum = false; break; }
                    //        }
                    //        if (allNum)
                    //        {
                    //            date8 = core.Substring(0, 8);
                    //            // 切掉前 8 碼；若中間有分隔符號（_ 或 空白等）一併處理
                    //            if (core.Length > 8)
                    //            {
                    //                string rest = core.Substring(8).TrimStart(new char[] { '_', '-', ' ', '．', '.', '、' });
                    //                core = rest;
                    //            }
                    //            else
                    //            {
                    //                core = "";
                    //            }
                    //        }
                    //    }

                    //    // 若還沒抓到日期，再嘗試從字串中找到 8 碼連續數字
                    //    if (date8 == "")
                    //    {
                    //        string buf = "";
                    //        for (int i = 0; i < src.Length; i++)
                    //        {
                    //            char ch = src[i];
                    //            if (char.IsDigit(ch))
                    //            {
                    //                buf += ch;
                    //                if (buf.Length == 8) { date8 = buf; break; }
                    //            }
                    //            else
                    //            {
                    //                buf = "";
                    //            }
                    //        }
                    //    }

                    //    return new Tuple<string, string>(core, date8);
                    //};

                    //// yyyymmddHHMMss -> DateTime；失敗回 DateTime.MinValue
                    //Func<string, DateTime> Parse14 = s =>
                    //{
                    //    s = Safe(s);
                    //    if (s.Length >= 14)
                    //    {
                    //        int yyyy, MM, dd, HH, mm, ss;
                    //        if (int.TryParse(s.Substring(0, 4), out yyyy) &&
                    //            int.TryParse(s.Substring(4, 2), out MM) &&
                    //            int.TryParse(s.Substring(6, 2), out dd) &&
                    //            int.TryParse(s.Substring(8, 2), out HH) &&
                    //            int.TryParse(s.Substring(10, 2), out mm) &&
                    //            int.TryParse(s.Substring(12, 2), out ss))
                    //        {
                    //            try { return new DateTime(yyyy, MM, dd, HH, mm, ss); } catch { }
                    //        }
                    //    }
                    //    return DateTime.MinValue;
                    //};

                    //// 一般字串 -> DateTime；失敗回 DateTime.MinValue
                    //Func<object, DateTime> AsDate = o =>
                    //{
                    //    if (o == null) return DateTime.MinValue;
                    //    DateTime dt;
                    //    if (o is DateTime) return (DateTime)o;
                    //    if (DateTime.TryParse(o.ToString(), out dt)) return dt;
                    //    return DateTime.MinValue;
                    //};

                    //// 產生 Key = core + "_" + date8（全部小寫）
                    //Func<string, string, string> MakeKey = (core, date8) =>
                    //{
                    //    string c = Safe(core).Trim().ToLowerInvariant();
                    //    string d = Safe(date8).Trim();
                    //    return c + "_" + d;
                    //};

                    ///* =============== 建立 unifiedTable（先用 dtRecords.Clone 再補欄位） =============== */
                    //System.Data.DataTable unifiedTable = dtRecords.Clone();
                    //EnsureCol(unifiedTable, "source", typeof(string));
                    //EnsureCol(unifiedTable, "EIPID", typeof(string));
                    //EnsureCol(unifiedTable, "EIP_HID", typeof(string));  // 你後面有用到
                    //EnsureCol(unifiedTable, "SortDT", typeof(DateTime));
                    //EnsureCol(unifiedTable, "PinVal", typeof(int));      // 排序用（若有置頂）

                    //// 確保關鍵欄位存在（避免某些來源沒帶）
                    //EnsureCol(unifiedTable, "HID", typeof(string));
                    //EnsureCol(unifiedTable, "HTopicName", typeof(string));
                    //EnsureCol(unifiedTable, "HContent", typeof(string));
                    //EnsureCol(unifiedTable, "UserName", typeof(string));
                    //EnsureCol(unifiedTable, "HCreateDT", typeof(string));
                    //EnsureCol(unifiedTable, "HPinTop", typeof(string));

                    ///* =============== 建 MSSQL Map：Key 指向 unifiedTable 的 DataRow =============== */
                    //Dictionary<string, DataRow> map = new Dictionary<string, DataRow>();

                    //// 先把 MSSQL 的 dtRecords 加入 unifiedTable，同時建立 map
                    //for (int i = 0; i < dtRecords.Rows.Count; i++)
                    //{
                    //    DataRow s = dtRecords.Rows[i];
                    //    DataRow r = unifiedTable.NewRow();

                    //    // 複製欄位（保守作法：逐欄位複製）
                    //    for (int c = 0; c < dtRecords.Columns.Count; c++)
                    //    {
                    //        string colName = dtRecords.Columns[c].ColumnName;
                    //        r[colName] = s[colName];
                    //    }

                    //    // 強制補齊欄位
                    //    r["source"] = "EDU";
                    //    r["EIPID"] = "";
                    //    r["EIP_HID"] = Safe(AsStr(r["EIP_HID"]));

                    //    // PinVal
                    //    int pinVal = 0;
                    //    string pinStr = AsStr(r["HPinTop"]).Trim();
                    //    int.TryParse(pinStr == "" ? "0" : pinStr, out pinVal);
                    //    r["PinVal"] = pinVal;

                    //    // SortDT
                    //    DateTime sortDt = AsDate(s["HCreateDT"]);
                    //    r["SortDT"] = sortDt;

                    //    // 補 HCreateDT（若 MSSQL 原本是 DateTime，這裡也轉成字串放著以便前端顯示一致）
                    //    if (dtRecords.Columns["HCreateDT"].DataType == typeof(DateTime))
                    //    {
                    //        r["HCreateDT"] = sortDt == DateTime.MinValue ? "" : sortDt.ToString("yyyy-MM-dd HH:mm:ss");
                    //    }

                    //    // 產生 Key（用標題解析）
                    //    var parsed = ParseTitle(AsStr(r["HTopicName"]));
                    //    string key = MakeKey(parsed.Item1, parsed.Item2);

                    //    unifiedTable.Rows.Add(r);

                    //    // 只有當 key 兩段都不為空時才建立 Map
                    //    if (parsed.Item1 != "" && parsed.Item2 != "" && !map.ContainsKey(key))
                    //    {
                    //        map.Add(key, r);
                    //    }
                    //}

                    ///* =============== 合併 MySQL（EIP） =============== */
                    //for (int i = 0; i < mysqlRecords.Rows.Count; i++)
                    //{
                    //    DataRow s = mysqlRecords.Rows[i];

                    //    string lid = AsStr(s["HID"]);  // 你的 EIP SQL 已把 l_id AS HID
                    //    string rawTitle = AsStr(s["HTopicName"]);
                    //    string rawContent = AsStr(s["HContent"]);

                    //    // 只取第一個 <br 之前當標題
                    //    string tTitle = rawTitle;
                    //    string tExtra = "";
                    //    int brIdx = -1;

                    //    // 找 "<br"（大小寫不敏感）
                    //    string low = rawTitle.ToLowerInvariant();
                    //    int idx = low.IndexOf("<br");
                    //    if (idx >= 0)
                    //    {
                    //        tTitle = rawTitle.Substring(0, idx).Trim();
                    //        tExtra = rawTitle.Substring(idx); // 後段保留（含 <br...）
                    //    }

                    //    // 內容：把標題後段（正規化 br）+ 原內容
                    //    string content = NormalizeBrToHtml(tExtra) + NormalizeBrToHtml(rawContent);

                    //    // 解析 Key
                    //    var parsed = ParseTitle(tTitle);
                    //    string key = MakeKey(parsed.Item1, parsed.Item2);

                    //    DataRow target = null;
                    //    if (parsed.Item1 != "" && parsed.Item2 != "" && map.ContainsKey(key))
                    //    {
                    //        target = map[key];
                    //    }

                    //    if (target != null)
                    //    {
                    //        // 合併內容
                    //        string old = AsStr(target["HContent"]);
                    //        string sep = old.Length == 0 ? "" : "\n---\n";
                    //        target["HContent"] = old + sep + content;

                    //        // 累加 EIP_HID
                    //        string oldEip = AsStr(target["EIP_HID"]);
                    //        target["EIP_HID"] = (oldEip == "") ? lid : (oldEip + "," + lid);
                    //    }
                    //    else
                    //    {
                    //        // 新增為獨立主題（EIP）
                    //        DataRow r = unifiedTable.NewRow();

                    //        // 目標欄位填值（盡量對齊 unifiedTable）
                    //        r["HID"] = lid;
                    //        r["HTopicName"] = tTitle;
                    //        r["HContent"] = content;
                    //        r["UserName"] = AsStr(s["UserName"]) == "" ? "EIP" : AsStr(s["UserName"]);
                    //        r["HCreateDT"] = AsStr(s["HCreateDT"]);  // yyyymmddHHMMss
                    //        r["source"] = "EIP";
                    //        r["EIPID"] = lid;
                    //        r["EIP_HID"] = lid;

                    //        // 置頂值（EIP 沒有就 0）
                    //        r["HPinTop"] = "FALSE";
                    //        r["PinVal"] = 0;

                    //        // SortDT（來自 HCreateDT 的 14 碼）
                    //        DateTime eipDt = Parse14(AsStr(s["HCreateDT"]));
                    //        r["SortDT"] = eipDt;

                    //        unifiedTable.Rows.Add(r);

                    //        // 這筆若有合法 key，也建入 map（之後其它 EIP 仍能疊）
                    //        if (parsed.Item1 != "" && parsed.Item2 != "" && !map.ContainsKey(key))
                    //        {
                    //            map.Add(key, r);
                    //        }
                    //    }
                    //}

                    ///* =============== 排序 + 綁定 =============== */
                    //// 用 DataView 排序：先置頂、再由新到舊
                    //DataView dv = new DataView(unifiedTable);
                    //dv.Sort = "PinVal DESC, SortDT DESC";
                    //DataTable sortedTable = dv.ToTable();

                    //// （除錯）你若要看兩邊原始資料，可暫時保留以下兩行：
                    //GridView1.DataSource = unifiedTable; GridView1.DataBind();
                    //GridView2.DataSource = mysqlRecords;  GridView2.DataBind();
                    

                    //Session["SearchIndex"] = null;
                    //Session["SearchIndexEIP"] = null;

                    //Rpt_HSCTopic.DataSource = sortedTable;
                    //Rpt_HSCTopic.DataBind();
                    #endregion




                    #region 整合EIP資料顯示
                    //新版專欄資料
                    // 1. 撈 SQL Server 資料
                    var dtRecords = SQLdatabase.ExecuteDataTable(Session["SearchIndex"].ToString());

                    // 2. 撈 MySQL 資料
                    //var mysqlConn = new MySqlConnection("Server =61.220.221.250;Database=heip;Uid=GwUser;Pwd=!@#Gw2025;Charset=utf8mb4;SslMode=none;");

                    var mysqlConn = new MySqlConnection(EIPconnStr);
                    var mysqlCmd = new MySqlCommand(Session["SearchIndexEIP"].ToString(), mysqlConn);
                    var mysqlAdapter = new MySqlDataAdapter(mysqlCmd);
                    System.Data.DataTable mysqlRecords = new System.Data.DataTable();
                    mysqlAdapter.Fill(mysqlRecords);

                    // 3. 建立 MSSQL 主題索引（以 "主題核心+日期" 為 Key）
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

                    // 4. 建立最終合併結果表 unifiedTable
                    System.Data.DataTable unifiedTable = dtRecords.Clone();
                    //新增欄位
                    unifiedTable.Columns.Add("source", typeof(string));   // 來源
                    unifiedTable.Columns.Add("EIPID", typeof(string));  // 合併進來的 EIP l_id
                    HashSet<string> mergedKeys = new HashSet<string>();






                    // 5. 先將 MSSQL 資料加入 unifiedTable
                    foreach (var row in dtRecords.Rows)
                    {
                        DataRow newRow = unifiedTable.NewRow();
                        newRow.ItemArray = ((DataRow)row).ItemArray.Clone() as object[];
                        //新增欄位
                        newRow["source"] = "EDU";
                        newRow["EIPID"] = ""; // 預設無
                        unifiedTable.Rows.Add(newRow);
                    }

                    //GridView1.DataSource = unifiedTable;
                    //GridView1.DataBind();


                    //GridView2.DataSource = mysqlRecords;
                    //GridView2.DataBind();

                    // 6. 處理 MySQL 資料
                    foreach (DataRow row in mysqlRecords.Rows)
                    {
                        string lid = row["HID"].ToString();

                        //改第一個br前才作為標題
                        int brIndex = row["HTopicName"].ToString().IndexOf("br", StringComparison.OrdinalIgnoreCase);
                        string title = row["HTopicName"].ToString();
                        string extra = "";

                        if (brIndex >= 0)
                        {
                            title = row["HTopicName"].ToString().Substring(0, brIndex); // 取 br 前作為新標題
                            extra = row["HTopicName"].ToString().Substring(brIndex + 2); // 剩下的移到內容（跳過 br 共4字元）
                        }
                        string content = extra.Replace("br", "<br/>") + row["HContent"].ToString();
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

                                // 記錄 EIP l_id
                                string oldEip = targetRow.Table.Columns.Contains("EIP_HID") ? targetRow["EIP_HID"].ToString() : "";
                                if (!string.IsNullOrEmpty(oldEip))
                                {

                                    targetRow["EIP_HID"] = oldEip + "," + lid;

                                    //Response.Write("oldEip=" + oldEip + "lid=" + lid + "<br/>");
                                    //Response.End();
                                }
                                else
                                {
                                    targetRow["EIP_HID"] = lid;

                                }

                                matched = true;
                                break;
                            }

                        }

                        // 放在塞資料的 for/foreach 之前
                        //if (unifiedTable.Columns.Contains("HTopicName"))
                        //{
                        //    // -1 代表不限制長度；或改成你要的上限（例如 200、500）
                        //    unifiedTable.Columns["HTopicName"].MaxLength = -1;
                        //}

                        if (!matched)
                        {
                            // 沒有找到相同主題，新增 MySQL 資料為獨立主題
                            DataRow newRow = unifiedTable.NewRow();

                            newRow["HID"] = lid;

                            newRow["HTopicName"] = title;
                            newRow["HContent"] = content;
                            //newRow["HMemberID"] = luid;
                            newRow["UserName"] = "EIP";
                            newRow["HCreateDT"] = row["HCreateDT"];
                            //newRow["EIPlrID"] = "";
                            unifiedTable.Rows.Add(newRow);
                        }
                        else
                        {
                            // Response.Write("A<br/>");
                        }

                    }



                    //GridView1.DataSource = unifiedTable;
                    //GridView1.DataBind();

                    //加入排序條件
                    DataView sortedView = unifiedTable.DefaultView;
                    sortedView.Sort = "HCreateDT DESC";
                    DataTable sortedTable = sortedView.ToTable();

                    //清除session
                    Session["SearchIndex"] = null;
                    Session["SearchIndexEIP"] = null;

                    Rpt_HSCTopic.DataSource = sortedTable;
                    Rpt_HSCTopic.DataBind();
                    #endregion
                }
                //else
                //{
                //    SDS_HSCTopic.SelectCommand = Session["SearchIndex"].ToString();
                //    Rpt_HSCTopic.DataBind();
                //}

            }
            //else
            //{
            //    SDS_HSCTopic.SelectCommand = gQuerySCTopic;
            //    Rpt_HSCTopic.DataBind();
            //}

      

        }


        if (!IsPostBack)
        {
            //AA240724_選單引用：App_Code/SelectListItem/RBL_ListItem.cs
            RBL_HSCJiugonggeType.DataSource = RBL_ListItem.HSCJiugonggeType.ListItem;
            RBL_HSCJiugonggeType.DataTextField = "Value";
            RBL_HSCJiugonggeType.DataValueField = "Key";
            RBL_HSCJiugonggeType.DataBind();

            RBL_HSCJiugonggeTypeM.DataSource = RBL_ListItem.HSCJiugonggeType.ListItem;
            RBL_HSCJiugonggeTypeM.DataTextField = "Value";
            RBL_HSCJiugonggeTypeM.DataValueField = "Key";
            RBL_HSCJiugonggeTypeM.DataBind();


            //報名哪些課程初始化
            //List<string> userCourseIDs = new List<string>();
            //DataTable dt = SQLdatabase.ExecuteDataTable("SELECT HCourseID FROM HCourseBooking WHERE HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND HStatus=1 AND HItemStatus=1 AND HCourseDonate='0'");
            //foreach (DataRow row in dt.Rows)
            //{
            //    userCourseIDs.Add(row["HCourseID"].ToString());
            //}
            //Session["UserBookedCourses"] = userCourseIDs;

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



        }

    }


    #region 發表主題
    protected void Btn_Launch_Click(object sender, EventArgs e)
    {
        #region 必填判斷
        if (string.IsNullOrEmpty(TB_HSCTopicName.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請輸入主題名稱~');", true);
            return;
        }

        if (DDL_HSCForumClassA.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區主類別~');", true);
            return;
        }

        if (DDL_HSCForumClassB.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區次類別~');", true);
            return;
        }

        if (DDL_HSCForumClassC.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區名稱~');", true);
            return;
        }
        #endregion

        SqlCommand cmd = new SqlCommand("INSERT INTO HSCTopic ( HSCForumClassID, HTopicName, HPinTop, HSCClassID, HSCRecordTypeID, HSCJiugonggeTypeID, HGProgress, HOGProgress, HContent, HFile1, HFile2, HFile3, HVideoLink, HHashTag, HStatus, HCreate, HCreateDT)VALUES(@HSCForumClassID, @HTopicName, @HPinTop, @HSCClassID, @HSCRecordTypeID, @HSCJiugonggeTypeID, @HGProgress, @HOGProgress, @HContent, @HFile1, @HFile2, @HFile3, @HVideoLink, @HHashTag, @HStatus, @HCreate, @HCreateDT)", ConStr);

        ConStr.Open();
        cmd.Parameters.AddWithValue("@HSCForumClassID", DDL_HSCForumClassC.SelectedValue);
        cmd.Parameters.AddWithValue("@HTopicName", TB_HSCTopicName.Text.Trim());
        cmd.Parameters.AddWithValue("@HPinTop", RBtn_HPinTop.SelectedValue == "1" ? "TRUE" : "FALSE");
        cmd.Parameters.AddWithValue("@HSCClassID", DDL_HSCClass.SelectedValue);
        cmd.Parameters.AddWithValue("@HSCRecordTypeID", DDL_HSCRecordType.SelectedValue);
        cmd.Parameters.AddWithValue("@HSCJiugonggeTypeID", RBL_HSCJiugonggeTypeM.SelectedValue);
        cmd.Parameters.AddWithValue("@HGProgress", DDL_HGProgress.SelectedValue);
        cmd.Parameters.AddWithValue("@HOGProgress", TB_HOGProgress.Text.Trim());
        cmd.Parameters.AddWithValue("@HContent", CKE_HContent.Text.Trim());
        cmd.Parameters.AddWithValue("@HFile1", LB_File1.Text);
        cmd.Parameters.AddWithValue("@HFile2", LB_File2.Text);
        cmd.Parameters.AddWithValue("@HFile3", LB_File3.Text);
        cmd.Parameters.AddWithValue("@HVideoLink", TB_HVideoLink.Text.Trim());
        cmd.Parameters.AddWithValue("@HHashTag", TB_HHashTag.Text.Trim());
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();


        #region 清空資料
        TB_HSCTopicName.Text = null;
        DDL_HSCForumClassA.SelectedValue = "0";
        DDL_HSCForumClassB.SelectedValue = "0";
        DDL_HSCForumClassC.SelectedValue = "0";
        RBtn_HPinTop.SelectedValue = null;
        DDL_HSCClass.SelectedValue = null;
        DDL_HSCRecordType.SelectedValue = null;
        RBL_HSCJiugonggeTypeM.SelectedValue = null;
        DDL_HGProgress.SelectedValue = "0";
        TB_HOGProgress.Text = null;
        CKE_HContent.Text = null;
        LB_File1.Text = null;
        LB_File2.Text = null;
        LB_File3.Text = null;
        TB_HVideoLink.Text = null;
        TB_HHashTag.Text = null;
        #endregion

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('已發表主題~');", true);

        SDS_HSCTopic.SelectCommand = gQuerySCTopic;
    }
    #endregion

    #region 關閉主題
    protected void Btn_Close_Click(object sender, EventArgs e)
    {

    }
    #endregion

    #region 上傳照片 (.jpg、.png、.gif、.mp3)
    protected void UploadFile()
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
                Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
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
                Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
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
                Response.Write("<script>alert('上傳的檔案不能超過10MB喔！');</script>");
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
            //上傳檔是否大於80M
            if (FU_HFileM2.PostedFile.ContentLength > (80 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過80MB喔！');</script>");
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
            //上傳檔是否大於40M
            if (FU_HFileM3.PostedFile.ContentLength > (40 * 1024 * 1024))
            {
                Response.Write("<script>alert('上傳的檔案不能超過40MB喔！');</script>");
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

        //Rpt_HSCTopic.DataBind();

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

                //DDL_HSCClassM.SelectedValue = QuerySCTopic["HSCClassID"].ToString();
                //DDL_HSCRecordTypeM.SelectedValue = QuerySCTopic["HSCRecordTypeID"].ToString();

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

                    if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                    {
                        IMG_File1.Visible = true;
                        IMG_File1.ImageUrl = SCFile + LB_FileM1.Text;
                    }
                    else if (gExtension[1] == "mp3")
                    {
                        Audio1.Visible = true;
                        Source1.Src = SCFile + LB_File1.Text;
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
                    if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                    {
                        IMG_File2.Visible = true;
                        IMG_File2.ImageUrl = SCFile + LB_FileM2.Text;
                    }
                    else if (gExtension[1] == "mp3")
                    {
                        Audio2.Visible = true;
                        Source2.Src = SCFile + LB_File2.Text;
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
                    if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                    {
                        IMG_File3.Visible = true;
                        IMG_File3.ImageUrl = SCFile + LB_FileM3.Text;
                    }
                    else if (gExtension[1] == "mp3")
                    {
                        Audio3.Visible = true;
                        Source3.Src = SCFile + LB_File3.Text;
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
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請輸入主題名稱~');", true);
            return;
        }

        //if (DDL_HSCForumClassAM.SelectedValue == "0")
        //{
        //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區主類別~');", true);
        //    return;
        //}

        //if (DDL_HSCForumClassBM.SelectedValue == "0")
        //{
        //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區次類別~');", true);
        //    return;
        //}

        if (DDL_HSCForumClassCM.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請選擇討論區名稱~');", true);
            return;
        }
        #endregion


        //更新資料庫
        using (SqlConnection con = new SqlConnection(Con))
        {
            SqlCommand cmd = new SqlCommand("UPDATE HSCTopic SET HSCForumClassID=@HSCForumClassID, HTopicName=@HTopicName, HPinTop=@HPinTop, HSCClassID=@HSCClassID, HSCRecordTypeID=@HSCRecordTypeID, HSCJiugonggeTypeID=@HSCJiugonggeTypeID, HGProgress=@HGProgress, HOGProgress=@HOGProgress, HContent=@HContent, HFile1=@HFile1, HFile2=@HFile2, HFile3=@HFile3, HVideoLink=@HVideoLink, HHashTag=@HHashTag, HStatus=@HStatus, HModify=@HModify,HModifyDT=@HModifyDT WHERE HID=@HID", con);
            con.Open();
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
            cmd.Parameters.AddWithValue("@HHashTag", TB_HHashTagM.Text.Trim());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
            cmd.Cancel();
        }

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO HSCTopic_Log (HSCTopicID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

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

        SDS_HSCTopic.SelectCommand = gQuerySCTopic;
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

        SDS_HSCTopic.SelectCommand = gQuerySCTopic;
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
        LinkButton FeelingTypeBtn = sender as LinkButton;
        string gFeelingType_CA = FeelingTypeBtn.CommandArgument;
        //int gFeelingType_CN = Convert.ToInt32(btn.CommandName);

        LB_SCTopicID.Text = gFeelingType_CA;

        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        //Response.Write("LB_SCTopicID=" + gFeelingType_CA + "<br/>");
        //Response.Write("A="+ FeelingTypeBtn.TabIndex.ToString()+"<br/>");
        //Response.Write("B=" + ((Label)RI.FindControl("LB_HMemberID")).Text + "<br/>");
        //Response.Write("C=" + ((Label)Master.FindControl("LB_HUserHID")).Text + "<br/>");
        //Response.Write("D=" + ((Label)RI.FindControl("LB_HID")).Text + "<br/>");

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
            Notification.AddNotification(((Label)RI.FindControl("LB_HMemberID")).Text, ((Label)Master.FindControl("LB_HUserHID")).Text, (Convert.ToInt32(FeelingTypeBtn.TabIndex.ToString()) + 1).ToString(), ((Label)RI.FindControl("LB_HID")).Text, "HSCTopic");
        }

        SqlDataReader QueryFeeling = SQLdatabase.ExecuteReader("SELECT HID, HSCTopicID, HMemberID, HType FROM HSCTopic_Mood WHERE HSCTopicID='" + gFeelingType_CA + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        if (QueryFeeling.Read())
        {
            if (FeelingTypeBtn.TabIndex.ToString() != QueryFeeling["HType"].ToString())
            {
                SqlCommand cmd = new SqlCommand("UPDATE [HSCTopic_Mood] SET HType=@HType, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCTopicID=@HSCTopicID AND HMemberID=@HMemberID", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HSCTopicID", gFeelingType_CA);
                cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HType", FeelingTypeBtn.TabIndex.ToString());
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                cmd.ExecuteNonQuery();
                ConStr.Close();
                cmd.Cancel();
            }

        }
        else
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO [HSCTopic_Mood] (HSCTopicID, HMemberID, HType, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HType, @HStatus, @HCreate, @HCreateDT)", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCTopicID", gFeelingType_CA);
            cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HType", FeelingTypeBtn.TabIndex.ToString());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
        }
        QueryFeeling.Close();

        //Response.Write("F=SELECT HID, HSCTopicID, HMemberID, HType FROM HSCTopic_Mood WHERE HSCTopicID = '" + gFeelingType_CA + "' AND HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "<br/>");

        //隱藏顯示的區域
        //((HtmlControl)Rpt_HSCTopic.Items[gFeelingType_CN].FindControl("Div_FeelingArea")).Visible = false;

        //資料繫結
        SDS_HSCTopic.SelectCommand = gQuerySCTopic;

        DataTable dtHSCTopic = Session["UnifiedTable"] as DataTable;
        if (dtHSCTopic != null)
        {
            Rpt_HSCTopic.DataSource = dtHSCTopic;
            Rpt_HSCTopic.DataBind();
        }


        //Rpt_HSCTopic.DataSource = SDS_HSCTopic;
        //Rpt_HSCTopic.DataBind();


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

            SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_SCTopicID.Text + "' AND a.HType='1' AND a.HStatus=1 ";

        }
        else if (btn.TabIndex.ToString() == "2")
        {
            LBtn_ThumbsUpM.CssClass = "nav-link font-weight-bold";
            LBtn_HeartM.CssClass = "nav-link  active show font-weight-bold";
            LBtn_SmileM.CssClass = "nav-link font-weight-bold";

            SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_SCTopicID.Text + "' AND a.HType='2' AND a.HStatus=1";

        }
        else if (btn.TabIndex.ToString() == "3")
        {
            LBtn_ThumbsUpM.CssClass = "nav-link font-weight-bold";
            LBtn_HeartM.CssClass = "nav-link font-weight-bold";
            LBtn_SmileM.CssClass = "nav-link  active show font-weight-bold";

            SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_SCTopicID.Text + "' AND a.HType='3' AND a.HStatus=1";

        }

        Rpt_HSCTopicMood.DataBind();
        Rpt_HSCTopic.DataBind();

        //計算數量
        FeelingCounts(LB_SCTopicID.Text);

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

    }
    #endregion

    #region 更新主題取消功能
    protected void Btn_UPDCancel_Click(object sender, EventArgs e)
    {

    }
    #endregion

    #region 資料繫結
    protected void Rpt_HSCTopic_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;


        string gSource = "1"; //1:新版專欄、2:EIP
        if (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HSCForumClassB")).Text))
        {
            ((Label)e.Item.FindControl("LB_HSCForumClassB")).Text = "大愛光老師專欄";
            ((Label)e.Item.FindControl("LB_HSCForumClassC")).Text = "EIP";
        
        }

        if (((Label)e.Item.FindControl("LB_HSCForumClassC")).Text == "EIP")
        {
            gSource = "2";
        }

        #region 大頭照
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
        #endregion

        if (Session["SearchType"] != null)
        {
            if (Session["SearchType"].ToString() == "2")
            {
                ((HtmlGenericControl)e.Item.FindControl("HotTag")).Visible = true;
            }
            else
            {
                ((HtmlGenericControl)e.Item.FindControl("HotTag")).Visible = false;
            }

        }


        #region 數量計算
        //留言與回應
        ((Label)e.Item.FindControl("LB_MsgNum")).Text = (ResponseCounts(((Label)e.Item.FindControl("LB_HID")).Text, gSource)).ToString("N0");

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
        ((LinkButton)e.Item.FindControl("LBtn_Seen")).Text = "<span class='ti-eye mr-2'></span>" + (ViewCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString("N0");
        //((Label)e.Item.FindControl("LB_SeenNum")).Text = (ViewCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString("N0");

        //分享次數
        ((Label)e.Item.FindControl("LB_ShareNum")).Text = (ShareCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString("N0");
        #endregion

        #region 判斷是否有報名該課程

        //250902-改
        #region 判斷是否有報名該課程（標題 vs 已報名課名，子字串 & 不分大小寫）
        var divTopic = (HtmlGenericControl)e.Item.FindControl("Div_Topicarea");
        var divNoPerm = (HtmlGenericControl)e.Item.FindControl("Div_NoPermission");
        var divShow = (HtmlGenericControl)e.Item.FindControl("Div_Show");

        // 1) 取得這筆主題的標題
        string gTopicName = (gDRV["HTopicName"] == DBNull.Value) ? "" : gDRV["HTopicName"].ToString().Trim();

        // 2) 取得使用者已報名的課名集合（主流程已放進 Session，大小寫不敏感）
        var bookedNames = Session["UserBookedCourseNames"] as HashSet<string>;

        // 3) 判斷：只要主題標題包含任一已報名課名（不分大小寫），就視為「有報名」
        bool hasBooked = false;
        string compareName = NormalizeTopicName(gTopicName);


        if (!string.IsNullOrEmpty(gTopicName) && bookedNames != null && bookedNames.Count > 0)
        {

            foreach (var cn in bookedNames)
            {
                if (string.IsNullOrWhiteSpace(cn)) continue;

                // 不分大小寫的子字串比對
                //if (gTopicName.IndexOf(cn, StringComparison.OrdinalIgnoreCase) >= 0)
                //{
                //    hasBooked = true;
                //    break;
                //}

                if (compareName.IndexOf(cn, StringComparison.OrdinalIgnoreCase) >= 0 || cn.ToString().Contains(compareName))
                {
                    hasBooked = true;
                    break;
                }
            }
        }
        else
        {
            // 沒標題或沒有報名清單 → 預設視為未報名
            hasBooked = false;
        }

        // 4) 前端標記 + 顯示控制
        divTopic.Attributes["data-booked"] = hasBooked ? "1" : "0";
        divNoPerm.Visible = !hasBooked;           // 未報名 → 顯示無權限
        divShow.Visible = hasBooked;            // 已報名 → 顯示可看內容


        //divNoPerm.Visible =false;           // 未報名 → 顯示無權限
        //divShow.Visible = true;            // 已報名 → 顯示可看內容

        // 5) 連動按鈕可否點擊（例如：詳情按鈕）
        var btnDetail = (LinkButton)e.Item.FindControl("LBtn_MsgDetail");
        if (btnDetail != null) btnDetail.Enabled = hasBooked;
        #endregion
        #endregion

     //   ((HyperLink)e.Item.FindControl("HL_QuickBook")).NavigateUrl = "HCourseList.aspx?";




    }

    #region  工具庫
    // 將標題正規化：移空白/符號、全形轉半形、中文數字轉阿拉伯數字、去常見尾碼（第x期、xx班…）
    private string NormalizeCourseTitle(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";

        s = ToHalfWidth(s).ToLowerInvariant();

        // 去除括號內容（中英皆去）
        s = Regex.Replace(s, @"[\(\（][^\)\）]*[\)\）]", " ");

        // 常見冗字（自行增減）：課程/班/第X期/體驗/說明會/線上/實體/班別/梯次 等
        string[] stopwords = { "課程", "課", "班", "第", "期", "梯次", "體驗", "說明會", "工作坊", "線上", "實體", "精修", "入門", "進階" };
        foreach (var sw in stopwords) s = s.Replace(sw, " ");

        // 中文數字轉阿拉伯（簡化版：一二三四五六七八九十）
        s = ChineseNumberToArabic(s);

        // 移除標點與空白
        s = Regex.Replace(s, @"[^\p{L}\p{Nd}]+", ""); // 只留字母與數字
        return s;
    }

    // 全形 -> 半形
    private string ToHalfWidth(string input)
    {
        var chars = input.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] == 12288) { chars[i] = (char)32; continue; }        // 全形空白
            if (chars[i] >= 65281 && chars[i] <= 65374)                       // 全形標點/字元
                chars[i] = (char)(chars[i] - 65248);
        }
        return new string(chars);
    }

    // 粗略將中文數字轉為阿拉伯（處理「第十期」「二十一」等常見）
    private string ChineseNumberToArabic(string s)
    {
        // 先處理「第…期」中的數字
        s = Regex.Replace(s, @"第([一二三四五六七八九十百千兩〇零0-9]+)期", m => NumberWordToInt(m.Groups[1].Value).ToString());

        // 零散數字詞也轉
        s = Regex.Replace(s, @"[一二三四五六七八九十百千兩〇零]+", m => NumberWordToInt(m.Value).ToString());
        return s;
    }

    // 簡化中文數字轉整數（常見到千位）
    private int NumberWordToInt(string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return 0;
        // 若原本就有數字
        int tmp; if (int.TryParse(word, out tmp)) return tmp;

        Dictionary<char, int> map = new Dictionary<char, int> {
        { '零',0},{ '〇',0},{ '一',1},{ '二',2},{ '兩',2},{ '三',3},{ '四',4},{ '五',5},{ '六',6},{ '七',7},{ '八',8},{ '九',9}
    };
        Dictionary<char, int> unit = new Dictionary<char, int> {
        { '十',10},{ '百',100},{ '千',1000}
    };

        int result = 0, current = 0;
        foreach (char c in word)
        {
            if (map.ContainsKey(c)) current = current * 10 + map[c];
            else if (unit.ContainsKey(c))
            {
                if (current == 0) current = 1;
                result += current * unit[c];
                current = 0;
            }
        }
        result += current;
        return result == 0 ? 0 : result;
    }

    // token 包含：較嚴格的「normName 是 normTitle 的子序列」檢查
    private bool TitleContains(string normTitle, string normName)
    {
        if (string.IsNullOrEmpty(normTitle) || string.IsNullOrEmpty(normName)) return false;
        return normTitle.Contains(normName) || normName.Contains(normTitle);
    }

    // Jaccard 相似度（以字元集合計）：回傳 0~1，越大越像
    private double Jaccard(string a, string b)
    {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0.0;

        var setA = new HashSet<char>(a.ToCharArray());
        var setB = new HashSet<char>(b.ToCharArray());

        int inter = setA.Intersect(setB).Count();
        int union = setA.Union(setB).Count();

        return union == 0 ? 0.0 : (double)inter / union;
    }


    #endregion



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
        if (Request.Cookies["TopicID"] != null && Request.Cookies["TopicID"].Value != "undefined")
        {

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

            //清除cookie
            Response.Cookies["TopicID"].Expires = DateTime.Now.AddDays(-1);
        }



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

        SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + gMood_CA + "' AND a.HType='1' AND a.HStatus=1";
        Rpt_HSCTopic.DataBind();

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

    #region 只顯示可參與
    protected void Btn_Participate_Click(object sender, EventArgs e)
    {
        //Response.Write("A");

        if (CB_Participate.Checked == true)
        {
            //Response.Write("B");

            if (!string.IsNullOrEmpty(((Label)Master.FindControl("LB_HUserHID")).Text))
            {


                // 讀取 HMember.HType（維持你原本邏輯：HType == "0" 視為無類型）
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

                // SDS_HSCTopic.SelectCommand = gQuery + " WHERE a.HStatus=1 AND f.HIRestriction LIKE '%" + StrHType + "%' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName , a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY IIF(a.HPinTop IS NULL,0,a.HPinTop)  DESC, IIF(a.HModifyDT IS NOT NULL,a.HModifyDT, a.HCreateDT) DESC";

            
                // 來源表：統一整併後的資料
                DataTable src = Session["UnifiedTable"] as DataTable;
                if (src == null)
                {
                    Panel_NoResult.Visible = true;
                    LB_Nodata.Text = "沒有可用的資料表（UnifiedTable）。";
                    Rpt_HSCTopic.Visible = false;
                    return;
                }
                if (!src.Columns.Contains("HIRestriction"))
                {
                    Panel_NoResult.Visible = true;
                    LB_Nodata.Text = "資料缺少 HIRestriction 欄位，無法依參與資格篩選。";
                    Rpt_HSCTopic.Visible = false;
                    return;
                }

                // 建立結果表（與 src 架構相同）
                DataTable result = src.Clone();

                // 支援使用者 HType 可能是多值（例如 "1,3,5"）
                string[] userTypes = (StrHType ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                // 逐列檢查，避免 LIKE '%1%' 命中 '10'，改用「逗號邊界」判斷
                for (int i = 0; i < src.Rows.Count; i++)
                {
                    DataRow row = src.Rows[i];
                    string restr = (row["HIRestriction"] == DBNull.Value) ? "" : row["HIRestriction"].ToString().Trim();

                    bool openToAll = string.IsNullOrEmpty(restr); // 無限制（空字串或 NULL）
                    bool eligible = false;

                    if (userTypes.Length == 0)
                    {
                        // 使用者無類型（HType==0 或空）→ 只看無限制的
                        eligible = openToAll;
                    }
                    else
                    {
                        // 篩選（以逗號為邊界）
                        string padded = "," + restr.Trim(',') + ","; // 例如 ",1,3,5,"
                        for (int u = 0; u < userTypes.Length; u++)
                        {
                            string ut = userTypes[u].Trim();
                            if (ut.Length == 0) continue;

                            if (padded.IndexOf("," + ut + ",", StringComparison.Ordinal) >= 0)
                            {
                                eligible = true;
                                break;
                            }
                        }

                        // 如果你要「有類型的人也能看到無限制」→ 打開下面這行
                        // if (openToAll) eligible = true;
                    }

                    if (eligible) result.ImportRow(row);
                }

                // 綁定輸出
                if (result.Rows.Count > 0)
                {
                    Rpt_HSCTopic.DataSource = result;
                    Rpt_HSCTopic.DataBind();
                    Rpt_HSCTopic.Visible = true;
                    Panel_NoResult.Visible = false;
                    LB_Nodata.Text = "";
                }
                else
                {
                    Rpt_HSCTopic.DataSource = null;
                    Rpt_HSCTopic.DataBind();
                    Rpt_HSCTopic.Visible = false;
                    Panel_NoResult.Visible = true;
                    LB_Nodata.Text = "目前沒有您可參與的討論區。";
                }



            }

           // Rpt_HSCTopic.DataBind();


            



        }
    }
    #endregion

    #region 只顯示已報名
    protected void Btn_Booking_Click(object sender, EventArgs e)
    {
        if (CB_Booking.Checked)
        {
            string userID = ((Label)Master.FindControl("LB_HUserHID")).Text.Trim();
            if (!string.IsNullOrEmpty(userID))
            {
                SqlDataReader dr = SQLdatabase.ExecuteReader(
                    "SELECT TOP(15) HCoursename FROM HCourseBooking " +
                    "WHERE HMemberID = '" + userID + "'" +
                    "AND HStatus = 1 AND HItemStatus = 1 AND HCourseDonate = '0' ORDER BY HID DESC"
                );

                List<string> courseNames = new List<string>();
                while (dr.Read())
                {
                    courseNames.Add(dr["HCourseName"].ToString().Trim().Replace("'", "''")); // 加上 SQL 安全處理
                }
                dr.Close();

                DataTable dtHSCTopic = Session["UnifiedTable"] as DataTable;

                if (dtHSCTopic != null && courseNames.Count > 0)
                {
                    // 建立過濾條件
                    List<string> filters = new List<string>();
                    foreach (string name in courseNames)
                    {
                        filters.Add("HTopicName LIKE '%" + name + "%'");
                    }
                    string filter = string.Join(" OR ", filters);

                    DataRow[] matchedRows = dtHSCTopic.Select(filter);

                    if (matchedRows.Length > 0)
                    {
                        DataTable resultTable = matchedRows.CopyToDataTable();
                        Rpt_HSCTopic.DataSource = resultTable;
                        Rpt_HSCTopic.DataBind();

                        Panel_NoResult.Visible = false;
                        Rpt_HSCTopic.Visible = true;
                    }
                    else
                    {
                        // 沒找到資料
                        Rpt_HSCTopic.DataSource = null;
                        Rpt_HSCTopic.DataBind();

                        Panel_NoResult.Visible = true;
                        LB_Nodata.Text = "您目前沒有已報名課程的相關討論區哦~";
                    }
                }
                else
                {
                    // 沒有任何課程名稱
                    Panel_NoResult.Visible = true;
                    LB_Nodata.Text = "您目前沒有已報名課程的相關討論區哦~";

                }
            }
        }
        else
        {
            DataTable dtHSCTopic = Session["UnifiedTable"] as DataTable;

            Panel_NoResult.Visible = false;
            LB_Nodata.Text = "";
            Rpt_HSCTopic.DataSource = dtHSCTopic;
            Rpt_HSCTopic.DataBind();
            Rpt_HSCTopic.Visible = true;
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

            //AA20241225_判斷登入者是否有按心情符號
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
        ////資料讀取
        //SqlDataReader QueryView = SQLdatabase.ExecuteReader("SELECT HTimes, IIF(HModifyDT IS NUll, HCreateDT,HModifyDT) AS Date FROM HSCTopic_View WHERE HSCTopicID='" + SCTopicID + "' AND HStatus=1 ORDER BY IIF(HModifyDT IS NUll, HCreateDT,HModifyDT ) DESC");
        //int gView = 0;
        //while (QueryView.Read())
        //{
        //    gView += Convert.ToInt32(QueryView["HTimes"].ToString());
        //}
        //QueryView.Close();

        //LB_ViewNum.Text = gView.ToString("N0");
        //return gView;

        System.Data.DataTable dtView = SQLdatabase.ExecuteDataTable("SELECT HTimes, IIF(HModifyDT IS NUll, HCreateDT,HModifyDT) AS Date FROM HSCTopic_View WHERE HSCTopicID='" + SCTopicID + "' AND HStatus=1 ORDER BY IIF(HModifyDT IS NUll, HCreateDT,HModifyDT ) DESC");

        int gView = 0;
        foreach (DataRow datarow in dtView.Rows)
        {
            gView += Convert.ToInt32(datarow["HTimes"].ToString());
        }

        return gView;
    }
    #endregion

    #region 分享數量計算
    private int ShareCounts(string SCTopicID)
    {
        //資料讀取
        //SqlDataReader QueryShare = SQLdatabase.ExecuteReader("SELECT HTimes FROM HSCTopic_Share WHERE HSCTopicID='" + SCTopicID + "' AND HStatus=1 ");
        //int gView = 0;

        //while (QueryShare.Read())
        //{
        //    gView += Convert.ToInt32(QueryShare["HTimes"].ToString());
        //}
        //QueryShare.Close();

        //LB_ViewNum.Text = gView.ToString("N0");

        //return gView;



        System.Data.DataTable dtShare = SQLdatabase.ExecuteDataTable("SELECT HTimes FROM HSCTopic_Share WHERE HSCTopicID='" + SCTopicID + "' AND HStatus=1");

        int gShare = 0;
        foreach (DataRow datarow in dtShare.Rows)
        {
            gShare += Convert.ToInt32(datarow["HTimes"].ToString());
        }

        return gShare;


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


        System.Data.DataTable dtHSCTopic_View = SQLdatabase.ExecuteDataTable("SELECT  (b.HArea +' '+ISNULL(b.HSystemName,'無體系')  +' / '+HPeriod+' '+HUserName)   AS HUserName, a.HTimes, a.HCreateDT AS FirstTime, a.HModifyDT AS LatestTime FROM HSCTopic_View AS a JOIN MemberList AS b ON a.HMemberID= b.HID WHERE a.HStatus=1 AND a.HSCTopicID='" + gSeen_CA + "'");

        Rpt_HSCTopic_View.DataSource = dtHSCTopic_View;
        Rpt_HSCTopic_View.DataBind();

        if (Session["UnifiedTable"] !=null)
        {
            Rpt_HSCTopic.DataSource = Session["UnifiedTable"];
            Rpt_HSCTopic.DataBind();

        }

    }
    #endregion

    protected void Rpt_HSCTopic_View_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        if (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_LatestTime")).Text))
        {
            ((Label)e.Item.FindControl("LB_LatestTime")).Text = "-";
        }


    }


    //private DataTable GetAvailableTopics(int userId)
    //{
    //    string connStr = ConfigurationManager.ConnectionStrings["MyConnStr"].ConnectionString;
    //    using (var conn = new MySql.Data.MySqlClient.MySqlConnection(connStr))
    //    {
    //        string sql = @"
    //    SELECT 
    //        t.TopicID,
    //        t.TopicName,
    //        t.Deadline
    //    FROM Topics t
    //    JOIN Users u ON u.UserID = @UserID
    //    WHERE u.Status = 1
    //        AND t.RequiredLevel <= u.UserLevel
    //        AND NOW() <= t.Deadline
    //        AND NOT EXISTS (
    //            SELECT 1 FROM Signup s WHERE s.TopicID = t.TopicID AND s.UserID = @UserID
    //        )
    //        AND (
    //            SELECT COUNT(*) FROM Signup WHERE TopicID = t.TopicID
    //        ) < t.MaxSignup";

    //        var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conn);
    //        cmd.Parameters.AddWithValue("@UserID", userId);
    //        var da = new MySql.Data.MySqlClient.MySqlDataAdapter(cmd);
    //        DataTable dt = new DataTable();
    //        da.Fill(dt);
    //        return dt;
    //    }
    //}

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
          AND HCourseDonate = '0' 
ORDER BY HID DESC
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

