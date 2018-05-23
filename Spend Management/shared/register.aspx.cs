#region Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.GeneralOptions;
using BusinessLogic.Identity;
using BusinessLogic.Modules;
using BusinessLogic.ProductModules;

using Common.Cryptography;

using expenses;

using Spend_Management;
using Spend_Management.shared.code;
using Spend_Management.shared.code.Validation.BankAccount;
using Spend_Management.shared.code.Validation.BankAccount.PostCodeAnywhere;

using SpendManagementLibrary;
using SpendManagementLibrary.Account;
using SpendManagementLibrary.Addresses;
using SpendManagementLibrary.BaseClasses;
using SpendManagementLibrary.Employees;
using SpendManagementLibrary.Enumerators;
using BankAccount = SpendManagementLibrary.Employees.BankAccount;

#endregion

/// <summary>
/// The register.
/// </summary>
public partial class register : Page
{
    /// <summary>
    /// The udfs added.
    /// </summary>
    public bool UdfsAdded = false;

    #region Public Methods and Operators

    /// <summary>
    /// The create advances signoff summary.
    /// </summary>
    /// <param name="accountid">
    /// The accountid.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string CreateAdvancesSignoffSummary(int accountid)
    {
        var output = new StringBuilder();
        output.Append("<div class=\"inputpanel\">");
        output.Append("<div class=\"inputpaneltitle\">Signoff Group For Advances</div>");
        output.Append("<table>");

        output.Append("<tr><td class=\"labeltd\">Advances Signoff Group</td><td class=\"inputtd\">");
        if (this.ViewState["advancesgroupid"] != null)
        {
            var clsgroups = new cGroups(accountid);
            cGroup group = clsgroups.GetGroupById((int)this.ViewState["advancesgroupid"]);
            if (group != null)
            {
                output.Append(group.groupname);
            }
        }

        output.Append("</td></tr>");
        output.Append("</table>");
        output.Append("</div>");
        return output.ToString();
    }

    /// <summary>
    /// The create car summary.
    /// </summary>
    /// <param name="accountid">
    /// The accountid.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string CreateCarSummary(int accountid, bool showMileageCategoriesForUsers)
    {
        var output = new StringBuilder();
        output.Append("<div class=\"inputpanel\">");
        output.Append("<div class=\"inputpaneltitle\">Car Details</div>");
        output.Append("<table>");
        output.Append(
            "<tr><td class=\"labeltd\">Do you use a car in the course of your work</td><td class=\"inputtd\">");
        output.Append(this.chkusecar.Checked ? "Yes" : "No");

        output.Append("</td></tr>");

        if (this.chkusecar.Checked)
        {
            output.Append(
                "<tr><td class=\"labeltd\">Make</td><td class=\"inputtd\">" + this.txtmake.Text + "</td></tr>");
            output.Append(
                "<tr><td class=\"labeltd\">Model</td><td class=\"inputtd\">" + this.txtmodel.Text + "</td></tr>");
            output.Append(
                "<tr><td class=\"labeltd\">Registration Number</td><td class=\"inputtd\">" + this.txtregno.Text
                + "</td></tr>");
            output.Append(
                "<tr><td class=\"labeltd\">Mileage Unit of Measure</td><td class=\"inputtd\">" + this.cmbUom.Text
                + "</td></tr>");
            output.Append("<tr><td class=\"labeltd\">Engine Type</td><td class=\"inputtd\">");
            output.Append(this.cmbcartype.SelectedItem.Text);
            output.Append("</td></tr>");

            if (showMileageCategoriesForUsers)
            {
                output.Append("<tr><td class=\"labeltd\">Vehicle Journey Rate Category</td><td class=\"inputtd\">");
                if (this.ViewState["mileageid"] != null)
                {
                    var clsmileage = new cMileagecats(accountid);
                    cMileageCat mileage = clsmileage.GetMileageCatById((int)this.ViewState["mileageid"]);
                    if (mileage != null)
                    {
                        output.Append(mileage.carsize);
                    }
                }
                output.Append("</td></tr>");
            }


        }
        else
        {
            output.Append("<tr><td class=\"labeltd\">Make</td><td class=\"inputtd\">-</td></tr>");
            output.Append("<tr><td class=\"labeltd\">Model</td><td class=\"inputtd\">-</td></tr>");
            output.Append("<tr><td class=\"labeltd\">Registration Number</td><td class=\"inputtd\">-</td></tr>");
            output.Append("<tr><td class=\"labeltd\">Mileage Unit of Measure</td><td class=\"inputtd\">-</td></tr>");
            output.Append("<tr><td class=\"labeltd\">Engine Type</td><td class=\"inputtd\">");
            output.Append("-");
            output.Append("</td></tr>");
            if (showMileageCategoriesForUsers)
            {
                output.Append("<tr><td class=\"labeltd\">Vehicle Journey Rate Category</td><td class=\"inputtd\">");
                output.Append("-</td></tr>");
            }
        }

        output.Append("</table>");
        output.Append("</div>");
        return output.ToString();
    }

    /// <summary>
    /// The create employee contact summary.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string CreateEmployeeContactSummary()
    {
        var output = new StringBuilder();
        output.Append("<div class=\"inputpanel\">");
        output.Append("<div class=\"inputpaneltitle\">Employee Contact Information</div>");
        output.Append("<table>");

        output.Append(
            "<tr><td class=\"labeltd\">Mobile Number</td><td class=\"inputtd\">" + this.txtmobile.Text + "</td></tr>");
        output.Append("<tr><td class=\"labeltd\">Pager</td><td class=\"inputtd\">" + this.txtpager.Text + "</td></tr>");
        output.Append(
            "<tr><td class=\"labeltd\">Extension Number</td><td class=\"inputtd\">" + this.txtextension.Text
            + "</td></tr>");
        output.Append("</table>");
        output.Append("</div>");
        return output.ToString();
    }

    /// <summary>
    /// The create employement info summary.
    /// </summary>
    /// <param name="accountid">
    /// The accountid.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string CreateEmployementInfoSummary(int accountid)
    {
        // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
        var subaccs = new cAccountSubAccounts(accountid);
        int subAccountId = subaccs.getFirstSubAccount().SubAccountID;

        var output = new StringBuilder();
        output.Append("<div class=\"inputpanel\">");
        output.Append("<div class=\"inputpaneltitle\">Employment Information</div>");
        output.Append("<table>");

        output.Append(
            "<tr><td class=\"labeltd\">Purchase Ledger Number</td><td class=\"inputtd\">" + this.txtpurchaseledger.Text
            + "</td></tr>");
        output.Append(
            "<tr><td class=\"labeltd\">Position:</td><td class=\"inputtd\">" + this.txtposition.Text + "</td></tr>");
        output.Append(
            "<tr><td class=\"labeltd\">Payroll Number</td><td class=\"inputtd\">" + this.txtpayrollnumber.Text
            + "</td></tr>");

        output.Append("<tr><td class=\"labeltd\">Line Manager</td><td class=\"inputtd\">");
        if (this.cmblinemanager.SelectedItem != null)
        {
            var clsemployees = new cEmployees(accountid);
            Employee reqemp = clsemployees.GetEmployeeById(int.Parse(this.cmblinemanager.SelectedValue));
            if (reqemp != null)
            {
                output.Append(reqemp.Title + " " + reqemp.Forename + " " + reqemp.Surname);
            }
        }

        output.Append("</td></tr>");
        output.Append("<tr><td class=\"labeltd\">Primary Country</td><td class=\"inputtd\">");
        if (this.cmbcountry.SelectedItem != null)
        {
            var clscountries = new cCountries(accountid, subAccountId);
            var clsglobalcountries = new cGlobalCountries();
            cCountry country = clscountries.getCountryById(int.Parse(this.cmbcountry.SelectedValue));
            if (country != null)
            {
                output.Append(clsglobalcountries.getGlobalCountryById(country.GlobalCountryId).Country);
            }
        }

        output.Append("</td></tr>");
        output.Append("<tr><td class=\"labeltd\">Primary Currency</td><td class=\"inputtd\">");
        if (this.cmbcurrency.SelectedItem != null)
        {
            var clsglobalcurrencies = new cGlobalCurrencies();
            var clscurrencies = new cCurrencies(accountid, subAccountId);
            cCurrency currency = clscurrencies.getCurrencyById(int.Parse(this.cmbcurrency.SelectedValue));
            if (currency != null)
            {
                output.Append(clsglobalcurrencies.getGlobalCurrencyById(currency.globalcurrencyid).label);
            }
        }

        output.Append("</td></tr>");
        output.Append("<tr><td class=\"labeltd\">Department</td><td class=\"inputtd\">");
        if (this.cmbdepartment.SelectedItem != null)
        {
            if (this.cmbdepartment.SelectedValue != string.Empty)
            {
                var clsdepartments = new cDepartments(accountid);
                cDepartment department = clsdepartments.GetDepartmentById(int.Parse(this.cmbdepartment.SelectedValue));
                if (department != null)
                {
                    output.Append(department.Department);
                }
            }
        }

        output.Append("</td></tr>");
        output.Append("<tr><td class=\"labeltd\">Cost Code</td><td class=\"inputtd\">");
        if (this.cmbcostcode.SelectedItem != null)
        {
            if (this.cmbcostcode.SelectedValue != string.Empty)
            {
                var clscostcodes = new cCostcodes(accountid);
                cCostCode costcode = clscostcodes.GetCostcodeById(int.Parse(this.cmbcostcode.SelectedValue));
                if (costcode != null)
                {
                    output.Append(costcode.Costcode);
                }
            }
        }

        output.Append("</td></tr>");
        output.Append("<tr><td class=\"labeltd\">Project Code</td><td class=\"inputtd\">");
        if (this.cmbprojectcode.SelectedItem != null)
        {
            if (this.cmbprojectcode.SelectedValue != string.Empty)
            {
                var clsprojectcodes = new cProjectCodes(accountid);
                cProjectCode projectcode =
                    clsprojectcodes.getProjectCodeById(int.Parse(this.cmbprojectcode.SelectedValue));
                if (projectcode != null)
                {
                    output.Append(projectcode.projectcode);
                }
            }
        }

        output.Append("</td></tr>");
        output.Append("</table>");
        output.Append("</div>");
        return output.ToString();
    }

