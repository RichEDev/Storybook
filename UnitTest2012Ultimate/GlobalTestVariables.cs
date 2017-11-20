using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace UnitTest2012Ultimate
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

                throw new ConfigurationErrorsException("Your Web.config has an AppSetting parameter called " + propertyName + ", but it has no value");
            }

            throw new ConfigurationErrorsException("Your Web.config does not have an AppSetting called " + propertyName + " or your app may be misconfigured.");
        }

        /// <summary>
        /// The get web config int value.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="ConfigurationErrorsException">
        /// ConfigurationErrorsException
        /// </exception>
        public static int GetWebConfigIntValue(string propertyName)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(propertyName))
            {
                int retVal = 0;
                if (ConfigurationManager.AppSettings[propertyName] != null && int.TryParse(ConfigurationManager.AppSettings[propertyName], out retVal))
                {
                    return retVal;
                }

                throw new ConfigurationErrorsException("Your Web.config has an AppSetting parameter called " + propertyName + ", but it either has no value or an invalid one.");
            }

            throw new ConfigurationErrorsException("Your Web.config does not have an AppSetting called " + propertyName + " or your app may be misconfigured.");
        }

        /// <summary>
        /// The get web config bool value.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="ConfigurationErrorsException">
        /// ConfigurationErrorsException
        /// </exception>
        public static bool GetWebConfigBoolValue(string propertyName)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(propertyName))
            {
                bool retVal;
                if (ConfigurationManager.AppSettings[propertyName] != null && bool.TryParse(ConfigurationManager.AppSettings[propertyName], out retVal))
                {
                    return retVal;
                }

                throw new ConfigurationErrorsException("Your Web.config has an AppSetting parameter called " + propertyName + ", but it either has no value or an invalid one.");
            }

            throw new ConfigurationErrorsException("Your Web.config does not have an AppSetting called " + propertyName + " or your app may be misconfigured.");
        }

        #endregion Utility methods

        #region Properties

        /// <summary>
        /// Gets the account id.
        /// </summary>
        public static int AccountId
        {
            get { return GetWebConfigIntValue("AccountID"); }
        }

        /// <summary>
        /// Gets the sub account id.
        /// </summary>
        public static int SubAccountId
        {
            get { return GetWebConfigIntValue("SubAccountID"); }
        }

        /// <summary>
        /// Gets the employee id.
        /// </summary>
        public static int EmployeeId
        {
            get { return GetWebConfigIntValue("EmployeeID"); }
        }

        /// <summary>
        /// Gets the alternative employee id.
        /// </summary>
        public static int AlternativeEmployeeId
        {
            get { return GetWebConfigIntValue("AlternativeEmployeeID"); }
        }

        /// <summary>
        /// Gets the active module id.
        /// </summary>
        public static int ActiveModuleId
        {
            get { return GetWebConfigIntValue("active_module"); }
        }

        /// <summary>
        /// Gets the active module.
        /// </summary>
        public static Modules ActiveModule
        {
            get { return (Modules)GetWebConfigIntValue("active_module"); }
        }

        /// <summary>
        /// Gets the delegate id.
        /// </summary>
        public static int DelegateId
        {
            get { return GetWebConfigIntValue("DelegateID"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether use delegate.
        /// </summary>
        public static bool UseDelegate { get; set; }

        /// <summary>
        /// Gets the images path.
        /// </summary>
        public static string ImagesPath
        {
            get { return GetWebConfigStringValue("UnitTestImagesPath"); }
        }

		public static string StaticLibraryPath
    	{
			get { return GetWebConfigStringValue("StaticLibraryPath"); }
    	}

		public static string StaticLibraryFolderLocation
		{
			get { return GetWebConfigStringValue("StaticLibraryFolderLocation"); }
		}

        #endregion Properties
    }



}
