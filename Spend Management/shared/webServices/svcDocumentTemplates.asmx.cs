using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Spend_Management;
using SpendManagementLibrary;
using System.Web.Script.Services;

namespace Spend_Management
{
    using Spend_Management.shared.code;

    /// <summary>
    /// Summary description for svcDocumentTemplates
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcDocumentTemplates : System.Web.Services.WebService
    {
        /// <summary>
        /// Saves a copy of an existing document template
        /// </summary>
        /// <param name="templateID">ID of template document to create a copy of</param>
        /// <param name="docTitle">Title of new template document</param>
        /// <param name="docDescription">Description to save against new template</param>
        /// <returns>Two elements { new document ID, return message }</returns>
        [WebMethod(EnableSession = true)]
        public string[] CopyTemplate(int templateID, string docTitle, string docDescription)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            List<string> retVals = new List<string>();

            DocumentTemplate docs = new DocumentTemplate(currentUser.AccountID, currentUser.CurrentSubAccountId, currentUser.EmployeeID);
            cDocumentTemplate doc = docs.getTemplateById(templateID);

            cDocumentTemplate newTemplate = new cDocumentTemplate(0, docTitle, doc.DocumentPath, doc.DocumentFilename, docDescription, doc.DocumentContentType, DateTime.Now, currentUser.EmployeeID, null, null, doc.MergeProjectId);
            byte[] docFile = docs.getDocument(doc.DocumentId);
            int newId = docs.StoreDocument(newTemplate, docFile);

            retVals.Add(newId.ToString());
            switch (newId)
            {
                case -1:
                    retVals.Add("A template with that name already exists");
                    break;
                case 0:
                    retVals.Add("The template failed to save");
                    break;
                default:
                    retVals.Add("Template copy successful");
                    break;
            }

            return retVals.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int deleteTemplate(int documentId)
        {
            HttpApplication appinfo = (HttpApplication)HttpContext.Current.ApplicationInstance;
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (!currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.DocumentConfigurations, true))
            {
                return 0;
            }
            DBConnection db = new DBConnection(cAccounts.getConnectionString(currentUser.AccountID));
            DocumentTemplate templates = new DocumentTemplate(currentUser.AccountID, currentUser.CurrentSubAccountId, currentUser.EmployeeID);

            db.sqlexecute.Parameters.AddWithValue("@documentid", documentId);
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", currentUser.EmployeeID);
            if (currentUser.isDelegate == true)
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }
            db.ExecuteProc("deleteDocumentTemplate");

            return 0;
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int deleteDocMergeAssociation(int docmergeassociationid)
        {
            try
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();
                DocumentTemplate clsTemplates = new DocumentTemplate(currentUser.AccountID, currentUser.CurrentSubAccountId, currentUser.EmployeeID);                        
                clsTemplates.DeleteDocumentTemplateAssociation(docmergeassociationid);
                return 0;
            }
            catch (Exception ex)
            {
                //nice if we add some logging
                return -1;
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] getMergeAssociationGrid(int docid)
        {
            try
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();
                bool bAllowDelete = currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.DocumentConfigurations, true);

                DocumentTemplate clsTemplates = new DocumentTemplate(currentUser.AccountID, currentUser.CurrentSubAccountId, currentUser.EmployeeID);

                return clsTemplates.GetMergeAssociationGrid(currentUser, docid, bAllowDelete);
            }
            catch (Exception ex)
            {
                return new string[] { };
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] getAvailableMergeAssociationsGrid(int docid)
        {
            try
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();
                bool bAllowDelete = currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.DocumentConfigurations, true);

                DocumentTemplate clsTemplates = new DocumentTemplate(currentUser.AccountID, currentUser.CurrentSubAccountId, currentUser.EmployeeID);

                return clsTemplates.GetAvailableAssociationsGrid(docid, currentUser);
            }
            catch (Exception ex)
            {
                return new string[] {};
            }
        }

        public string[] GetDocumentTemplateGrid()
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DocumentTemplates, false, true);

            var grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gDocumentTemplates",
                "select document_templates.documentid, doc_name, doc_description, document_mergeprojects.project_name, document_templates.modifieddate from document_templates ")
            {
                deletelink = "javascript:DeleteTemplate({documentid});",
                editlink = "javascript:window.location.href='aedocumenttemplate.aspx?action=edit&docid={documentid}';",
                enabledeleting = currentUser.CheckAccessRole(AccessRoleType.Delete,
                    SpendManagementElement.DocumentTemplates, true),
                enableupdating = currentUser.CheckAccessRole(AccessRoleType.Edit,
                    SpendManagementElement.DocumentTemplates, true)
            };

            grid.addEventColumn("link", cMisc.Path + "/shared/images/icons/16/plain/link.png",
                "javascript:window.location.href='aedoctemplatesassociation.aspx?docid={documentid}';", "",
                "Associate document template for document configuration.");
            grid.addEventColumn("link", cMisc.Path + "/shared/images/icons/view.png",
                "javascript:window.open('../getDocument.axd?id={documentid}');", "",
                "Open or Save document template.");
            grid.addEventColumn("link", cMisc.Path + "/shared/images/icons/16/plain/document_out.png",
                "javascript:CopyTemplate({documentid});", "", "Save New copy of the document template.");

            grid.getColumnByName("documentid").hidden = true;
            grid.KeyField = "documentid";

            return grid.generateGrid();
        }
    }
}
