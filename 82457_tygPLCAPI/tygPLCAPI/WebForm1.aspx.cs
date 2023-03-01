using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;

namespace tygPLCAPI
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            JArray jsonarr = new JArray();
                    
            JObject jsonObject1 =
                new JObject(
             new JProperty("Date", DateTime.Now),
             new JProperty("Album", "Me Against The World"),
             new JProperty("Year", "James 2Pac-King's blog."),
             new JProperty("Artist", "2Pac")
         );
            JObject jsonObject2 =
                new JObject(
             new JProperty("Date", DateTime.Now),
             new JProperty("Album", "Me 2Against The World"),
             new JProperty("Year", "James2 2Pac-King's blog."),
             new JProperty("Artist", "2Pac2")
         );

            jsonarr.Add(jsonObject1);
            jsonarr.Add(jsonObject2);

            JObject jsonObject0 = new JObject(new JProperty("Data", jsonarr));
            string s = jsonObject0.ToString();
        }
    }
}