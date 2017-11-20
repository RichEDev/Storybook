ALTER TABLE [dbo].[registeredusers]
    ADD CONSTRAINT [DF_registeredusers_advancesEnabled] DEFAULT ((1)) FOR [advancesEnabled];

