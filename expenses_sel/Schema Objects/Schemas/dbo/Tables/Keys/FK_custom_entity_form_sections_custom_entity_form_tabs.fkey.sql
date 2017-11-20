ALTER TABLE [dbo].[custom_entity_form_sections]
    ADD CONSTRAINT [FK_custom_entity_form_sections_custom_entity_form_tabs] FOREIGN KEY ([tabid]) REFERENCES [dbo].[custom_entity_form_tabs] ([tabid]) ON DELETE NO ACTION ON UPDATE SET NULL;

