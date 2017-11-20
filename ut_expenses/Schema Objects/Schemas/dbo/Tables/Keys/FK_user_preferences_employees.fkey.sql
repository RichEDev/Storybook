ALTER TABLE [dbo].[user_preferences]
    ADD CONSTRAINT [FK_user_preferences_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

