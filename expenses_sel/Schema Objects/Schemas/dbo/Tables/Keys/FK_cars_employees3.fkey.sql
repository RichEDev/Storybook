ALTER TABLE [dbo].[cars]
    ADD CONSTRAINT [FK_cars_employees3] FOREIGN KEY ([servicecheckedby]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

