using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices;
using System.Security.Principal;

public partial class Login : System.Web.UI.Page
{
    public static string computername = "tyg.com.tw";

    protected void Page_Load(object sender, EventArgs e)
    {
        string username = Request.Form["username"].ToString();
        string password = Request.Form["password"].ToString();

        string strPath = string.Format(@"LDAP://{0}", computername);
        DirectoryEntry entry = new DirectoryEntry(strPath, username, password);        

        try
        {
            string objectSid = (new SecurityIdentifier((byte[])entry.Properties["objectSid"].Value, 0).Value);

            //Session["user"] = entry.Properties["name"].Value;
            Session["user"] = username;
            Session["username"] = username;

            Response.Write("<script>location.href = 'Default.aspx';</script>");
        }
        catch (Exception)// (DirectoryServicesCOMException)
        {
            Response.Write("<script>alert('驗證失敗');location.href = 'Enter.aspx';</script>");
        }
        finally
        {
            entry.Dispose();
        }
    }
}