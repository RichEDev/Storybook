ALTER TABLE [dbo].[notes]
    ADD CONSTRAINT [FK_notes_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

