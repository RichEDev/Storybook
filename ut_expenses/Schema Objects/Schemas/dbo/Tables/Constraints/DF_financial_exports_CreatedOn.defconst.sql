ALTER TABLE [dbo].[financial_exports]
    ADD CONSTRAINT [DF_financial_exports_CreatedOn] DEFAULT (getdate()) FOR [CreatedOn];

