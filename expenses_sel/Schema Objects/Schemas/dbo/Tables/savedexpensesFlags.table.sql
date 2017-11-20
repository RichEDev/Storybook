CREATE TABLE [dbo].[savedexpensesFlags] (
    [expenseid]          INT            NOT NULL,
    [flagID]             INT            NULL,
    [flagType]           TINYINT        NOT NULL,
    [flagText]           NVARCHAR (MAX) NOT NULL,
    [duplicateExpenseID] INT            NULL
);

