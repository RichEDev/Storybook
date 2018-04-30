namespace BusinessLogic.ImportExport
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Import a file into expenses.
    /// </summary>
    /// <typeparam name="T"> The target file type to inport into.
    /// </typeparam>
    public interface IImportFile<T>
    {
        /// <summary>
        /// Import a given file into expenses.
        /// </summary>
        /// <param name="defaultValues">
        /// The default values for any non-matched columns.
        /// </param>
        /// <param name="matchedColumns">
        /// The import file columns matched to <see cref="Guid"/> file ID's within expenses.
        /// </param>
        /// <param name="fileContents">
        /// The file content.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/> of imort stats results (one per row).
        /// </returns>
        List<string> Import(
            SortedList<Guid, string> defaultValues,
            List<ImportField> matchedColumns,
            List<List<object>> fileContents);
    }
}
