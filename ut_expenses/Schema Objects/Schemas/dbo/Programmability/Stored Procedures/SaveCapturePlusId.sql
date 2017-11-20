CREATE PROCEDURE [dbo].[SaveCapturePlusId]
@company NVARCHAR(250),
@address1 NVARCHAR(250),
@address2 NVARCHAR(250),
@city NVARCHAR(250),
@county NVARCHAR(250),
@postcode NVARCHAR(15),
@country INT,
@capturePlusId NVARCHAR(40),
@userid INT

AS

DECLARE @auditInformation NVARCHAR(250) = ''; 
DECLARE @addressCount INT;
DECLARE @returnValue INT = 1;

SET @addressCount =  (SELECT COUNT(companyid) FROM companies WHERE 
       address1 = @address1 AND
       city = @city AND
       postcode = @postcode AND
       country = @country);
                          
IF (@addressCount = 0)
BEGIN
 INSERT INTO [companies] (company, CreatedOn, CreatedBy, address1,  address2,  city,  county,  postcode,  country,  AddressCreationMethod, CapturePlusId)
  VALUES     (@company ,  GETDATE(), @userid,   @address1, @address2, @city, @county, @postcode, @country, 1,                     @capturePlusId, NULL,          NULL);
  
 SET @returnValue = SCOPE_IDENTITY(); 

 IF LEN(@postcode) > 0 
  SET @auditInformation = @postcode + ' ';
 IF LEN(@address1) > 0
  SET @auditInformation = @auditInformation + @address1 + ' ';

 SET @auditInformation = @auditInformation + '(AddressId: ' + CAST(@returnValue as nvarchar(250)) + ')';

 IF @userid > 0
 BEGIN
  EXEC addInsertEntryToAuditLog @userid, 0, 38, @returnValue, @auditInformation, NULL;
 END
END
ELSE
BEGIN 
 UPDATE [companies] SET CapturePlusId = @capturePlusId WHERE 
 address1 = @address1 AND
    city = @city AND
    postcode = @postcode AND
    country = @country;
 
 SET @returnValue = (SELECT companyid FROM [companies] WHERE
      address1 = @address1 AND
      city = @city AND
      postcode = @postcode AND
      country = @country AND
      CapturePlusId = @capturePlusId);
END

RETURN @returnValue;