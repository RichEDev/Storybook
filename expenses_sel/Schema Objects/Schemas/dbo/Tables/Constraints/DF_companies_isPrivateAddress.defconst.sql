ALTER TABLE [dbo].[companies]
    ADD CONSTRAINT [DF_companies_isPrivateAddress] DEFAULT ((0)) FOR [isPrivateAddress];

