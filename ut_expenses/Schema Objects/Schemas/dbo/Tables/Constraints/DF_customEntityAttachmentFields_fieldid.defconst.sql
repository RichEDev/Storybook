ALTER TABLE [dbo].[customEntityAttachmentFields]
    ADD CONSTRAINT [DF_customEntityAttachmentFields_fieldid] DEFAULT (newid()) FOR [fieldid];

