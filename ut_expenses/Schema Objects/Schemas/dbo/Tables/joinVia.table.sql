CREATE TABLE [dbo].[joinVia] (
	[joinViaID]			INT IDENTITY(1,1) NOT NULL,
	[joinViaDescription] NVARCHAR(MAX) NOT NULL,
	[joinViaAS]			UNIQUEIDENTIFIER NOT NULL,
	[joinViaPathHash]	CHAR(32) NOT NULL,
	[CacheExpiry]		DATETIME NULL
)


