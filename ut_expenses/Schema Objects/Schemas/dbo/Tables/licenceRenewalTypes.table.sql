CREATE TABLE [dbo].[licenceRenewalTypes] (
    [renewalType]  INT           IDENTITY (1, 1) NOT NULL,
    [subAccountId] INT           NULL,
    [description]  NVARCHAR (50) NULL,
    [archived]     BIT           NOT NULL,
    [createdon]    DATETIME      NOT NULL,
    [createdby]    INT           NOT NULL,
    [modifiedon]   DATETIME      NULL,
    [modifiedby]   INT           NULL
);

