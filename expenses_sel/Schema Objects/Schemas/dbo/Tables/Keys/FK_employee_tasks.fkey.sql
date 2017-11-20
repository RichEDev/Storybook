ALTER TABLE [dbo].[tasks]
    ADD CONSTRAINT [FK_employee_tasks] FOREIGN KEY ([taskCreatorId]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

