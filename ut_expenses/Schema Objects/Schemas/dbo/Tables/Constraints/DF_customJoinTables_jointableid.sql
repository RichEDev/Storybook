ALTER TABLE [dbo].[customJoinTables]
    ADD CONSTRAINT [DF_customJoinTables_jointableid] DEFAULT (NEWID()) FOR [jointableid];
