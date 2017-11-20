CREATE TABLE [dbo].[accountProperties] (
    [subAccountID] INT            NOT NULL,
    [stringKey]    NVARCHAR (150) NOT NULL,
    [stringValue]  NVARCHAR (MAX) NULL,
    [createdOn]    DATETIME       NULL,
    [createdBy]    INT            NULL,
    [modifiedOn]   DATETIME       NULL,
    [modifiedBy]   INT            NULL,
    [formPostKey]  NVARCHAR (150) NULL,
    [isGlobal]     BIT            NOT NULL
);

