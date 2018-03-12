CREATE PROCEDURE [dbo].[GetPercentageOfClaimItemsForValidation] @stageId INT
AS
BEGIN
	DECLARE @percentage DECIMAL(5, 2)

	SELECT ClaimPercentageToValidate
	FROM signoffs
	WHERE signoffid = @stageId

	RETURN @percentage
END