using System;
using System.Data;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Activities.Expressions;

/// <summary>
/// Notification 的摘要描述
/// 專欄各式通知紀錄
/// </summary>
public class Notification
{
    SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    public Notification()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //

    }

    /// <summary>
    /// 新增通知紀錄
    /// </summary>
    /// <param name="gHMemberID">接收通知者</param>
    /// <param name="gHActorMemberID">觸發通知者</param>
    /// <param name="gHNotifyType">通知類型(1:回應、2:心情(讚)、3:心情(愛心)、4:心情(微笑)、5:分享、6:提問)、7:被刪除留言/回應、8:被隱藏留言/回應)</param>
    /// <param name="gHTargetID">對應資料表的流水號</param>
    /// <param name="gTableName">對應資料表名稱</param>
    static public void AddNotification(string gHMemberID, string gHActorMemberID, string gHNotifyType, string gHTargetID, string gTableName)
    {
        SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);
        SqlCommand cmd = new SqlCommand("INSERT INTO HNotification (HMemberID, HActorMemberID, HNotifyType, HTargetID, HTableName, HReadStatus, HStatus, HCreate, HCreateDT) VALUES (@HMemberID, @HActorMemberID, @HNotifyType, @HTargetID, @HTableName, @HReadStatus, @HStatus, @HCreate, @HCreateDT)", ConStr);

        ConStr.Open();
        cmd.Parameters.AddWithValue("@HMemberID", gHMemberID);
        cmd.Parameters.AddWithValue("@HActorMemberID", gHActorMemberID);
        cmd.Parameters.AddWithValue("@HNotifyType", gHNotifyType);
        cmd.Parameters.AddWithValue("@HTargetID", gHTargetID);
        cmd.Parameters.AddWithValue("@HTableName", gTableName);
        cmd.Parameters.AddWithValue("@HReadStatus", 0);
        cmd.Parameters.AddWithValue("@HStatus", 1);
        cmd.Parameters.AddWithValue("@HCreate", gHActorMemberID);
        cmd.Parameters.AddWithValue("@HCreateDT", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
        ConStr.Close();
        cmd.Cancel();

    }
}