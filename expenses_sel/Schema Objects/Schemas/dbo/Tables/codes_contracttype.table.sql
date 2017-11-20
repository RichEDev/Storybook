CREATE TABLE [dbo].[codes_contracttype] (
    [contractTypeId]          INT           IDENTITY (1, 1) NOT NULL,
    [subAccountId]            INT           NULL,
    [contractTypeDescription] NVARCHAR (50) NULL,
    [financialContract]       INT           NULL,
    [archived]                BIT           NOT NULL,
    [createdOn]               DATETIME      NULL,
    [createdBy]               INT           NULL,
    [modifiedOn]              DATETIME      NULL,
    [modifiedBy]              INT           NULL
);

