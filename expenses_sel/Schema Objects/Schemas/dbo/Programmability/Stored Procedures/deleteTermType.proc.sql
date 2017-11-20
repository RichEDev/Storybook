CREATE PROCEDURE [dbo].[deleteTermType] 
(
@ID INT, 
@employeeId INT,
@delegateID int
)
AS
DECLARE @termTypeDescription nvarchar(50);
DECLARE @subAccountId int;
DECLARE @cnt int;
SET @cnt = 0;


select @termTypeDescription = termTypeDescription, @subAccountId = subAccountId from codes_termtype where termTypeId = @ID;

select @cnt = COUNT(contractId) from contract_details where termTypeId = @ID;

IF @cnt > 0
	BEGIN
		RETURN -1;
	END

delete from codes_termtype where termTypeId = @ID;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 113, @ID, @termTypeDescription, @subAccountId;

return @cnt

