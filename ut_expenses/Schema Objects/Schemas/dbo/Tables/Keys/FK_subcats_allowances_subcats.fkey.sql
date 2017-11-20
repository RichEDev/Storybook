ALTER TABLE [dbo].[subcats_allowances]
    ADD CONSTRAINT [FK_subcats_allowances_subcats] FOREIGN KEY ([subcatid]) REFERENCES [dbo].[subcats] ([subcatid]) ON DELETE CASCADE ON UPDATE NO ACTION;

