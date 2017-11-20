CREATE TABLE [dbo].[column_templates] (
    [templateid]   INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [employeeid]   INT              NOT NULL,
    [templatename] NVARCHAR (50)    NOT NULL,
    [description]  NVARCHAR (2000)  NULL,
    [basetable]    UNIQUEIDENTIFIER NOT NULL
);

