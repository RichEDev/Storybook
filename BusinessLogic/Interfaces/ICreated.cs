namespace BusinessLogic.Interfaces
{
    using System;

    /// <summary>
    /// Defines that an object that may have a created owner.
    /// </summary>
    public interface ICreated
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ICreated"/> had a created date.
        /// </summary>
        DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ICreated"/> had a created by user.
        /// </summary>
        int? CreatedBy { get; set; }
    }
}
