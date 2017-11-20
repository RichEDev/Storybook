CREATE TABLE [dbo].[pending_email] (
    [emailId]       INT            IDENTITY (1, 1) NOT NULL,
    [emailType]     INT            NOT NULL,
    [Datestamp]     DATETIME       NOT NULL,
    [Subject]       NVARCHAR (150) NULL,
    [Body]          NVARCHAR (MAX) NOT NULL,
    [recipientId]   INT            NOT NULL,
    [recipientType] INT            NOT NULL
);

