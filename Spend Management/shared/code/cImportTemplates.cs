using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpendManagementLibrary;
using System.Data.SqlClient;
using System.Web.Caching;
using System.Xml.Linq;

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

            SqlDataReader reader;

            string strsql = "SELECT templateID, templateName, applicationType, isAutomated, NHSTrustID FROM dbo.importTemplates";
            expdata.sqlexecute.CommandText = strsql;
            SqlCacheDependency dep = new SqlCacheDependency(expdata.sqlexecute);

            List<cImportTemplateMapping> lstMappings = new List<cImportTemplateMapping>();
            
            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                TemplateID = reader.GetInt32(reader.GetOrdinal("templateID"));
                TemplateName = reader.GetString(reader.GetOrdinal("templateName"));
                AppType = (ApplicationType)reader.GetByte(reader.GetOrdinal("applicationType"));
                IsAutomated = reader.GetBoolean(reader.GetOrdinal("isAutomated"));

                if (reader.IsDBNull(reader.GetOrdinal("NHSTrustID")) == true)
                {
                    NHSTrustID = 0;
                }
                else
                {
                    NHSTrustID = reader.GetInt32(reader.GetOrdinal("NHSTrustID"));
                }

                 if (reader.IsDBNull(reader.GetOrdinal("createdOn")) == true)
                {
                    createdOn = new DateTime(1900, 01, 01);
                }
                else
                {
                    createdOn = reader.GetDateTime(reader.GetOrdinal("createdOn"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdBy")) == true)
                {
                    createdBy = 0;
                }
                else
                {
                    createdBy = reader.GetInt32(reader.GetOrdinal("createdBy"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedOn")) == true)
                {
                    modifiedOn = new DateTime(1900, 01, 01);
                }
                else
                {
                    modifiedOn = reader.GetDateTime(reader.GetOrdinal("modifiedOn"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("modifiedBy")) == true)
                {
                    modifiedBy = 0;
                }
                else
                {
                    modifiedBy = reader.GetInt32(reader.GetOrdinal("modifiedBy"));
                }

                lstTemplates.Add(TemplateID, new cImportTemplate(TemplateID, TemplateName, AppType, IsAutomated, NHSTrustID, getImportTemplateMappings(TemplateID), createdOn, createdBy, modifiedOn, modifiedBy));
            }

            reader.Close();

            Cache.Insert("ImportTemplates" + AccountID, lstTemplates, dep, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30), System.Web.Caching.CacheItemPriority.NotRemovable, null);

            return lstTemplates;
        }

        /// <summary>
        /// Get all import template mappings from the database for the specific template ID
        /// </summary>
        /// <param name="TemplateID">Unique ID of the template</param>
        /// <returns>A List of cImportTemplateMapping objects</returns>
        public List<cImportTemplateMapping> getImportTemplateMappings(int TemplateID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            List<cImportTemplateMapping> lstMappings = new List<cImportTemplateMapping>();

            int TemplateMappingID;
            Guid BaseTableID, FieldID;
            string DestinationField;
            int ColRef;
            ImportElementType ElementType;
            bool Mandatory;
            DataType dataType;
            string ReferenceTable;
            string ReferenceField;

            SqlDataReader reader;

            string strsql = "SELECT * FROM importTemplateMappings WHERE templateID = @templateID";
            expdata.sqlexecute.Parameters.AddWithValue("@templateID", TemplateID);
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();

            while (reader.Read())
            {
                TemplateMappingID = reader.GetInt32(reader.GetOrdinal("templateMappingID"));
                BaseTableID = reader.GetGuid(reader.GetOrdinal("baseTableID"));
                FieldID = reader.GetGuid(reader.GetOrdinal("fieldID"));

                if (reader.IsDBNull(reader.GetOrdinal("destinationField")) == true)
                {
                    DestinationField = "";
                }
                else
                {
                    DestinationField = reader.GetString(reader.GetOrdinal("destinationField"));
                }

                if (reader.IsDBNull(reader.GetOrdinal("columnRef")) == true)
                {
                    ColRef = 0;
                }
                else
                {
                    ColRef = reader.GetInt32(reader.GetOrdinal("columnRef"));
                }
                ElementType = (ImportElementType)reader.GetByte(reader.GetOrdinal("importElementType"));

                Mandatory = reader.GetBoolean(reader.GetOrdinal("mandatory"));
                dataType = (DataType)reader.GetByte(reader.GetOrdinal("dataType"));

                if (reader.IsDBNull(reader.GetOrdinal("referenceTable")) == true)
                {
                    ReferenceTable = "";
                }
                else
                {
                    ReferenceTable = reader.GetString(reader.GetOrdinal("referenceTable"));
                }

                if (reader.IsDBNull(reader.GetOrdinal("referenceField")) == true)
                {
                    ReferenceField = "";
                }
                else
                {
                    ReferenceField = reader.GetString(reader.GetOrdinal("referenceField"));
                }

                lstMappings.Add(new cImportTemplateMapping(TemplateMappingID, TemplateID, BaseTableID, FieldID, DestinationField, ColRef, ElementType, Mandatory, dataType, ReferenceTable, ReferenceField));
            }

            reader.Close();

            return lstMappings;
        }

        /// <summary>
        /// Save the import template to the database
        /// </summary>
        /// <param name="template">Template object</param>
        /// <returns>The ID of the saved template</returns>
        public int saveImportTemplate(cImportTemplate template)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(AccountID));
            int TemplateID;

            expdata.sqlexecute.Parameters.AddWithValue("@templateID", template.TemplateID);
            expdata.sqlexecute.Parameters.AddWithValue("@templateName", template.TemplateName);
            expdata.sqlexecute.Parameters.AddWithValue("@applicationType", template.appType);
            expdata.sqlexecute.Parameters.AddWithValue("@isAutomated", template.IsAutomated);
            expdata.sqlexecute.Parameters.AddWithValue("@NHSTrustID", template.NHSTrustID);

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

            //Save the mappings for the template
            saveImportTemplateMappings(TemplateID, template.Mappings);

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
                expdata.sqlexecute.Parameters.AddWithValue("@templateID", TemplateID);
                expdata.sqlexecute.Parameters.AddWithValue("@templateMappingID", mapping.TemplateMappingID);
                expdata.sqlexecute.Parameters.AddWithValue("@baseTableID", mapping.BaseTableID);
                expdata.sqlexecute.Parameters.AddWithValue("@fieldID", mapping.FieldID);
                expdata.sqlexecute.Parameters.AddWithValue("@destinationField", mapping.DestinationField);
                expdata.sqlexecute.Parameters.AddWithValue("@columnRef", mapping.ColRef);
                expdata.sqlexecute.Parameters.AddWithValue("@importElementType", mapping.ElementType);
                expdata.sqlexecute.Parameters.AddWithValue("@mandatory", mapping.Mandatory);
                expdata.sqlexecute.Parameters.AddWithValue("@dataType", mapping.dataType);
                expdata.sqlexecute.Parameters.AddWithValue("@referenceTable", mapping.ReferenceTable);
                expdata.sqlexecute.Parameters.AddWithValue("@referenceField", mapping.ReferenceField);

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

            expdata.ExecuteProc("dbo.deleteImportTemplate");
            expdata.sqlexecute.Parameters.Clear();
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
        /// <returns>A list of all mappings for creating the import template</returns>
        public Dictionary<string, List<XMLMapFields>> GetApplicationXMLMappings(ApplicationType AppType)
        {
            XName name;
            Dictionary<string, List<XMLMapFields>> lstMappings = new Dictionary<string, List<XMLMapFields>>();

            switch (AppType)
            {
                case ApplicationType.ESROutboundImport:
                    name = XName.Get("Person");

                    List<XMLMapFields> lstPersonXMLMaps = ReadESROutboundXMLTemplateElement(XDocument.Load(HttpContext.Current.Server.MapPath(@"\XMLImportMappings\ESROutboundMappings.xml")).Root.Element(name), 0);
                    lstMappings.Add("Employee", lstPersonXMLMaps);

                    name = XName.Get("Assignment");

                    List<XMLMapFields> lstAssignmentXMLMaps = ReadESROutboundXMLTemplateElement(XDocument.Load(HttpContext.Current.Server.MapPath(@"\XMLImportMappings\ESROutboundMappings.xml")).Root.Element(name), 0);
                    lstMappings.Add("Assignment", lstAssignmentXMLMaps);

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
                    }

                    //Reference Table
                    attribute = child.Attribute(XName.Get("ReferenceTable"));
                    ESRMap.referenceTable = attribute.Value;

                    //Reference Field
                    attribute = child.Attribute(XName.Get("ReferenceField"));
                    ESRMap.referenceField = attribute.Value;

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
                if (temp.appType == ApplicationType.ESROutboundImport)
                {
                    if (temp.IsAutomated)
                    {
                        return temp.TemplateID;
                    }
                }
            }

            return 0;
        }

        public void processESROutboundImport(int TemplateID, byte[] data)
        {
            
            cESRImport import = new cESRImport(AccountID, TemplateID, data);

            bool valid = import.validateImport();

            if (valid)
            {
                import.importOutboundData();
            }
        }
 
    }
}
