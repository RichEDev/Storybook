create PROCEDURE [dbo].[SaveEmployeeConsentPortalAccessInformation]
   @employeeId INT,
    @driverId INT,
    @consentDate DATETIME,
    @securityCode uniqueidentifier ,
    @firstname NVARCHAR(150),
    @surname NVARCHAR(150),
    @dateofbirth DATETIME,
    @sex INT,
	@licenceNumber NVARCHAR(150),
	@email NVARCHAR(100),
	@middleName NVARCHAR(100),
	@responseCode NVARCHAR(100)
AS
BEGIN

DECLARE    @username NVARCHAR(50)


SELECT @username= username FROM employees     
        WHERE employeeid = @employeeid;

UPDATE employees
        SET DriverId = @driverId
            ,SecurityCode = @securityCode
            ,DVLAConsentDate = @consentDate        
            ,drivinglicence_firstname=dbo.getEncryptedValue(@firstname)
            ,drivinglicence_surname=dbo.getEncryptedValue(@surname)
            ,drivinglicence_dateofbirth=dbo.getEncryptedValue(@dateofbirth)
            ,drivinglicence_sex=dbo.getEncryptedValue(@sex)
			,drivinglicence_Email = dbo.getEncryptedValue(@email)
			,drivinglicence_licenceNumber = dbo.getEncryptedValue(@licencenumber)
			,drivinglicence_middlename = dbo.getEncryptedValue(@middleName)
			,DvlaResponseCode=@responseCode
			,AgreeToProvideConsent=1
        WHERE employeeid = @employeeid;
        Exec addUpdateEntryToAuditLog @employeeId,'',197,@employeeId,null,'','Agreed','Check consent',null
		            
        END