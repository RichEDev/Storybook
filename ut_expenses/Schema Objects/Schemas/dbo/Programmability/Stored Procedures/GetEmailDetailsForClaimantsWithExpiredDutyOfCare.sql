
CREATE PROCEDURE [dbo].[GetEmailDetailsForClaimantsWithExpiredDutyOfCare] @employeeid INT
AS
BEGIN
	DECLARE @vehicleDocumentsTableId VARCHAR(20) = [dbo].[GetEntityId]('F0247D8E-FAD3-462D-A19D-C9F793F984E8', 1) --vehicle
	DECLARE @attVehicleType VARCHAR(20) = [dbo].[GetAttributeId]('F190DA6F-DC4E-4B54-92C2-1EB68451EFC9', 1) ---att2073
	DECLARE @attVehicleTypeValue VARCHAR(20) = [dbo].[GetAttributeId]('F190DA6F-DC4E-4B54-92C2-1EB68451EFC9', 0) ---2073
	DECLARE @attVehicle VARCHAR(20) = [dbo].[GetAttributeId]('1E5F7E5E-FF90-45E2-A7E5-FF1799668E5F', 1) ---att2074
	DECLARE @attExpireDate VARCHAR(20) = [dbo].[GetAttributeId]('9E46EB4E-00E8-475D-A89B-1643B139C490', 1) ---att2055
	DECLARE @attDrivingExpireDate VARCHAR(20) = [dbo].[GetAttributeId]('292D65BC-75E5-4214-9AF3-CC37661A566B', 1) ---att1566
	DECLARE @drivingLicenceTableId VARCHAR(20) = [dbo].[GetEntityId]('223018FE-EDAE-408E-8851-C09ABA09DF81', 1) --c145
	DECLARE @attEmployee VARCHAR(20) = [dbo].[GetAttributeId]('91596ACE-E16C-4DB3-9E02-776785EB11CF', 1) ---att1595
	DECLARE @attLicenceType VARCHAR(20) = [dbo].[GetAttributeId]('CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE', 1) ---att1568
	DECLARE @attLicenceTypeValue VARCHAR(20) = [dbo].[GetAttributeId]('CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE', 0) ---1568
	DECLARE @attLicenceNumber VARCHAR(20) = [dbo].[GetAttributeId]('F8123BD2-0E21-43C3-9325-BE3AF17A95DB', 1) ---att1563
	DECLARE @sqlQuery NVARCHAR(MAX);
	DECLARE @reminderdays INT
	DECLARE @documentname NVARCHAR(Max)
	DECLARE @attLicenceDvla varchar(50);
		Declare @attLicenceTypeId varchar(20) =[dbo].[GetAttributeId] ('CD3164BF-5C67-47F9-AF02-2FA6BB6F0BCE',0);	
	 SELECT @attLicenceDvla = cast(valueid as nvarchar(10)) from customEntityAttributeListItems where attributeid = +@attLicenceTypeId  and item = 'Driving Licence (Automatic DVLA Check)';

	CREATE TABLE #temp (
		DocumentName NVARCHAR(MAX)
		,NumberOfDays INT
		,ExpiryDate DATETIME
		)

	IF EXISTS (
			SELECT *
			FROM accountProperties
			WHERE stringKey = 'dutyOfCareEmailReminderForClaimantDays'
			)
		SET @reminderdays = (
				SELECT stringValue
				FROM accountProperties
				WHERE stringKey = 'dutyOfCareEmailReminderForClaimantDays'
				)

	DECLARE @today DATETIME = cast(getDate() AS DATE);
	DECLARE @expirydate DATETIME = (
			SELECT DATEADD(day, @reminderdays, @today)
			);

	SET @sqlQuery = '


if ((select stringValue from accountProperties where stringKey = ''blockTaxExpiry'') = 1)
insert into #temp
SELECT ''Tax Document for '' + c.make ' + '+ '' '' +' + '  c.model ' + '+ '' '' +' + ' c.registration as DocumentName,' + convert(VARCHAR, @reminderdays) + ' as NumberOfDays,  cast(' + '''' + convert(VARCHAR, @expirydate) + '''' + ' as Date) as ExpiryDate FROM ' + @vehicleDocumentsTableId + ' vehicle
inner join cars c on c.carid = vehicle.' + @attVehicle + '  
where c.employeeid = ' + convert(VARCHAR, @employeeid) + ' and cast(vehicle.' + @attExpireDate + '   as Date) = ' + '''' + convert(VARCHAR, @expirydate) + '''' + ' and vehicle.' + @attVehicleType + '  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = ' + @attVehicleTypeValue + '   AND item = ''Tax'')

if ((select stringValue from accountProperties where stringKey = ''blockMOTExpiry'') = 1)
insert into #temp
SELECT ''MOT Document for '' + c.make ' + '+ '' '' +' + '  c.model ' + '+ '' '' +' + ' c.registration as DocumentName,' + 
		convert(VARCHAR, @reminderdays) + ' as NumberOfDays, cast(' + '''' + convert(VARCHAR, @expirydate) + '''' + ' as Date) as ExpiryDate  FROM ' + @vehicleDocumentsTableId + ' vehicle
