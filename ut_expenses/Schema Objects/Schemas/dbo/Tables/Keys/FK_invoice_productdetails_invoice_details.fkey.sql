ALTER TABLE [dbo].[invoiceProductDetails]
    ADD CONSTRAINT [FK_invoice_productdetails_invoice_details] FOREIGN KEY ([invoiceID]) REFERENCES [dbo].[invoices] ([invoiceID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

