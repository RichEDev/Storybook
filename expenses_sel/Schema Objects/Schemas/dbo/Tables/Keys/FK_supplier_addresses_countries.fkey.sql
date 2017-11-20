ALTER TABLE [dbo].[supplier_addresses]
    ADD CONSTRAINT [FK_supplier_addresses_countries] FOREIGN KEY ([countryid]) REFERENCES [dbo].[countries] ([countryid]) ON DELETE CASCADE ON UPDATE NO ACTION;

