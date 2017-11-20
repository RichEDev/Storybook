using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    /// <summary>
    /// Methods to extract the data and check against for the Credentials
    /// </summary>
    public class cAPICredentials
    {
        public cAPICredentials()
        {

        }

        public cAPICredential GetCredentialsByUsername(string Username)
        {
            return null;
            //DBConnection db = new DBConnection(
        }
    }

    /// <summary>
    /// Object that contains the details of the API Credentials for a provider
    /// </summary>
    public class cAPICredential
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nCredentialID"></param>
        /// <param name="sUsername"></param>
        /// <param name="sPassword"></param>
        /// <param name="bActive"></param>
        /// <param name="nProviderID"></param>
        /// <param name="dtCreatedOn"></param>
        /// <param name="dtModifiedOn"></param>
        public cAPICredential(int nCredentialID, string sUsername, string sPassword, bool bActive, int nProviderID, DateTime dtCreatedOn, DateTime dtModifiedOn)
        {
            CredentialID = nCredentialID;
            Username = sUsername;
            Password = sPassword;
            Active = bActive;
            ProviderID = nProviderID;
            CreatedOn = dtCreatedOn;
            ModifiedOn = dtModifiedOn;
        }

        #region Properties

        /// <summary>
        /// Unique ID of the API Credentials
        /// </summary>
        public int CredentialID { get; set; }

        /// <summary>
        /// Username for the credentials
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password for the credentials
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Flag to say whether the API Credentials are active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// ID of the provider who wants access to the API
        /// </summary>
        public int ProviderID { get; set; }

        /// <summary>
        /// Created On date
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Modified On date
        /// </summary>
        public DateTime ModifiedOn { get; set; }

        #endregion
    }
}
