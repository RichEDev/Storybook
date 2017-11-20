CREATE PROCEDURE [dbo].[deleteProductLicenceType]
(
@ID INT, 
@employeeId INT,
@delegateID int
)
AS
DECLARE @description nvarchar(150);
DECLARE @subAccountId int;
DECLARE @cnt int;
SET @cnt = 0;

select @description = description, @subAccountId = subAccountId from codes_licencetypes where licenceTypeId = @ID;

select @cnt = COUNT(licenceID) from productLicences where licenceType = @ID;


IF @cnt > 0
	BEGIN
		RETURN -1;
	END

delete from codes_licencetypes where licenceTypeId = @ID;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 136, @ID, @description, @subAccountId;
