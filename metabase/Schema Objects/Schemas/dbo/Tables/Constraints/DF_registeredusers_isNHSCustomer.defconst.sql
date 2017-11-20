ALTER TABLE [dbo].[registeredusers]
    ADD CONSTRAINT [DF_registeredusers_isNHSCustomer] DEFAULT ((0)) FOR [isNHSCustomer];

