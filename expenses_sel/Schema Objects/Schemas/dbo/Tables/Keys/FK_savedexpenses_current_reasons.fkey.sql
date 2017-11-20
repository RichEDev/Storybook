ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [FK_savedexpenses_current_reasons] FOREIGN KEY ([reasonid]) REFERENCES [dbo].[reasons] ([reasonid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

