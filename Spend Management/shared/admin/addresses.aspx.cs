namespace Spend_Management.shared.admin
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// The address admin page
    /// </summary>
    public partial class addresses : Page
    {
        /// <summary>
        /// Is the postcode field mandatory
        /// </summary>
        public bool PostcodeMandatory = false;

        /// <summary>
        /// The global country identifier of the default home country for the account as a string for inclusion in javascript
        /// </summary>
        public string DefaultCountry = string.Empty;

        /// <summary>
        /// Whether to force users to enter an address name before they can use the address, this option is only available to Address+ Standard (TeleAtlas) accounts
        /// </summary>
        public bool ForceAddressNameEntry { get; set; }

        /// <summary>
        /// The message presented to users when they are prompted to enter an address name
        /// </summary>
        public string AddressNameEntryMessage { get; set; }

        #region Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Master.Page.Title = "Addresses";
            this.Master.PageSubTitle = "Addresses";
            this.Master.UseDynamicCSS = true;

            CurrentUser currentUser = cMisc.GetCurrentUser();

            #region Postback, add and view functionality

            if (this.IsPostBack)
            {
                return;
            }

            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Addresses, true, true);

            if (currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Addresses, true, false) == false)
            {
                this.lnkNewAddress.Style.Add(HtmlTextWriterStyle.Display, "none");
            }

            #endregion Postback, add and view functionality

            #region Get preferences

            cAccountProperties subAccountProperties = new cAccountSubAccounts(currentUser.AccountID).getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;

            if (subAccountProperties.MandatoryPostcodeForAddresses)
            {
                this.rfvPostcode.Enabled = true;
                this.PostcodeMandatory = true;
                this.lblPostcode.Text += "*";
                this.lblPostcode.CssClass = "mandatory";
            }

            this.ForceAddressNameEntry = subAccountProperties.ForceAddressNameEntry;
            this.AddressNameEntryMessage = StringManipulators.HtmlSafe(subAccountProperties.AddressNameEntryMessage);

            #endregion Get preferences

            #region Populate countries

            cCountries countries = new cCountries(currentUser.AccountID, currentUser.CurrentSubAccountId);
            this.ddlCountry.Items.Add(new ListItem("[None]", string.Empty));
            foreach (cGlobalCountry country in countries.GetPostcodeAnywhereEnabledCountries())
            {
                this.ddlCountry.Items.Add(new ListItem(country.Country, country.Alpha3CountryCode));
            }

            if (subAccountProperties.HomeCountry > 0 && countries.getCountryById(subAccountProperties.HomeCountry) != null)
            {
                var globalCountry = new cGlobalCountries().getGlobalCountryById(countries.getCountryById(subAccountProperties.HomeCountry).GlobalCountryId);
                this.DefaultCountry = globalCountry.Alpha3CountryCode;
            }

            #endregion Populate countries

            #region Grid

            string gridHtml = string.Empty,
                gridJavaScript = string.Empty,
                gridAwabelsHtml = string.Empty,
                gridAwabelsJavaScript = string.Empty;

            string[] gridStrings = this.Grid(currentUser);

            if (gridStrings.Length == 2)
            {
                gridJavaScript = gridStrings[0];
                gridHtml = gridStrings[1];
            }

            gridStrings = AccountWideLabelsGrid(currentUser, 0);

            if (gridStrings.Length == 2)
            {
                gridAwabelsJavaScript = gridStrings[0];
                gridAwabelsHtml = gridStrings[1];
            }

            #endregion Grid

            this.phAddressesGrid.Controls.Add(new Literal { Text = gridHtml });

            this.phAwabelsGrid.Controls.Add(new Literal { Text = gridAwabelsHtml });

            this.ClientScript.RegisterStartupScript(this.GetType(), "addressesGridJavaScript", cGridNew.generateJS_init("addressesGridJavaScript", new List<string> { gridJavaScript, gridAwabelsJavaScript }, currentUser.CurrentActiveModule), true);
        }

        /// <summary>
        /// Grid for addresses and favourites
        /// </summary>
        /// <param name="currentUser">The current user object for account and employee</param>
        /// <returns>Grid strings for javascript and html</returns>
        private string[] Grid(ICurrentUser currentUser)
        {
            const string GridName = "gridAddresses";
            cGridNew grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, GridName, Addresses.GridSql)
            {
                KeyField = "AddressID",
                EmptyText = "There are no Addresses to display.",
                editlink = "javascript:SEL.Addresses.Address.Edit({AddressID});",
                deletelink = "javascript:SEL.Addresses.Address.Delete({AddressID});",
                enableupdating = currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Addresses, true, false),
                enabledeleting = currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Addresses, true, false)
            };

            grid.getColumnByName("AddressID").hidden = true;
            grid.getColumnByName("CreationMethod").hidden = true;

            grid.InitialiseRow += this.AddressesGridOnInitialiseRow;
            grid.ServiceClassForInitialiseRowEvent = "Spend_Management.shared.admin.addresses";
            grid.ServiceClassMethodForInitialiseRowEvent = "AddressesGridOnInitialiseRow";
            if (currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Addresses, true, false))
            {
                grid.addTwoStateEventColumn("Archived", (cFieldColumn)grid.getColumnByName("Archived"), true, false, "/shared/images/icons/folder_into.png", "javascript:SEL.Addresses.Address.ToggleArchive({AddressID});", "Click to un-archive.", "Un-archive", "/shared/images/icons/folder_lock.png", "javascript:SEL.Addresses.Address.ToggleArchive({AddressID});", "Click to archive.", "Archive");
            }
            else
            {
                grid.getColumnByName("Archived").hidden = true;
            }

            grid.addTwoStateEventColumn("AccountWideFavourite", (cFieldColumn)grid.getColumnByName("AccountWideFavourite"), true, false, GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/star_blue.png", "javascript:SEL.Addresses.Address.ToggleAccountWideFavourite({AddressID});", "Click to remove from account-wide favourites.", "Remove account-wide favourite status", GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/star_clear.png", "javascript:SEL.Addresses.Address.ToggleAccountWideFavourite({AddressID});", "Click to set as an account-wide favourite.", "Set as an account-wide favourite");
            const string CleanseTooltip = "Click to remove all favourites, labels and recommended distances for this address.";
            grid.addEventColumn("Cleanse", GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/brush2.png", "javascript:SEL.Addresses.Address.Cleanse({AddressID});", CleanseTooltip, CleanseTooltip);

            grid.addFilter(new cFields(currentUser.AccountID).GetFieldByID(new Guid("57C6F118-85BC-412C-9B6F-EC97D1E9D44A")), ConditionType.Equals, new object[] { false }, null, ConditionJoiner.And); // Obsolete

            return grid.generateGrid();
        }

        /// <summary>
        /// Post grid-render method, called by cGridNew every time a grid's row is initialised
        /// </summary>
        /// <param name="row">The cGridNew row</param>
        /// <param name="gridInfo">A dictionary of objects, necessary for cGridNew but not used here</param>
        private void AddressesGridOnInitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo = null)
        {
            var creationMethod = (int)row.getCellByID("CreationMethod").Value;
            if (creationMethod != (int)Address.AddressCreationMethod.ManualByAdministrator && creationMethod != (int)Address.AddressCreationMethod.ManualByClaimant && creationMethod != (int)Address.AddressCreationMethod.ImplementationImportRoutine)
            {
                row.getCellByID("Archived").Value = "<div title=\"Only manually added addresses can be archived\"><img src=\"/shared/images/icons/folder_lock_disabled.png\" alt=\"Not archivable.\"></div>";
            }
        }

        /// <summary>
        /// Grid for account-wide labels
        /// </summary>
        /// <param name="currentUser">The current user object for account and employee</param>
        /// <param name="addressIdentifier">The address Identifier for the current address object</param>
        /// <returns>Grid strings for javascript and html</returns>
        private static string[] AccountWideLabelsGrid(ICurrentUser currentUser, int addressIdentifier)
        {
            cGridNew grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridAwabels", AddressLabels.GridSql)
            {
                KeyField = "AddressLabelID",
                EmptyText = "There are no Account-wide Labels to display.",
                editlink = "javascript:SEL.Addresses.Address.AccountWideLabels.Edit({AddressLabelID});",
                deletelink = "javascript:SEL.Addresses.Address.AccountWideLabels.Delete({AddressLabelID});",
                enableupdating = currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Addresses, true, false),
                enabledeleting = currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Addresses, true, false)
            };

            grid.getColumnByName("AddressLabelID").hidden = true;

            grid.addFilter(new cFields(currentUser.AccountID).GetFieldByID(new Guid("2BA5481D-219F-470C-8001-E079EAE4D76E")), ConditionType.Equals, new object[] { addressIdentifier }, null, ConditionJoiner.And); // AddressID
            grid.addFilter(new cFields(currentUser.AccountID).GetFieldByID(new Guid("7DC642BC-0421-460A-867F-62B5822CA77A")), ConditionType.DoesNotContainData, null, null, ConditionJoiner.And); // EmployeeID

            return grid.generateGrid();
        }

        #endregion
    }
}