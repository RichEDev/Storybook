ALTER TABLE [dbo].[custom_entity_forms]
    ADD CONSTRAINT [FK_custom_entity_forms_custom_entities] FOREIGN KEY ([entityid]) REFERENCES [dbo].[custom_entities] ([entityid]) ON DELETE CASCADE ON UPDATE NO ACTION;

