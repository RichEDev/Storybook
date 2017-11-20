ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [FK_savedexpenses_previous_allowances] FOREIGN KEY ([allowanceid]) REFERENCES [dbo].[allowances] ([allowanceid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

