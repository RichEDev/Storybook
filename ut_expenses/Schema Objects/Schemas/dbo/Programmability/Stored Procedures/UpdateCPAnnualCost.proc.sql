CREATE PROCEDURE [dbo].[UpdateCPAnnualCost] @contractId int, @acv int
AS
	IF(@contractId > 0)
	BEGIN
		IF(@acv = 1)
		BEGIN
		UPDATE contract_details SET [annualContractValue] = (SELECT dbo.GetAnnualContractValue(@contractId)) WHERE [contractId] = @contractId
		END
		
		UPDATE contract_details SET [totalMaintenanceValue] = (SELECT dbo.GetAnnualContractValue(@contractId)) WHERE [contractId] = @contractId
	END
	ELSE
	BEGIN
		-- Update for ALL contracts
		IF(@acv = 1)
		BEGIN
			UPDATE contract_details SET [annualContractValue] = (SELECT dbo.GetAnnualContractValue([ContractId])) 
		END

		UPDATE contract_details SET [totalMaintenanceValue] = (SELECT dbo.GetAnnualContractValue([ContractId])) 
	END
