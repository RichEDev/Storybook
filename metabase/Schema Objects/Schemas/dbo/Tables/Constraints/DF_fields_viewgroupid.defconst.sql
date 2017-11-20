ALTER TABLE [dbo].[fields_base]
    ADD CONSTRAINT [DF_fields_viewgroupid] DEFAULT ((1)) FOR [viewgroupid_old];

