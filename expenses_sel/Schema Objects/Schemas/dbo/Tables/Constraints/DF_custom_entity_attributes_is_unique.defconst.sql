ALTER TABLE [dbo].[custom_entity_attributes]
    ADD CONSTRAINT [DF_custom_entity_attributes_is_unique] DEFAULT ((0)) FOR [is_unique];

