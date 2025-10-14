using DocumentFormat.OpenXml.Presentation;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class HSCForumDetail : System.Web.UI.Page
{
    string EIPconnStr = ConfigurationManager.ConnectionStrings["HochiEIPConnection"].ConnectionString;

    SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);
    string Con = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;

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
        if (!IsPostBack)
        {
            //MA20240709_選單引用：App_Code/SelectListItem/RBL_ListItem.cs
            RBL_HSCJiugonggeTypeID.DataSource = RBL_ListItem.HSCJiugonggeType.ListItem;
            RBL_HSCJiugonggeTypeID.DataTextField = "Value";
            RBL_HSCJiugonggeTypeID.DataValueField = "Key";
            RBL_HSCJiugonggeTypeID.DataBind();

            RBL_HSCJiugonggeTypeM.DataSource = RBL_ListItem.HSCJiugonggeType.ListItem;
            RBL_HSCJiugonggeTypeM.DataTextField = "Value";
            RBL_HSCJiugonggeTypeM.DataValueField = "Key";
            RBL_HSCJiugonggeTypeM.DataBind();



            string gTypekeyword = "";

            if (!string.IsNullOrEmpty(Request.QueryString["FClassCID"].ToString()))   //HSCForumClass.HID(討論區名稱)
            {
                LB_HSCForumClassCID.Text = Request.QueryString["FClassCID"].ToString();

                SqlDataReader QueryHSCForumClass = SQLdatabase.ExecuteReader("SELECT a.HID, b.HID AS HSCForumClassBID, b.HSCFCName AS HSCForumClassB, a.HSCFCName AS HSCForumClassC, c.HSCFCName AS  HSCForumClassA, d.HSCMPublicYN, e.HSCMRule, a.HPublic FROM HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON a.HSCFCMaster=b.HID  LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID LEFT JOIN HSCMPublicYN AS d ON a.HID=d.HSCForumClassID LEFT JOIN HSCMRule AS e ON a.HID=e.HSCForumClassID WHERE a.HID='" + Request.QueryString["FClassCID"].ToString() + "'");

                if (QueryHSCForumClass.Read())
                {
                    HL_HSCForumClassA.Text = QueryHSCForumClass["HSCForumClassA"].ToString();
                    HL_HSCForumClassB.Text = QueryHSCForumClass["HSCForumClassB"].ToString();
                    HL_HSCForumClassA.NavigateUrl = "HSCForum.aspx?FClassBID=" + QueryHSCForumClass["HSCForumClassBID"].ToString();
                    //HL_HSCForumClassB.NavigateUrl = "HSCForum.aspx?FClassBID=" + QueryHSCForumClass["HSCForumClassBID"].ToString();
                    LTR_HSCForumClassC.Text = QueryHSCForumClass["HSCForumClassC"].ToString() + "討論區";
                    LB_HPublic.Text = QueryHSCForumClass["HPublic"].ToString() == "1" ? "公開" : "非公開";
                    LB_HSCMRule.Text = QueryHSCForumClass["HSCMRule"].ToString();


                    if (QueryHSCForumClass["HSCForumClassC"].ToString() == "各線晨光上")
                    {
                        gTypekeyword = "晨光上";
                    }
                    else if (QueryHSCForumClass["HSCForumClassC"].ToString() == "晨光下＆星光空間")
                    {
                        gTypekeyword = "晨光下%星光空間";
                    }
                    else if (QueryHSCForumClass["HSCForumClassC"].ToString() == "幸福班")
                    {
                        gTypekeyword = "幸福班";
                    }

                }
                QueryHSCForumClass.Close();




                //若尚未編輯版規則以範本的做顯示
                if (string.IsNullOrEmpty(LB_HSCMRule.Text))
                {
                    SqlDataReader QuerySCMRule_T = SQLdatabase.ExecuteReader("SELECT TOP (1) HID, HSCRule, HStatus FROM HSCRule_T WHERE HStatus='1'");

                    if (QuerySCMRule_T.Read())
                    {
                        LB_HSCMRule.Text = QuerySCMRule_T["HSCRule"].ToString();
                    }
                    QuerySCMRule_T.Close();
                }

                #region 我的最愛
                SqlDataReader QueryHSCForumFavorite = SQLdatabase.ExecuteReader("SELECT HSCFClassFavorite FROM HMember WHERE HSCFClassFavorite LIKE '%," + Request.QueryString["FClassCID"].ToString() + ",%'");

                if (QueryHSCForumFavorite.Read())
                {
                    LB_HSCFavorite.Text = QueryHSCForumFavorite["HSCFClassFavorite"].ToString();

                    LBtn_HSCFClassFavorite.Visible = false;
                    LBtn_CancelFavorite.Visible = true;
                }
                else
                {
                    LBtn_HSCFClassFavorite.Visible = true;
                    LBtn_CancelFavorite.Visible = false;
                }
                QueryHSCForumFavorite.Close();
                #endregion

                #region 判斷是否有管理者權限
                SqlDataReader QueryHSCModerator = SQLdatabase.ExecuteReader("SELECT HID, HSCForumClassID, HMemberID FROM      HSCModeratorSetting WHERE HSCForumClassID='" + Request.QueryString["FClassCID"].ToString() + "' AND HMemberID LIKE '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%' ");
                if (!QueryHSCModerator.Read())
                {
                    LBtn_HManager.Visible = false;
                }
                else
                {
                    LBtn_HManager.Visible = true;
                }
                QueryHSCModerator.Close();
                #endregion


                #region 新版專欄
                //SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC, f.HSCFCName AS HSCForumClassA,  HTopicName, HPinTop,HContent, HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, IIF(e.HID IS NULL,'0',e.HID) AS HMemberID,e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID LEFT JOIN HSCForumClass AS f ON c.HSCFCMaster=f.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 AND a.HSCForumClassID='" + LB_HSCForumClassCID.Text + "' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName ,f.HSCFCName,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY a.HPinTop DESC, a.HCreateDT DESC";
                //Rpt_HSCTopic.DataBind();
                #endregion


                #region 整合EIP
                var dtRecords = SQLdatabase.ExecuteDataTable("SELECT a.HID, c.HSCFCName AS HSCForumClassB, b.HSCFCName AS HSCForumClassC, f.HSCFCName AS HSCForumClassA, HTopicName, HPinTop, HContent, HFile1, (e.HSystemName + ' ' + e.HArea + '/' + e.HPeriod + ' ' + e.HUserName) AS UserName, IIF(e.HID IS NULL, '0', e.HID) AS HMemberID, e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID = b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster = c.HID LEFT JOIN HSCForumClass AS f ON c.HSCFCMaster = f.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID = d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus = 1 AND a.HSCForumClassID = '" + LB_HSCForumClassCID.Text + "' GROUP BY a.HID, a.HCourseID, c.HSCFCName, b.HSCFCName, f.HSCFCName, a.HTopicName, a.HPinTop, a.HContent, a.HFile1, e.HSystemName, e.HArea, e.HPeriod, e.HUserName, e.HID, e.HImg, a.HCreateDT, a.HModifyDT ORDER BY a.HPinTop DESC, a.HCreateDT DESC");

                //Response.Write("SELECT  l_id AS HID, 'EIP' AS HSCForumClassB, 'EIP' AS HSCForumClassC,  l_title AS Title, l_Content AS Content, l_recorddate AS HCreateDT,  l_cname AS UserName FROM heip.laoshi WHERE l_status=2 AND l_category=0 AND l_recorddate LIKE CONCAT('%', DATE_FORMAT(CURRENT_DATE(), '%y%m%d'), '%') AND l_title LIKE N'%" + gTypekeyword + "%'  ORDER BY l_recorddate DESC, l_no DESC");
                //Response.End();


                // 2. 撈 MySQL 資料
                var mysqlConn = new MySqlConnection(EIPconnStr);
                var mysqlCmd = new MySqlCommand("SELECT  l_id AS HID, 'EIP' AS HSCForumClassB, 'EIP' AS HSCForumClassC,  l_title AS Title, l_Content AS Content, l_recorddate AS HCreateDT,  l_cname AS UserName FROM heip.laoshi WHERE l_status=2 AND l_category=0 AND l_title NOT LIKE '%成長紀錄%'  AND l_recorddate >= '20250707000000' AND l_recorddate < DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 15 HOUR), '%Y%m%d%H%i%S') AND l_title LIKE N'%" + gTypekeyword + "%'  ORDER BY l_no DESC", mysqlConn);/*  l_recorddate LIKE CONCAT('%', DATE_FORMAT(CURRENT_DATE(), '%y%m%d'), '%') AND l_recorddate < DATE_FORMAT(DATE_ADD(NOW(), INTERVAL 15 HOUR), '%y%m%d%H%i%s') */
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
                HashSet<string> mergedKeys = new HashSet<string>();

                // 3. 先將 MSSQL 資料加入 unifiedTable
                foreach (var row in dtRecords.Rows)
                {
                    DataRow newRow = unifiedTable.NewRow();
                    newRow.ItemArray = ((DataRow)row).ItemArray.Clone() as object[];
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
                        unifiedTable.Rows.Add(newRow);
                    }

                }

                Session["UnifiedTable"] = unifiedTable;

                //加入排序條件
                DataView sortedView = unifiedTable.DefaultView;
                sortedView.Sort = "HCreateDT DESC";
                DataTable sortedTable = sortedView.ToTable();

                Rpt_HSCTopic.DataSource = sortedTable;
                Rpt_HSCTopic.DataBind();


                #endregion



                SDS_HSCForumClassC.SelectCommand = "SELECT a.HID, a.HSCFCName, a.HSCFCLevel, a.HStatus, b.HSCFCName AS HSCForumMaster FROM HSCForumClass AS a LEFT JOIN HSCForumClass AS b ON b.HID=a.HSCFCMaster WHERE a.HStatus = 1 AND a.HSCFCLevel = '30' AND a.HID ='" + LB_HSCForumClassCID.Text + "'";
                DDL_HSCForumClassC.DataBind();
                DDL_HSCForumClassC.SelectedValue = LB_HSCForumClassCID.Text;

            }


        }


    }

    #region 進入管理員功能
    protected void LBtn_HManager_Click(object sender, EventArgs e)
    {
        Response.Redirect("HSCModerator.aspx?FClassCID=" + LB_HSCForumClassCID.Text);
    }
    #endregion


    #region 發表主題
    protected void Btn_Launch_Click(object sender, EventArgs e)
    {
        #region 必填判斷
        if (string.IsNullOrEmpty(TB_HTopicName.Text.Trim()))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('請輸入主題名稱~');$('#ContentModal').modal('show');", true);
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

        SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC, f.HSCFCName AS HSCForumClassA,  HTopicName, HPinTop,HContent, HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, IIF(e.HID IS NULL,'0',e.HID) AS HMemberID,e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID LEFT JOIN HSCForumClass AS f ON c.HSCFCMaster=f.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 AND a.HSCForumClassID='" + LB_HSCForumClassCID.Text + "' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName , f.HSCFCName, a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY a.HPinTop DESC, a.HCreateDT DESC";

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
        if (FU_HFile1.HasFile)
        {
            //上傳檔是否大於10M
            if (FU_HFile1.PostedFile.ContentLength > (5 * 1024 * 1024))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('上傳的檔案不能超過5MB喔！');", true);
                //Response.Write("<script>alert('上傳的檔案不能超過5MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_HFile1.FileName);
                //檔名
                LB_File1.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;

                this.FU_HFile1.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_File1.Text));


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
        if (FU_HFile2.HasFile)
        {
            //上傳檔是否大於10M
            if (FU_HFile2.PostedFile.ContentLength > (5 * 1024 * 1024))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('上傳的檔案不能超過5MB喔！');", true);
                //Response.Write("<script>alert('上傳的檔案不能超過5MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_HFile2.FileName);
                //檔名
                LB_File2.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;

                this.FU_HFile2.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_File2.Text));


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
        if (FU_HFile3.HasFile)
        {
            //上傳檔是否大於10M
            if (FU_HFile3.PostedFile.ContentLength > (5 * 1024 * 1024))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('上傳的檔案不能超過5MB喔！');", true);
                //Response.Write("<script>alert('上傳的檔案不能超過5MB喔！');</script>");
                return;
            }
            else
            {
                string FileExtension = Path.GetExtension(FU_HFile3.FileName);
                //檔名
                LB_File3.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;

                this.FU_HFile3.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_File3.Text));


            }
        }

        //重開Modal
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#ContentModal').modal('show');</script>", false);//執行js
    }



    #region 編輯主題 上傳檔案
    protected void LBtn_HFileM1Upload_Click(object sender, EventArgs e)
    {
        //建立資料夾
        if (!Directory.Exists(SCFileRoot))
        {
            Directory.CreateDirectory(SCFileRoot);
        }
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
                string FileExtension = Path.GetExtension(FU_HFileM1.FileName);
                //檔名
                LB_FileM1.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;
                LB_FileM1.Visible = true;
                this.FU_HFileM1.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_FileM1.Text));


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
                string FileExtension = Path.GetExtension(FU_HFileM2.FileName);
                //檔名
                LB_FileM2.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;
                LB_FileM2.Visible = true;
                this.FU_HFileM2.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_FileM2.Text));


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
                string FileExtension = Path.GetExtension(FU_HFileM3.FileName);
                //檔名
                LB_FileM3.Text = "SC" + DateTime.Now.ToString("yyMMddmmssff") + FileExtension;
                LB_FileM3.Visible = true;
                this.FU_HFileM3.SaveAs(Server.MapPath("~/uploads/SpecialColumn/" + LB_FileM3.Text));


            }
        }

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Edit').modal('show');</script>", false);//執行js
    }
    #endregion

    #endregion

    #region 資料繫結
    protected void Rpt_HSCTopic_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ////判斷若還沒報名課程，則不能看內容
        //if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HCTemplateID")).Text))
        //{
        //    SqlDataReader QueryHCB = SQLdatabase.ExecuteReader("SELECT a.HID, a.HOrderGroup, a.HItemStatus FROM HCourseBooking AS a LEFT JOIN HCourse AS b ON a.HCourseID = b.HID WHERE b.HCTemplateID ='" + ((Label)e.Item.FindControl("LB_HCTemplateID")).Text + "' AND a.HMemberID = '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "' AND a.HStatus = '1'  AND a.HChangeStatus = '2'");

        //    while (!QueryHCB.Read())
        //    {
        //        ((HtmlControl)e.Item.FindControl("Div_NoPermission")).Visible = true;
        //        ((HtmlControl)e.Item.FindControl("Div_Show")).Visible = false;
        //    }
        //    QueryHCB.Close();
        //}

        string gSource = "1"; //1:新版專欄、2:EIP

        if (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HSCForumClassB")).Text))
        {
            ((Label)e.Item.FindControl("LB_HSCForumClassB")).Text = "大愛光老師專欄";
            ((Label)e.Item.FindControl("LB_HSCForumClassC")).Text = "EIP";
            gSource = "2";
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
        //GA20241225_新增
        //if (!string.IsNullOrEmpty((FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString()))
        //{
        //    if ((FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString().Split(',').Length == 2)
        //    {
        //        if ((FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString().Split(',')[1] == "1")
        //        {
        //            ((Label)e.Item.FindControl("LB_MoodIcon")).CssClass = "ti-thumb-up mr-2 text-info";
        //            ((Label)e.Item.FindControl("LB_MoodIcon")).Style.Add("font-size", "20px");
        //        }
        //        else if ((FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString().Split(',')[1] == "2")
        //        {
        //            ((Label)e.Item.FindControl("LB_MoodIcon")).CssClass = "fa fa-heart mr-2 text-danger";
        //            ((Label)e.Item.FindControl("LB_MoodIcon")).Style.Add("font-size", "20px");
        //        }
        //        else if ((FeelingCounts(((Label)e.Item.FindControl("LB_HID")).Text)).ToString().Split(',')[1] == "3")
        //        {
        //            ((Label)e.Item.FindControl("LB_MoodIcon")).CssClass = "ti-face-smile mr-2 text-success";
        //            ((Label)e.Item.FindControl("LB_MoodIcon")).Style.Add("font-size", "20px");
        //        }
        //    }
        //}


        #endregion

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

        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            var btnSeen = e.Item.FindControl("LBtn_Seen") as LinkButton;
            if (btnSeen != null)
            {
                // 讓這個按鈕觸發的是「局部回傳」而非整頁 PostBack
                ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btnSeen);
            }
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
                //DDL_HSCClassM.SelectedValue = !string.IsNullOrEmpty(QuerySCTopic["HSCClassID"].ToString()) ? QuerySCTopic["HSCClassID"].ToString():"0";
                //DDL_HSCRecordTypeM.SelectedValue = !string.IsNullOrEmpty(QuerySCTopic["HSCRecordTypeID"].ToString()) ? QuerySCTopic["HSCRecordTypeID"].ToString() : "0"; 

                //if (!string.IsNullOrEmpty(QuerySCTopic["HSCJiugonggeTypeID"].ToString()) && QuerySCTopic["HSCJiugonggeTypeID"].ToString() != "0")
                //{
                //    RBL_HSCJiugonggeTypeM.SelectedValue = QuerySCTopic["HSCJiugonggeTypeID"].ToString();
                //}

                //DDL_HGProgressM.SelectedValue = QuerySCTopic["HGProgress"].ToString();
                //TB_HOGProgressM.Text = QuerySCTopic["HOGProgress"].ToString();
                CKE_HContentM.Text = QuerySCTopic["HContent"].ToString();

                if (!string.IsNullOrEmpty(QuerySCTopic["HFile1"].ToString()))
                {
                    LB_File1.Visible = false;
                    LB_File1.Text = QuerySCTopic["HFile1"].ToString();
                    string[] gExtension = LB_File1.Text.Split('.');
                    if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "heic" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                    {
                        IMG_File1.Visible = true;
                        IMG_File1.ImageUrl = SCFile + LB_File1.Text;
                    }
                    else if (gExtension[1] == "mp3")
                    {
                        Audio1.Visible = true;
                        Source1.Src = SCFile + LB_File1.Text;
                    }
                }

                if (!string.IsNullOrEmpty(QuerySCTopic["HFile2"].ToString()))
                {
                    LB_File2.Visible = false;
                    LB_File2.Text = QuerySCTopic["HFile2"].ToString();
                    string[] gExtension = LB_File2.Text.Split('.');
                    if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "heic" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                    {
                        IMG_File2.Visible = true;
                        IMG_File2.ImageUrl = SCFile + LB_File2.Text;
                    }
                    else if (gExtension[1] == "mp3")
                    {
                        Audio2.Visible = true;
                        Source2.Src = SCFile + LB_File2.Text;
                    }
                }

                if (!string.IsNullOrEmpty(QuerySCTopic["HFile3"].ToString()))
                {
                    LB_File3.Visible = false;
                    LB_File3.Text = QuerySCTopic["HFile3"].ToString();
                    string[] gExtension = LB_File3.Text.Split('.');
                    if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "heic" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                    {
                        IMG_File3.Visible = true;
                        IMG_File3.ImageUrl = SCFile + LB_File3.Text;
                    }
                    else if (gExtension[1] == "mp3")
                    {
                        Audio3.Visible = true;
                        Source3.Src = SCFile + LB_File3.Text;
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
            cmd.Parameters.AddWithValue("@HHashTag", gLBox_HSCHotHashTag);
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


        SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC, f.HSCFCName AS HSCForumClassA,  HTopicName, HPinTop,HContent, HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, IIF(e.HID IS NULL,'0',e.HID) AS HMemberID,e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID LEFT JOIN HSCForumClass AS f ON c.HSCFCMaster=f.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 AND a.HSCForumClassID='" + LB_HSCForumClassCID.Text + "' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName , f.HSCFCName, a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY a.HPinTop DESC, a.HCreateDT DESC";
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

        SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC, f.HSCFCName AS HSCForumClassA,  HTopicName, HPinTop,HContent, HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, IIF(e.HID IS NULL,'0',e.HID) AS HMemberID,e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID LEFT JOIN HSCForumClass AS f ON c.HSCFCMaster=f.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 AND a.HSCForumClassID='" + LB_HSCForumClassCID.Text + "' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName , f.HSCFCName, a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY a.HPinTop DESC, a.HCreateDT DESC";
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

        SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC, f.HSCFCName AS HSCForumClassA,  HTopicName, HPinTop,HContent, HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, IIF(e.HID IS NULL,'0',e.HID) AS HMemberID,e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID LEFT JOIN HSCForumClass AS f ON c.HSCFCMaster=f.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 AND a.HSCForumClassID='" + LB_HSCForumClassCID.Text + "' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName , f.HSCFCName, a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY a.HPinTop DESC, a.HCreateDT DESC";
        Rpt_HSCTopic.DataBind();
    }
    #endregion

    #region 更新主題取消功能
    protected void Btn_UPDCancel_Click(object sender, EventArgs e)
    {

    }
    #endregion

    #region 取消我的最愛
    protected void LBtn_CancelFavorite_Click(object sender, EventArgs e)
    {
        string[] strFavoriteForumID = LB_HSCFavorite.Text.TrimEnd(',').TrimStart(',').Split(',');
        strFavoriteForumID = strFavoriteForumID.Where(val => val != LB_HSCForumClassCID.Text).ToArray();


        if (!string.IsNullOrEmpty(string.Join(",", strFavoriteForumID)))
        {

            SqlCommand upcmd = new SqlCommand("UPDATE HMember SET HSCFClassFavorite=@HSCFClassFavorite, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);

            ConStr.Open();
            upcmd.Parameters.AddWithValue("@HSCFClassFavorite", "," + string.Join(",", strFavoriteForumID) + ",");
            upcmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            upcmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            upcmd.Parameters.AddWithValue("@HID", ((Label)Master.FindControl("LB_HUserHID")).Text);

            upcmd.ExecuteNonQuery();
            ConStr.Close();
            upcmd.Cancel();

            Response.Write("<script>alert('成功取消我的最愛囉~!');window.location.href='HSCForumDetail.aspx?FClassCID=" + LB_HSCForumClassCID.Text + "';</script>");

            LBtn_HSCFClassFavorite.Visible = true;
            LBtn_CancelFavorite.Visible = false;
        }
        else
        {
            SqlCommand upcmd = new SqlCommand("UPDATE HMember SET HSCFClassFavorite=@HSCFClassFavorite, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);

            ConStr.Open();
            upcmd.Parameters.AddWithValue("@HSCFClassFavorite", "");
            upcmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            upcmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            upcmd.Parameters.AddWithValue("@HID", ((Label)Master.FindControl("LB_HUserHID")).Text);

            upcmd.ExecuteNonQuery();
            ConStr.Close();
            upcmd.Cancel();

            Response.Write("<script>alert('成功取消我的最愛囉~!');window.location.href='HSCForumDetail.aspx?FClassCID=" + LB_HSCForumClassCID.Text + "';</script>");

            LBtn_HSCFClassFavorite.Visible = true;
            LBtn_CancelFavorite.Visible = false;
        }

        //重新繫結Masterpage的SqlDataSource
        SqlDataSource SDS_HSCFClassFavorite = (SqlDataSource)this.Master.FindControl("SDS_HSCFClassFavorite");
        Repeater Rpt_HSCFClassFavorite = (Repeater)this.Master.FindControl("Rpt_HSCFClassFavorite");
        SDS_HSCFClassFavorite.SelectCommand = "SELECT LEFT(m.SCFCName, len(m.SCFCName) - 1) AS HSCFCName, LEFT(m.HSCForumClassID, len(m.HSCForumClassID) - 1)  AS HSCForumClassID FROM ( SELECT DISTINCT (cast(HSCForumClass.HSCFCName AS NVARCHAR) + ',') AS SCFCName,   (cast(HSCForumClass.HID AS NVARCHAR) + ',') AS HSCForumClassID    FROM HMember CROSS APPLY SPLIT(HSCFClassFavorite, ',') INNER JOIN HSCForumClass ON value = HSCForumClass.HID WHERE HMember.HID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "') AS m";
        Rpt_HSCFClassFavorite.DataBind();
    }

    #endregion

    #region 加入我的最愛
    protected void LBtn_HSCFClassFavorite_Click(object sender, EventArgs e)
    {
        SqlDataReader QuerySCFavorite = SQLdatabase.ExecuteReader("SELECT HSCFClassFavorite FROM HMember WHERE HID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");

        if (QuerySCFavorite.Read())
        {
            if (!string.IsNullOrEmpty(QuerySCFavorite["HSCFClassFavorite"].ToString()))
            {
                StringBuilder strHSCFClassFavorite = new StringBuilder();

                string[] gFavoriteSCFClassD = QuerySCFavorite["HSCFClassFavorite"].ToString().Split(',');
                string gCK = "0";
                for (int i = 0; i < gFavoriteSCFClassD.Length - 1; i++)
                {
                    if (gFavoriteSCFClassD[i].ToString() == LB_HSCForumClassCID.Text)
                    {
                        gCK = "1";
                    }
                }

                if (gCK == "1")
                {
                    //Response.Write("<script>alert('此討論區已加入過我的最愛囉~!');</script>");
                    //Response.Write("<script>confirm('確定要取消我的最愛嗎?');</script>");
                    //<未完成> 要觸發Btn_CancelFavorite_Click
                    //ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "if(confirm('確定要取消我的最愛嗎?')){alert('123');$('#ctl00_ContentPlaceHolder1_Btn_CancelFavorite').click();}", true);


                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('此討論區已加入過我的最愛囉~!');", true);
                    //ScriptManager.RegisterStartupScript(this, GetType(), "confirm", ("myFunction();"), true);



                }
                else
                {

                    strHSCFClassFavorite.Append(QuerySCFavorite["HSCFClassFavorite"].ToString() + LB_HSCForumClassCID.Text + ",");

                    SqlCommand upcmd = new SqlCommand("UPDATE HMember SET HSCFClassFavorite=@HSCFClassFavorite, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);

                    ConStr.Open();
                    upcmd.Parameters.AddWithValue("@HSCFClassFavorite", strHSCFClassFavorite.ToString());
                    upcmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    upcmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    upcmd.Parameters.AddWithValue("@HID", ((Label)Master.FindControl("LB_HUserHID")).Text);

                    upcmd.ExecuteNonQuery();
                    ConStr.Close();
                    upcmd.Cancel();

                    Response.Write("<script>alert('此討論區成功加入我的最愛囉~!');window.location.href='HSCForumDetail.aspx?FClassCID=" + LB_HSCForumClassCID.Text + "';</script>");

                    LBtn_HSCFClassFavorite.Visible = false;
                    LBtn_CancelFavorite.Visible = true;

                }


            }
            else
            {
                SqlCommand upcmd = new SqlCommand("UPDATE HMember SET HSCFClassFavorite=@HSCFClassFavorite, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);

                ConStr.Open();
                upcmd.Parameters.AddWithValue("@HSCFClassFavorite", "," + LB_HSCForumClassCID.Text + ",");
                upcmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                upcmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                upcmd.Parameters.AddWithValue("@HID", ((Label)Master.FindControl("LB_HUserHID")).Text);

                upcmd.ExecuteNonQuery();
                ConStr.Close();
                upcmd.Cancel();

                Response.Write("<script>alert('此討論區成功加入我的最愛囉~!');window.location.href='HSCForumDetail.aspx?FClassCID=" + LB_HSCForumClassCID.Text + "';</script>");

                LBtn_HSCFClassFavorite.Visible = false;
                LBtn_CancelFavorite.Visible = true;
            }
        }
        QuerySCFavorite.Close();

        //重新繫結Masterpage的SqlDataSource
        SqlDataSource SDS_HSCFClassFavorite = (SqlDataSource)this.Master.FindControl("SDS_HSCFClassFavorite");
        Repeater Rpt_HSCFClassFavorite = (Repeater)this.Master.FindControl("Rpt_HSCFClassFavorite");
        SDS_HSCFClassFavorite.SelectCommand = "SELECT LEFT(m.SCFCName, len(m.SCFCName) - 1) AS HSCFCName, LEFT(m.HSCForumClassID, len(m.HSCForumClassID) - 1)  AS HSCForumClassID FROM ( SELECT DISTINCT (cast(HSCForumClass.HSCFCName AS NVARCHAR) + ',') AS SCFCName,   (cast(HSCForumClass.HID AS NVARCHAR) + ',') AS HSCForumClassID    FROM HMember CROSS APPLY SPLIT(HSCFClassFavorite, ',') INNER JOIN HSCForumClass ON value = HSCForumClass.HID WHERE HMember.HID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "') AS m";
        Rpt_HSCFClassFavorite.DataBind();
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



    #region 主題-心情功能
    //HType：1=讚 /  2=愛心 / 3=微笑
    protected void LBtn_FeelingType_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        string gFeelingType_CA = btn.CommandArgument;
        //int gFeelingType_CN = Convert.ToInt32(btn.CommandName);

        var IBtn = sender as IButtonControl;
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
        Notification.AddNotification(((Label)RI.FindControl("LB_HMemberID")).Text, ((Label)Master.FindControl("LB_HUserHID")).Text, (Convert.ToInt32(btn.TabIndex.ToString()) + 1).ToString(), ((Label)RI.FindControl("LB_HID")).Text, "HSCTopic");


        SqlDataReader QueryFeeling = SQLdatabase.ExecuteReader("SELECT HID, HSCTopicID, HMemberID, HType FROM HSCTopic_Mood WHERE HSCTopicID='" + gFeelingType_CA + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        if (QueryFeeling.Read())
        {
            if (btn.TabIndex.ToString() != QueryFeeling["HType"].ToString())
            {
                SqlCommand cmd = new SqlCommand("UPDATE [HSCTopic_Mood] SET HType=@HType, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCTopicID=@HSCTopicID AND HMemberID=@HMemberID", ConStr);
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
            SqlCommand cmd = new SqlCommand("INSERT INTO [HSCTopic_Mood] (HSCTopicID, HMemberID, HType, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HType, @HStatus, @HCreate, @HCreateDT)", ConStr);
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

        //資料繫結
        SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC, f.HSCFCName AS HSCForumClassA,  HTopicName, HPinTop,HContent, HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, IIF(e.HID IS NULL,'0',e.HID) AS HMemberID,e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID LEFT JOIN HSCForumClass AS f ON c.HSCFCMaster=f.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 AND a.HSCForumClassID='" + LB_HSCForumClassCID.Text + "' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName ,f.HSCFCName,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY a.HPinTop DESC, a.HCreateDT DESC";
        Rpt_HSCTopic.DataBind();


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
    //protected void LBtn_FeelingTypeM_Click(object sender, EventArgs e)
    //{
    //    LinkButton btn = (LinkButton)sender;

    //    if (btn.TabIndex.ToString() == "1")
    //    {
    //        LBtn_ThumbsUp.CssClass = "nav-link  active show font-weight-bold";
    //        LBtn_Heart.CssClass = "nav-link font-weight-bold";
    //        LBtn_Smile.CssClass = "nav-link font-weight-bold";

    //        SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_SCTopicID.Text + "' AND a.HType='1' AND a.HStatus=1";
    //        Rpt_HSCTopicMood.DataBind();
    //        //計算數量
    //        //FeelingCounts(LB_SCTopicID.Text);
    //    }
    //    else if (btn.TabIndex.ToString() == "2")
    //    {
    //        LBtn_ThumbsUp.CssClass = "nav-link font-weight-bold";
    //        LBtn_Heart.CssClass = "nav-link  active show font-weight-bold";
    //        LBtn_Smile.CssClass = "nav-link font-weight-bold";

    //        SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_SCTopicID.Text + "' AND a.HType='2' AND a.HStatus=1";

    //        Rpt_HSCTopicMood.DataBind();
    //        //計算數量
    //        //FeelingCounts(LB_SCTopicID.Text);
    //    }
    //    else if (btn.TabIndex.ToString() == "3")
    //    {
    //        LBtn_ThumbsUp.CssClass = "nav-link font-weight-bold";
    //        LBtn_Heart.CssClass = "nav-link font-weight-bold";
    //        LBtn_Smile.CssClass = "nav-link  active show font-weight-bold";

    //        SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (b.HSystemName+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_SCTopicID.Text + "' AND a.HType='3' AND a.HStatus=1";

    //        Rpt_HSCTopicMood.DataBind();
    //        //計算數量
    //        //FeelingCounts(LB_SCTopicID.Text);

    //    }


    //    //Response.Write("EE= SELECT HMemberID, HType FROM HSCTopic_Mood WHERE HSCTopicID = '" + LB_SCTopicID.Text);
    //    //資料讀取
    //    SqlDataReader QueryFeeling = SQLdatabase.ExecuteReader("SELECT HMemberID, HType FROM HSCTopic_Mood WHERE HSCTopicID='" + LB_SCTopicID.Text + "'");

    //    int gThumbsUp = 0;
    //    int gLove = 0;
    //    int gSmile = 0;

    //    while (QueryFeeling.Read())
    //    {

    //        if (QueryFeeling["HType"].ToString() == "1")
    //        {
    //            gThumbsUp++;
    //        }
    //        else if (QueryFeeling["HType"].ToString() == "2")
    //        {
    //            gLove++;
    //        }
    //        else if (QueryFeeling["HType"].ToString() == "3")
    //        {
    //            gSmile++;
    //        }
    //    }
    //    QueryFeeling.Close();

    //    //<未完成>
    //    LBtn_ThumbsUp.Text = "<span class='ti-thumb-up mr-2'></span>" + gThumbsUp.ToString();
    //    LB_LoveNum.Text = "<span class='ti-heart-up mr-2'></span>" + gLove.ToString();
    //    LB_SmileNum.Text = "<span class='ti-face-smile mr-2'></span>" + gSmile.ToString();

    //    //Response.Write("GG="+ gThumbsUp);
    //    //LB_ThumbsUpNum.Text = gThumbsUp.ToString();

    //    //Response.Write("HH=" + LB_ThumbsUpNum.Text+"<br/>"+ gThumbsUp.ToString());
    //    //LB_LoveNum.Text = gLove.ToString("N0");
    //    //LB_SmileNum.Text = gSmile.ToString("N0");
    //    //Response.Write("AA=" + LB_SCTopicID.Text + "<br/>");



    //    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

    //}

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

    #region 熱門標籤搜尋
    protected void LBtn_HSCHotHashTag_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_HSCHotHashTag = sender as LinkButton;
        string gHSCHotHashTag_CA = LBtn_HSCHotHashTag.CommandArgument;

        string Sql = "SELECT a.HID, c.HSCFCName AS HSCForumClassB, b.HSCFCName AS HSCForumClassC, a.HTopicName, a.HPinTop, a.HContent, a.HFile1 , (e.HSystemName + ' ' + e.HArea + '/' + e.HPeriod + ' ' + e.HUserName) AS UserName, e.HImg, a.HCreateDT, a.HModifyDT, d.HCTemplateID FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID = b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster = c.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID = d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus = 1 AND ((a.HHashTag LIKE '%" + gHSCHotHashTag_CA + "%')) ORDER BY a.HPinTop DESC, a.HCreateDT DESC";

        Session["SearchIndex"] = Sql.ToString();

        Response.Redirect("HSCIndex.aspx");

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

        // 更新 UpdatePanel、顯示 modal
        UPD_Seen.Update();


    }
    #endregion

    protected void Rpt_HSCTopic_View_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        if (string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_LatestTime")).Text))
        {
            ((Label)e.Item.FindControl("LB_LatestTime")).Text = "-";
        }


    }

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
        SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC, f.HSCFCName AS HSCForumClassA,  HTopicName, HPinTop,HContent, HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, IIF(e.HID IS NULL,'0',e.HID) AS HMemberID,e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID LEFT JOIN HSCForumClass AS f ON c.HSCFCMaster=f.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 AND a.HSCForumClassID='" + LB_HSCForumClassCID.Text + "' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName ,f.HSCFCName,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY a.HPinTop DESC, a.HCreateDT DESC";
        //Rpt_HSCTopic.DataBind();

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
        //SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC, f.HSCFCName AS HSCForumClassA,  HTopicName, HPinTop,HContent, HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, IIF(e.HID IS NULL,'0',e.HID) AS HMemberID,e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID LEFT JOIN HSCForumClass AS f ON c.HSCFCMaster=f.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 AND a.HSCForumClassID='" + LB_HSCForumClassCID.Text + "' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName ,f.HSCFCName,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY a.HPinTop DESC, a.HCreateDT DESC";
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
        //SDS_HSCTopic.SelectCommand = "SELECT a.HID,  c.HSCFCName AS HSCForumClassB,  b.HSCFCName AS HSCForumClassC, f.HSCFCName AS HSCForumClassA,  HTopicName, HPinTop,HContent, HFile1 , (e.HSystemName+' '+ e.HArea+'/'+e.HPeriod +' '+ e.HUserName) AS UserName, IIF(e.HID IS NULL,'0',e.HID) AS HMemberID,e.HImg, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID LEFT JOIN HSCForumClass AS f ON c.HSCFCMaster=f.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID =d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 AND a.HSCForumClassID='" + LB_HSCForumClassCID.Text + "' GROUP BY a.HID,  a.HCourseID,  c.HSCFCName ,  b.HSCFCName ,f.HSCFCName,  a.HTopicName, a.HPinTop,a.HContent, a.HFile1 , e.HSystemName,e.HArea,e.HPeriod,e.HUserName,e.HID,e.HImg, a.HCreateDT, a.HModifyDT ORDER BY a.HPinTop DESC, a.HCreateDT DESC";
        Rpt_HSCTopic.DataBind();

        //計算數量
        FeelingCounts(gMood_CA);
    }
    #endregion
}