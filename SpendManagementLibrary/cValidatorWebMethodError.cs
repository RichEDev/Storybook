using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    /// <summary>
    /// Class used for passing web method errors back from a validator that errored during execution, these values can then be double-checked server side
    /// When using web methods in validators it is important to still allow the user to submit a page when a validator is failing its client-side calls
    /// These errors can be trapped and processed server side
    /// </summary>
    public class cValidatorWebMethodError
    {
        private string sControlID;
        private string sControlToValidateID;
        private string sValidatorErrorMessage;
        private string sServerError;

        /// <summary>
        /// JSON constructor
        /// </summary>
        public cValidatorWebMethodError() { }

        /// <summary>
        /// Object to hold information relating to a web-method call that has failed in javascript
        /// </summary>
        /// <param name="controlID"></param>
        /// <param name="controlToValidateID"></param>
        /// <param name="validatorErrorMessage"></param>
        /// <param name="serverError"></param>
        public cValidatorWebMethodError(string controlID, string controlToValidateID, string validatorErrorMessage, string serverError)
        {
            sControlID = controlID;
            sControlToValidateID = controlToValidateID;
            sValidatorErrorMessage = validatorErrorMessage;
            sServerError = serverError;
        }

        /// <summary>
        /// The ClientID for the validator control
        /// </summary>
        public string ID
        {
            get { return sControlID; }
            set { sControlID = value; }
        }
        /// <summary>
        /// The ClientID for the control being validated
        /// </summary>
        public string ControlToValidate
        {
            get { return sControlToValidateID; }
            set { sControlToValidateID = value; }
        }
        /// <summary>
        /// The ErrorMessage string for the validator
        /// </summary>
        public string ValidatorErrorMessage
        {
            get { return sValidatorErrorMessage; }
            set { sValidatorErrorMessage = value; }
        }
        /// <summary>
        /// The error message as returned from the server in response to the web-method call
        /// </summary>
        public string ServerError
        {
            get { return sServerError; }
            set { sServerError = value; }
        }
    }
}
