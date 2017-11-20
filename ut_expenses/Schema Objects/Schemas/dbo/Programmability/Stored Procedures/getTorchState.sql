CREATE PROCEDURE [dbo].[getTorchMergeState]
	@mergeProjectId int,
	@requestNumber uniqueidentifier
AS
	select
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
	from
		TorchMergeStates
	where
		MergeProjectId = @MergeProjectId
		and RequestNumber = @RequestNumber

RETURN 0
