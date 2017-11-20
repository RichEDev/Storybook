using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Spend_Management.shared
{
    public partial class attachfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Attach File";
            Master.PageSubTitle = Title;

        }
    }
}
