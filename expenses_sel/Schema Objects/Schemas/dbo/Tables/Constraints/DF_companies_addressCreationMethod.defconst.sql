ALTER TABLE [dbo].[companies]
    ADD CONSTRAINT [DF_companies_addressCreationMethod] DEFAULT ((1)) FOR [addressCreationMethod];

