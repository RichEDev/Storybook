ALTER TABLE [dbo].[customEntities]
    ADD CONSTRAINT [DF_customEntities_tableid_new] DEFAULT (newid()) FOR [tableid];

