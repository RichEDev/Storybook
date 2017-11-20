
namespace SpendManagementLibrary.Expedite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Fund
    {
       
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

    }
}