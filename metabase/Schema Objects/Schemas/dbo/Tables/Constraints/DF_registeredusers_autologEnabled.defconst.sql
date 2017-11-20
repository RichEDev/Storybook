ALTER TABLE [dbo].[registeredusers]
    ADD CONSTRAINT [DF_registeredusers_autologEnabled] DEFAULT ((1)) FOR [autologEnabled];

