using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{

    [Serializable()]
    public class cEmployeeCorporateCard
    {
        private int nCorporateCardid;
        private int nEmployeeid;
        private cCardProvider cCardProvider;
        private string sCardnumber;
        private bool bActive;
        private DateTime? dtCreatedOn;
        private int? nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;

        public cEmployeeCorporateCard(int corporatecardid, int employeeid, cCardProvider cardprovider, string cardnumber, bool active, DateTime? createdon, int? createdby, DateTime? modifiedon, int? modifiedby)
        {
            nCorporateCardid = corporatecardid;
            nEmployeeid = employeeid;
            cCardProvider = cardprovider;
            sCardnumber = cardnumber;
            bActive = active;
            dtCreatedOn = createdon;
            nCreatedBy = createdby;
            dtModifiedOn = modifiedon;
            nModifiedBy = modifiedby;
        }

        public void updateCorporateCardId(int id)
        {
            nCorporateCardid = id;
        }
        #region properties
        public int corporatecardid
        {
            get { return nCorporateCardid; }
            set { nCorporateCardid = value; }
        }
        public int employeeid
        {
            get { return nEmployeeid; }
        }
        public cCardProvider cardprovider
        {
            get { return cCardProvider; }
        }
        public string cardnumber
        {
            get { return sCardnumber; }
        }
        public bool active
        {
            get { return bActive; }
        }
        public DateTime? CreatedOn
        {
            get { return dtCreatedOn; }
        }
        public int? CreatedBy
        {
            get { return nCreatedBy; }
        }
        public DateTime? ModifiedOn
        {
            get { return dtModifiedOn; }
        }
        public int? ModifiedBy
        {
            get { return nModifiedBy; }
        }
        #endregion
    }
}
