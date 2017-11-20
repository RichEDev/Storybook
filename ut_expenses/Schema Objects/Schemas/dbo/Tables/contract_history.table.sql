CREATE TABLE [dbo].[contract_history] (
    [historyId]    INT           IDENTITY (1, 1) NOT NULL,
    [contractId]   INT           NULL,
    [actionDate]   DATETIME      NOT NULL,
    [Action]       VARCHAR (20)  NULL,
    [modifierName] VARCHAR (100) NULL,
    [employeeId]   INT           NULL,
    [changeField]  VARCHAR (100) NULL,
    [PreVal]       NTEXT         COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [PostVal]      NTEXT         COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [SummaryTab]   INT           NULL
);

