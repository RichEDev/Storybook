ALTER TABLE [dbo].[viewgroups_base]
    ADD CONSTRAINT [DF_viewgroups_amendedon] DEFAULT (getdate()) FOR [amendedon];

