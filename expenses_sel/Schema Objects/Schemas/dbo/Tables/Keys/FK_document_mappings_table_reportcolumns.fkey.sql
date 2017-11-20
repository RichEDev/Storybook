ALTER TABLE [dbo].[document_mappings_table]
    ADD CONSTRAINT [FK_document_mappings_table_reportcolumns] FOREIGN KEY ([reportcolumnid]) REFERENCES [dbo].[reportcolumns] ([reportcolumnid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

