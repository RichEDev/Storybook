CREATE PROCEDURE [dbo].[DeleteContractType]
(
@ID INT, 
@employeeId INT,
@delegateID int
)
AS
DECLARE @contractTypeDescription nvarchar(50);
DECLARE @subAccountId int;
DECLARE @cnt int;
SET @cnt = 0;


select @contractTypeDescription = contractTypeDescription, @subAccountId = subAccountId from codes_contracttype where contractTypeId = @ID;

select @cnt = COUNT(contractId) from contract_details where contractTypeId = @ID;

IF @cnt > 0
	BEGIN
		RETURN -1;
	END

delete from codes_contracttype where contractTypeId = @ID;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 110, @ID, @contractTypeDescription, @subAccountId;

return @cnt

