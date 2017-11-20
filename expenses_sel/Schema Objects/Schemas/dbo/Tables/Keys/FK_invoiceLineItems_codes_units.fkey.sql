ALTER TABLE [dbo].[invoiceLineItems]
    ADD CONSTRAINT [FK_invoiceLineItems_codes_units] FOREIGN KEY ([unitOfMeasureID]) REFERENCES [dbo].[codes_units] ([unitId]) ON DELETE NO ACTION ON UPDATE CASCADE;

