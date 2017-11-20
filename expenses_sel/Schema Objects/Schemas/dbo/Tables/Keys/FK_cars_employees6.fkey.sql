ALTER TABLE [dbo].[cars]
    ADD CONSTRAINT [FK_cars_employees6] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

