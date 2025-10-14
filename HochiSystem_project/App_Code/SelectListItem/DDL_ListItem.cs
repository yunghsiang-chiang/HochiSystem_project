using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// DDL_ListItem 的摘要描述
/// </summary>
public class DDL_ListItem
{
    public class HFeedback
    {
        public static readonly Dictionary<int, string> Category = new Dictionary<int, string>
        {
            { 1, "建議" },
            { 2, "改善(問題回報)" },
        };

        public static readonly Dictionary<int, string> EventStatus = new Dictionary<int, string>
        {
            { 1, "待處理" },
            { 2, "處理中" },
            { 3, "處理完成" },
            //{ 4, "退回/需補充資訊" },
        };
    }


    public class HTeacher
    {
        public static readonly Dictionary<int, string> HTeacherClass = new Dictionary<int, string>
        {
            { 1, "識透講師" },
            { 2, "玉成講師" },
            { 3, "教練" },
            { 4, "傳承師" },
        };

        public static readonly Dictionary<int, string> HTearcherLV = new Dictionary<int, string>
        {
            { 1, "見習" },
            { 2, "初任" },
            { 3, "正式" },
        };
    }

}