CREATE TABLE [dbo].[audit_log] (
    [auditlogid] INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [companyid]  NVARCHAR (50)   NULL,
    [username]   NVARCHAR (100)  NOT NULL,
    [datestamp]  DATETIME        NOT NULL,
    [action]     CHAR (1)        COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [category]   NVARCHAR (50)   NOT NULL,
    [field]      NVARCHAR (250)  NULL,
    [oldvalue]   NVARCHAR (2000) NULL,
    [newvalue]   NVARCHAR (2000) NULL
);

