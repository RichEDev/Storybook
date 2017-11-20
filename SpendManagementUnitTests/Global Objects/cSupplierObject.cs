using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cSupplierObject
    {
        public static cSupplier CreateSupplier()
        {
            cSuppliers clsSuppliers = new cSuppliers(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCountries clsCountries = new cCountries(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cAddress supplierAddress = new cAddress(0, "Unit Test Supplier Address" + DateTime.Now.ToShortDateString(), "123 ABC Street", "Off some road", "Some Town", "Some County", "AB12 8CD", clsCountries.getCountryByCode("GB").countryid, "01234 567890", "09876 543210", false, DateTime.Now, cGlobalVariables.EmployeeID, null, null);

            cGlobalVariables.SupplierID = clsSuppliers.UpdateSupplier(new cSupplier(0, cGlobalVariables.SubAccountID, "Unit Test Supplier " + DateTime.Now.ToShortDateString(), null, null, "UTSupCode", supplierAddress, "", 4, null, null, 0, 1, new Dictionary<string, cSupplierContact>(), new SortedList<int, object>(), null, string.Empty, string.Empty, false, false));

            if (cGlobalVariables.SupplierID <= 0)
            {
                Assert.Fail("Failed to create supplier in cSupplierObject.CreateSupplier()");
            }
            clsSuppliers.ResetCache(cGlobalVariables.SupplierID);
            clsSuppliers = new cSuppliers(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cSupplier tempSupplier = clsSuppliers.getSupplierById(cGlobalVariables.SupplierID);

            return tempSupplier;
        }

        public static cSupplier CreateSupplierWithCategoriesAndStatus(bool denyContractAddStatus)
        {
            DeleteSupplier();
            DeleteFinancialStatus();
            DeleteSupplierCategory();
            DeleteSupplierStatus();

            cSuppliers clsSuppliers = new cSuppliers(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cCountries clsCountries = new cCountries(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cAddress newAddress = new cAddress(0, "ADDR_TITLE", "ADDR1", "ADDR2", "TOWN", "COUNTY", "PCODE", clsCountries.getCountryByCode("GB").countryid, "01234 567890", "09876 543210", false, DateTime.Now, cGlobalVariables.EmployeeID, null, null);
                        
            cSupplierStatus newStatus = cSupplierObject.CreateSupplierStatus(denyContractAddStatus);            
            cSupplierCategory newCategory = cSupplierObject.CreateSupplierCategory();            
            cFinancialStatus newFStatus = cSupplierObject.CreateFinancialStatus();
            cCurrencies currencies = new cCurrencies(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);

            cSupplier newsupplier = new cSupplier(0, cGlobalVariables.SubAccountID, "NEWSUPPLIER", newStatus, newCategory, "ABC123", newAddress, "http://www.abc.com", 1, newFStatus, currencies.getCurrencyByAlphaCode("GBP").currencyid, 999.88, 333, new Dictionary<string, cSupplierContact>(), new SortedList<int, object>(), null, string.Empty, string.Empty, false, false);

            cGlobalVariables.SupplierID = clsSuppliers.UpdateSupplier(newsupplier);

            if (cGlobalVariables.SupplierID <= 0)
            {
                Assert.Fail("Failed to create supplier in cSupplierObject.CreateSupplierWithCategoriesAndStatus()");
            }
            clsSuppliers.ResetCache(cGlobalVariables.SupplierID);
            clsSuppliers = new cSuppliers(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cSupplier tempSupplier = clsSuppliers.getSupplierById(cGlobalVariables.SupplierID);

            return tempSupplier;
        }

        public static void DeleteSupplier()
        {
            cSuppliers clsSuppliers = new cSuppliers(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            clsSuppliers.DeleteSupplier(cGlobalVariables.SupplierID);
            cGlobalVariables.SupplierID = 0;
        }

        public static cSupplierStatus CreateSupplierStatus(bool deny_contract_add)
        {
            cBaseDefinitions bdefs = new cBaseDefinitions(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID, SpendManagementElement.SupplierStatus);
            cTables clsTables = new cTables(cGlobalVariables.AccountID);
            cFields clsFields = new cFields(cGlobalVariables.AccountID);

            string bdTableName = "supplier_status";
            cTable bdTable = clsTables.GetTableByName(bdTableName);

            List<cNewGridColumn> dummy = new List<cNewGridColumn>();
            List<cField> bdFields = bdefs.SetBaseDefinitionFields(bdTable.TableID, ref dummy);
            List<cBaseDefinitionValues> bdVals = new List<cBaseDefinitionValues>();
            bdVals.Add(new cBaseDefinitionValues("description", bdTableName, bdTable.TableID.ToString(), "S", false, false, clsFields.GetFieldByTableAndFieldName(bdTable.TableID, "description").FieldID.ToString(), "NEW_STATUS" + DateTime.Now.ToShortDateString()));
            bdVals.Add(new cBaseDefinitionValues("sequence", bdTableName, bdTable.TableID.ToString(), "N", false, false, clsFields.GetFieldByTableAndFieldName(bdTable.TableID, "description").FieldID.ToString(), "99"));
            bdVals.Add(new cBaseDefinitionValues("deny_contract_add", bdTableName, bdTable.TableID.ToString(), "X", false, false, clsFields.GetFieldByTableAndFieldName(bdTable.TableID, "description").FieldID.ToString(), deny_contract_add ? "1" : "0"));
            cGlobalVariables.SupplierStatusID = bdefs.SaveDefinition(-1, bdVals.ToArray());

            return (cSupplierStatus)bdefs.GetDefinitionByID(cGlobalVariables.SupplierStatusID);
        }

        public static cSupplierCategory CreateSupplierCategory()
        {
            cBaseDefinitions bdefs = new cBaseDefinitions(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID, SpendManagementElement.SupplierCategory);
            cTables clsTables = new cTables(cGlobalVariables.AccountID);
            cFields clsFields = new cFields(cGlobalVariables.AccountID);

            string bdTableName = "supplier_categories";
            cTable bdTable = clsTables.GetTableByName(bdTableName);

            List<cNewGridColumn> dummy = new List<cNewGridColumn>();
            List<cField> bdFields = bdefs.SetBaseDefinitionFields(bdTable.TableID, ref dummy);
            List<cBaseDefinitionValues> bdVals = new List<cBaseDefinitionValues>();
            bdVals.Add(new cBaseDefinitionValues("description", bdTableName, bdTable.TableID.ToString(), "S", false, false, clsFields.GetFieldByTableAndFieldName(bdTable.TableID, "description").FieldID.ToString(), "NEW_CATEGORY" + DateTime.Now.ToShortDateString()));

            cGlobalVariables.SupplierCategoryID = bdefs.SaveDefinition(-1, bdVals.ToArray());

            return (cSupplierCategory)bdefs.GetDefinitionByID(cGlobalVariables.SupplierCategoryID);
        }

        public static cFinancialStatus CreateFinancialStatus()
        {
            cBaseDefinitions bdefs = new cBaseDefinitions(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID, SpendManagementElement.FinancialStatus);
            cTables clsTables = new cTables(cGlobalVariables.AccountID);
            cFields clsFields = new cFields(cGlobalVariables.AccountID);

            string bdTableName = "financial_status";
            cTable bdTable = clsTables.GetTableByName(bdTableName);

            List<cNewGridColumn> dummy = new List<cNewGridColumn>();
            List<cField> bdFields = bdefs.SetBaseDefinitionFields(bdTable.TableID, ref dummy);
            List<cBaseDefinitionValues> bdVals = new List<cBaseDefinitionValues>();
            bdVals.Add(new cBaseDefinitionValues("description", bdTableName, bdTable.TableID.ToString(), "S", false, false, clsFields.GetFieldByTableAndFieldName(bdTable.TableID, "description").FieldID.ToString(), "NEW_FINANCIAL_STATUS" + DateTime.Now.ToShortDateString()));

            cGlobalVariables.FinancialStatusID = bdefs.SaveDefinition(-1, bdVals.ToArray());

            return (cFinancialStatus)bdefs.GetDefinitionByID(cGlobalVariables.FinancialStatusID);
        }

        public static void DeleteSupplierCategory()
        {
            cBaseDefinitions bdefs = new cBaseDefinitions(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID, SpendManagementElement.SupplierCategory);
            bdefs.DeleteDefinition(cGlobalVariables.SupplierCategoryID);
            cGlobalVariables.SupplierCategoryID = 0;
        }

        public static void DeleteSupplierStatus()
        {
            cBaseDefinitions bdefs = new cBaseDefinitions(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID, SpendManagementElement.SupplierStatus);
            bdefs.DeleteDefinition(cGlobalVariables.SupplierStatusID);
            cGlobalVariables.SupplierStatusID = 0;
        }

        public static void DeleteFinancialStatus()
        {
            cBaseDefinitions bdefs = new cBaseDefinitions(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID, SpendManagementElement.FinancialStatus);
            bdefs.DeleteDefinition(cGlobalVariables.FinancialStatusID);
            cGlobalVariables.FinancialStatusID = 0;
        }
    }
}
