ALTER TABLE [dbo].[cars]
    ADD CONSTRAINT [FK_cars_employees2] FOREIGN KEY ([motcheckedby]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

