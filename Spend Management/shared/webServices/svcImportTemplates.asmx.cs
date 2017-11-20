using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using SpendManagementLibrary;

namespace Spend_Management
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Summary description for svcImportTemplates
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcImportTemplates : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int SaveImportTemplateMappings(int TemplateID, string TemplateName, int AppType, int TrustID, int IsAutomated, string signOffOwnerFieldId, string lineManagerFieldId, params object[] TempMaps)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cImportTemplates clsImportTemplates = new cImportTemplates(currentUser.AccountID);

            bool Auto = Convert.ToBoolean(IsAutomated);
            int colRef = 0;
            string destinationField = "";
            Guid fieldID = Guid.Empty;
            ImportElementType impType = new ImportElementType();
            bool mandatory = false;
            DataType dataType;
            Guid lookupTableId = Guid.Empty;
            Guid matchFieldId = Guid.Empty;
            bool overridePK = false;
            bool populated = true;
            bool allowdynamicmapping = false;
            bool importField = false;

            object[] tmpArr;
            cImportTemplateMapping tmpImportTemplateMapping;
            List<cImportTemplateMapping> lstMappings = new List<cImportTemplateMapping>();
            cTables clstables = new cTables(currentUser.AccountID);
            cFields clsfields = new cFields(currentUser.AccountID);
            var esrTrusts = new cESRTrusts(currentUser.AccountID);
            if (esrTrusts.GetESRTrustByID(TrustID).EsrInterfaceVersionNumber == 2)
            {
                AppType = 2;
            }

            cField field;
            for (int i = 0; i < TempMaps.Length; i++)
            {
                for (int j = 0; j < ((object[])TempMaps[i]).Length; j++)
                {
                    tmpArr = ((object[])((object[])TempMaps[i])[j]);

                    impType = cImportTemplates.GetElementType(tmpArr[0].ToString());

                    colRef = (int)tmpArr[1];
                    destinationField = tmpArr[2].ToString();
                    fieldID = new Guid(tmpArr[3].ToString());
                    field = clsfields.GetFieldByID(fieldID);
                    mandatory = Convert.ToBoolean(tmpArr[4]);
                    dataType = (DataType)((int)tmpArr[5]);
                    Guid.TryParseExact(tmpArr[6].ToString(), "D", out lookupTableId);
                    cTable lktable = null;
                    if (lookupTableId != Guid.Empty)
                    {
                        lktable = clstables.GetTableByID(lookupTableId);
                    }
                    Guid.TryParseExact(tmpArr[7].ToString(), "D", out matchFieldId);
                    cField matchField = null;
                    if (matchFieldId != Guid.Empty)
                    {
                        matchField = clsfields.GetFieldByID(matchFieldId);
                    }
                    overridePK = Convert.ToBoolean(tmpArr[8]);
                    populated = Convert.ToBoolean(tmpArr[9]);
                    allowdynamicmapping = Convert.ToBoolean(tmpArr[10]);
                    importField = Convert.ToBoolean(tmpArr[11]);

                    tmpImportTemplateMapping = new cImportTemplateMapping(0, TemplateID, fieldID, field, destinationField, colRef, impType, mandatory, dataType, lktable, matchField, overridePK, populated, allowdynamicmapping, importField);
                    lstMappings.Add(tmpImportTemplateMapping);
                }
            }


            cImportTemplate tempImpTemp = new cImportTemplate(TemplateID, TemplateName, (ApplicationType)AppType, Auto, TrustID, (String.IsNullOrWhiteSpace(signOffOwnerFieldId) ? Guid.Empty : new Guid(signOffOwnerFieldId)), (String.IsNullOrWhiteSpace(signOffOwnerFieldId) ? Guid.Empty : new Guid(lineManagerFieldId)), lstMappings, DateTime.Now, currentUser.EmployeeID, DateTime.Now, currentUser.EmployeeID);
            int retVal = clsImportTemplates.saveImportTemplate(tempImpTemp);
            return retVal;
        }

        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public int DeleteImportTemplate(int templateID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cImportTemplates clsImportTemplates = new cImportTemplates(currentUser.AccountID);
            cImportTemplate reqImportTemplate = clsImportTemplates.getImportTemplateByID(templateID);

            if (reqImportTemplate != null)
            {
                clsImportTemplates.deleteImportTemplate(reqImportTemplate.TemplateID);
            }
            else
            {
                templateID = 0;
            }

            return templateID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableID"></param>
        /// <param name="dataType"></param>
        /// <param name="includeUserdefinedFields"></param>
        /// <param name="includeIdField"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<sFieldBasics> GetTableFields(string tableID, DataType dataType, bool includeUserdefinedFields = false, bool includeIdField = false)
        {
            Guid gTableID = Guid.Empty;
            if (!Guid.TryParseExact(tableID, "D", out gTableID))
            {
                return new List<sFieldBasics>();
            }
            
            CurrentUser currentUser = cMisc.GetCurrentUser();
            List<sFieldBasics> lstFields = new List<sFieldBasics>();
            cTables clsTables = new cTables(currentUser.AccountID);
            cFields clsFields = new cFields(currentUser.AccountID);
            cUserdefinedFields clsUserfields = new cUserdefinedFields(currentUser.AccountID);

            var fieldDataType = dataType;
            switch (dataType)
            {
                case DataType.referenceLookup:
                    fieldDataType = DataType.intVal;
                    break;
                case DataType.longLookup:
                    fieldDataType = DataType.longVal;
                    break;
            }

            lstFields = clsFields.GetFieldBasicsByTableIDWithUserdefined(gTableID, fieldDataType, includeIdField);

            cTable curTable = clsTables.GetTableByID(gTableID);
            if (curTable != null && curTable.GetUserdefinedTable() != null)
            {
                fieldDataType = dataType;
                switch (dataType)
                {
                    case DataType.referenceLookup:
                        fieldDataType = DataType.stringVal;
                        break;
                    case DataType.longLookup:
                        fieldDataType = DataType.stringVal;
                        break;
                }

                lstFields.AddRange(clsUserfields.GetUserdefinedFieldBasicsByType(curTable.UserDefinedTableID, fieldDataType));
            }

            return lstFields.OrderBy(x => x.Description).ToList();
        }
    }
}
