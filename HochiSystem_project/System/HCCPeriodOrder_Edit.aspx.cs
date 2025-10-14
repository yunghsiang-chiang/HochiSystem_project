using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using ClosedXML.Excel;
using System.Net;
using NPOI.SS.Formula.Functions;
using System.Activities.Expressions;

//AA20230808_新增信用卡定期定額授權訂單頁面
public partial class System_HCCPeriodOrder_Edit : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);


    #region 分頁copy-1
    private int PageMax = 15;   //分頁最大顯示數量
    private int LastPage = 10;  //分頁數量
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        #region 分頁copy-2
        if (!IsPostBack)
        {
            ViewState["Search"] = "";

            //SDS_HCCPOrder.SelectCommand = "SELECT  HID, HCCPOrderCode, HCCPImportNum FROM HCCPOrder WHERE HStatus=1 AND HPaperYN='1' ORDER BY HCreateDT DESC";

            SDS_HCCPOrder.SelectCommand = "SELECT a.HID, a.HCCPOrderCode, a.HCCPImportNum, STRING_AGG(b.HCCPeriodCode, ',') AS HCCPeriodCode FROM HCCPOrder AS a JOIN HCCPOrderDetail AS b ON a.HID = b.HCCPOrderID WHERE a.HStatus = 1 AND a.HPaperYN = '1' GROUP BY a.HID, a.HCCPOrderCode, a.HCCPImportNum,a.HCreateDT ORDER BY a.HCreateDT DESC";
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCPOrder.SelectCommand, PageMax, LastPage, false, Rpt_HCCPOrder);
            ViewState["Search"] = SDS_HCCPOrder.SelectCommand;
        }
        else
        {
            SDS_HCCPOrder.SelectCommand = ViewState["Search"].ToString();
            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.FrontPagingLoad("HochiSystemConnection", ViewState["Search"].ToString(), PageMax, LastPage, false, Rpt_HCCPOrder);
        }
        #endregion
    }

    #region 編輯功能
    protected void LBtn_Edit_Click(object sender, EventArgs e)
    {
        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        Panel_List.Visible = false;
        Panel_Edit.Visible = true;

        LB_HID.Text = ((Label)RI.FindControl("LB_HID")).Text;
        TB_HCCPOrderCode.Text = ((Label)RI.FindControl("LB_HCCPOrderCode")).Text;
        //TB_HCCPImportNum.Text = ((Label)RI.FindControl("LB_HCCPImportNum")).Text;
        TB_FHCCPImportNum.Text = ((Label)RI.FindControl("LB_HCCPImportNum")).Text;

        SDS_HCCPOrderDetail.SelectCommand = "SELECT b.HID, a.HCCPOrderCode, a.HCCPImportNum, b.HCCPeriodCode,  b.HMerchantTradeNo,b.HCourseName, b.HDUserName,  b.HDonor,  b.HCardHolder,  b.HCardNum,  b.HCardBank,  b.HCVCCode,  b.HCardValidDate, b.HDTotal, b.HDCCPTimes, b.HDCCPAmount, b.HCHPhone, b.HRtnCode FROM HCCPOrder AS a LEFT JOIN HCCPOrderDetail AS b ON a.HID=b.HCCPOrderID WHERE  a.HCCPOrderCode='" + ((Label)RI.FindControl("LB_HCCPOrderCode")).Text + "' AND a.HPaperYN='1' ";
        Rpt_HCCPOrderDetail.DataBind();

    }
    #endregion

    #region  搜尋功能
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        //string Select = "SELECT  HID, HCCPOrderCode, HCCPImportNum FROM HCCPOrder ";

        string Select = "SELECT a.HID, a.HCCPOrderCode, a.HCCPImportNum, STRING_AGG(b.HCCPeriodCode, ',') AS HCCPeriodCode FROM HCCPOrder AS a JOIN HCCPOrderDetail AS b ON a.HID = b.HCCPOrderID ";

        //搜尋條件
        StringBuilder sql = new StringBuilder(Select);
        List<string> WHERE = new List<string>();

        if (string.IsNullOrEmpty(TB_HCCPOrderCodeS.Text.Trim()) && string.IsNullOrEmpty(TB_HTMNameS.Text.Trim()))
        {
            Response.Write("<script>alert('請先輸入至少一個搜尋條件~');</script>");
            return;
        }

        if (!string.IsNullOrEmpty(TB_HCCPOrderCodeS.Text.Trim()))
        {
            WHERE.Add(" (a.HCCPOrderCode LIKE N'%" + TB_HCCPOrderCodeS.Text.Trim() + "%' OR HCCPeriodCode LIKE N'%"+ TB_HCCPOrderCodeS.Text.Trim() + "%') ");
        }

        if (!string.IsNullOrEmpty(TB_HTMNameS.Text.Trim()))
        {
            WHERE.Add(" a.HCCPImportNum LIKE N'%" + TB_HTMNameS.Text.Trim() + "%'");
        }


        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" WHERE " + wh);
        }


        //SDS_HCCPOrder.SelectCommand = "SELECT  HID, HCCPOrderCode, HCCPImportNum FROM HCCPOrder WHERE HCCPImportNum LIKE '%" + TB_HTMNameS.Text.Trim() + "%' AND HStatus=1 ORDER BY HCreateDT DESC";

        //Response.Write(sql.ToString());
        //Response.End();
        SDS_HCCPOrder.SelectCommand = sql.ToString() + "  AND a.HStatus=1 AND a.HPaperYN='1' GROUP BY a.HID, a.HCCPOrderCode, a.HCCPImportNum,a.HCreateDT ORDER BY a.HCreateDT DESC";
        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCPOrder.SelectCommand, PageMax, LastPage, true, Rpt_HCCPOrder);
        ViewState["Search"] = SDS_HCCPOrder.SelectCommand;
    }
    #endregion

    #region 取消搜尋功能
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        TB_HCCPOrderCodeS.Text = null;
        TB_HTMNameS.Text = null; //EE20230815_清除搜尋資料
        //SDS_HCCPOrder.SelectCommand = "SELECT  HID, HCCPOrderCode, HCCPImportNum FROM HCCPOrder  WHERE HStatus=1  AND HPaperYN='1' ORDER BY HCreateDT DESC";

        SDS_HCCPOrder.SelectCommand = "SELECT a.HID, a.HCCPOrderCode, a.HCCPImportNum, STRING_AGG(b.HCCPeriodCode, ',') AS HCCPeriodCode FROM HCCPOrder AS a JOIN HCCPOrderDetail AS b ON a.HID = b.HCCPOrderID WHERE a.HStatus = 1 AND a.HPaperYN = '1' GROUP BY a.HID, a.HCCPOrderCode, a.HCCPImportNum,a.HCreateDT ORDER BY a.HCreateDT DESC";
        Pg_Paging.FrontPagingLoad("HochiSystemConnection", SDS_HCCPOrder.SelectCommand, PageMax, LastPage, true, Rpt_HCCPOrder);
        ViewState["Search"] = SDS_HCCPOrder.SelectCommand;

    }
    #endregion

    #region 授權訂單明細資料繫結
    protected void Rpt_HCCPOrderDetail_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        //千分位
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HDCCPAmount")).Text))
        {
            ((Label)e.Item.FindControl("LB_HDCCPAmount")).Text = Convert.ToInt32(((Label)e.Item.FindControl("LB_HDCCPAmount")).Text).ToString("N0");
        }

        //判斷授權狀態(HRtnCode)：0=授權失敗、1=授權成功、2=處理中
        ((Label)e.Item.FindControl("LB_HRtnCode")).CssClass = gDRV["HRtnCode"].ToString() == "0" ? "badge badge-danger border-0" : gDRV["HRtnCode"].ToString() == "1" ? "badge badge-success border-0" : "badge badge-info border-0";
        ((Label)e.Item.FindControl("LB_HRtnCode")).Text = gDRV["HRtnCode"].ToString() == "0" ? "授權失敗" : gDRV["HRtnCode"].ToString() == "1" ? "授權成功" : "處理中";

        //隱藏敏感資訊(信用卡卡號)
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HCardNum")).Text))
        {
            ((Label)e.Item.FindControl("LB_HCardNum")).Text = HideSensitiveInfo(((Label)e.Item.FindControl("LB_HCardNum")).Text.Replace("-", ""), 6, 4, true);
        }

        //AA20241126_新增判斷:當授權成功才能進行部分付款與補登作業
        if (gDRV["HRtnCode"].ToString() == "1")
        {
            ((LinkButton)e.Item.FindControl("LBtn_Partial")).Enabled = true;
        }
        else
        {
            ((LinkButton)e.Item.FindControl("LBtn_Partial")).Enabled = false;
            ((LinkButton)e.Item.FindControl("LBtn_Partial")).CssClass = "btn btn-gray";
        }
    }
    #endregion

    #region 隱藏敏感信息
    //// <summary>
    /// 隱藏敏感信息
    /// </summary>
    /// <param name="info">信息實體</param>
    /// <param name="left">左邊保留的字符數</param>
    /// <param name="right">右邊保留的字符數</param>
    /// <param name="basedOnLeft">當長度異常時，是否顯示左邊 
    /// <code>true</code>顯示左邊，<code>false</code>顯示右邊
    /// </param>
    /// <returns></returns>
    public static string HideSensitiveInfo(string info, int left, int right, bool basedOnLeft = true)
    {
        if (String.IsNullOrEmpty(info))
        {
            return "";
        }
        StringBuilder sbText = new StringBuilder();
        int hiddenCharCount = info.Length - left - right;
        if (hiddenCharCount > 0)
        {
            string prefix = info.Substring(0, left), suffix = info.Substring(info.Length - right);
            sbText.Append(prefix);
            for (int i = 0; i < hiddenCharCount; i++)
            {
                sbText.Append("*");
            }
            sbText.Append(suffix);
        }
        else
        {
            if (basedOnLeft)
            {
                if (info.Length > left && left > 0)
                {
                    sbText.Append(info.Substring(0, left) + "****");
                }
                else
                {
                    sbText.Append(info.Substring(0, 1) + "****");
                }
            }
            else
            {
                if (info.Length > right && right > 0)
                {
                    sbText.Append("****" + info.Substring(info.Length - right));
                }
                else
                {
                    sbText.Append("****" + info.Substring(info.Length - 1));
                }
            }
        }
        return sbText.ToString();
    }


    #endregion

    #region 匯出Excel (.csv)
    protected void LBtn_Export_Click(object sender, EventArgs e)
    {
        //刪除檔案--未完成 (先清空資料夾)
        //FileInfo OldFile = new FileInfo(@"D:\Website\System\HochiSystem\App_Data\DownloadCSV\");
        //OldFile.Delete();

        //判斷是否已按匯出資料，避免重複更新
        SqlDataReader QueryCCPeriodOrder = SQLdatabase.ExecuteReader("SELECT HMerchantTradeNo, HRtnCode FROM HCCPOrderDetail WHERE HCCPOrderID='" + LB_HID.Text + "' AND HRtnCode IS NOT NULL AND HPaperYN='1' ");

        //Response.Write("SELECT HMerchantTradeNo, HRtnCode FROM HCCPOrderDetail WHERE HCCPOrderID='" + LB_HID.Text + "' AND HRtnCode <>'2'");
        //Response.End();

        if (QueryCCPeriodOrder.Read())
        {
            Response.Write("<script>alert('已匯出過Excel囉~請勿重複操作，謝謝~');</script>");
            return;
        }
        else
        {
            //string QueryCCPeriod = "SELECT b.HCCPeriodCode,'0' AS HPeriod, b.HDCCPAmount,  b.HCardHolder,  Replace(b.HCardNum, '-', '') AS HCardNum, b.HCardValidDate,   b.HCVCCode, '1' AS HFrequency, b.HDCCPTimes,  b.HCHPhone FROM HCCPOrder AS a LEFT JOIN HCCPOrderDetail AS b ON a.HID = b.HCCPOrderID WHERE a.HCCPOrderCode = '" + TB_HCCPOrderCode.Text.Trim() + "'";
            //SqlCommand cmd = new SqlCommand("SELECT  b.HCCPeriodCode,'0' AS HPeriod, b.HDCCPAmount,  b.HCardHolder,  Replace(b.HCardNum,'-','') AS HCardNum,  b.HCardValidDate,   b.HCVCCode, '1' AS HFrequency,  b.HDCCPTimes,  b.HCHPhone FROM HCCPOrder AS a LEFT JOIN HCCPOrderDetail AS b ON a.HID=b.HCCPOrderID WHERE  a.HCCPOrderCode='" + TB_HCCPOrderCode.Text.Trim() + "'", con);

            //EE20230814_欄位名稱
            //SqlCommand cmd = new SqlCommand("SELECT  b.HCCPeriodCode AS '刷卡項目(必填)','0' AS '分期期數(必填)', b.HDCCPAmount AS '刷卡金額(必填)',  b.HCardHolder AS '持卡人姓名',  Replace(b.HCardNum,'-','') AS '信用卡卡號(必填)',  b.HCardValidDate AS '有效年月(必填。例:06/18)',   b.HCVCCode AS '信用卡背面末3碼', '1'  AS '定期定額「月」扣款頻率(例: 每3個月扣1次，請填3)',  b.HDCCPTimes AS '定期定額扣款期數(例: 總共要扣10次，請填10)',  b.HCHPhone AS '手機號碼' FROM HCCPOrder AS a LEFT JOIN HCCPOrderDetail AS b ON a.HID=b.HCCPOrderID WHERE  a.HCCPOrderCode='" + TB_HCCPOrderCode.Text.Trim() + "'", con);

            //AE20230824_不帶入手機號碼(避免匯入綠界後失敗)
            SqlCommand cmd = new SqlCommand("SELECT  b.HCCPeriodCode AS '刷卡項目(必填)','0' AS '分期期數(必填)', b.HDCCPAmount AS '刷卡金額(必填)',  b.HCardHolder AS '持卡人姓名',  Replace(b.HCardNum,'-','') AS '信用卡卡號(必填)',  b.HCardValidDate AS '有效年月(必填。例:06/18)',   b.HCVCCode AS '信用卡背面末3碼', '1'  AS '定期定額「月」扣款頻率(例: 每3個月扣1次，請填3)',  b.HDCCPTimes AS '定期定額扣款期數(例: 總共要扣10次，請填10)', '' AS '手機號碼' FROM HCCPOrder AS a LEFT JOIN HCCPOrderDetail AS b ON a.HID=b.HCCPOrderID WHERE  a.HCCPOrderCode='" + TB_HCCPOrderCode.Text.Trim() + "' AND a.HPaperYN='1' ", con);

            DataTable dt = new DataTable();
            SqlDataReader Dr;
            con.Open();
            Dr = cmd.ExecuteReader();

            dt.Load(Dr);


            //GridView1.DataSource = dt;
            //GridView1.DataBind();

            //先更新授權狀態為處理中 (授權狀態(HRtnCode)：0=授權失敗、1=授權成功、2=處理中)
            SQLdatabase.ExecuteNonQuery("UPDATE HCCPOrderDetail SET HRtnCode ='2',  HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPOrderID ='" + LB_HID.Text + "'");

            con.Close();

            string filePath = @"D:\Website\System\HochiSystem\App_Data\DownloadCSV\";
            //儲存CSV
            SaveCSV(dt, filePath + TB_HCCPOrderCode.Text.Trim() + "_" + DateTime.Now.ToString("yyMMdd") + ".csv");


            //設定要下載的檔案路徑及儲存的檔名
            //string path = "D://Website/ImageWebsite/NifcoCustom/" + LB_3DIMG.Text;
            string path = Server.MapPath("~/App_Data/DownloadCSV/") + TB_HCCPOrderCode.Text.Trim() + "_" + DateTime.Now.ToString("yyMMdd") + ".csv";
            string FileName = TB_HCCPOrderCode.Text.Trim() + "_" + DateTime.Now.ToString("yyMMdd") + ".csv";
            //宣告並建立WebClient物件
            WebClient wc = new WebClient();
            //載入要下載的檔案
            byte[] b = wc.DownloadData(path);

            //清除Response內的HTML
            Response.Clear();
            //設定標頭檔資訊 attachment 是本文章的關鍵字
            Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
            //開始輸出讀取到的檔案
            Response.BinaryWrite(b);
            //一定要加入這一行，否則會持續把Web內的HTML文字也輸出。
            Response.End();



        }
        QueryCCPeriodOrder.Close();





    }
    #endregion

    /// <summary>
    /// 將DataTable轉換成CSV文件
    /// </summary>
    /// <param name="dt">DataTable</param>
    /// <param name="filePath">文件路徑</param>
    public static void SaveCSV(DataTable dt, string filePath)
    {
        FileStream fs = new FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
        string data = "";

        //寫出列名稱
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            data += dt.Columns[i].ColumnName.ToString();
            if (i < dt.Columns.Count - 1)
            {
                data += ",";
            }
        }
        sw.WriteLine(data);

        //寫出各行數據
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            data = "";
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                data += dt.Rows[i][j].ToString();
                if (j < dt.Columns.Count - 1)
                {
                    data += ",\t";  //EE20230814_文字格式
                }
            }
            sw.WriteLine(data);
        }
        sw.Close();
        fs.Close();




    }

    #region 匯入Excel (.csv)-開啟Modal
    protected void LBtn_Import_Click(object sender, EventArgs e)
    {
        //開啟Modal
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_UploadFile').modal('show');</script>", false);

        //判斷授權狀態(HRtnCode)：0=授權失敗、1=授權成功、2=處理中
        //判斷是否已匯入過資料，避免重複更新
        //SqlDataReader QueryCCPeriodOrder = SQLdatabase.ExecuteReader("SELECT HMerchantTradeNo, HRtnCode FROM HCCPOrderDetail WHERE HCCPOrderID='" + LB_HID.Text + "' AND HRtnCode !='2'");

        //if (QueryCCPeriodOrder.Read())
        //{
        //    Response.Write("<script>alert('信用卡授權訂單已匯入完成囉~請勿重複操作，謝謝~');</script>");
        //    return;
        //}
        //else
        //{
        //    //開啟Modal
        //    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_UploadFile').modal('show');</script>", false);
        //}
        //QueryCCPeriodOrder.Close();


    }
    #endregion

    #region 內頁儲存功能
    protected void Btn_Submit_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(TB_FHCCPImportNum.Text.Trim()))
        {
            Response.Write("<script>alert('已有匯入編號囉~!');</script>");
            return;
        }

        if (!string.IsNullOrEmpty(TB_HCCPImportNum.Text.Trim()))
        {
            SQLdatabase.ExecuteNonQuery("UPDATE HCCPOrder SET HCCPImportNum='" + TB_HCCPImportNum.Text.Trim() + "', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPOrderCode='" + TB_HCCPOrderCode.Text.Trim() + "'");

            TB_FHCCPImportNum.Text = TB_HCCPImportNum.Text.Trim();
            TB_HCCPImportNum.Text = null;

            Response.Write("<script>alert('匯入編號儲存成功~!');</script>");
        }
        else
        {
            Response.Write("<script>alert('請先輸入匯入編號~!');</script>");
            return;

        }

    }
    #endregion

    #region 內頁取消功能
    protected void Btn_Cancel_Click(object sender, EventArgs e)
    {
        TB_HCCPImportNum.Text = null;
        //Response.Redirect("HCCPeriodOrder_Edit.aspx");
    }
    #endregion

    #region 匯入Excel取消功能
    protected void Btn_ImportCancel_Click(object sender, EventArgs e)
    {

    }
    #endregion

    #region 匯入Excel (.csv)功能
    protected void Btn_Import_Click(object sender, EventArgs e)
    {


        string gHCCPeriodCode, gHMerchantTradeNo;
        int InsertALLCount = 1; //匯入總筆數(excel第一欄為標題，所以 + 1)
        string Path = Page.Server.MapPath("~/uploads/CSVFile/");
        bool FileUp = false;
        string fileExtension;
        string FileName;
        int i;

        if (FU_CCPeriodOrder.HasFile)
        {
            fileExtension = System.IO.Path.GetExtension(FU_CCPeriodOrder.FileName).ToLower();   // 取得檔案格式   
            string[] allowedExtensions = new[] { ".xls", ".xlsx", ".csv" };   // 定義允許的檔案格式   
            for (i = 0; i <= allowedExtensions.Length - 1; i++)   // 逐一檢查允許的格式中是否有上傳的格式   
            {
                if (fileExtension == allowedExtensions[i])
                    FileUp = true;
            }

            // 將原來的檔名更改。
            FileName = System.IO.Path.GetFileName(FU_CCPeriodOrder.FileName);
            int index;
            index = FileName.LastIndexOf(".");
            string lastname;
            lastname = FileName.Substring(index, FileName.Length - index);
            string newname;
            newname = "HCCP" + TB_HCCPOrderCode.Text.Trim() + lastname;


            DataTable dataTable_CCPeriod = null;

            if (FileUp == true)
            {
                FU_CCPeriodOrder.PostedFile.SaveAs(Path + newname);  // 將上傳的檔案儲存
                string serverPath = Server.MapPath("~/uploads/CSVFile/" + newname);

                // 讀取csv檔寫入資料庫
                dataTable_CCPeriod = NPOIHelperCSV.ReadCsvAsTableNPOI(Server.MapPath("~/uploads/CSVFile/" + newname));

                //GridView1.DataSource = dataTable_CCPeriod;
                //GridView1.DataBind();
                ////Response.End();
                //Response.Write("Count=" + dataTable_CCPeriod.Rows.Count+"<br/>");

                for (int x = 0; x < dataTable_CCPeriod.Rows.Count; x++)
                {
                    gHCCPeriodCode = dataTable_CCPeriod.Rows[x]["交易描述"].ToString().Replace("\"", " ").Trim();
                    gHMerchantTradeNo = dataTable_CCPeriod.Rows[x]["廠商訂單編號"].ToString().Replace("\"", " ").Trim();

                    //Response.Write("gHCCPeriodCode=" + gHCCPeriodCode + "，gHMerchantTradeNo=" + gHMerchantTradeNo + "<br/><br/>");
                    //Response.End();

                    if (!string.IsNullOrEmpty(gHCCPeriodCode) && !string.IsNullOrEmpty(gHMerchantTradeNo))
                    {
                        //更新HCCPeriodCode 開始
                        SqlCommand dbCmd = default(SqlCommand);
                        con.Open();


                        //Response.Write("UPDATE HCCPOrderDetail SET HMerchantTradeNo='" + gHMerchantTradeNo + "', HRtnCode='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPeriodCode='" + gHCCPeriodCode + "' AND HCCPOrderID='" + LB_HID.Text + "'" );

                        //string UpdSQLHQ = "UPDATE HCCPOrderDetail SET HMerchantTradeNo='" + gHMerchantTradeNo + "', HRtnCode='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPeriodCode='" + gHCCPeriodCode + "' AND HRtnCode='2' AND HCCPOrderID='" + LB_HID.Text + "'";

                        //AE230823_更改為不管授權狀態，可以重新更新授權狀態
                        string UpdSQLHQ = "UPDATE HCCPOrderDetail SET HMerchantTradeNo='" + gHMerchantTradeNo + "', HRtnCode='1', HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPeriodCode='" + gHCCPeriodCode + "' AND HCCPOrderID='" + LB_HID.Text + "'";
                        dbCmd = new SqlCommand(UpdSQLHQ, con);
                        dbCmd.ExecuteNonQuery();

                        con.Close();
                        //更新HCCPeriodCode 結束
                    }


                }

                //更新HRtnCode='0' (授權失敗)
                //AE20250617_改用IN的寫法 因為有可能會不只一筆失敗
                SQLdatabase.ExecuteNonQuery("UPDATE HCCPOrderDetail SET HRtnCode ='0',  HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HCCPeriodCode IN(SELECT HCCPeriodCode FROM HCCPOrderDetail WHERE (HMerchantTradeNo='' OR HMerchantTradeNo IS NULL) AND HRtnCode ='2' AND HCCPOrderID ='" + LB_HID.Text + "'  )");

                Response.Write("<script>alert('綠界廠商訂單編號匯入成功~!');</script>");

                SDS_HCCPOrderDetail.SelectCommand = "SELECT b.HID, a.HCCPOrderCode, a.HCCPImportNum, b.HCCPeriodCode,  b.HMerchantTradeNo,b.HCourseName, b.HDUserName,  b.HDonor,  b.HCardHolder,  b.HCardNum,  b.HCardBank,  b.HCVCCode,  b.HCardValidDate, b.HDTotal, b.HDCCPTimes, b.HDCCPAmount, b.HCHPhone, b.HRtnCode FROM HCCPOrder AS a LEFT JOIN HCCPOrderDetail AS b ON a.HID=b.HCCPOrderID WHERE a.HCCPOrderCode='" + TB_HCCPOrderCode.Text + "' AND a.HPaperYN='1' ";
                Rpt_HCCPOrderDetail.DataBind();
                //Response.End();
            }
        }
    }
    #endregion

    #region 查看授權書內容功能
    protected void LBtn_Review_Click(object sender, EventArgs e)
    {
        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        //開啟Modal
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_HCCPeriod').modal('show');</script>", false);

        SqlDataReader QueryCCPeriod = SQLdatabase.ExecuteReader("SELECT HCCPeriodCode, HMemberID, HCTemplateID, HCourseID, HCourseName, HDateRange, HDUserName, HDEmail, HDPostal, HDAddress, HDTel, HDPhone, HDBirth, HDPersonID, HDReceiptSType, HDonor, HCardHolder,HCHPersonID, HCardNum, HCardBank, HCardType, HCVCCode, HCardValidDate, HDTotal, HDCCPTimes, HDCCPAmount,HDCCPSDate, HDCCPEDate, HCHPhone, HDAPublic, HVerifyStatus, HOrderStatus, HStatus FROM HCCPeriod WHERE HCCPeriodCode='" + ((Label)RI.FindControl("LB_HCCPeriodCode")).Text + "'");

        if (QueryCCPeriod.Read())
        {
            LB_HTitle.Text = QueryCCPeriod["HCourseName"].ToString();
            LB_MHCCPeriodCode.Text = QueryCCPeriod["HCCPeriodCode"].ToString();
            TB_HDUserName.Text = QueryCCPeriod["HDUserName"].ToString();
            TB_HDEmail.Text = QueryCCPeriod["HDEmail"].ToString();
            TB_HDTel.Text = QueryCCPeriod["HDTel"].ToString();
            TB_HDPhone.Text = QueryCCPeriod["HDPhone"].ToString();
            TB_HDBirth.Text = QueryCCPeriod["HDBirth"].ToString();
            TB_HDPersonID.Text = QueryCCPeriod["HDPersonID"].ToString();
            TB_HDPostal.Text = QueryCCPeriod["HDPostal"].ToString();
            TB_HDAddress.Text = QueryCCPeriod["HDAddress"].ToString();
            RBL_HDReceiptSType.SelectedValue = !string.IsNullOrEmpty(QueryCCPeriod["HDReceiptSType"].ToString()) ? QueryCCPeriod["HDReceiptSType"].ToString() : "1";

            //EA20230815_必填顯示
            if (RBL_HDReceiptSType.SelectedValue == "2")
            {
                Span_HDPersonID.Visible = true;
            }
            else
            {
                Span_HDPersonID.Visible = false;
            }

            TB_HDonor.Text = QueryCCPeriod["HDonor"].ToString();
            TB_HCardHolder.Text = QueryCCPeriod["HCardHolder"].ToString();
            TB_HCHPersonID.Text = QueryCCPeriod["HCHPersonID"].ToString();
            TB_HCHPhone.Text = QueryCCPeriod["HCHPhone"].ToString();
            TB_HCardBank.Text = QueryCCPeriod["HCardBank"].ToString();
            TB_HCardNum1.Text = QueryCCPeriod["HCardNum"].ToString().Split('-')[0].ToString();
            TB_HCardNum2.Text = QueryCCPeriod["HCardNum"].ToString().Split('-')[1].ToString();
            TB_HCardNum3.Text = QueryCCPeriod["HCardNum"].ToString().Split('-')[2].ToString();
            TB_HCardNum4.Text = QueryCCPeriod["HCardNum"].ToString().Split('-')[3].ToString();
            TB_HCVCCode.Text = QueryCCPeriod["HCVCCode"].ToString();
            TB_HCardValidDateM.Text = QueryCCPeriod["HCardValidDate"].ToString().Split('/')[0].ToString();
            TB_HCardValidDateY.Text = QueryCCPeriod["HCardValidDate"].ToString().Split('/')[1].ToString();
            TB_HDTotal.Text = QueryCCPeriod["HDTotal"].ToString();
            TB_HDCCPTimes.Text = QueryCCPeriod["HDCCPTimes"].ToString();
            TB_HDCCPAmount.Text = QueryCCPeriod["HDCCPAmount"].ToString();
            TB_HDCCPSDate.Text = QueryCCPeriod["HDCCPSDate"].ToString();
            TB_HDCCPEDate.Text = QueryCCPeriod["HDCCPEDate"].ToString();
            CB_HDAPublic.Checked = !string.IsNullOrEmpty(QueryCCPeriod["HDAPublic"].ToString()) && QueryCCPeriod["HDAPublic"].ToString() == "1" ? true : false;
            RBL_HCardType.SelectedValue = QueryCCPeriod["HCardType"].ToString();


            //授權訂單狀態(HOrderStatus)：1=未成立、2=已成立
            LB_MHOrderStatus.CssClass = QueryCCPeriod["HOrderStatus"].ToString() == "1" ? "text-gray" : QueryCCPeriod["HOrderStatus"].ToString() == "2" ? "text-success border-0" : "text-gray border-0";
            LB_MHOrderStatus.Text = QueryCCPeriod["HOrderStatus"].ToString() == "1" ? "未成立" : QueryCCPeriod["HOrderStatus"].ToString() == "2" ? "已成立" : "未成立";

            //授權書狀態(HStatus)：0=停用、1=有效、2=作廢
            LB_MHStatus.CssClass = QueryCCPeriod["HStatus"].ToString() == "1" ? "text-success" : QueryCCPeriod["HStatus"].ToString() == "2" ? "text-danger" : "text-danger ";

            //EE20240119_加入停用
            LB_MHStatus.Text = QueryCCPeriod["HStatus"].ToString() == "1" ? "有效" : QueryCCPeriod["HStatus"].ToString() == "2" ? "作廢" : "停用";


            //審核狀態(HVerifyStatus)：0=請選擇、1=未送審、2=送審中、3=審核通過、4=審核不通過
            LB_MHVerifyStatus.Text = QueryCCPeriod["HVerifyStatus"].ToString() == "1" ? "未送審" : QueryCCPeriod["HVerifyStatus"].ToString() == "2" ? "送審中" : QueryCCPeriod["HVerifyStatus"].ToString() == "3" ? "審核通過" : QueryCCPeriod["HVerifyStatus"].ToString() == "4" ? "審核不通過" : "";

            LB_MHVerifyStatus.CssClass = QueryCCPeriod["HVerifyStatus"].ToString() == "1" ? "text-info " : QueryCCPeriod["HVerifyStatus"].ToString() == "2" ? "text-info" : QueryCCPeriod["HVerifyStatus"].ToString() == "3" ? "text-success" : QueryCCPeriod["HVerifyStatus"].ToString() == "4" ? "text-danger" : "";



        }
        QueryCCPeriod.Close();


    }
    #endregion

    #region 資料繫結
    protected void Rpt_HCCPOrder_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        //判斷授權狀態(HRtnCode)：0=授權失敗、1=授權成功、2=處理中
        SqlDataReader QueryNum = SQLdatabase.ExecuteReader("SELECT b.HID, b.HRtnCode FROM HCCPOrder AS a LEFT JOIN HCCPOrderDetail AS b ON a.HID= b.HCCPOrderID  WHERE HCCPOrderCode='" + gDRV["HCCPOrderCode"].ToString() + "'");

        int Total = 0;
        int successed = 0;
        int failed = 0;
        int processed = 0;

        while (QueryNum.Read())
        {
            Total += 1;

            if (!string.IsNullOrEmpty(QueryNum["HRtnCode"].ToString()))
            {
                if (QueryNum["HRtnCode"].ToString() == "0")
                {
                    failed += 1;
                }
                else if (QueryNum["HRtnCode"].ToString() == "1")
                {
                    successed += 1;
                }
                else if (QueryNum["HRtnCode"].ToString() == "2")
                {
                    processed += 1;
                }
            }
        }
        QueryNum.Close();

        ((Label)e.Item.FindControl("LB_TotalNum")).Text = Total.ToString();
        ((Label)e.Item.FindControl("LB_Successed")).Text = successed.ToString();
        ((Label)e.Item.FindControl("LB_Failed")).Text = failed.ToString();
        ((Label)e.Item.FindControl("LB_Processed")).Text = processed.ToString();

        if (((Label)e.Item.FindControl("LB_Failed")).Text != "0")
        {
            ((Label)e.Item.FindControl("LB_Failed")).CssClass = "text-danger";
        }

        if (((Label)e.Item.FindControl("LB_Successed")).Text != "0")
        {
            ((Label)e.Item.FindControl("LB_Successed")).CssClass = "text-success";
        }
    }
    #endregion

    #region 回上一頁
    protected void Btn_Back_Click(object sender, EventArgs e)
    {
        Response.Write("<script>window.location.href='HCCPeriodOrder_Edit.aspx';</script>");
    }
    #endregion

    #region 補登
    protected void LBtn_Partial_Click(object sender, EventArgs e)
    {
        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        //開啟Modal
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_HCCOPaidRecord').modal('show');</script>", false);

        SqlDataReader QueryCCPeriod = SQLdatabase.ExecuteReader("SELECT HCCPeriodCode, HMemberID, HCTemplateID, HCourseID, HCourseName, HDateRange, HDUserName, HDEmail, HDPostal, HDAddress, HDTel, HDPhone, HDBirth, HDPersonID, HDReceiptSType, HDonor, HCardHolder,HCHPersonID, HCardNum, HCardBank, HCardType, HCVCCode, HCardValidDate, HDTotal, HDCCPTimes, HDCCPAmount,HDCCPSDate, HDCCPEDate, HCHPhone, HDAPublic, HVerifyStatus, HOrderStatus, HStatus FROM HCCPeriod WHERE HCCPeriodCode='" + ((Label)RI.FindControl("LB_HCCPeriodCode")).Text + "'");

        if (QueryCCPeriod.Read())
        {
            LB_RTitle.Text = QueryCCPeriod["HCourseName"].ToString();
            LB_RHCCPeriodCode.Text = QueryCCPeriod["HCCPeriodCode"].ToString();
            TB_RDUserName.Text = QueryCCPeriod["HDUserName"].ToString();
            TB_RDEmail.Text = QueryCCPeriod["HDEmail"].ToString();
            TB_RDTel.Text = QueryCCPeriod["HDTel"].ToString();
            TB_RDPhone.Text = QueryCCPeriod["HDPhone"].ToString();


            TB_RDTotal.Text = QueryCCPeriod["HDTotal"].ToString();
            TB_RDCCPTimes.Text = QueryCCPeriod["HDCCPTimes"].ToString();
            TB_RDCCPAmount.Text = QueryCCPeriod["HDCCPAmount"].ToString();
            TB_RDCCPSDate.Text = QueryCCPeriod["HDCCPSDate"].ToString();
            TB_RDCCPEDate.Text = QueryCCPeriod["HDCCPEDate"].ToString();


            //授權訂單狀態(HOrderStatus)：1=未成立、2=已成立
            LB_RHOrderStatus.CssClass = QueryCCPeriod["HOrderStatus"].ToString() == "1" ? "text-gray" : QueryCCPeriod["HOrderStatus"].ToString() == "2" ? "text-success border-0" : "text-gray border-0";
            LB_RHOrderStatus.Text = QueryCCPeriod["HOrderStatus"].ToString() == "1" ? "未成立" : QueryCCPeriod["HOrderStatus"].ToString() == "2" ? "已成立" : "未成立";

            //授權書狀態(HStatus)：0=停用、1=有效、2=作廢
            LB_RHStatus.CssClass = QueryCCPeriod["HStatus"].ToString() == "1" ? "text-success" : QueryCCPeriod["HStatus"].ToString() == "2" ? "text-danger" : "text-danger ";

            //EE20240119_加入停用
            LB_RHStatus.Text = QueryCCPeriod["HStatus"].ToString() == "1" ? "有效" : QueryCCPeriod["HStatus"].ToString() == "2" ? "作廢" : "停用";


            //審核狀態(HVerifyStatus)：0=請選擇、1=未送審、2=送審中、3=審核通過、4=審核不通過
            LB_RHVerifyStatus.Text = QueryCCPeriod["HVerifyStatus"].ToString() == "1" ? "未送審" : QueryCCPeriod["HVerifyStatus"].ToString() == "2" ? "送審中" : QueryCCPeriod["HVerifyStatus"].ToString() == "3" ? "審核通過" : QueryCCPeriod["HVerifyStatus"].ToString() == "4" ? "審核不通過" : "";

            LB_RHVerifyStatus.CssClass = QueryCCPeriod["HVerifyStatus"].ToString() == "1" ? "text-info " : QueryCCPeriod["HVerifyStatus"].ToString() == "2" ? "text-info" : QueryCCPeriod["HVerifyStatus"].ToString() == "3" ? "text-success" : QueryCCPeriod["HVerifyStatus"].ToString() == "4" ? "text-danger" : "";



        }
        QueryCCPeriod.Close();

        LB_HCCPOPaidRecordID.Text = ((Label)RI.FindControl("LB_HID")).Text;

        //扣款紀錄
        SDS_HCCPOTRecord.SelectCommand = "SELECT d.HCCPOrderCode,b.HCCPeriodCode, b.HCourseName, (c.HArea+'/'+c.HPeriod+' '+c.HUserName) AS MemberName,  b.HDCCPTimes, b.HDTotal, a.HMerchantTradeNo, a.HAmount, a.HRtnMsg, a.HRtnCode, a.HProcessDate FROM HCCPOTRecord AS a JOIN HCCPOrderDetail AS b ON a.HCCPODetailID=b.HID JOIN HCCPOrder AS d ON b.HCCPOrderID=d.HID LEFT JOIN MemberList AS c ON b.HMemberID =c.HID WHERE b.HCCPeriodCode='" + ((Label)RI.FindControl("LB_HCCPeriodCode")).Text + "'";
        Rpt_HCCPOTRecord.DataSourceID = "SDS_HCCPOTRecord";
        Rpt_HCCPOTRecord.DataBind();

        //已扣款金額&尚未扣款金額
        int PaidTotal = 0;
        int UnPaidTotal = 0;
        for (int x = 0; x < Rpt_HCCPOTRecord.Items.Count; x++)
        {
            PaidTotal += Convert.ToInt32(((Label)Rpt_HCCPOTRecord.Items[x].FindControl("LB_HAmount")).Text.Replace(",", ""));
        }
        UnPaidTotal = Convert.ToInt32(TB_RDTotal.Text.Replace(",", ""))- PaidTotal;

        LB_HPaidTotal.Text= PaidTotal.ToString("N0");
        LB_HUnPaidTotal.Text = UnPaidTotal.ToString("N0");

        //補登紀錄
        SDS_HCCPOPaidRecord.SelectCommand = "SELECT HID, HCCPODetailID, HPayAmount, HPaymentDate, HPayMethod, HPayStatus, HRemark FROM HCCPOPaidRecord WHERE HCCPODetailID='" + ((Label)RI.FindControl("LB_HID")).Text + "' AND HStatus=1 ORDER BY HPaymentDate DESC";
        Rpt_HCCPOPaidRecord.DataBind();

        int TotalAmount = 0;
        for (int x = 0; x < Rpt_HCCPOPaidRecord.Items.Count; x++)
        {
            TotalAmount += Convert.ToInt32(((Label)Rpt_HCCPOPaidRecord.Items[x].FindControl("LB_HPayAmount")).Text.Replace(",", ""));
        }

        LB_RecognizeTotal.Text = TotalAmount.ToString("N0");

    }
    #endregion

    #region 定期定額綠界扣款紀錄
    protected void Rpt_HCCPOTRecord_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        #region 金額轉成千分位
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HAmount")).Text))
        {
            ((Label)e.Item.FindControl("LB_HAmount")).Text = Convert.ToInt32(((Label)e.Item.FindControl("LB_HAmount")).Text).ToString("N0");
        }

        #endregion


        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HRtnMsg")).Text) && ((Label)e.Item.FindControl("LB_HRtnMsg")).Text.Contains("付款失敗") == true)
        {
            ((Label)e.Item.FindControl("LB_HRtnMsg")).Text = "付款失敗";
        }

        if (((Label)e.Item.FindControl("LB_HRtnCode")).Text != "1")
        {
            ((Label)e.Item.FindControl("LB_HRtnCode")).Visible = true;
        }
        else
        {
            ((Label)e.Item.FindControl("LB_HRtnCode")).Text = "-";
        }

    }
    #endregion


    #region 新增補登紀錄
    protected void Btn_PaidRecordAdd_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_HPaymentDate.Text.Trim()) && string.IsNullOrEmpty(TB_HPayAmount.Text.Trim()) && DDL_HPayMethod.SelectedValue == "0")
        {
            Response.Write("<script>alert('請填寫必填欄位(付款時間、付款金額、付款方式');$('#Div_HCCOPaidRecord').modal('show');</script>");
            //return;
        }
        else
        {
            //加入資料庫
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);
            SqlCommand cmd = new SqlCommand("INSERT INTO HCCPOPaidRecord (HCCPODetailID, HPayAmount, HPaymentDate, HPayMethod, HPayStatus, HRemark, HStatus, HCreate, HCreateDT) VALUES (@HCCPODetailID, @HPayAmount, @HPaymentDate, @HPayMethod, @HPayStatus, @HRemark, @HStatus, @HCreate, @HCreateDT)", con);

            cmd.Parameters.AddWithValue("@HCCPODetailID", LB_HCCPOPaidRecordID.Text); //0=捐款
            cmd.Parameters.AddWithValue("@HPayAmount", TB_HPayAmount.Text.Trim());//99=血脈轉輪
            cmd.Parameters.AddWithValue("@HPaymentDate", TB_HPaymentDate.Text.Trim());
            cmd.Parameters.AddWithValue("@HPayMethod", DDL_HPayMethod.SelectedValue);
            cmd.Parameters.AddWithValue("@HPayStatus", "1");
            cmd.Parameters.AddWithValue("@HRemark", TB_HRemark.Text.Trim());
            cmd.Parameters.AddWithValue("@HStatus", "1");
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Cancel();

            TB_HPayAmount.Text = null;
            TB_HPaymentDate.Text = null;
            DDL_HPayMethod.SelectedValue = "0";
            TB_HRemark.Text = null;

            SDS_HCCPOPaidRecord.SelectCommand = "SELECT HID, HCCPODetailID, HPayAmount, HPaymentDate, HPayMethod, HPayStatus, HRemark FROM HCCPOPaidRecord WHERE HCCPODetailID='" + LB_HCCPOPaidRecordID.Text + "'  AND HStatus=1 ORDER BY HPaymentDate DESC";
            Rpt_HCCPOPaidRecord.DataBind();
        }

        //開啟Modal
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_HCCOPaidRecord').modal('show');</script>", false);
    }
    #endregion

    #region 補登紀錄
    protected void Rpt_HCCPOPaidRecord_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HPayAmount")).Text))
        {
            ((Label)e.Item.FindControl("LB_HPayAmount")).Text = Convert.ToInt32(((Label)e.Item.FindControl("LB_HPayAmount")).Text).ToString("N0");
        }
    }
    #endregion

    #region 刪除補登資料
    protected void Btn_Del_Click(object sender, EventArgs e)
    {
        RepeaterItem RI = (sender as Button).NamingContainer as RepeaterItem;

        //刪除資料庫
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);
        SqlCommand cmd = new SqlCommand(" UPDATE HCCPOPaidRecord SET HStatus=0, HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' WHERE HID='" + ((Label)RI.FindControl("LB_HID")).Text + "'", con);

        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();
        cmd.Cancel();

        //開啟Modal
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_HCCOPaidRecord').modal('show');</script>", false);

        SDS_HCCPOPaidRecord.SelectCommand = "SELECT HID, HCCPODetailID, HPayAmount, HPaymentDate, HPayMethod, HPayStatus, HRemark FROM HCCPOPaidRecord WHERE HCCPODetailID='" + LB_HCCPOPaidRecordID.Text + "'  AND HStatus=1 ORDER BY HPaymentDate DESC";
        Rpt_HCCPOPaidRecord.DataBind();
    }
    #endregion
}