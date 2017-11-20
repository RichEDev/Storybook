ALTER TABLE [dbo].[invoiceLog]
    ADD CONSTRAINT [FK_invoice_log_invoice_details] FOREIGN KEY ([invoiceID]) REFERENCES [dbo].[invoices] ([invoiceID]) ON DELETE CASCADE ON UPDATE NO ACTION;