    /// <summary>
    /// The create first timeline.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string CreateFirstTimeline()
    {
        var output = new StringBuilder();

        var steps = new ArrayList();
        int stepcount = 3;
        steps.Add("Employee Name &amp; Logon Details");
        steps.Add("Enter a Password");

        steps.Add("Summary");

        stepcount++;

        var distance = (float)Math.Round(100 / (float)stepcount, 1, MidpointRounding.AwayFromZero);
        for (int i = 0; i < steps.Count; i++)
        {
            float left = distance * (i + 1);
            output.Append("<div class=\"timelineevent\" style=\"left: " + left + "%;\">");
            if (this.wizregister.ActiveStepIndex == i)
            {
                output.Append(
                    "<img src=\"/shared/images/buttons/timeline_event" + (i + 1) + "_sel.gif\" class=\"timelineimg\">");
            }
            else
            {
                output.Append(
                    "<img src=\"/shared/images/buttons/timeline_event" + (i + 1) + ".gif\" class=\"timelineimg\">");
            }

            output.Append("<br>");
            output.Append("<span class=\"timeeventlabel\">" + (string)steps[i] + "</span>");
            output.Append("</div>\n");
        }

        return output.ToString();
    }

    /// <summary>
    /// The create home address summary.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string CreateHomeAddressSummary()
    {
        var output = new StringBuilder();
        output.Append("<div class=\"inputpanel\">");
        output.Append("<div class=\"inputpaneltitle\">Contact Information</div>");
        output.Append("<table>");

        output.Append(
            "<tr><td class=\"labeltd\">Address Line 1</td><td class=\"inputtd\">" + this.txtaddressline1.Text
            + "</td></tr>");
        output.Append(
            "<tr><td class=\"labeltd\">Address Line 2</td><td class=\"inputtd\">" + this.txtaddressline2.Text
            + "</td></tr>");
        output.Append(
            "<tr><td class=\"labeltd\">City/Town</td><td class=\"inputtd\">" + this.txtcity.Text
            + "</td></tr>");
        output.Append(
            "<tr><td class=\"labeltd\">CountyState</td><td class=\"inputtd\">" + this.txtcounty.Text
            + "</td></tr>");
        output.Append(
            "<tr><td class=\"labeltd\">Country</td><td class=\"inputtd\">" + this.txtcountry.Text
            + "</td></tr>");
        output.Append(
            "<tr><td class=\"labeltd\">Postcode/Zip</td><td class=\"inputtd\">" + this.txtpostcode.Text
            + "</td></tr>");
        output.Append(
            "<tr><td class=\"labeltd\">Lived at Address Since</td><td class=\"inputtd\">" + this.txtdateataddress.Text
            + "</td></tr>");


        output.Append(
            "<tr><td class=\"labeltd\">Home Telephone</td><td class=\"inputtd\">" + this.txthomephone.Text
            + "</td></tr>");
        output.Append(
            "<tr><td class=\"labeltd\">Home Fax</td><td class=\"inputtd\">" + this.txthomefax.Text + "</td></tr>");
        output.Append(
            "<tr><td class=\"labeltd\">Home E-mail Address</td><td class=\"inputtd\">" + this.txthomeemail.Text
            + "</td></tr>");
        output.Append("</table>");
        output.Append("</div>");
        return output.ToString();
    }

    /// <summary>
    /// The create mileage.
    /// </summary>
    /// <param name="accountid">
    /// The accountid.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string CreateMileage(int accountid)
    {
        var clsmileage = new cMileagecats(accountid);

        var generalOptions = FunkyInjector.Container.GetInstance<IDataFactory<IGeneralOptions, int>>()[cMisc.GetCurrentUser().CurrentSubAccountId].WithMileage();

        string rowclass = "row1";
        var output = new StringBuilder();
        output.Append("<div class=\"inputpanel\">");
        output.Append("<div class=\"inputpaneltitle\">Vehicle Journey Rate Category</div>");
        output.Append("<table class=\"datatbl\">");
        output.Append(
            "<tr><th></th><th>Vehicle Journey Rate Category</th><th>Description</th><th>Pence/mile<br>before "
            + generalOptions.Mileage.Mileage + "<br>miles</th><th>Pence/mile<br>after " + generalOptions.Mileage.Mileage
            + "<br>miles</th></tr>");

        int mileageid = 0;
        if (this.ViewState["mileageid"] != null)
        {
            mileageid = (int)this.ViewState["mileageid"];
        }

        var mileageItems = clsmileage.CreateDropDown();

        for (int i = 0; i < mileageItems.Count; i++)
        {
            var reqmileage = clsmileage.GetMileageCatById(int.Parse(mileageItems[i].Value));
            output.Append(
                "<tr><td class=\"" + rowclass + "\"><input type=radio name=mileageid id=mileageid value=\""
                + reqmileage.mileageid + "\"");
            if (reqmileage.mileageid == mileageid)
            {
                output.Append(" checked");
            }

            output.Append("></td>");
            output.Append("<td class=\"" + rowclass + "\">" + reqmileage.carsize + "</td>");
            output.Append("<td class=\"" + rowclass + "\">" + reqmileage.comment + "</td>");

            output.Append("</tr>");
            rowclass = rowclass == "row1" ? "row2" : "row1";
        }

        output.Append("</table>");
        output.Append("</div>");
        return output.ToString();
    }

    /// <summary>
    /// The create role summary.
    /// </summary>
    /// <param name="accountid">
    /// The accountid.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string CreateRoleSummary(int accountid)
    {
        var output = new StringBuilder();
        output.Append("<div class=\"inputpanel\">");
        output.Append("<div class=\"inputpaneltitle\">Role</div>");
        output.Append("<table>");

        output.Append("<tr><td class=\"labeltd\">Role</td><td class=\"inputtd\">");
        if (this.ViewState["roleid"] != null)
        {
            var clsAccessRoles = new cAccessRoles(accountid, cAccounts.getConnectionString(accountid));
            cAccessRole role = clsAccessRoles.GetAccessRoleByID((int)this.ViewState["roleid"]);
            if (role != null)
            {
                output.Append(role.RoleName);
            }
        }

        output.Append("</td></tr>");
        output.Append("</table>");
        output.Append("</div>");
        return output.ToString();
    }

    /// <summary>
    /// The create roles.
    /// </summary>
    /// <param name="accountid">
    /// The accountid.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string CreateRoles(int accountid)
    {
        var clsAccessRoles = new cAccessRoles(accountid, cAccounts.getConnectionString(accountid));

        int roleid = 0;

        if (this.ViewState["roleid"] != null)
        {
            roleid = (int)this.ViewState["roleid"];
        }

        string rowclass = "row1";
        var output = new StringBuilder();
        output.Append("<div class=\"inputpanel\">");
        output.Append("<div class=\"inputpaneltitle\">Role</div>");
        output.Append("<table class=\"datatbl\">");
        output.Append("<tr><th></th><th>Role Name</th><th>Description</th></tr>");

        foreach (cAccessRole accessRole in clsAccessRoles.AccessRoles.Values)
        {
            output.Append("<tr>");
            output.Append(
                "<td class=\"" + rowclass + "\"><input type=\"radio\" value=\"" + accessRole.RoleID
                + "\" name=\"role\" id=\"role\"");
            if (accessRole.RoleID == roleid)
            {
                output.Append(" checked");
            }

            output.Append("></td>");
            output.Append("<td class=\"" + rowclass + "\">" + accessRole.RoleName + "</td>");
            output.Append("<td class=\"" + rowclass + "\">" + accessRole.Description + "</td>");
            output.Append("</tr>");
            rowclass = rowclass == "row1" ? "row2" : "row1";
        }

        output.Append("</table>");
        output.Append("</div>");
        return output.ToString();
    }

