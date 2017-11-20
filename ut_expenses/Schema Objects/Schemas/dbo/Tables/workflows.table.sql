CREATE TABLE [dbo].[workflows] (
    [workflowID]         INT              IDENTITY (1, 1) NOT NULL,
    [workflowName]       NVARCHAR (250)   NOT NULL,
    [description]        NVARCHAR (4000)  NULL,
    [canBeChildWorkflow] BIT              CONSTRAINT [DF_workflows_canBeChildWorkflow] DEFAULT ((0)) NOT NULL,
    [runOnCreation]      BIT              CONSTRAINT [DF_Table_1_runOnCreation] DEFAULT ((0)) NOT NULL,
    [runOnChange]        BIT              CONSTRAINT [DF_Table_1_runOnChange] DEFAULT ((0)) NOT NULL,
    [runOnDelete]        BIT              NOT NULL,
    [workflowType]       INT              NOT NULL,
    [createdon]          DATETIME         NULL,
    [createdby]          INT              NULL,
    [modifiedon]         DATETIME         NULL,
    [modifiedby]         INT              NULL,
    [baseTableID]        UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_workflows] PRIMARY KEY CLUSTERED ([workflowID] ASC),
    CONSTRAINT [IX_workflows] UNIQUE NONCLUSTERED ([workflowName] ASC)
);




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1=claim approval;2=selfregistration;3=advances approval;4=car approval;5=table;', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'workflows', @level2type = N'COLUMN', @level2name = N'workflowType';

