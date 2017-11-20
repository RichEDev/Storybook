ALTER TABLE [dbo].[flagAggregates]
    ADD CONSTRAINT [FK_flagAggregates_flagAggregates] FOREIGN KEY ([flagID]) REFERENCES [dbo].[flags] ([flagID]) ON DELETE CASCADE ON UPDATE NO ACTION;

