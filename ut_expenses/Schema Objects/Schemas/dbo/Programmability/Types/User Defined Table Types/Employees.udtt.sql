CREATE TYPE [dbo].[Employees] AS TABLE(
	[employeeid] [int] NOT NULL,
	[checked] [bit] NULL,
	PRIMARY KEY CLUSTERED 
(
	[employeeid] ASC
)WITH (IGNORE_DUP_KEY = ON)
)