ALTER TABLE [dbo].[supplier_details]
    ADD CONSTRAINT [FK_supplier_status_supplier_details] FOREIGN KEY ([statusid]) REFERENCES [dbo].[supplier_status] ([statusid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

