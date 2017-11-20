CREATE TABLE [dbo].[emailNotifications] (
    [emailNotificationID]   INT             IDENTITY (13, 1) NOT FOR REPLICATION NOT NULL,
    [name]                  NVARCHAR (100)  COLLATE Latin1_General_CI_AS NOT NULL,
    [description]           NVARCHAR (4000) COLLATE Latin1_General_CI_AS NULL,
    [emailTemplateID]       INT             NOT NULL,
    [enabled]               BIT             CONSTRAINT [DF_emailNotifications_enabled] DEFAULT ((0)) NOT NULL,
    [customerType]          INT             CONSTRAINT [DF_emailNotifications_customerType] DEFAULT ((1)) NOT NULL,
    [emailNotificationType] INT             NOT NULL,
    CONSTRAINT [PK_emailNotifications] PRIMARY KEY CLUSTERED ([emailNotificationID] ASC)
);



