ALTER TABLE [dbo].[supplier_contacts]
    ADD CONSTRAINT [FK_supplier_details_supplier_contacts] FOREIGN KEY ([supplierid]) REFERENCES [dbo].[supplier_details] ([supplierid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

