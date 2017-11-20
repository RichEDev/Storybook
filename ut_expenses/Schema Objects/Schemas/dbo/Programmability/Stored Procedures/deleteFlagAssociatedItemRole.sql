CREATE PROCEDURE [dbo].[deleteFlagAssociatedItemRole] 
 @flagID int,
 @roleID int,
 @employeeID INT,
 @delegateID INT
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

    -- Insert statements for procedure here
 delete from flagAssociatedRoles where flagID = @flagID and roleID = @roleID
 
 declare @description nvarchar(max)
    declare @rolename nvarchar(50)
    declare @flagDescription nvarchar(2000)
    
    set @rolename = (select rolename from item_roles where itemroleid = @roleID)
    set @flagDescription = (select description from flags where flagID = @flagID)
    set @description = @flagDescription + ' - Item role ' + @rolename + ' removed from the flag rule.'
    
    exec addDeleteEntryToAuditLog @employeeID, @delegateID, 33, @flagid, @description, null;
    
 END

GO