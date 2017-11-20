CREATE TABLE [dbo].[returnedexpenses] (
    [returnid]  INT             IDENTITY (1, 1)  NOT NULL,
    [note]      NVARCHAR (4000) NOT NULL,
    [corrected] BIT             NOT NULL,
    [expenseid] INT             NOT NULL,
    [dispute]   NVARCHAR (4000) NULL
);

