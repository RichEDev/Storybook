using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using SpendManagementUnitTests.Global_Objects;
using System.Reflection;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cAccountSubAccountsTest and is intended
    ///to contain all cAccountSubAccountsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cAccountSubAccountsTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            cSubAccountObject.CreateSubAccount();
        }
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            cSubAccountObject.DeleteSubAccount();
        }
        //
        #endregion


        /// <summary>
        ///A test for SaveAccountProperties
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cAccountSubAccountsTest_SaveAccountPropertiesTest()
        {
            int accountId = cGlobalVariables.AccountID;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subaccount = target.getSubAccountById(cGlobalVariables.SubAccountID);

            cAccountProperties safeProperties = subaccount.SubAccountProperties.Clone();
            cAccountProperties subAccountProperties = subaccount.SubAccountProperties.Clone();
            int EmployeeID = cGlobalVariables.EmployeeID;

            #region update values using reflection
            Type t = subAccountProperties.GetType();

            object oldVal;
            object newVal;
            string uniq;

            foreach (PropertyInfo pi in t.GetProperties())
            {
                oldVal = pi.GetValue(safeProperties, null);
                newVal = null;
                uniq = DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString();

                // DON'T CHANGE THE SUBACCOUNT!!
                if (pi.Name == "SubAccountID")
                {
                    continue;
                }


                // Should be an entry here for each type used in a property to create a value different to the one in the standard/current properties
                if (pi.PropertyType == typeof(Boolean))
                {
                    newVal = !((Boolean)oldVal);
                }
                else if (pi.PropertyType == typeof(Int16))
                {
                    newVal = (Int16)(((Int16)oldVal) + (Int16)1);
                }
                else if (pi.PropertyType == typeof(Int16?))
                {
                    if (((Int16?)oldVal).HasValue)
                    {
                        newVal = null;
                    }
                    else
                    {
                        newVal = (Int16)1;
                    }
                }
                else if (pi.PropertyType == typeof(Int32))
                {
                    newVal = (Int32)(((Int32)oldVal) + (Int32)1);
                }
                else if (pi.PropertyType == typeof(Int32?))
                {
                    if (((Int32?)oldVal).HasValue)
                    {
                        newVal = null;
                    }
                    else
                    {
                        newVal = (Int32)1;
                    }
                }
                else if (pi.PropertyType == typeof(String))
                {
                    newVal = (String)oldVal + (String)uniq;
                }
                else if (pi.PropertyType.BaseType == typeof(Enum))
                {
                    newVal = null;
                    Array tmpEnumVals = pi.PropertyType.GetEnumValues(); // get a list of the enum integer values as object array from the type
                    foreach (object o in tmpEnumVals)
                    {
                        if ((int)o != (int)oldVal)
                        {
                            newVal = Enum.ToObject(pi.PropertyType, (int)o);
                        }
                    }
                }
                else if (pi.PropertyType == typeof(DateTime))
                {
                    newVal = DateTime.UtcNow;
                }
                else if (pi.PropertyType == typeof(DateTime?))
                {
                    newVal = DateTime.UtcNow;
                }
                else if (pi.PropertyType == typeof(cRechargeSetting))
                {
                    newVal = null;
                }
                else if (pi.PropertyType == typeof(Decimal))
                {
                    newVal = (Decimal)(((Decimal)oldVal) + (Decimal)0.1);
                }
                else if (pi.PropertyType == typeof(Byte))
                {
                    newVal = ((Byte)oldVal > (Byte)0) ? (Byte)(((Byte)oldVal) - (Byte)1) : (Byte)1;
                }
                else if (pi.PropertyType == typeof(Guid))
                {
                    newVal = Guid.NewGuid();
                }
                else if (pi.PropertyType == typeof(Guid?))
                {
                    newVal = Guid.NewGuid();
                }
                else
                {
                    // let us know we've forgotten a type
                    throw new Exception("Unhandled Type = Name:" + pi.PropertyType.Name + "; BaseType:" + pi.PropertyType.BaseType + "; FullName:" + pi.PropertyType.FullName + ";");
                }

                pi.SetValue(subAccountProperties, newVal, null);
            }
            #endregion

            #region old manual method
            //subAccountProperties.ActivateCarOnUserAdd = !safeProperties.ActivateCarOnUserAdd;
            //subAccountProperties.AddLocations = !safeProperties.AddLocations;
            //subAccountProperties.AllowArchivedNotesAdd = !safeProperties.AllowArchivedNotesAdd;
            //subAccountProperties.AllowClaimantSelectHomeAddress = !safeProperties.AllowClaimantSelectHomeAddress;
            //subAccountProperties.AllowMultipleDestinations = !safeProperties.AllowMultipleDestinations;
            //subAccountProperties.AllowRecurring = !safeProperties.AllowRecurring;
            //subAccountProperties.AllowSelfReg = !safeProperties.AllowSelfReg;
            //subAccountProperties.AllowSelfRegAdvancesSignOff = !safeProperties.AllowSelfRegAdvancesSignOff;
            //subAccountProperties.AllowSelfRegBankDetails = !safeProperties.AllowSelfRegBankDetails;
            //subAccountProperties.AllowSelfRegCarDetails = !safeProperties.AllowSelfRegCarDetails;
            //subAccountProperties.AllowSelfRegDepartmentCostCode = !safeProperties.AllowSelfRegDepartmentCostCode;
            //subAccountProperties.AllowSelfRegEmployeeContact = !safeProperties.AllowSelfRegEmployeeContact;
            //subAccountProperties.AllowSelfRegEmployeeInfo = !safeProperties.AllowSelfRegEmployeeInfo;
            //subAccountProperties.AllowSelfRegHomeAddress = !safeProperties.AllowSelfRegHomeAddress;
            //subAccountProperties.AllowSelfRegItemRole = !safeProperties.AllowSelfRegItemRole;
            //subAccountProperties.AllowSelfRegRole = !safeProperties.AllowSelfRegRole;
            //subAccountProperties.AllowSelfRegSignOff = !safeProperties.AllowSelfRegSignOff;
            //subAccountProperties.AllowSelfRegUDF = !safeProperties.AllowSelfRegUDF;
            //subAccountProperties.AllowTeamMemberToApproveOwnClaim = !safeProperties.AllowTeamMemberToApproveOwnClaim;
            //subAccountProperties.AllowUsersToAddCars = !safeProperties.AllowUsersToAddCars;
            //subAccountProperties.ApplicationURL = "test url :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.ApproverDeclarationMsg = "test declaration msg :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks; ;
            //subAccountProperties.ArchiveGracePeriod = ++safeProperties.ArchiveGracePeriod;
            //subAccountProperties.AttachReceipts = !safeProperties.AttachReceipts;
            //subAccountProperties.AuditorEmailAddress = "audit" + DateTime.UtcNow + DateTime.UtcNow.Ticks + "@testemail.com";
            //subAccountProperties.AutoActivateType = (safeProperties.AutoActivateType == AutoActivateType.ActivateAuto ? AutoActivateType.ActivateManual : AutoActivateType.ActivateAuto);
            //subAccountProperties.AutoArchiveType = (safeProperties.AutoArchiveType == AutoArchiveType.ArchiveAuto ? AutoArchiveType.ArchiveManual : AutoArchiveType.ArchiveAuto);
            //subAccountProperties.AutoAssignAllocation = !safeProperties.AutoAssignAllocation;
            //subAccountProperties.AutoCalcHomeToLocation = !safeProperties.AutoCalcHomeToLocation;
            //subAccountProperties.AutoUpdateAnnualContractValue = !safeProperties.AutoUpdateAnnualContractValue;
            //subAccountProperties.AutoUpdateCVRechargeLive = !safeProperties.AutoUpdateCVRechargeLive;
            //subAccountProperties.AutoUpdateLicenceTotal = !safeProperties.AutoUpdateLicenceTotal;
            //if (safeProperties.BaseCurrency.HasValue)
            //{
            //    subAccountProperties.BaseCurrency = null;
            //}
            //else
            //{
            //    subAccountProperties.BaseCurrency = 1;
            //}
            //subAccountProperties.BlockCashCC = !safeProperties.BlockCashCC;
            //subAccountProperties.BlockCashPC = !safeProperties.BlockCashPC;
            //subAccountProperties.BlockInsuranceExpiry = !safeProperties.BlockInsuranceExpiry;
            //subAccountProperties.BlockLicenceExpiry = !safeProperties.BlockLicenceExpiry;
            //subAccountProperties.BlockMOTExpiry = !safeProperties.BlockMOTExpiry;
            //subAccountProperties.BlockTaxExpiry = !safeProperties.BlockTaxExpiry;
            //subAccountProperties.Broadcast = "test broadcast :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.cachePeriodLong = ++safeProperties.cachePeriodLong;
            //subAccountProperties.cachePeriodNormal = ++safeProperties.cachePeriodNormal;
            //subAccountProperties.cachePeriodShort = ++safeProperties.cachePeriodShort;
            //subAccountProperties.CacheTimeout = safeProperties.CacheTimeout + 1;
            //subAccountProperties.CalcHomeToLocation = !safeProperties.CalcHomeToLocation;
            //subAccountProperties.CCAdmin = !safeProperties.CCAdmin;
            //subAccountProperties.CCUserSettles = !safeProperties.CCUserSettles;
            //subAccountProperties.CheckESRAssignmentOnEmployeeAdd = !safeProperties.CheckESRAssignmentOnEmployeeAdd;
            //subAccountProperties.ClaimantDeclaration = !safeProperties.ClaimantDeclaration;
            //subAccountProperties.ClaimantsCanAddCompanyLocations = !safeProperties.ClaimantsCanAddCompanyLocations;
            //subAccountProperties.CommentsName = "test comment name :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.CompMileage = (subAccountProperties.CompMileage == 0 ? (byte)1 : (byte)0);
            //subAccountProperties.ContractCategoryTitle = "test category title :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.ContractCatMandatory = !safeProperties.ContractCatMandatory;
            //subAccountProperties.ContractDatesMandatory = !safeProperties.ContractDatesMandatory;
            //subAccountProperties.ContractDescShortTitle = "test short title :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.ContractDescTitle = "test title :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.ContractKey = "***:" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks; ;
            //subAccountProperties.ContractNumGen = !safeProperties.ContractNumGen;
            //subAccountProperties.ContractNumSeq = ++safeProperties.ContractNumSeq;
            //subAccountProperties.ContractScheduleDefault = "test schedule default :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.CostCodesOn = !safeProperties.CostCodesOn;
            //subAccountProperties.CountryName = "test country name :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.CurImportId = ++safeProperties.CurImportId;
            //subAccountProperties.CurrencyName = "test currency :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //switch ((CurrencyType)safeProperties.currencyType)
            //{
            //    case CurrencyType.Monthly:
            //        subAccountProperties.currencyType = CurrencyType.Range;
            //        break;
            //    case CurrencyType.Range:
            //        subAccountProperties.currencyType = CurrencyType.Static;
            //        break;
            //    case CurrencyType.Static:
            //        subAccountProperties.currencyType = CurrencyType.Monthly;
            //        break;
            //    default:
            //        subAccountProperties.currencyType = CurrencyType.Static;
            //        break;
            //}
            //subAccountProperties.CustomerHelpContactAddress = "test contact address :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.CustomerHelpContactEmailAddress = "test" + DateTime.UtcNow + DateTime.UtcNow.Ticks + "@helpemail.xyz";
            //subAccountProperties.CustomerHelpContactFax = "test help fax :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.CustomerHelpContactName = "test help contact name :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.CustomerHelpContactTelephone = "test customer help tel :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.CustomerHelpInformation = "<div>Test Help :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks + "</div>";
            //subAccountProperties.DateApprovedName = "test date approval name :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.DBVersion = ++safeProperties.DBVersion;
            //subAccountProperties.DeclarationMsg = "test decl msg :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //if (safeProperties.DefaultItemRole.HasValue)
            //{
            //    subAccountProperties.DefaultItemRole = null;
            //}
            //else
            //{
            //    subAccountProperties.DefaultItemRole = 1;
            //}
            //subAccountProperties.DefaultPageSize = ++safeProperties.DefaultPageSize;
            //if (safeProperties.DefaultRole.HasValue)
            //{
            //    subAccountProperties.DefaultRole = null;
            //}
            //else
            //{
            //    subAccountProperties.DefaultRole = 1;
            //}
            //subAccountProperties.DelApprovals = !safeProperties.DelApprovals;
            //subAccountProperties.DelAuditLog = !safeProperties.DelAuditLog;
            //subAccountProperties.DelCheckAndPay = !safeProperties.DelCheckAndPay;
            //subAccountProperties.DelCreditCards = !safeProperties.DelCreditCards;
            //subAccountProperties.DelEmployeeAccounts = !safeProperties.DelEmployeeAccounts;
            //subAccountProperties.DelEmployeeAdmin = !safeProperties.DelEmployeeAdmin;
            //subAccountProperties.DelExports = !safeProperties.DelExports;
            //subAccountProperties.DelPurchaseCards = !safeProperties.DelPurchaseCards;
            //subAccountProperties.DelQEDesign = !safeProperties.DelQEDesign;
            //subAccountProperties.DelReports = !safeProperties.DelReports;
            //subAccountProperties.DelReportsReadOnly = !safeProperties.DelReportsReadOnly;
            //subAccountProperties.DelSetup = !safeProperties.DelSetup;
            //subAccountProperties.DelSubmitClaim = !safeProperties.DelSubmitClaim;
            //subAccountProperties.DepartmentsOn = !safeProperties.DepartmentsOn;
            //subAccountProperties.DisplayFlagAdded = !safeProperties.DisplayFlagAdded;
            //subAccountProperties.DisplayLimits = !safeProperties.DisplayLimits;
            //subAccountProperties.DocumentRepository = "test doc rep :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //if (safeProperties.DrilldownReport.HasValue)
            //{
            //    subAccountProperties.DrilldownReport = null;
            //}
            //else
            //{
            //    subAccountProperties.DrilldownReport = Guid.NewGuid();
            //}
            //subAccountProperties.Duplicates = !safeProperties.Duplicates;
            //subAccountProperties.EditMyDetails = !safeProperties.EditMyDetails;
            //subAccountProperties.EmailAdministrator = "test administrator :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.EmailServerAddress = "test email server address :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.EmailServerFromAddress = "from" + DateTime.UtcNow + DateTime.UtcNow.Ticks + "@testsys.xyz";
            //subAccountProperties.EnableAttachmentHyperlink = !safeProperties.EnableAttachmentHyperlink;
            //subAccountProperties.EnableAttachmentUpload = !safeProperties.EnableAttachmentUpload;
            //subAccountProperties.EnableAutolog = !safeProperties.EnableAutolog;
            //subAccountProperties.EnableContractNumUpdate = !safeProperties.EnableContractNumUpdate;
            //subAccountProperties.EnableFlashingNotesIcon = !safeProperties.EnableFlashingNotesIcon;
            //subAccountProperties.EnableRecharge = !safeProperties.EnableRecharge;
            //subAccountProperties.EnableVariationAutoSeq = !safeProperties.EnableVariationAutoSeq;
            //subAccountProperties.EnterOdometerOnSubmit = !safeProperties.EnterOdometerOnSubmit;
            //subAccountProperties.ErrorEmailAddress = "error" + DateTime.UtcNow + DateTime.UtcNow.Ticks + "@testsys.xyz";
            //subAccountProperties.ErrorEmailFromAddress = "testfrom" + DateTime.UtcNow + DateTime.UtcNow.Ticks + "@testsys.xyz";
            //subAccountProperties.ExchangeRateName = "test xchg rate :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.ExchangeReadOnly = !safeProperties.ExchangeReadOnly;
            //subAccountProperties.FlagDate = !safeProperties.FlagDate;
            //subAccountProperties.FlagMessage = "test flag msg :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.FrequencyType = (safeProperties.FrequencyType == 0 ? (byte)1 : (byte)0);
            //subAccountProperties.FrequencyValue = ++safeProperties.FrequencyValue;
            //subAccountProperties.FYEnds = "fy end test :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.FYStarts = "fy start test :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.GlobalLocaleID = ++safeProperties.GlobalLocaleID;
            //subAccountProperties.HomeCountry = ++safeProperties.HomeCountry;
            //subAccountProperties.HomeToOffice=!safeProperties.HomeToOffice;
            //subAccountProperties.ImportCC=!safeProperties.ImportCC;
            //subAccountProperties.ImportHomeAddressFormat = "test import format :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.ImportPurchaseCard=!safeProperties.ImportPurchaseCard;
            //subAccountProperties.ImportUsernameFormat = "test import user format :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.IncreaseOthers=!safeProperties.IncreaseOthers;
            //subAccountProperties.InflatorActive=!safeProperties.InflatorActive;
            //if (safeProperties.InitialDate.HasValue)
            //{
            //    subAccountProperties.InitialDate = null;
            //}
            //else
            //{
            //    subAccountProperties.InitialDate = DateTime.Now;
            //}
            //subAccountProperties.InvoiceFreqActive=!safeProperties.InvoiceFreqActive;
            //subAccountProperties.KeepInvoiceForecasts=!safeProperties.KeepInvoiceForecasts;
            //subAccountProperties.Language = "test lang :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.LimitDates = !safeProperties.LimitDates;
            //subAccountProperties.LimitFrequency = !safeProperties.LimitFrequency;
            //if (safeProperties.LimitMonths.HasValue)
            //{
            //    subAccountProperties.LimitMonths = null;
            //}
            //else
            //{
            //    subAccountProperties.LimitMonths = 1;
            //}
            //subAccountProperties.Limits = (safeProperties.Limits == 0 ? (byte)1 : (byte)0);
            //subAccountProperties.LimitsReceipt = (safeProperties.LimitsReceipt == 0 ? (byte)1 : (byte)0);
            //subAccountProperties.LinkAttachmentDefault = (safeProperties.LinkAttachmentDefault == 0 ? (byte)1 : (byte)0);
            //subAccountProperties.LocationSearch = !safeProperties.LocationSearch;
            //subAccountProperties.LogoPath = "test logo path :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.MainAdministrator = ++safeProperties.MainAdministrator;
            //subAccountProperties.MandatoryPostcodeForAddresses = !safeProperties.MandatoryPostcodeForAddresses;
            //subAccountProperties.MaxClaimAmount = ++safeProperties.MaxClaimAmount;
            //subAccountProperties.MaxUploadSize = ++safeProperties.MaxUploadSize;
            //subAccountProperties.Mileage = ++safeProperties.Mileage;
            //subAccountProperties.MileageCalcType = (safeProperties.MileageCalcType == 0 ? (byte)1 : (byte)0);
            //subAccountProperties.MileagePrev = ++safeProperties.MileagePrev;
            //subAccountProperties.MinClaimAmount = ++safeProperties.MinClaimAmount;
            //subAccountProperties.OdometerDay = (subAccountProperties.OdometerDay == 1 ? (byte)2 : (byte)1);
            //subAccountProperties.OnlyCashCredit = !safeProperties.OnlyCashCredit;
            //subAccountProperties.OpenSaveAttachments = (safeProperties.OpenSaveAttachments == 0 ? (byte)1 : (byte)0);
            //subAccountProperties.OrderEndDateName = "test order end date name :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.OrderRecurrenceName = "test recurrence name :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.OverrideHome = !safeProperties.OverrideHome;
            //subAccountProperties.PartSubmit = !safeProperties.PartSubmit;
            //subAccountProperties.PenaltyClauseTitle = "test pc title :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.PolicyType = (safeProperties.PolicyType == 0 ? (byte)1 : (byte)0);
            //subAccountProperties.PONumberFormat = "test po format :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.PONumberGenerate = !safeProperties.PONumberGenerate;
            //subAccountProperties.PONumberName = "test po name :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.PONumberSequence = ++safeProperties.PONumberSequence;
            //subAccountProperties.PreApproval = !safeProperties.PreApproval;
            //subAccountProperties.ProductFieldType = (subAccountProperties.ProductFieldType != FieldType.AutoCompleteTextbox ? FieldType.AutoCompleteTextbox : FieldType.OTMSummary);
            //subAccountProperties.ProductName = "test product name :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.ProjectCodesOn = !safeProperties.ProjectCodesOn;
            //subAccountProperties.PurchaseCardSubCatId = ++safeProperties.PurchaseCardSubCatId;
            //subAccountProperties.PwdConstraint = (subAccountProperties.PwdConstraint == PasswordLength.AnyLength ? PasswordLength.Between : PasswordLength.AnyLength);
            //subAccountProperties.PwdExpires = !safeProperties.PwdExpires;
            //subAccountProperties.PwdExpiryDays = ++safeProperties.PwdExpiryDays;
            //subAccountProperties.PwdHistoryNum = ++safeProperties.PwdHistoryNum;
            //subAccountProperties.PwdLength1 = ++safeProperties.PwdLength1;
            //subAccountProperties.PwdLength2 = ++safeProperties.PwdLength2;
            //subAccountProperties.PwdMaxRetries = ++safeProperties.PwdMaxRetries;
            //subAccountProperties.PwdMustContainNumbers = !safeProperties.PwdMustContainNumbers;
            //subAccountProperties.PwdMustContainSymbol = !safeProperties.PwdMustContainSymbol;
            //subAccountProperties.PwdMustContainUpperCase = !safeProperties.PwdMustContainUpperCase;
            //subAccountProperties.RechargeUnrecoveredTitle = "test unrecovered title :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.RecordOdometer = !safeProperties.RecordOdometer;
            //subAccountProperties.RejectTip = !safeProperties.RejectTip;
            //subAccountProperties.SearchEmployees = !safeProperties.SearchEmployees;
            //subAccountProperties.SendReviewRequests = !safeProperties.SendReviewRequests;
            //subAccountProperties.ShowBankDetails = !safeProperties.ShowBankDetails;
            //subAccountProperties.ShowMileageCatsForUsers = !safeProperties.ShowMileageCatsForUsers;
            //subAccountProperties.ShowProductInSearch = !safeProperties.ShowProductInSearch;
            //subAccountProperties.ShowReviews = !safeProperties.ShowReviews;
            //subAccountProperties.SingleClaim = !safeProperties.SingleClaim;
            //subAccountProperties.SingleClaimCC = !safeProperties.SingleClaimCC;
            //subAccountProperties.SingleClaimPC = !safeProperties.SingleClaimPC;
            //subAccountProperties.SourceAddress = (safeProperties.SourceAddress == 0 ? (byte)1 : (byte)0);
            //subAccountProperties.Standards = "test standards :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.SupplierCatMandatory = !safeProperties.SupplierCatMandatory;
            //subAccountProperties.SupplierCatTitle = "test supplier cat title :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.SupplierFieldType = (safeProperties.SupplierFieldType == FieldType.AutoCompleteTextbox ? FieldType.Integer : FieldType.AutoCompleteTextbox);
            //subAccountProperties.SupplierFYEEnabled = !safeProperties.SupplierFYEEnabled;
            //subAccountProperties.SupplierIntContactEnabled = !safeProperties.SupplierIntContactEnabled;
            //subAccountProperties.SupplierLastFinCheckEnabled = !safeProperties.SupplierLastFinCheckEnabled;
            //subAccountProperties.SupplierLastFinStatusEnabled = !safeProperties.SupplierLastFinStatusEnabled;
            //subAccountProperties.SupplierNumEmployeesEnabled = !safeProperties.SupplierNumEmployeesEnabled;
            //subAccountProperties.SupplierPrimaryTitle = "test supplier primary title :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.SupplierRegionTitle = "test supplier region title :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.SupplierStatusEnforced = !safeProperties.SupplierStatusEnforced;
            //subAccountProperties.SupplierTurnoverEnabled = !safeProperties.SupplierTurnoverEnabled;
            //subAccountProperties.SupplierVariationTitle = "test variation title :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.TaskDueDateMandatory = !safeProperties.TaskDueDateMandatory;
            //subAccountProperties.TaskEndDateMandatory = !safeProperties.TaskEndDateMandatory;
            //subAccountProperties.TaskEscalationRepeat = ++safeProperties.TaskEscalationRepeat;
            //subAccountProperties.TaskStartDateMandatory = !safeProperties.TaskStartDateMandatory;
            //subAccountProperties.TermTypeActive = !safeProperties.TermTypeActive;
            //subAccountProperties.TipLimit = ++safeProperties.TipLimit;
            //subAccountProperties.TotalName = "test total name :" + DateTime.UtcNow + "::" + DateTime.UtcNow.Ticks;
            //subAccountProperties.UseCostCodeDescription = !safeProperties.UseCostCodeDescription;
            //subAccountProperties.UseCostCodeOnGenDetails = !safeProperties.UseCostCodeOnGenDetails;
            //subAccountProperties.UseCostCodes = !safeProperties.UseCostCodes;
            //subAccountProperties.UseCPExtraInfo = !safeProperties.UseCPExtraInfo;
            //subAccountProperties.UseDepartmentCodeDescription = !safeProperties.UseDepartmentCodeDescription;
            //subAccountProperties.UseDepartmentCodes = !safeProperties.UseDepartmentCodes;
            //subAccountProperties.UseDeptOnGenDetails = !safeProperties.UseDeptOnGenDetails;
            //subAccountProperties.UseMapPoint = !safeProperties.UseMapPoint;
            //subAccountProperties.UseProjectCodeDescription = !safeProperties.UseProjectCodeDescription;
            //subAccountProperties.UseProjectCodeOnGenDetails = !safeProperties.UseProjectCodeOnGenDetails;
            //subAccountProperties.UseProjectCodes = !safeProperties.UseProjectCodes;
            //subAccountProperties.Weekend = !safeProperties.Weekend;
            #endregion

            target.SaveAccountProperties(subAccountProperties, EmployeeID, null);
            target.InvalidateCache(cGlobalVariables.SubAccountID);

            cAccountSubAccounts new_subaccs = new cAccountSubAccounts(accountId);
            cAccountProperties newproperties = new_subaccs.getSubAccountById(cGlobalVariables.SubAccountID).SubAccountProperties.Clone();

            cCompareAssert.AreNothingEqual(safeProperties, newproperties, new List<string>() { "SubAccountID", "NumRows", "ThresholdType", "RechargeSettings", "EnableNotesUpdate", "MigrateUF", "EnableContractSavings" });
            #region old manual method
            //Assert.AreNotEqual(newproperties.ActivateCarOnUserAdd, safeProperties.ActivateCarOnUserAdd);
            //Assert.AreNotEqual(newproperties.AddLocations, safeProperties.AddLocations);
            //Assert.AreNotEqual(newproperties.AllowArchivedNotesAdd, safeProperties.AllowArchivedNotesAdd);
            //Assert.AreNotEqual(newproperties.AllowClaimantSelectHomeAddress, safeProperties.AllowClaimantSelectHomeAddress);
            //Assert.AreNotEqual(newproperties.AllowMultipleDestinations, safeProperties.AllowMultipleDestinations);
            //Assert.AreNotEqual(newproperties.AllowRecurring, safeProperties.AllowRecurring);
            //Assert.AreNotEqual(newproperties.AllowSelfReg, safeProperties.AllowSelfReg);
            //Assert.AreNotEqual(newproperties.AllowSelfRegAdvancesSignOff, safeProperties.AllowSelfRegAdvancesSignOff);
            //Assert.AreNotEqual(newproperties.AllowSelfRegBankDetails, safeProperties.AllowSelfRegBankDetails);
            //Assert.AreNotEqual(newproperties.AllowSelfRegCarDetails, safeProperties.AllowSelfRegCarDetails);
            //Assert.AreNotEqual(newproperties.AllowSelfRegDepartmentCostCode, safeProperties.AllowSelfRegDepartmentCostCode);
            //Assert.AreNotEqual(newproperties.AllowSelfRegEmployeeContact, safeProperties.AllowSelfRegEmployeeContact);
            //Assert.AreNotEqual(newproperties.AllowSelfRegEmployeeInfo, safeProperties.AllowSelfRegEmployeeInfo);
            //Assert.AreNotEqual(newproperties.AllowSelfRegHomeAddress, safeProperties.AllowSelfRegHomeAddress);
            //Assert.AreNotEqual(newproperties.AllowSelfRegItemRole, safeProperties.AllowSelfRegItemRole);
            //Assert.AreNotEqual(newproperties.AllowSelfRegRole, safeProperties.AllowSelfRegRole);
            //Assert.AreNotEqual(newproperties.AllowSelfRegSignOff, safeProperties.AllowSelfRegSignOff);
            //Assert.AreNotEqual(newproperties.AllowSelfRegUDF, safeProperties.AllowSelfRegUDF);
            //Assert.AreNotEqual(newproperties.AllowTeamMemberToApproveOwnClaim, safeProperties.AllowTeamMemberToApproveOwnClaim);
            //Assert.AreNotEqual(newproperties.AllowUsersToAddCars, safeProperties.AllowUsersToAddCars);
            //Assert.AreNotEqual(newproperties.ApplicationURL, safeProperties.ApplicationURL);
            //Assert.AreNotEqual(newproperties.ApproverDeclarationMsg, safeProperties.ApproverDeclarationMsg);
            //Assert.AreNotEqual(newproperties.ArchiveGracePeriod, safeProperties.ArchiveGracePeriod);
            //Assert.AreNotEqual(newproperties.AttachReceipts, safeProperties.AttachReceipts);
            //Assert.AreNotEqual(newproperties.AuditorEmailAddress, safeProperties.AuditorEmailAddress);
            //Assert.AreNotEqual(newproperties.AutoActivateType, safeProperties.AutoActivateType);
            //Assert.AreNotEqual(newproperties.AutoArchiveType, safeProperties.AutoArchiveType);
            //Assert.AreNotEqual(newproperties.AutoAssignAllocation, safeProperties.AutoAssignAllocation);
            //Assert.AreNotEqual(newproperties.AutoCalcHomeToLocation, safeProperties.AutoCalcHomeToLocation);
            //Assert.AreNotEqual(newproperties.AutoUpdateAnnualContractValue, safeProperties.AutoUpdateAnnualContractValue);
            //Assert.AreNotEqual(newproperties.AutoUpdateCVRechargeLive, safeProperties.AutoUpdateCVRechargeLive);
            //Assert.AreNotEqual(newproperties.AutoUpdateLicenceTotal, safeProperties.AutoUpdateLicenceTotal);
            //Assert.AreNotEqual(newproperties.BaseCurrency, safeProperties.BaseCurrency);
            //Assert.AreNotEqual(newproperties.BlockCashCC, safeProperties.BlockCashCC);
            //Assert.AreNotEqual(newproperties.BlockCashPC, safeProperties.BlockCashPC);
            //Assert.AreNotEqual(newproperties.BlockInsuranceExpiry, safeProperties.BlockInsuranceExpiry);
            //Assert.AreNotEqual(newproperties.BlockLicenceExpiry, safeProperties.BlockLicenceExpiry);
            //Assert.AreNotEqual(newproperties.BlockMOTExpiry, safeProperties.BlockMOTExpiry);
            //Assert.AreNotEqual(newproperties.BlockTaxExpiry, safeProperties.BlockTaxExpiry);
            //Assert.AreNotEqual(newproperties.Broadcast, safeProperties.Broadcast);
            //Assert.AreNotEqual(newproperties.cachePeriodLong, safeProperties.cachePeriodLong);
            //Assert.AreNotEqual(newproperties.cachePeriodNormal, safeProperties.cachePeriodNormal);
            //Assert.AreNotEqual(newproperties.cachePeriodShort, safeProperties.cachePeriodShort);
            //Assert.AreNotEqual(newproperties.CacheTimeout, safeProperties.CacheTimeout);
            //Assert.AreNotEqual(newproperties.CalcHomeToLocation, safeProperties.CalcHomeToLocation);
            //Assert.AreNotEqual(newproperties.CCAdmin, safeProperties.CCAdmin);
            //Assert.AreNotEqual(newproperties.CCUserSettles, safeProperties.CCUserSettles);
            //Assert.AreNotEqual(newproperties.CheckESRAssignmentOnEmployeeAdd, safeProperties.CheckESRAssignmentOnEmployeeAdd);
            //Assert.AreNotEqual(newproperties.ClaimantDeclaration, safeProperties.ClaimantDeclaration);
            //Assert.AreNotEqual(newproperties.ClaimantsCanAddCompanyLocations, safeProperties.ClaimantsCanAddCompanyLocations);
            //Assert.AreNotEqual(newproperties.CommentsName, safeProperties.CommentsName);
            //Assert.AreNotEqual(newproperties.CompMileage, safeProperties.CompMileage);
            //Assert.AreNotEqual(newproperties.ContractCategoryTitle, safeProperties.ContractCategoryTitle);
            //Assert.AreNotEqual(newproperties.ContractCatMandatory, safeProperties.ContractCatMandatory);
            //Assert.AreNotEqual(newproperties.ContractDatesMandatory, safeProperties.ContractDatesMandatory);
            //Assert.AreNotEqual(newproperties.ContractDescShortTitle, safeProperties.ContractDescShortTitle);
            //Assert.AreNotEqual(newproperties.ContractDescTitle, safeProperties.ContractDescTitle);
            //Assert.AreNotEqual(newproperties.ContractKey, safeProperties.ContractKey);
            //Assert.AreNotEqual(newproperties.ContractNumGen, safeProperties.ContractNumGen);
            //Assert.AreNotEqual(newproperties.ContractNumSeq, safeProperties.ContractNumSeq);
            //Assert.AreNotEqual(newproperties.ContractScheduleDefault, safeProperties.ContractScheduleDefault);
            //Assert.AreNotEqual(newproperties.CostCodesOn, safeProperties.CostCodesOn);
            //Assert.AreNotEqual(newproperties.CountryName, safeProperties.CountryName);
            //Assert.AreNotEqual(newproperties.CurImportId, safeProperties.CurImportId);
            //Assert.AreNotEqual(newproperties.CurrencyName, safeProperties.CurrencyName);
            //Assert.AreNotEqual(newproperties.currencyType, safeProperties.currencyType);
            //Assert.AreNotEqual(newproperties.CustomerHelpContactAddress, safeProperties.CustomerHelpContactAddress);
            //Assert.AreNotEqual(newproperties.CustomerHelpContactEmailAddress, safeProperties.CustomerHelpContactEmailAddress);
            //Assert.AreNotEqual(newproperties.CustomerHelpContactFax, safeProperties.CustomerHelpContactFax);
            //Assert.AreNotEqual(newproperties.CustomerHelpContactName, safeProperties.CustomerHelpContactName);
            //Assert.AreNotEqual(newproperties.CustomerHelpContactTelephone, safeProperties.CustomerHelpContactTelephone);
            //Assert.AreNotEqual(newproperties.CustomerHelpInformation, safeProperties.CustomerHelpInformation);
            //Assert.AreNotEqual(newproperties.DateApprovedName, safeProperties.DateApprovedName);
            //Assert.AreNotEqual(newproperties.DBVersion, safeProperties.DBVersion);
            //Assert.AreNotEqual(newproperties.DeclarationMsg, safeProperties.DeclarationMsg);
            //Assert.AreNotEqual(newproperties.DefaultItemRole, safeProperties.DefaultItemRole);
            //Assert.AreNotEqual(newproperties.DefaultPageSize, safeProperties.DefaultPageSize);
            //Assert.AreNotEqual(newproperties.DefaultRole, safeProperties.DefaultRole);
            //Assert.AreNotEqual(newproperties.DelApprovals, safeProperties.DelApprovals);
            //Assert.AreNotEqual(newproperties.DelAuditLog, safeProperties.DelAuditLog);
            //Assert.AreNotEqual(newproperties.DelCheckAndPay, safeProperties.DelCheckAndPay);
            //Assert.AreNotEqual(newproperties.DelCreditCards, safeProperties.DelCreditCards);
            //Assert.AreNotEqual(newproperties.DelEmployeeAccounts, safeProperties.DelEmployeeAccounts);
            //Assert.AreNotEqual(newproperties.DelEmployeeAdmin, safeProperties.DelEmployeeAdmin);
            //Assert.AreNotEqual(newproperties.DelExports, safeProperties.DelExports);
            //Assert.AreNotEqual(newproperties.DelPurchaseCards, safeProperties.DelPurchaseCards);
            //Assert.AreNotEqual(newproperties.DelQEDesign, safeProperties.DelQEDesign);
            //Assert.AreNotEqual(newproperties.DelReports, safeProperties.DelReports);
            //Assert.AreNotEqual(newproperties.DelReportsReadOnly, safeProperties.DelReportsReadOnly);
            //Assert.AreNotEqual(newproperties.DelSetup, safeProperties.DelSetup);
            //Assert.AreNotEqual(newproperties.DelSubmitClaim, safeProperties.DelSubmitClaim);
            //Assert.AreNotEqual(newproperties.DepartmentsOn, safeProperties.DepartmentsOn);
            //Assert.AreNotEqual(newproperties.DisplayFlagAdded, safeProperties.DisplayFlagAdded);
            //Assert.AreNotEqual(newproperties.DisplayLimits, safeProperties.DisplayLimits);
            //Assert.AreNotEqual(newproperties.DocumentRepository, safeProperties.DocumentRepository);
            //Assert.AreNotEqual(newproperties.DrilldownReport, safeProperties.DrilldownReport);
            //Assert.AreNotEqual(newproperties.Duplicates, safeProperties.Duplicates);
            //Assert.AreNotEqual(newproperties.EditMyDetails, safeProperties.EditMyDetails);
            //Assert.AreNotEqual(newproperties.EmailAdministrator, safeProperties.EmailAdministrator);
            //Assert.AreNotEqual(newproperties.EmailServerAddress, safeProperties.EmailServerAddress);
            //Assert.AreNotEqual(newproperties.EmailServerFromAddress, safeProperties.EmailServerFromAddress);
            //Assert.AreNotEqual(newproperties.EnableAttachmentHyperlink, safeProperties.EnableAttachmentHyperlink);
            //Assert.AreNotEqual(newproperties.EnableAttachmentUpload, safeProperties.EnableAttachmentUpload);
            //Assert.AreNotEqual(newproperties.EnableAutolog, safeProperties.EnableAutolog);
            //Assert.AreNotEqual(newproperties.EnableContractNumUpdate, safeProperties.EnableContractNumUpdate);
            //Assert.AreNotEqual(newproperties.EnableFlashingNotesIcon, safeProperties.EnableFlashingNotesIcon);
            //Assert.AreNotEqual(newproperties.EnableRecharge, safeProperties.EnableRecharge);
            //Assert.AreNotEqual(newproperties.EnableVariationAutoSeq, safeProperties.EnableVariationAutoSeq);
            //Assert.AreNotEqual(newproperties.EnterOdometerOnSubmit, safeProperties.EnterOdometerOnSubmit);
            //Assert.AreNotEqual(newproperties.ErrorEmailAddress, safeProperties.ErrorEmailAddress);
            //Assert.AreNotEqual(newproperties.ErrorEmailFromAddress, safeProperties.ErrorEmailFromAddress);
            //Assert.AreNotEqual(newproperties.ExchangeRateName, safeProperties.ExchangeRateName);
            //Assert.AreNotEqual(newproperties.ExchangeReadOnly, safeProperties.ExchangeReadOnly);
            //Assert.AreNotEqual(newproperties.FlagDate, safeProperties.FlagDate);
            //Assert.AreNotEqual(newproperties.FlagMessage, safeProperties.FlagMessage);
            //Assert.AreNotEqual(newproperties.FrequencyType, safeProperties.FrequencyType);
            //Assert.AreNotEqual(newproperties.FrequencyValue, safeProperties.FrequencyValue);
            //Assert.AreNotEqual(newproperties.FYEnds, safeProperties.FYEnds);
            //Assert.AreNotEqual(newproperties.FYStarts, safeProperties.FYStarts);
            //Assert.AreNotEqual(newproperties.GlobalLocaleID, safeProperties.GlobalLocaleID);
            //Assert.AreNotEqual(newproperties.HomeCountry, safeProperties.HomeCountry);
            //Assert.AreNotEqual(newproperties.HomeToOffice, safeProperties.HomeToOffice);
            //Assert.AreNotEqual(newproperties.ImportCC, safeProperties.ImportCC);
            //Assert.AreNotEqual(newproperties.ImportHomeAddressFormat, safeProperties.ImportHomeAddressFormat);
            //Assert.AreNotEqual(newproperties.ImportPurchaseCard, safeProperties.ImportPurchaseCard);
            //Assert.AreNotEqual(newproperties.ImportUsernameFormat, safeProperties.ImportUsernameFormat);
            //Assert.AreNotEqual(newproperties.IncreaseOthers, safeProperties.IncreaseOthers);
            //Assert.AreNotEqual(newproperties.InflatorActive, safeProperties.InflatorActive);
            //Assert.AreNotEqual(newproperties.InitialDate, safeProperties.InitialDate);
            //Assert.AreNotEqual(newproperties.InvoiceFreqActive, safeProperties.InvoiceFreqActive);
            //Assert.AreNotEqual(newproperties.KeepInvoiceForecasts, safeProperties.KeepInvoiceForecasts);
            //Assert.AreNotEqual(newproperties.Language, safeProperties.Language);
            //Assert.AreNotEqual(newproperties.LimitDates, safeProperties.LimitDates);
            //Assert.AreNotEqual(newproperties.LimitFrequency, safeProperties.LimitFrequency);
            //Assert.AreNotEqual(newproperties.LimitMonths, safeProperties.LimitMonths);
            //Assert.AreNotEqual(newproperties.Limits, safeProperties.Limits);
            //Assert.AreNotEqual(newproperties.LimitsReceipt, safeProperties.LimitsReceipt);
            //Assert.AreNotEqual(newproperties.LinkAttachmentDefault, safeProperties.LinkAttachmentDefault);
            //Assert.AreNotEqual(newproperties.LocationSearch, safeProperties.LocationSearch);
            //Assert.AreNotEqual(newproperties.LogoPath, safeProperties.LogoPath);
            //Assert.AreNotEqual(newproperties.MainAdministrator, safeProperties.MainAdministrator);
            //Assert.AreNotEqual(newproperties.MandatoryPostcodeForAddresses, safeProperties.MandatoryPostcodeForAddresses);
            //Assert.AreNotEqual(newproperties.MaxClaimAmount, safeProperties.MaxClaimAmount);
            //Assert.AreNotEqual(newproperties.MaxUploadSize, safeProperties.MaxUploadSize);
            //Assert.AreNotEqual(newproperties.Mileage, safeProperties.Mileage);
            //Assert.AreNotEqual(newproperties.MileageCalcType, safeProperties.MileageCalcType);
            //Assert.AreNotEqual(newproperties.MileagePrev, safeProperties.MileagePrev);
            //Assert.AreNotEqual(newproperties.MinClaimAmount, safeProperties.MinClaimAmount);
            //Assert.AreNotEqual(newproperties.OdometerDay, safeProperties.OdometerDay);
            //Assert.AreNotEqual(newproperties.OnlyCashCredit, safeProperties.OnlyCashCredit);
            //Assert.AreNotEqual(newproperties.OpenSaveAttachments, safeProperties.OpenSaveAttachments);
            //Assert.AreNotEqual(newproperties.OrderEndDateName, safeProperties.OrderEndDateName);
            //Assert.AreNotEqual(newproperties.OrderRecurrenceName, safeProperties.OrderRecurrenceName);
            //Assert.AreNotEqual(newproperties.OverrideHome, safeProperties.OverrideHome);
            //Assert.AreNotEqual(newproperties.PartSubmit, safeProperties.PartSubmit);
            //Assert.AreNotEqual(newproperties.PenaltyClauseTitle, safeProperties.PenaltyClauseTitle);
            //Assert.AreNotEqual(newproperties.PolicyType, safeProperties.PolicyType);
            //Assert.AreNotEqual(newproperties.PONumberFormat, safeProperties.PONumberFormat);
            //Assert.AreNotEqual(newproperties.PONumberGenerate, safeProperties.PONumberGenerate);
            //Assert.AreNotEqual(newproperties.PONumberName, safeProperties.PONumberName);
            //Assert.AreNotEqual(newproperties.PONumberSequence, safeProperties.PONumberSequence);
            //Assert.AreNotEqual(newproperties.PreApproval, safeProperties.PreApproval);
            //Assert.AreNotEqual(newproperties.ProductFieldType, safeProperties.ProductFieldType);
            //Assert.AreNotEqual(newproperties.ProductName, safeProperties.ProductName);
            //Assert.AreNotEqual(newproperties.ProjectCodesOn, safeProperties.ProjectCodesOn);
            //Assert.AreNotEqual(newproperties.PurchaseCardSubCatId, safeProperties.PurchaseCardSubCatId);
            //Assert.AreNotEqual(newproperties.PwdConstraint, safeProperties.PwdConstraint);
            //Assert.AreNotEqual(newproperties.PwdExpires, safeProperties.PwdExpires);
            //Assert.AreNotEqual(newproperties.PwdExpiryDays, safeProperties.PwdExpiryDays);
            //Assert.AreNotEqual(newproperties.PwdHistoryNum, safeProperties.PwdHistoryNum);
            //Assert.AreNotEqual(newproperties.PwdLength1, safeProperties.PwdLength1);
            //Assert.AreNotEqual(newproperties.PwdLength2, safeProperties.PwdLength2);
            //Assert.AreNotEqual(newproperties.PwdMaxRetries, safeProperties.PwdMaxRetries);
            //Assert.AreNotEqual(newproperties.PwdMustContainNumbers, safeProperties.PwdMustContainNumbers);
            //Assert.AreNotEqual(newproperties.PwdMustContainSymbol, safeProperties.PwdMustContainSymbol);
            //Assert.AreNotEqual(newproperties.PwdMustContainUpperCase, safeProperties.PwdMustContainUpperCase);
            //Assert.AreNotEqual(newproperties.RechargeUnrecoveredTitle, safeProperties.RechargeUnrecoveredTitle);
            //Assert.AreNotEqual(newproperties.RecordOdometer, safeProperties.RecordOdometer);
            //Assert.AreNotEqual(newproperties.RejectTip, safeProperties.RejectTip);
            //Assert.AreNotEqual(newproperties.SearchEmployees, safeProperties.SearchEmployees);
            //Assert.AreNotEqual(newproperties.SendReviewRequests, safeProperties.SendReviewRequests);
            //Assert.AreNotEqual(newproperties.ShowBankDetails, safeProperties.ShowBankDetails);
            //Assert.AreNotEqual(newproperties.ShowMileageCatsForUsers, safeProperties.ShowMileageCatsForUsers);
            //Assert.AreNotEqual(newproperties.ShowProductInSearch, safeProperties.ShowProductInSearch);
            //Assert.AreNotEqual(newproperties.ShowReviews, safeProperties.ShowReviews);
            //Assert.AreNotEqual(newproperties.SingleClaim, safeProperties.SingleClaim);
            //Assert.AreNotEqual(newproperties.SingleClaimCC, safeProperties.SingleClaimCC);
            //Assert.AreNotEqual(newproperties.SingleClaimPC, safeProperties.SingleClaimPC);
            //Assert.AreNotEqual(newproperties.SourceAddress, safeProperties.SourceAddress);
            //Assert.AreNotEqual(newproperties.Standards, safeProperties.Standards);
            //Assert.AreNotEqual(newproperties.SupplierCatMandatory, safeProperties.SupplierCatMandatory);
            //Assert.AreNotEqual(newproperties.SupplierCatTitle, safeProperties.SupplierCatTitle);
            //Assert.AreNotEqual(newproperties.SupplierFieldType, safeProperties.SupplierFieldType);
            //Assert.AreNotEqual(newproperties.SupplierFYEEnabled, safeProperties.SupplierFYEEnabled);
            //Assert.AreNotEqual(newproperties.SupplierIntContactEnabled, safeProperties.SupplierIntContactEnabled);
            //Assert.AreNotEqual(newproperties.SupplierLastFinCheckEnabled, safeProperties.SupplierLastFinCheckEnabled);
            //Assert.AreNotEqual(newproperties.SupplierLastFinStatusEnabled, safeProperties.SupplierLastFinStatusEnabled);
            //Assert.AreNotEqual(newproperties.SupplierNumEmployeesEnabled, safeProperties.SupplierNumEmployeesEnabled);
            //Assert.AreNotEqual(newproperties.SupplierPrimaryTitle, safeProperties.SupplierPrimaryTitle);
            //Assert.AreNotEqual(newproperties.SupplierRegionTitle, safeProperties.SupplierRegionTitle);
            //Assert.AreNotEqual(newproperties.SupplierStatusEnforced, safeProperties.SupplierStatusEnforced);
            //Assert.AreNotEqual(newproperties.SupplierTurnoverEnabled, safeProperties.SupplierTurnoverEnabled);
            //Assert.AreNotEqual(newproperties.SupplierVariationTitle, safeProperties.SupplierVariationTitle);
            //Assert.AreNotEqual(newproperties.TaskDueDateMandatory, safeProperties.TaskDueDateMandatory);
            //Assert.AreNotEqual(newproperties.TaskEndDateMandatory, safeProperties.TaskEndDateMandatory);
            //Assert.AreNotEqual(newproperties.TaskEscalationRepeat, safeProperties.TaskEscalationRepeat);
            //Assert.AreNotEqual(newproperties.TaskStartDateMandatory, safeProperties.TaskStartDateMandatory);
            //Assert.AreNotEqual(newproperties.TermTypeActive, safeProperties.TermTypeActive);
            //Assert.AreNotEqual(newproperties.TipLimit, safeProperties.TipLimit);
            //Assert.AreNotEqual(newproperties.TotalName, safeProperties.TotalName);
            //Assert.AreNotEqual(newproperties.UseCostCodeDescription, safeProperties.UseCostCodeDescription);
            //Assert.AreNotEqual(newproperties.UseCostCodeOnGenDetails, safeProperties.UseCostCodeOnGenDetails);
            //Assert.AreNotEqual(newproperties.UseCostCodes, safeProperties.UseCostCodes);
            //Assert.AreNotEqual(newproperties.UseCPExtraInfo, safeProperties.UseCPExtraInfo);
            //Assert.AreNotEqual(newproperties.UseDepartmentCodeDescription, safeProperties.UseDepartmentCodeDescription);
            //Assert.AreNotEqual(newproperties.UseDepartmentCodes, safeProperties.UseDepartmentCodes);
            //Assert.AreNotEqual(newproperties.UseDeptOnGenDetails, safeProperties.UseDeptOnGenDetails);
            //Assert.AreNotEqual(newproperties.UseMapPoint, safeProperties.UseMapPoint);
            //Assert.AreNotEqual(newproperties.UseProjectCodeDescription, safeProperties.UseProjectCodeDescription);
            //Assert.AreNotEqual(newproperties.UseProjectCodeOnGenDetails, safeProperties.UseProjectCodeOnGenDetails);
            //Assert.AreNotEqual(newproperties.UseProjectCodes, safeProperties.UseProjectCodes);
            //Assert.AreNotEqual(newproperties.Weekend, safeProperties.Weekend);
            #endregion

            // reset properties back to original values
            target.SaveAccountProperties(safeProperties, EmployeeID, null);
            target.InvalidateCache(safeProperties.SubAccountID);
        }

        /// <summary>
        ///A test for CreateDropDown
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cAccountSubAccountsTest_CreateDropDownTest()
        {
            int accountId = cGlobalVariables.AccountID;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subacc = target.getSubAccountById(cGlobalVariables.SubAccountID);

            ListItem[] actual = target.CreateDropDown(cGlobalVariables.SubAccountID);
            Assert.IsTrue(actual.Length > 0);

            int cnt = 0;
            foreach (ListItem li in actual)
            {
                if (li.Value == subacc.SubAccountID.ToString())
                {
                    cnt++;
                }
            }

            Assert.IsTrue(cnt == 1);
        }

        /// <summary>
        ///A test for cAccountSubAccounts Constructor
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cAccountSubAccountsTest_cAccountSubAccountsConstructorTest()
        {
            int accountId = cGlobalVariables.AccountID;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subacc = target.getSubAccountById(cGlobalVariables.SubAccountID);
            
            Assert.IsNotNull(target);
            Assert.IsTrue(target.Count > 0);
            Assert.IsNotNull(subacc);
        }

        ///// <summary>
        /////A test for CreateFilteredDropDown
        /////</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cAccountSubAccountsTest_CreateFilteredDropDownTest()
        {
            int accountId = cGlobalVariables.AccountID;
            cSubAccountObject.GrantAccessRole(cGlobalVariables.SubAccountID);

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subacc = target.getSubAccountById(cGlobalVariables.SubAccountID);

            int employeeId = cGlobalVariables.EmployeeID;

            ListItem[] actual;
            actual = target.CreateFilteredDropDown(employeeId, cGlobalVariables.SubAccountID);

            cSubAccountObject.CleanupAccessRoles();

            Assert.IsNotNull(actual);

            int cnt = 0;
            foreach (ListItem li in actual)
            {
                if (li.Value == cGlobalVariables.SubAccountID.ToString())
                {
                    cnt++;
                }
            }

            Assert.IsTrue(cnt == 1);
        }

        /// <summary>
        ///A test for SaveProperties
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cAccountSubAccountsTest_SavePropertiesTest()
        {
            int accountId = cGlobalVariables.AccountID;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subacc = target.getSubAccountById(cGlobalVariables.SubAccountID);

            cAccountProperties clsproperties = target.getSubAccountById(cGlobalVariables.SubAccountID).SubAccountProperties;

            Dictionary<string, string> properties = new Dictionary<string, string>();
            bool safeAMAC = clsproperties.AllowMenuContractAdd;
            string safeSPT = clsproperties.SupplierPrimaryTitle;
            bool newAMAC = !clsproperties.AllowMenuContractAdd;
            string newSPT = "** Supplier **";

            properties.Add("allowMenuAddContract", (newAMAC ? "1" : "0"));
            properties.Add("supplierPrimaryTitle", newSPT);

            int modifiedBy = cGlobalVariables.EmployeeID;
            target.SaveProperties(cGlobalVariables.SubAccountID, properties, modifiedBy, null);
            target.InvalidateCache(cGlobalVariables.SubAccountID);

            target = new cAccountSubAccounts(accountId);
            clsproperties = target.getSubAccountById(cGlobalVariables.SubAccountID).SubAccountProperties;

            Assert.AreEqual(clsproperties.AllowMenuContractAdd, newAMAC);
            Assert.AreEqual(clsproperties.SupplierPrimaryTitle, newSPT);
        }

        /// <summary>
        ///A test for getFirstSubAccount
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cAccountSubAccountsTest_getFirstSubAccountTest()
        {
            int accountId = cGlobalVariables.AccountID;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subacc = target.getSubAccountById(cGlobalVariables.SubAccountID);
            
            cAccountSubAccount actual;
            actual = target.getFirstSubAccount();

            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for getPropertiesModified
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cAccountSubAccountsTest_getPropertiesModifiedTest()
        {
            int accountId = cGlobalVariables.AccountID;
            int modifiedBy = cGlobalVariables.EmployeeID;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subacc = target.getSubAccountById(cGlobalVariables.SubAccountID);

            cAccountProperties clsProperties = target.getSubAccountById(cGlobalVariables.SubAccountID).SubAccountProperties.Clone();

            DateTime modifiedSince = DateTime.UtcNow;

            // create a new list of items changed from the existing
            Dictionary<string, string> lstChangedProperties = new Dictionary<string, string>();
            bool newBTE = !clsProperties.BlockTaxExpiry; // opposite to existing
            string newSPT = clsProperties.SupplierPrimaryTitle + "** Supplier **";
            // add them to the list to be saved
            lstChangedProperties.Add("blockTaxExpiry", (newBTE ? "1" : "0"));
            lstChangedProperties.Add("supplierPrimaryTitle", newSPT);

            // save the new properties
            target.SaveProperties(cGlobalVariables.SubAccountID, lstChangedProperties, modifiedBy, null);

            // new balls please
            target = new cAccountSubAccounts(cGlobalVariables.AccountID);

            // get the list of chaged properties
            Dictionary<string, string> actual;
            actual = target.getPropertiesModified(modifiedSince, cGlobalVariables.SubAccountID);

            // check they contain what we were expecting
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count == 2);
            Assert.IsTrue(actual.ContainsKey("blockTaxExpiry"));
            Assert.IsTrue(actual.ContainsKey("supplierPrimaryTitle"));            
        }

        /// <summary>
        ///A test for getSubAccountById
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cAccountSubAccountsTest_getSubAccountByIdTest()
        {

            int accountId = cGlobalVariables.AccountID;
            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            
            cAccountSubAccount actual;
            actual = target.getSubAccountById(cGlobalVariables.SubAccountID);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for getSubAccountsCollection
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cAccountSubAccountsTest_getSubAccountsCollectionTest()
        {
            int accountId = cGlobalVariables.AccountID;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            SortedList<int, cAccountSubAccount> actual;
            actual = target.getSubAccountsCollection();

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
        }

        /// <summary>
        ///A test for InvalidateCache
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cAccountSubAccountsTest_InvalidateCacheTest()
        {
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;

            int accountId = cGlobalVariables.AccountID;
            SortedList<int, cAccountSubAccount> tmpList;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            tmpList = (SortedList<int, cAccountSubAccount>)Cache["accountsubaccounts_" + accountId.ToString()];
            Assert.IsNotNull(tmpList);

            target.InvalidateCache(cGlobalVariables.SubAccountID);
            tmpList = (SortedList<int, cAccountSubAccount>)Cache["accountsubaccounts_" + accountId.ToString()];
            Assert.IsNull(tmpList);
        }

        /// <summary>
        ///A test for DeleteSubAccount
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cAccountSubAccountsTest_DeleteSubAccountTest()
        {
            int accountId = cGlobalVariables.AccountID;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);

            int employeeId = cGlobalVariables.EmployeeID;
            
            int actual;
            actual = target.DeleteSubAccount(cGlobalVariables.SubAccountID, employeeId);

            target = new cAccountSubAccounts(accountId);

            Assert.IsNull(target.getSubAccountById(cGlobalVariables.SubAccountID));            
        }

        /// <summary>
        ///A test for UpdateSubAccount
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cAccountSubAccountsTest_UpdateSubAccountTest()
        {
            int accountId = cGlobalVariables.AccountID;

            cAccountSubAccounts target = new cAccountSubAccounts(accountId);
            cAccountSubAccount subacc = target.getSubAccountById(cGlobalVariables.SubAccountID);

            cAccountSubAccount altered_subacc = new cAccountSubAccount(subacc.SubAccountID, "Test " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), subacc.IsArchived, subacc.SubAccountProperties, subacc.CreatedOn, subacc.CreatedBy, DateTime.UtcNow, cGlobalVariables.EmployeeID);
            target.UpdateSubAccount(altered_subacc, cGlobalVariables.EmployeeID, 0, cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            target = new cAccountSubAccounts(accountId);
            subacc = target.getSubAccountById(cGlobalVariables.SubAccountID);

            Assert.IsNotNull(subacc);
            Assert.IsTrue(subacc.SubAccountID == cGlobalVariables.SubAccountID);
            Assert.AreEqual(subacc.Description, altered_subacc.Description);
        }
    }
}
