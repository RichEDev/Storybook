CREATE PROCEDURE [dbo].[saveTorchMergeState]
	@MergeProjectId int,
	@RequestNumber uniqueidentifier,
    @ProgressCount int,
    @TotalToProcess int,
    @DocumentPath nvarchar(500),
    @DocumentUrl nvarchar(500),
    @DocumentIdentifier uniqueidentifier,
    @MergeCompletionDate datetime,
    @ErrorMessage nvarchar(max),
    @Status int,
    @OutputDocType int
AS
	if not exists (select * from TorchMergeStates where MergeProjectId = @MergeProjectId and RequestNumber = @RequestNumber)
	begin
		insert into TorchMergeStates
		(
			MergeProjectId,
			RequestNumber,
			ProgressCount,
			TotalToProcess,
			DocumentPath,
			DocumentUrl,
			DocumentIdentifier,
			MergeCompletionDate,
			ErrorMessage,
			[Status],
			OutputDocType
		)
		values
		(
			@MergeProjectId,
			@RequestNumber,
			@ProgressCount,
			@TotalToProcess,
			@DocumentPath,
			@DocumentUrl,
			@DocumentIdentifier,
			@MergeCompletionDate,
			@ErrorMessage,
			@Status,
			@OutputDocType
		)
	end
	else
	begin
		update
			TorchMergeStates
		set
			ProgressCount = @ProgressCount,
			TotalToProcess = @TotalToProcess,
			DocumentPath = @DocumentPath,
			DocumentUrl = @DocumentUrl,
			DocumentIdentifier = @DocumentIdentifier,
			MergeCompletionDate = @MergeCompletionDate,
			ErrorMessage = @ErrorMessage,
			[Status] = @Status,
			OutputDocType = @OutputDocType
		where
			@MergeProjectId = @MergeProjectId
			and @RequestNumber = @RequestNumber

	end
RETURN 0
