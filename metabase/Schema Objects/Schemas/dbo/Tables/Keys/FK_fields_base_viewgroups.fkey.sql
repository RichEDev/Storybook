ALTER TABLE [dbo].[fields_base]
    ADD CONSTRAINT [FK_fields_base_viewgroups] FOREIGN KEY ([viewgroupid]) REFERENCES [dbo].[viewgroups_base] ([viewgroupid]) ON DELETE SET NULL ON UPDATE NO ACTION;

