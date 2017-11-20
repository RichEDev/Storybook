CREATE TABLE [dbo].[scheduled_reports_criteria] (
    [scheduleid] INT              NOT NULL,
    [value1]     NVARCHAR (1000)  NOT NULL,
    [value2]     NVARCHAR (1000)  NULL,
    [criteriaid] UNIQUEIDENTIFIER NOT NULL
);

