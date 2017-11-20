ALTER TABLE [dbo].[audienceBudgetHolders]
    ADD CONSTRAINT [FK_audienceBudgetHolders_budgetholders] FOREIGN KEY ([budgetHolderID]) REFERENCES [dbo].[budgetholders] ([budgetholderid]) ON DELETE CASCADE ON UPDATE CASCADE;

