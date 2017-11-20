CREATE PROCEDURE [dbo].[deleteLicenceRenewalType]
(
@ID INT, 
@employeeId INT,
@delegateID int
)
AS
DECLARE @description nvarchar(50);
DECLARE @subAccountId int;
DECLARE @cnt int;
DECLARE @cnt1 int;
DECLARE @cnt2 int;
SET @cnt = 0;
SET @cnt1 = 0;
SET @cnt2 = 0;

select @description = description, @subAccountId = subAccountId from licenceRenewalTypes where renewalType = @ID;

select @cnt1 = COUNT(productId) from productDetails where renewalType = @ID;
select @cnt2 = COUNT(licenceID) from productLicences where renewalType = @ID;
SET @cnt = @cnt1 + @cnt2;

IF @cnt > 0
	BEGIN
		RETURN -1;
	END
	
delete from licenceRenewalTypes where renewalType = @ID;

exec addDeleteEntryToAuditLog @employeeId, @delegateID, 115, @ID, @description, @subAccountId;

return @cnt
