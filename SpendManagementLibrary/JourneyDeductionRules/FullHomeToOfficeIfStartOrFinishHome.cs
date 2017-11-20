namespace SpendManagementLibrary.JourneyDeductionRules
{
    using System.Collections.Generic;
    using System.Linq;
    
    /// <summary>
    /// The journey deduction rule for "full home to office if start and/or finish address is "home"
    /// </summary>
    public class FullHomeToOfficeIfStartOrFinishHome : BaseRule
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FullHomeToOfficeIfStartOrFinishHome"/> class.
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
        public FullHomeToOfficeIfStartOrFinishHome(SortedList<int, cJourneyStep> steps, decimal homeToOfficeDistance, decimal officeToHomeDistance, int employeeHomeLocationId, int employeeOfficeLocationId, cSubcat subcat)
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
            return string.Format("<br /><br />If the start address is home then a deduction of your full home to office distance ({0} {2}{3}) will be applied, if the end address is home then a deduction of your full office to home distance ({1} {2}{4}) will be applied.<br /><br />The full distance deduction will be made if the journey starts at your home address and another full distance deduction will be made if the journey ends at your home address, deducting off adjacent steps if less than the required deductible distance.", homeToOfficeDistance, officeToHomeDistance, uom, pluralUomHtO, pluralUomOtH);
        }
        
        /// <summary>
        /// Carry out the deduction for this rule
        /// TODO this isn't the neatest way of performing the deduction, the steps only need iterating through once but this will affect where the deductions are applied so it would technically be a functionality change
        /// </summary>
        /// <returns>The journey steps which have been recalculated</returns>
        public override SortedList<int, cJourneyStep> Deduct()
        {
            decimal deductionRemaining = 0;
            int lastStepNumber = this.Steps.Values.Count - 1;

            // go forwards through the journey steps
            foreach (cJourneyStep step in this.Steps.Values.Where(step => step.startlocation != null && step.endlocation != null))
            {
                this.EnforceHomeToOfficeAlwaysZero(step);
                this.EnforceHomeToOfficeMileageCap(step);
                // deduct the home to office distance if the start location is "home" on the first step, remaining miles are retained for subsequent steps
                if (step.stepnumber == 0 && step.startlocation.Identifier == this.EmployeeHomeLocationId)
                {
                    deductionRemaining = this.DeductFromStep(step, this.HomeToOfficeDistance);
                }

                // subsequent steps, deduct any remaining home to office miles
                if (deductionRemaining > 0 && step.stepnumber > 0)
                {
                    deductionRemaining = this.DeductFromStep(step, deductionRemaining);
                }
            }

            deductionRemaining = 0;

            // then backwards through the journey steps
            foreach (cJourneyStep step in this.Steps.Values.Where(step => step.startlocation != null && step.endlocation != null).Reverse())
            {
                // perform the deduction on the last step and keep the remainder
                if (step.stepnumber == lastStepNumber && step.endlocation.Identifier == this.EmployeeHomeLocationId)
                {
                    deductionRemaining = this.DeductFromStep(step, this.OfficeToHomeDistance);
                }

                // subsequent (previous) steps, deduct any remaining home to office miles
                if (deductionRemaining > 0 && step.stepnumber < lastStepNumber)
                {
                    deductionRemaining = this.DeductFromStep(step, deductionRemaining);
                }
            }

            return this.Steps;
        } 
    }
}
