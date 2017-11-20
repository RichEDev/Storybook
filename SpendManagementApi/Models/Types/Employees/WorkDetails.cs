namespace SpendManagementApi.Models.Types.Employees
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SpendManagementLibrary;
    using Spend_Management;
    using SpendManagementApi.Common;
    using Utilities;
    using Interfaces;


    /// <summary>
    /// Represents a collection of information about an <see cref="Employee">Employee's</see> job.
    /// </summary>
    public class WorkDetails : BaseExternalType, IRequiresValidation, IEquatable<WorkDetails>
    {
        /// <summary>
        /// The credit account for the employee.
        /// </summary>
        public string CreditAccount { get; set; }

        /// <summary>
        /// The payroll number for the employee.
        /// </summary>
        public string PayRollNumber { get; set; }

        /// <summary>
        /// The position of the employee.
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// The national insurance number of the employee.
        /// </summary>
        public string NationalInsuranceNumber { get; set; }

        /// <summary>
        /// The hire date of the employee.
        /// </summary>
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// The termination date of the employee.
        /// </summary>
        public DateTime? TerminationDate { get; set; }

        /// <summary>
        /// The employee number.
        /// </summary>
        public string EmployeeNumber { get; set; }

        /// <summary>
        /// The primary country id - List of countries can be obtained by calling 
        /// /countries/Find?CountryId=0 for unassigned global countries
        /// </summary>
        public int? PrimaryCountryId { get; set; }

        /// <summary>
        /// The primary currency id - List of currencies can be obtained by calling
        /// /currencies/Find?CurrencyId=0 for unassigned global currencies
        /// </summary>
        public int? PrimaryCurrencyId { get; set; }

        /// <summary>
        /// The user ID of the employee's line manager.
        /// </summary>
        public int? LineManagerUserId { get; set; }

        /// <summary>
        /// The starting mileage that this employee has driven.
        /// </summary>
        public int StartMileage { get; set; }

        /// <summary>
        /// The starting date for mileage.
        /// </summary>
        public DateTime? StartMileageDate { get; set; }

        /// <summary>
        /// The Esr Person Id, if ESR is enabled.
        /// </summary>
        public long? EsrPersonId { get; internal set; }

        /// <summary>
        /// The effective start date for ESR, if enabled.
        /// </summary>
        public DateTime? EsrEffectiveStartDate { get; internal set; }

        /// <summary>
        /// The effective end date for ESR, if enabled.
        /// </summary>
        public DateTime? EsrEffectiveEndDate { get; internal set; }

        /// <summary>
        /// The list of Cost Centre Breakdowns that apply to this container.
        /// </summary>
        public List<CostCentreBreakdown> CostCentreBreakdowns { get; set; }
        
        /// <summary>
        /// Gets the calculated percentage total.
        /// </summary>
        internal decimal CostCentreBreakdownTotal
        {
            get
            {
                return CostCentreBreakdowns.Sum(cc => cc.Percentage);
            }
        }

        /// <summary>
        /// Validation method
        /// </summary>
        public void Validate(IActionContext actionContext)
        {
            if (LineManagerUserId.HasValue && LineManagerUserId > 0 && actionContext.Employees.GetEmployeeById(this.LineManagerUserId.Value) == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorEmployeeLineManager);
            }

            DateTime dt = DateTime.UtcNow;
            if (this.HireDate != null & this.StartMileageDate != null)
            {
                if (this.StartMileageDate.Value.CompareTo(this.HireDate) < 0)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorEmployeeHireDateAfterMileageDate);
                }
            }

            if (this.TerminationDate != null && this.HireDate != null)
            {
                if (this.TerminationDate.Value.CompareTo(this.HireDate) < 0)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorEmployeeTerminationBeforeHire);
                }
            }

            if (PrimaryCountryId.HasValue && PrimaryCountryId > 0 && actionContext.Countries.getCountryById(this.PrimaryCountryId.Value) == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorEmployeePrimaryCountry);
            }

            if (PrimaryCurrencyId.HasValue && PrimaryCurrencyId > 0 && actionContext.Currencies.getCurrencyById(this.PrimaryCurrencyId.Value) == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorEmployeePrimaryCurrency);
            }

            if (CostCentreBreakdowns != null)
            {
                CostCentreBreakdowns.ForEach(cc => cc.Validate(actionContext));

                if (CostCentreBreakdowns.Any() && CostCentreBreakdownTotal != 100)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorCostCentrePercentageMustAddUp);
                }
            }
        }

        internal static WorkDetails Merge(WorkDetails dataToUpdate, WorkDetails existingData)
        {
            if (dataToUpdate == null)
            {
                dataToUpdate = new WorkDetails
                                   {
                                       CreditAccount = existingData.CreditAccount,
                                       EmployeeNumber = existingData.EmployeeNumber,
                                       EsrEffectiveEndDate = existingData.EsrEffectiveEndDate,
                                       EsrEffectiveStartDate = existingData.EsrEffectiveStartDate,
                                       EsrPersonId = existingData.EsrPersonId,
                                       HireDate = existingData.HireDate,
                                       LineManagerUserId = existingData.LineManagerUserId,
                                       NationalInsuranceNumber = existingData.NationalInsuranceNumber,
                                       PayRollNumber = existingData.PayRollNumber,
                                       Position = existingData.Position,
                                       PrimaryCountryId = existingData.PrimaryCountryId,
                                       PrimaryCurrencyId = existingData.PrimaryCurrencyId,
                                       StartMileage = existingData.StartMileage,
                                       StartMileageDate = existingData.StartMileageDate,
                                       TerminationDate = existingData.TerminationDate,
                                       CostCentreBreakdowns = existingData.CostCentreBreakdowns
                                   };
            }

            return dataToUpdate;
        }

        public bool Equals(WorkDetails other)
        {
            if (other == null)
            {
                return false;
            }
            return this.CostCentreBreakdowns.SequenceEqual(other.CostCentreBreakdowns)
                   && this.CreditAccount.Equals(other.CreditAccount)
                   && this.EmployeeNumber.Equals(other.EmployeeNumber)
                   && this.DateCompare(this.EsrEffectiveEndDate, other.EsrEffectiveEndDate)
                   && this.DateCompare(this.EsrEffectiveStartDate, other.EsrEffectiveStartDate)
                   && this.EsrPersonId.Equals(other.EsrPersonId) && this.DateCompare(this.HireDate, other.HireDate)
                   && this.LineManagerUserId.Equals(other.LineManagerUserId)
                   && this.NationalInsuranceNumber.Equals(other.NationalInsuranceNumber)
                   && this.PayRollNumber.Equals(other.PayRollNumber) && this.Position.Equals(other.Position)
                   && this.PrimaryCountryId.Equals(other.PrimaryCountryId)
                   && this.PrimaryCurrencyId.Equals(other.PrimaryCurrencyId)
                   && this.StartMileage.Equals(other.StartMileage)
                   && this.DateCompare(this.StartMileageDate, other.StartMileageDate)
                   && this.DateCompare(this.TerminationDate, other.TerminationDate);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as WorkDetails);
        }
    }

    internal static class WorkDetailsConversion
    {
        internal static TResult Cast<TResult>(this SpendManagementLibrary.Employees.Employee employee, cEmployees employees, cGlobalCountries globalCountries, cGlobalCurrencies globalCurrencies)
            where TResult : WorkDetails, new()
        {
            return new TResult
                {
                    CreditAccount = employee.Creditor,
                    EmployeeNumber = employee.EmployeeNumber,
                    HireDate = employee.HiredDate,
                    NationalInsuranceNumber = employee.NationalInsuranceNumber,
                    PayRollNumber = employee.PayrollNumber,
                    Position = employee.Position,
                    PrimaryCountryId = employee.PrimaryCountry,
                    PrimaryCurrencyId = employee.PrimaryCurrency,
                    StartMileage = employee.MileageTotal,
                    StartMileageDate = employee.MileageTotalDate,
                    TerminationDate = employee.TerminationDate,
                    CostCentreBreakdowns = Helper.NullIf(employee.GetCostBreakdown().Get() as List<cDepCostItem>).Cast<List<CostCentreBreakdown>>(),
                    EsrEffectiveEndDate = employee.EsrEffectiveStartDate,
                    EsrEffectiveStartDate = employee.EsrEffectiveEndDate,
                    EsrPersonId = employee.EsrPersonID,
                    LineManagerUserId = employee.LineManager
                };
        }
    }

}