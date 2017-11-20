using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    class cPurchaseOrderObject
    {
        public cPurchaseOrder CreatePurchaseOrder()
        {
            Dictionary<int, cPurchaseOrderProduct> lstPurchaseOrderProducts = new Dictionary<int, cPurchaseOrderProduct>();
            cPurchaseOrderProduct clsPurchaseOrderProduct = new cPurchaseOrderProductObject().CreatePurchaseOrderProduct();
            lstPurchaseOrderProducts.Add(clsPurchaseOrderProduct.PurchaseOrderProductID,clsPurchaseOrderProduct);
            cPurchaseOrder tempPurchaseOrder = new cPurchaseOrder(0, cSupplierObject.CreateSupplier(), "Unit Test Purchase Order " + DateTime.Now.ToString(), null, null, PurchaseOrderType.Individual, null, null, null, "Unit Test Comments", DateTime.Now, cGlobalVariables.EmployeeID, null, null, PurchaseOrderState.Unsubmitted, String.Empty, null, lstPurchaseOrderProducts, cCountryObject.CreateCountry(), cCurrencyObject.CreateCurrency(), null, null, null, null);

            return tempPurchaseOrder;
        }
    }
}
