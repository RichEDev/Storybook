ALTER TABLE [dbo].[document_mappings_static]
    ADD CONSTRAINT [FK_document_mappings_text_document_mappings_static] FOREIGN KEY ([static_textid]) REFERENCES [dbo].[document_mappings_text] ([static_textid]) ON DELETE CASCADE ON UPDATE NO ACTION;

