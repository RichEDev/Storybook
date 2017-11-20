ALTER TABLE [dbo].[custom_entity_attributes]
    ADD CONSTRAINT [DF_custom_entity_attributes_mandatory] DEFAULT ((0)) FOR [mandatory];

