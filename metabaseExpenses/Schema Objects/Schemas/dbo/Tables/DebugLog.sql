CREATE TABLE DebugLog
(
	[DateTime]			datetime		NOT NULL,
	[Source]			nvarchar(100)	NOT NULL,
	[Message]			nvarchar(MAX)	NULL,
	AccountId			int				NULL,
	UserEmployeeId		int				NULL,
	DeligateEmployeeId	int				NULL,
	Uri					nvarchar(MAX)	NULL,
	Body				nvarchar(MAX)	NULL,
	Headers				nvarchar(MAX)	NULL,
	PostData			nvarchar(MAX)	NULL,
	Cookies				nvarchar(MAX)	NULL,
	ServerVariables		nvarchar(MAX)	NULL,
	AdditionalData		nvarchar(MAX)	NULL,
	StackTrace			nvarchar(MAX)	NOT NULL
)
