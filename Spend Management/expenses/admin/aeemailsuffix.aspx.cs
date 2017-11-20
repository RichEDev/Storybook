using System;
using System.Web.UI;
using Spend_Management;
using SpendManagementLibrary;

public partial class admin_aeemailsuffix : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {
            Title = "Add/Edit E-mail Suffix";
            Master.title = Title;
        	Master.enablenavigation = false;			
            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.EmailSuffixes, true, true);
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;
           
            Spend_Management.Action action = Spend_Management.Action.Add;

            if (Request.QueryString["action"] != null)
            {
                action = (Spend_Management.Action)byte.Parse(Request.QueryString["action"]);
            }

            ViewState["action"] = action;
            if (action == Spend_Management.Action.Edit)
            {
                int suffixid = int.Parse(Request.QueryString["suffixid"]);
                cEmailSuffixes clssuffixes = new cEmailSuffixes(user.AccountID);
                cEmailSuffix clssuffix = clssuffixes.getSuffixById(suffixid);
                txtsuffix.Text = clssuffix.suffix;

                ViewState["suffixid"] = suffixid;
            }
        }
    }
    protected void cmdcancel_Click(object sender, ImageClickEventArgs e)
    {
        Response.Redirect("emailsuffixes.aspx", true);
    }
    protected void cmdok_Click(object sender, ImageClickEventArgs e)
    {
        string suffix = txtsuffix.Text;

        cEmailSuffixes clssuffixes = new cEmailSuffixes((int)ViewState["accountid"]);

        Spend_Management.Action action = (Spend_Management.Action)ViewState["action"];

        if (action == Spend_Management.Action.Edit)
        {
            if (clssuffixes.updateSuffix((int)ViewState["suffixid"], suffix) == ReturnCode.AlreadyExists)
            {
                lblmsg.Text = "The e-mail suffix cannot be updated as the e-mail suffix already exists.";
                lblmsg.Visible = true;
                return;
            }
        }
        else
        {
            if (clssuffixes.addSuffix(suffix) == ReturnCode.AlreadyExists)
            {
                lblmsg.Text = "The e-mail suffix cannot be added as the e-mail suffix already exists.";
                lblmsg.Visible = true;
                return;
            }
        }

        Response.Redirect("emailsuffixes.aspx", true);
    }
}
