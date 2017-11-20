CREATE FUNCTION [dbo].[CostCodeIfExistsInEsrAssignmentCostings] (@cc NVARCHAR(50))
RETURNS NVARCHAR(50)
AS
BEGIN
	DECLARE @ccResult NVARCHAR(50) = NULL;

	IF EXISTS (SELECT [ESRCostingAllocationId] FROM [ESRAssignmentCostings] WHERE [CostCentre] = @cc)
	BEGIN
		SET @ccResult = @cc;
	END
	IF (@ccResult is null)
	BEGIN
		IF EXISTS (SELECT [EsrOrganisationId] FROM [ESROrganisations] WHERE [DefaultCostCentre] = @cc)
		BEGIN
			SET @ccResult = @cc;
		END
	END

	RETURN @ccResult;
END
