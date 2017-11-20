ALTER TABLE [dbo].[purchaseOrderProductCostCentres]
    ADD CONSTRAINT [FK_purchaseOrderProductCostCentres_costcodes] FOREIGN KEY ([costCodeID]) REFERENCES [dbo].[costcodes] ([costcodeid]) ON DELETE CASCADE ON UPDATE CASCADE;

