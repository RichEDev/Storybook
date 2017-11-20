namespace SpendManagementApi.Models.Requests
{
    using System.ComponentModel.DataAnnotations;
    using Utilities;

    /// <summary>
    /// Facilitates the finding of Organisations
    /// </summary>
    public class FindOrganisationRequest 
    {
        /// <summary>
        /// The name / label for this Organisation.
        /// </summary>
        [MaxLength(256, ErrorMessage = ApiResources.ErrorMaxLength + @"256")]
        public string Label { get; set; }

        /// <summary>
        /// A comment of this Organisation object.
        /// </summary>
        [MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Comment { get; set; }

        /// <summary>
        /// The Organisation code
        /// </summary>
        [MaxLength(60, ErrorMessage = ApiResources.ErrorMaxLength + @"60")]
        public string Code { get; set; }

        /// <summary>
        /// Where the Organisation is archived
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// The first line of the address
        /// </summary>
        [MaxLength(256, ErrorMessage = ApiResources.ErrorMaxLength + @"256")]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// The city
        /// </summary>
        [MaxLength(256, ErrorMessage = ApiResources.ErrorMaxLength + @"256")]
        public string City { get; set; }

        /// <summary>
        /// The Postcode
        /// </summary>
        [MaxLength(32, ErrorMessage = ApiResources.ErrorMaxLength + @"32")]
        public string PostCode { get; set; }
    }
}