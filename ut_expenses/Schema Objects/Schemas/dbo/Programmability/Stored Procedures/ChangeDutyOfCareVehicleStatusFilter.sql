CREATE PROCEDURE [dbo].[ChangeDutyOfCareVehicleStatusFilter] @NewFilter NVARCHAR(150)
AS

DECLARE  @ViewAwaitingReview VARCHAR(20) =[dbo].[GetViewId]('EEDC1A2B-77AD-484D-B0A1-DCF5BCD78299')
DECLARE @ViewAllDocuments VARCHAR(20) =[dbo].[GetViewId] ('7FE0A05E-0E59-41C4-B6EC-A683F586CB6D') 
DECLARE @joinviaid INT
SELECT @joinviaid = [joinviaid] FROM [dbo].[joinVia]WHERE dbo.joinVia.joinViaDescription = 'Vehicle'

IF NOT EXISTS(SELECT 1 FROM dbo.customEntityViews WHERE dbo.customEntityViews.viewid in(@ViewAwaitingReview,@ViewAllDocuments))
BEGIN
	PRINT 'View does not exist'
END
ELSE
BEGIN

DECLARE @VehicleSatusCount INT = 0
DECLARE @VehicleSatusFilterId UNIQUEIDENTIFIER = 'DAD8F087-497B-4A83-AB40-6B5B816911EB'
DECLARE @VehicleStartDateFilterId UNIQUEIDENTIFIER = 'D226C1BD-ECC3-4F37-A5FE-58638B1BD66C'
DECLARE @VehicleEndDateFilterId UNIQUEIDENTIFIER = '2AB21296-77EE-4B3D-807C-56EDF936613D'
DECLARE @entityid int =(select dbo.GetEntityId('F0247D8E-FAD3-462D-A19D-C9F793F984E8',0))
SELECT @VehicleSatusCount = count(1)
FROM fieldFilters
WHERE viewid in(@ViewAwaitingReview,@ViewAllDocuments)
	AND fieldFilters.fieldid = @VehicleSatusFilterId
	AND fieldFilters.value = 1

IF @NewFilter = 'VehicleStartAndEndDate' 
BEGIN
    IF  @VehicleSatusCount > 0
	BEGIN
	DELETE 	FROM fieldFilters	WHERE viewid  in(@ViewAwaitingReview,@ViewAllDocuments)	AND fieldFilters.fieldid = @VehicleSatusFilterId
	END

    IF NOT EXISTS(SELECT *FROM fieldFilters WHERE viewid = @ViewAwaitingReview 	AND fieldFilters.fieldid = @VehicleStartDateFilterId AND fieldFilters.condition = 48)
	BEGIN	
	INSERT INTO fieldFilters ([viewid],[fieldid],[condition],[value],[order],[joinViaID],[valueTwo])
	VALUES (@ViewAwaitingReview,@VehicleStartDateFilterId,48,'',(SELECT isnull(1 + MAX([Order]),0) FROM fieldFilters WHERE viewid = @ViewAwaitingReview),@joinviaid,'')
	END

	IF NOT EXISTS(SELECT *FROM fieldFilters WHERE viewid = @ViewAllDocuments 	AND fieldFilters.fieldid = @VehicleStartDateFilterId AND fieldFilters.condition = 48)
	BEGIN	
    INSERT INTO fieldFilters ([viewid],[fieldid],[condition],[value],[order],[joinViaID],[valueTwo])
	VALUES (@ViewAllDocuments,@VehicleStartDateFilterId,48,'',(SELECT isnull(1 + MAX([Order]),0) FROM fieldFilters WHERE viewid = @ViewAllDocuments),@joinviaid,'')
	END

	 IF NOT EXISTS(SELECT *FROM fieldFilters WHERE viewid = @ViewAwaitingReview 	AND fieldFilters.fieldid = @VehicleEndDateFilterId AND fieldFilters.condition = 47)
	BEGIN	
	INSERT INTO fieldFilters ([viewid],[fieldid],[condition],[value],[order],[joinViaID],[valueTwo])
	VALUES (@ViewAwaitingReview,@VehicleEndDateFilterId,47,'',(SELECT isnull(1 + MAX([Order]),0) FROM fieldFilters WHERE viewid = @ViewAwaitingReview),@joinviaid,'')
	END

	IF NOT EXISTS(SELECT *FROM fieldFilters WHERE viewid = @ViewAllDocuments 	AND fieldFilters.fieldid = @VehicleEndDateFilterId AND fieldFilters.condition = 47)
	BEGIN
	INSERT INTO fieldFilters ([viewid],[fieldid],[condition],[value],[order],[joinViaID],[valueTwo])
	VALUES (@ViewAllDocuments,@VehicleEndDateFilterId,47,'',(SELECT isnull(1 + MAX([Order]),0) FROM fieldFilters WHERE viewid = @ViewAllDocuments),@joinviaid,'')
	END

  DELETE fieldFilters WHERE formid in (SELECT formid FROM customEntityForms WHERE entityid =@entityid) and fieldid=@VehicleSatusFilterId
  INSERT INTO fieldFilters(fieldid,condition,value,[order],attributeid,formid) 
  SELECT @VehicleStartDateFilterId,48,'',(SELECT isnull(1 + MAX([Order]),0) FROM fieldFilters WHERE formid = customEntityForms.formid),(SELECT dbo.GetAttributeId('1E5F7E5E-FF90-45E2-A7E5-FF1799668E5F',0)),formid from customEntityForms where entityid =@entityid
  Union
  SELECT @VehicleEndDateFilterId,47,'',(SELECT isnull(2 + MAX([Order]),0) FROM fieldFilters WHERE formid = customEntityForms.formid),(SELECT dbo.GetAttributeId('1E5F7E5E-FF90-45E2-A7E5-FF1799668E5F',0)),formid from customEntityForms where entityid =@entityid

