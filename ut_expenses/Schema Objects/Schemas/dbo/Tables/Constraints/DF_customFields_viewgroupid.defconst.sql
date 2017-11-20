ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_viewgroupid] DEFAULT ((1)) FOR [viewgroupid_old];

