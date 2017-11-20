ALTER TABLE [dbo].[teamemps]
    ADD CONSTRAINT [DF_teamemps_teamid] DEFAULT (0) FOR [teamid];

