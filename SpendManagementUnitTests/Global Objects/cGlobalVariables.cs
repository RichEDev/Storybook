using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary;
using System.Reflection;

namespace SpendManagementUnitTests
{
    class cGlobalVariables
    {
        public static int EmployeeID = 517;
        public static int DelegateID;
        public static int AccountID = 322;
        public static int SubAccountID = 1;
        public static int DefaultSubAccountID = 1;
        public static int ClaimID;
        public static int ExpenseID;
        public static int CurrencyID;
        public static int GlobalCurrencyID;
        public static int NHSTrustID;
        public static int ESRElementID;
        public static int CategoryID;
        public static int SubcatID;
        public static int ESRAssignmentID;
        public static int TemplateID;
        public static int OutboundEmployeID;
        public static int HomeAddressID;
        public static int WorkAddressID;
        public static int OutboundLinemanagerEmployeeID;
        public static int ExportID;
        public static int HistoryID;
        public static int TaskID;
        public static int InboundDataID;
        public static int OutboundDataID;
        public static int BaseDefinitionID;
        public static int RoleID;
        public static int SupplierID;
        public static int SupplierStatusID;
        public static int SupplierCategoryID;
        public static int FinancialStatusID;
        public static byte BroadcastID;
        public static int NoteID;
        public static int CostcodeID;
        public static int DepartmentID;
        public static int ProjectcodeID;
        public static int ItemRoleID;
        public static Guid ConcurrentUserManageID;
        public static string CreditCardTestFilesPath = @"<DRIVE_LETTER>:\Projects\Spend Management\<BRANCH_NAME>\SpendManagementUnitTests\Credit Cards\Test Data Files\";
        public static int CorporateCardID;
        public static string CardProvider;
    }



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
                        if (pi.Name == fieldName)
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

                switch(t.FullName)
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

    public static class cESRTrustAssert
    {
        public static void AreEqual(cESRTrust expected, cESRTrust actual)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            if (expected.AccountID != actual.AccountID)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cESRTrust.AccountID values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.AccountID,
                        actual.AccountID));
            }
            if (expected.Archived != actual.Archived)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cESRTrust.Archived values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.Archived,
                        actual.Archived));
            }
            if (expected.CreatedOn != actual.CreatedOn)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cESRTrust.CreatedOn values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.CreatedOn,
                        actual.CreatedOn));
            }
            if (expected.expTrustID != actual.expTrustID)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cESRTrust.expTrustID values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.expTrustID,
                        actual.expTrustID));
            }
            if (expected.FTPAddress != actual.FTPAddress)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cESRTrust.FTPAddress values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.FTPAddress,
                        actual.FTPAddress));
            }
            if (expected.FTPPassword != actual.FTPPassword)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cESRTrust.FTPPassword values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.FTPPassword,
                        actual.FTPPassword));
            }
            if (expected.FTPUsername != actual.FTPUsername)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cESRTrust.FTPUsername values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.FTPUsername,
                        actual.FTPUsername));
            }
            if (expected.ModifiedOn != actual.ModifiedOn)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cESRTrust.ModifiedOn values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.ModifiedOn,
                        actual.ModifiedOn));
            }
            if (expected.PeriodRun != actual.PeriodRun)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cESRTrust.PeriodRun values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.PeriodRun,
                        actual.PeriodRun));
            }
            if (expected.PeriodType != actual.PeriodType)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cESRTrust.PeriodType values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.PeriodType,
                        actual.PeriodType));
            }
            if (expected.RunSequenceNumber != actual.RunSequenceNumber)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cESRTrust.RunSequenceNumber values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.RunSequenceNumber,
                        actual.RunSequenceNumber));
            }
            if (expected.TrustID != actual.TrustID)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cESRTrust.TrustID values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.TrustID,
                        actual.TrustID));
            }
            if (expected.TrustName != actual.TrustName)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cESRTrust.TrustName values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.TrustName,
                        actual.TrustName));
            }
            if (expected.TrustVPD != actual.TrustVPD)
            {
                throw new AssertFailedException(
                    string.Format(
                        "cESRTrust.TrustVPD values are different.{0}Expected: {1}, Actual: {2}",
                        Environment.NewLine,
                        expected.TrustVPD,
                        actual.TrustVPD));
            }
        }
    }
}
