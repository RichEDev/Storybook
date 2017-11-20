ALTER TABLE [dbo].[exporthistory]
    ADD CONSTRAINT [FK_exporthistory_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

