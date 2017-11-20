ALTER TABLE [dbo].[default_sorts]
    ADD CONSTRAINT [FK_default_sorts_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

