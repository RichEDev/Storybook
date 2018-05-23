namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;

    /// <summary>
    /// MobileAPI_jsonp interface which is used as a WCF contract..
    /// </summary>
    [ServiceContract(Namespace = "Spend_Management.Mobile", Name = "MobileAPI_jsonp")]
    public interface IMobileAPI_jsonp
    {
        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage GetServerDateTime();

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EmployeeBasic GetEmployeeBasicDetails(string pairingKey, string serialKey, int employeeID);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage PairDevice(string pairingKey, string serialKey);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        AddExpensesScreenDetails GetAddEditExpensesScreenSetup(string pairingKey, string serialKey);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CategoryResult GetExpenseItemCategories(string pairingKey, string serialKey);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        GeneralOptions GetGeneralOptions(string pairingKey, string serialKey);

        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ExpenseItemResult GetEmployeeSubCats(string pairingKey, string serialKey);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CurrencyResult GetCurrencyList(string pairingKey, string serialKey);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        AddExpenseItemResult SaveExpense(string pairingKey, string serialKey, List<ExpenseItem> items);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        UploadReceiptResult UploadReceipt(string pairingKey, string serialKey, int expenseID, string receipt);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ClaimToCheckResult GetClaimsAwaitingApproval(string pairingKey, string serialKey);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ExpenseItemsResult GetExpenseItemsByClaimID(string pairingKey, string serialKey, int claimID);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        SubcatResult GetSubcatList(string pairingKey, string serialKey);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage ApproveItems(string pairingKey, string serialKey, int claimid, List<int> lstItems);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage ReturnItems(string pairingKey, string serialKey, int claimid, string reason, List<int> lstItems);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ClaimItemResult GetExpenseItemByID(string pairingKey, string serialKey, int claimid, int expenseid);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        SubcatItemResult GetSubcatByID(string pairingKey, string serialKey, int subcatid);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ReceiptResult GetReceiptByID(string pairingKey, string serialKey, int expenseid);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage ApproveClaim(string pairingKey, string serialKey, int claimid);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ClaimToCheckCountResult GetClaimsAwaitingApprovalCount(string pairingKey, string serialKey);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage AllocateClaimForPayment(string pairingKey, string serialKey, int claimid);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage UnapproveItem(string pairingKey, string serialKey, int claimid, int expenseid);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage ValidatePairingKey(string pairingKey, string serialKey);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CompanyPolicyResult GetCompanyPolicy(string pairingKey, string serialKey);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        VersionResult GetCurrentVersion(string pairingKey, string serialKey, string typeKey);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        AllowanceTypesResult GetAllowanceTypes(string pairingKey, string serialKey);

        [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ServiceResultMessage Authenticate(string pairingKey, string serialKey);

    }
}