inner join cars c on c.carid = vehicle.' + @attVehicle + '  
where c.employeeid = ' + convert(VARCHAR, @employeeid) + ' and cast(vehicle.' + @attExpireDate + '   as Date) = ' + '''' + convert(VARCHAR, @expirydate) + '''' + ' and vehicle.' + @attVehicleType + '  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = ' + @attVehicleTypeValue + '   AND item = ''MOT'')

insert into #temp
SELECT ''Service Document for '' + c.make ' + '+ '' '' +' + '  c.model ' + '+ '' '' +' + ' c.registration as DocumentName,' + convert(VARCHAR, @reminderdays) + ' as NumberOfDays,  cast(' + '''' + convert(VARCHAR, @expirydate) + '''' + ' as Date) as ExpiryDate FROM ' + @vehicleDocumentsTableId + ' vehicle
inner join cars c on c.carid = vehicle.' + @attVehicle + '  
where c.employeeid = ' + convert(VARCHAR, @employeeid) + ' and cast(vehicle.' + @attExpireDate + '   as Date) = ' + 
		'''' + convert(VARCHAR, @expirydate) + '''' + ' and vehicle.' + @attVehicleType + '  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = ' + @attVehicleTypeValue + '   AND item = ''Service'')

if ((select stringValue from accountProperties where stringKey = ''blockInsuranceExpiry'') = 1)
insert into #temp
SELECT ''Insurance Document for '' + c.make ' + '+ '' '' +' + '  c.model ' + '+ '' '' +' + ' c.registration as DocumentName,' + convert(VARCHAR, @reminderdays) + ' as NumberOfDays,  cast(' + '''' + convert(VARCHAR, @expirydate) + '''' + ' as Date) as ExpiryDate  FROM ' + @vehicleDocumentsTableId + ' vehicle
inner join cars c on c.carid = vehicle.' + @attVehicle + '  
where c.employeeid = ' + convert(VARCHAR, @employeeid) + ' and cast(vehicle.' + @attExpireDate + '   as Date) = ' + '''' + convert(VARCHAR, @expirydate) + '''' + ' and vehicle.' + @attVehicleType + '  =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = ' + @attVehicleTypeValue + 
		'   AND item = ''Insurance'')

IF ((SELECT stringValue FROM accountProperties WHERE stringKey = ''blockBreakdownCoverExpiry'') = 1)
INSERT INTO #temp
SELECT ''Breakdown Cover Document for '' + c.make ' + '+ '' '' +' + '  c.model ' + '+ '' '' +' + '  c.registration AS DocumentName,' + convert(VARCHAR, @reminderdays) + ' AS NumberOfDays,  cast(' + '''' + convert(VARCHAR, @expirydate) + '''' + ' AS Date) AS ExpiryDate  
FROM ' + @vehicleDocumentsTableId + ' vehicle
INNER JOIN cars c ON c.carid = vehicle.' + @attVehicle + '  
WHERE c.employeeid = ' + convert(VARCHAR, @employeeid) + ' AND cast(vehicle.' + @attExpireDate + '   as Date) = ' + '''' + convert(VARCHAR, @expirydate) + '''' + ' AND vehicle.' + @attVehicleType + '  = (SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = ' + @attVehicleTypeValue + ' AND item = ''Breakdown Cover'')

if ((select stringValue from accountProperties where stringKey = ''blockDrivingLicence'') = 1)		
		 BEGIN
		 insert into #temp
		  SELECT  ''Driving licence - '' + ' + 
		@attLicenceNumber + ' as DocumentName,' + convert(VARCHAR, @reminderdays) + ' as NumberOfDays,  cast(' + '''' + convert(VARCHAR, @expirydate) + '''' + ' as Date) as ExpiryDate  From ' + @drivingLicenceTableId + '
        where cast(' + @attDrivingExpireDate + ' as Date) = ' + '''' + convert(VARCHAR, @expirydate) + '''' + ' and ' + @attEmployee + ' = ' + convert(VARCHAR, @employeeid) + ' 
         and '+@attLicenceType +' = '+@attLicenceDvla+ '
		 UNION ALL
        SELECT  ''Photocard driving licence - '' + ' + 
		@attLicenceNumber + ' as DocumentName, ' + convert(VARCHAR, @reminderdays) + '  as NumberOfDays,  cast(' + '''' + convert(VARCHAR, @expirydate) + '''' + ' as Date) as ExpiryDate  From ' + @drivingLicenceTableId + '
        where cast(' + @attDrivingExpireDate + ' as Date) = ' + '''' + convert(VARCHAR, @expirydate) + '''' + ' and ' + @attEmployee + ' = ' + convert(VARCHAR, @employeeid) + ' and ' + @attLicenceType + ' = (select valueid from customEntityAttributeListItems where attributeid = ' + @attLicenceTypeValue + ' and item = ''DVLA Photocard Licence'')
		END
		'

	EXEC (@sqlQuery)

	SELECT *
	FROM #temp

	DROP TABLE #temp
END
