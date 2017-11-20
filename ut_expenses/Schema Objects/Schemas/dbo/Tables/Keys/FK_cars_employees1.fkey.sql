ALTER TABLE [dbo].[cars]
    ADD CONSTRAINT [FK_cars_employees1] FOREIGN KEY ([insurancecheckedby]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

