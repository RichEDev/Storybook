ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_allowimport] DEFAULT ((0)) FOR [allowimport];

