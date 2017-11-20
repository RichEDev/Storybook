ALTER TABLE [dbo].[supplierContactNotes]
    ADD CONSTRAINT [FK_suppliercontactnotes_employees] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

