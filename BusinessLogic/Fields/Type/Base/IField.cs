namespace BusinessLogic.Fields.Type.Base
{
    using System;

    using BusinessLogic.Fields.Type.Attributes;
    using BusinessLogic.Interfaces;
    using BusinessLogic.Tables.Type;

    /// <summary>
    /// Defines the <see cref="IField"/> used throughout the system.
    /// </summary>
    public interface IField : IIdentifier<Guid>
    {
        /// <summary>
        /// Gets the name of the <see cref="IField"/>
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the description of the <see cref="IField"/>
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the comment(s) of the <see cref="IField"/>
        /// </summary>
        string Comment { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Guid"/> of the <see cref="ITable"/> that thisw <see cref="IField"/> belongs to.
        /// </summary>
        Guid TableId { get; set; }

        /// <summary>
        /// Gets or sets the Class property name of the <see cref="IField"/>
        /// </summary>
        string ClassPropertyName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FieldAttributes"/> of the <see cref="IField"/>, these store extra info for each type of field.
        /// </summary>
        FieldAttributes FieldAttributes { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Guid"/> of the View group (old style reports)
        /// </summary>
        Guid ViewGroupId { get; set; }

        /// <summary>
        /// Gets or sets the width of the <see cref="IField"/>
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Gets or sets the length of the <see cref="IField"/>
        /// </summary>
        int Length { get; set; }
    }
}
