ALTER TABLE [dbo].[userdefinedProductLicences]
    ADD CONSTRAINT [FK_userdefinedProductLicences_productLicences] FOREIGN KEY ([licenceID]) REFERENCES [dbo].[productLicences] ([licenceID]) ON DELETE CASCADE ON UPDATE NO ACTION;

