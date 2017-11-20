using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;

namespace Spend_Management
{
    public partial class policyCustomHelp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Custom Help &amp; Support";
            Master.PageSubTitle = Title;

        }
    }
}
