CREATE PROCEDURE [dbo].[GetCarDocumentsExpiryInformation]
@carId INT,
@expenseItemDate DATETIME
AS
BEGIN
SET NOCOUNT ON;

     Declare @vehicleDocumentsTableId varchar(20) =[dbo].[GetEntityId] ('F0247D8E-FAD3-462D-A19D-C9F793F984E8',1)  --c218
     Declare @attVehicleType varchar(20) =[dbo].[GetAttributeId] ('F190DA6F-DC4E-4B54-92C2-1EB68451EFC9',1)  ---att2073
	 Declare @attVehicleTypeValue varchar(20) =[dbo].[GetAttributeId] ('F190DA6F-DC4E-4B54-92C2-1EB68451EFC9',0)  ---2073
	 Declare @attVehicle varchar(20) =[dbo].[GetAttributeId] ('1E5F7E5E-FF90-45E2-A7E5-FF1799668E5F',1)  ---attt2074
	 Declare @attStartDate varchar(20) =[dbo].[GetAttributeId] ('AE8D3951-54EA-40DA-801D-E27B7239C338',1)  ---att2070
	 Declare @attExpireDate varchar(20) =[dbo].[GetAttributeId] ('9E46EB4E-00E8-475D-A89B-1643B139C490',1)  ---att2055
     Declare @attStatus varchar(20) =[dbo].[GetAttributeId] ('19EC2F1F-0EF4-465B-B6EE-D2CE2FA7021B',1)  ---att2071
	 Declare @attStatusValue varchar(20) =[dbo].[GetAttributeId] ('19EC2F1F-0EF4-465B-B6EE-D2CE2FA7021B',0)  ---2071
     DECLARE @sqlQuery NVARCHAR(MAX); 

SET @sqlQuery ='

  DECLARE @Taxexpiry DATETIME
  DECLARE @Motexpiry DATETIME
  DECLARE @Insuranceexpiry DATETIME
  DECLARE @BreakdownCoverexpiry DATETIME
  DECLARE @employeeid INT
  DECLARE @blockMOT BIT
  DECLARE @blockTax BIT
  DECLARE @blockInsurance BIT
  DECLARE @blockBreakdownCover BIT
  DECLARE @taxReviewed BIT = 1
  DECLARE @motReviewed BIT = 1
  DECLARE @insuranceReviewed BIT = 1
  DECLARE @breakdowncoverReviewed BIT = 1
  DECLARE @status INT

