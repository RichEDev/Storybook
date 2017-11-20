using System;
using System.Collections.Generic;
using SpendManagementLibrary;
using System.Reflection;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Spend_Management 
{
    /// <summary>
    /// Functional class that allow the saving/deleting/getting of any base definition from the datbase or cache
    /// </summary>
    public class cBaseDefinitions
    {
        private int nAccountID;
        private int nSubAccountID;
        private SpendManagementElement eElement;
        private string sCacheKey;
        private string sCacheDepSQL;
        private string sSaveStoredProcName;
        private string sDeleteStoredProcName;
        private string sRenameFrom;
        private string sRenameTo;

        private Dictionary<int, cBaseDefinition> bdList;
        private readonly Utilities.DistributedCaching.Cache _cache = new Utilities.DistributedCaching.Cache();

        public const string CacheKey = "BaseDefinitions";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="AccountID"></param>
        /// <param name="SubAccountID"></param>
        /// <param name="element"></param>
        public cBaseDefinitions(int AccountID, int SubAccountID, SpendManagementElement element)
        {
            nAccountID = AccountID;
            nSubAccountID = SubAccountID;
            eElement = element;
            setProperties();

            initialiseData();
        }

        #region Properties

        /// <summary>
        /// ID of the logged in account
        /// </summary>
        public int AccountID
        {
            get { return nAccountID; }
        }

        /// <summary>
        /// ID of the current sub-account
        /// </summary>
        public int SubAccountID
        {
            get { return nSubAccountID; }
        }

        /// <summary>
        /// The element type for this class
        /// </summary>
        public SpendManagementElement element
        {
            get { return eElement; }
        }

        /// <summary>
        /// Value of the cache dependency SQL for the definition type
        /// </summary>
        public string CacheDepSQL
        {
            get { return sCacheDepSQL; }
        }

        /// <summary>
        /// Value of the save stored procedure for the definition type
        /// </summary>
        public string SaveStoredProcName
        {
            get { return sSaveStoredProcName; }
        }

        /// <summary>
        /// Value of the delete stored procedure for the definition type
        /// </summary>
        public string DeleteStoredProcName
        {
            get { return sDeleteStoredProcName; }
        }

        #endregion

        /// <summary>
        /// Set the value of the properties depending on the element definition type
        /// </summary>
        private void setProperties()
        {
            cAccountSubAccounts subaccs = new cAccountSubAccounts(AccountID);
            cAccountProperties properties = subaccs.getSubAccountById(SubAccountID).SubAccountProperties;
            sRenameTo = "";
            sRenameFrom = "";

            switch (element)
            {
                #region Set the property for the base definition type

                case SpendManagementElement.ContractCategories:
                    sCacheKey = "ContractCategories";
                    sSaveStoredProcName = "saveContractCategory";
                    sDeleteStoredProcName = "deleteContractCategory";
                    sCacheDepSQL = "select categoryId, categoryDescription, createdOn, createdBy, modifiedOn, modifiedBy, archived from dbo.codes_contractcategory where subAccountId = @subAccountID";
                    break;
                case SpendManagementElement.ContractStatus:
                    sCacheKey = "ContractStatuses";
                    sSaveStoredProcName = "saveContractStatus";
                    sDeleteStoredProcName = "deleteContractStatus";
                    sCacheDepSQL = "select statusId, statusDescription, isArchive, archived, createdOn, createdBy, modifiedOn, modifiedBy from dbo.codes_contractstatus where subAccountId = @subAccountID";
                    break;
                case SpendManagementElement.ContractTypes:
                    sCacheKey = "ContractTypes";
                    sSaveStoredProcName = "saveContractType";
                    sDeleteStoredProcName = "deleteContractType";
                    sCacheDepSQL = "select contractTypeId, contractTypeDescription, financialContract, archived, createdOn, createdBy, modifiedOn, modifiedBy from dbo.codes_contracttype where subAccountId = @subAccountID";
                    break;
                case SpendManagementElement.FinancialStatus:
                    sCacheKey = "FinancialStatuses";
                    sSaveStoredProcName = "saveFinancialStatus";
                    sDeleteStoredProcName = "deleteFinancialStatus";
                    sCacheDepSQL = "SELECT statusid, subaccountid, description, createdon, createdby, modifiedon, modifiedby, archived FROM dbo.financial_status  where subAccountId = @subAccountID";
                    break;
                case SpendManagementElement.TermTypes:
                    sCacheKey = "TermTypes";
                    sSaveStoredProcName = "saveTermType";
                    sDeleteStoredProcName = "deleteTermType";
                    sCacheDepSQL = "select termTypeId, termTypeDescription, archived, createdOn, createdBy, modifiedOn, modifiedBy from dbo.codes_termtype where subAccountId = @subAccountID";
                    break;
                case SpendManagementElement.InflatorMetrics:
                    sCacheKey = "InflatorMetrics";
                    sSaveStoredProcName = "saveInflatorMetric";
                    sDeleteStoredProcName = "deleteInflatorMetric";
                    sCacheDepSQL = "select metricId, name, percentage, requiresExtraPct, archived, createdOn, createdBy, modifiedOn, modifiedBy from dbo.codes_inflatormetrics where subAccountId = @subAccountID";
                    break;
                case SpendManagementElement.InvoiceFrequencyTypes:
                    sCacheKey = "InvoiceFrequencyTypes";
                    sSaveStoredProcName = "saveInvoiceFrequencyType";
                    sDeleteStoredProcName = "deleteInvoiceFrequencyType";
                    sCacheDepSQL = "select [invoiceFrequencyTypeId], [invoiceFrequencyTypeDesc], [frequencyInMonths], [archived], createdon, createdby, modifiedon, modifiedby from dbo.codes_invoicefrequencytype where subAccountId = @subAccountID";
                    break;
                case SpendManagementElement.InvoiceStatus:
                    sCacheKey = "InvoiceStatuses";
                    sSaveStoredProcName = "saveInvoiceStatusType";
                    sDeleteStoredProcName = "deleteInvoiceStatusType";
                    sCacheDepSQL = "select [invoiceStatusTypeID], [description], isArchive, archived, createdon, createdby, modifiedon, modifiedby from dbo.invoiceStatusType where subAccountId = @subAccountID";
                    break;
                case SpendManagementElement.TaskTypes:
                    sCacheKey = "TaskTypes";
                    sSaveStoredProcName = "saveTaskType";
                    sDeleteStoredProcName = "deleteTaskType";
                    sCacheDepSQL = "SELECT [typeId], [typeDescription], archived, createdon, createdby, modifiedon, modifiedby FROM dbo.codes_tasktypes  where subAccountId = @subAccountID";
                    break;
                case SpendManagementElement.Units:
                    sCacheKey = "Units";
                    sSaveStoredProcName = "saveUnit";
                    sDeleteStoredProcName = "deleteUnit";
                    sCacheDepSQL = "select [unitId], [description], archived, createdon, createdby, modifiedon, modifiedby from dbo.codes_units where subAccountId = @subAccountID";
                    break;
                case SpendManagementElement.ProductCategories:
                    sCacheKey = "ProductCategories";
                    sSaveStoredProcName = "saveProductCategory";
                    sDeleteStoredProcName = "deleteProductCategory";
                    sCacheDepSQL = "SELECT [CategoryId],[Description],archived, createdon, createdby,modifiedon,modifiedby FROM dbo.productCategories where subAccountId = @subAccountID";
                    break;
                case SpendManagementElement.SubAccounts:
                    sCacheKey = "SubAccounts";
                    sSaveStoredProcName = "saveSubAccount";
                    sDeleteStoredProcName = "deleteSubAccount";
                    sCacheDepSQL = "SELECT subAccountId, description, archived, createdon, createdby, modifiedon, modifiedby FROM dbo.accountsSubAccounts";
                    break;
                case SpendManagementElement.LicenceRenewalTypes:
                    sCacheKey = "LicenceRenewalTypes";
                    sSaveStoredProcName = "saveLicenceRenewalType";
                    sDeleteStoredProcName = "deleteLicenceRenewalType";
                    sCacheDepSQL = "SELECT [renewalType],description, archived, createdon, createdby, modifiedon, modifiedby FROM dbo.licenceRenewalTypes where subAccountId = @subAccountID";
                    break;
                case SpendManagementElement.SupplierStatus:
                    sCacheKey = "SupplierStatuses";
                    sSaveStoredProcName = "saveSupplierStatus";
                    sDeleteStoredProcName = "deleteSupplierStatus";
                    sCacheDepSQL = "SELECT statusid, subaccountid, description, sequence, deny_contract_add, archived, createdon, createdby, modifiedon, modifiedby FROM dbo.supplier_status where subAccountId = @subAccountID";
                    sRenameFrom = "Supplier";
                    sRenameTo = properties.SupplierPrimaryTitle;
                    break;
                case SpendManagementElement.SupplierCategory:
                    sCacheKey = "SupplierCategories";
                    sSaveStoredProcName = "saveSupplierCategory";
                    sDeleteStoredProcName = "deleteSupplierCategory";
                    sCacheDepSQL = "SELECT categoryid, subaccountid, description, createdon, createdby, modifiedon, modifiedby, archived FROM dbo.supplier_categories where subAccountId = @subAccountID";
                    sRenameFrom = "Supplier Category";
                    sRenameTo = properties.SupplierCatTitle;
                    break;
                case SpendManagementElement.ProductLicenceTypes:
                    sCacheKey = "ProductLicenceTypes";
                    sSaveStoredProcName = "saveProductLicenceType";
                    sDeleteStoredProcName = "deleteProductLicenceType";
                    sCacheDepSQL = "select licenceTypeId, description, createdon, createdby, modifiedon, modifiedby, archived from dbo.codes_licenceTypes where subAccountId = @subAccountID";
                    break;
                case SpendManagementElement.SalesTax:
                    sCacheKey = "SalesTax";
                    sSaveStoredProcName = "saveSalesTax";
                    sDeleteStoredProcName = "deleteSalesTax";
                    sCacheDepSQL = "SELECT salesTaxID, description, salesTax, archived, createdOn, createdBy, modifiedOn, modifiedBy FROM dbo.codes_salesTax where subAccountId = @subAccountID";
                    break;
                default:
                    sCacheKey = "";
                    sSaveStoredProcName = "";
                    sDeleteStoredProcName = "";
                    break;

                #endregion
            }

            if (!string.IsNullOrEmpty(sCacheDepSQL))
                sCacheDepSQL += " AND " + AccountID.ToString() + " = " + AccountID.ToString();

            sCacheKey += "_" + AccountID + "_" + SubAccountID;
        }

        /// <summary>
        /// Get the class type for the base definition element
        /// </summary>
        /// <returns></returns>
        private Type getBaseDefinitionObjectType()
        {
            Type t = null;
                
            switch (element)
            {
                case SpendManagementElement.ContractStatus:
                    t  = typeof(cContractStatus);
                    break;
                case SpendManagementElement.ContractCategories:
                    t = typeof(cContractCategory);
                    break;
                case SpendManagementElement.ContractTypes:
                    t = typeof(cContractType);
                    break;
                case SpendManagementElement.InvoiceFrequencyTypes:
                    t = typeof(cInvoiceFrequencyType);
                    break;
                case SpendManagementElement.InvoiceStatus:
                    t = typeof(cInvoiceStatus);
                    break;
                case SpendManagementElement.LicenceRenewalTypes:
                    t = typeof(cLicenceRenewalType);
                    break;
                case SpendManagementElement.InflatorMetrics:
                    t = typeof(cInflatorMetric);
                    break;
                case SpendManagementElement.TermTypes:
                    t = typeof(cTermType);
                    break;
                case SpendManagementElement.FinancialStatus:
                    t = typeof(cFinancialStatus);
                    break;
                case SpendManagementElement.TaskTypes:
                    t = typeof(cTaskType);
                    break;
                case SpendManagementElement.Units:
                    t = typeof(cUnit);
                    break;
                case SpendManagementElement.ProductCategories:
                    t = typeof(cProductCategory);
                    break;
                case SpendManagementElement.SupplierStatus:
                    t = typeof(cSupplierStatus);
                    break;
                case SpendManagementElement.SupplierCategory:
                    t = typeof(cSupplierCategory);
                    break;
                case SpendManagementElement.ProductLicenceTypes:
                    t = typeof(cProductLicenceType);
                    break;
                case SpendManagementElement.SalesTax:
                    t = typeof(cSalesTax);
                    break;
            }

            return t;
        }

        /// <summary>
        /// Populate collection from cache (or database)
        /// </summary>
        private void initialiseData()
        {
            var tmpList = _cache.Get(AccountID, CacheKey, sCacheKey) as Dictionary<int, cBaseDefinition>;
            bdList = tmpList ?? cacheDefinitions();
        }

        /// <summary>
        /// Cache the specified base definition type 
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, cBaseDefinition> cacheDefinitions()
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            Dictionary<int, cBaseDefinition> lstDefinitions = new Dictionary<int, cBaseDefinition>();

            int ID;
            string Description;
            DateTime? CreatedOn;
            int? CreatedBy;
            DateTime? ModifiedOn;
            int? ModifiedBy;
            bool archived;

            bool isArchived;
            int FrequencyInMonths;
            decimal Percentage;
            bool RequiresExtraPct;
            short Sequence;
            bool DenyContractAdd;
            decimal SalesTax;

            db.sqlexecute.Parameters.AddWithValue("@subAccountID", SubAccountID);
            db.sqlexecute.Parameters.AddWithValue("@element", element);

            using (SqlDataReader reader = db.GetStoredProcReader("GetBaseDefinition"))
            {
                while (reader.Read())
                {
                    #region Base Properties

                    ID = reader.GetInt32(0);
                    Description = reader.GetString(1);

                    if (reader.IsDBNull(2))
                    {
                        CreatedOn = null;
                    }
                    else
                    {
                        CreatedOn = reader.GetDateTime(2);
                    }

                    if (reader.IsDBNull(3))
                    {
                        CreatedBy = null;
                    }
                    else
                    {
                        CreatedBy = reader.GetInt32(3);
                    }

                    if (reader.IsDBNull(4))
                    {
                        ModifiedOn = null;
                    }
                    else
                    {
                        ModifiedOn = reader.GetDateTime(4);
                    }

                    if (reader.IsDBNull(5))
                    {
                        ModifiedBy = null;
                    }
                    else
                    {
                        ModifiedBy = reader.GetInt32(5);
                    }

                    archived = reader.GetBoolean(6);

                    #endregion

                    switch (element)
                    {
                        case SpendManagementElement.ContractStatus:
                            isArchived = reader.GetBoolean(7);
                            lstDefinitions.Add(ID, new cContractStatus(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, isArchived, archived));
                            break;
                        case SpendManagementElement.ContractCategories:
                            lstDefinitions.Add(ID, new cContractCategory(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, archived));
                            break;
                        case SpendManagementElement.ContractTypes:
                            lstDefinitions.Add(ID, new cContractType(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, archived));
                            break;
                        case SpendManagementElement.InvoiceFrequencyTypes:
                            FrequencyInMonths = reader.GetInt16(7);
                            lstDefinitions.Add(ID, new cInvoiceFrequencyType(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, archived, FrequencyInMonths));
                            break;
                        case SpendManagementElement.InvoiceStatus:
                            isArchived = reader.GetBoolean(7);
                            lstDefinitions.Add(ID, new cInvoiceStatus(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, isArchived, archived));
                            break;
                        case SpendManagementElement.LicenceRenewalTypes:
                            lstDefinitions.Add(ID, new cLicenceRenewalType(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, archived));
                            break;
                        case SpendManagementElement.InflatorMetrics:
                            Percentage = reader.GetDecimal(7);
                            RequiresExtraPct = reader.GetBoolean(8);
                            lstDefinitions.Add(ID, new cInflatorMetric(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, archived, Percentage, RequiresExtraPct));
                            break;
                        case SpendManagementElement.TermTypes:
                            lstDefinitions.Add(ID, new cTermType(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, archived));
                            break;
                        case SpendManagementElement.FinancialStatus:
                            lstDefinitions.Add(ID, new cFinancialStatus(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, archived));
                            break;
                        case SpendManagementElement.TaskTypes:
                            lstDefinitions.Add(ID, new cTaskType(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, archived));
                            break;
                        case SpendManagementElement.Units:
                            lstDefinitions.Add(ID, new cUnit(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, archived));
                            break;
                        case SpendManagementElement.ProductCategories:
                            lstDefinitions.Add(ID, new cProductCategory(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, archived));
                            break;
                        case SpendManagementElement.SupplierStatus:
                            Sequence = reader.GetInt16(7);
                            DenyContractAdd = reader.GetBoolean(8);
                            lstDefinitions.Add(ID, new cSupplierStatus(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, archived, Sequence, DenyContractAdd));
                            break;
                        case SpendManagementElement.SupplierCategory:
                            lstDefinitions.Add(ID, new cSupplierCategory(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, archived));
                            break;
                        case SpendManagementElement.ProductLicenceTypes:
                            lstDefinitions.Add(ID, new cProductLicenceType(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, archived));
                            break;
                        case SpendManagementElement.SalesTax:
                            SalesTax = reader.GetDecimal(7);
                            lstDefinitions.Add(ID, new cSalesTax(ID, Description, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, archived, SalesTax));
                            break;
                    }
                }

                reader.Close();
            }

            _cache.Add(AccountID, CacheKey, sCacheKey, lstDefinitions);

            return lstDefinitions;
        }

        /// <summary>
        /// Force an update of the cache for this base definition
        /// </summary>
        private void resetCache()
        {
            _cache.Delete(AccountID, CacheKey, sCacheKey);
            bdList = null;
            initialiseData();
        }

        /// <summary>
        /// Get the base definition object by its ID
        /// </summary>
        /// <param name="RecordID"></param>
        /// <returns></returns>
        public cBaseDefinition GetDefinitionByID(int RecordID)
        {
            cBaseDefinition def = null;
            bdList.TryGetValue(RecordID, out def);
            return def;
        }

        /// <summary>
        /// Get the values for the base definition so they can output onto the screen
        /// </summary>
        /// <param name="RecordID"></param>
        /// <param name="fieldIDs"></param>
        /// <returns></returns>
        public cBaseDefinitionValues[] GetBaseDefinitionRecord(int RecordID, Guid[] fieldIDs)
        {
            cFields fields = new cFields(AccountID);
            List<cBaseDefinitionValues> retValues = new List<cBaseDefinitionValues>();
            cField field = null;
            string dataValue;
            cBaseDefinition def = null;
            PropertyInfo[] piList = null;

            //if editing then need to compare the definition properties to set the values against
            if (RecordID > -1)
            {
                def = GetDefinitionByID(RecordID);

                if (def != null)
                {
                    piList = def.GetType().GetProperties();
                }
            }
   
            foreach (Guid uID in fieldIDs)
            {
                field = fields.GetFieldByID(uID);

                //adding a new base definition
                if (RecordID == -1)
                {
                    retValues.Add(new cBaseDefinitionValues(field.FieldName, field.GetParentTable().TableName, field.TableID.ToString(), field.FieldType, field.GenList, field.ValueList, field.FieldID.ToString(), ""));
                }
                else //editing a base definition
                {
                    if (def != null)
                    {
                        foreach (PropertyInfo info in piList)
                        {
                            if (info.CanRead)
                            {
                                if (field.ClassPropertyName == info.Name)
                                {
                                    dataValue = info.GetValue(def, null).ToString();

                                    retValues.Add(new cBaseDefinitionValues(field.FieldName, field.GetParentTable().TableName, field.TableID.ToString(), field.FieldType, field.GenList, field.ValueList, field.FieldID.ToString(), dataValue));
                                }
                            }
                        }
                    }
                }
            }

            return retValues.ToArray();
        }

        /// <summary>
        /// Set the columns used to create the forms and grid on the basedefinitions.aspx page
        /// </summary>
        /// <param name="baseTableID"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public List<cField> SetBaseDefinitionFields(Guid baseTableID, ref List<cNewGridColumn> columns)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            List<cField> bdColumns = new List<cField>();
            columns = new List<cNewGridColumn>();

            if (baseTableID != Guid.Empty)
            {
                cFields fields = new cFields(AccountID);

                cField tmpField = null;

                Type t = getBaseDefinitionObjectType();

                if (t != null)
                {
                    PropertyInfo[] piList = t.GetProperties();

                    foreach (PropertyInfo pi in piList)
                    {
                        if (pi.CanRead)
                        {
                            bool skipColumnAdd = false;
                            if (pi.Name.ToLower() != "createdby" && pi.Name.ToLower() != "createdon" && pi.Name.ToLower() != "modifiedby" && pi.Name.ToLower() != "modifiedon")
                            {
                                tmpField = fields.getFieldByTableAndClassPropertyName(baseTableID, pi.Name);

                                if (tmpField != null)
                                {
                                    if (sRenameTo != "")
                                    {
                                        tmpField.Description = tmpField.Description.Replace(sRenameFrom, sRenameTo);
                                    }

                                    if (pi.Name.ToLower() != "id" && pi.Name.ToLower() != "archived")
                                    {
                                        if (pi.Name.ToLower() == "description")
                                        {
                                            bdColumns.Insert(0, tmpField);
                                            columns.Insert(0, new cFieldColumn(tmpField)); // force description to be first column in grid
                                            skipColumnAdd = true;
                                        }
                                        else
                                        {
                                            bdColumns.Add(tmpField);
                                        }
                                    }

                                    if (!skipColumnAdd)
                                    {
                                        columns.Add(new cFieldColumn(tmpField));
                                    }

                                    #region Sub Account dropdown listitem creation

                                    //if (element == SpendManagementElement.SubAccounts)
                                    //{
                                    //    cAccountSubAccounts clsSubAccs = new cAccountSubAccounts(AccountID);
                                    //    ListItem[] items = clsSubAccs.CreateFilteredDropDown(curUser.EmployeeID, null);

                                    //    int keyIndex = 0;

                                    //    foreach (ListItem item in items)
                                    //    {
                                    //        keyIndex = 0;

                                    //        int.TryParse(item.Value.ToString(), out keyIndex);

                                    //        if (!tmpField.listitems.ContainsKey(keyIndex))
                                    //        {
                                    //            tmpField.listitems.Add(keyIndex, item.Text);
                                    //        }
                                    //    }
                                    //}

                                    #endregion
                                }
                            }
                        }
                    }
                }
                
            }
            return bdColumns;
        }

        /// <summary>
        /// Save the specific base definition to the database
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="defValues"></param>
        /// <returns></returns>
        public int SaveDefinition(int ID, cBaseDefinitionValues[] defValues)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));

            db.sqlexecute.Parameters.Add("@defID", System.Data.SqlDbType.Int);
            db.sqlexecute.Parameters["@defID"].Direction = System.Data.ParameterDirection.ReturnValue;

            db.sqlexecute.Parameters.AddWithValue("@subAccountId", currentUser.CurrentSubAccountId);
            db.sqlexecute.Parameters.AddWithValue("@employeeId", currentUser.EmployeeID);
            db.sqlexecute.Parameters.AddWithValue("@ID", ID);           

            if (currentUser.Delegate == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }

            for (int x = 0; x < defValues.Length; x++)
            {
                if (defValues[x].fieldValue == "")
                {
                    switch (defValues[x].fieldType)
                    {
                        case "N":
                        case "I":
                        case "C":
                        case "F":
                        case "M":
                            defValues[x].fieldValue = "0";
                            break;
                        default:
                            defValues[x].fieldValue = "";
                            break;
                    }
                }
                // fieldName (spaces replace with "_") must match stored proc params for this to work (i.e. categoryId field match param @categoryId in SP)
                db.sqlexecute.Parameters.AddWithValue("@" + defValues[x].fieldName.Replace(" ", "_"), defValues[x].fieldValue);
            }

            int RecordID = -1;

            if (SaveStoredProcName != "")
            {
                db.ExecuteProc(SaveStoredProcName);
                RecordID = (int)db.sqlexecute.Parameters["@defID"].Value;
                db.sqlexecute.Parameters.Clear();
                resetCache();
            }

            return RecordID;
        }

        /// <summary>
        /// Delete the specific base definition from the database
        /// </summary>
        /// <param name="ID"></param>
        public int DeleteDefinition(int ID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            int returnVal = 0;

            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            db.sqlexecute.Parameters.AddWithValue("@ID", ID);   
            db.sqlexecute.Parameters.AddWithValue("@employeeId", user.EmployeeID);
            if (user.Delegate == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@delegateID", user.Delegate.EmployeeID);
            }
            db.sqlexecute.Parameters.Add("@returnVal", System.Data.SqlDbType.Int);
            db.sqlexecute.Parameters["@returnVal"].Direction = System.Data.ParameterDirection.ReturnValue; 

            if (DeleteStoredProcName != "")
            {
                db.ExecuteProc(DeleteStoredProcName);
                returnVal = (int)db.sqlexecute.Parameters["@returnVal"].Value;
                db.sqlexecute.Parameters.Clear();
                resetCache();
            }

            return returnVal;
        }

        /// <summary>
        /// Archive the base definition
        /// </summary>
        /// <param name="ID"></param>
        public void ArchiveDefinition(int ID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
            db.sqlexecute.Parameters.AddWithValue("@ID", ID);
            db.sqlexecute.Parameters.AddWithValue("@element", element);
            db.sqlexecute.Parameters.AddWithValue("@subAccountID", user.CurrentSubAccountId);
            db.sqlexecute.Parameters.AddWithValue("@employeeID", user.EmployeeID);
            if (user.Delegate == null)
            {
                db.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@delegateID", user.Delegate.EmployeeID);
            }

            db.ExecuteProc("ArchiveBaseDefinition");
            db.sqlexecute.Parameters.Clear();
            resetCache();
            
        }

        /// <summary>
        /// Create a list of the ListItems for the base definition
        /// </summary>
        /// <param name="selected_type"></param>
        /// <param name="addNoneEntry"></param>
        /// <returns></returns>
        public ListItem[] CreateDropDown(bool addNoneEntry, int SelectedID)
        {
            List<ListItem> items = new List<ListItem>();

            foreach (cBaseDefinition def in bdList.Values)
            {
                if (def.Archived)
                {
                    //If the archived definition is the selected value for the base definition then allow it to be added to the dropdown
                    if (SelectedID == def.ID)
                    {
                        items.Add(new ListItem(def.Description, def.ID.ToString()));
                    }
                }
                else
                {
                    items.Add(new ListItem(def.Description, def.ID.ToString()));
                }
            }

            //sort the text value in ascending order
            items.Sort(delegate(ListItem lt1, ListItem lt2)
            {
                return lt1.Text.CompareTo(lt2.Text);
            });

            if (addNoneEntry)
            {
                items.Insert(0, new ListItem("[None]", "0"));
            }

            return items.ToArray();
        }
    }
}
