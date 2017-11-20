CREATE PROCEDURE [dbo].[deleteContractStatus]
(
@ID INT, 
@employeeId INT,
@delegateID int
)
AS
DECLARE @statusDescription nvarchar(50);
DECLARE @subAccountId int;
DECLARE @cnt int;
SET @cnt = 0;

select @statusDescription = statusDescription, @subAccountId = subAccountId from codes_contractstatus where statusId = @ID;

select @cnt = COUNT(contractId) from contract_details where contractStatusId = @ID;

IF @cnt > 0
	BEGIN
		RETURN -1;
	END

delete from codes_contractstatus where statusId = @ID;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 111, @ID, @statusDescription, @subAccountId;

return @cnt
