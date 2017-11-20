CREATE FUNCTION [dbo].[GetNextVariationSequence] (@contractId INT)
RETURNS INT AS
BEGIN

DECLARE @sequenceNum INT

SET @sequenceNum = (
SELECT ISNULL(MAX(CONVERT(int,[scheduleNumber])),0) AS [MaxVariationNumber] FROM [contract_details] 
INNER JOIN [link_variations] ON [link_variations].[variationContractId] = [contract_details].[contractId]
WHERE [link_variations].[primaryContractId] = @contractId AND dbo.IsVariation([contract_details].[contractId]) > 0 AND ISNUMERIC([scheduleNumber]) > 0
)

RETURN @sequenceNum + 1
END
