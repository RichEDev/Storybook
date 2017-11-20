ALTER TABLE [dbo].[claimhistory]
    ADD CONSTRAINT [DF_claimhistory_stage] DEFAULT (1) FOR [stage];

