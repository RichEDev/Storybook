using BusinessLogic.Fields.Type.Base;

namespace BusinessLogic.Tables.Type
{
    using System;

    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Interfaces;

    /// <summary>
    /// <see cref="ITable"/> defines a table.
    /// </summary>
    public interface ITable : IIdentifier<Guid>
    {
        /// <summary>
        /// Gets or sets the Name of the <see cref="ITable"/>
        /// </summary>
         string Name { get; set; }

        /// <summary>
        /// Gets or sets the JoinType of the <see cref="ITable"/>
        /// </summary>
        byte JoinType { get; set; }

        /// <summary>
        /// Gets or sets the Description of the <see cref="ITable"/>
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow reporting of the <see cref="ITable"/>
        /// </summary>
        bool AllowReportOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow importing of the <see cref="ITable"/>
        /// </summary>
        bool AllowImport { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow workflows with this <see cref="ITable"/>
        /// </summary>
        bool AllowWorkflow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow  entity relationships within the <see cref="ITable"/>
        /// </summary>
        bool AllowEntityRelationship { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ITable"/> has user defined fields
        /// </summary>
        bool HasUserdefinedFields { get; }

        /// <summary>
        /// Gets or sets the <see cref="Guid"/> which identifies the primary key <see cref="IField"/>
        /// </summary>
         Guid PrimaryKeyId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Guid"/> which identifies the key <see cref="IField"/>
        /// </summary>
        Guid KeyFieldId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Guid"/> which identifies the user defined <see cref="ITable"/>
        /// </summary>
        Guid UserDefinedTableId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Guid"/> which identifies the sub account  <see cref="IField"/> on this <see cref="ITable"/>
        /// </summary>
        Guid SubAccountIdFieldId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ModuleElements"/> which identifies the module this <see cref="ITable"/> is valid for.
        /// </summary>
        ModuleElements Element { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IField"/> is a system view
        /// </summary>
        bool IsSystemView { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="string"/> relabels the<see cref="IField"/> with a different name.
        /// </summary>
        string RelabelParam { get; set; }
    }
}
