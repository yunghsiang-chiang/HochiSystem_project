using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class System_UpdateSCClassSort : System.Web.UI.Page
{
    SqlConnection ConStr = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        // 初始化頁面時需要的操作
    }

    // 接收 AJAX 請求的 Web 方法
    [WebMethod]
    public static string UpdateOrder(List<NestableItem> items)
    {
        try
        {
            // 更新數據庫中的排序結構
            UpdateDatabaseWithNewOrder(items);
            return "Order updated successfully!";
        }
        catch (Exception ex)
        {
            return "Error: "+ex.Message;
        }
    }

    // 遞歸解析 JSON 結構並更新數據庫
    private static void UpdateDatabaseWithNewOrder(List<NestableItem> items, int? parentId = null)
    {
        foreach (var item in items)
        {

            // 查詢現有項目資料
            var existingItem = GetItemFromDatabase(item.Id);

            // 如果排序或層級發生變化則更新
            if (existingItem.ParentId != parentId || existingItem.OrderIndex != item.Order)
            {
                UpdateItemInDatabase(item.Id, parentId, item.Order);
            }

            // 遞歸處理子節點
            if (item.Children != null && item.Children.Count > 0)
            {
                UpdateDatabaseWithNewOrder(item.Children, item.Id);
            }
        }
    }

    // 模擬數據庫查詢
    private static Item GetItemFromDatabase(int id)
    {
        // 模擬查詢：請根據實際的數據庫查詢邏輯進行替換
        SqlConnection dbConn = default(SqlConnection);
        SqlCommand dbCmd = default(SqlCommand);
        string strDBConn = null;
        strDBConn = ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString;
        dbConn = new SqlConnection(strDBConn);
        dbConn.Open();


        string strSelHQSCForumClass = "SELECT HID, HSCFCMaster, HSCFCLevel, HSort FROM HSCForumClass WHERE HID = '" + id + "'";
        dbCmd = new SqlCommand(strSelHQSCForumClass, dbConn);


        //ParentId:層級；OrderIndex:排序
        int gSCFCMaster =0;
        int gSort = 0;

        SqlDataReader QueryHQSCForumClass = dbCmd.ExecuteReader();
        if (QueryHQSCForumClass.Read())
        {
            gSCFCMaster =!string.IsNullOrEmpty(QueryHQSCForumClass["HSCFCMaster"].ToString()) ? Convert.ToInt32(QueryHQSCForumClass["HSCFCMaster"].ToString()):0;
            gSort = Convert.ToInt32(QueryHQSCForumClass["HSort"].ToString());
        }
        QueryHQSCForumClass.Close();

        dbConn.Close();

        return new Item { Id = id, ParentId = gSCFCMaster, OrderIndex = gSort };

    }

    // 模擬數據庫更新
    private static void UpdateItemInDatabase(int id, int? parentId, int order)
    {
        // 請根據實際的數據庫更新邏輯進行替換
        //Console.WriteLine("Updating Item {id}: ParentId = {parentId}, OrderIndex = {order}");
    }
}

// 定義項目結構
public class NestableItem
{
    public int Id { get; set; }
    public int Order { get; set; }
    public List<NestableItem> Children { get; set; }
}

// 模擬數據庫項目結構
public class Item
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public int OrderIndex { get; set; }
}