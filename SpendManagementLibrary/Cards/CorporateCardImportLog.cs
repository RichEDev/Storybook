namespace SpendManagementLibrary.Cards
{
    using System;
    using System.Text;
    using Enumerators;

    /// <summary>
    /// A class to represent a CorporateCardLog record 
    /// </summary>
    public class CorporateCardImportLog
    {
        /// <summary>
        /// A private instance of <see cref="StringBuilder"/>
        /// </summary>
        private readonly StringBuilder messageBuilder;

        /// <summary>
        /// Create a new instance of <see cref="CorporateCardImportLog"/>
        /// </summary>
        /// <param name="cardProviderId">The ID of the <seealso cref="cCardProvider"/></param>
        public CorporateCardImportLog(int cardProviderId)
        {
            this.CardProviderId = cardProviderId;
            this.ImportId = Guid.NewGuid();
            this.Date = DateTime.Now;
            this.messageBuilder = new StringBuilder();
        }

        public CorporateCardImportLog(int cardProviderId, Guid importId, DateTime date, string logMessage, int statementId, CorporateCardImportStatus status, int numberOfErrors)
        {
            this.CardProviderId = cardProviderId;
            this.ImportId = importId;
            this.Date = date;
            this.messageBuilder = new StringBuilder();
            this.messageBuilder.Append(logMessage);
            this.StatementId = statementId;
            this.Status = status;
            this.NumberOfErrors = numberOfErrors;
        }

        /// <summary>
        /// Gets the ID of the Log record
        /// </summary>
        public Guid ImportId { get; }


        /// <summary>
        /// Gets the ID of the <seealso cref="cCardProvider"/>
        /// </summary>
        public int CardProviderId { get; set; }

        /// <summary>
        /// Gets the log message
        /// </summary>
        public string LogMessage
        {
            get { return this.messageBuilder.ToString(); }
        }

        /// <summary>
        /// Gets the Date the Log entry was created.
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// Gets the ID of the <seealso cref="cCardStatement"/>
        /// </summary>
        public int StatementId { get; set; }

        /// <summary>
        /// Gets the status of the Import
        /// </summary>
        public CorporateCardImportStatus Status { get; set; }

        /// <summary>
        /// Gets the number of errors in the Import
        /// </summary>
        public int NumberOfErrors { get; set; }

        /// <summary>
        /// Append an entry to the current <see cref="LogMessage"/>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AppendToLog(string message)
        {
            try
            {
                this.messageBuilder.Append(message);
                this.messageBuilder.Append(Environment.NewLine);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
