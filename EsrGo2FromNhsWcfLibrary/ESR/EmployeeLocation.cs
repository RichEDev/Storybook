using EsrGo2FromNhsWcfLibrary.Base;

namespace EsrGo2FromNhsWcfLibrary.ESR
{
    /// <summary>
    /// An instance of Employee Address.  Only used to compare before and after esr go2 updates
    /// </summary>
    public class EmployeeLocation : DataClassBase
    {
        /// <summary>
        /// A public instance of the employee ID
        /// </summary>
        public int EmployeeId;

        /// <summary>
        /// A public instance of a postcode.
        /// </summary>
        public string PostCode;
    }
}
