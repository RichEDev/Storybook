ALTER TABLE [dbo].[financial_exports]
    ADD CONSTRAINT [DF_financial_exports_curexportnum] DEFAULT ((1)) FOR [curexportnum];

