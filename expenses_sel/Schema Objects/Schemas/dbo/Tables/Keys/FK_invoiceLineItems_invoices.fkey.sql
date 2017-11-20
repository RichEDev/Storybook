ALTER TABLE [dbo].[invoiceLineItems]
    ADD CONSTRAINT [FK_invoiceLineItems_invoices] FOREIGN KEY ([invoiceID]) REFERENCES [dbo].[invoices] ([invoiceID]) ON DELETE CASCADE ON UPDATE CASCADE;

