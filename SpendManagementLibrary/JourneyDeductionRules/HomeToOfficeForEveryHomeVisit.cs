namespace SpendManagementLibrary.JourneyDeductionRules
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The journey deduction rule for "full home to office if start and/or finish address is "home"
    /// </summary>
    public class HomeToOfficeForEveryHomeVisit : BaseRule
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="HomeToOfficeForEveryHomeVisit"/> class.
        /// </summary>
        /// <param name="steps">
        /// The journey steps for the deduction rule
        /// </param>
        /// <param name="homeToOfficeDistance">
        /// The employee's home to office distance
        /// </param>
        /// <param name="officeToHomeDistance">
        /// The employee's office to home distance
        /// </param>
        /// <param name="employeeHomeLocationId">
        /// The employee's "home" location (company) Id
        /// </param>
        /// <param name="employeeOfficeLocationId">
        /// The employee's "office" location (company) Id
        /// </param>
        /// <param name="subcat">
        /// The "Home To Office Always Zero" rule for the subcat.
        /// </param>
        public HomeToOfficeForEveryHomeVisit(SortedList<int, cJourneyStep> steps, decimal homeToOfficeDistance, decimal officeToHomeDistance, int employeeHomeLocationId, int employeeOfficeLocationId, cSubcat subcat)
            : base(steps, homeToOfficeDistance, officeToHomeDistance, employeeHomeLocationId, employeeOfficeLocationId, subcat)
        {
        }

        /// <summary>
        /// Returns the message text which explains how this deduction rule works
        /// </summary>
        /// <param name="homeToOfficeDistance">
        /// The employee's home to office distance
        /// </param>
        /// <param name="officeToHomeDistance">
        /// The employee's office to home distance
        /// </param>
        /// <param name="uom">
        /// The distance unit of measurement
        /// </param>
        /// <param name="pluralUomHtO">
        /// The plural string for home to office (usually "s" if the distance is anything other than 1)
        /// </param>
        /// <param name="pluralUomOtH">
        /// The plural string for office to home (usually "s" if the distance is anything other than 1)
        /// </param>
        /// <returns>The formatted message text</returns>
        public static string GetMessage(decimal homeToOfficeDistance, decimal officeToHomeDistance, string uom, string pluralUomHtO, string pluralUomOtH)
        {
            return string.Format("<br /><br />A deduction of {0} {2}{3} for your home to office distance will be made when start address is home and a deduction of {1} {2}{4} office to home distance made when end address is home.<br /><br />The deduction will be made every time your home address is visited in your journey.", homeToOfficeDistance, officeToHomeDistance, uom, pluralUomHtO, pluralUomOtH);
        }

        /// <summary>
        /// Carry out the deduction for this rule
        /// </summary>
        /// <returns>The journey steps which have been recalculated</returns>
        public override SortedList<int, cJourneyStep> Deduct()
        {
            // go forwards through the journey steps
            foreach (cJourneyStep step in this.Steps.Values.Where(step => step.startlocation != null && step.endlocation != null))
            {
                this.EnforceHomeToOfficeAlwaysZero(step);
                this.EnforceHomeToOfficeMileageCap(step);
                // deduct the home to office distance if the start or end location is "home" on any step
                if (step.startlocation.Identifier == this.EmployeeHomeLocationId && step.endlocation.Identifier != this.EmployeeHomeLocationId)
                {
                    this.DeductFromStep(step, this.HomeToOfficeDistance);
                }
                else if (step.startlocation.Identifier != this.EmployeeHomeLocationId && step.endlocation.Identifier == this.EmployeeHomeLocationId)
                {
                    this.DeductFromStep(step, this.OfficeToHomeDistance);
                }
            }

            return this.Steps;
        }
    }
}
