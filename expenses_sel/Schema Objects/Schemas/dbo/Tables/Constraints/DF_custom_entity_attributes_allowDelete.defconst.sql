ALTER TABLE [dbo].[custom_entity_attributes]
    ADD CONSTRAINT [DF_custom_entity_attributes_allowDelete] DEFAULT ((1)) FOR [allowDelete];

