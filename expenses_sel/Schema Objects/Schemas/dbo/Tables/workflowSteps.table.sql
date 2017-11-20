CREATE TABLE [dbo].[workflowSteps] (
    [workflowStepID]     INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [workflowID]         INT            NOT NULL,
    [parentStepID]       INT            NULL,
    [description]        NVARCHAR (150) NOT NULL,
    [action]             INT            NOT NULL,
    [actionID]           INT            NULL,
    [showQuestionDialog] BIT            NOT NULL,
    [question]           NVARCHAR (150) NULL,
    [trueOption]         NVARCHAR (150) NULL,
    [falseOption]        NVARCHAR (150) NULL,
    [formID]             INT            NULL,
    [relatedStepID]      INT            NULL,
    [message]            NVARCHAR (MAX) NULL
);

