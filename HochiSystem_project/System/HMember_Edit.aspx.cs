using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


public partial class HMember_Edit : System.Web.UI.Page
{
    #region 連接字串
    string ConStr = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
    string Con = "HochiSystemConnection";
    #endregion

    #region 寄件資訊<正式>
    public string Sender = MailConfig.Sender;
    public string EAcount = MailConfig.Account;
    public string EPasword = MailConfig.Password;
    public string EHost = MailConfig.Host;
    public int EPort = MailConfig.Port;
    public bool EEnabledSSL = MailConfig.EnableSSL;
    #endregion

    #region 分頁
    private int PageMax = 15;   //分頁最大顯示數量
    private int LastPage = 10;  //分頁數量
    #endregion

    #region --根目錄--
    string ImgRoot = "D:\\Website\\System\\HochiSystem\\uploads\\Account\\";
    string Img = "~/uploads/Account/";
    #endregion


    #region  紀錄舊有資料
    string HArea;
    string HTeam;
    string HPosition = null;
    string HUTask;
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


        if (!IsPostBack)
        {
            ViewState["temp"] = "[]";  //子女
            ViewState["Select"] = "[]";
            ViewState["Volunteer"] = "[]";  //志願服務
            ViewState["Famen"] = "[]";  //法門
            ViewState["Group"] = "[]";  //團體

        }

