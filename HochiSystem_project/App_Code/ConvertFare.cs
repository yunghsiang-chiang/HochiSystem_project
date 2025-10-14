using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 數字轉換(千分位)
/// </summary>
public class ConvertFare
{
    public string InputFare { get; set; }
    public int OutFare { get; set; }

    //public string ToThousands(string InputFare, string text)
    //{
    //    decimal OutFare = 0M;  //預設值
    //    bool Result = decimal.TryParse(InputFare.Trim(), out OutFare);
    //    return OutFare.ToString("N"); 
    //}

    public static string ToThousands(int InputFare)
    {
        int OutFare = 0;  //預設值
        return InputFare.ToString("N0");
    }
}