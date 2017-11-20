ALTER TABLE [dbo].[employee_costcodes]
    ADD CONSTRAINT [FK_employee_costcodes_departments] FOREIGN KEY ([departmentid]) REFERENCES [dbo].[departments] ([departmentid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

