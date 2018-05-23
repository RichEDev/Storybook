namespace BusinessLogic.Reasons
{
    using System;

    /// <summary>
    /// Defines a <see cref="Reason"/> and all it's members
    /// </summary>
    [Serializable]
    public class Reason : IReason
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Reason"/> class.
        /// </summary>
        /// <param name="id">The id of the <see cref="Reason"/></param>
        /// <param name="archived">The archived of the <see cref="Reason"/></param>
        /// <param name="description">The description of the <see cref="Reason"/></param>
        /// <param name="name">The name of the <see cref="Reason"/></param>
        /// <param name="accountCodeVat">The account code vat of the <see cref="Reason"/></param>
        /// <param name="accountCodeNoVat">The account code no vat of the <see cref="Reason"/></param>
        public Reason(int id, bool archived, string description, string name, string accountCodeVat, string accountCodeNoVat, int? createdBy, DateTime? createdOn, int? modifiedBy, DateTime? modifiedOn)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Archived = archived;
            this.AccountCodeVat = accountCodeVat;
            this.AccountCodeNoVat = accountCodeNoVat;
            this.CreatedBy = createdBy;
            this.CreatedOn = createdOn;
            this.ModifiedBy = modifiedBy;
            this.ModifiedOn = modifiedOn;
        }

        /// <summary>
        /// Constructor for creating default instances
        /// </summary>
        public Reason()
        {
            
        }

        /// <summary>
        /// Gets or sets the id for this <see cref="Reason"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the archived state for this <see cref="Reason"/>.
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// Gets or sets the description for this <see cref="Reason"/>.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the name for this <see cref="Reason"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the account code vat for this <see cref="Reason"/>.
        /// </summary>
        public string AccountCodeVat { get; set; }

        /// <summary>
        /// Gets or sets the account code no vat for this <see cref="Reason"/>.
        /// </summary>
        public string AccountCodeNoVat { get; set; }

        /// <summary>
        /// Gets or sets the created on for this <see cref="Reason"/>.
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the created by for this <see cref="Reason"/>.
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the modified on for this <see cref="Reason"/>.
        /// </summary>
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets the modified by for this <see cref="Reason"/>.
        /// </summary>
        public int? ModifiedBy { get; set; }
    }
}
