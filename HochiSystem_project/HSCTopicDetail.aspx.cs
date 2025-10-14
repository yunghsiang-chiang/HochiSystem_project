

using MySql.Data.MySqlClient;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WebGrease.Css.Extensions;
using Color = System.Drawing.Color;
using Image = System.Web.UI.WebControls.Image;
using Path = System.IO.Path;

public partial class HSCTopicDetail : System.Web.UI.Page
{
    string EIPconnStr = ConfigurationManager.ConnectionStrings["HochiEIPConnection"].ConnectionString;
    SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    #region --根目錄--
    string SCFile = "~/uploads/SpecialColumn/";

    string File1Root = "D:\\Website\\System\\HochiSystem\\uploads\\HSCTMsg\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";

    string ACMaterialRoot = "uploads/ACourseMaterial/UPart/";
    #endregion


    #region --EIP根目錄--
    string EIPFile = "https://eip.hochi.org.tw/ulfiles/laoshi/";
    #endregion

    #region 回應主題&回應留言SQL SelectCommand String
    string HSCTMsgCom = "SELECT  a.HID, a.HSCTopicID, a.HMemberID, a.HContent, a.HFile1, a.HVideoLink, a.HStatus, a.HCreate, a.HCreateDT, a.HQuestionYN, (b.HSystemName+' '+b.HArea+'<br/>'+b.HPeriod+' '+b.HUserName) AS HUserName, c.HIMG FROM HSCTMsg AS a LEFT JOIN MemberList AS b ON a.HMemberID=b.HID LEFT JOIN HMember AS c ON a.HMemberID=c.HID";

    string HSCTMsgResponseCom = "SELECT a.HID, a.HSCTMsgID, a.HMemberID, a.HMsgResponse, a.HVoice, a.HStatus, a.HCreate,  a.HCreateDT, (b.HSystemName+' '+b.HArea+'<br/>'+b.HPeriod+' '+b.HUserName) AS HUserName, c.HIMG FROM HSCTMsgResponse AS a LEFT JOIN MemberList AS b ON a.HMemberID=b.HID LEFT JOIN HMember AS c ON a.HMemberID=c.HID";
    #endregion

    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        #region 數量計算

        //瀏覽次數
        LB_SeenNum.Text = ViewCounts(LB_TID.Text).ToString("N0");

        int MsgSum = 0;
        MsgSum += Rpt_HSCTMsg.Items.Count; //主題-留言數量計算(回應主題)


        //主題-心情數量計算
        int TopicThumbUpSum = 0, TopicHeartSum = 0, TopicSmile = 0;
        SqlDataReader TopicMood = SQLdatabase.ExecuteReader("SELECT HID, HSCTopicID, HMemberID, HType FROM HSCTopic_Mood WHERE HSCTopicID='" + LB_TID.Text + "'");


        while (TopicMood.Read())
        {
            if (TopicMood["HType"].ToString() == "1")
            {
                TopicThumbUpSum++;
            }
            else if (TopicMood["HType"].ToString() == "2")
            {
                TopicHeartSum++;
            }
            else if (TopicMood["HType"].ToString() == "3")
            {
                TopicSmile++;
            }
        }
        TopicMood.Close();

        LB_ThumbsUpNum.Text = TopicThumbUpSum.ToString("N0");
        LB_LoveNum.Text = TopicHeartSum.ToString("N0");
        LB_SmileNum.Text = TopicSmile.ToString("N0");



        for (int x = 0; x < Rpt_HSCTMsg.Items.Count; x++)
        {
            ////主題-留言數量計算(回應留言) (不含回應留言的數量，要與外層的統計相符)
            ////GE20240817_改用function
            //Repeater Rpt_HSCTMsgResponse = Rpt_HSCTMsg.Items[x].FindControl("Rpt_HSCTMsgResponse") as Repeater;  //找到Repeater物件
            //MsgSum += Rpt_HSCTMsgResponse.Items.Count;



            //回應主題-心情數量計算
            int MsgThumbUpSum = 0, MsgHeartSum = 0, MsgSmile = 0;
            SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT a.HID, a.HSCTMsgID, a.HMemberID, a.HType, b.HID FROM HSCTMsg_Mood AS a INNER JOIN HSCTMsg AS b ON a.HSCTMsgID=b.HID WHERE b.HSCTopicID='" + LB_TID.Text + "' AND  a.HSCTMsgID='" + ((Label)Rpt_HSCTMsg.Items[x].FindControl("LB_HSCTMsgID")).Text + "'");

            while (dr.Read())
            {
                if (dr["HType"].ToString() == "1")
                {
                    MsgThumbUpSum++;
                }
                else if (dr["HType"].ToString() == "2")
                {
                    MsgHeartSum++;
                }
                else if (dr["HType"].ToString() == "3")
                {
                    MsgSmile++;
                }
            }
            dr.Close();

            ((Label)Rpt_HSCTMsg.Items[x].FindControl("LB_MsgThumbUpSum")).Text = MsgThumbUpSum.ToString("N0");
            ((Label)Rpt_HSCTMsg.Items[x].FindControl("LB_MsgHeartSum")).Text = MsgHeartSum.ToString("N0");
            ((Label)Rpt_HSCTMsg.Items[x].FindControl("LB_MsgSmileSum")).Text = MsgSmile.ToString("N0");
        }

        LB_MsgSum.Text = MsgSum.ToString("N0"); //主題-留言數量計算(回應主題)

        #endregion

        //GA20240817_主題心情、回應、瀏覽次數、分享次數計算
        //FeelingCounts(LB_TID.Text);
        ResponseCounts(LB_TID.Text);
        ViewCounts(LB_TID.Text);
        ShareCounts(LB_TID.Text);

