using System;
using System.Web.UI;
using System.Web.Security;

namespace Spend_Management
{
    using SpendManagementLibrary.Employees;

    /// <summary>
    /// Authenticates an employee password key 
    /// </summary>
    public partial class AuthenticatePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Reset Password";
            Master.Title  = Title;

            if (IsPostBack == true)
            {
                if (string.IsNullOrEmpty(txtKey.Text) == false)
                {
                    GetDetails(txtKey.Text);
                }
                else
                {
                    rfKey.IsValid = false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Request.QueryString["key"]))
                {
                    rfKey.IsValid = false;
                }
                else
                {
                    GetDetails(Request.QueryString["key"].ToString());
                }
            }
        }

        public void GetDetails(string uniqueKey)
        {
            Tuple<int, Employee> returnTuple = cMisc.MatchPasswordKey(uniqueKey);

            if (returnTuple == null || returnTuple.Item2 == null)
            {
                rfKey.IsValid = false;
            }
            else
            {
                FormsAuthentication.RedirectFromLoginPage(returnTuple.Item1.ToString() + "," + returnTuple.Item2.EmployeeID.ToString(), false);
                returnTuple.Item2.MarkFirstLogonComplete(null);

                this.Session["ChangePasswordUserId"] = returnTuple.Item2.EmployeeID;
                var url = string.Format("~/shared/changepassword.aspx?returnto=3&employeeid={0}&resetKey={1}", returnTuple.Item2.EmployeeID, uniqueKey);
                Response.Redirect(url, true);
            }
        }

        protected void btnOk_Click(object sender, ImageClickEventArgs e)
        {

        }
    }
}
