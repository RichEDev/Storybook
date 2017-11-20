ALTER TABLE [dbo].[productLicences]
    ADD CONSTRAINT [FK_codes_licencetypes_product_licences] FOREIGN KEY ([licenceTypeId]) REFERENCES [dbo].[productLicences] ([licenceID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

