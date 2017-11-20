CREATE TABLE [dbo].[reports_common_columns] (
    [oldtableid] INT              NULL,
    [oldfieldid] INT              NULL,
    [tableid]    UNIQUEIDENTIFIER NOT NULL,
    [fieldid]    UNIQUEIDENTIFIER NOT NULL,
	[joinPath]   NVARCHAR(MAX)    NULL
);

