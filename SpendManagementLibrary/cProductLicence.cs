using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    /// <summary>
    /// cProductLicence class
    /// </summary>
	public class cProductLicence
	{
		public cProductLicence(int licenceid, int productid, string licencekey, int licencetype, DateTime expiry, cLicenceRenewalType renewaltype, int notifyid, AudienceType notifytype, int notifydays, string licencelocation, bool hardcopy, bool softcopy, bool unlimitedlicence, int numbercopiesheld, string availableversion, string installedversion, string usercode, DateTime? dateinstalled, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby)
		{
			LicenceId = licenceid;
			ProductId = productid;
			LicenceKey = licencekey;
			LicenceType = licencetype;
			LicenceExpiry = expiry;
			LicenceRenewalType = renewaltype;
			LicenceLocation = licencelocation;
			NotifyId = notifyid;
			NotifyType = notifytype;
			NotifyDays = notifydays;
			IsElectronicCopyHeld = softcopy;
			IsHardCopyHeld = hardcopy;
			IsUnlimitedLicence = unlimitedlicence;
			NumberCopiesHeld = numbercopiesheld;
			sInstalledVersionNumber = installedversion;
			sAvailableVersionNumber = availableversion;
            sUserCode = usercode;
            dDateInstalled = dateinstalled;
			CreatedOn = createdon;
			CreatedBy = createdby;
			ModifiedOn = modifiedon;
			ModifiedBy = modifiedby;
		}

		#region properties
		private int nLicenceId;
		public int LicenceId
		{
			get { return nLicenceId; }
			set { nLicenceId = value; }
		}
		private int nProductId;
		public int ProductId
		{
			get { return nProductId; }
			set { nProductId = value; }
		}
		private string sLicenceKey;
		public string LicenceKey
		{
			get { return sLicenceKey; }
			set { sLicenceKey = value; }
		}
		private int sLicenceType;
		public int LicenceType
		{
			get { return sLicenceType; }
			set { sLicenceType = value; }
		}
		private DateTime dExpiry;
		public DateTime LicenceExpiry
		{
			get { return dExpiry; }
			set { dExpiry = value; }
		}
		private cLicenceRenewalType lrRenewalType;
		public cLicenceRenewalType LicenceRenewalType
		{
			get { return lrRenewalType; }
			set { lrRenewalType = value; }
		}
		private int nNotifyId;
		public int NotifyId
		{
			get { return nNotifyId; }
			set { nNotifyId = value; }
		}
		private AudienceType atNotifyType; // team or individual
		public AudienceType NotifyType
		{
			get { return atNotifyType; }
			set { atNotifyType = value; }
		}
		private string sLicenceLocation;
		public string LicenceLocation
		{
			get { return sLicenceLocation; }
			set { sLicenceLocation = value; }
		}
		private bool bElectronicCopyHeld;
		public bool IsElectronicCopyHeld
		{
			get { return bElectronicCopyHeld; }
			set { bElectronicCopyHeld = value; }
		}
		private bool bHardCopyHeld;
		public bool IsHardCopyHeld
		{
			get { return bHardCopyHeld; }
			set { bHardCopyHeld = value; }
		}
		private bool bUnlimitedLicence;
		public bool IsUnlimitedLicence
		{
			get { return bUnlimitedLicence; }
			set { bUnlimitedLicence = value; }
		}
		private int nNotifyDays;
		public int NotifyDays
		{
			get { return nNotifyDays; }
			set { nNotifyDays = value; }
		}
		private int nNumberCopiesHeld;
		public int NumberCopiesHeld
		{
			get { return nNumberCopiesHeld; }
			set { nNumberCopiesHeld = value; }
		}
		/// <summary>
		/// Product version currently installed
		/// </summary>
		private string sInstalledVersionNumber;
		/// <summary>
		/// Product version currently installed
		/// </summary>
		public string InstalledVersion
		{
			get { return sInstalledVersionNumber; }
			set { sInstalledVersionNumber = value; }
		}
		/// <summary>
		/// Date product was installed
		/// </summary>
		private DateTime? dDateInstalled;
		/// <summary>
		/// Date product was installed
		/// </summary>
		public DateTime? DateInstalled
		{
			get { return dDateInstalled; }
			set { dDateInstalled = value; }
		}
		/// <summary>
		/// Available version number
		/// </summary>
		private string sAvailableVersionNumber;
		/// <summary>
		/// Available version number
		/// </summary>
		public string AvailableVersion
		{
			get { return sAvailableVersionNumber; }
			set { sAvailableVersionNumber = value; }
		}
        /// <summary>
        /// User Licence Code
        /// </summary>
        private string sUserCode;
        /// <summary>
        /// User Licence Code
        /// </summary>
        public string UserCode
        {
            get { return sUserCode; }
            set { sUserCode = value; }
        }
        /// <summary>
        /// Date the licence record was created
        /// </summary>
		private DateTime dtCreatedOn;
        /// <summary>
        /// Date the licence record was created
        /// </summary>
		public DateTime CreatedOn
		{
			get { return dtCreatedOn; }
			set { dtCreatedOn = value; }
		}
        /// <summary>
        /// Employee ID of record creator
        /// </summary>
		private int nCreatedBy;
        /// <summary>
        /// Employee ID of record creator
        /// </summary>
		public int CreatedBy
		{
			get { return nCreatedBy; }
			set { nCreatedBy = value; }
		}
		private DateTime? dtModifiedOn;
		public DateTime? ModifiedOn
		{
			get { return dtModifiedOn; }
			set { dtModifiedOn = value; }
		}
		private int? nModifiedBy;
		public int? ModifiedBy
		{
			get { return nModifiedBy; }
			set { nModifiedBy = value; }
		}
		#endregion
	}
}
