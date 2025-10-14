using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// NPOIHelperCSV 的摘要描述
/// /// 讀取CSV並轉成DataTable
/// </summary>
public class NPOIHelperCSV
{
    public static DataTable ReadCsvAsTableNPOI(string fullFileName, Int16 firstRow = 0, Int16 firstColumn = 0, Int16 getRows = 0, Int16 getColumns = 0, bool haveTitleRow = true)
    {
        DataTable dt = new DataTable();
        FileStream fs = new FileStream(fullFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
        //記錄每次讀取的一行記錄
        string strLine = "";
        //記錄每行記錄中的各字段內容
        string[] aryLine;
        //標示列數
        int columnCount = 0;
        //是否已建立了表的字段
        bool bCreateTableColumns = false;
        //第幾行
        int iRow = 1;

        //去除無用行
        if (firstRow > 0)
        {
            for (int i = 1; i < firstRow; i++)
            {
                sr.ReadLine();
            }
        }

        // { ",", ".", "!", "?", ";", ":", " " };
        string[] separators = { "," };
        //逐行讀取CSV中的數據
        while ((strLine = sr.ReadLine()) != null)
        {
            strLine = strLine.Trim();
            aryLine = strLine.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

            if (bCreateTableColumns == false)
            {
                bCreateTableColumns = true;
                columnCount = aryLine.Length;
                //創建列
                for (int i = firstColumn; i < (getColumns == 0 ? columnCount : firstColumn + getColumns); i++)
                {
                    DataColumn dc
                        = new DataColumn(haveTitleRow == true ? aryLine[i] : "COL" + i.ToString());
                    dt.Columns.Add(dc);
                }

                bCreateTableColumns = true;

                if (haveTitleRow == true)
                {
                    continue;
                }
            }


            DataRow dr = dt.NewRow();
            //for (int j = firstColumn; j < (getColumns == 0 ? columnCount : firstColumn + getColumns); j++)
            //{
            //    dr[j - firstColumn] = aryLine[j];
            //}
            for (int j = firstColumn; j < aryLine.Length; j++)
            {
                dr[j - firstColumn] = aryLine[j];
            }
            dt.Rows.Add(dr);

            iRow = iRow + 1;
            if (getRows > 0)
            {
                if (iRow > getRows)
                {
                    break;
                }
            }

        }

        sr.Close();
        fs.Close();
        return dt;
    }
}



