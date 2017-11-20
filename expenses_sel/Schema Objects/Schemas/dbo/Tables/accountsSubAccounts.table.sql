CREATE TABLE [dbo].[accountsSubAccounts] (
    [subAccountID] INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [description]  NVARCHAR (150) NULL,
    [archived]     BIT            NOT NULL,
    [createdOn]    DATETIME       NOT NULL,
    [createdBy]    INT            NOT NULL,
    [modifiedOn]   DATETIME       NULL,
    [modifiedBy]   INT            NULL,
    [CacheExpiry]  DATETIME       NULL
);

