ALTER TABLE [dbo].[custom_entity_attribute_summary]
    ADD CONSTRAINT [FK_custom_entity_attribute_summary_attributeid] FOREIGN KEY ([attributeid]) REFERENCES [dbo].[custom_entity_attributes] ([attributeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

