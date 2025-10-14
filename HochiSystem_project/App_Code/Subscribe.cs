using System;
using System.Data;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;
using System.Text;

public partial class Subscribe
{
    static public void Notice()
    {
        #region 寄件資訊<正式>
        //string Sender = "mail.edu@hochi.org.tw";  //寄信人地址
        //string EAcount = "mail.edu@hochi.org.tw";  //寄信人帳號
        //string EPasword = "love1234";
        //string EHost = "smtp.gmail.com";  //寄信伺服器
        //int EPort = 587;
        //bool EEnabledSSL = true;

        string Sender = MailConfig.Sender;
        string EAcount = MailConfig.Account;
        string EPasword = MailConfig.Password;
        string EHost = MailConfig.Host;
        int EPort = MailConfig.Port;
        bool EEnabledSSL = MailConfig.EnableSSL;
        #endregion

        #region 測試
        //string Sender = "testmail@tecgenic.com";  //寄信人地址
        //string EAcount = "testmail@tecgenic.com";  //寄信人帳號
        //string EPasword = "1qaz@test79";
        //string EHost = "mail.tecgenic.com";    //寄信伺服器
        //int EPort = 25;
        //bool EEnabledSSL = false;
        #endregion



        string Email = null;
        string Member = null;
        string MemberID = null;

        string RemainPoints = null;
        string BCPoints = null;

        string NewCourseID = null;

        #region   撈出上課起始日+7為今天日期的所有帶狀課程，並撈出所有3個月前報名過上一堂帶狀課程的學員名單(姓名+信箱)
        SqlDataReader QueryInfo = SQLdatabase.ExecuteReader("SELECT  D.HID, D.HAccount as Email, D.HUserName,C.HCourseID AS CourseID, SUM(ISNULL(E.HBuy, 0)) AS HBuy, SUM(ISNULL(E.HUse, 0)) AS HUse FROM Course_BackendList AS A  JOIN HCourse_T AS B ON A.HCTemplateID = B.HID JOIN HCourseBooking AS C ON A.HID = C.HCourseID JOIN HMember AS D ON C.HMemberID = D.HID JOIN HPoints AS E ON E.HMemberID = D.HID  WHERE C.HCourseID =(SELECT  A.HSCourseID FROM  HCourse AS A INNER JOIN HCourse_T AS B ON A.HCTemplateID = B.HID WHERE A.HVerifyStatus = '2' AND B.HSerial = 1  AND DateDiff(dd, getdate(), SUBSTRING(HDateRange, 1, 10)) = 7) AND DateDiff(MONTH, getdate(),C.HBDate) < 3 GROUP BY D.HID, D.HAccount , D.HUserName, C.HCourseID");

        while (QueryInfo.Read())
        {
            Member = QueryInfo["HUserName"].ToString();
            MemberID = QueryInfo["HID"].ToString();
            Email += QueryInfo["Email"].ToString() + ",";

            #region  自動續報開始

            //STEP 1 判斷學員點數是否足夠
            RemainPoints = Convert.ToString(Convert.ToInt32(QueryInfo["HBuy"].ToString()) - Convert.ToInt32(QueryInfo["HUse"].ToString()));   //尚餘點數

            //STEP 2 取得要幫忙報名的課程ID
            SqlDataReader QueryNewCourse = SQLdatabase.ExecuteReader("SELECT  A.HID, A.HSCourseID FROM  HCourse AS A INNER JOIN HCourse_T AS B ON A.HCTemplateID = B.HID WHERE A.HVerifyStatus = '2'  AND B.HSerial = 1  AND DateDiff(dd, getdate(), SUBSTRING(HDateRange, 1, 10)) = 7");
            if (QueryNewCourse.Read())
            {
                NewCourseID = QueryNewCourse["HID"].ToString();
            }
            QueryNewCourse.Close();

            //STEP 3 判斷學員點數是否足夠
            SqlDataReader QueryCourse = SQLdatabase.ExecuteReader("SELECT A.HCourseName, A.HBCPoint, B.HPlaceName FROM HCourse AS A JOIN HPlace AS B ON A.HOCPlace = B.HID  WHERE A.HID='" + NewCourseID + "'");
            if (QueryCourse.Read())
            {
                BCPoints = QueryCourse["HBCPoint"].ToString();

                if (Convert.ToInt32(RemainPoints) < Convert.ToInt32(QueryCourse["HBCPoint"].ToString()))  //點數不足，寄信通知學員
                {
                    #region 寄信通知學員
                    //信件本體宣告
                    MailMessage mail = new MailMessage();
                    // 寄件者, 收件者和副本郵件地址        
                    mail.From = new MailAddress(Sender, "和氣大愛玉成系統自動寄信機器人");
                    // 設定收件者
                    mail.To.Add(Email);
                    // 優先等級
                    mail.Priority = MailPriority.Normal;
                    // 主旨
                    mail.Subject = "和氣大愛玉成系統 - 帶狀課程續報失敗通知，點數不足請儲值點數";
                    //信件內容         
                    mail.Body = "<p>" + Member + @"，您好 ~ </p><br/><p>【" + QueryCourse["HCourseName"].ToString() + "_" + QueryCourse["HPlaceName"].ToString() + "】因點數不足，續報失敗~</p><br/><p>請您先儲值點數並報名課程，謝謝~。</p><hr/> " +
                           "<p style='font-weight:bold;'>此郵件為和氣大愛玉成系統自動寄出，請勿回信。</p>";

                    mail.IsBodyHtml = true;
                    mail.SubjectEncoding = Encoding.UTF8;
                    mail.BodyEncoding = Encoding.GetEncoding("utf-8");
                    SmtpClient smtpServer = new SmtpClient();
                    smtpServer.Credentials = new System.Net.NetworkCredential(EAcount, EPasword);
                    smtpServer.Port = EPort;
                    smtpServer.Host = EHost;
                    smtpServer.EnableSsl = EEnabledSSL;
                    try
                    {
                        // 寄出郵件
                        smtpServer.Send(mail);
                    }
                    catch (Exception ex)
                    {
                    }
                    #endregion
                }
                else
                {
                    //如果尚未報名才執行
                    SqlDataReader QueryRegister = SQLdatabase.ExecuteReader("SELECT HMemberID, HCourseID FROM HCourseBooking WHERE HMemberID='" + MemberID + "' AND HCourseID='" + NewCourseID + "'");

                    if (QueryRegister.Read())
                    {
                        //已報名，不做任何事
                    }
                    else
                    {
                        SqlConnection dbConn = default(SqlConnection);
                        SqlCommand dbCmd = default(SqlCommand);
                        string strDBConn = null;
                        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
                        dbConn = new SqlConnection(strDBConn);
                        dbConn.Open();

                        //自動報名
                        string strOther = "IF EXISTS(SELECT HOrderNum FROM HCourseBooking WHERE SUBSTRING(HOrderNum,2,8) = CONVERT(nvarchar, getdate(), 112)) " +
                         "BEGIN " +
                           "IF EXISTS(SELECT HMemberID, HCourseID FROM HCourseBooking WHERE HMemberID=@HMemberID AND HCourseID=@HCourseID) " +
                         "BEGIN " +
                         "UPDATE [HCourseBooking] SET HStatus=1 WHERE HMemberID=@HMemberID AND HCourseID=@HCourseID " +
                          "END " +
                          "ELSE " +
                         "INSERT INTO HCourseBooking (HCourseID, HMemberID, HOrderNum, HPMethod, HAttend, HBDate, HLDiscount, HPoint,  HStatus, HCreate, HCreateDT) VALUES (@HCourseID, @HMemberID,'B' + CONVERT(nvarchar, (SELECT CONVERT(numeric, Right(Max(HOrderNum), 12)) FROM HCourseBooking) + 1), @HPMethod, @HAttend, @HBDate, @HLDiscount, @HPoint, @HStatus, @HCreate, @HCreateDT);" +
                         "END " +
                         "ELSE " +
                         "BEGIN " +
                         " INSERT INTO HCourseBooking (HCourseID, HMemberID, HOrderNum, HPMethod, HAttend, HBDate, HLDiscount, HPoint, HStatus, HCreate, HCreateDT) VALUES (@HCourseID, @HMemberID,'B' + CONVERT(nvarchar, getdate(), 112) + '0001', @HPMethod, @HAttend, @HBDate, @HLDiscount, @HPoint,  @HStatus, @HCreate, @HCreateDT); " +
                         "END ";


                        SqlCommand cmdDetail = new SqlCommand(strOther, dbConn);

                        cmdDetail.Parameters.AddWithValue("@HCourseID", NewCourseID);
                        cmdDetail.Parameters.AddWithValue("@HMemberID", MemberID);
                        cmdDetail.Parameters.AddWithValue("@HPMethod", "4");  //1:線上刷卡、2:線上ATM、3:超商繳費、4:其他
                        cmdDetail.Parameters.AddWithValue("@HAttend", "1");
                        cmdDetail.Parameters.AddWithValue("@HBDate", DateTime.Now.ToString("yyyy/MM/dd"));
                        cmdDetail.Parameters.AddWithValue("@HLDiscount", "0");
                        cmdDetail.Parameters.AddWithValue("@HPoint", "0");
                        cmdDetail.Parameters.AddWithValue("@HStatus", 1);   // 1:訂單取消、2:已付款、3:訂單成立(未付款)/付款失敗
                        cmdDetail.Parameters.AddWithValue("@HCreate", MemberID);
                        cmdDetail.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                        //cmdDetail.Parameters.AddWithValue("@HRemark", TA_Remark.InnerText);
                        cmdDetail.ExecuteNonQuery();
                        cmdDetail.Cancel();

                        //扣除點數  HPayMethod= 1:線上刷卡、2:線上ATM、3:超商繳費、9:其他
                        string strInsHP = "Insert Into HPoints (HMemberID, HRecrodDate, HPayMethod, HAmount, HRecord, HUse, HUseFor, HStatus, HCreate, HCreateDT) values ('" + MemberID + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '4', '0', '系統自動進行「帶狀課程續報」扣除點數', '" + BCPoints + "', '扣除點數', '1', '" + MemberID + "', '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "')";
                        dbCmd = new SqlCommand(strInsHP, dbConn);
                        dbCmd.ExecuteReader();
                        dbConn.Close();
                        dbCmd.Cancel();


                        #region 寄信通知學員
                        //信件本體宣告
                        MailMessage mail = new MailMessage();
                        // 寄件者, 收件者和副本郵件地址        
                        mail.From = new MailAddress(Sender, "和氣大愛玉成系統自動寄信機器人");
                        // 設定收件者
                        mail.To.Add(Email.TrimEnd(','));
                        // 優先等級
                        mail.Priority = MailPriority.Normal;
                        // 主旨
                        mail.Subject = "和氣大愛玉成系統 - 帶狀課程續報成功通知";
                        //信件內容         
                        mail.Body = "<p>" + Member + @"，您好 ~ </p><br/><p>【" + QueryCourse["HCourseName"].ToString() + "_" + QueryCourse["HPlaceName"].ToString() + "】已續報完成~</p><br/><p>可至學員專區-課程資訊管理中進行確認，謝謝~。</p><hr/> " +
                               "<p style='font-weight:bold;'>此郵件為和氣大愛玉成系統自動寄出，請勿回信。</p>";

                        mail.IsBodyHtml = true;
                        mail.SubjectEncoding = Encoding.UTF8;
                        mail.BodyEncoding = Encoding.GetEncoding("utf-8");
                        SmtpClient smtpServer = new SmtpClient();
                        smtpServer.Credentials = new System.Net.NetworkCredential(EAcount, EPasword);
                        smtpServer.Port = EPort;
                        smtpServer.Host = EHost;
                        smtpServer.EnableSsl = EEnabledSSL;
                        try
                        {
                            // 寄出郵件
                            smtpServer.Send(mail);
                        }
                        catch (Exception ex)
                        {
                        }
                        #endregion


                    }
                    QueryRegister.Close();



                }
            }
            QueryCourse.Close();
            #endregion

        }
        QueryInfo.Close();
        #endregion





    }





}
