CREATE PROCEDURE [dbo].[deleteUserdefinedGrouping]
	@userdefinedGroupID INT,
	@employeeid int,
@CUemployeeID INT,
@CUdelegateID INT,
@lastUpdated datetime out
AS
BEGIN

DECLARE @count INT;
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @groupName NVARCHAR(50);
    SELECT @groupName = groupName FROM userdefinedGroupings WHERE userdefinedGroupID = @userdefinedGroupID;
    
	DELETE FROM [userdefinedGroupings] WHERE userdefinedGroupID = @userdefinedGroupID;
	
	EXEC addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 56, @userdefinedGroupID, @groupName, null
	
	select @lastUpdated = GETDATE()
	RETURN 0;
END
