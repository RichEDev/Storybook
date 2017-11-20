using System.Globalization;

namespace Spend_Management.shared.usercontrols
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using SpendManagementLibrary;
    using code;

    /// <summary>
    /// A widget for finding and selecting claims.
    /// </summary>
    public partial class ClaimSelection : UserControl
    {
        /// <summary>
        /// Whether to include the selectable column in the results.
        /// Note that this will also remove the cancel button, since there should be one in the parent.
        /// </summary>
        public bool Selectable { get; set; }

        private ScriptManager _scriptManager;
        
        protected override void OnInit(EventArgs e)
        {
            ClientIDMode = ClientIDMode.Static;

            _scriptManager = ScriptManager.GetCurrent(Page);
            if (_scriptManager == null)
            {
                _scriptManager = new ScriptManager();
                Page.Form.Controls.AddAt(0, _scriptManager);
            }

            base.OnInit(e);
        }

        /// <summary>
        /// Happens on page load.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            _scriptManager.Scripts.Add(new ScriptReference("~/shared/javaScript/sel.claims.js"));
            _scriptManager.Scripts.Add(new ScriptReference("~/shared/javaScript/minify/sel.claimSelector.js"));
            _scriptManager.Scripts.Add(new ScriptReference("~/shared/javaScript/minify/sel.selectinator.js"));
            _scriptManager.Services.Add(new ServiceReference("~/shared/webServices/svcClaim.asmx"));
            //_scriptManager.Scripts.Add(new ScriptReference("tooltips"));

            const string javascriptVariableConditionType = "All";
            var user = ((IRequireCurrentUser)Page).CurrentUser;
            var employeeTableGuid = new Guid("618DB425-F430-4660-9525-EBAB444ED754");
            var employeeEmailField = new Guid("0F951C3E-29D1-49F0-AC13-4CFCABF21FDA");
            var getEmployeeFirstnameSurnameByIdFunctionField = new Guid("142EA1B4-7E52-4085-BAAA-9C939F02EB77");
            var tables = new cTables(user.AccountID);
            var matchfields = string.Format("{0},{1}", getEmployeeFirstnameSurnameByIdFunctionField, employeeEmailField);
            var javascriptFilters = FieldFilterGenerator.GetHierarchyJsFieldFilters(user);
            var filters = FieldFilterGenerator.GetHeirarchyFieldFilters(user);
            var claimants = AutoComplete.GetAutoCompleteMatches(user, 0, employeeTableGuid.ToString(), getEmployeeFirstnameSurnameByIdFunctionField.ToString(), matchfields, string.Empty, true, javascriptFilters);
            var lookup = new Guid("96f11c6d-7615-4abd-94ec-0e4d34e187a0");
            var extendedCliaimHierarchy = FieldFilters.GetClaimHeirachyList(lookup, user.EmployeeID, user, true);
            var clsclaims = new cClaims(user.AccountID); 

            foreach (var employeeId in extendedCliaimHierarchy.Where(employeeId => clsclaims.GetClaimsToInclude(user, employeeId).Count == 0))
            {
                //remove claimant as they are either no using a costcode owner as a sign off stage
                //or none of the expense items belonging to claimant use the costcode that the viewer owns.
                claimants.RemoveAll(x => x.value == employeeId.ToString(CultureInfo.InvariantCulture));
            }

            var highClaimantCount = false;
            if (claimants.Count <= 25)
            {
                ClaimantText.Style.Add(HtmlTextWriterStyle.Display, "none");
                ClaimantTextSearchIcon.Style.Add(HtmlTextWriterStyle.Display, "none");
                ClaimantCombo.Items.Add(new ListItem("[None]", "0"));
                ClaimantCombo.Items.AddRange(claimants.Select(x => new ListItem(x.label, x.value)).ToArray());
            }
            else
            {
                highClaimantCount = true;
                ClaimantCombo.Style.Add(HtmlTextWriterStyle.Display, "none");
                ClaimantTextSearchIcon.ImageUrl = GlobalVariables.StaticContentLibrary + "/icons/16/new-icons/find.png";
                var parameters = AutoComplete.getAutoCompleteQueryParams("employeesWithUsername");
                var autocompleteBinding = AutoComplete.createAutoCompleteBindString("ClaimantText", 15, tables.GetTableByID(employeeTableGuid).TableID, (Guid)parameters[0], (List<Guid>)parameters[1], fieldFilters: filters);

                var js = new StringBuilder();
                js.Append("SEL.ClaimSelector.ClaimantSearchPanel = '" + ClaimantSearchPanel.ClientID + "';\n");
                js.Append("SEL.ClaimSelector.ClaimantSearchModal = '" + ClaimantSearchModal.ClientID + "';\n");
                js.Append("SEL.ClaimSelector.ClaimantCombo = AutoCompleteSearches.New('Claimant', SEL.ClaimSelector.DomIDs.ClaimantText, SEL.ClaimSelector.ClaimantSearchModal, SEL.ClaimSelector.ClaimantSearchPanel);\n");
                Page.ClientScript.RegisterStartupScript(GetType(), "ClaimantJS", js.ToString(), true);
                Page.ClientScript.RegisterStartupScript(GetType(), "ClaimantAutoCompleteBind", AutoComplete.generateScriptRegisterBlock(new List<string> { autocompleteBinding }), true);
            }

            if (Selectable)
            {
                CloseButton.Visible = false;
            }

            var scriptVariables = new StringBuilder();
            scriptVariables.AppendFormat("SEL.ClaimSelector.HighClaimantCount = '{0}';\n", highClaimantCount ? "true" : "false");
            scriptVariables.AppendFormat("SEL.ClaimSelector.Selectable = {0};\n", Selectable ? "true" : "false");
            scriptVariables.AppendFormat("SEL.ClaimSelector.NumberOfClaimants = '{0}';\n", claimants.Count);
            scriptVariables.AppendFormat("SEL.ClaimSelector.ConditionType = '{0}';\n", javascriptVariableConditionType);
            scriptVariables.AppendFormat("SEL.ClaimSelector.DomIDs.ClaimName = '{0}';\n", ClaimNameText.ClientID);
            scriptVariables.AppendFormat("SEL.ClaimSelector.DomIDs.ClaimantId = '{0}';\n", ClaimantText_ID.ClientID);
            Page.ClientScript.RegisterStartupScript(GetType(), "claimselectorJS", "SEL.ClaimSelector.RootClaimSelector = 'true';", true);
            var previousUrl = (SiteMap.CurrentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;
            scriptVariables.Append(string.Format("var returnPage = '{0}'; ", previousUrl));
            Page.ClientScript.RegisterStartupScript(GetType(), "Variables", scriptVariables.ToString(), true);
        }
    }
}