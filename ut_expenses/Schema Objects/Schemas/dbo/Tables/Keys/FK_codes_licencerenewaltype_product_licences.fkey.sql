ALTER TABLE [dbo].[productLicences]
    ADD CONSTRAINT [FK_codes_licencerenewaltype_product_licences] FOREIGN KEY ([renewalType]) REFERENCES [dbo].[licenceRenewalTypes] ([renewalType]) ON DELETE NO ACTION ON UPDATE NO ACTION;

