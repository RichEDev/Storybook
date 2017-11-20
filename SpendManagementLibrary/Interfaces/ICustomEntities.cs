namespace SpendManagementLibrary.Interfaces
{
    using System;

    public interface ICustomEntities
    {
        /// <summary>
        /// Get an instance of <see cref="cCustomEntity"/> by it's ID
        /// </summary>
        /// <param name="id">The <see cref="int"/>ID to retrieve</param>
        /// <returns>An instance of <see cref="cCustomEntity"/>or NULL if none found to match</returns>
        cCustomEntity getEntityById(int id);

        /// <summary>
        /// Get an instance of <see cref="cCustomEntity"/> by it's table ID
        /// </summary>
        /// <param name="id">The <see cref="Guid"/>ID of the table to retrieve</param>
        /// <returns>An instance of <see cref="cCustomEntity"/>if found, otherwise null</returns>
        cCustomEntity getEntityByTableId(Guid id);

        /// <summary>
        /// Get an instance of <see cref="cAttribute"/> by it's ID
        /// </summary>
        /// <param name="fieldID">The <see cref="Guid"/>ID of the <seealso cref="cAttribute"/>to retrieve</param>
        /// <returns>An instance of <see cref="cAttribute"/>if found, or null.</returns>
        cAttribute getAttributeByFieldId(Guid fieldID);
    }
}