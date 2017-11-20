namespace SpendManagementLibrary.Holidays
{
    using System;

    /// <summary>
    /// A class that represents a holiday.
    /// </summary>
    public class Holiday
    {
        /// <summary>
        /// The holiday Id.
        /// </summary>
        public int HolidayId { get; set; }

        /// <summary>
        /// The employee Id
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// The holiday start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The holiday end date
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Holiday"/> class.
        /// </summary>
        public Holiday()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Holiday"/> class.
        /// </summary>
        /// <param name="holidayId">
        /// The holiday id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="startDate">
        /// The holiday start date.
        /// </param>
        /// <param name="endDate">
        /// The holiday end date.
        /// </param>
        public Holiday(int holidayId, int employeeId, DateTime startDate, DateTime endDate)

        {
            this.HolidayId = holidayId;
            this.EmployeeId = employeeId;
            this.StartDate = startDate;
            this.EndDate = endDate;
        }
    }
}
