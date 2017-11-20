ALTER TABLE [dbo].[document_mappings_table]
    ADD CONSTRAINT [DF_document_mappings_table_hiddencolumnid] DEFAULT (newid()) FOR [hiddencolumnid];

