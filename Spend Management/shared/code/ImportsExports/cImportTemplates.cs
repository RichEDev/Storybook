using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementLibrary;
using System.Data.SqlClient;
using System.Web.Caching;
using System.Xml.Linq;
using SpendManagementLibrary.ESRTransferServiceClasses;
using System.Configuration;

namespace Spend_Management
{
    public class cImportTemplates
    {
        private int nAccountID;
        private SortedList<int, cImportTemplate> list;

        private Cache Cache = (Cache)HttpRuntime.Cache;


        public cImportTemplates(int AccountID)
        {
            nAccountID = AccountID;

            InitialiseData();
        }

        #region Properties

        public int AccountID
        {
            get { return nAccountID; }
        }

        #endregion

        /// <summary>
        /// Initialise the Cache data
        /// </summary>
        private void InitialiseData()
        {
            list = (SortedList<int, cImportTemplate>)Cache["ImportTemplates" + AccountID];
            if (list == null)
            {
                list = CacheList();
            }
        }

        /// <summary>
        /// Clear the cache on the current server
        /// </summary>
        private void ClearCache()
        {
            Cache.Remove("ImportTemplates" + AccountID);
            list = null;
            InitialiseData();
        }

        /// <summary>
        /// Cache a list of cImportTemplate objects 
        /// </summary>
        /// <returns></returns>
        private SortedList<int, cImportTemplate> CacheList()
        {
            SortedList<int, cImportTemplate> lstTemplates = new SortedList<int, cImportTemplate>();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            int TemplateID;
            string TemplateName;
            ApplicationType AppType;
            bool IsAutomated;
            int NHSTrustID;
            DateTime createdOn, modifiedOn;
            int createdBy, modifiedBy;

            cESRTrusts trusts = new cESRTrusts(AccountID);

            SqlDataReader reader;

            const string strsql = "SELECT templateID, templateName, applicationType, isAutomated, NHSTrustID, SignOffOwnerFieldId, LineManagerFieldId, createdOn, createdBy, modifiedOn, modifiedBy FROM dbo.importTemplates";
            expdata.sqlexecute.CommandText = strsql;

            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);
                Cache.Insert("ImportTemplates" + AccountID, lstTemplates, dep, Cache.NoAbsoluteExpiration,
                    TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Medium), CacheItemPriority.Default, null);
            }

            using (reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    TemplateID = reader.GetInt32(reader.GetOrdinal("templateID"));
                    TemplateName = reader.GetString(reader.GetOrdinal("templateName"));
                    AppType = (ApplicationType)reader.GetByte(reader.GetOrdinal("applicationType"));
                    IsAutomated = reader.GetBoolean(reader.GetOrdinal("isAutomated"));

                    if (reader.IsDBNull(reader.GetOrdinal("NHSTrustID")))
                    {
                        NHSTrustID = 0;
                    }
                    else
                    {
                        NHSTrustID = reader.GetInt32(reader.GetOrdinal("NHSTrustID"));
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("createdOn")))
                    {
                        createdOn = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        createdOn = reader.GetDateTime(reader.GetOrdinal("createdOn"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("createdBy")))
                    {
                        createdBy = 0;
                    }
                    else
                    {
                        createdBy = reader.GetInt32(reader.GetOrdinal("createdBy"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedOn")))
                    {
                        modifiedOn = new DateTime(1900, 01, 01);
                    }
                    else
                    {
                        modifiedOn = reader.GetDateTime(reader.GetOrdinal("modifiedOn"));
                    }
                    if (reader.IsDBNull(reader.GetOrdinal("modifiedBy")))
                    {
                        modifiedBy = 0;
                    }
                    else
                    {
                        modifiedBy = reader.GetInt32(reader.GetOrdinal("modifiedBy"));
                    }

                    var signOffOwnerFieldId = (reader.IsDBNull(reader.GetOrdinal("SignOffOwnerFieldId"))
                        ? Guid.Empty
                        : reader.GetGuid(reader.GetOrdinal("SignOffOwnerFieldId")));

                    var lineManagerFieldId = (reader.IsDBNull(reader.GetOrdinal("LineManagerFieldId"))
                        ? Guid.Empty
                        : reader.GetGuid(reader.GetOrdinal("LineManagerFieldId")));

                    cESRTrust esrTrust = trusts.GetESRTrustByID(NHSTrustID);
                    byte esrVersionNumber = esrTrust == null ? (byte)1 : esrTrust.EsrInterfaceVersionNumber;

                    lstTemplates.Add(TemplateID, new cImportTemplate(TemplateID, TemplateName, AppType, IsAutomated, NHSTrustID, signOffOwnerFieldId,
                        lineManagerFieldId, this.GetImportTemplateMappings(TemplateID, AppType, esrVersionNumber), createdOn, createdBy, modifiedOn, modifiedBy));
                }

                reader.Close();
            }

            return lstTemplates;
        }

        /// <summary>
        /// Get all import template mappings from the database for the specific template ID
        /// </summary>
        /// <param name="templateID">Unique ID of the template</param>
        /// <param name="appType">Application type to obtain template mappings for</param>
        /// <param name="esrVersionNumber">Version number of the ESR Interface being used</param>
        /// <returns>A List of cImportTemplateMapping objects</returns>
        public List<cImportTemplateMapping> GetImportTemplateMappings(int templateID, ApplicationType appType, byte esrVersionNumber)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            List<cImportTemplateMapping> lstMappings = getDefaultTemplateMappings(appType, esrVersionNumber);

            int TemplateMappingID;
            Guid FieldID, LookupTable, MatchField;
            string DestinationField;
            int ColRef;
            ImportElementType ElementType;
            bool Mandatory, OverridePK, ImportField;
            DataType dataType;
            cTables clsTables = new cTables(AccountID);
            cFields clsfields = new cFields(AccountID);
            cField field;
            SqlDataReader reader;
            cImportTemplateMapping newMapping;

            const string strsql =
                "SELECT templateMappingID, fieldID, destinationField, columnRef, importElementType, mandatory, dataType, lookupTable, matchField, overridePrimaryKey, importField FROM importTemplateMappings WHERE templateID = @templateID";
            expdata.sqlexecute.Parameters.AddWithValue("@templateID", templateID);

            using (reader = expdata.GetReader(strsql))
            {
                #region Set Ordinals

                int tmappingId_Ord = reader.GetOrdinal("templateMappingID");
                int fieldId_Ord = reader.GetOrdinal("fieldID");
                int destField_Ord = reader.GetOrdinal("destinationField");
                int columnRef_Ord = reader.GetOrdinal("columnRef");
                int elementType_Ord = reader.GetOrdinal("importElementType");
                int mandatory_Ord = reader.GetOrdinal("mandatory");
                int dataType_Ord = reader.GetOrdinal("dataType");
                int lookupTable_Ord = reader.GetOrdinal("lookupTable");
                int matchField_Ord = reader.GetOrdinal("matchField");
                int overridePK_Ord = reader.GetOrdinal("overridePrimaryKey");
                int importField_Ord = reader.GetOrdinal("importField");

                #endregion

                expdata.sqlexecute.Parameters.Clear();

                while (reader.Read())
                {
                    TemplateMappingID = reader.GetInt32(tmappingId_Ord);
                    FieldID = reader.GetGuid(fieldId_Ord);
                    field = clsfields.GetFieldByID(FieldID);

                    DestinationField = reader.IsDBNull(destField_Ord) ? string.Empty : reader.GetString(destField_Ord);
                    ColRef = reader.IsDBNull(columnRef_Ord) ? 0 : reader.GetInt32(columnRef_Ord);
                    ElementType = (ImportElementType)reader.GetByte(elementType_Ord);
                    Mandatory = reader.GetBoolean(mandatory_Ord);
                    dataType = (DataType)reader.GetByte(dataType_Ord);
                    LookupTable = reader.IsDBNull(lookupTable_Ord) ? Guid.Empty : reader.GetGuid(lookupTable_Ord);
                    MatchField = reader.IsDBNull(matchField_Ord) ? Guid.Empty : reader.GetGuid(matchField_Ord);
                    OverridePK = reader.GetBoolean(overridePK_Ord);
                    ImportField = reader.GetBoolean(importField_Ord);

                    newMapping = new cImportTemplateMapping(
                        TemplateMappingID,
                        templateID,
                        FieldID,
                        field,
                        DestinationField,
                        ColRef,
                        ElementType,
                        Mandatory,
                        dataType,
                        clsTables.GetTableByID(LookupTable),
                        clsfields.GetFieldByID(MatchField),
                        OverridePK,
                        true,
                        true,
                        ImportField);

                    int mappingIndex = (from x in lstMappings where x.ElementType == ElementType && x.DestinationField == DestinationField select lstMappings.IndexOf(x)).FirstOrDefault();

                    if (mappingIndex > 0)
                    {
                        cImportTemplateMapping defaultMapping = lstMappings[mappingIndex];
                        newMapping.DefaultMappingField = defaultMapping.DefaultMappingField;
                        newMapping.DefaultMappingTable = defaultMapping.DefaultMappingTable;

                        lstMappings[mappingIndex] = newMapping;
                    }
                    else
                    {
                        lstMappings.Add(newMapping);
                    }
                }

                reader.Close();
            }

            return lstMappings;
        }

        /// <summary>
        /// Gets the default ESR Mappings from the XML template
        /// </summary>
        /// <param name="appType"></param>
        /// <param name="esrVersionNumber">Version of ESR Interface being used</param>
        /// <returns></returns>
        private List<cImportTemplateMapping> getDefaultTemplateMappings(ApplicationType appType, byte esrVersionNumber)
        {
            List<cImportTemplateMapping> lstMappings = new List<cImportTemplateMapping>();
            Dictionary<string, List<XMLMapFields>> lstXmlMappings = GetApplicationXMLMappings(appType, esrVersionNumber);
            cTables clstables = new cTables(AccountID);
            cFields clsfields = new cFields(AccountID);

            foreach (KeyValuePair<string, List<XMLMapFields>> kvp in lstXmlMappings)
            {
                ImportElementType elementType = GetElementType(kvp.Key);

                foreach (XMLMapFields mapField in kvp.Value)
                {
                    cTable lookupTable = null;
                    cField refField = null;
                    Guid refFieldId = Guid.Empty;
                    cField matchField = null;

                    if (!string.IsNullOrEmpty(mapField.referenceTable))
                    {
                        cTable refTable = clstables.GetTableByName(mapField.referenceTable);

                        if (refTable != null && !string.IsNullOrEmpty(mapField.referenceField))
                        {
                            refField = clsfields.GetCustomFieldByTableAndFieldName(refTable.TableID, mapField.referenceField);
                            refFieldId = refField.FieldID;
                        }
            }

                    if (!string.IsNullOrEmpty(mapField.lookupTable))
                    {
                        lookupTable = clstables.GetTableByName(mapField.lookupTable);

                        if (lookupTable != null && !string.IsNullOrEmpty(mapField.matchField))
                        {
                            matchField = clsfields.GetBy(lookupTable.TableID, mapField.matchField);
                        }
        }

                    cImportTemplateMapping mapping = new cImportTemplateMapping(0, 0, refFieldId, refField, mapField.DestinationField, mapField.ColRef, elementType, mapField.Mandatory, mapField.dataType, lookupTable, matchField, mapField.overridePrimaryKey, mapField.populated, esrVersionNumber == 1 || mapField.allowDynamicMapping, mapField.importField);

                    lstMappings.Add(mapping);
                }
            }

            return lstMappings;
        }

        /// <summary>
        /// Converts a string into it's equivalent ImportElementType, otherwise returns type None
        /// </summary>
        /// <param name="elementName">String name to attempt to convert into an ImportElementType enumeration</param>
        /// <returns>ImportElementType enumeration</returns>
        public static ImportElementType GetElementType(string elementName)
        {
            ImportElementType retType;
            if (ImportElementType.TryParse(elementName, true, out retType))
            {
                return retType;
            }

            return ImportElementType.None;
        }

        /// <summary>
        /// Save the import template to the database
        /// </summary>
        /// <param name="template">Template object</param>
        /// <returns>The ID of the saved template</returns>
        public int saveImportTemplate(cImportTemplate template)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            int TemplateID;

            expdata.sqlexecute.Parameters.AddWithValue("@templateID", template.TemplateID);
            expdata.sqlexecute.Parameters.AddWithValue("@templateName", template.TemplateName);
            expdata.sqlexecute.Parameters.AddWithValue("@applicationType", template.appType);
            expdata.sqlexecute.Parameters.AddWithValue("@isAutomated", template.IsAutomated);
            expdata.sqlexecute.Parameters.AddWithValue("@NHSTrustID", template.NHSTrustID);
            expdata.sqlexecute.Parameters.AddWithValue("@SignOffOwnerFieldId", (template.SignOffOwnerFieldId == Guid.Empty ? (object)DBNull.Value : template.SignOffOwnerFieldId));
            expdata.sqlexecute.Parameters.AddWithValue("@LineManagerFieldId", (template.LineManagerFieldId == Guid.Empty ? (object)DBNull.Value : template.LineManagerFieldId));

            if (template.TemplateID == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", template.createdOn);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", template.createdBy);

                expdata.sqlexecute.Parameters.Add("@returnvalue", System.Data.SqlDbType.Int);
                expdata.sqlexecute.Parameters["@returnvalue"].Direction = System.Data.ParameterDirection.ReturnValue;
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", template.modifiedOn);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", template.modifiedBy);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            expdata.ExecuteProc("dbo.saveImportTemplate");

            if (template.TemplateID == 0)
            {
                TemplateID = (int)expdata.sqlexecute.Parameters["@returnvalue"].Value;
            }
            else
            {
                TemplateID = template.TemplateID;
            }

            expdata.sqlexecute.Parameters.Clear();

            //Delete the template mappings
            expdata.sqlexecute.Parameters.AddWithValue("@templateID", TemplateID);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            expdata.ExecuteProc("dbo.deleteImportTemplateMappings");

            //Save the mappings for the template
            saveImportTemplateMappings(TemplateID, template.Mappings);

            ClearCache();

            return TemplateID;
        }

        /// <summary>
        /// Save the mappings for the specific import template to the database
        /// </summary>
        /// <param name="TemplateID">Unique ID of the Template</param>
        /// <param name="lstMappings">List of cImportTemplateMapping objects</param>
        public void saveImportTemplateMappings(int TemplateID, List<cImportTemplateMapping> lstMappings)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));

            foreach(cImportTemplateMapping mapping in lstMappings)
            {
                if (!mapping.Populated)
                {
                    continue;
                }

                expdata.sqlexecute.Parameters.AddWithValue("@templateID", TemplateID);
                expdata.sqlexecute.Parameters.AddWithValue("@templateMappingID", mapping.TemplateMappingID);
                expdata.sqlexecute.Parameters.AddWithValue("@fieldID", mapping.FieldID);
                expdata.sqlexecute.Parameters.AddWithValue("@destinationField", mapping.DestinationField);
                expdata.sqlexecute.Parameters.AddWithValue("@columnRef", mapping.ColRef);
                expdata.sqlexecute.Parameters.AddWithValue("@importElementType", mapping.ElementType);
                expdata.sqlexecute.Parameters.AddWithValue("@mandatory", mapping.Mandatory);
                expdata.sqlexecute.Parameters.AddWithValue("@dataType", mapping.dataType);
                if (mapping.LookupTable == null)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@lookupTable", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@lookupTable", mapping.LookupTable.TableID);
                }

                if (mapping.MatchField == null)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@matchField", DBNull.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@matchField", mapping.MatchField.FieldID);    
                }
                
                expdata.sqlexecute.Parameters.AddWithValue("@overridePrimaryKey", mapping.OverridePrimaryKey);
                expdata.sqlexecute.Parameters.AddWithValue("@importField", mapping.ImportField);

                CurrentUser currentUser = cMisc.GetCurrentUser();
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }

                expdata.ExecuteProc("dbo.saveImportTemplateMapping");

                expdata.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// Delete the import template from the database. All mappings will remove on a cascade delete
        /// </summary>
        /// <param name="TemplateID">ID of the template</param>
        public void deleteImportTemplate(int TemplateID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            expdata.sqlexecute.Parameters.AddWithValue("@templateID", TemplateID);

            CurrentUser currentUser = cMisc.GetCurrentUser();
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            expdata.ExecuteProc("dbo.deleteImportTemplate");
            expdata.sqlexecute.Parameters.Clear();
            ClearCache();
        }

        /// <summary>
        /// Get the cImportTemplate object by its ID
        /// </summary>
        /// <param name="TemplateID"></param>
        /// <returns></returns>
        public cImportTemplate getImportTemplateByID(int TemplateID)
        {
            cImportTemplate template = null;
            list.TryGetValue(TemplateID, out template);
            return template;
        }

        /// <summary>
        /// Read the XML field mapping for the passed in third party application
        /// </summary>
        /// <param name="AppType">Third Party Application Type</param>
        /// <param name="esrVersion">ESR Interface version</param>
        /// <returns>A list of all mappings for creating the import template</returns>
        public Dictionary<string, List<XMLMapFields>> GetApplicationXMLMappings(ApplicationType AppType, byte esrVersion = 1)
        {
            XName name;
            Dictionary<string, List<XMLMapFields>> lstMappings = new Dictionary<string, List<XMLMapFields>>();
            string xmlFilename = string.Format(@"\ESROutboundMappings{0}.xml", (esrVersion == 1 ? string.Empty : "_v" + esrVersion.ToString()));

            XDocument XMLTemplate;

            if (HttpContext.Current != null)
            {
                XMLTemplate = XDocument.Load(HttpContext.Current.Server.MapPath(@"\XMLImportMappings" + xmlFilename));
            }
            else
            {
                string mappingsPath = ConfigurationManager.AppSettings["ImplementationSpreadsheetXMLTemplatePath"];

                XMLTemplate = XDocument.Load(mappingsPath + (mappingsPath.EndsWith("\\") ? xmlFilename.Substring(1) : xmlFilename));
            }

            switch (AppType)
            {
                case ApplicationType.ESROutboundImport:
                    lstMappings.Add("Employee", ReadESROutboundXMLTemplateElement(XMLTemplate.Root.Element(XName.Get("Person")), 0));
                    lstMappings.Add("Assignment", ReadESROutboundXMLTemplateElement(XMLTemplate.Root.Element(XName.Get("Assignment")), 0));
                    if (esrVersion > 1)
                    {
                        lstMappings.Add("Location", ReadESROutboundXMLTemplateElement(XMLTemplate.Root.Element(XName.Get("Location")), 0));
                        lstMappings.Add("Organisation", ReadESROutboundXMLTemplateElement(XMLTemplate.Root.Element(XName.Get("Organisation")), 0));
                        lstMappings.Add("Position", ReadESROutboundXMLTemplateElement(XMLTemplate.Root.Element(XName.Get("Position")), 0));
                        lstMappings.Add("Phone", ReadESROutboundXMLTemplateElement(XMLTemplate.Root.Element(XName.Get("Phone")), 0));
                        lstMappings.Add("Address", ReadESROutboundXMLTemplateElement(XMLTemplate.Root.Element(XName.Get("Address")), 0));
                        lstMappings.Add("Vehicle", ReadESROutboundXMLTemplateElement(XMLTemplate.Root.Element(XName.Get("Vehicle")), 0));
                        lstMappings.Add("Costing", ReadESROutboundXMLTemplateElement(XMLTemplate.Root.Element(XName.Get("Costing")), 0));
                    }
                    break;
            }

            return lstMappings;
        }

        /// <summary>
        /// Read all child elements from the passed in parent element
        /// </summary>
        /// <param name="element">Parent XML element</param>
        /// <param name="depth">The depth to search for child elements</param>
        /// <returns></returns>
        public List<XMLMapFields> ReadESROutboundXMLTemplateElement(XElement element, int depth)
        {
            XAttribute attribute;

            List<XMLMapFields> lstXMLMaps = new List<XMLMapFields>();
            XMLMapFields ESRMap;
            bool Mandatory = false;
            bool Populated = true;
            bool OverridePK = false;
            bool AllowDynamicMapping = false;
            string DefaultTableName = string.Empty;
            string RelatedExpensesTable = string.Empty;

            attribute = element.Attribute(XName.Get("DefaultTableName"));
            if (attribute != null)
            {
                DefaultTableName = attribute.Value;
            }

            attribute = element.Attribute(XName.Get("RelatedExpensesTable"));
            if (attribute != null)
            {
                RelatedExpensesTable = attribute.Value;
            }

            if (element.HasElements)
            {
                foreach (var child in element.Elements())
                {
                    ESRMap = new XMLMapFields();
                    //Destination Field
                    attribute = child.Attribute(XName.Get("DestinationField"));
                    ESRMap.DestinationField = attribute.Value;

                    //Column Reference
                    attribute = child.Attribute(XName.Get("ColumnRef"));
                    ESRMap.ColRef = int.Parse(attribute.Value);

                    //Mandatory
                    attribute = child.Attribute(XName.Get("Mandatory"));
                    bool.TryParse(attribute.Value.ToLower(), out Mandatory);
                    ESRMap.Mandatory = Mandatory; 

                    //DataType
                    attribute = child.Attribute(XName.Get("DataType"));
                    switch (attribute.Value)
                    {
                        case "B":
                            ESRMap.dataType = DataType.longVal;
                            break;
                        case "N":
                            ESRMap.dataType = DataType.intVal;
                            break;
                        case "S":
                            ESRMap.dataType = DataType.stringVal;
                            break;
                        case "DT":
                            ESRMap.dataType = DataType.dateVal;
                            break;
                        case "D":
                            ESRMap.dataType = DataType.decimalVal;
                            break;
                        case "X":
                            ESRMap.dataType = DataType.booleanVal;
                            break;
                        case "L":
                            ESRMap.dataType = DataType.referenceLookup;
                            attribute = child.Attribute(XName.Get("LookupTable"));
                            ESRMap.lookupTable = attribute.Value;
                            attribute = child.Attribute(XName.Get("MatchField"));
                            ESRMap.matchField = attribute.Value;
                            if (child.Attribute(XName.Get("OverridePrimaryKey")) != null)
                            {
                                attribute = child.Attribute(XName.Get("OverridePrimaryKey"));
                                bool.TryParse(attribute.Value.ToLower(), out OverridePK);
                                ESRMap.overridePrimaryKey = OverridePK;
                            }

                            break;
                        case "LL":
                            ESRMap.dataType = DataType.longLookup;
                            attribute = child.Attribute(XName.Get("LookupTable"));
                            ESRMap.lookupTable = attribute.Value;
                            attribute = child.Attribute(XName.Get("MatchField"));
                            ESRMap.matchField = attribute.Value;
                            if (child.Attribute(XName.Get("OverridePrimaryKey")) != null)
                            {
                                attribute = child.Attribute(XName.Get("OverridePrimaryKey"));
                                bool.TryParse(attribute.Value.ToLower(), out OverridePK);
                                ESRMap.overridePrimaryKey = OverridePK;
                            }

                            break;
                        case "F":
                            ESRMap.dataType = DataType.floatVal;
                            break;
                    }

                    // Reference Table
                    attribute = child.Attribute(XName.Get("ReferenceTable"));
                    ESRMap.referenceTable = attribute.Value;

                    ESRMap.defaultTableName = DefaultTableName;
                    ESRMap.relatedExpensesTable = RelatedExpensesTable;

                    attribute = child.Attribute(XName.Get("DefaultField"));
                    if (attribute != null)
                    {
                        ESRMap.defaultFieldName = attribute.Value;
                    }
                    
                    // Reference Field
                    attribute = child.Attribute(XName.Get("ReferenceField"));
                    ESRMap.referenceField = attribute.Value;

                    // Populated
                    attribute = child.Attribute(XName.Get("Populated"));
                    if (attribute != null)
                    {
                        bool.TryParse(attribute.Value.ToLower(), out Populated);
                    }
                    ESRMap.populated = Populated;

                    attribute = child.Attribute(XName.Get("AllowDynamicMapping"));
                    if (attribute != null)
                    {
                        bool.TryParse(attribute.Value.ToLower(), out AllowDynamicMapping);
                    }
                    ESRMap.allowDynamicMapping = AllowDynamicMapping;
                    ESRMap.importField = ESRMap.populated; // set to true initially if populated

                    lstXMLMaps.Add(ESRMap);
                }
            }

            return lstXMLMaps;
        }

        /// <summary>
        /// A call from the ESR Windows Service will chack to see if an automated import exists for the detected file it has received. 
        /// If a template does exist then its template ID is returned so it can be used for the import
        /// </summary>
        /// <param name="NHSTrustID">Unique ID of the trust</param>
        /// <returns>The template ID for the import</returns>
        public int checkExistenceOfESRAutomatedTemplate(int NHSTrustID)
        {
            foreach (cImportTemplate temp in list.Values)
            {
                if (temp.appType != ApplicationType.ExcelImport)
                {
                    if (temp.IsAutomated && temp.NHSTrustID == NHSTrustID)
                    {
                        return temp.TemplateID;
                    }
                }
            }

            return 0;
        }

        
        /// <summary>
        /// Validate the ESR Outbound file before allowing an import
        /// </summary>
        /// <param name="TemplateID"></param>
        /// <param name="dataID"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public OutboundStatus processESROutboundImport(int TemplateID,int dataID, byte[] data)
        {
            OutboundStatus status = OutboundStatus.None;
            var user = cMisc.GetCurrentUser(string.Format("{0}, 0", this.AccountID));
            cESRImport import = new cESRImport(user, TemplateID, data);

            bool valid = import.validateImport(dataID);

            if (valid)
            {
                status = import.importOutboundData(dataID);
            }
            else
            {
                status = OutboundStatus.ValidationFailed;
            }

            return status;
        }

        public SortedList<int, cImportTemplate> listImportTemplates()
        {
            return list;
        }
    }
}
