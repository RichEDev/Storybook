ALTER TABLE [dbo].[financial_exports]
    ADD CONSTRAINT [FK_financial_exports_esrTrusts] FOREIGN KEY ([NHSTrustID]) REFERENCES [dbo].[esrTrusts] ([trustID]) ON DELETE NO ACTION ON UPDATE SET NULL;

