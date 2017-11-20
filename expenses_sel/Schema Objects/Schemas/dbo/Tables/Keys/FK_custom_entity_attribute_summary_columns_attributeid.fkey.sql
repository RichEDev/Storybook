ALTER TABLE [dbo].[custom_entity_attribute_summary_columns]
    ADD CONSTRAINT [FK_custom_entity_attribute_summary_columns_attributeid] FOREIGN KEY ([attributeid]) REFERENCES [dbo].[custom_entity_attributes] ([attributeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

