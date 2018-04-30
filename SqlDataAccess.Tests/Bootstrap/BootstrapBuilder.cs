namespace SqlDataAccess.Tests.Bootstrap
{
    /// <summary>
    /// The testing bootstrap for creating objects used in unit tests.
    /// </summary>
    public class BootstrapBuilder 
    {
        /// <summary>
        /// Gets or sets the Mocked "System" objects
        /// </summary>
        public ISystem System { get; set; }

        /// <summary>
        /// Gets or sets the Mocked "Tables" objects
        /// </summary>
        public ITables Tables { get; set; }

        /// <summary>
        /// Gets or sets the Mocked "Fields" objects
        /// </summary>
        public IFields Fields { get; set; }

        /// <summary>
        /// Gets or sets the Mocked "Project Code" objects
        /// </summary>
        public IProjectCodes ProjectCodes { get; set; }

        /// <summary>
        /// Populate the <see cref="ISystem"/> object with Mocks.
        /// </summary>
        /// <returns>
        /// A valid <see cref="BootstrapBuilder"/> with the <seealso cref="ISystem"/> property populated.
        /// </returns>
        public BootstrapBuilder WithSystem()
        {
            this.System = new System();
            return this;
        }

        /// <summary>
        /// Populate the <see cref="ITables"/> object with Mocks.
        /// </summary>
        /// <returns>
        /// A valid <see cref="BootstrapBuilder"/> with the <seealso cref="ITables"/> property populated.
        /// </returns>
        public BootstrapBuilder WithTables()
        {
            if (this.System == null)
            {
                this.WithSystem();
            }

            this.Tables = new Tables(this.System.Logger);
            return this;
        }

        /// <summary>
        /// Populate the <see cref="IFields"/> object with Mocks.
        /// </summary>
        /// <returns>
        /// A valid <see cref="BootstrapBuilder"/> with the <seealso cref="IFields"/> property populated.
        /// </returns>
        public BootstrapBuilder WithFields()
        {
            if (this.System == null)
            {
                this.WithSystem();
            }

            this.Fields = new Fields(this.System.Logger);
            return this;
        }

        /// <summary>
        /// Populate the <see cref="IProjectCodes"/> object with Mocks.
        /// </summary>
        /// <returns>
        /// A valid <see cref="BootstrapBuilder"/> with the <seealso cref="IProjectCodes"/> property populated.
        /// </returns>
        public BootstrapBuilder WithProjectCodes()
        {
            if (this.System == null)
            {
                this.WithSystem();
            }

            if (this.Fields == null)
            {
                this.WithFields();
            }

            if (this.Tables == null)
            {
                this.WithTables();
            }

            this.ProjectCodes = new ProjectCodes(this.System, this.Fields, this.Tables);
            return this;
        }
    }
}
