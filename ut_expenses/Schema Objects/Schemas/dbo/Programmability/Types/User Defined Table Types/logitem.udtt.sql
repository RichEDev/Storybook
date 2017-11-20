CREATE TYPE [dbo].[logitem] AS TABLE(
	[reasonID] [int] NOT NULL,
	[elementID] [int] NULL,
	[logItem] [nvarchar](4000) NOT NULL
)