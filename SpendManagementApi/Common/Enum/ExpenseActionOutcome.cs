namespace SpendManagementApi.Common.Enum
{
    /// <summary>
    /// The expense action outcome.
    /// </summary>
    public enum ExpenseActionOutcome
    {
        /// <summary>
        /// The successs.
        /// </summary>
        Successs,

        /// <summary>
        /// The vat greater than total.
        /// </summary>
        VATGreaterThanTotal,

        /// <summary>
        /// The expense date not in between vehicle dates.
        /// </summary>
        ExpenseDateNotInBetweenVehicleDates,

        /// <summary>
        /// The recommended mileage exceeded.
        /// </summary>
        RecommendedMileageExceeded,

        /// <summary>
        /// The credit card items already reconciled.
        /// </summary>
        CreditCardItemsAlreadyReconciled,

        /// <summary>
        /// The purchase card items already reconciled.
        /// </summary>
        PurchaseCardItemsAlreadyReconciled,

        /// <summary>
        /// The addresses not matched.
        /// </summary>
        AddressesNotMatched
    }
}