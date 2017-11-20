ALTER TABLE [dbo].[custom_entity_form_fields]
    ADD CONSTRAINT [FK_custom_entity_form_fields_custom_entity_form_sections] FOREIGN KEY ([sectionid]) REFERENCES [dbo].[custom_entity_form_sections] ([sectionid]) ON DELETE NO ACTION ON UPDATE SET NULL;

