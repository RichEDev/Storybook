ALTER TABLE [dbo].[invoiceNotes]
    ADD CONSTRAINT [FK_invoice_notes_invoice_details] FOREIGN KEY ([invoiceID]) REFERENCES [dbo].[invoices] ([invoiceID]) ON DELETE CASCADE ON UPDATE NO ACTION;

