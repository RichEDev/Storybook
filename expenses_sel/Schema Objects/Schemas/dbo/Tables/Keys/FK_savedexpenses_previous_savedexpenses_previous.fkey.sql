ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [FK_savedexpenses_previous_savedexpenses_previous] FOREIGN KEY ([expenseid]) REFERENCES [dbo].[savedexpenses_previous] ([expenseid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

