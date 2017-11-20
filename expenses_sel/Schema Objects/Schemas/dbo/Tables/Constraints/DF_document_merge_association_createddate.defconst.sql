ALTER TABLE [dbo].[document_merge_association]
    ADD CONSTRAINT [DF_document_merge_association_createddate] DEFAULT (getdate()) FOR [createddate];

