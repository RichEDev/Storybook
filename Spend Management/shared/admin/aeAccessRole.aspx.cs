namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using AjaxControlToolkit;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Logic_Classes.Fields;
    using SpendManagementLibrary.Logic_Classes.Tables;

    /// <summary>
    /// Create or edit an access role
    /// </summary>
    public partial class aeAccessRole : System.Web.UI.Page
    {
        /// <summary>
        /// Access role currently being edited
        /// </summary>
        public int accessRoleID = 0;
        /// <summary>
        /// Maximum amount that can be claimed by claimants in expenses
        /// </summary>
        public string ClaimMaximumAmount;
        /// <summary>
        /// Minimum amount that can be claimed by claimants in expenses
        /// </summary>
        public string ClaimMinimumAmount;
        /// <summary>
        /// Indicates whether role can edit Project Code
        /// </summary>
        public string chkCanEditProjectCodesObj;
        /// <summary>
        /// Control ID for checkbox that indicates whether role can edit cost code
        /// </summary>
        public string chkCanEditCostCodesObj;
        /// <summary>
        /// Control ID for checkbox that indicates whether role can edit department
        /// </summary>
        public string chkCanEditDepartmentsObj;
     
        /// <summary>
        /// Control ID for checkbox that indicates whether role requires a Bank Account to claim Expenses
        /// </summary>
        public string chkMustHaveBankAccountObj;

        /// <summary>
        /// The exclusion type of an access role.
        /// </summary>
        public int ExclusionType;

        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.AccessRoles, true, true);
            cAccessRole reqAccessRole = null;
            var clsAccessRoles = new cAccessRoles(currentUser.AccountID, cAccounts.getConnectionString(currentUser.AccountID));

            Master.enablenavigation = false;
            List<cModule> lstModules = currentUser.Account.AccountModules();

            reqAccessRole = PopulateTheForm(currentUser,  clsAccessRoles, lstModules);

            var tabContainer = new TabContainer {ID = "tcElementAccess"};
            tabContainer.Style.Add(HtmlTextWriterStyle.MarginTop, "5px");

            var clsCustomEntities = new cCustomEntities(currentUser);

            CreateTheTabs(lstModules, currentUser, reqAccessRole, tabContainer, clsCustomEntities);

            PutTheElementsIntoTheTabsTables(currentUser, tabContainer, reqAccessRole, clsCustomEntities);

            if (tabContainer.Tabs.Count > 0)
            {
                tabContainer.Tabs[0].Style[HtmlTextWriterStyle.Visibility] = String.Empty; // Sets the SM tab as visible, not sure why this needs to set this but if it is not, no tabs are visible on page load
            }
            phElementAccessTabs.Controls.Add(tabContainer); // Adds the TabContainer to the asp page

            var sbJS = new StringBuilder();
            sbJS.Append("if (window.addEventListener) // W3C standard\n");
            sbJS.Append("{\n");
            sbJS.Append("\twindow.addEventListener('load', checkViewChecks, false);\n");
            sbJS.Append("}\n");
            sbJS.Append("else if (window.attachEvent) // Microsoft\n");
            sbJS.Append("{\n");
            sbJS.Append("\twindow.attachEvent('onload', checkViewChecks);\n");
            sbJS.Append("}\n");
            switch (currentUser.CurrentActiveModule)
            {
                case Modules.SpendManagement:
                case Modules.SmartDiligence:
                case Modules.contracts:
                    sbJS.Append("if(document.getElementById('reportAccessLevelDiv') != null) { document.getElementById('reportAccessLevelDiv').style.display = 'none'; }\n");
                    break;
                default:
                    litclaimantreportopt.Text = @" Data from employees they approve";
                    break;
            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AddChecksForPageLoad", sbJS.ToString(), true);
        }

        private cAccessRole PopulateTheForm(ICurrentUser currentUser, cAccessRolesBase clsAccessRoles,
            List<cModule> lstModules)
        {
            cAccessRole reqAccessRole = null;
            bool blnEditMode = false;
            if (Request.QueryString["accessRoleID"] != null && Request.QueryString["accessRoleID"] != "")
            {
                int.TryParse(Request.QueryString["accessRoleID"], out accessRoleID);

                if (accessRoleID == 0)
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }

                currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.AccessRoles, true, true);

                reqAccessRole = clsAccessRoles.GetAccessRoleByID(accessRoleID);

                if (reqAccessRole == null)
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                }
                else
                {
                    currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.AccessRoles, true, true);

                    Master.title = "Access Role: " + reqAccessRole.RoleName;
                    blnEditMode = true;
                    accessRoleID = reqAccessRole.RoleID;
                    txtRoleName.Text = reqAccessRole.RoleName;
                    txtDescription.Text = reqAccessRole.Description;

                    if (reqAccessRole.AccessLevel == AccessRoleLevel.AllData)
                    {
                        radReportAccessLevel_3.Checked = true;
                    }
                    else if (reqAccessRole.AccessLevel == AccessRoleLevel.EmployeesResponsibleFor)
                    {
                        radReportAccessLevel_1.Checked = true;
                    }
                    else if (reqAccessRole.AccessLevel == AccessRoleLevel.SelectedRoles)
                    {
                        radReportAccessLevel_2.Checked = true;
                    }


                    var apiLicenced = lstModules.Select(x => x.ModuleID == (int) SpendManagementElement.Api).Any();
                    var mobileLicenced =
                        lstModules.Select(x => x.ModuleID == (int) SpendManagementElement.MobileDevices).Any();

                    if (!apiLicenced && !mobileLicenced)
                    {
                        chkWebsite.Checked = true;
                        accessSpan.Visible = false;
                    }
                    else
                    {
                        if (reqAccessRole.AllowApiAccess || reqAccessRole.AllowMobileAccess ||
                            reqAccessRole.AllowWebsiteAccess)
                        {
                            lblWebsite.Text = "Website";
                            lblMobile.Text = "Mobile";
                            lblAPI.Text = "API";
                            lblWebsite.CssClass = string.Empty;
                            lblMobile.CssClass = string.Empty;
                            lblAPI.CssClass = string.Empty;
                        }

                        chkWebsite.Checked = reqAccessRole.AllowWebsiteAccess;

                        if (mobileLicenced)
                        {
                            chkMobile.Checked = reqAccessRole.AllowMobileAccess;
                        }
                        else
                        {
                            mobileDiv.Visible = false;
                        }


                        if (apiLicenced)
                        {
                            chkAPI.Checked = reqAccessRole.AllowApiAccess;
                        }
                        else
                        {
                            ApiDiv.Visible = false;
                        }
                    }
                }
            }
            else
            {
                currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.AccessRoles, true, true);

                Master.title = "Access Role: New";
                blnEditMode = false;
                switch (currentUser.CurrentActiveModule)
                {
                    case Modules.SpendManagement:
                    case Modules.SmartDiligence:
                    case Modules.contracts:
                        radReportAccessLevel_3.Checked = true;
                        break;
                    default:
                        radReportAccessLevel_1.Checked = true;
                        break;
                }
            }

            switch (currentUser.CurrentActiveModule)
            {
                case Modules.contracts:
                    Master.helpid = blnEditMode ? 1149 : 1126;
                    break;
                default:
                    Master.helpid = 1054;
                    break;
            }

            Master.PageSubTitle = "Access Role Details";

            var sbCheckbox = new StringBuilder();

            foreach (cAccessRole accessRole in clsAccessRoles.AccessRoles.Values)
            {
                if (reqAccessRole == null || (reqAccessRole.RoleID != accessRole.RoleID))
                {
                    sbCheckbox.AppendFormat("<input type=\"checkbox\" value=\"on\" id=\"linkedAccessRole{0}\"", accessRole.RoleID);

                    if (reqAccessRole != null)
                    {
                        if (reqAccessRole.AccessRoleLinks.Count > 0)
                        {
                            if (reqAccessRole.AccessRoleLinks.Contains(accessRole.RoleID))
                            {
                                sbCheckbox.Append(" checked=\"checked\"");
                            }
                        }
                    }

                    sbCheckbox.AppendFormat("><label for=\"linkedAccessRole{0}\" id=\"lblLinkedAccessRole{0}\">{1}</label><br />", accessRole.RoleID, accessRole.RoleName);
                }
            }

            if (clsAccessRoles.AccessRoles.Count - 1 > 15)
            {
                lnkAccessRoles.Style.Add(HtmlTextWriterStyle.Overflow, "auto");
                lnkAccessRoles.Style.Add(HtmlTextWriterStyle.Height, "300px");
            }

            litLinkedAccessRoles.Text = sbCheckbox.ToString();

            return reqAccessRole;
        }

        private void CreateTheTabs(IEnumerable<cModule> lstModules, CurrentUser currentUser, cAccessRole reqAccessRole, TabContainer tabContainer, cCustomEntities clsCustomEntities)
        {
            foreach (cModule module in lstModules)
            {
                var tbPanel = new TabPanel
                {
                    ID = "tb" + module.ModuleID.ToString(),
                    HeaderText = ((module.ModuleID == (int) Modules.Greenlight ||
                                   module.ModuleID == (int) Modules.GreenlightWorkforce) &&
                                  (module.ModuleID == (int) currentUser.CurrentActiveModule))
                        ? "General"
                        : module.ModuleName
                };

               
  
                SetAdditionalAccessRoleFieldsOnTheirRelevantTab(module, tbPanel, reqAccessRole, currentUser.Account.HasLicensedElement(SpendManagementElement.BankAccounts));

                CreateElementTablesForThisTab(tbPanel, module);

                // add the tab to the tab container
                tabContainer.Tabs.Add(tbPanel);
            }

            CreateCustomEntitiesTab(tabContainer, clsCustomEntities);
            CreateReportingFieldsTab(tabContainer, reqAccessRole?.ExclusionType ?? 1, clsCustomEntities, currentUser);
        }

        private static void PutTheElementsIntoTheTabsTables(CurrentUser currentUser, TabContainer tabContainer,
            cAccessRole reqAccessRole, cCustomEntities clsCustomEntities)
        {
            TableRow row;
            const string nonBreakingSpace = @"&nbsp;";
            var rowCss = "row1";

            var clsModules = new cModules();

            //Literal litSpace;

            foreach (cModule module in currentUser.Account.AccountModules()) // loop through all modules in list
            {
                var tbPanel = tabContainer.FindControl("tb" + module.ModuleID.ToString()) as TabPanel; // get the modules tab panel
                if (tbPanel == null)
                {
                    // if panel not present, then don't output options for that module
                    continue;
                }
                var tbl = tbPanel.FindControl("tbl_" + module.ModuleID.ToString()) as Table; // get the modules table

                if (tbl != null)
                {
                    row = new TableRow();
                    CheckBox chk;
                    AddModuleRow(row, module.ModuleName, module.ModuleID.ToString());
                    tbl.Rows.Add(row);

                    Dictionary<cElementCategory, SortedList<string, cElement>> lstSortedCategories =
                        clsModules.GetCategoryElements(currentUser.AccountID, module.ModuleID);

                    foreach (KeyValuePair<cElementCategory, SortedList<string, cElement>> kvpElements in lstSortedCategories)
                        // loop through all the current modules categories
                    {
                        // add a new catagory line to the table
                        var tmpElementCat = (cElementCategory) kvpElements.Key;

                        row = AddCategoryRow(tmpElementCat.ElementCategoryID, tmpElementCat.ElementCategoryName);

                        tbl.Rows.Add(row);

                        foreach (KeyValuePair<string, cElement> kvpElement in lstSortedCategories[kvpElements.Key])
                            // loop through all the current categories elements
                        {
                            if (!kvpElement.Value.AccessRolesApplicable)
                            {
                                continue;
                            }

                            var element = (cElement) kvpElement.Value;
                            if (tbPanel.FindControl("cat_child_" + element.ElementCategoryID + "_" + element.ElementID) == null)
                            {
                                row = new TableRow
                                {
                                    ID = "cat_child_" + element.ElementCategoryID + "_" + element.ElementID
                                };

                                rowCss = rowCss == "row1" ? "row2" : "row1";

                                row.CssClass = rowCss;

                                var cell = new TableCell {Text = element.FriendlyName};
                                row.Cells.Add(cell);
                                var checkedValue = false;

                                if (reqAccessRole != null)
                                {
                                    if (reqAccessRole.ElementAccess.ContainsKey((SpendManagementElement) element.ElementID) ==
                                        true &&
                                        reqAccessRole.ElementAccess[(SpendManagementElement) element.ElementID].CanView == true)
                                    {
                                        checkedValue = true;
                                    }
                                }
                                CreateCheckBox(row, element.ElementID.ToString(), "View", "element", checkedValue);

                                checkedValue = false;
                                if (element.AccessRolesCanAdd == true)
                                {
                                    if (reqAccessRole != null)
                                    {
                                        if (reqAccessRole.ElementAccess.ContainsKey((SpendManagementElement)element.ElementID) ==
                                            true &&
                                            reqAccessRole.ElementAccess[(SpendManagementElement)element.ElementID].CanAdd == true)
                                        {
                                            checkedValue = true;
                                        }
                                    }
                                    CreateCheckBox(row, element.ElementID.ToString(), "Add", "element", checkedValue);
                                }
                                else
                                {
                                    cell = new TableCell
                                    {
                                        HorizontalAlign = HorizontalAlign.Center,
                                        Text = nonBreakingSpace
                                    };
                                    row.Cells.Add(cell);
                                }
                                

                                checkedValue = false;
                                if (element.AccessRolesCanEdit == true)
                                {
                                    if (reqAccessRole != null)
                                    {
                                        if (reqAccessRole.ElementAccess.ContainsKey((SpendManagementElement)element.ElementID) ==
                                            true &&
                                            reqAccessRole.ElementAccess[(SpendManagementElement)element.ElementID].CanEdit == true)
                                        {
                                            checkedValue = true;
                                        }
                                    }
                                    CreateCheckBox(row, element.ElementID.ToString(), "Edit", "element", checkedValue);
                                }
                                else
                                {
                                    cell = new TableCell
                                    {
                                        HorizontalAlign = HorizontalAlign.Center,
                                        Text = nonBreakingSpace
                                    };
                                    row.Cells.Add(cell);
                                }
                                
                                checkedValue = false;
                                if (element.AccessRolesCanDelete == true)
                                {
                                    if (reqAccessRole != null)
                                    {
                                        if (reqAccessRole.ElementAccess.ContainsKey((SpendManagementElement)element.ElementID) ==
                                            true &&
                                            reqAccessRole.ElementAccess[(SpendManagementElement)element.ElementID].CanDelete == true)
                                        {
                                            checkedValue = true;
                                        }
                                    }
                                    CreateCheckBox(row, element.ElementID.ToString(), "Delete", "element", checkedValue);
                                }
                                else
                                {
                                    cell = new TableCell
                                    {
                                        HorizontalAlign = HorizontalAlign.Center,
                                        Text = nonBreakingSpace
                                    };
                                    row.Cells.Add(cell);
                                }

                                tbl.Rows.Add(row);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Element: " + element.ElementID.ToString());
                                System.Diagnostics.Debug.WriteLine("Element Cat: " + element.ElementCategoryID.ToString());
                                System.Diagnostics.Debug.WriteLine("Module: " + module.ModuleID.ToString());
                            }
                        }
                    }
                }
            }

            #region custom entities

            if (clsCustomEntities.CustomEntities.Count > 0)
            {
                var tbPanel = tabContainer.FindControl("tbCustomEntities") as TabPanel;
                if (tbPanel != null)
                {
                    var tbl =  tbPanel.FindControl("tbl_CustomEntities") as Table;

                    if (tbl != null)
                    {
                        #region Custom Entities Row

                        row = new TableRow();

                        AddModuleRow(row, "GreenLights", "customentities");

                    
                        tbl.Rows.Add(row);

                        #endregion Custom Entities Row

                        var entityPermissions = (from y in clsCustomEntities.CustomEntities.Values
                            where y.IsSystemView == false && y.Views.Count > 0
                            select y).ToList();
                        foreach (var entityRecord in entityPermissions)
                        {
                            row = AddCategoryRow(entityRecord.entityid, entityRecord.entityname);
                            tbl.Rows.Add(row);
                            foreach (var a in entityRecord.Views.Values)
                            {
                                rowCss = AddElementRow(currentUser, reqAccessRole, tbPanel, entityRecord, a, rowCss, tbl);
                            }
                        }
                    }
                }
            }

            #endregion custom entities
        }

        private static TableRow AddCategoryRow( int id, string name)
        {
            var row = new TableRow {ID = "cat_parent_" + id + "_"};
            var cell = new TableCell
            {
                Text =
                    string.Format(
                        "<span style=\"cursor: pointer;\" onclick=\"ChangeCategoryDisplay({0});\"><img src=\"/shared/images/buttons/close.gif\" height=\"9\" width=\"9\" id=\"cat_parent_{0}_expand_image\" /> <b>{1}</b></span>",
                        id, name)
            };
            row.Cells.Add(cell);

            CreateCheckBox(row, id.ToString(), "View", "cat");
            CreateCheckBox(row, id.ToString(), "Add", "cat");
            CreateCheckBox(row, id.ToString(), "Edit", "cat");
            CreateCheckBox(row, id.ToString(), "Delete", "cat");
            
            return row;
        }

        private static void AddModuleRow(TableRow row, string moduleName, string moduleId)
        {
            var cell = new TableCell {Width = new Unit("440"), Text = moduleName};
            row.Cells.Add(cell);
            CreateCheckBox(row, moduleId, "View", "module");
            CreateCheckBox(row, moduleId, "Add", "module");
            CreateCheckBox(row, moduleId, "Edit", "module");
            CreateCheckBox(row, moduleId, "Delete", "module");
        }

        private static void CreateCheckBox(TableRow row, string moduleId, string accessType,string idLabel, bool checkedValue = false)
        {
            var cell = new TableCell {HorizontalAlign = HorizontalAlign.Center};
            var chk = new CheckBox {ID = string.Format("chk_{0}_{1}_{2}", idLabel, accessType.ToLower(), moduleId), Checked = checkedValue};
            switch (idLabel)
            {
                case "cat":
                    chk.Attributes.Add("onclick", string.Format("updateCategoryElementCheckboxes(this, '{0}', '{1}');", accessType, moduleId));
                    break;
                case "element":
                    chk.Attributes.Add("onclick", string.Format("updateElementCheckboxes(this, '{0}');", accessType));
                    break;
                default:
                    chk.Attributes.Add("onclick", string.Format("updateModuleElementCheckboxes(this, '{0}');", accessType));
                    break;
            }
            
            cell.Controls.Add(chk);
            row.Cells.Add(cell);
        }

        private static string AddElementRow(ICurrentUserBase currentUser, cAccessRole reqAccessRole, TabPanel tbPanel,
            cCustomEntity entityRecord, cCustomEntityView a, string rowCss, Table tbl)
        {
            #region Element Row

            if (tbPanel.FindControl("cev_cat_child_" + entityRecord.entityid + "_" + a.viewid) == null)
            {
                var row = new TableRow {ID = "cev_cat_child_" + entityRecord.entityid + "_" + a.viewid};

                rowCss = rowCss == "row1" ? "row2" : "row1";

                row.CssClass = rowCss;

                var cell = new TableCell {Text = a.viewname};
                row.Cells.Add(cell);

                cell = new TableCell {HorizontalAlign = HorizontalAlign.Center};
                var chk = new CheckBox {ID = "cev_chk_element_view_" + a.viewid};

                if (reqAccessRole != null &&
                    reqAccessRole.CustomEntityAccess.ContainsKey(entityRecord.entityid) == true)
                {
                    if (
                        reqAccessRole.CustomEntityAccess[entityRecord.entityid].ViewAccess.ContainsKey(a.viewid) ==
                        true &&
                        reqAccessRole.CustomEntityAccess[entityRecord.entityid].ViewAccess[a.viewid].CanView ==
                        true)
                    {
                        chk.Checked = true;
                    }
                }

                chk.Attributes.Add("onclick", "updateElementCheckboxes(this, 'View');");
                cell.Controls.Add(chk);
                row.Cells.Add(cell);


                cell = new TableCell {HorizontalAlign = HorizontalAlign.Center};

                chk = new CheckBox();
                chk.ID = "cev_chk_element_add_" + a.viewid;

                if (reqAccessRole != null &&
                    reqAccessRole.CustomEntityAccess.ContainsKey(entityRecord.entityid) == true)
                {
                    if (
                        reqAccessRole.CustomEntityAccess[entityRecord.entityid].ViewAccess.ContainsKey(a.viewid) ==
                        true &&
                        reqAccessRole.CustomEntityAccess[entityRecord.entityid].ViewAccess[a.viewid].CanAdd ==
                        true)
                    {
                        chk.Checked = true;
                    }
                }

                chk.Attributes.Add("onclick", "updateElementCheckboxes(this, 'Add');");
                cell.Controls.Add(chk);

                row.Cells.Add(cell);

                cell = new TableCell {HorizontalAlign = HorizontalAlign.Center};

                chk = new CheckBox {ID = "cev_chk_element_edit_" + a.viewid};

                if (reqAccessRole != null &&
                    reqAccessRole.CustomEntityAccess.ContainsKey(entityRecord.entityid) == true)
                {
                    if (
                        reqAccessRole.CustomEntityAccess[entityRecord.entityid].ViewAccess.ContainsKey(a.viewid) ==
                        true &&
                        reqAccessRole.CustomEntityAccess[entityRecord.entityid].ViewAccess[a.viewid].CanEdit ==
                        true)
                    {
                        chk.Checked = true;
                    }
                }

                chk.Attributes.Add("onclick", "updateElementCheckboxes(this, 'Edit');");
                cell.Controls.Add(chk);
                row.Cells.Add(cell);

                cell = new TableCell {HorizontalAlign = HorizontalAlign.Center};
                chk = new CheckBox {ID = "cev_chk_element_delete_" + a.viewid};

                if (reqAccessRole != null &&
                    reqAccessRole.CustomEntityAccess.ContainsKey(entityRecord.entityid) == true)
                {
                    if (
                        reqAccessRole.CustomEntityAccess[entityRecord.entityid].ViewAccess.ContainsKey(a.viewid) ==
                        true &&
                        reqAccessRole.CustomEntityAccess[entityRecord.entityid].ViewAccess[a.viewid].CanDelete ==
                        true)
                    {
                        chk.Checked = true;
                    }
                }

                chk.Attributes.Add("onclick", "updateElementCheckboxes(this, 'Delete');");
                cell.Controls.Add(chk);
                row.Cells.Add(cell);
                tbl.Rows.Add(row);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Element: " + a.viewid);
                System.Diagnostics.Debug.WriteLine("Element Cat: " + entityRecord.entityid);
                System.Diagnostics.Debug.WriteLine("Module: {0}",
                    currentUser.CurrentActiveModule == Modules.GreenlightWorkforce
                        ? "GreenLight Workforce"
                        : "Greenlight");
            }

            #endregion Element Row

            return rowCss;
        }

        private static void CreateCustomEntitiesTab(TabContainer tabContainer, cCustomEntities clsCustomEntities)
        {
            if (clsCustomEntities.CustomEntities.Count > 0)
            {
                var tbPanel = new TabPanel {ID = "tbCustomEntities", HeaderText = "GreenLights"};

                var pnl = new Panel {CssClass = "sectiontitle"};
                CreateLiteral(pnl, "Element Access");
                
                tbPanel.Controls.Add(pnl);

                // create a table for each tab
                CreateTable(tbPanel, "tbl_CustomEntities");

                // add the tab to the tab container
                tabContainer.Tabs.Add(tbPanel);
            }
        }

        /// <summary>
        /// The create reporting fields tab.
        /// </summary>
        /// <param name="tabContainer">
        /// The tab container.
        /// </param>
        /// <param name="exclusionType">
        /// The exclusion Type.
        /// </param>      
        /// <param name="customEntities">
        /// The custom entity object.
        /// </param>
        /// <param name="reqCurrentUser">
        /// The current user object.
        /// </param>
        private void CreateReportingFieldsTab(TabContainer tabContainer, int exclusionType, cCustomEntities customEntities, CurrentUser reqCurrentUser)
        {
            var tbPanel = new TabPanel { ID = "tbReportingFields", HeaderText = "Reportable Fields" };

            // create a table for each tab
            this.LoadReportingFields(tbPanel, exclusionType, customEntities, reqCurrentUser);

             // add the tab to the tab container
             tabContainer.Tabs.Add(tbPanel);
        }

        /// <summary>
        /// The load reporting fields.
        /// </summary>
        /// <param name="tabPanel">
        /// The tab Panel.
        /// </param>
        /// <param name="exclusionType">
        /// The exclusion type of an accessrole.
        /// </param>
        /// <param name="customEntities">
        /// The custom entity object.
        /// </param>
        /// <param name="reqCurrentUser">
        /// The current user object.
        /// </param>
        private void LoadReportingFields(TabPanel tabPanel, int exclusionType, cCustomEntities customEntities, CurrentUser reqCurrentUser)
        {
            var pnl = new Panel { CssClass = "sectiontitle" };
            CreateLiteral(pnl, "Element Access");
            tabPanel.Controls.Add(pnl);

            pnl = new Panel { CssClass = "twocolumn" };
            CreateLabel(pnl, "lblExclusionType", "ddlExclusionType", "Exclusion Type");
            CreateLiteral(pnl, "<span class=\"inputs\">");
            var exclusionTypes = new List<ListItem>();
            exclusionTypes.Add(new ListItem("Access role can see all data", "2"));
            exclusionTypes.Add(new ListItem("Include selected below", "1"));
            exclusionTypes.Add(new ListItem("Exclude selected below", "0"));
            CreateDropDown(pnl, "ddlExclusionType", exclusionTypes, exclusionType.ToString());
            this.ExclusionType = exclusionType;
            CreateLiteral(pnl, "</span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\">&nbsp;</span>");
            var img = new Image();
            img.ImageUrl = "~/shared/images/icons/16/plain/tooltip.png";
            img.ID = "imgExclusionTypeTooltipr";
            img.CssClass = "tooltipicon";
            img.Attributes.Add("onmouseover", "SEL.Tooltip.Show('E2B1F578-5E02-45D4-9662-0C811F3BDB50', 'ex', this);");
            pnl.Controls.Add(img);
            CreateLiteral(pnl, "</span><span class=\"inputvalidatorfield\">&nbsp;</span>");
            tabPanel.Controls.Add(pnl);

            pnl = new Panel { CssClass = "twocolumn reportableTables" };
            var listItemFactory = new ListItemFactory(reqCurrentUser, customEntities);
            var accountProperties = new cAccountSubAccounts(reqCurrentUser.AccountID).getSubAccountById(reqCurrentUser.CurrentSubAccountId).SubAccountProperties;
            var tables = new SubAccountTables(new cTables(reqCurrentUser.AccountID), new TableRelabler(accountProperties));
            CreateLabel(pnl, "lblProductArea", "ddlProductArea", "Product Area");
            CreateLiteral(pnl, "<span class=\"inputs\">");
            CreateDropDown(pnl, "ddlProductArea", listItemFactory.CreateList(tables.GetReportableTables(reqCurrentUser.CurrentActiveModule)));
            CreateLiteral(pnl, "</span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\">&nbsp;</span>");
            img = new Image();
            img.ImageUrl = "~/shared/images/icons/16/plain/tooltip.png";
            img.ID = "ImpProductAreaTooltip";
            img.CssClass = "tooltipicon";
            img.Attributes.Add("onmouseover", "SEL.Tooltip.Show('C1E48BFB-65CB-474A-B2B5-446985BD5DB7', 'ex', this);");
            pnl.Controls.Add(img);
            CreateLiteral(pnl, "</span><span class=\"inputvalidatorfield\">&nbsp;</span>");
            tabPanel.Controls.Add(pnl);

            pnl = new Panel { CssClass = "reportingFields" };
            tabPanel.Controls.Add(pnl);
        }

        private void SetAdditionalAccessRoleFieldsOnTheirRelevantTab(cModule module, TabPanel tbPanel,
             cAccessRole reqAccessRole, bool hasBankAccountLicensedElement)
        {
            switch ((Modules) module.ModuleID)
            {
                case Modules.expenses:
                case Modules.Greenlight:
                case Modules.GreenlightWorkforce:
                    var pnl = new Panel {CssClass = "sectiontitle"};
                    CreateLiteral(pnl, "General Options");
                    tbPanel.Controls.Add(pnl);
                    pnl = new Panel {CssClass = "twocolumn"};
                    CreateLabel(pnl, "lblCanEditProjectCodes", "chkCanEditProjectCodes", "Can edit project code");
                    CreateLiteral(pnl, "<span class=\"inputs\">");
                    this.chkCanEditProjectCodesObj = CreateCheckbox(pnl, "chkCanEditProjectCodes",  "fillspan", reqAccessRole != null && reqAccessRole.CanEditProjectCode);
                    CreateLiteral(pnl, "</span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\">&nbsp;</span>");
                    CreateLabel(pnl, "lblCanEditCostCodes", "chkCanEditCostCodes", "Can edit cost code");
                    CreateLiteral(pnl, "<span class=\"inputs\">");
                    this.chkCanEditCostCodesObj = CreateCheckbox(pnl, "chkCanEditCostCodes", "fillspan",
                        reqAccessRole != null && reqAccessRole.CanEditCostCode);
                    CreateLiteral(pnl, "</span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\">&nbsp;</span>");
                    tbPanel.Controls.Add(pnl);
                    // chkCanEditDepartments
                    pnl = new Panel {CssClass = "twocolumn"};
                    CreateLabel(pnl, "lblCanEditDepartments", "chkCanEditDepartments", "Can edit department");
                    CreateLiteral(pnl,"<span class=\"inputs\">");
                    this.chkCanEditDepartmentsObj = CreateCheckbox(pnl, "chkCanEditDepartments", "fillspan",
                        reqAccessRole != null && reqAccessRole.CanEditDepartment);
                    CreateLiteral(pnl, "</span>");

                    if ((Modules) module.ModuleID == Modules.expenses)
                    {
                        CreateLiteral(pnl,
                            "<span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\">&nbsp;</span>");
                        tbPanel.Controls.Add(pnl);
                        pnl = new Panel {CssClass = "twocolumn"};
                        CreateLabel(pnl,  "lblClaimMaximumAmount","txtClaimMaximumAmount", "Maximum claim amount");
                        CreateLiteral(pnl,"<span class=\"inputs\">");
                        ClaimMaximumAmount = CreateTextBox(pnl, "txtClaimMaximumAmount", reqAccessRole == null ? string.Empty : reqAccessRole.ExpenseClaimMaximumAmount.ToString());
                        CreateLiteral(pnl,
                            "</span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\">&nbsp;</span>");
                        CreateLabel(pnl, "lblClaimMinimumAmount", "txtClaimMinimumAmount", "Minimum claim amount");
                        CreateLiteral(pnl, "<span class=\"inputs\">");
                        ClaimMinimumAmount = CreateTextBox(pnl, "txtClaimMinimumAmount", reqAccessRole == null ? string.Empty : reqAccessRole.ExpenseClaimMinimumAmount.ToString());
                        CreateLiteral(pnl, "</span><span class=\"inputicon\">&nbsp;</span><span class=\"inputtooltipfield\">&nbsp;</span><span class=\"inputvalidatorfield\">&nbsp;</span>");
                        tbPanel.Controls.Add(pnl);

                        if (hasBankAccountLicensedElement)
                        {
                            pnl = new Panel { CssClass = "twocolumn" };
                            CreateLabel(pnl, "lblMustHaveBankAccount", "chkMustHaveBankAccount", "Employee must have at least one active bank account to claim expenses");
                            CreateLiteral(pnl, "<span class=\"inputs\">");
                            this.chkMustHaveBankAccountObj = CreateCheckbox(pnl, "chkMustHaveBankAccount", "fillspan", reqAccessRole != null && reqAccessRole.MustHaveBankAccount);
                            CreateLiteral(pnl, "</span>");
                            tbPanel.Controls.Add(pnl);
                        }
                    }
               
                    break;

                case Modules.contracts:
                    break;

                default:
                    break;
            }
        }

        private static string CreateTextBox(Panel pnl,  string id, string text)
        {
            var txt = new TextBox {ID = id, CssClass = "fillspan", Text = text};
            pnl.Controls.Add(txt);
            return txt.ClientID;
        }

        /// <summary>
        /// The create drop down controls on the page.
        /// </summary>
        /// <param name="pnl">
        /// The panel control.
        /// </param>
        /// <param name="id">
        /// The id of the dropdown to be created.
        /// </param>
        /// <param name="text">
        /// The text of the dropdown values.
        /// </param>
        /// <param name="defaultValue">
        /// The default value selected for a dropdown.
        /// </param>
        private static void CreateDropDown(Panel pnl, string id, List<ListItem> text, string defaultValue = null)
        {
            var ddl = new DropDownList { ID = id, CssClass = id };
            foreach (ListItem name in text)
            {
                ddl.Items.Add(name);
            }
            if (defaultValue != null)
            {
                ddl.SelectedValue = defaultValue;
            }
            
            pnl.Controls.Add(ddl);
        }


        private static string CreateCheckbox(Panel pnl, string id, string cssClass, bool itemChecked)
        {
            var checkBox = new CheckBox {ID = id, CssClass = cssClass, Checked = itemChecked, ClientIDMode = ClientIDMode.AutoID};
            
            pnl.Controls.Add(checkBox);
            return checkBox.ClientID;
        }

        private static void CreateLabel(Panel pnl, string id, string associatedControlId, string text)
        {
            var lbl = new Label
            {
                ID = id,
                AssociatedControlID = associatedControlId,
                Text = text
            };
            pnl.Controls.Add(lbl);
        }

        private static void CreateLiteral(Panel pnl, string text)
        {
            var lit = new Literal {Text = text};
            pnl.Controls.Add(lit);
        }

        private static void CreateElementTablesForThisTab(TabPanel tbPanel, cModule module)
        {
            var pnl = new Panel {CssClass = "sectiontitle"};
            CreateLiteral(pnl, "Element Access");
            
            tbPanel.Controls.Add(pnl);

            // create a table for each tab
            CreateTable(tbPanel, "tbl_" + module.ModuleID);
        }

        private static void CreateTable(TabPanel tbPanel, string id)
        {
            var tbl = new Table {CssClass = "cGrid", ID = id};

            var headerRow = new TableHeaderRow();
            var headerCell = new TableHeaderCell {Text = "Element"};
            headerRow.Cells.Add(headerCell);

            headerCell = new TableHeaderCell {Text = "View"};
            headerRow.Cells.Add(headerCell);

            headerCell = new TableHeaderCell {Text = "Add"};
            headerRow.Cells.Add(headerCell);

            headerCell = new TableHeaderCell {Text = "Edit"};
            headerRow.Cells.Add(headerCell);

            headerCell = new TableHeaderCell {Text = "Delete"};
            headerRow.Cells.Add(headerCell);

            tbl.Rows.Add(headerRow);

            // add the table to the current tab
            tbPanel.Controls.Add(tbl);
        }
    }
}
