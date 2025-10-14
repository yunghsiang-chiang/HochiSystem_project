using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Text;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Security.Policy;
using System.Configuration;
using System.Data.SqlClient;

/// <summary>
/// SmsSender 的摘要描述
/// </summary>
public class SmsSender
{
    public static string SendSms(string phoneNumber, string OTP)
    {
        string url = "https://api.e8d.tw/API21/HTTP/sendSMS.ashx?";

        string gMessage = "您好，感謝您註冊和氣大愛玉成系統，請輸入驗證碼「"+ OTP + "」，以完成驗證程序，謝謝!";
        // 將 message 進行 URL Encode，避免中文字出錯
        string encodedMessage = Uri.EscapeDataString(gMessage);
        string postData = "UID=17870945&PWD=Love123hochi&DEST=" + phoneNumber + "&MSG=" + encodedMessage;

        var SMSUrl = url + postData;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SMSUrl);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(postData);
        request.ContentLength = bs.Length;
        request.GetRequestStream().Write(bs, 0, bs.Length);
        //取得 WebResponse 的物件 然後把回傳的資料讀出
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader sr = new StreamReader(response.GetResponseStream());
        string gResult = sr.ReadToEnd();
        return gResult;

    }


    public static string SendSmsResetPW(string phoneNumber, string OTP)
    {
        string url = "https://api.e8d.tw/API21/HTTP/sendSMS.ashx?";

        string gMessage = "您好，您在和氣大愛玉成系統申請忘記密碼，請輸入驗證碼「" + OTP + "」重設您的密碼，謝謝!";
        // 將 message 進行 URL Encode，避免中文字出錯
        string encodedMessage = Uri.EscapeDataString(gMessage);
        string postData = "UID=17870945&PWD=Love123hochi&DEST=" + phoneNumber + "&MSG=" + encodedMessage;

        var SMSUrl = url + postData;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SMSUrl);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(postData);
        request.ContentLength = bs.Length;
        request.GetRequestStream().Write(bs, 0, bs.Length);
        //取得 WebResponse 的物件 然後把回傳的資料讀出
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader sr = new StreamReader(response.GetResponseStream());
        string gResult = sr.ReadToEnd();
        return gResult;

    }

}
