namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// cCustomEntityForm class
    /// </summary>
    public class cCustomEntityForm
    {
        private int nEntityid;
        private int nFormid;
        private string sFormName;
        private string sDescription;
        private DateTime dtCreatedOn;
        private int nCreatedBy;
        private DateTime? dtModifiedOn;
        private int? nModifiedBy;
        private string sSaveAndDuplicateButtonText;
        private string saveAndNewButtonText;
        private string sSaveAndStayButtonText;
        private string sSaveButtonText;
        private string sCancelButtonText;
        private bool bShowBreadcrumbs;
        private bool bShowSaveAndDuplicate;
        private bool showSaveAndNew;
        private bool bShowSaveAndStay;
        private bool bShowSave;
        private bool bShowSubMenus;
        private bool bShowCancel;
        private SortedList<int, cCustomEntityFormTab> lstTabs;
        private SortedList<int, cCustomEntityFormSection> lstSections;
        private SortedList<int, cCustomEntityFormField> lstFields;

        /// <summary>
        /// cCustomEntityForm constructor
        /// </summary>
        /// <param name="formid">
        /// Custom Entity Form ID
        /// </param>
        /// <param name="entityid">
        /// Custom Entity ID
        /// </param>
        /// <param name="formname">
        /// Custom Entity Form Name
        /// </param>
        /// <param name="description">
        /// Form Description
        /// </param>
        /// <param name="showSave">
        /// Show the Save Button on the form
        /// </param>
        /// <param name="saveText">
        /// Text to display in the save button
        /// </param>
        /// <param name="showSaveAndDuplicate">
        /// Show the Save and New button on the form
        /// </param>
        /// <param name="saveAndDuplicateText">
        /// Text to display in the save and new button
        /// </param>
        /// <param name="showSaveAndStay">
        /// Show the Save and Stay button on the form
        /// </param>
        /// <param name="saveAndStayText">
        /// Text to display in the save and stay button
        /// </param>
        /// <param name="showCancel">
        /// Show the Cancel button on the form
        /// </param>
        /// <param name="cancelText">
        /// Text to display in the cancel button
        /// </param>
        /// <param name="showSubMenus">
        /// Show Sub-menu panel (Page Options) panel on the screen
        /// </param>
        /// <param name="showBreadcrumbs">
        /// Show the breadcrumbs on the form page
        /// </param>
        /// <param name="createdon">
        /// Date the custom entity form record was created
        /// </param>
        /// <param name="createdby">
        /// Employee ID who created the custom entity form record
        /// </param>
        /// <param name="modifiedon">
        /// Date the custom entity form was last modified
        /// </param>
        /// <param name="modifiedby">
        /// Employee ID who last modified the custom entity form
        /// </param>
        /// <param name="tabs">
        /// Collection of tabs that make up the form
        /// </param>
        /// <param name="sections">
        /// Collection of sections used on the form
        /// </param>
        /// <param name="fields">
        /// Fields to be rendered on the custom entity form
        /// </param>
        /// <param name="saveAndNewText">
        /// The save And New button Text.
        /// </param>
        /// <param name="showSaveAndNew">
        /// The show Save And New button.
        /// </param>
        /// <param name="checkDefaultValues">
        /// Go through each field within the form and check for default values
        /// </param>
        /// <param name="hideTorch"></param>
        /// <param name="hideAttachments"></param>
        /// <param name="hideAudiences"></param>
        /// <param name="builtIn"></param>
        /// <param name="systemCustomEntityFormId">A unique system identifier for this form</param>
        public cCustomEntityForm(int formid, int entityid, string formname, string description, bool showSave, string saveText, bool showSaveAndDuplicate, string saveAndDuplicateText, bool showSaveAndStay, string saveAndStayText, bool showCancel, string cancelText, bool showSubMenus, bool showBreadcrumbs, DateTime createdon, int createdby, DateTime? modifiedon, int? modifiedby, SortedList<int, cCustomEntityFormTab> tabs, SortedList<int, cCustomEntityFormSection> sections, SortedList<int, cCustomEntityFormField> fields, string saveAndNewText, bool showSaveAndNew, bool checkDefaultValues = false, bool hideTorch = false, bool hideAttachments = false, bool hideAudiences = false, bool builtIn = false, Guid? systemCustomEntityFormId = null)
        {
            this.nEntityid = entityid;
            this.nFormid = formid;
            this.sFormName = formname;
            this.sDescription = description;
            this.bShowBreadcrumbs = showBreadcrumbs;
            this.bShowCancel = showCancel;
            this.sCancelButtonText = cancelText;
            this.bShowSave = showSave;
            this.sSaveButtonText = saveText;
            this.bShowSaveAndDuplicate = showSaveAndDuplicate;
            this.sSaveAndDuplicateButtonText = saveAndDuplicateText;
            this.showSaveAndNew = showSaveAndNew;
            this.saveAndNewButtonText = saveAndNewText;
            this.bShowSaveAndStay = showSaveAndStay;
            this.sSaveAndStayButtonText = saveAndStayText;
            this.bShowSubMenus = showSubMenus;
            this.dtCreatedOn = createdon;
            this.nCreatedBy = createdby;
            this.dtModifiedOn = modifiedon;
            this.nModifiedBy = modifiedby;
            this.lstTabs = tabs;
            this.lstSections = sections;
            this.lstFields = fields;
            this.CheckDefaultValues = checkDefaultValues;
            this.HideTorch = hideTorch;
            this.HideAttachments = hideAttachments;
            this.HideAudiences = hideAudiences;
            this.BuiltIn = builtIn;
            this.SystemCustomEntityFormId = systemCustomEntityFormId;
        }

        /// <summary>
        /// getTabsForForm: Retrieve tabs associated with the current form
        /// </summary>
        /// <returns>Sorted list collection of tab definitions</returns>
        public SortedList<byte, cCustomEntityFormTab> getTabsForForm()
        {
            return this.sortTabs();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionheader"></param>
        /// <returns></returns>
        public cCustomEntityFormTab getTabBySection(string sectionheader)
        {
            return (cCustomEntityFormTab)(from x in this.sections.Values
                where x.headercaption == sectionheader
                select x.tab).FirstOrDefault();

            //foreach (cCustomEntityFormSection section in sections.Values)
            //{
            //    if (section.headercaption == sectionheader)
            //    {
            //        return section.tab;
            //    }
            //}
            //return null;
        }

        public cCustomEntityFormTab getTabByName(string name)
        {
            return (cCustomEntityFormTab)(from x in this.lstTabs.Values
                where x.headercaption == name
                select x).FirstOrDefault();

            //foreach (cCustomEntityFormTab tab in lstTabs.Values)
            //{
            //    if (tab.headercaption == name)
            //    {
            //        return tab;
            //    }
            //}
            //return null;
        }
        private SortedList<byte, cCustomEntityFormTab> sortTabs()
        {
            SortedList<byte, cCustomEntityFormTab> tabs = new SortedList<byte, cCustomEntityFormTab>((from x in this.lstTabs.Values
                select x).ToDictionary(a => a.order));
            //foreach (cCustomEntityFormTab tab in lstTabs.Values)
            //{
            //    tabs.Add(tab.order, tab);
            //}
            return tabs;
        }
        #region properties
        public int entityid
        {
            get { return this.nEntityid; }
        }
        public int formid
        {
            get { return this.nFormid; }
        }
        public string formname
        {
            get { return this.sFormName; }
        }
        public string description
        {
            get { return this.sDescription; }
        }
        public bool ShowSaveAndStayButton
        {
            get { return this.bShowSaveAndStay; }
        }
        public string SaveAndStayButtonText
        {
            get { return this.sSaveAndStayButtonText; }
        }
        public DateTime createdon
        {
            get { return this.dtCreatedOn; }
        }
        public int createdby
        {
            get { return this.nCreatedBy; }
        }
        public DateTime? modifiedon
        {
            get { return this.dtModifiedOn; }
        }
        public int? modifiedby
        {
            get { return this.nModifiedBy; }
        }
        public SortedList<int, cCustomEntityFormField> fields
        {
            get { return this.lstFields; }
        }
        public SortedList<int, cCustomEntityFormTab> tabs
        {
            get { return this.lstTabs; }
        }
        public SortedList<int, cCustomEntityFormSection> sections
        {
            get { return this.lstSections; }
        }

        /// <summary>
        /// Gets if the save button should be shown
        /// </summary>
        public bool ShowSaveButton
        {
            get { return this.bShowSave; }
        }

        /// <summary>
        /// Gets if the cancel button should be shown
        /// </summary>
        public bool ShowCancelButton
        {
            get { return this.bShowCancel; }
        }

        /// <summary>
        /// Gets what text will be displayed for this button action
        /// </summary>
        public string SaveButtonText
        {
            get { return this.sSaveButtonText; }
        }

        /// <summary>
        /// Gets what text will be displayed for this button action
        /// </summary>
        public string SaveAndDuplicateButtonText
        {
            get { return this.sSaveAndDuplicateButtonText; }
        }

        /// <summary>
        /// Gets what text will be displayed for this button action
        /// </summary>
        public string SaveAndNewButtonText
        {
            get
            {
                return this.saveAndNewButtonText;
            }
        }

        /// <summary>
        /// Gets if the Save and New button should be shown
        /// </summary>
        public bool ShowSaveAndNew
        {
            get
            {
                return this.showSaveAndNew;
            }
        }

        /// <summary>
        /// Gets what text will be displayed for this button action
        /// </summary>
        public string CancelButtonText
        {
            get { return this.sCancelButtonText; }
        }

        /// <summary>
        /// Determines if the page options menu is shown via the master page.
        /// </summary>
        public bool ShowSubMenus
        {
            get { return this.bShowSubMenus; }
            set { this.bShowSubMenus = value; }
        }

        /// <summary>
        /// Gets and sets Determines if the Save And Duplicate button will be displayed.
        /// </summary>
        public bool ShowSaveAndDuplicate
        {
            get { return this.bShowSaveAndDuplicate; }
            set { this.bShowSaveAndDuplicate = value; }
        }


        /// <summary>
        /// Determines if breadcrumbs are shown or not.
        /// </summary>
        public bool ShowBreadCrumbs
        {
            get { return this.bShowBreadcrumbs; }
            set { this.bShowBreadcrumbs = value; }
        }

        /// <summary>
        /// Gets a value indicating whether to check for default values on form fields.
        /// </summary>
        public bool CheckDefaultValues { get; private set; }

        /// <summary>
        /// Optionally hide the torch tab when the form is displayed.
        /// </summary>
        public bool HideTorch { get; set; }

        /// <summary>
        /// Optionally hide the attachments tab when the form is displayed.
        /// </summary>
        public bool HideAttachments { get; set; }

        /// <summary>
        /// Optionally hide the audience tab when the form is displayed.
        /// </summary>
        public bool HideAudiences { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this GreenLight Form is built-in A.K.A "System".
        /// Built in GreenLights (and their built-in sub components) can be copied between customer databases, although the functionality to do so is not provided within this product.
        /// Only "adminonly" employees can set this value, once set it cannot be un-set.
        /// </summary>
        public bool BuiltIn { get; set; }

        /// <summary>
        /// Gets or sets a unique system identifier for this form, for System GreenLights
        /// </summary>
        public Guid? SystemCustomEntityFormId { get; set; }

        #endregion
    }
}