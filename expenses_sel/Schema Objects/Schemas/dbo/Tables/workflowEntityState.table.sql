CREATE TABLE [dbo].[workflowEntityState] (
    [workflowID] INT NOT NULL,
    [entityID]   INT NOT NULL,
    [stepID]     INT NOT NULL,
    [ownerID]    INT NOT NULL,
    [approverID] INT NULL
);

