ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [FK_savedexpenses_previous_countries] FOREIGN KEY ([countryid]) REFERENCES [dbo].[countries] ([countryid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

