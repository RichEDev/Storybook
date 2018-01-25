using System.Collections.Generic;
using System.Linq;

namespace SpendManagementLibrary.JourneyDeductionRules 
{
    public class FixedDeductionIfStartOrFinishHome : BaseRule
    {

        public FixedDeductionIfStartOrFinishHome(SortedList<int, cJourneyStep> steps, decimal homeToOfficeDistance, decimal officeToHomeDistance, int employeeHomeLocationId, int employeeOfficeLocationId, cSubcat subcat) : base(steps, homeToOfficeDistance, officeToHomeDistance, employeeHomeLocationId, employeeOfficeLocationId, subcat)

        {
        }

        public override SortedList<int, cJourneyStep> Deduct()
        {
            decimal deductionRemaining = 0;

            // go forwards through the journey steps
            foreach (cJourneyStep step in this.Steps.Values.Where(step => step.startlocation != null && step.endlocation != null))
            {
                this.EnforceHomeToOfficeAlwaysZero(step);
                this.EnforceHomeToOfficeMileageCap(step);
                
                // check any home to office mileage remaining and deduct from the next step
                if (deductionRemaining > 0)
                {
                    deductionRemaining = this.DeductFromStep(step, deductionRemaining);
                }

                if (step.startlocation.Identifier == this.EmployeeHomeLocationId && step.endlocation.Identifier != this.EmployeeHomeLocationId)
                {
                    // deduct home to office distance if the step starts "home"
                    deductionRemaining += this.DeductFromStep(step, this._fixedMileage);
                }
                else if (step.startlocation.Identifier != this.EmployeeHomeLocationId && step.endlocation.Identifier == this.EmployeeHomeLocationId)
                {
                    // deduct office to home distance if the step ends "home"
                    deductionRemaining += this.DeductFromStep(step, this._fixedMileage);
                }
            }

            // then backwards through the journey steps
            foreach (cJourneyStep step in this.Steps.Values.Where(step => step.startlocation != null && step.endlocation != null).Reverse())
            {
                // deduct any remaining miles until there are 0 (or we run out of steps)
                if (deductionRemaining > 0)
                {
                    deductionRemaining = this.DeductFromStep(step, deductionRemaining);
                }
            }

            return this.Steps;
        }

        public static string GetMessage(decimal homeToOfficeDistance, decimal officeToHomeDistance, string uom, string pluralUomHtO, string pluralUomOtH)
        {
            return homeToOfficeDistance == 0 ? "" : string.Format("<br /><br />A deduction of {0} {2}{3} for your home to office distance will be made when start address is home and a deduction of {1} {2}{4} office to home distance made when end address is home.<br /><br />The deduction will be made every time your home address is visited in your journey.", homeToOfficeDistance, officeToHomeDistance, uom, pluralUomHtO, pluralUomOtH);
        }
    }
}
