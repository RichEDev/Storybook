ALTER TABLE [dbo].[custom_entity_form_fields]
    ADD CONSTRAINT [DF_custom_entity_form_fields_readonly] DEFAULT ((0)) FOR [readonly];

