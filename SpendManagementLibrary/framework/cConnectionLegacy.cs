using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace SpendManagementLibrary
{
	[Serializable()]
	public class cFWSettings
	{
		public bool Empty
		{
			get
			{
				return _Empty;
			}
			set
			{
				_Empty = value;
			}
		}
		private bool _Empty = true;
		public int XmlFileVersion
		{
			get
			{
				return _xmlFileVersion;
			}
			set
			{
				_xmlFileVersion = value;
			}
		}
		private int _xmlFileVersion; // = DBVERSION;
		public int glDBEngine
		{
			get
			{
				return _glDBEngine;
			}
			set
			{
				_glDBEngine = value;
			}
		}
		private int _glDBEngine = 0;
		public string glLanguage
		{
			get
			{
				return _glLanguage;
			}
			set
			{
				_glLanguage = value;
			}
		}
		private string _glLanguage = "";
		public string glUserID
		{
			get
			{
				return _glUserID;
			}
			set
			{
				_glUserID = value;
			}
		}
		private string _glUserID;
		public string glUserFullName
		{
			get
			{
				return _glUserFullName;
			}
			set
			{
				_glUserFullName = value;
			}
		}
		private string _glUserFullName;
		public string glLogSQL
		{
			get
			{
				return _glLogSQL;
			}
			set
			{
				_glLogSQL = value;
			}
		}
		private string _glLogSQL;
		public string glDatabase
		{
			get
			{
				return _glDatabase;
			}
			set
			{
				_glDatabase = value;
			}
		}
		private string _glDatabase;
		public string glDBUserId
		{
			get
			{
				return _glDBUserId;
			}
			set
			{
				_glDBUserId = value;
			}
		}
		private string _glDBUserId;
		public string glDBPassword
		{
			get
			{
				return _glDBPassword;
			}
			set
			{
				_glDBPassword = value;
			}
		}
		private string _glDBPassword;
		public string glEmailLog
		{
			get
			{
				return _glEmailLog;
			}
			set
			{
				_glEmailLog = value;
			}
		}
		private string _glEmailLog;
		public string glServer
		{
			get
			{
				return _glServer;
			}
			set
			{
				_glServer = value;
			}
		}
		private string _glServer;
		public int glDBTimeout
		{
			get
			{
				return _glDBTimeout;
			}
			set
			{
				_glDBTimeout = value;
			}
		}
		private int _glDBTimeout = 15;
		public string glMailServer
		{
			get
			{
				return _glMailServer;
			}
			set
			{
				_glMailServer = value;
			}
		}
		private string _glMailServer;
		public string glMailFrom
		{
			get
			{
				return _glMailFrom;
			}
			set
			{
				_glMailFrom = value;
			}
		}
		private string _glMailFrom;
		public string glFWLogo
		{
			get
			{
				return _glFWLogo;
			}
			set
			{
				_glFWLogo = value;
			}
		}
		private string _glFWLogo;
		public int glPageSize
		{
			get
			{
				return _glPageSize;
			}
			set
			{
				_glPageSize = value;
			}
		}
		private int _glPageSize;
		public ViewType glInitViewType
		{
			get
			{
				return _glInitViewType;
			}
			set
			{
				_glInitViewType = value;
			}
		}
		private ViewType _glInitViewType = ViewType.Basic;
		public int glKeepForecast
		{
			get
			{
				return _glKeepForecast;
			}
			set
			{
				_glKeepForecast = value;
			}
		}
		private int _glKeepForecast;
		public AttachmentHandling glAttachmentHandling
		{
			get
			{
				return _glAttachmentHandling;
			}
			set
			{
				_glAttachmentHandling = value;
			}
		}
		private AttachmentHandling _glAttachmentHandling = AttachmentHandling.Upload;
		public string KeyPrefix
		{
			get
			{
				return _KeyPrefix;
			}
			set
			{
				_KeyPrefix = value;
			}
		}
		private string _KeyPrefix;
		public string glDocRepository
		{
			get
			{
				return _glDocRepository;
			}
			set
			{
				_glDocRepository = value;
			}
		}
		private string _glDocRepository;
		public string glSecureDocRepository
		{
			get
			{
				return _glSecureDocRepository;
			}
			set
			{
				_glSecureDocRepository = value;
			}
		}
		private string _glSecureDocRepository;
		public string glAuditorList
		{
			get
			{
				return _glAuditorList;
			}
			set
			{
				_glAuditorList = value;
			}
		}
		private string _glAuditorList;
		public bool glPwdExpiry
		{
			get
			{
				return _glPwdExpiry;
			}
			set
			{
				_glPwdExpiry = value;
			}
		}
		private bool _glPwdExpiry;
		public int glPwdExpiryDays
		{
			get
			{
				return _glPwdExpiryDays;
			}
			set
			{
				_glPwdExpiryDays = value;
			}
		}
		private int _glPwdExpiryDays;
		public PasswordLengthSetting glPwdLengthSetting
		{
			get
			{
				return _glPwdLengthSetting;
			}
			set
			{
				_glPwdLengthSetting = value;
			}
		}
		private PasswordLengthSetting _glPwdLengthSetting = PasswordLengthSetting.AnyLength;
		public int glPwdLength1
		{
			get
			{
				return _glPwdLength1;
			}
			set
			{
				_glPwdLength1 = value;
			}
		}
		private int _glPwdLength1;
		public int glPwdLength2
		{
			get
			{
				return _glPwdLength2;
			}
			set
			{
				_glPwdLength2 = value;
			}
		}
		private int _glPwdLength2;
		public bool glPwdUCase
		{
			get
			{
				return _glPwdUCase;
			}
			set
			{
				_glPwdUCase = value;
			}
		}
		private bool _glPwdUCase;
		public bool glPwdNums
		{
			get
			{
				return _glPwdNums;
			}
			set
			{
				_glPwdNums = value;
			}
		}
		private bool _glPwdNums;
		public bool hasDorana
		{
			get
			{
				return _hasDorana;
			}
			set
			{
				_hasDorana = value;
			}
		}
		private bool _hasDorana;
		public string glDoranaCode
		{
			get
			{
				return _glDoranaCode;
			}
			set
			{
				_glDoranaCode = value;
			}
		}
		private string _glDoranaCode;
		public bool glUseDUNS
		{
			get
			{
				return _glUseDUNS;
			}
			set
			{
				_glUseDUNS = value;
			}
		}
		private bool _glUseDUNS;
		public bool glAllowMenuAdd
		{
			get
			{
				return _glAllowMenuAdd;
			}
			set
			{
				_glAllowMenuAdd = value;
			}
		}
		private bool _glAllowMenuAdd;
		public int glMaxRetries
		{
			get
			{
				return _glMaxRetries;
			}
			set
			{
				_glMaxRetries = value;
			}
		}
		private int _glMaxRetries;
		public int glNumPwdHistory
		{
			get
			{
				return _glNumPwdHistory;
			}
			set
			{
				_glNumPwdHistory = value;
			}
		}
		private int _glNumPwdHistory;
		public string glWebEmailerURL
		{
			get
			{
				return _glWebEmailerURL;
			}
			set
			{
				_glWebEmailerURL = value;
			}
		}
		private string _glWebEmailerURL;
		public double glWebEmailerCheckInterval
		{
			get
			{
				return _glWebEmailerCheckInterval;
			}
			set
			{
				_glWebEmailerCheckInterval = value;
			}
		}
		private string _glWebEmailerAdmin;
		public string glWebEmailerAdmin
		{
			get
			{
				return _glWebEmailerAdmin;
			}
			set
			{
				_glWebEmailerAdmin = value;
			}
		}
		private double _glWebEmailerCheckInterval;
		public bool glAllowNotesAdd
		{
			get
			{
				return _glAllowNotesAdd;
			}
			set
			{
				_glAllowNotesAdd = value;
			}
		}
		private bool _glAllowNotesAdd;
		public bool glAutoUpdateCV
		{
			get
			{
				return _glAutoUpdateCV;
			}
			set
			{
				_glAutoUpdateCV = value;
			}
		}
		private bool _glAutoUpdateCV = true;
		public string glApplicationPath
		{
			get
			{
				return _glApplicationPath;
			}
			set
			{
				_glApplicationPath = value;
			}
		}
		private string _glApplicationPath;
		public string glErrorSubmitEmail
		{
			get
			{
				return _glErrorSubmitEmail;
			}
			set
			{
				_glErrorSubmitEmail = value;
			}
		}
		private string _glErrorSubmitEmail;
		public string glErrorSubmitFrom
		{
			get
			{
				return _glErrorSubmitFrom;
			}
			set
			{
				_glErrorSubmitFrom = value;
			}
		}
		private string _glErrorSubmitFrom;
		public bool glUseRechargeFunction
		{
			get
			{
				return _glUseRechargeFunction;
			}
			set
			{
				_glUseRechargeFunction = value;
			}
		}
		private bool _glUseRechargeFunction;
		public bool glUseSavings
		{
			get
			{
				return _glUseSavings;
			}
			set
			{
				_glUseSavings = value;
			}
		}
		private bool _glUseSavings;
		public bool glShowProductonSearch
		{
			get
			{
				return _glShowProductonSearch;
			}
			set
			{
				_glShowProductonSearch = value;
			}
		}
		private bool _glShowProductonSearch;
		public string glCompanyName
		{
			get
			{
				return _glCompanyName;
			}
			set
			{
				_glCompanyName = value;
			}
		}
		private string _glCompanyName;
		public int glCompanyNo
		{
			get
			{
				return _glCompanyNo;
			}
			set
			{
				_glCompanyNo = value;
			}
		}
		private int _glCompanyNo;
		public int glActiveDBVersion
		{
			get
			{
				return _glActiveDBVersion;
			}
			set
			{
				_glActiveDBVersion = value;
			}
		}
		private int _glActiveDBVersion;
		public bool glUseWebService
		{
			get
			{
				return _glUseWebService;
			}
			set
			{
				_glUseWebService = value;
			}
		}
		private bool _glUseWebService;
		private Guid _glMasterDrilldownReport;
		public Guid glMasterDrilldownReport
		{
			get
			{
				return _glMasterDrilldownReport;
			}
			set
			{
				_glMasterDrilldownReport = value;
			}
		}
		private bool _glUseSingleSignOn;
		public bool glUseSSO
		{
			get
			{
				return _glUseSingleSignOn;
			}
			set
			{
				_glUseSingleSignOn = value;
			}
		}
		private int _glLicencedUsers;
		public int glLicencedUsers
		{
			get
			{
				return _glLicencedUsers;
			}
			set
			{
				_glLicencedUsers = value;
			}
		}
		private int _MetabaseConnectionId;
		public int MetabaseConnectionId
		{
			get
			{
				return _MetabaseConnectionId;
			}
			set
			{
				_MetabaseConnectionId = value;
			}
		}
		private int _MetabaseCustomerId;
		public int MetabaseCustomerId
		{
			get
			{
				return _MetabaseCustomerId;
			}
			set
			{
				_MetabaseCustomerId = value;
			}
		}
		private string _MetabaseAccountKey;
		public string MetabaseAccountKey
		{
			get
			{
				return _MetabaseAccountKey;
			}
			set
			{
				_MetabaseAccountKey = value;
			}
		}

		private string _MetabaseConnectionString;
		public string MetabaseConnectionString
		{
			get { return _MetabaseConnectionString; }
			set { _MetabaseConnectionString = value; }
		}

		public string getConnectionString
		{
			get
			{
				SqlConnectionStringBuilder connstr = new SqlConnectionStringBuilder();
				connstr.DataSource = _glServer;
				connstr.InitialCatalog = _glDatabase;
				connstr.MaxPoolSize = 1000;
				connstr.Password = _glDBPassword;
				connstr.UserID = _glDBUserId;
				connstr.PersistSecurityInfo = true;
			    connstr.ApplicationName = GlobalVariables.DefaultApplicationInstanceName;
				return connstr.ToString();
			}
		}

	}

	[Serializable()]
	public class UserInfo
	{
		private bool bError;
		private string sErrorMessage;
		private int nUserId;
		private string sUserName;
		private string sPosition;
		private string sFullName;
		private string sPasswordHint;
		private string sUserEmailAddress;
		private int nActiveLocation;
		private int nRoleId;
		private string sRoleName;
		private string sActiveLocationDesc;
        //private bool bpermAdmin;
        //private bool bpermTabMaint;
        //private bool bpermCodeMaint;
        //private bool bpermInsert;
        //private bool bpermAmend;
        //private bool bpermDelete;
        //private bool bpermFinancial;
        //private bool bpermAuditor;
        //private bool bpermExport;
        //private bool bpermNotes;
        //private bool bpermPublishGlobal;
        //private bool bpermDorana;
        //private bool bpermReports;
        //private bool bpermReadOnlyReports;
		private ViewType aViewType;
		private int nNumAccessibleLocations;
		private System.Collections.SortedList slLocListId;
		private System.Collections.SortedList slLocListNames;
		private System.Collections.SortedList slTabAccessList;
		private int nxMax;
		private int nyMax;
		private int nglSiteCount;
		private int nglRechargeClientCount;
		private int nglStaffCount;
		private int nglRechargeAccCount;
		private fwIconSize aglIconSize;

		#region properties
		public bool IsError
		{
			get
			{
				return bError;
			}
			set
			{
				bError = value;
			}
		}
		public string ErrorMessage
		{
			get
			{
				return sErrorMessage;
			}
			set
			{
				sErrorMessage = value;
			}
		}
		public int UserId
		{
			get
			{
				return nUserId;
			}
			set
			{
				nUserId = value;
			}
		}

		public string UserName
		{
			get
			{
				return sUserName;
			}
			set
			{
				sUserName = value;
			}
		}

		public string Position
		{
			get
			{
				return sPosition;
			}
			set
			{
				sPosition = value;
			}
		}

		public string FullName
		{
			get
			{
				return sFullName;
			}
			set
			{
				sFullName = value;
			}
		}

		public string UserEmailAddress
		{
			get
			{
				return sUserEmailAddress;
			}
			set
			{
				sUserEmailAddress = value;
			}
		}

		public int ActiveLocation
		{
			get
			{
				return nActiveLocation;
			}
			set
			{
				nActiveLocation = value;
			}
		}

		public string ActiveLocationDesc
		{
			get
			{
				return sActiveLocationDesc;
			}
			set
			{
				sActiveLocationDesc = value;
			}
		}

		public string PasswordHint
		{
			get
			{
				return sPasswordHint;
			}
			set
			{
				sPasswordHint = value;
			}
		}

		public int RoleId
		{
			get
			{
				return nRoleId;
			}
			set
			{
				nRoleId = value;
			}
		}

		public string RoleName
		{
			get
			{
				return sRoleName;
			}
			set
			{
				sRoleName = value;
			}
		}

        //public bool permAdmin
        //{
        //    get
        //    {
        //        return bpermAdmin;
        //    }
        //    set
        //    {
        //        bpermAdmin = value;
        //    }
        //}

        //public bool permTabMaint
        //{
        //    get
        //    {
        //        return bpermTabMaint;
        //    }
        //    set
        //    {
        //        bpermTabMaint = value;
        //    }
        //}

        //public bool permCodeMaint
        //{
        //    get
        //    {
        //        return bpermCodeMaint;
        //    }
        //    set
        //    {
        //        bpermCodeMaint = value;
        //    }
        //}

        //public bool permInsert
        //{
        //    get
        //    {
        //        return bpermInsert;
        //    }
        //    set
        //    {
        //        bpermInsert = value;
        //    }
        //}

        //public bool permAmend
        //{
        //    get
        //    {
        //        return bpermAmend;
        //    }
        //    set
        //    {
        //        bpermAmend = value;
        //    }
        //}

        //public bool permDelete
        //{
        //    get
        //    {
        //        return bpermDelete;
        //    }
        //    set
        //    {
        //        bpermDelete = value;
        //    }
        //}

        //public bool permFinancial
        //{
        //    get
        //    {
        //        return bpermFinancial;
        //    }
        //    set
        //    {
        //        bpermFinancial = value;
        //    }
        //}

        //public bool permAuditor
        //{
        //    get
        //    {
        //        return bpermAuditor;
        //    }
        //    set
        //    {
        //        bpermAuditor = value;
        //    }
        //}

        //public bool permExport
        //{
        //    get
        //    {
        //        return bpermExport;
        //    }
        //    set
        //    {
        //        bpermExport = value;
        //    }
        //}

        //public bool permNotes
        //{
        //    get
        //    {
        //        return bpermNotes;
        //    }
        //    set
        //    {
        //        bpermNotes = value;
        //    }
        //}

        //public bool permPublishGlobal
        //{
        //    get
        //    {
        //        return bpermPublishGlobal;
        //    }
        //    set
        //    {
        //        bpermPublishGlobal = value;
        //    }
        //}

        //public bool permDorana
        //{
        //    get
        //    {
        //        return bpermDorana;
        //    }
        //    set
        //    {
        //        bpermDorana = value;
        //    }
        //}

        //public bool permReports
        //{
        //    get
        //    {
        //        return bpermReports;
        //    }
        //    set
        //    {
        //        bpermReports = value;
        //    }
        //}

        //public bool permReadOnlyReports
        //{
        //    get
        //    {
        //        return bpermReadOnlyReports;
        //    }
        //    set
        //    {
        //        bpermReadOnlyReports = value;
        //    }
        //}

		public ViewType ViewType
		{
			get
			{
				return aViewType;
			}
			set
			{
				aViewType = value;
			}
		}

		public int NumAccessibleLocations
		{
			get
			{
				return nNumAccessibleLocations;
			}
			set
			{
				nNumAccessibleLocations = value;
			}
		}

		public System.Collections.SortedList LocListId
		{
			get
			{
				return slLocListId;
			}
			set
			{
				slLocListId = value;
			}
		}

		public System.Collections.SortedList LocListNames
		{
			get
			{
				return slLocListNames;
			}
			set
			{
				slLocListNames = value;
			}
		}

		public System.Collections.SortedList TabAccessList
		{
			get
			{
				return slTabAccessList;
			}
			set
			{
				slTabAccessList = value;
			}
		}

		public int xMax
		{
			get
			{
				return nxMax;
			}
			set
			{
				nxMax = value;
			}
		}

		public int yMax
		{
			get
			{
				return nyMax;
			}
			set
			{
				nyMax = value;
			}
		}

		public int glSiteCount
		{
			get
			{
				return nglSiteCount;
			}
			set
			{
				nglSiteCount = value;
			}
		}

		public int glRechargeClientCount
		{
			get
			{
				return nglRechargeClientCount;
			}
			set
			{
				nglRechargeClientCount = value;
			}
		}

		public int glStaffCount
		{
			get
			{
				return nglStaffCount;
			}
			set
			{
				nglStaffCount = value;
			}
		}

		public int glRechargeAccCount
		{
			get
			{
				return nglRechargeAccCount;
			}
			set
			{
				nglRechargeAccCount = value;
			}
		}

		public fwIconSize glIconSize
		{
			get
			{
				return aglIconSize;
			}
			set
			{
				aglIconSize = value;
			}
		}
		#endregion

		public UserInfo()
		{

		}
	}

}


