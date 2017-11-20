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
using System.Threading;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Auto_Tests.UIMaps.UserDefinedFieldsUIMapClasses;


namespace Auto_Tests.Coded_UI_Tests.Expenses.Tailoring.User_Defined_Fields
{
    /// <summary>
    /// This class contains all of the Expenses specific tests for User Defined Fields
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
        /// This test ensures every Applies To option can be selected when creating a UDF within Expenses
        /// </summary>
        [TestMethod]
        public void UserDefinedFieldsSuccessfullyAddEveryUDFForAppliesToWithinExpenses()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/adminuserdefined.aspx");

            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());
            System.Data.SqlClient.SqlDataReader reader = db.GetReader("SELECT * FROM UDFAppliesTo WHERE Expenses = '1'");

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
                /// 
                cUserDefined.ValidateAddedUDF(cSharedMethods.browserWindow, reader.GetValue(2).ToString());

                cUserDefined.ClickDeleteUdfField(cSharedMethods.browserWindow, cUserDefined.EnterDisplayNameParams.UIDisplayNameEditText);
            }

            reader.Close();
        }


        /// <summary>
        /// This test ensures every UDF Type can be added, edited and deleted within Expenses
        /// </summary>
        [TestMethod]
        public void UserDefinedFieldsSuccessfullyAddEveryUDFWithinExpenses()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/adminuserdefined.aspx");

            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());
            System.Data.SqlClient.SqlDataReader reader = db.GetReader("SELECT * FROM UDFType WHERE Expenses = '1'");

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
            string sUDFNameToUpdate = string.Empty;
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
                    cUserDefined.ClickEditFieldLink(cSharedMethods.browserWindow, sUDFNameToUpdate);
                    //cUserDefined.PressEditUserDefinedFieldLink();
                    bEditing = true;
                }
                else
                {
                    /// Add a new UDF
                    cUserDefined.PressAddUserDefinedFieldLink();
                    bEditing = false;
                    sUDFNameToUpdate = sUDFName;
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

                    if (sType == "Yes/No" || sType == "Hyperlink")
                    {
                        cUserDefined.ValidateMandatoryIsNotEnabled();
                        cUserDefined.ValidateMandatory();
                    }
                    else
                    {
                        cUserDefined.ValidateMandatoryIsEnabled();
                        cUserDefined.ValidateMandatory();
                    }

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

                        //cUserDefined.PressEditUserDefinedFieldLink();
                        Thread.Sleep(1000);
                        cUserDefined.ClickEditFieldLink(cSharedMethods.browserWindow, sUDFName);
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

                        cUserDefined.ClickEditFieldLink(cSharedMethods.browserWindow, sUDFName);
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

                        cUserDefined.ClickEditFieldLink(cSharedMethods.browserWindow, sUDFName);
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
                            cUserDefined.ClickEditFieldLink(cSharedMethods.browserWindow, sUDFName);
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
                            cUserDefined.ClickEditFieldLink(cSharedMethods.browserWindow, sUDFName);
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

                        cUserDefined.ClickEditFieldLink(cSharedMethods.browserWindow, sUDFName);
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

                        cUserDefined.ClickEditFieldLink(cSharedMethods.browserWindow, sUDFName);
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

                        cUserDefined.ClickEditFieldLink(cSharedMethods.browserWindow, sUDFName);
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

                    #region All Others

                    default:
                        cUserDefined.PressUDFSaveButton();

                        #region Validate the save was successful

                        cUserDefined.ClickEditFieldLink(cSharedMethods.browserWindow, sUDFName);
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
                    ///
                    cUserDefined.ClickDeleteUdfField(cSharedMethods.browserWindow, sUDFName);
                    //cUserDefined.DeleteUDF();
                }

                #endregion
            }

            reader.Close();
        }


        /// <summary>
        /// This test ensures a UDF with duplicate details cannot be created
        /// </summary>
        [TestMethod]
        public void UserDefinedFieldUnSuccessfullyAddUDFWithDuplicateDetailsWithinExpenses()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the UDF page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/adminuserdefined.aspx");

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
            
            /// Validate the error message and press OK
            cUserDefined.ValidateDuplicateMessage();
            cUserDefined.CancelAddDuplicateUDFMessage();

            /// Delete duplicate UDF for future tests
            cUserDefined.PressDeleteIconForDuplicateUDFTests();
        }


        /// <summary>
        /// This test ensures a UDF can be created that is Item Specific
        /// </summary>
        [TestMethod]
        public void UserDefinedFieldSuccessfullyAddUDFWhereItemSpecificIsSetWithinExpenses()
        {
            RestoreDefaultSortingOrder(Auto_Tests.Tools.EnumHelper.TableSortOrder.ASC);

            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/adminuserdefined.aspx");

            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());
            System.Data.SqlClient.SqlDataReader reader = db.GetReader("SELECT * FROM UDFAppliesTo WHERE Expenses = '1'");

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

                if (reader.GetValue(3).ToString() == "Expense Items")
                {
                    cUserDefined.TickItemSpecific();

                    /// Press save
                    cUserDefined.PressUDFSaveButton();

                    /// Validate the UDF has added
                    cUserDefined.PressEditUserDefinedFieldLink();

                    cUserDefined.ValidateItemSpecificIsTicked();
                }
                else
                {
                    cUserDefined.ValidateItemSpecificIsNotEnabled();
                }

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
        /// This test ensures a UDF can be created that is Mandatory
        /// </summary>
        [TestMethod]
        public void UserDefinedFieldSuccessfullyAddUDFWhereMandatoryIsSetWithinExpenses()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/adminuserdefined.aspx");
            cUserDefined.PressAddUserDefinedFieldLink();

            /// Add a UDF that is not set to mandatory
            cUserDefined.EnterDisplayName();
            cUserDefined.EnterDescription();
            cUserDefined.EnterTooltip();
            cUserDefined.SelectType();
            cUserDefined.PressUDFSaveButton();

            /// Edit the UDF to make it mandatory
            cUserDefined.PressEditUserDefinedFieldLink();
            cUserDefined.ValidateMandatory();
            cUserDefined.TickMandatory();
            cUserDefined.PressUDFSaveButton();

            /// Validate the changes
            cUserDefined.PressEditUserDefinedFieldLink();
            cUserDefined.ValidateMandatoryIsTicked();
            cUserDefined.PressUDFSaveButton();
            cUserDefined.DeleteUDF();

            /// Add a UDF that is set to mandatory
            cUserDefined.PressAddUserDefinedFieldLink();
            cUserDefined.EnterDisplayName();
            cUserDefined.EnterDescription();
            cUserDefined.EnterTooltip();
            cUserDefined.SelectType();
            cUserDefined.SelectMandatory();
            cUserDefined.PressUDFSaveButton();

            /// Validate the changes
            cUserDefined.PressEditUserDefinedFieldLink();
            cUserDefined.ValidateMandatoryIsTicked();
            cUserDefined.PressUDFSaveButton();
            cUserDefined.DeleteUDF();
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

        [TestMethod]
        public void ValidateTableSortingOrders()
        {
            RestoreDefaultSortingOrder(Auto_Tests.Tools.EnumHelper.TableSortOrder.ASC);

            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to the page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/adminuserdefined.aspx");
            
            //Ensure Table is sorted correctly

            //Sorts Display name
            HtmlHyperlink displayNameLink = cUserDefined.UIUserdefinedFieldsWinWindow1.UIUserdefinedFieldsDocument.UITbl_gridFieldsTable1.UIDisplayNameHyperlink;
            cUserDefined.ClickTableHeader(displayNameLink);
            cUserDefined.DoesTableContainCorrectElements(SortByColumn.DisplayName, Auto_Tests.Tools.EnumHelper.TableSortOrder.DESC);
            cUserDefined.ClickTableHeader(displayNameLink);
            cUserDefined.DoesTableContainCorrectElements(SortByColumn.DisplayName, Auto_Tests.Tools.EnumHelper.TableSortOrder.ASC);
            
            //Sorts Description
            displayNameLink = cUserDefined.UIUserdefinedFieldsWinWindow1.UIUserdefinedFieldsDocument.UITbl_gridFieldsTable1.UIDescriptionHyperlink;
            cUserDefined.ClickTableHeader(displayNameLink);
            cUserDefined.DoesTableContainCorrectElements(SortByColumn.Description, Auto_Tests.Tools.EnumHelper.TableSortOrder.ASC);
            cUserDefined.ClickTableHeader(displayNameLink);
            cUserDefined.DoesTableContainCorrectElements(SortByColumn.Description, Auto_Tests.Tools.EnumHelper.TableSortOrder.DESC);

            //Sorts FieldType
            displayNameLink = cUserDefined.UIUserdefinedFieldsWinWindow1.UIUserdefinedFieldsDocument.UITbl_gridFieldsTable2.UIFieldTypeHyperlink;
            cUserDefined.ClickTableHeader(displayNameLink);
            cUserDefined.DoesTableContainCorrectElements(SortByColumn.FieldType, Auto_Tests.Tools.EnumHelper.TableSortOrder.ASC);
            cUserDefined.ClickTableHeader(displayNameLink);
            cUserDefined.DoesTableContainCorrectElements(SortByColumn.FieldType, Auto_Tests.Tools.EnumHelper.TableSortOrder.DESC);

            //Sorts Mandatory
            displayNameLink = cUserDefined.UIUserdefinedFieldsWinWindow1.UIUserdefinedFieldsDocument.UITbl_gridFieldsTable2.UIMandatoryHyperlink;
            cUserDefined.ClickTableHeader(displayNameLink);
            cUserDefined.DoesTableContainCorrectElements(SortByColumn.Mandatory, Auto_Tests.Tools.EnumHelper.TableSortOrder.ASC);
            cUserDefined.ClickTableHeader(displayNameLink);
            cUserDefined.DoesTableContainCorrectElements(SortByColumn.Mandatory, Auto_Tests.Tools.EnumHelper.TableSortOrder.DESC);

            //Sorts AppliesTo
            displayNameLink = cUserDefined.UIUserdefinedFieldsWinWindow1.UIUserdefinedFieldsDocument.UITbl_gridFieldsTable2.UIAppliesToHyperlink;
            cUserDefined.ClickTableHeader(displayNameLink);
            cUserDefined.DoesTableContainCorrectElements(SortByColumn.AppliesTo, Auto_Tests.Tools.EnumHelper.TableSortOrder.ASC);
            cUserDefined.ClickTableHeader(displayNameLink);
            cUserDefined.DoesTableContainCorrectElements(SortByColumn.AppliesTo, Auto_Tests.Tools.EnumHelper.TableSortOrder.DESC);
        }
        
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

        private void RestoreDefaultSortingOrder(Auto_Tests.Tools.EnumHelper.TableSortOrder sortingOrder)
        {
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(ProductType.expenses));
            string strSQL1 = "DELETE FROM employeeGridSortOrders WHERE employeeID = 21333 AND gridID = 'gridFields'";
            dbex_CodedUI.ExecuteSQL(strSQL1);
            dbex_CodedUI.sqlexecute.Parameters.Clear();
            string strSQL2 = "INSERT INTO employeeGridSortOrders VALUES ('21333', 'gridFields', 'B384F241-1CF5-4C9E-8BBE-07880AB2A0A4', @Order)";
            //string strSQL2 = "UPDATE employeeGridSortOrders SET sortOrder = @Order WHERE employeeID = 21333 and gridID = 'gridFields' ";
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@Order", (int)sortingOrder);
            dbex_CodedUI.ExecuteSQL(strSQL2);
            dbex_CodedUI.sqlexecute.Parameters.Clear();
        }
    }
}
