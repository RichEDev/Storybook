namespace SpendManagementLibrary
{
    /// <summary>
    /// What area does this field / task apply to.
    /// </summary>
    public enum AppliesTo
    {
        /// <summary>
        /// Applies to contract details.
        /// </summary>
        ContractDetails = 1,

        /// <summary>
        /// Applies to contract products.
        /// </summary>
        ContractProducts = 2,

        /// <summary>
        /// Applies to product details.
        /// </summary>
        ProductDetails = 3,

        /// <summary>
        /// Applies to vendor details.
        /// </summary>
        VendorDetails = 4,

        /// <summary>
        /// Applies to contract grouping.
        /// </summary>
        ContractGrouping = 5,

        /// <summary>
        /// Applies to recharge grouping.
        /// </summary>
        RechargeGrouping = 6,

        /// <summary>
        /// Applies to contract product grouping.
        /// </summary>
        ConprodGrouping = 7,

        /// <summary>
        /// Applies to staff details.
        /// </summary>
        StaffDetails = 8,

        /// <summary>
        /// Applies to invoice details.
        /// </summary>
        InvoiceDetails = 9,

        /// <summary>
        /// Applies to invoice forecasts.
        /// </summary>
        InvoiceForecasts = 10,

        /// <summary>
        /// Applies to vendor contacts.
        /// </summary>
        VendorContacts = 11,

        /// <summary>
        /// Applies to vendor grouping.
        /// </summary>
        VendorGrouping = 12,

        /// <summary>
        /// Applies to employee.
        /// </summary>
        Employee = 13,

        /// <summary>
        /// Applies to expense item.
        /// </summary>
        ExpenseItem = 14,

        /// <summary>
        /// Applies to claim.
        /// </summary>
        Claim = 15,

        /// <summary>
        /// Applies to item category.
        /// </summary>
        ItemCategory = 16,

        /// <summary>
        /// Applies to cars.
        /// </summary>
        Car = 17,

        /// <summary>
        /// Applies to the company.
        /// </summary>
        Company = 18,

        /// <summary>
        /// Applies to costcodes.
        /// </summary>
        Costcode = 19,

        /// <summary>
        /// Applies to departments.
        /// </summary>
        Department = 20,

        /// <summary>
        /// Applies to projectcodes.
        /// </summary>
        Projectcode = 21
    }
}
