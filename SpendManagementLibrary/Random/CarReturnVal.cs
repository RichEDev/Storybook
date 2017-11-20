using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    /// <summary>
    /// Enumerable type that defines the car return value when deleting
    /// </summary>
    public enum CarReturnVal
    {
        AssignedToCEorUDF = -10,
        Success = 0,
        AssociatedToExpenseItem = 1
    }
}
