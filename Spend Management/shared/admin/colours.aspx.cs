//===========================================================================
// This file was modified as part of an ASP.NET 2.0 Web project conversion.
// The class name was changed and the class modified to inherit from the abstract base class
// in file 'App_Code\Migrated\admin\Stub_colours_aspx_cs.cs'.
// During runtime, this allows other classes in your web application to bind and access
// the code-behind page using the abstract base class.
// The associated content page 'admin\colours.aspx' was also modified to refer to the new class name.
// For more information on this code pattern, please refer to http://go.microsoft.com/fwlink/?LinkId=46995
//===========================================================================


namespace Spend_Management
{
    using System;
    using System.Web.UI;

    using BusinessLogic;
    using BusinessLogic.AccountProperties;
    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;
    using BusinessLogic.Modules;

    /// <summary>
    /// Summary description for colours.
    /// </summary>
    public partial class colours : Page
    {
        /// <summary>
        /// An instance of <see cref="IDataFactory{IAccountProperty,AccountPropertyCacheKey}"/> to get a <see cref="IAccountProperty"/>
        /// </summary>
        [Dependency]
        public IDataFactory<IAccountProperty, AccountPropertyCacheKey> AccountPropertiesFactory { get; set; }

        /// <summary>
        /// The Header Background Colour
        /// </summary>
        public string headerbgcolour;

        /// <summary>
        /// The Header Text Colour
        /// </summary>
        public string headertxtcolour;

        /// <summary>
        /// The Page Title Text Colour
        /// </summary>
        public string pagetitletxtcolour;

        /// <summary>
        /// The Section Heading Underline Colour
        /// </summary>
        public string sectionheadingunderlinecolour;

        /// <summary>
        /// The Section Heading Text Colour
        /// </summary>
        public string sectionheadingextcolour;

        /// <summary>
        /// The Field Text Colour
        /// </summary>
        public string fieldtxtcolour;

        /// <summary>
        /// The Page Options Background Colour
        /// </summary>
        public string pageoptionsbgcolour;

        /// <summary>
        /// The Page Options text Colour
        /// </summary>
        public string pageoptionstxtcolour;

        /// <summary>
        /// The Table Header Background Colour
        /// </summary>
        public string tableheaderbgcolour;

        /// <summary>
        /// The Table Header text Colour
        /// </summary>
        public string tableheadertxtcolour;

        /// <summary>
        /// The Tab Option Background Colour
        /// </summary>
        public string taboptionbgcolour;

        /// <summary>
        /// The Tab Option Text Colour
        /// </summary>
        public string taboptiontxtcolour;

        /// <summary>
        /// The Row Background Colour
        /// </summary>
        public string rowbgcolour;

        /// <summary>
        /// The Row Text Colour
        /// </summary>
        public string rowtxtcolour;

        /// <summary>
        /// The Alternate Row Background Colour
        /// </summary>
        public string altrowbgcolour;

        /// <summary>
        /// The Alternate Row Text Colour
        /// </summary>
        public string altrowtxtcolour;

        /// <summary>
        /// The Menu Option Hover Text Colour
        /// </summary>
        public string menuoptionhovertxtcolour;

        /// <summary>
        /// The Menu Option Standard Text Colour
        /// </summary>
        public string menuoptionstdtxtcolour;

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Master.enablenavigation = false;

            if (IsPostBack == false)
            {
                Title = "Colours";
                Master.PageSubTitle = "Colour Details";

                CurrentUser user = cMisc.GetCurrentUser();
                switch (user.CurrentActiveModule)
                {
                    case Modules.Contracts:
                        Master.helpid = 1222;
                        break;
                    default:
                        Master.helpid = 1036;
                        break;
                }

                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Colours, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cColours clscolours = new cColours(user.AccountID, user.CurrentSubAccountId, user.CurrentActiveModule);
                txtheaderbg.Text = clscolours.headerBGColour.Replace("#", "");
                txtheadertxt.Text = clscolours.headerBreadcrumbTxtColour.Replace("#", "");
                txtpagetitletxt.Text = clscolours.pageTitleTxtColour.Replace("#", "");
                txtsectionheadingunderline.Text = clscolours.sectionHeadingUnderlineColour.Replace("#", "");
                txtsectionheadingtxt.Text = clscolours.sectionHeadingTxtColour.Replace("#", "");
                txtfieldtext.Text = clscolours.fieldTxtColour.Replace("#", "");

                txtpageoptionsbg.Text = clscolours.pageOptionsBGColour.Replace("#", "");
                txtpageoptionstxt.Text = clscolours.pageOptionsTxtColour.Replace("#", "");
                txttableheaderbg.Text = clscolours.tableHeaderBGColour.Replace("#", "");
                txttableheadertxt.Text = clscolours.tableHeaderTxtColour.Replace("#", "");
                txttaboptionbg.Text = clscolours.tabOptionBGColour.Replace("#", "");
                txttaboptiontxt.Text = clscolours.tabOptionTxtColour.Replace("#", "");

                txttablerow.Text = clscolours.rowBGColour.Replace("#", "");
                txtRowFGColour.Text = clscolours.rowTxtColour.Replace("#", "");
                txtalternatetablerow.Text = clscolours.altRowBGColour.Replace("#", "");
                txtAltRowFGColour.Text = clscolours.altRowTxtColour.Replace("#", "");

                txthoverbg.Text = clscolours.menuOptionHoverTxtColour.Replace("#", "");
                txtPageOptionColour.Text = clscolours.menuOptionStdTxtColour.Replace("#", "");

                txtTooltipbg.Text = clscolours.tooltipBgColour.Replace("#", "");
                txtTooltipTextColour.Text = clscolours.tooltipTextColour.Replace("#", "");

                txtGreenLightField.Text = clscolours.greenLightFieldColour.Replace("#", "");
                txtGreenLightSectionText.Text = clscolours.greenLightSectionTextColour.Replace("#", "");
                txtGreenLightSectionBackground.Text = clscolours.greenLightSectionBackgroundColour.Replace("#", "");
                txtGreenLightSectionUnderline.Text = clscolours.greenLightSectionUnderlineColour.Replace("#", "");

                string btnCancelNavigateUrl = "/";

                switch (user.CurrentActiveModule)
                {
                    case Modules.SmartDiligence:
                    case Modules.SpendManagement:
                    case Modules.Contracts:
                        btnCancelNavigateUrl = "/MenuMain.aspx?menusection=tailoring";
                        break;
                    case Modules.Expenses:
                        btnCancelNavigateUrl = "/tailoringmenu.aspx";
                        break;
                    default:
                        btnCancelNavigateUrl = "/tailoringmenu.aspx";
                        break;
                }

                btnCancel.OnClientClick = "window.location.href = appPath + '" + btnCancelNavigateUrl + "'; return false;";

            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

        }
        #endregion

