CREATE PROCEDURE [dbo].[DeleteContractCategory] 
(
@ID INT, 
@employeeId INT,
@delegateID int
)
AS
DECLARE @categoryDescription nvarchar(50);
DECLARE @subAccountId int;
DECLARE @cnt int;
SET @cnt = 0;

select @categoryDescription = categoryDescription, @subAccountId = subAccountId from codes_contractcategory where categoryId = @ID;

select @cnt = COUNT(contractId) from contract_details where categoryId = @ID;

IF @cnt > 0
	BEGIN
		RETURN -1;
	END

delete from codes_contractcategory where categoryId = @ID;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 109, @ID, @categoryDescription, @subAccountId;

return @cnt
