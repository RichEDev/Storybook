ALTER TABLE [dbo].[logDataItems]
    ADD CONSTRAINT [FK_logDataItems_logNames] FOREIGN KEY ([logID]) REFERENCES [dbo].[logNames] ([logID]) ON DELETE CASCADE ON UPDATE NO ACTION;

