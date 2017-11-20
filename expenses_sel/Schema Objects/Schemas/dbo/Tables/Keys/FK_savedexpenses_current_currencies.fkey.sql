ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [FK_savedexpenses_current_currencies] FOREIGN KEY ([currencyid]) REFERENCES [dbo].[currencies] ([currencyid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

