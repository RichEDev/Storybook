using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Common.Cryptography;
using Microsoft.SqlServer.Server;
using SpendManagementLibrary;
using SpendManagementLibrary.Helpers;
using Utilities.Cryptography;

namespace PasswordEncryptToHash
{
    public class EmployeeConvert : HashConvertBase
    {
        public EmployeeConvert(IEncryptor encryptor, ExpensesCryptography secureData) : base(encryptor, secureData)
        {
        }

        public override void Convert(cAccount account)
        {
            this.Convert(account,
                "select employeeid, [password], passwordMethod, 0 as [Order] from employees where passwordMethod <> 5 and password > ''",
                Global.EmployeeUpdate);
            return;
        }
    }
}
