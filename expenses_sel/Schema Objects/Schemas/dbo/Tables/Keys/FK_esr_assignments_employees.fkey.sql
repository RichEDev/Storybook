ALTER TABLE [dbo].[esr_assignments]
    ADD CONSTRAINT [FK_esr_assignments_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

