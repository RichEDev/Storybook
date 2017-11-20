CREATE TABLE [dbo].[email_schedule] (
    [scheduleId]      INT          IDENTITY (1, 1) NOT NULL,
    [emailType]       INT          NOT NULL,
    [emailParam]      VARCHAR (30) NULL,
    [emailFrequency]  INT          NULL,
    [frequencyParam]  INT          NULL,
    [nextRunDate]     DATETIME     NULL,
    [runTime]         VARCHAR (8)  NULL,
    [templateId]      INT          NULL,
    [runSubAccountId] INT          NOT NULL
);

