using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Spend_Management;

using System.Text;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Allows fields and values to be selected
    /// </summary>
    public partial class fields_selector : System.Web.UI.UserControl
    {
        private CriteriaMode eAction = CriteriaMode.Select;
        /// <summary>
        /// The mode to work in, Select or Update - default is select
        /// </summary>
        public string sAction;

        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (eAction == CriteriaMode.Update)
            {
                sAction = "Update";
            }
            else
            {
                sAction = "Select";
            }
        }

        /// <summary>
        /// Set if you want to use the user control in a default status
        /// </summary>
        public CriteriaMode criteriaMode
        {
            set { eAction = value; }
        }


    }
}