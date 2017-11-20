CREATE PROCEDURE [dbo].[deleteSupplierStatus]
(
@ID INT, 
@employeeId INT,
@delegateID int
)
AS
DECLARE @description nvarchar(50);
DECLARE @subAccountId int;
DECLARE @cnt int;
SET @cnt = 0;

select @description = description, @subAccountId = subAccountId from supplier_status where statusId = @ID;

select @cnt = COUNT(supplierid) from supplier_details where statusid = @ID;

IF @cnt > 0
	BEGIN
		RETURN -1;
	END

delete from supplier_status where statusId = @ID;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 55, @ID, @description, @subAccountId;

return @cnt

