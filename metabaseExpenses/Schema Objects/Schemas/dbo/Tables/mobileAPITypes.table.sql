CREATE TABLE [dbo].[mobileAPITypes]
(
	[API_TypeId] [uniqueidentifier] NOT NULL,
	[typeKey] [nvarchar](50) NOT NULL,
	[typeDescription] [nvarchar](250) NULL,
	[modifiedOn] [datetime] NULL
)