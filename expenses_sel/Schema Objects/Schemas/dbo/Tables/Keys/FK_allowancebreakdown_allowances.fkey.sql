ALTER TABLE [dbo].[allowancebreakdown]
    ADD CONSTRAINT [FK_allowancebreakdown_allowances] FOREIGN KEY ([allowanceid]) REFERENCES [dbo].[allowances] ([allowanceid]) ON DELETE CASCADE ON UPDATE NO ACTION;

