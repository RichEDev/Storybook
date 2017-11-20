using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace SpendManagementLibrary
{
    static class cExtensionMethods
    {
        public static void ImageToolTipOnClick(this Image img, string productKey, string tooltipId)
        {
            img.Attributes.Add("onclick", "SEL.Tooltip.Show('" + tooltipId + "', '" + productKey + "', 'this" + ")");
        }
    }
}
