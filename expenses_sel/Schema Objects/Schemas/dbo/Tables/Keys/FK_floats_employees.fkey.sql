ALTER TABLE [dbo].[floats]
    ADD CONSTRAINT [FK_floats_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

