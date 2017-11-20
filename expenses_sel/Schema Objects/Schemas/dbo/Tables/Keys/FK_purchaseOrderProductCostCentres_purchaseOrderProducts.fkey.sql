ALTER TABLE [dbo].[purchaseOrderProductCostCentres]
    ADD CONSTRAINT [FK_purchaseOrderProductCostCentres_purchaseOrderProducts] FOREIGN KEY ([purchaseOrderProductID]) REFERENCES [dbo].[purchaseOrderProducts] ([purchaseOrderProductID]) ON DELETE CASCADE ON UPDATE CASCADE;

