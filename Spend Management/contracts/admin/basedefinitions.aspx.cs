using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Script;
using System.Web.Script.Services;
using System.Web.Services;
using SpendManagementLibrary;

namespace Spend_Management
{
    using BusinessLogic.Modules;

    #region cBaseDefinitionValues
    /// <summary>
    /// Simple class for passing base definition field information to and from client using Ajax
    /// </summary>
    [Serializable()]
    public class cBaseDefinitionValues
    {
        private string gFieldId;
        private string gTableId;
        private string sFieldValue;
        private string sFieldName;
        private string sTableName;
        private string sFieldType;
        private bool bGenList;
        private bool bIsValueList;
        
        #region properties
        /// <summary>
        /// table ID of base definition table
        /// </summary>
        public string tableID
        {
            get { return gTableId; }
            set { gTableId = value; }
        }
        /// <summary>
        /// fieldID of individual base definition field
        /// </summary>
        public string fieldID
        {
            get { return gFieldId; }
            set { gFieldId = value; }
        }
        /// <summary>
        /// Current data content value of the definition field
        /// </summary>
        public string fieldValue
        {
            get { return sFieldValue; }
            set { sFieldValue = value; }
        }
        /// <summary>
        /// Name of the definition field for display in the field label
        /// </summary>
        public string fieldName
        {
            get { return sFieldName; }
            set { sFieldName = value; }
        }
        /// <summary>
        /// Base definition table name
        /// </summary>
        public string tableName
        {
            get { return sTableName; }
            set { sTableName = value; }
        }
        /// <summary>
        /// Individual base definition field type
        /// </summary>
        public string fieldType
        {
            get { return sFieldType; }
            set { sFieldType = value; }
        }
        /// <summary>
        /// Is individual base definition field a genlist type
        /// </summary>
        public bool genlist
        {
            get { return bGenList; }
            set { bGenList = value; }
        }
        /// <summary>
        /// Is individual base definition field a value list type
        /// </summary>
        public bool isValueList
        {
            get { return bIsValueList; }
            set { bIsValueList = value; }
        }
#endregion

        /// <summary>
        /// Empty constructor for serialization
        /// </summary>
        public cBaseDefinitionValues() { }

        /// <summary>
        /// cBaseDefinitionValues constructor
        /// </summary>
        /// <param name="fieldName">Field Name</param>
        /// <param name="tableName">Table Name</param>
        /// <param name="tableid">Table ID</param>
        /// <param name="fieldType">Field ID</param>
        /// <param name="genList">Is field a GenList type</param>
        /// <param name="valueList">Is field a valueList type</param>
        /// <param name="fieldid">Field ID</param>
        /// <param name="fieldValue">Data value for the field</param>
        public cBaseDefinitionValues(string fieldName, string tableName, string tableid, string fieldType, bool genList, bool valueList, string fieldid, string fieldValue)
        {
            sFieldValue = fieldValue;
            gFieldId = fieldid;
            sFieldName = fieldName;
            sTableName = tableName;
            gTableId = tableid;
            sFieldType = fieldType;
            bGenList = genList;
            bIsValueList = valueList;
        }        
    }
    #endregion

    /// <summary>
    /// Base definitions administration class
    /// </summary>
    public partial class basedefinitions : System.Web.UI.Page
    {
        /// <summary>
        /// Current base definition table
        /// </summary>
        public static cTable baseTable;
        /// <summary>
        /// Current definition table ID
        /// </summary>
        public static Guid baseTableId;
        /// <summary>
        /// Name of the validation group
        /// </summary>
        public static string valGroup;
        /// <summary>
        /// Current spend management element
        /// </summary>
        public static SpendManagementElement currentElement;
        /// <summary>
        /// Summary grid collection of columns
        /// </summary>
        public static List<cNewGridColumn> columns;

        /// <summary>
        /// Page_init
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Init(object sender, EventArgs e)
        {
            // call ajax page load to retrieve the definition grid
            ScriptManager.RegisterStartupScript(this, this.GetType(), "disableBottomToolbar", "Sys.Application.add_load(function(){ setTimeout(loadBDefTables,0); });\n", true);
        }

        /// <summary>
        /// Primary Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires=0;
            Response.AddHeader("Pragma","no-cache");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            CurrentUser curUser = cMisc.GetCurrentUser();
            cTables tables = new cTables(curUser.AccountID);
            Title = "Base Definitions";
            Master.title = "Base Definitions";
            switch (curUser.CurrentActiveModule)
            {
                case Modules.Contracts:
                    Master.helpid = 1133;
                    break;
                default:
                    Master.helpid = 0;
                    break;
            }

