ALTER TABLE [dbo].[custom_entity_attributes]
    ADD CONSTRAINT [FK_custom_entity_attributes_custom_entity_views] FOREIGN KEY ([viewid]) REFERENCES [dbo].[custom_entity_views] ([viewid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

