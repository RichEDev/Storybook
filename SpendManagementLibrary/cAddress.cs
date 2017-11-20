using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cAddress
    {
        protected int nAddressId;
        protected string sAddressTitle;
        protected string sAddr1;
        protected string sAddr2;
        protected string sTown;
        protected string sCounty;
        protected string sPostCode;
        protected int nCountryId;
        protected string sSwitchboard;
        protected string sFax;
        protected bool bPrivateAddress;
        protected DateTime dCreatedDate;
        protected int nCreatedById;
        protected DateTime? dModifiedDate;
        protected int? nModifiedById;

        public cAddress(int addressid, string address_title, string addr1, string addr2, string town, string county, string postcode, int countryid, string switchboard, string fax, bool private_address,DateTime createddate,int createdbyid,DateTime? modifieddate,int? modifiedbyid)
        {
            nAddressId = addressid;
            sAddressTitle = address_title;
            sAddr1 = addr1;
            sAddr2 = addr2;
            sTown = town;
            sCounty = county;
            sPostCode = postcode;
            nCountryId = countryid;
            sSwitchboard = switchboard;
            sFax = fax;
            bPrivateAddress = private_address;
            dCreatedDate = createddate;
            nCreatedById = createdbyid;
            dModifiedDate = modifieddate;
            nModifiedById = modifiedbyid;
        }

        public cAddress()
        { 
            // This is required for serialization in AJAX update
        }

        #region properties
        public int AddressId
        {
            get { return nAddressId; }
            set { nAddressId = value; }
        }
        public string AddressTitle
        {
            get { return sAddressTitle; }
            set { sAddressTitle = value; }
        }
        public string AddressLine1
        {
            get { return sAddr1; }
            set { sAddr1 = value; }
        }
        public string AddressLine2
        {
            get { return sAddr2; }
            set { sAddr2 = value; }
        }
        public string County
        {
            get { return sCounty; }
            set { sCounty = value; }
        }
        public string Town
        {
            get { return sTown; }
            set { sTown = value; }
        }
        public string PostCode
        {
            get { return sPostCode; }
            set { sPostCode = value; }
        }
        public int CountryId
        {
            get { return nCountryId; }
            set { nCountryId = value; }
        }
        public string Switchboard
        {
            get { return sSwitchboard; }
            set { sSwitchboard = value; }
        }
        public string Fax
        {
            get { return sFax; }
            set { sFax = value; }
        }
        public bool IsPrivateAddress
        {
            get { return bPrivateAddress; }
            set { bPrivateAddress = value; }
        }
        public DateTime CreatedDate
        {
            get { return dCreatedDate; }
            set { dCreatedDate = value; }
        }
        public int CreatedById
        {
            get { return nCreatedById; }
            set { nCreatedById = value; }
        }
        public DateTime? LastModifiedDate
        {
            get { return dModifiedDate; }
            set { dModifiedDate = value; }
        }
        public int? LastModifiedById
        {
            get { return nModifiedById; }
            set { nModifiedById = value; }
        }
        #endregion
    }
}
