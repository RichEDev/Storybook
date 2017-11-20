ALTER TABLE [dbo].[ESRElements]
    ADD CONSTRAINT [FK_ESRElements_esrTrusts] FOREIGN KEY ([NHSTrustID]) REFERENCES [dbo].[esrTrusts] ([trustID]) ON DELETE CASCADE ON UPDATE NO ACTION;

