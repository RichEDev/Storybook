ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [FK_contract_details_codes_invoicefrequencytype] FOREIGN KEY ([invoiceFrequencyTypeId]) REFERENCES [dbo].[codes_invoicefrequencytype] ([invoiceFrequencyTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

