ALTER TABLE [dbo].[companies]
    ADD CONSTRAINT [DF_companies_archived] DEFAULT ((0)) FOR [archived];

