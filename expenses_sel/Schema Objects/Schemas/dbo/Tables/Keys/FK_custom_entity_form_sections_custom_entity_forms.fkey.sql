ALTER TABLE [dbo].[custom_entity_form_sections]
    ADD CONSTRAINT [FK_custom_entity_form_sections_custom_entity_forms] FOREIGN KEY ([formid]) REFERENCES [dbo].[custom_entity_forms] ([formid]) ON DELETE CASCADE ON UPDATE NO ACTION;

