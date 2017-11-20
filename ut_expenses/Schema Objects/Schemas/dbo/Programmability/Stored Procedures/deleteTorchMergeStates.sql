CREATE PROCEDURE [dbo].[deleteTorchMergeStates]
	@MergeCompletionDateBefore datetime
AS
	delete from
		TorchMergeStates
	where
		MergeCompletionDate is not null
		and MergeCompletionDate < @MergeCompletionDateBefore

RETURN 0
