using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// RBL_ListItem 的摘要描述
/// </summary>
public class RBL_ListItem
{
    //九宮格類型
    public class HSCJiugonggeType
    {
        public static readonly Dictionary<int, string> ListItem = new Dictionary<int, string>
        {
            { 1, "身" },
            { 2, "心" },
            { 3, "靈" },
            { 4, "人" },
            { 5, "事" },
            { 6, "境" },
            { 7, "金錢" },
            { 8, "關係" },
            { 9, "時間" }
        };
    }

}