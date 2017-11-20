using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Spend_Management
{
    public static class cExtensionMethods
    {
        public static void SetButtonText(this Button btn, string text)
        {
            btn.Text = text;
        }
    }
}
