namespace SpendManagementLibrary.Interfaces
{
    /// <summary>
    /// The DetailBase interface.
    /// </summary>
    public interface IOwnership
    {
        #region Public Properties

        /// <summary>
        ///     Gets the combined item key.
        /// </summary>
        string CombinedItemKey { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The element type.
        /// </summary>
        /// <returns>
        ///     The <see cref="SpendManagementElement" />.
        /// </returns>
        SpendManagementElement ElementType();

        /// <summary>
        ///     The item definition.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        string ItemDefinition();

        /// <summary>
        ///     The item primary id.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        int ItemPrimaryID();

        /// <summary>
        ///     The owner element type.
        /// </summary>
        /// <returns>
        ///     The <see cref="SpendManagementElement" />.
        /// </returns>
        SpendManagementElement OwnerElementType();

        /// <summary>
        ///     The owner id.
        /// </summary>
        /// <returns>
        ///     The <see cref="int?" />.
        /// </returns>
        int? OwnerId();

        #endregion
    }
}