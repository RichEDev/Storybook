ALTER TABLE [dbo].[mimeTypes]
    ADD CONSTRAINT [DF_mimeTypes_archived] DEFAULT ((0)) FOR [archived];

