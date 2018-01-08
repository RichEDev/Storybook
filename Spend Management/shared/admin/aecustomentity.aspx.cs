using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;

namespace Spend_Management
{
    using System.Globalization;

    using shared.usercontrols;
    using shared.admin;
    using shared.webServices;

    /// <summary>
    /// Add/Edit GreenLight page
    /// </summary>
    public partial class aecustomentity : System.Web.UI.Page
    {
        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            smProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/shortcut.js"));
            smProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery-ui.datepicker-en-gb.js"));
            smProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.ui.timepicker-0.3.2.js"));
            smProxy.Scripts.Add(new ScriptReference(GlobalVariables.StaticContentLibrary + "/js/jQuery/jquery.ui.multiselect.js"));

            Master.enablenavigation = false;

            Master.UseDynamicCSS = true;

            Master.helpid = 2222;

            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();

                if (user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CustomEntities, true) == false)
                {
                    Response.Redirect(
                    "~/shared/restricted.aspx?reason=Current%20access%20role%20does%20not%20permit%20you%20to%20view%20this%20page.",
                    true);
                }

                Session.Remove("dockableControls");
                Session.Remove("tabs");
                cmbattributetype.Attributes.Add("onchange", "showFurtherAttributeOptions(true);");
                cmbntoOnerelationshipentity.Attributes.Add("onchange", "SEL.CustomEntityAdministration.Attributes.Relationship.ManyToOne.GetLookupOptions();");
                cmbOnetonrelationshipentity.Attributes.Add("onchange", "SEL.CustomEntityAdministration.Attributes.Relationship.OneToMany.GetViews();");
                cmbsourceentity.Attributes.Add("onchange", "SEL.CustomEntityAdministration.Attributes.Summary.GetSummarySources();");
                cmbtextformat.Attributes.Add("onchange", "ShowDisplayWidthOptions('text', true)");
                chkAuditIdentifier.InputAttributes.Add("onclick", "return ConfirmAuditIdentifierChange();");
                cmbDisplayWidth.Attributes.Add("onchange", "FireDropdownValidatorWhenNoneSelected(cmbDisplayWidthID, reqDisplayWidth);");
                cmbdefaultvalue.Attributes.Add("onchange", "FireDropdownValidatorWhenNoneSelected(cmbdefaultvalueid, reqDefaultYesNo);");
                cmbdateformat.Attributes.Add("onchange", "FireDropdownValidatorWhenNoneSelected(cmbdateformatid, reqDateFormat);");
                cmbtextformatlarge.Attributes.Add("onchange", "FireDropdownValidatorWhenNoneSelected(cmbtextformatlargeid, reqLargeTextFormat);");
                ddlDefaultCurrency.Attributes.Add("onchange", "FireDropdownValidatorWhenNoneSelected(ddlDefaultCurrencyID, cvDefaultCurrencyID);");
                cmbmtodisplayfield.Attributes.Add("onchange", "FireDropdownValidatorWhenNoneSelected(SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.DisplayField, SEL.CustomEntityAdministration.DomIDs.Attributes.Relationship.NtoOne.Modal.Fields.DisplayFieldReqValidator);");
                
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
                int entityid = 0;
                if (Request.QueryString["entityid"] != null)
                {
                    if (!Int32.TryParse(Request.QueryString["entityid"], out entityid))
                    {
                        Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }
                }

