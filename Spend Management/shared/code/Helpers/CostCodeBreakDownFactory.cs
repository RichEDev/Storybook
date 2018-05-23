namespace Spend_Management.shared.code.Helpers
{
    using System;

    using SpendManagementLibrary;

    /// <summary>
    /// The cost code break down factory.
    /// </summary>
    public class CostCodeBreakDownFactory
    {
        /// <summary>
        /// Gets a instance of a cost code breakdown element class.
        /// </summary>
        /// <param name="filterType">
        /// The filter type.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> of the initialized class.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public object Initialize(FilterType filterType, int accountId)
        {
            switch (filterType)
            {             
                case FilterType.Costcode:
                    return new cCostcodes(accountId);
                   
                case FilterType.Department:
                    return new cDepartments(accountId);
                  
                case FilterType.Projectcode:
                    return new cProjectCodes(accountId);

                case FilterType.Userdefined:
                    return new cUserdefinedFields(accountId);

                default:
                    throw new ArgumentOutOfRangeException(nameof(filterType), filterType, null);
            }
        }
    }
}