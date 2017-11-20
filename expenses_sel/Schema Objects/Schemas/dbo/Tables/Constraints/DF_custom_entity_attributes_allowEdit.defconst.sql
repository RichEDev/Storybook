ALTER TABLE [dbo].[custom_entity_attributes]
    ADD CONSTRAINT [DF_custom_entity_attributes_allowEdit] DEFAULT ((1)) FOR [allowEdit];

