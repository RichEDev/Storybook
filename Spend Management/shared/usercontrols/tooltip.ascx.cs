using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Spend_Management
{
    public partial class tooltip : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("SEL.Tooltip.tooltipPopupContent = '" + pnlTooltipContent.ClientID + "';");
            sb.Append("SEL.Tooltip.tooltipPopupExtenderID = '" + pceTooltip.ClientID + "';");
            Page.ClientScript.RegisterStartupScript("".GetType(), "tooltipLoader", sb.ToString(), true);
        }
    }
}