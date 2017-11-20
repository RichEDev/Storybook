
namespace SpendManagementLibrary.DVLA
{
    /// <summary>
    /// Represents Dvla Service error code informations
    /// </summary>
    public class DvlaServiceResponseCodes
    {

        /// <summary>
        /// Description of the dvla error response code
        /// </summary>
        public string ResponseCodeDescription { get; set; }

        /// <summary>
        /// Friendly text for Dvla error responses
        /// </summary>
        public string ResponseCodeFriendlyMessages { get; set; }

        /// <summary>
        /// Create a new instance of <see cref="DvlaServiceResponseCodes"/>
        /// </summary>
        /// <param name="responseCodeDescription">Description of the response code returned by the Dvla Api</param>
        /// <param name="responseCodeFriendlyMessages">Friendly text for the response code returned by the Dvla Api</param>
        public DvlaServiceResponseCodes(string responseCodeDescription, string responseCodeFriendlyMessages)
        {
            this.ResponseCodeDescription = responseCodeDescription;
            this.ResponseCodeFriendlyMessages = responseCodeFriendlyMessages;
        }
    }
}
