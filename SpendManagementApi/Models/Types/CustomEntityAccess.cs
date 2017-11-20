namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Types.Employees;
    using SpendManagementLibrary;

    /// <summary>
    /// A group of permissions that correspond to operations the user can perform.
    /// </summary>
    public class AccessRights
    {
        #region Private

        private bool _canView;

        #endregion Private

        #region Constructors

        /// <summary>
        /// Creates an AccessRights object. All properties are default false.
        /// </summary>
        public AccessRights() { }

        /// <summary>
        /// Creates an AccessRights object. Pass in the properties to activate.
        /// </summary>
        /// <param name="canView">Whether the <see cref="Employee">Employee</see> can View the element / area.</param>
        /// <param name="canAdd">Whether the <see cref="Employee">Employee</see> can Add the element / area.</param>
        /// <param name="canEdit">Whether the <see cref="Employee">Employee</see> can Edit the element / area.</param>
        /// <param name="canDelete">Whether the <see cref="Employee">Employee</see> can Delete the element / area.</param>
        public AccessRights(bool canView, bool canAdd, bool canEdit, bool canDelete)
        {
            CanAdd = canAdd;
            CanEdit = canEdit;
            CanDelete = canDelete;
            CanView = canView;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Whether the <see cref="Employee">Employee</see> can View the element / area.
        /// </summary>
        public bool CanView
        {
            get { return CanAdd || CanEdit || CanDelete || _canView; }
            set { _canView = value; }
        }

        /// <summary>
        /// Whether the <see cref="Employee">Employee</see> can Add the element / area.
        /// </summary>
        public bool CanAdd { get; set; }

        /// <summary>
        /// Whether the <see cref="Employee">Employee</see> can Edit the element / area.
        /// </summary>
        public bool CanEdit { get; set; }

        /// <summary>
        /// Whether the <see cref="Employee">Employee</see> can Delete the element / area.
        /// </summary>
        public bool CanDelete { get; set; }

        #endregion Public Properties
    }


    /// <summary>
    /// Defines the permissions for accessing a custom entity / Greenlight
    /// </summary>
    public class CustomEntityGroupAccess : IApiFrontForDbObject<cCustomEntityAccess, CustomEntityGroupAccess>
    {
        #region Public Properties

        /// <summary>
        /// The Id of the Greenlight group
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or Sets the Name of the Greenlight Entity
        /// </summary>
        public string CustomEntityName { get; set; }

        /// <summary>
        /// The Access Rights for this CustomEntityAccess
        /// </summary>
        [Required]
        public AccessRights AccessRights { get; set; }

        /// <summary>
        /// A List of ElementAccess objects for this GreenLight custom object.
        /// </summary>
        public IList<CustomEntityElementAccess> ElementAccess { get; set; }

        #endregion Public Properties

        #region IApiFrontForDbObject

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public CustomEntityGroupAccess From(cCustomEntityAccess dbType, IActionContext actionContext)
        {
            var converted = new CustomEntityGroupAccess
            {
                Id = dbType.CustomEntityID,
                AccessRights = new AccessRights
                {
                    CanView = dbType.CanView,
                    CanAdd = dbType.CanAdd,
                    CanEdit = dbType.CanEdit,
                    CanDelete = dbType.CanDelete
                },
                ElementAccess = new List<CustomEntityElementAccess>()
            };

            foreach (var dbForm in dbType.FormAccess)
            {
                converted.ElementAccess.Add(new CustomEntityFormAccess().From(dbForm.Value, actionContext));
            }

            foreach (var dbView in dbType.ViewAccess)
            {
                converted.ElementAccess.Add(new CustomEntityViewAccess().From(dbView.Value, actionContext));
            }

            return converted;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns></returns>
        public cCustomEntityAccess To(IActionContext actionContext)
        {
            var sortedView = new SortedList<int, cCustomEntityViewAccess>();
            var sortedForm = new SortedList<int, cCustomEntityFormAccess>();

            foreach (var element in ElementAccess)
            {
                if (element.Type == CustomEntityElementType.Form)
                {
                    sortedForm.Add(element.Id, element.ToFormAccess().To(actionContext));
                }

                if (element.Type == CustomEntityElementType.View)
                {
                    sortedView.Add(element.Id, element.ToViewAccess().To(actionContext));
                }
            }

            return new cCustomEntityAccess(Id,
                AccessRights.CanView, AccessRights.CanAdd,
                AccessRights.CanEdit, AccessRights.CanDelete,
                sortedView, sortedForm);
        }

        #endregion IApiFrontForDbObject
    }

    /// <summary>
    /// Defines the permissions for accessing a Greenlight element.
    /// </summary>
    public class CustomEntityElementAccess
    {
        #region Public Properties

        /// <summary>
        /// Returns the custom entity Form id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Returns the related custom entity's Id
        /// </summary>
        public int CustomEntityId { get; set; }

        /// <summary>
        /// The type of this element (form/view etc)
        /// </summary>
        public CustomEntityElementType Type { get; set; }

        /// <summary>
        /// The Access Rights for this CustomEntityAccess
        /// </summary>
        public AccessRights AccessRights { get; set; }

        /// <summary>
        /// Converts this base class to a CustomEntityFormAccess.
        /// It gets round covariance issue from not implementing an entire custom binder, 
        /// just to handle the difference between these two types.
        /// </summary>
        /// <returns>A new CustomEntityFormAccess with it's properties matching this.</returns>
        public CustomEntityFormAccess ToFormAccess()
        {
            return new CustomEntityFormAccess
            {
                Id = Id,
                Type = Type,
                CustomEntityId = CustomEntityId,
                AccessRights = AccessRights
            };
        }

        /// <summary>
        /// Converts this base class to a CustomEntityViewAccess.
        /// It gets round covariance issue from not implementing an entire custom binder, 
        /// just to handle the difference between these two types.
        /// </summary>
        /// <returns>A new CustomEntityViewAccess with it's properties matching this.</returns>
        public CustomEntityViewAccess ToViewAccess()
        {
            return new CustomEntityViewAccess
            {
                Id = Id,
                Type = Type,
                CustomEntityId = CustomEntityId,
                AccessRights = AccessRights
            };
        }


        #endregion Public Properties
    }

    /// <summary>
    /// Defines the permissions for accessing a Greenlight form.
    /// </summary>
    public class CustomEntityFormAccess : CustomEntityElementAccess,
        IApiFrontForDbObject<cCustomEntityFormAccess, CustomEntityElementAccess>
    {
        #region IApiFrontForDbObject

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="actionContext">The IActionContext.</param>
        /// <returns></returns>
        public CustomEntityElementAccess From(cCustomEntityFormAccess dbType, IActionContext actionContext)
        {
            Id = dbType.CustomEntityFormID;
            Type = CustomEntityElementType.Form;
            CustomEntityId = dbType.CustomEntityID;
            AccessRights = new AccessRights(dbType.CanView, dbType.CanAdd, dbType.CanEdit, dbType.CanDelete);
            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns></returns>
        public cCustomEntityFormAccess To(IActionContext actionContext)
        {
            return new cCustomEntityFormAccess(CustomEntityId, Id, AccessRights.CanView, AccessRights.CanAdd, AccessRights.CanEdit, AccessRights.CanDelete);
        }

        #endregion IApiFrontForDbObject
    }

    /// <summary>
    /// Defines the permissions for accessing a Greenlight view.
    /// </summary>
    public class CustomEntityViewAccess : CustomEntityElementAccess, IApiFrontForDbObject<cCustomEntityViewAccess, CustomEntityViewAccess>
    {
        #region IApiFrontForDbObject

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="actionContext">The IActionContext</param>
        /// <returns></returns>
        public CustomEntityViewAccess From(cCustomEntityViewAccess dbType, IActionContext actionContext)
        {
            Id = dbType.CustomEntityViewID;
            CustomEntityId = dbType.CustomEntityID;
            Type = CustomEntityElementType.View;
            AccessRights = new AccessRights(dbType.CanView, dbType.CanAdd, dbType.CanEdit, dbType.CanDelete);
            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns></returns>
        public cCustomEntityViewAccess To(IActionContext actionContext)
        {
            return new cCustomEntityViewAccess(CustomEntityId, Id, AccessRights.CanView, AccessRights.CanAdd, AccessRights.CanEdit, AccessRights.CanDelete);
        }

        #endregion IApiFrontForDbObject
    }
    
}