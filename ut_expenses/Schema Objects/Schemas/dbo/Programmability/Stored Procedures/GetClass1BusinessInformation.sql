
CREATE PROCEDURE [dbo].[GetClass1BusinessInformation]
@carId INT
AS
BEGIN
SET NOCOUNT ON;

     DECLARE @vehicleDocumentsTableId VARCHAR(20) =[dbo].[GetEntityId] ('F0247D8E-FAD3-462D-A19D-C9F793F984E8',1)  --c218
     DECLARE @attIsClass1BusinessIncluded VARCHAR(20) =[dbo].[GetAttributeId] ('8FA37405-00EE-4A6B-83B5-C99E97F8F10F',1)  ---att2057
	 DECLARE @attVehicle VARCHAR(20) =[dbo].[GetAttributeId] ('1E5F7E5E-FF90-45E2-A7E5-FF1799668E5F',1)  ---2074
	 DECLARE @attExpireDate VARCHAR(20) =[dbo].[GetAttributeId] ('9E46EB4E-00E8-475D-A89B-1643B139C490',1)  ---2055
     DECLARE @attStatus VARCHAR(20) =[dbo].[GetAttributeId] ('19EC2F1F-0EF4-465B-B6EE-D2CE2FA7021B',1)  ---att2071
	 DECLARE @attStatusValue VARCHAR(20) =[dbo].[GetAttributeId] ('19EC2F1F-0EF4-465B-B6EE-D2CE2FA7021B',0)  ---2071
     DECLARE @attVehicleType VARCHAR(20) =[dbo].[GetAttributeId] ('F190DA6F-DC4E-4B54-92C2-1EB68451EFC9',1)  ---att2073
	 DECLARE @attVehicleTypeValue VARCHAR(20) =[dbo].[GetAttributeId] ('F190DA6F-DC4E-4B54-92C2-1EB68451EFC9',0)  ---2073
	 DECLARE @sqlQuery NVARCHAR(MAX); 

SET @sqlQuery ='
    DECLARE @blockInsurance BIT
    SELECT @blockInsurance = stringValue FROM accountProperties WHERE stringKey = ''blockInsuranceExpiry''
    SELECT DISTINCT registration,vehicle.'+@attIsClass1BusinessIncluded +'    FROM cars c 
    INNER JOIN '+@vehicleDocumentsTableId +' vehicle ON c.carid = vehicle.'+@attVehicle +'  
    WHERE c.carid = '+convert(VARCHAR, @carId)+' 
    AND vehicle.'+@attExpireDate +'   >=GETUTCDATE() 
    AND vehicle.'+@attIsClass1BusinessIncluded +'  = 0 
    AND '+@attStatus+'  = (SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attStatusValue+' AND item = ''Reviewed-OK'') 
    AND vehicle.'+@attVehicleType +'  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +'  AND item =''Insurance'') 
	AND @blockinsurance =1'

EXEC (@sqlQuery)
END