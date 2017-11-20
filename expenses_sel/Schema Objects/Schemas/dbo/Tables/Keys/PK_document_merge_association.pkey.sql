ALTER TABLE [dbo].[document_merge_association]
    ADD CONSTRAINT [PK_document_merge_association] PRIMARY KEY CLUSTERED ([docmergeassociationid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

