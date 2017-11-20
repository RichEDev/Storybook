ALTER TABLE [dbo].[importTemplateMappings]
    ADD CONSTRAINT [DF_importTemplateMappings_mandatory] DEFAULT ((0)) FOR [mandatory];

