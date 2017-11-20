using System;
using System.Collections.Generic;
using System.Linq;

namespace SpendManagementLibrary.JourneyDeductionRules
{
    public class JuniorDoctorRotation : BaseRule
    {
        private readonly int baseAddressId;

        private int baseLocationId;
        private readonly IJourneyDistances journeyDistances;

        /// <summary>
        /// Initialises a new instance of the <see cref="JuniorDoctorRotation"/> class.
        /// </summary>
        /// <param name="steps">
        ///     The journey steps for the deduction rule
        /// </param>
        /// <param name="homeToOfficeDistance">
        ///     The employee's home to office distance
        /// </param>
        /// <param name="officeToHomeDistance">
        ///     The employee's office to home distance
        /// </param>
        /// <param name="employeeHomeLocationId">
        ///     The employee's "home" location (company) Id
        /// </param>
        /// <param name="employeeOfficeLocationId">
        ///     The employee's "office" location (company) Id
        /// </param>
        /// <param name="subcat">
        ///     The "Home To Office Always Zero" rule for the subcat.
        /// </param>
        /// <param name="baseAddress"></param>
        /// <param name="baseLocationId">The ID of the base (primary) location for this employee</param>
        /// <param name="journeyDistances">An instance of the <see cref="IJourneyDistances"/> class/param>
        public JuniorDoctorRotation(SortedList<int, cJourneyStep> steps, decimal homeToOfficeDistance, decimal officeToHomeDistance, int employeeHomeLocationId, int employeeOfficeLocationId, cSubcat subcat, cEmployeeWorkLocation baseAddress)
            : base(steps, homeToOfficeDistance, officeToHomeDistance, employeeHomeLocationId, employeeOfficeLocationId, subcat)
        {
            if (baseAddress != null)
            {
                this.baseAddressId = baseAddress.LocationID;
            }
        }

        /// <summary>
        /// Carry out the deduction for this rule
        /// </summary>
        /// <returns>The journey steps which have been recalculated</returns>
        public override SortedList<int, cJourneyStep> Deduct()
        {
            // Iterate through the steps, if any are not "office" then must be an official journey, and therefore no deduction to mileage.
            foreach (cJourneyStep step in this.Steps.Values.Where(step => step.startlocation != null && step.endlocation != null))
            {
                step.OfficialJourney = true;

                if (step.startlocation.Identifier == base.EmployeeHomeLocationId && (step.endlocation.Identifier == this.baseAddressId || step.endlocation.Identifier == base.EmployeeOfficeLocationId))
                {
                    step.OfficialJourney = false;
                }

                if (step.endlocation.Identifier == base.EmployeeHomeLocationId && (step.startlocation.Identifier == this.baseAddressId || step.startlocation.Identifier == base.EmployeeOfficeLocationId))
                {
                    step.OfficialJourney = false;
                }
            }

            if (this.Steps.Any(x => x.Value.OfficialJourney))
            {
                return this.Steps;
            }

            // If not official, subtract home to base from home to office.  and base to home from office to home.
            foreach (cJourneyStep step in this.Steps.Values.Where(step => step.startlocation != null && step.endlocation != null))
            {
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
            return string.Format("<br /><br />If your journey is from 'Home' to 'Office' or 'Office' to 'Home', then a deduction will be made that is equal to the distance between 'Home' and your current work location. This will be paid at the Public Transport Rate.<br/><br/>If your journey contains a destination other than 'Home' or 'Office', this journey is classed as an 'Official Journey'. <br/><br/> For an 'Official Journey', all mileage will be paid at your Vehicle Journey Rate, apart from the distance between your home and any other destination. This will be paid at the full Vehicle Journey Rate minus the 'Public Transport Rate' for the distance between your current work(office) and the destination.", homeToOfficeDistance, officeToHomeDistance, uom, pluralUomHtO, pluralUomOtH);
        }

    }
}
