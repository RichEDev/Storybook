ALTER TABLE [dbo].[custom_entity_views]
    ADD CONSTRAINT [FK_custom_entity_views_custom_entities] FOREIGN KEY ([entityid]) REFERENCES [dbo].[custom_entities] ([entityid]) ON DELETE CASCADE ON UPDATE NO ACTION;

