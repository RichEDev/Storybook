CREATE PROCEDURE [dbo].[flagItem]
	@expenseID INT,
	@flagID INT, 
	@flagType TINYINT,
	@flagText NVARCHAR(MAX),
	@flagDescription nvarchar(max),
	@duplicateExpenseID int,
	@flagColour tinyint,
	@associatedExpenses IntPk readonly,
	@exceededLimit decimal(18,2),
	@stepNumber tinyint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @flaggeditemid int;
	if (select count(flaggeditemid) from savedexpensesFlags where expenseid = @expenseID and flagID = @flagID and (@stepNumber is null or (@stepNumber is not null and stepNumber = @stepNumber))) > 0
		begin
			set @flaggeditemid = (select flaggeditemid from savedexpensesFlags where expenseid = @expenseID and flagID = @flagID and (@stepNumber is null or (@stepNumber is not null and stepNumber = @stepNumber)))
			update savedexpensesFlags set flagType = @flagType, flagDescription = @flagDescription, flagText = @flagText, duplicateExpenseID = @duplicateExpenseID, flagColour = @flagColour, exceededLimit = @exceededLimit, stepNumber = @stepNumber where expenseid = @expenseID and flagID = @flagID and ((@stepnumber is not null and stepNumber = @stepNumber) or @stepNumber is null)
			delete from savedexpensesFlagAssociatedExpenses where flaggedItemId = @flaggeditemid
		end
	else
		begin
			INSERT INTO dbo.savedexpensesFlags (expenseid , flagID , flagType , flagDescription, flagText,duplicateExpenseID, flagColour, exceededLimit, stepNumber) VALUES  (@expenseID, @flagID, @flagType, @flagDescription, @flagText, @duplicateExpenseID, @flagColour, @exceededLimit, @stepNumber)
			set @flaggeditemid = SCOPE_IDENTITY()
		end

	insert into savedExpensesFlagAssociatedExpenses (flaggedItemId, associatedExpenseId) select @flaggeditemid, c1 from @associatedExpenses

	return @flaggeditemid
END

GO