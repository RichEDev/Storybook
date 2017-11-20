ALTER TABLE [dbo].[supplier_details]
    ADD CONSTRAINT [FK_supplier_categories_supplier_details] FOREIGN KEY ([categoryid]) REFERENCES [dbo].[supplier_categories] ([categoryid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

