ALTER TABLE [dbo].[registeredusers]
    ADD CONSTRAINT [DF_registeredusers_archived] DEFAULT ((0)) FOR [archived];

