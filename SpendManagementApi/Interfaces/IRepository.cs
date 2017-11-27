using System.Collections.Generic;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Interfaces
{
    /// <summary>
    /// Represents a repository that has archiving functionality and works with IArchivalble
    /// </summary>
    /// <typeparam name="T">The underlying Type of the repository to create.</typeparam>
    public interface IArchivingRepository<T> : IRepository<T> where T : ArchivableBaseExternalType
    {
        /// <summary>
        /// Archives or un-archives the archivable object.
        /// </summary>
        /// <param name="id">The id of the object to archive</param>
        /// <param name="archive">Whether to archive or un-archive the item.</param>
        /// <returns></returns>
        T Archive(int id, bool archive);
    }

    /// <summary>
    /// Represents a typed repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> : IBasicRepository where T : BaseExternalType
    {

        /// <summary>
        /// Gets all entries
        /// </summary>
        /// <returns>A List of Type T</returns>
        IList<T> GetAll();

        /// <summary>
        /// Gets a single entry by it's id.
        /// </summary>
        /// <param name="id">The id of the entry.</param>
        /// <returns>The Entry, of Type T</returns>
        T Get(int id);

        /// <summary>
        /// Adds a new Entry to the repository.
        /// </summary>
        /// <param name="dataToAdd">The item to add</param>
        /// <returns>The added item</returns>
        T Add(T dataToAdd);

        /// <summary>
        /// Updates an item in the repository.
        /// </summary>
        /// <param name="dataToUpdate">The item to update.</param>
        /// <returns>The updated item.</returns>
        T Update(T dataToUpdate);

        /// <summary>
        /// Deletes an existing entry. Checks should be made to ensure that the record exists
        /// This method should throw an exception if data cannot be saved successfully
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Delete(int id);
    }
}
