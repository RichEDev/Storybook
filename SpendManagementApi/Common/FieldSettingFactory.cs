namespace SpendManagementApi.Common
{
    using Interfaces;
    using SpendManagementLibrary;
    using Spend_Management;

    /// <summary>
    /// A Factory to get the FieldSettings for the supplied instance
    /// </summary>
    /// <typeparam name="T">The generic type</typeparam>
    public static class FieldSettingFactory<T> where T : IFieldSetting, new()
    {
        /// <summary>
        /// Gets the FieldSettings for the Type
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>The FieldSettings interface, populated with data</returns>
        public static IFieldSetting PopulateFieldSettings(int accountId)
        {
            cMisc misc = new cMisc(accountId);
            string fieldCode;
            var fieldsetting = new T();

            switch (fieldsetting.GetType().Name)
            {
                case "OrganisationSettings":
                    fieldCode = "organisation";
                    break;
                case "ReasonSettings":
                    fieldCode = "reason";
                    break;
                case "CurrencySettings":
                    fieldCode = "currency";
                    break;
                case "CountrySettings":
                    fieldCode = "country";
                    break;
                case "OtherDetailsSettings":
                    fieldCode = "otherdetails";
                    break;
                case "ToSettings":
                    fieldCode = "to";
                    break;
                case "FromSettings":
                    fieldCode = "from";
                    break;
                case "DepartmentSettings":
                    fieldCode = "department";
                    break;
                case "CostcodeSettings":
                    fieldCode = "costcode";
                    break;
                case "ProjectcodeSettings":
                    fieldCode = "projectcode";
                    break;
                default:
                    fieldCode = string.Empty;
                    break;
            }

            cFieldToDisplay fieldSettings = misc.GetGeneralFieldByCode(fieldCode);
            fieldsetting.DisplayAs = fieldSettings.description;
            fieldsetting.DisplayForCash = fieldSettings.display;
            fieldsetting.DisplayForCreditCard = fieldSettings.displaycc;
            fieldsetting.DisplayForPurchaseCard = fieldSettings.displaypc;
            fieldsetting.DisplayOnIndividualItem = fieldSettings.individual;
            fieldsetting.MandatoryForCash = fieldSettings.mandatory;
            fieldsetting.MandatoryForCreditCard = fieldSettings.mandatorycc;
            fieldsetting.MandatoryForPurchaseCard = fieldSettings.mandatorypc;

            return fieldsetting;
        }
    }
}