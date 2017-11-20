ALTER TABLE [dbo].[customEntityAttributeListItems]
    ADD CONSTRAINT [FK_customEntityAttributeListItems_customEntityAttributes] FOREIGN KEY ([attributeid]) REFERENCES [dbo].[customEntityAttributes] ([attributeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

