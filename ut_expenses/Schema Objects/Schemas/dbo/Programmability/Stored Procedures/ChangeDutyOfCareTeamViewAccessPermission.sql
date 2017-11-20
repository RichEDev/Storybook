CREATE PROCEDURE [dbo].[ChangeDutyOfCareTeamViewAccessPermission] @NewValue NVARCHAR(150)
AS
 
DECLARE @AwaitingViewID INT =0
SELECT @AwaitingViewID= viewId FROM customEntityViews WHERE SystemCustomEntityViewId= 'EEDC1A2B-77AD-484D-B0A1-DCF5BCD78299'
DECLARE @MyTeamsVehicleDocumentDocumentViewAllDocuments VARCHAR(20) =[dbo].[GetViewId] ('7FE0A05E-0E59-41C4-B6EC-A683F586CB6D') 
DECLARE @UsernameFilterId UNIQUEIDENTIFIER = '1C45B860-DDAA-47DA-9EEC-981F59CCE795'

DECLARE @FilterCount INT = 0
SELECT @FilterCount = count(1)
FROM fieldFilters
WHERE viewid in(@AwaitingViewID,@MyTeamsVehicleDocumentDocumentViewAllDocuments)
	AND fieldFilters.fieldid = @UsernameFilterId
	AND fieldFilters.value = '@MY_HIERARCHY'

IF @NewValue = N''
	AND @FilterCount > 0
BEGIN

	DELETE
	FROM fieldFilters
	WHERE viewid  in(@AwaitingViewID,@MyTeamsVehicleDocumentDocumentViewAllDocuments)
		AND fieldFilters.fieldid = @UsernameFilterId
		AND fieldFilters.value = '@MY_HIERARCHY'
END

ELSE IF NOT EXISTS(SELECT 1 FROM dbo.customEntityViews WHERE dbo.customEntityViews.viewid in(@AwaitingViewID,@MyTeamsVehicleDocumentDocumentViewAllDocuments))
BEGIN

	PRINT 'View does not exist'

END

ELSE IF @NewValue = '@MY_HIERARCHY'
	AND @FilterCount = 0
BEGIN

	DECLARE @joinviaid INT = NULL
	SELECT @joinviaid = [joinviaid]
	FROM [dbo].[joinVia]
	WHERE dbo.joinVia.joinViaDescription = 'Vehicle: Employee ID: Line Manager'	

	-- Filter for My Team vehicle Document (Awating Review)
	INSERT INTO fieldFilters (
		[viewid]
		,[fieldid]
		,[condition]
		,[value]
		,[order]
		,[joinViaID]
		,[valueTwo]
		)

	VALUES (
		@AwaitingViewID
		,@UsernameFilterId
		,255 --equals
		,@NewValue
		,(SELECT isnull(1 + MAX([Order]),0) FROM fieldFilters WHERE viewid = @AwaitingViewID) --order
		,@joinviaid --joinviaid
		,''
		)

		-- Filter for My Team vehicle Document (All)
		INSERT INTO fieldFilters (
		[viewid]
		,[fieldid]
		,[condition]
		,[value]
		,[order]
		,[joinViaID]
		,[valueTwo]
		)

	VALUES (
		@MyTeamsVehicleDocumentDocumentViewAllDocuments
		,@UsernameFilterId
		,255 --equals
		,@NewValue
		,(SELECT isnull(1 + MAX([Order]),0) FROM fieldFilters WHERE viewid = @MyTeamsVehicleDocumentDocumentViewAllDocuments) --order
		,@joinviaid --joinviaid
		,''
		)

END 
