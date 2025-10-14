using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// BookingLog 的摘要描述
/// 產生log紀錄
/// </summary>
public class BookingLog
{

	static public void Log(string Page, string HOrderGroup, string HTradeNo, string HAccount, string HPayMethod, string HPayAmt, string HStatus, string HItemStatus, string HModify)
	{
		//更新的log紀錄
		if (!Directory.Exists("D:\\WebSite\\System\\HochiSystem\\file\\Bookinglog"))
		{
			//建立資料夾
			Directory.CreateDirectory("D:\\WebSite\\System\\HochiSystem\\file\\Bookinglog");
		}

		if (!File.Exists("D:\\WebSite\\System\\HochiSystem\\file\\Bookinglog" + "\\" + "Log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt"))
		{
			//建立檔案
			File.Create("D:\\WebSite\\System\\HochiSystem\\file\\Bookinglog" + "\\" + "Log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt").Close();
		}

		using (StreamWriter sw = File.AppendText("D:\\WebSite\\System\\HochiSystem\\file\\Bookinglog" + "\\" + "Log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt"))
		{
			//WriteLine為換行 
			sw.Write(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "更新的訂單資訊 ：");
			sw.WriteLine("執行的頁面：" + Page);
			sw.WriteLine("訂單代碼："+ HOrderGroup);
			sw.WriteLine("綠界訂單編號：" + HTradeNo);
			sw.WriteLine("使用者帳號："+ HAccount);
			sw.WriteLine("付款方式：" + HPayMethod);
			sw.WriteLine("付款金額：" + HPayAmt);
			sw.WriteLine("訂單狀態：" + HStatus);
			sw.WriteLine("付款狀態：" + HItemStatus);
			sw.WriteLine("修改者：" + HModify);
			sw.WriteLine("");
		}


		

	}


	
}