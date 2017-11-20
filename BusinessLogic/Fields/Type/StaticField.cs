namespace BusinessLogic.Fields.Type
{
    using System;

    using BusinessLogic.Fields.Type.Attributes;
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.Tables.Type;

    /// <summary>
    /// An instance of <see cref="StaticField"/>.
    /// </summary>
    public class StaticField : IField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticField"/> class. 
        /// </summary>
        /// <param name="name">
        /// The column name of the field.
        /// </param>
        /// <param name="value">
        /// The value of the field.
        /// </param>
        public StaticField(string name, string value)
        {
            this.Name = name;
            this.Description = value;
        }

        /// <summary>
        /// Gets or sets the identifier for <see cref="IField"/>
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets the name of the <see cref="IField"/>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the description of the <see cref="IField"/>
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the comment(s) of the <see cref="IField"/>
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Guid"/> of the <see cref="ITable"/> that thisw <see cref="IField"/> belongs to.
        /// </summary>
        public Guid TableId { get; set; }

        /// <summary>
        /// Gets or sets the Class property name of the <see cref="IField"/>
        /// </summary>
        public string ClassPropertyName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IField.FieldAttributes"/> of the <see cref="IField"/>, these store extra info for each type of field.
        /// </summary>
        public FieldAttributes FieldAttributes { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Guid"/> of the View group (old style reports)
        /// </summary>
        public Guid ViewGroupId { get; set; }

        /// <summary>
        /// Gets or sets the width of the <see cref="IField"/>
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the length of the <see cref="IField"/>
        /// </summary>
        public int Length { get; set; }
    }
}
