CREATE TABLE [dbo].[audienceBudgetHolders] (
    [audienceBudgetHolderID] INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [audienceID]             INT NOT NULL,
    [budgetHolderID]         INT NOT NULL
);

