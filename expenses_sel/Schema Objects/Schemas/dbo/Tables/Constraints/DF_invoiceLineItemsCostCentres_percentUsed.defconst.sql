ALTER TABLE [dbo].[invoiceLineItemCostCentres]
    ADD CONSTRAINT [DF_invoiceLineItemsCostCentres_percentUsed] DEFAULT ((0)) FOR [percentUsed];

