CREATE TABLE [dbo].[codes_termtype] (
    [termTypeId]          INT           IDENTITY (1, 1) NOT NULL,
    [subAccountId]        INT           NULL,
    [termTypeDescription] NVARCHAR (50) NULL,
    [createdOn]           DATETIME      NULL,
    [createdBy]           INT           NULL,
    [modifiedOn]          DATETIME      NULL,
    [modifiedBy]          INT           NULL,
    [archived]            BIT           NOT NULL
);

