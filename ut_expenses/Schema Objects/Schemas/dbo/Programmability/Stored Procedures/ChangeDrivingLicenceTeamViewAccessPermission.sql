CREATE PROCEDURE [dbo].[ChangeDrivingLicenceTeamViewAccessPermission] @NewValue NVARCHAR(150)

AS
DECLARE @FilterCount INT = 0
DECLARE @AwaitingViewID INT =0

SELECT @AwaitingViewID= viewid from customEntityViews where SystemCustomEntityViewId='4A4D12D1-A7F1-4331-A4EC-13FBB5402060'
DECLARE @MyTeamsDrivingLicenceDocumentViewAlDocuments VARCHAR(20) =[dbo].[GetViewId] ('ED4463D4-0090-481D-9D28-1977336B886E')  -- My Team's Driving Licences (All)	 

SELECT @FilterCount = count(1)
FROM fieldFilters
WHERE viewid IN(@AwaitingViewID,@MyTeamsDrivingLicenceDocumentViewAlDocuments)
	AND fieldFilters.fieldid = '1C45B860-DDAA-47DA-9EEC-981F59CCE795'
	AND fieldFilters.value = '@MY_HIERARCHY'
IF @NewValue = N''
	AND @FilterCount > 0
BEGIN

	DELETE
	FROM fieldFilters
	WHERE viewid IN(@AwaitingViewID,@MyTeamsDrivingLicenceDocumentViewAlDocuments)
		AND fieldFilters.fieldid = '1C45B860-DDAA-47DA-9EEC-981F59CCE795'
		AND fieldFilters.value = '@MY_HIERARCHY'
END

ELSE IF NOT EXISTS(SELECT 1 FROM dbo.customEntityViews WHERE dbo.customEntityViews.viewid IN(@AwaitingViewID,@MyTeamsDrivingLicenceDocumentViewAlDocuments))

BEGIN
	PRINT 'Views does not exist'
END

ELSE IF @NewValue = '@MY_HIERARCHY'
	AND @FilterCount = 0

BEGIN

	DECLARE @joinviaid INT = NULL
	SELECT @JoinViaID = [joinViaID]
	FROM [dbo].[joinVia]
	WHERE [joinViaDescription] = 'Employee: Line Manager'
		AND dbo.joinVia.joinViaPathHash = '2FC4C02CB41534C7DC88E33E45A6B409'

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
		,'1C45B860-DDAA-47DA-9EEC-981F59CCE795'
		,255 --equals
		,@NewValue
		,(SELECT isnull(1 + MAX([Order]),0) FROM fieldFilters WHERE viewid = @AwaitingViewID) --order
		,@joinviaid --joinviaid
		,''
		)

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
		@MyTeamsDrivingLicenceDocumentViewAlDocuments
		,'1C45B860-DDAA-47DA-9EEC-981F59CCE795'
		,255 --equals
		,@NewValue
		,(SELECT isnull(1 + MAX([Order]),0) FROM fieldFilters WHERE viewid = @MyTeamsDrivingLicenceDocumentViewAlDocuments) --order
		,@joinviaid --joinviaid
		,''
		)


END

GO


