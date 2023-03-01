using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GroupMod : System.Web.UI.Page
{
    public int GroupAuth = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["user"] == null || Session["user"].ToString() == "")
        {
            Response.Write("<script>alert('請登入後方可正常操作本系統');location.href = 'Default.aspx';</script>");
        }
        else
        {
            string strCon = @"Data Source=172.17.3.106;Database=PMI;Uid=pmi;Pwd=pmidb!;";
            SqlConnection conn = new SqlConnection(strCon);
            conn.Open();
            string username = Session["username"].ToString();
            string sqlstr = string.Format("select GroupPermission.Username, Groups.GroupName, Groups.DetailAuth, Groups.InfoAuth, Groups.NewsAuth, Groups.HistoryAuth, Groups.StatusAuth, Groups.FactoryAuth, Groups.AreaAuth, Groups.PlatformAuth, Groups.GroupAuth from GroupPermission inner join Groups on GroupPermission.GroupNo = Groups.GroupNo where GroupPermission.Username = '{0}'", username);

            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                GroupAuth = Convert.ToInt32(dr["GroupAuth"].ToString());                
            }

            if (GroupAuth != 2)
            {
                Response.Write("<script>alert('抱歉，您並無此功能的操作權限，將返回上一頁。');history.back();</script>");
            }

            cmd.Cancel();
            dr.Close();
            conn.Close();
            conn.Dispose();
        }
    }
}