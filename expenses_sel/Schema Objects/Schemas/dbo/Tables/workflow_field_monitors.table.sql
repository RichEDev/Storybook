CREATE TABLE [dbo].[workflow_field_monitors] (
    [fieldID]        INT NOT NULL,
    [workflowID]     INT NOT NULL,
    [listOrder]      INT NOT NULL,
    [fieldMonitorID] INT IDENTITY (1, 1) NOT NULL
);

