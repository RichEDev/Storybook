ALTER TABLE [dbo].[reports_export_options]
    ADD CONSTRAINT [DF_reports_export_options_removeCarriageReturns] DEFAULT ((0)) FOR [removeCarriageReturns];

