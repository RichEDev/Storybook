ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_mandatory] DEFAULT ((0)) FOR [mandatory];

