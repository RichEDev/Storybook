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

	set @count = (select count([stringkey]) from accountProperties where stringKey = @stringKey and subAccountId = @subAccountID);

	if @count = 0
	begin
		insert into accountProperties (subAccountID, stringKey, stringValue, isGlobal, createdOn, createdBy)
		values (@subAccountID, @stringKey, @stringValue, @isGlobal, getdate(), @modifiedBy);

		exec addInsertEntryToAuditLog @employeeID, @delegateID, 34, null, @stringKey, @subAccountID;
	end
	else
	begin
		select @oldvalue = stringValue from accountProperties where stringKey = @stringKey and subAccountId = @subAccountID;
		UPDATE accountProperties SET stringValue=@stringValue, modifiedBy=@modifiedBy, modifiedOn=getdate() WHERE stringKey=@stringKey AND subAccountID=@subAccountID;
		exec addUpdateEntryToAuditLog @employeeID, @delegateid, 34, null, null, @oldvalue, @stringValue, @stringKey, @subAccountID;
	end
	
	update accountsSubAccounts set CacheExpiry = getdate() where subAccountID = @subAccountID;
