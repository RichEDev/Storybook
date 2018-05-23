namespace SpendManagementLibrary.Mobile
{
    using Addresses;
    using Flags;
using System;
using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Serialization;

    [DataContract]
    public class ServiceResultMessage
    {
        #region properties

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public MobileReturnCode ReturnCode { get; set; }

        [DataMember]
        public string FunctionName { get; set; }

        #endregion
    }

    [DataContract]
    public class UploadReceiptResult : ServiceResultMessage
    {
        [DataMember]
        public int MobileID { get; set; }
    }

    [DataContract]
    public class CategoryResult : ServiceResultMessage
    {
        [DataMember]
        public List<Category> List { get; set; }
    }

    [DataContract]
    public class SubcatItemResult : ServiceResultMessage
    {
        [DataMember]
        public cSubcat subcat { get; set; }
    }

    [DataContract]
    public class ReasonResult : ServiceResultMessage
    {
        [DataMember]
        public List<Reason> List { get; set; }
    }

    [DataContract]
    public class CurrencyResult : ServiceResultMessage
    {
        [DataMember]
        public List<Currency> List { get; set; }
    }

    [DataContract]
    public class GeneralOptions : ServiceResultMessage
    {
        #region properties

        [DataMember]
        public string InitialDate { get; set; }

        [DataMember]
        public int? LimitMonths { get; set; }

        [DataMember]
        public bool FlagDate { get; set; }

        [DataMember]
        public string ServiceMessage { get; set; }

        [DataMember]
        public bool ClaimantDeclarationRequired { get; set; }

        [DataMember]
        public string ClaimantDeclaration { get; set; }

        [DataMember]
        public string ApproverDeclaration { get; set; }

        [DataMember]
        public bool AttachReceipts { get; set; }

        [DataMember]
        public bool EnableMobileDevices { get; set; }

        [DataMember]
        public bool AllowMultipleDestinations { get; set; }

        [DataMember]
        public bool PostcodesMandatory { get; set; }

        /// <summary>
        /// Gets or sets the postcode anywhere key.
        /// </summary>
        [DataMember]
        public string PostcodeAnywhereKey { get; set; }

        #endregion
    }

    [DataContract]
    public class AddExpenseItemResult : ServiceResultMessage
    {
        [DataMember]
        public SortedList<int, int> List { get; set; }
    }

    [DataContract]
    public class SaveExpenseItemResult : ServiceResultMessage
    {
        [DataMember]
        public SortedList<string, int> List { get; set; }
    }

    /// <summary>
    /// The response returned to mobile API calls to save a journey from a device
    /// </summary>
    [DataContract]
    public class SaveJourneyResult : ServiceResultMessage
    {
        /// <summary>
        /// Gets or sets the list of journeys
        /// </summary>
        [DataMember]
        public SortedList<string, int> List { get; set; }
    }

    [DataContract]
    public class ClaimToCheckResult : CheckAndPayRoleResult
    {
        [DataMember]
        public List<Claim> List { get; set; }
    }

    [DataContract]
    public class ClaimToCheckCountResult : CheckAndPayRoleResult
    {
        [DataMember]
        public int Count { get; set; }
    }

    [DataContract]
    public class ExpenseItemsResult : CheckAndPayRoleResult
    {
        [DataMember]
        public List<cExpenseItemResult> List { get; set; }
    }

    [DataContract]
    public class SubcatResult : ServiceResultMessage
    {
        [DataMember]
        public List<cSubcat> List { get; set; }
    }

    [DataContract]
    public class ExpenseItemResult : ServiceResultMessage
    {
        [DataMember]
        public List<ExpenseItemDetail> List { get; set; }
    }

    [DataContract]
    public class Reason
    {
        #region properties

        [DataMember]
        public string reason { get; set; }

        //[DataMember]
        public int accountid { get; set; }

        [DataMember]
        public int reasonid { get; set; }

        [DataMember]
        public string description { get; set; }

        [DataMember]
        public string accountcodevat { get; set; }

        [DataMember]
        public string accountcodenovat { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        [DataMember]
        public string ServiceMessage { get; set; }

        #endregion
    }

    [DataContract]
    public class VersionResult : ServiceResultMessage
    {
        [DataMember]
        public string VersionNumber { get; set; }

        [DataMember]
        public bool DisableAppUsage { get; set; }

        [DataMember]
        public string NotifyMessage { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string SyncMessage { get; set; }

        [DataMember]
        public string AppStoreURL { get; set; }

        [DataMember]
        public string ApiType { get; set; }
    }

    [DataContract]
    public class Currency
    {
        [DataMember]
        public int currencyID { get; set; }

        [DataMember]
        public string label { get; set; }

        [DataMember]
        public string symbol { get; set; }
    }

    [DataContract]
    public class Category
    {
        #region properties

        [DataMember]
        public int categoryid { get; set; }

        [DataMember]
        public string category { get; set; }

        [DataMember]
        public string description { get; set; }

        //[DataMember]
        public DateTime createdon { get; set; }

        //[DataMember]
        public int createdby { get; set; }

        //[DataMember]
        public DateTime modifiedon { get; set; }

        //[DataMember]
        public int modifiedby { get; set; }

        [DataMember]
        public string ServiceMessage { get; set; }

        #endregion
    }


    public class Claim
    {
        [DataMember]
        public int ClaimID { get; set; }

        [DataMember]
        public int ClaimNumber { get; set; }

        [DataMember]
        public string ClaimName { get; set; }

        [DataMember]
        public string EmployeeName { get; set; }

        [DataMember]
        public decimal Total { get; set; }

        [DataMember]
        public decimal ApprovedTotal { get; set; }

        [DataMember]
        public int BaseCurrency { get; set; }

        [DataMember]
        public int Stage { get; set; }

        [DataMember]
        public ClaimStatus Status { get; set; }

        [DataMember]
        public bool Approved { get; set; }

        [DataMember]
        public bool DisplayOneClickSignoff { get; set; }

        [DataMember]
        public bool DisplayDeclaration { get; set; }

    }

    [DataContract]
    public class ReceiptResult : CheckAndPayRoleResult
    {
        [DataMember]
        public string Receipt { get; set; }

        [DataMember]
        public string Extension { get; set; }

        [DataMember]
        public string mimeHeader { get; set; }
    }

    [DataContract]
    public class ExpenseItem
    {
        [DataMember]
        public int MobileID { get; set; }

        [DataMember]
        public bool IsQuickMileageItem { get; set; }

        [DataMember]
        public string OtherDetails { get; set; }

        [DataMember]
        public int? ReasonID { get; set; }

        [DataMember]
        public decimal Total { get; set; }

        [DataMember]
        public int SubcatID { get; set; }

        [DataMember]
        public string Date
        {
            get { return dtDate.ToShortDateString(); }
            set
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                dtDate = DateTime.ParseExact(value, "yyyyMMdd", provider);
            }
        }

        [DataMember]
        public int? CurrencyID { get; set; }

        [DataMember]
        public decimal Miles { get; set; }

        [DataMember]
        public string JourneySteps { get; set; }

        [DataMember]
        public double Quantity { get; set; }

        [DataMember]
        public string FromLocation { get; set; }

        [DataMember]
        public string ToLocation { get; set; }

        [DataMember]
        public string allowanceStartDate
        {
            get { return dtAllowanceStartDate.HasValue ? dtAllowanceStartDate.Value.ToShortDateString() : null; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    string format = "yyyyMMdd";

                    if (value.Length > 8)
                        format += " HH:mm:ss";

                    dtAllowanceStartDate = DateTime.ParseExact(value, format, provider);

                }
                else
                {
                    dtAllowanceStartDate = null;
                }
            }
        }

        [DataMember]
        public string allowanceEndDate
        {
            get { return dtAllowanceEndDate.HasValue ? dtAllowanceEndDate.Value.ToShortDateString() : null; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    string format = "yyyyMMdd";

                    if (value.Length > 8)
                        format += " HH:mm:ss";

                    dtAllowanceEndDate = DateTime.ParseExact(value, format, provider);
                }
                else
                {
                    dtAllowanceEndDate = null;
                }
            }
        }

        [DataMember]
        public string ItemNotes { get; set; }

        [DataMember]
        public decimal AllowanceDeductAmount { get; set; }

        [DataMember]
        public int? AllowanceTypeID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the employee who created the item
        /// </summary>
        public int CreatedBy { get; set; }

        public DateTime dtDate { get; set; }
        public DateTime? dtAllowanceStartDate { get; set; }
        public DateTime? dtAllowanceEndDate { get; set; }
        public bool HasReceipt { get; set; }
        public int? MobileDeviceTypeId { get; set; }
    }

    /// <summary>
    /// Class holding a journey posted from a mobile device.
    /// </summary>
    [DataContract]
    public class MobileJourney
    {
        /// <summary>
        /// Gets or sets the journey ID.
        /// </summary>
        [DataMember]
        public int JourneyId { get; set; }

        /// <summary>
        /// Gets or sets the subcat ID used for the journey.
        /// </summary>
        [DataMember]
        public int SubcatId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the employee who created this mobile journey.
        /// </summary>
        [DataMember]
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the steps to the journey.
        /// </summary>
        [DataMember]
        public string JourneyJson { get; set; }

        /// <summary>
        /// Gets or sets the date of the journey.
        /// </summary>
        [DataMember]
        public string JourneyDate
        {
            get
            {
                return this.JourneyDateTime.ToShortDateString();
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    string format = "yyyyMMdd";

                    if (value.Length > 8)
                    {
                        format += " HH:mm:ss";
                    }

                    this.JourneyDateTime = DateTime.ParseExact(value, format, provider);
                }
            }
        }

        /// <summary>
        /// Gets or sets the start time of the journey.
        /// </summary>
        [DataMember]
        public string StartTime
        {
            get
            {
                return this.JourneyStartTime.ToShortTimeString();
            }

            set
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                this.JourneyStartTime = DateTime.ParseExact(value, "HH:mm", provider);
            }

        }

        /// <summary>
        /// Gets or sets the journey start time as a <see cref="DateTime"/>.
        /// </summary>
        [DataMember]
        public DateTime JourneyStartTime { get; set; }

        /// <summary>
        /// Gets or sets the start time of the journey.
        /// </summary>
        [DataMember]
        public string EndTime
        {
            get
            {
                return this.JourneyEndTime.ToShortTimeString();
            }

            set
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                this.JourneyEndTime = DateTime.ParseExact(value, "HH:mm", provider);
            }
        }

        /// <summary>
        /// Gets or sets the journey end time as a <see cref="DateTime"/>.
        /// </summary>
        public DateTime JourneyEndTime { get; set; }

        /// <summary>
        /// Gets or sets the date/time of the journey
        /// </summary>
        public DateTime JourneyDateTime { get; set; }

        /// <summary>
        /// Gets or sets the list of steps on this journey.
        /// </summary>
        public List<MobileJourneyStep> Steps { get; set; }

        /// <summary>
        /// Gets or sets the list of whether the journey is still active
        /// </summary>
        [DataMember]
        public bool Active { get; set; }

    }

    /// <summary>
    /// An individual location visited from a mobile device.
    /// </summary>
    public class MobileJourneyStep
    {
        /// <summary>
        /// Gets or sets the step number.
        /// </summary>
        public int StepNumber { get; set; }

        /// <summary>
        /// Gets or sets the first line of the address.
        /// </summary>
        public string Line1 { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the postcode.
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the journey step contained heavy/bulky equipment.
        /// </summary>
        public bool HeavyBulkyEquipment { get; set; }

        /// <summary>
        /// Gets or sets the number of passengers on the journey step.
        /// Ultimately gets converted to a byte, hence the MaxValue range check.
        /// </summary>
        [Range(0, byte.MaxValue)]
        public byte NumberPassengers { get; set; }

        /// <summary>
        /// Gets or sets the names of the passengers for the journey step.
        /// </summary>
        public string PassengerNames { get; set; }

        /// <summary>
        /// Gets or sets the list of matched manual addresses
        /// </summary>
        public List<ManualAddress> ManualAddresses { get; set; }

        /// <summary>
        /// Gets or sets the Postcode Anywhere results
        /// </summary>
        public List<PostcodeAnywhereResult> PostcodeAnywhereResults { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public Address Address { get; set; }
    }

    /// <summary>
    /// This is a lightweight version of the Address struct to return data to the page from Autocomplete
    /// </summary>
    public struct ManualAddress
    {
        /// <summary>
        /// Gets or sets the global identifier.
        /// </summary>
        public string GlobalIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the friendly name.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Identifier { get; set; }

        /// <summary>
        /// Gets or sets the address line 1.
        /// </summary>
        public string Line1 { get; set; }

        /// <summary>
        /// Gets or sets the address line 2.
        /// </summary>
        public string Line2 { get; set; }

        /// <summary>
        /// Gets or sets the postcode.
        /// </summary>
        public string Postcode { get; set; }
    }

    /// <summary>
    /// This is the return response when querying PCA for retrievable addresses
    /// </summary>
    public struct PostcodeAnywhereResult
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the match position.
        /// </summary>
        public string Match { get; set; }

        /// <summary>
        /// Gets or sets the suggestion text.
        /// </summary>
        public string Suggestion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the result is retrievable.
        /// </summary>
        public bool IsRetrievable { get; set; }
    }


    [DataContract]
    public class DisplayField
    {
        #region properties

        //[DataMember]
        //public Guid fieldid { get; set; }

        [DataMember]
        public string code { get; set; }

        [DataMember]
        public string description { get; set; }

        [DataMember]
        public bool display { get; set; }

        [DataMember]
        public bool individualItem { get; set; }

        //[DataMember]
        //public bool mandatory { get; set; }

        //[DataMember]
        //public bool individual { get; set; }

        //[DataMember]
        //public bool displaycc { get; set; }

        //[DataMember]
        //public bool mandatorycc { get; set; }

        //[DataMember]
        //public bool displaypc { get; set; }

        //[DataMember]
        //public bool mandatorypc { get; set; }

        //[DataMember]
        //public DateTime createdon { get; set; }

        //[DataMember]
        //public int createdby { get; set; }

        //[DataMember]
        //public DateTime modifiedon { get; set; }

        //[DataMember]
        //public int modifiedby { get; set; }

        //[DataMember]
        //public string ServiceMessage { get; set; }

        #endregion
    }

    [DataContract]
    public class AddExpensesScreenDetails : ServiceResultMessage
    {
        [DataMember]
        public SortedList<string, DisplayField> Fields { get; set; }

        [DataMember]
        public List<DisplayField> List { get; set; }
    }

    [DataContract]
    public class ClaimItemResult : ServiceResultMessage
    {
        [DataMember]
        public cExpenseItem Item { get; set; }

        [DataMember]
        public cSubcat Subcat { get; set; }
    }

    /// <summary>
    /// Mobile API Employee class exposing required properties.
    /// </summary>
    [DataContract]
    public class EmployeeBasic : CheckAndPayRoleResult
    {
        #region properties

        [DataMember]
        public string firstname { get; set; }

        [DataMember]
        public string surname { get; set; }

        [DataMember]
        public int primaryCurrency { get; set; }

        [DataMember]
        public string ServiceMessage { get; set; }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class CheckAndPayRoleResult : ServiceResultMessage
    {
        [DataMember]
        public bool isApprover { get; set; }
    }

    /// <summary>
    /// Mobile API representation of cExpenseItemDetail
    /// </summary>
    [DataContract]
    public class ExpenseItemDetail
    {
        #region properties

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int Calculation { get; set; }

        [DataMember]
        public decimal AllowanceAmount { get; set; }

        [DataMember]
        public int CategoryID { get; set; }

        [DataMember]
        public bool VatNumberApp { get; set; }

        [DataMember]
        public bool VatNumberMandatory { get; set; }

        [DataMember]
        public bool ShowFrom { get; set; }

        [DataMember]
        public bool ShowTo { get; set; }

        /// <summary>
        /// Gets or sets whether the number of passengers should be shown on this expense item.
        /// </summary>
        [DataMember]
        public bool ShowNumberOfPassengers { get; set; }

        /// <summary>
        /// Gets or sets whether the passenger names field should be shown on this expense item.
        /// </summary>
        [DataMember]
        public bool ShowPassengerNames { get; set; }

        /// <summary>
        /// Gets or sets whether the option for heavy/bulky equipment should be shown on this expense item.
        /// </summary>
        [DataMember]
        public bool ShowHeavyBulky { get; set; }

        [DataMember]
        public string ServiceMessage { get; set; }

        #endregion
    }

    public class cExpenseItemResult
    {
        [DataMember]
        public cExpenseItem ExpenseItem { get; set; }

        [DataMember]
        public cSubcat Subcat { get; set; }

        [DataMember]
        public FlaggedItemsManager Flags { get; set; }
    }

    /// <summary>
    /// Mobile API class exposing required properties for Company Policy
    /// </summary>
    [DataContract]
    public class CompanyPolicyResult : ServiceResultMessage
    {
        #region properties

        /// <summary>
        /// Gets or sets the company policy text.
        /// </summary>
        [DataMember]
        public string CompanyPolicy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the policy is html.
        /// </summary>
        [DataMember]
        public bool isHTML { get; set; }

        /// <summary>
        /// Gets or sets the pdf policy url path
        /// </summary>
        [DataMember]
        public string PdfPolicyUrlPath { get; set; }

        #endregion
    }

    [DataContract]
    public class AllowanceTypesResult : ServiceResultMessage
    {
        #region properties

        [DataMember]
        public List<cAPIAllowance> AllowanceTypes { get; set; }

        #endregion
    }

    [DataContract]
    public class cAPIAllowance
    {
        [DataMember]
        public int AllowanceID { get; set; }

        [DataMember]
        public string Allowance { get; set; }

        [DataMember]
        public string Description { get; set; }

    }

    /// <summary>
    /// Class holding a receipt posted from mobile devices.
    /// </summary>
    public class ReceiptObject
    {
        public int MobileExpenseID { get; set; }
        public string Receipt { get; set; }
    }

}