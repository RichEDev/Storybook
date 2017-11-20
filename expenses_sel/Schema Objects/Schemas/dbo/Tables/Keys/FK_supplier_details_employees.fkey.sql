ALTER TABLE [dbo].[supplier_details]
    ADD CONSTRAINT [FK_supplier_details_employees] FOREIGN KEY ([internalStaffContactId]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE SET NULL ON UPDATE NO ACTION;

