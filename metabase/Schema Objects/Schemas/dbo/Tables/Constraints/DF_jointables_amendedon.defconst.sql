ALTER TABLE [dbo].[jointables_base]
    ADD CONSTRAINT [DF_jointables_amendedon] DEFAULT (getdate()) FOR [amendedon];

