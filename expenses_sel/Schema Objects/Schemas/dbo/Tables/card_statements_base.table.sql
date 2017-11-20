CREATE TABLE [dbo].[card_statements_base] (
    [statementid]    INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [name]           NVARCHAR (250) NOT NULL,
    [statementdate]  DATETIME       NULL,
    [cardproviderid] INT            NOT NULL,
    [CreatedOn]      DATETIME       NULL,
    [CreatedBy]      INT            NULL,
    [ModifiedOn]     DATETIME       NULL,
    [ModifiedBy]     INT            NULL
);

