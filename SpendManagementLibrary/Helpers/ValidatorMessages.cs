namespace SpendManagementLibrary.Helpers
{
    /// <summary>
    /// Messages that may be output as the result of a validation failure on a field
    /// </summary>
    public class ValidatorMessages
    {
        #region Mandatory
        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public static string Mandatory(string labelName)
        {
            return "Please specify a value for " + labelName + ".";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public static string MandatoryText(string labelName)
        {
            return labelName + " is a mandatory field. Please enter a value in the box provided.";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string StringLength(string labelName, int maxLength)
        {
            return string.Format("{0} can not exceed {1} characters.", labelName, maxLength);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public static string MandatoryDateFromDateTime(string labelName)
        {
            return "Please specify a date value for date and time field " + labelName;        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public static string MandatoryTimeFromDateTime(string labelName)
        {
            return "Please specify a time value for date and time field " + labelName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public static string MandatoryDropdown(string labelName)
        {
            return labelName + " is a mandatory field. Please enter a value in the box provided.";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public static string MandatoryRelationshipTextbox(string labelName)
        {
            return labelName + " is a mandatory field. Please enter a value in the box provided.";
        }
        #endregion Mandatory

        #region Format
        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public static string FormatCurrency(string labelName)
        {
            return string.Format("The value you have entered for {0} is invalid. Valid characters are the numbers 0-9 and a full stop (.) limited to two decimal places", labelName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public static string FormatInteger(string labelName)
        {
            return string.Format("The value you have entered for {0} is invalid. Valid characters are the numbers 0-9", labelName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="minValue"></param>
        /// <returns></returns>
        public static string FormatIntegerGreaterThan(string labelName, int minValue)
        {
            return string.Format("The value you have entered for {0} must be greater than {1}", labelName, minValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="minValue"></param>
        /// <returns></returns>
        public static string FormatIntegerGreaterThanOrEqualTo(string labelName, int minValue)
        {
            return string.Format("The value you have entered for {0} must be greater than or equal to {1}", labelName, minValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static string FormatIntegerLessThanOrEqualTo(string labelName, string maxValue)
        {
            return string.Format("The value you have entered for {0} must be less than or equal to {1}", labelName, maxValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static string FormatIntegerLessThan(string labelName, int maxValue)
        {
            return string.Format("The value you have entered for {0} must be less than {1}", labelName, maxValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public static string FormatNumber(string labelName)
        {
            return string.Format("The value you have entered for {0} is invalid. Valid characters are the numbers 0-9 and a full stop (.)", labelName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public static string FormatDate(string labelName)
        {
            return string.Format("The value you have entered for date field {0} is invalid.", labelName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public static string FormatTime(string labelName)
        {
            return string.Format("The value you have entered for time field {0} is invalid.", labelName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public static string FormatDateAndTime(string labelName)
        {
            return string.Format("The value you have entered for date and time field {0} is invalid.", labelName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="minimumDate"></param>
        /// <returns></returns>
        public static string FormatDateMinimum(string labelName, string minimumDate)
        {
            return string.Format("The value you have entered for date field {0} must be on or after {1}", labelName, minimumDate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="minimumDate"></param>
        /// <returns></returns>
        public static string FormatDateAndTimeMinimum(string labelName, string minimumDate)
        {
            return string.Format("The value you have entered for date and time field {0} must be on or after {1}", labelName, minimumDate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <returns></returns>
        public static string FormatDateMaximum(string labelName, string maximumDate)
        {
            return string.Format("The value you have entered for date field {0} must be on or before {1}", labelName, maximumDate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="maximumDate"></param>
        /// <returns></returns>
        public static string FormatDateAndTimeMaximum(string labelName, string maximumDate)
        {
            return string.Format("The value you have entered for date and time field {0} must be on or before {1}", labelName, maximumDate);
        }

        /// <summary>
        /// Provides a validation message for contact (email) fields
        /// </summary>
        /// <param name="labelName">The label text for the field</param>
        /// <returns>The validation message</returns>
        public static string FormatContactEmail(string labelName)
        {
            return "The value you have entered for email field " + labelName + " is invalid.";
        }

        #endregion Format
    }
}
