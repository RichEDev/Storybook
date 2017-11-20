CREATE FUNCTION dbo.GetPostWarrantyCharge(@RA_Id int, @curDate datetime, @NumDays int)
RETURNS float
BEGIN
	DECLARE @curValue float
	SET @curValue = 0

	DECLARE @numDaysInMonth int
	DECLARE @ApportionType int
	DECLARE @PWPortion float
	DECLARE @Maintenance float
	DECLARE @Quantity float

	SET @numDaysInMonth = 
	CASE DATEPART(month,@curDate)
	WHEN 2 THEN
		CASE DATEPART(year,@curDate) % 4
		WHEN 0 THEN 29
		ELSE 28
		END
	WHEN 4 THEN 30
	WHEN 6 THEN 30
	WHEN 9 THEN 30
	WHEN 11 THEN 30
	ELSE 31
	END

	SET @ApportionType = (SELECT [PostWarrantyApportionId] FROM [recharge_associations] WHERE [RechargeId] = @RA_Id)
	SET @PWPortion = (SELECT [PostWarrantyPortion] FROM [recharge_associations] WHERE [RechargeId] = @RA_Id)
	SET @Maintenance = (SELECT [MaintenanceValue] FROM [recharge_associations] INNER JOIN [contract_productdetails] ON [contract_productdetails].[ContractProductId] = [recharge_associations].[ContractProductId] WHERE [RechargeId] = @RA_Id)
	SET @Quantity = (SELECT [Quantity] FROM [recharge_associations] INNER JOIN [contract_productdetails] ON [contract_productdetails].[ContractProductId] = [recharge_associations].[ContractProductId] WHERE [RechargeId] = @RA_Id)

	SET @curValue = 
	CASE @ApportionType
	WHEN 0 THEN ((@Maintenance / 12) / 100) * @PWPortion
	WHEN 1 THEN @PWPortion
	WHEN 2 THEN ((@Maintenance / 12) / @Quantity) * @PWPortion
	ELSE 0
	END

	IF(@numDaysInMonth > 0 And @NumDays > 0)
	BEGIN
		-- only return a proportion of the monthly charge
		DECLARE @retVal float
		SET @retVal = (@curValue / @numDaysInMonth) * @NumDays

		SET @curValue = @retVal
	END
	ELSE
	BEGIN
		SET @curValue = 0
	END

	RETURN @curValue
END
