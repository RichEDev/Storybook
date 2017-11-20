
CREATE PROCEDURE [dbo].[GetCarsWithSornDeclaration]
@employeeId INT,
@expenseItemDate DATETIME
AS
BEGIN
SET NOCOUNT ON;
	 DECLARE @sqlQuery NVARCHAR(MAX); 
	 Declare @vehicleDocumentsTableId varchar(20) =[dbo].[GetEntityId] ('F0247D8E-FAD3-462D-A19D-C9F793F984E8',1)  --vehicle
	 Declare @attVehicle varchar(20) =[dbo].[GetAttributeId] ('1E5F7E5E-FF90-45E2-A7E5-FF1799668E5F',1)  ---attt2074
     Declare @attVehicleType varchar(20) =[dbo].[GetAttributeId] ('F190DA6F-DC4E-4B54-92C2-1EB68451EFC9',1)  ---att2073
	 Declare @attVehicleTypeValue varchar(20) =[dbo].[GetAttributeId] ('F190DA6F-DC4E-4B54-92C2-1EB68451EFC9',0)  ---2073
	 Declare @attStartDate varchar(20) =[dbo].[GetAttributeId] ('AE8D3951-54EA-40DA-801D-E27B7239C338',1)  ---attt2070
	 Declare @attExpireDate varchar(20) =[dbo].[GetAttributeId] ('9E46EB4E-00E8-475D-A89B-1643B139C490',1)  ---attt2055
     Declare @attSORN varchar(20) =[dbo].[GetAttributeId] ('AA693323-8197-420E-AE72-CB9049019CD8',1)  ---att2069
	 DECLARE @DisableCarOutsideOfStartEndDate BIT

SELECT @DisableCarOutsideOfStartEndDate=stringValue FROM accountProperties WHERE stringKey = 'DisableCarOutsideOfStartEndDate'
IF EXISTS(SELECT * from tables where tableid ='28D592D7-4596-49C4-96B8-45655D8DF797')
BEGIN

SET @sqlQuery ='
     SELECT CarId,registration FROM cars WHERE employeeid = '+convert(varchar, @employeeId)+'
	 AND ((active=1 AND '+convert(varchar,@DisableCarOutsideOfStartEndDate)+'=0) 
	 OR  ('+convert(varchar,@DisableCarOutsideOfStartEndDate)+'=1 AND (('+''''+convert (varchar,@expenseItemDate)+''''+' between startdate and enddate )
	 OR  (startdate IS NULL AND enddate IS NULL)
  	 OR (startdate IS NULL AND '+''''+convert (varchar,@expenseItemDate)+''''+' <enddate)
     OR (enddate IS NULL AND '+''''+convert (varchar,@expenseItemDate)+''''+' >= startdate))))
	 AND carid IN(select vehicle.'+@attVehicle +' FROM '+@vehicleDocumentsTableId+' vehicle 
     WHERE  (vehicle.'+@attSORN+'=1 
	 AND  ((('+''''+convert (varchar,@expenseItemDate)+''''+' BETWEEN '+@attStartDate +'  AND '+@attExpireDate +')) 	
	  AND vehicle.'+@attVehicleType+' IN (SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +'  AND item = ''Tax'' ))))'
  
exec (@sqlQuery)

END
END