using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


public partial class HCourseTemplate_Add : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

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

    //刪除資料夾內所有檔案
    public void DeleteSrcFolder(string Path)
    {
        //去除資料夾和子檔案的只讀屬性
        //去除資料夾的只讀屬性
        //System.IO.DirectoryInfo fileInfo = new DirectoryInfo(Path);
        //fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
        //去除檔案的只讀屬性
        System.IO.File.SetAttributes(Path, System.IO.FileAttributes.Normal);
        //判斷資料夾是否還存在
        if (Directory.Exists(Path))
        {
            foreach (string f in Directory.GetFileSystemEntries(Path))
            {
                //刪除1天前的檔案
                if (File.Exists(f) && (File.GetLastWriteTime(f) < DateTime.Now.AddDays(-1)))
                {
                    //Response.Write("檔案寫入時間1："+File.GetLastWriteTime(f) +"<br/>");
                    //如果有子檔案刪除檔案
                    File.Delete(f);
                }
                else
                {
                    //迴圈遞迴刪除子資料夾 
                    DeleteSrcFolder1(f);
                }
            }
            //刪除空資料夾
            //Directory.Delete(Path);
        }
    }
    //迴圈遞迴刪除子資料夾
    public void DeleteSrcFolder1(string Path)
    {
        //去除資料夾和子檔案的只讀屬性
        //去除資料夾的只讀屬性
        //System.IO.DirectoryInfo fileInfo = new DirectoryInfo(Path);
        //fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
        //去除檔案的只讀屬性
        System.IO.File.SetAttributes(Path, System.IO.FileAttributes.Normal);
        //判斷資料夾是否還存在
        if (Directory.Exists(Path))
        {
            foreach (string f in Directory.GetFileSystemEntries(Path))
            {
                //刪除1天前的檔案
                if (File.Exists(f) && (File.GetLastWriteTime(f) < DateTime.Now.AddDays(-1)))
                {
                    //Response.Write("檔案寫入時間2：" + File.GetLastWriteTime(f) + "<br/>");
                    //如果有子檔案刪除檔案
                    File.Delete(f);
                }
                else
                {
                    //迴圈遞迴刪除子資料夾 
                    DeleteSrcFolder1(f);
                }
            }
            //刪除空資料夾
            //Directory.Delete(Path);
        }
    }

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        for (int i = 0; i < RPT_Tag.Items.Count; i++)
        {
            string currentHID = ((HiddenField)RPT_Tag.Items[i].FindControl("HF_HID")).Value;

            if (LB_NavTab.Text == currentHID)
            {
                ((LinkButton)RPT_Tag.Items[i].FindControl("LBtn_Tag")).CssClass = "nav-link active show";
            }
            else
            {
                ((LinkButton)RPT_Tag.Items[i].FindControl("LBtn_Tag")).CssClass = "nav-link";
            }

        }

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RegScript();//註冊js

        //ScriptManager.RegisterOnSubmitStatement(this, this.GetType(), "updatescript", "CKEDITOR.instances['ctl00_ContentPlaceHolder1_faqeditor'].updateElement();");
        //Page.ClientScript.RegisterOnSubmitStatement(CKE_HContent.GetType(),"editor","FCKeditorAPI.GetInstance('" + CKE_HContent.ClientID + "').UpdateLinkedField();");

        if (!IsPostBack)
        {
            #region Tab顯示

            RPT_Tag.DataSource = HCourse_NavTab.HCourseTemp_NavTab.NavTabs
    .Where(tab => tab.Visible)
    .Select(tab => new
    {
        HID = tab.ID,
        HName_TW = tab.Name
    }).ToList();

            RPT_Tag.DataBind();
            LB_NavTab.Text = "1";

            Panel_Template.Attributes.Add("class", "tab-pane fade active show");

            #endregion

            SqlConnection dbConn = default(SqlConnection);
            SqlCommand dbCmd = default(SqlCommand);
            string strDBConn = null;
            strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
            dbConn = new SqlConnection(strDBConn);
            dbConn.Open();

            //刪除1天前未存檔的資料
            string strDelHC_T = "delete from HCourse_T where HSave='0' and HCreateDT < '" + DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss") + "'";
            dbCmd = new SqlCommand(strDelHC_T, dbConn);
            dbCmd.ExecuteNonQuery();

            //string strDelHCM_T = "delete from HCourseMaterial_T where HCTemplateID not in (select HID from HCourse_T)";
            string strDelHCM_T = "delete from HCourseMaterial_T where HSave='0' and HCreateDT < '" + DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss") + "'";
            dbCmd = new SqlCommand(strDelHCM_T, dbConn);
            dbCmd.ExecuteNonQuery();

            //string strDelHLC_T = "delete from HLeadingCourse_T where HCTemplateID not in (select HID from HCourse_T)";
            string strDelHLC_T = "delete from HLeadingCourse_T where HSave='0' and HCreateDT < '" + DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss") + "'";
            dbCmd = new SqlCommand(strDelHLC_T, dbConn);
            dbCmd.ExecuteNonQuery();

            //string strDelHTL_T = "delete from HTodoList_T where HCTemplateID not in (select HID from HCourse_T)";
            string strDelHTL_T = "delete from HTodoList_T where HSave='0' and HCreateDT < '" + DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss") + "'";
            dbCmd = new SqlCommand(strDelHTL_T, dbConn);
            dbCmd.ExecuteNonQuery();

            //刪除教材temp檔
            string gHCMFileTemp = "~/uploads/CourseMaterialTemp/";
            DeleteSrcFolder(Server.MapPath(gHCMFileTemp));

            //EA20250408_講師教材
            string strDelHCTM_T = "delete from  HCourseTMaterial_T where HSave='0' and HCreateDT < '" + DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss") + "'";
            dbCmd = new SqlCommand(strDelHCTM_T, dbConn);
            dbCmd.ExecuteNonQuery();

            string strSelHQ = "select HID, HTitle, HStatus from HQuestion order by HTitle";
            dbCmd = new SqlCommand(strSelHQ, dbConn);
            SqlDataReader MyQueryHQ = dbCmd.ExecuteReader();
            while (MyQueryHQ.Read())
            {
                if (MyQueryHQ["HStatus"].ToString() == "0")
                {
                    LBox_HQuestionID.Items.Add(new ListItem("(停用)" + MyQueryHQ["HTitle"].ToString(), MyQueryHQ["HID"].ToString()));
                }
                else
                {
                    LBox_HQuestionID.Items.Add(new ListItem(MyQueryHQ["HTitle"].ToString(), MyQueryHQ["HID"].ToString()));
                }
            }
            MyQueryHQ.Close();

            //未完成--主班團隊
            //string strSelHM = "select HID, HUserName from HMember order by HUserName";
            string strSelHM = "select HMember.HID, HUserName,HAreaID, HPeriod, (HArea+'/'+HPeriod+' '+HUserName) as UserName from HMember Left Join HArea On HMember.HAreaID =HArea.HID WHERE HMember.HStatus=1 order by HUserName";
            //Response.Write(strSQL);
            dbCmd = new SqlCommand(strSelHM, dbConn);
            SqlDataReader MyQueryHM = dbCmd.ExecuteReader();
            while (MyQueryHM.Read())
            {
                LBox_HTeam.Items.Add(new ListItem(MyQueryHM["UserName"].ToString(), MyQueryHM["HID"].ToString()));
            }
            MyQueryHM.Close();


            dbConn.Close();
            dbCmd.Cancel();


            SqlDataReader QueryHCourse_Class = SQLdatabase.ExecuteReader("SELECT HID, HCourseName FROM HCourse_Class WHERE HStatus=1");
            while (QueryHCourse_Class.Read())
            {
                DDL_HType.Items.Add(new ListItem(QueryHCourse_Class["HCourseName"].ToString(), QueryHCourse_Class["HID"].ToString()));
            }
            QueryHCourse_Class.Close();

            //DDL_TDept.Items.Add(new ListItem("-請選擇-", "0"));
            SqlDataReader QueryHGroup = SQLdatabase.ExecuteReader("SELECT HID, HGroupName FROM HGroup WHERE HStatus='1'");
            while (QueryHGroup.Read())
            {
                DDL_HGroupName.Items.Add(new ListItem(QueryHGroup["HGroupName"].ToString(), (QueryHGroup["HID"].ToString())));
            }
            QueryHGroup.Close();

            SDS_HSystem.SelectCommand = "SELECT HID, HSystemName, HStatus FROM HSystem WHERE HStatus=1";
            SDS_HDharma.SelectCommand = "SELECT HID, HDTypeID, HDharmaName FROM HDharma WHERE HStatus='1'";
            SDS_HMType.SelectCommand = "SELECT HID, HMType FROM HMType WHERE HStatus=1 ORDER BY HID ASC";
            SDS_HBudgetType.SelectCommand = "SELECT HID, HContent, HStatus FROM HBudgetType WHERE HStatus=1";
            SDS_HExamContentName.SelectCommand = "SELECT HID, (HExamContentName+'-'+(CASE HExamType WHEN 1 THEN '筆試' WHEN 2 THEN '實作' WHEN '3' THEN '試教' ELSE '' END)) AS HExamContentName  FROM HExamContent WHERE HStatus=1";
            SDS_HExamSubject.SelectCommand = "SELECT HID, HExamSubjectName FROM HExamSubject WHERE HStatus=1";
            SDS_HTMaterial.SelectCommand = "SELECT a.HID, (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) AS HTMName, a.HTMaterial, a.HStatus FROM   HTeacherMaterial AS a LEFT JOIN HDocCTypeClass AS b ON a.HDocCTClassID=b.HID LEFT JOIN HCourseLevel AS c ON a.HCourseLevelID=c.HID LEFT JOIN HFileType AS d ON a.HFileTypeID=d.HID WHERE a.HStatus='1' AND (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) <>'' AND (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) IS NOT NULL ORDER BY a.HStatus DESC";

        }

    }

    #region 課程類別選單
    protected void DDL_HType_SelectedIndexChanged(object sender, EventArgs e)
    {
        SqlDataReader QueryHCourse_Class = SQLdatabase.ExecuteReader("SELECT HID, HType FROM HCourse_Class where HID='" + DDL_HType.SelectedValue + "'");
        if (QueryHCourse_Class.Read())
        {
            //RBL_TestCourse.SelectedValue = QueryHCourse_Class["HType"].ToString() == "2" ? "1" : "0";

            //EA20240421_加入檢覈科目名稱顯示判斷；檢覈課程即顯示
            if (QueryHCourse_Class["HType"].ToString() == "2")
            {
                RBL_TestCourse.SelectedValue = "1";
                Span_HExamSubject.Visible = true;
                DDL_HExamSubject.Enabled = true;
            }
            else
            {
                RBL_TestCourse.SelectedValue = "0";
                Span_HExamSubject.Visible = false;
                DDL_HExamSubject.Enabled = false;
                DDL_HExamSubject.Items.Clear();
                DDL_HExamSubject.Items.Add(new ListItem("-請選擇-", "0"));
                DDL_HExamSubject.DataBind();
            }

        }

        QueryHCourse_Class.Close();

        //20240420_



        //活動-繳費帳戶限定基金會 //221111-Amy 與絲嵐確認後不用限制活動只有500元
        //if (DDL_HType.SelectedValue == "13")
        //{
        //	DDL_HPMethod.SelectedValue = "1";
        //	DDL_HPMethod.Enabled = false;
        //	TB_HBCPoint.Enabled = false;
        //	//TB_HBCPoint.Text = "50";  //50點
        //	TB_HBCPoint.Text = "500";  //220819-前台顯示改成金額，故改為500元
        //}
        //else
        //{
        //	DDL_HPMethod.SelectedValue = "0";
        //	DDL_HPMethod.Enabled = true;
        //	TB_HBCPoint.Enabled = true;
        //	TB_HBCPoint.Text = null;
        //}



    }
    #endregion

    #region 學員課程教材
    protected void Rpt_HCourseMaterial_T_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ((FileUpload)e.Item.FindControl("FU_HCMaterial")).Visible = false;
        ((HyperLink)e.Item.FindControl("HL_HCMaterial")).Visible = true;
        ((HyperLink)e.Item.FindControl("HL_HCMaterial")).NavigateUrl = "";
        ((HyperLink)e.Item.FindControl("HL_HCMaterial")).Target = "_blank";

        ((TextBox)e.Item.FindControl("TB_HCMName")).Text = HttpUtility.HtmlDecode(((TextBox)e.Item.FindControl("TB_HCMName")).Text);
        ((TextBox)e.Item.FindControl("TB_HCMLink")).Text = HttpUtility.HtmlDecode(((TextBox)e.Item.FindControl("TB_HCMLink")).Text);
    }

    #region 學員課程教材-新增功能
    protected void LBtn_HCourseMaterial_T_add_Click(object sender, EventArgs e)
    {

        if (TB_HCMName.Text.Trim() == "")
        {
            //string RS_PeGN = (string)this.GetGlobalResourceObject("LangSource", "RS_PeGN");
            Response.Write("<script>alert('請輸入教材名稱');</script>");
            return;
        }

        //建立日期;系統時間
        //DateTime gDate = DateTime.Now;
        string gDay = DateTime.Now.Day.ToString();
        string gMonth = DateTime.Now.Month.ToString();
        string gYear = DateTime.Now.Year.ToString();
        string gDate = System.DateTime.Now.ToString("yyyy/MM/dd");

        string gHCMFileTemp = "~/uploads/CourseMaterialTemp/";
        //string gHCMFileRoot = "~/uploads/CourseMaterial/";
        string gHCMFileName = "";//主檔名
        string gHCMFileExtension = "";//副檔名

        bool fileIsValid = false;
        // Directory.CreateDirectory(gHCMFileRoot);
        if (FU_HCMaterial.HasFile)
        {
            //取得上傳檔案大小
            int fileSize = FU_HCMaterial.PostedFile.ContentLength;
            if (fileSize > 200 * 1024 * 1024) //200MB
            {
                //Response.Write("<script>alert('只能上傳mp4、wmv或avi檔哦~~!');</script>");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('單一教材上限為200MB哦!');", true);
                return;
            }
            else
            {
                //取得副檔名
                gHCMFileExtension = Path.GetExtension(FU_HCMaterial.FileName);

                String[] restrictExtension = { ".pdf", ".PDF", ".mp3", ".MP3" };
                foreach (string i in restrictExtension)
                {
                    if (gHCMFileExtension == i)
                    {
                        fileIsValid = true;
                        break;
                    }
                }

                if (fileIsValid)
                {

                    //檔名
                    //gHCMFileName = TB_HCMName.Text.Trim() + "_" + DateTime.Now.ToString("yyMMddssff") + gHCMFileExtension;
                    gHCMFileName = Path.GetFileNameWithoutExtension(FU_HCMaterial.FileName) + "_" + DateTime.Now.ToString("yyMMddssff") + gHCMFileExtension;
                    FU_HCMaterial.SaveAs(Server.MapPath(gHCMFileTemp) + gHCMFileName);
                    //FU_HCMaterial.SaveAs(Server.MapPath(gHCMFileTemp) + gHCMFileName);
                    //FU_HCMaterial.SaveAs(Server.MapPath(gHCMFileTemp) + FU_HCMaterial.FileName);
                }
                else
                {
                    Response.Write("<script>alert('只能上傳PDF或mp3檔哦~~!');</script>");
                    return;
                }
            }

        }
        else if (FU_HCMaterial.HasFile == false && string.IsNullOrEmpty(TB_HCMLink.Text))
        {
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert", "<script>alert('請先上傳教材喔!');</script>");
            Response.Write("<script>alert('請上傳教材或影片連結哦~!');</script>");
            return;
        }

        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        if (LB_HID.Text.Trim() == "")
        {
            SubmitFun("HCourseMaterial_T", "Insert");
        }


        //EE20241113_改為參數寫法&加入排序欄位
        dbCmd = new SqlCommand("INSERT INTO HCourseMaterial_T (HCTemplateID, HCMName, HCMaterial, HCMLink, HStatus, HCreate, HCreateDT, HSave, HSort) values (@HCTemplateID, @HCMName, @HCMaterial, @HCMLink, @HStatus, @HCreate, @HCreateDT, @HSave, @HSort)", dbConn);
        //dbConn.Open();
        dbCmd.Parameters.AddWithValue("@HCTemplateID", LB_HID.Text);
        dbCmd.Parameters.AddWithValue("@HCMName", HttpUtility.HtmlEncode(TB_HCMName.Text.Trim()));
        dbCmd.Parameters.AddWithValue("@HCMaterial", gHCMFileName);
        dbCmd.Parameters.AddWithValue("@HCMLink", HttpUtility.HtmlEncode(TB_HCMLink.Text.Trim()));
        dbCmd.Parameters.AddWithValue("@HStatus", "1");
        dbCmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        dbCmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        dbCmd.Parameters.AddWithValue("@HSave", "1");
        dbCmd.Parameters.AddWithValue("@HSort", TB_HSort.Text.Trim());

        dbCmd.ExecuteNonQuery();
        dbConn.Close();
        dbCmd.Cancel();

        //string strSelHCM_T = "Insert Into HCourseMaterial_T (HCTemplateID, HCMName, HCMaterial, HCMLink, HStatus, HCreate, HCreateDT, HSave) values ('" + LB_HID.Text + "', '" + HttpUtility.HtmlEncode(TB_HCMName.Text.Trim()) + "', '" + gHCMFileName + "', '" + HttpUtility.HtmlEncode(TB_HCMLink.Text.Trim()) + "', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "','0')";
        //dbCmd = new SqlCommand(strSelHCM_T, dbConn);
        //dbCmd.ExecuteReader();
        //dbConn.Close();
        //dbCmd.Cancel();


        //EE20241113_加入排序欄位；資料依排序顯示
        SDS_HCourseMaterial_T.SelectCommand = "SELECT HID, HCMName, HCMaterial, HCMLink, HSort FROM HCourseMaterial_T where HCTemplateID='" + LB_HID.Text + "' ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
        //SDS_HCourseMaterial_T.SelectCommand = "SELECT HID, HCMName, HCMaterial, HCMLink FROM HCourseMaterial_T where HCTemplateID='" + LB_HID.Text + "' ORDER BY case when HCMName <> '' then 0 else 1 end ASC";
        Rpt_HCourseMaterial_T.DataBind();

        //清除textbox
        TB_HCMName.Text = "";
        TB_HCMLink.Text = "";
        TB_HSort.Text = "";

    }
    #endregion

    #region 學員課程教材-刪除功能
    protected void LBtn_HCourseMaterial_T_Del_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Del = sender as LinkButton;
        string Del_CA = LBtn_Del.CommandArgument;

        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        //刪除1天前未存檔的資料
        string strDelHC_T = "delete from HCourseMaterial_T where HID='" + Del_CA + "'";
        dbCmd = new SqlCommand(strDelHC_T, dbConn);
        dbCmd.ExecuteNonQuery();

        dbConn.Close();
        dbCmd.Cancel();

        //EE20241113_加入排序欄位；資料依排序顯示
        SDS_HCourseMaterial_T.SelectCommand = "SELECT HID, HCMName, HCMaterial, HCMLink, HSort FROM HCourseMaterial_T where HCTemplateID='" + LB_HID.Text + "' ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
        //SDS_HCourseMaterial_T.SelectCommand = "SELECT HID, HCMName, HCMaterial, HCMLink FROM HCourseMaterial_T where HCTemplateID='" + LB_HID.Text + "' ORDER BY case when HCMName <> '' then 0 else 1 end ASC";
        Rpt_HCourseMaterial_T.DataBind();
    }
    #endregion

    //protected void LBtn_HCourseMaterial_T_Click(object sender, EventArgs e)
    //{
    //    //Panel_HCourseMaterial_T.Visible = (Panel_HCourseMaterial_T.Visible == false ? true : false);
    //    //Panel_HLeadingCourse_T.Visible = false;
    //    //Panel_HTodoList_T.Visible = false;
    //}
    //protected void LBtn_HLeadingCourse_T_Click(object sender, EventArgs e)
    //{
    //    //Panel_HCourseMaterial_T.Visible = false;
    //    //Panel_HLeadingCourse_T.Visible = (Panel_HLeadingCourse_T.Visible == false ? true : false);
    //   // Panel_HTodoList_T.Visible = false;
    //}
    //protected void LBtn_HTodoList_T_Click(object sender, EventArgs e)
    //{
    //   // Panel_HCourseMaterial_T.Visible = false;
    //    //Panel_HLeadingCourse_T.Visible = false;
    //    //Panel_HTodoList_T.Visible = (Panel_HTodoList_T.Visible == false ? true : false);
    //}

    #endregion

    #region 體系護持工作項目
    protected void Rpt_HTodoList_T_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //取得Repeater的DataRowView
        DataRowView gDRV = (DataRowView)e.Item.DataItem;
        SqlDataReader QueryHGroup = SQLdatabase.ExecuteReader("SELECT HID, HGroupName FROM HGroup WHERE HStatus='1'");
        while (QueryHGroup.Read())
        {
            ((DropDownList)e.Item.FindControl("DDL_HGroupName")).Items.Add(new ListItem(QueryHGroup["HGroupName"].ToString(), (QueryHGroup["HID"].ToString())));
        }
        QueryHGroup.Close();
        ((DropDownList)e.Item.FindControl("DDL_HGroupName")).SelectedValue = gDRV["HGroupName"].ToString();

        SqlDataReader QueryHCourseTask = SQLdatabase.ExecuteReader("SELECT HID, HTask, HTaskContent FROM HCourseTask where HGroupID='" + ((DropDownList)e.Item.FindControl("DDL_HGroupName")).SelectedValue + "'");
        while (QueryHCourseTask.Read())
        {
            ((DropDownList)e.Item.FindControl("DDL_HTask")).Items.Add(new ListItem(QueryHCourseTask["HTask"].ToString(), (QueryHCourseTask["HID"].ToString())));
        }
        QueryHCourseTask.Close();
        ((DropDownList)e.Item.FindControl("DDL_HTask")).SelectedValue = gDRV["HTask"].ToString();

        ((TextBox)e.Item.FindControl("TB_HTaskContent")).Text = HttpUtility.HtmlDecode(((TextBox)e.Item.FindControl("TB_HTaskContent")).Text);

        ((TextBox)e.Item.FindControl("TB_HTaskNum")).Text = gDRV["HTaskNum"].ToString();

    }

    #region 體系護持工作項目-新增功能
    protected void LBtn_HTodoList_T_add_Click(object sender, EventArgs e)
    {
        if (DDL_HGroupName.SelectedValue == "0")
        {
            Response.Write("<script>alert('請選擇所屬組別!');</script>");
            return;
        }





        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        if (LB_HID.Text.Trim() == "")
        {
            SubmitFun("HTodoList_T", "Insert");
        }

        //EE20240503_加入人數
        string strSelHTL_T = "insert into HTodoList_T (HCTemplateID, HGroupName, HTask, HTaskNum, HTaskContent, HGroupLeaderID,HStatus, HCreate, HCreateDT, HSave) values ('" + LB_HID.Text + "', '" + DDL_HGroupName.SelectedValue + "', '" + DDL_HTask.SelectedValue + "', '" + TB_HTaskNum.Text.Trim() + "', '" + HttpUtility.HtmlEncode(TB_HTaskContent.Text.Trim()) + "','0', '1', '" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '1')";
        dbCmd = new SqlCommand(strSelHTL_T, dbConn);
        dbCmd.ExecuteReader();

        dbConn.Close();
        dbCmd.Cancel();


        //清空資料
        DDL_HGroupName.SelectedValue = "0";
        DDL_HTask.SelectedValue = "0";
        TB_HTaskNum.Text = null;
        TB_HTaskContent.Text = null;

        SDS_HTodoList_T.SelectCommand = "SELECT HID, HGroupName, HTask, HTaskContent, HTaskNum FROM HTodoList_T where HCTemplateID='" + LB_HID.Text + "'";
        Rpt_HTodoList_T.DataBind();
    }
    #endregion

    #region 體系護持工作項目-刪除功能
    protected void LBtn_HTodoList_T_Del_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Del = sender as LinkButton;
        string Del_CA = LBtn_Del.CommandArgument;

        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        //刪除1天前未存檔的資料
        string strDelHC_T = "delete from HTodoList_T where HID='" + Del_CA + "'";
        dbCmd = new SqlCommand(strDelHC_T, dbConn);
        dbCmd.ExecuteNonQuery();

        dbConn.Close();
        dbCmd.Cancel();

        SDS_HTodoList_T.SelectCommand = "SELECT HID, HGroupName, HTask, HTaskContent, HTaskNum FROM HTodoList_T where HCTemplateID='" + LB_HID.Text + "'";
        Rpt_HTodoList_T.DataBind();
    }
    #endregion

    #region 所屬組別
    protected void DDL_HGroupName_SelectedIndexChanged(object sender, EventArgs e)
    {
        DDL_HTask.Items.Clear();
        DDL_HTask.Enabled = true;
        DDL_HTask.Items.Add(new ListItem("-請選擇-", "0"));
        SqlDataReader QueryHCourseTask = SQLdatabase.ExecuteReader("SELECT HID, HTask, HTaskContent FROM HCourseTask where HGroupID='" + DDL_HGroupName.SelectedValue + "'");
        while (QueryHCourseTask.Read())
        {
            DDL_HTask.Items.Add(new ListItem(QueryHCourseTask["HTask"].ToString(), (QueryHCourseTask["HID"].ToString())));
        }
        QueryHCourseTask.Close();
    }
    #endregion

    #region 任務職稱
    protected void DDL_HTask_SelectedIndexChanged(object sender, EventArgs e)
    {
        SqlDataReader QueryHCourseTask = SQLdatabase.ExecuteReader("SELECT HID, HTask, HTaskContent FROM HCourseTask where HID='" + DDL_HTask.SelectedValue + "'");
        while (QueryHCourseTask.Read())
        {
            TB_HTaskContent.Text = QueryHCourseTask["HTaskContent"].ToString();
        }
        QueryHCourseTask.Close();
    }
    #endregion

    #endregion

    #region 講師教材設定

    #region 講師教材-新增功能
    protected void LBtn_HTMaterial_add_Click(object sender, EventArgs e)
    {

        if (DDL_HTMaterial.SelectedValue == "0")
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('請選擇講師教材名稱!');", true);
            //Response.Write("<script>alert('請選擇講師教材名稱!');</script>");
            return;
        }

        if (LB_HID.Text.Trim() == "")
        {
            SubmitFun("HCourseTMaterial_T", "Insert");
        }

        #region 判斷是否重複

        SqlCommand read = new SqlCommand("SELECT HID FROM  HCourseTMaterial_T WHERE HCTemplateID=@HCTemplateID AND HTMaterialID=@HTMaterialID AND HStatus='1'", con);
        con.Open();
        read.Parameters.AddWithValue("@HCTemplateID", LB_HID.Text);
        read.Parameters.AddWithValue("@HTMaterialID", DDL_HTMaterial.SelectedValue);

        SqlDataReader MyEBF = read.ExecuteReader();

        if (MyEBF.Read())
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "alert", "alert('資料已重複囉!');", true);
            return;
        }
        MyEBF.Close();
        con.Close();

        #endregion


        SqlCommand cmd = new SqlCommand("INSERT INTO [HCourseTMaterial_T] (HCTemplateID, HTMaterialID, HSort, HTMaterial, HSave, HStatus, HCreate, HCreateDT)VALUES(@HCTemplateID, @HTMaterialID, @HSort, @HTMaterial, @HSave, @HStatus, @HCreate, @HCreateDT)", con);
        con.Open();
        cmd.Parameters.AddWithValue("@HCTemplateID", LB_HID.Text);
        cmd.Parameters.AddWithValue("@HTMaterialID", DDL_HTMaterial.SelectedValue);
        cmd.Parameters.AddWithValue("@HSort", TB_HTMaterialSort.Text.Trim());
        cmd.Parameters.AddWithValue("@HTMaterial", DDL_HTMaterial.SelectedItem.Text);
        cmd.Parameters.AddWithValue("@HStatus", "1");
        cmd.Parameters.AddWithValue("@HSave", "1");
        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        con.Close();
        cmd.Cancel();


        DDL_HTMaterial.SelectedValue = "0";
        TB_HTMaterialSort.Text = null;

        SDS_HTMaterial_T.SelectCommand = "SELECT HID, HCTemplateID, HTMaterialID, HSort FROM HCourseTMaterial_T WHERE HCTemplateID='" + LB_HID.Text + "' AND HStatus='1' ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
        Rpt_HTMaterial_T.DataBind();


    }
    #endregion

    protected void Rpt_HTMaterial_T_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            DataRowView gDRV = (DataRowView)e.Item.DataItem;
            DropDownList ddl = (DropDownList)e.Item.FindControl("DDL_HTMaterial");
            ddl.Items.Clear();
            using (SqlDataReader QueryHTMaterial = SQLdatabase.ExecuteReader(
            "SELECT a.HID, (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) AS HTMName, a.HTMaterial, a.HStatus " +
            "FROM HTeacherMaterial AS a " +
            "LEFT JOIN HDocCTypeClass AS b ON a.HDocCTClassID=b.HID " +
            "LEFT JOIN HCourseLevel AS c ON a.HCourseLevelID=c.HID " +
            "LEFT JOIN HFileType AS d ON a.HFileTypeID=d.HID " +
            "WHERE a.HStatus='1' AND (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) <>'' " +
            "AND (d.HFType+'-'+b.HDocCTClass+'-'+c.HCourseLevel+'-'+HTMName) IS NOT NULL " +
            "ORDER BY a.HStatus DESC"))
            {
                while (QueryHTMaterial.Read())
                {
                    ddl.Items.Add(new ListItem(
                        QueryHTMaterial["HTMName"].ToString(),
                        QueryHTMaterial["HID"].ToString()
                    ));
                }
            }

            // 設定預設選取值
            string selectedID = gDRV["HTMaterialID"] != DBNull.Value ? gDRV["HTMaterialID"].ToString() : null;
            if (!string.IsNullOrEmpty(selectedID) && ddl.Items.FindByValue(selectedID) != null)
            {
                ddl.SelectedValue = selectedID;
            }
            else
            {
                // 避免找不到值時出錯，可選擇加個預設項目：
                ddl.Items.Insert(0, new ListItem("-請選擇-", "0"));
                ddl.SelectedValue = "0";
            }
        }
    }

    #region 講師教材-刪除功能
    protected void LBtn_HTMaterial_Del_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Del = sender as LinkButton;
        string Del_CA = LBtn_Del.CommandArgument;

        SqlCommand cmd = new SqlCommand("DELETE FROM HCourseTMaterial_T WHERE HID =@HID", con);
        con.Open();
        cmd.Parameters.AddWithValue("@HID", Del_CA);
        cmd.ExecuteNonQuery();
        con.Close();
        cmd.Cancel();

        SDS_HTMaterial_T.SelectCommand = "SELECT HID, HCTemplateID, HTMaterialID, HSort FROM HCourseTMaterial_T WHERE HCTemplateID='" + LB_HID.Text + "' AND HStatus='1' ORDER BY CASE WHEN HSort IS NULL THEN 1 ELSE 0 END, HSort ASC";
        Rpt_HTMaterial_T.DataBind();
    }
    #endregion

    #endregion

    #region 儲存功能
    protected void Btn_Submit_Click(object sender, EventArgs e)
    {

        if (LB_HID.Text.Trim() == "")
        {
            SubmitFun("submit", "Insert");
        }
        else
        {
            SubmitFun("submit", "Update");
        }


    }
    #endregion

    #region 取消儲存功能
    protected void Btn_Cancel_Click(object sender, EventArgs e)
    {
        Response.Write("<script>window.location.href='HCourseTemplate_Edit.aspx';</script>");
    }
    #endregion

    #region Tab切換
    protected void LBtn_NavTab_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        LB_NavTab.Text = btn.TabIndex.ToString();

        Panel[] Tabstr = { Panel_Template, Panel_Content, Panel_Material, Panel_Jobs, Panel_Homework, Panel_HTMaterial };

        for (int k = 0; k < Tabstr.Length; k++)
        {
            Tabstr[k].Attributes.Add("class", "tab-pane fade");

            if (k == btn.TabIndex - 1)
            {
                Tabstr[k].Attributes.Add("class", "tab-pane fade active show");
            }

        }


    }

    #endregion

    #region 繳費帳戶選單
    protected void DDL_HPMethod_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DDL_HPMethod.SelectedValue == "1")
        {
            DDL_HBudgetType.Enabled = true;
            Required.Visible = true;

        }
        else
        {
            DDL_HBudgetType.Enabled = false;
            Required.Visible = false;
            DDL_HBudgetType.SelectedValue = "0";
        }
    }
    #endregion

    #region 執行js
    public void RegScript()
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$('.js-example-basic-single').select2({closeOnSelect: true,});"), true);//執行js

        //ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS2", ("$('.timepicker').timepicker({ timeFormat: 'HH:mm',template: false,showInputs: false,minuteStep: 5});"), true);     //執行日曆js

        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS3", ("$('.ListB_Multi').SumoSelect({ search: true, placeholder: '-請選擇-', csvDispCount: 5, });"), true);//執行js

        //ScriptManager.RegisterStartupScript(Page, this.GetType(), "Datepicker", ("$('.datepicker').datepicker({format: 'yyyy/mm/dd',todayHighlight: true,orientation: 'bottom auto',toggleActive: true,autoclose: true, });"), true);


    }
    #endregion

    #region 帶狀課程選單
    //EA20231030_課程報名截止日天數顯示判斷：帶狀1-課程結束日後N天；非帶狀0-課程開始日前N天
    protected void RBL_HSerial_SelectedIndexChanged(object sender, EventArgs e)
    {
        //0：非帶狀；1：帶狀
        if (RBL_HSerial.SelectedValue == "0")
        {
            LB_HCDeadlineDayTitle.Text = "課程開始日前";
            TB_HCDeadlineDay.Text = "7";
        }
        else
        {
            LB_HCDeadlineDayTitle.Text = "課程結束日後";
            TB_HCDeadlineDay.Text = "1";
        }
    }
    #endregion

    #region 是否為軸線課程
    protected void RBL_HAxisYN_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (RBL_HAxisYN.SelectedValue == "0")
        {
            DDL_HAxisClass.Enabled = false;
            Span_HAxisClass.Visible = false;
            DDL_HAxisClass.SelectedValue = "0";
            //DDL_HAxisClass.Items.Clear();
            //DDL_HAxisClass.Items.Add(new ListItem("-請選擇-", "0"));
            //DDL_HAxisClass.DataBind();
        }
        else if (RBL_HAxisYN.SelectedValue == "1")
        {
            DDL_HAxisClass.Enabled = true;
            Span_HAxisClass.Visible = true;
        }
    }
    #endregion

    #region 儲存Function
    private void SubmitFun(string From, string StatementType)
    {
        #region 必填判斷

        if (DDL_HType.SelectedValue == "0")
        {
            Response.Write("<script>alert('請選擇課程類別!');</script>");
            return;
        }

        if (string.IsNullOrEmpty(TB_HTemplateName.Text.Trim()))
        {
            Response.Write("<script>alert('請輸入課程範本名稱!');</script>");
            return;
        }

        //220819 加入必填判斷
        if (DDL_HPMethod.SelectedValue == "0")
        {
            Response.Write("<script>alert('請選擇繳費帳戶!');</script>");
            return;
        }
        else
        {
            if (DDL_HPMethod.SelectedValue == "1")
            {
                if (DDL_HBudgetType.SelectedValue == "0")
                {
                    Response.Write("<script>alert('請選擇預算類別!');</script>");
                    return;
                }
            }
        }

        //220819 加入必填判斷
        if (string.IsNullOrEmpty(TB_HBCPoint.Text.Trim()))
        {
            Response.Write("<script>alert('請輸入參與課程基本金額!');</script>");
            return;
        }

        //EA20240421_加入檢覈科目名稱；如為檢覈課程即為必填
        //EE20250502_拿掉條件值(檢覈科目名稱)&& DDL_HExamSubject.SelectedValue == "0"。新增檢覈內容名稱判斷
        if (RBL_TestCourse.SelectedValue == "1" && DDL_HExamContentID.SelectedValue == "0")
        {
            Response.Write("<script>alert('請選擇檢覈內容名稱!');</script>");
            //Response.Write("<script>alert('請選擇檢覈科目名稱!');</script>");
            return;
        }

        //EA20240421_加入是否為軸線；如為軸線課程時軸線類別即為必填
        if (RBL_HAxisYN.SelectedValue == "1" && DDL_HAxisClass.SelectedValue == "0")
        {
            Response.Write("<script>alert('請選擇軸線類別!');</script>");
            return;
        }

        #endregion

        #region 教材檔案
        int gCT = 1;

        //將上傳之教材暫存檔名改為正式檔名
        string gHCMFileTemp = "~/uploads/CourseMaterialTemp/";
        string gHCMFileRoot = "~/uploads/CourseMaterial/";
        string gHCMFileName = "";//主檔名
        //string gHCMFileExtension = "";//副檔名

        string strSelHCM_T = "select HCMaterial from HCourseMaterial_T where HCTemplateID = '" + LB_HID.Text + "' and HSave='1'";
        SqlDataReader MyQueryHCM_T = SQLdatabase.ExecuteReader(strSelHCM_T);

        while (MyQueryHCM_T.Read())
        {
            if (!string.IsNullOrEmpty(MyQueryHCM_T["HCMaterial"].ToString()))
            {
                //主檔名+副檔名
                gHCMFileName = MyQueryHCM_T["HCMaterial"].ToString();

                //取得檔名
                string[] Filename = gHCMFileName.Split('.');
                //string New_name = Filename[0].ToString() + "_" + DateTime.Now.ToString("yyMMdd") + "." + Filename[1].ToString();  //避免名稱重複 資料夾已有檔案會error
                //將上傳之教材從暫存區移動到正式區
                //System.IO.File.Move(Server.MapPath(gHCMFileTemp) + gHCMFileName, Server.MapPath(gHCMFileRoot) + New_name);
                if (System.IO.File.Exists(Server.MapPath(gHCMFileTemp) + gHCMFileName))
                {
                    System.IO.File.Move(Server.MapPath(gHCMFileTemp) + gHCMFileName, Server.MapPath(gHCMFileRoot) + gHCMFileName);
                }
            }
        }
        MyQueryHCM_T.Close();

        #endregion

        #region 取ListBox的值，使用ForEach方式
        string gLBox_HRSystem = "";
        foreach (ListItem LBoxHRSystem in LBox_HRSystem.Items)
        {
            if (LBoxHRSystem.Selected == true)
            {
                gLBox_HRSystem += LBoxHRSystem.Value + ",";
            }
        }

        string gLBox_HNRequirement = "";
        foreach (ListItem LBoxHNRequirement in LBox_HNRequirement.Items)
        {
            if (LBoxHNRequirement.Selected == true)
            {
                gLBox_HNRequirement += LBoxHNRequirement.Value + ",";
            }
        }

        string gLBox_HTeam = "";
        foreach (ListItem LBoxHTeam in LBox_HTeam.Items)
        {
            if (LBoxHTeam.Selected == true)
            {
                gLBox_HTeam += LBoxHTeam.Value + ",";
            }
        }

        string gLBox_HIRestriction = "";
        foreach (ListItem LBoxHIRestriction in LBox_HIRestriction.Items)
        {
            if (LBoxHIRestriction.Selected == true)
            {
                gLBox_HIRestriction += LBoxHIRestriction.Value + ",";
            }
        }

        string gLBox_HQuestionID = "";
        foreach (ListItem LBoxHQuestionID in LBox_HQuestionID.Items)
        {
            if (LBoxHQuestionID.Selected == true)
            {
                gLBox_HQuestionID += LBoxHQuestionID.Value + ",";
            }
        }

        string gLBox_HTMaterialID = "";
        //foreach (ListItem LBoxHTMaterialID in LBox_HTMaterialID.Items)
        //{
        //    if (LBoxHTMaterialID.Selected == true)
        //    {
        //        gLBox_HTMaterialID += LBoxHTMaterialID.Value + ",";
        //    }
        //}

        #endregion

        string HID;
        //220819-基本費用(元)存入DB要先轉換成點數概念(1元=10點)
        int BasicPrice = 0;
        //int BasicPrice = Convert.ToInt32(TB_HBCPoint.Text.Trim()) / 10;
        if (!string.IsNullOrEmpty(TB_HBCPoint.Text.Trim()))
        {
            BasicPrice = Convert.ToInt32(TB_HBCPoint.Text.Trim()) / 10;
        }

        //AE20250611_新增HBookByDateYN(是否開放單天報名)欄位
        string strUpdHC_T = "EXECUTE [sp_HCourseTemplate] @StatementType, @HID, @HTemplateName  ,@HType  ,@HOSystem  ,@HRSystem  ,@HNLCourse  ,@HNGuide  ,@HNFull  ,@HNRequirement  ,@HTeam  ,@HNCWSheet  ,@HNCWDay  ,@HQuestionID  ,@HPMethod  ,@HBCPoint  ,@HSGList  ,@HIRestriction  ,@HRemark  ,@HContentTitle  ,@HContent  ,@HStatus  ,@HCMID  ,@HCMDT  ,@HSave  ,@HSerial  ,@HBudget  ,@HBudgetType  ,@HLodging  ,@HTMaterialID  ,@HCDeadlineDay  ,@HAxisYN  ,@HAxisClass ,@HExamSubject, @HExamContentID, @HParticipantLimit, @HBookByDateYN, @HCCPeriodYN";
        SqlCommand cmd = new SqlCommand(strUpdHC_T, con);
        con.Open();
        cmd.Parameters.AddWithValue("@StatementType", StatementType);
        //cmd.Parameters.AddWithValue("@StatementType", "Update");
        cmd.Parameters.AddWithValue("@HID", LB_HID.Text);
        cmd.Parameters.AddWithValue("@HTemplateName", TB_HTemplateName.Text.Trim());
        cmd.Parameters.AddWithValue("@HType", DDL_HType.SelectedValue);
        cmd.Parameters.AddWithValue("@HOSystem", DDL_HOSystem.SelectedValue);
        cmd.Parameters.AddWithValue("@HRSystem", gLBox_HRSystem);
        cmd.Parameters.AddWithValue("@HNLCourse", RBL_HNLCourse.SelectedValue);
        cmd.Parameters.AddWithValue("@HNGuide", RBL_HNGuide.SelectedValue);
        cmd.Parameters.AddWithValue("@HNFull", RBL_HNFull.SelectedValue);
        cmd.Parameters.AddWithValue("@HNRequirement", gLBox_HNRequirement);
        cmd.Parameters.AddWithValue("@HTeam", gLBox_HTeam);
        cmd.Parameters.AddWithValue("@HNCWSheet", HttpUtility.HtmlEncode(TB_HNCWSheet.Text.Trim()));
        cmd.Parameters.AddWithValue("@HNCWDay", HttpUtility.HtmlEncode(TB_HNCWDay.Text.Trim()));
        cmd.Parameters.AddWithValue("@HQuestionID", gLBox_HQuestionID);
        cmd.Parameters.AddWithValue("@HPMethod", DDL_HPMethod.SelectedValue);
        cmd.Parameters.AddWithValue("@HBCPoint", BasicPrice.ToString());
        cmd.Parameters.AddWithValue("@HSGList", RBL_HSGList.SelectedValue);
        cmd.Parameters.AddWithValue("@HIRestriction", gLBox_HIRestriction);
        cmd.Parameters.AddWithValue("@HRemark", HttpUtility.HtmlEncode(TB_HRemark.Text.Trim()));
        cmd.Parameters.AddWithValue("@HContentTitle", HttpUtility.HtmlEncode(TB_HContentTitle.Text.Trim()));
        cmd.Parameters.AddWithValue("@HContent", HttpUtility.HtmlEncode(CKE_HContent.Text));
        cmd.Parameters.AddWithValue("@HStatus", "1");
        cmd.Parameters.AddWithValue("@HCMID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCMDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("@HSave", "1");
        cmd.Parameters.AddWithValue("@HSerial", RBL_HSerial.SelectedValue);
        cmd.Parameters.AddWithValue("@HBudget", RBL_HBudget.SelectedValue);
        cmd.Parameters.AddWithValue("@HBudgetType", DDL_HBudgetType.SelectedValue);
        cmd.Parameters.AddWithValue("@HLodging", RBL_HLodging.SelectedValue);
        cmd.Parameters.AddWithValue("@HTMaterialID", gLBox_HTMaterialID);
        cmd.Parameters.AddWithValue("@HCDeadlineDay", TB_HCDeadlineDay.Text.Trim());
        cmd.Parameters.AddWithValue("@HAxisYN", RBL_HAxisYN.SelectedValue);
        cmd.Parameters.AddWithValue("@HAxisClass", DDL_HAxisClass.SelectedValue);
        cmd.Parameters.AddWithValue("@HExamSubject", DDL_HExamSubject.SelectedValue);
        cmd.Parameters.AddWithValue("@HExamContentID", DDL_HExamContentID.SelectedValue);
        cmd.Parameters.AddWithValue("@HParticipantLimit", TB_HParticipantLimit.Text.Trim());
        cmd.Parameters.AddWithValue("@HBookByDateYN", RBL_HBookByDateYN.SelectedValue);
        cmd.Parameters.AddWithValue("@HCCPeriodYN", RBL_HCCPeriodYN.SelectedValue);

        if (StatementType == "Insert")
        {
            HID = cmd.ExecuteScalar().ToString();

            LB_HID.Text = HID;
        }
        else
        {
            cmd.ExecuteReader();
        }
        con.Close();
        cmd.Cancel();


        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();

        for (int i = 0; i < Rpt_HCourseMaterial_T.Items.Count; i++)
        {
            dbCmd = new SqlCommand("UPDATE HCourseMaterial_T SET HCTemplateID=@HCTemplateID, HCMName=@HCMName, HCMLink=@HCMLink, HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT, HSave=@HSave, HSort=@HSort WHERE HCTemplateID =@HCTemplateID AND HID=@HID", dbConn);
            //dbConn.Open();
            dbCmd.Parameters.AddWithValue("@HCTemplateID", LB_HID.Text);
            dbCmd.Parameters.AddWithValue("@HID", ((Label)Rpt_HCourseMaterial_T.Items[i].FindControl("LB_HID")).Text);
            dbCmd.Parameters.AddWithValue("@HCMName", ((TextBox)Rpt_HCourseMaterial_T.Items[i].FindControl("TB_HCMName")).Text.Trim());
            dbCmd.Parameters.AddWithValue("@HCMLink", ((TextBox)Rpt_HCourseMaterial_T.Items[i].FindControl("TB_HCMLink")).Text.Trim());
            dbCmd.Parameters.AddWithValue("@HStatus", "1");
            dbCmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            dbCmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            dbCmd.Parameters.AddWithValue("@HSave", "1");

            //EE20241113_改為參數寫法&加入排序欄位
            if (((TextBox)Rpt_HCourseMaterial_T.Items[i].FindControl("TB_HSort")).Text.Trim() != ((Label)Rpt_HCourseMaterial_T.Items[i].FindControl("LB_HSort")).Text.Trim())
            {
                dbCmd.Parameters.AddWithValue("@HSort", ((TextBox)Rpt_HCourseMaterial_T.Items[i].FindControl("TB_HSort")).Text.Trim());
            }
            else
            {
                dbCmd.Parameters.AddWithValue("@HSort", ((Label)Rpt_HCourseMaterial_T.Items[i].FindControl("LB_HSort")).Text.Trim());
            }
            dbCmd.ExecuteNonQuery();

            //string strUpdHCM_T = "update HCourseMaterial_T set  HCMName='" + ((TextBox)Rpt_HCourseMaterial_T.Items[i].FindControl("TB_HCMName")).Text + "', HCMLink='" + ((TextBox)Rpt_HCourseMaterial_T.Items[i].FindControl("TB_HCMLink")).Text + "', HStatus='1', HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HCreateDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1' where HCTemplateID = '" + LB_HID.Text + "' AND HID='" + ((Label)Rpt_HCourseMaterial_T.Items[i].FindControl("LB_HID")).Text + "'";
            //dbCmd = new SqlCommand(strUpdHCM_T, dbConn);
            //dbCmd.ExecuteReader();

        }

        string strUpdHLC_T = "update HLeadingCourse_T set HStatus='1', HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HCreateDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1' where HCTemplateID = '" + LB_HID.Text + "'";
        dbCmd = new SqlCommand(strUpdHLC_T, dbConn);
        dbCmd.ExecuteReader();

        string strUpdHTL_T = "update HTodoList_T set HStatus='1', HCreate='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "', HCreateDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', HSave='1' where HCTemplateID = '" + LB_HID.Text + "'";
        dbCmd = new SqlCommand(strUpdHTL_T, dbConn);
        dbCmd.ExecuteReader();

        //EA20250408_講師教材設定
        for (int i = 0; i < Rpt_HTMaterial_T.Items.Count; i++)
        {
            if (((TextBox)Rpt_HTMaterial_T.Items[i].FindControl("TB_HTMaterialSort")).Text.Trim() != ((Label)Rpt_HTMaterial_T.Items[i].FindControl("LB_HTMaterialSort")).Text.Trim())
            {
                //SQL開始
                dbCmd = new SqlCommand("UPDATE HCourseTMaterial_T SET HSort=@HSort, HModify=@HModify,HModifyDT=@HModifyDT WHERE  HID =@HID ", dbConn);
                dbCmd.Parameters.AddWithValue("@HID", ((Label)Rpt_HTMaterial_T.Items[i].FindControl("LB_HID")).Text.Trim());
                dbCmd.Parameters.AddWithValue("@HSort", ((TextBox)Rpt_HTMaterial_T.Items[i].FindControl("TB_HTMaterialSort")).Text.Trim());
                dbCmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                dbCmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                dbCmd.ExecuteNonQuery();
                dbCmd.Cancel();
            }
        }
       


        dbConn.Close();
        dbCmd.Cancel();

        if (From == "submit")
        {
            Response.Write("<script>alert('存檔成功!');window.location.href='HCourseTemplate_Edit.aspx';</script>");
        }

    }

    #endregion



}