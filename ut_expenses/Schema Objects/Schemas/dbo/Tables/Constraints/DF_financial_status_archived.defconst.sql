ALTER TABLE [dbo].[financial_status]
    ADD CONSTRAINT [DF_financial_status_archived] DEFAULT ((0)) FOR [archived];

