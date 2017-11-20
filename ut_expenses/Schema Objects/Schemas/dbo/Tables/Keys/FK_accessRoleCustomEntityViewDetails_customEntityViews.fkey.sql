ALTER TABLE [dbo].[accessRoleCustomEntityViewDetails]
    ADD CONSTRAINT [FK_accessRoleCustomEntityViewDetails_customEntityViews] FOREIGN KEY ([customEntityViewID]) REFERENCES [dbo].[customEntityViews] ([viewid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