                if (entityid < 0 )
                {
                    Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true); 
                }

                ViewState["entityid"] = entityid;

                var menuService = new SvcCustomMenu();
                this.menuTreeData.Value  = new CustomMenu().AddMenuNodesInWebTree(menuService);

                var disabledModules = user.Account.AccountModules();
                if (disabledModules.Count() > 1)
                {
                    this.MenuDisabledModulesRepeater.DataSource = disabledModules.OrderBy(module => module.ModuleID);
                    this.MenuDisabledModulesRepeater.DataBind();
                }

                List<Guid> excludeTables = new List<Guid>();

                // some common guids for use with Selectinators
                var employeesTableId = new Guid("618DB425-F430-4660-9525-EBAB444ED754");
                var employeeFullNameField = new Guid("142EA1B4-7E52-4085-BAAA-9C939F02EB77");
                var employeeEmailField = new Guid("0F951C3E-29D1-49F0-AC13-4CFCABF21FDA");

                var filters = new List<JSFieldFilter>();
                var selAdminFieldFilter = new JSFieldFilter()
                {
                    FieldID = new Guid("1c45b860-ddaa-47da-9eec-981f59cce795"),
                    ConditionType = ConditionType.NotLike,
                    ValueOne = "admin%"
                };
                filters.Add(selAdminFieldFilter);

                // Greenlight owner selectinator
                Selectinator selOwner = (Selectinator)Page.LoadControl("~/shared/usercontrols/Selectinator.ascx");
                selOwner.Filters = filters;
                selOwner.ID = "Owner";
                selOwner.Name = "Owner";
                selOwner.TableGuid = employeesTableId;
                selOwner.DisplayField = employeeFullNameField;
                selOwner.MatchFields = new List<Guid>()
                                           {
                                               employeeEmailField,
                                               employeeFullNameField
                                           };
                selOwner.IsEnabled = true;
                selOwner.Tooltip = "4680AE5A-0B69-4821-B4E5-EB10C0407F8D";
                selOwner.ValidationControlGroup = "vgMain";
                selOwner.Mandatory = false;
                pnlOwnerSelectinator.Controls.Add(selOwner);

                // Greenlight support contact selectinator
                Selectinator selSupportContact = (Selectinator)Page.LoadControl("~/shared/usercontrols/Selectinator.ascx");
                selSupportContact.Filters = filters;
                selSupportContact.ID = "SupportContact";
                selSupportContact.Name = "Support contact";
                selSupportContact.TableGuid = employeesTableId;
                selSupportContact.DisplayField = employeeFullNameField;
                selSupportContact.MatchFields = new List<Guid>()
                                           {
                                               employeeEmailField,
                                               employeeFullNameField
                                           };
                selSupportContact.IsEnabled = true;
                selSupportContact.Tooltip = "601132CC-74DE-455A-90FE-335AC335B3C5";
                selSupportContact.ValidationControlGroup = "vgMain";
                selSupportContact.Mandatory = false;
                pnlSupportQuestionSelectinator.Controls.Add(selSupportContact);

                // only adminonly users can create built-in/system greenlights
                if (user.Employee.AdminOverride)
                {
                    chkBuiltIn.Enabled = true;
                    chkFormBuiltIn.Enabled = true;
                    chkAttributeBuiltIn.Enabled = true;
                    chkOneToNRelationshipBuiltIn.Enabled = true;
                    chkNToOneRelationshipBuiltIn.Enabled = true;
                    chkViewBuiltIn.Enabled = true;
                }

                cCurrencies clsCurrencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
                cCustomEntities clsentities = new cCustomEntities(user);

                string[] attributeGridData;
                string[] formGridData;
                string[] viewGridData;

                if (entityid > 0)
                {
                    user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.CustomEntities, true, true);
                    cCustomEntity entity = clsentities.getEntityById(entityid);
                    if (entity == null)
                    {
                        Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
                    }
                    excludeTables.Add(entity.table.TableID);

                    txtentityname.Text = entity.entityname;
                    txtpluralname.Text = entity.pluralname;
                    txtdescription.Text = entity.description;
                    chkBuiltIn.Checked = entity.BuiltIn;

                    // built-in/system greenlights can't be demoted back to normal greenlights, so disable this (it may have been enabled if the user is adminonly)
                    if (chkBuiltIn.Checked)
                    {
                        chkBuiltIn.Enabled = false;
                    }

                    chkenableattachments.Checked = entity.EnableAttachments;
                    chkEnableAudiences.Checked = entity.AudienceView != AudienceViewType.NoAudience;
                    if (chkEnableAudiences.Checked)
                    {
                        lblAudienceBehaviour.CssClass = "mandatory audienceBehaviour";
                        lblAudienceBehaviour.Text += "*";    
                    }
                    
                    cmpAudiences.Enabled = chkEnableAudiences.Checked;
                    ddlAudienceBehaviour.Enabled = chkEnableAudiences.Checked;
                    ddlAudienceBehaviour.SelectedIndex = (int) entity.AudienceView;

                    chkallowdocmerge.Checked = entity.AllowMergeConfigAccess;
                    chkEnableCurrencies.Checked = entity.EnableCurrencies;
                    chkEnableCurrencies.Enabled = !chkEnableCurrencies.Checked;
                    ddlDefaultCurrency.Enabled = !chkEnableCurrencies.Checked;
                    ddlDefaultCurrency.Items.AddRange(entity.DefaultCurrencyID.HasValue
                        ? clsCurrencies.CreateDropDown(entity.DefaultCurrencyID.Value)
                        : clsCurrencies.CreateDropDown().ToArray());
                    chkEnablePopupWindow.Checked = entity.EnablePopupWindow;
                    ddlPopupWindowView.Items.Clear();
                    ddlPopupWindowView.Enabled = chkEnablePopupWindow.Checked;
                    chkenablelocking.Checked = entity.EnableLocking;
                    ddlPopupWindowView.Items.AddRange(clsentities.createViewDropDown(entity.entityid,
                        entity.DefaultPopupView, user.AccountID));
                    ddlFormSelectionAttribute.Items.AddRange(
                        entity.FormSelectionAttributes.Select(
                            a =>
                                new ListItem
                                {
                                    Text = a.displayname,
                                    Value = a.attributeid.ToString(CultureInfo.InvariantCulture)
                                }).ToArray());

                    if (entity.FormSelectionAttributeId.HasValue)
                    {
                        ddlFormSelectionAttribute.SelectedValue = entity.FormSelectionAttributeId.Value.ToString();
                    }

                    if (entity.HasFormSelectionMappings())
                    {
                        ddlFormSelectionAttribute.Enabled = false;
                    }


                    if (entity.OwnerId.HasValue || entity.SupportContactId.HasValue)
                    {
                        cEmployees employees = new cEmployees(user.AccountID);

                        if (entity.OwnerId.HasValue)
                        {
                            selOwner.SetValue(employees.GetEmployeeById(entity.OwnerId.Value).FullNameUsername,
                                entity.OwnerId.Value.ToString());
                        }

                        if (entity.SupportContactId.HasValue)
                        {
                            selSupportContact.SetValue(
                                employees.GetEmployeeById(entity.SupportContactId.Value).FullNameUsername,
                                entity.SupportContactId.Value.ToString());
                        }
                    }

                    txtSupportQuestion.Text = entity.SupportQuestion;

                    cAttribute att = (from x in entity.attributes.Values
                        where x.isauditidentifer
                        select x).FirstOrDefault();
                    if (att == null)
                    {
                        hdnAuditAttributeID.Text = "";
                        hdnAuditAttributeDisplayName.Text = "";
                    }
                    else
                    {
                        hdnAuditAttributeID.Text = att.attributeid.ToString();
                        hdnAuditAttributeDisplayName.Text = att.displayname;
                    }
                    //ddlDefaultCurrency.ClearSelection();
                    //ddlDefaultCurrency.SelectedIndex = ddlDefaultCurrency.Items.IndexOf(ddlDefaultCurrency.Items.FindByValue(entity.DefaultCurrencyID.ToString()));
                    attributeGridData = clsentities.createAttributeGrid(entity);
                    formGridData = clsentities.createFormGrid(entity);
                    viewGridData = clsentities.createViewGrid(entity);

                    cWorkflows clsworkflows = new cWorkflows(user);
                    cmbattributeworkflow.Items.AddRange(clsworkflows.CreateDropDown(entity.table.TableID).ToArray());
                    cmbviewaddform.Items.AddRange(entity.CreateFormDropDown().ToArray());
                    cmbvieweditform.Items.AddRange(entity.CreateFormDropDown().ToArray());
                    Master.title = "GreenLight: " + entity.entityname;
                }
                else
                {
                    user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.CustomEntities, true, true);

                    Master.title = "New GreenLight";
                    attributeGridData = clsentities.createAttributeGrid(null);
                    formGridData = clsentities.createFormGrid(null);
                    viewGridData = clsentities.createViewGrid(null);
                    ddlDefaultCurrency.Items.AddRange(clsCurrencies.CreateDropDown().ToArray());
                    ddlDefaultCurrency.Enabled = false;
                }

                chkEnableCurrencies.Attributes.Add("onclick", "setCurrencyListState();");
                
                litattributegrid.Text = attributeGridData[1];
                litformgrid.Text = formGridData[1];
                litviewgrid.Text = viewGridData[1];


                Master.PageSubTitle = "GreenLight Details";
                ClientScript.RegisterClientScriptBlock(GetType(), "variables", "var entityid = " + entityid + "; var accountid = " + user.AccountID + ";\n", true);
                ClientScript.RegisterStartupScript(GetType(), "variables2", cGridNew.generateJS_init("CEGridVars", new List<string>() { attributeGridData[0], formGridData[0], viewGridData[0] }, user.CurrentActiveModule), true);
                ClientScript.RegisterStartupScript(GetType(), "variables3", "SEL.CustomEntityAdministration.IDs.Entity = " + entityid + ";\n", true);

                /* Static paths */
                string informationPngUrl = "/shared/images/icons/24/plain/information.png";
                imgFormDesignerHelp.ImageUrl = informationPngUrl;
                imgViewColumnHelp.ImageUrl = informationPngUrl;
                imgViewFilterHelp.ImageUrl = informationPngUrl;
                imgManyToOneFilterHelp.ImageUrl = informationPngUrl;
                imgDisplayFieldHelp.ImageUrl = informationPngUrl;
                imgOneToManyFilterHelp.ImageUrl = informationPngUrl;
                tcFields.StaticLibraryPath = GlobalVariables.StaticContentLibrary + "/";
                tcFields.ThemesPath = GlobalVariables.StaticContentLibrary + "/js/jstree/themes/";
                tcFilters.StaticLibraryPath = GlobalVariables.StaticContentLibrary + "/";
                tcFilters.ThemesPath = GlobalVariables.StaticContentLibrary + "/js/jstree/themes/";
                tcRelFilters.StaticLibraryPath = GlobalVariables.StaticContentLibrary + "/";
                tcRelFilters.ThemesPath = GlobalVariables.StaticContentLibrary + "/js/jstree/themes/";

                tcMTODisplayField.StaticLibraryPath = GlobalVariables.StaticContentLibrary + "/";
                tcMTODisplayField.ThemesPath = GlobalVariables.StaticContentLibrary + "/js/jstree/themes/";

                txtentityname.Focus();
            }
        }

        /// <summary>
        /// Save/OK button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("custom_entities.aspx", true);
        }

        /// <summary>
        /// Cancel Button event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdcancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("custom_entities.aspx", true);
        }

        /// <summary>
        /// Get the global Static Library Path
        /// </summary>
        /// <returns></returns>
        public string GetStaticLibPath()
        {
            return GlobalVariables.StaticContentLibrary;
        }
    }
}
