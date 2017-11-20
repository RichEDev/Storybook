ALTER TABLE [dbo].[previouspasswords]
    ADD CONSTRAINT [FK_previouspasswords_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE CASCADE;

