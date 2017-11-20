namespace BusinessLogic.Fields.Type.Base
{
    using System;

    using BusinessLogic.Fields.Type.Attributes;
    using BusinessLogic.Tables.Type;

    /// <summary>
    /// Defines common members of a <see cref="Field"/>.
    /// </summary>
    public class Field : IField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="id">
        /// The unique identifier for this <see cref="Field"/>
        /// </param>
        /// <param name="name">
        /// The name of this <see cref="Field"/>.
        /// </param>
        /// <param name="description">
        /// The description of this <see cref="Field"/>.
        /// </param>
        /// <param name="commment">
        /// The commment of this <see cref="Field"/>.
        /// </param>
        /// <param name="tableId">
        /// The unique identifier of the table this <see cref="Field"/> belongs to.
        /// </param>
        /// <param name="classPropertyName">
        /// The class property name.
        /// </param>
        /// <param name="fieldAttributes">
        /// Attributes this <see cref="Field"/> has.
        /// </param>
        /// <param name="viewGroupId">
        /// The <see cref="Guid"/> of the View group (old style reports)
        /// </param>
        /// <param name="width">
        /// The width of this <see cref="Field"/>.
        /// </param>
        /// <param name="length">
        /// The length of this <see cref="Field"/>.
        /// </param>
        public Field(Guid id, string name, string description, string commment, Guid tableId, string classPropertyName, FieldAttributes fieldAttributes, Guid viewGroupId, int width, int length)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Comment = commment;
            this.TableId = tableId;
            this.ClassPropertyName = classPropertyName;
            this.FieldAttributes = fieldAttributes;
            this.ViewGroupId = viewGroupId;
            this.Width = width;
            this.Length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="field">
        /// Creates an instance of <see cref="Field"/> from another <see cref="Field"/> instance.
        /// </param>
        public Field(Field field)
            : this(field.Id, field.Name, field.Description, field.Comment, field.TableId, field.ClassPropertyName, field.FieldAttributes, field.ViewGroupId, field.Width, field.Length)
        {
        }
        
        /// <summary>
        /// Gets or sets the id.
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
