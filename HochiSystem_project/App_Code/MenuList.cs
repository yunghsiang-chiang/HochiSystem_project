using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// 權限判斷用類別
/// </summary>
public class MenuList
{
    //結構
    public class MenuConvertItem
    {
        public string MenuCode { get; set; }
        public string MenuPageName { get; set; }                
    }
    public List<MenuConvertItem> ConvertList { get; set; }

    /// <summary>
    /// 頁面名稱及權限代號的轉換
    /// </summary>
    /// <param name="PageName">頁面名稱</param>
    /// <returns>權限代號，如果無法轉換則輸出x</returns>
    public string Convert(string PageName)
    {
        //表列 頁面關鍵字及權限代號轉換
        MenuList Permission = new MenuList();
        Permission.ConvertList = new List<MenuConvertItem>();

        //學員及權限管理模組
        Permission.ConvertList.Add(new MenuConvertItem { MenuCode = "A01", MenuPageName = "HMember_" });
        Permission.ConvertList.Add(new MenuConvertItem { MenuCode = "A02", MenuPageName = "HSystem_" });
        Permission.ConvertList.Add(new MenuConvertItem { MenuCode = "A03", MenuPageName = "HRoles_" });
        //檢覈模組
        Permission.ConvertList.Add(new MenuConvertItem { MenuCode = "B01", MenuPageName = "HTest_" });
        Permission.ConvertList.Add(new MenuConvertItem { MenuCode = "B02", MenuPageName = "HQualify_" });
        //課程模組
        Permission.ConvertList.Add(new MenuConvertItem { MenuCode = "C01", MenuPageName = "HCourseTemplate_" });
        Permission.ConvertList.Add(new MenuConvertItem { MenuCode = "C02", MenuPageName = "HCourse_" });
        Permission.ConvertList.Add(new MenuConvertItem { MenuCode = "C03", MenuPageName = "HReport" });
        //簽核管理
        Permission.ConvertList.Add(new MenuConvertItem { MenuCode = "D01", MenuPageName = "HCourseVerify" });
        Permission.ConvertList.Add(new MenuConvertItem { MenuCode = "D02", MenuPageName = "HTransferVerify" });
        //訂單管理
        Permission.ConvertList.Add(new MenuConvertItem { MenuCode = "E01", MenuPageName = "HManageOrder" });
        //點名管理
        Permission.ConvertList.Add(new MenuConvertItem { MenuCode = "F01", MenuPageName = "HRollcall" });
        //最新消息管理
        Permission.ConvertList.Add(new MenuConvertItem { MenuCode = "G01", MenuPageName = "HNews_" });

        //判斷
        bool isContain = false;
        string MenuCode = "x";
        for (int x=0;x< Permission.ConvertList.Count;x++)
        {
            string MenuPageName = Permission.ConvertList[x].MenuPageName.ToLower();
            isContain = PageName.ToString().ToLower().Contains(MenuPageName);
            if (isContain == true)
            {
                MenuCode = Permission.ConvertList[x].MenuCode;
            }
            isContain = false;
        }

        return MenuCode;
    }

    /// <summary>
    /// 判斷持有的權限
    /// </summary>
    /// <param name="PageName">頁面名稱</param>
    /// <param name="UserHID">使用者的HID</param>
    /// <returns>回傳權限，R：閲讀，W：可讀可編輯，x：沒有權限</returns>
    public string PermissionCheck(string PageName, string UserHID)
    {
        string Result = "x";

        //頁面名稱及權限代號的轉換
        MenuList Mlist = new MenuList();
        string MenuCode = Mlist.Convert(PageName);

        if (MenuCode!="x")
        {            
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["HochiSystemConnection"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT HBackUserTable.HID,HBackUserTable.HBAccount,HBackUserTable.HBUMenuID,HBackUserMenu.HMenu FROM  HBackUserTable INNER JOIN HBackUserMenu ON HBackUserTable.HBUMenuID = HBackUserMenu.HID WHERE HBackUserTable.HID = @HID", con);
                con.Open();
                cmd.Parameters.AddWithValue("@HID", UserHID);
                using (SqlDataReader MyEBF = cmd.ExecuteReader())
                {
                    if (MyEBF.Read())
                    {
                        string HMenu = MyEBF["HMenu"].ToString();

                        //反序列化                    
                        Menu Get = JsonConvert.DeserializeObject<Menu>(HMenu);

                        //判斷
                        for (int x = 0; x < Get.MenuList.Count; x++)
                        {
                            if (Get.MenuList[x].MenuCode == MenuCode)
                            {
                                Result = Get.MenuList[x].Apply;
                            }
                        }
                    }
                }
                cmd.Cancel();
            }
        }
        else
        {
            Result = "x";
        }

        return Result;
    }

}