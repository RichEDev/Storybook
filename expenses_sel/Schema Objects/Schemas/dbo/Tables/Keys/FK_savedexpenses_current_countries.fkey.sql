ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [FK_savedexpenses_current_countries] FOREIGN KEY ([countryid]) REFERENCES [dbo].[countries] ([countryid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

