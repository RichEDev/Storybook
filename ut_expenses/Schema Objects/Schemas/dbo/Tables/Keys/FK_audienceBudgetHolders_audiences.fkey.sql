ALTER TABLE [dbo].[audienceBudgetHolders]
    ADD CONSTRAINT [FK_audienceBudgetHolders_audiences] FOREIGN KEY ([audienceID]) REFERENCES [dbo].[audiences] ([audienceID]) ON DELETE CASCADE ON UPDATE CASCADE;

