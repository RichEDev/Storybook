CREATE TABLE [dbo].[codes_units] (
    [unitId]       INT           IDENTITY (1, 1) NOT NULL,
    [subAccountId] INT           NULL,
    [description]  NVARCHAR (50) NULL,
    [archived]     BIT           NOT NULL,
    [createdon]    DATETIME      NULL,
    [createdby]    INT           NULL,
    [modifiedon]   DATETIME      NULL,
    [modifiedby]   INT           NULL
);

