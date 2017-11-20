using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Spend_Management
{
    public partial class printKBA : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int kbaID;
                int.TryParse(Request.QueryString["kbaid"], out kbaID);

                if (kbaID > 0)
                {
                    svcHelp help = new svcHelp();

                    litContent.Text = "<html><body>" + help.GetKBAHTMLNoStyle(kbaID) + "</body></html>";
                }
                else
                {
                    litContent.Text = "The article id specified is not valid";
                }
            }
        }
    }
}
