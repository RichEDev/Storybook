namespace SpendManagementApi.Models.Types
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using SpendManagementApi.Interfaces;

    using SpendManagementLibrary;

    /// <summary>
    /// Represents a setting that can be enabled in general options to display these fields when claiming for an expense
    /// </summary>
    [MetadataType(typeof(IFieldSetting))]
    public class GeneralOptionsDisplayFieldSetting : BaseExternalType, IFieldSetting, IApiTypeToBaseClass<cFieldToDisplay, GeneralOptionsDisplayFieldSetting>
    {
        /// <summary>
        /// Gets or sets the generic code for the field
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the friendly name that can be set to the field
        /// </summary>
        public string DisplayAs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field should be displayed for cash items.
        /// </summary>
        public bool DisplayForCash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field should be displayed for credit card items.
        /// </summary>
        public bool DisplayForCreditCard { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field should be displayed for purchase card items.
        /// </summary>
        public bool DisplayForPurchaseCard { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field should be displayed in general details ot for individual items.
        /// </summary>
        public bool DisplayOnIndividualItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field should be mandatory for cash items.
        /// </summary>
        public bool MandatoryForCash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field should be mandatory for credit card items.
        /// </summary>
        public bool MandatoryForCreditCard { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field should be mandatory for purchase card items.
        /// </summary>
        public bool MandatoryForPurchaseCard { get; set; }

        /// <summary>
        /// Converts the API object FieldSetting to cFieldToDisplay
        /// </summary>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <returns>
        /// The <see cref="cFieldToDisplay"/>.
        /// </returns>
        public cFieldToDisplay ToBaseClass(IActionContext actionContext)
        {
            string fieldId;

            switch (this.Code)
            {
                case "organisation":
                    fieldId = "4D0F2409-0705-4F0F-9824-42057B25AEBE";
                    break;
                case "reason":
                    fieldId = "AF839FE7-8A52-4BD1-962C-8A87F22D4A10";
                    break;
                case "currency":
                    fieldId = "1EE53AE2-2CDF-41B4-9081-1789ADF03459";
                    break;
                case "country":
                    fieldId = "EC527561-DFEE-48C7-A126-0910F8E031B0";
                    break;
                case "otherdetails":
                    fieldId = "7CF61909-8D25-4230-84A9-F5701268F94B";
                    break;
                case "to":
                    fieldId = "B0A89FBD-641B-4D77-967B-2702CFC1787F";
                    break;
                case "from":
                    fieldId = "2CF623AE-B9CA-4298-95EB-E43B201C9EB6";
                    break;
                case "department":
                    fieldId = "9617A83E-6621-4B73-B787-193110511C17";
                    break;
                case "costcode":
                    fieldId = "359DFAC9-74E6-4BE5-949F-3FB224B1CBFC";
                    break;
                case "projectcode":
                    fieldId = "6D06B15E-A157-4F56-9FF2-E488D7647219";
                    break;
                default:
                    fieldId = string.Empty;
                    break;
            }

            return new cFieldToDisplay(
                new Guid(fieldId),
                this.Code,
                this.DisplayAs,
                this.DisplayForCash,
                this.MandatoryForCash,
                this.DisplayOnIndividualItem,
                this.DisplayForCreditCard,
                this.MandatoryForCreditCard,
                this.DisplayForPurchaseCard,
                this.MandatoryForPurchaseCard,
                DateTime.Now,
                0,
                DateTime.Now,
                0);
        }
    }
}