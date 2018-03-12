namespace SQLDataAccess.Receipts
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;

    using BusinessLogic;
    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Identity;
    using BusinessLogic.Images;
    using BusinessLogic.ISELCloud;
    using BusinessLogic.Receipts;

    using Common.Logging;

    using SQLDataAccess.Accounts;
    using SQLDataAccess.Elements;

    /// <summary>
    /// Implements methods to retrieve and create instances of <see cref="IWalletReceipt"/> in <see cref="IDataConnection{T}"/>
    /// </summary>
    public class SqlWalletReceiptsFactory : IDataFactory<IWalletReceipt, int>
    {
        /// <summary>
        /// An instance of the <see cref="IdentityProvider"/> for obtaining the currently logged in user.
        /// </summary>
        private readonly IdentityProvider _identityProvider;

        /// <summary>
        /// An instance of <see cref="ICustomerDataConnection{T}"/> to use for accessing <see cref="IDataConnection{T}"/>
        /// </summary>
        private readonly ICustomerDataConnection<SqlParameter> _customerDataConnection;

        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="SqlWalletReceiptsFactory"/> diagnostics and information.
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// Backing instance of the licenced element factory.
        /// </summary>
        private readonly SqlAccountWithElementsFactory _sqlAccountWithElementsFactory;

        /// <summary>
        /// Backing instance of element factory.
        /// </summary>
        private readonly SqlElementFactory _sqlElementFactory;

        /// <summary>
        /// An instance of <see cref="IImageConversion"/> for converting <see cref="WalletReceipt"/> to a different file type.
        /// </summary>
        private readonly IImageConversion _imageConversion;

        /// <summary>
        /// An instance of <see cref="IImageManipulation"/> for manipulating <see cref="WalletReceipt"/> image data.
        /// </summary>
        private readonly IImageManipulation _imageManipulation;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlWalletReceiptsFactory"/> class. 
        /// </summary>
        /// <param name="customerDataConnection">An instance of <see cref="ICustomerDataConnection{SqlParameter}"/> to use when accessing <see cref="IDataConnection{SqlParameter}"/></param>
        /// <param name="identityProvider">An instance of the <see cref="IdentityProvider"/> for obtaining the currently logged in user.</param>
        /// <param name="logger">An instance of <see cref="ILog"/> to use when logging information.</param>
        /// <param name="sqlAccountWithElementsFactory">An instance of <see cref="SqlAccountWithElementsFactory"/></param>
        /// <param name="sqlElementFactory">An instance of <see cref="SqlElementFactory"/></param>
        /// <param name="imageConversion">An instance of <see cref="IImageConversion"/> to convert image types</param>
        /// <param name="imageManipulation">An instance of <see cref="IImageManipulation"/> to manipulate image type</param>
        /// <exception cref="ArgumentNullException"><paramref name="customerDataConnection"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="identityProvider"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="sqlAccountWithElementsFactory"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="sqlElementFactory"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="imageConversion"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="imageManipulation"/> is <see langword="null" />.</exception>
        public SqlWalletReceiptsFactory(ICustomerDataConnection<SqlParameter> customerDataConnection, IdentityProvider identityProvider, ILog logger, SqlAccountWithElementsFactory sqlAccountWithElementsFactory, SqlElementFactory sqlElementFactory, IImageConversion imageConversion, IImageManipulation imageManipulation)
        {
            Guard.ThrowIfNull(customerDataConnection, nameof(customerDataConnection));
            Guard.ThrowIfNull(identityProvider, nameof(identityProvider));
            Guard.ThrowIfNull(logger, nameof(logger));
            Guard.ThrowIfNull(sqlAccountWithElementsFactory, nameof(sqlAccountWithElementsFactory));
            Guard.ThrowIfNull(sqlElementFactory, nameof(sqlElementFactory));
            Guard.ThrowIfNull(imageConversion, nameof(imageConversion));
            Guard.ThrowIfNull(imageManipulation, nameof(imageManipulation));

            this._customerDataConnection = customerDataConnection;
            this._identityProvider = identityProvider;
            this._logger = logger;
            this._sqlAccountWithElementsFactory = sqlAccountWithElementsFactory;
            this._sqlElementFactory = sqlElementFactory;
            this._imageConversion = imageConversion;
            this._imageManipulation = imageManipulation;
        }

        /// <summary>
        /// Gets an instance of <see cref="IWalletReceipt"/> with a matching ID from memory if possible, if not it will search cache for an entry and finally <see cref="IDataConnection{SqlParameter}"/>
        /// </summary>
        /// <param name="id">The ID of the <see cref="IWalletReceipt"/> you want to retrieve</param>
        /// <returns>The required <see cref="IWalletReceipt"/> or <see langword="null" /> if it cannot be found</returns>
        public IWalletReceipt this[int id]
        {
            get
            {
                IWalletReceipt receipt = null;
                var receipts = this.GetFromDatabase(id);
                if (receipts != null && receipts.Count > 0)
                {
                    receipt = receipts[0];
                }

                return receipt;
            }
        }

        /// <summary>
        /// Adds or updates the specified instance of <see cref="IWalletReceipt"/> to <see cref="IDataConnection{SqlParameter}"/>, <see cref="RepositoryBase{T,TK}"/>
        /// </summary>
        /// <param name="receipt">The <see cref="IWalletReceipt"/> to add or update.</param>
        /// <returns>The unique identifier for the <see cref="IWalletReceipt"/>.</returns>
        public IWalletReceipt Save(IWalletReceipt receipt)
        {
            if (receipt == null)
            {
                return null;
            }

            UserIdentity identity = this._identityProvider.GetUserIdentity();
            receipt.CreatedBy = identity.EmployeeId;
            receipt.CreatedOn = DateTime.Now;

            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug("SqlWalletReceiptsFactory.Add(receipt) called.");
                this._logger.Debug(receipt);
            }

            if (receipt.FileExtension == "jpg" || receipt.FileExtension == "jpeg")
            {
                receipt.ReceiptData = this._imageManipulation.RemoveExifData(new MemoryStream(Convert.FromBase64String(receipt.ReceiptData)));
            }

            var conversion = this._imageConversion.AttemptConversion(Convert.FromBase64String(receipt.ReceiptData));

            if (!conversion.ConvertedFile.Equals(conversion.SourceFile))
            {
                receipt.FileExtension = "jpg";
            }

            var element = this._sqlElementFactory[(int) ModuleElements.ReceiptReader];

            var elements = this._sqlAccountWithElementsFactory[identity.AccountId].LicencedElements.ToList();

            receipt.Status = elements.Find(licencedElement => licencedElement.Id == element.Id) == null ? (int)WalletReceiptStatus.Skip : (int)WalletReceiptStatus.New;

            this._customerDataConnection.Parameters.Clear();

            this._customerDataConnection.Parameters.Add(new SqlParameter("@fileExtension", SqlDbType.NVarChar) { Value = receipt.FileExtension });

            this._customerDataConnection.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = receipt.CreatedBy });

            this._customerDataConnection.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int) { Value = receipt.Status });
             
            this._customerDataConnection.Parameters.Add(new SqlParameter("@returnvalue", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue });

            this._customerDataConnection.Parameters.AddAuditing(identity);

            this._customerDataConnection.Parameters.ReturnValue = "@returnvalue";

            receipt.Id = this._customerDataConnection.ExecuteProc<int>("SaveWalletReceipt");

            this._customerDataConnection.Parameters.Clear();

            if (this._logger.IsDebugEnabled)
            {
                this._logger.DebugFormat($"Add completed with id {receipt.Id}.");
            }

            new CloudWalletReceipt().Save($"Wallet{receipt.Id}.{receipt.FileExtension}", conversion, identity.AccountId.ToString());

            return receipt;
        }

        /// <summary>
        /// Deletes the instance of <see cref="IWalletReceipt"/> with a matching id.
        /// </summary>
        /// <param name="id">The id of the <see cref="IWalletReceipt"/> to delete.</param>
        /// <returns>An <see cref="int"/> containing the result of the delete.</returns>
        public int Delete(int id)
        {
            UserIdentity identity = this._identityProvider.GetUserIdentity();

            if (this._logger.IsDebugEnabled)
            {
                this._logger.Debug($"SqlWalletReceiptsFactory.Delete({id}) called.");
            }

            var receipt = this.GetFromDatabase(id)[0];

            this._customerDataConnection.Parameters.Clear();
            this._customerDataConnection.Parameters.Add(new SqlParameter("@WalletReceiptId", SqlDbType.Int) { Value = id });

            this._customerDataConnection.Parameters.AddAuditing(identity);

            this._customerDataConnection.Parameters.ReturnValue = "@returnvalue";
            int returnCode = this._customerDataConnection.ExecuteProc<int>("DeleteWalletReceipt");

            if (this._logger.IsDebugEnabled)
            {
                this._logger.DebugFormat($"Delete completed with id {id} and return code {returnCode}.");
            }

            if (returnCode == 0)
            {
                new CloudWalletReceipt().Delete($"Wallet{id}.{receipt.FileExtension}", identity.AccountId.ToString());
            }

            return returnCode;
        }

        /// <summary>
        /// Gets a list of all available <see cref="IWalletReceipt"/>
        /// </summary>
        /// <returns>The list of <see cref="IWalletReceipt"/></returns>
        public IList<IWalletReceipt> Get()
        {
            return this.GetFromDatabase(null);
        }

        /// <summary>
        /// Gets an instance of <see cref="IList{T}"/> containing all available <see cref="IWalletReceipt"/> that match <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Criteria to match on.</param>
        /// <returns>An instance of <see cref="IList{T}"/> containing all available <see cref="IWalletReceipt"/> that match <paramref name="predicate"/>.</returns>
        public IList<IWalletReceipt> Get(Predicate<IWalletReceipt> predicate)
        {
            IList<IWalletReceipt> receipts = this.Get();

            if (predicate == null)
            {
                return receipts;
            }

            List<IWalletReceipt> matchReceipts = new List<IWalletReceipt>();

            foreach (IWalletReceipt receipt in receipts)
            {
                if (predicate.Invoke(receipt))
                {
                    matchReceipts.Add(receipt);
                }
            }

            return matchReceipts;
        }

        /// <summary>
        /// Gets a collection of <see cref="IWalletReceipt"/> or a specific instance if <paramref name="id"/> has a value.
        /// </summary>
        /// <param name="id">The <c>Id</c> of the <see cref="IWalletReceipt"/> you want to retrieve from <see cref="IDataConnection{SqlParameter}"/> or null if you want every <see cref="IWalletReceipt"/></param>
        /// <returns>The required <see cref="IWalletReceipt"/> or <see langword="null" /> or a collection of <see cref="IWalletReceipt"/> from <see cref="IDataConnection{SqlParameter}"/></returns>
        private List<IWalletReceipt> GetFromDatabase(int? id)
        {
            UserIdentity identity = this._identityProvider.GetUserIdentity();

            List<IWalletReceipt> receipts = new List<IWalletReceipt>();

            string sql =
                @"SELECT WalletReceipts.WalletReceiptId, WalletReceipts.FileExtension, WalletReceipts.[Status], ProcessedReceipts.[Date], ProcessedReceipts.Total, ProcessedReceipts.Merchant, WalletReceipts.CreatedOn, WalletReceipts.CreatedBy FROM WalletReceipts FULL OUTER JOIN ProcessedReceipts ON ProcessedReceipts.WalletReceiptId = WalletReceipts.WalletReceiptId WHERE WalletReceipts.CreatedBy = @employeeId";

            this._customerDataConnection.Parameters.Add(new SqlParameter("@employeeId", SqlDbType.Int) { Value = identity.EmployeeId });

            if (id.HasValue)
            {
                sql += " AND WalletReceipts.WalletReceiptId = @WalletReceiptId";
                this._customerDataConnection.Parameters.Add(new SqlParameter("@WalletReceiptId", SqlDbType.Int) { Value = id.Value });
            }

            using (var reader = this._customerDataConnection.GetReader(sql))
            {
                int receiptIdOrdinal = reader.GetOrdinal("WalletReceiptId");
                int fileExtensionOrdinal = reader.GetOrdinal("FileExtension");
                int statusOrdinal = reader.GetOrdinal("Status");
                int dateOrdinal = reader.GetOrdinal("Date");
                int totalOrdinal = reader.GetOrdinal("Total");
                int merchantOrdinal = reader.GetOrdinal("Merchant");
                int createdOnOrdinal = reader.GetOrdinal("CreatedOn");
                int createdByOrdinal = reader.GetOrdinal("CreatedBy");

                while (reader.Read())
                {
                    int receiptId = reader.GetInt32(receiptIdOrdinal);
                    string fileExtension = reader.GetString(fileExtensionOrdinal);
                    int status = reader.GetByte(statusOrdinal);

                    DateTime? date = null;
                    if (!reader.IsDBNull(dateOrdinal))
                    {
                        date = reader.GetDateTime(dateOrdinal);
                    }

                    decimal? total = null;
                    if (!reader.IsDBNull(totalOrdinal))
                    {
                        total = reader.GetDecimal(totalOrdinal);
                    }

                    string merchant = string.Empty;
                    if (!reader.IsDBNull(merchantOrdinal))
                    {
                        merchant = reader.GetString(merchantOrdinal);
                    }

                    int createdBy = reader.GetInt32(createdByOrdinal);
                    DateTime createdOn = reader.GetDateTime(createdOnOrdinal);

                    var receipt = new WalletReceipt(receiptId, fileExtension, "")
                    {
                        ReceiptData = new CloudWalletReceipt().Get($"Wallet{receiptId}.{fileExtension}", identity.AccountId.ToString()),
                        Status = status,
                        Date = date,
                        Total = total,
                        Merchant = merchant,
                        CreatedBy = createdBy,
                        CreatedOn = createdOn
                    };

                    receipts.Add(receipt);
                }
            }
            
            this._customerDataConnection.Parameters.Clear();

            return receipts;
        }
    }
}
