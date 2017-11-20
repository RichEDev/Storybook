ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [FK_savedexpenses_current_allowances] FOREIGN KEY ([allowanceid]) REFERENCES [dbo].[allowances] ([allowanceid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

