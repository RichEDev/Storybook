ALTER TABLE [dbo].[document_mappings_mergetable]
    ADD CONSTRAINT [FK_document_mappings_mergetable_table_mappingid] FOREIGN KEY ([mappingid]) REFERENCES [dbo].[document_mappings] ([mappingid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

