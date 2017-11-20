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

namespace Spend_Management
{
    public partial class runReport : System.Web.UI.UserControl
    {
        private int nReportID;
        private CurrentUser clsCurrentUser;
        private int nReportRequestNumber;

        protected void Page_Load(object sender, EventArgs e)
        {
            

        }

        public CurrentUser currentUser
        {
            set { clsCurrentUser = value; }
        }

        public int ReportRequestNumber
        {
            get { return nReportRequestNumber; }
            set { nReportRequestNumber = value; }
        }

        public int ReportID
        {
            get { return nReportID; }
            set { nReportID = value; }
        }

        public int EmployeeID
        {
            get { return clsCurrentUser.EmployeeID; }
        }

        public int AccountID
        {
            get { return clsCurrentUser.AccountID; }
        }
    }
}