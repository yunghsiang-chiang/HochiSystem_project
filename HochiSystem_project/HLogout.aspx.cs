using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class HLogout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string strUserId = Session["HAccount"] as string;
        ArrayList list = Application.Get("GLOBAL_USER_LIST") as ArrayList;
        if (strUserId != null && list != null)
        {
            list.Remove(strUserId);
            Application.Add("GLOBAL_USER_LIST", list);
        }
        //清掉Session
        Session.Clear();
        Session.Abandon();

        //清掉cookie
        Response.Cookies["HochiInfo"]["HAccount"] = "";
        //Response.Cookies["HochiInfo"]["HPassword"] = "";
        Response.Cookies["HochiInfo"]["HUserName"] = "";
        Response.Cookies["HochiInfo"]["HUserHID"] = "";
        Response.Cookies["HochiInfo"]["EIPUID"] = ""; //EIP的p_uid
        Response.Cookies["HochiInfo"].Expires = DateTime.Now.AddDays(-1);//直接過期

        Response.Cookies["HochiInfoHAccount"].Value = "";
        //Response.Cookies["HochiInfoHPassword"].Value = "";
        Response.Cookies["HochiInfoHUserName"].Value = "";
        Response.Cookies["HochiInfoHUserHID"].Value = "";
        Response.Cookies["HochiInfoEIPUID"].Value = ""; //EIP的p_uid

        Response.Cookies["HochiInfoHAccount"].Expires = DateTime.Now.AddDays(-1);//直接過期
        //Response.Cookies["HochiInfoHPassword"].Expires = DateTime.Now.AddDays(-1);//直接過期
        Response.Cookies["HochiInfoHUserName"].Expires = DateTime.Now.AddDays(-1);//直接過期
        Response.Cookies["HochiInfoHUserHID"].Expires = DateTime.Now.AddDays(-1);//直接過期
        Response.Cookies["HochiInfoEIPUID"].Expires = DateTime.Now.AddDays(-1);//直接過期

        Response.Write("<script>alert('您已成功登出!');window.location.href='HIndex.aspx';</script>");
        //Response.Redirect("~/System/HLogin.aspx");

    }
}