CREATE TABLE [dbo].[email_templates] (
    [templateId]       INT            IDENTITY (1, 1) NOT NULL,
    [templateName]     NVARCHAR (50)  NULL,
    [templateType]     INT            NOT NULL,
    [templatePath]     NVARCHAR (200) NULL,
    [templateFilename] NVARCHAR (50)  NULL
);