    /// <summary>
    /// The create signoff groups.
    /// </summary>
    /// <param name="accountid">
    /// The accountid.
    /// </param>
    /// <param name="signofftype">
    /// The signofftype.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string CreateSignoffGroups(int accountid, string signofftype)
    {
        var clsgroups = new cGroups(accountid);

        int groupid = 0;

        if (signofftype == "group")
        {
            if (this.ViewState["groupid"] != null)
            {
                groupid = (int)this.ViewState["groupid"];
            }
        }
        else
        {
            if (this.ViewState["advancesgroupid"] != null)
            {
                groupid = (int)this.ViewState["advancesgroupid"];
            }
        }

        string rowclass = "row1";
        var output = new StringBuilder();
        output.Append("<div class=\"inputpanel\">");
        output.Append(
            signofftype == "group"
                ? "<div class=\"inputpaneltitle\">Signoff Group</div>"
                : "<div class=\"inputpaneltitle\">Signoff Group For Advances</div>");

        output.Append("<table class=\"datatbl\">");
        output.Append("<tr><th></th><th>Group Name</th><th>Description</th></tr>");
        for (int i = 0; i < clsgroups.groupList.Count; i++)
        {
            var group = (cGroup)clsgroups.groupList.GetByIndex(i);
            output.Append("<tr>");
            output.Append(
                "<td class=\"" + rowclass + "\"><input type=\"radio\" value=\"" + group.groupid + "\" name=\""
                + signofftype + "\" id=\"" + signofftype + "\"");
            if (groupid == group.groupid)
            {
                output.Append(" checked");
            }

            output.Append("></td>");
            output.Append("<td class=\"" + rowclass + "\">" + group.groupname + "</td>");
            output.Append("<td class=\"" + rowclass + "\">" + group.description + "</td>");
            output.Append("</tr>");
            rowclass = rowclass == "row1" ? "row2" : "row1";
        }

        output.Append("</table>");
        output.Append("</div>");
        return output.ToString();
    }

    /// <summary>
    /// The create signoff summary.
    /// </summary>
    /// <param name="accountid">
    /// The accountid.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string CreateSignoffSummary(int accountid)
    {
        var output = new StringBuilder();
        output.Append("<div class=\"inputpanel\">");
        output.Append("<div class=\"inputpaneltitle\">Signoff Group</div>");
        output.Append("<table>");

        output.Append("<tr><td class=\"labeltd\">Signoff Group</td><td class=\"inputtd\">");
        if (this.ViewState["groupid"] != null)
        {
            var clsgroups = new cGroups(accountid);
            cGroup group = clsgroups.GetGroupById((int)this.ViewState["groupid"]);
            if (group != null)
            {
                output.Append(group.groupname);
            }
        }

        output.Append("</td></tr>");
        output.Append("</table>");
        output.Append("</div>");
        return output.ToString();
    }

    /// <summary>
    /// The create timeline.
    /// </summary>
    /// <param name="accountid">
    /// The accountid.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string CreateTimeline(int accountid)
    {
        var output = new StringBuilder();

        var subaccs = new cAccountSubAccounts(accountid);
        int subAccountId = subaccs.getFirstSubAccount().SubAccountID;

        var generalOptions = FunkyInjector.Container.GetInstance<IDataFactory<IGeneralOptions, int>>()[subAccountId]
            .WithSelfRegistration().WithCar();

        var steps = new ArrayList();
        int stepcount = 0;

        var stepsVisited = (ArrayList)this.ViewState["stepsVisited"];
        var step = new wizardStep(0, stepcount, "Employee Name &amp; Logon Details");
        steps.Add(step);
        stepcount++;
        step = new wizardStep(1, stepcount, "Enter a Password");
        steps.Add(step);
        stepcount++;
        if (generalOptions.SelfRegistration.AllowSelfRegEmployeeContact)
        {
            step = new wizardStep(2, stepcount, "Employee Contact Details");
            steps.Add(step);
            stepcount++;
        }

        if (generalOptions.SelfRegistration.AllowSelfRegHomeAddress)
        {
            step = new wizardStep(3, stepcount, "Employee Home Address &amp; Contact Details");
            steps.Add(step);

            stepcount++;
        }

        if (generalOptions.SelfRegistration.AllowSelfRegEmployeeInfo)
        {
            step = new wizardStep(4, stepcount, "Employment Details");
            steps.Add(step);
            stepcount++;
        }

        if (generalOptions.SelfRegistration.AllowSelfRegRole)
        {
            step = new wizardStep(5, stepcount, "Role");
            steps.Add(step);
            stepcount++;
        }

        if (generalOptions.SelfRegistration.AllowSelfRegSignOff)
        {
            step = new wizardStep(6, stepcount, "Signoff Group");
            steps.Add(step);
            stepcount++;
        }

        if (generalOptions.SelfRegistration.AllowSelfRegAdvancesSignOff)
        {
            step = new wizardStep(7, stepcount, "Signoff Group for Advances");
            steps.Add(step);
            stepcount++;
        }

        if (generalOptions.SelfRegistration.AllowSelfRegDepartmentCostCode)
        {
            step = new wizardStep(8, stepcount, "Coding Breakdown");
            steps.Add(step);
            stepcount++;
        }

        if (generalOptions.SelfRegistration.AllowSelfRegBankDetails)
        {
            step = new wizardStep(9, stepcount, "Bank Details");
            steps.Add(step);
            stepcount++;
        }

        if (generalOptions.SelfRegistration.AllowSelfRegCarDetails)
        {
            step = new wizardStep(10, stepcount, "Vehicle Details");
            steps.Add(step);
            stepcount++;
            if (generalOptions.Car.ShowMileageCatsForUsers)
            {
                step = new wizardStep(11, stepcount, "Mileage Details");
                steps.Add(step);
                stepcount++;
            }
        }

        if (generalOptions.SelfRegistration.AllowSelfRegUDF)
        {
            step = new wizardStep(12, stepcount, "Other Information");
            steps.Add(step);
            stepcount++;
        }

        step = new wizardStep(13, stepcount, "Summary");
        steps.Add(step);

        stepcount++;

        var distance = (float)Math.Round(100 / ((float)stepcount + 1), 1, MidpointRounding.AwayFromZero);
        for (int i = 0; i < steps.Count; i++)
        {
            step = (wizardStep)steps[i];
            float left = distance * (i + 1);
            output.Append("<div class=\"timelineevent\" style=\"left: " + left + "%;\">");
            if (stepsVisited.Contains(step.Actualstep))
            {
                output.Append("<a href=\"javascript:changeStep(" + step.Actualstep + ");\">");
            }

            if (this.wizregister.ActiveStepIndex == step.Actualstep)
            {
                output.Append(
                    "<img src=\"/shared/images/buttons/timeline_event" + (i + 1) + "_sel.gif\" class=\"timelineimg\">");
            }
            else
            {
                output.Append(
                    "<img src=\"/shared/images/buttons/timeline_event" + (i + 1) + ".gif\" class=\"timelineimg\">");
            }

            if (stepsVisited.Contains(step.Actualstep))
            {
                output.Append("</a>");
            }

            output.Append("<br>");
            output.Append("<span class=\"timeeventlabel\">" + step.Label + "</span>");
            output.Append("</div>\n");
        }

        return output.ToString();
    }

    /// <summary>
    /// The create udf summary.
    /// </summary>
    /// <param name="accountid">
    /// The accountid.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string CreateUdfSummary(int accountid)
    {
        var output = new StringBuilder();
        output.Append("<div class=\"inputpanel\">");
        output.Append("<div class=\"inputpaneltitle\">Other Information</div>");
        output.Append("<table>");

        var clstables = new cTables((int)this.ViewState["accountid"]);
        cTable tbl = clstables.GetTableByID(new Guid("972ac42d-6646-4efc-9323-35c2c9f95b62"));
        var clsUserdefinedFields = new cUserdefinedFields((int)this.ViewState["accountid"]);

        SortedList<int, object> lstUdfs = clsUserdefinedFields.getItemsFromPage(ref this.tbludf, tbl, false, string.Empty);
        var udfs = new HybridDictionary();
        this.ViewState["udfs"] = udfs;

        if (udfs != null)
        {
            foreach (KeyValuePair<int, object> kvp in lstUdfs)
            {
                cUserDefinedField reqUdf = clsUserdefinedFields.GetUserDefinedById(kvp.Key);

                output.Append("<tr><td class=\"labeltd\">" + reqUdf.label + "</td><td class=\"inputtd\">");

                if (reqUdf.fieldtype == FieldType.List && Convert.ToInt32(kvp.Value) > 0)
                {
                    output.Append(reqUdf.items[Convert.ToInt32(kvp.Value)].elementText);
                }
                else
                {
                    output.Append(kvp.Value);
                }

                output.Append("</td></tr>");
            }
        }

        output.Append("</table>");
        output.Append("</div>");
        return output.ToString();
    }

