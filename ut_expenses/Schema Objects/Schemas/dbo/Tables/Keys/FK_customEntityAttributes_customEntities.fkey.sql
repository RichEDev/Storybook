ALTER TABLE [dbo].[customEntityAttributes]
    ADD CONSTRAINT [FK_customEntityAttributes_customEntities] FOREIGN KEY ([entityid]) REFERENCES [dbo].[customEntities] ([entityid]) ON DELETE CASCADE ON UPDATE NO ACTION;

