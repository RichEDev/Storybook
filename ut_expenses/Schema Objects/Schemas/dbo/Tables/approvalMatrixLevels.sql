CREATE TABLE [dbo].[approvalMatrixLevels] (
    [approvalMatrixLevelId]  INT      IDENTITY (1, 1) NOT NULL,
    [approvalMatrixId]       INT      NOT NULL,
    [approvalLimit]          MONEY    NOT NULL,
    [approverEmployeeId]     INT      NULL,
    [approverTeamId]         INT      NULL,
    [approverBudgetHolderId] INT      NULL,
    [createdOn]              DATETIME CONSTRAINT [DF_approvalMatrixLevels_createdOn] DEFAULT (getutcdate()) NOT NULL,
    [createdBy]              INT      NOT NULL,
    [modifiedOn]             DATETIME NULL,
    [modifiedBy]             INT      NULL,
    CONSTRAINT [PK_approvalMatrixLevels] PRIMARY KEY CLUSTERED ([approvalMatrixLevelId] ASC),
    CONSTRAINT [FK_approvalMatrixLevels_approvalMatrices] FOREIGN KEY ([approvalMatrixId]) REFERENCES [dbo].[approvalMatrices] ([approvalMatrixId]) ON DELETE CASCADE,
    CONSTRAINT [FK_approvalMatrixLevels_budgetholders] FOREIGN KEY ([approverBudgetHolderId]) REFERENCES [dbo].[budgetholders] ([budgetholderid]),
    CONSTRAINT [FK_approvalMatrixLevels_employees] FOREIGN KEY ([approverEmployeeId]) REFERENCES [dbo].[employees] ([employeeid]),
    CONSTRAINT [FK_approvalMatrixLevels_employees_createdBy] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[employees] ([employeeid]),
    CONSTRAINT [FK_approvalMatrixLevels_employees_modifiedBy] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[employees] ([employeeid]),
    CONSTRAINT [FK_approvalMatrixLevels_teams] FOREIGN KEY ([approverTeamId]) REFERENCES [dbo].[teams] ([teamid])
);


