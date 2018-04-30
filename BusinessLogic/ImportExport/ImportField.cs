namespace BusinessLogic.ImportExport
{
    using System;

    using BusinessLogic.Fields.Type.Base;

    /// <summary>
    /// A class to define an Field imported from another source.
    /// </summary>
    public class ImportField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportField"/> class.
        /// </summary>
        /// <param name="destinationColumn">
        /// The destination column  <see cref="IField"/> ID.
        /// </param>
        /// <param name="lookupColumn">
        /// The lookup column  <see cref="IField"/> ID.
        /// </param>
        /// <param name="defaultValue">
        /// The default value for the column if no value is found.
        /// </param>
        public ImportField(Guid destinationColumn, Guid lookupColumn, string defaultValue)
        {
            this.DestinationColumn = destinationColumn;
            this.LookupColumn = lookupColumn;
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Gets or sets the destination column <see cref="IField"/> ID.
        /// </summary>
        public Guid DestinationColumn { get; set; }

        /// <summary>
        /// Gets or sets the lookup column <see cref="IField"/> ID.
        /// </summary>
        public Guid LookupColumn { get; set;  }

        /// <summary>
        /// Gets or sets the default value for the column if no value is found.
        /// </summary>
        public string DefaultValue { get; set;  }
    }
}