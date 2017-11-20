using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;

namespace SpendManagementUnitTests.Global_Objects
{
    class cPurchaseOrderProductObject
    {
        public cPurchaseOrderProduct CreatePurchaseOrderProduct()
        {
            cProduct product = cProductObject.CreateProduct();

            cBaseDefinitions clsBaseDefinitions = new cBaseDefinitions(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID, SpendManagementElement.Units);
            cUnit tempUnit = (cUnit)clsBaseDefinitions.GetDefinitionByID(1);

            cDepartment department = cDepartmentObject.CreateDepartment();
            cCostCode costCode = cCostCodeObject.CreateCostCode();
            cProjectCode projectCode = cProjectCodeObject.CreateProjectCode();

            cPurchaseOrderProductCostCentre tempCostCentre = new cPurchaseOrderProductCostCentre(null, department, costCode, projectCode, 100);
            List<cPurchaseOrderProductCostCentre> lstCostCentres = new List<cPurchaseOrderProductCostCentre>() { tempCostCentre };
            cPurchaseOrderProduct tempPurchaseOrderProduct = new cPurchaseOrderProduct(0, product, tempUnit, (decimal)1.44, (decimal)7.32, lstCostCentres);

            return tempPurchaseOrderProduct;
        }
    }
}