        if (!IsPostBack)
        {


            //面板顯示
            Panel_List.Visible = true;
            Panel_Edit.Visible = false;


            if (Session["HUserHID"] != null)
            {
                #region 判斷是否為最高權限者
                SqlConnection dbConn;
                SqlCommand dbCmd;
                string strDBConn, strSQL;
                strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
                dbConn = new SqlConnection(strDBConn);
                dbConn.Open();
                // strSQL = "SELECT HMemberID  FROM HRole  WHERE HID=1 AND (HMemberID like '%" + "," + Session["HUserHID"] + "," + "%')";
                strSQL = "SELECT HID, HMemberID, HCrossRegion FROM HRole  WHERE HID=1 AND (HMemberID like '%" + "," + ((Label)Master.FindControl("LB_HUserHID")).Text + "," + "%')";
                dbCmd = new SqlCommand(strSQL, dbConn);
                SqlDataReader MyQuery;
                MyQuery = dbCmd.ExecuteReader();

                if (MyQuery.Read())  //最高權限
                {
                    DDL_SHArea.Enabled = true;

                    SDS_Member.SelectCommand = "SELECT HMember.HID, HSeries,HAccount, HUserName,HSystemID, HAreaID,HTeamID, HTeamType, HPeriod, HEmail, HPhone, HSystemName, HArea, HMember.HStatus,HType, HMType.HMType,case when HMemberID is null then '沒參班過' else '參過班' end '參班' From HMember Left JOIN HSystem ON HMember.HSystemID = HSystem.HID  Left JOIN HArea ON HMember.HAreaID = HArea.HID LEFT JOIN HMType ON HMember.HType = HMType.HID LEFT JOIN (　SELECT DISTINCT HMemberID    FROM HCourseBooking) b ON b.HMemberID = HMember.HID ORDER BY HStatus DESC,HMember.HAreaID ASC";
                    // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
                    Pg_Paging.PagingLoad("HochiSystemConnection", SDS_Member.SelectCommand, PageMax, LastPage, false, Rpt_Member);
                    ViewState["Select"] = SDS_Member.SelectCommand;
                }
                else  //不是最高權限
                {
                    //221229-加入判斷是否擁有跨區權限
                    SqlDataReader MPermission = SQLdatabase.ExecuteReader("SELECT  HRAccess, (M.HCrossRegion+',') AS CrossRegion FROM ( SELECT HMemberID,(SELECT DISTINCT(','+HRAccess) FROM HRole WHERE HMemberID like '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%' FOR XML PATH('')) AS HRAccess, (SELECT DISTINCT(','+cast(HCrossRegion AS NVARCHAR)) FROM HRole WHERE HMemberID like '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%' FOR XML PATH('')) AS HCrossRegion FROM HRole AS b) M WHERE M.HMemberID like '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%' AND (M.HCrossRegion+',') LIKE '%,1,%' GROUP BY HRAccess, HCrossRegion");

                    if (MPermission.Read())
                    {
                        DDL_SHArea.Enabled = true;

                        SDS_Member.SelectCommand = "SELECT HMember.HID, HSeries,HAccount, HUserName,HSystemID, HAreaID,HTeamID, HTeamType, HPeriod, HEmail, HPhone, HSystemName, HArea, HMember.HStatus,HType, HMType.HMType,case when HMemberID is null then '沒參班過' else '參過班' end '參班' From HMember Left JOIN HSystem ON HMember.HSystemID = HSystem.HID  Left JOIN HArea ON HMember.HAreaID = HArea.HID LEFT JOIN HMType ON HMember.HType = HMType.HID LEFT JOIN (　SELECT DISTINCT HMemberID    FROM HCourseBooking) b ON b.HMemberID = HMember.HID ORDER BY HStatus DESC,HMember.HAreaID ASC";
                        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
                        Pg_Paging.PagingLoad("HochiSystemConnection", SDS_Member.SelectCommand, PageMax, LastPage, false, Rpt_Member);
                        ViewState["Select"] = SDS_Member.SelectCommand;
                    }
                    else
                    {
                        #region //只能看自己區屬的學員資料

                        //string AreaID = null;
                        SqlDataReader QueryHAreaID = SQLdatabase.ExecuteReader("SELECT HAreaID FROM HMember where HID='" + Session["HUserHID"].ToString() + "'");
                        while (QueryHAreaID.Read())
                        {
                            //AreaID = QueryHAreaID["HAreaID"].ToString();
                            LB_HArea.Text = QueryHAreaID["HAreaID"].ToString();
                        }
                        QueryHAreaID.Close();

                        DDL_SHArea.SelectedValue = LB_HArea.Text;
                        DDL_SHArea.Enabled = false;

                        //220822--同區屬才能看
                        SDS_Member.SelectCommand = "SELECT HMember.HID, HSeries,HAccount, HUserName,HSystemID, HAreaID,HTeamID, HTeamType, HPeriod, HEmail, HPhone, HSystemName, HArea, HMember.HStatus,HType, HMType.HMType,case when HMemberID is null then '沒參班過' else '參過班' end '參班' From HMember Left JOIN HSystem ON HMember.HSystemID = HSystem.HID  Left JOIN HArea ON HMember.HAreaID = HArea.HID LEFT JOIN HMType ON HMember.HType = HMType.HID LEFT JOIN (　SELECT DISTINCT HMemberID    FROM HCourseBooking) b ON b.HMemberID = HMember.HID WHERE HMember.HAreaID='" + LB_HArea.Text + "'  ORDER BY HStatus DESC,HMember.HAreaID ASC";
                        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
                        Pg_Paging.PagingLoad("HochiSystemConnection", SDS_Member.SelectCommand, PageMax, LastPage, false, Rpt_Member);
                        ViewState["Select"] = SDS_Member.SelectCommand;

                        #endregion
                    }
                    MPermission.Close();



                }
                MyQuery.Close();
                #endregion


            }
            else
            {
                if (Request.Cookies["HochiInfo"] == null)
                {
                    Response.Write("<script>alert('連線逾時，請重新登入!');window.location.href='../../HLogin.aspx';</script>");
                }
            }

        }
        else
        {

            // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
            Pg_Paging.PagingLoad("HochiSystemConnection", ViewState["Select"].ToString(), PageMax, LastPage, false, Rpt_Member);
        }
    }


    protected void SDS_Member_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
    {
        e.Command.CommandTimeout = 60;  //or more time....
    }

    #region 列表選取功能
    protected void LBtn_Edit_Click(object sender, EventArgs e)
    {
        Panel_List.Visible = false;
        Panel_Edit.Visible = true;

        LinkButton LBtn_Edit = (LinkButton)sender;
        string Edit_CA = LBtn_Edit.CommandArgument;

        DDL_HType.DataBind();
        DDL_EPType.DataBind();
        DDL_HAreaID.DataBind();
        DDL_HAreaID.DataBind();
        DDL_HTeamID.DataBind();
        DDL_HSystemID.DataBind();
        DDL_HRole.DataBind();
        DDL_HCountryID.DataBind();
        DDL_HWorkType.DataBind();
        DDL_HWTItem.DataBind();
        DDL_EPType.DataBind();
        LBox_HEPItem.DataBind();
        DDL_HKWay.DataBind();
        DDL_HSWay.DataBind();
        LBox_HAxisType.DataBind();
        DDL_HLifeLeaderID.DataBind();


        //初始化
        //TB_HBirth.Attributes.Add("ReadOnly", "true");
        //TB_HBirth.Style.Add("background-color", "#fff");


        //内容載入
        using (SqlConnection con = new SqlConnection(ConStr))
        {


            //KE20240220_新增學員軸線類別的欄位
            //KE20250326_新增三載體光、七彩光、二十一道光、是否為光使欄位
            SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HMember.HID, HSeries, HAccount, HPassword, HUserName, HPeriod, HAreaType, HAreaID, HTeamID, HSystemID, HPositionID, HSex, HBlood, HPPName, HCountryID, HPersonID,HForeignID, HBirth, HAge, HUsualTask, HOTel, HCTel, HPhone, HOEmail,HEmail, HOTPStatus, HOTP, HEmerName, HEmerRelated,HEmerPhone, HRPostal, HRAddress, HCPostal, HCAddress,HLineID, HFB, HWechat, HWhatsapp, HIntroName, HIntroRelated, HDisease, HFDisease, HDad, HDFellow, HDLife,	HMom, HMFellow, HMLife, HMarriage, HSpouse, HSFellow, HSiblingNum, HRank, HEducation, HGraduate,HWorkType, HServiceTitle, HWorkName, HSchoolLevel, HExpertise, HVolunteer, HUnitName, HAuthor, HReligion, HFamen, HGroup, HLeaveDT, HDeadDT,HStatus, HChildren,HType,HImg, HTeamType,HWTItem, HEPType, HEPItem,HKWay, HSWay, HJDuration, HCareer, HQReason, HSADate, HSAReason, HDLTogether, HMLTogether, HSLTogether, HWTOthers, HUploadIRS, HAxisType, HLifeLeaderID, HCarrier, HRainbow, HLightEnvoy FROM HMember WHERE HMember.HID=" + Edit_CA);

            if (dr.Read())
            {
                LB_HID.Text = dr["HID"].ToString();
                TB_HSeries.Text = dr["HSeries"].ToString();
                TB_HEmail.Text = dr["HAccount"].ToString();
                //TB_HPassword.Text = dr["HPassword"].ToString();
                LB_OriPassword.Text = dr["HPassword"].ToString();
                RBL_HStatus.SelectedValue = dr["HStatus"].ToString();
                TB_HPeriod.Text = dr["HPeriod"].ToString();
                DDL_HAreaID.SelectedValue = !string.IsNullOrEmpty(dr["HAreaID"].ToString()) ? GetValue(dr["HAreaID"].ToString()) : "0";
                DDL_AreaType.SelectedValue = !string.IsNullOrEmpty(dr["HAreaID"].ToString()) ? dr["HAreaType"].ToString() : "1";
                LB_HOArea.Text = DDL_HAreaID.SelectedItem.Text;

                if (dr["HTeamID"].ToString() != "0" && (dr["HTeamID"].ToString().Split(',').Length == 2 && dr["HTeamID"].ToString().Contains(",") == true))
                {
                    DDL_HTeamID.SelectedValue = dr["HTeamID"].ToString();
                }
                else
                {
                    DDL_HTeamID.SelectedValue = "0";
                }



                LB_HOTeam.Text = DDL_HTeamID.SelectedItem.Text;

                DDL_HSystemID.SelectedValue = !string.IsNullOrEmpty(dr["HSystemID"].ToString()) ? GetValue(dr["HSystemID"].ToString()) : "0";
                DDL_HRole.SelectedValue = GetValue(dr["HPositionID"].ToString());
                LB_HOPositionID.Text = dr["HPositionID"].ToString();
                LB_HOPosition.Text = DDL_HRole.SelectedItem.Text;

                TB_HUserName.Text = dr["HUserName"].ToString();
                DDL_HSex.SelectedValue = GetValue(dr["HSex"].ToString());
                DDL_HBlood.SelectedValue = GetValue(dr["HBlood"].ToString());
                TB_HPPName.Text = dr["HPPName"].ToString();

                //KA20240328_軸線
                LBox_HAxisType.DataBind();
                string[] gHAxisType = dr["HAxisType"].ToString().Split(',');
                for (int i = 0; i < gHAxisType.Length - 1; i++)
                {
                    for (int j = 0; j < LBox_HAxisType.Items.Count; j++)
                    {
                        if (gHAxisType[i].ToString() == LBox_HAxisType.Items[j].Value)
                        {
                            LBox_HAxisType.Items[j].Selected = true;
                        }
                    }
                }
                LB_HAxisType.Text = dr["HAxisType"].ToString();


                if (dr["HCountryID"].ToString() == "0")
                {
                    DDL_HCountryID.Enabled = true;
                }
                else
                {
                    DDL_HCountryID.SelectedValue = GetValue(dr["HCountryID"].ToString());
                    DDL_HCountryID.Enabled = false;
                }


                TB_HPersonID.Text = dr["HPersonID"].ToString();

                //EA20240225_加入是否上傳國稅局
                DDL_HUploadIRS.SelectedValue = !string.IsNullOrEmpty(dr["HUploadIRS"].ToString()) ? dr["HUploadIRS"].ToString() : "99";

                TB_HForeignID.Text = dr["HForeignID"].ToString();
                TB_HBirth.Text = dr["HBirth"].ToString();

                DateTime birthdate;
                if (!string.IsNullOrEmpty(dr["HBirth"].ToString()))
                {
                    if (DateTime.TryParseExact(TB_HBirth.Text.Trim(), "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out birthdate))
                    {
                        int age = GetAgeByBirthdate(DateTime.ParseExact(TB_HBirth.Text, "yyyy/MM/dd", null));
                        TB_HAge.Text = Convert.ToString(age).ToString();
                        LB_BirthNotice.Visible = false;
                        TB_HBirth.Style.Add("border", "1px solid #ababab");
                    }
                    else
                    {
                        LB_BirthNotice.Visible = true;
                        TB_HBirth.Style.Add("border", "2px solid #de4848");
                        TB_HAge.Text = ""; // 或顯示錯誤訊息，例如：TB_HAge.Text = "格式錯誤";
                    }
                }


                #region 光系
                //三載光
                DDL_HCarrier.SelectedValue = !string.IsNullOrEmpty(dr["HCarrier"].ToString()) ? dr["HCarrier"].ToString() : "0";

                //七彩光
                DDL_HRainbow.SelectedValue = !string.IsNullOrEmpty(dr["HRainbow"].ToString()) ? dr["HRainbow"].ToString() : "0";

                //二十一道光
                if (DDL_HCarrier.SelectedValue != "0" && DDL_HRainbow.SelectedValue != "0")
                {
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "light_data.json");
                    LightDataCache.LoadData(path);

                    var allData = LightDataCache.GetAll();
                    var first = LightDataCache.GetById(1);

                    var result = LightDataCache.GetAll().Find(light => light.HCarrier == Convert.ToInt32(DDL_HCarrier.SelectedValue) && light.HRainbow == Convert.ToInt32(DDL_HRainbow.SelectedValue));

                    LB_HLightName.Text = result.HLightName;
                }
                else
                {
                    LB_HLightName.Text = "未定光系";
                }

                //是否為光使
                RBL_HLightEnvoy.SelectedValue = !string.IsNullOrEmpty(dr["HLightEnvoy"].ToString()) ? dr["HLightEnvoy"].ToString() : "0";
                #endregion


                //TB_HAge.Text = dr["HAge"].ToString();
                TB_HOTel.Text = dr["HOTel"].ToString();
                TB_HCTel.Text = dr["HCTel"].ToString();
                TB_HPhone.Text = dr["HPhone"].ToString();
                TB_HOEmail.Text = dr["HOEmail"].ToString();
                //TB_HEmail.Text = dr["HEmail"].ToString();
                TB_HEmerName.Text = dr["HEmerName"].ToString();
                TB_HEmerRelated.Text = dr["HEmerRelated"].ToString();
                TB_HEmerPhone.Text = dr["HEmerPhone"].ToString();
                TB_HRPostal.Text = dr["HRPostal"].ToString();
                TB_HRAddress.Text = dr["HRAddress"].ToString();
                TB_HCPostal.Text = dr["HCPostal"].ToString();
                TB_HCAddress.Text = dr["HCAddress"].ToString();
                TB_HLineID.Text = dr["HLineID"].ToString();
                TB_HFB.Text = dr["HFB"].ToString();
                TB_HWechat.Text = dr["HWechat"].ToString();
                TB_HWhatsapp.Text = dr["HWhatsapp"].ToString();
                TB_HIntroName.Text = dr["HIntroName"].ToString();
                TB_HIntroRelated.Text = dr["HIntroRelated"].ToString();
                TB_HDisease.Text = dr["HDisease"].ToString();
                TB_HFDisease.Text = dr["HFDisease"].ToString();
                TB_HDad.Text = dr["HDad"].ToString();
                RBL_HDFellow.SelectedValue = GetBooleanValue(dr["HDFellow"].ToString());
                RBL_HDLife.SelectedValue = GetBooleanValue(dr["HDLife"].ToString());
                TB_HMom.Text = dr["HMom"].ToString();
                RBL_HMFellow.SelectedValue = GetBooleanValue(dr["HMFellow"].ToString());
                RBL_HMLife.SelectedValue = GetBooleanValue(dr["HMLife"].ToString());
                DDL_HMarriage.SelectedValue = GetValue(dr["HMarriage"].ToString());
                TB_HSpouse.Text = dr["HSpouse"].ToString();
                TB_HSiblingNum.Text = dr["HSiblingNum"].ToString();
                DDL_HEducation.SelectedValue = GetValue(dr["HEducation"].ToString());
                TB_HGraduate.Text = dr["HGraduate"].ToString();
                //DDL_HWorkType.SelectedValue = GetValue(dr["HWorkType"].ToString());
                TB_HServiceTitle.Text = dr["HServiceTitle"].ToString();
                TB_HWorkName.Text = dr["HWorkName"].ToString();
                DDL_HSchoolLevel.SelectedValue = !string.IsNullOrEmpty(dr["HSchoolLevel"].ToString()) ? GetValue(dr["HSchoolLevel"].ToString()) : "0";
                //TB_HExpertise.Text = dr["HExpertise"].ToString();

                TB_HUnitName.Text = dr["HUnitName"].ToString();
                RBL_HAuthor.SelectedValue = GetBooleanValue(dr["HAuthor"].ToString());

                TB_HLeaveDT.Text = dr["HLeaveDT"].ToString();
                TB_HDeadDT.Text = dr["HDeadDT"].ToString();
                DDL_HType.Text = !string.IsNullOrEmpty(dr["HType"].ToString()) ? GetValue(dr["HType"].ToString()) : "0";
                //EA20250703_新增生命導師
                //DDL_HLifeLeaderID.Text = !string.IsNullOrEmpty(dr["HLifeLeaderID"].ToString()) ? GetValue(dr["HLifeLeaderID"].ToString()) : "0";
                DDL_HLifeLeaderID.SelectedValue = DDL_HLifeLeaderID.Items.FindByValue(dr["HLifeLeaderID"].ToString()) != null ? dr["HLifeLeaderID"].ToString() : "0";
                DDL_EPType.SelectedValue = GetValue(dr["HEPType"].ToString());
                DDL_HWorkType.SelectedValue = GetValue(dr["HWorkType"].ToString());
                DDL_HWTItem.SelectedValue = GetValue(dr["HWTItem"].ToString());
                TB_HWTOthers.Text = dr["HWTOthers"].ToString();

                DDL_HKWay.SelectedValue = !string.IsNullOrEmpty(dr["HKWay"].ToString()) ? dr["HKWay"].ToString() : "0";
                DDL_HSWay.SelectedValue = !string.IsNullOrEmpty(dr["HSWay"].ToString()) ? dr["HSWay"].ToString() : "0";
                TB_HJDuration.Text = dr["HJDuration"].ToString();

                RBL_HDLTogether.SelectedValue = GetBooleanValue(dr["HDLTogether"].ToString());
                RBL_HMLTogether.SelectedValue = GetBooleanValue(dr["HMLTogether"].ToString());
                RBL_HSLTogether.SelectedValue = GetBooleanValue(dr["HSLTogether"].ToString());

                //TB_HCareer.Text = dr["HCareer"].ToString();
                TB_HCareer.Text = dr["HCareer"].ToString().Replace("<br/>", "" + Environment.NewLine + "");
                TB_HSADate.Text = dr["HSADate"].ToString();
                TB_HSAReason.Text = dr["HSAReason"].ToString();
                TB_HQReason.Text = dr["HQReason"].ToString();

                string[] gHReligion = dr["HReligion"].ToString().Split(',');
                for (int i = 0; i < gHReligion.Length - 1; i++)
                {
                    for (int j = 0; j < LBox_HReligion.Items.Count; j++)
                    {
                        if (gHReligion[i].ToString() == LBox_HReligion.Items[j].Value)
                        {
                            LBox_HReligion.Items[j].Selected = true;
                        }
                    }
                }

                //職業主類別
                if (dr["HWorkType"].ToString() != "0")
                {
                    if (DDL_HWorkType.SelectedItem.Text == "其他")
                    {
                        DDL_HWTItem.Visible = false;
                        TB_HWTOthers.Visible = true;
                    }
                    else
                    {
                        DDL_HWTItem.Visible = true;
                        TB_HWTOthers.Visible = false;

                        SDS_WTItem.SelectCommand = "SELECT HID, HWType, HWTItemName FROM HWTItem WHERE HWType=" + DDL_HWorkType.SelectedValue;
                        DDL_HWTItem.DataBind();
                    }

                }
                else
                {
                    SDS_WTItem.SelectCommand = "SELECT HID, HWType, HWTItemName FROM HWTItem ORDER BY HID ASC";
                    DDL_HWTItem.DataBind();
                }

                //專長
                if (!string.IsNullOrEmpty(dr["HEPType"].ToString()))
                {
                    //SDS_HEPItem.SelectCommand = "SELECT HID, HEPType, HEPItem FROM   HEPItem WHERE HEPType=" + DDL_EPType.SelectedValue;
                    SDS_HEPItem.SelectCommand = "SELECT HID, HEPType, HEPItem FROM HEPItem ORDER BY HID ASC";
                    LBox_HEPItem.DataBind();
                }
                else
                {
                    //SDS_HEPItem.SelectCommand = "SELECT HID, HEPType, HEPItem FROM HEPItem ORDER BY HID ASC";
                    SDS_HEPItem.SelectCommand = "SELECT HID, HEPType, HEPItem FROM HEPItem ORDER BY HID ASC";
                    LBox_HEPItem.DataBind();
                }

                string[] gHEPItem = dr["HEPItem"].ToString().Split(',');
                for (int i = 0; i < gHEPItem.Length - 1; i++)
                {
                    for (int j = 0; j < LBox_HEPItem.Items.Count; j++)
                    {
                        if (gHEPItem[i].ToString() == LBox_HEPItem.Items[j].Value)
                        {
                            LBox_HEPItem.Items[j].Selected = true;
                        }
                    }
                }


                if (!string.IsNullOrEmpty(dr["HImg"].ToString()))
                {
                    IMG_Pic.ImageUrl = "../uploads/Account/" + dr["HImg"].ToString();
                    LB_OldPic.Text = dr["HImg"].ToString();
                }
                else
                {
                    Btn_Del.Visible = false;
                    IMG_Pic.Visible = false;
                    IMG_Pic.Width = 0;
                }

                #region 子女資料
                string Children_content = dr["HChildren"].ToString();

                if (!string.IsNullOrEmpty(Children_content))
                {
                    List<Children.list_item> Table = JsonConvert.DeserializeObject<List<Children.list_item>>(Children_content.ToString());
                    Rpt_HChildren.DataSource = Table;
                    Rpt_HChildren.DataBind();

                    int x = 0;
                    foreach (Children.list_item Element in Table)
                    {
                        ((TextBox)Rpt_HChildren.Items[x].FindControl("TB_HCName_Edit")).Text = Element.CName;
                        ((DropDownList)Rpt_HChildren.Items[x].FindControl("DDL_HCSex_Edit")).Text = Element.CSex;
                        ((RadioButtonList)Rpt_HChildren.Items[x].FindControl("RBL_HCFellow_Edit")).Text = Element.CFellow;
                        ((RadioButtonList)Rpt_HChildren.Items[x].FindControl("RBL_HCLife_Edit")).Text = Element.CLife;
                        x++;
                    }

                    //加入Table
                    ViewState["temp"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
                    CreateTable();

                    //註冊事件
                    for (int i = 0; x < Rpt_HChildren.Items.Count; i++)
                    {
                        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Rpt_HChildren.Items[i].FindControl("LBtn_Off"));
                    }
                }

                #endregion

                #region 志願服務
                string Volunteer = dr["HVolunteer"].ToString();

                if (!string.IsNullOrEmpty(Volunteer))
                {
                    List<Volunteer.Volunteer_item> Table = JsonConvert.DeserializeObject<List<Volunteer.Volunteer_item>>(Volunteer.ToString());
                    Rpt_HVolunteer.DataSource = Table;
                    Rpt_HVolunteer.DataBind();

                    int x = 0;
                    foreach (Volunteer.Volunteer_item Element in Table)
                    {
                        ((TextBox)Rpt_HVolunteer.Items[x].FindControl("TB_SName_Edit")).Text = Element.SName;
                        ((TextBox)Rpt_HVolunteer.Items[x].FindControl("TB_SUnit_Edit")).Text = Element.SUnit;
                        x++;
                    }

                    //加入Table
                    ViewState["Volunteer"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
                    CreateVolunteer();

                    //註冊事件
                    for (int i = 0; x < Rpt_HVolunteer.Items.Count; i++)
                    {
                        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Rpt_HVolunteer.Items[i].FindControl("LBtn_HVolunteerDel"));
                    }
                }
                #endregion

                #region 法門
                string Famen = dr["HFamen"].ToString();

                if (!string.IsNullOrEmpty(Famen))
                {
                    List<Famen.Famen_item> Table = JsonConvert.DeserializeObject<List<Famen.Famen_item>>(Famen.ToString());
                    Rpt_HFamen.DataSource = Table;
                    Rpt_HFamen.DataBind();

                    int x = 0;
                    foreach (Famen.Famen_item Element in Table)
                    {
                        ((TextBox)Rpt_HFamen.Items[x].FindControl("TB_HFamen_Edit")).Text = Element.Famen;
                        x++;
                    }

                    //加入Table
                    ViewState["Famen"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
                    CreateFamen();

                    //註冊事件
                    for (int i = 0; x < Rpt_HFamen.Items.Count; i++)
                    {
                        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Rpt_HFamen.Items[i].FindControl("LBtn_HFamenDel"));
                    }
                }
                #endregion

                #region 團體
                string Group = dr["HGroup"].ToString();

                if (!string.IsNullOrEmpty(Group))
                {
                    List<Group.Group_item> Table = JsonConvert.DeserializeObject<List<Group.Group_item>>(Group.ToString());
                    Rpt_HGroup.DataSource = Table;
                    Rpt_HGroup.DataBind();


                    int x = 0;
                    foreach (Group.Group_item Element in Table)
                    {
                        ((DropDownList)Rpt_HGroup.Items[x].FindControl("DDL_GType_Edit")).SelectedValue = Element.GType;
                        ((TextBox)Rpt_HGroup.Items[x].FindControl("TB_HGOthers_Edit")).Text = Element.GOtherType;
                        ((TextBox)Rpt_HGroup.Items[x].FindControl("TB_GName_Edit")).Text = Element.GName;
                        ((TextBox)Rpt_HGroup.Items[x].FindControl("TB_Duration_Edit")).Text = Element.Duration;
                        ((TextBox)Rpt_HGroup.Items[x].FindControl("TB_Job_Edit")).Text = Element.Job;
                        ((DropDownList)Rpt_HGroup.Items[x].FindControl("DDL_JobLevel_Edit")).SelectedValue = Element.JobLevel;
                        x++;
                    }

                    //加入Table
                    ViewState["Group"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
                    CreateGroup();

                    //註冊事件
                    for (int i = 0; x < Rpt_HGroup.Items.Count; i++)
                    {
                        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(Rpt_HGroup.Items[i].FindControl("LBtn_HGroupDel"));
                    }
                }
                #endregion

                //常態任務
                SqlDataReader UsualTask = SQLdatabase.ExecuteReader("SELECT HID, HRName, HMemberID, HRType FROM  HRole Where HRType = 1 AND HMemberID like '%," + Edit_CA + ",%'");

                while (UsualTask.Read())
                {
                    LB_HUsualTask.Text += UsualTask["HRName"].ToString() + ",";
                }
                UsualTask.Close();


                //管理日期判斷是否為自己，則不能修改
                if (LB_HID.Text == ((Label)Master.FindControl("LB_HUserHID")).Text)
                {
                    Panel_Manage.Enabled = false;
                }

            }
            dr.Close();
        }


    }
    #endregion

    /// <summary>
    /// 載入編輯資料
    /// </summary>
    /// <param name="SeletedValue">下拉選單值</param>
    protected string GetValue(string SeletedValue)
    {
        string Value = "0";  //預設值

        if (SeletedValue != "0")
        {
            Value = SeletedValue;
        }
        else
        {
            Value = "0";  //-請選擇-
        }
        return Value;
    }

    /// <summary>
    /// 載入編輯資料
    /// </summary>
    /// <param name="Boolean">RadioButton</param>
    protected string GetBooleanValue(string SeletedValue)
    {
        string BooleanValue = "0";  //預設值

        if (SeletedValue == "False")
        {
            BooleanValue = "0";
        }
        else if (SeletedValue == "True")
        {
            BooleanValue = "1";
        }
        return BooleanValue;
    }

    #region 列表刪除(停用) 
    protected void LBtn_Off_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Off = sender as LinkButton;
        int HID = Convert.ToInt32(LBtn_Off.CommandArgument);

        string UpdSQL = "Update HMember set HStatus='0',HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "',HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' where HID='" + HID + "'";
        SQLdatabase.ExecuteNonQuery(UpdSQL);

        Response.Write("<script>alert('學員停用成功');window.location.href='HMember_Edit.aspx';</script>");
    }
    #endregion

    #region 列表啟用帳號
    protected void LBtn_On_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_On = sender as LinkButton;
        int HID = Convert.ToInt32(LBtn_On.CommandArgument);

        string UpdSQL = "Update HMember set HStatus='1',HModify='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "',HModifyDT='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' where HID='" + HID + "'";
        SQLdatabase.ExecuteNonQuery(UpdSQL);

        Response.Write("<script>alert('學員啟用成功');window.location.href='HMember_Edit.aspx';</script>");
    }
    #endregion

    #region   列表-資料綁定
    protected void Rpt_Member_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        HtmlGenericControl Activation = e.Item.FindControl("Activation") as HtmlGenericControl;
        //停用判斷
        if (((Label)e.Item.FindControl("LB_HStatus")).Text == "1")
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "啟用";
            Activation.Attributes.Add("class", "badge badge-success");
            ((LinkButton)e.Item.FindControl("LBtn_Off")).Visible = true;
            ((LinkButton)e.Item.FindControl("LBtn_On")).Visible = false;
        }
        else
        {
            ((Label)e.Item.FindControl("LB_HStatus")).Text = "停用";
            Activation.Attributes.Add("class", "badge badge-default");
            ((LinkButton)e.Item.FindControl("LBtn_On")).Visible = true;
            ((LinkButton)e.Item.FindControl("LBtn_Off")).Visible = false;
        }

        //光團資料顯示
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HTeamID")).Text) && ((Label)e.Item.FindControl("LB_HTeamID")).Text != "0")
        {
            //分割
            string[] TeamID = ((Label)e.Item.FindControl("LB_HTeamID")).Text.Split(',');
            //分割後
            string TeamHID = null;
            string TeamType = null;    //光團類型  1:母光團，2:光團

            if (TeamID.Length == 2)
            {
                TeamHID = TeamID[0].ToString();
                TeamType = TeamID[1].ToString();    //光團類型  1:母光團，2:光團

                if (TeamType == "1")  //母光團
                {
                    SqlDataReader MTeam = SQLdatabase.ExecuteReader("SELECT HMTeam FROM HMTeam Where HID ='" + TeamHID + "'");
                    if (MTeam.Read())
                    {
                        ((Label)e.Item.FindControl("LB_HTeamName")).Text = MTeam["HMTeam"].ToString();
                    }
                    MTeam.Close();
                }
                else if (TeamType == "2")   //光團
                {
                    SqlDataReader CTeam = SQLdatabase.ExecuteReader("SELECT HCTeam FROM HCTeam Where HID ='" + TeamHID + "'");
                    if (CTeam.Read())
                    {
                        ((Label)e.Item.FindControl("LB_HTeamName")).Text = CTeam["HCTeam"].ToString();
                    }
                    CTeam.Close();
                }
            }
            else
            {
                ((Label)e.Item.FindControl("LB_HTeamName")).Text = "";
            }




        }

    }
    #endregion

    #region 儲存
    protected void Btn_Submit_Click(object sender, EventArgs e)
    {
   

        if (string.IsNullOrEmpty(TB_HUserName.Text.Trim()))
        {
            Response.Write("<script>alert('請輸入姓名~!');</script>");
            return;
        }


        if (DDL_HAreaID.SelectedValue == "0")
        {
            Response.Write("<script>alert('請選擇區屬~!');</script>");
            return;
        }
        if (DDL_HCountryID.SelectedValue == "0")
        {
            Response.Write("<script>alert('請選擇國家~!');</script>");
            return;
        }
        if (string.IsNullOrEmpty(TB_HPeriod.Text))
        {
            Response.Write("<script>alert('請輸入期別~!');</script>");
            return;
        }

        //生日格式驗證
        if (!string.IsNullOrEmpty(TB_HBirth.Text.Trim()))
        {
            DateTime d;
            var ok = DateTime.TryParseExact(TB_HBirth.Text.Trim(), "yyyy/MM/dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out d)
                     && d.Year >= 1900 && d <= DateTime.Today;

            if (!ok)
            {
                Response.Write("<script>alert('生日格式有誤，請輸入正確格式(yyyy/MM/dd)~!');</script>");
                return;
            }

        }



        #region 計算年齡
        if (!string.IsNullOrEmpty(TB_HBirth.Text.Trim()))
        {
            DateTime birthdate;
            if (DateTime.TryParseExact(TB_HBirth.Text.Trim(), "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out birthdate))
            {
                int age = GetAgeByBirthdate(DateTime.ParseExact(TB_HBirth.Text.Trim(), "yyyy/MM/dd", null));
                TB_HAge.Text = Convert.ToString(age).ToString();
            }
        }
        #endregion

        using (SqlConnection con = new SqlConnection(ConStr))
        {
            string SeriesNo = null;

            if (string.IsNullOrEmpty(TB_HSeries.Text.Trim()))  //如果沒有法脈序號
            {
                #region 法脈序號 
                //命名規則:國家-期別-流水號 (國家和期別相同才會繼續編號下去)

                //DDL選擇的國家英文
                string strCountry = DDL_HCountryID.SelectedItem.Text.Substring(0, 2);

                SqlDataReader drSeries = SQLdatabase.ExecuteReader("select top 1 HSeries from HMember");
                if (drSeries.Read())
                {
                    //接著排序
                    string oldSeries = drSeries["HSeries"].ToString();

                    if (!string.IsNullOrEmpty(oldSeries))
                    {
                        //分割
                        string[] Series = oldSeries.Split('-');

                        //分割後
                        string Country = Series[0].ToString();
                        string Period = Series[1].ToString();
                        int SNo = Convert.ToInt32(Series[2].ToString());

                        if (strCountry == Country && TB_HPeriod.Text == Period)
                        {
                            SNo++;
                            SeriesNo = strCountry + "-" + TB_HPeriod.Text.Trim() + "-" + SNo.ToString("000");
                        }
                        else
                        {
                            SeriesNo = strCountry + "-" + TB_HPeriod.Text.Trim() + "-001";
                        }
                    }
                    else
                    {
                        SeriesNo = strCountry + "-" + TB_HPeriod.Text.Trim() + "-001";
                    }

                }
                else
                {
                    SeriesNo = strCountry + "-" + TB_HPeriod.Text.Trim() + "-001";
                }
                drSeries.Close();
                #endregion
            }
            else
            {
                SeriesNo = TB_HSeries.Text.Trim();
            }

            string picname = "";
            if (LB_OldPic.Text != "" && FU_HImg.HasFile == true)
            {
                //上傳圖片
                Upload_Pic();

                picname = LB_Pic.Text;
            }
            else if (LB_OldPic.Text != "" && FU_HImg.HasFile == false)
            {
                picname = LB_OldPic.Text;
            }
            else if (LB_OldPic.Text == "" && FU_HImg.HasFile == true)
            {
                //上傳圖片
                Upload_Pic();
                picname = LB_Pic.Text;
            }
            if (!string.IsNullOrEmpty(TB_HPersonID.Text.Trim()))
            {
                //身分證重複判斷與驗證

                //重複判斷--須排除自己
                SqlCommand cmd02 = new SqlCommand("SELECT HPersonID FROM HMember WHERE HPersonID=@HPersonID AND HID <> @HID", con);
                con.Open();
                cmd02.Parameters.AddWithValue("@HPersonID", TB_HPersonID.Text.ToUpper().Trim());
                cmd02.Parameters.AddWithValue("@HID", LB_HID.Text);
                SqlDataReader MyEBF = cmd02.ExecuteReader();
                if (MyEBF.Read())
                {
                    //Response.Write("aaaa");
                    //Response.End();
                    LB_NoticePersonID.Visible = true;
                    LB_NoticePersonID.Text = "此身分證已存在";
                    return;
                }
                else  //沒有重複再判斷格式
                {
                    //格式判斷
                    if (CheckId(TB_HPersonID.Text.ToUpper().Trim()) == false)
                    {
                        LB_NoticePersonID.Visible = true;
                        LB_NoticePersonID.Text = "身分證格式有誤";
                        return;
                    }
                    else
                    {
                        LB_NoticePersonID.Visible = false;
                    }

                }
                MyEBF.Close();
                cmd02.Cancel();
                con.Close();


            }

            //KA20240328_新增
            //取ListBox的值，使用ForEach方式
            string gLBox_HAxisType = "";
            foreach (ListItem LBoxHAxisType in LBox_HAxisType.Items)
            {
                if (LBoxHAxisType.Selected == true)
                {
                    gLBox_HAxisType += LBoxHAxisType.Value + ",";
                }
            }


            //取ListBox的值，使用ForEach方式
            string gLBox_HEPItem = "";
            foreach (ListItem LBoxHEItem in LBox_HEPItem.Items)
            {
                if (LBoxHEItem.Selected == true)
                {
                    gLBox_HEPItem += LBoxHEItem.Value + ",";
                }
            }


            //取ListBox的值，使用ForEach方式
            string gLBox_HReligion = "";
            foreach (ListItem LBoxHReligion in LBox_HReligion.Items)
            {
                if (LBoxHReligion.Selected == true)
                {
                    gLBox_HReligion += LBoxHReligion.Value + ",";
                }
            }


            #region 將密碼HASH存入DB
            string gPassword = null;
            if (!string.IsNullOrEmpty(TB_HPassword.Text.Trim()))
            {
                //KA240926_將密碼hash存入DB
                SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                byte[] gHashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(TB_HPassword.Text.Trim()));
                gPassword = Convert.ToBase64String(gHashValue);
            }
            else
            {
                gPassword = LB_OriPassword.Text;
            }

            #endregion


            //KE20240220_加入學員軸線類別
            SqlCommand cmd = new SqlCommand("update HMember set HSeries=@HSeries, HAccount=@HAccount, HPassword=@HPassword, HUserName=@HUserName, HPeriod=@HPeriod, HAreaType=@HAreaType, HAreaID=@HAreaID, HTeamID=@HTeamID, HSystemID=@HSystemID, HPositionID=@HPositionID, HSex=@HSex, HBlood=@HBlood, HPPName=@HPPName, HCountryID=@HCountryID, HPersonID=@HPersonID, HForeignID=@HForeignID, HBirth=@HBirth, HAge=@HAge, HOTel=@HOTel, HCTel=@HCTel, HPhone=@HPhone, HOEmail=@HOEmail,HOTPStatus=@HOTPStatus,  HEmerName=@HEmerName, HEmerRelated=@HEmerRelated, HEmerPhone=@HEmerPhone,HRPostal=@HRPostal, HRAddress=@HRAddress, HCPostal=@HCPostal, HCAddress=@HCAddress, HLineID=@HLineID, HFB=@HFB, HWechat=@HWechat, HWhatsapp=@HWhatsapp, HIntroName=@HIntroName, HIntroRelated=@HIntroRelated, HDisease=@HDisease,HFDisease=@HFDisease, HDad=@HDad,HDFellow=@HDFellow, HDLife=@HDLife, HMom=@HMom, HMFellow=@HMFellow, HMLife=@HMLife, HMarriage=@HMarriage, HSpouse=@HSpouse, HSFellow=@HSFellow, HSiblingNum=@HSiblingNum,HRank=@HRank, HEducation=@HEducation, HGraduate=@HGraduate, HWorkType=@HWorkType, HServiceTitle=@HServiceTitle, HWorkName=@HWorkName, HSchoolLevel=@HSchoolLevel, HVolunteer=@HVolunteer, HUnitName=@HUnitName, HAuthor=@HAuthor, HReligion=@HReligion, HFamen=@HFamen,	HGroup=@HGroup, HLeaveDT=@HLeaveDT, HDeadDT=@HDeadDT, HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT, HChildren=@HChildren,HType=@HType,HImg=@HImg,HWTItem=@HWTItem, HEPType=@HEPType, HEPItem=@HEPItem,HKWay=@HKWay, HSWay=@HSWay, HJDuration=@HJDuration, HCareer=@HCareer, HQReason=@HQReason, HSADate=@HSADate,HSAReason=@HSAReason, HDLTogether=@HDLTogether, HMLTogether=@HMLTogether, HSLTogether=@HSLTogether, HWTOthers=@HWTOthers, HAxisType=@HAxisType, HLifeLeaderID=@HLifeLeaderID, HCarrier=@HCarrier, HRainbow=@HRainbow, HLightEnvoy = @HLightEnvoy where HID = '" + LB_HID.Text + "'", con);

            con.Open();

            cmd.Parameters.AddWithValue("@HSeries", SeriesNo);
            cmd.Parameters.AddWithValue("@HAccount", TB_HEmail.Text.Trim());
            //KE20240926_改成Hash
            cmd.Parameters.AddWithValue("@HPassword", gPassword);
            cmd.Parameters.AddWithValue("@HUserName", TB_HUserName.Text.Trim());
            cmd.Parameters.AddWithValue("@HPeriod", TB_HPeriod.Text.Trim());
            cmd.Parameters.AddWithValue("@HAreaType", DDL_AreaType.SelectedValue);
            cmd.Parameters.AddWithValue("@HAreaID", DDL_HAreaID.SelectedValue);
            cmd.Parameters.AddWithValue("@HTeamID", DDL_HTeamID.SelectedValue);
            cmd.Parameters.AddWithValue("@HSystemID", DDL_HSystemID.SelectedValue);
            cmd.Parameters.AddWithValue("@HPositionID", DDL_HRole.SelectedValue);
            cmd.Parameters.AddWithValue("@HSex", DDL_HSex.SelectedValue);
            cmd.Parameters.AddWithValue("@HBlood", DDL_HBlood.SelectedValue);
            cmd.Parameters.AddWithValue("@HPPName", TB_HPPName.Text.Trim());
            cmd.Parameters.AddWithValue("@HCountryID", DDL_HCountryID.SelectedValue);
            cmd.Parameters.AddWithValue("@HPersonID", TB_HPersonID.Text.Trim());
            cmd.Parameters.AddWithValue("@HForeignID", TB_HForeignID.Text.Trim());
            cmd.Parameters.AddWithValue("@HBirth", TB_HBirth.Text.Trim());
            cmd.Parameters.AddWithValue("@HAge", TB_HAge.Text.Trim());
            cmd.Parameters.AddWithValue("@HOTel", TB_HOTel.Text.Trim());
            cmd.Parameters.AddWithValue("@HCTel", TB_HCTel.Text.Trim());
            cmd.Parameters.AddWithValue("@HPhone", TB_HPhone.Text.Trim());
            cmd.Parameters.AddWithValue("@HOEmail", TB_HOEmail.Text.Trim());
            cmd.Parameters.AddWithValue("@HOTPStatus", 1); //KE20230922_加入開啟Email驗證
            cmd.Parameters.AddWithValue("@HEmerName", TB_HEmerName.Text.Trim());
            cmd.Parameters.AddWithValue("@HEmerRelated", TB_HEmerRelated.Text.Trim());
            cmd.Parameters.AddWithValue("@HEmerPhone", TB_HEmerPhone.Text.Trim());
            cmd.Parameters.AddWithValue("@HRPostal", TB_HRPostal.Text.Trim());
            cmd.Parameters.AddWithValue("@HRAddress", TB_HRAddress.Text.Trim());
            cmd.Parameters.AddWithValue("@HCPostal", TB_HCPostal.Text.Trim());
            cmd.Parameters.AddWithValue("@HCAddress", TB_HCAddress.Text.Trim());
            cmd.Parameters.AddWithValue("@HLineID", TB_HLineID.Text.Trim());
            cmd.Parameters.AddWithValue("@HFB", TB_HFB.Text.Trim());
            cmd.Parameters.AddWithValue("@HWechat", TB_HWechat.Text.Trim());
            cmd.Parameters.AddWithValue("@HWhatsapp", TB_HWhatsapp.Text.Trim());
            cmd.Parameters.AddWithValue("@HIntroName", TB_HIntroName.Text.Trim());
            cmd.Parameters.AddWithValue("@HIntroRelated", TB_HIntroRelated.Text.Trim());
            cmd.Parameters.AddWithValue("@HDisease", TB_HDisease.Text.Trim());
            cmd.Parameters.AddWithValue("@HFDisease", TB_HFDisease.Text.Trim());
            cmd.Parameters.AddWithValue("@HDad", TB_HDad.Text.Trim());
            cmd.Parameters.AddWithValue("@HDFellow", RBL_HDFellow.SelectedValue);
            cmd.Parameters.AddWithValue("@HDLife", RBL_HDLife.SelectedValue);
            cmd.Parameters.AddWithValue("@HMom", TB_HMom.Text.Trim());
            cmd.Parameters.AddWithValue("@HMFellow", RBL_HMFellow.SelectedValue);
            cmd.Parameters.AddWithValue("@HMLife", RBL_HMLife.SelectedValue);
            cmd.Parameters.AddWithValue("@HMarriage", DDL_HMarriage.SelectedValue);
            cmd.Parameters.AddWithValue("@HSpouse", TB_HSpouse.Text.Trim());
            cmd.Parameters.AddWithValue("@HSFellow", RBL_HSFellow.SelectedValue);
            cmd.Parameters.AddWithValue("@HSiblingNum", TB_HSiblingNum.Text.Trim());
            cmd.Parameters.AddWithValue("@HRank", TB_HRank.Text.Trim());
            cmd.Parameters.AddWithValue("@HEducation", DDL_HEducation.SelectedValue);
            cmd.Parameters.AddWithValue("@HGraduate", TB_HGraduate.Text.Trim());
            cmd.Parameters.AddWithValue("@HWorkType", DDL_HWorkType.SelectedValue);
            cmd.Parameters.AddWithValue("@HServiceTitle", TB_HServiceTitle.Text.Trim());
            cmd.Parameters.AddWithValue("@HWorkName", TB_HWorkName.Text.Trim());
            cmd.Parameters.AddWithValue("@HSchoolLevel", DDL_HSchoolLevel.SelectedValue);
            cmd.Parameters.AddWithValue("@HVolunteer", ViewState["Volunteer"].ToString());
            cmd.Parameters.AddWithValue("@HUnitName", TB_HUnitName.Text.Trim());
            cmd.Parameters.AddWithValue("@HAuthor", RBL_HAuthor.Text.Trim());
            cmd.Parameters.AddWithValue("@HReligion", gLBox_HReligion);
            cmd.Parameters.AddWithValue("@HFamen", ViewState["Famen"].ToString());
            cmd.Parameters.AddWithValue("@HGroup", ViewState["Group"].ToString());
            cmd.Parameters.AddWithValue("@HLeaveDT", TB_HLeaveDT.Text.Trim());
            cmd.Parameters.AddWithValue("@HDeadDT", TB_HDeadDT.Text.Trim());
            cmd.Parameters.AddWithValue("@HStatus", RBL_HStatus.SelectedValue);
            cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@HChildren", ViewState["temp"].ToString());
            cmd.Parameters.AddWithValue("@HType", DDL_HType.SelectedValue);
            cmd.Parameters.AddWithValue("@HImg", picname);
            cmd.Parameters.AddWithValue("@HWTItem", DDL_HWTItem.SelectedValue);
            cmd.Parameters.AddWithValue("@HEPType", DDL_EPType.SelectedValue);
            cmd.Parameters.AddWithValue("@HEPItem", gLBox_HEPItem);
            cmd.Parameters.AddWithValue("@HKWay", DDL_HKWay.SelectedValue);
            cmd.Parameters.AddWithValue("@HSWay", DDL_HSWay.SelectedValue);
            cmd.Parameters.AddWithValue("@HJDuration", TB_HJDuration.Text.Trim());
            cmd.Parameters.AddWithValue("@HCareer", TB_HCareer.Text.Trim().Replace(System.Environment.NewLine, "<br/>"));
            cmd.Parameters.AddWithValue("@HQReason", TB_HQReason.Text.Trim());

            cmd.Parameters.AddWithValue("@HSADate", TB_HSADate.Text.Trim());
            cmd.Parameters.AddWithValue("@HSAReason", TB_HSAReason.Text.Trim());
            cmd.Parameters.AddWithValue("@HDLTogether", RBL_HDLTogether.SelectedValue);
            cmd.Parameters.AddWithValue("@HMLTogether", RBL_HMLTogether.SelectedValue);
            cmd.Parameters.AddWithValue("@HSLTogether", RBL_HSLTogether.SelectedValue);
            cmd.Parameters.AddWithValue("@HWTOthers", TB_HWTOthers.Text.Trim());
            //KA20240220_加入學員軸線類別
            cmd.Parameters.AddWithValue("@HAxisType", gLBox_HAxisType);
            //EA20250703_新增生命導師
            cmd.Parameters.AddWithValue("@HLifeLeaderID", DDL_HLifeLeaderID.SelectedValue);
            //KA20250326_新增三載體光、七彩光、是否為光使
            cmd.Parameters.AddWithValue("@HCarrier", DDL_HCarrier.SelectedValue);
            cmd.Parameters.AddWithValue("@HRainbow", DDL_HRainbow.SelectedValue);
            cmd.Parameters.AddWithValue("@HLightEnvoy", RBL_HLightEnvoy.SelectedValue);

            cmd.ExecuteNonQuery();
            cmd.Cancel();


            #region 220818-ADD 當原本已有天命法位，但改成別的天命法位時，要清掉學員在原本天命法位的權限
            if (LB_HOPositionID.Text != "0" && LB_HOPositionID.Text != DDL_HRole.SelectedValue)
            {
                string UpdMemberID = null;

                SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HMemberID FROM HRole WHERE HID=" + LB_HOPositionID.Text);
                if (dr.Read())
                {
                    if (!string.IsNullOrEmpty(dr["HMemberID"].ToString()))
                    {

                        string[] NewMemberID = dr["HMemberID"].ToString().Split(',');
                        string myRemoveNum = LB_HID.Text;

                        //移除陣列中特定的值，再重新組合
                        NewMemberID = NewMemberID.Where(val => val != myRemoveNum).ToArray();
                        UpdMemberID = String.Join(",", NewMemberID);

                        if (UpdMemberID == ",")
                        {
                            UpdMemberID = "";
                        }

                    }
                }
                dr.Close();

                //更新原天命法位裡的名單
                cmd = new SqlCommand("update HRole set HMemberID=@HMemberID, HModify=@HModify, HModifyDT=@HModifyDT where HID = '" + LB_HOPositionID.Text + "'", con);
                cmd.Parameters.AddWithValue("@HMemberID", UpdMemberID);
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                cmd.ExecuteNonQuery();
                cmd.Cancel();
            }



            #endregion


            #region 天命法位權限要存入
            if (DDL_HRole.SelectedValue != "0")
            {
                string MemberID = null;

                SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HMemberID FROM HRole WHERE HID=" + DDL_HRole.SelectedValue);
                if (dr.Read())
                {
                    if (!string.IsNullOrEmpty(dr["HMemberID"].ToString()))
                    {
                        MemberID = dr["HMemberID"].ToString() + LB_HID.Text + ",";
                    }
                    else
                    {
                        MemberID = "," + LB_HID.Text + ",";
                    }
                }
                dr.Close();

                cmd = new SqlCommand("update HRole set HMemberID=@HMemberID, HModify=@HModify, HModifyDT=@HModifyDT where HID = '" + DDL_HRole.SelectedValue + "'", con);
                cmd.Parameters.AddWithValue("@HMemberID", MemberID);
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                cmd.ExecuteNonQuery();
                cmd.Cancel();
            }


            con.Close();

            #endregion
        }




        #region 歷程記錄
        ////天命法位歷程
        if (LB_HOPosition.Text != DDL_HRole.SelectedItem.Text)
        {
            InsertHistory.Insert("HPositionHistory", LB_HID.Text, LB_HOPosition.Text, DDL_HRole.SelectedItem.Text, ((Label)Master.FindControl("LB_HUserName")).Text);
        }

        ////區屬歷程
        if (LB_HOArea.Text != DDL_HAreaID.SelectedItem.Text)
        {
            InsertHistory.Insert("HAreaHistory", LB_HID.Text, LB_HOArea.Text, DDL_HAreaID.SelectedItem.Text, ((Label)Master.FindControl("LB_HUserName")).Text);
        }

        ////光團歷程
        if (LB_HOTeam.Text != DDL_HTeamID.SelectedItem.Text)
        {
            InsertHistory.Insert("HTeamHistory", LB_HID.Text, LB_HOTeam.Text, DDL_HTeamID.SelectedItem.Text, ((Label)Master.FindControl("LB_HUserName")).Text);
        }



        #endregion


        Response.Write("<script>alert('存檔成功!');window.location.href='HMember_Edit.aspx';</script>");
    }
    #endregion

    #region 取消
    protected void Btn_Cancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("HMember_Edit.aspx");
    }
    #endregion

    #region 收合切換
    protected void LBtn_Contact_Click(object sender, EventArgs e)
    {
        Panel_Contact.Visible = (Panel_Contact.Visible == false ? true : false);
        Panel_Education.Visible = false;
        Panel_Family.Visible = false;
        Panel_Others.Visible = false;
        Panel_Manage.Visible = false;
    }

    protected void LBtn_Education_Click(object sender, EventArgs e)
    {
        Panel_Education.Visible = (Panel_Education.Visible == false ? true : false);
        Panel_Contact.Visible = false;
        Panel_Family.Visible = false;
        Panel_Others.Visible = false;
        Panel_Manage.Visible = false;
    }

    protected void LBtn_Family_Click(object sender, EventArgs e)
    {
        Panel_Family.Visible = (Panel_Family.Visible == false ? true : false);
        Panel_Education.Visible = false;
        Panel_Contact.Visible = false;
        Panel_Others.Visible = false;
        Panel_Manage.Visible = false;
    }

    protected void LBtn_Others_Click(object sender, EventArgs e)
    {
        Panel_Others.Visible = (Panel_Others.Visible == false ? true : false);
        Panel_Education.Visible = false;
        Panel_Contact.Visible = false;
        Panel_Family.Visible = false;
        Panel_Manage.Visible = false;
    }

    protected void LBtn_Manage_Click(object sender, EventArgs e)
    {
        Panel_Manage.Visible = (Panel_Manage.Visible == false ? true : false);
        Panel_Education.Visible = false;
        Panel_Contact.Visible = false;
        Panel_Family.Visible = false;
        Panel_Others.Visible = false;
    }
    #endregion

    /// <summary>
    /// 格式判斷 或 存在判斷
    /// </summary>
    protected void FormCheck(object sender, EventArgs e)
    {
        //初始值
        int Result = 1;

        #region //格式--信箱
        //if (!string.IsNullOrEmpty(TB_HEmail.Text.Trim()))
        //{
        //	string a = TB_HEmail.Text.Trim();
        //	string pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        //	if (Regex.IsMatch(a, pattern))
        //	{
        //		LB_NoticeEmail.Visible = false;
        //	}
        //	else
        //	{
        //		LB_NoticeEmail.Visible = true;
        //		Result = 0;
        //	}
        //}
        //else
        //{
        //	Result = 0;
        //}
        #endregion

        #region  公務信箱
        if (!string.IsNullOrEmpty(TB_HOEmail.Text.Trim()))
        {
            string a = TB_HOEmail.Text.Trim();
            string pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            if (Regex.IsMatch(a, pattern))
            {
                LB_NoticeOEmail.Visible = false;
            }
            else
            {
                LB_NoticeOEmail.Visible = true;
                Result = 0;
            }
        }
        else
        {
            Result = 0;
        }
        #endregion

        #region //格式--密碼
        if (!string.IsNullOrEmpty(TB_HPassword.Text.Trim()))
        {
            string a = TB_HPassword.Text.Trim();
            //string pattern = @"^\S{5,20}$";
            // 5 到 20 位的混合英文和數字
            string pattern = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{5,20}$";
            if (Regex.IsMatch(a, pattern))
            {
                LB_NoticePassword.Visible = false;
            }
            else
            {
                LB_NoticePassword.Visible = true;
                Result = 0;
            }
        }
        else
        {
            Result = 0;
        }
        #endregion

        #region//格式--帳號
        //if (!string.IsNullOrEmpty(TB_HAccount.Text.Trim()))
        //{
        //	string a = TB_HAccount.Text.Trim();
        //	string pattern = @"^[a-zA-Z0-9]{5,20}$";
        //	if (Regex.IsMatch(a, pattern))
        //	{
        //		LB_NoticeAccount2.Visible = false;
        //	}
        //	else
        //	{
        //		LB_NoticeAccount2.Visible = true;
        //		Result = 0;
        //	}
        //}
        //else
        //{
        //	Result = 0;
        //}
        #endregion

        #region//格式--身分證字號
        if (!string.IsNullOrEmpty(TB_HPersonID.Text.ToUpper().Trim()))
        {
            string a = TB_HPersonID.Text.ToUpper().Trim();
            string pattern = @"^[A-Z][0-9]{9}$";
            if (Regex.IsMatch(a, pattern))
            {
                LB_NoticePersonID.Visible = false;
            }
            else
            {
                LB_NoticePersonID.Visible = true;
                Result = 0;
            }
        }
        else
        {
            Result = 0;
        }
        #endregion

        #region//判斷存在--信箱
        //if (!string.IsNullOrEmpty(TB_HEmail.Text.Trim()))
        //{
        //    using (SqlConnection con = new SqlConnection(ConStr))
        //    {
        //        SqlCommand cmd = new SqlCommand("SELECT HID,HEmail,HStatus FROM HMember WHERE HEmail=@HEmail AND HStatus IN('1','2')", con);
        //        con.Open();
        //        cmd.Parameters.AddWithValue("@HEmail", TB_HEmail.Text.Trim());
        //        SqlDataReader MyEBF = cmd.ExecuteReader();
        //        if (MyEBF.Read())
        //        {
        //            LB_NoticeEmail2.Visible = true;
        //            Result = 0;
        //        }
        //        else
        //        {
        //            LB_NoticeEmail2.Visible = false;
        //        }
        //        cmd.Cancel();
        //        MyEBF.Close();
        //    }
        //}
        //else
        //{
        //    Result = 0;
        //}
        #endregion

        #region    //判斷存在--帳號
        //if (!string.IsNullOrEmpty(TB_HEmail.Text.Trim()))
        //{
        //    using (SqlConnection con = new SqlConnection(ConStr))
        //    {
        //        SqlCommand cmd = new SqlCommand("SELECT HID,HAccount,HStatus FROM HMember WHERE HAccount=@HAccount AND HStatus IN('1','2')", con);
        //        con.Open();
        //        cmd.Parameters.AddWithValue("@HAccount", TB_HEmail.Text.Trim());
        //        SqlDataReader MyEBF = cmd.ExecuteReader();
        //        if (MyEBF.Read())
        //        {
        //            TB_HEmail.Visible = true;
        //            Result = 0;
        //        }
        //        else
        //        {
        //            LB_NoticeEmail.Visible = false;
        //        }
        //        cmd.Cancel();
        //        MyEBF.Close();
        //    }
        //}
        //else
        //{
        //    Result = 0;
        //}
        #endregion

    }

    #region 年齡計算
    public int GetAgeByBirthdate(DateTime birthdate)
    {
        DateTime now = DateTime.Now;
        int age = now.Year - birthdate.Year;
        if (now.Month < birthdate.Month || (now.Month == birthdate.Month && now.Day < birthdate.Day))
        {
            age--;
        }
        return age < 0 ? 0 : age;
    }
    #endregion

    #region 生日計算年齡
    protected void TB_HBirth_TextChanged(object sender, EventArgs e)
    {
        string pattern = @"(19|20)[0-9]{2}[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])";
        Regex regHtml = new Regex(pattern);

        Match mHtml = regHtml.Match(TB_HBirth.Text.Trim());

        if (!mHtml.Success)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('請輸入正確生日格式（例如：1995/05/15）');", true);
            return;
        }

        DateTime birthdate;
        if (DateTime.TryParseExact(TB_HBirth.Text.Trim(), "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out birthdate))
        {
            int age = GetAgeByBirthdate(DateTime.ParseExact(TB_HBirth.Text, "yyyy/MM/dd", null));
            TB_HAge.Text = Convert.ToString(age).ToString();
            LB_BirthNotice.Visible = false;
            TB_HBirth.Style.Add("border", "1px solid #ababab");
        }
        else
        {
            LB_BirthNotice.Visible = true;
            TB_HBirth.Style.Add("border", "2px solid #de4848");
            TB_HAge.Text = "";
        }
    }
    #endregion

    #region 國家選台灣身分證為必填
    protected void DDL_HCountryID_SelectedIndexChanged(object sender, EventArgs e)
    {
        JS_Register();

        if (DDL_HCountryID.SelectedValue == "85")  //台灣
        {
            personIDmust.Visible = true;
        }
    }
    #endregion

    #region --子女資料建立--
    //建立
    public void CreateTable()
    {
        if (ViewState["temp"] != null && !string.IsNullOrEmpty(ViewState["temp"].ToString()))
        {
            List<Children.list_item> Table = JsonConvert.DeserializeObject<List<Children.list_item>>(ViewState["temp"].ToString());
            Rpt_HChildren.DataSource = Table;
            Rpt_HChildren.DataBind();
        }
    }
    //新增
    protected void LBtn_HChildAdd_Click(object sender, EventArgs e)
    {
        UpdateTable();

        List<Children.list_item> Table = JsonConvert.DeserializeObject<List<Children.list_item>>(ViewState["temp"].ToString());
        Table.Add(new Children.list_item()
        {
            CName = TB_HCName_Add.Text.Trim(),
            CSex = DDL_HCSex_Add.SelectedValue,
            CFellow = RBL_HCFellow_Add.SelectedValue,
            CLife = RBL_HCLife_Add.SelectedValue,
            CLTogether = RBL_HCLTogether_Add.SelectedValue,
        });
        ViewState["temp"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
        CreateTable();

        TB_HCName_Add.Text = null;
        DDL_HCSex_Add.SelectedValue = "0";
        RBL_HCFellow_Add.SelectedValue = "1";
        RBL_HCLife_Add.SelectedValue = "1";
        RBL_HCLTogether_Add.SelectedValue = "1";
    }

    //更新
    public void UpdateTable()
    {
        List<Children.list_item> Table = JsonConvert.DeserializeObject<List<Children.list_item>>(ViewState["temp"].ToString());
        int x = 0;
        foreach (Children.list_item Element in Table)
        {
            if (((TextBox)Rpt_HChildren.Items[x].FindControl("TB_HCName_Edit")).Text != Element.CName) //姓名
            {
                Element.CName = ((TextBox)Rpt_HChildren.Items[x].FindControl("TB_HCName_Edit")).Text.Trim();
            }

            if (((DropDownList)Rpt_HChildren.Items[x].FindControl("DDL_HCSex_Edit")).SelectedValue != Element.CSex) //性別
            {
                Element.CSex = ((DropDownList)Rpt_HChildren.Items[x].FindControl("DDL_HCSex_Edit")).SelectedValue;
            }

            if (((RadioButtonList)Rpt_HChildren.Items[x].FindControl("RBL_HCFellow_Edit")).SelectedValue != Element.CFellow)  //同修
            {
                Element.CFellow = ((RadioButtonList)Rpt_HChildren.Items[x].FindControl("RBL_HCFellow_Edit")).SelectedValue;
            }

            if (((RadioButtonList)Rpt_HChildren.Items[x].FindControl("RBL_HCLife_Edit")).SelectedValue != Element.CLife)  //存歿
            {
                Element.CLife = ((RadioButtonList)Rpt_HChildren.Items[x].FindControl("RBL_HCLife_Edit")).SelectedValue;
            }

            if (((RadioButtonList)Rpt_HChildren.Items[x].FindControl("RBL_HCLTogether_Edit")).SelectedValue != Element.CLife)  //同住
            {
                Element.CLife = ((RadioButtonList)Rpt_HChildren.Items[x].FindControl("RBL_HCLTogether_Edit")).SelectedValue;
            }


            x++;
        }
        ViewState["temp"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
    }



    //刪除
    protected void LBtn_HChildDelete_Click(object sender, EventArgs e)
    {
        UpdateTable();
        LinkButton Del = sender as LinkButton;
        string Del_CA = Del.CommandArgument;
        List<Children.list_item> Table = JsonConvert.DeserializeObject<List<Children.list_item>>(ViewState["temp"].ToString());
        Table.RemoveAt(Convert.ToInt32(Del_CA));
        ViewState["temp"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
        CreateTable();

    }

    //模型
    public class Children
    {
        public class list_item
        {
            public string CName { get; set; }
            public string CSex { get; set; }
            public string CFellow { get; set; }
            public string CLife { get; set; }
            public string CLTogether { get; set; }
        }
        public List<list_item> ChildrenList { get; set; }
    }
    #endregion

    #region 搜尋
    protected void LBtn_Search_Click(object sender, EventArgs e)
    {
        string SearchSQL = "SELECT HMember.HID, HSeries,HAccount, HUserName,HSystemID, HAreaID,HTeamID, HTeamType, HPeriod, HEmail, HPhone, HSystemName, HArea, HMember.HStatus,HType, HMType.HMType,case when HMemberID is null then '沒參班過' else '參過班' end '參班' From HMember Left JOIN HSystem ON HMember.HSystemID = HSystem.HID  Left JOIN HArea ON HMember.HAreaID = HArea.HID LEFT JOIN HMType ON HMember.HType = HMType.HID LEFT JOIN (　SELECT DISTINCT HMemberID    FROM HCourseBooking) b ON b.HMemberID = HMember.HID ";

        //搜尋條件
        StringBuilder sql = new StringBuilder(SearchSQL);
        List<string> WHERE = new List<string>();

        //排序規則
        string Order = " ORDER BY HMember.HStatus DESC,HMember.HAreaID ASC";

        //關鍵字
        if (!string.IsNullOrEmpty(TB_Search.Text.Trim()))
        {
            WHERE.Add(" (HUserName LIKE N'%" + TB_Search.Text.Trim() + "%' OR HPeriod LIKE N'%" + TB_Search.Text.Trim() + "%' OR HPhone LIKE N'%" + TB_Search.Text.Trim() + "%'  OR HAccount LIKE N'%" + TB_Search.Text.Trim() + "%')");
        }

        //區屬
        //判斷是否為最高權限&是否可以跨區
        SqlDataReader QueryRole = SQLdatabase.ExecuteReader("SELECT HID, HMemberID, HCrossRegion  FROM HRole  WHERE (HMemberID like '%" + "," + Session["HUserHID"] + "," + "%')");
        while (QueryRole.Read())  //是最高權限者或可以跨區才能搜尋區屬
        {
            if (QueryRole["HID"].ToString() == "1")  //最高權限
            {
                if (DDL_SHArea.SelectedValue != "0")
                {
                    WHERE.Add(" HAreaID = '" + DDL_SHArea.SelectedValue + "'");
                }
            }

            if (QueryRole["HCrossRegion"].ToString() == "True")  //可以跨區
            {
                if (DDL_SHArea.SelectedValue != "0")
                {
                    WHERE.Add(" HAreaID = '" + DDL_SHArea.SelectedValue + "'");
                }
            }

        }
        QueryRole.Close();

        //狀態
        if (DDL_Status.SelectedValue != "0")
        {
            if (DDL_Status.SelectedValue == "2")
            {
                WHERE.Add(" HMember.HStatus='0'");
            }
            else
            {
                WHERE.Add(" HMember.HStatus='" + DDL_Status.SelectedValue + "'");
            }
        }


        //判斷使用者是否選擇了條件
        if (WHERE.Count > 0)
        {
            string wh = string.Join(" AND ", WHERE.ToArray());
            sql.Append(" WHERE " + wh);
        }

        SDS_Member.SelectCommand = sql.ToString() + Order;

        Console.Write("<script>console.log('"+ sql.ToString() + Order + "')</script>");
        // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
        Pg_Paging.PagingLoad("HochiSystemConnection", SDS_Member.SelectCommand, PageMax, LastPage, true, Rpt_Member);
        ViewState["Select"] = SDS_Member.SelectCommand;
    }
    #endregion

    #region 取消搜尋
    protected void LBtn_SearchCancel_Click(object sender, EventArgs e)
    {
        if (Session["HUserHID"] != null)
        {
            #region 判斷是否為最高權限者或可以跨區
            SqlConnection dbConn;
            SqlCommand dbCmd;
            string strDBConn, strSQL;
            strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
            dbConn = new SqlConnection(strDBConn);
            dbConn.Open();
            //判斷是否為最高權限者
            strSQL = "SELECT HMemberID  FROM HRole  WHERE HID=1 AND (HMemberID like '%" + "," + Session["HUserHID"] + "," + "%')";
            dbCmd = new SqlCommand(strSQL, dbConn);
            SqlDataReader MyQuery;
            MyQuery = dbCmd.ExecuteReader();

            if (MyQuery.Read())
            {
                DDL_SHArea.SelectedValue = "0";
                DDL_SHArea.Enabled = true;
                LB_HArea.Text = "0";

                SDS_Member.SelectCommand = "SELECT HMember.HID, HSeries,HAccount, HUserName,HSystemID, HAreaID,HTeamID, HTeamType, HPeriod, HEmail, HPhone, HSystemName, HArea, HMember.HStatus,HType, HMType.HMType,case when HMemberID is null then '沒參班過' else '參過班' end '參班' From HMember Left JOIN HSystem ON HMember.HSystemID = HSystem.HID  Left JOIN HArea ON HMember.HAreaID = HArea.HID LEFT JOIN HMType ON HMember.HType = HMType.HID LEFT JOIN (　SELECT DISTINCT HMemberID    FROM HCourseBooking) b ON b.HMemberID = HMember.HID ORDER BY HStatus DESC,HMember.HAreaID ASC";
                // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
                Pg_Paging.PagingLoad("HochiSystemConnection", SDS_Member.SelectCommand, PageMax, LastPage, false, Rpt_Member);
                ViewState["Select"] = SDS_Member.SelectCommand;
            }
            else
            {
                //221229-加入判斷是否擁有跨區權限
                SqlDataReader MPermission = SQLdatabase.ExecuteReader("SELECT  HRAccess, (M.HCrossRegion+',') AS CrossRegion FROM ( SELECT HMemberID,(SELECT DISTINCT(','+HRAccess) FROM HRole WHERE HMemberID like '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%' FOR XML PATH('')) AS HRAccess, (SELECT DISTINCT(','+cast(HCrossRegion AS NVARCHAR)) FROM HRole WHERE HMemberID like '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%' FOR XML PATH('')) AS HCrossRegion FROM HRole AS b) M WHERE M.HMemberID like '%," + ((Label)Master.FindControl("LB_HUserHID")).Text + ",%' AND (M.HCrossRegion+',') LIKE '%,1,%' GROUP BY HRAccess, HCrossRegion");

                if (MPermission.Read())
                {
                    DDL_SHArea.Enabled = true;
                    DDL_SHArea.SelectedValue = "0";

                    SDS_Member.SelectCommand = "SELECT HMember.HID, HSeries,HAccount, HUserName,HSystemID, HAreaID,HTeamID, HTeamType, HPeriod, HEmail, HPhone, HSystemName, HArea, HMember.HStatus,HType, HMType.HMType,case when HMemberID is null then '沒參班過' else '參過班' end '參班' From HMember Left JOIN HSystem ON HMember.HSystemID = HSystem.HID  Left JOIN HArea ON HMember.HAreaID = HArea.HID LEFT JOIN HMType ON HMember.HType = HMType.HID LEFT JOIN (　SELECT DISTINCT HMemberID    FROM HCourseBooking) b ON b.HMemberID = HMember.HID ORDER BY HStatus DESC,HMember.HAreaID ASC";
                    // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
                    Pg_Paging.PagingLoad("HochiSystemConnection", SDS_Member.SelectCommand, PageMax, LastPage, false, Rpt_Member);
                    ViewState["Select"] = SDS_Member.SelectCommand;
                }
                else
                {
                    #region //只能看自己區屬的學員資料

                    //string AreaID = null;
                    SqlDataReader QueryHAreaID = SQLdatabase.ExecuteReader("SELECT HAreaID FROM HMember where HID='" + Session["HUserHID"].ToString() + "'");
                    while (QueryHAreaID.Read())
                    {
                        //AreaID = QueryHAreaID["HAreaID"].ToString();
                        LB_HArea.Text = QueryHAreaID["HAreaID"].ToString();
                        DDL_SHArea.Enabled = false;

                    }
                    QueryHAreaID.Close();

                    //220822--同區屬才能看
                    SDS_Member.SelectCommand = "SELECT HMember.HID, HSeries,HAccount, HUserName,HSystemID, HAreaID,HTeamID, HTeamType, HPeriod, HEmail, HPhone, HSystemName, HArea, HMember.HStatus,HType, HMType.HMType,case when HMemberID is null then '沒參班過' else '參過班' end '參班' From HMember Left JOIN HSystem ON HMember.HSystemID = HSystem.HID  Left JOIN HArea ON HMember.HAreaID = HArea.HID LEFT JOIN HMType ON HMember.HType = HMType.HID LEFT JOIN (　SELECT DISTINCT HMemberID    FROM HCourseBooking) b ON b.HMemberID = HMember.HID WHERE HMember.HAreaID='" + LB_HArea.Text + "'  ORDER BY HStatus DESC,HMember.HAreaID ASC";
                    // 呼叫分頁(連結資料庫, 資料庫命令, Repeater顯示總數, 分頁頁數, 觸發搜尋, DataList控件)
                    Pg_Paging.PagingLoad("HochiSystemConnection", SDS_Member.SelectCommand, PageMax, LastPage, false, Rpt_Member);
                    ViewState["Select"] = SDS_Member.SelectCommand;

                    DDL_SHArea.SelectedValue = LB_HArea.Text;
                    #endregion
                }
            }
            MyQuery.Close();


            #endregion
        }
        else
        {
            if (Request.Cookies["HochiInfo"] == null)
            {
                Response.Write("<script>alert('連線逾時，請重新登入!');window.location.href='../../HLogin.aspx';</script>");
            }

        }


        TB_Search.Text = "";

        DDL_Status.SelectedValue = "0";

    }
    #endregion

    #region 上傳照片 (.gif, .jpg, .bmp, .png)
    protected void Upload_Pic()
    {
        bool fileIsValid = false;
        if (FU_HImg.HasFile)
        {
            //取得上傳文件的副檔名
            String fileExtension = Path.GetExtension(FU_HImg.FileName).ToLower();
            String[] restrictExtension = { ".gif", ".jpg", ".bmp", ".png", ".jpeg" };
            foreach (string i in restrictExtension)
            {
                if (fileExtension == i)
                {
                    fileIsValid = true;
                    break;
                }
            }
            if (fileIsValid)
            {
                //上傳檔是否大於10M
                if (FU_HImg.PostedFile.ContentLength > (10 * 1024 * 1024))
                {
                    Response.Write("<script>alert('上傳的圖片不能超過10MB喔！');</script>");
                    return;
                }

                //建立資料夾
                Directory.CreateDirectory(ImgRoot);

                //檔名
                LB_Pic.Text = "HeadImg_" + DateTime.Now.ToString("yyMMddmmss") + fileExtension;

                this.FU_HImg.SaveAs(Server.MapPath("~/uploads/Account/" + LB_Pic.Text));
            }
            else
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert", "<script>alert('只能上傳.gif.jpg.bmp.png的圖檔喔！');</script>");
            }
        }
    }
    #endregion

    #region--移除圖片-
    protected void Btn_Del_Click(object sender, EventArgs e)
    {
        Btn_Del.Visible = false;
        IMG_Pic.ImageUrl = "";
        IMG_Pic.Width = 0;
    }
    #endregion

    #region 註冊JS 
    protected void JS_Register()
    {
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS1", ("$(function(){$('.datepicker').datepicker({format:'yyyy/mm/dd',autoclose:true,toggleActive:false,todayHighlight:true,orientation:'bottomauto',});});"), true);
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS2", ("$(function(){$('.js-example-basic-single').select2();});"), true);
        ScriptManager.RegisterStartupScript(Page, this.GetType(), "JS3", ("$('.ListB_Multi').SumoSelect({search: true,placeholder: '請選擇',	csvDispCount: 5,});"), true);
    }
    #endregion

    #region  職業類別切換
    protected void DDL_HWorkType_SelectedIndexChanged(object sender, EventArgs e)
    {
        SDS_WTItem.SelectCommand = "SELECT HID, HWType, HWTItemName FROM HWTItem WHERE HWType=" + DDL_HWorkType.SelectedValue;
        DDL_HWTItem.DataBind();

        if (DDL_HWorkType.SelectedItem.Text == "其他")
        {
            TB_HWTOthers.Visible = true;
            DDL_HWTItem.Visible = false;
        }
        else
        {
            TB_HWTOthers.Visible = false;
            DDL_HWTItem.Visible = true;
        }
    }
    #endregion

    #region  專長主類別切換
    protected void DDL_EPType_SelectedIndexChanged(object sender, EventArgs e)
    {
        //SDS_HEPItem.SelectCommand = "SELECT HID, HEPType, HEPItem FROM   HEPItem WHERE HEPType=" + DDL_EPType.SelectedValue;
        SDS_HEPItem.SelectCommand = "SELECT HID, HEPType, HEPItem FROM HEPItem ORDER BY HID ASC";
        LBox_HEPItem.DataBind();
    }
    #endregion

    #region -志願服務建立-

    //建立
    public void CreateVolunteer()
    {
        if (ViewState["Volunteer"] != null && !string.IsNullOrEmpty(ViewState["Volunteer"].ToString()))
        {
            List<Volunteer.Volunteer_item> Table = JsonConvert.DeserializeObject<List<Volunteer.Volunteer_item>>(ViewState["Volunteer"].ToString());
            Rpt_HVolunteer.DataSource = Table;
            Rpt_HVolunteer.DataBind();
        }
    }

    //新增
    protected void LBtn_HVolunteer_Add_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_SName_Add.Text) || string.IsNullOrEmpty(TB_SUnit_Add.Text))
        {
            ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert", "<script>alert('請輸入志願服務名稱和服務單位哦~');</script>");
            return;
        }

        UpdateVolunteer();

        List<Volunteer.Volunteer_item> Table = JsonConvert.DeserializeObject<List<Volunteer.Volunteer_item>>(ViewState["Volunteer"].ToString());
        Table.Add(new Volunteer.Volunteer_item()
        {
            SName = TB_SName_Add.Text.Trim(),
            SUnit = TB_SUnit_Add.Text.Trim(),
        });
        ViewState["Volunteer"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
        CreateVolunteer();

        TB_SName_Add.Text = null;
        TB_SUnit_Add.Text = null;
    }

    //刪除
    protected void LBtn_HVolunteerDel_Click(object sender, EventArgs e)
    {
        UpdateVolunteer();
        LinkButton Del = sender as LinkButton;
        string Del_CA = Del.CommandArgument;
        List<Volunteer.Volunteer_item> Table = JsonConvert.DeserializeObject<List<Volunteer.Volunteer_item>>(ViewState["Volunteer"].ToString());
        Table.RemoveAt(Convert.ToInt32(Del_CA));
        ViewState["Volunteer"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
        CreateVolunteer();
    }

    //更新
    public void UpdateVolunteer()
    {
        List<Volunteer.Volunteer_item> Table = JsonConvert.DeserializeObject<List<Volunteer.Volunteer_item>>(ViewState["Volunteer"].ToString());
        int x = 0;
        foreach (Volunteer.Volunteer_item Element in Table)
        {
            if (((TextBox)Rpt_HVolunteer.Items[x].FindControl("TB_SName_Edit")).Text != Element.SName)  //名稱
            {
                Element.SName = ((TextBox)Rpt_HVolunteer.Items[x].FindControl("TB_SName_Edit")).Text.Trim();
            }

            if (((TextBox)Rpt_HVolunteer.Items[x].FindControl("TB_SUnit_Edit")).Text != Element.SUnit)  //單位
            {
                Element.SUnit = ((TextBox)Rpt_HVolunteer.Items[x].FindControl("TB_SUnit_Edit")).Text.Trim();
            }

            x++;
        }
        ViewState["Volunteer"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
    }


    //模型
    public class Volunteer
    {
        public class Volunteer_item
        {
            public string SName { get; set; }
            public string SUnit { get; set; }
        }
        public List<Volunteer_item> VolunteerList { get; set; }
    }
    #endregion

    #region -法門建立-

    //建立
    public void CreateFamen()
    {
        if (ViewState["Famen"] != null && !string.IsNullOrEmpty(ViewState["Famen"].ToString()))
        {
            List<Famen.Famen_item> Table = JsonConvert.DeserializeObject<List<Famen.Famen_item>>(ViewState["Famen"].ToString());
            Rpt_HFamen.DataSource = Table;
            Rpt_HFamen.DataBind();
        }
    }

    //新增
    protected void LBtn_HFamen_Add_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_HFamen_Add.Text))
        {
            ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert", "<script>alert('請輸入法門名稱哦~');</script>");
            return;
        }

        UpdateFamen();

        List<Famen.Famen_item> Table = JsonConvert.DeserializeObject<List<Famen.Famen_item>>(ViewState["Famen"].ToString());
        Table.Add(new Famen.Famen_item()
        {
            Famen = TB_HFamen_Add.Text.Trim(),
        });
        ViewState["Famen"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
        CreateFamen();

        TB_HFamen_Add.Text = null;
    }

    //刪除
    protected void LBtn_HFamenDel_Click(object sender, EventArgs e)
    {
        UpdateFamen();
        LinkButton Del = sender as LinkButton;
        string Del_CA = Del.CommandArgument;
        List<Famen.Famen_item> Table = JsonConvert.DeserializeObject<List<Famen.Famen_item>>(ViewState["Famen"].ToString());
        Table.RemoveAt(Convert.ToInt32(Del_CA));
        ViewState["Famen"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
        CreateFamen();
    }

    //更新
    public void UpdateFamen()
    {
        List<Famen.Famen_item> Table = JsonConvert.DeserializeObject<List<Famen.Famen_item>>(ViewState["Famen"].ToString());
        int x = 0;
        foreach (Famen.Famen_item Element in Table)
        {
            if (((TextBox)Rpt_HFamen.Items[x].FindControl("TB_HFamen_Edit")).Text != Element.Famen)  //名稱
            {
                Element.Famen = ((TextBox)Rpt_HFamen.Items[x].FindControl("TB_HFamen_Edit")).Text.Trim();
            }

            x++;
        }
        ViewState["Famen"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
    }


    //模型
    public class Famen
    {
        public class Famen_item
        {
            public string Famen { get; set; }
        }
        public List<Famen_item> FamenList { get; set; }
    }
    #endregion

    #region -團體建立-
    //建立
    public void CreateGroup()
    {
        if (ViewState["Group"] != null && !string.IsNullOrEmpty(ViewState["Group"].ToString()))
        {
            List<Group.Group_item> Table = JsonConvert.DeserializeObject<List<Group.Group_item>>(ViewState["Group"].ToString());
            Rpt_HGroup.DataSource = Table;
            Rpt_HGroup.DataBind();
        }
    }

    //新增
    protected void LBtn_HGroup_Add_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_GName_Add.Text) || string.IsNullOrEmpty(TB_Job_Add.Text))
        {
            ClientScript.RegisterClientScriptBlock(this.GetType(), "Alert", "<script>alert('請輸入團體名稱與擔任職務哦~');</script>");
            return;
        }

        UpdateGroup();

        List<Group.Group_item> Table = JsonConvert.DeserializeObject<List<Group.Group_item>>(ViewState["Group"].ToString());
        Table.Add(new Group.Group_item()
        {
            GType = DDL_GType_Add.Text.Trim(),
            GOtherType = TB_HGOthers_Add.Text.Trim(),
            GName = TB_GName_Add.Text.Trim(),
            Duration = TB_Duration_Add.Text.Trim(),
            Job = TB_Job_Add.Text.Trim(),
            JobLevel = DDL_JobLevel_Add.Text.Trim(),
        });
        ViewState["Group"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
        CreateGroup();

        DDL_GType_Add.SelectedValue = "0";
        TB_HGOthers_Add.Text = null;
        TB_GName_Add.Text = null;
        TB_Duration_Add.Text = null;
        TB_Job_Add.Text = null;
        DDL_JobLevel_Add.SelectedValue = "0";
    }

    //刪除
    protected void LBtn_HGroupDel_Click(object sender, EventArgs e)
    {
        UpdateGroup();
        LinkButton Del = sender as LinkButton;
        string Del_CA = Del.CommandArgument;
        List<Group.Group_item> Table = JsonConvert.DeserializeObject<List<Group.Group_item>>(ViewState["Group"].ToString());
        Table.RemoveAt(Convert.ToInt32(Del_CA));
        ViewState["Group"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
        CreateGroup();
    }

    //更新
    public void UpdateGroup()
    {
        List<Group.Group_item> Table = JsonConvert.DeserializeObject<List<Group.Group_item>>(ViewState["Group"].ToString());
        int x = 0;
        foreach (Group.Group_item Element in Table)
        {
            if (((DropDownList)Rpt_HGroup.Items[x].FindControl("DDL_GType_Edit")).SelectedValue != Element.GType)
            {
                Element.GType = ((DropDownList)Rpt_HGroup.Items[x].FindControl("DDL_GType_Edit")).SelectedValue;
            }

            if (((TextBox)Rpt_HGroup.Items[x].FindControl("TB_HGOthers_Edit")).Text != Element.GOtherType)
            {
                Element.GOtherType = ((TextBox)Rpt_HGroup.Items[x].FindControl("TB_HGOthers_Edit")).Text.Trim();
            }

            if (((TextBox)Rpt_HGroup.Items[x].FindControl("TB_GName_Edit")).Text != Element.GName)
            {
                Element.GName = ((TextBox)Rpt_HGroup.Items[x].FindControl("TB_GName_Edit")).Text.Trim();
            }

            if (((TextBox)Rpt_HGroup.Items[x].FindControl("TB_Duration_Edit")).Text != Element.Duration)
            {
                Element.Duration = ((TextBox)Rpt_HGroup.Items[x].FindControl("TB_Duration_Edit")).Text.Trim();
            }

            if (((TextBox)Rpt_HGroup.Items[x].FindControl("TB_Job_Edit")).Text != Element.Job)
            {
                Element.Job = ((TextBox)Rpt_HGroup.Items[x].FindControl("TB_Job_Edit")).Text.Trim();
            }

            if (((DropDownList)Rpt_HGroup.Items[x].FindControl("DDL_JobLevel_Edit")).SelectedValue != Element.JobLevel)
            {
                Element.JobLevel = ((DropDownList)Rpt_HGroup.Items[x].FindControl("DDL_JobLevel_Edit")).SelectedValue;
            }
            x++;
        }
        ViewState["Group"] = JsonConvert.SerializeObject(Table, Formatting.Indented);
    }


    //模型
    public class Group
    {
        public class Group_item
        {
            public string GType { get; set; }
            public string GOtherType { get; set; }
            public string GName { get; set; }
            public string Duration { get; set; }
            public string Job { get; set; }
            public string JobLevel { get; set; }

        }
        public List<Group_item> GroupList { get; set; }
    }

    protected void DDL_GType_Add_SelectedIndexChanged(object sender, EventArgs e)
    {

        TB_HGOthers_Add.Visible = DDL_GType_Add.SelectedValue == "5" ? true : false;

    }

    protected void Rpt_HGroup_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        ((TextBox)e.Item.FindControl("TB_HGOthers_Edit")).Visible = ((DropDownList)e.Item.FindControl("DDL_GType_Edit")).SelectedValue == "5" ? true : false;
    }
    #endregion

    #region //檢查身分證字號格式
    public bool CheckId(string user_id)
    {
        int[] uid = new int[10]; //數字陣列存放身分證字號用
        int chkTotal; //計算總和用

        //初始值
        int Result = 1;

        if (user_id.Length == 10) //檢查長度
        {
            user_id = user_id.ToUpper(); //將身分證字號英文改為大寫

            //將輸入的值存入陣列中
            for (int i = 1; i < user_id.Length; i++)
            {
                uid[i] = Convert.ToInt32(user_id.Substring(i, 1));
            }
            //將開頭字母轉換為對應的數值
            switch (user_id.Substring(0, 1).ToUpper())
            {
                case "A": uid[0] = 10; break;
                case "B": uid[0] = 11; break;
                case "C": uid[0] = 12; break;
                case "D": uid[0] = 13; break;
                case "E": uid[0] = 14; break;
                case "F": uid[0] = 15; break;
                case "G": uid[0] = 16; break;
                case "H": uid[0] = 17; break;
                case "I": uid[0] = 34; break;
                case "J": uid[0] = 18; break;
                case "K": uid[0] = 19; break;
                case "L": uid[0] = 20; break;
                case "M": uid[0] = 21; break;
                case "N": uid[0] = 22; break;
                case "O": uid[0] = 35; break;
                case "P": uid[0] = 23; break;
                case "Q": uid[0] = 24; break;
                case "R": uid[0] = 25; break;
                case "S": uid[0] = 26; break;
                case "T": uid[0] = 27; break;
                case "U": uid[0] = 28; break;
                case "V": uid[0] = 29; break;
                case "W": uid[0] = 32; break;
                case "X": uid[0] = 30; break;
                case "Y": uid[0] = 31; break;
                case "Z": uid[0] = 33; break;
            }
            //檢查第一個數值是否為1.2(判斷性別)
            if (uid[1] == 1 || uid[1] == 2)
            {
                chkTotal = (uid[0] / 10 * 1) + (uid[0] % 10 * 9);

                int k = 8;
                for (int j = 1; j < 9; j++)
                {
                    chkTotal += uid[j] * k;
                    k--;
                }

                chkTotal += uid[9];

                if (chkTotal % 10 != 0)
                {
                    return false;
                    //Response.Write("身分證字號錯誤");
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
                //Response.Write("身分證字號錯誤");
            }
        }
        else
        {
            return false;
            //Response.Write("身分證字號長度錯誤");
        }

    }
    #endregion


    //230209-加入
    protected void TB_HPersonID_TextChanged(object sender, EventArgs e)
    {
        #region
        if (!string.IsNullOrEmpty(TB_HPersonID.Text.ToUpper().Trim()))
        {

            //重複判斷
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);
            SqlCommand cmd = new SqlCommand("SELECT HPersonID FROM HMember WHERE HPersonID=@HPersonID AND HAccount <> @HAccount", con);
            con.Open();
            cmd.Parameters.AddWithValue("@HPersonID", TB_HPersonID.Text.ToUpper().Trim());
            cmd.Parameters.AddWithValue("@HAccount", TB_HEmail.Text.Trim());
            SqlDataReader MyEBF = cmd.ExecuteReader();
            if (MyEBF.Read())
            {
                //Response.Write("aaaa");
                //Response.End();
                LB_NoticePersonID.Visible = true;
                LB_NoticePersonID.Text = "此身分證已存在";
            }
            else  //沒有重複再判斷格式
            {
                //格式判斷
                if (CheckId(TB_HPersonID.Text.ToUpper().Trim()) == false)
                {
                    LB_NoticePersonID.Visible = true;
                    LB_NoticePersonID.Text = "身分證格式有誤";
                }
                else
                {
                    LB_NoticePersonID.Visible = false;
                }

            }
            MyEBF.Close();
            con.Close();
            cmd.Cancel();
        }




        #endregion
    }


    #region 三載光
    protected void DDL_HCarrier_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DDL_HCarrier.SelectedValue != "0" && DDL_HRainbow.SelectedValue != "0")
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "light_data.json");
            LightDataCache.LoadData(path);

            var allData = LightDataCache.GetAll();
            var first = LightDataCache.GetById(1);

            var result = LightDataCache.GetAll().Find(light => light.HCarrier == Convert.ToInt32(DDL_HCarrier.SelectedValue) && light.HRainbow == Convert.ToInt32(DDL_HRainbow.SelectedValue));

            LB_HLightName.Text = result.HLightName;
        }
        else
        {
            LB_HLightName.Text = "未定光系";
        }
    }
    #endregion

    #region 七彩光
    protected void DDL_HRainbow_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DDL_HCarrier.SelectedValue != "0" && DDL_HRainbow.SelectedValue != "0")
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "light_data.json");
            LightDataCache.LoadData(path);

            var allData = LightDataCache.GetAll();
            var first = LightDataCache.GetById(1);

            var result = LightDataCache.GetAll().Find(light => light.HCarrier == Convert.ToInt32(DDL_HCarrier.SelectedValue) && light.HRainbow == Convert.ToInt32(DDL_HRainbow.SelectedValue));

            LB_HLightName.Text = result.HLightName;
        }
        else
        {
            LB_HLightName.Text = "未定光系";
        }
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