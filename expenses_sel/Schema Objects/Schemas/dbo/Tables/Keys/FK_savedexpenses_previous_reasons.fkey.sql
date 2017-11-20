ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [FK_savedexpenses_previous_reasons] FOREIGN KEY ([reasonid]) REFERENCES [dbo].[reasons] ([reasonid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

