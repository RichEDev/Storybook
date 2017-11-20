ALTER TABLE [dbo].[savings]
    ADD CONSTRAINT [FK_savings_employees] FOREIGN KEY ([loggedByUserId]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

