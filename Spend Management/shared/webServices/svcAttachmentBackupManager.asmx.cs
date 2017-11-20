using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using SpendManagementLibrary;
using System.Web.Script.Services;
using Spend_Management.AttachmentBackups;

namespace Spend_Management
{
    /// <summary>
    /// Web service calls to the Attachment Backup Classes
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)] 
    [System.Web.Script.Services.ScriptService]
    public class svcAttachmentBackupManager : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        public string[] GetProcessedBackupsGrid(bool PackagesReadyForDownload)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            return AttachmentBackupManager.GetPackagesGrid(user.AccountID, user.EmployeeID, PackagesReadyForDownload);
        }

        [WebMethod(EnableSession = true)]
        public string[] GetVolumesForPackageGrid(Guid PackageID)
        {
            CurrentUser user = cMisc.GetCurrentUser();

            return AttachmentBackupManager.GetVolumesForPackageGrid(PackageID, user.AccountID, user.EmployeeID);
        }
    }
}
