CREATE PROCEDURE [dbo].[deleteUnit]
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

select @description = description, @subAccountId = subAccountId from codes_units where unitId = @ID;

select @cnt = COUNT(contractProductId) from contract_productdetails where unitId = @ID;

IF @cnt > 0
	BEGIN
		RETURN -1;
	END
	
delete from codes_units where unitId = @ID;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 118, @ID, @description, @subAccountId;

return @cnt

