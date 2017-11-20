ALTER TABLE [dbo].[document_mappings_mergetable]
    ADD CONSTRAINT [DF_document_mappings_meregtable_mappingid] DEFAULT (newid()) FOR [mergetable_mappingid];

