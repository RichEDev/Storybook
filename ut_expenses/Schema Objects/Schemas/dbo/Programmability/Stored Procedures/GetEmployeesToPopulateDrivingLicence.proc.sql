
CREATE proc [dbo].[GetEmployeesToPopulateDrivingLicence]
as 
begin
declare @lookupFrequency int
select @lookupFrequency = stringValue from accountProperties where stringKey ='drivingLicenceLookupFrequency'
declare @automaticDrivingLicenceLookup int
select @automaticDrivingLicenceLookup = stringValue from accountProperties where stringKey ='enableAutomaticDrivingLicenceLookup'


select [dbo].[getDecryptedValue](drivinglicence_firstname) as drivingLicenceFirstname,[dbo].[getDecryptedValue](drivinglicence_surname) as drivingLicenceSurname ,CONVERT(datetime,[dbo].[getDecryptedValue](drivinglicence_dateofbirth)) as drivingLicenceDateOfBirth ,[dbo].[getDecryptedValue](drivinglicence_sex) as drivingLicenceSex,DriverId,[dbo].[getDecryptedValue](drivinglicence_licenceNumber) as DrivingLicenceNumber,employeeid as employeeId 
,Dvlalookupdate as DvlaLookupDate
from employees 
where @automaticDrivingLicenceLookup=1 and (DVLAConsentDate is not null and DVLAConsentDate >= DATEADD(YEAR,-3 ,GETUTCDATE())) and ( DvlaLookUpDate is null or DvlaLookUpDate < DATEADD(month,-@lookupFrequency,getutcdate())) 
and drivinglicence_firstname is not NULL and drivinglicence_surname is not NULL and drivinglicence_dateofbirth is not NULL AND drivinglicence_sex is not NULL AND DriverId is not NULL AND drivinglicence_licenceNumber is not NULL
AND (AgreeToProvideConsent is NULL or AgreeToProvideConsent=1)
end