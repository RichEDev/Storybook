using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcBaseDefinitions
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcBaseDefinitions : System.Web.Services.WebService
    {
        /// <summary>
        /// Get the base definiton record information
        /// </summary>
        /// <param name="element"></param>
        /// <param name="ID"></param>
        /// <param name="fieldIDs"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod()]
        public cBaseDefinitionValues[] getBaseDefinitionRecord(SpendManagementElement element, int ID, Guid[] fieldIDs)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cBaseDefinitions clsBaseDefs = new cBaseDefinitions(currentUser.AccountID, currentUser.CurrentSubAccountId, element);

            cBaseDefinitionValues[] retValues = clsBaseDefs.GetBaseDefinitionRecord(ID, fieldIDs);

            return retValues;
        }

        /// <summary>
        /// Save the base definition to the database
        /// </summary>
        /// <param name="baseTableID"></param>
        /// <param name="element"></param>
        /// <param name="ID"></param>
        /// <param name="defValues"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod()]
        public int saveDefinition(SpendManagementElement element, int ID, cBaseDefinitionValues[] defValues)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            AccessRoleType requiredPermission = (ID == -1 ? AccessRoleType.Add : AccessRoleType.Edit);

            int RecordID = 0;

            if (currentUser.CheckAccessRole(requiredPermission, element, true))
            {
                cBaseDefinitions clsBaseDefs = new cBaseDefinitions(currentUser.AccountID, currentUser.CurrentSubAccountId, element);

                RecordID = clsBaseDefs.SaveDefinition(ID, defValues);
            }

            return RecordID;
        }

        /// <summary>
        /// Delete the base definiton from the database
        /// </summary>
        /// <param name="element"></param>
        /// <param name="ID"></param>
        [WebMethod(EnableSession = true)]
        [ScriptMethod()]
        public int deleteDefinition(SpendManagementElement element, int ID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            if (currentUser.CheckAccessRole(AccessRoleType.Delete, element, true))
            {
                cBaseDefinitions clsBaseDef = new cBaseDefinitions(currentUser.AccountID, currentUser.CurrentSubAccountId, element);
                return clsBaseDef.DeleteDefinition(ID);
            }

            return 0;
        }

        /// <summary>
        /// Archive the base definition
        /// </summary>
        /// <param name="element"></param>
        /// <param name="ID"></param>
        [WebMethod(EnableSession = true)]
        [ScriptMethod()]
        public void ArchiveDefinition(SpendManagementElement element, int ID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            cBaseDefinitions clsBaseDef = new cBaseDefinitions(currentUser.AccountID, currentUser.CurrentSubAccountId, element);
            clsBaseDef.ArchiveDefinition(ID);
        }

        /// <summary>
        /// Displays a grid view of current base definitions
        /// </summary>
        /// <param name="baseTableId">Table ID for the base definition table</param>
        /// <param name="currentElement">Base Definition table enumerator to display definitions for</param>
        /// <param name="columns">Grid column collection</param>
        /// <param name="gridID">Unique identity name for the grid</param>
        /// <returns>HTML to render grid</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod()]
        public static string[] displayBaseDefinitions(Guid baseTableId, SpendManagementElement currentElement, List<cNewGridColumn> columns, string gridID)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            cTables tables = new cTables(curUser.AccountID);
            cTable baseTable = tables.GetTableByID(baseTableId);
            cFields fields = new cFields(curUser.AccountID);
            cField idfield = baseTable.GetPrimaryKey();
            string KeyField = baseTable.GetKeyField().FieldName;

            List<string> retVals = new List<string>();
            retVals.Add(gridID);
            
            if (currentElement != SpendManagementElement.None)
            {
                cGridNew bdGrid = new cGridNew(curUser.AccountID, curUser.EmployeeID, gridID, baseTable, columns);

                if (currentElement != SpendManagementElement.SubAccounts)
                {
                    bdGrid.addFilter(fields.GetBy(baseTable.TableID, "subAccountId"), ConditionType.Equals, new object[] { curUser.CurrentSubAccountId }, new object[] { }, ConditionJoiner.None);
                }
                bdGrid.getColumnByID(idfield.FieldID).hidden = true;
                bdGrid.EmptyText = "No definitions currently defined";
                bdGrid.enabledeleting = curUser.CheckAccessRole(AccessRoleType.Delete, currentElement, false);
                if (bdGrid.enabledeleting)
                {
                    bdGrid.deletelink = "javascript:baseDefObject.deleteDefinition({" + idfield.FieldName + "});";
                }
                else
                {
                    bdGrid.deletelink = "javascript:alert('permission denied');";
                }
                bdGrid.enableupdating = curUser.CheckAccessRole(AccessRoleType.Edit, currentElement, false);
                if (bdGrid.enableupdating)
                {
                    bdGrid.editlink = "javascript:baseDefObject.editDefinition({" + idfield.FieldName + "});";
                }
                else
                {
                    bdGrid.editlink = "javascript:alert('permission denied');";
                }
                bdGrid.enablearchiving = true;
                bdGrid.archivelink = "javascript:baseDefObject.archiveDefinition({" + idfield.FieldName + "});";
                bdGrid.ArchiveField = "archived";
                bdGrid.getColumnByName("archived").hidden = true;

                bdGrid.KeyField = KeyField;
                bdGrid.CssClass = "datatbl";

                retVals.AddRange(bdGrid.generateGrid());
            }

            return retVals.ToArray();
        }
    }
}
