ALTER TABLE [dbo].[custom_entity_attributes]
    ADD CONSTRAINT [FK_custom_entity_attributes_custom_entities1] FOREIGN KEY ([related_entity]) REFERENCES [dbo].[custom_entities] ([entityid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

