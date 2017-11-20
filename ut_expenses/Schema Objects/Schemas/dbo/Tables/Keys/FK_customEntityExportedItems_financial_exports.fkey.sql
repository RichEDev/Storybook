ALTER TABLE [dbo].[customEntityExportedItems]
    ADD CONSTRAINT [FK_customEntityExportedItems_financial_exports] FOREIGN KEY ([exportHistoryID]) REFERENCES [dbo].[exporthistory] ([exporthistoryid]) ON DELETE CASCADE ON UPDATE NO ACTION;

