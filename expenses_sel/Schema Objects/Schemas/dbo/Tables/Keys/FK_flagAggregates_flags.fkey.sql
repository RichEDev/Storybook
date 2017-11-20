ALTER TABLE [dbo].[flagAggregates]
    ADD CONSTRAINT [FK_flagAggregates_flags] FOREIGN KEY ([aggregateFlagID]) REFERENCES [dbo].[flags] ([flagID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

