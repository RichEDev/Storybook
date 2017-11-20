CREATE PROCEDURE [dbo].[returnExpenses] 
	@claimId INT,
	@ids IntPK READONLY,
	@userid INT,
	@modifiedon DATETIME,
	@reason NVARCHAR(4000)
AS
BEGIN
	DELETE FROM [returnedexpenses] WHERE [expenseid] IN (SELECT [c1] FROM @ids);
	INSERT INTO [returnedexpenses] ([note], [expenseid]) SELECT @reason, [c1] FROM @ids;
	EXEC [dbo].[addReturnComment] @claimId, @ids, @reason, @userid;
	UPDATE [savedexpenses] SET [returned] = 1 WHERE [expenseid] IN (SELECT [c1] FROM @ids);
	UPDATE [floats] SET [settled] = 0 WHERE [floatid] IN (SELECT [floatid] FROM [float_allocations] WHERE [expenseid] IN (SELECT [c1] FROM @ids));
END