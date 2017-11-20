CREATE FUNCTION dbo.GetUnrecoveredRecharge(@currentRechargeDate datetime, @CP_Id int)
RETURNS float
BEGIN
DECLARE @totalRechargedVal float
DECLARE @annualCost float
DECLARE @recoveredPct float
DECLARE @monthlyCost float

IF((SELECT COUNT(*) FROM recharge_associations WHERE [ContractProductId] = @CP_Id) > 0)
BEGIN
	SET @totalRechargedVal = ((SELECT SUM(dbo.CalcRechargeValue(@currentRechargeDate, [RechargeId])) FROM [recharge_associations] WHERE [ContractProductId] = @CP_Id))
END
ELSE
BEGIN
	SET @totalRechargedVal = 0
END

SET @annualCost = ((SELECT [MaintenanceValue] FROM [contract_productdetails] WHERE [ContractProductId] = @CP_Id))
IF(@annualCost > 0)
BEGIN
	SET @monthlyCost = (@annualCost / 12)
	SET @recoveredPct = ROUND(((@totalRechargedVal / @monthlyCost) * 100),2)
END
ELSE
BEGIN
	SET @recoveredPct = 100
END
RETURN (100 - @recoveredPct)
END
