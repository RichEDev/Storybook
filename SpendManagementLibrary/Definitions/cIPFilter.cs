using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cIPFilter
    {
        int nIPilterID;        
        string sIPAddress;        
        string sDescription;
        bool bActive;
        private DateTime dtCreatedon;
        private int nCreatedby;
        private DateTime? dtModifiedon;
        private int? nModifiedby;
        
        public cIPFilter(int ipfilterid, string ipaddress, string description, bool active, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby)
        {            
            nIPilterID = ipfilterid;
            sIPAddress = ipaddress;            
            sDescription = description;
            bActive = active;
            dtCreatedon = createdon;
            nCreatedby = createdby;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;
        }

        #region properties

        /// <summary>
        /// Returns the ID of the IP Filter
        /// </summary>
        public int IPFilterID
        {
            get { return nIPilterID; }
        }
        
        /// <summary>
        /// Returns the IP Address of the IP Filter
        /// </summary>
        public string IPAddress
        {
            get { return sIPAddress; }
        }

        /// <summary>
        /// Returns the description of the IP Filter
        /// </summary>
        public string Description
        {
            get { return sDescription; }
        }
        
        /// <summary>
        /// Gets and sets the active flag of the IP Filter. 
        /// If the IP Filter is active, it will be used for the current account
        /// </summary>
        public bool Active
        {
            get { return bActive; }
            set { bActive = value; }
        }
        
        /// <summary>
        /// Gets the creation date of the IP Filter
        /// </summary>
        public DateTime CreatedOn
        {
            get { return dtCreatedon; }
        }
        
        /// <summary>
        /// Gets the employee ID of the employee that created the IP Filter
        /// </summary>
        public int CreatedBy
        {
            get { return nCreatedby; }
        }
        
        /// <summary>
        /// Gets the last modified date of the IP Filter
        /// </summary>
        public DateTime? ModifiedOn
        {
            get { return dtModifiedon; }
        }
        
        /// <summary>
        /// Gets the employee ID of the employee that last modified the IP Filter
        /// </summary>
        public int? ModifiedBy
        {
            get { return nModifiedby; }
        }
        
        #endregion

    }
}
