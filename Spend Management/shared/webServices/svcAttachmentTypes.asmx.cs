using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SpendManagementLibrary;
using System.Web.UI.WebControls;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcAttachmentTypes
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcAttachmentTypes : System.Web.Services.WebService
    {
        /// <summary>
        /// Save the mime type into the database
        /// </summary>
        /// <param name="globalMimeID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession=true)]
        public int SaveAttachmentType(Guid globalMimeID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cMimeTypes clsMimeTypes = new cMimeTypes(user.AccountID, user.CurrentSubAccountId);
            int ID = clsMimeTypes.SaveMimeType(globalMimeID);

            return ID;
        }

        /// <summary>
        /// Create the grid for the Attachment types
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string[] CreateAttachmentTypeGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            string gridSQL = "SELECT mimeTypes.mimeID, mimeTypes.archived, globalMimeTypes.fileExtension, globalMimeTypes.description, mimeTypes.subAccountID FROM mimeTypes";
            
            cGridNew newgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridAttachmentTypes", gridSQL);

            newgrid.addFilter(((cFieldColumn)newgrid.getColumnByName("SubAccountID")).field, ConditionType.Equals, new object[] { user.CurrentSubAccountId }, null, ConditionJoiner.None);

            newgrid.enablearchiving = true;
            newgrid.enabledeleting = user.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.AttachmentMimeTypes, true);
            newgrid.enableupdating = false;
            newgrid.SortedColumn = newgrid.getColumnByName("fileExtension");
            newgrid.SortDirection = SpendManagementLibrary.SortDirection.Descending;
            newgrid.EmptyText = "There are currently no Attachment Types set up";
            newgrid.deletelink = "javascript:SEL.AttachmentTypes.DeleteAttachmentType({mimeID});";
            newgrid.archivelink = "javascript:SEL.AttachmentTypes.ChangeArchiveStatus({mimeID});";
            newgrid.ArchiveField = "archived";
            newgrid.getColumnByName("mimeID").hidden = true;
            newgrid.getColumnByName("archived").hidden = true;
            newgrid.getColumnByName("subAccountID").hidden = true;
            newgrid.KeyField = "mimeID";
            return newgrid.generateGrid();
        }

        /// <summary>
        /// Get the data to populate the attachment type modal
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public ListItem[] GetAttachmentData()
        {
            CurrentUser user = cMisc.GetCurrentUser();

            cMimeTypes clsMimeTypes = new cMimeTypes(user.AccountID, user.CurrentSubAccountId);

            ListItem[] lstItems = clsMimeTypes.CreateMimeTypeDropdown();

            return lstItems;
        }

        /// <summary>
        /// Delete the attachment type from the database
        /// </summary>
        /// <param name="mimeID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public MimeTypeReturnValue DeleteAttachmentType(int mimeID)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cAccountSubAccounts subAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties properties = subAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
            cMimeTypes clsMimeTypes = new cMimeTypes(user.AccountID, user.CurrentSubAccountId);

            if (properties.UseMobileDevices && properties.AttachReceipts)
            {
                // don't permit deletion of PNG file type as used by mobile device type

                cGlobalMimeTypes globalMimeTypes = new cGlobalMimeTypes(user.AccountID);
                cMimeType pngMimeType = clsMimeTypes.GetMimeTypeByGlobalID(globalMimeTypes.getMimeTypeByExtension("PNG").GlobalMimeID);

                if(pngMimeType != null && pngMimeType.MimeID == mimeID)
                {
                    return MimeTypeReturnValue.MobileDeviceRequirement;
                }
            }

            MimeTypeReturnValue returnValue = clsMimeTypes.DeleteMimeType(mimeID);

            return returnValue;
        }

        /// <summary>
        /// Archive the attachment type
        /// </summary>
        /// <param name="mimeID"></param>
        [WebMethod(EnableSession = true)]
        public MimeTypeReturnValue ArchiveAttachmentType(int mimeID)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            cAccountSubAccounts subAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties properties = subAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
            cMimeTypes clsMimeTypes = new cMimeTypes(user.AccountID, user.CurrentSubAccountId);

            if (properties.UseMobileDevices && properties.AttachReceipts)
            {
                // don't permit deletion of PNG file type as used by mobile device type

                cGlobalMimeTypes globalMimeTypes = new cGlobalMimeTypes(user.AccountID);
                cMimeType pngMimeType = clsMimeTypes.GetMimeTypeByGlobalID(globalMimeTypes.getMimeTypeByExtension("PNG").GlobalMimeID);

                if (pngMimeType != null && pngMimeType.MimeID == mimeID && !pngMimeType.Archived) 
                {
                    return MimeTypeReturnValue.MobileDeviceRequirement;
                }
            }

            clsMimeTypes.ArchiveMimeType(mimeID);

            return MimeTypeReturnValue.Success;
        }

        #region Custom attachment Types

        /// <summary>
        /// Create the grid for the Custom Attachment types
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string[] CreateCustomAttachmentTypeGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(user.AccountID);

            return clsGlobalMimeTypes.GenerateCustomAttachmentGrid(user);
        }

        /// <summary>
        /// Save the mime type into the database
        /// </summary>
        /// <param name="globalMimeID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public int SaveCustomAttachmentType(cGlobalMimeType gMime, string sMimeID)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            //Search disallowed extensions to ensure they cannot be added
            if (System.Configuration.ConfigurationManager.AppSettings["DisallowedAttachmentTypes"] != null)
            {
                string[] disallowedExtensions = System.Configuration.ConfigurationManager.AppSettings["DisallowedAttachmentTypes"].Split(',');
                foreach (string ext in disallowedExtensions)
                {
                    if (gMime.FileExtension == ext)
                    {
                        return -3;
                    }
                }
            }
            

            Guid GlobalMimeID;
            
            if(sMimeID != "")
            {
                GlobalMimeID = new Guid(sMimeID);
                gMime.GlobalMimeID = GlobalMimeID;
            }

            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(user.AccountID);
            int ID = clsGlobalMimeTypes.SaveCustomMimeHeader(gMime);

            return ID;
        }

        /// <summary>
        /// Get the data to populate the custom attachment type form modal
        /// </summary>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public cGlobalMimeType GetCustomAttachmentData(Guid MimeID)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(user.AccountID);
            cGlobalMimeType gMime = clsGlobalMimeTypes.getMimeTypeById(MimeID);

            return gMime;
        }

        // <summary>
        /// Delete the custom attachment type from the database
        /// </summary>
        /// <param name="mimeID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public int DeleteCustomAttachmentType(Guid mimeID)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(user.AccountID);

            int returnValue = clsGlobalMimeTypes.DeleteCustomMimeHeader(mimeID);

            return returnValue;
        }

        #endregion
    }
}
