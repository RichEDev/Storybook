ALTER TABLE [dbo].[customEntityViews]
    ADD CONSTRAINT [FK_customEntityViews_customEntities] FOREIGN KEY ([entityid]) REFERENCES [dbo].[customEntities] ([entityid]) ON DELETE CASCADE ON UPDATE NO ACTION;

