CREATE TABLE [dbo].[codes_licencetypes] (
    [licenceTypeId] INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [subAccountId]  INT            NULL,
    [description]   NVARCHAR (150) NULL,
    [createdBy]     INT            NULL,
    [createdOn]     DATETIME       NULL,
    [modifiedBy]    INT            NULL,
    [modifiedOn]    DATETIME       NULL,
    [archived]      BIT            NOT NULL
);

