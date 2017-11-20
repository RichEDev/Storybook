ALTER TABLE [dbo].[exporthistory]
    ADD CONSTRAINT [FK_exporthistory_financial_exports] FOREIGN KEY ([financialexportid]) REFERENCES [dbo].[financial_exports] ([financialexportid]) ON DELETE CASCADE ON UPDATE NO ACTION;

