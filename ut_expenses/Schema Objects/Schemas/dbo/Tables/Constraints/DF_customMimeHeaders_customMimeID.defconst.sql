ALTER TABLE [dbo].[customMimeHeaders]
    ADD CONSTRAINT [DF_customMimeHeaders_customMimeID] DEFAULT (newid()) FOR [customMimeID];

