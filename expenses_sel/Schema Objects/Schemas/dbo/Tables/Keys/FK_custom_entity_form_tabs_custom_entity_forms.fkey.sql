ALTER TABLE [dbo].[custom_entity_form_tabs]
    ADD CONSTRAINT [FK_custom_entity_form_tabs_custom_entity_forms] FOREIGN KEY ([formid]) REFERENCES [dbo].[custom_entity_forms] ([formid]) ON DELETE CASCADE ON UPDATE NO ACTION;

