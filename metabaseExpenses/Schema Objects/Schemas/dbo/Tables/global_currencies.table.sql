CREATE TABLE [dbo].[global_currencies] (
    [globalcurrencyid] INT            IDENTITY (171, 1) NOT FOR REPLICATION NOT NULL,
    [label]            NVARCHAR (500) COLLATE Latin1_General_CI_AS NOT NULL,
    [alphacode]        NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [numericcode]      NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [symbol]           NVARCHAR (50)  COLLATE Latin1_General_CI_AS NULL,
    [createdon]        DATETIME       NULL,
    [modifiedon]       DATETIME       NULL,
    CONSTRAINT [PK_global_currencies] PRIMARY KEY CLUSTERED ([globalcurrencyid] ASC)
);



