ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_FieldCategory] DEFAULT ((0)) FOR [FieldCategory];

