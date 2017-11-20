CREATE TABLE [dbo].[emailTemplates] (
    [emailtemplateid] INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [templatename]    NVARCHAR (100)   NULL,
    [subject]         NVARCHAR (500)   NULL,
    [bodyhtml]        NTEXT            NULL,
    [priority]        TINYINT          NOT NULL,
    [basetableid]     UNIQUEIDENTIFIER NOT NULL,
    [systemtemplate]  BIT              NOT NULL,
    [archived]        BIT              NOT NULL,
    [createdon]       DATETIME         NULL,
    [createdby]       INT              NULL,
    [modifiedon]      DATETIME         NULL,
    [modifiedby]      INT              NULL
);

