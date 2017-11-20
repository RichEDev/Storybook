using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SpendManagementApi.Interfaces;
using SpendManagementApi.Utilities;
using SpendManagementLibrary;

namespace SpendManagementApi.Models.Types
{
    /// <summary>
    /// An Allowance represents a sum of money that can be claimed against.
    /// </summary>
    public class Allowance : BaseExternalType, IApiFrontForDbObject<cAllowance, Allowance>
    {
        /// <summary>
        /// The unique Id for this Allowance object.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The name / label for this Allowance object.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string Label { get; set; }

        /// <summary>
        /// A description of this Allowance object.
        /// </summary>
        [MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Description { get; set; }

        /// <summary>
        /// The Id of the <see cref="Currency">Currency</see> under which this allowance is paid.
        /// </summary>
        [Required]
        public int CurrencyId { get; set; }

        /// <summary>
        /// How many hours the employee must be away before they can claim the higher <see cref="NightRate">NightRate</see>.
        /// </summary>
        public int NightHours { get; set; }

        /// <summary>
        /// The rate that the claim can be paid at, above the hours specified by <see cref="NightHours">NightHours</see>.
        /// </summary>
        public decimal NightRate { get; set; }

        /// <summary>
        /// Defines allowance rates further to the initial Night Rate allowance.
        /// </summary>
        public List<AllowanceRate> AllowanceRates { get; set; }

        /// <summary>
        /// Creates a new Allowance.
        /// </summary>
        public Allowance()
        {
            AllowanceRates = new List<AllowanceRate>();
        }

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <param name="dbType">The DAL type.</param>
        /// <param name="actionContext">The IActionContext.</param>
        /// <returns>This, the API type.</returns>
        public Allowance From(cAllowance dbType, IActionContext actionContext)
        {
            Id = dbType.allowanceid;
            Label = dbType.allowance;
            Description = dbType.description;
            NightHours = dbType.nighthours;
            NightRate = dbType.nightrate;
            CurrencyId = dbType.currencyid;
            AllowanceRates = dbType.breakdown.Select(b => new AllowanceRate().From(b, actionContext)).ToList();

            AccountId = dbType.accountid;
            CreatedById = dbType.createdby;
            CreatedOn = dbType.createdon;
            ModifiedById = dbType.modifiedby ?? -1;
            ModifiedOn = dbType.modifiedon;
            return this;
        }


        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type.</returns>
        public cAllowance To(IActionContext actionContext)
        {
            var breakdowns = new List<cAllowanceBreakdown>(AllowanceRates.Select(x => x.To(actionContext)));
            return new cAllowance(AccountId ?? -1, Id, Label, Description, CurrencyId, NightHours, NightRate, CreatedOn, CreatedById, ModifiedOn, ModifiedById, breakdowns);
        }
    }

    /// <summary>
    /// An Allowance Rate is attached to an allowance to say: "After THIS many hours, pay the claimant THIS rate.".
    /// </summary>
    public class AllowanceRate : IApiFrontForDbObject<cAllowanceBreakdown, AllowanceRate>
    {
        /// <summary>
        /// The Key / Id of this Allowance Rate.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Primary Key / Id of the <see cref="Allowance">Allowance</see> that this AllowanceRate is tied to.
        /// </summary>
        [Required]
        public int AllowanceId { get; set; }

        /// <summary>
        /// The threshold (hours) at which the <see cref="HourlyRate">HourlyRate</see> will be charged.
        /// </summary>
        public int HourThreshold { get; set; }

        /// <summary>
        /// The rate of charge after the <see cref="HourThreshold">HourThreshold</see> has been reached.
        /// </summary>
        public decimal HourlyRate { get; set; }

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <param name="dbType">The DAL type.</param>
        /// <returns>This, the API type.</returns>
        public AllowanceRate From(cAllowanceBreakdown dbType, IActionContext actionContext)
        {
            Id = dbType.breakdownid;
            AllowanceId = dbType.allowanceid;
            HourThreshold = dbType.hours;
            HourlyRate = dbType.rate;
            return this;
        }

        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type.</returns>
        public cAllowanceBreakdown To(IActionContext actionContext)
        {
            return new cAllowanceBreakdown(Id, AllowanceId, HourThreshold, HourlyRate);   
        }
    }
}