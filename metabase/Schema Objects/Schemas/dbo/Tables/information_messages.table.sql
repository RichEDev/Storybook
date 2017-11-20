CREATE TABLE [dbo].[information_messages] (
    [informationID]   INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [title]           NVARCHAR (22)   COLLATE Latin1_General_CI_AS NULL,
    [message]         NVARCHAR (1000) NULL,
    [administratorID] INT             NULL,
    [dateAdded]       DATETIME        NULL,
    [displayOrder]    INT             NULL,
    [deleted]         BIT             NOT NULL
);

