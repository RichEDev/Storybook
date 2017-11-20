ALTER TABLE [dbo].[customFields]
    ADD CONSTRAINT [DF_customFields_amendedon] DEFAULT (getdate()) FOR [amendedon];

