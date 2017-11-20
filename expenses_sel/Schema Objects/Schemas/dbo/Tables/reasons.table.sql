CREATE TABLE [dbo].[reasons] (
    [reasonid]         INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [reason]           NVARCHAR (50)   NOT NULL,
    [description]      NVARCHAR (4000) NULL,
    [accountcodevat]   NVARCHAR (50)   NULL,
    [accountcodenovat] NVARCHAR (50)   NULL,
    [CreatedOn]        DATETIME        NULL,
    [CreatedBy]        INT             NULL,
    [ModifiedOn]       DATETIME        NULL,
    [ModifiedBy]       INT             NULL
);

