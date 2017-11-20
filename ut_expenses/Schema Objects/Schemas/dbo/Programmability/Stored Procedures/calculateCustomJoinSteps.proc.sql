CREATE PROCEDURE dbo.calculateCustomJoinSteps
	@baseTableId UNIQUEIDENTIFIER, 
	@tableId UNIQUEIDENTIFIER
AS
	-- this will only recurse through 31 levels
	--		and it returns the *first* path that finds its way to the baseTableId
	--		not all of the paths
	SELECT * FROM dbo.FindFirstOneToManyPath(@baseTableId, @tableId) WHERE stepSequence > 0; -- any 0 entries are as a result of the recursion going too far and are not part of the joins