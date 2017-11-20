using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcAttachments
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [ScriptService]
    public class svcAttachments : WebService
    {
		/// <summary>
		/// deleteAttachment: remove an attachment and its stored binary file
		/// </summary>
		/// <param name="tableid">Table GUID of attachment table</param>
		/// <param name="attachmentid">ID of the attachment to be deleted</param>
		/// <returns>ID of the attachment deleted</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int deleteAttachment(string tableid, int attachmentid, int ID, AttachDocumentType docType)
        {
            if (User.Identity.IsAuthenticated)
            {
				Guid tableguid = new Guid(tableid);
                CurrentUser user = cMisc.GetCurrentUser();
                int? delegateId = null;
                if (user.isDelegate)
                {
                    delegateId = user.Delegate.EmployeeID;
                }
				cTables tables = new cTables(user.AccountID);
                cTable attachmentTable = tables.GetTableByID(tableguid);
                cAttachments clsAttachments = new cAttachments(user.AccountID, user.EmployeeID, user.CurrentSubAccountId, delegateId);
                clsAttachments.deleteAttachment(attachmentTable.TableName, attachmentid, ID, docType);
            }

			return attachmentid;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] getAttachmentData (AttachDocumentType docType, int ID)
        {
            object[] attachData = new object[4];
            CurrentUser user = cMisc.GetCurrentUser();
            int? delegateId = null;
            if (user.isDelegate)
            {
                delegateId = user.Delegate.EmployeeID;
            }
            cTables clsTables = new cTables(user.AccountID);
            cAttachments clsAttachments = new cAttachments(user.AccountID, user.EmployeeID, user.CurrentSubAccountId, delegateId);
            int attachmentID = clsAttachments.getAttachmentIDFromElement(docType, ID);
            cTable table;

            attachData[0] = (byte)docType;
            attachData[1] = attachmentID;
            attachData[2] = ID;

            switch (docType)
            {
                case AttachDocumentType.Licence:
                    table = clsTables.GetTableByName("employee_attachments");
                    attachData[3] = table.TableID;
                    break;
                case AttachDocumentType.Tax:
                case AttachDocumentType.MOT:
                case AttachDocumentType.Insurance:
                case AttachDocumentType.Service:
                    table = clsTables.GetTableByName("cars_attachments");
                    attachData[3] = table.TableID;
                    break;
                case AttachDocumentType.ExpenseReceipt:
                    table = clsTables.GetTableByName("savedexpenses_attachments");
                    attachData[3] = table.TableID;
                    break;
                default:
                    break;
            }

            return attachData;
        }

        /// <summary>
        /// Toggles whether or not a custom entity (torch generated) attachment is published
        /// </summary>
        /// <param name="tableId">The custom entity attachment table identifier</param>
        /// <param name="attachmentId">The custom entity record attachment identifier</param>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public void toggleTorchAttachmentPublished(string tableId, int attachmentId)
        {
            var user = cMisc.GetCurrentUser();
            
            int? delegateId = null;
            if (user.isDelegate)
            {
                delegateId = user.Delegate.EmployeeID;
            }

            cTable attachmentTable = new cTables(user.AccountID).GetTableByID(new Guid(tableId));
            var clsAttachments = new cAttachments(user.AccountID, user.EmployeeID, user.CurrentSubAccountId, delegateId);
            clsAttachments.ToggleAttachmentPublished(attachmentTable, attachmentId);
        }
    }
}
