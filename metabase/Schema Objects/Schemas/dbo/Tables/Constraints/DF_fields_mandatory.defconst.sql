ALTER TABLE [dbo].[fields_base]
    ADD CONSTRAINT [DF_fields_mandatory] DEFAULT ((0)) FOR [mandatory];

