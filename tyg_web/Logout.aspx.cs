using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["user"] = "";
        Session["username"] = "";

        Response.Write("<script>alert('謝謝您的使用。');location.href = 'Default.aspx';</script>");
    }
}