ALTER TABLE [dbo].[customEntityAttributes]
    ADD CONSTRAINT [FK_customEntityAttributes_customEntityViews] FOREIGN KEY ([viewid]) REFERENCES [dbo].[customEntityViews] ([viewid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

