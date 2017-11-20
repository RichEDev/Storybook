CREATE TABLE [dbo].[costcodes] (
    [costcodeid]          INT             IDENTITY (1, 1) NOT NULL,
    [costcode]            NVARCHAR (50)   NOT NULL,
    [description]         NVARCHAR (4000) NULL,
    [archived]            BIT             CONSTRAINT [DF_costcodes_archived] DEFAULT (0) NOT NULL,
    [CreatedOn]           DATETIME        NULL,
    [CreatedBy]           INT             NULL,
    [ModifiedOn]          DATETIME        NULL,
    [ModifiedBy]          INT             NULL,
    [OwnerEmployeeId]     INT             NULL,
    [OwnerTeamId]         INT             NULL,
    [OwnerBudgetHolderId] INT             NULL,
    CONSTRAINT [PK__cost codes__160F4887] PRIMARY KEY CLUSTERED ([costcodeid] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_costcodes_budgetholders] FOREIGN KEY ([OwnerBudgetHolderId]) REFERENCES [dbo].[budgetholders] ([budgetholderid]),
    CONSTRAINT [FK_costcodes_employees] FOREIGN KEY ([OwnerEmployeeId]) REFERENCES [dbo].[employees] ([employeeid]),
    CONSTRAINT [FK_costcodes_teams] FOREIGN KEY ([OwnerTeamId]) REFERENCES [dbo].[teams] ([teamid]),
    CONSTRAINT [IX_costcodes] UNIQUE NONCLUSTERED ([costcode] ASC)
);



