CREATE TYPE [dbo].[StringTable] AS TABLE(
	[StringValue] [nvarchar](255) NOT NULL,
	[Sequence] Int NOT NULL
	PRIMARY KEY CLUSTERED 
(	
	[Sequence] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
