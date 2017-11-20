using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public enum EmployeeAreaType
    {
        LicenceChecker = 0,
        TaxChecker,
        MOTChecker,
        InsuranceChecker,
        ServiceChecker,
        BudgetEmployee,
        FloatEmployee,
        EmployeeLoginPage
    }
}
