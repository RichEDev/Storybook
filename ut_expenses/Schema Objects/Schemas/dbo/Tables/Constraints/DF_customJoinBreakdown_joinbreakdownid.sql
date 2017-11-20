ALTER TABLE [dbo].[customJoinBreakdown]
    ADD CONSTRAINT [DF_customJoinBreakdown_joinbreakdownid] DEFAULT (NEWID()) FOR [joinbreakdownid];
