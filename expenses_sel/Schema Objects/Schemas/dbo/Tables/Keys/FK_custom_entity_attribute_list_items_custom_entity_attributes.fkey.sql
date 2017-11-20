ALTER TABLE [dbo].[custom_entity_attribute_list_items]
    ADD CONSTRAINT [FK_custom_entity_attribute_list_items_custom_entity_attributes] FOREIGN KEY ([attributeid]) REFERENCES [dbo].[custom_entity_attributes] ([attributeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

