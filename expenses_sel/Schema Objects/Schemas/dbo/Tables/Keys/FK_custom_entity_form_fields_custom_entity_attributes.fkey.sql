ALTER TABLE [dbo].[custom_entity_form_fields]
    ADD CONSTRAINT [FK_custom_entity_form_fields_custom_entity_attributes] FOREIGN KEY ([attributeid]) REFERENCES [dbo].[custom_entity_attributes] ([attributeid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

