ALTER TABLE [dbo].[employee_costcodes]
    ADD CONSTRAINT [FK_employee_costcodes_project_codes] FOREIGN KEY ([projectcodeid]) REFERENCES [dbo].[project_codes] ([projectcodeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

