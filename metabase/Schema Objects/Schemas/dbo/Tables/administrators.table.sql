CREATE TABLE [dbo].[administrators] (
    [administratorid] INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [username]        NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [password]        NVARCHAR (100) COLLATE Latin1_General_CI_AS NOT NULL,
    [email]           NVARCHAR (150) COLLATE Latin1_General_CI_AS NOT NULL,
    [title]           NVARCHAR (20)  COLLATE Latin1_General_CI_AS NOT NULL,
    [firstname]       NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [surname]         NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [resellerid]      INT            NULL,
    [archived]        BIT            NOT NULL
);

