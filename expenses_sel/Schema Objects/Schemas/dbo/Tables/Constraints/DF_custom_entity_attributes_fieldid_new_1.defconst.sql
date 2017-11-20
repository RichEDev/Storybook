ALTER TABLE [dbo].[custom_entity_attributes]
    ADD CONSTRAINT [DF_custom_entity_attributes_fieldid_new_1] DEFAULT (newid()) FOR [fieldid];

