CREATE TABLE [dbo].[emailTemplates] (
    [emailtemplateid] INT              IDENTITY (1, 1)  NOT NULL,
    [templatename]    NVARCHAR (100)   NULL,
    [subject]         NVARCHAR (MAX)   NULL,
    [bodyhtml]        NTEXT            NULL,
    [priority]        TINYINT          NOT NULL,
    [basetableid]     UNIQUEIDENTIFIER NOT NULL,
    [systemtemplate]  BIT              NOT NULL,
    [archived]        BIT              NOT NULL,
    [createdon]       DATETIME         NULL,
    [createdby]       INT              NULL,
    [modifiedon]      DATETIME         NULL,
    [modifiedby]      INT              NULL, 
    [emailDirection] TINYINT NULL, 
    [sendNote] BIT NULL, 
    [note] NVARCHAR(MAX) NULL, 
    [templateId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT UQ_TemplateId UNIQUE (templateId), 
    [sendEmail] BIT NULL, 
    [CanSendMobileNotification] BIT NULL, 
    [MobileNotificationMessage] NVARCHAR(400) NULL
);

