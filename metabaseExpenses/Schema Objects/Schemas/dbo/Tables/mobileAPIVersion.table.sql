CREATE TABLE [dbo].[mobileAPIVersion]
(
	[VersionID] [uniqueidentifier] NOT NULL,
	[mobileAPITypeID] [uniqueidentifier] NOT NULL,
	[versionNumber] [nvarchar](20) NULL,
	[disableAppUsage] [bit] NOT NULL,
	[notifyMessage] [nvarchar](300) NULL,
	[title] [nvarchar](100) NULL,
	[syncMessage] [nvarchar](300) NULL,
	[appStoreURL] [nvarchar](300) NULL,
	[modifiedOn] [datetime] NULL
)