    /// <summary>
    /// The create user defined.
    /// </summary>
    /// <param name="accountid">
    /// The accountid.
    /// </param>
    public void CreateUserDefined(int accountid)
    {
        var clstables = new cTables(accountid);
        cTable tbl = clstables.GetTableByID(new Guid("972ac42d-6646-4efc-9323-35c2c9f95b62"));

        var clsUserdefinedFields = new cUserdefinedFields(accountid);
        List<cUserDefinedField> lstUdfs = clsUserdefinedFields.GetFieldsByTable(tbl);
        var lstUdfids = lstUdfs.Select(tmpUdf => tmpUdf.userdefineid).ToList();

        clsUserdefinedFields.addItemsToPage(
            ref this.tbludf, tbl, false, string.Empty, new SortedList<int, object>(), lstUdfids, string.Empty);
    }

    #endregion

    #region Methods

    /// <summary>
    /// The create user wizard 1_ created user.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void CreateUserWizard1CreatedUser(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// A public instance of <see cref="IEncryptor"/>
    /// </summary>
    [Dependency]
    public IEncryptor Encryptor { get; set; }

    /// <summary>
    /// An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> to get a <see cref="IProductModule"/>
    /// </summary>
    [Dependency]
    public IDataFactory<IProductModule, Modules> ProductModuleFactory { get; set; }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsPostBack == false)
        {
            this.Title = "Register";
            this.Master.Title = this.Title;

            this.chkusecar.CheckedChanged += this.ChkusecarCheckedChanged;
            this.cmbvehicletype.SelectedIndexChanged += this.CmbvehicletypeSelectedIndexChanged;
            this.littimeline.Text = this.CreateFirstTimeline();

            this.ViewState["stepsVisited"] = new ArrayList();

            this.txtretypeemail.Attributes.Add("onblur", "return txtretypeemail_onblur();");
            string[] server = this.Request.ServerVariables["SERVER_NAME"].Split('.');
            string companyid = server[0].Trim();

            var clsaccounts = new cAccounts();
            cAccount accnt = clsaccounts.GetAccountByCompanyID(companyid);
            if (accnt != null)
            {
                this.ViewState["accountid"] = accnt.accountid;
            }

            CurrentUser currentUser = cMisc.GetCurrentUser();
            IProductModule module;

            if (currentUser != null)
            {
                module = this.ProductModuleFactory[currentUser.CurrentActiveModule];
            }
            else
            {
                Modules activeModule = HostManager.GetModule(this.Request.Url.Host);
                module = this.ProductModuleFactory[activeModule];
            }

            string brandName = (module != null) ? module.BrandName : "Expenses";
            this.requsername.ErrorMessage = "Please enter a username you would like to logon to " + brandName + @" with";

            CarsBase.AddVehicleTypesToDropDownList(ref this.cmbvehicletype);

            txtaddresssearch.Attributes["rel"] = addressidhidden.ClientID;
        }
        else
        {
            if (this.Request.Form["requiredStep"] != string.Empty)
            {
                this.wizregister.ActiveStepIndex = int.Parse(this.Request.Form["requiredStep"]);
            }
        }

        int accountId;
        if (ViewState["accountid"] != null && int.TryParse(this.ViewState["accountid"].ToString(), out accountId))
        {
            HttpContext.Current.User = new TemporaryWebPrincipal(new UserIdentity(accountId, 0));

            this.CreateUserDefined(accountId);
            Session["accountid"] = accountId;
            FillCurrency();
            FillCountry();
            if (cmbAccounttype.Items.Count <= 0)
            {
                BankAccountsBase.AddAccountTypesToDropDownList(ref this.cmbAccounttype);
            }
        }
    }

    private void FillCurrency()
    {
        if (ddlCurrency.Items.Count <= 0)
        {
            var subaccs = new cAccountSubAccounts((int)ViewState["accountid"]);
            int subAccountId = subaccs.getFirstSubAccount().SubAccountID;
            var clscurrencies = new cCurrencies((int)ViewState["accountid"], subAccountId);
            List<ListItem> lstCurrency = clscurrencies.CreateDropDown();
            var itemNone = new ListItem("None", "0");
            lstCurrency.Insert(0, itemNone);
            ddlCurrency.Items.AddRange(lstCurrency.ToArray());
        }
    }

    /// <summary>
    /// Fill the country dropdown 
    /// </summary>
    private void FillCountry()
    {
        int accountId = (int)ViewState["accountid"];
        var subAccounts = new cAccountSubAccounts(accountId);
        int subAccountId = subAccounts.getFirstSubAccount().SubAccountID;
        var countryList = new cCountries(accountId, subAccountId);
        var defaultPrimaryCountry = this.cmbcountry.SelectedValue;
        var selectedBankCountry = this.ddlBankCountry.SelectedValue;

        cAccountProperties reqProperties = subAccounts.getSubAccountById(subAccountId).SubAccountProperties;

        if (!string.IsNullOrWhiteSpace(defaultPrimaryCountry.ToString()))
        {
            if (Convert.ToInt32(defaultPrimaryCountry) != 0)
            {
                if (string.IsNullOrWhiteSpace(selectedBankCountry.ToString()) || Convert.ToInt32(selectedBankCountry) == 0)
                {
                    ddlBankCountry.SelectedValue = defaultPrimaryCountry.ToString();
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(defaultPrimaryCountry.ToString()) && !string.IsNullOrWhiteSpace(selectedBankCountry.ToString()))
        {
            if (Convert.ToInt32(defaultPrimaryCountry) == 0 && Convert.ToInt32(selectedBankCountry) == 0)
            {
                ddlBankCountry.SelectedValue = reqProperties.HomeCountry.ToString();
            }
        }
        if (ddlBankCountry.Items.Count <= 0)
        {
            ddlBankCountry.Items.AddRange(countryList.CreateDropDown().ToArray());
        }
    }

    /// <summary>
    /// Enable/disable the control and their validation if vehicle other than bicycle is selected.
    /// </summary>
    /// <param name="sender">
    /// checkbox as sender
    /// </param>
    /// <param name="e">
    /// checkbox checked changed as Event arguments
    /// </param>
    protected void ChkusecarCheckedChanged(object sender, EventArgs e)
    {
        this.txtmodel.Enabled = this.chkusecar.Checked;
        this.txtmake.Enabled = this.chkusecar.Checked;
        this.txtregno.Enabled = this.chkusecar.Checked;
        this.cmbUom.Enabled = this.chkusecar.Checked;
        this.cmbcartype.Enabled = this.chkusecar.Checked;

        this.reqmake.Enabled = this.chkusecar.Checked;
        this.reqmodel.Enabled = this.chkusecar.Checked;

        bool isBicylcle = this.IsVehicleBicycle();

        this.reqreg.Enabled = isBicylcle == true && this.chkusecar.Checked;
        this.reqenginetype.Enabled = isBicylcle == true && this.chkusecar.Checked;
        this.cvEngineSize.Enabled = isBicylcle == true && this.chkusecar.Checked;
        this.rvVehicleType.Enabled = this.chkusecar.Checked;

        this.txtregno.Enabled = isBicylcle && this.chkusecar.Checked;
        this.txtEngineSize.Enabled = isBicylcle && this.chkusecar.Checked;
        this.cmbUom.Enabled = isBicylcle && this.chkusecar.Checked;
        this.cmbcartype.Enabled = isBicylcle && this.chkusecar.Checked;
        this.cmbvehicletype.Enabled = this.chkusecar.Checked;
        this.cmbvehicletype.SelectedIndex = 0;
        this.cmbUom.SelectedIndex = 0;
        this.cmbcartype.SelectedIndex = 0;
    }

    /// <summary>
    /// Enable/disable control on cmbvehicletype selected index changed.
    /// </summary>
    /// <param name="sender">
    /// comobox box as sender.
    /// </param>
    /// <param name="e">
    /// selectedindex changed as Event arguments.
    /// </param>
    protected void CmbvehicletypeSelectedIndexChanged(object sender, EventArgs e)
    {
        bool isBicycleSelected = this.IsVehicleBicycle();


        this.txtmodel.Enabled = this.chkusecar.Checked;
        this.txtmake.Enabled = this.chkusecar.Checked;
        this.txtregno.Enabled = this.chkusecar.Checked;
        this.cmbUom.Enabled = this.chkusecar.Checked;
        this.cmbcartype.Enabled = this.chkusecar.Checked;

        this.reqmake.Enabled = this.chkusecar.Checked;
        this.reqmodel.Enabled = this.chkusecar.Checked;
        this.reqreg.Enabled = !isBicycleSelected && this.chkusecar.Checked;
        this.reqenginetype.Enabled = !isBicycleSelected && this.chkusecar.Checked;
        this.cvEngineSize.Enabled = !isBicycleSelected && this.chkusecar.Checked;

        this.txtregno.Enabled = !isBicycleSelected && this.chkusecar.Checked;
        this.txtEngineSize.Enabled = !isBicycleSelected && this.chkusecar.Checked;
        this.cmbUom.Enabled = !isBicycleSelected && this.chkusecar.Checked;
        this.cmbcartype.Enabled = !isBicycleSelected && this.chkusecar.Checked;

        if (isBicycleSelected)
        {
            this.txtregno.Text = string.Empty;
            this.txtEngineSize.Text = string.Empty;
            this.cmbcartype.SelectedIndex = 0;
            this.cmbUom.SelectedIndex = 0;
        }

    }

    /// <summary>
    /// The is vehicle bicycle.
    /// </summary>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    protected bool IsVehicleBicycle()
    {
        return Convert.ToInt32(cmbvehicletype.SelectedValue.ToString()) == (int)CarTypes.VehicleType.Bicycle ? true : false;
    }

    /// <summary>
    /// The wizregister_ active step changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void WizregisterActiveStepChanged(object sender, EventArgs e)
    {
        // update details
        if (this.Request.Form["role"] != null)
        {
            this.ViewState["roleid"] = int.Parse(this.Request.Form["role"]);
        }

        if (this.Request.Form["group"] != null)
        {
            this.ViewState["groupid"] = int.Parse(this.Request.Form["group"]);
        }

        if (this.Request.Form["advancesgroup"] != null)
        {
            this.ViewState["advancesgroupid"] = int.Parse(this.Request.Form["advancesgroup"]);
        }

        if (this.Request.Form["mileageid"] != null)
        {
            this.ViewState["mileageid"] = int.Parse(this.Request.Form["mileageid"]);
        }

        if (this.wizregister.ActiveStepIndex == 0)
        {
            return;
        }

        // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
        var accountId = (int)this.ViewState["accountid"];
        var subaccs = new cAccountSubAccounts(accountId);
        int subAccountId = subaccs.getFirstSubAccount().SubAccountID;
        var currentUser = new CurrentUser(accountId, 0, 0, Modules.SpendManagement, subAccountId);

        HttpContext.Current.User = new TemporaryWebPrincipal(new UserIdentity(accountId, 0));

        var misc = new cMisc(accountId);
        bool move = false;

        var generalOptions = FunkyInjector.Container.GetInstance<IDataFactory<IGeneralOptions, int>>()[currentUser.CurrentSubAccountId]
            .WithSelfRegistration().WithCountry().WithCurrency().WithCar();

        var stepsVisited = (ArrayList)this.ViewState["stepsVisited"];
        stepsVisited.Add(this.wizregister.ActiveStepIndex);
        switch (this.wizregister.ActiveStepIndex)
        {
            case 0:
                this.littimeline.Text = this.CreateFirstTimeline();
                break;
            case 1:
                this.littimeline.Text = this.CreateTimeline(accountId);
                this.litpolicy.Text = this.CreatePolicy();
                break;
            case 2: // emp contact
                if (generalOptions.SelfRegistration.AllowSelfRegEmployeeContact == false)
                {
                    move = true;
                }

                this.littimeline.Text = this.CreateTimeline(accountId);
                break;
            case 3: // home details

                if (generalOptions.SelfRegistration.AllowSelfRegHomeAddress == false)
                {
                    move = true;
                }

                this.littimeline.Text = this.CreateTimeline(accountId);
                break;
            case 4: // emp info
                if (generalOptions.SelfRegistration.AllowSelfRegEmployeeInfo == false)
                {
                    move = true;
                }

                this.littimeline.Text = this.CreateTimeline(accountId);
                var countries = new cCountries(accountId, subAccountId);
                if (this.cmbcountry.Items.Count == 0)
                {
                    this.cmbcountry.Items.AddRange(countries.CreateDropDown().ToArray());
                    if (generalOptions.Country.HomeCountry != 0
                        && this.cmbcountry.Items.Contains(countries.GetListItem(generalOptions.Country.HomeCountry)))
                    {
                        if (countries.list.ContainsKey(generalOptions.Country.HomeCountry))
                        {
                            this.cmbcountry.Items.Add(countries.GetListItem(generalOptions.Country.HomeCountry));
                        }
                    }

                    if (this.cmbcountry.Items.FindByValue(generalOptions.Country.HomeCountry.ToString(CultureInfo.InvariantCulture)) != null)
                    {
                        this.cmbcountry.Items.FindByValue(generalOptions.Country.HomeCountry.ToString(CultureInfo.InvariantCulture)).Selected = true;
                    }
                }

                if (this.cmbcurrency.Items.Count == 0)
                {
                    var currencies = new cCurrencies(accountId, subAccountId);
                    this.cmbcurrency.Items.AddRange(currencies.CreateDropDown((int) generalOptions.Currency.BaseCurrency));
                }

                if (this.cmblinemanager.Items.Count == 0)
                {
                    var employees = new cEmployees(accountId);
                    this.cmblinemanager.Items.Add(new ListItem(string.Empty, "0"));
                    this.cmblinemanager.Items.AddRange(employees.CreateDropDown(0, false));
                }

                break;
            case 5: // role
                if (generalOptions.SelfRegistration.AllowSelfReg == false)
                {
                    move = true;
                }

                this.littimeline.Text = this.CreateTimeline(accountId);
                this.litroles.Text = this.CreateRoles(accountId);
                break;
            case 6: // signoff group
                if (generalOptions.SelfRegistration.AllowSelfRegSignOff == false)
                {
                    move = true;
                }

                this.littimeline.Text = this.CreateTimeline(accountId);
                this.litsignoffs.Text = this.CreateSignoffGroups(accountId, "group");
                break;
            case 7: // advances signoff
                if (generalOptions.SelfRegistration.AllowSelfRegAdvancesSignOff == false)
                {
                    move = true;
                }

                this.littimeline.Text = this.CreateTimeline(accountId);
                this.litadvancessignoffs.Text = this.CreateSignoffGroups(
                    accountId, "advancesgroup");
                break;
            case 8: // dep / costcode breakdown
                if (generalOptions.SelfRegistration.AllowSelfRegDepartmentCostCode == false)
                {
                    move = true;
                }

                cFieldToDisplay costcodeField = misc.GetGeneralFieldByCode("costcode");
                cFieldToDisplay departmentField = misc.GetGeneralFieldByCode("department");
                cFieldToDisplay projectcodeField = misc.GetGeneralFieldByCode("projectcode");

                this.lbldepartment.Text = departmentField.description;
                this.lblcostcode.Text = costcodeField.description;
                this.lblprojectcode.Text = projectcodeField.description;

                this.littimeline.Text = this.CreateTimeline(accountId);

                this.cmbcostcode.Items.AddRange(new cCostcodes(accountId).CreateDropDown(false).ToArray());
                this.cmbdepartment.Items.AddRange(new cDepartments(accountId).CreateDropDown(false).ToArray());
                this.cmbprojectcode.Items.AddRange(new cProjectCodes(accountId).CreateDropDown(false).ToArray());

                break;
            case 9: // bank details
                if (generalOptions.SelfRegistration.AllowSelfRegBankDetails == false)
                {
                    move = true;
                }

                this.littimeline.Text = this.CreateTimeline(accountId);
                break;
            case 10: // car details
                if (generalOptions.SelfRegistration.AllowSelfRegCarDetails == false)
                {
                    move = true;
                }

                this.littimeline.Text = this.CreateTimeline(accountId);
                bool isBicycleSelected = this.IsVehicleBicycle();

                this.txtmodel.Enabled = false;
                this.txtmake.Enabled = false;
                this.txtregno.Enabled = false;
                this.cmbUom.Enabled = false;
                this.cmbcartype.Enabled = false;
                this.cmbvehicletype.Enabled = false;
                this.txtmodel.Enabled = this.chkusecar.Checked;
                this.txtmake.Enabled = this.chkusecar.Checked;

                this.txtregno.Enabled = !isBicycleSelected && this.chkusecar.Checked;
                this.cmbcartype.Enabled = !isBicycleSelected && this.chkusecar.Checked;
                this.cmbUom.Enabled = !isBicycleSelected && this.chkusecar.Checked;
                this.txtEngineSize.Enabled = !isBicycleSelected && this.chkusecar.Checked;
                this.cmbvehicletype.Enabled = this.chkusecar.Checked;

                this.reqmake.Enabled = this.chkusecar.Checked;
                this.reqmodel.Enabled = this.chkusecar.Checked;

                CarsBase.AddCarEngineTypesToDropDownList(currentUser, ref this.cmbcartype);

                this.reqreg.Enabled = !isBicycleSelected && this.chkusecar.Checked;
                this.reqenginetype.Enabled = !isBicycleSelected && this.chkusecar.Checked;
                this.cvEngineSize.Enabled = !isBicycleSelected && this.chkusecar.Checked;
                this.rvVehicleType.Enabled = this.chkusecar.Checked;

                break;
            case 11:
                if (this.chkusecar.Checked == false || !generalOptions.Car.ShowMileageCatsForUsers)
                {
                    move = true;
                }
                else
                {
                    this.litmileage.Text = this.CreateMileage(accountId);
                    this.littimeline.Text = this.CreateTimeline(accountId);
                }

                break;
            case 12: // udf
                if (generalOptions.SelfRegistration.AllowSelfRegUDF == false)
                {
                    move = true;
                }

                this.littimeline.Text = this.CreateTimeline(accountId);

                break;
            case 13: // summary screen
                this.littimeline.Text = this.CreateTimeline(accountId);
                this.lblname.Text = string.Format("{0} {1} {2}", this.txttitle.Text, this.txtfirstname.Text, this.txtsurname.Text);
                this.lblemail.Text = this.txtemail.Text;
                this.lblusername.Text = this.txtusername.Text;
                if (generalOptions.SelfRegistration.AllowSelfRegEmployeeContact)
                {
                    this.litempcontact.Text = this.CreateEmployeeContactSummary();
                }

                if (generalOptions.SelfRegistration.AllowSelfRegHomeAddress)
                {
                    this.lithomeaddr.Text = this.CreateHomeAddressSummary();
                }

                if (generalOptions.SelfRegistration.AllowSelfRegEmployeeInfo)
                {
                    this.litempinfo.Text = this.CreateEmployementInfoSummary(accountId);
                }

                if (generalOptions.SelfRegistration.AllowSelfRegRole)
                {
                    this.litrole.Text = this.CreateRoleSummary(accountId);
                }

                if (generalOptions.SelfRegistration.AllowSelfRegSignOff)
                {
                    this.litsignoff.Text = this.CreateSignoffSummary(accountId);
                }

                if (generalOptions.SelfRegistration.AllowSelfRegAdvancesSignOff)
                {
                    this.litadvancessignoff.Text = this.CreateAdvancesSignoffSummary(accountId);
                }

                if (generalOptions.SelfRegistration.AllowSelfRegCarDetails)
                {
                    this.litcardetails.Text = this.CreateCarSummary(accountId, generalOptions.Car.ShowMileageCatsForUsers);
                }

                if (generalOptions.SelfRegistration.AllowSelfRegUDF)
                {
                    this.litudfs.Text = this.CreateUdfSummary(accountId);
                }

                break;
        }

        if (move)
        {
            this.wizregister.ActiveStepIndex += 1;
        }
    }

    /// <summary>
    /// The wizregister_ cancel button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void WizregisterCancelButtonClick(object sender, EventArgs e)
    {
        this.Response.Redirect("logon.aspx", true);
    }

    /// <summary>
    /// The wizregister_ finish button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void WizregisterFinishButtonClick(object sender, WizardNavigationEventArgs e)
    {
        var accountId = (int)this.ViewState["accountid"];
        var clsemployees = new cEmployees(accountId);

        // ONLY WORKS FOR A SINGLE SUBACCOUNT AT PRESENT.
        var subaccs = new cAccountSubAccounts(accountId);
        int subAccountId = subaccs.getFirstSubAccount().SubAccountID;
        cAccountProperties accountProperties = subaccs.getFirstSubAccount().SubAccountProperties;

        var clstables = new cTables(accountId);
        cTable tbl = clstables.GetTableByID(new Guid("972ac42d-6646-4efc-9323-35c2c9f95b62"));
        var clsUserdefinedFields = new cUserdefinedFields(accountId);

        SortedList<int, object> finaludfs = clsUserdefinedFields.getItemsFromPage(ref this.tbludf, tbl, false, string.Empty);

        int linemanager = 0;
        int primarycountry = 0;
        int primarycurrency = 0;
        int roleid = 0;
        int itemroleid = 0;
        int groupid = 0;
        int advancesgroupid = 0;
        int departmentid = 0;
        int costcodeid = 0;
        int projectcodeid = 0;

        var password = (string)this.ViewState["password"];

        string title = this.txttitle.Text;
        string firstname = this.txtfirstname.Text;
        string surname = this.txtsurname.Text;
        string email = this.txtemail.Text;
        string username = this.txtusername.Text;
        string telno = this.txthomephone.Text;
        string faxno = this.txthomefax.Text;
        string homeemail = this.txthomeemail.Text;
        string position = this.txtposition.Text;
        string payroll = this.txtpayrollnumber.Text;

        if (this.cmblinemanager.SelectedValue != null)
        {
            linemanager = int.Parse(this.cmblinemanager.SelectedValue);
        }

        string extension = this.txtextension.Text;
        string pager = this.txtpager.Text;
        string mobile = this.txtmobile.Text;
        if (this.cmbcountry.SelectedItem != null)
        {
            primarycountry = int.Parse(this.cmbcountry.SelectedValue);
        }

        if (this.cmbcurrency.SelectedItem != null)
        {
            primarycurrency = int.Parse(this.cmbcurrency.SelectedValue);
        }

        if (this.ViewState["roleid"] != null)
        {
            roleid = (int)this.ViewState["roleid"];
        }

        if (this.ViewState["itemroleid"] != null)
        {
            itemroleid = (int)this.ViewState["itemroleid"];
        }

        if (this.ViewState["groupid"] != null)
        {
            groupid = (int)this.ViewState["groupid"];
        }

        if (this.ViewState["advancesgroupid"] != null)
        {
            advancesgroupid = (int)this.ViewState["advancesgroupid"];
        }

        if (this.cmbdepartment.SelectedItem != null)
        {
            if (this.cmbdepartment.SelectedValue != string.Empty)
            {
                departmentid = int.Parse(this.cmbdepartment.SelectedValue);
            }
        }

        if (this.cmbcostcode.SelectedItem != null)
        {
            if (this.cmbcostcode.SelectedValue != string.Empty)
            {
                costcodeid = int.Parse(this.cmbcostcode.SelectedValue);
            }
        }

        if (this.cmbprojectcode.SelectedItem != null)
        {
            if (this.cmbprojectcode.SelectedValue != string.Empty)
            {
                projectcodeid = int.Parse(this.cmbprojectcode.SelectedValue);
            }
        }

        int engineSize = this.txtEngineSize.Text == string.Empty ? 0 : int.Parse(this.txtEngineSize.Text.Trim());

        string accountName = (string.IsNullOrEmpty(this.txtaccountholdername.Text) == false ? this.txtaccountholdername.Text.Trim() : string.Empty);
        string accountNumber = (string.IsNullOrEmpty(this.txtaccountholdernumber.Text) == false ? this.txtaccountholdernumber.Text.Trim() : string.Empty);
        string sortCode = (string.IsNullOrEmpty(this.txtsortcode.Text) == false ? this.txtsortcode.Text.Trim() : string.Empty);
        string accountReference = (string.IsNullOrEmpty(this.txtaccountreference.Text) == false ? this.txtaccountreference.Text.Trim() : string.Empty);
        var iban = string.IsNullOrEmpty(this.txtIban.Text) == false ? this.txtIban.Text.Trim() : string.Empty;
        var swiftCode = string.IsNullOrEmpty(this.txtSwiftCode.Text) == false ? this.txtSwiftCode.Text.Trim() : string.Empty;


        int currencyId = (ddlCurrency.SelectedIndex > 0 ? Convert.ToInt32(ddlCurrency.SelectedValue) : 0);
        int bankCountryId = (ddlBankCountry.SelectedIndex > 0 ? Convert.ToInt32(ddlBankCountry.SelectedValue) : 0);
        string accountType = (cmbAccounttype.SelectedIndex > 0 ? cmbAccounttype.SelectedValue : string.Empty);

        if (this.ViewState["udfs"] != null)
        {
            var udfs = (HybridDictionary)this.ViewState["udfs"];
            IDictionaryEnumerator enumer = udfs.GetEnumerator();

            while (enumer.MoveNext())
            {
                finaludfs.Add((int)enumer.Key, enumer.Value);
            }
        }

        var clsAccessRoles = new cAccessRoles(accountId, cAccounts.getConnectionString(accountId));
        cAccessRole reqAccessRole = clsAccessRoles.GetAccessRoleByID(roleid);
        var breakdown = new cDepCostItem[1];
        breakdown[0] = new cDepCostItem(departmentid, costcodeid, projectcodeid, 100);

        var clsItemRoles = new ItemRoles(accountId);
        var lstItemRoles = new List<EmployeeItemRole>();
        if (itemroleid != 0)
        {
            ItemRole reqItemRole = clsItemRoles.GetItemRoleById(itemroleid);
            lstItemRoles.Add(new EmployeeItemRole(reqItemRole.ItemRoleId));
        }

        var subAccAccessRoles = new Dictionary<int, List<int>>();
        var lstAccessRoles = new List<int> { reqAccessRole.RoleID };
        subAccAccessRoles.Add(subAccountId, lstAccessRoles);

        var encryptedPassword = this.EncryptPassword(password);

        Session["currencyId"] = 0;

        // Save the new employee
        var reqEmp = new Employee(accountId, 0, username, encryptedPassword, email, title, firstname, string.Empty, string.Empty, surname, false, false, false, false, 0, 0, DateTime.UtcNow, 0, null, null, new BankAccount(accountName, accountNumber, accountType, sortCode, accountReference, bankCountryId, iban:iban, swiftCode: swiftCode), groupid, extension, mobile, pager, faxno, homeemail, linemanager, advancesgroupid, string.Empty, string.Empty, null, null, null, payroll, position, telno, string.Empty, CreationMethod.SelfRegistration, PasswordEncryptionMethod.RijndaelManaged, true, false, subAccountId, primarycurrency, primarycountry, 0, 0, false, null, null, string.Empty, string.Empty, string.Empty, null, null, null, 1, DateTime.UtcNow, 1, 0, null, false, currency: currencyId);
        reqEmp.EmployeeID = clsemployees.SaveEmployee(reqEmp, breakdown, new List<int>(), finaludfs);
        var bankValidation = this.ViewState["BankAccountValidation"] as IBankAccountValid;
        if (bankValidation != null)
        {
            // Save Account Validation
            var currentUser = cMisc.GetCurrentUser($"{this.ViewState["accountid"]}, {reqEmp.EmployeeID}");
            var bankAccounts = new BankAccounts(currentUser.AccountID, currentUser.EmployeeID);
            var bankAccount = bankAccounts.GetAccountAsListByEmployeeId(currentUser.EmployeeID).FirstOrDefault();
            if (bankAccount != null)
            {
                var bankAccountValidationResults = new BankAccountValidationResults(currentUser);
                bankAccountValidationResults.Save(bankAccount, bankValidation);
            }
        }
        // If we have a valid employee id and the user is allowed to set their home address then save the new address
        if (reqEmp.EmployeeID > 0 && accountProperties.AllowSelfRegHomeAddress)
        {
            int newAddressId;
            int.TryParse(addressidhidden.Value, out newAddressId);

            if (newAddressId == 0)
            {
                int countryId = this.GetCountryID(txtcountry.Text);
                var currentUser = new CurrentUser(accountId, reqEmp.EmployeeID, 0, Modules.SpendManagement, subAccountId);

                int udprn = 0;
                int.TryParse(hdnUdprn.Value, out udprn);

                Address.AddressCreationMethod creationMethod = string.IsNullOrEmpty(hdnGlobalIdentifier.Value) ? Address.AddressCreationMethod.ManualByClaimant : Address.AddressCreationMethod.CapturePlus;
                newAddressId = Address.Save(currentUser, newAddressId, txtAddressName.Text, txtaddressline1.Text, txtaddressline2.Text,
                                            string.Empty,
                                            txtcity.Text,
                                            txtcounty.Text, countryId, txtpostcode.Text, hdnLatitude.Value,
                                            hdnLongitude.Value, hdnGlobalIdentifier.Value, udprn, false, creationMethod);
                if (newAddressId == -1)
                {
                    newAddressId = Address.Find(currentUser, txtAddressName.Text, txtaddressline1.Text, txtcity.Text, txtpostcode.Text, countryId);
                }
            }

            // If we have a valid address id then save the new address as the new employees home location
            if (newAddressId > 0)
            {
                this.SaveHomeLocation(accountId, reqEmp.EmployeeID, newAddressId, this.txtdateataddress.Text);
            }
        }

        if (lstAccessRoles.Count > 0)
        {
            reqEmp.GetAccessRoles().Add(lstAccessRoles, subAccountId, null);
        }

        if (lstItemRoles.Count > 0)
        {
            reqEmp.GetItemRoles().Add(lstItemRoles, null);
        }

        byte checkpwd = clsemployees.checkpassword(password, accountId, reqEmp.EmployeeID, this.Encryptor, FunkyInjector.Container.GetInstance<IDataFactory<IGeneralOptions, int>>()[subAccountId]);
        reqEmp.ChangePassword(string.Empty, password, false, checkpwd, accountProperties.PwdHistoryNum, null, this.Encryptor);
        bool usecar = this.chkusecar.Checked;

        if (usecar)
        {
            string make = this.txtmake.Text;
            string model = this.txtmodel.Text;
            string registration = this.txtregno.Text;
            byte enginetype = byte.Parse(this.cmbcartype.SelectedValue);
            byte vehicletypeid = byte.Parse(this.cmbvehicletype.SelectedValue);

            var mileagecats = new List<int>();
            if (this.ViewState["mileageid"] != null)
            {
                var mileageid = (int)this.ViewState["mileageid"];
                mileagecats.Add(mileageid);
            }

            var reqCar = new cCar(
                accountId,
                reqEmp.EmployeeID,
                0,
                make,
                model,
                registration,
                null,
                null,
                true,
                mileagecats,
                enginetype,
                0,
                false,
                0,
                MileageUOM.Mile,
                engineSize,
                DateTime.Now,
                reqEmp.EmployeeID,
                null,
                null,
                false,
                false,
                vehicletypeid,
                null,
                false,
                null,
                false);

            var clsEmployeeCars = new cEmployeeCars(accountId, reqEmp.EmployeeID);
            clsEmployeeCars.SaveCar(reqCar);
        }

        this.SendValidateEmail(reqEmp);

        this.Response.Redirect("~/shared/register_success.aspx", true);
    }

    /// <summary>
    /// The wizregister_ next button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void WizregisterNextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        cEmployees clsemployees;
        cAccountSubAccounts clsSubAccounts;

        var stepsVisited = (ArrayList)this.ViewState["stepsVisited"];
        stepsVisited.Add(this.wizregister.ActiveStepIndex);
        switch (e.CurrentStepIndex)
        {
            case 0: // general details
                if (this.ViewState["accountid"] == null)
                {
                    if (this.GetAccountId() == false)
                    {
                        this.lblmsggeneral.Text = "Sorry, the e-mail address you have entered is invalid or your company does not use self registration.";
                        this.lblmsggeneral.Visible = true;
                        e.Cancel = true;
                        return;
                    }
                }

                var accountId = (int) this.ViewState["accountid"];

                HttpContext.Current.User = new TemporaryWebPrincipal(new UserIdentity(accountId, 0));

                clsemployees = new cEmployees(accountId);

                // check if emps can self register
                clsSubAccounts = new cAccountSubAccounts(accountId);
                cAccountProperties reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties.Clone();

                if (reqProperties.AllowSelfReg == false)
                {
                    this.lblmsggeneral.Text =
                        "Sorry, your organisation does not allow you to self register. Please contact your administrator.";
                    this.lblmsggeneral.Visible = true;
                    e.Cancel = true;
                    return;
                }

                // check username does not already exist
                if (clsemployees.alreadyExists(
                    this.txtusername.Text.Trim().ToLower(), 0, 0, accountId))
                {
                    this.lblmsggeneral.Text = "The username you have entered has already been used. Please enter a different username.";
                    this.lblmsggeneral.Visible = true;
                    e.Cancel = true;
                }

                // set the default role
                this.ViewState["roleid"] = reqProperties.DefaultRole;

                // set the default item role
                this.ViewState["itemroleid"] = reqProperties.DefaultItemRole;

                break;
            case 1: // password
                accountId = (int) this.ViewState["accountid"];
                clsemployees = new cEmployees(accountId);
                clsSubAccounts = new cAccountSubAccounts(accountId);

                if (clsemployees.checkpassword(this.txtnew.Text, (int)this.ViewState["accountid"], 0, this.Encryptor, FunkyInjector.Container.GetInstance<IDataFactory<IGeneralOptions, int>>()[clsSubAccounts.getFirstSubAccount().SubAccountID]) > 0)
                {
                    this.lblpassword.Text = "The password you have entered does not conform to the password policy.";
                    this.lblpassword.Visible = true;
                    e.Cancel = true;
                    return;
                }

                this.ViewState["password"] = this.txtrenew.Text;
                break;
            case 9: // Bank
                // Validate the bank account entry
                var currentUser = cMisc.GetCurrentUser($"{this.ViewState["accountid"]}, 0");
                var bankAccount = new SpendManagementLibrary.Account.BankAccount(0, 0, this.txtaccountholdername.Text, this.txtaccountholdernumber.Text, Convert.ToInt32(this.cmbAccounttype.SelectedValue), this.txtsortcode.Text, this.txtaccountreference.Text, Convert.ToInt32(this.ddlCurrency.SelectedValue), Convert.ToInt32(this.ddlBankCountry.SelectedValue), false);
                var bankAccounts = new BankAccounts(currentUser.AccountID, currentUser.EmployeeID);
                var bankvalidator = this.GetBankValidator(currentUser, bankAccounts);
                IBankAccountValid validateResult = bankvalidator.Validate(bankAccount);
                if (!validateResult.IsCorrect)
                {

                    switch (validateResult.StatusInformation)
                    {
                        case "UnknownSortCode":
                            this.lblmsgBankDetails.Text = @"The Sort Code entered is unknown.";
                            break;
                        case "InvalidAccountNumber":
                            this.lblmsgBankDetails.Text = @"The Account Number you have entered is invalid.";
                            break;
                        case "DetailsChanged":
                            this.lblmsgBankDetails.Text = $@"The Account and Sortcode should be changed for BACS submission. The Sort Code should be '{validateResult.CorrectedSortCode}' and the Account Number should be '{validateResult.CorrectedAccountNumber}'";
                            break;
                        default:
                            this.lblmsgBankDetails.Text = validateResult.StatusInformation;
                            break;
                    }
                    this.lblmsgBankDetails.Visible = true;
                    e.Cancel = true;
                }
                else
                {
                    this.ViewState["BankAccountValidation"] = validateResult;
                }
                break;
        }
    }

    /// <summary>
    /// Get a new instance of <see cref="IBankAccountValidator"/>
    /// </summary>
    /// <param name="currentUser">The current user to create the instance for</param>
    /// <param name="bankAccounts">An instance of <see cref="BankAccounts"/></param>
    /// <returns>A new instance of <see cref="IBankAccountValidator"/></returns>
    private IBankAccountValidator GetBankValidator(ICurrentUser currentUser,BankAccounts bankAccounts)
    {
        var bankAccountValidationResults = new BankAccountValidationResults(currentUser);
        var clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
        var subAccount = clsSubAccounts.getFirstSubAccount();
        var countries = new cCountries(currentUser.AccountID, subAccount.SubAccountID);
        return new BankAccountValidator(new PostCodeAnywhereBankAccountValidator(currentUser.Account), bankAccountValidationResults, bankAccounts, countries);
    }

    /// <summary>
    /// The create policy.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string CreatePolicy()
    {
        var subaccounts = new cAccountSubAccounts((int)this.ViewState["accountid"]);

        var properties = subaccounts.getFirstSubAccount();
        var output = new StringBuilder();

        var text = properties.PasswordPolicyText;
        foreach (string line in text)
        {
            output.Append($"<li>{line}");
        }

        if (output.Length > 0)
        {
            output.Insert(0, "<ul>");
            output.Insert(0, "<div class=\"inputpaneltitle\">Password Policy</div>");
            output.Insert(0, "<div class=\"inputpanel\">");

            output.Append("</ul>");
        }

        return output.ToString();
    }

    /// <summary>
    /// The get account id.
    /// </summary>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private bool GetAccountId()
    {
        if (this.ViewState["accountid"] != null)
        {
            return true;
        }

        var email = this.txtemail.Text;

        var suffix = email.Trim().ToLower();
        suffix = suffix.Substring(suffix.IndexOf("@") + 1, suffix.Length - suffix.IndexOf("@") - 1);

        var accountid = 0;

        var dbsChecked = new List<string>();

        foreach (cAccount reqAccount in cAccounts.CachedAccounts.Values)
        {
            if (reqAccount.archived == false && dbsChecked.Contains(reqAccount.dbname) == false)
            {
                cEmailSuffixes suffixes = new cEmailSuffixes(reqAccount.accountid);
                if (suffixes.getSuffixByValue(suffix) != null)
                {
                    if (accountid > 0)
                    {
                        // More than one account has been found, return false
                        return false;
                    }

                    accountid = reqAccount.accountid;
                    dbsChecked.Add(reqAccount.dbname);
                }
            }
        }

        if (accountid == 0)
        {
            return false;
        }

        this.ViewState["accountid"] = accountid;
        this.ViewState["suffix"] = suffix;
        return true;
    }

    /// <summary>
    /// The send validate email.
    /// </summary>
    /// <param name="employeeid">
    /// The employeeid.
    /// </param>
    private void SendValidateEmail(Employee employee)
    {
        var subaccs = new cAccountSubAccounts((int)this.ViewState["accountid"]);

        cAccountProperties properties = subaccs.getSubAccountById(employee.DefaultSubAccount).SubAccountProperties;

        var output = new StringBuilder();

        CurrentUser currentUser = cMisc.GetCurrentUser();

        IProductModule module;

        if (currentUser != null)
        {
            module = this.ProductModuleFactory[currentUser.CurrentActiveModule];
        }
        else
        {
            Modules activeModule = HostManager.GetModule(this.Request.Url.Host);
            module = this.ProductModuleFactory[activeModule];
        }

        string brandName = (module != null) ? module.BrandName : "Expenses";

        string subject = brandName + " registration details verification";
        string server = this.Request.ServerVariables["SERVER_NAME"];
        output.Append("Dear " + employee.Forename + "\n\n");

        output.Append("Thank you for registering with " + brandName + ".\n\n");

        output.Append(
            "In order to ensure the security of " + brandName
            +
            " and that your details are correct, please click the link below to take you to a page to verify that you are "
            + employee.Forename + " " + employee.Surname + ":\n\n");

        output.Append(
            "https://" + server + "/shared/verify.aspx?accountid=" + this.ViewState["accountid"] + "&employeeid="
            + employee.EmployeeID);

        output.Append("\n\nMany Thanks");

        string fromAddress = "admin@sel-expenses.com";
        if (properties.SourceAddress == 1 && properties.EmailAdministrator != string.Empty)
        {
            fromAddress = properties.EmailAdministrator;
        }

        var sender = new EmailSender(properties.EmailServerAddress);
        sender.SendEmail(fromAddress, employee.EmailAddress, subject, output.ToString());
    }

    /// <summary>
    /// The encrypt password.
    /// </summary>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    private string EncryptPassword(string password)
    {
        return Utilities.Cryptography.ExpensesCryptography.Encrypt(password);
    }

    /// <summary>
    /// Searches the database and returns the id of a country
    /// </summary>
    /// <param name="country">Country name to search for</param>
    /// <returns>Country Id</returns>
    private int GetCountryID(string country)
    {
        int countryId = 0;

        if (country != string.Empty)
        {
            cGlobalCountries clscountries = new cGlobalCountries();
            cGlobalCountry clscountry = clscountries.getCountryByName(country);
            if (clscountry != null)
            {
                countryId = clscountry.GlobalCountryId;
            }
        }

        return countryId;
    }

    /// <summary>
    /// Creates an entry in the employeeHomeLocations table to link a home address to an employee
    /// </summary>
    /// <param name="accountId">Organisation account id</param>
    /// <param name="newEmployeeid">Employee Id</param>
    /// <param name="newAddressId">Address Id</param>
    /// <param name="startdate">Date the address is valid from for making claims</param>
    /// <returns>True or false to indicate the save status</returns>
    private bool SaveHomeLocation(int accountId, int newEmployeeid, int newAddressId, string startdate)
    {
        // Make sure we have a valid date
        DateTime dtStartDate;
        DateTime.TryParse(startdate, out dtStartDate);
        if (dtStartDate == DateTime.MinValue)
        {
            dtStartDate = DateTime.Now;
        }

        cEmployees clsemployees = new cEmployees(accountId);
        Employee employee = clsemployees.GetEmployeeById(newEmployeeid);
        cEmployeeHomeLocation location = new cEmployeeHomeLocation(0, newEmployeeid, newAddressId, dtStartDate, null, DateTime.Now, newEmployeeid, null, null);

        employee.GetHomeAddresses().Add(location, null);
        return true;
    }

    #endregion

    #region WebMethods
    [WebMethod(EnableSession = true)]
    public static List<string> getCountryList(string prefixText)
    {
        DBConnection expdata = new DBConnection(ConfigurationManager.ConnectionStrings["metabase"].ConnectionString);
        List<string> items = new List<string>();
        string strsql = "select top 10 country from global_countries where country like @country order by country";

        expdata.sqlexecute.Parameters.AddWithValue("@country", prefixText + "%");
        SqlDataReader reader = expdata.GetReader(strsql);
        expdata.sqlexecute.Parameters.Clear();

        while (reader.Read())
        {
            items.Add(reader.GetString(0));
        }

        reader.Close();
        return items;
    }
    #endregion
}