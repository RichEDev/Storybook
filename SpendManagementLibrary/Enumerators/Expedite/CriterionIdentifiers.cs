namespace SpendManagementLibrary.Enumerators.Expedite
{
    /// <summary>
    /// Represents the 16 static identifiers of the common validation criteria.
    /// </summary>
    public enum CriterionIdentifiers
    {
        InvoiceNumber = 1,

        InvoiceTotal = 2,

        SupplierNameAndAddress = 3,

        Date = 4,

        TypeOfSupply = 5,

        RecipientBusinessName = 6,

        ReceiptAppearsOriginal = 7,

        ReceiptIsSingleReceipt = 8,

        ReceiptIsActualReceipt = 9,

        VatNumber = 10,

        AmountExVat = 11,

        VatAmount = 12,

        VatRate = 13,

        CompositeVatRate = 14,

        AmountIncVat = 15,

        UkVatReceipt = 16
    }
}

