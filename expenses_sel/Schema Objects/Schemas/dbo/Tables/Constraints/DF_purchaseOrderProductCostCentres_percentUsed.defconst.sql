ALTER TABLE [dbo].[purchaseOrderProductCostCentres]
    ADD CONSTRAINT [DF_purchaseOrderProductCostCentres_percentUsed] DEFAULT ((0)) FOR [percentUsed];

