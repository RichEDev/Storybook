ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [FK_savedexpenses_previous_esr_assignments1] FOREIGN KEY ([esrAssignID]) REFERENCES [dbo].[esr_assignments] ([esrAssignID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

