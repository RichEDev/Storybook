namespace Spend_Management.shared.webServices
{
    using System;

    /// <summary>
    /// A basic report class holding name and Id
    /// </summary>
    public class ReportBasic
    {
        /// <summary>
        /// The Name of the report
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ID <see cref="Guid"/> of the report
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Create an instance of <see cref="ReportBasic"/>
        /// </summary>
        /// <param name="name">The report name</param>
        /// <param name="id">The report ID</param>
        public ReportBasic(string name, Guid id)
        {
            this.Name = name;
            this.Id = id;
        }
    }
}