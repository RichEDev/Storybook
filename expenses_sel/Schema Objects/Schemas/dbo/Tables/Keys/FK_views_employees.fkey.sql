ALTER TABLE [dbo].[views]
    ADD CONSTRAINT [FK_views_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE CASCADE;

