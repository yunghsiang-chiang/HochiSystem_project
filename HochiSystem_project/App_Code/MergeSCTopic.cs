using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// MergeSCTopic 的摘要描述
/// </summary>
public class MergeSCTopic
{
    public MergeSCTopic()
    {
        //
        // TODO: 在這裡新增建構函式邏輯
        //
    }


    public struct ParsedTitle
    {
        public string Core;
        public string Date;
    }

    public static ParsedTitle ParseTitle(string title)
    {
        var result = new ParsedTitle();
        if (string.IsNullOrEmpty(title)) return result;

        string[] parts = title.Split('_');
        if (parts.Length >= 2)
        {
            string possibleDate = parts[parts.Length - 1];
            bool isDate = true;
            if (possibleDate.Length == 8)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (!char.IsDigit(possibleDate[i]))
                    {
                        isDate = false;
                        break;
                    }
                }
            }
            else
            {
                isDate = false;
            }

            if (isDate)
            {
                string core = "";
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    core += (i > 0 ? "_" : "") + parts[i];
                }
                result.Core = core;
                result.Date = possibleDate;
            }
        }

        return result;
    }



    public static DataTable MergeTopicRecords(DataTable dtMSSQL, DataTable dtMySQL)
    {
        Dictionary<string, DataRow> mssqlTopicMap = new Dictionary<string, DataRow>();

        // 1. 建立 MSSQL 主題索引（以 "主題核心+日期" 為 Key）
        foreach (DataRow row in dtMSSQL.Rows)
        {
            string title = row["HTopicName"].ToString();
            var parsed = ParseTitle(title);
            if (string.IsNullOrEmpty(parsed.Core) || string.IsNullOrEmpty(parsed.Date)) continue;

            string key = parsed.Core + "_" + parsed.Date;
            if (!mssqlTopicMap.ContainsKey(key))
            {
                mssqlTopicMap.Add(key, row);
            }
        }

        // 2. 建立最終合併結果表 unifiedTable
        DataTable unifiedTable = dtMSSQL.Clone();

        // 3. 先將 MSSQL 資料加入 unifiedTable
        foreach (DataRow row in dtMSSQL.Rows)
        {
            DataRow newRow = unifiedTable.NewRow();
            newRow.ItemArray = row.ItemArray.Clone() as object[];
            unifiedTable.Rows.Add(newRow);
        }

        // 4. 處理 MySQL 資料
        foreach (DataRow row in dtMySQL.Rows)
        {
            string lid = row["HID"].ToString();
            string title = row["Title"].ToString();
            string content = row["Content"].ToString();

            //如果有br要替換<br/>，讓文字自動換行
            int brIndex = title.IndexOf("br", StringComparison.OrdinalIgnoreCase);
            string extra = "";
            if (brIndex >= 0)
            {
                extra = title.Substring(brIndex + 2);
                title = title.Substring(0, brIndex);
            }
            content = extra.Replace("br", "<br/>") + content;

            var parsed = ParseTitle(title);
            if (string.IsNullOrEmpty(parsed.Core) || string.IsNullOrEmpty(parsed.Date)) continue;

            string datePart = parsed.Date;
            string titlePart = parsed.Core;

            if (parsed.Core.Length >= 8)
            {
                string first8 = parsed.Core.Substring(0, 8);
                bool isDigit = true;
                for (int i = 0; i < 8; i++)
                {
                    if (!char.IsDigit(first8[i]))
                    {
                        isDigit = false;
                        break;
                    }
                }

                if (isDigit)
                {
                    datePart = first8;

                    int splitIndex = parsed.Core.IndexOf("_", 8);
                    if (splitIndex > 8)
                    {
                        titlePart = parsed.Core.Substring(8, splitIndex - 8);
                    }
                    else
                    {
                        titlePart = parsed.Core.Substring(8);
                    }
                }
            }

            bool matched = false;
            foreach (KeyValuePair<string, DataRow> kvp in mssqlTopicMap)
            {
                string mssqlKey = kvp.Key;
                if (mssqlKey.Contains(titlePart) && mssqlKey.Contains(datePart))
                {
                    kvp.Value["HContent"] = kvp.Value["HContent"].ToString() + "\n---\n" + content;
                    matched = true;
                    break;
                }
            }

            if (!matched)
            {
                DataRow newRow = unifiedTable.NewRow();
                newRow["HID"] = lid;
                newRow["HTopicName"] = title;
                newRow["HContent"] = content;
                newRow["UserName"] = "EIP";
                newRow["HCreateDT"] = row["l_recorddate"];
                unifiedTable.Rows.Add(newRow);
            }
        }

        return unifiedTable;
    }



}