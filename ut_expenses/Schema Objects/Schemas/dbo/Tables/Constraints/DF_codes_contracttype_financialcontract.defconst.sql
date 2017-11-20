ALTER TABLE [dbo].[codes_contracttype]
    ADD CONSTRAINT [DF_codes_contracttype_financialcontract] DEFAULT ((0)) FOR [financialContract];

