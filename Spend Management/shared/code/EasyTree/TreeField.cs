namespace Spend_Management.shared.code.EasyTree
{
    using System;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Report;

    /// <summary>
    /// An implementation of <see cref="cField"/>that also includes the GroupName
    /// </summary>
    internal class TreeField : cField
    {
        /// <summary>
        /// Gets or sets the Group name for this <see cref="TreeField"/>
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// Create an instance of <see cref="TreeField"/> based on an existing <seealso cref="cField"/>
        /// </summary>
        /// <param name="field">The <see cref="cField"/>to use as a base for this <seealso cref="TreeField"/></param>
        /// <param name="treeGroups">An instance of <see cref="ITreeGroups"/>used to retrieve the group names</param>
        public TreeField(cField field, ITreeGroups treeGroups)
        {
            this.AccountID = field.AccountID;
            this.FieldID = field.FieldID;
            this.TableID = field.TableID;
            this.ViewGroupID = field.ViewGroupID;
            this.LookupFieldID = field.LookupFieldID;
            this.RelatedTableID = field.RelatedTableID;
            this.LookupTableID = field.LookupTableID;
            this.FieldName = field.FieldName;
            this.FieldType = field.FieldType;
            this.Description = field.Description;
            this.Comment = field.Comment;
            this.NormalView = field.NormalView;
            this.IDField = field.IDField;
            this.GenList = field.GenList;
            this.CanTotal = field.CanTotal;
            this.Width = field.Width;
            this.ValueList = field.ValueList;
            this.AllowImport = field.AllowImport;
            this.Mandatory = field.Mandatory;
            this.PrintOut = field.PrintOut;
            this.Length = field.Length;
            this.WorkflowUpdate = field.WorkflowUpdate;
            this.WorkflowSearch = field.WorkflowSearch;
            this.RelabelParam = field.RelabelParam;
            this.ClassPropertyName = field.ClassPropertyName;
            this.ListItems = field.ListItems;
            this.UseForLookup = field.UseForLookup;
            this.FieldSource = field.FieldSource;
            this.FieldCategory = field.FieldCategory;
            this.IsForeignKey = field.IsForeignKey;
            this.FriendlyNameTo = field.FriendlyNameTo;
            this.FriendlyNameFrom = field.FriendlyNameFrom;
            if (field.TreeGroup.HasValue)
            {
                this.GroupName = this.GetGroupName(field.TreeGroup.Value, treeGroups);
            }

            if (this.IsForeignKey && this.RelatedTableID == Guid.Empty)
            {
                this.IsForeignKey = false;
            }
        }

        /// <summary>
        /// Get the name of the group for the given <see cref="Guid"/>group id
        /// </summary>
        /// <param name="treeGroupId">The ID of the group to retrieve</param>
        /// <param name="treeGroups">An instance of <see cref="ITreeGroups"/></param>
        /// <returns></returns>
        private string GetGroupName(Guid treeGroupId, ITreeGroups treeGroups)
        {
            var result = treeGroups.Get(treeGroupId);
            return result?.Name;
        }
    }
}