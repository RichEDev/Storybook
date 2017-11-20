ALTER TABLE [dbo].[productNotes]
    ADD CONSTRAINT [FK_productnotes_employees] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

