ALTER TABLE [dbo].[quick_entry_columns]
    ADD CONSTRAINT [FK_quick_entry_columns_subcats] FOREIGN KEY ([subcatID]) REFERENCES [dbo].[subcats] ([subcatid]) ON DELETE CASCADE ON UPDATE NO ACTION;

