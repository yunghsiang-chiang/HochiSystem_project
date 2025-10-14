using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using NPOI.HSSF.Util;
using System.Data;
using System.IO;

/// <summary>
/// NPOIHelper 的摘要描述
/// 讀取Excel並轉成DataTable
/// </summary>
public class NPOIHelperXlsx
{
	public static DataTable ReadExcelAsTableNPOI(string fileName)
	{
		using (FileStream fs = new FileStream(fileName, FileMode.Open))
		{
			XSSFWorkbook wb = new XSSFWorkbook(fs);
			var sheet = wb.GetSheetAt(0);
			DataTable table = new DataTable();
			//由第一列取標題做為欄位名稱
			var headerRow = sheet.GetRow(0);
			int cellCount = headerRow.LastCellNum;
			for (int i = headerRow.FirstCellNum; i < cellCount; i++)
				//以欄位文字為名新增欄位，此處全視為字串型別以求簡化
				table.Columns.Add(
					new DataColumn(headerRow.GetCell(i).StringCellValue));

			//略過第零列(標題列)，一直處理至最後一列
			for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
			{
				var row = sheet.GetRow(i);
				if (row == null) continue;
				DataRow dataRow = table.NewRow();
				//依先前取得的欄位數逐一設定欄位內容
				for (int j = row.FirstCellNum; j < cellCount; j++)
					if (row.GetCell(j) != null)
						//如要針對不同型別做個別處理，可善用.CellType判斷型別
						//再用.StringCellValue, .DateCellValue, .NumericCellValue...取值
						//此處只簡單轉成字串
						dataRow[j] = row.GetCell(j).ToString();
				table.Rows.Add(dataRow);
			}
			return table;
		}
	}
}