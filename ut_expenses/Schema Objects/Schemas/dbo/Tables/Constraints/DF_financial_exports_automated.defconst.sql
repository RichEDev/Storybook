ALTER TABLE [dbo].[financial_exports]
    ADD CONSTRAINT [DF_financial_exports_automated] DEFAULT ((0)) FOR [automated];

