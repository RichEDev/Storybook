namespace SpendManagementApi.Models.Requests.SelfRegistration
{
    using System.ComponentModel.DataAnnotations;

    using SpendManagementApi.Models.Common;

    /// <summary>
    /// This object represents the first call to the self registration API process. The properties within are essential to receive a correct response from the API.
    /// The email address will be used to gain account specific properties essential for the correct information to be returned to the consumer.
    /// </summary>
    public class SelfRegistrationInitiatorRequest : ApiRequest
    {
        /// <summary>
        /// Title of new user. E.g. Mr, Mrs, Miss, Doctor etc.
        /// </summary>
        [Required(ErrorMessage = @"Title is required"), StringLength(30, ErrorMessage = @"Password should be 30 characters or less.")]
        public string Title { get; set; }

        /// <summary>
        /// The new user's first name.
        /// </summary>
        [Required(ErrorMessage = @"Firstname is required"), StringLength(150, ErrorMessage = @"Firstname should be 150 characters or less.")]
        public string Firstname { get; set; }

        /// <summary>
        /// The new user's second / surname.
        /// </summary>
        [Required(ErrorMessage = @"Surname is required"), StringLength(150, ErrorMessage = @"Surname should be 150 characters or less.")]
        public string Surname { get; set; }

        /// <summary>
        /// The email address to be used for registration.
        /// </summary>
        [Required(ErrorMessage = @"Email address is required"), StringLength(250, ErrorMessage = @"Password should be 250 characters or less.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        /// <summary>
        /// A username for the new user that will be used for logging in.
        /// </summary>
        [Required(ErrorMessage = @"Username is required"), StringLength(50, ErrorMessage = @"Username should be 50 characters or less.")]
        public string Username { get; set; }

        /// <summary>
        /// A password for the new user that will be used for logging in.
        /// </summary>
        [Required(ErrorMessage = @"Password is required"), StringLength(250, ErrorMessage = @"Password should be 250 characters or less.")]
        public string Password { get; set; }
    }
}