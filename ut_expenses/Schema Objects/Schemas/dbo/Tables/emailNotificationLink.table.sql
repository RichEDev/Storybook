CREATE TABLE [dbo].[emailNotificationLink] (
    [emailNotificationLinkID] INT IDENTITY (1, 1)  NOT NULL,
    [emailNotificationID]     INT NOT NULL,
    [employeeID]              INT NULL,
    [teamID]                  INT NULL
);

