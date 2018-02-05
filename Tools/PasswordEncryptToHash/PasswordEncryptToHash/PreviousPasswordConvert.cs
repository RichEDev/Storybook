using System;
using System.Collections.Generic;
using System.Data;
using Common.Cryptography;
using Microsoft.SqlServer.Server;
using SpendManagementLibrary;
using SpendManagementLibrary.Helpers;
using Utilities.Cryptography;

namespace PasswordEncryptToHash
{
    internal class PreviousPasswordConvert: HashConvertBase
    {

        public PreviousPasswordConvert(IEncryptor encryptor, ExpensesCryptography secureData) : base(encryptor,secureData)
        {
        }

        public override void Convert(cAccount account)
        {
            this.Convert(account,
                "select employeeid, [password], passwordMethod, [Order] from previouspasswords where passwordMethod <> 5 and password > ''",
                Global.PreviousPasswordUpdate);
            return;
        }
    }
}