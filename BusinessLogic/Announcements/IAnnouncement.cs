namespace BusinessLogic.Announcements
{
    using System;

    using BusinessLogic.Accounts;
    using BusinessLogic.Employees.AccessRoles;
    using BusinessLogic.Identity;

    using Interfaces;

    /// <summary>
    /// An interface defining the common fields of an Announcement
    /// </summary>
    public interface IAnnouncement : IIdentifier<Guid>
    {
        /// <summary>
        /// Gets or sets the Message for an <see cref="IAnnouncement"/>
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the list of users <see cref="ReadReceipt"/> that have read an <see cref="IAnnouncement"/>
        /// </summary>
        ReadReceipts ReadReceipts { get; set; }

        /// <summary>
        /// Gets or sets the date an <see cref="IAnnouncement"/> can automatically become visible
        /// </summary>
        DateTime StartDate { get; set; }
        
        /// <summary>
        /// Gets or sets the date an <see cref="IAnnouncement"/> can automatically be removed
        /// </summary>
        DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an <see cref="IAnnouncement"/> is active
        /// </summary>
        bool Active { get; set; }

        /// <summary>
        /// Gets or sets the values indicating what set of users an <see cref="IAnnouncement"/> is visible to
        /// </summary>
        AudienceFlags Audience { get; set; }

        /// <summary>
        /// Is this object valid, based on the given <see cref="IAccount"/> and <see cref="UserIdentity"/>.
        /// </summary>
        /// <param name="account">
        ///     An instance of <see cref="IAccount"/>.
        /// </param>
        /// <param name="userIdentity">
        ///     An instance of <see cref="UserIdentity"/>.
        /// </param>
        /// <param name="combinedEmployeesAccessRolesFactory"></param>
        /// <returns>
        /// <see cref="bool"/> true if this object is valid..
        /// </returns>
        bool Valid(
            IAccount account,
            UserIdentity userIdentity,
            IEmployeeCombinedAccessRoles combinedEmployeesAccessRolesFactory);
    }
}