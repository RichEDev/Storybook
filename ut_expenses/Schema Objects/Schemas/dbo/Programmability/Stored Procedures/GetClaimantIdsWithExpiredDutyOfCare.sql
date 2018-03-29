CREATE PROCEDURE [dbo].[GetClaimantIdsWithExpiredDutyOfCare] 
AS
BEGIN

 
     Declare @vehicleDocumentsTableId varchar(20) =[dbo].[GetEntityId] ('F0247D8E-FAD3-462D-A19D-C9F793F984E8',1)  --vehicle
     Declare @attVehicleType varchar(20) =[dbo].[GetAttributeId] ('F190DA6F-DC4E-4B54-92C2-1EB68451EFC9',1)  ---att2073
	 Declare @attVehicleTypeValue varchar(20) =[dbo].[GetAttributeId] ('F190DA6F-DC4E-4B54-92C2-1EB68451EFC9',0)  ---2073
	 Declare @attVehicle varchar(20) =[dbo].[GetAttributeId] ('1E5F7E5E-FF90-45E2-A7E5-FF1799668E5F',1)  ---att2074
	 Declare @attExpireDate varchar(20) =[dbo].[GetAttributeId] ('9E46EB4E-00E8-475D-A89B-1643B139C490',1)  ---att2055
	 Declare @attDrivingExpireDate varchar(20) =[dbo].[GetAttributeId] ('292D65BC-75E5-4214-9AF3-CC37661A566B',1)  ---att1566
	 
     Declare @drivingLicenceTableId varchar(20) =[dbo].[GetEntityId] ('223018FE-EDAE-408E-8851-C09ABA09DF81',1)  --c145
	 Declare @attEmployee varchar(20) =[dbo].[GetAttributeId] ('91596ACE-E16C-4DB3-9E02-776785EB11CF',1)  ---att1595
	 DECLARE @attLicenceDvla varchar(50);	
	 Declare @attLicenceTypeId varchar(20) =[dbo].[GetAttributeId] ('CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE',0);
	 Declare @attLicenceType varchar(20) =[dbo].[GetAttributeId] ('CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE',1);
	 SELECT @attLicenceDvla = cast(valueid as nvarchar(10)) from customEntityAttributeListItems where attributeid = +@attLicenceTypeId  and item = 'Driving Licence (Automatic DVLA Lookup)';

	 DECLARE @sqlQuery NVARCHAR(MAX); 


    declare @reminderdays int
    if exists (select * from accountProperties
    where stringKey = 'dutyOfCareEmailReminderForClaimantDays')
    SET @reminderdays = (SELECT stringValue FROM accountProperties
    WHERE stringKey = 'dutyOfCareEmailReminderForClaimantDays')

    if @reminderdays != -1
     BEGIN
        declare @today datetime =cast(getDate() as Date);
        declare @expirydate datetime = (SELECT DATEADD(day, @reminderdays, @today));
            create table #temp(EmployeeId int)

			
SET @sqlQuery ='

        if ((select stringValue from accountProperties where stringKey = ''blockTaxExpiry'') = 1)
        insert into #temp
        select c.employeeid as EmployeeId from '+@vehicleDocumentsTableId+' vehicle
        inner join cars c on c.carid = vehicle.'+@attVehicle +'  
        where cast('+@attExpireDate +'   as Date) = '+''''+convert (varchar,@expirydate)+''''+' and vehicle.'+@attVehicleType+'  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +'   AND item = ''Tax'')

        if ((select stringValue from accountProperties where stringKey = ''blockMOTExpiry'') = 1)
        insert into #temp
        select c.employeeid as EmployeeId from '+@vehicleDocumentsTableId+' vehicle
        inner join cars c on c.carid = vehicle.'+@attVehicle +'  
        where cast('+@attExpireDate +'  as Date) = '+''''+convert (varchar,@expirydate)+''''+' and vehicle.'+@attVehicleType+'  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +'   AND item = ''MOT'')

        insert into #temp
        select c.employeeid as EmployeeId from '+@vehicleDocumentsTableId+' vehicle
        inner join cars c on c.carid = vehicle.'+@attVehicle +'  
        where cast('+@attExpireDate +'  as Date) = '+''''+convert (varchar,@expirydate)+''''+' and vehicle.'+@attVehicleType+'  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +'   AND item = ''Service'')

        if ((select stringValue from accountProperties where stringKey = ''blockInsuranceExpiry'') = 1)
        insert into #temp
        select c.employeeid as EmployeeId from '+@vehicleDocumentsTableId+' vehicle
        inner join cars c on c.carid = vehicle.'+@attVehicle +'  
        where cast('+@attExpireDate +'  as Date) = '+''''+convert (varchar,@expirydate)+''''+' and vehicle.'+@attVehicleType+'  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +'   AND item = ''Insurance'')

        IF ((SELECT stringValue FROM accountProperties WHERE stringKey = ''blockBreakdownCoverExpiry'') = 1)
        INSERT INTO #temp
        SELECT c.employeeid AS EmployeeId FROM '+@vehicleDocumentsTableId+' vehicle
        INNER JOIN cars c ON c.carid = vehicle.'+@attVehicle +'  
        WHERE CAST('+@attExpireDate +'  AS Date) = '+''''+convert (varchar,@expirydate)+''''+' AND vehicle.'+@attVehicleType+' = (SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = '+@attVehicleTypeValue +' AND item = ''Breakdown Cover'')


		
        if ((select stringValue from accountProperties where stringKey = ''blockDrivingLicence'') = 1)		
			 BEGIN		
				insert into #temp
				select '+@attEmployee +' as EmployeeId from '+@drivingLicenceTableId +' 
				where cast('+@attDrivingExpireDate +' as Date) = '+''''+convert (varchar,@expirydate)+''''+' and ' + @attLicenceType + ' IN((select valueid from customEntityAttributeListItems where attributeid = ' + @attLicenceTypeId + ' and item = ''Photocard''),(select valueid from customEntityAttributeListItems where attributeid = ' + @attLicenceTypeId + ' and item = ''Driving Licence (Automatic DVLA Lookup)'')) 
		END'
		    
				
exec (@sqlQuery)  

        SELECT #temp.EmployeeId FROM #temp 
		INNER JOIN employees ON employees.employeeid = #temp.EmployeeId
		WHERE Employees.archived = 0
		GROUP BY #temp.EmployeeId

        Drop Table #temp

        END
    ELSE
        RETURN 0
END
