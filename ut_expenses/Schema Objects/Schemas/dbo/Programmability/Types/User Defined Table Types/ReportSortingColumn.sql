CREATE TYPE [dbo].[ReportSortingColumn] AS TABLE(
	[ColumnName] [nvarchar](255) NOT NULL,
	[SortingOrder] [nvarchar](4) NOT NULL,
	[Sequence] int NOT NULL
	PRIMARY KEY CLUSTERED 
(
	[Sequence] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)