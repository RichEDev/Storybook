ALTER TABLE [dbo].[custom_tables]
    ADD CONSTRAINT [DF_custom_tables_tableid_new] DEFAULT (newid()) FOR [tableid];

