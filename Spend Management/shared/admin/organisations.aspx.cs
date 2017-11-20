using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Spend_Management.shared.admin
{
    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Helpers;

    public partial class organisations : System.Web.UI.Page
    {
        /// <summary>
        /// Whether to force users to enter an address name before they can use the address, this option is only available to Address+ Standard (TeleAtlas) accounts
        /// </summary>
        public bool ForceAddressNameEntry { get; set; }

        /// <summary>
        /// The message presented to users when they are prompted to enter an address name
        /// </summary>
        public string AddressNameEntryMessage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            this.Master.Page.Title = "Organisations";
            this.Master.PageSubTitle = "Organisations";
            this.Master.UseDynamicCSS = true;

            #region Postback, add and view functionality

            if (this.IsPostBack)
            {
                return;
            }

            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Organisations, true, true);

            if (currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Organisations, true, false) == false)
            {
                this.lnkNewOrganisation.Style.Add(HtmlTextWriterStyle.Display, "none");
            }

            #endregion

            Guid organisationsTableId = new Guid("7BDAF84E-A373-4008-83D1-9E18AAA47F8E");
            List<object> parameters = AutoComplete.getAutoCompleteQueryParams("organisations");
            string organisationAutoComplete = AutoComplete.createAutoCompleteBindString("txtParentOrganisation", 15, organisationsTableId, (Guid)parameters[0], (List<Guid>)parameters[1]);

            #region Grid

            string gridHtml = string.Empty;
            string gridJavaScript = string.Empty;

            cTable table = new cTables(currentUser.AccountID).GetTableByID(new Guid("7BDAF84E-A373-4008-83D1-9E18AAA47F8E"));
            cFields fields = new cFields(currentUser.AccountID);
            JoinVia joinVia = new JoinVia(0, "Organisation Primary Address", Guid.NewGuid(), new SortedList<int, JoinViaPart> { { 0, new JoinViaPart(new Guid("B74065BB-6D13-464F-92E8-D6A49AE2F65E"), JoinViaPart.IDType.Field) } });

            List<cNewGridColumn> columns = new List<cNewGridColumn>
            {
                new cFieldColumn(fields.GetFieldByID(new Guid("9325D7AB-70DE-4CF8-A6B5-B1AC46481169"))),
                new cFieldColumn(fields.GetFieldByID(new Guid("4D0F2409-0705-4F0F-9824-42057B25AEBE"))),
                new cFieldColumn(fields.GetFieldByID(new Guid("9BB72DFE-CAFD-459C-AF10-97FFCFA1B06F"))),
                new cFieldColumn(fields.GetFieldByID(new Guid("0D08813C-00E3-45EB-BDCC-0EA512615ADE"))),
                new cFieldColumn(fields.GetFieldByID(new Guid("AC87C4C4-9107-4555-B2A3-27109B3EBFBB"))),
                new cFieldColumn(fields.GetFieldByID(new Guid("4B7873D6-8EDC-44D4-94F7-B8ABCBD87692"))),
                new cFieldColumn(fields.GetFieldByID(new Guid("25597581-F3C5-40F0-B7BC-53FB67AC1A0E")), joinVia),
                new cFieldColumn(fields.GetFieldByID(new Guid("B90920FE-BEA1-4948-A26B-CB997136C9F4")), joinVia),
                new cFieldColumn(fields.GetFieldByID(new Guid("5A999C9F-440E-4D65-8B3C-187BA69E0234")), joinVia)
            };

            var grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridOrganisations", table, columns)
            {
                KeyField = "OrganisationID",
                ArchiveField = "IsArchived",
                EmptyText = "There are no Organisations to display.",
                archivelink = "javascript:SEL.Organisations.Organisation.ToggleArchive({OrganisationID});",
                editlink = "javascript:SEL.Organisations.Organisation.Edit({OrganisationID});",
                deletelink = "javascript:SEL.Organisations.Organisation.Delete({OrganisationID});",
                enablearchiving = currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Organisations, true, false),
                enableupdating = currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Organisations, true, false),
                enabledeleting = currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Organisations, true, false)
            };

            grid.getColumnByName("OrganisationID").hidden = true;
            grid.getColumnByName("IsArchived").hidden = true;
            grid.getColumnByName("ParentOrganisationID").hidden = true;

            string[] gridStrings = grid.generateGrid();

            if (gridStrings.Length == 2)
            {
                gridJavaScript = gridStrings[0];
                gridHtml = gridStrings[1];
            }

            #endregion Grid

            this.phOrganisationsGrid.Controls.Add(new Literal
            {
                Text = gridHtml
            });

            this.ClientScript.RegisterStartupScript(this.GetType(), "organisationsGridJavaScript", cGridNew.generateJS_init("organisationsGridJavaScript", new List<string> { gridJavaScript }, currentUser.CurrentActiveModule), true);
            this.ClientScript.RegisterStartupScript(this.GetType(), "parentOrganisationJavaScript", AutoComplete.generateScriptRegisterBlock(new List<string> { organisationAutoComplete }), true);
        }
    }
}