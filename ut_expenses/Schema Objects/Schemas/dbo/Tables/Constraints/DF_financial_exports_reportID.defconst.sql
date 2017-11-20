ALTER TABLE [dbo].[financial_exports]
    ADD CONSTRAINT [DF_financial_exports_reportID] DEFAULT (newid()) FOR [reportID];

