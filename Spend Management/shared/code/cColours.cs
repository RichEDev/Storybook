namespace Spend_Management
{
    using BusinessLogic.AccountProperties;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Enums;
    using BusinessLogic.GeneralOptions;
    using BusinessLogic.Modules;

    using SpendManagementLibrary;

    /// <summary>
    /// Summary description for cColours.
    /// </summary>
    public class cColours
    {
        private int nAccountID;
        private int nSubAccountID;

        // Header with breadcrumbs
        /// <summary>
        /// The Header background colour
        /// </summary>
        public static string sHeaderBGColour; // = "#004990";
        /// <summary>
        /// The header breadcumbs text colour
        /// </summary>
        public static string sHeaderBreadcrumbTxtColour; // = "#FFFFFF";
        // Page title
        /// <summary>
        /// The Page Title Text Colour
        /// </summary>
        public static string sPageTitleTxtColour; // = "#4A65A0";
        /// <summary>
        /// 
        /// </summary>
        public static string sSectionHeadingUnderlineColour; // = "#FFFFFF";
        /// <summary>
        /// 
        /// </summary>
        public static string sSectionHeadingTxtColour; // = "#FFFFFF";
        /// <summary>
        /// 
        /// </summary>
        public static string sPageOptionsBGColour; // = "#FFFFFF";
        /// <summary>
        /// 
        /// </summary>
        public static string sPageOptionsTxtColour; // = "#FFFFFF";
        /// <summary>
        /// 
        /// </summary>
        public static string sTableHeaderBGColour; // = "#FFFFFF";
        /// <summary>
        /// 
        /// </summary>
        public static string sTableHeaderTxtColour; // = "#FFFFFF";
        /// <summary>
        /// 
        /// </summary>
        public static string sTabOptionBGColour; // = "#FFFFFF";
        /// <summary>
        /// 
        /// </summary>
        public static string sTabOptionTxtColour; // = "#FFFFFF";
        // grid colours
        /// <summary>
        /// 
        /// </summary>
        public static string sRowTxtColour;
        /// <summary>
        /// 
        /// </summary>
        public static string sRowBGColour; // = "#FFFFFF";
        /// <summary>
        /// 
        /// </summary>
        public static string sAlternateRowTxtColour;
        /// <summary>
        /// 
        /// </summary>
        public static string sAlternateRowBGColour; // = "#C7DFF7";

        // labels
        /// <summary>
        /// 
        /// </summary>
        public static string sFieldTxtColour; // = "#FFFFFF";

        // menu item titles and page links
        /// <summary>
        /// 
        /// </summary>
        public static string sMenuOptionHoverTxtColour; // = "#B6677B";
        /// <summary>
        /// 
        /// </summary>
        public static string sMenuOptionStandardTxtColour;
        // Tooltip fill colour
        /// <summary>
        ///
        /// </summary>
        public static string sTooltipBgColour;
        // Tooltip text colour
        /// <summary>
        ///
        /// </summary>
        public static string sTooltipTextColour;
        // New GreenLight theme colours
        /// <summary>
        /// This is the Field Label text colour for the GreenLight theme
        /// </summary>
        public static string sGreenLightFieldColour;
        /// <summary>
        /// This is the Section Title text colour for the GreenLight theme
        /// </summary>
        public static string sGreenLightSectionTextColour;
        /// <summary>
        /// This is the Section background colour for the GreenLight theme
        /// </summary>
        public static string sGreenLightSectionBackgroundColour;
        /// <summary>
        /// This is the Section underline colour for the GreenLight theme
        /// </summary>
        public static string sGreenLightSectionUnderlineColour;


        private cAccountProperties subAccProp;

        #region defaults
        public string defaultHeaderBGColour
        {
            get { return sHeaderBGColour; }
        }
        public string defaultHeaderBreadcrumbTxtColour
        {
            get { return sHeaderBreadcrumbTxtColour; }
        }
        public string defaultPageTitleTxtColour
        {
            get { return sPageTitleTxtColour; }
        }
        public string defaultSectionHeadingUnderlineColour
        {
            get { return sSectionHeadingUnderlineColour; }
        }
        public string defaultSectionHeadingTxtColour
        {
            get { return sSectionHeadingTxtColour; }
        }
        public string defaultPageOptionsBGColour
        {
            get { return sPageOptionsBGColour; }
        }
        public string defaultPageOptionsTxtColour
        {
            get { return sPageOptionsTxtColour; }
        }
        public string defaultTableHeaderBGColour
        {
            get { return sTableHeaderBGColour; }
        }
        public string defaultTableHeaderTxtColour
        {
            get { return sTableHeaderTxtColour; }
        }
        public string defaultTabOptionBGColour
        {
            get { return sTabOptionBGColour; }
        }
        public string defaultTabOptionTxtColour
        {
            get { return sTabOptionTxtColour; }
        }
        public string defaultRowBGColour
        {
            get { return sRowBGColour; }
        }
        public string defaultRowTxtColour
        {
            get { return sRowTxtColour; }
        }
        public string defaultAltRowBGColour
        {
            get { return sAlternateRowBGColour; }
        }
        public string defaultAltRowTxtColour
        {
            get { return sAlternateRowTxtColour; }
        }
        public string defaultFieldTxt
        {
            get { return sFieldTxtColour; }
        }
        public string defaultMenuOptionHoverTxtColour
        {
            get { return sMenuOptionHoverTxtColour; }
        }
        public string defaultTooltipBgColour
        {
            get { return sTooltipBgColour; }
        }
        public string defaultTooltipTextColour
        {
            get { return sTooltipTextColour; }
        }
        public string defaultMenuOptionStandardTxtColour
        {
            get { return sMenuOptionStandardTxtColour; }
        }
        public string defaultGreenLightFieldColour
        {
            get { return sGreenLightFieldColour; }
        }
        public string defaultGreenLightSectionTextColour
        {
            get { return sGreenLightSectionTextColour; }
        }
        public string defaultGreenLightSectionBackgroundColour
        {
            get { return sGreenLightSectionBackgroundColour; }
        }
        public string defaultGreenLightSectionUnderlineColour
        {
            get { return sGreenLightSectionUnderlineColour; }
        }
        #endregion

        public cColours(int accountID, int subAccountID, Modules activeModule)
        {
            nAccountID = accountID;
            nSubAccountID = subAccountID;
            SetDefaultColours(activeModule);

            RefreshColours();
        }

        public cColours(Modules activeModule)
        {
            SetDefaultColours(activeModule);
        }

        private static void SetDefaultColours(Modules activeModule)
        {
            switch (activeModule)
            {
                case Modules.Contracts:
                    sHeaderBGColour = "#00A0AF".ToUpper();
                    sHeaderBreadcrumbTxtColour = "#FFFFFF".ToUpper();

                    sPageTitleTxtColour = "#00A0AF".ToUpper();

                    sSectionHeadingUnderlineColour = "#00A0AF".ToUpper();
                    sSectionHeadingTxtColour = "#333333".ToUpper();

                    sPageOptionsBGColour = "#00A0AF".ToUpper();
                    sPageOptionsTxtColour = "#FFFFFF".ToUpper();

                    sTableHeaderBGColour = "#00A0AF".ToUpper();
                    sTableHeaderTxtColour = "#FFFFFF".ToUpper();

                    sTabOptionBGColour = "#00A0AF".ToUpper();
                    sTabOptionTxtColour = "#FFFFFF".ToUpper();

                    sRowBGColour = "#FFFFFF".ToUpper();
                    sRowTxtColour = "#333333".ToUpper();
                    sAlternateRowBGColour = "#D1F2F5".ToUpper();
                    sAlternateRowTxtColour = "#333333".ToUpper();


                    sFieldTxtColour = "#FFFFFF".ToUpper();

                    sMenuOptionHoverTxtColour = "#00A0AF".ToUpper();
                    sMenuOptionStandardTxtColour = "#00A0AF".ToUpper();

                    sTooltipBgColour = "#00A0AF".ToUpper();
                    sTooltipTextColour = "#FFFFFF".ToUpper();

                    sGreenLightFieldColour = "#333333".ToUpper();
                    sGreenLightSectionTextColour = "#333333".ToUpper();
                    sGreenLightSectionBackgroundColour = "#FFFFFF".ToUpper();
                    sGreenLightSectionUnderlineColour = "#00A0AF".ToUpper();
                    break;
                case Modules.Expenses:
                    sHeaderBGColour = "#004990".ToUpper();
                    sHeaderBreadcrumbTxtColour = "#FFFFFF".ToUpper();

                    sPageTitleTxtColour = "#53B9EC".ToUpper();

                    sSectionHeadingUnderlineColour = "#53B9EC".ToUpper();
                    sSectionHeadingTxtColour = "#333333".ToUpper();

                    sPageOptionsBGColour = "#53B9EC".ToUpper();
                    sPageOptionsTxtColour = "#FFFFFF".ToUpper();

                    sTableHeaderBGColour = "#53B9EC".ToUpper();
                    sTableHeaderTxtColour = "#FFFFFF".ToUpper();

                    sTabOptionBGColour = "#53B9EC".ToUpper();
                    sTabOptionTxtColour = "#FFFFFF".ToUpper();

                    sRowBGColour = "#FFFFFF".ToUpper();
                    sRowTxtColour = "#333333".ToUpper();
                    sAlternateRowBGColour = "#C7DFF7".ToUpper();
                    sAlternateRowTxtColour = "#333333".ToUpper();

                    sFieldTxtColour = "#333333".ToUpper();

                    sMenuOptionHoverTxtColour = "#004990".ToUpper();
                    sMenuOptionStandardTxtColour = "#004990".ToUpper();

                    sTooltipBgColour = "#EDEDED".ToUpper();
                    sTooltipTextColour = "#333333".ToUpper();

                    sGreenLightFieldColour = "#333333".ToUpper();
                    sGreenLightSectionTextColour = "#333333".ToUpper();
                    sGreenLightSectionBackgroundColour = "#FFFFFF".ToUpper();
                    sGreenLightSectionUnderlineColour = "#53b9ec".ToUpper();
                    break;
                case Modules.SmartDiligence:
                    sHeaderBGColour = "#1E2326".ToUpper();
                    sHeaderBreadcrumbTxtColour = "#E0E0E0".ToUpper();

                    sPageTitleTxtColour = "#C7114A".ToUpper();

                    sSectionHeadingUnderlineColour = "#C7114A".ToUpper();
                    sSectionHeadingTxtColour = "#FFFFFF".ToUpper();

                    sPageOptionsBGColour = "#C7114A".ToUpper();
                    sPageOptionsTxtColour = "#FFFFFF".ToUpper();

                    sTableHeaderBGColour = "#C7114A".ToUpper();
                    sTableHeaderTxtColour = "#FFFFFF".ToUpper();

                    sTabOptionBGColour = "#C7114A".ToUpper();
                    sTabOptionTxtColour = "#FFFFFF".ToUpper();

                    sRowBGColour = "#FFFFFF".ToUpper();
                    sRowTxtColour = "#333333".ToUpper();
                    sAlternateRowBGColour = "#C7114A".ToUpper();
                    sAlternateRowTxtColour = "#FFFFFF".ToUpper();


                    sFieldTxtColour = "#FFFFFF".ToUpper();

                    sMenuOptionHoverTxtColour = "#663399".ToUpper();
                    sMenuOptionStandardTxtColour = "#C7114A".ToUpper();

                    sTooltipBgColour = "#1E2326".ToUpper();
                    sTooltipTextColour = "#FFFFFF".ToUpper();

                    sGreenLightFieldColour = "#1E2326".ToUpper();
                    sGreenLightSectionTextColour = "#1E2326".ToUpper();
                    sGreenLightSectionBackgroundColour = "#FFFFFF".ToUpper();
                    sGreenLightSectionUnderlineColour = "#1E2326".ToUpper();
                    break;
                case Modules.Greenlight:
                    sHeaderBGColour = "#004990".ToUpper();
                    sHeaderBreadcrumbTxtColour = "#FFFFFF".ToUpper();

                    sPageTitleTxtColour = "#004990".ToUpper();

                    sSectionHeadingUnderlineColour = "#E46C0A".ToUpper();
                    sSectionHeadingTxtColour = "#004990".ToUpper();

                    sPageOptionsBGColour = "#004990".ToUpper();
                    sPageOptionsTxtColour = "#FFFFFF".ToUpper();

                    sTableHeaderBGColour = "#004990".ToUpper();
                    sTableHeaderTxtColour = "#FFFFFF".ToUpper();

                    sTabOptionBGColour = "#E46C0A".ToUpper();
                    sTabOptionTxtColour = "#FFFFFF".ToUpper();

                    sRowBGColour = "#DAD3CC".ToUpper();
                    sRowTxtColour = "#333333".ToUpper();
                    sAlternateRowBGColour = "#FFFFFF".ToUpper();
                    sAlternateRowTxtColour = "#333333".ToUpper();


                    sFieldTxtColour = "#004990".ToUpper();

                    sMenuOptionHoverTxtColour = "#E46C0A".ToUpper();
                    sMenuOptionStandardTxtColour = "#004990".ToUpper();

                    sTooltipBgColour = "#E46C0A".ToUpper();
                    sTooltipTextColour = "#FFFFFF".ToUpper();

                    sGreenLightFieldColour = "#004990".ToUpper();
                    sGreenLightSectionTextColour = "#004990".ToUpper();
                    sGreenLightSectionBackgroundColour = "#FFFFFF".ToUpper();
                    sGreenLightSectionUnderlineColour = "#004990".ToUpper();
                    break;
                case Modules.GreenlightWorkforce:
                    sHeaderBGColour = "#5E6E66".ToUpper();
                    sHeaderBreadcrumbTxtColour = "#FFFFFF".ToUpper();

                    sPageTitleTxtColour = "#E46C0A".ToUpper();

                    sSectionHeadingUnderlineColour = "#E46C0A".ToUpper();
                    sSectionHeadingTxtColour = "#FFFFFF".ToUpper();

                    sPageOptionsBGColour = "#E46C0A".ToUpper();
                    sPageOptionsTxtColour = "#FFFFFF".ToUpper();

                    sTableHeaderBGColour = "#E46C0A".ToUpper();
                    sTableHeaderTxtColour = "#FFFFFF".ToUpper();

                    sTabOptionBGColour = "#E46C0A".ToUpper();
                    sTabOptionTxtColour = "#FFFFFF".ToUpper();

                    sRowBGColour = "#DAD3CC".ToUpper();
                    sRowTxtColour = "#333333".ToUpper();
                    sAlternateRowBGColour = "#DAD3CC".ToUpper();
                    sAlternateRowTxtColour = "#333333".ToUpper();


                    sFieldTxtColour = "#FFFFFF".ToUpper();

                    sMenuOptionHoverTxtColour = "#E46C0A".ToUpper();
                    sMenuOptionStandardTxtColour = "#004990".ToUpper();

                    sTooltipBgColour = "#DAD3CC".ToUpper();
                    sTooltipTextColour = "#5E6E66".ToUpper();

                    sGreenLightFieldColour = "#5E6E66".ToUpper();
                    sGreenLightSectionTextColour = "#5E6E66".ToUpper();
                    sGreenLightSectionBackgroundColour = "#FFFFFF".ToUpper();
                    sGreenLightSectionUnderlineColour = "#5E6E66".ToUpper();
                    break;

                case Modules.CorporateDiligence:
                    sHeaderBGColour = "#43165A".ToUpper();
                    sHeaderBreadcrumbTxtColour = "#E0E0E0".ToUpper();

                    sPageTitleTxtColour = "#43165A".ToUpper();

                    sSectionHeadingUnderlineColour = "#663399".ToUpper();
                    sSectionHeadingTxtColour = "#1E2326".ToUpper();

                    sPageOptionsBGColour = "#43165A".ToUpper();
                    sPageOptionsTxtColour = "#e0e0e0".ToUpper();

                    sTableHeaderBGColour = "#43165A".ToUpper();
                    sTableHeaderTxtColour = "#FFFFFF".ToUpper();

                    sTabOptionBGColour = "#43165A".ToUpper();
                    sTabOptionTxtColour = "#FFFFFF".ToUpper();

                    sRowBGColour = "#FFFFFF".ToUpper();
                    sRowTxtColour = "#333333".ToUpper();
                    sAlternateRowBGColour = "#F1E4F8".ToUpper();
                    sAlternateRowTxtColour = "#000000".ToUpper();


                    sFieldTxtColour = "#43165A".ToUpper();

                    sMenuOptionHoverTxtColour = "#663399".ToUpper();
                    sMenuOptionStandardTxtColour = "#43165A".ToUpper();

                    sTooltipBgColour = "#43165a".ToUpper();
                    sTooltipTextColour = "#FFFFFF".ToUpper();

                    sGreenLightFieldColour = "#1E2326".ToUpper();
                    sGreenLightSectionTextColour = "#1E2326".ToUpper();
                    sGreenLightSectionBackgroundColour = "#FFFFFF".ToUpper();
                    sGreenLightSectionUnderlineColour = "#1E2326".ToUpper();
                    break;
                default:
                    sHeaderBGColour = "#4A65A0".ToUpper();
                    sHeaderBreadcrumbTxtColour = "#E0E0E0".ToUpper();

                    sPageTitleTxtColour = "#4A65A0".ToUpper();

                    sSectionHeadingUnderlineColour = "#4A65A0".ToUpper();
                    sSectionHeadingTxtColour = "#FFFFFF".ToUpper();

                    sPageOptionsBGColour = "#4A65A0".ToUpper();
                    sPageOptionsTxtColour = "#FFFFFF".ToUpper();

                    sTableHeaderBGColour = "#4A65A0".ToUpper();
                    sTableHeaderTxtColour = "#FFFFFF".ToUpper();

                    sTabOptionBGColour = "#4A65A0".ToUpper();
                    sTabOptionTxtColour = "#FFFFFF".ToUpper();

                    sRowBGColour = "#FFFFFF".ToUpper();
                    sRowTxtColour = "#333333".ToUpper();
                    sAlternateRowBGColour = "#D2E4EE".ToUpper();
                    sAlternateRowTxtColour = "#333333".ToUpper();


                    sFieldTxtColour = "#FFFFFF".ToUpper();

                    sMenuOptionHoverTxtColour = "#003768".ToUpper();
                    sMenuOptionStandardTxtColour = "#6280a7".ToUpper();

                    sTooltipBgColour = "#4A65A0".ToUpper();
                    sTooltipTextColour = "#FFFFFF".ToUpper();

                    sGreenLightFieldColour = "#4A65A0".ToUpper();
                    sGreenLightSectionTextColour = "#4A65A0".ToUpper();
                    sGreenLightSectionBackgroundColour = "#FFFFFF".ToUpper();
                    sGreenLightSectionUnderlineColour = "#4A65A0".ToUpper();
                    break;
            }
        }

        /// <summary>
        /// Restore default colours
        /// </summary>
        /// <param name="accountPropertiesFactory">An instance of <see cref="IDataFactory{TComplexType,TPrimaryKeyDataType}"/> for interacting with <see cref="IAccountProperty"/></param>
        public void RestoreDefaults(IDataFactory<IAccountProperty, AccountPropertyCacheKey> accountPropertiesFactory)
        {
            DeleteColours(accountPropertiesFactory);
            RefreshColours();
        }

        private void RefreshColours()
        {
            subAccProp = null;
            GetColours();
        }

        private void GetColours()
        {
            cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(nAccountID);
            subAccProp = clsSubAccounts.getSubAccountById(nSubAccountID).SubAccountProperties;
        }

        /// <summary>
        /// Updates the colours stored in the general options
        /// </summary>
        /// <param name="headerBG">
        /// The header BG.
        /// </param>
        /// <param name="headerBreadcrumbTxt">
        /// The header Breadcrumb Txt.
        /// </param>
        /// <param name="pageTitleTxt">
        /// The page Title Txt.
        /// </param>
        /// <param name="sectionHeadingUnderline">
        /// The section Heading Underline.
        /// </param>
        /// <param name="sectionHeadingTxt">
        /// The section Heading Txt.
        /// </param>
        /// <param name="fieldTxt">
        /// The field Txt.
        /// </param>
        /// <param name="pageOptionsBG">
        /// The page Options BG.
        /// </param>
        /// <param name="pageOptionsTxt">
        /// The page Options Txt.
        /// </param>
        /// <param name="tableHeaderBG">
        /// The table Header BG.
        /// </param>
        /// <param name="tableHeaderTxt">
        /// The table Header Txt.
        /// </param>
        /// <param name="tabOptionBG">
        /// The tab Option BG.
        /// </param>
        /// <param name="tabOptionTxt">
        /// The tab Option Txt.
        /// </param>
        /// <param name="rowBG">
        /// The row BG.
        /// </param>
        /// <param name="rowTxt">
        /// The row Txt.
        /// </param>
        /// <param name="altRowBG">
        /// The alt Row BG.
        /// </param>
        /// <param name="altRowTxt">
        /// The alt Row Txt.
        /// </param>
        /// <param name="menuOptionHoverTxt">
        /// The menu Option Hover Txt.
        /// </param>
        /// <param name="menuOptionStdTxt">
        /// The menu Option Std Txt.
        /// </param>
        /// <param name="tooltipBG">
        /// The tooltip BG.
        /// </param>
        /// <param name="tooltipText">
        /// The tooltip Text.
        /// </param>
        /// <param name="greenLightField">
        /// The green Light Field.
        /// </param>
        /// <param name="greenLightSectionText">
        /// The green Light Section Text.
        /// </param>
        /// <param name="greenLightSectionBackground">
        /// The green Light Section Background.
        /// </param>
        /// <param name="greenLightSectionUnderline">
        /// The green Light Section Underline.
        /// </param>
        /// <param name="accountPropertiesFactory">
        /// The account Properties Factory.
        /// </param>
        /// <returns>
        /// Currently true unless it errors, until the save sub account properties method returns a value
        /// </returns>
        public bool UpdateColours(string headerBG, string headerBreadcrumbTxt, string pageTitleTxt, string sectionHeadingUnderline, string sectionHeadingTxt, string fieldTxt, 
            string pageOptionsBG, string pageOptionsTxt, string tableHeaderBG, string tableHeaderTxt, string tabOptionBG, string tabOptionTxt, string rowBG, string rowTxt, 
            string altRowBG, string altRowTxt, string menuOptionHoverTxt, string menuOptionStdTxt, string tooltipBG, string tooltipText, string greenLightField, 
            string greenLightSectionText, string greenLightSectionBackground, string greenLightSectionUnderline, IDataFactory<IAccountProperty, AccountPropertyCacheKey> accountPropertiesFactory)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            headerBG = (!string.IsNullOrEmpty(headerBG)) ? headerBG.ToUpper().Replace("#", "") : headerBG;
            headerBreadcrumbTxt = (!string.IsNullOrEmpty(headerBreadcrumbTxt)) ? headerBreadcrumbTxt.ToUpper().Replace("#", "") : headerBreadcrumbTxt;
            pageTitleTxt = (!string.IsNullOrEmpty(pageTitleTxt)) ? pageTitleTxt.ToUpper().Replace("#", "") : pageTitleTxt;
            sectionHeadingUnderline = (!string.IsNullOrEmpty(sectionHeadingUnderline)) ? sectionHeadingUnderline.ToUpper().Replace("#", "") : sectionHeadingUnderline;
            sectionHeadingTxt = (!string.IsNullOrEmpty(sectionHeadingTxt)) ? sectionHeadingTxt.ToUpper().Replace("#", "") : sectionHeadingTxt;
            fieldTxt = (!string.IsNullOrEmpty(fieldTxt)) ? fieldTxt.ToUpper().Replace("#", "") : fieldTxt;
            pageOptionsBG = (!string.IsNullOrEmpty(pageOptionsBG)) ? pageOptionsBG.ToUpper().Replace("#", "") : pageOptionsBG;
            pageOptionsTxt = (!string.IsNullOrEmpty(pageOptionsTxt)) ? pageOptionsTxt.ToUpper().Replace("#", "") : pageOptionsTxt;
            tableHeaderBG = (!string.IsNullOrEmpty(tableHeaderBG)) ? tableHeaderBG.ToUpper().Replace("#", "") : tableHeaderBG;
            tableHeaderTxt = (!string.IsNullOrEmpty(tableHeaderTxt)) ? tableHeaderTxt.ToUpper().Replace("#", "") : tableHeaderTxt;
            tabOptionBG = (!string.IsNullOrEmpty(tabOptionBG)) ? tabOptionBG.ToUpper().Replace("#", "") : tabOptionBG;
            tabOptionTxt = (!string.IsNullOrEmpty(tabOptionTxt)) ? tabOptionTxt.ToUpper().Replace("#", "") : tabOptionTxt;
            rowBG = (!string.IsNullOrEmpty(rowBG)) ? rowBG.ToUpper().Replace("#", "") : rowBG;
            rowTxt = (!string.IsNullOrEmpty(rowTxt)) ? rowTxt.ToUpper().Replace("#", "") : rowTxt;
            altRowBG = (!string.IsNullOrEmpty(altRowBG)) ? altRowBG.ToUpper().Replace("#", "") : altRowBG;
            altRowTxt = (!string.IsNullOrEmpty(altRowTxt)) ? altRowTxt.ToUpper().Replace("#", "") : altRowTxt;
            menuOptionHoverTxt = (!string.IsNullOrEmpty(menuOptionHoverTxt)) ? menuOptionHoverTxt.ToUpper().Replace("#", "") : menuOptionHoverTxt;
            menuOptionStdTxt = (!string.IsNullOrEmpty(menuOptionStdTxt)) ? menuOptionStdTxt.ToUpper().Replace("#", "") : menuOptionStdTxt;
            tooltipBG = (!string.IsNullOrEmpty(tooltipBG)) ? tooltipBG.ToUpper().Replace("#", "") : tooltipBG;
            tooltipText = (!string.IsNullOrEmpty(tooltipText)) ? tooltipText.ToUpper().Replace("#", "") : tooltipText;
            greenLightField = (!string.IsNullOrEmpty(greenLightField)) ? greenLightField.ToUpper().Replace("#", "") : greenLightField;
            greenLightSectionText = (!string.IsNullOrEmpty(greenLightSectionText)) ? greenLightSectionText.ToUpper().Replace("#", "") : greenLightSectionText;
            greenLightSectionBackground = (!string.IsNullOrEmpty(greenLightSectionBackground)) ? greenLightSectionBackground.ToUpper().Replace("#", "") : greenLightSectionBackground;
            greenLightSectionUnderline = (!string.IsNullOrEmpty(greenLightSectionUnderline)) ? greenLightSectionUnderline.ToUpper().Replace("#", "") : greenLightSectionUnderline;

            var headerBackgroundProperty = string.Empty;

            if ("#" + headerBG != sHeaderBGColour)
            {
                headerBackgroundProperty = headerBG;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursHeaderBackground.GetDescription(), headerBackgroundProperty, currentUser.CurrentSubAccountId));

            var headerBreadcrumbTextProperty = string.Empty;

            if ("#" + headerBreadcrumbTxt != sHeaderBreadcrumbTxtColour)
            {
                headerBreadcrumbTextProperty = headerBreadcrumbTxt;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursHeaderBreadcrumbText.GetDescription(), headerBreadcrumbTextProperty, currentUser.CurrentSubAccountId));

            var pageTitleTxtProperty = string.Empty;

            if ("#" + pageTitleTxt != sPageTitleTxtColour)
            {
                pageTitleTxtProperty = pageTitleTxt;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursPageTitleText.GetDescription(), pageTitleTxtProperty, currentUser.CurrentSubAccountId));

            var sectionHeadingUnderlineProperty = string.Empty;

            if ("#" + sectionHeadingUnderline != sSectionHeadingUnderlineColour)
            {
                sectionHeadingUnderlineProperty = sectionHeadingUnderline;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursSectionHeadingUnderline.GetDescription(), sectionHeadingUnderlineProperty, currentUser.CurrentSubAccountId));

            var sectionHeadingTxtProperty = string.Empty;

            if ("#" + sectionHeadingTxt != sSectionHeadingTxtColour)
            {
                sectionHeadingTxtProperty = sectionHeadingTxt;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursSectionHeadingText.GetDescription(), sectionHeadingTxtProperty, currentUser.CurrentSubAccountId));

            var fieldTxtColourProperty = string.Empty;

            if ("#" + fieldTxt != sFieldTxtColour)
            {
                fieldTxtColourProperty = fieldTxt;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursFieldText.GetDescription(), fieldTxtColourProperty, currentUser.CurrentSubAccountId));

            var pageOptionsBGProperty = string.Empty;

            if ("#" + pageOptionsBG != sPageOptionsBGColour)
            {
                pageOptionsBGProperty = pageOptionsBG;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursPageOptionsBackground.GetDescription(), pageOptionsBGProperty, currentUser.CurrentSubAccountId));

            var pageOptionsTxtProperty = string.Empty;

            if ("#" + pageOptionsTxt != sPageOptionsTxtColour)
            {
                pageOptionsTxtProperty = pageOptionsTxt;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursPageOptionsText.GetDescription(), pageOptionsTxtProperty, currentUser.CurrentSubAccountId));

            var tableHeaderBGColourProperty = string.Empty;

            if ("#" + tableHeaderBG != sTableHeaderBGColour)
            {
                tableHeaderBGColourProperty = tableHeaderBG;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursTableHeaderBackground.GetDescription(), tableHeaderBGColourProperty, currentUser.CurrentSubAccountId));

            var tableHeaderTxtProperty = string.Empty;

            if ("#" + tableHeaderTxt != sTableHeaderTxtColour)
            {
                tableHeaderTxtProperty = tableHeaderTxt;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursTableHeaderText.GetDescription(), tableHeaderTxtProperty, currentUser.CurrentSubAccountId));

            var tabOptionBGProperty = string.Empty;

            if ("#" + tabOptionBG != sTabOptionBGColour)
            {
                tabOptionBGProperty = tabOptionBG;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursTabOptionBackground.GetDescription(), tabOptionBGProperty, currentUser.CurrentSubAccountId));

            var tabOptionTxtProperty = string.Empty;

            if ("#" + tabOptionTxt != sTabOptionTxtColour)
            {
                tabOptionTxtProperty = tabOptionTxt;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursTabOptionText.GetDescription(), tabOptionTxtProperty, currentUser.CurrentSubAccountId));

            var rowBGProperty = string.Empty;

            if ("#" + rowBG != sRowBGColour)
            {
                rowBGProperty = rowBG;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursRowBackground.GetDescription(), rowBGProperty, currentUser.CurrentSubAccountId));

            var rowTxtProperty = string.Empty;

            if ("#" + rowTxt != sRowTxtColour)
            {
                rowTxtProperty = rowTxt;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursRowText.GetDescription(), rowTxtProperty, currentUser.CurrentSubAccountId));

            var altRowBGProperty = string.Empty;

            if ("#" + altRowBG != sAlternateRowBGColour)
            {
                altRowBGProperty = altRowBG;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursAlternateRowBackground.GetDescription(), altRowBGProperty, currentUser.CurrentSubAccountId));

            var altRowTxtProperty = string.Empty;

            if ("#" + altRowTxt != sAlternateRowTxtColour)
            {
                altRowTxtProperty = altRowTxt;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursAlternateRowText.GetDescription(), altRowTxtProperty, currentUser.CurrentSubAccountId));

            var menuOptionHoverTxtProperty = string.Empty;

            if ("#" + menuOptionHoverTxt != sMenuOptionHoverTxtColour)
            {
                menuOptionHoverTxtProperty = menuOptionHoverTxt;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursMenuOptionHoverText.GetDescription(), menuOptionHoverTxtProperty, currentUser.CurrentSubAccountId));

            var menuOptionStdTxtProperty = string.Empty;

            if ("#" + menuOptionStdTxt != sMenuOptionStandardTxtColour)
            {
                menuOptionStdTxtProperty = menuOptionStdTxt;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursMenuOptionStandardText.GetDescription(), menuOptionStdTxtProperty, currentUser.CurrentSubAccountId));

            var ptooltipBGProperty = string.Empty;

            if ("#" + tooltipBG != sTooltipBgColour)
            {
                ptooltipBGProperty = tooltipBG;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursTooltipBackground.GetDescription(), ptooltipBGProperty, currentUser.CurrentSubAccountId));

            var tooltipTextProperty = string.Empty;

            if ("#" + tooltipText != sTooltipTextColour)
            {
                tooltipTextProperty = tooltipText;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursTooltipText.GetDescription(), tooltipTextProperty, currentUser.CurrentSubAccountId));

            var greenLightFieldProperty = string.Empty;

            if ("#" + greenLightField != sGreenLightFieldColour)
            {
                greenLightFieldProperty = greenLightField;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursGreenLightField.GetDescription(), greenLightFieldProperty, currentUser.CurrentSubAccountId));

            var greenLightSectionTextProperty = string.Empty;

            if ("#" + greenLightSectionText != sGreenLightSectionTextColour)
            {
                greenLightSectionTextProperty = greenLightSectionText;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursGreenLightSectionText.GetDescription(), greenLightSectionTextProperty, currentUser.CurrentSubAccountId));

            var greenLightSectionBackgroundProperty = string.Empty;

            if ("#" + greenLightSectionBackground != sGreenLightSectionBackgroundColour)
            {
                greenLightSectionBackgroundProperty = greenLightSectionBackground;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursGreenLightSectionBackground.GetDescription(), greenLightSectionBackgroundProperty, currentUser.CurrentSubAccountId));

            var greenLightSectionUnderlineProperty = string.Empty;

            if ("#" + greenLightSectionUnderline != sGreenLightSectionUnderlineColour)
            {
                greenLightSectionUnderlineProperty = greenLightSectionUnderline;
            }

            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursGreenLightSectionUnderline.GetDescription(), greenLightSectionUnderlineProperty, currentUser.CurrentSubAccountId));

            var accountBase = new cAccountSubAccountsBase(currentUser.AccountID);
            accountBase.InvalidateCache(currentUser.CurrentSubAccountId);

            RefreshColours();

            return true;
        }

        private void DeleteColours(IDataFactory<IAccountProperty, AccountPropertyCacheKey> accountPropertiesFactory)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursAlternateRowBackground.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursAlternateRowText.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursFieldText.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursMenuOptionHoverText.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursMenuOptionStandardText.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursHeaderBackground.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursHeaderBreadcrumbText.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursRowBackground.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursRowText.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursPageTitleText.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursSectionHeadingUnderline.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursSectionHeadingText.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursPageOptionsBackground.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursPageOptionsText.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursTableHeaderBackground.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursTableHeaderText.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursTabOptionBackground.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursTabOptionText.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursTooltipBackground.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursTooltipText.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursGreenLightField.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursGreenLightSectionText.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursGreenLightSectionBackground.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));
            accountPropertiesFactory.Save(new AccountProperty(AccountPropertyKeys.ColoursGreenLightSectionUnderline.GetDescription(), string.Empty, currentUser.CurrentSubAccountId));

            var subAccountsBase = new cAccountSubAccountsBase(currentUser.AccountID);
            subAccountsBase.InvalidateCache(currentUser.CurrentSubAccountId);

            RefreshColours();
        }

        /// <summary>
        /// Outputs styles for the page, usually only if they differ from the defaults
        /// </summary>
        /// <param name="displayLeftMenuImage">
        /// The display Left Menu Image.
        /// </param>
        /// <returns>
        /// string of styles enclosed in style tag
        /// </returns>
        public static string customiseStyles(bool displayLeftMenuImage = true)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            cColours clscolours = new cColours(user.AccountID, user.CurrentSubAccountId, user.CurrentActiveModule);

            ////int height = (int)Session["screenheight"] - 240;
            ////int width = (int)Session["screenwidth"] - 155;
            ////make page set height

            output.Append("<style type=\"text/css\">\n");

            if (displayLeftMenuImage == true)
            {
                output.Append("body\n");
                output.Append("{\n");
                output.Append("background: url(" + cMisc.Path + "/images/submenu_bg.jpg) repeat-y 0% 0%;\n");
                output.Append("}\n");
            }

            if (clscolours.headerBGColour != clscolours.defaultHeaderBGColour)
            {
                output.Append("#pagediv\n");
                output.Append("{\n");
                output.Append("background: url(" + cMisc.Path + "/images/spacer.gif);\n");
                output.Append("border-color: " + clscolours.headerBGColour + ";\n");
                output.Append("}\n");

                output.Append(".breadcrumb\n");
                output.Append("{\n");
                output.Append("border-color: " + clscolours.headerBGColour + ";\n");
                output.Append("background: url(" + cMisc.Path + "/images/spacer.gif);\n");
                output.Append("background-color: " + clscolours.headerBGColour + ";\n");
                output.Append("}\n");

                output.Append(".menuitem\n");
                output.Append("{\n");
                output.Append("background: url(" + cMisc.Path + "/images/spacer.gif);\n");
                output.Append("background-color: " + clscolours.headerBGColour + ";\n");
                output.Append("}\n");

                output.Append(".toplevelmenu\n");
                output.Append("{\n");
                output.Append("background: url(" + cMisc.Path + "/images/spacer.gif);\n");
                output.Append("background-color: " + clscolours.headerBGColour + ";\n");
                output.Append("}\n");

                output.Append(".tooltipCustomClass \n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.headerBGColour + "!important;\n");
                output.Append("}\n");
            }

            if (clscolours.headerBreadcrumbTxtColour != clscolours.defaultHeaderBreadcrumbTxtColour)
            {
                output.Append(".breadcrumb li\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.headerBreadcrumbTxtColour + "!important;\n");
                output.Append("}\n");

                output.Append(".menuitem\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.headerBreadcrumbTxtColour + "!important;\n");
                output.Append("}\n");

                output.Append(".breadcrumb li a:link, .breadcrumb li a:visited, .breadcrumb li a:active, .breadcrumb li a:hover\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.headerBreadcrumbTxtColour + "!important;\n");
                output.Append("}\n");

                output.Append("a.MenuItemLink\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.headerBreadcrumbTxtColour + ";\n");
                output.Append("}\n");
                output.Append("a.MenuItemLink:link\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.headerBreadcrumbTxtColour + ";\n");
                output.Append("}\n");
                output.Append("a.MenuItemLink:visited\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.headerBreadcrumbTxtColour + ";\n");
                output.Append("}\n");
                output.Append("a.MenuItemLink:hover\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.headerBreadcrumbTxtColour + ";\n");
                output.Append("}\n");

                output.Append("a.MenuLabelLink\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.headerBreadcrumbTxtColour + ";\n");
                output.Append("}\n");
                output.Append("a.MenuLabelLink:link\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.headerBreadcrumbTxtColour + ";\n");
                output.Append("}\n");
                output.Append("a.MenuLabelLink:visited\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.headerBreadcrumbTxtColour + ";\n");
                output.Append("}\n");
                output.Append("a.MenuLabelLink:hover\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.headerBreadcrumbTxtColour + ";\n");
                output.Append("}\n");

                output.Append(".r_nav span, .r_nav span:before, .r_nav span:after\n");
                output.Append("{\n");
                output.Append("background: " + clscolours.headerBreadcrumbTxtColour + ";\n");
                output.Append("}\n");

                output.Append(".tooltipCustomClass .qtip-content\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.headerBreadcrumbTxtColour + "!important;\n");
                output.Append("}\n");
                /*
                //output.Append(".spanTaskSummaryPopupLink a:link, .spanTaskSummaryPopupLink a:visited, .spanTaskSummaryPopupLink a:active, .spanTaskSummaryPopupLink a:hover\n");
                //output.Append("{\n");
                //output.Append("color: " + clscolours.sectionHeadingTxtColour + ";\n");
                //output.Append("}\n");

                //output.Append(".formpanel .cGrid\n");
                //output.Append("{\n");
                //output.Append("border: solid 1px " + clscolours.headerBreadcrumbTxtColour + ";\n");
                //output.Append("}\n");
                 * */
            }


            if (clscolours.pageTitleTxtColour != clscolours.defaultPageTitleTxtColour)
            {
                output.Append(".page-title-bar label, .page-title-bar\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.pageTitleTxtColour + ";");
                output.Append("}\n");
            }


            if (clscolours.sectionHeadingUnderlineColour != clscolours.defaultSectionHeadingUnderlineColour)
            {
                output.Append(".infobar\n");
                output.Append("{\n");
                output.Append("border: 1px solid " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("background: url(" + cMisc.Path + "/images/spacer.gif);\n");
                output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("}\n");

                output.Append(".inputpaneltitle, .formpanel .sectiontitle\n");
                output.Append("{\n");
                output.Append("border-bottom: 2px solid " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("}\n");

                output.Append(".userdefinedGroupings li\n{\n");
                output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("border-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("}\n");

                output.Append(".panel, .errorModal, .errorModalSubject\n");
                output.Append("{\n");
                output.Append("border-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("}\n");

                output.Append(".homepaneltitle\n");
                output.Append("{\n");
                output.Append("background: url(" + cMisc.Path + "/images/spacer.gif);\n");
                output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("border-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("}\n");

                output.Append("#broadcastheader, .errorModalSubject, #flagheader\n");
                output.Append("{\n;");
                output.Append("background: url(" + cMisc.Path + "/images/spacer.gif);\n");
                output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("}\n");

                output.Append(".datatbl .tableheader\n");
                output.Append("{\n");
                output.Append("background: url(" + cMisc.Path + "/images/spacer.gif);\n");
                output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("}\n");

                output.Append(".faqQuestion\n");
                output.Append("{\n");
                output.Append("background-image: url('" + cMisc.Path + "/images/spacer.gif');\n");
                output.Append("border: 1px solid " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("}\n");

                output.Append(".logodiv\n");
                output.Append("{\n");
                output.Append("background-image: url('" + cMisc.Path + "/images/spacer.gif');\n");
                output.Append("border: 1px solid " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("}\n");

                output.Append("legend\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("}\n");

                output.Append(".passengersdialog .ui-dialog-titlebar, .passengersdialog li.token-input-token, .passengersdialog li.token-input-selected-token\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("}\n");

                output.Append("#torchGroupingColumns, #torchSortingColumns , #torchFilteringColumns, #torchSortedColumnColumns\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("}\n");

                output.Append(".accordianheader\n");
                output.Append("{\n");
                output.Append("border-bottom: 2px solid " + clscolours.sectionHeadingUnderlineColour + ";\n");
                output.Append("}\n");
            }

            if (clscolours.sectionHeadingTxtColour != clscolours.defaultSectionHeadingTxtColour)
            {
                output.Append(".pagetitle, .formpanel .sectiontitle, .inputpaneltitle, .inputpaneltitlelabel, .infobar, legend, #broadcastheader, #flagheader, .errorModalSubject\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.sectionHeadingTxtColour + ";\n");
                output.Append("}\n");

                output.Append(".userdefinedGroupings span\n{\n");
                output.Append("color: " + clscolours.sectionHeadingTxtColour + ";\n");
                output.Append("}\n");

                output.Append(".accordianheader\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.sectionHeadingTxtColour + ";\n");
                output.Append("}\n");

                output.Append("#broadcastmsg\n");
                output.Append("{\n");
                output.Append("border-color: " + clscolours.sectionHeadingTxtColour + ";\n");
                output.Append("}\n");

                output.Append("#broadcastheader\n");
                output.Append("{\n");
                output.Append("border-bottom-color: " + clscolours.sectionHeadingTxtColour + ";\n");
                output.Append("}\n");

                output.Append(".faqQuestion\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.sectionHeadingTxtColour + ";\n");
                output.Append("}\n");

                output.Append(".passengersdialog .ui-dialog-titlebar, .passengersdialog li.token-input-token, .passengersdialog li.token-input-selected-token \n");
                output.Append("{\n");
                output.Append("color: " + clscolours.sectionHeadingTxtColour + ";\n");
                output.Append("}\n");

                output.Append("#torchGroupingColumns, #torchSortingColumns , #torchFilteringColumns, #torchSortedColumnColumns \n");
                output.Append("{\n");
                output.Append("color: " + clscolours.sectionHeadingTxtColour + ";\n");
                output.Append("}\n");

                output.Append(".groupingHeaderIncluded \n");
                output.Append("{\n");
                output.Append("color: " + clscolours.sectionHeadingTxtColour + ";\n");
                output.Append("}\n");
            }

            if (clscolours.rowBGColour != clscolours.defaultRowBGColour)
            {
                output.Append(".row1\n");
                output.Append("{\n");
                output.Append("background: url(" + cMisc.Path + "/images/spacer.gif);\n");
                output.Append("background-color: " + clscolours.rowBGColour + ";\n");
                output.Append("}\n");

            }

            if (clscolours.rowTxtColour != clscolours.defaultRowTxtColour)
            {
                output.Append(".row1, .row1 a, .row1 a:link, .row1 a:active, .row1 a:hover\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.rowTxtColour + ";\n");
                output.Append("}\n");
            }

            if (clscolours.altRowBGColour != clscolours.defaultAltRowBGColour)
            {
                output.Append(".row2\n");
                output.Append("{\n");
                output.Append("background: url(" + cMisc.Path + "/images/spacer.gif);\n");
                output.Append("background-color: " + clscolours.altRowBGColour + ";\n");
                output.Append("}\n");
                output.Append(".subtitle\n");
                output.Append("{\n");
                output.Append("background: url(" + cMisc.Path + "/images/spacer.gif);\n");
                output.Append("background-color: " + clscolours.altRowBGColour + ";\n");
                output.Append("}\n");

                output.Append("#torchGroupingBin, #torchSortingBin, #torchFilteringBin, #torchSortedColumnBin \n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.altRowBGColour + ";\n");
                output.Append("}\n");

                ////output.Append(".formpanel .sectiontitle\n");
                ////output.Append("{\n");
                ////output.Append("background: url(" + cMisc.path + "/images/spacer.gif);\n");
                ////output.Append("background-color: " + clscolours.altRowColour + ";\n");
                ////output.Append("border: 1px solid " + clscolours.altRowColour + ";\n");
                ////output.Append("}\n");
            }

            if (clscolours.altRowTxtColour != clscolours.defaultAltRowTxtColour)
            {
                output.Append(".row2, .row2 a, .row2 a:link, .row2 a:active, .row2 a:hover\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.altRowTxtColour + ";\n");
                output.Append("}\n");
                output.Append(".subtitle\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.altRowTxtColour + ";\n");
                output.Append("}\n");

                output.Append("#torchGroupingBin, #torchSortingBin, #torchFilteringBin, #torchSortedColumnBin \n");
                output.Append("{\n");
                output.Append("color: " + clscolours.altRowTxtColour + ";\n");
                output.Append("}\n");

                output.Append(".groupingHeaderExcluded \n");
                output.Append("{\n");
                output.Append("color: " + clscolours.altRowTxtColour + ";\n");
                output.Append("}\n");

            }

            // Set the colour for visited links
            output.Append(".row1 a:visited\n");
            output.Append("{\n");
            output.Append("color: #800080;\n");
            output.Append("}\n");
            output.Append(".row2 a:visited\n");
            output.Append("{\n");
            output.Append("color: #800080;\n");
            output.Append("}\n");

            if (clscolours.fieldTxtColour != clscolours.defaultFieldTxt)
            {
                output.Append(".labeltd, .labeltdmand, .labeltd a, .labeltd a:link, .labeltd a:visited, .labeltd a:active, .labeltd a:hover, .labeltdmand a, .labeltdmand a:link, .labeltdmand a:visited, .labeltdmand a:active, .labeltdmand a:hover\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.fieldTxtColour + ";\n");
                output.Append("}\n");

                output.Append(".formpanel label, .sm_panel label, .formpanel label a, .formpanel label a:link, .formpanel label a:visited, .formpanel label a:active, .formpanel label a:hover, .userdefinedField li, .userdefinedField span\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.fieldTxtColour + "!important;\n");
                output.Append("}\n");

                output.Append(".textlabel\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.fieldTxtColour + ";\n");
                output.Append("}\n");
            }

            if (clscolours.pageOptionsBGColour != clscolours.defaultPageOptionsBGColour)
            {
                output.Append(".paneltitle\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.pageOptionsBGColour + ";\n");
                output.Append("border-color: " + clscolours.pageOptionsBGColour + ";\n");
                output.Append("}\n");

                output.Append("a.submenuitem, a:link.submenuitem, a:visited.submenuitem, a:active.submenuitem, a:hover.submenuitem, #divPageMenu a\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.pageOptionsBGColour + ";\n");
                output.Append("}\n");
            }

            if (clscolours.pageOptionsTxtColour != clscolours.defaultPageOptionsTxtColour)
            {
                output.Append(" .paneltitle");
                output.Append("{\n");
                output.Append("color: " + clscolours.pageOptionsTxtColour + ";\n");
                output.Append("}\n");
            }

            if (clscolours.tableHeaderBGColour != clscolours.defaultTableHeaderBGColour)
            {
                output.Append(".formpanel .cGrid th\n");
                output.Append("{\n");
                output.Append("background: url(" + cMisc.Path + "/images/spacer.gif);\n");
                output.Append("background-color: " + clscolours.tableHeaderBGColour + "!important;\n");
                output.Append("}\n");

                output.Append(".formpanel .cGrid\n");
                output.Append("{\n");
                output.Append("border: 1px solid " + clscolours.tableHeaderBGColour + "!important;\n");
                output.Append("}\n");

                output.Append(".datatbl th, .infagisticsHeader\n");
                output.Append("{\n");
                output.Append("background: url(" + cMisc.Path + "/images/spacer.gif);\n");
                output.Append("background-color: " + clscolours.tableHeaderBGColour + "!important;\n");
                output.Append("}\n");

                output.Append("table thead tr th, table tbody tr th\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.tableHeaderBGColour + "!important;\n");
                output.Append("}\n");
            }

            if (clscolours.tableHeaderTxtColour != clscolours.defaultTableHeaderTxtColour)
            {
                output.Append(".formpanel .cGrid th, .formpanel .cGrid th a, .formpanel .cGrid th a:link, .formpanel .cGrid th a:visited, .formpanel .cGrid th a:active, .formpanel .cGrid th a:hover\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.tableHeaderTxtColour + "!important;\n");
                output.Append("}\n");

                output.Append(".datatbl th, .datatbl th a, .datatbl th a:link, .datatbl th a:visited, .datatbl th a:active, .datatbl th a:hover, .datatbl th .infagisticsHeader, .datatbl th .infagisticsHeader a, .datatbl th .infagisticsHeader a:link, .datatbl th .infagisticsHeader a:visited, .datatbl th .infagisticsHeader a:active, .datatbl th .infagisticsHeader a:hover\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.tableHeaderTxtColour + ";\n");
                output.Append("}\n");

                output.Append("table thead tr th, table thead tr th a, table tbody tr th, table tbody tr th a, .mileagegridtable th div\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.tableHeaderTxtColour + " !important;\n");
                output.Append("}\n");
            }

            if (clscolours.tabOptionBGColour != clscolours.defaultTabOptionBGColour)
            {
                output.Append(".ajax__tab_xp .ajax__tab_header .ajax__tab_active .ajax__tab_tab\n");
                output.Append("{\n");
                output.Append("background: " + clscolours.tabOptionBGColour + " !important;\n");
                output.Append("border: " + clscolours.tabOptionBGColour + " solid 1px !important;\n");
                output.Append("}\n");

                output.Append(".ajax__tab_xp .ajax__tab_header .ajax__tab_tab\n");
                output.Append("{\n");
                output.Append("background: " + clscolours.tabOptionBGColour + "!important;\n");
                output.Append("}\n");

                output.Append(".ui-widget-header\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.tabOptionBGColour + "!important;\n");
                output.Append("}\n");

                output.Append(".sm_tabheader\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.tabOptionBGColour + "!important;\n");
                output.Append("border: " + clscolours.tabOptionBGColour + " solid 1px !important;\n");
                output.Append("}\n");

            }

            if (clscolours.tabOptionTxtColour != clscolours.defaultTabOptionTxtColour)
            {
                output.Append(".ajax__tab_xp .ajax__tab_header .ajax__tab_tab\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.tabOptionTxtColour + " !important;\n");
                output.Append("}\n");

                output.Append(".ui-widget-header\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.tabOptionTxtColour + " !important;\n");
                output.Append("}\n");

                output.Append(".sm_tabheader\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.tabOptionTxtColour + "!important;\n");
                output.Append("}\n");
            }

            if (clscolours.menuOptionStdTxtColour != clscolours.defaultMenuOptionStandardTxtColour)
            {

                output.Append(".media-body h2\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.menuOptionStdTxtColour + ";\n");
                output.Append("}\n");
            }

            if (clscolours.menuOptionHoverTxtColour != clscolours.defaultMenuOptionHoverTxtColour)
            {
                output.Append(".well:hover h2\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.menuOptionHoverTxtColour + ";\n");
                output.Append("}\n");
            }


            if (clscolours.tooltipBgColour != clscolours.defaultTooltipBgColour)
            {
                output.Append(".tooltipcontainer\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.tooltipBgColour + ";\n");
                output.Append("}\n");


                output.Append(".tooltipcontent, .commenttooltip\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.tooltipBgColour + ";\n");
                output.Append("}\n");
            }

            if (clscolours.tooltipTextColour != clscolours.defaultTooltipTextColour)
            {
                output.Append(".tooltipcontainer .tooltipcontent, .commenttooltip\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.tooltipTextColour + " !important;\n");
                output.Append("}\n");

            }

            output.Append("</style>");
            return output.ToString();
        }

        #region properties
        public string headerBGColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursHeaderBackground))
                {
                    return sHeaderBGColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursHeaderBackground.ToUpper();
                }
            }
        }

        public string headerBreadcrumbTxtColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursHeaderBreadcrumbText))
                {
                    return sHeaderBreadcrumbTxtColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursHeaderBreadcrumbText.ToUpper();
                }
            }
        }

        public string pageTitleTxtColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursPageTitleText))
                {
                    return sPageTitleTxtColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursPageTitleText.ToUpper();
                }
            }
        }

        public string sectionHeadingUnderlineColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursSectionHeadingUnderline))
                {
                    return sSectionHeadingUnderlineColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursSectionHeadingUnderline.ToUpper();
                }
            }
        }

        public string sectionHeadingTxtColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursSectionHeadingText))
                {
                    return sSectionHeadingTxtColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursSectionHeadingText.ToUpper();
                }
            }
        }

        public string pageOptionsBGColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursPageOptionsBackground))
                {
                    return sPageOptionsBGColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursPageOptionsBackground.ToUpper();
                }
            }
        }

        public string pageOptionsTxtColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursPageOptionsText))
                {
                    return sPageOptionsTxtColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursPageOptionsText.ToUpper();
                }
            }
        }

        public string tableHeaderBGColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursTableHeaderBackground))
                {
                    return sTableHeaderBGColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursTableHeaderBackground.ToUpper();
                }
            }
        }

        public string tableHeaderTxtColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursTableHeaderText))
                {
                    return sTableHeaderTxtColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursTableHeaderText.ToUpper();
                }
            }
        }

        public string tabOptionBGColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursTabOptionBackground))
                {
                    return sTabOptionBGColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursTabOptionBackground.ToUpper();
                }
            }
        }

        public string tabOptionTxtColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursTabOptionText))
                {
                    return sTabOptionTxtColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursTabOptionText.ToUpper();
                }
            }
        }

        public string rowBGColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursRowBackground))
                {
                    return sRowBGColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursRowBackground.ToUpper();
                }
            }
        }

        public string rowTxtColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursRowText))
                {
                    return sRowTxtColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursRowText.ToUpper();
                }
            }
        }
        public string altRowBGColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursAlternateRowBackground))
                {
                    return sAlternateRowBGColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursAlternateRowBackground.ToUpper();
                }
            }
        }

        public string altRowTxtColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursAlternateRowText))
                {
                    return sAlternateRowTxtColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursAlternateRowText.ToUpper();
                }
            }
        }

        public string fieldTxtColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursFieldText))
                {
                    return sFieldTxtColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursFieldText.ToUpper();
                }
            }
        }
        public string menuOptionHoverTxtColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursMenuOptionHoverText))
                {
                    return sMenuOptionHoverTxtColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursMenuOptionHoverText.ToUpper();
                }
            }
        }
        public string menuOptionStdTxtColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursMenuOptionStandardText))
                {
                    return sMenuOptionStandardTxtColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursMenuOptionStandardText.ToUpper();
                }
            }
        }

        public string tooltipBgColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursTooltipBackground))
                {
                    return sTooltipBgColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursTooltipBackground.ToUpper();
                }
            }
        }

        public string tooltipTextColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursTooltipText))
                {
                    return sTooltipTextColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursTooltipText.ToUpper();
                }
            }
        }

        public string greenLightFieldColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursGreenLightField))
                {
                    return sGreenLightFieldColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursGreenLightField.ToUpper();
                }
            }
        }

        public string greenLightSectionTextColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursGreenLightSectionText))
                {
                    return sGreenLightSectionTextColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursGreenLightSectionText.ToUpper();
                }
            }
        }
        public string greenLightSectionBackgroundColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursGreenLightSectionBackground))
                {
                    return sGreenLightSectionBackgroundColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursGreenLightSectionBackground.ToUpper();
                }
            }
        }
        public string greenLightSectionUnderlineColour
        {
            get
            {
                if (subAccProp == null || string.IsNullOrEmpty(subAccProp.ColoursGreenLightSectionUnderline))
                {
                    return sGreenLightSectionUnderlineColour;
                }
                else
                {
                    return "#" + subAccProp.ColoursGreenLightSectionUnderline.ToUpper();
                }
            }
        }
        #endregion

        public static string customiseLogonPageColours(Modules activemodule)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            switch (activemodule)
            {
                case Modules.Contracts:
                    sb.Append("#logonpage { background-color: #ffffff; }");
                    sb.Append("#logonpage #breadcrumbbar a { color: #ffffff; }");
                    sb.Append("#logonpage #breadcrumbbar { background-image: none; background-color: #97ce8b; }");
                    sb.Append("#logonpage #pagetitlebar { background-color: #97ce8b; color: #ffffff; background-image: none; }");
                    sb.Append("#logonpage #logonoutercontainer { color: #00A0AF; }");
                    sb.Append("#logonpage #logoninnercontainer { border: 1px solid #00A0AF; background-color: #ffffff; }");
                    sb.Append("#logonpage #logonrightpanel { border-left: 1px dotted #00A0AF; }");
                    sb.Append("#logonpage #logonleftpanel .divider { color: #ff0000; }");
                    sb.Append("#logonpage #logonleftpanel .dividerfiller { border-bottom: 1px dashed #EFEFEF; }");
                    sb.Append("#logonpage #informationcontainer { background-color: #F8F8F8; border: 1px solid #CCCCCC; color: #000000; }");
                    sb.Append("#logonpage #informationcontainer h4 { color: #000000; background-color: #EAEAEA; border-bottom: 1px solid #CCCCCC; }");
                    sb.Append("#divforgotten {background-color: #00A0AF; color: #ffffff; }");
                    sb.Append("#divforgotten:before {border-top-color: #00A0AF; border-right-color: #00A0AF; border-bottom-color: transparent; border-left-color: transparent;}");
                    sb.Append("#divlocked {background-color: #00A0AF; color: #ffffff; }");
                    sb.Append("#divlocked:before {border-top-color: #00A0AF; border-right-color: #00A0AF; border-bottom-color: transparent; border-left-color: transparent;}");
                    break;
                case Modules.SmartDiligence:
                    // Pincent Pink #C7114A
                    // Pincent Black #0C2027 ? rgb(30,35,38) ? = #1E2326
                    sb.Append("#logonpage { background: #ffffff url(../images/SmartDiligence2.png) no-repeat center 160px; }");
                    sb.Append("#logonpage #breadcrumbbar a { color: #ffffff; }");
                    sb.Append("#logonpage #breadcrumbbar { background: #1E2326 url(../images/pinsent-black.png) no-repeat 20px 20px; top: 0px; height: 100px; }");
                    sb.Append("#logonpage #pagetitlebar { background-color: #C7114A; color: #C7114A; background-image: none; top: 100px; height: 25px; font-size: 12px; }");
                    sb.Append("#logonpage #logonoutercontainer { color: #1E2326; margin-top: -30px; }");
                    sb.Append("#logonpage #logoninnercontainer { border: 1px solid #C7114A; background-color: #ffffff; }");
                    sb.Append("#logonpage #logonrightpanel { border-left: 1px dotted #C7114A; }");
                    sb.Append("#logonpage #logonleftpanel .divider { color: #ff0000; }");
                    sb.Append("#logonpage #logonleftpanel .dividerfiller { border-bottom: 1px dashed #EFEFEF; }");
                    sb.Append("#logonpage #informationcontainer { background-color: #F8F8F8; border: 1px solid #CCCCCC; color: #000000; }");
                    sb.Append("#logonpage #informationcontainer h4 { color: #000000; background-color: #EAEAEA; border-bottom: 1px solid #CCCCCC; }");
                    sb.Append("#divforgotten {background-color: #C7114A; color: #ffffff;}");
                    sb.Append("#divforgotten:before {border-top-color: #C7114A; border-right-color: #C7114A; border-bottom-color: transparent; border-left-color: transparent;}");
                    sb.Append("#divlocked {background-color: #C7114A; color: #ffffff;}");
                    sb.Append("#divlocked:before {border-top-color: #C7114A; border-right-color: #C7114A; border-bottom-color: transparent; border-left-color: transparent;}");
                    break;
                case Modules.Expenses:
                    sb.Append("#logonpage { background-color: #ffffff; }");
                    sb.Append("#logonpage #breadcrumbbar a { color: #ffffff; }");
                    sb.Append("#logonpage #breadcrumbbar { background-color: #4A65A0; }");
                    sb.Append("#logonpage #pagetitlebar { background-color: #4A65A0; color: #ffffff; }");
                    sb.Append("#logonpage #logonoutercontainer { color: #002B56; }");
                    sb.Append("#logonpage #logoninnercontainer { border: 1px solid #013473; background-color: #ffffff; }");
                    sb.Append("#logonpage #logonrightpanel { border-left: 1px dotted #013473; }");
                    sb.Append("#logonpage #logonleftpanel .divider { color: #ff0000; }");
                    sb.Append("#logonpage #logonleftpanel .dividerfiller { border-bottom: 1px dashed #EFEFEF; }");
                    sb.Append("#logonpage #informationcontainer { background-color: #F8F8F8; border: 1px solid #CCCCCC; color: #000000; }");
                    sb.Append("#logonpage #informationcontainer h4 { color: #000000; background-color: #EAEAEA; border-bottom: 1px solid #CCCCCC; }");
                    sb.Append("#divforgotten {background-color: #4A65A0; color: #ffffff; }");
                    sb.Append("#divforgotten:before {border-top-color: #4A65A0; border-right-color: #4A65A0; border-bottom-color: transparent; border-left-color: transparent;}");
                    sb.Append("#divlocked {background-color: #4A65A0; color: #ffffff; }");
                    sb.Append("#divlocked:before {border-top-color: #4A65A0; border-right-color: #4A65A0; border-bottom-color: transparent; border-left-color: transparent;}");
                    break;
                case Modules.Greenlight:
                    sb.Append("#logonpage { background-color: #ffffff; }");
                    sb.Append("#logonpage #breadcrumbbar a { color: #ffffff; }");
                    sb.Append("#logonpage #breadcrumbbar { background-image: none; background-color: #E46C0A; }");
                    sb.Append("#logonpage #pagetitlebar { background-color: #E46C0A; color: #ffffff; background-image: none; }");
                    sb.Append("#logonpage #logonoutercontainer { color: #E46C0A; }");
                    sb.Append("#logonpage #logoninnercontainer { border: 1px solid #013473; background-color: #ffffff; }");
                    sb.Append("#logonpage #logonrightpanel { border-left: 1px dotted #013473; }");
                    sb.Append("#logonpage #logonleftpanel .divider { color: #ff0000; }");
                    sb.Append("#logonpage #logonleftpanel .dividerfiller { border-bottom: 1px dashed #EFEFEF; }");
                    sb.Append("#logonpage #informationcontainer { background-color: #F8F8F8; border: 1px solid #CCCCCC; color: #000000; }");
                    sb.Append("#logonpage #informationcontainer h4 { color: #000000; background-color: #E46C0A; border-bottom: 1px solid #CCCCCC; }");
                    sb.Append("#divforgotten {background-color: #E46C0A; color: #ffffff; }");
                    sb.Append("#divforgotten:before {border-top-color: #E46C0A; border-right-color: #E46C0A; border-bottom-color: transparent; border-left-color: transparent;}");
                    sb.Append("#divlocked {background-color: #E46C0A; color: #ffffff; }");
                    sb.Append("#divlocked:before {border-top-color: #E46C0A; border-right-color: #E46C0A; border-bottom-color: transparent; border-left-color: transparent;}");
                    break;
                case Modules.GreenlightWorkforce:
                    sb.Append("#logonpage { background-color: #ffffff; }");
                    sb.Append("#logonpage #breadcrumbbar a { color: #ffffff; }");
                    sb.Append("#logonpage #breadcrumbbar { background-image: none; background-color: #5E6E66; }");
                    sb.Append("#logonpage #pagetitlebar { background-color: #E46C0A; color: #ffffff; background-image: none; }");
                    sb.Append("#logonpage #logonoutercontainer { color: #5E6E66; }");
                    sb.Append("#logonpage #logoninnercontainer { border: 1px solid #013473; background-color: #ffffff; }");
                    sb.Append("#logonpage #logonrightpanel { border-left: 1px dotted #013473; }");
                    sb.Append("#logonpage #logonleftpanel .divider { color: #ff0000; }");
                    sb.Append("#logonpage #logonleftpanel .dividerfiller { border-bottom: 1px dashed #EFEFEF; }");
                    sb.Append("#logonpage #informationcontainer { background-color: #F8F8F8; border: 1px solid #CCCCCC; color: #000000; }");
                    sb.Append("#logonpage #informationcontainer h4 { color: #000000; background-color: #E46C0A; border-bottom: 1px solid #CCCCCC; }");
                    sb.Append("#divforgotten {background-color: #5E6E66; color: #ffffff; }");
                    sb.Append("#divforgotten:before {border-top-color: #5E6E66; border-right-color: #5E6E66; border-bottom-color: transparent; border-left-color: transparent;}");
                    sb.Append("#divlocked {background-color: #5E6E66; color: #ffffff; }");
                    sb.Append("#divlocked:before {border-top-color: #5E6E66; border-right-color: #5E6E66; border-bottom-color: transparent; border-left-color: transparent;}");
                    break;
                case Modules.CorporateDiligence:
                    // Pincent Pink #C7114A
                    // Pincent Black #0C2027 ? rgb(30,35,38) ? = #1E2326
                    sb.Append("#logonpage { background: #ffffff; }");
                    sb.Append("#logonpage #breadcrumbbar a { color: #ffffff; }");
                    sb.Append("#logonpage #breadcrumbbar { background: #1E2326; }");
                    sb.Append("#logonpage #pagetitlebar { background-color: #43165A; color: #ffffff; background-image: none;}");
                    sb.Append("#logonpage #logonoutercontainer { color: #1E2326;}");
                    sb.Append("#logonpage #logoninnercontainer { border: 1px solid #43165A; background-color: #ffffff; }");
                    sb.Append("#logonpage #logonrightpanel { border-left: 1px dotted #C7114A; }");
                    sb.Append("#logonpage #logonleftpanel .divider { color: #ff0000; }");
                    sb.Append("#logonpage #logonleftpanel .dividerfiller { border-bottom: 1px dashed #EFEFEF; }");
                    sb.Append("#logonpage #informationcontainer { background-color: #F8F8F8; border: 1px solid #CCCCCC; color: #000000; }");
                    sb.Append("#logonpage #informationcontainer h4 { color: #000000; background-color: #EAEAEA; border-bottom: 1px solid #CCCCCC; }");
                    sb.Append("#divforgotten {background-color: #43165A; color: #ffffff;}");
                    sb.Append("#divforgotten:before {border-top-color: #43165A; border-right-color: #43165A; border-bottom-color: transparent; border-left-color: transparent;}");
                    break;
                case Modules.SpendManagement:
                    break;
            }

            return sb.ToString();
        }
    }
}
