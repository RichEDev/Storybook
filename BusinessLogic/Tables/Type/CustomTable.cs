namespace BusinessLogic.Tables.Type
{
    using System;

    using BusinessLogic.Accounts.Elements;
    using BusinessLogic.Fields.Type.Base;

    public class CustomTable : MetabaseTable, ICustomTable
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CustomTable"/> class.
        /// </summary>
        /// <param name="name">The Name of the <see cref="ITable"/></param>
        /// <param name="joinType">The JoinType of the <see cref="ITable"/></param>
        /// <param name="description">The Description of the <see cref="ITable"/></param>
        /// <param name="allowReportOn">The flag to allow reporting of the <see cref="ITable"/></param>
        /// <param name="allowImport">The flag to allow importing of the <see cref="ITable"/></param>
        /// <param name="allowWorkflow">The flag to allow workflows with this <see cref="ITable"/></param>
        /// <param name="allowEntityRelationship">The flag to allow  entity relationships within the <see cref="ITable"/></param>
        /// <param name="id">The ID of this <see cref="ITable"/></param>
        /// <param name="primaryKeyId">The <see cref="Guid"/> which identifies the primary key <see cref="IField"/></param>
        /// <param name="keyFieldId">The <see cref="Guid"/> which identifies the key <see cref="IField"/></param>
        /// <param name="userDefinedTableId">The <see cref="Guid"/> which identifies the user defined <see cref="ITable"/></param>
        /// <param name="subAccountIdFieldId">The <see cref="Guid"/> which identifies the sub account  <see cref="IField"/> on this <see cref="ITable"/></param>
        /// <param name="element">The <see cref="ModuleElements"/> which identifies the module this <see cref="ITable"/> is valid for.</param>
        /// <param name="isSystemView">The flag which indicates that this <see cref="IField"/> is a system view</param>
        /// <param name="relabelParam">The <see cref="string"/> relabels the<see cref="IField"/> with a different name.</param>
        /// <param name="accountId">The <see cref="ICustomTable"/> Account ID.</param>
        public CustomTable(string name, byte joinType, string description, bool allowReportOn, bool allowImport, bool allowWorkflow, bool allowEntityRelationship, Guid id, Guid primaryKeyId, Guid keyFieldId, Guid userDefinedTableId, Guid subAccountIdFieldId, ModuleElements element, bool isSystemView, string relabelParam, int accountId)
            : base(name, joinType, description, allowReportOn, allowImport, allowWorkflow, allowEntityRelationship, id, primaryKeyId, keyFieldId, userDefinedTableId, subAccountIdFieldId, element, isSystemView, relabelParam)
        {
            this.AccountId = accountId;
        }


        /// <summary>
        /// Gets or sets the <see cref="ICustomTable"/> Account ID.
        /// </summary>
        public int AccountId { get; set; }
    }
}
