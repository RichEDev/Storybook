namespace InternalApi.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using BusinessLogic.Announcements;

    /// <summary>
    /// An implementation of the <see cref="AnnouncementDto"/> DTO
    /// </summary>
    public class AnnouncementDto
    {
        /// <summary>
        /// Gets or sets the Id for an <see cref="AnnouncementDto"/>
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Message for an <see cref="AnnouncementDto"/>
        /// </summary>
        [Required]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the date an <see cref="AnnouncementDto"/> can automatically become visible
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the date an <see cref="AnnouncementDto"/> can automatically be removed
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an <see cref="AnnouncementDto"/> is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the values indicating what set of users an <see cref="AnnouncementDto"/> is visible too
        /// </summary>
        [Required]
        public AudienceFlags Audience { get; set; }
    }
}