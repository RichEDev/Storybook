using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cSupplierContact : ICloneable
    {
        protected int nContactId;
        protected int nSupplierId;
        protected string sName;
        protected string sPosition;
        protected string sMobile;
        protected string sEmail;
        protected cAddress addrBusiness;
        protected cAddress addrHome;
        protected string sComments;
        protected bool bMainContact;
        protected DateTime? dtCreatedOn;
        protected int? nCreatedBy;
        protected DateTime? dtModifiedOn;
        protected int? nModifiedBy;
        [NonSerialized]
        protected SortedList<int, object> slUserDefined;

        public cSupplierContact(int contactid, int supplierid, string name, string position, string mobile, string email, cAddress business_address, cAddress home_address, string comments, bool maincontact, DateTime? createdon, int? createdby, DateTime? modifiedon, int? modifiedby, SortedList<int, object> userdefined)
        {
            nContactId = contactid;
            nSupplierId = supplierid;
            sName = name;
            sPosition = position;
            sMobile = mobile;
            sEmail = email;
            addrBusiness = business_address;
            addrHome = home_address;
            sComments = comments;
            bMainContact = maincontact;
            dtCreatedOn = createdon;
            nCreatedBy = createdby;
            dtModifiedOn = modifiedon;
            nModifiedBy = modifiedby;
            slUserDefined = userdefined;
        }

        public cSupplierContact()
        {
            // This is required for serialization in AJAX update
        }

        public object Clone()
        {
            return new cSupplierContact(this.ContactId, this.SupplierId, this.Name, this.Position, this.Mobile, this.Email, this.BusinessAddress, this.HomeAddress, this.Comments, this.MainContact, this.CreatedDate, this.CreatedById, this.ModifiedDate, this.ModifiedById, this.UserDefined);
        }

        #region properties
        public int ContactId
        {
            get { return nContactId; }
            set { nContactId = value; }
        }
        public int SupplierId
        {
            get { return nSupplierId; }
            set { nSupplierId = value; }
        }
        public string Name
        {
            get { return sName; }
            set { sName = value; }
        }
        public string Position
        {
            get { return sPosition; }
            set { sPosition = value; }
        }
        public string Mobile
        {
            get { return sMobile; }
            set { sMobile = value; }
        }
        public string Email
        {
            get { return sEmail; }
            set { sEmail = value; }
        }
        public string Comments
        {
            get { return sComments; }
            set { sComments = value; }
        }
        public cAddress BusinessAddress
        {
            get { return addrBusiness; }
            set { addrBusiness = value; }
        }
        public cAddress HomeAddress
        {
            get { return addrHome; }
            set { addrHome = value; }
        }
        public bool MainContact
        {
            get { return bMainContact; }
            set { bMainContact = value; }
        }
        public DateTime? CreatedDate
        {
            get { return dtCreatedOn; }
            set { dtCreatedOn = value; }
        }
        public int? CreatedById
        {
            get { return nCreatedBy; }
            set { nCreatedBy = value; }
        }
        public DateTime? ModifiedDate
        {
            get { return dtModifiedOn; }
            set { dtModifiedOn = value; }
        }
        public int? ModifiedById
        {
            get { return nModifiedBy; }
            set { nModifiedBy = value; }
        }
        public SortedList<int, object> UserDefined
        {
            get { return slUserDefined; }
            set { slUserDefined = value; }
        }
        #endregion
    }
}
