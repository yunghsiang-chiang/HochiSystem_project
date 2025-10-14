using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

/// <summary>
/// MailConfig 的摘要描述
/// </summary>
public class MailConfig
{
    public static string Sender
    {
        get { return ConfigurationManager.AppSettings["MailSender"]; }
    }

    public static string Account
    {
        get { return ConfigurationManager.AppSettings["MailAccount"]; }
    }

    public static string Password
    {
        get { return ConfigurationManager.AppSettings["MailPassword"]; }
    }

    public static string Host
    {
        get { return ConfigurationManager.AppSettings["MailHost"]; }
    }

    public static int Port
    {
        get { return int.Parse(ConfigurationManager.AppSettings["MailPort"]); }
    }

    public static bool EnableSSL
    {
        get { return bool.Parse(ConfigurationManager.AppSettings["MailEnableSSL"]); }
    }
}