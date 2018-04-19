using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using FWBase;

namespace SpendManagementLibrary
{
	public class cMigration
	{
		public static cFWSettings ConvertToFWSettings(cAccount account, SortedList<int, cAccountSubAccount> subaccounts, int? activeSubAccountId)
		{
			cAccountProperties properties;

			if (!activeSubAccountId.HasValue)
			{
				properties = subaccounts[subaccounts.Keys[0]].SubAccountProperties;
			}
			else
			{
				properties = subaccounts[activeSubAccountId.Value].SubAccountProperties;
			}

			return getFWSettings(account, properties);
		}

		private static cFWSettings getFWSettings(cAccount account, cAccountProperties subproperties)
		{
			cFWSettings fws = new cFWSettings();

			fws.glCompanyName = account.companyname;
			fws.MetabaseAccountKey = account.companyid;
			fws.MetabaseConnectionId = account.accountid;
			fws.MetabaseCustomerId = account.accountid;
			fws.glDatabase = account.dbname;
			fws.glDBEngine = 1;
            cSecureData crypt = new cSecureData();
            string decrypted_pwd = crypt.Decrypt(account.dbpassword);
			fws.glDBPassword = decrypted_pwd;
            fws.glServer = account.dbserver;
			fws.glDBTimeout = 15;
			fws.glDBUserId = account.dbusername;
			fws.glDocRepository = subproperties.DocumentRepository;
			fws.glErrorSubmitEmail = "framework-internalerrors@selenity.com";
			fws.glErrorSubmitFrom = subproperties.EmailServerFromAddress;
			fws.glKeepForecast = subproperties.KeepInvoiceForecasts ? 1 : 0;
			fws.glMailFrom = subproperties.EmailServerFromAddress;
			fws.glMailServer = subproperties.EmailServerAddress;
			fws.glMaxRetries = subproperties.PwdMaxRetries;
			fws.glNumPwdHistory = subproperties.PwdHistoryNum;
			fws.glPageSize = subproperties.DefaultPageSize;
			fws.glPwdExpiry = subproperties.PwdExpires;
			fws.glPwdExpiryDays = subproperties.PwdExpiryDays;
			fws.glPwdLength1 = subproperties.PwdLength1;
			fws.glPwdLength2 = subproperties.PwdLength2;
			fws.glPwdLengthSetting = (PasswordLengthSetting)subproperties.PwdConstraint;
			fws.glPwdNums = subproperties.PwdMustContainNumbers;
			fws.glPwdUCase = subproperties.PwdMustContainUpperCase;
			fws.glShowProductonSearch = subproperties.ShowProductInSearch;
			fws.glAuditorList = subproperties.AuditorEmailAddress;
			fws.glAllowNotesAdd = subproperties.AllowArchivedNotesAdd;
			fws.glAllowMenuAdd = subproperties.AllowMenuContractAdd;
			fws.glActiveDBVersion = 23;
			fws.KeyPrefix = subproperties.ContractKey;
			fws.glAutoUpdateCV = subproperties.AutoUpdateAnnualContractValue;

			return fws;
		}

	}
}
