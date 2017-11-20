namespace SpendManagementLibrary.DVLA
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents all the Response code for DVLA errors
    /// </summary>
    public class DutyOfCareConstants
    {
        private readonly Dictionary<string, DvlaServiceResponseCodes> _errorMap = new Dictionary<string, DvlaServiceResponseCodes>() {
            {"300",new DvlaServiceResponseCodes("Validation Error (See Details)","Validation Error")},
            {"301",new DvlaServiceResponseCodes("Invalid driver or issue number","Invalid driver or issue number")},
            {"302",new DvlaServiceResponseCodes("Driver with provided ID does not exist","Driver with provided ID does not exist")},
            {"312",new DvlaServiceResponseCodes("Driver with this driving licence number already exists in the system","Driver with this driving licence number already exists in the system")},
            {"303",new DvlaServiceResponseCodes("Driver has no valid consent on record ","Driver has no valid consent on record")},
            {"305",new DvlaServiceResponseCodes("Validation Error (See Details)","Driver not located in system")},
            {"310",new DvlaServiceResponseCodes("No check data available for driver ","No check data available for driver")},
            {"311",new DvlaServiceResponseCodes("Unable to validate driver data ","Unable to validate driver data ")},
            {"313",new DvlaServiceResponseCodes("Email address is not unique to this driver ","Email address is not unique to this driver")},
            {"314",new DvlaServiceResponseCodes("Unable to create driver in system","Unable to create driver in system")},
            {"401",new DvlaServiceResponseCodes("Email address is not valid email","Email address is not valid email")},
            {"500",new DvlaServiceResponseCodes("DVLA Systems failed to respond","DVLA Systems failed to respond")},
            {"501",new DvlaServiceResponseCodes("Insufficient credits available ","Insufficient credits available")},
            {"502",new DvlaServiceResponseCodes("Data Conversion Error (See Details Property) ","Data Conversion Error")},
            {"504",new DvlaServiceResponseCodes("Invalid username or password or Company Code provided ","Could not access licence check portal due to invalid username, password or company code")},
            {"601",new DvlaServiceResponseCodes("Mandate awaiting approval","Mandate awaiting approval")},
            {"602",new DvlaServiceResponseCodes("Unable to locate mandate ","Unable to locate mandate")},
            {"506",new DvlaServiceResponseCodes("Invalid Company Code provided ","Invalid Company Code provided")}
        };

        /// <summary>
        /// Dictionary to store the error code and the response details
        /// </summary>
        public Dictionary<string, DvlaServiceResponseCodes> ErrorMap => this.ErrorMap;

        /// <summary>
        /// Retunrs error reponse details with description of error code for matching error code passed
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns>The <see cref="DvlaServiceResponseCodes"/></returns>
        public DvlaServiceResponseCodes MapError(string errorCode) => this._errorMap[errorCode];
    }
}
