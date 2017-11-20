ALTER TABLE [dbo].[flagAssociatedExpenseItems]
    ADD CONSTRAINT [FK_flagAssociatedExpenseItems_flags] FOREIGN KEY ([flagID]) REFERENCES [dbo].[flags] ([flagID]) ON DELETE CASCADE ON UPDATE NO ACTION;

