CREATE TABLE [dbo].[previouspasswords] (
    [employeeid]     INT            NOT NULL,
    [password]       NVARCHAR (250) NOT NULL,
    [order]          INT            NOT NULL,
    [passwordMethod] TINYINT        NOT NULL
);

