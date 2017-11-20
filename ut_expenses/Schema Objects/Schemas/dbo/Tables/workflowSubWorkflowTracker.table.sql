CREATE TABLE [dbo].[workflowSubWorkflowTracker] (
    [workflowTrackerID]   INT IDENTITY (1, 1)  NOT NULL,
    [primaryWorkflowID]   INT NOT NULL,
    [secondaryWorkflowID] INT NOT NULL,
    [entityID]            INT NOT NULL
);

