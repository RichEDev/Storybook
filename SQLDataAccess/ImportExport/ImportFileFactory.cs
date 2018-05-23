namespace SQLDataAccess.ImportExport
{
    using System;

    using BusinessLogic;
    using BusinessLogic.ImportExport;

    /// <summary>
    /// A factory class to create instances of <see cref="IImportFile"/> where 'T' is derived from the Table ID
    /// </summary>
    public class ImportFileFactory
    {
        private readonly IImportFile _projectCodeImportFactory;

        private readonly IImportFile _reasonsImportFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportFileFactory"/> class.
        /// </summary>
        /// <param name="projectCodeImportFactory">An instance of <see cref="ImportProjectCodes"/></param>
        /// <param name="reasonsImportFactory">An instance of <see cref="ImportReasons"/></param>
        public ImportFileFactory(ImportProjectCodes projectCodeImportFactory, ImportReasons reasonsImportFactory)
        {
            Guard.ThrowIfNull(projectCodeImportFactory, nameof(projectCodeImportFactory));
            Guard.ThrowIfNull(reasonsImportFactory, nameof(reasonsImportFactory));

            this._projectCodeImportFactory = projectCodeImportFactory;
            this._reasonsImportFactory = reasonsImportFactory;
        }

        /// <summary>
        /// Create a New instance of <see cref="IImportFile"/> where T is derived from the <paramref name="tableId"/>
        /// </summary>
        /// <param name="tableId">The <see cref="Guid"/>ID of the table to import</param>
        /// <returns>A valid instance of <see cref="IImportFile"/> or null if not currently implemented.</returns>
        public IImportFile New(Guid tableId)
        {
            if (tableId == BusinessLogic.Constants.Tables.ProjectCodes.TableId)
                return this._projectCodeImportFactory;

            if (tableId == BusinessLogic.Constants.Tables.Reasons.TableId)
                return this._reasonsImportFactory;

            return null;
        }

    }
}
