ALTER TABLE [dbo].[jointables_base]
    ADD CONSTRAINT [DF_jointables_base_jointableid] DEFAULT (NEWID()) FOR [jointableid];
