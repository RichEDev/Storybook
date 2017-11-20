ALTER TABLE [dbo].[contractNotes]
    ADD CONSTRAINT [FK_contractnotes_employees] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

