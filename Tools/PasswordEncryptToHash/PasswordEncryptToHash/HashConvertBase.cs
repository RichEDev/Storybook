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
    public abstract class HashConvertBase
    {
        protected IEncryptor _encryptor;
        protected ExpensesCryptography _secureData;

        protected HashConvertBase(IEncryptor encryptor, ExpensesCryptography secureData)
        {
            this._encryptor = encryptor;
            this._secureData = secureData;
        }

        public abstract void Convert(cAccount account);

        public void Convert(cAccount account, string select, string update)
        {
            List<SqlDataRecord> employeePasswords = new List<SqlDataRecord>();
            SqlDataRecord row;
            var typelogitem = this.CreateSqlRecords();


            using (var connection = new DatabaseConnection(account.ConnectionString))
            {
                var idx = 1;
                using (var reader = connection.GetReader(select))
                {
                    if (reader == null)
                    {
                        return;
                    }

                    var employeeId = 0;
                    var encryptedPassword = string.Empty;
                    while (reader.Read())
                    {
                        try
                        {
                            Logger.Processing();
                            employeeId = reader.GetInt32(0);
                            encryptedPassword = reader.GetString(1);
                            
                            var decryptedPassword = this._secureData.DecryptString(encryptedPassword);
                            var hashPassword = this._encryptor.Encrypt(decryptedPassword);
                            row = new SqlDataRecord(typelogitem);
                            row.SetInt32(0, employeeId);
                            row.SetInt32(1, 0);
                            row.SetString(2, hashPassword);
                            employeePasswords.Add(row);
                        }
                        catch (Exception e)
                        {
                            // If the current password cannot be Decrypted, then store the "encrypted" password as a hash.
                            row = new SqlDataRecord(typelogitem);
                            row.SetInt32(0, employeeId);
                            row.SetInt32(1, 0);
                            row.SetString(2, this._encryptor.Encrypt(encryptedPassword));
                            employeePasswords.Add(row);
                        }
                        idx++;    
                    }
                    Logger.ProcessingEnd();
                    if (employeePasswords.Count == 0)
                    {
                        Logger.WriteLine("Zero passwords found to update.");
                        return;
                    }

                    connection.sqlexecute.Parameters.Clear();
                    connection.sqlexecute.Parameters.Add("@logitem", SqlDbType.Structured);
                    connection.sqlexecute.Parameters["@logitem"].Value = employeePasswords;
                    connection.sqlexecute.Parameters["@logitem"].TypeName = "dbo.logitem";
                    var result = connection.ExecuteSQL(update);
                    Logger.WriteLine($"Read {idx} employees, updated {result}");
                }
            }
        }


        protected SqlMetaData[] CreateSqlRecords()
        {
            
            SqlMetaData[] typelogitem =
            {
                new SqlMetaData("reasonID", System.Data.SqlDbType.Int),
                new SqlMetaData("elementID", System.Data.SqlDbType.Int),
                new SqlMetaData("logItem", SqlDbType.NVarChar,4000) 
            };
            return typelogitem;
        }
    }
}