using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    /// <summary>
    /// The unique access key used to access expenses via the API with
    /// </summary>
    public class cAccessKey
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nKeyID"></param>
        /// <param name="sKey"></param>
        /// <param name="nEmployeeID"></param>
        /// <param name="sDeviceID"></param>
        /// <param name="bActive"></param>
        /// <param name="dtCreatedOn"></param>
        /// <param name="nCreatedBy"></param>
        /// <param name="dtModifiedOn"></param>
        /// <param name="nModifiedBy"></param>
        public cAccessKey(int nKeyID, string sKey, int nEmployeeID, string sDeviceID, bool bActive, DateTime? dtCreatedOn, int? nCreatedBy, DateTime? dtModifiedOn, int? nModifiedBy)
        {
            KeyID = nKeyID;
            Key = sKey;
            EmployeeID = nEmployeeID;
            DeviceID = sDeviceID;
            Active = bActive;
            CreatedOn = dtCreatedOn;
            CreatedBy = nCreatedBy;
            ModifiedOn = dtModifiedOn;
            ModifiedBy = nModifiedBy;
        }

        #region Properties

        /// <summary>
        /// Unique ID in the database
        /// </summary>
        public int KeyID { get; set; }

        /// <summary>
        /// The Key value
        /// Key format is [xxxx-xxxxxx-xxxxxx] = [AccountID]-[EmployeeID]-[EpochDigits]
        /// Key is always 18 characters
        /// 4 maximum characters for accountID
        /// 6 maximum characters for employeeID
        /// 6 minimum or more if accountID or employeeId is less than their maximum for EpochDigits
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// ID of the associated employee
        /// </summary>
        public int EmployeeID { get; set; }

        /// <summary>
        /// The uniqueID of the associated device
        /// </summary>
        public string DeviceID { get; set; }
        /// <summary>
        /// Key is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Created On date
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Created By
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Modified On Date
        /// </summary>
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Modified By
        /// </summary>
        public int? ModifiedBy { get; set; }

        #endregion
    }
}
