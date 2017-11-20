namespace SpendManagementApi.Models.Types
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Attributes.Validation;
    using Interfaces;
    using SpendManagementLibrary;

    /// <summary>
    /// Defines, for a user, the appropriate access types for a given Element.
    /// </summary>
    public class ElementAccessDetail : IApiFrontForDbObject<cElementAccess, ElementAccessDetail>
    {
        #region Constructor

        /// <summary>
        /// Creates a blank ElementAccessDetail.
        /// </summary>
        public ElementAccessDetail() { }

        #endregion

        #region Properties

        /// <summary>
        /// The numeric value of the <see cref="SpendManagementElement">SpendManagementElement</see>.
        /// </summary>
        [Required, IdIsValidEnumValue(typeof (SpendManagementElement))]
        public int Id { get; set; }
        
        /// <summary>
        /// The name of the <see cref="SpendManagementElement">SpendManagementElement</see>.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The Access Rights for this ElementAccess.
        /// </summary>
        [Required]
        public AccessRights AccessRights { get; set; }

        #endregion

        #region Public Methods

        public ElementAccessDetail From(cElementAccess dbType, IActionContext actionContext)
        {
            Id = dbType.ElementID;
            Label = Enum.GetName(typeof (SpendManagementElement), Id);
            AccessRights = new AccessRights
            {
                CanView = dbType.CanView,
                CanAdd = dbType.CanAdd,
                CanEdit = dbType.CanEdit,
                CanDelete = dbType.CanDelete
            };
            
            return this;
        }

        public cElementAccess To(IActionContext actionContext)
        {
            return new cElementAccess(Id, AccessRights.CanView, AccessRights.CanAdd, AccessRights.CanEdit, AccessRights.CanDelete);
        }

        #endregion
    }
}