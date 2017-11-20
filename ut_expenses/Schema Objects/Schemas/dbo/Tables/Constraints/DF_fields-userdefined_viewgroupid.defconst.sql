ALTER TABLE [dbo].[fields_userdefined]
    ADD CONSTRAINT [DF_fields-userdefined_viewgroupid] DEFAULT (1) FOR [viewgroupid];