        void btnSave_Click(object sender, EventArgs e)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();

            string headerbgcolour;
            string headerbreadcrumbtxtcolour;
            string pagetitletxtcolour;
            string sectionheadingunderlinecolour;
            string sectionheadingtxtcolour;
            string fieldtxtcolour;
            string pageoptionsbgcolour;
            string pageoptionstxtcolour;
            string tableheaderbgcolour;
            string tableheadertxtcolour;
            string taboptionbgcolour;
            string taboptiontxtcolour;
            string rowbgcolour;
            string rowtxtcolour;
            string altrowbgcolour;
            string altrowtxtcolour;
            string menuoptionhovertxtcolour;
            string menuoptionstdtxtcolour;
            string tooltipBgColour;
            string tooltipTextColour;
            string greenlightfieldcolour;
            string greenlightsectiontextcolour;
            string greenlightsectionbackgroundcolour;
            string greenlightsectionunderlinecolour;

            headerbgcolour = txtheaderbg.Text.Replace("#", "");
            headerbreadcrumbtxtcolour = txtheadertxt.Text.Replace("#", "");

            pagetitletxtcolour = txtpagetitletxt.Text.Replace("#", "");

            sectionheadingunderlinecolour = txtsectionheadingunderline.Text.Replace("#", "");
            sectionheadingtxtcolour = txtsectionheadingtxt.Text.Replace("#", "");

            fieldtxtcolour = txtfieldtext.Text.Replace("#", "");

            pageoptionsbgcolour = txtpageoptionsbg.Text.Replace("#", "");
            pageoptionstxtcolour = txtpageoptionstxt.Text.Replace("#", "");

            tableheaderbgcolour = txttableheaderbg.Text.Replace("#", "");
            tableheadertxtcolour = txttableheadertxt.Text.Replace("#", "");

            taboptionbgcolour = txttaboptionbg.Text.Replace("#", "");
            taboptiontxtcolour = txttaboptiontxt.Text.Replace("#", "");

            rowbgcolour = txttablerow.Text.Replace("#", "");
            rowtxtcolour = txtRowFGColour.Text.Replace("#", "");
            altrowbgcolour = txtalternatetablerow.Text.Replace("#", "");
            altrowtxtcolour = txtAltRowFGColour.Text.Replace("#", "");

            menuoptionhovertxtcolour = txthoverbg.Text.Replace("#", "");
            menuoptionstdtxtcolour = txtPageOptionColour.Text.Replace("#", "");

            tooltipBgColour = txtTooltipbg.Text.Replace("#", "");
            tooltipTextColour = txtTooltipTextColour.Text.Replace("#", "");

            greenlightfieldcolour = txtGreenLightField.Text.Replace("#", "");
            greenlightsectiontextcolour = txtGreenLightSectionText.Text.Replace("#", "");
            greenlightsectionbackgroundcolour = txtGreenLightSectionBackground.Text.Replace("#", "");
            greenlightsectionunderlinecolour = txtGreenLightSectionUnderline.Text.Replace("#", "");

            cColours clscolours = new cColours((int)ViewState["accountid"], curUser.CurrentSubAccountId, curUser.CurrentActiveModule);
            clscolours.UpdateColours(headerbgcolour, headerbreadcrumbtxtcolour, pagetitletxtcolour, sectionheadingunderlinecolour, sectionheadingtxtcolour, fieldtxtcolour, 
                pageoptionsbgcolour, pageoptionstxtcolour, tableheaderbgcolour, tableheadertxtcolour, taboptionbgcolour, taboptiontxtcolour, rowbgcolour, rowtxtcolour, 
                altrowbgcolour, altrowtxtcolour, menuoptionhovertxtcolour, menuoptionstdtxtcolour, tooltipBgColour, tooltipTextColour, greenlightfieldcolour, 
                greenlightsectiontextcolour, greenlightsectionbackgroundcolour, greenlightsectionunderlinecolour, this.AccountPropertiesFactory);

            switch (curUser.CurrentActiveModule)
            {
                case Modules.SmartDiligence:
                case Modules.SpendManagement:
                case Modules.Contracts:
                    Response.Redirect("~/MenuMain.aspx?menusection=tailoring", true);
                    break;
                case Modules.Expenses:
                    Response.Redirect("~/tailoringmenu.aspx", true);
                    break;
                default:
                    Response.Redirect("~/tailoringmenu.aspx", true);
                    break;
            }
        }
    }
}