ALTER TABLE [dbo].[scheduled_reports]
    ADD CONSTRAINT [FK_scheduled_reports_financial_exports] FOREIGN KEY ([financialexportid]) REFERENCES [dbo].[financial_exports] ([financialexportid]) ON DELETE CASCADE ON UPDATE NO ACTION;

