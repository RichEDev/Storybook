ALTER TABLE [dbo].[custom_entity_attributes]
    ADD CONSTRAINT [DF_custom_entity_attributes_is_key_field] DEFAULT ((0)) FOR [is_key_field];

