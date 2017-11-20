ALTER TABLE [dbo].[employees]
    ADD CONSTRAINT [FK_employees_esrTrusts] FOREIGN KEY ([NHSTrustID]) REFERENCES [dbo].[esrTrusts] ([trustID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

