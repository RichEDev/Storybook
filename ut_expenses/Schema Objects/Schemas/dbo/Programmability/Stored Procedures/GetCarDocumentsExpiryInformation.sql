CREATE PROCEDURE [dbo].[GetCarDocumentsExpiryInformation]
@carId INT,
@expenseItemDate DATETIME
AS
BEGIN
SET NOCOUNT ON;

DECLARE @sqlQuery1 NVARCHAR(MAX);
DECLARE @sqlQuery3 NVARCHAR(MAX);
DECLARE @sqlQuery2 NVARCHAR(MAX);
DECLARE @TaxValueId NVARCHAR(10);
DECLARE @MotValueId NVARCHAR(10);
DECLARE @InsuranceValueId NVARCHAR(10);
DECLARE @BreakdownValueId NVARCHAR(10);
DECLARE @StatusValueId NVARCHAR(10);

DECLARE @vehicleDocumentsTableId VARCHAR(20) = [dbo].[GetEntityId]('F0247D8E-FAD3-462D-A19D-C9F793F984E8', 1);
DECLARE @attVehicleType VARCHAR(20) = [dbo].[GetAttributeId]('F190DA6F-DC4E-4B54-92C2-1EB68451EFC9', 1);
DECLARE @attVehicleTypeValue VARCHAR(20) = [dbo].[GetAttributeId]('F190DA6F-DC4E-4B54-92C2-1EB68451EFC9', 0);
DECLARE @attVehicle VARCHAR(20) = [dbo].[GetAttributeId]('1E5F7E5E-FF90-45E2-A7E5-FF1799668E5F', 1);
DECLARE @attStartDate VARCHAR(20) = [dbo].[GetAttributeId]('AE8D3951-54EA-40DA-801D-E27B7239C338', 1);
DECLARE @attExpireDate VARCHAR(20) = [dbo].[GetAttributeId]('9E46EB4E-00E8-475D-A89B-1643B139C490', 1);
DECLARE @attStatus VARCHAR(20) = [dbo].[GetAttributeId]('19EC2F1F-0EF4-465B-B6EE-D2CE2FA7021B', 1);
DECLARE @attStatusValue VARCHAR(20) = [dbo].[GetAttributeId]('19EC2F1F-0EF4-465B-B6EE-D2CE2FA7021B', 0);

SELECT @TaxValueId = cast(valueid AS NVARCHAR(10)) FROM customEntityAttributeListItems WHERE attributeid = @attVehicleTypeValue AND item = 'Tax';
SELECT @MotValueId = cast(valueid AS NVARCHAR(10)) FROM customEntityAttributeListItems WHERE attributeid = @attVehicleTypeValue AND item = 'MOT';
SELECT @InsuranceValueId = cast(valueid AS NVARCHAR(10)) FROM customEntityAttributeListItems WHERE attributeid = @attVehicleTypeValue AND item = 'Insurance';
SELECT @BreakdownValueId = cast(valueid AS NVARCHAR(10)) FROM customEntityAttributeListItems WHERE attributeid = @attVehicleTypeValue	AND item = 'Breakdown Cover';
SELECT @StatusValueId = valueid FROM customEntityAttributeListItems WHERE attributeid = @attStatusValue	AND item = 'Reviewed-OK';

SET NOCOUNT OFF;

