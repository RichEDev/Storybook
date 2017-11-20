using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using AjaxControlToolkit;

namespace Spend_Management
{
    using System.Text;

    public partial class aeImportTemplate : System.Web.UI.Page
    {
        private int _TemplateID = 0;

        ///// <summary>
        ///// Passes the edited template id through to JS
        ///// </summary>
        public int jsTemplateID
        {
            get { return _TemplateID; }
        }

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ImportTemplates, true, true);

            cImportTemplates clsImportTemplates = new cImportTemplates(currentUser.AccountID);
            cImportTemplate reqTemplate = null;
            this.Master.helpid = 1122;
            if (int.TryParse(Request.QueryString["templateid"], out _TemplateID))
            {
                currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ImportTemplates, true, true);
                reqTemplate = clsImportTemplates.getImportTemplateByID(_TemplateID);
                ViewState["ImportTemplateID"] = _TemplateID;
                Title = "Import Template: Edit";
            }
            else
            {
                currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ImportTemplates, true, true);
                Title = "Import Template: New";
            }

            Master.title = Title;
            Master.PageSubTitle = "Import Template Details";

            if (!IsPostBack)
            {
                ddlApplicationType.Items.Add(new ListItem("[None]", "-1"));
                ddlApplicationType.Items.Add(new ListItem("ESR Outbound File Import", ((int)ApplicationType.ESROutboundImport).ToString()));
                //ddlApplicationType.Items.Add(new ListItem("Excel Import", ((int)ApplicationType.ExcelImport).ToString()));

                cESRTrusts clsTrusts = new cESRTrusts(currentUser.AccountID);
                cESRTrust trust = null;
                ListItem li = null;
                clsTrusts.CreateDropDownList(ref ddlTrustID);

                if (int.TryParse(Request.QueryString["templateid"], out _TemplateID))
                {
                    if (reqTemplate != null)
                    {
                        var applicationType = (int)reqTemplate.appType;
                        if (applicationType == 2)
                        {
                            applicationType = 0;
                        }

                        this.ddlApplicationType.SelectedValue = applicationType.ToString();
                    }

                    ddlApplicationType.Enabled = false;
                    ddlTrustID.SelectedValue = reqTemplate.NHSTrustID.ToString();
                    txtTemplateName.Text = reqTemplate.TemplateName;

                    foreach (cImportTemplate temp in clsImportTemplates.listImportTemplates().Values)
                    {
                        if (temp.NHSTrustID != reqTemplate.NHSTrustID)
                        {
                            trust = clsTrusts.GetESRTrustByID(temp.NHSTrustID);
                            li = new ListItem(trust.TrustName, trust.TrustID.ToString());
                            ddlTrustID.Items.Remove(li);
                        }
                    }

                    //chkAutomated.Checked = reqTemplate.IsAutomated;
                }
                else
                {
                    foreach (cImportTemplate temp in clsImportTemplates.listImportTemplates().Values)
                    {
                        trust = clsTrusts.GetESRTrustByID(temp.NHSTrustID);
                        if (trust != null)
                        {
                            li = new ListItem(trust.TrustName, trust.TrustID.ToString());
                            ddlTrustID.Items.Remove(li);
                        }
                    }
                }

                CreateMappingTabs();
            }
        }

        /// <summary>
        /// Handle the Application Type template change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlApplicationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateMappingTabs();
        }

        /// <summary>
        /// Handle the Trust change to refresh the template fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlTrustID_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateMappingTabs();
        }

        /// <summary>
        /// Creates the tabs for element mapping templates and displays the selected one from the drop down above
        /// </summary>
        protected void CreateMappingTabs()
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            pnlMappings.Controls.Clear();

            // generate the placeholder text if no application type selected
            if (ddlApplicationType.SelectedValue == "-1" || ddlTrustID.SelectedValue == "0")
            {
                Literal lit = new Literal();
                lit.Text = "<div class=\"statuspanel\">Please select a valid Application Type and Trust from the dropdown lists</div>";
                
                pnlMappings.Controls.Add(lit);
                return;
            }

            TabContainer tabsContainer = new TabContainer();
            tabsContainer.ID = "tabsContainer";

            cTables clsTables = new cTables(currentUser.AccountID);
            int tabIdx = 0;

            // All tables to be used in the future. Only employees and esr_assignments can be used for now
            SortedList<string, cTable> lstTables = new SortedList<string, cTable>();
            cFields clsFields = new cFields(currentUser.AccountID);
            cESRTrusts clsTrusts = new cESRTrusts(currentUser.AccountID);
            cESRTrust currentTrust = clsTrusts.GetESRTrustByID(int.Parse(ddlTrustID.Items[ddlTrustID.SelectedIndex].Value));

            cImportTemplates clsImportTemplates = new cImportTemplates(currentUser.AccountID);
            cImportTemplate reqTemplate = null;

            Dictionary<string, List<XMLMapFields>> lstMappings = clsImportTemplates.GetApplicationXMLMappings((ApplicationType)(Convert.ToInt32(ddlApplicationType.SelectedValue)), currentTrust.EsrInterfaceVersionNumber);

            if (_TemplateID > 0)
            {
                reqTemplate = clsImportTemplates.getImportTemplateByID(_TemplateID);
            }

            foreach (KeyValuePair<string, List<XMLMapFields>> kvp in lstMappings)
            {
                int fieldCount = (from x in lstMappings from y in x.Value where y.populated && x.Key == kvp.Key select x).Count();
                int importFieldCount = 0;
                if (reqTemplate == null)
                {
                    importFieldCount = (from x in lstMappings from y in x.Value where y.populated && y.importField && x.Key == kvp.Key select x).Count();
                }
                else
                {
                    importFieldCount = (from x in reqTemplate.Mappings where x.Populated && x.ImportField && x.ElementType == cImportTemplates.GetElementType(kvp.Key) select x).Count();
                }

                string currentDefaultTableName = (from x in kvp.Value where !string.IsNullOrEmpty(x.defaultTableName) select x.defaultTableName).FirstOrDefault();
                cTable currentDefaultTable = null;
                if (!string.IsNullOrEmpty(currentDefaultTableName))
                {
                    currentDefaultTable = clsTables.GetTableByName(currentDefaultTableName);
                }

                VisibilityLevel checkboxColumnVisible = this.RecordCheckboxVisible(currentTrust, currentDefaultTable);
                string currentRelExpTableName = (from x in kvp.Value where !string.IsNullOrEmpty(x.relatedExpensesTable) select x.relatedExpensesTable).FirstOrDefault();
                int currentDefTableIdx = -1;
                int currentRelExpTableIdx = -1;

                TabPanel tab = new TabPanel();
                //tab.Style.Add(HtmlTextWriterStyle.Display, "block");
                tab.HeaderText = kvp.Key;

                Table tblMappings = new Table();
                tblMappings.CssClass = "datatbl";
                tblMappings.ID = "element_" + kvp.Key;
                TableHeaderRow thRow = new TableHeaderRow();
                TableHeaderCell thCell = new TableHeaderCell();
                thCell.Text = "Column Name";
                thRow.Controls.Add(thCell);
                thCell = new TableHeaderCell();
                thCell.Text = "Database Table";
                thRow.Controls.Add(thCell);
                thCell = new TableHeaderCell();
                thCell.Text = "Database Field";
                thRow.Controls.Add(thCell);
                thCell = new TableHeaderCell();
                thCell.ID = "importfield_" + kvp.Key;
                CheckBox chk = new CheckBox { ID = "chkImportFields_" + kvp.Key, ToolTip = "Select / Deselect record for import" };
                chk.Checked = importFieldCount >= (fieldCount / 2); // default to checked if more checked than unchecked
                chk.Attributes.Add("onclick", string.Format("SEL.ImportTemplates.ToggleFieldImportFlags(this, {0});", tabIdx++));
                thCell.Controls.Add(chk);
                if (checkboxColumnVisible == VisibilityLevel.Hidden)
                {
                    thCell.Style.Add(HtmlTextWriterStyle.Display, "none");
                }
                thRow.Controls.Add(thCell);
                tblMappings.Controls.Add(thRow);

                TableRow tr;
                TableCell td;
                Literal txtFieldName;
                HiddenField hfColRef;
                HiddenField hfMandatory;
                HiddenField hfDataType;
                HiddenField hfLookupTableId;
                HiddenField hfMatchFieldId;
                HiddenField hfOverridePK;
                HiddenField hfPopulated;
                HiddenField hfAllowDynamicMapping;
                CheckBox chkImportField;

                DropDownList ddlDBTable;
                DropDownList ddlDBField;
                CompareValidator reqCompVal;
                cImportTemplateMapping tempTemplateMapping = null;
                string rowCSS = "row1";

                if (currentTrust.EsrInterfaceVersionNumber == 1)
                {
                    cTable table = clsTables.GetTableByName("employees");
                    if (!lstTables.ContainsKey(table.TableName))
                    {
                        lstTables.Add(table.TableName, table);
                    }
                    table = clsTables.GetTableByName("esr_assignments");
                    if (!lstTables.ContainsKey(table.TableName))
                    {
                        lstTables.Add(table.TableName, table);
                    }
                }
                else
                {
                    if (currentDefaultTableName == "esr_assignments")
                    {
                        cTable table = clsTables.GetTableByName("employees");
                        if (!lstTables.ContainsKey(table.TableName))
                        {
                            lstTables.Add(table.TableName, table);
                        }
                    }

                    if (!string.IsNullOrEmpty(currentDefaultTableName) && !lstTables.ContainsKey(currentDefaultTableName))
                    {
                        lstTables.Add(currentDefaultTableName, clsTables.GetTableByName(currentDefaultTableName));
                        currentDefTableIdx = lstTables.IndexOfKey(currentDefaultTableName);
                    }

                    if (!string.IsNullOrEmpty(currentRelExpTableName) && !lstTables.ContainsKey(currentRelExpTableName))
                    {
                        lstTables.Add(currentRelExpTableName, clsTables.GetTableByName(currentRelExpTableName));
                        currentRelExpTableIdx = lstTables.IndexOfKey(currentRelExpTableName);
                    }
                }

                foreach (XMLMapFields val in kvp.Value)
                {
                    if (reqTemplate != null)
                    {
                        tempTemplateMapping = reqTemplate.GetMappingByColRef(kvp.Key, val.ColRef);
                    }
                    
                    if (!val.populated)
                    {
                        continue;
                    }
                    
                    tr = new TableRow();
                    tr.CssClass = rowCSS;
                    td = new TableCell();
                    txtFieldName = new Literal();
                    hfColRef = new HiddenField();
                    hfMandatory = new HiddenField();
                    hfDataType = new HiddenField();
                    hfLookupTableId = new HiddenField();
                    hfMatchFieldId = new HiddenField();
                    hfOverridePK = new HiddenField();
                    hfPopulated = new HiddenField();
                    hfAllowDynamicMapping = new HiddenField();
                    ddlDBTable = new DropDownList();
                    ddlDBField = new DropDownList();
                    chkImportField = new CheckBox();

                    txtFieldName.Text = val.DestinationField;
                    txtFieldName.ID = "fieldName_" + val.ColRef.ToString();
                    hfColRef.ID = "colRef_" + val.ColRef.ToString();
                    hfColRef.Value = val.ColRef.ToString();
                    hfMandatory.ID = "mand_" + val.ColRef.ToString();
                    hfMandatory.Value = val.Mandatory.ToString();
                    hfDataType.ID = "dtype_" + val.ColRef.ToString();
                    hfDataType.Value = ((int)val.dataType).ToString();
                    hfLookupTableId.ID = "lkup_" + val.ColRef.ToString();
                    string lkupTableId = string.Empty;
                    string matchFieldId = string.Empty;
                    if (!string.IsNullOrEmpty(val.lookupTable))
                    {
                        cTable tmpTable = clsTables.GetTableByName(val.lookupTable);
                        if (tmpTable != null)
                        {
                            lkupTableId = tmpTable.TableID.ToString();
                        }
                    }

                    hfLookupTableId.Value = lkupTableId;
                    if (!string.IsNullOrEmpty(val.matchField))
                    {
                        var lookupTable = clsTables.GetTableByName(val.lookupTable);
                        if (lookupTable != null)
                        {
                            cField tmpField = clsFields.GetBy(lookupTable.TableID, val.matchField);
                            if (tmpField != null)
                            {
                                matchFieldId = tmpField.FieldID.ToString();
                            }
                        }
                    }

                    hfMatchFieldId.ID = "mf_" + val.ColRef.ToString();
                    hfMatchFieldId.Value = matchFieldId;
                    hfOverridePK.ID = "opk_" + val.ColRef.ToString();
                    hfOverridePK.Value = val.overridePrimaryKey.ToString();
                    hfPopulated.ID = "pop_" + val.ColRef.ToString();
                    hfPopulated.Value = val.populated.ToString();
                    hfAllowDynamicMapping.ID = "dynmap_" + val.ColRef.ToString();
                    hfAllowDynamicMapping.Value = val.allowDynamicMapping.ToString();
                    chkImportField.ID = "if_" + val.ColRef.ToString();
                    chkImportField.Checked = val.importField;
                    chkImportField.ToolTip = string.Format("{0} field for import", (val.importField ? "Deselect" : "Select"));
                    
                    ddlDBTable.ID = "tableguid_" + val.ColRef.ToString();
                    ddlDBField.ID = "fieldguid_" + val.ColRef.ToString();

                    td.Controls.Add(txtFieldName);
                    td.Controls.Add(hfColRef);
                    td.Controls.Add(hfMandatory);
                    td.Controls.Add(hfDataType);
                    td.Controls.Add(hfLookupTableId);
                    td.Controls.Add(hfMatchFieldId);
                    td.Controls.Add(hfOverridePK);
                    td.Controls.Add(hfPopulated);
                    td.Controls.Add(hfAllowDynamicMapping);
                    tr.Controls.Add(td);

                    td = new TableCell();
                    ddlDBTable.Items.Add(new ListItem("[None]", "0"));
                    foreach (KeyValuePair<string, cTable> tbl in lstTables)
                    {
                        if (string.IsNullOrEmpty(tbl.Value.Description) == false)
                        {
                            ddlDBTable.Items.Add(new ListItem(tbl.Value.Description, tbl.Value.TableID.ToString()));

                            if (val.referenceTable == tbl.Value.TableName)
                            {
                                // does a mappings exist or are we adding new (prevent [None] mappings from being overridden by default mapping on edit
                                if ((tempTemplateMapping != null && reqTemplate != null) || (reqTemplate == null))
                                {
                                    ddlDBTable.SelectedIndex = ddlDBTable.Items.Count - 1;
                                    ddlDBTable.Enabled = currentTrust.EsrInterfaceVersionNumber > 1 && val.allowDynamicMapping;
                                }
                            }

                            if (tempTemplateMapping != null && tempTemplateMapping.FieldID != Guid.Empty && ddlDBTable.Enabled)
                            {
                                cField tempField = clsFields.GetFieldByID(tempTemplateMapping.FieldID);
                                if (tempField.FieldSource == cField.FieldSourceType.Userdefined)
                                {
                                    ddlDBTable.SelectedValue = clsTables.GetTableByUserdefineTableID(tempField.TableID).TableID.ToString();
                                }
                                else
                                {
                                    ddlDBTable.SelectedValue = tempField.TableID.ToString();
                                }
                            }
                        }
                    }
                    
                    td.Controls.Add(ddlDBTable);
                    tr.Controls.Add(td);

                    td = new TableCell();
                    ddlDBField.Items.Add(new ListItem("[None]", "0"));

                    if ((val.referenceTable != string.Empty && val.referenceField != string.Empty) || (tempTemplateMapping != null && ddlDBTable.SelectedIndex > 0))
                    {
                        svcImportTemplates svc = new svcImportTemplates();
                        List<sFieldBasics> lstFields = svc.GetTableFields(ddlDBTable.SelectedValue, val.dataType, true, true);

                        foreach (sFieldBasics fbasic in lstFields)
                        {
                            ddlDBField.Items.Add(new ListItem(fbasic.Description, fbasic.FieldID.ToString()));
                            if (val.referenceField == fbasic.FieldName)
                            {
                                ddlDBField.SelectedIndex = ddlDBField.Items.Count - 1;
                                ddlDBField.Enabled = currentTrust.EsrInterfaceVersionNumber > 1 && val.allowDynamicMapping;
                            }

                            if (tempTemplateMapping != null && ddlDBField.Enabled)
                            {
                                ddlDBField.SelectedValue = tempTemplateMapping.FieldID.ToString();
                            }
                        }
                    }

                    if (currentTrust.EsrInterfaceVersionNumber > 1)
                    {
                        if (ddlDBTable.SelectedIndex == 0 || (currentDefaultTable != null && ddlDBTable.SelectedItem.Value == currentDefaultTable.TableID.ToString()))
                        {
                            ddlDBField.Enabled = false;
                        }
                        else
                        {
                            ddlDBField.Enabled = val.allowDynamicMapping;
                        }
                    }
                    else if (ddlDBTable.SelectedIndex == 0)
                    {
                        ddlDBField.Enabled = false;
                    }

                    td.Controls.Add(ddlDBField);

                    if (val.Mandatory)
                    {
                        reqCompVal = new CompareValidator();
                        reqCompVal.ID = "reqCompVal" + val.ColRef.ToString();
                        reqCompVal.ControlToValidate = ddlDBField.ID;
                        reqCompVal.ValueToCompare = "0";
                        reqCompVal.Operator = ValidationCompareOperator.NotEqual;
                        reqCompVal.ErrorMessage = string.Format("{0} is a mandatory mapping", val.DestinationField);
                        reqCompVal.Text = "*";

                        td.Controls.Add(reqCompVal);
                    }

                    tr.Controls.Add(td);

                    td = new TableCell();
                    if (tempTemplateMapping != null)
                    {
                        chkImportField.Checked = tempTemplateMapping.ImportField;
                        chkImportField.ToolTip = string.Format("{0} field for import", (tempTemplateMapping.ImportField ? "Deselect" : "Select"));
                    }

                    switch (checkboxColumnVisible)
                    {
                        case VisibilityLevel.Hidden:
                            td.Style.Add(HtmlTextWriterStyle.Display, "none");
                            break;
                        case VisibilityLevel.VisibleAllOnly:
                            chkImportField.Style.Add(HtmlTextWriterStyle.Display, "none");
                            break;
                    }

                    td.Controls.Add(chkImportField);
                    tr.Controls.Add(td);
                    tblMappings.Controls.Add(tr);

                    string currentSelectionFieldId = string.Empty;
                    string defaultTableMappingFieldId = string.Empty;

                    if (currentTrust.EsrInterfaceVersionNumber > 1)
                    {
                        var referenceTable = clsTables.GetTableByName(val.referenceTable);
                        var defaultTable = clsTables.GetTableByName(val.defaultTableName);
                        currentSelectionFieldId = tempTemplateMapping == null
                                                             ? clsFields.GetBy(referenceTable.TableID, val.referenceField).FieldID.ToString()
                                                             : tempTemplateMapping.FieldID.ToString();

                        defaultTableMappingFieldId = string.IsNullOrEmpty(val.defaultFieldName)
                                                                ? clsFields.GetBy(referenceTable.TableID, val.referenceField).FieldID.ToString()
                                                                : clsFields.GetBy(defaultTable.TableID, val.defaultFieldName).FieldID.ToString();
                    }
                    ddlDBTable.Attributes.Add("onchange", string.Format("SEL.ImportTemplates.LoadFields(this, {0}, '{1}','{2}', '{3}');", (int)val.dataType, currentDefaultTable != null ? currentDefaultTable.TableID.ToString() : string.Empty, currentSelectionFieldId, defaultTableMappingFieldId));

                    // alternate the row style
                    rowCSS = (rowCSS == "row1") ? "row2" : "row1";
                }

                if (currentDefaultTableName == "esr_assignments")
                {
                    tblMappings.Rows.AddRange(new []
                    {
                        new TableRow
                        {
                            CssClass = rowCSS + " reverseMapping",
                            Cells = 
                            {
                                new TableCell { Controls =
                                {
                                    new DropDownList
                                    {
                                        CssClass = "ddlAssignmentSignOffOwner",
                                        Items =
                                        {
                                            new ListItem("[None]", Guid.Empty.ToString()),
                                            new ListItem("Supervisor Person ID", "D68DB9D1-0A73-4A76-AAE9-F0DE5F19F9FF") { Selected = reqTemplate == null || reqTemplate.SignOffOwnerFieldId == new Guid("D68DB9D1-0A73-4A76-AAE9-F0DE5F19F9FF") },
                                            new ListItem("Department Manager Assignment ID", "081837AA-A9EF-4316-BF97-15507B7FEBFE") { Selected = reqTemplate != null && reqTemplate.SignOffOwnerFieldId == new Guid("081837AA-A9EF-4316-BF97-15507B7FEBFE") }
                                        }
                                    }
                                }},
                                new TableCell { Text = "ESR Assignments" },
                                new TableCell { Text = "Supervisor" }
                            }
                        },
                        new TableRow
                        {
                            CssClass = "row" + (rowCSS.EndsWith("1") ? 2 : 1) + " reverseMapping",
                            Cells = 
                            {
                                new TableCell { Controls =
                                {
                                    new DropDownList
                                    {
                                        CssClass = "ddlEmployeeLineManager",
                                        Items =
                                        {
                                            new ListItem("[None]", Guid.Empty.ToString()),
                                            new ListItem("Supervisor Person ID (Primary Assignment)", "D68DB9D1-0A73-4A76-AAE9-F0DE5F19F9FF") { Selected = reqTemplate == null || reqTemplate.LineManagerFieldId == new Guid("D68DB9D1-0A73-4A76-AAE9-F0DE5F19F9FF") },
                                            new ListItem("Department Manager Assignment ID (Primary Assignment)", "081837AA-A9EF-4316-BF97-15507B7FEBFE") { Selected = reqTemplate != null && reqTemplate.LineManagerFieldId == new Guid("081837AA-A9EF-4316-BF97-15507B7FEBFE") }
                                        }
                                    }
                                }},
                                new TableCell { Text = "Employees" },
                                new TableCell { Text = "Line Manager" }
                            }
                        }
                    });
                }

                tab.Controls.Add(tblMappings);
                tabsContainer.Tabs.Add(tab);

                if (currentDefTableIdx != -1)
                {
                    lstTables.Remove(currentDefaultTableName);
                }

                if (currentRelExpTableIdx != -1)
                {
                    lstTables.Remove(currentRelExpTableName);
                }
            }

            // add the tabs to the panel inside the update panel
            pnlMappings.Controls.Add(tabsContainer);

            // javascript block to fix the IE9 non-compat mode display issue 
            Literal ieFixScript = new Literal();
            ieFixScript.Text =
                "<script language=\"javascript\" type=\"text/javascript\">$(document).ready(function() {\n$('.datatbl tbody').contents().filter(function () {\nreturn this.nodeType === 3;\n}).remove();\n});</script>";
            pnlMappings.Controls.Add(ieFixScript);
        }

        /// <summary>
        /// Decides whether to display the select all / individual field checkboxes depending on interface version and default table of the record
        /// </summary>
        /// <param name="trust"></param>
        /// <param name="defaultTableName"></param>
        /// <returns></returns>
        private VisibilityLevel RecordCheckboxVisible(cESRTrust trust, cTable defaultTable)
        {
            if (trust.EsrInterfaceVersionNumber == 1)
            {
                return VisibilityLevel.Hidden;
            }

            switch (defaultTable.TableName.ToLower())
            {
                case "esrpersons":
                case "esr_assignments":
                case "esrorganisations":
                case "esrpositions":
                    return VisibilityLevel.Hidden;
                    
                case "esrphones":
                case "esrvehicles":
                case "esrlocations":
                case "esrassignmentcostings":
                case "esraddresses":
                    return VisibilityLevel.VisibleAllOnly;

                default:
                    return VisibilityLevel.Hidden;
            }
        }

        private enum VisibilityLevel
        {
            Hidden = 0,

            VisibleAllOnly = 1,

            VisibleIndividual = 2
        }
    }
}
