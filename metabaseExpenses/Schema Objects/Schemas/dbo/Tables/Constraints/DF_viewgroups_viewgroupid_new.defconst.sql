ALTER TABLE [dbo].[viewgroups_base]
    ADD CONSTRAINT [DF_viewgroups_viewgroupid_new] DEFAULT (newid()) FOR [viewgroupid];

