using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Spend_Management.shared.admin
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                testEditor.Content = "Stuff";
            }

        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            string thing = testEditor.Content;
        }
    }
}
