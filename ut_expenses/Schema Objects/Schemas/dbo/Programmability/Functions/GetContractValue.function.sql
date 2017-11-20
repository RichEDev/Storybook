CREATE FUNCTION [dbo].[GetContractValue](@contractId INT, @includeVariations INT) 
RETURNS FLOAT AS
BEGIN
	DECLARE @contractValue FLOAT
	DECLARE @standardValue float
	SET @standardValue =  (SELECT [contractValue] FROM [contract_details] WHERE [contractId] = @contractId)
	DECLARE @variationsValue float
	SET @variationsValue = 0

	-- @includeVariatons: 1=include, 0=exclude
	IF(@includeVariations = 1)
	BEGIN
		SET @variationsValue = (SELECT ISNULL(SUM([contractValue]),0) FROM [contract_details] WHERE [contractId] IN
					(SELECT [variationContractId] FROM [link_variations] WHERE [primaryContractId] = @contractId))
	END

	SET @contractValue = (@standardValue + @variationsValue)

	RETURN @contractValue
END
