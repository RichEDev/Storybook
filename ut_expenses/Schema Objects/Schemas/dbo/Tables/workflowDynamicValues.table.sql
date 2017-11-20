CREATE TABLE [dbo].[workflowDynamicValues] (
    [dynamicValueID]      INT              IDENTITY (1, 1)  NOT NULL,
    [workflowStepID]      INT              NOT NULL,
    [dynamicValueFormula] NVARCHAR (MAX)   NULL,
    [fieldID]             UNIQUEIDENTIFIER NOT NULL
);

