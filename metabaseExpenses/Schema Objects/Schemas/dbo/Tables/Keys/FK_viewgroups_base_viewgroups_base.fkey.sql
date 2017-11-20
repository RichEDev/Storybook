ALTER TABLE [dbo].[viewgroups_base]
    ADD CONSTRAINT [FK_viewgroups_base_viewgroups_base] FOREIGN KEY ([parentid]) REFERENCES [dbo].[viewgroups_base] ([viewgroupid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

