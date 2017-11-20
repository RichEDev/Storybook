ALTER TABLE [dbo].[countrysubcats]
    ADD CONSTRAINT [FK_countrysubcats_countries] FOREIGN KEY ([countryid]) REFERENCES [dbo].[countries] ([countryid]) ON DELETE CASCADE ON UPDATE NO ACTION;

