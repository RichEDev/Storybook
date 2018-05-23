namespace PublicAPI.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;

    public class ReasonDTO
    {
        /// <summary>
        /// Gets or sets the id for this <see cref="ReasonDTO"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the archived state for this <see cref="ReasonDTO"/>.
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// Gets or sets the description for this <see cref="ReasonDTO"/>.
        /// </summary>
        [MaxLength(4000)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the name for this <see cref="ReasonDTO"/>.
        /// </summary>
        [Required, MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the account code vat for this <see cref="ReasonDTO"/>.
        /// </summary>
        [MaxLength(50)]
        public string AccountCodeVat { get; set; }

        /// <summary>
        /// Gets or sets the account code no vat for this <see cref="ReasonDTO"/>.
        /// </summary>
        [MaxLength(50)]
        public string AccountCodeNoVat { get; set; }

        /// <summary>
        /// Gets or sets the created on for this <see cref="ReasonDTO"/>.
        /// </summary>
        [JsonIgnore]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the created by for this <see cref="ReasonDTO"/>.
        /// </summary>
        [JsonIgnore]
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the modified on for this <see cref="ReasonDTO"/>.
        /// </summary>
        [JsonIgnore]
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets the modified by for this <see cref="ReasonDTO"/>.
        /// </summary>
        [JsonIgnore]
        public int? ModifiedBy { get; set; }
    }
}