SET @sqlQuery1 ='

    DECLARE @Taxexpiry DATETIME
	DECLARE @Motexpiry DATETIME
	DECLARE @Insuranceexpiry DATETIME
	DECLARE @BreakdownCoverexpiry DATETIME
	DECLARE @employeeid INT
	DECLARE @blockMOT BIT
	DECLARE @blockTax BIT
	DECLARE @blockInsurance BIT
	DECLARE @blockBreakdownCover BIT
	DECLARE @taxReviewed BIT = 0
	DECLARE @motReviewed BIT = 0
	DECLARE @insuranceReviewed BIT = 0
	DECLARE @breakdowncoverReviewed BIT = 0

	IF EXISTS(SELECT TOP 1 * FROM ' + @vehicleDocumentsTableId +'
	WHERE ' + @attVehicleType +' = ' + @TaxValueId + ' AND (' + @attStartDate + ' IS NULL OR @expenseItemDate BETWEEN ' + @attStartDate + ' AND ' + @attExpireDate +') AND ' + @attVehicle + ' = @carId  AND ' + @attStatus +' = ' + @StatusValueId +'
	ORDER BY ' + @attStatus +' DESC ,' + @attExpireDate +' DESC)
	BEGIN
	SELECT TOP 1 @Taxexpiry=' + @attExpireDate +'
		  ,@taxReviewed = case when ' + @attStatus +' = ' + @StatusValueId + ' THEN 1 ELSE 0 END 
	FROM ' + @vehicleDocumentsTableId +'
	WHERE ' + @attVehicleType +' = ' + @TaxValueId + ' AND (' + @attExpireDate +' IS NOT NULL) AND ' + @attVehicle + ' = @carId 
	ORDER BY ' + @attStatus +' DESC ,' + @attExpireDate +' DESC
	END
	ELSE
	BEGIN
	SELECT  @Taxexpiry=' + @attExpireDate +'
		  ,@taxReviewed = case when ' + @attStatus +' = ' + @StatusValueId + ' THEN 1 ELSE 0 END 
	FROM ' + @vehicleDocumentsTableId +'
	WHERE ' + @attVehicleType +' = ' + @TaxValueId + ' AND (' + @attExpireDate +' IS NOT NULL) AND ' + @attVehicle + ' = @carId 
	ORDER BY '+ @attExpireDate +' ASC
	END
	'
	SET @sqlQuery2 ='
	IF EXISTS(SELECT TOP 1 * FROM ' + @vehicleDocumentsTableId +'
	WHERE ' + @attVehicleType +' = ' + @MotValueId + ' AND (' + @attStartDate + ' IS NULL OR @expenseItemDate BETWEEN ' + @attStartDate + ' AND ' + @attExpireDate +') AND ' + @attVehicle + ' = @carId  AND ' + @attStatus +' = ' + @StatusValueId +'
	ORDER BY ' + @attStatus +' DESC ,' + @attExpireDate +' DESC)
	BEGIN
	SELECT TOP 1 @Motexpiry=' + @attExpireDate +'
		  ,@motReviewed = case when ' + @attStatus +' = ' + @StatusValueId + ' THEN 1 ELSE 0 END 
	FROM ' + @vehicleDocumentsTableId +'
	WHERE ' + @attVehicleType +' = ' + @MotValueId + ' AND (' + @attStartDate + '  IS NULL OR @expenseItemDate BETWEEN ' + @attStartDate + ' AND ' + @attExpireDate +' OR @expenseItemDate > ' + @attExpireDate +') AND ' + @attVehicle + ' = @carId
	ORDER BY ' + @attStatus +' DESC ,' + @attExpireDate +' DESC
	END
	ELSE
	BEGIN
	SELECT @Motexpiry=' + @attExpireDate +'
		  ,@motReviewed = case when ' + @attStatus +' = ' + @StatusValueId + ' THEN 1 ELSE 0 END 
	FROM ' + @vehicleDocumentsTableId +'
	WHERE ' + @attVehicleType +' = ' + @MotValueId + ' AND (' + @attStartDate + ' IS NULL OR @expenseItemDate BETWEEN ' + @attStartDate + ' AND ' + @attExpireDate +' OR @expenseItemDate > ' + @attExpireDate +') AND ' + @attVehicle + ' = @carId 
	ORDER BY '+ @attExpireDate +' ASC
	END

	IF EXISTS(SELECT TOP 1 * FROM ' + @vehicleDocumentsTableId +'
	WHERE ' + @attVehicleType +' = ' + @InsuranceValueId + ' AND (' + @attStartDate + ' IS NULL OR @expenseItemDate BETWEEN ' + @attStartDate + ' AND ' + @attExpireDate +') AND ' + @attVehicle + ' = @carId  AND ' + @attStatus +' = ' + @StatusValueId +'
	ORDER BY ' + @attStatus +' DESC ,' + @attExpireDate +' DESC)
	BEGIN
	SELECT TOP 1 @Insuranceexpiry=' + @attExpireDate +'
		  ,@insuranceReviewed = case when ' + @attStatus +' = ' + @StatusValueId + ' THEN 1 ELSE 0 END 
	FROM ' + @vehicleDocumentsTableId +'
	WHERE ' + @attVehicleType +' = ' + @InsuranceValueId + ' AND (' + @attStartDate + ' IS NULL OR @expenseItemDate BETWEEN ' + @attStartDate + ' AND ' + @attExpireDate +' OR @expenseItemDate > ' + @attExpireDate +') AND ' + @attVehicle + ' = @carId
	ORDER BY ' + @attStatus +' DESC , ' + @attExpireDate +' DESC
	END
	ELSE
	BEGIN
	SELECT  @Insuranceexpiry=' + @attExpireDate +'
		  ,@insuranceReviewed = case when ' + @attStatus +' = ' + @StatusValueId + ' THEN 1 ELSE 0 END 
	FROM ' + @vehicleDocumentsTableId +'
	WHERE ' + @attVehicleType +' = ' + @InsuranceValueId + ' AND (' + @attStartDate + ' IS NULL OR @expenseItemDate BETWEEN ' + @attStartDate + ' AND ' + @attExpireDate +' OR @expenseItemDate > ' + @attExpireDate +') AND ' + @attVehicle + ' = @carId 
	ORDER BY ' + @attExpireDate +' ASC
	END

	IF EXISTS(SELECT TOP 1 * FROM ' + @vehicleDocumentsTableId +'
	WHERE ' + @attVehicleType +' = ' + @BreakdownValueId + ' AND (' + @attStartDate + ' IS NULL OR @expenseItemDate BETWEEN ' + @attStartDate + ' AND ' + @attExpireDate +') AND ' + @attVehicle + ' = @carId  AND ' + @attStatus +' = ' + @StatusValueId +'
	ORDER BY ' + @attStatus +' DESC ,' + @attExpireDate +' DESC)
	BEGIN
	SELECT TOP 1 @BreakdownCoverexpiry=' + @attExpireDate +'
		  ,@breakdowncoverReviewed = case when ' + @attStatus +' = ' + @StatusValueId + ' THEN 1 ELSE 0 END 
	FROM ' + @vehicleDocumentsTableId +'
	WHERE ' + @attVehicleType +' = ' + @BreakdownValueId + ' AND (' + @attStartDate + ' IS NULL OR @expenseItemDate BETWEEN ' + @attStartDate + ' AND ' + @attExpireDate +' OR @expenseItemDate >' + @attExpireDate +') AND ' + @attVehicle + ' = @carId
	ORDER BY ' + @attStatus +' DESC ,' + @attExpireDate +' DESC
	END
	ELSE
	BEGIN
	SELECT  @BreakdownCoverexpiry=' + @attExpireDate +'
		  ,@breakdowncoverReviewed = case when ' + @attStatus +' = ' + @StatusValueId + ' THEN 1 ELSE 0 END 
	FROM ' + @vehicleDocumentsTableId +'
	WHERE ' + @attVehicleType +' = ' + @BreakdownValueId + ' AND (' + @attStartDate + ' IS NULL OR @expenseItemDate BETWEEN ' + @attStartDate + ' AND ' + @attExpireDate +' OR @expenseItemDate >' + @attExpireDate +') AND ' + @attVehicle + ' = @carId 
	ORDER BY '+@attExpireDate +' ASC
	END


	SELECT @blockMOT = stringValue FROM accountProperties WHERE stringKey = ''blockMOTExpiry''
	SELECT @blockTax = stringValue FROM accountProperties WHERE stringKey = ''blockTaxExpiry''
	SELECT @blockInsurance = stringValue FROM accountProperties WHERE stringKey = ''blockInsuranceExpiry''
	SELECT @blockBreakdownCover = stringValue FROM accountProperties WHERE stringKey = ''blockBreakdownCoverExpiry''

	DECLARE @registration NVARCHAR(100)
	SELECT @registration = registration FROM cars WHERE carid = @carId

	SELECT @registration AS [Registration], @Taxexpiry AS [TaxDocumentExpiryDate], @Motexpiry AS [MotDocumentExpiryDate], @Insuranceexpiry AS [InsuranceDocumentExpiryDate], @blockMOT AS BlockMOT, @blockTax AS BlockTax, @blockInsurance AS BlockInsurance, @taxReviewed AS TaxReviewed, @motReviewed AS MOTReviewed, @insuranceReviewed AS InsuranceReviewed, @blockBreakdownCover AS BlockBreakdownCover, @BreakdownCoverexpiry AS BreakdownCoverDocumentExpiryDate, @breakdowncoverReviewed AS BreakdownCoverReviewed
END';

SET @sqlQuery3 = @sqlQuery1 + @sqlQuery2
EXEC sp_executesql @sqlQuery3

END