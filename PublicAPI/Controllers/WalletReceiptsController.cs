namespace PublicAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Identity;
    using BusinessLogic.Receipts;

    using PublicAPI.Common.Actions;
    using PublicAPI.DTO;
    using PublicAPI.Security.Filters;

    using SEL.MessageBrokers;

    /// <summary>
    /// Controller enabling actions to be executed on <see cref="IWalletReceipt"/> using the <see cref="WalletReceiptDto"/>.
    /// </summary>
    [JwtAuthentication]
    public class WalletReceiptsController : ApiController, ICrud<WalletReceiptDto, int>
    {
        private readonly Lazy<IDataFactory<IWalletReceipt, int>> _walletReceipts = new Lazy<IDataFactory<IWalletReceipt, int>>(() => WebApiApplication.container.GetInstance<IDataFactory<IWalletReceipt, int>>());
        private readonly Lazy<IRpcClient> _rpcClient = new Lazy<IRpcClient>(() => WebApiApplication.container.GetInstance<IRpcClient>());
        private readonly Lazy<IIdentityProvider> _identity = new Lazy<IIdentityProvider>(() => WebApiApplication.container.GetInstance<IIdentityProvider>());

        /// <summary>
        /// Controller action to get all available instances of <see cref="WalletReceiptDto"/>.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/WalletReceipts">https://api.hostname/WalletReceipts</a>
        /// </remarks>
        /// <returns>A <see cref="IEnumerable{T}"/> containing all instances of <see cref="WalletReceiptDto"/>.</returns>
        public IHttpActionResult Get()
        {
            IEnumerable<IWalletReceipt> walletReceipts = this._walletReceipts.Value.Get();
            IEnumerable<WalletReceiptDto> walletReceiptsDtoCollection = MapObjects.Map<IEnumerable<WalletReceiptDto>>(walletReceipts);

            return this.Json(walletReceiptsDtoCollection);
        }

        /// <summary>
        /// Controller action to get a specific <see cref="WalletReceiptDto"/> with a matching <see cref="int"/> Id.
        /// </summary>
        /// <remarks>
        /// GET: <a href="https://api.hostname/WalletReceipts/{id}">https://api.hostname/WalletReceipts/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An instance of <see cref="WalletReceiptDto"/> with a matching id, of nothing is not matched.</returns>
        public IHttpActionResult Get(int id)
        {
            WalletReceiptDto walletReceipt = MapObjects.Map<WalletReceiptDto>(this._walletReceipts.Value[id]);

            return this.Json(walletReceipt);
        }

        /// <summary>
        /// Controller action to add (or update) an instance of <see cref="WalletReceiptDto"/>.
        /// </summary>
        /// <remarks>
        /// POST: <a href="https://api.hostname/WalletReceipts">https://api.hostname/WalletReceipts</a>
        ///  Body: <see cref="WalletReceiptDto"/>
        /// </remarks>
        /// <param name="value">The <see cref="WalletReceiptDto"/> to update with it's associated values.</param>
        /// <returns>An instance of <see cref="WalletReceiptDto"/> with updated properties post add (or update).</returns>
        public IHttpActionResult Post([FromBody]WalletReceiptDto value)
        {
            var user = this._identity.Value.GetUserIdentity();

            IWalletReceipt walletReceipt = MapObjects.Map<WalletReceipt>(value);
            walletReceipt.CreatedBy = user.EmployeeId;
            walletReceipt = this._walletReceipts.Value.Save(walletReceipt);

            //Fire and forget so the request isn't left hanging while waiting for a server
            if (walletReceipt.Status == (int)WalletReceiptStatus.New)
            {
                Task.Run(() => this.SendMessage($"{user.AccountId}/{walletReceipt.Id}"));
            }

            value = MapObjects.Map<WalletReceiptDto>(walletReceipt);

            return this.Json(value);
        }

        /// <summary>
        /// Controller action to add (or update) an instance of <see cref="WalletReceiptDto"/>.
        /// </summary>
        /// <remarks>
        /// PUT: <a href="https://api.hostname/WalletReceipts">https://api.hostname/WalletReceipts</a>
        ///  Body: <see cref="WalletReceiptDto"/>
        /// </remarks>
        /// <param name="value">The <see cref="WalletReceiptDto"/> to update with it's associated values.</param>
        /// <returns>An instance of <see cref="WalletReceiptDto"/> with updated properties post add (or update).</returns>
        public IHttpActionResult Put([FromBody]WalletReceiptDto value)
        {
            var user = this._identity.Value.GetUserIdentity();

            IWalletReceipt walletReceipt = MapObjects.Map<WalletReceipt>(value);
            walletReceipt.CreatedBy = user.EmployeeId;
            walletReceipt = this._walletReceipts.Value.Save(walletReceipt);

            //Fire and forget so the request isn't left hanging while waiting for a server
            if (walletReceipt.Status == (int) WalletReceiptStatus.New)
            {
                Task.Run(() => this.SendMessage($"{user.AccountId}/{walletReceipt.Id}"));
            }

            value = MapObjects.Map<WalletReceiptDto>(walletReceipt);

            return this.Json(value);
        }

        /// <summary>
        /// Controller action to delete a specific <see cref="WalletReceiptDto"/> with a matching <see cref="int"/> Id.
        /// </summary>
        /// <remarks>
        /// DELETE: <a href="https://api.hostname/WalletReceipts/{id}">https://api.hostname/WalletReceipts/{id}</a>
        /// </remarks>
        /// <param name="id">The id to match on</param>
        /// <returns>An <see cref="int"/> code with the response of the delete.</returns>
        public IHttpActionResult Delete(int id)
        {
            return this.Json(this._walletReceipts.Value.Delete(id));
        }

        /// <summary>
        /// Sends a message to the RabitMq broker
        /// </summary>
        /// <param name="message">Message to send</param>
        private void SendMessage(string message)
        {
            this._rpcClient.Value.Call(message);
            this._rpcClient.Value.Close();
        }
    }
}