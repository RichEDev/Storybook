using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Spend_Management
{
    public partial class MileageGrid : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("MileageGrid.Page_Load, IsPostback = " + IsPostBack);
        }
    }
}