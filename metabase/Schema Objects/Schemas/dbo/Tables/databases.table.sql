CREATE TABLE [dbo].[databases] (
    [databaseID]        INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [hostname]          NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [receiptpath]       NVARCHAR (150) COLLATE Latin1_General_CI_AS NULL,
    [cardtemplatepath]  NVARCHAR (150) COLLATE Latin1_General_CI_AS NULL,
    [offlineupdatepath] NVARCHAR (150) COLLATE Latin1_General_CI_AS NULL,
    [policyfilepath]    NVARCHAR (150) COLLATE Latin1_General_CI_AS NULL,
    [cardocumentpath]   NVARCHAR (150) COLLATE Latin1_General_CI_AS NULL,
    [logopath]          NVARCHAR (150) COLLATE Latin1_General_CI_AS NULL,
    [attachmentspath]   NVARCHAR (150) COLLATE Latin1_General_CI_AS NULL
);

