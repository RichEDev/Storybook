namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Receipts;

    using SEL.MessageBrokers;

    using Spend_Management;
    
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using WalletReceipt = SpendManagementApi.Models.Types.WalletReceipt;
    using SpendManagementApi.Utilities;

    /// <summary>
    /// WalletReceiptRepository manages data access for WalletReceipt.
    /// </summary>
    internal class WalletReceiptRepository : BaseRepository<WalletReceipt>
    {
        /// <summary>
        /// The data factory for wallet receipts
        /// </summary>
        private readonly IDataFactory<IWalletReceipt, int> _receipts;
        private readonly IRpcClient _rpcClient;

        /// <summary>
        /// Creates a new WalletReceiptRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        public WalletReceiptRepository(ICurrentUser user) : base(user, x => x.WalletReceiptId, null)
        {
            this._receipts = WebApiApplication.container.GetInstance<IDataFactory<IWalletReceipt, int>>();
            this._rpcClient = WebApiApplication.container.GetInstance<IRpcClient>();
        }

        /// <summary>
        /// Gets a list of all <see cref="WalletReceipt"/> for that user
        /// </summary>
        /// <returns>A list of <see cref="WalletReceipt"/></returns>
        public override IList<WalletReceipt> GetAll()
        {
            IList<IWalletReceipt> receipts = this._receipts.Get();

            if (receipts == null)
            {
                return null;
            }

            return (from receipt in receipts let walletReceipt = new WalletReceipt().From(receipt, this.ActionContext) select walletReceipt).ToList();
        }

        /// <summary>
        /// Get a <see cref="WalletReceipt"/>
        /// </summary>
        /// <param name="id">Id of the <see cref="WalletReceipt"/></param>
        /// <returns>A <see cref="WalletReceipt"/></returns>
        public override WalletReceipt Get(int id)
        {
            IWalletReceipt receipt = this._receipts[id];

            if (receipt == null)
            {
                return null;
            }

            return new WalletReceipt().From(receipt, this.ActionContext);
        }

        /// <summary>
        /// Save a <see cref="WalletReceipt"/>
        /// </summary>
        /// <param name="receiptData">The data of the <see cref="WalletReceipt"/></param>
        /// <returns></returns>
        public WalletReceipt UploadReceipt([FromBody] WalletReceiptRequest receiptData)
        {
            if (receiptData == null)
            {
                throw new ApiException(ApiResources.ApiErrorReceiptUploadInvalid, ApiResources.ApiErrorReceiptUploadMessage);
            }

            var receiptId = 0;

            try
            {
                var newReceipt = new WalletReceipt
                {
                    WalletReceiptId = 0,
                    Extension = receiptData.FileType,
                    ReceiptData = receiptData.Receipt
                };

                var result = this._receipts.Save(newReceipt.To(this.ActionContext));
                receiptId = result.Id;

                if (result.Status == (int)WalletReceiptStatus.New)
                {
                    Task.Run(() => this.SendMessage($"{this.User.AccountID}/{receiptId}"));
                }

                return new WalletReceipt().From(result, this.ActionContext);
            }
            catch (Exception)
            {
                this.DeleteReceipt(receiptId);

                throw new ApiException(ApiResources.ApiErrorReceiptUploadInvalid,
                    ApiResources.ApiErrorReceiptUploadMessage);
            }
        }

        /// <summary>
        /// Delete a <see cref="WalletReceipt"/>
        /// </summary>
        /// <param name="receiptId">Id of the <see cref="WalletReceipt"/></param>
        /// <returns>A numeric response based on the success of the deletion</returns>
        public int DeleteReceipt([FromBody] int receiptId)
        {
            return this._receipts.Delete(receiptId);
        }

        /// <summary>
        /// Sends a message to the RabitMq broker
        /// </summary>
        /// <param name="message">Message to send</param>
        public void SendMessage(string message)
        {
            this._rpcClient.Call(message);
            this._rpcClient.Close();
        }
    }
}