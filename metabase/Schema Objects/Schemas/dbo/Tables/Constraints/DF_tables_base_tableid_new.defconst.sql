ALTER TABLE [dbo].[tables_base]
    ADD CONSTRAINT [DF_tables_base_tableid_new] DEFAULT (newid()) FOR [tableid];

