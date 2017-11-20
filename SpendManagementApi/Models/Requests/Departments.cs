﻿namespace SpendManagementApi.Models.Requests
{ 
    using Common;

    /// <summary>
    /// Facilitates the finding of Departments, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindDepartmentsRequest : FindRequest
    {
        /// <summary>
        /// Search by label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Search by description.
        /// </summary>
        public string Description { get; set; }

    }
}