ALTER TABLE [dbo].[customTables]
    ADD CONSTRAINT [DF_customTables_tableid_new] DEFAULT (newid()) FOR [tableid];

