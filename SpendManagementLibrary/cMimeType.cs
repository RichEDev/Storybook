using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace SpendManagementLibrary
{
    /// <summary>
    ///  Object for the local mime types
    /// </summary>
    public class cMimeType
    {
        private int nMimeID;
        private Guid uGlobalMimeID;
        private bool bArchived;
        private DateTime? dtCreatedOn;
        private int? nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="MimeID"></param>
        /// <param name="GlobalMimeID"></param>
        /// <param name="Archived"></param>
        /// <param name="CreatedOn"></param>
        /// <param name="CreatedBy"></param>
        /// <param name="ModifiedOn"></param>
        /// <param name="ModifiedBy"></param>
        public cMimeType(int MimeID, Guid GlobalMimeID, bool Archived, DateTime? CreatedOn, int? CreatedBy, DateTime? ModifiedOn, int? ModifiedBy)
        {
            nMimeID = MimeID;
            uGlobalMimeID = GlobalMimeID;
            bArchived = Archived;
            dtCreatedOn = CreatedOn;
            nCreatedBy = CreatedBy;
            dtModifiedOn = ModifiedOn;
            nModifiedBy = ModifiedBy;
        }

        #region Properties

        /// <summary>
        /// Unique ID in the database
        /// </summary>
        public int MimeID
        {
            get { return nMimeID; }
        }

        /// <summary>
        /// Global mime ID
        /// </summary>
        public Guid GlobalMimeID
        {
            get { return uGlobalMimeID; }
        }

        /// <summary>
        /// Archived
        /// </summary>
        public bool Archived 
        { 
            get { return bArchived; } 
        }

        /// <summary>
        /// Created On date
        /// </summary>
        public DateTime? CreatedOn 
        { 
            get { return dtCreatedOn;} 
        }

        /// <summary>
        /// Created By
        /// </summary>
        public int? CreatedBy 
        { 
            get { return nCreatedBy ;} 
        }

        /// <summary>
        /// Modified On Date
        /// </summary>
        public DateTime? ModifiedOn 
        { 
            get { return dtModifiedOn ;} 
        }

        /// <summary>
        /// Modified By
        /// </summary>
        public int? ModifiedBy 
        { 
            get { return nModifiedBy ; }
        }

        #endregion

    }

    /// <summary>
    /// Enumerable type that describes the return value type when deleting a mime type from the database
    /// </summary>
    public enum MimeTypeReturnValue
    {
        Success = 0,
        CarAttachmentExists = -1,
        EmployeeAttachmentExists = -2,
        EmailTemplateAttachmentExists = -3,
        CustomEntityAttachmentExists = -4,
        MobileDeviceRequirement = -5
    }
}
