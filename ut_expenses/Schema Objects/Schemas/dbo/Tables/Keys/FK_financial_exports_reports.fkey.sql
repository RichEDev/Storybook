ALTER TABLE [dbo].[financial_exports]
    ADD CONSTRAINT [FK_financial_exports_reports] FOREIGN KEY ([reportID]) REFERENCES [dbo].[reports] ([reportID]) ON DELETE CASCADE ON UPDATE NO ACTION;