            if (!this.IsPostBack)
            {
                currentElement = (SpendManagementElement)int.Parse(Request.QueryString["bdt"]);
                baseTable = getBaseDefTableId(currentElement);

                if (baseTable != null)
                {
                    baseTableId = baseTable.TableID;
                    
                    // check that access permitted to this option
                    curUser.CheckAccessRole(AccessRoleType.View, currentElement, false, true);
                    
                    // check for permission to add new definition
                    if (curUser.CheckAccessRole(AccessRoleType.Add, currentElement, false))
                    {
                        litAddDef.Text = "<a class=\"submenuitem\" href=\"javascript:baseDefObject.editDefinition(-1);\">Add Definition</a>";
                    }

                    cBaseDefinitions clsBaseDefs = new cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, currentElement);

                    List<cField> defColumns = clsBaseDefs.SetBaseDefinitionFields(baseTableId, ref columns);

                    // prepare call to service to retrieve grid once page has loaded.
                    StringBuilder sb = new StringBuilder();
                    sb.Append("function loadBDefTables() {\n");
                    foreach(cField field in defColumns)
                    {
                        sb.Append("baseDefObject.addColumn('" + field.FieldID.ToString() + "','" + field.FieldType + "');\n");
                    }
                    sb.Append("baseDefObject.getBaseDefinitionTable('bdGrid_" + baseTableId.ToString().Replace("-","_") + "');\n");
                    sb.Append("}\n");

                    ClientScriptManager smgr = this.ClientScript;
                    smgr.RegisterStartupScript(this.GetType(), "bdefload", sb.ToString(), true);

                    // populate fields in modal
                    phBDefFields.Controls.Add(createBaseDefinitionFields(defColumns));
                }
            }
        }

        /// <summary>
        /// WebMethod method to render the base definitions grid to the screen
        /// </summary>
        /// <returns>cGrid HTML</returns>
        [WebMethod(EnableSession = true)]
        public static string[] getGrid(string gridID)
        {
            return svcBaseDefinitions.displayBaseDefinitions(baseTableId, currentElement, columns, gridID);
        }

        /// <summary>
        /// Get a base definition table by it's passed enumerator
        /// </summary>
        /// <param name="bdt">Base Definition table enumerator</param>
        /// <returns></returns>
        private cTable getBaseDefTableId(SpendManagementElement bdt)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            cAccountSubAccounts subaccs = new cAccountSubAccounts(curUser.AccountID);
            cAccountProperties properties = subaccs.getSubAccountById(curUser.CurrentSubAccountId).SubAccountProperties;
            cTable tId = null;
            cTables tables = new cTables(curUser.AccountID);
            cTable t = null;
             
            switch (bdt)
            {
                case SpendManagementElement.ContractCategories :
                    Master.PageSubTitle = properties.ContractCategoryTitle;
                    t = tables.GetTableByName("codes_contractcategory");
                    break;
                case SpendManagementElement.ContractStatus:
                    Master.PageSubTitle = "Contract Status";
                    t = tables.GetTableByName("codes_contractstatus");
                    break;
                case SpendManagementElement.ContractTypes:
                    Master.PageSubTitle = "Contract Types";
                    t = tables.GetTableByName("codes_contracttype");
                    break;
                case SpendManagementElement.TermTypes:
                    Master.PageSubTitle="Term Types";
                    t = tables.GetTableByName("codes_termtype");
                    break;
                case SpendManagementElement.FinancialStatus:
                    Master.PageSubTitle = "Financial Status";
                    t = tables.GetTableByName("financial_status");
                    break;
                case SpendManagementElement.InflatorMetrics:
                    Master.PageSubTitle = "Inflator Metrics";
                    t = tables.GetTableByName("codes_inflatormetrics");
                    break;
                case SpendManagementElement.InvoiceFrequencyTypes:
                    Master.PageSubTitle = "Invoice Frequencies";
                    t = tables.GetTableByName("codes_invoicefrequencytype");
                    break;
                case SpendManagementElement.InvoiceStatus:
                    Master.PageSubTitle = "Invoice Status";
                    t = tables.GetTableByName("invoiceStatusType");
                    break;
                case SpendManagementElement.TaskTypes:
                    Master.PageSubTitle = "Task Types";
                    t = tables.GetTableByName("codes_tasktypes");
                    break;
                case SpendManagementElement.Units:
                    Master.PageSubTitle = "Units";
                    t = tables.GetTableByName("codes_units");
                    break;
                case SpendManagementElement.ProductCategories:
                    Master.PageSubTitle = "Product/Service Categories";
                    t = tables.GetTableByName("productCategories");
                    break;
                case SpendManagementElement.SubAccounts:
                    Master.PageSubTitle = "Sub-Accounts";
                    t = tables.GetTableByName("accountsSubAccounts");
                    break;
                case SpendManagementElement.LicenceRenewalTypes:
                    Master.PageSubTitle = "Licence Renewal Types";
                    t = tables.GetTableByName("licenceRenewalTypes");
                    break;
                case SpendManagementElement.SupplierStatus:
                    Master.PageSubTitle = properties.SupplierPrimaryTitle + " Status";
                    t = tables.GetTableByName("supplier_status");
                    break;
                case SpendManagementElement.SupplierCategory:
                    Master.PageSubTitle = properties.SupplierCatTitle;
                    t = tables.GetTableByName("supplier_categories");
                    break;
                case SpendManagementElement.ProductLicenceTypes:
                    Master.PageSubTitle = "Product Licence Types";
                    t = tables.GetTableByName("codes_licencetypes");
                    break;
                case SpendManagementElement.AttachmentMimeTypes:
                    Master.PageSubTitle = "Attachment Mime Types";
                    t = tables.GetTableByName("mime_headers");
                    break;
                case SpendManagementElement.SalesTax:
                    Master.PageSubTitle = "Sales Tax";
                    t = tables.GetTableByName("codes_salestax");
                    break;
                default:
                    break;
            }

            if (t != null)
            {
                tId = t;
            }
            return tId;
        }

        /// <summary>
        /// Populates the field controls into the modal dialog for the current base definition table
        /// </summary>
        private PlaceHolder createBaseDefinitionFields(List<cField> defColumns)
        {
            StringBuilder sb = new StringBuilder();
            bool rowalt = false;
            string closeDiv = "";
            valGroup = "basedef";
            cFieldGenerator fg = new cFieldGenerator(valGroup);
            PlaceHolder ph = new PlaceHolder();
            Literal litBD;

            foreach (cField field in defColumns)
            {
                rowalt = rowalt ^ true;
                if (field.FieldType == "LT")
                {
                    sb.Append(closeDiv);
                    sb.Append("<div class=\"onecolumn\">\n");
                    closeDiv = "</div>\n";
                    rowalt = false;
                }
                else
                {
                    if (rowalt)
                    {
                        sb.Append(closeDiv);
                        sb.Append("<div class=\"twocolumn\">\n");
                        closeDiv = "</div>\n";
                    }
                }

                litBD = new Literal();
                litBD.ID = "lbl" + baseTable.TableName + "_" + field.FieldID;
                litBD.Text = sb.ToString();
                ph.Controls.Add(litBD);
                sb.Remove(0, sb.Length);

                switch (field.FieldType)
                {
                    case "S":
                    case "T":
                    case "LT":
                    case "FS":
                        ph.Controls.Add(fg.GetCharFieldEntry(baseTable.TableName + "_" + field.FieldName, field, string.Empty, string.Empty, string.Empty, "baseDefObject.saveDefinition()"));
                        break;
                    case "N":
                        if (field.GenList || field.ValueList)
                        {
                            ph.Controls.Add(fg.GetDDListFieldEntry(baseTable.TableName + "_" + field.FieldName, field, null, string.Empty, string.Empty, "baseDefObject.saveDefinition()"));
                        }
                        else
                        {
                            ph.Controls.Add(fg.GetNumberFieldEntry(baseTable.TableName + "_" + field.FieldName, field, null, string.Empty, string.Empty, "baseDefObject.saveDefinition()"));
                        }
                        break;
                    case "FI":
                        ph.Controls.Add(fg.GetNumberFieldEntry(baseTable.TableName + "_" + field.FieldName, field, null, string.Empty, string.Empty, "baseDefObject.saveDefinition()"));
                        break;
                    case "A":
                    case "C":
                    case "M":
                    case "FD":
                        ph.Controls.Add(fg.GetDecimalFieldEntry(baseTable.TableName + "_" + field.FieldName, field, null, string.Empty, string.Empty, "baseDefObject.saveDefinition()"));
                        break;
                    case "X":
                        ph.Controls.Add(fg.GetCheckBoxFieldEntry(baseTable.TableName + "_" + field.FieldName, field, false, string.Empty, string.Empty, "baseDefObject.saveDefinition()"));
                        break;
                    case "D":
                        ph.Controls.Add(fg.GetDateFieldEntry(baseTable.TableName + "_" + field.FieldName, field, null, string.Empty, string.Empty, "baseDefObject.saveDefinition()"));
                        break;

                    default:
                        break;
                }
            }

            litBD = new Literal();
            litBD.Text = closeDiv;
            ph.Controls.Add(litBD);

            return ph;
        }

        public List<cField> prepareGrid(Guid tableId, SpendManagementElement currentElement, ref List<cNewGridColumn> columns)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            
            List<cField> bdColumns = new List<cField>();
            columns = new List<cNewGridColumn>();

            if (baseTableId != Guid.Empty)
            {
                cFields fields = new cFields(curUser.AccountID);

                cField idfield = baseTable.GetPrimaryKey();
                cField tmpField = null;

                if (baseTable != null)
                {
                    columns.Add(new cFieldColumn(idfield));

                    switch (currentElement)
                    {
                        case SpendManagementElement.ContractCategories:
                            tmpField = fields.GetBy(baseTable.TableID, "categoryDescription");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.ContractStatus:
                            tmpField = fields.GetBy(baseTable.TableID, "statusDescription");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));

                            tmpField = fields.GetBy(baseTable.TableID, "isArchive");
                            columns.Add(new cFieldColumn(tmpField));
                            bdColumns.Add(tmpField);
                            break;
                        case SpendManagementElement.ContractTypes:
                            tmpField = fields.GetBy(baseTable.TableID, "contractTypeDescription");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.FinancialStatus:
                            tmpField = fields.GetBy(baseTableId, "description");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.TermTypes:
                            tmpField = fields.GetBy(baseTableId, "termTypeDescription");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.InflatorMetrics:
                            tmpField = fields.GetBy(baseTableId, "name");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));

                            tmpField = fields.GetBy(baseTableId, "percentage");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));

                            tmpField = fields.GetBy(baseTableId, "requiresExtraPct");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.InvoiceFrequencyTypes:
                            tmpField = fields.GetBy(baseTableId, "invoiceFrequencyTypeDesc");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));

                            tmpField = fields.GetBy(baseTableId, "frequencyInMonths");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.InvoiceStatus:
                            tmpField = fields.GetBy(baseTableId, "description");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));

                            tmpField = fields.GetBy(baseTableId, "isArchive");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.TaskTypes:
                            tmpField = fields.GetBy(baseTableId, "typeDescription");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.Units:
                            tmpField = fields.GetBy(baseTableId, "description");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.ProductCategories:
                            tmpField = fields.GetBy(baseTableId, "description");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.SubAccounts:
                            tmpField = fields.GetBy(baseTableId, "Description");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            tmpField = fields.GetBy(baseTableId, "associatedSubAccountID");
                            cAccountSubAccounts clsSubAccs = new cAccountSubAccounts(curUser.AccountID);
                            ListItem[] items = clsSubAccs.CreateFilteredDropDown(curUser.Employee, null);

                            int keyIndex = 0;

                            foreach (ListItem item in items)
                            {
                                keyIndex = 0;

                                int.TryParse(item.Value.ToString(), out keyIndex);

                                if (!tmpField.ListItems.ContainsKey(keyIndex))
                                {
                                    tmpField.ListItems.Add(keyIndex, item.Text);
                                }
                            }

                            bdColumns.Add(tmpField);
                            //columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.LicenceRenewalTypes:
                            tmpField = fields.GetBy(baseTableId, "description");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.SupplierStatus:
                            tmpField = fields.GetBy(baseTableId, "description");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));

                            tmpField = fields.GetBy(baseTableId, "sequence");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));

                            tmpField = fields.GetBy(baseTableId, "deny_contract_add");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.SupplierCategory:
                            tmpField = fields.GetBy(baseTableId, "description");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.ProductLicenceTypes:
                            tmpField = fields.GetBy(baseTableId, "description");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.AttachmentMimeTypes:
                            tmpField = fields.GetBy(baseTableId, "fileExtension");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));

                            tmpField = fields.GetBy(baseTableId, "mimeHeader");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        case SpendManagementElement.SalesTax:
                            tmpField = fields.GetBy(baseTableId, "description");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));

                            tmpField = fields.GetBy(baseTableId, "salesTax");
                            bdColumns.Add(tmpField);
                            columns.Add(new cFieldColumn(tmpField));
                            break;
                        default:
                            break;
                    }
                }
            }

            return bdColumns;
        }

        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string parentURL = (SiteMap.CurrentNode.ParentNode == null) ? "~/home.aspx" : SiteMap.CurrentNode.ParentNode.Url;

            CurrentUser user = cMisc.GetCurrentUser();
            switch (user.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.Contracts:
                    Response.Redirect(parentURL, true);
                    break;
                default:
                    Response.Redirect("~/home.aspx", true);
                    break;
            }
        }
    }
}
