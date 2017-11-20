ALTER TABLE [dbo].[registeredusers]
    ADD CONSTRAINT [DF_registeredusers_corporateCardsEnabled] DEFAULT ((1)) FOR [corporateCardsEnabled];

