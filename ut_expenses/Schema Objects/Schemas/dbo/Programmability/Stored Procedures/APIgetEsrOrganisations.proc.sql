CREATE PROCEDURE [dbo].[APIgetEsrOrganisations]
	@ESROrganisationId bigint
AS
BEGIN
IF @ESROrganisationId = 0
	BEGIN
		SELECT [ESROrganisationId]
		  ,[OrganisationName]
		  ,[OrganisationType]
		  ,[EffectiveFrom]
		  ,[EffectiveTo]
		  ,[HierarchyVersionId]
		  ,[HierarchyVersionFrom]
		  ,[HierarchyVersionTo]
		  ,[DefaultCostCentre]
		  ,[ParentOrganisationId]
		  ,[NACSCode]
		  ,[ESRLocationId]
		  ,[ESRLastUpdateDate]
		  ,[CostCentreDescription]
	  FROM [dbo].[ESROrganisations]
	END
ELSE
	BEGIN
		SELECT [ESROrganisationId]
		  ,[OrganisationName]
		  ,[OrganisationType]
		  ,[EffectiveFrom]
		  ,[EffectiveTo]
		  ,[HierarchyVersionId]
		  ,[HierarchyVersionFrom]
		  ,[HierarchyVersionTo]
		  ,[DefaultCostCentre]
		  ,[ParentOrganisationId]
		  ,[NACSCode]
		  ,[ESRLocationId]
		  ,[ESRLastUpdateDate]
		  ,[CostCentreDescription]
	  FROM [dbo].[ESROrganisations]
	  WHERE @ESROrganisationId = ESROrganisationId
	END
END