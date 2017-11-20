ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [FK_savedexpenses_current_savedexpenses_current1] FOREIGN KEY ([expenseid]) REFERENCES [dbo].[savedexpenses_current] ([expenseid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

