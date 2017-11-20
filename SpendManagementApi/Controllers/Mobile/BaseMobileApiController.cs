namespace SpendManagementApi.Controllers.Mobile
{
    using System.Web.Http;
    using System.Web.Http.Description;

    using SpendManagementApi.Attributes;

    using SpendManagementLibrary.Mobile;

    /// <summary>
    /// The base API controller for all mobile actions
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [Version(1)]
    public class BaseMobileApiController : ApiController
    {
        /// <summary>
        /// Gets or sets the parent ServiceResultMessage for mobile API requests
        /// </summary>
        public ServiceResultMessage ServiceResultMessage { get; set; }
        
        /// <summary>
        /// Gets or sets the pairing key and serial key for mobile API requests
        /// </summary>
        public PairingKeySerialKey PairingKeySerialKey { get; set; }

        /// <summary>
        /// Initializes the API controller with default values.
        /// </summary>
        /// <param name="serviceResultMessage">The <see cref="ServiceResultMessage"/></param>
        /// <param name="pairingKeySerialKey">The <see cref="PairingKeySerialKey"/></param>
        internal virtual void Initialise(ServiceResultMessage serviceResultMessage, PairingKeySerialKey pairingKeySerialKey)
        {
            ServiceResultMessage = serviceResultMessage;
            PairingKeySerialKey = pairingKeySerialKey;
        }
    }
}