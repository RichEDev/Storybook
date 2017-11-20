CREATE CLUSTERED INDEX IX_DebugLog ON DebugLog
(
	[DateTime] DESC,
	[Source]
)
GO
