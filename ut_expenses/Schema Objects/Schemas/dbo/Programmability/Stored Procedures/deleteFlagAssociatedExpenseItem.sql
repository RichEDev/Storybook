CREATE PROCEDURE [dbo].[deleteFlagAssociatedExpenseItem] 
 @flagID int,
 @subcatID int,
 @employeeID INT,
 @delegateID INT
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

    -- Insert statements for procedure here
 delete from flagAssociatedExpenseItems where flagID = @flagID and subCatID = @subcatID
 
 declare @description nvarchar(max)
    declare @subcat nvarchar(50)
    declare @flagDescription nvarchar(2000)
    
    set @subcat = (select subcat from subcats where subcatid = @subcatID)
    set @flagDescription = (select description from flags where flagID = @flagID)
 set @description = @flagDescription + ' - Expense item ' + @subcat + ' removed from the flag rule.'
 exec addDeleteEntryToAuditLog @employeeID, @delegateID, 33, @flagid, @description, null;
END

GO