END
ELSE IF @NewFilter = 'VehicleStatus'
BEGIN
    IF  EXISTS(SELECT *FROM fieldFilters WHERE viewid  in(@ViewAwaitingReview,@ViewAllDocuments)	AND fieldFilters.fieldid = @VehicleStartDateFilterId AND fieldFilters.condition = 48)
	BEGIN
		DELETE 	FROM fieldFilters	WHERE viewid  in(@ViewAwaitingReview,@ViewAllDocuments)	AND fieldFilters.fieldid = @VehicleStartDateFilterId AND fieldFilters.condition = 48
	END

	IF  EXISTS(SELECT *FROM fieldFilters WHERE viewid  in(@ViewAwaitingReview,@ViewAllDocuments)	AND fieldFilters.fieldid = @VehicleEndDateFilterId AND fieldFilters.condition = 47)
	BEGIN
		DELETE 	FROM fieldFilters	WHERE viewid  in(@ViewAwaitingReview,@ViewAllDocuments)	AND fieldFilters.fieldid = @VehicleEndDateFilterId AND fieldFilters.condition = 47
	END	 

	IF @VehicleSatusCount = 0
    BEGIN		
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
		@ViewAwaitingReview
		,@VehicleSatusFilterId
		,1 --equals
		,1
		,(SELECT isnull(1 + MAX([Order]),0) FROM fieldFilters WHERE viewid = @ViewAwaitingReview) --order
		,@joinviaid --joinviaid
		,''
		)

	    INSERT INTO fieldFilters
        (
		[viewid]
		,[fieldid]
		,[condition]
		,[value]
		,[order]
		,[joinViaID]
		,[valueTwo]
		)
    	VALUES (
		@ViewAllDocuments
		,@VehicleSatusFilterId
		,1 
		,1
		,(SELECT isnull(1 + MAX([Order]),0) FROM fieldFilters WHERE viewid = @ViewAllDocuments) --order
		,@joinviaid --joinviaid
		,''
		)
END

DELETE fieldFilters WHERE formid IN (SELECT formid FROM customEntityForms WHERE entityid =@entityid) and fieldid in(@VehicleStartDateFilterId,@VehicleEndDateFilterId)
INSERT INTO fieldFilters(fieldid,condition,value,[order],attributeid,formid) 
SELECT @VehicleSatusFilterId,1,1,(SELECT isnull(1 + MAX([Order]),0) FROM fieldFilters WHERE formid = customEntityForms.formid),(SELECT dbo.GetAttributeId('1E5F7E5E-FF90-45E2-A7E5-FF1799668E5F',0)),formid from customEntityForms where entityid =@entityid

END
END