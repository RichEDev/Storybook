CREATE TABLE [dbo].[scheduled_reports_columns] (
    [scheduleid]     INT              NOT NULL,
    [literalvalue]   NVARCHAR (1000)  NOT NULL,
    [reportcolumnid] UNIQUEIDENTIFIER NOT NULL
);

