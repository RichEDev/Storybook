CREATE TABLE [dbo].[logErrorReasons] (
    [logReasonID] INT             IDENTITY (13, 1) NOT FOR REPLICATION NOT NULL,
    [reasonType]  TINYINT         NOT NULL,
    [reason]      NVARCHAR (4000) COLLATE Latin1_General_CI_AS NOT NULL,
    [createdon]   DATETIME        NULL,
    [modifiedon]  DATETIME        NULL,
    CONSTRAINT [PK_logErrorReasons] PRIMARY KEY CLUSTERED ([logReasonID] ASC)
);



