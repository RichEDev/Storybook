ALTER TABLE [dbo].[companies]
    ADD CONSTRAINT [DF_companies_showfrom] DEFAULT ((0)) FOR [showfrom];

