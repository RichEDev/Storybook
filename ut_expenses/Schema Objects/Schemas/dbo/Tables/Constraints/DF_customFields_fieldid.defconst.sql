ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_fieldid] DEFAULT (newid()) FOR [fieldid];

