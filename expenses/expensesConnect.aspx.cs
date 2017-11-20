using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using expenses.Old_App_Code;

using System.Text;
using Spend_Management;
using SpendManagementLibrary;

namespace expenses
{
    public partial class expensesConnect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "expensesConnect";
            Master.title = Title;

            CurrentUser user = cMisc.GetCurrentUser();
            ViewState["accountid"] = user.AccountID;

            cAccounts clsAccounts = new cAccounts();
            cAccount reqAccount = clsAccounts.GetAccountByID(user.AccountID);

            int noOfLicenses = 0;// reqAccount.expensesConnectLicenses;

            StringBuilder strBuild = new StringBuilder();

            //strBuild.Append("<table><tr><td>");

            if (noOfLicenses > 0)
            {
                strBuild.Append("<br /><font style=\"font-style: italic;font-weight:bold;\">expenses</font><font style=\"font-weight:bold;\">Connect</font> allows you to enter your expenses even when you are not connected to the Internet.<br /><br />");
                strBuild.Append("<font style=\"font-style: italic;font-weight:bold;\">expenses</font><font style=\"font-weight:bold;\">Connect</font> is a Windows application installed on your laptop. ");
                strBuild.Append("You enter your expense items and when a connection is made to the Internet, the system automatically synchronizes your offline added claims with your online expenses account.<br /><br />");
                strBuild.Append("If you would like to start using <font style=\"font-style: italic;font-weight:bold;\">expenses</font><font style=\"font-weight:bold;\">Connect</font>, please download below:<br /><br />");
                strBuild.Append("<a href=\"expensesConnect/expensesConnect.exe\">Download <font style=\"font-style: italic;font-weight:bold;\">expenses</font><font style=\"font-weight:bold;\">Connect</font></a>");
            }
            else
            {
                strBuild.Append("<br /><font style=\"font-style: italic;font-weight:bold;\">expenses</font><font style=\"font-weight:bold;\">Connect</font> delivers the capability to your employees to add expenses even when they’re not connected to the internet, ");
                strBuild.Append("helping to increase the likelihood of expenses being submitted on time. <br /><br />");
                strBuild.Append("For real-time convenience <font style=\"font-style: italic;font-weight:bold;\">expenses</font><font style=\"font-weight:bold;\">Connect</font>");
                strBuild.Append(" is the ideal solution for an employee who travels frequently or can’t always connect to the internet.<br /><br />");
                strBuild.Append("<font style=\"font-weight:bold;\">No Net?! No Worry!</font><br /><br />");
                strBuild.Append("<font style=\"font-style: italic;font-weight:bold;\">expenses</font><font style=\"font-weight:bold;\">Connect</font> is a Windows based application installed on your laptop.");
                strBuild.Append("You enter your expense items and when a connection is made to the Internet, the system automatically synchronizes your offline added claims with your online expenses account.<br /><br />");
                strBuild.Append("If you have an Internet connection then you can submit your claims on <font style=\"font-style: italic;font-weight:bold;\">expenses</font><font style=\"font-weight:bold;\">Connect</font>.");
                strBuild.Append("Your expenses are then approved and processed in the same way as expense claims created and submitted online.<br /><br />");
                strBuild.Append("If you would like to start using <font style=\"font-style: italic;font-weight:bold;\">expenses</font><font style=\"font-weight:bold;\">Connect</font>");
                strBuild.Append(", please contact us:<br /><br /><table>");
                strBuild.Append("<tr><td class=\"labeltd\">Telephone</td><td class=\"inputtd\">01522 881300</td></tr>");
                strBuild.Append("<tr><td class=\"labeltd\">Email</td><td class=\"inputtd\"><a href=\"mailto:sales@software-europe.co.uk\">Email Us</a></td></tr></table><br />");
                strBuild.Append("Use of <font style=\"font-style: italic;font-weight:bold;\">expenses</font><font style=\"font-weight:bold;\">Connect</font> is subject to our terms and conditions.");
            }

            litDetails.Text = strBuild.ToString();
            
        }
    }
}
