CREATE PROCEDURE [dbo].[deleteFlagField] 
	@flagID int,
	@fieldID uniqueidentifier,
	@employeeID INT,
	@delegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	delete from flagAssociatedFields where flagID = @flagID and fieldID = @fieldID
	
	declare @description nvarchar(max)
    declare @fielddescription nvarchar(1000)
    declare @flagDescription nvarchar(2000)
    
    set @flagDescription = (select description from flags where flagID = @flagID)
    set @fieldDescription = (select description from fields where fieldid = @fieldID)
    
    set @description = @flagDescription + ' - Field ' + @fielddescription + ' removed from the flag rule.'
    exec addDeleteEntryToAuditLog @employeeID, @delegateID, 33, @flagid, @description, null;
END

GO