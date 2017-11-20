namespace ApiCrud.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ApiCrud.DataAccess;

    using ApiLibrary.DataObjects.Base;

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
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="DataClassBase"/>.
        /// </returns>
        List<T> Create(List<T> entity);

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
        /// read all records for an entity.
        /// </summary>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        List<T> ReadAll();

        /// <summary>
        /// The read by employee id method.
        /// </summary>
        /// <param name="personId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<T> ReadByEsrId(long personId);

        /// <summary>
        /// Special read.
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
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="DataClassBase"/>.
        /// </returns>
        List<T> Update(List<T> entity);

        /// <summary>
        /// Delete a specific entity by id.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="DataClassBase"/>.
        /// </returns>
        T Delete(int entityId);

        /// <summary>
        /// The delete by entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to delete.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T Delete(T entity);
    }
}

