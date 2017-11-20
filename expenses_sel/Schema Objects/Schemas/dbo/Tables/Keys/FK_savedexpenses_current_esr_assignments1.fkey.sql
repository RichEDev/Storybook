ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [FK_savedexpenses_current_esr_assignments1] FOREIGN KEY ([esrAssignID]) REFERENCES [dbo].[esr_assignments] ([esrAssignID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

