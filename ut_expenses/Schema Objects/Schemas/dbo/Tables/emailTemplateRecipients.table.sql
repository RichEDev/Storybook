CREATE TABLE [dbo].[emailTemplateRecipients] (
    [emailtemplateid]  INT            NOT NULL,
    [sendertype]       TINYINT        NOT NULL,
    [sender]           NVARCHAR (100) NULL,
    [idofsender]       INT            NULL,
    [recipienttype]    TINYINT        NOT NULL,
    [emailRecipientID] INT            IDENTITY (1, 1) NOT NULL
);

