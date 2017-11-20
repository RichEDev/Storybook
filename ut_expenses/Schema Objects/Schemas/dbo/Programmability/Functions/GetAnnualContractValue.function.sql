CREATE FUNCTION [dbo].[GetAnnualContractValue] (@contractId int) 
RETURNS float
AS
BEGIN
	DECLARE @retVal float
	SET @retVal = (SELECT SUM([maintenanceValue]) FROM [contract_productdetails] WHERE [archiveStatus] = 0 AND [contractId] = @contractId)
	
	if @retVal is null
		SET @retVal = 0;
	
	RETURN @retVal
END
