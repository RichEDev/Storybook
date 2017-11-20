namespace Spend_Management.shared.code.AccessRoles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Logic_Classes.Fields;

    using Spend_Management.shared.code.EasyTree;

    /// <summary>
    /// The access role reportable field class.
    /// </summary>
    public class AccessRoleReportableField
    {
        /// <summary>
        /// The _current user instance.
        /// </summary>
        private readonly CurrentUser _currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessRoleReportableField"/> class.
        /// </summary>
        /// <param name="currentUser">
        /// The current use instancer.
        /// </param>
        public AccessRoleReportableField(CurrentUser currentUser)
        {
            this._currentUser = currentUser;
        }

        /// <summary>
        /// Get reportable fields for the accessrole by passing tableID.
        /// </summary>
        /// <param name="thisTable">
        /// The table ID of the reportable fields.
        /// </param>
        /// <returns>
        /// The List of fields object.
        /// </returns>
        public List<cField> GetReportableFieldsForAccessRole(Guid thisTable)
        {
            var user = cMisc.GetCurrentUser();
            var clsSubAccounts = new cAccountSubAccounts(user.AccountID);
            var accountProperties = clsSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;
            var fields = new SubAccountFields(new cFields(user.AccountID), new FieldRelabler(accountProperties));
            var entityFields = fields.GetFieldsByTableIDForViews(thisTable);
            entityFields = this.FilterOutFieldsWhichAreNotRequiredToReportOn(entityFields);
            return entityFields;
        }

        /// <summary>
        /// Filter the given list and return fields that have either a view group or are foreign keys
        /// </summary>
        /// <param name="entityFields">
        /// The initialise <see cref="List{T}"/>of 
        /// <seealso cref="cField"/>
        /// </param>
        /// <returns>
        /// A list of fields that have view groups of are foreign keys unless no fields have view groups and no tree groups then return all fields
        /// </returns>
        private List<cField> FilterOutFieldsWhichAreNotRequiredToReportOn(List<cField> entityFields)
        {
            if (entityFields.Any(field => field.ViewGroupID != Guid.Empty) && entityFields.Any(field => field.TreeGroup.HasValue))
            {
                var result = entityFields.Where(f => f.ViewGroupID != Guid.Empty || f.TreeGroup.HasValue || f.IsForeignKey);
                entityFields = result.ToList();
            }

            var fieldToBeRemoved = new FilteredFields(this._currentUser.CurrentActiveModule);
            var finalFields = entityFields.Where(f => !fieldToBeRemoved.FilterField(f, string.Empty));
            finalFields = finalFields.Where(f => !f.Description.StartsWith("Old "));
            return finalFields.ToList();
        }
    }
}