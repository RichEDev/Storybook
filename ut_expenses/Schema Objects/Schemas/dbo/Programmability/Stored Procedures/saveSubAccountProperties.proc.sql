CREATE PROCEDURE [dbo].[saveSubAccountProperties]
	@stringKey nvarchar(150),
	@stringValue nvarchar(MAX),
	@modifiedBy int,
	@subAccountID int,
	@employeeID INT,
	@delegateID INT
AS 
	declare @count int
	declare @oldvalue nvarchar(max)
	
	declare @isGlobal bit;
	select @isGlobal = CASE
		WHEN @stringKey = 'pwdConstraint' THEN 1
		WHEN @stringKey = 'pwdExpires' THEN 1
		WHEN @stringKey = 'pwdExpiryDays' THEN 1
		WHEN @stringKey = 'pwdHistoryNum' THEN 1
		WHEN @stringKey = 'pwdLength1' THEN 1
		WHEN @stringKey = 'pwdLength2' THEN 1
		WHEN @stringKey = 'pwdMaxRetries' THEN 1
		WHEN @stringKey = 'pwdMCN' THEN 1
		WHEN @stringKey = 'pwdMCU' THEN 1
		WHEN @stringKey = 'pwdSymbol' THEN 1
		ELSE 0
		END
	
	IF @isGlobal = 1
	BEGIN
		-- stop duplicate entries under other subAccountIDs
		DELETE FROM accountProperties WHERE stringKey = @stringKey;
	END

	DECLARE @elementId INT	

	IF @stringKey = 'CompanyPolicy' 
	SELECT @elementId = elementid from Elements where elements.elementFriendlyName = 'Company Policy'
	ELSE
	SELECT @elementId = elementid from Elements where elements.elementFriendlyName = 'General Options'

	set @count = (select count([stringkey]) from accountProperties where stringKey = @stringKey and subAccountId = @subAccountID);

	if @count = 0
	begin
		insert into accountProperties (subAccountID, stringKey, stringValue, isGlobal, createdOn, createdBy)
		values (@subAccountID, @stringKey, @stringValue, @isGlobal, getutcdate(), @modifiedBy);

		exec addInsertEntryToAuditLog @employeeID, @delegateID, @elementId, null, @stringKey, @subAccountID;
	end
	else
	begin
		select @oldvalue = stringValue from accountProperties where stringKey = @stringKey and subAccountId = @subAccountID;
		UPDATE accountProperties SET stringValue=@stringValue, modifiedBy=@modifiedBy, modifiedOn=getutcdate() WHERE stringKey=@stringKey AND subAccountID=@subAccountID;
		exec addUpdateEntryToAuditLog @employeeID, @delegateid, @elementId, null, null, @oldvalue, @stringValue, @stringKey, @subAccountID;
	end
	
	update accountsSubAccounts set CacheExpiry = getutcdate() where subAccountID = @subAccountID;

GO