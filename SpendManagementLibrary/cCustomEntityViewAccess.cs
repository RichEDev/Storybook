﻿namespace SpendManagementLibrary
{
    using System;

    /// <summary>
    /// Access role details for a user about a specific view
    /// </summary>
    /// 
    [Serializable()]
    public class cCustomEntityViewAccess
    {
        private int nCustomEntityID;
        private int nCustomEntityViewID;
        private bool bCanView;
        private bool bCanAdd;
        private bool bCanEdit;
        private bool bCanDelete;

        /// <summary>
        /// Constructor for storing a role specific custom entity view access details
        /// </summary>
        /// <param name="customEntityID"></param>
        /// <param name="customEntityViewID"></param>
        /// <param name="canView">If this user can view records belonging to this element</param>
        /// <param name="canAdd">If this user can add records belonging to this element</param>
        /// <param name="canEdit">If this user can edit records belonging to this element</param>
        /// <param name="canDelete">If this user can delete records belonging to this element</param>
        public cCustomEntityViewAccess(int customEntityID, int customEntityViewID, bool canView, bool canAdd, bool canEdit, bool canDelete)
        {
            this.nCustomEntityID = customEntityID;
            this.nCustomEntityViewID = customEntityViewID;
            this.bCanView = canView;
            this.bCanAdd = canAdd;
            this.bCanEdit = canEdit;
            this.bCanDelete = canDelete;
        }

        /// <summary>
        /// Returns the related custom entity id
        /// </summary>
        public int CustomEntityID { get { return this.nCustomEntityID; } }

        /// <summary>
        /// Returns the custom entity view id
        /// </summary>
        public int CustomEntityViewID { get { return this.nCustomEntityViewID; } }

        /// <summary>
        /// Boolean stating if a user can view or not, if CanEdit, CanAdd or CanDelete are true CanView will return true
        /// </summary>
        public bool CanView
        {
            get
            {
                if (this.bCanAdd == true || this.bCanEdit == true || this.bCanDelete == true)
                {
                    return true;
                }
                else
                {
                    return this.bCanView;
                }
            }

            set { this.bCanView = value; }
        }

        /// <summary>
        /// Boolean stating if a user can add or not
        /// </summary>
        public bool CanAdd
        {
            get { return this.bCanAdd; }
            set { this.bCanAdd = value; }
        }

        /// <summary>
        /// Boolean stating if a user can edit or not
        /// </summary>
        public bool CanEdit
        {
            get { return this.bCanEdit; }
            set { this.bCanEdit = value; }
        }

        /// <summary>
        /// Boolean stating if a user can delete or not
        /// </summary>
        public bool CanDelete
        {
            get { return this.bCanDelete; }
            set { this.bCanDelete = value; }
        }

    }
}