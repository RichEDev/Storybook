ALTER TABLE [dbo].[custom_joinbreakdown]
    ADD CONSTRAINT [FK_custom_joinbreakdown_custom_jointables] FOREIGN KEY ([jointableid]) REFERENCES [dbo].[custom_jointables] ([jointableid]) ON DELETE CASCADE ON UPDATE NO ACTION;

