using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary;
using System.Reflection;
using Spend_Management;
using SpendManagementLibrary.ESRTransferServiceClasses;
using System.IO;

namespace tempMobileUnitTests
{
    class cGlobalVariables
    {
        public static int NHSTrustID;
        public static int CategoryID;
        public static int SubcatID;
        public static int TemplateID;
        public static int OutboundEmployeID;
        public static int HomeAddressID;
        public static int WorkAddressID;
        public static int OutboundLinemanagerEmployeeID;
        public static byte BroadcastID;
        public static int ItemRoleID;
        public static string CreditCardTestFilesPath = @"<DRIVE_LETTER>:\Projects\Spend Management\<BRANCH_NAME>\SpendManagementUnitTests\Credit Cards\Test Data Files\";
        public static int CorporateCardID;
        public static int DelegateID;
    }

    static class HelperMethods
    {
        public static void SetTestDelegateID()
        {
            Caching cache = new Caching();
            cache.Cache.Add("UnitTestDelegate", GlobalTestVariables.DelegateID, DateTimeOffset.MaxValue);
        }

        public static void ClearTestDelegateID()
        {
            Caching cache = new Caching();
            if (cache.Cache.Contains("UnitTestDelegate")) { cache.Cache.Remove("UnitTestDelegate"); }
        }
    }

    #region Assertion Objects

    public static class ExceptionAssert
    {
        /// <summary>
        /// Checks to make sure that the input delegate throws a exception of type exceptionType.
        /// </summary>
        /// <typeparam name="exceptionType">The type of exception expected.</typeparam>
        /// <param name="blockToExecute">The block of code to execute to generate the exception.</param>
        public static string ThrowsException<exceptionType>(System.Action blockToExecute) where exceptionType : System.Exception
        {
            try
            {
                blockToExecute();
            }
            catch (exceptionType ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected exception of type " + typeof(exceptionType) + " but type of " + ex.GetType() + " was thrown instead.");
            }

            Assert.Fail("Expected exception of type " + typeof(exceptionType) + " but no exception was thrown.");

            return "Passed";
        }
    }

    /// <summary>
    /// This can be used for any object type to iterate through its properties and get the values to compare
    /// </summary>
    public static class cCompareAssert
    {
        public enum CompareType
        {
            AreEqual = 0,
            AreNothingEqual
        }

        /// <summary>
        /// Compares the objects by doing an AreEqual on ALL of the properties
        /// </summary>
        /// <param name="expected">The exemplar object</param>
        /// <param name="actual">The test object</param>
        /// <param name="lstOmittedProperties">Any properties that should not be tested</param>
        public static void AreEqual(object expected, object actual, List<string> lstOmittedProperties = null)
        {
            cCompareAssert.Compare(expected, actual, CompareType.AreEqual, lstOmittedProperties);
        }

        /// <summary>
        /// Compares the objects by doing an AreNotEqual on ALL of the properties
        /// </summary>
        /// <param name="expected">The exemplar object</param>
        /// <param name="actual">The test object</param>
        /// <param name="lstOmittedProperties">Any properties that should not be tested</param>
        public static void AreNothingEqual(object expected, object actual, List<string> lstOmittedProperties = null)
        {
            cCompareAssert.Compare(expected, actual, CompareType.AreNothingEqual, lstOmittedProperties);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="compareType"></param>
        /// <param name="lstOmittedProperties"></param>
        public static void Compare(object expected, object actual, CompareType compareType, List<string> lstOmittedProperties = null)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            Type t = expected.GetType();
            object expectedVal = null;
            object actualVal = null;

            PropertyInfo[] piArr = t.GetProperties();

            foreach (PropertyInfo pi in piArr)
            {
                bool omittedProperty = false;

                if (lstOmittedProperties != null)
                {
                    foreach (string fieldName in lstOmittedProperties)
                    {
                        if (pi.Name.ToLower() == fieldName.ToLower())
                        {
                            omittedProperty = true;
                            break;
                        }
                    }
                }

                //If there is an omitted property then go to the next one
                if (omittedProperty)
                {
                    continue;
                }

                expectedVal = null;
                actualVal = null;

                switch (t.FullName)
                {
                    case "SpendManagementLibrary.cCurrency": //Currencies

                        expectedVal = pi.GetValue((cCurrency)expected, null);
                        actualVal = pi.GetValue((cCurrency)actual, null);
                        break;
                    case "SpendManagementLibrary.cESRTrust": //ESR Trusts

                        expectedVal = pi.GetValue((cESRTrust)expected, null);
                        actualVal = pi.GetValue((cESRTrust)actual, null);
                        break;
                    case "SpendManagementLibrary.cESRElement": //ESR Element Mappings

                        expectedVal = pi.GetValue((cESRElement)expected, null);
                        actualVal = pi.GetValue((cESRElement)actual, null);
                        break;

                    case "SpendManagementLibrary.cESRAssignment": //ESR Assignment Records

                        expectedVal = pi.GetValue((cESRAssignment)expected, null);
                        actualVal = pi.GetValue((cESRAssignment)actual, null);
                        break;
                    case "SpendManagementLibrary.cImportTemplate": //Import Template

                        expectedVal = pi.GetValue((cImportTemplate)expected, null);
                        actualVal = pi.GetValue((cImportTemplate)actual, null);
                        break;
                    case "SpendManagementLibrary.cFinancialExport": //Financial Export

                        expectedVal = pi.GetValue((cFinancialExport)expected, null);
                        actualVal = pi.GetValue((cFinancialExport)actual, null);
                        break;
                    case "SpendManagementLibrary.cImportHistoryItem": //Import History

                        expectedVal = pi.GetValue((cImportHistoryItem)expected, null);
                        actualVal = pi.GetValue((cImportHistoryItem)actual, null);
                        break;
                    case "Spend_Management.cTask": //Task

                        expectedVal = pi.GetValue((cTask)expected, null);
                        actualVal = pi.GetValue((cTask)actual, null);
                        break;
                    case "SpendManagementLibrary.cEmployee": //Employee

                        expectedVal = pi.GetValue((cEmployee)expected, null);
                        actualVal = pi.GetValue((cEmployee)actual, null);
                        break;

                    case "SpendManagementLibrary.cContractCategory": //Contract Category

                        expectedVal = pi.GetValue((cContractCategory)expected, null);
                        actualVal = pi.GetValue((cContractCategory)actual, null);
                        break;
                    case "SpendManagementLibrary.cContractStatus": //Contract Status

                        expectedVal = pi.GetValue((cContractStatus)expected, null);
                        actualVal = pi.GetValue((cContractStatus)actual, null);
                        break;
                    case "SpendManagementLibrary.cContractType": //Contract Type

                        expectedVal = pi.GetValue((cContractType)expected, null);
                        actualVal = pi.GetValue((cContractType)actual, null);
                        break;
                    case "SpendManagementLibrary.cInvoiceFrequencyType": //Invoice Frequency Type

                        expectedVal = pi.GetValue((cInvoiceFrequencyType)expected, null);
                        actualVal = pi.GetValue((cInvoiceFrequencyType)actual, null);
                        break;
                    case "SpendManagementLibrary.cInvoiceStatus": //Invoice Status

                        expectedVal = pi.GetValue((cInvoiceStatus)expected, null);
                        actualVal = pi.GetValue((cInvoiceStatus)actual, null);
                        break;
                    case "SpendManagementLibrary.cLicenceRenewalType": //Licence Renewal Type

                        expectedVal = pi.GetValue((cLicenceRenewalType)expected, null);
                        actualVal = pi.GetValue((cLicenceRenewalType)actual, null);
                        break;
                    case "SpendManagementLibrary.cInflatorMetric": //Inflator Metric

                        expectedVal = pi.GetValue((cInflatorMetric)expected, null);
                        actualVal = pi.GetValue((cInflatorMetric)actual, null);
                        break;
                    case "SpendManagementLibrary.cTermType": //Term Type

                        expectedVal = pi.GetValue((cTermType)expected, null);
                        actualVal = pi.GetValue((cTermType)actual, null);
                        break;
                    case "SpendManagementLibrary.cFinancialStatus": //Financial status

                        expectedVal = pi.GetValue((cFinancialStatus)expected, null);
                        actualVal = pi.GetValue((cFinancialStatus)actual, null);
                        break;
                    case "SpendManagementLibrary.cTaskType": //Task type

                        expectedVal = pi.GetValue((cTaskType)expected, null);
                        actualVal = pi.GetValue((cTaskType)actual, null);
                        break;
                    case "SpendManagementLibrary.cUnit": //Unit

                        expectedVal = pi.GetValue((cUnit)expected, null);
                        actualVal = pi.GetValue((cUnit)actual, null);
                        break;
                    case "SpendManagementLibrary.cProductCategory": //Product category

                        expectedVal = pi.GetValue((cProductCategory)expected, null);
                        actualVal = pi.GetValue((cProductCategory)actual, null);
                        break;
                    case "SpendManagementLibrary.cSupplierStatus": //Supplier Status

                        expectedVal = pi.GetValue((cSupplierStatus)expected, null);
                        actualVal = pi.GetValue((cSupplierStatus)actual, null);
                        break;
                    case "SpendManagementLibrary.cSupplierCategory": //Supplier Category

                        expectedVal = pi.GetValue((cSupplierCategory)expected, null);
                        actualVal = pi.GetValue((cSupplierCategory)actual, null);
                        break;
                    case "SpendManagementLibrary.cProductLicenceType": //Product Licence Type

                        expectedVal = pi.GetValue((cProductLicenceType)expected, null);
                        actualVal = pi.GetValue((cProductLicenceType)actual, null);
                        break;
                    case "SpendManagementLibrary.cSalesTax": //Sales Tax

                        expectedVal = pi.GetValue((cSalesTax)expected, null);
                        actualVal = pi.GetValue((cSalesTax)actual, null);
                        break;

                    case "SpendManagementLibrary.cUserdefinedFieldGrouping": // User Defined Field Groupings
                        expectedVal = pi.GetValue((cUserdefinedFieldGrouping)expected, null);
                        actualVal = pi.GetValue((cUserdefinedFieldGrouping)actual, null);
                        break;

                    case "SpendManagementLibrary.cGlobalMimeType": //Global Mime Type

                        expectedVal = pi.GetValue((cGlobalMimeType)expected, null);
                        actualVal = pi.GetValue((cGlobalMimeType)actual, null);
                        break;

                    case "SpendManagementLibrary.cTeam": // Team

                        expectedVal = pi.GetValue((cTeam)expected, null);
                        actualVal = pi.GetValue((cTeam)actual, null);
                        break;

                    case "SpendManagementLibrary.cCardTemplate": // Credit Card Template

                        expectedVal = pi.GetValue((cCardTemplate)expected, null);
                        actualVal = pi.GetValue((cCardTemplate)actual, null);
                        break;

                    case "SpendManagementLibrary.cCardStatement": // Card Statement

                        expectedVal = pi.GetValue((cCardStatement)expected, null);
                        actualVal = pi.GetValue((cCardStatement)actual, null);
                        break;

                    case "SpendManagementLibrary.cModule": // Module

                        expectedVal = pi.GetValue((cModule)expected, null);
                        actualVal = pi.GetValue((cModule)actual, null);
                        break;

                    case "SpendManagementLibrary.cAccountProperties": // Account Properties

                        expectedVal = pi.GetValue((cAccountProperties)expected, null);
                        actualVal = pi.GetValue((cAccountProperties)actual, null);
                        break;

                    case "SpendManagementLibrary.cNumberAttribute": // Number Attributes

                        expectedVal = pi.GetValue((cNumberAttribute)expected, null);
                        actualVal = pi.GetValue((cNumberAttribute)actual, null);
                        break;

                    case "SpendManagementLibrary.cTextAttribute": // Text Attributes

                        expectedVal = pi.GetValue((cTextAttribute)expected, null);
                        actualVal = pi.GetValue((cTextAttribute)actual, null);
                        break;

                    case "SpendManagementLibrary.cDateTimeAttribute": // DateTime Attributes

                        expectedVal = pi.GetValue((cDateTimeAttribute)expected, null);
                        actualVal = pi.GetValue((cDateTimeAttribute)actual, null);
                        break;

                    case "SpendManagementLibrary.cTickboxAttribute": // Tickbox Attributes

                        expectedVal = pi.GetValue((cTickboxAttribute)expected, null);
                        actualVal = pi.GetValue((cTickboxAttribute)actual, null);
                        break;

                    case "SpendManagementLibrary.cRunWorkflowAttribute": // RunWorkflow Attributes

                        expectedVal = pi.GetValue((cRunWorkflowAttribute)expected, null);
                        actualVal = pi.GetValue((cRunWorkflowAttribute)actual, null);
                        break;

                    //case "SpendManagementLibrary.cCommentAttribute": // Comment Attributes

                    //    expectedVal = pi.GetValue((cCommentAttribute)expected, null);
                    //    actualVal = pi.GetValue((cCommentAttribute)actual, null);
                    //    break;

                    case "SpendManagementLibrary.cOneToManyRelationship": // OneToManyRelationship Attributes

                        expectedVal = pi.GetValue((cOneToManyRelationship)expected, null);
                        actualVal = pi.GetValue((cOneToManyRelationship)actual, null);
                        break;

                    case "SpendManagementLibrary.cManyToOneRelationship": // ManyToOneRelationship Attributes

                        expectedVal = pi.GetValue((cManyToOneRelationship)expected, null);
                        actualVal = pi.GetValue((cManyToOneRelationship)actual, null);
                        break;

                    case "SpendManagementLibrary.cListAttribute": // List Attributes

                        expectedVal = pi.GetValue((cListAttribute)expected, null);
                        actualVal = pi.GetValue((cListAttribute)actual, null);
                        break;

                    case "SpendManagementLibrary.cCustomEntity": // Custom Entities

                        expectedVal = pi.GetValue((cCustomEntity)expected, null);
                        actualVal = pi.GetValue((cCustomEntity)actual, null);
                        break;

                    case "SpendManagementLibrary.cCustomEntityForm": // Custom Entity Forms

                        expectedVal = pi.GetValue((cCustomEntityForm)expected, null);
                        actualVal = pi.GetValue((cCustomEntityForm)actual, null);
                        break;

                    case "SpendManagementLibrary.cCustomEntityFormTab": // Custom Entity Form Tabs

                        expectedVal = pi.GetValue((cCustomEntityFormTab)expected, null);
                        actualVal = pi.GetValue((cCustomEntityFormTab)actual, null);
                        break;

                    case "SpendManagementLibrary.cCustomEntityView": // Custom Entity View

                        expectedVal = pi.GetValue((cCustomEntityView)expected, null);
                        actualVal = pi.GetValue((cCustomEntityView)actual, null);
                        break;

                    case "Spend_Management.cAudienceRecordStatus": // Audience Record Status

                        expectedVal = pi.GetValue((cAudienceRecordStatus)expected, null);
                        actualVal = pi.GetValue((cAudienceRecordStatus)actual, null);
                        break;

                    case "SpendManagementLibrary.cMobileDevice":
                        expectedVal = pi.GetValue((cMobileDevice)expected, null);
                        actualVal = pi.GetValue((cMobileDevice)actual, null);
                        break;

                    case "Spend_Management.ExpenseItem":
                        expectedVal = pi.GetValue((ExpenseItem)expected, null);
                        actualVal = pi.GetValue((ExpenseItem)actual, null);
                        break;

                    case "SpendManagementLibrary.cExpenseItem":
                        expectedVal = pi.GetValue((cExpenseItem)expected, null);
                        actualVal = pi.GetValue((cExpenseItem)actual, null);
                        break;

                    default:

                        throw new AssertFailedException("Object definition " + t.FullName + " is not configured for comparison in cCompareAssert");
                }

                if (expectedVal != null && actualVal != null)
                {
                    if (compareType == CompareType.AreEqual && expectedVal.ToString() != actualVal.ToString())
                    {
                        throw new AssertFailedException(
                        string.Format(
                            t.Name + "." + pi.Name + " values are different. {0} Expected: {1}, Actual: {2}",
                            Environment.NewLine,
                            expectedVal,
                            actualVal));
                    }
                    else if (compareType == CompareType.AreNothingEqual && expectedVal.ToString() == actualVal.ToString())
                    {
                        throw new AssertFailedException(
                        string.Format(
                            t.Name + "." + pi.Name + " values are the same. {0} Expected: {1}, Actual: {2}",
                            Environment.NewLine,
                            expectedVal,
                            actualVal));
                    }
                }
            }
        }
    }

