using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auto_Tests.Product_Variables.ModalMessages
{
    /// <summary>
    /// The view modal messages.
    /// </summary>
    internal class ViewModalMessages
    {
        #region Modal Messages for Views Filter Criteria
        /// <summary>
        /// Message Displayed When Filter Criteria is Left Blank
        /// </summary>
        internal const string EmptyFieldForFilterCriteria = "\r\nPlease select a value for Filter criteria.";
        /// <summary>
        /// Message Displayed When Value 1 is Left Blank
        /// </summary>
        internal const string EmptyFieldForValue1 = "\r\nPlease enter a value for Value 1.";

        /// <summary>
        /// Message Displayed When Number 1 is Left Blank
        /// </summary>
        internal const string EmptyFieldForNumber1 = "\r\nPlease enter a whole number for Number 1.";
        /// <summary>
        /// Message Displayed When Number 2 is Left Blank
        /// </summary>
        internal const string EmptyFieldForNumber2 = "\r\nPlease enter a whole number for Number 2.";
        /// <summary>
        /// Message Displayed When Number 1 is Nan
        /// </summary>
        internal const string InvalidFieldForNumber1 = "\r\nPlease enter a valid whole number for Number 1.";
        /// <summary>
        /// Message Displayed When Number 2 is NaN
        /// </summary>
        internal const string InvalidFieldForNumber2 = "\r\nPlease enter a valid whole number for Number 2.";
        /// <summary>
        /// Message appended to invalidField message When Number 1 is Nan specified rane for Number 1
        /// </summary>
        internal const string ValidFieldRangeForNumber1 = "\r\nPlease enter a whole number between -2147483647 and 2147483647 for Number 1.";
        /// <summary>
        /// Message appended to invalidField message When Number 2 is Nan specified rane for Number 2
        /// </summary>
        internal const string ValidFieldRangeForNumber2 = "\r\nPlease enter a whole number between -2147483647 and 2147483647 for Number 2.";
        /// <summary>
        /// Message Displayed When Number 1 is Greater than Number 2
        /// </summary>
        internal const string InvalidFieldForNumber2GreaterThanNumber1 = "\r\nPlease enter a valid whole number greater than Number 1 for Number 2.";

        /// <summary>
        /// Message Displayed When Decimal 1 is Left Blank
        /// </summary>
        internal const string EmptyFieldForDecimal1 = "\r\nPlease enter a number for Number 1.";
        /// <summary>
        /// Message Displayed When Decimal 2 is Left Blank
        /// </summary>
        internal const string EmptyFieldForDecimal2 = "\r\nPlease enter a number for Number 2.";
        /// <summary>
        /// Message Displayed When Decimal 1 is NaN
        /// </summary>
        internal const string InvalidFieldForDecimal1 = "\r\nPlease enter a valid number for Number 1.";
        /// <summary>
        /// Message Displayed When Decimal 2 is NaN
        /// </summary>
        internal const string InvalidFieldForDecimal2 = "\r\nPlease enter a valid number for Number 2.";
        /// <summary>
        /// Message Displayed When Decimal 1 is Greater than Decimal 2
        /// </summary>
        internal const string InvalidFieldForDecimal1GreaterThanDecimal2 = "\r\nPlease enter a valid number greater than Number 1 for Number 2.";

        /// <summary>
        /// Message Displayed When Currency 2 is Left Blank
        /// </summary>
        internal const string EmptyFieldForCurrency1 = "\r\nPlease enter an amount for Currency amount 1.";
        /// <summary>
        /// Message Displayed When Currency 2 is Left Blank
        /// </summary>
        internal const string EmptyFieldForCurrency2 = "\r\nPlease enter an amount for Currency amount 2.";
        /// <summary>
        /// Message Displayed When Currency 1 is NaN
        /// </summary>
        internal const string InvalidFieldForCurrency1 = "\r\nPlease enter a valid number for Currency amount 1.";
        /// <summary>
        /// Message Displayed When Currency 2 is NaN
        /// </summary>
        internal const string InvalidFieldForCurrency2 = "\r\nPlease enter a valid number for Currency amount 2.";
        /// <summary>
        /// Message Displayed When Currency 1 is Greater than Currency 2
        /// </summary>
        internal const string InvalidFieldForCurrency1GreaterThanCurrency2 = "\r\nPlease enter a valid number greater than Currency amount 1 for Currency amount 2.";

        /// <summary>
        /// Message Displayed When YesNo 2 is Left as None
        /// </summary>
        internal const string EmptyDropDownForYesOrNo = "\r\nPlease select a value for Yes or No.";

        /// <summary>
        /// Message Displayed When Date 1 is Left Blank
        /// </summary>
        internal const string EmptyFieldForDate1 = "\r\nPlease enter a date for Date 1.";
        /// <summary>
        /// Message Displayed When Date 2 is Left Blank
        /// </summary>
        internal const string EmptyFieldForDate2 = "\r\nPlease enter a date for Date 2.";
        /// <summary>
        /// Message Displayed When Days is Left Blank
        /// </summary>

        internal const string EmptyFieldForDays = "\r\nPlease enter a whole number for Number of days.";
        /// <summary>
        /// Message Displayed When Days is Left Blank
        /// </summary>
        internal const string EmptyFieldForWeeks = "\r\nPlease enter a whole number for Number of weeks.";
        /// <summary>
        /// Message Displayed When Days is Left Blank
        /// </summary>
        internal const string EmptyFieldForMonths = "\r\nPlease enter a whole number for Number of months.";
        /// <summary>
        /// Message Displayed When Days is Left Blank
        /// </summary>
        internal const string EmptyFieldForYears = "\r\nPlease enter a whole number for Number of years.";
        /// <summary>
        /// Message Displayed When Date 1 is NaD
        /// </summary>

        internal const string InvalidFieldForDate1 = "\r\nPlease enter a valid date for Date 1.";
        /// <summary>
        /// Message Displayed When Date 2 is NaD
        /// </summary>
        internal const string InvalidFieldForDate2 = "\r\nPlease enter a valid date for Date 2.";
        /// <summary>
        /// Message Displayed When Date 1 is greater than Date 2
        /// </summary>
        internal const string InvalidFieldForDate1GreaterThanDate2 = "\r\nPlease enter a valid date greater than Date 1 for Date 2.";

        /// <summary>
        /// Message Displayed When Days is NaN
        /// </summary>
        internal const string InvalidFieldForDays = "\r\nPlease enter a valid whole number for Number of days.";
        /// <summary>
        /// Message Displayed When Days is outside of range
        /// </summary>
        internal const string ValidFieldRangeForDays = "\r\nPlease enter a whole number between 1 and 9999 for Number of days.";
        /// <summary>
        /// Message Displayed When Weeks is NaN
        /// </summary>
        internal const string InvalidFieldForWeeks = "\r\nPlease enter a valid whole number for Number of weeks.";
        /// <summary>
        /// Message Displayed When Weeks is outside of range
        /// </summary>
        internal const string ValidFieldRangeForWeeks = "\r\nPlease enter a whole number between 1 and 9999 for Number of weeks.";
        /// <summary>
        /// Message Displayed When Month is NaN
        /// </summary>
        internal const string InvalidFieldForMonths = "\r\nPlease enter a valid whole number for Number of months.";
        /// <summary>
        /// Message Displayed When Month is outside of range
        /// </summary>
        internal const string ValidFieldRangeForMonths = "\r\nPlease enter a whole number between 1 and 9999 for Number of months.";
        /// <summary>
        /// Message Displayed When Years is NaN
        /// </summary>
        internal const string InvalidFieldForYears = "\r\nPlease enter a valid whole number for Number of years.";
        /// <summary>
        /// Message Displayed When Years is outside of range
        /// </summary>
        internal const string ValidFieldRangeForYears = "\r\nPlease enter a whole number between 1 and 9999 for Number of years.";

        /// <summary>
        /// Message Displayed When Date and time 1 is Left Blank
        /// </summary>
        internal const string EmptyFieldForDateAndTime1 = "\r\nPlease enter a date and time for Date and time 1.";
        /// <summary>
        /// Message Displayed When Date and time 2 is Left Blank
        /// </summary>
        internal const string EmptyFieldForDateAndTime2 = "\r\nPlease enter a date and time for Date and time 2.";
        /// <summary>
        /// Message Displayed When Date and time 1 is NaD
        /// </summary>
        internal const string InvalidFieldForDateAndTime1 = "\r\nPlease enter a valid date for Date and time 1.";
        /// <summary>
        /// Message Displayed When Date and time 2 is NaD
        /// </summary>
        internal const string InvalidFieldForDateAndTime2 = "\r\nPlease enter a valid date for Date and time 2.";
        /// <summary>
        /// Message Displayed When Date and time 1 is greater than Date and time 2
        /// </summary>
        internal const string InvalidFieldForDateAndTime1GreaterThanDateAndTime2 = "\r\nPlease enter a valid date greater than Date and time 1 for Date and time 2.";

        /// <summary>
        /// Message Displayed When Time 1 is Left Blank
        /// </summary>
        internal const string EmptyFieldForTime1 = "\r\nPlease enter a valid time for Time 1 (hh:mm).";
        /// <summary>
        /// Message Displayed When Time 2 is Left Blank
        /// </summary>
        internal const string EmptyFieldForTime2 = "\r\nPlease enter a valid time for Time 2 (hh:mm).";
        /// <summary>
        /// Message Displayed When time 1 is greater than time 2
        /// </summary>
        internal const string InvalidFieldForTime1GreaterThanTime2 = "\r\nPlease enter a valid time greater than Time 1 for Time 2.";

        /// <summary>
        /// Message displayed when List items is blank
        /// </summary>
        internal const string EmptyListSelecton = "\r\nPlease select an item from the list.";

        /// <summary>
        /// Message displayed when UDF cannot be deleted
        /// </summary>
        internal const string UDFCannotBeDeleted = "\r\nUserdefined field cannot be deleted as it is currently in use on a GreenLight View.";
        #endregion
    }

    /// <summary>
    /// The employee modal messages.
    /// </summary>
    internal class EmployeeModalMessages
    {
        #region Modal Messages for Employee Fields
        /// <summary>
        /// Message Displayed When mandatory employee title field is left blank
        /// </summary>
        internal const string EmptyFieldForEmployeeTitle = "Please enter the employees Title in the box provided";

        /// <summary>
        /// Message Displayed When mandatory employee title field is left blank
        /// </summary>
        internal const string EmptyFieldForEmployeeUsername = "Please enter a Username for this employee";

        /// <summary>
        /// Message Displayed When mandatory employee title field is left blank
        /// </summary>
        internal const string EmptyFieldForEmployeeFirstName = "Please enter the employees First Name";

        /// <summary>
        /// Message Displayed When mandatory employee title field is left blank
        /// </summary>
        internal const string EmptyFieldForEmployeeSurname = "Please enter the employee's surname";

        /// <summary>
        /// Message Displayed When employee email address field is invalid
        /// </summary>
        internal const string InvalideFieldForEmployeeEmailAddress = "Please enter a valid email address in the box provided";

        /// <summary>
        /// Message Displayed When employee home email address field is invalid
        /// </summary>
        internal const string InvalideFieldForEmployeeHomeEmailAddress = "Please enter a valid home email address in the box provided";

        /// <summary>
        /// Message Displayed When employee starting mileage field is invalid
        /// </summary>
        internal const string InvalideFieldForEmployeeStartingMileage = "Please enter a valid value for Starting Mileage";

        /// <summary>
        /// Message Displayed When employee starting mileage field is invalid
        /// </summary>
        internal const string InvalideFieldForEmployeeLeaveDateEarlierThanHireDate = "The leave date you have entered is earlier than the hire date";

        /// <summary>
        /// Message Displayed When unique employee username field is duplicated
        /// </summary>
        internal const string DuplicateFieldForEmployeeUsername = "The username you have entered already exists. Please enter another username.";

        /// <summary>
        /// Message Displayed When unarchicved employee is deleted
        /// </summary>
        internal const string DeleteEmployeeWhenUnarchived = "You must archive an employee before it can be deleted.";

        /// <summary>
        /// Message Displayed When budget holder employee is deleted
        /// </summary>
        internal const string DeleteEmployeeWhenUsedAsBudgetHolder = "This employee is currently set as a budget holder.";
        #endregion

        #region Modal Messages for Employee Car Fields
        /// <summary>
        /// Message Displayed When mandatory car make field is left blank
        /// </summary>
        internal const string EmptyFieldForCarMake = "\r\nPlease enter the make of this car";

        /// <summary>
        /// Message Displayed When mandatory car model field is left blank
        /// </summary>
        internal const string EmptyFieldForCarModel = "\r\nPlease enter the model of this car";

        /// <summary>
        /// Message Displayed When mandatory car reg number field is left blank
        /// </summary>
        internal const string EmptyFieldForCarRegistrationNumber = "\r\nPlease enter the registration number of this car";

        /// <summary>
        /// Message Displayed When mandatory car engine type field is left blank
        /// </summary>
        internal const string EmptyFieldForEngineType = "\r\nPlease select the engine type.";

        /// <summary>
        /// Message Displayed When mandatory car engine size field is left blank
        /// </summary>
        internal const string EmptyFieldForEngineSize = "\r\nPlease enter the engine size (in cc's)";
        #endregion

        #region Modal Messages for Employee Corporate Card Fields
        /// <summary>
        /// Message Displayed When mandatory card number field is left blank
        /// </summary>
        internal const string EmptyFieldForCardNumber = "\r\nPlease enter a card number in the box provided";
        #endregion

        #region Modal Messages for Employee Mobile Device Fields
        /// <summary>
        /// Message Displayed When mandatory mobile device name field is left blank
        /// </summary>
        internal const string EmptyFieldForDeviceName = "\r\nPlease enter a Name for the mobile device.";
        /// <summary>
        /// Message Displayed When mandatory mobile device type field is left blank
        /// </summary>
        internal const string EmptyFieldForDeviceType = "\r\nPlease select a Type for the mobile device.";
        #endregion

        #region Modal Messages for Employee Home Address Fields
        /// <summary>
        /// Message Displayed When mandatory home address field is left blank
        /// </summary>
        internal const string EmptyFieldForHomeAddress = "\r\nPlease enter an address to save";

        /// <summary>
        /// Message Displayed When mandatory home address start date field is left blank
        /// </summary>
        internal const string EmptyFieldForHomeAddressStartDate = "\r\nPlease enter a start date in the box provided";
        #endregion

        #region Modal Messages for Employee Work Address Fields
        /// <summary>
        /// Message Displayed When mandatory work address field is left blank
        /// </summary>
        internal const string EmptyFieldForWorkAddress = "\r\nPlease enter a work location address";

        /// <summary>
        /// Message Displayed When mandatory work address start date field is left blank
        /// </summary>
        internal const string EmptyFieldForWorkAddressStartDate = "\r\nPlease enter a valid start date";
        #endregion

        #region Modal Messages for Employee Assignment Fields
        /// <summary>
        /// Message Displayed When mandatory assignmentToAdd number field is left blank
        /// </summary>
        internal const string EmptyFieldForAssignmentNumber = "Please enter an assignment number in the box provided";

        /// <summary>
        /// Message Displayed When mandatory assignmentToAdd start date field is left blank
        /// </summary>
        internal const string EmptyFieldForEarliestAssignmentStartDate = "Please enter a value for the Earliest Assignment Start Date";

        /// <summary>
        /// Message Displayed When assignmentToAdd end date is earlier than assignmentToAdd start date
        /// </summary>
        internal const string InvalidFieldForEndDateEarlierThanStartDate = "ESR Start Date must preceed the ESR End Date";

        /// <summary>
        /// Message Displayed When assignmentToAdd number is duplicated
        /// </summary>
        internal const string DuplicateFieldForAssignmentNumber = "The Assignment Number you have entered already exists.";

        #endregion
    }

    /// <summary>
    /// The nhs trust modal messages.
    /// </summary>
    internal class NHSTrustModalMessages
    {
        #region Modal Messages for NHSTrust Fields
        /// <summary>
        /// Message Displayed When mandatory trust name field is left blank
        /// </summary>
        internal const string EmptyFieldForTrustName = "\r\nPlease enter the trust name";

        /// <summary>
        /// Message Displayed When unique trust name field is duplicated
        /// </summary>
        internal const string DuplicateFieldForTrustName = "\r\nThe Trust Name you have entered already exists.";
        /// <summary>
        /// Message Displayed When unique trust vpd field is duplicated
        /// </summary>
        internal const string DuplicateFieldForTrustVpd = "\r\nThe Trust VPD you have entered already exists.";

        /// <summary>
        /// Message Displayed When esr interface version is changed
        /// </summary>
        internal const string ChangedESRInterfaceVersion = "\r\nThe ESR Trust Version used by the trust has changed. Import Template Mappings require editing and updating for imports to succeed.";
        /// <summary>
        /// Message Displayed When turst to be deleted is associated to a financial export
        /// </summary>
        internal const string AssociatedFinanicialExport = "\r\nCannot delete the ESR Trust as it is associated to a financial export.";
        #endregion
    }

    /// <summary>
    /// The budget holder modal messages.
    /// </summary>
    internal class BudgetHolderModalMessages
    {
        #region Modal Messages for Budge Holder Fields
        /// <summary>
        /// Message Displayed When mandatory label field is left blank
        /// </summary>
        internal const string EmptyFieldLabel = "\r\nPlease enter a label for this budget holder";

        /// <summary>
        /// Message Displayed When unique label field is duplicated
        /// </summary>
        internal const string DuplicateFieldForLabel = "\r\nThe budget Holder name you have entered already exists";

        /// <summary>
        /// Message Displayed When mandatory employee responsible field is left blank
        /// </summary>
        internal const string EmptyFieldForEmployeeResponsible = "\r\nPlease enter a valid employee responsible";

        /// <summary>
        /// Message Displayed When employee responsible field is invalid
        /// </summary>
        internal const string InvalidFieldForEmployeeResponsible = "\r\nInvalid entry provided for Employee responsible - Type three or more characters and select a valid entry from the available options.";
        #endregion
    }

    /// <summary>
    /// The teams modal messages.
    /// </summary>
    internal class TeamsModalMessages
    {
        /// <summary>
        /// Message Displayed When mandatory team name field is left blank
        /// </summary>
        internal const string EmptyFieldForTeamName = "\r\nPlease enter a team name";

        /// <summary>
        /// The empty grid for team members.
        /// </summary>
        internal const string EmptyGridForTeamMembers =
            "\r\nThe team has no members. Empty teams are prohibited. Please add at least one member.";

        /// <summary>
        /// The delete only member.
        /// </summary>
        internal const string DeleteOnlyMember = "\r\nThere must be at least one team member present. Deletion of the last member is prohibited.";

        /// <summary>
        /// The duplicate field for team name.
        /// </summary>
        internal const string DuplicateFieldForTeamName = "\r\nA team with this name already exists";
    }

    /// <summary>
    ///  The approval matrices modal messages
    /// </summary>
    internal class ApprovalMatricesModalMessages
    {
        #region Modal Messages for approval matrices
        /// <summary>
        /// Message displayed when mandatory approval matrix name field is left blank
        /// </summary>
        internal const string EmptyFieldForApprovalMatrixName = "\r\nPlease enter an Approval matrix name.";

        /// <summary>
        /// Message displayed when mandatory default approver field is left blank
        /// </summary>
        internal const string EmptyFieldForDefaultApprover = "\r\nPlease enter a Default approver.";
        
        /// <summary>
        /// Message displayed when mandatory approval limit field is left blank
        /// </summary>
        internal const string EmptyFiedlForApprovalLimit = "\r\nPlease enter an Approval limit.";

        /// <summary>
        /// Message displayed when mandatory approver field is left blank
        /// </summary>
        internal const string EmptyFieldForApprover = "\r\nPlease enter an Approver.";

        /// <summary>
        /// Message displayed when unique approval matrix name field is duplicated
        /// </summary>
        internal static string DuplicateFieldForApprovalMatrixName = "\r\nThe Approval matrix name already exists.";

        /// <summary>
        /// Message displated when unique approval amount is duplicated
        /// </summary>
        internal static string DuplicateFieldForApprovalLimit = "\r\nThe Approval limit already exists.";

        /// <summary>
        /// Message displayed when mandatory default approver field is invalid
        /// </summary>
        internal const string InvalidFieldForDefaultApprover = "\r\nPlease enter a valid Default approver.";

        /// <summary>
        /// Message displayed when mandatory approver field is invalid
        /// </summary>
        internal const string InvalidFieldForApprover = "\r\nPlease enter a valid Approver.";
        #endregion
    }

    /// <summary>
    /// The approval matrices modal messages
    /// </summary>
    internal class UserDefinedFieldsModalMessages
    {
        #region Modal Messages for user defined fields
        /// <summary>
        /// Message displayed when mandatory display name field is left blank
        /// </summary>
        internal const string EmptyFieldForDisplayName = "\r\nPlease enter a name for this attribute in the box provided";

        /// <summary>
        /// Message displayed when mandatory default value field is left blank
        /// </summary>
        internal const string EmptyFieldForDefaultValue = "\r\nPlease select whether Yes or No should be selected by default";

        /// <summary>
        /// Message displayed when mandatory hyperlink text field is left blank
        /// </summary>
        internal const string EmptyFieldForHyperLinkText = "\r\nPlease enter a value for the Hyperlink Text";

        /// <summary>
        /// Message displayed when mandatory hyperlink path field is left blank
        /// </summary>
        internal const string EmptyFieldForHyperLinkPath = "\r\nPlease enter a value for the Hyperlink Path/URL";

        /// <summary>
        /// Message displayed when mandatory related table field is left blank
        /// </summary>
        internal const string EmptyFieldForRelatedTable = "\r\nYou must choose a related table to use in the relationship textbox";

        /// <summary>
        /// Message displayed when mandatory display field field is left blank
        /// </summary>
        internal const string EmptyFieldForDisplayField = "\r\nPlease select a valid display field for the relationship";

        /// <summary>
        /// Message displayed when mandatory match field field is left blank
        /// </summary>
        internal const string EmptyFieldForMatchField = "\r\nPlease add a Match Field.";
        #endregion
    }

    /// <summary>
    /// The cost codes modal messages.
    /// </summary>
    internal class CostCodesModalMessages
    {
        /// <summary>
        /// Message Displayed When mandatory cost code name field is left blank
        /// </summary>
        internal const string EmptyFieldForCostCodeName = "\r\nPlease enter a cost code";
    }

    /// <summary>
    /// The mobile devices modal messages.
    /// </summary>
    internal class MobileDevicesEmployeesPageModalMessages
    {
        /// <summary>
        /// The duplicate field for mobile device name.
        /// </summary>
        internal const string DuplicateFieldForMobileDeviceName = "This mobile device has already been added to the system.";

        /// <summary>
        /// The empty field for device name.
        /// </summary>
        internal const string EmptyFieldForMobileDeviceName = "Please enter a Name for the mobile device.";

        /// <summary>
        /// The empty field for mobile device type.
        /// </summary>
        internal const string EmptyFieldForMobileDeviceType = "Please select a Type for the mobile device.";
    }

    /// <summary>
    /// The colours modal messages
    /// </summary>
    internal class ColoursModalMessages
    {
        /// <summary>
        /// The invalid data for colours fields
        /// </summary>
        internal const string InvalidColours = "\r\nPlease enter a valid value for Menu Bar Background Colour." +
                                               "\r\nPlease enter a valid value for Menu Bar Text Colour." +
                                               "\r\nPlease enter a valid value for Title Bar Background Colour." +
                                               "\r\nPlease enter a valid value for Title Bar Text Colour." +
                                               "\r\nPlease enter a valid value for Field Label Background Colour." +
                                               "\r\nPlease enter a valid value for Field Label Text Colour." +
                                               "\r\nPlease enter a valid value for Table Row Background Colour." +
                                               "\r\nPlease enter a valid value for Table Row Foreground Colour." +
                                               "\r\nPlease enter a valid value for Table Alternate Row Background Colour." +
                                               "\r\nPlease enter a valid value for Table Alternate Row Foreground Colour." +
                                               "\r\nPlease enter a valid value for Hover Colour Background Colour." +
                                               "\r\nPlease enter a valid value for Hover Colour Page Option." +
                                               "\r\nPlease enter a valid value for Tooltip Colour Background Colour." +
                                               "\r\nPlease enter a valid value for Tooltip Colour Text Colour." +
                                               "\r\nPlease enter a valid value for GreenLight Colours Label Text Colour." +
                                               "\r\nPlease enter a valid value for GreenLight Colours Section Text Colour." +
                                               "\r\nPlease enter a valid value for GreenLight Colours Section Background Colour." +
                                               "\r\nPlease enter a valid value for GreenLight Colours Section Underline Colour.";
    }
}
