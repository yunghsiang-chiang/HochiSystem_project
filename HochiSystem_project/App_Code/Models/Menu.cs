using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 權限使用的模型
/// </summary>
public class Menu
{
    public class MenuItem
    {
        public string MenuCode { get; set; }
        public string Apply { get; set; }
    }
    public List<MenuItem> MenuList{ get; set; }
}