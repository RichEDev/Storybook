using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for ESR
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class ESR : System.Web.Services.WebService
    {
        /// <summary>
        /// Deletes an NHS Trust from the database
        /// </summary>
        /// <param name="trustID">The trustId you wish to delete from the database</param>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public ESRTrustReturnVal DeleteNHSTrust(int trustID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cESRTrusts clsTrusts = new cESRTrusts(currentUser.AccountID);
            ESRTrustReturnVal retVal = clsTrusts.DeleteTrust(trustID);
            return retVal;
        }

        /// <summary>
        /// Saves a trust to the database or updates it  if you pass in a positive trustID
        /// </summary>
        /// <param name="trust"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int[] SaveNHSTrust(cESRTrust trust)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cESRTrusts clsTrusts = new cESRTrusts(currentUser.AccountID);

            
            List<int> retValues = clsTrusts.SaveTrust(trust);


            return retValues.ToArray();
        }

        /// <summary>
        /// Get the ESR Trust by its unique identifier
        /// </summary>
        /// <param name="trustID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public cESRTrust GetESRTrustByID(int trustID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cESRTrusts clsTrusts = new cESRTrusts(currentUser.AccountID);
            cESRTrust reqTrust = clsTrusts.GetESRTrustByID(trustID);

            return reqTrust;   
        }

        /// <summary>
        /// Establishes whether any import mappings exist for the current trust ID
        /// </summary>
        /// <param name="trustID">The Trust ID to check</param>
        /// <returns>True if any exist, otherwise False</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public bool CheckForImportMappings(int trustID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cImportTemplates templates = new cImportTemplates(currentUser.AccountID);
            return templates.checkExistenceOfESRAutomatedTemplate(trustID) > 0;
        }

        /// <summary>
        /// Archive the ESR Trust
        /// </summary>
        /// <param name="trustID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public bool ArchiveTrust(int trustID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cESRTrusts clsTrusts = new cESRTrusts(currentUser.AccountID);
            bool archived = clsTrusts.ArchiveTrust(trustID);
            return archived;
        }

        /// <summary>
        /// Save an element from the ESR mappings screen
        /// </summary>
        /// <param name="trustID">Trust ID</param>
        /// <param name="ESRElementID">Element ID</param>
        /// <param name="globalESRElementID">Global Element ID</param>
        /// <param name="ESRFields">List of fields</param>
        /// <param name="subCats">Subcats matched to the Element</param>
        /// <returns>ID of element added success</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int SaveESRElementMapping(int trustID, int ESRElementID, int globalESRElementID, object[] ESRFields, object[] subCats)
        {
            int savedID = 0;
            CurrentUser currentUser = cMisc.GetCurrentUser();

            List<cESRElementField> lstFields = new List<cESRElementField>();
            for (int i = 0; i < ESRFields.Length; i++)
            {
                int elementFieldID = 0;
                int elementID = 0;
                int globalElementFieldID = Convert.ToInt32(((object[])ESRFields[i])[0]);
                string reportColumnValue = Convert.ToString(((object[])ESRFields[i])[1]);
                Guid reportColumnID;
                Aggregate aggregate;
                if (reportColumnValue.Contains("_"))
                {
                    string[] splits = reportColumnValue.Split(new string[] { "_" }, StringSplitOptions.None);
                    reportColumnID = new Guid(splits[0]);
                    aggregate = (Aggregate)Convert.ToInt32(splits[1]);
                }
                else
                {
                    reportColumnID = new Guid(reportColumnValue);
                    aggregate = Aggregate.None;
                }
                byte order = 0;
                lstFields.Add(new cESRElementField(elementFieldID, elementID, globalElementFieldID, reportColumnID, order, aggregate));
            }

            List<int> lstSubCatIDs = new List<int>();
            for (int i = 0; i < ((object[])subCats).Length; i++)
            {
                lstSubCatIDs.Add(Convert.ToInt32(((object[])subCats)[i]));
            }

            cESRElement tempElement = new cESRElement(ESRElementID, globalESRElementID, lstFields, lstSubCatIDs, trustID);
            cESRElementMappings clsElementMaps = new cESRElementMappings(currentUser.AccountID, trustID);
            savedID = clsElementMaps.saveESRElement(tempElement);

            return savedID;
        }

        /// <summary>
        ///  Deletes an element mapping
        /// </summary>
        /// <param name="elementID">ESR Element ID</param>
        /// <param name="trustID">Trust ID</param>
        /// <returns>ESR Element ID</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int DeleteElementMapping(int elementID, int trustID)
        {
            CurrentUser clsCurrentUser = cMisc.GetCurrentUser();
            cESRElementMappings clsESRElementMappings = new cESRElementMappings(clsCurrentUser.AccountID, trustID);
            cESRElement reqESRElement = clsESRElementMappings.getESRElementByID(elementID);

            if (reqESRElement != null)
            {
                clsESRElementMappings.deleteESRElement(reqESRElement.ElementID);
            }

            return elementID;
        }
    }
}
