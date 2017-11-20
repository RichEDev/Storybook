namespace SpendManagementLibrary.Expedite
{
    using System.Collections.Generic;

    using SpendManagementLibrary.Enumerators.Expedite;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// The expedite validation helper class which holds expedite related icons and descriptions.
    /// </summary>
    public static class ExpediteValidationHelper
    {
       /// <summary>
       /// The business icon.
       /// </summary>
       public static string BusinessIcon => GlobalVariables.StaticContentLibrary + "/icons/16/plain/validation_business.png";

       /// <summary>
       /// The vat icon.
       /// </summary>
       public static string VatIcon => GlobalVariables.StaticContentLibrary + "/icons/16/plain/validation_vat.png";

       /// <summary>
       /// The fraud icon.
       /// </summary>
       public static string FraudIcon => GlobalVariables.StaticContentLibrary + "/icons/16/plain/validation_fraud.png";

       /// <summary>
       /// The custom icon.
       /// </summary>
       public static string CustomIcon => GlobalVariables.StaticContentLibrary + "/icons/16/plain/validation_custom.png";

       /// <summary>
       /// The header icon.
       /// </summary>
       public static string HeaderIcon => GlobalVariables.StaticContentLibrary + "/icons/16/plain/validation_header.png";

       /// <summary>
       /// The warning icon.
       /// </summary>
       public static string WarningIcon => GlobalVariables.StaticContentLibrary + "/icons/16/plain/validation_warning.png";

       /// <summary>
       /// The business type description.
       /// </summary>
       public static string BusinessTypeDescription => "Validated for business reasons";

       /// <summary>
       /// The vat type description.
       /// </summary>
       public static string VatTypeDescription => "Validated for VAT reclaim reasons";

       /// <summary>
       /// The custom type description.
       /// </summary>
       public static string CustomTypeDescription => "Validated for custom reasons";

       /// <summary>
       /// The fraud type description.
       /// </summary>
       public static string FraudTypeDescription => "Validated for VAT reclaim reasons";

        /// <summary>
        /// Gets a dictionary of status icons
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/> of status icons.
        /// </returns>
        public static Dictionary<ExpenseValidationResultStatus, string> GetStatusIcons()
        {
            var statusIcons = new Dictionary<ExpenseValidationResultStatus, string>
                                  {
                                      {
                                          ExpenseValidationResultStatus.Fail,
                                          GlobalVariables.StaticContentLibrary
                                          + "/icons/16/plain/validation_cross{0}.png"
                                      },
                                      {
                                          ExpenseValidationResultStatus
                                          .NotApplicable,
                                          GlobalVariables.StaticContentLibrary
                                          + "/icons/16/plain/validation_header{0}.png"
                                      },
                                      {
                                          ExpenseValidationResultStatus.Pass,
                                          GlobalVariables.StaticContentLibrary
                                          + "/icons/16/plain/validation_tick{0}.png"
                                      }
                                  };
            return statusIcons;
        }

        /// <summary>
        /// Gets a dictionary of status descriptions
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/> of status descriptions.
        /// </returns>
        public static Dictionary<ExpenseValidationResultStatus, string> GetStatusDescriptions()
        {
            var statusDescriptions = new Dictionary<ExpenseValidationResultStatus, string>
                                         {
                                             {
                                                 ExpenseValidationResultStatus
                                                 .Fail,
                                                 EnumHelpers
                                                 <
                                                 ExpenseValidationResultStatus
                                                 >.GetDisplayValue(
                                                     ExpenseValidationResultStatus
                                                 .Fail)
                                             },
                                             {
                                                 ExpenseValidationResultStatus
                                                 .NotApplicable,
                                                 EnumHelpers
                                                 <
                                                 ExpenseValidationResultStatus
                                                 >.GetDisplayValue(
                                                     ExpenseValidationResultStatus
                                                 .NotApplicable)
                                             },
                                             {
                                                 ExpenseValidationResultStatus
                                                 .Pass,
                                                 EnumHelpers
                                                 <
                                                 ExpenseValidationResultStatus
                                                 >.GetDisplayValue(
                                                     ExpenseValidationResultStatus
                                                 .Pass)
                                             }
                                         };
            return statusDescriptions;
        }
    }
}
