using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cESRElementObjects
    {
        /// <summary>
        /// Create an empty global ESR Element object
        /// </summary>
        /// <returns></returns>
        public static cESRElement CreateESRElement()
        {
            cESRElementMappings clsMappings = new cESRElementMappings(cGlobalVariables.AccountID, cGlobalVariables.NHSTrustID);
            int tempMappingID = clsMappings.saveESRElement(new cESRElement(0, 4, new List<cESRElementField>(), new List<int>(), cGlobalVariables.NHSTrustID));
            cGlobalVariables.ESRElementID = tempMappingID;
            clsMappings = new cESRElementMappings(cGlobalVariables.AccountID, cGlobalVariables.NHSTrustID);
            cESRElement element = clsMappings.getESRElementByID(tempMappingID);
            return element;
        }

        /// <summary>
        /// Create the ESR Element global static variable with associated attributes
        /// </summary>
        /// <returns></returns>
        public static cESRElement CreateESRElementGlobalVariable()
        {
            cESRElement element = CreateESRElement();
            cESRElementMappings clsMappings = new cESRElementMappings(cGlobalVariables.AccountID, cGlobalVariables.NHSTrustID);
            cESRElementField elementField = new cESRElementField(0, element.ElementID, 7, new Guid("cd5fdd75-4f61-4d91-bd10-0e496fd58259"), 1, Aggregate.None);
            element.Fields.Add(elementField);
            element.Subcats.Add(cGlobalVariables.SubcatID);
            clsMappings.saveESRElement(element);
            return element;
        }

        /// <summary>
        /// Delete the global ESR Element from the database
        /// </summary>
        public static void DeleteESRElement()
        {
            cESRElementMappings clsMappings = new cESRElementMappings(cGlobalVariables.AccountID, cGlobalVariables.NHSTrustID);
            clsMappings.deleteESRElement(cGlobalVariables.ESRElementID);
        }
    }
}
