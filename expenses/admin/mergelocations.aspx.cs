using System;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Web.SessionState;
using expenses.Old_App_Code;
using ExpensesLibrary;
using SpendManagementLibrary;
using Spend_Management;



namespace expenses.admin
{
    public partial class mergelocations : System.Web.UI.Page
    {

        cCompanies clscompanies;

        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Merge Locations";
            Master.title = Title;
            Master.enablenavigation = false;

            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.getCurrentUser(User.Identity.Name);
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Locations, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cEmployees clsemployees = new cEmployees(user.AccountID);
                cEmployee reqemp;
                reqemp = clsemployees.GetEmployeeById(user.EmployeeID);
                cMisc clsmisc = new cMisc(reqemp.accountid);
                
                cGlobalProperties clsproperties = clsmisc.getGlobalProperties(reqemp.accountid);
                clscompanies = new cCompanies((int)ViewState["accountid"]);

                cmbFromCompany.Items.AddRange(clscompanies.CreateMergeDropDown("Select a location to merge").ToArray());
                cmbFromCompany.SelectedIndex = 0;
                cmbToCompany.Items.AddRange(clscompanies.CreateMergeDropDown("Select a location to merge into").ToArray());
            }
            
            



        }

        protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            lblError.Text = "";
            bool errMergeFromItems = false;
            bool errMergeToItem = false;
            bool errMergeIntoSelf = false;
            int mergeToCompany = int.Parse(cmbToCompany.SelectedValue);

            if (cmbFromCompany.Items.Count < 1)
            {
                lblError.Text += "<br />You must select atleast one location to to merge.";
                errMergeFromItems = true;
            }

            if (mergeToCompany < 1)
            {
                lblError.Text += "<br />You must select a location to merge into.";
                errMergeToItem = true;
            }

            if (errMergeFromItems == false && errMergeToItem == false)
            {
                clscompanies = new cCompanies((int)ViewState["accountid"]);

                foreach(ListItem item in cmbFromCompany.Items) 
                {
                    if (item.Selected == true)
                    {
                        //if (cmbToCompany == int.Parse(item.Value))
                        if(mergeToCompany == int.Parse(item.Value))
                        {
                            errMergeIntoSelf = true;
                            lblError.Text += "<br />You cannot merge a location into itself.";
                            break;
                        }
                    }
                }

                if (errMergeIntoSelf == false)
                {
                    clscompanies.mergeCompany(mergeToCompany, cmbFromCompany.Items);
                    Response.Redirect("~/shared/admin/locationsearch.aspx", true);
                }
            }
        }

        protected void cmdcancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/shared/admin/locationsearch.aspx", true);
        }


    }


}

