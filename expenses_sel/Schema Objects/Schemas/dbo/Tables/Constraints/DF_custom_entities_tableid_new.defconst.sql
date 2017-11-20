ALTER TABLE [dbo].[custom_entities]
    ADD CONSTRAINT [DF_custom_entities_tableid_new] DEFAULT (newid()) FOR [tableid];

