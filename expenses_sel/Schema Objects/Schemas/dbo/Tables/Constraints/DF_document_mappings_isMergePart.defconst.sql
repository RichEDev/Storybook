ALTER TABLE [dbo].[document_mappings]
    ADD CONSTRAINT [DF_document_mappings_isMergePart] DEFAULT ((0)) FOR [isMergePart];

