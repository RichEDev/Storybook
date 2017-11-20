ALTER TABLE [dbo].[supplierNotes]
    ADD CONSTRAINT [FK_suppliernotes_employees] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

