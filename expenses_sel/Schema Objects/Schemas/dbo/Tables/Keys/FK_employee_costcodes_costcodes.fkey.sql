ALTER TABLE [dbo].[employee_costcodes]
    ADD CONSTRAINT [FK_employee_costcodes_costcodes] FOREIGN KEY ([costcodeid]) REFERENCES [dbo].[costcodes] ([costcodeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

