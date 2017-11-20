CREATE FUNCTION [dbo].[VariationCount] (@PrimaryContractId INT)
RETURNS INT AS
BEGIN

DECLARE @variationCount INT
SET @variationCount = (SELECT COUNT(*) AS [variations] FROM [link_variations] WHERE [primaryContractId] = @PrimaryContractId)

RETURN(@variationCount)
END
