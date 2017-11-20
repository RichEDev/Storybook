CREATE TABLE [dbo].[rptlistitems] (
    [oldfieldid] INT              NULL,
    [listitem]   NVARCHAR (50)    COLLATE Latin1_General_CI_AS NOT NULL,
    [fieldid]    UNIQUEIDENTIFIER NOT NULL,
    [listvalue]  NVARCHAR (50)    COLLATE Latin1_General_CI_AS NOT NULL,
    [valuetype]  NVARCHAR (50)    COLLATE Latin1_General_CI_AS NULL
);

