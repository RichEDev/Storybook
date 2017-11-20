ALTER TABLE [dbo].[invoices]
    ADD CONSTRAINT [FK_invoice_details_contract_details] FOREIGN KEY ([contractID]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE CASCADE ON UPDATE NO ACTION;

