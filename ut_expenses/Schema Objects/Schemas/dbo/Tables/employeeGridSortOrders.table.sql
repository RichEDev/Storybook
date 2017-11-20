CREATE TABLE [dbo].[employeeGridSortOrders] (
    [employeeID]   INT              NOT NULL,
    [gridID]       NVARCHAR (250)   NOT NULL,
    [sortedColumn] UNIQUEIDENTIFIER NOT NULL,
    [sortOrder]    TINYINT          NOT NULL,
	[sortJoinViaID] INT				NULL,
	[modifiedOn]	DATETIME		NULL
);

