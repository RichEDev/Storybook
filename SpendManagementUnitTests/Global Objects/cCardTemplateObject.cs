using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cCardTemplateObject
    {
        /// <summary>
        /// Get an Allstar Fuel card template object
        /// </summary>
        /// <returns></returns>
        public static cCardTemplate GetAllstarFuelCardTemplate()
        {
            cCardTemplate template = new cCardTemplate("Allstar Fuel Card", 2, 0, ImportType.FlatFile, ",", 0, new SortedList<CardRecordType, cCardRecordType>(), "\"");
            return template;
        }

        /// <summary>
        /// Get an AMEX Daily Text template object
        /// </summary>
        /// <returns></returns>
        public static cCardTemplate GetAMEXDailyTextTemplate()
        {
            cCardTemplate template = new cCardTemplate("AMEX Daily Text", 0, 0, ImportType.FixedWidth, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
            return template;
        }

        /// <summary>
        /// Get an AMEX Monthly Text template object
        /// </summary>
        /// <returns></returns>
        public static cCardTemplate GetAMEXMonthlyTextTemplate()
        {
            cCardTemplate template = new cCardTemplate("AMEX Monthly Text", 0, 1, ImportType.FixedWidth, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
            return template;
        }

        /// <summary>
        /// Get an AMEX Monthly XLS template object
        /// </summary>
        /// <returns></returns>
        public static cCardTemplate GetAMEXMonthlyXLSTemplate()
        {
            cCardTemplate template = new cCardTemplate("AMEX Monthly XLS", 2, 1, ImportType.Excel, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
            return template;
        }

        /// <summary>
        /// Get a Barclaycard Enhanced template object
        /// </summary>
        /// <returns></returns>
        public static cCardTemplate GetBarclaycardEnhancedTemplate()
        {
            cCardTemplate template = new cCardTemplate("Barclaycard Enhanced", 2, 2, ImportType.Excel, "", 1, new SortedList<CardRecordType, cCardRecordType>(), "");
            return template;
        }

        /// <summary>
        /// Get a Barclaycard VCF4 template object
        /// </summary>
        /// <returns></returns>
        public static cCardTemplate GetBarclaycardVCF4Template()
        {
            cCardTemplate template = new cCardTemplate("Barclaycard VCF4", 0, 0, ImportType.FlatFile, "\\t", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
            return template;
        }

        /// <summary>
        /// Get a Barclaycard template object
        /// </summary>
        /// <returns></returns>
        public static cCardTemplate GetBarclaycardTemplate()
        {
            cCardTemplate template = new cCardTemplate("Barclaycard", 1, 1, ImportType.FlatFile, "{", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
            return template;
        }

        /// <summary>
        /// Get a Diners Card template object
        /// </summary>
        /// <returns></returns>
        public static cCardTemplate GetDinersCardTemplate()
        {
            cCardTemplate template = new cCardTemplate("Diners Card", 4, 2, ImportType.Excel, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
            return template;
        }

        /// <summary>
        /// Get a Fuel Card xls template object
        /// </summary>
        /// <returns></returns>
        public static cCardTemplate GetFuelCardXLSTemplate()
        {
            cCardTemplate template = new cCardTemplate("Fuel Card xls", 3, 2, ImportType.Excel, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
            return template;
        }

        /// <summary>
        /// Get a MBNA Credit Card template object
        /// </summary>
        /// <returns></returns>
        public static cCardTemplate GetMBNACreditCardTemplate()
        {
            cCardTemplate template = new cCardTemplate("MBNA Credit Card", 1, 1, ImportType.Excel, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
            return template;
        }

        /// <summary>
        /// Get a Premier Inn XLS template object
        /// </summary>
        /// <returns></returns>
        public static cCardTemplate GetPremierInnXLSTemplate()
        {
            cCardTemplate template = new cCardTemplate("Premier Inn XLS", 2, 1, ImportType.Excel, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
            return template;
        }

        /// <summary>
        /// Get a RBS Credit Card template object
        /// </summary>
        /// <returns></returns>
        public static cCardTemplate GetRBSCreditCardTemplate()
        {
            cCardTemplate template = new cCardTemplate("RBS Credit Card", 2, 0, ImportType.Excel, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
            return template;
        }

        /// <summary>
        /// Get a RBS Purchase Card template object
        /// </summary>
        /// <returns></returns>
        public static cCardTemplate GetRBSPurchaseCardTemplate()
        {
            cCardTemplate template = new cCardTemplate("RBS Purchase Card", 2, 0, ImportType.Excel, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
            return template;
        }

        /// <summary>
        /// Get a card template by provider 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static cCardTemplate GetProviderTemplate(string provider)
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(cGlobalVariables.AccountID);
            cCardTemplate temp = clsCardTemplates.getTemplate(provider);
            return temp;
        }
    }
}