    public static class cCurrencyAssert
    {
        public static void AreEqual(cCurrency expected, cCurrency actual)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            if (expected.accountid != actual.accountid)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cCurrency.accountid values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.accountid,
                        actual.accountid));
            }
            if (expected.archived != actual.archived)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cCurrency.archived values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.archived,
                        actual.archived));
            }
            if (expected.createdby != actual.createdby)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cCurrency.createdby values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.createdby,
                        actual.createdby));
            }
            if (expected.createdon != actual.createdon)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cCurrency.createdon values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.createdon,
                        actual.createdon));
            }
            if (expected.currencyid != actual.currencyid)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cCurrency.currencyid values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.currencyid,
                        actual.currencyid));
            }
            if (expected.globalcurrencyid != actual.globalcurrencyid)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cCurrency.globalcurrencyid values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.globalcurrencyid,
                        actual.globalcurrencyid));
            }
            if (expected.modifiedby != actual.modifiedby)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cCurrency.modifiedby values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.modifiedby,
                        actual.modifiedby));
            }
            if (expected.modifiedon != actual.modifiedon)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cCurrency.modifiedon values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.modifiedon,
                        actual.modifiedon));
            }
            if (expected.negativeFormat != actual.negativeFormat)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cCurrency.negativeFormat values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.negativeFormat,
                        actual.negativeFormat));
            }
            if (expected.positiveFormat != actual.positiveFormat)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cCurrency.positiveFormat values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.positiveFormat,
                        actual.positiveFormat));
            }
        }
    }

    public static class cGlobalCountryAssert
    {
        public static void AreEqual(cGlobalCountry expected, cGlobalCountry actual)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            if (expected.country != actual.country)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cGlobalCountry.country values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.country,
                        actual.country));
            }
            if (expected.countrycode != actual.countrycode)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cGlobalCountry.countrycode values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.countrycode,
                        actual.countrycode));
            }
            if (expected.createdon != actual.createdon)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cGlobalCountry.createdon values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.createdon,
                        actual.createdon));
            }
            if (expected.globalcountryid != actual.globalcountryid)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cGlobalCountry.globalcountryid values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.globalcountryid,
                        actual.globalcountryid));
            }
            if (expected.modifiedon != actual.modifiedon)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cGlobalCountry.modifiedon values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.modifiedon,
                        actual.modifiedon));
            }
        }
    }

    public static class cGlobalCurrencyAssert
    {
        public static void AreEqual(cGlobalCurrency expected, cGlobalCurrency actual)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            if (expected.alphacode != actual.alphacode)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cGlobalCurrency.alphacode values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.alphacode,
                        actual.alphacode));
            }
            if (expected.createdon != actual.createdon)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cGlobalCurrency.createdon values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.createdon,
                        actual.createdon));
            }
            if (expected.globalcurrencyid != actual.globalcurrencyid)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cGlobalCurrency.globalcurrencyid values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.globalcurrencyid,
                        actual.globalcurrencyid));
            }
            if (expected.label != actual.label)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cGlobalCurrency.label values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.label,
                        actual.label));
            }
            if (expected.modifiedon != actual.modifiedon)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cGlobalCurrency.modifiedon values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.modifiedon,
                        actual.modifiedon));
            }
            if (expected.numericcode != actual.numericcode)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cGlobalCurrency.numericcode values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.numericcode,
                        actual.numericcode));
            }
            if (expected.symbol != actual.symbol)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cGlobalCurrency.symbol values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.symbol,
                        actual.symbol));
            }
        }
    }

    #endregion

    #region Employee Object

    public class cEmployeeObject
    {
        public static int CreateUTEmployee()
        {
            int employeeID;
            cEmployees clsEmployees = new cEmployees(GlobalTestVariables.AccountID);
            cEmployee reqEmployee = GetUTEmployeeTemplateObject();

            employeeID = clsEmployees.saveEmployee(reqEmployee, new cDepCostItem[] { }, new List<int>(), null);

            return employeeID;
        }

        /// <summary>
        /// Create the employee deleagte object to be used for the unit tests where testing for a delegate is required
        /// </summary>
        /// <returns></returns>
        public static int CreateUTDelegateEmployee()
        {
            int employeeID;
            cEmployees clsEmployees = new cEmployees(GlobalTestVariables.AccountID);
            cEmployee reqEmployee = GetUTEmployeeTemplateObject();

            employeeID = clsEmployees.saveEmployee(reqEmployee, new cDepCostItem[] { }, new List<int>(), null);
            cGlobalVariables.DelegateID = employeeID;

            return employeeID;
        }

        /// <summary>
        /// Creates an inactive ESR Assignment number for an employee with start and final active dates provided
        /// </summary>
        /// <param name="employeeid"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public static cEmployee CreateUTEmployeeInactiveESRAssignmentNumber(int employeeid, DateTime startdate, DateTime? enddate)
        {
            cEmployees clsEmployees = new cEmployees(GlobalTestVariables.AccountID);
            cEmployee emp = clsEmployees.GetEmployeeById(employeeid);
            Dictionary<int, cESRAssignment> esrAssignments = new Dictionary<int, cESRAssignment>();
            cESRAssignment ass = new cESRAssignment(123, 0, "1234567", startdate, enddate, ESRAssignmentStatus.ActiveAssignment, "", "", "", "", "", "", "", "", "", false, "", "", "", "", "", "", true, 35, "", 0, 0, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", null, false, null, null, null, null);
            esrAssignments.Add(123, ass);

            cDepCostItem[] lstCostCodes = clsEmployees.GetCostCodeBreakdown(emp.employeeid);

            cEmployee newemp = new cEmployee(emp.accountid, emp.employeeid, emp.username, emp.password, emp.PasswordMethod, emp.title, emp.firstname, emp.surname, emp.mileagetotal, emp.mileagetotaldate, emp.email, emp.currefnum, emp.curclaimno, emp.address1, emp.address2, emp.city, emp.county, emp.postcode, emp.payroll, emp.position, emp.telno, emp.creditor, emp.archived, emp.groupid, emp.lastchange, emp.fax, emp.homeemail, emp.extension, emp.pagerno, emp.mobileno, emp.linemanager, emp.advancegroup, emp.primarycountry, emp.primarycurrency, emp.verified, emp.active, emp.licenceexpiry, emp.licencelastchecked, emp.licencecheckedby, emp.licencenumber, emp.groupidcc, emp.groupidpc, emp.ninumber, emp.middlenames, emp.maidenname, emp.gender, emp.dateofbirth, emp.hiredate, emp.leavedate, emp.country, emp.customiseditems, emp.createdon, emp.createdby, emp.modifiedon, emp.modifiedby, emp.Name, emp.AccountNumber, emp.AccountType, emp.Sortcode, emp.Reference, emp.LocaleID, emp.NHSTrustID, emp.currentLogonCount, emp.logonRetryCount, emp.FirstLogon, emp.LicenceAttachID, GlobalTestVariables.SubAccountID, emp.empCreationMethod); // , emp.adminonly

            int newEmpID = 0;
            if (emp != null)
            {
                DBConnection db = new DBConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountID));
                newEmpID = clsEmployees.saveEmployee(newemp, lstCostCodes, clsEmployees.GetEmailNotifications(employeeid), clsEmployees.GetUserDefinedFields(employeeid));
            }

            if (newEmpID > 0 && newEmpID == emp.employeeid)
            {
                cESRAssignments clsAssignments = new cESRAssignments(GlobalTestVariables.AccountID, emp.employeeid);
                clsAssignments.saveESRAssignment(ass);
            }

            return emp;
        }

        public static cEmployee CreateUTEmployeeObject(cEmployee reqEmployee)
        {
            int employeeID;
            cEmployees clsEmployees = new cEmployees(GlobalTestVariables.AccountID);
            cDepCostItem[] lstCostCodes = clsEmployees.GetCostCodeBreakdown(reqEmployee.employeeid);
            List<int> lstEmailNotifications = clsEmployees.GetEmailNotifications(reqEmployee.employeeid);
            SortedList<int, object> lstUserDefinedFields = clsEmployees.GetUserDefinedFields(reqEmployee.employeeid);

            employeeID = clsEmployees.saveEmployee(reqEmployee, lstCostCodes, lstEmailNotifications, lstUserDefinedFields);

            cEmployee savedEmployee = null;
            if (employeeID > 0)
            {
                savedEmployee = clsEmployees.GetEmployeeById(employeeID);
            }

            return savedEmployee;
        }

        /// <summary>
        /// Create an employee global static object that has item roles associated
        /// </summary>
        /// <returns></returns>
        public static cEmployee CreateUTEmployeeWithItemRolesObject()
        {
            //Create and associate an item role to the employee
            List<int> lstItemRoles = new List<int>();
            cItemRole role = cItemRoleObject_OLD.CreateItemRole();
            lstItemRoles.Add(role.itemroleid);

            cEmployee reqEmployee = new cEmployee(GlobalTestVariables.AccountID, 0, "UTUserName " + DateTime.UtcNow.ToString() + ":" + DateTime.UtcNow.Ticks, "A", PwdMethod.RijndaelManaged, "Auto", "Unit", "Tester", 0, null, "ut" + DateTime.Now.Ticks.ToString() + "@software-europe.co.uk", 0, 0, "1 UT Street", "", "Lincoln", "Lincs", "LN5 8SB", "12345678", "UTPosition", "01522881300", "UTCreditor", false, 0, DateTime.UtcNow, "01522881355", "uthome@software-europe.co.uk", "280", "01522000000", "07795000000", 0, 0, 0, 0, false, true, null, null, 0, "", 0, 0, "", "UTMIddle", "UTMaiden", "Male", DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "UK", false, DateTime.UtcNow, GlobalTestVariables.EmployeeID, null, null, "UTName", "UTAccountNumber", "UTAccountType", "UTSortCode", "UTReference", null, null, 0, 0, false, 0, GlobalTestVariables.SubAccountID, EmployeeCreationMethod.Manually); // , false

            reqEmployee._ItemRoles = lstItemRoles;

            return reqEmployee;
        }

        public static void UpdateEmployeePasswordDetails(PwdMethod pwdMethod, string planTextPassword, int employeeID)
        {
            string convertedPassword = string.Empty;

            switch (pwdMethod)
            {
                case PwdMethod.FWBasic:
                    convertedPassword = cPassword.Crypt(planTextPassword, "2");
                    break;
                case PwdMethod.Hash:
                case PwdMethod.MD5:
                    convertedPassword = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(planTextPassword, System.Web.Configuration.FormsAuthPasswordFormat.MD5.ToString());
                    break;
                case PwdMethod.RijndaelManaged:
                    cSecureData clsSecureData = new cSecureData();
                    convertedPassword = clsSecureData.Encrypt(planTextPassword);
                    break;
                case PwdMethod.SHA_Hash:
                    convertedPassword = cPassword.SHA_HashPassword(planTextPassword);
                    break;
                default:
                    throw new Exception("Unknown PwdMethod type");
            }


            DBConnection db = new DBConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountID));
            string strSQL = "UPDATE employees SET passwordMethod=@pwdMethod, password=@pwd WHERE employeeID=@employeeID";
            db.sqlexecute.Parameters.AddWithValue("@pwdMethod", pwdMethod);
            db.sqlexecute.Parameters.AddWithValue("@pwd", convertedPassword);
            db.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
            db.ExecuteSQL(strSQL);
        }

        public static cEmployee GetUTEmployeeTemplateObject(DateTime? licenceExpiryDate = null)
        {
            cEmployee employee = new cEmployee(GlobalTestVariables.AccountID, 0, "UTUserName " + DateTime.UtcNow.ToString() + ":" + DateTime.UtcNow.Ticks, "A", PwdMethod.RijndaelManaged, "Auto", "Unit", "Tester", 0, null, "ut" + DateTime.Now.ToShortDateString() + "@software-europe.co.uk", 0, 0, "1 UT Street", "", "Lincoln", "Lincs", "LN5 8SB", "12345678", "UTPosition", "01522881300", "UTCreditor", false, 0, DateTime.UtcNow, "01522881355", "uthome@software-europe.co.uk", "280", "01522000000", "07795000000", 0, 0, 0, 0, false, true, null, null, 0, "", 0, 0, "", "UTMIddle", "UTMaiden", "Male", DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "UK", false, DateTime.UtcNow, GlobalTestVariables.EmployeeID, null, null, "UTName", "UTAccountNumber", "UTAccountType", "UTSortCode", "UTReference", null, null, 0, 0, false, 0, GlobalTestVariables.SubAccountID, EmployeeCreationMethod.Manually); // , false
            return employee;
        }

        public static cEmployee GetUTEmployeeTemplateWhereArchivedObject()
        {
            cEmployee employee = new cEmployee(GlobalTestVariables.AccountID, 0, "UTUserName " + DateTime.UtcNow.ToString() + ":" + DateTime.UtcNow.Ticks, "A", PwdMethod.RijndaelManaged, "Auto", "Unit", "Tester", 0, null, "ut" + DateTime.Now.ToShortDateString() + " @software-europe.co.uk", 0, 0, "1 UT Street", "", "Lincoln", "Lincs", "LN5 8SB", "12345678", "UTPosition", "01522881300", "UTCreditor", false, 0, DateTime.UtcNow, "01522881355", "uthome@software-europe.co.uk", "280", "01522000000", "07795000000", 0, 0, 0, 0, false, true, null, null, 0, "", 0, 0, "", "UTMIddle", "UTMaiden", "Male", DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "UK", false, DateTime.UtcNow, GlobalTestVariables.EmployeeID, null, null, "UTName", "UTAccountNumber", "UTAccountType", "UTSortCode", "UTReference", null, null, 0, 0, false, 0, GlobalTestVariables.SubAccountID, EmployeeCreationMethod.Manually); // , false
            return employee;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static cEmployee GetUTEmployeeWhoIsActiveAndHasNoPasswordSetObject()
        {
            cEmployee employee = new cEmployee(GlobalTestVariables.AccountID, 0, "UTUserName " + DateTime.UtcNow.ToString() + ":" + DateTime.UtcNow.Ticks, "", PwdMethod.RijndaelManaged, "Auto", "Unit", "Tester", 0, null, "ut" + DateTime.Now.Ticks.ToString() + "@software-europe.co.uk", 0, 0, "1 UT Street", "", "Lincoln", "Lincs", "LN5 8SB", "12345678", "UTPosition", "01522881300", "UTCreditor", false, 0, DateTime.UtcNow, "01522881355", "uthome@software-europe.co.uk", "280", "01522000000", "07795000000", 0, 0, 0, 0, true, true, null, null, 0, "", 0, 0, "", "UTMIddle", "UTMaiden", "Male", DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "UK", false, DateTime.UtcNow, GlobalTestVariables.EmployeeID, null, null, "UTName", "UTAccountNumber", "UTAccountType", "UTSortCode", "UTReference", null, null, 0, 0, false, 0, GlobalTestVariables.SubAccountID, EmployeeCreationMethod.Manually); // , false
            return employee;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static cEmployee GetUTEmployeeWhoIsInactiveAndHasPasswordSetObject()
        {
            cEmployee employee = new cEmployee(GlobalTestVariables.AccountID, 0, "UTUserName " + DateTime.UtcNow.ToString() + ":" + DateTime.UtcNow.Ticks, "A", PwdMethod.RijndaelManaged, "Auto", "Unit", "Tester", 0, null, "ut" + DateTime.Now.Ticks.ToString() + "@software-europe.co.uk", 0, 0, "1 UT Street", "", "Lincoln", "Lincs", "LN5 8SB", "12345678", "UTPosition", "01522881300", "UTCreditor", false, 0, DateTime.UtcNow, "01522881355", "uthome@software-europe.co.uk", "280", "01522000000", "07795000000", 0, 0, 0, 0, false, true, null, null, 0, "", 0, 0, "", "UTMIddle", "UTMaiden", "Male", DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "UK", false, DateTime.UtcNow, GlobalTestVariables.EmployeeID, null, null, "UTName", "UTAccountNumber", "UTAccountType", "UTSortCode", "UTReference", null, null, 0, 0, false, 0, GlobalTestVariables.SubAccountID, EmployeeCreationMethod.Manually); // , false
            return employee;
        }

        /// <summary>
        /// This is used where comparisons need to be done for employees with the same username
        /// </summary>
        /// <returns></returns>
        public static cEmployee GetUTEmployeeTemplateObjectWithStaticUsername()
        {
            cEmployee employee = new cEmployee(GlobalTestVariables.AccountID, 0, "UTUserName", "A", PwdMethod.RijndaelManaged, "Auto", "Unit", "Tester", 0, null, "ut" + DateTime.Now.Ticks.ToString() + "@software-europe.co.uk", 0, 0, "1 UT Street", "", "Lincoln", "Lincs", "LN5 8SB", "12345678", "UTPosition", "01522881300", "UTCreditor", false, 0, DateTime.UtcNow, "01522881355", "uthome@software-europe.co.uk", "280", "01522000000", "07795000000", 0, 0, 0, 0, false, true, null, null, 0, "", 0, 0, "", "UTMIddle", "UTMaiden", "Male", DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "UK", false, DateTime.UtcNow, GlobalTestVariables.EmployeeID, null, null, "UTName", "UTAccountNumber", "UTAccountType", "UTSortCode", "UTReference", null, null, 0, 0, false, 0, GlobalTestVariables.SubAccountID, EmployeeCreationMethod.Manually); // , false
            return employee;
        }

        /// <summary>
        /// Employee whose password last chnaged date is 2 days old
        /// </summary>
        /// <returns></returns>
        public static cEmployee GetUTEmployeeTemplateWithPasswordLastChangedExpiredObject()
        {
            cEmployee employee = new cEmployee(GlobalTestVariables.AccountID, 0, "UTUserName " + DateTime.UtcNow.ToString() + ":" + DateTime.UtcNow.Ticks, "A", PwdMethod.RijndaelManaged, "Auto", "Unit", "Tester", 0, null, "ut" + DateTime.Now.Ticks.ToString() + "@software-europe.co.uk", 0, 0, "1 UT Street", "", "Lincoln", "Lincs", "LN5 8SB", "12345678", "UTPosition", "01522881300", "UTCreditor", false, 0, DateTime.UtcNow.AddDays(-2), "01522881355", "uthome@software-europe.co.uk", "280", "01522000000", "07795000000", 0, 0, 0, 0, false, true, null, null, 0, "", 0, 0, "", "UTMIddle", "UTMaiden", "Male", DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "UK", false, DateTime.UtcNow, GlobalTestVariables.EmployeeID, null, null, "UTName", "UTAccountNumber", "UTAccountType", "UTSortCode", "UTReference", null, null, 0, 0, false, 0, GlobalTestVariables.SubAccountID, EmployeeCreationMethod.Manually); // , false
            return employee;
        }

        public static void UpdateEmployeeBaseCurrency(int CurrencyID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountID));
            string strsql = "UPDATE employees SET primarycurrency = @currencyid WHERE employeeid = @employeeID ";

            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", GlobalTestVariables.EmployeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", CurrencyID);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Delete the delegate unit test employee from the database
        /// </summary>
        public static void DeleteDelegateUTEmployee()
        {
            cEmployees clsEmps = new cEmployees(GlobalTestVariables.AccountID);
            clsEmps.deleteEmployee(cGlobalVariables.DelegateID);
        }

        /// <summary>
        /// Delete the unit test employee from the database
        /// </summary>
        public static void DeleteUTEmployee(int ID, string username)
        {
            cEmployees clsEmps = new cEmployees(GlobalTestVariables.AccountID);
            clsEmps.archiveEmployee(username, GlobalTestVariables.AccountID);
            clsEmps.deleteEmployee(ID);
        }

        /// <summary>
        /// Create an unread broadcast message for the GlobalTestVariables.EmployeeID
        /// </summary>
        public static void CreateUnreadBroadcastMessage()
        {
            cBroadcastMessages msgs = new cBroadcastMessages(GlobalTestVariables.AccountID);

            //cGlobalVariables.BroadcastID = msgs.addBroadcastMessage("UT Test Broadcast Message", "This is a test message for use by unit tests.", DateTime.Now,DateTime.Now.AddDays(10), DateTime.Now.AddHours(1), true, broadcastLocation.HomePage,DateTime.Now, GlobalTestVariables.EmployeeID);

            return;
        }

        /// <summary>
        /// Delete the broadcast message for the GlobalTestVariables.EmployeeID
        /// </summary>
        public static void DeleteBroadcastMessage()
        {
            cBroadcastMessages msgs = new cBroadcastMessages(GlobalTestVariables.AccountID);

            msgs.deleteBroadcastMessage(cGlobalVariables.BroadcastID);
            return;
        }

        /// <summary>
        /// properties that will be omitted when the compare assert method is used to compare objects in unit tests
        /// </summary>
        public static readonly List<string> lstOmittedProperties;

        /// <summary>
        /// The constructor creates a new instance of the omitted properties and adds them
        /// </summary>
        static cEmployeeObject()
        {
            lstOmittedProperties = new List<string>();

            lstOmittedProperties.Add("employeeid");
        }
    }

    #endregion

    #region Item Role Object

    public class cItemRoleObject_OLD
    {
        /// <summary>
        /// Create the item role global static object
        /// </summary>
        /// <returns></returns>
        public static cItemRole CreateItemRole()
        {
            cItemRoles clsItemRoles = new cItemRoles(GlobalTestVariables.AccountID);

            int ItemRoleID = clsItemRoles.addRole("Unit test item role" + DateTime.Now.Ticks.ToString(), "Unit test item role description", new List<cRoleSubcat>(), GlobalTestVariables.EmployeeID);

            cItemRole role = clsItemRoles.getItemRoleById(ItemRoleID);
            cGlobalVariables.ItemRoleID = ItemRoleID;

            CreateRoleSubcat(ref role);

            return role;
        }

        /// <summary>
        /// Create a role subcat that will be added to the item role
        /// </summary>
        /// <param name="itemRole"></param>
        public static void CreateRoleSubcat(ref cItemRole itemRole)
        {
            cSubcat subcat = cSubcatObject_OLD.CreateDummySubcat();
            cItemRoles clsItemRoles = new cItemRoles(GlobalTestVariables.AccountID);
            clsItemRoles.saveRoleSubcat(new cRoleSubcat(0, itemRole.itemroleid, subcat, 0, 0, true));

            itemRole = clsItemRoles.getItemRoleById(itemRole.itemroleid);
        }

        /// <summary>
        /// Delete the item role from the datbase and the associated subcat
        /// </summary>
        public static void DeleteItemRole()
        {
            cItemRoles clsItemRoles = new cItemRoles(GlobalTestVariables.AccountID);
            clsItemRoles.deleteRole(cGlobalVariables.ItemRoleID);
            cSubcatObject_OLD.DeleteSubcat();
        }
    }

    #endregion

    #region Subcat Object

    public class cSubcatObject_OLD
    {
        /// <summary>
        /// Create a global mileage subcat
        /// </summary>
        /// <returns></returns>
        public static cSubcat CreateDummySubcat()
        {
            //Need the category associated
            cCategory category = CreateExpenseCategory();

            cSubcats clsSubcats = new cSubcats(GlobalTestVariables.AccountID);

            int tempSubcatID = clsSubcats.saveSubcat(new cSubcat(0, category.categoryid, "Unit Test dummy Item", "Unit Test dummy Item", false, false, false, false, false, false, 0, "UnitTest01", false, false, 0, false, false, CalculationType.NormalItem, false, false, "Used for Unit Tests", false, 0, true, false, false, false, false, false, false, false, false, false, false, "", false, false, 0, 0, false, false, new SortedList<int, object>(), DateTime.UtcNow, GlobalTestVariables.EmployeeID, null, null, "Unit Test Normal", false, false, new List<cCountrySubcat>(), new List<int>(), new List<int>(), new List<int>(), false, new List<cSubcatVatRate>(), false, HomeToLocationType.None, null, false, null, false));
            cGlobalVariables.SubcatID = tempSubcatID;
            clsSubcats = new cSubcats(GlobalTestVariables.AccountID);
            cSubcat subcat = clsSubcats.getSubcatById(tempSubcatID);
            return subcat;
        }


        /// <summary>
        /// Create the global object for the expense category
        /// </summary>
        /// <returns></returns>
        public static cCategory CreateExpenseCategory()
        {
            cCategories clsCats = new cCategories(GlobalTestVariables.AccountID);
            int tempCategoryID = clsCats.addCategory("Unit Test Category" + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), "Category for Unit Tests", GlobalTestVariables.EmployeeID);
            cGlobalVariables.CategoryID = tempCategoryID;
            clsCats = new cCategories(GlobalTestVariables.AccountID);
            cCategory category = clsCats.FindById(tempCategoryID);
            return category;
        }

        /// <summary>
        /// Delete the static subcat object from the database
        /// </summary>
        public static void DeleteSubcat()
        {
            cSubcats clsSubcats = new cSubcats(GlobalTestVariables.AccountID);
            clsSubcats.deleteSubcat(cGlobalVariables.SubcatID);
            DeleteExpenseCategory();
        }

        /// <summary>
        /// Delete the static expense category object from the database
        /// </summary>
        public static void DeleteExpenseCategory()
        {
            cCategories clsCats = new cCategories(GlobalTestVariables.AccountID);
            clsCats.deleteCategory(cGlobalVariables.CategoryID);
        }



        /// <summary>
        /// Create mileage item category
        /// </summary>
        /// <returns>Subcat for mileage items</returns>
        public static cSubcat CreateMileageSubcat()
        {
            cSubcats clsSubcats = new cSubcats(GlobalTestVariables.AccountID);
            cCategory oCategory = CreateExpenseCategory();
            int nSubcatID;

            try
            {
                nSubcatID = clsSubcats.saveSubcat(new cSubcat(0, oCategory.categoryid, "Unit Test Mileage Subcat: " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), "Unit Test dummy Item", true, false, false, false, false, false, 0, "UnitTest01", false, false, 0, false, false, CalculationType.NormalItem, false, false, "Used for Unit Tests", false, 0, true, false, false, false, false, false, false, false, false, false, false, "", false, false, 0, 0, false, false, new SortedList<int, object>(), DateTime.UtcNow, GlobalTestVariables.EmployeeID, null, null, "Unit Test Normal", false, false, new List<cCountrySubcat>(), new List<int>(), new List<int>(), new List<int>(), false, new List<cSubcatVatRate>(), false, HomeToLocationType.None, null, false, null, false));
            }
            catch (Exception e)
            {
                return null;
            }

            cSubcat oSubcat = clsSubcats.getSubcatById(nSubcatID);

            return oSubcat;
        }
    }

    #endregion

    #region Modules Object

    public class cModulesObject
    {
        /// <summary>
        /// Returns a list of the standard modules
        /// </summary>
        /// <returns>All modules</returns>
        public static List<cModule> CreateModuleListWithoutElements()
        {
            List<cModule> modules = new List<cModule>();
            modules.Add(cModulesObject.CreatePurchaseOrderModuleWithoutElements());
            modules.Add(cModulesObject.CreateExpensesModuleWithoutElements());
            modules.Add(cModulesObject.CreateFrameworkModuleWithoutElements());
            modules.Add(cModulesObject.CreateSpendManagementModuleWithoutElements());
            modules.Add(cModulesObject.CreateSmartDiligenceModuleWithoutElements());

            return modules;
        }

        /// <summary>
        /// Returns the Purchase Order module settings
        /// </summary>
        /// <returns>Purchase Order Module</returns>
        public static cModule CreatePurchaseOrderModuleWithoutElements()
        {
            return new cModule(1, "Purchase Orders", string.Empty, "<strong>Purchase Orders</strong>", "Purchase Orders");
        }

        /// <summary>
        /// Returns the Expenses module settings
        /// </summary>
        /// <returns>Expenses Module</returns>
        public static cModule CreateExpensesModuleWithoutElements()
        {
            return new cModule(2, "Expenses", string.Empty, "Expenses", "Expenses");
        }

        /// <summary>
        /// Returns the Framework module settings
        /// </summary>
        /// <returns>Framework Module</returns>
        public static cModule CreateFrameworkModuleWithoutElements()
        {
            return new cModule(3, "Contracts", string.Empty, "Framework", "Framework");
        }

        /// <summary>
        /// Returns the Spend Management module settings
        /// </summary>
        /// <returns>Spend Management Module</returns>
        public static cModule CreateSpendManagementModuleWithoutElements()
        {
            return new cModule(4, "Spend Management", string.Empty, "<strong>Spend Management</strong>", "Spend Management");
        }

        /// <summary>
        /// Returns the SmartDiligence module settings
        /// </summary>
        /// <returns>SmartDiligence Module</returns>
        public static cModule CreateSmartDiligenceModuleWithoutElements()
        {
            return new cModule(5, "SmartDiligence", string.Empty, "<strong>SmartDiligence</strong> &reg;", "SmartDiligence");
        }
    }

    #endregion

    #region Import Template Object

    public class cImportTemplateObject
    {
        /// <summary>
        /// Create a global import template object with associated mappings
        /// </summary>
        /// <returns></returns>
        public static cImportTemplate CreateImportTemplate()
        {
            cESRTrustObject.CreateESRTrustGlobalVariable();
            cImportTemplates clsImportTemps = new cImportTemplates(GlobalTestVariables.AccountID);
            List<cImportTemplateMapping> lstMappings = new List<cImportTemplateMapping>();

            #region Create Import template Mappings

            cFields clsfields = new cFields(GlobalTestVariables.AccountID);
            //Employee Info
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("6a76898b-4052-416c-b870-61479ca15ac1"), clsfields.GetFieldByID(new Guid("6a76898b-4052-416c-b870-61479ca15ac1")), "Employee Number", 2, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("28471060-247d-461c-abf6-234bcb4698aa"), clsfields.GetFieldByID(new Guid("28471060-247d-461c-abf6-234bcb4698aa")), "Title", 3, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("9d70d151-5905-4a67-944f-1ad6d22cd931"), clsfields.GetFieldByID(new Guid("9d70d151-5905-4a67-944f-1ad6d22cd931")), "Last Name", 4, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("6614acad-0a43-4e30-90ec-84de0792b1d6"), clsfields.GetFieldByID(new Guid("6614acad-0a43-4e30-90ec-84de0792b1d6")), "First Name", 5, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("b3caf703-e72b-4eb8-9d5c-b389e16c8c43"), clsfields.GetFieldByID(new Guid("b3caf703-e72b-4eb8-9d5c-b389e16c8c43")), "Middle Names", 6, ImportElementType.Employee, false, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("76473c0a-df08-40f9-8de0-632d0111a912"), clsfields.GetFieldByID(new Guid("76473c0a-df08-40f9-8de0-632d0111a912")), "Hire Date", 11, ImportElementType.Employee, true, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("b7cbf994-4a23-4405-93df-d66d4c3ed2a3"), clsfields.GetFieldByID(new Guid("b7cbf994-4a23-4405-93df-d66d4c3ed2a3")), "Termination Date", 12, ImportElementType.Employee, false, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("a0891ce2-d0c2-4b5b-9a78-7aaa3aaa87c1"), clsfields.GetFieldByID(new Guid("a0891ce2-d0c2-4b5b-9a78-7aaa3aaa87c1")), "Employee's Address 1st line", 14, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("0330c639-1524-402b-b7bc-04d26bfc05a1"), clsfields.GetFieldByID(new Guid("0330c639-1524-402b-b7bc-04d26bfc05a1")), "Employee's Address Town", 16, ImportElementType.Employee, false, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("9c9f07dd-a9d0-4ccf-9231-dd3c10d491b8"), clsfields.GetFieldByID(new Guid("9c9f07dd-a9d0-4ccf-9231-dd3c10d491b8")), "Employees Address Postcode", 18, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("8816caec-b520-4223-b738-47d2f22f3e1a"), clsfields.GetFieldByID(new Guid("8816caec-b520-4223-b738-47d2f22f3e1a")), "Employees Address Country", 19, ImportElementType.Employee, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("0f951c3e-29d1-49f0-ac13-4cfcabf21fda"), clsfields.GetFieldByID(new Guid("0f951c3e-29d1-49f0-ac13-4cfcabf21fda")), "Office e-Mail", 24, ImportElementType.Employee, false, DataType.stringVal));

            //ESR Assignment Info
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("7c0d8eab-d9af-415f-9bb7-d1be01f69e2f"), clsfields.GetFieldByID(new Guid("7c0d8eab-d9af-415f-9bb7-d1be01f69e2f")), "Assignment ID", 1, ImportElementType.Assignment, true, DataType.intVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("c23858b8-7730-440e-b481-c43fe8a1dbef"), clsfields.GetFieldByID(new Guid("c23858b8-7730-440e-b481-c43fe8a1dbef")), "Assignment Number", 2, ImportElementType.Assignment, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("c53828af-99ff-463f-93f9-2721df44e5f2"), clsfields.GetFieldByID(new Guid("c53828af-99ff-463f-93f9-2721df44e5f2")), "Earliest Assignment Start Date", 3, ImportElementType.Assignment, false, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("36eb4bb6-f4d5-414c-9106-ee62db01d902"), clsfields.GetFieldByID(new Guid("36eb4bb6-f4d5-414c-9106-ee62db01d902")), "Final Assignment End Date", 4, ImportElementType.Assignment, false, DataType.dateVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("9721ec22-404b-468b-83a4-d17d7559d3ef"), clsfields.GetFieldByID(new Guid("9721ec22-404b-468b-83a4-d17d7559d3ef")), "Assignment Status", 5, ImportElementType.Assignment, false, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("3e5750f0-1061-46c4-ad94-089d40a62dec"), clsfields.GetFieldByID(new Guid("3e5750f0-1061-46c4-ad94-089d40a62dec")), "Assignment Address 1st Line", 9, ImportElementType.Assignment, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("737a99ad-b0c5-4325-a565-c4d3fba536dd"), clsfields.GetFieldByID(new Guid("737a99ad-b0c5-4325-a565-c4d3fba536dd")), "Assignment Address Postcode", 13, ImportElementType.Assignment, true, DataType.stringVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("96f11c6d-7615-4abd-94ec-0e4d34e187a0"), clsfields.GetFieldByID(new Guid("96f11c6d-7615-4abd-94ec-0e4d34e187a0")), "Supervisor Employee Number", 17, ImportElementType.Assignment, false, DataType.intVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("fec46ed7-57f9-4c51-9916-ec92834371c3"), clsfields.GetFieldByID(new Guid("fec46ed7-57f9-4c51-9916-ec92834371c3")), "Primary Assignment", 22, ImportElementType.Assignment, true, DataType.booleanVal));
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("c50dae62-8dae-4289-a0ce-584c3129159e"), clsfields.GetFieldByID(new Guid("c50dae62-8dae-4289-a0ce-584c3129159e")), "Assignment Location", 37, ImportElementType.Assignment, true, DataType.stringVal));

            //Linked to the employees table
            lstMappings.Add(new cImportTemplateMapping(0, 0, new Guid("5f4a4551-1c05-4c85-b6d9-06d036bc327e"), clsfields.GetFieldByID(new Guid("5f4a4551-1c05-4c85-b6d9-06d036bc327e")), "Job Name", 39, ImportElementType.Assignment, false, DataType.stringVal));

            #endregion

            int tempImportTemplateID = clsImportTemps.saveImportTemplate(new cImportTemplate(0, "Unit Test Template", ApplicationType.ESROutboundImport, true, cGlobalVariables.NHSTrustID, lstMappings, DateTime.Now, GlobalTestVariables.EmployeeID, null, null));
            cGlobalVariables.TemplateID = tempImportTemplateID;

            //Interim solution
            //System.Threading.Thread.Sleep(1000);
            //clsImportTemps = new cImportTemplates(GlobalTestVariables.AccountID);

            cImportTemplate template = clsImportTemps.getImportTemplateByID(tempImportTemplateID);
            return template;
        }

        /// <summary>
        /// Delete the global import template object from the database
        /// </summary>
        public static void DeleteImportTemplate()
        {
            cImportTemplates clsImportTemps = new cImportTemplates(GlobalTestVariables.AccountID);
            clsImportTemps.deleteImportTemplate(cGlobalVariables.TemplateID);
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfo()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"test@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record which is invalid.
        /// The header record has been removed
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithNoHeaderRecord()
        {
            StringBuilder strBuild = new StringBuilder();

            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record which is invalid.
        /// The footer record has been removed
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithNoFooterRecord()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record which is invalid.
        /// The no of records in the footer record does not match the number of records in the file
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithInvalidNumberRecords()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,4");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithEmployeeNumberSetToNothing()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record
        /// with no employee title set
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithNoTitle()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"test@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record
        /// with no assignment location set 
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithNoAssignmentLocationSet()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"test@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// Create a byte array of data containing one ESR Outbound employee record and one ESR Outbound assignment record
        /// and the assignment record assignment number being set to nothing
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithAssignmentNumberSetToNothing()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"test@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,,19940801,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// This is an edited file with the employee email changed and the Assignment final date set
        /// </summary>
        /// <returns></returns>
        public static byte[] EditedDummyESROutboundFileInfo()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"61 Westwood Road\",\"Burnley\",\"\",\"\",\"BB12 0HR\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"editedTest@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,20100501,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Church Street\",\"\",\"Burnley\",\"LAN\",\"BB11 2DL\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"638 96 St Peter's Centre\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// This is an edited file with new home and work addresses
        /// </summary>
        /// <returns></returns>
        public static byte[] EditedDummyESROutboundFileInfoWithNewHomeAndWorkAddresses()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"1 Home Test Lane\",\"\",\"TestTown\",\"\",\"NG35 4TY\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"editedTest@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,20100501,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"1 Work Test lane\",\"\",\"WorkTestTown\",\"LINCS\",\"NG12 8YU\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"Unit Test Work\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,2");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// This is an ESR Outbound file with the employee first and then their associated line manager
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithLineManagerNotPreExisting()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"1 Home Test Lane\",\"\",\"TestTown\",\"\",\"NG35 4TY\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"editedTest@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,20100501,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"1 Work Test lane\",\"\",\"WorkTestTown\",\"LINCS\",\"NG12 8YU\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"Unit Test Work\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("PER,5073743,21272386,\"Mrs.\",\"Gill\",\"Catherine\",\"Spencer\",\"Walton\",\"Female\",19660407,NH829196B,20090302,,Yes,\"9, Oak Bank\",\"Whinney Hill Road\",\"Accrington\",\"Lancashire\",\"BB5 6NR\",\"GB\",\"linemanagertest@test.com\",\"01254 234539\",\"\",\"07528204188\",\"\",\"\",\"01282 474416\",,\"1610405\",\"\"\r\n");
            strBuild.Append("ASG,4689816,21272386,20090302,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Accrington Victoria Hospital\",\"Haywood Road\",\"Accrington\",\"LAN\",\"BB5 6AS\",\"GB\",1,,10807494,\"Mawdsley, Mrs. Carole Pamela\",\"\",FT,\"Permanent\",1,37.5,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"5317383|Modern Matron Band 7|NCH|Community Health Services|\",\"Modern Matron\",\"NCH\",\"638 04 Accrington Victoria Hos\",\"NHS|XR07|Review Body Band 7\",\"Nursing and Midwifery Registered|Modern Matron\",\"638 z 9643 USC Operational Managers|||\",\"GTA||\",,\"\",\"11569147\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,4");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }

        /// <summary>
        /// This is an ESR Outbound file with the employee first and then their associated line manager
        /// </summary>
        /// <returns></returns>
        public static byte[] CreateDummyESROutboundFileInfoWithLineManagerPreExisting()
        {
            StringBuilder strBuild = new StringBuilder();
            strBuild.Append("HDR,GO_123_ASG_20090706_Full_31820181.DAT\r\n");
            strBuild.Append("PER,5073743,21272386,\"Mrs.\",\"Gill\",\"Catherine\",\"Spencer\",\"Walton\",\"Female\",19660407,NH829196B,20090302,,Yes,\"9, Oak Bank\",\"Whinney Hill Road\",\"Accrington\",\"Lancashire\",\"BB5 6NR\",\"GB\",\"linemanagertest@test.com\",\"01254 234539\",\"\",\"07528204188\",\"\",\"\",\"01282 474416\",,\"1610405\",\"\"\r\n");
            strBuild.Append("ASG,4689816,21272386,20090302,,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"Accrington Victoria Hospital\",\"Haywood Road\",\"Accrington\",\"LAN\",\"BB5 6AS\",\"GB\",1,,10807494,\"Mawdsley, Mrs. Carole Pamela\",\"\",FT,\"Permanent\",1,37.5,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"5317383|Modern Matron Band 7|NCH|Community Health Services|\",\"Modern Matron\",\"NCH\",\"638 04 Accrington Victoria Hos\",\"NHS|XR07|Review Body Band 7\",\"Nursing and Midwifery Registered|Modern Matron\",\"638 z 9643 USC Operational Managers|||\",\"GTA||\",,\"\",\"11569147\"\r\n");
            strBuild.Append("PER,1507198,10805521,\"Mrs.\",\"Scott\",\"Debra\",\"Jane\",\"West\",\"Female\",19631127,NB880388B,19940801,,Yes,\"1 Home Test Lane\",\"\",\"TestTown\",\"\",\"NG35 4TY\",\"GB\",\"\",\"01282 433675\",\"\",\"\",\"editedTest@test.com\",\"\",\"\",,\"\",\"\"\r\n");
            strBuild.Append("ASG,1261442,10805521,19940801,20100501,\"Active Assignment\",Contracted,\"638 Monthly\",\"Calendar Month\",\"1 Work Test lane\",\"\",\"WorkTestTown\",\"LINCS\",\"NG12 8YU\",\"GB\",0,,21272386,\"Gill, Mrs. Catherine Spencer\",\"AfC Annual Leave Accrual 1 NHS\",PT,\"Permanent\",1,25,\"Week\",37.5,,,\"\",\"\",\"\",\"Availability Schedule\",\"638 L8 SP-CLTC-7850 Out of Hours Nursing\",5G7,\"4362624|Community Nurse Band 5|N6H|Community Health Services|\",\"Community Nurse\",\"N6H\",\"Unit Test Work\",\"NHS|XR05|Review Body Band 5\",\"Nursing and Midwifery Registered|Community Nurse\",\"638 z 9789 OOH St Peters Centre|Standard User||\",\"GTA||\",,\"\",\"\"\r\n");
            strBuild.Append("FTR,GO_123_ASG_20090706_Full_31820181.DAT,4");

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] data = encoding.GetBytes(strBuild.ToString());

            return data;
        }
    }

    #endregion

    #region ESR Trust Object

    public class cESRTrustObject
    {
        /// <summary>
        /// Global static variable for a trust object to be used in the unit tests for the ESR Trusts
        /// </summary>
        /// <returns></returns>
        public static cESRTrust CreateESRTrustGlobalVariable()
        {
            cESRTrusts clsTrusts = new cESRTrusts(GlobalTestVariables.AccountID);
            int tempTrustID = clsTrusts.SaveTrust(new cESRTrust(0, "TestTrust" + DateTime.Now.Ticks.ToString(), "123", "M", "N", 1, "ftp", "UnitTestAccount", "UnitTestPassword", false, new DateTime?(DateTime.Now), new DateTime?(DateTime.Now), ","));
            cGlobalVariables.NHSTrustID = tempTrustID;
            clsTrusts = new cESRTrusts(GlobalTestVariables.AccountID);
            cESRTrust trust = clsTrusts.GetESRTrustByID(tempTrustID);
            return trust;
        }

        /// <summary>
        /// Global static variable for a trust object to be used in the unit tests for the ESR Trusts where its values that 
        /// can be set to null or nothing are
        /// </summary>
        /// <returns></returns>
        public static cESRTrust CreateESRTrustGlobalVariableWithValuesThatCanBeSetToNullOrNothing()
        {
            cESRTrusts clsTrusts = new cESRTrusts(GlobalTestVariables.AccountID);
            int tempTrustID = clsTrusts.SaveTrust(new cESRTrust(0, "TestTrust" + DateTime.Now.Ticks.ToString(), "123", "M", "N", 1, "", "", "", false, null, null, ","));
            cGlobalVariables.NHSTrustID = tempTrustID;
            clsTrusts = new cESRTrusts(GlobalTestVariables.AccountID);
            cESRTrust trust = clsTrusts.GetESRTrustByID(tempTrustID);
            return trust;
        }

        /// <summary>
        /// Delete the static trust object from the database
        /// </summary>
        public static void DeleteTrust()
        {
            cESRTrusts clsTrusts = new cESRTrusts(GlobalTestVariables.AccountID);
            clsTrusts.DeleteTrust(cGlobalVariables.NHSTrustID);
        }

        /// <summary>
        /// Global static variable for a trust object used in the inbound and outbound services to be used in the unit tests
        /// </summary>
        /// <returns></returns>
        public static void CreateServiceESRTrustGlobalVariable()
        {
            cNHSTrusts clsTrusts = new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            cSecureData clsSecure = new cSecureData();
            clsTrusts.SaveTrust(new cESRTrust(0, 999, GlobalTestVariables.AccountID, "UnitTestTrust" + DateTime.Now.Ticks.ToString(), "123", "ftp", "UnitTestAccount", clsSecure.Encrypt("UnitTestPassword"), false, new DateTime?(DateTime.Now), new DateTime?(DateTime.Now)), GlobalTestVariables.AccountID);

            clsTrusts = new cNHSTrusts(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);

            #region Set the Global Inbound Data ID Variable

            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "SELECT trustID FROM NHSTrustDetails WHERE TrustName = @TrustName";
            smData.sqlexecute.Parameters.AddWithValue("@TrustName", "UnitTestTrust");

            int TrustID = 0;

            using (SqlDataReader reader = smData.GetReader(strSQL))
            {
                while (reader.Read())
                {
                    TrustID = reader.GetInt32(0);
                }

                reader.Close();
            }

            smData.sqlexecute.Parameters.Clear();

            cGlobalVariables.NHSTrustID = TrustID;

            #endregion

        }

        /// <summary>
        /// Delete the service trust from the ESRFileTransfer database
        /// </summary>
        public static void DeleteServiceTrust()
        {
            DBConnection smData = new DBConnection(ConfigurationManager.ConnectionStrings["ESRFileTransfer"].ConnectionString);
            string strSQL = "DELETE FROM NHSTrustDetails WHERE trustID = @TrustID";
            smData.sqlexecute.Parameters.AddWithValue("@TrustID", cGlobalVariables.NHSTrustID);
            smData.ExecuteSQL(strSQL);

            if (cNHSTrusts.lstTrusts != null)
            {
                cNHSTrusts.lstTrusts.Remove(cGlobalVariables.NHSTrustID);
            }
        }

    }

    #endregion

    #region Global Properties Object

    public class cGlobalPropertiesObject
    {
        public static void UpdateGlobalCurrency(int SubAccountID, int CurrencyID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountID));
            string strsql = "UPDATE accountProperties SET stringValue = @currencyid WHERE stringKey = 'baseCurrency' and subAccountID = @subaccountid";

            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", CurrencyID);
            expdata.sqlexecute.Parameters.AddWithValue("@subaccountid", SubAccountID);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Set the username format for the ESR outbound imports 
        /// </summary>
        /// <param name="format"></param>
        public static void UpdateUsernameFormat(string format)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountID));
            string strsql = "UPDATE accountProperties SET stringValue = @format WHERE stringKey = 'importUsernameFormat' and subAccountID = @subaccountid";

            expdata.sqlexecute.Parameters.AddWithValue("@format", format);
            expdata.sqlexecute.Parameters.AddWithValue("@subaccountid", GlobalTestVariables.SubAccountID);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Set the home address format for the ESR outbound imports 
        /// </summary>
        /// <param name="format"></param>
        public static void UpdateHomeAddressFormat(string format)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountID));
            string strsql = "UPDATE accountProperties SET stringValue = @format WHERE stringKey = 'ImportHomeAddressFormat' and subAccountID = @subaccountid";

            expdata.sqlexecute.Parameters.AddWithValue("@format", format);
            expdata.sqlexecute.Parameters.AddWithValue("@subaccountid", GlobalTestVariables.SubAccountID);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }
    }

    #endregion

    #region Base Definition Objects

    public class cBaseDefinitionObject
    {
        /// <summary>
        /// Used to store the current Base Definition ID as created by the dummy object
        /// </summary>
        public static int GlobalID;

        /// <summary>
        /// This creates a base definition for the specific type passed in
        /// </summary>
        /// <param name="TableID"></param>
        /// <param name="element"></param>
        public static cBaseDefinition CreateBaseDefinition(SpendManagementElement element, ref cBaseDefinition expected)
        {
            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountID, GlobalTestVariables.SubAccountID, element);

            List<cNewGridColumn> lstColumns = new List<cNewGridColumn>();
            Guid TableID = GetBaseDefTableID(element);

            //Get the fields associated with the base definition 
            List<cField> lstFields = clsBaseDefs.SetBaseDefinitionFields(TableID, ref lstColumns);

            //Get a base definition object with dummy information set 
            expected = getDummyBaseDefinitionObject(element);
            object dataValue;
            List<cBaseDefinitionValues> defValues = new List<cBaseDefinitionValues>();

            //Get the properties of the base definition object
            PropertyInfo[] piList = expected.GetType().GetProperties();

            #region Get the data values to populate the base definition  to pass to the save method

            foreach (cField field in lstFields)
            {
                foreach (PropertyInfo pi in piList)
                {
                    if (pi.CanRead)
                    {
                        if (field.ClassPropertyName == pi.Name)
                        {
                            dataValue = pi.GetValue(expected, null).ToString();

                            defValues.Add(new cBaseDefinitionValues(field.FieldName, field.ParentTable.TableName, field.TableID.ToString(), field.FieldType, field.GenList, field.ValueList, field.FieldID.ToString(), dataValue.ToString()));
                        }
                    }
                }
            }

            #endregion

            int ID = clsBaseDefs.SaveDefinition(-1, defValues.ToArray());
            cBaseDefinitionObject.GlobalID = ID;
            cBaseDefinition newBaseDef = clsBaseDefs.GetDefinitionByID(ID);

            return newBaseDef;
        }

        /// <summary>
        /// Edit a base definition
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static cBaseDefinition EditBaseDefinition(SpendManagementElement element)
        {
            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountID, GlobalTestVariables.SubAccountID, element);
            cBaseDefinition nullDef = null;
            cBaseDefinition baseDef = CreateBaseDefinition(element, ref nullDef);
            List<cNewGridColumn> lstColumns = new List<cNewGridColumn>();
            Guid TableID = GetBaseDefTableID(element);

            //Get the fields associated with the base definition 
            List<cField> lstFields = clsBaseDefs.SetBaseDefinitionFields(TableID, ref lstColumns);


            baseDef.Description = "Edited" + DateTime.Now.Ticks.ToString().Substring(0, 14);
            baseDef.ModifiedOn = DateTime.UtcNow;
            baseDef.ModifiedBy = GlobalTestVariables.EmployeeID;

            object dataValue;
            List<cBaseDefinitionValues> defValues = new List<cBaseDefinitionValues>();

            //Get the properties of the base definition object
            PropertyInfo[] piList = baseDef.GetType().GetProperties();

            #region Get the data values to populate the base definition  to pass to the save method

            foreach (cField field in lstFields)
            {
                foreach (PropertyInfo pi in piList)
                {
                    if (pi.CanRead)
                    {
                        if (field.ClassPropertyName == pi.Name)
                        {
                            dataValue = pi.GetValue(baseDef, null).ToString();

                            defValues.Add(new cBaseDefinitionValues(field.FieldName, field.ParentTable.TableName, field.TableID.ToString(), field.FieldType, field.GenList, field.ValueList, field.FieldID.ToString(), dataValue.ToString()));
                        }
                    }
                }
            }

            #endregion

            int ID = clsBaseDefs.SaveDefinition(baseDef.ID, defValues.ToArray());
            cBaseDefinitionObject.GlobalID = ID;

            return baseDef;
        }

        /// <summary>
        /// Delete the base definition from the database
        /// </summary>
        /// <param name="element"></param>
        public static void DeleteBaseDefinition(SpendManagementElement element)
        {
            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountID, GlobalTestVariables.SubAccountID, element);
            clsBaseDefs.DeleteDefinition(cBaseDefinitionObject.GlobalID);
        }

        /// <summary>
        /// Archive the base definition
        /// </summary>
        /// <param name="element"></param>
        public static void ArchiveBaseDefinition(SpendManagementElement element)
        {
            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(GlobalTestVariables.AccountID, GlobalTestVariables.SubAccountID, element);
            clsBaseDefs.ArchiveDefinition(cBaseDefinitionObject.GlobalID);
        }

        /// <summary>
        /// Get a dummy instance of a base definition object
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static cBaseDefinition getDummyBaseDefinitionObject(SpendManagementElement element)
        {
            string strDesc = "UTBD" + DateTime.Now.Ticks.ToString().Substring(0, 16);

            switch (element)
            {
                case SpendManagementElement.ContractStatus:
                    return new cContractStatus(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false, false);
                case SpendManagementElement.ContractCategories:
                    return new cContractCategory(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false);
                case SpendManagementElement.ContractTypes:
                    return new cContractType(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false);
                case SpendManagementElement.InvoiceFrequencyTypes:
                    return new cInvoiceFrequencyType(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false, 2);
                case SpendManagementElement.InvoiceStatus:
                    return new cInvoiceStatus(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false, false);
                case SpendManagementElement.LicenceRenewalTypes:
                    return new cLicenceRenewalType(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false);
                case SpendManagementElement.InflatorMetrics:
                    return new cInflatorMetric(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false, decimal.Parse("100.00"), true);
                case SpendManagementElement.TermTypes:
                    return new cTermType(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false);
                case SpendManagementElement.FinancialStatus:
                    return new cFinancialStatus(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false);
                case SpendManagementElement.TaskTypes:
                    return new cTaskType(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false);
                case SpendManagementElement.Units:
                    return new cUnit(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false);
                case SpendManagementElement.ProductCategories:
                    return new cProductCategory(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false);
                case SpendManagementElement.SupplierStatus:
                    return new cSupplierStatus(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false, 1, true);
                case SpendManagementElement.SupplierCategory:
                    return new cSupplierCategory(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false);
                case SpendManagementElement.ProductLicenceTypes:
                    return new cProductLicenceType(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false);
                case SpendManagementElement.SalesTax:
                    return new cSalesTax(0, strDesc, DateTime.Now, GlobalTestVariables.EmployeeID, null, null, false, decimal.Parse("100.00"));
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get the Table ID for the associated element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Guid GetBaseDefTableID(SpendManagementElement element)
        {
            string strTableID;

            #region Set the Guid value based on the element type

            switch (element)
            {
                case SpendManagementElement.ContractCategories:
                    strTableID = "20133759-fdb8-40d5-bd52-82450124168a";
                    break;
                case SpendManagementElement.ContractStatus:
                    strTableID = "8ceaa3fa-4b2c-4846-b988-22cc7e643d94";
                    break;
                case SpendManagementElement.ContractTypes:
                    strTableID = "53418d93-5c7b-4b14-b222-1f6bcbe59840";
                    break;
                case SpendManagementElement.TermTypes:
                    strTableID = "3a5d47d7-7dcf-4388-b2f3-d385304eecac";
                    break;
                case SpendManagementElement.FinancialStatus:
                    strTableID = "2ce5601d-6223-4269-b993-0e8aeb345a55";
                    break;
                case SpendManagementElement.InflatorMetrics:
                    strTableID = "85c8555c-0172-4feb-aa59-85f8607e4253";
                    break;
                case SpendManagementElement.InvoiceFrequencyTypes:
                    strTableID = "f6f78056-aea7-4089-b0dd-d39512aab2da";
                    break;
                case SpendManagementElement.InvoiceStatus:
                    strTableID = "27f00143-8058-4108-a0ec-ac73d6964382";
                    break;
                case SpendManagementElement.TaskTypes:
                    strTableID = "bd9b3bc1-54b6-4c93-87bc-16920f11f9c9";
                    break;
                case SpendManagementElement.Units:
                    strTableID = "6ac80b0f-9c1f-43ea-9aed-6eb7104a7a89";
                    break;
                case SpendManagementElement.ProductCategories:
                    strTableID = "fdc07b2d-1253-4b6d-a7e6-242242d958bc";
                    break;
                case SpendManagementElement.LicenceRenewalTypes:
                    strTableID = "6f291ba0-d13e-43db-bbea-3afbceda0570";
                    break;
                case SpendManagementElement.SupplierStatus:
                    strTableID = "e8cde388-ef35-4349-b685-9d45da385ef1";
                    break;
                case SpendManagementElement.SupplierCategory:
                    strTableID = "a9c7d7b7-ebed-4a25-bc5d-69d507afbe75";
                    break;
                case SpendManagementElement.ProductLicenceTypes:
                    strTableID = "b9f161fc-1888-435d-bf23-10ba0069de53";
                    break;
                case SpendManagementElement.SalesTax:
                    strTableID = "e9734332-1e62-43c5-a8f2-14b518c87542";
                    break;
                default:
                    strTableID = "";
                    break;
            }

            #endregion

            return new Guid(strTableID); ;
        }
    }

    /// <summary>
    /// A static collection of the base defintion element types
    /// </summary>
    public static class cBaseDefinitionElements
    {
        public static readonly List<SpendManagementElement> lstBaseDefinitions;
        public static readonly List<string> lstOmittedProperties;

        static cBaseDefinitionElements()
        {
            lstBaseDefinitions = new List<SpendManagementElement>();

            lstBaseDefinitions.Add(SpendManagementElement.ContractCategories);
            lstBaseDefinitions.Add(SpendManagementElement.ContractStatus);
            lstBaseDefinitions.Add(SpendManagementElement.ContractTypes);
            lstBaseDefinitions.Add(SpendManagementElement.TermTypes);
            lstBaseDefinitions.Add(SpendManagementElement.FinancialStatus);
            lstBaseDefinitions.Add(SpendManagementElement.InflatorMetrics);
            lstBaseDefinitions.Add(SpendManagementElement.InvoiceFrequencyTypes);
            lstBaseDefinitions.Add(SpendManagementElement.InvoiceStatus);
            lstBaseDefinitions.Add(SpendManagementElement.TaskTypes);
            lstBaseDefinitions.Add(SpendManagementElement.Units);
            lstBaseDefinitions.Add(SpendManagementElement.ProductCategories);
            lstBaseDefinitions.Add(SpendManagementElement.LicenceRenewalTypes);
            lstBaseDefinitions.Add(SpendManagementElement.SupplierStatus);
            lstBaseDefinitions.Add(SpendManagementElement.SupplierCategory);
            lstBaseDefinitions.Add(SpendManagementElement.ProductLicenceTypes);
            lstBaseDefinitions.Add(SpendManagementElement.SalesTax);

            lstOmittedProperties = new List<string>();

            lstOmittedProperties.Add("ID");
            lstOmittedProperties.Add("CreatedOn");
            lstOmittedProperties.Add("CreatedBy");
            lstOmittedProperties.Add("ModifiedOn");
            lstOmittedProperties.Add("ModifiedBy");
        }
    }
    #endregion Base Definition Objects

    #region Sub Account Objects

    public class cSubAccountObject
    {
        public static int AlternateSubAccountID;
        public static int RoleID;

        public static cAccountSubAccount CreateSubAccount()
        {
            cAccountSubAccounts subaccs = new cAccountSubAccounts(GlobalTestVariables.AccountID);

            cAccountSubAccount subacc = new cAccountSubAccount(-1, "Test " + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), false, new cAccountProperties(), DateTime.UtcNow, GlobalTestVariables.EmployeeID, null, null);
            AlternateSubAccountID = subaccs.UpdateSubAccount(subacc, GlobalTestVariables.EmployeeID, GlobalTestVariables.SubAccountID);

            subaccs = new cAccountSubAccounts(GlobalTestVariables.AccountID);
            subacc = subaccs.getSubAccountById(AlternateSubAccountID);
            return subacc;
        }

        public static void DeleteSubAccount()
        {
            if (AlternateSubAccountID != GlobalTestVariables.SubAccountID)
            {
                cAccountSubAccounts subaccs = new cAccountSubAccounts(GlobalTestVariables.AccountID);
                subaccs.DeleteSubAccount(AlternateSubAccountID, GlobalTestVariables.EmployeeID);
            }
        }

        public static void GrantAccessRole(int subaccountid)
        {
            cAccessRoles roles = new cAccessRoles(GlobalTestVariables.AccountID, cAccounts.getConnectionString(GlobalTestVariables.AccountID));
            cAccessRole role = roles.AccessRoles[roles.AccessRoles.Keys[0]];
            cSubAccountObject.RoleID = role.RoleID;

            cEmployees emps = new cEmployees(GlobalTestVariables.AccountID);

            emps.saveAccessRoles(GlobalTestVariables.EmployeeID, new List<int>() { cSubAccountObject.RoleID }, GlobalTestVariables.SubAccountID);

            return;
        }

        public static void CleanupAccessRoles()
        {
            cEmployees emps = new cEmployees(GlobalTestVariables.AccountID);

            emps.deleteEmployeeAccessRole(GlobalTestVariables.EmployeeID, cSubAccountObject.RoleID, GlobalTestVariables.SubAccountID);
        }
    }

    #endregion

    #region Card Statement Objects

    public class cCardStatementObject
    {
        /// <summary>
        /// Get a template card statement object
        /// </summary>
        /// <returns></returns>
        public static cCardStatement GetBarclaycardCardStatement()
        {
            cCardStatement statement = new cCardStatement(0, "TestStatement" + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), new DateTime(2011, 01, 01), cCorporateCardObject.CreateCorporateCard("Barclaycard"), DateTime.UtcNow, 0, null, null);
            return statement;
        }

        /// <summary>
        /// Get a template card statement object
        /// </summary>
        /// <returns></returns>
        public static cCardStatement GetBarclaycardVCF4CardStatement()
        {
            cCardStatement statement = new cCardStatement(0, "TestStatement" + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), new DateTime(2011, 01, 01), cCorporateCardObject.CreateCorporateCard("Barclaycard VCF4"), DateTime.UtcNow, 0, null, null);
            return statement;
        }

        /// <summary>
        /// Get a template card statement object with null statement date
        /// </summary>
        /// <returns></returns>
        public static cCardStatement GetBarclaycardCardStatementWithNullStatementDate()
        {
            cCardStatement statement = new cCardStatement(0, "TestStatement" + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), null, cCorporateCardObject.CreateCorporateCard("Barclaycard"), DateTime.UtcNow, 0, null, null);
            return statement;
        }

        /// <summary>
        /// Create and save the card statement to the database
        /// </summary>
        /// <returns></returns>
        public static cCardStatement CreateCardStatement(cCardStatement tmpStatement)
        {
            cCardStatements clsStatements = new cCardStatements(GlobalTestVariables.AccountID);

            int tempID = clsStatements.addStatement(tmpStatement);
            cCardStatement statement = clsStatements.getStatementById(tempID);
            return statement;
        }

        /// <summary>
        /// Delete the card statement from the database
        /// </summary>
        /// <param name="ID"></param>
        public static void DeleteCardStatement(int ID)
        {
            cCardStatements clsStatements = new cCardStatements(GlobalTestVariables.AccountID);
            clsStatements.deleteStatement(ID);
            cCorporateCardObject.DeleteCorporateCard(cGlobalVariables.CorporateCardID);
        }

        /// <summary>
        /// Method to check the success of imported transactions making sure they are added to the card transactions table and the additional table
        /// </summary>
        /// <param name="statementID"></param>
        /// <returns></returns>
        public static CardImportSuccess GetStatementTransactionImportSuccess(int statementID, string provider)
        {
            CardImportSuccess impSuccess = CardImportSuccess.Success;
            int cardTransactionCount = 0;
            int cardTransactionAdditionalCount = 0;
            DBConnection db = new DBConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountID));
            string strsql = "SELECT count(transactionid) from card_transactions WHERE statementid = @statementID";
            db.sqlexecute.Parameters.AddWithValue("@statementID", statementID);

            cardTransactionCount = db.getcount(strsql);

            strsql = string.Empty;

            switch (provider)
            {
                case "Allstar Fuel Card":
                    strsql = "SELECT count(ctAdditional.transactionid) from card_transactions_allstar ctAdditional";
                    break;
                case "Barclaycard":
                    strsql = "SELECT count(ctAdditional.transactionid) from card_transactions_barclaycard ctAdditional";
                    break;
                case "Barclaycard VCF4":
                    strsql = "SELECT count(ctAdditional.transactionid) from card_transactions_vcf4 ctAdditional";
                    break;
                default:
                    break;
            }

            if (strsql != string.Empty)
            {
                strsql += " inner join card_transactions ct on ct.transactionid = ctAdditional.transactionid where ct.statementid = @statementID";

                cardTransactionAdditionalCount = db.getcount(strsql);

                if (cardTransactionCount == 0 || cardTransactionAdditionalCount == 0)
                {
                    impSuccess = CardImportSuccess.Fail;
                }
                else if (cardTransactionCount != cardTransactionAdditionalCount)
                {
                    impSuccess = CardImportSuccess.TransactionCountsDontMatch;
                }
            }
            else
            {
                impSuccess = CardImportSuccess.Fail;
            }

            db.sqlexecute.Parameters.Clear();
            return impSuccess;
        }

        public static void ImportCardTransactions(cCardStatement statement, string provider)
        {
            cCardStatements clsCardStatements = new cCardStatements(GlobalTestVariables.AccountID);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate(provider);
            string transactionDataFileName = string.Empty;
            string sLocationForCardTestFiles = GlobalTestVariables.GetWebConfigStringValue("LocationForCardTestFiles");

            switch (provider)
            {
                case "Allstar Fuel Card":
                    transactionDataFileName = "Allstar Exclude Rows.txt";
                    break;
                case "Barclaycard":
                    transactionDataFileName = "Barclaycard Flat File.txt";
                    break;
                case "Barclaycard VCF4":
                    transactionDataFileName = "VCF4.txt";
                    break;
                default:
                    break;
            }

            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + transactionDataFileName);
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + transactionDataFileName);
            }
            cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);
            clsCardStatements.ImportCardTransactions(temp, statement.statementid, statement.corporatecard.cardprovider, fileData, import);
        }

        /// <summary>
        /// Add the card compaies to tbe database
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="provider"></param>
        public static void ImportVCF4CardCompanies(cCardStatement statement)
        {
            cCardStatements clsCardStatements = new cCardStatements(GlobalTestVariables.AccountID);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("Barclaycard VCF4");
            string sLocationForCardTestFiles = GlobalTestVariables.GetWebConfigStringValue("LocationForCardTestFiles");

            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "VCF4.txt");
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "VCF4.txt");
            }
            cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardCompany], fileData);

            SortedList<string, string> lstCompanies = clsCardStatements.GetVCF4CompanyRecords(import, temp.RecordTypes[CardRecordType.CardCompany]);

            cCardCompanies clsCardComps = new cCardCompanies(GlobalTestVariables.AccountID);

            foreach (KeyValuePair<string, string> kvp in lstCompanies)
            {
                clsCardComps.SaveCardCompany(new cCardCompany(0, kvp.Value, kvp.Key, true, null, null, null, null));
            }
        }

        /// <summary>
        /// Delete any card companies from the database
        /// </summary>
        public static void DeleteVCF4CardCompanies()
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountID));
            db.ExecuteSQL("DELETE FROM cardCompanies");
        }

        /// <summary>
        /// Get a card number from a transaction for an imported statement
        /// </summary>
        /// <param name="statementID"></param>
        /// <returns></returns>
        public static string GetCardNumber(int statementID)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountID));
            string strsql = "SELECT TOP 1 card_number from card_transactions WHERE statementid = @statementID";
            db.sqlexecute.Parameters.AddWithValue("@statementID", statementID);

            string cardNumber = db.getStringValue(strsql);

            db.sqlexecute.Parameters.Clear();

            return cardNumber;
        }

        /// <summary>
        /// Get a Transaction ID from a transaction for an imported statement
        /// </summary>
        /// <param name="statementID"></param>
        /// <returns></returns>
        public static int GetTransactionID(int statementID)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountID));
            string strsql = "SELECT TOP 1 transactionid from card_transactions WHERE statementid = @statementID";
            db.sqlexecute.Parameters.AddWithValue("@statementID", statementID);

            int transactionID = db.getIntSum(strsql);

            db.sqlexecute.Parameters.Clear();

            return transactionID;
        }

        /// <summary>
        /// List of omitted properties for the compare assert
        /// </summary>
        public static List<string> lstOmittedProperties
        {
            get
            {
                List<string> OmittedProperties = new List<string>();

                OmittedProperties.Add("statementid");
                OmittedProperties.Add("createdon");
                OmittedProperties.Add("createdby");
                OmittedProperties.Add("modifiedon");
                OmittedProperties.Add("modifiedby");

                return OmittedProperties;
            }
        }


    }

    /// <summary>
    /// Enumerable type to determine if the imported transactions are adding and to ensure if there is an additional table that the amount of transactions is the same as the card transactions table
    /// </summary>
    public enum CardImportSuccess
    {
        /// <summary>
        /// Successfully imported and the counts are the same
        /// </summary>
        Success = 0,

        /// <summary>
        /// One or both counts are 0 meaning there was a failure in the import 
        /// </summary>
        Fail = 1,

        /// <summary>
        /// Both counts have a value but they do not match
        /// </summary>
        TransactionCountsDontMatch = 2
    }

    #endregion

    #region Corporate Card Objects

    public class cCorporateCardObject
    {
        /// <summary>
        /// Get the global corporate card object    
        /// </summary>
        /// <returns></returns>
        public static cCorporateCard GetCorporateCard(string provider)
        {
            cCorporateCard card = new cCorporateCard(GetCardProvider(provider), false, DateTime.UtcNow, 0, null, null, null, false, false, true, false);
            return card;
        }

        /// <summary>
        /// Get a template card provider object
        /// </summary>
        /// <returns></returns>
        public static cCardProvider GetCardProvider(string provider)
        {
            cCardProviders clsCardProviders = new cCardProviders();
            cCardProvider prov = clsCardProviders.getProviderByName(provider);
            return prov;
        }

        /// <summary>
        /// Create and save the corporate card object to the database and return
        /// </summary>
        /// <returns></returns>
        public static cCorporateCard CreateCorporateCard(string CardProvider)
        {
            cCorporateCards clsCorpCards = new cCorporateCards(GlobalTestVariables.AccountID);
            cCorporateCard card = GetCorporateCard(CardProvider);

            clsCorpCards.addCorporateCard(card);

            SortedList<string, cCorporateCard> lstCorpCards = clsCorpCards.sortList();

            cCorporateCard tempCard = null;
            lstCorpCards.TryGetValue(card.cardprovider.cardprovider, out tempCard);

            cGlobalVariables.CorporateCardID = tempCard.cardprovider.cardproviderid;
            return tempCard;
        }

        /// <summary>
        /// Delete the corprate card from the database
        /// </summary>
        /// <param name="ID"></param>
        public static void DeleteCorporateCard(int ID)
        {
            cCorporateCards clsCorpCards = new cCorporateCards(GlobalTestVariables.AccountID);
            clsCorpCards.deleteCorporateCard(ID);
        }
    }

    #endregion

    #region Card Template Objects

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
            cCardTemplates clsCardTemplates = new cCardTemplates(GlobalTestVariables.AccountID);
            cCardTemplate temp = clsCardTemplates.getTemplate(provider);
            return temp;
        }
    }

    #endregion

    #region Global Mime Type Objects

    public class cGlobalMimeTypeObject
    {
        /// <summary>
        /// Create the global mime type object
        /// </summary>
        /// <returns></returns>
        public static cGlobalMimeType CreateGlobalMimeType()
        {
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(GlobalTestVariables.AccountID);
            cGlobalMimeType gMime = GetGlobalMimeType();
            clsGlobalMimeTypes.SaveCustomMimeHeader(gMime);
            gMime = clsGlobalMimeTypes.getMimeTypeByExtension(gMime.FileExtension);

            return gMime;
        }

        /// <summary>
        /// A global mime type object with all values set
        /// </summary>
        /// <returns></returns>
        public static cGlobalMimeType GetGlobalMimeType()
        {
            cGlobalMimeType tempMime = new cGlobalMimeType(Guid.Empty, "test" + DateTime.Now.Ticks.ToString().Substring(0, 16), "test", "testdesc");
            return tempMime;
        }

        /// <summary>
        /// A global mime type object with all values set to null or Nothing that can be
        /// </summary>
        /// <returns></returns>
        public static cGlobalMimeType GetGlobalMimeTypeWithValuesSetToNullOrNothingThatCanBe()
        {
            cGlobalMimeType tempMime = new cGlobalMimeType(Guid.Empty, "test" + DateTime.Now.Ticks.ToString().Substring(0, 16), "test", "");
            return tempMime;
        }

        /// <summary>
        /// Create the global mime type object with all values set to null or Nothing that can be
        /// </summary>
        /// <returns></returns>
        public static cGlobalMimeType CreateGlobalMimeTypeWithValuesSetToNullOrNothingThatCanBe()
        {
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(GlobalTestVariables.AccountID);
            cGlobalMimeType gMime = GetGlobalMimeTypeWithValuesSetToNullOrNothingThatCanBe();
            clsGlobalMimeTypes.SaveCustomMimeHeader(gMime);
            gMime = clsGlobalMimeTypes.getMimeTypeByExtension(gMime.FileExtension);

            return gMime;
        }

        /// <summary>
        /// Delete the global mime type object used for the unit test
        /// </summary>
        /// <param name="ID"></param>
        public static void DeleteGlobalMimeType(Guid ID)
        {
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(GlobalTestVariables.AccountID);
            clsGlobalMimeTypes.DeleteCustomMimeHeader(ID);
        }
    }

    #endregion

    #region Mime Type Objects

    public class cMimeTypeObject
    {
        /// <summary>
        /// Create the global mime type object
        /// </summary>
        /// <returns></returns>
        public static cMimeType CreateMimeType()
        {
            cGlobalMimeType gMime = cGlobalMimeTypeObject.CreateGlobalMimeType();

            cMimeTypes clsMimeTypes = new cMimeTypes(GlobalTestVariables.AccountID, GlobalTestVariables.SubAccountID);
            int ID = clsMimeTypes.SaveMimeType(gMime.GlobalMimeID);

            cMimeType mime = clsMimeTypes.GetMimeTypeByID(ID);
            return mime;
        }

        /// <summary>
        /// Delete the global mime type
        /// </summary>
        /// <param name="ID"></param>
        public static void DeleteMimeType(int ID)
        {
            cMimeTypes clsMimeTypes = new cMimeTypes(GlobalTestVariables.AccountID, GlobalTestVariables.SubAccountID);
            cMimeType mime = clsMimeTypes.GetMimeTypeByID(ID);

            if (mime != null)
            {
                cGlobalMimeTypeObject.DeleteGlobalMimeType(mime.GlobalMimeID);
                clsMimeTypes.DeleteMimeType(ID);
            }
        }
    }

    #endregion
}
