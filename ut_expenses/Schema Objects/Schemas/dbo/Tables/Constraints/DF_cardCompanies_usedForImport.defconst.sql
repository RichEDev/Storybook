ALTER TABLE [dbo].[cardCompanies]
    ADD CONSTRAINT [DF_cardCompanies_usedForImport] DEFAULT ((0)) FOR [usedForImport];

