ALTER TABLE [dbo].[custom_fields]
    ADD CONSTRAINT [DF_custom_fields_FieldCategory] DEFAULT ((0)) FOR [FieldCategory];

