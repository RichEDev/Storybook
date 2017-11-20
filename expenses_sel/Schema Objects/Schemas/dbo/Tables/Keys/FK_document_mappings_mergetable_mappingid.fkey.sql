ALTER TABLE [dbo].[document_mappings_mergetable]
    ADD CONSTRAINT [FK_document_mappings_mergetable_mappingid] FOREIGN KEY ([mappingid]) REFERENCES [dbo].[document_mappings] ([mappingid]) ON DELETE CASCADE ON UPDATE NO ACTION;

