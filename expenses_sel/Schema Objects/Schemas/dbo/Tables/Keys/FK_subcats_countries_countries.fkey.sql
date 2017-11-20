ALTER TABLE [dbo].[subcats_countries]
    ADD CONSTRAINT [FK_subcats_countries_countries] FOREIGN KEY ([countryid]) REFERENCES [dbo].[countries] ([countryid]) ON DELETE CASCADE ON UPDATE NO ACTION;

