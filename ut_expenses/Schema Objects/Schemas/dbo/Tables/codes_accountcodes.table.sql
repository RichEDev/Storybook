CREATE TABLE [dbo].[codes_accountcodes] (
    [CodeId]      INT          IDENTITY (1, 1) NOT NULL,
    [LocationId]  INT          NULL,
    [AccountCode] VARCHAR (20) NULL,
    [Description] VARCHAR (50) NULL,
    [CategoryId]  INT          NULL
);

