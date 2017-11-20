namespace SpendManagementLibrary
{
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
    using SpendManagementLibrary.GreenLight;

    /// <summary>
    /// Stores all the information on a view for the client side
    /// </summary>
    public struct sView
    {
        public string viewName;
        public string description;
        public bool builtIn;
        public int? nMenuid;
        public string MenuDescription;
        public bool ShowRecordCount;
        public List<int> MenuDisabledModuleIds;
        public bool allowAdd;
        public int addFormID;
        public bool allowEdit;
        public int editFormID;
        public bool allowDelete;
        public bool allowApproval;
        /// <summary>
        /// store allowArchive flag for the view
        /// </summary>
        public bool allowArchive;
        public string SortedColumnID;
        public SortDirection SortOrderDirection;
        public List<ListItem> formDropDownOptions;
        public ViewMenuIcon MenuIcon;
        public List<FormSelectionMapping> AddFormMappings;
        public List<FormSelectionMapping> EditFormMappings;
    }
}