using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using SpendManagementLibrary;
using SpendManagementLibrary.Flags;

namespace Spend_Management
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract(Namespace = "Spend_Management.Mobile", Name = "MobileAPI")]
    public interface IMobileAPI
    {
        [OperationContract]
        //[WebGet (BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage GetServerDateTime();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EmployeeBasic GetEmployeeBasicDetails(string pairingKey, string serialKey, int employeeID);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage PairDevice(string pairingKey, string serialKey);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        AddExpensesScreenDetails GetAddEditExpensesScreenSetup(string pairingKey, string serialKey);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CategoryResult GetExpenseItemCategories(string pairingKey, string serialKey);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ReasonResult GetReasonsList(string pairingKey, string serialKey);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        GeneralOptions GetGeneralOptions(string pairingKey, string serialKey);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ExpenseItemResult GetEmployeeSubCats(string pairingKey, string serialKey);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CurrencyResult GetCurrencyList(string pairingKey, string serialKey);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        AddExpenseItemResult SaveExpense(string pairingKey, string serialKey, List<ExpenseItem> items);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        UploadReceiptResult UploadReceipt(string pairingKey, string serialKey, int expenseID, string receipt);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ClaimToCheckResult GetClaimsAwaitingApproval(string pairingKey, string serialKey);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ExpenseItemsResult GetExpenseItemsByClaimID(string pairingKey, string serialKey, int claimID);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        SubcatResult GetSubcatList(string pairingKey, string serialKey);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage ApproveItems(string pairingKey, string serialKey, int claimid, List<int> lstItems);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage ReturnItems(string pairingKey, string serialKey, int claimid, string reason, List<int> lstItems);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ClaimItemResult GetExpenseItemByID(string pairingKey, string serialKey, int claimid, int expenseid);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        SubcatItemResult GetSubcatByID(string pairingKey, string serialKey, int subcatid);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ReceiptResult GetReceiptByID(string pairingKey, string serialKey, int expenseid);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage ApproveClaim(string pairingKey, string serialKey, int claimid);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ClaimToCheckCountResult GetClaimsAwaitingApprovalCount(string pairingKey, string serialKey);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage AllocateClaimForPayment(string pairingKey, string serialKey, int claimid);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage UnapproveItem(string pairingKey, string serialKey, int claimid, int expenseid);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage ValidatePairingKey(string pairingKey, string serialKey);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CompanyPolicyResult GetCompanyPolicy(string pairingKey, string serialKey);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        VersionResult GetCurrentVersion(string pairingKey, string serialKey, string typeKey);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        AllowanceTypesResult GetAllowanceTypes(string pairingKey, string serialKey);

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage Authenticate(string pairingKey, string serialKey);
    }

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

        /// <summary>
        /// Gets or sets a value indicating whether multiple destinations are enabled.
        /// </summary>
        [DataMember]
        public bool AllowMultipleDestinations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether postcodes are mandatory.
        /// </summary>
        [DataMember]
        public bool PostcodesMandatory { get; set; }

        #endregion
    }

    [DataContract]
    public class AddExpenseItemResult : ServiceResultMessage
    {
        [DataMember]
        public SortedList<int, int> List { get; set; }
    }

    [DataContract]
    public class ClaimToCheckResult : CheckAndPayRoleResult
    {
        [DataMember]
        public List<SpendManagementLibrary.Mobile.Claim> List { get; set; }
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
        public List<SubcatBasic> List { get; set; }
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

        //[DataMember]
        public DateTime createdon { get; set; }

        //[DataMember]
        public int createdby { get; set; }

        //[DataMember]
        public DateTime? modifiedon { get; set; }

        //[DataMember]
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
                if(!string.IsNullOrEmpty(value))
                {
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    string format = "yyyyMMdd";

                    if(value.Length > 8)
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
                if(!string.IsNullOrEmpty(value))
                {
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    string format = "yyyyMMdd";

                    if(value.Length > 8)
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

        public DateTime dtDate { get; set; }
        public DateTime? dtAllowanceStartDate { get; set; }
        public DateTime? dtAllowanceEndDate { get; set; }
        public bool HasReceipt { get; set; }
        public int? MobileDeviceTypeId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the employee who created the item
        /// </summary>
        public int CreatedBy { get; set; }
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

        [DataMember]
        public string CompanyPolicy { get; set; }

        [DataMember]
        public bool isHTML { get; set; }

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
}
