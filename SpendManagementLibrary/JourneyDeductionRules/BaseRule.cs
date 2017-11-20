namespace SpendManagementLibrary.JourneyDeductionRules
{
    using System.Collections.Generic;

    /// <summary>
    /// Base class for home to office deduction rules
    /// </summary>
    public abstract class BaseRule
    {
        #region fields

        /// <summary>
        /// The employee's home to office distance
        /// </summary>
        private readonly decimal _homeToOfficeDistance;

        /// <summary>
        /// The employee's office to home distance
        /// </summary>
        private readonly decimal _officeToHomeDistance;

        /// <summary>
        /// The employee's "home" location (company) Id
        /// </summary>
        private readonly int _employeeHomeLocationId;

        /// <summary>
        /// The employee's "office" location (company) Id
        /// </summary>
        private readonly int _employeeOfficeLocationId;

        /// <summary>
        /// The journey steps to have a deduction rule applied to
        /// </summary>
        private readonly SortedList<int, cJourneyStep> _steps;

        /// <summary>
        /// The "home to office always zero" setting, when "true" every step will have a home to
        /// </summary>
        private readonly bool _homeToOfficeAlwaysZero;

        internal decimal _fixedMileage;

        /// <summary>
        /// The "Enforce home to office mileage cap" setting, when "true" Home to ofiice mileage is set to maximum miles configured
        /// </summary>
        private readonly bool _enforceHomeToOfficeMileageCap;

        /// <summary>
        /// Maximum limit set as Mileage Cap if Enforce home to office mileage cap is enabled
        /// </summary>
        internal decimal MileageCap;

        #endregion fields

        /// <summary>
        /// Initialises a new instance of the <see cref="BaseRule"/> class.
        /// </summary>
        /// <param name="steps">
        /// The journey steps for the deduction rule
        /// </param>
        /// <param name="homeToOfficeDistance">
        /// The employee's home to office steps
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
        /// The "Home To Office Always Zero" rule
        /// </param>
        protected BaseRule(SortedList<int, cJourneyStep> steps, decimal homeToOfficeDistance, decimal officeToHomeDistance, int employeeHomeLocationId, int employeeOfficeLocationId, cSubcat subcat)
        {
            this._steps = steps;
            this._homeToOfficeDistance = homeToOfficeDistance;
            this._officeToHomeDistance = officeToHomeDistance;
            this._employeeHomeLocationId = employeeHomeLocationId;
            this._employeeOfficeLocationId = employeeOfficeLocationId;
            this._homeToOfficeAlwaysZero = subcat == null ? false : subcat.HomeToOfficeAlwaysZero;
            this._fixedMileage = subcat == null ? 0 : (decimal) (subcat.HomeToOfficeFixedMiles.HasValue ? subcat.HomeToOfficeFixedMiles : 0);
            this._enforceHomeToOfficeMileageCap = subcat != null && subcat.EnforceToOfficeMileageCap;
            this.MileageCap = subcat == null ? 0 : (decimal)(subcat.HomeToOfficeMileageCap ?? 0);
        }

        #region properties

        /// <summary>
        /// Gets the employee's "home" location (company) Id
        /// </summary>
        protected int EmployeeHomeLocationId
        {
            get { return this._employeeHomeLocationId; }
        }

        /// <summary>
        /// Gets the employee's "office" location (company) Id
        /// </summary>
        protected int EmployeeOfficeLocationId
        {
            get { return this._employeeOfficeLocationId; }
        }

        /// <summary>
        /// Gets the journey steps by the deduction rule
        /// </summary>
        protected SortedList<int, cJourneyStep> Steps
        {
            get { return this._steps; }
        }

        /// <summary>
        /// Gets the employee's home to office distance
        /// </summary>
        protected decimal HomeToOfficeDistance
        {
            get { return this._homeToOfficeDistance; }
        }

        /// <summary>
        /// Gets the employee's office to home distance
        /// </summary>
        protected decimal OfficeToHomeDistance
        {
            get { return this._officeToHomeDistance; }
        }

        #endregion properties

        /// <summary>
        /// Deducts a distance from the specified journey step
        /// </summary>
        /// <param name="step">The step on which to perform the deduction</param>
        /// <param name="deduction">The amount to deduct</param>
        /// <returns>The amount remaining after the deduction has been performed</returns>
        public decimal DeductFromStep(cJourneyStep step, decimal deduction)
        {
            decimal remainder = 0;

            if (step.nummiles > deduction)
            {
                step.nummiles -= deduction;
            }
            else
            {
                remainder = deduction - step.nummiles;
                step.nummiles = 0;
            }

            return remainder;
        }

        /// <summary>
        /// Sets the number of miles on a journey step to 0 if the "Home To Office Always Zero" rule is enabled and the step is "home to office" or "office to home"
        /// </summary>
        /// <param name="step">The journey step</param>
        public void EnforceHomeToOfficeAlwaysZero(cJourneyStep step)
        {
            if (this._homeToOfficeAlwaysZero && ((step.startlocation.Identifier == this.EmployeeHomeLocationId &&
                    step.endlocation.Identifier == this.EmployeeOfficeLocationId) ||
                (step.startlocation.Identifier == this.EmployeeOfficeLocationId &&
                    step.endlocation.Identifier == this.EmployeeHomeLocationId)))
            {
                step.nummiles = 0;
            }
        }

        /// <summary>
        /// Sets the number of miles on a journey step to maximum miles configured if the "Enforce mileage cap on Home to Office journeys" rule is enabled and the step is "home to office" or "office to home"
        /// </summary>
        /// <param name="step">The journey step</param>
        public void EnforceHomeToOfficeMileageCap(cJourneyStep step)
        {
            if (!this._homeToOfficeAlwaysZero && this._enforceHomeToOfficeMileageCap)
            {
                if (((step.startlocation.Identifier == this.EmployeeHomeLocationId &&
                     step.endlocation.Identifier == this.EmployeeOfficeLocationId) ||
                 (step.startlocation.Identifier == this.EmployeeOfficeLocationId &&
                     step.endlocation.Identifier == this.EmployeeHomeLocationId)))
                {
                    if (this.MileageCap > 0 && step.nummiles > this.MileageCap)
                    {
                        step.nummiles = this.MileageCap;
                    }
                }
            }
        }
        
        /// <summary>
        /// Performs the rule's deductions on the journey steps
        /// </summary>
        /// <returns>The journey steps for this rule</returns>
        public abstract SortedList<int, cJourneyStep> Deduct();
    }
}