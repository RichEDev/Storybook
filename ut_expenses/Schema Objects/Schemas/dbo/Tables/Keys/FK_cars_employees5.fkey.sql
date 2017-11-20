ALTER TABLE [dbo].[cars]
    ADD CONSTRAINT [FK_cars_employees5] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

