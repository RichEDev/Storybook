CREATE TABLE [dbo].[help_text_new] (
    [helpid]      FLOAT          NOT NULL,
    [page]        NVARCHAR (255) COLLATE Latin1_General_CI_AS NULL,
    [description] NVARCHAR (255) COLLATE Latin1_General_CI_AS NULL,
    [helptext]    NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NULL
);

