namespace SpendManagementLibrary
{
    using System.Collections.Generic;

    /// <summary>
    /// Stores all the form details and controls
    /// </summary>
    public struct sForm
    {
        public string formName;
        public string description;
        public bool showSave;
        public string saveButtonText;
        public bool showSaveAndDuplicate;
        public string saveAndDuplicateButtonText;
        public bool showSaveAndNew;
        public string saveAndNewButtonText;
        public bool showSaveAndStay;
        public string saveAndStayButtonText;
        public bool showCancel;
        public string cancelButtonText;
        public bool showSubMenus;
        public bool showBreadcrumbs;
        public bool hideTorch;
        public bool hideAttachments;
        public bool hideAudiences;
        public bool builtIn;
        public List<sCEFormTab> tabs;
    }
}