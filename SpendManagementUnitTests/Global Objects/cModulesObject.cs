using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cModulesObject
    {
        //public static cModules CreateModulesObject()
        //{
        //    cModules modules = new cModules();
        //    modules.Modules = cModulesObject.CreateModuleListWithoutElements();
        //}

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
}
