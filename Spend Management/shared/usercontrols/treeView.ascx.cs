using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Spend_Management
{
    public partial class treeView : System.Web.UI.UserControl
    {
        private string sOnClickFieldMethod = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        public string OnClickMethod
        {
            get { return sOnClickFieldMethod; }
            set { sOnClickFieldMethod = value; }
        }


    }
}