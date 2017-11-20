CREATE TABLE [dbo].[information_messages] (
    [informationID]   INT             IDENTITY (87, 1) NOT FOR REPLICATION NOT NULL,
    [title]           NVARCHAR (22)   COLLATE Latin1_General_CI_AS NULL,
    [message]         NVARCHAR (1000) NULL,
    [administratorID] INT             NULL,
    [dateAdded]       DATETIME        NULL,
    [displayOrder]    INT             NULL,
    [deleted]         BIT             CONSTRAINT [DF_information_messages_deleted] DEFAULT ((0)) NOT NULL,
    [MobileInformationMessage] NVARCHAR(400) NULL, 
    CONSTRAINT [PK_information_messages] PRIMARY KEY CLUSTERED ([informationID] ASC)
);



