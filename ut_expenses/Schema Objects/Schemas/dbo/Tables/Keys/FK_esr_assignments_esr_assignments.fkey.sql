ALTER TABLE [dbo].[esr_assignments]
    ADD CONSTRAINT [FK_esr_assignments_esr_assignments] FOREIGN KEY ([esrAssignID]) REFERENCES [dbo].[esr_assignments] ([esrAssignID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

