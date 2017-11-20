ALTER TABLE [dbo].[document_mappings_mergetable]
    ADD CONSTRAINT [PK_document_mappings_mergetable] PRIMARY KEY CLUSTERED ([mergetable_mappingid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

