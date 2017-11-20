using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
using System.Configuration;


namespace Auto_Tests.Coded_UI_Tests.Framework.Tailoring.User_Defined_Fields
{
    /// <summary>
    /// This class contains all of the Framework specific tests for User Defined Fields
    /// </summary>
    [CodedUITest]
    public class UserDefinedFieldsTests
    {
        public UserDefinedFieldsTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.UserDefinedFieldsUIMapClasses.UserDefinedFieldsUIMap cUserDefined = new UIMaps.UserDefinedFieldsUIMapClasses.UserDefinedFieldsUIMap();

        /// <summary>
        /// This test ensures every Applies To option can be selected when creating a UDF within Framwork
        /// </summary>
        [TestMethod]
        public void UserDefinedFieldsSuccessfullyAddEveryUDFForAppliesToWithinFramework()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/adminuserdefined.aspx");

            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());
            System.Data.SqlClient.SqlDataReader reader = db.GetReader("SELECT * FROM UDFAppliesTo WHERE Framework = '1'");

            while (reader.Read())
            {
                /// Press add new udf
                cUserDefined.PressAddUserDefinedFieldLink();

                /// Enter display name
                cUserDefined.EnterDisplayNameParams.UIDisplayNameEditText = reader.GetValue(2).ToString();
                cUserDefined.EnterDisplayName();

                /// Select applies to
                cUserDefined.SelectAppliesToParams.UIAppliesToComboBoxSelectedItem = reader.GetValue(3).ToString();
                cUserDefined.SelectAppliesTo();

                /// Enter description
                cUserDefined.EnterDescriptionParams.UIDescriptionEditText = reader.GetValue(4).ToString();
                cUserDefined.EnterDescription();

                /// Press save
                cUserDefined.PressUDFSaveButton();

                /// Validate the UDF has added
                cUserDefined.ValidateAddUDFExpectedValues.UI__UDFTestCarsCellInnerText = reader.GetValue(2).ToString();
                cUserDefined.ValidateAddUDF();

                /// Delete the UDF
                cUserDefined.DeleteUDF();
            }

