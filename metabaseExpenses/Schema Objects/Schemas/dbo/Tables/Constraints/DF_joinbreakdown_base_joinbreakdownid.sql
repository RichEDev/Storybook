ALTER TABLE [dbo].[joinbreakdown_base]
    ADD CONSTRAINT [DF_joinbreakdown_base_joinbreakdownid] DEFAULT (NEWID()) FOR [joinbreakdownid];
