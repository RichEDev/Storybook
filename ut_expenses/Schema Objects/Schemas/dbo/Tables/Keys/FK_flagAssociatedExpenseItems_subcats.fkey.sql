ALTER TABLE [dbo].[flagAssociatedExpenseItems]
    ADD CONSTRAINT [FK_flagAssociatedExpenseItems_subcats] FOREIGN KEY ([subCatID]) REFERENCES [dbo].[subcats] ([subcatid]) ON DELETE CASCADE ON UPDATE NO ACTION;

