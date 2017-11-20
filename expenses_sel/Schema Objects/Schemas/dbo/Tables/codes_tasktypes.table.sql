CREATE TABLE [dbo].[codes_tasktypes] (
    [typeId]          INT            IDENTITY (1, 1) NOT NULL,
    [subAccountId]    INT            NOT NULL,
    [typeDescription] NVARCHAR (100) NULL,
    [archived]        BIT            NOT NULL,
    [createdOn]       DATETIME       NULL,
    [createdBy]       INT            NULL,
    [modifiedOn]      DATETIME       NULL,
    [modifiedBy]      INT            NULL
);

