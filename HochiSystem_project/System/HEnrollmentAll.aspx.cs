using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class System_HEnrollmentAll : System.Web.UI.Page
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
            ViewState["Search"] = "";
            ViewState["TotalList"] = "";
            ViewState["TotalListArea"] = "";
        }

        if (!IsPostBack)
        {
            SDS_HC.SelectCommand = "SELECT a.HCourseName, a.HDateRange, a.HBookByDateYN FROM HCourse as a join HPlace as b on a.HOCPlace=b.HID where a.HStatus='1' group by HCourseName, HDateRange, a.HBookByDateYN ORDER BY a.HDateRange DESC";
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


            SDS_TotalList.SelectCommand = ViewState["TotalList"].ToString();
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Rpt_TotalList.DataBind();
            //Pg_TotalList.FrontPagingLoad("HochiSystemConnection", ViewState["Search"].ToString(), PageMax, LastPage, false, Rpt_TotalList);


            SDS_TotalListArea.SelectCommand = ViewState["TotalListArea"].ToString();
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Rpt_TotalListArea.DataBind();
            //Pg_TotalListArea.FrontPagingLoad("HochiSystemConnection", ViewState["Search"].ToString(), PageMax, LastPage, false, Rpt_TotalListArea);


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

        // 加入未定光系統計列（如有）
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
        //string gHOCPlace = DDL_HOCPlace.SelectedValue=="0"?"like '%'":"='"+ DDL_HOCPlace.SelectedValue + "'";//地點沒輸入則全找
        //string gHOCPlace = DDL_HOCPlace.SelectedValue == "0" ? "='0'" : "='" + DDL_HOCPlace.SelectedValue + "'";//地點沒輸入則全不找

        SDS_HC.SelectCommand = "SELECT a.HCourseName, a.HDateRange, a.HBookByDateYN FROM HCourse as a join HPlace as b on a.HOCPlace=b.HID WHERE HCourseName like N'%" + TB_Search.Text + "%'  and ((left(HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)<='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and left(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "'and right(HDateRange,10)>='" + gHDateRangeArray[1].Trim() + "') or (left(HDateRange,10)>='" + gHDateRangeArray[0].Trim() + "' and right(HDateRange,10)<='" + gHDateRangeArray[1].Trim() + "')) and a.HStatus='1'  group by HCourseName, HDateRange , a.HBookByDateYN ORDER BY a.HDateRange DESC";
        //SDS_HC.SelectCommand = "SELECT * FROM Course_BackendList WHERE HCourseName like'%" + TB_Search.Text + "%' and HStatus='1' ";

        #region 分頁copy-3搜尋用
        ViewState["Search"] = SDS_HC.SelectCommand;
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

        SDS_HC.SelectCommand = "SELECT a.HCourseName, a.HDateRange, a.HBookByDateYN FROM HCourse as a join HPlace as b on a.HOCPlace=b.HID where a.HStatus='1'  group by HCourseName, HDateRange, a.HBookByDateYN ORDER BY a.HDateRange DESC";
        //SDS_HC.SelectCommand = "SELECT * FROM Course_BackendList where HStatus='1'";
        ViewState["Search"] = SDS_HC.SelectCommand;

        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HC.SelectCommand, PageMax, LastPage, true, Rpt_HC);
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
        string TotalListArea_CN = LBtn_TotalListArea.CommandName;

        //string[] gTotalListArea_CA = TotalListArea_CA.Split(',');

        LB_HCourseNameTLATitle.Text = TotalListArea_CA;
        LB_HDateRangeTLATitle.Text = TotalListArea_CN;
        //LB_HDateRangeTLATitle.Text = "課程日期：" + TotalListArea_CN;

        //230731-報名完成條件式改為HCourseBooking.HStatus=1 AND HItemStatus=1
        //AE20230908_拿掉d.CourseID，避免group by出現重複資料
        // SDS_TotalListArea.SelectCommand = "select a.HAreaID, d.HArea from HMember as a join HCourseBooking as b on a.HID = b.HMemberID join HCourse as c on b.HCourseID = c.HID left join HArea as d on a.HAreaID=d.HID LEFT JOIN HLArea AS e ON d.HLAreaID = e.HID where c.HCourseName='" + TotalListArea_CA + "' and c.HDateRange='" + TotalListArea_CN + "' and b.HStatus='1' AND b.HItemStatus='1' group by  a.HAreaID, d.HArea, e.HSort, d.HSort ORDER BY e.HSort, d.HSort";

        //AE20230930_將HCourseBooking改成OrderList_Merge&不用再join HCourse
        SDS_TotalListArea.SelectCommand = "select a.HAreaID, d.HArea from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID left join HArea as d on a.HAreaID=d.HID LEFT JOIN HLArea AS e ON d.HLAreaID = e.HID where b.HCourseName=N'" + TotalListArea_CA + "' and b.HDateRange='" + TotalListArea_CN + "' and b.HStatus='1' AND b.HItemStatus='1' group by  a.HAreaID, d.HArea, e.HSort, d.HSort ORDER BY e.HSort, d.HSort";

        //Response.Write("<br/><br/><br/><br/><br/>select a.HAreaID, d.HArea from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID left join HArea as d on a.HAreaID=d.HID LEFT JOIN HLArea AS e ON d.HLAreaID = e.HID where b.HCourseName=N'" + TotalListArea_CA + "' and b.HDateRange='" + TotalListArea_CN + "' and b.HStatus='1' AND b.HItemStatus='1' group by  a.HAreaID, d.HArea, e.HSort, d.HSort ORDER BY e.HSort, d.HSort<br/>");
        //Response.End();

        //SDS_HC.SelectCommand = "SELECT * FROM Course_BackendList where HStatus='1'";
        Rpt_TotalListArea.DataBind();

        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
        //Pg_TotalListArea.FrontPagingLoad("HochiSystemConnection", SDS_TotalListArea.SelectCommand, PageMax, LastPage, true, Rpt_TotalListArea);

        ViewState["TotalListArea"] = SDS_TotalListArea.SelectCommand;





        //SqlDataReader QueryHCourse = SQLdatabase.ExecuteReader("select a.HCourseName, a.HDateRange from HCourse as a where a.HCourseName='" + gTotalListArea_CA[0].ToString() + "' and a.HDateRange='" + gTotalListArea_CA[1].ToString() + "'");

        //while (QueryHCourse.Read())
        //{
        //	LB_HCourseNameTLTitle.Text = QueryHCourse["HCourseName"].ToString();
        //	LB_HDateRangeTLTitle.Text = "課程日期：" + QueryHCourse["HDateRange"].ToString();
        //}

        //QueryHCourse.Close();


    }

    int ApplyNumTLA = 0;//報名人數

    protected void Rpt_TotalListArea_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;


        //230731-報名完成條件式改為HCourseBooking.HStatus=1 AND HItemStatus=1
        //EE20230817_工作項目顯示、語法調整
        //AE20230930_將HCourseBooking改成OrderList_Merge
        //((SqlDataSource)e.Item.FindControl("SDS_TotalListAreaChild")).SelectCommand = "SELECT M.HAreaID, M.HArea, M.HTeamID, M.HPeriod, M.HID, M.HUserName, M.HCourseID, M.HDateRange, M.HSex, M.HAttend, M.HLDate, M.HPlaceName,M.HMemberGroup, M.HRemark, M.HRoomName, M.HRoomTime, LEFT(M.HTask, len(M.HTask) - 1) AS HTask FROM (SELECT a.HAreaID, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, c.HDateRange, a.HSex, b.HAttend, b.HLDate, e.HPlaceName,b.HMemberGroup, b.HRemark, g.HRoomName, b.HRoomTime, b.HItemStatus, b.HStatus, c.HCourseName, (SELECT DISTINCT (cast(HTask AS NVARCHAR) + '、')  FROM HCourseBooking_Group CROSS APPLY SPLIT(HTask, '、')  where HCourseBooking_Group.HBookingID=b.HID  FOR XML PATH(''))  AS HTask FROM HMember as a join OrderList_Merge as b on a.HID = b.HMemberID join HCourse as c on b.HCourseID = c.HID left join HArea as d on a.HAreaID=d.HID left join HPlace as e on c.HOCPlace=e.HID left join HCourseBooking_Group as f on b.HID = f.HBookingID left join HRoom as g on b.HRoom =g.HID  GROUP BY a.HAreaID, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, c.HDateRange, a.HSex, b.HAttend, b.HLDate, e.HPlaceName, b.HRemark,b.HMemberGroup, b.HItemStatus, b.HStatus, g.HRoomName, b.HRoomTime, b.HID, c.HCourseName) M where M.HCourseName='" + LB_HCourseNameTLATitle.Text + "' AND M.HDateRange='" + LB_HDateRangeTLATitle.Text + "' and M.HAreaID='" + gDRV["HAreaID"].ToString() + "'and M.HStatus='1' and M.HItemStatus='1' GROUP BY M.HAreaID, M.HArea, M.HTeamID, M.HPeriod, M.HID, M.HUserName, M.HCourseID, M.HDateRange, M.HSex, M.HAttend, M.HLDate, M.HPlaceName,M.HMemberGroup, M.HRemark, M.HRoomName, M.HRoomTime, M.HTask";

        //AE20231122_加入體系資訊&不用再join HCourse&HPlace
        //EE20240318_加入排序：光團->體系
        //((SqlDataSource)e.Item.FindControl("SDS_TotalListAreaChild")).SelectCommand = "SELECT M.HSystemName, M.HAreaID, M.HArea, M.HTeamID, M.HPeriod, M.HID, M.HUserName, M.HCourseID, M.HDateRange, M.HSex, M.HAttend, M.HLDate, M.HPlaceName,M.HMemberGroup, M.HRemark, M.HRoomName, M.HRoomTime, LEFT(M.HTask, len(M.HTask) - 1) AS HTask FROM (SELECT a.HAreaID, a.HArea, a.HSystemName, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, a.HSex, b.HAttend, b.HLDate, b.HPlaceName, b.HMemberGroup, b.HRemark, g.HRoomName, b.HRoomTime, b.HItemStatus, b.HStatus, b.HCourseName, (SELECT DISTINCT(cast(HTask AS NVARCHAR) + '、')  FROM HCourseBooking_Group CROSS APPLY SPLIT(HTask, '、')  where HCourseBooking_Group.HBookingID = b.HID  FOR XML PATH(''))  AS HTask FROM MemberList as a join OrderList_Merge as b on a.HID = b.HMemberID left join HArea as d on a.HAreaID = d.HID left join HCourseBooking_Group as f on b.HID = f.HBookingID left join HRoom as g on b.HRoom = g.HID GROUP BY a.HAreaID,a.HArea, a.HSystemName, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, a.HSex, b.HAttend, b.HLDate, b.HPlaceName, b.HRemark,b.HMemberGroup, b.HItemStatus, b.HStatus, g.HRoomName, b.HRoomTime, b.HID, b.HCourseName) M where M.HCourseName = N'" + LB_HCourseNameTLATitle.Text + "' AND M.HDateRange = '" + LB_HDateRangeTLATitle.Text + "' and M.HAreaID = '" + gDRV["HAreaID"].ToString() + "'and M.HStatus = '1' and M.HItemStatus = '1' GROUP BY M.HAreaID, M.HArea, M.HSystemName,M.HTeamID, M.HPeriod, M.HID, M.HUserName, M.HCourseID, M.HDateRange, M.HSex, M.HAttend, M.HLDate, M.HPlaceName,M.HMemberGroup, M.HRemark, M.HRoomName, M.HRoomTime, M.HTask ORDER BY M.HTeamID DESC,  M.HSystemName DESC";

        //JE20250106_優化SQL指令
        //AE20250327_加入三載體光、七彩光、21道光、是否為光使
        ((SqlDataSource)e.Item.FindControl("SDS_TotalListAreaChild")).SelectCommand = "SELECT M.HSystemName, M.HAreaID, M.HArea, M.HTeamID, M.HPeriod, M.HID, M.HUserName,M.HCarrier, M.HRainbow, M.HLightEnvoy,  M.HAccount, M.HPhone, M.HCourseID, M.HDateRange, M.HSex, M.HAttend, M.HLDate, M.HPlaceName,M.HMemberGroup, M.HRemark, M.HRoomName, M.HRoomTime, LEFT(M.HTask, len(M.HTask) - 1) AS HTask FROM (SELECT a.HAreaID, a.HArea, a.HSystemName, a.HTeamID, a.HPeriod, a.HID, a.HUserName, a.HAccount, a.HPhone, a.HCarrier, a.HRainbow, a.HLightEnvoy, b.HCourseID, b.HDateRange, a.HSex, b.HAttend, b.HLDate, b.HPlaceName, b.HMemberGroup, b.HRemark, g.HRoomName, b.HRoomTime, b.HItemStatus, b.HStatus, b.HCourseName, (SELECT DISTINCT(cast(HTask AS NVARCHAR) + '、')  FROM HCourseBooking_Group CROSS APPLY SPLIT(HTask, '、')  where HCourseBooking_Group.HBookingID = b.HID  FOR XML PATH(''))  AS HTask FROM MemberList as a join OrderList_Merge as b on a.HID = b.HMemberID left join HArea as d on a.HAreaID = d.HID left join HCourseBooking_Group as f on b.HID = f.HBookingID left join HRoom as g on b.HRoom = g.HID WHERE HCourseName = N'" + LB_HCourseNameTLATitle.Text + "' AND HDateRange = '" + LB_HDateRangeTLATitle.Text + "' AND HAreaID = '" + gDRV["HAreaID"].ToString() + "'AND b.HStatus = '1' AND HItemStatus = '1' GROUP BY a.HAreaID,a.HArea, a.HSystemName, a.HTeamID, a.HPeriod, a.HID, a.HUserName, a.HAccount, a.HPhone, a.HCarrier, a.HRainbow, a.HLightEnvoy, b.HCourseID, b.HDateRange, a.HSex, b.HAttend, b.HLDate, b.HPlaceName, b.HRemark,b.HMemberGroup, b.HItemStatus, b.HStatus, g.HRoomName, b.HRoomTime, b.HID, b.HCourseName) AS M WHERE M.HCourseName = N'" + LB_HCourseNameTLATitle.Text + "' AND M.HDateRange = '" + LB_HDateRangeTLATitle.Text + "' AND M.HAreaID = '" + gDRV["HAreaID"].ToString() + "'AND M.HStatus = '1' AND M.HItemStatus = '1' GROUP BY M.HAreaID, M.HArea, M.HSystemName,M.HTeamID, M.HPeriod, M.HID, M.HUserName, M.HCarrier, M.HRainbow, M.HLightEnvoy,  M.HAccount, M.HPhone, M.HCourseID, M.HDateRange, M.HSex, M.HAttend, M.HLDate, M.HPlaceName,M.HMemberGroup, M.HRemark, M.HRoomName, M.HRoomTime, M.HTask ORDER BY M.HTeamID DESC,  M.HSystemName DESC";

        //Response.Write("<br/><br/><br/><br/><br/>SELECT M.HSystemName, M.HAreaID, M.HArea, M.HTeamID, M.HPeriod, M.HID, M.HUserName, M.HCourseID, M.HDateRange, M.HSex, M.HAttend, M.HLDate, M.HPlaceName,M.HMemberGroup, M.HRemark, M.HRoomName, M.HRoomTime, LEFT(M.HTask, len(M.HTask) - 1) AS HTask FROM (SELECT a.HAreaID, a.HArea, a.HSystemName, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, a.HSex, b.HAttend, b.HLDate, b.HPlaceName, b.HMemberGroup, b.HRemark, g.HRoomName, b.HRoomTime, b.HItemStatus, b.HStatus, b.HCourseName, (SELECT DISTINCT(cast(HTask AS NVARCHAR) + '、')  FROM HCourseBooking_Group CROSS APPLY SPLIT(HTask, '、')  where HCourseBooking_Group.HBookingID = b.HID  FOR XML PATH(''))  AS HTask FROM MemberList as a join OrderList_Merge as b on a.HID = b.HMemberID left join HArea as d on a.HAreaID = d.HID left join HCourseBooking_Group as f on b.HID = f.HBookingID left join HRoom as g on b.HRoom = g.HID WHERE HCourseName = N'" + LB_HCourseNameTLATitle.Text + "' AND HDateRange = '" + LB_HDateRangeTLATitle.Text + "' AND HAreaID = '" + gDRV["HAreaID"].ToString() + "'AND b.HStatus = '1' AND HItemStatus = '1' GROUP BY a.HAreaID,a.HArea, a.HSystemName, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, a.HSex, b.HAttend, b.HLDate, b.HPlaceName, b.HRemark,b.HMemberGroup, b.HItemStatus, b.HStatus, g.HRoomName, b.HRoomTime, b.HID, b.HCourseName) AS M WHERE M.HCourseName = N'" + LB_HCourseNameTLATitle.Text + "' AND M.HDateRange = '" + LB_HDateRangeTLATitle.Text + "' AND M.HAreaID = '" + gDRV["HAreaID"].ToString() + "'AND M.HStatus = '1' AND M.HItemStatus = '1' GROUP BY M.HAreaID, M.HArea, M.HSystemName,M.HTeamID, M.HPeriod, M.HID, M.HUserName, M.HCourseID, M.HDateRange, M.HSex, M.HAttend, M.HLDate, M.HPlaceName,M.HMemberGroup, M.HRemark, M.HRoomName, M.HRoomTime, M.HTask ORDER BY M.HTeamID DESC,  M.HSystemName DESC<br/>");

        //Response.End();

        //AE20230905_加入人數統計
        ((SqlDataSource)e.Item.FindControl("SDS_TotalListAreaChild")).DataBind();
        ((Repeater)e.Item.FindControl("Rpt_TotalListAreaChild")).DataBind();
        ((Label)e.Item.FindControl("LB_Count")).Text = "(" + (((Repeater)e.Item.FindControl("Rpt_TotalListAreaChild")).Items.Count).ToString() + ")";

        //EA20240323_加入報名人數
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
            //LightDataCache.LoadData("../App_Data/light_data.json");

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

        //1 = 參班【一般】、2 = 參班【學青(25歲以下在學青年)】、3 = 參班【經濟困難(需經光團1號導師審核通過)】、4 = 參班【經濟困難(護持出家師父及全心奉獻各修行組織團體之清修人士(請光團1號導師協助確認)】、5 = 不參班(專業護持)、6 = 不參班(純護持)


        //((Label)e.Item.FindControl("LB_HAttend")).Text = gDRV["HAttend"].ToString() == "5" ? "專業護持者" : gDRV["HAttend"].ToString() == "" || gDRV["HAttend"].ToString() == null ? "護持者" : gDRV["HAttend"].ToString() == "0" ? "" : "參班";


        //AE20231017_更正1 = 參班【一般】、5 = 純護持(非班員)、6 = 參班兼護持
        ((Label)e.Item.FindControl("LB_HAttend")).Text = gDRV["HAttend"].ToString() == "5" ? "純護持(非班員)" : gDRV["HAttend"].ToString() == "6" ? "參班兼護持" : gDRV["HAttend"].ToString() == "" || gDRV["HAttend"].ToString() == null ? "未選擇" : gDRV["HAttend"].ToString() == "0" ? "" : "參班";










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
        string Identity_CN = LBtn_Identity.CommandName;

        //string[] gIdentity_CA = Identity_CA.Split(',');


        //230731-報名完成條件式改為HCourseBooking.HStatus=1 AND HItemStatus=1
        //SqlDataReader QueryHMCBCA = SQLdatabase.ExecuteReader("select a.HAreaID, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, c.HDateRange, a.HSex, isnull(b.HAttend,0) as HAttend, c.HOCPlace, b.HLDate, c.HRemark from HMember as a join HCourseBooking as b on a.HID = b.HMemberID join HCourse as c on b.HCourseID = c.HID left join HArea as d on a.HAreaID=d.HID where c.HCourseName='" + Identity_CA + "' and c.HDateRange='" + Identity_CN + "' and b.HStatus='1' AND b.HItemStatus='1' order by b.HCourseID");

        //AE20230930_將HCourseBooking改成OrderList_Merge&不用再join HCourse
        SqlDataReader QueryHMCBCA = SQLdatabase.ExecuteReader("select a.HAreaID, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, b.HDateRange, a.HSex, isnull(b.HAttend,0) as HAttend, b.HOCPlace, b.HLDate, b.HRemark from HMember as a join OrderList_Merge as b on a.HID = b.HMemberID  left join HArea as d on a.HAreaID=d.HID where b.HCourseName=N'" + Identity_CA + "' and b.HDateRange='" + Identity_CN + "' and b.HStatus='1' AND b.HItemStatus='1' order by b.HCourseID");


		//AE20240409_將OrderList_Merge改寫
		//SqlDataReader QueryHMCBCA = SQLdatabase.ExecuteReader("select a.HAreaID, d.HArea, a.HTeamID, (N''''+a.HPeriod) AS Period , a.HID, a.HUserName, b.HCourseID, b.HDateRange, a.HSex, isnull(b.HAttend,0) as HAttend,f.HOCPlace, b.HLDate, b.HRemark from HMember as a  JOIN (SELECT  A.HMemberID, A.HCourseID, A.HCourseName, A.HDateRange FROM HCourseBooking AS A INNER JOIN HCourse AS B On A.HCourseID = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND (A.HChangeStatus IS NULL OR A.HChangeStatus = '2') UNION SELECT A.HMemberID, D.HCourseIDNew AS HCourseID, D.HCourseName, D.HDateRange FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON A.HOrderGroup = D.HOrderGroupNew INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND A.HCourseName = '換課' AND(D.HChangeStatus IS NULL OR D.HChangeStatus = '2') UNION SELECT A.HMemberID, D.HCourseIDNew AS HCourseID, D.HCourseName, D.HDateRange FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON(A.HOrderGroup = D.HOrderGroupSrc AND A.HOrderNum = D.HOrderNumSrc) OR(A.HOrderGroup = D.HOrderGroupSrc AND D.HOrderNumSrc = '') INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND(D.HOrderGroupNew = '' OR D.HOrderGroupNew IS NULL) UNION SELECT  A.HMemberID, D.HCourseIDNew AS HCourseID, D.HCourseName, D.HDateRange FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON(A.HOrderGroup = D.HOrderGroupSrc) LEFT JOIN HCBChangeRecord AS E ON(D.HOrderGroupSrc = E.HOrderGroupNew) INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND A.HCourseName = '換課' AND(D.HOrderGroupNew = '' OR D.HOrderGroupNew IS NULL) AND(E.HChangeStatus = '1') AND(A.HChangeStatus IS NULL OR A.HChangeStatus = '2'))  as b on a.HID = b.HMemberID LEFT JOIN HCourse AS f ON b.HCourseID=f.HID LEFT JOIN HArea AS d ON a.HAreaID=d.HID WHERE b.HCourseName='" + Identity_CA + "' and b.HDateRange='" + Identity_CN + "'  ORDER BY b.HCourseID");

		while (QueryHMCBCA.Read())
        {
            //已停用
            //1 = 參班【一般】、2 = 參班【學青(25歲以下在學青年)】、3 = 參班【經濟困難(需經光團1號導師審核通過)】、4 = 參班【經濟困難(護持出家師父及全心奉獻各修行組織團體之清修人士(請光團1號導師協助確認)】、5 = 不參班(專業護持)、6 = 不參班(純護持)

            //230323-修改
            //EE20240318_加入HAttend=6(參班兼護持)
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


        //EE20230817_取到小數點第二位
        LB_AttendPCT.Text = Math.Round(((Convert.ToInt32(LB_Attend.Text) / gTotal) * 100), 2).ToString();
        LB_ProGuidePCT.Text = Math.Round(((Convert.ToInt32(LB_ProGuide.Text) / gTotal) * 100), 2).ToString();
        LB_GuidePCT.Text = Math.Round(((Convert.ToInt32(LB_Guide.Text) / gTotal) * 100), 2).ToString();
        //LB_AttendPCT.Text = Convert.ToString((Convert.ToInt32(LB_Attend.Text) / gTotal) * 100);
        //LB_ProGuidePCT.Text = Convert.ToString((Convert.ToInt32(LB_ProGuide.Text) / gTotal) * 100);
        //LB_GuidePCT.Text = Convert.ToString((Convert.ToInt32(LB_Guide.Text) / gTotal) * 100);




        LB_HCourseNameITitle.Text = Identity_CA;
        LB_HDateRangeITitle.Text = "課程日期：" + Identity_CN;




        //SqlDataReader QueryHCourse = SQLdatabase.ExecuteReader("select a.HCourseName, a.HDateRange from HCourse as a where a.HCourseName='" + gIdentity_CA[0].ToString() + "' and a.HDateRange='" + gIdentity_CA[1].ToString() + "'");

        //while (QueryHCourse.Read())
        //{
        //	LB_HCourseNameTLTitle.Text = QueryHCourse["HCourseName"].ToString();
        //	LB_HDateRangeTLTitle.Text = "課程日期：" + QueryHCourse["HDateRange"].ToString();
        //}

        //QueryHCourse.Close();




    }
    #endregion

    #region 依組別
    protected void LBtn_TotalList_Click(object sender, EventArgs e)
    {
        Panel_CourseList.Visible = false;
        Panel_TotalList.Visible = true;
        Panel_Search.Visible = false;


        LinkButton LBtn_TotalList = sender as LinkButton;
        string TotalList_CA = LBtn_TotalList.CommandArgument;
        string TotalList_CN = LBtn_TotalList.CommandName;


        //string[] gTotalList_CA = TotalList_CA.Split(',');

        //230731-報名完成條件式改為HCourseBooking.HStatus=1 AND HItemStatus=1
        //EE20230817_工作項目顯示、語法調整
        // SDS_TotalList.SelectCommand = "SELECT M.HAreaID, M.HArea, M.HTeamID, M.HPeriod, M.HID, M.HUserName, M.HCourseID, M.HDateRange, M.HSex, M.HAttend, M.HOCPlace, 
        //AE20230930_將HCourseBooking改成OrderList_Merge
        //AE20230930_將HCourseBooking改成OrderList_Merge
        //SDS_TotalList.SelectCommand = "SELECT M.HAreaID, M.HArea, M.HTeamID, M.HPeriod, M.HID, M.HUserName, M.HCourseID, M.HDateRange, M.HSex, M.HAttend, M.HOCPlace, M.HLDate, M.HPlaceName, M.HMemberGroup, M.HRemark, M.HRoomName, M.HRoomTime, M.HStatus, M.HItemStatus, LEFT(M.HTask, len(M.HTask) - 1) AS HTask FROM (SELECT a.HAreaID, b.HMemberGroup, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, c.HDateRange, a.HSex, b.HAttend, c.HOCPlace, b.HLDate, b.HRemark, e.HPlaceName, h.HRoomName, b.HRoomTime, b.HStatus, b.HItemStatus, c.HCourseName, (SELECT DISTINCT (cast(HTask AS NVARCHAR) + '、')  FROM HCourseBooking_Group CROSS APPLY SPLIT(HTask, '、')  where HCourseBooking_Group.HBookingID=b.HID  FOR XML PATH('')) AS HTask FROM HMember AS a JOIN OrderList_Merge AS b ON a.HID = b.HMemberID JOIN HCourse AS c ON b.HCourseID = c.HID LEFT JOIN HArea AS d ON a.HAreaID=d.HID LEFT JOIN HPlace AS e ON c.HOCPlace=e.HID LEFT JOIN HLArea AS f ON d.HLAreaID = f.HID left join HCourseBooking_Group as g on b.HID = g.HBookingID left join HRoom as h on b.HRoom =h.HID GROUP BY a.HAreaID, b.HMemberGroup, d.HArea, a.HTeamID, a.HPeriod, a.HID, a.HUserName, b.HCourseID, c.HDateRange, a.HSex, b.HAttend, c.HOCPlace, b.HLDate, b.HRemark, e.HPlaceName, h.HRoomName, b.HRoomTime, b.HStatus, b.HItemStatus,b.HID, c.HCourseName) M WHERE M.HCourseName='" + TotalList_CA + "' AND M.HDateRange='" + TotalList_CN + "' AND M.HStatus='1'  AND M.HItemStatus='1' ORDER BY HAreaID ASC";

        //AE20231122_加入體系資訊&不用再join HCourse&HPlace
        //EE20240318_加入排序：光團->體系
        //AE20250326_加入三載體光、七彩光、二十一道光、光使
        SDS_TotalList.SelectCommand = "SELECT M.HAreaID, M.HArea,M.HSystemName, M.HTeamID, M.HPeriod, M.HID, M.HUserName, M.HCarrier, M.HRainbow, M.HLightEnvoy, M.HAccount, M.HPhone, M.HCourseID, M.HDateRange, M.HSex, M.HAttend, M.HOCPlace, M.HLDate, M.HPlaceName, M.HMemberGroup, M.HRemark, M.HRoomName, M.HRoomTime, M.HStatus, M.HItemStatus, LEFT(M.HTask, len(M.HTask) - 1) AS HTask FROM (SELECT a.HAreaID, a.HArea, a.HSystemName, b.HMemberGroup, a.HTeamID, a.HPeriod, a.HID, a.HUserName, a.HAccount, a.HPhone, a.HCarrier, a.HRainbow, a.HLightEnvoy, b.HCourseID, b.HDateRange, a.HSex, b.HAttend, b.HOCPlace, b.HLDate, b.HRemark, b.HPlaceName, h.HRoomName, b.HRoomTime, b.HStatus, b.HItemStatus, b.HCourseName, (SELECT DISTINCT (cast(HTask AS NVARCHAR) + '、')  FROM HCourseBooking_Group CROSS APPLY SPLIT(HTask, '、') where HCourseBooking_Group.HBookingID = b.HID  FOR XML PATH('')) AS HTask FROM MemberList AS a JOIN OrderList_Merge AS b ON a.HID = b.HMemberID  LEFT JOIN HArea AS d ON a.HAreaID = d.HID  LEFT JOIN HLArea AS f ON d.HLAreaID = f.HID left join HCourseBooking_Group as g on b.HID = g.HBookingID left join HRoom as h on b.HRoom = h.HID GROUP BY a.HAreaID, a.HArea, a.HSystemName,b.HMemberGroup, a.HTeamID, a.HPeriod, a.HID, a.HUserName,a.HAccount, a.HPhone,a.HCarrier, a.HRainbow, a.HLightEnvoy,  b.HCourseID,b.HDateRange, a.HSex, b.HAttend, b.HOCPlace, b.HLDate, b.HRemark, b.HPlaceName, h.HRoomName, b.HRoomTime, b.HStatus, b.HItemStatus,b.HID, b.HCourseName) M WHERE M.HCourseName = N'" + TotalList_CA + "' AND M.HDateRange ='" + TotalList_CN + "'  AND M.HStatus = '1'  AND M.HItemStatus = '1' ORDER BY HAreaID ASC, M.HTeamID DESC,  M.HSystemName DESC";

        //SDS_HC.SelectCommand = "SELECT * FROM Course_BackendList where HStatus='1'";
        Rpt_TotalList.DataBind();

        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
        // Pg_TotalList.FrontPagingLoad("HochiSystemConnection", SDS_TotalList.SelectCommand, PageMax, LastPage, true, Rpt_TotalList);


        ViewState["TotalList"] = SDS_TotalList.SelectCommand;


        LB_HCourseNameTLTitle.Text = TotalList_CA;
        LB_HDateRangeTLTitle.Text = TotalList_CN;
        //LB_HDateRangeTLTitle.Text = "課程日期：" + TotalList_CN;

        //SqlDataReader QueryHCourse = SQLdatabase.ExecuteReader("select a.HCourseName, a.HDateRange from HCourse as a where a.HCourseName='" + TotalList_CA + "' and a.HDateRange='" + TotalList_CN + "'");

        //while (QueryHCourse.Read())
        //{
        //	LB_HCourseNameTLTitle.Text = QueryHCourse["HCourseName"].ToString();
        //	LB_HDateRangeTLTitle.Text = "課程日期：" + QueryHCourse["HDateRange"].ToString();
        //}

        //QueryHCourse.Close();




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
            //LightDataCache.LoadData("../App_Data/light_data.json");

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

        //1 = 參班【一般】、2 = 參班【學青(25歲以下在學青年)】、3 = 參班【經濟困難(需經光團1號導師審核通過)】、4 = 參班【經濟困難(護持出家師父及全心奉獻各修行組織團體之清修人士(請光團1號導師協助確認)】、5 = 不參班(專業護持)、6 = 不參班(純護持)

        //((Label)e.Item.FindControl("LB_HAttend")).Text = gDRV["HAttend"].ToString() == "5" ? "專業護持者" : gDRV["HAttend"].ToString() == "" || gDRV["HAttend"].ToString() == null ? "護持者" : gDRV["HAttend"].ToString() == "0" ? "" : "參班";


        //AE20231017_更正1 = 參班【一般】、5 = 純護持(非班員)、6 = 參班兼護持
        ((Label)e.Item.FindControl("LB_HAttend")).Text = gDRV["HAttend"].ToString() == "5" ? "純護持(非班員)" : gDRV["HAttend"].ToString() == "6" ? "參班兼護持" : gDRV["HAttend"].ToString() == "" || gDRV["HAttend"].ToString() == null ? "未選擇" : gDRV["HAttend"].ToString() == "0" ? "" : "參班";
    }

    #endregion

    //protected void LBtn_ApplyCheck_Click(object sender, EventArgs e)
    //{
    //	Panel_CourseList.Visible = false;
    //	Panel_TotalList.Visible = false;
    //	Panel_Identity.Visible = false;
    //	Panel_TotalListArea.Visible = false;
    //}

    #region 課程列表繫結
    protected void Rpt_HC_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        //EA20240323_加入報名人數
        //SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT COUNT(a.HUserName) AS  HApplyNum FROM MemberList as a join OrderList_Merge as b on a.HID = b.HMemberID where b.HCourseName = '" + gDRV["HCourseName"] + "' AND b.HDateRange = '" + gDRV["HDateRange"] + "' and b.HStatus = '1' and b.HItemStatus = '1'");


		//AE20240409_修改OrderList_Merge改成直接寫
		SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT COUNT(a.HUserName) AS  HApplyNum FROM MemberList as a join (SELECT  A.HMemberID, A.HCourseID, A.HCourseName, A.HDateRange FROM HCourseBooking AS A INNER JOIN HCourse AS B On A.HCourseID = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND (A.HChangeStatus IS NULL OR A.HChangeStatus = '2') UNION SELECT A.HMemberID, D.HCourseIDNew AS HCourseID, D.HCourseName, D.HDateRange FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON A.HOrderGroup = D.HOrderGroupNew INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND A.HCourseName = '換課' AND(D.HChangeStatus IS NULL OR D.HChangeStatus = '2') UNION SELECT A.HMemberID, D.HCourseIDNew AS HCourseID, D.HCourseName, D.HDateRange FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON(A.HOrderGroup = D.HOrderGroupSrc AND A.HOrderNum = D.HOrderNumSrc) OR(A.HOrderGroup = D.HOrderGroupSrc AND D.HOrderNumSrc = '') INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND(D.HOrderGroupNew = '' OR D.HOrderGroupNew IS NULL) UNION SELECT  A.HMemberID, D.HCourseIDNew AS HCourseID, D.HCourseName, D.HDateRange FROM HCourseBooking AS A LEFT JOIN HCBChangeRecord AS D ON(A.HOrderGroup = D.HOrderGroupSrc) LEFT JOIN HCBChangeRecord AS E ON(D.HOrderGroupSrc = E.HOrderGroupNew) INNER JOIN HCourse AS B On D.HCourseIDNew = B.HID INNER JOIN HPlace AS C On B.HOCPlace = C.HID WHERE A.HStatus = '1' AND A.HItemStatus = '1' AND A.HCourseName = '換課' AND(D.HOrderGroupNew = '' OR D.HOrderGroupNew IS NULL) AND(E.HChangeStatus = '1') AND(A.HChangeStatus IS NULL OR A.HChangeStatus = '2')) as b on a.HID = b.HMemberID where b.HCourseName = N'" + gDRV["HCourseName"] + "' AND b.HDateRange = '" + gDRV["HDateRange"] + "'");

		if (dr.Read())
        {
            ((Label)e.Item.FindControl("LB_HApplyNum")).Text = dr["HApplyNum"].ToString();
        }
        dr.Close();

        //((Label)e.Item.FindControl("LB_HTeacherName")).Text = "";
        //	string[] gHTeacherName = gDRV["HTeacherName"].ToString().Split(',');
        //	for (int i = 0; i < gHTeacherName.Length - 1; i++)
        //	{
        //		//Response.Write("SELECT HUserName FROM HMember where HID = '" + gHTeacherName[i].ToString() + "'");
        //		SqlDataReader QueryHMember = SQLdatabase.ExecuteReader("SELECT HUserName FROM HMember where HID = '" + gHTeacherName[i].ToString() + "'");
        //		if (QueryHMember.Read())
        //		{
        //			((Label)e.Item.FindControl("LB_HTeacherName")).Text = QueryHMember["HUserName"].ToString() + "," + ((Label)e.Item.FindControl("LB_HTeacherName")).Text;
        //	}
        //		QueryHMember.Close();
        //	}

        //((Label)e.Item.FindControl("LB_HTeacherName")).Text = ((Label)e.Item.FindControl("LB_HTeacherName")).Text.Substring(0, ((Label)e.Item.FindControl("LB_HTeacherName")).Text.Length-1);

        //AA20250625_新增是否開放單日報名的課程
        if (gDRV["HBookByDateYN"].ToString()=="1")
        {
            ((LinkButton)e.Item.FindControl("LBtn_BBDate")).Visible = true;
            ((LinkButton)e.Item.FindControl("LBtn_TotalListArea")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_Identity")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_TotalList")).Visible = false;
        }
        else
        {
            ((LinkButton)e.Item.FindControl("LBtn_BBDate")).Visible = false;
            ((LinkButton)e.Item.FindControl("LBtn_TotalListArea")).Visible = true;
            ((LinkButton)e.Item.FindControl("LBtn_Identity")).Visible = true;
            ((LinkButton)e.Item.FindControl("LBtn_TotalList")).Visible = true;
        }


    }
    #endregion

    #region 回上一頁
    protected void LBtn_Back_Click(object sender, EventArgs e)
    {
        // Response.Write("<script>window.location.href='HEnrollmentAll';</script>");

        Panel_Search.Visible = true;
        Panel_CourseList.Visible = true;
        Panel_TotalList.Visible = false;
        Panel_Identity.Visible = false;
        Panel_TotalListArea.Visible = false;

        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HC.SelectCommand, PageMax, LastPage, true, Rpt_HC);
    }
    #endregion

    #region 產生開放單日報名的報表
    //AA20250625_開放單日報名的課程報名顯示
    protected void LBtn_BBDate_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_BBDate = sender as LinkButton;
        string[] gBBDate_CA = LBtn_BBDate.CommandArgument.Split('^');
        //'<%# Eval("HCourseName") +"^"+ Eval("HDateRange")%>'

        Response.Write("<script>window.location.href='HBBDateEnrollmentAll.aspx?N=" + HttpUtility.HtmlEncode(gBBDate_CA[0]) + "&D=" + HttpUtility.HtmlEncode(gBBDate_CA[1]) + "';</script>");
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
}