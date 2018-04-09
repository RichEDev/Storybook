CREATE PROCEDURE [dbo].[GetExpiredDrivingLicenceReview]
AS 
BEGIN
Declare @vehicleDocumentsTableId varchar(20) =[dbo].[GetEntityId] ('5137C32E-E500-4297-BFF5-69CFED26C9B6',1)  --vehicle
Declare @attStatus varchar(20) =[dbo].[GetAttributeId] ('C790EC93-A920-4CFC-8846-605F8B4B50B5',1) 
Declare @attStatusValue varchar(20) =[dbo].[GetAttributeId] ('C790EC93-A920-4CFC-8846-605F8B4B50B5',0)   
Declare @attReviewDate varchar(20) =[dbo].[GetAttributeId] ('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE',1)  

DECLARE @sqlQuery NVARCHAR(MAX);

declare @reviewFrequency int
declare @enableDrvingLicence int
DECLARE @checkDrivingLicence int
declare @reminderEnabled int
DECLARE @reminderFrequency int

IF EXISTS(SELECT stringkey FROM accountProperties where stringKey = 'DrivingLicenceReviewPeriodically')
SET @enableDrvingLicence = (SELECT stringValue from accountProperties WHERE stringKey ='DrivingLicenceReviewPeriodically')
IF EXISTS(SELECT stringkey FROM accountProperties where stringKey = 'DrivingLicenceReviewFrequency')
SET @reviewFrequency = (SELECT stringValue from accountProperties WHERE stringKey ='DrivingLicenceReviewFrequency')
IF EXISTS(SELECT stringkey FROM accountProperties where stringKey = 'DrivingLicenceReviewReminder')
SET @reminderEnabled = (SELECT stringValue from accountProperties WHERE stringKey ='DrivingLicenceReviewReminder')
IF EXISTS(SELECT stringkey FROM accountProperties where stringKey = 'DrivingLicenceReviewReminderDays')
SET @reminderFrequency = 0 --Default off. If reminderEnabled, gets set to value
IF EXISTS(SELECT stringkey FROM accountProperties where stringKey = 'blockDrivingLicence')
SET @checkDrivingLicence = (SELECT stringValue from accountProperties WHERE stringKey ='blockDrivingLicence')

IF (@enableDrvingLicence != 0 AND @reminderEnabled != 0 AND @checkDrivingLicence !=0) 
BEGIN
SET @reminderFrequency = (SELECT stringValue from accountProperties WHERE stringKey ='DrivingLicenceReviewReminderDays')
DECLARE @currentDate DATETIME = GETDATE();
DECLARE @expiryDate datetime = (SELECT DATEADD(MONTH, -@reviewFrequency, @currentDate));
DECLARE @reminderDate datetime =  (SELECT DATEADD(DAY, @reminderFrequency, @expiryDate));

CREATE TABLE #LicenceDetails(EmployeeId INT, ReviewDate DATETIME)

SET @sqlQuery = 'insert into #LicenceDetails SELECT DISTINCT CREATEDBY as EmployeeId, cast('+@attReviewDate+' as date) as ReviewDate FROM '+@vehicleDocumentsTableId+' Where  cast(' + @attReviewDate + '   as Date)  = ' + '''' + convert(VARCHAR, @reminderDate) + '''' + '  and
'+@attStatus+' =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = ' + @attStatusValue + ' AND item = ''Reviewed - OK'') order by ReviewDate desc'
	EXEC (@sqlQuery)
	SELECT * FROM #LicenceDetails

	DROP TABLE #LicenceDetails
END
END
GO