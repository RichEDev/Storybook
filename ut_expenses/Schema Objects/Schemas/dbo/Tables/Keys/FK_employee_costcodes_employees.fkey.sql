ALTER TABLE [dbo].[employee_costcodes]
    ADD CONSTRAINT [FK_employee_costcodes_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE CASCADE;

