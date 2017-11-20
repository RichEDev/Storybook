namespace EsrGo2FromNhsWcfLibrary.Interfaces
{
    using System.Collections.Generic;

    using EsrGo2FromNhsWcfLibrary.Base;

    /// <summary>
    /// The DataAccess interface.
    /// </summary>
    /// <typeparam name="T">
    /// The DataClass to use / return with this instance
    /// </typeparam>
    public interface IDataAccess<T>
    {
        /// <summary>
        /// Create a specific entity.
        /// </summary>
        /// <param name="entities">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="DataClassBase"/>.
        /// </returns>
        List<T> Create(List<T> entities);

        /// <summary>
        /// Read a specific entity.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="DataClassBase"/>.
        /// </returns>
        T Read(int entityId);

        /// <summary>
        /// Read a specific entity.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="DataClassBase"/>.
        /// </returns>
        T Read(long entityId);

        /// <summary>
        /// read all records for an entity.
        /// </summary>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        List<T> ReadAll();

        /// <summary>
        /// The read by esr id method.
        /// </summary>
        /// <param name="esrId">
        /// The esr id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<T> ReadByEsrId(long esrId);

        /// <summary>
        /// Special read
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        List<T> ReadSpecial(string reference);

        /// <summary>
        /// Update a specific entity.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="DataClassBase"/>.
        /// </returns>
        List<T> Update(List<T> entities);

        /// <summary>
        /// Delete a specific entity.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="DataClassBase"/>.
        /// </returns>
        T Delete(long entityId);

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Delete(T entity);
    }
}

