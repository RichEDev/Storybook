CREATE TABLE [dbo].[help_text] (
    [page]        NVARCHAR (50)    COLLATE Latin1_General_CI_AS NULL,
    [description] NVARCHAR (200)   COLLATE Latin1_General_CI_AS NOT NULL,
    [helptext]    NVARCHAR (4000)  COLLATE Latin1_General_CI_AS NULL,
    [tooltipID]   UNIQUEIDENTIFIER NOT NULL,
    [tooltipArea] NVARCHAR (3)     COLLATE Latin1_General_CI_AS NOT NULL,
    [moduleID]    INT              NULL
);

