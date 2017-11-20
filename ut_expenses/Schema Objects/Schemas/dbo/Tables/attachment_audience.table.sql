CREATE TABLE [dbo].[attachment_audience] (
    [audienceId]   INT IDENTITY (1, 1) NOT NULL,
    [attachmentId] INT NULL,
    [audienceType] INT NULL,
    [accessId]     INT NULL
);

