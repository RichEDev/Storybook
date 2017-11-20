CREATE TABLE [dbo].[customised_help_text] (
    [helpid]      INT              NOT NULL,
    [page]        NVARCHAR (50)    NULL,
    [description] NVARCHAR (200)   NOT NULL,
    [helptext]    NVARCHAR (4000)  NULL,
    [tooltipID]   UNIQUEIDENTIFIER NOT NULL,
    [tooltipArea] NVARCHAR (3)     NOT NULL,
    [moduleID]    INT              NULL
);

