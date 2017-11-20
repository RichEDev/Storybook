CREATE TABLE [dbo].[workflowConditions] (
    [conditionID]     INT              IDENTITY (1, 1)  NOT NULL,
    [workflowID]      INT              NOT NULL,
    [workflowStepID]  INT              NOT NULL,
    [fieldID]         UNIQUEIDENTIFIER NOT NULL,
    [operator]        TINYINT          NOT NULL,
    [valueOne]        NVARCHAR (200)   NULL,
    [valueTwo]        NVARCHAR (200)   NULL,
    [runtime]         BIT              NULL,
    [updateCriteria]  BIT              NOT NULL,
    [replaceElements] BIT              NOT NULL
);

