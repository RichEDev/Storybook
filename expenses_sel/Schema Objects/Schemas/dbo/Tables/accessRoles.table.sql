CREATE TABLE [dbo].[accessRoles] (
    [roleID]                                 INT             IDENTITY (1, 1) NOT NULL,
    [rolename]                               NVARCHAR (100)  NOT NULL,
    [description]                            NVARCHAR (4000) NULL,
    [expenseClaimMinimumAmount]              DECIMAL (18, 2) NULL,
    [expenseClaimMaximumAmount]              DECIMAL (18, 2) NULL,
    [CreatedOn]                              DATETIME        NULL,
    [CreatedBy]                              INT             NULL,
    [ModifiedOn]                             DATETIME        NULL,
    [ModifiedBy]                             INT             NULL,
    [employeesCanAmendDesignatedCostCode]    BIT             NOT NULL,
    [employeesCanAmendDesignatedDepartment]  BIT             NOT NULL,
    [employeesCanAmendDesignatedProjectCode] BIT             NOT NULL,
    [roleAccessLevel]                        SMALLINT        NOT NULL
);

