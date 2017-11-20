ALTER TABLE [dbo].[custom_entity_attribute_summary]
    ADD CONSTRAINT [FK_custom_entity_attribute_summary_otm_attributeid] FOREIGN KEY ([otm_attributeid]) REFERENCES [dbo].[custom_entity_attributes] ([attributeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

