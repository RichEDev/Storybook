namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;

    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// The c cost code.
    /// </summary>
    [Serializable]
    public class cCostCode : IOwnership
    {
        #region Fields

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="cCostCode"/> class.
        /// </summary>
        /// <param name="costcodeId">
        /// The costcodeid.
        /// </param>
        /// <param name="costcode">
        /// The costcode name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="archived">
        /// Whether or not the costcode is archived.
        /// </param>
        /// <param name="createdOn">
        /// The created on date.
        /// </param>
        /// <param name="createdBy">
        /// The created by date.
        /// </param>
        /// <param name="modifiedOn">
        /// The modified on date.
        /// </param>
        /// <param name="modifiedBy">
        /// The modified by date.
        /// </param>
        /// <param name="userdefined">
        /// The userdefined fields list.
        /// </param>
        /// <param name="ownerEmployeeId">
        /// The owner employee id.
        /// </param>
        /// <param name="ownerTeamId">
        /// The ownerteamid.
        /// </param>
        /// <param name="ownerBudgetHolderId">
        /// The owner budget holder id.
        /// </param>
        public cCostCode(
            int costcodeId, 
            string costcode, 
            string description, 
            bool archived, 
            DateTime createdOn, 
            int createdBy, 
            DateTime? modifiedOn, 
            int? modifiedBy, 
            SortedList<int, object> userdefined, 
            int? ownerEmployeeId, 
            int? ownerTeamId, 
            int? ownerBudgetHolderId)
        {
            this.CostcodeId = costcodeId;
            this.Costcode = costcode;
            this.Description = description;
            this.Archived = archived;
            this.CreatedOn = createdOn;
            this.CreatedBy = createdBy;
            this.ModifiedOn = modifiedOn;
            this.ModifiedBy = modifiedBy;
            this.UserdefinedFields = userdefined;
            this.OwnerEmployeeId = ownerEmployeeId;
            this.OwnerTeamId = ownerTeamId;
            this.OwnerBudgetHolderId = ownerBudgetHolderId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the combined item key (element type, primary item id).
        /// </summary>
        public string CombinedItemKey
        {
            get
            {
                return string.Format("{0},{1}", (int)this.ElementType(), this.ItemPrimaryID());
            }
        }

        /// <summary>
        /// Gets a value indicating whether archived or not.
        /// </summary>
        public bool Archived { get; private set; }

        /// <summary>
        /// Gets the costcode.
        /// </summary>
        public string Costcode { get; private set; }

        /// <summary>
        /// Gets the costcode id.
        /// </summary>
        public int CostcodeId { get; set; }

        /// <summary>
        /// Gets the created by date.
        /// </summary>
        public int CreatedBy { get; private set; }

        /// <summary>
        /// Gets the created on date.
        /// </summary>
        public DateTime CreatedOn { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the modified by date.
        /// </summary>
        public int? ModifiedBy { get; private set; }

        /// <summary>
        /// Gets the modified on date.
        /// </summary>
        public DateTime? ModifiedOn { get; private set; }

        /// <summary>
        /// Gets the owner budget holder id.
        /// </summary>
        public int? OwnerBudgetHolderId { get; private set; }

        /// <summary>
        /// Gets the owner employee id.
        /// </summary>
        public int? OwnerEmployeeId { get; private set; }

        /// <summary>
        /// Gets the owner team id.
        /// </summary>
        public int? OwnerTeamId { get; private set; }

        /// <summary>
        /// Gets or sets the user defined fields.
        /// </summary>
        public SortedList<int, object> UserdefinedFields { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The element type.
        /// </summary>
        /// <returns>
        /// The <see cref="SpendManagementElement"/>.
        /// </returns>
        public SpendManagementElement ElementType()
        {
            return SpendManagementElement.CostCodes;
        }

        /// <summary>
        /// The item definition.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ItemDefinition()
        {
            return this.Costcode;
        }

        /// <summary>
        /// The item primary id.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ItemPrimaryID()
        {
            return this.CostcodeId;
        }

        /// <summary>
        /// The owner element type.
        /// </summary>
        /// <returns>
        /// The <see cref="SpendManagementElement"/>.
        /// </returns>
        public SpendManagementElement OwnerElementType()
        {
            return this.OwnerEmployeeId.HasValue
                       ? SpendManagementElement.Employees
                       : (this.OwnerBudgetHolderId.HasValue
                              ? SpendManagementElement.BudgetHolders
                              : (this.OwnerTeamId.HasValue ? SpendManagementElement.Teams : SpendManagementElement.None));
        }

        /// <summary>
        /// The Owner Id.
        /// </summary>
        /// <returns>
        /// The owner id if it exists. Null otherwise />.
        /// </returns>
        public int? OwnerId()
        {
            int ownerId = this.OwnerEmployeeId.HasValue
                              ? this.OwnerEmployeeId.Value
                              : (this.OwnerBudgetHolderId.HasValue
                                     ? this.OwnerBudgetHolderId.Value
                                     : (this.OwnerTeamId.HasValue ? this.OwnerTeamId.Value : 0));

            return ownerId;
        }

        #endregion
    }
}