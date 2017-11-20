ALTER TABLE [dbo].[registeredusers]
    ADD CONSTRAINT [DF_registeredusers_accounttype] DEFAULT ((1)) FOR [accounttype];

