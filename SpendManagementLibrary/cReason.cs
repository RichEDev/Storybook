using System;
using System.Collections.Generic;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cReason
    {
        private int nAccountid;
        private int nReasonid;
        private string sReason;
        private string sDescription;
        private string sAccountcodeVat;
        private string sAccountcodeNoVat;
        private DateTime dtCreatedon;
        private int nCreatedby;
        private DateTime? dtModifiedon;
        private int? nModifiedby;
        private bool isArchived;

        public cReason(int accountid, int reasonid, string reason, string description, string accountcodevat, string accountcodenovat, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, bool archive)
        {
            nAccountid = accountid;
            nReasonid = reasonid;
            sReason = reason;
            sDescription = description;
            sAccountcodeVat = accountcodevat;
            sAccountcodeNoVat = accountcodenovat;
            
            dtCreatedon = createdon;
            nCreatedby = createdby;
            dtModifiedon = modifiedon;
            nModifiedby = modifiedby;
            this.isArchived = archive;
        }

        #region properties

        public bool Archive
        {
            get
            {
                return this.isArchived;
            }
        }
        public string reason
        {
            get
            {
                return sReason;
            }
        }

        public int accountid
        {
            get { return nAccountid; }
        }

        public int reasonid
        {
            get { return nReasonid; }
        }

        public string description
        {
            get { return sDescription; }
        }

        public string accountcodevat
        {
            get { return sAccountcodeVat; }
        }
        public string accountcodenovat
        {
            get { return sAccountcodeNoVat; }
        }
        public DateTime createdon
        {
            get { return dtCreatedon; }
        }
        public int createdby
        {
            get { return nCreatedby; }
        }
        public DateTime? modifiedon
        {
            get { return dtModifiedon; }
        }
        public int? modifiedby
        {
            get { return nModifiedby; }
        }
        #endregion
    }
}
