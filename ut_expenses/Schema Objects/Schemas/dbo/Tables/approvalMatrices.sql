CREATE TABLE [dbo].[approvalMatrices] (
    [approvalMatrixId]              INT             IDENTITY (1, 1) NOT NULL,
    [name]                          NVARCHAR (250)  NOT NULL,
    [description]                   NVARCHAR (4000) NULL,
    [createdOn]                     DATETIME        CONSTRAINT [DF_approvalMatrices_createdOn] DEFAULT (getutcdate()) NOT NULL,
    [createdBy]                     INT             NOT NULL,
    [modifiedOn]                    DATETIME        NULL,
    [modifiedBy]                    INT             NULL,
    [cacheExpiry]                   DATETIME        NOT NULL,
    [defaultApproverBudgetHolderId] INT             NULL,
    [defaultApproverEmployeeId]     INT             NULL,
    [defaultApproverTeamId]         INT             NULL,
    CONSTRAINT [PK_approvalMatrices] PRIMARY KEY CLUSTERED ([approvalMatrixId] ASC),
    CONSTRAINT [FK_approvalMatrices_budgetholders] FOREIGN KEY ([defaultApproverBudgetHolderId]) REFERENCES [dbo].[budgetholders] ([budgetholderid]),
    CONSTRAINT [FK_approvalMatrices_employees] FOREIGN KEY ([defaultApproverEmployeeId]) REFERENCES [dbo].[employees] ([employeeid]),
    CONSTRAINT [FK_approvalMatrices_employees_createdBy] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[employees] ([employeeid]),
    CONSTRAINT [FK_approvalMatrices_employees_modifiedBy] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[employees] ([employeeid]),
    CONSTRAINT [FK_approvalMatrices_teams] FOREIGN KEY ([defaultApproverTeamId]) REFERENCES [dbo].[teams] ([teamid])
);