using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendManagementApi.Models.Types.Expedite
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Attributes.Validation;
    using Interfaces;
    using Utilities;

    /// <summary>
    /// Represents a Fund associated to expedite client 
    /// get fund available, top up the fund
    /// </summary>
    public class FundManager : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Expedite.Fund, FundManager>
    {
        /// <summary>
        /// The unqiue primary key of this fund transaction.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The AccountId of the expedite client that 
        /// this transaction associated to
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// The  AvailableFund for the selected axpedite client
        /// </summary>
        public decimal AvailableFund { get; set; }

        /// <summary>
        /// The  Top up made for the selected expedite client's Fund
        /// </summary>
        public decimal FundTopup { get; set; }


        /// <summary>
        /// Transaction Type
        /// </summary>
        public int TransType { get; set; }

        /// <summary>
        /// Fund limit
        /// </summary>
        public decimal FundLimit { get; set; }
             

        /// <summary>
        /// Converts to a API type from a base class of Fund 
        /// </summary>
        /// <param name="dbType">An fund.</param>
        /// <param name="actionContext">The actionContext which contains the DAL classes.</param>
        /// <returns></returns>
        public FundManager From(SpendManagementLibrary.Expedite.Fund dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.Id = dbType.Id;
            this.AccountId = dbType.AccountId;
            this.AvailableFund = dbType.AvailableFund;
            this.FundTopup = dbType.FundTopup;
            this.TransType = dbType.TransType;
            this.FundLimit = dbType.FundLimit;
            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Expedite.Fund To(IActionContext actionContext)
        {
            return new SpendManagementLibrary.Expedite.Fund
            {
                Id = this.Id,
                AccountId = this.AccountId,
                AvailableFund = this.AvailableFund,
                FundTopup = this.FundTopup,
                TransType = this.TransType,
                FundLimit = this.FundLimit
            };
        }
    }
}
