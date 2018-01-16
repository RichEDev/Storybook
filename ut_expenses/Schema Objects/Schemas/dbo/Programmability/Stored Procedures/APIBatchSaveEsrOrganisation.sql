
CREATE PROCEDURE [dbo].[APIBatchSaveEsrOrganisation] @list ApiBatchSaveEsrOrganisationType READONLY
AS
BEGIN
	DECLARE @index INT
	DECLARE @count INT
	DECLARE @ESROrganisationId BIGINT
	DECLARE @OrganisationName NVARCHAR(240)
	DECLARE @OrganisationType NVARCHAR(80)
	DECLARE @EffectiveFrom DATETIME
	DECLARE @EffectiveTo DATETIME
	DECLARE @HierarchyVersionId BIGINT
	DECLARE @HierarchyVersionFrom DATETIME
	DECLARE @HierarchyVersionTo DATETIME
	DECLARE @DefaultCostCentre NVARCHAR(15)
	DECLARE @ParentOrganisationId BIGINT
	DECLARE @NACSCode NVARCHAR(30)
	DECLARE @ESRLocationId BIGINT
	DECLARE @ESRLastUpdateDate DATETIME
	DECLARE @CostCentreDescription NVARCHAR(240)
	DECLARE @tmp TABLE (
		tmpID BIGINT
		,ESROrganisationId BIGINT
		)

	INSERT @tmp
	SELECT ROW_NUMBER() OVER (
			ORDER BY ESROrganisationId
			)
		,ESROrganisationId
	FROM @list

	SELECT @count = count(*)
	FROM @tmp

	SET @index = 1

	WHILE @index <= @count
	BEGIN
		SET @ESROrganisationId = (
				SELECT TOP 1 ESROrganisationId
				FROM @tmp
				WHERE tmpID = @index
				);

		SELECT TOP 1 @OrganisationName = [OrganisationName]
			,@OrganisationType = [OrganisationType]
			,@EffectiveFrom = [EffectiveFrom]
			,@EffectiveTo = EffectiveTo
			,@HierarchyVersionId = [HierarchyVersionId]
			,@HierarchyVersionFrom = [HierarchyVersionFrom]
			,@HierarchyVersionTo = [HierarchyVersionTo]
			,@DefaultCostCentre = [DefaultCostCentre]
			,@ParentOrganisationId = [ParentOrganisationId]
			,@NACSCode = [NACSCode]
			,@ESRLocationId = [ESRLocationId]
			,@ESRLastUpdateDate = [ESRLastUpdateDate]
			,@CostCentreDescription = [CostCentreDescription]
		FROM @list
		WHERE ESROrganisationId = @ESROrganisationId

		IF NOT EXISTS (
				SELECT *
				FROM ESROrganisations
				WHERE ESROrganisationId = @ESROrganisationId
				)
		BEGIN
			INSERT INTO [dbo].[ESROrganisations] (
				[ESROrganisationId]
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
				)
			VALUES (
				@ESROrganisationId
				,@OrganisationName
				,@OrganisationType
				,@EffectiveFrom
				,@EffectiveTo
				,@HierarchyVersionId
				,@HierarchyVersionFrom
				,@HierarchyVersionTo
				,@DefaultCostCentre
				,@ParentOrganisationId
				,@NACSCode
				,@ESRLocationId
				,@ESRLastUpdateDate
				,@CostCentreDescription
				)
		END
		ELSE
		BEGIN
			UPDATE [dbo].[ESROrganisations]
			SET [ESROrganisationId] = @ESROrganisationId
				,[OrganisationName] = @OrganisationName
				,[OrganisationType] = @OrganisationType
				,[EffectiveFrom] = @EffectiveFrom
				,[EffectiveTo] = @EffectiveTo
				,[HierarchyVersionId] = @HierarchyVersionId
				,[HierarchyVersionFrom] = @HierarchyVersionFrom
				,[HierarchyVersionTo] = @HierarchyVersionTo
				,[DefaultCostCentre] = @DefaultCostCentre
				,[ParentOrganisationId] = @ParentOrganisationId
				,[NACSCode] = @NACSCode
				,[ESRLocationId] = @ESRLocationId
				,[ESRLastUpdateDate] = @ESRLastUpdateDate
				,[CostCentreDescription] = @CostCentreDescription
			WHERE [ESROrganisationId] = @ESROrganisationId
		END

		SET @index = @index + 1;
	END

	EXEC ApiSetEsrOrganisationLevel;

	RETURN 0;
END