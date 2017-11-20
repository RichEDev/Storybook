ALTER TABLE [dbo].[customJoinBreakdown]
    ADD CONSTRAINT [FK_customJoinBreakdown_customJoinTables] FOREIGN KEY ([jointableid]) REFERENCES [dbo].[customJoinTables] ([jointableid]) ON DELETE CASCADE ON UPDATE NO ACTION;

