CREATE PROCEDURE [dbo].[deleteInflatorMetric]
(
@ID INT, 
@employeeId INT,
@delegateID int
)
AS
DECLARE @name nvarchar(50);
DECLARE @subAccountId int;
DECLARE @cnt int;
SET @cnt = 0;

select @name = name, @subAccountId = subAccountId from codes_inflatormetrics where metricId = @ID;

select @cnt = COUNT(contractId) from contract_details where maintenanceInflatorX = @ID or maintenanceInflatorY = @ID;

IF @cnt > 0
	BEGIN
		RETURN -1;
	END

delete from codes_inflatormetrics where metricId = @ID;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 112, @ID, @name, @subAccountId;

return @cnt


