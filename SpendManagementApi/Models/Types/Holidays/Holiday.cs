namespace SpendManagementApi.Models.Types.Holidays
{
    using System;

    using Interfaces;

    /// <summary>
    /// A class to model holiday.
    /// </summary>
    public class Holiday : ArchivableBaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Holidays.Holiday, Holiday>
    {
        /// <summary>
        /// The holiday Id.
        /// </summary>
        public int HolidayId { get; set; }

        /// <summary>
        /// The employee Id.
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// The holiday start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The holiday end date.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from. <see cref="SpendManagementLibrary.Holidays.Holiday"/></param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type <see cref="HolidayId"/></returns>
        public Holiday From(SpendManagementLibrary.Holidays.Holiday dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null; 
            }

            this.HolidayId = dbType.HolidayId;
            this.EmployeeId = dbType.EmployeeId;
            this.StartDate = dbType.StartDate;
            this.EndDate = dbType.EndDate;

            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type <see cref="Holiday"/></returns>
        public SpendManagementLibrary.Holidays.Holiday To(IActionContext actionContext)
        {
           return new SpendManagementLibrary.Holidays.Holiday(this.HolidayId, this.EmployeeId, this.StartDate, this.EndDate);               
        }
    }
}