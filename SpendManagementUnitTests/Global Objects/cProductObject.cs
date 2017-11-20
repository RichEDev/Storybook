using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    class cProductObject
    {
        public static cProduct CreateProduct()
        {
            cProducts clsProducts = new cProducts(cGlobalVariables.AccountID);
            int tempProductId = clsProducts.UpdateProduct(new cProduct(0, null, "Unit Test ProductCode", "Unit Test Product Name " + DateTime.Now.ToString(), "Description", null, false, new DateTime(), cGlobalVariables.EmployeeID, null, null), cGlobalVariables.EmployeeID);
            clsProducts = new cProducts(cGlobalVariables.AccountID);
            cProduct tempProduct = clsProducts.GetProductById(tempProductId);

            return tempProduct;
        }
    }
}
