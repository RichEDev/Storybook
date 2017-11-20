ALTER TABLE [dbo].[subcats_allowances]
    ADD CONSTRAINT [FK_subcats_allowances_allowances] FOREIGN KEY ([allowanceid]) REFERENCES [dbo].[allowances] ([allowanceid]) ON DELETE CASCADE ON UPDATE NO ACTION;

