ALTER TABLE [dbo].[document_mappings]
    ADD CONSTRAINT [FK_document_mappings_document_mergesources] FOREIGN KEY ([mergesourceid]) REFERENCES [dbo].[document_mergesources] ([mergesourceid]) ON DELETE CASCADE ON UPDATE NO ACTION;

