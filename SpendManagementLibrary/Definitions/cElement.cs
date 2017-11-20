using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    public class cElement
    {
        public bool AccessRolesApplicable { get; set; }
        private int nElementID;
        private int nElementCategoryID;
        private string sElementName;
        private string sElementFriendlyName;
        private string sElementDescription;
        private bool bAccessRolesCanAdd;
        private bool bAccessRolesCanEdit;
        private bool bAccessRolesCanDelete;

        /// <summary>
        /// Constructor for cElement
        /// </summary>
        /// <param name="elementID">elementID</param>
        /// <param name="elementCategoryID">elementCategoryID</param>
        /// <param name="elementName">element name</param>
        /// <param name="elementDescription">element description</param>
        /// <param name="canAdd">If this element can have add access roles</param>
        /// <param name="canEdit">If this element can have edit access roles</param>
        /// <param name="canDelete">If this element can have delete access roles</param>
        /// <param name="friendlyName">A human friendly name</param>
        /// <param name="accessRolesApplicable">True if used in access Roles</param>
        public cElement(int elementID, int elementCategoryID, string elementName, string elementDescription, bool canAdd, bool canEdit, bool canDelete, string friendlyName, bool accessRolesApplicable)
        {
            this.AccessRolesApplicable = accessRolesApplicable;
            this.nElementID = elementID;
            this.nElementCategoryID = elementCategoryID;
            this.sElementName = elementName;
            this.sElementFriendlyName = friendlyName;
            this.sElementDescription = elementDescription;
            this.bAccessRolesCanAdd = canAdd;
            this.bAccessRolesCanEdit = canEdit;
            this.bAccessRolesCanDelete = canDelete;
        }

        /// <summary>
        /// Gets the elementID for this element
        /// </summary>
        public int ElementID { get { return nElementID; } }

        /// <summary>
        /// Gets the elements categoryID for this element
        /// </summary>
        public int ElementCategoryID { get { return nElementCategoryID; } }

        /// <summary>
        /// Gets the element name for this element
        /// </summary>
        public string Name { get { return sElementName; } }

        /// <summary>
        /// Gets the description for this element
        /// </summary>
        public string Description { get { return sElementDescription; } }

        /// <summary>
        /// If this element can have edit access set with AccessRoles
        /// </summary>
        public bool AccessRolesCanEdit { get { return bAccessRolesCanEdit; } }

        /// <summary>
        /// If this element can have add access set with AccessRoles
        /// </summary>
        public bool AccessRolesCanAdd { get { return bAccessRolesCanAdd; } }

        /// <summary>
        /// If this element can have delete access set with AccessRoles
        /// </summary>
        public bool AccessRolesCanDelete { get { return bAccessRolesCanDelete; } }

        /// <summary>
        /// Gets the human friendly name for this element
        /// </summary>
        public string FriendlyName { get { return sElementFriendlyName; } }
    }
}
