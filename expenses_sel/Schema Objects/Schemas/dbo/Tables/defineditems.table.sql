CREATE TABLE [dbo].[defineditems] (
    [itemid]       INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [userdefineid] INT             NOT NULL,
    [item]         NVARCHAR (50)   NOT NULL,
    [comment]      NVARCHAR (4000) NULL,
    [createdon]    DATETIME        NULL,
    [createdby]    INT             NULL,
    [modifiedon]   DATETIME        NULL,
    [modifiedby]   INT             NULL
);

