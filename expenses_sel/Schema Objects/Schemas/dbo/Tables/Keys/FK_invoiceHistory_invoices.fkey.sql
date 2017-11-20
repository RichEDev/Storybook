ALTER TABLE [dbo].[invoiceHistory]
    ADD CONSTRAINT [FK_invoiceHistory_invoices] FOREIGN KEY ([invoiceID]) REFERENCES [dbo].[invoices] ([invoiceID]) ON DELETE CASCADE ON UPDATE NO ACTION;

