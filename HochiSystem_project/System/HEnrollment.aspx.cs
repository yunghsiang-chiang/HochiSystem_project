using MyWebControls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class System_HEnrollment : System.Web.UI.Page
{

    #region 分頁copy-1
    private int PageMax = 10;   //分頁最大顯示數量
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
            //Response.Write("<script language=javascript>alert('字符串含有非法字符！')</script>");
            //Response.Write("<script language=javascript>window.location.href='HCourse_Edit.aspx';</script>");
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //加入按Enter預設執行的按鈕
        this.Page.Form.DefaultButton = LBtn_Search.UniqueID;

        #region 分頁copy-2
        if (!IsPostBack)
        {
            ViewState["HCSearch"] = "";
            ViewState["TotalList"] = "";
            ViewState["TotalListArea"] = "";
        }

        if (!IsPostBack)
        {
            SDS_HC.SelectCommand = "SELECT a.HID, a.HCourseName, a.HTeacherName, a.HDateRange, b.HPlaceName, a.HBookByDateYN FROM HCourse as a join HPlace as b on a.HOCPlace=b.HID where a.HStatus='1' ORDER BY a.HDateRange DESC";
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HC.SelectCommand, PageMax, LastPage, false, Rpt_HC);
            ViewState["HCSearch"] = SDS_HC.SelectCommand;

        }
        else
        {
            SDS_HC.SelectCommand = ViewState["HCSearch"].ToString();
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", ViewState["HCSearch"].ToString(), PageMax, LastPage, false, Rpt_HC);


            SDS_TotalList.SelectCommand = ViewState["TotalList"].ToString();
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Rpt_TotalList.DataBind();
            //Pg_TotalList.FrontPagingLoad("HochiSystemConnection", ViewState["Search"].ToString(), PageMax, LastPage, false, Rpt_TotalList);


            SDS_TotalListArea.SelectCommand = ViewState["TotalListArea"].ToString();
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Rpt_TotalListArea.DataBind();

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

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        // 用來判斷現在是顯示哪一個Repeater
        List<RepeaterItem> itemsToProcess = new List<RepeaterItem>();

        if (Rpt_TotalList.Visible && Rpt_TotalList.Items.Count > 0)
        {
            // 單層 Repeater
            foreach (RepeaterItem item in Rpt_TotalList.Items)
            {
                itemsToProcess.Add(item);
            }
        }
        else if (Rpt_TotalListArea.Visible && Rpt_TotalListArea.Items.Count > 0)
        {
            // 多層（巢狀）Repeater
            foreach (RepeaterItem parentItem in Rpt_TotalListArea.Items)
            {
                Repeater childRepeater = parentItem.FindControl("Rpt_TotalListAreaChild") as Repeater;

                if (childRepeater != null)
                {
                    foreach (RepeaterItem childItem in childRepeater.Items)
                    {
                        itemsToProcess.Add(childItem);
                    }
                }
            }
        }

        // 欄位定義
        string[] colors = new string[] { "紅光", "橙光", "黃光", "綠光", "藍光", "靛光", "紫光" };
        string[] colorHex = new string[] { "#ffb2b2", "#ffd3b4", "#fff7ca", "#aad2b4", "#c4f3ff", "#8abdf1", "#d9c1ff" };
        string[] lights = new string[] { "金光", "銀光", "純光" };
        string[] lightscolor = new string[] { "#f5eec8", "#dddddd", "#f0f0f0" };
        string[] angelTypes = new string[] { "光使", "非光使" };

        // 統計資料
        Dictionary<string, Dictionary<string, Dictionary<string, int>>> statTable =
            new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();

        int undefinedTotal = 0; // 未定光系總數

        // 統一處理
        foreach (RepeaterItem item in itemsToProcess)
        {
            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
            {
                string color = ((Label)item.FindControl("LB_HRainbow")).Text.Trim();       // 七彩光
                string light = ((Label)item.FindControl("LB_HCarrier")).Text.Trim();       // 三載體光
                string angel = ((Label)item.FindControl("LB_HLightEnvoy")).Text.Trim() == "V" ? "光使" : "非光使";

                if (!colors.Contains(color))
                {
                    undefinedTotal++;
                    continue;
                }

                if (!statTable.ContainsKey(color))
                    statTable[color] = new Dictionary<string, Dictionary<string, int>>();

                if (!statTable[color].ContainsKey(light))
                    statTable[color][light] = new Dictionary<string, int>();

                if (!statTable[color][light].ContainsKey(angel))
                    statTable[color][light][angel] = 0;

                statTable[color][light][angel]++;
            }
        }

        // 產出 HTML 表格
        StringBuilder html = new StringBuilder();
        Dictionary<string, int> totalPerColumn = new Dictionary<string, int>();      // 每一欄（光使/非光使）的總數
        Dictionary<string, int> subTotalPerLight = new Dictionary<string, int>();    // 每一個三載體光的小計
        int totalAll = 0;

        html.Append("<table class='table table-bordered'><tr><th rowspan='2' class='text-center font-weight-bold' style='vertical-align: middle;'>光系</th>");

        // 第一排：載體光標題區
        for (int i = 0; i < lights.Length; i++)
        {
            string bg = lightscolor[i];
            html.Append("<th colspan='3' class='text-center font-weight-bold' style='background-color:" + bg + ";'>" + lights[i] + "</th>");
        }
        html.Append("<th rowspan='2' class='text-center font-weight-bold' style='vertical-align: middle;'>總計</th></tr><tr>");

        // 第二排：光使／非光使／小計
        for (int i = 0; i < lights.Length; i++)
        {
            for (int j = 0; j < angelTypes.Length; j++)
            {
                string key = lights[i] + "_" + angelTypes[j];
                totalPerColumn[key] = 0;
                html.Append("<th class='text-center font-weight-bold'>" + angelTypes[j] + "</th>");
            }

            string subtotalKey = lights[i] + "_小計";
            subTotalPerLight[lights[i]] = 0;
            html.Append("<th class='text-center font-weight-bold' style='background-color:" + lightscolor[i] + ";'>小計</th>");
        }
        html.Append("</tr>");

        // 內容資料列
        for (int c = 0; c < colors.Length; c++)
        {
            string color = colors[c];
            string bg = colorHex[c];
            html.Append("<tr><td class='text-center' style='background-color:" + bg + ";'>" + color + "</td>");

            int rowTotal = 0;

            for (int l = 0; l < lights.Length; l++)
            {
                string light = lights[l];
                int subTotal = 0;

                for (int a = 0; a < angelTypes.Length; a++)
                {
                    string angel = angelTypes[a];
                    string colKey = light + "_" + angel;

                    int count = 0;
                    if (statTable.ContainsKey(color) &&
                        statTable[color].ContainsKey(light) &&
                        statTable[color][light].ContainsKey(angel))
                    {
                        count = statTable[color][light][angel];
                    }

                    rowTotal += count;
                    subTotal += count;
                    totalPerColumn[colKey] += count;

                    html.Append("<td class='text-center'>" + (count > 0 ? count.ToString() : "0") + "</td>");
                }

                subTotalPerLight[light] += subTotal;
                html.Append("<td class='text-center font-weight-bold' style='background-color:" + lightscolor[l] + ";'>" + (subTotal > 0 ? subTotal.ToString() : "0") + "</td>");
            }

            totalAll += rowTotal;
            html.Append("<td class='text-center font-weight-bold'>" + (rowTotal > 0 ? rowTotal.ToString() : "0") + "</td>");
            html.Append("</tr>");
        }

        // 最底總計列
        html.Append("<tr><td class='text-center font-weight-bold'>小計</td>");
        for (int l = 0; l < lights.Length; l++)
        {
            string light = lights[l];
            int lightSubTotal = 0;

            for (int a = 0; a < angelTypes.Length; a++)
            {
                string colKey = light + "_" + angelTypes[a];
                int colTotal = totalPerColumn.ContainsKey(colKey) ? totalPerColumn[colKey] : 0;
                html.Append("<td class='text-center font-weight-bold'>" + colTotal.ToString() + "</td>");
                lightSubTotal += colTotal;
            }

            html.Append("<td class='text-center font-weight-bold' style='background-color:" + lightscolor[l] + ";'>" + lightSubTotal.ToString() + "</td>");
        }
        html.Append("<td class='text-center font-weight-bold'>" + totalAll.ToString() + "</td>");
        html.Append("</tr>");

        // 加入未定光系統計列
        if (undefinedTotal > 0)
        {
            int colspan = lights.Length * 3 + 1; // 光使/非光使/小計 × 載體光數量 + 光系欄
            html.Append("<tr><td colspan='" + colspan + "' class='text-right font-weight-bold text-danger'>未定光系人數統計</td>");
            html.Append("<td class='text-center font-weight-bold text-danger'>" + undefinedTotal + "</td></tr>");
        }

        // 最後一列：報名總人數（已統計未定 + 有定光系）
        int totalRegistered = totalAll + undefinedTotal;
        int fullColspan = lights.Length * 3 + 1;

        html.Append("<tr><td colspan='" + fullColspan + "' class='text-right font-weight-bold'>報名總人數</td>");
        html.Append("<td class='text-center font-weight-bold'>" + totalRegistered.ToString() + "</td></tr>");

        html.Append("</table>");

        // 顯示在頁面上
        Literal1.Text = html.ToString();


    }


    #region 搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {

        string[] gHDateRangeArray = TB_SearchDate.Text == "" ? "2000/01/01-3000/12/31".Split('-') : TB_SearchDate.Text.Split('-');
        string gHOCPlace = DDL_HOCPlace.SelectedValue == "0" ? "like '%'" : "='" + DDL_HOCPlace.SelectedValue + "'";//地點沒輸入則全找

        SDS_HC.SelectCommand = "SELECT a.HID, a.HCourseName, a.HTeacherName, a.HDateRange, b.HPlaceName, a.HBookByDateYN  FROM HCourse as a join HPlace as b on a.HOCPlace=b.HID WHERE HCourseName like N'%" + TB_Search.Text + "%' and b.HID " + gHOCPlace + " and ((left(HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and left(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "'and right(HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "')) and a.HStatus='1' ORDER BY a.HDateRange DESC";



        #region 分頁copy-3搜尋用
        ViewState["HCSearch"] = SDS_HC.SelectCommand;
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

        SDS_HC.SelectCommand = "SELECT a.HID, a.HCourseName, a.HTeacherName, a.HDateRange, b.HPlaceName, a.HBookByDateYN  FROM HCourse as a join HPlace as b on a.HOCPlace=b.HID where a.HStatus='1' ORDER BY a.HDateRange DESC";
        //SDS_HC.SelectCommand = "SELECT * FROM Course_BackendList where HStatus='1'";
        ViewState["HCSearch"] = SDS_HC.SelectCommand;

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


        



        //GA20250625_判斷是否開放單日報名
        if (gDRV["HBookByDateYN"].ToString() == "1")
        {
            ((LinkButton)e.Item.FindControl("LBtn_TotalListArea")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_Identity")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_TotalList")).Visible = false;
            

            ((LinkButton)e.Item.FindControl("LBtn_BBDate")).Visible = true;
            //GE20240409_修改OrderList_Merge改成直接寫
            SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT COUNT(a.HUserName) AS  HApplyNum FROM MemberList as a join (SELECT  A.HMemberID, A.HCourseID FROM HCourseBooking AS A INNER JOIN HCourse AS B On A.HCourseID = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND (A.HChangeStatus IS NULL OR A.HChangeStatus = '2') GROUP BY A.HCourseID, A.HMemberID) as b on a.HID = b.HMemberID where b.HCourseID =  '" + gDRV["HID"] + "'");


            if (dr.Read())
            {
                ((Label)e.Item.FindControl("LB_HApplyNum")).Text = dr["HApplyNum"].ToString();
            }
            dr.Close();
        }
        else
        {
            ((LinkButton)e.Item.FindControl("LBtn_TotalListArea")).Visible = true;
            ((LinkButton)e.Item.FindControl("LBtn_BBDate")).Visible = false;

            //GE20240409_修改OrderList_Merge改成直接寫
            SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT COUNT(a.HUserName) AS  HApplyNum FROM MemberList as a join (SELECT  A.HMemberID, A.HCourseID FROM HCourseBooking AS A INNER JOIN HCourse AS B On A.HCourseID = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND (A.HChangeStatus IS NULL OR A.HChangeStatus = '2') UNION SELECT A.HMemberID, D.HCourseIDNew AS HCourseID FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON A.HOrderGroup = D.HOrderGroupNew INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND A.HCourseName = '換課' AND(D.HChangeStatus IS NULL OR D.HChangeStatus = '2') UNION SELECT A.HMemberID, D.HCourseIDNew AS HCourseID FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON(A.HOrderGroup = D.HOrderGroupSrc AND A.HOrderNum = D.HOrderNumSrc) OR(A.HOrderGroup = D.HOrderGroupSrc AND D.HOrderNumSrc = '') INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND(D.HOrderGroupNew = '' OR D.HOrderGroupNew IS NULL) UNION SELECT  A.HMemberID, D.HCourseIDNew AS HCourseID FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON(A.HOrderGroup = D.HOrderGroupSrc) LEFT JOIN HCBChangeRecord AS E ON(D.HOrderGroupSrc = E.HOrderGroupNew) INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND A.HCourseName = '換課' AND(D.HOrderGroupNew = '' OR D.HOrderGroupNew IS NULL) AND(E.HChangeStatus = '1') AND(A.HChangeStatus IS NULL OR A.HChangeStatus = '2')) as b on a.HID = b.HMemberID where b.HCourseID = '" + gDRV["HID"] + "'");


            if (dr.Read())
            {
                ((Label)e.Item.FindControl("LB_HApplyNum")).Text = dr["HApplyNum"].ToString();
            }
            dr.Close();

        }
    }

    protected void Rpt_TotalList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        #region 光系

        ((Label)e.Item.FindControl("LB_HCarrier")).Text = gDRV["HCarrier"].ToString() == "1" ? "金光" : gDRV["HCarrier"].ToString() == "2" ? "銀光" : gDRV["HCarrier"].ToString() == "3" ? "純光" : "未定";

        ((Label)e.Item.FindControl("LB_HRainbow")).Text = gDRV["HRainbow"].ToString() == "1" ? "紅光" : gDRV["HRainbow"].ToString() == "2" ? "橙光" : gDRV["HRainbow"].ToString() == "3" ? "黃光" : gDRV["HRainbow"].ToString() == "4" ? "綠光" : gDRV["HRainbow"].ToString() == "5" ? "藍光" : gDRV["HRainbow"].ToString() == "6" ? "靛光" : gDRV["HRainbow"].ToString() == "7" ? "紫光" : "未定";

        ((Label)e.Item.FindControl("LB_HLightEnvoy")).Text = gDRV["HLightEnvoy"].ToString() == "1" ? "V" : "";


        if (gDRV["HCarrier"].ToString() != "0" && gDRV["HRainbow"].ToString() != "0")
        {
            string BasicPath = AppDomain.CurrentDomain.BaseDirectory;


            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "light_data.json");
            LightDataCache.LoadData(path);

            var allData = LightDataCache.GetAll();
            var first = LightDataCache.GetById(1);

            var result = LightDataCache.GetAll().Find(light => light.HCarrier == Convert.ToInt32(gDRV["HCarrier"].ToString()) && light.HRainbow == Convert.ToInt32(gDRV["HRainbow"].ToString()));

            ((Label)e.Item.FindControl("LB_HLightName")).Text = result.HID + " " + result.HLightName;
        }
        else
        {
            ((Label)e.Item.FindControl("LB_HLightName")).Text = "未定光系";
        }
        #endregion

        string gHTeamType = "";
        string gHTeamName = "";
        string[] gHTeamID = ((Label)e.Item.FindControl("LB_HTeamID")).Text.Split(',');
        if (gHTeamID.Length > 1)
        {
            gHTeamType = gHTeamID[1] == "1" ? "HMTeam" : "HCTeam";
            gHTeamName = gHTeamID[1] == "1" ? "HMTeam" : "HCTeam";
            SqlDataReader QueryHTeam = SQLdatabase.ExecuteReader("select " + gHTeamName + " from " + gHTeamType + " where HID='" + gHTeamID[0] + "'");
            while (QueryHTeam.Read())
            {
                ((Label)e.Item.FindControl("LB_HTeamID")).Text = QueryHTeam[gHTeamName].ToString();
            }
            QueryHTeam.Close();

        }
        else
        {
            ((Label)e.Item.FindControl("LB_HTeamID")).Text = "";
        }

         //1=男、2=女
         ((Label)e.Item.FindControl("LB_HSex")).Text = gDRV["HSex"].ToString() == "1" ? "男" : "女";


        //GE20231017_更正1 = 參班【一般】、5 = 純護持(非班員)、6 = 參班兼護持
        ((Label)e.Item.FindControl("LB_HAttend")).Text = gDRV["HAttend"].ToString() == "5" ? "純護持(非班員)" : gDRV["HAttend"].ToString() == "6" ? "參班兼護持" : gDRV["HAttend"].ToString() == "" || gDRV["HAttend"].ToString() == null ? "未選擇" : gDRV["HAttend"].ToString() == "0" ? "" : "參班";
    }

    int ApplyNumTLA = 0;//報名人數

    protected void Rpt_TotalListArea_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        //221005-合併重複資料(調整期別轉成文字)
        //230731-報名完成條件式改為HCourseBooking.HStatus=1 AND HItemStatus=1
        //TE20230817_同學員工作項目合併顯示
        //GE20230930_將HCourseBooking改成OrderList_Merge

        //GE20231122_加入體系資訊&不用再join HCourse&HPlace
        //TE20240322_加入排序：光團->體系
        //GE20250327_加入三載體光、七彩光、21道光、是否為光使
        ((SqlDataSource)e.Item.FindControl("SDS_TotalListAreaChild")).SelectCommand = "SELECT  M.HArea, M.HSystemName, M.HTeamID, M.HPeriod, M.HID, M.HUserName,M.HCarrier, M.HRainbow, M.HLightEnvoy,  M.HAccount, M.HPhone,M.HCourseID, M.HDateRange, M.HSex, M.HAttend, M.HOCPlace, M.HLDate, M.HPlaceName,M.HMemberGroup, M.HRemark, M.HRoomName, M.HRoomTime, LEFT(M.HTask, len(M.HTask) - 1) AS HTask FROM (SELECT a.HAreaID, a.HArea, a.HSystemName, a.HTeamID, a.HPeriod, a.HID, a.HUserName, a.HAccount, a.HPhone, a.HCarrier, a.HRainbow, a.HLightEnvoy, b.HCourseID, b.HDateRange, a.HSex, b.HAttend, b.HOCPlace, b.HLDate, b.HPlaceName,b.HMemberGroup, b.HRemark, g.HRoomName, b.HRoomTime, b.HItemStatus, b.HStatus , (SELECT DISTINCT (cast(HTask AS NVARCHAR) + '、')  FROM HCourseBooking_Group CROSS APPLY SPLIT(HTask, '、') where HCourseBooking_Group.HBookingID=b.HID   FOR XML PATH(''))   AS HTask FROM MemberList AS a join OrderList_Merge as b on a.HID = b.HMemberID left join HArea as d on a.HAreaID=d.HID left join HCourseBooking_Group as f on b.HID = f.HBookingID left join HRoom as g on b.HRoom =g.HID GROUP BY a.HAreaID, a.HArea,a.HSystemName, a.HTeamID, a.HPeriod, a.HID, a.HUserName, a.HAccount, a.HPhone,a.HCarrier, a.HRainbow, a.HLightEnvoy,  b.HCourseID, b.HDateRange, a.HSex, b.HAttend, b.HOCPlace, b.HLDate, b.HPlaceName, b.HRemark,b.HMemberGroup, b.HItemStatus, b.HStatus, g.HRoomName, b.HRoomTime, b.HID) M WHERE M.HCourseID='" + gDRV["HCourseID"].ToString() + "' and M.HAreaID='" + gDRV["HAreaID"].ToString() + "'  and M.HStatus='1' and M.HItemStatus='1' GROUP BY M.HArea,M.HSystemName, M.HTeamID, M.HPeriod, M.HID, M.HUserName, M.HCarrier, M.HRainbow, M.HLightEnvoy,  M.HAccount, M.HPhone,M.HCourseID, M.HDateRange, M.HSex, M.HAttend, M.HOCPlace, M.HLDate, M.HPlaceName,M.HMemberGroup, M.HRemark, M.HRoomName, M.HRoomTime, M.HTask ORDER BY M.HTeamID DESC,  M.HSystemName DESC";



        //GE20230905_加入人數統計
        ((SqlDataSource)e.Item.FindControl("SDS_TotalListAreaChild")).DataBind();
        ((Repeater)e.Item.FindControl("Rpt_TotalListAreaChild")).DataBind();
        ((Label)e.Item.FindControl("LB_Count")).Text = "(" + (((Repeater)e.Item.FindControl("Rpt_TotalListAreaChild")).Items.Count).ToString() + ")";

        //TA20240323_加入報名人數
        if (((Repeater)e.Item.FindControl("Rpt_TotalListAreaChild")).Items.Count != 0)
        {
            ApplyNumTLA += ((Repeater)e.Item.FindControl("Rpt_TotalListAreaChild")).Items.Count;
        }
        LB_ApplyNumTLA.Text = (ApplyNumTLA).ToString();
    }

    protected void Rpt_TotalListAreaChild_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        #region 光系

        ((Label)e.Item.FindControl("LB_HCarrier")).Text = gDRV["HCarrier"].ToString() == "1" ? "金光" : gDRV["HCarrier"].ToString() == "2" ? "銀光" : gDRV["HCarrier"].ToString() == "3" ? "純光" : "未定";

        ((Label)e.Item.FindControl("LB_HRainbow")).Text = gDRV["HRainbow"].ToString() == "1" ? "紅光" : gDRV["HRainbow"].ToString() == "2" ? "橙光" : gDRV["HRainbow"].ToString() == "3" ? "黃光" : gDRV["HRainbow"].ToString() == "4" ? "綠光" : gDRV["HRainbow"].ToString() == "5" ? "藍光" : gDRV["HRainbow"].ToString() == "6" ? "靛光" : gDRV["HRainbow"].ToString() == "7" ? "紫光" : "未定";



        ((Label)e.Item.FindControl("LB_HLightEnvoy")).Text = gDRV["HLightEnvoy"].ToString() == "1" ? "V" : "";


        if (gDRV["HCarrier"].ToString() != "0" && gDRV["HRainbow"].ToString() != "0")
        {
            string BasicPath = AppDomain.CurrentDomain.BaseDirectory;


            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "light_data.json");
            LightDataCache.LoadData(path);

            var allData = LightDataCache.GetAll();
            var first = LightDataCache.GetById(1);

            var result = LightDataCache.GetAll().Find(light => light.HCarrier == Convert.ToInt32(gDRV["HCarrier"].ToString()) && light.HRainbow == Convert.ToInt32(gDRV["HRainbow"].ToString()));

            ((Label)e.Item.FindControl("LB_HLightName")).Text = result.HID + " " + result.HLightName;
        }
        else
        {
            ((Label)e.Item.FindControl("LB_HLightName")).Text = "未定光系";
        }
        #endregion


        string gHTeamType = "";
        string gHTeamName = "";
        string[] gHTeamID = ((Label)e.Item.FindControl("LB_HTeamID")).Text.Split(',');
        if (gHTeamID.Length > 1)
        {
            gHTeamType = gHTeamID[1] == "1" ? "HMTeam" : "HCTeam";
            gHTeamName = gHTeamID[1] == "1" ? "HMTeam" : "HCTeam";
            SqlDataReader QueryHTeam = SQLdatabase.ExecuteReader("select " + gHTeamName + " from " + gHTeamType + " where HID='" + gHTeamID[0] + "'");
            while (QueryHTeam.Read())
            {
                ((Label)e.Item.FindControl("LB_HTeamID")).Text = QueryHTeam[gHTeamName].ToString();
            }
            QueryHTeam.Close();
        }
        else
        {
            ((Label)e.Item.FindControl("LB_HTeamID")).Text = "";
        }

     //1=男、2=女
     ((Label)e.Item.FindControl("LB_HSex")).Text = gDRV["HSex"].ToString() == "1" ? "男" : "女";

        //GE20231017_更正1 = 參班【一般】、5 = 純護持(非班員)、6 = 參班兼護持
        ((Label)e.Item.FindControl("LB_HAttend")).Text = gDRV["HAttend"].ToString() == "5" ? "純護持(非班員)" : gDRV["HAttend"].ToString() == "6" ? "參班兼護持" : gDRV["HAttend"].ToString() == "" || gDRV["HAttend"].ToString() == null ? "未選擇" : gDRV["HAttend"].ToString() == "0" ? "" : "參班";














    }

    #region 依組別
    protected void LBtn_TotalList_Click(object sender, EventArgs e)
    {
        Panel_CourseList.Visible = false;
        Panel_TotalList.Visible = true;
        Panel_Search.Visible = false;
        Panel_BBDateBookingList.Visible = false;
        Panel_GeneralCourseView.Visible = true;

        LinkButton LBtn_TotalList = sender as LinkButton;
        string TotalList_CA = LBtn_TotalList.CommandArgument;


        //221005-合併重複資料
        //221005-合併重複資料(調整期別轉成文字)
        //TE20230817_工作項目顯示、語法調整
        //GE20230930_將HCourseBooking改成OrderList_Merge
        //GE20231122_加入體系資訊&不用再join HCourse&HPlace
        //TE20240322_加入排序：光團->體系
        //GE20250326_加入三載體光、七彩光、二十一道光、光使
        SDS_TotalList.SelectCommand = "SELECT M.HSortL, M.HArea, M.HSystemName, M.HSort, M.HTeamID, M.Period, M.HID, M.HUserName, M.HCarrier, M.HRainbow, M.HLightEnvoy, M.HAccount, M.HPhone, M.HCourseID, M.HDateRange, M.HSex, M.HAttend, M.HOCPlace, M.HLDate, M.HPlaceName, M.HMemberGroup, M.HRemark, M.HRoomName, M.HRoomTime, M.HStatus, M.HItemStatus, LEFT(M.HTask, len(M.HTask) - 1) AS HTask FROM(SELECT h.HSort AS HSortL, a.HAreaID, a.HArea, a.HSystemName, d.HSort, a.HTeamID, (N'' + a.HPeriod + '') AS Period, a.HID, a.HUserName, a.HAccount, a.HPhone, a.HCarrier, a.HRainbow, a.HLightEnvoy, b.HCourseID, b.HDateRange, a.HSex, b.HAttend, b.HOCPlace, b.HLDate, b.HPlaceName, b.HMemberGroup, b.HRemark, g.HRoomName, b.HRoomTime, b.HStatus, b.HItemStatus, (SELECT DISTINCT(cast(HTask AS NVARCHAR) + '、')  FROM HCourseBooking_Group CROSS APPLY SPLIT(HTask, '、') where HCourseBooking_Group.HBookingID = b.HID  FOR XML PATH(''))  AS HTask FROM MemberList AS a JOIN OrderList_Merge AS b ON a.HID = b.HMemberID  LEFT JOIN HArea AS d ON a.HAreaID = d.HID  LEFT JOIN HCourseBooking_Group AS f ON b.HID = f.HBookingID LEFT JOIN HRoom AS g ON b.HRoom = g.HID LEFT JOIN HLArea AS h ON d.HLAreaID = h.HID GROUP BY h.Hsort, a.HAreaID, a.HArea,a.HSystemName, d.HSort, a.HTeamID, a.HPeriod, a.HID, a.HUserName,a.HAccount, a.HPhone,a.HCarrier, a.HRainbow, a.HLightEnvoy,  b.HCourseID, b.HDateRange, a.HSex, b.HAttend, b.HOCPlace, b.HLDate, b.HPlaceName,b.HMemberGroup, b.HRemark, g.HRoomName, b.HRoomTime, b.HStatus, b.HItemStatus,b.HID ) M WHERE M.HCourseID =  '" + TotalList_CA + "' AND M.HStatus = '1' AND M.HItemStatus = '1' ORDER BY HAreaID ASC, M.HTeamID DESC,  M.HSystemName DESC";
        //ORDER BY M.HMemberGroup, M.HSort


        Rpt_TotalList.DataBind();

        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
        //Pg_TotalList.FrontPagingLoad("HochiSystemConnection", SDS_TotalList.SelectCommand, PageMax, LastPage, true, Rpt_TotalList);

        ViewState["TotalList"] = SDS_TotalList.SelectCommand;


        //GE20230908_先清空
        LB_HTeacherNameTLTitle.Text = null;


        SqlDataReader QueryHCourse = SQLdatabase.ExecuteReader("select a.HCourseName, a.HTeacherName, a.HDateRange from HCourse as a where a.HID='" + TotalList_CA + "'");

        while (QueryHCourse.Read())
        {
            LB_HCourseNameTLTitle.Text = QueryHCourse["HCourseName"].ToString();


            string[] gHTeacherName = QueryHCourse["HTeacherName"].ToString().Split(',');
            for (int i = 0; i < gHTeacherName.Length - 1; i++)
            {
                //Response.Write("SELECT HUserName FROM HMember where HID = '" + gHTeacherName[i].ToString() + "'");
                SqlDataReader QueryHTeacher = SQLdatabase.ExecuteReader("SELECT B.HUserName FROM HTeacher AS A JOIN HMember AS B ON A.HMemberID =B.HID where A.HID = '" + gHTeacherName[i].ToString() + "'");
                if (QueryHTeacher.Read())
                {

                    LB_HTeacherNameTLTitle.Text = QueryHTeacher["HUserName"].ToString() + "," + LB_HTeacherNameTLTitle.Text;
                }
                QueryHTeacher.Close();
            }

            LB_HTeacherNameTLTitle.Text = LB_HTeacherNameTLTitle.Text.Length > 0 ? "講師：" + LB_HTeacherNameTLTitle.Text.Substring(0, LB_HTeacherNameTLTitle.Text.Length - 1) : "講師：";

            LB_HDateRangeTLTitle.Text = "課程日期：" + QueryHCourse["HDateRange"].ToString();


        }

        QueryHCourse.Close();

    }
    #endregion

    #region 依身分別
    protected void LBtn_Identity_Click(object sender, EventArgs e)
    {
        Panel_CourseList.Visible = false;
        Panel_TotalList.Visible = false;
        Panel_Identity.Visible = true;
        Panel_TotalListArea.Visible = false;
        Panel_Search.Visible = false;

        LinkButton LBtn_Identity = sender as LinkButton;
        string Identity_CA = LBtn_Identity.CommandArgument;

        //230731-報名完成條件式改為HCourseBooking.HStatus=1 AND HItemStatus=1
        //GE20230930_將HCourseBooking改成OrderList_Merge
        //GE20231122_不用再join HCourse
        //GE20240409_將OrderList_Merge改寫
        SqlDataReader QueryHMCBCA = SQLdatabase.ExecuteReader("select a.HAreaID, d.HArea, a.HTeamID, (N''''+a.HPeriod) AS Period , a.HID, a.HUserName, b.HCourseID, b.HDateRange, a.HSex, isnull(b.HAttend,0) as HAttend,f.HOCPlace, b.HLDate, b.HRemark from HMember as a  JOIN (SELECT  A.HMemberID, A.HCourseID, A.HDateRange, A.HAttend, A.HLDate,A.HRemark FROM HCourseBooking AS A INNER JOIN HCourse AS B On A.HCourseID = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND (A.HChangeStatus IS NULL OR A.HChangeStatus = '2') UNION SELECT A.HMemberID, D.HCourseIDNew AS HCourseID, D.HDateRange,  D.HAttendNew AS HAttend, D.HLDate,D.HRemark FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON A.HOrderGroup = D.HOrderGroupNew INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND A.HCourseName = '換課' AND(D.HChangeStatus IS NULL OR D.HChangeStatus = '2') UNION SELECT A.HMemberID, D.HCourseIDNew AS HCourseID, D.HDateRange,  D.HAttendNew AS HAttend, D.HLDate,D.HRemark FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON(A.HOrderGroup = D.HOrderGroupSrc AND A.HOrderNum = D.HOrderNumSrc) OR(A.HOrderGroup = D.HOrderGroupSrc AND D.HOrderNumSrc = '') INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND(D.HOrderGroupNew = '' OR D.HOrderGroupNew IS NULL) UNION SELECT  A.HMemberID, D.HCourseIDNew AS HCourseID, D.HDateRange,  D.HAttendNew AS HAttend, D.HLDate,D.HRemark FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON(A.HOrderGroup = D.HOrderGroupSrc) LEFT JOIN HCBChangeRecord AS E ON(D.HOrderGroupSrc = E.HOrderGroupNew) INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND A.HCourseName = '換課' AND(D.HOrderGroupNew = '' OR D.HOrderGroupNew IS NULL) AND(E.HChangeStatus = '1') AND(A.HChangeStatus IS NULL OR A.HChangeStatus = '2')) as b on a.HID = b.HMemberID LEFT JOIN HCourse AS f ON b.HCourseID=f.HID LEFT JOIN HArea AS d ON a.HAreaID=d.HID WHERE b.HCourseID='" + Identity_CA + "'  ORDER BY b.HCourseID");

        while (QueryHMCBCA.Read())
        {

            //230323-修改
            //TE20240318_加入HAttend=6(參班兼護持)
            //1 = 參班【一般】、5 = 不參班(專業護持)、6=參班兼護持
            if (QueryHMCBCA["HAttend"].ToString() == "1" || QueryHMCBCA["HAttend"].ToString() == "2" || QueryHMCBCA["HAttend"].ToString() == "3" || QueryHMCBCA["HAttend"].ToString() == "4")
            {
                LB_Attend.Text = Convert.ToString(Convert.ToInt32(LB_Attend.Text) + 1);
            }
            else if (QueryHMCBCA["HAttend"].ToString() == "5")
            {
                LB_Guide.Text = Convert.ToString(Convert.ToInt32(LB_Guide.Text) + 1);
            }
            else if (QueryHMCBCA["HAttend"].ToString() == "6")
            {
                LB_ProGuide.Text = Convert.ToString(Convert.ToInt32(LB_ProGuide.Text) + 1);
            }

        }

        QueryHMCBCA.Close();

        double gTotal = 0;
        gTotal = Convert.ToInt32(LB_Attend.Text) + Convert.ToInt32(LB_ProGuide.Text) + Convert.ToInt32(LB_Guide.Text);



        //TE20230817_取到小數點第二位
        LB_AttendPCT.Text = Math.Round(((Convert.ToInt32(LB_Attend.Text) / gTotal) * 100), 2).ToString();
        LB_ProGuidePCT.Text = Math.Round(((Convert.ToInt32(LB_ProGuide.Text) / gTotal) * 100), 2).ToString();
        LB_GuidePCT.Text = Math.Round(((Convert.ToInt32(LB_Guide.Text) / gTotal) * 100), 2).ToString();



        //GE20230908_先清空
        LB_HTeacherNameITitle.Text = null;


        SqlDataReader QueryHCourse = SQLdatabase.ExecuteReader("select a.HCourseName, a.HTeacherName, a.HDateRange from HCourse as a where a.HID='" + Identity_CA + "'");

        while (QueryHCourse.Read())
        {
            LB_HCourseNameITitle.Text = QueryHCourse["HCourseName"].ToString();


            string[] gHTeacherName = QueryHCourse["HTeacherName"].ToString().Split(',');
            for (int i = 0; i < gHTeacherName.Length - 1; i++)
            {
                SqlDataReader QueryHTeacher = SQLdatabase.ExecuteReader("SELECT B.HUserName FROM HTeacher AS A JOIN HMember AS B ON A.HMemberID =B.HID where A.HID = '" + gHTeacherName[i].ToString() + "'");
                if (QueryHTeacher.Read())
                {

                    LB_HTeacherNameITitle.Text = QueryHTeacher["HUserName"].ToString() + "," + LB_HTeacherNameITitle.Text;
                }
                QueryHTeacher.Close();
            }

            LB_HTeacherNameITitle.Text = LB_HTeacherNameITitle.Text.Length > 0 ? "講師：" + LB_HTeacherNameITitle.Text.Substring(0, LB_HTeacherNameITitle.Text.Length - 1) : "講師：";

            LB_HDateRangeITitle.Text = "課程日期：" + QueryHCourse["HDateRange"].ToString();


        }

        QueryHCourse.Close();




    }
    #endregion

    #region 依區屬
    protected void LBtn_TotalListArea_Click(object sender, EventArgs e)
    {
        Panel_CourseList.Visible = false;
        Panel_TotalList.Visible = false;
        Panel_Search.Visible = false;
        Panel_TotalListArea.Visible = true;

        LinkButton LBtn_TotalListArea = sender as LinkButton;
        string TotalListArea_CA = LBtn_TotalListArea.CommandArgument;

        //230731-報名完成條件式改為HCourseBooking.HStatus=1 AND HItemStatus=1
        //GE20230905_修正出現重複的資料(拿掉HMemberGroup欄位)
        //GE20230930_將HCourseBooking改成OrderList_Merge
        //SDS_TotalListArea.SelectCommand = "select e.HSort AS HSortL, a.HAreaID, d.HArea, d.HSort, b.HCourseID from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID join HCourse as c on b.HCourseID = c.HID left join HArea as d on a.HAreaID=d.HID LEFT JOIN HLArea AS e ON d.HLAreaID = e.HID where b.HCourseID='" + TotalListArea_CA + "' and b.HStatus='1' and b.HItemStatus='1' GROUP BY d.HArea, a.HAreaID, b.HCourseID, e.HSort, d.HSort ORDER BY  e.HSort, d.HSort";

        //GE20231122_不用再join HCourse
        SDS_TotalListArea.SelectCommand = "select e.HSort AS HSortL, a.HAreaID, d.HArea, d.HSort, b.HCourseID from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID left join HArea as d on a.HAreaID=d.HID LEFT JOIN HLArea AS e ON d.HLAreaID = e.HID where b.HCourseID='" + TotalListArea_CA + "' and b.HStatus='1' and b.HItemStatus='1' GROUP BY d.HArea, a.HAreaID, b.HCourseID, e.HSort, d.HSort ORDER BY  e.HSort, d.HSort";

        Rpt_TotalListArea.DataBind();


        ViewState["TotalListArea"] = SDS_TotalListArea.SelectCommand;


        //GE20230908_先清空
        LB_HTeacherNameTLATitle.Text = null;

        SqlDataReader QueryHCourse = SQLdatabase.ExecuteReader("select a.HCourseName, a.HTeacherName, a.HDateRange from HCourse as a where a.HID='" + TotalListArea_CA + "'");


        while (QueryHCourse.Read())
        {
            LB_HCourseNameTLATitle.Text = QueryHCourse["HCourseName"].ToString();


            string[] gHTeacherName = QueryHCourse["HTeacherName"].ToString().Split(',');
            for (int i = 0; i < gHTeacherName.Length - 1; i++)
            {
                //GE20230908_加入空值判斷
                if (!string.IsNullOrEmpty(gHTeacherName[i].ToString()))
                {
                    SqlDataReader QueryHTeacher = SQLdatabase.ExecuteReader("SELECT B.HUserName FROM HTeacher AS A JOIN HMember AS B ON A.HMemberID =B.HID where A.HID = '" + gHTeacherName[i].ToString() + "'");

                    if (QueryHTeacher.Read())
                    {
                        LB_HTeacherNameTLATitle.Text = QueryHTeacher["HUserName"].ToString() + "," + LB_HTeacherNameTLATitle.Text;
                    }
                    QueryHTeacher.Close();
                }

            }

            LB_HTeacherNameTLATitle.Text = LB_HTeacherNameTLATitle.Text.Length > 0 ? "講師：" + LB_HTeacherNameTLATitle.Text.Substring(0, LB_HTeacherNameTLATitle.Text.Length - 1) : "講師：";

            LB_HDateRangeTLATitle.Text = "課程日期：" + QueryHCourse["HDateRange"].ToString();


        }

        QueryHCourse.Close();



    }
    #endregion




    #region 匯出Excel (沒有用到)
    protected void IBtn_ToExcel_Click(object sender, ImageClickEventArgs e)
    {


        Response.Clear();
        //通知瀏覽器下載檔案
        Response.AddHeader("content-disposition", "attachment;filename=單一課程報名總名單.xls");
        Response.ContentType = "application/application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        System.IO.StringWriter sw = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter hw = new HtmlTextWriter(sw);

        ToExcelList.RenderControl(hw);

        System.IO.StreamWriter swUTF8 = new System.IO.StreamWriter(Response.OutputStream, System.Text.Encoding.UTF8);//這樣一筆的時侯,就不會亂碼了
        swUTF8.Write(sw.ToString());
        swUTF8.Close();

        Response.End();
    }
    #endregion





    #region 光系
    public class LightInfo
    {
        public int HID { get; set; }
        public int HCarrier { get; set; }
        public int HRainbow { get; set; }
        public string HLightName { get; set; }
        public int HSort { get; set; }
        public int HStatus { get; set; }
    }

    public static class LightDataCache
    {
        private static List<LightInfo> _lightData;

        public static void LoadData(string filePath)
        {
            var json = File.ReadAllText(filePath);
            _lightData = JsonConvert.DeserializeObject<List<LightInfo>>(json);
        }

        public static List<LightInfo> GetAll()
        {
            return _lightData;
        }

        public static LightInfo GetById(int id)
        {
            if (_lightData == null)
                return null;

            return _lightData.Find(x => x.HID == id);
        }
    }
    #endregion




    string gPlaceName = null;

    #region 單日報名的課程產生對應的報表
    protected void Btn_BBDate_Click(object sender, EventArgs e)
    {

        Panel_CourseList.Visible = false;
        Panel_TotalList.Visible = true;
        Panel_Identity.Visible = false;
        Panel_Search.Visible = false;
        Panel_TotalListArea.Visible = false;
        Panel_BBDateBookingList.Visible = true;
        Panel_GeneralCourseView.Visible = false;

        Button Btn_BBDate = sender as Button;
        string gBBDate_CA = Btn_BBDate.CommandArgument;
        string[] gBBDate_CN = Btn_BBDate.CommandName.Split('^');

        WordExcelButton3.Visible = false;

        LB_BBDatePlaceName.Text = gBBDate_CN[0];
         gPlaceName = gBBDate_CN[0];
        string gHOCPlace = " AND ( g.HPlaceName = '" + gBBDate_CN[0] + "')";


        ShowITable(gBBDate_CN[1], gBBDate_CN[2], gHOCPlace);

        //GE20230908_先清空
        LB_HTeacherNameTLTitle.Text = null;

        SqlDataReader QueryHCourse = SQLdatabase.ExecuteReader("select a.HCourseName, a.HTeacherName, a.HDateRange from HCourse as a where a.HID='" + gBBDate_CA + "'");

        while (QueryHCourse.Read())
        {
            LB_HCourseNameTLTitle.Text = QueryHCourse["HCourseName"].ToString();


            string[] gHTeacherName = QueryHCourse["HTeacherName"].ToString().Split(',');
            for (int i = 0; i < gHTeacherName.Length - 1; i++)
            {
                //Response.Write("SELECT HUserName FROM HMember where HID = '" + gHTeacherName[i].ToString() + "'");
                SqlDataReader QueryHTeacher = SQLdatabase.ExecuteReader("SELECT B.HUserName FROM HTeacher AS A JOIN HMember AS B ON A.HMemberID =B.HID where A.HID = '" + gHTeacherName[i].ToString() + "'");
                if (QueryHTeacher.Read())
                {

                    LB_HTeacherNameTLTitle.Text = QueryHTeacher["HUserName"].ToString() + "," + LB_HTeacherNameTLTitle.Text;
                }
                QueryHTeacher.Close();
            }

            LB_HTeacherNameTLTitle.Text = LB_HTeacherNameTLTitle.Text.Length > 0 ? "講師：" + LB_HTeacherNameTLTitle.Text.Substring(0, LB_HTeacherNameTLTitle.Text.Length - 1) : "講師：";

            LB_HDateRangeTLTitle.Text = "課程日期：" + QueryHCourse["HDateRange"].ToString();


        }

        QueryHCourse.Close();


        //GE20240409_修改OrderList_Merge改成直接寫
        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT COUNT(a.HUserName) AS  HApplyNum FROM MemberList as a join (SELECT  A.HMemberID, A.HCourseID FROM HCourseBooking AS A INNER JOIN HCourse AS B On A.HCourseID = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND (A.HChangeStatus IS NULL OR A.HChangeStatus = '2') GROUP BY A.HCourseID, A.HMemberID) as b on a.HID = b.HMemberID where b.HCourseID = '" + gBBDate_CA + "'");


        if (dr.Read())
        {
            LB_BBDateTotalNum.Text = dr["HApplyNum"].ToString();
        }
        dr.Close();


    }
    #endregion

    #region 產生開放單日報名的報表
    //GA20250625_開放單日報名的課程報名顯示
    protected void LBtn_BBDate_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_BBDate = sender as LinkButton;
        string[] gBBDate_CA = LBtn_BBDate.CommandArgument.Split('^');

        Response.Write("<script>window.location.href='HBBDateEnrollment.aspx?HID=" + gBBDate_CA[0] + "&N="+ HttpUtility.HtmlEncode(gBBDate_CA[2]) + "&D="+ HttpUtility.HtmlEncode(gBBDate_CA[3]) + "&P="+ HttpUtility.HtmlEncode(gBBDate_CA[1]) + "';</script>");
    }
    #endregion

    #region 動態table(七天班會)
    private void ShowITable(string gHCourseName, string gHDateRange, string gHOCPlace)
    {
        //*****************************使用動態 table開始*******************************//

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS3", ("$('.date').datepicker({format: 'dd/mm/yyyy', todayHighlight: true,orientation: 'bottom auto',autoclose: true,startDate:new Date(),});"), true);     //執行日曆js


        int gCT = 0;
        int gACT = 0;
        int gACTAll = 0;
        int gLCTAll = 0;

        int gAllCT = 0;//總筆數

        string gSDate = "";
        string gEDate = "";

        //判斷天數相差幾天
        int gDateDiff = 0;


        string[] gHDateRangeA = { };
        string gHDateRangeList = "";


        //GE20231002_將HCourseBooking改成OrderList_Merge
        //TE20231110-條件式加入參班身分為參班(HAttend='1')
        //GE20240924-條件式加入參班身分為參班兼護持(HAttend='6')
        //GE20250620_改成七天班會，先不分參班身分，只抓報名的人
        SqlDataReader QueryHMCBCAct = SQLdatabase.ExecuteReader("select Count(a.HID) as CT, b.HDateRange from HMember as a join HCourseBooking as b on a.HID = b.HMemberID left join HArea as d on a.HAreaID = d.HID  JOIN Course_BackendList AS g ON b.HCourseID = g.HID where b.HCourseName='" + gHCourseName + "' and b.HDateRange='" + gHDateRange + "'" + gHOCPlace + " and b.HStatus = 1 and b.HItemStatus = 1 group by b.HDateRange");


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

                Response.Write("<script>alert('日期錯誤!');</script>");
                return;
            }

        }
        QueryHMCBCAct.Close();

        string[] gAll = new string[gDateDiff * Convert.ToInt32(gAllCT) * 3];



        ////GE20231002-將HCourseBooking改成OrderList_Merge
        ////TE20231110-條件式加入參班身分為參班(HAttend='1')；加入體系
        ////GE20240924_條件式加入參班身分為參班兼護持(HAttend='6')
        //GE20250620_修改為七天班會的報名資料
        //GE20250624_加入其他所需的欄位
        SqlDataReader QueryHMCBCA = SQLdatabase.ExecuteReader("SELECT M.HSortL, M.HArea, M.HSystemName, M.HSort, M.HTeamID, M.HPeriod, M.HUserName, M.HCarrier, M.HRainbow, M.HLightEnvoy, M.HAccount, M.HPhone, M.HCourseID,M.HCourseName, M.HDateRange, M.HSex, STRING_AGG(FORMAT(M.HDate,'yyyy/MM/dd'), ',') AS HDate, STRING_AGG(M.HAttend, ',') AS HAttend, M.HLDate, M.HPlaceName, M.HMemberGroup, M.HRemark, M.HRoomName, M.HRoomTime, M.HStatus, M.HItemStatus,HAreaID, STRING_AGG(M.HTask, '') AS HTask FROM(SELECT h.HSort AS HSortL, a.HAreaID, a.HArea, a.HSystemName, d.HSort, a.HTeamID, (N'' + a.HPeriod + '') AS HPeriod,  a.HUserName, a.HAccount, a.HPhone, a.HCarrier, a.HRainbow, a.HLightEnvoy, b.HCourseID, b.HDateRange, a.HSex,i.HDate,i.HAttend,b.HCourseName,b.HLDate, g.HPlaceName, b.HMemberGroup, b.HRemark, j.HRoomName, b.HRoomTime,b.HStatus, b.HItemStatus, STRING_AGG(f.HTask, '') AS HTask FROM MemberList AS a JOIN HCourseBooking AS b ON a.HID = b.HMemberID INNER JOIN HCourseBooking_DateAttend AS i ON b.HID = i.HCourseBookingID LEFT JOIN HArea AS d ON a.HAreaID = d.HID LEFT JOIN HCourseBooking_Group AS f ON b.HID = f.HBookingID LEFT JOIN HRoom AS j ON b.HRoom = j.HID LEFT JOIN HLArea AS h ON d.HLAreaID = h.HID JOIN Course_BackendList AS c ON b.HCourseID = c.HID LEFT JOIN HPlaceList AS g ON c.HOCPLace = g.HID GROUP BY h.Hsort, a.HAreaID, a.HArea,a.HSystemName, d.HSort, a.HTeamID, a.HPeriod, a.HID, a.HUserName,a.HAccount, a.HPhone,a.HCarrier, a.HRainbow, a.HLightEnvoy, b.HCourseID, b.HCourseName, b.HDateRange, a.HSex, i.HAttend, i.HDate, b.HLDate, g.HPlaceName,b.HMemberGroup, b.HRemark, j.HRoomName, b.HRoomTime, b.HStatus, b.HItemStatus ) M WHERE HCourseName ='" + gHCourseName + "'  AND HDateRange ='" + gHDateRange + "' AND M.HPlaceName='" + gPlaceName + "' AND M.HStatus = '1' AND M.HItemStatus = '1' GROUP BY   M.HSortL, M.HArea, M.HSystemName, M.HSort, M.HTeamID,  M.HPeriod, M.HUserName, M.HCarrier, M.HRainbow,  M.HLightEnvoy, M.HAccount, M.HPhone, M.HCourseID,  M.HCourseName, M.HDateRange, M.HSex, M.HLDate,  M.HPlaceName, M.HMemberGroup, M.HRemark, M.HRoomName,  M.HRoomTime, M.HStatus, M.HItemStatus, M.HAreaID ORDER BY HAreaID ASC, M.HTeamID DESC, M.HSystemName DESC");





        //********表格標題開始********
       TableRow HRow1 = new TableRow();
        //TableCell HCell1 = new TableCell();
        TableCell HCell2 = new TableCell();
        TableCell HCell3 = new TableCell();
        TableCell HCell4 = new TableCell();
        TableCell HCell5 = new TableCell();
        TableCell HCell6 = new TableCell();
        TableCell HCell7 = new TableCell();
        TableCell HCell8 = new TableCell();
        TableCell HCell9 = new TableCell();
        TableCell HCell10 = new TableCell();
        TableCell HCell11 = new TableCell();
        TableCell HCell12 = new TableCell();
        TableCell HCell13 = new TableCell();
        TableCell HCell14 = new TableCell();
        TableCell HCell15 = new TableCell();

        HCell2.Text = "區屬";
        HCell3.Text = "光團";
        HCell4.Text = "期別";
        HCell5.Text = "姓名";
        HCell6.Text = "體系";
        HCell7.Text = "三載體光";
        HCell8.Text = "七彩光";
        HCell9.Text = "21道光";
        HCell10.Text = "光使";
        HCell11.Text = "信箱";
        HCell12.Text = "連絡電話";
        HCell13.Text = "性別";
        HCell15.Text = "備註";


        HCell2.BorderWidth = 1;
        HCell3.BorderWidth = 1;
        HCell4.BorderWidth = 1;
        HCell5.BorderWidth = 1;
        HCell6.BorderWidth = 1;
        HCell7.BorderWidth = 1;
        HCell8.BorderWidth = 1;
        HCell9.BorderWidth = 1;
        HCell10.BorderWidth = 1;
        HCell11.BorderWidth = 1;
        HCell12.BorderWidth = 1;
        HCell13.BorderWidth = 1;
        HCell15.BorderWidth = 1;


        HCell2.Width = Unit.Percentage(5);
        HCell3.Width = Unit.Percentage(7);
        HCell4.Width = Unit.Percentage(4);
        HCell5.Width = Unit.Percentage(5);
        HCell6.Width = Unit.Percentage(4);
        HCell7.Width = Unit.Percentage(4);
        HCell8.Width = Unit.Percentage(4);
        HCell9.Width = Unit.Percentage(4);
        HCell10.Width = Unit.Percentage(4);
        HCell11.Width = Unit.Percentage(4);
        HCell12.Width = Unit.Percentage(4);
        HCell13.Width = Unit.Percentage(4);
        HCell15.Width = Unit.Percentage(4);

        HCell2.HorizontalAlign = HorizontalAlign.Center;
        HCell3.HorizontalAlign = HorizontalAlign.Center;
        HCell4.HorizontalAlign = HorizontalAlign.Center;
        HCell5.HorizontalAlign = HorizontalAlign.Center;
        HCell6.HorizontalAlign = HorizontalAlign.Center;
        HCell7.HorizontalAlign = HorizontalAlign.Center;
        HCell8.HorizontalAlign = HorizontalAlign.Center;
        HCell9.HorizontalAlign = HorizontalAlign.Center;
        HCell10.HorizontalAlign = HorizontalAlign.Center;
        HCell11.HorizontalAlign = HorizontalAlign.Center;
        HCell12.HorizontalAlign = HorizontalAlign.Center;
        HCell13.HorizontalAlign = HorizontalAlign.Center;
        HCell15.HorizontalAlign = HorizontalAlign.Center;

        HCell2.Font.Bold = true;
        HCell3.Font.Bold = true;
        HCell4.Font.Bold = true;
        HCell5.Font.Bold = true;
        HCell6.Font.Bold = true;
        HCell7.Font.Bold = true;
        HCell8.Font.Bold = true;
        HCell9.Font.Bold = true;
        HCell10.Font.Bold = true;
        HCell11.Font.Bold = true;
        HCell12.Font.Bold = true;
        HCell13.Font.Bold = true;
        HCell15.Font.Bold = true;



        int gAllDateCT = 0;

        if (gCT == 0)
        {
            HRow1.Cells.Add(HCell2);
            HRow1.Cells.Add(HCell3);
            HRow1.Cells.Add(HCell4);
            HRow1.Cells.Add(HCell5);
            HRow1.Cells.Add(HCell6);
            HRow1.Cells.Add(HCell7);
            HRow1.Cells.Add(HCell8);
            HRow1.Cells.Add(HCell9);
            HRow1.Cells.Add(HCell10);
            HRow1.Cells.Add(HCell11);
            HRow1.Cells.Add(HCell12);
            HRow1.Cells.Add(HCell13);
            HRow1.Cells.Add(HCell15);

            TableCell[] HCell = new TableCell[gDateDiff];
            for (int i = 0; i < gHDateRangeA.Length; i++)
            {
                HCell[i] = new TableCell();

                HCell[i].Text = Convert.ToDateTime(gHDateRangeA[i]).ToString("MM/dd");
                HCell[i].BorderWidth = 1;
                HCell[i].HorizontalAlign = HorizontalAlign.Center;
                HCell[i].Font.Bold = true;

                HRow1.Cells.Add(HCell[i]);


            }
            TBL_HBBDateBookingList.Rows.Add(HRow1);

        }
        //********表格標題結束********










        string gHDateList = "";
        string gHAttendList = "";

        string gHPlaceNameList = "";
        string gHAreaList = "";
        string gHTeamIDList = "";
        string gHPeriodList = "";
        string gHUserNameList = "";
        string gHSystemNameList = "";
        string gHCourseIDList = "";
        string gHCarrierList = "";
        string gHRainbowList = "";
        string gTOLightList = "";//21道光
        string gHLightEnvoyList = "";
        string gHAccountList = "";
        string gHPhoneList = "";
        string gHSexList = "";
        string gHTaskList = "";
        string gHRemarkList = "";


        string[] gHPlaceNameListArray = { };
        string[] gHAreaListArray = { };
        string[] gHTeamIDListArray = { };
        string[] gHPeriodListArray = { };
        string[] gHUserNameListArray = { };
        string[] gHSystemNameListArray = { };
        string[] gHCourseIDListArray = { };
        string[] gHMemberIDListArray = { };
        string[] gHCarrierListArray = { };
        string[] gHRainbowListArray = { };
        string[] gTOLightListArray = { };//21道光
        string[] gHLightEnvoyListArray = { };
        string[] gHAccountListArray = { };
        string[] gHPhoneListArray = { };
        string[] gHSexListArray = { };
        string[] gHTaskListArray = { };
        string[] gHRemarkListArray = { };

        string[] gHDateListArray = { };
        string[] gHAttendListArray = { };


        while (QueryHMCBCA.Read())
        {
            gHPlaceNameList += "'" + QueryHMCBCA["HPlaceName"].ToString() + "'" + "|";
            gHAreaList += "'" + QueryHMCBCA["HArea"].ToString() + "'" + "|";
            gHTeamIDList += "'" + QueryHMCBCA["HTeamID"].ToString() + "'" + "|";
            gHPeriodList += "'" + QueryHMCBCA["HPeriod"].ToString() + "'" + "|";
            gHUserNameList += "'" + QueryHMCBCA["HUserName"].ToString() + "'" + "|";
            gHSystemNameList += "'" + QueryHMCBCA["HSystemName"].ToString() + "'" + "|";
            gHCourseIDList += "'" + QueryHMCBCA["HCourseID"].ToString() + "'" + ",";
            gHCarrierList += "'" + QueryHMCBCA["HCarrier"].ToString() +"'" + ",";
            gHRainbowList += "'" + QueryHMCBCA["HRainbow"].ToString() + "'" + ",";
            gTOLightList += "'" + "',";//21道光

            gHLightEnvoyList += "'" + QueryHMCBCA["HLightEnvoy"].ToString()+"'" + ",";
            gHAccountList += "'" + QueryHMCBCA["HAccount"].ToString() + "'" + ",";
            gHPhoneList += "'" + QueryHMCBCA["HPhone"].ToString() + "'" + ",";
            gHSexList += "'" + QueryHMCBCA["HSex"].ToString() + "'" + ",";
            //GE20250625暫時先註解
            //gHTaskList += "'" + QueryHMCBCA["HTask"].ToString() + "'" + ",";
            gHTaskList += "'" + "',";
            gHRemarkList += "'" + QueryHMCBCA["HRemark"].ToString() + "'" + ",";

            gHDateList += "'" + QueryHMCBCA["HDate"].ToString() + "'" + "|";
            gHAttendList += "'" + QueryHMCBCA["HAttend"].ToString() + "'" + "|";

        }
        QueryHMCBCA.Close();


        gHPlaceNameListArray = gHPlaceNameList.Trim('|').Split('|');
        gHAreaListArray = gHAreaList.Trim('|').Split('|');
        gHTeamIDListArray = gHTeamIDList.Trim('|').Split('|');
        gHPeriodListArray = gHPeriodList.Trim('|').Split('|');
        gHUserNameListArray = gHUserNameList.Trim('|').Split('|');
        gHSystemNameListArray = gHSystemNameList.Trim('|').Split('|');
        gHCourseIDListArray = gHCourseIDList.Trim(',').Split(',');
        gHCarrierListArray = gHCarrierList.Trim(',').Split(',');
        gHRainbowListArray = gHRainbowList.Trim(',').Split(',');
        gTOLightListArray = gTOLightList.Trim(',').Split(',');//21道光
        gHLightEnvoyListArray = gHLightEnvoyList.Trim(',').Split(',');
        gHAccountListArray = gHAccountList.Trim(',').Split(',');
        gHPhoneListArray = gHPhoneList.Trim(',').Split(',');
        gHSexListArray = gHSexList.Trim(',').Split(',');
        gHTaskListArray = gHTaskList.Trim(',').Split(',');
        gHRemarkListArray = gHRemarkList.Trim(',').Split(',');



 

        gHDateListArray = gHDateList.Trim('|').Split('|');
        gHAttendListArray = gHAttendList.Trim('|').Split('|');




        #region 移除多餘陣列
        List<string> resultList = new List<string>();
        foreach (string Data in gHCourseIDListArray)
        {
            if (!resultList.Contains(Data))
            {
                resultList.Add(Data);
            }
        }
        string[] gHCourseIDArrayNew = resultList.ToArray();
        string gHCourseIDListNew = String.Join(",", gHCourseIDArrayNew);//還原為字串

        #endregion




        for (int k = 0; k < gHUserNameListArray.Length; k++)
        {
            TableRow BRow = new TableRow();

            //********表格前段內容開始********

            gCT++;

            TableCell BCell2 = new TableCell();//區屬
            TableCell BCell3 = new TableCell();//光團
            TableCell BCell4 = new TableCell();//期別
            TableCell BCell5 = new TableCell();//姓名
            TableCell BCell6 = new TableCell();//體系
            TableCell BCell7 = new TableCell();//三載體光
            TableCell BCell8 = new TableCell();//七彩光
            TableCell BCell9 = new TableCell();//21道光
            TableCell BCell10 = new TableCell();//光使
            TableCell BCell11 = new TableCell();//信箱
            TableCell BCell12 = new TableCell();//連絡電話
            TableCell BCell13 = new TableCell();//性別
            TableCell BCell15 = new TableCell();//備註


            Label LB_QName2 = new Label();
            Label LB_QName3 = new Label();
            Label LB_QName4 = new Label();
            Label LB_QName5 = new Label();
            Label LB_QName6 = new Label();
            Label LB_QName7 = new Label();
            Label LB_QName8 = new Label();
            Label LB_QName9 = new Label();
            Label LB_QName10 = new Label();
            Label LB_QName11 = new Label();
            Label LB_QName12 = new Label();
            Label LB_QName13 = new Label();
            Label LB_QName15 = new Label();


            LB_QName2.ID = "HArea" + gCT.ToString();
            LB_QName3.ID = "HTeamID" + gCT.ToString();
            LB_QName4.ID = "HPeriod" + gCT.ToString();
            LB_QName5.ID = "HUserName" + gCT.ToString();
            LB_QName6.ID = "HSystemName" + gCT.ToString();
            LB_QName7.ID = "HCarrier" + gCT.ToString();
            LB_QName8.ID = "HRainBow" + gCT.ToString();
            LB_QName9.ID = "TOLightList" + gCT.ToString();//21道光
            LB_QName10.ID = "HLightEnvoy" + gCT.ToString();
            LB_QName11.ID = "HAccount" + gCT.ToString();
            LB_QName12.ID = "HPhone" + gCT.ToString();
            LB_QName13.ID = "HSex" + gCT.ToString();
            LB_QName15.ID = "HRemark" + gCT.ToString();




            LB_QName2.Text = gHAreaListArray[k].ToString().Trim('\'');

            string gHTeamType = "";
            string gHTeamName = "";
            string[] gHTeamID = gHTeamIDListArray[k].ToString().Split(',');


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


            LB_QName4.Text = gHPeriodListArray[k].ToString().Trim('\'');
            LB_QName5.Text = gHUserNameListArray[k].ToString().Trim('\'');
            LB_QName6.Text = gHSystemNameListArray[k].ToString().Trim('\'');


            LB_QName7.Text = gHCarrierListArray[k].ToString().Trim('\'') == "1" ? "金光" : gHCarrierListArray[k].ToString().Trim('\'') == "2" ? "銀光" : gHCarrierListArray[k].ToString().Trim('\'') == "3" ? "純光" : "未定";


            LB_QName8.Text = gHRainbowListArray[k].ToString().Trim('\'') == "1" ? "紅光" : gHRainbowListArray[k].ToString().Trim('\'') == "2" ? "橙光" : gHRainbowListArray[k].ToString().Trim('\'') == "3" ? "黃光" : gHRainbowListArray[k].ToString().Trim('\'') == "4" ? "綠光" : gHRainbowListArray[k].ToString().Trim('\'') == "5" ? "藍光" : gHRainbowListArray[k].ToString().Trim('\'') == "6" ? "靛光" : gHRainbowListArray[k].ToString().Trim('\'') == "7" ? "紫光" : "未定";




            string gHLightName = "";
            if (gHCarrierListArray[k].ToString().Trim('\'') != "0" && gHRainbowListArray[k].ToString().Trim('\'') != "0")
            {
                string BasicPath = AppDomain.CurrentDomain.BaseDirectory;

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "light_data.json");
                LightDataCache.LoadData(path);

                var allData = LightDataCache.GetAll();
                var first = LightDataCache.GetById(1);

                var result = LightDataCache.GetAll().Find(light => light.HCarrier == Convert.ToInt32(gHCarrierListArray[k].ToString().Trim('\'')) && light.HRainbow == Convert.ToInt32(gHRainbowListArray[k].ToString().Trim('\'')));

                gHLightName = result.HID + result.HLightName;
            }
            else
            {
                gHLightName = "未定光系";
            }
            LB_QName9.Text = gHLightName;


            LB_QName10.Text = gHLightEnvoyListArray[k].ToString().Trim('\'') == "1" ? "V" : "";


            LB_QName11.Text = gHAccountListArray[k].ToString().Trim('\'');
            LB_QName12.Text = gHPhoneListArray[k].ToString().Trim('\'');
            LB_QName13.Text = gHSexListArray[k].ToString().Trim('\'') == "1" ? "男" : "女";
            LB_QName15.Text = gHRemarkListArray[k].ToString().Trim('\'');

            BCell2.BorderWidth = 1;
            BCell3.BorderWidth = 1;
            BCell4.BorderWidth = 1;
            BCell5.BorderWidth = 1;
            BCell6.BorderWidth = 1;
            BCell7.BorderWidth = 1;
            BCell8.BorderWidth = 1;
            BCell9.BorderWidth = 1;
            BCell10.BorderWidth = 1;
            BCell11.BorderWidth = 1;
            BCell12.BorderWidth = 1;
            BCell13.BorderWidth = 1;
            BCell15.BorderWidth = 1;

            BCell2.HorizontalAlign = HorizontalAlign.Center;
            BCell4.HorizontalAlign = HorizontalAlign.Center;
            BCell5.HorizontalAlign = HorizontalAlign.Center;
            BCell6.HorizontalAlign = HorizontalAlign.Center;
            BCell7.HorizontalAlign = HorizontalAlign.Center;
            BCell8.HorizontalAlign = HorizontalAlign.Center;
            BCell9.HorizontalAlign = HorizontalAlign.Left;
            BCell10.HorizontalAlign = HorizontalAlign.Center;
            BCell11.HorizontalAlign = HorizontalAlign.Left;
            BCell12.HorizontalAlign = HorizontalAlign.Center;
            BCell13.HorizontalAlign = HorizontalAlign.Center;
            BCell15.HorizontalAlign = HorizontalAlign.Center;

            BCell2.Controls.Add(LB_QName2);
            BCell3.Controls.Add(LB_QName3);
            BCell4.Controls.Add(LB_QName4);
            BCell5.Controls.Add(LB_QName5);
            BCell6.Controls.Add(LB_QName6);
            BCell7.Controls.Add(LB_QName7);
            BCell8.Controls.Add(LB_QName8);
            BCell9.Controls.Add(LB_QName9);
            BCell10.Controls.Add(LB_QName10);
            BCell11.Controls.Add(LB_QName11);
            BCell12.Controls.Add(LB_QName12);
            BCell13.Controls.Add(LB_QName13);
            BCell15.Controls.Add(LB_QName15);

            BRow.Cells.Add(BCell2);
            BRow.Cells.Add(BCell3);
            BRow.Cells.Add(BCell4);
            BRow.Cells.Add(BCell5);
            BRow.Cells.Add(BCell6);
            BRow.Cells.Add(BCell7);
            BRow.Cells.Add(BCell8);
            BRow.Cells.Add(BCell9);
            BRow.Cells.Add(BCell10);
            BRow.Cells.Add(BCell11);
            BRow.Cells.Add(BCell12);
            BRow.Cells.Add(BCell13);
            BRow.Cells.Add(BCell15);

            //********表格前段內容結束********





            //********表格後段內容開始********

            TableCell BCell16 = new TableCell();//日期序列


            #region 比對上課日期

            string[] gHDateRangeArray = gHDateRangeList.Trim(',').Split(',');

            string[] gHDateArray = { };
            string[] gHAttendArray = { };



            for (int x = 0; x < gHDateRangeArray.Length; x++)//列表日期(完整月份)
            {
                BCell16 = new TableCell();
                BCell16.BorderWidth = 1;
                BCell16.HorizontalAlign = HorizontalAlign.Center;
                BRow.Cells.Add(BCell16);


                if (gHDateListArray[k].ToString().Trim() != "")
                {
                    gHDateArray = gHDateListArray[k].Trim('\'').Trim(',').Split(',');
                    gHAttendArray = gHAttendListArray[k].Trim('\'').Trim(',').Split(',');

                    if (gHDateArray[0].ToString().Trim() != "")
                    {

                        for (int z = 0; z < gHDateArray.Length; z++)
                        {

                            gHDateArray = gHDateListArray[k].Trim('\'').Trim(',').Split(',');
                            gHAttendArray = gHAttendListArray[k].Trim('\'').Trim(',').Split(',');


                            if (gHDateRangeArray[x].ToString().Trim('\'') == gHDateArray[z].ToString().Trim('\''))
                            {

                                BCell16.Text = gHAttendArray[z].Trim('\'') == "1" ? "1" : gHAttendArray[z].Trim('\'') == "5" ? "2" : gHAttendArray[z].Trim('\'') == "6" ? "3" : "待確認";
                                gACT++;
                            }
                            else
                            {
                                gACT++;
                            }

                        }
                    }


                }
                else
                {
                    BCell16 = new TableCell();
                    BCell16.BorderWidth = 1;
                    BCell16.HorizontalAlign = HorizontalAlign.Center;
                    BRow.Cells.Add(BCell16);
                    BCell16.Text = "";
                }
            }




            //********表格後段內容結束********
            #endregion



            TBL_HBBDateBookingList.Rows.Add(BRow);






        }









        //****************************************使用動態 table結束 * *******************************************
    }

    #endregion




    
}