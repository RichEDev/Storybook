ALTER TABLE [dbo].[quick_entry_columns]
    ADD CONSTRAINT [FK_quick_entry_columns_quick_entry_forms] FOREIGN KEY ([quickentryid]) REFERENCES [dbo].[quick_entry_forms] ([quickentryid]) ON DELETE CASCADE ON UPDATE NO ACTION;

