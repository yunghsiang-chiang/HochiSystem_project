using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.HSSF.Util;
using NPOI.XSSF.Streaming;
using NPOI.SS.Formula.Functions;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Activities.Expressions;
using RestSharp;
using NPOI.XWPF.UserModel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Math;
using System.Drawing;

public partial class System_HAnnualReport : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    #region 常態課程小計
    int HNSubTotal1 = 0, HNSubTotal2 = 0, HNSubTotal3 = 0, HNSubTotal4 = 0, HNSubTotal5 = 0, HNSubTotal6 = 0, HNSubTotal7 = 0, HNSubTotal8 = 0, HNSubTotal9 = 0, HNSubTotal10 = 0, HNSubTotal11 = 0, HNSubTotal12 = 0;
    #endregion

    #region 單一課程小計
    int HSSubTotal1 = 0, HSSubTotal2 = 0, HSSubTotal3 = 0, HSSubTotal4 = 0, HSSubTotal5 = 0, HSSubTotal6 = 0, HSSubTotal7 = 0, HSSubTotal8 = 0, HSSubTotal9 = 0, HSSubTotal10 = 0, HSSubTotal11 = 0, HSSubTotal12 = 0;
    #endregion

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        #region 常態課程小計
        for (int x = 0; x < Rpt_HNormal.Items.Count; x++)
        {
            if (!string.IsNullOrEmpty(((Label)Rpt_HNormal.Items[x].FindControl("LB_HJanTotal")).Text))
            {
                HNSubTotal1 += Convert.ToInt32(((Label)Rpt_HNormal.Items[x].FindControl("LB_HJanTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HNormal.Items[x].FindControl("LB_HFebTotal")).Text))
            {
                HNSubTotal2 += Convert.ToInt32(((Label)Rpt_HNormal.Items[x].FindControl("LB_HFebTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HNormal.Items[x].FindControl("LB_HMarTotal")).Text))
            {
                HNSubTotal3 += Convert.ToInt32(((Label)Rpt_HNormal.Items[x].FindControl("LB_HMarTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HNormal.Items[x].FindControl("LB_HAprTotal")).Text))
            {
                HNSubTotal4 += Convert.ToInt32(((Label)Rpt_HNormal.Items[x].FindControl("LB_HAprTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HNormal.Items[x].FindControl("LB_HMayTotal")).Text))
            {
                HNSubTotal5 += Convert.ToInt32(((Label)Rpt_HNormal.Items[x].FindControl("LB_HMayTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HNormal.Items[x].FindControl("LB_HJunTotal")).Text))
            {
                HNSubTotal6 += Convert.ToInt32(((Label)Rpt_HNormal.Items[x].FindControl("LB_HJunTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HNormal.Items[x].FindControl("LB_HJulTotal")).Text))
            {
                HNSubTotal7 += Convert.ToInt32(((Label)Rpt_HNormal.Items[x].FindControl("LB_HJulTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HNormal.Items[x].FindControl("LB_HAugTotal")).Text))
            {
                HNSubTotal8 += Convert.ToInt32(((Label)Rpt_HNormal.Items[x].FindControl("LB_HAugTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HNormal.Items[x].FindControl("LB_HSepTotal")).Text))
            {
                HNSubTotal9 += Convert.ToInt32(((Label)Rpt_HNormal.Items[x].FindControl("LB_HSepTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HNormal.Items[x].FindControl("LB_HOctTotal")).Text))
            {
                HNSubTotal10 += Convert.ToInt32(((Label)Rpt_HNormal.Items[x].FindControl("LB_HOctTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HNormal.Items[x].FindControl("LB_HNovTotal")).Text))
            {
                HNSubTotal11 += Convert.ToInt32(((Label)Rpt_HNormal.Items[x].FindControl("LB_HNovTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HNormal.Items[x].FindControl("LB_HDecTotal")).Text))
            {
                HNSubTotal12 += Convert.ToInt32(((Label)Rpt_HNormal.Items[x].FindControl("LB_HDecTotal")).Text.Replace(",", ""));
            }
        }

        LB_HNSubTotal1.Text = HNSubTotal1.ToString("N0");
        LB_HNSubTotal2.Text = HNSubTotal2.ToString("N0");
        LB_HNSubTotal3.Text = HNSubTotal3.ToString("N0");
        LB_HNSubTotal4.Text = HNSubTotal4.ToString("N0");
        LB_HNSubTotal5.Text = HNSubTotal5.ToString("N0");
        LB_HNSubTotal6.Text = HNSubTotal6.ToString("N0");
        LB_HNSubTotal7.Text = HNSubTotal7.ToString("N0");
        LB_HNSubTotal8.Text = HNSubTotal8.ToString("N0");
        LB_HNSubTotal9.Text = HNSubTotal9.ToString("N0");
        LB_HNSubTotal10.Text = HNSubTotal10.ToString("N0");
        LB_HNSubTotal11.Text = HNSubTotal11.ToString("N0");
        LB_HNSubTotal12.Text = HNSubTotal12.ToString("N0");
        #endregion


        #region 單一課程小計
        for (int x = 0; x < Rpt_HSingle.Items.Count; x++)
        {
            if (!string.IsNullOrEmpty(((Label)Rpt_HSingle.Items[x].FindControl("LB_HJanTotal")).Text))
            {
                HSSubTotal1 += Convert.ToInt32(((Label)Rpt_HSingle.Items[x].FindControl("LB_HJanTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HSingle.Items[x].FindControl("LB_HFebTotal")).Text))
            {
                HSSubTotal2 += Convert.ToInt32(((Label)Rpt_HSingle.Items[x].FindControl("LB_HFebTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HSingle.Items[x].FindControl("LB_HMarTotal")).Text))
            {
                HSSubTotal3 += Convert.ToInt32(((Label)Rpt_HSingle.Items[x].FindControl("LB_HMarTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HSingle.Items[x].FindControl("LB_HAprTotal")).Text))
            {
                HSSubTotal4 += Convert.ToInt32(((Label)Rpt_HSingle.Items[x].FindControl("LB_HAprTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HSingle.Items[x].FindControl("LB_HMayTotal")).Text))
            {
                HSSubTotal5 += Convert.ToInt32(((Label)Rpt_HSingle.Items[x].FindControl("LB_HMayTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HSingle.Items[x].FindControl("LB_HJunTotal")).Text))
            {
                HSSubTotal6 += Convert.ToInt32(((Label)Rpt_HSingle.Items[x].FindControl("LB_HJunTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HSingle.Items[x].FindControl("LB_HJulTotal")).Text))
            {
                HSSubTotal7 += Convert.ToInt32(((Label)Rpt_HSingle.Items[x].FindControl("LB_HJulTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HSingle.Items[x].FindControl("LB_HAugTotal")).Text))
            {
                HSSubTotal8 += Convert.ToInt32(((Label)Rpt_HSingle.Items[x].FindControl("LB_HAugTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HSingle.Items[x].FindControl("LB_HSepTotal")).Text))
            {
                HSSubTotal9 += Convert.ToInt32(((Label)Rpt_HSingle.Items[x].FindControl("LB_HSepTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HSingle.Items[x].FindControl("LB_HOctTotal")).Text))
            {
                HSSubTotal10 += Convert.ToInt32(((Label)Rpt_HSingle.Items[x].FindControl("LB_HOctTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HSingle.Items[x].FindControl("LB_HNovTotal")).Text))
            {
                HSSubTotal11 += Convert.ToInt32(((Label)Rpt_HSingle.Items[x].FindControl("LB_HNovTotal")).Text.Replace(",", ""));
            }
            if (!string.IsNullOrEmpty(((Label)Rpt_HSingle.Items[x].FindControl("LB_HDecTotal")).Text))
            {
                HSSubTotal12 += Convert.ToInt32(((Label)Rpt_HSingle.Items[x].FindControl("LB_HDecTotal")).Text.Replace(",", ""));
            }
        }

        LB_HSSubTotal1.Text = HSSubTotal1.ToString("N0");
        LB_HSSubTotal2.Text = HSSubTotal2.ToString("N0");
        LB_HSSubTotal3.Text = HSSubTotal3.ToString("N0");
        LB_HSSubTotal4.Text = HSSubTotal4.ToString("N0");
        LB_HSSubTotal5.Text = HSSubTotal5.ToString("N0");
        LB_HSSubTotal6.Text = HSSubTotal6.ToString("N0");
        LB_HSSubTotal7.Text = HSSubTotal7.ToString("N0");
        LB_HSSubTotal8.Text = HSSubTotal8.ToString("N0");
        LB_HSSubTotal9.Text = HSSubTotal9.ToString("N0");
        LB_HSSubTotal10.Text = HSSubTotal10.ToString("N0");
        LB_HSSubTotal11.Text = HSSubTotal11.ToString("N0");
        LB_HSSubTotal12.Text = HSSubTotal12.ToString("N0");
        #endregion

        #region 總計
        LB_SumJan.Text = (HNSubTotal1+ HSSubTotal1).ToString("N0");
        LB_SumFeb.Text = (HNSubTotal2 + HSSubTotal2).ToString("N0");
        LB_SumMar.Text = (HNSubTotal3 + HSSubTotal3).ToString("N0");
        LB_SumApr.Text = (HNSubTotal4 + HSSubTotal4).ToString("N0");
        LB_SumMay.Text = (HNSubTotal5 + HSSubTotal5).ToString("N0");
        LB_SumJun.Text = (HNSubTotal6 + HSSubTotal6).ToString("N0");
        LB_SumJul.Text = (HNSubTotal7 + HSSubTotal7).ToString("N0");
        LB_SumAug.Text = (HNSubTotal8 + HSSubTotal8).ToString("N0");
        LB_SumSep.Text = (HNSubTotal9 + HSSubTotal9).ToString("N0");
        LB_SumOct.Text = (HNSubTotal10 + HSSubTotal10).ToString("N0");
        LB_SumNov.Text = (HNSubTotal11 + HSSubTotal11).ToString("N0");
        LB_SumDec.Text = (HNSubTotal12 + HSSubTotal12).ToString("N0");

        #endregion
    }


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

        if (!IsPostBack)
        {
            #region 選單
            for (int i = 0; i < 10; i++)
            {
                DDL_SHCDYear.Items.Add(new ListItem((DateTime.Now.Year - i).ToString(), (DateTime.Now.Year - i).ToString()));
            }

            for (int i = 1; i <= 12; i++)
            {
                DDL_SHCDMonth.Items.Add(new ListItem(i.ToString("00"), i.ToString("00")));
            }


            #endregion
        }

    }


    #region 搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {

        #region 年份+月份必選
        //WA250114_新增必填判斷，避免資料過多連線逾時
        if (DDL_SHCDYear.SelectedValue =="99" && DDL_SHCDMonth.SelectedValue == "99")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('請先選擇課程年份和課程月份哦~!');", true);
             return;
        }
        #endregion


        #region 常態課程


        SDS_HNormal.SelectCommand = "EXEC sp_GetMonthlyCourseOrder @SearchYear = " + DDL_SHCDYear.SelectedValue + ", @SearchMonth =  "+ DDL_SHCDMonth.SelectedValue;


        Rpt_HNormal.DataSourceID = "SDS_HNormal";
        Rpt_HNormal.DataBind();

        Panel_NormalList.Visible = true;

        #endregion

        #region 單一課程


        SDS_HSingle.SelectCommand= "EXEC sp_GetOtherCoursesMonthly @SearchYear = " + DDL_SHCDYear.SelectedValue + ", @SearchMonth =  "+ DDL_SHCDMonth.SelectedValue+ ",@SearchPMethod =" + DDL_SHPMethod.SelectedValue;
        Rpt_HSingle.DataSourceID = "SDS_HSingle";
        Rpt_HSingle.DataBind();

        Panel_SingleList.Visible = true;


        #endregion



    }
    #endregion

    #region 搜尋取消功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_SHCourseName.Text = "";
        DDL_SHCDYear.Text = "99";
        DDL_SHPMethod.SelectedValue = "0";
        DDL_SHCDMonth.SelectedValue = "99";
        Panel_NormalList.Visible = false;
        SDS_HNormal.SelectCommand = "";
        Rpt_HNormal.DataBind();

        Panel_SingleList.Visible = false;
        SDS_HSingle.SelectCommand = "";
        Rpt_HSingle.DataBind();

    }
    #endregion


    #region 匯出文化報名明細表(依付款日期條件)
    protected void LBtn_AnnualReport_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_SHPaymentDate.Text))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('請輸入付款日期區間!');", true);
            return;
        }

        DataTable dataTable = new DataTable();

        SqlCommand dbCmd = default(SqlCommand);


        string QuerySql = "SELECT a.HOrderGroup, a.HOrderNum, IIF(a.HCourseName IS NOT NULL,a.HCourseName, e.HCourseName) AS HCourseName, (b.HArea + '/' + b.HPeriod + ' ' + b.HUserName) AS HUserName,IIF(a.HOrderGroupSrc <>'', a.HSubTotal ,IIF( a.HPMethod='2' AND (a.HCourseDonate='0' OR a.HCourseDonate IS NULL ), Convert(int,a.HPoint)*10 ,a.HPoint)) AS HPoint, IIF(c.HUse IS NULL, 0, Convert(int,c.HUse )*10) AS HUse, a.HOrderGroupSrc,a.HECPAmount FROM OrderList_Detail a LEFT JOIN MemberList AS b ON a.HMemberID = b.HID LEFT JOIN HPoints AS c ON a.HOrderGroup = c.HOrderGroup  LEFT JOIN HCourse AS e ON a.HCourseID=e.HID Cross Apply SPLIT(Replace(IIF(a.HDateRange IS NOT NULL,a.HDateRange, e.HDateRange ),' - ',','), ',') AS d ";

        //搜尋條件
        StringBuilder sql = new StringBuilder(QuerySql);
        List<string> WHERE = new List<string>();


        //付款日期
        string[] gHPDateRangeArray = TB_SHPaymentDate.Text.Trim() == "" ? "2000/01/01-3000/12/31".Split('-') : TB_SHPaymentDate.Text.Split('-');
        if (!string.IsNullOrEmpty(TB_SHPaymentDate.Text))
        {
            WHERE.Add(" (CONVERT(varchar(10), a.HPaymentDate, 111) >='" + gHPDateRangeArray[0].Trim() + "' AND CONVERT(varchar(10), a.HPaymentDate, 111)<='" + gHPDateRangeArray[1].Trim() + "') ");
        }


        //課程/捐款：0=課程；1=捐款
        //繳費帳戶：1=基金會；2=文化事業
        //訂單狀態：1=訂單成立、2=訂單取消
        //付款狀態：1=已付款、2=未付款、3=已退款
        //WHERE.Add(" (a.HCourseDonate =0 OR a.HCourseDonate ='' OR a.HCourseDonate IS NULL) AND ( a.HPMethod =2) AND ( a.HStatus =1) AND ( a.HItemStatus =1) ");

        //WE20240108_加入部分退款判斷
        //WHERE.Add(" (a.HCourseDonate =0 OR a.HCourseDonate ='' OR a.HCourseDonate IS NULL) AND ( a.HPMethod =2) AND ( a.HStatus =1) AND ( a.HItemStatus =1 OR a.HItemStatus=3) ");

        //250110_Add
        WHERE.Add(" (a.HCourseDonate =0 OR a.HCourseDonate ='' OR a.HCourseDonate IS NULL) AND ( a.HPMethod =2) AND ( a.HStatus =1) AND(a.HItemStatus = 1 OR a.HItemStatus = 3) AND(a.HSubTotal - IIF(HCXLAmount IS NOT NULL, HCXLAmount, 0)) != 0 ");
        

        //判斷使用者是否選擇了條件
        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" WHERE " + wh+ " ");/*AND a.HOrderGroup='C202412310006'*/
        }
        con.Open();


        //WE20250109_
        dbCmd = new SqlCommand(sql.ToString() + "  GROUP BY  a.HInvoiceNo, a.HInvoiceDate, a.HPaymentDate, a.HOrderGroup, a.HOrderGroupSrc, a.HOrderNum, a.HMerchantTradeNo, a.HTradeNo, a.HCourseName, e.HCourseName, a.HPMethod, b.HArea, b.HPeriod, b.HUserName,a.HAttend, a.HDCode, a.HCourseDonate, a.HCourseDonate, c.HUse, a.HPayMethod, a.HPaymentDate, a.HStatus, a.HItemStatus, a.HChangeStatus, a.HPoint, a.HECPAmount,a.HSubTotal,a.HCXLAmount, a.HCXLTotal ORDER BY a.HPaymentDate ASC,HCXLTotal DESC,a.HOrderNum ASC", con);


        // create data adapter
        SqlDataAdapter da = new SqlDataAdapter(dbCmd);
        // this will query your database and return the result to your datatable
        da.Fill(dataTable);



        #region  TEST
        IWorkbook workbook;
        string fileExt = Path.GetExtension(".xlsx").ToLower();
        if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); } else { workbook = null; }
        if (workbook == null) { return; }
        string filename = "Order";

        //pmethod：1=匯出年度各月各課程大表；2=年度課程報表

        ISheet sheet = string.IsNullOrEmpty(dataTable.TableName) ? workbook.CreateSheet("報名文化課程總計") : workbook.CreateSheet(dataTable.TableName);
        filename = "文化年度各月課程大表";

        //表頭  
        IRow row = sheet.CreateRow(0);
        //利用sheet建立列
        sheet.GetRow(0).CreateCell(0).SetCellValue("訂單代碼");
        sheet.GetRow(0).CreateCell(1).SetCellValue("項目編號");
        sheet.GetRow(0).CreateCell(2).SetCellValue("課程名稱");
        sheet.GetRow(0).CreateCell(3).SetCellValue("學員姓名");
        sheet.GetRow(0).CreateCell(4).SetCellValue("報名/護持金額");
        sheet.GetRow(0).CreateCell(5).SetCellValue("使用點數");
        //WA20240105_新增欄位
        sheet.GetRow(0).CreateCell(6).SetCellValue("原訂單代碼");
        //sheet.GetRow(0).CreateCell(7).SetCellValue("原訂單已付總金額");

        //sheet.GetRow(0).CreateCell(7).SetCellValue("部分退款金額");
        //sheet.GetRow(0).CreateCell(8).SetCellValue("原訂單付款總金額");

        sheet.GetRow(0).CreateCell(7).SetCellValue("付款總金額");

        //資料  
        for (int i = 0; i < dataTable.Rows.Count; i++)
        {
            IRow row1 = sheet.CreateRow(i + 1);
            for (int j = 0; j < dataTable.Columns.Count; j++)
            {
                //sheet.AutoSizeColumn((short)j);//自動根據長度調整儲存格寬度
                NPOI.SS.UserModel.ICell cell = row1.CreateCell(j);
                cell.SetCellValue(dataTable.Rows[i][j].ToString());
            }




            #region 格式轉換
            //IRow AmtFormat = sheet.GetRow(i);
            //AmtFormat.GetCell(6).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");

            //報名/護持金額
            IRow HPointFormat = sheet.GetRow(i + 1);
            //HPointFormat.GetCell(4).SetCellFormula("Value(" + dataTable.Rows[i][4].ToString() + ")");
            HPointFormat.GetCell(4).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##");

            //WE20250109_部分退款金額變更對應的欄位位置
            if (!string.IsNullOrEmpty(dataTable.Rows[i][7].ToString()))
            {
                IRow HCXLTotalFormat = sheet.GetRow(i + 1);
                //HCXLTotalFormat.GetCell(7).SetCellFormula("Value(" + dataTable.Rows[i][7].ToString() + ")");
                HCXLTotalFormat.GetCell(7).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##");
            }

            //WE20240106_付款總金額變更對應的欄位位置
            IRow HECPAmountFormat = sheet.GetRow(i + 1);
            //HECPAmountFormat.GetCell(8).SetCellFormula("Value(" + dataTable.Rows[i][8].ToString() + ")");
            //HECPAmountFormat.GetCell(8).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##");

            HECPAmountFormat.GetCell(7).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##");

            //WE20250109_最後訂單總金額變更對應的欄位位置
            //IRow HFinalTotalFormat = sheet.GetRow(i + 1);
            //HFinalTotalFormat.GetCell(9).SetCellFormula("Value(" + dataTable.Rows[i][9].ToString() + ")");
            //HFinalTotalFormat.GetCell(9).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##");
            #endregion

            #region 付款總金額 隱藏
            //sheet.SetColumnHidden(6, true);
            #endregion

        }



        //做加總
        for (int i = 0; i < dataTable.Rows.Count; i++)
        {
         
            int sameCount = 0;

            //WE20240106_付款總金額變更對應的欄位位置
            
            string gOrderCode = dataTable.Rows[i][0].ToString();  // 訂單代碼

            //加總相同訂單的退款金額
            for (int k = i + 1; k < dataTable.Rows.Count; k++)
            {
               
                //判斷若訂單代碼相同
                if (dataTable.Rows[k][0].ToString() == gOrderCode)
                {

                    //Response.Write("CXLA=" + refundAmount+"<br>");
                    sameCount++;
                    sheet.AutoSizeColumn((short)k);//自動根據長度調整儲存格寬度

                    sheet.GetRow(k + 1).GetCell(7).SetCellValue("0");

                    //WA20240106_把後面得值變0
                    //sheet.GetRow(k + 1).GetCell(8).SetCellValue("0");
                    //sheet.GetRow(k + 1).GetCell(9).SetCellValue("0");

                   

                }
            }

            //Response.Write("CXL="+ gTotalRefundAmount);
            //Response.End();

            if (sameCount != 0)
            {
                int end = i + sameCount + 1;




                CellRangeAddress strHOrderGroup = new CellRangeAddress(i + 1, end, 0, 0);//訂單代碼
                sheet.AddMergedRegion(strHOrderGroup);

                CellRangeAddress strUserName = new CellRangeAddress(i + 1, end, 3, 3);//學員姓名
                sheet.AddMergedRegion(strUserName);

                CellRangeAddress strHPoint = new CellRangeAddress(i + 1, end, 5, 5);//使用點數
                sheet.AddMergedRegion(strHPoint);

                //WA20240106_原訂單代碼合併儲存格
                CellRangeAddress strHOrderGroupSrc = new CellRangeAddress(i + 1, end, 6, 6);//原訂單代碼
                sheet.AddMergedRegion(strHOrderGroupSrc);

                //WA20240108_原訂單付款總金額合併儲存格
                CellRangeAddress strHECPAmount = new CellRangeAddress(i + 1, end, 8, 8);//付款總金額
                sheet.AddMergedRegion(strHECPAmount);

                //WA20250109_最後訂單付款總金額合併儲存格
                CellRangeAddress strFinalTotal = new CellRangeAddress(i + 1, end, 9, 9);//最後訂單付款總金額
                sheet.AddMergedRegion(strFinalTotal);


              

                i += sameCount;
            }



            #region 統計行
            if (i == dataTable.Rows.Count - 1)//最後一行
            {
                IRow rowCount = sheet.CreateRow(i + 4);//追加一行統計列
                                                       //sheet.AddMergedRegion(new CellRangeAddress(i +1, i + 1, 0, 5));//合併單元格（第幾行，到第幾行，第幾列，到第幾列）
                NPOI.SS.UserModel.ICell cellEnd1 = rowCount.CreateCell(3);
                NPOI.SS.UserModel.ICell cellEnd2 = rowCount.CreateCell(4);
                NPOI.SS.UserModel.ICell cellEnd3 = rowCount.CreateCell(5);
                NPOI.SS.UserModel.ICell cellEnd4 = rowCount.CreateCell(6);
                NPOI.SS.UserModel.ICell cellEnd5 = rowCount.CreateCell(7);
                NPOI.SS.UserModel.ICell cellEnd6 = rowCount.CreateCell(8);
                NPOI.SS.UserModel.ICell cellEnd7 = rowCount.CreateCell(9);
                cellEnd1.SetCellValue("總計：");
                sheet.SetColumnWidth(6, 16 * 256);//設置單元格寬度
                sheet.SetColumnWidth(7, 14 * 256);
                sheet.SetColumnWidth(8, 16 * 256);
                sheet.SetColumnWidth(9, 14 * 256);
                string CellFormulaString1 = "sum(E2:E" + (dataTable.Rows.Count + 1) + ")"; //計算表達式，行統計： sum（A1:C1）,列統計：sum(B1,B10) (報名金額)
                string CellFormulaString2 = "sum(F2:F" + (dataTable.Rows.Count + 1) + ")"; //計算表達式 (使用點數)
                string CellFormulaString3 = "sum(H2:H" + (dataTable.Rows.Count + 1) + ")"; //計算表達式 (部分退款金額)
                string CellFormulaString4 = "sum(I2:I" + (dataTable.Rows.Count + 1) + ")"; //計算表達式(原訂單付款金額)
                string CellFormulaString5 = "sum(J2:J" + (dataTable.Rows.Count + 1) + ")"; //計算表達式(最後訂單付款金額)
                sheet.GetRow(dataTable.Rows.Count + 3).GetCell(4).SetCellFormula(CellFormulaString1);//賦值表達式
                sheet.GetRow(dataTable.Rows.Count + 3).GetCell(5).SetCellFormula(CellFormulaString2);
                sheet.GetRow(dataTable.Rows.Count + 3).GetCell(7).SetCellFormula(CellFormulaString3);
                sheet.GetRow(dataTable.Rows.Count + 3).GetCell(8).SetCellFormula(CellFormulaString4);
                sheet.GetRow(dataTable.Rows.Count + 3).GetCell(9).SetCellFormula(CellFormulaString5);



            }
            #endregion

        }


        //下載Excel檔
        //HttpContext.Current.Response.BinaryWrite(stream.GetBuffer());
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", filename + DateTime.Now.ToString("yyyyMMdd") + ".xlsx"));
        HttpContext.Current.Response.AddHeader("Transfer-Encoding", "identity");
        workbook.Write(HttpContext.Current.Response.OutputStream);
        HttpContext.Current.Response.Flush();
        HttpContext.Current.Response.Close();



        #endregion


        //WE20240106_暫時註解
        TableToExcel(dataTable, ".xlsx", "1");

        con.Close();
        dbCmd.Cancel();

    }
    #endregion

    #region 匯出年度課程報表
    protected void LBtn_MCourseReport_Click(object sender, EventArgs e)
    {
        if (DDL_SHCDYear.SelectedValue == "99" || DDL_SHCDMonth.SelectedValue == "99")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('請輸入年份與月份!');", true);
            return;
        }

    }
    #endregion

    #region Datable匯出成Excel
    /// <summary>
    /// Datable匯出成Excel
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="file"></param>
    public static void TableToExcel(DataTable dt, string file, string pmethod)
    {
        IWorkbook workbook;
        string fileExt = Path.GetExtension(file).ToLower();
        if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(); } else { workbook = null; }
        if (workbook == null) { return; }
        string filename = "Order";

        //pmethod：1=匯出年度各月各課程大表；2=年度課程報表
        if (pmethod == "1")
        {
            ISheet sheet = string.IsNullOrEmpty(dt.TableName) ? workbook.CreateSheet("年度各月各課程報表") : workbook.CreateSheet(dt.TableName);
            filename = "年度各月各課程報表";

            //表頭  
            IRow row = sheet.CreateRow(0);
            //利用sheet建立列
            sheet.GetRow(0).CreateCell(0).SetCellValue("訂單代碼");
            sheet.GetRow(0).CreateCell(1).SetCellValue("項目編號");
            sheet.GetRow(0).CreateCell(2).SetCellValue("課程名稱");
            sheet.GetRow(0).CreateCell(3).SetCellValue("學員姓名");
            sheet.GetRow(0).CreateCell(4).SetCellValue("報名/護持金額");
            sheet.GetRow(0).CreateCell(5).SetCellValue("使用點數");
            //WA20240105_新增欄位
            sheet.GetRow(0).CreateCell(6).SetCellValue("原訂單代碼");
            //sheet.GetRow(0).CreateCell(7).SetCellValue("原訂單已付總金額");

            sheet.GetRow(0).CreateCell(7).SetCellValue("付款總金額");


            //資料  
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row1 = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    //sheet.AutoSizeColumn((short)j);//自動根據長度調整儲存格寬度
                    NPOI.SS.UserModel.ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());

                }

                #region 格式轉換
                //IRow AmtFormat = sheet.GetRow(i);
                //AmtFormat.GetCell(6).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");

                //報名/護持金額
                IRow HPointFormat = sheet.GetRow(i + 1);
                HPointFormat.GetCell(4).SetCellFormula("Value(" + dt.Rows[i][4].ToString() + ")");
                HPointFormat.GetCell(4).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##");

                //WE20240106_付款總金額變更對應的欄位位置
                IRow AmtFormat = sheet.GetRow(i + 1);
                AmtFormat.GetCell(7).SetCellFormula("Value(" + dt.Rows[i][7].ToString() + ")");
                AmtFormat.GetCell(7).CellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##");





                #endregion

                #region 付款總金額 隱藏
                //sheet.SetColumnHidden(6, true);
                #endregion

            }


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int sameCount = 0;
                //int subtotal = Convert.ToInt32(dt.Rows[i][6].ToString());

                //WE20240106_付款總金額變更對應的欄位位置
                int subtotal = Convert.ToInt32(dt.Rows[i][7].ToString());

                for (int k = i + 1; k < dt.Rows.Count; k++)
                {
                    if (dt.Rows[i][0].ToString() == dt.Rows[k][0].ToString())
                    {
                        sameCount++;
                        sheet.AutoSizeColumn((short)k);//自動根據長度調整儲存格寬度
                                                       //subtotal += Convert.ToInt32(dt.Rows[k][6].ToString());
                                                       // //WA20240106
                                                       //sheet.GetRow(k).GetCell(7).SetCellValue("0");
                    }
                }




                if (sameCount != 0)
                {
                    int end = i + sameCount + 1;

                    #region 將重複的資料的金額變成0


                    for (int k = i + 1; k <= end; k++)
                    {
                        sheet.GetRow(k).GetCell(7).SetCellValue("");
                    }
                    #endregion

                    CellRangeAddress strHOrderGroup = new CellRangeAddress(i + 1, end, 0, 0);//訂單代碼
                    sheet.AddMergedRegion(strHOrderGroup);

                    CellRangeAddress strUserName = new CellRangeAddress(i + 1, end, 3, 3);//學員姓名
                    sheet.AddMergedRegion(strUserName);

                    CellRangeAddress strHPoint = new CellRangeAddress(i + 1, end, 5, 5);//使用點數
                    sheet.AddMergedRegion(strHPoint);

                    CellRangeAddress strHPayAmt = new CellRangeAddress(i + 1, end, 7, 7);//付款總金額



                    sheet.AddMergedRegion(strHPayAmt);



                    i += sameCount;
                }



                #region 統計行
                if (i == dt.Rows.Count - 1)//最後一行
                {
                    IRow rowCount = sheet.CreateRow(i + 4);//追加一行統計列
                                                           //sheet.AddMergedRegion(new CellRangeAddress(i +1, i + 1, 0, 5));//合併單元格（第幾行，到第幾行，第幾列，到第幾列）
                    NPOI.SS.UserModel.ICell cellEnd1 = rowCount.CreateCell(3);
                    NPOI.SS.UserModel.ICell cellEnd2 = rowCount.CreateCell(4);
                    NPOI.SS.UserModel.ICell cellEnd3 = rowCount.CreateCell(5);
                    NPOI.SS.UserModel.ICell cellEnd4 = rowCount.CreateCell(6); ;
                    cellEnd1.SetCellValue("總計：");
                    //cellEnd3.SetCellValue("確認總金額：");
                    //cellEnd1.CellStyle = textStyle;
                    //cellEnd3.CellStyle = textStyle;
                    //cellEnd2.CellStyle = moneyStyle;
                    //cellEnd4.CellStyle = moneyStyle;
                    sheet.SetColumnWidth(6, 16 * 256);//設置單元格寬度
                    sheet.SetColumnWidth(7, 14 * 256);
                    sheet.SetColumnWidth(8, 16 * 256);
                    sheet.SetColumnWidth(9, 14 * 256);
                    string CellFormulaString1 = "sum(E2:E" + (dt.Rows.Count + 1) + ")"; //計算表達式，行統計： sum（A1:C1）,列統計：sum(B1,B10)
                    string CellFormulaString2 = "sum(F2:F" + (dt.Rows.Count + 1) + ")"; //計算表達式
                    string CellFormulaString3 = "sum(G2:G" + (dt.Rows.Count + 1) + ")"; //計算表達式
                    sheet.GetRow(dt.Rows.Count + 3).GetCell(4).SetCellFormula(CellFormulaString1);//賦值表達式
                    sheet.GetRow(dt.Rows.Count + 3).GetCell(5).SetCellFormula(CellFormulaString2);
                    sheet.GetRow(dt.Rows.Count + 3).GetCell(6).SetCellFormula(CellFormulaString3);



                }
                #endregion

            }

        }




        //下載Excel檔
        //HttpContext.Current.Response.BinaryWrite(stream.GetBuffer());
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", filename + DateTime.Now.ToString("yyyyMMdd") + ".xlsx"));
        HttpContext.Current.Response.AddHeader("Transfer-Encoding", "identity");
        workbook.Write(HttpContext.Current.Response.OutputStream);
        HttpContext.Current.Response.Flush();
        HttpContext.Current.Response.Close();

    }
    #endregion

}