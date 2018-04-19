namespace PublicAPI.DTO
{
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;

    /// <summary>
    /// Defines a <see cref="AccountPropertiesDTO"/> and it's members
    /// </summary>
    public class AccountPropertiesDTO
    {
        /// <summary>
        /// Gets or sets the Id for this <see cref="AccountPropertiesDTO"/>.
        /// </summary>
        [JsonIgnore]
        public string Id => $"{this.SubAccountId}/{this.Key}";

        /// <summary>
        /// Gets or sets the key for this <see cref="AccountPropertiesDTO"/>.
        /// </summary>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name for this <see cref="AccountPropertiesDTO"/>.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the sub account id for this <see cref="AccountPropertiesDTO"/>.
        /// </summary>
        [Required]
        public int SubAccountId { get; set; }

        /// <summary>
        /// Gets or sets the post key for this <see cref="AccountPropertiesDTO"/>.
        /// </summary>
        public string PostKey { get; set; }

        /// <summary>
        /// Gets or sets the is global for this <see cref="AccountPropertiesDTO"/>.
        /// </summary>
        public bool IsGlobal { get; set; }
    }
}