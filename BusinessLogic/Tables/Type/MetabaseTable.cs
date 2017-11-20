namespace BusinessLogic.Tables.Type
{
    using System;

    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Fields.Type.Base;

    /// <summary>
    /// an implentation of <see cref="ITable"/> for metabase tables.
    /// </summary>
    public class MetabaseTable : ITable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetabaseTable"/> class. 
        /// </summary>
        /// <param name="name">
        /// The Name of the <see cref="ITable"/>
        /// </param>
        /// <param name="joinType">
        /// The JoinType of the <see cref="ITable"/>
        /// </param>
        /// <param name="description">
        /// The Description of the <see cref="ITable"/>
        /// </param>
        /// <param name="allowReportOn">
        /// The flag to allow reporting of the <see cref="ITable"/>
        /// </param>
        /// <param name="allowImport">
        /// The flag to allow importing of the <see cref="ITable"/>
        /// </param>
        /// <param name="allowWorkflow">
        /// The flag to allow workflows with this <see cref="ITable"/>
        /// </param>
        /// <param name="allowEntityRelationship">
        /// The flag to allow  entity relationships within the <see cref="ITable"/>
        /// </param>
        /// <param name="id">
        /// The ID of this <see cref="ITable"/>
        /// </param>
        /// <param name="primaryKeyId">
        /// The <see cref="Guid"/> which identifies the primary key <see cref="IField"/>
        /// </param>
        /// <param name="keyFieldId">
        /// The <see cref="Guid"/> which identifies the key <see cref="IField"/>
        /// </param>
        /// <param name="userDefinedTableId">
        /// The <see cref="Guid"/> which identifies the user defined <see cref="ITable"/>
        /// </param>
        /// <param name="subAccountIdFieldId">
        /// The <see cref="Guid"/> which identifies the sub account  <see cref="IField"/> on this <see cref="ITable"/>
        /// </param>
        /// <param name="element">
        /// The <see cref="ModuleElements"/> which identifies the module this <see cref="ITable"/> is valid for.
        /// </param>
        /// <param name="systemView">
        /// The flag which indicates that this <see cref="IField"/> is a system view
        /// </param>
        /// <param name="relabelParam">
        /// The <see cref="string"/> relabels the<see cref="IField"/> with a different name.
        /// </param>
        public MetabaseTable(string name, byte joinType, string description, bool allowReportOn, bool allowImport, bool allowWorkflow, bool allowEntityRelationship, Guid id, Guid primaryKeyId, Guid keyFieldId, Guid userDefinedTableId, Guid subAccountIdFieldId, ModuleElements element,  bool systemView, string relabelParam)
        {
            this.Name = name;
            this.JoinType = joinType;
            this.Description = description;
            this.AllowReportOn = allowReportOn;
            this.AllowImport = allowImport;
            this.AllowWorkflow = allowWorkflow;
            this.AllowEntityRelationship = allowEntityRelationship;
            this.Id = id;
            this.PrimaryKeyId = primaryKeyId;
            this.KeyFieldId = keyFieldId;
            this.UserDefinedTableId = userDefinedTableId;
            this.SubAccountIdFieldId = subAccountIdFieldId;
            this.Element = element;
            this.IsSystemView = systemView;
            this.RelabelParam = relabelParam;
            this.HasUserdefinedFields = false;
        }

        /// <summary>
        /// Gets or sets the identifier for <see cref="ITable"/>
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or setsthe Name of the <see cref="ITable"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or setsthe JoinType of the <see cref="ITable"/>
        /// </summary>
        public byte JoinType { get; set; }

        /// <summary>
        /// Gets or setsthe Description of the <see cref="ITable"/>
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow reporting of the <see cref="ITable"/>
        /// </summary>
        public bool AllowReportOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether  to allow importing of the <see cref="ITable"/>
        /// </summary>
        public bool AllowImport { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether  to allow workflows with this <see cref="ITable"/>
        /// </summary>
        public bool AllowWorkflow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether  to allow  entity relationships within the <see cref="ITable"/>
        /// </summary>
        public bool AllowEntityRelationship { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ITable"/> has user defined fields
        /// </summary>
        public bool HasUserdefinedFields { get; }

        /// <summary>
        /// Gets or setsthe <see cref="Guid"/> which identifies the primary key <see cref="IField"/>
        /// </summary>
        public Guid PrimaryKeyId { get; set; }

        /// <summary>
        /// Gets or setsthe <see cref="Guid"/> which identifies the key <see cref="IField"/>
        /// </summary>
        public Guid KeyFieldId { get; set; }

        /// <summary>
        /// Gets or setsthe <see cref="Guid"/> which identifies the user defined <see cref="ITable"/>
        /// </summary>
        public Guid UserDefinedTableId { get; set; }

        /// <summary>
        /// Gets or setsthe <see cref="Guid"/> which identifies the sub account  <see cref="IField"/> on this <see cref="ITable"/>
        /// </summary>
        public Guid SubAccountIdFieldId { get; set; }

        /// <summary>
        /// Gets or setsthe <see cref="ModuleElements"/> which identifies the module this <see cref="ITable"/> is valid for.
        /// </summary>
        public ModuleElements Element { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether that this <see cref="IField"/> is a system view
        /// </summary>
        public bool IsSystemView { get; set; }

        /// <summary>
        /// Gets or setsthe <see cref="string"/> relabels the<see cref="IField"/> with a different name.
        /// </summary>
        public string RelabelParam { get; set; }
    }
}
