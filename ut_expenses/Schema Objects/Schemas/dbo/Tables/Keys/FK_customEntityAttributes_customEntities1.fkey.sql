ALTER TABLE [dbo].[customEntityAttributes]
    ADD CONSTRAINT [FK_customEntityAttributes_customEntities1] FOREIGN KEY ([related_entity]) REFERENCES [dbo].[customEntities] ([entityid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

