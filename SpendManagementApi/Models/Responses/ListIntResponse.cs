namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;

    /// <summary>
    /// A response that returns a List of ints
    /// </summary>
    public class ListIntResponse : ApiResponse
    {
        /// <summary>
        /// Creates a List of ints
        /// </summary>
        public List<int> List { get; set; }
    }
}