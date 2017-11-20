ALTER TABLE [dbo].[document_mappings_table]
    ADD CONSTRAINT [DF_document_mappings_table_table_mappingid] DEFAULT (newid()) FOR [table_mappingid];

