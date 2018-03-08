namespace SpendManagementLibrary.Receipts
{
    using System;
    using System.Data;
    using System.IO;

    using BusinessLogic.Receipts;

    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// Defines a <see cref="ProcessedReceipt"/> and all its members
    /// </summary>
    public class ProcessedReceipts
    {
        /// <summary>
        /// The id of the account
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Constructor for the <see cref="ProcessedReceipt"/>
        /// </summary>
        /// <param name="accountId">The id of the account</param>
        public ProcessedReceipts(int accountId)
        {
            this.AccountId = accountId;
        }

        /// <summary>
        /// Save the data found on the <see cref="WalletReceipt"/>
        /// </summary>
        /// <param name="processedReceipt">The <see cref="ProcessedReceipt"/> to be saved</param>
        /// <returns>A return val based on the success of the save</returns>
        public int PostReceiptData(ProcessedReceipt processedReceipt)
        {
            using (var databaseConnection =
                new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                databaseConnection.AddWithValue("@WalletReceiptId ", processedReceipt.WalletReceiptId);

                if (processedReceipt.Total == null)
                {
                    databaseConnection.AddWithValue("@Total", DBNull.Value);
                }
                else
                {
                    databaseConnection.AddWithValue("@Total", processedReceipt.Total);
                }

                if (processedReceipt.Date == null)
                {
                    databaseConnection.AddWithValue("@Date", DBNull.Value);
                }
                else
                {
                    databaseConnection.AddWithValue("@Date", processedReceipt.Date);
                }

                if (string.IsNullOrWhiteSpace(processedReceipt.Merchant))
                {
                    databaseConnection.AddWithValue("@Merchant", DBNull.Value);
                }
                else
                {
                    databaseConnection.AddWithValue("@Merchant", processedReceipt.Merchant);
                }

                databaseConnection.AddReturn("@ReturnValue", SqlDbType.Int);
                databaseConnection.ExecuteProc("SaveProcessedReceiptData");
                return databaseConnection.GetReturnValue<int>("@ReturnValue");
            }
        }

        /// <summary>
        /// Gets a <see cref="ProcessedReceipt"/>
        /// </summary>
        /// <param name="walletReceiptId">Id of the <see cref="WalletReceipt"/></param>
        /// <returns>A <see cref="ProcessedReceipt"/></returns>
        public ProcessedReceipt GetProcessedReceipt(int walletReceiptId)
        {
            var processedReceipt = new ProcessedReceipt
            {
                WalletReceiptId = walletReceiptId,
                AccountId = this.AccountId
            };

            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                databaseConnection.AddWithValue("@WalletReceiptId ", walletReceiptId);

                using (var reader = databaseConnection.GetReader(
                    "GetProcessedReceipt", CommandType.StoredProcedure))
                {
                    databaseConnection.sqlexecute.Parameters.Clear();
                    while (reader.Read())
                    {
                        processedReceipt.WalletReceiptId = reader.GetInt32(reader.GetOrdinal("WalletReceiptId"));

                        if (processedReceipt.WalletReceiptId == -1)
                        {
                            continue;
                        }

                        processedReceipt.FileExtension = reader.GetString(reader.GetOrdinal("FileExtension"));
                        processedReceipt.Status = reader.GetInt32(reader.GetOrdinal("Status"));
                        processedReceipt.ReceiptData = this.GetReceiptFromCloud(walletReceiptId,processedReceipt.FileExtension);
                    }

                    reader.Close();
                }

                return processedReceipt;
            }
        }

        /// <summary>
        /// Set the wallet receipt status from In progress to new
        /// </summary>
        /// <param name="walletReceiptId">The id of the wallet receipt</param>
        public void ResetReceipt(int walletReceiptId)
        {
            using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                databaseConnection.AddWithValue("@WalletReceiptId ", walletReceiptId);
                databaseConnection.ExecuteProc("ResetWalletReceipt");
                databaseConnection.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// Gets the receipt data from the cloud
        /// </summary>
        /// <param name="receiptId">The id of the receipt</param>
        /// <param name="fileExtension">The file extension of the receipt</param>
        /// <returns>The data of the receipt in base 64 string format</returns>
        private string GetReceiptFromCloud(int receiptId, string fileExtension)
        {
            string output;

            using (var stream = new MemoryStream())
            {
                var receiptFilename = receiptId + "." + fileExtension;
                SELCloud.SELCloud.Instance.GetObject(this.AccountId.ToString(), @"Receipts\Wallet" + receiptFilename, stream);
                output = Convert.ToBase64String(stream.GetBuffer(), Base64FormattingOptions.None);

            }

            return output;
        }
    }
}