SELECT @status=valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attStatusValue+' AND item = ''Reviewed-OK''
DECLARE @registration NVARCHAR(100)
SELECT @registration=registration FROM cars WHERE carid = '+convert(varchar, @carId)+'
SELECT @Taxexpiry='+@attExpireDate +'  FROM '+@vehicleDocumentsTableId +' WHERE '+@attVehicleType +'  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +'  AND item = ''Tax'') AND '+@attStatus +'  =@status AND ('+@attStartDate +'  IS NULL OR '+''''+convert (varchar,@expenseItemDate)+''''+' BETWEEN '+@attStartDate +'   AND '+@attExpireDate +' OR '+''''+convert (varchar,@expenseItemDate)+''''+' >'+@attExpireDate +') AND '+@attVehicle +'  ='+convert(varchar, @carId)+' ORDER BY '+@attExpireDate +' ASC
SELECT @Motexpiry='+@attExpireDate +' FROM '+@vehicleDocumentsTableId +' WHERE '+@attVehicleType +'  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +'  AND item = ''MOT'') AND '+@attStatus +'  =@status AND ('+@attStartDate +'  IS NULL OR '+''''+convert (varchar,@expenseItemDate)+''''+' BETWEEN '+@attStartDate +'   AND '+@attExpireDate +' OR '+''''+convert (varchar,@expenseItemDate)+''''+' >'+@attExpireDate +') AND '+@attVehicle +'  ='+convert(varchar, @carId)+' ORDER BY '+@attExpireDate +' ASC
SELECT @Insuranceexpiry='+@attExpireDate +' FROM '+@vehicleDocumentsTableId +' WHERE '+@attVehicleType +'  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +'  AND item = ''Insurance'') AND '+@attStatus +'  =@status AND ('+@attStartDate +'  IS NULL OR '+''''+convert (varchar,@expenseItemDate)+''''+' BETWEEN '+@attStartDate +'   AND '+@attExpireDate +' OR '+''''+convert (varchar,@expenseItemDate)+''''+' >'+@attExpireDate +') AND '+@attVehicle +'  ='+convert(varchar, @carId)+' ORDER BY '+@attExpireDate +' ASC
SELECT @BreakdownCoverexpiry='+@attExpireDate +' FROM '+@vehicleDocumentsTableId +' WHERE '+@attVehicleType +'  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +'  AND item = ''Breakdown Cover'') AND '+@attStatus +'  =@status AND ('+@attStartDate +'  IS NULL OR '+''''+convert (varchar,@expenseItemDate)+''''+' BETWEEN '+@attStartDate +'   AND '+@attExpireDate +' OR '+''''+convert (varchar,@expenseItemDate)+''''+' >'+@attExpireDate +') AND '+@attVehicle +'  ='+convert(varchar, @carId)+' ORDER BY '+@attExpireDate +' ASC
SELECT @blockMOT = stringValue FROM accountProperties WHERE stringKey = ''blockMOTExpiry''
SELECT @blockTax = stringValue FROM accountProperties WHERE stringKey = ''blockTaxExpiry''
SELECT @blockInsurance = stringValue FROM accountProperties WHERE stringKey = ''blockInsuranceExpiry''
SELECT @blockBreakdownCover = stringValue FROM accountProperties WHERE stringKey = ''blockBreakdownCoverExpiry''

 -- Expiry dates will be null if there is no document that has been approved. Hence, we then get the latest document that has been uploaded to get the expiry date (which in this case will be the expiry date of the document that has been uploaded but hasn''t been approved.)

   IF @Taxexpiry is null
   BEGIN
	SELECT @Taxexpiry='+@attExpireDate +' FROM '+@vehicleDocumentsTableId +' WHERE '+@attVehicleType +'  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +'  AND item = ''Tax'') AND ('+@attStartDate +'  IS NULL OR '+''''+convert (varchar,@expenseItemDate)+''''+' BETWEEN '+@attStartDate +'   AND '+@attExpireDate +' ) AND '+@attVehicle +'  ='+convert(varchar, @carId)+' ORDER BY '+@attExpireDate +' ASC
   SET @taxReviewed = 0
   END
   
   IF @Motexpiry is null
   BEGIN
	SELECT @Motexpiry='+@attExpireDate +' FROM '+@vehicleDocumentsTableId +' WHERE '+@attVehicleType +'  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +'  AND item = ''MOT'') AND ('+@attStartDate +'  IS NULL OR '+''''+convert (varchar,@expenseItemDate)+''''+' BETWEEN '+@attStartDate +'   AND '+@attExpireDate +') AND '+@attVehicle +'  ='+convert(varchar, @carId)+' ORDER BY '+@attExpireDate +' ASC
   SET @motReviewed = 0
   END

   IF @Insuranceexpiry is null
   BEGIN
	SELECT @Insuranceexpiry='+@attExpireDate +' FROM '+@vehicleDocumentsTableId +' WHERE '+@attVehicleType +'  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +'  AND item = ''Insurance'') AND ('+@attStartDate +'  IS NULL OR '+''''+convert (varchar,@expenseItemDate)+''''+' BETWEEN '+@attStartDate +'   AND '+@attExpireDate +') AND '+@attVehicle +'  ='+convert(varchar, @carId)+' ORDER BY '+@attExpireDate +' ASC
   SET @insuranceReviewed = 0
   END

   IF @BreakdownCoverexpiry is null
   BEGIN
	SELECT @BreakdownCoverexpiry='+@attExpireDate +' FROM '+@vehicleDocumentsTableId +' WHERE '+@attVehicleType +'  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +'  AND item = ''Breakdown Cover'') AND ('+@attStartDate +'  IS NULL OR '+''''+convert (varchar,@expenseItemDate)+''''+' BETWEEN '+@attStartDate +'   AND '+@attExpireDate +') AND '+@attVehicle +'  ='+convert(varchar, @carId)+' ORDER BY '+@attExpireDate +' ASC
   SET @breakdowncoverReviewed = 0
   END

SELECT @registration AS [Registration],@Taxexpiry AS [TaxDocumentExpiryDate],@Motexpiry AS [MotDocumentExpiryDate],@Insuranceexpiry AS [InsuranceDocumentExpiryDate], @blockMOT AS BlockMOT, @blockTax AS BlockTax, @blockInsurance AS BlockInsurance, @taxReviewed AS TaxReviewed, @motReviewed AS MOTReviewed, @insuranceReviewed AS InsuranceReviewed, @blockBreakdownCover AS BlockBreakdownCover, @BreakdownCoverexpiry AS BreakdownCoverDocumentExpiryDate, @breakdowncoverReviewed AS BreakdownCoverReviewed'

exec (@sqlQuery)

END