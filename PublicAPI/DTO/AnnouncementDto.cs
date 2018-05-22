namespace PublicAPI.DTO
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
        /// Gets or sets the Id for a <see cref="AnnouncementDto"/>
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Message for a <see cref="AnnouncementDto"/>
        /// </summary>
        [Required]
        public string Message { get; set; }
    }
}