ALTER TABLE [dbo].[custom_entity_attributes]
    ADD CONSTRAINT [FK_custom_entity_attributes_custom_entity_forms] FOREIGN KEY ([formid]) REFERENCES [dbo].[custom_entity_forms] ([formid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