        if (Rpt_HSCTMsg.Items.Count == 0)
        {
            Div_TopicMsg.Visible = false;
        }
        else
        {
            Div_TopicMsg.Visible = true;
        }

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        #region 頁面初始化
        if (!IsPostBack)
        {

            if (Request.QueryString["TID"] != null)
            {
                if (Request.QueryString["F"] != null)
                {
                    if (Request.QueryString["F"].ToString() == "0")
                    {
                        if (!string.IsNullOrEmpty(Request.QueryString["TID"].ToString()))
                        {
                            LB_TID.Text = Request.QueryString["TID"].ToString();

                            LB_HLink.Text = "https://" + Request.Url.Authority + "/HSCTopicDetail.aspx?TID=" + LB_TID.Text + "&F=0";

                      
                            SqlDataReader QueryHSCTopic = SQLdatabase.ExecuteReader("SELECT a.HID, a.HSCForumClassID,  c.HSCFCName AS HSCSubName,  b.HSCFCName AS HSCForumName,  HTopicName, HPinTop,HSCClassID, HSCRecordTypeID, HSCJiugonggeTypeID, HGProgress, HOGProgress, HContent, HFile1, HFile2, HFile3, HVideoLink, HHashTag , (ISNULL(e.HSystemName,'')+' '+ ISNULL(e.HArea,'')+'/'+ISNULL(e.HPeriod,'') +' '+ e.HUserName) AS UserName, e.HImg, a.HCreate, a.HCreateDT, a.HModifyDT FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID=b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster=c.HID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus=1 AND a.HID='" + LB_TID.Text + "'");

                            if (QueryHSCTopic.Read())
                            {
                                LB_HSCForumClassID.Text = QueryHSCTopic["HSCForumClassID"].ToString();
                                LB_HSCSubClass.Text = QueryHSCTopic["HSCSubName"].ToString();
                                LB_HSCForumName.Text = QueryHSCTopic["HSCForumName"].ToString();
                                LTR_HTopicName.Text = QueryHSCTopic["HTopicName"].ToString();
                                LB_HDate.Text = QueryHSCTopic["HCreateDT"].ToString();
                                LB_HHashTag.Text = QueryHSCTopic["HHashTag"].ToString();

                                if (string.IsNullOrEmpty(QueryHSCTopic["HHashTag"].ToString()))
                                {
                                    Div_HashTag.Style.Add("display", "none !important");
                                }
                                else
                                {
                                    string[] gHashTag = LB_HHashTag.Text.Split(',');
                                    Rpt_HHashTag.DataSource = gHashTag;
                                    Rpt_HHashTag.DataBind();
                                }

                                LB_HContent.Text = QueryHSCTopic["HContent"].ToString();

                                if (!string.IsNullOrEmpty(QueryHSCTopic["HFile1"].ToString()))
                                {
                                    HF_File1.Value = QueryHSCTopic["HFile1"].ToString();
                                    string[] gExtension = HF_File1.Value.Split('.');
                                    if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "heic" || gExtension[1] == "heif" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                                    {
                                        IMG_File1.Visible = true;
                                        IMG_File1.ImageUrl = SCFile + HF_File1.Value;
                                    }
                                    else if (gExtension[1] == "mp3")
                                    {
                                        Audio1.Visible = true;
                                        Source1.Src = SCFile + HF_File1.Value;
                                    }
                                    else if (gExtension[1] == "pdf")
                                    {
                                        hfFile1.Value = "uploads/SpecialColumn/" + HF_File1.Value;
                                        Pdf_Content1.Text = PDFLayout("1");
                                        Pdf_Content1.Visible = true;
                                    }
                                }

                                if (!string.IsNullOrEmpty(QueryHSCTopic["HFile2"].ToString()))
                                {
                                    HF_File2.Value = QueryHSCTopic["HFile2"].ToString();
                                    string[] gExtension = HF_File2.Value.Split('.');
                                    if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "heic" || gExtension[1] == "heif" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                                    {
                                        IMG_File2.Visible = true;
                                        IMG_File2.ImageUrl = SCFile + HF_File2.Value;
                                    }
                                    else if (gExtension[1] == "mp3")
                                    {
                                        Audio2.Visible = true;
                                        Source2.Src = SCFile + HF_File2.Value;
                                    }
                                    else if (gExtension[1] == "pdf")
                                    {
                                        hfFile2.Value = "uploads/SpecialColumn/" + HF_File2.Value;
                                        Pdf_Content2.Text = PDFLayout("2");
                                        Pdf_Content2.Visible = true;
                                    }
                                }

                                if (!string.IsNullOrEmpty(QueryHSCTopic["HFile3"].ToString()))
                                {
                                    HF_File3.Value = QueryHSCTopic["HFile3"].ToString();
                                    string[] gExtension = HF_File3.Value.Split('.');
                                    if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "heic" || gExtension[1] == "heif" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                                    {
                                        IMG_File3.Visible = true;
                                        IMG_File3.ImageUrl = SCFile + HF_File3.Value;
                                    }
                                    else if (gExtension[1] == "mp3")
                                    {
                                        Audio3.Visible = true;
                                        Source3.Src = SCFile + HF_File3.Value;
                                    }
                                    else if (gExtension[1] == "pdf")
                                    {
                                        hfFile3.Value = "uploads/SpecialColumn/" + HF_File3.Value;
                                        Pdf_Content3.Text = PDFLayout("3");
                                        Pdf_Content3.Visible = true;
                                    }
                                }

                                LB_HImg.Text = QueryHSCTopic["HImg"].ToString();

                                if (string.IsNullOrEmpty(QueryHSCTopic["UserName"].ToString()))
                                {
                                    LB_HMemberID.Text = "0";
                                    LB_HMember.Text = "系統自動產生";
                                }
                                else
                                {
                                    LB_HMemberID.Text = QueryHSCTopic["HCreate"].ToString();
                                    LB_HMember.Text = QueryHSCTopic["UserName"].ToString();
                                }

                                if (LB_HMember.Text == "系統自動產生")
                                {
                                    IMG_Creator.ImageUrl = "~/images/icon.png";
                                }
                                else
                                {
                                    UserImgShow(LB_HImg.Text, IMG_Creator);
                                }


                                //嵌入式影片
                                if (!string.IsNullOrEmpty(QueryHSCTopic["HVideoLink"].ToString()))
                                {
                                    this.iframe_video.Attributes.Add("src", QueryHSCTopic["HVideoLink"].ToString());
                                    Div_Video.Visible = true;
                                }
                                else
                                {
                                    this.iframe_video.Attributes.Add("style", "display:none");
                                    Div_Video.Visible = false;
                                }


                            }
                            QueryHSCTopic.Close();

                            ////GE20250211_新增課後教材的教材資訊
                            ////課後教材格式只有pdf、mp3
                            //SqlDataReader QueryHACMaterial = SQLdatabase.ExecuteReader("SELECT HUP1, HUP2, HUP3, HUP4, HUP5, HCMLink FROM HACMaterial WHERE SUBSTRING(HEIPLink, CHARINDEX('=', HEIPLink) + 1, LEN(HEIPLink)) = '" + LB_TID.Text + "'");

                            //if (QueryHACMaterial.Read())
                            //{
                            //    if (!string.IsNullOrEmpty(QueryHACMaterial["HUP1"].ToString()))
                            //    {
                            //        if (QueryHACMaterial["HUP1"].ToString().Substring(QueryHACMaterial["HUP1"].ToString().Length - 4, 4) == ".pdf")
                            //        {
                            //            HF_HUP1.Value = ACMaterialRoot + QueryHACMaterial["HUP1"].ToString();
                            //            Pdf_ACContent1.Text = ACPDFLayout("1");
                            //            Pdf_ACContent1.Visible = true;
                            //        }
                            //        else if (QueryHACMaterial["HUP1"].ToString().Substring(QueryHACMaterial["HUP1"].ToString().Length - 4, 4) == ".mp3")
                            //        {
                            //            ACMaterial_audio1.Visible = true;
                            //            ACMaterial_audio1.Src = ACMaterialRoot + QueryHACMaterial["HUP1"].ToString();
                            //        }
                            //        else if (QueryHACMaterial["HUP1"].ToString().Substring(QueryHACMaterial["HUP1"].ToString().Length - 4, 4) == ".m4a")
                            //        {
                            //            ACMaterial_audio1.Visible = true;
                            //            ACMaterial_audio1.Src = ACMaterialRoot + QueryHACMaterial["HUP1"].ToString();
                            //            ACMaterial_audio1.Attributes.Add("type", "audio/x-m4a");
                            //        }

                            //    }

                            //    if (!string.IsNullOrEmpty(QueryHACMaterial["HUP2"].ToString()))
                            //    {

                            //        if (QueryHACMaterial["HUP2"].ToString().Substring(QueryHACMaterial["HUP2"].ToString().Length - 4, 4) == ".pdf")
                            //        {
                            //            HF_HUP2.Value = ACMaterialRoot + QueryHACMaterial["HUP2"].ToString();
                            //            Pdf_ACContent2.Text = ACPDFLayout("2");
                            //            Pdf_ACContent2.Visible = true;
                            //        }
                            //        else if (QueryHACMaterial["HUP2"].ToString().Substring(QueryHACMaterial["HUP2"].ToString().Length - 4, 4) == ".mp3")
                            //        {
                            //            ACMaterial_audio2.Visible = true;
                            //            ACMaterial_audio2.Src = ACMaterialRoot + QueryHACMaterial["HUP2"].ToString();
                            //        }
                            //        else if (QueryHACMaterial["HUP2"].ToString().Substring(QueryHACMaterial["HUP2"].ToString().Length - 4, 4) == ".m4a")
                            //        {
                            //            ACMaterial_audio2.Visible = true;
                            //            ACMaterial_audio2.Src = ACMaterialRoot + QueryHACMaterial["HUP2"].ToString();
                            //            ACMaterial_audio2.Attributes.Add("type", "audio/x-m4a");
                            //        }


                            //    }

                            //    if (!string.IsNullOrEmpty(QueryHACMaterial["HUP3"].ToString()))
                            //    {
                            //        if (QueryHACMaterial["HUP3"].ToString().Substring(QueryHACMaterial["HUP3"].ToString().Length - 4, 4) == ".pdf")
                            //        {
                            //            HF_HUP3.Value = ACMaterialRoot + QueryHACMaterial["HUP3"].ToString();
                            //            Pdf_ACContent3.Text = ACPDFLayout("3");
                            //            Pdf_ACContent3.Visible = true;
                            //        }
                            //        else if (QueryHACMaterial["HUP3"].ToString().Substring(QueryHACMaterial["HUP3"].ToString().Length - 4, 4) == ".mp3")
                            //        {
                            //            ACMaterial_audio3.Visible = true;
                            //            ACMaterial_audio3.Src = ACMaterialRoot + QueryHACMaterial["HUP3"].ToString();
                            //        }
                            //        else if (QueryHACMaterial["HUP3"].ToString().Substring(QueryHACMaterial["HUP3"].ToString().Length - 4, 4) == ".m4a")
                            //        {
                            //            ACMaterial_audio3.Visible = true;
                            //            ACMaterial_audio3.Src = ACMaterialRoot + QueryHACMaterial["HUP3"].ToString();
                            //            ACMaterial_audio3.Attributes.Add("type", "audio/x-m4a");
                            //        }

                            //    }

                            //    if (!string.IsNullOrEmpty(QueryHACMaterial["HUP4"].ToString()))
                            //    {
                            //        if (QueryHACMaterial["HUP4"].ToString().Substring(QueryHACMaterial["HUP4"].ToString().Length - 4, 4) == ".pdf")
                            //        {
                            //            HF_HUP4.Value = ACMaterialRoot + QueryHACMaterial["HUP4"].ToString();
                            //            Pdf_ACContent4.Text = ACPDFLayout("4");
                            //            Pdf_ACContent4.Visible = true;
                            //        }
                            //        else if (QueryHACMaterial["HUP4"].ToString().Substring(QueryHACMaterial["HUP4"].ToString().Length - 4, 4) == ".mp3")
                            //        {
                            //            ACMaterial_audio4.Visible = true;
                            //            ACMaterial_audio4.Src = ACMaterialRoot + QueryHACMaterial["HUP4"].ToString();
                            //        }
                            //        else if (QueryHACMaterial["HUP4"].ToString().Substring(QueryHACMaterial["HUP4"].ToString().Length - 4, 4) == ".m4a")
                            //        {
                            //            ACMaterial_audio4.Visible = true;
                            //            ACMaterial_audio4.Src = ACMaterialRoot + QueryHACMaterial["HUP4"].ToString();
                            //            ACMaterial_audio4.Attributes.Add("type", "audio/x-m4a");
                            //        }

                            //    }

                            //    if (!string.IsNullOrEmpty(QueryHACMaterial["HUP5"].ToString()))
                            //    {
                            //        if (QueryHACMaterial["HUP5"].ToString().Substring(QueryHACMaterial["HUP5"].ToString().Length - 4, 4) == ".pdf")
                            //        {
                            //            HF_HUP5.Value = ACMaterialRoot + QueryHACMaterial["HUP5"].ToString();
                            //            Pdf_ACContent5.Text = ACPDFLayout("5");
                            //            Pdf_ACContent5.Visible = true;
                            //        }
                            //        else if (QueryHACMaterial["HUP5"].ToString().Substring(QueryHACMaterial["HUP5"].ToString().Length - 4, 4) == ".mp3")
                            //        {
                            //            ACMaterial_audio5.Visible = true;
                            //            ACMaterial_audio5.Src = ACMaterialRoot + QueryHACMaterial["HUP5"].ToString();
                            //        }
                            //        else if (QueryHACMaterial["HUP5"].ToString().Substring(QueryHACMaterial["HUP5"].ToString().Length - 4, 4) == ".m4a")
                            //        {
                            //            ACMaterial_audio5.Visible = true;
                            //            ACMaterial_audio5.Src = ACMaterialRoot + QueryHACMaterial["HUP5"].ToString();
                            //            ACMaterial_audio5.Attributes.Add("type", "audio/x-m4a");
                            //        }

                            //    }

                            //}
                            //QueryHACMaterial.Close();



                            ////GE20241225_加入顯示隱藏的留言
                            ////SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "' AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
                            ////SDS_HSCTMsg.DataBind();
                            ////Rpt_HSCTMsg.DataBind();

                            //DataTable mysqlTable = null;
                            //DataTable mssqlTable = null;

                            //#region 留言資料合併EIP與EDU專欄
                            ////GA20250731_新增合併內容
                            //try
                            //{

                            //    mysqlTable = new DataTable();
                            //     mssqlTable = new DataTable();

                            //    //EIP資料
                            //    using (var conn = new MySqlConnection(EIPconnStr))
                            //    {
                            //        conn.Open();
                            //        string mysqlQuery = "select laoshireply.lr_id AS HID, laoshireply.lr_cdate AS HCreateDT ,laoshireply.lr_file1 AS HImg,laoshireply.lr_file1 AS HFile1,'' AS HVideoLink, u_info AS HUserName, laoshireply.lr_uid AS HMemberID, laoshireply.lr_content AS HContent,  lr_status AS HStatus, lr_q AS HQuestionYN  from laoshireply,sysuser WHERE laoshireply.lr_status = 1 AND laoshireply.lr_lid = '" + Request.QueryString["TID"].ToString().PadLeft(10, '0') + "' ORDER BY laoshireply.lr_cdate DESC";
                            //        using (var cmd = new MySqlCommand(mysqlQuery, conn))
                            //        {
                            //            using (var adapter = new MySqlDataAdapter(cmd))
                            //            {
                            //                adapter.Fill(mysqlTable);
                            //            }
                            //        }

                            //    }

                            //    //EDU資料
                            //    string mssqlQuery = "SELECT  a.HID, a.HCreateDT, c.HIMG ,  a.HFile1, a.HVideoLink,  (ISNULL(b.HSystemName, '') + ' ' +ISNULL(b.HArea, '') + '<br/>' +ISNULL(b.HPeriod, '') + ' ' +ISNULL(b.HUserName, '')  ) AS HUserName, a.HMemberID, a.HContent, a.HStatus, a.HQuestionYN FROM HSCTMsg AS a LEFT JOIN MemberList AS b ON a.HMemberID=b.HID LEFT JOIN HMember AS c ON a.HMemberID=c.HID WHERE (a.HStatus=1 OR a.HStatus=2) AND a.HSCTopicID = '" + LB_TID.Text + "' ORDER BY a.HCreateDT DESC";
                            //    mssqlTable = SQLdatabase.ExecuteDataTable(mssqlQuery);

                            //    //合併兩張表
                            //    //1.確保兩張表的欄位相同
                            //    foreach (DataColumn col in mssqlTable.Columns)
                            //    {
                            //        if (!mysqlTable.Columns.Contains(col.ColumnName))
                            //            mysqlTable.Columns.Add(col.ColumnName, col.DataType);
                            //    }

                            //    //2.合併 MSSQL 資料進 MySQL 資料表
                            //    foreach (DataRow row in mssqlTable.Rows)
                            //    {
                            //        mysqlTable.ImportRow(row);
                            //    }

                            //    //排序並綁定
                            //    DataView dv = mysqlTable.DefaultView;
                            //    dv.Sort = "HCreateDT DESC"; // 或 DESC，如果你希望新版資料在前面
                            //    Rpt_HSCTMsg.DataSource = dv;
                            //    Rpt_HSCTMsg.DataBind();

                            //}
                            //finally
                            //{
                            //    if (mysqlTable != null) mysqlTable.Dispose();
                            //    if (mssqlTable != null) mssqlTable.Dispose();
                            //}



                            ////SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "'  AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
                            //////SDS_HSCTMsg.DataBind();
                            ////Rpt_HSCTMsg.DataBind();
                            //#endregion


                            //MA20240718_學員登入才可建立留言
                            if (string.IsNullOrEmpty(((Label)Master.FindControl("LB_HUserHID")).Text))
                            {
                                Div_HSCTMsg.Visible = false;
                            }
                            else
                            {
                                Div_HSCTMsg.Visible = true;
                            }
                        }
                        else
                        {
                            Response.Redirect("HSCIndex.aspx");
                        }
                    }
                    else if (Request.QueryString["F"].ToString() == "1")  //資料來自EIP
                    {


                        LB_TID.Text = Request.QueryString["TID"].ToString();

                        LB_HLink.Text = "https://" + Request.Url.Authority + "/HSCTopicDetail.aspx?TID=" + LB_TID.Text + "&F=1";


                       

                        #region EIP的資料顯示
                        MySqlDataReader dr = MySQLdatabase.ExecuteReader("SELECT l_id, l_title, l_Content, l_recorddate, l_file1, l_file2, l_file3 FROM laoshi WHERE l_id=" + Request.QueryString["TID"].ToString().PadLeft(10, '0'));

                        if (dr.Read())
                        {
                            LB_HSCSubClass.Text = "大愛光老師專欄";
                            LB_HSCForumName.Text = "EIP";
                            //Div_ForumName.Visible = false;
                            Div_HashTag.Visible = false;

                            LB_TID.Text = dr["l_id"].ToString();
                            //LTR_HTopicName.Text = dr["l_title"].ToString();

                            int brIndex = dr["l_title"].ToString().IndexOf("br", StringComparison.OrdinalIgnoreCase);
                            string title = dr["l_title"].ToString();
                            string extra = "";

                            if (brIndex >= 0)
                            {
                                title = dr["l_title"].ToString().Substring(0, brIndex); // 取 br 前作為新標題
                                extra = dr["l_title"].ToString().Substring(brIndex + 2); // 剩下的移到內容（跳過 br 共4字元）
                            }
                            LTR_HTopicName.Text = title;
                            LB_HContent.Text = extra.Replace("br", "<br/>") + dr["l_Content"].ToString();

                            LB_HMember.Text = "EIP大愛光老師專欄";
                            LB_HDate.Text = dr["l_recorddate"].ToString();

                            DateTime MDate = DateTime.ParseExact(dr["l_recorddate"].ToString(), "yyyyMMddHHmmss", null);
                            DateTime CDate = DateTime.ParseExact(dr["l_recorddate"].ToString(), "yyyyMMddHHmmss", null);

                            if (!string.IsNullOrEmpty(dr["l_recorddate"].ToString()))
                            {
                                LB_HDate.Text = MDate.ToString("yyyy/MM/dd");
                            }
                            else
                            {
                                LB_HDate.Text = CDate.ToString("yyyy/MM/dd");
                            }



                            if (!string.IsNullOrEmpty(dr["l_file1"].ToString()))
                            {
                                HF_File1.Value = dr["l_file1"].ToString();
                                string[] gExtension = HF_File1.Value.Split('.');
                                if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "heic" || gExtension[1] == "heif" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                                {
                                    IMG_File1.Visible = true;
                                    IMG_File1.ImageUrl = EIPFile + HF_File1.Value;
                                }
                                else if (gExtension[1] == "mp3")
                                {
                                    Audio1.Visible = true;
                                    Source1.Src = EIPFile + HF_File1.Value;
                                }
                                else if (gExtension[1] == "pdf")
                                {

                                    //hfFile1.Value = EIPFile + HF_File1.Value;
                                    // 改成用後端丟 URL避免跨域無法預覽
                                    string proxyUrl = "PDFProxy.ashx?url=" + HttpUtility.UrlEncode(EIPFile + HF_File1.Value);
                                    hfFile1.Value = proxyUrl;
                                    Pdf_Content1.Text = PDFLayout("6");
                                    Pdf_Content1.Visible = true;
                                }
                            }

                            if (!string.IsNullOrEmpty(dr["l_file2"].ToString()))
                            {
                                HF_File2.Value = dr["l_file2"].ToString();
                                string[] gExtension = HF_File2.Value.Split('.');
                                if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "heic" || gExtension[1] == "heif" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                                {
                                    IMG_File2.Visible = true;
                                    IMG_File2.ImageUrl = EIPFile + HF_File2.Value;
                                }
                                else if (gExtension[1] == "mp3")
                                {
                                    Audio2.Visible = true;
                                    Source2.Src = EIPFile + HF_File2.Value;
                                }
                                else if (gExtension[1] == "pdf")
                                {
                                    //hfFile2.Value = EIPFile + HF_File2.Value;
                                    // 改成用後端丟 URL避免跨域無法預覽
                                    string proxyUrl = "PDFProxy.ashx?url=" + HttpUtility.UrlEncode(EIPFile + HF_File2.Value);
                                    hfFile2.Value = proxyUrl;
                                    Pdf_Content2.Text = PDFLayout("7");
                                    Pdf_Content2.Visible = true;
                                }
                            }

                            if (!string.IsNullOrEmpty(dr["l_file3"].ToString()))
                            {
                                HF_File3.Value = dr["l_file3"].ToString();
                                string[] gExtension = HF_File3.Value.Split('.');
                                if (gExtension[1] == "png" || gExtension[1] == "jpg" || gExtension[1] == "heic" || gExtension[1] == "heif" || gExtension[1] == "jpeg" || gExtension[1] == "gif")
                                {
                                    IMG_File3.Visible = true;
                                    IMG_File3.ImageUrl = EIPFile + HF_File3.Value;
                                }
                                else if (gExtension[1] == "mp3")
                                {
                                    Audio3.Visible = true;
                                    Source3.Src = EIPFile + HF_File3.Value;
                                }
                                else if (gExtension[1] == "pdf")
                                {
                                    //hfFile3.Value = EIPFile + HF_File3.Value;
                                    // 改成用後端丟 URL避免跨域無法預覽
                                    string proxyUrl = "PDFProxy.ashx?url=" + HttpUtility.UrlEncode(EIPFile + HF_File3.Value);
                                    hfFile3.Value = proxyUrl;
                                    Pdf_Content3.Text = PDFLayout("8");
                                    Pdf_Content3.Visible = true;
                                }
                            }

                            Div_Video.Visible = false;

                        }
                        dr.Close();
                        #endregion


                       

                        #region EIP留言內容
                        SDS_HSCTMsgMySQL.SelectCommand = "select laoshireply.lr_id AS HID, laoshireply.lr_cdate AS HCreateDT ,laoshireply.lr_file1 AS HImg,laoshireply.lr_file1 AS HFile1, '' AS HVideoLink, u_info AS HUserName, laoshireply.lr_uid AS HMemberID, laoshireply.lr_content AS HContent,  lr_status AS HStatus, lr_q AS HQuestionYN  from laoshireply,sysuser where lr_uid=u_id and lr_status=1 and lr_parent='' and lr_lid='" + Request.QueryString["TID"].ToString().PadLeft(10, '0') + "' order by lr_cdate DESC";

                        Rpt_HSCTMsg.DataSource = SDS_HSCTMsgMySQL;
                        Rpt_HSCTMsg.DataBind();

                        #endregion

                    }
                }
                else
                {
                    Response.Redirect("HSCIndex.aspx");
                }
            }
            else
            {
                Response.Redirect("HSCIndex.aspx");
            }


        }
        #endregion

    }

    #region 主題-心情功能
    //HType：1=讚 /  2=愛心 / 3=微笑
    protected void LBtn_HTopicType_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;

        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HSCTopicID, HMemberID, HType FROM HSCTopic_Mood WHERE HSCTopicID='" + LB_TID.Text + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        if (dr.Read())
        {
            if (btn.TabIndex.ToString() != dr["HType"].ToString())
            {
                SqlCommand cmd = new SqlCommand("UPDATE HSCTopic_Mood SET HType=@HType, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCTopicID=@HSCTopicID AND HMemberID=@HMemberID", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HSCTopicID", LB_TID.Text);
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

            cmd.Parameters.AddWithValue("@HSCTopicID", LB_TID.Text);
            cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HType", btn.TabIndex.ToString());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            ConStr.Close();
            cmd.Cancel();
        }
        dr.Close();



        //Div_TFeelingArea.Visible = false;


    }

    #endregion

    #region 回應主題
    //MA20240718_新增回應主題-送出回應、資料繫結、上傳檔案功能
    //MA20240719_新增回應主題-心情功能
    //MA20240720_新增刪除已上傳檔案功能、編輯留言功能、刪除留言功能
    //MA20240725_加入檔案顯示判斷(圖片、其他文件檔)
    //MA20240826_加入回應主題編輯/刪除/隱藏顯示權限判斷
    #region 回應主題-送出回應
    protected void LBtn_HSubmitMsg_Click(object sender, EventArgs e)
    {
        Panel_Comment.Visible = false;

        if (string.IsNullOrEmpty(CKE_HContent.Text) && string.IsNullOrEmpty(TB_HVideoLink.Text) && string.IsNullOrEmpty(LB_HFile1.Text))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入想要回應的內容');", true);
            return;
        }

        string HID = null; //流水號

        #region  判斷來源是否為EIP
        if (Request.QueryString["F"].ToString() == "1")
        {

            using (var conn = new MySqlConnection(EIPconnStr))
            {
                string uid = ((TextBox)Master.FindControl("TB_EIPUid")).Text;
                string nowYear = DateTime.Now.Year.ToString();
                string nowTime = DateTime.Now.AddHours(15).ToString("yyyyMMddHHmmss");
                string cname = HttpUtility.UrlDecode(((Label)Master.FindControl("LB_HUserName")).Text);
                string file1 = LB_HFile1.Text.Trim();

                conn.Open();

                // 一次查出 max_id 與 year_count
                string getInfoSql =
     "SELECT " +
     "IFNULL(MAX(lr_id), 0) + 1 AS next_id, " +
     "(SELECT COUNT(*) + 1 FROM laoshireply " +
     " WHERE lr_uid = @uid AND lr_status < 9 AND lr_cdate LIKE @yearPrefix) AS year_no " +
     "FROM laoshireply;";

                string nextId = null;
                int yearNo = 0;

                using (var infoCmd = new MySqlCommand(getInfoSql, conn))
                {
                    infoCmd.Parameters.AddWithValue("@uid", uid);
                    infoCmd.Parameters.AddWithValue("@yearPrefix", nowYear + "%");

                    using (var reader = infoCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            nextId = reader["next_id"].ToString().PadLeft(10, '0');
                            yearNo = Convert.ToInt32(reader["year_no"]);
                        }
                        reader.Close();
                    }
                }

                // 接著插入資料
                string insertSql = @"
        INSERT INTO laoshireply 
        (lr_id, lr_lid, lr_uid, lr_content, lr_status, lr_file1, lr_cdate, lr_cname, lr_parent, lr_q, lr_isshowdaily, lr_no, lr_qno, lr_yqno) 
        VALUES 
        (@lr_id, @lr_lid, @lr_uid, @lr_content, @lr_status, @lr_file1, @lr_cdate, @lr_cname, @lr_parent, @lr_q, @lr_isshowdaily, @lr_no, @lr_qno, @lr_yqno);";

                using (var cmd = new MySqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@lr_id", nextId);
                    cmd.Parameters.AddWithValue("@lr_lid", LB_TID.Text);
                    cmd.Parameters.AddWithValue("@lr_uid", uid.PadLeft(10, '0'));
                    cmd.Parameters.AddWithValue("@lr_content", CKE_HContent.Text.Trim());
                    cmd.Parameters.AddWithValue("@lr_status", 1);
                    cmd.Parameters.AddWithValue("@lr_file1", "<a href='https://edu.hochi.org.tw/HSCTopicDetail.aspx?TID="+ uid + "&F=1' target='_blank'>請回EDU查看上傳的檔案</a>");
                    cmd.Parameters.AddWithValue("@lr_cdate", nowTime);
                    cmd.Parameters.AddWithValue("@lr_cname", cname);
                    cmd.Parameters.AddWithValue("@lr_parent", "");
                    cmd.Parameters.AddWithValue("@lr_q", 0);
                    cmd.Parameters.AddWithValue("@lr_isshowdaily", 1);
                    cmd.Parameters.AddWithValue("@lr_no", nowYear + "-" + yearNo);
                    cmd.Parameters.AddWithValue("@lr_qno", "");
                    cmd.Parameters.AddWithValue("@lr_yqno", "");

                    cmd.ExecuteNonQuery();
                }
                conn.Close();
                HID = nextId.ToString();
            }




        }
        else
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO [HSCTMsg] (HSCTopicID, HMemberID, HContent, HFile1, HVideoLink,HQuestionYN, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTopicID, @HMemberID, @HContent, @HFile1, @HVideoLink, @HQuestionYN, @HStatus, @HCreate, @HCreateDT);SELECT SCOPE_IDENTITY() AS HID ", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCTopicID", LB_TID.Text.Trim());
            cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HContent", CKE_HContent.Text.Trim());
            cmd.Parameters.AddWithValue("@HFile1", LB_HFile1.Text.Trim());
            cmd.Parameters.AddWithValue("@HVideoLink", TB_HVideoLink.Text.Trim());
            cmd.Parameters.AddWithValue("@HQuestionYN", CB_HQuestionYN.Checked);
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            //cmd.ExecuteNonQuery();
            HID = cmd.ExecuteScalar().ToString();
            ConStr.Close();
            cmd.Cancel();
        }



        #endregion






        //回應主題通知
        /// <summary>
        /// 新增通知紀錄
        /// </summary>
        /// <param name="gHMemberID">接收通知者</param>
        /// <param name="gHActorMemberID">觸發通知者</param>
        /// <param name="gHNotifyType">通知類型(1:回應、2:心情(讚)、3:心情(愛心)、4:心情(微笑)、5:分享、6:提問、7:被刪除留言/回應、8:被隱藏留言/回應)</param>
        /// <param name="gHTargetID">對應資料表的流水號</param>
        /// <param name="gTableName">對應資料表名稱</param>
        string gNotifyType = CB_HQuestionYN.Checked ? "6" : "1";

        Notification.AddNotification(LB_HMemberID.Text, ((Label)Master.FindControl("LB_HUserHID")).Text, gNotifyType, HID, "HSCTMsg");


        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('新增成功!');", true);

        CB_HQuestionYN.Checked = false;
        CKE_HContent.Text = null;
        TB_HVideoLink.Text = null;
        LB_HFile1.Text = null;
        LB_HFileMsg1.Text = null;
        LBtn_HFile1_Del.Visible = false;



        #region 留言資料合併EIP與EDU專欄
        //GA20250731_新增合併內容
        DataTable mysqlTable = new DataTable();
        DataTable mssqlTable = new DataTable();



        


        //EIP資料
        using (var conn = new MySqlConnection(EIPconnStr))
        {
            conn.Open();
            string mysqlQuery = "select laoshireply.lr_id AS HID, laoshireply.lr_cdate AS HCreateDT ,laoshireply.lr_file1 AS HImg,laoshireply.lr_file1 AS HFile1,'' AS HVideoLink, u_info AS HUserName, laoshireply.lr_uid AS HMemberID, laoshireply.lr_content AS HContent,  lr_status AS HStatus, lr_q AS HQuestionYN  from laoshireply,sysuser WHERE laoshireply.lr_status = 1 AND laoshireply.lr_lid = '" + Request.QueryString["TID"].ToString().PadLeft(10, '0') + "' AND laoshireply.lr_parent='' ORDER BY laoshireply.lr_cdate DESC";
            using (var cmd = new MySqlCommand(mysqlQuery, conn))
            {
                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(mysqlTable);
                }
            }

        }

        //EDU資料
        string mssqlQuery = "SELECT  a.HID, a.HCreateDT, c.HIMG ,  a.HFile1, a.HVideoLink,  (ISNULL(b.HSystemName, '') + ' ' +ISNULL(b.HArea, '') + '<br/>' +ISNULL(b.HPeriod, '') + ' ' +ISNULL(b.HUserName, '')  ) AS HUserName, a.HMemberID, a.HContent, a.HStatus, a.HQuestionYN FROM HSCTMsg AS a LEFT JOIN MemberList AS b ON a.HMemberID=b.HID LEFT JOIN HMember AS c ON a.HMemberID=c.HID WHERE (a.HStatus=1 OR a.HStatus=2)  AND a.HSCTopicID = '" + LB_TID.Text + "' ORDER BY a.HCreateDT DESC";
        mssqlTable = SQLdatabase.ExecuteDataTable(mssqlQuery);

        //合併兩張表
        //1.確保兩張表的欄位相同
        foreach (DataColumn col in mssqlTable.Columns)
        {
            if (!mysqlTable.Columns.Contains(col.ColumnName))
                mysqlTable.Columns.Add(col.ColumnName, col.DataType);
        }

        //2.合併 MSSQL 資料進 MySQL 資料表
        foreach (DataRow row in mssqlTable.Rows)
        {
            mysqlTable.ImportRow(row);
        }

        //排序並綁定
        DataView dv = mysqlTable.DefaultView;
        dv.Sort = "HCreateDT DESC"; // 或 DESC，如果你希望新版資料在前面
        Rpt_HSCTMsg.DataSource = dv;
        Rpt_HSCTMsg.DataBind();

        //SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "'  AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
        ////SDS_HSCTMsg.DataBind();
        //Rpt_HSCTMsg.DataBind();
        #endregion

    }
    #endregion

    #region 回應主題-資料繫結
    protected void Rpt_HSCTMsg_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //大頭照顯示
        UserImgShow(((Label)e.Item.FindControl("LB_HImg")).Text, ((Image)e.Item.FindControl("IMG_HImg")));

        #region 判斷是大愛光老師，才顯示語音回應按鈕
        if (((Label)Master.FindControl("LB_HUserHID")).Text == "9390")
        {
            ((LinkButton)e.Item.FindControl("LBtn_VoiceMsg")).Visible = true;
        }
        else
        {
            ((LinkButton)e.Item.FindControl("LBtn_VoiceMsg")).Visible = false;
        }
        #endregion


        #region 檔案顯示判斷

        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_File1")).Text))
        {
            string[] filename = ((Label)e.Item.FindControl("LB_File1")).Text.Split('_');
            string fileroot = "~/uploads/HSCTMsg/" + filename[0].Substring(0, 8) + "/";
            string gUPFileExtension = System.IO.Path.GetExtension(((Label)e.Item.FindControl("LB_File1")).Text).ToLower();

            String[] FileType = { ".jpg", ".png", ".gif", ".mp3", ".pdf", ".docx", ".doc", ".xlsx", ".xls" };
            if (gUPFileExtension == FileType[0] || gUPFileExtension == FileType[1] || gUPFileExtension == FileType[2])    //圖片顯示
            {
                ((HtmlControl)e.Item.FindControl("Div_Image")).Visible = true;
                ((Image)e.Item.FindControl("IMG_File1")).ImageUrl = fileroot + ((Label)e.Item.FindControl("LB_File1")).Text;
            }
            else if (gUPFileExtension == FileType[3]) //語音檔
            {
                ((HtmlControl)e.Item.FindControl("Audio_Reply")).Visible = true;
                ((HtmlAudio)e.Item.FindControl("Audio_Reply")).Src = fileroot + ((Label)e.Item.FindControl("LB_File1")).Text;
            }
            else if (gUPFileExtension == FileType[4] || gUPFileExtension == FileType[5] || gUPFileExtension == FileType[6] || gUPFileExtension == FileType[7] || gUPFileExtension == FileType[8])//其他文件檔
            {
                ((HtmlControl)e.Item.FindControl("Div_Document")).Visible = true;

            }
            else
            {
                ((HtmlControl)e.Item.FindControl("Div_Image")).Visible = false;
                ((HtmlControl)e.Item.FindControl("Audio_Reply")).Visible = false;
                ((HtmlControl)e.Item.FindControl("Div_Document")).Visible = false;
            }


        }

        //GA20240818_加入影片嵌入
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HVideoLink")).Text))
        {
            ((HtmlIframe)e.Item.FindControl("iframe_video")).Attributes.Add("src", ((Label)e.Item.FindControl("LB_HVideoLink")).Text);
            ((HtmlGenericControl)e.Item.FindControl("Div_Video")).Visible = true;
        }
        else
        {
            ((HtmlGenericControl)e.Item.FindControl("Div_Video")).Visible = false;
        }



        #endregion

        #region 編輯留言顯示判斷
        int auth = 0;

        //最高權限管理者(討論區管理員)
        SqlDataReader strSQLdr = SQLdatabase.ExecuteReader("SELECT HID, HSCForumClassID, HMemberID FROM HSCModeratorSetting WHERE HSCForumClassID='" + LB_HSCForumClassID.Text + "' AND (HMemberID LIKE '%" + "," + ((Label)Master.FindControl("LB_HUserHID")).Text + "," + "%')");

        if (strSQLdr.Read())
        {
            auth = auth + 1;
        }
        else
        {
            //登入者是否等於留言者
            SqlDataReader userdr = SQLdatabase.ExecuteReader("SELECT HID, HMemberID FROM HSCTMsg WHERE HID = '" + ((Label)e.Item.FindControl("LB_HSCTMsgID")).Text + "' ");
            if (userdr.Read())
            {
                if (((Label)Master.FindControl("LB_HUserHID")).Text == userdr["HMemberID"].ToString())
                {
                    auth = auth + 1;
                }
                else
                {
                    auth = 0;
                }
            }
            userdr.Close();
        }

        if (auth > 0)
        {
            //MA20240828_有權限則開啟點點點的按鈕
            ((LinkButton)e.Item.FindControl("LBtn_More")).Visible = true;
        }
        else
        {
            //GA20240828_沒有權限則隱藏點點點的按鈕
            ((LinkButton)e.Item.FindControl("LBtn_More")).Visible = false;
        }

        #endregion

        #region 回應留言
        Repeater Rpt_HSCTMsgResponse = e.Item.FindControl("Rpt_HSCTMsgResponse") as Repeater;  //找到Repeater物件
        SqlDataSource SDS_HSCTMsgResponse = e.Item.FindControl("SDS_HSCTMsgResponse") as SqlDataSource;  //找到SqlDataSource物件        

        #region 新版專欄
        ////GE20241225_加入顯示隱藏的留言
        //SDS_HSCTMsgResponse.SelectCommand = HSCTMsgResponseCom + " WHERE a.HSCTMsgID='" + ((Label)e.Item.FindControl("LB_HSCTMsgID")).Text + "' AND (a.HStatus=1 OR a.HStatus=2)";

        //SDS_HSCTMsgResponse.DataBind();
        //Rpt_HSCTMsgResponse.DataBind();
        #endregion

        #region 回應留言資料合併EIP與EDU專欄
        ////GA20250731_新增合併內容
        //DataTable mysqlTable = new DataTable();
        //DataTable mssqlTable = new DataTable();

        ////EIP資料
        //using (var conn = new MySqlConnection(EIPconnStr))
        //{
        //    conn.Open();
        //    string mysqlQuery = "select laoshireply.lr_id AS HID, laoshireply.lr_lid AS HSCTMsgID, laoshireply.lr_cdate AS HCreateDT ,laoshireply.lr_file1 AS HImg,laoshireply.lr_file1 AS HFile1,'' AS HVideoLink, u_info AS HUserName, laoshireply.lr_uid AS HMemberID, laoshireply.lr_content AS HMsgResponse,'' AS HVoice,   lr_status AS HStatus, lr_q AS HQuestionYN  from laoshireply,sysuser WHERE laoshireply.lr_status <9 AND laoshireply.lr_lid = '" + Request.QueryString["TID"].ToString().PadLeft(10, '0') + "' AND laoshireply.lr_parent='"+ ((Label)e.Item.FindControl("LB_HSCTMsgID")).Text + "' ORDER BY laoshireply.lr_cdate DESC";
        //    using (var cmd = new MySqlCommand(mysqlQuery, conn))
        //    {
        //        using (var adapter = new MySqlDataAdapter(cmd))
        //        {
        //            adapter.Fill(mysqlTable);
        //        }
        //    }

        //}

        ////EDU資料
        //string mssqlQuery = HSCTMsgResponseCom + " WHERE a.HSCTMsgID='" + ((Label)e.Item.FindControl("LB_HSCTMsgID")).Text + "' AND (a.HStatus=1 OR a.HStatus=2)";
        //mssqlTable = SQLdatabase.ExecuteDataTable(mssqlQuery);

        ////合併兩張表
        ////1.確保兩張表的欄位相同
        //foreach (DataColumn col in mssqlTable.Columns)
        //{
        //    if (!mysqlTable.Columns.Contains(col.ColumnName))
        //        mysqlTable.Columns.Add(col.ColumnName, col.DataType);
        //}

        ////2.合併 MSSQL 資料進 MySQL 資料表
        //foreach (DataRow row in mssqlTable.Rows)
        //{
        //    mysqlTable.ImportRow(row);
        //}

        ////排序並綁定
        //DataView dv = mysqlTable.DefaultView;
        //dv.Sort = "HCreateDT DESC"; // 或 DESC，如果你希望新版資料在前面
        //Rpt_HSCTMsgResponse.DataSource = dv;
        //Rpt_HSCTMsgResponse.DataBind();

        ////SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "'  AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
        //////SDS_HSCTMsg.DataBind();
        ////Rpt_HSCTMsg.DataBind();
        #endregion



        #endregion


        //GA20241225_判斷留言是否被隱藏
        if (((Label)e.Item.FindControl("LB_HStatus")).Text == "2" && (((Label)e.Item.FindControl("LB_HMemberID")).Text == ((Label)Master.FindControl("LB_HUserHID")).Text))
        {
            ((HtmlGenericControl)e.Item.FindControl("Div_SCMsg")).Visible = true;
            ((HtmlGenericControl)e.Item.FindControl("Div_Status")).Visible = true;
            ((HtmlGenericControl)e.Item.FindControl("Li_MsgHide")).Visible = false;
            ((HtmlGenericControl)e.Item.FindControl("Div_Editmore")).Visible = true;
        }
        else if (((Label)e.Item.FindControl("LB_HStatus")).Text == "2" && (((Label)e.Item.FindControl("LB_HMemberID")).Text != ((Label)Master.FindControl("LB_HUserHID")).Text))
        {
            ((HtmlGenericControl)e.Item.FindControl("Div_SCMsg")).Visible = false;
            ((HtmlGenericControl)e.Item.FindControl("Div_Editmore")).Visible = false;
        }



        //GA20240829_加入點擊後會先跳提示訊息，避免誤點
        ((LinkButton)e.Item.FindControl("LBtn_MsgDel")).Attributes.Add("onclick", "return confirm('是否確認要刪除?')");
        ((LinkButton)e.Item.FindControl("LBtn_MsgHide")).Attributes.Add("onclick", "return confirm('是否確認要隱藏?')");


        if (((Label)e.Item.FindControl("LB_HQuestionYN")).Text == "1")
        {
            ((HtmlGenericControl)e.Item.FindControl("Div_Question")).Visible = true;
        }


        #region EIP留言者的資訊

        ((Label)e.Item.FindControl("LB_EIPinfo")).Text = ((Label)e.Item.FindControl("LB_EIPinfo")).Text.Replace("．", "<br>");
        ((Label)e.Item.FindControl("LB_HUserName")).Text = ((Label)e.Item.FindControl("LB_EIPinfo")).Text;

        //時間顯示的轉換
        if (!string.IsNullOrEmpty(((Label)e.Item.FindControl("LB_HCreateDT")).Text))
        {
            DateTime dt;
            if (DateTime.TryParseExact(((Label)e.Item.FindControl("LB_HCreateDT")).Text, "yyyyMMddHHmmss",
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None, out dt))
            {
                ((Label)e.Item.FindControl("LB_HCreateDT")).Text = dt.ToString("yyyy/MM/dd HH:mm");
            }
        }
        //((Label)e.Item.FindControl("LB_HCreateDT")).Text = Convert.ToDateTime(((Label)e.Item.FindControl("LB_HCreateDT")).Text).ToString("yyyy/MM/dd HH:mm:ss");
        #endregion

        //如果有換行符號顯示換行符號
        ((Label)e.Item.FindControl("LB_HContent")).Text = ((Label)e.Item.FindControl("LB_HContent")).Text.Replace(Environment.NewLine, "<br />");

    }
    #endregion

    #region 回應主題-上傳檔案功能
    protected void LBtn_HFile1_Click(object sender, EventArgs e)
    {

        //判斷檔案路徑是否存在，不存在則建立資料夾 
        if (!Directory.Exists(Server.MapPath("~/uploads/HSCTMsg/" + DateTime.Now.ToString("yyyyMMdd"))))
        {
            Directory.CreateDirectory(Server.MapPath("~/uploads/HSCTMsg/" + DateTime.Now.ToString("yyyyMMdd")));//不存在就建立目錄 
        }

        string gUPFile = "~/uploads/HSCTMsg/" + DateTime.Now.ToString("yyyyMMdd") + "/";
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
                    LB_HFile1.Text = DateTime.Now.ToString("yyyyMMddssff") + "_" + gUPFileMain + gUPFileExtension;
                    FU_HFile1.SaveAs(Server.MapPath(gUPFile) + LB_HFile1.Text);

                    LB_HFileMsg1.Text = "上傳成功";
                    //GE20240724_調整樣式
                    LB_HFileMsg1.Style.Add("color", "#2ebb4e");
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

        return;


    }
    #endregion

    #region 回應主題-刪除已上傳檔案
    protected void LBtn_HFile1_Del_Click(object sender, EventArgs e)
    {
        string sourceFile = File1Root + LB_HFile1.Text;

        if (File.Exists(sourceFile)) File.Delete(sourceFile);
        LB_HFile1.Text = null;
        LB_HFileMsg1.Text = null;
        LBtn_HFile1_Del.Visible = false;

    }
    #endregion

    #region 留言-心情功能
    //HType：1=讚 /  2=愛心 / 3=微笑
    protected void LBtn_HMsgType_Click(object sender, EventArgs e)
    {
        LinkButton btn = (LinkButton)sender;
        int index = Convert.ToInt32(btn.CommandArgument);

        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HID, HSCTMsgID, HMemberID, HType FROM HSCTMsg_Mood WHERE HSCTMsgID='" + ((Label)Rpt_HSCTMsg.Items[index].FindControl("LB_HSCTMsgID")).Text + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        if (dr.Read())
        {
            if (btn.TabIndex.ToString() != dr["HType"].ToString())
            {
                SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsg_Mood] SET HType=@HType, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCTMsgID=@HSCTMsgID AND HMemberID=@HMemberID", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HSCTMsgID", ((Label)Rpt_HSCTMsg.Items[index].FindControl("LB_HSCTMsgID")).Text);
                cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HType", btn.TabIndex.ToString());
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                cmd.ExecuteNonQuery();
                ConStr.Close();
                cmd.Cancel();

                string HID = dr["HID"].ToString();
                //心情通知
                /// <summary>
                /// 新增通知紀錄
                /// </summary>
                /// <param name="gHMemberID">接收通知者</param>
                /// <param name="gHActorMemberID">觸發通知者</param>
                /// <param name="gHNotifyType">通知類型(1:回應、2:心情(讚)、3:心情(愛心)、4:心情(微笑)、5:分享、6:提問)</param>
                /// <param name="gHTargetID">對應資料表的流水號</param>
                /// <param name="gTableName">對應資料表名稱</param>
                Notification.AddNotification(((Label)RI.FindControl("LB_HMemberID")).Text, ((Label)Master.FindControl("LB_HUserHID")).Text, (Convert.ToInt32(btn.TabIndex.ToString()) + 1).ToString(), HID, "HSCTMsg_Mood");
            }

        }
        else
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO [HSCTMsg_Mood] (HSCTMsgID, HMemberID, HType, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgID, @HMemberID, @HType, @HStatus, @HCreate, @HCreateDT);SELECT SCOPE_IDENTITY() AS HID", ConStr);
            ConStr.Open();

            cmd.Parameters.AddWithValue("@HSCTMsgID", ((Label)Rpt_HSCTMsg.Items[index].FindControl("LB_HSCTMsgID")).Text);
            cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HType", btn.TabIndex.ToString());
            cmd.Parameters.AddWithValue("@HStatus", 1);
            cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
            cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

            //cmd.ExecuteNonQuery();
            string HID = cmd.ExecuteScalar().ToString();
            ConStr.Close();
            cmd.Cancel();

            //心情通知
            /// <summary>
            /// 新增通知紀錄
            /// </summary>
            /// <param name="gHMemberID">接收通知者</param>
            /// <param name="gHActorMemberID">觸發通知者</param>
            /// <param name="gHNotifyType">通知類型(1:回應、2:心情(讚)、3:心情(愛心)、4:心情(微笑)、5:分享、6:提問)</param>
            /// <param name="gHTargetID">對應資料表的流水號</param>
            /// <param name="gTableName">對應資料表名稱</param>
            Notification.AddNotification(((Label)RI.FindControl("LB_HMemberID")).Text, ((Label)Master.FindControl("LB_HUserHID")).Text, (Convert.ToInt32(btn.TabIndex.ToString()) + 1).ToString(), HID, "HSCTMsg_Mood");

        }
        dr.Close();


        SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "'  AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
        Rpt_HSCTMsg.DataBind();

        //if (((HtmlGenericControl)Rpt_HSCTMsg.Items[index].FindControl("Div_FeelingArea")).Visible == true)
        //{
        //    ((HtmlGenericControl)Rpt_HSCTMsg.Items[index].FindControl("Div_FeelingArea")).Visible = false;
        //}

    }
    #endregion

    #region 回應主題的更多選項(編輯、刪除、隱藏)
    //GA20240724_新增回應主題的編輯、刪除、隱藏
    protected void LBtn_More_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_More = sender as LinkButton;
        string gMore_CA = LBtn_More.CommandArgument;
        int gMore_CN = Convert.ToInt32(LBtn_More.CommandName);

        //判斷若已經有點開某一個，則要隱藏
        for (int i = 0; i < Rpt_HSCTMsg.Items.Count; i++)
        {
            if (i != gMore_CN)
            {
                ((HtmlControl)Rpt_HSCTMsg.Items[i].FindControl("Div_Editarea")).Visible = false;
            }
        }

        if (((HtmlControl)Rpt_HSCTMsg.Items[gMore_CN].FindControl("Div_Editarea")).Visible == false)
        {
            ((HtmlControl)Rpt_HSCTMsg.Items[gMore_CN].FindControl("Div_Editarea")).Visible = true;
        }
        else
        {
            ((HtmlControl)Rpt_HSCTMsg.Items[gMore_CN].FindControl("Div_Editarea")).Visible = false;
        }

        //Div_TFeelingArea.Visible = false;

    }
    #endregion

    #region 回應留言的更多選項(編輯、刪除、隱藏)
    //GA20240724_新增回應留言的編輯、刪除、隱藏
    protected void LBtn_ReplyMore_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_ReplyMore = sender as LinkButton;
        string gMore_CA = LBtn_ReplyMore.CommandArgument;
        int gMore_CN = Convert.ToInt32(LBtn_ReplyMore.CommandName);

        //GA20240828_抓父層(上一層)的repeater寫法(用NamingContainer)
        var gChildRepeaterItem = (RepeaterItem)LBtn_ReplyMore.NamingContainer;
        var gChildRepeater = (Repeater)gChildRepeaterItem.NamingContainer;
        var gParentRepeaterItem = (RepeaterItem)gChildRepeater.NamingContainer;
        var gParentRepeaterItemIndex = gParentRepeaterItem.ItemIndex;

        Repeater Rpt_HSCTMsgResponse = ((Repeater)Rpt_HSCTMsg.Items[gParentRepeaterItemIndex].FindControl("Rpt_HSCTMsgResponse")) as Repeater;
        ((HtmlControl)Rpt_HSCTMsgResponse.Items[gMore_CN].FindControl("Div_ReplyArea")).Visible = false;

        //判斷外層repeater的
        for (int y = 0; y < Rpt_HSCTMsg.Items.Count; y++)
        {
            if (y != gParentRepeaterItemIndex)
            {
                Repeater OtherRepeater = ((Repeater)Rpt_HSCTMsg.Items[y].FindControl("Rpt_HSCTMsgResponse")) as Repeater;

                for (int i = 0; i < OtherRepeater.Items.Count; i++)
                {
                    ((HtmlControl)OtherRepeater.Items[i].FindControl("Div_ReplyArea")).Visible = false;
                }
            }
            else
            {
                for (int x = 0; x < Rpt_HSCTMsgResponse.Items.Count; x++)
                {
                    if (x != gMore_CN)
                    {
                        ((HtmlControl)Rpt_HSCTMsgResponse.Items[x].FindControl("Div_ReplyArea")).Visible = false;
                    }

                }
            }

        }

        //顯示資料
        if (((HtmlControl)Rpt_HSCTMsgResponse.Items[gMore_CN].FindControl("Div_ReplyArea")).Visible == false)
        {
            ((HtmlControl)Rpt_HSCTMsgResponse.Items[gMore_CN].FindControl("Div_ReplyArea")).Visible = true;
        }
        else
        {
            ((HtmlControl)Rpt_HSCTMsgResponse.Items[gMore_CN].FindControl("Div_ReplyArea")).Visible = false;
        }


    }
    #endregion

    #region 回應主題-編輯留言

    #region 編輯留言Modal
    protected void LBtn_MsgEdit_Click(object sender, EventArgs e)
    {
        LinkButton gButton1 = sender as LinkButton;
        string gButton1_CA = gButton1.CommandArgument;
        //GA20240724_新增取得itemindex
        int gEdit_CN = Convert.ToInt32(gButton1.CommandName);
        LB_HSCTMsgID.Text = gButton1_CA;

        //GA20240724_隱藏更多選項的區域
        ((HtmlControl)Rpt_HSCTMsg.Items[gEdit_CN].FindControl("Div_Editarea")).Visible = false;

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_MsgEdit').modal('show');</script>", false);//執行js

        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HContent, HFile1, HVideoLink  FROM HSCTMsg WHERE HID ='" + gButton1_CA + "'");

        if (dr.Read())
        {
            CKE_HContent_Edit.Text = dr["HContent"].ToString();
            TB_HVideoLink_Edit.Text = dr["HVideoLink"].ToString();
            LB_HFile1_Edit.Text = dr["HFile1"].ToString();

            if (!string.IsNullOrEmpty(dr["HFile1"].ToString()))
            {
                LBtn_HFile1_EDel.Visible = true;

                string[] filename = dr["HFile1"].ToString().Split('_');
                //HL_HFile1_Edit.Text = dr["HFile1"].ToString();
                HL_HFile1_Edit.Text = "瀏覽檔案";
                HL_HFile1_Edit.NavigateUrl = "~/uploads/HSCTMsg/" + filename[0].Substring(0, 8) + "/" + dr["HFile1"].ToString();
            }

        }
        dr.Close();

    }
    #endregion

    #region 儲存功能
    protected void Btn_MsgEditSubmit_Click(object sender, EventArgs e)
    {
        //MA20240828_加入非本人隱藏留言需輸入原因判斷
        //登入者是否等於留言者
        SqlDataReader userdr = SQLdatabase.ExecuteReader("SELECT HID, HMemberID FROM HSCTMsg WHERE HID = '" + LB_HSCTMsgID.Text + "' ");
        if (userdr.Read())
        {
            if (((Label)Master.FindControl("LB_HUserHID")).Text != userdr["HMemberID"].ToString())
            {
                if (!string.IsNullOrEmpty(TB_HMsgReason_Edit.Text))
                {
                    //Log紀錄
                    SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCTMsg_Log] (HSCTMsgID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
                    ConStr.Open();

                    logcmd.Parameters.AddWithValue("@HSCTMsgID", LB_HSCTMsgID.Text);
                    logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    logcmd.Parameters.AddWithValue("@HLogType", 1);
                    logcmd.Parameters.AddWithValue("@HReason", TB_HMsgReason_Edit.Text.Trim());
                    logcmd.Parameters.AddWithValue("@HStatus", 1);
                    logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                    logcmd.ExecuteNonQuery();
                    ConStr.Close();
                    logcmd.Cancel();

                    TB_HMsgReason_Edit.Text = null;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入編輯留言原因');", true);
                    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_MsgEdit').modal('show');</script>", false);//執行js
                    return;
                }
            }
        }
        userdr.Close();

        SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsg] SET HContent=@HContent, HFile1=@HFile1, HVideoLink=@HVideoLink, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgID.Text);
        cmd.Parameters.AddWithValue("@HContent", CKE_HContent_Edit.Text.Trim());
        cmd.Parameters.AddWithValue("@HFile1", LB_HFile1_Edit.Text.Trim());
        cmd.Parameters.AddWithValue("@HVideoLink", TB_HVideoLink_Edit.Text.Trim());
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();


        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('儲存成功!');", true);

        SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "' AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
        SDS_HSCTMsg.DataBind();
        Rpt_HSCTMsg.DataBind();
    }
    #endregion

    #region 上傳檔案功能
    protected void LBtn_HFile1_Edit_Click(object sender, EventArgs e)
    {
        //判斷檔案路徑是否存在，不存在則建立資料夾 
        if (!Directory.Exists(Server.MapPath("~/uploads/HSCTMsg/" + DateTime.Now.ToString("yyyyMMdd"))))
        {
            Directory.CreateDirectory(Server.MapPath("~/uploads/HSCTMsg/" + DateTime.Now.ToString("yyyyMMdd")));//不存在就建立目錄 
        }

        string gUPFile = "~/uploads/HSCTMsg/" + DateTime.Now.ToString("yyyyMMdd") + "/";
        bool FileIsValidUP1 = false;

        if (FU_HFile1_Edit.HasFile)
        {
            //取得上傳檔案大小
            int fileSize = FU_HFile1_Edit.PostedFile.ContentLength;
            if (fileSize > 200 * 1024 * 1024) //200MB
            {
                LB_HFileMsg1_Edit.Text = "檔案上限為200MB";
                LB_HFileMsg1_Edit.ForeColor = Color.Red;
            }
            else
            {

                string gUPFileMain = Path.GetFileNameWithoutExtension(FU_HFile1_Edit.FileName);//取得上傳檔案的主檔名(沒有副檔名)
                string gUPFileExtension = System.IO.Path.GetExtension(FU_HFile1_Edit.FileName).ToLower(); //取得上傳檔案的副檔名

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
                    LB_HFile1_Edit.Text = DateTime.Now.ToString("yyyyMMddssff") + "_" + gUPFileMain + gUPFileExtension;
                    FU_HFile1_Edit.SaveAs(Server.MapPath(gUPFile) + LB_HFile1_Edit.Text);

                    LB_HFileMsg1_Edit.Text = "上傳成功";
                    //GE20240724_調整樣式
                    LB_HFileMsg1_Edit.Style.Add("color", "#2ebb4e");
                    LBtn_HFile1_EDel.Visible = true;

                }
                else
                {
                    LB_HFileMsg1_Edit.Text = "上傳檔案格式錯誤";
                    LB_HFileMsg1_Edit.ForeColor = Color.Red;

                }


            }
        }
        else
        {
            LB_HFileMsg1_Edit.Text = "請選擇上傳檔案";
            LB_HFileMsg1_Edit.ForeColor = Color.Red;

        }

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_MsgEdit').modal('show');</script>", false);//執行js

    }
    #endregion

    #region 刪除已上傳檔案
    protected void LBtn_HFile1_EDel_Click(object sender, EventArgs e)
    {
        string sourceFile = File1Root + LB_HFile1_Edit.Text;

        if (File.Exists(sourceFile)) File.Delete(sourceFile);
        LB_HFile1_Edit.Text = null;
        LB_HFileMsg1_Edit.Text = null;
        LBtn_HFile1_EDel.Visible = false;
        HL_HFile1_Edit.Text = null;
        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_MsgEdit').modal('show');</script>", false);//執行js

    }
    #endregion

    #endregion

    #region 回應主題-刪除留言
    //留言狀態：dbo.HSCTMsg.HStatus (0:刪除/1:正常/2:隱藏/3:取消隱藏)
    //留言Log類型：dbo.HSCTMsg_Log.HLogType (0:刪除/1:編輯/2:隱藏/3:取消隱藏)
    #region 刪除留言Modal
    protected void LBtn_MsgDel_Click(object sender, EventArgs e)
    {
        LinkButton gButton1 = sender as LinkButton;
        string gButton1_CA = gButton1.CommandArgument;
        //GA20240724_新增取得itemindex
        int gDel_CN = Convert.ToInt32(gButton1.CommandName);
        LB_HSCTMsgID.Text = gButton1_CA;

        //GA20240724_隱藏更多選項的區域
        ((HtmlControl)Rpt_HSCTMsg.Items[gDel_CN].FindControl("Div_Editarea")).Visible = false;

        //MA20240828_加入非本人刪除留言需輸入原因判斷
        //登入者是否等於留言者
        int gMsgMemberID = 0;  //留言者HMemberID


        SqlDataReader userdr = SQLdatabase.ExecuteReader("SELECT HID, HMemberID FROM HSCTMsg WHERE HID = '" + LB_HSCTMsgID.Text + "' ");
        if (userdr.Read())
        {
            gMsgMemberID = Convert.ToInt32(userdr["HMemberID"].ToString());
            if (((Label)Master.FindControl("LB_HUserHID")).Text != userdr["HMemberID"].ToString())
            {
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgDelModal').modal();"), true);
            }
            else
            {
                //變更留言狀態
                SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsg] SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgID.Text);
                cmd.Parameters.AddWithValue("@HStatus", 0);
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                cmd.ExecuteNonQuery();
                ConStr.Close();
                cmd.Cancel();

                //Log紀錄
                SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCTMsg_Log] (HSCTMsgID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
                ConStr.Open();

                logcmd.Parameters.AddWithValue("@HSCTMsgID", LB_HSCTMsgID.Text);
                logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                logcmd.Parameters.AddWithValue("@HLogType", 0);
                logcmd.Parameters.AddWithValue("@HReason", TB_HMsgReason_Del.Text.Trim());
                logcmd.Parameters.AddWithValue("@HStatus", 1);
                logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                logcmd.ExecuteNonQuery();
                ConStr.Close();
                logcmd.Cancel();


                //刪除留言通知
                /// <summary>
                /// 新增通知紀錄
                /// </summary>
                /// <param name="gHMemberID">接收通知者</param>
                /// <param name="gHActorMemberID">觸發通知者</param>
                /// <param name="gHNotifyType">通知類型(1:回應、2:心情(讚)、3:心情(愛心)、4:心情(微笑)、5:分享、6:提問、7:被刪除留言/回應、8:被隱藏留言/回應)</param>
                /// <param name="gHTargetID">對應資料表的流水號</param>
                /// <param name="gTableName">對應資料表名稱</param>
                Notification.AddNotification(gMsgMemberID.ToString(), ((Label)Master.FindControl("LB_HUserHID")).Text, "7", LB_HSCTMsgID.Text, "HSCTMsg");



                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('刪除成功!');", true);

                SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "' AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
                SDS_HSCTMsg.DataBind();
                Rpt_HSCTMsg.DataBind();
            }
        }
        userdr.Close();

        Div_MsgDel.Visible = true;
        Div_MsgResponseDel.Visible = false;


    }
    #endregion

    #region 刪除留言
    protected void Btn_MsgDel_Click(object sender, EventArgs e)
    {

        if (string.IsNullOrEmpty(TB_HMsgReason_Del.Text))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入刪除留言原因');", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgDelModal').modal();"), true);
            return;
        }


        //變更留言狀態
        SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsg] SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgID.Text);
        cmd.Parameters.AddWithValue("@HStatus", 0);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCTMsg_Log] (HSCTMsgID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCTMsgID", LB_HSCTMsgID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 0);
        logcmd.Parameters.AddWithValue("@HReason", TB_HMsgReason_Del.Text.Trim());
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('刪除成功!');", true);

        SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "' AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
        SDS_HSCTMsg.DataBind();
        Rpt_HSCTMsg.DataBind();

    }
    #endregion


    #region 取消隱藏留言
    //GA20241225_新增取消隱藏留言功能
    protected void LBtn_MsgHideCancel_Click(object sender, EventArgs e)
    {
        LinkButton gButton1 = sender as LinkButton;
        string gButton1_CA = gButton1.CommandArgument;
        int gDel_CN = Convert.ToInt32(gButton1.CommandName);
        LB_HSCTMsgID.Text = gButton1_CA;

        //變更留言狀態
        SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsg] SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgID.Text);
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCTMsg_Log] (HSCTMsgID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCTMsgID", LB_HSCTMsgID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 3);
        logcmd.Parameters.AddWithValue("@HReason", TB_HMsgReason_Del.Text.Trim());
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('取消隱藏成功!');", true);

        SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "' AND   (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
        SDS_HSCTMsg.DataBind();
        Rpt_HSCTMsg.DataBind();


    }
    #endregion



    #endregion

    #region 回應主題-隱藏留言
    //留言狀態：dbo.HSCTMsg.HStatus (0:刪除/1:正常/2:隱藏)
    //留言Log類型：dbo.HSCTMsg_Log.HLogType (0:刪除/1:編輯/2:隱藏)
    #region 隱藏留言Modal
    protected void LBtn_MsgHide_Click(object sender, EventArgs e)
    {
        LinkButton gButton1 = sender as LinkButton;
        string gButton1_CA = gButton1.CommandArgument;
        //GA20240724_新增取得itemindex
        int gHide_CN = Convert.ToInt32(gButton1.CommandName);
        LB_HSCTMsgID.Text = gButton1_CA;

        //GA20240724_隱藏更多選項的區域
        ((HtmlControl)Rpt_HSCTMsg.Items[gHide_CN].FindControl("Div_Editarea")).Visible = false;

        //MA20240828_加入非本人隱藏留言需輸入原因判斷
        //登入者是否等於留言者
        int gMsgMemberID = 0;  //留言者HMemberID

        SqlDataReader userdr = SQLdatabase.ExecuteReader("SELECT HID, HMemberID FROM HSCTMsg WHERE HID = '" + LB_HSCTMsgID.Text + "' ");
        if (userdr.Read())
        {
            gMsgMemberID = Convert.ToInt32(userdr["HMemberID"].ToString());

            if (((Label)Master.FindControl("LB_HUserHID")).Text != userdr["HMemberID"].ToString())
            {
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgHideModal').modal();"), true);
            }
            else
            {
                //變更留言狀態
                SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsg] SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgID.Text);
                cmd.Parameters.AddWithValue("@HStatus", 2);
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                cmd.ExecuteNonQuery();
                ConStr.Close();
                cmd.Cancel();

                //Log紀錄
                SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCTMsg_Log] (HSCTMsgID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
                ConStr.Open();

                logcmd.Parameters.AddWithValue("@HSCTMsgID", LB_HSCTMsgID.Text);
                logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                logcmd.Parameters.AddWithValue("@HLogType", 2);
                logcmd.Parameters.AddWithValue("@HReason", TB_HMsgReason_Hide.Text.Trim());
                logcmd.Parameters.AddWithValue("@HStatus", 1);
                logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                logcmd.ExecuteNonQuery();
                ConStr.Close();
                logcmd.Cancel();


                //隱藏留言通知
                /// <summary>
                /// 新增通知紀錄
                /// </summary>
                /// <param name="gHMemberID">接收通知者</param>
                /// <param name="gHActorMemberID">觸發通知者</param>
                /// <param name="gHNotifyType">通知類型(1:回應、2:心情(讚)、3:心情(愛心)、4:心情(微笑)、5:分享、6:提問、7:被刪除留言/回應、8:被隱藏留言/回應)</param>
                /// <param name="gHTargetID">對應資料表的流水號</param>
                /// <param name="gTableName">對應資料表名稱</param>
                Notification.AddNotification(gMsgMemberID.ToString(), ((Label)Master.FindControl("LB_HUserHID")).Text, "8", LB_HSCTMsgID.Text, "HSCTMsg");

                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('隱藏成功!');", true);

                SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "' AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
                SDS_HSCTMsg.DataBind();
                Rpt_HSCTMsg.DataBind();

            }
        }
        userdr.Close();

        Div_MsgHide.Visible = true;
        Div_MsgResponseHide.Visible = false;
    }
    #endregion

    #region 隱藏留言
    protected void Btn_MsgHide_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_HMsgReason_Hide.Text))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入隱藏留言原因');", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgHideModal').modal();"), true);
            return;
        }

        //變更留言狀態
        SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsg] SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgID.Text);
        cmd.Parameters.AddWithValue("@HStatus", 2);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCTMsg_Log] (HSCTMsgID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCTMsgID", LB_HSCTMsgID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 2);
        logcmd.Parameters.AddWithValue("@HReason", TB_HMsgReason_Hide.Text.Trim());
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('隱藏成功!');", true);

        SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "' AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
        SDS_HSCTMsg.DataBind();
        Rpt_HSCTMsg.DataBind();

    }
    #endregion

    #endregion

    #region 回應主題-下載其他文件檔案
    //MA20240726_新增功能
    protected void LBtn_DownloadFile_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_File = sender as LinkButton;
        string File_CA = LBtn_File.CommandArgument;

        string[] filename = File_CA.Split('_');

        //從Server端取得檔案 
        Stream FileStream;
        FileStream = File.OpenRead(Server.MapPath("~/uploads/HSCTMsg/" + filename[0].Substring(0, 8) + "/" + File_CA));

        Byte[] Buf = new byte[FileStream.Length];
        FileStream.Read(Buf, 0, int.Parse(FileStream.Length.ToString()));
        FileStream.Close();

        //準備下載檔案 
        Response.ClearHeaders();
        Response.Clear();
        Response.Expires = 0;
        Response.Buffer = false;
        Response.ContentType = "Application/save-as";
        Response.Charset = "utf-8";
        //透過Header設定檔名 
        Response.AddHeader("Content-Disposition", "Attachment; filename=" + HttpUtility.UrlEncode(File_CA));
        Response.BinaryWrite(Buf);
        Response.End();

    }
    #endregion

    #endregion

    #region 回應留言
    //MA20240718_新增回應留言-送出回應功能
    //MA20240720_編輯回應功能、刪除回應功能
    //MA20240828_加入回應留言編輯/刪除/隱藏顯示權限判斷
    #region 回應留言-送出回應
    protected void LBtn_HSubmitMsgResponse_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Submit = sender as LinkButton;
        int index = Convert.ToInt32(LBtn_Submit.CommandArgument);

        if (string.IsNullOrEmpty(((HtmlTextArea)Rpt_HSCTMsg.Items[index].FindControl("TA_HMsgResponse")).InnerText))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入想要回應留言的內容');", true);
            return;
        }

        SqlCommand cmd = new SqlCommand("INSERT INTO [HSCTMsgResponse] (HSCTMsgID, HMemberID, HMsgResponse, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgID, @HMemberID, @HMsgResponse, @HStatus, @HCreate, @HCreateDT);SELECT SCOPE_IDENTITY() AS HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HSCTMsgID", ((Label)Rpt_HSCTMsg.Items[index].FindControl("LB_HSCTMsgID")).Text);
        cmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HMsgResponse", ((HtmlTextArea)Rpt_HSCTMsg.Items[index].FindControl("TA_HMsgResponse")).InnerText);
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        //cmd.ExecuteNonQuery();
        string HID = cmd.ExecuteScalar().ToString();
        ConStr.Close();
        cmd.Cancel();

        //回應留言通知
        /// <summary>
        /// 新增通知紀錄
        /// </summary>
        /// <param name="gHMemberID">接收通知者</param>
        /// <param name="gHActorMemberID">觸發通知者</param>
        /// <param name="gHNotifyType">通知類型(1:回應、2:心情(讚)、3:心情(愛心)、4:心情(微笑)、5:分享、6:提問)</param>
        /// <param name="gHTargetID">對應資料表的流水號</param>
        /// <param name="gTableName">對應資料表名稱</param>
        Notification.AddNotification(LB_HMemberID.Text, ((Label)Master.FindControl("LB_HUserHID")).Text, "1", HID, "HSCTMsgResponse");


        ((HtmlTextArea)Rpt_HSCTMsg.Items[index].FindControl("TA_HMsgResponse")).InnerText = null;

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('新增成功!');", true);

        Repeater Rpt_HSCTMsgResponse = ((Repeater)Rpt_HSCTMsg.Items[index].FindControl("Rpt_HSCTMsgResponse")) as Repeater;  //找到repeater物件
        SqlDataSource SDS_HSCTMsgResponse = ((SqlDataSource)Rpt_HSCTMsg.Items[index].FindControl("SDS_HSCTMsgResponse")) as SqlDataSource;   //找到SqlDataSource物件        

        SDS_HSCTMsgResponse.SelectCommand = HSCTMsgResponseCom + " WHERE a.HSCTMsgID='" + ((Label)Rpt_HSCTMsg.Items[index].FindControl("LB_HSCTMsgID")).Text + "' AND  (a.HStatus=1 OR a.HStatus=2)";
        SDS_HSCTMsgResponse.DataBind();
        Rpt_HSCTMsgResponse.DataBind();

        //隱藏我要回應的區域
        ((Panel)Rpt_HSCTMsg.Items[index].FindControl("Panel_ReplyMsg")).Visible = false;


    }
    #endregion

    #region 回應留言-資料繫結
    protected void Rpt_HSCTMsgResponse_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //大頭照顯示
        UserImgShow(((Label)e.Item.FindControl("LB_HImg")).Text, ((Image)e.Item.FindControl("IMG_HImg")));

        #region 編輯回應顯示判斷
        int auth = 0;

        //最高權限管理者(討論區管理員)
        SqlDataReader strSQLdr = SQLdatabase.ExecuteReader("SELECT HID, HSCForumClassID, HMemberID FROM HSCModeratorSetting WHERE HSCForumClassID='" + LB_HSCForumClassID.Text + "' AND (HMemberID LIKE '%" + "," + ((Label)Master.FindControl("LB_HUserHID")).Text + "," + "%')");

        if (strSQLdr.Read())
        {
            auth = auth + 1;
        }
        else
        {
            //登入者是否等於留言者
            SqlDataReader userdr = SQLdatabase.ExecuteReader("SELECT HID, HMemberID FROM HSCTMsgResponse WHERE HID = '" + ((Label)e.Item.FindControl("LB_HID")).Text + "'");

            if (userdr.Read())
            {
                if (((Label)Master.FindControl("LB_HUserHID")).Text == userdr["HMemberID"].ToString())
                {
                    auth = auth + 1;

                }
                else
                {
                    auth = 0;

                }
            }
            userdr.Close();
        }

        if (auth > 0)
        {
            //MA20240828_有權限則開啟點點點的按鈕
            ((LinkButton)e.Item.FindControl("LBtn_ReplyMore")).Visible = true;
        }
        else
        {
            //GA20240828_沒有權限則隱藏點點點的按鈕
            ((LinkButton)e.Item.FindControl("LBtn_ReplyMore")).Visible = false;
        }
        #endregion

        //GA20240829_加入點擊後會先跳提示訊息，避免誤點
        ((LinkButton)e.Item.FindControl("LBtn_MsgResponseDel")).Attributes.Add("onclick", "return confirm('是否確認要刪除?')");
        ((LinkButton)e.Item.FindControl("LBtn_MsgResponseHide")).Attributes.Add("onclick", "return confirm('是否確認要隱藏?')");


        //GA20241225_判斷回應是否被隱藏
        if (((Label)e.Item.FindControl("LB_HStatus")).Text == "2" && (((Label)e.Item.FindControl("LB_HMemberID")).Text == ((Label)Master.FindControl("LB_HUserHID")).Text))
        {
            ((HtmlGenericControl)e.Item.FindControl("teachermsg")).Visible = true;
            ((HtmlGenericControl)e.Item.FindControl("Div_Status")).Visible = true;
            ((HtmlGenericControl)e.Item.FindControl("Li_MsgResponseHide")).Visible = false;
            ((HtmlGenericControl)e.Item.FindControl("Div_Editmore")).Visible = true;
        }
        else if (((Label)e.Item.FindControl("LB_HStatus")).Text == "2" && (((Label)e.Item.FindControl("LB_HMemberID")).Text != ((Label)Master.FindControl("LB_HUserHID")).Text))
        {
            ((HtmlGenericControl)e.Item.FindControl("teachermsg")).Visible = false;
            ((HtmlGenericControl)e.Item.FindControl("Div_Editmore")).Visible = false;
        }



        //GA20241228_加入語音檔
        string gUPFileExtension = System.IO.Path.GetExtension(((Label)e.Item.FindControl("LB_HVoice")).Text).ToLower();
        if (gUPFileExtension == ".mp3") //語音檔
        {
            ((HtmlControl)e.Item.FindControl("AudioResponse")).Visible = true;
            ((HtmlAudio)e.Item.FindControl("AudioResponse")).Src = "~/uploads/Voice/" + ((Label)e.Item.FindControl("LB_HVoice")).Text;
        }

    }
    #endregion

    #region 回應留言-編輯回應

    #region 編輯回應Modal
    protected void LBtn_MsgResponseEdit_Click(object sender, EventArgs e)
    {
        LinkButton gButton1 = sender as LinkButton;
        LB_HSCTMsgResponseID.Text = gButton1.CommandArgument;
        //GA20240724_加入回應第幾筆&判斷
        int gMsgResponse_CN = Convert.ToInt32(gButton1.CommandName);

        //GA20240829_抓父層(上一層)的repeater寫法(用NamingContainer)
        var gChildRepeaterItem = (RepeaterItem)gButton1.NamingContainer;
        var gChildRepeater = (Repeater)gChildRepeaterItem.NamingContainer;
        var gParentRepeaterItem = (RepeaterItem)gChildRepeater.NamingContainer;
        var gParentRepeaterItemIndex = gParentRepeaterItem.ItemIndex;

        Repeater Rpt_HSCTMsgResponse = ((Repeater)Rpt_HSCTMsg.Items[gParentRepeaterItemIndex].FindControl("Rpt_HSCTMsgResponse")) as Repeater;
        ((HtmlControl)Rpt_HSCTMsgResponse.Items[gMsgResponse_CN].FindControl("Div_ReplyArea")).Visible = false;


        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_MsgResponseEdit').modal('show');</script>", false);//執行js

        SqlDataReader dr = SQLdatabase.ExecuteReader("SELECT HMsgResponse FROM HSCTMsgResponse WHERE HID ='" + LB_HSCTMsgResponseID.Text + "'");

        if (dr.Read())
        {
            TA_HMsgResponse_Edit.InnerText = dr["HMsgResponse"].ToString();
        }
        dr.Close();

    }
    #endregion

    #region 儲存功能
    protected void Btn_MsgREitdSubmit_Click(object sender, EventArgs e)
    {
        // EA20240829_加入非本人隱藏留言需輸入原因判斷
        //登入者是否等於留言者
        SqlDataReader userdr = SQLdatabase.ExecuteReader("SELECT HID, HMemberID FROM HSCTMsgResponse WHERE HID = '" + LB_HSCTMsgResponseID.Text + "' ");
        if (userdr.Read())
        {
            if (((Label)Master.FindControl("LB_HUserHID")).Text != userdr["HMemberID"].ToString())
            {
                if (!string.IsNullOrEmpty(TB_HMsgResReason_Edit.Text))
                {
                    //Log紀錄
                    SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCTMsgResponse_Log] (HSCTMsgResponseID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgResponseID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
                    ConStr.Open();

                    logcmd.Parameters.AddWithValue("@HSCTMsgResponseID", LB_HSCTMsgResponseID.Text);
                    logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    logcmd.Parameters.AddWithValue("@HLogType", 1);
                    logcmd.Parameters.AddWithValue("@HReason", TB_HMsgResReason_Edit.Text.Trim());
                    logcmd.Parameters.AddWithValue("@HStatus", 1);
                    logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                    logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                    logcmd.ExecuteNonQuery();
                    ConStr.Close();
                    logcmd.Cancel();

                    TB_HMsgResReason_Edit.Text = null;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入編輯回應原因');", true);
                    ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_MsgResponseEdit').modal('show');</script>", false);//執行js
                    return;
                }
            }
        }
        userdr.Close();

        SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsgResponse] SET HMsgResponse=@HMsgResponse, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgResponseID.Text);
        cmd.Parameters.AddWithValue("@HMsgResponse", TA_HMsgResponse_Edit.InnerText);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();


        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('儲存成功!');", true);


        SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "' AND a.HStatus=1 ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
        SDS_HSCTMsg.DataBind();
        Rpt_HSCTMsg.DataBind();

    }
    #endregion


    #endregion

    #region 回應留言-刪除回應
    //回應留言狀態：dbo.HSCTMsgResponse.HStatus (0:刪除/1:正常/2:隱藏)
    //回應留言Log類型：dbo.HSCTMsgResponse_Log.HLogType (0:刪除/1:編輯/2:隱藏)
    #region 刪除留言Modal
    protected void LBtn_MsgResponseDel_Click(object sender, EventArgs e)
    {

        LinkButton gButton1 = sender as LinkButton;
        LB_HSCTMsgResponseID.Text = gButton1.CommandArgument;
        //GA20240724_加入回應第幾筆&判斷
        int gMsgResponse_CN = Convert.ToInt32(gButton1.CommandName);

        //GA20240829_抓父層(上一層)的repeater寫法(用NamingContainer)
        var gChildRepeaterItem = (RepeaterItem)gButton1.NamingContainer;
        var gChildRepeater = (Repeater)gChildRepeaterItem.NamingContainer;
        var gParentRepeaterItem = (RepeaterItem)gChildRepeater.NamingContainer;
        var gParentRepeaterItemIndex = gParentRepeaterItem.ItemIndex;

        Repeater Rpt_HSCTMsgResponse = ((Repeater)Rpt_HSCTMsg.Items[gParentRepeaterItemIndex].FindControl("Rpt_HSCTMsgResponse")) as Repeater;
        ((HtmlControl)Rpt_HSCTMsgResponse.Items[gMsgResponse_CN].FindControl("Div_ReplyArea")).Visible = false;

        //MA20240829_加入非本人刪除留言需輸入原因判斷
        //登入者是否等於留言者
        SqlDataReader userdr = SQLdatabase.ExecuteReader("SELECT HID, HMemberID FROM HSCTMsgResponse WHERE HID = '" + LB_HSCTMsgResponseID.Text + "' ");
        if (userdr.Read())
        {
            if (((Label)Master.FindControl("LB_HUserHID")).Text != userdr["HMemberID"].ToString())
            {
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgDelModal').modal();"), true);
            }
            else
            {
                //變更留言狀態
                SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsgResponse] SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgResponseID.Text);
                cmd.Parameters.AddWithValue("@HStatus", 0);
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                cmd.ExecuteNonQuery();
                ConStr.Close();
                cmd.Cancel();

                //Log紀錄
                SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCTMsgResponse_Log] (HSCTMsgResponseID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgResponseID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
                ConStr.Open();

                logcmd.Parameters.AddWithValue("@HSCTMsgResponseID", LB_HSCTMsgResponseID.Text);
                logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                logcmd.Parameters.AddWithValue("@HLogType", 0);
                logcmd.Parameters.AddWithValue("@HReason", TB_MsgResponse_Del.Text.Trim());
                logcmd.Parameters.AddWithValue("@HStatus", 1);
                logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                logcmd.ExecuteNonQuery();
                ConStr.Close();
                logcmd.Cancel();

                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('刪除成功!');", true);

                SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "' AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
                SDS_HSCTMsg.DataBind();
                Rpt_HSCTMsg.DataBind();
            }
        }
        userdr.Close();



        Div_MsgDel.Visible = false;
        Div_MsgResponseDel.Visible = true;
    }
    #endregion

    #region 刪除留言
    protected void Btn_MsgResponseDel_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_MsgResponse_Del.Text))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入刪除回應原因');", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgDelModal').modal();"), true);
            return;
        }

        //變更留言狀態
        SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsgResponse] SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgResponseID.Text);
        cmd.Parameters.AddWithValue("@HStatus", 0);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCTMsgResponse_Log] (HSCTMsgResponseID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgResponseID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCTMsgResponseID", LB_HSCTMsgResponseID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 0);
        logcmd.Parameters.AddWithValue("@HReason", TB_MsgResponse_Del.Text.Trim());
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('刪除成功!');", true);

        SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "' AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
        SDS_HSCTMsg.DataBind();
        Rpt_HSCTMsg.DataBind();

    }
    #endregion




    #endregion

    #region 回應留言-隱藏回應
    //回應留言狀態：dbo.HSCTMsgResponse.HStatus (0:刪除/1:正常/2:隱藏/3:取消隱藏)
    //回應留言Log類型：dbo.HSCTMsgResponse_Log.HLogType (0:刪除/1:編輯/2:隱藏/3:取消隱藏)
    #region 隱藏回應Modal
    protected void LBtn_MsgResponseHide_Click(object sender, EventArgs e)
    {
        LinkButton gButton1 = sender as LinkButton;
        LB_HSCTMsgResponseID.Text = gButton1.CommandArgument;
        //GA20240724_加入回應第幾筆&判斷
        int gMsgResponse_CN = Convert.ToInt32(gButton1.CommandName);

        //GA20240829_抓父層(上一層)的repeater寫法(用NamingContainer)
        var gChildRepeaterItem = (RepeaterItem)gButton1.NamingContainer;
        var gChildRepeater = (Repeater)gChildRepeaterItem.NamingContainer;
        var gParentRepeaterItem = (RepeaterItem)gChildRepeater.NamingContainer;
        var gParentRepeaterItemIndex = gParentRepeaterItem.ItemIndex;

        Repeater Rpt_HSCTMsgResponse = ((Repeater)Rpt_HSCTMsg.Items[gParentRepeaterItemIndex].FindControl("Rpt_HSCTMsgResponse")) as Repeater;
        ((HtmlControl)Rpt_HSCTMsgResponse.Items[gMsgResponse_CN].FindControl("Div_ReplyArea")).Visible = false;

        //MA20240828_加入非本人隱藏留言需輸入原因判斷
        //登入者是否等於留言者
        SqlDataReader userdr = SQLdatabase.ExecuteReader("SELECT HID, HMemberID FROM HSCTMsgResponse WHERE HID = '" + LB_HSCTMsgResponseID.Text + "' ");
        if (userdr.Read())
        {
            if (((Label)Master.FindControl("LB_HUserHID")).Text != userdr["HMemberID"].ToString())
            {
                ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgHideModal').modal();"), true);
            }
            else
            {
                //變更留言狀態
                SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsgResponse] SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgResponseID.Text);
                cmd.Parameters.AddWithValue("@HStatus", 2);
                cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
                cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                cmd.ExecuteNonQuery();
                ConStr.Close();
                cmd.Cancel();

                //Log紀錄
                SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCTMsgResponse_Log] (HSCTMsgResponseID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgResponseID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
                ConStr.Open();

                logcmd.Parameters.AddWithValue("@HSCTMsgResponseID", LB_HSCTMsgResponseID.Text);
                logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
                logcmd.Parameters.AddWithValue("@HLogType", 2);
                logcmd.Parameters.AddWithValue("@HReason", TB_MsgResponse_Hide.Text.Trim());
                logcmd.Parameters.AddWithValue("@HStatus", 1);
                logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
                logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                logcmd.ExecuteNonQuery();
                ConStr.Close();
                logcmd.Cancel();

                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('隱藏成功!');", true);

                SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "' AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
                SDS_HSCTMsg.DataBind();
                Rpt_HSCTMsg.DataBind();
            }
        }
        userdr.Close();

        Div_MsgHide.Visible = false;
        Div_MsgResponseHide.Visible = true;
    }
    #endregion

    #region 隱藏回應
    protected void Btn_MsgResponseHide_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(TB_MsgResponse_Hide.Text))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alter", "alert('請輸入隱藏回應原因');", true);
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Modal", ("$('#Div_MsgHideModal').modal();"), true);
            return;
        }

        //變更留言狀態
        SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsgResponse] SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgResponseID.Text);
        cmd.Parameters.AddWithValue("@HStatus", 2);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCTMsgResponse_Log] (HSCTMsgResponseID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgResponseID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCTMsgResponseID", LB_HSCTMsgResponseID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 2);
        logcmd.Parameters.AddWithValue("@HReason", TB_MsgResponse_Hide.Text.Trim());
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('隱藏成功!');", true);

        SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "' AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
        SDS_HSCTMsg.DataBind();
        Rpt_HSCTMsg.DataBind();
    }
    #endregion


    #region 取消隱藏回應功能
    //GA20241225_新增取消隱藏回應功能
    protected void LBtn_MsgResponseHideCancel_Click(object sender, EventArgs e)
    {
        //變更留言狀態
        SqlCommand cmd = new SqlCommand("UPDATE [HSCTMsgResponse] SET HStatus=@HStatus, HModify=@HModify, HModifyDT=@HModifyDT WHERE HID=@HID", ConStr);
        ConStr.Open();

        cmd.Parameters.AddWithValue("@HID", LB_HSCTMsgResponseID.Text);
        cmd.Parameters.AddWithValue("@HStatus", 3);
        cmd.Parameters.AddWithValue("@HModify", ((Label)Master.FindControl("LB_HUserHID")).Text);
        cmd.Parameters.AddWithValue("@HModifyDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

        //Log紀錄
        SqlCommand logcmd = new SqlCommand("INSERT INTO [HSCTMsgResponse_Log] (HSCTMsgResponseID, HMemberID, HLogType, HReason, HStatus, HCreate, HCreateDT ) VALUES ( @HSCTMsgResponseID, @HMemberID, @HLogType, @HReason, @HStatus, @HCreate, @HCreateDT)", ConStr);
        ConStr.Open();

        logcmd.Parameters.AddWithValue("@HSCTMsgResponseID", LB_HSCTMsgResponseID.Text);
        logcmd.Parameters.AddWithValue("@HMemberID", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HLogType", 3);
        logcmd.Parameters.AddWithValue("@HReason", TB_MsgResponse_Hide.Text.Trim());
        logcmd.Parameters.AddWithValue("@HStatus", 1);
        logcmd.Parameters.AddWithValue("@HCreate", ((Label)Master.FindControl("LB_HUserHID")).Text);
        logcmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

        logcmd.ExecuteNonQuery();
        ConStr.Close();
        logcmd.Cancel();

        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "", "alert('取消隱藏回應成功!');", true);

        SDS_HSCTMsg.SelectCommand = HSCTMsgCom + " WHERE a.HSCTopicID='" + LB_TID.Text + "' AND  (a.HStatus=1 OR a.HStatus=2) ORDER BY a.HCreateDT DESC, c.HLearningType DESC";
        SDS_HSCTMsg.DataBind();
        Rpt_HSCTMsg.DataBind();
    }
    #endregion


    #endregion

    #endregion

    #region 主題心情項目(按讚、愛心、笑臉)
    protected void LBtn_HSCTopicMood_Click(object sender, EventArgs e)
    {

        //if (Div_TFeelingArea.Visible == false)
        //{
        //    Div_TFeelingArea.Visible = true;
        //}
        //else
        //{
        //    Div_TFeelingArea.Visible = false;
        //}

    }
    #endregion

    #region 留言心情項目(按讚、愛心、笑臉)
    protected void LBtn_HSCTMsgMood_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_HSCTMsgMood = sender as LinkButton;

        string gFeeling_CA = LBtn_HSCTMsgMood.CommandArgument;
        int gFeeling_CN = 0;

        if (!string.IsNullOrEmpty(LBtn_HSCTMsgMood.CommandName))
        {
            gFeeling_CN = Convert.ToInt32(LBtn_HSCTMsgMood.CommandName);
        }
        //判斷若已經有點開某一個，則要隱藏
        for (int i = 0; i < Rpt_HSCTMsg.Items.Count; i++)
        {
            if (i != gFeeling_CN)
            {
                ((HtmlControl)Rpt_HSCTMsg.Items[i].FindControl("Div_FeelingArea")).Visible = false;
            }
        }

        if (((HtmlControl)Rpt_HSCTMsg.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible == false)
        {
            ((HtmlControl)Rpt_HSCTMsg.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible = true;
        }
        else
        {
            ((HtmlControl)Rpt_HSCTMsg.Items[gFeeling_CN].FindControl("Div_FeelingArea")).Visible = false;
        }



    }
    #endregion

    #region Function

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
        //資料讀取
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


    #region 心情數量計算
    private string MsgFeelingCounts(string SCTMsgID)
    {
        //資料讀取
        System.Data.DataTable dtFeeling = SQLdatabase.ExecuteDataTable("SELECT HMemberID, HType FROM HSCTMsg_Mood WHERE HSCTMsgID='" + SCTMsgID + "'");

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
    private int ResponseCounts(string SCTopicID)
    {
        //資料讀取
        SqlDataReader QueryResponse = SQLdatabase.ExecuteReader("SELECT Count(HID) AS Num FROM HSCTMsg  WHERE HSCTopicID='" + SCTopicID + "' AND HStatus=1 UNION ALL SELECT Count(a.HID) AS Num FROM HSCTMsg AS a  JOIN HSCTMsgResponse AS b ON a.HID= b.HSCTMsgID WHERE a.HSCTopicID='" + SCTopicID + "' AND a.HStatus=1 AND b.HStatus=1");

        int gResponse = 0;

        while (QueryResponse.Read())
        {
            gResponse += Convert.ToInt32(QueryResponse["Num"].ToString());
        }
        QueryResponse.Close();

        //LB_MsgNum.Text = gResponse.ToString("N0");

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

    #endregion

    #region HashTag標籤連結
    protected void LBtn_HHashTag_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_HSCHotHashTag = sender as LinkButton;
        string gHSCHotHashTag_CA = LBtn_HSCHotHashTag.CommandArgument;

        string Sql = "SELECT a.HID, c.HSCFCName AS HSCForumClassB, b.HSCFCName AS HSCForumClassC, a.HTopicName, a.HPinTop, a.HContent, a.HFile1 , (e.HSystemName + ' ' + e.HArea + '/' + e.HPeriod + ' ' + e.HUserName) AS UserName, e.HImg, a.HCreateDT, a.HModifyDT, d.HCTemplateID FROM HSCTopic AS a LEFT JOIN HSCForumClass AS b ON a.HSCForumClassID = b.HID LEFT JOIN HSCForumClass AS c ON b.HSCFCMaster = c.HID LEFT JOIN HSCCTopicSetting AS d ON a.HSCForumClassID = d.HSCForumClassID LEFT JOIN MemberList AS e ON a.HCreate = e.HID WHERE a.HStatus = 1 AND ((a.HHashTag LIKE '%" + gHSCHotHashTag_CA + "%')) ORDER BY a.HPinTop DESC, a.HCreateDT DESC";

        Session["SearchIndex"] = Sql.ToString();
        Response.Redirect("HSCIndex.aspx");

    }
    #endregion

    #region 分享連結
    protected void Btn_Share_Click(object sender, EventArgs e)
    {
        if (Request.Cookies["TopicID"] != null)
        {
            if (Request.Cookies["TopicID"].Value != "undefined")
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

            }

            //清除cookie
            Response.Cookies["TopicID"].Expires = DateTime.Now.AddDays(-1);
        }



    }
    #endregion

    //專欄PDF樣版
    private string PDFLayout(string num)
    {
        string strReturn = string.Empty;
        strReturn += "<div class='text-center pdf-toolbar'>";
        strReturn += " <div class='btn-group'>";
        strReturn += "  <button id = 'prev" + num + "' class='btn btn-white' onclick='return false;'><i class='fa fa-arrow-left'></i> <span class='hidden-xs'>上一頁</span></button>";
        strReturn += "  <button id = 'next" + num + "' class='btn btn-white' onclick='return false;'><i class='fa fa-arrow-right'></i> <span class='hidden-xs'>下一頁</span></button>";
        strReturn += "  <button id = 'zoomin" + num + "' class='btn btn-white' onclick='return false;'><i class='fa fa-search-plus'></i> <span class='hidden-xs'>放大</span></button>";
        strReturn += "  <button id = 'zoomout" + num + "' class='btn btn-white' onclick='return false;'><i class='fa fa-search-minus'></i> <span class='hidden-xs'>縮小</span> </button>";
        strReturn += "  <button id = 'zoomfit" + num + "' class='btn btn-white' onclick='return false;'> 100%</button>";
        strReturn += "  <span class='btn btn-white hidden-xs'>頁數: </span>";

        strReturn += "  <div class='input-group' style='width:20%'>";
        strReturn += "      <input type = 'text' class='form-control' id='page_num" + num + "'>";
        strReturn += "      <div class='input-group-btn'>";
        strReturn += "          <button type = 'button' class='btn btn-white' id='page_count" + num + "'> /0</button>";
        strReturn += "      </div>";
        strReturn += "  </div>";
        strReturn += "  </div>";
        strReturn += "</div>";
        strReturn += "<div class='text-center m-t-md'>";
        strReturn += "  <canvas id='the-canvas" + num + "'  class='pdfcanvas border-left-right border-top-bottom b-r-md'></canvas>";
        strReturn += "</div>";
        return strReturn;
    }

    //課後教材PDF樣版
    private string ACPDFLayout(string num)
    {
        string strReturn = string.Empty;
        strReturn += "<div class='text-center pdf-toolbar'>";
        strReturn += " <div class='btn-group'>";
        strReturn += "  <button id = 'prev" + num + "' class='btn btn-white' onclick='return false;'><i class='fa fa-arrow-left'></i> <span class='hidden-xs'>上一頁</span></button>";
        strReturn += "  <button id = 'next" + num + "' class='btn btn-white' onclick='return false;'><i class='fa fa-arrow-right'></i> <span class='hidden-xs'>下一頁</span></button>";
        strReturn += "  <button id = 'zoomin" + num + "' class='btn btn-white' onclick='return false;'><i class='fa fa-search-plus'></i> <span class='hidden-xs'>放大</span></button>";
        strReturn += "  <button id = 'zoomout" + num + "' class='btn btn-white' onclick='return false;'><i class='fa fa-search-minus'></i> <span class='hidden-xs'>縮小</span> </button>";
        strReturn += "  <button id = 'zoomfit" + num + "' class='btn btn-white' onclick='return false;'> 100%</button>";
        strReturn += "  <span class='btn btn-white hidden-xs'>頁數: </span>";

        strReturn += "  <div class='input-group' style='width:20%'>";
        strReturn += "      <input type = 'text' class='form-control' id='page_num" + num + "'>";
        strReturn += "      <div class='input-group-btn'>";
        strReturn += "          <button type = 'button' class='btn btn-white' id='page_count" + num + "'> /0</button>";
        strReturn += "      </div>";
        strReturn += "  </div>";
        strReturn += "  </div>";
        strReturn += "</div>";
        strReturn += "<div class='text-center m-t-md'>";
        strReturn += "  <canvas id='the-canvas" + num + "'  class='pdfcanvas border-left-right border-top-bottom b-r-md'></canvas>";
        strReturn += "</div>";
        return strReturn;
    }




    #region 語音回應
    //GA20241227_新增語音回應
    protected void LBtn_VoiceMsg_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_VoiceMsg = sender as LinkButton;
        int index = Convert.ToInt32(LBtn_VoiceMsg.CommandArgument);
        //string gButton1_CN = LBtn_VoiceMsg.CommandArgument;  //被回應的MsgHID
        //ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#AudioModal').modal('show');</script>", false);//執行js

        //呼叫func_audioreply
        ScriptManager.RegisterStartupScript(this, GetType(), "audioreply", "func_audioreply(" + ((Label)Rpt_HSCTMsg.Items[index].FindControl("LB_HSCTMsgID")).Text + ", " + ((Label)Rpt_HSCTMsg.Items[index].FindControl("LB_HMemberID")).Text + ");", true);
    }
    #endregion

    #region 上傳錄音
    protected void Btn_SendVoiceMsg_Click(object sender, EventArgs e)
    {

    }
    #endregion


    #region 我要留言
    protected void LBtn_Msg_Click(object sender, EventArgs e)
    {
        Panel_Comment.Visible = true;
    }
    #endregion


    #region 主題-心情功能
    //HType：1=讚 /  2=愛心 / 3=微笑
    protected void LBtn_FeelingType_Click(object sender, EventArgs e)
    {
        LinkButton FeelingTypeBtn = sender as LinkButton;
        //string gFeelingType_CA = FeelingTypeBtn.CommandArgument;
        //int gFeelingType_CN = Convert.ToInt32(btn.CommandName);

        //LB_TID.Text = gFeelingType_CA;




        SqlDataReader QueryFeeling = SQLdatabase.ExecuteReader("SELECT HID, HSCTopicID, HMemberID, HType FROM HSCTopic_Mood WHERE HSCTopicID='" + LB_TID.Text + "' AND HMemberID='" + ((Label)Master.FindControl("LB_HUserHID")).Text + "'");
        if (QueryFeeling.Read())
        {
            if (FeelingTypeBtn.TabIndex.ToString() != QueryFeeling["HType"].ToString())
            {
                SqlCommand cmd = new SqlCommand("UPDATE [HSCTopic_Mood] SET HType=@HType, HModify=@HModify, HModifyDT=@HModifyDT WHERE HSCTopicID=@HSCTopicID AND HMemberID=@HMemberID", ConStr);
                ConStr.Open();

                cmd.Parameters.AddWithValue("@HSCTopicID", LB_TID.Text);
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

            cmd.Parameters.AddWithValue("@HSCTopicID", LB_TID.Text);
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


    }
    #endregion


    #region 瀏覽紀錄
    protected void LBtn_Seen_Click(object sender, EventArgs e)
    {
        LB_Head.Text = LTR_HTopicName.Text + "<br/>讀訊人數";

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_SeenRecord').modal('show');</script>", false);//執行js

        System.Data.DataTable dtHSCTopic_View = SQLdatabase.ExecuteDataTable("SELECT  (b.HArea +' '+ISNULL(b.HSystemName,'無體系')  +' / '+HPeriod+' '+HUserName)   AS HUserName, a.HTimes, a.HCreateDT AS FirstTime, a.HModifyDT AS LatestTime FROM HSCTopic_View AS a JOIN MemberList AS b ON a.HMemberID= b.HID WHERE a.HStatus=1 AND a.HSCTopicID='" + LB_TID.Text + "'");

        Rpt_HSCTopic_View.DataSource = dtHSCTopic_View;
        Rpt_HSCTopic_View.DataBind();

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

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

        //預設顯示按讚的資訊
        LBtn_ThumbsUpM.CssClass = "nav-link active show font-weight-bold";
        LBtn_HeartM.CssClass = "nav-link font-weight-bold";
        LBtn_SmileM.CssClass = "nav-link font-weight-bold";

        SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_TID.Text + "' AND a.HType='1' AND a.HStatus=1";

        //計算數量
        FeelingCounts(LB_TID.Text);
    }
    #endregion


    #region 按愛心數量名單查看
    protected void LBtn_LoveNum_Click(object sender, EventArgs e)
    {
        LB_HSCType.Text = "1";

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

        //預設顯示按讚的資訊
        LBtn_ThumbsUpM.CssClass = "nav-link font-weight-bold";
        LBtn_HeartM.CssClass = "nav-link active show  font-weight-bold";
        LBtn_SmileM.CssClass = "nav-link font-weight-bold";

        SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_TID.Text + "' AND a.HType='2' AND a.HStatus=1";

        //計算數量
        FeelingCounts(LB_TID.Text);
    }
    #endregion


    #region 按笑臉數量名單查看
    protected void LBtn_SmileNum_Click(object sender, EventArgs e)
    {
        LB_HSCType.Text = "1";

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

        //預設顯示按讚的資訊
        LBtn_ThumbsUpM.CssClass = "nav-link font-weight-bold";
        LBtn_HeartM.CssClass = "nav-link  font-weight-bold";
        LBtn_SmileM.CssClass = "nav-link active show font-weight-bold";

        SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_TID.Text + "' AND a.HType='3' AND a.HStatus=1";

        //計算數量
        FeelingCounts(LB_TID.Text);
    }
    #endregion



    #region 主題-心情功能
    //HType：1=讚 /  2=愛心 / 3=微笑
    protected void LBtn_FeelingTypeM_Click(object sender, EventArgs e)
    {

        LinkButton btn = (LinkButton)sender;

        //LB_HSCType ==1: HSCTopic、2=HSCTMsg
        if (LB_HSCType.Text == "1")
        {
            if (btn.TabIndex.ToString() == "1")
            {
                LBtn_ThumbsUpM.CssClass = "nav-link  active show font-weight-bold";
                LBtn_HeartM.CssClass = "nav-link font-weight-bold";
                LBtn_SmileM.CssClass = "nav-link font-weight-bold";

                SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_TID.Text + "' AND a.HType='1' AND a.HStatus=1 ";

            }
            else if (btn.TabIndex.ToString() == "2")
            {
                LBtn_ThumbsUpM.CssClass = "nav-link font-weight-bold";
                LBtn_HeartM.CssClass = "nav-link  active show font-weight-bold";
                LBtn_SmileM.CssClass = "nav-link font-weight-bold";

                SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_TID.Text + "' AND a.HType='2' AND a.HStatus=1";

            }
            else if (btn.TabIndex.ToString() == "3")
            {
                LBtn_ThumbsUpM.CssClass = "nav-link font-weight-bold";
                LBtn_HeartM.CssClass = "nav-link font-weight-bold";
                LBtn_SmileM.CssClass = "nav-link  active show font-weight-bold";

                SDS_HSCTopicMood.SelectCommand = "SELECT a.HType, a.HMemberID, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTopic_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTopicID='" + LB_TID.Text + "' AND a.HType='3' AND a.HStatus=1";

            }
            //計算數量
            FeelingCounts(LB_TID.Text);
        }
        else if (LB_HSCType.Text == "2")
        {
            for (int x = 0; x < Rpt_HSCTMsg.Items.Count; x++)
            {
                if (btn.TabIndex.ToString() == "1")
                {
                    LBtn_ThumbsUpM.CssClass = "nav-link  active show font-weight-bold";
                    LBtn_HeartM.CssClass = "nav-link font-weight-bold";
                    LBtn_SmileM.CssClass = "nav-link font-weight-bold";

                    SDS_HSCTopicMood.SelectCommand = "SELECT a.HID, a.HSCTMsgID, a.HMemberID, a.HType, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTMsg_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTMsgID='" + ((Label)Rpt_HSCTMsg.Items[x].FindControl("LB_HSCTMsgID")).Text + "' AND a.HType='1' AND a.HStatus=1";

                }
                else if (btn.TabIndex.ToString() == "2")
                {
                    LBtn_ThumbsUpM.CssClass = "nav-link font-weight-bold";
                    LBtn_HeartM.CssClass = "nav-link  active show font-weight-bold";
                    LBtn_SmileM.CssClass = "nav-link font-weight-bold";

                    SDS_HSCTopicMood.SelectCommand = "SELECT a.HID, a.HSCTMsgID, a.HMemberID, a.HType, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTMsg_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTMsgID='" + ((Label)Rpt_HSCTMsg.Items[x].FindControl("LB_HSCTMsgID")).Text + "' AND a.HType='2' AND a.HStatus=1";

                }
                else if (btn.TabIndex.ToString() == "3")
                {
                    LBtn_ThumbsUpM.CssClass = "nav-link font-weight-bold";
                    LBtn_HeartM.CssClass = "nav-link font-weight-bold";
                    LBtn_SmileM.CssClass = "nav-link  active show font-weight-bold";

                    SDS_HSCTopicMood.SelectCommand = "SELECT a.HID, a.HSCTMsgID, a.HMemberID, a.HType, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTMsg_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTMsgID='" + ((Label)Rpt_HSCTMsg.Items[x].FindControl("LB_HSCTMsgID")).Text + "' AND a.HType='3' AND a.HStatus=1";

                }
                MsgFeelingCounts(((Label)Rpt_HSCTMsg.Items[x].FindControl("LB_HSCTMsgID")).Text);
            }



        }



        Rpt_HSCTopicMood.DataBind();



        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

    }
    #endregion


    #region 心情紀錄(Modal資料繫結)
    protected void Rpt_HSCTopicMood_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView gDRV = (DataRowView)e.Item.DataItem;

        UserImgShow(gDRV["HImg"].ToString(), ((Image)e.Item.FindControl("Img_HImg")));
    }
    #endregion


    #region 按讚數量名單查看
    protected void LBtn_MsgThumbsUpNum_Click(object sender, EventArgs e)
    {
        LB_HSCType.Text = "2";

        LinkButton btn = (LinkButton)sender;
        int index = Convert.ToInt32(btn.CommandArgument);

        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

        //預設顯示按讚的資訊
        LBtn_ThumbsUpM.CssClass = "nav-link active show font-weight-bold";
        LBtn_HeartM.CssClass = "nav-link font-weight-bold";
        LBtn_SmileM.CssClass = "nav-link font-weight-bold";

        SDS_HSCTopicMood.SelectCommand = "SELECT a.HID, a.HSCTMsgID, a.HMemberID, a.HType, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTMsg_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTMsgID='" + ((Label)Rpt_HSCTMsg.Items[index].FindControl("LB_HSCTMsgID")).Text + "' AND a.HType='1' AND a.HStatus=1";

        Rpt_HSCTopicMood.DataBind();

        //計算數量
        MsgFeelingCounts(((Label)Rpt_HSCTMsg.Items[index].FindControl("LB_HSCTMsgID")).Text);

    }
    #endregion


    #region 按愛心數量名單查看
    protected void LBtn_MsgLoveNum_Click(object sender, EventArgs e)
    {
        LB_HSCType.Text = "2";

        LinkButton btn = (LinkButton)sender;
        int index = Convert.ToInt32(btn.CommandArgument);

        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

        //預設顯示按讚的資訊
        LBtn_ThumbsUpM.CssClass = "nav-link font-weight-bold";
        LBtn_HeartM.CssClass = "nav-link active show  font-weight-bold";
        LBtn_SmileM.CssClass = "nav-link font-weight-bold";

        SDS_HSCTopicMood.SelectCommand = "SELECT a.HID, a.HSCTMsgID, a.HMemberID, a.HType, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTMsg_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTMsgID='" + ((Label)Rpt_HSCTMsg.Items[index].FindControl("LB_HSCTMsgID")).Text + "' AND a.HType='2' AND a.HStatus=1";

        Rpt_HSCTopicMood.DataBind();

        //計算數量
        MsgFeelingCounts(((Label)Rpt_HSCTMsg.Items[index].FindControl("LB_HSCTMsgID")).Text);

    }
    #endregion

    #region 按笑臉數量名單查看
    protected void LBtn_MsgSmileNum_Click(object sender, EventArgs e)
    {
        LB_HSCType.Text = "2";

        LinkButton btn = (LinkButton)sender;
        int index = Convert.ToInt32(btn.CommandArgument);

        var IBtn = sender as IButtonControl;
        RepeaterItem RI = (sender as LinkButton).NamingContainer as RepeaterItem;

        ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "<script>$('#Div_Smiley').modal('show');</script>", false);//執行js

        //預設顯示按讚的資訊
        LBtn_ThumbsUpM.CssClass = "nav-link font-weight-bold";
        LBtn_HeartM.CssClass = "nav-link font-weight-bold";
        LBtn_SmileM.CssClass = "nav-link active show font-weight-bold";

        SDS_HSCTopicMood.SelectCommand = "SELECT a.HID, a.HSCTMsgID, a.HMemberID, a.HType, (ISNULL(b.HSystemName,'無體系')+' '+ b.HArea+'/'+HPeriod+' '+HUserName) AS HUserName, b.HImg FROM HSCTMsg_Mood AS a JOIN MemberList AS b ON a.HMemberID=b.HID WHERE a.HSCTMsgID='" + ((Label)Rpt_HSCTMsg.Items[index].FindControl("LB_HSCTMsgID")).Text + "' AND a.HType='3' AND a.HStatus=1";

        Rpt_HSCTopicMood.DataBind();

        //計算數量
        MsgFeelingCounts(((Label)Rpt_HSCTMsg.Items[index].FindControl("LB_HSCTMsgID")).Text);

    }
    #endregion

    #region 我要回應
    protected void LBtn_Reply_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Reply = sender as LinkButton;
        int index = Convert.ToInt32(LBtn_Reply.CommandArgument);

        ((Panel)Rpt_HSCTMsg.Items[index].FindControl("Panel_ReplyMsg")).Visible = true;

    }
    #endregion

    #region 取消留言
    protected void LBtn_HSubmitMsgCancel_Click(object sender, EventArgs e)
    {
        Panel_Comment.Visible = false;
    }
    #endregion

    #region 取消回應
    protected void LBtn_HSubmitMsgResponseCancel_Click(object sender, EventArgs e)
    {
        LinkButton LBtn_Reply = sender as LinkButton;
        int index = Convert.ToInt32(LBtn_Reply.CommandArgument);

        ((Panel)Rpt_HSCTMsg.Items[index].FindControl("Panel_ReplyMsg")).Visible = false;
    }
    #endregion
}