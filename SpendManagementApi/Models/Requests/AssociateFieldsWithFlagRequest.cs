namespace SpendManagementApi.Models.Requests
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The associate fields with flag request.
    /// </summary>
    public class AssociateFieldsWithFlagRequest
    {
        /// <summary>
        /// Gets or sets the flag id.
        /// </summary>
        public int FlagId { get; set; }

        /// <summary>
        /// Gets or sets the field ids.
        /// </summary>
        public List<Guid> FieldIds { get; set; }
    }
}