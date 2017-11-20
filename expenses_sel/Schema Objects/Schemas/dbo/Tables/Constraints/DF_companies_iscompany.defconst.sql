ALTER TABLE [dbo].[companies]
    ADD CONSTRAINT [DF_companies_iscompany] DEFAULT ((0)) FOR [iscompany];

