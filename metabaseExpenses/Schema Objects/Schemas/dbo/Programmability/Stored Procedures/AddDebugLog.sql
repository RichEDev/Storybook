create procedure AddDebugLog
(
	@Source				nvarchar(100),
	@Message			nvarchar(MAX),
	@AccountId			int,
	@UserEmployeeId		int,
	@DeligateEmployeeId	int,
	@Uri				nvarchar(MAX),
	@Body				nvarchar(MAX),
	@Headers			nvarchar(MAX),
	@PostData			nvarchar(MAX),
	@Cookies			nvarchar(MAX),
	@ServerVariables	nvarchar(MAX),
	@AdditionalData		nvarchar(MAX),
	@StackTrace			nvarchar(MAX)
)
as
	insert into DebugLog
	values
	(
		getutcdate(),
		@Source,
		@Message,
		@AccountId,
		@UserEmployeeId,
		@DeligateEmployeeId,
		@Uri,
		@Body,
		@Headers,
		@PostData,
		@Cookies,
		@ServerVariables,
		@AdditionalData,
		@StackTrace
	)
