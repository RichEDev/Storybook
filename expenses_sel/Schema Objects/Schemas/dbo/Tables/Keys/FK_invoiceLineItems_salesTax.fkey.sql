ALTER TABLE [dbo].[invoiceLineItems]
    ADD CONSTRAINT [FK_invoiceLineItems_salesTax] FOREIGN KEY ([salesTaxID]) REFERENCES [dbo].[salesTax] ([salesTaxID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

