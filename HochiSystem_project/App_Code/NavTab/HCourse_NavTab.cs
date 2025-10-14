using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// HCourseTemp_Tab 的摘要描述
/// </summary>
public class HCourse_NavTab
{
    public class NavTabItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Visible { get; set; }
    }


    public class HCourseTemp_NavTab
    {
        public static readonly List<NavTabItem> NavTabs = new List<NavTabItem>
{
    new NavTabItem { ID = 1, Name = "課程範本資訊", Visible = true },
    new NavTabItem { ID = 2, Name = "內文", Visible = true },
    new NavTabItem { ID = 3, Name = "學員課程教材", Visible = true },
    new NavTabItem { ID = 4, Name = "體系護持工作項目", Visible = true },
    new NavTabItem { ID = 5, Name = "作業", Visible = false },
    new NavTabItem { ID = 6, Name = "講師教材", Visible = true },
};


    }
}