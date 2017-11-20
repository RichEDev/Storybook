ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [FK_savedexpenses_previous_currencies] FOREIGN KEY ([currencyid]) REFERENCES [dbo].[currencies] ([currencyid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

