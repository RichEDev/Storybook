CREATE PROCEDURE [dbo].[GetEmployeesWithManualIntervention]  
AS   
BEGIN  
DECLARE @automaticDrivingLicenceLookup INT
SELECT @automaticDrivingLicenceLookup = stringValue FROM accountProperties WHERE stringKey ='enableAutomaticDrivingLicenceLookup'
  
SELECT [dbo].[getDecryptedValue](drivinglicence_firstname) AS drivingLicenceFirstname,[dbo].[getDecryptedValue](drivinglicence_surname) AS drivingLicenceSurname ,CONVERT(datetime,[dbo].[getDecryptedValue](drivinglicence_dateofbirth)) as drivingLicenceDateOfBirth ,
[dbo].[getDecryptedValue](drivinglicence_sex) AS drivingLicenceSex,DriverId,[dbo].[getDecryptedValue](drivinglicence_licenceNumber) AS DrivingLicenceNumber,employeeid AS employeeId FROM employees   
WHERE  @automaticDrivingLicenceLookup=1 and (DVLAConsentDate is not null and DVLAConsentDate >= DATEADD(YEAR,-3 ,GETUTCDATE()))  AND DvlaLookUpDate IS NOT NULL AND (AgreeToProvideConsent is NULL or AgreeToProvideConsent=1)  AND  (DvlaResponseCode='601' OR  DvlaResponseCode='602')
  
END