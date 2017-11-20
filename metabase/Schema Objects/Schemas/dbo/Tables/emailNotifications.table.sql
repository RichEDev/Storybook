CREATE TABLE [dbo].[emailNotifications] (
    [emailNotificationID]   INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [name]                  NVARCHAR (100)  COLLATE Latin1_General_CI_AS NOT NULL,
    [description]           NVARCHAR (4000) COLLATE Latin1_General_CI_AS NULL,
    [emailTemplateID]       INT             NOT NULL,
    [enabled]               BIT             NOT NULL,
    [customerType]          INT             NOT NULL,
    [emailNotificationType] INT             NOT NULL
);

