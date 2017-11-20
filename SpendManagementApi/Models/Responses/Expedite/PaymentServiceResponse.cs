namespace SpendManagementApi.Models.Responses.Expedite
{
     using System.Collections.Generic;
    using SLE = SpendManagementLibrary.Expedite;
    using Types.Expedite;
    using Common;
   
    /// <summary>
    /// Represents a response containing a list of <see cref="PaymentService"/> accounts.
    /// </summary>
    public class PaymentServiceResponse : ApiResponse<PaymentService>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="PaymentServiceResponse"/> class.
        /// </summary>
        public PaymentServiceResponse()
        {
            this.List = new List<PaymentService>();
        }
        /// <summary>
        /// Gets or sets the list of PaymentService accounts.
        /// </summary>
        public List<PaymentService> List { get; set; }

    }
    /// <summary>
    /// Represents a response containing Payment process status update(download or execute status)
    /// </summary>
    public class PaymentProcessStatusResponse : ApiResponse<PaymentService>
    {
        /// Gets or sets processed status for a request
        /// </summary>
        public int isProcessed { get; set; }
    }
    /// <summary>
    /// A response containing information regarding sending of email.
    /// </summary>
    public class EmailSenderResponse : ApiResponse
    {
        /// <summary>
        /// flag telling whether sending email was successful or not
        /// </summary>
        public bool isSendingSuccessful { get; set; }
        /// <summary>
        /// error message to be set if any exceptions arise
        /// </summary>
        public string errorMessage { get; set; }
    }
    /// <summary>
    /// A response containing information of the expedite customers details
    /// </summary>
    public class CustomersDetailsResponse : ApiResponse
    {
        /// <summary>
        /// list of customer details.
        /// </summary>
       public List<SLE.CustomerEmailDetails> Items { get; set; }

    }
}
  
