ALTER TABLE [dbo].[purchaseOrderProducts]
    ADD CONSTRAINT [FK_purchaseOrderProducts_codes_units] FOREIGN KEY ([productUnitOfMeasureID]) REFERENCES [dbo].[codes_units] ([unitId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

