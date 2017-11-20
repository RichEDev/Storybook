ALTER TABLE [dbo].[fields_base]
    ADD CONSTRAINT [DF_fields_valuelist] DEFAULT ((0)) FOR [valuelist];

