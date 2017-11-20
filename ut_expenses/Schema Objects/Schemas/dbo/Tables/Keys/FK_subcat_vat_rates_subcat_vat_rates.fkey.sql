ALTER TABLE [dbo].[subcat_vat_rates]
    ADD CONSTRAINT [FK_subcat_vat_rates_subcat_vat_rates] FOREIGN KEY ([subcatid]) REFERENCES [dbo].[subcats] ([subcatid]) ON DELETE CASCADE ON UPDATE NO ACTION;

