CREATE FUNCTION [dbo].[IsVariation] (@contractId INT)
RETURNS SMALLINT AS
BEGIN
	DECLARE @isVariation INT
	SET @isVariation = (SELECT COUNT(*) AS [IsVariation] FROM [link_variations] WHERE [variationContractId] = @contractId)
	RETURN @isVariation
END