            reader.Close();
        }


        /// <summary>
        /// This test ensures every UDF Type can be added, edited and deleted within Framework
        /// </summary>
        [TestMethod]
        public void UserDefinedFieldsSuccessfullyAddEveryUDFWithinFramework()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/adminuserdefined.aspx");

            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());
            System.Data.SqlClient.SqlDataReader reader = db.GetReader("SELECT * FROM UDFType WHERE Framework = '1'");

            #region Declare variables

            string sUDFName = string.Empty;
            string sDescription = string.Empty;
            string sTooltip = string.Empty;
            string sType = string.Empty;
            string sMaxTextLength = string.Empty;
            string sTextFormat = string.Empty;
            string sDecimalPrecision = string.Empty;
            string sYesNoDefault = string.Empty;
            string sDateFormat = string.Empty;
            string sLargeTextLength = string.Empty;
            string sLargeTextFormat = string.Empty;
            string sHyperlinkText = string.Empty;
            string sHyperlinkPath = string.Empty;
            string sRelatedTable = string.Empty;
            bool bEditing = false;
            string sListItems = string.Empty;
            string sFirstItem = string.Empty;
            string sCurrentItem = string.Empty;
            bool bComplete = false;

            #endregion

            while (reader.Read())
            {
                #region Set variables

                sUDFName = reader.GetValue(reader.GetOrdinal("UDF Name")).ToString();
                sDescription = reader.GetValue(reader.GetOrdinal("Description")).ToString();
                sTooltip = reader.GetValue(reader.GetOrdinal("Tooltip")).ToString();
                sType = reader.GetValue(reader.GetOrdinal("Type")).ToString();
                sMaxTextLength = reader.GetValue(reader.GetOrdinal("Maximum Text Length")).ToString();
                sTextFormat = reader.GetValue(reader.GetOrdinal("Text Format")).ToString();
                sDecimalPrecision = reader.GetValue(reader.GetOrdinal("Decimal Precision")).ToString();
                sYesNoDefault = reader.GetValue(reader.GetOrdinal("YesNo Default")).ToString();
                sDateFormat = reader.GetValue(reader.GetOrdinal("Date Format")).ToString();
                sLargeTextLength = reader.GetValue(reader.GetOrdinal("Large Text Length")).ToString();
                sLargeTextFormat = reader.GetValue(reader.GetOrdinal("Large Text Format")).ToString();
                sHyperlinkText = reader.GetValue(reader.GetOrdinal("Hyperlink Text")).ToString();
                sHyperlinkPath = reader.GetValue(reader.GetOrdinal("Hyperlink Path")).ToString();
                sRelatedTable = reader.GetValue(reader.GetOrdinal("Related Table")).ToString();
                sListItems = reader.GetValue(reader.GetOrdinal("List Items")).ToString();
                bComplete = false;

                #endregion


                #region Press the Add or Edit link

                if (sUDFName.EndsWith("EDITED"))
                {
                    /// Edit the UDF                    
                    cUserDefined.PressEditUserDefinedFieldLink();
                    bEditing = true;
                }
                else
                {
                    /// Add a new UDF
                    cUserDefined.PressAddUserDefinedFieldLink();
                    bEditing = false;
                }

                #endregion


                #region Enter Display name, description, tooltip and type

                /// Enter display name
                cUserDefined.EnterDisplayNameParams.UIDisplayNameEditText = sUDFName;
                cUserDefined.EnterDisplayName();

                /// Enter description
                cUserDefined.EnterDescriptionParams.UIDescriptionEditText = sDescription;
                cUserDefined.EnterDescription();

                /// Enter tooltip
                cUserDefined.EnterTooltipParams.UITooltipEditText = sTooltip;
                cUserDefined.EnterTooltip();

                if (sUDFName.EndsWith("EDITED") == false)
                {
                    /// Enter type
                    cUserDefined.SelectTypeParams.UITypeComboBoxSelectedItem = sType;
                    cUserDefined.SelectType();
                }

                #endregion

                switch (sType)
                {
                    #region Text

                    case "Text":
                        cUserDefined.EnterMaximumLengthParams.UICtl00contentmaintxtmEditText = sMaxTextLength;
                        cUserDefined.EnterMaximumLength();

                        cUserDefined.SelectTextFormatParams.UICtl00contentmaincmbtComboBoxSelectedItem = sTextFormat;
                        cUserDefined.SelectTextFormat();

                        cUserDefined.PressUDFSaveButton();

                        #region Validate the save was successful

                        cUserDefined.PressEditUserDefinedFieldLink();
                        // Display Name
                        cUserDefined.ValidateDisplayNameExpectedValues.UIDisplayNameEditText = sUDFName;
                        cUserDefined.ValidateDisplayName();
                        // Description
                        cUserDefined.ValidateDescriptionExpectedValues.UIDescriptionEditText = sDescription;
                        cUserDefined.ValidateDescription();
                        // Tooltip
                        cUserDefined.ValidateTooltipExpectedValues.UITooltipEditText = sTooltip;
                        cUserDefined.ValidateTooltip();
                        // Type
                        cUserDefined.ValidateSelectedTypeExpectedValues.UITypeComboBoxSelectedItem = sType;
                        cUserDefined.ValidateSelectedType();
                        cUserDefined.ValidateTypeEnabled();
                        // Max Length
                        cUserDefined.ValidateTextLengthExpectedValues.UICtl00contentmaintxtmEditText = sMaxTextLength;
                        cUserDefined.ValidateTextLength();
                        // Format
                        cUserDefined.ValidateTextFormatExpectedValues.UICtl00contentmaincmbtComboBoxSelectedItem = sTextFormat;
                        cUserDefined.ValidateTextFormat();

                        cUserDefined.PressUDFSaveButton();

                        #endregion

                        break;

                    #endregion

                    #region Decimal

                    case "Decimal":
                        if (bEditing == false)
                        {
                            cUserDefined.EnterPrecisionParams.UICtl00contentmaintxtpEditText = sDecimalPrecision;
                            cUserDefined.EnterPrecision();
                        }
                        cUserDefined.PressUDFSaveButton();

                        #region Validate the save was successful

                        cUserDefined.PressEditUserDefinedFieldLink();
                        // Display Name
                        cUserDefined.ValidateDisplayNameExpectedValues.UIDisplayNameEditText = sUDFName;
                        cUserDefined.ValidateDisplayName();
                        // Description
                        cUserDefined.ValidateDescriptionExpectedValues.UIDescriptionEditText = sDescription;
                        cUserDefined.ValidateDescription();
                        // Tooltip
                        cUserDefined.ValidateTooltipExpectedValues.UITooltipEditText = sTooltip;
                        cUserDefined.ValidateTooltip();
                        // Type
                        cUserDefined.ValidateSelectedTypeExpectedValues.UITypeComboBoxSelectedItem = sType;
                        cUserDefined.ValidateSelectedType();
                        cUserDefined.ValidateTypeEnabled();
                        // Precision
                        cUserDefined.ValidatePrecisioExpectedValues.UICtl00contentmaintxtpEdit1Text = sDecimalPrecision;
                        cUserDefined.ValidatePrecisio();
                        cUserDefined.ValidatePrecisionIsNotEnabled();

                        cUserDefined.PressUDFSaveButton();

                        #endregion

                        break;

                    #endregion

                    #region Yes/No

                    case "Yes/No":

                        cUserDefined.EnterYesNoDefaultParams.UICtl00contentmaincmbdComboBoxSelectedItem = sYesNoDefault;
                        cUserDefined.EnterYesNoDefault();

                        cUserDefined.PressUDFSaveButton();

                        #region Validate the save was successful

                        cUserDefined.PressEditUserDefinedFieldLink();
                        // Display Name
                        cUserDefined.ValidateDisplayNameExpectedValues.UIDisplayNameEditText = sUDFName;
                        cUserDefined.ValidateDisplayName();
                        // Description
                        cUserDefined.ValidateDescriptionExpectedValues.UIDescriptionEditText = sDescription;
                        cUserDefined.ValidateDescription();
                        // Tooltip
                        cUserDefined.ValidateTooltipExpectedValues.UITooltipEditText = sTooltip;
                        cUserDefined.ValidateTooltip();
                        // Type
                        cUserDefined.ValidateSelectedTypeExpectedValues.UITypeComboBoxSelectedItem = sType;
                        cUserDefined.ValidateSelectedType();
                        cUserDefined.ValidateTypeEnabled();
                        // Default value
                        cUserDefined.ValidateYesNoDefaultValueExpectedValues.UICtl00contentmaincmbdComboBoxSelectedItem = sYesNoDefault;
                        cUserDefined.ValidateYesNoDefaultValue();

                        cUserDefined.PressUDFSaveButton();

                        #endregion

                        break;

                    #endregion

                    #region List

                    case "List":
                                                
                        bComplete = false;
                        sCurrentItem = sListItems;

                        if (bEditing == false)
                        {
                            sFirstItem = string.Empty;
                            while (bComplete == false)
                            {                               
                                if (sCurrentItem == "")
                                {
                                    bComplete = true;
                                    break;
                                }
                                else if (sCurrentItem.Contains(","))
                                {
                                    if (sFirstItem == string.Empty)
                                    {
                                        sFirstItem = sCurrentItem.Substring(0, (sCurrentItem.IndexOf(",")));
                                    }

                                    cUserDefined.PressAddListItem();

                                    cUserDefined.EnterListItemNameParams.UICtl00contentmaintxtlEditText = sCurrentItem.Substring(0, (sCurrentItem.IndexOf(",")));
                                    cUserDefined.EnterListItemName();

                                    sCurrentItem = sCurrentItem.Remove(0, sCurrentItem.IndexOf(",") + 2).Trim();
                                }
                                else
                                {
                                    if (sFirstItem == string.Empty)
                                    {
                                        sFirstItem = sCurrentItem.Substring(0, (sCurrentItem.IndexOf(",")));
                                    }

                                    cUserDefined.PressAddListItem();

                                    cUserDefined.EnterListItemNameParams.UICtl00contentmaintxtlEditText = sCurrentItem;
                                    cUserDefined.EnterListItemName();

                                    bComplete = true;
                                    break;
                                }
                            }

                            #region Validate the save was successful

                            cUserDefined.PressUDFSaveButton();
                            cUserDefined.PressEditUserDefinedFieldLink();
                            // Display Name
                            cUserDefined.ValidateDisplayNameExpectedValues.UIDisplayNameEditText = sUDFName;
                            cUserDefined.ValidateDisplayName();
                            // Description
                            cUserDefined.ValidateDescriptionExpectedValues.UIDescriptionEditText = sDescription;
                            cUserDefined.ValidateDescription();
                            // Tooltip
                            cUserDefined.ValidateTooltipExpectedValues.UITooltipEditText = sTooltip;
                            cUserDefined.ValidateTooltip();
                            // Type
                            cUserDefined.ValidateSelectedTypeExpectedValues.UITypeComboBoxSelectedItem = sType;
                            cUserDefined.ValidateSelectedType();
                            cUserDefined.ValidateTypeEnabled();
                            // List items
                            if (sListItems == string.Empty)
                            {
                                cUserDefined.ValidateThereAreNoListItemsExpectedValues.UICtl00contentmainlstiListInnerText = null;
                                cUserDefined.ValidateThereAreNoListItems();
                            }
                            else
                            {
                                cUserDefined.ValidateListItemsExpectedValues.UICtl00contentmainlstiListInnerText = " " + sListItems.Replace(",", "");
                                cUserDefined.ValidateListItems();
                            }
                            cUserDefined.PressUDFSaveButton();

                            #endregion
                        }
                        else
                        {
                            if (sFirstItem == string.Empty)
                            {
                                // If there are no list items, add instead of edit
                                cUserDefined.PressAddListItem();

                                cUserDefined.EnterListItemNameParams.UICtl00contentmaintxtlEditText = sListItems;
                                cUserDefined.EnterListItemName();
                            }
                            else
                            {
                                cUserDefined.SelectAndPressEditForFirstItemParams.UICtl00contentmainlstiListSelectedItemsAsString = sFirstItem;
                                cUserDefined.SelectAndPressEditForFirstItem();

                                if (sListItems.Contains(","))
                                {
                                    cUserDefined.EnterListItemNameParams.UICtl00contentmaintxtlEditText = sListItems.Substring(0, sListItems.IndexOf(","));
                                    cUserDefined.EnterListItemName();
                                }
                                else
                                {
                                    cUserDefined.EnterListItemNameParams.UICtl00contentmaintxtlEditText = sListItems;
                                    cUserDefined.EnterListItemName();
                                }
                            }

                            #region Validate the save was successful

                            cUserDefined.PressUDFSaveButton();
                            cUserDefined.PressEditUserDefinedFieldLink();
                            // Display Name
                            cUserDefined.ValidateDisplayNameExpectedValues.UIDisplayNameEditText = sUDFName;
                            cUserDefined.ValidateDisplayName();
                            // Description
                            cUserDefined.ValidateDescriptionExpectedValues.UIDescriptionEditText = sDescription;
                            cUserDefined.ValidateDescription();
                            // Tooltip
                            cUserDefined.ValidateTooltipExpectedValues.UITooltipEditText = sTooltip;
                            cUserDefined.ValidateTooltip();
                            // Type
                            cUserDefined.ValidateSelectedTypeExpectedValues.UITypeComboBoxSelectedItem = sType;
                            cUserDefined.ValidateSelectedType();
                            cUserDefined.ValidateTypeEnabled();
                            // List items
                            if (sListItems == string.Empty)
                            {
                                cUserDefined.ValidateThereAreNoListItemsExpectedValues.UICtl00contentmainlstiListInnerText = null;
                                cUserDefined.ValidateThereAreNoListItems();
                            }
                            else
                            {
                                cUserDefined.ValidateListItemsExpectedValues.UICtl00contentmainlstiListInnerText = " " + sListItems.Replace(",", "");
                                cUserDefined.ValidateListItems();
                            }

                            cUserDefined.PressUDFSaveButton();

                            #endregion
                        }

                        break;

                    #endregion

                    #region Date

                    case "Date":
                        if (bEditing == false)
                        {
                            cUserDefined.SelectDateFormatParams.UICtl00contentmaincmbdComboBox1SelectedItem = sDateFormat;
                            cUserDefined.SelectDateFormat();
                        }
                        cUserDefined.PressUDFSaveButton();

                        #region Validate the save was successful

                        cUserDefined.PressEditUserDefinedFieldLink();
                        // Display Name
                        cUserDefined.ValidateDisplayNameExpectedValues.UIDisplayNameEditText = sUDFName;
                        cUserDefined.ValidateDisplayName();
                        // Description
                        cUserDefined.ValidateDescriptionExpectedValues.UIDescriptionEditText = sDescription;
                        cUserDefined.ValidateDescription();
                        // Tooltip
                        cUserDefined.ValidateTooltipExpectedValues.UITooltipEditText = sTooltip;
                        cUserDefined.ValidateTooltip();
                        // Type
                        cUserDefined.ValidateSelectedTypeExpectedValues.UITypeComboBoxSelectedItem = sType;
                        cUserDefined.ValidateSelectedType();
                        cUserDefined.ValidateTypeEnabled();
                        // Format
                        cUserDefined.ValidateDateFormatExpectedValues.UICtl00contentmaincmbdComboBox1SelectedItem = sDateFormat;
                        cUserDefined.ValidateDateFormat();
                        cUserDefined.ValidateDateFormatIsNotEnabled();

                        cUserDefined.PressUDFSaveButton();

                        #endregion

                        break;

                    #endregion

                    #region Large Text

                    case "Large Text":
                        cUserDefined.EnterLargeTextMaximumLengthParams.UICtl00contentmaintxtmEdit1Text = sLargeTextLength;
                        cUserDefined.EnterLargeTextMaximumLength();

                        cUserDefined.SelectLargeTextFormatParams.UICtl00contentmaincmbtComboBox1SelectedItem = sLargeTextFormat;
                        cUserDefined.SelectLargeTextFormat();

                        cUserDefined.PressUDFSaveButton();

                        #region Validate the save was successful

                        cUserDefined.PressEditUserDefinedFieldLink();
                        // Display Name
                        cUserDefined.ValidateDisplayNameExpectedValues.UIDisplayNameEditText = sUDFName;
                        cUserDefined.ValidateDisplayName();
                        // Description
                        cUserDefined.ValidateDescriptionExpectedValues.UIDescriptionEditText = sDescription;
                        cUserDefined.ValidateDescription();
                        // Tooltip
                        cUserDefined.ValidateTooltipExpectedValues.UITooltipEditText = sTooltip;
                        cUserDefined.ValidateTooltip();
                        // Type
                        cUserDefined.ValidateSelectedTypeExpectedValues.UITypeComboBoxSelectedItem = sType;
                        cUserDefined.ValidateSelectedType();
                        cUserDefined.ValidateTypeEnabled();
                        // Max length and Format
                        cUserDefined.ValidateLargeTextMaximumLengthExpectedValues.UICtl00contentmaintxtmEdit1Text = sLargeTextLength;
                        cUserDefined.ValidateLargeTextMaximumLength();

                        cUserDefined.ValidateLargeTextFormatExpectedValues.UICtl00contentmaincmbtComboBox1SelectedItem = sLargeTextFormat;
                        cUserDefined.ValidateLargeTextFormat();

                        cUserDefined.PressUDFSaveButton();

                        #endregion

                        break;

                    #endregion

                    #region Hyperlink

                    case "Hyperlink":
                        cUserDefined.EnterHyperlinkTextParams.UICtl00contentmaintxtHEdit2Text = sHyperlinkText;
                        cUserDefined.EnterHyperlinkText();

                        cUserDefined.EnterHyperlinkPathParams.UICtl00contentmaintxtHEdit1Text = sHyperlinkPath;
                        cUserDefined.EnterHyperlinkPath();

                        cUserDefined.PressUDFSaveButton();

                        #region Validate the save was successful

                        cUserDefined.PressEditUserDefinedFieldLink();
                        // Display Name
                        cUserDefined.ValidateDisplayNameExpectedValues.UIDisplayNameEditText = sUDFName;
                        cUserDefined.ValidateDisplayName();
                        // Description
                        cUserDefined.ValidateDescriptionExpectedValues.UIDescriptionEditText = sDescription;
                        cUserDefined.ValidateDescription();
                        // Tooltip
                        cUserDefined.ValidateTooltipExpectedValues.UITooltipEditText = sTooltip;
                        cUserDefined.ValidateTooltip();
                        // Type
                        cUserDefined.ValidateSelectedTypeExpectedValues.UITypeComboBoxSelectedItem = sType;
                        cUserDefined.ValidateSelectedType();
                        cUserDefined.ValidateTypeEnabled();
                        // Hyperlink text and Path
                        cUserDefined.ValidateHyperlinkTextExpectedValues.UICtl00contentmaintxtHEdit2Text = sHyperlinkText;
                        cUserDefined.ValidateHyperlinkText();

                        cUserDefined.ValidateHyperlinkPathExpectedValues.UICtl00contentmaintxtHEdit3Text = sHyperlinkPath;
                        cUserDefined.ValidateHyperlinkPath();

                        cUserDefined.PressUDFSaveButton();

                        #endregion

                        break;

                    #endregion

                    #region Relationship TextBox

                    case "Relationship TextBox":
                        if (bEditing == false)
                        {
                            cUserDefined.SelectRelatedTableParams.UIRelatedTableComboBoxSelectedItem = sRelatedTable;
                            cUserDefined.SelectRelatedTable();
                        }
                        cUserDefined.PressUDFSaveButton();

                        #region Validate the save was successful

                        cUserDefined.PressEditUserDefinedFieldLink();
                        // Display Name
                        cUserDefined.ValidateDisplayNameExpectedValues.UIDisplayNameEditText = sUDFName;
                        cUserDefined.ValidateDisplayName();
                        // Description
                        cUserDefined.ValidateDescriptionExpectedValues.UIDescriptionEditText = sDescription;
                        cUserDefined.ValidateDescription();
                        // Tooltip
                        cUserDefined.ValidateTooltipExpectedValues.UITooltipEditText = sTooltip;
                        cUserDefined.ValidateTooltip();
                        // Type
                        cUserDefined.ValidateSelectedTypeExpectedValues.UITypeComboBoxSelectedItem = sType;
                        cUserDefined.ValidateSelectedType();
                        cUserDefined.ValidateTypeEnabled();
                        // Related Table
                        cUserDefined.ValidateRelatedTableExpectedValues.UIRelatedTableComboBoxSelectedItem = sRelatedTable;
                        cUserDefined.ValidateRelatedTable();
                        cUserDefined.ValidateRelatedTableIsNotEnabled();

                        cUserDefined.PressUDFSaveButton();

                        #endregion

                        break;

                    #endregion

                    #region All Others

                    default:
                        cUserDefined.PressUDFSaveButton();

                        #region Validate the save was successful

                        cUserDefined.PressEditUserDefinedFieldLink();
                        // Display Name
                        cUserDefined.ValidateDisplayNameExpectedValues.UIDisplayNameEditText = sUDFName;
                        cUserDefined.ValidateDisplayName();
                        // Description
                        cUserDefined.ValidateDescriptionExpectedValues.UIDescriptionEditText = sDescription;
                        cUserDefined.ValidateDescription();
                        // Tooltip
                        cUserDefined.ValidateTooltipExpectedValues.UITooltipEditText = sTooltip;
                        cUserDefined.ValidateTooltip();
                        // Type
                        cUserDefined.ValidateSelectedTypeExpectedValues.UITypeComboBoxSelectedItem = sType;
                        cUserDefined.ValidateSelectedType();
                        cUserDefined.ValidateTypeEnabled();

                        cUserDefined.PressUDFSaveButton();

                        #endregion

                        break;

                    #endregion
                }

                #region Delete the UDF after it has been added and edited

                if (sUDFName.EndsWith("EDITED"))
                {
                    /// Delete the UDF
                    cUserDefined.DeleteUDF();
                }

                #endregion
            }

            reader.Close();             
        }


        /// <summary>
        /// This test ensures a UDF with duplicate details cannot be created
        /// </summary>
        [TestMethod]
        public void UserDefinedFieldUnSuccessfullyAddUDFWithDuplicateDetailsWithinFramework()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the UDF page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/adminuserdefined.aspx");

            /// Create a UDF
            cUserDefined.PressAddUserDefinedFieldLink();
            cUserDefined.EnterDisplayName();
            cUserDefined.EnterDescription();
            cUserDefined.EnterTooltip();
            cUserDefined.PressUDFSaveButton();

            /// Attemp to create a UDF with the same details
            cUserDefined.PressAddUserDefinedFieldLink();
            cUserDefined.EnterDisplayName();
            cUserDefined.EnterDescription();
            cUserDefined.EnterTooltip();
            cUserDefined.PressUDFSaveButton();

            /// Validate the error message and press ok
            cUserDefined.ValidateDuplicateMessage();
            cUserDefined.CancelAddDuplicateUDFMessage();

            /// Delete UDF for future tests
            cUserDefined.PressDeleteIconForDuplicateUDFTests();
        }


        /// <summary>
        /// This test ensures a UDF can be created that has Allow Search is set
        /// </summary>
        [TestMethod]
        public void UserDefinedFieldSuccessfullyAddUDFWhereAllowSearchIsSetWithinFramework()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/adminuserdefined.aspx");
            cUserDefined.PressAddUserDefinedFieldLink();

            /// Add a UDF that is not set to allow search
            cUserDefined.EnterDisplayName();
            cUserDefined.EnterDescription();
            cUserDefined.EnterTooltip();
            cUserDefined.SelectType();
            cUserDefined.PressUDFSaveButton();

            /// Edit the UDF to allow search
            cUserDefined.PressEditUserDefinedFieldLink();
            cUserDefined.ValidateAllowSearch();
            cUserDefined.TickAllowSearch();
            cUserDefined.PressUDFSaveButton();

            /// Validate the changes
            cUserDefined.PressEditUserDefinedFieldLink();
            cUserDefined.ValidateAllowSearchExpectedValues.UIAllowSearchCheckBoxChecked = true;
            cUserDefined.ValidateAllowSearch();
            cUserDefined.PressUDFSaveButton();
            cUserDefined.DeleteUDF();

            /// Add a UDF that is set to allow search
            cUserDefined.PressAddUserDefinedFieldLink();
            cUserDefined.EnterDisplayName();
            cUserDefined.EnterDescription();
            cUserDefined.EnterTooltip();
            cUserDefined.SelectType();
            cUserDefined.TickAllowSearch();
            cUserDefined.PressUDFSaveButton();

            /// Validate the changes
            cUserDefined.PressEditUserDefinedFieldLink();
            cUserDefined.ValidateAllowSearchExpectedValues.UIAllowSearchCheckBoxChecked = true;
            cUserDefined.ValidateAllowSearch();
            cUserDefined.PressUDFSaveButton();
            cUserDefined.DeleteUDF();
        }


        /// <summary>
        /// This test ensures a UDF can be created that has a Group set
        /// </summary>
        [TestMethod]
        public void UserDefinedFieldSuccessfullyAddUDFWhereGroupIsSetWithinFramework()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/adminuserdefined.aspx");

            #region Create a UDF without a group and edit it to include a group

            /// Add a UDF that has no group set
            cUserDefined.PressAddUserDefinedFieldLink();
            cUserDefined.EnterDisplayName();
            cUserDefined.EnterDescription();
            cUserDefined.EnterTooltip();
            cUserDefined.SelectAppliesToParams.UIAppliesToComboBoxSelectedItem = "Employees";
            cUserDefined.SelectAppliesTo();
            cUserDefined.PressUDFSaveButton();

            /// Edit the UDF to include a Group
            cUserDefined.PressEditUserDefinedFieldLink();
            cUserDefined.ValidateGroupExpectedValues.UIGroupComboBoxSelectedItem = "[None]";
            cUserDefined.ValidateGroup();
            cUserDefined.SelectGroupParams.UIGroupComboBoxSelectedItem = "CodedUI 2 Do Not Delete";
            cUserDefined.SelectGroup();
            cUserDefined.PressUDFSaveButton();

            /// Validate the changes
            cUserDefined.PressEditUserDefinedFieldLink();
            cUserDefined.ValidateGroupExpectedValues.UIGroupComboBoxSelectedItem = "CodedUI 2 Do Not Delete";
            cUserDefined.ValidateGroup();
            cUserDefined.PressUDFSaveButton();
            cUserDefined.DeleteUDF();

            #endregion

            #region Create a UDF with a group and edit it to another group

            /// Add a UDF that has a group set
            cUserDefined.PressAddUserDefinedFieldLink();
            cUserDefined.EnterDisplayName();
            cUserDefined.EnterDescription();
            cUserDefined.EnterTooltip();
            cUserDefined.SelectAppliesToParams.UIAppliesToComboBoxSelectedItem = "Employees";
            cUserDefined.SelectAppliesTo();
            cUserDefined.SelectGroup();
            cUserDefined.PressUDFSaveButton();

            /// Edit the UDF to include a different Group
            cUserDefined.PressEditUserDefinedFieldLink();
            cUserDefined.ValidateGroup();
            cUserDefined.SelectGroupParams.UIGroupComboBoxSelectedItem = "CodedUI 1 Do Not Delete";
            cUserDefined.SelectGroup();
            cUserDefined.PressUDFSaveButton();

            /// Validate the changes
            cUserDefined.PressEditUserDefinedFieldLink();
            cUserDefined.ValidateGroupExpectedValues.UIGroupComboBoxSelectedItem = "CodedUI 1 Do Not Delete";
            cUserDefined.ValidateGroup();
            cUserDefined.PressUDFSaveButton();
            cUserDefined.DeleteUDF();

            #endregion

            #region Create a UDF with a group and edit it to remove the group

            /// Add a UDF that has a group set
            cUserDefined.PressAddUserDefinedFieldLink();
            cUserDefined.EnterDisplayName();
            cUserDefined.EnterDescription();
            cUserDefined.EnterTooltip();
            cUserDefined.SelectAppliesToParams.UIAppliesToComboBoxSelectedItem = "Employees";
            cUserDefined.SelectAppliesTo();
            cUserDefined.SelectGroup();
            cUserDefined.PressUDFSaveButton();

            /// Edit the UDF to include no Group
            cUserDefined.PressEditUserDefinedFieldLink();
            cUserDefined.ValidateGroup();
            cUserDefined.SelectGroupParams.UIGroupComboBoxSelectedItem = "[None]";
            cUserDefined.SelectGroup();
            cUserDefined.PressUDFSaveButton();

            /// Validate the changes
            cUserDefined.PressEditUserDefinedFieldLink();
            cUserDefined.ValidateGroupExpectedValues.UIGroupComboBoxSelectedItem = "[None]";
            cUserDefined.ValidateGroup();
            cUserDefined.PressUDFSaveButton();
            cUserDefined.DeleteUDF();

            #endregion

        }



        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        //}

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        //}

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;
    }
}
