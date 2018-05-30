namespace BusinessLogic.Announcements
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic.Accounts;
    using BusinessLogic.Employees.AccessRoles;
    using BusinessLogic.Identity;
    using BusinessLogic.Validator;

    /// <summary>
    /// An implementation of the <see cref="IAnnouncement"/> interface for an Announcement.
    /// </summary>
    [Serializable]
    public class Announcement : IAnnouncement
    {
        /// <summary>
        /// A private <see cref="List{T}"/> of <seealso cref="IValidator"/>.
        /// </summary>
        private readonly List<IValidator> _validators;

        /// <summary>
        /// A private instance of <see cref="AudienceFlags"/>.
        /// </summary>
        private AudienceFlags _audience;

        /// <summary>
        /// Initializes a new instance of the <see cref="Announcement"/> class.
        /// </summary>
        public Announcement()
        {
            this._validators = new List<IValidator>();
            this.ReadReceipts = new ReadReceipts();
        }

        /// <summary>
        /// Gets or sets the Id for an <see cref="Announcement"/>
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Message for an <see cref="Announcement"/>
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the list of users that have read an <see cref="Announcement"/>
        /// </summary>
        public ReadReceipts ReadReceipts { get; set; }

        /// <summary>
        /// Gets or sets the date an <see cref="Announcement"/> can automatically become visible
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the date an <see cref="Announcement"/> can automatically be removed
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an <see cref="Announcement"/> is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the values indicating what set of users an <see cref="Announcement"/> is visible too
        /// </summary>
        public AudienceFlags Audience
        {
            get => this._audience;
            set
            {
                this._audience = value;

                if ((this.Audience & AudienceFlags.NonNhs) == AudienceFlags.NonNhs)
                {
                    this._validators.Add(new NonNhsValidator());
                }

                if ((this.Audience & AudienceFlags.Nhs) == AudienceFlags.Nhs)
                {
                    this._validators.Add(new NhsValidator());
                }

                if ((this.Audience & AudienceFlags.Admins) == AudienceFlags.Admins)
                {
                    this._validators.Add(new AdminValidator());
                }

                if ((this.Audience & AudienceFlags.Claimants) == AudienceFlags.Claimants)
                {
                    this._validators.Add(new ClaimantValidator());
                }
            }
        }

        /// <summary>
        /// A method that returns a boolean value indicating whether an instance of <see cref="IAnnouncement"/> should be visible by the current user on the current day.
        /// </summary>
        /// <param name="account">An instance of <see cref="IAccount"/> associated with the current account</param>
        /// <param name="userIdentity">An instance of <see cref="UserIdentity"/></param>
        /// <param name="combinedEmployeesAccessRolesFactory">A instance of <see cref="IEmployeeCombinedAccessRoles"/></param>
        /// <returns>True if the message is valid for that user, otherwise false</returns>
        public bool Valid(
            IAccount account,
            UserIdentity userIdentity,
            IEmployeeCombinedAccessRoles combinedEmployeesAccessRolesFactory)
        {
            if (this.StartDate > DateTime.Today || this.EndDate < DateTime.Today)
            {
                return false;
            }
            
            foreach (IValidator validator in this._validators)
            {
                if (!validator.Valid(account, userIdentity, combinedEmployeesAccessRolesFactory))
                {
                    return false;
                }
            }

            return true;
        }
    }
}