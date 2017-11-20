CREATE TABLE [dbo].[employee_attachmentData] (
    [attachmentID] INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [fileData]     VARBINARY (MAX) NOT NULL
);

