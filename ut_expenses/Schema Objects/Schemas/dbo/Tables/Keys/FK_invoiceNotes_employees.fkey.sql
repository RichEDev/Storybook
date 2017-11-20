ALTER TABLE [dbo].[invoiceNotes]
    ADD CONSTRAINT [FK_invoiceNotes_employees] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

