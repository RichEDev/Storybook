ALTER TABLE [dbo].[registeredusers]
    ADD CONSTRAINT [DF_registeredusers_contactHelpDeskAllowed] DEFAULT ((1)) FOR [contactHelpDeskAllowed];

