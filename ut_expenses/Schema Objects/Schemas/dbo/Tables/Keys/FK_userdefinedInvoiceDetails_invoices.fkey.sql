ALTER TABLE [dbo].[userdefinedInvoiceDetails]
    ADD CONSTRAINT [FK_userdefinedInvoiceDetails_invoices] FOREIGN KEY ([invoiceid]) REFERENCES [dbo].[invoices] ([invoiceID]) ON DELETE CASCADE ON UPDATE NO ACTION;

