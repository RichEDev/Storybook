ALTER TABLE [dbo].[invoiceLineItemCostCentres]
    ADD CONSTRAINT [FK_invoiceLineItemCostCentres_costcodes] FOREIGN KEY ([costCodeId]) REFERENCES [dbo].[costcodes] ([costcodeid]) ON DELETE CASCADE ON UPDATE CASCADE;

