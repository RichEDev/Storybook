namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;

    [Serializable()]
    public class cCustomEntityAccess
    {
        private int nCustomEntityID;
        private bool bCanView;
        private bool bCanAdd;
        private bool bCanEdit;
        private bool bCanDelete;
        private SortedList<int, cCustomEntityViewAccess> lstViewAccess;
        private SortedList<int, cCustomEntityFormAccess> lstFormAccess;

        /// <summary>
        /// Constructor for storing a role specific custom entity access details
        /// </summary>
        /// <param name="customEntityID">The custom entity id</param>
        /// <param name="canView">If this user can view records belonging to this element</param>
        /// <param name="canAdd">If this user can add records belonging to this element</param>
        /// <param name="canEdit">If this user can edit records belonging to this element</param>
        /// <param name="canDelete">If this user can delete records belonging to this element</param>
        /// <param name="formAccess"></param>
        /// <param name="viewAccess"></param>
        public cCustomEntityAccess(int customEntityID, bool canView, bool canAdd, bool canEdit, bool canDelete, SortedList<int, cCustomEntityViewAccess> viewAccess, SortedList<int, cCustomEntityFormAccess> formAccess)
        {
            this.nCustomEntityID = customEntityID;
            this.bCanView = canView;
            this.bCanAdd = canAdd;
            this.bCanEdit = canEdit;
            this.bCanDelete = canDelete;
            this.lstViewAccess = viewAccess;
            this.lstFormAccess = formAccess;
        }

        /// <summary>
        /// Returns the custom entity id
        /// </summary>
        public int CustomEntityID { get { return this.nCustomEntityID; } }

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

        /// <summary>
        /// List of view access details
        /// </summary>
        public SortedList<int, cCustomEntityViewAccess> ViewAccess
        {
            get { return this.lstViewAccess; }
            set { this.lstViewAccess = value; }
        }

        /// <summary>
        /// List of form access details
        /// </summary>
        public SortedList<int, cCustomEntityFormAccess> FormAccess
        {
            get { return this.lstFormAccess; }
            set { this.lstFormAccess = value; }
        }
    }
}