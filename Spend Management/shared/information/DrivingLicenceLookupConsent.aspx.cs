namespace Spend_Management.shared.information
{
    using System;
    using System.Globalization;

    using SpendManagementLibrary.Employees.DutyOfCare;
    using System.Web.Services;
    using DutyOfCareAPI.DutyOfCare;
    using SpendManagementLibrary.DVLA;
    using code;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Bootstrap;

    using BusinessLogic.Modules;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;

    using Spend_Management.shared.code.DVLA;

    public partial class DrivingLicenceLookupConsent : System.Web.UI.Page
    {
        /// <summary>
        /// Page Load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                this.Title = @"DVLA Check Consent";
                this.Master.title = this.Title;
                this.Master.enablenavigation = false;

                var user = cMisc.GetCurrentUser();       
                var subAccounts = new cAccountSubAccounts(user.AccountID);
                var subAccount = subAccounts.getFirstSubAccount();
                var accountProperties = subAccount.SubAccountProperties;
                var entityview = new DutyOfCareDocumentsInformation().GetDocEntityAndViewIdByGuid("223018FE-EDAE-408E-8851-C09ABA09DF81", "F95C0754-82FA-493A-97B4-E1ED42B3C337", user.AccountID);
                this.dvlaLookUpFrequencyValue.Text = DvlaConsentLookUp.BuildFrequencyMessage(accountProperties.DrivingLicenceLookupFrequency).Trim();

                if (user.CurrentActiveModule != Modules.Expenses || user.isDelegate || !(accountProperties.EnableAutomaticDrivingLicenceLookup && user.Account.HasDvlaLookupKeyAndDvlaConnectLicenceElement(SpendManagementElement.DvlaConnect)) || !user.CheckAccessRole(AccessRoleType.View, CustomEntityElementType.View, Convert.ToInt32(entityview.Split(',')[0]), Convert.ToInt32(entityview.Split(',')[1]), false))
                {
                    this.Response.Redirect("~/shared/restricted.aspx?reason=Current%20access%20role%20does%20not%20permit%20you%20to%20view%20this%20page.", true);
                }
                this.txtLookUpDate.Text = user.Employee.DvlaLookUpDate != null ? Convert.ToDateTime(user.Employee.DvlaLookUpDate).ToString("dd/MM/yyyy") : string.Empty;
                this.txtFirstName.Text = user.Employee.DrivingLicenceFirstname ?? user.Employee.Forename;
                this.txtSurName.Text = user.Employee.DrivingLicenceSurname ?? user.Employee.Surname;
                this.txtMiddleName.Text = user.Employee.DrivingLicenceMiddleName ?? user.Employee.MiddleNames;
                this.txtEmail.Text = user.Employee.DrivingLicenceEmail ?? user.Employee.EmailAddress;  
                this.txtConsentExpiryDate.Text = user.Employee.DrivingLicenceDateOfBirth != null ? Convert.ToDateTime(user.Employee.DrivingLicenceDateOfBirth).ToString("dd/MM/yyyy") : (user.Employee.DateOfBirth != null ? Convert.ToDateTime(user.Employee.DateOfBirth).ToString("dd/MM/yyyy") : null);
                var licenceNumber = user.Employee.GetDrivingLicenceNumberByEmployeeId(user.EmployeeID, user.AccountID);
                this.txtDrivingLicenceNumber.Text = user.Employee.DrivingLicenceNumber ?? licenceNumber;
                this.txtDateOfBirth.Text = user.Employee.DrivingLicenceDateOfBirth != null ? Convert.ToDateTime(user.Employee.DrivingLicenceDateOfBirth).ToString("dd/MM/yyyy") : (user.Employee.DateOfBirth != null ? Convert.ToDateTime(user.Employee.DateOfBirth).ToString("dd/MM/yyyy") : null);
                this.ddlsex.SelectedValue = user.Employee.DrivingLicenceSex ?? user.Employee.GetGenderValue().ToString();
                if (!string.IsNullOrEmpty(user.Employee.SecurityCode.ToString()) && user.Employee.SecurityCode != Guid.Empty && user.Employee.AgreeToProvideConsent.HasValue && user.Employee.AgreeToProvideConsent.Value)
                {
                    this.txtConsentProvided.Text = user.Employee.DvlaConsentDate.ToString();
                    this.txtConsentExpiryDate.Text = Convert.ToDateTime(user.Employee.DvlaConsentDate).AddYears(3).ToString("dd/MM/yyyy");
                    this.txtSecurityCode.Text = user.Employee.SecurityCode.ToString();
                    this.consentDetailsHolder.Visible = true;
                }
            }
        }

        /// <summary>
        /// Navigate to previous page
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        protected void CmdCancelClick(object sender, EventArgs eventArgs)
        {
            var user = cMisc.GetCurrentUser();
            var customMenu = new CustomMenuStructure(user.AccountID);
            var customMenuItem = customMenu.GetCustomMenuIdByName("My Duty of Care Documents",true);
            this.Response.Redirect(cMisc.Path + "/shared/ViewCustomMenu.aspx?menuid=" + customMenuItem);
        }

        /// <summary>
        /// Gets the static content URL
        /// </summary>
        /// <returns>The URL to static content</returns>
        public string GetStaticLibraryPath()
        {
            return GlobalVariables.StaticContentLibrary;
        }

        /// <summary>
        /// Records the user as having denied consent and takes them back to the previous page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdDenyConsent_OnClick(object sender, EventArgs e)
        {
            var user = cMisc.GetCurrentUser();
            var connection = new DatabaseConnection(cAccounts.getConnectionString(user.AccountID));
            var audit = new cAuditLog(user.AccountID, user.EmployeeID);

            DvlaConsentLookUp.DenyDvlaConsent(user, connection);
            DvlaConsentLookUp.NotifyApproverOnDenyOfConsent(user);

            if (user.Employee.AgreeToProvideConsent.HasValue && user.Employee.AgreeToProvideConsent.Value == true)
            {
                var api = BootstrapDvla.CreateNew();
                if (user.Employee.SecurityCode != null) api.InvalidateUsersConsent(user.Employee.SecurityCode.Value);
                audit.editRecord(user.EmployeeID, "Check consent", SpendManagementElement.DvlaConnect, Guid.Empty,"Agreed","Revoked");
            }
            else
            {
                audit.editRecord(user.EmployeeID, "Check consent", SpendManagementElement.DvlaConnect, Guid.Empty, "", "Denied");
            }

            var customMenu = new CustomMenuStructure(user.AccountID);
            var customMenuItem = customMenu.GetCustomMenuIdByName("My Duty of Care Documents", true);
            this.Response.Redirect(cMisc.Path + "/shared/ViewCustomMenu.aspx?menuid=" + customMenuItem);
        }
    }
}