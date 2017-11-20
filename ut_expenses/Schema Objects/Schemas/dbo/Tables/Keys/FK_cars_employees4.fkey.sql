ALTER TABLE [dbo].[cars]
    ADD CONSTRAINT [FK_cars_employees4] FOREIGN KEY ([taxcheckedby]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

