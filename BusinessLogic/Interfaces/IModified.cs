namespace BusinessLogic.Interfaces
{
    using System;

    /// <summary>
    /// Defines that an object that may have a modified owner.
    /// </summary>
    public interface IModified
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IModified"/> had a modified date.
        /// </summary>
        DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IModified"/> had a modified by user.
        /// </summary>
        int? ModifiedBy { get; set; }
    }
}
