using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;
using System.Reflection;

namespace SpendManagementUnitTests
{
    public class cBaseDefinitionObject
    {
        /// <summary>
        /// This creates a base definition for the specific type passed in
        /// </summary>
        /// <param name="TableID"></param>
        /// <param name="element"></param>
        public static cBaseDefinition CreateBaseDefinition(SpendManagementElement element, ref cBaseDefinition expected)
        {
            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID, element);

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

                            defValues.Add(new cBaseDefinitionValues(field.FieldName, field.Table.TableName, field.TableID.ToString(), field.FieldType, field.GenList, field.ValueList, field.FieldID.ToString(), dataValue.ToString()));
                        }
                    }
                }
            }

            #endregion

            int ID = clsBaseDefs.SaveDefinition(-1, defValues.ToArray());
            cGlobalVariables.BaseDefinitionID = ID;
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
            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID, element);
            cBaseDefinition nullDef = null;
            cBaseDefinition baseDef = CreateBaseDefinition(element, ref nullDef);
            List<cNewGridColumn> lstColumns = new List<cNewGridColumn>();
            Guid TableID = GetBaseDefTableID(element);

            //Get the fields associated with the base definition 
            List<cField> lstFields = clsBaseDefs.SetBaseDefinitionFields(TableID, ref lstColumns);


            baseDef.Description ="Edited" + DateTime.Now.Ticks.ToString().Substring(0, 14);
            baseDef.ModifiedOn = DateTime.UtcNow;
            baseDef.ModifiedBy = cGlobalVariables.EmployeeID;

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

                            defValues.Add(new cBaseDefinitionValues(field.FieldName, field.Table.TableName, field.TableID.ToString(), field.FieldType, field.GenList, field.ValueList, field.FieldID.ToString(), dataValue.ToString()));
                        }
                    }
                }
            }

            #endregion

            int ID = clsBaseDefs.SaveDefinition(baseDef.ID, defValues.ToArray());
            cGlobalVariables.BaseDefinitionID = ID;

            return baseDef;
        }

        /// <summary>
        /// Delete the base definition from the database
        /// </summary>
        /// <param name="element"></param>
        public static void DeleteBaseDefinition(SpendManagementElement element)
        {
            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID, element);
            clsBaseDefs.DeleteDefinition(cGlobalVariables.BaseDefinitionID);
        }

        /// <summary>
        /// Archive the base definition
        /// </summary>
        /// <param name="element"></param>
        public static void ArchiveBaseDefinition(SpendManagementElement element)
        {
            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID, element);
            clsBaseDefs.ArchiveDefinition(cGlobalVariables.BaseDefinitionID);
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
                    return new cContractStatus(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false, false);
                case SpendManagementElement.ContractCategories:
                    return new cContractCategory(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false);
                case SpendManagementElement.ContractTypes:
                    return new cContractType(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false);
                case SpendManagementElement.InvoiceFrequencyTypes:
                    return new cInvoiceFrequencyType(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false, 2);
                case SpendManagementElement.InvoiceStatus:
                    return new cInvoiceStatus(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false, false);
                case SpendManagementElement.LicenceRenewalTypes:
                    return new cLicenceRenewalType(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false);
                case SpendManagementElement.InflatorMetrics:
                    return new cInflatorMetric(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false, decimal.Parse("100.00"), true);
                case SpendManagementElement.TermTypes:
                    return new cTermType(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false);
                case SpendManagementElement.FinancialStatus:
                    return new cFinancialStatus(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false);
                case SpendManagementElement.TaskTypes:
                    return new cTaskType(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false);
                case SpendManagementElement.Units:
                    return new cUnit(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false);
                case SpendManagementElement.ProductCategories:
                    return new cProductCategory(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false);
                case SpendManagementElement.SupplierStatus:
                    return new cSupplierStatus(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false, 1, true);
                case SpendManagementElement.SupplierCategory:
                    return new cSupplierCategory(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false);
                case SpendManagementElement.ProductLicenceTypes:
                    return new cProductLicenceType(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false);
                case SpendManagementElement.SalesTax:
                    return new cSalesTax(0, strDesc, DateTime.Now, cGlobalVariables.EmployeeID, null, null, false, decimal.Parse("100.00"));
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
                case SpendManagementElement.ContractCategories :
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
}
