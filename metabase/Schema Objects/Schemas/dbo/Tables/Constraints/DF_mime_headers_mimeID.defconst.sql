ALTER TABLE [dbo].[mime_headers]
    ADD CONSTRAINT [DF_mime_headers_mimeID] DEFAULT (newid()) FOR [mimeID];

