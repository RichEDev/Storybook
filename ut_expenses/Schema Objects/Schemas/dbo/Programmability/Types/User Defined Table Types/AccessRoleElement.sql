CREATE TYPE [dbo].[AccessRoleElement] AS TABLE(
	[elementID] [int] NOT NULL,
	[elementType] [int] NOT NULL,
	[entityElementID] [int] NULL,
	[viewAccess] [bit] NULL,
	[insertAccess] [bit] NULL,
	[updateAccess] [bit] NULL,
	[deleteAccess] [bit] NULL,
	PRIMARY KEY CLUSTERED 
(
	[elementID], [elementType] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)