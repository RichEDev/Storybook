using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;

namespace SpendManagementUnitTests.Global_Objects
{
    class cStageObject
    {
        public static cStage FromTemplate()
        {
            cStage signoff = new cStage(0, 1, 1, StageInclusionType.Always, 0, 1, 0, 0, 0, 0, 0, false, false, false, false, DateTime.UtcNow, cGlobalVariables.EmployeeID, DateTime.UtcNow, cGlobalVariables.EmployeeID);
            return signoff;
        }

        public static cStage FromTemplateWithType2(int employeeid)
        {
            cStage signoff = new cStage(0, 2, employeeid, StageInclusionType.Always, 0, 1, 0, 0, 0, 0, 0, false, false, false, false, DateTime.UtcNow, cGlobalVariables.EmployeeID, DateTime.UtcNow, cGlobalVariables.EmployeeID);
            return signoff;
        }
    }
}
