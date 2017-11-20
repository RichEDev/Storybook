CREATE FUNCTION [dbo].[GetAnnualContractValue] (@contractId int) 
RETURNS float
AS
BEGIN
	DECLARE @retVal float
	SET @retVal = (SELECT SUM([maintenanceValue]) FROM [contract_productdetails] WHERE [archiveStatus] = 0 AND [contractId] = @contractId)

	RETURN @retVal
END
