CREATE TABLE [dbo].[workflowsApproval] (
    [workflowStepID]  INT     NOT NULL,
    [workflowID]      INT     NULL,
    [approverType]    TINYINT NOT NULL,
    [approverID]      INT     NULL,
    [oneClickSignOff] BIT     NOT NULL,
    [filterItems]     BIT     NOT NULL,
    [emailTemplateID] INT     NULL
);

