ALTER TABLE [dbo].[document_mappings_table]
    ADD CONSTRAINT [FK_document_mappings_document_mappings_table] FOREIGN KEY ([mappingid]) REFERENCES [dbo].[document_mappings] ([mappingid]) ON DELETE CASCADE ON UPDATE NO ACTION;

