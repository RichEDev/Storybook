ALTER TABLE [dbo].[companies]
    ADD CONSTRAINT [DF_companies_showto] DEFAULT ((0)) FOR [showto];

