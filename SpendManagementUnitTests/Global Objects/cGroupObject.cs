using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    class cGroupObject
    {
        public static cGroup FromTemplate()
        {
            cGroup signoff = new cGroup(cGlobalVariables.AccountID, 0, "Unit Test Group " + DateTime.UtcNow.ToString(), "Unit Tests", false, string.Empty, DateTime.UtcNow, cGlobalVariables.EmployeeID, DateTime.UtcNow, cGlobalVariables.EmployeeID, new SortedList<int, cStage>());
            return signoff;
        }

        public static int CreateID()
        {
            cGroups groups = new cGroups(cGlobalVariables.AccountID);
            cGroup group = FromTemplate();
            int groupID = groups.addGroup(group.groupname, group.description, false, cGlobalVariables.EmployeeID);

            return groupID;
        }

        public static cGroup CreateObject()
        {
            int newGroupID = CreateID();
            cGroups groups = new cGroups(cGlobalVariables.AccountID);            
            cGroup group = groups.GetGroupById(newGroupID);

            return group;
        }

        public static cGroup CreateObjectWithSteps()
        {
            cGroups groups = new cGroups(cGlobalVariables.AccountID);
            cGroup group = CreateObject();
            cStage stage = cStageObject.FromTemplate();

            groups.addStage(group.groupid, stage.signofftype, stage.relid, (int)stage.include, stage.amount, stage.notify, stage.onholiday, stage.holidaytype, stage.holidayid, stage.includeid, stage.claimantmail, stage.singlesignoff, stage.sendmail, stage.displaydeclaration, cGlobalVariables.EmployeeID, stage.signoffid, false);

            return group;
        }
    }
}
