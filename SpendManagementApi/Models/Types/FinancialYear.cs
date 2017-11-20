using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpendManagementApi.Models.Types
{
    /// <summary>
    /// Financial Year Class.
    /// Year Start and Year End are a 1900 based date to determine the financial year.
    /// </summary>
    public class FinancialYear
    {
        /// <summary>
        /// Financial Year Id.
        /// </summary>
        public int FinancialYearID { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether the financial year is active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Whether the financial year is primary.
        /// </summary>
        public bool Primary { get; set; }

        /// <summary>
        /// Financial Year Start 
        /// </summary>
        public string YearStart { get; set; }

        /// <summary>
        /// Financial Year End
        /// </summary>
        public string YearEnd { get; set; }

    }

    internal static class FinancialYearConversion
    {
        internal static TRes Cast<TRes>(this SpendManagementLibrary.FinancialYears.FinancialYear year)
            where TRes : FinancialYear, new()
        {
            if (year == null)
            {
                return null;
            }
            return new TRes
                       {
                           Active = year.Active,
                           Description = year.Description,
                           FinancialYearID = year.FinancialYearID,
                           Primary = year.Primary,
                           YearEnd = year.YearEndMonthDay,
                           YearStart = year.YearStartMonthDay
                       };

        }
    }
}