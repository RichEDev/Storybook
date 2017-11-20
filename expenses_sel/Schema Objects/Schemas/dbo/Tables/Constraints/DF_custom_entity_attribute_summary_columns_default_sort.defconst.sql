ALTER TABLE [dbo].[custom_entity_attribute_summary_columns]
    ADD CONSTRAINT [DF_custom_entity_attribute_summary_columns_default_sort] DEFAULT ((0)) FOR [default_sort];

