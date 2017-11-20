ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [FK_savedexpenses_previous_esr_assignments] FOREIGN KEY ([esrAssignID]) REFERENCES [dbo].[esr_assignments] ([esrAssignID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

