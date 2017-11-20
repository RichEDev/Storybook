CREATE PROCEDURE [dbo].[Getexpireddrivinglicencereview] 
AS 
  BEGIN 
      DECLARE @vehicleDocumentsTableId VARCHAR(20) = 
              [dbo].[Getentityid] ('5137C32E-E500-4297-BFF5-69CFED26C9B6', 1) 
      --vehicle 
      DECLARE @attStatus VARCHAR(20) =[dbo].[Getattributeid] ( 
                                      'C790EC93-A920-4CFC-8846-605F8B4B50B5', 1) 
      DECLARE @attStatusValue VARCHAR(20) = 
              [dbo].[Getattributeid] ('C790EC93-A920-4CFC-8846-605F8B4B50B5', 0) 
      DECLARE @attReviewDate VARCHAR(20) = 
              [dbo].[Getattributeid] ('80DD4B17-3DB9-49DE-B9A4-8F20B5B8B5BE', 1) 
      DECLARE @sqlQuery NVARCHAR(max); 
      DECLARE @reviewFrequency INT 
      DECLARE @enableDrvingLicence INT 
      DECLARE @checkDrivingLicence INT 
      DECLARE @reminderFrequency INT 

      IF EXISTS(SELECT * 
                FROM   accountproperties 
                WHERE  stringkey = 'DrivingLicenceReviewPeriodically') 
        SET @enableDrvingLicence = (SELECT stringvalue 
                                    FROM   accountproperties 
                                    WHERE 
        stringkey = 'DrivingLicenceReviewPeriodically') 

      IF EXISTS(SELECT * 
                FROM   accountproperties 
                WHERE  stringkey = 'DrivingLicenceReviewFrequency') 
        SET @reviewFrequency = (SELECT stringvalue 
                                FROM   accountproperties 
                                WHERE 
        stringkey = 'DrivingLicenceReviewFrequency' 
                               ) 

      IF EXISTS(SELECT *
                FROM   accountproperties 
                WHERE  stringkey = 'DrivingLicenceReviewReminder') 
        SET @reminderFrequency = (SELECT stringvalue 
                                  FROM   accountproperties 
                                  WHERE 
        stringkey = 'DrivingLicenceReviewReminder') 

      IF EXISTS(SELECT *
                FROM   accountproperties 
                WHERE  stringkey = 'blockDrivingLicence') 
        SET @checkDrivingLicence = (SELECT stringvalue 
                                    FROM   accountproperties 
                                    WHERE  stringkey = 'blockDrivingLicence') 

      IF ( @enableDrvingLicence != 0 
           AND @reviewFrequency > 0 
           AND @checkDrivingLicence != 0 
           AND @reminderFrequency != 0 ) 
        BEGIN 
            SET @reminderFrequency = 7; 

            DECLARE @currentDate DATETIME = Getdate(); 
            DECLARE @expiryDate DATETIME = (SELECT 
                    Dateadd(month, -@reviewFrequency, 
                    @currentDate)); 
            DECLARE @reminderDate DATETIME = (SELECT 
                    Dateadd(day, @reminderFrequency, 
                    @expiryDate)); 

            CREATE TABLE #licencedetails 
              ( 
                 employeeid INT, 
                 reviewdate DATETIME 
              ) 

            SET @sqlQuery = 
'insert into #LicenceDetails SELECT DISTINCT CREATEDBY as EmployeeId, cast(' 
+ @attReviewDate 
+ ' as date) as ReviewDate FROM ' 
+ @vehicleDocumentsTableId + ' Where  cast(' 
+ @attReviewDate + '   as Date)  = ' + '''' 
+ CONVERT(VARCHAR, @reminderDate) + '''' + '  and ' 
+ @attStatus 
+ 
' =(SELECT valueid FROM customEntityAttributeListItems WHERE attributeid = ' 
                + @attStatusValue 
                + ' AND item = ''Reviewed - OK'') order by ReviewDate desc' 

EXEC (@sqlQuery) 

SELECT * 
FROM   #licencedetails 

DROP TABLE #licencedetails 
END 
END 