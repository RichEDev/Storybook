CREATE TABLE [dbo].[floathistory] (
    [floathistoryid] INT             IDENTITY (1, 1)  NOT NULL,
    [floatid]        INT             NOT NULL,
    [datestamp]      DATETIME        NOT NULL,
    [comment]        NVARCHAR (4000) NOT NULL
);

