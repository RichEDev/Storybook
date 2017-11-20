using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace tempMobileUnitTests
{
    public class GlobalTestVariables
    {
        #region Utility methods

        public static string GetWebConfigStringValue(string propertyName)
        {
            if (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings.AllKeys.Contains(propertyName))
            {
                if (ConfigurationManager.AppSettings[propertyName] != null)
                {
                    return ConfigurationManager.AppSettings[propertyName];
                }
                else
                {
                    throw new ConfigurationErrorsException("Your Web.config has an AppSetting parameter called " + propertyName.ToString() + ", but it has no value");
                }
            }
            else
            {
                throw new ConfigurationErrorsException("Your Web.config does not have an AppSetting called " + propertyName.ToString() + " or your app may be misconfigured.");
            }
        }

        public static int GetWebConfigIntValue(string propertyName)
        {
            if (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings.AllKeys.Contains(propertyName))
            {
                int retVal = 0;
                if (ConfigurationManager.AppSettings[propertyName] != null && int.TryParse(ConfigurationManager.AppSettings[propertyName], out retVal))
                {
                    return retVal;
                }
                else
                {
                    throw new ConfigurationErrorsException("Your Web.config has an AppSetting parameter called " + propertyName.ToString() + ", but it either has no value or an invalid one.");
                }
            }
            else
            {
                throw new ConfigurationErrorsException("Your Web.config does not have an AppSetting called " + propertyName.ToString() + " or your app may be misconfigured.");
            }
        }

        public static bool GetWebConfigBoolValue(string propertyName)
        {
            if (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings.AllKeys.Contains(propertyName))
            {
                bool retVal = false;
                if (ConfigurationManager.AppSettings[propertyName] != null && bool.TryParse(ConfigurationManager.AppSettings[propertyName], out retVal))
                {
                    return retVal;
                }
                else
                {
                    throw new ConfigurationErrorsException("Your Web.config has an AppSetting parameter called " + propertyName.ToString() + ", but it either has no value or an invalid one.");
                }
            }
            else
            {
                throw new ConfigurationErrorsException("Your Web.config does not have an AppSetting called " + propertyName.ToString() + " or your app may be misconfigured.");
            }
        }

        #endregion Utility methods

        #region Properties

        public static int AccountID
        {
            get { return GetWebConfigIntValue("AccountID"); }
        }

        public static int SubAccountID
        {
            get { return GetWebConfigIntValue("SubAccountID"); }
        }

        public static int EmployeeID
        {
            get { return GetWebConfigIntValue("EmployeeID"); }
        }

        public static int AlternativeEmployeeID
        {
            get { return GetWebConfigIntValue("AlternativeEmployeeID"); }
        }

        public static int ActiveModuleID
        {
            get { return GetWebConfigIntValue("active_module"); }
        }

        public static Modules ActiveModule
        {
            get { return (Modules)GetWebConfigIntValue("active_module"); }
        }

        public static int DelegateID
        {
            get { return GetWebConfigIntValue("DelegateID"); }
        }

        public static bool UseDelegate { get; set; }

        #endregion Properties
    }
}
