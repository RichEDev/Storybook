ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_idfield] DEFAULT ((0)) FOR [idfield];

