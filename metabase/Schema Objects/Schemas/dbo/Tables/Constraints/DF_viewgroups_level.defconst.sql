ALTER TABLE [dbo].[viewgroups_base]
    ADD CONSTRAINT [DF_viewgroups_level] DEFAULT ((1)) FOR [level];

